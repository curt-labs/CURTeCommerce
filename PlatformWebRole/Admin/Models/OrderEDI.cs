using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using System.Data.Linq;

namespace Admin {
    partial class OrderEDI {

        public OrderEDI Get(int id) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            OrderEDI edi = db.OrderEDIs.Where(x => x.ID.Equals(id)).FirstOrDefault();
            return edi;
        }

        public OrderEDI GetByOrderID(int id) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            OrderEDI edi = db.OrderEDIs.Where(x => x.orderID.Equals(id)).FirstOrDefault();
            return edi;
        }

        public void Save() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            OrderEDI edi = db.OrderEDIs.Where(x => x.orderID.Equals(this.orderID)).FirstOrDefault();
            if (edi != null && edi.ID > 0) {
                edi.filename = this.filename;
                edi.editext = this.editext;
                edi.dateGenerated = DateTime.UtcNow;
            } else {
                edi = new OrderEDI {
                    orderID = this.orderID,
                    filename = this.filename,
                    editext = this.editext,
                    dateGenerated = DateTime.UtcNow
                    dateAcknowledged = DateTime.UtcNow
                };
                db.OrderEDIs.InsertOnSubmit(edi);
            }
            db.SubmitChanges();
        }

    }

}