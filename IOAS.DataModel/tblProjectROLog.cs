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
    
    public partial class tblProjectROLog
    {
        public int RO_LogId { get; set; }
        public Nullable<int> RO_Id { get; set; }
        public Nullable<decimal> RO_ExistingValue { get; set; }
        public Nullable<decimal> RO_AddEditValue { get; set; }
        public Nullable<decimal> RO_NewValue { get; set; }
        public string RO_LogStatus { get; set; }
        public Nullable<bool> Is_Deleted { get; set; }
        public System.DateTime Crtd_TS { get; set; }
        public int Crtd_UserId { get; set; }
        public Nullable<System.DateTime> Uptd_TS { get; set; }
        public Nullable<int> Uptd_UserId { get; set; }
    
        public virtual tblProjectROSummary tblProjectROSummary { get; set; }
    }
}
