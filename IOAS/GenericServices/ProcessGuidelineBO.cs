using System;
using System.Collections.Generic;
using System.Linq;
using IOAS.Models.Process;
using IOAS.DataModel;
using System.Data;


namespace IOAS.GenericServices.Process
{
    public class ProcessGuidelineBO
    {
        public static ControlDataList LoadControls()
        {
            ControlDataList objcbl = new ControlDataList();

            using (var context = new IOASDBEntities())
            {
                var queryfunction = (from F in context.tblFunction
                                     orderby F.FunctionName
                                     select F).ToList();
                if (queryfunction.Count > 0)
                {
                    objcbl.FunctionList = new List<Function>();
                    for (int i = 0; i < queryfunction.Count; i++)
                    {
                        objcbl.FunctionList.Add(new Function()
                        {
                            FunctionId = (Int32)queryfunction[i].FunctionId,
                            FunctionName = Convert.ToString(queryfunction[i].FunctionName)
                        });
                    }
                }

                var queryApprover = (from U in context.tblUser
                                     where U.Status == "Active"
                                     orderby U.UserName
                                     select U).ToList();
                if (queryApprover.Count > 0)
                {
                    objcbl.ApproverList = new List<Approver>();
                    for (int i = 0; i < queryApprover.Count; i++)
                    {
                        objcbl.ApproverList.Add(new Approver()
                        {
                            ApproverId = (Int32)queryApprover[i].UserId,
                            ApproverName = Convert.ToString(queryApprover[i].UserName) + "-" + queryApprover[i].FirstName + " " + queryApprover[i].LastName
                        });
                    }
                }

                var queryStatus = (from F in context.tblFunctionStatus
                                   where F.FunctionId == 8
                                   orderby F.Status
                                   select F).ToList();
                if (queryStatus.Count > 0)
                {
                    objcbl.StatusList = new List<ProcessFlowStatus>();
                    for (int i = 0; i < queryStatus.Count; i++)
                    {
                        objcbl.StatusList.Add(new ProcessFlowStatus()
                        {
                            StatusId = (Int32)queryStatus[i].FunctionStatusId,
                            StatusName = Convert.ToString(queryStatus[i].Status)
                        });
                    }
                }

                var queryDocument = (from D in context.tblDocument
                                     orderby D.DocumentName
                                     select D).ToList();
                if (queryDocument.Count > 0)
                {
                    objcbl.DocumentList = new List<Document>();
                    for (int i = 0; i < queryDocument.Count; i++)
                    {
                        objcbl.DocumentList.Add(new Document()
                        {
                            DocumentId = (Int32)queryDocument[i].DocumentId,
                            DocumentName = Convert.ToString(queryDocument[i].DocumentName)
                        });
                    }
                }
                var query = (from A in context.tblCodeControl
                             where A.CodeName == "ApproverCategory"
                             select new { A.CodeValAbbr, A.CodeValDetail }).ToList();
                if (query.Count > 0)
                {
                    objcbl.ApproverType = new List<ApproverCategoryModel>();
                    for (int i = 0; i < query.Count; i++)
                    {
                        objcbl.ApproverType.Add(new ApproverCategoryModel()
                        {
                            ApproverFlagId = query[i].CodeValAbbr,
                            ApproverFlagname = query[i].CodeValDetail
                        });
                    }

                }
            }

            return objcbl;
        }

        public static List<ProcessFlowModel> GetProcessFlowList(int processGuidelineId)
        {
            List<ProcessFlowModel> processFlowDetails = new List<ProcessFlowModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from P in context.tblProcessGuidelineDetail
                             where P.ProcessGuidelineId == processGuidelineId
                             orderby P.ProcessGuidelineDetailId
                             select P).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        int pgDetailId = (Int32)query[i].ProcessGuidelineDetailId;
                        var userCount = (from P in context.tblProcessGuidelineUser where P.ProcessGuidelineDetailId == pgDetailId select P).ToList();
                        processFlowDetails.Add(new ProcessFlowModel()
                        {
                            ProcessGuidelineDetailId = (Int32)query[i].ProcessGuidelineDetailId,
                            FlowTitle = Convert.ToString(query[i].FlowTitle),
                            UserCount = userCount.Count.ToString()
                        });
                    }
                }
            }
            return processFlowDetails;
        }
        public static ProcessFlowModel GetUserCount(int pgDetailId)
        {
            try
            {
                ProcessFlowModel Process = new ProcessFlowModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProcessGuidelineUser where P.ProcessGuidelineDetailId == pgDetailId select P).ToList();
                    var flowTitle = (from FT in context.tblProcessGuidelineDetail where FT.ProcessGuidelineDetailId == pgDetailId select FT.FlowTitle).FirstOrDefault();
                    var approverCount = (from AC in context.tblProcessGuidelineWorkFlow where AC.ProcessGuidelineDetailId == pgDetailId select AC).ToList();
                    Process.FlowTitle = flowTitle;
                    Process.UserCount = query.Count.ToString();
                    Process.ApproverCount = approverCount.Count.ToString();
                }
                return Process;
            }
            catch (Exception ex)
            {
                ProcessFlowModel Process = new ProcessFlowModel();
                return Process;
            }
        }
        public static int AddProcessFlow(ProcessFlowModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var chkflowname = context.tblProcessGuidelineDetail.Where(m => m.FlowTitle == model.FlowTitle && m.ProcessGuidelineId == model.ProcessGuidelineid).FirstOrDefault();
                    if (chkflowname != null)
                        return -2;
                    tblProcessGuidelineDetail objIU = new tblProcessGuidelineDetail();
                    objIU.ProcessGuidelineId = model.ProcessGuidelineid;
                    objIU.FlowTitle = model.FlowTitle;
                    objIU.CreatedTS = DateTime.Now;
                    objIU.CreatedUserId = model.UserId;
                    context.tblProcessGuidelineDetail.Add(objIU);
                    context.SaveChanges();
                    context.Dispose();
                    return objIU.ProcessGuidelineDetailId;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static int UpdateProcessFlow(ProcessFlowModel model)
        {
            tblProcessGuidelineDetail pgd;
            using (var context = new IOASDBEntities())
            {
                pgd = context.tblProcessGuidelineDetail.Where(s => s.ProcessGuidelineDetailId == model.ProcessGuidelineDetailId).FirstOrDefault();
            }
            if (pgd != null)
            {
                pgd.FlowTitle = model.FlowTitle;
                pgd.LastUpdatedTS = DateTime.Now;
                pgd.LastUpdatedUserId = model.UserId;
            }
            using (var dbCtx = new IOASDBEntities())
            {
                dbCtx.Entry(pgd).State = System.Data.Entity.EntityState.Modified;
                dbCtx.SaveChanges();
            }
            return model.ProcessGuidelineDetailId;
        }

        public static int DeleteProcessFlow(int processGuidelineDetailId)
        {
            try
            {
                tblProcessGuidelineDetail pglDetail;
                int rowsAffected = 0;
                using (var context = new IOASDBEntities())
                {
                    //Delete Users
                    context.tblProcessGuidelineUser.RemoveRange(context.tblProcessGuidelineUser.Where(x => x.ProcessGuidelineDetailId == processGuidelineDetailId));

                    var pglWF = from WF in context.tblProcessGuidelineWorkFlow
                                join D in context.tblProcessGuidelineDocument on WF.ProcessGuidelineWorkFlowId equals D.WorkflowId
                                where WF.ProcessGuidelineDetailId == processGuidelineDetailId
                                select WF;

                    //Delete Documents
                    foreach (var record in pglWF)
                    {
                        context.tblProcessGuidelineDocument.RemoveRange(context.tblProcessGuidelineDocument.Where(x => x.WorkflowId == record.ProcessGuidelineWorkFlowId));
                    }

                    //Delete Workflow
                    context.tblProcessGuidelineWorkFlow.RemoveRange(context.tblProcessGuidelineWorkFlow.Where(x => x.ProcessGuidelineDetailId == processGuidelineDetailId));

                    //Delete Detail
                    pglDetail = context.tblProcessGuidelineDetail.Where(s => s.ProcessGuidelineDetailId == processGuidelineDetailId).FirstOrDefault();
                    context.Entry(pglDetail).State = System.Data.Entity.EntityState.Deleted;

                    rowsAffected = context.SaveChanges();
                }
                return rowsAffected;
            }
            catch
            {
                return -1;
            }
        }


        public static List<ProcessFlowUser> GetProcessFlowUserDetails(int processGuidelineDetailId)
        {
            List<ProcessFlowUser> processFlowUserDetails = new List<ProcessFlowUser>();
            using (var context = new IOASDBEntities())
            {
                var query = (from D in context.tblProcessGuidelineDetail
                             from U in context.tblUser
                             join PU in context.tblProcessGuidelineUser on new { DetailId = (int?)D.ProcessGuidelineDetailId, UserId = (int?)U.UserId } equals new { DetailId = PU.ProcessGuidelineDetailId, UserId = PU.UserId } into temp
                             from PUG in temp.DefaultIfEmpty()
                             where (D.ProcessGuidelineDetailId == processGuidelineDetailId && U.Status == "Active" && U.RoleId != 7)
                             orderby U.UserId
                             select new
                             {
                                 UserId = U.UserId
                                ,
                                 UserName = U.UserName
                                ,
                                 UserFlag = (PUG.UserId == null ? 0 : 1)
                                ,
                                 ProcessGuidelineDetailId = D.ProcessGuidelineDetailId//pgd.ProcessGuidelineDetailId
                                 ,
                                 UserwiseName = U.FirstName + " " + U.LastName
                             }
                             ).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        processFlowUserDetails.Add(new ProcessFlowUser()
                        {
                            ProcessGuidelineDetailId = (Int32)query[i].ProcessGuidelineDetailId,
                            UserId = Convert.ToInt32(query[i].UserId),
                            UserName = Convert.ToString(query[i].UserName) + "-" + query[i].UserwiseName,
                            UserFlag = Convert.ToInt32(query[i].UserFlag) == 0 ? false : true
                        });
                    }
                }
            }
            return processFlowUserDetails;
        }


        public static List<ProcessFlowUser> GetApproverList()
        {
            List<ProcessFlowUser> approvalList = new List<ProcessFlowUser>();
            using (var context = new IOASDBEntities())
            {
                var query = (from U in context.tblUser where U.ApproverF == true orderby U.UserId select U).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        approvalList.Add(new ProcessFlowUser()
                        {
                            UserId = Convert.ToInt32(query[i].UserId),
                            UserName = Convert.ToString(query[i].UserName) + "-" + query[i].FirstName + " " + query[i].LastName
                        });
                    }
                }
            }
            return approvalList;
        }
        public static List<ProcessFlowStatus> GetStatus()
        {
            List<ProcessFlowStatus> statusList = new List<ProcessFlowStatus>();
            using (var context = new IOASDBEntities())
            {
                var query = (from S in context.tblFunctionStatus orderby S.FunctionStatusId select S).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        statusList.Add(new ProcessFlowStatus()
                        {
                            StatusId = Convert.ToInt32(query[i].FunctionStatusId),
                            StatusName = Convert.ToString(query[i].Status)
                        });
                    }
                }
            }
            return statusList;
        }

        public static int AddApproverDetails(ProcessFlowApproverList model)
        {


            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int rowsAffected = 0, pglWorkflowId = 0;
                        if (model.ProcessguidlineworkflowId == 0)
                        {
                            tblProcessGuidelineWorkFlow objIU = new tblProcessGuidelineWorkFlow();
                            objIU.ProcessGuidelineId = model.processguidlineId;
                            objIU.ProcessGuidelineDetailId = model.ProcessGuidelineDetailId;
                            objIU.StatusId = model.StatusId;
                            objIU.ApproverId = model.UserId;
                            objIU.ApproverLevel = model.ApproverLevel;
                            objIU.Approve_f = model.ApproveFlag;
                            objIU.Reject_f = model.RejectFlag;
                            objIU.Clarify_f = model.ClarifyFlag;
                            objIU.Mark_f = model.MarkFlag;
                            //foreach (int item in model.ApproverList)
                            //{

                            //        if (item == 1)
                            //        {
                            //            objIU.Approve_f = true;
                            //        }
                            //        else if (item == 2)
                            //        {
                            //            objIU.Reject_f = true;
                            //        }
                            //        else if (item == 3)
                            //        {
                            //            objIU.Clarify_f = true;
                            //        }
                            //        else if (item == 4)
                            //        {
                            //            objIU.Mark_f = true;
                            //        }
                            //    }

                            context.tblProcessGuidelineWorkFlow.Add(objIU);
                            rowsAffected = context.SaveChanges();
                            pglWorkflowId = objIU.ProcessGuidelineWorkFlowId;
                        }
                        else
                        {
                            tblProcessGuidelineWorkFlow objupdate = new tblProcessGuidelineWorkFlow();
                            objupdate = context.tblProcessGuidelineWorkFlow.Where(M => M.ProcessGuidelineWorkFlowId == model.ProcessguidlineworkflowId).FirstOrDefault();
                            if (objupdate != null)
                            {
                                objupdate.ProcessGuidelineId = model.processguidlineId;
                                objupdate.ProcessGuidelineDetailId = model.ProcessGuidelineDetailId;
                                objupdate.StatusId = model.StatusId;
                                objupdate.ApproverId = model.UserId;
                                objupdate.ApproverLevel = model.ApproverLevel;
                                objupdate.Approve_f = model.ApproveFlag;
                                objupdate.Reject_f = model.RejectFlag;
                                objupdate.Clarify_f = model.ClarifyFlag;
                                objupdate.Mark_f = model.MarkFlag;
                                //foreach (string doc in model.ApproverList)
                                //{
                                //    var appval = context.tblCodeControl.Where(M => M.CodeValDetail == doc && M.CodeName == "ApproverCategory").FirstOrDefault();
                                //    if (appval != null)
                                //    {
                                //        if (appval.CodeValAbbr == 1)
                                //        {
                                //            objupdate.Approve_f = true;
                                //        }
                                //        else if (appval.CodeValAbbr == 2)
                                //        {
                                //            objupdate.Reject_f = true;
                                //        }
                                //        else if (appval.CodeValAbbr == 3)
                                //        {
                                //            objupdate.Clarify_f = true;
                                //        }
                                //        else if (appval.CodeValAbbr == 4)
                                //        {
                                //            objupdate.Mark_f = true;
                                //        }
                                //    }
                                //}
                                rowsAffected = context.SaveChanges();
                                pglWorkflowId = objupdate.ProcessGuidelineWorkFlowId;
                            }
                        }
                        ////Document insertion
                        //if ((model.ProcessguidlineworkflowId == 0 && rowsAffected > 0) || model.ProcessguidlineworkflowId > 0)
                        //{
                        //    //Remove existing document before insert new documents   
                        //    context.tblProcessGuidelineDocument.RemoveRange(context.tblProcessGuidelineDocument.Where(x => x.WorkflowId == pglWorkflowId));
                        //    context.SaveChanges();

                        //    //Insert new document
                        //    foreach (string doc in model.SelectedDocument)
                        //    {
                        //        tblProcessGuidelineDocument objDocIU = new tblProcessGuidelineDocument();
                        //        tblDocument objDoc = new tblDocument();
                        //        objDoc = context.tblDocument.Where(M => M.DocumentName == doc).FirstOrDefault();
                        //        if (objDoc != null)
                        //        {
                        //            objDocIU.WorkflowId = pglWorkflowId;
                        //            objDocIU.DocumentId = objDoc.DocumentId;
                        //            context.tblProcessGuidelineDocument.Add(objDocIU);
                        //            context.SaveChanges();
                        //        }
                        //    }

                        //}
                        //Document insertion
                        if ((model.ProcessguidlineworkflowId == 0 && rowsAffected > 0) || model.ProcessguidlineworkflowId > 0)
                        {
                            //Remove existing document before insert new documents   
                            context.tblProcessGuidelineWorkflowDocument.RemoveRange(context.tblProcessGuidelineWorkflowDocument.Where(x => x.ProcessGuidelineWorkflowId == pglWorkflowId));
                            context.SaveChanges();

                            //Insert new document
                            foreach (string doc in model.SelectedDocument)
                            {
                                tblProcessGuidelineWorkflowDocument objDocIU = new tblProcessGuidelineWorkflowDocument();
                                tblDocument objDoc = new tblDocument();
                                objDoc = context.tblDocument.Where(M => M.DocumentName == doc).FirstOrDefault();
                                if (objDoc != null)
                                {
                                    objDocIU.ProcessGuidelineWorkflowId = pglWorkflowId;
                                    objDocIU.DocumentId = objDoc.DocumentId;
                                    objDocIU.DocumentName = objDoc.DocumentName;
                                    objDocIU.DocumentType = model.DocumentType;
                                    objDocIU.IsRequired = model.IsRequired;
                                    objDocIU.UUID = model.UUID;
                                    context.tblProcessGuidelineWorkflowDocument.Add(objDocIU);
                                    context.SaveChanges();
                                }
                            }

                        }
                        transaction.Commit();
                        return pglWorkflowId;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return -1;
                    }
                }
            }

        }

        public static List<ProcessFlowApproverList> GetAllApproverList(int processheaderid, int processDetailId)
        {
            try
            {
                List<ProcessFlowApproverList> approvalList = new List<ProcessFlowApproverList>();
                using (var context = new IOASDBEntities())
                {
                    var query = (
                                    from WF in context.tblProcessGuidelineWorkFlow
                                    from U in context.tblUser
                                    from S in context.tblFunctionStatus
                                    where (WF.ProcessGuidelineId == processheaderid && WF.StatusId == S.FunctionStatusId && WF.ApproverId == U.UserId && WF.ProcessGuidelineDetailId == processDetailId)
                                    orderby WF.ProcessGuidelineWorkFlowId
                                    select new { WF.ProcessGuidelineWorkFlowId, WF.ProcessGuidelineDetailId, WF.ApproverId, U.UserName, S.Status, WF.StatusId, WF.ApproverLevel, WF.Approve_f, WF.Reject_f, WF.Clarify_f, WF.Mark_f }
                                 ).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            tblProcessGuidelineDocument objDoc = new tblProcessGuidelineDocument();
                            int pgwfid = query[i].ProcessGuidelineWorkFlowId;
                            //objDoc = context.tblProcessGuidelineDocument.Where(M => M.WorkflowId == pgwfid).FirstOrDefault();

                            //var lstDoc = (from D in context.tblDocument
                            //              join DD in context.tblProcessGuidelineDocument on D.DocumentId equals DD.DocumentId
                            //              where DD.WorkflowId == pgwfid
                            //              select D.DocumentName).ToList();
                            var lstDoc = (from D in context.tblProcessGuidelineWorkflowDocument
                                          where D.ProcessGuidelineWorkflowId == pgwfid
                                          select D.DocumentName).ToList();
                            //var appname = (from P in context.tblProcessGuidelineWorkFlow
                            //               where (P.ProcessGuidelineWorkFlowId == pgwfid)
                            //               select new
                            //               {
                            //                   P.Approve_f,
                            //                   P.Reject_f,
                            //                   P.Clarify_f,
                            //                   P.Mark_f
                            //               }).ToList();

                            //int[] _appList = new int[5];


                            //    if(query[i].Approve_f==true)
                            //    {

                            //        _appList[0] = 1;

                            //    }
                            //    else
                            //{
                            //    _appList[0] = 0;
                            //}
                            //     if(query[i].Reject_f==true)
                            //    {

                            //        _appList[1] = 2;

                            //    }
                            //     else
                            //{
                            //    _appList[1] = 0;
                            //}
                            //     if(query[i].Clarify_f==true)
                            //    {
                            //        _appList[2] = 3;
                            //    }
                            //     else
                            //{
                            //    _appList[2] = 0;
                            //}
                            //     if(query[i].Mark_f == true)
                            //    {
                            //        _appList[3] = 4;
                            //    }
                            //else
                            //{
                            //    _appList[3] = 0;
                            //}


                            //System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                            approvalList.Add(new ProcessFlowApproverList()
                            {
                                UserId = Convert.ToInt32(query[i].ApproverId),
                                UserName = Convert.ToString(query[i].UserName),
                                ProcessGuidelineDetailId = Convert.ToInt32(query[i].ProcessGuidelineDetailId),
                                ApproverLevel = Convert.ToInt32(query[i].ApproverLevel),
                                StatusId = Convert.ToInt32(query[i].StatusId),
                                StatusName = Convert.ToString(query[i].Status),
                                ApproveFlag = Convert.ToBoolean(query[i].Approve_f),
                                RejectFlag = Convert.ToBoolean(query[i].Reject_f),
                                ClarifyFlag = Convert.ToBoolean(query[i].Clarify_f),
                                MarkFlag = Convert.ToBoolean(query[i].Mark_f),
                                ProcessguidlineworkflowId = query[i].ProcessGuidelineWorkFlowId,
                                DocumentId = 0,//no use
                                //ApproverList = _appList,
                                SelectedDocument = lstDoc
                            });
                        }
                    }
                }
                return approvalList;
            }
            catch (Exception ex)
            {
                return new List<ProcessFlowApproverList>();
            }
        }

        public static int DeletePGLWorkflow(int processguidlineworkflowId)
        {
            try
            {
                tblProcessGuidelineWorkFlow pglWF;
                int rowsAffected = 0;
                using (var context = new IOASDBEntities())
                {
                    pglWF = context.tblProcessGuidelineWorkFlow.Where(s => s.ProcessGuidelineWorkFlowId == processguidlineworkflowId).FirstOrDefault();
                    context.Entry(pglWF).State = System.Data.Entity.EntityState.Deleted;
                    rowsAffected = context.SaveChanges();
                }
                return rowsAffected;
            }
            catch
            {
                return -1;
            }
        }

        public static int InsertProcessGuideline(ProcessGuideline model)
        {
            try
            {

                using (var context = new IOASDBEntities())
                {

                    tblProcessGuidelineHeader objIU = new tblProcessGuidelineHeader();

                    if (model.ProcessGuidelineId == 0)
                    {
                        var chkprocessname = context.tblProcessGuidelineHeader.Where(m => m.ProcessGuidelineTitle == model.ProcessName).FirstOrDefault();
                        if (chkprocessname != null)
                            return -2;
                        objIU.ProcessGuidelineId = model.ProcessGuidelineId;
                        objIU.FunctionId = model.FunctionId;
                        objIU.ProcessGuidelineTitle = model.ProcessName;
                        context.tblProcessGuidelineHeader.Add(objIU);
                        context.SaveChanges();
                        context.Dispose();
                    }
                    else
                    {
                        objIU.ProcessGuidelineId = model.ProcessGuidelineId;
                        objIU.FunctionId = model.FunctionId;
                        objIU.ProcessGuidelineTitle = model.ProcessName;
                        context.Entry(objIU).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }

                    return objIU.ProcessGuidelineId;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }



        public static int MapProcessflowUser(List<ProcessFlowUser> selectedUser)
        {
            try
            {
                int rowsAffected = 0;
                using (var context = new IOASDBEntities())
                {
                    if (selectedUser.Count > 0)
                    {
                        foreach (var user in selectedUser)
                        {
                            tblProcessGuidelineUser objIU = new tblProcessGuidelineUser();
                            objIU.ProcessGuidelineDetailId = user.ProcessGuidelineDetailId;
                            objIU.UserId = user.UserId;
                            context.tblProcessGuidelineUser.Add(objIU);
                            rowsAffected = rowsAffected + context.SaveChanges();
                        }
                    }
                }
                return rowsAffected;
            }
            catch
            {
                return -1;
            }
        }

        public static int UnmapProcessflowUser(List<ProcessFlowUser> selectedUser)
        {
            try
            {
                tblProcessGuidelineUser pglUser;
                int rowsAffected = 0;
                using (var context = new IOASDBEntities())
                {
                    if (selectedUser.Count > 0)
                    {
                        foreach (var user in selectedUser)
                        {
                            pglUser = context.tblProcessGuidelineUser.Where(s => s.UserId == user.UserId && s.ProcessGuidelineDetailId == user.ProcessGuidelineDetailId).FirstOrDefault();
                            context.Entry(pglUser).State = System.Data.Entity.EntityState.Deleted;
                            rowsAffected = rowsAffected + context.SaveChanges();
                        }
                    }
                }
                return rowsAffected;
            }
            catch
            {
                return -1;
            }
        }

        public static List<ProcessGuideline> GetProcessGuideLineList(int functionId, string processName)
        {
            List<ProcessGuideline> objProcessGuideline = new List<ProcessGuideline>();
            using (var context = new IOASDBEntities())
            {
                var query = (from pgl in context.tblProcessGuidelineHeader
                             from f in context.tblFunction
                             where (pgl.FunctionId == f.FunctionId && pgl.status != "InActive" && (pgl.FunctionId == functionId || functionId == -1)
                             && (pgl.ProcessGuidelineTitle.Contains(processName) || processName == string.Empty)
                             )
                             orderby pgl.ProcessGuidelineId
                             select new { pgl.ProcessGuidelineId, pgl.ProcessGuidelineTitle, f.FunctionName, f.FunctionId }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        objProcessGuideline.Add(new ProcessGuideline()
                        {
                            SlNo= i+1,
                            ProcessGuidelineId = (Int32)query[i].ProcessGuidelineId,
                            ProcessName = Convert.ToString(query[i].ProcessGuidelineTitle),
                            FunctionId = Convert.ToInt32(query[i].FunctionId),
                            FunctionName = Convert.ToString(query[i].FunctionName)
                        });
                    }
                }
            }
            return objProcessGuideline;
        }
    }
}