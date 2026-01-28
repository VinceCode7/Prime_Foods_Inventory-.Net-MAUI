--create table InventoryOutGoing(
--ItemName varchar(20)  not null,
--ItemCategory varchar(20) not null,
--Quantity decimal not null,
--UnitOfMeasurement varchar(20),
--GivenTo varchar(20) not null,
--GivenBy varchar(20) not null,
--DateRecorded date not null default GETDATE(),
--);
--drop table InventoryOutGoing;
--select * from InventoryOutGoing;
--SELECT SUM(o.Quantity),DateRecorded FROM InventoryOutGoing o GROUP BY DateRecorded;
--alter table InventoryOutGoing alter Column Quantity decimal(10,2);
--select ISNULL(SUM(r.Quantity), 0),r.DateRecorded from ReturnedProducts r where r.ReturnType = 'NOT REJECT' group by r.DateRecorded ;
DECLARE @StartDate DATE = '2025-12-01';
DECLARE @EndDate DATE   = '2025-12-31';

--SELECT ItemName,ItemCategory,
--DateRecorded,
--UnitOfMeasurement,
--SUM(Quantity) AS DailyTotal
--FROM InventoryOutGoing
--WHERE DateRecorded BETWEEN @StartDate AND @EndDate
--GROUP BY
--ItemName,
--ItemCategory,
--UnitOfMeasurement,
--DateRecorded
--ORDER BY DateRecorded, ItemName;
SELECT ItemName,ItemCategory,Quantity,UnitOfMeasurement,GivenTo,GivenBy,DateRecorded FROM InventoryOutGoing WHERE DateRecorded BETWEEN @StartDate AND @EndDate;