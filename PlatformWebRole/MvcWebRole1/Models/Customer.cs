using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommercePlatform.Models;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.Linq;
using System.Diagnostics;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Serialization;

namespace EcommercePlatform {
    partial class Customer {

        public bool remember { get; set; }
        public List<Cart> Orders { get; set; }
        public Cart Cart { get; set; }

        internal void ValidatePasswords(string pass1, string pass2) {
            if (pass1 == null || pass1.Trim().Length == 0) {
                throw new Exception("Password is required.");
            } else {
                if (pass1.Trim().Length < 6) {
                    throw new Exception("Password must be at least 6 characters long.");
                }
                pass1 = pass1.Trim();
            }
            if (pass2 == null || pass2.Trim().Length == 0) {
                throw new Exception("You must enter confirmation password.");
            } else {
                pass2 = pass2.Trim();
            }
            if (pass1 != pass2) {
                throw new Exception("Passwords must match.");
            } else {
                this.password = UDF.EncryptString(pass1);
            }
        }

        internal void ValidateCurrentPassword(string pass) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Customer c = db.Customers.Where(x => x.ID.Equals(this.ID)).First<Customer>();
            if (c.password != UDF.EncryptString(pass)) {
                throw new Exception("The current password you entered was incorrect. Try Again");
            }
        }

        internal void ValidateEmail(string email1, string email2) {
            if (email1 == null || email1.Trim().Length == 0) {
                throw new Exception("E-Mail address is required.");
            } else {
                email1 = email1.Trim();
            }
            if (email2 == null || email2.Trim().Length == 0) {
                throw new Exception("You must enter confirmation E-Mail Address.");
            } else {
                email2 = email2.Trim();
            }
            if (email1 != email2) {
                throw new Exception("E-Mail addresses must match.");
            } else {
                this.email = email1;
            }
        }

        internal void Save() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Settings settings = new Settings();
            bool RequireCustomerActivation = true;
            if (settings.Get("RequireCustomerActivation") == "false") {
                RequireCustomerActivation = false;
            }

            // Make sure we don't have an account with this e-mail address
            Customer cust = this.GetCustomerByEmail();
            if (cust != null && cust.ID > 0) {
                throw new Exception("An account using the E-Mail address you provided already exists.");
            }

            // We are going to make an attempt at saving the Customer record

            Customer new_customer = new Customer {
                email = this.email,
                fname = this.fname,
                lname = this.lname,
                phone = this.phone,
                dateAdded = this.dateAdded,
                isSuspended = this.isSuspended,
                isValidated = this.isValidated,
                validator = this.validator,
                password = this.password,
                receiveNewsletter = this.receiveNewsletter,
                receiveOffers = this.receiveOffers,
            };

            if (!RequireCustomerActivation) {
                new_customer.isValidated = 1;
            }
            db.Customers.InsertOnSubmit(new_customer);
            db.SubmitChanges();
            this.ID = new_customer.ID;

            if (RequireCustomerActivation) {
                SendValidation();
            }
        }

        internal void Update(string email, string fname, string lname, string phone, int receiveOffers, int receiveNewsletter) {
            Customer tmp = new Customer();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            tmp = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Customer>();
            tmp.email = email;
            tmp.fname = fname;
            tmp.lname = lname;
            tmp.phone = phone;
            tmp.receiveNewsletter = receiveNewsletter;
            tmp.receiveOffers = receiveOffers;
            db.SubmitChanges();
        }

        internal void UpdatePassword() {
            Customer tmp = new Customer();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            tmp = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Customer>();
            tmp.password = this.password;
            db.SubmitChanges();
        }

        internal void SaveAddresses(Address billing, Address shipping) {
            
            this.billingID = billing.ID;
            this.shippingID = shipping.ID;

            Customer tmp = new Customer();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            tmp = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Customer>();
            tmp.billingID = this.billingID;
            tmp.shippingID = this.shippingID;
            db.SubmitChanges();
        }

        internal void ClearAddress(int id) {
            if (this.billingID == id || this.shippingID == id) {
                Customer tmp = new Customer();
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                tmp = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Customer>();
                if (tmp.billingID == id) {
                    tmp.billingID = 0;
                }
                if (tmp.shippingID == id) {
                    tmp.shippingID = 0;
                }
                db.SubmitChanges();
            }
        }
        internal List<Address> GetAddresses() {
            List<Address> addresses = new List<Address>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            addresses = db.Addresses.Where(x => x.cust_id.Equals(this.ID) && x.active).ToList<Address>();
            return addresses;
        }

        internal void UpdateAddress(int ID, string first,string last, string street1, string street2, string city, int state_id, string zip, bool residential) {
            Address tmp = new Address();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            tmp = db.Addresses.Where(x => x.ID.Equals(ID)).FirstOrDefault<Address>();
            tmp.Update(first, last, street1, street2, city, state_id, zip, residential);
        }

        internal void Login() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Customer cust = db.Customers.Where(x => x.email.Equals(this.email) && x.password.Equals(this.password)).FirstOrDefault<Customer>();
            if (cust == null || cust.ID == 0) {
                throw new Exception("Failed to log you into the system. Please try again.");
            }
            if (cust.isValidated == 0) {
                throw new Exception("We're sorry the account you're trying to access has not been validated. Please check your e-mail to validate your account.<br /><a href='/Authenticate/SendValidation?id=" + cust.ID + "' title='Resend E-Mail'>Click here</a> to resend validation e-mail.");
            } else if(cust.isSuspended == 1) {
                throw new Exception("We're sorry, but the account you're trying to access has been suspended.");
            }
            this._ID = cust.ID;
            this.email = cust.email;
            this.password = cust.password;
            this.fname = cust.fname;
            this.lname = cust.lname;
            this.phone = cust.phone;
            this.dateAdded = cust.dateAdded;
            this.isSuspended = cust.isSuspended;
            this.receiveNewsletter = cust.receiveNewsletter;
            this.receiveOffers = cust.receiveOffers;
            this.billingID = cust.billingID;
            this.shippingID = cust.shippingID;
            this.isValidated = cust.isValidated;
            this.validator = cust.validator;
            this.Address = cust.Address;
            this.Address1 = cust.Address1;
        }

        internal void ValidateCreation() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            if (this.ID == null || this.ID == 0 || this.validator == null) {
                throw new Exception("Invalid reference.");
            }
            Customer cust = db.Customers.Where(x => x.ID.Equals(this.ID) && x.validator.Equals(this.validator)).FirstOrDefault<Customer>();
            if (cust == null || cust.ID == 0) {
                throw new Exception("No Customer record found.");
            }
            cust.isValidated = 1;
            db.SubmitChanges();
        }

        internal Customer GetCustomerByEmail(string email = null) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            if (email == null) {
                return db.Customers.Where(x => x.email.Equals(this.email)).FirstOrDefault<Customer>();
            } else {
                return db.Customers.Where(x => x.email.Equals(email)).FirstOrDefault<Customer>();
            }
        }

        public static bool CheckCustomerEmail(string email = null) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            if (email == null) {
                return true;
            } else {
                bool exists = false;
                if(db.Customers.Where(x => x.email.Equals(email)).Count() > 0) {
                    exists = true;
                }
                return exists;
            }
        }

        internal void Get() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Customer cust = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Customer>();

            this.ID = cust.ID;
            this.email = cust.email;
            this.password = cust.password;
            this.fname = cust.fname;
            this.lname = cust.lname;
            this.phone = cust.phone;
            this.dateAdded = cust.dateAdded;
            this.isSuspended = cust.isSuspended;
            this.receiveNewsletter = cust.receiveNewsletter;
            this.receiveOffers = cust.receiveOffers;
            this.billingID = cust.billingID;
            this.shippingID = cust.shippingID;
            this.isValidated = cust.isValidated;
            this.validator = cust.validator;
            this.Address = cust.Address;
            this.Address1 = cust.Address1;
        }

        internal Customer Get(int id = 0) {
            try {
                Customer cust = new Customer();
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                cust = db.Customers.Where(x => x.ID.Equals(id)).FirstOrDefault<Customer>();
                return cust;
            } catch (Exception) {
                throw new Exception("Unable to retrieve Customer record.");
            }
        }

        internal void BindAddresses() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            if (this.billingID != 0) {
                this.Address = db.Addresses.Where(x => x.ID.Equals(this.billingID)).FirstOrDefault<Address>();
            }
            if (this.shippingID != 0) {
                this.Address1 = db.Addresses.Where(x => x.ID.Equals(this.shippingID)).FirstOrDefault<Address>();
            }

            this.Cart.BindAddresses();

        }

        internal void SetBillingDefaultAddress(int id) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Customer cust = new Customer();
            cust = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Customer>();
            cust.billingID = id;
            db.SubmitChanges();
            this.billingID = id;
        }

        internal void SetShippingDefaultAddress(int id) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Customer cust = new Customer();
            cust = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Customer>();
            cust.shippingID = id;
            db.SubmitChanges();
            this.shippingID = id;
        }

        internal void GetFromStorage() {

            HttpCookie cart_cookie = null;
            int cartID = 0;
            int custID = 0;
            Cart cart = new Cart();
            cart_cookie = HttpContext.Current.Request.Cookies.Get("hdcart");
            if (cart_cookie != null && cart_cookie.Value != null && cart_cookie.Value.Length > 0) {
                cartID = Convert.ToInt32(cart_cookie.Value);
            }
            if (cartID == 0) {
                cart = cart.Save();
                cartID = cart.ID;
                HttpCookie cook = new HttpCookie("hdcart", cartID.ToString());
                if (this.remember) {
                    cook.Expires = DateTime.Now.AddDays(30);
                }
                HttpContext.Current.Response.Cookies.Add(cook);
            } else {
                cart = new Cart().Get(cartID);
                custID = cart.cust_id;
            }

            Customer customer = new Customer();
            if (custID > 0) {
                try {
                    customer = customer.Get(custID);
                } catch { }
            }
            customer.Cart = cart;

            this.ID = customer.ID;
            this.email = customer.email;
            this.fname = customer.fname;
            this.lname = customer.lname;
            this.phone = customer.phone;
            this.dateAdded = customer.dateAdded;
            this.isSuspended = customer.isSuspended;
            this.receiveNewsletter = customer.receiveNewsletter;
            this.receiveOffers = customer.receiveOffers;
            this.billingID = customer.billingID;
            this.shippingID = customer.shippingID;
            this.validator = customer.validator;
            this.Cart = customer.Cart;
            this.remember = customer.remember;
        }

        internal void SerializeToStorage() {
            /*Customer serializable_customer = new Customer {
                ID = this.ID,
                email = this.email,
                fname = this.fname,
                lname = this.lname,
                phone = this.phone,
                dateAdded = this.dateAdded,
                isSuspended = this.isSuspended,
                receiveNewsletter = this.receiveNewsletter,
                receiveOffers = this.receiveOffers,
                billingID = this.billingID,
                shippingID = this.shippingID,
                validator = this.validator,
                remember = this.remember,
                Cart = this.Cart
            };
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string cust_json = JsonConvert.SerializeObject(serializable_customer, Formatting.None, settings);

            HttpCookie cook = new HttpCookie("customer", cust_json);
            if (this.remember) {
                cook.Expires = DateTime.Now.AddDays(30);
            }
            HttpContext.Current.Response.Cookies.Add(cook);

            //HttpContext.Current.Session.Add("customer", cust_json);*/
        }

        internal void SendValidation() {
            if (this.isValidated == 1) {
                throw new Exception("Accound is already validated.");
            }
            Settings settings = new Settings();
            string[] tos = { this.email };
            StringBuilder sb = new StringBuilder();

            sb.Append("<p>A new account with this e-mail address was created from <a href='" + settings.Get("SiteURL") + "' title='" + settings.Get("SiteName") + ">" + settings.Get("SiteName") + "</a>.</p>");
            sb.Append("<hr />");
            sb.AppendFormat("<p>To <a href='" + settings.Get("SiteURL") + "Authenticate/Validate?id={0}&validator={1}' title='Validate' target='_blank'>validate</a> your account for <a href='" + settings.Get("SiteURL") + "' title='" + settings.Get("SiteName") + "'>" + settings.Get("SiteName") + "</a>, please use the link provided below.</p>", this.ID, this.validator);
            sb.AppendFormat("<a href='" + settings.Get("SiteURL") + "Authenticate/Validate?id={0}&validator={1}'>" + settings.Get("SiteURL") + "Authenticate/Validate?id={0}&validator={1}</a><hr />", this.ID, this.validator);
            sb.Append("<hr /><br />");
            sb.Append("<p style='font-size:11px'>If you feel this was a mistake please disregard this e-mail.</p>");

            UDF.SendEmail(tos, "New Account for " + settings.Get("SiteName"), true, sb.ToString());
        }

        internal void ResetPassword() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            string orig_pass = this.password;
            try {
                PasswordGenerator pw = new PasswordGenerator();
                string new_pass = pw.Generate();
                Settings settings = new Settings();

                string[] tos = { this.email };
                StringBuilder sb = new StringBuilder();

                sb.Append("<p>A new password has been generated for an account with this e-mail address from <a href='" + settings.Get("SiteURL") + "' title='" + settings.Get("SiteName") + "'>" + settings.Get("SiteName") + "</a>.</p>");
                sb.Append("<hr />");
                sb.Append("<p>To <a href='" + settings.Get("SiteURL") + "' title='Login' target='_blank'>login</a> to " + settings.Get("SiteName") + ", please use the information provided below.</p>");
                sb.Append("<span>The new password is: <strong>" + new_pass + "</strong></span><br />");
                sb.Append("<hr /><br />");
                sb.Append("<p style='font-size:11px'>If you feel this was a mistake please disregard this e-mail.</p>");

                UDF.SendEmail(tos, "New password for " + settings.Get("SiteName"), true, sb.ToString());
                Customer c = db.Customers.Where(x => x.ID.Equals(this.ID)).First<Customer>();
                c.password = UDF.EncryptString(new_pass);
            } catch {}
            db.SubmitChanges();
        }

        internal bool LoggedIn() {
            this.GetFromStorage();
            if (this.ID > 0) {
                return true;
            }
            return false;
        }

        internal void BindOrders() {
            if (this._ID > 0) {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                this.Orders = db.Carts.Where(x => x.cust_id.Equals(this.ID) && x.payment_id > 0).ToList<Cart>();
            } else {
                this.Orders = new List<Cart>();
            }
        }

        internal Cart GetOrder(int id) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.Carts.Where(x => x.cust_id.Equals(this.ID) && x.ID.Equals(id)).FirstOrDefault<Cart>();
            } catch (Exception) {
                return new Cart();
            }
        }

        internal Cart GetOrderByPayment(int id) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.Carts.Where(x => x.cust_id.Equals(this.ID) && x.payment_id.Equals(id)).FirstOrDefault<Cart>();
            } catch (Exception) {
                return new Cart();
            }
        }
    }
}
