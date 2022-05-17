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
    
    public partial class syncobj_0x3843443636334635
    {
        public int OrderId { get; set; }
        public Nullable<int> AppointmentId { get; set; }
        public Nullable<int> AppointmentType { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<int> OrderType { get; set; }
        public string OrderNo { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> Basic { get; set; }
        public Nullable<decimal> OldHRA { get; set; }
        public Nullable<decimal> HRA { get; set; }
        public Nullable<decimal> HRAPercentage { get; set; }
        public Nullable<decimal> MedicalAmount { get; set; }
        public Nullable<int> MedicalType { get; set; }
        public Nullable<int> OldProjectId { get; set; }
        public Nullable<int> NewProjectId { get; set; }
        public Nullable<int> OldDesignation { get; set; }
        public Nullable<int> NewDesignation { get; set; }
        public Nullable<System.DateTime> CrtdTS { get; set; }
        public Nullable<int> CrtdUser { get; set; }
        public Nullable<System.DateTime> UpdtTS { get; set; }
        public Nullable<int> UpdtUser { get; set; }
        public Nullable<decimal> CommitmentAmmount { get; set; }
        public Nullable<decimal> WithdrawAmmount { get; set; }
        public Nullable<decimal> GST { get; set; }
        public Nullable<decimal> OldGST { get; set; }
        public string CommitteeRemarks { get; set; }
        public Nullable<int> CommitteeApprovedBy { get; set; }
        public Nullable<int> OrderwiseSeqId { get; set; }
        public Nullable<int> SeqId { get; set; }
        public bool isHRA { get; set; }
        public bool isMedical { get; set; }
        public bool isCommitmentReject { get; set; }
        public bool isExtended { get; set; }
        public bool isUpdated { get; set; }
        public bool isGovAgencyFund { get; set; }
        public Nullable<int> MeternityOrderId { get; set; }
        public bool isRelisedPayment { get; set; }
        public Nullable<System.DateTime> ActualAppointmentStartDate { get; set; }
        public Nullable<System.DateTime> ActualAppointmentEndDate { get; set; }
        public bool Is_Clarify { get; set; }
        public bool IsGSTApplicable { get; set; }
        public Nullable<System.DateTime> ArrearOrDeductionTillDate { get; set; }
        public Nullable<decimal> ArrearOrDeductionAmount { get; set; }
        public Nullable<int> RequestedBy { get; set; }
        public Nullable<System.DateTime> WithdrawTillDate { get; set; }
        public Nullable<bool> InitByPI_f { get; set; }
        public Nullable<int> WFSeqId { get; set; }
        public Nullable<int> SalaryLevelId { get; set; }
        public Nullable<int> OrderRequestId { get; set; }
        public Nullable<int> CommitteeMember { get; set; }
        public Nullable<int> CommitteeMembers { get; set; }
        public Nullable<int> Chairperson { get; set; }
    }
}
