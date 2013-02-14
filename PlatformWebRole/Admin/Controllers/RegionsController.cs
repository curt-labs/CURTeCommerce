using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Net;
using System.IO;
using Admin.Models;

namespace Admin.Controllers {
    public class RegionsController : BaseController {

        public ActionResult Index() {
            List<Country> countries = new Country().GetAll();
            ViewBag.countries = countries;
            return View();
        }

        public string SaveRate(int stateID, decimal rate, bool hide) {
            try {
                State state = new State().Get(stateID);
                state.SaveState(rate, hide);
                return "";
            } catch {
                return "There was a problem saving the region. Try again later.";
            }
        }

    }
}
