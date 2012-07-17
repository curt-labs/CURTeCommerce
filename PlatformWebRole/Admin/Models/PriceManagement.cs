using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Admin.Models {
    public class PriceManagement {

        private string api_key = new Settings().Get("CURTAPIKey");
        private string api_domain = System.Configuration.ConfigurationManager.AppSettings["CURT_API_DOMAIN"];
        private string customerID = new Settings().Get("CURTAccount");

        internal List<SimplePricing> GetPricing() {
            try {
                List<SimplePricing> priceList = new List<SimplePricing>();

                using (System.Net.WebClient client = new System.Net.WebClient()) {
                    StringBuilder sb = new StringBuilder(api_domain);
                    sb.AppendFormat("GetPricing?key={0}&customerID={1}", api_key, customerID);

                    string resp = client.DownloadString(sb.ToString());
                    priceList = JsonConvert.DeserializeObject<List<SimplePricing>>(resp);
                }
                return priceList;
            } catch (Exception e) {
                return new List<SimplePricing>();
            }
        }

        internal SimplePricing SetPrice(int partID, decimal price, int isSale = 0, string sale_start = "", string sale_end = "") {
            SimplePricing point = new SimplePricing();
            StringBuilder sb = new StringBuilder(api_domain);
            sb.AppendFormat("SetPrice?key={0}", api_key);

            Dictionary<string, string> post_data = new Dictionary<string, string>();
            post_data.Add("customerID", customerID.ToString());
            post_data.Add("price", price.ToString());
            post_data.Add("partID", partID.ToString());
            post_data.Add("isSale", isSale.ToString());
            post_data.Add("sale_start", (sale_start.Length == 0)?DateTime.Now.ToString():sale_start);
            post_data.Add("sale_end", (sale_end.Length == 0)?DateTime.Now.ToString():sale_end);

            string json = UDF.POSTRequest(sb.ToString(), post_data);
            try {
                point = JsonConvert.DeserializeObject<SimplePricing>(json);
            } catch (Exception) {
                throw new Exception(json);
            }
            
            return point;
        }

        internal SimplePricing ResetToList(int id) {
            SimplePricing point = new SimplePricing();
            StringBuilder sb = new StringBuilder(api_domain);
            sb.AppendFormat("ResetToList?key={0}", api_key);

            Dictionary<string, string> post_data = new Dictionary<string, string>();
            post_data.Add("customerID", customerID.ToString());
            post_data.Add("partID", id.ToString());

            string json = UDF.POSTRequest(sb.ToString(), post_data);

            point = JsonConvert.DeserializeObject<SimplePricing>(json);

            return point;
        }

        internal void RemoveSale(int partID, decimal price) {
            StringBuilder sb = new StringBuilder(api_domain);
            sb.AppendFormat("RemoveSale?key={0}", api_key);

            Dictionary<string, string> post_data = new Dictionary<string, string>();
            post_data.Add("customerID", customerID);
            post_data.Add("price", price.ToString());
            post_data.Add("partID", partID.ToString());

            string resp = UDF.POSTRequest(sb.ToString(), post_data);
            if (resp.Length > 0) {
                throw new Exception(resp);
            }
        }
    }

    public class SimplePricing {
        public int cust_id { get; set; }
        public int partID { get; set; }
        public decimal price { get; set; }
        public int isSale { get; set; }
        public string sale_start { get; set; }
        public string sale_end { get; set; }
    }
}