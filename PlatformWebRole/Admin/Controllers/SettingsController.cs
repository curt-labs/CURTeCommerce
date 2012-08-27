using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Controllers;
using Admin.Models;
using Newtonsoft.Json;

namespace Admin.Controllers {
    public class SettingsController : BaseController {
        //
        // GET: /Index/

        public ActionResult Index() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<SettingGroup> groups = db.SettingGroups.ToList<SettingGroup>();
            ViewBag.groups = groups;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Setting> settinglist = db.Settings.ToList<Setting>();
            foreach (Setting s in settinglist) {
                try {
                    s.value = Request.Form[s.name].Trim();
                } catch {};
            }
            db.SubmitChanges();
            return RedirectToAction("index");
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public string CreateGroup(string name = "") {
            try {
                if(name.Trim() == "") {
                    throw new Exception("Setting Group must have a name.");
                }
                SettingGroup g = new SettingGroup {
                    name = name
                };
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                if (db.SettingGroups.Where(x => x.name.ToLower().Trim().Equals(name.Trim().ToLower())).Count() == 0) {
                    db.SettingGroups.InsertOnSubmit(g);
                    db.SubmitChanges();
                } else {
                    throw new Exception("A setting group called " + name + " already exists");
                }
                return JsonConvert.SerializeObject(g);
            } catch (Exception e) {
                return e.Message;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string DeleteGroup(int id = 0) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                SettingGroup g = db.SettingGroups.Where(x => x.settingGroupID.Equals(id)).First<SettingGroup>();
                db.SettingGroups.DeleteOnSubmit(g);
                db.SubmitChanges();
                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string AddSetting(string sname = "", int gid = 0, bool isImage = false) {
            try {
                if (sname.Trim() == "") {
                    throw new Exception("Setting must have a name.");
                }
                Setting s = new Setting {
                    name = sname,
                    groupID = gid,
                    isImage = isImage,
                };
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                if (db.Settings.Where(x => x.name.ToLower().Trim().Equals(sname.ToLower().Trim())).Count() == 0) {
                    db.Settings.InsertOnSubmit(s);
                    db.SubmitChanges();
                } else {
                    throw new Exception("A Setting named " + sname + " already exists!");
                }
                return JsonConvert.SerializeObject(s);
            } catch (Exception e) {
                return e.Message;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string DeleteSetting(string name = "") {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                Setting s = db.Settings.Where(x => x.name.ToLower().Equals(name.Trim().ToLower())).First<Setting>();
                db.Settings.DeleteOnSubmit(s);
                db.SubmitChanges();
                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }

    }
}
