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
    
    public partial class tblOHReversalDocumentDetails
    {
        public int OHReversalDocumentId { get; set; }
        public Nullable<int> OHReversalId { get; set; }
        public Nullable<int> DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string DocumentActualName { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<int> CRTD_BY { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public Nullable<int> UPTD_BY { get; set; }
    }
}
