--create table ReturnedProducts(
--ItemName varchar(20)  not null,
--ItemCategory varchar(20) not null,
--Quantity decimal not null,
--UnitOfMeasurement varchar(20),
--ReturnedBy varchar(20) not null,
--ReturnedTo varchar(20) not null,
--ReturnType varchar(20) not null,
--DateRecorded date not null default GETDATE(),
--);
--DROP TABLE ReturnedProducts;
--SELECT * FROM ReturnedProducts;
--alter table ReturnedProducts alter Column Quantity decimal(10,2);
DECLARE @StartDate DATE = '2025-12-01';
DECLARE @EndDate DATE   = '2025-12-31';

--SELECT 
--    o.ItemName,
--    o.ItemCategory,
--    DATEPART(YEAR, o.DateRecorded) AS Year,
--    DATEPART(WEEK, o.DateRecorded) AS WeekNumber,
--	o.UnitOfMeasurement,
--    SUM(o.Quantity) - ISNULL(SUM(r.Quantity), 0) AS WeeklyUsage
--FROM InventoryOutGoing o
--LEFT JOIN ReturnedProducts r
--    ON o.ItemName = r.ItemName
--    AND o.ItemCategory = r.ItemCategory
--    AND o.DateRecorded = r.DateRecorded
--    AND r.ReturnType = 'NOT REJECT'
--WHERE o.DateRecorded BETWEEN @StartDate AND @EndDate
--GROUP BY 
--    o.ItemName,
--    o.ItemCategory,
--	o.UnitOfMeasurement,
--    DATEPART(YEAR, o.DateRecorded),
--    DATEPART(WEEK, o.DateRecorded)
--ORDER BY 
--    Year, WeekNumber, o.ItemName;

--SELECT ItemName,ItemCategory,DateRecorded,UnitOfMeasurement,SUM(Quantity) AS DailyTotal FROM ReturnedProducts WHERE DateRecorded BETWEEN @StartDate AND @EndDate GROUP BY ItemName,ItemCategory,UnitOfMeasurement,DateRecorded ORDER BY DateRecorded, ItemName;;
SELECT ItemName,ItemCategory,DATEPART(YEAR, DateRecorded) AS Year,DATEPART(WEEK, DateRecorded) AS WeekNumber,UnitOfMeasurement,SUM(Quantity) AS DailyTotal FROM ReturnedProducts WHERE DateRecorded BETWEEN @StartDate AND @EndDate GROUP BY ItemName, ItemCategory,DATEPART(YEAR, DateRecorded) ,DATEPART(WEEK, DateRecorded),UnitOfMeasurement ORDER BY Year, WeekNumber, ItemName;

