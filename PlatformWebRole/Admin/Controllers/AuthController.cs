using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using System.Web.Script.Serialization;

namespace Admin.Controllers {
    public class AuthController : Controller {

        public ActionResult Index(bool failed = false, bool generated_forgotton = false) {
            ViewBag.generated_forgotten = generated_forgotton;
            ViewBag.failed = failed;
            return View();
        }

        public ActionResult FirstUse() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            try {
                Profile p = db.Profiles.Where(x => x.username.Equals("admin")).First<Profile>();
                if (p.password == "") {
                    p.password = Crypto.EncryptString("admin");
                    db.SubmitChanges();
                }
            } catch { };
            return RedirectToAction("Index");
        }

        public ActionResult In() {
            try {
                string username = Request.Form["username"];
                string password = Request.Form["password"];

                Profile p = Profiles.GetProfile(0, username, password);
                if (p == null || p.id == 0) {
                    throw new Exception();
                }
                p.password = "Ya'll suckas got ketchuped!";
                
                Profile serial_prof = new Profile {
                    id = p.id,
                    username = p.username,
                    password = p.password,
                    email = p.email,
                    first = p.first,
                    last = p.last,
                    date_added = p.date_added,
                    image = p.image,
                    bio = p.bio
                };
                string jsonProf = new JavaScriptSerializer().Serialize(serial_prof);
                HttpCookie acct = new HttpCookie("acct");
                acct.Value = jsonProf;
                acct.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(acct);

                return Redirect("/admin");
            } catch (Exception e) {
                return RedirectToAction("Index", "Auth", new { failed = true });
            }
        }

        public dynamic Out() {
            try {
                Response.Cookies["acct"].Expires = DateTime.Now.AddYears(-100);

                return RedirectToAction("Index", "Auth");
            } catch (Exception) {
                return RedirectToAction("Index", "Auth");
            }
        }

        public ActionResult Forgot(bool failed = false, string message = "", string email = "") {
            ViewBag.failed = failed;
            return View();
        }

        public dynamic ForgotSubmission() {
            string email = "";
            try {
                email = Request.Form["email"];
                if (email == null || email.Length == 0) {
                    throw new Exception("E-Mail is empty");
                }

                Profile p = Profiles.GetProfileFromEmail(email);
                if (p != null) {
                    Profiles.SendForgotten(p);
                } else {
                    throw new Exception("Profile was not found.");
                }
                return RedirectToAction("Index", "Auth", new { generated_forgotten = true });
            } catch (Exception e) {
                return RedirectToAction("Forgot", "Auth", new { failed = true, message = e.Message + e.Source + e.InnerException, email = email });
            }
        }
    }
}
