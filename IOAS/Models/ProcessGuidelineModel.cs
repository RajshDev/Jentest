using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Process
{
    public class ProcessFlowModel
    {
        public int ProcessGuidelineDetailId { get; set; }
        public int ProcessGuidelineid { get; set; }
        public string FlowTitle { get; set; }
        public string FlowDescription { get; set; }
        public string UserCount { get; set; }
        public string ApproverCount { get; set; }
        public int UserId { get; set; }
    }
    public class ProcessGuideline
    {
        public int SlNo { get; set; }
        public int ProcessGuidelineId { get; set; }
        public int FunctionId { get; set; }
        public string FunctionName { get; set; }
        public string ProcessName { get; set; }

    }
    public class Function
    {
        /// <summary>
        /// Used to get or set model member named FunctionId
        /// </summary>
        public int FunctionId { get; set; }

        /// <summary>
        /// Used to get or set model member named FunctionName
        /// </summary>
        public string FunctionName { get; set; }
    }

    /// <summary>
    /// Used to get or set view model Approver class
    /// </summary>
    public class Approver
    {
        /// <summary>
        /// Used to get or set model member named ApproverId
        /// </summary>
        public int ApproverId { get; set; }

        /// <summary>
        /// Used to get or set model member named ApproverName
        /// </summary>
        public string ApproverName { get; set; }
    }

    /// <summary>
    /// Used to get or set view model Status class
    /// </summary>
    public class ProcessFlowStatus
    {
        /// <summary>
        /// Used to get or set model member named StatusId
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        /// Used to get or set model member named StatusName
        /// </summary>
        public string StatusName { get; set; }
    }
    public class ApproverCategoryModel
    {
        public int ApproverFlagId { get; set; }
        public string ApproverFlagname { get; set; }
    }

    public class ControlDataList
    {
        public List<Function> FunctionList { get; set; }
        public List<Approver> ApproverList { get; set; }
        public List<ProcessFlowStatus> StatusList { get; set; }
        public List<Document> DocumentList { get; set; }
        public List<ApproverCategoryModel> ApproverType { get; set; }
    }

    public class Document
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
    }
    public class ProcessFlowUser
    {
        public int ProcessGuidelineDetailId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool UserFlag { get; set; }
        public bool ApprovalFlag { get; set; }

    }


    public class ProcessFlowApproverList
    {
        public int ProcessguidlineworkflowId { get; set; }
        public int processguidlineId { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int ProcessGuidelineDetailId { get; set; }
        public int ApproverLevel { get; set; }
        public bool ApproveFlag { get; set; }
        public bool RejectFlag { get; set; }
        public bool ClarifyFlag { get; set; }
        public bool MarkFlag { get; set; }
        public int DocumentId { get; set; }
        //public List<Document> SelectedDocument { get; set; }
        public int[] ApproverList { get; set; }
        public List<string> SelectedDocument { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public bool IsRequired { get; set; }
        public string UUID { get; set; }

    }


}