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
    partial class ThemeArea {

        public List<ThemeArea> GetAll() {
            List<ThemeArea> themeareas = new List<ThemeArea>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            themeareas = db.ThemeAreas.OrderBy(x => x.name).ToList();
            return themeareas;
        }

        public ThemeArea Get(int id = 0) {
            ThemeArea area = new ThemeArea();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            area = db.ThemeAreas.Where(x => x.ID.Equals(id)).FirstOrDefault();
            return area;
        }
    }


}
