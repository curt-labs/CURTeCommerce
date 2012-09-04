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
            IOrderedQueryable<InvoiceCount> invoices = new Invoice().GetAll();
            ViewBag.invoices = invoices;
            return View();
        }

        public ActionResult List(DateTime date) {
            ViewBag.invoicedate = date;
            
            List<Invoice> invoices = new Invoice().GetAllByDate(date);
            ViewBag.invoices = invoices;
            return View();
        }

        public ActionResult Print(DateTime date) {
            List<Invoice> invoices = new Invoice().GetAllByDate(date);
            foreach (Invoice invoice in invoices) {
                invoice.LoadOrder();
                invoice.Print();
            }
            ViewBag.invoices = invoices;
            return View();
        }

        public ActionResult Details(int id = 0) {
            Invoice invoice = new Invoice().Get(id);
            ViewBag.invoice = invoice;
            Cart order = new Cart();
            try {
                order = new Cart().GetByPayment(Convert.ToInt32(invoice.orderID));
            } catch { }
            ViewBag.order = order;
            return View();
        }

        public ActionResult UnPrinted() {
            List<Invoice> invoices = new Invoice().GetAllUnprinted();
            ViewBag.invoices = invoices;
            return View();
        }

        [NoValidation]
        public ActionResult Paid(int id = 0) {
            Invoice invoice = new Invoice().Get(id);
            invoice.Paid();
            return RedirectToAction("Details",new {id = id});
        }

        [NoValidation]
        public string PrintInvoice(int id = 0) {
            Invoice invoice = new Invoice().Get(id);
            invoice.Print();
            return "success";
        }
    }
}
