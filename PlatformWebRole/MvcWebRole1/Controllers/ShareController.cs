using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using System.Net.Mail;
using System.Text;
using System.IO;
using System.Net;

namespace EcommercePlatform.Controllers {
    public class ShareController : BaseController {

        public ActionResult Index(string type = "", int partID = 0, bool layout = true, string error = "") {

            ViewBag.part = new APIPart();
            ViewBag.type = type;
            ViewBag.layout = layout;
            ViewBag.error = error;
            if (type.ToLower() != "site")
                ViewBag.part = CURTAPI.GetPart(partID);


            return View();
        }

        
        public dynamic SendMail(string type = "", int partID = 0, bool ajax = false) {
            try {
                Settings settings = ViewBag.settings;
                string sender = ((Request.Form["sender"] != null) ? Request.Form["sender"] : "");
                string msg = ((Request.Form["msg"] != null) ? Request.Form["msg"] : "");
                string recipient = ((Request.Form["recipient"] != null) ? Request.Form["recipient"] : "");
                string subj = ((Request.Form["subj"] != null) ? Request.Form["subj"] : "");

                if (sender == null || sender.Length == 0) { throw new Exception("You must enter your e-mail address"); }
                if (recipient == null || recipient.Length == 0) { throw new Exception("You must enter the recipient's e-mail address"); }
                if (msg == null || msg.Length == 0) { throw new Exception("You must enter a message"); }

                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head><link href='http://fonts.googleapis.com/css?family=Droid+Sans:400,700' rel='stylesheet' type='text/css'></head><body><div style=\"font-family:Droid Sans;width:95%;margin:auto;padding:10px;color:#343434;font-size:13px\">");

                if (type.ToLower() == "site") {
                    sb.Append("<span style=\"color:#FA241A;font-size:26px;display:block;padding:5px;height:30px;line-height:30px;border-bottom:2px solid #343434;margin-bottom:20px\">" + settings.Get("SiteURL") + "</span><br />");
                    sb.Append("<span style=\"display:block;\">Your friend &lt;" + sender + "&gt; wanted to share " + settings.Get("SiteName") + "</span><br />");
                    sb.Append("<p style=\"font-size:12px;color:#777777;padding:15px 0px\">\"" + msg + "\"</p>");
                    sb.Append("<span style=\"font-size:16px;display:block;\"><a href=\"" + settings.Get("SiteURL") + "\" style=\"color:#FA241A\">" + settings.Get("SiteName") + "</a> has been pleasing our customers since 1974.</span>");
                    sb.Append("<ul style=\"list-style:circle inside;margin:10px 0px\">");
                    sb.Append("<li style=\"padding:5px 0px\">The largest truck accessory retail showroom in the country!</li>");
                    sb.Append("<li style=\"padding:5px 0px\">Multiple conveniently located retail stores -  with full service installation facilities!</li>");
                    sb.Append("<li style=\"padding:5px 0px\">Privately owned warehouses with inventory to meet your needs!</li>");
                    sb.Append("<li style=\"padding:5px 0px\">Knowledgeable customer service teams – on call 6 days a week – providing 5 STAR SERVICE!</li>");
                    sb.Append("</ul>");
                } else {
                    APIPart part = CURTAPI.GetPart(partID);
                    sb.Append("<span style=\"display:block;\">Your friend &lt;" + sender + "&gt; wanted to share " + part.shortDesc + " on " + settings.Get("SiteName") + "</span><br />");
                    sb.Append("<p style=\"font-size:12px;color:#777777;padding:15px 0px\">\"" + msg + "\"</p>");
                    sb.Append("<span style=\"color:#FA241A;font-size:26px;display:block;padding:5px;height:30px;line-height:30px;border-bottom:2px solid #343434;margin-bottom:20px\">" + part.shortDesc + "</span>");
                    APIImage img = part.images.Where(x => x.size.Equals("Grande") && x.sort.Equals("a")).FirstOrDefault<APIImage>();
                    if (img != null && img.path != null && img.path.Length > 0) {
                        sb.Append("<img src=\"" + img.path + "\" alt=\"" + part.shortDesc + "\" style='width:150px; display:block;margin:10px' />");
                    }
                    if (part.attributes.Count > 0) {
                        sb.Append("<table style='width:300px;border-collapse:separate;border-spacing:4px'>");
                        foreach (APIAttribute att in part.attributes) {
                            sb.Append("<tr>");
                            sb.Append("<td style='background:#efefef;padding:2px 5px 2px 10px;text-align:right'>" + att.key + "</td>");
                            sb.Append("<td style='background:white;padding:2px 10px 2px 5px;text-align:left'>" + att.value + "</td>");
                            sb.Append("</tr>");
                        }
                        sb.Append("</table>");
                    }
                    sb.Append("<a href='"+HttpContext.Request.Url.Scheme+"://"+HttpContext.Request.Url.Host+"/Part/"+part.partID+"' title='View "+part.shortDesc+"'>View "+part.shortDesc+"</a><br />");

                    sb.Append("<span style=\"font-size:16px;display:block;margin-top:20px\"><a href=\"" + settings.Get("SiteURL") + "\" style=\"color:#FA241A\">" + settings.Get("SiteName") + "</a> has been pleasing our customers since 1974.</span>");
                    sb.Append("<ul style=\"list-style:circle inside;margin:10px 0px\">");
                    sb.Append("<li style=\"padding:5px 0px\">The largest truck accessory retail showroom in the country!</li>");
                    sb.Append("<li style=\"padding:5px 0px\">Multiple conveniently located retail stores -  with full service installation facilities!</li>");
                    sb.Append("<li style=\"padding:5px 0px\">Privately owned warehouses with inventory to meet your needs!</li>");
                    sb.Append("<li style=\"padding:5px 0px\">Knowledgeable customer service teams – on call 6 days a week – providing 5 STAR SERVICE!</li>");
                    sb.Append("</ul>");
                }
                sb.Append("<br /><hr /></div></body></html>");

                string[] recips = new string[]{recipient};
                UDF.SendEmail(recips, subj, true, sb.ToString(), false);

                if (ajax) {
                    return "";
                } else {
                    return RedirectToAction("Index", "Share", new { type = type, partID = partID, error = "Thank you for sharing!" });
                }
            } catch (Exception e) {
                if (ajax) {
                    return e.Message;
                } else {
                    return RedirectToAction("Index", "Share", new { type = type, partID = partID, error = e.Message });
                }
            }
        }

    }
}
