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
    
    public partial class vw_ClarifiedTransactions
    {
        public string successMethod { get; set; }
        public int ProcessTransactionId { get; set; }
        public string RefNumber { get; set; }
        public string Comments { get; set; }
        public string ClarifierName { get; set; }
        public Nullable<System.DateTime> TransactionTS { get; set; }
        public string InitiatorName { get; set; }
        public string DepartmentName { get; set; }
        public string aActionStatus { get; set; }
        public string bActionStatus { get; set; }
        public string ActionLink { get; set; }
        public Nullable<int> aApproverid { get; set; }
        public Nullable<int> bApproverid { get; set; }
        public Nullable<int> RefId { get; set; }
    }
}