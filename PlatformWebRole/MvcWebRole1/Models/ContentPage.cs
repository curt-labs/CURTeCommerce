using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommercePlatform {
    partial class ContentPage {
        public List<ContentPage> getSubpages() {
            List<ContentPage> pages = new List<ContentPage>();
            List<ContentNesting> nestings = this.ContentNestings.ToList<ContentNesting>();
            foreach (ContentNesting n in nestings) {
                pages.AddRange(n.ContentPages.ToList<ContentPage>());
            }
            return pages;
        }
        public List<ContentPage> getBreadcrumbs() {
            List<ContentPage> pages = new List<ContentPage>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            bool isRoot = false;
            ContentPage currentPage = this;
            while (!isRoot) {
                if (currentPage.ContentNesting != null) {
                    pages.Add(currentPage.ContentNesting.ContentPage);
                    currentPage = currentPage.ContentNesting.ContentPage;
                } else {
                    isRoot = true;
                }
            }
            return pages;
        }
    }
}