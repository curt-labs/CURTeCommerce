using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Norm;
using Newtonsoft.Json;

namespace EcommercePlatform.Models {
    public class LocationModel {

        public static List<PrettyLocation> GetAll() {
            List<PrettyLocation> locs = new List<PrettyLocation>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            locs = (from l in db.Locations
                    join s in db.States on l.stateID equals s.stateID
                    select new PrettyLocation {
                        locationID = l.locationID,
                        name = l.name,
                        phone = l.phone,
                        fax = l.fax,
                        email = l.email,
                        address = l.address,
                        city = l.city,
                        stateID = l.stateID,
                        zip = l.zip,
                        isPrimary = l.isPrimary,
                        latitude = l.latitude,
                        longitude = l.longitude,
                        places_id = l.places_id,
                        places_reference = l.places_reference,
                        places_status = l.places_status,
                        foursquare_id = l.foursquare_id,
                        abbr = s.abbr,
                        state = s.state1,
                        Services = (from serv in db.Services
                                    join ls in db.LocationServices on serv.ID equals ls.serviceID
                                    where ls.locationID.Equals(l.locationID)
                                    select serv).ToList<Service>()
                    }).OrderBy(x => x.locationID).ToList<PrettyLocation>();
            return locs;
        }

        public static List<Location> GetNearest(float lat, float lon) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                string query = String.Format(@"SELECT * FROM Locations ORDER BY ( 3959 * acos( cos( radians({0}) ) * cos( radians( latitude ) ) * cos( radians( longitude ) - radians({1}) ) + sin( radians({0}) ) * sin( radians( latitude ) ) ) )", lat, lon);
                List<Location> loc = db.ExecuteQuery<Location>(query).ToList<Location>();

                return loc;
            } catch (Exception e) {
                throw e;
            }
        }
    }

    public class PrettyLocation : Location {
        public string state { get; set; }
        public string abbr { get; set; }

        [JsonIgnore]
        public List<Service> Services { get; set; }
    }
}