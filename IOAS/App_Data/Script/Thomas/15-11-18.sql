USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblTravelBillTravellerDetail]    Script Date: 11/15/2018 17:33:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTravelBillTravellerDetail](
	[TravellerDetailId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NULL,
	[TravellerId] [int] NULL,
	[TravellerName] [varchar](max) NULL,
	[Boarding] [varchar](max) NULL,
	[PerDiem] [decimal](18, 2) NULL,
	[Status] [varchar](max) NULL,
	[UPDT_By] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
	[TravelBillId] [int] NULL,
 CONSTRAINT [PK_tblTravelBillTravellerDetail] PRIMARY KEY CLUSTERED 
(
	[TravellerDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblTravelBillExpenseDetail]    Script Date: 11/15/2018 17:33:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTravelBillExpenseDetail](
	[TravelBillExpenseDetailId] [int] IDENTITY(1,1) NOT NULL,
	[TravelBillId] [int] NULL,
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
 CONSTRAINT [PK_tblTravelBillExpenseDetail] PRIMARY KEY CLUSTERED 
(
	[TravelBillExpenseDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblTravelBillDocumentDetail]    Script Date: 11/15/2018 17:33:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTravelBillDocumentDetail](
	[TravelBillDocumentDetailId] [int] IDENTITY(1,1) NOT NULL,
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
	[TravelBillId] [int] NULL,
 CONSTRAINT [PK_tblTravelBillDocumentDetail] PRIMARY KEY CLUSTERED 
(
	[TravelBillDocumentDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblTravelBillDetail]    Script Date: 11/15/2018 17:33:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTravelBillDetail](
	[TravelBillDetailId] [int] IDENTITY(1,1) NOT NULL,
	[TravelBillId] [int] NULL,
	[CountryId] [int] NULL,
	[Place] [varchar](max) NULL,
	[Purpose] [varchar](max) NULL,
	[InvoiceNo] [varchar](max) NULL,
	[TravelFromDate] [datetime] NULL,
	[TravelToDate] [datetime] NULL,
	[NoOfTraveller] [int] NULL,
	[DocumentActualName] [varchar](max) NULL,
	[DocumentName] [varchar](max) NULL,
	[TotalPerDiem] [decimal](18, 2) NULL,
	[OtherExpense] [decimal](18, 2) NULL,
	[TotalExpense] [decimal](18, 2) NULL,
	[Status] [varchar](max) NULL,
	[UPDT_By] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
 CONSTRAINT [PK_tblTravelBillDetail] PRIMARY KEY CLUSTERED 
(
	[TravelBillDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblTravelBillDeductionDetail]    Script Date: 11/15/2018 17:33:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTravelBillDeductionDetail](
	[TravelBillDeductionDetailId] [int] IDENTITY(1,1) NOT NULL,
	[TravelBillId] [int] NULL,
	[DeductionHeadId] [int] NULL,
	[AccountGroupId] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
	[UPDT_By] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
	[Delete_By] [int] NULL,
	[Status] [varchar](max) NULL,
 CONSTRAINT [PK_tblTravelBillDeductionDetail] PRIMARY KEY CLUSTERED 
(
	[TravelBillDeductionDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblTravelBillCommitmentDetail]    Script Date: 11/15/2018 17:33:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTravelBillCommitmentDetail](
	[TravelBillCommitmentDetailId] [int] IDENTITY(1,1) NOT NULL,
	[TravelBillId] [int] NULL,
	[CommitmentDetailId] [int] NULL,
	[PaymentAmount] [decimal](18, 2) NULL,
	[UPDT_By] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
	[Delete_By] [int] NULL,
	[Status] [varchar](max) NULL,
 CONSTRAINT [PK_tblTravelBillCommitmentDetail] PRIMARY KEY CLUSTERED 
(
	[TravelBillCommitmentDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblTravelBillCheckDetail]    Script Date: 11/15/2018 17:33:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTravelBillCheckDetail](
	[TravelBillCheckDetailId] [int] IDENTITY(1,1) NOT NULL,
	[TravelBillId] [int] NULL,
	[Verified_By] [int] NULL,
	[UPDT_By] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
	[Delete_By] [int] NULL,
	[Status] [varchar](max) NULL,
	[FunctionCheckListId] [int] NULL,
 CONSTRAINT [PK_tblTravelBillCheckDetail] PRIMARY KEY CLUSTERED 
(
	[TravelBillCheckDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblTravelBillBreakUpDetail]    Script Date: 11/15/2018 17:33:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTravelBillBreakUpDetail](
	[BreakUpDetailId] [int] IDENTITY(1,1) NOT NULL,
	[ExpendityTypeId] [int] NULL,
	[ClaimedCurrencySpent] [decimal](18, 2) NULL,
	[ClaimedForexAmt] [decimal](18, 2) NULL,
	[ClaimedConvRate] [decimal](18, 2) NULL,
	[ClaimedTotalAmount] [decimal](18, 2) NULL,
	[ProcessedForexAmt] [decimal](18, 2) NULL,
	[ProcessedConvRate] [decimal](18, 2) NULL,
	[ProcessedTotalAmount] [decimal](18, 2) NULL,
	[DifferenceAmt] [decimal](18, 2) NULL,
	[Remarks] [varchar](max) NULL,
	[Status] [varchar](max) NULL,
	[UPDT_By] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
	[TravelBillId] [int] NULL,
 CONSTRAINT [PK_tblTravelBillBreakUpDetail] PRIMARY KEY CLUSTERED 
(
	[BreakUpDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblTravelBill]    Script Date: 11/15/2018 17:33:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTravelBill](
	[TravelBillId] [int] IDENTITY(1,1) NOT NULL,
	[ProjectId] [int] NULL,
	[TravelType] [int] NULL,
	[RefTravelBillId] [int] NULL,
	[TransactionTypeCode] [varchar](max) NULL,
	[SourceReferenceNumber] [int] NULL,
	[SourceEmailDate] [datetime] NULL,
	[Source] [int] NULL,
	[BillNumber] [varchar](max) NULL,
	[CRTD_TS] [datetime] NULL,
	[UPTD_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
	[UPTD_By] [int] NULL,
	[Status] [varchar](max) NULL,
	[EstimatedValue] [decimal](18, 2) NULL,
	[AdvanceValue] [decimal](18, 2) NULL,
	[OverallExpense] [decimal](18, 2) NULL,
	[Remarks] [varchar](max) NULL,
	[Adv100Pct_f] [bit] NULL,
	[ProformaInvoiceSubmit_f] [bit] NULL,
 CONSTRAINT [PK_tblTravelBill] PRIMARY KEY CLUSTERED 
(
	[TravelBillId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Default [DF_tblTravelBill_Adv100Pct_f]    Script Date: 11/15/2018 17:33:43 ******/
ALTER TABLE [dbo].[tblTravelBill] ADD  CONSTRAINT [DF_tblTravelBill_Adv100Pct_f]  DEFAULT ((0)) FOR [Adv100Pct_f]
GO





alter table tblBillEntry
add InvoiceNumber varchar(max),InvoiceDate datetime ,PaymentType int
alter table dbo.tblBillEntry
add PartAdvance_f bit default 0


INSERT [dbo].[tblCodeControl] ( [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'TravellerCategory', 1, N'PI / CoPI', N'PI / CoPI', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ( [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'TravellerCategory', 2, N'Student', N'Student', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ( [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'TravellerCategory', 3, N'Visitor', N'Visitor', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ( [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'TravelType', 1, N'International', N'International', NULL, NULL, NULL)
INSERT [dbo].[tblCodeControl] ( [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES ( N'TravelType', 2, N'Domestic', N'Domestic', NULL, NULL, NULL)



INSERT [dbo].[tblFunctionDocument] ([FunctionDocumentId], [FunctionId], [DocumentId]) VALUES ( 29, 12)
INSERT [dbo].[tblFunctionDocument] ([FunctionDocumentId], [FunctionId], [DocumentId]) VALUES ( 30, 12)
INSERT [dbo].[tblFunctionDocument] ([FunctionDocumentId], [FunctionId], [DocumentId]) VALUES ( 31, 12)

SET IDENTITY_INSERT [dbo].[tblFunctionCheckList] ON
INSERT [dbo].[tblFunctionCheckList] ([FunctionCheckListId], [FunctionId], [CheckList]) VALUES (1, 29, N'Purchase order')
INSERT [dbo].[tblFunctionCheckList] ([FunctionCheckListId], [FunctionId], [CheckList]) VALUES (2, 29, N'Supporting document')
INSERT [dbo].[tblFunctionCheckList] ([FunctionCheckListId], [FunctionId], [CheckList]) VALUES (3, 30, N'Purchase order')
INSERT [dbo].[tblFunctionCheckList] ([FunctionCheckListId], [FunctionId], [CheckList]) VALUES (4, 30, N'Supporting document')
INSERT [dbo].[tblFunctionCheckList] ([FunctionCheckListId], [FunctionId], [CheckList]) VALUES (5, 31, N'Purchase order')
INSERT [dbo].[tblFunctionCheckList] ([FunctionCheckListId], [FunctionId], [CheckList]) VALUES (6, 31, N'Supporting document')
SET IDENTITY_INSERT [dbo].[tblFunctionCheckList] OFF



drop table [tblCountries]
GO
/****** Object:  Table [dbo].[tblCountries]    Script Date: 11/16/2018 11:37:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblCountries](
	[countryID] [int] IDENTITY(1,1) NOT NULL,
	[countryName] [varchar](150) NOT NULL,
	[countryCode] [varchar](50) NULL,
	[currencyCode] [varchar](50) NULL,
 CONSTRAINT [PK__tblcountrie__D320769C5070F446] PRIMARY KEY CLUSTERED 
(
	[countryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ__tblcountrie__0756ED8C534D60F1] UNIQUE NONCLUSTERED 
(
	[countryName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[tblCountries] ON
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (1, N'AFGHANISTAN', N'AF', N'AFN')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (2, N'ALGERIA', N'AG', N'DZD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (3, N'LEBANON', N'leb', N'LBP')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (4, N'ARMENIA', N'AM', N'AMD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (5, N'BELGIUM', N'BE', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (6, N'BOLIVIA', N'BL', N'BOB')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (7, N'BRAZIL', N'BR', N'BRL')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (8, N'CAMEROON', N'CM', N'XAF')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (9, N'CHINA', N'CN', N'CNY')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (10, N'COSTA RICA', N'CR', N'CRC')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (11, N'CROATIA', N'HR', N'HRK')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (12, N'CZECH REPUBLIC', N'CZ', N'CZK')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (13, N'ECUADOR', N'EC', N'USD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (14, N'GIBRALTAR', N'GI', N'GIP')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (15, N'GUINEA', N'PU', N'GNF')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (16, N'ICELAND', N'IC', N'ISK')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (17, N'INDONESIA', N'ID', N'IDR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (18, N'IRAN', N'IR', N'IRR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (19, N'ISLE OF MAN', N'IM', N'IMP')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (20, N'ISRAEL', N'IL', N'ILS')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (21, N'ITALY', N'IT', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (22, N'JAMAICA', N'JM', N'JMD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (23, N'JERSEY', N'JE', N'JEP')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (24, N'KUWAIT', N'KW', N'KWD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (25, N'LIBYA', N'LY', N'LYD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (26, N'LIECHTENSTEIN', N'LS', N'CHF')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (27, N'LITHUANIA', N'LH', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (28, N'MALI', N'ML', N'XOF')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (29, N'MONGOLIA', N'MG', N'MNT')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (30, N'MOROCCO', N'MA', N'MAD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (31, N'MOZAMBIQUE', N'MZ', N'MZN')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (32, N'NETHERLANDS', N'NL', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (33, N'NEW ZEALAND', N'NZ', N'NZD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (34, N'NIGERIA', N'NG', N'NGN')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (35, N'NORWAY', N'NO', N'NOK')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (36, N'PARAGUAY', N'PG', N'PYG')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (37, N'PHILIPPINES', N'PH', N'PHP')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (38, N'SAMOA', N'WS', N'WST')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (39, N'SAUDI ARABIA', N'SA', N'SAR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (40, N'SENEGAL', N'SG', N'XOF')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (41, N'SEYCHELLES', N'SL', N'SCR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (42, N'SOMALIA', N'SO', N'SOS')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (43, N'SOUTH AFRICA', N'ZA', N'ZAR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (44, N'SPAIN', N'ES', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (45, N'SRI LANKA', N'CE', N'LKR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (46, N'SUDAN', N'SU', N'SDG')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (47, N'THAILAND', N'TH', N'THB')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (48, N'UGANDA', N'UG', N'UGX')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (49, N'UKRAINE', N'UA', N'UAH')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (50, N'UNITED ARAB EMIRATES', N'AE', N'AED')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (51, N'URUGUAY', N'UY', N'UYU')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (52, N'VIETNAM', N'VN', N'VND')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (53, N'VIRGIN ISLANDS', N'VQ', N'USD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (54, N'YEMEN', N'YM', N'YER')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (55, N'YUGOSLAVIA', N'YI', N'YUM')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (56, N'Ohio', N'OH', NULL)
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (57, N'MALAWI', N'MW', N'MWK')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (58, N'MACEDONIA', N'mac', N'MKD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (59, N'MYANMAR', N'myan', N'MMK')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (60, N'AMSTERDAM', N'AD', NULL)
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (61, N'TOGO', N'TG', N'XOF')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (62, N'ERITREA', N'ER', N'ERN')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (63, N'GHANA', N'GH', N'GHS')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (64, N'RWANDA', N'RW', N'RWF')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (65, N'REPUBLIC OF GUATEMALA', N'REG', N'GTQ')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (66, N'ARABIAN GULF', N'arbg', NULL)
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (67, N'Papua New Guinea', N'png', N'PGK')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (68, N'PALESTINE', N'PS', N'ILS')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (69, N'AZERBAIJAN', N'AJ', N'AZN')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (70, N'BOTSWANA', N'BC', N'BWP')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (71, N'BRITISH VIRGIN ISLANDS', N'VI', N'USD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (72, N'CAMBODIA', N'CB', N'KHR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (73, N'CANADA', N'CA', N'CAD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (74, N'CHILE', N'CL', N'CLP')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (75, N'CONGO', N'CG', N'CDF')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (76, N'CYPRUS', N'CY', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (77, N'EAST TIMOR', N'TT', NULL)
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (78, N'ETHIOPIA', N'ET', N'ETB')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (79, N'GERMANY', N'DE', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (80, N'GREECE', N'GR', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (81, N'HONG KONG', N'HK', N'HKD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (82, N'IRAQ', N'IZ', N'IQD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (83, N'JORDAN', N'JO', N'JOD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (84, N'KAZAKHSTAN', N'KZ', N'KZT')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (85, N'KENYA', N'KE', N'KES')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (86, N'MALDIVES', N'MV', N'MVR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (87, N'MAURITIUS', N'MP', N'MUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (88, N'MEXICO', N'MX', N'MXN')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (89, N'NEPAL', N'NP', N'NPR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (90, N'NICARAGUA', N'NU', N'NIO')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (91, N'OMAN', N'OM', N'OMR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (92, N'PERU', N'PE', N'PEN')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (93, N'PORTUGAL', N'PT', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (94, N'QATAR', N'QA', N'QAR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (95, N'RUSSIA', N'RU', N'RUB')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (96, N'SINGAPORE', N'SG', N'SGD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (97, N'SLOVENIA', N'SI', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (98, N'SWITZERLAND', N'CH', N'CHF')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (99, N'TAIWAN', N'TW', N'TWD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (100, N'TURKEY', N'TR', N'TRY')
GO
print 'Processed 100 total records'
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (101, N'UNITED KINGDOM', N'GB', N'GBP')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (102, N'VENEZUELA', N'VE', N'VES')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (103, N'ZIMBABWE', N'ZI', N'USD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (104, N'UNITED STATES', N'US', N'USD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (105, N'DJIBOUTI', N'Dji', N'DJF')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (106, N'ARGENTINA', N'AR', N'ARS')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (107, N'AUSTRALIA', N'AU', N'AUD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (108, N'AUSTRIA', N'AT', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (109, N'BAHAMAS, THE', N'BF', N'BSD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (110, N'BAHRAIN', N'BH', N'BHD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (111, N'BANGLADESH', N'BG', N'BDT')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (112, N'BARBADOS', N'BB', N'BBD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (113, N'BERMUDA', N'BD', N'BMD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (114, N'BRUNEI', N'BX', N'BND')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (115, N'BULGARIA', N'BU', N'BGN')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (116, N'COLOMBIA', N'CO', N'COP')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (117, N'CUBA', N'CU', N'CUP')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (118, N'DENMARK', N'DK', N'DKK')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (119, N'DOMINICAN REPUBLIC', N'DR', N'DOP')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (120, N'EGYPT', N'EG', N'EGP')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (121, N'FIJI', N'FJ', N'FJD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (122, N'FINLAND', N'FI', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (123, N'FRANCE', N'FR', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (124, N'GUERNSEY', N'GK', N'GGP')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (125, N'GUYANA', N'GY', N'GYD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (126, N'HONDURAS', N'HO', N'HNL')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (127, N'HUNGARY', N'HU', N'HUF')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (128, N'INDIA', N'IN', N'INR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (129, N'IRELAND', N'IE', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (130, N'JAPAN', N'JP', N'JPY')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (131, N'KIRIBATI', N'KI', N'AUD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (132, N'LUXEMBOURG', N'LU', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (133, N'MADAGASCAR', N'MA', N'MGA')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (134, N'MALAYSIA', N'MY', N'MYR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (135, N'MALTA', N'MT', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (136, N'MONACO', N'MN', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (137, N'NORTH KOREA', N'KN', N'KPW')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (138, N'PAKISTAN', N'PK', N'PKR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (139, N'POLAND', N'PL', N'PLN')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (140, N'ROMANIA', N'RO', N'RON')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (141, N'SLOVAKIA', N'LO', N'EUR')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (142, N'SOUTH KOREA', N'KR', N'KRW')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (143, N'SWAZILAND', N'WZ', NULL)
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (144, N'SWEDEN', N'SE', N'SEK')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (145, N'SYRIA', N'SY', N'SYP')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (146, N'TAJIKISTAN', N'TI', N'TJS')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (147, N'TANZANIA', N'TZ', N'TZS')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (148, N'TUNISIA', N'TS', N'TND')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (149, N'UZBEKISTAN', N'UZ', N'UZS')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (150, N'ZAMBIA', N'ZM', N'ZMW')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (151, N'ARAB COUNTRIES GCC', N'GCC', N'USD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (152, N'ARAB COUNTRIES NON GCC', N'NGCC', N'USD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (153, N'ASIA', N'ASIA', N'USD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (154, N'ENGLISH SPEAKING', N'ESPC', N'USD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (155, N'ANTARCTICA', N'AQ', N'USD')
INSERT [dbo].[tblCountries] ([countryID], [countryName], [countryCode], [currencyCode]) VALUES (156, N'PANAMA', N'PA', N'USD')
SET IDENTITY_INSERT [dbo].[tblCountries] OFF
/****** Object:  View [dbo].[vwStudentDetails]    Script Date: 11/16/2018 11:37:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vwStudentDetails]
AS
SELECT     RollNumber, StudentName AS FirstName, DepartmentCode, DepartmentName, ProgramCode, ProgramName, EmailID, ContactNumber
FROM         WFADS.dbo.Student_Details
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Student_Details (WFADS.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 126
               Right = 211
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwStudentDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwStudentDetails'
GO
