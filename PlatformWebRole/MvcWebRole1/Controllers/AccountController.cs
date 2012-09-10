using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class AccountController : CustomerAuthController {

        [RequireHttps]
        public ActionResult Index() {

            // Instantiate our Customer object
            Customer cust = new Customer();

            // Retrieve from Session/Cookie
            cust.GetFromStorage();

            // Get the Customer record
            cust.Get();

            cust.BindAddresses();

            ViewBag.countries = UDF.GetCountries();
            ViewBag.cust = cust;
            ViewBag.error = TempData["error"];
            return View();
        }

        [RequireHttps]
        public ActionResult Orders() {
            Customer cust = new Customer();
            cust.GetFromStorage();
            cust.BindOrders();

            ViewBag.cust = cust;
            return View();
        }

        [RequireHttps]
        public ActionResult Password(string message = "") {
            ViewBag.message = message;
            Customer cust = new Customer();
            cust.GetFromStorage();
            ViewBag.cust = cust;
            return View();
        }

        [RequireHttps,AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResetPassword() {
            Customer cust = new Customer();
            cust.GetFromStorage();
            string message = "";
            try {
                string current = Request.Form["current"];
                string newpw = Request.Form["new"];
                string confirm = Request.Form["confirm"];

                if (String.IsNullOrEmpty(current) || String.IsNullOrEmpty(newpw) || String.IsNullOrEmpty(confirm)) {
                    throw new Exception("You must enter all password fields. Try Again");
                }

                cust.ValidateCurrentPassword(current);

                cust.ValidatePasswords(newpw, confirm);
                cust.UpdatePassword();
                message = "Your password was successfully updated.";

            } catch (Exception e) {
                message = e.Message;
            }
            return RedirectToAction("Password", new { message = message });
        }

        [RequireHttps]
        public ActionResult Addresses() {
            Customer cust = ViewBag.customer;
            List<Address> addresses = cust.GetAddresses();
            List<Country> countries = UDF.GetCountries();
            ViewBag.countries = countries;
            cust.BindAddresses();
            ViewBag.cust = cust;
            ViewBag.addresses = addresses;
            return View();
        }

        [RequireHttps]
        public ActionResult Order(int id = 0) {
            Customer cust = new Customer();
            cust.ID = ViewBag.customer.ID;
            Cart order = cust.GetOrderByPayment(id);
            if (order == null || order.ID == 0) {
                return RedirectToAction("Orders", "Account");
            }
            Payment payment = order.getPayment();
            ViewBag.payment = payment;
            ViewBag.order = order;
            return View();
        }

        [RequireHttps]
        public ActionResult Save() {

            Customer cust = new Customer();
            try {
                cust.GetFromStorage();

                #region Basic Information
                string email = cust.email;
                if (Request.Form["email"] != null && Request.Form["email"].Length > 0) {
                    email = Request.Form["email"];
                }
                if (email != cust.email) {
                    // Make sure we don't have an account with this e-mail address
                    if (Customer.CheckCustomerEmail(email)) {
                        throw new Exception("An account using the E-Mail address you provided already exists.");
                    }
                }
                string fname = cust.fname;
                if(Request.Form["fname"] != null && Request.Form["fname"].Length > 0){
                    fname = Request.Form["fname"];
                }
                string lname = cust.lname;
                if(Request.Form["lname"] != null && Request.Form["lname"].Length > 0){
                    lname = Request.Form["lname"];
                }
                string phone = cust.phone;
                if(Request.Form["phone"] != null && Request.Form["phone"].Length > 0){
                    phone = Request.Form["phone"];
                }
                int receiveOffers = cust.receiveOffers;
                int receiveNewsletter = cust.receiveNewsletter;
                if (Request.Form["receiveOffers"] != null) {
                    try {
                        receiveOffers = Convert.ToInt32(Request.Form["receiveOffers"]);
                    } catch (Exception) { }
                } else {
                    receiveOffers = 0;
                }
                if (Request.Form["receiveNewsletter"] != null) {
                    try {
                        receiveNewsletter = Convert.ToInt32(Request.Form["receiveNewsletter"]);
                    } catch (Exception) { }
                } else {
                    receiveNewsletter = 0;
                }
                cust.Update(email,fname,lname,phone,receiveOffers,receiveNewsletter);
                #endregion

                TempData["error"] = "You're account has been successfully updated.";
                return Redirect("/Account");
            } catch (Exception e) {
                if (e.Message.ToLower().Contains("a potentially dangerous")) {
                    throw new HttpException(403, "Forbidden");
                }
                TempData["customer"] = cust;
                TempData["error"] = "Failed to save your account information. " + e.Message + e.StackTrace;
                return Redirect("/Account");
            }
        }

        [RequireHttps]
        public ActionResult AddAddress() {
            try {
                // Create Customer
                Customer customer = new Customer();
                customer.GetFromStorage();
                if (!customer.LoggedIn()) {
                    return RedirectToAction("Index", "Authenticate");
                }

                Address address = new Address();
                // Build out our Billing object
                address = new Address {
                    first = Request.Form["first"],
                    last = Request.Form["last"],
                    street1 = Request.Form["street1"],
                    street2 = (Request.Form["street2"].Trim() == "") ? null : Request.Form["street2"].Trim(),
                    city = Request.Form["city"],
                    postal_code = Request.Form["zip"],
                    residential = (Request.Form["residential"] == null) ? false : true,
                    active = true
                };
                try {
                    address.state = Convert.ToInt32(Request.Form["state"]);
                } catch (Exception) {
                    throw new Exception("You must select a state/province.");
                }
                address.Save(customer.ID);

            } catch (Exception e) {
                if (e.Message.ToLower().Contains("a potentially dangerous")) {
                    throw new HttpException(403, "Forbidden");
                }
            }
            return RedirectToAction("Addresses");

        }

        [RequireHttps]
        public ActionResult DeleteAddress(int id = 0) {
            Customer cust = new Customer();
            cust.GetFromStorage();
            Address a = new Address().Get(id);
            cust.ClearAddress(a.ID);
            if (a.cust_id == cust.ID) {
                a.Delete(id);
            }
            return RedirectToAction("Addresses");
        }

        [RequireHttps]
        public ActionResult SetBillingDefault(int id = 0) {
            Customer cust = new Customer();
            cust.GetFromStorage();
            Address a = new Address().Get(id);
            if (a.cust_id == cust.ID) {
                cust.SetBillingDefaultAddress(id);
                cust.BindAddresses();
            }
            return RedirectToAction("Addresses");
        }

        [RequireHttps]
        public ActionResult SetShippingDefault(int id = 0) {
            Customer cust = new Customer();
            cust.GetFromStorage();
            Address a = new Address().Get(id);
            if (a.cust_id == cust.ID) {
                cust.SetShippingDefaultAddress(id);
                cust.BindAddresses();
            }
            return RedirectToAction("Addresses");
        }

    }
}
