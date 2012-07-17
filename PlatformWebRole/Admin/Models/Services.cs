using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models {
    public class ServicesModel {

        internal static List<Service> GetAll() {
            try {
                List<Service> services = new List<Service>();
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();

                services = db.Services.OrderBy(x => x.title).ToList<Service>();

                return services;
            } catch (Exception) {
                return new List<Service>();
            }
        }

        internal static Service Get(int id) {
            try {
                Service s = new Service();
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                s = db.Services.Where(x => x.ID.Equals(id)).FirstOrDefault<Service>();
                return s;
            } catch (Exception) {
                return new Service();
            }
        }

        internal static List<Location> GetLocations(int id) {
            try {
                List<Location> locs = new List<Location>();
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();

                locs = (from l in db.Locations
                        join ls in db.LocationServices on l.locationID equals ls.locationID
                        where ls.serviceID.Equals(id)
                        select l).ToList<Location>();
                return locs;
            } catch (Exception) {
                return new List<Location>();
            }
        }

        internal static void Save(int id, string title, string description, decimal price, string hourly, out Service service){
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            service = new Service();

            if(id != 0){
                service = db.Services.Where(x => x.ID.Equals(id)).FirstOrDefault<Service>();
            }

            // Validate the fields
            if(title.Length == 0){ throw new Exception("Title is required");}else{ service.title = title; }
            if(description.Length == 0){ throw new Exception("Description is required");}else{ service.description = description; }
            if(price == 0){ throw new Exception("Price must be greater than zero");}else{ service.price = price; }
            service.hourly = hourly;

            if(id == 0){
                db.Services.InsertOnSubmit(service);
            }
            db.SubmitChanges();
        }

        internal static void Delete(int id) {
            if (id == 0) { throw new Exception("Invalid reference."); }

            EcommercePlatformDataContext db = new EcommercePlatformDataContext();

            // Get all the LocationServices for the Service
            List<LocationService> ls = new List<LocationService>();
            ls = db.LocationServices.Where(x => x.serviceID.Equals(id)).ToList<LocationService>();
            db.LocationServices.DeleteAllOnSubmit<LocationService>(ls);

            // Get the Service and remove it
            Service serv = new Service();
            serv = db.Services.Where(x => x.ID.Equals(id)).FirstOrDefault<Service>();
            db.Services.DeleteOnSubmit(serv);

            // Commit
            db.SubmitChanges();
        }
        
        internal static void AddLocation(int serviceID, int locationID) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            
            // Make sure they aren't already matched up
            LocationService ls = new LocationService();
            ls = db.LocationServices.Where(x => x.serviceID.Equals(serviceID) && x.locationID.Equals(locationID)).FirstOrDefault<LocationService>();
            if (ls != null && ls.locationID != 0) { throw new Exception("Record already exists."); }

            ls = new LocationService {
                serviceID = serviceID,
                locationID = locationID
            };
            db.LocationServices.InsertOnSubmit(ls);
            db.SubmitChanges();
        }

        internal static void RemoveLocation(int serviceID, int locationID) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();

            LocationService ls = new LocationService();
            ls = db.LocationServices.Where(x => x.serviceID.Equals(serviceID) && x.locationID.Equals(locationID)).FirstOrDefault<LocationService>();

            db.LocationServices.DeleteOnSubmit(ls);
            db.SubmitChanges();
        }

    }
}