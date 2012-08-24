using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Net;
using System.IO;
using GCheckout;
using GCheckout.Util;
using GCheckout.AutoGen;
using Admin.Models;
using AuthorizeNet;
using AuthorizeNet.Helpers;

namespace Admin.Controllers {
    public class OrdersController : BaseController {

        public ActionResult Index() {
            List<Cart> carts = new Cart().GetAll();
            ViewBag.carts = carts;
            return View();
        }

        public ActionResult Items(int id = 0) {
            try {
                Cart order = new Cart();
                order = order.GetByPayment(id);
                Customer customer = new Customer{ ID = order.cust_id };
                customer.Get();
                ViewBag.customer = customer;
                ViewBag.order = order;
            } catch (Exception) {
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Void(int id = 0) {
            Cart order = new Cart();
            order = order.GetByPayment(id);
            order.Void();
            return RedirectToAction("Items", new { id = id });
        }

        public ActionResult Add() {
            List<Customer> customers = new Customer().GetAll().OrderBy(x => x.lname).ToList<Customer>();
            ViewBag.customers = customers;

            List<Country> countries = UDF.GetCountries();

            Customer cust = new Customer();
            Address billing = new Address();
            Address shipping = new Address();
            try {
                cust = (Customer)TempData["customer"];
            } catch (Exception) { }
            try {
                billing = (Address)TempData["billing"];
            } catch (Exception) { }
            try {
                shipping = (Address)TempData["shipping"];
            } catch (Exception) { }
            ViewBag.error = (TempData["error"] != null) ? (string)TempData["error"] : "";
            ViewBag.cust = cust;
            ViewBag.billing = billing;
            ViewBag.shipping = shipping;
            ViewBag.countries = countries;
            return View();
        }

        public ActionResult Step1New() {
            Customer cust = new Customer();
            Address billing = new Address();
            Address shipping = new Address();
            try {
                #region Object Instantiation
                // Build out our Customer object
                cust = new Customer {
                    email = Request.Form["email"],
                    fname = Request.Form["fname"],
                    lname = Request.Form["lname"],
                    phone = Request.Form["phone"],
                    dateAdded = DateTime.Now,
                    isSuspended = 0,
                    isValidated = 1,
                    validator = Guid.NewGuid()
                };

                cust.GeneratePassword();

                // Build out our Billing object
                billing = new Address {
                    first = Request.Form["bfirst"],
                    last = Request.Form["blast"],
                    street1 = Request.Form["bstreet1"],
                    street2 = Request.Form["bstreet2"],
                    city = Request.Form["bcity"],
                    postal_code = Request.Form["bzip"],
                    residential = (Request.Form["bresidential"] == null) ? false : true,
                    active = true
                };
                
                // Build out our Shipping object
                shipping = new Address {
                    first = Request.Form["sfirst"],
                    last = Request.Form["slast"],
                    street1 = Request.Form["sstreet1"],
                    street2 = Request.Form["sstreet2"],
                    city = Request.Form["scity"],
                    postal_code = Request.Form["szip"],
                    residential = (Request.Form["sresidential"] == null) ? false : true,
                    active = true
                };
                #endregion
                
                #region Address state validation
                // Validate billing state
                try {
                    billing.state = Convert.ToInt32(Request.Form["bstate"]);
                } catch (Exception) {
                    throw new Exception("You must select a billing state/province.");
                }
                // Validate shipping state
                try {
                    shipping.state = Convert.ToInt32(Request.Form["sstate"]);
                } catch (Exception) {
                    throw new Exception("You must select a shipping state/province.");
                }
                #endregion

                string[] nullables = new string[] { "phone", "address", "address1", "issuspended", "receivenewsletter", "receiveoffers", "isvalidated", "billingid", "shippingid", "cart", "id", "orders" };
                UDF.Sanitize(cust,nullables);

                cust.Save();
                billing.Save(cust.ID);
                if(billing.Equals(shipping)) {
                    shipping = billing;
                } else {
                    shipping.Save(cust.ID);
                }
                cust.SaveAddresses(billing, shipping);
                cust.Address = billing;
                cust.Address1 = shipping;
                return RedirectToAction("Step2", new { id = cust.ID });
            } catch (Exception e) {
                TempData["customer"] = cust;
                TempData["billing"] = billing;
                TempData["shipping"] = shipping;
                TempData["error"] = e.Message;
                return RedirectToAction("Add");
            }

        }

        public ActionResult Step2(int id = 0) {
            Customer c = new Customer { 
                ID = id
            };
            c.Get();
            Cart currentCart = new Cart();
            try {
                currentCart = c.Carts.Where(x => x.payment_id == 0).First<Cart>();
            } catch {
                Cart cart = new Cart();
                currentCart = cart.Save(c.ID);
            }
            ViewBag.customer = c;
            ViewBag.cart = currentCart;
            return View("Add-Items");
        }

        public ActionResult Step3(int id = 0) {
            Customer c = new Customer {
                ID = id
            };
            c.Get();
            ViewBag.customer = c;

            string error = (TempData["message"] != null) ? (string)TempData["message"] : null;
            ViewBag.error = error;

            List<Address> addresses = c.GetAddresses();
            ViewBag.addresses = addresses;

            Cart currentCart = c.Carts.Where(x => x.payment_id == 0).First<Cart>();
            ViewBag.cart = currentCart;

            List<Country> countries = UDF.GetCountries();
            ViewBag.countries = countries;

            ShippingResponse shippingresponse = new ShippingResponse();
            shippingresponse = currentCart.getShipping();
            ViewBag.shippingresponse = shippingresponse;
            return View("Add-Shipping");
        }

        public ActionResult Step4(int id = 0, string shipping_type = "") {
            Customer c = new Customer {ID = id};
            c.Get();
            ViewBag.customer = c;

            string error = (TempData["message"] != null) ? (string)TempData["message"] : null;
            ViewBag.error = error;

            List<Address> addresses = c.GetAddresses();
            ViewBag.addresses = addresses;

            Cart currentCart = c.Carts.Where(x => x.payment_id == 0).First<Cart>();

            List<Country> countries = UDF.GetCountries();
            ViewBag.countries = countries;

            if (shipping_type != "") {
                decimal shipping_price = 0;
                string shiptype = "";
                string[] typesplit = shipping_type.Split('|');
                shiptype = typesplit[0];
                shipping_price = Convert.ToDecimal(typesplit[1]);
                currentCart.setShippingType(shiptype, shipping_price);
            }
            // We need to calculate the tax now that we know the shipping state
            ViewBag.cart = currentCart;

            return View("Add-Billing");
        }

        [RequireHttps]
        public ActionResult Step5(int id = 0, int address = 0) {
            Customer c = new Customer { ID = id };
            c.Get();
            ViewBag.customer = c;

            Cart currentCart = c.Carts.Where(x => x.payment_id == 0).First<Cart>();
            if (address != 0) {
                currentCart.SetBilling(address);
            }
            currentCart.SetTax();
            ViewBag.cart = currentCart;
            
            ViewBag.showShipping = true;
            ViewBag.message = TempData["message"];

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

            return View("Add-Payment");
        }

        [RequireHttps]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Authorize(int id = 0) {
            Customer c = new Customer { ID = id };
            c.Get();
            Cart currentCart = c.Carts.Where(x => x.payment_id == 0).First<Cart>();
            Settings settings = new Settings();

            decimal amount = currentCart.getTotal();
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
            request.AddCustomer(c.ID.ToString(), first, last, currentCart.Billing.street1 + ((currentCart.Billing.street2 != "") ? " " + currentCart.Billing.street2 : ""), currentCart.Billing.State1.abbr, currentCart.Billing.postal_code);

            //order number
            //request.AddInvoice("invoiceNumber");

            //Custom values that will be returned with the response
            //request.AddMerchantValue("merchantValue", "value");

            //Shipping Address
            request.AddShipping(c.ID.ToString(), currentCart.Shipping.first, currentCart.Shipping.last, currentCart.Shipping.street1 + ((currentCart.Shipping.street2 != "") ? " " + currentCart.Shipping.street2 : ""), currentCart.Shipping.State1.abbr, currentCart.Shipping.postal_code);


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
                currentCart.AddPayment("credit card", response.AuthorizationCode,"Complete");
                currentCart.SendConfirmation();
                int cartid = currentCart.ID;
                return RedirectToAction("Step6", new { id = cartid });
            } else {
                TempData["message"] = response.Message;
                return RedirectToAction("Step5", new { id = c.ID });
            }
        }

        [RequireHttps]
        public ActionResult Step6(int id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            ViewBag.orderID = db.Carts.Where(x => x.ID.Equals(id)).Select(x => x.payment_id).First();
            EDI edi = new EDI();
            edi.CreatePurchaseOrder(id);
            return View("Confirmation");
        }
        [NoValidation]
        public void ProcessNotifications() {
            #region
            /*string id, key, query;
            id = System.Configuration.ConfigurationManager.AppSettings["GoogleMerchantId"];
            key = System.Configuration.ConfigurationManager.AppSettings["GoogleMerchantKey"];
            query = String.Format("{0}:{1}",id,key);
            byte[] auth_bytes = ASCIIEncoding.ASCII.GetBytes(query);
            string auth = Convert.ToBase64String(auth_bytes);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://sandbox.google.com/checkout/api/checkout/v2/reports/Merchant/" + id);
            req.Accept = "application/xml; charset=UTF-8";
            req.ContentType = "application/xml; charset=UTF-8";
            req.Headers.Add("Authorization", "Basic " + auth);
            req.Method = "GET";

            WebResponse resp = req.GetResponse();
            Stream dataStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            Response.Write(reader.ReadToEnd());*/
            #endregion

            //NotificationHistoryRequest req = new NotificationHistoryRequest(new List<string>());
        }

        [NoValidation]
        public ActionResult ChooseShipping(int id, int address) {
            Customer customer = new Customer { ID = id };
            customer.Get();
            Cart currentCart = customer.Carts.Where(x => x.payment_id == 0).First<Cart>();
            currentCart.SetShipping(address);
            return RedirectToAction("Step3", new { id = id });
        }

        public ActionResult AddShippingAddress() {
            int ID = Convert.ToInt32(Request.Form["customerID"]);
            try {
                Customer customer = new Customer { ID = ID };
                customer.Get();
                Cart currentCart = customer.Carts.Where(x => x.payment_id == 0).First<Cart>();

                Address shipping = new Address();
                // Build out our Billing object
                shipping = new Address {
                    first = Request.Form["sfirst"],
                    last = Request.Form["slast"],
                    street1 = Request.Form["sstreet1"],
                    street2 = Request.Form["sstreet2"],
                    city = Request.Form["scity"],
                    postal_code = Request.Form["szip"],
                    residential = (Request.Form["sresidential"] == null) ? false : true,
                    active = true
                };
                try {
                    shipping.state = Convert.ToInt32(Request.Form["sstate"]);
                } catch (Exception) {
                    throw new Exception("You must select a shipping state/province.");
                }
                //shipping.GeoLocate();
                shipping.Save(customer.ID);

                // Retrieve Customer from Sessions/Cookie
                currentCart.SetShipping(shipping.ID);
            } catch (Exception e) {
                TempData["error"] = e.Message;
            }

            return RedirectToAction("Step3", new { id = ID });
        }

        public ActionResult AddBillingAddress() {
            int ID = Convert.ToInt32(Request.Form["customerID"]);
            try {
                Customer customer = new Customer { ID = ID };
                customer.Get();
                Cart currentCart = customer.Carts.Where(x => x.payment_id == 0).First<Cart>();

                Address billing = new Address();
                // Build out our Billing object
                billing = new Address {
                    first = Request.Form["bfirst"],
                    last = Request.Form["blast"],
                    street1 = Request.Form["bstreet1"],
                    street2 = Request.Form["bstreet2"],
                    city = Request.Form["bcity"],
                    postal_code = Request.Form["bzip"],
                    residential = (Request.Form["bresidential"] == null) ? false : true,
                    active = true
                };
                try {
                    billing.state = Convert.ToInt32(Request.Form["bstate"]);
                } catch (Exception) {
                    throw new Exception("You must select a billing state/province.");
                }
                //shipping.GeoLocate();
                billing.Save(customer.ID);

                // Retrieve Customer from Sessions/Cookie
                currentCart.SetBilling(billing.ID);
            } catch (Exception e) {
                TempData["error"] = e.Message;
            }

            return RedirectToAction("Step4", new { id = ID });
        }
        
        public void EmptyCart(int cartID = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Cart c = db.Carts.Where(x => x.ID == cartID).First<Cart>();
            c.Empty();
        }

        [NoValidation]
        public string AddPartToCart(int cartID = 0, int partID = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Cart c = db.Carts.Where(x => x.ID == cartID).First<Cart>();
            CartItem ci = c.Add(partID);
            return Newtonsoft.Json.JsonConvert.SerializeObject(ci);
        }

        public void RemovePartFromCart(int cartID = 0, int partID = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Cart c = db.Carts.Where(x => x.ID == cartID).First<Cart>();
            c.Remove(partID);
        }

    }
}
