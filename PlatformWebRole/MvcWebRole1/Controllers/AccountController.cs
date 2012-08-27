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
            Cart order = cust.GetOrderByPayment(id);
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
            try {
                cust.GetFromStorage();

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

                TempData["error"] = "You're account has been successfully updated.";
                return Redirect("/Account");
            } catch (Exception e) {
                TempData["customer"] = cust;
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
