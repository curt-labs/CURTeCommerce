using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class EDIController : BaseController {
        //
        // GET: /Index/
        public string Read() {
            Settings settings = new Settings();
            if (settings.Get("EDIOrderProcessing") == "true") {
                EDI edi = new EDI();
                edi.Read();
            }
            return "done";
        }

        public string Write() {
            EDI edi = new EDI();
            edi.Write();
            return "done";
        }
    }
}
