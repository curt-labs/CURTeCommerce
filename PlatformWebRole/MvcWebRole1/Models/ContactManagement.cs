using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using EcommercePlatform.Models;

namespace EcommercePlatform {
    partial class ContactInquiry {

        /// <summary>
        /// Save the ContactInquiry to the database
        /// </summary>
        /// <param name="send_mail">If true, send e-mail to ContactType</param>
        internal void Save(bool send_mail = true,string send_to = null) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            db.ContactInquiries.InsertOnSubmit(this);
            db.SubmitChanges();
            if (send_mail) {
                SendMail(send_to);
            }
        }

        internal void SendMail(string send_to = null) {
            // Retrieve the 'to' based on the contact_type reference
            ContactType type = GetContactType();
            string[] to = new string[] { "" };
            if (send_to != null) {
                to = new string[] { send_to };
                type.label = send_to;
            }else{
                to = new string[]{ type.email };
            }
            string subject = "New Contact Inquiry { " + type.label + "}";

            StringBuilder sb = new StringBuilder();
            sb.Append("<div style='border:1px solid #e6e6e6'>");
            sb.AppendFormat("<span style='font-size:16px;'>{0} Inquiry</span>",type.label);
            sb.Append("<div style='margin:15px 0px 10px 10px;font-size:13px'>");
            sb.AppendFormat("<span style='border:1px solid #e9e9e9;margin:5px'><strong>Name: </strong>{0}</span><br />",this._name);
            sb.AppendFormat("<span style='border:1px solid #e9e9e9;margin:5px'><strong>Phone: </strong></span><br />",this._phone);
            sb.AppendFormat("<span style='border:1px solid #e9e9e9;margin:5px'><strong>E-Mail: </strong>{0}</span><br />", this._email);
            sb.Append("</div>");
            sb.AppendFormat("<p style='margin:5px; border:1px solid #e9e9e9; padding:5px'>{0}</p>",this._message);
            sb.Append("</div>");

            UDF.SendEmail(to, subject, true, sb.ToString());
        }

        internal ContactType GetContactType() {
            ContactType type = new ContactType { ID = this._contact_type };
            return type.Get();
        }
    }

    partial class ContactType {
        internal static List<ContactType> GetAll() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                return db.ContactTypes.OrderBy(x => x.label).ToList<ContactType>();
            } catch (Exception) {
                return new List<ContactType>();
            }
        }

        internal ContactType Get() {
            try {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                if (this._ID > 0) {
                    return db.ContactTypes.Where(x => x.ID.Equals(this._ID)).FirstOrDefault<ContactType>();
                } else if (this._label != null && this._label.Length > 0) {
                    return db.ContactTypes.Where(x => x.label.Equals(this._label)).FirstOrDefault<ContactType>();
                } else if (this._email != null && this._email.Length > 0) {
                    return db.ContactTypes.Where(x => x.email.Equals(this._email)).FirstOrDefault<ContactType>();
                }
                return new ContactType();
            } catch (Exception) {
                return new ContactType();
            }
        }
    }
}