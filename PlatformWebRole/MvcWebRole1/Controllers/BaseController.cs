using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using System.Web.Routing;
using System.Security;
using Newtonsoft.Json;

namespace EcommercePlatform.Controllers {
    public class BaseController : Controller {

        protected override void Initialize(System.Web.Routing.RequestContext requestContext) {
            base.Initialize(requestContext);

            // Get the vehicle years
            List<double> years = CURTAPI.GetYears();
            ViewBag.years = years;

            // Get the parent categories
            List<APICategory> parent_cats = CURTAPI.GetParentCategories();
            ViewBag.parent_cats = parent_cats;

            // We're gonna dump our Customer Session object out
            Customer customer = new Customer();
            customer.GetFromStorage();
            
            ViewBag.settings = new Settings();

            // Check to see if the cart object exists
            if (customer.Cart == null) {
                // Create a new Cart object and add it to the Customer object
                Cart cart = new Cart();
                customer.Cart = cart;
                customer.SerializeToStorage();
            }

            ViewBag.customer = customer;

        }

    }
}
