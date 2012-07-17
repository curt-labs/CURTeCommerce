using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommercePlatform.Models {
    public class BrowserConsole {
        public static void log(string message = "", string dataType = "string", bool includeTimestamp = true, bool includeScript = true) {
            string log = "";
            string timestamp = String.Format("{0:h:mm:ss.fff tt}", DateTime.Now) + " | ";
            if (dataType == "json") {
                log = "$.parseJSON(\"" + message + "\")";
                if (includeTimestamp) {
                    log = timestamp + "," + log;
                }
                log = "console.log(\"" + log + "\");";
            } else {
                // string
                if (includeTimestamp) {
                    log = "console.log(\"" + timestamp + message + "\");";
                }
            }
            if (includeScript) {
                log = "<script>" + log + "</script>";
            }
            HttpContext.Current.Response.Write(log);
        }
    }
}