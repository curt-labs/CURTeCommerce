using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models
{
    public class BlogCommentModel
    {
        public static List<CommentWithPost> GetAll()
        {
            try
            {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<CommentWithPost> comments = new List<CommentWithPost>();

                comments = (from c in db.Comments
                        where c.active.Equals(true)
                        orderby c.approved, c.createdDate
                        select new CommentWithPost
                        {
                            commentID = c.commentID,
                            blogPostID = c.blogPostID,
                            name = c.name,
                            email = c.email,
                            comment_text = c.comment_text,
                            createdDate = c.createdDate,
                            approved = c.approved,
                            active = c.active,
                            post = (from p in db.BlogPosts where p.blogPostID.Equals(c.blogPostID) select p).First<BlogPost>()
                        }).ToList<CommentWithPost>();

                return comments;
            }
            catch (Exception e)
            {
                return new List<CommentWithPost>();
            }
        }

        public static List<Comment> GetAllByPost(int id = 0)
        {
            try
            {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<Comment> comments = db.Comments.Where(x => x.blogPostID == id).Where(x => x.active == true).OrderBy(x => x.createdDate).ToList<Comment>();
                return comments;
            }
            catch (Exception e)
            {
                return new List<Comment>();
            }
        }

        public static CommentWithPost Get(int id = 0)
        {
            try
            {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                CommentWithPost comment = new CommentWithPost();

                comment = (from c in db.Comments
                           where c.commentID.Equals(id)
                           select new CommentWithPost
                           {
                               commentID = c.commentID,
                               blogPostID = c.blogPostID,
                               name = c.name,
                               email = c.email,
                               comment_text = c.comment_text,
                               createdDate = c.createdDate,
                               approved = c.approved,
                               active = c.active,
                               post = (from p in db.BlogPosts where p.blogPostID.Equals(c.blogPostID) select p).First<BlogPost>()
                           }).First<CommentWithPost>();

                return comment;
            }
            catch (Exception e)
            {
                return new CommentWithPost();
            }
        }

        public static CommentNoPost GetNoPost(int id = 0) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                CommentNoPost comment = new CommentNoPost();

                comment = (from c in db.Comments
                           where c.commentID.Equals(id)
                           select new CommentNoPost {
                               commentID = c.commentID,
                               blogPostID = c.blogPostID,
                               name = c.name,
                               email = c.email,
                               comment_text = c.comment_text,
                               createdDate = String.Format("{0:M/dd/yyyy h:mm:ss tt}", c.createdDate),
                               approved = (c.approved) ? "Yes" : "No",
                               active = c.active
                           }).First<CommentNoPost>();

                return comment;
            } catch (Exception e) {
                return new CommentNoPost();
            }
        }

        public static void Approve(int id = 0)
        {
            try
            {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                Comment c = db.Comments.Where(x => x.commentID == id).FirstOrDefault<Comment>();
                c.approved = true;
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void Delete(int id = 0)
        {
            try
            {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                Comment c = db.Comments.Where(x => x.commentID == id).FirstOrDefault<Comment>();
                c.active = false;
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    public class CommentWithPost : Comment
    {
        public BlogPost post { get; set; }
    }

    public class CommentNoPost {
        public int commentID { get; set; }
        public int blogPostID { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string comment_text { get; set; }
        public string createdDate { get; set; }
        public string approved { get; set; }
        public bool active { get; set; }
    }
}