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
    
    public partial class syncobj_0x3338434443434441
    {
        public int ImprestBankStatementRecId { get; set; }
        public Nullable<int> ImprestBankStatementDetailId { get; set; }
        public Nullable<int> ImprestBillBookingId { get; set; }
        public Nullable<decimal> PendingAdjustment { get; set; }
        public Nullable<decimal> AdjustedValue { get; set; }
        public string Status { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
    }
}
