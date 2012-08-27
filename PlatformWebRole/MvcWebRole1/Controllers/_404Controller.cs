using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class _404Controller : BaseController {

        public ActionResult Index() {
            try {
                string errorpath = Request.QueryString["aspxerrorpath"];
                if (errorpath.LastIndexOf("/") != -1 && errorpath.LastIndexOf("/") == 0) {
                    string name = errorpath.Substring(1);
                    int page = ContentManagement.GetPageID(errorpath);
                    if (page > 0) {
                        return RedirectToAction("Page", "Index", new { id = page });
                    }
                }
            } catch (Exception e) {
                string message = e.Message;
            }

            return View();
        }
    }
}
