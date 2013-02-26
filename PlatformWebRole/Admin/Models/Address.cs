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
    partial class Address {

        public decimal? latitude { get; set; }
        public decimal? longitude { get; set; }

        internal void Save(int cust_id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();

            // We are going to make an attempt at saving the Address record

            this.cust_id = cust_id;

            // First let's Sanitize our Addresses
            UDF.Sanitize(this,new string[]{"street2", "state1","latitude","longitude"});

            Address new_address = this;

            db.Addresses.InsertOnSubmit(new_address);
            db.SubmitChanges();
            this.ID = new_address.ID;

        }

        internal void Update(string first, string last, string street1, string street2, string city, int state_id, string zip, bool residential = false) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Address a = db.Addresses.Where(x => x.ID.Equals(this.ID)).FirstOrDefault();
            if (a != null && a.ID > 0) {
                a.first = first;
                a.last = last;
                a.street1 = street1;
                a.street2 = street2;
                a.city = city;
                a.state = state_id;
                a.postal_code = zip;
                a.residential = residential;
                db.SubmitChanges();
            }
        }

        internal Address Get(int id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Address a = db.Addresses.Where(x => x.ID == id).Where(x => x.active == true).First<Address>();
            return a;
        }

        internal bool Delete(int id = 0) {
            bool success = false;
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            using (TransactionScope ts = new TransactionScope()) {
                try {
                    Address a = db.Addresses.Where(x => x.ID == id).Where(x => x.active == true).First<Address>();
                    a.active = false;
                    db.SubmitChanges();
                    success = true;
                    ts.Complete();
                } catch { }
            }
            return success;
        }

        internal bool Equals(Address address) {
            bool isequal = false;
            isequal = (
                    this.first.ToLower().Trim() == address.first.ToLower().Trim() &&
                    this.last.ToLower().Trim() == address.last.ToLower().Trim() &&
                    this.street1.ToLower().Trim() == address.street1.ToLower().Trim() &&
                    this.street2.ToLower().Trim() == address.street2.ToLower().Trim() &&
                    this.city.ToLower().Trim() == address.city.ToLower().Trim() &&
                    this.state == address.state &&
                    this.postal_code.ToLower().Trim() == address.postal_code.ToLower().Trim()) &&
                    this.residential == address.residential;
            return isequal;
        }

        internal  ShippingAddress getShipping() {
            ShippingAddress address = new ShippingAddress {
                StreetLines = new string[] { this.street1, this.street2 },
                City = this.city,
                StateOrProvinceCode = this.State1.abbr,
                CountryCode = this.State1.Country.abbr,
                PostalCode = this.postal_code,
                Residential = this.residential,
                ResidentialSpecified = true,
                UrbanizationCode = null
            };
            return address;
        }

        internal Address GeoLocate() {
            GeocodingResponse geo = Geocoding.GetGeoLocation(this.city, 0, this.postal_code, this.State1.Country.abbr, this.State1.abbr);
            LatitudeLongitude latLon = new LatitudeLongitude();
            if (geo.results.Count > 0) {
                latLon = geo.results[0].geometry.location;
                this.latitude = latLon.lat;
                this.longitude = latLon.lng;
            }
            return this;
        }

        internal bool isPOBox() {
            try {
                if (this.street1.Contains("PO ") ||
                    this.street1.Contains("P.O. ") ||
                    this.street2.Contains("PO ") ||
                    this.street2.Contains("P.O. ")) {
                    return true;
                }
            } catch { }
            return false;
        }
    }
}
