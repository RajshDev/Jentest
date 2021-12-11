
alter table dbo.tblProjectCoPI
add Status varchar(50)
alter table dbo.tblProjectCoPI
drop column isdeleted
alter table dbo.tblProjectAllocation
add Year int  

alter table dbo.tblProject
alter column FinancialYear int

alter table [dbo].[tblProjectCoPI]
add PCF decimal(18,3), RMF decimal(18,3)

alter table [dbo].[tblProject]
add IsYearWiseAllocation bit default 0, PCF decimal(18, 3),RMF decimal(18, 3)


/****** Object:  Table [dbo].[tblInstallment]    Script Date: 10/06/2018 09:46:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblInstallment](
	[InstallmentID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectId] [int] NULL,
	[InstallmentNo] [int] NULL,
	[InstallmentValue] [decimal](18, 3) NULL,
	[Year] [int] NULL,
	[NoOfInstallment] [int] NULL,
	[InstallmentValueForYear] [decimal](18, 3) NULL,
 CONSTRAINT [PK_tblInstallment] PRIMARY KEY CLUSTERED 
(
	[InstallmentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblBudgetHead]    Script Date: 10/06/2018 10:06:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBudgetHead](
	[BudgetHeadId] [int] IDENTITY(1,1) NOT NULL,
	[HeadName] [varchar](250) NULL,
	[IsRecurring] [bit] NULL,
 CONSTRAINT [PK_tblBudgetHead] PRIMARY KEY CLUSTERED 
(
	[BudgetHeadId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[tblBudgetHead] ON
INSERT [dbo].[tblBudgetHead] ([BudgetHeadId], [HeadName], [IsRecurring]) VALUES (1, N'Staff', 1)
INSERT [dbo].[tblBudgetHead] ([BudgetHeadId], [HeadName], [IsRecurring]) VALUES (2, N'Consumables', 1)
INSERT [dbo].[tblBudgetHead] ([BudgetHeadId], [HeadName], [IsRecurring]) VALUES (3, N'Contingencies', 1)
INSERT [dbo].[tblBudgetHead] ([BudgetHeadId], [HeadName], [IsRecurring]) VALUES (4, N'Travel', 1)
INSERT [dbo].[tblBudgetHead] ([BudgetHeadId], [HeadName], [IsRecurring]) VALUES (5, N'Component', 1)
INSERT [dbo].[tblBudgetHead] ([BudgetHeadId], [HeadName], [IsRecurring]) VALUES (6, N'Overheads', 1)
INSERT [dbo].[tblBudgetHead] ([BudgetHeadId], [HeadName], [IsRecurring]) VALUES (7, N'Equipment', 0)
SET IDENTITY_INSERT [dbo].[tblBudgetHead] OFF
/****** Object:  Default [DF_tblBudgetHead_IsRecurring]    Script Date: 10/06/2018 10:06:56 ******/
ALTER TABLE [dbo].[tblBudgetHead] ADD  CONSTRAINT [DF_tblBudgetHead_IsRecurring]  DEFAULT ((0)) FOR [IsRecurring]
GO


GO

/****** Object:  Table [dbo].[tblSponsoredSchemes]    Script Date: 10/09/2018 19:24:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblSponsoredSchemes](
	[SponsoredSchemesId] [int] IDENTITY(1,1) NOT NULL,
	[SchemeCode] [varchar](50) NULL,
 CONSTRAINT [PK_tblSponsoredSchemes] PRIMARY KEY CLUSTERED 
(
	[SponsoredSchemesId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO





drop table dbo.tblFinancialYear
GO

/****** Object:  Table [dbo].[tblFinYear]    Script Date: 10/11/2018 12:09:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblFinYear](
	[FinYearId] [int] IDENTITY(1,1) NOT NULL,
	[Year] [varchar](10) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[CurrentYearFlag] [bit] NULL,
 CONSTRAINT [PK_tblFinYear] PRIMARY KEY CLUSTERED 
(
	[FinYearId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[tblFinYear] ADD  CONSTRAINT [DF_tblFinYear_CurrentYearFlag]  DEFAULT ((0)) FOR [CurrentYearFlag]
GO


alter table dbo.tblFinYear 
alter column year varchar(10)

alter table dbo.tblOtherInstituteCoPI
alter column Name varchar(250)

alter table dbo.tblOtherInstituteCoPI
alter column Institution varchar(250)

alter table dbo.tblOtherInstituteCoPI
alter column Department varchar(250)

alter table dbo.tblProposal
add TentativeStartDate datetime,TentativeCloseDate datetime
alter table dbo.tblProposal
add FinancialYear int,SourceReferenceNumber int, SourceEmailDate datetime

update tblProposal set ProjectSchemeCode = ''
alter table dbo.tblProposal
alter column ProjectSchemeCode int

update tblproject set SchemeCode = ''
alter table dbo.tblproject
alter column SchemeCode int

alter table dbo.tblproposal
add Status varchar(50)
alter table dbo.tblproject
add ScientistName varchar(250),ScientistEmail varchar(250),ScientistMobile varchar(250),ScientistAddress varchar(250)

GO

/****** Object:  Table [dbo].[tblProjectOtherInstituteCoPI]    Script Date: 10/11/2018 12:14:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblProjectOtherInstituteCoPI](
	[OtherCoPIId] [int] IDENTITY(1,1) NOT NULL,
	[ProjectId] [int] NULL,
	[Name] [varchar](250) NULL,
	[Department] [varchar](250) NULL,
	[Institution] [varchar](250) NULL,
	[Remarks] [varchar](250) NULL,
	[Crtd_TS] [datetime] NULL,
	[CrtdUserId] [int] NULL,
	[Updt_TS] [datetime] NULL,
	[UpdtUserId] [int] NULL,
	[DeletedDate] [datetime] NULL,
	[DeletedUserid] [int] NULL,
	[Status] [varchar](50) NULL,
 CONSTRAINT [PK_tblProjectOtherInstituteCoPI] PRIMARY KEY CLUSTERED 
(
	[OtherCoPIId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


alter table tblproject
alter column FinancialYear int

alter table tblAccountGroup
add ParentgroupId int


alter table dbo.tblProjectEnhancement alter column Status varchar(max)
alter table dbo.tblProjectEnhancementAllocation add Status varchar(max)