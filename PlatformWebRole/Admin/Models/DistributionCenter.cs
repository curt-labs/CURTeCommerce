using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Admin.Models;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.Linq;

namespace Admin {
    partial class DistributionCenter {

        public Address getAddress() {
            Address address = new Address { 
                first = this.Name,
                street1 = this.Street1,
                street2 = this.Street2,
                city = this.City,
                state = this.State,
                State1 = this.State1,
                postal_code = this.PostalCode,
                residential = false,
                latitude = this.Latitude,
                longitude = this.Longitude
            };
            return address;
        }

        public DistributionCenter GetNearest(Address address) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            DistributionCenter distcenter = new DistributionCenter();
            try {
                double earth = 3963.1676; // radius of Earth in miles
                DCList d = (from dc in db.DistributionCenters
                            where dc.State1.countryID == address.State1.countryID
                            select new DCList {
                                ID = dc.ID,
                                distance = earth * (
                                                    2 * Math.Atan2(
                                                        Math.Sqrt((Math.Sin((Convert.ToDouble(dc.Latitude - address.latitude) * (Math.PI / 180)) / 2) * Math.Sin((Convert.ToDouble(dc.Latitude - address.latitude) * (Math.PI / 180)) / 2)) + ((Math.Sin((Convert.ToDouble(dc.Longitude - address.longitude) * (Math.PI / 180)) / 2)) * (Math.Sin((Convert.ToDouble(dc.Longitude - address.longitude) * (Math.PI / 180)) / 2))) * Math.Cos(Convert.ToDouble(address.latitude) * (Math.PI / 180)) * Math.Cos(Convert.ToDouble(dc.Latitude) * (Math.PI / 180))),
                                                        Math.Sqrt(1 - ((Math.Sin((Convert.ToDouble(dc.Latitude - address.latitude) * (Math.PI / 180)) / 2) * Math.Sin((Convert.ToDouble(dc.Latitude - address.latitude) * (Math.PI / 180)) / 2)) + ((Math.Sin((Convert.ToDouble(dc.Longitude - address.longitude) * (Math.PI / 180)) / 2)) * (Math.Sin((Convert.ToDouble(dc.Longitude - address.longitude) * (Math.PI / 180)) / 2))) * Math.Cos(Convert.ToDouble(address.latitude) * (Math.PI / 180)) * Math.Cos(Convert.ToDouble(dc.Latitude) * (Math.PI / 180))))
                                                    )
                                                )
                            }).OrderBy(x => x.distance).First<DCList>();

                distcenter = db.DistributionCenters.Where(x => x.ID == d.ID).First<DistributionCenter>();
            } catch {};
            return distcenter;
        }

    }
}
