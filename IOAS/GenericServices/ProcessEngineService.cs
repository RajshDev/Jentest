using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using DataAccessLayer;
using IOAS.GenericServices;
using IOAS.Models;
using IOAS.DataModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Data.SqlClient;

namespace IOAS.GenericServices
{
    public class ProcessEngineService
    {

        public static ProcessGuidelineHeader GetProcessFlowByName(string flowName)
        {
            try
            {
                ProcessGuidelineHeader model = new ProcessGuidelineHeader();

                using (var context = new IOASDBEntities())
                {
                    var result = context.tblProcessGuidelineHeader
                         .Where(ph => ph.ProcessGuidelineTitle == flowName)
                         .Select(prop => prop)
                         .SingleOrDefault();
                    if (result != null)
                    {
                        model.ProcessGuidelineId = result.ProcessGuidelineId;
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }

        public static List<ProcessEngineModel> GetProcessFlowByUser(int processGuideLineId, int userId)
        {
            try
            {
                List<ProcessEngineModel> model = new List<ProcessEngineModel>();
                ProcessEngine db = new ProcessEngine();
                DataSet dsTransaction = db.GetProcessFlowByUser(processGuideLineId, userId, -1);
                DataTable dtProcessFlow = dsTransaction.Tables[0];
                model = Converter.GetEntityList<ProcessEngineModel>(dtProcessFlow);

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }

        public static List<ProcessEngineModel> GetProcessStatusByUser(int processGuideLineId, int userId, int transactionId)
        {
            try
            {
                List<ProcessEngineModel> model = new List<ProcessEngineModel>();
                ProcessEngine db = new ProcessEngine();
                DataSet dsTransaction = db.GetProcessFlowByUser(processGuideLineId, userId, transactionId);
                DataTable dtProcessFlow = dsTransaction.Tables[1];
                model = Converter.GetEntityList<ProcessEngineModel>(dtProcessFlow);

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }

        public static List<ProcessEngineModel> GetProcessStatusByGuidelineId(int processGuideLineId)
        {
            try
            {
                List<ProcessEngineModel> model = new List<ProcessEngineModel>();
                using (var context = new IOASDBEntities())
                {
                    var records = (from PT in context.tblProcessTransaction
                                   join PTD in context.tblProcessTransactionDetail on PT.ProcessTransactionId equals PTD.ProcessTransactionId
                                   join U in context.tblUser on PT.InitiatedUserId equals U.UserId
                                   //join WF in context.tblProcessGuidelineWorkFlow on PT.ProcessGuidelineDetailId equals WF.ProcessGuidelineDetailId
                                   join PG in context.tblProcessGuidelineDetail on PT.ProcessGuidelineDetailId equals PG.ProcessGuidelineDetailId
                                   join PGH in context.tblProcessGuidelineHeader on PG.ProcessGuidelineId equals PGH.ProcessGuidelineId
                                   join F in context.tblFunction on PGH.FunctionId equals F.FunctionId
                                   where (PG.ProcessGuidelineId == processGuideLineId)
                                   select new
                                   {
                                       PT.ProcessTransactionId,
                                       PTD.ProcessTransactionDetailId,
                                       PTD.ProcessGuidelineDetailId,
                                       PTD.ProcessSeqNumber,
                                       PTD.Approverid,
                                       PTD.ActionStatus,
                                       PTD.RefId,
                                       PTD.RefTable,
                                       PTD.RefFieldName,
                                       PTD.TransactionTS,
                                       PTD.TransactionIP,
                                       PTD.MacID,
                                       U.UserName,
                                       U.UserImage,
                                       U.FirstName,
                                       U.LastName,
                                       PGH.FunctionId,
                                       F.FunctionName,
                                       F.ActionName,
                                       F.ControllerName

                                   }).ToList();
                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {

                            model.Add(new ProcessEngineModel
                            {
                                ProcessTransactionId = records[i].ProcessTransactionId,
                                ProcessTransactionDetailId = records[i].ProcessTransactionDetailId,
                                ProcessGuidelineDetailId = Convert.ToInt32(records[i].ProcessGuidelineDetailId),
                                ProcessSeqNumber = Convert.ToInt32(records[i].ProcessSeqNumber),
                                ApproverId = Convert.ToInt32(records[i].Approverid),
                                ActionStatus = records[i].ActionStatus,
                                RefId = Convert.ToInt32(records[i].RefId),
                                RefTable = records[i].RefTable,
                                RefFieldName = records[i].RefFieldName,
                                FunctionId = Convert.ToInt32(records[i].FunctionId),
                                ActionName = records[i].ActionName,
                                ControllerName = records[i].ControllerName,
                                TransactionTS = Convert.ToDateTime(records[i].TransactionTS),
                                //model[i].TransactionIP = records[i].TransactionIP,
                                //model[i].MacID = records[i].MacID;
                                //model[i].UserName = records[i].UserName,
                                UserImage = records[i].UserImage,
                                FirstName = records[i].FirstName,
                                LastName = records[i].LastName
                            });

                        }
                    }


                    return model;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static PagedData<ProcessEngineModel> GetApproveList(int userId, int page, int pageSize)
        {
            try
            {
                List<ProcessEngineModel> model = new List<ProcessEngineModel>();
                int skiprec = 0;

                if (page == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (page - 1) * pageSize;
                }
                var searchData = new PagedData<ProcessEngineModel>();
                using (var context = new IOASDBEntities())
                {
                    var records = (from PT in context.tblProcessTransaction
                                   join PTD in context.tblProcessTransactionDetail on PT.ProcessTransactionId equals PTD.ProcessTransactionId
                                   join PGU in context.tblProcessGuidelineUser on PT.ProcessGuidelineDetailId equals PGU.ProcessGuidelineDetailId
                                   //join WF in context.tblProcessGuidelineWorkFlow on PT.ProcessGuidelineDetailId equals WF.ProcessGuidelineDetailId
                                   join PG in context.tblProcessGuidelineDetail on PT.ProcessGuidelineDetailId equals PG.ProcessGuidelineDetailId
                                   join PGH in context.tblProcessGuidelineHeader on PG.ProcessGuidelineId equals PGH.ProcessGuidelineId
                                   join F in context.tblFunction on PGH.FunctionId equals F.FunctionId
                                   join U in context.tblUser on PGU.UserId equals U.UserId
                                   join IU in context.tblUser on PT.InitiatedUserId equals IU.UserId
                                   //where (PGU.UserId == userId || PT.InitiatedUserId == userId)
                                   where (PGU.UserId == userId && PTD.ActionStatus == "Initiated")
                                   orderby PT.ProcessTransactionId
                                   select new
                                   {
                                       PT.ProcessTransactionId,
                                       PT.ActionLink,
                                       PT.InitiatedUserId,
                                       InitUserName = IU.UserName,
                                       InitFirstName = IU.FirstName,
                                       InitLastName = IU.LastName,
                                       InitUserImage = IU.UserImage,
                                       PTD.ProcessTransactionDetailId,
                                       PTD.ProcessGuidelineDetailId,
                                       PTD.ProcessSeqNumber,
                                       PTD.Approverid,
                                       PTD.ActionStatus,
                                       PTD.RefId,
                                       PTD.RefTable,
                                       PTD.RefFieldName,
                                       PTD.TransactionTS,
                                       PTD.TransactionIP,
                                       PTD.MacID,
                                       ApproverUserName = U.UserName,
                                       ApproverUserImage = U.UserImage,
                                       ApproverFirstName = U.FirstName,
                                       ApproverLastName = U.LastName,
                                       PGH.FunctionId,
                                       F.FunctionName,
                                       F.ActionName,
                                       F.ControllerName
                                   }).Skip(skiprec).Take(pageSize).ToList();
                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {

                            model.Add(new ProcessEngineModel
                            {
                                ProcessTransactionId = records[i].ProcessTransactionId,
                                ProcessTransactionDetailId = records[i].ProcessTransactionDetailId,
                                ProcessGuidelineDetailId = Convert.ToInt32(records[i].ProcessGuidelineDetailId),
                                ProcessSeqNumber = Convert.ToInt32(records[i].ProcessSeqNumber),
                                ApproverId = Convert.ToInt32(records[i].Approverid),
                                ActionStatus = records[i].ActionStatus,
                                RefId = Convert.ToInt32(records[i].RefId),
                                RefTable = records[i].RefTable,
                                RefFieldName = records[i].RefFieldName,
                                FunctionId = Convert.ToInt32(records[i].FunctionId),
                                ActionName = records[i].ActionName,
                                ControllerName = records[i].ControllerName,
                                TransactionTS = Convert.ToDateTime(records[i].TransactionTS),
                                //model[i].TransactionIP = records[i].TransactionIP,
                                //model[i].MacID = records[i].MacID;
                                //model[i].UserName = records[i].UserName,
                                ApproverUserImage = records[i].ApproverUserImage,
                                ApproverFirstName = records[i].ApproverFirstName,
                                ApproverLastName = records[i].ApproverLastName,
                                InitUserImage = records[i].InitUserImage,
                                InitFirstName = records[i].InitFirstName,
                                InitLastName = records[i].InitLastName
                            });

                        }
                        var recordCount = (from PT in context.tblProcessTransaction
                                           join PTD in context.tblProcessTransactionDetail on PT.ProcessTransactionId equals PTD.ProcessTransactionId
                                           join PGU in context.tblProcessGuidelineUser on PT.ProcessGuidelineDetailId equals PGU.ProcessGuidelineDetailId
                                           //join WF in context.tblProcessGuidelineWorkFlow on PT.ProcessGuidelineDetailId equals WF.ProcessGuidelineDetailId
                                           join PG in context.tblProcessGuidelineDetail on PT.ProcessGuidelineDetailId equals PG.ProcessGuidelineDetailId
                                           join PGH in context.tblProcessGuidelineHeader on PG.ProcessGuidelineId equals PGH.ProcessGuidelineId
                                           join F in context.tblFunction on PGH.FunctionId equals F.FunctionId
                                           join U in context.tblUser on PGU.UserId equals U.UserId
                                           where (PGU.UserId == userId)
                                           orderby PT.ProcessTransactionId
                                           select PTD).ToList().Count();

                        searchData.Data = model;
                        searchData.TotalRecords = recordCount;
                        searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / pageSize));
                    }


                    return searchData;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new PagedData<ProcessEngineModel>();
            }
        }


        public static List<ProcessGuidelineWorkflowDocument> GetWorkFlowDocumentList(int processGuideLineId, int userId)
        {
            try
            {
                List<ProcessGuidelineWorkflowDocument> model = new List<ProcessGuidelineWorkflowDocument>();
                using (var context = new IOASDBEntities())
                {
                    var records = (from PGD in context.tblProcessGuidelineDetail
                                   join WF in context.tblProcessGuidelineWorkFlow on PGD.ProcessGuidelineDetailId equals WF.ProcessGuidelineDetailId
                                   join WFD in context.tblProcessGuidelineWorkflowDocument on WF.ProcessGuidelineWorkFlowId equals WFD.ProcessGuidelineWorkflowId
                                   where PGD.ProcessGuidelineId == processGuideLineId && WF.ApproverId == userId
                                   select new
                                   {
                                       WFD.ProcessGuidelineWorkflowDocumentId,
                                       WFD.ProcessGuidelineWorkflowId,
                                       WFD.DocumentId,
                                       WFD.DocumentName,
                                       WFD.DocumentType,
                                       WFD.IsRequired
                                   }).ToList();
                    if (records != null && records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new ProcessGuidelineWorkflowDocument
                            {
                                ProcessGuidelineWorkflowId = Convert.ToInt32(records[i].ProcessGuidelineWorkflowId),
                                ProcessGuidelineWorkflowDocumentId = Convert.ToInt32(records[i].ProcessGuidelineWorkflowDocumentId),
                                DocumentId = Convert.ToInt32(records[i].DocumentId),
                                DocumentName = records[i].DocumentName,
                                DocumentType = records[i].DocumentType,
                                IsRequired = Convert.ToBoolean(records[i].IsRequired)
                            });
                        }
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static List<ProcessEngineModel> GetRecordByRefId(int ProcessGuidelineId, int userId, int refId)
        {
            try
            {
                List<ProcessEngineModel> transactions = new List<ProcessEngineModel>();
                using (var context = new IOASDBEntities())
                {
                    var records = (from PGD in context.tblProcessGuidelineDetail
                                   join PT in context.tblProcessTransaction on PGD.ProcessGuidelineDetailId equals PT.ProcessGuidelineDetailId
                                   //join PTD in context.tblProcessTransactionDetail on PT.ProcessTransactionId equals PTD.ProcessTransactionId
                                   join U in context.tblUser on PT.InitiatedUserId equals U.UserId
                                   where PT.RefId == refId && PGD.ProcessGuidelineId == ProcessGuidelineId
                                   orderby PT.ProcessTransactionId
                                   select new
                                   {
                                       PT.ProcessTransactionId,
                                       PT.ProcessGuidelineDetailId,
                                       U.UserName,
                                       U.UserImage,
                                       U.FirstName,
                                       U.LastName,
                                       PT.RefId
                                   }).ToList();

                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            transactions.Add(new ProcessEngineModel
                            {
                                ProcessTransactionId = records[i].ProcessTransactionId,
                                //ProcessTransactionDetailId = records[i].ProcessTransactionDetailId,
                                ProcessGuidelineDetailId = Convert.ToInt32(records[i].ProcessGuidelineDetailId),
                                //ProcessSeqNumber = Convert.ToInt32(records[i].ProcessSeqNumber),
                                //Approverid = Convert.ToInt32(records[i].Approverid),
                                //ActionStatus = Convert.ToString(records[i].ActionStatus),
                                InitUserName = records[i].UserName,
                                InitUserImage = records[i].UserImage,
                                InitFirstName = records[i].FirstName,
                                InitLastName = records[i].LastName,
                                RefId = Convert.ToInt32(records[i].RefId)
                            });

                        }
                    }
                }
                return transactions;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static List<ProcessEngineModel> GetPendingTransactionByUser(int processGuideLineId, int userId)
        {
            try
            {
                List<ProcessEngineModel> model = new List<ProcessEngineModel>();
                ProcessEngine db = new ProcessEngine();
                DataSet dsTrasaction = db.GetPendingTransactionByUser(processGuideLineId, userId);
                model = Converter.GetEntityList<ProcessEngineModel>(dsTrasaction.Tables[0]);
                using (var context = new IOASDBEntities())
                {
                    //var records = (from po in context.tblProcessTransactionDetail
                    //               join pt in context.tblProcessTransaction on po.ProcessTransactionId equals pt.ProcessTransactionId
                    //               join pgd in context.tblProcessGuidelineDetail on pt.ProcessGuidelineDetailId equals pgd.ProcessGuidelineDetailId
                    //               from clarify in context.tblProcessTransactionDetail.Where(m => m.ProcessTransactionId == po.ProcessTransactionId && m.ActionStatus == "Clarify").OrderByDescending(m => m.TransactionTS).Take(1)
                    //               join user in context.tblUser on clarify.Approverid equals user.UserId
                    //               where po.Approverid == userId && po.ActionStatus == "Initiated" && po.Clarified == true && pt.Closed_F == false
                    //               && !context.tblProcessTransactionDetail.Any(m => m.RefId == po.RefId && m.RefFieldName == po.RefFieldName && m.ActionStatus == "Initiated" && (m.Rejected == true || m.Clarified == false || m.Clarified == null))
                    //               select new
                    //               {
                    //                   po.ProcessTransactionId,
                    //                   po.ProcessGuidelineDetailId,
                    //                   po.RefId,
                    //                   //po.RefNumber,
                    //                   user.FirstName,
                    //                   user.LastName,
                    //                   pgd.FlowTitle,
                    //                   pt.ActionLink,
                    //                   po.TransactionTS,
                    //                   po.ActionStatus,
                    //                   pt.RefNumber
                    //               }).OrderByDescending(m => m.TransactionTS).ToList();
                    var UserId = new SqlParameter("@userId", SqlDbType.Int)
                    {
                        Value = userId
                    };
                    var records = context.Database.SqlQuery<ProcessEngineModel>(
                                "EXEC Sp_GetProcessTransactionDetail @userId", UserId).ToList();

                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new ProcessEngineModel
                            {
                                ProcessTransactionId = records[i].ProcessTransactionId ,
                                ProcessGuidelineDetailId = Convert.ToInt32(records[i].ProcessGuidelineDetailId),
                                FirstName = records[i].FirstName,
                                LastName = records[i].LastName,
                                RefId = records[i].RefId,
                                RefNumber = records[i].RefNumber,
                                FlowTitle = records[i].FlowTitle,
                                ActionLink = records[i].ActionLink,
                                TransactionTS = Convert.ToDateTime(records[i].TransactionTS),
                                ActionStatus = "Clarify"
                            });

                        }
                    }
                }


                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }
    }

    public class FlowEngine
    {
        private ProcessEngineModel _model = new ProcessEngineModel();
        private List<ProcessEngineModel> listModel = new List<ProcessEngineModel>();
        private int _processGuideLineId;
        private int _userId;
        private int _refId;
        private string _refFieldName;
        private int processTransactionId;
        private int processTransactionDetailId;
        private string comment;
        private string refTable;
        private bool isApproved;
        private bool isRejected;
        private bool isClarified;
        private bool isLastApprover;
        private int nextApprover;
        private bool canApprove;
        private bool canReject;
        private bool canClarify;
        private string actionLink;
        private string successMethod;
        private string failedMethod;
        private string clarifyMethod;
        private int functionId;
        private string refNumber;

        public string errorMsg;

        private delegate bool ProcessCompleteDelegate(int refId, int updateUserID);
        private delegate bool ProcessFailedDelegate(int refId, int updateUserID);
        private delegate bool ProcessClarifyDelegate(int refId, int updateUserID);


        public ProcessEngineModel record = new ProcessEngineModel();
        public ProcessEngineModel records = new ProcessEngineModel();
        public List<ProcessTransactionDocuments> documents = new List<ProcessTransactionDocuments>();
        public List<ProcessGuidelineWorkflowDocument> workFlowDocuments = new List<ProcessGuidelineWorkflowDocument>();

        FlowEngine()
        {

        }
        FlowEngine(int processGuideLineId, int userId, int refId, string refFieldName)
        {
            _processGuideLineId = processGuideLineId;
            _userId = userId;
            _refId = refId;
            _refFieldName = refFieldName;
            using (var context = new IOASDBEntities())
            {

                ProcessEngine db = new ProcessEngine();
                DataSet dsTransaction = db.GetProcessFlowByUser(this._processGuideLineId, this._userId, this._refId);
                DataTable dtProcessFlow = dsTransaction.Tables[1];

                if (dtProcessFlow.Rows.Count > 0)
                {
                    foreach (DataRow row in dtProcessFlow.Rows)
                    {
                        object value = row["ProcessTransactionDetailId"];
                        if (value == DBNull.Value)
                        {
                            this.nextApprover = Convert.ToInt32(row["ApproverId"].ToString());
                            break;
                        }
                    }
                }

                var records = (from PGD in context.tblProcessGuidelineDetail
                               join WF in context.tblProcessGuidelineWorkFlow on PGD.ProcessGuidelineDetailId equals WF.ProcessGuidelineDetailId
                               join PT in context.tblProcessTransaction on PGD.ProcessGuidelineDetailId equals PT.ProcessGuidelineDetailId
                               join U in context.tblUser on WF.ApproverId equals U.UserId
                               where PT.RefId == this._refId && PGD.ProcessGuidelineId == this._processGuideLineId
                               && WF.ApproverId == this._userId
                               orderby PT.ProcessTransactionId
                               select new
                               {
                                   PT.ProcessTransactionId,
                                   PT.ProcessGuidelineDetailId,
                                   WF.ApproverLevel,
                                   WF.ApproverId,
                                   WF.Approve_f,
                                   WF.Clarify_f,
                                   WF.Reject_f,
                                   U.UserName,
                                   U.FirstName,
                                   U.LastName,
                                   PT.RefId,
                                   PT.failedMethod,
                                   PT.successMethod,
                                   PT.clarifyMethod
                               }).ToList();
                if (records.Count > 0)
                {
                    this.canApprove = Convert.ToBoolean(records[0].Approve_f);
                    this.canReject = Convert.ToBoolean(records[0].Reject_f);
                    this.canClarify = Convert.ToBoolean(records[0].Clarify_f);
                    this.successMethod = records[0].successMethod;
                    this.failedMethod = records[0].failedMethod;
                    this.clarifyMethod = records[0].clarifyMethod;
                }
            }

        }

        public static FlowEngine Init(int processGuideLineId, int userId, int refId, string refFieldName)
        {
            return new FlowEngine(processGuideLineId, userId, refId, refFieldName);
        }
        public static FlowEngine Empty_Init()
        {
            return new FlowEngine();
        }
        public FlowEngine Comment(string comment)
        {
            this.comment = comment;
            return this;
        }

        public FlowEngine ActionLink(string link)
        {
            this.actionLink = link;
            return this;
        }
        public FlowEngine FunctionId(int fId)
        {
            this.functionId = fId;
            return this;
        }
        public FlowEngine ReferenceNumber(string refNo)
        {
            this.refNumber = refNo;
            return this;
        }
        public FlowEngine SuccessMethod(string name)
        {
            this.successMethod = name;
            return this;
        }

        public FlowEngine FailedMethod(string name)
        {
            this.failedMethod = name;
            return this;
        }

        public FlowEngine ClarifyMethod(string name)
        {
            this.clarifyMethod = name;
            return this;
        }

        private FlowEngine IsApproved()
        {
            List<ProcessEngineModel> transactions = CheckTransactionFound(this._processGuideLineId, this._userId, this._refId);
            if (transactions != null && transactions.Count > 0)
            {
                for (int i = 0; i < transactions.Count; i++)
                {
                    if (transactions[i].ActionStatus != "" && transactions[i].ActionStatus.ToLower() == "approved")
                    {
                        this.errorMsg = "Already approved.";
                        this.isApproved = true;
                        this.processTransactionId = transactions[i].ProcessTransactionId;
                        return this;
                    }
                }
                this.errorMsg = "";
                return this;
            }
            return this;
        }

        private FlowEngine IsRejected()
        {
            using (var context = new IOASDBEntities())
            {
                var records = (from PGD in context.tblProcessGuidelineDetail
                               join PT in context.tblProcessTransaction on PGD.ProcessGuidelineDetailId equals PT.ProcessGuidelineDetailId
                               join PTD in context.tblProcessTransactionDetail on PT.ProcessTransactionId equals PTD.ProcessTransactionId
                               join U in context.tblUser on PT.InitiatedUserId equals U.UserId
                               where PTD.RefId == this._refId && PGD.ProcessGuidelineId == this._processGuideLineId
                               orderby PTD.ProcessTransactionDetailId
                               select new
                               {
                                   PT.ProcessTransactionId,
                                   PT.ProcessGuidelineDetailId,
                                   PTD.ProcessTransactionDetailId,
                                   PTD.ProcessSeqNumber,
                                   PTD.Approverid,
                                   PTD.ActionStatus,
                                   U.UserName,
                                   U.UserImage,
                                   U.FirstName,
                                   U.LastName,
                                   PTD.RefId
                               }).ToList();

                if (records.Count > 0)
                {
                    int len = records.Count;
                    this.isRejected = (records[len - 1].ActionStatus.ToLower() == "rejected");
                    if (this.isRejected)
                    {
                        this.errorMsg = "Already rejected.";
                        this.processTransactionId = records[len - 1].ProcessTransactionId;
                        return this;
                    }

                }
            }
            this.errorMsg = "";
            return this;
        }

        private FlowEngine IsClarified()
        {

            using (var context = new IOASDBEntities())
            {
                var records = (from PGD in context.tblProcessGuidelineDetail
                               join PT in context.tblProcessTransaction on PGD.ProcessGuidelineDetailId equals PT.ProcessGuidelineDetailId
                               join PTD in context.tblProcessTransactionDetail on PT.ProcessTransactionId equals PTD.ProcessTransactionId
                               join U in context.tblUser on PT.InitiatedUserId equals U.UserId
                               where PTD.RefId == this._refId && PGD.ProcessGuidelineId == this._processGuideLineId
                               orderby PTD.ProcessTransactionDetailId
                               select new
                               {
                                   PT.ProcessTransactionId,
                                   PT.ProcessGuidelineDetailId,
                                   PTD.ProcessTransactionDetailId,
                                   PTD.ProcessSeqNumber,
                                   PTD.Approverid,
                                   PTD.ActionStatus,
                                   U.UserName,
                                   U.UserImage,
                                   U.FirstName,
                                   U.LastName,
                                   PTD.RefId
                               }).ToList();

                if (records.Count > 0)
                {
                    int len = records.Count;
                    this.isClarified = records[len - 1].ActionStatus.ToLower() == "clarify";  //len == 1 ? (records[len].ActionStatus.ToLower() == "clarify") : (records[len - 1].ActionStatus.ToLower() == "clarify");
                    if (this.isClarified)
                    {
                        this.errorMsg = "Already send for clarificaion.";
                        this.processTransactionId = records[len - 1].ProcessTransactionId; //len == 1 ? records[len].ProcessTransactionId : records[len-1].ProcessTransactionId;
                        return this;
                    }
                }
            }
            this.errorMsg = "";
            return this;
        }

        private FlowEngine IsLastApprover()
        {

            using (var context = new IOASDBEntities())
            {
                var records = (from PGD in context.tblProcessGuidelineDetail
                               join WF in context.tblProcessGuidelineWorkFlow on PGD.ProcessGuidelineDetailId equals WF.ProcessGuidelineDetailId
                               join PT in context.tblProcessTransaction on PGD.ProcessGuidelineDetailId equals PT.ProcessGuidelineDetailId
                               //join PTD in context.tblProcessTransactionDetail on PT.ProcessTransactionId equals PTD.ProcessTransactionId
                               join U in context.tblUser on WF.ApproverId equals U.UserId
                               where WF.ProcessGuidelineId == this._processGuideLineId && PT.RefId == this._refId
                               orderby WF.ApproverLevel ascending
                               select new
                               {
                                   PT.ProcessTransactionId,
                                   PT.ProcessGuidelineDetailId,
                                   WF.ApproverId,
                                   WF.ApproverLevel,
                                   U.UserName,
                                   U.UserImage,
                                   U.FirstName,
                                   U.LastName,
                                   PT.RefId
                               }).ToList();

                if (records.Count > 0)
                {
                    int len = records.Count;
                    this.isLastApprover = (Convert.ToInt32(records[len - 1].ApproverId) == this._userId);
                }
            }
            this.errorMsg = "";
            return this;
        }

        public FlowEngine ProcessGuideLineId(int processGuideLineId)
        {
            this._processGuideLineId = processGuideLineId;
            return this;
        }

        public FlowEngine ProcessInit()
        {
            try
            {
                ProcessEngineModel model = new ProcessEngineModel();
                int processTransacionId = 0;
                List<ProcessEngineModel> transactions = CheckTransactionFound(this._processGuideLineId, this._userId, this._refId);
                if (transactions != null && transactions.Count > 0)
                {
                    for (int i = 0; i < transactions.Count; i++)
                    {
                        if (transactions[i].ActionStatus != "" && (transactions[i].ActionStatus.ToLower() == "clarified" || transactions[i].ActionStatus.ToLower() == "rejected"))
                        {
                            break;
                        }
                        else if (transactions[i].ActionStatus != "" && transactions[i].ActionStatus.ToLower() == "approved")
                        {
                            this.errorMsg = "Already approved.";
                            return this;
                        }
                    }
                    //this.errorMsg = "";
                    //return this;
                }
                using (var context = new IOASDBEntities())
                {
                    var id = (from d in context.tblProcessGuidelineDetail
                              join tx in context.tblProcessTransactionDetail on d.ProcessGuidelineDetailId equals tx.ProcessGuidelineDetailId
                              join u in context.tblProcessGuidelineUser on d.ProcessGuidelineDetailId equals u.ProcessGuidelineDetailId
                              where d.ProcessGuidelineId == this._processGuideLineId && tx.RefId == this._refId && tx.Clarified == true && tx.ActionStatus == "Clarify"
                              select tx.ProcessTransactionId
                              ).FirstOrDefault();
                    //context.tblProcessTransactionDetail
                    //.Where(PTD => PTD.RefId == this._refId && PTD.ProcessGuidelineDetailId == this._processGuideLineId && PTD.Clarified == true && PTD.ActionStatus == "Clarify")
                    //.Select(R => R.ProcessTransactionId).FirstOrDefault();
                    processTransacionId = id ?? 0;
                    var records = (from PGD in context.tblProcessGuidelineDetail
                                   join PGH in context.tblProcessGuidelineHeader on PGD.ProcessGuidelineId equals PGH.ProcessGuidelineId
                                   join WF in context.tblProcessGuidelineWorkFlow on PGD.ProcessGuidelineDetailId equals WF.ProcessGuidelineDetailId
                                   join F in context.tblFunction on PGH.FunctionId equals F.FunctionId
                                   join PGU in context.tblProcessGuidelineUser on PGD.ProcessGuidelineDetailId equals PGU.ProcessGuidelineDetailId
                                   where PGD.ProcessGuidelineId == this._processGuideLineId
                                   && PGU.UserId == this._userId
                                   select new
                                   {
                                       PGH.ProcessGuidelineId,
                                       PGH.ProcessGuidelineTitle,
                                       PGD.ProcessGuidelineDetailId,
                                       PGU.UserId,
                                       WF.ApproverId,
                                       WF.ApproverLevel,
                                       PGU.StartApprover_level,
                                       PGH.FunctionId
                                   }).ToList();
                    if (records != null && records.Count > 0)
                    {
                        model.ProcessTransactionId = processTransacionId;
                        model.ProcessGuidelineDetailId = records[0].ProcessGuidelineDetailId;
                        model.InitiatedUserId = this._userId;
                        model.InitiatedTS = System.DateTime.Now;
                        model.ProcessSeqNumber = 1;
                        model.ApproverId = this._userId;
                        model.ActionStatus = "Initiated";
                        model.Comments = this.comment != "" ? this.comment : "Initiated";
                        model.RefId = this._refId;
                        model.RefTable = "";
                        model.RefFieldName = this._refFieldName;
                        model.ActionLink = this.actionLink;
                        model.successMethod = this.successMethod;
                        model.failedMethod = this.failedMethod;
                        model.clarifyMethod = this.clarifyMethod;
                        model.RefNumber = this.refNumber;
                        model.FunctionId = this.functionId;
                    }
                    else
                    {
                        this.errorMsg = "Data mismatch in process flow. Please contact administrator.";
                        return this;
                    }

                }

                this.record = InsertProcessTransaction(model);

                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                this.errorMsg = ex.ToString();
                return null;
            }
        }

        public FlowEngine ProcessClarify()
        {
            try
            {
                ProcessEngineModel model = new ProcessEngineModel();
                using (var context = new IOASDBEntities())
                {
                    var records = (from PGD in context.tblProcessGuidelineDetail
                                   join PT in context.tblProcessTransaction on PGD.ProcessGuidelineDetailId equals PT.ProcessGuidelineDetailId
                                   join PTD in context.tblProcessTransactionDetail on PT.ProcessTransactionId equals PTD.ProcessTransactionId
                                   join U in context.tblUser on PT.InitiatedUserId equals U.UserId
                                   where PTD.RefId == this._refId && PGD.ProcessGuidelineId == this._processGuideLineId
                                   orderby PTD.ProcessTransactionDetailId
                                   select new
                                   {
                                       PT.ProcessTransactionId,
                                       PT.ProcessGuidelineDetailId,
                                       PTD.ProcessTransactionDetailId,
                                       PTD.ProcessSeqNumber,
                                       PTD.Approverid,
                                       PTD.ActionStatus,
                                       U.UserName,
                                       U.UserImage,
                                       U.FirstName,
                                       U.LastName,
                                       PTD.RefId
                                   }).ToList();
                    if (records != null && records.Count > 0)
                    {
                        model.ProcessGuidelineDetailId = Convert.ToInt32(records[0].ProcessGuidelineDetailId);
                        model.ProcessTransactionId = Convert.ToInt32(records[0].ProcessTransactionId);
                        model.InitiatedUserId = this._userId;
                        model.InitiatedTS = System.DateTime.Now;
                        model.ProcessSeqNumber = 1;
                        model.ApproverId = this._userId;
                        model.ActionStatus = "Clarified";
                        model.Comments = this.comment != "" ? this.comment : "Clarified";
                        model.RefId = this._refId;
                        model.RefTable = "";
                        model.RefFieldName = this._refFieldName;
                        model.ActionLink = this.actionLink;
                        model.successMethod = this.successMethod;
                        model.failedMethod = this.failedMethod;
                    }

                }

                this.record = InsertProcessTransaction(model);

                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                this.errorMsg = ex.ToString();
                return null;
            }
        }


        public static bool DuplicateEntryValidation (string refNumber , int ProcessGuidelineDetailId)
        {
            try
            {
                var DuplicateEntry="";
                bool retunval = false;
                using (var context = new IOASDBEntities())
                {
                    
                    DuplicateEntry = (from Pt in context.tblProcessTransaction
                                      where  Pt.RefNumber == refNumber && Pt.Closed_F == true && Pt.ProcessGuidelineDetailId==ProcessGuidelineDetailId
                                      select Pt.RefNumber).FirstOrDefault();

                    if (refNumber != DuplicateEntry)
                    {
                        retunval = true;
                    }



                }
                return retunval;


            }

            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
                (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }


        private static ProcessEngineModel InsertProcessTransaction(ProcessEngineModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                   // var  DuplicateEntry = "";
                    tblProcessTransaction trans = new tblProcessTransaction();
                    tblProcessTransactionDetail transDetail = new tblProcessTransactionDetail();
                    if (model.ProcessTransactionId != 0)
                    {
                        trans.ProcessTransactionId = model.ProcessTransactionId;
                        if (model.ActionStatus == "Initiated")
                        {
                            var query = context.tblProcessTransaction.FirstOrDefault(m => m.ProcessTransactionId == model.ProcessTransactionId);
                            if (query != null)
                            {
                                query.InitiatedUserId = model.InitiatedUserId;
                                query.InitiatedTS = DateTime.Now;
                                query.ProcessGuidelineDetailId = model.ProcessGuidelineDetailId;
                                query.Closed_F = false;
                            }
                        }
                    }
                    else
                    {
                        trans.ProcessGuidelineDetailId = model.ProcessGuidelineDetailId;
                        trans.InitiatedUserId = model.InitiatedUserId;
                        trans.InitiatedMacID = model.InitiatedMacID;
                        trans.InitiatedTS = System.DateTime.Now;
                        trans.Closed_F = model.Closed_F;
                        trans.ClosingStatus = model.ClosingStatus;
                        trans.ActionLink = model.ActionLink;
                        trans.successMethod = model.successMethod;
                        trans.failedMethod = model.failedMethod;
                        trans.clarifyMethod = model.clarifyMethod;
                        trans.RefId = model.RefId;
                        trans.RefTable = model.RefTable;
                        trans.RefFieldName = model.RefFieldName;
                        trans.FunctionId = model.FunctionId;
                        trans.RefNumber = model.RefNumber;
                        //rajesh duplication


                        //var DuplicateEntry = DuplicateEntryValidation(model.RefNumber.ToString());

                        if (DuplicateEntryValidation(model.RefNumber.ToString(),model.ProcessGuidelineDetailId))
                        {
                            context.tblProcessTransaction.Add(trans);
                            context.SaveChanges();
                        }
                    }

                    //  Right curly brace}


                transDetail.ProcessTransactionId = trans.ProcessTransactionId;
                transDetail.ProcessGuidelineDetailId = model.ProcessGuidelineDetailId;
                transDetail.ProcessSeqNumber = model.ProcessSeqNumber;
                transDetail.Approverid = model.ApproverId;
                transDetail.ActionStatus = model.ActionStatus;
                transDetail.TransactionTS = System.DateTime.Now;
                transDetail.TransactionIP = model.TransactionIP;
                transDetail.MacID = model.MacID;
                transDetail.RefId = model.RefId;
                transDetail.RefTable = model.RefTable;
                transDetail.RefFieldName = model.RefFieldName;
                transDetail.Comments = model.Comments;
                transDetail.Rejected = false;
                transDetail.Clarified = false;
                context.tblProcessTransactionDetail.Add(transDetail);
                context.SaveChanges();

                    if (model.ActionStatus == "Rejected" || model.ActionStatus == "Clarify")
                    {
                        var details = context.tblProcessTransactionDetail
                            .Where(p => p.RefId == model.RefId && p.ProcessTransactionId == model.ProcessTransactionId
                            && p.Rejected == false && p.Clarified == false).ToList();
                        if (model.ActionStatus == "Rejected")
                        {
                            details.ForEach(p => p.Rejected = true);
                        }
                        if (model.ActionStatus == "Clarify")
                        {
                            details.ForEach(p => p.Clarified = true);
                        }

                        context.SaveChanges();

                    }                   

                    context.Dispose();

                    return model;
                }
                    

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private static ProcessEngineModel CloseProcessTransaction(ProcessEngineModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblProcessTransaction.FirstOrDefault(m => m.ProcessTransactionId == model.ProcessTransactionId);
                    if (query != null)
                    {
                        query.Closed_F = true;
                        context.SaveChanges();
                    }
                }
                return model;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private List<ProcessEngineModel> CheckTransactionFound(int ProcessGuidelineId, int userId, int refId)
        {
            try
            {
                List<ProcessEngineModel> transactions = new List<ProcessEngineModel>();
                var processTrasactionId = -1;
                using (var context = new IOASDBEntities())
                {
                    var records = (from PT in context.tblProcessTransaction
                                   join PTD in context.tblProcessTransactionDetail on PT.ProcessTransactionId equals PTD.ProcessTransactionId
                                   join U in context.tblUser on PT.InitiatedUserId equals U.UserId
                                   join PGD in context.tblProcessGuidelineDetail on PT.ProcessGuidelineDetailId equals PGD.ProcessGuidelineDetailId
                                   where (PGD.ProcessGuidelineId == ProcessGuidelineId && PTD.Approverid == userId && PTD.RefId == refId
                                   && PTD.Rejected == false && PTD.Clarified == false)
                                   orderby PTD.ProcessGuidelineDetailId descending
                                   select new
                                   {
                                       PT.ProcessTransactionId,
                                       PTD.ProcessTransactionDetailId,
                                       PTD.ProcessGuidelineDetailId,
                                       PTD.ProcessSeqNumber,
                                       PTD.Approverid,
                                       PTD.ActionStatus,
                                       PTD.TransactionTS,
                                       PTD.TransactionIP,
                                       PTD.MacID,
                                       U.UserName,
                                       U.UserImage,
                                       U.FirstName,
                                       U.LastName,
                                       PTD.RefId
                                   }).ToList();
                    if (records.Count > 0)
                    {
                        processTrasactionId = records[0].ProcessTransactionId;
                        for (int i = 0; i < records.Count; i++)
                        {
                            transactions.Add(new ProcessEngineModel
                            {
                                ProcessTransactionId = records[i].ProcessTransactionId,
                                ProcessTransactionDetailId = records[i].ProcessTransactionDetailId,
                                ProcessGuidelineDetailId = records[i].ProcessTransactionDetailId,
                                ProcessSeqNumber = Convert.ToInt32(records[i].ProcessSeqNumber),
                                ApproverId = Convert.ToInt32(records[i].Approverid),
                                ActionStatus = Convert.ToString(records[i].ActionStatus),
                                TransactionTS = Convert.ToDateTime(records[i].TransactionTS),
                                TransactionIP = records[i].TransactionIP,
                                MacID = records[i].MacID,
                                UserName = records[i].UserName,
                                UserImage = records[i].UserImage,
                                FirstName = records[i].FirstName,
                                LastName = records[i].LastName,
                                RefId = Convert.ToInt32(records[i].RefId)
                            });

                        }
                    }
                }
                return transactions;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private List<ProcessTransactionDocuments> UpdateDocumentDetails(string uuid)
        {
            try
            {
                var model = new List<ProcessTransactionDocuments>();
                int ProcessTransactionDetailId = this.processTransactionDetailId;
                using (var context = new IOASDBEntities())
                {
                    var processDocs = context.tblProcessTransactionDocuments.Where(doc => doc.UUID == uuid).ToList();
                    processDocs.ForEach(x =>
                    {
                        x.ProcessTransactionDetailId = ProcessTransactionDetailId;
                        x.UPTD_TS = System.DateTime.Now;
                    });
                    context.SaveChanges();
                    context.Dispose();
                    var documents = context.tblProcessTransactionDocuments
                        .Where(doc => doc.ProcessTransactionDetailId == ProcessTransactionDetailId).ToList();

                    for (int i = 0; i < documents.Count; i++)
                    {
                        model.Add(new ProcessTransactionDocuments
                        {
                            DocumentId = Convert.ToInt32(documents[i].DocumentId),
                            DocumentName = documents[i].DocumentName,
                            DocumentPath = documents[i].DocumentPath,
                            IsRequired = Convert.ToBoolean(documents[i].IsRequired),
                            UUID = documents[i].UUID
                        });
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private List<ProcessTransactionDocuments> SaveDocumentDetails(List<ProcessTransactionDocuments> model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {

                    for (int i = 0; i < model.Count; i++)
                    {
                        var processDocs = new tblProcessTransactionDocuments();
                        processDocs.DocumentId = model[i].DocumentId;
                        processDocs.DocumentName = model[i].DocumentName;
                        processDocs.DocumentPath = model[i].DocumentPath;
                        processDocs.IsRequired = model[i].IsRequired;
                        processDocs.UUID = model[i].UUID;
                        processDocs.CRTD_TS = System.DateTime.Now;
                        context.tblProcessTransactionDocuments.Add(processDocs);
                        context.SaveChanges();
                        model[i].ProcessTransactionDocumentId = processDocs.ProcessTransactionDocumentId;
                    }
                    context.Dispose();

                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private List<ProcessGuidelineWorkflowDocument> GetWorkFlowDocumentList(int processGuideLineId, int userId)
        {
            try
            {
                List<ProcessGuidelineWorkflowDocument> model = new List<ProcessGuidelineWorkflowDocument>();
                using (var context = new IOASDBEntities())
                {
                    var records = (from PGD in context.tblProcessGuidelineDetail
                                   join WF in context.tblProcessGuidelineWorkFlow on PGD.ProcessGuidelineDetailId equals WF.ProcessGuidelineDetailId
                                   join WFD in context.tblProcessGuidelineWorkflowDocument on WF.ProcessGuidelineWorkFlowId equals WFD.ProcessGuidelineWorkflowId
                                   where PGD.ProcessGuidelineId == processGuideLineId && WF.ApproverId == userId
                                   select new
                                   {
                                       WFD.ProcessGuidelineWorkflowDocumentId,
                                       WFD.ProcessGuidelineWorkflowId,
                                       WFD.DocumentId,
                                       WFD.DocumentName,
                                       WFD.DocumentType,
                                       WFD.IsRequired
                                   }).ToList();
                    if (records != null && records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new ProcessGuidelineWorkflowDocument
                            {
                                ProcessGuidelineWorkflowId = Convert.ToInt32(records[i].ProcessGuidelineWorkflowId),
                                ProcessGuidelineWorkflowDocumentId = Convert.ToInt32(records[i].ProcessGuidelineWorkflowDocumentId),
                                DocumentId = Convert.ToInt32(records[i].DocumentId),
                                DocumentName = records[i].DocumentName,
                                DocumentType = records[i].DocumentType,
                                IsRequired = Convert.ToBoolean(records[i].IsRequired)
                            });
                        }
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public FlowEngine GetDocuments()
        {
            this.workFlowDocuments = GetWorkFlowDocumentList(this._processGuideLineId, this._userId);
            return this;
        }


        public FlowEngine SaveDocuments(List<ProcessTransactionDocuments> model)
        {
            this.documents = SaveDocumentDetails(model);
            return this;
        }

        public FlowEngine UpdateDocuments(string uuid)
        {
            this.documents = UpdateDocumentDetails(uuid);
            return this;
        }

        public FlowEngine Approve()
        {
            try
            {
                ProcessEngineModel model = new ProcessEngineModel();
                var validate = this.IsApproved();
                if (this.nextApprover != this._userId)
                {
                    this.errorMsg = "Previous approval is pending";
                    return this;
                }
                if (!this.canApprove)
                {
                    this.errorMsg = "Sorry you can not perform this action.";
                    return this;
                }
                if (validate.isApproved)
                {
                    return this;
                }
                var clarify = this.IsClarified();

                if (clarify.isClarified)
                {
                    return this;
                }
                var reject = this.IsRejected();
                if (reject.isRejected)
                {
                    return this;
                }
                int funcId = 0;
                using (var context = new IOASDBEntities())
                {
                    var records = (from PGD in context.tblProcessGuidelineDetail
                                   join PGH in context.tblProcessGuidelineHeader on PGD.ProcessGuidelineId equals PGH.ProcessGuidelineId
                                   join WF in context.tblProcessGuidelineWorkFlow on PGD.ProcessGuidelineDetailId equals WF.ProcessGuidelineDetailId
                                   //join F in context.tblFunction on PGH.FunctionId equals F.FunctionId
                                   join PGU in context.tblProcessGuidelineUser on PGD.ProcessGuidelineDetailId equals PGU.ProcessGuidelineDetailId
                                   //join PTD in context.tblProcessTransactionDetail on PGD.ProcessGuidelineDetailId equals PTD.ProcessGuidelineDetailId
                                   join PT in context.tblProcessTransaction on PGD.ProcessGuidelineDetailId equals PT.ProcessGuidelineDetailId
                                   where PGD.ProcessGuidelineId == this._processGuideLineId && PT.RefId == this._refId
                                   select new
                                   {
                                       PGH.ProcessGuidelineId,
                                       PGH.ProcessGuidelineTitle,
                                       PGD.ProcessGuidelineDetailId,
                                       PT.ProcessTransactionId,
                                       PGU.UserId,
                                       PGU.StartApprover_level,
                                       WF.ApproverId,
                                       WF.ApproverLevel,
                                       PT.FunctionId
                                   }).ToList();

                    var ProcessSeqNumber = (from PTD in context.tblProcessTransactionDetail
                                            join PGD in context.tblProcessGuidelineDetail on PTD.ProcessGuidelineDetailId equals PGD.ProcessGuidelineDetailId
                                            where PGD.ProcessGuidelineId == this._processGuideLineId && PTD.RefId == this._refId
                                            select new { PTD.ProcessTransactionId, PTD.ProcessSeqNumber, PTD.RefId }).Max(x => x.ProcessSeqNumber);

                    if (records != null && records.Count > 0)
                    {
                        model.ProcessTransactionId = Convert.ToInt32(records[0].ProcessTransactionId);
                        model.ProcessGuidelineDetailId = records[0].ProcessGuidelineDetailId;
                        model.InitiatedUserId = this._userId;
                        model.InitiatedTS = System.DateTime.Now;
                        model.ProcessSeqNumber = Convert.ToInt32(ProcessSeqNumber) != 0 ? Convert.ToInt32(ProcessSeqNumber) + 1 : 1;
                        model.ApproverId = this._userId;
                        model.ActionStatus = "Approved";
                        model.Comments = this.comment != "" ? this.comment : "Approved";
                        model.RefId = this._refId;
                        model.RefTable = "";
                        model.RefFieldName = this._refFieldName;
                        funcId = Convert.ToInt32(records[0].FunctionId);
                    }

                }
                this.record = InsertProcessTransaction(model);
                this.IsLastApprover();
                if (this.isLastApprover && this.successMethod != "")
                {
                    //this.errorMsg = "";
                    CloseProcessTransaction(model);
                    string methodName = this.successMethod;
                    Type type = typeof(ProcessSuccessService);
                    MethodInfo method = type.GetMethod(this.successMethod);
                    //ProcessCompleteDelegate pro = method.GetType();
                    //ParameterInfo param = method.GetParameters();
                    ProcessSuccessService pss = new ProcessSuccessService();
                    ProcessCompleteDelegate pi;
                    pi = (ProcessCompleteDelegate)Delegate.CreateDelegate(typeof(ProcessCompleteDelegate), pss, methodName, false);                    
                    var status = pi(this._refId, this._userId);
                    if (!status)
                    {
                        CoreAccountsService _cs = new CoreAccountsService();
                        this.errorMsg = "Something went wrong, Please try after some times.";
                        _cs.RollBackLastApproved(this._refId, funcId);
                    }
                    //Type[] methodArgs = { typeof(int), typeof(int) };
                    //DynamicMethod invokeSuccess = new DynamicMethod(this.successMethod, typeof(bool), methodArgs, typeof(ProcessSuccessService).Module);
                    //ILGenerator il = invokeSuccess.GetILGenerator();
                    //il.Emit(OpCodes.Ldarg_0);
                    //il.Emit(OpCodes.Conv_I8);
                    //il.Emit(OpCodes.Dup);
                    //il.Emit(OpCodes.Mul);
                    //il.Emit(OpCodes.Ret);

                    //ProcessSuccessService complete = new ProcessSuccessService();
                    //complete.TADWFInitSuccess(this._refId, this._userId);
                }
                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                this.errorMsg = ex.ToString();
                return null;
            }
        }

        public FlowEngine Reject()
        {
            try
            {
                ProcessEngineModel model = new ProcessEngineModel();
                if (this.nextApprover != this._userId)
                {
                    this.errorMsg = "Previous approval is pending";
                    return this;
                }
                if (!this.canReject)
                {
                    this.errorMsg = "Sorry you can not perform this action.";
                    return this;
                }
                var validate = this.IsApproved();
                if (validate.isApproved)
                {
                    return this;
                }
                var clarify = this.IsClarified();

                if (clarify.isClarified)
                {
                    return this;
                }
                var reject = this.IsRejected();
                if (reject.isRejected)
                {
                    return this;
                }
                int funcId = 0;
                using (var context = new IOASDBEntities())
                {
                    var records = (from PGD in context.tblProcessGuidelineDetail
                                   join PGH in context.tblProcessGuidelineHeader on PGD.ProcessGuidelineId equals PGH.ProcessGuidelineId
                                   join WF in context.tblProcessGuidelineWorkFlow on PGD.ProcessGuidelineDetailId equals WF.ProcessGuidelineDetailId
                                   //join F in context.tblFunction on PGH.FunctionId equals F.FunctionId
                                   join PGU in context.tblProcessGuidelineUser on PGD.ProcessGuidelineDetailId equals PGU.ProcessGuidelineDetailId
                                   join PTD in context.tblProcessTransactionDetail on PGD.ProcessGuidelineDetailId equals PTD.ProcessGuidelineDetailId
                                   where PGD.ProcessGuidelineId == this._processGuideLineId && PTD.RefId == this._refId
                                   select new
                                   {
                                       PGH.ProcessGuidelineId,
                                       PGH.ProcessGuidelineTitle,
                                       PGD.ProcessGuidelineDetailId,
                                       PTD.ProcessTransactionId,
                                       PGU.UserId,
                                       PGU.StartApprover_level,
                                       WF.ApproverId,
                                       WF.ApproverLevel,
                                       PGH.FunctionId
                                   }).ToList();

                    var ProcessSeqNumber = (from PTD in context.tblProcessTransactionDetail
                                            join PGD in context.tblProcessGuidelineDetail on PTD.ProcessGuidelineDetailId equals PGD.ProcessGuidelineDetailId
                                            where PGD.ProcessGuidelineId == this._processGuideLineId && PTD.RefId == this._refId
                                            select new { PTD.ProcessTransactionId, PTD.ProcessSeqNumber, PTD.RefId }).Max(x => x.ProcessSeqNumber);

                    if (records != null && records.Count > 0)
                    {
                        
                        model.ProcessTransactionId = Convert.ToInt32(records[0].ProcessTransactionId);
                        model.ProcessGuidelineDetailId = records[0].ProcessGuidelineDetailId;
                        model.InitiatedUserId = this._userId;
                        model.InitiatedTS = System.DateTime.Now;
                        model.ProcessSeqNumber = Convert.ToInt32(ProcessSeqNumber) != 0 ? Convert.ToInt32(ProcessSeqNumber) + 1 : 1;
                        model.ApproverId = this._userId;
                        model.ActionStatus = "Rejected";
                        model.Comments = this.comment != "" ? this.comment : "Rejected";
                        model.RefId = this._refId;
                        model.RefTable = "";
                        model.RefFieldName = this._refFieldName;
                        if (model.ProcessTransactionId > 0)
                        {
                            
                            funcId = context.tblProcessTransaction.Where(x => x.ProcessTransactionId == model.ProcessTransactionId).Select(x => x.FunctionId).FirstOrDefault()??0;
                            //funcId = Convert.ToInt32(records[0].FunctionId);
                        }
                    }

                }
                //processTrasactionId = InsertProcessTransaction(model);
                this.record = InsertProcessTransaction(model);
                //DynamicMethod callFailed = new DynamicMethod(this.failedMethod, typeof(bool), methodArgs, typeof().Module);
                if (this.failedMethod != "")
                {
                    string methodName = this.failedMethod;
                    Type type = typeof(ProcessFailureService);
                    MethodInfo method = type.GetMethod(this.failedMethod);
                    ProcessFailureService pfs = new ProcessFailureService();
                    ProcessFailedDelegate pf;
                    pf = (ProcessFailedDelegate)Delegate.CreateDelegate(typeof(ProcessFailedDelegate), pfs, methodName, false);
                    var status = pf(this._refId, this._userId);
                    if (!status)
                    {
                        CoreAccountsService _cs = new CoreAccountsService();
                        this.errorMsg = "Something went wrong, Please try after some times.";
                        _cs.RollBackLastApproved(this._refId, funcId, false, true);
                    }
                }
                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                this.errorMsg = ex.ToString();
                return null;
            }
        }

        public FlowEngine Clarify()
        {
            try
            {
                ProcessEngineModel model = new ProcessEngineModel();
                if (this.nextApprover != this._userId)
                {
                    this.errorMsg = "Previous approval is pending";
                    return this;
                }
                if (!this.canClarify)
                {
                    this.errorMsg = "Sorry you can not perform this action.";
                    return this;
                }
                var validate = this.IsApproved();
                if (validate.isApproved)
                {
                    return this;
                }
                var clarify = this.IsClarified();

                if (clarify.isClarified)
                {
                    return this;
                }
                var reject = this.IsRejected();
                if (reject.isRejected)
                {
                    return this;
                }
                int funcId = 0;
                using (var context = new IOASDBEntities())
                {
                    var records = (from PGD in context.tblProcessGuidelineDetail
                                   join PGH in context.tblProcessGuidelineHeader on PGD.ProcessGuidelineId equals PGH.ProcessGuidelineId
                                   join WF in context.tblProcessGuidelineWorkFlow on PGD.ProcessGuidelineDetailId equals WF.ProcessGuidelineDetailId
                                   //join F in context.tblFunction on PGH.FunctionId equals F.FunctionId
                                   join PGU in context.tblProcessGuidelineUser on PGD.ProcessGuidelineDetailId equals PGU.ProcessGuidelineDetailId
                                   join PTD in context.tblProcessTransactionDetail on PGD.ProcessGuidelineDetailId equals PTD.ProcessGuidelineDetailId
                                   where PGD.ProcessGuidelineId == this._processGuideLineId && PTD.RefId == this._refId
                                   select new
                                   {
                                       PGH.ProcessGuidelineId,
                                       PGH.ProcessGuidelineTitle,
                                       PGD.ProcessGuidelineDetailId,
                                       PTD.ProcessTransactionId,
                                       PGU.UserId,
                                       PGU.StartApprover_level,
                                       WF.ApproverId,
                                       WF.ApproverLevel,
                                       PGH.FunctionId
                                   }).ToList();

                    var ProcessSeqNumber = (from PTD in context.tblProcessTransactionDetail
                                            join PGD in context.tblProcessGuidelineDetail on PTD.ProcessGuidelineDetailId equals PGD.ProcessGuidelineDetailId
                                            where PGD.ProcessGuidelineId == this._processGuideLineId && PTD.RefId == this._refId
                                            select new { PTD.ProcessTransactionId, PTD.ProcessSeqNumber, PTD.RefId }).Max(x => x.ProcessSeqNumber);

                    if (records != null && records.Count > 0)
                    {
                        model.ProcessTransactionId = Convert.ToInt32(records[0].ProcessTransactionId);
                        model.ProcessGuidelineDetailId = records[0].ProcessGuidelineDetailId;
                        model.InitiatedUserId = this._userId;
                        model.InitiatedTS = System.DateTime.Now;
                        model.ProcessSeqNumber = Convert.ToInt32(ProcessSeqNumber) != 0 ? Convert.ToInt32(ProcessSeqNumber) + 1 : 1;
                        model.ApproverId = this._userId;
                        model.ActionStatus = "Clarify";
                        model.Comments = this.comment != "" ? this.comment : "Clarify";
                        model.RefId = this._refId;
                        model.RefTable = "";
                        model.RefFieldName = this._refFieldName;
                        if (model.ProcessTransactionId > 0)
                        {

                            funcId = context.tblProcessTransaction.Where(x => x.ProcessTransactionId == model.ProcessTransactionId).Select(x => x.FunctionId).FirstOrDefault() ?? 0;
                            //funcId = Convert.ToInt32(records[0].FunctionId);
                        }
                    }

                }
                //processTrasactionId = InsertProcessTransaction(model);
                this.record = InsertProcessTransaction(model);
                if (this.clarifyMethod != "")
                {
                    string methodName = this.clarifyMethod;
                    Type type = typeof(ProcessClarifyService);
                    MethodInfo method = type.GetMethod(this.clarifyMethod);
                    ProcessClarifyService pfs = new ProcessClarifyService();
                    ProcessClarifyDelegate pf;
                    pf = (ProcessClarifyDelegate)Delegate.CreateDelegate(typeof(ProcessClarifyDelegate), pfs, methodName, false);
                    var status = pf(this._refId, this._userId);
                    if (!status)
                    {
                        CoreAccountsService _cs = new CoreAccountsService();
                        this.errorMsg = "Something went wrong, Please try after some times.";
                        _cs.RollBackLastApproved(this._refId, funcId, true, false);
                    }
                }
                return this;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                this.errorMsg = ex.ToString();
                return null;
            }
        }
    }
}