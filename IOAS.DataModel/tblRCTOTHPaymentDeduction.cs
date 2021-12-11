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
    
    public partial class tblRCTOTHPaymentDeduction
    {
        public int OTHPayDeductionId { get; set; }
        public string EmployeeNo { get; set; }
        public Nullable<int> AppointmentId { get; set; }
        public Nullable<int> AppointmentType { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string MonthandYear { get; set; }
        public Nullable<int> CrtdUser { get; set; }
        public Nullable<System.DateTime> CrtdTS { get; set; }
        public Nullable<int> UpdtUser { get; set; }
        public Nullable<System.DateTime> UpdtTs { get; set; }
        public string Status { get; set; }
        public string OTHPayDeductionNo { get; set; }
        public Nullable<int> OrderId { get; set; }
        public string OTHPaymentDeductionFile { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string ProcessedMonthandYear { get; set; }
        public Nullable<int> SalaryType { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public bool Isbackend_f { get; set; }
        public Nullable<bool> IITMPensioner_f { get; set; }
        public Nullable<decimal> Basic { get; set; }
        public Nullable<decimal> HRA { get; set; }
        public Nullable<decimal> Medical { get; set; }
        public string ProcessType { get; set; }
        public Nullable<bool> IsNoCommitment_f { get; set; }
        public Nullable<bool> IsBulkBooking_f { get; set; }
        public Nullable<bool> MedicalInclusive_f { get; set; }
    }
}
