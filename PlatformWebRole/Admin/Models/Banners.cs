using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin {
    partial class Banner {

        internal List<Banner> GetAll() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Banner> banners = new List<Banner>();
            banners = db.Banners.OrderBy(x => x.order).ToList<Banner>();
            return banners;
        }

        internal void Get() {
            Banner tmp = new Banner();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();

            tmp = db.Banners.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Banner>();
            this.image = tmp.image;
            this.title = tmp.title;
            this.body = tmp.body;
            this.link = tmp.link;
            this.order = tmp.order;
            this.isVisible = tmp.isVisible;
        }

        internal void Save() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Banner tmp = db.Banners.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Banner>();
            if (tmp == null) {
                tmp = new Banner();
                int nextsort = 1;
                if(db.Banners.Count() > 0) {
                    nextsort = db.Banners.OrderByDescending(x => x.order).Select(x => x.order).Take(1).First() + 1;
                }
                tmp.order = nextsort;
            }
            tmp.image = this.image;
            tmp.title = this.title;
            tmp.body = this.body;
            tmp.link = this.link;
            tmp.isVisible = this.isVisible;

            if (this.ID == 0) {
                db.Banners.InsertOnSubmit(tmp);
            }
            db.SubmitChanges();
            this._ID = tmp.ID;

        }

        internal void Delete() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Banner tmp = db.Banners.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Banner>();
            db.Banners.DeleteOnSubmit(tmp);
            db.SubmitChanges();
        }

        public static void Sort(List<string> ids) {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                int sort = 0;
                foreach (string id in ids) {
                    sort++;
                    Banner b = db.Banners.Where(x => x.ID == Convert.ToInt32(id)).First();
                    b.order = sort;
                    db.SubmitChanges();
                }
            } catch { }
        }

    }
}