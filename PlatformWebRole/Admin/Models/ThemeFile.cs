using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Admin.Models;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.Linq;

namespace Admin {
    partial class ThemeFile {

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
            return file;
        }

    }


}
