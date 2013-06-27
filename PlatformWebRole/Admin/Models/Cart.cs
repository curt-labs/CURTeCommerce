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
            this.date_created = DateTime.UtcNow;
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

        public OrderHistory GetStatus() {
            OrderHistory history = new OrderHistory();
            try {
                history = this.OrderHistories.OrderByDescending(x => x.dateAdded).First();
            } catch {
                history = new OrderHistory {
                    statusID = 0,
                    OrderStatus = new OrderStatus {
                        status = "Unknown",
                        ID = 0
                    },
                };
            }
            return history;
        }

        public void SetStatus(int statusID, string name) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            OrderHistory status = new OrderHistory {
                dateAdded = DateTime.UtcNow,
                changedBy = name,
                orderID = this.ID,
                statusID = statusID
            };
            status.Save();
        }

        public void Void(string name) {
            SetStatus((int)OrderStatuses.Void, name);
        }

        public Cart Save(int cust_id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            try {
                Customer cust = db.Customers.Where(x => x.ID == cust_id).First<Customer>();
                Cart c = new Cart {
                    cust_id = cust_id,
                    date_created = DateTime.UtcNow,
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

        public void SaveNotes(string notes = "") {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Cart c = db.Carts.Where(x => x.ID == this.ID).First<Cart>();
            c.notes = notes;
            db.SubmitChanges();
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
            total += this.handling_fee;
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
                setHandlingFee();
            } catch (Exception) { }
        }

        public void setHandlingFee() {
            if (this.cust_id > 0) {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                try {
                    Cart cart = db.Carts.Where(x => x.ID == this.ID).First<Cart>();
                    cart.handling_fee = cart.Shipping.State1.handlingFee;
                    this.handling_fee = cart.handling_fee;
                    db.SubmitChanges();
                } catch { }
            }
        }

        public bool HasFreeShipping() {
            try {
                List<int> excludedStates = new State().GetExcludedStates().Select(x => x.stateID).ToList();
                if (this.Shipping != null && excludedStates.Contains(this.Shipping.state)) {
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
                created = DateTime.UtcNow,
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

        internal Shipment AddShipment(string trackingnum) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Shipment shipment = new Shipment {
                order_id = this.ID,
                tracking_number = trackingnum,
                dateShipped = DateTime.UtcNow,
            };
            db.Shipments.InsertOnSubmit(shipment);
            db.SubmitChanges();
            return shipment;
        }

        internal bool ClearShipments() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            try {
                List<Shipment> shipments = db.Shipments.Where(x => x.order_id.Equals(this.ID)).ToList();
                db.Shipments.DeleteAllOnSubmit(shipments);
                db.SubmitChanges();
            } catch {
                return false;
            }
            return true;
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
            if (this.handling_fee > 0) {
                sb.AppendFormat("<tr><td colspan=\"2\" style=\"text-align: right;\">Handling:</td><td style=\"text-align:right;\">{0}</td></tr>", String.Format("{0:C}", this.handling_fee));
            }
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

        internal void SendInternalOrderEmail() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Payment payment = this.getPayment();

            string phone = db.Customers.Where(x => x.ID == this.cust_id).Select(x => x.phone).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            Settings settings = new Settings();
            string supportemail = settings.Get("SupportEmail");

            List<string> tolist = new List<string>();
            if (settings.Get("EDIOrderProcessing") != "true") {
                string curtemail = settings.Get("CurtOrderEmail");
                if (curtemail.Trim() != "") {
                    tolist.Add(curtemail);
                }
            }
            tolist.Add(supportemail);
            string[] tos = tolist.ToArray();
            decimal total = 0;
            sb.Append("<html><body style=\"font-family: arial, helvetica,sans-serif;\">");
            sb.Append("<a href=\"" + settings.Get("SiteURL") + "\"><img src=\"" + settings.Get("EmailLogo") + "\" alt=\"" + settings.Get("SiteName") + "\" /></a>");
            sb.Append("<h2>A New Order Has Been Placed</h2>");
            sb.Append("<hr />");
            sb.AppendFormat("<p><strong>Customer ID:</strong> {0}<br />", settings.Get("CURTAccount"));
            sb.AppendFormat("<p><strong>Order ID:</strong> {0}<br />", this.payment_id);
            sb.AppendFormat("<strong>Paid By:</strong> {0} on {1}</p>", payment.PaymentTypes.name, String.Format("{0:M/d/yyyy} at {0:h:mm tt}", payment.created.ToLocalTime()));
            sb.AppendFormat("<strong>Phone:</strong> {0}</p>", phone);
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
            sb.AppendFormat("<tr><td colspan=\"2\" style=\"border-top: 1px solid #222; text-align: right;\">({0}) Shipping:</td>", myTI.ToTitleCase(this.shipping_type.Replace("_", " ")));
            sb.AppendFormat("<td style=\"border-top: 1px solid #222; text-align:right;\">{0}</td></tr>", (this.shipping_price == 0) ? "Free" : String.Format("{0:C}", this.shipping_price));
            if (this.handling_fee > 0) {
                sb.AppendFormat("<tr><td colspan=\"2\" style=\"text-align: right;\">Handling:</td><td style=\"text-align:right;\">{0}</td></tr>", String.Format("{0:C}", this.handling_fee));
            }
            sb.Append("<tr><td colspan=\"2\" style=\"text-align: right;\"><strong>SubTotal:<strong></td>");
            sb.AppendFormat("<td style=\"text-align:right;\"><strong>{0}</strong></td></tr>", String.Format("{0:C}", this.GetSubTotal()));
            sb.Append("<tr><td colspan=\"2\" style=\"text-align: right;\">Tax:</td>");
            sb.AppendFormat("<td style=\"text-align:right;\">{0}</td></tr>", String.Format("{0:C}", this.tax));
            sb.Append("<tr><td colspan=\"2\" style=\"text-align: right;\"><strong>Total:<strong></td>");
            total += this.shipping_price;
            sb.AppendFormat("<td style=\"text-align:right;\"><strong>{0}</strong></td></tr>", String.Format("{0:C}", this.getTotal()));
            sb.Append("</tfoot></table>");
            sb.Append("<hr /><br />");
            sb.Append("</body></html>");
            UDF.SendEmail(tos, settings.Get("CURTAccount") + " Order - PO " + this.payment_id, true, sb.ToString());
        }

        internal void SendShippingNotification() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Shipment> shipments = db.Shipments.Where(x => x.order_id.Equals(this.ID)).ToList<Shipment>();

            if (shipments.Count > 0) {
                DateTime shipdate = shipments[0].dateShipped ?? DateTime.UtcNow;
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
                foreach (Shipment shipment in shipments) {
                    sb.Append("<a href=\"http://www.fedex.com/Tracking?tracknumber_list=" + shipment.tracking_number + "\">" + shipment.tracking_number + "</a><br />");
                }
                sb.Append("<hr /><br />");
                sb.Append("<p style='font-size:11px'>If you have any questions, or if you did not place this order, please <a href='" + settings.Get("SiteURL") + "contact'>contact us</a>.</p>");
                sb.Append("</body></html>");
                UDF.SendEmail(tos, settings.Get("SiteName") + " Shipping Notification", true, sb.ToString());
            }
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

        public static List<Cart> GetLongUnsubmittedOrders() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<int> statuses = new List<int> { (int)OrderStatuses.Void, (int)OrderStatuses.Cancelled, (int)OrderStatuses.AwaitingCancellation, (int)OrderStatuses.Fraudulent, (int)OrderStatuses.PaymentDeclined, (int)OrderStatuses.PaymentPending };
            List<Cart> orders = new List<Cart>();
            orders = db.Carts.Where(x => x.payment_id > 0 && x.OrderEDI == null && x.Payment.created < DateTime.UtcNow.AddHours(-2) && !statuses.Contains(x.OrderHistories.OrderByDescending(y => y.dateAdded).Select(y => y.statusID).FirstOrDefault())).ToList();
            return orders;
        }

        public static List<Cart> GetUnacknowledgedOrders() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Cart> orders = new List<Cart>();
            List<int> statuses = new List<int> { (int)OrderStatuses.Void, (int)OrderStatuses.Cancelled, (int)OrderStatuses.AwaitingCancellation, (int)OrderStatuses.Fraudulent, (int)OrderStatuses.PaymentDeclined, (int)OrderStatuses.PaymentPending };
            orders = db.Carts.Where(x => x.payment_id > 0 && x.OrderEDI != null && x.OrderEDI.dateGenerated < DateTime.UtcNow.AddHours(-2) && x.OrderEDI.dateAcknowledged == null && !statuses.Contains(x.OrderHistories.OrderByDescending(y => y.dateAdded).Select(y => y.statusID).FirstOrDefault())).ToList();
            return orders;
        }

    }
}
