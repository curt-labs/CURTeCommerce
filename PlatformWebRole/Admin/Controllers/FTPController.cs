using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using Admin.Controllers;
using System.Web.Script.Serialization;

namespace Admin.Controllers {
    public class FTPController : BaseController {

        public ActionResult Index() {
            List<FTPFirewall> ipaddresses = new List<FTPFirewall>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            ipaddresses = db.FTPFirewalls.ToList<FTPFirewall>();
            ViewBag.ipaddresses = ipaddresses;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Add(string ipaddress) {
            if (ipaddress != null && ipaddress.Trim() != "") {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                // check if ip already exists
                FTPFirewall ip = new FTPFirewall();
                try {
                    ip = db.FTPFirewalls.Where(x => x.ipaddress.Equals(ipaddress.Trim())).First<FTPFirewall>();
                } catch {
                    ip.ipaddress = ipaddress;
                    db.FTPFirewalls.InsertOnSubmit(ip);
                    db.SubmitChanges();
                }
            }
            return RedirectToAction("Index");
        }

        [NoValidation]
        public ActionResult Delete(int id) {
            if (id > 0) {
                EcommercePlatformDataContext db = new EcommercePlatformDataContext();
                FTPFirewall ipaddress = db.FTPFirewalls.Where(x => x.ID.Equals(id)).FirstOrDefault<FTPFirewall>();
                db.FTPFirewalls.DeleteOnSubmit(ipaddress);
                db.SubmitChanges();
            }
            return RedirectToAction("Index");
        }

    }
}
