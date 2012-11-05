using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models {
    public class ContentManagement {

        internal static string[] fixed_pages = { "homepage", "newsletter", "contact", "faq", "privacy policy", "about us" };

        internal static List<string> GetFixedTitles() {
            List<string> pages = new List<string>(fixed_pages);
            return pages;
        }

        internal static void CheckFixed() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            foreach (string title in fixed_pages) {
                ContentPage page = db.ContentPages.Where(x => x.Title.ToLower().Equals(title.ToLower())).FirstOrDefault<ContentPage>();
                if (page == null) {
                    page = new ContentPage {
                        Title = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title),
                        content = ""
                    };
                    db.ContentPages.InsertOnSubmit(page);
                }
            }
            db.SubmitChanges();
        }

        internal static ContentPage GetParentPage(int pageID) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                ContentPage page = new ContentPage();

                page = (from cp in db.ContentPages
                        join cn in db.ContentNestings on cp.ID equals cn.parentID
                        where cn.pageID.Equals(pageID)
                        select cp).FirstOrDefault<ContentPage>();
                return page;
            } catch (Exception) {
                return new ContentPage();
            }
        }

        internal static ContentPage GetPage(int id) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.ContentPages.Where(x => x.ID.Equals(id)).FirstOrDefault<ContentPage>();
            } catch (Exception) {
                return new ContentPage();
            }
        }

        internal static StrippedContent GetStrippedPage(int id) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                StrippedContent page = (from c in db.ContentPages
                                        where c.ID.Equals(id)
                                        select new StrippedContent {
                                            ID = c.ID,
                                            Title = c.Title,
                                            parent = db.ContentNestings.Where(x => x.pageID.Equals(c.ID)).Select(x => new StrippedContent { ID = x.ContentPage.ID, Title = x.ContentPage.Title }).FirstOrDefault<StrippedContent>(),
                                            subpages = (from cn in c.ContentNestings
                                                        select new StrippedContent {
                                                            ID = cn.ContentPage.ID,
                                                            Title = cn.ContentPage.Title
                                                        }).ToList<StrippedContent>()
                                        }).FirstOrDefault<StrippedContent>();
                return page;
            } catch (Exception) {
                return new StrippedContent();
            }
        }

        internal static List<StrippedContent> GetAll() {
            List<StrippedContent> pages = new List<StrippedContent>();
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                pages = (from c in db.ContentPages
                         orderby c.Title
                         select new StrippedContent {
                             ID = c.ID,
                             Title = c.Title,
                             parent = db.ContentNestings.Where(x => x.pageID.Equals(c.ID)).Select(x => new StrippedContent{ ID = x.ContentPage.ID, Title = x.ContentPage.Title }).FirstOrDefault<StrippedContent>(),
                             subpages = (from cn in c.ContentNestings
                                         select new StrippedContent {
                                             ID = cn.ContentPage.ID,
                                             Title = cn.ContentPage.Title
                                         }).ToList<StrippedContent>()
                         }).ToList<StrippedContent>();
            } catch (Exception) { }
            return pages;
        }

        internal static ContentPage Add(ContentPage page) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();

            if (page == null) { throw new Exception("Page is null."); }
            if (page.Title == null || page.Title.Length == 0) { throw new Exception("Page title is required."); }
            if (page.ID > 0) {
                ContentPage existing = db.ContentPages.Where(x => x.ID.Equals(page.ID)).FirstOrDefault<ContentPage>();
                existing.Title = page.Title;
                existing.content = page.content;
                existing.metaTitle = (page.metaTitle.Trim() == "") ? null : page.metaTitle;
                existing.metaDescription = (page.metaDescription.Trim() == "") ? null : page.metaDescription;
                existing.visible = page.visible;
                db.SubmitChanges();
                page = existing;
            } else {
                db.ContentPages.InsertOnSubmit(page);
                db.SubmitChanges();
            }
            return page;
        }

        internal static void ClearParents(int id) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<ContentNesting> parents = new List<ContentNesting>();

            parents = db.ContentNestings.Where(x => x.pageID.Equals(id)).ToList<ContentNesting>();
            db.ContentNestings.DeleteAllOnSubmit<ContentNesting>(parents);
            db.SubmitChanges();
        }

        internal static void AddParent(int id, int parent) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();

                // Make sure there isn't already a listing for this
                ContentNesting exists = db.ContentNestings.Where(x => x.pageID.Equals(id) && x.parentID.Equals(parent)).FirstOrDefault<ContentNesting>();
                if (exists != null && exists.ID > 0) {
                    throw new Exception("Relationship exists.");
                }

                ContentNesting nest = new ContentNesting {
                    pageID = id,
                    parentID = parent
                };
                db.ContentNestings.InsertOnSubmit(nest);
                db.SubmitChanges();
            } catch (Exception) { }
        }

        internal static void RemoveParent(int id, int parent) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            ContentNesting nesting = new ContentNesting();
            nesting = db.ContentNestings.Where(x => x.parentID.Equals(parent) && x.pageID.Equals(id)).FirstOrDefault<ContentNesting>();

            db.ContentNestings.DeleteOnSubmit(nesting);
            db.SubmitChanges();
        }

        internal static void DeletePage(int id) {
            if (id == 0) { throw new Exception("Invalid refernece to ContentPage"); }
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            
            // Get and delete the menu options the reference this page
            List<ContentNesting> nestings = db.ContentNestings.Where(x => x.pageID.Equals(id) || x.parentID.Equals(id)).ToList<ContentNesting>();
            db.ContentNestings.DeleteAllOnSubmit<ContentNesting>(nestings);

            // Get and delete the ContentPage record
            ContentPage page = db.ContentPages.Where(x => x.ID.Equals(id)).FirstOrDefault<ContentPage>();
            db.ContentPages.DeleteOnSubmit(page);

            db.SubmitChanges();
        }
    }

    public class StrippedContent {
        public int ID { get; set; }
        public string Title { get; set; }
        public List<StrippedContent> subpages { get; set; }
        public StrippedContent parent { get; set; }
    }

}