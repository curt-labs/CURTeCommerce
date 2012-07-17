using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using Admin.Controllers;
using System.Web.Script.Serialization;

namespace Admin.Controllers {
    public class ProfileController : BaseController {

        public ActionResult Index(bool saved = false, bool failed = false) {
            Profile prof = new Profile();
            try {
                HttpCookie acct = new HttpCookie("acct");
                acct = Request.Cookies.Get("acct");
                prof = new JavaScriptSerializer().Deserialize<Profile>(acct.Value);
            } catch (Exception) {
                return RedirectToAction("Index", "Auth");
            }

            ViewBag.saved = saved;
            ViewBag.failed = failed;
            ViewBag.errors = TempData["errors"];
            ViewBag.hide_modules = true;
            ViewBag.prof = Profiles.GetProfile(prof.id);
            return View("Profile");
        }

        public ActionResult Edit(int id = 0, bool ulist = false) {
            List<string> errors = new List<string>();
            try {

                // Read the form data
                string first = (Request.Form["first"] != null)?Request.Form["first"]:"";
                string last = (Request.Form["last"] != null) ? Request.Form["last"] : "";
                string username = (Request.Form["username"] != null) ? Request.Form["username"] : "";
                string p1 = (Request.Form["p1"] != null) ? Request.Form["p1"].Trim() : "";
                string p2 = (Request.Form["p2"] != null) ? Request.Form["p2"].Trim() : "";
                string email = (Request.Form["email"] != null) ? Request.Form["email"] : "";
                string bio = (Request.Form["bio"] != null) ? Request.Form["bio"] : "";
                HttpPostedFileBase file = (Request.Files[0] != null)?Request.Files[0]:null;

                // Validate the form
                if (first.Length == 0) { errors.Add("First name is required."); }
                if (last.Length == 0) { errors.Add("Last name is required."); }
                if (username.Length == 0) { errors.Add("Username is required."); }
                if (email.Length == 0) { errors.Add("E-Mail is required."); }
                if (p1 != p2) {
                    p1 = "";
                }
                if (errors.Count > 0) {
                    throw new Exception();
                }
                Profiles.Add(id, username, p1, email, first, last, file, bio);
                if (!ulist) {
                    return RedirectToAction("Index", "Profile", new { saved = true });
                }

                Profile p = Profiles.GetProfileFromEmail(email);
                List<int> mods = (Request.Form["mod"] != null)?Request.Form["mod"].Split(',').Select(x => int.Parse(x)).ToList<int>():new List<int>();
                Profiles.DeleteProfileModules(p.id);
                Profiles.AddModules(p.id, mods);
                return RedirectToAction("Index", "Profiles");
            } catch (Exception e) {
                errors.Add(e.Message);
                TempData["errors"] = errors;
                return RedirectToAction("Index", "Profile", new { failed = true});
            }
        }

        public string DeleteProfileImage() {
            Profile prof = new Profile();
            try {
                HttpCookie acct = new HttpCookie("acct");
                acct = Request.Cookies.Get("acct");
                prof = new JavaScriptSerializer().Deserialize<Profile>(acct.Value);
                Profiles.DeleteProfileImage("",prof.id);

                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }

    }
}
