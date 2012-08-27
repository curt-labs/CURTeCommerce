using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers {
    public class LocationsController : BaseController {

        public ActionResult Index() {

            // Get all the locations
            List<Location> locs = Locations.GetAll();
            ViewBag.locations = locs;

            return View();
        }

        public ActionResult Edit(int id = 0,bool failed = false) {
            ViewBag.errors = new List<string>();
            if (TempData["errors"] != null) {
                ViewBag.errors = TempData["errors"];
            }

            ViewBag.location = new Location();
            if (id > 0) {
                ViewBag.location = Locations.Get(id);
            }

            if (TempData["location"] != null) {
                ViewBag.location = TempData["location"];
            }
            ViewBag.settings = ViewBag.settings;
            ViewBag.countries = UDF.GetCountries();
            ViewBag.failed = failed;
            return View();
        }

        public dynamic Delete(int id = 0) {
            try {
                Locations.Delete(id);

                return RedirectToAction("Index", "Locations");
            } catch (Exception e) {
                List<string> errors = new List<string>();
                errors.Add(e.Message);
                TempData["errors"] = errors;

                return RedirectToAction(Request.ServerVariables["HTTP_REFERER"]);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(int id = 0, string name = "", string phone = "", string fax = "", string email = "", string address = "", string city = "", int stateID = 0, int zip = 0, int isPrimary = 0, int google_places = 0, int foursquare = 0) {
            Location loc = new Location();
            List<string> errors = new List<string>();
            try {
                Locations.Save(id, name, phone, fax, email, address, city, stateID, zip, isPrimary, google_places, foursquare, out loc, out errors);

                if (errors.Count > 0) {
                    throw new Exception();
                }
                return RedirectToAction("Index", "Locations");
            } catch (Exception) {
                TempData["errors"] = errors;
                TempData["location"] = loc;
                return RedirectToAction("Edit", "Locations", new { id = id, failed = true });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string ViewGoogPlace(string reference = "") {
            return Geocoding.GetGooglePlace(reference);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string DeletePlaceReference(int locationID = 0) {
            try {
                Geocoding.RemovePlaceReference(locationID);
                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }
    }
}
