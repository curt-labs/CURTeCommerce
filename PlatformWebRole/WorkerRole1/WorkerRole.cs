using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using AzureFtpServer.Azure;
using AzureFtpServer.Ftp;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace FTPServerRole {
    public class WorkerRole : RoleEntryPoint {
        private FtpServer _server;

        public override void Run() {

            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("FTPRole entry point called", "Information");

            while (true) {
                if (_server.Started) {
                    Thread.Sleep(10000);
                    Trace.WriteLine("Server is alive.", "Information");
                }
                else {
                    _server.Start();
                    Trace.WriteLine("Server starting.", "Control");
                }

            }
        }

        public override bool OnStart() {

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            //DiagnosticMonitor.Start("DiagnosticsConnectionString");

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging;

            if (_server == null)
                _server = _server = new FtpServer(new AzureFileSystemFactory());

            _server.NewConnection += ServerNewConnection;

            return base.OnStart();
        }

        static void ServerNewConnection(int nId) {
            Trace.WriteLine(String.Format("Connection {0} accepted", nId), "Connection");
        }

        private static void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e) {
            // If a configuration setting is changing
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange)) {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }
    }
}
