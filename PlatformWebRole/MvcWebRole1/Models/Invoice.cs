using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommercePlatform.Models;
using System.Text;
using System.Reflection;
using System.Data.Linq;

namespace EcommercePlatform {
    partial class Invoice {

        public void Save(List<InvoiceItem> items, List<InvoiceCode> codes) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            this.created = DateTime.Now;
            this.paid = false;
            try {
                Invoice i = db.Invoices.Where(x => x.curtOrder.Equals(this.curtOrder)).First<Invoice>();
            } catch {
                db.Invoices.InsertOnSubmit(this);
                db.SubmitChanges();

                foreach (InvoiceItem itm in items) {
                    itm.invoiceID = this.id;
                }
                db.InvoiceItems.InsertAllOnSubmit(items);

                foreach (InvoiceCode code in codes) {
                    code.invoiceID = this.id;
                }
                db.InvoiceCodes.InsertAllOnSubmit(codes);
                db.SubmitChanges();
            }

        }

    }

}