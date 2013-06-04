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
        public ActionResult Index() {

            List<APICategory> parents = new List<APICategory>();
            parents = ViewBag.parent_cats;
            if (parents == null) {
                parents = CURTAPI.GetParentCategories();
            }

            // We need to retrieve our recent parts from our session object
            List<APIPart> recentParts = SessionWorker.GetRecentParts();
            ViewBag.recentParts = recentParts;

            // We need to get 5 random banners
            List<Banner> banners = UDF.GetRandomBanners(5);
            ViewBag.banners = banners;

            // Retrieve the homepage content
            ContentPage page = ContentManagement.GetPageByTitle("homepage");
            ViewBag.page = page;

            return View();
        }

    }
}
