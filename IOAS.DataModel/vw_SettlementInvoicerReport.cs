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
    
    public partial class vw_SettlementInvoicerReport
    {
        public string RefNumber { get; set; }
        public string TapalNo { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectType { get; set; }
        public string SchemeCode { get; set; }
        public string CommitmentNumber { get; set; }
        public string BudgetHead { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public string GSTIN { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public Nullable<decimal> TaxPercentage { get; set; }
        public Nullable<decimal> CGST { get; set; }
        public Nullable<decimal> IGST { get; set; }
        public Nullable<decimal> SGST { get; set; }
        public string CreditBankName { get; set; }
        public string PONumber { get; set; }
        public Nullable<System.DateTime> PODate { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public string SettTDSSection { get; set; }
        public Nullable<decimal> SettTDSAmount { get; set; }
        public Nullable<decimal> SettGSTTDSAmount { get; set; }
        public string BillStatus { get; set; }
        public string Paymentstatus { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
