using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class PartController : BaseController {
        //
        // GET: /Part/

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(int id = 0, bool ajax = false, string year = "", string make = "", string model = "", string style = "") {
            if (id <= 0) {
                Response.Redirect(Request.ServerVariables["HTTP_REFERER"]);
            }
            int vehicleID = 0;
            if (year.Length > 0 && make.Length > 0 && model.Length > 0 && style.Length > 0) {
                FullVehicle vehicle = CURTAPI.getVehicle(year, make, model, style);
                UDF.SetCookies(year, make, model, style, vehicle.vehicleID);
                ViewBag.year = year;
                ViewBag.make = make;
                ViewBag.model = model;
                ViewBag.style = style;
                ViewBag.vehicleID = vehicleID;
            } else {
                year = ViewBag.year ?? "";
                make = ViewBag.make ?? "";
                model = ViewBag.model ?? "";
                style = ViewBag.style ?? "";
                vehicleID = ViewBag.vehicleID ?? 0;
            }

            // Get the Part record
            APIPart part = CURTAPI.GetPart(id,vehicleID);

            if (part == null) { // The part page does not fit the vehicle
                // Get the part information without binding to a vehicle record
                part = CURTAPI.GetPart(id);
            }
            if (part == null) { Response.Redirect(Request.ServerVariables["HTTP_REFERER"]); }

            if (part.drilling.Length > 0) {
                part.attributes.Add(new APIAttribute { key = "Drilling", value = part.drilling });
            }
            if (part.exposed.Length > 0) {
                part.attributes.Add(new APIAttribute { key = "Exposed/Concealed", value = part.exposed });
            }
            if (part.installTime > 0) {
                part.attributes.Add(new APIAttribute { key = "Install Time", value = part.installTime.ToString() });
            }
            if (part.pClass.Length > 0) {
                part.attributes.Add(new APIAttribute { key = "Class", value = part.pClass });
            }

            // We need to push this partID into the recent parts Session object
            SessionWorker.AddRecentPart(part.partID);

            ViewBag.part = part;

            // Validate the vehicle info
            List<APIPart> connectors = new List<APIPart>();
            if (vehicleID > 0 && part.pClass.Length > 0) {
                connectors = CURTAPI.GetConnector(vehicleID);
            }
            ViewBag.connectors = connectors;

            // Get the Related Parts
            List<APIPart> related_parts = CURTAPI.GetRelatedParts(id);
            ViewBag.related_parts = related_parts;

            // Get the vehicles that match this part
            List<FullVehicle> vehicles = CURTAPI.GetPartVehicles(id);
            ViewBag.vehicles = vehicles;

            ViewBag.ajax = ajax;

            return View();
        }

    }
}
