using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers {
    public class FAQManagerController : BaseController {

        public ActionResult Index() {

            // Get all the FAQ topics
            List<FaqTopic> topics = FaqTopic.GetAll();
            ViewBag.topics = topics;

            // Get all the FAQ
            List<FAQ> faqs = FAQ.GetAll();
            ViewBag.faqs = faqs;

            return View();
        }

        public ActionResult EditTopic(int id = 0) {

            // Get the FAQ to be edited
            FaqTopic topic = new FaqTopic { ID = id };
            topic.Get();
            ViewBag.topic = topic;

            try {
                FaqTopic tmp = (FaqTopic)TempData["topic"];
                if (tmp != null) {
                    ViewBag.topic = tmp;
                }
            } catch (Exception) { }

            if (TempData["error"] != null) {
                ViewBag.error = TempData["error"].ToString();
            }
            return View();
        }

        public ActionResult EditQuestion(int id = 0) {

            // Get the FAQ to be edited
            FAQ faq = new FAQ { ID = id };
            faq.Get();
            ViewBag.faq = faq;

            // Get the topics
            List<FaqTopic> topics = FaqTopic.GetAll();
            ViewBag.topics = topics;

            try {
                FAQ tmp = (FAQ)TempData["faq"];
                if (tmp != null) {
                    ViewBag.faq = tmp;
                }
            } catch (Exception) { }

            if (TempData["error"] != null) {
                ViewBag.error = TempData["error"].ToString();
            }
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveTopic(int id = 0, string topic = "") {
            FaqTopic new_topic = new FaqTopic();
            try {
                new_topic = new FaqTopic {
                    ID = id,
                    topic = topic
                };
                new_topic.Save();
                return RedirectToAction("Index", "FAQManager");
            } catch (Exception e) {
                TempData["topic"] = topic;
                TempData["error"] = e.Message;
                return RedirectToAction("EditTopic", "FAQManager", new { id = id });
            }
        }

        public void DeleteTopic(int id = 0) {
            try {
                FaqTopic tmp = new FaqTopic {
                    ID = id
                };
                tmp.Get();
                tmp.Delete();
            } catch (Exception e) {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                Response.StatusDescription = e.Message;
                Response.Write(e.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveQuestion(int id = 0, string question = "", string answer = "", int topic = 0) {
            FAQ new_faq = new FAQ();
            try {
                new_faq = new FAQ {
                    ID = id,
                    question = question,
                    answer = answer,
                    topic = topic
                };
                new_faq.Save();
                return RedirectToAction("Index", "FAQManager");
            } catch (Exception e) {
                TempData["faq"] = new_faq;
                TempData["error"] = e.Message;
                return RedirectToAction("EditQuestion", "FAQManager", new { id = id });
            }
        }

        public void DeleteQuestion(int id = 0) {
            try {
                FAQ tmp = new FAQ {
                    ID = id
                };
                tmp.Get();
                tmp.Delete();
            } catch (Exception e) {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                Response.StatusDescription = e.Message;
                Response.Write(e.Message);
            }
        }

    }
}
