//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkFlow
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblAdvanceTravelBillDetail
    {
        public int TravelBillDetailId { get; set; }
        public Nullable<int> TravelBillId { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string Place { get; set; }
        public string Purpose { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> TravelFromDate { get; set; }
        public Nullable<System.DateTime> TravelToDate { get; set; }
        public Nullable<int> NoOfTraveller { get; set; }
        public string DocumentActualName { get; set; }
        public string DocumentName { get; set; }
        public Nullable<decimal> TotalPerDiem { get; set; }
        public Nullable<decimal> OtherExpense { get; set; }
        public Nullable<decimal> TotalExpense { get; set; }
        public string Status { get; set; }
        public Nullable<int> UPDT_By { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<System.DateTime> UPDT_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
    }
}
