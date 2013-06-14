using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommercePlatform.Models;
using System.Text;
using System.Reflection;
using System.Data.Linq;

namespace EcommercePlatform {
    partial class OrderHistory {

        public void Save() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            db.OrderHistories.InsertOnSubmit(this);
            db.SubmitChanges();
        }

    }

}