using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommercePlatform.Models;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.Linq;

namespace EcommercePlatform {
    partial class Theme {

        public int getTheme() {
            HttpCookie activeTheme = new HttpCookie("activetheme");
            activeTheme = HttpContext.Current.Request.Cookies.Get("activetheme");
            int themeid = 0;

            if (activeTheme != null && activeTheme.Value != null) {
                int inttest;
                if (int.TryParse(activeTheme.Value, out inttest)) {
                    themeid = Convert.ToInt32(activeTheme.Value);
                }
            } else {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                themeid = db.Themes.Where(x => x.active.Equals(true)).Select(x => x.ID).FirstOrDefault();
                activeTheme = new HttpCookie("activetheme");
                activeTheme.Value = themeid.ToString();
                activeTheme.Expires = DateTime.Now.AddHours(1);
                HttpContext.Current.Response.Cookies.Add(activeTheme);
            }
            return themeid;
        }

        private Dictionary<int,List<ThemeFile>> getBaseFiles() {
            Dictionary<int, List<ThemeFile>> files = new Dictionary<int, List<ThemeFile>>();
            List<ThemeFile> basefiles = new List<ThemeFile>();
            int themeID = getTheme();
            if (HttpContext.Current.Session["basefiles"] != null && themeID == getSessionTheme()) {
                basefiles = (List<ThemeFile>)HttpContext.Current.Session["basefiles"];
            } else {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                basefiles = db.Themes.Where(x => x.ID.Equals(themeID)).SelectMany(x => x.ThemeFiles.Where(y => y.ThemeArea.controller.ToLower().Equals("base"))).OrderBy(x => x.ThemeFileTypeID).ThenBy(x => x.renderOrder).ToList();
                HttpContext.Current.Session["basefiles"] = basefiles;
            }
            foreach(ThemeFile file in basefiles) {
                if(!files.Keys.Contains(file.ThemeFileTypeID)) {
                    List<ThemeFile> sublist = new List<ThemeFile>();
                    sublist.Add(file);
                    files.Add(file.ThemeFileTypeID,sublist);
                } else {
                    files[file.ThemeFileTypeID].Add(file);
                }
            }
            return files;
        }

        public Dictionary<int, List<ThemeFile>> getFiles(string controller) {
            Dictionary<int, List<ThemeFile>> files = getBaseFiles();
            List<ThemeFile> themefiles = new List<ThemeFile>();
            string keyname = controller + "files";
            int themeID = getTheme();
            if (HttpContext.Current.Session[keyname] != null && themeID == getSessionTheme()) {
                themefiles = (List<ThemeFile>)HttpContext.Current.Session[keyname];
            } else {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                themefiles = db.Themes.Where(x => x.ID.Equals(themeID)).SelectMany(x => x.ThemeFiles.Where(y => y.ThemeArea.controller.ToLower().Equals(controller.ToLower()))).OrderBy(x => x.ThemeFileTypeID).ThenBy(x => x.renderOrder).ToList();
                HttpContext.Current.Session[keyname] = themefiles;
                HttpContext.Current.Session["activetheme"] = themeID;
            }
            foreach (ThemeFile file in themefiles) {
                if (!files.Keys.Contains(file.ThemeFileTypeID)) {
                    List<ThemeFile> sublist = new List<ThemeFile>();
                    sublist.Add(file);
                    files.Add(file.ThemeFileTypeID, sublist);
                } else {
                    files[file.ThemeFileTypeID].Add(file);
                }
            }
            return files;
        }

        private int getSessionTheme() {
            int sessionTheme = 0;
            if (HttpContext.Current.Session["activetheme"] != null) {
                sessionTheme = (int)HttpContext.Current.Session["activetheme"];
            }
            return sessionTheme;
        }

    }

}