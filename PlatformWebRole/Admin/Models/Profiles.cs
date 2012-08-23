using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using Admin.Models;
using System.Text;
using System.IO;
using System.Drawing;
using System.Net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Diagnostics;
using System.Net.Configuration;


namespace Admin.Models {
    public class Profiles {

        private static CloudBlobClient _BlobClient = null;
        private static CloudBlobContainer _BlobContainer = null;
        private static string[] allowed_profiletypes = { ".jpg", ".png", ".bmp", ".gif", ".jpeg" };

        public static List<Profile> GetAll() {
            try {
                List<Profile> profs = new List<Profile>();
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();

                profs = db.Profiles.OrderBy(x => x.date_added).ToList<Profile>();

                return profs;
            } catch (Exception) {
                return new List<Profile>();
            }
        }

        public static List<string> GetAllUsernames() {
            try {
                List<string> usernames = new List<string>();
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();

                usernames = db.Profiles.Select(x => x.username).ToList<string>();

                return usernames;
            } catch (Exception) {
                return new List<string>();
            }
        }

        public static Profile GetProfile(int id = 0, string username = "", string pass = "") {
            try {
                Console.WriteLine("Looking up user");
                Profile p = new Profile();
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                if (id == 0) {
                    p = db.Profiles.Where(x => x.username.Equals(username) && x.password.Equals(Crypto.EncryptString(pass))).FirstOrDefault<Profile>();
                    Console.WriteLine(p);
                } else {
                    p = db.Profiles.Where(x => x.id.Equals(id)).FirstOrDefault<Profile>();
                }
                return p;
            } catch (Exception) {
                return new Profile();
            }
        }

        public static Profile GetProfileFromEmail(string email = "") {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                Profile p = new Profile();

                p = db.Profiles.Where(x => x.email.Equals(email)).FirstOrDefault<Profile>();
                return p;
            } catch (Exception e) {
                throw new Exception("Failed to get profile record: "+e.Message);
            }
        }

        public static Profile Add(int id = 0, string username = "", string password = "", string email = "", string first = "", string last = "", HttpPostedFileBase file = null, string bio = "") {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                string filename = "";
                Profile p = new Profile();
                p.date_added = DateTime.Now;
                if (id > 0) {
                    p = db.Profiles.Where(x => x.id.Equals(id)).FirstOrDefault<Profile>();
                }
                p.username = username;
                p.email = email;
                p.first = first;
                p.last = last;
                p.bio = bio;
                //p.image = filename;
                if (password.Length > 0) {
                    p.password = Crypto.EncryptString(password);
                }

                if(id == 0){
                    // Attempt to find a Profile that matches the username or email
                    Profile existing = (from pro in db.Profiles
                                        where pro.username.Equals(p.username) || pro.email.Equals(p.email)
                                        select pro).FirstOrDefault<Profile>();
                    if (existing != null && existing.id > 0) {
                        throw new Exception("User already exists for that username/e-mail address.");
                    }

                    // Attempt to find a Profile that matches the first and last name
                    existing = (from pro in db.Profiles
                                        where pro.first.Equals(p.first) && pro.last.Equals(p.last)
                                        select pro).FirstOrDefault<Profile>();
                    if (existing != null && existing.id > 0) {
                        throw new Exception("User already exists for that name.");
                    }
                    db.Profiles.InsertOnSubmit(p);
                }
                db.SubmitChanges();

                if (file != null && file.ContentLength > 0) {
                    try {
                        StreamReader sr = new StreamReader(file.InputStream);

                        filename = UploadProfileImage(p.id, p.username, file);
                        p = db.Profiles.Where(x => x.id.Equals(p.id)).FirstOrDefault<Profile>();
                        p.image = filename;

                        db.SubmitChanges();
                    } catch (Exception e) {
                        string err = e.Message;
                    }
                } else if(id == 0) {
                    p.image = "/Admin/Content/img/profile_pics/User.png";
                    db.SubmitChanges();
                }
                if (id == 0) { AlertNewUser(p, password); }

                return p;
            } catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        internal static void SendForgotten(Profile p) {
            try {
                string new_pass = SetNewPassword(p.id);
                string[] tos = {p.email};
                StringBuilder sb = new StringBuilder();
                Settings settings = new Settings();

                sb.Append("<p>A new password has been generated for an account with this e-mail address from the " + settings.Get("SiteName") + " Administrative Console.</p>");
                sb.Append("<hr />");
                sb.Append("<p>To <a href='" + settings.Get("SiteURL") + "' title='Login' target='_blank'>login</a> to the " + settings.Get("SiteName") + " Administrative Console, please use the information provided below.</p>");
                sb.Append("<span>The username is: <strong>" + p.username + "</strong></span><br />");
                sb.Append("<span>The new password is: <strong>" + new_pass + "</strong></span><br />");
                sb.Append("<hr /><br />");
                sb.Append("<p style='font-size:11px'>If you feel this was a mistake please disregard this e-mail.</p>");

                UDF.SendEmail(tos, "New password for " + settings.Get("SiteName") + " Administrative Console", true, sb.ToString());
            } catch (Exception e) {
                throw new Exception("Failed to send e-mail.: " + e.Message + e.InnerException);
            }
        }

        private static string SetNewPassword(int profile_id) {
            try {
                // Retrieve the Profile record
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                Profile prof = db.Profiles.Where(x => x.id.Equals(profile_id)).FirstOrDefault<Profile>();
                if (prof == null) {
                    throw new Exception();
                }

                // Generate a new password
                string pwd = new PasswordGenerator().Generate();
                prof.password = Crypto.EncryptString(pwd);

                // Save the profile and return the new password
                db.SubmitChanges();
                return pwd;

            } catch (Exception) {
                throw new Exception("Failed to update password.");
            }
        }

        internal static void DeleteProfileImage(string username = "", int id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Profile prof = new Profile();
            if (id == 0) {
                prof = db.Profiles.Where(x => x.username.Equals(username)).FirstOrDefault<Profile>();
            } else {
                prof = db.Profiles.Where(x => x.id.Equals(id)).FirstOrDefault<Profile>();
            }

            if (prof != null && prof.image != null && prof.image.Length > 0 && prof.image.ToUpper() != "USER.PNG") {
                /*UDF.OpenPermissions("/Admin/Content/img/profile_pics");
                System.IO.File.Delete(Path.Combine(HttpContext.Current.Server.MapPath("/Admin/Content/img/profile_pics"), Path.GetFileName(prof.image)));
                prof.image = "";*/
                try {
                    // Retrieve storage acocunt from connection string
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

                    // Creat the blob client
                    CloudBlobClient client = storageAccount.CreateCloudBlobClient();

                    // Retrieve reference to a previously created container
                    CloudBlobContainer container = client.GetContainerReference("profile-pictures");

                    // Create FlatListing options
                    BlobRequestOptions opts = new BlobRequestOptions();
                    opts.UseFlatBlobListing = true;

                    // Retrieve reference to a blob name
                    foreach(CloudBlob blob in container.ListBlobs(opts)){
                        if(blob.Uri.ToString() == prof.image){
                            blob.DeleteIfExists();
                            prof.image = "";
                            break;
                        }
                    }

                    db.SubmitChanges();
                } catch (Exception) { }
                
            }
        }

        private static string UploadProfileImage(int id = 0, string username = "", HttpPostedFileBase file = null) {
            try {
                DeleteProfileImage("", id);

                #region Old way
                /*string directory = "/Admin/Content/img/profile_pics";
                string ext = Path.GetExtension(file.FileName).ToLower();
                UDF.OpenPermissions(directory);

                string file_path = Path.Combine(HttpContext.Current.Server.MapPath(directory), Path.GetFileName(username + ext));
                if (!allowed_profiletypes.Contains(ext)) {
                    throw new Exception();
                }

                Image img = Image.FromStream(file.InputStream);
                Size size = new Size(72,72);
                img = ResizeImage(img, size);
                img.Save(file_path, System.Drawing.Imaging.ImageFormat.Png);*/
                #endregion

                #region Azure Blob

                // Set up connection to Windows Azure Storage
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
                _BlobClient = storageAccount.CreateCloudBlobClient();


                // For large file copies you need to set up a custom timeout period
                // and using parallel settings appears to spread the copy across multiple threads
                // if you have big bandwidth you can increase the thread number below
                // because Azure accepts blobs broken into blocks in any order of arrival. Fucking awesome!
                _BlobClient.Timeout = new System.TimeSpan(1, 0, 0);
                _BlobClient.ParallelOperationThreadCount = 2;

                // Get and create the container
                _BlobContainer = _BlobClient.GetContainerReference("profile-pictures");
                _BlobContainer.CreateIfNotExist();

                // Set the permissions on the container to be public
                _BlobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                // Make a unique blob name
                string extension = System.IO.Path.GetExtension(file.FileName);
                string filename = username;

                // Create the Blob and upload the file
                CloudBlob blob = _BlobContainer.GetBlobReference(filename + extension);

                // Create an image object and resize the image to 72x72
                Image img = Image.FromStream(file.InputStream);
                Size size = new Size(72, 72);
                img = ResizeImage(img, size);

                // Push the image into a MemoryStream and upload the stream to our blob, fuck this is too much work
                // why can't we just say here's the image, now put it in a blob! Damn you!!!!
                MemoryStream stream = new MemoryStream();
                img.Save(stream,System.Drawing.Imaging.ImageFormat.Png);

                byte[] imgBytes = stream.GetBuffer();
                stream.Seek(0,SeekOrigin.Begin);

                blob.UploadFromStream(stream);

                // Oh yeah, dispose the stream so we don't eat up memory
                stream.Dispose();

                /// Set the metadata into the blob
                blob.Metadata["FileName"] = filename;
                blob.Metadata["Submitter"] = "Automated Encoder";
                blob.SetMetadata();

                // Set the properties
                blob.Properties.ContentType = file.ContentType;
                blob.SetProperties();
                #endregion

                return blob.Uri.ToString();
            } catch (Exception) {
                return "";
            }
        }

        private static Image ResizeImage(Image img, Size size) {
            int srcWidth = img.Width;
            int srcHeight = img.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)srcWidth);
            nPercentH = ((float)size.Height / (float)srcHeight);
            if (nPercentH < nPercentW) {
                nPercent = nPercentH;
            } else {
                nPercent = nPercentW;
            }

            int destWidth = (int)(srcWidth * nPercent);
            int destHeight = (int)(srcHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            
            g.DrawImage(img, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        public static List<GroupedModule> GetProfileModules(int profileID = 0) {
            try {
                List<GroupedModule> mods = new List<GroupedModule>();
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();

                mods = (from m in db.Modules
                        where m.parentID.Equals(0)
                        select new GroupedModule {
                            id = m.id,
                            name = m.name,
                            path = m.path,
                            alt_text = m.alt_text,
                            hasAccess = ((from pm1 in db.ProfileModules
                                         where pm1.profileID.Equals(profileID) && pm1.moduleID.Equals(m.id)
                                         select pm1.id).Count() > 0)?1:0,
                            subs = (from m2 in db.Modules
                                    join pm2 in db.ProfileModules on m2.id equals pm2.moduleID
                                    where m2.parentID.Equals(m.id) && pm2.profileID.Equals(profileID)
                                    select m2).ToList<Module>(),
                            inMenu = m.inMenu,
                            image = m.image
                        }).OrderBy(x => x.name).ToList<GroupedModule>();
                return mods;
            } catch (Exception) {
                return new List<GroupedModule>();
            }
        }

        public static List<GroupedModule> GetModules() {
            List<GroupedModule> mods = new List<GroupedModule>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();

            mods = (from m in db.Modules
                    where m.parentID.Equals(0)
                    select new GroupedModule {
                        id = m.id,
                        name = m.name,
                        path = m.path,
                        parentID = m.parentID,
                        alt_text = m.alt_text,
                        inMenu = m.inMenu,
                        subs = db.Modules.Where(x => x.parentID.Equals(m.id)).ToList<Module>()
                    }).OrderBy(x => x.name).ToList<GroupedModule>();
            return mods;
        }

        public static void AddModules(int id, List<int> modules) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<ProfileModule> new_mods = new List<ProfileModule>();
                foreach (int m in modules) {
                    // Make sure we don't have a record for this user that ties them to the looped module
                    ProfileModule pm = db.ProfileModules.Where(x => x.moduleID.Equals(m) && x.profileID.Equals(id)).FirstOrDefault<ProfileModule>();
                    if (pm == null) {
                        ProfileModule pm_new = new ProfileModule {
                            moduleID = m,
                            profileID = id
                        };
                        new_mods.Add(pm_new);
                    }
                }
                db.ProfileModules.InsertAllOnSubmit<ProfileModule>(new_mods);
                db.SubmitChanges();
            } catch (Exception) {
                throw new Exception();
            }
        }

        // Remove the relationship between the profile id and all modules
        public static void DeleteProfileModules(int id) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<ProfileModule> pms = new List<ProfileModule>();

                pms = db.ProfileModules.Where(x => x.profileID.Equals(id)).ToList<ProfileModule>();

                db.ProfileModules.DeleteAllOnSubmit<ProfileModule>(pms);
                db.SubmitChanges();
            } catch (Exception) {
                throw new Exception();
            }
        }

        public static void DeleteProfile(int id) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();

            // Get the profile record
            Profile p = db.Profiles.Where(x => x.id.Equals(id)).FirstOrDefault<Profile>();

            // Get the modules for this profile
            List<ProfileModule> mods = db.ProfileModules.Where(x => x.id.Equals(p.id)).ToList<ProfileModule>();
            db.ProfileModules.DeleteAllOnSubmit<ProfileModule>(mods);

            // Delete the profile image
            DeleteProfileImage(p.username, p.id);

            // Delete the profile
            db.Profiles.DeleteOnSubmit(p);
            
            db.SubmitChanges();
        }

        private static void AlertNewUser(Profile p, string pass) {
            try {
                if (pass == null || pass.Length == 0) { throw new Exception(); }

                StringBuilder sb = new StringBuilder();
                Settings settings = new Settings();

                sb.Append("<p>A new account with this e-mail address has been created for the " + settings.Get("SiteName") + " Administrative Console.</p>");
                sb.Append("<hr />");
                sb.Append("<p>To <a href='" + settings.Get("SiteURL") + "' title='Login' target='_blank'>login</a> to the " + settings.Get("SiteName") + " Administrative Console, please use the information provided below.</p>");
                sb.Append("<span>The username is: <strong>" + p.username + "</strong></span><br />");
                sb.Append("<span>The new password is: <strong>" + pass + "</strong> ( You will be able to change this once logged in )</span><br />");
                sb.Append("<hr /><br />");
                sb.Append("<p style='font-size:11px'>If you feel this was a mistake please disregard this e-mail.</p>");

                string[] tos = {p.email};
                UDF.SendEmail(tos, "Account successfully created for " + settings.Get("SiteName") + " Administrative Console", true, sb.ToString());
            } catch (Exception e) {
                string h = e.Message;
            }
        }
    }
}