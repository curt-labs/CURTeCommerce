using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers {
    public class CustomersController : BaseController {

        public ActionResult Index() {

            List<Customer> custs = new Customer().GetAll();
            ViewBag.custs = custs;

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
