using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models
{
    #region CashBook
    public class CashBookModel
    {
        public int Id { get; set; }
        public int AccountHeadId { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public int BOAPaymentDetailId { get; set; }
        public string ReferenceDate { get; set; }
        public string VoucherNumber { get; set; }
        public decimal Amount { get; set; }
        public string PayeeBank { get; set; }
        public int BOAId { get; set; }
        public string TransactionType { get; set; }
        public int BankHeadName { get; set; }
        public int BankHeadID { get; set; }
        public string PayeeName { get; set; }
        public string TransactionTypeCode { get; set; }
        public string VoucherPayee { get; set; }
        public int BankId { get; set; }
        public List<CashBookRecModel> REC { get; set; }
        public List<CashBookPayModel> PAY { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string Bank { get; set; }
        public string FinalOB { get; set; }
        public string FinalCB { get; set; }
        public string username { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public string CashBookDate { get; set; }
        public decimal PaymentTotalAmount { get; set; }
        public decimal ReceiptTotalAmount { get; set; }
    }
    public class CashBookRecModel
    {
        public int AccountHeadId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int BOAPaymentDetailId { get; set; }
        public string ReferenceDate { get; set; }
        public string VoucherNumber { get; set; }
        public decimal Amount { get; set; }
        public string PayeeBank { get; set; }
        public int BOAId { get; set; }
        public string TransactionType { get; set; }
        public int BankHeadID { get; set; }
        public string PayeeName { get; set; }
        public string TransactionTypeCode { get; set; }
        public string VoucherPayee { get; set; }
        public int BankId { get; set; }
        public string TempVoucherNo { get; set; }
        public string ChequeNo { get; set; }
    }
    public class CashBookPayModel
    {
        public int AccountHeadId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int BOAPaymentDetailId { get; set; }
        public string ReferenceDate { get; set; }
        public string VoucherNumber { get; set; }
        public decimal Amount { get; set; }
        public string PayeeBank { get; set; }
        public int BOAId { get; set; }
        public string TransactionType { get; set; }
        public int BankHeadID { get; set; }
        public string PayeeName { get; set; }
        public string TransactionTypeCode { get; set; }
        public string VoucherPayee { get; set; }
        public int BankId { get; set; }
        public string TempVoucherNo { get; set; }

    }
    #endregion
    #region ProcessGuideLineReport
    public class ProcessGuideLineReportModel
    {
        public int User { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FileMovement { get; set; }
        public string Action { get; set; }
        public List<ProcessGuideLineListReportModel> Flow { get; set; }
        public List<ProcessGuideLineHeadingModel> Heading { get; set; }
    }
    public class ProcessGuideLineListReportModel
    {
        public string Name { get; set; }
        public string Action { get; set; }
        public string FunctionName { get; set; }
        public string RefNo { get; set; }
        public string Date { get; set; }
        public string ActionLink { get; set; }
        public string InitatedDate { get; set; }
    }
    public class ProcessGuideLineHeadingModel
    {
        public string Head { get; set; }
        public string RefNumber { get; set; }
        public string Date { get; set; }
    }
    #endregion
    public class ProjectNumModel
    {
        public string ProjectNumber { get; set; }
    }
    public class TransactionTypeModel
    {
        public string TransactionType { get; set; }
    }

    public class TrailBalanceModel
    {
        public string Accounts { get; set; }
        public string Groups { get; set; }
        public int AccountGroupId { get; set; }
        public int AccountHeadId { get; set; }
        public string AccountHead { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public bool Creditor_f { get; set; }
        public bool Debtor_f { get; set; }
        public int BOATransactionId { get; set; }
        public decimal ttlAssetDr { get; set; }
        public decimal ttlAssetCr { get; set; }
        public decimal ttlLiabilityDr { get; set; }
        public decimal ttlLiabilityCr { get; set; }
        public decimal ttlIncomeDr { get; set; }
        public decimal ttlIncomeCr { get; set; }
        public decimal ttlExpenseDr { get; set; }
        public decimal ttlExpenseCr { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public int HeadId { get; set; }

    }
    public class PostingsModel
    {
        public DateTime PostedDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string TransactionTypeCode { get; set; }
        public string TempVoucherNumber { get; set; }
        public string TransType { get; set; }
        public int BOATransactionId { get; set; }
        public int AccountGroupId { get; set; }
        public string Accounts { get; set; }
        public string Groups { get; set; }
        public int AccountHeadId { get; set; }
        public string AccountHead { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public bool Creditor_f { get; set; }
        public bool Debtor_f { get; set; }
    }
    public class AccountTypeModel
    {
        public string FinancialYear { get; set; }
        public int FinancialId { get; set; }
        public int Financial { get; set; }
    }
    public class CommitmentReportModel
    {
        public string ProjectNumber { get; set; }
        public string ProjectId { get; set; }
        public string CommitmentNumber { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ProjectType { get; set; }
        public string ProjectTypeName { get; set; }
        public string CommitmentType { get; set; }
        public DateTime CommitmentDate { get; set; }
        public decimal CommitmentAmount { get; set; }
        public decimal BookedValue { get; set; }
    }
    
    public class ProposalRepotViewModels
    {
        [Required]
        [Display(Name = "From date")]
        public DateTime FromDate { get; set; }
        [Required]
        [Display(Name = "To date")]
        public DateTime ToDate { get; set; }
        [Required]
        [Display(Name = "Project type")]
        public int ProjecttypeId { get; set; }
        public string Proposalnumber { get; set; }
        public string PI { get; set; }
        public string ProposalTitle { get; set; }
        public string Department { get; set; }
        public DateTime InwardDate { get; set; }
        public int Durationofprojectyears { get; set; }
        public int Durationofprojectmonths { get; set; }
        public DateTime Crtd_TS { get; set; }
        public string keysearch { get; set; }

    }
    public class BOATransactionDetailsModels
    {
        public DateTime PostedDate { get; set; }
        public string ProjectNumber { get; set; }
        public string HeadName { get; set; }
        public string CommitmentNumber { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
    }
    public class ProjectTransactionModel
    {
        public string ProjectNo { get; set; }
        public string Title { get; set; }
        public string PI { get; set; }
        public List<string> Copi { get; set; }
        public decimal SantionedValue { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal NegBal { get; set; }
        public decimal NetBalance { get; set; }
        public string ProjNo { get; set; }
        public int ProjId { get; set; }
        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public string RefNo { get; set; }
        public string TransType { get; set; }
        public string Code { get; set; }
        public string FunctionName { get; set; }
        public string Category { get; set; }
        public string CommitmentNumber { get; set; }
    }
    public class VendorReportModel
    {
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string BillType { get; set; }
        public string RefNumber { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public string VendorAddress { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PAN { get; set; }
        public string GSTIN { get; set; }
        public string HSNSAC { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceNo { get; set; }
        public string CommitmentNo { get; set; }
        public string ProjectNo { get; set; }
        public decimal TaxableValue { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal CGSTTDS { get; set; }
        public decimal SGSTTDS { get; set; }
        public decimal IGSTTDS { get; set; }
        public string CommitmentAmount { get; set; }
        public string BudgetHead { get; set; }
        public decimal TotalInvoiceValue { get; set; }
        public decimal TDSValue { get; set; }
        public string TDSSection { get; set; }
        public string Remarks { get; set; }
        public string PaymentStatus { get; set; }
    }
    public class AgencyReportModel
    {
        public int Category { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string PIName { get; set; }
        public string ProjectNumber { get; set; }
        public decimal TaxableValue { get; set; }
        public decimal CGST { get; set; }
        public decimal IGST { get; set; }
        public decimal SGST { get; set; }
        public decimal TotalInvoiceValue { get; set; }
        public string BillerName { get; set; }
        public string GSTIN { get; set; }
        public decimal TotalReceiptsAmount { get; set; }
        public decimal TDSReceivable { get; set; }
        public decimal CreditNote { get; set; }

    }

    #region Travel Bill report
    public class TravelBillReportModel
    {
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public decimal AllocatedValue { get; set; }
        public decimal RecoupmentValue { get; set; }
        public decimal PaymentValue { get; set; }
        public decimal BalanceinAdvance { get; set; }
        public decimal TempAdvanceValue { get; set; }
        public decimal TempSettleValue { get; set; }
        public string StudentName { get; set; }
        public string InstName { get; set; }
        public string RollNo { get; set; }
        public string CourseOfStudy { get; set; }
        public int YearOfStudy { get; set; }
        public string Email { get; set; }
        public string IntenshipDuration { get; set; }
        public string IntenshipStartDate { get; set; }
        public string IntenshipEndDate { get; set; }
        public decimal TotalStipendValue { get; set; }
        public string PayableBankName { get; set; }
        public string AccNo { get; set; }
        public string Branch { get; set; }
        public string IFSC { get; set; }
        public string PI { get; set; }
        public string Department { get; set; }
        public string NameOfReceiver { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjCloseDate { get; set; }
        public string ProjectType { get; set; }
        public int BillId { get; set; }
        public string BillDate { get; set; }
        public string BillMonth { get; set; }
        public string BankAccount { get; set; }
        public string RefNumber { get; set; }
        public decimal TotalBillValue { get; set; }
        public string BillNumber { get; set; }
        public string BillType { get; set; }
        public string PONumber { get; set; }
        public string PODate { get; set; }
        public string ModeOfSupply { get; set; }
        public string BrNO { get; set; }
        public string VoucherNo { get; set; }
        public string CommitmentNo { get; set; }
        public string HeadNo { get; set; }
        public string CheqNo { get; set; }
        public string CheqDate { get; set; }
        public string Rupees { get; set; }
        public string InvoiceDate { get; set; }
        public decimal PayableAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string BillHeading { get; set; }
        public string PrintedDate { get; set; }
        public string PrintedBy { get; set; }
        public string TravelerName { get; set; }
        public decimal DisSalary { get; set; }
        public decimal HonSalary { get; set; }
        public decimal MdySalary { get; set; }
        public decimal FssSalary { get; set; }
        public decimal EucoValue { get; set; }
        public string EucoCode { get; set; }
        public decimal Others { get; set; }
        public string TDSSection { get; set; }
        public string BSRCode { get; set; }
        public decimal GSTInput { get; set; }
        public decimal GSTOutput { get; set; }
        public decimal TDSReceivable { get; set; }
        public decimal RoundOffCredit { get; set; }
        public decimal RoundOffDebit { get; set; }

        public List<PayableListModel> Payable { get; set; }
        public List<CommitListModel> Comm { get; set; }
        public List<TDSITListForTravelModel> TDSIT { get; set; }
        public List<TDSGSTForTravelModel> TDSGST { get; set; }
        public List<GSTForTravelModel> GST { get; set; }
        public List<TravelerVistListModel> TravlerVisit { get; set; }
        public List<StudentListReportModel> StudentList { get; set; }
        public List<TDSPerModel> TDSPerList { get; set; }
        public List<InvoiceListModel> InvoiceList { get; set; }
        public List<HonororiumPCFModel> PCF { get; set; }
        public List<HeadCreditPayeeLedgerModel> PayeeLeadger { get; set; }
    }
    public class PayableListModel
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string AccNo { get; set; }
        public string Bank { get; set; }
        public string IFSC { get; set; }
        public decimal Amount { get; set; }
        public string RollNo { get; set; }
        public string Branch { get; set; }
        public string Particulars { get; set; }
        public string BillNo { get; set; }
        public string PAN { get; set; }
        public string GSTIN { get; set; }
        public int NoOfDays { get; set; }
        public decimal AmountPerdays { get; set; }
        public string Type { get; set; }
        public string PaybillNo { get; set; }
        public string PaymentMode { get; set; }
    }
    public class CommitListModel
    {
        public string Number { get; set; }
        public string ProjNo { get; set; }
        public string Date { get; set; }
        public string StartDate { get; set; }
        public string SchemeCode { get; set; }
        public string ProjectType { get; set; }
        public decimal NetBalance { get; set; }
        public string Head { get; set; }
        public decimal Value { get; set; }
    }
    public class TDSITListForTravelModel
    {
        public string Head { get; set; }
        public decimal Value { get; set; }
    }
    public class TDSGSTForTravelModel
    {
        public string Head { get; set; }
        public decimal Value { get; set; }
        public string Type { get; set; }
    }
    public class GSTForTravelModel
    {
        public string Head { get; set; }
        public decimal Value { get; set; }
    }
    public class TravelerVistListModel
    {
        public string TravlerName { get; set; }
        public string PlaceOfvisit { get; set; }
        public string Duration { get; set; }
    }
    public class StudentListReportModel
    {
        public string StudentName { get; set; }
        public string RollNo { get; set; }
        public string IntenStartDate { get; set; }
        public string IntenEndDate { get; set; }
        public decimal AmountPerDay { get; set; }
        public decimal TotalAmount { get; set; }
    }
    public class TDSPerModel
    {
        public string PayeeName { get; set; }
        public decimal BasicAmt { get; set; }
        public decimal TDSAmt { get; set; }
        public decimal NetAmt { get; set; }
        public string TDSPer { get; set; }
      
    }
    public class InvoiceListModel
    {
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public decimal Amount { get; set; }
        public decimal GSTAmt { get; set; }

    }
    public class HeadCreditPayeeLedgerModel
    {
        public string AccountHead { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string PayeeName { get; set; }
    }
    #endregion
    public class InvoiceReportModel
    {
        public string Amount { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string ProjectNumber { get; set; }
        public string DepartmentName { get; set; }
        public string PIName { get; set; }
        public string SACNumber { get; set; }
        public string IITMGSTIN { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public string PinCode { get; set; }
        public string State { get; set; }
        public string StateCode { get; set; }
        public string GSTIN { get; set; }
        public string PANNo { get; set; }
        public string TANNo { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string DescriptionofServices { get; set; }
        public decimal TaxableValue { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal TotalInvoiceValue { get; set; }
        public string TotalInvoiceValueInWords { get; set; }
        public string ACName { get; set; }
        public string ACNo { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string IFSC { get; set; }
        public string MICRCode { get; set; }
        public string SWIFTCode { get; set; }
        public string TaxableValue1 { get; set; }
        public string CreditNoteNo { get; set; }
        public string CreditNoteDate { get; set; }
        public string IGSTPct { get; set; }
        public string SGSTPct { get; set; }
        public string CGSTPct { get; set; }
        public string CGSTstr { get; set; }
        public string SGSTstr { get; set; }
        public string IGSTstr { get; set; }
        public bool SignNR_f { get; set; }
    }
    #region AnnualAccounts Report
    public class AnnualAccounts
    {
        //public Nullable<DateTime> FromDate { get; set; }
        //public Nullable<DateTime> ToDate { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectType { get; set; }
        public decimal OB { get; set; }
        public decimal TR { get; set; }
        public decimal TTX { get; set; }
        public decimal OOH { get; set; }
        public decimal OH { get; set; }
        public decimal Staff { get; set; }
        public decimal Equipment { get; set; }
        public decimal Consumables { get; set; }
        public decimal Contingencies { get; set; }
        public decimal Travel { get; set; }
        public decimal Components { get; set; }
        public decimal InsOverhead { get; set; }
        public decimal Others { get; set; }
        public decimal Miscellaneous { get; set; }
        public decimal NR { get; set; }
        public decimal CB { get; set; }
        public string startDate { get; set; }
        public string CloseDate { get; set; }
    }
    #endregion
    #region Provisional Statement
    public class ProvisionalStatementReportModel
    {
        public string LoginTS { get; set; }
        public string SponseringAgency { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectTitle { get; set; }
        public string SanctionNo { get; set; }
        public string ProjCor { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public decimal OB { get; set; }
        public decimal Receipt { get; set; }
        public decimal ReceiptTotal { get; set; }
        public decimal Interest { get; set; }
        public decimal Staff { get; set; }
        public decimal Equipment { get; set; }
        public decimal Consumables { get; set; }
        public decimal Contingency { get; set; }
        public decimal Travel { get; set; }
        public decimal Components { get; set; }
        public decimal Overheads { get; set; }
        public decimal Others { get; set; }
        public decimal Miscellaneous { get; set; }
        public decimal TotalExpenditure { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceCommitments { get; set; }
        public decimal NetBalance { get; set; }

    }
    #endregion
    
    #region Receipt Voucher
    public class ReceiptVoucherModel
    {
        public int ReceiptId { get; set; }
        public decimal ReceiptAmt { get; set; }
        public string ReceiptNo { get; set; }
        public string ReceiptDate { get; set; }
        public string ProjectNo { get; set; }
        public string Dept { get; set; }
        public string InvoiceNo { get; set; }
        public string PIName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public decimal InvoiceAmt { get; set; }
        public string TransDate { get; set; }
        public string TransNo { get; set; }
        public string Bank { get; set; }
        public decimal BankAmt { get; set; }
        public List<Receivables> RecList { get; set; }
        public ProjectSummaryModel projsumm { get; set; }
        public List<ReceiptJournal> Journal { get; set; }
        public string Category { get; set; }
        public string BillRefNumber { get; set; }
        public decimal InvoiceValueInFRNCurrency { get; set; }
        public string InvoiceCurrencyCode { get; set; }
    }
    public class Receivables
    {
        public string Head { get; set; }
        public decimal Amount { get; set; }
    }
    public class ReceiptJournal
    {
        public string Head { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
    }
    public class BankTransferDetails
    {
        public string TransactionType { get; set; }
        public string BankName { get; set; }
    }
    #endregion
    
    #region Long Bill
    public class LongBill
    {
        public decimal BASICPAY { get; set; }
        public decimal MEDINS { get; set; }
        public decimal MEDCHARG { get; set; }
        public decimal MISCPAYNT { get; set; }
        public decimal CFACC { get; set; }
        public decimal CCALL { get; set; }
        public decimal OTHERPAY { get; set; }
        public decimal OTALL { get; set; }
        public decimal TRANSALL { get; set; }
        public decimal LIC { get; set; }
        public decimal INCOMETAX { get; set; }
        public decimal TEMPLE { get; set; }
        public decimal AYYAPPA { get; set; }
        public decimal MISCRECVY { get; set; }
        public decimal PEELI { get; set; }
        public decimal CANBANK { get; set; }
        public decimal PAYRECVRY { get; set; }
        public decimal PROFLTAX { get; set; }
        public decimal COURT { get; set; }
        public decimal PMRELFND { get; set; }
        public decimal ITOTHPAY { get; set; }
        public decimal PAYMENTS { get; set; }
        public decimal RECOVERIES { get; set; }
        public decimal NETPAYABLE { get; set; }
        public decimal CANARABANK { get; set; }
        public decimal OTHERBANK { get; set; }
        public decimal CHEQUE { get; set; }
        public decimal IITMPENSIONER { get; set; }
        public string AmtWords { get; set; }
        public decimal PaymentTotal { get; set; }
        public decimal RecoveryTotal { get; set; }
    }
    #endregion
    #region OfficeMonthly Report
    public class OfficeMonthlyReportModel
    {
        public decimal TotalAmount { get; set; }
        public decimal TotalPrevAmt { get; set; }
        public decimal ExTotalAmount { get; set; }
        public decimal ExTotalPrevAmt { get; set; }
        public string CurrMonth { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string PrevDate { get; set; }
        public string months { get; set; }
        public List<OfficeMonthlyListReportModel> List { get; set; }
        public List<OfficeMonthlyCenterListReportModel> CenterList { get; set; }
    }
    public class OfficeMonthlyListReportModel
    {
        public string months { get; set; }
        public string DepartmentName { get; set; }
        public string Type { get; set; }
        public int NoOfProjCurMonth_A { get; set; }
        public int NoOfProjPrevMonth_A { get; set; }
        public int TotalProj_A { get; set; }
        public string AmtforCurMonth_A { get; set; }
        public string AmtforPrevMonth_A { get; set; }
        public decimal TotalAmt_A { get; set; }
        public string ExAmtforCurMonth_A { get; set; }
        public string ExAmtforPrevMonth_A { get; set; }
        public decimal ExTotalAmt_A { get; set; }


        public int NoOfProjCurMonth_B { get; set; }
        public int NoOfProjPrevMonth_B { get; set; }
        public int TotalProj_B { get; set; }
        public string AmtforCurMonth_B { get; set; }
        public string AmtforPrevMonth_B { get; set; }
        public decimal TotalAmt_B { get; set; }
        public string ExAmtforCurMonth_B { get; set; }
        public string ExAmtforPrevMonth_B { get; set; }
        public decimal ExTotalAmt_B { get; set; }

        public int NoOfProjCurMonth_C { get; set; }
        public int NoOfProjPrevMonth_C { get; set; }
        public int TotalProj_C { get; set; }
        public string AmtforCurMonth_C { get; set; }
        public string AmtforPrevMonth_C { get; set; }
        public decimal TotalAmt_C { get; set; }
        public string ExAmtforCurMonth_C { get; set; }
        public string ExAmtforPrevMonth_C { get; set; }
        public decimal ExTotalAmt_C { get; set; }

        public int NoOfProjCurMonth_D { get; set; }
        public int NoOfProjPrevMonth_D { get; set; }
        public int TotalProj_D { get; set; }
        public string AmtforCurMonth_D { get; set; }
        public string AmtforPrevMonth_D { get; set; }
        public decimal TotalAmt_D { get; set; }
        public string ExAmtforCurMonth_D { get; set; }
        public string ExAmtforPrevMonth_D { get; set; }
        public decimal ExTotalAmt_D { get; set; }

        public decimal AmtforRecCurMonth { get; set; }
        public decimal AmtforRecPrevMonth { get; set; }
        public decimal TotalRecAmt { get; set; }
        public decimal TotalPrevAmt { get; set; }
        public decimal ExAmtforRecCurMonth { get; set; }
        public decimal ExAmtforRecPrevMonth { get; set; }
        public decimal ExTotalRecAmt { get; set; }
        public decimal ExTotalPrevAmt { get; set; }
    }
    public class OfficeMonthlyCenterListReportModel
    {
        public string months { get; set; }
        public string DepartmentName { get; set; }
        public int NoOfProjCurMonth { get; set; }
        public int NoOfProjPrevMonth { get; set; }
        public int TotalProj { get; set; }
        public decimal AmtforCurMonth { get; set; }
        public decimal AmtforPrevMonth { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal AmtforRecCurMonth { get; set; }
        public decimal AmtforRecPrevMonth { get; set; }
        public decimal TotalRecAmt { get; set; }
    }
    #endregion

    #region Receipt Report
    public class ReceiptReportModel
    {
        public string Month { get; set; }
        public int NoOfClm { get; set; }
        public List<ReceiptReportListModel> List { get; set; }
        public List<MonthListReportModel> MonthList { get; set; }
        public List<ReceiptReportBasedonScheme> SchemeList { get; set; }
        public List<SponSchemeList> SponScheme { get; set; }
        public List<ConsSchemeList> ConsScheme { get; set; }
    }
    public class ReceiptReportListModel
    {
        public string values { get; set; }
    }
    public class SponSchemeList
    {
        public string Name { get; set; }
        public int PrevNoOfProject { get; set; }
        public string PrevValue { get; set; }
        public int CurrNoOfProject { get; set; }
        public string CurrValue { get; set; }
    }
    public class MonthListReportModel
    {
        public string Month { get; set; }
    }
    public class ConsReceiptReportModel
    {
        public string Month { get; set; }
        public int NoOfClm { get; set; }
        public List<ConsReceiptReportListModel> List { get; set; }
        public List<ConsMonthListReportModel> MonthList { get; set; }
        public List<ConsSchemeList> ConsScheme { get; set; }
    }
    public class ConsReceiptReportListModel
    {
        public string values { get; set; }
        public string Exvalues { get; set; }
    }
    public class ConsSchemeList
    {
        public string Name { get; set; }
        public int PrevNoOfProject { get; set; }
        public string PrevValue { get; set; }
        public string ExPrevValue { get; set; }
        public int CurrNoOfProject { get; set; }
        public string CurrValue { get; set; }
        public string ExCurrValue { get; set; }
    }
    public class ConsMonthListReportModel
    {
        public string Month { get; set; }
    }
    public class ReceiptReportBasedonScheme
    {
        public string Name { get; set; }
        public int PrevNoOfProject { get; set; }
        public string PrevValue { get; set; }
        public string ExPrevValue { get; set; }
        public int CurrNoOfProject { get; set; }
        public string CurrValue { get; set; }
        public string ExCurrValue { get; set; }

    }
    #endregion

    #region TravelReportmodel
    public class TravelReportModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public Nullable<int> TravelType { get; set; }
        public string BillNumber { get; set; }
        public string PayeeName { get; set; }
    }
    #endregion
    #region Form 24Q
    public class Form24QModel
    {
        public DateTime Date { get; set; }
        public List<TDS94CModel> TDS94C { get; set; }
        public List<TDS94JModel> TDS94J { get; set; }
        public List<TDS94HModel> TDS94H { get; set; }
        public List<TDS94IModel> TDS94I { get; set; }
    }
    public class TDS94CModel
    {
        public int SlNo { get; set; }
        public string PaymentMonth { get; set; }
        public string Section { get; set; }
        public string PAN { get; set; }
        public string Name { get; set; }
        public string EmployeeId { get; set; }
        public DateTime DateofPayment { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Deduction { get; set; }
        public decimal EducationFees { get; set; }
        public decimal TDS { get; set; }
        public string BSRCode { get; set; }
        public string ChallenNo { get; set; }
        public string DateofDeposit { get; set; }
        public decimal Rate { get; set; }
    }
    public class TDS94JModel
    {
        public int SlNo { get; set; }
        public string Section { get; set; }
        public string PAN { get; set; }
        public string Name { get; set; }
        public DateTime DateofPayment { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TDS { get; set; }
        public string BSRCode { get; set; }
        public string ChallenNo { get; set; }
        public string DateofDeposit { get; set; }
        public decimal Rate { get; set; }
    }
    public class TDS94HModel
    {
        public int SlNo { get; set; }
        public string Section { get; set; }
        public string PAN { get; set; }
        public string Name { get; set; }
        public DateTime DateofPayment { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TDS { get; set; }
        public string BSRCode { get; set; }
        public string ChallenNo { get; set; }
        public string DateofDeposit { get; set; }
        public decimal Rate { get; set; }
    }
    public class TDS94IModel
    {
        public int SlNo { get; set; }
        public string Section { get; set; }
        public string PAN { get; set; }
        public string Name { get; set; }
        public DateTime DateofPayment { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TDS { get; set; }
        public string BSRCode { get; set; }
        public string ChallenNo { get; set; }
        public string DateofDeposit { get; set; }
        public decimal Rate { get; set; }
    }
    #endregion
    #region Interest Refund
    public class InterestRefundReportModel
    {
        public string ProjectNumber { get; set; }
        public string FinancialYear { get; set; }
        [Required]
        [Display(Name = "Financial Year")]
        public int FinYearId { get; set; }
        public List<InterestRefundMonthReport> Monthlist { get; set; }
        public decimal TotalIntrestAMT { get; set; }
        public decimal InterestPct { get; set; }
        public bool CommitmentIncl_f { get; set; }
    }
    public class InterestRefundMonthReport
    {
        public int SNo { get; set; }
        public string Month { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ReceiptReceived { get; set; }
        public decimal ExpenditureTotal { get; set; }
        public decimal CommitmentTotal { get; set; }
        public decimal ActualBalance { get; set; }
        public decimal ReceiptNotConsideredforinterest { get; set; }
        public decimal AmountEligibleforInterest { get; set; }
        public decimal InterestRateperAnnum { get; set; }
        public decimal InterestAmount { get; set; }
        public int FinYear { get; set; }
        public DateTime FromDate { get; set; }
    }
    #endregion
    public class ProjectExpDateModel
    {
        public int Id { get; set; }
        public string label { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

  
    #region ICSROH Report
    public class ICSROHReportModel
    {
        public int FinancialYear { get; set; }
        public decimal CONSCorpus { get; set; }
        public decimal CONSDist { get; set; }
        public decimal Interest { get; set; }
        public decimal SPONCorpus { get; set; }
        public decimal OtherIncome { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal Salary { get; set; }
        public decimal InterestExp { get; set; }
        public decimal RepairsMaint { get; set; }
        public decimal ITEquipment { get; set; }
        public decimal OtherExpenses { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get; set; }

        //

        public string CONSCorpusPct { get; set; }
        public string CONSDistPct { get; set; }
        public string InterestPct { get; set; }
        public string SPONCorpusPct { get; set; }
        public string OtherIncomePct { get; set; }
        public string TotalIncomePct { get; set; }
        public string SalaryPct { get; set; }
        public string InterestExpPct { get; set; }
        public string RepairsMaintPct { get; set; }
        public string ITEquipmentPct { get; set; }
        public string OtherExpensesPct { get; set; }
        public string TotalExpensesPct { get; set; }
        public string NetIncomePct { get; set; }
        public ICSROHReportListModel ICSROHlist { get; set; }
    }
    public class ICSROHReportListModel
    {
        public int FinancialYear { get; set; }
        public List<decimal> CONSCorpus { get; set; }
        public List<decimal> CONSDist { get; set; }
        public List<decimal> Interest { get; set; }
        public List<decimal> SPONCorpus { get; set; }
        public List<decimal> OtherIncome { get; set; }
        public List<decimal> TotalIncome { get; set; }
        public List<decimal> Salary { get; set; }
        public List<decimal> InterestExp { get; set; }
        public List<decimal> RepairsMaint { get; set; }
        public List<decimal> ITEquipment { get; set; }
        public List<decimal> OtherExpenses { get; set; }
        public List<decimal> TotalExpenses { get; set; }
        public List<decimal> NetIncome { get; set; }


    }
    #endregion

    #region RF Report
    public class RFreportModel
    {
        public int Id { get; set; }

        public List<RFYearModel> Yearlist { get; set; }
        public List<RFYearValueModel> YearValuelist { get; set; }


        public List<RFAgencyModel> Agencylist { get; set; }
        public List<RFAgencyValueModel> AgencyValuelist { get; set; }

        public List<RFDepartModel> Departlist { get; set; }
        public List<RFDepartValueModel> DepartValuelist { get; set; }
    }
    public class RFYearModel
    {
        public string Year { get; set; }
    }
    public class RFYearValueModel
    {
        public List<decimal> values { get; set; }
    }
    public class RFAgencyModel
    {
        public string Agency { get; set; }
    }
    public class RFAgencyValueModel
    {
        public List<decimal> values { get; set; }
    }
    public class RFDepartModel
    {
        public string Departments { get; set; }
    }
    public class RFDepartValueModel
    {
        public List<decimal> values { get; set; }
    }
    #endregion
    #region NIRF Report
    public class NIRFReportModel
    {
        public int Id { get; set; }
    }
    #endregion
    #region SOE
    public class SOEmodel
    {
        public int Id { get; set; }
        public List<MonthandYearExp> MonthandYearExpList { get; set; }
    }
    public class MonthandYearExp
    {
        public string Year { get; set; }
        public decimal Amount { get; set; }
        public decimal IntersetAmount { get; set; }
    }
    #endregion
    #region Old Receipt and Exp
    public class OldReceiptModel
    {
        public int ProjectId { get; set; }
        public string FinYear { get; set; }
        public string ProjectNo { get; set; }
        public string ReceiptNo { get; set; }
        public string AgencyName { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string coordinator { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal OH { get; set; }
        public DateTime Date { get; set; }
    }
    public class OldExpModel
    {
        public int ProjectId { get; set; }
        public string FinYear { get; set; }
        public string ProjectNo { get; set; }
        public string RefNo { get; set; }
        public string CommitmentNo { get; set; }
        public string Head { get; set; }
        public string PayeeName { get; set; }       
        public decimal TotalAmount { get; set; }
        public decimal Tax { get; set; }
        public decimal TDS { get; set; }
        public decimal NetAmount { get; set; }
        public DateTime Date { get; set; }
    }
    #endregion

    #region Balance Sheet
    public class BalanceSheetModel
    {
       public decimal DecGrossAmt { get; set; }
       public string GrossAmt { get; set; }
        public decimal PrevDecGrossAmt { get; set; }
        public string PrevGrossAmt { get; set; }
        public decimal CurrDecGrossAmt { get; set; }
        public string CurrGrossAmt { get; set; }

        public List<BalanceSheetNoteModel> Note { get; set; }
    }
    public class BalanceSheetNoteModel
    {
        public string Head { get; set; }
       public string Amount { get; set; }
        public string PrevAmount { get; set; }
        public string CurrAmount { get; set; }
        public decimal  DecimalCurrAmount { get; set; }
    }

    #endregion
    public class InvoiceReportPrintModel
    {
        public string Amount { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string ProjectNumber { get; set; }
        public string DepartmentName { get; set; }
        public string PIName { get; set; }
        public string SACNumber { get; set; }
        public string IITMGSTIN { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PONumber { get; set; }
        public string District { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string State { get; set; }
        public string StateCode { get; set; }
        public string GSTIN { get; set; }
        public string PANNo { get; set; }
        public string TANNo { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string DescriptionofServices { get; set; }
        public string TaxableValue { get; set; }
        public string CGST { get; set; }
        public string SGST { get; set; }
        public string IGST { get; set; }
        public string CGSTPct { get; set; } = "0";
        public string SGSTPct { get; set; } = "0";
        public string IGSTPct { get; set; } = "0";
        public string TotalInvoiceValue { get; set; }
        public string TotalInvoiceValueInWords { get; set; }
        public string ACName { get; set; }
        public string ACNo { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string IFSC { get; set; }
        public string MICRCode { get; set; }
        public string SWIFTCode { get; set; }
        public bool SignNR_f { get; set; }
        public bool Watermark_f { get; set; }
    }
}