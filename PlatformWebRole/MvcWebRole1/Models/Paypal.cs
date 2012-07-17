using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.paypal.sdk.services;
using com.paypal.sdk.profiles;
using com.paypal.sdk.util;
using System.Configuration;

namespace EcommercePlatform.Models {
    public class Paypal {


        public string ECDoExpressCheckout(string token, string payerID, string amount, List<CartItem> items, decimal shippingamount) {
            Settings settings = new Settings();
            NVPCallerServices caller = new NVPCallerServices();
            IAPIProfile profile = getProfile();
            caller.APIProfile = profile;

            NVPCodec encoder = new NVPCodec();
            encoder["VERSION"] = "84.0";
            encoder["METHOD"] = "DoExpressCheckoutPayment";

            // Add request-specific fields to the request.
            encoder["TOKEN"] = token;
            encoder["PAYERID"] = payerID;
            encoder["RETURNURL"] = getSiteURL() + "Payment/PayPalCheckout";
            encoder["CANCELURL"] = getSiteURL() + "Payment";
            encoder["PAYMENTREQUEST_0_AMT"] = amount;
            encoder["PAYMENTREQUEST_0_PAYMENTACTION"] = "Sale";
            encoder["PAYMENTREQUEST_0_CURRENCYCODE"] = "USD";
            encoder["BRANDNAME"] = settings.Get("SiteName");
            encoder["LOGIN"] = "Login";
            encoder["HDRIMG"] = settings.Get("EmailLogo");
            encoder["CUSTOMERSERVICENUMBER"] = settings.Get("PhoneNumber");
            encoder["PAYMENTREQUEST_0_SHIPPINGAMT"] = shippingamount.ToString();
            encoder["PAYMENTREQUEST_0_DESC"] = "Your " + settings.Get("SiteName") + " Order";
            encoder["ALLOWNOTE"] = "0";
            encoder["NOSHIPPING"] = "1";
            int count = 0;
            decimal total = 0;
            foreach (CartItem item in items) {
                encoder["L_PAYMENTREQUEST_0_NUMBER" + count] = item.partID.ToString();
                encoder["L_PAYMENTREQUEST_0_NAME" + count] = item.shortDesc;
                encoder["L_PAYMENTREQUEST_0_AMT" + count] = String.Format("{0:C}", item.price);
                encoder["L_PAYMENTREQUEST_0_QTY" + count] = item.quantity.ToString();
                encoder["L_PAYMENTREQUEST_0_ITEMCATEGORY" + count] = "Physical";
                encoder["L_PAYMENTREQUEST_0_ITEMURL" + count] = settings.Get("SiteURL") + "part/" + item.partID;
                total += item.price * item.quantity;
                count++;
            }
            encoder["PAYMENTREQUEST_0_ITEMAMT"] = total.ToString();
            // Execute the API operation and obtain the response.
            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = caller.Call(pStrrequestforNvp);

            NVPCodec decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);
            return decoder["ACK"];
        }

        public PayPalResponse ECGetExpressCheckout(string token) {
            NVPCallerServices caller = new NVPCallerServices();
            IAPIProfile profile = getProfile();
            caller.APIProfile = profile;

            NVPCodec encoder = new NVPCodec();
            encoder["VERSION"] = "84.0";
            encoder["METHOD"] = "GetExpressCheckoutDetails";

            // Add request-specific fields to the request.
            encoder["TOKEN"] = token; // Pass the token returned in SetExpressCheckout.

            // Execute the API operation and obtain the response.
            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = caller.Call(pStrrequestforNvp);

            NVPCodec decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);
            PayPalResponse response = new PayPalResponse {
                token = (decoder["TOKEN"] != null) ? decoder["TOKEN"] : "",
                acknowledgement = decoder["ACK"],
                first = decoder["FIRSTNAME"],
                last = decoder["LASTNAME"],
                email = decoder["EMAIL"],
                amount = decoder["PAYMENTREQUEST_0_AMT"],
                payerID = decoder["PAYERID"]
            };
            return response;
        }

        public string ECSetExpressCheckout(string amount,List<CartItem> items,decimal shippingamount) {
            Settings settings = new Settings();
            NVPCallerServices caller = new NVPCallerServices();
            IAPIProfile profile = getProfile();
            caller.APIProfile = profile;

            NVPCodec encoder = new NVPCodec();
            encoder["VERSION"] = "84.0";
            encoder["METHOD"] = "SetExpressCheckout";

            // Add request-specific fields to the request.
            encoder["RETURNURL"] = getSiteURL() + "Payment/PayPalCheckout";
            encoder["CANCELURL"] = getSiteURL() + "Payment";
            encoder["PAYMENTREQUEST_0_AMT"] = amount;
            encoder["PAYMENTREQUEST_0_PAYMENTACTION"] = "Sale";
            encoder["PAYMENTREQUEST_0_CURRENCYCODE"] = "USD";
            encoder["BRANDNAME"] = settings.Get("SiteName") + " eCommerce Platform";
            encoder["LOGIN"] = "Login";
            encoder["HDRIMG"] = settings.Get("EmailLogo");
            encoder["CUSTOMERSERVICENUMBER"] = "888-894-4824";
            encoder["PAYMENTREQUEST_0_SHIPPINGAMT"] = shippingamount.ToString();
            encoder["PAYMENTREQUEST_0_DESC"] = "Your " + settings.Get("SiteName") + " Order";
            encoder["ALLOWNOTE"] = "0";
            encoder["NOSHIPPING"] = "1";
            int count = 0;
            decimal total = 0;
            foreach (CartItem item in items) {
                encoder["L_PAYMENTREQUEST_0_NUMBER" + count] = item.partID.ToString();
                encoder["L_PAYMENTREQUEST_0_NAME" + count] = item.shortDesc;
                encoder["L_PAYMENTREQUEST_0_AMT" + count] = String.Format("{0:C}", item.price);
                encoder["L_PAYMENTREQUEST_0_QTY" + count] = item.quantity.ToString();
                encoder["L_PAYMENTREQUEST_0_ITEMCATEGORY" + count] = "Physical";
                encoder["L_PAYMENTREQUEST_0_ITEMURL" + count] = settings.Get("SiteURL") + "part/" + item.partID;
                total += item.price * item.quantity; 
                count++;
            }
            encoder["PAYMENTREQUEST_0_ITEMAMT"] = total.ToString();

            // Execute the API operation and obtain the response.
            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = caller.Call(pStrrequestforNvp);

            NVPCodec decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);
            if (decoder["TOKEN"] != null) {
                return decoder["TOKEN"];
            } else {
                return decoder["ACK"];
            }
        }

        private string getSiteURL() {
            Settings settings = new Settings();
            if (HttpContext.Current.Request.Url.Host.Contains("127.0.0")) {
                return "http://127.0.0.1/";
            } else if(HttpContext.Current.Request.Url.Host.Contains("localhost")) {
                return "http://localhost:59295/";
            } else {
                return settings.Get("SiteURL");
            }
        }

        private IAPIProfile getProfile() {
            IAPIProfile profile = ProfileFactory.createSignatureAPIProfile();
            Settings settings = new Settings();
            if (HttpContext.Current.Request.Url.Host.Contains("127.0.0") || HttpContext.Current.Request.Url.Host.Contains("localhost")) {
                profile.APIUsername = settings.Get("PayPalAPIDevUserName");
                profile.APIPassword = settings.Get("PayPalAPIDevPassword");
                profile.APISignature = settings.Get("PayPalAPIDevSignature");
                profile.Environment = "sandbox";
            } else {
                profile.APIUsername = settings.Get("PayPalAPIUserName");
                profile.APIPassword = settings.Get("PayPalAPIPassword");
                profile.APISignature = settings.Get("PayPalAPISignature");
                profile.Environment = "live";
            }

            return profile;
        }

    }

    public class PayPalResponse {
        public string token { get; set; }
        public string email { get; set; }
        public string first { get; set; }
        public string last { get; set; }
        public string acknowledgement { get; set; }
        public string amount { get; set; }
        public string payerID { get; set; }
    }
}