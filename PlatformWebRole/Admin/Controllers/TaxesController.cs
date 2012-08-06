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
    public class TaxesController : BaseController {

        public ActionResult Index() {
            List<Country> countries = new Country().GetAll();
            ViewBag.countries = countries;
            return View();
        }

        public string SaveRate(int stateID, decimal rate) {
            try {
                State state = new State().Get(stateID);
                state.SaveRate(rate);
                return "";
            } catch {
                return "There was a problem saving your rate. Try again later.";
            }
        }

    }
}
