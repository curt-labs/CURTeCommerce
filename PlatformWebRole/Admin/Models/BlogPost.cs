using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models {
    public class BlogPostModel {
        public static List<PostWithCategories> GetAll() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<PostWithCategories> posts = new List<PostWithCategories>();

                posts = (from p in db.BlogPosts
                        where p.active.Equals(true)
                        orderby p.publishedDate, p.createdDate
                        select new PostWithCategories {
                            blogPostID = p.blogPostID,
                            post_title = p.post_title,
                            slug = p.slug,
                            profileID = p.profileID,
                            post_text = p.post_text,
                            publishedDate = p.publishedDate,
                            createdDate = p.createdDate,
                            lastModified = p.lastModified,
                            meta_title = p.meta_title,
                            meta_description = p.meta_description,
                            active = p.active,
                            author = GetAuthor(p.profileID),
                            categories = (from c in db.BlogCategories join pc in db.BlogPost_BlogCategories on c.blogCategoryID equals pc.blogCategoryID where pc.blogPostID.Equals(p.blogPostID) select c).ToList<BlogCategory>(),
                            comments = (from cm in db.Comments where cm.blogPostID.Equals(p.blogPostID) && cm.approved.Equals(true) && cm.active.Equals(true) select cm).ToList<Comment>(),
                            mod_comments = (from cm in db.Comments where cm.blogPostID.Equals(p.blogPostID) && cm.approved.Equals(false) && cm.active.Equals(true) select cm).ToList<Comment>()
                        }).ToList<PostWithCategories>();

                return posts;
           } catch (Exception e) {
                return new List<PostWithCategories>();
            }
        }

        public static List<BlogPost> GetAllPublished() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<BlogPost> posts = new List<BlogPost>();

                posts = db.BlogPosts.Where(x => x.active == true).Where(x => x.publishedDate != null).OrderBy(x => x.publishedDate).ToList<BlogPost>();

                return posts;
            } catch (Exception e) {
                return new List<BlogPost>();
            }
        }

        public static PostWithCategories Get(int id = 0) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                PostWithCategories post = new PostWithCategories();
                post = (from p in db.BlogPosts
                         where p.blogPostID.Equals(id)
                         select new PostWithCategories {
                             blogPostID = p.blogPostID,
                             post_title = p.post_title,
                             slug = p.slug,
                             profileID = p.profileID,
                             post_text = p.post_text,
                             publishedDate = p.publishedDate,
                             createdDate = p.createdDate,
                             lastModified = p.lastModified,
                             keywords = p.keywords,
                             meta_title = p.meta_title,
                             meta_description = p.meta_description,
                             active = p.active,
                             author = GetAuthor(p.profileID),
                             categories = (from c in db.BlogCategories join pc in db.BlogPost_BlogCategories on c.blogCategoryID equals pc.blogCategoryID where pc.blogPostID.Equals(p.blogPostID) select c).ToList<BlogCategory>(),
                             comments = (from cm in db.Comments where cm.blogPostID.Equals(p.blogPostID) && cm.approved.Equals(true) && cm.active.Equals(true) select cm).ToList<Comment>(),
                             mod_comments = (from cm in db.Comments where cm.blogPostID.Equals(p.blogPostID) && cm.approved.Equals(false) && cm.active.Equals(true) select cm).ToList<Comment>()
                         }).First<PostWithCategories>();

                return post;
            } catch (Exception e) {
                return new PostWithCategories();
            }
        }

        public static int Save(PostWithCategories p, bool publish = false) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            BlogPost post = new BlogPost();
            if (p.blogPostID == 0) {
                post = new BlogPost {
                    profileID = p.profileID,
                    post_title = p.post_title,
                    slug = p.slug,
                    post_text = p.post_text,
                    createdDate = DateTime.Now,
                    lastModified = DateTime.Now,
                    meta_title = p.meta_title,
                    meta_description = p.meta_description,
                    keywords = p.keywords,
                    active = true
                };
                if (publish)
                    post.publishedDate = DateTime.Now;
                db.BlogPosts.InsertOnSubmit(post);
                db.SubmitChanges();

                if (p.categories.Count() > 0) {
                    foreach (BlogCategory cat in p.categories) {
                        if (cat.blogCategoryID != 0) {
                            try {
                                BlogPost_BlogCategory postcat = new BlogPost_BlogCategory {
                                    blogPostID = post.blogPostID,
                                    blogCategoryID = cat.blogCategoryID
                                };
                                db.BlogPost_BlogCategories.InsertOnSubmit(postcat);
                            } catch { }
                        }
                    }
                }
            } else {
                post = db.BlogPosts.Where(x => x.blogPostID == p.blogPostID).FirstOrDefault<BlogPost>();
                post.meta_title = p.meta_title;
                post.meta_description = p.meta_description;
                post.keywords = p.keywords;
                post.profileID = p.profileID;
                post.lastModified = DateTime.Now;
                post.post_title = p.post_title;
                post.slug = p.slug;
                post.post_text = p.post_text;

                if (publish) {
                    if (post.publishedDate == null) { post.publishedDate = DateTime.Now; }
                } else {
                    post.publishedDate = null;
                }

                List<BlogPost_BlogCategory> postcats = db.BlogPost_BlogCategories.Where(x => x.blogPostID == post.blogPostID).ToList<BlogPost_BlogCategory>();
                db.BlogPost_BlogCategories.DeleteAllOnSubmit<BlogPost_BlogCategory>(postcats);
                db.SubmitChanges();

                if (p.categories.Count() > 0) {
                    foreach (BlogCategory cat in p.categories) {
                        if (cat.blogCategoryID != 0) {
                            try {
                                BlogPost_BlogCategory postcat = new BlogPost_BlogCategory {
                                    blogPostID = post.blogPostID,
                                    blogCategoryID = cat.blogCategoryID
                                };
                                db.BlogPost_BlogCategories.InsertOnSubmit(postcat);
                            } catch { }
                        }
                    }
                }
            }
            db.SubmitChanges();

            return post.blogPostID;
        }
        public static void Delete(int id = 0) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<BlogPost_BlogCategory> postcats = db.BlogPost_BlogCategories.Where(x => x.blogPostID.Equals(id)).ToList<BlogPost_BlogCategory>();
                db.BlogPost_BlogCategories.DeleteAllOnSubmit(postcats);
                BlogPost p = db.BlogPosts.Where(x => x.blogPostID == id).FirstOrDefault<BlogPost>();
                p.active = false;
                db.SubmitChanges();
            } catch (Exception e) {
                throw e;
            }
        }

        private static Profile GetAuthor(int id = 0) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            return (from p in db.Profiles where p.id.Equals(id) select p).FirstOrDefault<Profile>();
        }
    }

    public class PostWithCategories : BlogPost {
        public Profile author { get; set; }
        public List<BlogCategory> categories { get; set; }
        public List<Comment> comments { get; set; }
        public List<Comment> mod_comments { get; set; }
    }
}