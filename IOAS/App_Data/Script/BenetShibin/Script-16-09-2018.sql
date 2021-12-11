
/************* Updated on 09/16/2018 ************/
GO
alter table [tblTapal]
add Notes varchar(max)
GO
alter table [tblTapalDocumentDetail]
add [IsCurrentVersion] bit
GO
alter table tblModules 
  add ModuleIcon varchar(50)
GO
/************* END ************/