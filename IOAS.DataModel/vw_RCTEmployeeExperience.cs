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
    
    public partial class vw_RCTEmployeeExperience
    {
        public int ExperienceId { get; set; }
        public string EmployeeId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public string AppointmentType { get; set; }
        public Nullable<System.DateTime> EffectiveFrom { get; set; }
        public Nullable<System.DateTime> EffectiveTo { get; set; }
        public string OrderType { get; set; }
        public Nullable<decimal> Basic { get; set; }
        public Nullable<decimal> HRA { get; set; }
        public string ProjectNumber { get; set; }
        public string Designation { get; set; }
        public Nullable<int> OrderId { get; set; }
    }
}
