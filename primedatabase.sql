insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Main','/Admin',0,'Home Page',1,'/Admin/Content/img/module_icons/Home.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Orders','/Admin/Orders',0,'Manage Orders',1,'/Admin/Content/img/module_icons/ShoppingCart.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Pricing','/Admin/Pricing',0,'Manage Pricing',1,'/Admin/Content/img/module_icons/Wallet.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Settings','/Admin/Settings',0,'Site Settings',1,'/Admin/Content/img/module_icons/ControlPanel.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Locations','/Admin/Locations',0,'Manage Locations',1,'/Admin/Content/img/module_icons/Web.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Profiles','/Admin/Profiles',0,'Manage Profiles',1,'/Admin/Content/img/module_icons/Users.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Distribution Centers','/Admin/Distribution',0,'Manage Distribution Centers',1,'/Admin/Content/img/module_icons/Wall.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Services','/Admin/Services',0,'Manage Services',1,'/Admin/Content/img/module_icons/Box.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('File Manager','/Admin/FileManager',0,'File Management',1,'/Admin/Content/img/module_icons/Database.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Newsletter','/Admin/Newsletter',0,'Newsletter Subscriptions',1,'/Admin/Content/img/module_icons/PostageStamp.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Content Manager','/Admin/ContentManager',0,'Content Manager',1,'/Admin/Content/img/module_icons/BookOpen.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Contact Manager','/Admin/ContactManager',0,'Manage Contact Inquiries',1,'/Admin/Content/img/module_icons/AddressBook.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('FAQ Manager','/Admin/FAQManager',0,'Manage Frequently Asked Questions',1,'/Admin/Content/img/module_icons/Question.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Testimonials','/Admin/Testimonials',0,'Manage Testimonials',1,'/Admin/Content/img/module_icons/Quill.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Blog','/Admin/Blog',0,'Blog Management',1,'/Admin/Content/img/module_icons/FormatQuote.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Customers','/Admin/Customers',0,'View Customers',1,'/Admin/Content/img/module_icons/MemberCard.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Banners','/Admin/Banner',0,'Banner Management',1,'/Admin/Content/img/module_icons/Desktop.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Reporting','/Admin/Reporting',0,'Reporting Tools',1,'/Admin/Content/img/module_icons/FormatNumber.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('FTP','/Admin/FTP',0,'FTP Access',0,NULL);
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Task Scheduler','/Admin/Scheduler',0,'Task Scheduler',1,'/Admin/Content/img/module_icons/SiteMap.png');
insert into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Invoices','/Admin/Invoice',0,'View Invoices',1,'/Admin/Content/img/module_icons/Inbox.png');
GO

insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Order Details','/Admin/Orders/Items',2,'Order Details',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Add','/Admin/Orders/Add',2,'Add Order',1);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Add - New Cust','/Admin/Orders/Step1New',2,'Add Order New Customer',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Add - Cart','/Admin/Orders/Step2',2,'Add Order Step 2',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Add - Shipping','/Admin/Orders/Step3',2,'Add Order Step 3',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Add - Billing','/Admin/Orders/Step4',2,'Add Order Step 4',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Add - Payment','/Admin/Orders/Step5',2,'Add Order Step 5',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Add - Confirmation','/Admin/Orders/Step6',2,'Add Order Step 6',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Void Order','/Admin/Orders/Void',2,'Void Order',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Edit','/Admin/Pricing/Edit',3,'Edit Pricing',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Edit','/Admin/Locations/Edit',5,'Edit Locations',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Delete','/Admin/Locations/Delete',5,'Delete Locations',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Edit','/Admin/Profiles/Edit',6,'Edit Profile',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Add','/Admin/Profiles/Add',6,'Add Profile',1);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Delete','/Admin/Profiles/Delete',6,'Delete Profile',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Edit','/Admin/Distribution/Edit',7,'Edit Distribution Centers',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Delete','/Admin/Distribution/Delete',7,'Delete Distribution Centers',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Edit','/Admin/Services/Edit',8,'Edit Service',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Delete','/Admin/Services/Delete',8,'Delete Service',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Upload','/Admin/FileManager/Upload',9,'Upload',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('View Files','/Admin/FileManager/Folder',9,'View Files',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Delete','/Admin/Newsletter/Delete',10,'Delete Subscription',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Add','/Admin/ContentManager/Edit',11,'Add Content',1);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Delete','/Admin/ContentManager/Delete',11,'Delete Content',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Delete Type','/Admin/ContactManager/DeleteType',12,'Delete Contact Type',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Edit Topics','/Admin/FAQManager/EditTopic',13,'Edit Topics',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Edit Questions','/Admin/FAQManager/EditQuestion',13,'Edit Frequently Asked Question',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Delete Questions','/Admin/FAQManager/DeleteQuestion',13,'Delete Frequently Asked Questions',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Delete Topics','/Admin/FAQManager/DeleteTopic',13,'Delete Topics',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Approved','/Admin/Testimonials/Approved',14,'Approved Testimonials',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Edit Category','/Admin/Blog/EditCategory',15,'Blog Category Management',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Add','/Admin/Blog/EditPost',15,'Add Blog Post',1);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('View Comment','/Admin/Blog/ViewComment',15,'View Blog Comment',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Post Comments','/Admin/Blog/PostComments',15,'View Blog Post Comments',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Info','/Admin/Customers/Info',16,'Customer Info',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Edit','/Admin/Banner/Edit',17,'Edit Banners',0);
insert into Modules (name,path,parentID,alt_text,inMenu) VALUES ('Details','/Admin/Invoice/Details',21,'View Invoice Details',0);
GO

insert into Profile (username,password,email,first,last,date_added) VALUES ('admin','','admin@changeme.com','administrator','administrator',Getdate());
GO

insert into ProfileModules (profileID,moduleID) 
select 1, id from Modules;
GO

INSERT INTO SettingGroup (name) VALUES ('General');
INSERT INTO SettingGroup (name) VALUES ('Payment Gateways');
INSERT INTO SettingGroup (name) VALUES ('Social Media');
INSERT INTO SettingGroup (name) VALUES ('Graphics');
INSERT INTO SettingGroup (name) VALUES ('Email');
INSERT INTO SettingGroup (name) VALUES ('FTP');
INSERT INTO SettingGroup (name) VALUES ('CURT API');
INSERT INTO SettingGroup (name) VALUES ('FedEx');
GO

INSERT INTO Setting (groupID,name,isImage) VALUES (1,'BlogDescription',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'CURTAccount',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'CURTAPIKey',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'EDIPhone',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'FreeShippingAmount',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'FreeShippingType',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'GoogleAnalyticsCode',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'GoogleAPIKey',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'logging',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'PhoneNumber',0);
INSERT INTO Setting (groupID,name,value,isImage) VALUES (1,'PlacesAPIDomain','https://maps.googleapis.com/maps/api/place/',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'ReCaptchaPrivateKey',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'ReCaptchaPublicKey',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'SiteName',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'SiteURL',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'SiteTitle',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (1,'SiteDescription',0);
INSERT INTO Setting (groupID,name,value,isImage) VALUES (1,'CustomerLoginAfterRegistration','false',0);
INSERT INTO Setting (groupID,name,value,isImage) VALUES (1,'RequireCustomerActivation','true',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (2,'AuthorizeNetGatewayID',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (2,'AuthorizeNetLoginKey',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (2,'AuthorizeNetTransactionKey',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (2,'GoogleCheckoutEnv',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (2,'GoogleDevMerchantId',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (2,'GoogleDevMerchantKey',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (2,'GoogleMerchantId',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (2,'GoogleMerchantKey',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (2,'PayPalAPIDevPassword',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (2,'PayPalAPIDevSignature',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (2,'PayPalAPIDevUserName',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (2,'PayPalAPIPassword',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (2,'PayPalAPISignature',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (2,'PayPalAPIUserName',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (3,'FacebookPageURL',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (3,'TwitterPageURL',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (3,'YouTubePageURL',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (4,'EmailLogo',1);
INSERT INTO Setting (groupID,name,isImage) VALUES (4,'HeaderLogo',1);
INSERT INTO Setting (groupID,name,isImage) VALUES (4,'HeaderLogoScroll',1);
INSERT INTO Setting (groupID,name,isImage) VALUES (4,'HomeBannerLeft',1);
INSERT INTO Setting (groupID,name,isImage) VALUES (4,'HomeBannerLeftAltText',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (4,'HomeBannerTop',1);
INSERT INTO Setting (groupID,name,isImage) VALUES (4,'HomeBannerTopAltText',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (5,'NoReplyEmailAddress',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (5,'SMTPPassword',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (5,'SMTPPort',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (5,'SMTPServer',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (5,'SMTPSSL',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (5,'SMTPUserName',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (5,'SupportEmail',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (6,'FTPInvoicePath',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (6,'FTPOrderPath',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (6,'FTPPass',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (6,'FTPServer',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (6,'FTPUser',0);
INSERT INTO Setting (groupID,name,value,isImage) VALUES (7,'CURTAPIDOMAIN','http://api.curtmfg.com/v2/',0);
INSERT INTO Setting (groupID,name,value,isImage) VALUES (7,'CURTAPISHIPPINGDOMAIN','http://api.curtmfg.com/Ordering/',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (8,'FedExAccount',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (8,'FedExEnvironment',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (8,'FedExKey',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (8,'FedExMeter',0);
INSERT INTO Setting (groupID,name,isImage) VALUES (8,'FedExPassword',0);
GO

INSERT INTO Country (name,abbr) VALUES ('United States','US');
INSERT INTO Country (name,abbr) VALUES ('Canada','CA');
GO

INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Alabama','AL',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Alaska','AK',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('American Samoa','AS',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Arizona','AR',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Arkansas','AR',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('California','CA',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Colorado','CO',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Connecticut','CT',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Delaware','DE',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('District Of Columbia','DC',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Federated States Of Micronesia','FM',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Florida','FL',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Georgia','GA',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Guam','GU',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Hawaii','HI',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Idaho','ID',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Illinois','IL',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Indiana','IN',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Iowa','IA',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Kansas','KS',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Kentucky','KY',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Louisiana','LA',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Maine','ME',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Marshall Islands','MH',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Maryland','MD',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Massachusetts','MA',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Michigan','MI',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Minnesota','MN',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Mississippi','MS',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Missouri','MO',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Montana','MT',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Nebraska','NE',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Nevada','NV',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('New Hampshire','NH',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('New Jersey','NJ',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('New Mexico','NM',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('New York','NY',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('North Carolina','NC',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('North Dakota','ND',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Northern Mariana Islands','MP',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Ohio','OH',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Pennsylvania','PA',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Puerto Rico','PR',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Palau','PW',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Rhode Island','RI',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('South Carolina','SC',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('South Dakota','SD',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Tennessee','TN',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Texas','TX',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Utah','UT',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Vermont','VT',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Virgin Islands','VI',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Virginia','VA',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Washington','WA',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('West Virginia','WV',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Wisconsin','WI',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Wyoming','WY',1,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Ontario','ON',2,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Quebec','QC',2,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Nova Scotia','NS',2,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('New Brunswick','NB',2,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Manitoba','MB',2,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('British Columbia','BC',2,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Prince Edward Island','PE',2,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Saskatchewan','SK',2,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Alberta','AB',2,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Newfoundland and Labrador','NL',2,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Northwest Territories','NT',2,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Yukon','YT',2,0);
INSERT INTO States (state,abbr,countryID,taxRate) VALUES ('Nunavut','NU',2,0);
GO

INSERT INTO DistributionCenters (Name,Phone,Fax,Street1,Street2,City,PostalCode,CountryCode,Latitude,Longitude,State) VALUES ('CURT Headquarters','877.287.8634','715.831.8712','6208 Industrial Dr','','Eau Claire','54701','US',44.7937928,-91.4105522,56);
INSERT INTO DistributionCenters (Name,Phone,Fax,Street1,Street2,City,PostalCode,CountryCode,Latitude,Longitude,State) VALUES ('Atlanta Area','866.528.2878','866.529.2878','110 Northpoint Parkway','Suite 200','Acworth','30102','US',34.0816304,-84.6444805,13);
INSERT INTO DistributionCenters (Name,Phone,Fax,Street1,Street2,City,PostalCode,CountryCode,Latitude,Longitude,State) VALUES ('Central California','888.488.5688','559.442.1952','3134 S. East Avenue','Suite 104','Fresno','93725','US',36.689023,-119.770402,6);
INSERT INTO DistributionCenters (Name,Phone,Fax,Street1,Street2,City,PostalCode,CountryCode,Latitude,Longitude,State) VALUES ('Dallas Area','888.635.9824','972.352.2617','1102 W. N. Carrier Parkway','Suite 300','Grand Prairie','75050','US',32.7853096,-97.0476728,49);
INSERT INTO DistributionCenters (Name,Phone,Fax,Street1,Street2,City,PostalCode,CountryCode,Latitude,Longitude,State) VALUES ('Detroit Area','877.287.8634','734.729.4216','5820 N. Hix Road','','Westland','48185','US',42.325723,-83.417394,27);
INSERT INTO DistributionCenters (Name,Phone,Fax,Street1,Street2,City,PostalCode,CountryCode,Latitude,Longitude,State) VALUES ('Houston Area','866.341.7295','281.341.9243','4921 Timber Lane','','Rosenberg','77471','US',29.567977,-95.778192,49);
INSERT INTO DistributionCenters (Name,Phone,Fax,Street1,Street2,City,PostalCode,CountryCode,Latitude,Longitude,State) VALUES ('Northeast/Mid-Atlantic','800.325.2613','978.870.1056','6370 Hedgewood Dr','Suite 110','Allentown','18106','US',40.593165,-75.602074,42);
INSERT INTO DistributionCenters (Name,Phone,Fax,Street1,Street2,City,PostalCode,CountryCode,Latitude,Longitude,State) VALUES ('Phoenix Area','866.817.7189','602393.1434','4002 West Turney','Suite 1','Phoenix','85019','US',33.5015379,-112.14433,5);
INSERT INTO DistributionCenters (Name,Phone,Fax,Street1,Street2,City,PostalCode,CountryCode,Latitude,Longitude,State) VALUES ('Seattle Area','877.832.2370','253.804.3511','2108 B Street NW','Suite 120','Auburn','98001','US',47.325739,-122.231159,54);
INSERT INTO DistributionCenters (Name,Phone,Fax,Street1,Street2,City,PostalCode,CountryCode,Latitude,Longitude,State) VALUES ('Toronto Area','877.287.8634','905.569.8330','3250 Ridgeway Dr','Unit 15','Mississauga','L5l 5Y6','Canada',43.5191392,-79.6981668,58);
GO

INSERT INTO PaymentType (name) VALUES ('Credit Card');
INSERT INTO PaymentType (name) VALUES ('Google Checkout');
INSERT INTO PaymentType (name) VALUES ('Paypal');
GO