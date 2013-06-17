using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;

namespace EcommercePlatform.Controllers {
    public class InfoController : BaseController {

        public async Task<ActionResult> Index() {
            var pcats = CURTAPI.GetParentCategoriesAsync();
            await Task.WhenAll(new Task[] { pcats });
            ViewBag.parent_cats = await pcats;
            return View();
        }

    }
}
