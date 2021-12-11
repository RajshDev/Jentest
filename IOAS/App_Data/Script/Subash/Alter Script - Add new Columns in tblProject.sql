
/****** Newly Added Columns ******/
Alter table dbo.tblProject Add 
ProjectNumber nvarchar(200) NULL,
ProposalId int NULL,
ProposalApprovedDate datetime NULL, 
FundingGovtMinistry int NULL,
SponProjectCategory nvarchar(200) NULL,
ForeignProjectAgencyState nvarchar(200) NULL,
ForeignProjectAgencyLocation nvarchar(max) NULL, 
TotalProjectStaffCount int NULL,
JRFStaffCount int NULL,
JRFStaffSalary decimal(18, 2) NULL,
SRFStaffCount int NULL,
SRFStaffSalary decimal(18, 2) NULL,
RAStaffCount int NULL,
RAStaffSalary decimal(18, 2) NULL,
PAStaffCount int NULL,
PAStaffSalary decimal(18, 2) NULL,
PQStaffCount int NULL,
PQStaffSalary decimal(18, 2) NULL,
SumofStaffCount int NULL,
SumSalaryofStaff decimal(18, 2) NULL, 
CrtdUserId int NULL,
CrtdTS datetime  NULL


/****** Modified Columns ******/

ALTER TABLE dbo.tblProject 
ALTER COLUMN SchemeAgencyName nvarchar(200)

ALTER TABLE dbo.tblProject 
ALTER COLUMN SchemePersonDesignation nvarchar(200)
 
ALTER TABLE dbo.tblProject 
ALTER COLUMN FundingType nvarchar(200)

ALTER TABLE dbo.tblProject 
ALTER COLUMN IndianFundedBy nvarchar(200)

ALTER TABLE dbo.tblProject 
ALTER COLUMN FundingGovtBody nvarchar(200)

ALTER TABLE dbo.tblProject 
ALTER COLUMN FundingGovtAgency nvarchar(200)

ALTER TABLE dbo.tblProject 
ALTER COLUMN FundingNonGovtBody nvarchar(200)

ALTER TABLE dbo.tblProject 
ALTER COLUMN FundingNonGovtAgency nvarchar(200)

ALTER TABLE dbo.tblProject 
ALTER COLUMN ForeignFundedBy nvarchar(200)

ALTER TABLE dbo.tblProject 
ALTER COLUMN ForeignFundedFundingBody nvarchar(200)

ALTER TABLE dbo.tblProject 
ALTER COLUMN IndianProjectAgencyState nvarchar(200)

ALTER TABLE dbo.tblProject 
ALTER COLUMN TaxStatus nvarchar(200)

ALTER TABLE dbo.tblProject 
ALTER COLUMN SponsoringAgencyCode int 
 
 