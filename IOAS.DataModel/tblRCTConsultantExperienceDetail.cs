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
    
    public partial class tblRCTConsultantExperienceDetail
    {
        public int ConsultantExperienceDetailId { get; set; }
        public Nullable<int> ConsultantAppointmentId { get; set; }
        public Nullable<int> TypeId { get; set; }
        public string Organisation { get; set; }
        public string Designation { get; set; }
        public Nullable<System.DateTime> FromYear { get; set; }
        public Nullable<System.DateTime> ToYear { get; set; }
        public Nullable<decimal> SalaryDrawn { get; set; }
        public string DocumentFile { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> CrtdUser { get; set; }
        public Nullable<System.DateTime> CrtdTs { get; set; }
        public Nullable<int> UptdUser { get; set; }
        public Nullable<System.DateTime> UptdTs { get; set; }
        public string Status { get; set; }
        public string DocumentName { get; set; }
        public Nullable<int> DesignationId { get; set; }
    }
}
