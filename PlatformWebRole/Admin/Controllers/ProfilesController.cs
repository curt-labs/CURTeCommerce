using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers {
    public class ProfilesController : BaseController {

        public ActionResult Index() {

            // Get all the profiles
            ViewBag.profiles = Profiles.GetAll();

            return View();
        }

        public ActionResult Add(bool failed = false) {
            try {
                ViewBag.listed_modules = Profiles.GetModules();
                ViewBag.hide_modules = false;
            } catch (Exception) { }

            ViewBag.timezones = UDF.GetTimeZones();

            ViewBag.ulist = true;
            ViewBag.failed = failed;
            return View("Profile");
        }

        public ActionResult Edit(int id = 0, bool failed = false) {

            try {
                // Get the profile
                Profile prof = Profiles.GetProfile(id);
                ViewBag.prof = prof;

                ViewBag.timezones = UDF.GetTimeZones();

                // Get the modules for this profile
                List<GroupedModule> user_mods = Profiles.GetProfileModules(prof.id);
                ViewBag.user_modules = user_mods;

                ViewBag.listed_modules = Profiles.GetModules();
                ViewBag.hide_modules = false;
            } catch (Exception) {
                
                throw;
            }

            ViewBag.failed = failed;
            ViewBag.errors = TempData["errors"];
            ViewBag.ulist = true;
            return View("Profile");
        }

        public ActionResult Delete(int id = 0) {
            try {

                Profiles.DeleteProfile(id);
                return RedirectToAction("Index", "Profiles");
            } catch (Exception e) {
                List<string> errs = new List<string>();
                errs.Add(e.Message);
                TempData["errors"] = errs;
                return RedirectToAction("Edit", "Profiles", new { id = id, failed = true });
            }
        }

    }
}
