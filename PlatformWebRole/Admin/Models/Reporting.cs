using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models {
    public class Reporting {
        public static List<Cart> GetOrdersByDateRange(DateTime start, DateTime end) {
            List<Cart> orders = new List<Cart>();
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                orders = db.Carts.Where(x => x.Payment.created >= start.ToUniversalTime()).Where(x => x.Payment.created <= end.ToUniversalTime()).Where(x => x.voided != true).ToList<Cart>();
            } catch { };

            return orders;
        }

        public static Invoice GetInvoiceByID(string invoiceID) {
            Invoice invoice = new Invoice();
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                invoice = db.Invoices.Where(x => x.number.Equals(invoiceID)).FirstOrDefault<Invoice>();
            } catch { };

            return invoice;
        }

        public static List<Invoice> GetInvoicesByDateRange(DateTime start, DateTime end) {
            List<Invoice> invoices = new List<Invoice>();
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                invoices = db.Invoices.Where(x => x.created > start && x.created <= end).ToList<Invoice>();
            } catch { };

            return invoices;
        }

    }
}