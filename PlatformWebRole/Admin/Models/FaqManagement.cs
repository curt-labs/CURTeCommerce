using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Admin.Models;
using System.Reflection;

namespace Admin {
    partial class FAQ {

        internal static List<FAQ> GetAll() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.FAQs.OrderBy(x => x.order).ToList<FAQ>();
            } catch (Exception) {
                return new List<FAQ>();
            }
        }

        internal void Get() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            FAQ tmp = db.FAQs.Where(x => x.ID.Equals(this._ID)).FirstOrDefault<FAQ>();
            if (tmp != null && tmp.ID > 0) {
                this.ID = tmp.ID;
                this.question = tmp.question;
                this.answer = tmp.answer;
                this.order = tmp.order;
                this.topic = tmp.topic;
            }
        }

        internal void Save() {
            // Run our faq through the Sanitizer, just to make sure it doesn't have a dirty mouth
            UDF.Sanitize(this, new string[] { "faqtopic" });

            // Conntect to db
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();

            if (this.ID == 0) { // Insert new

                /****** 
                 * I'd like to be able to do a comparison of the submitted question to make sure don't have this question already, but fucking
                 * SQL Server won't allow us to do a comparison on Text data types. Bastards!!
                 *****/
                // Make sure we don't have a faq with the same question
                /*FAQ tmp = db.FAQs.Where(x => x.question.ToLower().Equals(this._question.ToLower())).FirstOrDefault<FAQ>();
                if (tmp != null && tmp._ID > 0) {
                    throw new Exception("Question exists, try again.");
                }*/

                this._order = db.FAQs.Select(x => x.order).Max();
                db.FAQs.InsertOnSubmit(this);
            } else { // Update existing
                FAQ exist = db.FAQs.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<FAQ>();
                if (exist == null || exist.ID == 0) {
                    throw new Exception("Failed to reference to existing question for identifier: " + this.ID);
                }
                exist.question = this.question;
                exist.answer = this.answer;
                exist.order = this.order;
                exist.topic = this.topic;
            }
            db.SubmitChanges();
            
        }

        internal void Delete() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            FAQ tmp = db.FAQs.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<FAQ>();

            db.FAQs.DeleteOnSubmit(tmp);
            db.SubmitChanges();
        }
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
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                FaqTopic tmp = db.FaqTopics.Where(x => x.ID.Equals(this._ID)).FirstOrDefault<FaqTopic>();
                /*foreach (PropertyInfo prop in this.GetType().GetProperties()) {
                    prop.SetValue(this,tmp,null);
                }*/
                this._ID = tmp.ID;
                this._topic = tmp.topic;
                this._dateAdded = tmp.dateAdded;
                this._FAQs = tmp.FAQs;
            } catch (Exception) {
                this._ID = 0;
                this._topic = "";
                this._dateAdded = DateTime.Now;
                this._FAQs = new System.Data.Linq.EntitySet<FAQ>();
            }
        }

        internal void Save() {

            // Run our topic through the Sanitizer and clean it up
            UDF.Sanitize(this, new string[]{""});

            // Connect to db
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            
            if (this._ID == 0) { // Insert new

                // Make sure we don't have topic with the same title
                FaqTopic tmp = db.FaqTopics.Where(x => x.topic.ToLower().Equals(this._topic.ToLower())).FirstOrDefault<FaqTopic>();
                if (tmp != null && tmp.ID > 0) {
                    throw new Exception("Topic exists, try again.");
                }
                this._dateAdded = DateTime.Now;
                db.FaqTopics.InsertOnSubmit(this);

            } else { // Update existing
                FaqTopic exist = db.FaqTopics.Where(x => x.ID.Equals(this._ID)).FirstOrDefault<FaqTopic>();
                if (exist == null || exist.ID == 0) {
                    throw new Exception("Failed to reference to existing topic for identifier: " + this._ID);
                }
                exist.topic = this._topic;
            }
            db.SubmitChanges();
        }

        internal void Delete() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();

            // We need to get all the questions that are tied to this topic and set them to zero
            List<FAQ> faqs = db.FAQs.Where(x => x.topic.Equals(this._ID)).ToList<FAQ>();
            foreach (FAQ faq in faqs) {
                faq.topic = 0;
            }

            // Get the topic to be deleted
            FaqTopic tmp = db.FaqTopics.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<FaqTopic>();

            db.FaqTopics.DeleteOnSubmit(tmp);
            db.SubmitChanges();
        }
    }
}