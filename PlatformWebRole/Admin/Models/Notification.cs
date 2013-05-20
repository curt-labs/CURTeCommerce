using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models {
    public class Notification {
        public int orderID { get; set; }
        public string message { get; set; }

        public static List<Notification> GetNotifications() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Notification> notices = new List<Notification>();
            Settings settings = new Settings();
            if (settings.Get("EDIOrderProcessing") == "true") {
                List<Cart> notsubmitted = Cart.GetLongUnsubmittedOrders();
                foreach (Cart order in notsubmitted) {
                    Notification notice = new Notification {
                        orderID = order.payment_id,
                        message = "Order #" + order.payment_id + " is over 2 hours old and has not been sent to CURT."
                    };
                    notices.Add(notice);
                }
                List<Cart> notacknowledged = Cart.GetUnacknowledgedOrders();
                foreach (Cart order in notacknowledged) {
                    Notification notice = new Notification {
                        orderID = order.payment_id,
                        message = "Order #" + order.payment_id + " was sent to CURT over 2 hours ago and has not been acknowledged yet."
                    };
                    notices.Add(notice);
                }
            }
            return notices;
        }
    }
}