alter table tblTravelBill
add CommitmentId	int	

alter table tblBOADetail
add Payment_f bit default 0

alter table tblTravelBill
add DeductionType	varchar(max),EligibilityCheck_f bit default 0

alter table dbo.tblDeductionHead
add EligibilityCheck_f bit default 0, DeductionType varchar(max)

GO


/****** Object:  Table [dbo].[tblBillTDSDetail]    Script Date: 11/22/2018 19:03:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblBillTDSDetail](
	[BillTDSDetailId] [int] IDENTITY(1,1) NOT NULL,
	[VendorTDSDetailId] [int] NULL,
	[BillId] [int] NULL,
	[CRTD_By] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[Status] [varchar](max) NULL,
 CONSTRAINT [PK_tblBillTDSDetailId] PRIMARY KEY CLUSTERED 
(
	[BillTDSDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


GO

/****** Object:  Table [dbo].[tblTransactionDefinition]    Script Date: 11/22/2018 19:08:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblTransactionDefinition](
	[TransactionDefinitionId] [int] IDENTITY(1,1) NOT NULL,
	[TransactionTypeCode] [varchar](max) NULL,
	[SubCode] [varchar](max) NULL,
	[AccountGroupId] [int] NULL,
	[AccountHeadId] [int] NULL,
	[TransactionType] [varchar](max) NULL,
	[IsJV_f] [bit] NULL,
 CONSTRAINT [PK_tblTransactionDefinition] PRIMARY KEY CLUSTERED 
(
	[TransactionDefinitionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[tblTransactionDefinition] ADD  CONSTRAINT [DF_tblTransactionDefinition_IsJV_f]  DEFAULT ((0)) FOR [IsJV_f]
GO






/****** Object:  Table [dbo].[tblVendorTDSDetail]    Script Date: 11/22/2018 18:53:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblVendorTDSDetail](
	[VendorTDSDetailId] [int] IDENTITY(1,1) NOT NULL,
	[VendorId] [int] NULL,
	[Section] [varchar](max) NULL,
	[NatureOfIncome] [varchar](max) NULL,
	[TDSPercentage] [decimal](18, 2) NULL,
 CONSTRAINT [PK_tblVendorTDSDetail] PRIMARY KEY CLUSTERED 
(
	[VendorTDSDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
