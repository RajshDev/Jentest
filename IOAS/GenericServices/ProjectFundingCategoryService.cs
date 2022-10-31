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
                    int? projectid;
                    decimal? totEditedVal = 0;
                    decimal? totNewval = 0;
                    string roIds = "";
                    int aprvId = 0;
                    if (model.ROId > 0)
                    {/*Edit region*/
                        if (model.RODetails.Count > 0)
                        {
                            int ROid = Convert.ToInt32(model.ROId);
                            var RO = (from c in context.tblProjectROSummary
                                      where c.RO_Id == model.ROId && c.RO_Status == "Open"
                                      select c).FirstOrDefault();

                            if (RO != null)
                            {
                                projectid = model.ProjId;
                                RO.Uptd_TS = DateTime.Now;
                                RO.Uptd_UserId = logged_in_user;
                                //RO.RO_Number = model.RONumber;
                                context.tblProjectROLog.RemoveRange(context.tblProjectROLog.Where(m => m.RO_Id == ROid));
                                context.SaveChanges();
                                if (model.RODetails != null)
                                {
                                    tblProjectROLog rOLog = new tblProjectROLog();
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
                                    context.tblProjectROLog.Add(rOLog);
                                    context.SaveChanges();

                                    tblProjectROApprovalRequest rOAprvRq = new tblProjectROApprovalRequest();
                                    rOAprvRq.Crtd_TS = DateTime.Now;
                                    rOAprvRq.Crtd_UserId = logged_in_user;
                                    rOAprvRq.RO_Id_List = roIds;
                                    rOAprvRq.RO_TotalAddEditValue = totEditedVal;
                                    context.tblProjectROApprovalRequest.Add(rOAprvRq);
                                    context.SaveChanges();


                                }
                                transaction.Commit();
                                return 1;
                            }

                            else
                            {//temp ro Region
                            }
                        }
                    }

                    else
                    {
                        /*Create Region*/
                        //if (model.RODetails.Count > 0)
                        //{
                        tblProjectROSummary roSum = new tblProjectROSummary();
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


                        tblProjectROLog rOLog = new tblProjectROLog();

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


                        tblProjectROApprovalRequest roAprvRq = new tblProjectROApprovalRequest();

                        roAprvRq.RO_Id_List = roIds.Remove(roIds.Length - 1);
                        //roAprvRq.RO_Id_List = roIds;
                        roAprvRq.RO_TotalAddEditValue = totEditedVal;
                        roAprvRq.RO_TotNewValue = totNewval;
                        roAprvRq.Crtd_TS = DateTime.Now;
                        roAprvRq.Crtd_UserId = logged_in_user;
                        
                        context.tblProjectROApprovalRequest.Add(roAprvRq);
                        context.SaveChanges();

                        /*Update AprvdID in Summary table*/
                        aprvId = roAprvRq.RO_ProjectApprovalId;
                        var id = roAprvRq.RO_Id_List.Split(',').Select(int.Parse).ToList();
                        foreach (var a in id) {
                            context.tblProjectROSummary.Where(m => m.RO_Id == a).
                                ToList().
                                ForEach(
                                m => m.RO_ProjectApprovalId = aprvId
                                );


                            context.tblProjectROLog.Where(m => m.RO_Id == a).ToList().
                                ForEach(
                                m=>m.RO_ProjectApprovalId = aprvId
                                );
                            context.SaveChanges();


                        }
                        
                    }
                    //else if(model.TempRODetails.count>0)
                    //    {
                    //        /*temp RO region*/

                    //    }
                    //context.tblProjectROLog.Where(x=>x.get log id

                        transaction.Commit();

                    }
                }
                return 1;
            }
            #region RO Number

            //public int CreateRO1(CreateROModel model, int logged_in_user)
            //    {
            //        using (var context = new IOASDBEntities())
            //        {
            //            using (var transaction = context.Database.BeginTransaction())
            //            {
            //                tblProjectROSummary rOSummary = new tblProjectROSummary();
            //                tblProjectROLog rOLog = new tblProjectROLog();
            //                tblProjectROApprovalRequest roAprvRq = new tblProjectROApprovalRequest();

            //                string RO_Id_List = "";
            //                try
            //                {
            //                    if (model.RODetails != null)
            //                    {
            //                        foreach (var item in model.RODetails)
            //                        {
            //                            //int roid = 0;
            //                            rOSummary.RO_Number = item.RONumber;
            //                            rOSummary.ProjectId = model.ProjId;
            //                            rOSummary.RO_ProjectValue = item.NewValue ?? 0;
            //                            rOSummary.RO_Status = "Open";
            //                            rOSummary.RO_InvoiceValue = 0;
            //                            rOSummary.RO_ReceiptValue = 0;
            //                            rOSummary.RO_CommitmentValue = 0;
            //                            rOSummary.RO_ExpenditureValue = 0;
            //                            rOSummary.RO_BalanceValue = 0;
            //                            rOSummary.Is_Active = true;
            //                            rOSummary.Is_Deleted = false;
            //                            rOSummary.Is_TempRO = false;
            //                            rOSummary.Crtd_TS = DateTime.Now;
            //                            rOSummary.Crtd_UserId = logged_in_user;
            //                            context.tblProjectROSummary.Add(rOSummary);
            //                            context.SaveChanges();

            //                            item.RO_Id = rOSummary.RO_Id;
            //                        }

            //                        foreach (var itemRo in model.RODetails)
            //                        {
            //                            rOLog.RO_Id = itemRo.RO_Id;
            //                            rOLog.RO_ExistingValue = itemRo.ExistingValue ?? 0;
            //                            rOLog.RO_AddEditValue = itemRo.EditedValue ?? 0;
            //                            rOLog.RO_NewValue = itemRo.NewValue ?? 0;
            //                            rOLog.RO_LogStatus = "Open";
            //                            rOLog.Is_Deleted = false;
            //                            rOLog.Crtd_TS = DateTime.Now;
            //                            rOLog.Crtd_UserId = logged_in_user;
            //                            context.tblProjectROLog.Add(rOLog);
            //                            context.SaveChanges();
            //                            RO_Id_List = string.Join(",",itemRo.RO_Id);
            //                        }



            //                    }
            //                    else if (model.TempRODetails != null)
            //                    {
            //                        var projectNo = Common.getprojectnumber(model.ProjId);
            //                        rOSummary.RO_Number = projectNo + "TEMPRO" + 2223;//model.TempRODetails.TempRONumber;
            //                        rOSummary.ProjectId = model.ProjId;
            //                        rOSummary.RO_ProjectValue = model.TempRODetails.ExistingValue ?? 0;
            //                        rOSummary.RO_Status = "Open";
            //                        rOSummary.RO_InvoiceValue = 0;
            //                        rOSummary.RO_ReceiptValue = 0;
            //                        rOSummary.RO_CommitmentValue = 0;
            //                        rOSummary.RO_ExpenditureValue = 0;
            //                        rOSummary.RO_BalanceValue = model.TempRODetails.NewValue ?? 0;
            //                        rOSummary.Is_Active = false;
            //                        rOSummary.Is_Deleted = false;
            //                        rOSummary.Is_TempRO = true;
            //                        rOSummary.Crtd_TS = DateTime.Now;
            //                        rOSummary.Crtd_UserId = logged_in_user;
            //                        context.tblProjectROSummary.Add(rOSummary);
            //                        context.SaveChanges();

            //                        model.TempRODetails.RO_Id = rOSummary.RO_Id;

            //                        rOLog.RO_Id = model.TempRODetails.RO_Id;
            //                        rOLog.RO_ExistingValue = model.TempRODetails.ExistingValue ?? 0;
            //                        rOLog.RO_AddEditValue = model.TempRODetails.EditedValue ?? 0;
            //                        rOLog.RO_NewValue = model.TempRODetails.NewValue ?? 0;
            //                        rOLog.RO_LogStatus = "Open";

            //                        rOLog.Is_Deleted = false;
            //                        rOLog.Crtd_TS = DateTime.Now;
            //                        rOLog.Crtd_UserId = logged_in_user;

            //                        RO_Id_List = string.Join(",", rOLog.RO_Id);
            //                        context.tblProjectROLog.Add(rOLog);
            //                        context.SaveChanges();


            //                    }
            //                    else
            //                    {
            //                        //var RONumber = Common.GetRONumber(model.ROId ?? 0);
            //                        //rOLog.RO_Id = model.
            //                    }
            //                    roAprvRq.RO_Id_List = RO_Id_List;
            //                    roAprvRq.RO_TotalAddEditValue = Common.GetTotEditedValue(model.ProjId);
            //                    roAprvRq.Crtd_TS = DateTime.Now;
            //                    roAprvRq.Crtd_UserId = logged_in_user;
            //                    context.tblProjectROApprovalRequest.Add(roAprvRq);
            //                    context.SaveChanges();

            //                    transaction.Commit();

            //                }
            //                catch (Exception ex)
            //                {

            //                    Infrastructure.IOASException.Instance.HandleMe(this, ex);
            //                    transaction.Rollback();
            //                    return -1;
            //                }
            //            }
            //            return 1;
            //        }
            //    }
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
                                 select new { c, c.FirstOrDefault().RO_Number, c.FirstOrDefault().RO_ProjectValue,
                                 c.FirstOrDefault().RO_Status,c.FirstOrDefault().RO_ProjectApprovalId,
                                     P.ProjectNumber,P.ProjectId, fa.PiIdName ,AH.AccountHead}
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

#endregion

