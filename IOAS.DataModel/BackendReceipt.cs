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
    
    public partial class BackendReceipt
    {
        public Nullable<int> ClassificationOfReceipt { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> ModeOfReceipt { get; set; }
        public Nullable<int> OHPId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> BankId { get; set; }
        public Nullable<bool> IsProject { get; set; }
        public int Id { get; set; }
        public Nullable<int> ReturnReceiptId { get; set; }
        public Nullable<int> PaymentId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<System.DateTime> UpdtTS { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Percent { get; set; }
        public Nullable<int> PIId { get; set; }
    }
}
