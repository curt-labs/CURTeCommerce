using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommercePlatform.Models {
    public class NewsletterFunctions {

        internal static void Add(string name, string email) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Newsletter exists = new Newsletter();

            // Check for existing
            exists = db.Newsletters.Where(x => x.Email.Equals(email)).FirstOrDefault<Newsletter>();
            if (exists != null) { throw new Exception("We are already sending the newsletter to " + email); }

            Newsletter nw = new Newsletter {
                Name = name,
                Email = email,
                DateAdded = DateTime.Now,
                Unsubscribe = Guid.NewGuid()
            };
            db.Newsletters.InsertOnSubmit(nw);
            db.SubmitChanges();
        }

        internal static void Unsubscribe(Guid unsub) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();

            if (unsub == null) { throw new Exception("Invalid reference to a subscription."); }
            Newsletter nw = db.Newsletters.Where(x => x.Unsubscribe.Equals(unsub)).FirstOrDefault<Newsletter>();
            if (nw == null) { throw new Exception("No subscription found."); }

            db.Newsletters.DeleteOnSubmit(nw);
            db.SubmitChanges();
        }
    }
}