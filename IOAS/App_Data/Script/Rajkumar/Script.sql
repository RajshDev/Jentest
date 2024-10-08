Drop table tblUserType

ALTER TABLE tblUser DROP column UserType;


--Date for table modfication 03/08/2018

Alter table tblDepartment ADD CreatedUserId int,LastUpdateUserId int,CreatedTS datetime,UpdatedTS datetime,IsDeleted bit
--set IsDeleted defalut value =0

Alter table tblRole ADD CreatedUserId int,CreatedTS datetime,LastUpdateUserId int,UpdatedTS datetime,IsDeleted bit
--set IsDeleted defalut value =0

Alter table tblUser ADD CreatedUserId int,LastUpdateUserId int 

-----------------------------------------------------------------------------------------------------------------------------------

--Date for table modfication 31/08/2018

-----------------------------------------------------------

ALTER TABLE [dbo].[tblDepartment] DROP COLUMN IsDeleted

ALTER TABLE [dbo].[tblDepartment] ADD Status varchar(50)

ALTER TABLE [dbo].[tblDepartment] ADD LastUpdateUserId int ,LastUpdated_TS datetime

update [dbo].[tblDepartment] set Status='Active'
---------------------------------------------------------------------
ALTER TABLE [dbo].[tblRole] DROP constraint DF_tblRole_IsDeleted 
ALTER TABLE [dbo].[tblRole]
DROP COLUMN IsDeleted

ALTER TABLE [dbo].[tblRole] ADD Status varchar(50)

update [dbo].[tblRole] set Status='Active'
---------------------------------------------------------------------
ALTER TABLE [dbo].[tblInstituteMaster] DROP COLUMN IsDeleted

ALTER TABLE [dbo].[tblInstituteMaster] ADD Status varchar(50)

ALTER TABLE [dbo].[tblInstituteMaster] ADD LastUpdatedUserId int

update [dbo].[tblInstituteMaster] set Status='Active'

----------------------------------------------------------------------
ALTER TABLE [dbo].[tblUser] DROP constraint DF_tblUser_IsDeleted 
ALTER TABLE [dbo].[tblUser] DROP COLUMN IsDeleted

ALTER TABLE [dbo].[tblUser] ADD Status varchar(50)

update [dbo].[tblUser] set Status='Active'
----------------------------------------------------------------------
ALTER TABLE  [dbo].[tblRoleaccess] ADD Status varchar(50)
update [dbo].[tblRoleaccess] set Status='Active'
---------------------------------------------------------------------
ALTER TABLE  [dbo].[tblInstituteMaster] ADD LastUpdatedUserId int
---------------------------------------------------------------------
/**Drop table list**/
drop table [dbo].[tblAgencyMaster]
drop table [dbo].[tblAllocationHeadsMaster]
drop table [dbo].[tblConsultancyFundingCategory]
drop table [dbo].[tblFunction]
drop table [dbo].[tblFunctionStatus]
drop table [dbo].[tblModules]
drop table [dbo].[tblProjectStaffCategoryMaster]
drop table [dbo].[tblSchemes]
drop table [dbo].[tblSRBItemCategory]

----------------------------------------------------------------------------


USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblAccountGroup]    Script Date: 8/31/2018 11:21:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAccountGroup](
	[AccountGroupId] [int] IDENTITY(1,1) NOT NULL,
	[AccountGroup] [varchar](50) NULL,
	[AccountType] [int] NULL,
	[AccountGroupCode] [int] NULL,
	[CreatedTS] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	[LastUpdatedTS] [datetime] NULL,
	[LastUpdatedUserId] [int] NULL,
 CONSTRAINT [PK_tblAccountGroup] PRIMARY KEY CLUSTERED 
(
	[AccountGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblAccountType]    Script Date: 8/31/2018 11:21:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAccountType](
	[AccountTypeId] [int] IDENTITY(1,1) NOT NULL,
	[AccountType] [varchar](50) NULL,
	[AccountTypeCode] [varchar](10) NULL,
 CONSTRAINT [PK_tblAccountType] PRIMARY KEY CLUSTERED 
(
	[AccountTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblAgencyMaster]    Script Date: 8/31/2018 11:21:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAgencyMaster](
	[AgencyId] [int] IDENTITY(1,1) NOT NULL,
	[AgencyName] [nvarchar](max) NULL,
	[ContactPerson] [nvarchar](max) NULL,
	[ContactNumber] [nvarchar](200) NULL,
	[ContactEmail] [nvarchar](200) NULL,
	[Address] [nvarchar](max) NULL,
	[State] [nvarchar](200) NULL,
	[Country] [int] NULL,
	[Crtd_TS] [datetime] NULL,
	[Crtd_UserId] [int] NULL,
	[LastupdatedUserid] [int] NULL,
	[Lastupdate_TS] [datetime] NULL,
	[Reason] [nvarchar](200) NULL,
	[AgencyCode] [nvarchar](200) NULL,
	[AgencyType] [int] NULL,
	[Scheme] [int] NULL,
	[Status] [varchar](50) NULL,
	[GSTIN] [varchar](100) NULL,
	[TAN] [varchar](100) NULL,
	[PAN] [varchar](10) NULL,
 CONSTRAINT [PK_tblAgencyMaster] PRIMARY KEY CLUSTERED 
(
	[AgencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblAllocationHeadsMaster]    Script Date: 8/31/2018 11:21:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAllocationHeadsMaster](
	[AllocationHeadId] [int] IDENTITY(1,1) NOT NULL,
	[AllocationHead] [nvarchar](max) NULL,
	[CrtdUserId] [int] NULL,
	[CrtdTS] [datetime] NULL,
	[Reason] [nvarchar](max) NULL,
	[LastUpdatedUserid] [int] NULL,
	[LastUpdated_TS] [datetime] NULL,
	[Status] [varchar](50) NULL,
 CONSTRAINT [PK_tblAllocationHeadsMaster] PRIMARY KEY CLUSTERED 
(
	[AllocationHeadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblFunction]    Script Date: 8/31/2018 11:21:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblFunction](
	[FunctionId] [int] IDENTITY(1,1) NOT NULL,
	[FunctionName] [varchar](50) NULL,
	[ActionName] [varchar](50) NULL,
	[ControllerName] [varchar](50) NULL,
 CONSTRAINT [PK_tblFunction] PRIMARY KEY CLUSTERED 
(
	[FunctionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblFunctionStatus]    Script Date: 8/31/2018 11:21:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblFunctionStatus](
	[FunctionStatusId] [int] IDENTITY(1,1) NOT NULL,
	[FunctionId] [int] NULL,
	[Status] [varchar](50) NULL,
 CONSTRAINT [PK_tblFunctionStatus] PRIMARY KEY CLUSTERED 
(
	[FunctionStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblModules]    Script Date: 8/31/2018 11:21:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblModules](
	[ModuleID] [int] IDENTITY(1,1) NOT NULL,
	[ModuleName] [varchar](50) NULL,
	[IsDeleted] [bit] NULL,
	[FunctionId] [int] NULL,
 CONSTRAINT [PK_tblModules] PRIMARY KEY CLUSTERED 
(
	[ModuleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblSchemes]    Script Date: 8/31/2018 11:21:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblSchemes](
	[SchemeId] [int] IDENTITY(1,1) NOT NULL,
	[SchemeName] [nvarchar](max) NULL,
	[ProjectType] [int] NULL,
	[SchemeCode] [nvarchar](max) NULL,
	[Status] [varchar](50) NULL,
	[CreatedUserId] [int] NULL,
	[Created_TS] [datetime] NULL,
	[LastUpdatedUsedId] [int] NULL,
	[LastUpdated_TS] [datetime] NULL,
 CONSTRAINT [PK_tblSchemes] PRIMARY KEY CLUSTERED 
(
	[SchemeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblSRBItemCategory]    Script Date: 8/31/2018 11:21:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblSRBItemCategory](
	[SRBItemCategotyId] [int] IDENTITY(1,1) NOT NULL,
	[Category] [varchar](200) NULL,
	[Asset_f] [bit] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_UserID] [int] NULL,
	[UPDT_UserID] [int] NULL,
	[Status] [varchar](50) NULL,
 CONSTRAINT [PK_tblSRBItemCategory] PRIMARY KEY CLUSTERED 
(
	[SRBItemCategotyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[tblAccountGroup] ON 

GO
INSERT [dbo].[tblAccountGroup] ([AccountGroupId], [AccountGroup], [AccountType], [AccountGroupCode], [CreatedTS], [CreatedUserId], [LastUpdatedTS], [LastUpdatedUserId]) VALUES (1, N'Account', 4, 1, CAST(0x0000A94A012D2D47 AS DateTime), 1, NULL, NULL)
GO
INSERT [dbo].[tblAccountGroup] ([AccountGroupId], [AccountGroup], [AccountType], [AccountGroupCode], [CreatedTS], [CreatedUserId], [LastUpdatedTS], [LastUpdatedUserId]) VALUES (2, N'Account 1', 4, 2, CAST(0x0000A94A012DF51D AS DateTime), 1, NULL, NULL)
GO
INSERT [dbo].[tblAccountGroup] ([AccountGroupId], [AccountGroup], [AccountType], [AccountGroupCode], [CreatedTS], [CreatedUserId], [LastUpdatedTS], [LastUpdatedUserId]) VALUES (3, N'test', 1, 3, CAST(0x0000A94A012FAFB7 AS DateTime), 1, CAST(0x0000A94B00E902E3 AS DateTime), 1)
GO
SET IDENTITY_INSERT [dbo].[tblAccountGroup] OFF
GO
SET IDENTITY_INSERT [dbo].[tblAccountType] ON 

GO
INSERT [dbo].[tblAccountType] ([AccountTypeId], [AccountType], [AccountTypeCode]) VALUES (1, N'Expense', N'E')
GO
INSERT [dbo].[tblAccountType] ([AccountTypeId], [AccountType], [AccountTypeCode]) VALUES (2, N'Income', N'I')
GO
INSERT [dbo].[tblAccountType] ([AccountTypeId], [AccountType], [AccountTypeCode]) VALUES (3, N'Liability', N'L')
GO
INSERT [dbo].[tblAccountType] ([AccountTypeId], [AccountType], [AccountTypeCode]) VALUES (4, N'Asset', N'A')
GO
SET IDENTITY_INSERT [dbo].[tblAccountType] OFF
GO
SET IDENTITY_INSERT [dbo].[tblAgencyMaster] ON 

GO
INSERT [dbo].[tblAgencyMaster] ([AgencyId], [AgencyName], [ContactPerson], [ContactNumber], [ContactEmail], [Address], [State], [Country], [Crtd_TS], [Crtd_UserId], [LastupdatedUserid], [Lastupdate_TS], [Reason], [AgencyCode], [AgencyType], [Scheme], [Status], [GSTIN], [TAN], [PAN]) VALUES (1, N'MNC&CO', N'testperson', N'1234567890', N'MNC@gmail.com', N'chennai-600036', N'TamilNadu', 128, CAST(0x0000A945010C30CE AS DateTime), 1, 1, CAST(0x0000A94600D65BA8 AS DateTime), NULL, N'MNC001', 1, 0, N'Active', N'DSPI12', N'TAN123', N'ABCD123')
GO
INSERT [dbo].[tblAgencyMaster] ([AgencyId], [AgencyName], [ContactPerson], [ContactNumber], [ContactEmail], [Address], [State], [Country], [Crtd_TS], [Crtd_UserId], [LastupdatedUserid], [Lastupdate_TS], [Reason], [AgencyCode], [AgencyType], [Scheme], [Status], [GSTIN], [TAN], [PAN]) VALUES (2, N'fdfsdf', N'sfdf', N'04412345455', N'ads@gmail.com', N'dffd', N'vsfvsdv', 80, CAST(0x0000A945010FD774 AS DateTime), 1, 1, CAST(0x0000A945010FDE7B AS DateTime), NULL, N'fsdddfd', 1, 0, N'InActive', NULL, NULL, NULL)
GO
INSERT [dbo].[tblAgencyMaster] ([AgencyId], [AgencyName], [ContactPerson], [ContactNumber], [ContactEmail], [Address], [State], [Country], [Crtd_TS], [Crtd_UserId], [LastupdatedUserid], [Lastupdate_TS], [Reason], [AgencyCode], [AgencyType], [Scheme], [Status], [GSTIN], [TAN], [PAN]) VALUES (3, N'testagency', N'testperson', N'123567', N'abc@gmail.com', N'chennai', N'tamilnadu', 128, CAST(0x0000A94600D9697C AS DateTime), 1, NULL, NULL, NULL, N'agencycode1', 2, 4, N'Active', NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[tblAgencyMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[tblAllocationHeadsMaster] ON 

GO
INSERT [dbo].[tblAllocationHeadsMaster] ([AllocationHeadId], [AllocationHead], [CrtdUserId], [CrtdTS], [Reason], [LastUpdatedUserid], [LastUpdated_TS], [Status]) VALUES (1, N'Staff', 1, CAST(0x0000A94501151038 AS DateTime), NULL, NULL, NULL, N'Active')
GO
INSERT [dbo].[tblAllocationHeadsMaster] ([AllocationHeadId], [AllocationHead], [CrtdUserId], [CrtdTS], [Reason], [LastUpdatedUserid], [LastUpdated_TS], [Status]) VALUES (2, N'Traval', 1, CAST(0x0000A94501151FD0 AS DateTime), NULL, 1, CAST(0x0000A94501153315 AS DateTime), N'InActive')
GO
INSERT [dbo].[tblAllocationHeadsMaster] ([AllocationHeadId], [AllocationHead], [CrtdUserId], [CrtdTS], [Reason], [LastUpdatedUserid], [LastUpdated_TS], [Status]) VALUES (3, N'other', 1, CAST(0x0000A94501152CF9 AS DateTime), NULL, NULL, NULL, N'Active')
GO
INSERT [dbo].[tblAllocationHeadsMaster] ([AllocationHeadId], [AllocationHead], [CrtdUserId], [CrtdTS], [Reason], [LastUpdatedUserid], [LastUpdated_TS], [Status]) VALUES (4, N'Travals', 1, CAST(0x0000A9450115F0B5 AS DateTime), NULL, 1, CAST(0x0000A94501160622 AS DateTime), N'Active')
GO
SET IDENTITY_INSERT [dbo].[tblAllocationHeadsMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[tblFunction] ON 

GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (1, N'Department', N'Department', N'Account')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (2, N'Role', N'Role', N'Account')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (3, N'Access Rights', N'AccessRights', N'Account')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (4, N'Reset Password', N'ResetPassword', N'Account')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (5, N'User Management', N'Createuser', N'Account')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (6, N'Principal Investigator', N'CreatePI', N'Account')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (7, N'Institute', N'Institute', N'Account')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (8, N'Process Guideline', N'ProcessGuideline', N'ProcessGuideline')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (9, N'Proposal', N'CreateProposal', N'Proposal')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (10, N'Project opening', N'ProjectOpening', N'Project')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (11, N'Project Enhancement', N'ProjectEnhancement', N'Project')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (12, N'Project Extension', N'ProjectExtension', N'Project')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (13, N'SRB', N'SRBList', N'Facility')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (14, N'Tapal', N'Tapal', N'Facility')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (15, N'Builder', N'List', N'Reports')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (16, N'Viewer', N'ReportViewer', N'Reports')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (17, N'Project Proposal', N'ProjectProposal', N'CrystalReport')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (18, N'Project Report', N'Projectreport', N'CrystalReport')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (19, N'Agency Management', N'Createagency', N'Account')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (20, N'Allocation Head', N'AllocationHead', N'Account')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (21, N'Project Staff Category', N'Projectstaff', N'Account')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (22, N'Consultancy Funding Category', N'Consultancyfundingcategory', N'Account')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (23, N'Scheme', N'Schemes', N'Account')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (24, N'Account Group', N'AccountGroup', N'Account')
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName]) VALUES (25, N'SRB Item Category', N'SRBItemcategory', N'Account')
GO
SET IDENTITY_INSERT [dbo].[tblFunction] OFF
GO
SET IDENTITY_INSERT [dbo].[tblFunctionStatus] ON 

GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (1, 1, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (2, 1, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (3, 2, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (4, 2, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (5, 3, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (6, 3, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (7, 4, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (8, 4, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (9, 5, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (10, 5, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (11, 6, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (12, 6, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (13, 7, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (14, 7, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (15, 8, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (16, 8, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (17, 9, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (18, 9, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (19, 10, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (20, 10, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (21, 11, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (22, 11, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (23, 12, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (24, 12, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (25, 13, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (26, 13, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (27, 14, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (28, 14, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (29, 15, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (30, 15, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (31, 16, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (32, 16, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (33, 17, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (34, 17, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (35, 18, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (36, 18, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (37, 19, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (38, 19, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (39, 20, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (40, 20, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (41, 21, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (42, 21, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (43, 22, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (44, 22, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (45, 23, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (46, 23, N'InActive')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (47, 25, N'Active')
GO
INSERT [dbo].[tblFunctionStatus] ([FunctionStatusId], [FunctionId], [Status]) VALUES (48, 25, N'InActive')
GO
SET IDENTITY_INSERT [dbo].[tblFunctionStatus] OFF
GO
SET IDENTITY_INSERT [dbo].[tblModules] ON 

GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (1, N'Administration', 0, 1)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (2, N'Administration', 0, 2)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (3, N'Administration', 0, 3)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (4, N'Administration', 0, 4)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (5, N'Administration', 0, 5)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (6, N'Administration', 0, 6)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (7, N'Administration', 0, 7)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (8, N'Administration', 0, 8)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (9, N'Project', 0, 9)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (10, N'Project', 0, 10)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (11, N'Project', 0, 11)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (12, N'Project', 0, 12)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (13, N'Facility', 0, 13)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (14, N'Facility', 0, 14)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (15, N'Reports', 0, 15)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (16, N'Reports', 0, 16)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (17, N'Reports', 0, 17)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (18, N'Reports', 0, 18)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (19, N'Administration', 0, 19)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (20, N'Administration', 0, 20)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (21, N'Administration', 0, 21)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (22, N'Administration', 0, 22)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (23, N'Administration', 0, 23)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (24, N'Accounts', 0, 24)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted], [FunctionId]) VALUES (25, N'Administration', 0, 25)
GO
SET IDENTITY_INSERT [dbo].[tblModules] OFF
GO
SET IDENTITY_INSERT [dbo].[tblSchemes] ON 

GO
INSERT [dbo].[tblSchemes] ([SchemeId], [SchemeName], [ProjectType], [SchemeCode], [Status], [CreatedUserId], [Created_TS], [LastUpdatedUsedId], [LastUpdated_TS]) VALUES (1, N'Research Fund', 1, N'code1', N'Active', NULL, NULL, 1, CAST(0x0000A94700CBB3CE AS DateTime))
GO
INSERT [dbo].[tblSchemes] ([SchemeId], [SchemeName], [ProjectType], [SchemeCode], [Status], [CreatedUserId], [Created_TS], [LastUpdatedUsedId], [LastUpdated_TS]) VALUES (2, N'NFSG', 2, N'code2', N'Active', NULL, NULL, 1, CAST(0x0000A9460105B7E7 AS DateTime))
GO
INSERT [dbo].[tblSchemes] ([SchemeId], [SchemeName], [ProjectType], [SchemeCode], [Status], [CreatedUserId], [Created_TS], [LastUpdatedUsedId], [LastUpdated_TS]) VALUES (3, N'Institute funded', 2, N'code3', N'Active', NULL, NULL, 1, CAST(0x0000A9460105C36E AS DateTime))
GO
INSERT [dbo].[tblSchemes] ([SchemeId], [SchemeName], [ProjectType], [SchemeCode], [Status], [CreatedUserId], [Created_TS], [LastUpdatedUsedId], [LastUpdated_TS]) VALUES (4, N'ICSR', 1, N'code4', N'Active', NULL, NULL, 1, CAST(0x0000A94700CDE922 AS DateTime))
GO
INSERT [dbo].[tblSchemes] ([SchemeId], [SchemeName], [ProjectType], [SchemeCode], [Status], [CreatedUserId], [Created_TS], [LastUpdatedUsedId], [LastUpdated_TS]) VALUES (5, N'test', 1, N'test1', N'InActive', NULL, NULL, 1, CAST(0x0000A94601070F0F AS DateTime))
GO
INSERT [dbo].[tblSchemes] ([SchemeId], [SchemeName], [ProjectType], [SchemeCode], [Status], [CreatedUserId], [Created_TS], [LastUpdatedUsedId], [LastUpdated_TS]) VALUES (7, N'sadd', 1, N'asdsad', N'InActive', 1, CAST(0x0000A9460106DC2E AS DateTime), 1, CAST(0x0000A946010707FE AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[tblSchemes] OFF
GO
SET IDENTITY_INSERT [dbo].[tblSRBItemCategory] ON 

GO
INSERT [dbo].[tblSRBItemCategory] ([SRBItemCategotyId], [Category], [Asset_f], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status]) VALUES (1, N'Electronic', 1, NULL, NULL, NULL, NULL, N'Active')
GO
INSERT [dbo].[tblSRBItemCategory] ([SRBItemCategotyId], [Category], [Asset_f], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status]) VALUES (2, N'Stationary', 0, NULL, NULL, NULL, NULL, N'Active')
GO
INSERT [dbo].[tblSRBItemCategory] ([SRBItemCategotyId], [Category], [Asset_f], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status]) VALUES (3, N'Electricals', 1, NULL, NULL, NULL, NULL, N'Active')
GO
INSERT [dbo].[tblSRBItemCategory] ([SRBItemCategotyId], [Category], [Asset_f], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status]) VALUES (4, N'Industrial', 0, NULL, NULL, NULL, NULL, N'Active')
GO
INSERT [dbo].[tblSRBItemCategory] ([SRBItemCategotyId], [Category], [Asset_f], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status]) VALUES (5, N'test', 1, CAST(0x0000A94C00E386AB AS DateTime), CAST(0x0000A94C00E6860B AS DateTime), 1, 1, N'InActive')
GO
INSERT [dbo].[tblSRBItemCategory] ([SRBItemCategotyId], [Category], [Asset_f], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status]) VALUES (6, N'testupdate', 1, CAST(0x0000A94C01291AA2 AS DateTime), CAST(0x0000A94C012AEB87 AS DateTime), 1, 1, N'InActive')
GO
SET IDENTITY_INSERT [dbo].[tblSRBItemCategory] OFF
GO
ALTER TABLE [dbo].[tblModules] ADD  CONSTRAINT [DF_tblModules_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblConsultancyFundingCategory]    Script Date: 8/31/2018 7:20:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblConsultancyFundingCategory](
	[ConsultancyFundingCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[ConsultancyFundingCategory] [nvarchar](200) NULL,
	[CrtdUserId] [int] NULL,
	[CrtdTS] [datetime] NULL,
	[Reason] [nvarchar](max) NULL,
	[Status] [varchar](50) NULL,
	[LastUpdatedUserId] [int] NULL,
	[Lastupdated_TS] [datetime] NULL,
 CONSTRAINT [PK_tblConsultancyFundingCategory] PRIMARY KEY CLUSTERED 
(
	[ConsultancyFundingCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblProjectStaffCategoryMaster]    Script Date: 8/31/2018 7:20:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblProjectStaffCategoryMaster](
	[ProjectStaffCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[ProjectStaffCategory] [nvarchar](max) NULL,
	[CrtdUserId] [int] NULL,
	[CrtdTS] [datetime] NULL,
	[Reason] [nvarchar](max) NULL,
	[LastUpdatedUserid] [int] NULL,
	[LastUpdated_TS] [datetime] NULL,
	[Status] [varchar](50) NULL,
 CONSTRAINT [PK_tblProjectStaffCategoryMaster] PRIMARY KEY CLUSTERED 
(
	[ProjectStaffCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[tblConsultancyFundingCategory] ON 

GO
INSERT [dbo].[tblConsultancyFundingCategory] ([ConsultancyFundingCategoryId], [ConsultancyFundingCategory], [CrtdUserId], [CrtdTS], [Reason], [Status], [LastUpdatedUserId], [Lastupdated_TS]) VALUES (1, N'testfunding1', 1, CAST(0x0000A9450132E262 AS DateTime), NULL, N'InActive', 1, CAST(0x0000A94501332023 AS DateTime))
GO
INSERT [dbo].[tblConsultancyFundingCategory] ([ConsultancyFundingCategoryId], [ConsultancyFundingCategory], [CrtdUserId], [CrtdTS], [Reason], [Status], [LastUpdatedUserId], [Lastupdated_TS]) VALUES (2, N'testfunding', 1, CAST(0x0000A9450132EFD5 AS DateTime), NULL, N'Active', 1, CAST(0x0000A9450132F765 AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[tblConsultancyFundingCategory] OFF
GO
SET IDENTITY_INSERT [dbo].[tblProjectStaffCategoryMaster] ON 

GO
INSERT [dbo].[tblProjectStaffCategoryMaster] ([ProjectStaffCategoryId], [ProjectStaffCategory], [CrtdUserId], [CrtdTS], [Reason], [LastUpdatedUserid], [LastUpdated_TS], [Status]) VALUES (1, N'staff', 1, CAST(0x0000A945012DA01E AS DateTime), NULL, 1, CAST(0x0000A945012DC5AD AS DateTime), N'Active')
GO
INSERT [dbo].[tblProjectStaffCategoryMaster] ([ProjectStaffCategoryId], [ProjectStaffCategory], [CrtdUserId], [CrtdTS], [Reason], [LastUpdatedUserid], [LastUpdated_TS], [Status]) VALUES (2, N'staff', 1, CAST(0x0000A945012DB042 AS DateTime), NULL, 1, CAST(0x0000A945012DB70F AS DateTime), N'InActive')
GO
SET IDENTITY_INSERT [dbo].[tblProjectStaffCategoryMaster] OFF
GO
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--Date for table modfication 04/09/2018
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
/**Drop table list**/
drop table [dbo].[tblModules]
drop table [dbo].[tblRoleaccess]
drop table [dbo].[tblFunction]
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblFunction]    Script Date: 9/4/2018 9:41:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblFunction](
	[FunctionId] [int] NOT NULL,
	[FunctionName] [varchar](50) NULL,
	[ActionName] [varchar](50) NULL,
	[ControllerName] [varchar](50) NULL,
	[ModuleID] [int] NULL,
	[MenuGroupID] [int] NULL,
 CONSTRAINT [PK_tblFunction] PRIMARY KEY CLUSTERED 
(
	[FunctionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblMenuGroup]    Script Date: 9/4/2018 9:41:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblMenuGroup](
	[MenuGroupID] [int] NOT NULL,
	[ModuleID] [int] NULL,
	[MenuGroup] [varchar](250) NULL,
 CONSTRAINT [PK_tblMenuGroup] PRIMARY KEY CLUSTERED 
(
	[MenuGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblModules]    Script Date: 9/4/2018 9:41:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblModules](
	[ModuleID] [int] NOT NULL,
	[ModuleName] [varchar](50) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_tblModules] PRIMARY KEY CLUSTERED 
(
	[ModuleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblRoleaccess]    Script Date: 9/4/2018 9:41:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblRoleaccess](
	[RoleaccessId] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NULL,
	[FunctionId] [int] NULL,
	[Read_f] [bit] NULL,
	[Add_f] [bit] NULL,
	[Delete_f] [bit] NULL,
	[Approve_f] [bit] NULL,
	[Update_f] [bit] NULL,
	[DepartmentId] [int] NULL,
	[Status] [varchar](50) NULL,
 CONSTRAINT [PK_tblRoleaccess] PRIMARY KEY CLUSTERED 
(
	[RoleaccessId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (1, N'Department', N'Department', N'Account', 1, 1)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (2, N'Role', N'Role', N'Account', 1, 1)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (3, N'Access Rights', N'AccessRights', N'Account', 1, 1)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (4, N'Reset Password', N'ResetPassword', N'Account', 1, 1)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (5, N'User Management', N'Createuser', N'Account', 1, 1)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (6, N'Principal Investigator', N'CreatePI', N'Account', 1, 1)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (7, N'Institute', N'Institute', N'Account', 1, 2)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (8, N'Process Guideline', N'ProcessGuideline', N'ProcessGuideline', 1, 1)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (9, N'Proposal', N'CreateProposal', N'Proposal', 2, 3)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (10, N'Project opening', N'ProjectOpening', N'Project', 2, 3)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (11, N'Project Enhancement', N'ProjectEnhancement', N'Project', 2, 3)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (12, N'Project Extension', N'ProjectExtension', N'Project', 2, 3)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (13, N'SRB', N'SRBList', N'Facility', 3, 4)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (14, N'Tapal', N'Tapal', N'Facility', 3, 5)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (15, N'Builder', N'List', N'Reports', 4, 6)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (16, N'Viewer', N'ReportViewer', N'Reports', 4, 6)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (17, N'Project Proposal', N'ProjectProposal', N'CrystalReport', 4, 6)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (18, N'Project Report', N'Projectreport', N'CrystalReport', 4, 6)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (19, N'Agency Management', N'Createagency', N'Account', 1, 2)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (20, N'Allocation Head', N'AllocationHead', N'Account', 1, 2)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (21, N'Project Staff Category', N'Projectstaff', N'Account', 2, 2)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (22, N'Consultancy Funding Category', N'Consultancyfundingcategory', N'Account', 2, 2)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (23, N'Scheme', N'Schemes', N'Account', 2, 2)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (24, N'Account Group', N'AccountGroup', N'Account', 5, 7)
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (25, N'SRB Item Category', N'SRBItemcategory', N'Account', 3, 2)
GO
INSERT [dbo].[tblMenuGroup] ([MenuGroupID], [ModuleID], [MenuGroup]) VALUES (1, 1, N'Access Management')
GO
INSERT [dbo].[tblMenuGroup] ([MenuGroupID], [ModuleID], [MenuGroup]) VALUES (2, 1, N'Master')
GO
INSERT [dbo].[tblMenuGroup] ([MenuGroupID], [ModuleID], [MenuGroup]) VALUES (3, 2, N'Project Management')
GO
INSERT [dbo].[tblMenuGroup] ([MenuGroupID], [ModuleID], [MenuGroup]) VALUES (4, 3, N'SRB')
GO
INSERT [dbo].[tblMenuGroup] ([MenuGroupID], [ModuleID], [MenuGroup]) VALUES (5, 3, N'Tapal Managment')
GO
INSERT [dbo].[tblMenuGroup] ([MenuGroupID], [ModuleID], [MenuGroup]) VALUES (6, 4, N'Administration')
GO
INSERT [dbo].[tblMenuGroup] ([MenuGroupID], [ModuleID], [MenuGroup]) VALUES (7, 5, N'AccountGroup')
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted]) VALUES (1, N'Administration', 0)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted]) VALUES (2, N'Project', 0)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted]) VALUES (3, N'Facility', 0)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted]) VALUES (4, N'Reports', 0)
GO
INSERT [dbo].[tblModules] ([ModuleID], [ModuleName], [IsDeleted]) VALUES (5, N'Accounts', 0)
GO
SET IDENTITY_INSERT [dbo].[tblRoleaccess] ON 

GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (2, 1, 5, 1, 1, 1, 1, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (11, 2, 14, 1, 1, 1, 1, 0, 2, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (12, 3, 14, 0, 0, 0, 0, 0, 2, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (13, 2, 13, 1, 1, 1, 1, 0, 2, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (14, 3, 13, 1, 0, 0, 0, 0, 2, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (15, 1, 15, 1, 1, 1, 1, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (16, 1, 16, 1, 1, 1, 1, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (17, 1, 1002, 1, 1, 0, 1, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (18, 1, 20, 1, 1, 1, 1, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (19, 1, 21, 1, 1, 1, 1, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (20, 1, 19, 1, 1, 0, 1, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (21, 1, 22, 1, 1, 0, 0, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (22, 1, 10, 1, 0, 0, 0, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (23, 1, 6, 1, 0, 0, 0, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (24, 1, 7, 1, 0, 0, 0, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (25, 1, 23, 1, 1, 0, 0, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (26, 1, 4, 1, 1, 0, 0, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (27, 1, 8, 1, 0, 0, 0, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (28, 1, 25, 1, 0, 0, 0, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (29, 1, 2, 1, 1, 0, 0, 0, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (30, 1, 3, 1, 0, 0, 0, 1, 1, N'Active')
GO
INSERT [dbo].[tblRoleaccess] ([RoleaccessId], [RoleId], [FunctionId], [Read_f], [Add_f], [Delete_f], [Approve_f], [Update_f], [DepartmentId], [Status]) VALUES (31, 1, 24, 1, 1, 0, 0, 0, 1, N'Active')
GO
SET IDENTITY_INSERT [dbo].[tblRoleaccess] OFF
GO
ALTER TABLE [dbo].[tblModules] ADD  CONSTRAINT [DF_tblModules_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
---------------------------------------------------------------------------------------------------------------------------
--Date for table modfication 08/09/2018
---------------------------------------------------------------------------------------------------------------------------
USE [IOASDB]
GO
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (26, N'Project Sanction Report', N'Sanctionreport', N'ProjectReport', 2, 8)
GO
INSERT [dbo].[tblMenuGroup] ([MenuGroupID], [ModuleID], [MenuGroup]) VALUES (8, 2, N'Report')
GO

----------------------------------------------------------------------------------------------------------
USE [IOASDB]
GO
SET IDENTITY_INSERT [dbo].[tblCodeControl] ON 

GO
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (49, N'ReportGroup', 1, N'Faculty', N'Faculty', NULL, NULL, NULL)
GO
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (54, N'ReportGroup', 2, N'Department', N'Department', NULL, NULL, NULL)
GO
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (55, N'ReportGroup', 3, N'Agency', N'Agency', NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[tblCodeControl] OFF
GO
-------------------------------------------------------------------------------------------------------------------
--Date for table modfication 17/09/2018
------------------------------------------------------------------------------------------------------------------
INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID]) VALUES (27, N'New Proposal Funding ', N'Newproposalfunding', N'ProposalReport', 2, 8)


-------------------------------------------------------------------------------------------------------------------
--Date for table modfication 24/09/2018
------------------------------------------------------------------------------------------------------------------

drop table tblGender
alter table tblUser drop column Role

INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (61, N'Gender', 1, N'Male', N'Gender', NULL, NULL, NULL)
GO
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (62, N'Gender', 2, N'Female', N'Gender', NULL, NULL, NULL)
GO

alter table tblAccountGroup add Is_Subgroup bit 

alter table tblAccountGroup add SeqNbr int

alter table tblAccountGroup add Status varchar(100)

alter table tblAccountGroup add Is_Subgroup bit

ALTER TABLE dbo.tblAccountGroup 
ALTER COLUMN AccountGroupCode varchar(100) 