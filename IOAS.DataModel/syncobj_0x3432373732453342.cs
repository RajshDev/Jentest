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
    
    public partial class syncobj_0x3432373732453342
    {
        public int PaymentId { get; set; }
        public string TransactionTypeCode { get; set; }
        public Nullable<int> ReferenceId { get; set; }
        public string ReferenceNumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public string VoucherNumber { get; set; }
    }
}
