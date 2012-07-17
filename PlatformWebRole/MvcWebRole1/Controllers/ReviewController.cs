using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class ReviewController : BaseController {
        public ActionResult Index() {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult New(int id = 0, bool hide_layout = false, string name = "", string email = "", string subject = "", string review = "", int rating = 5, string err = "") {

            APIPart part = CURTAPI.GetPart(id);
            ViewBag.part = part;

            ViewBag.partID = id;
            ViewBag.hide_layout = hide_layout;
            ViewBag.name = name;
            ViewBag.email = email;
            ViewBag.subject = subject;
            ViewBag.rating = rating;
            ViewBag.review = review;

            return View();
        }

        public ActionResult Submit(int id = 0, string name = "", string email = "", string subject = "", int rating = 5, string review = "") {
            try {
                CURTAPI.SubmitReview(id, rating, subject, review, name, email);
                return Redirect("/Part/" + id);
            } catch (Exception e) {
                return RedirectToAction("New", "Review", new { id = id, hide_layout = false, name = name, email = email, subject = subject, rating = rating, review = review, err = e.Message });
            }
        }

    }
}
