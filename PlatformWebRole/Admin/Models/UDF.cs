using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.AccessControl;
using System.Management;
using System.Management.Instrumentation;
using Admin.Models;
using System.Net.Mail;
using System.Text;
using System.Net.Configuration;
using System.Net;
using System.Collections.Specialized;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Admin.Models {
    public class UDF {

        public static void OpenPermissions(string folder = "") {
            try {
                bool isModified = false;
                DirectoryInfo dirInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath(folder));
                DirectorySecurity dirSec = dirInfo.GetAccessControl();

                AccessRule rule = new FileSystemAccessRule("Users", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow);
                dirSec.ModifyAccessRule(AccessControlModification.Add, rule, out isModified);
                dirInfo.SetAccessControl(dirSec);

                rule = new FileSystemAccessRule("IIS_IUSRS", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow);
                dirSec.ModifyAccessRule(AccessControlModification.Add, rule, out isModified);
                dirInfo.SetAccessControl(dirSec);
            } catch (Exception e) {
                string err = e.Message;
            }
        }

        public static void SubmitBug(string name, string email, string desc, string url = "", string subject = "") {

            // Validate the required fields
            if (name == null || name.Length == 0 || email == null || email.Length == 0 || desc == null || desc.Length == 0)
                throw new Exception();
            Settings settings = new Settings();
            StringBuilder sb = new StringBuilder();

            sb.Append("<p>A new bug has been submitted for the above listed domain.</p>");
            sb.Append("<hr />");
            sb.Append("<p>Submitter: " + name + " <" + email + "></p>");
            if (subject.Length > 0) {
                sb.Append("<span><strong>Subject: </strong>" + subject + "</span><br />");
            }
            if (url.Length > 0) {
                sb.Append("<span><strong>URI: </strong>" + url + "</span><br />");
            }
            sb.Append("<br /><span><strong>Bug</strong></span><br />");
            sb.Append("<p>" + desc + "</p>");
            sb.Append("<hr /><br />");
            sb.Append("<p style='font-size:11px'>If you feel this was a mistake please disregard this e-mail.</p>");

            string[] tos = { settings.Get("SupportEmail") };
            SendEmail(tos, "Bug submitted from " + HttpContext.Current.Request.Url.Host, true, sb.ToString());
        }

        public static List<Country> GetCountries() {
            try {
                return new EcommercePlatformDataContext().Countries.ToList<Country>();
            } catch (Exception) {
                return new List<Country>();
            }
        }

        public static string GenerateSlug(string phrase = "") {
            string str = RemoveAccent(phrase).ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars           
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            str = str.Trim(); // cut and trim it   
            str = Regex.Replace(str, @"\s", "_"); // underscores

            return str;
        }

        public static string getRemoteIP() {
            string ipaddress = "";
            ipaddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (String.IsNullOrEmpty(ipaddress)) {
                ipaddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ipaddress;
        }

        public static string RemoveAccent(string txt = "") {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        internal static void SendEmail(string[] to, string subject = "", bool isHTML = true, string message = "", bool suppressErrors = false) {
            Settings settings = new Settings();
            SmtpClient client = null;
            try {
                MailMessage mail = new MailMessage();
                //SmtpSection smtp = null;
                client = new SmtpClient(settings.Get("SMTPServer"), Convert.ToInt32(settings.Get("SMTPPort")));
                client.Credentials = new NetworkCredential(settings.Get("SMTPUserName"), settings.Get("SMTPPassword"));
                bool enableSSL = (settings.Get("SMTPSSL") == "true") ? true : false;
                client.EnableSsl = enableSSL;

                mail.From = new MailAddress(settings.Get("NoReplyEmailAddress"));
                foreach (string recip in to) {
                    mail.To.Add(recip);
                }
                mail.Subject = subject;
                mail.IsBodyHtml = isHTML;
                mail.Body = message;

                client.Send(mail);
            } catch (Exception e) {
                if (!suppressErrors) {
                    throw new Exception(client.Host + ":" + client.Port + " " + e.Message);
                }
            }
        }

        internal static string POSTRequest(string url, Dictionary<string, string> data) {
            string post_data = "";
            foreach (KeyValuePair<string, string> param in data) {
                post_data += param.Key + "=" + param.Value;
                post_data += "&";
            }

            byte[] byteArr = Encoding.UTF8.GetBytes(post_data);
            WebRequest req = WebRequest.Create(url);
            req.Method = "POST";
            req.ContentLength = byteArr.Length;
            req.ContentType = "application/x-www-form-urlencoded";

            Stream dataStream = req.GetRequestStream();
            dataStream.Write(byteArr, 0, byteArr.Length);
            dataStream.Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            if ((int)resp.StatusCode != 200) {
                throw new Exception(resp.StatusDescription);
            }

            Stream respStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(respStream);

            string json = reader.ReadToEnd();
            resp.Close();
            respStream.Dispose();
            reader.Dispose();

            return json;
        }

        public static string EncryptString(string input) {
            try {
                System.Security.Cryptography.MD5CryptoServiceProvider crypto = new System.Security.Cryptography.MD5CryptoServiceProvider();

                byte[] data = System.Text.Encoding.ASCII.GetBytes(input);
                data = crypto.ComputeHash(data);

                return System.Text.Encoding.ASCII.GetString(data);
            } catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Check all the fields in the ContactInquiry object that are required
        /// </summary>
        internal static void Sanitize(Object obj, string[] nullable_columns) {
            // Get the properties of ContactInquiry
            PropertyInfo[] props = obj.GetType().GetProperties();

            // Loop through the ContactInquire properties
            foreach (PropertyInfo field in props) {
                if (!nullable_columns.Contains(field.Name.ToLower())) { // Check to make sure this column isn't a nullable type

                    if (field.GetValue(obj, null) == null || field.GetValue(obj, null).ToString().Length == 0) { // Check if the field is null or empty
                        // throw a message letting the user know that they are fucking stupid!
                        throw new Exception(field.Name.Substring(0, 1).ToUpper() + field.Name.Substring(1) + " is required.");
                    }
                }
            }
        }

        Object CreateObjectByType(Type t) {
            Object obj = Activator.CreateInstance(t);
            return obj;
        }

        public static string getBlobFileName(Uri uri) {
            string filename = "";
            filename = uri.Segments[(uri.Segments.Count() - 1)].Replace("/","");
            return filename;
        }

        public static string getFolderPath(Uri uri) {
            string folderpath = "";
            for (var i = 1; i < uri.Segments.Count(); i++) {
                folderpath += uri.Segments[i].Replace("/", "");
                if(i != uri.Segments.Count() - 1) {
                    folderpath += "/";
                }
            }
            return folderpath;
        }

        public static string getURLWithoutQueryString() {
            string url = "";
            url = HttpContext.Current.Request.Url.AbsolutePath;
            return url;
        }
    }

    public static class UDE {
        public static IDictionary<string, string> ToFormDictionary(this NameValueCollection col) {
            var dict = new Dictionary<string, string>();
            foreach (string key in col.Keys) {
                dict.Add(key, col[key]);
            }
            return dict;
        }
    }
    public class DCList {
        public int ID { get; set; }
        public double distance { get; set; }
    }
}
