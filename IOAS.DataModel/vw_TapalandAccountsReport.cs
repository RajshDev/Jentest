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
    
    public partial class vw_TapalandAccountsReport
    {
        public int TapalId { get; set; }
        public string TapalNo { get; set; }
        public string Forwardedby { get; set; }
        public Nullable<System.DateTime> OutwardDateTime { get; set; }
        public string Receiver1 { get; set; }
        public Nullable<System.DateTime> Receiver1inwardsdate { get; set; }
        public string FinalReceiver { get; set; }
        public Nullable<System.DateTime> FinalReceiverinwardsdate { get; set; }
        public string BillNumber { get; set; }
        public string BillType { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string BillStatus { get; set; }
        public Nullable<System.DateTime> BillEnteredDate { get; set; }
        public Nullable<System.DateTime> BillApprovedDate { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<int> TapalAcceptedAgeing { get; set; }
        public Nullable<int> BillProcessageing { get; set; }
        public Nullable<int> Billapprovalageing { get; set; }
        public Nullable<int> PaymentProcessageing { get; set; }
        public string BillUsername { get; set; }
        public string FinallevelApprover { get; set; }
        public string PINAME { get; set; }
        public Nullable<System.DateTime> TapalDate { get; set; }
        public int Receiver1Id { get; set; }
        public int Receiver2Id { get; set; }
    }
}
