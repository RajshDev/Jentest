using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models
{
    //public class DesignationModel
    //{

    //    public Nullable<int> DesignationId { get; set; }
    //    [Required]
    //    [Display(Name = "Type Of Appointment")]
    //    public Nullable<int> TypeOfAppointment { get; set; }
    //    [Required]
    //    [Display(Name = "Designation Code")]
    //    [StringLength(3, MinimumLength = 2)]
    //    public string DesignationCode { get; set; }
    //    [Required]
    //    [Display(Name = "Designation")]
    //    public string Designation { get; set; }
    //    [RequiredIf("TypeOfAppointment", 4, ErrorMessage = "Department requried")]
    //    public Nullable<int> DepartmentId { get; set; }
    //    [Required]
    //    [Display(Name = "Pay structure Minimum")]
    //    [LessThanOrEqualTo("PayStructureMaximum", ErrorMessage = "Pay Structure Minimum should not be Greater than Pay Structure Maximum Value.")]
    //    public Nullable<decimal> PayStructureMinMum { get; set; }
    //    [Required]
    //    [Display(Name = "Pay structure Maximum")]
    //    [GreaterThanOrEqualTo("PayStructureMinMum", ErrorMessage = "Pay Structure Maximum should not be less than Pay Structure MinMum Value.")]
    //    public Nullable<decimal> PayStructureMaximum { get; set; }
    //    public string HRA { get; set; }
    //    [RequiredIf("HRA", "Yes", ErrorMessage = "HRA Basic requried")]
    //    [Range(1, 100)]
    //    [Display(Name = "HRA Basic")]
    //    public Nullable<decimal> HRABasic { get; set; }
    //    public string Medical { get; set; }
    //    [RequiredIf("Medical", "Yes", ErrorMessage = "Medical Deduction requried")]
    //    [Range(1, 999)]
    //    [Display(Name = "Medical Deduction")]
    //    public Nullable<decimal> MedicalDeduction { get; set; }
    //    [Required]
    //    [Display(Name = "Age Limit")]
    //    [Range(18, 100)]
    //    public Nullable<int> AgeLimit { get; set; }
    //    [Required]
    //    [Display(Name = "Annual Increment")]

    //    public Nullable<decimal> AnnualIncrement { get; set; }
    //    public Nullable<int> Status { get; set; }
    //    public Nullable<int> UserId { get; set; }
    //    public string ErrorMsg { get; set; }
    //    public string TypeofAccountName { get; set; }
    //    public string HRAType { get; set; }
    //    public string MedicalType { get; set; }
    //    public string StatusType { get; set; }
    //    public Nullable<int> SNo { get; set; }
    //    public string RecordStatus { get; set; }
    //    [Required]
    //    public string GateScore { get; set; }
    //    [Required]
    //    [Display(Name = "Pay Type")]
    //    public string IsConsulandFellowship { get; set; }
    //    public List<DesignationDetailModel> Detail { get; set; } = new List<DesignationDetailModel>();
    //}
    public class DesignationDetailModel
    {
        public Nullable<int> DesignationDetailId { get; set; }

        public Nullable<int> Qualification { get; set; }

        public Nullable<int>[] QualificationCourse { get; set; }

        [Display(Name = "Marks")]
        public Nullable<int> Marks { get; set; }
        public List<MasterlistviewModel> ddlList { get; set; }

        [Display(Name = "RelevantExperience")]
        public Nullable<int> RelevantExperience { get; set; }

        [Display(Name = "CGPA")]
        public Nullable<int> CGPA { get; set; }
    }

    public class SearchdesignationModel
    {
        public string TypeofAccountName { get; set; }
        public string DesignationCode { get; set; }
        public string Designation { get; set; }
        public Nullable<decimal> PayStructureMinMum { get; set; }
        public Nullable<decimal> PayStructureMaximum { get; set; }
        public Nullable<bool> HRA { get; set; }
        public Nullable<bool> Medical { get; set; }
        public Nullable<int> Status { get; set; }
        public List<DesignationModel> Designationlist { get; set; }
        public int TotalRecords { get; set; }

    }
    public class DesignationModel
    {
        public Nullable<int> DesignationId { get; set; }
        [Required]
        [Display(Name = "Type Of Appointment")]
        public Nullable<int> TypeOfAppointment { get; set; }
        [Required]
        [Display(Name = "Designation Code")]
        //[MaxLength(3),MinLength(2)]
        public string DesignationCode { get; set; }
        [Required]
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [RequiredIf("TypeOfAppointment", 4, ErrorMessage = "Department required")]
        public Nullable<int> DepartmentId { get; set; }
        //public bool IsNotValidDesignation { get; set; }
        public string IsNotValidDesignation { get; set; }
        [RequiredIf("IsNotValidDesignation", "No", ErrorMessage = "Pay structure Minimum required")]
        [Display(Name = "Pay structure Minimum")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999.99, ErrorMessage = "Only 9 digits are allowed")]
        [LessThanOrEqualTo("PayStructureMaximum", ErrorMessage = "Pay Structure Minimum should not be Greater than Pay Structure Maximum Value.")]
        public Nullable<decimal> PayStructureMinMum { get; set; }
        [RequiredIf("IsNotValidDesignation", "No", ErrorMessage = "Pay structure Maximum required")]
        [Display(Name = "Pay structure Maximum")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 99999999999.99, ErrorMessage = "Only 11 digits are allowed")]
        [GreaterThanOrEqualTo("PayStructureMinMum", ErrorMessage = "Pay Structure Maximum should not be less than Pay Structure MinMum Value.")]
        public Nullable<decimal> PayStructureMaximum { get; set; }
        public string HRA { get; set; }
        [RequiredIf("HRA", "Yes", ErrorMessage = "HRA Basic required")]
        [Range(1, 100)]
        [Display(Name = "HRA Basic")]
        public Nullable<decimal> HRABasic { get; set; }
        public string Medical { get; set; }
        [RequiredIf("Medical", "Yes", ErrorMessage = "Medical Deduction requried")]
        [Range(1, 999)]
        [Display(Name = "Medical Deduction")]
        public Nullable<decimal> MedicalDeduction { get; set; }
        [RequiredIf("IsNotValidDesignation", "No", ErrorMessage = "Age Limit required")]
        [Display(Name = "Age Limit")]
        [Range(18, 100)]
        public Nullable<int> AgeLimit { get; set; }
        [RequiredIf("IsNotValidDesignation", "No", ErrorMessage = "Annual Increment required")]
        [Display(Name = "Annual Increment")]
        public Nullable<decimal> AnnualIncrement { get; set; }
        [RequiredIf("TypeOfAppointment", 3, ErrorMessage = "SC / ST required")]
        public string IsSCST { get; set; }
        [RequiredIf("IsSCST", "Yes", ErrorMessage = "SC / ST Age Limit requried")]
        [Display(Name = "SC / ST Age Limit")]
        [Range(18, 100)]
        public Nullable<int> SCSTAgeLimit { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> UserId { get; set; }
        public string ErrorMsg { get; set; }
        public string TypeofAccountName { get; set; }
        public string HRAType { get; set; }
        public string MedicalType { get; set; }
        public string StatusType { get; set; }
        public Nullable<int> SNo { get; set; }
        public string RecordStatus { get; set; }
        [Required]
        public string GateScore { get; set; }
        [Required]
        [Display(Name = "Pay Type")]
        public string IsConsulandFellowship { get; set; }
        public List<DesignationDetailModel> Detail { get; set; } = new List<DesignationDetailModel>();
    }

    public class StatutoryModel
    {
        public Nullable<int> StatutoryId { get; set; }
        [Required]
        [Display(Name = "PF Employee Percentage")]
        [Range(0, 100)]
        public Nullable<decimal> PFEmployeePercentage { get; set; }
        [Required]
        [Display(Name = "PF Employer Percentage")]
        [Range(0, 100)]
        public Nullable<decimal> PFEmployerPercentage { get; set; }
        [Required]
        [Display(Name = "PF Amount")]
        public Nullable<decimal> PFEmployeeAmount { get; set; }
        [Required]
        [Display(Name = "ESIC Employee Percentage")]
        [Range(0, 100)]
        public Nullable<decimal> ESICEmployeePercentage { get; set; }
        [Required]
        [Display(Name = "ESIC Employer Percentage")]
        [Range(0, 100)]
        public Nullable<decimal> ESICEmployerPercentage { get; set; }
        [Required]
        [Display(Name = "ESIC Physical Amount")]
        public Nullable<decimal> ESICPhysicalAmount { get; set; }
        [Required]
        [Display(Name = "ESIC General Amount")]
        public Nullable<decimal> ESICGeneralAmount { get; set; }
        [Required]
        [Display(Name = "LWF Employee Contribution")]

        public Nullable<decimal> LWFEmployeeContribution { get; set; }
        [Required]
        [Display(Name = "LWF Employer Contribution")]

        public Nullable<decimal> LWFEmployerContribution { get; set; }
        [Required]
        [Display(Name = "Value Date")]
        public Nullable<DateTime> ValueDate { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> SNo { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string ValueDateType { get; set; }
        public string EndDateType { get; set; }
        public string Status { get; set; }
        public string CurrDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> CurrentDate { get; set; }
    }

    public class SearchStatutoryModel
    {
        public List<StatutoryModel> StatutoryList { get; set; }
        public int TotalRecords { get; set; }
    }

    //public class ProfessionalTaxModel
    //{
    //    public Nullable<int> ProfessionalTaxId { get; set; }
    //    [Required]
    //    [Display(Name = "Monthy Salary")]
    //    [Range(0, 100)]
    //    public Nullable<decimal> MonthySalary35 { get; set; }
    //    [Required]
    //    [Display(Name = "Monthy Salary")]
    //    [Range(0, 100)]
    //    public Nullable<decimal> MonthySalary35to5 { get; set; }
    //    [Required]
    //    [Display(Name = "Monthy Salary")]
    //    [Range(0, 100)]
    //    public Nullable<decimal> MonthySalary5to75 { get; set; }
    //    [Required]
    //    [Display(Name = "Monthy Salary")]
    //    [Range(0, 100)]
    //    public Nullable<decimal> MonthySalary75to10 { get; set; }
    //    [Required]
    //    [Display(Name = "Monthy Salary")]
    //    [Range(0, 100)]
    //    public Nullable<decimal> MonthySalary10to12 { get; set; }
    //    [Required]
    //    [Display(Name = "Monthy Salary")]
    //    [Range(0, 100)]
    //    public Nullable<decimal> MonthySalaryAbove12 { get; set; }
    //    [Required]
    //    [Display(Name = "Value Date")]

    //    public Nullable<DateTime> ValueDate { get; set; }
    //    public Nullable<int> UserId { get; set; }
    //    public Nullable<int> SNo { get; set; }
    //    public Nullable<DateTime> EndDate { get; set; }
    //    public string ValueDateType { get; set; }
    //    public string EndDateType { get; set; }
    //    public string Status { get; set; }
    //}

    public class SearchProfessionalTaxModel
    {
        public List<ProfessionalTaxModel> proftaxList { get; set; }
        public int TotalRecords { get; set; }
    }

    #region Committee member AddModel

    public class MemberModel
    {
        public Nullable<int> MemberId { get; set; }

        public string EmployeeName { get; set; }
        [Required]
        [Display(Name = "Employee Name")]
        public Nullable<int> EmployeeId { get; set; }
        [Required]
        [Display(Name = "Member Type")]
        public Nullable<int> MemberType { get; set; }
        public bool validFromDate
        {
            get
            {
                return (this.MemberStatus == "Open" || (string.IsNullOrEmpty(this.MemberStatus)));
            }
        }
        [RequiredIf("validFromDate", true, ErrorMessage = "Effactive Date requried")]
        [Display(Name = "From Date")]
        public Nullable<DateTime> FromDate { get; set; }
        [Required]
        [Display(Name = "To Date")]
        public Nullable<DateTime> ToDate { get; set; }
        public Nullable<int> UserId { get; set; }
        public string MemberTypeName { get; set; }
        public string FrmDate { get; set; }
        public string TostrDate { get; set; }
        public string Status { get; set; }
        public string EmployeeCode { get; set; }
        public Nullable<int> SNo { get; set; }
        public string MemberStatus { get; set; }
        [RequiredIf("MemberStatus", "InActive", ErrorMessage = "Effactive Date requried")]
        public Nullable<DateTime> EffectiveDate { get; set; }
    }

    public class SearchMemberModel
    {
        public string EmployeeName { get; set; }
        public string MemberTypeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Status { get; set; }
        public int TotalRecords { get; set; }
        public List<MemberModel> memlist { get; set; }
    }

    public class MemberViewModel
    {
        public string MemberType { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeDesignation { get; set; }
        public string EmployeeDepartmentCode { get; set; }
        public string EmployeeDepartmentName { get; set; }
        public string EmpEmail { get; set; }
        public string ConNumber { get; set; }
        public string FrmDate { get; set; }
        public string ToStrDate { get; set; }
        public string ActualendDate { get; set; }
    }

    #endregion

    public class ProfessionalTaxModel
    {
        public Nullable<int> ProfessionalTaxId { get; set; }
        [Required]
        [Display(Name = "Monthy Salary")]
        //[Range(0, 100)]
        public Nullable<decimal> MonthySalary35 { get; set; }
        [Required]
        [Display(Name = "Monthy Salary")]
        //[Range(0, 100)]
        public Nullable<decimal> MonthySalary35to5 { get; set; }
        [Required]
        [Display(Name = "Monthy Salary")]
        //[Range(0, 100)]
        public Nullable<decimal> MonthySalary5to75 { get; set; }
        [Required]
        [Display(Name = "Monthy Salary")]
        //[Range(0, 100)]
        public Nullable<decimal> MonthySalary75to10 { get; set; }
        [Required]
        [Display(Name = "Monthy Salary")]
        //[Range(0, 100)]
        public Nullable<decimal> MonthySalary10to12 { get; set; }
        [Required]
        [Display(Name = "Monthy Salary")]
        //[Range(0, 100)]
        public Nullable<decimal> MonthySalaryAbove12 { get; set; }
        [Required]
        [Display(Name = "Value Date")]

        public Nullable<DateTime> ValueDate { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> SNo { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string ValueDateType { get; set; }
        public string EndDateType { get; set; }
        public string Status { get; set; }
    }

    #region OutSourceing AgencyMaster

    public class AgencySalaryMasterModel
    {
        public Nullable<int> SalaryAgencyId { get; set; }
        [Required]
        [Display(Name = "Agency Name")]
        public string AgencyName { get; set; }
        [Required]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        [Required]
        [Display(Name = "Contact Number")]
        [MaxLength(10)]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Contact Number")]
        public string ContactNumber { get; set; }
        [Required]
        [Display(Name = "Contact Email")]
        [EmailAddress]
        public string ContactEmail { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }
        public string AgencyCode { get; set; }
        public string Status { get; set; }
        [Required]
        [MaxLength(15)]
        [RegularExpression("^([0]{1}[1-9]{1}|[1-2]{1}[0-9]{1}|[3]{1}[0-7]{1})([a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9a-zA-Z]{1}[zZ]{1}[0-9a-zA-Z]{1})+$", ErrorMessage = "Invalid GST Number")]
        [Display(Name = "GST Number")]
        public string GSTIN { get; set; }
        [RegularExpression("^([\\w+-.%]+@[\\w-.]+\\.[A-Za-z]{2,6},?)+$", ErrorMessage = "Invalid CC Mail example:abc@mail.com,abx")]
        public string CCMail { get; set; }
        public string TAN { get; set; }
        [Required]
        [MaxLength(10)]
        [RegularExpression("[A-Z|a-z]{5}[0-9]{4}[A-Z|a-z]{1}", ErrorMessage = "Invalid PAN Number")]
        [Display(Name = "PAN Number")]
        public string PAN { get; set; }
        [Required]
        [Display(Name = "State")]
        public Nullable<int> StateId { get; set; }
        [Required]
        [Display(Name = "BankName")]
        public string BankName { get; set; }
        [Required]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }
        [Required]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }
        [Required]
        [MaxLength(11)]
        [RegularExpression("[A-Z|a-z]{4}[0][a-zA-Z0-9]{6}", ErrorMessage = "Invalid IFSC Code")]
        [Display(Name = "IFSC Code")]
        public string IFSCCode { get; set; }
        public string District { get; set; }
        public Nullable<int> PinCode { get; set; }
        public Nullable<int> UserId { get; set; }
        public HttpPostedFileBase Attachment { get; set; }
        [Required]
        [Display(Name = "Agencyfee")]
        [Range(1, 100)]
        public Nullable<decimal> Agencyfee { get; set; }
        [Required]
        [Display(Name = "Insurance")]
        [Range(1, 9999)]
        public Nullable<int> Insurance { get; set; }
        [Required]
        [Display(Name = "GST Percetage")]
        public Nullable<decimal> GSTPercentage { get; set; }
        public string DocumentPath { get; set; }
        public Nullable<int> SNo { get; set; }
        public bool GSTExcepted { get; set; }
        [RequiredIf("GSTExcepted", true, ErrorMessage = "GST Certificate No is Required")]
        public string GSTCertificateNo { get; set; }
        [RequiredIf("GSTExcepted", true, ErrorMessage = "Validity is Required")]
        public Nullable<DateTime> GSTValidity { get; set; }
        public bool TDSExcepted { get; set; }
        [RequiredIf("TDSExcepted", true, ErrorMessage = "TDS Certificate No is Required")]
        public string TDSCertificateNo { get; set; }
        [RequiredIf("TDSExcepted", true, ErrorMessage = "Validity is Required")]
        public Nullable<DateTime> TDSValidity { get; set; }
        public Nullable<DateTime> FinFormDate { get; set; }
        public Nullable<DateTime> FinToDate { get; set; }
        public List<AgencySalaryDocModel> AgencyDocList { get; set; } = new List<AgencySalaryDocModel>();
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public string ModifyDate { get; set; }
    }

    public class AgencySalaryDocModel
    {
        public Nullable<int> DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentFileName { get; set; }
        public string DocumentPath { get; set; }
        public string FormDate { get; set; }
        public string ToDate { get; set; }
        public string DocumentCatecory { get; set; }
        public HttpPostedFileBase Document { get; set; }
    }

    public class AgencySalaryMasterSearchModel
    {
        public string SearchAgencyname { get; set; }
        public string SearchContactperson { get; set; }
        public string SearchGSTINnumber { get; set; }
        public string SearchAgencyCode { get; set; }
        public string SearchStatus { get; set; }
        public Nullable<int> TotalRecords { get; set; }
        public List<AgencySalaryMasterModel> List { get; set; }
    }

    #endregion

    public class OSGSalaryDetailModel
    {
        public string EmployeeNo { get; set; }
        public decimal DifferenceAmount { get; set; }

        [Display(Name = "Employee Type")]
        public string EmpType { get; set; }
        public string EmpName { get; set; }
        public string EmpDesig { get; set; }
        public string PhysicalyHandicaped { get; set; }
        public Nullable<decimal> EmpPFBasicWages { get; set; }
        public Nullable<decimal> EmployeePF { get; set; }
        public Nullable<decimal> EmployeeESIC { get; set; }
        public Nullable<decimal> RecommendedSalary { get; set; }
        public Nullable<decimal> EmployeeProfessionalTax { get; set; }
        public Nullable<decimal> EmployeeTtlDeduct { get; set; }
        public Nullable<decimal> EmployeeNetSalary { get; set; }
        public Nullable<decimal> EmployerPF { get; set; }
        public Nullable<decimal> EmployerIns { get; set; }
        public Nullable<decimal> EmployerESIC { get; set; }
        public Nullable<decimal> EmployerTtlContribute { get; set; }
        public Nullable<decimal> EmployeeCTC { get; set; }
        public Nullable<decimal> AgencyFee { get; set; }
        public Nullable<decimal> SalaryGST { get; set; }
        public Nullable<decimal> CTCwithAgencyFee { get; set; }
        public Nullable<decimal> TotalCTC { get; set; }
        public Nullable<decimal> LWFAmount { get; set; }
    }

}