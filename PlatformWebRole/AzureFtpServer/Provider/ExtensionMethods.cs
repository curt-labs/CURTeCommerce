using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;

namespace AzureFtpServer.Provider
{
    public static class AzureProviderMethods
    {
        public static bool HasFiles(this CloudBlobContainer c, string prefix)
        {
            IEnumerable<CloudBlob> results =
                (from f in c.ListBlobs(new BlobRequestOptions {UseFlatBlobListing = true}).OfType<CloudBlob>()
                 where f.Uri.ToString().Contains(prefix)
                 select f);
            return results.Count() > 0;
        }

        public static bool HasDirectories(this CloudBlobContainer c, string prefix)
        {
            IEnumerable<CloudBlobDirectory> results =
                (from d in c.ListBlobs(new BlobRequestOptions {UseFlatBlobListing = true}).OfType<CloudBlobDirectory>()
                 where d.Uri.ToString().Contains(prefix)
                 select d);
            return results.Count() > 0;
        }
    }
}