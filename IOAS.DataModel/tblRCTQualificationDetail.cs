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
    
    public partial class tblRCTQualificationDetail
    {
        public int QualificationDetailId { get; set; }
        public Nullable<int> Designationid { get; set; }
        public Nullable<int> DesignationDetailId { get; set; }
        public Nullable<int> QualificationId { get; set; }
        public Nullable<int> CourseId { get; set; }
        public Nullable<int> CrtdUser { get; set; }
        public Nullable<System.DateTime> CrtdTs { get; set; }
        public Nullable<bool> IsCurrentVersion { get; set; }
        public Nullable<System.DateTime> UptdTs { get; set; }
        public Nullable<int> UptdUser { get; set; }
    }
}
