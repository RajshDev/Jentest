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
    
    public partial class tblContractorBillDetails
    {
        public int ContractorBillDetailsId { get; set; }
        public Nullable<int> ContractorBillId { get; set; }
        public string SupportingVocherNo { get; set; }
        public Nullable<System.DateTime> VoucherDate { get; set; }
        public Nullable<decimal> Unit { get; set; }
        public string Description { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> TotalCost { get; set; }
        public string Remarks { get; set; }
    }
}
