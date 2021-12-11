

USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblCodeControl]    Script Date: 08/09/2018 11:38:13 ******/
SET IDENTITY_INSERT [dbo].[tblCodeControl] ON
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (51, N'ProjectStatus', 1, N'Active', N'Active', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (53, N'ProjectStatus', 2, N'InActive', N'InActive', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (54, N'ProjectStatus', 3, N'Completed', N'Completed', NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[tblCodeControl] OFF

USE [IOASDB]
GO

/****** Object:  Table [dbo].[tblProjectStatusLog]    Script Date: 08/09/2018 11:08:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblProjectStatusLog](
	[ProjectStatusLogId] [int] IDENTITY(1,1) NOT NULL,
	[ProjectId] [int] NULL,
	[FromStatus] [int] NULL,
	[ToStatus] [int] NULL,
	[UpdtdUserId] [int] NULL,
	[UpdtdTS] [datetime] NULL,
	[IsCurrentStatus] [bit] NULL,
 CONSTRAINT [PK_tblProjectStatusLog] PRIMARY KEY CLUSTERED 
(
	[ProjectStatusLogId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


GO

  alter table [tblProjectStatusLog]
  add Remarks varchar(500),DocumentName varchar(200)

GO
/****** Object:  Table [dbo].[tblEvent]   Script Date: 10/09/2018  ******/

CREATE TABLE [dbo].[tblEvent](
	[EventId] [int] IDENTITY(1,1) NOT NULL,
	[EventTitle] [varchar](1000) NULL,
	[EventStart] [datetime] NULL,
	[EventEnd] [datetime] NULL,
	[Url] [varchar](250) NULL,
	[allDay] [bit] NULL,
	[Start] [varchar](50) NULL,
	[End] [varchar](50) NULL,
	CreatedTS [datetime] NULL,
	LastUpdatedTS [datetime] NULL,
	CreatedUserId [int] NULL,
	LastUpdatedUserId [int] NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_tblEvent] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/************ Date:06-09-2018  ****************/
GO
--Drop first Tapal table after Insert
DROP TABLE [dbo].[tblTapal]
GO

CREATE TABLE [dbo].[tblTapal](
	[TapalId] [int] IDENTITY(1,1) NOT NULL,
	[TapalType] [int] NULL,
	[SenderDetail] [varchar](max) NULL,
	[ProjectTabal] [bit] NULL,
	[PIName] [int] NULL,
	[ProjectNumber] [int] NULL,
	[IsInward] [bit] NULL,
	[CreatedTS] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	[LastUpdatedTS] [datetime] NULL,
	[LastUpdatedUserId] [int] NULL,
	[IsClosed] [bit] NULL,
	[TapalNo] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[TapalId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[tblTapalWorkflow](
	[TapalWorkflowId] [int] IDENTITY(1,1) NOT NULL,
	[TapalId] [int] NULL,
	[DateTimeReceipt] [datetime] NULL,
	[InwardDateTime] [datetime] NULL,
	[MarkTo] [int] NULL,
	[Role] [int] NULL,
	[UserId] [int] NULL,
	[Comments] [varchar](max) NULL,
	[OutwardDateTime] [datetime] NULL,
	[CreatedTS] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	[Is_Active] [bit] NULL,
	[TapalAction] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[TapalWorkflowId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[tblCodeControl] ON
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (55, N'TapalAction', 1, N'Forward', N'Forward', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (59, N'TapalAction', 2, N'Recommend', N'Recommend', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (61, N'TapalAction', 3, N'Accept', N'Accept', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (62, N'TapalAction', 4, N'Not Relavant', N'Not Relavant', NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[tblCodeControl] OFF
GO
/************ Date:07-09-2018  ****************/
Go
CREATE TABLE [dbo].[tblFinancialYear](
	[FinancialYearId] [int] IDENTITY(1,1) NOT NULL,
	[FinancialYear] [varchar](5) NULL,
	[Status] [varchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[FinancialYearId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[tblFinancialYear] ON
INSERT [dbo].[tblFinancialYear] ([FinancialYearId], [FinancialYear], [Status]) VALUES (4, N'1819', N'Active')
SET IDENTITY_INSERT [dbo].[tblFinancialYear] OFF
GO
