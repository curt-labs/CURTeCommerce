using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;

namespace TaskScheduler {
    class Logger {
        public static void log(string message = "") {
            Settings settings = new Settings();
            string loggingstring = settings.Get("logging");
            bool isLoggingEnabled = (loggingstring.ToLower().Trim() != "true") ? false : true;
            if (isLoggingEnabled) {
                DiscountBlobContainer logblobs = new DiscountBlobContainer();
                logblobs = BlobManagement.GetContainer("logs");
                BlobContainerPermissions perms = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob };
                logblobs.Container.SetPermissions(perms);
                CloudBlob blob = logblobs.Container.GetBlobReference("log-" + String.Format("{0:MM-dd-yyyy}", DateTime.Now) + ".txt");
                string blobtext = "";
                try {
                    blobtext = blob.DownloadText();
                } catch { };
                blob.UploadText(blobtext + String.Format("{0:M-d-yyyy HH:mm:ss.fff: }", DateTime.Now) + message + Environment.NewLine);
            }
        }
    }
}
