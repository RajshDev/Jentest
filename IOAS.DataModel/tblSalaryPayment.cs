//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IOAS.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblSalaryPayment
    {
        public int PaymentId { get; set; }
        public Nullable<int> PaymentHeadId { get; set; }
        public string ProjectNo { get; set; }
        public string EmployeeId { get; set; }
        public string EmpNo { get; set; }
        public Nullable<decimal> Basic { get; set; }
        public Nullable<decimal> HRA { get; set; }
        public Nullable<decimal> MA { get; set; }
        public Nullable<decimal> DA { get; set; }
        public Nullable<decimal> Conveyance { get; set; }
        public Nullable<decimal> Deduction { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> ProfTax { get; set; }
        public Nullable<decimal> TaxableIncome { get; set; }
        public Nullable<decimal> NetSalary { get; set; }
        public Nullable<decimal> MonthSalary { get; set; }
        public Nullable<decimal> MonthlyTax { get; set; }
        public Nullable<decimal> AnnualSalary { get; set; }
        public Nullable<decimal> AnnualExemption { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public string PaymentMonthYear { get; set; }
        public string PaymentCategory { get; set; }
        public Nullable<int> ModeOfPayment { get; set; }
        public Nullable<int> TypeOfPayBill { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<bool> IsPaid { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string PayBillId { get; set; }
        public string PayBillNo { get; set; }
        public string Paytype { get; set; }
        public string Status { get; set; }
        public string CommitmentNo { get; set; }
        public Nullable<decimal> OtherAllowance { get; set; }
        public Nullable<decimal> CurrentMonthSalary { get; set; }
        public Nullable<decimal> DirectAllowance { get; set; }
        public Nullable<decimal> MedicalRecovery { get; set; }
        public string PayBill { get; set; }
        public Nullable<decimal> GrossTotal { get; set; }
        public Nullable<decimal> InstituteHospital { get; set; }
        public Nullable<bool> TaxExempted { get; set; }
        public string Remarks { get; set; }
        public Nullable<decimal> AnnualTaxableSalary { get; set; }
        public Nullable<bool> PTExempted { get; set; }
        public Nullable<int> RCTPayrollProcessDetailId { get; set; }
        public string OldPayBill { get; set; }
        public string PrePayBill { get; set; }
    }
}
