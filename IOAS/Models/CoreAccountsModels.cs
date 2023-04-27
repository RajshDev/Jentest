using Foolproof;
using IOAS.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IOAS.Models
{

    #region Payment
    #region PartTimePayment

    public class PartTimePaymentModel : CommonPaymentModel
    {
        public int PartTimeStudentId { get; set; }
        public int PartTimePaymentId { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }
        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }
        [Required]
        public Nullable<Int32> Source { get; set; }
        [Required]
        [Display(Name = "Project")]
        public Nullable<int> ProjectId { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public Nullable<Int32> BankId { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }
        public string BankName { get; set; }

        [Required]
        public Nullable<int> PIId { get; set; }
        public string PIName { get; set; }
        public string ProjectNumber { get; set; }
        public string PartTimePaymentNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Stipend Value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalStipendValue { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public int SlNo { get; set; }
        public Nullable<int> TotalnoofStudents { get; set; }
        public List<StudentListModel> StudentDetails { get; set; }
        public PartTimePaymentSearchFieldModel SearchField { get; set; }

        public PagedData<PartTimePaymentSearchResultModel> SearchResult { get; set; }
        public bool BillProcessingStatus { get; set; }
        public bool PFInit { get; set; }
        public string SourceName { get; set; }
        public string SourceBillEmailDate { get; set; }
    }
    public class StudentListModel
    {

        public Nullable<int> Session { get; set; }
        [Display(Name = "Name")]
        public Nullable<Int32> UserId { get; set; }
        public string Department { get; set; }
        public string Name { get; set; }
        public string StudentAddress { get; set; }
        public string StudentRollNo { get; set; }
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email address")]
        public string StudentEmail { get; set; }
        public string StudentCourse { get; set; }
        public Nullable<int> StudentYearofStudy { get; set; }
        public Nullable<int> Duration { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Stipend Requested")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> StipendValueperHour { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Stipend Requested")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> StipendValue { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchName { get; set; }
        public string IFSCCode { get; set; }
        public Nullable<DateTime> InternshipFromDate { get; set; }
        public Nullable<DateTime> InternshipToDate { get; set; }
        public string InternFromDate { get; set; }
        public string InternToDate { get; set; }
        public string SessionName { get; set; }
    }
    public class SearchPartTimePaymentModel
    {
        public string SearchINPIName { get; set; }
        //public string SearchINStudentName { get; set; }
        //public string SearchINCollegeName { get; set; }
        public string SearchINProjectNumber { get; set; }
        public Nullable<int> SearchINNoofStudents { get; set; }
        public string SearchINInternshipNumber { get; set; }
        public Nullable<decimal> SearchINAmount { get; set; }
        public string SearchEXPIName { get; set; }
        //public string SearchEXStudentName { get; set; }
        //public string SearchEXCollegeName { get; set; }
        public string SearchEXProjectNumber { get; set; }
        public string SearchEXInternshipNumber { get; set; }
        public Nullable<decimal> SearchEXAmount { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public Nullable<DateTime> FromSMIDate { get; set; }
        public Nullable<DateTime> ToSMIDate { get; set; }
        public string SearchINStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<PartTimePaymentModel> PTPList { get; set; }
    }
    public class PartTimePaymentSearchResultModel
    {
        public Nullable<int> SlNo { get; set; }
        public Nullable<int> PartTimePaymentId { get; set; }
        public Nullable<int> TotalnoofStudents { get; set; }
        public string PIName { get; set; }
        public string StudentName { get; set; }
        public Nullable<int> Projectid { get; set; }
        public string ProjectNumber { get; set; }
        public string PartTimePaymentNumber { get; set; }
        public string College { get; set; }
        public string Duration { get; set; }
        public string Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalStipendValue { get; set; }

    }
    public class PartTimePaymentSearchFieldModel
    {
        public string PIName { get; set; }
        public string StudentName { get; set; }
        public string CollegeName { get; set; }
        public string ProjectNumber { get; set; }
        public string PartTimePaymentNumber { get; set; }
        public string SearchBy { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public Nullable<DateTime> FromSIDate { get; set; }
        public Nullable<DateTime> ToSIDate { get; set; }

    }
    #endregion
    #region Purchase Order
    public class BillEntryModel : CommonPaymentModel
    {
        public int BillId { get; set; }


        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }

        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }

        [Required]
        public Nullable<Int32> Source { get; set; }

        public string BillNumber { get; set; }

        [Required]
        [Display(Name = "Vendor")]
        public Nullable<Int32> VendorId { get; set; }

        [Required]
        [Display(Name = "PO Date")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> PODate { get; set; }

        //[Required]
        [Display(Name = "PO Type")]
        public Nullable<Int32> POType { get; set; }

        [RequiredIfNot("PaymentType", 2, ErrorMessage = "PO Number field is required")]
        [Display(Name = "PO Number")]
        public string PONumber { get; set; }

        [Required]
        //[RequiredIfNot("TransactionTypeCode", "STM", ErrorMessage = "Invoice Date field is required.")]
        [Display(Name = "Pro-forma Invoice Date")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> InvoiceDate { get; set; }

        [Required]
        //[RequiredIfNot("TransactionTypeCode", "STM", ErrorMessage = "Invoice Number field is required.")]
        [Display(Name = "Pro-forma Invoice Number")]
        public string InvoiceNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Invoice Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> InvoiceAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Bank Guarantee Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> BankGuaranteeAmount { get; set; }

        public string BankGuaranteeRemarks { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Invoice Tax Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> InvoiceTaxAmount { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Advance Percentage")]
        [Range(1, 100)]
        public Nullable<decimal> AdvancePercentage { get; set; }
        public bool ExpenseRequired
        {
            get
            {
                return (this.isHaveElgGST && this.AdvancePercentage != 100 && this.InclusiveOfTax_f);
            }
        }
        [RequiredIf("PaymentType", 2, ErrorMessage = "PO Number field is required")]
        public Nullable<Int32> selPONumber { get; set; }
        public string GST { get; set; }
        public bool isHaveElgGST { get; set; }
        public Nullable<Int32> PaymentType { get; set; }

        [Required]
        [Display(Name = "Bank")]
        public Nullable<Int32> BankHead { get; set; }

       
        [Display(Name = "Bank Name")]
        public Nullable<Int32> BankId { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }

        [Required]
        [Display(Name = "Bill Type")]
        public Nullable<Int32> BillType { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Bill Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> BillAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Bill Tax Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> BillTaxAmount { get; set; }

        public string Status { get; set; }
        [StringLength(200, ErrorMessage = "The Remarks cannot exceed 200 characters. ")]
        public string Remarks { get; set; }
        public string Vendor { get; set; }
        public string PostedDate { get; set; }
        public int SlNo { get; set; }
        public Nullable<Int32> VendorTDSDetailId { get; set; }
        [RequiredIfNot("VendorTDSDetailId", null, ErrorMessage = "TDS Basic Value field is required.")]
        public Nullable<decimal> TDSBasicAmt { get; set; }

        public Nullable<decimal> TDSGSTBasicAmt { get; set; }
        public Nullable<decimal> hiddenSettAmt { get; set; }

        public Nullable<decimal> hiddenSettTaxAmt { get; set; }
        public Nullable<decimal> hiddenTaxEligibleAmt { get; set; }
        public bool PartAdvance_f { get; set; }
        public bool InclusiveOfTax_f { get; set; } = true;
        public bool ICSROverHead_f { get; set; }
        public bool RCM_f { get; set; }
        public List<BillPODetailModel> PODetail { get; set; } = new List<BillPODetailModel>();

        public List<VendorInvoiceBreakUpDetailModel> InvoiceBreakDetail { get; set; } = new List<VendorInvoiceBreakUpDetailModel>();
        public bool BillProcessingStatus { get; set; }
    }

    public class BillPODetailModel
    {
        public Nullable<Int32> BillPODetailId { get; set; }
        [Required]
        [Display(Name = "Type of Service / Item Category")]
        public Nullable<int> TypeOfServiceOrCategory { get; set; }
        public bool Service_f { get; set; }
        public string Description { get; set; }
        //[RequiredIf("Service_f", false, ErrorMessage = "UOM field is required")]
        public Nullable<Int32> UOM { get; set; }
        //[RequiredIf("Service_f", false, ErrorMessage = "Qty field is required")]
        public Nullable<Decimal> Quantity { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Tax Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TaxAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Advance Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> AdvanceAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Net Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> NetAmount { get; set; }

        public string ItemName { get; set; }       

        public Nullable<decimal> TaxPct { get; set; }

        public bool IsTaxEligible { get; set; }
    }
    public class AdvanceBillSearchModel
    {
        public string SearchBillNumber { get; set; }
        public string SearchVendor { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> Billamount { get; set; }
        public Nullable<decimal> Settlementamount { get; set; }
        public int TotalRecords { get; set; }
        public List<BillEntryModel> Billentry { get; set; }
    }
    public class BillEntryModelViewModel : CommonPaymentModel
    {
        public string RequestReference { get; set; }
        public string ReferenceNumber { get; set; }
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string VendorName { get; set; }
        public string TdsSection { get; set; }
        public string PoNumber { get; set; }
        public Nullable<DateTime> PoDate { get; set; }
        public string ProformaInvoiceNumber { get; set; }
        public Nullable<DateTime> ProformaInvoiceDate { get; set; }
        public string BillType { get; set; }
        public string PanNumber { get; set; }
        public string GstinNumber { get; set; }
        public string Address { get; set; }
        public decimal Advance { get; set; }
        public Nullable<int> Source { get; set; }
        public string SourceName { get; set; }
        public Nullable<int> SoureceReference { get; set; }
        public Nullable<DateTime> SourceEmailDate { get; set; }
        public string BillNumber { get; set; }
        public string BankHeadName { get; set; }
        public decimal AdvancePercentage { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<decimal> InvoiceTaxAmount { get; set; }
        public Nullable<decimal> InvoiceAmount { get; set; }
        public string PaymentType { get; set; }
        public bool PartAdvance_f { get; set; }
        public bool InclusiveOfTax_f { get; set; }
        public bool ICSROverHead_f { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<decimal> hiddenSettAmt { get; set; }
        public Nullable<decimal> hiddenSettTaxAmt { get; set; }
        public Nullable<decimal> hiddenTaxEligibleAmt { get; set; }
        public string GST { get; set; }
        public int BillId { get; set; }
        public Nullable<Int32> BillTypes { get; set; }
        public Nullable<decimal> BillTaxAmount { get; set; }
        public Nullable<decimal> BillAmount { get; set; }
        public List<BillPODetailModel> PODetail { get; set; }
        public List<VendorInvoiceBreakUpDetailModel> InvoiceBreakDetail { get; set; } = new List<VendorInvoiceBreakUpDetailModel>();
        public string SourceBillEmailDate { get; set; }
        public string BillPoDate { get; set; }
        public string ProInvoiceDate { get; set; }
        public decimal TdsITBaseValue { get; set; }
        public decimal TDSGstBasicValue { get; set; }
        public decimal BankGuaranteeAmount { get; set; }
        public string BankGuaranteeRemarks { get; set; }
        public string Remarks { get; set; }
        public string PoType { get; set; }
        public decimal TDSBasicAmt { get; set; }
        public bool PFInit { get; set; }
        public bool RCM_f { get; set; }

        //Vinoth Changes for adding Account details
        public string VendorBankDetails { get; set; }
        public string BankName { get; set; }
    }


 
    #endregion
    #region Travel

    public class TravelAdvanceModel
    {
        public int TravelBillId { get; set; }

        public string TransactionTypeCode { get; set; }
        public Nullable<decimal> OverallExpense { get; set; }

        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }

        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }

        [Required]
        public Nullable<Int32> Source { get; set; }

        [Required(ErrorMessage = "Please select proper PI from the list")]
        public Nullable<Int32> PI { get; set; }

        public string PIName { get; set; }

        public string BillNumber { get; set; }
        [Required]
        public string Place { get; set; }
        [Required]
        public string Purpose { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Estimate Value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> EstimatedValue { get; set; }

        [Required]
        [LessThanOrEqualTo("EstimatedValue", ErrorMessage = "Advance Requested should not be Greater than Estimate Value.")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Advance Requested")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> AdvanceValue { get; set; }
        [Required]
        [Display(Name = "From Date")]
        public Nullable<DateTime> TravelFromDate { get; set; }
        [Required]
        [Display(Name = "To Date")]
        public Nullable<DateTime> TravelToDate { get; set; }
        [Required]
        [Display(Name = "No of Traveller")]
        public Nullable<int> NoOfTraveller { get; set; } = 1;
        public string Remarks { get; set; }
        public string Status { get; set; }
        public int SlNo { get; set; }
        public string RequestedDate { get; set; }
        [Required]
        public Nullable<int>[] CountryId { get; set; }
        [Required]
        [Display(Name = "Category")]
        public Nullable<int>[] CategoryId { get; set; }
        [Display(Name = "Traveller Name")]
        public string[] TravellerId { get; set; }
        [Display(Name = "Traveller Name")]
        public string[] TravellerName { get; set; }
        public bool Adv100Pct_f { get; set; }
        public bool ProformaInvoiceSubmit_f { get; set; }
        public Nullable<decimal> AdvanceValueWOClearanceAgent { get; set; }
        public List<AttachmentDetailModel> DocumentDetail { get; set; } = new List<AttachmentDetailModel>();
        public string TravelstrDate { get; set; }
        public string TravelfinDate { get; set; }
        public bool BillProcessingStatus { get; set; }
        public string SourceName { get; set; }
        public string[] CategoryName { get; set; }
        public string CountryName { get; set; }
        public string SourceBillEmailDate { get; set; }
        public string TravelBillFromDate { get; set; }
        public string TravelBillToDate { get; set; }
        public bool PFInit { get; set; }

        public Nullable<Int32> BankId { get; set; }

    }

    public class TravelAdvanceBillEntryModel : CommonPaymentModel
    {
        public int TravelBillId { get; set; }
        public Nullable<decimal> OverallExpense { get; set; }

        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }

        public string ReferenceNumber { get; set; }

        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }

        [Required]
        public Nullable<Int32> Source { get; set; }

        [Required(ErrorMessage = "Please select proper PI from the list")]
        public Nullable<Int32> PI { get; set; }

        public string PIName { get; set; }

        public string BillNumber { get; set; }
        [Required]
        public string Place { get; set; }
        [Required]
        public string Purpose { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Estimate Value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> EstimatedValue { get; set; }

        [Required]
        [LessThanOrEqualTo("EstimatedValue", ErrorMessage = "Advance Requested should not be Greater than Estimate Value.")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Advance Requested")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> AdvanceValue { get; set; }
        [Required]
        [Display(Name = "From Date")]
        public Nullable<DateTime> TravelFromDate { get; set; }
        [Required]
        [Display(Name = "To Date")]
        public Nullable<DateTime> TravelToDate { get; set; }
        [Required]
        [Display(Name = "No of Traveller")]
        public Nullable<int> NoOfTraveller { get; set; } = 1;
        public string Remarks { get; set; }
        public string Status { get; set; }
        public int SlNo { get; set; }
        public string RequestedDate { get; set; }
        [Required]
        public Nullable<int>[] CountryId { get; set; }
        [Required]
        [Display(Name = "Category")]
        public Nullable<int>[] CategoryId { get; set; }
        [Display(Name = "Traveller Name")]
        public string[] TravellerId { get; set; }
        [Display(Name = "Traveller Name")]
        public string[] TravellerName { get; set; }
        public bool Adv100Pct_f { get; set; }
        public bool ProformaInvoiceSubmit_f { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "TDS Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PaymentTDSAmount { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Payment Break Up Total")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PaymentBUTotal { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "GST Offset Total")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> GSTOffsetTotal { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Invoice Break Up Total")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> InvoiceBUTotal { get; set; }


        [Required]
        [Display(Name = "Bank Name")]
        public Nullable<Int32> BankId { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }
        public string BankHeadName { get; set; }


        public List<InvoiceBreakUpDetailModel> InvoiceBreakDetail { get; set; } = new List<InvoiceBreakUpDetailModel>();
        public List<PaymentBreakUpDetailModel> PaymentBreakDetail { get; set; } = new List<PaymentBreakUpDetailModel>();
        public string SourceName { get; set; }
        public string[] CategoryName { get; set; }
        public string CountryName { get; set; }
        public string SourceBillEmailDate { get; set; }
        public string TravelBillFromDate { get; set; }
        public string TravelBillToDate { get; set; }
        public bool PFInit { get; set; }
    }

    public class TravelSettlementModel : CommonPaymentModel
    {
        public int TravelBillId { get; set; }

        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }
        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }

        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }

        [Required]
        public Nullable<Int32> Source { get; set; }

        public bool ADVSettlement_f { get; set; } = true;
        public Nullable<decimal> PayableValue { get; set; }
        public Nullable<decimal> OverallExpense { get; set; }

        [RequiredIf("ADVSettlement_f", true, ErrorMessage = "Settlement for field is required")]
        public Nullable<Int32> selADVBillNumber { get; set; }

        public bool GetADVCommitment_f
        {
            get
            {
                return (this.ADVSettlement_f && this.TravelBillId == 0);
            }
        }

        [Required(ErrorMessage = "Please select proper PI from the list")]
        public Nullable<Int32> PI { get; set; }

        public string PIName { get; set; }
        public string Remarks { get; set; }

        public Nullable<decimal> AdvanceAmount { get; set; }
        public string BillNumber { get; set; }

        public string Status { get; set; }
        public int SlNo { get; set; }
        public string RequestedDate { get; set; }
        public List<TravelDetailModel> TravelDetail { get; set; } = new List<TravelDetailModel>();

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "TDS Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PaymentTDSAmount { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Payment Break Up Total")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PaymentBUTotal { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "GST Offset Total")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> GSTOffsetTotal { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Invoice Break Up Total")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> InvoiceBUTotal { get; set; }
        public Nullable<decimal> AdvanceValueWOClearanceAgent { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public Nullable<Int32> BankId { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }

        public List<InvoiceBreakUpDetailModel> InvoiceBreakDetail { get; set; } = new List<InvoiceBreakUpDetailModel>();
        public List<PaymentBreakUpDetailModel> PaymentBreakDetail { get; set; } = new List<PaymentBreakUpDetailModel>();
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public bool PFInit { get; set; }
    }


    public class DomesticTravelBillEntryModel : CommonPaymentModel
    {
        public Nullable<int> TravelBillId { get; set; }
        public Nullable<decimal> OverallExpense { get; set; }

        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }

        public string ReferenceNumber { get; set; }

        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }

        [Required]
        public Nullable<Int32> Source { get; set; }

        [Required(ErrorMessage = "Please select proper PI from the list")]
        public Nullable<Int32> PI { get; set; }

        public string PIName { get; set; }

        public string BillNumber { get; set; }
        [Required]
        public string Place { get; set; }
        [Required]
        public string Purpose { get; set; }

        [Required]
        [Display(Name = "From Date")]
        public Nullable<DateTime> TravelFromDate { get; set; }
        [Required]
        [Display(Name = "To Date")]
        public Nullable<DateTime> TravelToDate { get; set; }
        [Required]
        [Display(Name = "No of Traveller")]
        public Nullable<int> NoOfTraveller { get; set; } = 1;
        public string Remarks { get; set; }
        public string Status { get; set; }
        public int SlNo { get; set; }
        public string RequestedDate { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public Nullable<Int32> BankId { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }

        public string BankName { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "TDS Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PaymentTDSAmount { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Payment Break Up Total")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PaymentBUTotal { get; set; }

        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }
        [RequiredIf("TravelBillId", null, ErrorMessage = "Invoice Attachement field is required")]
        [Display(Name = "Invoice Attachement")]
        public HttpPostedFileBase InvoiceAttachment { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentActualName { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "GST Offset Total")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> GSTOffsetTotal { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Invoice Break Up Total")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> InvoiceBUTotal { get; set; }

        public List<InvoiceBreakUpDetailModel> InvoiceBreakDetail { get; set; } = new List<InvoiceBreakUpDetailModel>();
        public List<PaymentBreakUpDetailModel> PaymentBreakDetail { get; set; } = new List<PaymentBreakUpDetailModel>();

        public List<TravelerDetailModel> TravelerDetail { get; set; } = new List<TravelerDetailModel>();
        public List<TravelBreakUpDetailModel> BreakUpDetail { get; set; } = new List<TravelBreakUpDetailModel>();
        public string SoureceName { get; set; }
        public string SourceEmail { get; set; }
        public string TravelFrmDate { get; set; }
        public string TravelToDates { get; set; }
        public bool PFInit { get; set; }
    }
    
    public class TravelBreakUpDetailModel
    {
        public Nullable<Int32> BreakUpDetailId { get; set; }
        [Required]
        [Display(Name = "Type of Expense")]
        public Nullable<Int32> ExpenseTypeId { get; set; }


        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Claimed Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> ClaimedAmount { get; set; }


        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Processed Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> ProcessedAmount { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Difference")]
        //[Range(0, 9999999999999999.99)]
        public Nullable<decimal> DifferenceAmt { get; set; }

        public string Remarks { get; set; }
        public string ExpanseCategory { get; set; }
    }


    public class TravelDetailModel
    {
        public Nullable<Int32> TravelBillDetailId { get; set; }
        [Required]
        [Display(Name = "Country")]
        public Nullable<Int32> CountryId { get; set; }
        [Required]
        public string Place { get; set; }
        [Required]
        [Display(Name = "Purpose & Visit")]
        public string Purpose { get; set; }
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }
        [Required]
        [Display(Name = "From Date")]
        public Nullable<DateTime> TravelFromDate { get; set; }
        [Required]
        [Display(Name = "To Date")]
        public Nullable<DateTime> TravelToDate { get; set; }
        [RequiredIf("TravelBillDetailId", null)]
        [Display(Name = "Invoice Attachement")]
        public HttpPostedFileBase InvoiceAttachment { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentActualName { get; set; }
        public int[] TravellerDetailId { get; set; }
        [Required]
        [Display(Name = "Category")]
        public Nullable<Int32>[] CategoryId { get; set; }

        [Display(Name = "Name")]
        public string[] TravellerName { get; set; }

        [Display(Name = "Name")]
        public string[] TravellerId { get; set; }

        [Required]
        [Display(Name = "Boarding")]
        public string[] Boarding { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "per diem")]
        //[Range(0, 9999999999999999.99)]
        public Nullable<decimal>[] PerDiem { get; set; }
        public int[] BreakUpDetailId { get; set; }
        [Required]
        [Display(Name = "Type of Expense")]
        public Nullable<Int32>[] ExpenseTypeId { get; set; }

        [Required]
        [Display(Name = "Claimed Currency Spent")]
        //[Range(0, 9999999999999999.99)]
        public Nullable<int>[] ClaimedCurrencySpent { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Claimed Forex Amt")]
        //[Range(0, 9999999999999999.99)]
        public Nullable<decimal>[] ClaimedForexAmt { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Claimed Conv. Rate")]
        //[Range(0, 9999999999999999.99)]
        public Nullable<decimal>[] ClaimedConvRate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Claimed INR Amount")]
        //[Range(0, 9999999999999999.99)]
        public Nullable<decimal>[] ClaimedTotalAmount { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Processed Forex Amt")]
        //[Range(0, 9999999999999999.99)]
        public Nullable<decimal>[] ProcessedForexAmt { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Processed Conv. Rate")]
        //[Range(0, 9999999999999999.99)]
        public Nullable<decimal>[] ProcessedConvRate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Processed INR Amount")]
        //[Range(0, 9999999999999999.99)]
        public Nullable<decimal>[] ProcessedTotalAmount { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Difference")]
        //[Range(0, 9999999999999999.99)]
        public Nullable<decimal>[] DifferenceAmt { get; set; }

        public string[] Remarks { get; set; }
        public string CountryName { get; set; }
    }

    //public class TravellerDetailModel
    //{
    //    public Nullable<Int32> TravellerDetailId { get; set; }
    //    [Required]
    //    [Display(Name = "Category")]
    //    public Nullable<Int32> CategoryId { get; set; }

    //    [Required]
    //    [Display(Name = "Name")]
    //    public string TravellerName { get; set; }

    //    [Required]
    //    [Display(Name = "Boarding")]
    //    public string Boarding { get; set; }

    //    [Required]
    //    [DisplayFormat(DataFormatString = "{0:n2}")]
    //    [Display(Name = "per diem")]
    //    [Range(0, 9999999999999999.99)]
    //    public Nullable<decimal> PerDiem { get; set; }
    //}

    public class TravelPrdicate
    {
        public tblTravelBill trvbill { get; set; }
        public tblTravelBillDetail trvbilldetail { get; set; }
    }
    #endregion
    #region SBIPrepaidCard

    public class SBIECardModel : CommonPaymentModel
    {
        public int Sno { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public int SBIEcardId { get; set; }
        public string ProjectNumber { get; set; }
        public string SBIEcardNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalValueofCard { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> CurrentProjectAllotmentValue { get; set; }
        public string Projecttitle { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<int> SelectProject { get; set; }
        public string NameofPI { get; set; }
        public Nullable<int> Department { get; set; }
        public string PIDepartmentName { get; set; }
        public string Remarks { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> Sanctionvalue { get; set; }
        public string CurrentFinancialYear { get; set; }
        public string GSTNumber { get; set; }
        [MaxLength(10)]
        [RegularExpression("[A-Za-z]{5}[0-9]{4}[A-Za-z]{1}", ErrorMessage = "Invalid PAN Number")]
        [Display(Name = "PAN Number")]
        public string PAN { get; set; }
        public string PIFirstname { get; set; }
        public Nullable<int> TotalProjectsIncluded { get; set; }
        public string PIAddressLine1 { get; set; }
        public string PIAddressLine2 { get; set; }
        public string PIdistrict { get; set; }
        public string PIstate { get; set; }
        public string PIPincode { get; set; }
        public Nullable<int> PIGender { get; set; }
        public string PIMobile { get; set; }
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email address")]
        public string PIEmail { get; set; }
        public string PICity { get; set; }
        public string MothersMaiden { get; set; }
        public string FatherFirstName { get; set; }
        public string StateBankACNumber { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> DateOfBirth { get; set; }
        public string DOB { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> CardExpiryDate { get; set; }
        public string CardExpryDte { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> EntryDate { get; set; }
        public string EntryDte { get; set; }
        public string Status { get; set; }
        public HttpPostedFileBase[] docfile { get; set; }
        public string[] docfilename { get; set; }
        public string[] CardDocfilepath { get; set; }
        public Nullable<int>[] CardDocType { get; set; }
        public SBIECardSearchFieldModel SearchField { get; set; }
        public PagedData<SBIECardSearchResultModel> SearchResult { get; set; }
        public int SlNo { get; set; }
        public int SBIEcardProjectDetailsId { get; set; }
        public string RecoupmentNumber { get; set; }
        public int RecoupmentId { get; set; }
        public string RequestedDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> RecoupmentValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> BalanceValue { get; set; }
        public int[] DetailsID { get; set; }
        public string[] Particulars { get; set; }
        public Nullable<decimal>[] RateofItem { get; set; }
        public Nullable<int>[] QuantityofItem { get; set; }
        public Nullable<decimal>[] AmountofItem { get; set; }
        public string[] VendorName { get; set; }
        public string[] VendorBillNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalBillAmount { get; set; }
        public string PostdDate { get; set; }
        public Nullable<bool> IsRecoupmentpending { get; set; }
        public string SBIEcardPjctDetlsNumber { get; set; }
        public List<SBICardBillListModel> PaymentDetails { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalTaxAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalEligibleGST { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> NetPayableValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> PendingBillsRecoupValue { get; set; }
    }

    public class SBICardBillListModel
    {
        public Nullable<int> SBICardBillDetailId { get; set; }
        public string InvoiceNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Taxable Percentage")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TaxablePercentage { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Tax value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TaxValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Tax value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string InvDate { get; set; }
        [RequiredIf("IsTaxEligible", true, ErrorMessage = "GSTIN field is required")]
        [MaxLength(15)]
        [RegularExpression("^([0]{1}[1-9]{1}|[1-2]{1}[0-9]{1}|[3]{1}[0-7]{1})([a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9a-zA-Z]{1}[zZ]{1}[0-9a-zA-Z]{1})+$", ErrorMessage = "Invalid GST Number")]
        [Display(Name = "GST Number")]
        public string GSTIN { get; set; }
        public Nullable<bool> IsTaxEligible { get; set; }
        public string HSNCode { get; set; }
        public string Vendor { get; set; }
        public string VendorBillNumber { get; set; }
        public Nullable<bool> IsInterstate { get; set; }
        public Nullable<Int32> TypeOfServiceOrCategory { get; set; }
    }
    public class SBIECardSearchResultModel
    {
        public Nullable<int> Sno { get; set; }
        public Nullable<int> SBIEcardId { get; set; }
        public string NameofPI { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public string ProjectNumber { get; set; }
        public string SBIEcardNumber { get; set; }
        public string SBIEcardPjctDetlsNumber { get; set; }
        public string RecoupmentNumber { get; set; }
        public int RecoupmentId { get; set; }
        public int SBIEcardProjectDetailsId { get; set; }
        public string CardExpryDte { get; set; }
        public string Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalValueofCard { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> CurrentProjectAllotmentValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> RecoupmentValue { get; set; }
        public string RequestedDate { get; set; }
        public Nullable<int> SlNo { get; set; }
    }
    public class SBIECardSearchFieldModel
    {
        public string PIName { get; set; }
        public string ProjectNumber { get; set; }
        public string SBICardNumber { get; set; }
        public string SBICardACNumber { get; set; }
        public string RecoupmentNumber { get; set; }
        public Nullable<DateTime> EntryFromDate { get; set; }
        public Nullable<DateTime> EntryToDate { get; set; }
        public int RecoupmentId { get; set; }
        public int SBIEcardId { get; set; }
        public int SBIEcardProjectDetailsId { get; set; }
        public string SBIEcardPjctDetlsNumber { get; set; }
        public string SBIEcardNumber { get; set; }
        public string RequestedDate { get; set; }
        public Nullable<Decimal> CurrentProjectAllotmentValue { get; set; }
        public Nullable<Decimal> RecoupmentValue { get; set; }
        public string Status { get; set; }
    }

    public class SBIECardBillRecoupModel
    {
        public Nullable<Int32> SBIECardBillRecoupId { get; set; }
        public Nullable<Int32> SBIECardRecoupId { get; set; }
        public string SBIECardBillRecoupNumber { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }

        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }

        [Required]
        public Nullable<Int32> Source { get; set; }
        public string Narration { get; set; }
        public string Status { get; set; }
        public string PostedDate { get; set; }
        public int SlNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Expense Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CreditAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> DebitAmount { get; set; }
        public HttpPostedFileBase Document { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentActualName { get; set; }
        public Nullable<int> PIId { get; set; }
        public string SBIECardACNumber { get; set; }
        public string NameofPI { get; set; }
        public Nullable<Int32> SBIECardId { get; set; }
        public Nullable<Int32> SBIECardProjectId { get; set; }
        public Nullable<decimal> ITCAmount { get; set; }
        public SBIECardSearchFieldModel SearchField { get; set; }
        public List<SBIECardBillRecoupDetailModel> CrDetail { get; set; }
        public List<SBIECardBillRecoupDetailModel> DrDetail { get; set; }
    }
    public class SBIECardBillRecoupDetailModel
    {
        [Required]
        [Display(Name = "Account Group")]
        public Nullable<Int32> AccountGroupId { get; set; }

        public List<MasterlistviewModel> AccountHeadList { get; set; } = new List<MasterlistviewModel>();

        [Required]
        [Display(Name = "Account Head")]
        public Nullable<Int32> AccountHeadId { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }
    }
    public class SearchSBIECardMaster
    {
        public string InSBICardRefNumber { get; set; }
        public string InReferenceNumber { get; set; }
        public string InSBICardAccNumber { get; set; }
        public string InSBICardNumber { get; set; }
        public string InSBICardNamePI { get; set; }
        public Nullable<Decimal> InTotalCardValue { get; set; }
        public Nullable<Decimal> InBillValue { get; set; }
        public string InSBICardStatus { get; set; }
        public Nullable<DateTime> InFromDate { get; set; }
        public Nullable<DateTime> InToDate { get; set; }
        public string ExSBICardRefNumber { get; set; }
        public string ExReferenceNumber { get; set; }
        public string ExSBICardAccNumber { get; set; }
        public string ExSBICardNumber { get; set; }
        public string ExSBICardNamePI { get; set; }
        public Nullable<DateTime> ExFromDate { get; set; }
        public Nullable<DateTime> ExToDate { get; set; }
        public int TotalRecords { get; set; }
        public List<SBIECardModel> ImpMaster { get; set; }
        public List<SBIECardBillRecoupModel> BillRecoup { get; set; }
    }
    #endregion
    #region ImprestPayment
    public class SearchImpresrtMaster
    {
        public string InImprestNumber { get; set; }
        public string InReferenceNumber { get; set; }
        public string InImprestAccNumber { get; set; }
        public string InImprestCardNumber { get; set; }
        public string InImprestNamePI { get; set; }
        public Nullable<Decimal> InTotalCardValue { get; set; }
        public Nullable<Decimal> InBillValue { get; set; }
        public string InImprestStatus { get; set; }
        public Nullable<DateTime> InFromDate { get; set; }
        public Nullable<DateTime> InToDate { get; set; }
        public string ExImprestNumber { get; set; }
        public string ExReferenceNumber { get; set; }
        public string ExImprestAccNumber { get; set; }
        public string ExImprestCardNumber { get; set; }
        public string ExImprestNamePI { get; set; }
        public Nullable<DateTime> ExFromDate { get; set; }
        public Nullable<DateTime> ExToDate { get; set; }
        /* Added by Soundarkambli for bug #num 7916 */
        public string AccountHead { get; set; }
        public int TotalRecords { get; set; }
        public List<ImprestPaymentModel> ImpMaster { get; set; }
        public List<ImprestBillRecoupModel> BillRecoup { get; set; }
    }
    public class ImprestPaymentModel : CommonPaymentModel
    {
        public int Sno { get; set; }
        public int ImprestcardId { get; set; }
        public string ProjectNumber { get; set; }
        public string ImprestcardNumber { get; set; }
        public string ImprestNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalValueofCard { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> ImprestValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> CurrentImprestValue { get; set; }
        public string Projecttitle { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<int> SelectProject { get; set; }
        public string NameofPI { get; set; }
        public Nullable<int> Department { get; set; }
        public string PIDepartmentName { get; set; }
        public string Remarks { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> Sanctionvalue { get; set; }
        public string CurrentFinancialYear { get; set; }
        public string GSTNumber { get; set; }
        [MaxLength(10)]
        [RegularExpression("[A-Z]{5}[0-9]{4}[A-Z]{1}", ErrorMessage = "Invalid PAN Number")]
        [Display(Name = "PAN Number")]
        public string PAN { get; set; }
        public string PIFirstname { get; set; }
        public Nullable<int> TotalImprests { get; set; }
        public string PIAddressLine1 { get; set; }
        public string PIAddressLine2 { get; set; }
        public string PIdistrict { get; set; }
        public string PIstate { get; set; }
        public string PIPincode { get; set; }
        public Nullable<int> PIGender { get; set; }
        public string PIMobile { get; set; }
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email address")]
        public string PIEmail { get; set; }
        public string PICity { get; set; }
        public string MothersMaiden { get; set; }
        public string FatherFirstName { get; set; }
        public Nullable<int> SelectImprestACNumber { get; set; }
        public string ImprestBankACNumber { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> DateOfBirth { get; set; }
        public string DOB { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> CardExpiryDate { get; set; }
        public string CardExpryDte { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> EntryDate { get; set; }
        public string EntryDte { get; set; }
        public string Status { get; set; }
        public HttpPostedFileBase[] docfile { get; set; }
        public string[] docfilename { get; set; }
        public string[] CardDocfilepath { get; set; }
        public Nullable<int>[] CardDocType { get; set; }
        public int SlNo { get; set; }
        public int ImprestProjectDetailsId { get; set; }
        public string RecoupmentNumber { get; set; }
        public int RecoupmentId { get; set; }
        public string RequestedDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> RecoupmentValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> BalanceValue { get; set; }
        //public int[] DetailsID { get; set; }
        //public string[] Particulars { get; set; }
        //public Nullable<decimal>[] RateofItem { get; set; }
        //public Nullable<int>[] QuantityofItem { get; set; }
        //public Nullable<decimal>[] AmountofItem { get; set; }
        //public string[] VendorName { get; set; }
        //public string[] SACNumber { get; set; }
        //public string[] InvoiceNumber { get; set; }
        //public string[] InvoiceDate { get; set; }
        //public string[] TaxValue { get; set; }
        //public string[] TypeOfServiceOrCategory { get; set; }
        //public bool[] IsTaxEligible { get; set; }
        //public bool[] IsInterstate { get; set; }
        //public string[] GSTIN { get; set; }
        //public string[] VendorBillNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalBillAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalTaxAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalEligibleGST { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> NetPayableValue { get; set; }
        public string PostdDate { get; set; }
        public Nullable<bool> IsRecoupmentpending { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }
        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }
        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }
        [Required]
        public Nullable<Int32> Source { get; set; }
        public Nullable<int> SelBank { get; set; }
        public string BankName { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalProjectsValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalPrevImprestValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalPrevRecoupValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalPrevBillsRecoupValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PendingBillsRecoupValue { get; set; }
        public ImprestPaymentSearchFieldModel SearchField { get; set; }
        public PagedData<ImprestPaymentSearchResultModel> SearchResult { get; set; }
        public List<ImprestProjectListModel> PIProjectDetails { get; set; }
        public List<ImprestListModel> PIImprestDetails { get; set; }
        public string ImprestPaymentNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalImprestValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> EnhancedImprestValue { get; set; }
        public string ImprestEnhanceNumber { get; set; }
        public Nullable<bool> IsImprestEnhance { get; set; }
        public List<ImprestBillListModel> PaymentDetails { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public bool PFInit { get; set; }
        public int CloseId { get; set; }
        public int LedBankId { get; set; }
        public Nullable<decimal> ExistingImpresetBalance { get; set; }

       
        [Display(Name = "Bank Name")]
        public Nullable<Int32> BankId { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }

        public string BankHeadName { get; set; }

    }
    public class ImprestBillListModel
    {
        public Nullable<int> ImprestBillDetailId { get; set; }
        public string InvoiceNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Taxable Percentage")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TaxablePercentage { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Tax value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TaxValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Tax value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string InvDate { get; set; }
        [RequiredIf("IsTaxEligible", true, ErrorMessage = "GSTIN field is required")]
        [MaxLength(15)]
        [RegularExpression("^([0]{1}[1-9]{1}|[1-2]{1}[0-9]{1}|[3]{1}[0-7]{1})([a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9a-zA-Z]{1}[zZ]{1}[0-9a-zA-Z]{1})+$", ErrorMessage = "Invalid GST Number")]
        [Display(Name = "GST Number")]
        public string GSTIN { get; set; }
        public Nullable<bool> IsTaxEligible { get; set; }
        public string HSNCode { get; set; }
        public string Vendor { get; set; }
        public string VendorBillNumber { get; set; }
        public Nullable<bool> IsInterstate { get; set; }
        public Nullable<Int32> TypeOfServiceOrCategory { get; set; }
    }
    public class ImprestProjectListModel
    {
        public Nullable<int> PIProjectId { get; set; }
        public string PIProjectNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PIProjectSanctionValue { get; set; }
        public Nullable<int> SelProjecttype { get; set; }
        public string PIProjectType { get; set; }
    }
    public class ImprestListModel
    {
        public Nullable<int> DetailsId { get; set; }
        public string Type { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }
        public string CrtdDate { get; set; }

    }
    public class ImprestPaymentSearchResultModel
    {
        public Nullable<int> Sno { get; set; }
        public Nullable<int> ImprestcardId { get; set; }
        public Nullable<int> ImprestProjectDetailsId { get; set; }
        public string NameofPI { get; set; }
        public Nullable<int> PIId { get; set; }
        public string ProjectNumber { get; set; }
        public string ImprestcardNumber { get; set; }
        public string ImprestNumber { get; set; }
        public string ImprestEnhanceNumber { get; set; }
        public string ImprestBankACNumber { get; set; }
        public string CardExpryDte { get; set; }
        public string Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalValueofCard { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> ImprestValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> EnhancedImprestValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> RecoupmentValue { get; set; }
        public Nullable<bool> IsRecoupmentpending { get; set; }
        public Nullable<int> RecoupmentId { get; set; }
        public string RecoupmentNumber { get; set; }
        public string RequestedDate { get; set; }
        public Nullable<int> SlNo { get; set; }
    }
    public class ImprestPaymentSearchFieldModel
    {
        public string PIName { get; set; }
        //public string ProjectNumber { get; set; }
        public string ImprestcardNumber { get; set; }
        public string ImprestrefernceNumber { get; set; }
        public string ImprestacNumber { get; set; }
        public string RecoupmentNumber { get; set; }
        public Nullable<DateTime> EntryFromDate { get; set; }
        public Nullable<DateTime> EntryToDate { get; set; }
        public int RecoupmentId { get; set; }
        public string RequestedDate { get; set; }
        public string CreditAccountHead { get; set; }

    }
    public class ImprestBillRecoupModel
    {
        public Nullable<Int32> ImprestBillRecoupId { get; set; }
        public Nullable<Int32> ImprestRecoupId { get; set; }
        public string ImprestBillRecoupNumber { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }

        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }

        [Required]
        public Nullable<Int32> Source { get; set; }
        public string Narration { get; set; }
        public string Status { get; set; }
        public string PostedDate { get; set; }
        public int SlNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Expense Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CreditAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> DebitAmount { get; set; }
        public HttpPostedFileBase Document { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentActualName { get; set; }
        public Nullable<int> PIId { get; set; }
        public string ImprestBankACNumber { get; set; }
        public string NameofPI { get; set; }
        public Nullable<decimal> ITCAmount { get; set; }
        public ImprestPaymentSearchFieldModel SearchField { get; set; }
        public List<ImprestBillRecoupDetailModel> CrDetail { get; set; }
        public List<ImprestBillRecoupDetailModel> DrDetail { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public bool PFInit { get; set; }
        /* Added by Soundarkambli for bug #num 7916 */
        public string AccountHead { get; set; }
    }
    public class ImprestBillRecoupDetailModel
    {
        [Required]
        [Display(Name = "Account Group")]
        public Nullable<Int32> AccountGroupId { get; set; }

        public List<MasterlistviewModel> AccountHeadList { get; set; } = new List<MasterlistviewModel>();

        [Required]
        [Display(Name = "Account Head")]
        public Nullable<Int32> AccountHeadId { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }
        public string AccountHeadName { get; set; }
        public string AccountGroupName { get; set; }
    }

    #endregion
    #region Temporary
    #region Advance


    public class TemporaryAdvSearchModel
    {
        public string InProjectNumber { get; set; }
        public string InSettlementNumber { get; set; }
        public string InTemporaryAdvSearch { get; set; }
        public Nullable<decimal> Advancevalue { get; set; }
        public Nullable<decimal> settlemenevalue { get; set; }
        public string Status { get; set; }
        public int TotalRecords { get; set; }
        public List<TemporaryAdvanceModel> Templist { get; set; }

    }
    public class TemporaryAdvanceModel : CommonPaymentModel
    {
        public int TemporaryAdvanceId { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }

        public string ReferenceNumber { get; set; }
        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }
        [Required]
        public Nullable<Int32> Source { get; set; }
        [Required]
        [Display(Name = "Project")]
        public Nullable<int> ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public Nullable<int> PIId { get; set; }
        [Display(Name = "Name")]
        public string UserId { get; set; }
        public string Name { get; set; }
        public string PIName { get; set; }
        [Display(Name = "Projecttype")]
        public Nullable<int> ProjectType { get; set; }
        public string ProjectTypeName { get; set; }
        public string ProjectNumber { get; set; }
        public string TemporaryAdvanceNumber { get; set; }
        [Required]
        public string ClaimingDepartment { get; set; }
        [Required]
        public string NameofReceiver { get; set; }
        public Nullable<int> NoofUnsettledAdvance { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public Nullable<Int32> BankId { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }

        public string BankHeadName { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PendingSettlementAmount { get; set; }
        public string PendingSettlementAdvanceNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Estimate Value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalExpenseValue { get; set; }

        [Required]
        // [LessThanOrEqualTo("EstimatedValue", ErrorMessage = "Temporary Advance Requested should not be Greater than Estimate Value.")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Advance Requested")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TemporaryAdvanceValue { get; set; }

        public string Remarks { get; set; }
        public string Status { get; set; }
        public int SlNo { get; set; }
        public string RequestedDate { get; set; }
        public int[] DetailsID { get; set; }
        [Required]
        [Display(Name = "Particulars")]
        public string[] Particulars { get; set; }
        //public Nullable<decimal>[] RateofItem { get; set; }
        //public Nullable<int>[] QuantityofItem { get; set; }
        [Required]
        public Nullable<decimal>[] AmountofItem { get; set; }
        public string[] PendingAdvanceNumber { get; set; }
        public Nullable<decimal>[] PendingAdvanceAmount { get; set; }
        public Nullable<DateTime>[] AdvanceCreatedDate { get; set; }
        public String[] AdvCrtdDate { get; set; }
        public bool IsSettlementPending_f { get; set; }
        public bool IsSettlementPendingDoc_f { get; set; }
        public bool IsTempAdvanceDoc_f { get; set; }
        public TempAdvSearchFieldModel SearchField { get; set; }
        public PagedData<TempAdvSearchResultModel> SearchResult { get; set; }
        //public string[] VendorName { get; set; }
        //public string[] VendorBillNumber { get; set; }
        //[Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalBillAmount { get; set; }
        public string TempAdvSettlementNumber { get; set; }
        // [LessThanOrEqualTo("EstimatedValue", ErrorMessage = "Temporary Advance Requested should not be Greater than Estimate Value.")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Settlement Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> SettlementValue { get; set; }
        public int TempAdvSettlId { get; set; }
        public Nullable<Int32> PaymentType { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> ReceiptAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PaymentAmount { get; set; }

        public bool GetADVCommitment_f
        {
            get
            {
                return (this.TempAdvSettlId == 0);
            }
        }
        public Nullable<decimal> GSTOffsetTotal { get; set; }
        public Nullable<decimal> InvoiceBUTotal { get; set; }
        public List<InvoiceBreakUpDetailModel> InvoiceBreakDetail { get; set; } = new List<InvoiceBreakUpDetailModel>();
        public bool BillProcessingStatus { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public bool PFInit { get; set; }
        public string PINames { get; set; }
        public string ProjectNumberandtitle { get; set; }
    }
    public class TempAdvSearchResultModel
    {
        public Nullable<int> SlNo { get; set; }
        public Nullable<int> TemporaryAdvanceId { get; set; }
        public string NameofPI { get; set; }
        public Nullable<int> Projectid { get; set; }
        public string ProjectNumber { get; set; }
        public string TemporaryAdvanceNumber { get; set; }
        public string TempAdvSettlNumber { get; set; }
        public string RequestedDate { get; set; }
        public string Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TemporaryAdvanceValue { get; set; }

    }
    public class TempAdvSearchFieldModel
    {
        public string PIName { get; set; }
        public string ProjectNumber { get; set; }
        public string TemporaryAdvanceNumber { get; set; }
        public string TempAdvSettlNumber { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }

    }
    #endregion
    #region Settlement
    public class TemporaryAdvanceSettlementModel
    {
        public int TemporaryAdvanceId { get; set; }

        public string TransactionTypeCode { get; set; }

        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }
        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }

        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }

        [Required]
        public Nullable<Int32> Source { get; set; }

        public bool DirectSettlement_f { get; set; }

        [RequiredIf("DirectSettlement_f", true, ErrorMessage = "PO Number field is required")]
        public Nullable<Int32> selADVBillNumber { get; set; }

        [Required]
        [Display(Name = "Project")]
        public Nullable<Int32> ProjectId { get; set; }

        public string BillNumber { get; set; }

        public string Status { get; set; }
        public int SlNo { get; set; }
        public string RequestedDate { get; set; }
        public bool NeedUpdateTransDetail { get; set; }
        public Nullable<int> CheckListVerified_By { get; set; }
        public string CheckListVerifierName { get; set; }
        public List<BillCommitmentDetailModel> CommitmentDetail { get; set; } = new List<BillCommitmentDetailModel>();
        public List<BillExpenseDetailModel> ExpenseDetail { get; set; }
        public List<BillDeductionDetailModel> DeductionDetail { get; set; }
        public List<AttachmentDetailModel> DocumentDetail { get; set; } = new List<AttachmentDetailModel>();
        public List<CheckListModel> CheckListDetail { get; set; } = new List<CheckListModel>();
    }
    #endregion
    #endregion
    #region SummerInternship

    public class SummerInternshipModel : CommonPaymentModel
    {
        public int SummrInternStudentId { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }
        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }
        [Required]
        public Nullable<Int32> Source { get; set; }
        [Required]
        [Display(Name = "Project")]
        public Nullable<int> ProjectId { get; set; }
        [Required]
        public Nullable<int> PIId { get; set; }
        public string PIName { get; set; }
        public string ProjectNumber { get; set; }
        public string SummerInternshipNumber { get; set; }
        [Required]
        public string College { get; set; }
        [Required]
        public string StudentName { get; set; }
        [Required]
        public string StudentAddress { get; set; }
        [Required]
        public string StudentRollNo { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email address")]
        public string StudentEmail { get; set; }
        [Required]
        public string StudentCourse { get; set; }
        [Required]
        public Nullable<int> StudentYearofStudy { get; set; }
        public string Duration { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Stipend Value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalStipendValue { get; set; }

        //[DisplayFormat(DataFormatString = "{0:n2}")]
        //[Display(Name = "Stipend Requested")]
        //[Range(0, 9999999999999999.99)]
        //public Nullable<decimal> StipendValueperMonth { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public int SlNo { get; set; }
        [Required]
        public string BankName { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string BranchName { get; set; }
        [Required]
        public string IFSCCode { get; set; }
        [Required]
        public Nullable<DateTime> InternshipFromDate { get; set; }
        [Required]
        public Nullable<DateTime> InternshipToDate { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public Nullable<Int32> BankId { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }

        public string  BankHeadName { get; set; }

        public SummerInternshipSearchFieldModel SearchField { get; set; }

        public PagedData<SummerInternshipSearchResultModel> SearchResult { get; set; }
        public bool BillProcessingStatus { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public string InternshipStartDate { get; set; }
        public string InternshipCloseDate { get; set; }
        public bool PFInit { get; set; }



    }
    public class SummerInternshipSearchResultModel
    {
        public Nullable<int> SlNo { get; set; }
        public Nullable<int> SummrInternStudentId { get; set; }
        public string NameofPI { get; set; }
        public string StudentName { get; set; }
        public Nullable<int> Projectid { get; set; }
        public string ProjectNumber { get; set; }
        public string SummerInternshipNumber { get; set; }
        public string College { get; set; }
        public string Duration { get; set; }
        public string Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalStipendValue { get; set; }

    }
    public class SummerInternshipSearchFieldModel
    {
        public string PIName { get; set; }
        public string StudentName { get; set; }
        public string CollegeName { get; set; }
        public string ProjectNumber { get; set; }
        public string SummerInternshipNumber { get; set; }
        public string SearchBy { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public Nullable<DateTime> FromSIDate { get; set; }
        public Nullable<DateTime> ToSIDate { get; set; }

    }
    public class SearchSummerInternshipModel
    {
        public string SearchINPIName { get; set; }
        public string SearchINStudentName { get; set; }
        public string SearchINCollegeName { get; set; }
        public string SearchINProjectNumber { get; set; }
        public string SearchINDuration { get; set; }
        public string SearchINSummerInternshipNumber { get; set; }
        public Nullable<decimal> SearchINAmount { get; set; }
        public string SearchEXPIName { get; set; }
        public string SearchEXStudentName { get; set; }
        public string SearchEXCollegeName { get; set; }
        public string SearchEXProjectNumber { get; set; }
        public string SearchEXSummerInternshipNumber { get; set; }
        public Nullable<decimal> SearchEXAmount { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public Nullable<DateTime> FromSMIDate { get; set; }
        public Nullable<DateTime> ToSMIDate { get; set; }
        public string SearchINStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<SummerInternshipModel> SMIList { get; set; }
    }
    #endregion
    #region ClearancePayment

    public class ClearancePaymentEntryModel : CommonPaymentModel
    {
        public int BillId { get; set; }

        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }

        public string ReferenceNumber { get; set; }

        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }

        [Required]
        public Nullable<Int32> Source { get; set; }

        public string BillNumber { get; set; }
        [Required]
        [Display(Name = "Clearance Agent")]
        public Nullable<Int32> ClearanceAgentId { get; set; }

        [Required]
        [Display(Name = "Bank")]
        public Nullable<Int32> BankHeadId { get; set; }

        [Required]
        [Display(Name = "PO Date")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> PODate { get; set; }

        [Required]
        [Display(Name = "PO Number")]
        public string PONumber { get; set; }

        [Required]
        [Display(Name = "Invoice Date")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> InvoiceDate { get; set; }

        [Required]
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }

        //[DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Advance Percentage")]
        [Range(1, 100)]
        public Nullable<decimal> AdvancePercentage { get; set; }

        public Nullable<decimal> hiddenTaxEligibleAmt { get; set; }
        public Nullable<Int32> PaymentType { get; set; }
        public bool isHaveElgGST { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Bill Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> BillAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Bill Tax Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> BillTaxAmount { get; set; }


        public string Status { get; set; }

        public string ClearanceAgentName { get; set; }
        public string PostedDate { get; set; }
        public int SlNo { get; set; }
        public Nullable<Int32>[] ClearanceAgentTDSDetailId { get; set; }
        public Nullable<decimal> hiddenSettAmt { get; set; }
        public Nullable<decimal> hiddenSettTaxAmt { get; set; }
        public Nullable<decimal> hiddenNetTotalAmt { get; set; }
        public bool PartAdvance_f { get; set; }
        public bool InclusiveOfTax_f { get; set; } = true;
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "TDS Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PaymentTDSAmount { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Payment Break Up Total")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PaymentBUTotal { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "GST Offset Total")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> GSTOffsetTotal { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Invoice Break Up Total")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> InvoiceBUTotal { get; set; }

        public List<InvoiceBreakUpDetailModel> InvoiceBreakDetail { get; set; } = new List<InvoiceBreakUpDetailModel>();
        public List<PaymentBreakUpDetailModel> PaymentBreakDetail { get; set; } = new List<PaymentBreakUpDetailModel>();
        public List<BillPODetailModel> PODetail { get; set; } = new List<BillPODetailModel>();
        public bool BillProcessingStatus { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public string ClearnceInvoiceDate { get; set; }
        public string ClearncePoDate { get; set; }
        public string BankHeadName { get; set; }
        public bool PFInit { get; set; }
        public string ProjNo { get; set; }
        public string CommitmentNo { get; set; }
        public string InvoiceNo { get; set; }
        public string AccType { get; set; }
        public decimal AmountPayable { get; set; }
    }

    public class ClearancePaymentSearchModel
    {
        public string Number { get; set; }
        public string ClearanceAgent { get; set; }
        public string Status { get; set; }
        public string PONumber { get; set; }
        public List<ClearancePaymentEntryModel> list { get; set; }
        public int TotalRecords { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string InvoiceNo { get; set; }
        public string CommitmentNO { get; set; }
        public string ProjectNumber { get; set; }
        public string AccountType { get; set; }
        public Nullable<decimal> AmountPayableValue { get; set; }

    }   
    #endregion

    #region ClaimBill
    public class ClaimBillListModel
    {
        public Nullable<int> ProjectType { get; set; }
        public Nullable<int> PIId { get; set; }
        public string PIName { get; set; }
        public string SelectProject { get; set; }
        public string SelectInvoice { get; set; }
        public int Userrole { get; set; }
        public ClaimBillSearchFieldModel SearchField { get; set; }
        public PagedData<ClaimBillSearchResultModel> SearchResult { get; set; }
    }
    public class ClaimBillSearchResultModel
    {
        public Nullable<int> InvoiceId { get; set; }
        public string PIName { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public string InvoiceDate { get; set; }
        public string ProjectTitle { get; set; }
        public string SACNumber { get; set; }
        public string Service { get; set; }
        public string InvoiceNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalInvoiceValue { get; set; }
        public string Status { get; set; }
    }
    public class ClaimBillSearchFieldModel
    {
        public string InvoiceNumber { get; set; }
        public Nullable<int> PIName { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }

    }
    public class ClaimBillModel
    {
        public int Sno { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Nullable<int> InvoiceDraftId { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string Invoicedatestrng { get; set; }
        public Nullable<int>[] PreviousInvoiceId { get; set; }
        public string[] PreviousInvoiceNumber { get; set; }
        public string[] PreviousInvoiceDate { get; set; }
        public string[] PreviousInvoicedatestrng { get; set; }
        public Nullable<int>[] InstalmentId { get; set; }
        public Nullable<int>[] InstlmntNumber { get; set; }
        public Nullable<Decimal>[] InstalValue { get; set; }
        public Nullable<int>[] Instalmentyear { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] PreviousInvoicevalue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> AvailableBalance { get; set; }
        [Required]
        public string Projecttitle { get; set; }
        [Required]
        public Nullable<int> PIId { get; set; }
        public Nullable<int> Prjcttype { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public string SelectProject { get; set; }
        public string SelectInvoice { get; set; }
        public string NameofPI { get; set; }
        [Required]
        public Nullable<int> SponsoringAgency { get; set; }
        public string SponsoringAgencyName { get; set; }
        public int InvoicecrtdID { get; set; }
        public string ProjectNumber { get; set; }
        public string ProposalNumber { get; set; }
        public int PrpsalNumber { get; set; }
        [Required]
        public Nullable<int> Department { get; set; }
        public string PIDepartmentName { get; set; }
        public string Remarks { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> Sanctionvalue { get; set; }
        public string CurrentFinancialYear { get; set; }
        public string GSTNumber { get; set; }
        public string PAN { get; set; }
        public string TAN { get; set; }
        [Required]
        public string Agencyregname { get; set; }
        [Required]
        public string Agencyregaddress { get; set; }
        [Required]
        public string Agencydistrict { get; set; }
        public Nullable<int> AgencystateId { get; set; }
        public Nullable<int> Agncystatecode { get; set; }
        [Required]
        public string Agencystate { get; set; }
        [Required]
        public Nullable<int> Agencystatecode { get; set; }
        [Required]
        public Nullable<int> AgencyPincode { get; set; }
        [Required]
        public string Agencycontactperson { get; set; }
        public string Agencycontactpersondesignation { get; set; }
        public string AgencycontactpersonEmail { get; set; }
        public string Agencycontactpersonmobile { get; set; }
        public string SanctionOrderNumber { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> SanctionOrderDate { get; set; }
        public string SODate { get; set; }
        public string SACNumber { get; set; }
        public string IITMGSTIN { get; set; }
        public Nullable<int> ServiceType { get; set; }
        public string DescriptionofServices { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TaxableValue { get; set; }

        public Nullable<Decimal> CGST { get; set; }

        public string CGSTPercentage { get; set; }

        public Nullable<Decimal> SGST { get; set; }

        public string SGSTPercentage { get; set; }

        public Nullable<Decimal> IGST { get; set; }

        public string IGSTPercentage { get; set; }
        public Nullable<Decimal> TotalTaxValue { get; set; }
        public string TotalTaxpercentage { get; set; }
        public Nullable<int> TotalTaxpercentageId { get; set; }
        public Nullable<Decimal> TotalInvoiceValue { get; set; }
        public string CurrencyCode { get; set; }
        public string TotalInvoiceValueinwords { get; set; }
        public string CommunicationAddress { get; set; }
        public int Bank { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public Nullable<int> BankAccountId { get; set; }
        public Nullable<int> Instalmentnumber { get; set; }
        public Nullable<int> Instlmntyr { get; set; }
        public Nullable<Decimal> Instalmentvalue { get; set; }
        public string[] Invoiced { get; set; }
        public Nullable<Decimal> CurrInvVal { get; set; }
        public Nullable<int> CurrentFinyearId { get; set; }
        public Nullable<Decimal> TotalOpenInvoiceValue { get; set; }
        public Nullable<Decimal> TotalReceiptValue { get; set; }
        public Nullable<Decimal> TotalOpenBalReceiptValue { get; set; }
        public string InvoiceTypeName { get; set; }
        public string AgencyStateName { get; set; }
        public bool PFInit { get; set; }
        public Nullable<int> AccountGroupId { get; set; }
        public Nullable<int> AccountHeadId { get; set; }
        public Nullable<int> ProjectClassification { get; set; }
        public string AccountGroupName { get; set; }
        public string AccountHeadName { get; set; }

        public Nullable<Decimal> TotalReceiptValueInclIntrst { get; set; }
        public Nullable<Decimal> TotalOpenBalReceiptValueInclIntrst { get; set; }
    }
    #endregion
    #region  Receipt
    public class ReceiptBreakupModel
    {
        public int SlNo { get; set; }
        public Nullable<int> ReceiptId { get; set; }
        public Nullable<int> ReceiptBreakupId { get; set; }
        public string ReceiptNumber { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<decimal> ReceiptAmount { get; set; }
        public Nullable<decimal> TaxableReceiptAmount { get; set; }
        public string ReceiptDateStr { get; set; }
        public string BreakupDateStr { get; set; }
        public Nullable<DateTime> ReceiptDate { get; set; }
        public Nullable<DateTime> BreakupDate { get; set; }
        public string Status { get; set; }
        public HttpPostedFileBase AttachmentFile { get; set; }
        public string AttachmentPath { get; set; }
        public string AttachmentActualName { get; set; }
        public string AttachmentName { get; set; }
        public List<ReceiptBudgetGroupDetailModel> groups { get; set; }
    }

    public class ReceiptBudgetGroupDetailModel
    {
        public Nullable<int> BudgetGroupId { get; set; }
        public string BudgetGroup { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public bool BudgetHeadMapping_f { get; set; }
        public List<ReceiptBudgetHeadDetailModel> heads { get; set; }
        public List<MasterlistviewModel> headList { get; set; }
    }
    public class ReceiptBudgetHeadDetailModel
    {
        public Nullable<int> BudgetHeadId { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }

    public class ReceiptSearchFieldModel
    {
        public string ReceiptNumber { get; set; }
        public Nullable<int> PIId { get; set; }
        public string PIName { get; set; }
        public string SearchBy { get; set; }
        public string ProjectNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string ProjectTitle { get; set; }
        public Nullable<Decimal> ReceiptAmount { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public Nullable<int> ReceiptStatus { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
    }
    public class InvoiceModel
    {
        public int Sno { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Nullable<int> InvoiceDraftId { get; set; }
        public string InvoiceNumber { get; set; }
        [Required]
        public Nullable<int> InvoiceType { get; set; }
        public string TypeofInvoice { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string Invoicedatestrng { get; set; }
        public int[] PreviousInvoiceId { get; set; }
        public string[] PreviousInvoiceNumber { get; set; }
        public string[] PreviousInvoiceDate { get; set; }
        public string[] PreviousInvoicedatestrng { get; set; }
        public Nullable<int>[] InstalmentId { get; set; }
        public Nullable<int>[] InstlmntNumber { get; set; }
        public Nullable<Decimal>[] InstalValue { get; set; }
        public Nullable<int>[] Instalmentyear { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] PreviousInvoicevalue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> AvailableBalance { get; set; }
        [Required]
        public string Projecttitle { get; set; }
        [Required]
        public Nullable<int> PIId { get; set; }
        public Nullable<int> Prjcttype { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public Nullable<int> ProjectClassification { get; set; }
        public Nullable<int> AccountGroupId { get; set; }
        public Nullable<int> AccountHeadId { get; set; }
        public Nullable<decimal> CurrentAvailPjctBalance { get; set; }
        public string SelectProject { get; set; }
        public string SelectInvoice { get; set; }
        public string NameofPI { get; set; }
        [Required]
        public Nullable<int> SponsoringAgency { get; set; }
        [Required]
        [Display(Name = "Agency Name")]
        public string SponsoringAgencyName { get; set; }
        public int InvoicecrtdID { get; set; }
        public string ProjectNumber { get; set; }
        public string ProposalNumber { get; set; }
        public int PrpsalNumber { get; set; }
        [Required]
        public Nullable<int> Department { get; set; }
        public string PIDepartmentName { get; set; }
        public string Remarks { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> Sanctionvalue { get; set; }
        public string CurrentFinancialYear { get; set; }
        [RequiredIf("GSTNReq", true, ErrorMessage = "GSTIN field is required")]
        [StringLength(15, MinimumLength = 15, ErrorMessage = "This field must be 15 characters")]
        public string GSTNumber { get; set; }
        [RequiredIf("PANReq", true, ErrorMessage = "PAN field is required")]
        [RegularExpression("[A-Za-z]{5}[0-9]{4}[A-Za-z]{1}", ErrorMessage = "Invalid PAN Number")]
        public string PAN { get; set; }
        [RequiredIf("TANReq", true, ErrorMessage = "TAN field is required")]
        [RegularExpression("[A-Za-z]{4}[0-9]{5}[A-Za-z]{1}", ErrorMessage = "Invalid TAN Number")]
        public string TAN { get; set; }
        [Required]
        public string Agencyregname { get; set; }
        //[Required]
        public string Agencyregaddress { get; set; }
        //[RequiredIfNot("InvoiceType", 1, ErrorMessage = "Agency District field is required")]
        public string Agencydistrict { get; set; }
        //[Required]
        public string Agencystate { get; set; }
        [RequiredIf("InvoiceType", 1, ErrorMessage = "Country field is required")]
        public Nullable<int> AgencyCountryId { get; set; }
        [RequiredIf("InvoiceType", 1, ErrorMessage = "Place field is required")]
        public string ForeignAgencyPlace { get; set; }
        [Required]
        [Display(Name = "Agency State")]
        public Nullable<int> AgencystateId { get; set; }
        [Required]
        [Display(Name = "Agency State Code")]
        public Nullable<int> Agencystatecode { get; set; }
        public Nullable<int> Agncystatecode { get; set; }
        //[RequiredIfNot("InvoiceType", 1, ErrorMessage = "Agency Pin Code field is required")]
        public Nullable<int> AgencyPincode { get; set; }
        //[Required]
        public string Agencycontactperson { get; set; }
        public string Agencycontactpersondesignation { get; set; }
        [Required]
        [Display(Name = "Agency Email")]
        [EmailAddress]
        public string AgencycontactpersonEmail { get; set; }
        public string Agencycontactpersonmobile { get; set; }
        public string SanctionOrderNumber { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> SanctionOrderDate { get; set; }
        public string SODate { get; set; }
        public string SACNumber { get; set; }
        public string IITMGSTIN { get; set; }
        public Nullable<int> ServiceType { get; set; }
        [Required]
        public string DescriptionofServices { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TaxableValue { get; set; }

        public Nullable<Decimal> CGST { get; set; }
        public Nullable<Decimal> CGSTRate { get; set; }

        public string CGSTPercentage { get; set; }

        public Nullable<Decimal> SGST { get; set; }
        public Nullable<Decimal> SGSTRate { get; set; }

        public string SGSTPercentage { get; set; }

        public Nullable<Decimal> IGST { get; set; }
        public Nullable<Decimal> IGSTRate { get; set; }
        public Nullable<Decimal> AmountReceived { get; set; }
        public string IGSTPercentage { get; set; }
        public Nullable<Decimal> TotalTaxValue { get; set; }
        public string TotalTaxpercentage { get; set; }
        public Nullable<int> TotalTaxpercentageId { get; set; }
        public Nullable<Decimal> TotalInvoiceValue { get; set; }
        public string CurrencyCode { get; set; }
        public string TotalInvoiceValueinwords { get; set; }
        [RequiredIf("ProjectType", 2, ErrorMessage = "Communication Address field is required")]
        [Display(Name = "Communication Address")]
        public string CommunicationAddress { get; set; }
        public int Bank { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public Nullable<int> BankAccountId { get; set; }
        public Nullable<int> Instalmentnumber { get; set; }
        public Nullable<int> Instlmntyr { get; set; }
        public Nullable<Decimal> Instalmentvalue { get; set; }
        public string[] Invoiced { get; set; }
        public Nullable<int> TaxStatus { get; set; }
        public string PONumber { get; set; }
        public Nullable<int> CurrentFinyearId { get; set; }
        public Nullable<Decimal> TotalOpenInvoiceValue { get; set; }
        public Nullable<Decimal> TotalReceiptValue { get; set; }
        public Nullable<Decimal> TotalOpenBalReceiptValue { get; set; }
        public Nullable<Decimal> CurrInvVal { get; set; }
        public string Status { get; set; }
        [RequiredIf("InvoiceType", 1, ErrorMessage = "Currency field is required")]
        public Nullable<int> SelCurr { get; set; }
        [RequiredIf("InvoiceType", 1, ErrorMessage = "Invoice Value in Foreign Currency field is required")]
        public Nullable<Decimal> ForeignCurrencyValue { get; set; }
        public Nullable<Decimal> AllocatedForeignCurrencyValue { get; set; }
        public Nullable<Decimal> ForgnCurrenyRate { get; set; }
        public Nullable<Decimal> PrevinvForeignCurrencyValue { get; set; }
        public Nullable<Decimal> TDSReceived { get; set; }
        public Nullable<Decimal> OutstandingAmount { get; set; }
        public Nullable<Decimal> TDSReceivableGST { get; set; }
        public string PayinslipDate { get; set; }
        public string ModeofPayment { get; set; }
        public string PaySlipReferenceNumber { get; set; }
        public string ReceiptReference { get; set; }
        public string InstrumentsDate { get; set; }
        public string InstrumentsRemarks { get; set; }
        public Nullable<int> PayinslipId { get; set; }
        public Nullable<int> ExternalRefId { get; set; }
        public string CreditNoteNo { get; set; }
        public Nullable<Decimal> TotalCreditNoteAmount { get; set; }
        public PagedData<InvoiceSearchResultModel> SearchResult { get; set; }
        [RequiredIf("InvoiceType", 2, ErrorMessage = "Indian SEZ Tax Category field is required")]
        [Display(Name = "Indian SEZ Tax Category")]
        public Nullable<int> IndianSEZTaxCategory { get; set; }
        public bool ReverseCharge { get; set; }

        [NotMapped]
        public bool TANReq
        {
            get
            {
                return (this.ProjectType == 2 && this.InvoiceType != 1 && this.InvoiceType != null && String.IsNullOrEmpty(this.PAN));
            }
        }
        [NotMapped]
        public bool PANReq
        {
            get
            {
                return (this.ProjectType == 2 && this.InvoiceType != 1 && this.InvoiceType != null && String.IsNullOrEmpty(this.TAN));
            }
        }
        [NotMapped]
        public bool GSTNReq
        {
            get
            {
                return (this.ProjectType == 2 && this.InvoiceType != 1 && this.InvoiceType != null && this.InvoiceType != 4);
            }
        }
        public InvoiceSearchFieldModel SearchField { get; set; }

        public bool IsExternal { get; set; }
        public string InvoiceStatus { get; set; }
        public string InvoiceTypeName { get; set; }
        public string IRNno { get; set; }
        public string InvoiceDateStr { get; set; }
        public string IRNApplicationNo { get; set; }
        public Nullable<DateTime> IRNApplicationDate { get; set; }
        public HttpPostedFileBase IRNAttachmentFile { get; set; }
        public string IRNAttachmentPath { get; set; }
        public string IRNAttachmentActualName { get; set; }
        public string IRNAttachmentName { get; set; }
    }
    public class InvoiceSearchResultModel
    {
        public Nullable<int> Sno { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public string NameofPI { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<Decimal> TotalInvoiceValue { get; set; }
        public string Invoicedatestrng { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceStatus { get; set; }
        public string InvoiceTypeName { get; set; }
        public string CurrencyCode { get; set; }
        public Nullable<DateTime> InvoiceFromDate { get; set; }
        public Nullable<DateTime> InvoiceToDate { get; set; }
    }
    public class InvoiceSearchFieldModel
    {
        public Nullable<int> PIId { get; set; }
        public string PIName { get; set; }
        public string SearchBy { get; set; }
        public string ProjectNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string ProjectTitle { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public Nullable<DateTime> InvoiceFromDate { get; set; }
        public Nullable<DateTime> InvoiceToDate { get; set; }

    }
    public class CreateReceiptModel : CommonPaymentModel
    {
        public Nullable<Decimal> OverheadAllocBal { get; set; }
        public Nullable<Decimal> OverheadCommitBal { get; set; }
        public Nullable<Decimal> Currbalnceinsanctnval { get; set; }
        public Nullable<int> OpeningBalReceiptCount { get; set; }
        public Nullable<int> TotalRCVCount { get; set; }
        public Nullable<Decimal> PrevrcvCGST { get; set; }
        public Nullable<Decimal> PrevrcvSGST { get; set; }
        public Nullable<Decimal> PrevrcvIGST { get; set; }
        public Nullable<Decimal> CurrrcvCGST { get; set; }
        public Nullable<Decimal> CurrrcvSGST { get; set; }
        public Nullable<Decimal> CurrrcvIGST { get; set; }
        public int Sno { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }

        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }

        [Required]
        public Nullable<Int32> Source { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public Nullable<int> ReceiptID { get; set; }
        public string ReceiptNumber { get; set; }
        public Nullable<DateTime> ReceiptDate { get; set; }
        public string ReceiptDateString { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public string ProjectTypeName { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public string ProjectNumber { get; set; }
        public string Projecttitle { get; set; }
        public string NameofPI { get; set; }
        public string ProjectSanctionOrderNo { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<Decimal> AmountSpent { get; set; }
        public Nullable<Decimal> SanctionedValue { get; set; }
        public Nullable<Decimal> NetBalanceValue { get; set; }
        public Nullable<Decimal> TotalReceiptValue { get; set; }
        public Nullable<Decimal> InvoiceValue { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string Invoicedatestrng { get; set; }
        public string InvoiceDetails { get; set; }

        public string IGSTAmount { get; set; }

        public string CGSTAmount { get; set; }

        public string SGSTAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalTaxAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] PreviousReceiptvalue { get; set; }
        public Nullable<int>[] PreviousReceiptId { get; set; }
        public string[] PreviousReceiptNumber { get; set; }
        public string[] PreviousReceiptDate { get; set; }
        public string[] PreviousReceiptdatestrng { get; set; }
        public Nullable<Decimal> AvailableBalance { get; set; }
        public Nullable<Decimal> ReceiptAmount { get; set; }
        public int?[] BudgetHead { get; set; }
        public Nullable<int> ReceivedFromCr { get; set; }
        public Nullable<Decimal> ReceivedAmountCr { get; set; }
        public Nullable<int> BankACHeadDr { get; set; }
        public Nullable<Decimal> BankAmountDr { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] Allocationvalue { get; set; }
        public Nullable<int>[] AllocationId { get; set; }
        public Nullable<int>[] AllocationHead { get; set; }
        public string[] Allocationheadname { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] ReceivablesAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> AllocationTotal { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> ReceivablesTotal { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> NetTotal { get; set; }
        public Nullable<int>[] ReceivablesHeadId { get; set; }
        public string[] Receivablesheadname { get; set; }
        public string[] Receivablesheadcode { get; set; }
        public string Narration { get; set; }
        [Required]
        public Nullable<int> ModeofReceipt { get; set; }
        [RequiredIf("ModeofReceipt", 1, ErrorMessage = "Cheque Number field is required")]
        public string[] ChequeNumber { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime>[] ChequeDate { get; set; }

        [RequiredIf("ModeofReceipt", 1, ErrorMessage = "Cheque Date field is required")]
        public string[] Chqdate { get; set; }

        [DataType(DataType.DateTime)]
        public Nullable<DateTime>[] ChequeClearanceDate { get; set; }

        public string[] Chqclrdate { get; set; }

        [RequiredIf("ModeofReceipt", 1, ErrorMessage = "Cheque - Bank Name field is required")]
        public string[] ChequeBankName { get; set; }
        [RequiredIf("ModeofReceipt", 1, ErrorMessage = "Cheque - Bank Branch Name field is required")]
        public string[] ChequeBankBranch { get; set; }

        [RequiredIf("ModeofReceipt", 1, ErrorMessage = "Cheque Amount field is required")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] ChequeValue { get; set; }

        [RequiredIf("ModeofReceipt", 1, ErrorMessage = "Total Cheque Value field is required")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalChequeValue { get; set; }

        [RequiredIf("ModeofReceipt", 4, ErrorMessage = "Country field is required")]
        public Nullable<int>[] Foreigntransfercountry { get; set; }

        [RequiredIf("ModeofReceipt", 4, ErrorMessage = "Foreign Currency field is required")]
        public Nullable<int>[] Foreigntransfercurrency { get; set; }

        [RequiredIf("ModeofReceipt", 4, ErrorMessage = "Amount in Foreign Currency field is required")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] Foreigncurrencyamount { get; set; }

        [RequiredIf("ModeofReceipt", 4, ErrorMessage = "Conversion rate field is required")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] Conversionrate { get; set; }

        [RequiredIf("ModeofReceipt", 4, ErrorMessage = "Amount in INR field is required")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] INRValue { get; set; }

        [RequiredIf("ModeofReceipt", 4, ErrorMessage = "Foreign remitance Transaction reference number field is required")]
        public string[] Foreignremittransrefno { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime>[] ForeignremittransDate { get; set; }

        [RequiredIf("ModeofReceipt", 4, ErrorMessage = "Foreign remitance Transaction date field is required")]
        public string[] Forgnremitdate { get; set; }

        [RequiredIf("ModeofReceipt", 4, ErrorMessage = "Foreign remitance Transaction bank name field is required")]
        public string[] Foreignremitbankname { get; set; }

        [RequiredIf("ModeofReceipt", 4, ErrorMessage = "Foreign remitance Transaction bank name field is required")]
        public string[] Foreignremitbankbranch { get; set; }

        [RequiredIf("ModeofReceipt", 4, ErrorMessage = "Foreign remitance net transaction value field is required")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> Foreignremittotalamount { get; set; }

        [RequiredIf("ModeofReceipt", 2, ErrorMessage = "DD Number field is required")]
        public string[] DDNumber { get; set; }

        [DataType(DataType.DateTime)]
        public Nullable<DateTime>[] DDDate { get; set; }

        [RequiredIf("ModeofReceipt", 2, ErrorMessage = "DD Date field is required")]
        public string[] Dddte { get; set; }

        [RequiredIf("ModeofReceipt", 2, ErrorMessage = "DD - Bank name field is required")]
        public string[] DDBankName { get; set; }

        [RequiredIf("ModeofReceipt", 2, ErrorMessage = "DD - Bank name field is required")]
        public string[] DDBankBranch { get; set; }

        [RequiredIf("ModeofReceipt", 2, ErrorMessage = "DD Amount field is required")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] DDValue { get; set; }

        [RequiredIf("ModeofReceipt", 2, ErrorMessage = "DD Net transaction value field is required")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalDDValue { get; set; }

        [RequiredIf("ModeofReceipt", 3, ErrorMessage = "Transaction type field is required")]
        public Nullable<int>[] BankTransactiontype { get; set; }


        [DataType(DataType.DateTime)]
        public Nullable<DateTime>[] BankTransactionDate { get; set; }

        [RequiredIf("ModeofReceipt", 3, ErrorMessage = "Transaction date field is required")]
        public string[] Banktransdate { get; set; }

        [RequiredIf("ModeofReceipt", 3, ErrorMessage = "Transaction Reference number field is required")]
        public string[] BankTransactionrefno { get; set; }

        [RequiredIf("ModeofReceipt", 3, ErrorMessage = "Amount field is required")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] BankTransferAmount { get; set; }

        [RequiredIf("ModeofReceipt", 3, ErrorMessage = "Net transaction value field is required")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalBankTransactionAmount { get; set; }
        [RequiredIf("ModeofReceipt", 3, ErrorMessage = "Bank name field is required")]
        public string[] BankTransferBankName { get; set; }
        [RequiredIf("ModeofReceipt", 3, ErrorMessage = "Bank branch field is required")]
        public string[] BankTransferBankBranch { get; set; }
        public string Buttonvalue { get; set; }
        public string ReceiptStatus { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> PreviousReceiptTotal { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> BalanceinCurrentInvoice { get; set; }
        public string ReceivedFromAgencyName { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> CurrentReceiptOverheads { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> ProjectTotalValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> PrevReceiptsOverheads { get; set; }
     
        public Nullable<Decimal> ProjectTotalOverheadsvalue { get; set; }
        public Nullable<Decimal> TotalOverheadPercentage { get; set; }
        public List<ReceiptOverheadListModel> ReceiptOverheads { get; set; }
        public Nullable<Decimal> TotalPrevReceiptVal { get; set; }
        public string RMForCMF { get; set; }
        public Nullable<Decimal> TotalReceivedValue { get; set; }
        [RequiredIf("ModeofReceipt", 5, ErrorMessage = "TDS Certificate Number field is required")]
        public string[] TDSCertificateNumber { get; set; }
        public string[] TDSDocName { get; set; }
        public string[] TDSDocPath { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] AgencyShareAmount { get; set; }
        public Nullable<int>[] AgencysharedetailsId { get; set; }
        public Nullable<int>[] AgencyId { get; set; }
        public string[] Agencyname { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> ReceiptCGST { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> ReceiptSGST { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> ReceiptIGST { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> CGSTPercentage { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> SGSTPercentage { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> IGSTPercentage { get; set; }
        public Nullable<Decimal> InvoiceValueWithoutTax { get; set; }
        public Nullable<Decimal> TotalCreditNoteAmount { get; set; }
        public string ProjectCategory { get; set; }
        public Nullable<Decimal> OpeningINVRCVAmount { get; set; }
        public Nullable<Decimal> CurrRCVAmount { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public Nullable<int> ForeignCurrencyId { get; set; }
        public string ForeignCurrencyCode { get; set; }
        public Nullable<Decimal> INVForeignCurrencyAmount { get; set; }
        public Nullable<Decimal> BalanceinINVFornCurrAmt { get; set; }
        public Nullable<Decimal> ReceiptForeignCurrencyAmount { get; set; }
        public Nullable<int> ReceiptCount { get; set; }
        public Nullable<bool> Isnoallocation { get; set; }
        public string BankACHeadDrName { get; set; }
        public string ModeofReceiptName { get; set; }
        public string[] TransactionTypeName { get; set; }
        public string[] ForeigntransfercountryName { get; set; }
        public string[] ForeigntransfercurrencyName { get; set; }
        public bool PFInit { get; set; }
        public Nullable<Decimal> NetAvailablebalanceinproject { get; set; }
        [Required]
        [Display(Name = "Sanction Order Date")]
        public Nullable<DateTime> SanctionOrderDate { get; set; }
        [Required]
        [Display(Name = "Sanction Order No.")]
        public string SanctionOrderNo { get; set; }
        public bool IsBudgetHeadPosting { get; set; }
        public Nullable<Decimal> ToBeTaxDebited { get; set; }
    }
    public class ReceiptOverheadListModel
    {
        public Nullable<int> OverheadtypeId { get; set; }
        public string Overheadtypename { get; set; }
        public Nullable<Decimal> OverheadPercentage { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> OverheadAmount { get; set; }
        public Nullable<bool> IsRMF_f { get; set; }
        public Nullable<bool> IsCMF_f { get; set; }
    }
    public class ExchangeRateModel
    {
        public Nullable<int> id { get; set; }
        public string name { get; set; }
        public Nullable<Decimal> value { get; set; }
    }
    public class ReceiptListModel
    {
        public Nullable<int> ProjectType { get; set; }
        public string SelectProject { get; set; }
        public string SelectReceipt { get; set; }
        public int Userrole { get; set; }
        public ReceiptSearchFieldModel SearchField { get; set; }
        public PagedData<ReceiptSearchResultModel> SearchResult { get; set; }
        public InvoiceSearchFieldModel InvoiceSearchField { get; set; }
        public PagedData<InvoiceSearchResultModel> InvoiceResult { get; set; }
        public Nullable<int>[] InvoiceId { get; set; }
        public string[] PIName { get; set; }
        public Nullable<int>[] PIId { get; set; }
        public Nullable<int>[] ProjectId { get; set; }
        public string[] ProjectNumber { get; set; }
        public Nullable<Decimal>[] InvoiceValue { get; set; }
        public string[] InvoiceDatestr { get; set; }
        public DateTime[] InvoiceDate { get; set; }
        public string[] InvoiceNumber { get; set; }
        public Nullable<DateTime> InvoiceFromDate { get; set; }
        public Nullable<DateTime> InvoiceToDate { get; set; }
        public string PrjctNumber { get; set; }
        public string InvNumber { get; set; }
        public string NameofPI { get; set; }
    }
    public class ReceiptSearchResultModel
    {
        public Nullable<int> ReceiptId { get; set; }
        public string PIName { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ReceiptNumber { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public string ReceiptStatus { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public string ReceiptDate { get; set; }
        public string ProjectTitle { get; set; }
        public string SACNumber { get; set; }
        public string Service { get; set; }
        public string InvoiceNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalReceiptValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> InvoiceValue { get; set; }
    }

    #endregion
    #region Journal
    public class JournalModel
    {
        public int JournalId { get; set; }
        [Required]
        public Nullable<Int32> Reason { get; set; }
        public string JournalNumber { get; set; }
        public string ReasonString { get; set; }
        [Required]
        public string Narration { get; set; }
        public string Status { get; set; }
        public string PostedDate { get; set; }
        public int SlNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Expense Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CreditAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> DebitAmount { get; set; }

        public List<AttachmentDetailModel> DocumentDetail { get; set; } = new List<AttachmentDetailModel>();

        public List<BillExpenseDetailModel> ExpenseDetail { get; set; }
        public bool PFInit {get;set;}
    }
    #endregion
    #region Project Fund Transfer
    public class ProjectFundTransferModel
    {
        public Nullable<Int32> ProjectTransferId { get; set; }
        public string TransferNumber { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }

        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }

        [Required]
        public Nullable<Int32> Source { get; set; }
        public string Narration { get; set; }
        public string Status { get; set; }
        public string PostedDate { get; set; }
        public int SlNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Expense Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CreditAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> DebitAmount { get; set; }

        public HttpPostedFileBase Document { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentActualName { get; set; }
        [Required]
        [Display(Name = "Debit Project")]
        public Nullable<Int32> DebitProjectId { get; set; }
        public string DebitReason { get; set; }
        public string DebitProject { get; set; }
        [Required]
        [Display(Name = "Credit Project")]
        //[NotEqualTo("DebitProjectId", ErrorMessage = "Credit project and Debit project should not be same.")]
        public Nullable<Int32> CreditProjectId { get; set; }
        public string CreditReason { get; set; }
        public string CreditProject { get; set; }
        public List<ProjectTransferDetailModel> CrDetail { get; set; }
        public List<ProjectTransferDetailModel> DrDetail { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public bool PFInit { get; set; }
    }
    public class SearchProjectFunTransferModel
    {
        public string ProjectFundTransferNumber { get; set; }
        public string FrmFundProjectNumber { get; set; }
        public string ToFundProjectNumber { get; set; }
        public Nullable<decimal> FundAmount { get; set; }
        public string Status { get; set; }
        public int TotalRecords { get; set; }
        public List<ProjectFundTransferModel> prjfundlist { get; set; }
    }
    public class ProjectTransferSearch
    {
        public string ProjectTransferNumber { get; set; }
        public string FrmProjectNumber { get; set; }
        public string ToProjectNumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Status { get; set; }
        public int TotalRecords { get; set; }
        public List<ProjectTransferModel> prjlist { get; set; }
    }
    public class ProjectTransferDetailModel
    {
        [Required]
        public Nullable<Int32> BudgetHeadId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }
        public string BudgetHeadName { get; set; }
    }

    public class ProjectTransferModel
    {
        public Nullable<Int32> ProjectTransferId { get; set; }
        public string TransferNumber { get; set; }

        [Required]
        [Display(Name = "Debit Project")]
        public Nullable<Int32> DebitProjectId { get; set; }
        public string Remarks { get; set; }
        public string DebitProject { get; set; }
        [Required]
        [Display(Name = "Credit Project")]
        [NotEqualTo("DebitProjectId", ErrorMessage = "Credit project and Debit project should not be same.")]
        public Nullable<Int32> CreditProjectId { get; set; }
        [Required]
        [Display(Name = "Bank")]
        public Nullable<Int32> DebitBank { get; set; }
        [Required]
        [Display(Name = "Bank")]
        public Nullable<Int32> CreditBank { get; set; }
        public HttpPostedFileBase Document { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentActualName { get; set; }
        public string CreditProject { get; set; }
        public string PostedDate { get; set; }
        public string Status { get; set; }
        public Nullable<int> SlNo { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Credit Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CreditAmount { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Debit Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> DebitAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Commitment Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CommitmentAmount { get; set; }
        public List<BillCommitmentDetailModel> CommitmentDetail { get; set; } = new List<BillCommitmentDetailModel>();
        public string CreditBankName { get; set; }
        public string DebitBankName { get; set; }
        public bool PFInit { get; set; }
        public int CreditLedgerId { get; set; }
        public int DebitLedgerId { get; set; }
        public bool DebitIcsroh_f { get; set; }
        public bool CreditIcsroh_f { get; set; }
        public string CreditLedgerName { get; set; }
        public string DebitLedgerName { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }
        [Required]
        public Nullable<Int32> Source { get; set; }
        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }

        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }
    }
    #endregion
    #region Contra
    public class ContraModel
    {
        public Nullable<Int32> ContraId { get; set; }
        public string ContraNumber { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }

        public string ReferenceNumber { get; set; }

        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }

        [Required]
        public Nullable<Int32> Source { get; set; }
        public string Narration { get; set; }
        public string Status { get; set; }
        public string PostedDate { get; set; }
        public int SlNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Expense Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CreditAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> DebitAmount { get; set; }

        public HttpPostedFileBase Document { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentActualName { get; set; }
        
        public List<ContraDetailModel> CrDetail { get; set; }
        public List<ContraDetailModel> DrDetail { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public bool PFInit { get; set; }
        public string BillRefNoAC { get; set; }
        public string BillRefNo { get; set; }
    }
    public class SearchContra
    {
        public string ContraNumber { get; set; }
        public string Status { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public int TotalRecords { get; set; }
        public decimal Amount { get; set; }
        public List<ContraModel> Contralist { get; set; }
    }
    public class ContraDetailModel
    {
        [Required]
        [Display(Name = "Account Group")]
        public Nullable<Int32> AccountGroupId { get; set; }

        public List<MasterlistviewModel> AccountHeadList { get; set; } = new List<MasterlistviewModel>();

        [Required]
        [Display(Name = "Account Head")]
        public Nullable<Int32> AccountHeadId { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }
        public string AccountHeadName { get; set; }
        public string AccountGroupName { get; set; }
    }
    #endregion
    

    #region Honororium
    public class SearchHonororium
    {
        public string InHonororiumNumber { get; set; }
        public string InStatus { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string ExHonororiumNumber { get; set; }
        public string ExStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<HonororiumModel> Honororiumlist { get; set; }
    }


    public class HonororiumModel : CommonPaymentModel
    {
        public string HonDate { get; set; }
        public int HonororiumId { get; set; }
        public int SlNo { get; set; }
        public string HonororiumDate { get; set; }
        public int CategoryId { get; set; }
        public string Status { get; set; }
        public string HonororiumNo { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }


        [Required]
        [Display(Name = "Bank Name")]
        public Nullable<Int32> BankId { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }
        public string BankName { get; set; }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }
        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }
        [Required]
        public Nullable<Int32> Source { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> TotalTDS { get; set; }
        public Nullable<decimal> NetPayableAmount { get; set; }
        public Nullable<decimal> TotalPayableToPCF { get; set; }
        public Nullable<decimal> TotalPayableToOH { get; set; }
        public Nullable<decimal> NetTotal { get; set; }
        public string Remarks { get; set; }
        public string RequestReceivedFrom { get; set; }
        public List<HonororiumListModel> PODetail { get; set; }
        public List<HonororiumPCFModel> PCFDetail { get; set; }
        public honororiumSearchFieldModel SearchField { get; set; }
        public bool BillProcessingStatus { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public bool PFInit { get; set; }
    }

    public class HonororiumExportListModel
    {
        public double SNo { get; set; }
        public string PayeeType { get; set; }
        public string UserId { get; set; }
        public string Name{ get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> NetAmount { get; set; }
        public Nullable<decimal> TDS { get; set; }
        public Nullable<decimal> TDSAmt { get; set; }
        public string PaymentModeName { get; set; }
        public Nullable<int> PaymentModeVal { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string IFSC { get; set; }
        public string AccountNo { get; set; }
        public string PAN { get; set; }
        public string SelectedTdssection { get; set; }
        public string SelectedTdssectionID { get; set; }
        public string Status { get; set; }
    
    }
    public class HonororiumListModel
    {
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> TDS { get; set; }
        public Nullable<decimal> NetAmount { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string IFSC { get; set; }
        public string AccountNo { get; set; }
        [NotMapped]
        public bool PANReq
        {
            get
            {
                return (!String.IsNullOrEmpty(this.PAN));
            }
        }
        [RequiredIf("PANReq", true, ErrorMessage = "PAN field is required")]
        [RegularExpression("[A-Za-z]{5}[0-9]{4}[A-Za-z]{1}", ErrorMessage = "Invalid PAN Number")]
        public string PAN { get; set; }
        public string PayeeType { get; set; }
        public string Name { get; set; }
        public Nullable<int> UserId { get; set; }
        public string tdsdropdown { get; set; }
        public Nullable<int> PaymentMode { get; set; }
        public string NonAutoComplete { get; set; }
        public string PaymentModeName { get; set; }
        public string TDSPercent { get; set; }
        public Nullable<int> HonororiumTdsSection { get; set; }
        public string SelectedTdssection { get; set; }
    }
    public class HonororiumPCFModel
    {
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public Nullable<decimal> PCFAmount { get; set; }
        public string PCFName { get; set; }
        public Nullable<int> PCFUserId { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public Nullable<decimal> PayableToPCF { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public Nullable<decimal> PayableToOH { get; set; }
        public string OHDropdown { get; set; }


    }
    public class honororiumSearchFieldModel
    {
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string HonororiumNumber { get; set; }
        public string Status { get; set; }
        public string ReqResFrom { get; set; }

    }
    #endregion

    #region FellowShip
    public class SearchFellowShip
    {
        public string InFellowShipNumber { get; set; }
        public string InProjectNumber { get; set; }
        public string InStatus { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string ExFellowShipNumber { get; set; }
        public string ExProjectNumber { get; set; }
        public string ExStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<FellowShipModel> FellowShiplist { get; set; }
    }
    public class FellowShipModel
    {
        public int SlNo { get; set; }
        public int ViewId { get; set; }
        public int EditId { get; set; }
        public int ReviseId { get; set; }
        public int ReviseEditId { get; set; }
        public bool ReviseCheckBox1 { get; set; }
        public bool ReviseCheckBox2 { get; set; }
        public string Status { get; set; }
        public int RevisionNo { get; set; }
        public int FellowShipId { get; set; }
        public string FellowShipNumber { get; set; }
        public string FellowShipDate { get; set; }
        public string FromDate { get; set; }
        public string ViewFromDate { get; set; }
        public string DueFromDate { get; set; }
        public string DueDate { get; set; }
        public string ToDate { get; set; }
        public string ViewToDate { get; set; }
        public string DueToDate { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectNo { get; set; }
        public string ViewProjectNo { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string UserId { get; set; }
        public int PI { get; set; }
        public string PayeeName { get; set; }
        public string ViewPayeeName { get; set; }
        public Nullable<decimal> FellowShipValue { get; set; }
        public Nullable<decimal> ViewFellowShipValue { get; set; }
        public Nullable<decimal> NewValue { get; set; }
        public string CommitmentNo { get; set; }
        public string ReviseCommitmentNo { get; set; }
        public string ViewBagCommitNo { get; set; }
        public string ViewCommitmentNo { get; set; }
        public Nullable<decimal> AvailableBalance { get; set; }
        public Nullable<decimal> viewAvailableBalance { get; set; }
        public List<FellowShipRevisionModel> FellowRevision { get; set; }
        public FellowShipSearchFieldModel SearchField { get; set; }
    }
    public class FellowShipRevisionModel
    {
        public int Slno { get; set; }
        public string Date { get; set; }
        public string CommitNo { get; set; }
        public string OldDate { get; set; }
        public string NewDate { get; set; }
        public Nullable<decimal> NewValue { get; set; }
        public Nullable<decimal> OldValue { get; set; }
    }
    public class FellowShipSearchFieldModel
    {
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string FellowShipNumber { get; set; }
        public string Status { get; set; }
        public string PI { get; set; }
        public string ProjectNumber { get; set; }
    }
    #endregion
    #region FellowShipSalary
    public class SearchFellowshipSalary
    {
        public string InFellowshipSalaryNumber { get; set; }
        public string InStatus { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string ExFellowshipSalaryNumber { get; set; }
        public string ExStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<FellowshipSalaryModel> FellowshipSalarylist { get; set; }
    }
    public class FellowshipSalaryModel : CommonPaymentModel
    {
        public string FellowshipNo { get; set; }
        public string PI { get; set; }
        public decimal EditAmount { get; set; }
        public int SlNo { get; set; }
        public string ProjectNumber { get; set; }
        public int ProjectId { get; set; }
        public string PayeeName { get; set; }
        public int PayeeId { get; set; }
        public string FellowShipNumber { get; set; }
        public string FellowShipDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> FellowShipValue { get; set; }
        public int FellowShipId { get; set; }
        public int RevisionNo { get; set; }
        public int FellowshipSalId { get; set; }
        public string FellowshipSalNo { get; set; }
        public string Remarks { get; set; }

        public string MonthYear { get; set; }
        public string HiddenMonthYear { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Balance { get; set; }
        public decimal AvailBalance { get; set; }
        public string CommitmentNo { get; set; }
        public List<FellowshipSalaryListModel> ListModel { get; set; }
        public bool PFInit { get; set; }
    }
    public class FellowshipSalaryListModel
    {

        public int SlNo { get; set; }
        public string ProjectNumber { get; set; }
        public int ProjectId { get; set; }
        public string PayeeName { get; set; }
        public int PayeeId { get; set; }
        public string FellowShipNumber { get; set; }
        public string FellowShipDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> FellowShipValue { get; set; }
        public int FellowShipId { get; set; }
        public int RevisionNo { get; set; }
        public int FellowshipSalId { get; set; }
        public string FellowshipSalNo { get; set; }
        public string Remarks { get; set; }
        public Nullable<DateTime> HiddenMonthYear { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Balance { get; set; }
        public decimal AvailBalance { get; set; }
        public string CommitmentNo { get; set; }
    }
    #endregion
    #region Institute Salary Payment
    public class SearchInstituteSalary
    {
        public string InInstituteSalaryNumber { get; set; }
        public string InStatus { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string ExInstituteSalaryNumber { get; set; }
        public string ExStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<InstituteSalaryPaymentModel> InstituteSalarylist { get; set; }
    }
    public class InstituteSalaryPaymentModel : CommonPaymentModel
    {
        public int Slno { get; set; }
        public string InstituteSalaryPaymentNo { get; set; }
        public int InstituteSalaryPaymentId { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string InstituteSalaryDate { get; set; }

        public string MonthYear { get; set; }
        public string HiddenMonthYear { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public decimal Amount { get; set; }
        public List<InstituteSalaryPaymentListModel> IMS { get; set; }
    }

    public class InstituteSalaryPaymentListModel
    {
        public int Slno { get; set; }
        public int EmployeeOtherAllowanceId { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
    }
    public class SearchGridInstituteSalary
    {
        public string Category { get; set; }
        public string Type { get; set; }
        public int TotalRecords { get; set; }
        public string Month { get; set; }
        public List<InstituteSalaryPaymentListModel> InstituteSalarylist { get; set; }
    }
    #endregion


    #region Institute Claims
    public class SearchInstituteClaims
    {
        public string InInstituteClaimsNumber { get; set; }
        public string InStatus { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string ExInstituteClaimsNumber { get; set; }
        public string ExStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<InstituteClaims> InstituteClaimslist { get; set; }
    }
    public class InstituteClaims : CommonPaymentModel
    {
        public int SlNo { get; set; }
        public int BankHeadId { get; set; }
        public int ReceiptId { get; set; }
        [Display(Name = "File Document")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "AttachmentFile is Required")]
        public HttpPostedFileBase AttachmentFile { get; set; }
        public string AttachmentPath { get; set; }
        public string AttachmentActualName { get; set; }
        public string AttachmentName { get; set; }
        [Display(Name = "File Document")]
        // [Required(AllowEmptyStrings = false, ErrorMessage = "AttachmentFil is Required")]
        public HttpPostedFileBase AttachmentFil { get; set; }
        public string AttachmentPat { get; set; }
        public string AttachmentActualNam { get; set; }
        public string AttachmentNam { get; set; }
        public int InstituteClaimId { get; set; }
        public int ExpenseId { get; set; }
        public int ReceiptInstituteClaimId { get; set; }
        public string InstituteClaimNumber { get; set; }
        public string Status { get; set; }
        public string InstituteClaimDate { get; set; }
        public string ReceiptRefNumber { get; set; }
        // [Required(AllowEmptyStrings = false, ErrorMessage = "ClaimDate is Required")]
        [Required]
        public Nullable<DateTime> ClaimDate { get; set; }
        public string ClaimType { get; set; }
        public int ClaimTypeId { get; set; }
        public string BudgetHead { get; set; }
        public int BudegetHeadId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "PublicFundReversal is Required")]
        public string PublicFundR { get; set; }
        public bool PublicFundReversal { get; set; }
        public string Description { get; set; }
        public string TravelBillNo { get; set; }
        public string PurchaseNo { get; set; }
        public string Others { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "FacilityUsed is Required")]
        public string FacilityUsed { get; set; }
        public int FacilityUsedId { get; set; }
        public decimal ClaimAmount { get; set; }
        public decimal ReceiptTotalAmount { get; set; }
        [Required]
        public Nullable<DateTime> ReceiptDate { get; set; }
        public string ReceiptNumber { get; set; }
        public decimal ReceiptValue { get; set; }
        public string Projectno { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "PublicFundReversal is Required")]
        public int Projectid { get; set; }
        public string Comments { get; set; }
        public int ModeOfReceipt { get; set; }
        public string ChequeNo { get; set; }
        [Required]
        public Nullable<DateTime> TransDate { get; set; }
        public string BankRefNo { get; set; }
        public decimal RecAmount { get; set; }
        [Required]
        public string CommitmentNo { get; set; }
        public int CommitmentId { get; set; }
        public string ViewBagCommitNo { get; set; }
        public int ViewBagCommitId { get; set; }

        public decimal AvailableBalance { get; set; }
        // public List<ClaimReceiptList> REC { get; set; }
        public List<ClaimsExpenseDetailModel> ExpenseDetai { get; set; }
        public List<ClaimsDeductionDetailModel> DeductionDetai { get; set; }
    }
    //public class ClaimReceiptList
    //{
    //    public int ModeOfReceipt { get; set; }
    //    public string ChequeNo { get; set; }     
    //    public Nullable<DateTime> TransDate { get; set; }
    //    public string BankRefNo { get; set; }
    //    public decimal RecAmount { get; set; }


    //}
    public class ClaimsExpenseDetailModel
    {
        public Nullable<Int32> BillExpenseDetailI { get; set; }
        public string BudgetHeadNam { get; set; }
        [Required]
        [Display(Name = "Account Group")]
        public Nullable<Int32> AccountGroupI { get; set; }

        public List<MasterlistviewModel> AccountGroupLis { get; set; } = new List<MasterlistviewModel>();

        public List<MasterlistviewModel> AccountHeadLis { get; set; } = new List<MasterlistviewModel>();

        [Required]
        [Display(Name = "Account Head")]
        public Nullable<Int32> AccountHeadI { get; set; }
        [Required]
        public string TransactionTyp { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amoun { get; set; }

        public bool IsJ { get; set; } = false;
    }
    public class ClaimsDeductionDetailModel
    {
        public Nullable<Int32> BillDeductionDetailId { get; set; }
        [Display(Name = "Deduction Head")]
        public string DeductionHead { get; set; }

        public Nullable<int> DeductionHeadId { get; set; }

        [Display(Name = "Account Head")]
        public string AccountGroup { get; set; }

        public Nullable<int> AccountGroupId { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }
        public string DeductionType { get; set; }
        public string TransactionTypeCode { get; set; }
        public Nullable<decimal> TDSPercentage { get; set; }
        public Nullable<int> DeductionCategoryId { get; set; }
    }
    #endregion

    #region Overheads Posting
    public class OverheadsPostingModel : CommonPaymentModel
    {
        public int SlNo { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [Display(Name = "Account Group")]
        public Nullable<Int32> AccountGroupId { get; set; }

        public List<MasterlistviewModel> AccountHeadList { get; set; } = new List<MasterlistviewModel>();
        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }
        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }
        [Required]
        public Nullable<Int32> Source { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string OverheadsPostingNumber { get; set; }
        public Nullable<int> OverheadsPostingId { get; set; }
        public Nullable<Decimal> TotalAmount { get; set; }
        public Nullable<Decimal> TotalCorpusAmount { get; set; }
        public Nullable<Decimal> TotalICSROHAmount { get; set; }
        public Nullable<Decimal> TotalPCFAmount { get; set; }
        public Nullable<Decimal> TotalRMFAmount { get; set; }
        public Nullable<Decimal> TotalDDFAmount { get; set; }
        public Nullable<Decimal> TotalStaffWelfareAmount { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public string PostedDate { get; set; }
        public List<OverheadsDetailModel> OverheadsDetails { get; set; }
        public List<OverheadsPIShareDetailModel> PIShareDetails { get; set; }
        public List<OverheadsPIShareDetailModel> PIPCFShareDetails { get; set; }
        public List<OverheadsPIShareDetailModel> PIRMFShareDetails { get; set; }
        public List<OverheadsPCFCreditsModel> PCFCreditDetails { get; set; }
        public List<OverheadsRMFCreditsModel> RMFCreditDetails { get; set; }
        public List<OverheadsCorpusCreditsModel> CorpusCreditDetails { get; set; }
        public List<OverheadsICSROHCreditsModel> ICSRCreditDetails { get; set; }
        public List<OverheadsDDFCreditsModel> DDFCreditDetails { get; set; }
        public List<OverheadsStaffwelfareCreditsModel> StaffWelfareCreditDetails { get; set; }
        //public List<ContraDetailModel> CrDetail { get; set; }
        //public List<ContraDetailModel> DrDetail { get; set; }
        public string Narration { get; set; }
        public Nullable<Int32> ContraId { get; set; }
        public HttpPostedFileBase Document { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentActualName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ProjectNumber { get; set; }
        public string ReceiptNumber { get; set; }
        public Nullable<Int32> ProjectId { get; set; }
        public Nullable<Int32> SrchProjectId { get; set; }
        public Nullable<Int32> ReceiptId { get; set; }
        public Nullable<Int32> BtnPCF { get; set; }
        public Nullable<Int32> BtnRMF { get; set; }
       
    }
    public class OverheadsDetailModel
    {
        public Nullable<int> ReceiptId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<int> OverheadsPostingDetailsId { get; set; }
        public Nullable<int> OverheadsTypeId { get; set; }
        public string ReceiptNumber { get; set; }
        public string ReceiptDate { get; set; }
        public Nullable<Decimal> TotalOverheadsValue { get; set; }
        public Nullable<Decimal> CorpusPercent { get; set; }
        public Nullable<Decimal> CorpusAmount { get; set; }
        public Nullable<Decimal> RMFPercent { get; set; }
        public Nullable<Decimal> RMFAmount { get; set; }
        public Nullable<Decimal> ICSROHPercent { get; set; }
        public Nullable<Decimal> ICSROHAmount { get; set; }
        public Nullable<Decimal> DDFPercent { get; set; }
        public Nullable<Decimal> DDFAmount { get; set; }
        public Nullable<Decimal> StaffWelfarePercent { get; set; }
        public Nullable<Decimal> StaffWelfareAmount { get; set; }
        public Nullable<Decimal> PCFPercent { get; set; }
        public Nullable<Decimal> PCFAmount { get; set; }
        public Nullable<Decimal> TtlPCFPercent { get; set; }
        public Nullable<Decimal> TtlRMFPercent { get; set; }
        public string NameofPI { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<Decimal> ProjectTotalOverheadsvalue { get; set; }
        public Nullable<Decimal> CurrentReceiptOverheads { get; set; }
        public Nullable<Decimal> PrevReceiptsOverheads { get; set; }
        public Nullable<Decimal> ProjectTotalValue { get; set; }

    }
    public class OverheadsPIShareDetailModel
    {
        public Nullable<int> OverheadsPostingDetailsId { get; set; }
        public Nullable<int> OverheadsPIShareDetailsId { get; set; }
        public string OverheadsPostingNumber { get; set; }
        public string NameofPI { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> ReceiptId { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public Nullable<Decimal> PCFPercent { get; set; }
        public Nullable<Decimal> OldPCFPercent { get; set; }
        public Nullable<Decimal> PCFAmount { get; set; }
        public Nullable<Decimal> RMFPercent { get; set; }
        public Nullable<Decimal> OldRMFPercent { get; set; }
        public Nullable<Decimal> RMFAmount { get; set; }
        public Nullable<Decimal> TotalRMFAmount { get; set; }
        public Nullable<Decimal> TotalPCFAmount { get; set; }
    }
    public class OverheadsPCFCreditsModel
    {
        public Nullable<int> OverheadsTypeId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> ReceiptId { get; set; }
        public string ReceiptNumber { get; set; }
        public List<MasterlistviewModel> ProjectNumber { get; set; } = new List<MasterlistviewModel>();
        public List<MasterlistviewModel> Bank { get; set; } = new List<MasterlistviewModel>();
        public string NameofPI { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<Decimal> Amount { get; set; }
        public Nullable<Decimal> Percent { get; set; }
        public string PCFProjectNumber { get; set; }
        public string PCFBankName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<int> Id { get; set; }
        public Nullable<int> BankId { get; set; }
    }
    public class OverheadsRMFCreditsModel
    {
        public Nullable<int> OverheadsTypeId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> ReceiptId { get; set; }
        public string ReceiptNumber { get; set; }
        public List<MasterlistviewModel> ProjectNumber { get; set; }
        public List<MasterlistviewModel> Bank { get; set; } = new List<MasterlistviewModel>();
        public Nullable<int> BankId { get; set; }
        public string NameofPI { get; set; }
        public string RMFBankName { get; set; }
        public string RMFProjectNumber { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<Decimal> Amount { get; set; }
        public Nullable<Decimal> Percent { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<int> Id { get; set; }
    }
    public class OverheadsCorpusCreditsModel
    {
        public Nullable<int> OverheadsTypeId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> ReceiptId { get; set; }
        public string ReceiptNumber { get; set; }
        public List<MasterlistviewModel> ProjectNumber { get; set; }
        public List<MasterlistviewModel> Bank { get; set; } = new List<MasterlistviewModel>();
        public Nullable<int> BankId { get; set; }
        public string NameofPI { get; set; }
        public string CorpusBankName { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<Decimal> Amount { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<int> Id { get; set; }
    }
    public class OverheadsICSROHCreditsModel
    {
        public Nullable<int> OverheadsTypeId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> ReceiptId { get; set; }
        public string ReceiptNumber { get; set; }
        public List<MasterlistviewModel> ProjectNumber { get; set; }
        public List<MasterlistviewModel> Bank { get; set; } = new List<MasterlistviewModel>();
        public Nullable<int> BankId { get; set; }
        public string NameofPI { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<Decimal> Amount { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string ICSROHBankName { get; set; }
        public string ICSROHProjectNumber { get; set; }
        public Nullable<int> Id { get; set; }
    }
    public class OverheadsStaffwelfareCreditsModel
    {
        public Nullable<int> OverheadsTypeId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> ReceiptId { get; set; }
        public string ReceiptNumber { get; set; }
        public List<MasterlistviewModel> ProjectNumber { get; set; }
        public List<MasterlistviewModel> Bank { get; set; } = new List<MasterlistviewModel>();
        public Nullable<int> BankId { get; set; }
        public string NameofPI { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<Decimal> Amount { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string SwfBankName { get; set; }
        public string SwfProjectNumber { get; set; }
        public Nullable<int> Id { get; set; }
    }
    public class OverheadsDDFCreditsModel
    {
        public Nullable<int> OverheadsTypeId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> ReceiptId { get; set; }
        public string ReceiptNumber { get; set; }
        public List<MasterlistviewModel> ProjectNumber { get; set; }
        public List<MasterlistviewModel> Bank { get; set; } = new List<MasterlistviewModel>();
        public Nullable<int> BankId { get; set; }
        public string NameofPI { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<Decimal> Amount { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DDFBankName { get; set; }
        public string DDFProjectNumber { get; set; }
        public Nullable<int> Id { get; set; }
    }
    public class OverheadsPostingSearch
    {
        public string OverheadsNumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Status { get; set; }
        public List<OverheadsPostingModel> list { get; set; }
        public int TotalRecords { get; set; }
    }
    #endregion

    #region Distribution Overheads Posting
    public class DistributionOHPostingModel : CommonPaymentModel
    {
        public int SlNo { get; set; }

        public string Status { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<Decimal> TotalDistributionAmount { get; set; }

        public string OverheadsPostingNumber { get; set; }
        public Nullable<int> OverheadsPostingId { get; set; }

        public Nullable<int> ProjectType { get; set; }
        public string PostedDate { get; set; }
        public List<DistributionOHDetailModel> OverheadsDetails { get; set; }
        public List<DistributionOHPCFCreditsModel> PCFCreditDetails { get; set; }
        public List<DistributionOtherShareDetailModel> OtherShareDetails { get; set; }
        public DOHSearchFieldModel SearchField { get; set; }
        public PagedData<DOHSearchResultModel> SearchResult { get; set; }
        public string PaymentType { get; set; }
        public Nullable<int> PaymentTypeId { get; set; }
        //public string PaymentNumber { get; set; }
        public Nullable<int> PaymentNumberId { get; set; }
        public Nullable<Decimal> TotalCreditAmount { get; set; }
        public string ConsRCVFromDate { get; set; }
        public string ConsRCVToDate { get; set; }

    }
    public class DistributionOHDetailModel
    {
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> OverheadsPostingDetailsId { get; set; }
        public Nullable<int> OverheadsTypeId { get; set; }
        public string OverheadsType { get; set; }
        public Nullable<Decimal> OverheadsAmount { get; set; }
        public Nullable<Decimal> OverheadsPercent { get; set; }
        public Nullable<Decimal> TotalOverheadsValue { get; set; }
        public string OHProjectNumber { get; set; }
        public string NameofPI { get; set; }
        public string PjctNumber { get; set; }
        public List<MasterlistviewModel> ProjectNumber { get; set; } = new List<MasterlistviewModel>();
        public List<MasterlistviewModel> Bank { get; set; } = new List<MasterlistviewModel>();
        public Nullable<int> Id { get; set; }
        public Nullable<int> BankId { get; set; }
    }

    public class DistributionOtherShareDetailModel
    {
        public List<MasterlistviewModel> ProjectNumber { get; set; } = new List<MasterlistviewModel>();
        public List<MasterlistviewModel> Bank { get; set; } = new List<MasterlistviewModel>();
        public string NameofPI { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<int> PaymentType { get; set; }
        public Nullable<Decimal> Amount { get; set; }
        public Nullable<int> Id { get; set; }
        public Nullable<int> BankId { get; set; }
        public string Head { get; set; }
    }
    public class DistributionOHPCFCreditsModel
    {
        public Nullable<int> OverheadsTypeId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public List<MasterlistviewModel> ProjectNumber { get; set; } = new List<MasterlistviewModel>();
        public List<MasterlistviewModel> Bank { get; set; } = new List<MasterlistviewModel>();
        public string NameofPI { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<Decimal> Amount { get; set; }
        public Nullable<int> Id { get; set; }
        public Nullable<int> BankId { get; set; }
    }

    public class SearchPCFDistributionOH
    {
        public string SearchINPaymentType { get; set; }
        public string SearchINDistributionOHNumber { get; set; }
        public string SearchINDOHPaymentType { get; set; }
        public string SearchINDistributionOHAmount { get; set; }
        public string SearchEXPaymentType { get; set; }
        public string SearchEXDistributionOHNumber { get; set; }
        public string SearchEXDOHPaymentType { get; set; }
        public string SearchEXDistributionOHAmount { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string SearchINStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<DistributionOHPostingModel> DOHPList { get; set; }
    }
    public class DOHSearchResultModel
    {
        public Nullable<int> SlNo { get; set; }
        public Nullable<int> DOHId { get; set; }
        public string DOHNumber { get; set; }
        public string Paymenttype { get; set; }
        public string PostedDate { get; set; }
        public string Status { get; set; }
    }

    public class DOHSearchFieldModel
    {
        public string DOHNumber { get; set; }
        public string Paymenttype { get; set; }
        public string SearchBy { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
    }
    public class DateFilterModel
    {
        public Nullable<DateTime> to { get; set; }
        public Nullable<DateTime> from { get; set; }

    }
    #endregion

    #region Addition and Reversal 
    public class SearchOHReversal
    {
        public string InOHReversalNumber { get; set; }
        public string InStatus { get; set; }
        public string InProjectNo { get; set; }
        public string ExProjectNo { get; set; }
        public string InType { get; set; }
        public string ExType { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string ExOHReversalNumber { get; set; }
        public string ExStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<AdditionandReversalModel> OHReversallist { get; set; }
    }
    public class AdditionandReversalModel : CommonPaymentModel
    {
        public int ProjectType { get; set; }
        public string ProjCategory { get; set; }
        public string Remarks { get; set; }
        public int SlNo { get; set; }
        public bool Checkbox { get; set; }
        public string Status { get; set; }
        public string OHReversalDate { get; set; }
        public int AdditionandReversalId { get; set; }
        public string AdditionandReversalNumber { get; set; }
        public string Type { get; set; }
        public int ProjectId { get; set; }
        public string ProjectNo { get; set; }
        public string PostedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Corpus { get; set; }
        public decimal PCF { get; set; }
        public decimal RMF { get; set; }
        public decimal ICSROH { get; set; }
        public decimal DDF { get; set; }
        public decimal StaffWelfare { get; set; }
        public List<PCFandRMFModel> ShareList { get; set; }
        public List<ReceiptDetailsModel> ReceiptList { get; set; }
        public OHReversalSearchFieldModel SearchField { get; set; }
        public bool PFInit { get; set; }
    }
    public class PCFandRMFModel
    {
        public int SlNo { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public int PIid { get; set; }
        public int PCFProjId { get; set; }
        public int RMFProjId { get; set; }
        public string Role { get; set; }
        public decimal PCFPer { get; set; }
        public decimal PCFValue { get; set; }
        public decimal RMFPer { get; set; }
        public decimal RMFValue { get; set; }
    }
    public class ReceiptDetailsModel
    {
        public int SlNo { get; set; }
        public int PIid { get; set; }
        public string Category { get; set; }
        public string Role { get; set; }
        public string ProjectNumber { get; set; }
        public int ProjectId { get; set; }
        public int BankHeadId { get; set; }
        public int Classification { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
    }
    public class OHReversalSearchFieldModel
    {
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string OHReversalNumber { get; set; }
        public string Status { get; set; }

    }

    #endregion

    #region TDS Payment
    public class SearchTDSPayment
    {
        public string InTDSPaymentNumber { get; set; }
        public string InStatus { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public Nullable<DateTime> Month { get; set; }
        public string ExTDSPaymentNumber { get; set; }
        public string ExStatus { get; set; }
        public int TotalRecords { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public decimal Amount { get; set; }
        public int Bank { get; set; }
        public List<TDSPaymentModel> TDSPaymentlist { get; set; }
    }
    public class TDSPaymentModel
    {
        public string Month { get; set; }
        public int SlNo { get; set; }
        public string Status { get; set; }
        public int BankId { get; set; }
        public int FinalPayment { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public string TDSPaymentDate { get; set; }

        public Nullable<DateTime> ToDate { get; set; }
        public Nullable<DateTime> DateOfPayment { get; set; }
        public string ChallanNo { get; set; }
        public string BankRefNo { get; set; }
        public string Remark { get; set; }

        [Display(Name = "File Document")]
        public HttpPostedFileBase AttachmentFile { get; set; }
        public string AttachmentPath { get; set; }
        public string AttachmentActualName { get; set; }
        public string AttachmentName { get; set; }
        public bool payment { get; set; }
        public int BoaTransId { get; set; }
        public int TDSPaymentId { get; set; }
        public string TDSPaymentNumber { get; set; }
        public int Category { get; set; }
        public string CategoryName { get; set; }
        public string SectionName { get; set; }
        public Nullable<int> Section { get; set; }
        public int HeadId { get; set; }
        public string Head { get; set; }
        public Nullable<decimal> TotalTDS { get; set; }
        public Nullable<decimal> TotalGST { get; set; }
        public Nullable<decimal> TotalCGST { get; set; }
        public Nullable<decimal> TotalIGST { get; set; }
        public Nullable<decimal> TotalSGST { get; set; }
        public Nullable<decimal> BankGST { get; set; }
        public Nullable<decimal> CGSTTrans { get; set; }
        public Nullable<decimal> SGSTTrans { get; set; }
        public Nullable<decimal> IGSTTrans { get; set; }
        public Nullable<decimal> BankTransaction { get; set; }
        public Nullable<decimal> TDSTransaction { get; set; }
        public List<TDSPaymentListModel> TDSIncomeTax { get; set; }
        public List<TDSGSTListModel> TDSGST { get; set; }
        public TDSPaymentSearchFieldModel SearchField { get; set; }
        public string Bank { get; set; }
        [Required]
        public int PaymentBank { get; set; }
    }
    public class TDSPaymentListModel
    {
        public int SlNo { get; set; }
        public int BoaTransId { get; set; }
        public bool TDSCheckbox { get; set; }
        public string Party { get; set; }
        public string PAN { get; set; }
        public string TypeName { get; set; }
        public int Type { get; set; }
        public string ReferenceNo { get; set; }
        public string DateOfTransaction { get; set; }
        public int DetailId { get; set; }
        public Nullable<decimal> TDSAmount { get; set; }
        public string Bank { get; set; }
    }
    public class TDSGSTListModel
    {
        public int SlNo { get; set; }
        public int BoaTransId { get; set; }
        public bool TDSCheckbox { get; set; }
        public string TypeName { get; set; }
        public string Party { get; set; }
        public string GSTHead { get; set; }
        public int GSTHeadId { get; set; }
        public int Type { get; set; }
        public string ReferenceNo { get; set; }
        public string DateOfTransaction { get; set; }
        public Nullable<decimal> GSTAmount { get; set; }
        public int DetailId { get; set; }
    }
    public class TDSPaymentSearchFieldModel
    {
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string TDSPaymentNumber { get; set; }
        public string Status { get; set; }
        public string CategoryName { get; set; }
        public string SectionName { get; set; }
        public int Category { get; set; }
        public int Section { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public decimal Amount { get; set; }
        public int Bank { get; set; }
    }

    #endregion



    #region GSTOffset
    public class SearchGSTOffset
    {
        public string InGSTOffsetNumber { get; set; }
        public string InStatus { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string ExGSTOffsetNumber { get; set; }
        public string ExStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<GSTOffsetModel> GSTOffsetlist { get; set; }
    }
    public class GSTOffsetModel : CommonPaymentModel
    {
        public int SlNo { get; set; }
        public string Status { get; set; }
        public int GSTOffsetId { get; set; }
        public string Remarks { get; set; }
        public string GSTOffsetNumber { get; set; }
        public string GSTOffsetDate { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "FromDate is Required")]
        public string FromDate { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "ToDate is Required")]
        public string ToDate { get; set; }
        public Nullable<decimal> TotalInput { get; set; }
        public Nullable<decimal> TotalOutput { get; set; }
        public Nullable<decimal> TotalTDSReceivable { get; set; }
        public Nullable<decimal> TotalCredit { get; set; }
        public Nullable<decimal> TotalDebit { get; set; }
        public Nullable<decimal> TotalIGSTInput { get; set; }
        public Nullable<decimal> TotalCGSTInput { get; set; }
        public Nullable<decimal> TotalSGSTInput { get; set; }
        public Nullable<decimal> TotalIGSTOutput { get; set; }
        public Nullable<decimal> TotalCGSTOutput { get; set; }
        public Nullable<decimal> TotalSGSTOutput { get; set; }
        public Nullable<decimal> CreditAdjIGST { get; set; }
        public Nullable<decimal> CreditAdjCGST { get; set; }
        public Nullable<decimal> CreditAdjSGST { get; set; }
        public Nullable<decimal> IGSTBalance { get; set; }
        public Nullable<decimal> SGSTBalance { get; set; }
        public Nullable<decimal> CGSTBalance { get; set; }
        public Nullable<decimal> HiddenCGSTBalance { get; set; }
        public Nullable<decimal> HiddenIGSTBalance { get; set; }
        public Nullable<decimal> HiddenSGSTBalance { get; set; }
        public Nullable<decimal> TotalReceivable { get; set; }
        public Nullable<decimal> TDSIGST { get; set; }
        public Nullable<decimal> TDSSGST { get; set; }
        public Nullable<decimal> TDSCGST { get; set; }
        public Nullable<decimal> PayableIGST { get; set; }
        public Nullable<decimal> HiddenPayableIGST { get; set; }
        public Nullable<decimal> PayableSGST { get; set; }
        public Nullable<decimal> HiddenPayableSGST { get; set; }
        public Nullable<decimal> PayableCGST { get; set; }
        public Nullable<decimal> HiddenPayableCGST { get; set; }
        public Nullable<decimal> IGSTadjInCGST { get; set; }
        public Nullable<decimal> IGSTadjInSGST { get; set; }
        public Nullable<decimal> CGSTadjInIGST { get; set; }
        public Nullable<decimal> SGSTadjInIGST { get; set; }
        public Nullable<decimal> AdjPayableIGST { get; set; }
        public Nullable<decimal> AdjPayableSGST { get; set; }
        public Nullable<decimal> AdjPayableCGST { get; set; }
        public Nullable<decimal> NetPayableCGST { get; set; }
        public Nullable<decimal> NetPayableSGST { get; set; }
        public Nullable<decimal> NetPayableIGST { get; set; }
        public Nullable<decimal> BalanceTDSinIGST { get; set; }
        public Nullable<decimal> BalanceTDSinSGST { get; set; }
        public Nullable<decimal> BalanceTDSinCGST { get; set; }
        public Nullable<decimal> TotalGSTPayable { get; set; }
        public Nullable<decimal> TotalTDS { get; set; }
        public Nullable<decimal> PreviousIGST { get; set; }
        public Nullable<decimal> PreviousSGST { get; set; }
        public Nullable<decimal> PreviousCGST { get; set; }
        public Nullable<decimal> TotalRCM { get; set; }
        public Nullable<decimal> RCMCredit { get; set; }
        public List<GSTOffsetOutputModel> GSTOffsetOutput { get; set; }
        public List<GSTOffsetInputModel> GSTOffsetInput { get; set; }
        public List<GSTOffsetTDSModel> GSTOffsetTDS { get; set; }
        public List<RCMInputModel> RCMInput { get; set; }
        public List<RCMOuputModel> RCMOutput { get; set; }
        public GSTOffsetSearchFieldModel SearchField { get; set; }
        public GSTOffsetTotalAmtModel TotalModel { get; set; }

    }
    public class GSTOffsetOutputModel
    {
        public string ProjNo { get; set; }
        public int SlNo { get; set; }
        public bool GSTCheckbox { get; set; }

        public int BOATransactionId { get; set; }
        public int AccountHeadId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string TransactionType { get; set; }
        public string OutputTransaction { get; set; }
        public string OutputTransactionCode { get; set; }
        public string OutputNumber { get; set; }
        public string OutputHead { get; set; }
        public Nullable<decimal> OutputAmount { get; set; }
        public int OutputId { get; set; }
        public int OutputHeadId { get; set; }
        public bool OutputFlag { get; set; }
        public Nullable<decimal> OutputCredit { get; set; }
        public Nullable<decimal> OutputDebit { get; set; }
    }
    public class GSTOffsetInputModel
    {
        public string ProjNo { get; set; }
        public bool GSTCheckbox { get; set; }
        public int SlNo { get; set; }
        public int BOATransactionId { get; set; }
        public int AccountHeadId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string TransactionType { get; set; }
        public string InputTransaction { get; set; }
        public string InputTransactionCode { get; set; }
        public string InputNumber { get; set; }
        public int InputId { get; set; }
        public int InputHeadId { get; set; }
        public Nullable<decimal> InputDebit { get; set; }
        public Nullable<decimal> InputCredit { get; set; }
        public string InputHead { get; set; }
        public Nullable<decimal> InputAmount { get; set; }
        public bool InputFlag { get; set; }
    }
    public class RCMInputModel
    {
        public int SlNo { get; set; }
        public string VendorName { get; set; }
        public string GSTIN { get; set; }
        public string InvoiceNo { get; set; }
        public string Invoicedate { get; set; }
        public decimal Amount { get; set; }
        public string Interstate { get; set; }
        public int RCMId { get; set; }
        public string TransactionTypeCode { get; set; }
    }
    public class RCMOuputModel
    {
        public int SlNo { get; set; }
        public string VendorName { get; set; }
        public string GSTIN { get; set; }
        public string InvoiceNo { get; set; }
        public string Invoicedate { get; set; }
        public decimal Amount { get; set; }
        public string Interstate { get; set; }
        public int RCMId { get; set; }
        public string TransactionTypeCode { get; set; }
    }
    public class GSTOffsetTDSModel
    {
        public string ProjNo { get; set; }
        public bool GSTCheckbox { get; set; }
        public int SlNo { get; set; }
        public int BOATransactionId { get; set; }
        public int AccountHeadId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string TransactionType { get; set; }
        public int TDSId { get; set; }
        public bool TDSFlag { get; set; }
        public int TDSHeadId { get; set; }
        public string TDSTransaction { get; set; }
        public string TDSTransactionCode { get; set; }
        public string TDSNumber { get; set; }
        public string TDSHead { get; set; }
        public Nullable<decimal> TDSAmount { get; set; }
        public Nullable<decimal> TDSCredit { get; set; }
        public Nullable<decimal> TDSDebit { get; set; }
    }
    public class GSTOffsetSearchFieldModel
    {
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string GSTOffsetNumber { get; set; }
        public string Status { get; set; }

    }
    public class GSTOffsetTotalAmtModel
    {
        public decimal IGSTip { get; set; }
        public decimal SGSTip { get; set; }
        public decimal CGSTip { get; set; }
        public decimal Input { get; set; }
        public decimal RCMIGSTip { get; set; }
        public decimal RCMSGSTip { get; set; }
        public decimal RCMCGSTip { get; set; }
        public decimal RCMip { get; set; }
        public decimal IGSTop { get; set; }
        public decimal SGSTop { get; set; }
        public decimal CGSTop { get; set; }
        public decimal Output { get; set; }
        public decimal RCMIGSTop { get; set; }
        public decimal RCMSGSTop { get; set; }
        public decimal RCMCGSTop { get; set; }
        public decimal RCMop { get; set; }
    }
    #endregion

    #region GST Credit
    public class GSTCredit
    {
        public int GSTCreditId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string GSTINNo { get; set; }
        public string GSTIN { get; set; }
        public string InvoiceNo { get; set; }
        public string RefNo { get; set; }
        public decimal TotalAmt { get; set; }
        public List<GSTCreditList> CreditList { get; set; }
    }
    public class GSTCreditList
    {
        public int SlNo { get; set; }
        public string RefNo { get; set; }
        public int RefId { get; set; }
        public int Id { get; set; }
        public string TransactionTypeCode { get; set; }
        public string VendorName { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public Nullable<DateTime> InvDate { get; set; }
        public string GSTIN { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxPer { get; set; }
        public decimal TaxAmount { get; set; }
        public string Interstate { get; set; }
        public bool CreditCheckBox { get; set; }
        public int BoaId { get; set; }
        public Nullable<DateTime> Posteddate { get; set; }
        public string Strdate { get; set; }

    }
    #endregion

    
    #region OverHead Posting Payment Process
    public class OverHeadPaymentProcessModel
    {
        public int SlNo { get; set; }
        public string Status { get; set; }
        public int Id { get; set; }
        public string OHno { get; set; }
        public string Bank { get; set; }
        public decimal Amount { get; set; }
        public int BankId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Category { get; set; }
        public string Date { get; set; }
        public List<OverHeadPaymentProcessListModel> List { get; set; }
    }
    public class OverHeadPaymentProcessListModel
    {
        public string RefNo { get; set; }
        public int Id { get; set; }
        public int SlNo { get; set; }
        public bool Checkbox { get; set; }
        public string OHNumber { get; set; }
        public string ProjetNo { get; set; }
        public decimal Amt { get; set; }
        public string Date { get; set; }
        public string Bank { get; set; }
    }
    public class SearchOverHeadPaymentProcess
    {
        public string InOHNumber { get; set; }
        public string ExOHNumber { get; set; }
        public string InStatus { get; set; }
        public string Bank { get; set; }
        public int TotalRecords { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public List<OverHeadPaymentProcessModel> OHlist { get; set; }
    }
    #endregion

    #region common
    //public class PaymentBreakUpModel
    //{
    //    [Required]
    //    [DisplayFormat(DataFormatString = "{0:n2}")]
    //    [Display(Name = "Payment Break Up Total")]
    //    [Range(0, 9999999999999999.99)]
    //    public Nullable<decimal> PaymentBUTotal { get; set; }
    //    public List<PaymentBreakUpDetailModel> PaymentBreakDetail { get; set; }
    //}
    #region Bill Status
    public class BillStatusModel
    {
        public int BillId { get; set; }
        public string BillNo { get; set; }
        public string Status { get; set; }
        public string RefNumber { get; set; }
        public Nullable<DateTime> BillDate { get; set; }
        //public Nullable<DateTime> ExpDate { get; set; }
        public Nullable<DateTime> CashDate { get; set; }
        public string CurrDate { get; set; }
        public string Message { get; set; }
        public string Remarks { get; set; }
        public int FinancialYear { get; set; }
        public int FormId { get; set; }
        public int Period { get; set; }
        public int BankId { get; set; }
        public string Command { get; set; }
        public List<AttachmentDetailModel> AttachmentDetail { get; set; }
        public List<AttachmentDetailModel2> AttachmentDetail2 { get; set; }
    }
    public class AttachmentDetailModel2
    {
        public int Id { get; set; }
        public Nullable<Int32> DocumentDetailId { get; set; }
        [Required]
        [Display(Name = "Document Type")]
        public Nullable<Int32> DocumentType { get; set; }

        [Display(Name = "File Document")]
        public HttpPostedFileBase DocumentFile { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentName { get; set; }
        public string DocumentActualName { get; set; }
        public string Remarks { get; set; }
        public string DocumentTypeName { get; set; }

    }
    #endregion

    public class VerifyPaymentProcessModel
    {
        public Nullable<Int32> DraftId { get; set; }
        public Nullable<Int32> BankHead { get; set; }
        [Required]
        public Nullable<Int32> PaymentPayeeId { get; set; }
        [Required]
        public Nullable<Int32> PaymentMode { get; set; }
        [RequiredIf("PaymentMode", 2, ErrorMessage = "IFSC field is required")]
        public string IFSC { get; set; }
        [RequiredIf("PaymentMode", 2, ErrorMessage = "Account number field is required")]
        public string AccountNumber { get; set; }
        [RequiredIf("PaymentMode", 2, ErrorMessage = "Bank name field is required")]
        public string PayeeBank { get; set; }
        public string ReferenceNumber { get; set; }
        public string Narration { get; set; }
        public List<PaymentDetailModel> transDetail { get; set; }
        //public Nullable<DateTime> PostingDate { get; set; }
    }
    public class PaymentDetailModel
    {
        public string AccountGroup { get; set; }
        public string AccountHead { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public Nullable<decimal> Amount { get; set; }
        public string TransactionType { get; set; }
        public string TransactionTypeCode { get; set; }
        public string ReferenceNumber { get; set; }
        public bool IsJV { get; set; }
    }
    public class PaymentBreakUpDetailModel
    {
        public Nullable<Int32> PaymentBreakUpDetailId { get; set; }
        [Required]
        [Display(Name = "Category")]
        public Nullable<Int32> CategoryId { get; set; }

        [Display(Name = "Name")]
        public Nullable<Int32> UserId { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Mode of payment")]
        public Nullable<Int32> ModeOfPayment { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Payment Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PaymentAmount { get; set; }

        public bool IsHaveTDS { get; set; }
        [RequiredIf("IsHaveTDS", true, ErrorMessage = "TDS Section field is required")]
        [Display(Name = "TDS Section")]
        public Nullable<Int32> TDSSection { get; set; }

        [RequiredIf("IsHaveTDS", true, ErrorMessage = "TDS Amount field is required")]
        [Display(Name = "TDS Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TDSAmount { get; set; }

        [RequiredIf("IsHaveTDS", true, ErrorMessage = "TDS Payable field is required")]
        [Display(Name = "TDS Payable")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TDSPayable { get; set; }
        public string CategoryName { get; set; }
        public string ModePaymentname { get; set; }
        public string TdsSectionName { get; set; }
    }
    public class TravelerDetailModel
    {
        public Nullable<Int32> DetailId { get; set; }
        [Required]
        [Display(Name = "Category")]
        public Nullable<Int32> CategoryId { get; set; }

        [Display(Name = "Name")]
        public Nullable<Int32> UserId { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        public string CategoryName { get; set; }
    }
    public class CommonPaymentModel
    {
        public Nullable<int> RefId { get; set; }
        public Nullable<int> CheckListVerified_By { get; set; }
        public string CheckListVerifierName { get; set; }
        public string CreditorType { get; set; } = "Vendor";
        public string TransactionTypeCode { get; set; }
        public string SubCode { get; set; } = "1";
        public string ProjectCategory { get; set; }
        public bool NeedUpdateTransDetail { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Commitment Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CommitmentAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Expense Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> ExpenseAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> DeductionAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Expense Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CreditAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> DebitAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Creditor Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CreditorAmount { get; set; }


        public List<BillCommitmentDetailModel> CommitmentDetail { get; set; } = new List<BillCommitmentDetailModel>();
        public List<BillExpenseDetailModel> ExpenseDetail { get; set; }
        public List<BillDeductionDetailModel> DeductionDetail { get; set; }
        public List<AttachmentDetailModel> DocumentDetail { get; set; } = new List<AttachmentDetailModel>();
        public List<CheckListModel> CheckListDetail { get; set; } = new List<CheckListModel>();
    }
    public class BillCommitmentDetailModel
    {
        public int SlNo { get; set; }
        public Nullable<Int32> BillCommitmentDetailId { get; set; }
        public Nullable<Int32> CommitmentId { get; set; }
        public Nullable<Int32> CommitmentDetailId { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Payment Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PaymentAmount { get; set; }

        public Nullable<decimal> ReversedAmount { get; set; }

        public string CommitmentNumber { get; set; }

        public string ProjectNumber { get; set; }

        public Nullable<Int32> ProjectId { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Available Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> AvailableAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Booked Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> BookedAmount { get; set; }

        public string HeadName { get; set; }

        public Nullable<Int32> AllocationHeadId { get; set; }

        public string PONumber { get; set; }
        public string TypeOfCommitment { get; set; }
        public string BookedDate { get; set; }
        public string VendorName { get; set; }
        public string RefUCNumber { get; set; }

        /*7800 CNA - SNA*/
        public Nullable<Int32> BankHeadId { get; set; }

    }

    public class CommitmentMasterAndDetailModel : BillCommitmentDetailModel
    {
        public Nullable<decimal> CommitmentBookedAmount { get; set; }
        public Nullable<decimal> CommitmentBalanceAmount { get; set; }
    }
    public class BillReversalExpenseDetailModel
    {
        public Nullable<Int32> BillExpenseDetailId { get; set; }
        public string BudgetHeadName { get; set; }
        [Required]
        [Display(Name = "Account Group")]
        public Nullable<Int32> AccountGroupId { get; set; }

        public List<MasterlistviewModel> AccountGroupList { get; set; } = new List<MasterlistviewModel>();

        public List<MasterlistviewModel> AccountHeadList { get; set; } = new List<MasterlistviewModel>();

        [Required]
        [Display(Name = "Account Head")]
        public Nullable<Int32> AccountHeadId { get; set; }
        [Required]
        public string TransactionType { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }

        [RequiredIf("AccountGroupId", 15, ErrorMessage = "Category field is required")]
        [Display(Name = "Category")]
        public Nullable<Int32> CategoryId { get; set; }

        //[RequiredIf("AccountGroupId", 15, ErrorMessage = "Name field is required")]
        //[Display(Name = "Name")]
        public Nullable<Int32> UserId { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        public bool IsJV { get; set; } = false;
        public string VariableName { get; set; }
        public string AccountGroupName { get; set; }
        public string AccountHeadName { get; set; }
    }
    public class BillExpenseDetailModel
    {
        public Nullable<Int32> BillExpenseDetailId { get; set; }
        public string BudgetHeadName { get; set; }
        [Required]
        [Display(Name = "Account Group")]
        public Nullable<Int32> AccountGroupId { get; set; }

        public List<MasterlistviewModel> AccountGroupList { get; set; } = new List<MasterlistviewModel>();

        public List<MasterlistviewModel> AccountHeadList { get; set; } = new List<MasterlistviewModel>();

        [Required]
        [Display(Name = "Account Head")]
        public Nullable<Int32> AccountHeadId { get; set; }
        [Required]
        public string TransactionType { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }

        public bool IsJV { get; set; } = false;
        public string VariableName { get; set; }
        public string AccountGroupName { get; set; }
        public string AccountHeadName { get; set; }
    }
    public class BillDeductionDetailModel
    {
        public Nullable<Int32> BillDeductionDetailId { get; set; }
        [Display(Name = "Deduction Head")]
        public string DeductionHead { get; set; }

        public Nullable<int> DeductionHeadId { get; set; }

        [Display(Name = "Account Head")]
        public string AccountGroup { get; set; }

        public Nullable<int> AccountGroupId { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }
        public string DeductionType { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Expense Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> ExpenseAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> DeductionAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Expense Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CreditAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> DebitAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Creditor Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CreditorAmount { get; set; }
        public string TransactionTypeCode { get; set; }
        public Nullable<decimal> TDSPercentage { get; set; }
        public Nullable<int> DeductionCategoryId { get; set; }
        public string VariableName { get; set; }
    }
    public class DeductionPredicate
    {
        public tblAccountHead ah { get; set; }
        public tblDeductionHead dedut { get; set; }
        public tblAccountGroup ag { get; set; }
    }
    public class AttachmentDetailModel
    {
        public Nullable<Int32> DocumentDetailId { get; set; }
        [Required]
        [Display(Name = "Document Type")]
        public Nullable<Int32> DocumentType { get; set; }

        [Display(Name = "File Document")]
        public HttpPostedFileBase DocumentFile { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentName { get; set; }
        public string DocumentActualName { get; set; }
        public string Remarks { get; set; }
        public string DocumentTypeName { get; set; }

    }
    public class CheckListModel
    {
        public Nullable<Int32> FunctionCheckListId { get; set; }
        public string CheckList { get; set; }
        public bool IsChecked { get; set; }
    }
    public class InvoiceBreakUpDetailModel
    {
        [RequiredIfNot("HSNCode", null, ErrorMessage = "Invoice Number field is required")]
        public string InvoiceNumber { get; set; }

        [Display(Name = "Taxable Percentage")]
        public Nullable<decimal> TaxablePercentage { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TaxValue { get; set; }

        [RequiredIfNot("HSNCode", null, ErrorMessage = "Tax value field is required")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Tax value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }

        public string Description { get; set; }
        [RequiredIfNot("HSNCode", null, ErrorMessage = "Invoice Date field is required")]
        public Nullable<DateTime> InvoiceDate { get; set; }

        [RequiredIf("IsTaxEligible", true, ErrorMessage = "GSTIN field is required")]
        [MaxLength(15)]
        [RegularExpression("^([0]{1}[1-9]{1}|[1-2]{1}[0-9]{1}|[3]{1}[0-7]{1})([a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9a-zA-Z]{1}[zZ]{1}[0-9a-zA-Z]{1})+$", ErrorMessage = "Invalid GST Number")]
        [Display(Name = "GST Number")]
        public string GSTIN { get; set; }

        public bool IsTaxEligible { get; set; }

        public bool IsInterstate { get; set; }
        [RequiredIfNot("HSNCode", null, ErrorMessage = "SAC / HSN field is required")]
        public Nullable<Int32> TypeOfServiceOrCategory { get; set; }
        public string HSNCode { get; set; }
        [RequiredIfNot("HSNCode", null, ErrorMessage = "Vendor field is required")]
        public string Vendor { get; set; }
    }
    public class VendorInvoiceBreakUpDetailModel
    {
        [Required]
        public string InvoiceNumber { get; set; }

        [Display(Name = "Taxable Percentage")]
        public Nullable<decimal> TaxablePercentage { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TaxValue { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Tax value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }

        public bool IsTaxEligible { get; set; }

        [Required]
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string InvoiceDateView { get; set; }
    }
    public class BillHistoryModel
    {
        public int SlNo { get; set; }
        public string BillNumber { get; set; }
        public string BillDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string TransactionType { get; set; }
        public string PONumber { get; set; }
        public Nullable<Decimal> POAmount { get; set; }
        public Nullable<Decimal> BillAmount { get; set; }
    }
    #endregion
    #region BRS
    public class SearchBRSModel : CommonJSGridFilterModel
    {
        public string BRSNumber { get; set; }
        public string Bank { get; set; }
        public string VerifierName { get; set; }
        public string Status { get; set; }
        public Nullable<DateTime> postedDateFrom { get; set; }
        public Nullable<DateTime> postedDateTo { get; set; }
    }
    public class BankStatementDetailModel
    {
        public Nullable<DateTime> TransactionDate { get; set; }
        public string ReferenceNumber { get; set; }
        public string Description { get; set; }
        public Nullable<int> Reason { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public string Status { get; set; }
        public Nullable<int> BRSDetailId { get; set; }
        public string BOAPaymentDetailId { get; set; }
    }

    public class BRSModel
    {
        public int BRSId { get; set; }
        public string Remarks { get; set; }
        //[Required]
        [Display(Name = "Verified by")]
        public Nullable<int> Verified_By { get; set; }
        [Required]
        [Display(Name = "Bank")]
        public Nullable<int> BankHeadId { get; set; }
        public string VerifierName { get; set; }
        public string Bank { get; set; }
        public string BRSNumber { get; set; }
        public string Status { get; set; }
        public string PostedDate { get; set; }
        public Nullable<bool> EnableEditMode { get; set; }
        //[Required]
        [Display(Name = "From Date")]
        public Nullable<DateTime> BOAFromDate { get; set; }
        [Required]
        [Display(Name = "To Date")]
        public Nullable<DateTime> BOAToDate { get; set; }
        public int SlNo { get; set; }
        public string DocumentName { get; set; }
        public string DocumentActualName { get; set; }
        public string DocumentPath { get; set; }
        public List<BankStatementDetailModel> txDetail { get; set; }
        public List<BOAPaymentDetailModel> boaDetail { get; set; }
    }
    public class BRSReportModel
    {
        public Nullable<decimal> ReconcileAmount { get; set; }
        public Nullable<decimal> CashBookBalance { get; set; }
        public Nullable<decimal> BankClosingBalance { get; set; }
        public string BankLastDateofTx { get; set; }
        public string Bank { get; set; }
        public string BRSNumber { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public List<BRSUnReconcileModel> AddList { get; set; }
        public List<BRSUnReconcileModel> LessList { get; set; }

        public List<BRSUnReconcileModel> UnRecAddList { get; set; }
        public List<BRSUnReconcileModel> UnRecLessList { get; set; }
    }
    public class BRSUnReconcileModel
    {
        public string Reason { get; set; }
        public Nullable<decimal> TotalUnReconcileAmount { get; set; }
    }
    #endregion
    #region Credit Note
    public class CreditNoteModel
    {
        public int SlNo { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }
        public string TransactionTypeCode { get; set; }
        public string SubCode { get; set; } = "1";
        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }

        public string ReferenceNumber { get; set; }
        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }
        [Required]
        public Nullable<Int32> Source { get; set; }
        [Required]
        public Nullable<Int32> Reason { get; set; }
        public Nullable<int> CreditNoteId { get; set; }
        [Required(ErrorMessage = "Please select proper invoice number from the list")]
        public Nullable<int> InvoiceId { get; set; }
        public string CreditNoteNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public Nullable<Decimal> TotalInvoiceValue { get; set; }
        public Nullable<Decimal> AvailableBalance { get; set; }
        public Nullable<Decimal> TotalCreditAmount { get; set; }
        public string NameofPI { get; set; }
        public string SponsoringAgencyName { get; set; }
        public string ProjectNumber { get; set; }
        public string PIDepartmentName { get; set; }
        public string Remarks { get; set; }
        public string GSTNumber { get; set; }
        public string PAN { get; set; }
        public string TAN { get; set; }
        public string Agencyregaddress { get; set; }
        public string Agencydistrict { get; set; }
        public string Agencystate { get; set; }
        public Nullable<int> Agencystatecode { get; set; }
        public Nullable<int> AgencyPincode { get; set; }
        public string Agencycontactperson { get; set; }
        public string AgencycontactpersonEmail { get; set; }
        public string Agencycontactpersonmobile { get; set; }
        public string SanctionOrderNumber { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }

        public string DescriptionofServices { get; set; }
        [Required]
        [Display(Name = "Taxable Amount")]
        public Nullable<Decimal> TaxableAmount { get; set; }

        public Nullable<Decimal> CGST { get; set; }

        public Nullable<Decimal> CGSTPercentage { get; set; }

        public Nullable<Decimal> SGST { get; set; }

        public Nullable<Decimal> SGSTPercentage { get; set; }

        public Nullable<Decimal> IGST { get; set; }

        public Nullable<Decimal> IGSTPercentage { get; set; }
        public Nullable<Decimal> TotalTaxValue { get; set; }

        public Nullable<Decimal> TotalTaxpercentage { get; set; }

        public string Status { get; set; }
        public Nullable<int> Currency { get; set; }
        public Nullable<Decimal> ForeignCurrencyValue { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public Nullable<Decimal> InvForeignCurrencyValue { get; set; }
        public Nullable<Decimal> PrevCrNoteForeignCurrVal { get; set; }
        public Nullable<Decimal> CurrForeignCurrencyValue { get; set; }

        public List<ReceiptSearchResultModel> PreviousReceiptDetails { get; set; }
        public CreditNoteSearchFieldModel SearchField { get; set; }
        public string ResonName { get; set; }
        public string CurrencyName { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public bool PFInit { get; set; }
    }
    public class SearchCreditNoteModel
    {
        public string SearchINCreditNoteNumber { get; set; }
        public string SearchINInvoiceNumber { get; set; }
        public string SearchINStatus { get; set; }
        public Nullable<decimal> SearchINAmount { get; set; }
        public string SearchEXCreditNoteNumber { get; set; }
        public string SearchEXInvoiceNumber { get; set; }
        public string SearchEXStatus { get; set; }
        public Nullable<decimal> SearchEXAmount { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public int TotalRecords { get; set; }
        public List<CreditNoteModel> CNList { get; set; }
    }
    public class CreditNoteSearchFieldModel
    {
        public string PIName { get; set; }
        public string CreditNoteNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string Status { get; set; }
        public string SearchBy { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }


    }
    #endregion
    #region Transaction Definition
    public class TransactionAndTaxesModel
    {
        [Required(ErrorMessage = "Transaction is Required")]
        public string Transaction { get; set; }
        [Required(ErrorMessage = "SubCode is Required")]
        public string SubCode { get; set; }
        [Required(ErrorMessage = "Group is Required")]
        public string Group { get; set; }

        public int SubCodeId { get; set; }
        public int GroupId { get; set; }
        public string Head { get; set; }

        public int HeadId { get; set; }
        [Required(ErrorMessage = "Type is Required")]
        public string TransactionType { get; set; }

        public string TransactionTypeId { get; set; }
        public bool ISJV { get; set; }

        public string Category { get; set; }
        public bool InterState { get; set; }
        public string DeductionType { get; set; }
        public int TransactionDefID { get; set; }
        public int DeductionId { get; set; }

        public string INTERSTATE { get; set; }
        public int CategoryId { get; set; }
        public int Delete { get; set; }
    }
    #endregion
    #region AdhocPayment


    public class AdhocPaymentModel : CommonPaymentModel
    {
        public int AdhocId { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }
        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }
        [Required]
        public Nullable<Int32> Source { get; set; }
        public string AdhocPaymentNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Reimbursement Value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalAdhocPayValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalTaxValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> NetPayableValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> EligibleTaxValue { get; set; }
        public string Remark { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public Nullable<Int32> BankId { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }

        public string BankHeadName { get; set; }

        public string Status { get; set; }
        public int SlNo { get; set; }
        [Display(Name = "Name")]
        public string UserId { get; set; }
        public Nullable<Int32> CategoryId { get; set; }
        public string RollNo { get; set; }
        public string Name { get; set; }
        public string autoComplete { get; set; }
        public Nullable<int> PaymentMode { get; set; }
        [RequiredIf("PaymentMode", 2, ErrorMessage = "Bank Name field is required")]
        public string BankName { get; set; }
        [RequiredIf("PaymentMode", 2, ErrorMessage = "Account Number field is required")]
        public string AccountNumber { get; set; }
        [RequiredIf("PaymentMode", 2, ErrorMessage = "Branch field is required")]
        public string BranchName { get; set; }
        [RequiredIf("PaymentMode", 2, ErrorMessage = "IFSC field is required")]
        public string IFSCCode { get; set; }
        public string Description { get; set; }
        public string AdhocPaymentDate { get; set; }
        
        public Nullable<int> PaymentType { get; set; }
        public List<AdhocPaymentListModel> PaymentDetails { get; set; }
        public AdhocPaySearchFieldModel SearchField { get; set; }
        public PagedData<AdhocPaySearchResultModel> SearchResult { get; set; }
        public bool BillProcessingStatus { get; set; }
        public string PaymentTypeName { get; set; }
        public string PayeeTypeName { get; set; }
        public string PaymentModeName { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public bool PFInit { get; set; }
    }
    public class AdhocPaySearchResultModel
    {
        public Nullable<int> SlNo { get; set; }
        public Nullable<int> AdhocId { get; set; }
        public string Name { get; set; }
        public string AdhocPaymentNumber { get; set; }
        public string AdhocPaymentDate { get; set; }
        public string Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> NetPayableValue { get; set; }

    }
    public class ReimbursementSearchModel
    {
        public string InNameOfPI { get; set; }
        public string InPaymentNumber { get; set; }
        public Nullable<decimal> InNetPaybleAmount { get; set; }
        public string InStatus { get; set; }
        public string ExPaymentNumber { get; set; }
        public string ExNameOfPI { get; set; }
        public Nullable<DateTime> ExFromDate { get; set; }
        public Nullable<DateTime> ExToDate { get; set; }
        public List<AdhocPaymentModel> Adhoclist { get; set; }
        public int TotalRecords { get; set; }
    }
    public class AdhocPaySearchFieldModel
    {
        public string PayeeName { get; set; }
        public string AdhocPaymentNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
    }
    public class AdhocPaymentListModel
    {
        public Nullable<int> AdhocDetailId { get; set; }
        public string InvoiceNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Taxable Percentage")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TaxablePercentage { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Tax value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TaxValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Tax value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string InvDate { get; set; }
        [MaxLength(15)]
        [RegularExpression("^([0]{1}[1-9]{1}|[1-2]{1}[0-9]{1}|[3]{1}[0-7]{1})([a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9a-zA-Z]{1}[zZ]{1}[0-9a-zA-Z]{1})+$", ErrorMessage = "Invalid GST Number")]
        [Display(Name = "GST Number")]
        public string GSTIN { get; set; }
        public Nullable<bool> IsTaxEligible { get; set; }
        public string HSNCode { get; set; }
        public string Vendor { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Tax value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalValue { get; set; }
        public Nullable<bool> IsInterstate { get; set; }
        public Nullable<Int32> TypeOfServiceOrCategory { get; set; }
    }
    #endregion    
    #region Distribution
    public class SearchDistributionModel
    {
        public string SearchINProjectNumber { get; set; }
        public string SearchINReferencenumber { get; set; }
        public string SearchINPIName { get; set; }
        public Nullable<Decimal> SearchINAmount { get; set; }
        public string SearchEXProjectNumber { get; set; }
        public string SearchEXReferencenumber { get; set; }
        public string SearchPIName { get; set; }
        public string SearchEXAmount { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string SearchINStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<DistributionModel> DistributionList { get; set; }
    }

    public class DistributionModel : CommonPaymentModel
    {
        public int DistributionId { get; set; }
        public int SlNo { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }
        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }
        [Required]
        public Nullable<Int32> Source { get; set; }
        public string DistributionNumber { get; set; }
        public string PIName { get; set; }
        public Nullable<Int32> PIId { get; set; }
        public Nullable<Int32> ProjectType { get; set; }
        public Nullable<Int32> DistributionType { get; set; }
        public Nullable<Int32> ProjectId { get; set; }
        public string ProjectNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount Received")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PjctTotalAmountReceived { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount Received")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PjctTotalAmountReceivedOH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount Received")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PjctTotalAmountReceivedGST { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Total Amount Received")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PjctTotalAmtReceivedincldngOHandGST { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PjctTotalAmountSpent { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PjctOpenCommitmentAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PjctAvailableBalanceAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> InstituteOverheadAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> InstituteOverheadPercentage { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> DistributionAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> AmountAvailableforDistribution { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> EUCOAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> StoresConsumedAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> OthersAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> LabCodeAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CentralFacilitiesAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalExpenditureAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TransferedToIITAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> SubTotalAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalIndividualShareAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalPCFAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalPCFShareAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> FacultyDistributedTotalAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> FacultyDistributedTotalTDSAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> FacultyDistributedTotalNetAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PCFTransferedAmount { get; set; }
        public string LabCode { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public Nullable<DateTime> DistributeDate { get; set; }
        public string DistributionDate { get; set; }
        public Nullable<int> PaymentMode { get; set; }
        public Nullable<int> PaymentType { get; set; }
        public List<DistributionListModel> DistributionDetails { get; set; }
        public List<PCFDetailsModel> PCFDetails { get; set; }
        public DistributionSearchFieldModel SearchField { get; set; }
        public PagedData<DistributionSearchResultModel> SearchResult { get; set; }
        public List<DistributionOverheadListModel> DistributionOverheads { get; set; }
        public bool BillProcessingStatus { get; set; }
        public string DistributionTypes { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public bool PFInit { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public Nullable<Int32> BankId { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }

        public string BankHeadName { get; set; }

    }










    public class DistributionListModel
    {
        public Nullable<int> DistributionDetailId { get; set; }
        public string PayBillNumber { get; set; }
        public string ProjectNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "TDS value")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TDSValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Net Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> NetAmount { get; set; }
        public Nullable<int> PaymentMode { get; set; }
        public Nullable<bool> IsPaymentCompleted { get; set; }
        public int SlNo { get; set; }
        public string Category { get; set; }
        [Display(Name = "Name")]
        public Nullable<int> UserId { get; set; }
        public string Name { get; set; }
        public string autoComplete { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchName { get; set; }
        public string IFSCCode { get; set; }
        public string FacultyDepartment { get; set; }
        public string FacultyDesignation { get; set; }
        public string CategoryName { get; set; }
        public string PaymentModeName { get; set; }
    }
    public class PCFDetailsModel
    {
        public Nullable<int> PCFDetailId { get; set; }
        public string FacultyCategory { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PCFAmount { get; set; }
        public int SlNo { get; set; }
        [Display(Name = "Name")]
        public string PCFPIUserId { get; set; }
        public string PCFName { get; set; }
        public string PCFautoComplete { get; set; }
        public string PCFAccountNumber { get; set; }
        public string EmployeePCFId { get; set; }
        public string DepartmentofStaff { get; set; }
        public string DesignationofStaff { get; set; }
    }
    public class DistributionSearchResultModel
    {
        public Nullable<int> SlNo { get; set; }
        public Nullable<int> DistributionId { get; set; }
        public string PIName { get; set; }
        public string DistributionNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string DistributionDate { get; set; }
        public string Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> DistributionAmount { get; set; }

    }
    public class DistributionSearchFieldModel
    {
        public string DistributionNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string PIName { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
    }
    public class DistributionOverheadListModel
    {
        public Nullable<int> OverheadtypeId { get; set; }
        public string Overheadtypename { get; set; }
        public Nullable<Decimal> OverheadPercentage { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> OverheadAmount { get; set; }
        public Nullable<int> InstituteOverheadbreakupId { get; set; }

    }
    
    #endregion

    #region Negative Balance

    public class SearchNegativeBalanceModel
    {
        public string SearchINProjectNumber { get; set; }
        public string SearchINReferencenumber { get; set; }
        public string SearchINPIName { get; set; }
        public Nullable<Decimal> SearchINAmount { get; set; }
        public string SearchEXProjectNumber { get; set; }
        public string SearchEXReferencenumber { get; set; }
        public string SearchPIName { get; set; }
        public string SearchEXAmount { get; set; }
        public string SearchEXStatus { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string SearchINStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<NegativeBalanceModel> NBList { get; set; }
    }
    public class NegativeBalanceSearchFieldModel
    {
        public string NegativeBalanceNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string PIName { get; set; }
        public string Status { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
    }
    public class NegativeBalanceModel
    {
        public int NegativeBalanceId { get; set; }
        public Nullable<Int32> ProjectId { get; set; }
        public Nullable<Int32> PIId { get; set; }
        public string NegativeBalanceNumber { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }

        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }
        public Nullable<DateTime> RequestedDate { get; set; }
        public string RequestDate { get; set; }
        [Required]
        public Nullable<Int32> Source { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string PostedDate { get; set; }
        public int SlNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Claim Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> ClaimAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Adjustment Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> ReceiptAdjustmentAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> NegativeBalanceAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> BalanceAmountWhenClose { get; set; }
        public HttpPostedFileBase Document { get; set; }
        public string DocumentName { get; set; }
        public string NegativeBalanceDocPath { get; set; }
        public string DocumentPath { get; set; }
        public string NegativeBalanceCloseDocPath { get; set; }
        public string DocumentActualName { get; set; }
        public string ProjectNumber { get; set; }
        public string PIName { get; set; }
        public Nullable<Int32> SelProjectNumber { get; set; }
        public Nullable<Int32> selProjectType { get; set; }
        public ProjectSummaryModel prjDetails { get; set; }
        public HttpPostedFileBase CloseDocument { get; set; }
        public string ReasonforClose { get; set; }
        public string RemarksforClose { get; set; }
        public string CloseDocumentName { get; set; }
        public Nullable<DateTime> CloseDate { get; set; }
        public string ClsDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Sanctioned Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> SanctionedAmount { get; set; }
        public Nullable<decimal> TotalReceiptValue { get; set; }
        public NegativeBalanceSearchFieldModel SearchField { get; set; }
        public List<PrevNegativeBalanceModel> PrevNeg { get; set; }
        public string RequDate { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public string AgencyCode { get; set; }
        public string PIDepartmentName { get; set; }
        public decimal GranttobeReceived { get; set; }
        public bool PFInit { get; set; }
        public Nullable<decimal> TotalPrevNegBalAmount { get; set; }
        public Nullable<decimal> CurrentClaimAmount { get; set; }
        public Nullable<decimal> TotalClaimAmount { get; set; }
    }
    public class PrevNegativeBalanceModel
    {
        public string ProjectNo { get; set; }
        public string StartDate { get; set; }
        public string CloseDate { get; set; }
        public decimal ProjCost { get; set; }
        public decimal Receipt { get; set; }
        public decimal Balance { get; set; }
        // public string status { get; set; }
        public decimal NegBalance { get; set; }
        public decimal Exp { get; set; }
        public decimal Commit { get; set; }
        public decimal Net { get; set; }
        //18/10/2022 NegativeBalance #7395
        //public decimal PrevNegBal { get; set; }
        // public decimal CurrNegBal { get; set; }

        //Negative Balance Changes 
        public string ProjectNumber { get; set; }
        public string Status { get; set; }
        public decimal pennegbal { get; set; }
        public decimal ClaimAmount { get; set; }
        public decimal TotalNegativeBalance { get; set; }
        public decimal ProjectBalance { get; set; }
        public decimal AvailableBalance { get; set; }
    }
    public class CloseNegativeBalanceModel
    {
        public int NegativeBalanceId { get; set; }
        public Nullable<Int32> ProjectId { get; set; }
        public Nullable<Int32> PIId { get; set; }
        public string NegativeBalanceNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Claim Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> ClaimAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Adjustment Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> ReceiptAdjustmentAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> NegativeBalanceAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> BalanceAmountWhenClose { get; set; }
        public string NegativeBalanceCloseDocPath { get; set; }
        public string DocumentActualName { get; set; }
        public string ProjectNumber { get; set; }
        public string PIName { get; set; }
        public HttpPostedFileBase Document { get; set; }
        public string ReasonforClose { get; set; }
        public string RemarksforClose { get; set; }
        public string CloseDocumentName { get; set; }
        public Nullable<DateTime> CloseDate { get; set; }
        public string ClsDate { get; set; }

    }
    #endregion
    #region General Voucher
    public class GeneralVoucherModel
    {
        public string VoucherNumber { get; set; }
        public Nullable<int> VoucherId { get; set; }
        [Required]
        [Display(Name = "Category")]
        public Nullable<int> PaymentCategory { get; set; }
        public string PaymentRemarks { get; set; }
        [Required]
        [Display(Name = "Bank")]
        public Nullable<int> PaymentBank { get; set; }
        [Required]
        [Display(Name = "Amount")]
        public Nullable<Decimal> PaymentBankAmount { get; set; }
        public string PostedDate { get; set; }
        public string Status { get; set; }
        public Nullable<int> SlNo { get; set; }
        public List<PaymentBreakUpDetailModel> PaymentBreakDetail { get; set; } = new List<PaymentBreakUpDetailModel>();

        public List<BillExpenseDetailModel> PaymentExpenseDetail { get; set; }
        public List<BillDeductionDetailModel> PaymentDeductionDetail { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PaymentDebitAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Credit Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PaymentCreditAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Commitment Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> PaymentTDSAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Commitment Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CommitmentAmount { get; set; }
        public List<BillCommitmentDetailModel> CommitmentDetail { get; set; } = new List<BillCommitmentDetailModel>();
        public List<AttachmentDetailModel> DocumentDetail { get; set; } = new List<AttachmentDetailModel>();

        public bool BillProcessingStatus { get; set; }
        public string PaymentCategoryName { get; set; }
        public string BankName { get; set; }
        public bool PFInit { get; set; }

    }
    public class GeneralVoucherSearch
    {
        public string VoucherNumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> PaymentBankAmount { get; set; }
        public string Status { get; set; }
        public int TotalRecords { get; set; }
        public List<GeneralVoucherModel> vochlist { get; set; }
    }
    #endregion
    #region Other Receipt

    public class OtherReceiptModel
    {
        [RequiredIf("IsProject", true, ErrorMessage = "Sanction order date field is required")]
        public Nullable<DateTime> SanctionOrderDate { get; set; }
        public string SanctionOrderNo { get; set; }
        public string ProjectSanctionOrderNo { get; set; }
        public bool IsBudgetHeadPosting { get; set; }
        public string ReceiptNumber { get; set; }
        public Nullable<int> ReceiptId { get; set; }
        [Required]
        [Display(Name = "Category")]
        public Nullable<int> Category { get; set; }
        public string Remarks { get; set; }

        [RequiredIfNot("Category", 18, ErrorMessage = "Bank field is required")]
        [Display(Name = "Bank")]
        public Nullable<int> Bank { get; set; }
        [RequiredIfNot("Category", 18, ErrorMessage = "Amount field is required")]
        [Display(Name = "Amount")]
        public Nullable<Decimal> BankAmount { get; set; }
        public string PostedDate { get; set; }
        public string Status { get; set; }
        public Nullable<int> SlNo { get; set; }
        public string Project { get; set; }
        [NotMapped]
        public bool ProjectValidation
        {
            get
            {
                return (this.Category != 1 && this.Category != 14);
            }
        }
        [Display(Name = "Project")]

        //[RequiredIfNot("Category", "1", ErrorMessage = "Project field is required")]
        [RequiredIf("ProjectValidation", true, ErrorMessage = "Project field is required")]
        public Nullable<int> ProjectId { get; set; }
        public bool IsProject { get; set; }
        public Nullable<int> ModeOfReceipt { get; set; }
        public string RefNo { get; set; }
        public Nullable<DateTime> RefDate { get; set; }
        [RequiredIf("Category", 18, ErrorMessage = "Negative No. field is required")]
        public Nullable<int> NegativeReceiptNo { get; set; }
        public string DocumentName { get; set; }
        public string DocumentActualName { get; set; }
        public string DocumentPath { get; set; }
        public List<BillExpenseDetailModel> ExpenseDetail { get; set; }
        public List<BillDeductionDetailModel> DeductionDetail { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> DebitAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Credit Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CreditAmount { get; set; }

        public HttpPostedFileBase file { get; set; }

        public Nullable<int> ClassificationOfReceipt { get; set; }
        public string TransactionTypeCode { get; set; }
        public Nullable<bool> PaymentProcess_f { get; set; }
        public string CategoryName { get; set; }
        public string ModeofPayment { get; set; }
        public string BankName { get; set; }
        public string ReceiptDate { get; set; }
        public bool PFInit { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public string NegativeReceiptNoStr { get; set; }
        //public Nullable<int> InvoiceTypeId { get; set; }
        //[RequiredIf("InvoiceTypeId", 1, ErrorMessage = "Foreign Currency field is required")]
        public Nullable<decimal> RevarsalForeignCurrency { get; set; }
        //[DisplayFormat(DataFormatString = "{0:n2}")]
        //[Display(Name = "Commitment Amount")]
        //[Range(0, 9999999999999999.99)]
        //public Nullable<decimal> CommitmentAmount { get; set; }
        //public List<BillCommitmentDetailModel> CommitmentDetail { get; set; } = new List<BillCommitmentDetailModel>();
        public Nullable<bool> Posted_f { get; set; }
        public Nullable<decimal> IGST { get; set; }
        public Nullable<decimal> SGST { get; set; }
        public Nullable<decimal> CGST { get; set; }
    }


    #endregion

    #region Head Credit
    public class HeadCreditModel
    {
        public string HeadCreditNumber { get; set; }
        public Nullable<int> HeadCreditId { get; set; }
        [Required]
        [Display(Name = "Category")]
        public Nullable<int> Category { get; set; }
        public string Remarks { get; set; }
        [Required]
        [Display(Name = "Bank")]
        public Nullable<int> Bank { get; set; }
        [Required]
        [Display(Name = "Amount")]
        public Nullable<Decimal> BankAmount { get; set; }
        public string PostedDate { get; set; }
        public string Status { get; set; }
        public Nullable<int> SlNo { get; set; }
        public string Project { get; set; }
        [Display(Name = "Project")]
        [Required]
        public Nullable<int> ProjectId { get; set; }
        public bool IsProject { get; set; }
        [Required]
        public Nullable<int> ModeOfCredit { get; set; }
        public string RefNo { get; set; }
        public Nullable<DateTime> RefDate { get; set; }
        public string BillRefNoAC { get; set; }
        public string BillRefNo { get; set; }
        public string DocumentName { get; set; }
        public string DocumentActualName { get; set; }
        public string DocumentPath { get; set; }
        public List<BillReversalExpenseDetailModel> ExpenseDetail { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Deduction Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> DebitAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Credit Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CreditAmount { get; set; }

        public HttpPostedFileBase file { get; set; }

        public string TransactionTypeCode { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Commitment Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CommitmentAmount { get; set; }
        public List<BillCommitmentDetailModel> CommitmentDetail { get; set; } = new List<BillCommitmentDetailModel>();
        public List<ProjectTransferDetailModel> CrDetail { get; set; }

        public List<ReversedInvoiceModel> InvDetail { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }
        public string BankName { get; set; }
    }

    public class ReversedInvoiceModel
    {
        public string InvoiceNo { get; set; }
        public Nullable<decimal> ReversedInvoiceAmt { get; set; }
        public Nullable<decimal> ReversedGSTAmt { get; set; }
    }

    public class HeadCreditSearch
    {
        public string HeadCreditNumber { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Status { get; set; }
        public int TotalRecords { get; set; }
        public List<HeadCreditModel> list { get; set; }
    }
    #endregion

    #region ManDay
    public class SearchMandays
    {
        public string InMandaysNumber { get; set; }
        public string InStatus { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string ExMandaysNumber { get; set; }
        public string ExStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<ManDayModel> Mandayslist { get; set; }
    }
    public class ManDayModel : CommonPaymentModel
    {
        public int SlNo { get; set; }
        public Nullable<DateTime> ManDayDate { get; set; }
        public string Date { get; set; }
        public string Projectno { get; set; }
        public string Staff { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public int ManDayId { get; set; }
        public string ManDayNumber { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }
        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }
        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }
        [Required]
        public Nullable<Int32> Source { get; set; }
        [Required(ErrorMessage = "Month & Year field is required")]
        public Nullable<DateTime> MonthYear { get; set; }
        public string MonYear { get; set; }
        [Required(ErrorMessage = "Request Date field is required")]
        public Nullable<DateTime> ReqDate { get; set; }
        public string ReDate { get; set; }
        public List<ManDayListmodel> MDY { get; set; }
        public ManDaySearchFieldModel SearchField { get; set; }
        public string SourceName { get; set; }
        public string SourceEmail { get; set; }
        public string ReqstDate { get; set; }
        public string monthsyear { get; set; }
        public bool PFInit { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public Nullable<Int32> BankId { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }
        public string BankHeadName { get; set; }

    }
    public class ManDayListmodel
    {
        public int errmsgid { get; set; }
        public string errmsg { get; set; }
        public int ProjectId { get; set; }
        public string ProjectNumber { get; set; }
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public string Department { get; set; }
        public string DeptCode { get; set; }
        public Nullable<int> NoOfDays { get; set; }
        public decimal AmountPerDay { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMode { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string IFSC { get; set; }
        public string AccountNo { get; set; }
        public string PaymentModeName { get; set; }
    }
    public class ManDaySearchFieldModel
    {
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string ManDayNumber { get; set; }
        public string Status { get; set; }
        public Nullable<DateTime> ReqDate { get; set; }
        public Nullable<DateTime> MonthYear { get; set; }
    }
    #endregion


    #region Import Payment
    #region Foreign Remittance
    public class ForeignRemittanceModel : CommonPaymentModel
    {
        public int ForeignRemittanceId { get; set; }
        public int SlNo { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        public string ReferenceNumber { get; set; }
        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public Nullable<DateTime> SourceEmailDate { get; set; }

        public Nullable<int> ProjectId { get; set; }
        [Required]
        public Nullable<Int32> Source { get; set; }
        public string ForeignRemitNumber { get; set; }
        public Nullable<Int32> TypeofPayment { get; set; }
        public Nullable<Int32> PaymentBank { get; set; }
        public Nullable<Int32> PortfolioName { get; set; }
        public Nullable<Int32> Beneficiary { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryAddress { get; set; }
        public Nullable<Int32> BeneficiaryCountry { get; set; }
        public Nullable<Int32> BeneficiaryBankCountry { get; set; }
        public string BeneficiaryCountryName { get; set; }
        public string BeneficiaryBankCountryName { get; set; }
        public string Portfolio { get; set; }
        public string RemitDate { get; set; }
        public string Dated { get; set; }
        public Nullable<DateTime> RemitanceDate { get; set; }
        public string PONumber { get; set; }
        public string AccountNumber { get; set; }
        public string BeneficiaryBank { get; set; }
        public string BeneficiaryBankAcNo { get; set; }
        public string BeneficiaryBankAddress { get; set; }
        public string BeneficiaryBankSwiftCode { get; set; }
        public Nullable<Int32> AccountHeadId { get; set; }
        public string AccountNumberDebitedforCharges { get; set; }
        public Nullable<Int32> ChargesAccountHeadId { get; set; }
        public string ForgnCurrncyEqualtoINR { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Foreign Remitance Amount")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> ForeignRemittanceAmount { get; set; }
        public string ForeignRemittanceAmt { get; set; }
        public Nullable<int> RemittanceCurrency { get; set; }
        public string RemittanceCurrencyCode { get; set; }
        public string RemittanceAmountinWords { get; set; }
        public string InvoiceNumber { get; set; }
        public string IntermediaryBank { get; set; }
        public string InvDate { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string IntermediaryBankAddress { get; set; }
        public string IntermediaryBankCodeNumber { get; set; }
        public Nullable<int> ForeignBankChargesType { get; set; }
        public string ForeignBankChargesTypeName { get; set; }
        public string ShipmentDetails { get; set; }
        public string GoodsDescription { get; set; }
        public Nullable<int> PurposeofRemittance { get; set; }
        public string PurposofRemit { get; set; }
        public string HSClassificationCode { get; set; }
        public string ImportLicenseDetails { get; set; }
        public string SpecialReferenceNumber { get; set; }
        public string RateorContractBookedDetails { get; set; }
        public Nullable<int> ExpensesHead { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CommentstoBank { get; set; }
        public Nullable<int> PaymentMode { get; set; }
        public string PayDate { get; set; }
        public Nullable<DateTime> PaymentDate { get; set; }
        public string FottReferenceNumber { get; set; }
        public string UserReferenceNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> FXRate { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> BillAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> BillCommission { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> BillCGST { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> BillSGST { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalAmountDebitedtoProject { get; set; }
        public string BillofEntryNo { get; set; }
        public string BillofEntryDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> CustomsDuty { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> DemurrageCharges { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> AirportAuthorityDuty { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> ClearingCharges { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> OthersifAny { get; set; }
        public string ExpectedDateofDespatch { get; set; }
        public string PortofDespatch { get; set; }
        public string DestinationPort { get; set; }
        public string ShippingCompany { get; set; }
        public Nullable<int> CountryofOriginofGoods { get; set; }
        public string CountryofOriginName { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<int> CommitmentNumber { get; set; }
        public string CommitNumber { get; set; }
        public List<MasterlistviewModel> CommitmentList { get; set; } = new List<MasterlistviewModel>();
        public ForeignRemitSearchFieldModel SearchField { get; set; }
        //public PagedData<ForeignRemitSearchResultModel> SearchResult { get; set; }
        public bool IsEditDraft { get; set; }
        public string BeneficiaryCity { get; set; }
        public string BeneficiaryPinCode { get; set; }
        public string BeneficiaryBankBranch { get; set; }
        public bool IsEditBillofEntry { get; set; }
        public string CommitmentNo { get; set; }
        public string ProjectNo { get; set; }
        public string PaymentType { get; set; }
        public string DocDetail { get; set; }
        public string NewGenerateDoc { get; set; }
        public string Finalhref { get; set; }
        public string Newhref { get; set; }
        public HttpPostedFileBase FinalAttachDoc { get; set; }
    }

    public class BankAccountModel
    {
        public int SlNo { get; set; }
        public int BankAccountId { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
    }
    public class ForeignRemitSearchFieldModel
    {
        public string ForeignRemitNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string BeneficiaryName { get; set; }
        public string PIName { get; set; }
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string SearchBy { get; set; }
        public Nullable<DateTime> FromFRDate { get; set; }
        public Nullable<DateTime> ToFRDate { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
    }
    public class SearchForeignRemitModel
    {
        public string SearchINProjectNumber { get; set; }
        public string SearchINReferencenumber { get; set; }
        public string SearchINInvoicenumber { get; set; }
        public string SearchINBeneficiaryName { get; set; }
        public string SearchINPONumber { get; set; }
        public Nullable<decimal> SearchINAmount { get; set; }
        public string SearchEXProjectNumber { get; set; }
        public string SearchEXReferencenumber { get; set; }
        public string SearchEXBeneficiaryName { get; set; }
        public string SearchEXPONumber { get; set; }
        public string SearchEXInvoicenumber { get; set; }
        public string SearchEXAmount { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public Nullable<DateTime> FromFRDate { get; set; }
        public Nullable<DateTime> ToFRDate { get; set; }
        public string SearchINStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<ForeignRemittanceModel> ForeignRemitList { get; set; }
        public string CommitmentNO { get; set; }
        public string ProjectNumber { get; set; }
        public string TypeofPayment { get; set; }
    }
    #endregion



    #endregion
    #region LC Opening
    public class LCOpeningModel : CommonPaymentModel
    {
        public string CurrencyStr { get; set; }
        public int LCOpeningId { get; set; }
        public int LCAmmendmentId { get; set; }
        public int LCRetirementId { get; set; }
        public int SlNo { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.Source == 1 || this.Source == 3);
            }
        }

        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public Nullable<Int32> SourceReferenceNumber { get; set; }
        [RequiredIf("Source", 2, ErrorMessage = "Email Date field is required")]
        public string SourceEmailDate { get; set; }
        public Nullable<Int32> Source { get; set; }
        public string ReferenceNumber { get; set; }
        public string LCOpeningNumber { get; set; }
        public string DraftLCNumber { get; set; }
        public string ABLCNumber { get; set; }
        public Nullable<int> LCAmmendSeqNumber { get; set; }
        public string SelectLCNumber { get; set; }
        public Nullable<int> SelLCId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> CommitmentId { get; set; }
        public string ProjectNumber { get; set; }
        public string CommitmentNumber { get; set; }
        public string PONumber { get; set; }
        public Nullable<Int32> IssuingBank { get; set; }
        public string BankAccountNumber { get; set; }
        public string ProjectCategory { get; set; }
        public string PaymentTerms { get; set; }
        public Nullable<Int32> Type40A { get; set; }
        public string LCType { get; set; }
        public Nullable<Int32> PortfolioName { get; set; }
        public Nullable<Int32> Currency { get; set; }
        public string LCDraftCurrency { get; set; }
        public Nullable<decimal> LCDraftAmount { get; set; }
        public string LCDraftAmt { get; set; }
        public string LCRetireAmt { get; set; }
        public string RequestDate { get; set; }
        public string Dated { get; set; }
        public string LCExpiryDate { get; set; }
        public Nullable<DateTime> RequestedDate { get; set; }
        public string LCCrtdDate { get; set; }
        public string PlaceofExpiry { get; set; }
        public string ExpiryDate { get; set; }
        public Nullable<DateTime> ExpryDate { get; set; }
        public string CreditAvailableBank { get; set; }
        public Nullable<Int32> CreditAvailableBy { get; set; }
        public string CreditAvailableBy41A { get; set; }
        public string CreditAvailableFor { get; set; }
        public string Tenor42C { get; set; }
        public string AdvisingBank { get; set; }
        public string AdvisingBankBranch { get; set; }
        public string AdvisingBankAddress { get; set; }
        public string AdvisingBankCity { get; set; }
        public string AdvisingBankState { get; set; }
        public Nullable<Int32> LCCountry { get; set; }
        public string PostalCode { get; set; }
        public string SwiftorIFSCCode { get; set; }
        public string Drawee42A { get; set; }
        public string Beneficiary { get; set; }
        public string BeneficiaryAddress { get; set; }
        public string BeneficiaryCity { get; set; }
        public string BeneficiaryState { get; set; }
        public Nullable<Int32> BeneficiaryCountry { get; set; }
        public string BeneficiaryCountryName { get; set; }
        public string BeneficiaryPostalCode { get; set; }
        public Nullable<Int32> PartialShipmentOption { get; set; }
        public Nullable<Int32> TransShipmentOption { get; set; }
        public string PortorAirportofLoading { get; set; }
        public string PartShipOption { get; set; }
        public string TransShipOption { get; set; }
        public string PortorAirportofDestination { get; set; }
        public string LatestDateofShipment { get; set; }
        public Nullable<DateTime> LatestDteofShipment { get; set; }
        public string DescriptionofGoodsandService { get; set; }
        public string ProformaInvoiceNumber { get; set; }
        public string ProformaInvoiceDate { get; set; }
        public Nullable<DateTime> ProformaInvDate { get; set; }
        public Nullable<Int32> CountryofOrigin { get; set; }
        public string InsuranceCoverageDetails { get; set; }
        public string InsurancePolicyNumber { get; set; }
        public string InsurancePolicyDate { get; set; }
        public Nullable<DateTime> InsurncePolcyDate { get; set; }
        public Nullable<Int32> TradeINCOTerms { get; set; }
        public string INCOTerms { get; set; }
        public Nullable<decimal> ChargesAmount { get; set; }
        public string Charges71B { get; set; }
        public string PresentationPeriod { get; set; }
        public Nullable<Int32> ModeofDispatch { get; set; }
        public string DispatchMode { get; set; }
        public string AmmendBankRefNumber { get; set; }
        public Nullable<decimal> BankCharges { get; set; }
        public string DocContent { get; set; }
        public string AddtnlConditionsContent { get; set; }
        public Nullable<Int32> AccountHeadId { get; set; }
        public string AccountNumberDebitedforCharges { get; set; }
        public Nullable<Int32> ChargesAccountHeadId { get; set; }
        public string LCDraftAmountinWords { get; set; }
        public string LCRetirementAmountinWords { get; set; }
        public string InvoiceNumber { get; set; }
        public string IntermediaryBank { get; set; }
        public string InvDate { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string IntermediaryBankAddress { get; set; }
        public string IntermediaryBankCodeNumber { get; set; }
        public Nullable<int> ForeignBankChargesType { get; set; }
        public string ShipmentDetails { get; set; }
        public string GoodsDescription { get; set; }
        public Nullable<int> PurposeofRemittance { get; set; }
        public string HSClassificationCode { get; set; }
        public string ImportLicenseDetails { get; set; }
        public string RateorContractBookedDetails { get; set; }
        public Nullable<int> ExpensesHead { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string AmmendmentDate { get; set; }
        public Nullable<DateTime> AmmendDate { get; set; }
        public string lastAmendDate { get; set; }
        public Nullable<decimal> LCRetirementAmtinForgnCurrency { get; set; }
        public Nullable<decimal> LCPrevRetirementAmtinForgnCurrency { get; set; }
        public Nullable<decimal> LCCurrRetirementAmtinForgnCurrency { get; set; }
        public string LCRetireDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> FXRate { get; set; }
        public string FXRateAsOnDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> LCRetireEquivalentINRValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> LCRetireTotalDeduction { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> LCRetireNetTotalAmt { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> TotalPmtTermsAmt { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 9999999999999999.99)]
        public Nullable<decimal> OthersifAny { get; set; }
        public string ExpectedDateofDespatch { get; set; }
        public string PortofDespatch { get; set; }
        public string DestinationPort { get; set; }
        public string ShippingCompany { get; set; }
        public Nullable<int> CountryofOriginofGoods { get; set; }
        public string DocumentsRequired { get; set; }
        public bool IsEstablish { get; set; }
        public bool IsEditEstablish { get; set; }
        public bool IsEditMode { get; set; }
        public string DocumentActualName { get; set; }
        public string DocumentPath { get; set; }
        public HttpPostedFileBase Document { get; set; }
        public string DocumentName { get; set; }
        public string LCEstablishBankRefNo { get; set; }
        public string LCEstablishRemark { get; set; }
        public string EstablishedLCNo { get; set; }
        public Nullable<int> LCRetireSeqNo { get; set; }
        public string[] AmendDte { get; set; }
        public Nullable<int>[] AmendId { get; set; }
        public Nullable<int>[] AmendSeqNumber { get; set; }
        public Nullable<Decimal>[] ArrayEMIValue { get; set; }
        public List<LCConditonsModel> Conditions { get; set; }
        public List<LCRetireDeductionModel> RetireDeducts { get; set; }
        public List<LCConditonsModel> DraftLCConditions { get; set; }
        public List<LCConditonsModel> DocInfo { get; set; }
        public List<MasterlistviewModel> CommitmentList { get; set; } = new List<MasterlistviewModel>();
        public LCSearchFieldModel SearchField { get; set; }
        public PagedData<LCSearchResultModel> SearchResult { get; set; }
        public string Payment { get; set; }

    }

    public class LCSearchResultModel
    {
        public Nullable<int> SlNo { get; set; }
        public Nullable<int> LCDraftId { get; set; }
        public string ProjectNumber { get; set; }
        public string LCReferenceNumber { get; set; }
        public string PONumber { get; set; }
        public string LCDraftNumber { get; set; }
        public string BeneficiaryName { get; set; }
        public string RequestedDate { get; set; }
        public string Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> Amount { get; set; }
        public string LCAmt { get; set; }

    }
    public class LCSearchFieldModel
    {
        public string LCReferenceNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string BeneficiaryName { get; set; }
        public string PIName { get; set; }
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string SearchBy { get; set; }
        public Nullable<DateTime> FromFRDate { get; set; }
        public Nullable<DateTime> ToFRDate { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
    }
    public class LCConditonsModel
    {
        public Nullable<int> LCOpeningId { get; set; }
        public Nullable<int> LCAmmendmentId { get; set; }
        public string LCOpeningClause { get; set; }
        public string LCDraftClause { get; set; }
        public int SlNo { get; set; }
        public Nullable<int> ConditionsId { get; set; }
        public Nullable<int> DraftConditionsId { get; set; }
        public string LCDraftExistConditions { get; set; }
        public string LCDraftNewConditions { get; set; }
        public string LCOpeningConditions { get; set; }

    }
    public class LCDocInfoModel
    {
        public Nullable<int> LCOpeningId { get; set; }
        public int SlNo { get; set; }
        public int ConditionsId { get; set; }
        public string Condition { get; set; }
        public string LCDraftConditions { get; set; }

    }
    public class LCRetireDeductionModel
    {
        public Nullable<int> LCOpeningId { get; set; }
        public Nullable<int> LCRetirementId { get; set; }
        public string ReasonForDeduction { get; set; }
        public int SlNo { get; set; }
        public Nullable<int> RetireDeductsId { get; set; }
        public Nullable<decimal> LCRetireDeductsAmount { get; set; }

    }
    public class SearchLCOpeningModel
    {
        public string SearchINProjectNumber { get; set; }
        public string SearchINLCReferencenumber { get; set; }
        public string SearchINBeneficiaryName { get; set; }
        public string SearchINPONumber { get; set; }
        public string SearchINAmount { get; set; }
        public string SearchEXProjectNumber { get; set; }
        public string SearchEXLCReferencenumber { get; set; }
        public string SearchEXBeneficiaryName { get; set; }
        public string SearchEXPONumber { get; set; }
        public string SearchEXAmount { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string SearchINStatus { get; set; }
        public int TotalRecords { get; set; }
        public List<LCOpeningModel> LCList { get; set; }
    }
    #endregion
    
    #endregion
    #region BOA
    public class BOAModel
    {
        public int BOAId { get; set; }
        public Nullable<DateTime> PostedDate { get; set; }
        public Nullable<int> VoucherType { get; set; }
        public string TempVoucherNumber { get; set; }
        public string VoucherNumber { get; set; }
        public string TransactionTypeCode { get; set; }
        public string Narration { get; set; }
        public string RefTransactionCode { get; set; }
        public string RefNumber { get; set; }
        public int RefId { get; set; }
        public int Type { get; set; }
        public Nullable<DateTime> CrtTS { get; set; }
        public Nullable<int> PaymentMode { get; set; }
        public string Status { get; set; }
        public Nullable<int> RefBOAId { get; set; }
        public Nullable<Decimal> BOAValue { get; set; }
        public List<BOADetailModel> BOADetail { get; set; }
        public List<BOAPaymentDetailModel> BOAPaymentDetail { get; set; }
        
        public List<BOATransactionModel> BOATransaction { get; set; }
        public Nullable<bool> PaymentProcess_f { get; set; }
        public Nullable<bool> Posted_f { get; set; }
    }

    public class BOADetailModel
    {
        public int BOADetailId { get; set; }
        public Nullable<int> BOAId { get; set; }
        public Nullable<int> CommitmentDetailId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> BudgetHead { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> Payment_f { get; set; } = true;
    }

    public class BOAPaymentDetailModel
    {
        public Nullable<int> BOAId { get; set; }
        public Nullable<int> Reason { get; set; }
        public int BOAPaymentDetailId { get; set; }
        public string ReferenceNumber { get; set; }
        public Nullable<DateTime> ReferenceDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string PayeeBank { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> Reconciliation_f { get; set; }
        public string TransactionType { get; set; }
        public Nullable<int> BankHeadID { get; set; }
        public Nullable<int> PayeeId { get; set; }
        public Nullable<int> PaymentMode { get; set; }
        public Nullable<int> PayeeTypeId { get; set; }
        public string PayeeType { get; set; }
        public string PayeeName { get; set; }
        public string TransactionID { get; set; }
        public string TransactionStatus { get; set; }
        public string ChequeNumber { get; set; }
        public string StudentRoll { get; set; }
        public string AccountNumber { get; set; }
        public string IFSC { get; set; }
        public string Branch { get; set; }
        public Nullable<DateTime> VoucherDate { get; set; }
    }

   
    public class BOATransactionModel
    {
        public int BOATransactionId { get; set; }
        public Nullable<int> BOAId { get; set; }
        public Nullable<int> AccountHeadId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string TransactionType { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> Creditor_f { get; set; }
        public Nullable<bool> Debtor_f { get; set; }
        public Nullable<int> SubLedgerType { get; set; }
        public Nullable<int> SubLedgerId { get; set; }
        public Nullable<bool> Reconciliation_f { get; set; }
        public string StudentRoll { get; set; }
    }
    public class BOAExpModel
    {
       public List<tblBOAExpenditureDetail> listExp { get; set; }
        public List<tblBOAInvoiceDetail> listINV { get; set; }
        public List<tblBOACommitmentDetail> listCommit { get; set; }
    }   
    #endregion
    #region Payment Process
    public class PaymentProcessModel
    {
        public string Status { get; set; }
        public string PayeeName { get; set; }
        public string PaymentType { get; set; }
        public Nullable<int> PaymentPayeeId { get; set; }
        public Nullable<int> SelModeOfPayment { get; set; }
        public string ModeOfPayment { get; set; }
        public string ReferenceNumber { get; set; }
        public int SlNo { get; set; }
        public Nullable<decimal> PayableAmount { get; set; }

        public List<PaymentProcessVoucherModel> PaymentList { get; set; }
        public int TotalRecords { get; set; }
    }
    public class PaymentProcessVoucherModel
    {
        public Nullable<int> BOADraftId { get; set; }
        [Required]
        public Nullable<int> BankHead { get; set; }
        //[Required]
        //public Nullable<DateTime> PostingDate { get; set; }
        public string VoucherNumber { get; set; }
        public Nullable<decimal> BankTransferTotal { get; set; }
        public Nullable<decimal> ChequeTotal { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string VoucherDate { get; set; }
        public int SlNo { get; set; }
        public string Status { get; set; }
        public string Mode { get; set; }
        public string BankName { get; set; }
        public string BankHeadName { get; set; }
        public bool PFInit { get; set; }
        public string UTRStatus { get; set; }
        public bool IsendPaymentFailedMail { get; set; }
        public bool IsendUTRFailedMail { get; set; }
    }

    #endregion
    #region Bill Report
    public class BillReportModel
    {
        public int BillId { get; set; }
        public string BillDate { get; set; }
        public string BillMonth { get; set; }
        public string BankAccount { get; set; }
        public string RefNumber { get; set; }
        public string Remarks { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
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
       [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal PayableAmount { get; set; }
        public decimal TaxAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal TotalAmount { get; set; }
        public string BillHeading { get; set; }
        public string PrintedDate { get; set; }
        public string PrintedBy { get; set; }
        public string GSTIN { get; set; }
        public string PAN { get; set; }
        public string BankGuaranteeRemarks { get; set; }
        public List<PayableModel> Payable { get; set; }
        public List<CommListModel> Comm { get; set; }
        public List<TDSITListModel> TDSIT { get; set; }
        public List<TDSGSTModel> TDSGST { get; set; }
        public List<GSTModel> GST { get; set; }
        public List<InvoiceListModel> InvoiceList { get; set; }
        public List<AdvanceBillDetailsModel> Advance { get; set; }
    }
    public class AdvanceBillDetailsModel
    {
        public bool Advance_f { get; set; }
        public string AdvanceNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal AdvanceAmt { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal AdvanceGSTTDS { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal AdvanceITTDS { get; set; }
        public string AdvanceITTDSName { get; set; }
        public string AdvancePercentage { get; set; }
    }
    public class PayableModel
    {
        public string Name { get; set; }
        public string AccNo { get; set; }
        public string Bank { get; set; }
        public string IFSC { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Amount { get; set; }
        public string PAN { get; set; }
        public string GSTIN { get; set; }

    }
    public class CommListModel
    {
        public string Number { get; set; }
        public string ProjNo { get; set; }
        public string Head { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Value { get; set; }
        public string  StartDate { get; set; }
        public string Date { get; set; }
        public string SchemeCode { get; set; }
        public string ProjectType { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal NetBalance { get; set; }
    }
    public class TDSITListModel
    {
        public string Head { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Value { get; set; }
    }
    public class TDSGSTModel
    {
        public string Head { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Value { get; set; }
    }
    public class GSTModel
    {
        public string Head { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Value { get; set; }
    }
    #endregion
    #region ProjectSummary
    public class ProjSummaryModel
    {
        public string LoginTS { get; set; }
        public int ProjectId { get; set; }
        public int ProjId { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectStartDate { get; set; }
        public string ProjectCloseDate { get; set; }
        public decimal DistributionAmount { get; set; }
        public decimal ExpAmt { get; set; }
        public ProjectSummaryModel Summary { get; set; }
        public ProjectSummaryDetailModel Detail { get; set; }
        public ProjectDetailModel Common { get; set; }
        public string HeadName { get; set; }
        public decimal HeadAmount { get; set; }
        public string loginUser { get; set; }
        public string Date { get; set; }
        public string PrintDate { get; set; }
        public string CommitNo { get; set; }
        public decimal CommitAmt { get; set; }
        public decimal BalCommitAmt { get; set; }
        public decimal CommitBalanceAmt { get; set; }
        public string Remarks { get; set; }

        public string Currency { get; set; }
        public decimal ExcRate { get; set; }
        public string Addcharge { get; set; }
        public string PoNo { get; set; }
        public string vendorName { get; set; }
        public string ItemDesc { get; set; }
        public string StaffName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal MaxAllowed { get; set; }
        public decimal ForeCurr { get; set; }
        public decimal BudgAlloc { get; set; }
        public string Status { get; set; }
        public string SourceReference { get; set; }
        public string SourceEmail { get; set; }

        //IC36775 Commitmentype to be added 13/Dec/22 
        //for the ticket#8167

        public int CommitType { get; set; }
    }
    #endregion
    #region Project Status
    public class ProjectStatusUpdateModel
    {
        public int ProjectId { get; set; }
        public string ProjectNo { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

    #endregion
    #region Fixed Deposit
    public class FixedDepositModel : CommonPaymentModel
    {

        public Nullable<int> FixedDepositId { get; set; }
        [Required]
        [Display(Name = "Account Type")]
        public int AccountType { get; set; }
        [Required]
        [Display(Name = "Bank Account number")]
        public int BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public string TermDeposit { get; set; }
        public string FixedDepositNumber { get; set; }
        [Required]
        [Display(Name = "FD Number")]
        public string FDNumber { get; set; }
        [Required]
        [Display(Name = "Deposit Amount")]
        public Nullable<decimal> DepositAmount { get; set; }
        [Required]
        [Display(Name = "FD From Date")]
        public Nullable<DateTime> FDFromDate { get; set; }
        [Required]
        [Display(Name = "FD To Date")]
        public Nullable<DateTime> FDTODate { get; set; }
        [Required]
        [Display(Name = "Rate of Interest")]
        public Nullable<decimal> RateofInterest { get; set; }
        [Required]
        [Display(Name = "Total Interest")]
        public Nullable<decimal> TotalInterest { get; set; }
        [Required]
        [Display(Name = "Maturity Amount")]
        public Nullable<decimal> MaturityAmount { get; set; }
        public string Period { get; set; }
        public int SNo { get; set; }
        public string BankNumber { get; set; }
        public string Status { get; set; }
        public string AccounttypeName { get; set; }
        public string AccountNumber { get; set; }
        public string FromDatetime { get; set; }
        public string TofromDate { get; set; }
        public bool PF { get; set; }
        public string Remarks { get; set; }
    }
    public class SearchFixedDeposit
    {
        public string FixedDepositNumber { get; set; }
        public string Bankaccountnumber { get; set; }
        public string FDNumber { get; set; }
        public Nullable<decimal> DepositAmount { get; set; }
        public string Status { get; set; }
        public int TotalRecords { get; set; }
        public List<FixedDepositModel> fixlist { get; set; }
    }
    #endregion
    #region FixedDepostClosemodel
    public class FixedDepositClosedModel : CommonPaymentModel
    {
        public Nullable<int> FixedDepositClosedId { get; set; }
        public string FixedDepositNumber { get; set; }
        public int FixedDepositId { get; set; }
        public string FixedDepositClosedNumber { get; set; }
        [Required]
        [Display(Name = "Account Type")]
        public Nullable<int> AccountType { get; set; }
        [Required]
        [Display(Name = "Bank Account number")]
        public Nullable<int> BankAccountNumber { get; set; }
        [Required]
        [Display(Name = "FD Number")]
        public string FDNumber { get; set; }
        [Required]
        [Display(Name = "Deposit Amount")]
        public Nullable<decimal> DepositAmount { get; set; }
        [Required]
        [Display(Name = "FD From Date")]
        public Nullable<DateTime> FDFromDate { get; set; }
        [Required]
        [Display(Name = "FD To Date")]
        public Nullable<DateTime> FDTODate { get; set; }
        [Required]
        [Display(Name = "Rate of Interest")]
        public Nullable<decimal> RateofInterest { get; set; }
        [Required]
        [Display(Name = "Total Interest")]
        public Nullable<decimal> TotalInterest { get; set; }
        [Required]
        [Display(Name = "Maturity Amount")]
        public Nullable<decimal> MaturityAmount { get; set; }
        public string Period { get; set; }
        public int SNo { get; set; }
        public string BankNumber { get; set; }
        public string Status { get; set; }
        public string AccounttypeName { get; set; }
        public string AccountNumber { get; set; }
        public string FromDatetime { get; set; }
        public string TofromDate { get; set; }
        public bool PF { get; set; }
        public string Remarks { get; set; }
        [Required]
        [Display(Name = "Reason")]
        public string ResonForCloseing { get; set; }
        [Required]
        [Display(Name = "Actual Amount")]
        public Nullable<decimal> ActualAmount { get; set; }
    }

    public class SearchFixedDepositClose
    {
        public string FixedDepositClosedNumber { get; set; }
        public string Bankaccountnumber { get; set; }
        public string FDNumber { get; set; }
        public string FixedDepositNumber { get; set; }
        public Nullable<decimal> ActualAmount { get; set; }
        public Nullable<decimal> DepositAmount { get; set; }
        public string Status { get; set; }
        public int TotalRecords { get; set; }
        public List<FixedDepositClosedModel> fixclslist { get; set; }
    }
    #endregion
    #region Contractor Contingent Bill
    public class ContractorBillModel
    {
        public int SlNo { get; set; }
        public int ContractorBillId { get; set; }
        [Required]
        [Display(Name = "Project Number")]
        public Nullable<int> ProjectId { get; set; }
        public string ProjectNumber { get; set; }
        public string HeadOfAccounts { get; set; }
        public string AgreementNo { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string Address { get; set; }
        public Nullable<DateTime> FirmDate { get; set; }
        public string RefNo { get; set; }
        public Nullable<DateTime> CRTD_TS { get; set; }
        public Nullable<DateTime> UPTD_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public HttpPostedFileBase FinalAttachDoc { get; set; }
        public Nullable<DateTime> IssueDate { get; set; }
        public bool IsPdf { get; set; }
        public bool IsDoc { get; set; }
        public List<ContractorBillDetailsModel> BillDetails { get; set; }
        //public int[] ContractorBillDetailsId { get; set; }
        //public string []SupportingVocherNo { get; set; }
        //public DateTime []VoucherDate { get; set; }
        //public decimal[] Unit { get; set; }
        //public string []Description { get; set; }
        //public int[]Quantity { get; set; }
        //public decimal [] Rate { get; set; }
        //public decimal []TotalCost { get; set; }
        public string Remarks { get; set; }
        public string DocDetail { get; set; }

        public string IssuDate { get; set; }
        public string FDate { get; set; }
        public string TDate { get; set; }
        public string FirDate { get; set; }
        public string NewGenerateDoc { get; set; }
        public string Finalhref { get; set; }
        public string Newhref { get; set; }
        public string Totalcostwords { get; set; }
        public string CurrentDate { get; set; }
    }
    public class ContractorBillDetailsModel
    {
        public Nullable<int> ContractorBillDetailsId { get; set; }
        public Nullable<int> ContractorBillId { get; set; }
        public string SupportingVocherNo { get; set; }
        public Nullable<System.DateTime> VoucherDate { get; set; }
        public string vocDate { get; set; }
        public Nullable<decimal> Unit { get; set; }
        public string Description { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> TotalCost { get; set; }
        public string Amount { get; set; }
        public string DetailRemarks { get; set; }


    }
    public class ContingentBillModel
    {
        public Nullable<int> ProjectId { get; set; }
        [Required]
        [Display(Name = "Project Number")]
        public string ProjectNumber { get; set; }
        public Nullable<int> ConingentBillId { get; set; }
        public string VoucherNo { get; set; }
        public Nullable<decimal> AllotmentAmount { get; set; }
        public Nullable<decimal> ExpendedAmount { get; set; }
        public Nullable<decimal> BalanceAllotmentAmount { get; set; }
        public string ExpenditureAccount { get; set; }
        public string AuthorityNo { get; set; }
        public Nullable<System.DateTime> AuthorityDate { get; set; }
        public string AuthrityDate { get; set; }
        public Nullable<decimal> NetAmount { get; set; }
        public string Amounts { get; set; }
        public string PayDate { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string RefNo { get; set; }
        public int SlNo { get; set; }
        public int SlDate { get; set; }
        [Required]
        [Display(Name = "ProjectTittle")]
        public string ProjectTittle { get; set; }
        [Required]
        [Display(Name = "Project Cordinator")]
        public string ProjectCordinator { get; set; }
        public string DeptofEngg { get; set; }
        public int NoOfQuantity { get; set; }
        public Nullable<DateTime> AdvReceiveDate { get; set; }
        public string AdvRecDate { get; set; }
        public decimal Rate { get; set; }
        public decimal per { get; set; }
        public decimal PrjPayment { get; set; }
        public decimal ReceivedPayment { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public HttpPostedFileBase FinalAttachDoc { get; set; }
        public string DocDetail { get; set; }
        public string AdvReceivedFrom { get; set; }
        public string NewGenerateDoc { get; set; }
        public string Finalhref { get; set; }
        public string Newhref { get; set; }
        public string CurrentDate { get; set; }
        public string AmountinWords { get; set; }
        public string Duringyear { get; set; }
        public string Milestone { get; set; }
    }

    public class ContractSearchModel
    {
        public string SearchProjectNumber { get; set; }
        public string SearchRefNo { get; set; }
        public string HeadofAccounts { get; set; }
        public string SearchAgNo { get; set; }
        public List<ContractorBillModel> ContractList { get; set; }
        public int TotalRecords { get; set; }
    }
    public class ContingentSearchModel
    {
        public string SearchProjectNumber { get; set; }
        public string SearchProjecttitle { get; set; }
        public string SearchPIName { get; set; }
        public string SearchRefNo { get; set; }
        public List<ContingentBillModel> ContingentList { get; set; }
        public int TotalRecords { get; set; }
    }
    #endregion

    #region Deligate
    public class DeligateModel
    {
        public int DeligateId { get; set; }
        public string ApproverName { get; set; }
        public int ApproverId { get; set; }
        public List<DeligateListModel> ListModel { get; set; }
    }
    public class DeligateListModel
    {
        public int SlNo { get; set; }
        public int DeligateId { get; set; }
        public int ApproverId { get; set; }
        public string Approver { get; set; }
        public string ActualApprover { get; set; }
        public int ActualApproverId { get; set; }
        public int DeligateToId { get; set; }
        public string DeligateTo { get; set; }
        public string Process { get; set; }
        public int ProcessId { get; set; }
        public string Header { get; set; }
        public int HeaderId { get; set; }
        public string Status { get; set; }
    }
    #endregion
    #region Form 3C
    public class Form3CModel
    {
        public int SlNo { get; set; }
        public int Form3CId { get; set; }
        public int FormTypeId { get; set; }
        public string FormType { get; set; }
        public string VoucherNo { get; set; }
        public string ProjectNo { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ProjectTitle { get; set; }
        public string ProgramName { get; set; }
        public string RefNo { get; set; }
        public string RefDate { get; set; }
        public string StartDate { get; set; }
        public string Duration { get; set; }
        public string Assyear { get; set; }
        public string SancValue { get; set; }
        public string ReceiptNo { get; set; }
        public int ReceiptId { get; set; }
        public string RecAmt { get; set; }
        public string CheckNo { get; set; }
        public string Checkdate { get; set; }
        public string RecCount { get; set; }
        public string OverallRecAmt { get; set; }
        public string DeanName { get; set; }
        public string SponserName { get; set; }
        public string PAN { get; set; }
        public string NatureOfBus { get; set; }
        public string TurnOver { get; set; }
        public string Installment { get; set; }
        public string DocDetail { get; set; }
        public string NewGenerateDoc { get; set; }
        public string Finalhref { get; set; }
        public string Newhref { get; set; }
        public HttpPostedFileBase FinalAttachDoc { get; set; }

        public string AnnualResearch { get; set; }
        public string DeductionDetails { get; set; }
        public string AgreementDetails { get; set; }

    }
    public class Form3CSearchModel
    {
        public string SearchProjectNumber { get; set; }
        public string SearchRefNo { get; set; }
        public List<Form3CModel> Form3CList { get; set; }
        public int TotalRecords { get; set; }
    }
    public class ProjectForm3CDoc
    {
        public string DocPath { get; set; }
        public string DocDescrip { get; set; }
        public string DocName { get; set; }
        public string AttachName { get; set; }
    }
    #endregion
    #region AnnualAccounts
    public class AnnualAccountsModel
    {
        public bool ProposalNumber { get; set; }
        public bool ProjectNumber { get; set; }
        public bool ProjectType { get; set; }
        public bool PIName_f { get; set; }
        public bool SanctionOrderNumber { get; set; }
        public bool SanctionDate { get; set; }
        public bool ProjectTitle { get; set; }
        public bool StartDate { get; set; }
        public bool ClosureDate { get; set; }
        public bool sanctionValue { get; set; }
        public bool NetReceiptAmt { get; set; }
        public bool SponReceipt { get; set; }
        public bool Receipts { get; set; }
        public bool NetExpAmt { get; set; }
        public bool SponExp { get; set; }
        public bool TotalExpenditure { get; set; }
        public bool IOASOB { get; set; }
        public bool ClosingBalance { get; set; }
        public bool OpeningBalance { get; set; }
        public bool NegativeApprove { get; set; }
        public bool Commitment { get; set; }
        public bool AllDetails { get; set; }
        public bool Fin_Year { get; set; }
        public bool ProjectClassification { get; set; }
        ///Filter
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public Nullable<int> PI { get; set; }
        public string piname { get; set; }
        public Nullable<int> FinYear { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> Balance { get; set; }
        public Nullable<int> ProjId { get; set; }
        public string ProjNo { get; set; }
        public Nullable<int> ProjType { get; set; }
        public Nullable<int> ProjClass { get; set; }
        public string SponCat { get; set; }
        public string Department { get; set; }
        public string[] AlloHead { get; set; }
        public string[] ExpHead { get; set; }
        public bool Agency { get; set; }
        public bool Scheme { get; set; }
        public string AgencyName { get; set; }
        public string AgencyNameId { get; set; }
        public string SchemeCode { get; set; }
        public string SchemeCodeId { get; set; }
    }
    #endregion

    public class ChatModel
    {
        public string name { get; set; }
        public string date { get; set; }
        public string msg { get; set; }
    }
    #region CommonProjectSearchModel
    public class CommonProjectSearchModel
    {
        public List<CommonProjectListSearchModel> Project { get; set; }
        public List<CommonProjectListSearchModel> Commitment { get; set; }
        public List<CommonInvoiceListSearchModel> Invoice { get; set; }
    }
    public class CommonProjectListSearchModel
    {
        public string ProjectNumber { get; set; }
        public string CommitmentNumber { get; set; }
        public string AllocationHead { get; set; }
        public decimal Amount { get; set; }
    }
    public class CommonInvoiceListSearchModel
    {
        public string InvoiceNumber { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string Date { get; set; }
    }
    #endregion
    #region pay in slip
    public class CreateInvoiceModel
    {




        public int Sno { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Nullable<int> InvoiceDraftId { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public string TypeofInvoice { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string Invoicedatestrng { get; set; }
        public Nullable<int>[] PreviousInvoiceId { get; set; }
        public string[] PreviousInvoiceNumber { get; set; }
        public string[] PreviousInvoiceDate { get; set; }
        public string[] PreviousInvoicedatestrng { get; set; }
        public Nullable<int>[] InstalmentId { get; set; }
        public Nullable<int>[] InstlmntNumber { get; set; }
        public Nullable<Decimal>[] InstalValue { get; set; }
        public Nullable<int>[] Instalmentyear { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] PreviousInvoicevalue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> AvailableBalance { get; set; }
        [Required]
        public string Projecttitle { get; set; }
        [Required]
        public Nullable<int> PIId { get; set; }
        public Nullable<int> Prjcttype { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public string SelectProject { get; set; }
        public string SelectInvoice { get; set; }
        public string NameofPI { get; set; }
        [Required]
        public Nullable<int> SponsoringAgency { get; set; }
        public string SponsoringAgencyName { get; set; }
        public int InvoicecrtdID { get; set; }
        public string ProjectNumber { get; set; }
        public string ProposalNumber { get; set; }
        public int PrpsalNumber { get; set; }
        [Required]
        public Nullable<int> Department { get; set; }
        public string PIDepartmentName { get; set; }
        public string Remarks { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> Sanctionvalue { get; set; }
        public string CurrentFinancialYear { get; set; }
        public string GSTNumber { get; set; }
        public string PAN { get; set; }

        public string TAN { get; set; }
        [Required]
        public string Agencyregname { get; set; }
        [Required]
        public string Agencyregaddress { get; set; }
        [Required]
        public string Agencydistrict { get; set; }
        [Required]
        public string Agencystate { get; set; }
        public Nullable<int> AgencyCountryId { get; set; }
        public Nullable<int> AgencystateId { get; set; }
        [Required]
        public Nullable<int> Agencystatecode { get; set; }
        public Nullable<int> Agncystatecode { get; set; }
        [Required]
        public Nullable<int> AgencyPincode { get; set; }
        [Required]
        public string Agencycontactperson { get; set; }
        public string Agencycontactpersondesignation { get; set; }


        public string AgencycontactpersonEmail { get; set; }
        public string Agencycontactpersonmobile { get; set; }
        public string SanctionOrderNumber { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> SanctionOrderDate { get; set; }
        public string SODate { get; set; }
        public string SACNumber { get; set; }
        public string IITMGSTIN { get; set; }
        public Nullable<int> ServiceType { get; set; }
        [Required]
        public string DescriptionofServices { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TaxableValue { get; set; }

        public Nullable<Decimal> CGST { get; set; }
        public Nullable<Decimal> CGSTRate { get; set; }

        public string CGSTPercentage { get; set; }

        public Nullable<Decimal> SGST { get; set; }
        public Nullable<Decimal> SGSTRate { get; set; }

        public string SGSTPercentage { get; set; }

        public Nullable<Decimal> IGST { get; set; }
        public Nullable<Decimal> IGSTRate { get; set; }
        public Nullable<Decimal> AmountReceived { get; set; }
        public string IGSTPercentage { get; set; }
        public Nullable<Decimal> TotalTaxValue { get; set; }
        public string TotalTaxpercentage { get; set; }
        public Nullable<int> TotalTaxpercentageId { get; set; }
        public Nullable<Decimal> TotalInvoiceValue { get; set; }
        public string CurrencyCode { get; set; }
        public string TotalInvoiceValueinwords { get; set; }
        public string CommunicationAddress { get; set; }
        public int Bank { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public Nullable<int> BankAccountId { get; set; }
        public Nullable<int> Instalmentnumber { get; set; }
        public Nullable<int> Instlmntyr { get; set; }
        public Nullable<Decimal> Instalmentvalue { get; set; }
        public string[] Invoiced { get; set; }
        public Nullable<int> TaxStatus { get; set; }
        public string PONumber { get; set; }
        public Nullable<int> CurrentFinyearId { get; set; }
        public Nullable<Decimal> TotalOpenInvoiceValue { get; set; }
        public Nullable<Decimal> TotalReceiptValue { get; set; }
        public Nullable<Decimal> TotalOpenBalReceiptValue { get; set; }
        public Nullable<Decimal> CurrInvVal { get; set; }
        public string Status { get; set; }
        public Nullable<int> SelCurr { get; set; }
        public Nullable<Decimal> ForeignCurrencyValue { get; set; }
        public Nullable<Decimal> AllocatedForeignCurrencyValue { get; set; }
        public Nullable<Decimal> ForgnCurrenyRate { get; set; }
        public Nullable<Decimal> PrevinvForeignCurrencyValue { get; set; }
        public Nullable<Decimal> TDSReceived { get; set; }
        public Nullable<Decimal> OutstandingAmount { get; set; }
        public Nullable<Decimal> TDSReceivableGST { get; set; }
        public string PayinslipDate { get; set; }
        public string ModeofPayment { get; set; }
        public string PaySlipReferenceNumber { get; set; }
        public string ReceiptReference { get; set; }
        public string InstrumentsDate { get; set; }
        public string InstrumentsRemarks { get; set; }
        public Nullable<int> PayinslipId { get; set; }
        public Nullable<int> ExternalRefId { get; set; }
        public string CreditNoteNo { get; set; }
        public Nullable<Decimal> TotalCreditNoteAmount { get; set; }
      
    }
    public class SearchInvoiceModel : CommonJSGridFilterModel
    {
        public string SearchEXInvoiceNumber { get; set; }
        public Nullable<int> SearchEXPIName { get; set; }
        public string SearchEXProjectNumber { get; set; }
        public string SearchEXPayslipNumber { get; set; }
        public Nullable<int> SearchEXInvoiceType { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string SearchINInvoiceNumber { get; set; }
        public string SearchINPayslipNumber { get; set; }
        public string SearchINPIName { get; set; }
        public string SearchINAgency { get; set; }
        public string SearchINProjectNumber { get; set; }
        public string SearchINInvoiceType { get; set; }
        public string SearchINStatus { get; set; }
        public int TotalRecords { get; set; }
        public Nullable<int> projectId { get; set; }
        public Nullable<decimal> AmountReceived { get; set; }
    }
    #endregion

    #region UTR
    public class UTRModel
    {
        public Nullable<Int32> UTRMasterId { get; set; }
        public Nullable<Int32> BOADraftId { get; set; }
        public string UTRMasterNumber { get; set; }
        public string BatchNumber { get; set; }
        public string DocumentName { get; set; }
        public string DocumentActualName { get; set; }
        public string DocumentPath { get; set; }
        public string Status { get; set; }
        public Nullable<bool> IsValid { get; set; } = true;
        [Required]
        [Display(Name = "Verified by")]
        public Nullable<int> Verified_By { get; set; }
        public string VerifierName { get; set; }
        public List<UTRStatementDetailModel> txDetail { get; set; }
    }
    public class UTRStatementDetailModel
    {
        public Nullable<Int32> BOADraftDetailId { get; set; }
        public string Name { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
        public string Recordreferencenumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<DateTime> InputValueDate { get; set; }
        public string Status { get; set; }
        public string UserReferenceNumber { get; set; }
        public string UTRNO { get; set; }
        public string VerifyStatus { get; set; }
    }
    #endregion
}