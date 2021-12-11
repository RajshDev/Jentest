USE [IOASDB]
GO

ALTER TABLE tblProcessTransaction
ADD ActionLink varchar(max) null, successMethod varchar(max) null, failedMethod varchar(max) null


USE [IOASDB]
GO
/****** Object:  StoredProcedure [dbo].[GetProcessFlowByUser]    Script Date: 11/12/2018 07:34:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[GetProcessFlowByUser]
@ProcessGuidelineId INT,
@UserID int,
@TransactionId int
AS


	select 
	 PTD.ProcessTransactionId, PTD.RefFieldName, PTD.RefId, PTD.RefTable into #tempProcessTransDetail
	from tblProcessTransactionDetail PTD 
		inner join tblProcessGuidelineDetail PGD on PTD.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId
		where (PGD.ProcessGuidelineId = @processGuideLineId)

/*NOTE: Process guideline user must be initiated user. So that the approver can view the record and can approve*/
select PT.ProcessTransactionId, PT.InitiatedUserId

	,WF.ProcessGuidelineWorkFlowId, WF.ProcessGuidelineId, WF.ProcessGuidelineDetailId, WF.ApproverId, WF.ApproverLevel, 
	ISNULL(WF.Approve_f, 0) as Approve_f, ISNULL(WF.Reject_f, 0) as Reject_f, 
	ISNULL(WF.Clarify_f, 0) as Clarify_f, ISNULL(WF.Mark_f, 0) as Mark_f, 
	WF.CreatedTS, WF.CreatedUserId, WF.LastUpdatedTS, 
    WF.LastUpdatedUserId, WF.StatusId, 'Approval Pending' AS ActionStatus
   ,U.UserName
   ,U.UserImage
   ,U.FirstName
   ,U.LastName
   ,(select top(1) RefId from #tempProcessTransDetail where ProcessTransactionId = PT.ProcessTransactionId) as RefId
   ,(select top(1) RefTable from #tempProcessTransDetail where ProcessTransactionId = PT.ProcessTransactionId) as RefTable
   ,(select top(1) RefFieldName from #tempProcessTransDetail where ProcessTransactionId = PT.ProcessTransactionId) as RefFieldName
from tblProcessTransaction PT
   inner join tblProcessGuidelineDetail PGD on PT.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId
   inner join tblProcessGuidelineWorkFlow WF on PGD.ProcessGuidelineDetailId = WF.ProcessGuidelineDetailId
   inner join tblProcessGuidelineUser PGU on PT.InitiatedUserId = PGU.UserId AND PGU.ProcessGuidelineDetailId = WF.ProcessGuidelineDetailId
   inner join tblUser U on U.UserId = WF.ApproverId
where (PGD.ProcessGuidelineId = @ProcessGuidelineId) and (WF.ApproverId = @UserID)
order by PT.ProcessTransactionId

/*NOTE: Fetch one record at a time for approval. Process guideline user must be initiated user. So that the approver can view the record and can approve*/
	SELECT H.ProcessGuidelineId, H.ProcessGuidelineTitle , PGD.ProcessGuidelineDetailId
		--,F.FunctionId, F.FunctionName,  F.ModuleID, F.MenuGroupID, F.ActionName, F.ControllerName
		,PGD.FlowTitle, WF.ApproverId, WF.ApproverLevel, ISNULL(WF.Approve_f, 0) as Approve_f, ISNULL(WF.Reject_f, 0) as Reject_f
		 ,ISNULL(WF.Clarify_f, 0) as Clarify_f, ISNULL(WF.Mark_f, 0) as Mark_f
		 ,U.FirstName, U.LastName, U.UserName
		 , PT.ProcessTransactionId
		 , PTD.ProcessTransactionDetailId, PTD.ProcessSeqNumber, PTD.Approverid 
		 , ISNULL(PTD.ActionStatus,'Pending') AS ActionStatus, PTD.TransactionTS, PTD.TransactionIP, PTD.MacID, PTD.Comments, PTD.RefId, PTD.RefTable
	FROM tblProcessGuidelineHeader H
		INNER JOIN tblProcessGuidelineDetail PGD ON PGD.ProcessGuidelineId = H.ProcessGuidelineId
		INNER JOIN tblProcessGuidelineWorkFlow WF ON WF.ProcessGuidelineId = H.ProcessGuidelineId 
			AND WF.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId
		INNER JOIN tblProcessGuidelineUser  PGU ON PGU.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId 
		INNER JOIN tblFunction F ON F.FunctionId = H.FunctionId 
		INNER JOIN tblUser U ON U.UserId = Wf.ApproverId
		INNER JOIN tblProcessTransaction PT on PT.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId AND PT.InitiatedUserId = PGU.UserId
		
		--    and PT.ProcessTransactionId not in (select 
		-- PTD.ProcessTransactionId
		--from tblProcessTransactionDetail PTD
		--	inner join tblProcessGuidelineDetail PGD on PTD.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId
		--	where (PGD.ProcessGuidelineId = @processGuideLineId and PTD.Approverid = @userId and PTD.ProcessTransactionId = PT.ProcessTransactionId))
		
		LEFT OUTER JOIN tblProcessTransactionDetail PTD on PTD.ProcessGuidelineDetailId = PGD.ProcessGuidelineDetailId 
			AND PTD.ProcessTransactionID = PT.ProcessTransactionID AND PTD.Approverid = WF.ApproverId
		
					
	WHERE (H.ProcessGuidelineId = @ProcessGuidelineId OR @ProcessGuidelineId = -1)
		--AND (@UserID = -1 OR U.UserId = @UserID)
		AND (PT.ProcessTransactionId = @TransactionId)

	ORDER BY WF.ApproverLevel ASC
	
	
	

