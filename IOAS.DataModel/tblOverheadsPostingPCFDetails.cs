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
    
    public partial class tblOverheadsPostingPCFDetails
    {
        public int Id { get; set; }
        public Nullable<int> ReceiptId { get; set; }
        public Nullable<int> OverheadsPostingId { get; set; }
        public string ReceiptNumber { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<int> PCFProjectID { get; set; }
        public Nullable<int> PCFBankId { get; set; }
        public Nullable<decimal> PCFAmount { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public Nullable<int> ReturnReceiptId { get; set; }
        public Nullable<decimal> PCFPercent { get; set; }
    }
}
