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
    
    public partial class tblRCTSTEExperienceDetailLog
    {
        public int STEExperienceLogID { get; set; }
        public Nullable<int> STELogID { get; set; }
        public Nullable<int> STEExperienceDetailID { get; set; }
        public Nullable<int> STEID { get; set; }
        public Nullable<int> TypeID { get; set; }
        public string Organisation { get; set; }
        public string Designation { get; set; }
        public Nullable<System.DateTime> FromYear { get; set; }
        public Nullable<System.DateTime> ToYear { get; set; }
        public Nullable<decimal> SalaryDrawn { get; set; }
        public string DocumentFilePath { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> CrtdUser { get; set; }
        public Nullable<System.DateTime> CrtdTs { get; set; }
        public Nullable<int> UptdUser { get; set; }
        public Nullable<System.DateTime> UptdTs { get; set; }
        public Nullable<bool> isCurrentVersion { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public string FileName { get; set; }
        public string FileNo { get; set; }
    }
}
