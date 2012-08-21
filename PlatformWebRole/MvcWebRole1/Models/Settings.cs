using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommercePlatform.Models;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.Linq;

namespace EcommercePlatform.Models {
    public class Settings {

        internal Dictionary<string,string> GetAll() {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            try {
                var sessionSettings = HttpContext.Current.Session["settings"];
                if (sessionSettings != null && sessionSettings.GetType() == typeof(Dictionary<string, string>) && sessionSettings.ToString() != null) {
                    settings = (Dictionary<string, string>)sessionSettings;
                } else {
                    throw new Exception();
                }
            } catch {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                settings = db.Settings.Select(p => new {p.name,p.value}).AsEnumerable().ToDictionary(kvp => kvp.name, kvp => kvp.value);
                HttpContext.Current.Session.Add("settings", settings);
            }
            return settings;
        }

        public string Get(string name = "") {
            Dictionary<string, string> settings = GetAll();
            string val = "";
            try {
                val = settings[name].Trim();
            } catch { };
            return val;
        }

    }


}
