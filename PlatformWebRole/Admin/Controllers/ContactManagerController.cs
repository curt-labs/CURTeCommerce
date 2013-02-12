using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using Newtonsoft.Json;

namespace Admin.Controllers {
    public class ContactManagerController : BaseController {

        public ActionResult Index() {

            // Get all the contact inquiries
            List<ContactInquiry> inquiries = ContactInquiry.GetAll();
            ViewBag.inquiries = inquiries;

            // Get all the contact types
            List<ContactType> types = ContactType.GetAll();
            ViewBag.types = types;

            return View();
        }

        [NoValidation]
        public string AddType(string label = "", string email = "") {
            try {
                ContactType type = new ContactType {
                    label = label,
                    email = email
                };
                type.Add();
                return JsonConvert.SerializeObject(type);
            } catch (Exception e) {
                return e.Message;
            }
        }

        [NoValidation]
        public string Get(int id) {
            Admin.Profile profile = ViewBag.profile ?? new Admin.Profile();
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(profile.timezone ?? "UTC");
            try {
                ContactInquiry inq = new ContactInquiry{ ID = id };
                ContactInquiry resp = inq.Get();
                SimpleInquiry simple = new SimpleInquiry {
                    ID = resp.ID,
                    name = resp.name,
                    phone = resp.phone,
                    message = resp.message,
                    email = resp.email,
                    type = (resp.ContactType != null && resp.ContactType.label != null) ? resp.ContactType.label : "N/A",
                    dateAdded = String.Format("{0:M/dd/yyyy} at {0:h:mm tt}", TimeZoneInfo.ConvertTimeFromUtc(resp.dateAdded, tz)),
                    followedUp = resp.followedUp
                };

                return JsonConvert.SerializeObject(simple);
            } catch (Exception e) {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                Response.StatusDescription = e.Message;
                Response.Write(e.Message);
                return e.Message;
            }
        }

        [NoValidation]
        public void RemoveInquiry(int id) {
            try {
                ContactInquiry inq = new ContactInquiry { ID = id };
                inq.Delete();
            } catch (Exception e) {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                Response.StatusDescription = e.Message;
                Response.Write(e.Message);
            }
        }

        [NoValidation]
        public void MarkResponded(int id) {
            try {
                ContactInquiry inq = new ContactInquiry { ID = id };
                inq.MarkResponded();
            } catch (Exception e) {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                Response.StatusDescription = e.Message;
                Response.Write(e.Message);
            }
        }

        public void DeleteType(int id = 0) {
            try {
                ContactType type = new ContactType { ID = id };
                type.Delete();
            } catch (Exception e) {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                Response.StatusDescription = e.Message;
                Response.Write(e.Message);
            }
        }

    }
}
