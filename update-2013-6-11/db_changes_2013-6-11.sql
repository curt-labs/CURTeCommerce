/* Add Order Notes to Cart table */
alter table Cart add notes varchar(max) NULL;
alter table Cart add handling_fee decimal(18, 2) NOT NULL DEFAULT(0);
alter table States add handlingFee decimal(18, 2) NOT NULL DEFAULT(0);

CREATE TABLE OrderStatus (
	ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	status varchar(255) NOT NULL,
)
GO

CREATE TABLE OrderHistory (
	ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	orderID int NOT NULL FOREIGN KEY REFERENCES Cart(ID),
	statusID int NOT NULL FOREIGN KEY REFERENCES OrderStatus(ID),
	dateAdded datetime NOT NULL,
	changedBy varchar(255) NOT NULL
)
GO

SET IDENTITY_INSERT OrderStatus ON;

INSERT INTO OrderStatus (ID,status) VALUES
(1,'Payment Pending'),
(2,'Payment Complete'),
(3,'Payment Declined'),
(4,'Processed'),
(5,'Refunded'),
(6,'Back Order'),
(7,'Check Notes'),
(8,'Message Sent'),
(9,'Awaiting Tracking'),
(10,'Shipped'),
(11,'Awaiting Cancellation'),
(12,'Cancelled'),
(13,'Fraudulent'),
(14,'Void'),
(15,'Complete');

SET IDENTITY_INSERT OrderStatus OFF;
