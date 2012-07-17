using System;
using System.Text;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.Provider;

namespace AzureFtpServer.Azure
{
    public sealed class AzureFileInfo : IFileInfo
    {
        #region Implementation of IFileInfo

        private readonly AzureCloudFile _file;
        private readonly AzureBlobStorageProvider _provider;

        public AzureFileInfo(AzureCloudFile file, AzureBlobStorageProvider provider)
        {
            _file = file;
            _provider = provider;
        }

        public string Path()
        {
            return _file.Uri.ToString();
        }

        public bool FileObjectExists()
        {
            return _file != null;
        }

        public DateTime GetModifiedTime()
        {
            return _file.LastModified;
        }

        public long GetSize()
        {
            return _file.Size;
        }

        public string GetAttributeString()
        {
            bool fDirectory = IsDirectory();
            bool fReadOnly = false; // No file should be read-only.

            var builder = new StringBuilder();

            builder.Append(fDirectory ? "d" : "-");

            builder.Append("r");

            if (fReadOnly)
            {
                builder.Append("-");
            }
            else
            {
                builder.Append("w");
            }

            builder.Append(fDirectory ? "x" : "-");

            builder.Append(fDirectory ? "r-xr-x" : "r--r--");

            return builder.ToString();
        }

        public bool IsDirectory()
        {
            bool result = _file == null || !_provider.CheckBlobExists(_file.Uri.ToString());
            return result;
        }

        #endregion
    }
}