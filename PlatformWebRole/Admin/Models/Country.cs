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

        public List<State> getProvinces() {
            return this.States.OrderBy(x => x.abbr).ToList<State>();
        }

    }
}