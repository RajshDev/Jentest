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
    
    public partial class vw_TravelpaymentBreakup
    {
        public int TravelBillId { get; set; }
        public string TransactionTypeCode { get; set; }
        public string BillNumber { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public int TravelBillDetailId { get; set; }
        public string Place { get; set; }
        public Nullable<System.DateTime> TripFromDate { get; set; }
        public Nullable<System.DateTime> TripToDate { get; set; }
        public string TravellerName { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string Category { get; set; }
        public string Payeename { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Tapalno { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectType { get; set; }
        public string BatchNumber { get; set; }
        public string BatchStatus { get; set; }
    }
}
