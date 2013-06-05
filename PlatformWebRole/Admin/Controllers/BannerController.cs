using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers {
    public class BannerController : BaseController {

        public ActionResult Index() {

            List<Banner> banners = new Banner().GetAll();
            ViewBag.banners = banners;

            return View();
        }

        public ActionResult Edit(int id = 0, string error = "") {
            Banner banner = new Banner();
            banner.ID = id;
            if (id > 0) {
                banner.Get();
            }
            if (TempData["banner"] != null) {
                try {
                    banner = (Banner)TempData["banner"];
                } catch (Exception) { }
            }
            ViewBag.banner = banner;
            ViewBag.banner_count = banner.GetAll().Count;

            ViewBag.error = error;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(int id = 0, string title = "", string image = "", string body = "", string link = "", int isVisible = 0) {
            Banner banner = new Banner();
            try {
                banner = new Banner {
                    ID = id,
                    title = title,
                    image = image,
                    body = body,
                    link = link,
                    isVisible = isVisible
                };
                UDF.Sanitize(banner, new string[] { "body", "link" });
                banner.Save();

                return RedirectToAction("Index", "Banner");
            } catch (Exception e) {
                TempData["banner"] = banner;
                return RedirectToAction("Edit", "Banner", new { id = id, error = e.Message });
            }
        }

        [NoValidation,AcceptVerbs(HttpVerbs.Post)]
        public void updateSort() {
            List<string> banners = Request.QueryString["banner[]"].Split(',').ToList<string>();
            Banner.Sort(banners);
        }

        [NoValidation]
        public string Delete(int id = 0) {
            try {
                Banner banner = new Banner {
                    ID = id
                };
                banner.Delete();

                return "";
            } catch (Exception e) {
                return "There was an error while processing your request: " + e.Message;
            }
        }

    }
}
