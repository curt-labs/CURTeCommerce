using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using Newtonsoft.Json;

namespace Admin.Controllers {
    public class PricingController : BaseController {

        public ActionResult Index() {

            PriceManagement priceManager = new PriceManagement();
            List<SimplePricing> pricePoints = priceManager.GetPricing();
            ViewBag.pricePoints = pricePoints;

            return View();
        }

        [NoValidation]
        public string SetList(int id = 0) {
            try {
                PriceManagement priceManager = new PriceManagement();
                SimplePricing pricePoint = priceManager.ResetToList(id);
                return JsonConvert.SerializeObject(pricePoint);
            } catch (Exception) {
                return "";
            }
        }

        [NoValidation]
        public string SetPrice(int partID = 0, decimal price = 0) {
            try {
                PriceManagement manager = new PriceManagement();
                SimplePricing pricePoint = manager.SetPrice(partID, price);
                return JsonConvert.SerializeObject(pricePoint);
            } catch (Exception e) {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                Response.StatusDescription = e.Message;
                Response.ContentType = "application/json";
                Response.Write(e.Message);
                Response.End();
                return "";
            }
        }

        [NoValidation]
        public string SetSale(int partID = 0, decimal price = 0, string sale_start = "", string sale_end = "") {
            try {
                PriceManagement manager = new PriceManagement();
                SimplePricing pricePoint = manager.SetPrice(partID, price,1,sale_start, sale_end);
                return JsonConvert.SerializeObject(pricePoint);
            } catch (Exception e) {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                Response.StatusDescription = e.Message;
                Response.ContentType = "application/json";
                Response.Write(e.Message);
                Response.End();
                return "";
            }
        }

        [NoValidation]
        public void RemoveSale(int partID = 0, decimal price = 0) {
            try {
                PriceManagement manager = new PriceManagement();
                manager.RemoveSale(partID, price);
            } catch (Exception e) {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                Response.StatusDescription = e.Message;
                Response.ContentType = "application/json";
                Response.Write(e.Message);
                Response.End();
            }
        }

    }
}
