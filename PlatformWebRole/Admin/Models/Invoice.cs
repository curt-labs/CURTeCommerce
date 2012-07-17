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
    partial class Invoice {

        internal Invoice Get(int id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Invoice i = db.Invoices.Where(x => x.id == id).FirstOrDefault<Invoice>();
            return i;
        }

        internal List<Invoice> GetAll() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.Invoices.OrderByDescending(x => x.dateAdded).ToList<Invoice>();
            } catch (Exception) {
                return new List<Invoice>();
            }
        }

        internal void Paid() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Invoice i = db.Invoices.Where(x => x.id == this.id).FirstOrDefault<Invoice>();
            if (i.paid) {
                i.paid = false;
            } else {
                i.paid = true;
            }
            db.SubmitChanges();
        }
    }
}
