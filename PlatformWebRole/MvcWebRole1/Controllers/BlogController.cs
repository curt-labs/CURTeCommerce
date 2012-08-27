using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace EcommercePlatform.Controllers {
    public class BlogController : BaseController {
        //

        public ActionResult Index(int page = 1, int pageSize = 5) {

            List<PostWithCategories> posts = PostModel.GetAllPublished(page,pageSize);
            ViewBag.posts = posts;

            int postcount = PostModel.CountAllPublished();
            ViewBag.total = postcount;

            decimal pagecount = Math.Ceiling(Convert.ToDecimal(postcount) / Convert.ToDecimal(pageSize));
            ViewBag.pagecount = Convert.ToInt32(pagecount);

            List<BlogCategory> categories = BlogCategoryModel.GetCategories();
            ViewBag.categories = categories;

            List<Archive> months = Archive.GetMonths();
            ViewBag.months = months;

            ViewBag.page = page;

            return View();
        }

        public ActionResult ViewPost(string date = "", string title = "")
        {

            PostWithCategories post = PostModel.Get(date,title);
            ViewBag.post = post;

            List<Archive> months = Archive.GetMonths();
            ViewBag.months = months;

            List<BlogCategory> categories = BlogCategoryModel.GetCategories();
            ViewBag.categories = categories;

            return View();
        }

        public ActionResult ViewCategory(string name = "", int page = 1, int pageSize = 5) {

            BlogCategory category = BlogCategoryModel.GetCategoryByName(name);
            ViewBag.category = category;

            List<PostWithCategories> posts = PostModel.GetAllPublishedByCategory(name, page, pageSize);
            ViewBag.posts = posts;

            List<Archive> months = Archive.GetMonths();
            ViewBag.months = months;

            List<BlogCategory> categories = BlogCategoryModel.GetCategories();
            ViewBag.categories = categories;

            int postcount = PostModel.CountAllPublishedByCategory(name);
            ViewBag.total = postcount;

            decimal pagecount = Math.Ceiling(Convert.ToDecimal(postcount) / Convert.ToDecimal(5));
            ViewBag.pagecount = Convert.ToInt32(pagecount);

            ViewBag.page = page;

            return View();
        }

        public ActionResult ViewArchive(string month = "", string year = "", int page = 1, int pageSize = 5) {

            List<PostWithCategories> posts = PostModel.GetAllPublishedByDate(month, year, page, pageSize);
            ViewBag.posts = posts;

            List<BlogCategory> categories = BlogCategoryModel.GetCategories();
            ViewBag.categories = categories;
            
            List<Archive> months = Archive.GetMonths();
            ViewBag.months = months;

            int postcount = PostModel.CountAllPublishedByDate(month, year);
            ViewBag.total = postcount;

            decimal pagecount = Math.Ceiling(Convert.ToDecimal(postcount) / Convert.ToDecimal(pageSize));
            ViewBag.pagecount = Convert.ToInt32(pagecount);
            
            ViewBag.month = month;
            ViewBag.year = year;
            ViewBag.page = page;

            return View();
        }

        public ActionResult Author(string name = "") {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            string dname = Server.UrlDecode(name);
            string fname = dname.Split('_')[0];
            string lname = dname.Split('_')[1];
            Profile author = db.Profiles.Where(x => x.first == fname).Where(x => x.last == lname).FirstOrDefault<Profile>();
            ViewBag.author = author;

            int count = db.BlogPosts.Where(x => x.profileID == author.id).Where(x => x.active == true).Count();
            if (count == 0) {
                return RedirectToAction("Index", "_404");
            }

            List<Archive> months = Archive.GetMonths();
            ViewBag.months = months;

            List<BlogCategory> categories = BlogCategoryModel.GetCategories();
            ViewBag.categories = categories;

            return View();
        }

        [ValidateInput(false)]
        public ActionResult Comment(int id = 0, string name = "", string email = "", string comment_text = "") {
            BlogPost post = PostModel.GetById(id);
            string postdate = String.Format("{0:M-d-yyyy}", post.publishedDate);
            try {
                string remoteip = (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null) ? Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : Request.ServerVariables["REMOTE_ADDR"];
                if (!(Models.ReCaptcha.ValidateCaptcha(Request.Form["recaptcha_challenge_field"], Request.Form["recaptcha_response_field"], remoteip))) {
                    throw new Exception("Recaptcha Validation Failed.");
                }
                if (id == 0) { throw new Exception("You must be on a blog post to add a comment"); }
                if (name == "") { throw new Exception("You must enter your name"); }
                if (email != "" & (!IsValidEmail(email))) { throw new Exception("Your email address is not a valid format."); }
                if (comment_text.Trim() == "") { throw new Exception("You must enter a comment"); }

                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                Comment comment = new Comment {
                    blogPostID = id,
                    name = name,
                    email = email,
                    comment_text = Regex.Replace(comment_text.Trim(),"<.*?>",string.Empty),
                    createdDate = DateTime.Now,
                    active = true,
                    approved = false
                };
                db.Comments.InsertOnSubmit(comment);
                db.SubmitChanges();
                string message = "Your comment has been submitted for approval.";
                return RedirectToRoute("BlogPost", new { message = message, title = post.slug, date = postdate });
            } catch (Exception e) {
                return RedirectToRoute("BlogPost", new { err = e.Message, title = post.slug, date = postdate, email = email, name = name, comment_text = comment_text });
            }
        }

        public void Feed(string type = "rss") {
            List<PostWithCategories> posts = PostModel.GetAllPublished(1, 10);
            Settings settings = ViewBag.settings;
            DateTime lastBuild = PostModel.GetLatestPublishedDate();
            if (type.ToLower() == "rss") {
                Response.ContentType = "text/xml";
                XDocument xml = new XDocument();
                XElement rss_container = new XElement("rss");
                XAttribute rss_version = new XAttribute("version", "2.0");
                rss_container.Add(rss_version);
                XElement rss_channel = new XElement("channel");
                rss_channel.Add(new XElement("title", settings.Get("SiteName") + " Blog"));
                rss_channel.Add(new XElement("link", settings.Get("SiteURL") + "blog"));
                rss_channel.Add(new XElement("description", settings.Get("BlogDescription")));
                rss_channel.Add(new XElement("copyright", "© " + String.Format("{0:yyyy}", DateTime.Now) + " " + settings.Get("SiteName")));
                rss_channel.Add(new XElement("language", "en-us"));
                rss_channel.Add(new XElement("lastBuildDate", String.Format("{0:ddd, dd MMM yyyy HH':'mm':'ss 'CST'}", lastBuild)));
                foreach (PostWithCategories post in posts) {
                    XElement item = new XElement("item");
                    item.Add(new XElement("title", post.post_title));
                    item.Add(new XElement("description", post.post_text));
                    item.Add(new XElement("link", "http://" + Request.Url.Host + ((Request.Url.Host.Contains("localhost")) ? ":" + Request.Url.Port.ToString() : "") + Url.RouteUrl("BlogPost", new { date = String.Format("{0:M-dd-yyyy}", post.publishedDate), title = post.slug })));
                    item.Add(new XElement("pubDate", String.Format("{0:ddd, dd MMM yyyy HH':'mm':'ss 'CST'}", post.publishedDate)));
                    item.Add(new XElement("author", post.author.first + " " + post.author.last));
                    foreach (BlogCategory category in post.categories) {
                        item.Add(new XElement("category", category.name));
                    }
                    rss_channel.Add(item);
                }

                rss_container.Add(rss_channel);
                xml.Add(rss_container);
                Response.Write(xml);
                Response.End();

            } else {
                Response.ContentType = "text/xml";
                XDocument xml = new XDocument();
                XElement feed_container = new XElement("{http://www.w3.org/2005/Atom}feed");
                feed_container.Add(new XElement("{http://www.w3.org/2005/Atom}title", settings.Get("SiteName") + " Blog"));
                feed_container.Add(new XElement("{http://www.w3.org/2005/Atom}subtitle", settings.Get("BlogDescription")));
                XElement selflink = new XElement("{http://www.w3.org/2005/Atom}link");
                selflink.Add(new XAttribute("href", settings.Get("SiteURL") + "blog/feed/atom"));
                selflink.Add(new XAttribute("rel", "self"));
                feed_container.Add(selflink);
                XElement bloglink = new XElement("{http://www.w3.org/2005/Atom}link");
                bloglink.Add(new XAttribute("href", settings.Get("SiteURL") + "blog/"));
                feed_container.Add(bloglink);
                feed_container.Add(new XElement("{http://www.w3.org/2005/Atom}id", settings.Get("SiteURL")));
                feed_container.Add(new XElement("{http://www.w3.org/2005/Atom}updated", String.Format("{0:yyyy-MM-dd'T'HH:mm:ss'Z'}", lastBuild.ToUniversalTime())));
                feed_container.Add(new XElement("{http://www.w3.org/2005/Atom}rights", "© " + String.Format("{0:yyyy}", DateTime.Now) + " " + settings.Get("SiteName")));
                foreach (PostWithCategories post in posts) {
                    XElement item = new XElement("{http://www.w3.org/2005/Atom}entry");
                    item.Add(new XElement("{http://www.w3.org/2005/Atom}title", post.post_title));
                    item.Add(new XElement("{http://www.w3.org/2005/Atom}summary", post.post_text));
                    XElement entrylink = new XElement("{http://www.w3.org/2005/Atom}link");
                    entrylink.Add(new XAttribute("href", "http://" + Request.Url.Host + ((Request.Url.Host.Contains("localhost")) ? ":" + Request.Url.Port.ToString() : "") + Url.RouteUrl("BlogPost", new { date = String.Format("{0:M-dd-yyyy}", post.publishedDate), title = post.slug })));
                    item.Add(entrylink);
                    item.Add(new XElement("{http://www.w3.org/2005/Atom}updated", String.Format("{0:yyyy-MM-dd'T'HH:mm:ss'Z'}", Convert.ToDateTime(post.publishedDate).ToUniversalTime())));
                    item.Add(new XElement("{http://www.w3.org/2005/Atom}author",
                            new XElement("{http://www.w3.org/2005/Atom}name", post.author.first + " " + post.author.last),
                            new XElement("{http://www.w3.org/2005/Atom}email", post.author.email)));
                    feed_container.Add(item);
                }

                xml.Add(feed_container);
                Response.Write(xml);
                Response.End();
            }
        }

        public void CategoryFeed(string name = "", string type = "") {
            BlogCategory category = BlogCategoryModel.GetCategoryByName(name);
            List<PostWithCategories> posts = PostModel.GetAllPublishedByCategory(name, 1, 10);
            DateTime lastBuild = PostModel.GetLatestPublishedDate();
            Settings settings = ViewBag.settings;
            if (category != null) {
                if (type.ToLower() == "rss") {
                    Response.ContentType = "text/xml";
                    XDocument xml = new XDocument();
                    XElement rss_container = new XElement("rss");
                    XAttribute rss_version = new XAttribute("version", "2.0");
                    rss_container.Add(rss_version);
                    XElement rss_channel = new XElement("channel");
                    rss_channel.Add(new XElement("title", settings.Get("SiteName") + " Blog - Posts in Category '" + category.name + "'"));
                    rss_channel.Add(new XElement("link", settings.Get("SiteURL") + "blog/category/" + name));
                    rss_channel.Add(new XElement("description", settings.Get("BlogDescription")));
                    rss_channel.Add(new XElement("copyright", "© " + String.Format("{0:yyyy}", DateTime.Now) + " " + settings.Get("SiteName")));
                    rss_channel.Add(new XElement("language", "en-us"));
                    rss_channel.Add(new XElement("lastBuildDate", String.Format("{0:ddd, dd MMM yyyy HH':'mm':'ss 'CST'}", lastBuild)));
                    foreach (PostWithCategories post in posts) {
                        XElement item = new XElement("item");
                        item.Add(new XElement("title", post.post_title));
                        item.Add(new XElement("description", post.post_text));
                        item.Add(new XElement("link", "http://" + Request.Url.Host + ((Request.Url.Host.Contains("localhost")) ? ":" + Request.Url.Port.ToString() : "") + Url.RouteUrl("BlogPost", new { date = String.Format("{0:M-dd-yyyy}", post.publishedDate), title = post.slug })));
                        item.Add(new XElement("pubDate", String.Format("{0:ddd, dd MMM yyyy HH':'mm':'ss 'CST'}", post.publishedDate)));
                        item.Add(new XElement("author", post.author.first + " " + post.author.last));
                        foreach (BlogCategory cat in post.categories) {
                            item.Add(new XElement("category", cat.name));
                        }
                        rss_channel.Add(item);
                    }

                    rss_container.Add(rss_channel);
                    xml.Add(rss_container);
                    Response.Write(xml);
                    Response.End();

                } else {
                    Response.ContentType = "text/xml";
                    XDocument xml = new XDocument();
                    XElement feed_container = new XElement("{http://www.w3.org/2005/Atom}feed");
                    feed_container.Add(new XElement("{http://www.w3.org/2005/Atom}title", settings.Get("SiteName") + " Blog - Posts in Category '" + category.name + "'"));
                    feed_container.Add(new XElement("{http://www.w3.org/2005/Atom}subtitle", settings.Get("BlogDescription")));
                    XElement selflink = new XElement("{http://www.w3.org/2005/Atom}link");
                    selflink.Add(new XAttribute("href", settings.Get("SiteURL") + "blog/category/" + name + "/feed/atom"));
                    selflink.Add(new XAttribute("rel", "self"));
                    feed_container.Add(selflink);
                    XElement bloglink = new XElement("{http://www.w3.org/2005/Atom}link");
                    bloglink.Add(new XAttribute("href", settings.Get("SiteURL") + "blog/category/" + name));
                    feed_container.Add(bloglink);
                    feed_container.Add(new XElement("{http://www.w3.org/2005/Atom}id", settings.Get("SiteURL")));
                    feed_container.Add(new XElement("{http://www.w3.org/2005/Atom}updated", String.Format("{0:yyyy-MM-dd'T'HH:mm:ss'Z'}", lastBuild.ToUniversalTime())));
                    feed_container.Add(new XElement("{http://www.w3.org/2005/Atom}rights", "© " + String.Format("{0:yyyy}", DateTime.Now) + " " + settings.Get("SiteName")));
                    foreach (PostWithCategories post in posts) {
                        XElement item = new XElement("{http://www.w3.org/2005/Atom}entry");
                        item.Add(new XElement("{http://www.w3.org/2005/Atom}title", post.post_title));
                        item.Add(new XElement("{http://www.w3.org/2005/Atom}summary", post.post_text));
                        XElement entrylink = new XElement("{http://www.w3.org/2005/Atom}link");
                        entrylink.Add(new XAttribute("href", "http://" + Request.Url.Host + ((Request.Url.Host.Contains("localhost")) ? ":" + Request.Url.Port.ToString() : "") + Url.RouteUrl("BlogPost", new { date = String.Format("{0:M-dd-yyyy}", post.publishedDate), title = post.slug })));
                        item.Add(entrylink);
                        item.Add(new XElement("{http://www.w3.org/2005/Atom}updated", String.Format("{0:yyyy-MM-dd'T'HH:mm:ss'Z'}", Convert.ToDateTime(post.publishedDate).ToUniversalTime())));
                        item.Add(new XElement("{http://www.w3.org/2005/Atom}author",
                                new XElement("{http://www.w3.org/2005/Atom}name", post.author.first + " " + post.author.last),
                                new XElement("{http://www.w3.org/2005/Atom}email", post.author.email)));
                        feed_container.Add(item);
                    }

                    xml.Add(feed_container);
                    Response.Write(xml);
                    Response.End();
                }
            }
        }

        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }
    }
}
