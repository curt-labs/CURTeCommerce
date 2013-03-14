/* Add timezone to Profiles */
alter table Profile add timezone varchar(100) NOT NULL DEFAULT ('UTC')

/* Add show/hide bit to States */
alter table States add hide bit NOT NULL DEFAULT (0);

/* Alter Locations to allow for non US postal codes */
alter table Location alter column zip varchar(20) NULL;

/* Before running the next 2 queries, check to see if your database has the Regions module visible already.
This query is only necessary if the Regions / Taxes area is not visible */
Insert Into Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Regions / Taxes','/Admin/Regions',0,'Region / Tax Management',1,'/Admin/Content/img/module_icons/Bank.png');
INSERT INTO Modules (name,path,parentID,alt_text,inMenu) VALUES ('Save Rate','/Admin/Regions/SaveRate',22,'Save Tax Rate',0);
/* End */

/* Add Themes module to system */
INSERT INTO Modules (name,path,parentID,alt_text,inMenu,image) VALUES ('Themes','/Admin/Themes',0,'Theme Editor',1,'/Admin/Content/img/module_icons/ColorPalette.png');
INSERT INTO Modules (name,path,parentID,alt_text,inMenu) VALUES ('Add Theme','/Admin/Themes/Add',63,'Add Theme',1);
INSERT INTO Modules (name,path,parentID,alt_text,inMenu) VALUES ('Edit Theme','/Admin/Themes/Edit',63,'Edit Theme',0);
INSERT INTO Modules (name,path,parentID,alt_text,inMenu) VALUES ('Theme Files','/Admin/Themes/Files',63,'Manage Theme Files',0);

/* Creation of themes data structures */
CREATE TABLE Theme (
	ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	name varchar(255) NULL,
	active bit NOT NULL,
	screenshot varchar(255) NULL,
	dateAdded datetime NOT NULL
)
GO

CREATE TABLE ThemeFileType (
	ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	name varchar(100) NOT NULL,
	extension varchar(10) NOT NULL,
	mimetype varchar(50) NOT NULL,
	structure varchar(255) NULL
)
GO

CREATE TABLE ThemeArea (
	ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	name varchar(100) NOT NULL,
	controller varchar(100) NOT NULL
)
GO

CREATE INDEX IX_ThemeArea ON ThemeArea(controller)
GO

CREATE TABLE ThemeFile (
	ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	themeID int NOT NULL FOREIGN KEY REFERENCES Theme(ID),
	ThemeFileTypeID int NOT NULL FOREIGN KEY REFERENCES ThemeFileType(ID),
	themeAreaID int NOT NULL FOREIGN KEY REFERENCES ThemeArea(id),
	filePath varchar(255) NOT NULL,
	renderOrder int NOT NULL,
	dateAdded datetime NOT NULL,
	lastModified datetime NOT NULL,
	externalFile bit NOT NULL
)
GO
/* END Creation of themes data structures */

/* Populate Theme areas Types */
INSERT INTO ThemeArea (name,controller) VALUES
('Account Section','Account'),
('Authentication Pages','Authenticate'),
('Blog','Blog'),
('Shopping Cart','Cart'),
('Part Categories','Categories'),
('Contact Us','Contact'),
('Global Styles','Base'),
('FAQ','FAQ'),
('Homepage','Index'),
('Locations','Locations'),
('Content Pages','Page'),
('Newsletter','Newsletter'),
('Part Details','Part'),
('Payment Pages','Payment'),
('Review Form','Review'),
('Search Pages','Search'),
('Share Form','Share'),
('Lookup Landing Pages','Lookup'),
('Testimonials','Testimonials');

/* Populate Theme File Types */
INSERT INTO ThemeFileType (name,extension,mimetype,structure) VALUES
('Style Sheet','.css','text/css','<link href="[path]" rel="stylesheet" type="text/css">'),
('Javascript','.js','application/javascript','<script defer src="[path]"></script>');

/* Create OrderEDI table for EDI Tracking */
CREATE TABLE OrderEDI (
	ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	orderID int NOT NULL FOREIGN KEY REFERENCES Cart(ID),
	dateGenerated datetime NOT NULL,
	editext text NULL,
	filename varchar(255) NOT NULL,
	dateAcknowledged datetime NULL
)
GO

/* Index the OrderEDI table on order ID for faster querying. */
CREATE INDEX IX_OrderEDI_Cart ON OrderEDI(orderID);

/* This last SQL query is to populate all orders prior to the OrderEDI table's existence. */
INSERT INTO OrderEDI (orderID,dateGenerated,editext,filename,dateAcknowledged)
(SELECT ID,GetDATE(),'','',GETDATE() FROM Cart);