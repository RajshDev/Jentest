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
    
    public partial class syncobj_0x3833323137303941
    {
        public int BRSBOADetailId { get; set; }
        public Nullable<int> BOAPaymentDetailId { get; set; }
        public string ReferenceNumber { get; set; }
        public Nullable<System.DateTime> BankTxDate { get; set; }
        public Nullable<int> BRSId { get; set; }
        public string Remarks { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string TransactionType { get; set; }
        public Nullable<int> Reason { get; set; }
        public Nullable<bool> Reconciliation_f { get; set; }
        public Nullable<System.DateTime> VoucherDate { get; set; }
    }
}