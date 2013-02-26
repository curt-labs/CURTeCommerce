using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers {
    public class ThemesController : BaseController {

        public ActionResult Index() {
            
            List<Theme> themes = new Theme().GetAll();
            ViewBag.themes = themes;

            return View();
        }

        public ActionResult Add(string error = "") {
            ViewBag.error = error;
            return View("Edit");
        }

        public ActionResult Edit(int id = 0, string error = "") {
            Theme theme = new Theme().Get(id);
            ViewBag.theme = theme;
            ViewBag.error = error;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(int id = 0) {
            Theme theme = new Theme { ID = id };
            string name = Request.Form["name"].ToString().Trim();
            string screenshot = Request.Form["screenshot"].ToString().Trim();
            if (string.IsNullOrWhiteSpace(screenshot))
                screenshot = null;
            if (string.IsNullOrWhiteSpace(name)) {
                string error = "A theme name is required";
                if (id > 0)
                    return RedirectToAction("Edit", new { id = id, error = error });
                else
                    return RedirectToAction("Add", new { error = error });
            }
            try {
                theme.Save(name, screenshot);
            } catch (Exception e) {
                if (id > 0)
                    return RedirectToAction("Edit", new { id = id, error = e.Message });
                else
                    return RedirectToAction("Add", new { error = e.Message });
            }
            return RedirectToAction("Edit", new { id = theme.ID });
        }
    }
}
