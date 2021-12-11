using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using DataAccessLayer;
using IOAS.GenericServices;
using IOAS.Models;


namespace IOAS.Controllers
{
    public class ProcessEngineController : Controller
    {

        ProcessEngine db = new ProcessEngine();
        ProcessEngineService processService = new ProcessEngineService();
        // GET: ProcessEngine
        public ActionResult Index()
        {
            return View();
        }


        [Authorize]
        public ActionResult ApproveList()
        {
            try
            {
                int pageSize = 10;
                int page = 1;
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);

                var model = ProcessEngineService.GetApproveList(userId, page, pageSize);
                model.CurrentPage = page;
                model.pageSize = pageSize;
                model.visiblePages = 5;
                //var ApproveList = new PagedData<ProcessEngineModel>();
                //var ApproveList = new PagedData<ProcessEngineModel>();
                //ApproveList.Data = model;
                //ApproveList.CurrentPage = page;
                //ApproveList.pageSize = pageSize;
                //ApproveList.visiblePages = 5;
                //ApproveList.TotalPages = Convert.ToInt32(Math.Ceiling((double)model.Count / pageSize));

                return View(model);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult ApproveList(int page)
        {
            try
            {
                int pageSize = 10;

                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);

                var model = ProcessEngineService.GetApproveList(userId, page, pageSize);
                model.CurrentPage = page;
                model.pageSize = pageSize;
                model.visiblePages = 5;
                //var ApproveList = new PagedData<ProcessEngineModel>();
                //ApproveList.Data = model;
                //ApproveList.CurrentPage = page;
                //ApproveList.pageSize = pageSize;
                //ApproveList.TotalPages = Convert.ToInt32(Math.Ceiling((double)model.Count / pageSize));

                return View(model);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetApproveList(int page)
        {
            try
            {
                int pageSize = 10;

                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);

                var model = ProcessEngineService.GetApproveList(userId, page, pageSize);
                model.CurrentPage = page;
                model.pageSize = pageSize;
                model.visiblePages = 5;

                return View("_ApprovalData", model);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult ProcessFlow(int processGuideLineId, int refId)
        {
            try
            {
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                //List<ProcessEngineModel> model = new List<ProcessEngineModel>();
                //model = ProcessEngineService.GetProcessStatusByGuidelineId(processGuideLineId);
                var transactionId = -1;
                var pendingApproval = ProcessEngineService.GetProcessFlowByUser(processGuideLineId, userId);

                if(pendingApproval != null && pendingApproval.Count > 0)
                {
                    transactionId = pendingApproval[0].ProcessTransactionId;
                    //refId = pendingApproval[0].RefId;
                }

                var transaction = ProcessEngineService.GetProcessStatusByUser(processGuideLineId, userId, transactionId);

                ProcessEngine db = new ProcessEngine();
                DataSet dsTransaction = db.GetProcessFlowByUser(processGuideLineId, userId, refId);
                DataTable dtProcessFlow = dsTransaction.Tables[1];
                int currentApprover = 0;
                if (dtProcessFlow.Rows.Count > 0)
                {
                    foreach (DataRow row in dtProcessFlow.Rows)
                    {
                        object value = row["ProcessTransactionDetailId"];
                        if (value == DBNull.Value)
                        {
                            currentApprover = Convert.ToInt32(row["ApproverId"].ToString());
                            break;
                        }
                    }
                }
                var resultJson = new { transaction = transaction,
                    pending = pendingApproval,
                    currentApprover = currentApprover,
                    currentUser = userId
                };

                return Json(resultJson, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult LoadProcessFlowTransaction(int processGuideLineId, int refId)
        {
            try
            {
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                //userId = 1;

                //var transaction = ProcessEngineService.GetProcessStatusByUser(processGuideLineId, userId, refId);
                ProcessEngine db = new ProcessEngine();
                DataSet dsTransaction = db.GetProcessFlowByUser(processGuideLineId, userId, refId);
                DataTable dtProcessFlow = dsTransaction.Tables[1];
                DataTable dtHistory = dsTransaction.Tables[2];
                var transaction = Converter.GetEntityList<ProcessEngineModel>(dtProcessFlow);
                var history = Converter.GetEntityList<ProcessEngineModel>(dtHistory);
                bool approve = false;
                bool reject = false;
                bool clarify = false;
                int currentApprover = 0;
                if (dtProcessFlow.Rows.Count > 0)
                {
                    foreach (DataRow row in dtProcessFlow.Rows)
                    {
                        object value = row["ProcessTransactionDetailId"];
                        if (value == DBNull.Value)
                        {
                            currentApprover = Convert.ToInt32(row["ApproverId"].ToString());
                            approve = Convert.ToBoolean(row["Approve_f"].ToString());
                            reject = Convert.ToBoolean(row["Reject_f"].ToString());
                            clarify = Convert.ToBoolean(row["Clarify_f"].ToString());
                            break;
                        }
                    }
                }
                var resultJson = new
                {
                    transaction = transaction,
                    comments = history,
                    currentApprover = currentApprover,
                    currentUser = userId,
                    approve = approve,
                    reject = reject,
                    clarify = clarify
                };
                return Json(resultJson, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        //[Authorize]
        //[HttpPost]
        //public JsonResult ProcessTransaction(int processGuideLineId)
        //{
        //    try
        //    {
        //        var user = User.Identity.Name;
        //        var userId = AdminService.getUserByName(user);
        //        //List<ProcessEngineModel> flows = processService.GetProcessFlowByUser(processGuideLineId, userId);
        //        var err = new { reason = "Bad Reqeust" };

        //        List<ProcessEngineModel> process = new List<ProcessEngineModel>();

        //        ProcessEngineModel model = new ProcessEngineModel();

        //        return Json(model, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [Authorize]
        [HttpPost]
        public JsonResult UpdateProcessStatus(ProcessEngineModel model, int processGuideLineId)
        {
            try
            {
                int refId;
                string refFieldName;
                string msg = "", errorMsg = "";
                if (model.RefId > 0)
                {
                    refId = model.RefId;

                    var userId = AdminService.getUserByName(User.Identity.Name);
                    //userId = 1;

                    var record = ProcessEngineService.GetRecordByRefId(processGuideLineId, userId, refId);
                    refFieldName = model.RefFieldName;

                    var flowEngine = FlowEngine.Init(processGuideLineId, userId, refId, refFieldName);

                    string ActionStatus = (model.ActionStatus != null) ? model.ActionStatus.ToLower() : "";
                    flowEngine.Comment(model.Comments);
                    switch (ActionStatus)
                    {
                        case "approve":
                        case "recommend":
                        case "complete":
                            msg = "Approved successfully";
                            flowEngine.Approve();
                            break;
                        case "reject":
                            msg = "You've rejected";
                            flowEngine.Reject();
                            break;
                        case "clarify":
                            msg = "Send for clarification";
                            flowEngine.Clarify();
                            break;
                        default:
                            msg = "Sorry invalid action.";
                            break;
                    }
                    if(flowEngine.errorMsg != "" && flowEngine.errorMsg != null)
                    {
                        errorMsg = flowEngine.errorMsg;
                        msg = flowEngine.errorMsg;
                    }
                }
                else
                {
                    msg = "Something went wrong";
                }
                var result = new { result = msg, error = errorMsg };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

    }
}