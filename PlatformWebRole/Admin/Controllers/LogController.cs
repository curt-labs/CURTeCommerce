using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Admin.Controllers {
    public class LogController : BaseController {
        //
        // GET: /Log/

        public ActionResult Index() {
            return View();
        }

        public ActionResult JS(bool isHttps = false) {
            /*XDocument xml = new XDocument();
            if (isHttps) {
                xml = XDocument.Load("http://" + Request.Url.Host);
            } else {
                xml = XDocument.Load("https://" + Request.Url.Host);
            }
            ViewBag.xml = xml;
            */
            return View();
        }

    }
}
