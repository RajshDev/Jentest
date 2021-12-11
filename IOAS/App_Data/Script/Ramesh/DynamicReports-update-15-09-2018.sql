
USE [IOASDB]
GO

/****** Object:  StoredProcedure [dbo].[GetDynamicReportByUser]    Script Date: 09/16/2018 14:45:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[GetDynamicReportByUser]
@ReportID INT,
@User varchar(max),
@ReportName varchar(max)
AS
	SELECT     DR.ReportID, DR.ReportName, DR.Description, DR.TableName, DR.Fields, DR.GroupByFields, DR.OrderByFields, DR.RoleID, DR.ModuleID, DR.IsActive, 
				 DR.CRTD_TS, DR.UPDT_TS, DR.CRTD_UserID, DR.UPDT_UserID, DR.IsDeleted, DR.CanExport, DR.ToExcel, DR.ToPDF,
				 M.ModuleName
	FROM tblDynamicReports DR
		INNER JOIN tblDynamicReportRoles DRR ON DRR.ReportID = DR.ReportID
		INNER JOIN tblUserRole UR ON UR.RoleId = DRR.RoleId
		INNER JOIN tblUser U ON U.UserId = UR.UserId
		INNER JOIN tblRole R ON R.RoleId = UR.RoleId
		INNER JOIN tblModules M ON M.ModuleID = DR.ModuleID
	WHERE (DR.ReportID = @ReportID OR (@ReportID=-1 AND 1=1))
		AND (@User = '-1' OR U.UserName = @User)
		AND (@ReportName = '' OR DR.ReportName like '%'+@ReportName +'%') 
	
	SELECT     ReportID, ReportField, Aggregation, GroupBy, OrderBy
	FROM         tblDynamicSummary
	WHERE     (ReportID = @ReportID)
	
	SELECT     DF.ReportID, DF.ReportField, DF.FieldType, DF.RefTable, DF.RefField, DF.IsRange, O.DATA_TYPE as DType
	FROM         tblDynamicFilter DF
				INNER JOIN tblDynamicReports DR on DR.ReportID = DF.ReportID
				INNER JOIN INFORMATION_SCHEMA.COLUMNS O on O.TABLE_NAME = DR.TableName and DF.ReportField = O.COLUMN_NAME
	WHERE     (DF.ReportID = @ReportID)
	
	SELECT     RR.ReportID, RR.RoleID, R.RoleName, RR.IsDeleted
	FROM         tblDynamicReportRoles RR
				  inner join tblRole R ON RR.RoleId = R.RoleId
	WHERE     (ReportID = @ReportID)
	
	SELECT     R.RoleID, R.RoleName
	FROM         tblRole AS R
	WHERE      (ISNULL(R.IsDeleted,0) = 0 and R.RoleId NOT IN (SELECT ISNULL(RoleId, 0) AS RoleId FROM  tblDynamicReportRoles where ReportID = @ReportID))

	
	
	

