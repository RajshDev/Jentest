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
    
    public partial class tblCommitmentAddandCloseLog
    {
        public int LogId { get; set; }
        public Nullable<int> LogType { get; set; }
        public Nullable<int> CommitmentId { get; set; }
        public Nullable<int> CommitmentDetailId { get; set; }
        public Nullable<decimal> PrevCommitmentAmount { get; set; }
        public Nullable<decimal> PrevCommtBalanceAmount { get; set; }
        public Nullable<decimal> PrevCommtDetBalanceAmount { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> HeadId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> Reason { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> CRTD_BY { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<int> UPTD_BY { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public Nullable<int> BankID { get; set; }
        public Nullable<int> Pre_BankID { get; set; }
    }
}
