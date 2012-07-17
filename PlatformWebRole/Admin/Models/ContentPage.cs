using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin {
    partial class ContentPage {
        public List<ContentPage> getSubpages() {
            List<ContentPage> pages = new List<ContentPage>();
            List<ContentNesting> nestings = this.ContentNestings.ToList<ContentNesting>();
            foreach (ContentNesting n in nestings) {
                pages.AddRange(n.ContentPages.ToList<ContentPage>());
            }
            return pages;
        }

        public ContentPage getParent() {
            ContentPage page = new ContentPage();
            if (this.ContentNesting != null) {
                page = this.ContentNesting.ContentPage;
            }
            return page;
        }

        public Models.StrippedContent strip() {
            Models.StrippedContent stripped = new Models.StrippedContent {
                ID = this.ID,
                Title = this.Title,
                parent = (this.ContentNesting == null) ? new Models.StrippedContent() :
                            new Models.StrippedContent {
                                ID = this.ContentNesting.ContentPage.ID,
                                Title = this.ContentNesting.ContentPage.Title
                            },
                subpages = (from cn in this.ContentNestings
                            select new Models.StrippedContent {
                                ID = cn.ContentPage.ID,
                                Title = cn.ContentPage.Title
                            }).ToList<Models.StrippedContent>()
            };
            return stripped;
        }
    }
}