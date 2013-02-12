using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml;

namespace EcommercePlatform.Models {
    public class Archive {
        public int monthnum { get; set; }
        public string month { get; set; }
        public string year { get; set; }

        public static List<Archive> GetMonths() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                List<Archive> archives = new List<Archive>();

                archives = (from p in db.BlogPosts
                            where p.publishedDate.Value != null && p.publishedDate.Value <= DateTime.UtcNow && p.active.Equals(true) 
                            orderby p.publishedDate.Value.Year descending, p.publishedDate.Value.Month descending
                            select new Archive
                            {
                                monthnum = p.publishedDate.Value.Month,
                                month = p.publishedDate.Value.Month.ToString(),
                                year = p.publishedDate.Value.Year.ToString()
                            }).Distinct().ToList<Archive>();

                archives = (from a in archives
                            orderby a.year descending, a.monthnum
                            select a).Distinct().ToList<Archive>();

                return archives;
            } catch {
                return new List<Archive>();
            }
        }
    }
}