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
    
    public partial class syncobj_0x3934464342303637
    {
        public int BOAInvoiceDetailId { get; set; }
        public string VendorName { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<decimal> TaxableAmount { get; set; }
        public Nullable<decimal> TaxPct { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string GSTIN { get; set; }
        public Nullable<bool> TaxEligible { get; set; }
        public Nullable<bool> Interstate { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public Nullable<int> RefId { get; set; }
        public string ReferenceNumber { get; set; }
        public string TransactionTypeCode { get; set; }
    }
}
