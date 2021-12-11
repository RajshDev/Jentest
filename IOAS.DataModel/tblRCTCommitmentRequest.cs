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
    
    public partial class tblRCTCommitmentRequest
    {
        public int RecruitmentRequestId { get; set; }
        public string ReferenceNumber { get; set; }
        public string AppointmentType { get; set; }
        public string TypeCode { get; set; }
        public string CandidateName { get; set; }
        public string CandidateDesignation { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<decimal> TotalSalary { get; set; }
        public Nullable<decimal> RequestedCommitmentAmount { get; set; }
        public string AllocationHead { get; set; }
        public Nullable<int> AllocationHeadId { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> Crtd_TS { get; set; }
        public Nullable<int> Crtd_UserId { get; set; }
        public Nullable<System.DateTime> Updt_TS { get; set; }
        public Nullable<int> Updt_UserId { get; set; }
        public Nullable<int> CommitmentCrtdBy { get; set; }
        public Nullable<System.DateTime> CommitmentCrtdTS { get; set; }
        public string CommitmentNumber { get; set; }
        public Nullable<bool> IsBookedFullRequestAmount { get; set; }
        public Nullable<int> EmpId { get; set; }
        public string EmpNumber { get; set; }
        public Nullable<decimal> BookedAmount { get; set; }
        public string OrderNumber { get; set; }
        public string Remarks { get; set; }
        public string RequestType { get; set; }
        public string ReasonforClose { get; set; }
        public Nullable<bool> IsClosed_f { get; set; }
        public Nullable<int> ReasonId { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> RefId { get; set; }
    }
}
