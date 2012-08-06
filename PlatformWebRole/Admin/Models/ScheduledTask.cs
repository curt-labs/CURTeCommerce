using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Admin {
    partial class ScheduledTask {
        public List<ScheduledTask> getTasks() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<ScheduledTask> tasks = db.ScheduledTasks.ToList<ScheduledTask>();
            return tasks;
        }

        public ScheduledTask Get(int id) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            ScheduledTask task = db.ScheduledTasks.Where(x => x.ID.Equals(id)).FirstOrDefault<ScheduledTask>();
            return task;
        }

        public void Run() {
            string url = this.url;
            url += "?lastRan=" + ((this.lastRan != null) ? String.Format("{0:MMddyyyyHHmmss}", this.lastRan) : "");
            WebClient wc = new WebClient();
            wc.Proxy = null;
            wc.DownloadString(url);

            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            ScheduledTask t = db.ScheduledTasks.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<ScheduledTask>();
            t.lastRan = DateTime.Now;
            db.SubmitChanges();
        }

    }
}
