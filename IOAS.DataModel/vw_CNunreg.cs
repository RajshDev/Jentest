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
    
    public partial class vw_CNunreg
    {
        public string URType { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string CreditNoteNumber { get; set; }
        public Nullable<System.DateTime> CreditNoteDate { get; set; }
        public string DocumentType { get; set; }
        public string PlaceOfSupply { get; set; }
        public Nullable<decimal> RefundVoucherValue { get; set; }
        public string ApplicableofTaxRate { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Taxablevalue { get; set; }
        public string CessAmount { get; set; }
        public string PreGST { get; set; }
        public Nullable<decimal> CGST { get; set; }
        public Nullable<decimal> SGST { get; set; }
        public Nullable<decimal> IGST { get; set; }
    }
}
