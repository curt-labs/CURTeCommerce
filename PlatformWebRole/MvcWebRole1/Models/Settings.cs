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

        public Dictionary<string, string> values { get; set; }

        public Settings() {
            this.values = Populate();
        }

        internal Dictionary<string, string> Populate() {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            settings = db.Settings.Select(p => new { p.name, p.value }).AsEnumerable().ToDictionary(kvp => kvp.name, kvp => kvp.value);
            return settings;
        }

        public string Get(string name = "") {
            string val = "";
            try {
                if (this.values.Count == 0) {
                    this.values = Populate();
                }
                val = this.values[name].Trim();
            } catch { };
            return val;
        }

    }


}
