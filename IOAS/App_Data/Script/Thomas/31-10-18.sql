USE [IOASDB]
GO

/****** Object:  Table [dbo].[tblTaxMaster]    Script Date: 10/19/2018 12:42:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblTaxMaster](
	[TaxMasterId] [int] IDENTITY(1,1) NOT NULL,
	[Service_f] [bit] NULL,
	[ServiceType] [nvarchar](max) NULL,
	[TaxCode] [nvarchar](50) NULL,
	[TaxRate] [decimal](18, 2) NULL,
 CONSTRAINT [PK_tblTaxMaster] PRIMARY KEY CLUSTERED 
(
	[TaxMasterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[tblTaxMaster] ADD  CONSTRAINT [DF_tblTaxMaster_Service_f]  DEFAULT ((0)) FOR [Service_f]
GO


CREATE TABLE tblCommitmentDetails(
 ComitmentDetailId int,
 CommitmentId int,
 AllocationHeadId int,
 Amount decimal(18,3)
 )
 
 --alter table [tblCommitment]
 -- drop column [BudgetHeadId]
  
 -- alter table tblCommitment add CommitmentBalance decimal(18, 2)
  GO

/****** Object:  Table [dbo].[tblCommitment]    Script Date: 10/31/2018 17:35:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblCommitment](
	[CommitmentId] [int] IDENTITY(1,1) NOT NULL,
	[CommitmentType] [int] NULL,
	[CommitmentNumber] [varchar](50) NULL,
	[ProjectId] [int] NULL,
	[PurchaseOrder] [varchar](50) NULL,
	[VendorName] [varchar](200) NULL,
	[ItemDescription] [varchar](max) NULL,
	[CommitmentAmount] [decimal](18, 2) NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_UserID] [int] NULL,
	[UPDT_UserID] [int] NULL,
	[Status] [varchar](50) NULL,
	[CommitmentBalance] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[CommitmentId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



  
  USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblTransactionTypeCode]    Script Date: 10/20/2018 12:25:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTransactionTypeCode](
	[TransactionTypeCodeId] [int] IDENTITY(1,1) NOT NULL,
	[TransactionType] [varchar](max) NULL,
	[TransactionTypeCode] [varchar](max) NULL,
	[EntryTypeId] [int] NULL,
	[Functionid] [int] NULL,
 CONSTRAINT [PK_tblTransactionTypeCode] PRIMARY KEY CLUSTERED 
(
	[TransactionTypeCodeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[tblTransactionTypeCode] ON
INSERT [dbo].[tblTransactionTypeCode] ([TransactionTypeCodeId], [TransactionType], [TransactionTypeCode], [EntryTypeId], [Functionid]) VALUES (1, N'BillEntryTransaction', N'ADV', 1, 0)
INSERT [dbo].[tblTransactionTypeCode] ([TransactionTypeCodeId], [TransactionType], [TransactionTypeCode], [EntryTypeId], [Functionid]) VALUES (2, N'BillEntryPartWithdrawal', N'BPW', 1, 0)
INSERT [dbo].[tblTransactionTypeCode] ([TransactionTypeCodeId], [TransactionType], [TransactionTypeCode], [EntryTypeId], [Functionid]) VALUES (3, N'BillEntrySettlement', N'BPS', 1, 0)
INSERT [dbo].[tblTransactionTypeCode] ([TransactionTypeCodeId], [TransactionType], [TransactionTypeCode], [EntryTypeId], [Functionid]) VALUES (4, N'BillPayment', N'PMT', 2, 0)
SET IDENTITY_INSERT [dbo].[tblTransactionTypeCode] OFF
/****** Object:  Table [dbo].[tblGroupMapping]    Script Date: 10/20/2018 12:25:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblGroupMapping](
	[GroupMappingId] [int] IDENTITY(1,1) NOT NULL,
	[BudgetHeadId] [int] NULL,
	[AccountGroupId] [int] NULL,
 CONSTRAINT [PK_tblGroupMapping] PRIMARY KEY CLUSTERED 
(
	[GroupMappingId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[tblGroupMapping] ON
INSERT [dbo].[tblGroupMapping] ([GroupMappingId], [BudgetHeadId], [AccountGroupId]) VALUES (1, 1, 11)
INSERT [dbo].[tblGroupMapping] ([GroupMappingId], [BudgetHeadId], [AccountGroupId]) VALUES (2, 2, 4)
INSERT [dbo].[tblGroupMapping] ([GroupMappingId], [BudgetHeadId], [AccountGroupId]) VALUES (3, 3, 3)
INSERT [dbo].[tblGroupMapping] ([GroupMappingId], [BudgetHeadId], [AccountGroupId]) VALUES (4, 4, 7)
INSERT [dbo].[tblGroupMapping] ([GroupMappingId], [BudgetHeadId], [AccountGroupId]) VALUES (5, 6, 5)
INSERT [dbo].[tblGroupMapping] ([GroupMappingId], [BudgetHeadId], [AccountGroupId]) VALUES (6, 7, 1)
SET IDENTITY_INSERT [dbo].[tblGroupMapping] OFF
GO
/****** Object:  Table [dbo].[tblDeductionHead]    Script Date: 10/31/2018 17:54:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDeductionHead](
	[DeductionHeadId] [int] IDENTITY(1,1) NOT NULL,
	[AccountHeadId] [int] NULL,
	[TransactionTypeCode] [varchar](max) NULL,
	[EligibleForOffset_f] [bit] NULL,
	[DeductionCategoryId] [int] NULL,
	[Interstate_f] [bit] NULL,
 CONSTRAINT [PK_tblDeductionHead] PRIMARY KEY CLUSTERED 
(
	[DeductionHeadId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[tblDeductionHead] ON
INSERT [dbo].[tblDeductionHead] ([DeductionHeadId], [AccountHeadId], [TransactionTypeCode], [EligibleForOffset_f], [DeductionCategoryId], [Interstate_f]) VALUES (1, 9, N'ADV', 0, 1, 0)
INSERT [dbo].[tblDeductionHead] ([DeductionHeadId], [AccountHeadId], [TransactionTypeCode], [EligibleForOffset_f], [DeductionCategoryId], [Interstate_f]) VALUES (2, 10, N'ADV', 0, 1, 1)
INSERT [dbo].[tblDeductionHead] ([DeductionHeadId], [AccountHeadId], [TransactionTypeCode], [EligibleForOffset_f], [DeductionCategoryId], [Interstate_f]) VALUES (3, 11, N'ADV', 0, 1, 0)
INSERT [dbo].[tblDeductionHead] ([DeductionHeadId], [AccountHeadId], [TransactionTypeCode], [EligibleForOffset_f], [DeductionCategoryId], [Interstate_f]) VALUES (4, 15, N'ADV', 0, 2, 0)
INSERT [dbo].[tblDeductionHead] ([DeductionHeadId], [AccountHeadId], [TransactionTypeCode], [EligibleForOffset_f], [DeductionCategoryId], [Interstate_f]) VALUES (5, 16, N'ADV', 0, 2, 1)
INSERT [dbo].[tblDeductionHead] ([DeductionHeadId], [AccountHeadId], [TransactionTypeCode], [EligibleForOffset_f], [DeductionCategoryId], [Interstate_f]) VALUES (6, 17, N'ADV', 0, 2, 0)
SET IDENTITY_INSERT [dbo].[tblDeductionHead] OFF
/****** Object:  Default [DF_tblDeductionHead_EligibleForOffset_f]    Script Date: 10/31/2018 17:54:20 ******/
ALTER TABLE [dbo].[tblDeductionHead] ADD  CONSTRAINT [DF_tblDeductionHead_EligibleForOffset_f]  DEFAULT ((0)) FOR [EligibleForOffset_f]
GO
/****** Object:  Default [DF_tblDeductionHead_Interstate_f]    Script Date: 10/31/2018 17:54:20 ******/
ALTER TABLE [dbo].[tblDeductionHead] ADD  CONSTRAINT [DF_tblDeductionHead_Interstate_f]  DEFAULT ((0)) FOR [Interstate_f]
GO

GO
/****** Object:  Table [dbo].[tblBOATransaction]    Script Date: 10/31/2018 17:22:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBOATransaction](
	[BOATransactionId] [int] IDENTITY(1,1) NOT NULL,
	[BOAId] [int] NULL,
	[AccountHeadId] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
	[TransactionType] [varchar](max) NULL,
	[Remarks] [varchar](max) NULL,
	[Creditor_f] [bit] NULL,
	[Debtor_f] [bit] NULL,
	[SubLedgerType] [int] NULL,
 CONSTRAINT [PK_tblBOATransaction] PRIMARY KEY CLUSTERED 
(
	[BOATransactionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblBOASummary]    Script Date: 10/31/2018 17:22:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblBOASummary](
	[BOASummaryId] [int] IDENTITY(1,1) NOT NULL,
	[AccountHeadId] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
	[SubLedgerType] [int] NULL,
 CONSTRAINT [PK_tblBOASummary] PRIMARY KEY CLUSTERED 
(
	[BOASummaryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblBOASubTransaction]    Script Date: 10/31/2018 17:22:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBOASubTransaction](
	[BOASubTransactionId] [int] IDENTITY(1,1) NOT NULL,
	[BOATransactionId] [int] NULL,
	[SubLedgerType] [int] NULL,
	[SubLedgerId] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
	[TransactionType] [varchar](max) NULL,
	[Remarks] [varchar](max) NULL,
	[Reconciliation_f] [bit] NULL,
 CONSTRAINT [PK_tblBOASubTransaction] PRIMARY KEY CLUSTERED 
(
	[BOASubTransactionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblBOAPaymentDetail]    Script Date: 10/31/2018 17:22:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBOAPaymentDetail](
	[BOAPaymentDetailId] [int] IDENTITY(1,1) NOT NULL,
	[ReferenceNumber] [varchar](max) NULL,
	[ReferenceDate] [datetime] NULL,
	[Amount] [decimal](18, 2) NULL,
	[BankName] [varchar](max) NULL,
	[Remarks] [varchar](max) NULL,
	[Reconciliation_f] [bit] NULL,
 CONSTRAINT [PK_tblBOAPaymentDetail] PRIMARY KEY CLUSTERED 
(
	[BOAPaymentDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblBOADetail]    Script Date: 10/31/2018 17:22:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblBOADetail](
	[BOADetailId] [int] IDENTITY(1,1) NOT NULL,
	[BOAId] [int] NULL,
	[CommitmentDetailId] [int] NULL,
	[ProjectId] [int] NULL,
	[BudgetHead] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
 CONSTRAINT [PK_tblBOADetail] PRIMARY KEY CLUSTERED 
(
	[BOADetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblBOA]    Script Date: 10/31/2018 17:22:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBOA](
	[BOAId] [int] IDENTITY(1,1) NOT NULL,
	[PostedDate] [datetime] NULL,
	[VoucherType] [int] NULL,
	[TempVoucherNumber] [varchar](max) NULL,
	[VoucherNumber] [varchar](max) NULL,
	[TransactionTypeCode] [varchar](max) NULL,
	[Narration] [varchar](max) NULL,
	[PaymentMode] [int] NULL,
	[Status] [varchar](max) NULL,
	[RefBOAId] [int] NULL,
	[BOAValue] [decimal](18, 2) NULL,
 CONSTRAINT [PK_tblBOA] PRIMARY KEY CLUSTERED 
(
	[BOAId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblBillPODetail]    Script Date: 10/31/2018 17:22:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBillPODetail](
	[BillPODetailId] [int] IDENTITY(1,1) NOT NULL,
	[BillId] [int] NULL,
	[TypeOfServiceOrCategory] [int] NULL,
	[Description] [varchar](max) NULL,
	[UOM] [int] NULL,
	[Quantity] [int] NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[AdvanceAmount] [decimal](18, 2) NULL,
	[TaxAmount] [decimal](18, 2) NULL,
	[Status] [varchar](max) NULL,
	[UPDT_By] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
	[Delete_By] [int] NULL,
 CONSTRAINT [PK_tblBillPODetail] PRIMARY KEY CLUSTERED 
(
	[BillPODetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblBillExpenseDetail]    Script Date: 10/31/2018 17:22:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBillExpenseDetail](
	[BillExpenseDetailId] [int] IDENTITY(1,1) NOT NULL,
	[BillId] [int] NULL,
	[AccountGroupId] [int] NULL,
	[AccountHeadId] [int] NULL,
	[TransactionType] [varchar](max) NULL,
	[Amount] [decimal](18, 2) NULL,
	[UPDT_By] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
	[Delete_By] [int] NULL,
	[Status] [varchar](max) NULL,
 CONSTRAINT [PK_tblBillExpenseDetail] PRIMARY KEY CLUSTERED 
(
	[BillExpenseDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblBillEntry]    Script Date: 10/31/2018 17:22:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBillEntry](
	[BillId] [int] IDENTITY(1,1) NOT NULL,
	[TransactionTypeCode] [varchar](max) NULL,
	[SourceReferenceNumber] [int] NULL,
	[SourceEmailDate] [datetime] NULL,
	[Source] [int] NULL,
	[BillNumber] [varchar](max) NULL,
	[VendorId] [int] NULL,
	[PODate] [datetime] NULL,
	[PONumber] [varchar](max) NULL,
	[AdvancePercentage] [decimal](18, 2) NULL,
	[EligibleForOffset_f] [bit] NULL,
	[PartiallyEligibleForOffset_f] [bit] NULL,
	[BillType] [int] NULL,
	[BillAmount] [decimal](18, 2) NULL,
	[BillTaxAmount] [decimal](18, 2) NULL,
	[CommitmentAmount] [decimal](18, 2) NULL,
	[ExpenseAmount] [decimal](18, 2) NULL,
	[DeductionAmount] [decimal](18, 2) NULL,
	[CRTD_TS] [datetime] NULL,
	[UPTD_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
	[UPTD_By] [int] NULL,
	[Status] [varchar](max) NULL,
	[CheckListVerifiedBy] [int] NULL,
 CONSTRAINT [PK_tblBillEntry] PRIMARY KEY CLUSTERED 
(
	[BillId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblBillDocumentDetail]    Script Date: 10/31/2018 17:22:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBillDocumentDetail](
	[BillDocumentDetailId] [int] IDENTITY(1,1) NOT NULL,
	[DocumentType] [int] NULL,
	[DocumentName] [varchar](max) NULL,
	[DocumentActualName] [varchar](max) NULL,
	[Remarks] [varchar](max) NULL,
	[UPDT_By] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
	[Delete_By] [int] NULL,
	[Status] [varchar](max) NULL,
	[BillId] [int] NULL,
 CONSTRAINT [PK_tblBillDocumentDetail] PRIMARY KEY CLUSTERED 
(
	[BillDocumentDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblBillDeductionDetail]    Script Date: 10/31/2018 17:22:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBillDeductionDetail](
	[BillDeductionDetailId] [int] IDENTITY(1,1) NOT NULL,
	[BillId] [int] NULL,
	[DeductionHeadId] [int] NULL,
	[AccountGroupId] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
	[UPDT_By] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
	[Delete_By] [int] NULL,
	[Status] [varchar](max) NULL,
 CONSTRAINT [PK_tblBillDeductionDetail] PRIMARY KEY CLUSTERED 
(
	[BillDeductionDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblBillCommitmentDetail]    Script Date: 10/31/2018 17:22:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBillCommitmentDetail](
	[BillCommitmentDetailId] [int] IDENTITY(1,1) NOT NULL,
	[BillId] [int] NULL,
	[CommitmentDetailId] [int] NULL,
	[PaymentAmount] [decimal](18, 2) NULL,
	[UPDT_By] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
	[Delete_By] [int] NULL,
	[Status] [varchar](max) NULL,
 CONSTRAINT [PK_tblBillCommitmentDetail] PRIMARY KEY CLUSTERED 
(
	[BillCommitmentDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblBillCheckDetail]    Script Date: 10/31/2018 17:22:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblBillCheckDetail](
	[BillCheckDetailId] [int] IDENTITY(1,1) NOT NULL,
	[BillId] [int] NULL,
	[Verified_By] [int] NULL,
	[UPDT_By] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
	[Delete_By] [int] NULL,
	[Status] [varchar](max) NULL,
	[FunctionCheckListId] [int] NULL,
 CONSTRAINT [PK_tblBillCheckDetail] PRIMARY KEY CLUSTERED 
(
	[BillCheckDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Default [DF_tblBillEntry_EligibleForOffset_f]    Script Date: 10/31/2018 17:22:42 ******/
ALTER TABLE [dbo].[tblBillEntry] ADD  CONSTRAINT [DF_tblBillEntry_EligibleForOffset_f]  DEFAULT ((0)) FOR [EligibleForOffset_f]
GO
/****** Object:  Default [DF_tblBOAPaymentDetail_Reconciliation_f]    Script Date: 10/31/2018 17:22:42 ******/
ALTER TABLE [dbo].[tblBOAPaymentDetail] ADD  CONSTRAINT [DF_tblBOAPaymentDetail_Reconciliation_f]  DEFAULT ((0)) FOR [Reconciliation_f]
GO
/****** Object:  Default [DF_tblBOASubTransaction_Reconciliation_f]    Script Date: 10/31/2018 17:22:42 ******/
ALTER TABLE [dbo].[tblBOASubTransaction] ADD  CONSTRAINT [DF_tblBOASubTransaction_Reconciliation_f]  DEFAULT ((0)) FOR [Reconciliation_f]
GO
/****** Object:  Default [DF_tblBOASummary_Amount]    Script Date: 10/31/2018 17:22:42 ******/
ALTER TABLE [dbo].[tblBOASummary] ADD  CONSTRAINT [DF_tblBOASummary_Amount]  DEFAULT ((0)) FOR [Amount]
GO
/****** Object:  Default [DF_tblBOATransaction_Creditor_f]    Script Date: 10/31/2018 17:22:42 ******/
ALTER TABLE [dbo].[tblBOATransaction] ADD  CONSTRAINT [DF_tblBOATransaction_Creditor_f]  DEFAULT ((0)) FOR [Creditor_f]
GO
/****** Object:  Default [DF_tblBOATransaction_Debtor_f]    Script Date: 10/31/2018 17:22:42 ******/
ALTER TABLE [dbo].[tblBOATransaction] ADD  CONSTRAINT [DF_tblBOATransaction_Debtor_f]  DEFAULT ((0)) FOR [Debtor_f]
GO


INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (N'VoucherType', 1, N'CJV', N'CJV', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'VoucherType', 2, N'Payment', N'Payment', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'DeductionCategory', 1, N'Service', N'Service', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'DeductionCategory', 2, N'Supply', N'Supply', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'CommitmentType', 1, N'Staff Commitment', N'Staff Commitment', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'CommitmentType', 2, N'General Commitment', N'General Commitment', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'CommitmentType', 3, N'Purchase Commitment', N'Purchase Commitment', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'CommitmentType', 4, N'Negative Balance Commitment', N'Negative Balance Commitment', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'CommitmentType', 5, N'Commitment in Foreign Currency with exchange rates', N'Commitment in Foreign Currency with exchange rates', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'VoucherType', 1, N'CJV', N'CJV', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'VoucherType', 2, N'Invoice', N'Invoice', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'VoucherType', 3, N'Payment', N'Payment', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'VoucherType', 4, N'Receipt', N'Receipt', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'VoucherType', 5, N'Journal', N'Journal', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'VoucherType', 6, N'Adhoc Payment', N'Adhoc Payment', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'SubLedgerType', 1, N'Vendor', N'Vendor', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'SubLedgerType', 2, N'Agency', N'Agency', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'SubLedgerType', 3, N'Professor', N'Professor', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'SubLedgerType', 4, N'Student', N'Student', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'PaymentMode', 1, N'Cheque', N'Cheque', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'PaymentMode', 2, N'Bank Transfer', N'Bank Transfer', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'AdvancedPercentage', 25, N'25', N'25', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'AdvancedPercentage', 75, N'75', N'75', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'AdvancedPercentage', 50, N'50', N'50', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ([CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'AdvancedPercentage', 100, N'100', N'100', NULL, NULL, NULL)

GO

/****** Object:  Table [dbo].[tblFunctionCheckListId]    Script Date: 10/20/2018 17:26:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblFunctionCheckList](
	[FunctionCheckListId] [int] IDENTITY(1,1) NOT NULL,
	[FunctionId] [int] NULL,
	[CheckList] [varchar](max) NULL,
 CONSTRAINT [PK_tblFunctionCheckListId] PRIMARY KEY CLUSTERED 
(
	[FunctionCheckListId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


update dbo.tblCodeControl set codename = 'Source' where codename = 'ProjectSource'

GO

INSERT [dbo].[tblMenuGroup] ([MenuGroupID], [ModuleID], [MenuGroup]) VALUES (10, 5, N'Bill')
/****** Object:  Table [dbo].[tblFunctionDocument]    Script Date: 10/22/2018 13:10:13 ******/
SET IDENTITY_INSERT [dbo].[tblFunctionDocument] ON
INSERT [dbo].[tblFunctionDocument] ([FunctionDocumentId], [FunctionId], [DocumentId]) VALUES (21, 29, 12)
SET IDENTITY_INSERT [dbo].[tblFunctionDocument] OFF

INSERT [dbo].[tblFunction] ([FunctionId], [FunctionName], [ActionName], [ControllerName], [ModuleID], [MenuGroupID], [MenuSeq]) VALUES (29, N'Advance Bill Payment', N'AdvanceBillPaymentList', N'CoreAccounts', 5, 9, 29)
/****** Object:  Table [dbo].[tblDocument]    Script Date: 10/22/2018 13:10:13 ******/
SET IDENTITY_INSERT [dbo].[tblDocument] ON

INSERT [dbo].[tblDocument] ([DocumentId], [DocumentName]) VALUES (12, N'Purchase Order')
SET IDENTITY_INSERT [dbo].[tblDocument] OFF

drop table [dbo].[tblCommitmentDetails]
GO

/****** Object:  Table [dbo].[tblCommitmentDetails]    Script Date: 10/31/2018 17:41:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblCommitmentDetails](
	[ComitmentDetailId] [int] IDENTITY(1,1) NOT NULL,
	[CommitmentId] [int] NULL,
	[AllocationHeadId] [int] NULL,
	[Amount] [decimal](18, 3) NULL,
	[BalanceAmount] [decimal](18, 2) NULL,
 CONSTRAINT [PK_tblCommitmentDetails] PRIMARY KEY CLUSTERED 
(
	[ComitmentDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblVendorMaster]    Script Date: 10/31/2018 17:48:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblVendorMaster](
	[VendorId] [int] IDENTITY(1,1) NOT NULL,
	[Nationality] [int] NULL,
	[VendorCode] [nvarchar](max) NULL,
	[PFMSVendorCode] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[ContactPerson] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[MobileNumber] [nvarchar](max) NULL,
	[RegisteredName] [nvarchar](max) NULL,
	[PAN] [nvarchar](max) NULL,
	[TAN] [nvarchar](max) NULL,
	[GSTExempted] [bit] NULL,
	[Reason] [nvarchar](max) NULL,
	[GSTIN] [nvarchar](max) NULL,
	[AccountHolderName] [nvarchar](max) NULL,
	[BankName] [nvarchar](max) NULL,
	[Branch] [nvarchar](max) NULL,
	[IFSC] [nvarchar](max) NULL,
	[AccountNumber] [nvarchar](max) NULL,
	[BankAddress] [nvarchar](max) NULL,
	[ABANumber] [nvarchar](max) NULL,
	[SortCode] [nvarchar](max) NULL,
	[IBAN] [nvarchar](max) NULL,
	[SWIFTorBICCode] [nvarchar](max) NULL,
	[TypeofService] [nvarchar](max) NULL,
	[ReverseTax] [bit] NULL,
	[TDSExcempted] [bit] NULL,
	[CertificateNumber] [nvarchar](max) NULL,
	[ValidityPeriod] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_UserID] [int] NULL,
	[UPDT_UserID] [int] NULL,
	[Status] [nvarchar](max) NULL,
	[SeqNbr] [int] NULL,
	[Country] [int] NULL,
	[StateCode] [int] NULL,
	[StateId] [int] NULL,
 CONSTRAINT [PK__tblVendo__FC8618F3661079C2] PRIMARY KEY CLUSTERED 
(
	[VendorId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[tblVendorMaster] ON
INSERT [dbo].[tblVendorMaster] ([VendorId], [Nationality], [VendorCode], [PFMSVendorCode], [Name], [Address], [Email], [ContactPerson], [PhoneNumber], [MobileNumber], [RegisteredName], [PAN], [TAN], [GSTExempted], [Reason], [GSTIN], [AccountHolderName], [BankName], [Branch], [IFSC], [AccountNumber], [BankAddress], [ABANumber], [SortCode], [IBAN], [SWIFTorBICCode], [TypeofService], [ReverseTax], [TDSExcempted], [CertificateNumber], [ValidityPeriod], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status], [SeqNbr], [Country], [StateCode], [StateId]) VALUES (1, 1, N'V0001', NULL, N'Rajkumar', N'chennai', N'kumarraj351@gmail.com', N'raj', NULL, N'112131212312', N'Rajkumar', N'AAAPL1234C', N'DELA99999B', 0, NULL, N'33AAWFS6761Q2Z1', N'Rajkumar', N'axisbank', N'chennai', N'ifsc001', N'123456789', N'chennai', NULL, NULL, NULL, NULL, N'No', 0, 0, N'122', 2019, CAST(0x0000A980018525A7 AS DateTime), NULL, 1, NULL, N'Active', 1, 0, 33, 33)
SET IDENTITY_INSERT [dbo].[tblVendorMaster] OFF
