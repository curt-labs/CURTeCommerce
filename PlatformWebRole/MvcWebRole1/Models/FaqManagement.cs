using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommercePlatform {

    partial class FAQ {



    }

    partial class FaqTopic {
        internal static List<FaqTopic> GetAll() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.FaqTopics.OrderBy(x => x.topic).ToList<FaqTopic>();
            } catch (Exception) {
                return new List<FaqTopic>();
            }
        }

        internal void Get() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            if (this.ID > 0) {
                FaqTopic tmp = db.FaqTopics.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<FaqTopic>();
                this.ID = tmp.ID;
                this.topic = tmp.topic;
                this.dateAdded = tmp.dateAdded;
                this.FAQs = tmp.FAQs;
            } else if (this.topic != null && this.topic.Length > 0) {
                FaqTopic tmp = db.FaqTopics.Where(x => x.topic.Equals(this.topic)).FirstOrDefault<FaqTopic>();
                this.ID = tmp.ID;
                this.topic = tmp.topic;
                this.dateAdded = tmp.dateAdded;
                this.FAQs = tmp.FAQs;
            } else {
                this.ID = 0;
                this.topic = "";
                this.dateAdded = DateTime.Now;
            }
        }

    }
}