
/*These tables not used.*/

CREATE TABLE tblVoucherHead
(VoucherId int not null primary key identity(1,1),
 ReferenceType int null,
 ReferenceNo varchar(max) null,
 RequestId int null,
 VoucherDate datetime null,
 ProjectId int null,
 VoucherAmount money null,
 VoucherStatus int null,
 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null)
 /*These tables not used.*/
 CREATE TABLE tblVoucherDetails(
 VoucherDetailId int not null primary key identity(1,1),
 VoucherId int not null,
 VendorId int null,
 SubLedgerId int null,
 AccountHeadId int null,
 VoucherStatus int null,
 VoucherAmount money null,
 PaidAmount money null,
 PaymentMode int null,
 PayableTo varchar(max),
 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null 
 )

 /************************/
 
 
CREATE TABLE tblStaffAppointmentMaster
(StaffMasterId int not null primary key identity(1,1),
 AppointmentTypeId int null,
 OrderNumber varchar(max) null,
 FileNumber varchar(max) null,
 StaffID int null,
 DepartmentId int null,
 ProjectId int null,
 StaffName varchar(max) null,
 Designation varchar(max) null,
 GenderId int null,
 DOB datetime null,
 AppointmentFrom datetime null,
 AppointmentTo datetime null,
 BankAccount varchar(max) null,
 SalaryType int null,
 PAN varchar(max) null,
 MedicalInsurance varchar(max) null,
 TA money null,
 HRA money null,
 PayableAmount money null,
 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null)
 
 CREATE TABLE tblStaffAppointmentExtension
(StaffMasterId int not null primary key identity(1,1),
 ProjectType int null,
 FileNumber varchar(max) null,
 StaffName varchar(max) null,
 ProjectId int null,
 AppointmentTypeId int null,
 OrderNumber varchar(max) null,
 BankAccount varchar(max) null,
 Designation varchar(max) null,
 AppointmentDate datetime null,
 RelievingDate datetime null,
 Salary money null,
 HRA money null,
 PayableAmount money null, 
 StaffID int null,
 DepartmentId int null,
 GenderId int null,
 DOB datetime null,


 SalaryType int null,
 PAN varchar(max) null,
 MedicalInsurance varchar(max) null,
 TA money null,


 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null)
 
 
CREATE VIEW VWAppointInfo
AS
SELECT     AM.EmployeeID, AM.EmployeeName, AM.MeetingID, AM.CandidateID, AM.DOB, AM.DesignationCode, AM.DesignationName, AM.AppointmentDate, 
AM.ToDate, AM.RelieveDate, AM.BasicSalary, AM.PermanentAddress, AM.CommunicationAddress, 
AM.MobileNumber, AM.EmailID, AM.BankName, AM.BranchName, AM.BankAccountNo, AM.IFSC_Code, AM.OutSourcingCompany, AM.CreatedOn, AM.CreatedBy, 
AM.UpdatedOn, AM.UpdatedBy, AM.Remarks,
AD.OrderID, AD.OrderType, AD.ProjectNo, AD.FromDate, AD.ToDate as DetailToDate,
AD.GrossSalary, AD.CostToProject, AD.CommitmentNo
FROM         Recruit.dbo.AppointmentMaster AS AM
		INNER JOIN Recruit.dbo.AppointmentDetails AD ON AM.EmployeeID = AD.EmployeeID
		
		
CREATE VIEW VWOngoing
AS
SELECT     FileNo, AppointmentType, ProjectTitle, PROJECTNO, PTYPE, shon, NAME, departmentcode, DEPARTMENT, SPON, DesignationCode, Designation, AppointmentDate, ExtensionDate, RelieveDate, 
                      Paytype, BasicPay, HRA, Medical, CoordinatorCode, phon, CoordinatorName, Community, OldFileno, DOB, FATHER, ADDRESS1, ADDRESS2, ADDRESS3, ADDRESS4, Gender, BloodGroup, RH, 
                      PHONE, R_ADDR1, R_ADDR2, R_ADDR3, R_ADDR4, COURSE, paybill_id, paybill_no, email_id, Pensioner, Qualification, offerId, AppointmentLetterDate, AppointmentLetterNo, DateOfInput, 
                      ModifiedDate, username, commitmentNo
FROM         Recruit.dbo.Ongoing



CREATE VIEW VWOfficeTransactions
AS
SELECT     FileNo, AppointmentType, ProjectTitle, PROJECTNO, PTYPE, shon, NAME, departmentcode, DEPARTMENT, SPON, DesignationCode, Designation, AppointmentDate, ExtensionDate, RelieveDate, 
                      Paytype, BasicPay, HRA, Medical, CoordinatorCode, phon, CoordinatorName, Community, OldFileno, DOB, FATHER, ADDRESS1, ADDRESS2, ADDRESS3, ADDRESS4, Gender, BloodGroup, RH, 
                      PHONE, R_ADDR1, R_ADDR2, R_ADDR3, R_ADDR4, COURSE, paybill_id, paybill_no, email_id, Pensioner, Qualification, offerId, AppointmentLetterDate, AppointmentLetterNo, DateOfInput, 
                      ModifiedDate, TransactionType, Filename, username, commitmentNo, HashID, approvedPdf
FROM         Recruit.dbo.OfficeTransactions



CREATE TABLE tblITDeclaration
(DeclarationID int not null primary key identity(1,1),
 SNO int null,
 SectionName varchar(max) null,
 SectionCode varchar(max) null,
 Particulars varchar(max) null,
 MaxLimit money,
 Age int null,
 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null)
 
 INSERT INTO tblITDeclaration(SectionName, SectionCode, Particulars, MaxLimit, Age, CreatedAt)
values('80C', '80C', '80C - 5 Years of Fixed Deposit in Scheduled Bank', null, null, GETDATE()),
('80C', '80C', '80C - Children Tuition Fees', null,null, GETDATE()),
('80CCC', '80CCC', '80CCC - Contribution to Pension Fund', null,null, GETDATE()),
('80C', '80C', '80C - Deposit in NSC', null,null, GETDATE()),
('80C', '80C', '80C - Deposit in NSS', null, null,GETDATE()),
('80C', '80C', '80C - Deposit in Post Office Savings Schemes', null,null, GETDATE()),
('80C', '80C', '80C - Equity Linked Savings Scheme ( ELSS )', null,null, GETDATE()),
('80C', '80C', '80C - Infrastructure Bonds', null,null, GETDATE()),
('80C', '80C', '80C - Interest on NSC Reinvested', null,null, GETDATE()),
('80C', '80C', '80C - Kisan Vikas Patra (KVP)', null, null,GETDATE()),
('80C', '80C', '80C - Life Insurance Premium', null,null, GETDATE()),
('80C', '80C', '80C - Long term Infrastructure Bonds', null, null,GETDATE()),
('80C', '80C', '80C - Mutual Funds', null, null,GETDATE()),
('80C', '80C', '80C - NABARD Rural Bonds', null, null,GETDATE()),
('80C', '80C', '80C - National Pension Scheme', null, null,GETDATE()),
('80C', '80C', '80C - Post office time deposit for 5 years', null,null, GETDATE()),
('80C', '80C', '80C - Pradhan Mantri Suraksha Bima Yojana', null, null,GETDATE()),
('80C', '80C', '80C - Public Provident Fund', null,null, GETDATE()),
('80C', '80C', '80C - Repayment of Housing loan(Principal amount)', null, null,GETDATE()),
('80C', '80C', '80C - Stamp duty and Registration charges', null, null,GETDATE()),
('80C', '80C', '80C - Sukanya Samriddhi Yojana', null, null,GETDATE()),
('80C', '80C', '80C - Unit Linked Insurance Premium (ULIP)', null, null,GETDATE()),
('80D', '80D', '80D - Preventive Health Checkup - Dependant Parents', 5000, null,GETDATE()),
('80D', '80D', '80D - Medical Bills - Very Senior Citizen', 50000, null,GETDATE()),
('80D', '80D', '80D - Medical Insurance Premium', 25000, 60,GETDATE()),
('80D', '80D', '80D - Medical Insurance Premium - Dependant Parents', 25000, 60,GETDATE()),
('80D', '80D', '80D - Preventive Health Check-up', 5000, null,GETDATE()),
('80EE', '80EE', '80EE - Additional Interest on housing loan borrowed as on 1st Apr 2016', 5000, null,GETDATE()),
('80CCD1(B)', '80CCD1(B)', '80CCD1(B) - Contribution to NPS 2015', 5000, null,GETDATE()),
('80DDB', '80DDB', '80DDB - Medical Treatment (Specified Disease only)- Senior Citizen', 100000, null,GETDATE()),
('80DDB', '80DDB', '80DDB - Medical Treatment (Specified Disease only)- Very Senior Citizen', 100000, null,GETDATE()),
('80TTB', '80TTB', '80TTB - Interest on Deposits in Savings Account, FDs, Post Office And Cooperative Society for Senior Citizen', 5000, null,GETDATE()),
('80G', '80G', '80G - Donation - 100% Exemption', 99999999, null,GETDATE()),
('80G', '80G', '80G - Donation - 50% Exemption', 99999999, null,GETDATE()),
('80G', '80G', '80G - Donation - Children Education', 99999999, null,GETDATE()),
('80G', '80G', '80G - Donation - Political Parties', 5000, null,GETDATE()),
('80TTA', '80TTA', '80TTA - Interest on Deposits in Savings Account, FDs, Post Office And Cooperative Society', 10000, null,GETDATE()),
('80E', '80E', '80E - Interest on Loan of higher Self education', 99999999, null,GETDATE()),
('80E', '80E', '80DD - Medical Treatment / Insurance of handicapped Dependant', 75000, null,GETDATE()),
('80E', '80E', '80DD - Medical Treatment / Insurance of handicapped Dependant (Severe)', 125000, null,GETDATE()),
('80E', '80E', '80DDB - Medical Treatment ( Specified Disease only )', 40000, null,GETDATE()),
('80E', '80E', '80U - Permanent Physical disability (Above 40%)', 125000, null,GETDATE()),
('80E', '80E', '80U - Permanent Physical disability (Below 40%)', 75000, null,GETDATE()),
('80E', '80E', '80CCG - Rajiv Gandhi Equity Scheme', 25000, null,GETDATE())



CREATE TABLE tblEmpITDeclaration
(
 SectionID int not null primary key identity(1,1),
 DeclarationID int null,
 DocumentId int null,
 DocumentName varchar(max) null,
 Documentpath varchar(max) null,
 EmpNo varchar(max) null,
 SectionName varchar(max) null,
 SectionCode varchar(max) null,
 Particulars varchar(max) null,
 MaxLimit money,
 Amount money,
 Age int null,
 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null)
 
 CREATE TABLE tblEmpITDeclarationDoc
(
 DocumentId int not null primary key identity(1,1),
 DeclarationID int null,
 SectionID int null,
 DocumentName varchar(max) null,
 Documentpath varchar(max) null,
 EmpNo varchar(max) null,
 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null)
 
CREATE TABLE tblEmpSOPIncome
(
 ID int not null primary key identity(1,1),
 EligibleAmount money null,
 Amount money null,
 EmpId int null,
 EmpNo varchar(max) null,
 SubmittedOn datetime null,
 LenderName varchar(max),
 LenderPAN varchar(max),
 IsDeleted bit null,
 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null)
 
  CREATE TABLE tblEmpOtherIncome
(
 ID int not null primary key identity(1,1),
 EligibleAmount money null,
 Amount money null,
 EmpId int null,
 EmpNo varchar(max) null,
 SubmittedOn datetime null,
 IsDeleted bit null,
 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null) 
 
insert into tblCodeControl(CodeName, CodeValAbbr, CodeValDetail, CodeDescription)
values('SalaryPaymentType', 1, 'Cheque', 'Cheque'),
('SalaryPaymentType', 3, 'NEFT', 'NEFT'),
('SalaryPaymentType', 2, 'RTGS', 'RTGS')
insert into tblCodeControl(CodeName, CodeValAbbr, CodeValDetail, CodeDescription)
values('OfficeOrderType', 1, 'Cheque', 'Cheque'),
('OfficeOrderType', 3, 'NEFT', 'NEFT'),
('OfficeOrderType', 2, 'RTGS', 'RTGS')

CREATE TABLE tblOfficeOrder
(
 OrderId int not null primary key identity(1,1),
 OrderTypeId varchar(max) null,
 OrderType varchar(max) null,
 OrderDate datetime null,
 OrderFor varchar(max) null,
 EmployeeId varchar(max) null,
 EmpNo varchar(max) null,
 Attachment varchar(max) null,
 Remarks varchar(max) null,
 StaffName varchar(max) null,
 ProjectId int null,
 OrderNumber varchar(max) null,
 BankAccount varchar(max) null,
 Designation varchar(max) null,
 AppointmentDate datetime null,
 RelievingDate datetime null,
 PayableAmount money null, 
 StaffID int null,
 DepartmentId int null,
 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null)

CREATE TABLE tblOfficeOrderDetail
(
 OrderDetailId int not null primary key identity(1,1),
 SalaryHeadId int null,
 Year int null,
 RevisedSalary money null,
 ArrearFrom datetime null,
 ArrearPaymentDate datetime null,
 Remarks varchar(max) null,
 IsCurrent bit null,
 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null
) 
CREATE TABLE tblSalaryHead
(
  SalaryHeadId int not null primary key identity(1,1),
  OrderType varchar(max) null,
  EmployeeId varchar(max) null,
  EmpNo varchar(max) null,
  Basic money null,
  MA money null,
  HRA money null,
  EPF money null,
  MedicalInsurance money null,
   CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null)

CREATE TABLE tblOtherPayment
(PaymentId int not null primary key identity(1,1),
 EmployeeId varchar(max) null,
 PostedDate datetime null,
 PaymentMonthYear varchar(max) null,
 PaymentCategory varchar(max) null,
 PaidAmount money null,
 IsPaid bit null,
 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null)
 
 
 CREATE TABLE tblSalaryPayment
(PaymentId int not null primary key identity(1,1),
 EmployeeId varchar(max) null,
 EmpNo varchar(max) null,
 Basic money null,
 HRA money null,
 MA money null,
 DA money null,
 Conveyance money null,
 Deduction money null,
 Tax money null,
 ProfTax money null,
 TaxableIncome money null,
 NetSalary money null,
 MonthSalary money null,
 MonthlyTax money null,
 AnnualSalary money null,
 AnnualExemption money null, 
 PaidDate datetime null,
 PaymentMonthYear varchar(max) null,
 PaymentCategory varchar(max) null,
 PaidAmount money null,
 IsPaid bit null,
 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null)
 
 
 CREATE TABLE tblSalaryPaymentHead
(PaymentHeadId int not null primary key identity(1,1),
 PaymentNo int null,
 ProjectNo varchar(max) null,
 Amount money null,
 TypeOfPayBill int null,
 PaidDate datetime null,
 PaymentMonthYear varchar(max) null,
 PaidAmount money null,
 IsPaid bit null,
 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null)
 
 drop table tblSalaryPayment
 
 CREATE TABLE tblSalaryPayment
(PaymentId int not null primary key identity(1,1),
 PaymentHeadId int null,
 ProjectNo varchar(max) null,
 EmployeeId varchar(max) null,
 EmpNo varchar(max) null,
 Basic money null,
 HRA money null,
 MA money null,
 DA money null,
 Conveyance money null,
 Deduction money null,
 Tax money null,
 ProfTax money null,
 TaxableIncome money null,
 NetSalary money null,
 MonthSalary money null,
 MonthlyTax money null,
 AnnualSalary money null,
 AnnualExemption money null, 
 PaidDate datetime null,
 PaymentMonthYear varchar(max) null,
 PaymentCategory varchar(max) null,
 ModeOfPayment int null,
 TypeOfPayBill int null,
 PaidAmount money null,
 IsPaid bit null,
 CreatedAt datetime null,
 UpdatedAt datetime null,
 CreatedBy int null,
 UpdatedBy int null)
 
 alter table dbo.tblOfficeOrderDetail
add OrderId int