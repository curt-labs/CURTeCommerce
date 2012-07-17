using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.IO;

namespace Admin.Models {
    public class SquareAuth {

        public enum Method { GET, POST };
        public const string AUTHORIZE = "https://foursquare.com/oauth2/authenticate";
        public const string ACCESS_TOKEN = "https://foursquare.com/oauth2/access_token";
        public const string CALLBACK_URL = "http://localhost:59295/Admin/_4box/oauth";
        public const string API_DOMAIN = "https://api.foursquare.com/v2/";

        private string _clientID = "";
        private string _clientSecret = "";
        private string _token = "";

        public string ClientID {
            get {
                if (_clientID.Length == 0) {
                    _clientID = "RPUMVMSOUVKWYMUZMMM0I3K1AV54VKXDSXOZ4Q3SIJ1UBWE5";
                }
                return _clientID;
            }
            set {
                _clientID = value;
            }
        }

        public string ClientSecret {
            get {
                if (_clientSecret.Length == 0) {
                    _clientSecret = "0TE2YM01YL1XIGKGNHNPVQ2RV1BDIHXDM011WF5AEF5ZQ2VL";
                }
                return _clientSecret;
            }
            set {
                _clientSecret = value;
            }
        }

        public string Token {
            get {
                if (_token.Length == 0) {
                    HttpContext.Current.Session["oauth_requested_uri"] = HttpContext.Current.Request.Url.AbsoluteUri;
                    StringBuilder sb = new StringBuilder(AUTHORIZE);
                    sb.Append("?client_id=" + ClientID);
                    sb.Append("&response_type=code");
                    sb.Append("&redirect_uri="+CALLBACK_URL);
                    HttpContext.Current.Response.Redirect(sb.ToString());
                }
                return _token;
            }
            set {
                _token = value;
            }
        }

        internal string GetCategories() {
            try {
                StringBuilder sb = new StringBuilder(API_DOMAIN);
                sb.Append("venues/categories");
                sb.Append("?oauth_token=" + Token);
                WebClient wc = new WebClient();

                return wc.DownloadString(sb.ToString());
            } catch (Exception) {
                return "[]";
            }
        }
    }
}