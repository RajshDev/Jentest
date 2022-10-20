using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IOAS.Models;
using IOAS.DataModel;

namespace IOAS.GenericServices
{
    public class ProjectFundingCategoryService
    {
        #region RO Number
        public List<RODetailsListModel> GetRoDetails(int ProjId)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    List<RODetailsListModel> RODetails = new List<RODetailsListModel>();
                    try
                    {
                        RODetails = (from RO in context.tblProjectROSummary
                                             from ROLog in context.tblProjectROLog
                                             where RO.RO_Id == ROLog.RO_Id && RO.ProjectId == ProjId
                                             && RO.Is_Active != false
                                             select new
                                             {
                                                 ROLog.RO_Id,
                                                 ROLog.RO_ExistingValue,
                                                 ROLog.RO_AddEditValue,
                                                 ROLog.RO_NewValue,
                                                 ROLog.RO_LogStatus,
                                                 RO.RO_Number,
                                                 RO.Is_TempRO
                                             }).AsEnumerable()
                                    .Select((x) => new RODetailsListModel()
                                    {
                                        RONumber = x.RO_Number,
                                        EditedValue = x.RO_AddEditValue,
                                        ExistingValue = x.RO_ExistingValue,
                                        NewValue = x.RO_NewValue,
                                        //TempRONumber = x.RO_Number
                                    }).ToList(); 

                    }
                    catch (Exception ex)
                    {
                        Infrastructure.IOASException.Instance.HandleMe(this, ex);
                        transaction.Rollback();
                        return RODetails;
                    }
                    return RODetails;
                }
            }
                        
        }

        public int CreateRO(CreateROModel model, int logged_in_user)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    tblProjectROSummary rOSummary = new tblProjectROSummary();

                    var RO_Id = (from RO in context.tblProjectROSummary
                                 where (RO.ProjectId == model.ProjId || RO.RO_Id > 0)
                                 orderby RO.RO_Id descending
                                 select RO.RO_Id).FirstOrDefault();

                    if (RO_Id >= 0)
                    {
                        try
                        {
                            foreach (var item in model.RODetails)
                            {
                                rOSummary.RO_Number = item.RONumber;
                                rOSummary.ProjectId = model.ProjId;
                                rOSummary.RO_ProjectValue = item.ExistingValue ?? 0;
                                rOSummary.RO_Status = "open";
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
                            }
                           
                            tblProjectROLog rOLog = new tblProjectROLog();
                            foreach (var itemRoLog in model.RODetails)
                            {

                                if (RO_Id > 0)
                                {
                                    rOLog.RO_Id = RO_Id;
                                    rOLog.RO_ExistingValue = itemRoLog.ExistingValue ?? 0;
                                    rOLog.RO_AddEditValue = itemRoLog.EditedValue ?? 0;
                                    rOLog.RO_NewValue = itemRoLog.NewValue ?? 0;
                                    rOLog.RO_LogStatus = "open";
                                    rOLog.Is_Deleted = false;
                                    rOLog.Crtd_TS = DateTime.Now;
                                    rOLog.Crtd_UserId = logged_in_user;
                                    context.tblProjectROLog.Add(rOLog);
                                    context.SaveChanges();
                                }
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
        }
    }
}
        #endregion
  