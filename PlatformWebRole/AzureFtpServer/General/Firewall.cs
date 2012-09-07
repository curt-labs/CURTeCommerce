using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureFtpServer.General {
    class FirewallModel {
        public static bool allowConnection(string ipaddress) {
            bool hasPermission = false;
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            hasPermission = db.FTPFirewalls.Where(x => x.ipaddress.Equals(ipaddress.Trim())).Count() > 0;

            return hasPermission;
        }
    }
}
