using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers {
    public class ServicesController : BaseController {

        public ActionResult Index() {

            // Get all the services
            List<Service> services = ServicesModel.GetAll();
            ViewBag.services = services;

            return View();
        }

        public ActionResult Edit(int id = 0, bool failed = false) {
            ViewBag.errors = new List<string>();
            if (TempData["errors"] != null) {
                ViewBag.errors = TempData["errors"];
            }

            ViewBag.service = new Service();
            ViewBag.locations = new List<Location>();
            if (id > 0) {
                ViewBag.service = ServicesModel.Get(id);
                List<Location> locations = ServicesModel.GetLocations(id);
                List<Location> all_locations = Locations.GetAll();
                all_locations = (from al in all_locations
                                 where !(from l in locations
                                         select l.locationID).Contains(al.locationID)
                                 select al).ToList<Location>();
                ViewBag.locations = locations;
                ViewBag.all_locations = all_locations;
            }

            if (TempData["service"] != null) {
                ViewBag.service = TempData["service"];
            }
            ViewBag.failed = failed;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public dynamic Save(int id = 0, string title = "", string description = "", decimal price = 0, string hourly = "0") {
            Service service = new Service();
            List<string> errors = new List<string>();
            try {
                ServicesModel.Save(id, title, description, price, hourly, out service);

                return RedirectToAction("Index", "Services");
            } catch (Exception e) {
                errors.Add(e.Message);
                TempData["errors"] = errors;
                TempData["service"] = service;
                return RedirectToAction("Edit", "Services", new { id = id, failed = true });
            }
        }

        public ActionResult Delete(int id = 0) {
            try {
                if (id == 0) { throw new Exception("Invalid reference."); }
                ServicesModel.Delete(id);

                return RedirectToAction("Index", "Services");
            } catch (Exception e) {
                List<string> errors = new List<string>();
                errors.Add("Failed to remove service: " + e.Message);
                TempData["errors"] = errors;
                return RedirectToAction("Edit", "Services", new { id = id, failed = true });
            }
        }

        public string AddLocation(int serviceID = 0, int locationID = 0) {
            try {
                if (serviceID == 0 || locationID == 0) { throw new Exception("Invalid reference."); }
                // Tie a Service to a Location
                ServicesModel.AddLocation(serviceID, locationID);

                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }

        public string RemoveLocation(int serviceID = 0, int locationID = 0) {
            try {
                if (serviceID == 0 || locationID == 0) { throw new Exception("Invalid reference."); }
                ServicesModel.RemoveLocation(serviceID, locationID);

                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }

    }
}
