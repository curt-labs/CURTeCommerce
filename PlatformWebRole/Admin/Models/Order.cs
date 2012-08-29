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
        
    }

}