using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureFtpServer.Provider
{
    public static class AzureProviderMethods
    {
        public static bool HasFiles(this CloudBlobContainer c, string prefix)
        {
            if (prefix.StartsWith("/"))
                prefix = prefix.Remove(0, 1);

            int resultsa = c.ListBlobs(prefix).OfType<CloudBlockBlob>().Count();
            int resultsb = c.ListBlobs(prefix).OfType<CloudPageBlob>().Count();
            int total = resultsa + resultsb;
            return total > 0;
        }

        public static bool HasDirectories(this CloudBlobContainer c, string prefix)
        {
            if (prefix.StartsWith("/"))
                prefix = prefix.Remove(0, 1);

            int results = c.ListBlobs(prefix).OfType<CloudBlobDirectory>().Count();
            return results > 0;
        }
    }
}