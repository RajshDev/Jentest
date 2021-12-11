USE [IOASDB]
GO
/****** Object:  StoredProcedure [dbo].[GetPendingTransactionByUser]    Script Date: 11/29/2018 10:00:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetPendingTransactionByUser] 
@ProcessGuidelineId int,
@UserID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		PT.RefId, ISNULL(PTD.ActionStatus,'Pending') AS ActionStatus
		, WF.ApproverId, WF.ApproverLevel
		, H.ProcessGuidelineId, H.ProcessGuidelineTitle , PGD.ProcessGuidelineDetailId
		,PGD.FlowTitle, ISNULL(WF.Approve_f, 0) as Approve_f, ISNULL(WF.Reject_f, 0) as Reject_f
		 ,ISNULL(WF.Clarify_f, 0) as Clarify_f, ISNULL(WF.Mark_f, 0) as Mark_f
		 ,inituser.FirstName, inituser.LastName,inituser.UserName
		 , PT.ProcessTransactionId
		 , PTD.ProcessTransactionDetailId, PTD.ProcessSeqNumber, PTD.Approverid as PTD_Approverid
		 , initPTD.TransactionTS, PTD.TransactionIP, PTD.MacID, PTD.Comments,  PTD.RefTable, PT.ActionLink
		 into #tempPendingRecords
	FROM tblProcessGuidelineHeader H
		INNER JOIN tblProcessGuidelineDetail PGD ON PGD.ProcessGuidelineId = H.ProcessGuidelineId
		INNER JOIN tblProcessGuidelineWorkFlow WF ON WF.ProcessGuidelineId = H.ProcessGuidelineId 
			AND WF.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId
		INNER JOIN tblProcessGuidelineUser  PGU ON PGU.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId 
		INNER JOIN tblFunction F ON F.FunctionId = H.FunctionId 
		INNER JOIN tblUser U ON U.UserId = Wf.ApproverId
		INNER JOIN tblProcessTransaction PT on PT.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId AND PT.InitiatedUserId = PGU.UserId
		LEFT OUTER JOIN tblProcessTransactionDetail PTD on PTD.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId 
			AND PTD.ProcessTransactionID = PT.ProcessTransactionID AND PTD.Approverid = WF.ApproverId AND PTD.ActionStatus != 'Rejected'
		left join tbluser inituser on inituser.userId in (select Approverid from tblProcessTransactionDetail where ProcessTransactionID = PT.ProcessTransactionID and ActionStatus = 'Initiated')
		left join tblProcessTransactionDetail initPTD on initPTD.ProcessTransactionDetailId IN (select ProcessTransactionDetailId from tblProcessTransactionDetail where ProcessTransactionID = PT.ProcessTransactionID and ActionStatus = 'Initiated' )
		
	WHERE (H.ProcessGuidelineId = @ProcessGuidelineId OR @ProcessGuidelineId = -1)
		--AND (PT.RefId = @RefId)

	ORDER BY WF.ApproverLevel ASC

	/*NOTE: This is a temp table collects all pending records.*/
	select * from #tempPendingRecords where ActionStatus = 'Pending'

	/*NOTE:Get the pending and filter next approver by approver level. Minimum approval level value is next approver. Max value is last approver*/
	select MIN(ApproverLevel) as ApproverLevel, RefId into #tempPendingLevel from #tempPendingRecords 
	where ActionStatus = 'Pending' group by RefId

	select PR.* from #tempPendingRecords PR
			inner join #tempPendingLevel PL on PR.ApproverLevel = PL.ApproverLevel and PR.RefId = PL.RefId
	Where ApproverId = @UserID


	drop table #tempPendingRecords
	drop table #tempPendingLevel
END
