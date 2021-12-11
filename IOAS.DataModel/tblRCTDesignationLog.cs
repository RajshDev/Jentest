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
    
    public partial class tblRCTDesignationLog
    {
        public int DesignationLogId { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public Nullable<int> TypeOfAppointment { get; set; }
        public string DesignationCode { get; set; }
        public string Designation { get; set; }
        public Nullable<int> Department { get; set; }
        public Nullable<decimal> PayStructureMinMum { get; set; }
        public Nullable<bool> HRA { get; set; }
        public Nullable<decimal> HRABasic { get; set; }
        public Nullable<bool> Medical { get; set; }
        public Nullable<decimal> MedicalDeducation { get; set; }
        public Nullable<int> AgeLimit { get; set; }
        public Nullable<decimal> AnnualIncrement { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> CrtdUser { get; set; }
        public Nullable<System.DateTime> CrtdTs { get; set; }
        public Nullable<decimal> PayStructureMaxiMum { get; set; }
        public Nullable<bool> GateScore { get; set; }
        public bool ConsolidatedPay { get; set; }
        public bool FellowshipPay { get; set; }
        public Nullable<bool> IsNotValid { get; set; }
        public Nullable<bool> IsSCST { get; set; }
        public Nullable<int> SCSTAgeLimit { get; set; }
    }
}
