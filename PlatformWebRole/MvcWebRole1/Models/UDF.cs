using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Net.Configuration;
using System.Reflection;
using System.Security;
using System.Data.Linq.Mapping;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Web.Routing;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace EcommercePlatform.Models {
    public class UDF {

        public static Boolean FileExists(string path = "") {
            if (path == null || path.Trim().Length == 0) {
                return false;
            }
            WebRequest req = WebRequest.Create(path);
            req.Timeout = 2;
            req.Method = "HEAD";

            try {
                WebResponse resp = req.GetResponse();
                return true;
            } catch {
                return false;
            }
        }

        public static string GetControllerName(string cname) {
            string controllername = cname.Split('.').ToList().Last().Replace("Controller", "");
            return controllername;
        }

        public static string GenerateSlug(string phrase = "") {
            string str = RemoveAccent(phrase).ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars           
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            str = str.Trim(); // cut and trim it   
            str = Regex.Replace(str, @"\s", "_"); // underscores

            return str;
        }

        public static string RemoveAccent(string txt = "") {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static void SetCookies(HttpContext ctx, string year, string make, string model, string style, int vehicleID = 0) {
            // Store the vehicle fields in cookies
            HttpCookie year_cookie = new HttpCookie("vehicle_year");
            year_cookie.Value = year;
            year_cookie.Expires = DateTime.Now.AddDays(14);
            ctx.Response.Cookies.Add(year_cookie);

            HttpCookie make_cookie = new HttpCookie("vehicle_make");
            make_cookie.Value = make;
            make_cookie.Expires = DateTime.Now.AddDays(14);
            ctx.Response.Cookies.Add(make_cookie);

            HttpCookie model_cookie = new HttpCookie("vehicle_model");
            model_cookie.Value = model;
            model_cookie.Expires = DateTime.Now.AddDays(14);
            ctx.Response.Cookies.Add(model_cookie);

            HttpCookie style_cookie = new HttpCookie("vehicle_style");
            style_cookie.Value = style;
            style_cookie.Expires = DateTime.Now.AddDays(14);
            ctx.Response.Cookies.Add(style_cookie);

            HttpCookie vehicle_cookie = new HttpCookie("vehicle_id");
            vehicle_cookie.Value = vehicleID.ToString();
            vehicle_cookie.Expires = DateTime.Now.AddDays(14);
            ctx.Response.Cookies.Add(vehicle_cookie);
        }

        public static void SetCategoryCookie(HttpContext ctx, int catID) {
            HttpCookie category_cookie = new HttpCookie("last_category");
            category_cookie.Value = catID.ToString();
            category_cookie.Expires = DateTime.Now.AddDays(14);
            ctx.Response.Cookies.Add(category_cookie);
        }

        public static string GetYearCookie(HttpContext ctx) {
            HttpCookie vehicleYear = ctx.Request.Cookies.Get("vehicle_year");
            return (vehicleYear != null && vehicleYear.Value != null) ? vehicleYear.Value.ToString() : "";
        }

        public static string GetMakeCookie(HttpContext ctx) {
            HttpCookie vehicleMake = ctx.Request.Cookies.Get("vehicle_make");
            return (vehicleMake != null && vehicleMake.Value != null) ? vehicleMake.Value.ToString() : "";
        }

        public static string GetModelCookie(HttpContext ctx) {
            HttpCookie vehicleModel = ctx.Request.Cookies.Get("vehicle_model");
            return (vehicleModel != null && vehicleModel.Value != null) ? vehicleModel.Value.ToString() : "";

        }

        public static string GetStyleCookie(HttpContext ctx) {
            HttpCookie vehicleStyle = ctx.Request.Cookies.Get("vehicle_style");
            return (vehicleStyle != null && vehicleStyle.Value != null) ? vehicleStyle.Value.ToString() : "";
        }

        public static int GetVehicleCookie(HttpContext ctx) {
            HttpCookie vehicleID = ctx.Request.Cookies.Get("vehicle_id");
            return (vehicleID != null && vehicleID.Value != null) ? Convert.ToInt32(vehicleID.Value.ToString()) : 0;
        }

        public static int GetCategoryCookie(HttpContext ctx) {
            HttpCookie catID = ctx.Request.Cookies.Get("last_category");
            return (catID != null && catID.Value != null) ? Convert.ToInt32(catID.Value.ToString()) : 0;
        }

        public static Cart ExpireCart(HttpContext ctx, int cust_id) {
            Cart new_cart = new Cart().Save();
            new_cart.UpdateCart(ctx, cust_id);
            DateTime cookexp = ctx.Request.Cookies["hdcart"].Expires;
            HttpCookie cook = new HttpCookie("hdcart", new_cart.ID.ToString());
            cook.Expires = cookexp;
            ctx.Response.Cookies.Add(cook);
            return new_cart;
        }

        public static List<Country> GetCountries() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.Countries.ToList<Country>();
            } catch (Exception) {
                return new List<Country>();
            }
        }

        /// <summary>
        /// Check all the fields in the ContactInquiry object that are required
        /// </summary>
        internal static void Sanitize(Object obj, string[] nullable_columns) {
            // Get the properties of the object
            PropertyInfo[] props = obj.GetType().GetProperties();

            List<string> nullable_list = new List<string>();
            foreach (string col in nullable_columns) {
                nullable_list.Add(col.ToLower().Trim());
            }
            nullable_list.Add(GetPrimaryKey(obj));

            // Loop through the object properties
            foreach (PropertyInfo field in props.Where(x => !nullable_list.Contains(x.Name.ToLower())).ToList<PropertyInfo>()) {
                //if (!nullable_list.Contains(field.Name.Trim().ToLower())) { // Check to make sure this column isn't a nullable type
                    if (field.PropertyType == typeof(int)) {
                        if (field.GetValue(obj, null) == null || (int)field.GetValue(obj, null) == 0) { // Check if the field is null or empty
                            // throw a message letting the user know that they are fucking stupid!
                            throw new Exception(field.Name.Substring(0, 1).ToUpper() + field.Name.Substring(1) + " is required.");
                        }
                    } else {
                        if (field.GetValue(obj, null) == null || field.GetValue(obj, null).ToString().Length == 0) { // Check if the field is null or empty
                            // throw a message letting the user know that they are fucking stupid!
                            throw new Exception(field.Name.Substring(0, 1).ToUpper() + field.Name.Substring(1) + " is required.");
                        }
                    }
                //}
            }
        }

        private static string GetPrimaryKey(Object obj) {
            PropertyInfo[] props = obj.GetType().GetProperties();
            PropertyInfo pk = null;
            foreach (PropertyInfo prop in props) {
                var column = prop.GetCustomAttributes(false).Where(x => x.GetType() == typeof(ColumnAttribute)).FirstOrDefault(x => ((ColumnAttribute)x).IsPrimaryKey && ((ColumnAttribute)x).DbType.Contains("NOT NULL"));
                if (column != null) {
                    pk = prop;
                    break;
                }
                if (pk == null) {
                    return "";
                }
            }
            return pk.Name.ToLower();
        }

        Object CreateObjectByType(Type t) {
            Object obj = Activator.CreateInstance(t);
            return obj;
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

        internal static SecureString SecureToken(string token) {
            SecureString sec = new SecureString();
            foreach (char c in token) {
                sec.AppendChar(c);
            }
            sec.MakeReadOnly();
            return sec;
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

        public static List<T> Shuffle<T>(IList<T> list) {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1) {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list.ToList();
        }

        public static IPAddress GetIp(HttpContext ctx) {
            string ipString;
            if (string.IsNullOrEmpty(ctx.Request.ServerVariables["HTTP_X_FORWARDED_FOR"])) {
                ipString = ctx.Request.ServerVariables["REMOTE_ADDR"];
            } else {
                ipString = ctx.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]
                   .Split(",".ToCharArray(),
                   StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }

            IPAddress result;
            if (!IPAddress.TryParse(ipString, out result)) {
                result = IPAddress.None;
            }

            return result;
        }

        public static TimeZoneInfo GetTimeZone(HttpContext ctx) {
            string timezone = "UTC";
            HttpCookie zone_cookie = null;
            zone_cookie = ctx.Request.Cookies.Get("tzinfo");
            if (zone_cookie != null && zone_cookie.Value != null && zone_cookie.Value.Length > 0) {
                // zone cookie exists
                timezone = zone_cookie.Value;
            } else {
                TimeZone.GetTimeZone(ctx);
            }
            TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            return info;
        }

        public static string ShortTZ(TimeZoneInfo tz, DateTime time) {
            if (tz.Id.IndexOf(" ") > 0) {
                string toabbr = "";
                string abbr = "";
                if (tz.SupportsDaylightSavingTime) {
                    if (tz.IsDaylightSavingTime(time)) {
                        toabbr = tz.DaylightName;
                    } else {
                        toabbr = tz.StandardName;
                    }
                } else {
                    toabbr = tz.Id;
                }
                string[] words = toabbr.Split(' ');
                foreach (var word in words) {
                    abbr += word[0];
                }
                return abbr;
            }
            return tz.Id;
        }
    }
    public class DCList {
        public int ID { get; set; }
        public double distance { get; set; }
    }
}