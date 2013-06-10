using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class SitemapController : BaseController {
        //
        // GET: /Index/
        public async Task<ActionResult> Index() {
            var pcats = CURTAPI.GetParentCategoriesAsync();
            await Task.WhenAll(new Task[] { pcats });
            ViewBag.parent_cats = await pcats;

            // Get all the content pages
            List<ContentPage> contents = new List<ContentPage>();
            contents = ContentManagement.GetSitemap();
            ViewBag.contents = contents;

            // Get all the FaqTopics
            List<FaqTopic> topics = FaqTopic.GetAll();
            ViewBag.faqtopics = topics;

            // all the blog posts
            List<PostWithCategories> posts = PostModel.GetSitemap();
            ViewBag.posts = posts;
            
            // Retrieve the sitemap page content
            ContentPage page = ContentManagement.GetPageByTitle("sitemap");
            ViewBag.page = page;

            return View();
        }

    }
}
