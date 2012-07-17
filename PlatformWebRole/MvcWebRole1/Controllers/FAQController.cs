using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class FAQController : BaseController {
        
        public ActionResult Index() {

            // Get all the FaqTopics
            List<FaqTopic> topics = FaqTopic.GetAll();
            ViewBag.topics = topics;

            // Get the ContentPage for the FAQ
            ContentPage page = ContentManagement.GetPageByTitle("faq");
            ViewBag.page = page;

            return View();
        }

        public ActionResult Topic(string title = "") {

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
