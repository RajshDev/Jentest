Drop table [dbo].[tblAgencyMaster]
drop table tblAllocationHead
drop table [dbo].[tblAccountGroup]

insert into tblCodeControl (CodeName,CodeValAbbr,CodeValDetail,CodeDescription)values('AgencyCountry',1,'India','AgencyCountry')

insert into tblCodeControl(CodeName,CodeValAbbr,CodeValDetail,CodeDescription)values('AgencyCountry',2,'Foreign','AgencyCountry')

insert into tblCodeControl(CodeName,CodeValAbbr,CodeValDetail,CodeDescription)values('IndianAgencyCategory',1,'SEZ','IndianAgencyCategory')

insert into tblCodeControl(CodeName,CodeValAbbr,CodeValDetail,CodeDescription)values('IndianAgencyCategory',2,'NON SEZ','IndianAgencyCategory')

insert into tblCodeControl(CodeName,CodeValAbbr,CodeValDetail,CodeDescription)values('NonSEZCategory',1,'GST Registered','NonSEZCategory')

insert into tblCodeControl(CodeName,CodeValAbbr,CodeValDetail,CodeDescription)values('NonSEZCategory',2,'GST Un Registered','NonSEZCategory')

insert into tblDocument(DocumentName)values('Tax Certificate')

insert into tblDocument(DocumentName)values('Registration')

USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblAccountGroup]    Script Date: 10/13/2018 12:02:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAccountGroup](
	[AccountGroupId] [int] IDENTITY(1,1) NOT NULL,
	[AccountGroup] [nvarchar](max) NULL,
	[AccountType] [int] NULL,
	[AccountGroupCode] [nvarchar](max) NULL,
	[CreatedTS] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	[LastUpdatedTS] [datetime] NULL,
	[LastUpdatedUserId] [int] NULL,
	[SeqNbr] [int] NULL,
	[Status] [varchar](100) NULL,
	[Is_Subgroup] [bit] NULL,
	[ParentgroupId] [int] NULL,
 CONSTRAINT [PK_tblAccountGroup] PRIMARY KEY CLUSTERED 
(
	[AccountGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblAccountHead]    Script Date: 10/13/2018 12:02:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAccountHead](
	[AccountHeadId] [int] IDENTITY(1,1) NOT NULL,
	[AccountGroupId] [int] NULL,
	[AccountHead] [nvarchar](max) NULL,
	[AccountHeadCode] [nvarchar](max) NULL,
	[CrtdUserId] [int] NULL,
	[CrtdTS] [datetime] NULL,
	[SeqNbr] [int] NULL,
	[LastUpdatedUserId] [int] NULL,
	[Lastupdated_TS] [datetime] NULL,
	[Status] [varchar](50) NULL,
 CONSTRAINT [PK_tblAllocationHeadsMaster] PRIMARY KEY CLUSTERED 
(
	[AccountHeadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblAgencyDocument]    Script Date: 10/13/2018 12:02:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblAgencyDocument](
	[AgencyDocumentId] [int] IDENTITY(1,1) NOT NULL,
	[AgencyId] [int] NULL,
	[AgencyDocument] [nvarchar](max) NULL,
	[AttachmentPath] [nvarchar](max) NULL,
	[AttachmentName] [nvarchar](max) NULL,
	[DocumentType] [int] NULL,
	[IsCurrentVersion] [bit] NULL,
	[DocumentUploadUserId] [int] NULL,
	[DocumentUpload_Ts] [datetime] NULL,
 CONSTRAINT [PK_tblAgencyDocument] PRIMARY KEY CLUSTERED 
(
	[AgencyDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblAgencyMaster]    Script Date: 10/13/2018 12:02:23 PM ******/
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
	[StateId] [int] NULL,
	[StateCode] [varchar](5) NULL,
	[BankName] [nvarchar](max) NULL,
	[AccountNumber] [nvarchar](max) NULL,
	[BranchName] [nvarchar](max) NULL,
	[SwiftCode] [nvarchar](max) NULL,
	[MICRCode] [nvarchar](max) NULL,
	[IFSCCode] [nvarchar](max) NULL,
	[BankAddress] [nvarchar](max) NULL,
	[District] [nvarchar](max) NULL,
	[PinCode] [int] NULL,
	[AgencyCountryCategoryId] [int] NULL,
	[IndianAgencyCategoryId] [int] NULL,
	[NonSezCategoryId] [int] NULL,
	[AgencyRegisterName] [nvarchar](max) NULL,
	[AgencyRegisterAddress] [nvarchar](max) NULL,
	[SeqNbr] [int] NULL,
 CONSTRAINT [PK_tblAgencyMaster] PRIMARY KEY CLUSTERED 
(
	[AgencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblStateMaster]    Script Date: 10/13/2018 12:02:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblStateMaster](
	[StateId] [int] IDENTITY(1,1) NOT NULL,
	[StateName] [nvarchar](max) NULL,
	[StateCode] [nvarchar](max) NULL,
 CONSTRAINT [PK_tblStateMaster] PRIMARY KEY CLUSTERED 
(
	[StateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[tblAccountGroup] ON 

GO
INSERT [dbo].[tblAccountGroup] ([AccountGroupId], [AccountGroup], [AccountType], [AccountGroupCode], [CreatedTS], [CreatedUserId], [LastUpdatedTS], [LastUpdatedUserId], [SeqNbr], [Status], [Is_Subgroup], [ParentgroupId]) VALUES (1, N'Accoutgroup', 4, N'A01', CAST(0x0000A973013DC72F AS DateTime), 1, NULL, NULL, 1, N'Active', 0, NULL)
GO
SET IDENTITY_INSERT [dbo].[tblAccountGroup] OFF
GO
SET IDENTITY_INSERT [dbo].[tblAccountHead] ON 

GO
INSERT [dbo].[tblAccountHead] ([AccountHeadId], [AccountGroupId], [AccountHead], [AccountHeadCode], [CrtdUserId], [CrtdTS], [SeqNbr], [LastUpdatedUserId], [Lastupdated_TS], [Status]) VALUES (1, 1, N'Account1', N'A01-1111', 1, CAST(0x0000A97400EC3854 AS DateTime), 1, NULL, NULL, N'Active')
GO
SET IDENTITY_INSERT [dbo].[tblAccountHead] OFF
GO
SET IDENTITY_INSERT [dbo].[tblAgencyDocument] ON 

GO
INSERT [dbo].[tblAgencyDocument] ([AgencyDocumentId], [AgencyId], [AgencyDocument], [AttachmentPath], [AttachmentName], [DocumentType], [IsCurrentVersion], [DocumentUploadUserId], [DocumentUpload_Ts]) VALUES (3, 2, N'c1410a4a-e25c-4f1c-846b-f050747abceb_UAY17_IITM_031P.pdf', N'9e316658-e96c-4754-af89-f792bcd4aa7c_c1410a4a-e25c-4f1c-846b-f050747abceb_UAY17_IITM_031P.pdf', N'Supporting document', 7, 1, 1, CAST(0x0000A97800B0A773 AS DateTime))
GO
INSERT [dbo].[tblAgencyDocument] ([AgencyDocumentId], [AgencyId], [AgencyDocument], [AttachmentPath], [AttachmentName], [DocumentType], [IsCurrentVersion], [DocumentUploadUserId], [DocumentUpload_Ts]) VALUES (4, 2, N'GFR12A (1).pdf', N'24f4b8fa-b428-40bf-bb4a-801249b79ccc_GFR12A (1).pdf', N'tax supporting', 10, 0, 1, CAST(0x0000A97800B069EA AS DateTime))
GO
INSERT [dbo].[tblAgencyDocument] ([AgencyDocumentId], [AgencyId], [AgencyDocument], [AttachmentPath], [AttachmentName], [DocumentType], [IsCurrentVersion], [DocumentUploadUserId], [DocumentUpload_Ts]) VALUES (5, 4, N'c1410a4a-e25c-4f1c-846b-f050747abceb_UAY17_IITM_031P.pdf', N'c39e5999-b7e3-489d-a2cb-43a607605c43_c1410a4a-e25c-4f1c-846b-f050747abceb_UAY17_IITM_031P.pdf', N'support descing', 7, 1, 1, CAST(0x0000A97800B5AD7D AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[tblAgencyDocument] OFF
GO
SET IDENTITY_INSERT [dbo].[tblAgencyMaster] ON 

GO
INSERT [dbo].[tblAgencyMaster] ([AgencyId], [AgencyName], [ContactPerson], [ContactNumber], [ContactEmail], [Address], [State], [Country], [Crtd_TS], [Crtd_UserId], [LastupdatedUserid], [Lastupdate_TS], [Reason], [AgencyCode], [AgencyType], [Scheme], [Status], [GSTIN], [TAN], [PAN], [StateId], [StateCode], [BankName], [AccountNumber], [BranchName], [SwiftCode], [MICRCode], [IFSCCode], [BankAddress], [District], [PinCode], [AgencyCountryCategoryId], [IndianAgencyCategoryId], [NonSezCategoryId], [AgencyRegisterName], [AgencyRegisterAddress], [SeqNbr]) VALUES (2, N'ABC & Co', N'ABC company', N'314221341', N'abc@gmail.com', N'tnager', NULL, 128, CAST(0x0000A97800AEF0AE AS DateTime), 1, 1, CAST(0x0000A97800B09CA8 AS DateTime), NULL, N'A01', 2, NULL, N'Active', NULL, N'DELA99999B', N'AAAPL1234C', 33, N'33', NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'chennai', 6000026, 1, 1, 0, N'ABC Co', N'chennai', 1)
GO
INSERT [dbo].[tblAgencyMaster] ([AgencyId], [AgencyName], [ContactPerson], [ContactNumber], [ContactEmail], [Address], [State], [Country], [Crtd_TS], [Crtd_UserId], [LastupdatedUserid], [Lastupdate_TS], [Reason], [AgencyCode], [AgencyType], [Scheme], [Status], [GSTIN], [TAN], [PAN], [StateId], [StateCode], [BankName], [AccountNumber], [BranchName], [SwiftCode], [MICRCode], [IFSCCode], [BankAddress], [District], [PinCode], [AgencyCountryCategoryId], [IndianAgencyCategoryId], [NonSezCategoryId], [AgencyRegisterName], [AgencyRegisterAddress], [SeqNbr]) VALUES (4, N'Agencyforeign', N'ageny person', N'1332314', N'agy@gmail.com', N'foreignaddress', N'dubai main road', 124, CAST(0x0000A97800B59E64 AS DateTime), 1, NULL, NULL, NULL, N'A02', 2, NULL, N'Active', NULL, NULL, NULL, 0, NULL, N'axis bank', N'5632432455', N'dfsdjfb', N'hssbdhbshd', N'hshdh', N'vhvdvjhd', N'hhjsdh', NULL, NULL, 2, 0, 0, N'agency registername', N'Agency register address', 2)
GO
SET IDENTITY_INSERT [dbo].[tblAgencyMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[tblStateMaster] ON 

GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (1, N'JAMMU AND KASHMIR', N'01')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (2, N'HIMACHAL PRADESH

', N'02')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (3, N'PUNJAB', N'03')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (4, N'CHANDIGARH', N'04')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (5, N'UTTARAKHAND', N'05')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (6, N'HARYANA', N'06')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (7, N'DELHI', N'07')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (8, N'RAJASTHAN', N'08')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (9, N'UTTAR  PRADESH', N'09')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (10, N'BIHAR', N'10')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (11, N'SIKKIM', N'11')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (12, N'ARUNACHAL PRADESH', N'12')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (13, N'NAGALAND', N'13')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (14, N'MANIPUR', N'14')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (15, N'MIZORAM', N'15')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (16, N'TRIPURA', N'16')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (17, N'MEGHLAYA', N'17')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (18, N'ASSAM', N'18')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (19, N'WEST BENGAL', N'19')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (20, N'JHARKHAND', N'20')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (21, N'ODISHA', N'21')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (22, N'CHATTISGARH', N'22')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (23, N'MADHYA PRADESH', N'23')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (24, N'GUJARAT', N'24')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (25, N'DAMAN AND DIU', N'25')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (26, N'DADRA AND NAGAR HAVELI', N'26')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (27, N'MAHARASHTRA', N'27')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (28, N'ANDHRA PRADESH(Before Division)', N'28')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (29, N'KARNATAKA', N'29')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (30, N'GOA', N'30')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (31, N'LAKSHWADEEP', N'31')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (32, N'KERALA', N'32')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (33, N'TAMIL NADU', N'33')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (34, N'PUDUCHERRY', N'34')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (35, N'ANDAMAN AND NICOBAR ISLANDS', N'35')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (36, N'TELANGANA', N'36')
GO
INSERT [dbo].[tblStateMaster] ([StateId], [StateName], [StateCode]) VALUES (37, N'ANDHRA PRADESH (NEW)', N'37')
GO
SET IDENTITY_INSERT [dbo].[tblStateMaster] OFF
GO

alter table [dbo].[tblFunction] Add MenuSeq int

update tblFunction set MenuSeq = FunctionId