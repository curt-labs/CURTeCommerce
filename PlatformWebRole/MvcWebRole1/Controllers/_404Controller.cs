using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class _404Controller : Controller {

        /*public ActionResult Index() {
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
        }*/

        public ActionResult Http404() {
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
            //return Content("404", "text/plain");
        }

        public ActionResult Http500() {
            Response.StatusCode = 500;
            Response.StatusDescription = "Internal Server Error";
            return Content("500", "text/plain");
        }

        public ActionResult Http403() {
            Response.StatusCode = 403;
            Response.StatusDescription = "Forbidden";
            return Content("403", "text/plain");
        }
    }
}
