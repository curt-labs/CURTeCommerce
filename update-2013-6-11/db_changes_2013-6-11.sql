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

INSERT INTO OrderStatus (status) VALUES
('Payment Pending'),
('Payment Complete'),
('Payment Declined'),
('Processed'),
('Refunded'),
('Back Order'),
('Check Notes'),
('Message Sent'),
('Awaiting Tracking'),
('Shipped'),
('Awaiting Cancellation'),
('Cancelled'),
('Fraudulent'),
('Void'),
('Complete');