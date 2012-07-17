using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AzureFtpServer.Azure;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.Provider;

namespace AzureFTP
{
    public class AzureFileSystem : IFileSystem
    {
        private readonly AzureBlobStorageProvider _provider;
        private String _containerName;
        private String _password;
        private String _userName;

        // Constructor
        public AzureFileSystem(String userName, String password, String containerName, Modes mode)
        {
            _userName = userName;
            _password = password;

            // Set container name (if none specified, specify the development container default)
            _containerName = !String.IsNullOrEmpty(containerName) ? containerName : "DevelopmentContainer";
            _provider = new AzureBlobStorageProvider(containerName);
        }

        #region Implementation of IFileSystem

        public IFile OpenFile(string sPath, bool fWrite)
        {
            var f = new AzureFile();
            return f;
        }

        public IFileInfo GetFileInfo(string sPath)
        {
            sPath = PreparePath(sPath);
            AzureCloudFile file = _provider.Get(sPath, false);
            var info = new AzureFileInfo((AzureCloudFile) file, _provider);
            return info;
        }

        public string[] GetFiles(string sPath)
        {
            sPath = PreparePath(sPath);
            CloudFileCollection files = _provider.GetFileListing(sPath);
            string[] result = files.Select(r => r.Uri.ToString()).ToArray().ToFtpPath();
            return result;
        }

        public string[] GetFiles(string sPath, string sWildcard)
        {
            sPath = PreparePath(sPath);
            CloudFileCollection files = _provider.GetFileListing(sPath);
            IEnumerable<string> result = (from f in files
                                          where f.Uri.ToString().Contains(sWildcard)
                                          select f.Uri.ToString());
            string[] r = result.ToArray().ToFtpPath();
            return r;
        }

        public string[] GetDirectories(string sPath)
        {
            sPath = PreparePath(sPath);
            CloudDirectoryCollection directories = _provider.GetDirectoryListing(sPath);
            return directories.Select(r => r.Path).ToArray().ToFtpPath();
        }

        public string[] GetDirectories(string sPath, string sWildcard)
        {
            sPath = PreparePath(sPath);
            CloudDirectoryCollection directories = _provider.GetDirectoryListing(sPath);
            IEnumerable<CloudDirectory> result = (from dir in directories
                                                  where dir.Path.Contains(sWildcard)
                                                  select dir
                                                 );

            return directories.Select(r => r.Path).ToArray().ToFtpPath();
        }

        public bool DirectoryExists(string sPath)
        {
            sPath = PreparePath(sPath);
            return _provider.IsValidPath(sPath);
        }

        public bool FileExists(string sPath)
        {
            sPath = PreparePath(sPath);
            return _provider.CheckBlobExists(sPath);
        }

        public bool CreateDirectory(string sPath)
        {
            sPath = PreparePath(sPath);
            _provider.CreateDirectory(sPath);
            return true;
        }

        public bool Move(string sOldPath, string sNewPath)
        {
            sOldPath = PreparePath(sOldPath);
            sNewPath = PreparePath(sNewPath);

            return _provider.Rename(sOldPath, sNewPath) == StorageOperationResult.Completed;
        }

        public bool Delete(string sPath)
        {
            sPath = PreparePath(sPath);
            _provider.Delete(new AzureCloudFile {Uri = new Uri(sPath, UriKind.RelativeOrAbsolute)});
            return true;
        }

        #endregion

        #region IFileSystem Members

        public bool Put(string sPath, IFile oFile)
        {
            var f = new AzureCloudFile
                        {
                            Uri = new Uri(sPath, UriKind.RelativeOrAbsolute),
                            Data = oFile.File.ToArray(),
                            Size = oFile.File.Length
                        };

            _provider.Put(f);
            return true;
        }

        #endregion

        /// <summary>
        /// Correctly prefixes a given path with the container name
        /// </summary>
        /// <param name="sPath">The path from the FTP provider</param>
        /// <returns></returns>
        private static String PreparePath(String sPath)
        {
            sPath = sPath.Replace(@"\", @"/");
            return sPath;
        }
    }
}