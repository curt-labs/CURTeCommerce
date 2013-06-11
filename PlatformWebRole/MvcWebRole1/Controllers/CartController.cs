using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using Newtonsoft.Json;

namespace EcommercePlatform.Controllers {
    public class CartController : BaseController {

        public async Task<ActionResult> Index() {
            HttpContext ctx = System.Web.HttpContext.Current;
            var pcats = CURTAPI.GetParentCategoriesAsync();
            await Task.WhenAll(new Task[] { pcats });
            ViewBag.parent_cats = await pcats;

            // Create Customer
            Customer customer = new Customer();

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage(ctx);

            // Create Cart object from customer
            Cart cart = customer.Cart;

            // Get the api response from the parts in this Cart
            cart.GetParts();
            ViewBag.cart = cart;
            return View();
        }

        [RequireHttps]
        public async Task<ActionResult> Checkout() {
            HttpContext ctx = System.Web.HttpContext.Current;

            var pcats = CURTAPI.GetParentCategoriesAsync();
            await Task.WhenAll(new Task[] { pcats });
            ViewBag.parent_cats = await pcats;

            // start checkout process here
            // one page checkout form followed by shipping type
            Customer customer = ViewBag.customer;
            customer.GetFromStorage(ctx);
            if (customer.Cart.payment_id != 0) {
                // cart gets expired
                UDF.ExpireCart(ctx,customer.ID);
                return RedirectToAction("Index");
            }
            bool same = true;

            Address billing = new Address();
            Address shipping = new Address();

            if (customer.LoggedIn(ctx)) {
                customer.BindAddresses();
                billing = (customer.billingID != 0) ? customer.Address : new Address();
                shipping = (customer.shippingID != 0) ? customer.Address1 : new Address();
                if (customer.billingID != customer.shippingID)
                    same = false;
            }
            try {
                if (TempData["customer"] != null && TempData["billing"] != null && TempData["shipping"] != null) {
                    customer = (Customer)TempData["customer"];
                    billing = (Address)TempData["billing"];
                    shipping = (Address)TempData["shipping"];
                    same = (bool)TempData["same"];
                }
            } catch (Exception) { }

            ViewBag.billing = billing;
            ViewBag.shipping = shipping;
            List<Country> countries = UDF.GetCountries();
            ViewBag.countries = countries;
            ViewBag.same = same;
            ViewBag.error = TempData["error"];

            return View();
        }

        [RequireHttps]
        public ActionResult Proceed() {
            HttpContext ctx = System.Web.HttpContext.Current;
            Customer cust = ViewBag.customer;
            cust.GetFromStorage(ctx);
            Cart cart = cust.Cart;

            Settings settings = ViewBag.settings;
            Address billing = new Address();
            Address shipping = new Address();
            bool sameAsBilling = (Request.Form["same"] != null) ? true : false;
            string email = Request.Form["email"] ?? "";
            try {
                #region Get Or Create Customer
                // Build out our Customer object
                cust = cust.GetCustomerByEmail(email);
                if (cust == null || cust.ID == 0) {
                    cust = new Customer {
                        email = Request.Form["email"],
                        fname = Request.Form["fname"],
                        lname = Request.Form["lname"],
                        phone = Request.Form["phone"],
                        password = UDF.EncryptString(new PasswordGenerator().Generate()),
                        dateAdded = DateTime.UtcNow,
                        isSuspended = 0,
                        isValidated = 0,
                        receiveNewsletter = (Request.Form["receiveNewsletter"] != null) ? 1 : 0,
                        receiveOffers = (Request.Form["receiveOffers"] != null) ? 1 : 0,
                        validator = Guid.NewGuid()
                    };
                    cust.ValidateEmail(Request.Form["email"], Request.Form["email"]);
                    string[] nullables = new string[] { "phone", "issuspended", "receivenewsletter", "receiveoffers", "isvalidated", "billingid", "shippingid", "Address", "Address1", "cart", "id", "orders" };
                    UDF.Sanitize(cust, nullables);
                    cust.Save();
                }
                cart.UpdateCart(ctx, cust.ID);
                #endregion

                #region Address Initialization
                // Build out our Billing object
                billing = new Address {
                    first = Request.Form["bfirst"],
                    last = Request.Form["blast"],
                    street1 = Request.Form["bstreet1"],
                    street2 = Request.Form["bstreet2"],
                    city = Request.Form["bcity"],
                    postal_code = Request.Form["bzip"],
                    residential = (Request.Form["bresidential"] == null) ? false : true,
                    cust_id = cust.ID,
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
                    cust_id = cust.ID,
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
                if (!sameAsBilling || !billing.Equals(shipping)) {
                    try {
                        shipping.state = Convert.ToInt32(Request.Form["sstate"]);
                    } catch (Exception) {
                        throw new Exception("You must select a shipping state/province.");
                    }
                }
                #endregion

                #region Get Or Create Address in Database
                billing.cust_id = cust.ID;
                shipping.cust_id = cust.ID;
                billing.MatchOrSave();
                if (sameAsBilling || billing.Equals(shipping)) {
                    shipping = billing;
                } else {
                    shipping.MatchOrSave();
                }
                if (cust.billingID == 0 || cust.shippingID == 0) {
                    cust.SaveAddresses(billing, shipping);
                }
                #endregion
                cart.SetBilling(billing.ID);
                cart.SetShipping(shipping.ID);
                cart.BindAddresses();
                if (cart.Shipping.isPOBox()) {
                    throw new Exception("Your Shipping address cannot be a PO Box.");
                }

                return RedirectToAction("Shipping");
            } catch (Exception e) {
                if (e.Message.ToLower().Contains("a potentially dangerous")) {
                    throw new HttpException(403, "Forbidden");
                }
                TempData["customer"] = cust;
                TempData["billing"] = billing;
                TempData["shipping"] = shipping;
                TempData["same"] = sameAsBilling;
                TempData["error"] = e.Message + ' ' + e.StackTrace;
                return RedirectToAction("Checkout");
            }
        }

        [RequireHttps]
        public async Task<ActionResult> Billing() {
            var pcats = CURTAPI.GetParentCategoriesAsync();
            await Task.WhenAll(new Task[] { pcats });
            ViewBag.parent_cats = await pcats;

            HttpContext ctx = System.Web.HttpContext.Current;
            // Create Customer
            Customer customer = ViewBag.customer;

            // Get the contact ContentPage
            ContentPage page = ContentManagement.GetPageByTitle("billing");
            ViewBag.page = page;

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage(ctx);

            if (customer.Cart.payment_id == 0) {
                customer.BindAddresses();

                List<Address> addresses = customer.GetAddresses();
                ViewBag.addresses = addresses;
                List<Country> countries = UDF.GetCountries();
                ViewBag.countries = countries;

                return View();
            } else {
                UDF.ExpireCart(ctx, customer.ID);
                return RedirectToAction("Index");
            }
        }


        [RequireHttps]
        public async Task<ActionResult> Shipping(string error = "") {
            HttpContext ctx = System.Web.HttpContext.Current;
            Customer customer = ViewBag.customer;
            Settings settings = ViewBag.settings;
            ViewBag.error = error;

            // Retrieve Customer from Sessions/Cookie
            int shippingpad = 0;
            if (settings.Get("ShippingPadding") != "") {
                try {
                    shippingpad = Convert.ToInt32(settings.Get("ShippingPadding"));
                } catch { }
            }
            var pcats = CURTAPI.GetParentCategoriesAsync();
            await Task.WhenAll(new Task[] { pcats });
            ViewBag.parent_cats = await pcats;

            // Get the contact ContentPage
            ContentPage page = ContentManagement.GetPageByTitle("shipping");
            ViewBag.page = page;

            if (customer.Cart.payment_id == 0) {
                customer.BindAddresses();

                ShippingResponse shippingresponse = new ShippingResponse();
                if (customer.Cart.ship_to != 0 && !customer.Cart.Shipping.isPOBox()) {
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
                        shippingresponse = getShipping(ctx);
                    }
                }
                ViewBag.shippingpadding = shippingpad;
                ViewBag.shippingresponse = shippingresponse;
                return View();
            } else {
                UDF.ExpireCart(ctx,customer.ID);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Add(int id = 0, int qty = 1) {
            // Create Customer
            Customer customer = new Customer();
            HttpContext ctx = System.Web.HttpContext.Current;

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage(ctx);

            // Add the item to the cart
            if (customer.Cart.payment_id == 0) {
                customer.Cart.Add(ctx,id, qty);

                // Serialize the Customer back to where it came from
                return RedirectToAction("Index");
            } else {
                UDF.ExpireCart(ctx,customer.ID);
                return RedirectToAction("Index");
            }
        }

        public string AddAjax(int id = 0, int qty = 0) {
            HttpContext ctx = System.Web.HttpContext.Current;
            // Create Customer
            Customer customer = new Customer();

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage(ctx);
            if (customer.Cart.payment_id == 0) {
                customer.Cart.Add(ctx,id, qty);
            } else {
                UDF.ExpireCart(ctx,customer.ID);
            }

            return getCart();
        }

        public ActionResult Update(int id = 0, int qty = 1) {
            try {
                // Create Customer
                Customer customer = new Customer();
                HttpContext ctx = System.Web.HttpContext.Current;

                // Retrieve Customer from Sessions/Cookie
                customer.GetFromStorage(ctx);

                if (customer.Cart.payment_id == 0) {
                    customer.Cart.Update(ctx, id, qty);
                } else {
                    UDF.ExpireCart(ctx, customer.ID);
                }
            } catch (Exception e) {
                if (e.Message.ToLower().Contains("a potentially dangerous")) {
                    throw new HttpException(403, "Forbidden");
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Remove(int id = 0) {
            // Create Customer
            Customer customer = new Customer();
            HttpContext ctx = System.Web.HttpContext.Current;

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage(ctx);

            if (customer.Cart.payment_id == 0) {
                customer.Cart.Remove(ctx, id);
            } else {
                UDF.ExpireCart(ctx, customer.ID);
            }
            return RedirectToAction("Index");
        }

        public string RemoveAjax(int id = 0) {
            // Create Customer
            Customer customer = new Customer();
            HttpContext ctx = System.Web.HttpContext.Current;

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage(ctx);

            if (customer.Cart.payment_id == 0) {
                customer.Cart.Remove(ctx, id);
            } else {
                UDF.ExpireCart(ctx, customer.ID);
            }

            return Response.Cookies["cart"].Value;
        }

        public string getCart() {
            Customer customer = new Customer();
            HttpContext ctx = System.Web.HttpContext.Current;

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage(ctx);
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
            HttpContext ctx = System.Web.HttpContext.Current;

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage(ctx);

            if (customer.Cart.payment_id == 0) {
                if (customer.billingID == 0) {
                    customer.SetBillingDefaultAddress(id);
                }
                customer.Cart.SetBilling(id);

                return RedirectToAction("billing");
            } else {
                UDF.ExpireCart(ctx, customer.ID);
                return RedirectToAction("index");
            }
        }
        
        //[RequireHttps]
        public ActionResult AddBillingAddress() {
            try {
                // Create Customer
                Customer customer = new Customer();
                HttpContext ctx = System.Web.HttpContext.Current;
                customer.GetFromStorage(ctx);

                if (customer.Cart.payment_id == 0) {
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
                    if (customer.shippingID == 0 && !billing.isPOBox()) {
                        customer.SetShippingDefaultAddress(billing.ID);
                    }

                    // Retrieve Customer from Sessions/Cookie

                    customer.Cart.SetBilling(billing.ID);
                    if (customer.Cart.ship_to == 0 && !billing.isPOBox()) {
                        customer.Cart.SetShipping(billing.ID);
                    }
                } else {
                    UDF.ExpireCart(ctx, customer.ID);
                    return RedirectToAction("index");
                }
            } catch { }

            return RedirectToAction("shipping");
        }

        //[RequireHttps]
        public ActionResult ChooseShipping(int id = 0) {
            // Create Customer
            Customer customer = new Customer();
            HttpContext ctx = System.Web.HttpContext.Current;

            // Retrieve Customer from Sessions/Cookie
            customer.GetFromStorage(ctx);
            if (customer.Cart.payment_id == 0) {
                if (customer.shippingID == 0) {
                    customer.SetShippingDefaultAddress(id);
                }
                customer.Cart.SetShipping(id);

                return RedirectToAction("Shipping");
            } else {
                UDF.ExpireCart(ctx, customer.ID);
                return RedirectToAction("index");
            }
        }

        //[RequireHttps]
        public ActionResult AddShippingAddress() {
            string error = "";
            try {
                // Create Customer
                Customer customer = new Customer();
                HttpContext ctx = System.Web.HttpContext.Current;
                customer.GetFromStorage(ctx);

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
                if (shipping.isPOBox()) {
                    throw new Exception("You cannot ship to a PO Box.");
                }
                //shipping.GeoLocate();
                shipping.Save(customer.ID);

                // Retrieve Customer from Sessions/Cookie
                customer.Cart.SetShipping(shipping.ID);
            } catch (Exception e) {
                error = e.Message;
            }

            return RedirectToAction("shipping", new { error = error });
        }

        public ActionResult UpgradeShipping(string type = ""){
            HttpContext ctx = System.Web.HttpContext.Current;
            ShippingResponse resp = getShipping(ctx);
            if (resp.Status_Description == "OK") {
                ShipmentRateDetails details = resp.Result.FirstOrDefault<ShipmentRateDetails>();
                RateDetail rate = details.Rates.FirstOrDefault<RateDetail>();

                Customer customer = new Customer();

                // Retrieve Customer from Sessions/Cookie
                customer.GetFromStorage(ctx);
                decimal shipping_price = Convert.ToDecimal(rate.NetCharge.Key);
                string shipping_type = details.ServiceType;
                customer.Cart.setShippingType(shipping_type, shipping_price);
            }
            TempData["shipping_response"] = resp;
            return RedirectToAction("shipping");
        }

        public ShippingResponse getShipping(HttpContext ctx) {
            Customer customer = new Customer();
            Settings settings = ViewBag.settings;
            customer.GetFromStorage(ctx);
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
                Response.Redirect("/Cart/Checkout");
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
            HttpContext ctx = System.Web.HttpContext.Current;
            customer.GetFromStorage(ctx);
            if (customer.Cart.payment_id == 0) {

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
                    } else if (customer.Cart.bill_to == 0 || customer.Cart.ship_to == 0) {
                        return RedirectToAction("Checkout");
                    } else {
                        return RedirectToAction("Index");
                    }
                } catch {
                    return RedirectToAction("Checkout", "Cart");
                }
            } else {
                UDF.ExpireCart(ctx, customer.ID);
                return RedirectToAction("Index");
            }
        }
    }
}
