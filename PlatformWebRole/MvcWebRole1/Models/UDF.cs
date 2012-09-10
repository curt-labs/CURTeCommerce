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
            } catch (Exception e) {
                return false;
            }
        }

        public static Cart ExpireCart(int cust_id) {
            Cart new_cart = new Cart().Save();
            new_cart.UpdateCart(cust_id);
            DateTime cookexp = HttpContext.Current.Request.Cookies["hdcart"].Expires;
            HttpCookie cook = new HttpCookie("hdcart", new_cart.ID.ToString());
            cook.Expires = cookexp;
            HttpContext.Current.Response.Cookies.Add(cook);
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

                mail.From = new MailAddress(settings.Get("SMTPUserName"));
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

        public static List<Banner> GetRandomBanners(int count = 5) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Banner> banners = new List<Banner>();
            banners = db.Banners.Where(x => x.isVisible.Equals(1)).ToList<Banner>();
            banners = Shuffle<Banner>(banners);
            banners = banners.Take(count).ToList<Banner>();
            return banners;
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
    }
    public class DCList {
        public int ID { get; set; }
        public double distance { get; set; }
    }
}