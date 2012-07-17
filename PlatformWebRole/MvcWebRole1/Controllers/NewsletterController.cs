using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class NewsletterController : BaseController {

        /// <summary>
        /// View the Newsletter
        /// </summary>
        /// <returns>HTML View</returns>
        public ActionResult Index(string message = "") {
            ViewBag.page = ContentManagement.GetPageByTitle("newsletter");
            ViewBag.message = message;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SignUp(string name = "", string email = "") {
            bool success = false;
            string message = "We're sorry but we failed to add you to the mailing list for the Newsletter. ";
            try {
                if (name.Length == 0) { throw new Exception("You must enter your full name."); }
                if (email.Length == 0) { throw new Exception("You must enter your e-mail address."); }

                NewsletterFunctions.Add(name, email);
                success = true;
                message = "You have been successfully added to the mailing list for the Newsletter.";

                ViewBag.page = ContentManagement.GetPageByTitle("newsletter");
            } catch (Exception e) { }
            ViewBag.message = message;
            ViewBag.success = success;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Unsubscribe(Guid unsub) {
            string message = "We were unable to unsubscribe you from the newsletter";
            try {
                NewsletterFunctions.Unsubscribe(unsub);
                message = "We were able to remove your subscription and you will no longer recieve our newsletter.";
            } catch (Exception) { }
            return RedirectToAction("Index", "Newsletter", new { message = message });
        }
    }
}
