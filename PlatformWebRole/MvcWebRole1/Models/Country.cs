using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommercePlatform.Models;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.Linq;

namespace EcommercePlatform {
    partial class Country {

        public List<State> getProvinces() {
            return this.States.OrderBy(x => x.abbr).ToList<State>();
        }

        public static List<BasicCountry> GetBasic() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<BasicCountry> countries = (from c in db.Countries
                                            select new BasicCountry {
                                                ID = c.ID,
                                                name = c.name,
                                                abbr = c.abbr,
                                                states = (from s in db.States
                                                            where s.countryID.Equals(c.ID)
                                                            select new BasicState {
                                                                stateID = s.stateID,
                                                                state = s.state1,
                                                                abbr = s.abbr
                                                            }).ToList<BasicState>()
                                            }).ToList<BasicCountry>();
            return countries;
        }

    }

    public class BasicCountry {
        public int ID { get; set; }
        public string name { get; set; }
        public string abbr { get; set; }
        public List<BasicState> states { get; set; }
    }

    public class BasicState {
        public int stateID { get; set; }
        public string state { get; set; }
        public string abbr { get; set; }
    }
}