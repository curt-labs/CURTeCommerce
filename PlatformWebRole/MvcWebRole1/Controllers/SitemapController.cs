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

            // Retrieve the homepage content
            ContentPage page = ContentManagement.GetPageByTitle("sitemap");
            ViewBag.page = page;

            return View();
        }

    }
}
