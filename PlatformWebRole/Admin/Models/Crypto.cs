using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models {
    public class Crypto {

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