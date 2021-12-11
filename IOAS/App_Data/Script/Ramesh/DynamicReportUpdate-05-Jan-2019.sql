USE [IOASDB]
GO
/****** Object:  StoredProcedure [dbo].[GetDynamicReport]    Script Date: 05-Jan-19 3:10:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[GetDynamicReport]
@ReportID INT
AS
	SELECT     ReportID, ReportName, Description, TableName, Fields, GroupByFields, OrderByFields, RoleID, ModuleID, IsActive, 
				 CRTD_TS, UPDT_TS, CRTD_UserID, UPDT_UserID, IsDeleted, CanExport, ToExcel, ToPDF
	FROM         tblDynamicReports WHERE (ReportID = @ReportID OR (@ReportID=-1 AND 1=1))
	
	SELECT     DS.ReportID, DS.ReportField, DS.Aggregation, DS.GroupBy, DS.OrderBy, O.DATA_TYPE as DType
	FROM         tblDynamicSummary DS
				INNER JOIN tblDynamicReports DR on DR.ReportID = DS.ReportID
				INNER JOIN INFORMATION_SCHEMA.COLUMNS O on O.TABLE_NAME = DR.TableName and DS.ReportField = O.COLUMN_NAME
	WHERE     (DS.ReportID = @ReportID)
	
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
