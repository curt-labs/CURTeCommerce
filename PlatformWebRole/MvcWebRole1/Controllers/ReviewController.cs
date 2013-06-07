using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class ReviewController : BaseController {
        public ActionResult Index() {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> New(int id = 0, bool hide_layout = false, string name = "", string email = "", string subject = "", string review = "", int rating = 5, string err = "") {

            var pcats = CURTAPI.GetParentCategoriesAsync();
            await Task.WhenAll(new Task[] { pcats });
            ViewBag.parent_cats = await pcats;

            APIPart part = CURTAPI.GetPart(id);
            ViewBag.part = part;

            ViewBag.partID = id;
            ViewBag.hide_layout = hide_layout;
            ViewBag.name = name;
            ViewBag.email = email;
            ViewBag.subject = subject;
            ViewBag.rating = rating;
            ViewBag.review = review;
            ViewBag.err = err;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Submit(int id = 0, string name = "", string email = "", string subject = "", int rating = 5, string review = "") {
            try {
                if (!(ReCaptcha.ValidateCaptcha(System.Web.HttpContext.Current, Request.Form["recaptcha_challenge_field"], Request.Form["recaptcha_response_field"]))) {
                    throw new Exception("Recaptcha Validation Failed.");
                }
                CURTAPI.SubmitReview(id, rating, subject, review, name, email);
                return Redirect("/Part/" + id);
            } catch (Exception e) {
                return RedirectToAction("New", "Review", new { id = id, hide_layout = false, name = name, email = email, subject = subject, rating = rating, review = review, err = e.Message });
            }
        }

    }
}
