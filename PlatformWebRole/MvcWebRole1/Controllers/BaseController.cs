using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using System.Web.Routing;
using System.Security;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EcommercePlatform.Controllers {
    public class BaseController : Controller {

        protected override void Initialize(System.Web.Routing.RequestContext requestContext) {
            base.Initialize(requestContext);
            HttpContext ctx = System.Web.HttpContext.Current;

            ViewBag.year = UDF.GetYearCookie(ctx);
            ViewBag.make = UDF.GetMakeCookie(ctx);
            ViewBag.model = UDF.GetModelCookie(ctx);
            ViewBag.style = UDF.GetStyleCookie(ctx);
            ViewBag.vehicleID = UDF.GetVehicleCookie(ctx);

            // Get the theme ID
            int themeID = new Theme().getTheme(ctx);
            ViewBag.themeID = themeID;

            if (themeID > 0) {
                // if there is an active theme, get the files
                string cname = this.ControllerContext.Controller.ToString();
                Dictionary<int, List<ThemeFile>> themefiles = new Theme().getFiles(ctx,UDF.GetControllerName(cname));
                ViewBag.themefiles = themefiles;
            }

            // We're gonna dump our Customer Session object out
            Customer customer = new Customer();
            customer.GetFromStorage(ctx);


            Settings settings = new Settings();
            ViewBag.settings = settings;

            ViewBag.customer = customer;
        }

    }
}
