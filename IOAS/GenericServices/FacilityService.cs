using IOAS.DataModel;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace IOAS.GenericServices
{
    public class FacilityService
    {
        #region Tapal
        public int AddNewEntry(CreateTapalModel model, int logged_in_userId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    using (var transaction = context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            int TapalId = 0;
                            if (model.TapalId == 0)
                            {
                                tblTapal tapal = new tblTapal();
                                tapal.TapalType = model.selTapalType;
                                tapal.SenderDetail = model.SenderDetails;
                                tapal.ProjectTabal = model.ProjectTabal;
                                if (model.ProjectTabal)
                                {
                                    tapal.PIName = model.PIName;
                                    tapal.ProjectNumber = model.ProjectNo;
                                }
                                tapal.CreatedTS = DateTime.Now;
                               
                                tapal.Notes = model.Notes;
                                tapal.IsInward = false;
                                tapal.IsClosed = false;
                                tapal.CreatedUserId = logged_in_userId;
                                tapal.TapalNo = Common.getTapalNo();
                                context.tblTapal.Add(tapal);
                                context.SaveChanges();
                                TapalId = tapal.TapalId;
                                tblTapalWorkflow tapalWork = new tblTapalWorkflow();
                                tapalWork.TapalId = TapalId;
                                tapalWork.InwardDateTime = DateTime.Now;
                                tapalWork.UserId = logged_in_userId;
                                tapalWork.CreatedTS = DateTime.Now;
                                tapalWork.CreatedUserId = logged_in_userId;
                                tapalWork.DateTimeReceipt = model.ReceiptDate;
                                tapalWork.InwardDateTime = DateTime.Now;
                                tapalWork.Is_Active = true;
                                tapalWork.TapalAction = 0;
                                context.tblTapalWorkflow.Add(tapalWork);
                                context.SaveChanges();
                            }
                            else
                            {
                                var query = context.tblTapal.Where(m => m.TapalId == model.TapalId).FirstOrDefault();
                                var WorkflowQry = context.tblTapalWorkflow.Where(m => m.TapalId == model.TapalId).FirstOrDefault();
                                if (query != null)
                                {
                                    query.TapalType = model.selTapalType;
                                    query.SenderDetail = model.SenderDetails;
                                    query.ProjectTabal = model.ProjectTabal;
                                    if (model.ProjectTabal)
                                    {
                                        query.PIName = model.PIName;
                                        query.ProjectNumber = model.ProjectNo;
                                    }
                                    else
                                    {
                                        query.PIName = null;
                                        query.ProjectNumber = null;
                                    }
                                    query.LastUpdatedTS = DateTime.Now;
                                    query.IsInward = false;
                                    query.LastUpdatedUserId = logged_in_userId;
                                    query.Notes = model.Notes;
                                    TapalId = query.TapalId;
                                    context.SaveChanges();
                                    if (WorkflowQry != null)
                                    {
                                        WorkflowQry.DateTimeReceipt = model.ReceiptDate;
                                        context.SaveChanges();

                                    }
                                }

                                else
                                    return -1;
                            }
                            if (model.DocDetail.Count > 0)
                            {
                                //if (model.TapalId == 0) {
                                    for (int i = 0; i < model.DocDetail.Count; i++)
                                    {
                                        tblTapalDocumentDetail DocTapal = new tblTapalDocumentDetail();
                                        DocTapal.DocumentName = model.DocDetail[i].DocName;
                                        DocTapal.TapalId = TapalId;
                                        DocTapal.FileName = model.DocDetail[i].FileName;
                                        DocTapal.IsCurrentVersion = true;
                                        context.tblTapalDocumentDetail.Add(DocTapal);
                                        context.SaveChanges();
                                    }
                                //}
                                //else
                                //{
                                //    int TplID = model.TapalId??0;
                                //    var DocQuery = (from C in context.tblTapalDocumentDetail where C.TapalId == TplID && C.IsCurrentVersion==true select C).ToList();
                                //    if (DocQuery.Count > 0)
                                //    {
                                //        foreach(var item in DocQuery)
                                //        {
                                //            item.IsCurrentVersion = false;
                                //            context.SaveChanges();
                                //        }                                        
                                //    }
                                //    for (int i = 0; i < model.DocDetail.Count; i++)
                                //    {
                                //        tblTapalDocumentDetail DocTapal = new tblTapalDocumentDetail();
                                //        DocTapal.DocumentName = model.DocDetail[i].DocName;
                                //        DocTapal.TapalId = TapalId;
                                //        DocTapal.FileName = model.DocDetail[i].FileName;
                                //        DocTapal.IsCurrentVersion = true;
                                //        context.tblTapalDocumentDetail.Add(DocTapal);
                                //        context.SaveChanges();
                                //    }
                                //}
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {

                            transaction.Rollback();
                            return -1;
                        }
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {

                return -1;
            }
        }
        public PagedData<ListTapalModel> GetInventryDetail(int page, int pagesize, string Search)
        {
            try
            {
                //List<ListTapalModel> GetDetail = new List<ListTapalModel>();
                List<ListTapalModel> GetDetail = new List<ListTapalModel>();
                var searchData = new PagedData<ListTapalModel>();
                int skiprec = 0;

                if (page == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (page - 1) * pagesize;
                }
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblTapal
                                 join E in context.tblTapalWorkflow on C.TapalId equals E.TapalId
                                 join D in context.tblCodeControl on C.TapalType equals D.CodeValAbbr
                                 where C.IsInward == false && D.CodeName == "TapalCatagory" && C.IsClosed == false
                                 && (String.IsNullOrEmpty(Search) || C.SenderDetail == Search || D.CodeValDetail == Search)
                                 orderby C.TapalId descending
                                 select new { C, D.CodeValDetail, E }).Skip(skiprec).Take(pagesize).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            int TapalId = query[i].C.TapalId;
                            List<DocumentDetailModel> DocDetails = new List<DocumentDetailModel>();
                            var Documents = (from C in context.tblTapalDocumentDetail where C.TapalId == TapalId && C.IsCurrentVersion==true select C).ToList();
                            if (Documents.Count > 0)
                            {
                                for (int j = 0; j < Documents.Count; j++)
                                {
                                    DocDetails.Add(new DocumentDetailModel()
                                    {
                                        TabalId = Documents[j].TapalId ?? 0,
                                        FileName = Documents[j].FileName,
                                        DocName = Documents[j].DocumentName,
                                        TapalDocDetailId = Documents[j].TapalDocumentDetailId
                                    });
                                }
                            }

                            GetDetail.Add(new ListTapalModel()
                            {
                                TapalId = query[i].C.TapalId,
                                TapalType = Common.GetTapalType(query[i].C.TapalType ?? 0),
                                SenderDetails = query[i].C.SenderDetail,
                                ProjectTabal = query[i].C.ProjectTabal ?? true,
                                ProjectNo = Common.GetProjectNumber(query[i].C.ProjectNumber ?? 0),
                                PIName = Common.GetPIName(query[i].C.PIName ?? 0),
                                ReceiptDate = string.Format("{0:dd-MMM-yyyy}", query[i].E.DateTimeReceipt),
                                DocDetail = DocDetails,
                                slNo = i,
                                CreateUserId = query[i].E.CreatedUserId ?? 0,
                                Department = Common.GetDepartmentById(query[i].E.MarkTo ?? 0),
                                TapalNo = query[i].C.TapalNo
                            });

                        }
                        var records = (from C in context.tblTapal
                                       join E in context.tblTapalWorkflow on C.TapalId equals E.TapalId
                                       join D in context.tblCodeControl on C.TapalType equals D.CodeValAbbr
                                       where C.IsInward == false && D.CodeName == "TapalCatagory" && C.IsClosed == false
                                       && (String.IsNullOrEmpty(Search) || C.SenderDetail == Search || D.CodeValDetail == Search)
                                       orderby C.TapalId descending
                                       select new { C, D.CodeValDetail, E }).Count();
                        searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)records / pagesize));
                    }
                }
                searchData.Data = GetDetail;
                searchData.visiblePages = 3;
                searchData.CurrentPage = page;
                searchData.pageSize = pagesize;
                return searchData;
            }
            catch (Exception ex)
            {
                List<ListTapalModel> GetDetail = new List<ListTapalModel>();
                var searchData = new PagedData<ListTapalModel>();
                searchData.Data = GetDetail;
                return searchData;
            }
        }       
        public PagedData<ListTapalModel> GetInventryDetailByUser(int page, int pagesize, TapalSearchFieldModel Search, int UserId)
        {
            try
            {
                //List<ListTapalModel> GetDetail = new List<ListTapalModel>();
                List<ListTapalModel> GetDetail = new List<ListTapalModel>();
                var searchData = new PagedData<ListTapalModel>();
                int skiprec = 0;
                if (Search.selPIName == 0)
                    Search.selPIName = null;
                if (Search.selTapalType == null)
                    Search.selTapalType = 0;
                if (page == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (page - 1) * pagesize;
                }
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblTapal
                                 join E in context.tblTapalWorkflow on C.TapalId equals E.TapalId
                                 join D in context.tblCodeControl on C.TapalType equals D.CodeValAbbr
                                 where (C.IsInward == true || C.IsInward == false) && D.CodeName == "TapalCatagory" && E.UserId == UserId && E.Is_Active == true && C.IsClosed == false
                                 && (String.IsNullOrEmpty(Search.Keyword) || C.TapalNo.Contains(Search.Keyword))
                                 && ((E.InwardDateTime >= Search.FromCreatedDate && E.InwardDateTime <= Search.ToCreatedDate) ||
                                 (Search.FromCreatedDate == null && Search.ToCreatedDate == null))
                                 && (Search.selTapalType == 0 || C.TapalType == Search.selTapalType)
                                 && (Search.selPIName == null || C.PIName == Search.selPIName)
                                 orderby C.TapalId descending
                                 select new { C, D.CodeValDetail, E });
                    var Result = query.Skip(skiprec).Take(pagesize).ToList();
                    if (Result.Count > 0)
                    {
                        for (int i = 0; i < Result.Count; i++)
                        {
                            int TapalId = Result[i].C.TapalId;
                            List<DocumentDetailModel> DocDetails = new List<DocumentDetailModel>();
                            var Documents = (from C in context.tblTapalDocumentDetail where C.TapalId == TapalId && C.IsCurrentVersion == true select C).ToList();
                            if (Documents.Count > 0)
                            {
                                for (int j = 0; j < Documents.Count; j++)
                                {
                                    DocDetails.Add(new DocumentDetailModel()
                                    {
                                        TabalId = Documents[j].TapalId ?? 0,
                                        FileName = Documents[j].FileName,
                                        DocName = Documents[j].DocumentName,
                                        TapalDocDetailId = Documents[j].TapalDocumentDetailId
                                    });
                                }
                            }

                            GetDetail.Add(new ListTapalModel()
                            {
                                TapalId = Result[i].C.TapalId,
                                TapalType = Common.GetTapalType(Result[i].C.TapalType ?? 0),
                                SenderDetails = Result[i].C.SenderDetail,
                                ProjectTabal = Result[i].C.ProjectTabal ?? true,
                                ProjectNo = Common.GetProjectNumber(Result[i].C.ProjectNumber ?? 0),
                                PIName = Common.GetPIName(Result[i].C.PIName ?? 0),
                                ReceiptDate = string.Format("{0:dd-MMM-yyyy}", Result[i].E.DateTimeReceipt),
                                DocDetail = DocDetails,
                                slNo = i,
                                Action = Result[i].E.TapalAction ?? 0,
                                CreateUserId = Result[i].E.CreatedUserId ?? 0,
                                Department = Common.GetDepartmentById(Result[i].C.TapalId),
                                TapalNo = Result[i].C.TapalNo
                            });

                        }
                        var records = query.Count();
                        searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)records / pagesize));
                    }
                }
                searchData.Data = GetDetail;
                searchData.visiblePages = 3;
                searchData.CurrentPage = page;
                searchData.pageSize = pagesize;
                return searchData;
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                List<ListTapalModel> GetDetail = new List<ListTapalModel>();
                var searchData = new PagedData<ListTapalModel>();
                searchData.Data = GetDetail;
                return searchData;
            }
        }
        public int SaveInwardEntry(int Action, int Department, int Role, int ToUser, string remarks, int TapalId, int logged_in_userId, bool PopUpEdit)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var CheckQuery = (from C in context.tblTapal
                                              join D in context.tblTapalWorkflow on C.TapalId equals D.TapalId
                                              where (C.IsInward == true || C.IsInward == false) && D.Is_Active == true && C.TapalId == TapalId
                                              select new { C, D }).FirstOrDefault();
                            if (Action == 4)
                            {
                                var TplWF = context.tblTapalWorkflow.FirstOrDefault(m => m.TapalId == TapalId && m.Is_Active == true);
                                var Tabal = context.tblTapalWorkflow.FirstOrDefault(m => m.TapalId == TapalId && m.Is_Active == false && m.TapalAction == 0);
                                if (TplWF != null)
                                {
                                    TplWF.Is_Active = false;
                                    context.SaveChanges();
                                    tblTapalWorkflow twf = new tblTapalWorkflow();
                                    twf.TapalId = TapalId;
                                    twf.DateTimeReceipt = CheckQuery.D.DateTimeReceipt;
                                    twf.InwardDateTime = CheckQuery.D.OutwardDateTime;
                                    twf.TapalAction = Action;
                                    twf.MarkTo = Department;
                                    twf.Role = Role;
                                    twf.UserId = Tabal.UserId;
                                    twf.Comments = remarks;
                                    twf.OutwardDateTime = DateTime.Now;
                                    twf.CreatedTS = DateTime.Now;
                                    twf.CreatedUserId = logged_in_userId;
                                    twf.Is_Active = true;
                                    context.tblTapalWorkflow.Add(twf);
                                    context.SaveChanges();
                                }
                                
                            }
                            else { 
                            //var Login_User_Role=Common.GetRole
                            if (CheckQuery != null)
                            {
                                var TplWF = context.tblTapalWorkflow.FirstOrDefault(m => m.TapalId == TapalId && m.Is_Active == true);
                                if (TplWF != null)
                                {
                                    if (PopUpEdit != true)/* check the condition edit outward popup or inward submit outward*/
                                    {

                                         /*********** START  *************/
                                            TplWF.OutwardDateTime = DateTime.Now;
                                            TplWF.Is_Active = false;
                                        context.SaveChanges();
                                            /*********** END  *************/
                                            /* Action wise insert tapal workflow  Srart*/
                                            tblTapalWorkflow twf = new tblTapalWorkflow();
                                        twf.TapalId = TapalId;
                                        twf.DateTimeReceipt = CheckQuery.D.DateTimeReceipt;
                                        twf.InwardDateTime = DateTime.Now;
                                        twf.TapalAction = Action;
                                        if (Action == 3)
                                        {
                                            twf.MarkTo = TplWF.MarkTo;
                                            twf.Role = TplWF.Role;
                                            twf.UserId = TplWF.UserId;
                                        }
                                        else
                                        {
                                            twf.MarkTo = Department;
                                            twf.Role = Role;
                                            twf.UserId = ToUser;
                                        }
                                        twf.Comments = remarks;
                                        twf.OutwardDateTime = DateTime.Now;
                                        twf.CreatedTS = DateTime.Now;
                                        twf.CreatedUserId = logged_in_userId;
                                        twf.Is_Active = true;
                                        context.tblTapalWorkflow.Add(twf);
                                        context.SaveChanges();
                                        /* Action wise insert tapal workflow  End*/

                                    }
                                    else
                                    {
                                        TplWF.TapalAction = Action;
                                        TplWF.MarkTo = Department;
                                        TplWF.Role = Role;
                                        TplWF.UserId = ToUser;
                                        TplWF.Comments = remarks;
                                        TplWF.CreatedTS = DateTime.Now;
                                        TplWF.CreatedUserId = logged_in_userId;
                                        TplWF.Is_Active = true;
                                        context.SaveChanges();
                                    }
                                }

                            }
                            else
                            {
                                /*Update workflow table start*/
                                var UpTplWF = context.tblTapalWorkflow.FirstOrDefault(m => m.TapalId == TapalId);
                                if (UpTplWF != null)
                                {
                                    UpTplWF.TapalAction = Action;
                                    UpTplWF.MarkTo = Department;
                                    UpTplWF.Role = Role;
                                    UpTplWF.UserId = ToUser;
                                    UpTplWF.Comments = remarks;
                                    UpTplWF.OutwardDateTime = DateTime.Now;
                                    UpTplWF.Is_Active = true;
                                    context.SaveChanges();
                                }
                                /*Update workflow table start*/
                            }



                            /*Tapal table update start */
                            var UpdateTpl = context.tblTapal.FirstOrDefault(m => m.TapalId == TapalId);
                            if (UpdateTpl != null)
                            {

                                UpdateTpl.IsInward = true;
                                UpdateTpl.LastUpdatedTS = DateTime.Now;
                                UpdateTpl.LastUpdatedUserId = logged_in_userId;
                                if (Action == 3)
                                {
                                    UpdateTpl.IsClosed = true;
                                }
                                context.SaveChanges();

                            }


                        }
                            /*Tapal table update end */
                            var query = context.tblNotification.Where(m => m.ReferenceId == TapalId && m.NotificationType == "Tapal" && m.IsDeleted != true).FirstOrDefault();
                            if (query != null)
                                query.IsDeleted = true;
                            tblNotification notify = new tblNotification();
                            notify.FromUserId = logged_in_userId;
                            notify.ToUserId = ToUser;
                            notify.Description = remarks;
                            notify.NotificationType = "Tapal";
                            notify.ReferenceId = TapalId;
                            notify.Subject = "Tapal Notification";
                            notify.Crt_By = logged_in_userId;
                            notify.Crt_Ts = DateTime.Now;
                            notify.FunctionURL = "/Facility/PopupTapalDetails?TapalId=" + TapalId;
                            context.tblNotification.Add(notify);
                            context.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {

                            transaction.Rollback();
                            return -1;
                        }
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {

                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return -1;
            }
        }

        public static bool CheckDuplicateApproval(int Action, int Department, int Role, int ToUser, string remarks, int TapalId, int logged_in_userId, bool PopUpEdit)
        {
            bool checkexstingentry= false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTapalWorkflow.Where(x => x.TapalId == TapalId && x.MarkTo == Department && x.UserId == ToUser && x.TapalAction == Action&&x.Is_Active==true).OrderByDescending(x => x.TapalWorkflowId).FirstOrDefault();
                    if (query == null)
                        checkexstingentry = true;

                }
                return checkexstingentry;
            }
            catch(Exception ex)
            {
                return checkexstingentry;
            }
        }

        public static PagedData<ListTapalModel> GetOutwardDetails(int UserId, ListTapalModel model, DateFilterModel OutwardDate, int pageIndex, int pageSize)
        {
            try
            {
                PagedData<ListTapalModel> GetDetail = new PagedData<ListTapalModel>();
                List<ListTapalModel> list = new List<ListTapalModel>();
                int skiprec = 0;
                if (pageIndex == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (pageIndex - 1) * pageSize;
                }
                using (var context = new IOASDBEntities())
                {
                    bool dateWiseFilter = false;
                    Nullable<DateTime> fromTS = null;
                    Nullable<DateTime> toTS = null;
                    //if (!String.IsNullOrEmpty(OutwardDate.from) && !String.IsNullOrEmpty(OutwardDate.to))
                    //{
                    //    dateWiseFilter = true;
                    //    int fromIndx = OutwardDate.from.IndexOf(" GMT");
                    //    string strFrom = OutwardDate.from.Substring(0, fromIndx);
                    //    CultureInfo provider = CultureInfo.InvariantCulture;
                    //    fromTS = DateTime.ParseExact(strFrom, "ddd MMM dd yyyy HH:mm:ss", provider, DateTimeStyles.None);

                    //    int toIndx = OutwardDate.to.IndexOf(" GMT");
                    //    string strTo = OutwardDate.to.Substring(0, toIndx);
                    //    toTS = DateTime.ParseExact(strTo, "ddd MMM dd yyyy HH:mm:ss", provider, DateTimeStyles.None);
                    //    toTS = toTS.Value.Date.AddDays(1).AddTicks(-2);
                    //}
                    var query = (from C in context.tblTapal
                                 join D in context.tblTapalWorkflow on C.TapalId equals D.TapalId
                                 join v in context.vwFacultyStaffDetails on C.PIName ?? null equals v.UserId into g
                                 from v in g.DefaultIfEmpty()
                                 where C.IsInward == true && D.CreatedUserId == UserId && (D.Is_Active == true || D.Is_Active == false) && (C.IsClosed == false || C.IsClosed == true)
                                 && (D.TapalAction != 0 && D.TapalAction != 3)
                                 && (String.IsNullOrEmpty(model.TapalNo) || C.TapalNo.Contains(model.TapalNo))
                                 && (String.IsNullOrEmpty(model.SenderDetails) || C.SenderDetail.Contains(model.SenderDetails))
                                 && (!dateWiseFilter || (D.OutwardDateTime >= fromTS && D.OutwardDateTime <= toTS))
                                 && (v.FirstName.Contains(model.PIName) || model.PIName == null)
                                 orderby C.TapalId descending
                                 select new { C, D }).Skip(skiprec).Take(pageSize).ToList();
                    GetDetail.TotalRecords = (from C in context.tblTapal
                                              join D in context.tblTapalWorkflow on C.TapalId equals D.TapalId
                                              join v in context.vwFacultyStaffDetails on C.PIName ?? null equals v.UserId into g
                                              from v in g.DefaultIfEmpty()
                                              where C.IsInward == true && D.CreatedUserId == UserId && (D.Is_Active == true || D.Is_Active == false) && (C.IsClosed == false || C.IsClosed == true)
                                              && (D.TapalAction != 0 && D.TapalAction != 3)
                                              && (String.IsNullOrEmpty(model.TapalNo) || C.TapalNo.Contains(model.TapalNo))
                                              && (String.IsNullOrEmpty(model.SenderDetails) || C.SenderDetail.Contains(model.SenderDetails))
                                              && (!dateWiseFilter || (D.OutwardDateTime >= fromTS && D.OutwardDateTime <= toTS))
                                              && (v.FirstName.Contains(model.PIName) || model.PIName == null)
                                              orderby C.TapalId descending
                                              select new { C, D }).Count();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            int TapalId = query[i].C.TapalId;
                            List<DocumentDetailModel> DocDetails = new List<DocumentDetailModel>();
                            var Documents = (from C in context.tblTapalDocumentDetail where C.TapalId == TapalId && C.IsCurrentVersion == true select C).ToList();
                            if (Documents.Count > 0)
                            {
                                string absoluteLink = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + "/Facility/ShowDocument";
                                for (int j = 0; j < Documents.Count; j++)
                                {
                                    int TId = Documents[j].TapalId ?? 0;
                                    int DocID = Documents[j].TapalDocumentDetailId;
                                    DocDetails.Add(new DocumentDetailModel()
                                    {
                                        TabalId = TId,
                                        FileName = Documents[j].FileName,
                                        DocName = Documents[j].DocumentName,
                                        TapalDocDetailId = DocID,
                                        href = absoluteLink + "?TapalDocId=" + DocID + "&TapalId=" + TId
                                    });
                                }
                            }
                            var Department = "";
                            if (query[i].D.MarkTo != null && query[i].D.MarkTo != 0)
                            {
                                Department = Common.GetDepartmentName(query[i].D.MarkTo ?? 0);
                            }
                            else
                            {
                                Department = "Tapal";
                            }
                            var AccessRole = Common.GetRoleAccess(UserId, 14);

                            list.Add(new ListTapalModel()
                            {
                                slNo = skiprec + i + 1,
                                TapalId = TapalId,
                                TapalType = Common.GetTapalType(query[i].C.TapalType ?? 0),
                                SenderDetails = query[i].C.SenderDetail,
                                ProjectTabal = query[i].C.ProjectTabal ?? true,
                                ProjectNo = Common.GetProjectNumber(query[i].C.ProjectNumber ?? 0),
                                PIName = Common.GetPIName(query[i].C.PIName ?? 0),
                                ReceiptDt = string.Format("{0:dd-MMM-yyyy}", query[i].D.DateTimeReceipt),
                                Department = Department,//Common.GetDepartmentName(query[i].D.MarkTo ?? 0),
                                User = Common.GetUserListById(query[i].D.UserId ?? 0),
                                InwardDate = string.Format("{0:dd-MMM-yyyy}", query[i].D.InwardDateTime),
                                OutwardDate = string.Format("{0:dd-MMM-yyyy}", query[i].D.OutwardDateTime),
                                Remarks = query[i].D.Comments,
                                DocDetail = DocDetails,
                                CreateUserId = query[i].D.CreatedUserId ?? 0,
                                TapalNo = query[i].C.TapalNo,
                                IsClosed = query[i].C.IsClosed ?? false,
                                strAction = Common.GetTapalActionById(query[i].D.TapalAction ?? 0),
                                Roles = AccessRole
                            });

                        }
                    }
                }
                GetDetail.Data = list;
                return GetDetail;
            }
            catch (Exception ex)
            {
                PagedData<ListTapalModel> GetDetail = new PagedData<ListTapalModel>();
                GetDetail.Data = new List<ListTapalModel>();
                return GetDetail;
            }
        }

        public static PagedData<ListTapalModel> GetAcceptedTapalDetails(int Role, int UserId, ListTapalModel model, DateFilterModel OutwardDate, int pageIndex, int pageSize)
        {
            try
            {
                PagedData<ListTapalModel> AcceptedTapal = new PagedData<ListTapalModel>();
                List<ListTapalModel> list = new List<ListTapalModel>();
                int skiprec = 0;
                if (pageIndex == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (pageIndex - 1) * pageSize;
                }
                using (var context = new IOASDBEntities())
                {

                    //if (Role == 1)
                    //{
                    //    var query = (from C in context.tblTapal
                    //                 join D in context.tblTapalWorkflow on C.TapalId equals D.TapalId
                    //                 where C.IsInward == true && C.IsClosed==true && D.TapalAction==3
                    //                 orderby C.TapalId descending
                    //                 select new { C, D }).ToList();

                    //    if (query.Count > 0)
                    //    {
                    //        for (int i = 0; i < query.Count; i++)
                    //        {
                    //            int TapalId = query[i].C.TapalId;
                    //            List<DocumentDetailModel> DocDetails = new List<DocumentDetailModel>();
                    //            var Documents = (from C in context.tblTapalDocumentDetail where C.TapalId == TapalId select C).ToList();
                    //            if (Documents.Count > 0)
                    //            {
                    //                string absoluteLink = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + "/Facility/ShowDocument";
                    //                for (int j = 0; j < Documents.Count; j++)
                    //                {
                    //                    int TId = Documents[j].TapalId ?? 0;
                    //                    int DocID = Documents[j].TapalDocumentDetailId;
                    //                    DocDetails.Add(new DocumentDetailModel()
                    //                    {
                    //                        TabalId = TId,
                    //                        FileName = Documents[j].FileName,
                    //                        DocName = Documents[j].DocumentName,
                    //                        TapalDocDetailId = DocID,
                    //                        href = absoluteLink + "?TapalDocId=" + TId + "&TapalId=" + DocID
                    //                    });
                    //                }
                    //            }
                    //            AcceptedTapal.Add(new ListTapalModel()
                    //            {
                    //                slNo = i + 1,
                    //                TapalId = TapalId,
                    //                TapalType = Common.GetTapalType(query[i].C.TapalType ?? 0),
                    //                SenderDetails = query[i].C.SenderDetail,
                    //                ProjectTabal = query[i].C.ProjectTabal ?? true,
                    //                ProjectNo = Common.GetProjectNumber(query[i].C.ProjectNumber ?? 0),
                    //                PIName = Common.GetPIName(query[i].C.PIName ?? 0),
                    //                ReceiptDt = string.Format("{0:dd-MMMM-yyyy}", query[i].D.DateTimeReceipt),
                    //                Department = Common.GetDepartmentById(query[i].D.MarkTo ?? 0),
                    //                User = Common.GetUserListById(query[i].D.UserId ?? 0),
                    //                InwardDate = string.Format("{0:dd-MMMM-yyyy}", query[i].D.InwardDateTime),
                    //                OutwardDate = string.Format("{0:dd-MMMM-yyyy}", query[i].D.OutwardDateTime),
                    //                Remarks = query[i].D.Comments,
                    //                DocDetail = DocDetails,
                    //                CreateUserId = query[i].D.CreatedUserId ?? 0,
                    //                strAction=Common.GetTapalActionById(query[i].D.TapalAction ?? 0)
                    //            });

                    //        }
                    //    }
                    //}
                    //else
                    //{
                    var prequery = from C in context.tblTapal
                                   join D in context.tblTapalWorkflow on C.TapalId equals D.TapalId
                                   join DE in context.tblDepartment on D.MarkTo equals DE.DepartmentId
                                   join U in context.tblUser on D.UserId equals U.UserId
                                   join v in context.vwFacultyStaffDetails on C.PIName ?? null equals v.UserId into g
                                   from v in g.DefaultIfEmpty()
                                   select new MyTapalPredicate
                                   {
                                       tpl = C,
                                       tplwf = D,
                                       dep = DE,
                                       user = U,
                                       PI = v,
                                   };
                    var predicate = PredicateBuilder.BaseAnd<MyTapalPredicate>();
                    predicate = predicate.And(Ta => Ta.tpl.IsInward == true && Ta.tpl.LastUpdatedUserId == UserId && Ta.tpl.IsClosed == true && Ta.tplwf.Is_Active == true && Ta.tplwf.TapalAction == 3);
                    if (!string.IsNullOrEmpty(model.TapalNo))
                        predicate = predicate.And(Ta => Ta.tpl.TapalNo.Contains(model.TapalNo));
                    if (!string.IsNullOrEmpty(model.SenderDetails))
                        predicate = predicate.And(Ta => Ta.tpl.SenderDetail.Contains(model.SenderDetails));
                    if (!string.IsNullOrEmpty(model.Department))
                        predicate = predicate.And(Ta => Ta.dep.DepartmentName.Contains(model.Department));
                    if (!string.IsNullOrEmpty(model.User))
                        predicate = predicate.And(Ta => Ta.user.FirstName.Contains(model.User) || Ta.user.LastName.Contains(model.User));
                    if (!string.IsNullOrEmpty(model.PIName))
                        predicate = predicate.And(Ta => Ta.PI.FirstName.Contains(model.PIName));
                    if (OutwardDate.from != null && OutwardDate.to != null)
                    {
                        OutwardDate.to = OutwardDate.to.Value.Date.AddDays(1).AddTicks(-2);
                        predicate = predicate.And(Ta => Ta.tplwf.OutwardDateTime >= OutwardDate.from && Ta.tplwf.OutwardDateTime <= OutwardDate.to);
                    }
                    var query = prequery.Where(predicate).OrderByDescending(m => m.tpl.TapalId).Skip(skiprec).Take(pageSize).ToList();
                    AcceptedTapal.TotalRecords = prequery.Where(predicate).Count();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            int TapalId = query[i].tpl.TapalId;
                            List<DocumentDetailModel> DocDetails = new List<DocumentDetailModel>();
                            var Documents = (from C in context.tblTapalDocumentDetail where C.TapalId == TapalId && C.IsCurrentVersion == true select C).ToList();
                            if (Documents.Count > 0)
                            {
                                string absoluteLink = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + "/Facility/ShowDocument";
                                for (int j = 0; j < Documents.Count; j++)
                                {
                                    int TId = Documents[j].TapalId ?? 0;
                                    int DocID = Documents[j].TapalDocumentDetailId;
                                    DocDetails.Add(new DocumentDetailModel()
                                    {
                                        TabalId = TId,
                                        FileName = Documents[j].FileName,
                                        DocName = Documents[j].DocumentName,
                                        TapalDocDetailId = DocID,
                                        href = absoluteLink + "?TapalDocId=" + DocID + "&TapalId=" + TId
                                    });
                                }
                            }
                            list.Add(new ListTapalModel()
                            {
                                slNo = i + 1,
                                TapalId = TapalId,
                                TapalType = Common.GetTapalType(query[i].tpl.TapalType ?? 0),
                                SenderDetails = query[i].tpl.SenderDetail,
                                ProjectTabal = query[i].tpl.ProjectTabal ?? true,
                                ProjectNo = Common.GetProjectNumber(query[i].tpl.ProjectNumber ?? 0),
                                PIName = Common.GetPIName(query[i].tpl.PIName ?? 0),
                                ReceiptDt = string.Format("{0:dd-MMM-yyyy}", query[i].tplwf.DateTimeReceipt),
                                Department = Common.GetDepartmentName(query[i].tplwf.MarkTo ?? 0),
                                User = Common.GetUserListById(query[i].tplwf.UserId ?? 0),
                                InwardDate = string.Format("{0:dd-MMM-yyyy}", query[i].tplwf.InwardDateTime),
                                OutwardDate = string.Format("{0:dd-MMM-yyyy}", query[i].tplwf.OutwardDateTime),
                                Remarks = query[i].tplwf.Comments,
                                DocDetail = DocDetails,
                                strAction = Common.GetTapalActionById(query[i].tplwf.TapalAction ?? 0),
                                TapalNo = query[i].tpl.TapalNo
                            });

                        }
                    }
                    //}

                }
                AcceptedTapal.Data = list;
                return AcceptedTapal;
            }
            catch (Exception ex)
            {
                PagedData<ListTapalModel> AcceptedTapal = new PagedData<ListTapalModel>();
                return AcceptedTapal;
            }
        }
        public static List<ListTapalModel> GetOutwardDetails(int Role, int UserId)
        {
            try
            {
                List<ListTapalModel> GetDetail = new List<ListTapalModel>();
                using (var context = new IOASDBEntities())
                {

                    //if (Role == 1)
                    //{
                    //    var query = (from C in context.tblTapal
                    //                 join D in context.tblTapalWorkflow on C.TapalId equals D.TapalId
                    //                 where C.IsInward == true && C.IsClosed==false
                    //                 orderby C.TapalId descending
                    //                 select new { C, D }).ToList();

                    //    if (query.Count > 0)
                    //    {
                    //        for (int i = 0; i < query.Count; i++)
                    //        {
                    //            int TapalId = query[i].C.TapalId;
                    //            List<DocumentDetailModel> DocDetails = new List<DocumentDetailModel>();
                    //            var Documents = (from C in context.tblTapalDocumentDetail where C.TapalId == TapalId select C).ToList();
                    //            if (Documents.Count > 0)
                    //            {
                    //                string absoluteLink = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + "/Facility/ShowDocument";
                    //                for (int j = 0; j < Documents.Count; j++)
                    //                {
                    //                    int TId = Documents[j].TapalId ?? 0;
                    //                    int DocID = Documents[j].TapalDocumentDetailId;
                    //                    DocDetails.Add(new DocumentDetailModel()
                    //                    {
                    //                        TabalId = TId,
                    //                        FileName = Documents[j].FileName,
                    //                        DocName = Documents[j].DocumentName,
                    //                        TapalDocDetailId = DocID,
                    //                        href = absoluteLink + "?TapalDocId=" + TId + "&TapalId=" + DocID
                    //                    });
                    //                }
                    //            }
                    //            GetDetail.Add(new ListTapalModel()
                    //            {
                    //                slNo = i + 1,
                    //                TapalId = TapalId,
                    //                TapalType = Common.GetTapalType(query[i].C.TapalType ?? 0),
                    //                SenderDetails = query[i].C.SenderDetail,
                    //                ProjectTabal = query[i].C.ProjectTabal ?? true,
                    //                ProjectNo = Common.GetProjectNumber(query[i].C.ProjectNumber ?? 0),
                    //                PIName = Common.GetPIName(query[i].C.PIName ?? 0),
                    //                ReceiptDt = string.Format("{0:dd-MMMM-yyyy}", query[i].D.DateTimeReceipt),
                    //                Department = Common.GetDepartmentById(query[i].D.MarkTo ?? 0),
                    //                User = Common.GetUserListById(query[i].D.UserId ?? 0),
                    //                InwardDate = string.Format("{0:dd-MMMM-yyyy}", query[i].D.InwardDateTime),
                    //                OutwardDate = string.Format("{0:dd-MMMM-yyyy}", query[i].D.OutwardDateTime),
                    //                Remarks = query[i].D.Comments,
                    //                DocDetail = DocDetails,
                    //                CreateUserId = query[i].D.CreatedUserId ?? 0
                    //            });

                    //        }
                    //    }
                    //}
                    //else
                    //{
                    var query = (from C in context.tblTapal
                                 join D in context.tblTapalWorkflow on C.TapalId equals D.TapalId
                                 where C.IsInward == true && D.CreatedUserId == UserId && (D.Is_Active == true || D.Is_Active == false) && (C.IsClosed == false || C.IsClosed == true)
                                 && (D.TapalAction!= 0 && D.TapalAction != 3)
                                 orderby C.TapalId descending
                                 select new { C, D }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            int TapalId = query[i].C.TapalId;
                            List<DocumentDetailModel> DocDetails = new List<DocumentDetailModel>();
                            var Documents = (from C in context.tblTapalDocumentDetail where C.TapalId == TapalId&& C.IsCurrentVersion==true select C).ToList();
                            if (Documents.Count > 0)
                            {
                                string absoluteLink = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + "/Facility/ShowDocument";
                                for (int j = 0; j < Documents.Count; j++)
                                {
                                    int TId = Documents[j].TapalId ?? 0;
                                    int DocID = Documents[j].TapalDocumentDetailId;
                                    DocDetails.Add(new DocumentDetailModel()
                                    {
                                        TabalId = TId,
                                        FileName = Documents[j].FileName,
                                        DocName = Documents[j].DocumentName,
                                        TapalDocDetailId = DocID,
                                        href = absoluteLink + "?TapalDocId=" + DocID + "&TapalId=" + TId
                                    });
                                }
                            }
                            var Department = "";
                            if (query[i].D.MarkTo != null && query[i].D.MarkTo!=0)
                            {
                                Department = Common.GetDepartmentName(query[i].D.MarkTo ?? 0);
                            }
                            else
                            {
                                Department = "Tapal";
                            }
                            var AccessRole = Common.GetRoleAccess(UserId, 14);

                            GetDetail.Add(new ListTapalModel()
                            {
                                slNo = i + 1,
                                TapalId = TapalId,
                                TapalType = Common.GetTapalType(query[i].C.TapalType ?? 0),
                                SenderDetails = query[i].C.SenderDetail,
                                ProjectTabal = query[i].C.ProjectTabal ?? true,
                                ProjectNo = Common.GetProjectNumber(query[i].C.ProjectNumber ?? 0),
                                PIName = Common.GetPIName(query[i].C.PIName ?? 0),
                                ReceiptDt = string.Format("{0:dd-MMM-yyyy}", query[i].D.DateTimeReceipt),
                                Department = Department,//Common.GetDepartmentName(query[i].D.MarkTo ?? 0),
                                User = Common.GetUserListById(query[i].D.UserId ?? 0),
                                InwardDate = string.Format("{0:dd-MMM-yyyy}", query[i].D.InwardDateTime),
                                OutwardDate = string.Format("{0:dd-MMM-yyyy}", query[i].D.OutwardDateTime),
                                Remarks = query[i].D.Comments,
                                DocDetail = DocDetails,
                                CreateUserId = query[i].D.CreatedUserId ?? 0,
                                TapalNo = query[i].C.TapalNo,
                                IsClosed = query[i].C.IsClosed ?? false,
                                strAction = Common.GetTapalActionById(query[i].D.TapalAction ?? 0),
                                Roles=AccessRole
                            });

                        }
                    }
                    //}

                }
                return GetDetail;
            }
            catch (Exception ex)
            {
                List<ListTapalModel> GetDetail = new List<ListTapalModel>();
                return GetDetail;
            }
        }
        public static List<ListTapalModel> GetAcceptedTapalDetails(int Role, int UserId)
        {
            try
            {
                List<ListTapalModel> AcceptedTapal = new List<ListTapalModel>();
                using (var context = new IOASDBEntities())
                {

                    //if (Role == 1)
                    //{
                    //    var query = (from C in context.tblTapal
                    //                 join D in context.tblTapalWorkflow on C.TapalId equals D.TapalId
                    //                 where C.IsInward == true && C.IsClosed==true && D.TapalAction==3
                    //                 orderby C.TapalId descending
                    //                 select new { C, D }).ToList();

                    //    if (query.Count > 0)
                    //    {
                    //        for (int i = 0; i < query.Count; i++)
                    //        {
                    //            int TapalId = query[i].C.TapalId;
                    //            List<DocumentDetailModel> DocDetails = new List<DocumentDetailModel>();
                    //            var Documents = (from C in context.tblTapalDocumentDetail where C.TapalId == TapalId select C).ToList();
                    //            if (Documents.Count > 0)
                    //            {
                    //                string absoluteLink = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + "/Facility/ShowDocument";
                    //                for (int j = 0; j < Documents.Count; j++)
                    //                {
                    //                    int TId = Documents[j].TapalId ?? 0;
                    //                    int DocID = Documents[j].TapalDocumentDetailId;
                    //                    DocDetails.Add(new DocumentDetailModel()
                    //                    {
                    //                        TabalId = TId,
                    //                        FileName = Documents[j].FileName,
                    //                        DocName = Documents[j].DocumentName,
                    //                        TapalDocDetailId = DocID,
                    //                        href = absoluteLink + "?TapalDocId=" + TId + "&TapalId=" + DocID
                    //                    });
                    //                }
                    //            }
                    //            AcceptedTapal.Add(new ListTapalModel()
                    //            {
                    //                slNo = i + 1,
                    //                TapalId = TapalId,
                    //                TapalType = Common.GetTapalType(query[i].C.TapalType ?? 0),
                    //                SenderDetails = query[i].C.SenderDetail,
                    //                ProjectTabal = query[i].C.ProjectTabal ?? true,
                    //                ProjectNo = Common.GetProjectNumber(query[i].C.ProjectNumber ?? 0),
                    //                PIName = Common.GetPIName(query[i].C.PIName ?? 0),
                    //                ReceiptDt = string.Format("{0:dd-MMMM-yyyy}", query[i].D.DateTimeReceipt),
                    //                Department = Common.GetDepartmentById(query[i].D.MarkTo ?? 0),
                    //                User = Common.GetUserListById(query[i].D.UserId ?? 0),
                    //                InwardDate = string.Format("{0:dd-MMMM-yyyy}", query[i].D.InwardDateTime),
                    //                OutwardDate = string.Format("{0:dd-MMMM-yyyy}", query[i].D.OutwardDateTime),
                    //                Remarks = query[i].D.Comments,
                    //                DocDetail = DocDetails,
                    //                CreateUserId = query[i].D.CreatedUserId ?? 0,
                    //                strAction=Common.GetTapalActionById(query[i].D.TapalAction ?? 0)
                    //            });

                    //        }
                    //    }
                    //}
                    //else
                    //{
                    var query = (from C in context.tblTapal
                                 join D in context.tblTapalWorkflow on C.TapalId equals D.TapalId
                                 where C.IsInward == true && C.LastUpdatedUserId == UserId && D.Is_Active == true && C.IsClosed == true
                                 && D.TapalAction == 3
                                 orderby C.TapalId descending
                                 select new { C, D }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            int TapalId = query[i].C.TapalId;
                            List<DocumentDetailModel> DocDetails = new List<DocumentDetailModel>();
                            var Documents = (from C in context.tblTapalDocumentDetail where C.TapalId == TapalId && C.IsCurrentVersion==true select C).ToList();
                            if (Documents.Count > 0)
                            {
                                string absoluteLink = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + "/Facility/ShowDocument";
                                for (int j = 0; j < Documents.Count; j++)
                                {
                                    int TId = Documents[j].TapalId ?? 0;
                                    int DocID = Documents[j].TapalDocumentDetailId;
                                    DocDetails.Add(new DocumentDetailModel()
                                    {
                                        TabalId = TId,
                                        FileName = Documents[j].FileName,
                                        DocName = Documents[j].DocumentName,
                                        TapalDocDetailId = DocID,
                                        href = absoluteLink + "?TapalDocId=" + DocID + "&TapalId=" + TId
                                    });
                                }
                            }
                            AcceptedTapal.Add(new ListTapalModel()
                            {
                                slNo = i + 1,
                                TapalId = TapalId,
                                TapalType = Common.GetTapalType(query[i].C.TapalType ?? 0),
                                SenderDetails = query[i].C.SenderDetail,
                                ProjectTabal = query[i].C.ProjectTabal ?? true,
                                ProjectNo = Common.GetProjectNumber(query[i].C.ProjectNumber ?? 0),
                                PIName = Common.GetPIName(query[i].C.PIName ?? 0),
                                ReceiptDt = string.Format("{0:dd-MMM-yyyy}", query[i].D.DateTimeReceipt),
                                Department = Common.GetDepartmentName(query[i].D.MarkTo ?? 0),
                                User = Common.GetUserListById(query[i].D.UserId ?? 0),
                                InwardDate = string.Format("{0:dd-MMM-yyyy}", query[i].D.InwardDateTime),
                                OutwardDate = string.Format("{0:dd-MMM-yyyy}", query[i].D.OutwardDateTime),
                                Remarks = query[i].D.Comments,
                                DocDetail = DocDetails,
                                strAction = Common.GetTapalActionById(query[i].D.TapalAction ?? 0),
                                TapalNo = query[i].C.TapalNo
                            });

                        }
                    }
                    //}

                }
                return AcceptedTapal;
            }
            catch (Exception ex)
            {
                List<ListTapalModel> AcceptedTapal = new List<ListTapalModel>();
                return AcceptedTapal;
            }
        }
        public TapalDetailsModel GetTapalDetails(int TapalId,int UserId)
        {
            try
            {
                TapalDetailsModel model = new TapalDetailsModel();

                using (var context = new IOASDBEntities())
                {

                    var query = (from tpl in context.tblTapal
                                 join tplWF in context.tblTapalWorkflow on tpl.TapalId equals tplWF.TapalId
                                 join notify in context.tblNotification on
                                 new { id = tpl.TapalId, type = "Tapal" } equals
                                 new { id = notify.ReferenceId, type = notify.NotificationType }
                                 join user in context.vwFacultyStaffDetails on tpl.PIName equals user.UserId into g
                                 from user in g.DefaultIfEmpty()
                                 where tpl.TapalId == TapalId && (notify.Crt_By==UserId || notify.ToUserId == UserId)
                                 select new { tpl, user, notify.FromUserId, tplWF }).FirstOrDefault();
                    if (query != null)
                    {
                        List<DocumentDetailModel> DocDetails = new List<DocumentDetailModel>();
                        var Documents = (from C in context.tblTapalDocumentDetail where C.TapalId == TapalId && C.IsCurrentVersion==true select C).ToList();
                        if (Documents.Count > 0)
                        {
                            for (int j = 0; j < Documents.Count; j++)
                            {
                                DocDetails.Add(new DocumentDetailModel()
                                {
                                    TabalId = Documents[j].TapalId ?? 0,
                                    FileName = Documents[j].FileName,
                                    DocName = Documents[j].DocumentName,
                                    TapalDocDetailId = Documents[j].TapalDocumentDetailId
                                });
                            }
                        }
                        model.TapalId = TapalId;
                        model.ReceiptDate = String.Format("{0:ddd dd-MMM-yyyy}", query.tplWF.DateTimeReceipt);
                        model.FromUser = Common.GetUserFirstName(query.FromUserId ?? 0);
                        model.TapalType = Common.GetTapalType(query.tpl.TapalType ?? 0);
                        model.SenderDetails = query.tpl.SenderDetail;
                        model.ProjectTabal = query.tpl.ProjectTabal ?? false;
                        model.ProjectNumber = query.tpl.ProjectNumber != null ? Common.GetProjectNumber(query.tpl.ProjectNumber ?? 0) : "NA";
                        model.PIName = query.user != null ? query.user.EmployeeId + "-" + query.user.FirstName  : "NA";
                        model.Remarks = query.tpl.Notes;// query.tplWF.Comments;
                        model.DocDetail = DocDetails;
                    }

                }

                return model;
            }
            catch (Exception ex)
            {
                TapalDetailsModel model = new TapalDetailsModel();
                return model;
            }
        }
        public CreateTapalModel GetInwardForEdit(int TapalId)
        {
            try
            {
                CreateTapalModel model = new CreateTapalModel();
                using (var context = new IOASDBEntities())
                {

                    var query = (from tpl in context.tblTapal
                                 join tplWF in context.tblTapalWorkflow on tpl.TapalId equals tplWF.TapalId
                                 where tpl.TapalId == TapalId
                                 select new { tpl, tplWF }).FirstOrDefault();
                    if (query != null)
                    {
                        List<DocumentDetailModel> DocDetails = new List<DocumentDetailModel>();
                        var Documents = (from C in context.tblTapalDocumentDetail where C.TapalId == TapalId && C.IsCurrentVersion == true select C).ToList();
                        if (Documents.Count > 0)
                        {
                            for (int j = 0; j < Documents.Count; j++)
                            {
                                DocDetails.Add(new DocumentDetailModel()
                                {
                                    TabalId = Documents[j].TapalId ?? 0,
                                    FileName = Documents[j].FileName,
                                    DocName = Documents[j].DocumentName,
                                    TapalDocDetailId = Documents[j].TapalDocumentDetailId
                                });
                            }
                        }
                        bool pTapal = query.tpl.ProjectTabal ?? true;
                        model.TapalId = TapalId;
                        model.ReceiptDate = (DateTime)query.tplWF.DateTimeReceipt;
                        model.selTapalType = query.tpl.TapalType ?? 0;
                        model.SenderDetails = query.tpl.SenderDetail;
                        model.ProjectTabal = pTapal;
                        model.tapalType = pTapal ? "Yes" : "No";
                        model.ProjectNo = query.tpl.ProjectNumber;
                        model.PIName = query.tpl.PIName;
                        model.DocDetail = DocDetails;
                        ProjectDetailModel detail = new ProjectDetailModel();
                        detail= Common.GetProjectsDetails(model.ProjectNo ?? 0);
                        model.ProjectTittle = detail.ProjectTittle;
                        model.ProjectSubType = detail.ProjectSubType;
                        model.ProjectType = detail.ProjectType;
                        model.ProjectCategory = detail.ProjectCategory;
                        model.Notes = query.tpl.Notes;
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public CreateTapalModel GetOutwardForEdit(int TapalId, int UserId)
        {
            try
            {
                CreateTapalModel model = new CreateTapalModel();
                using (var context = new IOASDBEntities())
                {

                    var query = (from tpl in context.tblTapal
                                 join tplWF in context.tblTapalWorkflow on tpl.TapalId equals tplWF.TapalId
                                 where tpl.TapalId == TapalId && tplWF.CreatedUserId == UserId
                                 select new { tpl, tplWF }).FirstOrDefault();
                    if (query != null)
                    {

                        model.TapalId = TapalId;
                        model.selAction = query.tplWF.TapalAction ?? 0;
                        model.selDepartment = query.tplWF.MarkTo ?? 0;
                        model.selUser = query.tplWF.UserId ?? 0;
                        model.selRole = query.tplWF.Role ?? 0;
                        model.Remarks = query.tplWF.Comments;
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int DeleteDocument(int TapalDocId,int TapalId)
        {
            try
            {
                int result = 0;
                using(var context=new IOASDBEntities())
                {
                    var Query = (from C in context.tblTapalDocumentDetail where C.TapalDocumentDetailId == TapalDocId && C.TapalId == TapalId select C).FirstOrDefault();
                    if (Query != null)
                    {
                        Query.IsCurrentVersion = false;
                        context.SaveChanges();
                        result = 1;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {

                return 0;
            }
        }
        public CreateTapalModel GetDisplayTapalDetails(int TapalId)
        {
            try
            {
                CreateTapalModel model = new CreateTapalModel();
                using (var context = new IOASDBEntities())
                {
                    var qry = (from tpl in context.tblTapal
                               join tplWF in context.tblTapalWorkflow on tpl.TapalId equals tplWF.TapalId
                               join Fsd in context.vwFacultyStaffDetails on tpl.PIName equals Fsd.UserId into g
                               from Fsd in g.DefaultIfEmpty()
                               join cdcn in context.tblCodeControl on tpl.TapalType equals cdcn.CodeValAbbr
                               join p in context.tblProject.DefaultIfEmpty() on tpl.ProjectNumber equals p.ProjectId into pj
                               from p in pj.DefaultIfEmpty()
                               where cdcn.CodeName == "TapalCatagory" && tpl.TapalId == TapalId
                               select new { tpl, tplWF, Fsd, cdcn, p }).FirstOrDefault();
                    if (qry != null)
                    {
                        List<DocumentDetailModel> DocDetails = new List<DocumentDetailModel>();
                        var Documents = (from C in context.tblTapalDocumentDetail where C.TapalId == TapalId && C.IsCurrentVersion == true select C).ToList();
                        if (Documents.Count > 0)
                        {
                            for (int j = 0; j < Documents.Count; j++)
                            {
                                DocDetails.Add(new DocumentDetailModel()
                                {
                                    TabalId = Documents[j].TapalId ?? 0,
                                    FileName = Documents[j].FileName,
                                    DocName = Documents[j].DocumentName,
                                    TapalDocDetailId = Documents[j].TapalDocumentDetailId
                                });
                            }
                        }
                        model.TapalId = TapalId;
                        model.ReceiptDateStr = String.Format("{0:dd/MM/yyyy}", qry.tplWF.DateTimeReceipt);
                        if (qry.Fsd != null)
                            model.ProjectNoStr = qry.p.ProjectNumber;  //Project Number
                        if (qry.p != null)
                            model.PINameStr = qry.Fsd.FirstName;
                        model.TapalCategory = qry.cdcn.CodeValDetail;
                        model.DocDetail = DocDetails;
                        model.Notes = qry.tpl.Notes;
                        model.FilePath = "~/Content/TapalDocuments/";
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
        #region SRB
        public PagedData<SRBSearchResultModel> GetSRBList(SRBSearchFieldModel model, int page, int pageSize)
        {
            try
            {
                List<SRBSearchResultModel> list = new List<SRBSearchResultModel>();
                var searchData = new PagedData<SRBSearchResultModel>();
                int skiprec = 0;
                if (page == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (page - 1) * pageSize;
                }
                using (var context = new IOASDBEntities())
                {
                    var predicate = PredicateBuilder.BaseAnd<tblSRB>();
                    if (!string.IsNullOrEmpty(model.Keyword))
                        predicate = predicate.And(d => d.SRBNumber.Contains(model.Keyword) || d.SupplierName.Contains(model.Keyword) || d.InvoiceNumber.Contains(model.Keyword));
                    if (model.Department != null)
                        predicate = predicate.And(d => d.DepartmentId == model.Department);
                    if (model.FromPODate != null && model.ToPODate != null)
                        predicate = predicate.And(d => d.PurchaseDate >= model.FromPODate && d.PurchaseDate <= model.ToPODate);
                    if (model.FromSRBDate != null && model.ToSRBDate != null)
                        predicate = predicate.And(d => d.Crt_Ts >= model.FromSRBDate && d.Crt_Ts <= model.ToSRBDate);
                    if (model.FromInwardDate != null && model.ToInwardDate != null)
                        predicate = predicate.And(d => d.InwardDate >= model.FromInwardDate && d.InwardDate <= model.ToInwardDate);
                    predicate = predicate.And(d => d.Status != "InActive");
                    var query = context.tblSRB.Where(predicate).OrderByDescending(m => m.SRBId).Skip(skiprec).Take(pageSize).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            string docPath = string.Empty;
                            if (!String.IsNullOrEmpty(query[i].PODocs))
                            {
                                var doc = query[i].PODocs.Split(new char[] { '_' }, 2);
                                docPath = doc[1];
                            }
                            int srbId = query[i].SRBId;
                            var catgry = context.tblSRBDetails.FirstOrDefault(m => m.SRBId == srbId);
                            list.Add(new SRBSearchResultModel()
                            {
                                DocFullName = query[i].PODocs,
                                DocName = docPath,
                                InwardDate = String.Format("{0:ddd dd-MMM-yyyy}", query[i].InwardDate),
                                ItemCategory = catgry != null ? catgry.CategoryId : null,
                                SRBDate = String.Format("{0:ddd dd-MMM-yyyy}", query[i].Crt_Ts),
                                NetTotalAmount = query[i].NetTotalAmount ?? 0,
                                SRBNo = query[i].SRBNumber,
                                SRBId = query[i].SRBId,
                                Status = query[i].Status
                            });

                        }
                    }
                    var records = context.tblSRB.Where(predicate).OrderByDescending(m => m.SRBId).Count();
                    searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)records / pageSize));
                    searchData.Data = list;
                    searchData.pageSize = pageSize;
                    searchData.visiblePages = 10;
                    searchData.CurrentPage = page;
                }

                return searchData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public SRBDetailsModel GetSRBDetails(int SRBId)
        {
            try
            {
                SRBDetailsModel model = new SRBDetailsModel();

                using (var context = new IOASDBEntities())
                {

                    var query = (from srb in context.tblSRB
                                 join dpt in context.tblDepartment on srb.DepartmentId equals dpt.DepartmentId
                                 //join cc in context.tblCodeControl on
                                 //new { Category = srb.ItemCategoryId ?? 0, codeName = "SRB_ItemCategory" } equals
                                 //new { Category = cc.CodeValAbbr, codeName = cc.CodeName }                                 
                                 where srb.SRBId == SRBId
                                 select new { srb, dpt.DepartmentName }).FirstOrDefault();
                    if (query != null)
                    {
                        string docPath = string.Empty;
                        if (!String.IsNullOrEmpty(query.srb.PODocs))
                        {
                            var doc = query.srb.PODocs.Split(new char[] { '_' }, 2);
                            docPath = doc[1];
                        }
                        string srbNo = query.srb.SRBNumber;
                        model.DocFullName = query.srb.PODocs;
                        model.DocName = docPath;
                        model.InwardDate = String.Format("{0:ddd dd-MMM-yyyy}", query.srb.InwardDate);
                        model.Department = query.DepartmentName;
                        model.SupplierName = query.srb.SupplierName;
                        model.SRBNumber = srbNo;
                        model.SRBDate = String.Format("{0:ddd dd-MMM-yyyy}", query.srb.Crt_Ts);
                        model.InvoiceNumber = query.srb.InvoiceNumber;
                        //model.ItemCategory = query.CodeValDetail;
                        //model.ItemName = query.ItemName;
                        model.PONumber = query.srb.PONumber;
                        model.SRBId = query.srb.SRBId;
                        model.PONumber = query.srb.PONumber;
                        model.TaxRate = query.srb.TaxRate ?? 0;
                        model.NetTotalAmount = query.srb.NetTotalAmount ?? 0;
                        model.InvoiceDate = String.Format("{0:ddd dd-MMM-yyyy}", query.srb.PurchaseDate);
                        model.Remarks = query.srb.Remarks;
                        var detQuery = (from det in context.tblSRBDetails
                                        join cc in context.tblSRBItemCategory on
                                       det.CategoryId ?? 0 equals cc.SRBItemCategotyId
                                        where det.SRBId == SRBId
                                        select new { det, cc.Category }).ToList();
                        int count = detQuery.Count();
                        if (count > 0)
                        {

                            
                            string[] _ct = new string[count];
                            string[] _iName = new string[count];
                            string[] _iQty = new string[count];
                            decimal[] _ttlAmt = new decimal[count];
                            string[] _asset = new string[count];
                            string[] _iNumber = new string[count];
                            string[] _buyBackNo = new string[count];
                            for (int i = 0; i < count; i++)
                            {
                                int srbDetId = detQuery[i].det.tblSRBDetailId;
                                string buybackNo = "";
                                var queryBuyback = (from deact in context.tblSRBDeactivation
                                                    join srb in context.tblSRB on deact.SRBId equals srb.SRBId
                                                    where deact.BuybackRefId == srbDetId
                                                    select new { deact.SRBDeactivationId, itemNumber = srb.SRBNumber + "/" + deact.ItemNumber }).ToList();
                                if (queryBuyback.Count > 0)
                                    buybackNo = string.Join(", ", queryBuyback
                                        .Select(x => x.itemNumber));
                                int uom = detQuery[i].det.UOM ?? 0;
                                var qry = context.tblCodeControl.FirstOrDefault(m => m.CodeName == "UOM" && m.CodeValAbbr == uom);

                                _ct[i] = detQuery[i].Category;
                                _iName[i] = detQuery[i].det.ItemName;
                                _iQty[i] = qry != null ? detQuery[i].det.Quantity.ToString() + " " + qry.CodeValDetail : detQuery[i].det.Quantity.ToString();
                                _ttlAmt[i] = detQuery[i].det.ItemValue ?? 0;
                                _asset[i] = detQuery[i].det.Asset_f == true ? "Asset" : "";
                                _iNumber[i] = srbNo + "/" + detQuery[i].det.ItemNumber;
                                _buyBackNo[i] = buybackNo;
                            }
                            model.ItemName = _iName;
                            model.ItemCategory = _ct;
                            model.Quantity = _iQty;
                            model.TotalAmount = _ttlAmt;
                            model.Asset = _asset;
                            model.ItemNumber = _iNumber;
                            model.selBuyBackNo = _buyBackNo;
                        }
                    }

                }

                return model;
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
          (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return null;
            }
        }
        public SRBModel GetSRBForEdit(int SRBId)
        {
            try
            {
                SRBModel model = new SRBModel();

                using (var context = new IOASDBEntities())
                {

                    var query = context.tblSRB.Where(m => m.SRBId == SRBId && m.Status == "Open").FirstOrDefault();
                    if (query != null)
                    {
                        string docPath = string.Empty;
                        if (!String.IsNullOrEmpty(query.PODocs))
                        {
                            var doc = query.PODocs.Split(new char[] { '_' }, 2);
                            docPath = doc[1];
                        }

                        model.DocFullName = query.PODocs;
                        model.DocName = docPath;
                        model.InwardDate = (DateTime)query.InwardDate;
                        model.PONumber = query.PONumber;
                        model.SRBId = query.SRBId;
                        model.PONumber = query.PONumber;
                        model.Department = query.DepartmentId;
                        model.InvoiceNumber = query.InvoiceNumber;
                        model.SupplierName = query.SupplierName;
                        model.TaxRate = query.TaxRate;
                        //model.ProjectNumber = query.ProjectNumber;
                        model.PurchaseDate = (DateTime)query.PurchaseDate;
                        model.Remarks = query.Remarks;


                        var detQuery = (from c in context.tblSRBDetails
                                        where c.SRBId == SRBId
                                        select c).ToList();
                        int count = detQuery.Count();
                        if (count > 0)
                        {
                            int[] _ct = new int[count];
                            string[] _iName = new string[count];
                            string[] _buyback = new string[count];
                            string[] _buybackNo = new string[count];
                            decimal[] _iQty = new decimal[count];
                            decimal[] _ttlAmt = new decimal[count];
                            int[] _uom = new int[count];
                            bool[] _asset = new bool[count];
                            for (int i = 0; i < count; i++)
                            {
                                int srbDetId = detQuery[i].tblSRBDetailId;
                                string buyback = "", buybackNo = "";

                                var queryBuyback = (from deact in context.tblSRBDeactivation
                                                    join srb in context.tblSRB on deact.SRBId equals srb.SRBId
                                                    where deact.BuybackRefId == srbDetId
                                                    select new { deact.SRBDeactivationId, itemNumber = srb.SRBNumber + "/" + deact.ItemNumber }).ToList();
                                if (queryBuyback.Count > 0)
                                {
                                    buyback = string.Join(",", queryBuyback
                                        .Select(x => x.SRBDeactivationId));
                                    buybackNo = string.Join(", ", queryBuyback
                                        .Select(x => x.itemNumber));
                                }
                                _ct[i] = (Int32)detQuery[i].CategoryId;
                                _iName[i] = detQuery[i].ItemName;
                                _iQty[i] = detQuery[i].Quantity ?? 0;
                                _ttlAmt[i] = detQuery[i].ItemValue ?? 0;
                                _uom[i] = detQuery[i].UOM ?? 0;
                                _asset[i] = detQuery[i].Asset_f ?? false;
                                _buyback[i] = buyback;
                                _buybackNo[i] = buybackNo;
                            }
                            model.ItemName = _iName;
                            model.ItemCategory = _ct;
                            model.Quantity = _iQty;
                            model.TotalAmount = _ttlAmt;
                            model.IsAsset = _asset;
                            model.UOM = _uom;
                            model.selBuyBack = _buyback;
                            model.selBuyBackNo = _buybackNo;
                        }
                    }

                }

                return model;
            }
            catch (Exception ex)
            {
                return new SRBModel();
            }
        }
        public int PostSRB(SRBModel model, int LoggedInUser)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.SRBId > 0)
                        {
                            var query = context.tblSRB.FirstOrDefault(m => m.SRBId == model.SRBId);
                            if (query != null)
                            {
                                query.Updt_By = LoggedInUser;
                                query.Updt_Ts = DateTime.Now;
                                query.InwardDate = model.InwardDate;

                                if (!String.IsNullOrEmpty(model.DocFullName))
                                    query.PODocs = model.DocFullName;
                                query.PONumber = model.PONumber;
                                query.DepartmentId = model.Department;
                                query.InvoiceNumber = model.InvoiceNumber;
                                query.SupplierName = model.SupplierName;
                                query.PurchaseDate = model.PurchaseDate;
                                query.Remarks = model.Remarks;
                                query.TaxRate = model.TaxRate;
                                query.NetTotalAmount = model.TotalAmount.Sum();
                                context.tblSRBDetails.Where(m => m.SRBId == model.SRBId).Select(m => m.tblSRBDetailId).ToList()
                                    .ForEach(x =>
                                    {
                                        context.tblSRBDeactivation.Where(z => z.BuybackRefId == x).ToList()
                                       .ForEach(y => y.BuybackRefId = null);
                                    });

                                context.tblSRBDetails.RemoveRange(context.tblSRBDetails.Where(m => m.SRBId == model.SRBId));

                                context.SaveChanges();

                                for (int i = 0; i < model.ItemCategory.Length; i++)
                                {
                                    tblSRBDetails det = new tblSRBDetails();
                                    det.CategoryId = model.ItemCategory[i];
                                    det.SRBId = model.SRBId;
                                    det.ItemName = model.ItemName[i];
                                    det.ItemValue = model.TotalAmount[i];
                                    det.Quantity = model.Quantity[i];
                                    det.ItemNumber = (i + 1).ToString();
                                    det.UOM = model.UOM[i];
                                    det.Asset_f = model.IsAsset[i];
                                    det.CRTD_TS = DateTime.Now;
                                    det.CRTD_UserID = LoggedInUser;
                                    det.Status = "Open";
                                    context.tblSRBDetails.Add(det);
                                    context.SaveChanges();
                                    int detId = det.tblSRBDetailId;
                                    if (!String.IsNullOrEmpty(model.selBuyBack[i]))
                                    {
                                        var selItems = model.selBuyBack[i].Split(',');
                                        for (int j = 0; j < selItems.Length; j++)
                                        {
                                            int item = Convert.ToInt32(selItems[j]);
                                            var updtQuery = context.tblSRBDeactivation.FirstOrDefault(m => m.SRBDeactivationId == item);
                                            if (updtQuery != null)
                                            {
                                                updtQuery.BuybackRefId = detId;
                                                updtQuery.UPDT_TS = DateTime.Now;
                                                updtQuery.UPDT_UserID = LoggedInUser;
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                throw new Exception();
                                            }
                                        }
                                    }                                    
                                }

                                transaction.Commit();
                                return 2;
                            }
                            else
                            {
                                return -2;
                            }
                        }
                        else
                        {
                            tblSRB srb = new tblSRB();
                            srb.Crt_By = LoggedInUser;
                            srb.Crt_Ts = DateTime.Now;

                            srb.InwardDate = model.InwardDate;
                            srb.PODocs = model.DocFullName;
                            srb.PONumber = model.PONumber;
                            srb.DepartmentId = model.Department;
                            srb.InvoiceNumber = model.InvoiceNumber;
                            srb.SupplierName = model.SupplierName;
                            srb.PurchaseDate = model.PurchaseDate;
                            srb.Remarks = model.Remarks;
                            srb.Status = "Open";
                            srb.NetTotalAmount = model.TotalAmount.Sum();
                            srb.TaxRate = model.TaxRate;
                            srb.SRBNumber = Common.GetNewSRBNumber();
                            context.tblSRB.Add(srb);
                            context.SaveChanges();

                            int srbId = srb.SRBId;

                            for (int i = 0; i < model.ItemCategory.Length; i++)
                            {
                                tblSRBDetails det = new tblSRBDetails();
                                det.CategoryId = model.ItemCategory[i];
                                det.SRBId = srbId;
                                det.ItemName = model.ItemName[i];
                                det.Quantity = model.Quantity[i];
                                det.ItemValue = model.TotalAmount[i];
                                det.ItemNumber = (i + 1).ToString();
                                det.UOM = model.UOM[i];
                                det.Asset_f = model.IsAsset[i];
                                det.Status = "Open";
                                det.CRTD_TS = DateTime.Now;
                                det.CRTD_UserID = LoggedInUser;
                                context.tblSRBDetails.Add(det);
                                context.SaveChanges();
                                int detId = det.tblSRBDetailId;
                                if (!String.IsNullOrEmpty(model.selBuyBack[i]))
                                {
                                    var selItems = model.selBuyBack[i].Split(',');
                                    for (int j = 0; j < selItems.Length; j++)
                                    {
                                        int item = Convert.ToInt32(selItems[j]);
                                        var updtQuery = context.tblSRBDeactivation.FirstOrDefault(m => m.SRBDeactivationId == item);
                                        if (updtQuery != null)
                                        {
                                            updtQuery.BuybackRefId = detId;
                                            updtQuery.UPDT_TS = DateTime.Now;
                                            updtQuery.UPDT_UserID = LoggedInUser;
                                            context.SaveChanges();
                                        }
                                        else
                                        {
                                            throw new Exception();
                                        }
                                    }
                                }
                            }

                            transaction.Commit();
                            return 1;
                        }

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return -1;
                    }
                }
            }
        }

        public bool ActivateSRB(int SRBId, int LoggedInUser)
        {
            using (var context = new IOASDBEntities())
            {

                try
                {

                    var query = context.tblSRB.FirstOrDefault(m => m.SRBId == SRBId && m.Status == "Open");
                    if (query != null)
                    {
                        query.Status = "Active";
                        query.Updt_By = LoggedInUser;
                        query.Updt_Ts = DateTime.Now;
                        var details = context.tblSRBDetails.Where(x => x.SRBId == SRBId)
                            .ToList();
                        details.ForEach(m => m.Status = "Active");
                        context.SaveChanges();

                        return true;
                    }
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    return false;
                }

            }
        }
        public List<SRBItemDetailsModel> GetSRBItemForDeactivate()
        {
            try
            {
                List<SRBItemDetailsModel> details = new List<SRBItemDetailsModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from det in context.tblSRBDetails
                                 join cc in context.tblCodeControl on
                                        new { Category = det.CategoryId ?? 0, codeName = "SRB_ItemCategory" } equals
                                        new { Category = cc.CodeValAbbr, codeName = cc.CodeName }
                                 where det.Asset_f == true && det.Status == "Active"
                                 orderby det.tblSRBDetailId descending
                                 select new { det, cc.CodeValDetail }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {

                            details.Add(new SRBItemDetailsModel()
                            {
                                slNo = i + 1,
                                SRBDetailId = query[i].det.tblSRBDetailId,
                                ItemNumber = Common.GetSRBNumber(query[i].det.SRBId ?? 0) + "/" + query[i].det.ItemNumber,
                                ItemName = query[i].det.ItemName,
                                ItemCategory = query[i].CodeValDetail,
                                ItemValue = query[i].det.ItemValue,
                                Status = query[i].det.Status,
                                Quantity = query[i].det.Quantity.ToString()
                            });

                        }
                    }
                }
                return details;
            }
            catch (Exception ex)
            {
                List<SRBItemDetailsModel> details = new List<SRBItemDetailsModel>();
                return details;
            }
        }
        public SRBItemDetailsModel GetSRBItemForDeactivate(int SRBDetailId)
        {
            try
            {
                SRBItemDetailsModel details = new SRBItemDetailsModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from det in context.tblSRBDetails
                                 join cc in context.tblCodeControl on
                                        new { Category = det.CategoryId ?? 0, codeName = "SRB_ItemCategory" } equals
                                        new { Category = cc.CodeValAbbr, codeName = cc.CodeName }
                                 where det.Asset_f == true && det.Status == "Active" && det.tblSRBDetailId == SRBDetailId
                                 select new { det, cc.CodeValDetail }).FirstOrDefault();

                    if (query != null)
                    {
                        details.SRBDetailId = query.det.tblSRBDetailId;
                        details.ItemNumber = Common.GetSRBNumber(query.det.SRBId ?? 0) + "/" + query.det.ItemNumber;
                        details.ItemName = query.det.ItemName;
                        details.ItemCategory = query.CodeValDetail;
                        details.ItemValue = query.det.ItemValue;
                        details.Status = query.det.Status;
                        details.Quantity = query.det.Quantity.ToString();
                    }
                }
                return details;
            }
            catch (Exception ex)
            {
                SRBItemDetailsModel details = new SRBItemDetailsModel();
                return details;
            }
        }
        public bool ScrapSRBItem(int SRBDetailId, int LoggedInUser)
        {
            using (var context = new IOASDBEntities())
            {
                try
                {
                    var query = context.tblSRBDetails.FirstOrDefault(m => m.tblSRBDetailId == SRBDetailId && m.Status == "Active");
                    if (query != null)
                    {
                        query.Status = "Scrap";
                        query.UPDT_UserID = LoggedInUser;
                        query.UPDT_TS = DateTime.Now;

                        tblSRBDeactivation _dact = new tblSRBDeactivation();
                        _dact.ItemName = query.ItemName;
                        _dact.ItemNumber = query.ItemNumber;
                        _dact.SRBId = query.SRBId;
                        _dact.tblSRBDetailId = SRBDetailId;
                        _dact.CRTD_TS = DateTime.Now;
                        _dact.CRTD_UserID = LoggedInUser;
                        context.tblSRBDeactivation.Add(_dact);
                        context.SaveChanges();

                        return true;
                    }
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    return false;
                }

            }
        }

        public int Deactivation(SRBItemDetailsModel model, int LoggedInUser)
        {
            using (var context = new IOASDBEntities())
            {

                try
                {
                    var query = context.tblSRBDetails.FirstOrDefault(m => m.tblSRBDetailId == model.SRBDetailId && m.Status == "Active");
                    if (query != null)
                    {
                        query.Status = "Buyback";
                        query.UPDT_UserID = LoggedInUser;
                        query.UPDT_TS = DateTime.Now;

                        tblSRBDeactivation _dact = new tblSRBDeactivation();
                        _dact.ItemName = query.ItemName;
                        _dact.ItemNumber = query.ItemNumber;
                        _dact.SRBId = query.SRBId;
                        _dact.tblSRBDetailId = model.SRBDetailId;
                        _dact.CRTD_TS = DateTime.Now;
                        _dact.CRTD_UserID = LoggedInUser;
                        _dact.Attachment = model.DocName;
                        _dact.BuybackValue = model.BuybackValue;
                        _dact.Buyback_f = true;
                        _dact.Comments = model.Comments;
                        context.tblSRBDeactivation.Add(_dact);
                        context.SaveChanges();

                        return 1;
                    }
                    else
                        return -2;

                }
                catch (Exception ex)
                {
                    return -1;
                }

            }
        }
        public List<SRBItemDetailsModel> GetSRBDeactivatedItem(string selItems)
        {
            try
            {
                List<SRBItemDetailsModel> details = new List<SRBItemDetailsModel>();
                var selBuyback = selItems.Split(',');
                using (var context = new IOASDBEntities())
                {
                    var query = (from deact in context.tblSRBDeactivation
                                 join det in context.tblSRBDetails on deact.tblSRBDetailId equals det.tblSRBDetailId
                                 join cc in context.tblCodeControl on
                                        new { Category = det.CategoryId ?? 0, codeName = "SRB_ItemCategory" } equals
                                        new { Category = cc.CodeValAbbr, codeName = cc.CodeName }
                                 where deact.Buyback_f == true && (deact.BuybackRefId == null || selBuyback.Contains(deact.SRBDeactivationId.ToString()))
                                 orderby det.tblSRBDetailId descending
                                 select new { det, deact.BuybackValue, deact.SRBDeactivationId, cc.CodeValDetail }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {

                            details.Add(new SRBItemDetailsModel()
                            {
                                slNo = i + 1,
                                SRBDetailId = query[i].det.tblSRBDetailId,
                                SRBDeactivationId = query[i].SRBDeactivationId,
                                ItemNumber = Common.GetSRBNumber(query[i].det.SRBId ?? 0) + "/" + query[i].det.ItemNumber,
                                ItemName = query[i].det.ItemName,
                                ItemCategory = query[i].CodeValDetail,
                                ItemValue = query[i].det.ItemValue,
                                Status = query[i].det.Status,
                                Quantity = query[i].det.Quantity.ToString(),
                                BuybackValue = query[i].BuybackValue
                            });

                        }
                    }
                }
                return details;
            }
            catch (Exception ex)
            {
                List<SRBItemDetailsModel> details = new List<SRBItemDetailsModel>();
                return details;
            }
        }
        #endregion
    }
}