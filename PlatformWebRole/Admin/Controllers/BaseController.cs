using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using System.Web.Script.Serialization;
using System.Web.Routing;

namespace Admin.Controllers {
    public class BaseController : Controller {

        private static string[] static_pages = { "/ADMIN/PROFILE", "/ADMIN/BUG", "/ADMIN/SEARCH" };

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {

            
            // Attempt to retrieve user information from the cookie
            try {

                // Get the cookie
                HttpCookie acct = new HttpCookie("acct");
                acct = Request.Cookies.Get("acct");

                // Deserialize cookie value
                Profile p = new Profile();
                p = new JavaScriptSerializer().Deserialize<Profile>(acct.Value);

                // Validate the cookie data
                if (p == null || p.email == null || p.email.Length == 0 || p.id == 0) {
                    throw new Exception();
                }
                Settings settings = new Settings();
                ViewBag.settings = settings;

                // We need to attempt to get the active module
                string path = HttpContext.Request.Url.AbsolutePath;
                ViewBag.active_path = path;
                string[] path_parts = path.Trim().Split('/');
                string cleaned_path = "";
                foreach (string part in path_parts.Where(x => x.Length > 0)) {
                    try {
                        Int32.Parse(part);
                        break;
                    } catch (Exception) {
                        cleaned_path += "/" + part;
                    }
                }

                List<GroupedModule> mods = Profiles.GetProfileModules(p.id);
                ViewBag.modules = mods;

                bool validate = true;
                object[] attributes = filterContext.ActionDescriptor.GetCustomAttributes(false);
                foreach (object attr in attributes) {
                    if (attr.GetType() == typeof(NoValidationAttribute)) {
                        validate = false;
                    }
                }

                // Now we need to make sure the current profile has access to the module that they are loading

                if (cleaned_path.ToUpper() != "/ADMIN" && !static_pages.Contains(cleaned_path.ToUpper()) && HttpContext.Request.HttpMethod.ToUpper() != "POST" && validate) {
                    GroupedModule active_mod = (from m in mods
                                                where m.path.ToUpper().Equals(cleaned_path.ToUpper()) && m.hasAccess.Equals(1)
                                                select m).FirstOrDefault<GroupedModule>();
                    if (active_mod == null) {
                        // Check the sub mods
                        Module active_sub = null;
                        foreach (GroupedModule m in mods) {
                            active_sub = (from s in m.subs
                                          where s.path.ToUpper().Equals(cleaned_path.ToUpper())
                                          select s).FirstOrDefault<Module>();
                            if (active_sub != null) {
                                break;
                            }
                        }
                        if (active_sub == null) {
                            Response.Redirect("/Admin");
                        }
                    }
                }

                
                ViewBag.profile = p;
            } catch (Exception) { // We ran into issues validating, require the users to log back in
                HttpContext.Response.Redirect("/Admin/Auth");
            }
            base.OnActionExecuting(filterContext);
        }

    }

    class NoValidationAttribute : Attribute {

    }
}
