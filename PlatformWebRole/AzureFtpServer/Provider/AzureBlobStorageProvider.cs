using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using AzureFtpServer.Provider;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;

namespace AzureFtpServer.Provider {

    public class StorageProviderEventArgs : EventArgs {
        public StorageOperation Operation;
        public StorageOperationResult Result;
    }

    public sealed class AzureBlobStorageProvider {
        // Events

        #region Delegates

        public delegate void PutCompletedEventHandler(AzureCloudFile o, StorageOperationResult r);

        #endregion

        private CloudStorageAccount _account;
        private CloudBlobClient _blobClient;

        public AzureBlobStorageProvider(String containerName) {
            Initialise(containerName);
        }

        // Default constructor, required for reflection.
        public AzureBlobStorageProvider() {
            Initialise(null); // HTTPS disabled; Asynch Calls enabled by default.
        }

        private Uri BaseUri {
            get {
                if (StorageProviderConfiguration.Mode == Modes.Development || StorageProviderConfiguration.Mode == Modes.Debug) {
                    return new Uri(CloudStorageAccount.DevelopmentStorageAccount.BlobEndpoint.AbsoluteUri);
                } else {
                    return new Uri(string.Format("http://{0}.blob.core.windows.net", StorageProviderConfiguration.AccountName));
                }

            }
        }

        public bool UseHttps { get; private set; }

        public Boolean RetryOnTimeout { get; set; }
        public Boolean UseAsynchCalls { get; set; }
        public String ContainerName { private get; set; }

        #region IStorageProvider Members

        /// <summary>
        /// Occurs when a storage provider operation has completed.
        /// </summary>
        public event EventHandler<StorageProviderEventArgs> StorageProviderOperationCompleted;

        public String FolderDelimiter {
            get { return "/"; }
        }

        #endregion

        // Delegates

        // Initialiser method
        private void Initialise(String containerName) {

            if (String.IsNullOrEmpty(containerName))
                throw new ArgumentException("You must provide the base Container Name", "containerName");

            ContainerName = containerName;

            if (StorageProviderConfiguration.Mode == Modes.Development || StorageProviderConfiguration.Mode == Modes.Debug) {
                Uri base_uri = this.BaseUri;
                _account = CloudStorageAccount.DevelopmentStorageAccount;
                _blobClient = _account.CreateCloudBlobClient();
                _blobClient.ServerTimeout = new TimeSpan(0, 0, 0, 5);
            } else {
                string connstr = RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString");
                _account = CloudStorageAccount.Parse(connstr);
                _blobClient = _account.CreateCloudBlobClient();
                _blobClient.ServerTimeout = new TimeSpan(0, 0, 0, 5);
            }

            _blobClient.GetContainerReference(ContainerName).CreateIfNotExists();

        }


        #region "Storage operations"

        /// <summary>
        /// Puts the specified object onto the cloud storage provider.
        /// </summary>
        /// <param name="o">The object to store.</param>
        public void Put(AzureCloudFile o) {
            if (o.Data == null)
                throw new ArgumentNullException("o", "AzureCloudFile cannot be null.");

            if (o.Uri == null)
                throw new ArgumentException("Parameter 'Uri' of argument 'o' cannot be null.");

            string path = o.Uri.ToString();

            if (path.StartsWith(@"/"))
                path = path.Remove(0, 1);

            if (path.StartsWith(@"\\"))
                path = path.Remove(0, 1);

            if (path.StartsWith(@"\"))
                path = path.Remove(0, 1);

            // Remove double back slashes from anywhere in the path
            path = path.Replace(@"\\", @"\");

            CloudBlobContainer container = _blobClient.GetContainerReference(ContainerName);
            container.CreateIfNotExists();

            // Set permissions on the container
            BlobContainerPermissions perms = container.GetPermissions();
            if (perms.PublicAccess != BlobContainerPublicAccessType.Container) {
                perms.PublicAccess = BlobContainerPublicAccessType.Container;
                container.SetPermissions(perms);
            }


            // Create a reference for the filename
            String uniqueName = path;
            CloudBlockBlob blob = container.GetBlockBlobReference(uniqueName);

            // Create a new AsyncCallback instance
            AsyncCallback callback = PutOperationCompleteCallback;
            blob.BeginUploadFromStream(new MemoryStream(o.Data), callback, o.Uri);

            // Uncomment for synchronous upload
            //blob.UploadFromStream(new System.IO.MemoryStream(o.Data));
        }

        /// <summary>
        /// Delete the specified AzureCloudFile from the Azure container.
        /// </summary>
        /// <param name="o">The object to be deleted.</param>
        public void Delete(AzureCloudFile o) {
            ICloudBlob b = _blobClient.GetBlobReferenceFromServer(o.Uri);
            if (b != null)
                b.BeginDelete(new AsyncCallback(DeleteOperationCompleteCallback), o.Uri);
            else
                throw new ArgumentException("The container reference could not be retrieved from storage provider.", "o");
        }

        /// <summary>
        /// Retrieves the object from the storage provider
        /// </summary>
        /// <param name="path">The fully qualified OR relative URI to the object to be retrieved</param>
        /// <param name="downloadData">Boolean indicating whether to download the contents of the file to the Data property or not</param>
        /// <returns>AzureCloudFile</returns>
        /// <exception cref="FileNotFoundException">Throws a FileNotFoundException if the URI is not found on the provider.</exception>
        public AzureCloudFile Get(string path, bool downloadData) {
            UriKind kind = UriKind.RelativeOrAbsolute;
            CloudBlobContainer parentContainer = _blobClient.GetContainerReference(ContainerName);
            Uri u = new Uri(parentContainer.Uri.ToString() + path, kind);
            string blobPath = UriPathToString(u);

            if (blobPath.StartsWith(@"/"))
                blobPath = blobPath.Remove(0, 1);

            //Uri uri = new Uri(blobPath);
            //blobPath = RemoveContainerName(blobPath);
            var o = new AzureCloudFile();
            ICloudBlob b = null;
            try {
                b = _blobClient.GetBlobReferenceFromServer(u);
                
                b.FetchAttributes();
                o = new AzureCloudFile {
                    Meta = b.Metadata,
                    StorageOperationResult = StorageOperationResult.Completed,
                    Uri = new Uri(blobPath, UriKind.RelativeOrAbsolute),
                    LastModified = b.Properties.LastModified.Value.UtcDateTime,
                    ContentType = b.Properties.ContentType,
                    Size = b.Properties.Length
                };

                o.Meta.Add("ContentType", b.Properties.ContentType);
            } catch (StorageException ex) {
                StorageExtendedErrorInformation extendedInformation = ex.RequestInformation.ExtendedErrorInformation;
                if (extendedInformation != null && extendedInformation.ErrorCode == "BlobNotFound") {
                    throw new FileNotFoundException(
                        "The storage provider was unable to locate the object identified by the given URI.",
                        u.ToString());
                }

                if (extendedInformation != null && extendedInformation.ErrorCode == "ResourceNotFound") {
                    return null;
                }
            }

            // Try to download the data for the blob, if requested
            // TODO: Implement asynchronous calls for this
            try {
                if (downloadData && b != null) {
                    MemoryStream datastream = new MemoryStream();
                    b.DownloadToStream(datastream);
                    byte[] data = datastream.ToArray();
                    o.Data = data;
                }
            } catch (TimeoutException) {
                if (RetryOnTimeout) {
                    Get(blobPath, downloadData); // NOTE: Infinite retries, what fun! :)
                    // TODO: Implement retry attempt limitation
                } else {
                    throw;
                }
            }

            return o;
        }

        /// <summary>
        /// Checks if a blob exists within the container with the specified path (name).
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public bool CheckBlobExists(string path) {
            UriKind kind = UriKind.RelativeOrAbsolute;
            CloudBlobContainer parentContainer = _blobClient.GetContainerReference(ContainerName);
            Uri uri = new Uri(parentContainer.Uri.ToString() + path, kind);

            try {
                ICloudBlob b = _blobClient.GetBlobReferenceFromServer(uri);
                b.FetchAttributes();
                return true;
            } catch (StorageException e) {
                return false;
            }
        }

        /// <summary>
        /// Gets the directory listing of all blobs within the parent container specified.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public CloudDirectoryCollection GetDirectoryListing(string path) {
            path = ParsePath(path);
            CloudBlobContainer container = _blobClient.GetContainerReference(ContainerName);
            var directories = new CloudDirectoryCollection();

            if (path == "") {
                directories.AddRange(
                    container.ListBlobs().OfType<CloudBlobDirectory>().Select(
                        dir => new CloudDirectory { Path = dir.Uri.ToString() }));
            } else {
                CloudBlobDirectory parent = container.GetDirectoryReference(path);
                directories.AddRange(
                    parent.ListBlobs().OfType<CloudBlobDirectory>().Select(
                        dir => new CloudDirectory { Path = dir.Uri.ToString() }));
            }

            return directories;
        }

        public CloudFileCollection GetFileListing(string path) {
            String prefix = String.Concat(ContainerName, "/", ParsePath(path));
            var files = new CloudFileCollection();
            files.AddRange(
                _blobClient.ListBlobs(prefix).OfType<CloudBlockBlob>().Select(
                    blob =>
                    new AzureCloudFile {
                        Meta = blob.Metadata,
                        Uri = blob.Uri,
                        Size = blob.Properties.Length,
                        ContentType = blob.Properties.ContentType
                    }));

            return files;
        }

        /// <summary>
        /// Overwrites the object stored at the original path with the new object. Checks if the existing path exists, then calls PUT on the new path.
        /// </summary>
        /// <param name="originalPath">The original path.</param>
        /// <param name="newObject">The new object.</param>
        public void Overwrite(string originalPath, AzureCloudFile newObject) {
            // Check if the original path exists on the provider.
            if (!CheckBlobExists(originalPath)) {
                throw new FileNotFoundException("The path supplied does not exist on the storage provider.",
                                                originalPath);
            }

            // Put the new object over the top of the old...
            Put(newObject);
        }

        /// <summary>
        /// Renames the specified object by copying the original to a new path and deleting the original.
        /// </summary>
        /// <param name="originalPath">The original path.</param>
        /// <param name="newPath">The new path.</param>
        /// <returns></returns>
        public StorageOperationResult Rename(string originalPath, string newPath) {
            var u = new Uri(newPath, UriKind.RelativeOrAbsolute);
            //CloudBlobContainer c = GetContainerReference(ContainerName);

            newPath = UriPathToString(u);
            if (newPath.StartsWith("/"))
                newPath = newPath.Remove(0, 1);

            originalPath = UriPathToString(new Uri(originalPath, UriKind.RelativeOrAbsolute));
            if (originalPath.StartsWith("/"))
                originalPath = originalPath.Remove(0, 1);

            ICloudBlob originalBlob = _blobClient.GetBlobReferenceFromServer(new Uri(originalPath));

            // Check if the original path exists on the provider.
            if (!CheckBlobExists(originalPath)) {
                throw new FileNotFoundException("The path supplied does not exist on the storage provider.",
                                                originalPath);
            }

            CloudBlobContainer parentContainer = GetContainerReference(GetParentFromPath(newPath));
            AsyncCallback callback = MoveOperationCompleteCallback;
            List<Uri> blobrefs = new List<Uri>();
            blobrefs.Add(originalBlob.Uri);

            if (originalBlob.GetType() == typeof(CloudBlockBlob)) {
                CloudBlockBlob newBlob = parentContainer.GetBlockBlobReference(GetDirectoryFromPath(newPath));
                blobrefs.Add(newBlob.Uri);
                newBlob.BeginStartCopyFromBlob((CloudBlockBlob)originalBlob, callback, blobrefs);
            } else if (originalBlob.GetType() == typeof(CloudPageBlob)) {
                CloudPageBlob newBlob = parentContainer.GetPageBlobReference(GetDirectoryFromPath(newPath));
                blobrefs.Add(newBlob.Uri);
                newBlob.BeginStartCopyFromBlob((CloudPageBlob)originalBlob, callback, blobrefs);
            }

            return StorageOperationResult.Completed;
        }

        public void CreateDirectory(string path) {
            if (path.StartsWith("/"))
                path = path.Remove(0, 1);

            CloudBlobContainer container = _blobClient.GetContainerReference(ContainerName);
            string blobName = String.Concat(path, "/required.req");
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            string message = "#REQUIRED: At least one file is required to be present in this folder.";
            byte[] msgbytes = Encoding.ASCII.GetBytes(message);
            MemoryStream msgstream = new MemoryStream(msgbytes);
            blob.UploadFromStream(msgstream);

            BlobProperties props = blob.Properties;
            props.ContentType = "text/text";
            blob.SetProperties();
        }

        /// <summary>
        /// Determines whether the specified path is a valid blob or folder name (if it exists).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsValidPath(string path) {
            if (path != null)
                if (path == "/")
                    return true;

            CloudBlobContainer c = GetContainerReference(ContainerName);
            if (c.HasDirectories(path))
                return true;

            if (c.HasFiles(path))
                return true;

            try {
                ICloudBlob b = c.GetBlobReferenceFromServer(path);
                b.FetchAttributes();
            } catch (StorageException ex) {
                StorageExtendedErrorInformation extendedInformation = ex.RequestInformation.ExtendedErrorInformation;
                if (extendedInformation.ErrorCode == "ResourceNotFound") {
                    return false;
                } else {
                    throw;
                }
            }

            return false;
        }

        #endregion

        #region "Helper methods"

        /// <summary>
        /// Returns the container name from the fileNameAndPath parameter.
        /// </summary>
        /// <param name="path">The full URI to the stored object, including the filename.</param>
        /// <returns></returns>
        private static string ExtractContainerName(String path) {
            return path.Split('/')[0].ToLower(); // Azure requires URI's in lowercase
        }

        /// <summary>
        /// Returns the container name from the fileNameAndPath parameter.
        /// </summary>
        /// <param name="path">The full URI to the stored object, including the filename.</param>
        /// <returns></returns>
        internal static string GetParentFromPath(string path) {
            if (path.Contains('/')) {
                List<string> paths = path.Split('/').ToList();
                string parent = paths.FirstOrDefault();
                return parent;
            } else {
                return path;
            }
        }

        /// <summary>
        /// Returns the directory and blob name from the fileNameAndPath parameter.
        /// </summary>
        /// <param name="path">The full URI to the stored object, including the filename.</param>
        /// <returns></returns>
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

        internal static bool hasExtension(string path) {
            if (path.Contains('.')) {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// A helper method to return an initialised CloudBlobContainer object.
        /// </summary>
        /// <param name="containerName">The container to retrieve.</param>
        /// <returns></returns>
        private CloudBlobContainer GetContainerReference(string containerName) {
            // Put a reference to the container if one does not exist already
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();

            BlobContainerPermissions permissions = container.GetPermissions();
            if (permissions.PublicAccess != BlobContainerPublicAccessType.Container) {
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                container.SetPermissions(permissions);
            }

            return container;
        }

        /// <summary>
        /// Parses the path to ensure it is compatible with Azure.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private String ParsePath(String path) {
            if (!path.EndsWith("/"))
                path += "/";

            switch (path) {
                case "/":
                    path = "";
                    break;
                default:
                    if (!path.EndsWith("/")) {
                        path += "/";
                    } else {
                        path = path.Remove(0, 1);
                    }

                    break;
            }

            path = path.Replace(@"//", "/");

            return path;
        }

        /// <summary>
        /// Performs .ToString() on the specified URI dependant on type
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        private string UriPathToString(Uri u) {
            if (u.IsAbsoluteUri) {
                return u.PathAndQuery;
            } else {
                return u.ToString();
            }
        }

        /// <summary>
        /// Removes the container name and trailing slash from the specified path.
        /// </summary>
        /// <param name="path">The path you want to strip</param>
        /// <returns></returns>
        private string RemoveContainerName(string path) {
            path = path.Replace(ContainerName + @"/", "");
            return path;
        }

        #endregion

        #region "Callbacks"

        /// <summary>
        /// Announce completion of PUT operation
        /// </summary>
        /// <param name="result"></param>
        private void PutOperationCompleteCallback(IAsyncResult result) {
            var o = (Uri)result.AsyncState;
            if (StorageProviderOperationCompleted == null) return;
            var a = new StorageProviderEventArgs { Operation = StorageOperation.Put, Result = StorageOperationResult.Created };

            // Raise the event
            StorageProviderOperationCompleted(o, a);
        }

        /// <summary>
        /// Announce completion of a Delete operation.
        /// </summary>
        /// <param name="result"></param>
        private void DeleteOperationCompleteCallback(IAsyncResult result) {
            var o = (Uri)result.AsyncState;

            if (StorageProviderOperationCompleted == null) return;
            var a = new StorageProviderEventArgs { Operation = StorageOperation.Delete, Result = StorageOperationResult.Deleted };
            // Raise the event
            StorageProviderOperationCompleted(o, a);
        }

        /// <summary>
        /// Announce completion of MOVE operation
        /// </summary>
        /// <param name="result"></param>
        private void MoveOperationCompleteCallback(IAsyncResult result) {
            List<Uri> uris = (List<Uri>)result.AsyncState;
            if (StorageProviderOperationCompleted == null) return;
            var a = new StorageProviderEventArgs { Operation = StorageOperation.Put, Result = StorageOperationResult.Created };

            // Raise the event
            StorageProviderOperationCompleted(uris[1], a);

            ICloudBlob originalBlob = _blobClient.GetBlobReferenceFromServer(uris[0]);
            AsyncCallback callback = DeleteOperationCompleteCallback;
            originalBlob.BeginDeleteIfExists(callback, uris[0]);
        }
        #endregion
    }
}