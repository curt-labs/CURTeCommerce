using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;

namespace EcommercePlatform.Models {
    public class CURTAPI {

        internal static List<double> GetYears() {
            try {
                string year_json = "";
                WebClient wc = new WebClient();
                wc.Proxy = null;

                year_json = wc.DownloadString(getAPIPath() + "getyear?dataType=JSON");
                List<double> years = JsonConvert.DeserializeObject<List<double>>(year_json);
                return years;
            } catch (Exception) {
                return new List<double>();
            }
        }

        internal static List<APIPart> GetVehicleParts(string year, string make, string model, string style, int cust_id = 0) {
            try {
                Settings settings = new Settings();
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "getparts?dataType=JSON";
                url += "&cust_id=" + cust_id;
                url += "&year=" + year;
                url += "&make=" + make;
                url += "&model=" + model;
                url += "&style=" + style;
                url += "&cust_id=" + settings.Get("CURTAccount");

                string parts_json = wc.DownloadString(url);

                List<APIPart> parts = new List<APIPart>();
                parts = JsonConvert.DeserializeObject<List<APIPart>>(parts_json);
                return parts.OrderByDescending(x => x.pClass).ToList<APIPart>();
            } catch (Exception) {
                return new List<APIPart>();
            }
        }

        internal static List<APICategory> GetParentCategories() {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetParentCategories";
                url += "?dataType=JSON";

                List<APICategory> cats = new List<APICategory>();

                string cat_json = wc.DownloadString(url);
                cats = JsonConvert.DeserializeObject<List<APICategory>>(cat_json);

                return cats;
            } catch (Exception) {
                return new List<APICategory>();
            }
        }

        internal static List<APICategory> GetSubCategories(int catID) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;
                ServicePointManager.MaxServicePoints = int.MaxValue;

                StringBuilder sb = new StringBuilder(getAPIPath());
                sb.Append("GetCategories?dataType=JSON");
                sb.Append("&parentID=" + catID);

                return JsonConvert.DeserializeObject<List<APICategory>>(wc.DownloadString(sb.ToString()));
            } catch (Exception) {
                return new List<APICategory>();
            }
        }

        internal static APICategory GetCategoryByName(string cat) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetCategoryByName";
                url += "?dataType=JSON";
                url += "&catName="+cat;

                string cat_json = wc.DownloadString(url);
                JSONAPICategory ugly_cat = JsonConvert.DeserializeObject<JSONAPICategory>(cat_json);
                APICategory api_cat = ugly_cat.parent;
                api_cat.sub_categories = ugly_cat.sub_categories;

                return api_cat;
            } catch (Exception) {
                return new APICategory();
            }
        }

        internal static APICategory GetCategory(int id) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                StringBuilder sb = new StringBuilder();
                sb.Append(getAPIPath());
                sb.Append("GetCategory");
                sb.Append("?dataType=JSON");
                sb.Append("&catID=" + id);

                string cat_json = wc.DownloadString(sb.ToString());
                JSONAPICategory ugly_cat = JsonConvert.DeserializeObject<JSONAPICategory>(cat_json);
                APICategory api_cat = ugly_cat.parent;
                api_cat.sub_categories = ugly_cat.sub_categories;

                return api_cat;
            } catch (Exception) {
                return new APICategory();
            }
        }

        internal static List<APIPart> GetCategoryParts(int id, int page = 1, int per_page = 10) {
            try {
                Settings settings = new Settings();
                StringBuilder sb = new StringBuilder(getAPIPath());
                sb.Append("GetCategoryParts");
                sb.Append("?catID=" + id);
                sb.Append("&page=" + page);
                sb.Append("&perpage=" + per_page);
                sb.Append("&cust_id=" + settings.Get("CURTAccount"));
                sb.Append("&dataType=JSON");

                HttpWebRequest req = WebRequest.Create(sb.ToString()) as HttpWebRequest;
                req.Proxy = null;

                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                string json = new StreamReader(resp.GetResponseStream()).ReadToEnd();
                List<APIPart> parts = JsonConvert.DeserializeObject<List<APIPart>>(json);
                return parts;
            } catch (Exception) {
                return new List<APIPart>();
            }
        }

        internal static APIColorCode GetColorCode(int p) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetColor?partID=" + p;

                APIColorCode code = JsonConvert.DeserializeObject<APIColorCode>(wc.DownloadString(url));
                return code;
            } catch (Exception) {
                return new APIColorCode();
            }
        }

        internal static APIPart GetPart(int p, string year = "", string make = "", string model = "", string style = "") {
            try {
                Settings settings = new Settings();
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetPart?dataType=JSON";
                url += "&partID=" + p;
                url += "&cust_id=" + settings.Get("CURTAccount");
                if (year.Length > 0 && make.Length > 0 && model.Length > 0 && style.Length > 0) {
                    url += "&year=" + year;
                    url += "&make=" + make;
                    url += "&model=" + model;
                    url += "&style=" + style;
                }

                return JsonConvert.DeserializeObject<APIPart>(wc.DownloadString(url));
            } catch (Exception) {
                return new APIPart();
            }
        }

        internal static List<APIPart> GetPartsByList(string partlist = "", string year = "", string make = "", string model = "", string style = "") {
                Settings settings = new Settings();
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetPartsByList?dataType=JSON";
                url += "&partlist=" + partlist;
                url += "&cust_id=" + settings.Get("CURTAccount");
                if (year.Length > 0 && make.Length > 0 && model.Length > 0 && style.Length > 0) {
                    url += "&year=" + year;
                    url += "&make=" + make;
                    url += "&model=" + model;
                    url += "&style=" + style;
                }
                List<APIPart> parts = JsonConvert.DeserializeObject<List<APIPart>>(wc.DownloadString(url));
                return parts;
            try {
            } catch (Exception) {
                return new List<APIPart>();
            }
        }

        internal static List<APIPart> GetRelatedParts(int p) {
            try {
                Settings settings = new Settings();
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetRelatedParts?dataType=JSON";
                url += "&partID=" + p;
                url += "&cust_id=" + settings.Get("CURTAccount");

                return JsonConvert.DeserializeObject<List<APIPart>>(wc.DownloadString(url));
            } catch (Exception) {
                return new List<APIPart>();
            }
        }

        internal static List<FullVehicle> GetPartVehicles(int p) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetPartVehicles?dataType=JSON";
                url += "&partID=" + p;

                return JsonConvert.DeserializeObject<List<FullVehicle>>(wc.DownloadString(url));
            } catch (Exception) {
                return new List<FullVehicle>();
            }
        }

        internal static List<APIPart> GetConnector(string year, string make, string model, string style) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                Settings settings = new Settings();
                StringBuilder sb = new StringBuilder();
                sb.Append(getAPIPath());
                sb.Append("GetConnector?dataType=JSON");
                sb.Append("&year=" + year);
                sb.Append("&make=" + make);
                sb.Append("&model=" + model);
                sb.Append("&style=" + style);
                sb.Append("&cust_id=" + settings.Get("CURTAccount"));
                return JsonConvert.DeserializeObject<List<APIPart>>(wc.DownloadString(sb.ToString()));
            } catch (Exception e) {
                return new List<APIPart>();
            }
        }

        internal static void SubmitReview(int partID, int rating = 5, string subject = "", string review_text = "", string name = "", string email = "") {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                Settings settings = new Settings();
                StringBuilder sb = new StringBuilder();
                sb.Append(getAPIPath());
                sb.Append("SubmitReview");
                sb.Append("?partID=" + partID);
                sb.Append("&cust_id=" + settings.Get("CURTAccount"));
                sb.Append("&name=" + name);
                sb.Append("&email=" + email);
                sb.Append("&rating=" + rating);
                sb.Append("&subject=" + subject);
                sb.Append("&review_text=" + review_text);

                string resp = wc.DownloadString(sb.ToString());
                if (resp != "success") {
                    throw new Exception("Failed to submit review");
                }

            } catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        internal static List<APIPart> Search(string term, int page = 1, int per_page = 10) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                Settings settings = new Settings();
                StringBuilder sb = new StringBuilder();
                sb.Append(getAPIPath());
                sb.Append("PowerSearch?dataType=JSON");
                sb.Append("&search_term=" + term);
                sb.Append("&integrated=false");
                sb.Append("&customerID=" + settings.Get("CURTAccount"));

                return JsonConvert.DeserializeObject<List<APIPart>>(wc.DownloadString(sb.ToString())).Skip((page - 1) * per_page).Take(per_page).ToList<APIPart>();

            } catch (Exception) {
                return new List<APIPart>();
            }
        }

        internal static ShippingResponse GetShipping(FedExAuthentication auth, ShippingAddress origin, ShippingAddress dest, List<int> parts) {
            ShippingResponse response = new ShippingResponse();
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                Settings settings = new Settings();
                wc.Headers["Content-type"] = "application/x-www-form-urlencoded";
                string URI = settings.Get("CURTAPISHIPPINGDOMAIN") + "GetShipping";
                string parameters = "dataType=JSON";
                parameters += "&auth=" + Newtonsoft.Json.JsonConvert.SerializeObject(auth);
                parameters += "&origin=" + Newtonsoft.Json.JsonConvert.SerializeObject(origin);
                parameters += "&destination=" + Newtonsoft.Json.JsonConvert.SerializeObject(dest);
                parameters += "&parts=" + Newtonsoft.Json.JsonConvert.SerializeObject(parts);
                if (settings.Get("FedExEnvironment") == "development") {
                    parameters += "&environment=development";
                } else {
                    parameters += "&environment=production";
                }
                string APIresponse = wc.UploadString(URI, parameters);
                response = JsonConvert.DeserializeObject<ShippingResponse>(APIresponse);
                if (response.Status == "ERROR") {
                    throw new Exception("FedEx is having issues at the moment. Please try again.");
                }
            } catch (Exception) { }
            return response;
        }

        internal static List<string> GetShippingTypes() {
            try {
                Settings settings = new Settings();
                StringBuilder sb = new StringBuilder(settings.Get("CURTAPISHIPPINGDOMAIN"));
                sb.Append("GenerateJSONServiceTypes");
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string resp = wc.DownloadString(sb.ToString());

                List<string> types = JsonConvert.DeserializeObject<List<string>>(resp);
                return types;
            } catch (Exception) {
                return new List<string>();
            }
        }

        private static string getAPIPath() {
            Settings settings = new Settings();
            string API = settings.Get("CURTAPIDOMAIN");
            /*if (isSecure()) {
                return API.Replace("http:", "https:");
            }*/
            return API;
        }
        private static bool isSecure() {
            return HttpContext.Current.Request.IsSecureConnection;
        }
    }
}