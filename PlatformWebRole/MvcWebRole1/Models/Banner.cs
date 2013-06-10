using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommercePlatform.Models;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.Linq;
using System.Threading.Tasks;

namespace EcommercePlatform {
    partial class Banner {

        public static List<Banner> GetRandomBanners(int count = 5) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Banner> banners = new List<Banner>();
            banners = db.Banners.Where(x => x.isVisible.Equals(1)).ToList<Banner>();
            banners = UDF.Shuffle<Banner>(banners);
            banners = banners.Take(count).ToList<Banner>();
            return banners;
        }

        public static List<Banner> GetBanners() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Banner> banners = new List<Banner>();
            banners = db.Banners.Where(x => x.isVisible.Equals(1)).OrderBy(x => x.order).ToList<Banner>();
            return banners;
        }

        public static async Task<List<Banner>> GetBannersAsync() {
            List<Banner> banners = new List<Banner>();
            banners = await Task.Factory.StartNew(() => GetBanners());
            return banners;
        }

    }
}
