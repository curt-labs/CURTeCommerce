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