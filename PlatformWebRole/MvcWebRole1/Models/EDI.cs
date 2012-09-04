using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;

namespace EcommercePlatform.Models {
    public class EDI {
        internal void CreatePurchaseOrder(int id = 0) {
            if (!(HttpContext.Current.Request.Url.Host.Contains("127.0.0") || HttpContext.Current.Request.Url.Host.Contains("localhost"))) {
                try {
                    Settings settings = new Settings();
                    Cart order = new Cart().Get(id);
                    if (order.CartItems.Count > 0) {
                        Customer cust = new Customer { ID = order.cust_id };
                        cust.Get();
                        order.BindAddresses();
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
                        edicontent += "BEG*00*DS*" + order.payment_id + "**" + String.Format("{0:yyyyMMdd}", order.getPayment().created) + "~" + Environment.NewLine;
                        linecount++;
                        edicontent += "CUR*BT*USD~" + Environment.NewLine;
                        linecount++;
                        edicontent += "REF*CO*" + order.payment_id + "~" + Environment.NewLine;
                        linecount++;
                        edicontent += "REF*IA*" + settings.Get("CURTAccount") + "~" + Environment.NewLine;
                        linecount++;
                        edicontent += "DTM*002*" + String.Format("{0:yyyyMMdd}", order.getPayment().created) + "~" + Environment.NewLine;
                        linecount++;
                        edicontent += "N1*ST*" + order.Shipping.first + " " + order.Shipping.last + "~" + Environment.NewLine;
                        linecount++;
                        edicontent += "N3*" + order.Shipping.street1 + ((order.Shipping.street2 != null && order.Shipping.street2 != "") ? "*" + order.Shipping.street2 : "") + "~" + Environment.NewLine;
                        linecount++;
                        edicontent += "N4*" + order.Shipping.city + "*" + order.Shipping.State1.abbr + "*" + order.Shipping.postal_code + "*" + order.Shipping.State1.Country.longAbbr + "~" + Environment.NewLine;
                        if (!String.IsNullOrEmpty(cust.phone)) {
                            linecount++;
                            edicontent += "PER*BD*" + cust.fname + " " + cust.lname + "*TE*" + cust.phone + "~" + Environment.NewLine;
                        }
                        linecount++;
                        edicontent += "PER*BD*" + cust.fname + " " + cust.lname + "*EM*" + cust.email + "~" + Environment.NewLine;
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
                    }
                } catch { };
            }
        }

        internal void Read() {
            CloudBlobDirectory InFolder = BlobManagement.GetDirectory("edi/in");
            CloudBlobDirectory ArchiveFolder = BlobManagement.GetDirectory("edi/archive");
            foreach (CloudBlob blob in InFolder.ListBlobs()) {
                if(blob.GetType().ToString().ToUpper() != "CLOUDBLOBDIRECTORY" && !blob.Name.Contains(".req")) {
                    string editext = blob.DownloadText().Replace("\r\n", "");
                    if (blob.Name.ToLower().Contains("inv")) {
                        // invoice file
                        try {
                            ReadInvoice(editext);
                            BlobManagement.MoveBlob(blob, "edi/archive", "edi/in");
                        } catch (Exception e) {
                            string[] tos = new string [] {"jjaniuk@curtmfg.com"};
                            UDF.SendEmail(tos, "Error in Invoice Read", false, e.Message + " " + e.StackTrace, false);
                        }
                    } else if (blob.Name.ToLower().Contains("asn")) {
                        // ship notification
                        try {
                            ReadShippingNotification(editext);
                            BlobManagement.MoveBlob(blob, "edi/archive", "edi/in");
                        } catch { }
                    }
                }
            }

        }

        internal void ReadInvoice(string editext) {
            List<Invoice> invoices = new List<Invoice>();
            List<InvoiceItem> items = new List<InvoiceItem>();
            List<InvoiceCode> codes = new List<InvoiceCode>();


            Dictionary<string, string> itdcodes = new Dictionary<string, string>();
            itdcodes.Add("01","Basic");
            itdcodes.Add("02","End of Month");
            itdcodes.Add("03","Fixed Date");
            itdcodes.Add("04","Def. or installment");
            itdcodes.Add("05","Discount N/A");
            itdcodes.Add("08","Basic disc. offered");
            itdcodes.Add("09","Proximo");
            itdcodes.Add("12","10 days after EOM");
            itdcodes.Add("14","Previously agreed");
            itdcodes.Add("17","Terms N/A");
            itdcodes.Add("ZZ","Mutually Defined");

            Invoice inv = new Invoice();
            Address address = new Address();
            char billorship = 'b';
            List<string> edilines = editext.Split('~').ToList<string>();
            foreach (string line in edilines) {
                List<string> lineelements = line.Split('*').ToList<string>();
                switch (lineelements[0]) {
                    case "ST":
                        // Beginning of invoice
                        inv = new Invoice();
                        items = new List<InvoiceItem>();
                        codes = new List<InvoiceCode>();
                        break;
                    case "BIG":
                        // Beginning statement
                        string dt = lineelements[1].Substring(4,2) + "-" + lineelements[1].Substring(6,2) + "-" + lineelements[1].Substring(0,4);
                        inv.dateAdded = Convert.ToDateTime(dt);
                        inv.number = lineelements[2];
                        inv.orderID = lineelements[4];
                        switch (lineelements[7]) {
                            case "CN":
                                inv.invoiceType = "Credit Invoice";
                                break;
                            case "DI":
                                inv.invoiceType = "Debit Invoice";
                                break;
                            case "ZZ":
                                inv.invoiceType = "Mutually Defined";
                                break;
                        }
                        break;
                    case "CUR":
                        inv.billToCurrency = lineelements[2];
                        break;
                    case "REF":
                        inv.curtOrder = Convert.ToInt32(lineelements[2].Trim());
                        break;
                    case "N1":
                        switch (lineelements[1]) {
                            case "RI":
                                inv.remitTo = lineelements[4];
                                break;
                            case "BT":
                                billorship = 'b';
                                address = new Address();
                                string[] namesplit = lineelements[2].Split(' ');
                                address.first = namesplit[0];
                                try {
                                    address.last = namesplit[1];
                                } catch {
                                    address.last = "";
                                }
                                break;
                            case "ST":
                                billorship = 's';
                                address = new Address();
                                namesplit = lineelements[2].Split(' ');
                                address.first = namesplit[0];
                                try {
                                    address.last = namesplit[1];
                                } catch {
                                    address.last = "";
                                }
                                break;
                        }
                        break;
                    case "N2":
                        break;
                    case "N3":
                        address.street1 = lineelements[1];
                        try {
                            address.street2 = lineelements[2];
                        } catch {
                            address.street2 = "";
                        }
                        break;
                    case "N4":
                        address.city = lineelements[1];
                        address.state = new State().getStateIDByAbbr(lineelements[2]);
                        address.postal_code = lineelements[3];
                        address.MatchOrSave();
                        if (billorship == 'b')
                            inv.billTo = address.ID;
                        else
                            inv.shipTo = address.ID;
                            break;
                    case "ITD":
                        // invoice due statement
                        inv.termsType = itdcodes[lineelements[1]];
                        inv.discountPercent = Convert.ToDecimal(lineelements[3]);
                        inv.discountDueDate = Convert.ToDateTime(lineelements[4].Substring(4, 2) + "-" + lineelements[4].Substring(6, 2) + "-2" + lineelements[4].Substring(1, 3));
                        inv.discountDueDays = Convert.ToInt32(lineelements[5]);
                        inv.netDueDate = Convert.ToDateTime(lineelements[6].Substring(4,2) + "-" + lineelements[6].Substring(6,2) + "-2" + lineelements[6].Substring(1,3));
                        inv.netDueDays = Convert.ToInt32(lineelements[7]);
                        inv.termsDescription = lineelements[12];
                        break;
                    case "IT1":
                        // item in the invoice
                        InvoiceItem i = new InvoiceItem();
                        try {
                            i.quantity = Convert.ToInt32(lineelements[2]);
                        } catch {
                            i.quantity = 0;
                        }
                        i.price = Convert.ToDecimal(lineelements[4]);
                        i.partID = lineelements[9];
                        i.description = lineelements[15];
                        /*InvoiceItem i = new InvoiceItem {
                            quantity = Convert.ToInt32(lineelements[2]),
                            price = Convert.ToDecimal(lineelements[4]),
                            partID = lineelements[9],
                            description = lineelements[15]
                        };*/
                        items.Add(i);
                        break;
                    case "TDS":
                        // totals
                        inv.total = (Convert.ToDecimal(lineelements[1]) / 100);
                        try {
                            inv.subtotal = (Convert.ToDecimal(lineelements[2]) / 100);
                        } catch { };
                        try {
                            inv.discountTotal = (Convert.ToDecimal(lineelements[3]) / 100);
                        } catch { };
                        try {
                            inv.discount = (Convert.ToDecimal(lineelements[4]) / 100);
                        } catch { };
                        break;
                    case "TXI":
                        // special tax line for canadians
                        inv.salesTax = Convert.ToDecimal(lineelements[2]);
                        break;
                    case "SAC":
                        // special codes
                        InvoiceCode c = new InvoiceCode {
                            type = (lineelements[1] == "C") ? "Charge" : "Allowance",
                            code = lineelements[2],
                            value = (Convert.ToDecimal(lineelements[5]) / 100),
                            description = lineelements[15]
                        };
                        codes.Add(c);
                        break;
                    case "SE":
                        // End of Invoice
                        inv.Save(items,codes);
                        break;
                }
            }
        }

        internal void ReadShippingNotification(string editext) {
            string trackingcode = "";
            string purchaseOrderID = "";
            string shipmentNumber = "";
            string weight = "";
            Cart order = new Cart();
            List<Shipment> shipments = new List<Shipment>();
            DateTime shipdate = DateTime.Now;

            List<string> edilines = editext.Split('~').ToList<string>();
            foreach (string line in edilines) {
                List<string> lineelements = line.Split('*').ToList<string>();
                switch (lineelements[0]) {
                    case "ST":
                        // Beginning of invoice
                        order = new Cart();
                        shipments = new List<Shipment>();
                        weight = "";
                        break;
                    case "BSN":
                        // Original Shipment Number from Shipper
                        shipmentNumber = lineelements[2];
                        break;
                    case "PRF":
                        // Purchase Order Reference
                        purchaseOrderID = lineelements[1];
                        break;
                    case "REF":
                        // Tracking Code reference
                        trackingcode = lineelements[2];
                        Shipment shipment = new Shipment {
                            tracking_number = trackingcode
                        };
                        shipments.Add(shipment);
                        break;
                    case "DTM":
                        shipdate = Convert.ToDateTime(lineelements[2].Substring(4, 2) + "/" + lineelements[2].Substring(6, 2) + "/20" + lineelements[2].Substring(2, 2));
                        break;
                    case "TD1":
                        weight = lineelements[7] + " " + lineelements[8];
                        break;
                    case "SE":
                        // End of Invoice
                        try {
                            order = new Cart().GetByPaymentID(Convert.ToInt32(purchaseOrderID));
                            foreach (Shipment s in shipments) {
                                s.order_id = order.ID;
                                s.dateShipped = shipdate;
                                s.shipment_number = shipmentNumber;
                                s.weight = weight;
                            }
                            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                            db.Shipments.InsertAllOnSubmit(shipments);
                            db.SubmitChanges();
                            order.SendShippingNotification();
                        } catch { }
                        break;
                }
            }
        }
    }

}