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
    
    public partial class tblDateLog
    {
        public int DateLogId { get; set; }
        public Nullable<System.DateTime> CommitmentLogDate { get; set; }
        public Nullable<System.DateTime> BillDate { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<System.DateTime> BoaDate { get; set; }
        public Nullable<System.DateTime> BoaExpenditureDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> CRTD_UserId { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public string TransactionTypeCode { get; set; }
        public string ReferenceNumber { get; set; }
        public Nullable<int> RefId { get; set; }
        public Nullable<System.DateTime> ModifiedCommLogdate { get; set; }
        public Nullable<System.DateTime> modifiedBoadate { get; set; }
        public Nullable<System.DateTime> modifiedBoaExpdate { get; set; }
        public string Remarks { get; set; }
    }
}
