using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Net;
using System.IO;
using Admin.Models;

namespace Admin.Controllers {
    public class InvoiceController : BaseController {

        public ActionResult Index() {
            List<Invoice> invoices = new Invoice().GetAll();
            ViewBag.invoices = invoices;
            return View();
        }

        public ActionResult Details(int id = 0) {
            Invoice invoice = new Invoice().Get(id);
            ViewBag.invoice = invoice;
            return View();
        }

        [NoValidation]
        public ActionResult Paid(int id = 0) {
            Invoice invoice = new Invoice().Get(id);
            invoice.Paid();
            return RedirectToAction("Details",new {id = id});
        }
    }
}
