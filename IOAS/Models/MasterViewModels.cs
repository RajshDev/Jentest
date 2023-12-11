using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models
{
    public class BankMasterViewModel
    {

        public string Bankname { get; set; }
        public string AccountNumber { get; set; }
        public string BranchName { get; set; }
        public string SwiftCode { get; set; }
        public string MICRCode { get; set; }
        public string IFSCCode { get; set; }
        public string BankAddress { get; set; }
    }


    public class InternalAgencyViewModel
    {
        public int sno { get; set; }
        public Nullable<int> InternalAgencyId { get; set; }
        [Required]
        [Display(Name = "Agency Name")]
        public string InternalAgencyName { get; set; }
        [Required]
        [Display(Name = "Agency Code")]
        public string InternalAgencyCode { get; set; }
        //[Required]
        //[Display(Name = "Contact Person")]
        public string InternalAgencyContactPerson { get; set; }
        //[Required]
        //[DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Address")]
        public string InternalConatactEmail { get; set; }
        //[Required]
        //[Display(Name = "Contact Number")]
        public string InternalAgencyContactNumber { get; set; }
        //[Required]
        //[Display(Name = "Agency Address")]
        public string InternalAgencyAddress { get; set; }
        //[Required]
        //[Display(Name = "Agency Register Name")]
        public string InternalAgencyRegisterName { get; set; }
        //[Required]
        //[Display(Name = "Agency Register Address")]
        public string InternalAgencyRegisterAddress { get; set; }
        public string InternalDistrict { get; set; }
        public Nullable<int> InternalPincode { get; set; }
        //[Required]
        //[Display(Name = "State")]
        public string InternalAgencyState { get; set; }

        public int InternalAgencyUserId { get; set; }
        public string InternalAgencyType { get; set; }
        public HttpPostedFileBase[] File { get; set; }
        public string[] DocPath { get; set; }
        public int[] DocumentType { get; set; }
        public string[] DocumentName { get; set; }
        public string[] AttachName { get; set; }
        public int[] DocumentId { get; set; }
        public int UserId { get; set; }
        public int ProjectType { get; set; }
        public string SearchAgencyName { get; set; }
        public string SearchAgencyCode { get; set; }
    }
    public class TdsSectionModel
    {
        public string NatureOfIncome { get; set; }
        public decimal Percentage { get; set; }
    }

    public class LedgerOBBalanceModel
    {
        public int AccountCategoryId { get; set; }
        public int FinalYearId { get; set; }
        public string AccountGroupName { get; set; }
        public string AccountHeadName { get; set; }
        public decimal CurrentOpeningBalance { get; set; }
        public decimal PopupCurrentOpeningBalance { get; set; }
        public decimal PopModeifiedOpeningBalance { get; set; }
        public int Userid { get; set; }
        public string Password { get; set; }
        public int HeadOpeningBalanceId { get; set; }
        public int AccountHeadId { get; set; }
        public string Username { get; set; }
        public int sno { get; set; }

    }

    public class BankAccountMaster
    {
        public Nullable<int> StaffBankId { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int Category { get; set; }
        [Required]
        [Display(Name = "Bank Type")]
        public int BankType { get; set; }
        [Required]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }
        public string Userid { get; set; }
        public int CreateUser { get; set; }
        [Required]
        [Display(Name = "IFSC code")]
        public string IFSCCode { get; set; }
        [Required]
        [Display(Name = "Branch")]
        public string Branch { get; set; }
        [Required]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
        [Required]
        [Display(Name = "Accountant Name")]
        public string AccountantName { get; set; }
        public string PAN { get; set; }

        [Required]
        [Display(Name = "Payment Type")]
        public Nullable<int> PayFor { get; set; }
    }
    public class BankMasterListModel
    {
        public int SNo { get; set; }
        public int StaffBankId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
    }
    public class SearchBankMaster
    {
        public string CategorySearch { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public int TotalRecords { get; set; }
        public List<BankMasterListModel> listbank { get; set; }
    }
    public class VendorMasterViewModel
    {
        public Nullable<int> VendorId { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int Nationality { get; set; }
        public string VendorCode { get; set; }
        public string PFMSVendorCode { get; set; }
        public string Name { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        [RequiredIf("Nationality", 1, ErrorMessage = "Please Enter Contact Person")]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Number Must be numeric")]
        [RequiredIf("Nationality", 1, ErrorMessage = "Mobile Number Required")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [RequiredIf("Nationality", 1, ErrorMessage = "Please Enter Registered Name")]
        [Display(Name = "Registered Name")]
        public string RegisteredName { get; set; }
        [MaxLength(10)]
        [RegularExpression("[A-Z]{5}[0-9]{4}[A-Z]{1}", ErrorMessage = "Invalid PAN Number")]
        [Display(Name = "PAN Number")]
        public string PAN { get; set; }
        [MaxLength(10)]
        [RegularExpression("[A-Z]{4}[0-9]{5}[A-Z]{1}", ErrorMessage = "Invalid TAN Number")]
        [Display(Name = "TAN Number")]
        public string TAN { get; set; }
        [MaxLength(15)]
        // [RegularExpression("^([0]{1}[1-9]{1}|[1-2]{1}[0-9]{1}|[3]{1}[0-8]{1})([a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9a-zA-Z]{1}[zZ]{1}[0-9a-zA-Z]{1})+$", ErrorMessage = "Invalid GST Number")]
        [Display(Name = "GST Number")]
        public string GSTIN { get; set; }

        public bool GSTExempted { get; set; }
        public string Reason { get; set; }
        [Required]
        [Display(Name = "Account Holder Name")]
        public string AccountHolderName { get; set; }
        [Required]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
        [RequiredIf("Nationality", 1, ErrorMessage = "Please Enter Branch")]
        [Display(Name = "Branch")]
        public string Branch { get; set; }
        [RequiredIf("Nationality", 1, ErrorMessage = "Please Enter Bank Ifsc Code")]
        [Display(Name = "IFSC Code")]
        public string IFSCCode { get; set; }
        [Required]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }
        [RequiredIf("Nationality", 1, ErrorMessage = "Bank Address")]
        [Display(Name = "Bank Address")]
        public string BankAddress { get; set; }
        public Nullable<int> BankCountry { get; set; }
        public string ABANumber { get; set; }
        public string SortCode { get; set; }
        public string IBAN { get; set; }
        public string SWIFTorBICCode { get; set; }
        public bool ReverseTax { get; set; }
        public bool TDSExcempted { get; set; }
        [RequiredIf("TDSExcempted", true, ErrorMessage = "Please CertificateNumber is Required")]
        public string CertificateNumber { get; set; }
        [RequiredIf("TDSExcempted", true, ErrorMessage = "Please Validity Period is Required")]
        public Nullable<int> ValidityPeriod { get; set; }
        [RequiredIf("Nationality", 2, ErrorMessage = "Please Country name Field is Required")]
        [Display(Name = "Country Name")]
        public Nullable<int> CountryId { get; set; }
        [RequiredIf("Nationality", 1, ErrorMessage = "Please State name Field is Required")]
        [Display(Name = "State Name")]
        public Nullable<int> StateId { get; set; }
        [Display(Name = "State Code")]
        [RequiredIf("Nationality", 1, ErrorMessage = "Please State Code Field is Required")]
        public Nullable<int> StateCode { get; set; }
        public HttpPostedFileBase[] GSTFile { get; set; }
        public string[] GSTDocPath { get; set; }
        public int[] GSTDocumentType { get; set; }
        public string[] GSTDocumentName { get; set; }
        public string[] GSTAttachName { get; set; }
        public int[] GSTDocumentId { get; set; }
        public int UserId { get; set; }
        public HttpPostedFileBase[] VendorFile { get; set; }
        public string[] VendorDocPath { get; set; }
        public int[] VendorDocumentType { get; set; }
        public string[] VendorDocumentName { get; set; }
        public string[] VendorAttachName { get; set; }
        public int[] VendorDocumentId { get; set; }
        public HttpPostedFileBase[] TDSFile { get; set; }
        public string[] TDSDocPath { get; set; }
        public int[] TDSDocumentType { get; set; }
        public string[] TDSDocumentName { get; set; }
        public string[] TDSAttachName { get; set; }
        public int[] TDSDocumentId { get; set; }
        public int sno { get; set; }
        public string CountryName { get; set; }
        public string VendorSearchname { get; set; }
        public string VendorsearchCode { get; set; }
        public Nullable<int> VendorCountry { get; set; }
        [RequiredIf("Nationality", 1, ErrorMessage = "Service Category Required")]
        [Display(Name = "Service Category")]
        public Nullable<int> ServiceCategory { get; set; }
        [RequiredIf("ServiceCategory", 1, ErrorMessage = "Service Type Required")]
        [Display(Name = "Service Type")]
        public Nullable<int> ServiceType { get; set; }
        [RequiredIf("ServiceCategory", 2, ErrorMessage = "Supplier Type Required")]
        [Display(Name = "Supplier Type")]
        public Nullable<int> SupplierType { get; set; }
        public string ReverseTaxReason { get; set; }
        public string BankNature { get; set; }

        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Address")]
        public string BankEmailId { get; set; }
        public string MICRCode { get; set; }
        public List<MasterlistviewModel> PONumberList { get; set; }
        public List<MasterlistviewModel> TDSList { get; set; }
        // public List<ApplicableTDSModel> TDSDetail { get; set; }

        public int[] VendorTDSDetailId { get; set; }
        public int[] Section { get; set; }
        public string[] NatureOfIncome { get; set; }
        public decimal[] TDSPercentage { get; set; }
        public string PinCode { get; set; }
        public string City { get; set; }
        public string Status { get; set; }
        public bool ClearanceAgency_f { get; set; }
        public bool TravelAgency_f { get; set; }
        public decimal TDSLimit { get; set; }
    }



    public class VendorSearchModel
    {

        public string INVendorSearchname { get; set; }
        public string INVendorsearchCode { get; set; }
        public string INStatus { get; set; }
        public Nullable<int> EXCountryName { get; set; }
        public string EXVendorSearchname { get; set; }
        public string EXINVendorsearchCode { get; set; }
        public string INBankName { get; set; }
        public string INAccountNumber { get; set; }
        public int TotalRecords { get; set; }
        public List<VendorMasterViewModel> VendorList { get; set; }
    }
    #region AdhocPICreationModel
    public class AdhocPICreationModel
    {
        public Nullable<int> FacultyDetailsId { get; set; }
        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "PI Code should be 6 characters")]
        [Display(Name = "PI Code")]
        public string PICode { get; set; }
        [Required]
        [Display(Name = "PI Name")]
        public string PIName { get; set; }

        public string Designation { get; set; }
        [Required]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Department Code should be 2 characters")]
        [Display(Name = "Department Code")]
        public string DepartmentCode { get; set; }
        [Required]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public Nullable<int> SNo { get; set; }
    }
    public class AdhocPISearchModel
    {
        public string PIName { get; set; }
        public string PICode { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public int TotalRecords { get; set; }
        public List<AdhocPICreationModel> list { get; set; }
    }
    #endregion
}