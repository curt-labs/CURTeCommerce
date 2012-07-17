using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace EcommercePlatform.Models {
    public class Logger {

        public string GetTempPath() {
            string path = System.Environment.GetEnvironmentVariable("TEMP");
            if (!path.EndsWith("\\")) path += "\\";
            return path;
        }

        public static void LogMessageToFile(string msg) {
            
            System.IO.StreamWriter sw = System.IO.File.AppendText(Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Content/Logs"),"log.txt"));
            try {
                string logLine = System.String.Format(
                    "{0:M/d/yyyy h:mm:ss.fff tt}: {1}.", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
            } finally {
                sw.Close();
            }
        }
    }
}