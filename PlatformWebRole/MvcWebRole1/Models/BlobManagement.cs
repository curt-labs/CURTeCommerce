﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure;
using System.IO;
using System.Drawing;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EcommercePlatform.Models {
    public class BlobManagement {

        internal static DiscountBlobContainer CreateContainer(string name = "", string parent = "", bool make_public = true) {

            // Build out a relationship between the new container name and parent (if parent is provided)
            string conName = name.ToLower().Replace(' ','-');

            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            // Create the blobl client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            
            CloudBlobContainer con = null;
            if (parent.Length > 0) {
                //conName = parent + "/" + name;
                CloudBlobContainer parentContainer = client.GetContainerReference(parent);
                CloudBlobDirectory subCon = parentContainer.GetDirectoryReference(name);
                subCon.Container.CreateIfNotExists();
                con = subCon.Container;
            }else{
                // Retrieve a reference to a container
                con = client.GetContainerReference(conName);

                // Create the container if it doesn't already exist
                con.CreateIfNotExists();
            }

            if (make_public) { // Make access to this container public
                con.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            // Cast to our object
            DiscountBlobContainer container = new DiscountBlobContainer {
                Container = client.GetContainerReference(conName),
                BlobCount = 0,
                SubContainers = GetSubContainers(conName)
            };
            
            return container;
        }

        internal static DiscountBlobContainer GetContainer(string name) {
            if (name == null || name.Length == 0) {
                return new DiscountBlobContainer { 
                    Container = null,
                    BlobCount = 0
                };
            }
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            // Create the blobl client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a previously created container
            CloudBlobContainer con = client.GetContainerReference(name);
            con.CreateIfNotExists();
            con.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });

            DiscountBlobContainer container = new DiscountBlobContainer {
                Container = con,
                BlobCount = GetBlobs(con.Name).Count,
                SubContainers = GetSubContainers(con.Name)
            };
            return container;
        }

        internal static CloudBlobDirectory GetDirectory(string parent, string name) {
            
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            // Create the blobl client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer parentCont = client.GetContainerReference(parent);
            CloudBlobDirectory subCon = parentCont.GetDirectoryReference(name);

            return subCon;
        }

        internal static DiscountBlobContainer RenameContainer(string old_name, string new_name) {
            if (old_name == null || old_name.Length == 0) {
                throw new Exception("Invalid reference to existing name.");
            }
            if (new_name == null || new_name.Length == 0) {
                throw new Exception("No new name specified.");
            }

            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            // Create blob client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a previously created container
            CloudBlobContainer oldContainer = client.GetContainerReference(old_name);

            // Create and retrieve reference to new container
            CloudBlobContainer newContainer = client.GetContainerReference(new_name);
            newContainer.CreateIfNotExists();
            newContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            foreach (var blob in oldContainer.ListBlobs()) {

                if (blob.GetType() == typeof(CloudBlobDirectory)) {
                    CloudBlobDirectory oldDir = (CloudBlobDirectory)blob;

                    // Get the name of the directory
                    string dirName = oldDir.Container.Name;
                } else if (blob.GetType() == typeof(CloudPageBlob)) {
                    CloudPageBlob oldBlob = (CloudPageBlob)blob;

                    // Get the filename of the existing blob
                    string filename = Path.GetFileName(blob.Uri.ToString());

                    // Create blob reference for the new container using the existing blob's filename
                    CloudPageBlob newBlob = newContainer.GetPageBlobReference(filename);

                    // Copy old blob to new blob
                    newBlob.StartCopyFromBlob(oldBlob);
                    // Delete old Blob
                    oldBlob.DeleteIfExists();

                } else {

                    CloudBlockBlob oldBlob = (CloudBlockBlob)blob;

                    // Get the filename of the existing blob
                    string filename = Path.GetFileName(blob.Uri.ToString());

                    // Create blob reference for the new container using the existing blob's filename
                    CloudBlockBlob newBlob = newContainer.GetBlockBlobReference(filename);

                    // Copy old blob to new blob
                    newBlob.StartCopyFromBlob(oldBlob);

                    // Delete old Blob
                    oldBlob.DeleteIfExists();
                }
            }

            // Delete old container
            oldContainer.Delete();

            DiscountBlobContainer con = new DiscountBlobContainer {
                Container = newContainer,
                BlobCount = newContainer.ListBlobs().Count()
            };
            return con;
        }

        internal static void DeleteContainer(string name) {
            if (name == null || name.Length == 0) {
                throw new Exception("Invalid container name.");
            }

            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            // Create the blob client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a previously created container
            CloudBlobContainer container = client.GetContainerReference(name);

            container.Delete();
        }

        internal static List<DiscountBlobContainer> GetContainers() {
            try {
                // Retrieve storage account from connection string
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

                // Create the blobl client
                CloudBlobClient client = storageAccount.CreateCloudBlobClient();

                List<CloudBlobContainer> cloudContainers = new List<CloudBlobContainer>();
                List<DiscountBlobContainer> containers = new List<DiscountBlobContainer>();
                cloudContainers = client.ListContainers().ToList<CloudBlobContainer>();

                foreach (CloudBlobContainer con in cloudContainers) {
                    DiscountBlobContainer discountContainer = new DiscountBlobContainer { 
                        BlobCount = GetBlobs(con.Name).Count,
                        Container = con,
                        SubContainers = GetSubContainers(con.Name)
                    };
                    containers.Add(discountContainer);
                }

                return containers;
            } catch (Exception) {
                return new List<DiscountBlobContainer>();
            }
        }

        internal static List<DiscountBlobContainer> GetSubContainers(string parent = "") {
            try {
                // Retrieve storage account from connection string
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

                // Create the blobl client
                CloudBlobClient client = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = client.GetContainerReference(parent);
                List<DiscountBlobContainer> subs = new List<DiscountBlobContainer>();

                foreach (CloudBlobDirectory sub_dir in container.ListBlobs().OfType<CloudBlobDirectory>()) {
                    DiscountBlobContainer sub = new DiscountBlobContainer {
                        BlobCount = sub_dir.ListBlobs().Count(),
                        Container = sub_dir.Container
                    };
                    subs.Add(sub);
                }

                return subs;
            } catch (Exception) {
                return new List<DiscountBlobContainer>();
            }
        }

        internal static List<IListBlobItem> GetBlobs(string containerName = "") {
            if (containerName.Length == 0) { return new List<IListBlobItem>(); }

            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            // Create the blobl client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a previously created container
            CloudBlobContainer container = client.GetContainerReference(containerName);

            List<IListBlobItem> blobs = new List<IListBlobItem>();
            blobs = container.ListBlobs().ToList<IListBlobItem>();

            return blobs;
        }

        internal static void MoveBlob(IListBlobItem blob, string topath = "", string currentpath = "") {
            // Retrieve stroage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            // Create the blob client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            CloudBlobDirectory toFolder = null;
            CloudBlobContainer parentContainer = null;
            string filepath = "";
            if (blob.GetType() == typeof(CloudPageBlob)) {
                CloudPageBlob bblob = (CloudPageBlob)blob;
                filepath = bblob.Name.Substring(bblob.Name.IndexOf("/") + 1);
            } else {
                CloudBlockBlob bblob = (CloudBlockBlob)blob;
                filepath = bblob.Name.Substring(bblob.Name.IndexOf("/") + 1);
            }
            if (topath.Contains('/')) {
                List<string> paths = topath.Split('/').ToList();
                string parent = paths.FirstOrDefault();
                paths.RemoveAt(0);
                string folder = string.Join("/", paths.ToArray());

                parentContainer = client.GetContainerReference(parent);
                toFolder = parentContainer.GetDirectoryReference(folder);
                if(blob.GetType() == typeof(CloudPageBlob)) {
                    CloudPageBlob pblob = (CloudPageBlob)blob;
                    CloudPageBlob newblob = toFolder.GetPageBlobReference(filepath);
                    newblob.StartCopyFromBlob(pblob);
                    pblob.DeleteIfExists();

                } else {
                    CloudBlockBlob bblob = (CloudBlockBlob)blob;
                    CloudBlockBlob newblob = toFolder.GetBlockBlobReference(filepath);
                    newblob.StartCopyFromBlob(bblob);
                    bblob.DeleteIfExists();
                }
            } else {
                parentContainer = client.GetContainerReference(topath);
                if (blob.GetType() == typeof(CloudPageBlob)) {
                    CloudPageBlob pblob = (CloudPageBlob)blob;
                    filepath = pblob.Name.Substring(pblob.Name.IndexOf("/") + 1);
                    CloudPageBlob newblob = parentContainer.GetPageBlobReference(filepath);
                    newblob.StartCopyFromBlob(pblob);
                    pblob.DeleteIfExists();

                } else {
                    CloudBlockBlob bblob = (CloudBlockBlob)blob;
                    filepath = bblob.Name.Substring(bblob.Name.IndexOf("/") + 1);
                    CloudBlockBlob newblob = parentContainer.GetBlockBlobReference(filepath);
                    newblob.StartCopyFromBlob(bblob);
                    bblob.DeleteIfExists();
                }
            }
        }

        internal static CloudBlockBlob CreateBlob(string container = "", string filename = "", Stream file = null) {
            if (file == null) { throw new Exception("No file found."); }

            if (file.Length > 0) {
                // Retrieve stroage account from connection string
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

                // Create the blob client
                CloudBlobClient client = storageAccount.CreateCloudBlobClient();

                // For large file copies you need to set up a custom timeout period
                // and using parallel settings appears to spread the copy across multiple threads
                // if you have big bandwidth you can increase the thread number below
                // because Azure accepts blobs broken into blocks in any order of arrival. Fucking awesome!
                client.ServerTimeout = new System.TimeSpan(1, 0, 0);
                client.ParallelOperationThreadCount = 2;


                // Retrieve reference to the previously created container
                CloudBlobContainer blobContainer = null;
                if (container.Length == 0) {
                    blobContainer = client.GetContainerReference("Misc");
                } else {
                    blobContainer = client.GetContainerReference(container);
                }
                blobContainer.CreateIfNotExists(); // Create the container if it doesn't exist
                blobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                CloudBlockBlob blob = null;
                blob = blobContainer.GetBlockBlobReference(filename.Replace(" ", ""));

                // Get the content type of the file;
                string content_type = "image/png";
                Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(Path.GetExtension(filename).ToLower());
                if(rk != null && rk.GetValue("Content Type") != null){ content_type = rk.GetValue("Content Type").ToString(); }

                blob.UploadFromStream(file);

                /// Set the metadata into the blob
                blob.Metadata["FileName"] = filename;
                blob.Metadata["Submitter"] = "Automated Encoder";
                blob.SetMetadata();

                // Set the properties
                blob.Properties.ContentType = content_type;
                blob.SetProperties();

                return blob;
            } else {
                throw new Exception("Invalid Content-Length: " + file.Length);
            }
        }

        internal static void DeleteBlob(string containerName, string blobUri) {
            if (containerName == null || containerName.Length == 0) {
                throw new Exception("Invalid container reference.");
            } else if (blobUri == null || blobUri.Length == 0) {
                throw new Exception("Invalid blob reference.");
            }

            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            // Create the blobl client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a previously created container
            CloudBlobContainer container = client.GetContainerReference(containerName);

            List<IListBlobItem> blobs = new List<IListBlobItem>();
            blobs = container.ListBlobs().ToList<IListBlobItem>();

            foreach (IListBlobItem blob in blobs) {
                if (blob.Uri.ToString().ToLower() == blobUri.ToLower()) {
                    if (blob.GetType() == typeof(CloudBlockBlob)) {
                        CloudBlockBlob item = (CloudBlockBlob)blob;
                        item.DeleteIfExists();
                    } else if (blob.GetType() == typeof(CloudPageBlob)) {
                        CloudPageBlob item = (CloudPageBlob)blob;
                        item.DeleteIfExists();
                    }
                }
            }
        }
    }

    public class DiscountBlobContainer {
        public CloudBlobContainer Container { get; set; }
        public int BlobCount { get; set; }
        public List<DiscountBlobContainer> SubContainers { get; set; }
    }
}