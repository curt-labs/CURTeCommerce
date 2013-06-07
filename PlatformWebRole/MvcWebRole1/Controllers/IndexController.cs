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
            await Task.WhenAll(new Task[] { pcats, rparts });
            
            ViewBag.parent_cats = await pcats;

            // We need to retrieve our recent parts from our session object
            ViewBag.recentParts = await rparts;
            List<double> years = ViewBag.years;

            // We need to get 5 random banners
            List<Banner> banners = UDF.GetBanners();
            ViewBag.banners = banners;

            // Retrieve the homepage content
            ContentPage page = ContentManagement.GetPageByTitle("homepage");
            ViewBag.page = page;


            return View();
        }

    }
}
