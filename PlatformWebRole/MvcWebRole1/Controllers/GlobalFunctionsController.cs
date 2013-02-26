using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace EcommercePlatform.Controllers {
    public class GlobalFunctionsController : Controller {

        public string FileExists(string path = "") {
            try {
                if (UDF.FileExists(path)) {
                    return "200";
                } else {
                    return "404";
                }
            } catch (Exception) {
                return "404";
            }
        }

        public string GetCountries() {
            try {
                List<BasicCountry> countries = Country.GetBasic();

                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                return JsonConvert.SerializeObject(countries, Formatting.None, settings);
            } catch {
                return "[]";
            }
        }

        public string GenerateCaptcha() {
            return ReCaptcha.GenerateCaptcha();
        }
    }
}