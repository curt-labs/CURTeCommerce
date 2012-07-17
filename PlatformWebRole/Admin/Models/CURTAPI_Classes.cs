using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Security.Cryptography;

namespace Admin.Models {

    public class FillerFunctions {
        public static Boolean FileExists(string path = "") {
            if (path == null || path.Trim().Length == 0) {
                return false;
            }
            WebRequest req = WebRequest.Create(path);
            req.Method = "HEAD";

            try {
                WebResponse resp = req.GetResponse();
                return true;
            } catch (Exception e) {
                return false;
            }
        }

        public static List<T> Shuffle<T>(IList<T> list) {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1) {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list.ToList();
        }
    }
    
    public class APIPart {
        private List<APIAttribute> _content = new List<APIAttribute>();
        private List<APIAttribute> _attributes = new List<APIAttribute>();
        private List<APIAttribute> _pricing = new List<APIAttribute>();
        private List<APIReview> _reviews = new List<APIReview>();
        List<APIImage> _images = new List<APIImage>();
        private int _partID = 0;
        private int _custPartID = 0;
        private int _status = 0;
        private string _dateModified = "";
        private string _dateAdded = "";
        private string _shortDesc = "";
        private string _oldPartNumber = "";
        private string _listPrice = "";
        private string _pClass = "";
        private int _relatedCount = 0;
        private int _installTime = 0;
        private double? _averageReview = 0;
        private string _drilling = "";
        private string _exposed = "";
        private int? _vehicleID = 0;
        private string _colorCode = "";


        public int partID {
            get {
                return this._partID;
            }
            set {
                if (value != null && value != this._partID) {
                    this._partID = value;
                }
            }
        }
        public int custPartID {
            get {
                return this._custPartID;
            }
            set {
                if (value != null && value != this._custPartID) {
                    this._custPartID = value;
                }
            }
        }
        public int status {
            get {
                return this._status;
            }
            set {
                if (value != null && value != this._status) {
                    this._status = value;
                }
            }
        }
        public string dateModified {
            get {
                return this._dateModified;
            }
            set {
                if (value != null && value != this._dateModified) {
                    this._dateModified = value;
                }
            }
        }
        public string dateAdded {
            get {
                return this._dateAdded;
            }
            set {
                if (value != null && value != this._dateAdded) {
                    this._dateAdded = value;
                }
            }
        }
        public string shortDesc {
            get {
                return this._shortDesc;
            }
            set {
                if (value != null && value != this._shortDesc) {
                    this._shortDesc = value;
                }
            }
        }
        public string oldPartNumber {
            get {
                return this._oldPartNumber;
            }
            set {
                if (value != null && value != this._oldPartNumber) {
                    this._oldPartNumber = value;
                }
            }
        }
        public string listPrice {
            get {
                return this._listPrice;
            }
            set {
                if (value != null && value != this._listPrice) {
                    this._listPrice = value;
                }
            }
        }
        public List<APIAttribute> attributes {
            get {
                return this._attributes;
            }
            set {
                if (value != null && value != this._attributes) {
                    this._attributes = value;
                }
            }
        }
        public List<APIAttribute> content {
            get {
                return this._content;
            }
            set {
                if (value != null && value != this._content) {
                    this._content = value;
                }
            }
        }
        public List<APIAttribute> pricing {
            get {
                return this._pricing;
            }
            set {
                if (value != null && value != this._pricing) {
                    this._pricing = value;
                }
            }
        }
        public List<APIReview> reviews {
            get {
                return this._reviews;
            }
            set {
                if (value != null && value != this._reviews) {
                    this._reviews = value;
                }
            }
        }
        public List<APIImage> images {
            get {
                return this._images;
            }
            set {
                if (value != null && value != this._images) {
                    this._images = value;
                }
            }
        }
        public string pClass {
            get {
                return this._pClass;
            }
            set {
                if (value != null && value != this._pClass) {
                    this._pClass = value;
                }
            }
        }

        public int relatedCount {
            get {
                return this._relatedCount;
            }
            set {
                if (value != null && value != this._relatedCount) {
                    this._relatedCount = value;
                }
            }
        }

        public int installTime {
            get {
                return this._installTime;
            }
            set {
                if (value != null && value != this._installTime) {
                    this._installTime = value;
                }
            }
        }

        public double? averageReview {
            get {
                return this._averageReview;
            }
            set {
                if (value != null && value != this._averageReview) {
                    this._averageReview = value;
                }
            }
        }

        public string drilling {
            get {
                return this._drilling;
            }
            set {
                if (value != null && value != this._drilling) {
                    this._drilling = value;
                }
            }
        }

        public string exposed {
            get {
                return this._exposed;
            }
            set {
                if (value != null && value != this._exposed) {
                    this._exposed = value;
                }
            }
        }
        public string colorCode {
            get {
                return this._colorCode;
            }
            set {
                if (value != null && value != this._colorCode) {
                    this._colorCode = value;
                }
            }
        }
        public int? vehicleID {
            get {
                return this._vehicleID;
            }
            set {
                if (value != null && value != this._vehicleID) {
                    this._vehicleID = value;
                }
            }
        }
    }

    public class APIReview {
        public int reviewID { get; set; }
        public int partID { get; set; }
        public int rating { get; set; }
        public string subject { get; set; }
        public string review_text { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string createdDate { get; set; }
    }

    public class APIAttribute {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class APICategory {
        private List<APICategory> _subs = new List<APICategory>();

        public int catID { get; set; }
        public int parentID { get; set; }
        public DateTime dateAdded { get; set; }
        public string catTitle { get; set; }
        public string shortDesc { get; set; }
        public string longDesc { get; set; }
        public string image { get; set; }
        public int isLifestyle { get; set; }
        public List<APICategory> sub_categories {
            get {
                return this._subs;
            }
            set {
                this._subs = value;
            }
        }
    }

    public class APIImage {
        public int imageID { get; set; }
        public string size { get; set; }
        public string path { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public int partID { get; set; }
        public char sort { get; set; }
    }

    public class JSONAPICategory {
        public APICategory parent { get; set; }
        public List<APICategory> sub_categories { get; set; }
    }

    public class APIColorCode {
        public int codeID { get; set; }
        public string code { get; set; }
        public string font { get; set; }
    }

    /// <summary>
    /// This object will hold strings containing the actual values of the year, make, model, and style for a vehicle.
    /// </summary>
    public class FullVehicle {
        public int vehicleID { get; set; }
        public int yearID { get; set; }
        public int makeID { get; set; }
        public int modelID { get; set; }
        public int styleID { get; set; }
        public int aaiaID { get; set; }
        public double year { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string style { get; set; }
        public int installTime { get; set; }
        public string drilling { get; set; }
        public string exposed { get; set; }
    }

    public class FedExAuthentication {
        public string Key { get; set; }
        public string Password { get; set; }
        public int AccountNumber { get; set; }
        public int MeterNumber { get; set; }
        public string CustomerTransactionId { get; set; }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://fedex.com/ws/rate/v10")]
    public class ShippingAddress {

        private string[] streetLinesField;

        private string cityField;

        private string stateOrProvinceCodeField;

        private string postalCodeField;

        private string urbanizationCodeField;

        private string countryCodeField;

        private bool residentialField;

        private bool residentialFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("StreetLines")]
        public string[] StreetLines {
            get {
                return this.streetLinesField;
            }
            set {
                this.streetLinesField = value;
            }
        }

        /// <remarks/>
        public string City {
            get {
                return this.cityField;
            }
            set {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string StateOrProvinceCode {
            get {
                return this.stateOrProvinceCodeField;
            }
            set {
                this.stateOrProvinceCodeField = value;
            }
        }

        /// <remarks/>
        public string PostalCode {
            get {
                return this.postalCodeField;
            }
            set {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        public string UrbanizationCode {
            get {
                return this.urbanizationCodeField;
            }
            set {
                this.urbanizationCodeField = value;
            }
        }

        /// <remarks/>
        public string CountryCode {
            get {
                return this.countryCodeField;
            }
            set {
                this.countryCodeField = value;
            }
        }

        /// <remarks/>
        public bool Residential {
            get {
                return this.residentialField;
            }
            set {
                this.residentialField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ResidentialSpecified {
            get {
                return this.residentialFieldSpecified;
            }
            set {
                this.residentialFieldSpecified = value;
            }
        }
    }

    public class ShippingResponse {
        public string Status { get; set; }
        public string Status_Description { get; set; }
        public List<ShipmentRateDetails> Result { get; set; }
    }

    public class ShipmentRateDetails {
        public string ServiceType { get; set; }
        public string PackagingType { get; set; }
        public List<RateDetail> Rates { get; set; }
        public string TransitTime { get; set; }
        public List<ShippingNotification> Notifications { get; set; }
    }

    public class RateDetail {
        public string RateType { get; set; }
        public KeyValuePair<decimal, string> TotalBillingWeight { get; set; }
        public KeyValuePair<decimal, string> TotalBaseCharge { get; set; }
        public KeyValuePair<decimal, string> TotalFreightDiscounts { get; set; }
        public KeyValuePair<decimal, string> TotalSurcharges { get; set; }
        public List<ShippingSurcharge> Surcharges { get; set; }
        public KeyValuePair<decimal, string> NetCharge { get; set; }
    }

    public class ShippingSurcharge {
        public string Type { get; set; }
        public KeyValuePair<decimal, string> Charge { get; set; }
    }

    public class ShippingNotification {
        public int NotificationID { get; set; }
        public string Severity { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
    }
}