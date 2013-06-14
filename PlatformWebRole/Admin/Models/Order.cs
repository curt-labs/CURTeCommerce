using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models {
    public class Orders {

        public int orderID { get; set; }
        public string customerName { get; set; }
        public int itemcount { get; set; }
        public DateTime orderDate { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderHistory> history { get; set; }
        public decimal total { get; set; }
        public OrderEDI edi { get; set; }

        public List<Orders> GetAll() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Orders> orders = new List<Orders>();
            orders = (from c in db.Carts
                      where c.payment_id != 0
                      select new Orders {
                          orderID = c.payment_id,
                          customerName = c.Customer.fname + " " + c.Customer.lname,
                          itemcount = c.CartItems.Count,
                          orderDate = c.Payment.created,
                          PaymentMethod = c.Payment.PaymentTypes.name,
                          history = c.OrderHistories.ToList(),
                          total = c.getTotal(),
                          edi = c.OrderEDI
                      }).ToList<Orders>();

            return orders;
        }

        public List<Orders> GetOrdersByPage(int page = 1, int perpage = 10) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Orders> orders = new List<Orders>();
            orders = (from c in db.Carts
                      orderby c.payment_id descending
                      where c.payment_id != 0
                      select new Orders {
                          orderID = c.payment_id,
                          customerName = c.Customer.fname + " " + c.Customer.lname,
                          itemcount = c.CartItems.Count,
                          orderDate = c.Payment.created,
                          PaymentMethod = c.Payment.PaymentTypes.name,
                          history = c.OrderHistories.ToList(),
                          total = c.getTotal(),
                          edi = c.OrderEDI
                      }).Skip(perpage * (page - 1)).Take(perpage).ToList<Orders>();

            return orders;
        }

        public OrderHistory GetStatus() {
            OrderHistory history = new OrderHistory();
            try {
                history = this.history.OrderByDescending(x => x.dateAdded).First();
            } catch {
                history = new OrderHistory {
                    OrderStatus = new OrderStatus {
                        status = "Unknown"
                    },
                };
            }
            return history;
        }

        public int GetStatusID() {
            int statusID = 0;
            try {
                statusID = this.history.OrderByDescending(x => x.dateAdded).First().statusID;
            } catch {}
            return statusID;
        }

        public string GetCurtStatus() {
            if (this.edi == null) {
                return "Not Sent Yet";
            } else if (this.edi.dateAcknowledged == null) {
                return "Awaiting Response";
            } else {
                return "Received";
            }
        }

        internal int Count() {
            int orders = 0;
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            orders = db.Carts.Where(x => x.payment_id > 0).Count();
            return orders;
        }

        public static string Search(string searchtext = "") {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Orders> orders = new List<Orders>();
            if (searchtext.Trim() != "") {
                orders = (from c in db.Carts
                          where c.payment_id > 0 && c.payment_id.ToString().Contains(searchtext)
                          select new Orders {
                              orderID = c.payment_id,
                              customerName = c.Customer.fname + " " + c.Customer.lname,
                              itemcount = c.CartItems.Count,
                              orderDate = c.Payment.created,
                              PaymentMethod = c.Payment.PaymentTypes.name,
                              history = c.OrderHistories.ToList(),
                              total = c.getTotal(),
                              edi = c.OrderEDI
                          }).ToList<Orders>();
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(orders);
        }

    }

}