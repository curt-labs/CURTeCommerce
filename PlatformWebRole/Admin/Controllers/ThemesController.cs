using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using Newtonsoft.Json;

namespace Admin.Controllers {
    public class ThemesController : BaseController {

        public ActionResult Index() {
            
            List<Theme> themes = new Theme().GetAll();
            ViewBag.themes = themes;

            int activeTheme = new Theme().getActiveCookie();
            ViewBag.activeTheme = activeTheme;

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

        [AcceptVerbs(HttpVerbs.Post)]
        public string Preview(int id = 0) {
            bool success = new Theme().Preview(id);
            if (success) {
                return "{\"success\":true}";
            } else {
                return "{\"success\":false}";
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string EndPreview(int id = 0) {
            bool success = new Theme().EndPreview(id);
            if (success) {
                return "{\"success\":true}";
            } else {
                return "{\"success\":false}";
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string Duplicate(int id = 0) {
            Theme theme = new Theme().Duplicate(id);
            return JsonConvert.SerializeObject(theme);
        }

        public ActionResult Files(int id = 0) {
            Theme theme = new Theme().Get(id);
            ViewBag.theme = theme;

            int activeTheme = new Theme().getActiveCookie();
            ViewBag.activeTheme = activeTheme;

            List<ThemeArea> areas = new ThemeArea().GetAll();
            ViewBag.areas = areas;

            List<ThemeFileType> types = new ThemeFileType().GetAll();
            ViewBag.types = types;

            return View();
        }

        [NoValidation]
        public string AreaFiles(int themeID, int areaID, int typeID) {
            ThemeDetails deets = new ThemeDetails {
                themeID = themeID,
                type = new ThemeFileType().Get(typeID),
                area = new ThemeArea().Get(areaID),
                files = new ThemeFile().GetAllFiles(themeID, areaID, typeID)
            };

            return JsonConvert.SerializeObject(deets);
        }

        public ActionResult AddFile(int themeID, int areaID, int typeID) {
            Theme theme = new Theme().Get(themeID);
            ViewBag.theme = theme;

            ThemeArea area = new ThemeArea().Get(areaID);
            ViewBag.area = area;

            ThemeFileType type = new ThemeFileType().Get(typeID);
            ViewBag.type = type;

            return View();
        }

        public ActionResult EditFile(int id) {
            ThemeFile file = new ThemeFile().Get(id);
            ViewBag.file = file;

            Theme theme = file.Theme;
            ViewBag.theme = theme;

            ThemeArea area = file.ThemeArea;
            ViewBag.area = area;

            ThemeFileType type = file.ThemeFileType;
            ViewBag.type = type;

            return View("AddFile");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string SaveFile(int themeID, int areaID, int typeID, string content, string name, int fileID = 0, bool externalFile = false) {
            ThemeFile file = new ThemeFile().Save(fileID, themeID, areaID, typeID, content, name, externalFile);
            return JsonConvert.SerializeObject(file);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string DeleteFile(int id = 0) {
            bool success = new ThemeFile().Delete(id);
            if (success) {
                return "{\"success\":true}";
            } else {
                return "{\"success\":false}";
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string updateFileSort() {
            List<string> filesort = Request.QueryString["file[]"].Split(',').ToList<string>();
            new ThemeFile().updateSort(filesort);
            return "";
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string Upload(int themeID, int areaID, int typeID) {
            ThemeFile file = new ThemeFile();
            try {
                string filename = HttpContext.Request.Headers["X-File-Name"];
                string filetype = HttpContext.Request.Headers["X-File-Type"];
                Stream input = Request.InputStream;
                file = new ThemeFile().Upload(input, filename, filetype, themeID, areaID, typeID);
            } catch {}
            return JsonConvert.SerializeObject(file);
        }
    }
    public class ThemeDetails {
        public int themeID { get; set; }
        public ThemeArea area { get; set; }
        public ThemeFileType type { get; set; }
        public List<ThemeFile> files { get; set; }
    }
}
