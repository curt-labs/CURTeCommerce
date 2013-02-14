using AzureFTP;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.Provider;
using System.Linq;
using System.Data.Linq;
using System;
using System.Text;

namespace AzureFtpServer.Azure {
    public class AzureFileSystemFactory : IFileSystemClassFactory {
        #region Implementation of IFileSystemClassFactory

        public IFileSystem Create(string sUser, string sPassword) {
            string containerName = "";
            // TODO: Put your authentication call here. Return NULL if authentication fails. Return an initialised AzureFileSystem() object if authentication is successful.
            // In the example below, to demonstrate, any username will work so long as the password == "test".
            // Remember: you can plug in your own authentication & authorisation API to fetch the correct container name for the specified user.

            #region "REPLACE THIS WITH CODE TO YOUR OWN AUTHENTICATION API"
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            try {
                Profile p = AzureFtpServer.General.ProfileModel.GetProfile(sUser, sPassword);
                if (!AzureFtpServer.General.ProfileModel.hasPermission(p.id)) {
                    throw new Exception("You do not have permission.");
                }
                containerName = p.username;
            } catch {
                return null;
            };

            #endregion

            IFileSystem system = new AzureFTP.AzureFileSystem(sUser, sPassword, containerName);
            return system;
        }

        #endregion
    }
}