using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;

namespace Admin.Models {
    public class EDI {
        internal void CreatePurchaseOrder(int id = 0) {
            if (!(HttpContext.Current.Request.Url.Host.Contains("127.0.0") || HttpContext.Current.Request.Url.Host.Contains("localhost"))) {
                try {
                    Cart order = new Cart().Get(id);
                    Settings settings = new Settings();
                    CloudBlob blob = null;
                    string edicontent = "";
                    int linecount = 1;
                    // linecount is just for the PO section and doesn't include the head or tail
                    // next two lines are head
                    edicontent += "ISA*00*          *00*          *12*" + settings.Get("EDIPhone") + "     *01*809988975      *" + String.Format("{0:yyMMdd}*{0:hhmm}", DateTime.Now) + "*U*00401*" + order.payment_id.ToString("000000000") + "*0*P*>~" + Environment.NewLine;
                    edicontent += "GS*PO*" + settings.Get("EDIPhone") + "*809988975*" + String.Format("{0:yyyyMMdd}*{0:hhmm}", DateTime.Now) + "*" + order.payment_id.ToString("000000000") + "*X*004010~" + Environment.NewLine;
                    // begin PO section
                    edicontent += "ST*850*" + order.payment_id + "~" + Environment.NewLine;
                    linecount++;
                    edicontent += "BEG*00*DS*" + order.payment_id + "**" + String.Format("{0:yyyyMMdd}", order.Payment.created) + "~" + Environment.NewLine;
                    linecount++;
                    edicontent += "CUR*BT*USD~" + Environment.NewLine;
                    linecount++;
                    edicontent += "REF*CO*" + order.payment_id + "~" + Environment.NewLine;
                    linecount++;
                    edicontent += "REF*IA*" + settings.Get("CURTAccount") + "~" + Environment.NewLine;
                    linecount++;
                    edicontent += "DTM*002*" + String.Format("{0:yyyyMMdd}", order.Payment.created) + "~" + Environment.NewLine;
                    linecount++;
                    edicontent += "N1*ST*" + order.Shipping.first + " " + order.Shipping.last + "~" + Environment.NewLine;
                    linecount++;
                    edicontent += "N3*" + order.Shipping.street1 + ((order.Shipping.street2 != null && order.Shipping.street2 != "") ? "*" + order.Shipping.street2 : "") + "~" + Environment.NewLine;
                    linecount++;
                    edicontent += "N4*" + order.Shipping.city + "*" + order.Shipping.State1.abbr + "*" + order.Shipping.postal_code + "*" + order.Shipping.State1.Country.longAbbr + "~" + Environment.NewLine;
                    if (!String.IsNullOrEmpty(order.Customer.phone)) {
                        linecount++;
                        edicontent += "PER*BD*" + order.Customer.fname + " " + order.Customer.lname + "*TE*" + order.Customer.phone + "~" + Environment.NewLine;
                    }
                    linecount++;
                    edicontent += "PER*BD*" + order.Customer.fname + " " + order.Customer.lname + "*EM*" + order.Customer.email + "~" + Environment.NewLine;
                    linecount++;
                    edicontent += "TD5**2*FDEG*P*" + order.shipping_type + "~" + Environment.NewLine;
                    for (int i = 0; i < order.CartItems.Count; i++) {
                        linecount++;
                        edicontent += "PO1*" + (i + 1).ToString("000") + "*" + order.CartItems[i].quantity + "*EA***BP*" + order.CartItems[i].partID + "*VP*" + order.CartItems[i].partID + "*UP*" + order.CartItems[i].upc + "~" + Environment.NewLine;
                        linecount++;
                        edicontent += "CTP*PUR*" + String.Format("{0:0.00}", order.CartItems[i].price) + "~" + Environment.NewLine;
                        linecount++;
                        edicontent += "PID*F*08***" + order.CartItems[i].shortDesc + "~" + Environment.NewLine;
                    }
                    linecount++;
                    edicontent += "CTT*" + order.CartItems.Count + "*" + order.getCount() + "~" + Environment.NewLine;
                    linecount++;
                    edicontent += "SE*" + linecount + "*" + order.payment_id + "~" + Environment.NewLine;
                    // end PO section
                    // begin Tail section
                    edicontent += "GE*1*" + order.payment_id.ToString("000000000") + "~" + Environment.NewLine;
                    edicontent += "IEA*1*" + order.payment_id.ToString("000000000") + "~";

                    // write file
                    DiscountBlobContainer blobcontainer = BlobManagement.GetContainer("edi");
                    BlobContainerPermissions perms = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob };
                    blobcontainer.Container.SetPermissions(perms);
                    blob = blobcontainer.Container.GetBlobReference(string.Format("out\\PO{0}_{1}.txt", String.Format("{0:yyyyMMdd}", DateTime.Now), String.Format("{0:HHmmss}", DateTime.Now)));
                    blob.UploadText(edicontent);
                } catch { };
            }
        }
    }
}