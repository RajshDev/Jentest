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
    
    public partial class syncobj_0x3846444130384541
    {
        public int BillPODetailId { get; set; }
        public Nullable<int> BillId { get; set; }
        public Nullable<int> TypeOfServiceOrCategory { get; set; }
        public string Description { get; set; }
        public Nullable<int> UOM { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> AdvanceAmount { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public string Status { get; set; }
        public Nullable<int> UPDT_By { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<System.DateTime> UPDT_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<int> Delete_By { get; set; }
        public string ItemName { get; set; }
        public Nullable<bool> IsTaxEligible { get; set; }
        public Nullable<decimal> TaxablePercentage { get; set; }
    }
}