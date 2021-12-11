using IOAS.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models
{
    public class PaymentTypeModel
    {
        public int PaymentTypeId { get; set; }
        public string PaymentType { get; set; }

        public int monthId { get; set; }
        public int year { get; set; }
    }

    public class ProjectCommitmentModel
    {
        public string ProjectNo { get; set; }
        public int PaymentHeadId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public string CommitmentNo { get; set; }
        public decimal SalaryToBePaid { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal BalanceAfter { get; set; }
        public bool IsBalanceAavailable { get; set; }

        public DateTime PaidDate { get; set; }
        public string PaymentMonthYear { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; }
        public bool MakePayment { get; set; }
        public int SlNo { get; set; }
        public Nullable<decimal> CommitmentBalance { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }
    public class AdhocOtherAllowDetailModel
    {
        public Nullable<decimal> TotalFellowshipAmount { get; set; }
        public Nullable<decimal> TotalHonororiumAmount { get; set; }
        public Nullable<decimal> TotalMandaysAmount { get; set; }
        public Nullable<decimal> TotalDistributionAmount { get; set; }
    }
    public class EmployeeDetailsModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string MeetingID { get; set; }
        public string CandidateID { get; set; }
        public bool MakePayment { get; set; }
        public string PaymentMonthYear { get; set; }
        public string PaymentCategory { get; set; }
        public DateTime DOB { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime RelieveDate { get; set; }
        public decimal BasicSalary { get; set; }
        public string PermanentAddress { get; set; }
        public string CommunicationAddress { get; set; }
        public string MobileNumber { get; set; }
        public string EmailID { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string BankAccountNo { get; set; }
        public string IFSC_Code { get; set; }
        public string OutSourcingCompany { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string Remarks { get; set; }
        public int OrderID { get; set; }
        public string OrderType { get; set; }
        public string ProjectNo { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime DetailToDate { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal CostToProject { get; set; }
        public string CommitmentNo { get; set; }
        public int ModeOfPayment { get; set; }

        public int TypeOfPayBill { get; set; }
        public SalaryModel SalaryDetail { get; set; }


    }


    public class SalaryPaymentHead
    {
        public int SlNo { get; set; }
        public int PaymentHeadId { get; set; }
        public string ProjectNo { get; set; }
        public string PaymentNo { get; set; }
        public string CommitmentNo { get; set; }
        public decimal Amount { get; set; }
        public int TypeOfPayBill { get; set; }
        public string TypeOfPayBillText { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime PaidDate { get; set; }

        public string PaidDateText { get; set; }
        public string PaymentMonthYear { get; set; }
        public decimal PaidAmount { get; set; }
        public bool IsPaid { get; set; }
        public string Status { get; set; }
        public string SaveStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public AdhocOtherAllowDetailModel OADetail { get; set; }
        public List<SalaryModel> Salary { get; set; }
        public List<EmployeeDetailsModel> Employees { get; set; }
        public List<AdhocEmployeeModel> AdhocEmployees { get; set; }

        public int AccountHeadId { get; set; }
        public string AccountHead { get; set; }
        public string AccountHeadCode { get; set; }

        public int AccountGroupId { get; set; }
        public string AccountGroup { get; set; }
        public int AccountType { get; set; }
        public string AccountGroupCode { get; set; }
        public bool Pensioner_f { get; set; }
    }

    public class SalaryModel
    {
        public int PayrollProDetId { get; set; }
        public int PaymentId { get; set; }
        public string EmployeeId { get; set; }
        public string EmpNo { get; set; }
        public string DOB { get; set; }
        public string ProjectNo { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationCode { get; set; }
        public decimal Basic { get; set; }
        public decimal HRA { get; set; }
        public decimal MA { get; set; }
        public decimal ActualBasic { get; set; }
        public decimal ActualHRA { get; set; }
        public decimal ActualMA { get; set; }
        public decimal DA { get; set; }
        public decimal Conveyance { get; set; }
        public decimal Medical { get; set; }
        public decimal InstituteHospital { get; set; }
        public decimal fellowship { get; set; }
        public decimal pension { get; set; }
        public decimal Deduction { get; set; }
        public decimal Tax { get; set; }

        public decimal ProfTax { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal NetSalary { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal AffectableProjExp { get; set; }
        public decimal StaffCommitmentAmount { get; set; }
        public decimal MonthSalary { get; set; }
        public decimal CurrentMonthSalary { get; set; }
        public decimal MonthlyTax { get; set; }
        public decimal TaxPayable { get; set; }
        public decimal Mics_Pay { get; set; }
        public decimal Mics_HRA_Pay { get; set; }
        public decimal Mics_Recovery { get; set; }
        public decimal Medical_Recovery { get; set; }
        public decimal LOP { get; set; }
        public decimal Cess { get; set; }
        public decimal AnnualSalary { get; set; }
        public decimal AnnualTaxableSalary { get; set; }
        public decimal AnnualExemption { get; set; }
        public decimal PreviousPT { get; set; }
        public int ProjectedNoMonths { get; set; }
        public decimal ProjectedSalary { get; set; }
        public decimal PreviousIT { get; set; }
        public decimal PreviousGross { get; set; }
        public bool IsPaid { get; set; }
        public decimal PaidAmount { get; set; }
        public int NoOfMonths { get; set; }
        public DateTime PaidDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CurrentMonth { get; set; }
        public string PaymentCategory { get; set; }
        public string PaymentMonthYear { get; set; }
        public string ModeOfPaymentName { get; set; }
        public string Gender { get; set; }
        public int ModeOfPayment { get; set; }
        public int TypeOfPayBill { get; set; }
        public int NoOfDaysPresent { get; set; }
        public int NoOfDaysDedcution { get; set; }

        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }

        public string paybill_id { get; set; }
        public string paybill_no { get; set; }
        public string Status { get; set; }

        public decimal OtherAllowanceAmount { get; set; }
        public decimal DirectAllowanceAmount { get; set; }
        public Nullable<bool> TaxExempted { get; set; }
        public Nullable<bool> PTExempted { get; set; }
        public List<TaxSlab> taxSlab { get; set; }

        public List<EmpOtherAllowance> OtherAllowance { get; set; }
        public AgencyVerifyEmployeeModel DirectAllowance { get; set; }
        public Nullable<bool> ITElegible_c { get; set; } = true;
        public Nullable<bool> PTElegible_c { get; set; } = true;
    }
    public class MonthOfDayModel
    {
        public string MonthYear { get; set; }
        public int TotalDays { get; set; }
        public int TotalPresentDays { get; set; }
    }
    public class ValidateSalary
    {
        public HttpPostedFileBase template { get; set; }
        public int typeOfPayBill { get; set; }
        public string paymentMonthYear { get; set; }
    }
    public class AdhocEmployeeModel
    {
        public bool MakePayment { get; set; }
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string FileNo { get; set; }
        public string AppointmentType { get; set; }
        public string ProjectTitle { get; set; }
        public string PROJECTNO { get; set; }
        public string PTYPE { get; set; }
        public string shon { get; set; }
        public string NAME { get; set; }
        public string departmentcode { get; set; }
        public string DEPARTMENT { get; set; }
        public string SPON { get; set; }
        public string DesignationCode { get; set; }
        public string Designation { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime ExtensionDate { get; set; }
        public Nullable<DateTime> RelieveDate { get; set; }
        public string Paytype { get; set; }
        public decimal BasicPay { get; set; }
        public Nullable<decimal> NetSalary { get; set; }
        public Nullable<decimal> GrossTotal { get; set; }
        public decimal HRA { get; set; }
        public decimal Medical { get; set; }
        public string CoordinatorCode { get; set; }
        public string phon { get; set; }
        public string CoordinatorName { get; set; }
        public string Community { get; set; }
        public string OldFileno { get; set; }
        public Nullable<DateTime> DOB { get; set; }
        public string FATHER { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string ADDRESS3 { get; set; }
        public string ADDRESS4 { get; set; }
        public string Gender { get; set; }
        public string BloodGroup { get; set; }
        public string RH { get; set; }
        public string PHONE { get; set; }
        public string R_ADDR1 { get; set; }
        public string R_ADDR2 { get; set; }
        public string R_ADDR3 { get; set; }
        public string R_ADDR4 { get; set; }
        public string COURSE { get; set; }
        public string paybill_id { get; set; }
        public string paybill_no { get; set; }
        public string email_id { get; set; }
        public string Pensioner { get; set; }
        public string Qualification { get; set; }
        public string offerId { get; set; }
        public DateTime AppointmentLetterDate { get; set; }
        public string AppointmentLetterNo { get; set; }
        public DateTime DateOfInput { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string username { get; set; }
        public string commitmentNo { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int NoOfDays { get; set; }
        public int TypeOfPayBill { get; set; }
        public SalaryModel SalaryDetail { get; set; }

        public string ModeOfPaymentName { get; set; }
        public int ModeOfPayment { get; set; }
        public int SlNo { get; set; }
        public Nullable<bool> InclusiveMedical { get; set; }
        public string PaymentMonthYear { get; set; }
        public Nullable<Int32> PaymentId { get; set; }
        public Nullable<Int32> PayrollProDetId { get; set; }
        public string Bank { get; set; }
        public string Branch { get; set; }
        public string IFSC { get; set; }
        public string AccountNo { get; set; }
        public string PAN { get; set; }
        public string FinYear { get; set; }
        public string DateOfBirth { get; set; }
        public string ExtenDate { get; set; }
        public string RelDate { get; set; }
        public bool IsNotInMain { get; set; }
    }
    public class AdhocEmployeeDetailModel
    {
        public int PayrollProDetId { get; set; }
        public string FileNo { get; set; }
        public string PROJECTNO { get; set; }
        public string NAME { get; set; }
        public string departmentcode { get; set; }
        public string DEPARTMENT { get; set; }
        public string DesignationCode { get; set; }
        public Nullable<DateTime> AppointmentDate { get; set; }
        public Nullable<DateTime> ExtensionDate { get; set; }
        public Nullable<DateTime> RelieveDate { get; set; }
        public Nullable<decimal> BasicPay { get; set; }
        public Nullable<decimal> HRA { get; set; }
        public Nullable<decimal> Medical { get; set; }
        public Nullable<decimal> currentHra { get; set; }
        public Nullable<decimal> currentMedical { get; set; }
        public Nullable<decimal> currentPay { get; set; }
        public Nullable<decimal> Spl_Allowance { get; set; }
        public Nullable<decimal> Transport_Allowance { get; set; }
        public Nullable<decimal> PF_Revision { get; set; }
        public Nullable<decimal> ESIC_Revision { get; set; }
        public Nullable<decimal> Round_off { get; set; }
        public Nullable<decimal> Arrears { get; set; }
        public Nullable<decimal> OthersPay { get; set; }
        public Nullable<decimal> Contribution_to_PF { get; set; }
        public Nullable<decimal> Recovery { get; set; }
        public Nullable<decimal> OthersDeduction { get; set; }
        public Nullable<decimal> Professional_tax { get; set; }
        public Nullable<decimal> Loss_Of_Pay { get; set; }
        public Nullable<decimal> Medical_Recovery { get; set; }
        public Nullable<decimal> HRA_Arrears { get; set; }
        public Nullable<decimal> HRA_Recovery { get; set; }
        public Nullable<DateTime> DOB { get; set; }
        public string Gender { get; set; }
        
        public string commitmentNo { get; set; }

        public Nullable<bool> InclusiveMedical { get; set; }
        public Nullable<bool> TaxExempted { get; set; }
        public Nullable<bool> PTExempted { get; set; }
        public Nullable<int> NoOfDaysPresent { get; set; }
    }
    public class SalaryTemplate
    {
        public Nullable<int> PayBill { get; set; }
        public string FileNumber { get; set; }
        public string Name { get; set; }
        public string Dept { get; set; }
        public string Designation { get; set; }
        public string DesignationCode { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string BankName { get; set; }
        public string BankAccountNo { get; set; }
        public string IFSC_Code { get; set; }
        public string Branch { get; set; }
        public string Remarks { get; set; }
        public Nullable<DateTime> DOB { get; set; }
        public string PAN { get; set; }
        public Nullable<decimal> PreviouseSalary { get; set; }
        public Nullable<decimal> PreviousPT { get; set; }
        public Nullable<decimal> PreviousIT { get; set; }
        public Nullable<decimal> ProjectedSalary { get; set; }
        public string ProjectNo { get; set; }
        public string CommitmentNo { get; set; }
        public Nullable<decimal> CommitmentBalance { get; set; }
        public Nullable<decimal> Basic { get; set; }
        public Nullable<decimal> MISS_PAY { get; set; }
        public Nullable<decimal> HRA { get; set; }
        public Nullable<decimal> MISS_HRA_PAY { get; set; }
        public Nullable<decimal> MedicalAllowance { get; set; }
        public Nullable<decimal> GrossSalary { get; set; }
        public Nullable<decimal> IncomeTax { get; set; }
        public Nullable<decimal> MISS_REC { get; set; }
        public Nullable<decimal> MedicalInsurance { get; set; }
        public Nullable<decimal> LLP { get; set; }
        public Nullable<decimal> PROF_TAX { get; set; }
        public Nullable<decimal> TotalDeducation { get; set; }
        public Nullable<decimal> NetPay { get; set; }
        public Nullable<decimal> OtherPay { get; set; }
    }
    public class SalaryTransaction : CommonPaymentModel
    {

        public int PaymentHeadId { get; set; }

        public decimal Amount { get; set; }

        public int AccountHeadId { get; set; }
        public string AccountHead { get; set; }
        public string AccountHeadCode { get; set; }

        public int AccountGroupId { get; set; }
        public string AccountGroup { get; set; }
        public int AccountType { get; set; }
        public string AccountGroupCode { get; set; }
        public int TypeOfPayBill { get; set; }
        public string TypeOfPayBillText { get; set; }
        public DateTime PaidDate { get; set; }
        public string PaymentMonthYear { get; set; }
        public string ProjectNo { get; set; }
        public string PaymentNo { get; set; }

        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }

        public int TransactionId { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime ApprovedDate { get; set; }
        public int SalaryType { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        //public decimal CommitmentAmount { get; set; }
        //public decimal ExpenseAmount { get; set; }
        //public decimal DeductionAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int CheckListVerifiedBy { get; set; }
        public int PaymentType { get; set; }

        public List<SalaryTransactionDetail> detail { get; set; }

    }

    public class SalaryTransactionDetail
    {
        public int TransactionDetailId { get; set; }
        public int TransactionId { get; set; }
        public int PaymentHeadId { get; set; }
        public int AccountGroupId { get; set; }
        public int AccountHeadId { get; set; }
        public string PaymentNo { get; set; }
        public DateTime PostedDate { get; set; }

        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }
        public int SalaryType { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int CheckListVerifiedBy { get; set; }
        public int PaymentType { get; set; }
    }

    public class TaxSlab
    {
        public int id { get; set; }
        public decimal RangeFrom { get; set; }
        public decimal RangeTo { get; set; }
        public decimal Percentage { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class EmpOtherAllowance
    {
        public int id { get; set; }
        public string EmployeeId { get; set; }
        public string ComponentName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime AddedDate { get; set; }
        public bool deduction { get; set; }
        public string Status { get; set; }
        public bool IsPaid { get; set; }
        public bool taxable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }
    public class EmployeeDepartmentModel
    {
        public string Code { get; set; }
        public string Department { get; set; }
    }
    public class EmpOfficeOrderModel
    {
        public string FileNo { get; set; }
        public string PaybillNo { get; set; }
        public string OrderType { get; set; }
        public string ProjectNo { get; set; }
        public string Designation { get; set; }
        public string Crt_TS { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string RelieveDate { get; set; }
        public Nullable<decimal> Basic { get; set; }
        public Nullable<decimal> HRA { get; set; }
        public Nullable<decimal> MA { get; set; }
    }
    #region Agency Salary
    public class AgencySalaryModel : CommonPaymentModel
    {
        public int SlNo { get; set; }
        public Nullable<int> AgencySalaryID { get; set; }
        public string PaymentNo { get; set; }
        public string MonthYear { get; set; }
        public string DateOfPayment { get; set; }
        public int TotalEmployee { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> TotalFellowshipAmount { get; set; }
        public Nullable<decimal> TotalHonororiumAmount { get; set; }
        public Nullable<decimal> TotalMandaysAmount { get; set; }
        public Nullable<decimal> TotalDistributionAmount { get; set; }
        public Nullable<decimal> NetAmount { get; set; }
        public Nullable<decimal> CGST { get; set; }
        public Nullable<decimal> SGST { get; set; }
        public Nullable<decimal> GST { get; set; }
        public Nullable<decimal> ServiceCharge { get; set; }
        public Nullable<decimal> NetPayable { get; set; }
        public Nullable<decimal> NetCommitmentAmount { get; set; }
        public string Status { get; set; }
        public AgencySearchFieldModel Search { get; set; }

        public PagedData<AgencyStaffDetailsModel> EmployeeDetails { get; set; }
        public PagedData<AgencyStaffDetailsModel> VerifiedDetails { get; set; }
    }

    public class AgencyStaffDetailsModel
    {
        public Nullable<int> PayrollDetailId { get; set; }
        public Nullable<int> AgencySalaryID { get; set; }
        public Nullable<int> VerifiedSalaryId { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public string EmployeeId { get; set; }
        public int SlNo { get; set; }
        public string Name { get; set; }
        public string VerifiedBy { get; set; }
        public string Designation { get; set; }
        public Nullable<decimal> BasicSalary { get; set; }
        public Nullable<decimal> NetSalary { get; set; }
        public bool IsVerified { get; set; }
        public bool AddNew_f { get; set; }
        public Nullable<decimal> HRA { get; set; }
        public Nullable<decimal> Bonus { get; set; }
        public Nullable<decimal> SpecialAllowance { get; set; }
        public Nullable<decimal> PF { get; set; }
        public Nullable<decimal> ESI { get; set; }

        public Nullable<decimal> EmployerPF { get; set; }
        public Nullable<decimal> EmployerESI { get; set; }
        public Nullable<decimal> Insurance { get; set; }
        public Nullable<decimal> EmployerContribution { get; set; }
        public Nullable<decimal> IncomeTax { get; set; }
        public Nullable<decimal> GrossSalary { get; set; }
        public Nullable<decimal> GrossTotal { get; set; }
        public Nullable<decimal> TotalDeduction { get; set; }
        public Nullable<decimal> OtherExpTotal { get; set; }
        public string ProjectNo { get; set; }
        public string CommitmentNo { get; set; }
        public string MonthandYear { get; set; }
        public string Remarks { get; set; }
        public Nullable<decimal> MA { get; set; }
        public Nullable<decimal> MA2 { get; set; }
        public Nullable<DateTime> RelieveDate { get; set; }
        public Nullable<DateTime> ExtensionDate { get; set; }
        public Nullable<decimal> MiscPay { get; set; }
        public Nullable<decimal> MiscRecovery { get; set; }
        public Nullable<decimal> MedicalRecovery { get; set; }
        public Nullable<decimal> LLP { get; set; }
        public Nullable<decimal> OtherPay { get; set; }
        public List<SalaryBreakUpDetailsModel> breakUpDetail { get; set; }
        public List<tblEmpOtherAllowance> otherDetail { get; set; }
    }
    public class SalaryBreakUpDetailsModel
    {
        public Nullable<int> TypeId { get; set; }
        public Nullable<int> HeadId { get; set; }
        public string Type { get; set; }
        public string Head { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Remarks { get; set; }
        public bool IsTaxable { get; set; }
        public Nullable<bool> IsAffectProjectExp { get; set; }
        public List<MasterlistviewModel> HeadList { get; set; }
    }
    public class AgencySearchFieldModel
    {
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string PaymentNo { get; set; }
    }
    public class AgencyVerifyEmployeeModel
    {
        public string EmployeeId { get; set; }
        public int AgencySalaryID { get; set; }
        public int PayrollDetailId { get; set; }
        public string MonthYear { get; set; }
        public List<SalaryBreakUpDetailsModel> buDetail { get; set; }
    }
    #endregion
}