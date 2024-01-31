using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IOAS.DataModel;

namespace IOAS.Models
{
    public class ProcessEngineModel : tblUser
    {
        public int ProcessTransactionId { get; set; }
        public int ProcessGuidelineId { get; set; }
        public int ProcessGuidelineDetailId { get; set; }
        public int InitiatedUserId { get; set; }
        public DateTime InitiatedTS { get; set; }
        public string InitiatedMacID { get; set; }
        public string RefNumber { get; set; }
        public bool Closed_F { get; set; }
        public string ClosingStatus { get; set; }
        public int ProcessTransactionDetailId { get; set; }
        public int ProcessSeqNumber { get; set; }
        public int ApproverId { get; set; }
        public string ActionStatus { get; set; }
        public DateTime TransactionTS { get; set; }
        public string TransactionIP { get; set; }
        public string MacID { get; set; }

        public string Comments { get; set; }
        public int RefId { get; set; }
        public string RefTable { get; set; }
        public string RefFieldName { get; set; }

        public int FunctionId { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }

        public string ActionLink { get; set; }
        public string successMethod { get; set; }
        public string failedMethod { get; set; }
        public string FlowTitle { get; set; }

        public string InitUserName { get; set; }
        public string InitFirstName { get; set; }
        public string InitLastName { get; set; }
        public string InitUserImage { get; set; }

        public string ApproverUserName { get; set; }
        public string ApproverUserImage { get; set; }
        public string ApproverFirstName { get; set; }
        public string ApproverLastName { get; set; }
        public string ApproverFullName { get; set; }
        public string clarifyMethod { get; set; }
        public string Validatestatus { get; set; }


    }

    public class ProcessTransactionModel
    {
        public int ProcessTransactionId { get; set; }
        public int ProcessGuidelineDetailId { get; set; }
        public int InitiatedUserId { get; set; }
        public DateTime InitiatedTS { get; set; }
        public string InitiatedMacID { get; set; }
        public bool Closed_F { get; set; }
        public string ClosingStatus { get; set; }

        public string ActionLink { get; set; }
        public string successMethod { get; set; }
        public string failedMethod { get; set; }

    }

    public class ProcessTransactionDetailModel
    {

        public int ProcessTransactionDetailId { get; set; }
        public int ProcessTransactionId { get; set; }
        public int ProcessGuidelineDetailId { get; set; }
        public int ProcessSeqNumber { get; set; }
        public int Approverid { get; set; }
        public string ActionStatus { get; set; }
        public DateTime TransactionTS { get; set; }
        public string TransactionIP { get; set; }
        public string MacID { get; set; }
    }

    public class ProcessGuidelineHeader
    {
        public int ProcessGuidelineId { get; set; }
        public string ProcessGuidelineTitle { get; set; }
        public string ProcessGuidelineDescription { get; set; }
        public Nullable<int> FunctionId { get; set; }
        public Nullable<System.DateTime> CreatedTS { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> LastUpdatedTS { get; set; }
        public Nullable<int> LastUpdatedUserId { get; set; }
    }

    public class ProcessGuidelineWorkflowDocument
    {
        public int ProcessGuidelineWorkflowDocumentId { get; set; }
        public int ProcessGuidelineWorkflowId { get; set; }
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public bool IsRequired { get; set; }
    }

    public class ProcessTransactionDocuments
    {
        public int ProcessTransactionDocumentId { get; set; }
        public int ProcessTransactionDetailId { get; set; }
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }

        public bool IsRequired { get; set; }
        public string UUID { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
    }

}