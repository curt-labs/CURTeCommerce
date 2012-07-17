using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers {
    public class NewsletterController : BaseController {

        public ActionResult Index() {

            // Get all the subscriptions
            List<Newsletter> subs = NewsletterFunctions.GetAll();
            ViewBag.subs = subs;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string Delete(int id = 0) {
            return NewsletterFunctions.Delete(id);
        }

    }
}
