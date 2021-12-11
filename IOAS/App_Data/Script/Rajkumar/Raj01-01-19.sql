insert into tblStateMaster (StateName,StateCode)values('Other Territory',97)


insert into tblCodeControl (CodeName,CodeValAbbr,CodeValDetail,CodeDescription)values('CompanyType',1,'Government','To calculate TDS under GST')

insert into tblCodeControl (CodeName,CodeValAbbr,CodeValDetail,CodeDescription)values('CompanyType',2,'Public','To calculate TDS under GST')

insert into tblCodeControl (CodeName,CodeValAbbr,CodeValDetail,CodeDescription)values('CompanyType',3,'Private','To calculate TDS under GST')

insert into tblCodeControl (CodeName,CodeValAbbr,CodeValDetail,CodeDescription)values('CompanyType',4,'Firm','To calculate TDS under GST')

insert into tblCodeControl (CodeName,CodeValAbbr,CodeValDetail,CodeDescription)values('CompanyType',5,'Associate','To calculate TDS under GST')

insert into tblCodeControl (CodeName,CodeValAbbr,CodeValDetail,CodeDescription)values('CompanyType',6,'Joint Ventures','To calculate TDS under GST')

insert into tblCodeControl (CodeName,CodeValAbbr,CodeValDetail,CodeDescription)values('CompanyType',7,'Others','To calculate TDS under GST')

insert into tblCodeControl (CodeName,CodeValAbbr,CodeValDetail,CodeDescription)values('govertmentagy',1,'Ministry','AgencyMaster')

alter table tblAgencyMaster add CompanyType int

alter table tblAgencyMaster add GovermentAgencyType int