alter table dbo.tblProject
add PCF decimal(18, 3),RMF decimal(18, 3)

alter table dbo.tblProjectCoPI
add PCF decimal(18, 3),RMF decimal(18, 3)

alter table dbo.tblProjectAllocation
add [Year] int

alter table dbo.tblSchemes
add OverheadPercentage decimal(18, 3)

alter table dbo.tblProject
add IsYearWiseAllocation bit default 0