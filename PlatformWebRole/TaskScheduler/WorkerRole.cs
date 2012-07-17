using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace TaskScheduler {
    public class WorkerRole : RoleEntryPoint {
        public override void Run() {
            // This is a sample worker implementation. Replace with your logic.
            Scheduler scheduler = new Scheduler();
            int sleepMinutes = 1;
            int sleepInterval = sleepMinutes * 60 * 1000;

            while (true) {
                Thread.Sleep(sleepInterval);
                Logger.log("Checking Tasks");
                Trace.WriteLine("Checking Tasks", "Information");
                scheduler.runTasks(sleepInterval);
            }
        }

        public override bool OnStart() {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            //DiagnosticMonitor.Start("DiagnosticsConnectionString");
            Trace.WriteLine("Starting Task Scheduler", "Information");
            Logger.log("Starting Task Scheduler");

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
