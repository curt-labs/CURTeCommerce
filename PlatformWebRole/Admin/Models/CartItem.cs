using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using Admin.Models;

namespace Admin {
    partial class CartItem {
        public APIPart apipart { get; set; }
        public CartItem(int partID = 0, int quantity = 0, decimal price = 0, string shortDesc = "", string upc = "") {
            this.partID = partID;
            this.quantity = quantity;
            this.price = price;
            this.shortDesc = shortDesc;
            this.upc = upc;
        }

        public string GetImage() {
            try {
                string image = "";
                WebClient wc = new WebClient();
                StringBuilder sb = new StringBuilder(System.Configuration.ConfigurationManager.AppSettings["CURT_API_DOMAIN"]);
                sb.AppendFormat("GetPart?partID={0}&dataType=JSON", this.partID);

                string resp = wc.DownloadString(sb.ToString());
                APIPart part = Newtonsoft.Json.JsonConvert.DeserializeObject<APIPart>(resp);
                image = part.images.Where(x => x.size.Equals("Grande") && x.sort.ToString().Contains('a')).Select(x => x.path).FirstOrDefault<string>();

                if (image == null) {
                    foreach (APIImage img in part.images) {
                        image = img.path;
                        break;
                    }
                }

                return image;
            } catch (Exception e) {
                return e.Message;
            }
        }


    }

}