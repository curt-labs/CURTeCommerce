using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin {
    partial class Banner {

        internal List<Banner> GetAll() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Banner> banners = new List<Banner>();
            banners = db.Banners.ToList<Banner>();
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
            }
            tmp.image = this.image;
            tmp.title = this.title;
            tmp.body = this.body;
            tmp.link = this.link;
            tmp.order = this.order;
            tmp.isVisible = this.isVisible;
            //tmp.OrganizeSort();

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

        private void OrganizeSort() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();

            int max_order = 0;
            int min_order = 0;
            try {
                max_order = db.Banners.Select(x => x.order).Max<int>();
            } catch (Exception) { }
            try {
                min_order = db.Banners.Select(x => x.order).Min<int>();
            } catch (Exception) { }
            if (this.order <= max_order) {
                // Get all the Banners with a sort higher than this one.
                List<Banner> banners = db.Banners.Where(x => x.order >= this.order && x.ID != this.ID).ToList<Banner>();
                foreach (Banner b in banners) {

                    b.order = b.order + 1;
                }
                db.SubmitChanges();
            }

        }

    }
}