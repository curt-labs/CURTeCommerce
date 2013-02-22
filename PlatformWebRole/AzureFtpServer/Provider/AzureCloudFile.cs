using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace AzureFtpServer.Provider
{
    public sealed class AzureCloudFile
    {
        public DateTime LastModified { get; set; }

        #region AzureCloudFile Members

        public byte[] Data { get; set; }
        public Uri Uri { get; set; }
        public StorageOperationResult StorageOperationResult { get; set; }
        public IDictionary<string,string> Meta { get; set; }
        public long Size { get; set; }
        public String ContentType { get; set; }

        #endregion
    }
}