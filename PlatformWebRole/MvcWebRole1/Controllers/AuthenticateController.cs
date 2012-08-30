using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace EcommercePlatform.Controllers {
    public class AuthenticateController : BaseController {

        /// <summary>
        /// Allow a Customer to login to their account
        /// </summary>
        /// <param name="error">Error message from login</param>
        /// <returns>View page</returns>
        public ActionResult Index(string referrer = "") {

            if(referrer == "" && Request.UrlReferrer != null && Request.UrlReferrer.Host != null && Request.UrlReferrer.Host.Contains(Request.Url.Host)){
                referrer = Request.UrlReferrer.AbsoluteUri;
            }

            List<Country> countries = UDF.GetCountries();

            Customer cust = new Customer();
            /*Address billing = new Address();
            Address shipping = new Address();*/
            try {
                cust = (Customer)TempData["customer"];
            } catch (Exception) { }
            /*try {
                billing = (Address)TempData["billing"];
            } catch (Exception) { }
            try {
                shipping = (Address)TempData["shipping"];
            } catch (Exception) { }*/

            ViewBag.cust = cust;
            /*ViewBag.billing = billing;
            ViewBag.shipping = shipping;*/
            ViewBag.countries = countries;
            ViewBag.error = TempData["error"];
            ViewBag.referrer = referrer;
            return View();
        }

        [RequireHttps]
        public ActionResult Login(string email = "", string password = "", int remember = 0, string redirect = "") {
            try {
                /**
                 * Store any Customer object from Session/Cookie into a tmp object
                 * We'll remove the cart from the tmp object and add it to our new Authenticated Customer
                 */
                Customer tmp = new Customer();
                tmp.GetFromStorage();
                Cart tmp_cart = tmp.Cart;


                string enc_password = UDF.EncryptString(password);

                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                Customer cust = new Customer {
                    email = email,
                    password = enc_password
                };
                cust.Login();
                cust.password = "Ya'll suckas got ketchup'd!";

                if (tmp_cart.CartItems.Count == 0) {
                    try {
                        Cart cust_cart = tmp_cart;
                        cust_cart = db.Carts.Where(x => x.cust_id == cust.ID).Where(x => x.payment_id == 0).OrderByDescending(x => x.last_updated).First<Cart>();
                        tmp_cart.RemoveCart();
                        tmp.Cart = cust_cart;
                    } catch {
                        tmp_cart.UpdateCart(cust.ID);
                    }
                } else {
                    tmp_cart.UpdateCart(cust.ID);
                }
                HttpCookie cook = new HttpCookie("hdcart", tmp.Cart.ID.ToString());
                if (remember != 0) {
                    cook.Expires = DateTime.Now.AddDays(30);
                }
                Response.Cookies.Add(cook);

                if(redirect != null && redirect.Length != 0 && !redirect.ToUpper().Contains("AUTHENTICATE")){
                    return Redirect(redirect);
                }else{
                    return Redirect("/Account");
                }
            } catch (Exception e) {
                TempData["error"] = e.Message;
                return Redirect("/Authenticate/Index");
            }
        }

        public ActionResult Logout() {
            try {

                Session.Clear();
                Session.Abandon();

                if (Request.Cookies["hdcart"] != null) {
                    var c = new HttpCookie("hdcart");
                    c.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(c);
                }
            } catch (Exception) { }
            return RedirectToAction("Index", "Authenticate");
        }

        [RequireHttps]
        public ActionResult Signup() {
            Customer cust = new Customer();
            Settings settings = ViewBag.settings;
            /*Address billing = new Address();
            Address shipping = new Address();*/
            bool loginAfterRegistration = false;
            if (settings.Get("CustomerLoginAfterRegistration") == "true") {
                loginAfterRegistration = true;
            }
            try {
                #region Object Instantiation
                string referrer = Request.Form["redirect"];
                // Build out our Customer object
                cust = new Customer {
                    email = Request.Form["email"],
                    fname = Request.Form["fname"],
                    lname = Request.Form["lname"],
                    phone = Request.Form["phone"],
                    dateAdded = DateTime.Now,
                    isSuspended = 0,
                    isValidated = 0,
                    validator = Guid.NewGuid()
                };

                // Build out our Billing object
                /*billing = new Address {
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
                };*/
                #endregion

                cust.ValidatePasswords(Request.Form["password"], Request.Form["password2"]);
                cust.ValidateEmail(Request.Form["email"], Request.Form["email2"]);

                #region Offers/Newsletter
                int receiveOffers = 0;
                int receiveNewsletter = 0;
                if (Request.Form["receiveOffers"] != null) {
                    try {
                        receiveOffers = Convert.ToInt32(Request.Form["receiveOffers"]);
                    } catch (Exception) { }
                }
                if (Request.Form["receiveNewsletter"] != null) {
                    try {
                        receiveNewsletter = Convert.ToInt32(Request.Form["receiveNewsletter"]);
                    } catch (Exception) { }
                }

                cust.receiveNewsletter = receiveNewsletter;
                cust.receiveOffers = receiveOffers;
                #endregion

                #region Address state validation
                // Validate billing state
                /*try {
                    billing.state = Convert.ToInt32(Request.Form["bstate"]);
                } catch (Exception) {
                    throw new Exception("You must select a billing state/province.");
                }
                // Validate shipping state
                try {
                    shipping.state = Convert.ToInt32(Request.Form["sstate"]);
                } catch (Exception) {
                    throw new Exception("You must select a shipping state/province.");
                }*/
                #endregion

                string[] nullables = new string[] { "phone", "issuspended", "receivenewsletter", "receiveoffers", "isvalidated", "billingid", "shippingid", "Address", "Address1" , "cart", "id", "orders" };
                UDF.Sanitize(cust,nullables);

                cust.Save();
                /*billing.Save(cust.ID);
                if(billing.Equals(shipping)) {
                    shipping = billing;
                } else {
                    shipping.Save(cust.ID);
                }
                cust.SaveAddresses(billing, shipping);
                cust.Address = billing;
                cust.Address1 = shipping;*/

                if (loginAfterRegistration) {
                    return RedirectToAction("login", new { email = cust.email, password = Request.Form["password"], remember = 0, redirect = referrer });
                } else {
                    TempData["error"] = "You're account has been successfully created. Please check your e-mail to confirm your account.";
                    return Redirect("/Authenticate");
                }
            } catch (Exception e) {
                if (e.Message.ToLower().Contains("a potentially dangerous")) {
                    throw new HttpException(403, "Forbidden");
                }
                TempData["customer"] = cust;
                /*TempData["billing"] = billing;
                TempData["shipping"] = shipping;*/
                TempData["error"] = e.Message;
                return Redirect(String.Format("/Authenticate/Index?#{0}","signup"));
            }
        }

        public ActionResult SendValidation(int id = 0) {
            try {
                Customer cust = new Customer { ID = id };
                cust.Get();
                cust.SendValidation();

                TempData["error"] = "An e-mail has been sent to you with a validation link.";
            } catch (Exception) {
                TempData["error"] = "We're sorry, but we failed to send your validation e-mail.";
            }
            return Redirect("/Authenticate");
        }

        public ActionResult Validate(int id, Guid validator) {
            try {
                if (id == 0 || validator == null) {
                    return Redirect("/Authenticate/Index#signup");
                } else {
                    Customer cust = new Customer {
                        ID = id,
                        validator = validator
                    };
                    cust.ValidateCreation();
                    TempData["error"] = "You're account has been validated. You may now log in to your account.";
                    return Redirect("/Authenticate/Index#login");
                }
            } catch (Exception) {
                TempData["error"] = "We failed to validate your account.";
                return Redirect("/Authenticate/Index#signup");
            }

        }

        public ActionResult Forgot() {
            ViewBag.error = TempData["error"];
            return View();
        }

        public ActionResult ForgotPassword(string email = "") {
            try {
                Customer cust = new Customer();
                cust.GetCustomerByEmail(email);

                cust.ResetPassword();

                TempData["error"] = String.Format("An e-mail has been sent to {0} with information on retrieving your account.", email);
                return RedirectToAction("Index","Authenticate"); 
            } catch (Exception) {
                TempData["error"] = "We're sorry we were unable to reset the password on your account.";
                return RedirectToAction("Forgot","Authenticate");
            }
        }
    }
}
