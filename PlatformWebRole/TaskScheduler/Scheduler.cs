using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TaskScheduler {
    class Scheduler {
        public void runTasks(Object intervalValue) {
            Trace.WriteLine("Running Scheduled Tasks", "Information");
            int interval = (int)intervalValue;
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            DateTime now = DateTime.Now.ToUniversalTime();
            DateTime past = DateTime.Now.AddMilliseconds(-interval).ToUniversalTime();
            List<ScheduledTask> tasks = db.ScheduledTasks.Where(x => (x.runtime != null && x.runtime.Value.TimeOfDay > past.TimeOfDay && x.runtime.Value.TimeOfDay <= now.TimeOfDay) || (x.runtime == null && (x.lastRan == null || x.lastRan.Value.AddMinutes(Convert.ToDouble(x.interval)).TimeOfDay <= now.TimeOfDay))).ToList<ScheduledTask>();
            Trace.WriteLine("Running " + tasks.Count + " Tasks", "Information");
            Logger.log("Running " + tasks.Count + " Tasks");
            foreach (ScheduledTask task in tasks) {
                Trace.WriteLine("Running Task: " + task.name, "Information");
                Logger.log("Running Task: " + task.name);
                task.Run();
            }
        }
    }
}
