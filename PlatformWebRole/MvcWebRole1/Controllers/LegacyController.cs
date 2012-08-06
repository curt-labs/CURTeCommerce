using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace EcommercePlatform.Controllers {
    public class LegacyController : BaseController {
        //

        public ActionResult legacyRedirect() {
            string eventname = Request.QueryString["event"];
            if (eventname == "pageview") {
                string cpieceID = Request.QueryString["contentpieceid"];
                if (cpieceID == "1375") {
                    return RedirectToRoutePermanent("CategoryWithID", new { id = 1, name = "Hitches" });
                } else if (cpieceID == "7452") {
                    return RedirectToRoutePermanent("TechSupportContact");
                } else if (cpieceID == "1377") {
                    return RedirectToActionPermanent("Index", "Video");
                } else if (cpieceID == "7291" || cpieceID == "1345" || cpieceID == "1347" || cpieceID == "1346") {
                    return RedirectToRoutePermanent("Page", new { name = "understanding_towing" });
                } else if (cpieceID == "8864") {
                    return RedirectToRoutePermanent("CategoryWithID", new { id = 11, name = "Front Mount Hitches" });
                } else if (cpieceID == "1374") {
                    // Do Something
                }
            }
            if (eventname == "prodetail") {
                string idstr = Request.QueryString["id"];
                string categoryid = Request.QueryString["categoryid"];
                if (idstr == "86" && categoryid == "38") {
                    return RedirectToRoutePermanent("CategoryWithID", new { id = 49, name = "20K" });
                }
                if (idstr == "6" && categoryid == "26") {
                    return RedirectToRoutePermanent("CategoryWithID", new { id = 200, name = "Brake Controllers" });
                }
            }
            if (eventname == "prolist") {
                string categoryid = Request.QueryString["categoryid"];
                if (categoryid == "47") {
                    return RedirectToRoutePermanent("CategoryWithID", new { id = 153, name = "Ball Mounts" });
                }
                if (categoryid == "36") {
                    return RedirectToRoutePermanent("CategoryWithID", new { id = 80, name = "Weight Distribution" });
                }
            }
            if (eventname == "sheet") {
                string code = Request.QueryString["code"];
                return RedirectToRoutePermanent("Sheet", new { id = code });
            }
            if (eventname == "providers") {
                return RedirectToActionPermanent("Index", "WhereToBuy");
            }
            return RedirectToActionPermanent("index", "index");
        }

    }
}
