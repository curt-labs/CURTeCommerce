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

namespace EcommercePlatform.Controllers {
    public class PaymentController : BaseController {

        protected GCheckoutButton gButton = new GCheckoutButton();

        [RequireHttps]
        public ActionResult Index(string message = "") {
            Customer customer = new Customer();

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();
            if (!customer.LoggedIn()) {
                return RedirectToAction("Index", "Authenticate", new { referrer = "https://" + Request.Url.Host + "/Cart/Checkout" });
            }

            // Create Cart object from customer
            customer.BindAddresses();
            Cart cart = customer.Cart;

            // Get the parts from this Cart
            cart.GetParts();

            ViewBag.showShipping = true;
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
            Customer customer = new Customer();
            Settings settings = ViewBag.settings;
            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();
            if (!customer.LoggedIn()) {
                return RedirectToAction("Index", "Authenticate", new { referrer = "https://" + Request.Url.Host + "/Cart/Checkout" });
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

            //step 3 - make some money
            IGatewayResponse response = gate.Send(request);
            if (response.Approved) {
                customer.Cart.AddPayment("credit card",response.AuthorizationCode,"Complete");
                customer.Cart.SendConfirmation();
                int cartid = customer.Cart.ID;
                
                Cart new_cart = new Cart().Save();
                new_cart.UpdateCart(customer.ID);
                DateTime cookexp = Request.Cookies["hdcart"].Expires;
                HttpCookie cook = new HttpCookie("hdcart", new_cart.ID.ToString());
                cook.Expires = cookexp;
                Response.Cookies.Add(cook);

                customer.Cart = new_cart;
                customer.Cart.BindAddresses();

                EDI edi = new EDI();
                edi.CreatePurchaseOrder(cartid); 

                return RedirectToAction("Complete", new { id = cartid });
            } else {
                return RedirectToAction("Index", new { message = response.Message });
            }
        }

        [RequireHttps]
        public ActionResult Google() {

            Customer customer = ViewBag.customer;
            customer.GetFromStorage();
            if (!customer.LoggedIn()) {
                return RedirectToAction("Index", "Authenticate", new { referrer = "https://" + Request.Url.Host + "/Cart/Checkout" });
            }

            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Settings settings = ViewBag.settings;
            CheckoutShoppingCartRequest req = gButton.CreateRequest();
            if (Request.Url.Host.Contains("127.0.0") || Request.Url.Host.Contains("localhost") || settings.Get("GoogleCheckoutEnv") == "override") {
                req.MerchantID = settings.Get("GoogleDevMerchantId");
                req.MerchantKey = settings.Get("GoogleDevMerchantKey");
                req.Environment = GCheckout.EnvironmentType.Sandbox;
            } else {
                req.MerchantID = settings.Get("GoogleMerchantId");
                req.MerchantKey = settings.Get("GoogleMerchantKey");
                req.Environment = GCheckout.EnvironmentType.Production;
            }
            if (settings.Get("GoogleAnalyticsCode") != "") {
                req.AnalyticsData = Request.Form["analyticsdata"];
            }
            req.ContinueShoppingUrl = Request.Url.Scheme + "://" + Request.Url.Host;
            //req.EditCartUrl = Request.Url.Host + "/Cart";

            foreach (CartItem item in customer.Cart.CartItems) {
                ShoppingCartItem sitem = new ShoppingCartItem {
                    Name = "CURT Part #" + item.partID,
                    Description = item.shortDesc,
                    Price = item.price,
                    Quantity = item.quantity,
                    Weight = Convert.ToDouble(item.weight)
                };
                req.AddItem(sitem);
            }
            System.Xml.XmlDocument tempDoc = new System.Xml.XmlDocument();
            System.Xml.XmlNode tempNode = tempDoc.CreateElement("OrderNumber");
            tempNode.InnerText = customer.Cart.ID.ToString();
            req.AddMerchantPrivateDataNode(tempNode);

            req.AddShippingPackage("0", customer.Cart.Shipping.city, customer.Cart.Shipping.State1.state1, customer.Cart.Shipping.postal_code);
            req.AddFlatRateShippingMethod(customer.Cart.shipping_type, customer.Cart.shipping_price);

            Country country = db.Countries.Where(x => x.abbr.Equals("US")).FirstOrDefault();
            if(country != null) {
                foreach(State state in country.States) {
                    req.AddStateTaxRule(state.abbr, Convert.ToDouble(state.taxRate / 100), true);
                }
            }
            
            GCheckoutResponse resp = req.Send();
            if (resp.IsGood) {
                customer.Cart.AddPayment("Google Checkout", "", "Pending");
                
                Cart new_cart = new Cart().Save();
                new_cart.UpdateCart(customer.ID);
                DateTime cookexp = Request.Cookies["hdcart"].Expires;
                HttpCookie cook = new HttpCookie("hdcart", new_cart.ID.ToString());
                cook.Expires = cookexp;
                Response.Cookies.Add(cook);
                
                customer.Cart = new_cart;
                customer.Cart.BindAddresses();

                return Redirect(resp.RedirectUrl);
            } else {
                return RedirectToAction("Index", new { message = resp.ErrorMessage });
            }
        }
        
        [RequireHttps]
        public ActionResult PayPal() {
            Customer customer = ViewBag.customer;
            customer.GetFromStorage();
            if (!customer.LoggedIn()) {
                return RedirectToAction("Index", "Authenticate", new { referrer = "https://" + Request.Url.Host + "/Cart/Checkout" });
            }
            Paypal p = new Paypal();
            string token = p.ECSetExpressCheckout(customer.Cart);
            if (!token.ToLower().Contains("failure")) {
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
            Customer customer = ViewBag.customer;
            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();
            if (!customer.LoggedIn()) {
                return RedirectToAction("Index", "Authenticate", new { referrer = "https://" + Request.Url.Host + "/Cart/Checkout" });
            }

            // Create Cart object from customer
            customer.BindAddresses();
            Cart cart = customer.Cart;

            // Get the parts from this Cart
            cart.GetParts();

            ViewBag.showShipping = true;
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
            Customer customer = ViewBag.customer;
            customer.GetFromStorage();
            if (!customer.LoggedIn()) {
                return RedirectToAction("Index", "Authenticate", new { referrer = "https://" + Request.Url.Host + "/Cart/Checkout" });
            }
            decimal total = customer.Cart.getTotal();
            Paypal p = new Paypal();
            string confirmationKey = p.ECDoExpressCheckout(token, payerID, total.ToString(), customer.Cart);
            if (confirmationKey == "Success") {
                customer.Cart.AddPayment("PayPal", token, "Complete");
                customer.Cart.SendConfirmation();
                int cartid = customer.Cart.ID;

                Cart new_cart = new Cart().Save();
                new_cart.UpdateCart(customer.ID);
                DateTime cookexp = Request.Cookies["hdcart"].Expires;
                HttpCookie cook = new HttpCookie("hdcart", new_cart.ID.ToString());
                cook.Expires = cookexp;
                Response.Cookies.Add(cook);

                customer.Cart = new_cart;
                customer.Cart.BindAddresses();

                EDI edi = new EDI();
                edi.CreatePurchaseOrder(cartid); 
                return RedirectToAction("Complete", new { id = cartid });
            } else {
                return RedirectToAction("Index", new { message = "Your PayPal Transaction Could not be processed. Try Again." });
            }

        }

        [RequireHttps]
        public ActionResult Complete(int id = 0) {
            Customer customer = new Customer();

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();
            Cart order = new Cart().Get(id);

            if (!customer.LoggedIn() || order.cust_id != customer.ID) {
                return RedirectToAction("Index", "Authenticate");
            }

            order.BindAddresses();
            Payment payment = order.getPayment();

            ViewBag.order = order;
            ViewBag.payment = payment;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void GoogleNotification() {
            string serial = Request.Form["serial-number"] ?? "";
            
            string resp = "<notification-acknowledgment xmlns=\"http://checkout.google.com/schema/2\" serial-number=\"" + serial + "\" />";
            Response.Write(resp);
            Response.End();
            GoogleCheckout.getNotification(serial);
        }

    }
}
