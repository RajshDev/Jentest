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
    
    public partial class syncobj_0x3443383431353432
    {
        public int TransactionDetailId { get; set; }
        public int TransactionId { get; set; }
        public int PaymentHeadId { get; set; }
        public Nullable<int> AccountGroupId { get; set; }
        public Nullable<int> AccountHeadId { get; set; }
        public string PaymentNo { get; set; }
        public Nullable<System.DateTime> PostedDate { get; set; }
        public Nullable<int> SalaryType { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string TransactionType { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> CheckListVerifiedBy { get; set; }
        public Nullable<int> PaymentType { get; set; }
    }
}