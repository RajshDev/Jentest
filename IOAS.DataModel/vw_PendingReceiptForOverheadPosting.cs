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
    
    public partial class vw_PendingReceiptForOverheadPosting
    {
        public string ProjectNumber { get; set; }
        public string ProjectType { get; set; }
        public string ReceiptNumber { get; set; }
        public Nullable<System.DateTime> ReceiptDate { get; set; }
        public Nullable<decimal> ReceiptAmount { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> ReceiptOverheadValue { get; set; }
        public Nullable<decimal> CorpusAdmin { get; set; }
        public Nullable<decimal> ICSROH { get; set; }
        public Nullable<decimal> StaffWelfare { get; set; }
        public Nullable<decimal> PCF { get; set; }
        public Nullable<decimal> RMF { get; set; }
        public Nullable<decimal> DDF { get; set; }
    }
}
