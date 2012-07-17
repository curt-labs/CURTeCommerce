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
                            where p.publishedDate.Value != null && p.publishedDate.Value <= DateTime.Now && p.active.Equals(true) 
                            orderby p.publishedDate.Value.Year descending, p.publishedDate.Value.Month descending
                            select new Archive
                            {
                                monthnum = Convert.ToInt16(Convert.ToDateTime(p.publishedDate).Month.ToString()),
                                month = Convert.ToDateTime(p.publishedDate).Month.ToString(),
                                year = Convert.ToDateTime(p.publishedDate).Year.ToString()
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