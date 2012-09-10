using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommercePlatform.Models;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.Linq;

namespace EcommercePlatform {
    partial class InvoiceAddress {

        internal void MatchOrSave() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            try {
                InvoiceAddress address = (from a in db.InvoiceAddresses
                                           where a.first.ToLower().Trim().Equals(this.first.ToLower().Trim()) &&
                                           a.last.ToLower().Trim().Equals(this.last.ToLower().Trim()) &&
                                           a.street1.ToLower().Trim().Equals(this.street1.ToLower().Trim()) &&
                                           a.street2.ToLower().Trim().Equals(this.street2.ToLower().Trim()) &&
                                           a.city.ToLower().Trim().Equals(this.city.ToLower().Trim()) &&
                                           a.state.Equals(this.state) &&
                                           a.postal_code.ToLower().Trim().Equals(this.postal_code.ToLower().Trim())
                                           select a).First();
                this.ID = address.ID;
            } catch {
                try {
                    this.country = (from c in db.Countries
                                    join st in db.States on c.ID equals st.countryID
                                    where st.abbr.Equals(this.state.ToUpper().Trim())
                                    select c.name).First();
                } catch { }
                db.InvoiceAddresses.InsertOnSubmit(this);
                db.SubmitChanges();
            }
        }


        internal bool Equals(InvoiceAddress address) {
            bool isequal = false;
            isequal = (
                    this.first.ToLower().Trim() == address.first.ToLower().Trim() &&
                    this.last.ToLower().Trim() == address.last.ToLower().Trim() &&
                    this.street1.ToLower().Trim() == address.street1.ToLower().Trim() &&
                    this.street2.ToLower().Trim() == address.street2.ToLower().Trim() &&
                    this.city.ToLower().Trim() == address.city.ToLower().Trim() &&
                    this.state == address.state &&
                    this.postal_code.ToLower().Trim() == address.postal_code.ToLower().Trim());
            return isequal;
        }

    }
}
