using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers {
    public class BugController : BaseController {
        //
        // GET: /Bug/

        public ActionResult Index(bool failed = false) {
            ViewBag.failed = failed;
            return View();
        }

        public ActionResult Submit() {
            try {
                string name = (Request.Form["name"] != null) ? Request.Form["name"] : "";
                string email = (Request.Form["email"] != null) ? Request.Form["email"] : "";
                string url = (Request.Form["url"] != null) ? Request.Form["url"] : "";
                string subj = (Request.Form["subject"] != null) ? Request.Form["subject"] : "";
                string desc = (Request.Form["desc"] != null) ? Request.Form["desc"] : "";

                try {
                    UDF.SubmitBug(name, email, desc, url, subj);
                } catch (Exception) {
                    return RedirectToAction("Index", "Bug", new { failed = true });
                }

                return View();
            } catch (Exception) {
                return RedirectToAction("Index", "Bug", new { failed = true });
            }
        }

    }
}
