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
    
    public partial class tblProjectROSummary
    {
        public int RO_Id { get; set; }
        public int ProjectId { get; set; }
        public string RO_Number { get; set; }
        public decimal RO_ProjectValue { get; set; }
        public decimal RO_InvoiceValue { get; set; }
        public decimal RO_ReceiptValue { get; set; }
        public decimal RO_CommitmentValue { get; set; }
        public decimal RO_ExpenditureValue { get; set; }
        public decimal RO_BalanceValue { get; set; }
        public string RO_Status { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Deleted { get; set; }
        public bool Is_TempRO { get; set; }
        public System.DateTime Crtd_TS { get; set; }
        public int Crtd_UserId { get; set; }
        public Nullable<System.DateTime> Uptd_TS { get; set; }
        public Nullable<int> Uptd_UserId { get; set; }
        public Nullable<int> RO_ProjectApprovalId { get; set; }
    }
}
