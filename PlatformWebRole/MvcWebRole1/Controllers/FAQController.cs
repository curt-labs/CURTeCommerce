using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class FAQController : BaseController {
        
        public async Task<ActionResult> Index() {
            var pcats = CURTAPI.GetParentCategoriesAsync();
            await Task.WhenAll(new Task[] { pcats });
            ViewBag.parent_cats = await pcats;

            // Get all the FaqTopics
            List<FaqTopic> topics = FaqTopic.GetAll();
            ViewBag.topics = topics;

            // Get the ContentPage for the FAQ
            ContentPage page = ContentManagement.GetPageByTitle("faq");
            ViewBag.page = page;

            return View();
        }

        public async Task<ActionResult> Topic(string title = "") {
            var pcats = CURTAPI.GetParentCategoriesAsync();
            await Task.WhenAll(new Task[] { pcats });
            ViewBag.parent_cats = await pcats;

            // Get all the FaqTopics
            List<FaqTopic> topics = FaqTopic.GetAll();
            ViewBag.topics = topics;

            // Get the topic that matches the title
            FaqTopic topic = topics.Where(x => x.topic.Equals(title.Trim())).FirstOrDefault<FaqTopic>();
            ViewBag.topic = topic;

            return View();
        }

    }
}
