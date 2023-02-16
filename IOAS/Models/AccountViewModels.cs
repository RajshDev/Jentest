using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Linq;
using System.IO;
using Foolproof;
using System.ComponentModel.DataAnnotations.Schema;
using IOAS.DataModel;

namespace IOAS.Models
{
    //public class CommitmentModel
    //{
    //    public Nullable<int> commitmentId { get; set; }
    //    [Display(Name = "Commitment type")]
    //    [Required]
    //    public int selCommitmentType { get; set; }

    //    [RequiredIf("selCommitmentType", "1", ErrorMessage = "Purpose field is required")]
    //    public Nullable<int> selPurpose { get; set; }
    //    public string Remarks { get; set; }
    //    //public int selAccountGroup { get; set; }
    //    //public int selAccountHead { get; set; }
    //    [Required]
    //    [Display(Name = "Allocation Head")]
    //    public int[] selAllocationHead { get; set; }
    //    [Required]
    //    [Display(Name = "Allocation Value")]
    //    public decimal[] AllocationValue { get; set; }
    //    [RequiredIf("selCommitmentType", "3", ErrorMessage = "PO number field is required")]
    //    public string PONumber { get; set; }

    //    [RequiredIf("selCommitmentType", "3", ErrorMessage = "Vendor field is required")]
    //    public Nullable<int> selVendor { get; set; }
    //    [RequiredIf("selCommitmentType", "5", ErrorMessage = "Currency field is required")]
    //    public Nullable<int> selCurrency { get; set; }
    //    public Nullable<decimal> currencyRate { get; set; }
    //    [Display(Name = "Project type")]
    //    public Nullable<int> selProjectType { get; set; }

    //    public string projectNumber { get; set; }
    //    [Display(Name = "Project No")]
    //    [Required]
    //    public int SelProjectNumber { get; set; }
    //    [Display(Name = "Request refrence")]
    //    [Required]
    //    public int selRequestRefrence { get; set; }
    //    public bool SourceTapalOrWorkflow
    //    {
    //        get
    //        {
    //            return (this.selRequestRefrence == 1 || this.selRequestRefrence == 3);
    //        }
    //    }
    //    [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
    //    public string selRefNo { get; set; }
    //    [RequiredIf("selRequestRefrence", "2", ErrorMessage = "Email date field is required")]
    //    public Nullable<DateTime> EmailDate { get; set; }

    //    [RequiredIf("selCommitmentType", "5", ErrorMessage = "Foreign Currency Value field is required")]
    //    public Nullable<decimal> ForeignCurrencyValue { get; set; }
    //    [Required]
    //    [Display(Name = "Funding body")]
    //    public int selFundingBody { get; set; }
    //    public decimal commitmentValue { get; set; }
    //    public string CommitmentNo { get; set; }
    //    public decimal CommitmentBalance { get; set; }

    //    public List<AllocationDetailsModel> ListAllocation { get; set; }

    //    public ProjectSummaryModel prjDetails { get; set; }

    //    public CommitSearchFieldModel SearchField { get; set; }
    //    public Nullable<decimal> AmountSpent { get; set; }
    //    [RequiredIf("selCommitmentType", "5", ErrorMessage = "Start date field is required")]
    //    public Nullable<DateTime> StartDate { get; set; }
    //    [RequiredIf("selCommitmentType", "5", ErrorMessage = "Close date field is required")]
    //    public Nullable<DateTime> CloseDate { get; set; }

    //    public Nullable<decimal> BasicPay { get; set; }

    //    public Nullable<decimal> MedicalAllowance { get; set; }

    //    [RequiredIf("selCommitmentType", "5", ErrorMessage = "Employee name field is required")]
    //    public string EmployeeId { get; set; }

    //    public Nullable<decimal> Total { get; set; }

    //    public int AttachType { get; set; }
    //    public string AttachName { get; set; }
    //    public HttpPostedFileBase file { get; set; }
    //    public string AttachPath { get; set; }
    //    public string DocName { get; set; }
    //    public bool IsDeansApproval { get; set; }
    //    public Nullable<decimal> AdditionalCharge { get; set; }
    //}
    public class AgencySearchModel
    {
        public string InName { get; set; }
        public string InCode { get; set; }
        public string ExName { get; set; }
        public string Excode { get; set; }
        public Nullable<int> ExCountry { get; set; }
        public List<AgencyModel> agencylist { get; set; }
        public int TotalRecords { get; set; }
    }
    public class VendorManagementModel
    {
        public int VedorId { get; set; }
        public string VendorName { get; set; }
        public string PFMSCode { get; set; }
        public string Category { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string StateCode { get; set; }
        public string City { get; set; }
        public string PinCode { get; set; }
        public string RegName { get; set; }
        public string PAN { get; set; }
        public string TAN { get; set; }
        public string GSTIN { get; set; }
        public string TaxExpReason { get; set; }
        public string IdentityAttachmentType { get; set; }
        public string IdentityAttachmentName { get; set; }
        public string IdentityAttachment { get; set; }
        public string AccHolderName { get; set; }
        public string AccNo { get; set; }
        public string IFSC { get; set; }
        public string BankName { get; set; }
        public string BankNature { get; set; }
        public string Branch { get; set; }
        public string BankAddress { get; set; }
        public string BankEmail { get; set; }
        public string MICRCode { get; set; }
        public string ServiceCategory { get; set; }
        public string Type { get; set; }
        public string ReverseTaxReson { get; set; }
        public string ServiceAttachmentType { get; set; }
        public string ServiceAttachmentName { get; set; }
        public string ServiceAttachment { get; set; }
        public string CertificateNo { get; set; }
        public string ValidityReson { get; set; }
        public bool GSTExp { get; set; }
        public bool RevTax { get; set; }
        public bool TDSExp { get; set; }
        public string ABANumber { get; set; }
        public string SortCode { get; set; }
        public string IBAN { get; set; }
        public string SWiftCode { get; set; }
        public string BankCountry { get; set; }
        public bool ClearanceAgency_f { get; set; }
        public bool TravelAgency_f { get; set; }
        public List<VendorTDSDetails> List { get; set; }
        public List<GSTDocumentModel> GstDocument { get; set; }
        public List<TDSDocumentModel> TdsDocument { get; set; }
        public List<TaxDocumentModel> TaxDocument { get; set; }
    }

    public class GSTDocumentModel
    {
        public int GstDocumentid { get; set; }
        public string GstDocumentName { get; set; }
        public string GstAttachementName { get; set; }
        public string GstDocumenttypename { get; set; }
        public string GstDocumentPath { get; set; }

    }
    public class TDSDocumentModel
    {
        public int TdsDocumentid { get; set; }
        public string TdsDocumentName { get; set; }
        public string TdsAttachementName { get; set; }
        public string TdsDocumenttypename { get; set; }
        public string TdsDocumentPath { get; set; }

    }
    public class TaxDocumentModel
    {
        public int TaxDocumentid { get; set; }
        public string TaxDocumentName { get; set; }
        public string TaxAttachementName { get; set; }
        public string TaxDocumenttypename { get; set; }
        public string TaxDocumentPath { get; set; }

    }
    public class CommitmentModel
    {
        [Display(Name = "Subhead")]
        public string selSubhead { get; set; }
        public List<AttachmentDetailModel> doc { get; set; }
        public Nullable<int> commitmentId { get; set; }
        public int commitmentDetialId { get; set; }
        [Display(Name = "Commitment type")]
        [Required]
        public int selCommitmentType { get; set; }
        public Nullable<int> selPurpose { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        //public int selAccountGroup { get; set; }
        //public int selAccountHead { get; set; }
        [Required]
        [Display(Name = "Allocation Head")]
        public int selAllocationHead { get; set; }
        [Required]
        [Display(Name = "Allocation Value")]
        public decimal AllocationValue { get; set; }
        [RequiredIf("selCommitmentType", "3", ErrorMessage = "PO number field is required")]
        public string PONumber { get; set; }

        //[RequiredIf("selCommitmentType", "3", ErrorMessage = "Vendor field is required")]
        //public Nullable<int> selVendor { get; set; }

        [RequiredIf("selCommitmentType", "3", ErrorMessage = "Vendor field is required")]
        public string VendorName { get; set; }
        [RequiredIf("selCommitmentType", "5", ErrorMessage = "Currency field is required")]
        public Nullable<int> selCurrency { get; set; }
        public Nullable<decimal> currencyRate { get; set; }

        public Nullable<int> selProjectType { get; set; }

        public string ItemDescription { get; set; }
        public string projectNumber { get; set; }
        [Display(Name = "Project No")]
        [Required]
        public int SelProjectNumber { get; set; }
        [Display(Name = "Request refrence")]
        [Required]
        public int selRequestRefrence { get; set; }
        public string RefNo { get; set; }
        public bool SourceTapalOrWorkflow
        {
            get
            {
                return (this.selRequestRefrence == 1 || this.selRequestRefrence == 3);
            }
        }
        [RequiredIf("SourceTapalOrWorkflow", true, ErrorMessage = "Ref. Number field is required")]
        public string selRefNo { get; set; }
        [RequiredIf("selRequestRefrence", "2", ErrorMessage = "Email date field is required")]
        public Nullable<DateTime> EmailDate { get; set; }
        public decimal commitmentValue { get; set; }
        public string CommitmentNo { get; set; }
        public bool AllocationNR_f { get; set; }
        public decimal CommitmentBalance { get; set; }

        public List<AllocationDetailsModel> ListAllocation { get; set; }

        public ProjectSummaryModel prjDetails { get; set; }

        public CommitSearchFieldModel SearchField { get; set; }
        public Nullable<decimal> AmountSpent { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> CloseDate { get; set; }

        public Nullable<decimal> BasicPay { get; set; }

        public Nullable<decimal> MedicalAllowance { get; set; }
        public string EmployeeId { get; set; }
        public Nullable<int> CastEmployeeId { get; set; }

        public Nullable<decimal> Total { get; set; }

        public int AttachType { get; set; }
        public string AttachName { get; set; }
        public HttpPostedFileBase file { get; set; }
        public string AttachPath { get; set; }
        public string DocName { get; set; }
        public bool IsDeansApproval { get; set; }
        public Nullable<decimal> AdditionalCharge { get; set; }
        [RequiredIf("selCommitmentType", "5", ErrorMessage = "Foreign Currency Value field is required")]
        public Nullable<decimal> ForeignCurrencyValue { get; set; }
        [Required]
        [Display(Name = "Funding body")]
        public int selFundingBody { get; set; }
        public CommitAllocationHeadDetails AllocationDtls { get; set; }
        public ProjectDetailModel ProjectDetail { get; set; }
        public int FinYearId { get; set; } = Infrastructure.Common.GetCurrentFinYearId();

        [Required]
        [Display(Name = "Bank Name")]
        public Nullable<Int32> BankId { get; set; }

        public Nullable<Int32> BankHeadId { get; set; }
        public string BankName { get; set; }
    }
    public class CommitmentResultModel
    {
        public CommitAllocationHeadDetails AllocModel { get; set; }
        [Required]
        public int AllHeadId { get; set; }
        [Required]
        public decimal AvlBalance { get; set; }
        [Required]
        public decimal AddCloseAmt { get; set; }
        [Required]
        public int LogTypeId { get; set; }
        public int ProjectId { get; set; }
        /// <summary>
        /// //
        /// </summary>
        public int ComitmentId { get; set; }
        public int SlNo { get; set; }
        public string CommitmentType { get; set; }
        public string Purpose { get; set; }
        public string Remarks { get; set; }
        public string Currency { get; set; }
        public decimal CurrencyRate { get; set; }
        public string ProjectType { get; set; }
        public string RequestRef { get; set; }
        public string RefNo { get; set; }
        public string EmailDate { get; set; }
        public string CommitmentNo { get; set; }
        public string projectNumber { get; set; }
        public string PONumber { get; set; }
        public string VendorName { get; set; }
        public string AllocationHead { get; set; }
        public decimal AllocationValue { get; set; }
        public decimal CommitmentAmount { get; set; }
        public decimal AmountSpent { get; set; }
        public string CreatedDate { get; set; }
        public string Status { get; set; }
        public string StartDate { get; set; }
        public string CloseDate { get; set; }
        public decimal BasicPay { get; set; }
        public decimal MedicalAllowance { get; set; }
        public string EmployeeName { get; set; }
        public decimal Total { get; set; }
        public int AttachType { get; set; }
        public string AttachName { get; set; }
        public string AttachPath { get; set; }
        public string DocName { get; set; }
        public bool IsDeansApproval { get; set; }
        public decimal AdditionalCharge { get; set; }
        public ProjectSummaryModel prjDetails { get; set; }
        public int Action { get; set; }
        public int Reason { get; set; }
        public string strRemarks { get; set; }
        public string NewGenerateDoc { get; set; }
        public string Finalhref { get; set; }
        public string Newhref { get; set; }
        public HttpPostedFileBase FinalAttachDoc { get; set; }
        public string BankHeadName { get; set; }
        public int BankHeadId { get; set; }
        public int Pre_BankID { get; set; }
    }

    public class CommitAllocationHeadDetails
    {
        public decimal TotalAllocation { get; set; }
        public decimal AllocationForCurrentYear { get; set; }
        public decimal TotalCommitmentTilDate { get; set; }
        public decimal TotalCommitForCurrentYear { get; set; }
        public bool IsYearWise { get; set; }
        public bool IsAllocation { get; set; }
        public decimal SanctionedValue { get; set; }
        public decimal OpeningBalance { get; set; }
        public List<MasterlistviewModel> subheadList { get; set; }
    }
    public class RoleModel
    {
        
        public int Roleid { get; set; }
        [Required]
        [Display(Name = "Role name")]
        public string Rolename { get; set; }
        public int Departmentroleid { get; set; }
        [Required]
        [Display(Name = "Department name")]
        public int Departmentid { get; set; }
        public int sno { get; set; }
        public string Departmentname { get; set; }
        public string Createduser { get; set; }
        public string SearchRole { get; set; }
        public Nullable<int> SearchDepartment { get; set; }
    }
    public class DepartmentModel
    {
        public int Sno { get; set; }
        public int Departmentid { get; set; }
        [Required]
        [Display(Name = "Department name")]
        public string Departmentname { get; set; }
        [Required]
        [Display(Name = "HOD")]
        public string HOD { get; set; }
        public string Createduser { get; set; }
        public string SearchDepartment { get; set; }
        public string SearchHead { get; set; }

    }
    public class Functionviewmodel
    {
        [Required]
        [Display(Name = "Select Any one Function")]
        public int Functionid { get; set; }
        public string Rolename { get; set; }
        public int sno { get; set; }
        public int Roleid { get; set; }
        [Required]
        [Display(Name = "Select Any one Department")]
        public int Departmentid { get; set; }

        public bool Read { get; set; }
        public bool Add { get; set; }
        public bool Delete { get; set; }
        public bool Approve { get; set; }
        public bool Update { get; set; }
    }
    public class Functionlistmodel
    {
        public int Functionid { get; set; }
        public string Functionname { get; set; }
    }
    public class RegisterModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string Firstname { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string Lastname { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        public Nullable<int> Department { get; set; }

        [Required]
        public Nullable<int> RoleId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public Nullable<int> Gender { get; set; }
        [Required]
        [Display(Name = "Expiry Date")]

        public DateTime ExpiryDate { get; set; }
        public string ExpiryDateof { get; set; }
        public int[] SelectedRoles { get; set; }
        [Required]
        [Display(Name = "Institute")]
        public int InstituteId { get; set; }
        public string[] SelectedRolesName { get; set; }
        public int UserId { get; set; }
        [Required]
        [Display(Name = "User type")]
        public int UsertypeId { get; set; }

        public HttpPostedFileBase UserImage { get; set; }
        public string Image { get; set; }
        [Required]
        [Display(Name = "Employee code")]
        public string EMPCode { get; set; }
        [Required]
        [Display(Name = "Designation")]
        public Nullable<int> Designation { get; set; }
        public string Createuser { get; set; }
        public string SearchName { get; set; }
        public int SearchUserId { get; set; }
        public Nullable<int> SearchRoleId { get; set; }
        public Nullable<int> SearchDeptId { get; set; }
    }
    public class UserResultModels
    {
        public int Sno { get; set; }
        public Nullable<int> Userid { get; set; }
        [Display(Name = "First Name")]
        public string Firstname { get; set; }


        [Display(Name = "Last name")]
        public string Lastname { get; set; }


        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address / Username")]
        [Display(Name = "User name / Email")]
        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }


        public Nullable<int> Department { get; set; }


        public Nullable<int> RoleId { get; set; }


        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "Gender")]
        public Nullable<int> Gender { get; set; }

        [Display(Name = "Date Of brith")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> Dateofbirth { get; set; }

        public int[] SelectedRoles { get; set; }



        public int Addtionaldepartment { get; set; }
        public string RoleName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Image { get; set; }
        public string Usertype { get; set; }
    }
    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    
    public class ForgotPasswordModel
    {
        [Required]
        [Display(Name = "User name")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email ID")]
        public string Email { get; set; }

        public string password { get; set; }


    }
    public class ResetPassword
    {
        [Required]
        [Display(Name = "Select Role name")]
        public int Roleid { get; set; }
        public string Rolename { get; set; }
        [Required]
        [Display(Name = "Select User name")]
        public int Userid { get; set; }
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class CreateInstituteModel
    {
        public int Sno { get; set; }
        public int InstituteId { get; set; }
        public string Countryname { get; set; }
        [Required]
        [Display(Name = "Institutename")]
        public string Institutename { get; set; }

        [Required]
        [Display(Name = "Contact first name")]
        [StringLength(250)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Contact last name")]
        [StringLength(150)]
        public string lastName { get; set; }

        [Required]
        [Display(Name = "Contact designation")]
        [StringLength(250)]
        public string contactDES { get; set; }

        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email address")]
        public string Email { get; set; }
        public string Location { get; set; }

        [Required]
        [Display(Name = "Address line1")]
        public string Address1 { get; set; }

        [Required]
        [Display(Name = "Address line2")]
        public string Address2 { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "state")]
        public string State { get; set; }


        public CountryListViewModel Country { get; set; }

        [Required]
        [Display(Name = "Country")]
        public Nullable<int> selCountry { get; set; }

        [Display(Name = "Post code / Zip")]
        [StringLength(25)]
        public string zipCode { get; set; }


        [StringLength(25)]
        public string ContactMobile { get; set; }

        [Display(Name = "Institute logo :")]
        public HttpPostedFileBase logoURL { get; set; }
        public string logo { get; set; }



        public string InstituteCode { get; set; }
    }
    public class UserlistModel
    {
        public int Userid { get; set; }
        public string Username { get; set; }
    }
    public class DashboardModel
    {
       public List<NotificationModel> nofity { get; set; }
       public List<ProcessEngineModel> approveList { get; set; }
       public List<WFProposalModel> wfproposallist { get; set; }

    }
    public class WFProposalModel
    {
        public string ActionLink { get; set; }
        public int WFProposalId { get; set; }
        public string WFProposalNo { get; set; }
        public string PIName { get; set; }
        public string Agency { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Date { get; set; }
        public int  WFid { get; set; }
    }
    public class NotificationModel
    {
        public Int32 NotificationId { get; set; }
        public Int32 ReferenceId { get; set; }
        public string NotificationType { get; set; }
        public string FunctionURL { get; set; }
        public string NotificationDateTime { get; set; }
        public string FromUserName { get; set; }
    }
    public class AgencyModel
    {
        [Required]
        [Display(Name = "Project Type")]
        public int ProjectTypeId { get; set; }
        public Nullable<int> AgencyId { get; set; }
        [Required]
        [Display(Name = "Agency name")]
        public string AgencyName { get; set; }
        //[Required]
        //[Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        //[Required]
        //[Display(Name = "Contact Number")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact number must be Number")]
        public string ContactNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        //[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email address")]
        public string ContactEmail { get; set; }
        //[Required]
        //[Display(Name = "Address")]
        public string Address { get; set; }
        [RequiredIf("AgencycountryCategoryId", 2, ErrorMessage = "state")]
        [Display(Name = "state")]
        public string State { get; set; }
        [RequiredIf("AgencycountryCategoryId", 2, ErrorMessage = "Country")]
        [Display(Name = "country")]
        public Nullable<int> Country { get; set; }
        [Required]
        [Display(Name = "Agency code")]
        [StringLength(4)]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Special characters or Numbers not allowed")]
        public string AgencyCode { get; set; }
        [Required]
        [Display(Name = "Agency type")]
        public int AgencyType { get; set; }
        [Required]
        [Display(Name = "Scheme")]
        public int Scheme { get; set; }
        public int UserId { get; set; }
        public int sno { get; set; }
        [MaxLength(15)]
        //[RegularExpression("^([0]{1}[1-9]{1}|[1-2]{1}[0-9]{1}|[3]{1}[0-7]{1})([a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9a-zA-Z]{1}[zZ]{1}[0-9a-zA-Z]{1})+$", ErrorMessage = "Invalid GST Number")]
        [Display(Name = "GST Number")]
        public string GSTIN { get; set; }

        [MaxLength(10)]
        [RegularExpression("[A-Z]{4}[0-9]{5}[A-Z]{1}", ErrorMessage = "Invalid TAN Number")]
        [Display(Name = "TAN Number")]
        public string TAN { get; set; }
        [MaxLength(10)]
        [RegularExpression("[A-Z]{5}[0-9]{4}[A-Z]{1}", ErrorMessage = "Invalid PAN Number")]
        [Display(Name = "PAN Number")]
        public string PAN { get; set; }
        [RequiredIf("AgencycountryCategoryId", 1, ErrorMessage = "State Name Field")]
        public Nullable<int> StateId { get; set; }
        public string StateCode { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchName { get; set; }
        public string SWIFTCode { get; set; }
        public string MICRCode { get; set; }
        public string IFSCCode { get; set; }
        public string BankAddress { get; set; }
        public string District { get; set; }
        public string CountryName { get; set; }
        public Nullable<int> PinCode { get; set; }
        [Required]
        [Display(Name = "Agency Country type")]
        public int AgencycountryCategoryId { get; set; }
        [RequiredIf("ProjAndCountry", true, ErrorMessage = "Indian Agency Category Type")]
        public Nullable<int> IndianagencyCategoryId { get; set; }
        [RequiredIf("IndianagencyCategoryId", 2, ErrorMessage = "NON SEZ Category type")]
        [Display(Name = "NON SEZ Category type")]
        public Nullable<int> NonSezCategoryId { get; set; }
        //[Required]
        //[Display(Name = "Agency register name")]
        public string AgencyRegisterName { get; set; }
        //[Required]
        //[Display(Name = "Agency register address")]
        public string AgencyRegisterAddress { get; set; }
        public string[] DocPath { get; set; }

        public HttpPostedFileBase[] File { get; set; }
        // public List<AgencyDocumentModel> doucument { get; set; }
        public int[] DocumentType { get; set; }
        public string[] DocumentName { get; set; }
        public string[] AttachName { get; set; }
        public int[] DocumentId { get; set; }
        public string SearchAgencyName { get; set; }
        public string SearchAgencyCode { get; set; }
        public Nullable<int> SearchAgencyCountry { get; set; }
        [Required]
        [Display(Name = "Company Type")]
        public int CompanyId { get; set; }
        public Nullable<int> Ministry { get; set; }
        [NotMapped]
        public bool ProjAndCountry
        {
            get
            {
                return (this.ProjectTypeId == 2 && this.AgencycountryCategoryId == 1);
            }
        }
    }
    //public class AgencyDocumentModel
    //{
    //    public Nullable<int> AgencyDocoumentId { get; set; }
    //    public string AttachmentDocument { get; set; }
    //    public string AgencyDocumentName { get; set; }
    //}
    public class AccountHeadViewModel
    {
        public int AccountHeadId { get; set; }
        [Required]
        [Display(Name = "Account Group Code")]
        public int AccountGroupId { get; set; }
        [Required]
        [Display(Name = "Account Head")]
        public string AccountHead { get; set; }
        [Required]
        [Display(Name = "Account Head Code")]
        public string AccountHeadCode { get; set; }
        public int userid { get; set; }
        public int sno { get; set; }
        public string AccountHeadSearch { get; set; }
        public string AccountHeadCodeSearch { get; set; }
        public Nullable<int> AccountGroupIdSearch { get; set; }

    }
    public class Projectstaffcategorymodel
    {
        public int ProjectstaffcategoryId { get; set; }
        [Required]
        [Display(Name = "Project staff category")]
        public string ProjectstaffCategory { get; set; }
        public int userid { get; set; }
        public int sno { get; set; }
    }
    public class ConsultancyFundingcategorymodel
    {
        public int ConsultancyFundingcategoryid { get; set; }
        [Required]
        [Display(Name = "Consultancy Funding Category")]
        public string ConsultancyFundingcategory { get; set; }
        public int userid { get; set; }
        public int sno { get; set; }
    }
    public class Schemeviewmodel
    {
        public int SchemeId { get; set; }
        [Required]
        [Display(Name = "Scheme Name")]
        public string SchemeName { get; set; }
        [Required]
        [Display(Name = "Project type")]
        public int ProjectType { get; set; }
        [Required]
        [Display(Name = "Sheme code")]
        public string Schemecode { get; set; }
        public int sno { get; set; }
        public int userId { get; set; }
    }
    public class Accountgroupmodel
    {
        public int AccountGroupId { get; set; }
        [MaxLength(50)]
        [Required]

        [Display(Name = "Account group")]
        public string AccountGroup { get; set; }
        public int AccountType { get; set; }
        public string Accounttypename { get; set; }
        
        public string AccountGroupCode { get; set; }
        public int userid { get; set; }
        public int sno { get; set; }
        public int parentgroupId { get; set; }
        public string Parentgroupcode{get;set;}
        public int SeqNbr { get; set; }
        public bool Issubgroup { get; set; }
        public string AccountGroupSearch { get; set; }
        public string AccountGroupCodeSearch { get; set; }
        public Nullable<int> AccountTypeSearch { get; set; }
    }
    public class SRBItemcategory
    {
        public int sno { get; set; }
        public int SRBItemCategotyId { get; set; }
        public string Category { get; set; }
        public bool Asset_f { get; set; }
        public int userid { get; set; }
    }


    //public class ApplicableTDSModel
    //{
    //    public int VendorTDSDetailId { get; set; }
    //    public int VendorId { get; set; }
    //    public string Section { get; set; }
    //    public string NatureOfIncome { get; set; }
    //    public decimal TDSPercentage { get; set; }
    //}


    public class CommitSearchFieldModel
    {
        public Nullable<int> ProjectType { get; set; }
        public Nullable<int> ProjectNumber { get; set; }
        public string ProjectNo { get; set; }
        public string Keyword { get; set; }
        public Nullable<DateTime> FromCreatedDate { get; set; }
        public Nullable<DateTime> ToCreatedDate { get; set; }

        public string SearchINProjectNumber { get; set; }
        public string SearchINCommitmentNumber { get; set; }
    }
    

    public class AllocationDetailsModel
    {
        public int AllocationId { get; set; }
        public Decimal Amount { get; set; }
    }

    public class CommitmentStatusModel
    {
        public CommitSearchFieldModel searchField { get; set; }
        public List<CommitmentResultModel> getDetails { get; set; }
    }

    public class CommitmentUpdateStatusModel
    {
        public int CommitmentId { get; set; }
        public int Action { get; set; }

    }
    
    public class VendorTDSDetails
    {
        public string Section { get; set; }
        public string NatureOfIncome { get; set; }
        public string TDSPercentage { get; set; }
    }
    public class ClearancePaymentAgentMasterViewModel
    {
        public int ClearancePaymentAgentId { get; set; }
        public int Nationality { get; set; }
        public string VendorCode { get; set; }
        public string PFMSVendorCode { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
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
        [RegularExpression("^([0]{1}[1-9]{1}|[1-2]{1}[0-9]{1}|[3]{1}[0-7]{1})([a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9a-zA-Z]{1}[zZ]{1}[0-9a-zA-Z]{1})+$", ErrorMessage = "Invalid GST Number")]
        [Display(Name = "GST Number")]
        public string GSTIN { get; set; }

        public bool GSTExempted { get; set; }
        public string Reason { get; set; }
        public string AccountHolderName { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string IFSCCode { get; set; }
        public string AccountNumber { get; set; }
        public string BankAddress { get; set; }
        public string ABANumber { get; set; }
        public string SortCode { get; set; }
        public string IBAN { get; set; }
        public string SWIFTorBICCode { get; set; }
        public string TypeofService { get; set; }
        public bool ReverseTax { get; set; }
        public bool TDSExcempted { get; set; }
        public string CertificateNumber { get; set; }
        public int ValidityPeriod { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int StateCode { get; set; }
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
        public List<MasterlistviewModel> PONumberList { get; set; }
        public List<MasterlistviewModel> TDSList { get; set; }
    }
    public class CommitmentSearchModel
    {
        public string SearchProjectNumber { get; set; }
        public string SearchCommitmentNumber { get; set; }
        public List<CommitmentResultModel> CommitmentList { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public Nullable<decimal> AvilableAmount { get; set; }
        public string Status { get; set; }
        public int TotalRecords { get; set; }
    }


    public class CommitmentPredicate
    {
        public tblCommitment com { get; set; }
        public tblProject prj { get; set; }
        public tblCommitmentDetails ComDet { get; set; }
    }


    public class UserManualModel
    {
        public string UserName { get; set; }
        public int UserId { get; set; }
        [Required]
        [Display(Name = "Module")]
        public int? ModuleId { get; set; }
        [Required]
        [Display(Name = "Menugroup")]
        public int? MenugroupId { get; set; }
        [Required]
        [Display(Name = "Function")]
        public int? FunctionId { get; set; }
        public int SINo { get; set; }
        public string Module { get; set; }
        public string Menugroup { get; set; }
        public string Function { get; set; }
        public HttpPostedFileBase Attachment { get; set; }
        public string FileName { get; set; }
        public string ActualFileName { get; set; }
        public string FileAddress { get; set; }
        public string UserManualList { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public string Status { get; set; }
    }
}
