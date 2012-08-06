using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Net;

namespace Admin {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{file}.txt");
            routes.IgnoreRoute("{file}.htm");
            routes.IgnoreRoute("{file}.html");
            routes.IgnoreRoute("{file}.xml");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Index", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            WebRequest.DefaultWebProxy = null;
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest(Object sender, EventArgs e) {
            if (HttpContext.Current.Request.IsSecureConnection == false) {
                Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"] + HttpContext.Current.Request.RawUrl);
            }
        }

        protected void Application_Error(object sender, EventArgs e) {
            var error = Server.GetLastError();
            var code = (error is HttpException) ? (error as HttpException).GetHttpCode() : 500;

            if (code != 404) {
                Response.ContentType = "text/plain";
                Response.Write(error.Data + "\r\n");
                Response.Write(error.Message + error.Source + "\r\n");
                Response.Write(error.StackTrace + "\r\n");
                Response.End();
            } else {
                Response.ContentType = "text/plain";
                Response.Write("404 Not Found");
                Response.End();
            }
            //Response.Clear();
            //Server.ClearError();

        }
    }
}