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
    
    public partial class syncobj_0x4430364233334133
    {
        public int BOADraftId { get; set; }
        public string TempVoucherNumber { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> BankTransferTotal { get; set; }
        public Nullable<decimal> ChequeTotal { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public Nullable<int> BankHeadId { get; set; }
        public Nullable<System.DateTime> PostedDate { get; set; }
        public string UTRStatus { get; set; }
    }
}
