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
        internal Cart order { get; set; }

        internal Invoice Get(int id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Invoice i = db.Invoices.Where(x => x.id == id).FirstOrDefault<Invoice>();
            return i;
        }

        internal IOrderedQueryable<InvoiceCount> GetAll() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.Invoices.GroupBy(dates => dates.created.Date, invoices => invoices.created, (dates, invoices) => new InvoiceCount { invoiceDate = dates, invoiceCount = invoices.Count() }).OrderByDescending(x => x.invoiceDate);
            } catch (Exception) {
                return null;
            }
        }

        internal List<Invoice> GetAllByDate(DateTime date) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.Invoices.Where(x => x.created.Date.Equals(date.Date)).OrderBy(x => x.number).ToList<Invoice>();
            } catch (Exception) {
                return new List<Invoice>();
            }
        }

        internal void LoadOrder() {
            try {
                this.order = new Cart().GetByPayment(Convert.ToInt32(this.orderID));
            } catch {
                this.order = null;
            }
        }

        internal void Print() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Invoice i = db.Invoices.Where(x => x.id == this.id).FirstOrDefault<Invoice>();
            i.printed = true;
            db.SubmitChanges();
        }

        internal List<Invoice> GetAllUnprinted() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.Invoices.Where(x => !x.printed).ToList<Invoice>();
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
