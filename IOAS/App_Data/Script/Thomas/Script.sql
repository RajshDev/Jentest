update tblTapal set ProjectNumber = null
alter table tblTapal
alter column ProjectNumber int  


GO

/****** Object:  Table [dbo].[tblLoginDetails]    Script Date: 07/27/2018 18:50:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblLoginDetails](
	[LoginDetailId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[LoginTime] [datetime] NULL,
 CONSTRAINT [PK_LoginDetails] PRIMARY KEY CLUSTERED 
(
	[LoginDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

GO

/****** Object:  Table [dbo].[tblModules]    Script Date: 08/01/2018 18:04:28 ******/
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
 CONSTRAINT [PK_tblModules] PRIMARY KEY CLUSTERED 
(
	[ModuleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[tblModules] ADD  CONSTRAINT [DF_tblModules_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO



/****** Object:  Table [dbo].[tblNotification]    Script Date: 08/02/2018 16:23:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblNotification](
	[NotificationId] [int] IDENTITY(1,1) NOT NULL,
	[NotificationType] [varchar](250) NULL,
	[FromUserId] [int] NULL,
	[ToUserId] [int] NULL,
	[Subject] [varchar](500) NULL,
	[Description] [varchar](max) NULL,
	[FunctionURL] [varchar](200) NULL,
	[ReferenceId] [int] NOT NULL,
	[Crt_Ts] [datetime] NULL,
	[Crt_By] [int] NULL,
 CONSTRAINT [PK_tblNotification] PRIMARY KEY CLUSTERED 
(
	[NotificationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

alter table tblNotification
add IsDeleted bit DEFAULT 0;

alter table dbo.tblProposalCoPI
alter column Name int not null

alter table tblSRB
add SupplierName varchar(250),DepartmentId int,InvoiceNumber varchar(50),SRBNumber varchar(50)

GO

/****** Object:  Table [dbo].[tblSRBDetails]    Script Date: 08/28/2018 17:23:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblSRBDetails](
	[tblSRBDetailsId] [int] IDENTITY(1,1) NOT NULL,
	[SRBId] [int] NULL,
	[ItemName] [varchar](300) NULL,
	[ItemCategoryId] [int] NULL,
	[Quantity] [int] NULL,
 CONSTRAINT [PK_tblSRBDetails] PRIMARY KEY CLUSTERED 
(
	[tblSRBDetailsId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


alter table dbo.tblSRBDetails
add ItemValue decimal(18, 3), ItemNumber varchar(50), UOM int, Comments varchar(3000),Status varchar(50),Asset_f bit default false,CRTD_TS datetime,UPDT_TS datetime,CRTD_UserID int, UPDT_UserID int

alter table dbo.tblSRB
add NetTotalAmount decimal(18, 3)

alter table tblSRB
add IsIncludeProjectDetails bit default false, SupplierName varchar(250), DepartmentId int, InvoiceNumber varchar(50), SRBNumber varchar(50), Status varchar(50)


GO
/****** Object:  Table [dbo].[tblSRBItemCategory]    Script Date: 09/12/2018 16:21:35 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblSRBDeactivation]    Script Date: 09/12/2018 16:21:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblSRBDeactivation](
	[SRBDeactivationId] [int] IDENTITY(1,1) NOT NULL,
	[tblSRBDetailId] [int] NULL,
	[ItemName] [varchar](300) NULL,
	[ItemNumber] [varchar](50) NULL,
	[Buyback_f] [bit] NULL,
	[BuybackValue] [decimal](18, 3) NULL,
	[Comments] [varchar](3000) NULL,
	[SRBId] [int] NULL,
	[Attachment] [varchar](500) NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_UserID] [int] NULL,
	[UPDT_UserID] [int] NULL,
	[BuybackRefId] [int] NULL,
 CONSTRAINT [PK_tblSRBDeactivation] PRIMARY KEY CLUSTERED 
(
	[SRBDeactivationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Default [DF_tblSRBDeactivation_Buyback_f]    Script Date: 09/12/2018 16:21:35 ******/
ALTER TABLE [dbo].[tblSRBDeactivation] ADD  CONSTRAINT [DF_tblSRBDeactivation_Buyback_f]  DEFAULT ((0)) FOR [Buyback_f]
GO
