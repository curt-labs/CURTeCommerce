using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace EcommercePlatform.Models
{
    public class PostModel
    {
        public static List<BlogPost> GetAll()
        {
            try
            {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<BlogPost> posts = new List<BlogPost>();

                posts = db.BlogPosts.Where(x => x.active == true).OrderBy(x => x.publishedDate).OrderBy(x => x.createdDate).ToList<BlogPost>();

                return posts;
            }
            catch (Exception e)
            {
                return new List<BlogPost>();
            }
        }

        public static List<PostWithCategories> GetAllPublished(int page = 1, int pageSize = 5)
        {
            try
            {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<PostWithCategories> posts = new List<PostWithCategories>();

                posts = (from p in db.BlogPosts
                         where p.publishedDate.Value <= DateTime.Now && p.active.Equals(true)
                         orderby p.publishedDate descending
                         select new PostWithCategories
                         {
                             blogPostID = p.blogPostID,
                             post_title = p.post_title,
                             post_text = p.post_text,
                             slug = p.slug,
                             publishedDate = p.publishedDate,
                             createdDate = p.createdDate,
                             lastModified = p.lastModified,
                             keywords = p.keywords,
                             meta_title = p.meta_title,
                             meta_description = p.meta_description,
                             active = p.active,
                             author = GetAuthor(p.profileID),
                             categories = (from c in db.BlogCategories join pc in db.BlogPost_BlogCategories on c.blogCategoryID equals pc.blogCategoryID where pc.blogPostID.Equals(p.blogPostID) select c).ToList<BlogCategory>(),
                             comments = (from cm in db.Comments where cm.blogPostID.Equals(p.blogPostID) && cm.active.Equals(true) && cm.approved.Equals(true) select cm).ToList<Comment>()
                         }).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return posts;
            }
            catch (Exception e)
            {
                return new List<PostWithCategories>();
            }
        }

        public static List<PostWithCategories> GetSitemap() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<PostWithCategories> posts = new List<PostWithCategories>();

                posts = (from p in db.BlogPosts
                         where p.publishedDate.Value <= DateTime.Now && p.active.Equals(true)
                         orderby p.publishedDate descending
                         select new PostWithCategories {
                             blogPostID = p.blogPostID,
                             post_title = p.post_title,
                             post_text = p.post_text,
                             slug = p.slug,
                             publishedDate = p.publishedDate,
                             createdDate = p.createdDate,
                             lastModified = p.lastModified,
                             keywords = p.keywords,
                             meta_title = p.meta_title,
                             meta_description = p.meta_description,
                             active = p.active,
                             author = GetAuthor(p.profileID),
                             categories = (from c in db.BlogCategories join pc in db.BlogPost_BlogCategories on c.blogCategoryID equals pc.blogCategoryID where pc.blogPostID.Equals(p.blogPostID) select c).ToList<BlogCategory>(),
                             comments = (from cm in db.Comments where cm.blogPostID.Equals(p.blogPostID) && cm.active.Equals(true) && cm.approved.Equals(true) select cm).ToList<Comment>()
                         }).ToList();

                return posts;
            } catch (Exception e) {
                return new List<PostWithCategories>();
            }
        }
        
        public static List<PostWithCategories> GetAllPublishedByCategory(string name = "", int page = 1, int pageSize = 5) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<PostWithCategories> posts = new List<PostWithCategories>();

                posts = (from p in db.BlogPosts
                         join pca in db.BlogPost_BlogCategories on p.blogPostID equals pca.blogPostID
                         join ca in db.BlogCategories on pca.blogCategoryID equals ca.blogCategoryID
                         where p.publishedDate.Value <= DateTime.Now && p.active.Equals(true) && ca.slug.Equals(name)
                         orderby p.publishedDate descending
                         select new PostWithCategories {
                             blogPostID = p.blogPostID,
                             post_title = p.post_title,
                             post_text = p.post_text,
                             slug = p.slug,
                             publishedDate = p.publishedDate,
                             createdDate = p.createdDate,
                             lastModified = p.lastModified,
                             keywords = p.keywords,
                             meta_title = p.meta_title,
                             meta_description = p.meta_description,
                             active = p.active,
                             author = GetAuthor(p.profileID),
                             categories = (from c in db.BlogCategories join pc in db.BlogPost_BlogCategories on c.blogCategoryID equals pc.blogCategoryID where pc.blogPostID.Equals(p.blogPostID) select c).ToList<BlogCategory>(),
                             comments = (from cm in db.Comments where cm.blogPostID.Equals(p.blogPostID) && cm.active.Equals(true) && cm.approved.Equals(true) select cm).ToList<Comment>()
                         }).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return posts;
            } catch (Exception e) {
                return new List<PostWithCategories>();
            }
        }

        public static List<PostWithCategories> GetAllPublishedByDate(string month = "", string year = "", int page = 1, int pageSize = 5) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<PostWithCategories> posts = new List<PostWithCategories>();
                DateTime startDate = Convert.ToDateTime(month + " 1, " + year);
                DateTime endDate = startDate.AddMonths(1);

                posts = (from p in db.BlogPosts
                         where p.publishedDate.Value <= endDate && p.publishedDate.Value >= startDate && p.active.Equals(true)
                         orderby p.publishedDate descending
                         select new PostWithCategories {
                             blogPostID = p.blogPostID,
                             post_title = p.post_title,
                             post_text = p.post_text,
                             slug = p.slug,
                             publishedDate = p.publishedDate,
                             createdDate = p.createdDate,
                             lastModified = p.lastModified,
                             keywords = p.keywords,
                             meta_title = p.meta_title,
                             meta_description = p.meta_description,
                             active = p.active,
                             author = GetAuthor(p.profileID),
                             categories = (from c in db.BlogCategories join pc in db.BlogPost_BlogCategories on c.blogCategoryID equals pc.blogCategoryID where pc.blogPostID.Equals(p.blogPostID) select c).ToList<BlogCategory>(),
                             comments = (from cm in db.Comments where cm.blogPostID.Equals(p.blogPostID) && cm.active.Equals(true) && cm.approved.Equals(true) select cm).ToList<Comment>()
                         }).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return posts;
            } catch (Exception e) {
                return new List<PostWithCategories>();
            }
        }

        public static int CountAllPublished() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                int count = 0;
                count = (from p in db.BlogPosts
                         where p.publishedDate.Value <= DateTime.Now && p.active.Equals(true)
                         select p.blogPostID
                         ).Count();
                return count;
            } catch (Exception e) {
                return 0;
            }
        }

        public static int CountAllPublishedByDate(string month = "", string year = "") {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                DateTime startDate = Convert.ToDateTime(month + " 1, " + year);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                int count = 0;
                count = (from p in db.BlogPosts
                         where p.publishedDate.Value <= endDate && p.publishedDate.Value >= startDate && p.active.Equals(true)
                         select p.blogPostID
                         ).Count();
                return count;
            } catch (Exception e) {
                return 0;
            }
        }

        public static DateTime GetLatestPublishedDate() {
            DateTime latest = DateTime.Now;
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                latest = (from p in db.BlogPosts
                         where p.publishedDate.Value != null && p.active.Equals(true)
                         orderby p.publishedDate descending
                         select (DateTime)p.publishedDate).Single();
            } catch {}
            return latest;
        }

        public static DateTime GetLatestPublishedDateByCategory(string name = "") {
            DateTime latest = DateTime.Now;
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                latest = (from p in db.BlogPosts
                          join pca in db.BlogPost_BlogCategories on p.blogPostID equals pca.blogPostID
                          join c in db.BlogCategories on pca.blogCategoryID equals c.blogCategoryID
                          where p.publishedDate.Value != null && p.active.Equals(true) && c.slug.Equals(name)
                          orderby p.publishedDate descending
                          select (DateTime)p.publishedDate).Single();
            } catch { }
            return latest;
        }
        
        public static int CountAllPublishedByCategory(string name = "") {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                int count = 0;
                count = (from p in db.BlogPosts
                         join pca in db.BlogPost_BlogCategories on p.blogPostID equals pca.blogPostID
                         join c in db.BlogCategories on pca.blogCategoryID equals c.blogCategoryID
                         where p.publishedDate.Value <= DateTime.Now && p.active.Equals(true) && c.slug.Equals(name)
                         select p.blogPostID
                         ).Count();
                return count;
            } catch (Exception e) {
                return 0;
            }
        }

        public static PostWithCategories Get(string date = "", string title = "")
        {
            try {
                DateTime post_date = Convert.ToDateTime(date);
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                PostWithCategories post = new PostWithCategories();
                post = (from p in db.BlogPosts
                         where p.slug.Equals(title) && Convert.ToDateTime(p.publishedDate).Day.Equals(post_date.Day)
                         && Convert.ToDateTime(p.publishedDate).Year.Equals(post_date.Year) && Convert.ToDateTime(p.publishedDate).Month.Equals(post_date.Month)
                         select new PostWithCategories
                         {
                             blogPostID = p.blogPostID,
                             post_title = p.post_title,
                             post_text = p.post_text,
                             slug = p.slug,
                             publishedDate = p.publishedDate,
                             createdDate = p.createdDate,
                             lastModified = p.lastModified,
                             meta_title = p.meta_title,
                             meta_description = p.meta_description,
                             active = p.active,
                             author = GetAuthor(p.profileID),
                             categories = (from c in db.BlogCategories join pc in db.BlogPost_BlogCategories on c.blogCategoryID equals pc.blogCategoryID where pc.blogPostID.Equals(p.blogPostID) select c).ToList<BlogCategory>(),
                             comments = (from cm in db.Comments where cm.blogPostID.Equals(p.blogPostID) && cm.active.Equals(true) && cm.approved.Equals(true) select cm).ToList<Comment>()
                         }).First<PostWithCategories>();

                return post;
            } catch (Exception e) {
                return new PostWithCategories();
            }
        }

        public static BlogPost GetById(int id = 0)
        {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                BlogPost post = new BlogPost();
                post = (from p in db.BlogPosts
                        where p.blogPostID.Equals(id)
                        select p).First<BlogPost>();

                return post;
            } catch {
                return new BlogPost();
            }
        }

        public static void Delete(int id = 0)
        {
            try
            {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                BlogPost p = db.BlogPosts.Where(x => x.blogPostID == id).FirstOrDefault<BlogPost>();
                p.active = false;
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static Profile GetAuthor(int id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            return (from p in db.Profiles where p.id.Equals(id) select p).First<Profile>();
        }
    }
    
    public class PostWithCategories : BlogPost
    {
        public Profile author { get; set; }
        public List<EcommercePlatform.BlogCategory> categories { get; set; }
        public List<Comment> comments { get; set; }
    }

}