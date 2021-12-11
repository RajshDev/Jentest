using IOAS.DataModel;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace IOAS.GenericServices
{
    public class UCService
    {
        public PagedData<UCListModel> GetUCList(SearchUCModel srchModel)
        {
            PagedData<UCListModel> model = new PagedData<UCListModel>();
            var list = new List<UCListModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    int skiprec = 0;
                    if (srchModel.pageIndex == 1)
                    {
                        skiprec = 0;
                    }
                    else
                    {
                        skiprec = (srchModel.pageIndex - 1) * srchModel.pageSize;
                    }
                    var predicate = PredicateBuilder.BaseAnd<UCListModel>();
                    if (!string.IsNullOrEmpty(srchModel.FinYear))
                        predicate = predicate.And(d => d.FinYear.Contains(srchModel.FinYear));

                    if (!string.IsNullOrEmpty(srchModel.Type))
                        predicate = predicate.And(d => d.Type.Contains(srchModel.Type));
                    if (!string.IsNullOrEmpty(srchModel.ProjectNumber))
                        predicate = predicate.And(d => d.ProjectNumber.Contains(srchModel.ProjectNumber));
                    if (!string.IsNullOrEmpty(srchModel.UCNumber))
                        predicate = predicate.And(d => d.UCNumber.Contains(srchModel.UCNumber));
                    if (srchModel.ProjectId != null)
                        predicate = predicate.And(m => m.ProjectId == srchModel.ProjectId);
                    if (srchModel.UCFrom != null && srchModel.UCTo != null)
                    {
                        srchModel.UCTo = srchModel.UCTo.Value.Date.AddDays(1).AddTicks(-2);
                        predicate = predicate.And(d => d.UCDate >= srchModel.UCFrom && d.UCDate <= srchModel.UCTo);
                    }
                    var query = (from uc in context.tblUCHead
                                 join fin in context.tblFinYear on uc.FinancialYear equals fin.FinYearId
                                 join cc in context.tblCodeControl on
                                             new { type = uc.TypeofUC ?? 0, codeName = "TypeofUC" } equals
                                             new { type = cc.CodeValAbbr, codeName = cc.CodeName }
                                 where uc.Status != "InActive"
                                 select new UCListModel
                                 {
                                     UCDate = uc.CRTD_TS,
                                     FinYear = fin.Year,
                                     ProjectId = uc.ProjectId,
                                     UCId = uc.UCId,
                                     ProjectNumber = uc.ProjectNumber,
                                     Type = cc.CodeValDetail,
                                     UCNumber = uc.UCNumber,
                                     Status = uc.Status,
                                     TypeofUC = uc.TypeofUC
                                 });
                    list = query.Where(predicate)
                               .OrderByDescending(m => m.UCId)
                                 .AsEnumerable()
                                 .Select((x, index) => new UCListModel()
                                 {
                                     SlNo = index + 1,
                                     FinYear = x.FinYear,
                                     ProjectId = x.ProjectId,
                                     UCId = x.UCId,
                                     Type = x.Type,
                                     UCNumber = x.UCNumber,
                                     ProjectNumber = x.ProjectNumber,
                                     UCDateStr = String.Format("{0:dd MMM yyyy}", x.UCDate),
                                     Status = x.Status,
                                     IsEditable = x.TypeofUC == 3 ? true : CheckIsEditable(x.UCId ?? 0, x.ProjectId)
                                 }).Skip(skiprec).Take(srchModel.pageSize).ToList();

                    model.TotalRecords = query.Where(predicate).Count();

                }
                model.Data = list;
                return model;
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(this, ex);
                model.Data = list;
                return model;
            }
        }
        public UCModel GetUCDetail(int UCId)
        {
            UCModel model = new UCModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var uc = context.tblUCHead.FirstOrDefault(m => m.UCId == UCId);
                    if (uc != null)
                    {
                        model.FinancialYear = uc.FinancialYear;
                        model.ProjectId = uc.ProjectId;
                        model.ProjectNumber = uc.ProjectNumber;
                        model.Remarks = uc.Remarks;
                        model.TemplateId = uc.TemplateId;
                        model.TotalExpenditure = uc.TotalExpenditure;
                        model.TypeOfUC = uc.TypeofUC;
                        model.UCRawFile = uc.UCRawFile;

                        model.ExpDetails = (from d in context.tblUCExpenditureDetail
                                            join h in context.tblBudgetHead on d.HeadId equals h.BudgetHeadId
                                            where d.UCId == UCId && d.Status == "Active"
                                            select new UCExpenditureModel()
                                            {
                                                ExpenditureAsPerBook = d.ExpenditureAsPerBook ?? 0,
                                                ExpenditureAsPerUC = d.ExpenditureAsPerUC ?? 0,
                                                CommitmentTreatedAsExp = d.CommitmentTreatedAsExp ?? 0,
                                                AllocationHeadId = d.HeadId,
                                                AllocationHead = h.HeadName,
                                                UCCommitmentWithdrawn = d.UCCommitmentWithdrawn ?? 0,
                                                UCCommitmentUtilized = d.UCCommitmentUtilized ?? 0,
                                                UCCommitmentUnutilized = d.UCCommitmentUnutilized ?? 0
                                            }).ToList();

                        model.CommitDetails = (from d in context.tblUCCommitmentDetail
                                               join c in context.tblCommitment on d.CommitmentId equals c.CommitmentId
                                               join cd in context.tblCommitmentDetails on c.CommitmentId equals cd.CommitmentId
                                               join h in context.tblBudgetHead on cd.AllocationHeadId equals h.BudgetHeadId
                                               where d.UCId == UCId && d.Status == "Active"
                                               select new UCCommitmentModel()
                                               {
                                                   AllocationHead = h.HeadName,
                                                   AllocationHeadId = h.BudgetHeadId,
                                                   CommitmentId = d.CommitmentId,
                                                   AvailableBalance = d.AmountTreatedAsExpenditure,
                                                   CommitmentAmount = c.CommitmentAmount,
                                                   CommitmentNumber = c.CommitmentNumber
                                               }).ToList();

                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(this, ex);
                return model;
            }
        }
        public int CreateUC(UCModel model, int logged_in_user)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.TypeOfUC == 1)
                        {
                            var existsQuery = context.tblUCHead.FirstOrDefault(m => m.ProjectId == model.ProjectId && m.FinancialYear == model.FinancialYear && m.TypeofUC == 1 && m.Status != "InAvtive" && m.UCId != model.UCId);
                            if (existsQuery != null)
                                return -2;
                            else if (model.UCId > 0)
                            {
                                var chkPreProQuery = context.tblUCHead.FirstOrDefault(m => m.ProjectId == model.ProjectId && m.Status != "InAvtive" && m.TypeofUC == 2 && m.UCId > model.UCId);
                                if (existsQuery != null)
                                    return -6;
                            }
                        }
                        else if (model.TypeOfUC == 2)
                        {
                            var existsQuery = context.tblUCHead.FirstOrDefault(m => m.ProjectId == model.ProjectId && m.Status != "InAvtive" && ((m.FinancialYear == model.FinancialYear && m.TypeofUC == 1) || m.TypeofUC == 3));
                            if (existsQuery != null)
                                return -4;
                            else if (model.UCId > 0)
                            {
                                var chkPreProQuery = context.tblUCHead.FirstOrDefault(m => m.ProjectId == model.ProjectId && m.Status != "InAvtive" && m.TypeofUC == 2 && m.UCId > model.UCId);
                                if (existsQuery != null)
                                    return -5;
                            }
                        }
                        else if (model.TypeOfUC == 3)
                        {
                            var existsQuery = context.tblUCHead.FirstOrDefault(m => m.ProjectId == model.ProjectId && m.Status != "InAvtive" && m.TypeofUC == 3 && m.UCId != model.UCId);
                            if (existsQuery != null)
                                return -3;
                        }
                        if (model.UCId == 0)
                        {

                            int UCId = 0;
                            tblUCHead uc = new tblUCHead();
                            uc.FinancialYear = model.FinancialYear;
                            uc.ProjectId = model.ProjectId;
                            uc.ProjectNumber = Common.GetProjectNumber(model.ProjectId ?? 0, true);
                            uc.UCNumber = Common.GetNewUCNo("UC");
                            uc.CRTD_By = logged_in_user;
                            uc.TemplateId = model.TemplateId;
                            uc.CRTD_TS = DateTime.Now;
                            uc.Status = "Open";
                            uc.Remarks = model.Remarks;
                            uc.TotalExpenditure = model.TotalExpenditure;
                            uc.TypeofUC = model.TypeOfUC;
                            uc.UCRawFile = model.UCRawFile;
                            context.tblUCHead.Add(uc);
                            context.SaveChanges();
                            UCId = uc.UCId;
                            foreach (var item in model.CommitDetails ?? new List<UCCommitmentModel>())
                            {
                                tblUCCommitmentDetail cd = new tblUCCommitmentDetail();
                                cd.CommitmentId = item.CommitmentId;
                                cd.AmountTreatedAsExpenditure = item.AvailableBalance;
                                cd.UCId = UCId;
                                cd.Status = "Active";
                                context.tblUCCommitmentDetail.Add(cd);
                                context.SaveChanges();
                            }
                            foreach (var item in model.ExpDetails ?? new List<UCExpenditureModel>())
                            {
                                tblUCExpenditureDetail exp = new tblUCExpenditureDetail();
                                exp.UCId = UCId;
                                exp.ExpenditureAsPerUC = item.ExpenditureAsPerUC;
                                exp.ExpenditureAsPerBook = item.ExpenditureAsPerBook;
                                exp.CommitmentTreatedAsExp = item.CommitmentTreatedAsExp;
                                exp.HeadId = item.AllocationHeadId;
                                exp.UCCommitmentUnutilized = item.UCCommitmentUnutilized;
                                exp.UCCommitmentUtilized = item.UCCommitmentUtilized;
                                exp.UCCommitmentWithdrawn = item.UCCommitmentWithdrawn;
                                exp.Status = "Active";
                                context.tblUCExpenditureDetail.Add(exp);
                                context.SaveChanges();
                            }

                            transaction.Commit();
                            return UCId;
                        }
                        else
                        {
                            int UCId = model.UCId;
                            var uc = context.tblUCHead.FirstOrDefault(m => m.UCId == UCId && m.Status == "Open");
                            if (uc != null)
                            {
                                uc.FinancialYear = model.FinancialYear;
                                uc.ProjectId = model.ProjectId;
                                uc.ProjectNumber = Common.GetProjectNumber(model.ProjectId ?? 0, true);
                                uc.UPTD_By = logged_in_user;
                                uc.UPTD_TS = DateTime.Now;
                                uc.Remarks = model.Remarks;
                                uc.TemplateId = model.TemplateId;
                                uc.TotalExpenditure = model.TotalExpenditure;
                                uc.TypeofUC = model.TypeOfUC;
                                uc.UCRawFile = model.UCRawFile;
                                context.SaveChanges();

                                context.tblUCCommitmentDetail.RemoveRange(context.tblUCCommitmentDetail.Where(m => m.UCId == UCId));
                                context.SaveChanges();
                                foreach (var item in model.CommitDetails ?? new List<UCCommitmentModel>())
                                {
                                    tblUCCommitmentDetail cd = new tblUCCommitmentDetail();
                                    cd.CommitmentId = item.CommitmentId;
                                    cd.AmountTreatedAsExpenditure = item.AvailableBalance;
                                    cd.UCId = UCId;
                                    cd.Status = "Active";
                                    context.tblUCCommitmentDetail.Add(cd);
                                    context.SaveChanges();
                                }
                                context.tblUCExpenditureDetail.RemoveRange(context.tblUCExpenditureDetail.Where(m => m.UCId == UCId));
                                context.SaveChanges();
                                foreach (var item in model.ExpDetails ?? new List<UCExpenditureModel>())
                                {
                                    tblUCExpenditureDetail exp = new tblUCExpenditureDetail();
                                    exp.UCId = UCId;
                                    exp.ExpenditureAsPerUC = item.ExpenditureAsPerUC;
                                    exp.ExpenditureAsPerBook = item.ExpenditureAsPerBook;
                                    exp.CommitmentTreatedAsExp = item.CommitmentTreatedAsExp;
                                    exp.HeadId = item.AllocationHeadId;
                                    exp.UCCommitmentUnutilized = item.UCCommitmentUnutilized;
                                    exp.UCCommitmentUtilized = item.UCCommitmentUtilized;
                                    exp.UCCommitmentWithdrawn = item.UCCommitmentWithdrawn;
                                    exp.Status = "Active";
                                    context.tblUCExpenditureDetail.Add(exp);
                                    context.SaveChanges();
                                }
                                transaction.Commit();
                                return UCId;
                            }
                            else
                                return -1;

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

        public List<UCExpenditureModel> GetExpenditure(int projectId, int UCId, int UCType, int finYear)
        {
            try
            {
                List<UCExpenditureModel> List = new List<UCExpenditureModel>();
                using (var context = new IOASDBEntities())
                {
                    var data = Common.GetFinPeriod(finYear);
                    DateTime startDate = data.Item1;
                    DateTime toDate = data.Item2;
                    //toDate = toDate.AddDays(1).AddTicks(-2);
                    var qryHead = (from C in context.tblCommitment
                                   join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                   where C.ProjectId == projectId && C.Status == "Active"
                                   select new { AllocationHead = D.AllocationHeadId }).ToList();
                    var queryAlloc = (from C in context.tblProjectAllocation
                                      where C.ProjectId == projectId
                                      select C).OrderBy(c => c.AllocationId).ToList();
                    var queryPFT = (from C in context.tblProjectTransfer
                                    join D in context.tblProjectTransferDetails on C.ProjectTransferId equals D.ProjectTransferId
                                    where (C.DebitProjectId == projectId || C.CreditProjectId == projectId) && C.Status == "Completed"
                                    select new { AllocationHead = D.BudgetHeadId }).ToList();
                    var queryOpenExp = (from C in context.tblProjectOBDetail
                                        where C.ProjectId == projectId
                                        select new { AllocationHead = C.HeadId }).ToList();
                    var queryEnhAlloc = (from C in context.tblProjectEnhancementAllocation
                                         where C.ProjectId == projectId && C.IsCurrentVersion == true
                                         select C).OrderBy(c => c.AllocationHead).ToList();
                    var headname = qryHead.Select(m => m.AllocationHead).Distinct().ToArray();
                    var distCount = queryAlloc.Select(m => m.AllocationHead).Distinct().ToArray();
                    var distEnha = queryEnhAlloc.Select(m => m.AllocationHead).Distinct().ToArray();
                    var distOpen = queryOpenExp.Select(m => m.AllocationHead).Distinct().ToArray();
                    var distPFT = queryPFT.Select(m => m.AllocationHead).Distinct().ToArray();
                    List<int?> headOH = new List<int?>();
                    headOH.Add(6);
                    int?[] TotalHead;
                    TotalHead = distCount.Union(distEnha).Union(headname).Union(distOpen).Union(distPFT).Union(headOH).ToArray();
                    if (TotalHead.Length > 0)
                    {
                        var arryPrvCommitment = (from uc in context.tblUCHead
                                                 join c in context.tblUCCommitmentDetail on uc.UCId equals c.UCId
                                                 where uc.FinancialYear != finYear && uc.TypeofUC == 1
                                                 select c.CommitmentId).ToList();
                        for (int i = 0; i < TotalHead.Length; i++)
                        {
                            decimal prevAdjComm = 0;
                            int headId = TotalHead[i] ?? 0;
                            var HeadName = context.tblBudgetHead.Where(m => m.BudgetHeadId == headId).Select(m => m.HeadName).FirstOrDefault();

                            var qryBalance = (from exp in context.vw_ProjectExpenditureReport.AsNoTracking()
                                              where exp.ProjectId == projectId && exp.AllocationHeadId == headId
                                              && exp.CommitmentDate >= startDate && exp.CommitmentDate <= toDate
                                              && (!arryPrvCommitment.Contains(exp.CommitmentId))
                                              select exp).ToList();
                            var qryBookBalance = (from exp in context.vw_ProjectExpenditureReport.AsNoTracking()
                                                  where exp.ProjectId == projectId && exp.AllocationHeadId == headId
                                                  && exp.CommitmentDate >= startDate && exp.CommitmentDate <= toDate
                                                  select exp).ToList();
                            decimal SpentAmountWOPreC = qryBalance.Select(m => m.AmountSpent).Sum() ?? 0;
                            decimal SpentAmount = qryBookBalance.Select(m => m.AmountSpent).Sum() ?? 0;

                            //if (finYear == 48)
                            //{
                            //    var queryOB = (from C in context.tblProjectOBDetail
                            //                   where C.ProjectId == projectId && C.HeadId == headId
                            //                   select C).FirstOrDefault();
                            //    if (queryOB != null)
                            //        OB = queryOB.OpeningExp ?? 0;
                            //}
                            //if (UCType == 2)
                            prevAdjComm = (from C in context.tblUCCommitmentDetail
                                           join uc in context.tblUCHead on C.UCId equals uc.UCId
                                           join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                           where D.AllocationHeadId == headId && C.Status == "Active"
                                           //&& uc.FinancialYear == finYear
                                           && uc.TypeofUC == 1
                                           && uc.UCId != UCId
                                           && uc.ProjectId == projectId
                                           select D).Sum(m => m.BalanceAmount) ?? 0;
                            var utilizedAmt = (from exp in context.vw_ProjectExpenditureReport.AsNoTracking()
                                               where exp.ProjectId == projectId && exp.AllocationHeadId == headId
                                               && exp.CommitmentDate >= startDate && exp.CommitmentDate <= toDate
                                               && arryPrvCommitment.Contains(exp.CommitmentId)
                                               select exp.AmountSpent).Sum() ?? 0;
                            var withdrawnAmt = (from c in context.tblCommitment
                                                join cd in context.tblCommitmentDetails on c.CommitmentId equals cd.CommitmentId
                                                join cl in context.tblCommitmentClosedLog on c.CommitmentId equals cl.CommitmentID
                                                where cl.CRTD_TS >= startDate && cl.CRTD_TS <= toDate && cd.AllocationHeadId == headId
                                               && arryPrvCommitment.Contains(c.CommitmentId)
                                                select cd.ClosedAmount).Sum() ?? 0;
                            List.Add(new UCExpenditureModel()
                            {
                                AllocationHeadId = headId,
                                AllocationHead = HeadName,
                                ExpenditureAsPerUC = SpentAmountWOPreC,
                                ExpenditureAsPerBook = SpentAmount,
                                UCCommitmentUnutilized = prevAdjComm,
                                UCCommitmentUtilized = utilizedAmt,
                                UCCommitmentWithdrawn = withdrawnAmt,
                                CommitmentTreatedAsExp = 0
                            });
                        }
                    }
                }
                return List;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<BillCommitmentDetailModel> SearchUCCommitments(int projectId, int UCId)
        {
            try
            {
                List<BillCommitmentDetailModel> commitments = new List<BillCommitmentDetailModel>();
                using (var context = new IOASDBEntities())
                {

                    commitments = (from c in context.tblCommitment
                                   join cd in context.tblCommitmentDetails on c.CommitmentId equals cd.CommitmentId
                                   join hd in context.tblBudgetHead on cd.AllocationHeadId equals hd.BudgetHeadId
                                   join p in context.tblProject on c.ProjectId equals p.ProjectId
                                   join cc in context.tblCodeControl on new { codeAbb = c.CommitmentType ?? 0, codeName = "CommitmentType" } equals new { codeAbb = cc.CodeValAbbr, codeName = cc.CodeName }
                                   orderby c.CommitmentId descending
                                   where c.ProjectId == projectId
                                   && c.Status == "Active"
                                   //&& !context.tblUCCommitmentDetail.Any(m => m.CommitmentId == c.CommitmentId && m.UCId != UCId)
                                   && !(from prc in context.tblUCCommitmentDetail
                                        join preUC in context.tblUCHead on prc.UCId equals preUC.UCId
                                        where prc.UCId != UCId && preUC.TypeofUC != 2
                                        select prc.CommitmentId).Contains(c.CommitmentId)
                                   && c.CommitmentBalance > 0
                                   select new
                                   {
                                       c.CommitmentId,
                                       c.CommitmentNumber,
                                       p.ProjectNumber,
                                       c.ProjectId,
                                       c.CommitmentAmount,
                                       hd.HeadName,
                                       c.CommitmentBalance,
                                       c.PurchaseOrder,
                                       c.CRTD_TS,
                                       cc.CodeValDetail,
                                       c.VendorNameStr
                                   })
                                    .AsEnumerable()
                                              .Select((x) => new BillCommitmentDetailModel()
                                              {
                                                  CommitmentId = x.CommitmentId,
                                                  CommitmentNumber = x.CommitmentNumber,
                                                  ProjectNumber = x.ProjectNumber,
                                                  ProjectId = x.ProjectId,
                                                  BookedAmount = x.CommitmentAmount,
                                                  HeadName = x.HeadName,
                                                  AvailableAmount = x.CommitmentBalance ?? 0,
                                                  PONumber = x.PurchaseOrder,
                                                  BookedDate = String.Format("{0:dd-MMM-yyyy}", x.CRTD_TS),
                                                  TypeOfCommitment = x.CodeValDetail,
                                                  RefUCNumber = GetUCRefNo(x.CommitmentId),
                                                  VendorName = x.VendorNameStr
                                              }).ToList();

                }
                return commitments;
            }
            catch (Exception ex)
            {
                return new List<BillCommitmentDetailModel>();
            }
        }

        public string CreateTemplate(string tempName, string tempData, int userId)
        {
            try
            {
                string status = "Success";
                using (var context = new IOASDBEntities())
                {
                    var queryExists = context.tblUCTemplate.FirstOrDefault(m => m.TemplateName == tempName && m.Status != "InActive");
                    if (queryExists != null)
                        return "Template Name already exists.";
                    tblUCTemplate temp = new tblUCTemplate();
                    temp.Status = "Active";
                    temp.TemplateName = tempName;
                    temp.TemplateContent = tempData;
                    temp.CRTD_By = userId;
                    temp.CRTD_TS = DateTime.Now;
                    context.tblUCTemplate.Add(temp);
                    context.SaveChanges();
                }
                return status;
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

        public string GetTemplateDetail(int tempId)
        {
            string data = "";
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblUCTemplate.FirstOrDefault(m => m.UCTemplateId == tempId);
                    if (query != null)
                        data = query.TemplateContent;
                }
                return data;
            }
            catch (Exception ex)
            {
                return data;
            }
        }
        public string GetCreatedUC(int UCId)
        {
            string data = "";
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblUCHead.FirstOrDefault(m => m.UCId == UCId);
                    if (query != null)
                        data = query.UCRawFile;
                }
                return data;
            }
            catch (Exception ex)
            {
                return data;
            }
        }

        public UCComponentModel GetUCComponent(int ProjectId, int finYear)
        {
            try
            {
                UCComponentModel comp = new UCComponentModel();
                Dictionary<string, object> basic = new Dictionary<string, object>();
                List<YearWiseUCComponentModel> yearwiseDetail = new List<YearWiseUCComponentModel>();
                using (var context = new IOASDBEntities())
                {
                    ProjectService _ps = new ProjectService();
                    //var prjSum = _ps.getProjectSummary(ProjectId);
                    var data = Common.GetFinPeriod(finYear);
                    DateTime From = data.Item1;
                    DateTime To = data.Item2;
                    var Qry = context.tblProject.Where(m => m.ProjectId == ProjectId).FirstOrDefault();
                    decimal qryOpeningBal = context.tblProjectOB.Where(m => m.ProjectId == ProjectId).Sum(m => m.OpeningBalance) ?? 0;
                    var QryReceipt = (from C in context.tblReceipt where C.ProjectId == ProjectId && C.Posted_f==true&& C.CrtdTS >= From && C.CrtdTS <= To && C.CategoryId != 16 && C.Status == "Completed" select C).ToList();
                    decimal receiptAmt = QryReceipt.Sum(m => m.ReceiptAmount ?? 0);
                    decimal CGST = QryReceipt.Sum(m => m.CGST ?? 0);
                    decimal SGST = QryReceipt.Sum(m => m.SGST ?? 0);
                    decimal IGST = QryReceipt.Sum(m => m.IGST ?? 0);
                    decimal GST = CGST + SGST + IGST;
                    receiptAmt = (receiptAmt) - (GST);
                    var QryReceiptInt = (from C in context.tblReceipt where C.ProjectId == ProjectId && C.Posted_f == true && C.CrtdTS >= From && C.CrtdTS <= To && C.CategoryId == 16 && C.Status == "Completed" select C).ToList();
                    decimal receiptAmtInt = QryReceiptInt.Sum(m => m.ReceiptAmount ?? 0);
                    decimal CGSTInt = QryReceiptInt.Sum(m => m.CGST ?? 0);
                    decimal SGSTInt = QryReceiptInt.Sum(m => m.SGST ?? 0);
                    decimal IGSTInt = QryReceiptInt.Sum(m => m.IGST ?? 0);
                    decimal GSTInt = CGST + SGST + IGST;
                    receiptAmtInt = (receiptAmtInt) - (GSTInt);
                    decimal? spentAmt = 0;
                    var qrySpenAmt = (from C in context.vw_ProjectExpenditureReport where C.ProjectId == ProjectId && C.CommitmentDate >= From && C.CommitmentDate <= To select C.AmountSpent).Sum() ?? 0;
                    spentAmt = qrySpenAmt;
                    var OBQryReceipt = (from C in context.tblReceipt where C.ProjectId == ProjectId && C.Posted_f == true && C.CrtdTS < From && C.Status == "Completed" select C).ToList();
                    decimal OBreceiptAmt = OBQryReceipt.Sum(m => m.ReceiptAmount ?? 0);
                    decimal OBCGST = OBQryReceipt.Sum(m => m.CGST ?? 0);
                    decimal OBSGST = OBQryReceipt.Sum(m => m.SGST ?? 0);
                    decimal OBIGST = OBQryReceipt.Sum(m => m.IGST ?? 0);
                    decimal OBGST = OBCGST + OBSGST + OBIGST;
                    OBreceiptAmt = (OBreceiptAmt) - (OBGST);
                    decimal? OBspentAmt = 0;
                    var OBqrySpenAmt = (from C in context.vw_ProjectExpenditureReport where C.ProjectId == ProjectId && C.CommitmentDate < From select C.AmountSpent).Sum() ?? 0;
                    OBspentAmt = OBqrySpenAmt;
                    decimal OpeningBal = (qryOpeningBal + (OBreceiptAmt - OBspentAmt)) ?? 0;
                    decimal ttlExp = context.vw_ProjectExpenditureReport.Where(m => m.ProjectId == ProjectId && m.CommitmentDate >= From && m.CommitmentDate <= To).Sum(m => m.AmountSpent) ?? 0;
                    decimal ttlExpEqup = context.vw_ProjectExpenditureReport.Where(m => m.ProjectId == ProjectId && m.CommitmentDate >= From && m.CommitmentDate <= To && m.AllocationHeadId == 7).Sum(m => m.AmountSpent) ?? 0;
                    decimal ttlExpSalary = context.vw_ProjectExpenditureReport.Where(m => m.ProjectId == ProjectId && m.CommitmentDate >= From && m.CommitmentDate <= To && m.AllocationHeadId == 1).Sum(m => m.AmountSpent) ?? 0;
                    var BalanceCommAmt = (from C in context.tblCommitment
                                          join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                          where C.ProjectId == ProjectId && C.Status == "Active"
                                          select D.BalanceAmount).Sum() ?? 0;

                    string pType = Common.getprojectTypeName(Qry.ProjectType ?? 0);
                    if (Qry.ProjectType == 1 && Qry.ProjectSubType != 1)
                        pType += Qry.SponProjectCategory == "1" ? "-PFMS" : Qry.SponProjectCategory == "2" ? "-NON-PFMS" : "";
                    else if (Qry.ProjectType == 1 && Qry.ProjectSubType == 1)
                        pType += " - Internal";
                    int frmdate = From.Year;
                    int fintodate = To.Year;
                    string FinyearFrmDate = frmdate + "-" + fintodate;
                    string DeptPrjNo = string.Empty;
                    string ReceiptinWords = CoreAccountsService.words(receiptAmt);
                    string TotalExpamtWords = CoreAccountsService.words(ttlExp);
                    if (Qry.FinancialYear < 48)
                    {
                        if (Qry.ProjectType == 1)
                        {
                            string Prjno = Qry.ProjectNumber;
                            string dept = Prjno.Substring(0, 3);
                            string Nos = Prjno.Substring(7, 4);
                            string Noonly = Regex.Replace(Nos, "[^0-9]", "");
                            DeptPrjNo = dept + "-" + Noonly;
                        }
                        else if (Qry.ProjectType == 2)
                        {
                            string Prjno = Qry.ProjectNumber;
                            string dept = Prjno.Substring(0, 2);
                            string Nos = Prjno.Substring(9, 4);
                            string Noonly = Regex.Replace(Nos, "[^0-9]", "");
                            DeptPrjNo = dept + "-" + Noonly;
                        }
                    }
                    else if (Qry.FinancialYear >= 48)
                    {
                        if (Qry.ProjectType == 1)
                        {
                            string Prjno = Qry.ProjectNumber;
                            string dept = Prjno.Substring(0, 2);
                            string Nos = Prjno.Substring(6, 4);
                            string Noonly = Regex.Replace(Nos, "[^0-9]", "");
                            DeptPrjNo = dept + "-" + Noonly;
                        }
                        else if (Qry.ProjectType == 2)
                        {
                            string Prjno = Qry.ProjectNumber;
                            string dept = Prjno.Substring(0, 2);
                            string Nos = Prjno.Substring(8, 4);
                            string Noonly = Regex.Replace(Nos, "[^0-9]", "");
                            DeptPrjNo = dept + "-" + Noonly;
                        }
                    }
                    basic.Add("FinYearPer", FinyearFrmDate);
                    basic.Add("StartFinYear", frmdate);
                    basic.Add("EndFinYear", fintodate);
                    basic.Add("Project_Number", Qry.ProjectNumber);
                    basic.Add("Project_Dept", DeptPrjNo);
                    basic.Add("Proposal_Number", Qry.ProposalNumber);
                    basic.Add("Project_Title", Qry.ProjectTitle);
                    basic.Add("PI_Name", Common.GetPINameOnly(Qry.PIName ?? 0));
                    basic.Add("Project_Type", pType);
                    //basic.Add("Project_Duration", Qry.ProjectDuration);
                    basic.Add("Start_Date", String.Format("{0:ddd dd-MMM-yyyy}", Qry.TentativeStartDate));
                    basic.Add("Close_Date", String.Format("{0:ddd dd-MMM-yyyy}", Common.GetProjectDueDate(ProjectId) ?? Qry.TentativeCloseDate));
                    basic.Add("Base_Value", Qry.BaseValue);
                    basic.Add("Sanction_Order_No", Qry.SanctionOrderNumber);
                    if (Qry.SanctionOrderDate != null)
                        basic.Add("Sanction_Order_Date", String.Format("{0:dd}", (DateTime)Qry.SanctionOrderDate) + "-" + String.Format("{0:MM}", (DateTime)Qry.SanctionOrderDate) + "-" + String.Format("{0:yyyy}", (DateTime)Qry.SanctionOrderDate));
                    //basic.Add("Sanction_Order_Date",String.Format("{0:dd-MMM-yyyy}", Qry.SanctionOrderDate));
                    basic.Add("Sanctioned_Value", Qry.SanctionValue ?? 0);
                    basic.Add("Start_Financial_Year", Common.GetFinYear(Qry.FinancialYear ?? 0));
                    basic.Add("GST", GST);
                    //basic.Add("OverHeads", prjSum.OverHeads);
                    basic.Add("Opening_Balance", OpeningBal);
                    basic.Add("Grant_Received", receiptAmt);
                    basic.Add("Grant_Words", ReceiptinWords);
                    basic.Add("Interest", receiptAmtInt);
                    basic.Add("Net_Balance", OpeningBal + receiptAmt + receiptAmtInt);
                    basic.Add("Total_Expenditure", ttlExp);
                    basic.Add("Total_Exp_Words", TotalExpamtWords);
                    basic.Add("Total_Exp_Equp", ttlExpEqup);
                    basic.Add("Total_Exp_Salary", ttlExpSalary);
                    basic.Add("Total_Exp_ExclEqup", ttlExp - ttlExpEqup);
                    basic.Add("Total_Exp_ExclEqupAndSalary", ttlExp - ttlExpEqup - ttlExpSalary);
                    //basic.Add("Negative_Balance", prjSum.ApprovedNegativeBalance);
                    basic.Add("Available_Balance", 0);//(OpeningBal + receiptAmt) - ttlExp
                    basic.Add("Actual_Balance_Commitments", BalanceCommAmt);
                    basic.Add("Balance_Commitments", 0);
                    basic.Add("Balance_After_Commitments", 0);
                    var queryYear = context.tblYearwiseSummary.Where(m => m.ProjectId == ProjectId).ToList();
                    if (queryYear.Count > 0)
                    {
                        for (int i = 1; i <= queryYear.Count; i++)
                        {
                            Dictionary<string, object> finYearMaster = new Dictionary<string, object>();
                            List<YearWiseDetailUCComponentModel> finHeadDet = new List<YearWiseDetailUCComponentModel>();
                            var year = Common.GetFinancialYear(queryYear[i].FinYear ?? 0);
                            int sumId = queryYear[i].YearwiseSummaryId;
                            finYearMaster.Add("finYearwise_" + i + "_FinYear", year);
                            finYearMaster.Add("finYearwise_" + i + "_ExpAmount", queryYear[i].ExpAmount);
                            finYearMaster.Add("finYearwise_" + i + "_UCExpAmount", queryYear[i].UCExpAmount);
                            finYearMaster.Add("finYearwise_" + i + "_OpeningBal", queryYear[i].OpeningBal);
                            finYearMaster.Add("finYearwise_" + i + "_UCOpeningBal", queryYear[i].UCOpeningBal);
                            finYearMaster.Add("finYearwise_" + i + "_GrantReceived", queryYear[i].GrantReceived);
                            finYearMaster.Add("finYearwise_" + i + "_ClosingBal", queryYear[i].ClosingBal);
                            finYearMaster.Add("finYearwise_" + i + "_UCClosingBal", queryYear[i].UCClosingBal);
                            var queryDetail = context.tblYearwiseSummaryDetail.Where(m => m.YearwiseSummaryId == sumId).ToList();
                            if (queryDetail.Count > 0)
                            {
                                for (int j = 1; j <= queryDetail.Count; j++)
                                {
                                    YearWiseDetailUCComponentModel headModel = new YearWiseDetailUCComponentModel();
                                    Dictionary<string, object> heads = new Dictionary<string, object>();
                                    heads.Add("headDetail_" + year + "_" + j + "_HeadName", queryDetail[j].HeadName);
                                    heads.Add("headDetail_" + year + "_" + j + "_ExpAmount", queryDetail[j].ExpAmount);
                                    heads.Add("headDetail_" + year + "_" + j + "_UCExpAmount", queryDetail[j].UCExpAmount);
                                    headModel.heads = heads;
                                    finHeadDet.Add(headModel);
                                }
                            }

                            yearwiseDetail.Add(new YearWiseUCComponentModel()
                            {
                                yearDetail = finHeadDet,
                                yearMaster = finYearMaster
                            });
                        }
                    }
                }
                comp.basic = basic;
                comp.finYearwise = yearwiseDetail;
                return comp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string GetUCRefNo(int cId)
        {
            string refNo = string.Empty;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from prc in context.tblUCCommitmentDetail
                                 join preUC in context.tblUCHead on prc.UCId equals preUC.UCId
                                 orderby preUC.UCId descending
                                 where prc.CommitmentId == cId && preUC.TypeofUC == 1
                                 select preUC).FirstOrDefault();
                    if (query != null)
                        refNo = query.UCNumber;
                }
                return refNo;
            }
            catch (Exception ex)
            {
                return refNo;
            }
        }
        public bool CheckIsEditable(int UCId, int? projectId)
        {
            bool editable = true;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblUCHead.FirstOrDefault(m => m.UCId > UCId && m.ProjectId == projectId && m.Status != "InAvtive" && m.TypeofUC == 2);
                    if (query != null)
                        editable = false;
                }
                return editable;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string UpdateTemplate(int tempId, string tempData, int userId)
        {
            try
            {
                string status = "Success";
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblUCTemplate.Where(m => m.UCTemplateId == tempId).FirstOrDefault();
                    if (query != null)
                    {
                        query.TemplateContent = tempData;
                        query.CRTD_By = userId;
                        query.CRTD_TS = DateTime.Now;
                        context.SaveChanges();
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

    }
}