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
    
    public partial class tblRCTMember
    {
        public int MemberId { get; set; }
        public Nullable<int> TypeOfMember { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeDesignation { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string Status { get; set; }
        public Nullable<int> CrtdUser { get; set; }
        public Nullable<System.DateTime> CrtdTS { get; set; }
        public Nullable<int> UpdtUser { get; set; }
        public Nullable<System.DateTime> UpdtTS { get; set; }
        public Nullable<System.DateTime> Effectivedate { get; set; }
    }
}
