USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblProcessTransactionDocuments]    Script Date: 10/02/2018 13:33:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProcessTransactionDocuments](
	[ProcessTransactionDocumentId] [int] IDENTITY(1,1) NOT NULL,
	[ProcessTransactionDetailId] [int] NULL,
	[DocumentId] [int] NULL,
	[DocumentName] [nvarchar](max) NULL,
	[DocumentPath] [nvarchar](max) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblProcessTransactionDetail]    Script Date: 10/02/2018 13:33:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblProcessTransactionDetail](
	[ProcessTransactionDetailId] [int] IDENTITY(1,1) NOT NULL,
	[ProcessTransactionId] [int] NULL,
	[ProcessGuidelineDetailId] [int] NULL,
	[ProcessSeqNumber] [int] NULL,
	[Approverid] [int] NULL,
	[ActionStatus] [nvarchar](max) NULL,
	[TransactionTS] [datetime] NULL,
	[TransactionIP] [varchar](max) NULL,
	[MacID] [nvarchar](max) NULL,
	[Comments] [nvarchar](max) NULL,
	[RefId] [int] NULL,
	[RefTable] [nvarchar](max) NULL,
	[RefFieldName] [nvarchar](max) NULL,
 CONSTRAINT [PK__tblProce__E3F9C36B367C1819] PRIMARY KEY CLUSTERED 
(
	[ProcessTransactionDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblProcessTransaction]    Script Date: 10/02/2018 13:33:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblProcessTransaction](
	[ProcessTransactionId] [int] IDENTITY(1,1) NOT NULL,
	[ProcessGuidelineDetailId] [int] NULL,
	[InitiatedUserId] [int] NULL,
	[InitiatedTS] [datetime] NULL,
	[InitiatedMacID] [nvarchar](max) NULL,
	[Closed_F] [bit] NULL,
	[ClosingStatus] [varchar](max) NULL,
 CONSTRAINT [PK__tblProce__0312E0FD32AB8735] PRIMARY KEY CLUSTERED 
(
	[ProcessTransactionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblProcessGuidelineDocument]    Script Date: 10/02/2018 13:33:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProcessGuidelineDocument](
	[PGLDocumentId] [int] IDENTITY(1,1) NOT NULL,
	[WorkflowId] [int] NULL,
	[DocumentId] [int] NULL,
 CONSTRAINT [PK_tblProcessGuidelineDocument] PRIMARY KEY CLUSTERED 
(
	[PGLDocumentId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


USE [IOASDB]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessTransactionByUser]    Script Date: 10/02/2018 13:37:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[GetProcessTransactionByUser]
@ProcessGuidelineId int,
@UserID varchar(max)
AS

	SELECT     PT.ProcessTransactionID, TD.ProcessTransactionDetailId, TD.ProcessTransactionID, TD.ProcessGuidelineDetailId, 
				TD.ProcessSeqNumber, TD.Approverid, TD.ActionStatus, 
				TD.TransactionTS, TD.TransactionIP, TD.MacID, U.UserName, U.UserImage, U.FirstName, U.LastName
				--,WF.ApproverId AS NextApprover, WF.ApproverLevel, WF.Approve_f, WF.Clarify_f,WF.Reject_f, WF.StatusId
	FROM         tblProcessTransactionDetail TD
			INNER JOIN tblProcessTransaction PT ON PT.ProcessTransactionID = TD.ProcessTransactionID			
			INNER JOIN tblUser U ON U.UserId = PT.InitiatedUserId
			INNER JOIN tblProcessGuidelineDetail PGD ON PGD.ProcessGuidelineDetailId = PT.ProcessGuidelineDetailId
			
			--RIGHT OUTER JOIN tblProcessGuidelineWorkFlow WF on WF.ProcessGuidelineDetailId = PT.ProcessGuidelineDetailId
			--	and WF.ApproverId = TD.Approverid			
			--LEFT OUTER JOIN tblProcessGuidelineUser  PGU ON PGU.ProcessGuidelineDetailId = PT.ProcessGuidelineDetailId					
	WHERE (PGD.ProcessGuidelineId = @ProcessGuidelineId OR @ProcessGuidelineId = -1) 
			AND (@UserID = -1 OR TD.ApproverId = @UserID) 

	SELECT 
			PGD.ProcessGuidelineDetailId, PGD.ProcessGuidelineId,
			U.UserName, U.UserImage, U.FirstName, U.LastName
			,WF.ApproverId, WF.ApproverLevel, WF.Approve_f, WF.Clarify_f,WF.Reject_f, WF.StatusId
			,'Approval Pending' AS ActionStatus
			, F.ActionName, F.ControllerName, F.FunctionId
	FROM tblProcessGuidelineWorkFlow WF 
		INNER JOIN tblProcessGuidelineDetail PGD ON WF.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId
		INNER JOIN tblUser U ON U.UserId = WF.ApproverId 
			AND WF.ApproverId NOT IN(SELECT ApproverId FROM tblProcessTransactionDetail PTD WHERE PTD.ProcessGuidelineDetailId = WF.ProcessGuidelineDetailId)
		INNER JOIN tblProcessGuidelineHeader PGH ON PGH.ProcessGuidelineId = PGD.ProcessGuidelineId
		INNER JOIN tblFunction F ON F.FunctionId = PGH.FunctionId
	WHERE (PGD.ProcessGuidelineId = @ProcessGuidelineId OR @ProcessGuidelineId = -1) 
			AND (@UserID = -1 OR WF.ApproverId = @UserID) 
			
					
-- exec GetProcessTransactionByUser -1, -1	
	
	-- exec GetProcessTransactionByUser 2, 6

USE [IOASDB]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessFlowByUser]    Script Date: 10/02/2018 13:37:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[GetProcessFlowByUser]
@ProcessGuidelineId INT,
@UserID int,
@FunctionID int,
@FlowID int
AS

	SELECT H.ProcessGuidelineId, H.ProcessGuidelineTitle, PGD.ProcessGuidelineDetailId, F.FunctionId, F.FunctionName,  F.ModuleID, F.MenuGroupID, F.ActionName, F.ControllerName,
		PGD.FlowTitle, WF.ApproverId, WF.ApproverLevel, ISNULL(WF.Approve_f, 0) as Approve_f, ISNULL(WF.Reject_f, 0) as Reject_f, 
		ISNULL(WF.Clarify_f, 0) as Clarify_f, ISNULL(WF.Mark_f, 0) as Mark_f,
		 U.FirstName, U.LastName, U.UserName
		 , PT.ProcessTransactionID
		 , PTD.ProcessTransactionDetailId, PTD.ProcessSeqNumber, PTD.Approverid 
		 , PTD.ActionStatus, PTD.TransactionTS, PTD.TransactionIP, PTD.MacID, PTD.Comments, PTD.RefId, PTD.RefTable
	FROM tblProcessGuidelineHeader H
		INNER JOIN tblProcessGuidelineDetail PGD ON PGD.ProcessGuidelineId = H.ProcessGuidelineId
		INNER JOIN tblProcessGuidelineWorkFlow WF ON WF.ProcessGuidelineId = H.ProcessGuidelineId 
			AND WF.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId
		INNER JOIN tblProcessGuidelineUser  PGU ON PGU.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId
		INNER JOIN tblFunction F ON F.FunctionId = H.FunctionId 
		INNER JOIN tblUser U ON U.UserId = Wf.ApproverId
		LEFT OUTER JOIN tblProcessTransaction PT on PT.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId
		LEFT OUTER JOIN tblProcessTransactionDetail PTD on PTD.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId 
			AND PTD.ProcessTransactionID = PT.ProcessTransactionID --AND PTD.Approverid = WF.ApproverId
		
					
	WHERE (H.ProcessGuidelineId = @ProcessGuidelineId OR @ProcessGuidelineId = -1)
		AND (@UserID = -1 OR U.UserId = @UserID)
		AND (@FunctionID = -1 OR F.FunctionId = @FunctionID)
		AND (@FlowID = -1 OR WF.ProcessGuidelineWorkFlowId = @FlowID)
	ORDER BY WF.ApproverLevel ASC
	
	
	

