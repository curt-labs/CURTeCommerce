using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models {
    public class Locations : Location {

        internal static List<Location> GetAll() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<Location> locs = new List<Location>();

                locs = db.Locations.OrderBy(x => x.name).ToList<Location>();
                return locs;
            } catch (Exception) {
                return new List<Location>();
            }
        }

        internal static Location Get(int id) {
            try {
                Location loc = new Location();
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();

                loc = db.Locations.Where(x => x.locationID.Equals(id)).FirstOrDefault<Location>();
                return loc;
            } catch (Exception) {
                return new Location();
            }
        }

        internal static void Save(int id, string name, string phone, string fax, string email, string address, string city, int stateID, string zip, int isPrimary, int google_places, int foursquare, out Location loc, out List<string> errors) {

            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Dictionary<List<string>, Location> response = new Dictionary<List<string>, Location>();
            loc = new Location();
            errors = new List<string>();

            if (id != 0) {
                loc = db.Locations.Where(x => x.locationID.Equals(id)).FirstOrDefault<Location>();
            }

            // Validate the fields
            if (name.Length == 0) { throw new Exception(); } else { loc.name = name; }
            if (phone.Length == 0) { throw new Exception(); } else { loc.phone = phone; }
            if (email.Length == 0) { throw new Exception(); } else { loc.email = email; }
            if (address.Length == 0) { throw new Exception(); } else { loc.address = address; }
            if (city.Length == 0) { throw new Exception(); } else { loc.city = city; }
            if (stateID == 0) { throw new Exception(); } else { loc.stateID = stateID; }
            if (zip == "") { throw new Exception(); } else { loc.zip = zip; }
            loc.isPrimary = isPrimary;
            loc.fax = fax;

            GeocodingResponse geo = Geocoding.GetGeoLocation(loc.address, loc.city, stateID, zip.ToString());
            LatitudeLongitude lat_lon = new LatitudeLongitude();
            if (geo.results.Count > 0) {
                lat_lon = geo.results[0].geometry.location;
                loc.latitude = lat_lon.lat;
                loc.longitude = lat_lon.lng;
            } else {
                errors.Add("Failed to retrive geographical location.");
            }

            if (google_places == 1) {
                try {
                    string form_types = HttpContext.Current.Request.Form["place_types"];
                    List<string> place_types = new List<string>();
                    if (form_types != null) {
                        if (form_types.Contains(',')) {
                            place_types = form_types.Split(',').ToList<string>();
                        } else {
                            place_types.Add(form_types);
                        }
                    }

                    LatitudeLongitude place_latlng = new LatitudeLongitude {
                        lat = loc.latitude,
                        lng = loc.longitude
                    };

                    NewPlace place = new NewPlace {
                        location = place_latlng,
                        accuracy = 50,
                        name = loc.name,
                        types = place_types,
                        language = "en"
                    };

                    var returned_place = Geocoding.AddPlace(place);
                    try {
                        AddPlaceResponse new_place = new AddPlaceResponse();
                        try {
                            new_place = (AddPlaceResponse)returned_place;
                        } catch (Exception) {
                            throw new Exception(returned_place);
                        }

                        if (new_place.status != "OK") {
                            throw new Exception("Google Places request was denied");
                        }

                        loc.places_status = new_place.status;
                        loc.places_reference = new_place.reference;
                        loc.places_id = new_place.id;

                    } catch (Exception e) {
                        errors.Add(e.Message);
                    }
                } catch (Exception) {
                    errors.Add("Failed to submit Google Places listing");
                }
            }
            if (errors.Count == 0) {
                if (id == 0) {
                    db.Locations.InsertOnSubmit(loc);
                }
                db.SubmitChanges();
            }
            //response.Add(errors, loc);
            //return response;
            
        }

        internal static void Delete(int id) {
            Location loc = new Location();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();

            loc = db.Locations.Where(x => x.locationID.Equals(id)).FirstOrDefault<Location>(); // Get the Location record
            string places_reference = loc.places_reference;

            db.Locations.DeleteOnSubmit(loc); // Delete it
            db.SubmitChanges(); // Commit

            // Check if it has a Google Places refernce
            if (places_reference != null && places_reference.Length > 0) {
                Geocoding.DeletePlaceEntry(places_reference);
            }
        }
    }

    public class DisplayLocation : Location {
        public string state { get; set; }
        public string state_abbr { get; set; }
    }
}