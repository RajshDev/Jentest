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
    
    public partial class tblReceiptOB
    {
        public int ReceiptOBId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<decimal> ReceiptOpeningBal { get; set; }
        public Nullable<decimal> ReceiptOpeningBalExclInterest { get; set; }
        public Nullable<decimal> InterestReceiptOpeningBal { get; set; }
    }
}
