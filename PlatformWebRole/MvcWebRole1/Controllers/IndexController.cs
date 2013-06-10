using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class IndexController : BaseController {
        //
        // GET: /Index/
        public async Task<ActionResult> Index() {
            HttpContext ctx = System.Web.HttpContext.Current;

            var pcats = CURTAPI.GetParentCategoriesAsync();
            var rparts = SessionWorker.GetRecentParts(ctx);
            var bannertask = Banner.GetBannersAsync();
            var contenttask = ContentManagement.GetPageByTitleAsync("homepage");
            await Task.WhenAll(new Task[] { pcats, rparts, bannertask, contenttask });
            
            ViewBag.parent_cats = await pcats;

            // We need to retrieve our recent parts from our session object
            ViewBag.recentParts = await rparts;

            // We need to get 5 random banners
            List<Banner> banners = await bannertask;
            ViewBag.banners = banners;

            // Retrieve the homepage content
            ContentPage page = await contenttask;
            ViewBag.page = page;


            return View();
        }

    }
}
