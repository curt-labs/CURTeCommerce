using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure;
using System.IO;
using System.Drawing;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text;

namespace Admin.Models {
    public class BlobManagement {

        internal static DiscountBlobContainer CreateContainer(string name = "", string parent = "", bool make_public = true) {

            // Build out a relationship between the new container name and parent (if parent is provided)
            string conName = name.ToLower().Replace(' ','-');
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            // Create the blobl client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            DiscountBlobContainer container = new DiscountBlobContainer();
            
            CloudBlobContainer con = null;
            if (parent.Length > 0) {
                string[] folders = parent.Split('/');
                parent = "";
                for (var i = 1; i < folders.Count(); i++) {
                    parent += folders[i];
                    if (i < folders.Count() - 1 && parent != "") {
                        parent += "/";
                    }
                }
                conName = (parent != "") ? parent + "/" + conName : conName;
                string fileName = "/required.req";
                string filetext = "#REQUIRED: At least one file is required to be present in this folder.";
                con = client.GetContainerReference(folders[0]);
                CloudBlockBlob f = con.GetBlockBlobReference(conName + fileName);

                byte[] blobBytes = Encoding.ASCII.GetBytes(filetext);
                MemoryStream stream = new MemoryStream(blobBytes);

                f.UploadFromStream(stream);
                //f.UploadText(filetext);
                CloudBlobDirectory d = con.GetDirectoryReference(conName);
                
                // Cast to our object
                container = new DiscountBlobContainer {
                    Container = d.Container,
                    BlobCount = d.ListBlobs().Count(),
                    uri = d.Uri,
                    SubContainers = GetSubContainers(conName)
                };

            }else{
                // Retrieve a reference to a container
                con = client.GetContainerReference(conName);

                // Create the container if it doesn't already exist
                con.CreateIfNotExists();
                
                // Cast to our object
                container = new DiscountBlobContainer {
                    Container = client.GetContainerReference(conName),
                    BlobCount = 0,
                    uri = client.GetContainerReference(conName).Uri,
                    SubContainers = GetSubContainers(conName)
                };
            }

            if (make_public) { // Make access to this container public
                con.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            
            return container;
        }

        internal static string GetParentFromPath(string path) {
            if (path.Contains('/')) {
                List<string> paths = path.Split('/').ToList();
                string parent = paths.FirstOrDefault();
                return parent;
            } else {
                return path;
            }
        }

        internal static string GetDirectoryFromPath(string path) {
            if (path.Contains('/')) {
                List<string> paths = path.Split('/').ToList();
                paths.RemoveAt(0);
                string folder = string.Join("/", paths.ToArray());
                return folder;
            } else {
                return "";
            }
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
            BlobContainerPermissions perms = con.GetPermissions();
            if (perms.PublicAccess != BlobContainerPublicAccessType.Container) {
                perms.PublicAccess = BlobContainerPublicAccessType.Container;
                con.SetPermissions(perms);
            }


            DiscountBlobContainer container = new DiscountBlobContainer {
                Container = con,
                BlobCount = GetBlobs(con.Name).Count,
                uri = con.Uri,
                SubContainers = GetSubContainers(con.Name)
            };
            return container;
        }

        internal static DiscountBlobContainer GetFolder(string name) {
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
            CloudBlobContainer parentContainer = client.GetContainerReference(GetParentFromPath(name));
            CloudBlobDirectory con = parentContainer.GetDirectoryReference(GetDirectoryFromPath(name));

            DiscountBlobContainer container = new DiscountBlobContainer {
                Container = con.Container,
                parent = con.Parent ?? null,
                BlobCount = (con.ListBlobs() == null) ? 0 : con.ListBlobs().Count(),
                uri = con.Uri,
                SubContainers = GetSubContainers(name)
            };
            return container;
        }

        internal static DiscountBlobContainer GetFolderForSerialization(string name) {
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
            CloudBlobContainer parentContainer = client.GetContainerReference(GetParentFromPath(name));
            CloudBlobDirectory con = parentContainer.GetDirectoryReference(GetDirectoryFromPath(name));

            DiscountBlobContainer container = new DiscountBlobContainer {
                Container = null,
                parent = null,
                BlobCount = (con.ListBlobs() == null) ? 0 : con.ListBlobs().Count(),
                uri = con.Uri,
                SubContainers = GetSubContainers(name)
            };
            return container;
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
                uri = newContainer.Uri,
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
                    List<IListBlobItem> blobs = GetBlobs(con.Name);
                    List<BlobFile> files = new List<BlobFile>();
                    DiscountBlobContainer discountContainer = new DiscountBlobContainer {
                        BlobCount = blobs.Count,
                        Container = con,
                        uri = con.Uri,
                        SubContainers = GetSubContainers(con.Name)
                    };
                    foreach (IListBlobItem blob in blobs) {
                        if (!blob.GetType().Equals(typeof(CloudBlobDirectory))) {
                            BlobFile bf = new BlobFile {
                                uri = blob.Uri
                            };
                            files.Add(bf);
                        }
                    }
                    discountContainer.files = files;
                    containers.Add(discountContainer);
                }

                return containers;
            } catch (Exception) {
                return new List<DiscountBlobContainer>();
            }
        }

        public static List<DiscountBlobContainer> GetSubContainers(string parent = "") {
            try {
                // Retrieve storage account from connection string
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

                // Create the blobl client
                CloudBlobClient client = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer parentContainer = client.GetContainerReference(GetParentFromPath(parent));
                List<DiscountBlobContainer> subs = new List<DiscountBlobContainer>();
                if (parent.Contains("/")) {
                    CloudBlobDirectory container = parentContainer.GetDirectoryReference(GetDirectoryFromPath(parent));
                    foreach (CloudBlobDirectory sub_dir in container.ListBlobs().OfType<CloudBlobDirectory>()) {
                        DiscountBlobContainer sub = new DiscountBlobContainer {
                            BlobCount = sub_dir.ListBlobs().Count(),
                            Container = sub_dir.Container,
                            uri = sub_dir.Uri
                        };
                        subs.Add(sub);
                    }
                } else {
                    foreach (CloudBlobDirectory sub_dir in parentContainer.ListBlobs().OfType<CloudBlobDirectory>()) {
                        DiscountBlobContainer sub = new DiscountBlobContainer {
                            BlobCount = sub_dir.ListBlobs().Count(),
                            Container = sub_dir.Container,
                            uri = sub_dir.Uri
                        };
                        subs.Add(sub);
                    }

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
            blobs = container.ListBlobs().Where(x => x.GetType() != typeof(CloudBlobDirectory)).ToList<IListBlobItem>();

            return blobs;
        }

        internal static List<IListBlobItem> GetDirectoryBlobs(string directoryName = "") {
            if (directoryName.Length == 0) { return new List<IListBlobItem>(); }

            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            // Create the blobl client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            List<string> paths = directoryName.Split('/').ToList();
            string parent = paths.FirstOrDefault();
            paths.RemoveAt(0);
            string folder = string.Join("/", paths.ToArray());

            CloudBlobContainer parentContainer = client.GetContainerReference(parent);
            // Retrieve a reference to a previously created container
            CloudBlobDirectory container = parentContainer.GetDirectoryReference(folder);

            List<IListBlobItem> blobs = new List<IListBlobItem>();
            blobs = container.ListBlobs().Where(x => x.GetType() != typeof(CloudBlobDirectory)).ToList<IListBlobItem>();

            return blobs;
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
                CloudBlockBlob blob = null;
                if (container.Length == 0) {
                    container = "Misc";
                }
                if (container.Contains('/')) {
                    CloudBlobContainer parentContainer = client.GetContainerReference(GetParentFromPath(container));
                    CloudBlobDirectory blobContainer = parentContainer.GetDirectoryReference(GetDirectoryFromPath(container));
                    blob = blobContainer.GetBlockBlobReference(filename.Replace(" ", ""));
                } else {
                    CloudBlobContainer blobContainer = client.GetContainerReference(container);
                    blob = blobContainer.GetBlockBlobReference(filename.Replace(" ", ""));
                }


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

        internal static void DeleteBlob(string blobPath) {
            if (blobPath == null || blobPath.Length == 0) {
                throw new Exception("Invalid blob reference.");
            }

            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            // Create the blobl client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            Uri blobUri = new Uri(blobPath);
            ICloudBlob blob = client.GetBlobReferenceFromServer(blobUri);
            blob.DeleteIfExists();
        }

        internal static CloudBlockBlob GetOrCreateBlob(string path) {
            //UDF.SendEmail(new string[] { "jjaniuk@curtmfg.com" }, "Save log", false, path, true);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            // Create the blobl client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            // path format = /Container/folder/file.ext
            // there could be many folders
            path = path.Remove(0, 1);
            List<string> paths = path.Split('/').ToList();
            string containername = paths[0];
            paths.RemoveAt(0);
            string filepath = String.Join("/", paths.ToArray());
            //UDF.SendEmail(new string[] { "jjaniuk@curtmfg.com" }, "Save log", false, path, true);
            DiscountBlobContainer container = GetContainer(containername);
            CloudBlockBlob blob = container.Container.GetBlockBlobReference(filepath);
            return blob;
        }

        /// <summary>
        /// Renames the specified object by copying the original to a new path and deleting the original.
        /// </summary>
        /// <param name="originalPath">The original path.</param>
        /// <param name="newPath">The new path.</param>
        /// <returns></returns>
        public static CloudBlockBlob RenameFile(Uri originalPath, Uri newPath) {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            // Create the blobl client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            CloudBlockBlob oldblob = GetOrCreateBlob(originalPath.LocalPath);
            MemoryStream datastream = new MemoryStream();
            oldblob.DownloadToStream(datastream);
            byte[] databytes = datastream.ToArray();

            CloudBlockBlob newblob = GetOrCreateBlob(newPath.LocalPath);
            // upload new data to blob
            MemoryStream newstream = new MemoryStream(databytes);
            newblob.UploadFromStream(newstream);

            // delete old blob
            oldblob.Delete();

            return newblob;
        }

        public static void DeleteFile(Uri filePath) {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            // Create the blobl client
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            CloudBlockBlob blob = GetOrCreateBlob(filePath.LocalPath);
            if (blob.Exists()) {
                blob.Delete();
            }
        }

    }

    public class DiscountBlobContainer {
        public CloudBlobContainer Container { get; set; }
        public CloudBlobDirectory parent { get; set; }
        public int BlobCount { get; set; }
        public Uri uri { get; set; }
        public List<DiscountBlobContainer> SubContainers { get; set; }
        public List<BlobFile> files { get; set; }
    }

    public class BlobFile {
        public Uri uri { get; set; }
    }

}