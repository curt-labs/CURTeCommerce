using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers {
    public class _4boxController : Controller {

        SquareAuth sq = new SquareAuth();

        public string Index() {
            string token = sq.Token;

            return token;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public void oauth(string code = "") {
            try {
                sq.Token = code;
                string redirect = Session["oauth_requested_uri"].ToString();
                if (redirect.Length > 0) {
                    Response.Write("<script type='text/javascript'>window.location = '" + redirect + "'</script>");
                }
                Response.Write("<script type='text/javascript'>window.location = '/Admin'</script>");
            } catch (Exception) {
                Response.Write("<script type='text/javascript'>window.location = '/Admin'</script>");
            }
            Response.Flush();
        }

        public string GetCategories() {
            return sq.GetCategories();
        }

    }
}
