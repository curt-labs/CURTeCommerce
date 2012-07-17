using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class _404Controller : BaseController {

        public ActionResult Index() {
            string path = "";
            if (Request.QueryString.ToString().StartsWith("/")) {
                path = Request.QueryString.ToString().Substring(1);
            } else {
                path = Request.QueryString.ToString();
            }
            int page = ContentManagement.GetPageID(path);
            if (page > 0) {
                return RedirectToAction("Page", "Index", new { id = page });
            }
            return View();
        }

    }
}
