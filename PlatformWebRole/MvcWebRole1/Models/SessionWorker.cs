using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace EcommercePlatform.Models {
    public class SessionWorker {

        /// <summary>
        /// Add a PartID to our Recently Viewed Parts Session object
        /// </summary>
        /// <param name="id">ID of the part to be added.</param>
        internal static void AddRecentPart(int id) {
            try {
                HttpCookie recent_cookie = HttpContext.Current.Request.Cookies.Get("recent_parts");
                List<int> recent_parts = new List<int>();
                if (recent_cookie == null || recent_cookie.Value == null) {
                    recent_cookie = new HttpCookie("recent_parts");
                    recent_cookie.Expires = DateTime.Now.AddDays(30);

                    recent_parts.Add(id);

                    recent_cookie.Value = Newtonsoft.Json.JsonConvert.SerializeObject(recent_parts);
                    HttpContext.Current.Response.Cookies.Add(recent_cookie);
                } else {
                    List<int> unique_ids = new List<int>();
                    List<int> recent_partIds = new List<int>();

                    recent_partIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(recent_cookie.Value);
                    recent_cookie.Expires = DateTime.Now.AddDays(-1);

                    HttpCookie new_cookie = new HttpCookie("recent_parts");
                    new_cookie.Expires = DateTime.Now.AddDays(30);

                    if (!recent_partIds.Contains(id)) {
                        recent_partIds.Add(id);
                    }
                    recent_partIds = recent_partIds.Take(5).ToList<int>();
                    new_cookie.Value = Newtonsoft.Json.JsonConvert.SerializeObject(recent_partIds);
                    HttpContext.Current.Response.Cookies.Add(new_cookie);
                }
            } catch (Exception) { }
        }



        internal static List<APIPart> GetRecentParts() {
            try {
                List<int> recent = new List<int>();
                List<APIPart> parts = new List<APIPart>();
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("recent_parts");

                if (cookie != null && cookie.Value != null) {
                    recent = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(cookie.Value);

                    // Loop through the IDs and get the part record from the API
                    string part_list = "";
                    for (int i = 0; i < recent.Count; i++) {
                        if (i != 0) { part_list += ","; }
                        part_list += recent[i].ToString();
                    }
                    if (part_list.Length > 0) {
                        parts = CURTAPI.GetPartsByList(part_list);
                    }
                }
                return parts;
            } catch (Exception) {
                return new List<APIPart>();
            }
        }
    }
}