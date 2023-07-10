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
    
    public partial class tblSummerInternshipStudentDetails
    {
        public int SummerInternshipStudentId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string StudentName { get; set; }
        public string SummerInternshipNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string RollNo { get; set; }
        public string Course { get; set; }
        public Nullable<int> YearofStudy { get; set; }
        public string CollegeName { get; set; }
        public Nullable<System.DateTime> InternStartDate { get; set; }
        public Nullable<System.DateTime> InternCloseDate { get; set; }
        public string Duration { get; set; }
        public Nullable<int> SourceReferenceNumber { get; set; }
        public Nullable<System.DateTime> SourceEmailDate { get; set; }
        public Nullable<int> Source { get; set; }
        public Nullable<decimal> StipendAmountperMonth { get; set; }
        public Nullable<decimal> TotalStipendAmount { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> CommitmentId { get; set; }
        public string TransactionTypeCode { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchName { get; set; }
        public string IFSCCode { get; set; }
        public Nullable<bool> IsPendingPayment_f { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public string Status { get; set; }
        public Nullable<int> CheckListVerifiedBy { get; set; }
        public Nullable<bool> Pmt_f { get; set; }
        public Nullable<int> BankID { get; set; }
    }
}
