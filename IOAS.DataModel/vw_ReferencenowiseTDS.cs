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
    
    public partial class vw_ReferencenowiseTDS
    {
        public long RowNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string Name { get; set; }
        public string TDSHead { get; set; }
        public Nullable<decimal> TDSAmount { get; set; }
        public Nullable<decimal> GSTTDS { get; set; }
        public Nullable<int> PayeeType { get; set; }
        public Nullable<int> PayeeId { get; set; }
        public int RefID { get; set; }
        public Nullable<int> Payeedetailid { get; set; }
        public string Code { get; set; }
    }
}
