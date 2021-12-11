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
    
    public partial class vw_Oustanding
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceType { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string AgencyRegGSTIN { get; set; }
        public string AgencyRegName { get; set; }
        public decimal TaxableValue { get; set; }
        public Nullable<decimal> Taxvalue { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectType { get; set; }
        public string PIName { get; set; }
        public decimal TotalInvoiceValue { get; set; }
        public string InvoiceStatus { get; set; }
        public Nullable<decimal> ReceiptAmount { get; set; }
        public Nullable<decimal> CreditNoteAmount { get; set; }
        public Nullable<decimal> Outstanding { get; set; }
        public Nullable<decimal> TaxableOutstanding { get; set; }
        public int ProjectId { get; set; }
        public decimal InvoiceINR { get; set; }
        public decimal ReceiptINR { get; set; }
        public decimal ReceiptTax { get; set; }
        public decimal IOASRec { get; set; }
        public decimal IOASTax { get; set; }
        public string CrtdUser { get; set; }
        public decimal SponReceipt { get; set; }
        public decimal CreditNoteAmountINR { get; set; }
        public decimal CreditNoteTaxINR { get; set; }
        public decimal TaxableOutstandingINR { get; set; }
        public string AgencyContactPersonEmail { get; set; }
        public string DescriptionofServices { get; set; }
    }
}
