using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models
{
    public class BlogCategoryModel
    {
        public static List<BlogCategory> GetAll() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<BlogCategory> categories = new List<BlogCategory>();

                categories = db.BlogCategories.Where(x => x.active == true).ToList<BlogCategory>();

                return categories;
            } catch (Exception e) {
                return new List<BlogCategory>();
            }
        }

        public static BlogCategory Get(int id = 0) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                BlogCategory category = new BlogCategory();
                category = db.BlogCategories.Where(x => x.blogCategoryID == id).Where(x => x.active == true).FirstOrDefault<BlogCategory>();

                return category;
            } catch (Exception e) {
                return new BlogCategory();
            }
        }

        public static List<BlogCategory> GetList(List<string> categories = null) {
            List<BlogCategory> cats = new List<BlogCategory>();
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                
                foreach (string cat in categories) {
                    try {
                        BlogCategory c = db.BlogCategories.Where(x => x.blogCategoryID == Convert.ToInt32(cat)).Where(x => x.active == true).First<BlogCategory>();
                        cats.Add(c);
                    } catch {}
                }

            } catch {}
            return cats;
        }

        public static int Save(int id = 0, string name = "") {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            BlogCategory category = new BlogCategory();
            try {
                category = db.BlogCategories.Where(x => x.blogCategoryID == id).FirstOrDefault<BlogCategory>();
                category.name = name;
                category.slug = UDF.GenerateSlug(name);
            } catch {
                category = new BlogCategory {
                    name = name,
                    slug = UDF.GenerateSlug(name),
                    active = true
                };
                db.BlogCategories.InsertOnSubmit(category);
            }
            db.SubmitChanges();
            return category.blogCategoryID;
        }

        public static string Delete(int id = 0) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<BlogPost_BlogCategory> postcats = db.BlogPost_BlogCategories.Where(x => x.blogCategoryID.Equals(id)).ToList<BlogPost_BlogCategory>();
                db.BlogPost_BlogCategories.DeleteAllOnSubmit(postcats);
                db.SubmitChanges();
                BlogCategory category = (from c in db.BlogCategories
                                         where c.blogCategoryID.Equals(id)
                                         select c).FirstOrDefault<BlogCategory>();
                db.BlogCategories.DeleteOnSubmit(category);
                db.SubmitChanges();
                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }
    }
}