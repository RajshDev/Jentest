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
    
    public partial class tblImprestCloseExpenseDetail
    {
        public int ImprestCloseExpenseDetailId { get; set; }
        public Nullable<int> ImprestDetailsId { get; set; }
        public Nullable<int> ImprestCloseId { get; set; }
        public Nullable<int> AccountGroupId { get; set; }
        public Nullable<int> AccountHeadId { get; set; }
        public string TransactionType { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> UPDT_By { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<System.DateTime> UPDT_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<int> Delete_By { get; set; }
        public string Status { get; set; }
        public Nullable<bool> IsJV_f { get; set; }
        public Nullable<int> ImprestUserId { get; set; }
    }
}
