using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models {
    public class AdminCustomer {
        public int ID { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int ordercount { get; set; }
        public DateTime created { get; set; }
        public string status { get; set; }

        public static string Search(string searchtext = "") {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<AdminCustomer> custs = new List<AdminCustomer>();
            if (searchtext.Trim() != "") {
                if (searchtext.Contains("@")) {
                    // email address
                    custs = (from c in db.Customers
                             where c.email.Contains(searchtext)
                             select new AdminCustomer {
                                 ID = c.ID,
                                 fname = c.fname,
                                 lname = c.lname,
                                 phone = c.phone,
                                 email = c.email,
                                 created = c.dateAdded,
                                 status = (c.isSuspended == 1) ? "Suspended" : "Active",
                                 ordercount = c.Carts.Where(x => x.payment_id > 0).Count()
                             }).ToList<AdminCustomer>();
                } else if (searchtext.Contains(" ")) {
                    // first and last
                    string[] names = searchtext.Split(' ');
                    custs = (from c in db.Customers
                             where c.fname.Contains(names[0]) && c.lname.Contains(names[1])
                             select new AdminCustomer {
                                 ID = c.ID,
                                 fname = c.fname,
                                 lname = c.lname,
                                 phone = c.phone,
                                 email = c.email,
                                 created = c.dateAdded,
                                 status = (c.isSuspended == 1) ? "Suspended" : "Active",
                                 ordercount = c.Carts.Where(x => x.payment_id > 0).Count()
                             }).ToList<AdminCustomer>();
                } else {
                    // search all
                    custs = (from c in db.Customers
                             where c.fname.Contains(searchtext) || c.lname.Contains(searchtext) || c.email.Contains(searchtext)
                             select new AdminCustomer {
                                 ID = c.ID,
                                 fname = c.fname,
                                 lname = c.lname,
                                 phone = c.phone,
                                 email = c.email,
                                 created = c.dateAdded,
                                 status = (c.isSuspended == 1) ? "Suspended" : "Active",
                                 ordercount = c.Carts.Where(x => x.payment_id > 0).Count()
                             }).ToList<AdminCustomer>();
                }
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(custs);
        }
    }
}