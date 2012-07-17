using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models {
    public class NewsletterFunctions {

        internal static List<Newsletter> GetAll() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.Newsletters.ToList<Newsletter>();
            } catch (Exception) {
                return new List<Newsletter>();
            }
        }

        internal static string Delete(int id) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                Newsletter nw = new Newsletter();

                nw = db.Newsletters.Where(x => x.ID.Equals(id)).FirstOrDefault<Newsletter>();

                db.Newsletters.DeleteOnSubmit(nw);
                db.SubmitChanges();

                return "";
            } catch (Exception e) {
                return e.Message;
            }
        }
    }
}