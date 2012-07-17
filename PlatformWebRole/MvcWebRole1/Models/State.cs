using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommercePlatform.Models;
using System.Text;
using System.Reflection;
using System.Data.Linq;

namespace EcommercePlatform {
    partial class State {

        public int getStateIDByAbbr(string abbr) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            State state = db.States.Where(x => x.abbr.ToLower().Equals(abbr.ToLower())).FirstOrDefault();
            return state.stateID;
        }



    }

}