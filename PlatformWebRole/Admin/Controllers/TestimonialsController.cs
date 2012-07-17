using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace Admin.Controllers
{
    public class TestimonialsController : BaseController {
        //
        // GET: /Admin_Review/

        public ActionResult Index()
        {
            List<Testimonial> testimonials = TestimonialModel.GetAllUnapproved();
            ViewBag.testimonials = testimonials;
            return View();
        }

        public ActionResult Approved() {
            List<Testimonial> testimonials = TestimonialModel.GetAllApproved();
            ViewBag.testimonials = testimonials;
            return View();
        }

        [NoValidation]
        public string Get(int id = 0) {
            Testimonial testimonial = TestimonialModel.Get(id);
            return JsonConvert.SerializeObject(testimonial);
        }

        [NoValidation]
        public string Approve(int id = 0) {
            return TestimonialModel.Approve(id);
        }

        [NoValidation]
        public string Delete(int id = 0) {
            // Permanently Delete testimonial
            if (!TestimonialModel.Remove(id)) {
                return "error";
            }
            return "";
        }
    }
}
