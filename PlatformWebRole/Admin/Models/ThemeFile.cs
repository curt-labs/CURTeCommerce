using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Admin.Models;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.Linq;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace Admin {
    partial class ThemeFile {
        public string content { get; set; }

        public List<ThemeFile> GetAll(int id) {
            List<ThemeFile> files = new List<ThemeFile>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            files = db.ThemeFiles.Where(x => x.themeID.Equals(id)).OrderBy(x => x.ThemeFileTypeID).ThenBy(x => x.renderOrder).ToList();
            return files;
        }

        public List<ThemeFile> GetAllByArea(int themeID, int areaID) {
            List<ThemeFile> files = new List<ThemeFile>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            files = db.ThemeFiles.Where(x => x.themeID.Equals(themeID) && x.themeAreaID.Equals(areaID)).ToList();
            return files;
        }

        public ThemeFile Get(int id) {
            ThemeFile file = new ThemeFile();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            file = db.ThemeFiles.Where(x => x.ID.Equals(id)).FirstOrDefault();
            Uri filepath = new Uri(file.filePath);

            CloudBlockBlob blob = BlobManagement.GetOrCreateBlob(filepath.LocalPath);
            MemoryStream datastream = new MemoryStream();
            blob.DownloadToStream(datastream);
            byte[] databytes = datastream.ToArray();
            file.content = Encoding.Default.GetString(databytes);
            return file;
        }

        public string name() {
            string name = "";
            if(this.filePath != null && this.filePath != "") {
                name = filePath.Split('/').ToList().Last();
            }
            return name;
        }

        public ThemeFile Save(int fileID, int themeID, int areaID, int typeID, string data, string name = "") {
            ThemeFile file = new ThemeFile();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            data = data.Replace("\n\r", Environment.NewLine).Trim();
            string localpath = "";
            if(String.IsNullOrWhiteSpace(name)) {
                return file;
            }
            ThemeFileType type = new ThemeFileType().Get(typeID);
            if(name.Trim().ToLower().IndexOf(type.extension.ToLower()) == -1) {
                name = name.Trim() + type.extension;
            }
            if (fileID > 0) {
                // prior file
                file = db.ThemeFiles.Where(x => x.ID.Equals(fileID)).FirstOrDefault();
                if (file.ID > 0) {
                    // ex: http://curtplatform.blob.core.windows.net/assets/curt-theme.jpg
                    Uri filepath = new Uri(file.filePath);
                    localpath = filepath.LocalPath;
                    file.lastModified = DateTime.UtcNow;
                }
            } else {
                // new file
                localpath = "/themes/" + themeID + "/" + areaID + "/" + name;
            }

            // get blob from blob store
            CloudBlockBlob blob = BlobManagement.GetOrCreateBlob(localpath);

            // upload new data to blob
            byte[] databytes = Encoding.ASCII.GetBytes(data);
            MemoryStream datastream = new MemoryStream(databytes);
            blob.UploadFromStream(datastream);

            if (fileID == 0) {
                // create new file
                file = new ThemeFile {
                    dateAdded = DateTime.UtcNow,
                    lastModified = DateTime.UtcNow,
                    filePath = blob.Uri.OriginalString,
                    themeID = themeID,
                    themeAreaID = areaID,
                    ThemeFileTypeID = typeID,
                    renderOrder = (db.ThemeFiles.Where(x => x.themeID.Equals(themeID) && x.themeAreaID.Equals(areaID) && x.ThemeFileTypeID.Equals(typeID)).OrderByDescending(x => x.renderOrder).Select(x => x.renderOrder).FirstOrDefault() + 1)
                };
                db.ThemeFiles.InsertOnSubmit(file);
            } else {
                // check file name if needs changing
                string filename = file.filePath.Split('/').ToList().Last();
                if (filename.Trim().ToLower() != name.Trim().ToLower()) {
                    List<string> frags = blob.Uri.OriginalString.Split('/').ToList();
                    frags.RemoveAt(frags.Count - 1);
                    frags.Add(name.Trim());
                    Uri newFile = new Uri(String.Join("/", frags.ToArray()));
                    // filename needs to change
                    CloudBlockBlob newblob = BlobManagement.RenameFile(blob.Uri, newFile);
                    file.filePath = newblob.Uri.OriginalString;
                }
            }
            db.SubmitChanges();
            return file;
        }

    }


}
