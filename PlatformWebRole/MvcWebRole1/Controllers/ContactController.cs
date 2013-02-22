using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using Newtonsoft.Json;

namespace EcommercePlatform.Controllers {
    public class ContactController : BaseController {

        public ActionResult Index(string message = "", bool hide_layout = false) {

            // Get all the Contact Types
            List<ContactType> types = ContactType.GetAll();
            ViewBag.types = types;

            // Get the contact ContentPage
            ContentPage page = ContentManagement.GetPageByTitle("contact");
            ViewBag.page = page;

            if (TempData["error"] != null) {
                message = TempData["error"].ToString();
            }
            ViewBag.message = message;

            ContactInquiry inquiry = new ContactInquiry();
            if (TempData["inquiry"] != null) {
                try{
                    inquiry = (ContactInquiry)TempData["inquiry"];
                }catch(Exception){}
            }
            ViewBag.inquiry = inquiry;
            ViewBag.hide_layout = hide_layout;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Send(string name = null, string phone = null, string email = null, int contact_type = 0, string message = null) {

            ContactInquiry inq = new ContactInquiry();
            try {
                inq = new ContactInquiry {
                    name = name,
                    phone = phone,
                    email = email,
                    contact_type = contact_type,
                    message = message,
                    dateAdded = DateTime.UtcNow,
                    followedUp = 0
                };
                bool recaptchavalid = ReCaptcha.ValidateCaptcha(Request.Form["recaptcha_challenge_field"], Request.Form["recaptcha_response_field"]);
                if (!recaptchavalid) throw new Exception("Captcha Incorrect!");

                UDF.Sanitize(inq, new string[] { "phone", "followedUp" });
                inq.Save();
                TempData["error"] = "Thank you for your inquiry, someone will contact you soon to follow up with your request.";
                return RedirectToAction("Index", "Contact");
            } catch (Exception e) {
                TempData["inquiry"] = inq;
                TempData["error"] = e.Message;
                return RedirectToAction("Index", "Contact");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SendAJAX(string name = null, string phone = null, string email = null, string to = null, string message = null, string recaptcha_challenge_field = null, string recaptcha_response_field = null) {
            ContactInquiry inq = new ContactInquiry();
            try {
                inq = new ContactInquiry {
                    name = name,
                    phone = phone,
                    email = email,
                    contact_type = 0,
                    message = message,
                    dateAdded = DateTime.UtcNow,
                    followedUp = 0
                };
                bool recaptchavalid = ReCaptcha.ValidateCaptcha(recaptcha_challenge_field, recaptcha_response_field);
                if (!recaptchavalid) throw new Exception("Captcha Incorrect!");

                UDF.Sanitize(inq, new string[] { "phone", "contact_type", "followedUp" });
                inq.Save(true, to);
                TempData["error"] = "Thank you for your contact inquiry. Someone will respond to your request in a timely matter.";
                return RedirectToAction("Index","Locations");
            } catch (Exception e) {
                TempData["error"] = "Sorry, an error has occurred while processing your request." + e.Message;
                return RedirectToAction("Index", "Locations");
            }
        }

    }
}
