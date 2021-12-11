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
    
    public partial class GetProcessFlowByUser_Result
    {
        public int ProcessTransactionId { get; set; }
        public Nullable<int> InitiatedUserId { get; set; }
        public int ProcessGuidelineWorkFlowId { get; set; }
        public Nullable<int> ProcessGuidelineId { get; set; }
        public Nullable<int> ProcessGuidelineDetailId { get; set; }
        public Nullable<int> ApproverId { get; set; }
        public Nullable<int> ApproverLevel { get; set; }
        public bool Approve_f { get; set; }
        public bool Reject_f { get; set; }
        public bool Clarify_f { get; set; }
        public bool Mark_f { get; set; }
        public Nullable<System.DateTime> CreatedTS { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> LastUpdatedTS { get; set; }
        public Nullable<int> LastUpdatedUserId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string ActionStatus { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<int> RefId { get; set; }
        public string RefTable { get; set; }
        public string RefFieldName { get; set; }
    }
}
