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
    partial class ThemeFileType {

        public List<ThemeFileType> GetAll() {
            List<ThemeFileType> filetypes = new List<ThemeFileType>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            filetypes = db.ThemeFileTypes.OrderBy(x => x.name).ToList();
            return filetypes;
        }

        public ThemeFileType Get(int id = 0) {
            ThemeFileType type = new ThemeFileType();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            type = db.ThemeFileTypes.Where(x => x.ID.Equals(id)).FirstOrDefault();
            return type;
        }

    }


}
