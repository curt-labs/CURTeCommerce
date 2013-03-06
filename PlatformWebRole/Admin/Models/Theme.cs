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
    partial class Theme {

        public List<Theme> GetAll() {
            List<Theme> themes = new List<Theme>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            themes = db.Themes.OrderBy(x => x.name).ToList();
            return themes;
        }

        public Theme Get(int id = 0) {
            Theme theme = new Theme();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            theme = db.Themes.Where(x => x.ID.Equals(id)).FirstOrDefault();
            return theme;
        }

        public void Save(string name, string screenshot = "") {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Theme theme = new Theme();
            if (this.ID > 0)
                theme = db.Themes.Where(x => x.ID.Equals(this.ID)).FirstOrDefault();
            else
                theme = new Theme();
            theme.name = name;
            theme.screenshot = (string.IsNullOrWhiteSpace(screenshot)) ? null : screenshot;
            if (theme.ID == 0) {
                theme.dateAdded = DateTime.UtcNow;
                theme.active = false;
                db.Themes.InsertOnSubmit(theme);
            }
            db.SubmitChanges();
            this.ID = theme.ID;
        }

        public bool Delete(int id) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Theme theme = db.Themes.Where(x => x.ID.Equals(id)).FirstOrDefault();
            if (theme != null && theme.ID > 0) {
                foreach(ThemeFile file in theme.ThemeFiles) {
                    new ThemeFile().Delete(file.ID);
                }
                db.ThemeFiles.DeleteAllOnSubmit(theme.ThemeFiles);
                db.Themes.DeleteOnSubmit(theme);
                db.SubmitChanges();
                return true;
            }
            return false;
        }

        public bool Activate(int id) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Theme theme = db.Themes.Where(x => x.ID.Equals(id)).FirstOrDefault();
            if (theme != null && theme.ID > 0) {
                List<Theme> themes = db.Themes.Where(x => x.active == true).ToList();
                foreach (Theme t in themes) {
                    t.active = false;
                }
                theme.active = true;
                db.SubmitChanges();

                // adjust cookies so theme shows up immediately
                HttpCookie activeTheme = new HttpCookie("activetheme");
                activeTheme = new HttpCookie("activetheme");
                activeTheme.Value = theme.ID.ToString();
                activeTheme.Expires = DateTime.Now.AddHours(1);
                HttpContext.Current.Response.Cookies.Add(activeTheme);
                return true;
            }
            return false;
        }

        public Theme Duplicate(int id) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Theme theme = db.Themes.Where(x => x.ID.Equals(id)).FirstOrDefault();
            Theme newtheme = new Theme();
            if (theme != null && theme.ID > 0) {
                newtheme = new Theme {
                    screenshot = theme.screenshot,
                    name = "Copy of " + theme.name,
                    active = false,
                    dateAdded = DateTime.UtcNow
                };
                db.Themes.InsertOnSubmit(newtheme);
                db.SubmitChanges();
                foreach (ThemeFile file in theme.ThemeFiles) {
                    file.Duplicate(newtheme.ID);
                }
            }
            return newtheme;
        }
    }


}
