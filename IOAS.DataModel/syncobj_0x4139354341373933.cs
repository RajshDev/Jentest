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
    
    public partial class syncobj_0x4139354341373933
    {
        public int CreditNoteId { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public string CreditNoteNumber { get; set; }
        public Nullable<decimal> TotalCreditAmount { get; set; }
        public Nullable<decimal> CreditAmount { get; set; }
        public Nullable<decimal> CGST { get; set; }
        public Nullable<decimal> CGSTPercentage { get; set; }
        public Nullable<decimal> SGST { get; set; }
        public Nullable<decimal> SGSTPercentage { get; set; }
        public Nullable<decimal> IGST { get; set; }
        public Nullable<decimal> IGSTPercentage { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> Reason { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public string TransactionTypeCode { get; set; }
        public Nullable<int> SourceReferenceNumber { get; set; }
        public Nullable<System.DateTime> SourceEmailDate { get; set; }
        public Nullable<int> Source { get; set; }
        public Nullable<decimal> InvoiceValueinForeignCurrency { get; set; }
        public Nullable<int> CurrencyId { get; set; }
    }
}
