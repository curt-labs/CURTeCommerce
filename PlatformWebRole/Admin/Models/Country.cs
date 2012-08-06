using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Admin.Models;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.Linq;

namespace Admin {
    partial class Country {

        public List<Country> GetAll() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Country> countries = db.Countries.OrderBy(x => x.name).ToList<Country>();
            return countries;
        }

        public List<State> getProvinces() {
            return this.States.OrderBy(x => x.abbr).ToList<State>();
        }

    }
}