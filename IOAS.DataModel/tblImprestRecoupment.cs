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
    
    public partial class tblImprestRecoupment
    {
        public int RecoupmentId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<int> ImprestUserDetailsId { get; set; }
        public Nullable<int> ImprestDetailsId { get; set; }
        public string RecoupmentNumber { get; set; }
        public string ImprestCardNumber { get; set; }
        public Nullable<int> ExpenseHead { get; set; }
        public string TransactionTypeCode { get; set; }
        public Nullable<decimal> AllocatedValue { get; set; }
        public Nullable<decimal> ExpenseValue { get; set; }
        public Nullable<decimal> RecoupmentValue { get; set; }
        public Nullable<decimal> BalanceinAdvance { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public string Status { get; set; }
        public Nullable<int> CheckListVerifiedBy { get; set; }
        public Nullable<int> SourceReferenceNumber { get; set; }
        public Nullable<System.DateTime> SourceEmailDate { get; set; }
        public Nullable<int> Source { get; set; }
        public Nullable<decimal> TotalTaxValue { get; set; }
        public Nullable<decimal> TotalEligibleGST { get; set; }
        public Nullable<decimal> NetPayableAmount { get; set; }
        public Nullable<decimal> BillAmount { get; set; }
    }
}
