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
    
    public partial class vwCommitmentSpentBalance
    {
        public int CommitmentLogId { get; set; }
        public Nullable<int> CommitmentDetailID { get; set; }
        public Nullable<decimal> OldAmount { get; set; }
        public Nullable<decimal> NewAmount { get; set; }
        public string TransactionTypeCode { get; set; }
        public Nullable<int> RefId { get; set; }
        public Nullable<bool> Reject_f { get; set; }
        public Nullable<bool> Reversed_f { get; set; }
        public int ProjectId { get; set; }
        public int CommitmentId { get; set; }
        public Nullable<decimal> AmountSpent { get; set; }
        public Nullable<int> AllocationHeadId { get; set; }
        public string HeadName { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public int YearWise { get; set; }
        public Nullable<bool> Posted_f { get; set; }
        public string CommitmentNumber { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public Nullable<int> ProjectClassification { get; set; }
        public string PurchaseOrder { get; set; }
        public string Subhead { get; set; }
    }
}
