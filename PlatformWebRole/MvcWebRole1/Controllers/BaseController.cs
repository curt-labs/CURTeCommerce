using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using System.Web.Routing;
using System.Security;
using Newtonsoft.Json;

namespace EcommercePlatform.Controllers {
    public class BaseController : Controller {

        protected override void Initialize(System.Web.Routing.RequestContext requestContext) {
            base.Initialize(requestContext);

            // Get the vehicle years
            List<double> years = CURTAPI.GetYears();
            ViewBag.years = years;

            // Get the theme ID
            int themeID = new Theme().getTheme();
            ViewBag.themeID = themeID;

            if (themeID > 0) {
                // if there is an active theme, get the files
                string cname = this.ControllerContext.Controller.ToString();
                Dictionary<int, List<ThemeFile>> themefiles = new Theme().getFiles(UDF.GetControllerName(cname));
                ViewBag.themefiles = themefiles;
            }

            // Get the parent categories
            List<APICategory> parent_cats = CURTAPI.GetParentCategories();
            ViewBag.parent_cats = parent_cats;

            // We're gonna dump our Customer Session object out
            Customer customer = new Customer();
            customer.GetFromStorage();

            Settings settings = new Settings();
            ViewBag.settings = settings;

            ViewBag.customer = customer;

            // Get all the cookies
            HttpCookie vehicleYear = Request.Cookies.Get("vehicle_year");
            ViewBag.year = (vehicleYear != null && vehicleYear.Value != null) ? vehicleYear.Value.ToString() : "";

            HttpCookie vehicleMake = Request.Cookies.Get("vehicle_make");
            ViewBag.make = (vehicleMake != null && vehicleMake.Value != null) ? vehicleMake.Value.ToString() : "";

            HttpCookie vehicleModel = Request.Cookies.Get("vehicle_model");
            ViewBag.model = (vehicleModel != null && vehicleModel.Value != null) ? vehicleModel.Value.ToString() : "";

            HttpCookie vehicleStyle = Request.Cookies.Get("vehicle_style");
            ViewBag.style = (vehicleStyle != null && vehicleStyle.Value != null) ? vehicleStyle.Value.ToString() : "";

        }

    }
}
