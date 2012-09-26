using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;
using System.IO;

namespace EcommercePlatform.Models {

    public class Geocoding {

        public static GeocodingResponse GetGeoLocation(string address, string city, int stateID, string zip, string countryCode = "US", string StateProvidence = "") {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                string abbr = "";
                if (stateID != 0) {
                    abbr = db.States.Where(x => x.stateID.Equals(stateID)).Select(x => x.abbr).FirstOrDefault<string>();
                } else {
                    abbr = StateProvidence;
                }

                string url = "https://maps.googleapis.com/maps/api/geocode/json?address=";
                url += HttpUtility.UrlEncode(address.Trim() + " " + city + ", " + abbr + " " + zip + " " + countryCode);
                url += "&sensor=false";

                WebClient wc = new WebClient();
                wc.Proxy = null;
                string resp = wc.DownloadString(url);
                GeocodingResponse geo = new JavaScriptSerializer().Deserialize<GeocodingResponse>(resp);

                return geo;
            } catch (Exception) {
                return new GeocodingResponse();
            }
        }

        public static dynamic AddPlace(NewPlace place) {
            try {
                string jsonPlace = new JavaScriptSerializer().Serialize(place);
                Settings settings = new Settings();
                StringBuilder sb = new StringBuilder(settings.Get("PlacesAPIDomain"));
                sb.Append("add/json?sensor=false");
                sb.Append("&key=" + settings.Get("GoogleAPIKey"));

                byte[] byteArr = Encoding.UTF8.GetBytes(jsonPlace);

                WebRequest req = WebRequest.Create(sb.ToString());
                req.Method = "POST";
                req.ContentType = "application/json";
                req.ContentLength = byteArr.Length;

                Stream dataStream = req.GetRequestStream();
                dataStream.Write(byteArr, 0, byteArr.Length);
                dataStream.Close();

                WebResponse resp = req.GetResponse();
                string resp_status = (((HttpWebResponse)resp).StatusDescription);
                dataStream = resp.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);
                string returned_resp = reader.ReadToEnd();

                AddPlaceResponse formatted_resp = new JavaScriptSerializer().Deserialize<AddPlaceResponse>(returned_resp);
                return formatted_resp;

            } catch (Exception) {
                return "[]";
            }
        }

        public static void RemovePlaceReference(int id) {

            // Instantiate our database and Location object
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Location loc = new Location();

            // Get the location from the db
            loc = db.Locations.Where(x => x.locationID.Equals(id)).FirstOrDefault<Location>();

            // Update Google Places reference
            loc.places_id = "";
            loc.places_reference = "";
            loc.places_status = "";

            // Commit to db
            db.SubmitChanges();
        }

        internal static void DeletePlaceEntry(string referenceCode = "") {
            DeletePlace place = new DeletePlace {
                reference = referenceCode
            };
            string json_reference = new JavaScriptSerializer().Serialize(place);
            Settings settings = new Settings();
            StringBuilder sb = new StringBuilder(settings.Get("PlacesAPIDomain"));
            sb.Append("delete/json?sensor=false");
            sb.Append("&key=" + settings.Get("GoogleAPIKey"));

            byte[] byteArr = Encoding.UTF8.GetBytes(json_reference);

            WebRequest req = WebRequest.Create(sb.ToString());
            req.Method = "POST";
            req.ContentType = "application/json";
            req.ContentLength = byteArr.Length;

            Stream dataStream = req.GetRequestStream();
            dataStream.Write(byteArr, 0, byteArr.Length);
            dataStream.Close();

            WebResponse resp = req.GetResponse();
            string resp_status = (((HttpWebResponse)resp).StatusDescription);
            dataStream = resp.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);
            string returned_resp = reader.ReadToEnd();

            DeletePlaceResponse response = new JavaScriptSerializer().Deserialize<DeletePlaceResponse>(returned_resp);
            if (response.status != "OK") {
                throw new Exception("Failed to delete Google Place listing with error code: " + response.status);
            }
        }

        internal static string GetGooglePlace(string reference) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;
                StringBuilder sb = new StringBuilder();
                sb.Append("https://maps.googleapis.com/maps/api/place/details/json?key=");
                sb.Append(new Settings().Get("GoogleAPIKey"));
                sb.Append("&sensor=false");
                sb.Append("&reference=" + reference);

                return wc.DownloadString(sb.ToString());
            } catch (Exception) {
                return "";
            }
        }
    }

    public class NewPlace {
        public LatitudeLongitude location { get; set; }
        public int accuracy { get; set; }
        public string name { get; set; }
        public List<string> types { get; set; }
        public string language { get; set; }
    }

    public class AddPlaceResponse {
        public string status { get; set; }
        public string reference { get; set; }
        public string id { get; set; }
    }

    public class DeletePlace {
        public string reference { get; set; }
    }

    public class DeletePlaceResponse {
        public string status { get; set; }
    }

    public class GeocodingResponse {
        public List<GeocodingResult> results { get; set; }
        public string status { get; set; }
    }

    public class GeocodingResult {
        public List<GeocodingAddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public GeocodingGeometry geometry { get; set; }
        public bool partial_match { get; set; }
        public List<string> types { get; set; }
    }

    public class GeocodingGeometry {
        public Dictionary<string, LatitudeLongitude> bounds { get; set; }
        public LatitudeLongitude location { get; set; }
        public string location_type { get; set; }
        public Dictionary<string, LatitudeLongitude> viewport { get; set; }
    }

    public class GeocodingAddressComponent {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class LatitudeLongitude {
        public decimal lat { get; set; }
        public decimal lng { get; set; }
    }
}