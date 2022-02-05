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
    
    public partial class syncobj_0x4537383541394342
    {
        public int BillId { get; set; }
        public string TransactionTypeCode { get; set; }
        public Nullable<int> SourceReferenceNumber { get; set; }
        public Nullable<System.DateTime> SourceEmailDate { get; set; }
        public Nullable<int> Source { get; set; }
        public string BillNumber { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<System.DateTime> PODate { get; set; }
        public string PONumber { get; set; }
        public Nullable<decimal> AdvancePercentage { get; set; }
        public Nullable<bool> EligibleForOffset_f { get; set; }
        public Nullable<bool> PartiallyEligibleForOffset_f { get; set; }
        public Nullable<int> BillType { get; set; }
        public Nullable<decimal> BillAmount { get; set; }
        public Nullable<decimal> BillTaxAmount { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public Nullable<decimal> ExpenseAmount { get; set; }
        public Nullable<decimal> DeductionAmount { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public string Status { get; set; }
        public Nullable<int> CheckListVerifiedBy { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<int> PaymentType { get; set; }
        public Nullable<bool> PartAdvance_f { get; set; }
        public Nullable<bool> InclusiveOfTax_f { get; set; }
        public Nullable<decimal> InvoiceAmount { get; set; }
        public Nullable<decimal> InvoiceTaxAmount { get; set; }
        public string SubCode { get; set; }
        public Nullable<bool> Pmt_f { get; set; }
        public Nullable<int> BankHead { get; set; }
        public Nullable<bool> ICSROH_f { get; set; }
        public string Remarks { get; set; }
        public Nullable<decimal> TDSGSTBasicAmt { get; set; }
        public Nullable<int> POType { get; set; }
        public Nullable<decimal> BankGuaranteeAmount { get; set; }
        public string BankGuaranteeRemarks { get; set; }
        public Nullable<bool> RCM_f { get; set; }
    }
}
