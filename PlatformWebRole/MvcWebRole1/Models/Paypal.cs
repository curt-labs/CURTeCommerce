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


        public string ECDoExpressCheckout(string token, string payerID, string amount, Cart cart) {
            Settings settings = new Settings();
            NVPCallerServices caller = new NVPCallerServices();
            IAPIProfile profile = getProfile();
            caller.APIProfile = profile;

            NVPCodec encoder = new NVPCodec();
            encoder.Add("VERSION","84.0");
            encoder.Add("METHOD","DoExpressCheckoutPayment");

            // Add request-specific fields to the request.
            encoder.Add("TOKEN",token);
            encoder.Add("PAYERID",payerID);
            encoder.Add("RETURNURL",getSiteURL() + "Payment/PayPalCheckout");
            encoder.Add("CANCELURL",getSiteURL() + "Payment");
            encoder.Add("PAYMENTREQUEST_0_AMT",amount);
            encoder.Add("PAYMENTREQUEST_0_PAYMENTACTION","Sale");
            encoder.Add("PAYMENTREQUEST_0_CURRENCYCODE","USD");
            encoder.Add("BRANDNAME",settings.Get("SiteName"));
            encoder.Add("LOGIN","Login");
            encoder.Add("HDRIMG",settings.Get("EmailLogo"));
            encoder.Add("CUSTOMERSERVICENUMBER",settings.Get("PhoneNumber"));
            encoder.Add("PAYMENTREQUEST_0_SHIPPINGAMT",cart.shipping_price.ToString());
            encoder.Add("PAYMENTREQUEST_0_DESC","Your " + settings.Get("SiteName") + " Order");
            encoder.Add("ALLOWNOTE","0");
            encoder.Add("NOSHIPPING","1");
            int count = 0;
            decimal total = 0;
            foreach (CartItem item in cart.CartItems) {
                encoder.Add("L_PAYMENTREQUEST_0_NUMBER" + count, item.partID.ToString());
                encoder.Add("L_PAYMENTREQUEST_0_NAME" + count, item.shortDesc);
                encoder.Add("L_PAYMENTREQUEST_0_AMT" + count, String.Format("{0:N2}", item.price));
                encoder.Add("L_PAYMENTREQUEST_0_QTY" + count, item.quantity.ToString());
                encoder.Add("L_PAYMENTREQUEST_0_ITEMCATEGORY" + count, "Physical");
                encoder.Add("L_PAYMENTREQUEST_0_ITEMURL" + count, settings.Get("SiteURL") + "part/" + item.partID);
                total += item.price * item.quantity;
                count++;
            }
            encoder.Add("PAYMENTREQUEST_0_TAXAMT", String.Format("{0:N2}", cart.tax));
            encoder.Add("PAYMENTREQUEST_0_ITEMAMT", String.Format("{0:N2}", total));
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

        public string ECSetExpressCheckout(Cart cart) {
            Settings settings = new Settings();
            NVPCallerServices caller = new NVPCallerServices();
            IAPIProfile profile = getProfile();
            caller.APIProfile = profile;

            NVPCodec encoder = new NVPCodec();
            encoder.Add("VERSION", "84.0");
            encoder.Add("METHOD", "SetExpressCheckout");
            // Add request-specific fields to the request.
            encoder.Add("RETURNURL", getSiteURL() + "Payment/CompletePayPalCheckout");
            encoder.Add("CANCELURL", getSiteURL() + "Payment");
            encoder.Add("PAYMENTREQUEST_0_AMT", String.Format("{0:N2}", cart.getTotal()));
            encoder.Add("PAYMENTREQUEST_0_PAYMENTACTION", "Sale");
            encoder.Add("PAYMENTREQUEST_0_CURRENCYCODE", "USD");
            encoder.Add("BRANDNAME", settings.Get("SiteName") + " eCommerce Platform");
            encoder.Add("LOGIN", "Login");
            encoder.Add("HDRIMG", settings.Get("EmailLogo"));
            encoder.Add("CUSTOMERSERVICENUMBER", "888-894-4824");
            encoder.Add("PAYMENTREQUEST_0_SHIPPINGAMT", String.Format("{0:N2}", cart.shipping_price));
            encoder.Add("PAYMENTREQUEST_0_DESC", "Your " + settings.Get("SiteName") + " Order");
            encoder.Add("ALLOWNOTE", "0");
            encoder.Add("NOSHIPPING", "1");
            int count = 0;
            decimal total = 0;
            foreach (CartItem item in cart.CartItems) {
                encoder.Add("L_PAYMENTREQUEST_0_NUMBER" + count, item.partID.ToString());
                encoder.Add("L_PAYMENTREQUEST_0_NAME" + count, item.shortDesc);
                encoder.Add("L_PAYMENTREQUEST_0_AMT" + count, String.Format("{0:N2}", item.price));
                encoder.Add("L_PAYMENTREQUEST_0_QTY" + count, item.quantity.ToString());
                encoder.Add("L_PAYMENTREQUEST_0_ITEMCATEGORY" + count, "Physical");
                encoder.Add("L_PAYMENTREQUEST_0_ITEMURL" + count, settings.Get("SiteURL") + "part/" + item.partID);
                total += item.price * item.quantity;
                count++;
            }
            encoder.Add("PAYMENTREQUEST_0_TAXAMT", String.Format("{0:N2}", cart.tax));
            encoder.Add("PAYMENTREQUEST_0_ITEMAMT", String.Format("{0:N2}", total));

            // Execute the API operation and obtain the response.
            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = caller.Call(pStrrequestforNvp);

            NVPCodec decoder = new NVPCodec();
            decoder.Decode(pStresponsenvp);
            if (decoder["TOKEN"] != null) {
                return decoder["TOKEN"];
            } else {

                string errors = " CorrelationID: " + decoder["CORRELATIONID"] + " error code: " + decoder["L_ERRORCODE0"] + " Messages: " + decoder["L_SHORTMESSAGE0"] + ", " + decoder["L_SHORTMESSAGE2"] + ", " + decoder["L_LONGMESSAGE0"] + ", " + decoder["L_LONGMESSAGE1"];
                return decoder["ACK"] + errors;
            }
        }

        private string getSiteURL() {
            Settings settings = new Settings();
            if (HttpContext.Current.Request.Url.Host.Contains("127.0.0")) {
                return "https://127.0.0.1";
            } else if(HttpContext.Current.Request.Url.Host.Contains("localhost")) {
                return "https://localhost:59295/";
            } else {
                return settings.Get("SiteURL").Replace("http:","https:");
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