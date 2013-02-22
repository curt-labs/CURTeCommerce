using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using System.Web.Script.Serialization;

namespace EcommercePlatform.Controllers {
    public class TestimonialsController : BaseController {

        public ActionResult Index(string message = "", int page = 1, int pageSize = 10) {
            ViewBag.timezone = UDF.GetTimeZone();

            List<string> errors = (List<string>)TempData["errors"];
            ViewBag.first_name = ((string)TempData["first_name"] != null) ? (string)TempData["first_name"] : "";
            ViewBag.last_name = ((string)TempData["last_name"] != null) ? (string)TempData["last_name"] : "";
            ViewBag.location = ((string)TempData["location"] != null) ? (string)TempData["location"] : "";
            ViewBag.testimonial = ((string)TempData["testimonial"] != null) ? (string)TempData["testimonial"] : "";
            ViewBag.errors = (errors == null) ? new List<string>() : errors;
            ViewBag.message = message;

            // Get all the FAQs listed alphabetically by question
            List<Testimonial> testimonials = TestimonialModel.GetAll(page, pageSize);
            ViewBag.testimonials = testimonials;

            int total = TestimonialModel.CountAll();

            decimal pagecount = Math.Ceiling(Convert.ToDecimal(total) / Convert.ToDecimal(pageSize));
            ViewBag.pagecount = Convert.ToInt32(pagecount);
            ViewBag.page = page;

            return View();
        }

        public ActionResult Add(string message = "") {
            ViewBag.testimonial = (Testimonial)TempData["testimonial"] ?? new Testimonial();
            ViewBag.message = message;

            // Get the contact ContentPage
            ContentPage page = ContentManagement.GetPageByTitle("Add a Testimonial");
            ViewBag.page = page;
            
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Add(string first_name = "", string last_name = "", string location = "", string testimonial = "") {
            Testimonial t = new Testimonial {
                testimonial1 = testimonial,
                first_name = (first_name.Trim() == "") ? null : first_name.Trim(),
                last_name = (last_name.Trim() == "") ? null : last_name.Trim(),
                location = (location.Trim() == "") ? null : location.Trim()
            };
            string message = "";
            try {
                bool recaptchavalid = ReCaptcha.ValidateCaptcha(Request.Form["recaptcha_challenge_field"], Request.Form["recaptcha_response_field"]);
                if (!recaptchavalid) message += "Captcha was incorrect. ";
                if (testimonial.Trim() == "") message += "A Testimonial is required.";

                if (message.Length > 0) throw new Exception(message);
                t = TestimonialModel.Add(first_name, last_name, location, testimonial);
                return RedirectToAction("Add", "Testimonials", new { message = "Thanks for submitting a Testimonial!" });

            } catch (Exception e) { 
                message = e.Message;
                TempData["testimonial"] = t;
            }
            return RedirectToAction("Add", "Testimonials", new { message = message });
        }
    }
}
