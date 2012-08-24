using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using Newtonsoft.Json;

namespace EcommercePlatform.Controllers {
    public class CartController : BaseController {

        public ActionResult Index() {
            // Create Customer
            Customer customer = new Customer();

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();

            // Create Cart object from customer
            Cart cart = customer.Cart;

            // Get the parts from this Cart
            List<CartItem> parts = cart.GetParts();
            cart.CartItems.Clear();
            cart.CartItems.AddRange(parts);
            ViewBag.cart = cart;
            return View();
        }

        public void CreatePO(int id = 0) {
            EDI edi = new EDI();
            edi.CreatePurchaseOrder(id);
        }

        [RequireHttps]
        public ActionResult Checkout() {
            return RedirectToAction("Billing");
        }

        [RequireHttps]
        public ActionResult Billing() {
            // Create Customer
            Customer customer = ViewBag.customer;

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();

            if (!customer.LoggedIn()) {
                return RedirectToAction("Index", "Authenticate", new { referrer = "https://" + Request.Url.Host + "/Cart/Checkout" });
            }

            customer.BindAddresses();

            List<Address> addresses = customer.GetAddresses();
            ViewBag.addresses = addresses;

            List<Country> countries = UDF.GetCountries();
            ViewBag.countries = countries;

            return View();
        }

        [RequireHttps]
        public ActionResult Shipping() {

            Customer customer = ViewBag.customer;

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();

            if (!customer.LoggedIn()) {
                return RedirectToAction("Index", "Authenticate", new { referrer = "https://" + Request.Url.Host + "/Cart/Checkout" });
            }

            customer.BindAddresses();

            List<Address> addresses = customer.GetAddresses();
            ViewBag.addresses = addresses;

            List<Country> countries = UDF.GetCountries();
            ViewBag.countries = countries;

            if (customer.Cart.ship_to == 0 && customer.Cart.bill_to != 0) {
                RedirectToAction("ChooseShipping", new {id = customer.Cart.bill_to});
            }

            List<string> shipping_types = CURTAPI.GetShippingTypes();
            ViewBag.shipping_types = shipping_types;

            ShippingResponse shippingresponse = new ShippingResponse();
            if (customer.Cart.ship_to != 0) {
                if (TempData["shipping_response"] != null) {
                    try {
                        shippingresponse = (ShippingResponse)TempData["shipping_response"];
                    } catch (Exception) { }
                }
                if (shippingresponse == null || shippingresponse.Result == null) {
                    string type = "";
                    if (customer.Cart.shipping_type != null) {
                        type = customer.Cart.shipping_type;
                    }
                    shippingresponse = getShipping();
                }
            }
            ViewBag.shippingresponse = shippingresponse;
            return View();
        }

        public ActionResult Add(int id = 0, int qty = 1) {
            // Create Customer
            Customer customer = new Customer();

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();

            // Add the item to the cart
            customer.Cart.Add(id,qty);

            // Serialize the Customer back to where it came from
            return RedirectToAction("Index");
        }

        public string AddAjax(int id = 0, int qty = 0) {
            // Create Customer
            Customer customer = new Customer();

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();
            customer.Cart.Add(id, qty);

            return getCart();
        }

        public ActionResult Update(int id = 0, int qty = 1) {
            // Create Customer
            Customer customer = new Customer();

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();

            customer.Cart.Update(id, qty);
            return RedirectToAction("Index");
        }

        public ActionResult Remove(int id = 0) {
            // Create Customer
            Customer customer = new Customer();

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();

            customer.Cart.Remove(id);
            return RedirectToAction("Index");
        }

        public string RemoveAjax(int id = 0) {
            // Create Customer
            Customer customer = new Customer();

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();

            customer.Cart.Remove(id);

            return Response.Cookies["cart"].Value;
        }

        public string getCart() {
            Customer customer = new Customer();

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();
            Cart cart = customer.Cart;
            Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            Newtonsoft.Json.Formatting format = Newtonsoft.Json.Formatting.None;

            return Newtonsoft.Json.JsonConvert.SerializeObject(cart,format,settings);
        }

        //[RequireHttps]
        public ActionResult ChooseBilling(int id = 0) {
            // Create Customer
            Customer customer = new Customer();

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();
            if (!customer.LoggedIn()) {
                return RedirectToAction("Index", "Authenticate", new { referrer = "https://" + Request.Url.Host + "/Cart/Checkout" });
            }


            if (customer.billingID == 0) {
                customer.SetBillingDefaultAddress(id);
            }
            customer.Cart.SetBilling(id);

            return RedirectToAction("billing");
        }
        
        //[RequireHttps]
        public ActionResult AddBillingAddress() {
            try {
                // Create Customer
                Customer customer = new Customer();
                customer.GetFromStorage();
                if (!customer.LoggedIn()) {
                    return RedirectToAction("Index", "Authenticate", new { referrer = "https://" + Request.Url.Host + "/Cart/Checkout" });
                }

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
                billing.Save(customer.ID);
                if (customer.billingID == 0) {
                    customer.SetBillingDefaultAddress(billing.ID);
                }
                if (customer.shippingID == 0) {
                    customer.SetShippingDefaultAddress(billing.ID);
                }

                // Retrieve Customer from Sessions/Cookie

                customer.Cart.SetBilling(billing.ID);
                if (customer.Cart.ship_to == 0) {
                    customer.Cart.SetShipping(billing.ID);
                }
            } catch { }

            return RedirectToAction("shipping");
        }

        //[RequireHttps]
        public ActionResult ChooseShipping(int id = 0) {
            // Create Customer
            Customer customer = new Customer();

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();
            if (!customer.LoggedIn()) {
                return RedirectToAction("Index", "Authenticate", new { referrer = "https://" + Request.Url.Host + "/Cart/Checkout" });
            }

            if (customer.shippingID == 0) {
                customer.SetShippingDefaultAddress(id);
            }
            customer.Cart.SetShipping(id);

            return RedirectToAction("Shipping");
        }

        //[RequireHttps]
        public ActionResult AddShippingAddress() {
            try {
                // Create Customer
                Customer customer = new Customer();
                customer.GetFromStorage();
                if (!customer.LoggedIn()) {
                    return RedirectToAction("Index", "Authenticate", new { referrer = "https://" + Request.Url.Host + "/Cart/Checkout" });
                }


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
                customer.Cart.SetShipping(shipping.ID);
            } catch(Exception e) {
                TempData["error"] = e.Message;
            }

            return RedirectToAction("shipping");
        }

        public ActionResult UpgradeShipping(string type = ""){
            ShippingResponse resp = getShipping();
            if (resp.Status_Description == "OK") {
                ShipmentRateDetails details = resp.Result.FirstOrDefault<ShipmentRateDetails>();
                RateDetail rate = details.Rates.FirstOrDefault<RateDetail>();

                Customer customer = new Customer();

                // Retrieve Customer from Sessions/Cookie
                customer.GetFromStorage();
                if (!customer.LoggedIn()) {
                    return RedirectToAction("Index", "Authenticate", new { referrer = "https://" + Request.Url.Host + "/Cart/Checkout" });
                }

                decimal shipping_price = Convert.ToDecimal(rate.NetCharge.Key);
                string shipping_type = details.ServiceType;
                customer.Cart.setShippingType(shipping_type, shipping_price);
            }
            TempData["shipping_response"] = resp;
            return RedirectToAction("shipping");
        }

        public ShippingResponse getShipping() {
            Customer customer = new Customer();
            Settings settings = new Settings();
            customer.GetFromStorage();
            if (!customer.LoggedIn()) {
                Response.Redirect("/Authenticate");
            }

            FedExAuthentication auth = new FedExAuthentication {
                AccountNumber = Convert.ToInt32(settings.Get("FedExAccount")),
                Key = settings.Get("FedExKey"),
                Password = settings.Get("FedExPassword"),
                CustomerTransactionId = "",
                MeterNumber = Convert.ToInt32(settings.Get("FedExMeter"))
            };

            customer.Cart.BindAddresses();

            ShippingAddress destination = new ShippingAddress();
            try {
                destination = customer.Cart.Shipping.getShipping();
            } catch (Exception) {
                Response.Redirect("/Authenticate");
            }
            DistributionCenter d = new DistributionCenter().GetNearest(customer.Cart.Shipping.GeoLocate());
            ShippingAddress origin = d.getAddress().getShipping();
            List<int> parts = new List<int>();
            foreach (CartItem item in customer.Cart.CartItems) {
                for (int i = 1; i <= item.quantity; i++) {
                    parts.Add(item.partID);
                }
            }

            ShippingResponse response = CURTAPI.GetShipping(auth, origin, destination, parts);

            return response;
        }

        public ActionResult ChooseShippingType(string shipping_type = "") {
            Customer customer = new Customer();

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage();
            if (!customer.LoggedIn()) {
                return RedirectToAction("Index", "Authenticate", new { referrer = "https://" + Request.Url.Host + "/Cart/Checkout" });
            }

            decimal shipping_price = 0;
            string shiptype = "";
            try {
                string[] typesplit = shipping_type.Split('|');
                shiptype = typesplit[0];
                shipping_price = Convert.ToDecimal(typesplit[1]);
                customer.Cart.setShippingType(shiptype, shipping_price);

                // We need to calculate the tax now that we know the shipping state
                customer.Cart.SetTax();

                if (customer.Cart.Validate()) {
                    return RedirectToAction("Index", "Payment");
                } else if (customer.Cart.bill_to == 0) {
                    return RedirectToAction("Billing");
                } else if (customer.Cart.ship_to == 0) {
                    return RedirectToAction("Shipping");
                } else {
                    return RedirectToAction("Index");
                }
            } catch {
                return RedirectToAction("Cart", "Checkout");
            }
        }

    }
}
