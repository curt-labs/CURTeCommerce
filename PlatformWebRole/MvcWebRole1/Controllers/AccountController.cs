using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class AccountController : CustomerAuthController {

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

        public ActionResult Orders() {
            Customer cust = new Customer();
            cust.GetFromStorage();
            cust.BindOrders();

            ViewBag.cust = cust;
            return View();
        }

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

        public ActionResult Order(int id = 0) {
            Customer cust = new Customer();
            cust.ID = ViewBag.customer.ID;
            Cart order = cust.GetOrder(id);
            if (order == null || order.ID == 0) {
                return RedirectToAction("Orders", "Account");
            }
            Payment payment = order.getPayment();
            ViewBag.payment = payment;
            ViewBag.order = order;
            return View();
        }

        public ActionResult Save() {

            Customer cust = new Customer();
            Address billing = new Address();
            Address shipping = new Address();
            try {
                cust.GetFromStorage();
                cust.BindAddresses();

                #region Basic Information
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
                cust.Update(fname,lname,phone,receiveOffers,receiveNewsletter);
                #endregion

                #region Billing Information
                string bfirst = cust.Address.first;
                if (Request.Form["bfirst"] != null && Request.Form["bfirst"].Length > 0) {
                    bfirst = Request.Form["bfirst"];
                }
                string blast = cust.Address.last;
                if (Request.Form["blast"] != null && Request.Form["blast"].Length > 0) {
                    bfirst = Request.Form["blast"];
                }
                string bstreet1 = cust.Address.street1;
                if (Request.Form["bstreet1"] != null && Request.Form["bstreet1"].Length > 0) {
                    bstreet1 = Request.Form["bstreet1"];
                }
                string bstreet2 = cust.Address.street2;
                if (Request.Form["bstreet2"] != null && Request.Form["bstreet2"].Length > 0) {
                    bstreet2 = Request.Form["bstreet2"];
                }
                string bcity = cust.Address.city;
                if (Request.Form["bcity"] != null && Request.Form["bcity"].Length > 0) {
                    bcity = Request.Form["bcity"];
                }
                int bstate_id = cust.Address.state;
                try {
                    bstate_id = Convert.ToInt32(Request.Form["bstate"]);
                } catch (Exception) { }
                string bzip = cust.Address.postal_code;
                if (Request.Form["bzip"] != null && Request.Form["bzip"].Length > 0) {
                    bzip = Request.Form["bzip"];
                }
                bool bresidential = (Request.Form["bresidential"] == null) ? false : true;
                cust.UpdateAddress(cust.billingID,bfirst,blast,bstreet1,bstreet2,bcity,bstate_id,bzip,bresidential);
                #endregion

                #region Shipping Information
                string sfirst = cust.Address1.first;
                if (Request.Form["sfirst"] != null && Request.Form["sfirst"].Length > 0) {
                    sfirst = Request.Form["sfirst"];
                }
                string slast = cust.Address1.last;
                if (Request.Form["slast"] != null && Request.Form["slast"].Length > 0) {
                    slast = Request.Form["slast"];
                }
                string sstreet1 = cust.Address1.street1;
                if (Request.Form["sstreet1"] != null && Request.Form["sstreet1"].Length > 0) {
                    sstreet1 = Request.Form["sstreet1"];
                }
                string sstreet2 = cust.Address1.street2;
                if (Request.Form["sstreet2"] != null && Request.Form["sstreet2"].Length > 0) {
                    sstreet2 = Request.Form["sstreet2"];
                }
                string scity = cust.Address1.city;
                if (Request.Form["scity"] != null && Request.Form["scity"].Length > 0) {
                    scity = Request.Form["scity"];
                }
                int sstate_id = cust.Address1.state;
                try {
                    sstate_id = Convert.ToInt32(Request.Form["sstate"]);
                } catch (Exception) { }
                string szip = cust.Address1.postal_code;
                if (Request.Form["szip"] != null && Request.Form["szip"].Length > 0) {
                    szip = Request.Form["szip"];
                }
                bool sresidential = (Request.Form["sresidential"] == null) ? false : true;
                cust.UpdateAddress(cust.shippingID, sfirst, slast, sstreet1, sstreet2, scity, sstate_id, szip, sresidential);
                #endregion

                TempData["error"] = "You're account has been successfully updated.";
                return Redirect("/Account");
            } catch (Exception e) {
                TempData["customer"] = cust;
                TempData["billing"] = billing;
                TempData["shipping"] = shipping;
                TempData["error"] = "Failed to save your account information. " + e.Message + e.StackTrace;
                return Redirect("/Account");
            }
        }

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
