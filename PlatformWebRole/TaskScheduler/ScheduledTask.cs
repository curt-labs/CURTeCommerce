using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;

namespace TaskScheduler {
    partial class ScheduledTask {
        public void Run() {
            string url = this.url;
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            ScheduledTask t = db.ScheduledTasks.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<ScheduledTask>();
            t.lastRan = DateTime.Now.ToUniversalTime();
            db.SubmitChanges();

            WebClient wc = new WebClient();
            wc.Proxy = null;
            wc.DownloadString(url);
        }

    }
}
