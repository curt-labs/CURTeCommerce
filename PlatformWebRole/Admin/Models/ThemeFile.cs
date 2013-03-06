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
                if (db.ThemeFiles.Any(x => x.themeID.Equals(themeID) && x.ThemeFileTypeID.Equals(typeID) && x.themeAreaID.Equals(areaID) && x.filePath.ToLower().Contains(localpath.ToLower()))) {
                    return file;
                }
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

        public bool Delete(int id) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            ThemeFile file = db.ThemeFiles.Where(x => x.ID.Equals(id)).FirstOrDefault();
            if (file != null && file.ID > 0) {
                Uri filepath = new Uri(file.filePath);
                BlobManagement.DeleteFile(filepath);
                db.ThemeFiles.DeleteOnSubmit(file);
                db.SubmitChanges();
                return true;
            }
            return false;
        }

        public void Duplicate(int themeID) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Uri original = new Uri(this.filePath);
            string newpath = original.Scheme + "://" + original.Host + "/themes/" + themeID + "/" + this.themeAreaID + "/" + original.Segments.ToList().Last();
            Uri newfile = new Uri(newpath);
            CloudBlockBlob blob = BlobManagement.DuplicateFile(original, newfile);
            ThemeFile file = new ThemeFile {
                filePath = blob.Uri.OriginalString,
                dateAdded = DateTime.UtcNow,
                lastModified = DateTime.UtcNow,
                renderOrder = this.renderOrder,
                themeAreaID = this.themeAreaID,
                ThemeFileTypeID = this.ThemeFileTypeID,
                themeID = themeID
            };
            db.ThemeFiles.InsertOnSubmit(file);
            db.SubmitChanges();
        }

        public void updateSort(List<string> files) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            for (int i = 0; i < files.Count; i++) {
                ThemeFile f = db.ThemeFiles.Where(x => x.ID.Equals(Convert.ToInt32(files[i]))).FirstOrDefault();
                if (f.ID > 0) {
                    f.renderOrder = i + 1;
                    db.SubmitChanges();
                }
            }
        }

    }


}
