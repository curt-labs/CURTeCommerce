﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Admin.Models;
using System.Text;
using System.Transactions;

namespace Admin {
    partial class Customer {
        public string plainpassword { get; set; }

        internal List<AdminCustomer> GetAll() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<AdminCustomer> custs = (from c in db.Customers
                                         orderby c.dateAdded
                                         select new AdminCustomer {
                                             ID = c.ID,
                                             fname = c.fname,
                                             lname = c.lname,
                                             phone = c.phone,
                                             email = c.email,
                                             created = c.dateAdded,
                                             status = (c.isSuspended == 1) ? "Suspended" : "Active",
                                             ordercount = c.Carts.Where(x => x.payment_id > 0).Count()
                                         }).AsParallel().ToList<AdminCustomer>();
            if (custs == null) {
                custs = new List<AdminCustomer>();
            }
            return custs;
        }

        internal List<AdminCustomer> GetCustomersByPage(int page = 1, int perpage = 10) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            
            List<AdminCustomer> custs = (from c in db.Customers
                                         orderby c.lname
                                         select new AdminCustomer {
                                             ID = c.ID,
                                             fname = c.fname,
                                             lname = c.lname,
                                             phone = c.phone,
                                             email = c.email,
                                             created = c.dateAdded,
                                             status = (c.isSuspended == 1) ? "Suspended" : "Active",
                                             ordercount = c.Carts.Where(x => x.payment_id > 0).Count()
                                         }).AsParallel().Skip(perpage * (page - 1)).Take(perpage).ToList<AdminCustomer>();
            if (custs == null) {
                custs = new List<AdminCustomer>();
            }
            return custs;
        }

        internal int Count() {
            int custs = 0;
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            custs = db.Customers.Count();
            return custs;
        }

        internal void Get() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Customer cust = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Customer>();
            if (cust == null) {
                throw new Exception("Customer not found.");
            }
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
            this.Carts = cust.Carts;
        }

        internal void ToggleSuspended() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Customer tmp = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Customer>();
            if (tmp.isSuspended == 0) {
                tmp.isSuspended = 1;
            } else {
                tmp.isSuspended = 0;
            }
            db.SubmitChanges();
        }

        internal void Save() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            PasswordGenerator pw = new PasswordGenerator();
                string new_pass = pw.Generate();

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

            db.Customers.InsertOnSubmit(new_customer);
            db.SubmitChanges();
            this.ID = new_customer.ID;

            SendNotification();
        }

        internal void Update(string email, string fname, string lname, string phone) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();

            Customer c = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Customer>();
            // Make sure we don't have an account with this e-mail address
            if (email != this.email) {
                // Make sure we don't have an account with this e-mail address
                if (Customer.CheckCustomerEmail(email)) {
                    throw new Exception("An account using the E-Mail address you provided already exists.");
                }
                c.email = email;
            }

            // We are going to make an attempt at saving the Customer record
            if (fname != this.fname) {
                c.fname = fname;
            }
            if (lname != this.lname) {
                c.lname = lname;
            }
            if (phone != this.phone) {
                c.phone = phone;
            }

            db.SubmitChanges();
        }

        public static bool CheckCustomerEmail(string email = null) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            if (email == null) {
                return true;
            } else {
                bool exists = false;
                if (db.Customers.Where(x => x.email.Equals(email)).Count() > 0) {
                    exists = true;
                }
                return exists;
            }
        }

        public bool SetDefaultAddress(int id, string type = "billing") {
            bool success = false;
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            using (TransactionScope ts = new TransactionScope()) {
                try {
                    Customer c = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault();
                    if (type == "billing") {
                        c.billingID = id;
                    } else {
                        c.shippingID = id;
                    }
                    db.SubmitChanges();

                    success = true;

                    ts.Complete();
                } catch { }
            }
            return success;
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

        internal List<Address> GetAddresses() {
            List<Address> addresses = new List<Address>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            addresses = db.Addresses.Where(x => x.cust_id.Equals(this.ID) && x.active).ToList<Address>();
            return addresses;
        }

        internal Customer GetCustomerByEmail(string email = null) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            if (email == null) {
                return db.Customers.Where(x => x.email.Equals(this.email)).FirstOrDefault<Customer>();
            } else {
                return db.Customers.Where(x => x.email.Equals(email)).FirstOrDefault<Customer>();
            }
        }

        internal void SendNotification() {
            string[] tos = { this.email };
            StringBuilder sb = new StringBuilder();
            Settings settings = new Settings();

            sb.Append("<p>A new account with this e-mail address was created from <a href='" + settings.Get("SiteURL") + "' title='" + settings.Get("SiteName") + "'>" + settings.Get("SiteName") + "</a>.</p>");
            sb.Append("<hr />");
            sb.AppendFormat("<p>Your password has been automatically generated and is listed below.</p><hr />");
            sb.AppendFormat("<p><strong>Password:</strong> {0}</p>", this.plainpassword);
            sb.Append("<hr /><br />");
            sb.Append("<p style='font-size:11px'>If you feel this was a mistake please disregard this e-mail.</p>");

            UDF.SendEmail(tos, "New Account for " + settings.Get("SiteName"), true, sb.ToString());
            this.plainpassword = "";
        }

        internal void SendChangedPasswordNotification() {
            string[] tos = { this.email };
            StringBuilder sb = new StringBuilder();
            Settings settings = new Settings();

            sb.Append("<p>Your password has been changed for your account at <a href='" + settings.Get("SiteURL") + "' title='" + settings.Get("SiteName") + "'>" + settings.Get("SiteName") + "</a>.</p>");
            sb.Append("<hr />");
            sb.AppendFormat("<p><strong>Password:</strong> {0}</p>", this.plainpassword);
            sb.Append("<hr /><br />");
            sb.Append("<p style='font-size:11px'>If you feel this was a mistake please disregard this e-mail.</p>");

            UDF.SendEmail(tos, "Password Change for your account at " + settings.Get("SiteName"), true, sb.ToString());
            this.plainpassword = "";
        }

        internal void GeneratePassword() {
            try {
                PasswordGenerator pw = new PasswordGenerator();
                string new_pass = pw.Generate();
                this.plainpassword = new_pass;
                this.password = UDF.EncryptString(new_pass);
            } catch (Exception) {}
        }

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

        internal void UpdatePassword() {
            Customer tmp = new Customer();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            tmp = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Customer>();
            tmp.password = this.password;
            db.SubmitChanges();
        }

    }
}