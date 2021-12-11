alter table dbo.tblProject
add column FundingGovtAgencyDept nvarchar(MAX)

alter table dbo.tblProject
add column FundingGovtDeptAmount decimal(18, 2)

alter table dbo.tblProject
add column FundingGovtMinistryAmount decimal(18, 2)

alter table dbo.tblProject
add column FundingGovtUniv nvarchar(MAX)

alter table dbo.tblProject
add column FundingGovtUnivAmount decimal(18, 2)

alter table dbo.tblProject
add column FundingNonGovtAgencyIndustry nvarchar(MAX)

alter table dbo.tblProject
add column FundingNonGovtIndstryAmount decimal(18, 2)

alter table dbo.tblProject
add column FundingNonGovtUniv nvarchar(MAX)

alter table dbo.tblProject
add column FundingNonGovtUnivAmount decimal(18, 2)

alter table dbo.tblProject
add column FundingNonGovtOthers nvarchar(MAX)

alter table dbo.tblProject
add column FundingNonGovtOthersAmount decimal(18, 2)

alter table dbo.tblProject
add column ForgnGovtAgencyDepartmentCountry int

alter table dbo.tblProject
add column ForgnGovtAgencyDepartment nvarchar(MAX)

alter table dbo.tblProject
add column ForgnGovtAgencyDepartmentAmount decimal(18, 2)

alter table dbo.tblProject
add column ForgnGovtUnivCountry int

alter table dbo.tblProject
add column ForgnGovtUniversity nvarchar(MAX)

alter table dbo.tblProject
add column ForgnGovtUniversityAmount decimal(18, 2)


alter table dbo.tblProject
add column ForgnGovtOthersCountry int

alter table dbo.tblProject
add column ForgnGovtOthers nvarchar(MAX)

alter table dbo.tblProject
add column ForgnGovtOthersAmount decimal(18, 2)

alter table dbo.tblProject
add column ForgnNonGovtAgencyDepartmentCountry int

alter table dbo.tblProject
add column ForgnNonGovtAgencyDepartment nvarchar(MAX)

alter table dbo.tblProject
add column ForgnNonGovtAgencyDepartmentAmount decimal(18, 2)

alter table dbo.tblProject
add column ForgnNonGovtAgencyUnivCountry int

alter table dbo.tblProject
add column ForgnNonGovtAgencyUniversity nvarchar(MAX)

alter table dbo.tblProject
add column ForgnNonGovtAgencyUnivAmount decimal(18, 2)

alter table dbo.tblProject
add column ForgnNonGovtOthersCountry int

alter table dbo.tblProject
add column ForgnNonGovtOthers nvarchar(MAX)

alter table dbo.tblProject
add column ForgnNonGovtOthersAmount decimal(18, 2)

alter table dbo.tblProject
drop column FundingGovtAgency 

alter table dbo.tblProject
drop column FundingGovtAmount 

alter table dbo.tblProject
drop column FundingNonGovtAgency 

alter table dbo.tblProject
drop column FundingNonGovtAmount 


USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblProjectFundingBody]    Script Date: 09/21/2018 15:37:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProjectFundingBody](
	[FundingBodyId] [int] IDENTITY(1,1) NOT NULL,
	[ProjectId] [int] NULL,
	[IndProjectFundingGovtBody] [int] NULL,
	[IndProjectFundingNonGovtBody] [int] NULL,
	[ForgnProjectFundingGovtBody] [int] NULL,
	[ForgnProjectFundingNonGovtBody] [int] NULL,
	[CrtdTS] [datetime] NULL,
	[CrtdUserId] [int] NULL,
	[IsDeleted] [bit] NULL,
	[UpdtUserId] [int] NULL,
	[UpdtTS] [datetime] NULL,
 CONSTRAINT [PK_tblProjectFundingBody] PRIMARY KEY CLUSTERED 
(
	[FundingBodyId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO