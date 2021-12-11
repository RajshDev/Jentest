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
    
    public partial class tblSettlementTravelBill
    {
        public int TravelBillId { get; set; }
        public Nullable<int> TravelType { get; set; }
        public Nullable<int> RefTravelBillId { get; set; }
        public string TransactionTypeCode { get; set; }
        public Nullable<int> SourceReferenceNumber { get; set; }
        public Nullable<System.DateTime> SourceEmailDate { get; set; }
        public Nullable<int> Source { get; set; }
        public string BillNumber { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> EstimatedValue { get; set; }
        public Nullable<decimal> AdvanceValue { get; set; }
        public Nullable<decimal> OverallExpense { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> Adv100Pct_f { get; set; }
        public Nullable<bool> ProformaInvoiceSubmit_f { get; set; }
        public Nullable<int> CommitmentId { get; set; }
        public string DeductionType { get; set; }
        public Nullable<bool> EligibilityCheck_f { get; set; }
        public Nullable<bool> ADVSettlement_f { get; set; }
        public Nullable<int> CheckListVerifiedBy { get; set; }
        public Nullable<bool> Pmt_f { get; set; }
        public Nullable<decimal> BalanceinAdvance { get; set; }
        public Nullable<decimal> PaymentValue { get; set; }
        public string SubCode { get; set; }
        public Nullable<int> PI { get; set; }
        public string PIName { get; set; }
        public Nullable<decimal> PaymentTDSAmount { get; set; }
        public Nullable<bool> IOAS_f { get; set; }
        public string WorkflowNumber { get; set; }
    }
}
