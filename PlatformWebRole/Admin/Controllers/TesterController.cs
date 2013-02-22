using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Admin.Controllers
{
    public class TesterController : Controller
    {
        //
        // GET: /Tester/

        public string Index()
        {
            List<Location> locs = new List<Location>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            locs = db.Locations.OrderBy(x => x.locationID).ToList<Location>();
            foreach (Location loc in locs) {
                Response.Write(loc.name + "\r\n");
            }

            return "";
        }

        public string ListBlobs() {
            try {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));


                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer con = blobClient.GetContainerReference("profile-pictures");

                foreach (ICloudBlob blob in con.ListBlobs()) {
                    Response.Write(blob.Name);
                }

                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }

    }
}
