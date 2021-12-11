USE [IOASDB]
GO

/****** Object:  Table [dbo].[tblProcessTransactionDocuments]    Script Date: 10/13/2018 17:18:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblProcessTransactionDocuments]') AND type in (N'U'))
DROP TABLE [dbo].[tblProcessTransactionDocuments]
GO

USE [IOASDB]
GO

/****** Object:  Table [dbo].[tblProcessTransactionDocuments]    Script Date: 10/13/2018 17:18:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblProcessTransactionDocuments](
	[ProcessTransactionDocumentId] [int] IDENTITY(1,1) NOT NULL,
	[ProcessTransactionDetailId] [int] NULL,
	[DocumentId] [int] NULL,
	[DocumentName] [nvarchar](max) NULL,
	[DocumentPath] [nvarchar](max) NULL,
	[IsRequired] [bit] NULL,
	[UUID] [nvarchar](max) NULL,
	[CRTD_TS] [datetime] NULL,
	[UPTD_TS] [datetime] NULL,
 CONSTRAINT [PK_tblProcessTransactionDocuments] PRIMARY KEY CLUSTERED 
(
	[ProcessTransactionDocumentId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

USE [IOASDB]
GO

/****** Object:  Table [dbo].[tblProcessGuidelineWorkflowDocument]    Script Date: 10/13/2018 17:40:51 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblProcessGuidelineWorkflowDocument]') AND type in (N'U'))
DROP TABLE [dbo].[tblProcessGuidelineWorkflowDocument]
GO

USE [IOASDB]
GO

/****** Object:  Table [dbo].[tblProcessGuidelineWorkflowDocument]    Script Date: 10/13/2018 17:40:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblProcessGuidelineWorkflowDocument](
	[ProcessGuidelineWorkflowDocumentId] [int] IDENTITY(1,1) NOT NULL,
	[ProcessGuidelineWorkflowId] [int] NULL,
	[DocumentId] [int] NULL,
	[DocumentName] [nvarchar](max) NULL,
	[DocumentType] [nvarchar](max) NULL,
	[IsRequired] [bit] NULL,
	[UUID] [nvarchar](max) NULL,
	[CRTD_TS] [datetime] NULL,
	[UPTD_TS] [datetime] NULL
) ON [PRIMARY]

GO




