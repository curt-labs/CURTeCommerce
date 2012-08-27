using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.Linq;

namespace TaskScheduler {
    public class Settings {

        internal Dictionary<string,string> GetAll() {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            settings = db.Settings.Select(p => new {p.name,p.value}).AsEnumerable().ToDictionary(kvp => kvp.name, kvp => kvp.value);
            return settings;
        }

        public string Get(string name = "") {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            string val = "";
            try {
                val = db.Settings.Where(x => x.name.ToLower().Trim().Equals(name.ToLower().Trim())).Select(x => x.value).First().Trim();
            } catch { };
            return val;
        }

    }


}
