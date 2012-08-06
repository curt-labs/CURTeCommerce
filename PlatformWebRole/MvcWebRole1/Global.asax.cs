using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Hosting;
using System.Net;

namespace MvcWebRole1 {
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
                "LegecyCatch", // Route name
                "index.cfm", // URL with parameters
                new { controller = "Legacy", action = "legacyRedirect" } // Parameter defaults
            );

            routes.MapRoute(
                "VehicleLookup",
                "Lookup/{year}/{make}/{model}/{style}",
                new { controller = "Lookup", action = "Index", year = "", make = "", model = "", style = "" });


            routes.MapRoute(
                "Category",
                "Category/{*cat}",
                new { controller = "Categories", action = "Index", cat = "" }
            );

            routes.MapRoute(
                "Part Details",
                "Part/{id}",
                new { controller = "Part", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Category Parts",
                "Categories/Parts/{id}/{page}/{per_page}",
                new { controller = "Categories", action = "Parts", id = UrlParameter.Optional, page = UrlParameter.Optional, per_page = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ContentPages",
                "Page/{title}",
                new { controller = "Page", action = "Index", title = UrlParameter.Optional });

            routes.MapRoute(
                "FAQ",
                "FAQ",
                new { controller = "FAQ", action = "Index" }
            );

            // http://localhost/Blog/Archive/January/2011, page is optional
            routes.MapRoute(
                "BlogArchive",
                "Blog/Archive/{month}/{year}",
                new { controller = "Blog", action = "ViewArchive", month = "", year = "" }
            );

            // http://localhost/Blog/Author/Test_Testerson, page is optional
            routes.MapRoute(
                "BlogAuthor",
                "Blog/Author/{name}",
                new { controller = "Blog", action = "Author", name = "" }
            );

            // http://localhost/Blog/Category/Hitches/page/1, page is optional
            routes.MapRoute(
                "BlogFeed",
                "Blog/Feed/{type}",
                new { controller = "Blog", action = "Feed", feed = "rss" }
            );

            // http://localhost/Blog/Category/Hitches, page is optional
            routes.MapRoute(
                "BlogCategory",
                "Blog/Category/{name}",
                new { controller = "Blog", action = "ViewCategory", name = "" }
            );

            // http://localhost/Blog/Category/Hitches/page/1, page is optional
            routes.MapRoute(
                "BlogCategoryFeed",
                "Blog/Category/{name}/Feed/{type}",
                new { controller = "Blog", action = "CategoryFeed", name = "", type = "rss" }
            );

            // http://localhost/Blog/Post/Comment/1, the number is the post id
            routes.MapRoute(
                "BlogPostComment",
                "Blog/Post/Comment/{id}",
                new { controller = "Blog", action = "Comment", id = "", message = UrlParameter.Optional }
            );

            // http://localhost/Blog/8-24-2011/This+is+a+blog+post+title
            routes.MapRoute(
                "BlogPost",
                "Blog/Post/{date}/{title}",
                new { controller = "Blog", action = "ViewPost", date = "", title = "" }
            );

            routes.MapRoute(
                "Shipping",
                "Shipping",
                new { controller = "Page", action = "Index", title = "Shipping/Return Information" }
            );

            routes.MapRoute(
                "Return",
                "Returns",
                new { controller = "Page", action = "Index", title = "Shipping/Return Information" }
            );

            routes.MapRoute(
                "Warranty",
                "Warranty",
                new { controller = "Page", action = "Index", title = "Warranty" }
            );

            routes.MapRoute(
                "Privacy Policy",
                "PrivacyPolicy",
                new { controller = "Page", action = "Index", title = "Privacy Policy" }
            );

            routes.MapRoute(
                "About",
                "About",
                new { controller = "Page", action = "Index", title = "About Us" }
            );

            routes.MapRoute(
                "About Us",
                "About Us",
                new { controller = "Page", action = "Index", title = "About Us" }
            );

            routes.MapRoute(
                "AboutUs",
                "AboutUs",
                new { controller = "Page", action = "Index", title = "About Us" }
            );

            routes.MapRoute(
                "FAQ Topic",
                "FAQ/{title}",
                new { controller = "FAQ", action = "Topic", title = UrlParameter.Optional }
            );

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
    }
}