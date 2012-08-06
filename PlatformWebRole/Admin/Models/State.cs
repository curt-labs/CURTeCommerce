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
    partial class State {

        public State Get(int id) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            State state = db.States.Where(x => x.stateID.Equals(id)).First<State>();
            return state;
        }

        public void SaveRate(decimal taxRate) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            State state = db.States.Where(x => x.stateID.Equals(this.stateID)).First<State>();
            state.taxRate = taxRate;
            db.SubmitChanges();
        }

    }
}