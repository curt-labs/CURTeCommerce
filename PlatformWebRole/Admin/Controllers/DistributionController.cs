using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers {
    public class DistributionController : BaseController {

        public ActionResult Index() {

            // Get all the CURT Distribution Centers
            List<DistributionCenter> dcList = DC.GetAll();
            ViewBag.dcList = dcList;

            return View();
        }

        public ActionResult Edit(int id = 0, bool failed = false) {
            ViewBag.errors = new List<string>();
            if (TempData["errors"] != null) {
                ViewBag.errors = TempData["errors"];
            }

            ViewBag.dc = new DistributionCenter();
            if (id > 0) {
                ViewBag.dc = DC.Get(id);
            }

            if (TempData["dc"] != null) {
                ViewBag.dc = TempData["dc"];
            }
            ViewBag.countries = UDF.GetCountries();
            ViewBag.failed = failed;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(int id = 0, string Name = "", string Phone = "", string Fax = "", string Street1 = "", string Street2 = "", string City = "", int State = 0, string PostalCode = "") {
            DistributionCenter dc = new DistributionCenter();
            List<string> errors = new List<string>();
            try {
                DC.Save(id, Name, Phone, Fax, Street1, Street2, City, State, PostalCode, out dc, out errors);

                if (errors.Count > 0) {
                    throw new Exception();
                }
                return RedirectToAction("Index", "Distribution");
            } catch (Exception) {
                TempData["errors"] = errors;
                TempData["dc"] = dc;
                return RedirectToAction("Edit", "Distribution", new { id = id, failed = true });
            }
        }

        public dynamic Delete(int id = 0) {
            try {
                DC.Delete(id);
                return RedirectToAction("Index", "Distribution");
            } catch (Exception) {
                List<string> errors = new List<string>();
                errors.Add("Failed to delete distribution center.");
                TempData["errors"] = errors;
                return RedirectToAction("Edit", "Distribution", new { id = id, failed = true });
            }
        }

    }
}
