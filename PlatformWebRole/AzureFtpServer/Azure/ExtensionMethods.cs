using System;
using System.Web;

namespace AzureFtpServer.Azure
{
    public static class ExtensionMethods
    {
        // Convert a string instance of a URI to a relative FTP path
        public static String ToFtpPath(String s)
        {
            // Remove any trailing slashes
            if (s.EndsWith(@"/"))
            {
                int lastIndex = s.LastIndexOf(@"/");
                s = s.Remove(lastIndex);
            }

            // We want to remove the TOP level directory (which should be the development storage account)
            var u = new Uri(s);
            string path = u.PathAndQuery;

            // Remove all path information except for the file name itself
            int startIndex = path.LastIndexOf(@"/");
            path = path.Substring(startIndex + 1);

            return HttpUtility.UrlDecode(path);
        }

        // Convert an array of string URI paths to a string array of FTP paths
        public static string[] ToFtpPath(this string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = ToFtpPath(paths[i]);
            }
            return paths;
        }
    }
}