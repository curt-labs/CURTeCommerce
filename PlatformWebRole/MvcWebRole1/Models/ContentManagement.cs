using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommercePlatform.Models {
    public class ContentManagement {

        internal static List<string> GetPageTitles() {
            try {
                List<string> titles = new List<string>();
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();

                titles = db.ContentPages.Select(x => x.Title).ToList<string>();

                return titles;
            } catch (Exception) {
                return new List<string>();
            }
        }

        internal static int GetPageID(string title = "") {
            try {
                int id = 0;
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                id = db.ContentPages.Where(x => x.Title.ToUpper().Equals(title.ToUpper())).Select(x => x.ID).FirstOrDefault<int>();
                return id;
            } catch (Exception) {
                return 0;
            }
        }

        internal static ContentPage GetPage(int id) {
            try {
                ContentPage page = new ContentPage();
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                page = db.ContentPages.Where(x => x.ID.Equals(id)).FirstOrDefault<ContentPage>();

                return page;
            } catch (Exception) {
                return null;
            }
        }

        internal static ContentPage GetPageByTitle(string title) {
            try {
                ContentPage page = new ContentPage();
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                page = db.ContentPages.Where(x => x.Title.Trim().ToUpper().Equals(title.Trim().ToUpper())).FirstOrDefault<ContentPage>();

                return page;
            } catch (Exception) {
                return null;
            }
        }

    }

}