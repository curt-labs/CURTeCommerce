using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using EcommercePlatform.Models;
using System.Web.Script.Serialization;

namespace EcommercePlatform.Controllers {

    public class LookupController : BaseController {

        public ActionResult Index(string year = "", string make = "", string model = "", string style = "") {
            Settings settings = ViewBag.settings;
            style = style.Replace('!', '/');

            Session["year"] = year.Trim();
            Session["make"] = make.Trim();
            Session["model"] = model.Trim();
            Session["style"] = style.Trim();
            Session.Timeout = 30;

            int cust_id = 0;
            try {
                cust_id = Convert.ToInt32(settings.Get("CURTAccount"));
            } catch (Exception) { }

            // Get all the parts that match our search criteria
            List<APIPart> parts = CURTAPI.GetVehicleParts(year.Trim(), make.Trim(), model.Trim(), style.Trim(), cust_id);

            // Get the unique classes of the returned parts
            IEnumerable<IGrouping<string,APIPart>> distinct = parts.GroupBy(x => x.pClass.Trim());

            // Loop through the unique groups and get the color codes for each pClass
            foreach (IGrouping<string,APIPart> d in distinct) {
                APIPart p = d.FirstOrDefault<APIPart>();

                // Get the color code from the API
                APIColorCode color_code = CURTAPI.GetColorCode(p.partID);

                // Set up default code if something went wrong
                if (color_code == null || color_code.code == null) {
                    color_code = new APIColorCode();
                    color_code.code = "ffffff";
                }

                // Get the parts that match this pClass and bind our colorCode to them
                List<APIPart> codedParts = parts.Where(x => x.pClass.Equals(p.pClass)).ToList<APIPart>();
                foreach (APIPart codedPart in codedParts) {
                    codedPart.colorCode = color_code.code;
                }
            }


            int part_count = parts.Count;
            Dictionary<string, List<APIPart>> ordered_parts = new Dictionary<string, List<APIPart>>();
            foreach (APIPart part in parts) {
                if (part.pClass.Length > 0) {
                    if (ordered_parts.Keys.Contains(part.pClass)) { // Already added to dictionary
                        List<APIPart> existing_parts = ordered_parts.Where(x => x.Key == part.pClass).Select(x => x.Value).FirstOrDefault<List<APIPart>>();
                        existing_parts.Add(part);
                        ordered_parts[part.pClass] = existing_parts;
                    } else { // New Color Code
                        List<APIPart> new_parts = new List<APIPart>();
                        new_parts.Add(part);
                        ordered_parts.Add(part.pClass, new_parts);
                    }
                } else {
                    if (ordered_parts.Keys.Contains("Wiring")) { // Already added to dictionary
                        List<APIPart> existing_parts = ordered_parts.Where(x => x.Key == "Wiring").Select(x => x.Value).FirstOrDefault<List<APIPart>>();
                        existing_parts.Add(part);
                        ordered_parts["Wiring"] = existing_parts;
                    } else { // New Color Code
                        List<APIPart> new_parts = new List<APIPart>();
                        new_parts.Add(part);
                        ordered_parts.Add("Wiring", new_parts);
                    }
                }
            }

            ViewBag.parts = ordered_parts;
            ViewBag.partCount = part_count;
            ViewBag.year = year;
            ViewBag.make = make;
            ViewBag.model = model;
            ViewBag.style = style;

            return View();
        } // End Index
    } // End LookupController
} // End namespace
