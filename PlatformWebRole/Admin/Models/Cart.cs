using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Admin.Models;
using System.Text;
using System.Globalization;
using System.Net;

namespace Admin
{
    partial class Cart {

        partial void OnCreated() {
            this.date_created = DateTime.Now;
            this.ship_to = 0;
            this.bill_to = 0;
            this.voided = false;
        }

        public CartItem Add(int partID = 0, int quantity = 1) {
            APIPart part = CURTAPI.GetPart(partID);
            string upcval = part.attributes.Where(x => x.key.ToLower().Equals("upc")).Select(x => x.value).FirstOrDefault();
            CartItem i = new CartItem(partID, quantity, Convert.ToDecimal(part.listPrice.Replace("$", "")), part.shortDesc, upcval);
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            try {
                CartItem item = db.CartItems.Where(x => x.partID == partID).Where(x => x.order_id == this.ID).First<CartItem>();
                item.quantity += quantity;
            } catch {
                i.order_id = this.ID;
                db.CartItems.InsertOnSubmit(i);
            };
            db.SubmitChanges();
            if (this.CartItems.Any(item => item.partID == i.partID)) {
                this.CartItems.Where(x => x.partID.Equals(partID)).FirstOrDefault<CartItem>().quantity += quantity;
            } else {
                this.CartItems.Add(i);
            }
            return i;
        }

        public void Update(int partID = 0, int quantity = 0) {
            if (quantity > 0) {
                if (this.cust_id > 0) {
                    EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                    CartItem i = db.CartItems.Where(x => x.order_id == this.ID).Where(x => x.partID == partID).First<CartItem>();
                    i.quantity = quantity;
                    db.SubmitChanges();
                }
                this.CartItems.Where(x => x.partID.Equals(partID)).FirstOrDefault<CartItem>().quantity = quantity;
            } else {
                Remove(partID);
            }
        }

        public void Remove(int partID = 0) {
            if (this.cust_id > 0) {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                CartItem i = db.CartItems.Where(x => x.order_id == this.ID).Where(x => x.partID == partID).First<CartItem>();
                db.CartItems.DeleteOnSubmit(i);
                db.SubmitChanges();
            }
            if (this.CartItems != null && this.CartItems.Any(item => item.partID == partID)) {
                this.CartItems.Remove(this.CartItems.Where(x => x.partID.Equals(partID)).FirstOrDefault<CartItem>());
            }
        }

        public void Void() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Cart c = db.Carts.Where(x => x.ID == this.ID).First<Cart>();
            c.voided = true;
            db.SubmitChanges();
        }

        public Cart Save(int cust_id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            try {
                Customer cust = db.Customers.Where(x => x.ID == cust_id).First<Customer>();
                Cart c = new Cart {
                    cust_id = cust_id,
                    date_created = DateTime.Now,
                    ship_to = cust.shippingID,
                    bill_to = cust.billingID,
                    shipping_price = 0
                };
                db.Carts.InsertOnSubmit(c);
                db.SubmitChanges();
                return c;
            } catch { };
            return this;
        }

        public void Empty() {
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
        }

        public List<CartItem> GetParts() {
            string partlist = "";
            List<int> items = new List<int>();
            if (this.cust_id > 0) {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                items = db.CartItems.Where(x => x.order_id == this.ID).Select(x => x.partID).ToList();
            } else {
                items = CartItems.Select(x => x.partID).ToList();
            }
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
            return this.CartItems.ToList<CartItem>();
        }

        public int getCount() {
            int count = 0;
            if (this.cust_id > 0) {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                try {
                    count = db.CartItems.Where(x => x.order_id == this.ID).Sum(x => x.quantity);
                } catch { }
            } else {
                count = this.CartItems.Sum(x => x.quantity);
            }
            return count;
        }

        public decimal getTotal() {
            decimal total = 0;
            total += GetSubTotal();
            total += this.shipping_price;
            total += this.tax;
            return total;
        }

        public decimal GetSubTotal() {
            decimal total = 0;
            foreach (Admin.CartItem item in this.CartItems) {
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
        }

        public bool HasFreeShipping() {
            try {
                List<int> excludedStates = new List<int> { 2, 15 };
                if (this.Shipping != null && this.Shipping.state != null && excludedStates.Contains(this.Shipping.state)) {
                    return false;
                }
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

        internal Cart Get(int id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Cart c = db.Carts.Where(x => x.ID == id).FirstOrDefault<Cart>();
            return c;
        }

        internal Cart GetByPayment(int id = 0) {
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

        internal Payment getPayment() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Payment p = new Payment();
            try {
                p = db.Payments.Where(x => x.ID == this.payment_id).First<Payment>();
            } catch { };
            return p;
        }

        internal void SendConfirmation() {
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
            sb.Append("<h2>Thank you for your order!</h2>");
            sb.Append("<hr />");
            sb.AppendFormat("<p><strong>Order ID:</strong> {0}<br />", this.payment_id);
            sb.AppendFormat("<strong>Paid By:</strong> {0} on {1}</p>", payment.PaymentTypes.name, String.Format("{0:M/d/yyyy} at {0:h:mm tt}", payment.created.ToLocalTime()));
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
            sb.Append("<tr><td colspan=\"2\" style=\"border-top: 1px solid #222; text-align: right;\"><strong>SubTotal:<strong></td>");
            sb.AppendFormat("<td style=\"border-top: 1px solid #222; text-align:right;\"><strong>{0}</strong></td></tr>", String.Format("{0:C}", this.GetSubTotal()));
            sb.AppendFormat("<tr><td colspan=\"2\" style=\"text-align: right;\">({0}) Shipping:</td>", myTI.ToTitleCase(this.shipping_type.Replace("_", " ")));
            sb.AppendFormat("<td style=\"text-align:right;\">{0}</td></tr>", (this.shipping_price == 0) ? "Free" : String.Format("{0:C}", this.shipping_price));
            sb.Append("<tr><td colspan=\"2\" style=\"text-align: right;\">Tax:</td>");
            sb.AppendFormat("<td style=\"text-align:right;\">{0}</td></tr>", String.Format("{0:C}", this.tax));
            sb.Append("<tr><td colspan=\"2\" style=\"text-align: right;\"><strong>Total:<strong></td>");
            total += this.shipping_price;
            sb.AppendFormat("<td style=\"text-align:right;\"><strong>{0}</strong></td></tr>", String.Format("{0:C}", this.getTotal()));
            sb.Append("</tfoot></table>");
            sb.Append("<hr /><br />");
            sb.Append("<p style='font-size:11px'>If you have any questions, or if you did not place this order, please <a href='" + settings.Get("SiteURL") + "contact'>contact us</a>.</p>");
            sb.Append("</body></html>");
            UDF.SendEmail(tos, settings.Get("SiteName") + " Order Confirmation", true, sb.ToString());
        }

        internal void SetTax() {
            decimal tax = 0;
            tax = Math.Round(((this.GetSubTotal() + this.shipping_price) * (this.Shipping.State1.taxRate / 100)), 2);

            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Cart c = db.Carts.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Cart>();
            c.tax = tax;
            db.SubmitChanges();

            this.tax = tax;
        }

        public ShippingResponse getShipping() {
            Settings settings = new Settings();
            FedExAuthentication auth = new FedExAuthentication {
                AccountNumber = Convert.ToInt32(settings.Get("FedExAccount")),
                Key = settings.Get("FedExKey"),
                Password = settings.Get("FedExPassword"),
                CustomerTransactionId = "",
                MeterNumber = Convert.ToInt32(settings.Get("FedExMeter"))
            };

            ShippingAddress destination = new ShippingAddress();
            destination = this.Shipping.getShipping();
            DistributionCenter d = new DistributionCenter().GetNearest(this.Shipping.GeoLocate());
            ShippingAddress origin = d.getAddress().getShipping();
            List<int> parts = new List<int>();
            foreach (CartItem item in this.CartItems) {
                for (int i = 1; i <= item.quantity; i++) {
                    parts.Add(item.partID);
                }
            }

            ShippingResponse response = CURTAPI.GetShipping(auth, origin, destination, parts);

            return response;
        }

        internal List<Cart> GetAll() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.Carts.Where(x => x.payment_id > 0).OrderByDescending(x => x.date_created).ToList<Cart>();
            } catch (Exception) {
                return new List<Cart>();
            }
        }
    }
}
