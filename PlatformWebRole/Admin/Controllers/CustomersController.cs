using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using Newtonsoft.Json;

namespace Admin.Controllers {
    public class CustomersController : BaseController {

        public ActionResult Index(int page = 1, int perpage = 10) {

            int count = new Customer().Count();
            int pages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(count) / perpage));
            if (page > pages) {
                page = pages;
            }
            List<AdminCustomer> custs = new List<AdminCustomer>();
            if (page > 0) {
                custs = new Customer().GetCustomersByPage(page, perpage);
            }
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

        public ActionResult Info(int id = 0, string message = "", string error = ""){

            try {
                if (id == 0) {
                    throw new Exception("Invalid reference to Customer");
                }
                Customer cust = new Customer {
                    ID = id
                };
                cust.Get();
                ViewBag.customer = cust;
                ViewBag.addresses = cust.GetAddresses();
                ViewBag.countries = UDF.GetCountries();
                ViewBag.message = message;
                ViewBag.error = error;

                return View();
            } catch (Exception e) {
                TempData["error"] = e.Message;
                return RedirectToAction("Index");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public dynamic Save(int id = 0, string email = "", string fname = "", string lname = "", string phone = "") {
            string error = "";
            try {
                Customer cust = new Customer { ID = id };
                cust.Get();
                cust.Update(email, fname, lname, phone);

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

        [NoValidation]
        public string GetAddress(int id) {
            Address a = new Address().Get(id);
            return JsonConvert.SerializeObject(a);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveAddress() {
            int custID = (Request.Form["id"].Trim() == "") ? 0 : Convert.ToInt32(Request.Form["id"]);
            int addressID = (Request.Form["addressID"].Trim() == "") ? 0 : Convert.ToInt32(Request.Form["addressID"]);
            if (custID == 0) {
                return RedirectToAction("Index");
            }
            try {
                Address address = new Address();
                int stateID = 0;
                try {
                    stateID = Convert.ToInt32(Request.Form["state"]);
                } catch (Exception) {
                    throw new Exception("You must select a state/province.");
                }
                if (addressID == 0) {
                    // Build out our Billing object
                    address = new Address {
                        first = Request.Form["first"].Trim(),
                        last = Request.Form["last"].Trim(),
                        street1 = Request.Form["street1"].Trim(),
                        street2 = (Request.Form["street2"].Trim() == "") ? null : Request.Form["street2"].Trim(),
                        city = Request.Form["city"].Trim(),
                        state = stateID,
                        postal_code = Request.Form["postalcode"].Trim(),
                        residential = (Request.Form["residential"] == null) ? false : true,
                        active = true
                    };
                    address.Save(custID);
                } else {
                    address = new Address().Get(addressID);
                    address.Update(Request.Form["first"].Trim(), Request.Form["last"].Trim(), Request.Form["street1"].Trim(), (Request.Form["street2"].Trim() == "") ? null : Request.Form["street2"].Trim(), Request.Form["city"].Trim(), stateID, Request.Form["postalcode"].Trim(), (Request.Form["residential"] == null) ? false : true);
                }

            } catch (Exception e) {
                if (e.Message.ToLower().Contains("a potentially dangerous")) {
                    throw new HttpException(403, "Forbidden");
                }
            }
            return new RedirectResult(Url.Action("Info",new { id = custID }) + "#addresses");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string DeleteAddress(int id = 0) {
            Address a = new Address().Get(id);
            Customer c = new Customer {
                ID = a.cust_id
            };
            c.Get();
            if (c.billingID == a.ID) {
                c.SetDefaultAddress(0, "billing");
            }
            if (c.shippingID == a.ID) {
                c.SetDefaultAddress(0,"shipping");
            }
            bool success = a.Delete(id);
            return "{\"success\":" + success.ToString().ToLower() + "}";
        }

        [NoValidation]
        public ActionResult SetBillingDefault(int id) {
            Address a = new Address().Get(id);
            Customer c = new Customer { ID = a.cust_id };
            c.Get();
            c.SetDefaultAddress(id, "billing");
            return new RedirectResult(Url.Action("Info", new { id = a.cust_id }) + "#addresses");
        }

        [NoValidation]
        public ActionResult SetShippingDefault(int id) {
            Address a = new Address().Get(id);
            Customer c = new Customer { ID = a.cust_id };
            c.Get();
            c.SetDefaultAddress(id, "shipping");
            return new RedirectResult(Url.Action("Info", new { id = a.cust_id }) + "#addresses");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangePassword() {
            int custID = (Request.Form["customerID"].Trim() == "") ? 0 : Convert.ToInt32(Request.Form["customerID"]);
            bool notify = (Request.Form["notify"] == null) ? false : true;
            string password = Request.Form["password"];
            string confirm = Request.Form["confirmpassword"];
            string message = "";
            string error = "";
            try {
                Customer cust = new Customer { ID = custID };
                cust.Get();

                if (String.IsNullOrEmpty(password) || String.IsNullOrEmpty(confirm)) {
                    throw new Exception("You must enter all password fields. Try Again");
                }

                cust.ValidatePasswords(password, confirm);
                cust.UpdatePassword();
                if (notify) {
                    cust.plainpassword = password;
                    cust.SendChangedPasswordNotification();
                }
            } catch (Exception e) {
                error = e.Message;
            }
            return new RedirectResult(Url.Action("Info", new { id = custID, message = message, error = error }) + "#password");
        }
    }
}
