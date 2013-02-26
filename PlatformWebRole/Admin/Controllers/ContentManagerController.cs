using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Web.Script.Serialization;

namespace Admin.Controllers {
    public class ContentManagerController : BaseController {

        public ActionResult Index() {

            // Make sure we have entries for our fixed pages
            ContentManagement.CheckFixed();

            List<StrippedContent> pages = ContentManagement.GetAll();
            ViewBag.pages = pages;

            // We need to pass in the fixed_titles so we know when we can't allow the user to edit the title
            string[] fixed_pages = ContentManagement.fixed_pages;
            ViewBag.fixed_pages = fixed_pages;

            return View();
        }

        public ActionResult Edit(int id = 0, string message = "") {
            // Load the page from the database
            ContentPage page = new ContentPage();
            if (id > 0) {
                page = ContentManagement.GetPage(id);
            }
            ViewBag.page = page;

            // Try override 'page' with our page from TempData, in case there was a fucking error while saving. Fuck that error!!
            ContentPage errPage = (ContentPage)TempData["page"];
            if (errPage != null) {
                ViewBag.page = errPage;
            }
            ViewBag.message = TempData["error"];

            // Build out the listing of parent pages i.e. pages who list this page in there menu
            ViewBag.parent_page = page.getParent();

            // Build out the listing of subpages i.e. pages who list this page as their parent page
            ViewBag.sub_pages = page.getSubpages();

            // We need to pass in the fixed_titles so we know when we can't allow the user to edit the title
            string[] fixed_pages = ContentManagement.fixed_pages;
            ViewBag.fixed_pages = fixed_pages;

            return View();
        }

        public ActionResult Delete(int id = 0) {
            try {
                ContentManagement.DeletePage(id);
                return RedirectToAction("Index", "ContentManager");
            } catch (Exception e) {
                TempData["error"] = e.Message;
                return RedirectToAction("Edit", "ContentManager", new { id = id });
            }
        }

        [ValidateInput(false)]
        public ActionResult Save(int id = 0) {
            ContentPage page = null;
            try {
                NameValueCollection data = Request.Form;
                string post_json = JsonConvert.SerializeObject(data.ToFormDictionary());
                page = JsonConvert.DeserializeObject<ContentPage>(post_json);
                page.ID = id;
                ContentManagement.Add(page);

                return RedirectToAction("Index", "ContentManager");
            } catch (Exception e) {
                TempData["page"] = page;
                TempData["error"] = e.Message;
                return RedirectToAction("Edit", "ContentManager", new { id = id });
            }

        }

        [NoValidation]
        public string GetPages(int id = 0) {
            try {
                return JsonConvert.SerializeObject(ContentManagement.GetAll());
            } catch (Exception) {
                return JsonConvert.SerializeObject(new List<ContentPage>());
            }
        }

        [NoValidation]
        public string AddParent(int id, int parentID) {
            try {
                //ContentManagement.ClearParents(id);
                ContentManagement.AddParent(id, parentID);
                List<StrippedContent> pages = new List<StrippedContent>();
                pages.Add(ContentManagement.GetStrippedPage(parentID));
                return JsonConvert.SerializeObject(pages);
            } catch (Exception e) {
                return e.Message;
            }
        }

        [NoValidation]
        public string AddSubs(int id, string subs) {
            try {
                List<string> subList = subs.Split(',').ToList<string>();
                List<StrippedContent> pages = new List<StrippedContent>();
                foreach (string sub in subList) {
                    ContentManagement.AddParent(Convert.ToInt32(sub), id);
                    StrippedContent page = ContentManagement.GetStrippedPage(Convert.ToInt32(sub));
                    pages.Add(page);
                }
                return JsonConvert.SerializeObject(pages);
            } catch (Exception e) {
                return e.Message;
            }
        }

        [NoValidation]
        public string RemoveRelationship(int id, int parent) {
            try {
                ContentManagement.RemoveParent(id, parent);
                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }

    }
}