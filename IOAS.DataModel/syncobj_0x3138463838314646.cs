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
    
    public partial class syncobj_0x3138463838314646
    {
        public int Id { get; set; }
        public Nullable<int> LCOpeningId { get; set; }
        public string DraftLCNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public Nullable<int> AmendmentSequenceNumber { get; set; }
        public Nullable<System.DateTime> AmmendmentDate { get; set; }
        public string BankReferenceNumber { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public string Status { get; set; }
        public Nullable<int> CheckListVerifiedBy { get; set; }
        public string TransactionTypeCode { get; set; }
    }
}
