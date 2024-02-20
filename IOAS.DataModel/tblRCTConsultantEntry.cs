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
    
    public partial class tblRCTConsultantEntry
    {
        public int Consultant_AppointmentId { get; set; }
        public int Consultant_MasterId { get; set; }
        public string Consultant_EmpNo { get; set; }
        public string Consultant_ServiceNo { get; set; }
        public string Consultant_ApplicationNo { get; set; }
        public string Consultant_ServiceDescription { get; set; }
        public string Consultant_Type { get; set; }
        public Nullable<int> Consultant_Code { get; set; }
        public string Consultant_Title { get; set; }
        public string Consultant_AoE { get; set; }
        public string Consultant_SoW { get; set; }
        public Nullable<int> Consultant_GSTStatus { get; set; }
        public string Consultant_GSTIN { get; set; }
        public Nullable<decimal> Consultant_GSTINPercentage { get; set; }
        public string Consultant_TandC { get; set; }
        public string Consultant_PaymentTerms { get; set; }
        public Nullable<int> Consultant_ProjectId { get; set; }
        public Nullable<System.DateTime> Consultant_AppStartDt { get; set; }
        public Nullable<System.DateTime> Consultant_AppEndDt { get; set; }
        public Nullable<System.DateTime> Consultant_ActualAppStartDt { get; set; }
        public Nullable<System.DateTime> Consultant_ActualAppEndDt { get; set; }
        public string Consultant_CurrType { get; set; }
        public Nullable<decimal> Consultant_CurrValue { get; set; }
        public Nullable<decimal> Consultant_CurrConvertionRate { get; set; }
        public Nullable<decimal> Consultant_CurrFluctuationvalue { get; set; }
        public Nullable<decimal> Consultant_RetainerFee { get; set; }
        public Nullable<decimal> Consultant_GSTvalue { get; set; }
        public Nullable<int> Consultant_GSTEligibility { get; set; }
        public Nullable<decimal> Consultant_Commitvalue { get; set; }
        public Nullable<int> Consultant_CommitmentID { get; set; }
        public string Consultant_CommitmentNumber { get; set; }
        public Nullable<decimal> Consultant_AvailableCommitvalue { get; set; }
        public Nullable<int> Consultant_ITTDSType { get; set; }
        public Nullable<decimal> Consultant_ITTDSPercentage { get; set; }
        public Nullable<System.DateTime> Consultant_ITTDSExemptedDate { get; set; }
        public Nullable<int> Consultant_GSTTDSType { get; set; }
        public Nullable<int> Consultant_RCMType { get; set; }
        public Nullable<int> Consultant_RCMCategory { get; set; }
        public Nullable<int> Consultant_PayType { get; set; }
        public Nullable<bool> Consultant_AuditApproval { get; set; }
        public Nullable<int> Consultant_WPCategory { get; set; }
        public string Consultant_WorkPlace { get; set; }
        public string Consultant_Remarks { get; set; }
        public Nullable<System.DateTime> Consultant_ReqReceivedDate { get; set; }
        public Nullable<int> Consultant_ReqInitBy { get; set; }
        public System.DateTime Consultant_CrtdTs { get; set; }
        public int Consultant_CrtdUser { get; set; }
        public Nullable<System.DateTime> Consultant_UptdTs { get; set; }
        public Nullable<int> Consultant_UptdUser { get; set; }
        public string Consultant_Status { get; set; }
        public bool Consultant_ActivityStatus { get; set; }
        public bool Consultant_IsDeleted { get; set; }
        public int Consultant_SeqNbr { get; set; }
        public string Consultant_DAComments { get; set; }
        public string Consultant_FlowApprover { get; set; }
    }
}
