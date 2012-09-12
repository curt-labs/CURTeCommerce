using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers {
    public class CustomersController : BaseController {

        public ActionResult Index(int page = 1, int perpage = 10) {

            int count = new Customer().Count();
            int pages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(count) / perpage));
            if (page > pages) {
                page = pages;
            }
            List<AdminCustomer> custs = new Customer().GetCustomersByPage(page, perpage);
            ViewBag.custs = custs;
            ViewBag.page = page;
            ViewBag.pages = pages;
            ViewBag.perpage = perpage;
            ViewBag.count = count;

            string error = "";
            if (TempData["error"] != null) {
                error = TempData["error"].ToString();
            }
            ViewBag.error = error;
            return View();
        }

        public ActionResult Info(int id = 0){

            try {
                if (id == 0) {
                    throw new Exception("Invalid reference to Customer");
                }
                Customer cust = new Customer {
                    ID = id
                };
                cust.Get();
                ViewBag.customer = cust;

                return View();
            } catch (Exception e) {
                TempData["error"] = e.Message;
                return RedirectToAction("Index");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public dynamic Save(int id = 0, string email = "", string fname = "", string lname = "") {
            string error = "";
            try {
                Customer cust = new Customer { ID = id };
                cust.Get();
                cust.Update(email, fname, lname);

            } catch (Exception e) {
                error = e.Message;
            }

            return RedirectToAction("Info", "Customers", new { id = id });
        }

        [NoValidation]
        public string Search(string searchtext = "") {
            return AdminCustomer.Search(searchtext);
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public dynamic Suspension(int id = 0, bool ajax = false) {
            string error = "";
            try{
                Customer cust = new Customer { ID = id };
                cust.Get();
                cust.ToggleSuspended();

            }catch(Exception e){
                error = e.Message;
            }
            if (ajax) {
                return error;
            } else {
                return RedirectToAction("Info", "Customer", new { id = id });
            }
        }

    }
}
