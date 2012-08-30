using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using System.Web.Script.Serialization;

namespace EcommercePlatform.Controllers {
    public class SearchController : BaseController {

        public ActionResult Index(string search = "", int page = 1, int per_page = 10) {
            try {
                ViewBag.search_term = search;

                // Run the search term against our PowerSearch API
                List<APIPart> parts = CURTAPI.Search(search, page, per_page);
                ViewBag.parts = parts;

                Dictionary<string, List<APIPart>> ordered_parts = new Dictionary<string, List<APIPart>>();
                foreach (APIPart part in parts) {

                    APIColorCode color_code = CURTAPI.GetColorCode(part.partID);
                    part.colorCode = color_code.code;

                    if (ordered_parts.Keys.Contains("search")) { // Already added to dictionary
                        List<APIPart> existing_parts = ordered_parts.Where(x => x.Key == "search").Select(x => x.Value).FirstOrDefault<List<APIPart>>();
                        existing_parts.Add(part);
                        ordered_parts["search"] = existing_parts;
                    } else { // New Color Code
                        List<APIPart> new_parts = new List<APIPart>();
                        new_parts.Add(part);
                        ordered_parts.Add("search", new_parts);
                    }
                }
                ViewBag.parts = ordered_parts;

                // We need to figure out if there are going to be more parts to display
                int more_count = CURTAPI.Search(search, page + 1, per_page).Count;
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
