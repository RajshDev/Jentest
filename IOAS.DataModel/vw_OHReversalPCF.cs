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
    
    public partial class vw_OHReversalPCF
    {
        public string TransactionType { get; set; }
        public string BeneficiaryIFSCCode { get; set; }
        public string BeneficiaryAccountNo { get; set; }
        public string BeneficiaryName { get; set; }
        public Nullable<int> BeneAddressLine1 { get; set; }
        public Nullable<int> BeneAddressLine2 { get; set; }
        public Nullable<int> BeneAddressLine3 { get; set; }
        public Nullable<int> BeneAddressLine4 { get; set; }
        public string TxnRefNo { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string SendertoReceiverInfo { get; set; }
        public Nullable<int> AddInfo1 { get; set; }
        public Nullable<int> AddInfo2 { get; set; }
        public Nullable<int> AddInfo3 { get; set; }
        public Nullable<int> AddInfo4 { get; set; }
    }
}
