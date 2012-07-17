using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Controllers;
using Admin.Models;
using Newtonsoft.Json;

namespace Admin.Controllers {
    public class SchedulerController : BaseController {
        //
        // GET: /Index/

        public ActionResult Index() {
            List<ScheduledTask> tasks = new ScheduledTask().getTasks();
            ViewBag.tasks = tasks;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string DeleteTask(int id = 0) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                ScheduledTask t = db.ScheduledTasks.Where(x => x.ID.Equals(id)).First<ScheduledTask>();
                db.ScheduledTasks.DeleteOnSubmit(t);
                db.SubmitChanges();
                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddTask(string name = "", string runtime = "", int interval = 0,string url = "") {
            try {
                if (url.Trim() == "") {
                    throw new Exception("Task must have a path.");
                }
                if (runtime.Trim() == "" && interval < 1) {
                    throw new Exception("Task must have a run time or an interval greater than 5 minutes.");
                }

                ScheduledTask s = new ScheduledTask {
                    name = name,
                    url = url
                };
                if (runtime.Trim() != "") {
                    DateTime rtime = Convert.ToDateTime(runtime).ToUniversalTime();
                    s.runtime = rtime;
                } else if(interval > 1) {
                    s.interval = interval;
                }
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                db.ScheduledTasks.InsertOnSubmit(s);
                db.SubmitChanges();
            } catch {}
            return RedirectToAction("index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string RunTask(int id) {
            try {
                ScheduledTask s = new ScheduledTask().Get(id);
                s.Run();
                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }

    }
}
