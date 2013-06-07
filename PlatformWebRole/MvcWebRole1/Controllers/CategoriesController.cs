using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Controllers;
using EcommercePlatform.Models;
using System.Web.Script.Serialization;
using System.Threading.Tasks;

namespace EcommercePlatform.Controllers {
    public class CategoriesController : BaseController {

        public async Task<ActionResult> Index(int catID = 0) {

            if (catID > 0) {
                var pcats = CURTAPI.GetParentCategoriesAsync();
                var cat = CURTAPI.GetCategoryAsync(catID);
                await Task.WhenAll(new Task[] { pcats, cat });
                ViewBag.parent_cats = await pcats;

                APICategory category = await cat;
                
                ViewBag.category = category;

                if (category.SubCategories.Count == 0) { // Redirect to view the parts for this category
                    return RedirectToAction("Parts", "Categories", new { id = category.catID });
                }
            } else {
                var pcats = CURTAPI.GetParentCategoriesAsync();
                await Task.WhenAll(new Task[] { pcats });
                ViewBag.parent_cats = await pcats;

                APICategory category = new APICategory();
                category.catTitle = "Product Categories";
                category.catID = 0;
                category.SubCategories = ViewBag.parent_cats;
                ViewBag.category = category;
            }

            return View();
        }

        public async Task<ActionResult> Parts(int id = 0, int page = 1, int per_page = 10) {
            if (id > 0) {
                APICategory category = new APICategory();
                var pcats = CURTAPI.GetParentCategoriesAsync();
                var catTask = CURTAPI.GetCategoryAsync(id);
                var partTask = CURTAPI.GetCategoryPartsAsync(id, page, per_page);
                var moreTask = CURTAPI.GetCategoryPartsAsync(id, page + 1, per_page);
                await Task.WhenAll(new Task[] { pcats, catTask, partTask, moreTask });
                ViewBag.parent_cats = await pcats;

                category = await catTask;
                ViewBag.category = category;

                List<APIPart> parts = await partTask;

                Dictionary<string, List<APIPart>> ordered_parts = new Dictionary<string, List<APIPart>>();
                foreach (APIPart part in parts) {
                    APIColorCode color_code = CURTAPI.GetColorCode(part.partID);
                    part.colorCode = color_code.code;
                    if (part.pClass.Length > 0) {
                        if (ordered_parts.Keys.Contains(part.pClass)) { // Already added to dictionary
                            List<APIPart> existing_parts = ordered_parts.Where(x => x.Key == part.pClass).Select(x => x.Value).FirstOrDefault<List<APIPart>>();
                            existing_parts.Add(part);
                            ordered_parts[part.pClass] = existing_parts;
                        } else { // New Color Code
                            List<APIPart> new_parts = new List<APIPart>();
                            new_parts.Add(part);
                            ordered_parts.Add(part.pClass, new_parts);
                        }
                    } else {
                        if (ordered_parts.Keys.Contains(category.catTitle.Trim())) { // Already added to dictionary
                            List<APIPart> existing_parts = ordered_parts.Where(x => x.Key == category.catTitle.Trim()).Select(x => x.Value).FirstOrDefault<List<APIPart>>();
                            existing_parts.Add(part);
                            ordered_parts[category.catTitle.Trim()] = existing_parts;
                        } else { // New Color Code
                            List<APIPart> new_parts = new List<APIPart>();
                            new_parts.Add(part);
                            ordered_parts.Add(category.catTitle.Trim(), new_parts);
                        }
                    }
                }
                ViewBag.parts = ordered_parts;

                // We need to figure out if there are going to be more parts to display
                List<APIPart> moreparts = await moreTask;
                int more_count = moreparts.Count;
                ViewBag.more_count = more_count;

                ViewBag.page = page;
                ViewBag.per_page = per_page;

                return View();
            } else {
                return RedirectToAction("Index", "Index");
            }
        }

        public string GetMore(int catID = 0, int page = 1, int perpage = 10) {
            try {
                return Newtonsoft.Json.JsonConvert.SerializeObject(CURTAPI.GetCategoryParts(catID, page, perpage));
            } catch (Exception) {
                return "[]";
            }
        }

    }
}
