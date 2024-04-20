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
    
    public partial class vw_InvoiceReport
    {
        public string InvoiceNumber { get; set; }
        public string InvType { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string InvoiceType { get; set; }
        public string ProjectNumber { get; set; }
        public string Projecttype { get; set; }
        public string ProjectClassification { get; set; }
        public string ProjectSubType { get; set; }
        public string PIName { get; set; }
        public string TaxCode { get; set; }
        public string DescriptionofServices { get; set; }
        public Nullable<decimal> TaxableValue { get; set; }
        public Nullable<decimal> TotalInvoiceValue { get; set; }
        public string AgencyRegName { get; set; }
        public string AgencyRegGSTIN { get; set; }
        public string AgencyRegPAN { get; set; }
        public string AgencyRegTAN { get; set; }
        public Nullable<System.DateTime> InvoiceCreatedDate { get; set; }
        public string Status { get; set; }
        public string CurrencyCode { get; set; }
        public Nullable<decimal> invoiceValueinForeignCurrency { get; set; }
        public Nullable<decimal> CGSTRate { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> IGSTRate { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public Nullable<decimal> SGSTRate { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> TotalTaxValue { get; set; }
        public string AgencyState { get; set; }
        public Nullable<decimal> OpeningReceiptAmount { get; set; }
        public string AgencyRegCommunicationAddress { get; set; }
        public string ConsultancyType { get; set; }
        public Nullable<int> Pincode { get; set; }
    }
}
