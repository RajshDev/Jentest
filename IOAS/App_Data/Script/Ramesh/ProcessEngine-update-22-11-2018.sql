USE [IOASDB]
GO

ALTER TABLE tblProcessTransaction
add [RefId] [int] NULL,
	[RefTable] [nvarchar](max) NULL,
	[RefFieldName] [nvarchar](max) NULL