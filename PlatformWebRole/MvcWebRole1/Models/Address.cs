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
            this.first = first;
            this.last = last;
            this.street1 = street1;
            this.street2 = street2;
            this.city = city;
            this.state = state_id;
            this.postal_code = zip;
            this.residential = residential;
            db.SubmitChanges();
        }

        internal Address Get(int id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Address a = db.Addresses.Where(x => x.ID == id).Where(x => x.active == true).First<Address>();
            return a;
        }

        internal void MatchOrSave() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            try {
                Address address = (from a in db.Addresses
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
                this.residential = true;
                this.cust_id = 0;
                db.Addresses.InsertOnSubmit(this);
                db.SubmitChanges();
            }
        }

        internal void Delete(int id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Address a = db.Addresses.Where(x => x.ID == id).Where(x => x.active == true).First<Address>();
            a.active = false;
            db.SubmitChanges();
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

        internal ShippingAddress getShipping() {
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
            GeocodingResponse geo = Geocoding.GetGeoLocation(this.street1 + " " + this.street2, this.city, 0, this.postal_code, this.State1.Country.abbr, this.State1.abbr);
            LatitudeLongitude latLon = new LatitudeLongitude();
            if (geo.results.Count > 0) {
                latLon = geo.results[0].geometry.location;
                this.latitude = latLon.lat;
                this.longitude = latLon.lng;
            }
            return this;
        }

    }
}
