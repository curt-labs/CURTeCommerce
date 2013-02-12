using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin {
    partial class ContactInquiry {
        internal static List<ContactInquiry> GetAll(){
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            return db.ContactInquiries.ToList<ContactInquiry>();
        }

        internal ContactInquiry Get() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                if (this._ID > 0) {
                    return db.ContactInquiries.Where(x => x.ID.Equals(this._ID)).FirstOrDefault<ContactInquiry>();
                }
                return new ContactInquiry();
            } catch (Exception) {
                return new ContactInquiry();
            }
        }

        internal void Delete() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            if (this._ID > 0) {
                ContactInquiry inq = db.ContactInquiries.Where(x => x.ID.Equals(this._ID)).FirstOrDefault<ContactInquiry>();
                db.ContactInquiries.DeleteOnSubmit(inq);
                db.SubmitChanges();
            } else {
                throw new Exception("Invalid reference to inquiry");
            }
        }

        internal void MarkResponded() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            if (this._ID > 0) {
                ContactInquiry inq = db.ContactInquiries.Where(x => x.ID.Equals(this._ID)).FirstOrDefault<ContactInquiry>();
                inq.followedUp = 1;
                db.SubmitChanges();
            } else {
                throw new Exception("Invalid reference to inquiry");
            }
        }
    }

    partial class ContactType {
        

        internal static List<ContactType> GetAll() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            return db.ContactTypes.OrderBy(x => x.email).ToList<ContactType>();
        }

        internal void Add() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();

            // Validate fields
            if (this._label == null || this._label.Trim().Length == 0) {
                throw new Exception("Label cannot be empty.");
            }
            if (this._email == null || this._email.Trim().Length == 0) {
                throw new Exception("E-Mail cannot be empty.");
            }

            // Make sure we don't have a label with the same title
            ContactType exist = db.ContactTypes.Where(x => x.label.ToLower().Equals(this._label.Trim().ToLower())).FirstOrDefault<ContactType>();
            if (exist != null && exist.ID > 0) {
                throw new Exception("A contact type with this label exists.");
            }

            db.ContactTypes.InsertOnSubmit(this);
            db.SubmitChanges();
        }

        internal void Delete() {
            if (this._ID == 0) {
                throw new Exception("Inavlid refernce to contact type, must set the ContactType ID field.");
            }
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            ContactType type = db.ContactTypes.Where(x => x.ID.Equals(this._ID)).FirstOrDefault<ContactType>();
            db.ContactTypes.DeleteOnSubmit(type);
            db.SubmitChanges();
        }
    }

    public class SimpleInquiry {
        public int ID { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string message { get; set; }
        public string type { get; set; }
        public string dateAdded { get; set; }
        public int followedUp { get; set; }
    }
}