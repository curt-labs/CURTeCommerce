using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommercePlatform.Models {

    public class TestimonialModel {

        public static List<Testimonial> GetAll(int page = 1, int pageSize = 10) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.Testimonials.Where(x => x.active == true).Where(x => x.approved == true).OrderByDescending(x => x.dateAdded).Skip((page - 1) * pageSize).Take(pageSize).ToList<Testimonial>();
            } catch (Exception e) {
                return new List<Testimonial>();
            }
        }

        public static int CountAll() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.Testimonials.Where(x => x.active == true).Where(x => x.approved == true).Count();
            } catch (Exception e) {
                return 0;
            }
        }

        public static Testimonial Add(string first_name = "", string last_name = "", string location = "", string testimonial = "") {
            Testimonial t = new Testimonial();
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                t = new Testimonial {
                    testimonial1 = testimonial,
                    first_name = first_name,
                    last_name = last_name,
                    location = location,
                    dateAdded = DateTime.UtcNow,
                    approved = false,
                    active = true,
                };
                db.Testimonials.InsertOnSubmit(t);
                db.SubmitChanges();
                return t;

            } catch (Exception e) {
                return t;
            }
        }

    }

}