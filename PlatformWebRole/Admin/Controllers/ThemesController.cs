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
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string Delete(int id = 0) {
            bool success = new Theme().Delete(id);
            if(success) {
                return "{\"success\":true}";
            } else {
                return "{\"success\":false}";
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string Activate(int id = 0) {
            bool success = new Theme().Activate(id);
            if (success) {
                return "{\"success\":true}";
            } else {
                return "{\"success\":false}";
            }
        }

        public ActionResult Files(int id = 0) {
            Theme theme = new Theme().Get(id);
            ViewBag.theme = theme;

            List<ThemeArea> areas = new ThemeArea().GetAll();
            ViewBag.areas = areas;

            List<ThemeFileType> types = new ThemeFileType().GetAll();
            ViewBag.types = types;

            return View();
        }

        public ActionResult Area(int themeID, int areaID) {
            Theme theme = new Theme().Get(themeID);
            ViewBag.theme = theme;

            List<ThemeFile> files = theme.ThemeFiles.Where(x => x.themeAreaID.Equals(areaID)).ToList();
            ViewBag.files = files;

            ThemeArea area = files.Select(x => x.ThemeArea).FirstOrDefault();
            ViewBag.area = area;

            List<ThemeFileType> types = new ThemeFileType().GetAll();
            ViewBag.types = types;

            return View();
        }
    }
}
