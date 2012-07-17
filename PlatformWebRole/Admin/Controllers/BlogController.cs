using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers
{
    public class BlogController : BaseController
    {
        //
        // GET: /Blog/


        // Start Post Methods 
        public ActionResult Index()
        {
            // Get all the posts
            List<PostWithCategories> posts = BlogPostModel.GetAll();
            ViewBag.posts = posts;

            List<BlogCategory> categories = BlogCategoryModel.GetAll();
            ViewBag.categories = categories;

            List<CommentWithPost> comments = BlogCommentModel.GetAll();
            ViewBag.comments = comments;

            return View();
        }

        [ValidateInput(false)]
        public ActionResult SavePost(List<string> categories, int id = 0, int profileid = 0, bool publish = false, string post_title = "", int contentID = 0, string content = "", string meta_title = "", string meta_description = "", string keywords = "") {
            string message = "Saved Successfully";
            PostWithCategories p = new PostWithCategories {
                blogPostID = id,
                profileID = profileid,
                post_title = post_title.Trim(),
                slug = UDF.GenerateSlug(post_title.Trim()),
                post_text = content.Trim(),
                createdDate = DateTime.Now,
                lastModified = DateTime.Now,
                meta_title = meta_title.Trim(),
                meta_description = meta_description.Trim(),
                keywords = keywords.Trim(),
                categories = BlogCategoryModel.GetList(categories),
                active = true
            };
            try {
                if (p.post_title.Length == 0) { throw new Exception("You must enter a title for the post"); }

                p.blogPostID = BlogPostModel.Save(p,publish);

            } catch (Exception e) {
                TempData["post"] = p;
                message = e.Message;
            }
            return RedirectToAction("EditPost", new { id = p.blogPostID, error = message });
        }

        public ActionResult EditPost(int id = 0, string error = "") {
            // Get all the Authors
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Profile> authors = db.Profiles.OrderBy(x => x.last).ToList<Profile>();
            ViewBag.authors = authors;

            List<BlogCategory> categories = BlogCategoryModel.GetAll();
            ViewBag.categories = categories;

            PostWithCategories post = ((PostWithCategories)TempData["post"] == null) ? BlogPostModel.Get(id) : (PostWithCategories)TempData["post"];
            ViewBag.post = post;
            ViewBag.error = error;

            return View();
        }

        public string DeletePost(int id = 0) {
            try {
                BlogPostModel.Delete(id);
                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }
        // End Post Methods

        // Start Category Methods
        public ActionResult EditCategory(int id = 0, string error = "") {
            BlogCategory category = BlogCategoryModel.Get(id);
            ViewBag.category = category;
            ViewBag.error = error;
            return View();
        }

        [NoValidation]
        public ActionResult SaveCategory(int id = 0, string name = "") {
            string message = "Saved Successfully!";
            try {
                if (name.Length == 0) { throw new Exception("You must enter a category name"); }

                id = BlogCategoryModel.Save(id, name);

            } catch (Exception e) {
                message = e.Message;
            }
            return RedirectToAction("EditCategory", new { id = id, error = message });
        }

        [NoValidation]
        public string DeleteCategory(int id = 0) {
            return BlogCategoryModel.Delete(id);            
        }        
        // End Category Methods

        // Start Comment Methods
        public ActionResult ViewComment(int id = 0) {
            CommentWithPost comment = BlogCommentModel.Get(id);
            ViewBag.comment = comment;

            return View();
        }

        [NoValidation]
        public string CommentJSON(int id = 0) {
            CommentNoPost comment = BlogCommentModel.GetNoPost(id);
            return Newtonsoft.Json.JsonConvert.SerializeObject(comment);
        }

        public ActionResult PostComments(int id = 0) {
            PostWithCategories post = BlogPostModel.Get(id);
            List<Comment> comments = BlogCommentModel.GetAllByPost(id);

            ViewBag.post = post;
            ViewBag.comments = comments;

            return View();
        }

        [NoValidation]
        public void ApproveComment(int id = 0) {
            try {
                BlogCommentModel.Approve(id);

            } catch { }
            Response.Redirect("/Admin/Blog#blogcomments");
        }

        [NoValidation]
        public string ApproveCommentAjax(int id = 0) {
            try {
                BlogCommentModel.Approve(id);
                return CommentJSON(id);
            } catch (Exception e) { return e.Message; }
        }

        [NoValidation]
        public string DeleteComment(int id = 0) {
            try {
                BlogCommentModel.Delete(id);
                return "[]";
            } catch (Exception e) {
                return e.Message;
            }
        }
        // End Comment Methods
    }
}
