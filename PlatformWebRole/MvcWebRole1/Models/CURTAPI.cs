﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;
using System.Threading.Tasks;

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

    internal static async Task<List<double>> GetYearsAsync() {
        WebClient wc = new WebClient();
        wc.Proxy = null;
        Uri targeturi = new Uri(getAPIPath() + "getyear?dataType=JSON");
        var year_json = await wc.DownloadStringTaskAsync(targeturi);
        List<double> years = JsonConvert.DeserializeObject<List<double>>(year_json);
        return years;
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

        internal static async Task<List<APIPart>> GetVehiclePartsAsync(string year, string make, string model, string style, int cust_id = 0) {
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
                Uri targeturi = new Uri(url);
                var parts_json = await wc.DownloadStringTaskAsync(targeturi);
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
                url += "GetFullParentCategories";
                url += "?dataType=JSON";

                List<APICategory> cats = new List<APICategory>();

                string cat_json = wc.DownloadString(url);
                cats = JsonConvert.DeserializeObject<List<APICategory>>(cat_json);

                return cats;
            } catch (Exception) {
                return new List<APICategory>();
            }
        }

    internal static async Task<List<APICategory>> GetParentCategoriesAsync() {
        try {
            WebClient wc = new WebClient();
            wc.Proxy = null;

            string url = getAPIPath();
            url += "GetFullParentCategories";
            url += "?dataType=JSON";

            Uri targeturi = new Uri(url);
            List<APICategory> cats = new List<APICategory>();

            var cat_json = await wc.DownloadStringTaskAsync(targeturi).ConfigureAwait(continueOnCapturedContext: false);
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

        internal static async Task<List<APICategory>> GetSubCategoriesAsync(int catID) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;
                ServicePointManager.MaxServicePoints = int.MaxValue;

                StringBuilder sb = new StringBuilder(getAPIPath());
                sb.Append("GetCategories?dataType=JSON");
                sb.Append("&parentID=" + catID);
                Uri targeturi = new Uri(sb.ToString());
                List<APICategory> cats = new List<APICategory>();

                var cat_json = await wc.DownloadStringTaskAsync(targeturi);

                return JsonConvert.DeserializeObject<List<APICategory>>(cat_json);
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
                api_cat.SubCategories = ugly_cat.sub_categories;

                return api_cat;
            } catch (Exception) {
                return new APICategory();
            }
        }

        internal static async Task<APICategory> GetCategoryByNameAsync(string cat) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetCategoryByName";
                url += "?dataType=JSON";
                url += "&catName=" + cat;
                Uri targeturi = new Uri(url);
                var cat_json = await wc.DownloadStringTaskAsync(targeturi);

                JSONAPICategory ugly_cat = JsonConvert.DeserializeObject<JSONAPICategory>(cat_json);
                APICategory api_cat = ugly_cat.parent;
                api_cat.SubCategories = ugly_cat.sub_categories;

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
                api_cat.SubCategories = ugly_cat.sub_categories;

                return api_cat;
            } catch (Exception) {
                return new APICategory();
            }
        }

        internal static async Task<APICategory> GetCategoryAsync(int id) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                StringBuilder sb = new StringBuilder();
                sb.Append(getAPIPath());
                sb.Append("GetCategory");
                sb.Append("?dataType=JSON");
                sb.Append("&catID=" + id);

                Uri targeturi = new Uri(sb.ToString());
                var cat_json = await wc.DownloadStringTaskAsync(targeturi);
                JSONAPICategory ugly_cat = JsonConvert.DeserializeObject<JSONAPICategory>(cat_json);
                APICategory api_cat = ugly_cat.parent;
                api_cat.SubCategories = ugly_cat.sub_categories;

                return api_cat;
            } catch (Exception) {
                return new APICategory();
            }
        }

        internal static List<APIPart> GetCategoryParts(int id, int page = 1, int per_page = 10) {
            try {
                Settings settings = new Settings();
                WebClient wc = new WebClient();
                wc.Proxy = null;

                StringBuilder sb = new StringBuilder(getAPIPath());
                sb.Append("GetCategoryParts");
                sb.Append("?catID=" + id);
                sb.Append("&page=" + page);
                sb.Append("&perpage=" + per_page);
                sb.Append("&cust_id=" + settings.Get("CURTAccount"));
                sb.Append("&dataType=JSON");

                string json = wc.DownloadString(sb.ToString());
                List<APIPart> parts = JsonConvert.DeserializeObject<List<APIPart>>(json);
                return parts;
            } catch (Exception) {
                return new List<APIPart>();
            }
        }

        internal static async Task<List<APIPart>> GetCategoryPartsAsync(int id, int page = 1, int per_page = 10) {
            try {
                Settings settings = new Settings();
                WebClient wc = new WebClient();
                wc.Proxy = null;

                StringBuilder sb = new StringBuilder(getAPIPath());
                sb.Append("GetCategoryParts");
                sb.Append("?catID=" + id);
                sb.Append("&page=" + page);
                sb.Append("&perpage=" + per_page);
                sb.Append("&cust_id=" + settings.Get("CURTAccount"));
                sb.Append("&dataType=JSON");

                Uri targeturi = new Uri(sb.ToString());
                var json = await wc.DownloadStringTaskAsync(targeturi);

                List<APIPart> parts = JsonConvert.DeserializeObject<List<APIPart>>(json);
                return parts;
            } catch (Exception) {
                return new List<APIPart>();
            }
        }

        public static List<APICategory> GetBreadcrumbs(int catId = 0) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "/GetCategoryBreadCrumbs";
                url += "?dataType=JSON";
                url += "&catId=" + catId;

                List<APICategory> cats = new List<APICategory>();

                string cat_json = wc.DownloadString(url);
                cats = JsonConvert.DeserializeObject<List<APICategory>>(cat_json);

                return cats;
            } catch (Exception) {
                return new List<APICategory>();
            }
        }

        public static async Task<List<APICategory>> GetBreadcrumbsAsync(int catId = 0) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "/GetCategoryBreadCrumbs";
                url += "?dataType=JSON";
                url += "&catId=" + catId;

                List<APICategory> cats = new List<APICategory>();

                Uri targeturi = new Uri(url);
                var json = await wc.DownloadStringTaskAsync(targeturi);

                cats = JsonConvert.DeserializeObject<List<APICategory>>(json);

                return cats;
            } catch (Exception) {
                return new List<APICategory>();
            }
        }

        public static List<APICategory> GetPartBreadcrumbs(int partID = 0, int catId = 0) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "/GetPartBreadCrumbs?partID=" + partID;
                url += "&catId=" + catId + "&dataType=JSON";

                List<APICategory> cats = new List<APICategory>();

                string cat_json = wc.DownloadString(url);
                cats = JsonConvert.DeserializeObject<List<APICategory>>(cat_json);

                return cats;
            } catch (Exception) {
                return new List<APICategory>();
            }
        }

        public static async Task<List<APICategory>> GetPartBreadcrumbsAsync(int partID = 0, int catId = 0) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "/GetPartBreadCrumbs?partID=" + partID;
                url += "&catId=" + catId + "&dataType=JSON";

                List<APICategory> cats = new List<APICategory>();

                Uri targeturi = new Uri(url);
                var json = await wc.DownloadStringTaskAsync(targeturi);

                cats = JsonConvert.DeserializeObject<List<APICategory>>(json);

                return cats;
            } catch (Exception) {
                return new List<APICategory>();
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

        internal static async Task<APIColorCode> GetColorCodeAsync(int p) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetColor?partID=" + p;
                Uri targeturi = new Uri(url);
                var json = await wc.DownloadStringTaskAsync(targeturi);

                APIColorCode code = JsonConvert.DeserializeObject<APIColorCode>(json);
                return code;
            } catch (Exception) {
                return new APIColorCode();
            }
        }

        internal static APIColorCode GetCategoryColorCode(int catID) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetCategoryColor?catID=" + catID;

                APIColorCode code = JsonConvert.DeserializeObject<APIColorCode>(wc.DownloadString(url));
                return code;
            } catch (Exception) {
                return new APIColorCode();
            }
        }

        internal static async Task<APIColorCode> GetCategoryColorCodeAsync(int catID) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetCategoryColor?catID=" + catID;
                Uri targeturi = new Uri(url);
                var json = await wc.DownloadStringTaskAsync(targeturi);

                APIColorCode code = JsonConvert.DeserializeObject<APIColorCode>(json);
                return code;
            } catch (Exception) {
                return new APIColorCode();
            }
        }

        internal static FullVehicle getVehicle(string year, string make, string model, string style) {
            try {
                Settings settings = new Settings();
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath() + "/GetVehicle?year=" + year;
                url += "&make=" + make + "&model=" + model + "&style=" + HttpUtility.UrlEncode(style);
                url += "&dataType=JSON";

                FullVehicle vehicle = JsonConvert.DeserializeObject<List<FullVehicle>>(wc.DownloadString(url)).FirstOrDefault<FullVehicle>();
                return vehicle;
            } catch {
                return new FullVehicle();
            };
        }

        internal static async Task<FullVehicle> getVehicleAsync(string year, string make, string model, string style) {
            try {
                Settings settings = new Settings();
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath() + "/GetVehicle?year=" + year;
                url += "&make=" + make + "&model=" + model + "&style=" + HttpUtility.UrlEncode(style);
                url += "&dataType=JSON";
                Uri targeturi = new Uri(url);
                var json = await wc.DownloadStringTaskAsync(targeturi);

                FullVehicle vehicle = JsonConvert.DeserializeObject<List<FullVehicle>>(json).FirstOrDefault<FullVehicle>();
                return vehicle;
            } catch {
                return new FullVehicle();
            }
        }

        internal static APIPart GetPart(int p, int vehicleID = 0) {
            try {
                Settings settings = new Settings();
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetPart?dataType=JSON";
                url += "&partID=" + p;
                url += "&vehicleID=" + vehicleID;
                url += "&cust_id=" + settings.Get("CURTAccount");

                return JsonConvert.DeserializeObject<APIPart>(wc.DownloadString(url));
            } catch (Exception) {
                return new APIPart();
            }
        }

        internal static async Task<APIPart> GetPartAsync(int p, int vehicleID = 0) {
            try {
                Settings settings = new Settings();
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetPart?dataType=JSON";
                url += "&partID=" + p;
                url += "&vehicleID=" + vehicleID;
                url += "&cust_id=" + settings.Get("CURTAccount");
                Uri targeturi = new Uri(url);
                var json = await wc.DownloadStringTaskAsync(targeturi);

                return JsonConvert.DeserializeObject<APIPart>(json);
            } catch {
                return new APIPart();
            }
        }

        internal static List<APIPart> GetPartsByList(string partlist = "", string year = "", string make = "", string model = "", string style = "") {
            try {
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
            } catch (Exception) {
                return new List<APIPart>();
            }
        }

        internal static async Task<List<APIPart>> GetPartsByListAsync(string partlist = "", string year = "", string make = "", string model = "", string style = "") {
            try {
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
                Uri targeturi = new Uri(url);
                var json = await wc.DownloadStringTaskAsync(targeturi);

                List<APIPart> parts = JsonConvert.DeserializeObject<List<APIPart>>(json);
                return parts;
            } catch {
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

        internal static async Task<List<APIPart>> GetRelatedPartsAsync(int p) {
            try {
                Settings settings = new Settings();
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetRelatedParts?dataType=JSON";
                url += "&partID=" + p;
                url += "&cust_id=" + settings.Get("CURTAccount");
                Uri targeturi = new Uri(url);
                var json = await wc.DownloadStringTaskAsync(targeturi);

                return JsonConvert.DeserializeObject<List<APIPart>>(json);
            } catch {
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

        internal static async Task<List<FullVehicle>> GetPartVehiclesAsync(int p) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "GetPartVehicles?dataType=JSON";
                url += "&partID=" + p;
                Uri targeturi = new Uri(url);
                var json = await wc.DownloadStringTaskAsync(targeturi);

                return JsonConvert.DeserializeObject<List<FullVehicle>>(json);
            } catch {
                return new List<FullVehicle>();
            }
       }

        internal static List<APIPart> GetConnector(int vehicleID = 0) {
            try {
                if (vehicleID > 0) {
                    WebClient wc = new WebClient();
                    wc.Proxy = null;

                    string url = getAPIPath() + "/GetConnector?vehicleID=" + vehicleID + "&dataType=JSON";
                    string part_json = wc.DownloadString(url);
                    List<APIPart> parts = JsonConvert.DeserializeObject<List<APIPart>>(part_json);
                    return parts;
                } else {
                    return new List<APIPart>();
                }
            } catch {
                return new List<APIPart>();
            };
        }

        internal static async Task<List<APIPart>> GetConnectorAsync(int vehicleID = 0) {
            try {
                if (vehicleID > 0) {
                    WebClient wc = new WebClient();
                    wc.Proxy = null;

                    string url = getAPIPath() + "/GetConnector?vehicleID=" + vehicleID + "&dataType=JSON";
                    Uri targeturi = new Uri(url);
                    var part_json = await wc.DownloadStringTaskAsync(targeturi);

                    List<APIPart> parts = JsonConvert.DeserializeObject<List<APIPart>>(part_json);
                    return parts;
                } else {
                    return new List<APIPart>();
                }
            } catch {
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

        internal static async Task<List<APIPart>> SearchAsync(string term, int page = 1, int per_page = 10) {
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
                Uri targeturi = new Uri(sb.ToString());
                var part_json = await wc.DownloadStringTaskAsync(targeturi);

                return JsonConvert.DeserializeObject<List<APIPart>>(part_json).Skip((page - 1) * per_page).Take(per_page).ToList<APIPart>();

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
        private static bool isSecure(HttpContext ctx) {
            return ctx.Request.IsSecureConnection;
        }
    }
}