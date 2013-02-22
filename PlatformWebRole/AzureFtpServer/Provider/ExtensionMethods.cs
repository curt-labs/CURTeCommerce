using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureFtpServer.Provider
{
    public static class AzureProviderMethods
    {
        public static bool HasFiles(this CloudBlobContainer c, string prefix)
        {
            IEnumerable<CloudBlockBlob> resultsa =
                (from f in c.ListBlobs(prefix,true).OfType<CloudBlockBlob>()
                 where f.Uri.ToString().Contains(prefix)
                 select f);
            IEnumerable<CloudPageBlob> resultsb =
                (from f in c.ListBlobs(prefix, true).OfType<CloudPageBlob>()
                 where f.Uri.ToString().Contains(prefix)
                 select f);
            return (resultsa.Count() + resultsb.Count()) > 0;
        }

        public static bool HasDirectories(this CloudBlobContainer c, string prefix)
        {
            IEnumerable<CloudBlobDirectory> results =
                (from d in c.ListBlobs(prefix,true).OfType<CloudBlobDirectory>()
                 where d.Uri.ToString().Contains(prefix)
                 select d);
            return results.Count() > 0;
        }
    }
}