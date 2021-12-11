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
    
    public partial class tblTDSDetails
    {
        public int TDSDetailsId { get; set; }
        public Nullable<System.DateTime> PostedDate { get; set; }
        public string FunctionName { get; set; }
        public string ReferenceNumber { get; set; }
        public string Name { get; set; }
        public string PAN { get; set; }
        public string BillStatus { get; set; }
        public string CreditBankName { get; set; }
        public Nullable<int> BoaTransactionId { get; set; }
        public Nullable<int> BoaId { get; set; }
        public string RefTransactionCode { get; set; }
        public string RefNumber { get; set; }
        public Nullable<int> AccountHeadId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string TransactionType { get; set; }
        public Nullable<int> BankHeadId { get; set; }
        public Nullable<int> Detailid { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        public Nullable<decimal> Basic { get; set; }
    }
}
