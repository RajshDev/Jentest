USE [IOASDB]
GO

ALTER TABLE tblDynamicReports
ADD CanExport bit not null default(0), ToExcel bit not null default(0), ToPDF bit not null default(0)

ALTER TABLE tblDynamicFilter
ADD IsRange bit not null default(0)

USE [IOASDB]
GO
/****** Object:  StoredProcedure [dbo].[GetDynamicReport]    Script Date: 08/26/2018 14:50:22 ******/
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
	
	SELECT     ReportID, ReportField, Aggregation, GroupBy, OrderBy
	FROM         tblDynamicSummary
	WHERE     (ReportID = @ReportID)
	
	SELECT     ReportID, ReportField, FieldType, RefTable, RefField, IsRange
	FROM         tblDynamicFilter
	WHERE     (ReportID = @ReportID)
	
	SELECT     RR.ReportID, RR.RoleID, R.RoleName, RR.IsDeleted
	FROM         tblDynamicReportRoles RR
				  inner join tblRole R ON RR.RoleId = R.RoleId
	WHERE     (ReportID = @ReportID)
	
	SELECT     R.RoleID, R.RoleName
	FROM         tblRole AS R
	WHERE      (ISNULL(R.IsDeleted,0) = 0 and R.RoleId NOT IN (SELECT ISNULL(RoleId, 0) AS RoleId FROM  tblDynamicReportRoles where ReportID = @ReportID))



	
	

ALTER PROC [dbo].[GetDynamicReportByUser]
@ReportID INT,
@User varchar(max),
@ReportName varchar(max)
AS
	SELECT     DR.ReportID, DR.ReportName, DR.Description, DR.TableName, DR.Fields, DR.GroupByFields, DR.OrderByFields, DR.RoleID, DR.ModuleID, DR.IsActive, 
				 DR.CRTD_TS, DR.UPDT_TS, DR.CRTD_UserID, DR.UPDT_UserID, DR.IsDeleted, DR.CanExport, DR.ToExcel, DR.ToPDF
	FROM tblDynamicReports DR
		INNER JOIN tblDynamicReportRoles DRR ON DRR.ReportID = DR.ReportID
		INNER JOIN tblUserRole UR ON UR.RoleId = DRR.RoleId
		INNER JOIN tblUser U ON U.UserId = UR.UserId
		INNER JOIN tblRole R ON R.RoleId = UR.RoleId	
	WHERE (DR.ReportID = @ReportID OR (@ReportID=-1 AND 1=1))
		AND (@User = '-1' OR U.UserName = @User)
		AND (@ReportName = '' OR DR.ReportName like '%'+@ReportName +'%') 
	
	SELECT     ReportID, ReportField, Aggregation, GroupBy, OrderBy
	FROM         tblDynamicSummary
	WHERE     (ReportID = @ReportID)
	
	SELECT     ReportID, ReportField, FieldType, RefTable, RefField, IsRange
	FROM         tblDynamicFilter
	WHERE     (ReportID = @ReportID)
	
	SELECT     RR.ReportID, RR.RoleID, R.RoleName, RR.IsDeleted
	FROM         tblDynamicReportRoles RR
				  inner join tblRole R ON RR.RoleId = R.RoleId
	WHERE     (ReportID = @ReportID)
	
	SELECT     R.RoleID, R.RoleName
	FROM         tblRole AS R
	WHERE      (ISNULL(R.IsDeleted,0) = 0 and R.RoleId NOT IN (SELECT ISNULL(RoleId, 0) AS RoleId FROM  tblDynamicReportRoles where ReportID = @ReportID))

	
USE [IOASDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
ALTER PROCEDURE [dbo].[DynamicReportsIU]
(

@ReportID Int OUTPUT,
@ReportName Varchar(50),
@Description Varchar(50),
@TableName Varchar(50),
@Fields Varchar(50),
@GroupByFields Varchar(50),
@OrderByFields Varchar(50),
@IsActive INT,
@UserId Int,
@RoleId Int,
@ModuleId Int,
@IsDeleted bit,
@dtSummary dtDynSummary readonly,
--@dtFilter dtDynFilter readonly,
@dtRoles dtDynReportRoles readonly,
@canExport bit,
@toPdf bit,
@toExcel bit,
@Status int OUTPUT 

)
AS
BEGIN
	select @ReportID as ReportID
	BEGIN TRANSACTION ReportSave
  IF (ISNULL(@ReportID, 0) = 0)
	BEGIN
		
		
		SET NOCOUNT ON;
		set @ReportID = ''
		INSERT INTO tblDynamicReports(ReportName, Description, TableName, Fields, GroupByFields, OrderByFields, 
			IsActive, IsDeleted, CRTD_TS, CRTD_UserID, RoleID, ModuleID, CanExport, ToPDF, ToExcel)
          
		Values(ISNULL(@ReportName,'') , ISNULL(@Description,''),ISNULL(@TableName,''),ISNULL(@Fields,''),ISNULL(@GroupByFields,''),
			   ISNULL(@OrderByFields,''),ISNULL(@IsActive,0), ISNULL(@IsDeleted,0), GETDATE(), @UserId,@RoleId, @ModuleId,
			   @canExport, @toPdf, @toExcel)
			  
			  SET  @ReportID =IDENT_CURRENT('tblDynamicReports')
   			   --set @Status='Report '+@ReportName+' created successfully' 
   			   set @Status=1 
   			   
   			   
   			insert into tblDynamicSummary(ReportID, ReportField, Aggregation, GroupBy, OrderBy)
   			select @ReportID, ReportField, Aggregation, GroupBy, OrderBy from @dtSummary
   			
   			--insert into tblDynamicFilter(ReportID, ReportField, FieldType, RefTable, RefField, IsRange)
   			--select @ReportID, ReportField, FieldType, RefTable, RefField, IsRange from @dtFilter
   			
   			insert into tblDynamicReportRoles(ReportID, RoleId)
   			select @ReportID, RoleId from @dtRoles
   			
   			IF (@@ERROR <> 0)
			BEGIN
				ROLLBACK TRANSACTION ReportSave
				RETURN
			END
   			
	END
 ELSE

	BEGIN
    SET NOCOUNT ON;
		 					
		UPDATE tblDynamicReports
		SET
			ReportName= ISNULL(@ReportName,''),
			TableName=ISNULL(@TableName,''),
			Fields=ISNULL(@Fields,''),
			GroupByFields=ISNULL(@GroupByFields,''),
			OrderByFields=ISNULL(@OrderByFields,''),
			IsActive=ISNULL(@IsActive,''),
			IsDeleted=ISNULL(@IsDeleted,0),
			UPDT_TS=GETDATE(),
			UPDT_UserId=@UserId,
			CanExport=@canExport,			
			ToPDF=@toPdf,
			ToExcel=@toExcel,
			ModuleID=@ModuleId

		WHERE           
			ReportID=@ReportID 
			
		delete from tblDynamicSummary where ReportID = @ReportID
		delete from tblDynamicFilter where ReportID = @ReportID
		delete from tblDynamicReportRoles where ReportID = @ReportID
		
		insert into tblDynamicSummary(ReportID, ReportField, Aggregation, GroupBy, OrderBy)
   		select @ReportID, ReportField, Aggregation, GroupBy, OrderBy from @dtSummary
		
		--insert into tblDynamicFilter(ReportID, ReportField, FieldType, RefTable, RefField, IsRange)
  -- 		select @ReportID, ReportField, FieldType, RefTable, RefField, IsRange from @dtFilter
		
		insert into tblDynamicReportRoles(ReportID, RoleId)
		select @ReportID, RoleId from @dtRoles   
							
		IF (@@ERROR <> 0)
		BEGIN
			ROLLBACK TRANSACTION ReportSave
			RETURN
		END   					
   					
		--set @Status='Report '+@ReportName+' updated successfully' 
		set @Status=2 
	END
		
	
	COMMIT TRANSACTION ReportSave
	
END

	
	
USE [IOASDB]
GO

/****** Object:  UserDefinedTableType [dbo].[dtDynFilter]    Script Date: 09/01/2018 20:23:43 ******/
IF  EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'dtDynFilter' AND ss.name = N'dbo')
DROP TYPE [dbo].[dtDynFilter]
GO

USE [IOASDB]
GO

/****** Object:  UserDefinedTableType [dbo].[dtDynFilter]    Script Date: 09/01/2018 20:23:43 ******/
CREATE TYPE [dbo].[dtDynFilter] AS TABLE(
	[ReportID] [int] NULL,
	[ReportField] [varchar](100) NULL,
	[FieldType] [varchar](100) NULL,
	[RefTable] [varchar](100) NULL,
	[RefField] [varchar](100) NULL,
	[IsRange] [bit]
)
GO

USE [IOASDB]
GO
/****** Object:  StoredProcedure [dbo].[DynamicReportsIU]    Script Date: 09/01/2018 20:22:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
ALTER PROCEDURE [dbo].[DynamicReportsIU]
(

@ReportID Int OUTPUT,
@ReportName Varchar(50),
@Description Varchar(50),
@TableName Varchar(50),
@Fields Varchar(50),
@GroupByFields Varchar(50),
@OrderByFields Varchar(50),
@IsActive INT,
@UserId Int,
@RoleId Int,
@ModuleId Int,
@IsDeleted bit,
@dtSummary dtDynSummary readonly,
@dtFilter dtDynFilter readonly,
@dtRoles dtDynReportRoles readonly,
@canExport bit,
@toPdf bit,
@toExcel bit,
@Status int OUTPUT 

)
AS
BEGIN
	select @ReportID as ReportID
	BEGIN TRANSACTION ReportSave
  IF (ISNULL(@ReportID, 0) = 0)
	BEGIN
		
		
		SET NOCOUNT ON;
		set @ReportID = ''
		INSERT INTO tblDynamicReports(ReportName, Description, TableName, Fields, GroupByFields, OrderByFields, 
			IsActive, IsDeleted, CRTD_TS, CRTD_UserID, RoleID, ModuleID, CanExport, ToPDF, ToExcel)
          
		Values(ISNULL(@ReportName,'') , ISNULL(@Description,''),ISNULL(@TableName,''),ISNULL(@Fields,''),ISNULL(@GroupByFields,''),
			   ISNULL(@OrderByFields,''),ISNULL(@IsActive,0), ISNULL(@IsDeleted,0), GETDATE(), @UserId,@RoleId, @ModuleId,
			   @canExport, @toPdf, @toExcel)
			  
			  SET  @ReportID =IDENT_CURRENT('tblDynamicReports')
   			   --set @Status='Report '+@ReportName+' created successfully' 
   			   set @Status=1 
   			   
   			   
   			insert into tblDynamicSummary(ReportID, ReportField, Aggregation, GroupBy, OrderBy)
   			select @ReportID, ReportField, Aggregation, GroupBy, OrderBy from @dtSummary
   			
   			insert into tblDynamicFilter(ReportID, ReportField, FieldType, RefTable, RefField, IsRange)
   			select @ReportID, ReportField, FieldType, RefTable, RefField, IsRange from @dtFilter
   			
   			insert into tblDynamicReportRoles(ReportID, RoleId)
   			select @ReportID, RoleId from @dtRoles
   			
   			IF (@@ERROR <> 0)
			BEGIN
				ROLLBACK TRANSACTION ReportSave
				RETURN
			END
   			
	END
 ELSE

	BEGIN
    SET NOCOUNT ON;
		 					
		UPDATE tblDynamicReports
		SET
			ReportName= ISNULL(@ReportName,''),
			TableName=ISNULL(@TableName,''),
			Fields=ISNULL(@Fields,''),
			GroupByFields=ISNULL(@GroupByFields,''),
			OrderByFields=ISNULL(@OrderByFields,''),
			IsActive=ISNULL(@IsActive,''),
			IsDeleted=ISNULL(@IsDeleted,0),
			UPDT_TS=GETDATE(),
			UPDT_UserId=@UserId,
			CanExport=@canExport,			
			ToPDF=@toPdf,
			ToExcel=@toExcel,
			ModuleID=@ModuleId

		WHERE           
			ReportID=@ReportID 
			
		delete from tblDynamicSummary where ReportID = @ReportID
		delete from tblDynamicFilter where ReportID = @ReportID
		delete from tblDynamicReportRoles where ReportID = @ReportID
		
		insert into tblDynamicSummary(ReportID, ReportField, Aggregation, GroupBy, OrderBy)
   		select @ReportID, ReportField, Aggregation, GroupBy, OrderBy from @dtSummary
		
		insert into tblDynamicFilter(ReportID, ReportField, FieldType, RefTable, RefField, IsRange)
   		select @ReportID, ReportField, FieldType, RefTable, RefField, IsRange from @dtFilter
		
		insert into tblDynamicReportRoles(ReportID, RoleId)
		select @ReportID, RoleId from @dtRoles   
							
		IF (@@ERROR <> 0)
		BEGIN
			ROLLBACK TRANSACTION ReportSave
			RETURN
		END   					
   					
		--set @Status='Report '+@ReportName+' updated successfully' 
		set @Status=2 
	END
		
	
	COMMIT TRANSACTION ReportSave
	
END

	
	


