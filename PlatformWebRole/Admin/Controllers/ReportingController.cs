using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Net;
using System.IO;
using Admin.Models;

namespace Admin.Controllers {
    public class ReportingController : BaseController {

        public ActionResult Index() {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void OrdersByDateRange(string startdate = "", string enddate = "") {
            DateTime start = Convert.ToDateTime(startdate);
            DateTime end = Convert.ToDateTime(enddate);

            List<Cart> orders = Reporting.GetOrdersByDateRange(start,end);

            StringWriter writer = new StringWriter();


            writer.WriteLine("Order ID,Order Date,SKU,Quantity,Item Name,Price (each),Vendor ID,Vendor Name,Customer ID,Customer Name,Customer Email,Customer Phone,Billing Name,Billing Address 1,Billing Address 2,Billing City,Billing State,Billing Zip,Shipping Name,Shipping Address 1,Shipping Address 2,Shipping City,Shipping State,Shipping Zip,Subtotal,Tax,Shipping Cost,Total");
            foreach (Cart order in orders) {
                foreach (CartItem item in order.CartItems) {
                    try {
                        writer.WriteLine(order.payment_id + "," + String.Format("{0:M/dd/yyyy H:mm}", order.Payment.created) + "," + item.partID + "," + item.quantity + "," + item.shortDesc.Replace(",", "") + "," + String.Format("{0:C}", item.price) + ",,," + order.cust_id + "," + order.Customer.fname + " " + order.Customer.lname + "," +
                            order.Customer.email + "," + order.Customer.phone + "," + order.Billing.first.Replace(",", "") + " " + order.Billing.last.Replace(",", "") + "," + order.Billing.street1.Replace(",", "") + "," + order.Billing.street2.Replace(",", "") + "," + order.Billing.city.Replace(",", "") + "," + order.Billing.State1.abbr + "," + order.Billing.postal_code + "," +
                            order.Shipping.first.Replace(",", "") + " " + order.Shipping.last.Replace(",", "") + "," + order.Shipping.street1.Replace(",", "") + "," + order.Shipping.street2.Replace(",", "") + "," + order.Shipping.city.Replace(",", "") + "," + order.Shipping.State1.abbr + "," + order.Shipping.postal_code + "," + String.Format("{0:C}", order.GetSubTotal()) + "," +
                            String.Format("{0:C}", order.tax) + "," + String.Format("{0:C}", order.shipping_price) + "," + String.Format("{0:C}", order.getTotal()));
                    } catch { };
                }
            }
            string attachment = "attachment; filename=OrdersByDateRange-" + String.Format("{0:MMddyyyyHHmmss}",DateTime.Now) + ".csv";
            HttpContext.Response.Clear();
            HttpContext.Response.ClearHeaders();
            HttpContext.Response.ClearContent();
            HttpContext.Response.AddHeader("content-disposition", attachment);
            HttpContext.Response.ContentType = "text/csv";
            HttpContext.Response.AddHeader("Pragma", "public");
            HttpContext.Response.Write(writer.ToString());
            HttpContext.Response.End();
        }

        [NoValidation]
        public void InvoiceByID(string invoiceNumber = "") {
            Invoice invoice = Reporting.GetInvoiceByID(invoiceNumber);

            StringWriter writer = new StringWriter();

            writer.WriteLine("Invoice Number|Invoice Date|orderID|CURT Order|Invoice Type|remit To|currency|subtotal|sales tax|total|discount amount|discount total|terms type|terms description|discount percent|discount due date|discount days due|net due date|net days due|Bill To|Billing Street 1|Billing Street 2|Billing City|Billing Province|Billing Postal|Billing Country|Ship To|Shipping Street|Shipping City|Shipping Province|Shipping Postal|Shipping Country|Shipping Phone");
            int loopcount = invoice.InvoiceItems.Count;
            if (invoice.InvoiceCodes.Count > loopcount) {
                loopcount = invoice.InvoiceCodes.Count;
            }
            List<InvoiceCode> codes = invoice.InvoiceCodes.OrderBy(x => x.code).ToList<InvoiceCode>();
            for (int i = 0; i < loopcount; i++) {
                string iline = invoice.number + "|" + String.Format("{0:MM-dd-yyyy}", invoice.dateAdded) + "|" + invoice.orderID + "|" + invoice.curtOrder + "|" + invoice.invoiceType + "|" + invoice.remitTo + "|" + invoice.billToCurrency + "|" + invoice.subtotal + "|" + invoice.salesTax + "|" + invoice.total + "|" + invoice.discount + "|" + invoice.discountTotal + "|" + invoice.termsType + "|" + invoice.termsDescription + "|" + invoice.discountPercent + "|" + String.Format("{0:MM-dd-yyyy}", invoice.discountDueDate) + "|" + invoice.discountDueDays + "|" + String.Format("{0:MM-dd-yyyy}", invoice.netDueDate) + "|" + invoice.netDueDays + "|" + invoice.BillTo.first + " " + invoice.BillTo.last + "|" + invoice.BillTo.street1 + "|" + invoice.BillTo.city + "|" + invoice.BillTo.state + "|" + invoice.BillTo.postal_code + "|" + invoice.BillTo.country + "|" + invoice.ShipTo.first + " " + invoice.ShipTo.last + "|" + invoice.ShipTo.street1 + "|" + invoice.ShipTo.street2 + "|" + invoice.ShipTo.city + "|" + invoice.ShipTo.state + "|" + invoice.ShipTo.postal_code + "|" + invoice.ShipTo.country + "|" + invoice.ShipTo.street2 + "|";
                try {
                    iline += invoice.InvoiceItems[i].partID + "|" + invoice.InvoiceItems[i].quantity + "|" + invoice.InvoiceItems[i].price.ToString("0.00") + "|" + invoice.InvoiceItems[i].description + "|";
                } catch {
                    iline += "||||";
                }
                try {
                    iline += codes[i].type + "|" + codes[i].code + "|" + codes[i].value + "|" + codes[i].description;
                } catch {
                    iline += "|||";
                }
                writer.WriteLine(iline);
            }
            string attachment = "attachment; filename=Invoice-" + invoiceNumber + ".txt";
            HttpContext.Response.Clear();
            HttpContext.Response.ClearHeaders();
            HttpContext.Response.ClearContent();
            HttpContext.Response.AddHeader("content-disposition", attachment);
            HttpContext.Response.ContentType = "text/csv";
            HttpContext.Response.AddHeader("Pragma", "public");
            HttpContext.Response.Write(writer.ToString());
            HttpContext.Response.End();
        }
        
        [NoValidation]
        public void DailyOrderReport() {
            DateTime start = DateTime.Now.AddDays(-1);
            DateTime end = DateTime.Now;
            Settings settings = new Settings();

            List<Cart> orders = Reporting.GetOrdersByDateRange(start, end);

            StringWriter writer = new StringWriter();

            writer.WriteLine("Order ID,Order Date,SKU,Quantity,Item Name,Price (each),Vendor ID,Vendor Name,Customer ID,Customer Name,Customer Email,Customer Phone,Billing Name,Billing Address 1,Billing Address 2,Billing City,Billing State,Billing Zip,Shipping Name,Shipping Address 1,Shipping Address 2,Shipping City,Shipping State,Shipping Zip,Subtotal,Tax,Shipping Cost,Total");
            foreach (Cart order in orders) {
                foreach (CartItem item in order.CartItems) {
                    try {
                        writer.WriteLine(order.payment_id + "," + String.Format("{0:M/dd/yyyy H:mm}", order.Payment.created) + "," + item.partID + "," + item.quantity + "," + item.shortDesc.Replace(",", "") + "," + String.Format("{0:C}", item.price) + ",,," + order.cust_id + "," + order.Customer.fname + " " + order.Customer.lname + "," +
                            order.Customer.email + "," + order.Customer.phone + "," + order.Billing.first.Replace(",", "") + " " + order.Billing.last.Replace(",", "") + "," + order.Billing.street1.Replace(",", "") + "," + order.Billing.street2.Replace(",", "") + "," + order.Billing.city.Replace(",", "") + "," + order.Billing.State1.abbr + "," + order.Billing.postal_code + "," +
                            order.Shipping.first.Replace(",", "") + " " + order.Shipping.last.Replace(",", "") + "," + order.Shipping.street1.Replace(",", "") + "," + order.Shipping.street2.Replace(",", "") + "," + order.Shipping.city.Replace(",", "") + "," + order.Shipping.State1.abbr + "," + order.Shipping.postal_code + "," + String.Format("{0:C}", order.GetSubTotal()) + "," +
                            String.Format("{0:C}", order.tax) + "," + String.Format("{0:C}", order.shipping_price) + "," + String.Format("{0:C}", order.getTotal()));
                    } catch { };
                }
            }

            if (orders.Count > 0) {
                string filename = string.Format("/OrdersByDateRange-{0}.csv", String.Format("{0:MMddyyyyHHmmss}", DateTime.Now));
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                byte[] fileContents = encoding.GetBytes(writer.ToString());

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(settings.Get("FTPServer") + settings.Get("FTPOrderPath") + filename);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                request.Credentials = new NetworkCredential(settings.Get("FTPUser"), settings.Get("FTPPass"));

                // Copy the contents of the file to the request stream.
                Stream s = new MemoryStream(fileContents);
                request.ContentLength = s.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }

        }

        [NoValidation]
        public void AutomatedInvoiceReport(string startdate = "", string enddate = "") {
            DateTime start = new DateTime();
            DateTime end = new DateTime();
            Settings settings = new Settings();
            if (startdate == "") {
                start = DateTime.Now.AddDays(-1);
            } else {
                start = Convert.ToDateTime(startdate);
            }
            if (enddate == "") {
                end = DateTime.Now;
            } else {
                end = Convert.ToDateTime(enddate);
            }
            List<Invoice> invoices = Reporting.GetInvoicesByDateRange(start, end);

            foreach (Invoice invoice in invoices) {

                StringWriter writer = new StringWriter();

                writer.WriteLine("Invoice Number|Invoice Date|orderID|CURT Order|Invoice Type|remit To|currency|subtotal|sales tax|total|discount amount|discount total|terms type|terms description|discount percent|discount due date|discount days due|net due date|net days due|Bill To|Billing Street 1|Billing Street 2|Billing City|Billing Province|Billing Postal|Billing Country|Ship To|Shipping Street|Shipping City|Shipping Province|Shipping Postal|Shipping Country|Shipping Phone|PartID|Quantity|price|Description|CodeType|code|value|Description");
                int loopcount = invoice.InvoiceItems.Count;
                if (invoice.InvoiceCodes.Count > loopcount) {
                    loopcount = invoice.InvoiceCodes.Count;
                }
                List<InvoiceCode> codes = invoice.InvoiceCodes.OrderBy(x => x.code).ToList<InvoiceCode>();
                for (int i = 0; i < loopcount; i++) {
                    string iline = invoice.number + "|" + String.Format("{0:MM-dd-yyyy}", invoice.dateAdded) + "|" + invoice.orderID + "|" + invoice.curtOrder + "|" + invoice.invoiceType + "|" + invoice.remitTo + "|" + invoice.billToCurrency + "|" + invoice.subtotal + "|" + invoice.salesTax + "|" + invoice.total + "|" + invoice.discount + "|" + invoice.discountTotal + "|" + invoice.termsType + "|" + invoice.termsDescription + "|" + invoice.discountPercent + "|" + String.Format("{0:MM-dd-yyyy}", invoice.discountDueDate) + "|" + invoice.discountDueDays + "|" + String.Format("{0:MM-dd-yyyy}", invoice.netDueDate) + "|" + invoice.netDueDays + "|" + invoice.BillTo.first + " " + invoice.BillTo.last + "|" + invoice.BillTo.street1 + "|" + invoice.BillTo.city + "|" + invoice.BillTo.state + "|" + invoice.BillTo.postal_code + "|" + invoice.BillTo.country + "|" + invoice.ShipTo.first + " " + invoice.ShipTo.last + "|" + invoice.ShipTo.street1 + "|" + invoice.ShipTo.street2 + "|" + invoice.ShipTo.city + "|" + invoice.ShipTo.state + "|" + invoice.ShipTo.postal_code + "|" + invoice.ShipTo.country + "|" + invoice.ShipTo.street2 + "|";
                    try {
                        iline += invoice.InvoiceItems[i].partID + "|" + invoice.InvoiceItems[i].quantity + "|" + invoice.InvoiceItems[i].price.ToString("0.00") + "|" + invoice.InvoiceItems[i].description + "|";
                    } catch {
                        iline += "||||";
                    }
                    try {
                        iline += codes[i].type + "|" + codes[i].code + "|" + codes[i].value + "|" + codes[i].description;
                    } catch {
                        iline += "|||";
                    }
                    writer.WriteLine(iline);
                }

                string filename = "/invoice-" + invoice.number + ".csv";
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                byte[] fileContents = encoding.GetBytes(writer.ToString());

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(settings.Get("FTPServer") + settings.Get("FTPInvoicePath") + filename);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                request.Credentials = new NetworkCredential(settings.Get("FTPUser"), settings.Get("FTPPass"));

                // Copy the contents of the file to the request stream.
                Stream s = new MemoryStream(fileContents);
                request.ContentLength = s.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }

        }
    }
}
