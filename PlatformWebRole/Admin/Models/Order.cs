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
        public string status { get; set; }
        public decimal total { get; set; }

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
                          status = ((c.voided) ? "Void" : c.Payment.status),
                          total = c.getTotal()
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
                          status = ((c.voided) ? "Void" : c.Payment.status),
                          total = c.getTotal()
                      }).Skip(perpage * (page - 1)).Take(perpage).ToList<Orders>();

            return orders;
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
                              status = ((c.voided) ? "Void" : c.Payment.status),
                              total = c.getTotal()
                          }).ToList<Orders>();
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(orders);
        }
    }

}