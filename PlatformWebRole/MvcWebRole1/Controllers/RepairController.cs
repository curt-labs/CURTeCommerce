using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class RepairController : BaseController {

        public ActionResult Index() {

            // Get the content page for Trailer Repair
            ContentPage page = ContentManagement.GetPageByTitle("Trailer Repair");
            ViewBag.page = page;

            return View();
        }

    }
}
