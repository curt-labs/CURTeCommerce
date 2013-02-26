using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models {
    public class DC {

        internal static List<DistributionCenter> GetAll() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<DistributionCenter> dcList = new List<DistributionCenter>();

                dcList = db.DistributionCenters.OrderBy(x => x.Name).ToList<DistributionCenter>();
                return dcList;
            } catch (Exception) {
                return new List<DistributionCenter>();
            }
        }

        internal static DistributionCenter Get(int id) {
            try {
                DistributionCenter dc = new DistributionCenter();
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();

                dc = db.DistributionCenters.Where(x => x.ID.Equals(id)).FirstOrDefault<DistributionCenter>();

                return dc;
            } catch (Exception) {
                return new DistributionCenter();
            }
        }

        internal static void Save(int id, string Name, string Phone, string Fax, string Street1, string Street2, string City, int State, string PostalCode, out DistributionCenter dc, out List<string> errors) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            dc = new DistributionCenter();
            errors = new List<string>();

            if (id != 0) {
                dc = db.DistributionCenters.Where(x => x.ID.Equals(id)).FirstOrDefault<DistributionCenter>();
            }
            try {
                // Validate the fields
                if (Name.Length == 0) { throw new Exception(); } else { dc.Name = Name; }
                if (Phone.Length == 0) { throw new Exception(); } else { dc.Phone = Phone; }
                if (Street1.Length == 0) { throw new Exception(); } else { dc.Street1 = Street1; }
                if (City.Length == 0) { throw new Exception(); } else { dc.City = City; }
                if (State == 0) { throw new Exception(); } else { dc.State = State; }
                if (PostalCode.Length == 0) { throw new Exception(); } else { dc.PostalCode = PostalCode; }
                dc.Fax = Fax;
                dc.Street2 = Street2;

                // Geocode the DC
                GeocodingResponse geo = Geocoding.GetGeoLocation(dc.City, 0, dc.PostalCode, dc.State1.Country.abbr, dc.State1.abbr);
                LatitudeLongitude latLon = new LatitudeLongitude();
                if (geo.results.Count > 0) {
                    latLon = geo.results[0].geometry.location;
                    dc.Latitude = latLon.lat;
                    dc.Longitude = latLon.lng;
                } else {
                    errors.Add("Failed to retrieve geographical location.");
                }

                if (errors.Count == 0) {
                    if (id == 0) {
                        db.DistributionCenters.InsertOnSubmit(dc);
                    }
                    db.SubmitChanges();
                }
            } catch (Exception e) {
                errors.Add(e.Message);
            }
        }

        internal static void Delete(int id) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            DistributionCenter dc = new DistributionCenter();
            dc = db.DistributionCenters.Where(x => x.ID.Equals(id)).FirstOrDefault<DistributionCenter>();

            db.DistributionCenters.DeleteOnSubmit(dc);
            db.SubmitChanges();
        }
    }
}