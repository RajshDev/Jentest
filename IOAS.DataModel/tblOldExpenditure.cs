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
    
    public partial class tblOldExpenditure
    {
        public int id { get; set; }
        public string RefNbr { get; set; }
        public string ProjectNumber { get; set; }
        public string CommitmentNumber { get; set; }
        public string Head { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public Nullable<decimal> TDS { get; set; }
        public Nullable<decimal> NetPayment { get; set; }
        public string PayeeName { get; set; }
        public Nullable<int> PIInstituteID { get; set; }
        public Nullable<int> Headid { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<System.DateTime> ExpenditureDate { get; set; }
        public Nullable<int> Finyear { get; set; }
    }
}
