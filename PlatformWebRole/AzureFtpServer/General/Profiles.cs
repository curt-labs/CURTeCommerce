using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureFtpServer.General {
    class ProfileModel {
        public static Profile GetProfile(string username = "", string pass = "") {
            Profile p = new Profile();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            string encpass = EncryptString(pass);
            p = db.Profiles.Where(x => x.username.Equals(username) && x.password.Equals(encpass)).First<Profile>();
            return p;
        }

        public static bool hasPermission(int profileID) {
            bool hasPermission = false;
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            hasPermission = (from m in db.Modules
                                  join pm in db.ProfileModules on m.id equals pm.moduleID
                                  where pm.profileID == profileID && m.name.ToLower().Equals("ftp")
                                  select m.id).Count() > 0;

            return hasPermission;
        }

        public static string EncryptString(string input) {
            try {
                System.Security.Cryptography.MD5CryptoServiceProvider crypto = new System.Security.Cryptography.MD5CryptoServiceProvider();

                byte[] data = System.Text.Encoding.ASCII.GetBytes(input);
                data = crypto.ComputeHash(data);

                return System.Text.Encoding.ASCII.GetString(data);
            } catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

    }
}
