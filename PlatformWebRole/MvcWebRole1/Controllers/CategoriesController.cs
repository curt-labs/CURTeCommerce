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

        public async Task<ActionResult> Index(int catID = 0, int page = 1, int per_page = 10) {
            HttpContext ctx = System.Web.HttpContext.Current;

            APICategory category = new APICategory();
            APIColorCode color_code = new APIColorCode();
            List<APIPart> catparts = new List<APIPart>();
            List<APIPart> moreparts = new List<APIPart>();
            Task<List<APICategory>> pcats = CURTAPI.GetParentCategoriesAsync();
            Task<List<APICategory>> crumbs = null;
            Task<APICategory> cat = null;
            Task<List<APIPart>> parts = null;
            Task<List<APIPart>> moreTask = null;
            Task<APIColorCode> codetask = null;
            List<APICategory> breadcrumbs = new List<APICategory>();
            List<Task> tasks = new List<Task> { pcats };

            if (catID > 0) {
                UDF.SetCategoryCookie(ctx, catID);
                cat = CURTAPI.GetCategoryAsync(catID);
                parts = CURTAPI.GetCategoryPartsAsync(catID, page, per_page);
                moreTask = CURTAPI.GetCategoryPartsAsync(catID, page + 1, per_page);
                codetask = CURTAPI.GetCategoryColorCodeAsync(catID);
                crumbs = CURTAPI.GetBreadcrumbsAsync(catID);
                tasks.Add(cat);
                tasks.Add(parts);
                tasks.Add(moreTask);
                tasks.Add(codetask);
                tasks.Add(crumbs);
            } else {
                category.catTitle = "Product Categories";
                category.catID = 0;
            }
            await Task.WhenAll( tasks.ToArray() );
            ViewBag.parent_cats = await pcats;
            if (catID > 0) {
                category = await cat;
                color_code = await codetask;
            } else {
                category.SubCategories = ViewBag.parent_cats;
            }
            ViewBag.category = category;
            if (crumbs != null) {
                breadcrumbs = await crumbs;
            }
            ViewBag.breadcrumbs = breadcrumbs;
            if (parts != null) {
                catparts = await parts;
                moreparts = await moreTask;
                if (catparts.Count > 0) {
                    ViewBag.parts = catparts;
                    int more_count = moreparts.Count;
                    ViewBag.more_count = more_count;

                    ViewBag.page = page;
                    ViewBag.per_page = per_page;

                    return View("Parts");
                }
            }

            return View();
        }

        public ActionResult Parts(int id = 0, int page = 1, int per_page = 10) {
            if (id > 0) {
                return RedirectToRoutePermanent("CategoryByID", new { catID = id, cat = "", page = page, per_page = per_page });
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
