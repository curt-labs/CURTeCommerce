using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Admin.Models {
    public class TestimonialModel {

        public static Testimonial Get(int id = 0) {
            Testimonial testimonial = new Testimonial();
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                testimonial = db.Testimonials.Where(x => x.testimonialID == id).FirstOrDefault<Testimonial>();
            } catch {}
            return testimonial;
        }

        public static List<Testimonial> GetAll() {
            List<Testimonial> testimonials = new List<Testimonial>();
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                testimonials = db.Testimonials.Where(x => x.active == true).OrderByDescending(x => x.dateAdded).ToList<Testimonial>();
            } catch { }
            return testimonials;
        }

        public static List<Testimonial> GetAllUnapproved() {
            List<Testimonial> testimonials = new List<Testimonial>();
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                testimonials = db.Testimonials.Where(x => x.approved == false).Where(x => x.active == true).OrderByDescending(x => x.dateAdded).ToList<Testimonial>();
            } catch { }
            return testimonials;
        }

        public static List<Testimonial> GetAllApproved() {
            List<Testimonial> testimonials = new List<Testimonial>();
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                testimonials = db.Testimonials.Where(x => x.approved == true).Where(x => x.active == true).OrderByDescending(x => x.dateAdded).ToList<Testimonial>();
            } catch { }
            return testimonials;
        }

        public static Boolean Remove(int id = 0) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                Testimonial testimonial = db.Testimonials.Where(x => x.testimonialID == id).First<Testimonial>();
                testimonial.active = false;
                db.SubmitChanges();
                return true;
            } catch { return false; }
        }

        public static string Approve(int id = 0) {
            string approvedmsg = "0";
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                Testimonial testimonial = db.Testimonials.Where(x => x.testimonialID == id).First<Testimonial>();
                if (testimonial.approved) {
                    testimonial.approved = false;
                } else {
                    testimonial.approved = true;
                    approvedmsg = "1";
                }
                db.SubmitChanges();
                return approvedmsg;
            } catch { return approvedmsg; }
        }
    }
}