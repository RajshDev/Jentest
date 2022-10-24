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
        #region RO Number
        
        public int CreateRO(CreateROModel model, int logged_in_user)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    tblProjectROSummary rOSummary = new tblProjectROSummary();
                    tblProjectROLog rOLog = new tblProjectROLog();

                    try
                    {
                        if (model.RODetails != null)
                        {
                            foreach (var item in model.RODetails)
                            {
                                //int roid = 0;
                                rOSummary.RO_Number = item.RONumber;
                                rOSummary.ProjectId = model.ProjId;
                                rOSummary.RO_ProjectValue = item.ExistingValue ?? 0;
                                rOSummary.RO_Status = "Open";
                                rOSummary.RO_InvoiceValue = 0;
                                rOSummary.RO_ReceiptValue = 0;
                                rOSummary.RO_CommitmentValue = 0;
                                rOSummary.RO_ExpenditureValue = 0;
                                rOSummary.RO_BalanceValue = item.NewValue ?? 0;
                                rOSummary.Is_Active = true;
                                rOSummary.Is_Deleted = false;
                                rOSummary.Is_TempRO = false;
                                rOSummary.Crtd_TS = DateTime.Now;
                                rOSummary.Crtd_UserId = logged_in_user;
                                context.tblProjectROSummary.Add(rOSummary);
                                context.SaveChanges();

                                item.RO_Id = rOSummary.RO_Id;
                            }
                            //transaction.Commit();

                            foreach (var itemRo in model.RODetails)
                            {
                                rOLog.RO_Id = itemRo.RO_Id;
                                rOLog.RO_ExistingValue = itemRo.ExistingValue ?? 0;
                                rOLog.RO_AddEditValue = itemRo.EditedValue ?? 0;
                                rOLog.RO_NewValue = itemRo.NewValue ?? 0;
                                rOLog.RO_LogStatus = "Open";
                                rOLog.Is_Deleted = false;
                                rOLog.Crtd_TS = DateTime.Now;
                                rOLog.Crtd_UserId = logged_in_user;
                                context.tblProjectROLog.Add(rOLog);
                                context.SaveChanges();
                            }
                        }
                        else if(model.TempRODetails != null)
                        {
                            var projectNo= Common.getprojectnumber(model.ProjId);
                            rOSummary.RO_Number = projectNo + "TEMPRO" + 2223;//model.TempRODetails.TempRONumber;
                            rOSummary.ProjectId = model.ProjId;
                            rOSummary.RO_ProjectValue = model.TempRODetails.ExistingValue ?? 0;
                            rOSummary.RO_Status = "Open";
                            rOSummary.RO_InvoiceValue = 0;
                            rOSummary.RO_ReceiptValue = 0;
                            rOSummary.RO_CommitmentValue = 0;
                            rOSummary.RO_ExpenditureValue = 0;
                            rOSummary.RO_BalanceValue = model.TempRODetails.NewValue ?? 0;
                            rOSummary.Is_Active = true;
                            rOSummary.Is_Deleted = false;
                            rOSummary.Is_TempRO = true;
                            rOSummary.Crtd_TS = DateTime.Now;
                            rOSummary.Crtd_UserId = logged_in_user;
                            context.tblProjectROSummary.Add(rOSummary);
                            context.SaveChanges();

                            model.TempRODetails.RO_Id = rOSummary.RO_Id;

                            rOLog.RO_Id = model.TempRODetails.RO_Id;
                            rOLog.RO_ExistingValue = model.TempRODetails.ExistingValue ?? 0;
                            rOLog.RO_AddEditValue = model.TempRODetails.EditedValue ?? 0;
                            rOLog.RO_NewValue = model.TempRODetails.NewValue ?? 0;
                            rOLog.RO_LogStatus = "Open";
                            rOLog.Is_Deleted = false;
                            rOLog.Crtd_TS = DateTime.Now;
                            rOLog.Crtd_UserId = logged_in_user;
                            context.tblProjectROLog.Add(rOLog);
                            context.SaveChanges();
                        }
                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {

                        Infrastructure.IOASException.Instance.HandleMe(this, ex);
                        transaction.Rollback();
                        return -1;
                    }
                }
                return 1;
            }
        }
        public static RODetailSearch GetROList(RODetailSearch model, DateFilterModel PrsntDueDate, int page, int pageSize)
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
                                 join P in context.tblProject on RO.ProjectId equals P.ProjectId
                                 join RoLog in context.tblProjectROLog on RO.RO_Id equals RoLog.RO_Id
                                 where (RO.RO_Status != "InActive" && RO.Is_Active == true)
                                 && (P.ProjectNumber.Contains(model.ProjectNumber) || model.ProjectNumber == null)
                                 && (RO.RO_Number.Contains(model.RONumber) || model.RONumber == null)
                                 && ((RO.Crtd_TS >= PrsntDueDate.@from && RO.Crtd_TS <= PrsntDueDate.to) || (PrsntDueDate.@from == null && PrsntDueDate.to == null))
                                 && (RO.RO_ProjectValue == model.ROProjValue || model.ROProjValue == null)
                                 && (RO.RO_BalanceValue == model.ROBalanceValue || model.ROBalanceValue == null)
                                 && (RO.RO_Status.Contains(model.Status) || model.Status == null)
                                 orderby RO.Crtd_TS descending
                                 select new { RO,RoLog.RO_Id, P.ProjectNumber,P.ProjectId}
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
                                ROId = query[i].RO_Id,
                                RONumber = query[i].RO.RO_Number,
                                ROProjValue = query[i].RO.RO_ProjectValue,
                                ROBalanceValue = query[i].RO.RO_BalanceValue,
                                RODate = query[i].RO.Crtd_TS,
                                PrsntDueDate = query[i].RO.Crtd_TS == null ? String.Format("{0:s}", query[i].RO.Crtd_TS) : String.Format("{0:s}", query[i].RO.Crtd_TS),
                                Status = query[i].RO.RO_Status
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
    }
}
#endregion

