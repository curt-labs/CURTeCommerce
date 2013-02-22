using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Configuration;

namespace EcommercePlatform.Models {
    public class ReCaptcha {

        public static string GenerateCaptcha(string theme = "clean") {
            string captcha = "";
            Settings settings = new Settings();
            string publickey = settings.Get("ReCaptchaPublicKey");
            captcha += "<script type=\"text/javascript\">" +
                        "var RecaptchaOptions = { theme : '" + theme + "' };</script>";
            captcha += "<script type=\"text/javascript\"" +
                        " src=\"//www.google.com/recaptcha/api/challenge?k=" + publickey + "\">" +
                        "</script>" +
                          "<noscript>" +
                             "<iframe src=\"//www.google.com/recaptcha/api/noscript?k=" + publickey + "\"" +
                                 " height=\"300\" width=\"500\" frameborder=\"0\"></iframe><br>" +
                             "<textarea name=\"recaptcha_challenge_field\" rows=\"3\" cols=\"40\">" +
                             "</textarea>" +
                             "<input type=\"hidden\" name=\"recaptcha_response_field\"" +
                                 " value=\"manual_challenge\">" +
                          "</noscript>";
            return captcha;
        }

        public static bool ValidateCaptcha(string challenge = "", string apiresponse = "") {
            bool valid = false;
            Settings settings = new Settings();
            string privatekey = settings.Get("ReCaptchaPrivateKey");

            string postdata = "privatekey=" + privatekey +
                                "&challenge=" + challenge +
                                "&response=" + apiresponse + 
                                "&remoteip=" + UDF.GetIp().ToString();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/verify");
            request.Method = "POST";
            ASCIIEncoding encoding=new ASCIIEncoding();
            byte[] postbytes = encoding.GetBytes(postdata);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postdata.Length;
            Stream postStream = request.GetRequestStream();
            postStream.Write(postbytes, 0, postbytes.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string responsestring = reader.ReadToEnd();
            postStream.Close();
            responseStream.Close();
            string[] responselines = responsestring.Split('\n');
            valid = Convert.ToBoolean(responselines[0]);

            return valid;
        }
    }
}