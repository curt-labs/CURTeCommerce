using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GCheckout.Checkout;
using GCheckout.Util;
using AuthorizeNet;
using AuthorizeNet.Helpers;
using EcommercePlatform.Models;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace EcommercePlatform.Controllers {
    public class PaymentController : BaseController {

        protected GCheckoutButton gButton = new GCheckoutButton();

        [RequireHttps]
        public async Task<ActionResult> Index(string message = "") {
            HttpContext ctx = System.Web.HttpContext.Current;
            Customer customer = new Customer();
            ViewBag.timezone = UDF.GetTimeZone(ctx);

            var pcats = CURTAPI.GetParentCategoriesAsync();
            await Task.WhenAll(new Task[] { pcats });
            ViewBag.parent_cats = await pcats;

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage(ctx);

            if (!customer.Cart.Validate()) {
                return RedirectToAction("Index", "Cart");
            }

            if (customer.Cart.payment_id > 0) {
                UDF.ExpireCart(ctx, customer.ID);
                return RedirectToAction("Index", "Cart");
            }
            // Create Cart object from customer
            customer.BindAddresses();
            Cart cart = customer.Cart;

            // Get the parts from this Cart
            cart.GetParts();

            ViewBag.showTotal = true;
            ViewBag.cart = cart;
            ViewBag.message = message;
            List<int> months = new List<int>();
            for (int i = 1; i <= 12; i++) {
                months.Add(i);
            }
            List<int> yearlist = new List<int>();
            for (int i = DateTime.Now.Year; i <= (DateTime.Now.Year + 20); i++) {
                yearlist.Add(i);
            }
            ViewBag.months = months;
            ViewBag.yearlist = yearlist;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post),RequireHttps]
        public ActionResult Authorize() {
            HttpContext ctx = System.Web.HttpContext.Current;
            Customer customer = new Customer();
            Settings settings = ViewBag.settings;
            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage(ctx);
            if (!customer.Cart.Validate()) {
                return RedirectToAction("Index", "Cart");
            }

            if (customer.Cart.GetPaymentID() > 0) {
                UDF.ExpireCart(ctx, customer.ID);
                return RedirectToAction("Index", "Cart");
            }

            customer.BindAddresses();

            decimal amount = customer.Cart.getTotal();
            string cardnum = Request.Form["cardnumber"];
            string month = Request.Form["expiremonth"];
            string year = Request.Form["expireyear"];
            string cvv = Request.Form["cvv"];
            string first = Request.Form["first"];
            string last = Request.Form["last"];

            //step 1 - create the request
            IGatewayRequest request = new AuthorizationRequest(cardnum, month + year, amount, "Transaction");

            //These are optional calls to the API
            request.AddCardCode(cvv);

            //Customer info - this is used for Fraud Detection
            request.AddCustomer(customer.ID.ToString(), first, last, customer.Cart.Billing.street1 + ((customer.Cart.Billing.street2 != "") ? " " + customer.Cart.Billing.street2 : ""), customer.Cart.Billing.State1.abbr, customer.Cart.Billing.postal_code);

            //order number
            //request.AddInvoice("invoiceNumber");

            //Custom values that will be returned with the response
            //request.AddMerchantValue("merchantValue", "value");

            //Shipping Address
            request.AddShipping(customer.ID.ToString(), customer.Cart.Shipping.first, customer.Cart.Shipping.last, customer.Cart.Shipping.street1 + ((customer.Cart.Shipping.street2 != "") ? " " + customer.Cart.Shipping.street2 : ""), customer.Cart.Shipping.State1.abbr, customer.Cart.Shipping.postal_code);


            //step 2 - create the gateway, sending in your credentials and setting the Mode to Test (boolean flag)
            //which is true by default
            //this login and key are the shared dev account - you should get your own if you 
            //want to do more testing
            bool testmode = false;
            if (settings.Get("AuthorizeNetTestMode").Trim() == "true") {
                testmode = true;
            }

            Gateway gate = new Gateway(settings.Get("AuthorizeNetLoginKey"), settings.Get("AuthorizeNetTransactionKey"), testmode);
            customer.Cart.SetStatus((int)OrderStatuses.PaymentPending);

            //step 3 - make some money
            IGatewayResponse response = gate.Send(request);
            if (response.Approved) {
                customer.Cart.AddPayment("credit card", response.AuthorizationCode, "Complete");
                customer.Cart.SetStatus((int)OrderStatuses.PaymentComplete);
                customer.Cart.SendConfirmation(ctx);
                customer.Cart.SendInternalOrderEmail(ctx);
                int cartid = customer.Cart.ID;
                
                Cart new_cart = new Cart().Save();
                new_cart.UpdateCart(ctx, customer.ID);
                DateTime cookexp = Request.Cookies["hdcart"].Expires;
                HttpCookie cook = new HttpCookie("hdcart", new_cart.ID.ToString());
                cook.Expires = cookexp;
                Response.Cookies.Add(cook);

                customer.Cart = new_cart;
                customer.Cart.BindAddresses();

                return RedirectToAction("Complete", new { id = cartid });
            } else {
                customer.Cart.SetStatus((int)OrderStatuses.PaymentDeclined);
                return RedirectToAction("Index", new { message = response.Message });
            }
        }

        [RequireHttps]
        public ActionResult PayPal() {
            HttpContext ctx = System.Web.HttpContext.Current;
            Customer customer = ViewBag.customer;
            customer.GetFromStorage(ctx);
            if (!customer.Cart.Validate()) {
                return RedirectToAction("Index", "Cart");
            }
            if (customer.Cart.GetPaymentID() > 0) {
                UDF.ExpireCart(ctx, customer.ID);
                return RedirectToAction("Index", "Cart");
            }
            Paypal p = new Paypal();
            string token = p.ECSetExpressCheckout(customer.Cart);
            if (!token.ToLower().Contains("failure")) {
                customer.Cart.SetStatus((int)OrderStatuses.PaymentPending);
                customer.Cart.paypalToken = token;
                if (Request.Url.Host.Contains("127.0.0") || Request.Url.Host.Contains("localhost")) {
                    return Redirect("https://www.sandbox.paypal.com/webscr?cmd=_express-checkout&token=" + token);
                } else {
                    return Redirect("https://www.paypal.com/webscr?cmd=_express-checkout&token=" + token);
                }
            } else {
                return RedirectToAction("Index", new { message = "There was a problem with your PayPal transaction. " + token });
            }
        }
        
        [RequireHttps]
        public ActionResult PayPalCheckout(string token = "", string payerID = "") {
            HttpContext ctx = System.Web.HttpContext.Current;
            Customer customer = ViewBag.customer;
            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage(ctx);

            // Create Cart object from customer
            customer.BindAddresses();
            Cart cart = customer.Cart;

            // Get the parts from this Cart
            cart.GetParts();

            ViewBag.showTotal = true;
            ViewBag.showQty = false;
            ViewBag.cart = cart;
            ViewBag.message = TempData["message"];
            Paypal p = new Paypal();
            PayPalResponse paypalResponse = p.ECGetExpressCheckout(token);
            if (paypalResponse.acknowledgement == "Success") {
                ViewBag.paypalResponse = paypalResponse;
                return View();
            }
            return RedirectToAction("Index", new { message = "Your PayPal Transaction Could not be processed. Try Again." });
        }

        [RequireHttps]
        public ActionResult CompletePayPalCheckout(string token = "", string payerID = "") {
            HttpContext ctx = System.Web.HttpContext.Current;
            Customer customer = ViewBag.customer;
            customer.GetFromStorage(ctx);
            if (!customer.Cart.Validate()) {
                return RedirectToAction("Index", "Cart");
            }
            if (customer.Cart.GetPaymentID() > 0) {
                UDF.ExpireCart(ctx, customer.ID);
                return RedirectToAction("Index", "Cart");
            }
            if (customer.Cart.GetStatus().statusID != (int)OrderStatuses.PaymentPending) {
                UDF.ExpireCart(ctx, customer.ID);
                return RedirectToAction("Index", "Cart");
            };
            decimal total = customer.Cart.getTotal();
            Paypal p = new Paypal();
            string confirmationKey = p.ECDoExpressCheckout(token, payerID, total.ToString(), customer.Cart);
            if (confirmationKey == "Success") {
                customer.Cart.AddPayment("PayPal", token, "Complete");
                customer.Cart.SetStatus((int)OrderStatuses.PaymentComplete);
                customer.Cart.SendConfirmation(ctx);
                customer.Cart.SendInternalOrderEmail(ctx);
                int cartid = customer.Cart.ID;

                Cart new_cart = new Cart().Save();
                new_cart.UpdateCart(ctx, customer.ID);
                DateTime cookexp = Request.Cookies["hdcart"].Expires;
                HttpCookie cook = new HttpCookie("hdcart", new_cart.ID.ToString());
                cook.Expires = cookexp;
                Response.Cookies.Add(cook);

                customer.Cart = new_cart;
                customer.Cart.BindAddresses();

                return RedirectToAction("Complete", new { id = cartid });
            } else {
                return RedirectToAction("Index", new { message = "Your PayPal Transaction Could not be processed. Try Again." });
            }

        }

        [RequireHttps]
        public async Task<ActionResult> Complete(int id = 0) {
            HttpContext ctx = System.Web.HttpContext.Current;
            Customer customer = new Customer();
            ViewBag.timezone = UDF.GetTimeZone(ctx);

            var pcats = CURTAPI.GetParentCategoriesAsync();
            await Task.WhenAll(new Task[] { pcats });
            ViewBag.parent_cats = await pcats;

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage(ctx);
            Cart order = new Cart().Get(id);

            order.BindAddresses();
            Payment payment = order.getPayment();

            ViewBag.order = order;
            ViewBag.payment = payment;

            return View();
        }

    }
}
