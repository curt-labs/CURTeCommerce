using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers {
    public class ThemesController : BaseController {

        public ActionResult Index() {
            return View();
        }
    }
}
