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
    
    public partial class tblGeneralVoucher
    {
        public int GeneralVoucherId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> BankHeadId { get; set; }
        public Nullable<decimal> BankTxAmount { get; set; }
        public string VoucherNumber { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public string Status { get; set; }
        public string TransactionTypeCode { get; set; }
        public Nullable<decimal> PaymentTDSAmount { get; set; }
        public Nullable<bool> Pmt_f { get; set; }
        public Nullable<bool> Payee_f { get; set; }
        public Nullable<int> BankID { get; set; }
        public Nullable<System.DateTime> SourceEmailDate { get; set; }
        public Nullable<int> Source { get; set; }
        public Nullable<int> SourceReferenceNumber { get; set; }
    }
}
