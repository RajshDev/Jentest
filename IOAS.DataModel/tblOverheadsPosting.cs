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
    
    public partial class tblOverheadsPosting
    {
        public int OverheadsPostingId { get; set; }
        public string OverheadsPostingNumber { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<int> Source { get; set; }
        public Nullable<int> SourceReferenceNumber { get; set; }
        public Nullable<System.DateTime> SourceEmailDate { get; set; }
        public string TransactionTypeCode { get; set; }
        public string Narration { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string ContraStatus { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public string Status { get; set; }
        public string DocumentActualName { get; set; }
        public string DocumentName { get; set; }
        public Nullable<bool> OHPrint_F { get; set; }
        public string Remarks { get; set; }
    }
}
