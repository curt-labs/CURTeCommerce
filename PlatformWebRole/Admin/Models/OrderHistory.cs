using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Admin.Models;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.Linq;
using System.Transactions;

namespace Admin {
    partial class OrderHistory {

        public void Save() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            db.OrderHistories.InsertOnSubmit(this);
            db.SubmitChanges();
        }

    }

    public class OrderHistoryJSON : OrderHistory {
        public string added { get; set; }
    }

}
