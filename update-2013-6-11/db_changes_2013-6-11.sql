/* Add Order Notes to Cart table */
alter table Cart add notes varchar(max) NULL;
alter table Cart add handling_fee decimal(18, 2) NOT NULL DEFAULT(0);
alter table States add handlingFee decimal(18, 2) NOT NULL DEFAULT(0);