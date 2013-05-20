using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using Newtonsoft.Json;
using System.Xml.Linq;
using GCheckout.OrderProcessing;
using GCheckout.Util;

namespace EcommercePlatform.Models {
    public class GoogleCheckout {
        public static void getNotification(string serial) {
            // get Google Order number from serial
            string ordernum = serial.Split('-')[0];

            // create web client
            WebClient wc = new WebClient();
            wc.Proxy = null;
            
            // get merchant info from settings
            Settings settings = new Settings();
            string MerchantID = settings.Get("GoogleMerchantId");
            string MerchantKey = settings.Get("GoogleMerchantKey");
            GCheckout.EnvironmentType env = GCheckout.EnvironmentType.Production;
            if (HttpContext.Current.Request.Url.Host.Contains("127.0.0") || HttpContext.Current.Request.Url.Host.Contains("localhost") || settings.Get("GoogleCheckoutEnv") == "override") {
                MerchantID = settings.Get("GoogleDevMerchantId");
                MerchantKey = settings.Get("GoogleDevMerchantKey");
                env = GCheckout.EnvironmentType.Sandbox;
            }

            NotificationHistoryRequest request = new NotificationHistoryRequest(new List<string> {ordernum});
            request.MerchantID = MerchantID;
            request.MerchantKey = MerchantKey;
            request.Environment = env;
            
            request.RetrieveAllNotifications = true;

            NotificationHistoryResponse response = (NotificationHistoryResponse)request.Send();

            // Iterate through the notification history for this order looking for the notification that exactly matches the given serial number
            foreach (object notification in response.NotificationResponses) {
                if (notification.GetType().Equals(typeof(GCheckout.AutoGen.NewOrderNotification))) {
                    GCheckout.AutoGen.NewOrderNotification newOrderNotification = (GCheckout.AutoGen.NewOrderNotification)notification;
                    if (newOrderNotification.serialnumber.Equals(serial)) {
                        HandleNewOrderNotification(newOrderNotification);
                    }
                } else if (notification.GetType().Equals(typeof(GCheckout.AutoGen.OrderStateChangeNotification))) {
                    GCheckout.AutoGen.OrderStateChangeNotification statechange = (GCheckout.AutoGen.OrderStateChangeNotification)notification;
                    if (statechange.serialnumber.Equals(serial)) {
                        HandleOrderStateChangeNotification(statechange);
                    }
                } else if (notification.GetType().Equals(typeof(GCheckout.AutoGen.RiskInformationNotification))) {
                    GCheckout.AutoGen.RiskInformationNotification riskInformationNotification = (GCheckout.AutoGen.RiskInformationNotification)notification;
                    if (riskInformationNotification.serialnumber.Equals(serial)) {
                        //HandleRiskInformationNotification(riskInformationNotification);
                    }
                } else if (notification.GetType().Equals(typeof(GCheckout.AutoGen.AuthorizationAmountNotification))) {
                    GCheckout.AutoGen.AuthorizationAmountNotification authorizationAmountNotification = (GCheckout.AutoGen.AuthorizationAmountNotification)notification;
                    if (authorizationAmountNotification.serialnumber.Equals(serial)) {
                        HandleAuthorizationAmountNotification(authorizationAmountNotification);
                    }
                } else if (notification.GetType().Equals(typeof(GCheckout.AutoGen.ChargeAmountNotification))) {
                    GCheckout.AutoGen.ChargeAmountNotification chargeAmountNotification = (GCheckout.AutoGen.ChargeAmountNotification)notification;
                    if (chargeAmountNotification.serialnumber.Equals(serial)) {
                        HandleChargeAmountNotification(chargeAmountNotification);
                    }
                } else {
                    //throw new ArgumentOutOfRangeException("Unhandled Type [" + notification.GetType().ToString() + "]!; serialNumber=[" + serial + "];");
                }
            }

        }

        public static void HandleNewOrderNotification(GCheckout.AutoGen.NewOrderNotification neworder) {
            string googleOrderID = neworder.googleordernumber;
            int orderID = 0;
            System.Xml.XmlNode[] privateData = neworder.shoppingcart.merchantprivatedata.Any;
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            try {
                orderID = Convert.ToInt32(privateData.Where(x => x.Name.ToLower().Equals("ordernumber")).Select(x => x.InnerText).First());
            } catch { };
            Cart order = new Cart().Get(orderID);
            order.UpdatePaymentConfirmationCode(googleOrderID);
            order.UpdatePayment(neworder.financialorderstate.ToString());
        }

        public static void HandleAuthorizationAmountNotification(GCheckout.AutoGen.AuthorizationAmountNotification authnotification) {
            string googleOrderID = authnotification.googleordernumber;
            Cart order = new Cart().GetByPayment(googleOrderID);
            if (order.getPayment().status != "Complete") {
                order.UpdatePayment(authnotification.ordersummary.financialorderstate.ToString());
            }
        }

        public static void HandleChargeAmountNotification(GCheckout.AutoGen.ChargeAmountNotification chargenotification) {
            string googleOrderID = chargenotification.googleordernumber;
            Cart order = new Cart().GetByPayment(googleOrderID);
            if (order.getTotal() == chargenotification.totalchargeamount.Value) {
                order.UpdatePayment("Complete");
                order.SendConfirmation();
                order.SendInternalOrderEmail();
            }
        }

        public static void HandleOrderStateChangeNotification(GCheckout.AutoGen.OrderStateChangeNotification statechange) {
            string googleOrderID = statechange.googleordernumber;
            Cart order = new Cart().GetByPayment(googleOrderID);
            if (order.getPayment().status != "Complete") {
                order.UpdatePayment(statechange.newfinancialorderstate.ToString());
            }
        }
    }
}