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
    
    public partial class vw_AdvanceOutstandingReport
    {
        public string AdvanceType { get; set; }
        public string AdvanceReferenceNumber { get; set; }
        public string POnumber { get; set; }
        public Nullable<System.DateTime> PoDate { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectType { get; set; }
        public string PartyName { get; set; }
        public Nullable<decimal> POValue { get; set; }
        public Nullable<decimal> AmountPaid { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<decimal> PendingAmount { get; set; }
        public Nullable<int> Ageing { get; set; }
        public Nullable<decimal> AdvanceValue { get; set; }
    }
}
