using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommercePlatform.Controllers
{
    public class CustomerAuthController : BaseController {

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            base.OnActionExecuting(filterContext);

            // We need to make sure the user is logged in, if they are not we will redirect them to the customer login page
            Customer cust = new Customer();
            if (!cust.LoggedIn()) {
                Response.Redirect("/Authenticate");
            }

        }
    }
}
