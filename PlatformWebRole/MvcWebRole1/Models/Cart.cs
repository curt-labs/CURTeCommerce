using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using EcommercePlatform.Models;
using System.Text;
using System.Globalization;
using System.Net;
using Newtonsoft.Json;

namespace EcommercePlatform
{
    partial class Cart {
        public string paypalToken { get; set; }

        partial void OnCreated() {
            this.date_created = DateTime.Now;
            this.ship_to = 0;
            this.bill_to = 0;
            this.voided = false;
        }

        public void Add(int partID = 0, int quantity = 1) {
            if (this.payment_id == 0) {
                APIPart part = CURTAPI.GetPart(partID);
                string upcval = part.attributes.Where(x => x.key.ToLower().Equals("upc")).Select(x => x.value).FirstOrDefault();
                string weight = part.attributes.Where(x => x.key.ToLower().Equals("weight")).Select(x => x.value).FirstOrDefault();
                CartItem i = new CartItem(partID, quantity, Convert.ToDecimal(part.listPrice.Replace("$", "")), part.shortDesc, upcval, Convert.ToDecimal(weight));

                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                try {
                    CartItem item = db.CartItems.Where(x => x.partID == partID).Where(x => x.order_id == this.ID).First<CartItem>();
                    item.quantity += quantity;
                } catch {
                    i.order_id = this.ID;
                    db.CartItems.InsertOnSubmit(i);
                };
                db.SubmitChanges();
            } else {
                UDF.ExpireCart(this.cust_id);
            }
        }

        public void Update(int partID = 0, int quantity = 0) {
            if (this.payment_id == 0) {
                if (quantity > 0) {
                    EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                    CartItem i = db.CartItems.Where(x => x.order_id == this.ID).Where(x => x.partID == partID).First<CartItem>();
                    i.quantity = quantity;
                    db.SubmitChanges();
                } else {
                    Remove(partID);
                }
            } else {
                UDF.ExpireCart(this.cust_id);
            }
        }

        public void Remove(int partID = 0) {
            if (this.payment_id == 0) {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                CartItem i = db.CartItems.Where(x => x.order_id == this.ID).Where(x => x.partID == partID).First<CartItem>();
                db.CartItems.DeleteOnSubmit(i);
                db.SubmitChanges();
            } else {
                UDF.ExpireCart(this.cust_id);
            }
        }

        public void RemoveCart() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Cart i = db.Carts.Where(x => x.ID.Equals(this.ID)).First<Cart>();
            db.Carts.DeleteOnSubmit(i);
            db.SubmitChanges();
        }

        public void UpdateCart(int cust_id = 0) {
            if (this.payment_id == 0) {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                Cart c = db.Carts.Where(x => x.ID.Equals(this.ID)).First<Cart>();
                c.cust_id = cust_id;
                try {
                    Customer cust = db.Customers.Where(x => x.ID == cust_id).First<Customer>();
                    c.ship_to = cust.shippingID;
                    c.bill_to = cust.billingID;
                } catch { }
                db.SubmitChanges();
            } else {
                UDF.ExpireCart(this.cust_id);
            }
        }

        public Cart Save() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            try {
                Cart c = new Cart {
                    cust_id = 0,
                    date_created = DateTime.Now,
                    shipping_price = 0
                };
                db.Carts.InsertOnSubmit(c);
                db.SubmitChanges();
                return c;
            } catch { };
            return this;
        }

        public void Empty() {
            if (this.payment_id == 0) {
                if (this.cust_id > 0) {
                    EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                    List<CartItem> items = new List<CartItem>();
                    try {
                        items = db.CartItems.Where(x => x.order_id == this.ID).ToList<CartItem>();
                        db.CartItems.DeleteAllOnSubmit(items);
                        db.SubmitChanges();
                    } catch { };
                }
                foreach (CartItem i in this.CartItems) {
                    this.CartItems.Remove(i);
                }
            } else {
                UDF.ExpireCart(this.cust_id);
            }
        }

        public void GetParts() {
            string partlist = "";
            List<int> items = this.CartItems.Select(x => x.partID).ToList();
            foreach (int item in items) {
                if (partlist != "") partlist += ",";
                partlist += item.ToString();
            }
            List<APIPart> parts = new List<APIPart>();
            if (partlist.Length > 0) {
                parts = CURTAPI.GetPartsByList(partlist);
                if (parts.Count > 0) {
                    foreach (CartItem item in this.CartItems) {
                        item.apipart = parts.Find(x => x.partID == item.partID);
                    };
                }
            }
        }

        public int getCount() {
            return this.CartItems.Sum(x => x.quantity);
        }

        public decimal getTotal() {
            decimal total = 0;
            foreach (EcommercePlatform.CartItem item in this.CartItems) {
                total += (item.quantity * item.price);
            }
            total += this.shipping_price;
            total += this.tax;
            return total;
        }

        public decimal GetSubTotal() {
            decimal total = 0;
            foreach (EcommercePlatform.CartItem item in this.CartItems) {
                total += (item.quantity * item.price);
            }
            return total;
        }

        public void setShippingType(string shipping_type, decimal shipping_price) {
            if (this.cust_id > 0) {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                try {
                    Cart cart = db.Carts.Where(x => x.ID == this.ID).First<Cart>();
                    cart.shipping_price = shipping_price;
                    cart.shipping_type = shipping_type;
                    db.SubmitChanges();
                } catch { }
            }
            this.shipping_type = shipping_type;
            this.shipping_price = shipping_price;
        }

        internal void GetBilling() {
            Address billing = new Address();
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                billing = db.Addresses.Where(x => x.ID.Equals(this.bill_to)).FirstOrDefault<Address>();
            } catch (Exception) { }
            this.Billing = billing;
        }

        internal void SetBilling(int id = 0) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                if (id == 0) {
                    id = db.Customers.Where(x => x.ID == this.cust_id).Select(x => x.billingID).First();
                }
                Cart c = db.Carts.Where(x => x.ID == this.ID).First<Cart>();
                c.bill_to = id;
                this._bill_to = id;
                db.SubmitChanges();
            } catch (Exception) { }
            GetBilling();
        }

        internal void GetShipping() {
            Address shipping = new Address();
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                shipping = db.Addresses.Where(x => x.ID.Equals(this.ship_to)).FirstOrDefault<Address>();
            } catch (Exception) { }
            this.Shipping = shipping;
        }

        internal void SetShipping(int id = 0) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                if (id == 0) {
                    id = db.Customers.Where(x => x.ID == this.cust_id).Select(x => x.shippingID).First();
                }
                Cart c = db.Carts.Where(x => x.ID == this.ID).First<Cart>();
                c.ship_to = id;
                db.SubmitChanges();
                this._ship_to = id;
            } catch (Exception) { }
            GetShipping();
        }

        public bool HasFreeShipping() {
            try {
                Settings settings = new Settings();
                decimal freeship = Convert.ToDecimal(settings.Get("FreeShippingAmount"));
                decimal total = 0;
                foreach (CartItem item in this.CartItems) {
                    total += (item.price * item.quantity);
                }
                if (total >= freeship) {
                    return true;
                }
            } catch (Exception) { }
            return false;
        }

        internal void BindAddresses() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            if (this._bill_to == 0) {
                this._bill_to = db.Customers.Where(x => x.ID == this.cust_id).Select(x => x.billingID).FirstOrDefault();
            }
            if (this._ship_to == 0) {
                this._ship_to = db.Customers.Where(x => x.ID == this.cust_id).Select(x => x.shippingID).FirstOrDefault();
            }
            this.Billing = db.Addresses.Where(x => x.ID.Equals(this.bill_to)).FirstOrDefault<Address>();
            this.Shipping = db.Addresses.Where(x => x.ID.Equals(this.ship_to)).FirstOrDefault<Address>();
            Customer cust = null;
            if (this.Billing == null) {
                cust = db.Customers.Where(x => x.ID.Equals(this.cust_id)).FirstOrDefault<Customer>();
                if (cust != null) {
                    this.Billing = db.Addresses.Where(x => x.ID.Equals(cust.billingID)).FirstOrDefault<Address>();
                    this._bill_to = cust.billingID;
                } else {
                    this.Billing = new Address();
                    this._bill_to = 0;
                }
            }
            if (this.Shipping == null) {
                if (cust == null) {
                    cust = db.Customers.Where(x => x.ID.Equals(this.cust_id)).FirstOrDefault<Customer>();
                }
                if (cust != null) {
                    this.Shipping = db.Addresses.Where(x => x.ID.Equals(cust.shippingID)).FirstOrDefault<Address>();
                    this._ship_to = cust.shippingID;
                } else {
                    this.Shipping = new Address();
                    this._ship_to = 0;
                }
            }
        }

        internal Cart Get(int id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Cart c = db.Carts.Where(x => x.ID == id).FirstOrDefault<Cart>();
            return c;
        }

        internal Cart GetByPaymentID(int id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Cart c = db.Carts.Where(x => x.payment_id == id).FirstOrDefault<Cart>();
            return c;
        }

        internal void AddPayment(string type, string confirmKey, string status) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            PaymentType ptype = db.PaymentTypes.Where(x => x.name.ToLower() == type.ToLower()).FirstOrDefault();
            Payment p = new Payment {
                type = ptype.ID,
                created = DateTime.Now,
                confirmationKey = confirmKey,
                status = status
            };
            db.Payments.InsertOnSubmit(p);
            db.SubmitChanges();
            Cart c = db.Carts.Where(x => x.ID == this.ID).FirstOrDefault<Cart>();
            c.payment_id = p.ID;
            db.SubmitChanges();
            this._payment_id = p.ID;
        }

        internal void UpdatePaymentConfirmationCode(string confirmcode) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Payment p = db.Payments.Where(x => x.ID.Equals(this.payment_id)).FirstOrDefault<Payment>();
            p.confirmationKey = confirmcode;
            db.SubmitChanges();
        }

        internal void UpdatePayment(string status) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Payment p = db.Payments.Where(x => x.ID.Equals(this.payment_id)).FirstOrDefault<Payment>();
            p.status = status;
            db.SubmitChanges();
            string[] voidstatuses = {"CANCELLED","CANCELLED_BY_GOOGLE"};
            if(voidstatuses.Contains(status.ToUpper())) {
                // void order
                Void();
                // send notification about voided order
                SendCancelNotice();
            }
        }

        internal Payment getPayment() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Payment p = new Payment();
            try {
                p = db.Payments.Where(x => x.ID == this.payment_id).First<Payment>();
            } catch { };
            return p;
        }

        public void Void() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Cart c = db.Carts.Where(x => x.ID == this.ID).First<Cart>();
            c.voided = true;
            db.SubmitChanges();
        }

        public bool Validate() {
            bool valid = false;
            if (this.bill_to > 0 && this.ship_to > 0 && this.GetSubTotal() > 0) {
                valid = true;
            }
            return valid;
        }

        internal void SendConfirmation() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Payment payment = this.getPayment();

            string toemail = db.Customers.Where(x => x.ID == this.cust_id).Select(x => x.email).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            TextInfo myTI = new CultureInfo("en-US",false).TextInfo;
            Settings settings = new Settings();

            string[] tos = { toemail };
            decimal total = 0;
            sb.Append("<html><body style=\"font-family: arial, helvetica,sans-serif;\">");
            sb.Append("<a href=\"" + settings.Get("SiteURL") + "\"><img src=\"" + settings.Get("EmailLogo") + "\" alt=\"" + settings.Get("SiteName") + "\" /></a>");
            sb.Append("<h2>Thank you for your order!</h2>");
            sb.Append("<hr />");
            sb.AppendFormat("<p><strong>Order ID:</strong> {0}<br />", this.payment_id);
            sb.AppendFormat("<strong>Paid By:</strong> {0} on {1}</p>", payment.PaymentTypes.name, String.Format("{0:M/d/yyyy} at {0:h:mm tt}",payment.created));
            sb.Append("<p style=\"font-size: 12px;\"><strong style=\"font-size: 14px;\">Billing Address:</strong><br />");
            sb.AppendFormat("{0} {1}<br />", this.Billing.first, this.Billing.last);
            sb.AppendFormat("{0}{1}<br />{2}, {3} {4}<br />{5}</p>", this.Billing.street1, this.Billing.street2, this.Billing.city, this.Billing.State1.abbr, this.Billing.postal_code, this.Billing.State1.Country.name);
            sb.Append("<p style=\"font-size: 12px;\"><strong style=\"font-size: 14px;\">Shipping Address:</strong><br />");
            sb.AppendFormat("{0} {1}<br />", this.Shipping.first, this.Shipping.last);
            sb.AppendFormat("{0}{1}<br />{2}, {3} {4}<br />{5}</p>", this.Shipping.street1, this.Shipping.street2, this.Shipping.city, this.Shipping.State1.abbr, this.Shipping.postal_code, this.Shipping.State1.Country.name);
            sb.Append("<table style=\"width: 100%;\" border=\"0\" cellpadding=\"5\" cellspacing=\"0\"><thead><tr><th style=\"background-color: #343434; color: #fff;\">Item</th><th style=\"background-color: #343434; color: #fff;\">Quantity</th><th style=\"background-color: #343434; color: #fff;\">Price</th></tr></thead><tbody style=\"font-size: 12px;\">");
            foreach(CartItem item in this.CartItems) {
                sb.AppendFormat("<tr><td><a href=\"" + settings.Get("SiteURL") + "part/{0}\">{1}</a></td><td style=\"text-align:center;\">{2}</td><td style=\"text-align:right;\">{3}</td></tr>", item.partID, item.shortDesc, item.quantity, String.Format("{0:C}", item.price));
                total += (item.quantity * item.price);
            }
            sb.Append("</tbody><tfoot style=\"font-size: 12px;\">");
            sb.Append("<tr><td colspan=\"2\" style=\"border-top: 1px solid #222; text-align: right;\"><strong>SubTotal:<strong></td>");
            sb.AppendFormat("<td style=\"border-top: 1px solid #222; text-align:right;\"><strong>{0}</strong></td></tr>", String.Format("{0:C}", this.GetSubTotal()));
            sb.AppendFormat("<tr><td colspan=\"2\" style=\"text-align: right;\">({0}) Shipping:</td>", myTI.ToTitleCase(this.shipping_type.Replace("_", " ")));
            sb.AppendFormat("<td style=\"text-align:right;\">{0}</td></tr>", (this.shipping_price == 0) ? "Free" : String.Format("{0:C}", this.shipping_price));
            sb.Append("<tr><td colspan=\"2\" style=\"text-align: right;\">Tax:</td>");
            sb.AppendFormat("<td style=\"text-align:right;\">{0}</td></tr>", String.Format("{0:C}", this.tax));
            sb.Append("<tr><td colspan=\"2\" style=\"text-align: right;\"><strong>Total:<strong></td>");
            total += this.shipping_price;
            sb.AppendFormat("<td style=\"text-align:right;\"><strong>{0}</strong></td></tr>",String.Format("{0:C}", this.getTotal()));
            sb.Append("</tfoot></table>");
            sb.Append("<hr /><br />");
            sb.Append("<p style='font-size:11px'>If you have any questions, or if you did not place this order, please <a href='" + settings.Get("SiteURL") + "contact'>contact us</a>.</p>");
            sb.Append("</body></html>");
            UDF.SendEmail(tos, settings.Get("SiteName") + " Order Confirmation", true, sb.ToString());
        }

        internal void SendShippingNotification() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Shipment> shipments = db.Shipments.Where(x => x.order_id.Equals(this.ID)).ToList<Shipment>();

            if (shipments.Count > 0) {
                DateTime shipdate = (DateTime)shipments[0].dateShipped;
                string toemail = db.Customers.Where(x => x.ID == this.cust_id).Select(x => x.email).FirstOrDefault();
                StringBuilder sb = new StringBuilder();
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                Settings settings = new Settings();

                string[] tos = { toemail };
                sb.Append("<html><body style=\"font-family: arial, helvetica,sans-serif;\">");
                sb.Append("<a href=\"" + settings.Get("SiteURL") + "\"><img src=\"" + settings.Get("EmailLogo") + "\" alt=\"" + settings.Get("SiteName") + "\" /></a>");
                sb.Append("<h2>Your Order Has Shipped!</h2>");
                sb.Append("<hr />");
                sb.AppendFormat("<p><strong>Order ID:</strong> {0}<br />", this.payment_id);
                sb.Append("<p>Your order shipped on " + String.Format("{0:dddd, MMMM d, yyyy}", shipdate) + ". Your Tracking Numbers are:<br />");
                foreach(Shipment shipment in shipments) {
                    sb.Append("<a href=\"http://www.fedex.com/Tracking?tracknumber_list=" + shipment.tracking_number + "\">" + shipment.tracking_number + "</a><br />");
                }
                sb.Append("<hr /><br />");
                sb.Append("<p style='font-size:11px'>If you have any questions, or if you did not place this order, please <a href='" + settings.Get("SiteURL") + "contact'>contact us</a>.</p>");
                sb.Append("</body></html>");
                UDF.SendEmail(tos, settings.Get("SiteName") + " Shipping Notification", true, sb.ToString());
            }
        }

        internal void SendCancelNotice() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Payment payment = this.getPayment();

            string toemail = db.Customers.Where(x => x.ID == this.cust_id).Select(x => x.email).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            Settings settings = new Settings();

            string[] tos = { toemail };
            decimal total = 0;
            sb.Append("<html><body style=\"font-family: arial, helvetica,sans-serif;\">");
            sb.Append("<a href=\"" + settings.Get("SiteURL") + "\"><img src=\"" + settings.Get("EmailLogo") + "\" alt=\"" + settings.Get("SiteName") + "\" /></a>");
            sb.Append("<h2>We're Sorry</h2>");
            sb.Append("<hr />");
            sb.AppendFormat("<p>Your recent order placed with {0} has been cancelled. Google Checkout was unable to process your payment. The order details are listed below.</p>", settings.Get("SiteName"));
            sb.AppendFormat("<p><strong>Order ID:</strong> {0}<br />", this.payment_id);
            sb.AppendFormat("<strong>Paid By:</strong> {0} on {1}</p>", payment.PaymentTypes.name, String.Format("{0:M/d/yyyy} at {0:h:mm tt}", payment.created));
            sb.AppendFormat("<strong>Payment Status:</strong> {0}</p>", payment.status);
            sb.Append("<p style=\"font-size: 12px;\"><strong style=\"font-size: 14px;\">Billing Address:</strong><br />");
            sb.AppendFormat("{0} {1}<br />", this.Billing.first, this.Billing.last);
            sb.AppendFormat("{0}{1}<br />{2}, {3} {4}<br />{5}</p>", this.Billing.street1, this.Billing.street2, this.Billing.city, this.Billing.State1.abbr, this.Billing.postal_code, this.Billing.State1.Country.name);
            sb.Append("<p style=\"font-size: 12px;\"><strong style=\"font-size: 14px;\">Shipping Address:</strong><br />");
            sb.AppendFormat("{0} {1}<br />", this.Shipping.first, this.Shipping.last);
            sb.AppendFormat("{0}{1}<br />{2}, {3} {4}<br />{5}</p>", this.Shipping.street1, this.Shipping.street2, this.Shipping.city, this.Shipping.State1.abbr, this.Shipping.postal_code, this.Shipping.State1.Country.name);
            sb.Append("<table style=\"width: 100%;\" border=\"0\" cellpadding=\"5\" cellspacing=\"0\"><thead><tr><th style=\"background-color: #343434; color: #fff;\">Item</th><th style=\"background-color: #343434; color: #fff;\">Quantity</th><th style=\"background-color: #343434; color: #fff;\">Price</th></tr></thead><tbody style=\"font-size: 12px;\">");
            foreach (CartItem item in this.CartItems) {
                sb.AppendFormat("<tr><td><a href=\"" + settings.Get("SiteURL") + "part/{0}\">{1}</a></td><td style=\"text-align:center;\">{2}</td><td style=\"text-align:right;\">{3}</td></tr>", item.partID, item.shortDesc, item.quantity, String.Format("{0:C}", item.price));
                total += (item.quantity * item.price);
            }
            sb.Append("</tbody><tfoot style=\"font-size: 12px;\">");
            sb.AppendFormat("<tr><td colspan=\"2\" style=\"border-top: 1px solid #222;text-align: right;\">({0}) Shipping:</td>", myTI.ToTitleCase(this.shipping_type.Replace("_", " ")));
            sb.AppendFormat("<td style=\"border-top: 1px solid #222;text-align:right;\">{0}</td></tr>", (this.shipping_price == 0) ? "Free" : String.Format("{0:C}", this.shipping_price));
            sb.Append("<tr><td colspan=\"2\" style=\"text-align: right;\"><strong>Total:<strong></td>");
            total += this.shipping_price;
            sb.AppendFormat("<td style=\"text-align:right;\"><strong>{0}</strong></td></tr>", String.Format("{0:C}", total));
            sb.Append("</tfoot></table>");
            sb.Append("<hr /><br />");
            sb.Append("<p style='font-size:11px'>We hope you come back and try your order again.  If you have any questions, please <a href='" + settings.Get("SiteURL") + "contact'>contact us</a>.</p>");
            sb.Append("</body></html>");
            UDF.SendEmail(tos, settings.Get("SiteName") + " Order Cancellation Notice", true, sb.ToString());
        }

        internal void SetTax() {
            decimal tax = 0;
            tax = Math.Round(((this.GetSubTotal() + this.shipping_price) * (this.Billing.State1.taxRate / 100)),2);

            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Cart c = db.Carts.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Cart>();
            c.tax = tax;
            db.SubmitChanges();

            this.tax = tax;
        }

        public Cart GetByPayment(string confirmnumber) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                Cart cart = (from c in db.Carts
                          join p in db.Payments on c.payment_id equals p.ID
                          where p.confirmationKey.Equals(confirmnumber)
                          select c).First();
                return cart;
            } catch (Exception) {
                return null;
            }
        }
    }

    partial class CartItem {
        public APIPart apipart { get; set; }
        public CartItem(int partID = 0, int quantity = 0, decimal price = 0, string shortDesc = "", string upc = "", decimal weight = 0) {
            this.partID = partID;
            this.quantity = quantity;
            this.price = price;
            this.shortDesc = shortDesc;
            this.upc = upc;
            this.weight = weight;
        }

        public string GetImage() {
            try {
                string image = "";
                WebClient wc = new WebClient();
                wc.Proxy = null;
                Settings settings = new Settings();

                StringBuilder sb = new StringBuilder(settings.Get("CURTAPIDOMAIN"));
                sb.AppendFormat("GetPart?partID={0}&dataType=JSON", this.partID);

                string resp = wc.DownloadString(sb.ToString());
                APIPart part = Newtonsoft.Json.JsonConvert.DeserializeObject<APIPart>(resp);
                image = part.images.Where(x => x.size.Equals("Grande") && x.sort.ToString().Contains('a')).Select(x => x.path).FirstOrDefault<string>();

                if (image == null) {
                    foreach (APIImage img in part.images) {
                        image = img.path;
                        break;
                    }
                }

                return image;
            } catch (Exception e) {
                return e.Message;
            }
        }


    }

}
