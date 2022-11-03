using System;
using System.Collections.Generic;
using IOAS.Infrastructure;
using System.Linq;
using System.Web;
using IOAS.Models;
using IOAS.DataModel;

namespace IOAS.GenericServices
{
    public class ProjectFundingCategoryService
    {

        public int CreateRO(CreateROModel model, int logged_in_user)
        {

            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int? projectid;
                        decimal? totEditedVal = 0;
                        decimal? totNewval = 0;
                        string roIds = "";
                        int aprvId = 0;
                        if (model.ROAprvId > 0)
                        {/*Edit region*/
                            if (model.RODetails.Count > 0 || model.TempRODetails != null)
                            {
                                var RO = (from c in context.tblProjectROSummary
                                          where c.RO_ProjectApprovalId == model.ROAprvId && c.RO_Status == "Open"
                                          select c).FirstOrDefault();

                                if (RO != null)
                                {
                                    projectid = model.ProjId;
                                    RO.Uptd_TS = DateTime.Now;
                                    RO.Uptd_UserId = logged_in_user;
                                    //RO.RO_Number = model.RONumber;
                                    context.tblProjectROLog.RemoveRange(context.tblProjectROLog.Where(m => m.RO_ProjectApprovalId == model.ROAprvId));
                                    context.SaveChanges();
                                    tblProjectROLog rOLog = new tblProjectROLog();
                                    
                                        if (model.RODetails != null)
                                        {
                                            foreach (var item in model.RODetails)
                                            {
                                                rOLog.RO_Id = item.RO_Id;
                                                rOLog.RO_ExistingValue = item.ExistingValue ?? 0;
                                                rOLog.RO_AddEditValue = item.EditedValue ?? 0;
                                                rOLog.RO_NewValue = item.NewValue ?? 0;
                                                rOLog.RO_LogStatus = "Open";
                                                rOLog.Is_Deleted = false;
                                                rOLog.Crtd_TS = DateTime.Now;
                                                rOLog.Crtd_UserId = logged_in_user;


                                                totEditedVal += item.EditedValue;
                                                totNewval += item.ExistingValue;
                                                roIds = string.Join(",", item.RO_Id);
                                            }
                                        }
                                    if (model.isTemp == "Temp")
                                    {
                                        if (model.TempRODetails != null)
                                        {
                                            rOLog.RO_Id = model.TempRODetails.RO_Id;
                                            rOLog.RO_ExistingValue = model.TempRODetails.ExistingValue ?? 0;
                                            rOLog.RO_AddEditValue = model.TempRODetails.EditedValue ?? 0;
                                            rOLog.RO_NewValue = model.TempRODetails.NewValue ?? 0;
                                            rOLog.RO_LogStatus = "Open";
                                            rOLog.Is_Deleted = false;
                                            rOLog.Crtd_TS = DateTime.Now;
                                            rOLog.Crtd_UserId = logged_in_user;

                                            totEditedVal += model.TempRODetails.EditedValue;
                                            totNewval += model.TempRODetails.ExistingValue;
                                            roIds = string.Join(",", model.TempRODetails.RO_Id);
                                        }
                                    }
                                    rOLog.RO_ProjectApprovalId = model.ROAprvId;
                                    context.tblProjectROLog.Add(rOLog);
                                    context.SaveChanges();
                                    context.tblProjectROApprovalRequest.Where(m => m.RO_ProjectApprovalId == model.ROAprvId)
                                            .ToList()
                                            .ForEach(m =>
                                            {
                                                m.Crtd_TS = DateTime.Now;
                                                m.Crtd_UserId = logged_in_user;
                                                m.RO_Id_List = roIds;
                                                m.RO_TotalAddEditValue = totEditedVal;
                                            });
                                    transaction.Commit();
                                    return 1;
                                }
                            }
                        }

                        else
                        {
                            tblProjectROSummary roSum = new tblProjectROSummary();
                            tblProjectROLog rOLog = new tblProjectROLog();
                            tblProjectROApprovalRequest roAprvRq = new tblProjectROApprovalRequest();

                            if (model.isTemp == "Temp")
                            {
                                if (model.TempRODetails != null)
                                {
                                    /*temp RO region*/
                                    model.TempRODetails.TempRONumber = "TEMPRO_" + Common.getprojectnumber(model.ProjId);
                                    roSum.ProjectId = model.ProjId;
                                    roSum.RO_Number = model.TempRODetails.TempRONumber;
                                    roSum.RO_Status = "Open";
                                    roSum.Crtd_TS = DateTime.Now;
                                    roSum.Crtd_UserId = logged_in_user;
                                    roSum.Is_TempRO = true;
                                    roSum.Is_Active = true;

                                    context.tblProjectROSummary.Add(roSum);
                                    context.SaveChanges();
                                    model.TempRODetails.RO_Id = roSum.RO_Id;
                                }
                            }
                            /*Create Region*/
                           
                                if (model.RODetails.Count > 0)
                                {
                                    foreach (var item in model.RODetails)
                                    {
                                        roSum.ProjectId = model.ProjId;
                                        roSum.RO_Number = item.RONumber;
                                        roSum.RO_Status = "Open";
                                        roSum.Crtd_TS = DateTime.Now;
                                        roSum.Crtd_UserId = logged_in_user;
                                        roSum.Is_Active = true;
                                        roSum.Is_TempRO = false;

                                        context.tblProjectROSummary.Add(roSum);
                                        context.SaveChanges();
                                        item.RO_Id = roSum.RO_Id;
                                    }
                                }
                           
                            if (model.isTemp == "Temp")
                            {

                                if (model.TempRODetails != null)
                                {

                                    rOLog.RO_Id = model.TempRODetails.RO_Id;
                                    rOLog.RO_ExistingValue = model.TempRODetails.ExistingValue ?? 0;
                                    rOLog.RO_AddEditValue = model.TempRODetails.EditedValue ?? 0;
                                    rOLog.RO_NewValue = model.TempRODetails.NewValue ?? 0;
                                    rOLog.RO_LogStatus = "Open";
                                    rOLog.Is_Deleted = false;
                                    rOLog.Crtd_TS = DateTime.Now;
                                    rOLog.Crtd_UserId = logged_in_user;
                                    totEditedVal += model.TempRODetails.EditedValue;
                                    totNewval += model.TempRODetails.ExistingValue;
                                    context.tblProjectROLog.Add(rOLog);
                                    context.SaveChanges();
                                    roIds += model.TempRODetails.RO_Id.ToString() + ",";
                                }
                            }

                            
                                if (model.RODetails.Count > 0)
                                {
                                    foreach (var item in model.RODetails)
                                    {
                                        rOLog.RO_Id = item.RO_Id;
                                        rOLog.RO_ExistingValue = item.ExistingValue ?? 0;
                                        rOLog.RO_AddEditValue = item.EditedValue ?? 0;
                                        rOLog.RO_NewValue = item.NewValue ?? 0;
                                        rOLog.RO_LogStatus = "Open";
                                        rOLog.Is_Deleted = false;
                                        rOLog.Crtd_TS = DateTime.Now;
                                        rOLog.Crtd_UserId = logged_in_user;
                                        totEditedVal += item.EditedValue;
                                        totNewval += item.ExistingValue;

                                        context.tblProjectROLog.Add(rOLog);
                                        context.SaveChanges();
                                        roIds += item.RO_Id.ToString() + ",";
                                    }
                                }
                            
                            roAprvRq.RO_Id_List = roIds.Remove(roIds.Length - 1);
                            roAprvRq.RO_TotalAddEditValue = totEditedVal;
                            roAprvRq.RO_TotNewValue = totNewval;
                            roAprvRq.Crtd_TS = DateTime.Now;
                            roAprvRq.Crtd_UserId = logged_in_user;
                            context.tblProjectROApprovalRequest.Add(roAprvRq);
                            context.SaveChanges();

                            /*Update AprvdID in Summary table*/
                            aprvId = roAprvRq.RO_ProjectApprovalId;
                            var id = roAprvRq.RO_Id_List.Split(',').Select(int.Parse).ToList();
                            foreach (var a in id)
                            {
                                context.tblProjectROSummary.Where(m => m.RO_Id == a).
                                    ToList().
                                    ForEach(
                                    m => m.RO_ProjectApprovalId = aprvId
                                    );


                                context.tblProjectROLog.Where(m => m.RO_Id == a).ToList().
                                    ForEach(
                                    m => m.RO_ProjectApprovalId = aprvId
                                    );
                                context.SaveChanges();

                            }
                        }
                        transaction.Commit();
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return -1;
                    }
                }
            }
        }

        public static RODetailSearch GetROList(RODetailSearch model, int page, int pageSize)
        {
            RODetailSearch searchRO = new RODetailSearch();
            List<CreateROModel> ROModels = new List<CreateROModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    int skiprec = 0;
                    if (page == 1)
                    {
                        skiprec = 0;
                    }
                    else
                    {
                        skiprec = (page - 1) * pageSize;
                    }

                    var query = (from RO in context.tblProjectROSummary
                                 group RO by RO.RO_ProjectApprovalId into c
                                 //join RoLog in context.tblProjectROLog on RO.RO_Id equals RoLog.RO_Id
                                 join P in context.tblProject on c.FirstOrDefault().ProjectId equals P.ProjectId
                                 join AH in context.tblAccountHead on P.BankID equals AH.AccountHeadId
                                 join fa in context.vwFacultyStaffDetails_TSA on P.PIName equals fa.UserId
                                 where (c.FirstOrDefault().RO_Status != "InActive" && c.FirstOrDefault().Is_Active == true)
                                 && (P.ProjectNumber.Contains(model.ProjectNumber) || model.ProjectNumber == null)
                                 && (AH.AccountHead.Contains(model.BankName) || model.BankName == null)
                                 && (fa.PiIdName.Contains(model.PIdName) || model.PIdName == null)
                                 //&& (RoLog.RO_ExistingValue == model.ROExistingValue || model.ROExistingValue == null)
                                 //&& (RoLog.RO_AddEditValue == model.ROEditedValue || model.ROEditedValue == null)
                                 && (c.FirstOrDefault().RO_Status.Contains(model.Status) || model.Status == null)
                                 orderby c.FirstOrDefault().Crtd_TS descending
                                 select new
                                 {
                                     c,
                                     c.FirstOrDefault().RO_Number,
                                     c.FirstOrDefault().RO_ProjectValue,
                                     c.FirstOrDefault().RO_Status,
                                     c.FirstOrDefault().RO_ProjectApprovalId,
                                     P.ProjectNumber,
                                     P.ProjectId,
                                     fa.PiIdName,
                                     AH.AccountHead
                                 }
                                     ).Skip(skiprec).Take(pageSize).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            ROModels.Add(new CreateROModel()
                            {
                                Sno = i + 1,
                                ProjId = query[i].ProjectId,
                                ProjectNumber = query[i].ProjectNumber,
                                PIdName = query[i].PiIdName,
                                BankName = query[i].AccountHead,
                                //ROId = query[i].RO_Id,
                                RONumber = query[i].RO_Number,
                                // ROBalanceValue = query[i].RO_AddEditValue,
                                ROProjValue = query[i].RO_ProjectValue,
                                //RODate = query[i].RO.Crtd_TS,
                                Status = query[i].RO_Status,
                                ROAprvId = query[i].RO_ProjectApprovalId
                            });
                        }
                    }
                    searchRO.list = ROModels;
                    searchRO.TotalRecords = query.Count();
                }
                return searchRO;
            }
            catch (Exception ex)
            {
                return searchRO;
            }

        }
        /*Process flow for RO for a particular request*/
        public bool ProjectROWFInit(int AprvdId, int projId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var queryRO = context.tblProjectROSummary.FirstOrDefault(m => m.RO_ProjectApprovalId == AprvdId && m.ProjectId == projId && m.RO_Status == "Open");
                    var queryROLog = context.tblProjectROLog.Where(m => m.RO_ProjectApprovalId == AprvdId && m.RO_LogStatus == "Open").ToList();
                    var queryApprovalId = context.tblProjectROApprovalRequest.Where(m => m.RO_ProjectApprovalId == AprvdId).FirstOrDefault();
                    if (queryRO != null && queryROLog != null)
                    {

                        var fw = FlowEngine.Init(349, logged_in_user, AprvdId, "ROProjectApprovalId");
                        string url = "/ProjectFundingCategory/ViewRODetails?ProjectId=" + projId + "&AprvdId=" + AprvdId + "&Pfinit=true";
                        fw.ActionLink(url);
                        fw.FunctionId(236);
                        string pNo = Common.GetProjectNumber(projId);
                        fw.ReferenceNumber(pNo);
                        fw.ClarifyMethod("ProjectROWFInitClarify");
                        fw.SuccessMethod("ProjectROWFInitSuccess");
                        fw.ProcessInit();
                        if (String.IsNullOrEmpty(fw.errorMsg))
                        {

                            context.tblProjectROSummary.Where(m => m.RO_ProjectApprovalId == AprvdId && m.RO_Status == "Open")
                                .ToList()
                                .ForEach(m =>
                                {
                                    m.RO_Status = "Submit for approval";
                                    m.Uptd_UserId = logged_in_user;
                                    m.Uptd_TS = DateTime.Now;
                                });


                            context.tblProjectROLog.Where(p => p.RO_ProjectApprovalId == AprvdId && p.RO_LogStatus == "Open")
                                       .ToList()
                                       .ForEach(m =>
                                       {
                                           m.RO_LogStatus = "Submit for approval";
                                           m.Uptd_UserId = logged_in_user;
                                           m.Uptd_TS = DateTime.Now;
                                       });
                            context.SaveChanges();
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}