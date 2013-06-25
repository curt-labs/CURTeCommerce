using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using System.Web.Script.Serialization;
using System.Threading.Tasks;

namespace EcommercePlatform.Controllers {
    public class SearchController : BaseController {

        public async Task<ActionResult> Index(string search = "", int page = 1, int per_page = 10) {
            try {
                ViewBag.search_term = search;

                var pcats = CURTAPI.GetParentCategoriesAsync();
                var searchtask = CURTAPI.SearchAsync(search, page, per_page);
                var moretask = CURTAPI.SearchAsync(search, page + 1, per_page);
                await Task.WhenAll(new Task[] { pcats, searchtask, moretask });
                ViewBag.parent_cats = await pcats;

                // Run the search term against our PowerSearch API
                List<APIPart> parts = await searchtask;
                ViewBag.parts = parts;

                // We need to figure out if there are going to be more parts to display
                List<APIPart> moreparts = await moretask;
                int more_count = moreparts.Count;
                ViewBag.more_count = more_count;

                ViewBag.page = page;
                ViewBag.per_page = per_page;

            } catch (Exception e) {
                if (e.Message.ToLower().Contains("a potentially dangerous")) {
                    throw new HttpException(403, "Forbidden");
                }
            }
            return View();
        }


        public string GetMore(string search = "", int page = 1, int perpage = 20) {
            try {
                return new JavaScriptSerializer().Serialize(CURTAPI.Search(search, page, perpage));
            } catch (Exception) {
                return "[]";
            }
        }
    }
}
