using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Controllers;

namespace Admin.Controllers {
    public class IndexController : BaseController {
        //
        // GET: /Index/

        public ActionResult Index() {
            return View();
        }

    }
}
