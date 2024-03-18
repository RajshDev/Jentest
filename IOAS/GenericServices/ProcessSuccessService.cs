using ICSREMP.DataModel;
using IOAS.DataModel;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IOAS.GenericServices
{
    public class ProcessSuccessService
    {
        private static readonly Object lockObj = new Object();
        private static readonly Object TADlockObj = new Object();
        private static readonly Object TSTlockObj = new Object();
        private static readonly Object DTVlockObj = new Object();
        private static readonly Object BilllockObj = new Object();
        private static readonly Object SalarylockObj = new Object();
        private static readonly Object ProjectlockObj = new Object();
        private static readonly Object InternalProjectlockObj = new Object();
        private static readonly Object ProjectEnhancementProjectlockObj = new Object();
        private static readonly Object ProposallockObj = new Object();
        private static readonly Object GeneralVoucherlockObj = new Object();
        private static readonly Object PFTlockObj = new Object();
        private static readonly Object HonororiumlockObj = new Object();
        private static readonly Object ProjectTransferlockObj = new Object();
        private static readonly Object FRMlockObj = new Object();
        private static readonly Object ClearancePaymentlockObj = new Object();
        private static readonly Object OtherReceiptlockObj = new Object();
        private static readonly Object AdhocPaymentlockObj = new Object();
        private static readonly Object VendorlockObj = new Object();
        private static readonly Object NegativeBalancelockObj = new Object();
        private static readonly Object ContralockObj = new Object();
        private static readonly Object DistributionlockObj = new Object();
        private static readonly Object ECDlockObj = new Object();
        private static readonly Object ECRlockObj = new Object();
        private static readonly Object ECBlockObj = new Object();
        private static readonly Object SMIlockObj = new Object();
        private static readonly Object IMPlockObj = new Object();
        private static readonly Object IMPElockObj = new Object();
        private static readonly Object IMRlockObj = new Object();
        private static readonly Object IBRlockObj = new Object();
        private static readonly Object PTPlockObj = new Object();
        private static readonly Object TMPlockObj = new Object();
        private static readonly Object LCOlockObj = new Object();
        private static readonly Object LCAlockObj = new Object();
        private static readonly Object LCRlockObj = new Object();
        private static readonly Object HCRlockObj = new Object();
        private static readonly Object PaymentProcesslockObj = new Object();
        private static readonly Object FSSlockObj = new Object();
        private static readonly Object MDYlockObj = new Object();
        private static readonly Object AVOlockObj = new Object();
        private static readonly Object OHARlockObj = new Object();
        private static readonly Object TMSlockObj = new Object();
        private static readonly Object JVlockObj = new Object();
        private static readonly Object TXPlockObj = new Object();
        private static readonly Object IMElockObj = new Object();

        CoreAccountsService coreAccountService = new CoreAccountsService();
        public bool TADWFInitSuccess(int travelBillId, int logged_in_user)
        {
            try
            {
                lock (TADlockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == travelBillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "TAD");
                        if (query != null)
                        {
                            query.Status = "Completed";// "Pending Commitment";
                            query.UPTD_By = logged_in_user;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool TSTWFInitSuccess(int travelBillId, int logged_in_user)
        {
            try
            {
                lock (TSTlockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == travelBillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "TST");
                        if (query != null)
                        {
                            if (query.BalanceinAdvance == 0 && query.PaymentValue == 0)
                            {
                                if (!coreAccountService.BalancedTSTBOATransaction(travelBillId))
                                {
                                    //   coreAccountService.RollBackLastApproved(travelBillId, 43);
                                    return false;
                                }
                            }
                            else if (query.BalanceinAdvance > 0)
                            {
                                if (!coreAccountService.ReversedTSTBOATransaction(travelBillId))
                                {
                                    //   coreAccountService.RollBackLastApproved(travelBillId, 43);
                                    return false;
                                }
                            }
                            query.Status = "Completed";// "Pending Commitment";
                            query.UPTD_By = logged_in_user;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        {
                            // coreAccountService.RollBackLastApproved(travelBillId, 43);
                            return false;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                // coreAccountService.RollBackLastApproved(travelBillId, 43);
                return false;
            }
        }
public bool DTVWFInitSuccess(int travelBillId, int logged_in_user)
        {
            try
            {
                lock (DTVlockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == travelBillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "DTV");
                        if (query != null)
                        {
                            query.Status = "Completed";// "Pending Commitment";
                            query.UPTD_By = logged_in_user;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool BillWFInitSuccess(int billId, int loggedInUser)
        {
            try
            {
                lock (BilllockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblBillEntry.FirstOrDefault(m => m.BillId == billId && m.Status == "Submit for approval");
                        var funId = query.TransactionTypeCode == "STM" ? 31 : 30;
                        if (query != null)
                        {
                            if (query.TransactionTypeCode != "ADV")
                            {

                                if (!coreAccountService.BillBackEndEntry(billId, loggedInUser))
                                {
                                    //   coreAccountService.RollBackLastApproved(billId, funId);
                                    return false;
                                }
                                else
                                if (!coreAccountService.BillBOATransaction(billId))
                                {
                                    context.tblBillPaymentTransactionDetail.RemoveRange(context.tblBillPaymentTransactionDetail.Where(m => m.BillId == billId).ToList());
                                    context.SaveChanges();
                                    //   coreAccountService.RollBackLastApproved(billId, funId);
                                    return false;
                                }
                            }
                            query.Status = "Completed";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                //  coreAccountService.RollBackLastApproved(billId, funId);
                return false;
            }
        }
        public bool SalaryInitSuccess(int PaymentHeadId, int userId)
        {
            try
            {
                lock (SalarylockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblSalaryPaymentHead.FirstOrDefault(m => m.PaymentHeadId == PaymentHeadId && m.Status == "Approval Pending");
                        if (query != null)
                        {
                            query.Status = "Completed";
                            query.UpdatedBy = userId;
                            query.UpdatedAt = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool ProjectWFInitSuccess(int projectId, int loggedInUser)
        {
            try
            {
                lock (ProjectlockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblProject.FirstOrDefault(m => m.ProjectId == projectId && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            tblProjectStatusLog status = new tblProjectStatusLog();
                            status.FromStatus = "Submit for approval";
                            status.ToStatus = "Active";
                            status.ProjectId = projectId;
                            status.UpdtdUserId = loggedInUser;
                            status.UpdtdTS = DateTime.Now;
                            context.tblProjectStatusLog.Add(status);
                            context.SaveChanges();

                            query.Status = "Active";
                            query.UpdatedUserId = loggedInUser;
                            query.UpdatedTS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool InternalProjectWFInitSuccess(int projectId, int loggedInUser)
        {
            try
            {
                lock (InternalProjectlockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblProject.FirstOrDefault(m => m.ProjectId == projectId && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            tblProjectStatusLog status = new tblProjectStatusLog();
                            status.FromStatus = "Submit for approval";
                            status.ToStatus = "Active";
                            status.ProjectId = projectId;
                            status.UpdtdUserId = loggedInUser;
                            status.UpdtdTS = DateTime.Now;
                            context.tblProjectStatusLog.Add(status);
                            context.SaveChanges();

                            query.Status = "Active";
                            query.UpdatedUserId = loggedInUser;
                            query.UpdatedTS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool ProjectEnhancementWFInitSuccess(int projectEnhancementId, int loggedInUser)
        {
            try
            {
                lock (ProjectEnhancementProjectlockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblProjectEnhancement.FirstOrDefault(m => m.ProjectEnhancementId == projectEnhancementId && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            query.Status = "Active";
                            query.LastUpdtUserId = loggedInUser;
                            query.LastUpdtTS = DateTime.Now;
                            context.SaveChanges();
                            if (query.IsEnhancementonly == true)
                                Common.UpdateSanctionValue(query.ProjectId ?? 0);
                            if (query.IsExtensiononly == true)
                                Common.GetProjectDueDate(query.ProjectId ?? 0, true);
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool ProjectROWFInitSuccess(int ROApprovalId, int loggedInUser)
        {
            try
            {
                lock (ProjectEnhancementProjectlockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblProjectROSummary.FirstOrDefault(m => m.RO_ProjectApprovalId == ROApprovalId && m.RO_Status == "Submit for approval");
                        var queryApprovalId = context.tblProjectROApprovalRequest.Where(m => m.RO_ProjectApprovalId == ROApprovalId).FirstOrDefault();
                        if (query != null)
                        {
                            context.tblProjectROSummary.Where(m => m.RO_ProjectApprovalId == ROApprovalId && m.RO_Status == "Submit for approval")
                                .ToList()
                                .ForEach(m =>
                                {
                                    m.RO_Status = "Active";
                                    m.Uptd_UserId = loggedInUser;
                                    m.Uptd_TS = DateTime.Now;
                                });
                            context.tblProjectROLog.Where(p => p.RO_ProjectApprovalId == ROApprovalId && p.RO_LogStatus == "Submit for approval")
                                  .ToList()
                                  .ForEach(m =>
                                  {
                                      
                                      m.RO_LogStatus = "Active";
                                      m.Uptd_UserId = loggedInUser;
                                      m.Uptd_TS = DateTime.Now;
                                  });

                            if (queryApprovalId != null)
                            {
                                queryApprovalId.Uptd_TS = DateTime.Now;
                                queryApprovalId.Uptd_UserId = loggedInUser;
                            }
                            
                                
                            context.SaveChanges();
                            if (query.RO_Status == "Active")
                                Common.UpdateROSummaryLog(query.ProjectId, ROApprovalId);
                            
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool ProposalWFInitSuccess(int proposalId, int loggedInUser)
        {
            try
            {
                lock (ProposallockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblProposal.FirstOrDefault(m => m.ProposalId == proposalId && m.Status == "Submit for approval");
                        if (query != null)
                        {

                            query.Status = "Active";
                            query.Updt_Userid = loggedInUser;
                            query.Updt_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool GeneralVoucherWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (GeneralVoucherlockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblGeneralVoucher.FirstOrDefault(m => m.GeneralVoucherId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            if (query.Payee_f != true && !coreAccountService.GeneralVoucherBOATransaction(id))
                            {
                                //  coreAccountService.RollBackLastApproved(id, 70);
                                return false;
                            }
                            query.Status = "Completed";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        {
                            //   coreAccountService.RollBackLastApproved(id, 70);
                            return false;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                //  coreAccountService.RollBackLastApproved(id, 70);
                return false;
            }
        }
        public bool PFTWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (PFTlockObj)
                {
                    return coreAccountService.ApproveProjectFundTransfer(id, loggedInUser);

                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool HonororiumWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (HonororiumlockObj)
                {
                    return coreAccountService.HonororiumBillApproved(id, loggedInUser);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                  //coreAccountService.RollBackLastApproved(id, 63);
                return false;
            }
        }
        public bool ProjectTransferWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (ProjectTransferlockObj)
                {
                    return coreAccountService.ApproveProjectTransfer(id, loggedInUser);


                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool FRMWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (FRMlockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblForeignRemittance.FirstOrDefault(m => m.ForeignRemitId == id && m.Status == "Submit for approval" && m.TransactionTypeCode == "FRM");
                        if (query != null)
                        {
                            if (!coreAccountService.getForeignRemittanceBOAmodeldetails(id))
                            {
                                // coreAccountService.RollBackLastApproved(id, 76);
                                return false;
                            }
                            else
                            {
                                if (query.TypeofPayment == 1 && query.PurposeofRemittance == 1)
                                {
                                    query.Status = "Pending Bill of Entry";
                                    query.UPTD_By = loggedInUser;
                                    query.UPTD_TS = DateTime.Now;
                                    context.SaveChanges();
                                    coreAccountService.ForeignRemitEmailSend(id);
                                }
                                else
                                {
                                    query.Status = "Approved";
                                    query.UPTD_By = loggedInUser;
                                    query.UPTD_TS = DateTime.Now;
                                    context.SaveChanges();
                                    coreAccountService.ForeignRemitEmailSend(id);
                                }
                                return true;
                            }

                        }

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool ClearancePaymentWFInitSuccess(int clearancepaymentid, int logged_in_user)
        {
            try
            {
                lock (ClearancePaymentlockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblClearancePaymentEntry.FirstOrDefault(m => m.ClearancePaymentId == clearancepaymentid && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            if (!coreAccountService.getCLPBOAmodeldetails(clearancepaymentid))
                            {
                                //   coreAccountService.RollBackLastApproved(clearancepaymentid, 45);
                                return false;
                            }
                            else if (!coreAccountService.CLPBackEndEntry(clearancepaymentid, logged_in_user))
                            {
                                context.tblClearancePaymentTransactionDetail.RemoveRange(context.tblClearancePaymentTransactionDetail.Where(m => m.ClearancePaymentId == clearancepaymentid).ToList());
                                context.SaveChanges();
                                //  coreAccountService.RollBackLastApproved(clearancepaymentid, 45);
                                return false;
                            }
                            query.Status = "Active";
                            query.UPTD_By = logged_in_user;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool OtherReceiptInitSuccess(int id, int loggedInUser)
        {
            try
            {
                // lock (lockObj)
                // {
                using (var context = new IOASDBEntities())
                {
                    var billQuery = context.tblReceipt.FirstOrDefault(m => m.ReceiptId == id && m.Status == "Submit for approval");
                    if (billQuery != null)
                    {

                        int pId = billQuery.ProjectId ?? 0;
                        decimal ttlAmt = 0;
                        if (billQuery.CategoryId == 18)
                        {
                            decimal recAmt = billQuery.ReceiptAmount ?? 0;
                            decimal cgst = billQuery.CGST ?? 0;
                            decimal sgst = billQuery.SGST ?? 0;
                            decimal igst = billQuery.IGST ?? 0;
                            ttlAmt = recAmt - cgst - sgst - igst;
                            ttlAmt = -ttlAmt;
                            string msg = Common.ValidateNegReceipt(billQuery.InvoiceId.GetValueOrDefault(0), billQuery.NegativeReceiptId.GetValueOrDefault(0), billQuery.ReceiptId, billQuery.ProjectId.GetValueOrDefault(0));
                            if (msg != "Valid")
                            {
                                //  coreAccountService.RollBackLastApproved(id, 73);
                                return false;
                            }

                        }
                        else if (billQuery.CategoryId == 15)
                        {
                            decimal bankAmt = billQuery.BankAmountDr ?? 0;
                            decimal crAmt = context.tblReceiptRecivables.Where(m => m.TransactionType == "Credit" && m.ReceiptId == id && m.Tax_f != true).Select(m => m.ReceivabesAmount).Sum() ?? 0;
                            //decimal txAmt = context.tblReceiptRecivables.Where(m => m.ReceiptId == id && m.Tax_f == true).Select(m => m.ReceivabesAmount).Sum() ?? 0;
                            ttlAmt = Math.Round(bankAmt + crAmt, 2, MidpointRounding.AwayFromZero);
                            ProjectService _PS = new ProjectService();
                            var projectData = _PS.getProjectSummary(Convert.ToInt32(billQuery.ProjectId));
                            if (ttlAmt > projectData.NetBalance)
                            {
                                //  coreAccountService.RollBackLastApproved(id, 73);
                                return false;
                            }
                            ttlAmt = -ttlAmt;
                        }
                        else
                        {
                            ttlAmt = context.tblReceiptRecivables.Where(m => m.TransactionType == "Debit" && m.ReceiptId == id && m.Tax_f == false).Sum(m => m.ReceivabesAmount) ?? 0;
                            ttlAmt = ttlAmt + (billQuery.BankAmountDr ?? 0);
                        }

                        if (pId > 0 && billQuery.CategoryId != 18 && billQuery.CategoryId != 16 && !Common.ValidateProjectBalanceOnReceipt(pId, id, ttlAmt))
                        {
                            //   coreAccountService.RollBackLastApproved(id, 73);
                            return false;
                        }
                        if (coreAccountService.OtherReceiptBOATransaction(id))
                        {

                            if (billQuery.InvoiceId > 0)
                            {
                                int negRecId = billQuery.NegativeReceiptId.GetValueOrDefault(0);
                                if (negRecId > 0 && !coreAccountService.OverHeadsReversalBackEndPosting(negRecId))
                                {
                                    Common.InActiveBOA(billQuery.ReceiptNumber);
                                    //  coreAccountService.RollBackLastApproved(id, 73);
                                    return false;
                                }
                                var invquery = context.tblProjectInvoice.FirstOrDefault(m => m.InvoiceId == billQuery.InvoiceId);
                                if (invquery.Status == "Completed")
                                {
                                    invquery.Status = "Active";
                                    context.SaveChanges();
                                }
                            }
                            if (billQuery.ReceiptAmount > 0)
                            {
                                var Vquery = context.tblReceipt.SingleOrDefault(m => m.ReceiptId == id && m.CategoryId != 16);
                                if (Vquery != null)
                                {
                                    int vquerypjctid = Vquery.ProjectId ?? 0;
                                    var negativebalancequery = (from v in context.tblNegativeBalance
                                                                where v.ProjectId == vquerypjctid && v.Status == "Approved"
                                                                select v).ToList();

                                    if (negativebalancequery.Count() > 0)
                                    {
                                        decimal? claimamt = 0;
                                        decimal? balancewhenclosing = 0;
                                        decimal? negativebalamt = 0;
                                        decimal? adjustamount = 0;
                                        decimal? overheadsamt = Vquery.ReceiptOverheadValue ?? 0;
                                        decimal? gstamt = 0;
                                        decimal? newadjustamt = 0;

                                        if (Vquery.IGST > 0 && Vquery.CGST == 0)
                                        {
                                            gstamt = Vquery.IGST;
                                        }
                                        else if (Vquery.CGST > 0 && Vquery.IGST == 0)
                                        {
                                            gstamt = Vquery.CGST + Vquery.SGST;
                                        }
                                        decimal? receiptamt = Vquery.ReceiptAmount - gstamt;
                                        decimal? balinreceipt = receiptamt;

                                        for (int i = 0; i < negativebalancequery.Count; i++)
                                        {
                                            claimamt = negativebalancequery[i].ClaimAmount;
                                            balancewhenclosing = negativebalancequery[i].BalanceWhenClosing ?? 0;
                                            negativebalamt = negativebalancequery[i].NegativeBalanceAmount ?? 0;
                                            adjustamount = negativebalancequery[i].ReceiptAdjustmentAmount ?? 0;
                                            decimal? newnegativebalamt = 0;
                                            if (balinreceipt > 0)
                                            {
                                                if (balinreceipt >= negativebalamt)
                                                {
                                                    newadjustamt = adjustamount + negativebalamt;
                                                    negativebalancequery[i].ReceiptAdjustmentAmount = newadjustamt;
                                                    negativebalancequery[i].NegativeBalanceAmount = 0;
                                                    negativebalancequery[i].Status = "Closed";
                                                    negativebalancequery[i].BalanceWhenClosing = 0;
                                                    negativebalancequery[i].ClosedDate = DateTime.Now;
                                                    negativebalancequery[i].UPTD_By = loggedInUser;
                                                    negativebalancequery[i].UPTD_TS = DateTime.Now;
                                                    negativebalancequery[i].ReasonForClose = "Receipt Created for full negative balance amount.";
                                                    balinreceipt = balinreceipt - negativebalamt;
                                                    context.SaveChanges();
                                                }
                                                else
                                                {
                                                    newadjustamt = adjustamount + balinreceipt;
                                                    newnegativebalamt = claimamt - (newadjustamt + balancewhenclosing);
                                                    negativebalancequery[i].ReceiptAdjustmentAmount = newadjustamt;
                                                    negativebalancequery[i].NegativeBalanceAmount = newnegativebalamt;
                                                    negativebalancequery[i].UPTD_By = loggedInUser;
                                                    negativebalancequery[i].UPTD_TS = DateTime.Now;
                                                    balinreceipt = 0;
                                                    context.SaveChanges();
                                                }
                                                tblNegativeApprovalOffsetLog log = new tblNegativeApprovalOffsetLog();
                                                log.NegativeBalanceId = negativebalancequery[i].NegativeBalanceId;
                                                log.NegativeBalanceNumber = negativebalancequery[i].NegativeBalanceNumber;
                                                log.RefId = Vquery.ReceiptId;
                                                log.RefNumber = Vquery.ReceiptNumber;
                                                log.FunctionName = "Receipt";
                                                log.Remarks = "Negative Approval offset on Receipt Approval with Amount - " + receiptamt + "(" + Vquery.ReceiptAmount + " - " + "(" + overheadsamt + " + " + gstamt + "))";
                                                log.AmountModified = receiptamt;
                                                log.CRTD_BY = loggedInUser;
                                                log.CRTD_TS = DateTime.Now;
                                                context.tblNegativeApprovalOffsetLog.Add(log);
                                                context.SaveChanges();
                                            }
                                            else if (balinreceipt == 0)
                                            {
                                                newadjustamt = 0;
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }

                            }
                            billQuery.Status = "Completed";
                            billQuery.UpdtUserId = loggedInUser;
                            billQuery.UpdtTS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        else
                        {
                            //  coreAccountService.RollBackLastApproved(id, 73);
                            return false;
                        }
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }

        public bool AdhocPaymentWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (AdhocPaymentlockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblAdhocPayment.FirstOrDefault(m => m.AdhocPaymentId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            query.Status = "Completed";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        else
                        {
                            // coreAccountService.RollBackLastApproved(id, 59);
                            return false;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                // coreAccountService.RollBackLastApproved(id, 59);
                return false;
            }
        }
        public bool VendorWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (VendorlockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblVendorMaster.FirstOrDefault(m => m.VendorId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            query.Status = "Active";
                            query.UPDT_UserID = loggedInUser;
                            query.UPDT_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        else
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool NegativeBalanceWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (NegativeBalancelockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblNegativeBalance.FirstOrDefault(m => m.NegativeBalanceId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            if (!Common.ValidateNegativeBalance(query.ProjectId.GetValueOrDefault(0), query.ClaimAmount.GetValueOrDefault(0), id)) // (sanctionValue < overAllAmt)
                            {
                                //   coreAccountService.RollBackLastApproved(id, 66);
                                return false;
                            }
                            query.Status = "Approved";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            coreAccountService.NegativeEmailSend(query.PIId ?? 0, query.ProjectId ?? 0, query.NegativeBalanceAmount ?? 0);
                            return true;
                        }
                        else
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool ContraWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (ContralockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var billQuery = context.tblContra.SingleOrDefault(m => m.ContraId == id && m.Status == "Submit for approval");
                        if (billQuery != null && coreAccountService.ContraBOATransaction(id))
                        {

                            billQuery.Status = "Completed";
                            billQuery.UPTD_By = loggedInUser;
                            billQuery.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        else
                        {
                            // coreAccountService.RollBackLastApproved(id, 64);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool DistributionWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (DistributionlockObj)
                {
                    return coreAccountService.DistributionBillApproved(id, loggedInUser);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool ECDWFInitSuccess(int BillId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblSBIPrepaidCardProjectDetails.FirstOrDefault(m => m.SBIECardProjectDetailsId == BillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "ECD");
                    if (query != null)
                    {
                        if (!coreAccountService.getSBICardBOAmodeldetails(BillId))
                        {
                            //   coreAccountService.RollBackLastApproved(BillId, 39);
                            return false;
                        }
                        query.Status = "Approved";
                        query.UpdtUserId = logged_in_user;
                        query.UpdtTS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool ECRWFInitSuccess(int BillId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblSBICardRecoupment.FirstOrDefault(m => m.RecoupmentId == BillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "ECR");
                    if (query != null)
                    {
                        if (!coreAccountService.getSBICardRecoupmentBOAmodeldetails(BillId))
                        {
                            //   coreAccountService.RollBackLastApproved(BillId, 46);
                            return false;
                        }
                        query.Status = "Approved";
                        query.UPTD_By = logged_in_user;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool ECBRWFInitSuccess(int BillId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblSBICardBillRecoupment.FirstOrDefault(m => m.SBICardBillRecoupId == BillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "ECBR");
                    if (query != null)
                    {
                        if (!coreAccountService.SBICardBillRecoupBOATransaction(BillId))
                        {
                            //    coreAccountService.RollBackLastApproved(BillId, 126);
                            return false;
                        }
                        query.Status = "Approved";
                        query.UPTD_By = logged_in_user;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool SMIWFInitSuccess(int BillId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblSummerInternshipStudentDetails.FirstOrDefault(m => m.SummerInternshipStudentId == BillId && m.Status == "Submit for Approval" && m.TransactionTypeCode == "SMI");
                    if (query != null)
                    {
                        query.Status = "Completed";
                        query.UPTD_By = logged_in_user;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool IMPWFInitSuccess(int BillId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblIMPUserDetails.FirstOrDefault(m => m.IMPUserDetailsId == BillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "IMP");
                    var pjcts = context.tblImprestPaymentDetails.FirstOrDefault(m => m.IMPUserDetailsId == BillId && m.Status == "Open");
                    if (query != null)
                    {
                        if (!coreAccountService.getImprestBOAmodeldetails(BillId))
                        {
                            //  coreAccountService.RollBackLastApproved(BillId, 50);
                            return false;
                        }
                        query.Status = "Approved";
                        query.UpdtUserId = logged_in_user;
                        query.UpdtTS = DateTime.Now;
                        context.SaveChanges();

                        pjcts.Status = "Approved";
                        pjcts.UpdtUserId = logged_in_user;
                        pjcts.UpdtTS = DateTime.Now;
                        context.SaveChanges();

                        return true;
                    }
                    else
                    {
                        //  coreAccountService.RollBackLastApproved(BillId, 50);
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool IMPEWFInitSuccess(int BillId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblImprestPaymentDetails.FirstOrDefault(m => m.ImprestPaymentDetailsId == BillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "IME");

                    if (query != null)
                    {
                        var usrdetlsid = query.IMPUserDetailsId;
                        var usrdetls = context.tblIMPUserDetails.FirstOrDefault(m => m.IMPUserDetailsId == usrdetlsid);
                        if (!coreAccountService.getImprestEnhanceBOAmodeldetails(BillId))
                        {
                            // coreAccountService.RollBackLastApproved(BillId, 72);
                            return false;
                        }
                        query.Status = "Approved";
                        query.UpdtUserId = logged_in_user;
                        query.UpdtTS = DateTime.Now;
                        context.SaveChanges();
                        usrdetls.ImprestTotalValue = usrdetls.ImprestTotalValue + query.AmountAllocated;
                        usrdetls.NoofImprests = usrdetls.NoofImprests + 1;
                        usrdetls.UpdtUserId = logged_in_user;
                        usrdetls.UpdtTS = DateTime.Now;
                        context.SaveChanges();

                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool IMRWFInitSuccess(int BillId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblImprestRecoupment.FirstOrDefault(m => m.RecoupmentId == BillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "IMR");
                    if (query != null)
                    {
                        if (!coreAccountService.getImprestRecoupmentBOAmodeldetails(BillId))
                        {
                            // coreAccountService.RollBackLastApproved(BillId, 49);
                            return false;
                        }
                        query.Status = "Approved";
                        query.UPTD_By = logged_in_user;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool IBRWFInitSuccess(int BillId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblImprestBillRecoupment.FirstOrDefault(m => m.ImprestBillRecoupId == BillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "IBR");
                    if (query != null)
                    {
                        if (!coreAccountService.ImprestBillRecoupBOATransaction(BillId))
                        {
                            //  coreAccountService.RollBackLastApproved(BillId, 67);
                            return false;
                        }
                        query.Status = "Completed";
                        query.UPTD_By = logged_in_user;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool PTPWFInitSuccess(int BillId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblPartTimePayment.FirstOrDefault(m => m.PartTimePaymentId == BillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "PTP");
                    if (query != null)
                    {
                        query.Status = "Completed";
                        query.UPTD_By = logged_in_user;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool TMPWFInitSuccess(int BillId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTemporaryAdvance.FirstOrDefault(m => m.TemporaryAdvanceId == BillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "TMP");
                    if (query != null)
                    {
                        query.Status = "Active";
                        query.UPTD_By = logged_in_user;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool LCOWFInitSuccess(int LCDraftid, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblLCDraftDetails.FirstOrDefault(m => m.Id == LCDraftid && m.Status == "Establish LC Approval Pending" && m.TransactionTypeCode == "LCO");
                    if (query != null)
                    {
                        if (!coreAccountService.getLCOpeningBOAmodeldetails(LCDraftid))
                            return false;
                        query.Status = "Established";
                        query.UPTD_By = logged_in_user;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool LCAWFInitSuccess(int LCAmmendmentid, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblLCAmmendment.FirstOrDefault(m => m.Id == LCAmmendmentid && m.Status == "Amendment Approval Pending" && m.TransactionTypeCode == "LCA");
                    if (query != null)
                    {
                        if (!coreAccountService.getLCAmmendmentBOAmodeldetails(LCAmmendmentid))
                            return false;
                        var lcopeningid = query.LCOpeningId ?? 0;
                        var lcquery = context.tblLCDraftDetails.FirstOrDefault(m => m.Id == lcopeningid);
                        query.Status = "LC Amended";
                        query.UPTD_By = logged_in_user;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        lcquery.Status = "LC Amended";
                        context.SaveChanges();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool LCRWFInitSuccess(int LCRetirmentid, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblLCRetirement.FirstOrDefault(m => m.Id == LCRetirmentid && m.Status == "Retirement Approval Pending" && m.TransactionTypeCode == "LCR");
                    if (query != null)
                    {
                        if (!coreAccountService.getLCRetirementBOAmodeldetails(LCRetirmentid))
                            return false;
                        var lcopeningid = query.LCOpeningId ?? 0;
                        var lcquery = context.tblLCDraftDetails.FirstOrDefault(m => m.Id == lcopeningid);
                        var lcretire = (from r in context.tblLCRetirement
                                        where r.LCOpeningId == lcopeningid && r.Status != "InActive"
                                        orderby r.Id descending
                                        select r).ToList();
                        decimal lcdrafRemittance= lcretire.Sum(m => m.LCValueInForeignCurrency) ?? 0;
                        query.Status = "LC Retired";
                        query.UPTD_By = logged_in_user;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        if (lcdrafRemittance == lcquery.RemittanceAmount)
                            lcquery.Status = "LC Retired";
                        else
                            lcquery.Status = "Retirement Open";
                        context.SaveChanges();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool HCRWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (HCRlockObj)
                {
                    return coreAccountService.ApproveHeadCredit(id, loggedInUser);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool PaymentProcessWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                //lock (PaymentProcesslockObj)
                //{
                using (var context = new IOASDBEntities())
                {

                    var query = context.tblBOADraft.FirstOrDefault(m => m.BOADraftId == id && m.Status == "Submit for approval");
                    if (query != null)
                    {

                        bool valid = coreAccountService.ValidatePaymentProcess(id);
                        if (valid)
                        {
                            if (!coreAccountService.PaymentBOATransaction(id, loggedInUser))
                            {
                                //  coreAccountService.RollBackLastApproved(id, 51);
                                return false;
                            }
                            else
                                return true;
                        }
                        else
                        {
                            // coreAccountService.RollBackLastApproved(id, 51);
                            return false;
                        }
                    }
                    else
                    {
                        // coreAccountService.RollBackLastApproved(id, 51);
                        return false;
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                // coreAccountService.RollBackLastApproved(id, 51);
                return false;
            }
        }
        public bool FSSWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (FSSlockObj)
                {
                    return coreAccountService.FellowShipSalaryBillApproved(id, loggedInUser);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool MDYWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (MDYlockObj)
                {
                    if (coreAccountService.ManDayBillApproved(id, loggedInUser))
                        return true;
                    else
                    {
                        // coreAccountService.RollBackLastApproved(id, 77);
                        return false;

                    }

                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool AVOWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (AVOlockObj)
                {
                    if (coreAccountService.ApproveAdminVoucher(id, loggedInUser))
                        return true;
                    else
                    {
                        //  coreAccountService.RollBackLastApproved(id, 151);
                        return false;

                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return false;
            }
        }
        public bool OHARWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (OHARlockObj)
                {
                    if (!coreAccountService.AdditionandReversalOHBillApproved(id, loggedInUser))
                    {
                        // coreAccountService.RollBackLastApproved(id, 106);
                        return false;
                    }
                    else
                        return true;

                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                //  coreAccountService.RollBackLastApproved(id, 106);
                return false;
            }
        }
        public bool TMSWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (TMSlockObj)
                {
                    if (!coreAccountService.TempAdvSettlementApproved(id, loggedInUser))
                    {
                        //  coreAccountService.RollBackLastApproved(id, 47);
                        return false;
                    }
                    else
                        return true;

                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                //  coreAccountService.RollBackLastApproved(id, 47);
                return false;
            }
        }
        public bool JVWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (JVlockObj)
                {
                    if (coreAccountService.ApproveJournal(id, loggedInUser))
                        return true;
                    else
                    {
                        //   coreAccountService.RollBackLastApproved(id, 55);
                        return false;

                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                //   coreAccountService.RollBackLastApproved(id, 55);
                return false;
            }
        }
        public bool TXPWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (TXPlockObj)
                {
                    if (coreAccountService.TDSPaymentBOATransaction(id))
                    {
                        Common.ApprovalPendingForTDSPayment(id, loggedInUser);
                        return true;
                    }
                    else
                    {
                        //     coreAccountService.RollBackLastApproved(id, 75);
                        return false;

                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                //    coreAccountService.RollBackLastApproved(id, 75);
                return false;
            }
        }
        public bool IMEWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (IMElockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblImprestPaymentDetails.FirstOrDefault(m => m.ImprestPaymentDetailsId == id && m.Status == "Submit for approval");

                        var usr = context.tblIMPUserDetails.FirstOrDefault(m => m.IMPUserDetailsId == query.IMPUserDetailsId);

                        if (query != null)
                        {
                            if (!coreAccountService.getImprestEnhanceBOAmodeldetails(id))
                            {
                                //  coreAccountService.RollBackLastApproved(id, 72);
                                return false;
                            }

                            query.Status = "Approved";
                            query.UpdtUserId = loggedInUser;
                            query.UpdtTS = DateTime.Now;
                            context.SaveChanges();
                            usr.ImprestTotalValue = usr.ImprestTotalValue + query.AmountAllocated;
                            usr.NoofImprests = usr.NoofImprests + 1;
                            usr.UpdtUserId = loggedInUser;
                            usr.UpdtTS = DateTime.Now;
                            context.SaveChanges();
                            return true;

                        }
                        else
                        {
                            //   coreAccountService.RollBackLastApproved(id, 72);
                            return false;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
         (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                //  coreAccountService.RollBackLastApproved(id, 72);
                return false;
            }
        }

        public bool AnnouncementWFInitSccess(int AnnouncementID, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {

                    using (var context = new IOASDBEntities())
                    {
                        var queryDeanNote = context.tblRCTAnnouncementMaster.FirstOrDefault(m => m.AnnouncementID == AnnouncementID && m.Status == 3);
                        if (queryDeanNote != null)
                        {
                            queryDeanNote.Status = 4;
                            queryDeanNote.Upt_User = loggedInUser;
                            queryDeanNote.Upt_TS = DateTime.Now;
                            #region Status
                            tblRCTAnnouncementStatusLog status = new tblRCTAnnouncementStatusLog();
                            status.AnnouncementID = AnnouncementID;
                            status.PresentStatus = 3;
                            status.NewStatus = 4;
                            status.preBy = Common.GetPIName(loggedInUser);
                            status.Crt_By = loggedInUser;
                            status.Crt_TS = DateTime.Now;
                            status.Message = "Approved by user";
                            context.tblRCTAnnouncementStatusLog.Add(status);
                            context.SaveChanges();
                            #endregion
                            return true;
                        }
                        var queryShortDeanNote = context.tblRCTAnnouncementMaster.FirstOrDefault(m => m.AnnouncementID == AnnouncementID && m.Status == 7);
                        if (queryShortDeanNote != null)
                        {
                            queryShortDeanNote.Status = 10;
                            queryShortDeanNote.Upt_User = loggedInUser;
                            queryShortDeanNote.Upt_TS = DateTime.Now;
                            context.SaveChanges();
                            #region Status
                            tblRCTAnnouncementStatusLog status = new tblRCTAnnouncementStatusLog();
                            status.AnnouncementID = AnnouncementID;
                            status.PresentStatus = 7;
                            status.NewStatus = 9;
                            status.preBy = Common.GetPIName(loggedInUser);
                            status.Crt_By = loggedInUser;
                            status.Crt_TS = DateTime.Now;
                            status.Message = "Shortlisted candidate approved by user";
                            context.tblRCTAnnouncementStatusLog.Add(status);
                            context.SaveChanges();
                            #endregion
                            return true;
                        }
                        var querySelectionDeanNote = context.tblRCTAnnouncementMaster.FirstOrDefault(m => m.AnnouncementID == AnnouncementID && m.Status == 11);
                        if (querySelectionDeanNote != null)
                        {
                            querySelectionDeanNote.Status = 13;
                            querySelectionDeanNote.Upt_User = loggedInUser;
                            querySelectionDeanNote.Upt_TS = DateTime.Now;
                            context.SaveChanges();
                            #region Status
                            tblRCTAnnouncementStatusLog status = new tblRCTAnnouncementStatusLog();
                            status.AnnouncementID = AnnouncementID;
                            status.PresentStatus = 11;
                            status.NewStatus = 13;
                            status.preBy = Common.GetPIName(loggedInUser);
                            status.Crt_By = loggedInUser;
                            status.Crt_TS = DateTime.Now;
                            status.Message = "Selected candidate approved by user";
                            context.tblRCTAnnouncementStatusLog.Add(status);
                            context.SaveChanges();
                            #endregion
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CONAPWFInitSccess(int CONAPId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = (from c in context.tblRCTConsultantAppointment
                                     from d in context.tblRCTDesignation
                                     where c.DesignationId == d.DesignationId && c.ConsultantAppointmentId == CONAPId
                                     && c.Status == "Sent for approval"
                                     select new { c, d }).FirstOrDefault();
                        if (query != null)
                        {
                            query.c.Status = "Awaiting commitment booking";
                            query.c.UptdUser = loggedInUser;
                            query.c.UptdTs = DateTime.Now;
                            tblRCTCommitmentRequest Commitment = new tblRCTCommitmentRequest();
                            Commitment.ReferenceNumber = query.c.ApplicationNumber;
                            Commitment.AppointmentType = "Consultant Appointment";
                            Commitment.TypeCode = "CON";
                            Commitment.CandidateName = query.c.Name;
                            Commitment.CandidateDesignation = query.d.Designation;
                            Commitment.ProjectId = query.c.ProjectId;
                            Commitment.ProjectNumber = Common.GetProjectNumber(query.c.ProjectId ?? 0);
                            Commitment.TotalSalary = query.c.Salary;
                            Commitment.RequestedCommitmentAmount = query.c.CommitmentAmount;
                            Commitment.Status = "Awaiting Commitment Booking";
                            Commitment.RequestType = "New Appointment";
                            context.tblRCTCommitmentRequest.Add(Commitment);
                            context.SaveChanges();
                            RequirementService.PostCONStatusLog(CONAPId, "Sent for approval", query.c.Status, loggedInUser);

                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool STEWFInitSccess(int STEID, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = (from S in context.tblRCTSTE
                                     from D in context.tblRCTDesignation
                                     where S.DesignationId == D.DesignationId && S.STEID == STEID
                                     && S.Status == "Sent for approval"
                                     select new { S, D }).FirstOrDefault();
                        if (query != null)
                        {

                            bool nofund_f = Common.IsAvailablefundProject(query.S.ProjectId ?? 0, query.S.CommitmentAmount ?? 0,query.S.TypeofAppointment);
                            if (query.S.CSIRStaffPayMode == 2)
                            {
                                query.S.Status = "Awaiting Committee Approval";
                            }
                            else if (query.S.TypeofAppointment == 4 && nofund_f)
                            {
                                query.S.isGovAgencyFund = true;
                                query.S.Status = "Awaiting Committee Approval";
                            }
                            else
                            {
                                query.S.Status = "Awaiting Commitment Booking";

                                tblRCTCommitmentRequest add = new tblRCTCommitmentRequest();
                                add.ReferenceNumber = query.S.ApplicationNumber;
                                add.AppointmentType = "Short Term Engagement";
                                add.TypeCode = "STE";
                                add.CandidateName = query.S.Name;
                                add.CandidateDesignation = query.D.Designation;
                                add.ProjectId = query.S.ProjectId;
                                add.ProjectNumber = Common.getprojectnumber(query.S.ProjectId ?? 0);
                                add.TotalSalary = query.S.Salary;
                                add.RequestedCommitmentAmount = query.S.CommitmentAmount;
                                add.Status = "Awaiting Commitment Booking";
                                add.RequestType = "New Appointment";
                                add.Crtd_TS = DateTime.Now;
                                add.Crtd_UserId = loggedInUser;
                                context.tblRCTCommitmentRequest.Add(add);
                            }

                            query.S.UptdUser = loggedInUser;
                            query.S.UptdTs = DateTime.Now;
                            context.SaveChanges();
                            RequirementService.PostSTEStatusLog(STEID, "Sent for approval", query.S.Status, loggedInUser);
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string CommitmentNo(string AppRefNo, int? OrderId = null)
        {
            string CommitmentNumber = string.Empty;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (OrderId > 0)
                    {
                        CommitmentNumber = (from c in context.tblRCTCommitmentRequest
                                            where c.OrderId == OrderId && (c.Status == "Commitment Booked" || c.Status == "Commitment Withdrawn")
                                            && (c.RequestType == "Add Commitment" || c.RequestType == "New Commitment" || c.RequestType == "Withdraw Commitment")
                                            orderby c.RecruitmentRequestId descending
                                            select c.CommitmentNumber).FirstOrDefault();
                    }

                    CommitmentNumber = (from c in context.tblRCTCommitmentRequest
                                        where c.ReferenceNumber == AppRefNo
                                        && c.Status == "Commitment Booked" && c.RequestType == "New Appointment"
                                        orderby c.RecruitmentRequestId descending
                                        select c.CommitmentNumber).FirstOrDefault();

                }
                return CommitmentNumber;
            }
            catch (Exception ex)
            {
                return CommitmentNumber;
            }
        }

        public bool STEVERWFInitSccess(int STEID, int loggedInUser)
        {
            string EmployeeID = string.Empty;
            STEVerificationModel model = new STEVerificationModel();
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = (from S in context.tblRCTSTE
                                     from D in context.tblRCTDesignation
                                     where S.DesignationId == D.DesignationId && S.STEID == STEID
                                     && S.Status == "Sent for approval-Verify"
                                     select new { S, D }).FirstOrDefault();
                        if (query != null)
                        {
                            decimal WidthdrawAmmount = 0;
                            //if employee late join late join amount should be 
                            if (query.S.AppointmentStartdate < query.S.ActualDate && query.S.CSIRStaffPayMode != 2)
                            {
                                DateTime FromDate = query.S.AppointmentStartdate ?? DateTime.Now;
                                DateTime ToDate = query.S.ActualDate ?? DateTime.Now;
                                WidthdrawAmmount = Common.calculateWithdrawalAmount(STEID, "STE", FromDate, ToDate, true, 0, true);
                                if (WidthdrawAmmount > 0)
                                {
                                    tblRCTCommitmentRequest wd = new tblRCTCommitmentRequest();
                                    wd.ReferenceNumber = query.S.ApplicationNumber;
                                    wd.AppointmentType = "Verfication";
                                    wd.TypeCode = "STE";
                                    wd.CandidateName = query.S.Name;
                                    wd.CandidateDesignation = query.D.Designation;
                                    wd.ProjectId = query.S.ProjectId;
                                    wd.ProjectNumber = Common.getprojectnumber(query.S.ProjectId ?? 0);
                                    wd.TotalSalary = query.S.Salary;
                                    wd.RequestedCommitmentAmount = WidthdrawAmmount;
                                    wd.Status = "Awaiting Commitment Booking";
                                    wd.RequestType = "Withdraw Commitment";
                                    wd.EmpNumber = query.S.EmployeersID;
                                    wd.EmpId = loggedInUser;
                                    wd.Crtd_TS = DateTime.Now;
                                    wd.Crtd_UserId = loggedInUser;
                                    context.tblRCTCommitmentRequest.Add(wd);
                                    context.SaveChanges();
                                }
                            }

                            var Updateqry = (from a in context.tblRCTSTE where a.STEID == STEID select a).FirstOrDefault();
                            if (Updateqry != null)
                            {
                                var Actualstartdate = Updateqry.AppointmentStartdate;
                                Updateqry.ActualAppointmentStartDate = Actualstartdate;
                                Updateqry.ActualAppointmentEndDate = Updateqry.AppointmentEnddate;
                                decimal CommitmentAmount = 0;
                                CommitmentAmount = Updateqry.CommitmentAmount ?? 0;
                                Updateqry.AppointmentStartdate = query.S.ActualDate;                               
                                if (WidthdrawAmmount > 0)
                                    Updateqry.CommitmentAmount = CommitmentAmount - WidthdrawAmmount;
                                context.SaveChanges();
                            }


                            query.S.Status = "Verification Completed";
                            int VerificationSeqNo = 0;
                            int VerificationSequenceNo = (from SM in context.tblRCTSTE select SM.VerificationSeqNo).Max() ?? 0;
                            VerificationSeqNo = VerificationSequenceNo == 0 ? 10001 : VerificationSequenceNo + 1;
                            EmployeeID = query.S.OldNumber;
                           
                            if (query.S.EmployeeCategory == "Old Employee" && EmployeeID.Contains("IC"))
                            {
                                var preQuery = (from s in context.tblRCTSTE
                                                where s.EmployeersID == EmployeeID && s.IsActiveNow == true
                                                orderby s.STEID descending
                                                select s).FirstOrDefault();
                                if (preQuery != null)
                                {
                                    preQuery.IsActiveNow = false;
                                    context.SaveChanges();
                                    query.S.Paybill = !string.IsNullOrEmpty(preQuery.Paybill) ? preQuery.Paybill : null;
                                }
                            }
                            else
                                EmployeeID = "IC" + VerificationSeqNo;

                            if (EmployeeID.Contains("IC"))
                                query.S.EmployeersID = EmployeeID;
                            else
                                query.S.EmployeersID = "IC" + VerificationSeqNo;

                            query.S.VerificationSeqNo = VerificationSeqNo;
                            query.S.isEmployee = true;
                            query.S.IsActiveNow = true;
                            //Update Commitment table
                            var QryCommitment = (from C in context.tblRCTCommitmentRequest
                                                 where C.ReferenceNumber == query.S.ApplicationNumber && C.Status == "Commitment Booked"
                                                 select C).FirstOrDefault();
                            if (QryCommitment != null)
                                QryCommitment.EmpNumber = EmployeeID;
                            query.S.CommitmentNo = CommitmentNo(query.S.ApplicationNumber);
                           
                            context.SaveChanges();
                            Common.EmployeeHistoryLog(STEID, "STE");

                            //BA
                            tblRCTOrderEffectHistory his = new tblRCTOrderEffectHistory();
                            his.ApplicationId = STEID;
                            his.AppointmentType = "STE";
                            his.Basic = query.S.Salary;
                            his.DesignationId = query.S.DesignationId;
                            his.EffectiveFrom = query.S.ActualDate;
                            his.EffectiveTo = query.S.AppointmentEnddate;
                            his.EmployeeId = EmployeeID;
                            his.HRA = query.S.HRA;
                            his.Medical = query.S.MedicalAmmount;
                            his.OrderDate = DateTime.Now;
                            his.OrderTypeId = 0;
                            his.OrderId = 0;
                            his.OrderType = "New";
                            his.ProjectId = query.S.ProjectId;
                            his.AppointmentStartDate = query.S.ActualDate;
                            his.AppointmentEndDate = query.S.AppointmentEnddate;
                            his.isMedicalInclusive = query.S.Medical == 2 ? true : false;
                            his.IITMPensioner_f = query.S.IITMPensionerOrCSIRStaff == 1 ? true : false;
                            context.tblRCTOrderEffectHistory.Add(his);
                            context.SaveChanges();
                            #region Employee portal
                            if (query.S.EmployeeCategory == "New Employee")
                            {
                                using (var EmployeeContext = new ICSRExternalEntities())
                                {
                                    using (var Employeetransaction = EmployeeContext.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            var checkEmployeeExist = EmployeeContext.tblProjectStaffUser.Where(x => x.UserName == EmployeeID).FirstOrDefault();
                                            if (checkEmployeeExist == null)
                                            {
                                                var departdetail = Common.GetEmployeeDepartment(query.S.ProjectId ?? 0);
                                                tblProjectStaffUser addEmployeelogin = new tblProjectStaffUser();
                                                addEmployeelogin.UserName = EmployeeID;
                                                addEmployeelogin.Email = query.S.Email;
                                                addEmployeelogin.Name = query.S.Name;
                                                addEmployeelogin.Password = Guid.NewGuid().ToString("N").Substring(0, 12);
                                                addEmployeelogin.Status = "Active";
                                                addEmployeelogin.RoleId = 2;
                                                addEmployeelogin.Crts_Ts = DateTime.Now;
                                                addEmployeelogin.Designation = query.D.Designation;
                                                addEmployeelogin.DeptCode = departdetail.Item1;
                                                addEmployeelogin.DeptName = departdetail.Item2;
                                                EmployeeContext.tblProjectStaffUser.Add(addEmployeelogin);
                                                EmployeeContext.SaveChanges();
                                                int projectstaffid = addEmployeelogin.ProjectStaffId;
                                                var statusemail = RCTEmailContentService.SendMailProjectStaffNewuser(projectstaffid, EmployeeContext, query.S.Email);
                                                if (statusemail == 2 || statusemail == -1)
                                                {
                                                    Employeetransaction.Rollback();
                                                    //transaction.Rollback();
                                                    //return Tuple.Create(-1,0, "Employee portal Credentials not send this email Please Contact Administrator");
                                                    return false;
                                                }
                                            }
                                            Employeetransaction.Commit();
                                        }
                                        catch (Exception ex)
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                            else if (query.S.EmployeeCategory == "Old Employee")
                            {
                                using (var EmployeeContext = new ICSRExternalEntities())
                                {
                                    using (var Employeetransaction = EmployeeContext.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            var checkEmployeeExist = EmployeeContext.tblProjectStaffUser.Where(x => x.UserName == EmployeeID).FirstOrDefault();
                                            if (checkEmployeeExist != null)
                                            {
                                                var departdetail = Common.GetEmployeeDepartment(query.S.ProjectId ?? 0);
                                                checkEmployeeExist.Email = query.S.Email;
                                                checkEmployeeExist.Name = query.S.Name;
                                                checkEmployeeExist.DeptCode = departdetail.Item1;
                                                checkEmployeeExist.DeptName = departdetail.Item2;
                                                checkEmployeeExist.Status = "Active";
                                                checkEmployeeExist.Uptd_Ts = DateTime.Now;
                                                checkEmployeeExist.Uptd_Id = loggedInUser;
                                                context.SaveChanges();
                                            }
                                            Employeetransaction.Commit();
                                        }
                                        catch (Exception ex)
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                            #endregion
                            RequirementService.PostSTEStatusLog(STEID, "Sent for approval-Verify", "Verification Completed", loggedInUser);
                            return true;
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool OSGVERWFInitSccess(int OSGID, int loggedInUser)
        {
            string EmployeersID = string.Empty;
            STEVerificationModel model = new STEVerificationModel();
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        //var query = (from S in context.tblRCTSTE
                        //             from D in context.tblRCTDesignation
                        //             where S.DesignationId == D.DesignationId && S.STEID == STEID
                        //             && S.Status == "Sent for approval-Verify"
                        //             select new { S, D }).FirstOrDefault();

                        var _qryOSG = (from s in context.tblRCTOutsourcing
                                       from d in context.tblRCTDesignation
                                       where s.DesignationId == d.DesignationId && s.Status == "Sent for approval-Verify"
                                        && s.OSGID == OSGID
                                       select new { s, d }).FirstOrDefault();
                        if (_qryOSG != null)
                        {
                            decimal WithdrawAmmount = 0;
                            //Check Joining date if candtidate join deloy for the appointment tenure should Widthdraw commitment ammount
                            if (_qryOSG.s.AppointmentStartdate < _qryOSG.s.ActualDate && _qryOSG.s.CSIRStaffPayMode != 2)
                            {
                                DateTime FromDate = _qryOSG.s.AppointmentStartdate ?? DateTime.Now;
                                DateTime ToDate = _qryOSG.s.ActualDate ?? DateTime.Now;
                                WithdrawAmmount = Common.calculateWithdrawalAmount(OSGID, "OSG", FromDate, ToDate, true, 0, true);
                                if (WithdrawAmmount > 0)
                                {
                                    tblRCTCommitmentRequest WidthdrawCommitment = new tblRCTCommitmentRequest();
                                    WidthdrawCommitment.ReferenceNumber = _qryOSG.s.ApplicationNumber;
                                    WidthdrawCommitment.AppointmentType = "Verfication";
                                    WidthdrawCommitment.TypeCode = "OSG";
                                    WidthdrawCommitment.CandidateName = _qryOSG.s.Name;
                                    WidthdrawCommitment.CandidateDesignation = _qryOSG.d.Designation;
                                    WidthdrawCommitment.ProjectId = _qryOSG.s.ProjectId;
                                    WidthdrawCommitment.ProjectNumber = RequirementService.getProjectSummary(_qryOSG.s.ProjectId ?? 0).ProjectNumber;
                                    WidthdrawCommitment.TotalSalary = _qryOSG.s.Salary;
                                    WidthdrawCommitment.RequestedCommitmentAmount = WithdrawAmmount;
                                    WidthdrawCommitment.Status = "Awaiting Commitment Booking";
                                    WidthdrawCommitment.RequestType = "Withdraw Commitment";
                                    WidthdrawCommitment.EmpNumber = _qryOSG.s.EmployeersID;
                                    WidthdrawCommitment.EmpId = loggedInUser;
                                    WidthdrawCommitment.Crtd_TS = DateTime.Now;
                                    WidthdrawCommitment.Crtd_UserId = loggedInUser;
                                    context.tblRCTCommitmentRequest.Add(WidthdrawCommitment);
                                    context.SaveChanges();
                                }
                            }

                            var Updateqry = (from a in context.tblRCTOutsourcing
                                             where a.OSGID == OSGID
                                             select a).FirstOrDefault();
                            if (Updateqry != null)
                            {
                                var Actualstartdate = Updateqry.AppointmentStartdate;
                                Updateqry.ActualAppointmentStartDate = Actualstartdate;
                                Updateqry.ActualAppointmentEndDate = Updateqry.AppointmentEnddate;
                                decimal CommitmentAmount = 0;
                                CommitmentAmount = Updateqry.CommitmentAmount ?? 0;
                                Updateqry.AppointmentStartdate = _qryOSG.s.ActualDate;                                
                                if (WithdrawAmmount > 0)
                                    Updateqry.CommitmentAmount = CommitmentAmount - WithdrawAmmount;
                                context.SaveChanges();
                            }

                            _qryOSG.s.Status = "Verification Completed";
                            int VerificationSeqNo = 0;
                            var QryVerificationSeqNo = (from SM in context.tblRCTOutsourcing select SM.VerificationSeqNo).Max();
                            int VerificationSequenceNo = QryVerificationSeqNo ?? 0;
                            VerificationSeqNo = VerificationSequenceNo == 0 ? 10001 : VerificationSequenceNo + 1;

                            EmployeersID = _qryOSG.s.OldNumber;                           
                            if (_qryOSG.s.EmployeeCategory == "Old Employee" && EmployeersID.Contains("VS"))
                            {
                                var preQuery = (from s in context.tblRCTOutsourcing
                                                where s.EmployeersID == EmployeersID && s.IsActiveNow == true
                                                orderby s.OSGID descending
                                                select s).FirstOrDefault();
                                if (preQuery != null)
                                {
                                    preQuery.IsActiveNow = false;
                                    context.SaveChanges();
                                }
                            }
                            else
                                EmployeersID = "VS" + VerificationSeqNo;

                            if (EmployeersID.Contains("VS"))
                                _qryOSG.s.EmployeersID = EmployeersID;
                            else
                                _qryOSG.s.EmployeersID = "VS" + VerificationSeqNo;

                            _qryOSG.s.IsActiveNow = true;
                            _qryOSG.s.EmployeersID = EmployeersID;
                            _qryOSG.s.VerificationSeqNo = VerificationSeqNo;
                            _qryOSG.s.isEmployee = true;
                            //Update Commitment table
                            string ApplicationRefNo = _qryOSG.s.ApplicationNumber;
                            var QryCommitment = (from C in context.tblRCTCommitmentRequest
                                                 where C.ReferenceNumber == ApplicationRefNo
                                                 && C.Status == "Commitment Booked"
                                                 select C).FirstOrDefault();
                            QryCommitment.EmpNumber = EmployeersID;                            
                            _qryOSG.s.CommitmentNo = CommitmentNo(ApplicationRefNo);


                            context.SaveChanges();
                            var isLogged = Common.EmployeeHistoryLog(OSGID, "OSG");

                            tblRCTOrderEffectHistory his = new tblRCTOrderEffectHistory();
                            his.ApplicationId = OSGID;
                            his.AppointmentType = "OSG";
                            his.Basic = _qryOSG.s.Salary;
                            his.DesignationId = _qryOSG.s.DesignationId;
                            his.EffectiveFrom = _qryOSG.s.ActualDate;
                            his.EffectiveTo = _qryOSG.s.AppointmentEnddate;
                            his.EmployeeId = EmployeersID;
                            his.HRA = _qryOSG.s.HRA;
                            his.Medical = _qryOSG.s.MedicalAmmount;
                            his.ProjectId = _qryOSG.s.ProjectId;
                            his.OrderDate = DateTime.Now;
                            his.OrderTypeId = 0;
                            his.OrderId = 0;
                            his.OrderType = "New";
                            his.AppointmentStartDate = _qryOSG.s.ActualDate;
                            his.AppointmentEndDate = _qryOSG.s.AppointmentEnddate;
                            his.isMedicalInclusive = _qryOSG.s.Medical == 2 ? true : false;
                            his.IITMPensioner_f = _qryOSG.s.IITMPensionerOrCSIRStaff == 1 ? true : false;
                            context.tblRCTOrderEffectHistory.Add(his);
                            context.SaveChanges();
                            #region Employee portal
                            if (_qryOSG.s.EmployeeCategory == "New Employee")
                            {
                                using (var EmployeeContext = new ICSRExternalEntities())
                                {
                                    using (var Employeetransaction = EmployeeContext.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            var checkEmployeeExist = EmployeeContext.tblProjectStaffUser.Where(x => x.UserName == EmployeersID).FirstOrDefault();
                                            if (checkEmployeeExist == null)
                                            {
                                                var departdetail = Common.GetEmployeeDepartment(_qryOSG.s.ProjectId ?? 0);
                                                tblProjectStaffUser addEmployeelogin = new tblProjectStaffUser();
                                                addEmployeelogin.UserName = EmployeersID;
                                                addEmployeelogin.Email = _qryOSG.s.Email;
                                                addEmployeelogin.Name = _qryOSG.s.Name;
                                                addEmployeelogin.Password = Guid.NewGuid().ToString("N").Substring(0, 12);
                                                addEmployeelogin.Status = "Active";
                                                addEmployeelogin.RoleId = 3;
                                                addEmployeelogin.Crts_Ts = DateTime.Now;
                                                addEmployeelogin.Designation = _qryOSG.d.Designation;
                                                addEmployeelogin.DeptCode = departdetail.Item1;
                                                addEmployeelogin.DeptName = departdetail.Item2;
                                                EmployeeContext.tblProjectStaffUser.Add(addEmployeelogin);
                                                EmployeeContext.SaveChanges();
                                                int projectstaffid = addEmployeelogin.ProjectStaffId;
                                                var statusemail = RCTEmailContentService.SendMailProjectStaffNewuser(projectstaffid, EmployeeContext, _qryOSG.s.Email);
                                                if (statusemail == 2 || statusemail == -1)
                                                {
                                                    Employeetransaction.Rollback();
                                                    //transaction.Rollback();
                                                    //return Tuple.Create(-1, OSGID, "Employee portal Credentials not send this email Please Contact Administrator");
                                                    return false;
                                                }
                                            }
                                            Employeetransaction.Commit();
                                        }
                                        catch (Exception ex)
                                        {
                                            Employeetransaction.Rollback();
                                            //transaction.Rollback();
                                            //WriteLog.SendErrorToText(ex);
                                            //return Tuple.Create(-1, OSGID, "");
                                            return false;
                                        }
                                    }
                                }
                            }
                            else if (_qryOSG.s.EmployeeCategory == "Old Employee")
                            {
                                using (var EmployeeContext = new ICSRExternalEntities())
                                {
                                    using (var Employeetransaction = EmployeeContext.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            var checkEmployeeExist = EmployeeContext.tblProjectStaffUser.Where(x => x.UserName == EmployeersID).FirstOrDefault();
                                            if (checkEmployeeExist != null)
                                            {

                                                var departdetail = Common.GetEmployeeDepartment(_qryOSG.s.ProjectId ?? 0);
                                                checkEmployeeExist.Email = _qryOSG.s.Email;
                                                checkEmployeeExist.Name = _qryOSG.s.Name;
                                                checkEmployeeExist.DeptCode = departdetail.Item1;
                                                checkEmployeeExist.DeptName = departdetail.Item2;
                                                checkEmployeeExist.Status = "Active";
                                                checkEmployeeExist.Uptd_Ts = DateTime.Now;
                                                checkEmployeeExist.Uptd_Id = loggedInUser;
                                                context.SaveChanges();
                                            }
                                            Employeetransaction.Commit();
                                        }
                                        catch (Exception ex)
                                        {
                                            Employeetransaction.Rollback();
                                            return false;
                                        }
                                    }
                                }
                            }
                            #endregion
                            RequirementService.PostOSGStatusLog(OSGID, "Sent for approval-Verify", "Verification Completed", loggedInUser);
                            return true;
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool ExecuteSPSalaryChangeComponent()
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    context.Database.ExecuteSqlCommand("SPRCTExtensionAndEnhancementupdate");
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool STEORDVERWFInitSccess(int OrderId, int loggedInUser)
        {
            STEVerificationModel model = new STEVerificationModel();
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = (from o in context.tblOrder
                                     from od in context.tblOrderDetail
                                     from vw in context.vw_RCTOverAllApplicationEntry//o.Status == "Awaiting Verification" || o.Status == "Awaiting Verification-Draft" &&
                                     where o.OrderId == OrderId
                                     && o.OrderId == od.OrderId && o.OrderId == vw.OrderId
                                     select new { od, o, vw }).FirstOrDefault();
                        //var query = context.tblOrder.FirstOrDefault(m => m.OrderId == OrderId && m.Status == "Sent for approval-Verify");
                        if (query != null)
                        {                            
                            query.o.Status = "Completed";
                            context.SaveChanges();

                            var othQuery = context.tblRCTOTHPaymentDeduction.FirstOrDefault(m => m.OrderId == OrderId && m.Status == "Open");
                            if (othQuery != null)
                            {
                                othQuery.Status = "Completed";
                                othQuery.UpdtTs = DateTime.Now;
                                othQuery.UpdtUser = loggedInUser;
                                context.SaveChanges();
                            }

                            var isLogged = Common.EmployeeHistoryLog(query.vw.ApplicationId ?? 0, query.vw.Category, query.vw.OrderId);
                            var curr = DateTime.Now.Date;
                            if (query.o.FromDate <= curr)
                                ExecuteSPSalaryChangeComponent();
                            RequirementService.PostOrderStatusLog(OrderId, "Sent for approval-Verify", "Completed", loggedInUser);
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool OSGORDVERWFInitSccess(int OrderId, int loggedInUser)
        {
            STEVerificationModel model = new STEVerificationModel();
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = (from o in context.tblOrder
                                     from od in context.tblOrderDetail
                                     from vw in context.vw_RCTOverAllApplicationEntry//o.Status == "Awaiting Verification" || o.Status == "Awaiting Verification-Draft" &&
                                     where o.OrderId == OrderId
                                     && o.OrderId == od.OrderId && o.OrderId == vw.OrderId
                                     select new { od, o, vw }).FirstOrDefault();
                        //var query = context.tblOrder.FirstOrDefault(m => m.OrderId == OrderId && m.Status == "Sent for approval-Verify");
                        if (query != null)
                        {
                            query.o.Status = "Completed";
                            context.SaveChanges();

                            var othQuery = context.tblRCTOTHPaymentDeduction.FirstOrDefault(m => m.OrderId == OrderId && m.Status == "Open");
                            if (othQuery != null)
                            {
                                othQuery.Status = "Completed";
                                othQuery.UpdtTs = DateTime.Now;
                                othQuery.UpdtUser = loggedInUser;
                                context.SaveChanges();
                            }

                            var isLogged = Common.EmployeeHistoryLog(query.vw.ApplicationId ?? 0, query.vw.Category, query.vw.OrderId);
                            var curr = DateTime.Now.Date;
                            if (query.o.FromDate <= curr)
                                ExecuteSPSalaryChangeComponent();
                            RequirementService.PostOrderStatusLog(OrderId, "Sent for approval-Verify", "Completed", loggedInUser);
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RecruitCOPWFInitSccess(int OrderId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var queryorder = (from o in context.tblOrder
                                          where o.OrderId == OrderId && o.Status == "Sent for approval"
                                          select o).FirstOrDefault();
                        if (queryorder != null)
                        {
                            RequirementService RQS = new RequirementService();
                            var result = RQS.UpdateCOPDetails(OrderId, loggedInUser);
                            if (!result)
                            {
                                //  coreAccountService.RollBackLastApproved(OrderId, 196);
                                return false;
                            }
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RecruitExtensionWFInitSccess(int OrderId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {

                        var queryorder = (from o in context.tblOrder
                                          where o.OrderId == OrderId && o.Status == "Sent for approval"
                                          select o).FirstOrDefault();
                        if (queryorder != null)
                        {
                            RequirementService RQS = new RequirementService();
                            var result = RQS.UpdateExtensionDetails(OrderId, loggedInUser);
                            if (!result)
                            {
                                // coreAccountService.RollBackLastApproved(OrderId, 197);
                                return result;
                            }
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RecruitEnhancementWFInitSccess(int OrderId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {

                    RequirementService RQS = new RequirementService();
                    var result = RQS.UpdateEnhancementDetails(OrderId, loggedInUser);
                    if (!result)
                    {
                        // coreAccountService.RollBackLastApproved(OrderId, 198);
                        return result;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RecruitHRAWFInitSccess(int OrderId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblOrder.FirstOrDefault(m => m.OrderId == OrderId && m.Status == "Sent for approval");
                        if (query != null)
                        {

                            var querymast = (from m in context.tblRCTSTE
                                             from d in context.tblRCTDesignation
                                             where m.DesignationId == d.DesignationId && m.STEID == query.AppointmentId
                                             select new
                                             {
                                                 m.ApplicationNumber,
                                                 m.Name,
                                                 m.EmployeersID,
                                                 m.ProjectId,
                                                 m.CSIRStaffPayMode,
                                                 m.TypeofAppointment,
                                                 d.Designation
                                             }).FirstOrDefault();
                            if (querymast != null)
                            {
                                query.Status = "Awaiting Commitment Booking";
                                query.UpdtUser = loggedInUser;
                                query.UpdtTS = DateTime.Now;
                                if (query.OrderType == 5)
                                {
                                    var nofund_f = Common.IsAvailablefundProject(querymast.ProjectId ?? 0, query.CommitmentAmmount ?? 0,querymast.TypeofAppointment);
                                    if (querymast.CSIRStaffPayMode == 2)//Payment through agencies
                                    {
                                        query.Status = "Completed";
                                    }
                                    else if (querymast.TypeofAppointment == 4 && nofund_f)
                                    {
                                        query.isGovAgencyFund = true;
                                        query.Status = "Open";
                                    }
                                    else
                                    {
                                        tblRCTCommitmentRequest addCommit = new tblRCTCommitmentRequest();
                                        addCommit.ReferenceNumber = querymast.ApplicationNumber;
                                        addCommit.OrderNumber = query.OrderNo;
                                        addCommit.OrderId = OrderId;
                                        addCommit.AppointmentType = "HRA";
                                        addCommit.TypeCode = "STE";
                                        addCommit.CandidateName = querymast.Name;
                                        addCommit.CandidateDesignation = querymast.Designation;
                                        addCommit.ProjectId = query.NewProjectId;
                                        addCommit.ProjectNumber = Common.getprojectnumber(query.NewProjectId ?? 0);
                                        addCommit.TotalSalary = query.Basic;
                                        addCommit.RequestType = "Add Commitment";
                                        addCommit.RequestedCommitmentAmount = query.CommitmentAmmount;
                                        addCommit.Status = "Awaiting Commitment Booking";
                                        addCommit.EmpNumber = querymast.EmployeersID;
                                        addCommit.EmpId = loggedInUser;
                                        addCommit.Crtd_TS = DateTime.Now;
                                        addCommit.Crtd_UserId = loggedInUser;
                                        context.tblRCTCommitmentRequest.Add(addCommit);
                                        context.SaveChanges();
                                    }
                                }
                                else if (query.OrderType == 6)
                                {
                                    if (querymast.CSIRStaffPayMode == 2)//Payment through agencies
                                    {
                                        query.Status = "Completed";
                                    }
                                    else
                                    {
                                        tblRCTCommitmentRequest withdraw = new tblRCTCommitmentRequest();
                                        withdraw.ReferenceNumber = querymast.ApplicationNumber;
                                        withdraw.OrderNumber = query.OrderNo;
                                        withdraw.OrderId = OrderId;
                                        withdraw.AppointmentType = "HRA";
                                        withdraw.TypeCode = "STE";
                                        withdraw.CandidateName = querymast.Name;
                                        withdraw.CandidateDesignation = querymast.Designation;
                                        withdraw.ProjectId = query.NewProjectId;
                                        withdraw.ProjectNumber = Common.getprojectnumber(query.NewProjectId ?? 0);
                                        withdraw.TotalSalary = query.Basic;
                                        withdraw.RequestType = "Withdraw Commitment";
                                        withdraw.RequestedCommitmentAmount = query.WithdrawAmmount;
                                        withdraw.Status = "Awaiting Commitment Booking";
                                        withdraw.EmpNumber = querymast.EmployeersID;
                                        withdraw.EmpId = loggedInUser;
                                        withdraw.Crtd_TS = DateTime.Now;
                                        withdraw.Crtd_UserId = loggedInUser;
                                        context.tblRCTCommitmentRequest.Add(withdraw);
                                        context.SaveChanges();
                                    }
                                }
                                context.SaveChanges();
                            }

                            if (query.Status == "Completed")
                            {
                                var queryOTH = context.tblRCTOTHPaymentDeduction.FirstOrDefault(m => m.OrderId == OrderId && m.Status == "Open");
                                if (queryOTH != null)
                                {
                                    queryOTH.Status = "Completed";
                                    context.SaveChanges();
                                }

                                var logged_f = Common.EmployeeHistoryLog(query.AppointmentId ?? 0, "STE", query.OrderId);
                                var component_f = RequirementService.ExecuteSPSalaryChangeComponent();
                                if (!logged_f || !component_f)
                                {
                                    //coreAccountService.RollBackLastApproved(OrderId, 190);
                                    return false;
                                }

                                if (query.OrderType == 5)
                                {
                                    var orderpost_f = RequirementService.PostOfferDetails(query.AppointmentId ?? 0, "STE", "Order", loggedInUser, OrderId);
                                    if (!orderpost_f)
                                    {
                                        //coreAccountService.RollBackLastApproved(OrderId, 190);
                                        return false;
                                    }
                                }

                                if (query.OrderType == 6)
                                {
                                    var res = RCTEmailContentService.SendMailForHRA(query.OrderId, loggedInUser);
                                    if (res.Item1 != 1)
                                    {
                                        //coreAccountService.RollBackLastApproved(OrderId, 190);
                                        return false;
                                    }
                                }
                            }
                            RequirementService.PostOrderStatusLog(OrderId, "Sent for approval", query.Status, loggedInUser);
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RecruitSLPWFInitSccess(int OrderId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var queryorder = (from o in context.tblOrder
                                          from vw in context.vw_RCTOverAllApplicationEntry
                                          from od in context.tblOrderDetail
                                          where o.OrderId == od.OrderId && o.OrderId == vw.OrderId && o.OrderId == OrderId
                                          && (o.Status == "Initiated" || o.Status == "Reversal")
                                          select new { o, od, vw }).FirstOrDefault();
                        if (queryorder != null)
                        {
                            if (!RequirementService.UpdateSPLOPDetails(OrderId, loggedInUser))
                            {
                                //   coreAccountService.RollBackLastApproved(OrderId, 203);
                                return false;
                            }
                            queryorder.o.UpdtUser = loggedInUser;
                            queryorder.o.UpdtTS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler WriteLog = new ErrorHandler();
                WriteLog.SendErrorToText(ex);
                return false;
            }
        }

        public bool RecruitRelieveWFInitSccess(int OrderId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var queryorder = (from o in context.tblOrder
                                          where o.OrderId == OrderId && o.Status == "Relieving initiated"
                                          select o).FirstOrDefault();
                        if (queryorder != null)
                        {
                            RequirementService RQS = new RequirementService();
                            var result = RQS.UpdateRelieveDetails(OrderId, loggedInUser);
                            if (!result)
                            {
                                //coreAccountService.RollBackLastApproved(OrderId, 201);
                                return false;
                            }
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RecruitMLWFInitSccess(int OrderId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var ordQuery = (from o in context.tblOrder
                                        from od in context.tblOrderDetail
                                        where o.OrderId == od.OrderId && o.OrderId == OrderId
                                        select new { o, od }).FirstOrDefault();
                        if (ordQuery != null)
                        {
                            string prestatus = "";
                            bool mailsent_f = false;
                            prestatus = ordQuery.o.Status;
                            if (ordQuery.o.Status == "Initiated")
                            {
                                var mastQuery = (from vw in context.vw_RCTOverAllApplicationEntry
                                                 where vw.ApplicationId == ordQuery.o.AppointmentId && vw.AppointmentType == ordQuery.o.AppointmentType
                                                 && vw.ApplicationType == "New"
                                                 select new { vw }).FirstOrDefault();
                                if (mastQuery != null)
                                {
                                    var hislogged_f = Common.EmployeeHistoryLog(ordQuery.o.AppointmentId ?? 0, mastQuery.vw.Category, ordQuery.o.OrderId);
                                    if (!hislogged_f)
                                    {
                                        // coreAccountService.RollBackLastApproved(OrderId, 202);
                                        return false;
                                    }

                                    tblRCTOrderEffectHistory his = new tblRCTOrderEffectHistory();
                                    his.ApplicationId = ordQuery.o.AppointmentId ?? 0;
                                    his.AppointmentType = mastQuery.vw.Category;
                                    his.Basic = mastQuery.vw.BasicPay;
                                    his.DesignationId = mastQuery.vw.DesignationId;
                                    his.EffectiveFrom = ordQuery.o.FromDate;
                                    his.EffectiveTo = ordQuery.o.ToDate;
                                    his.EmployeeId = mastQuery.vw.EmployeersID;
                                    his.HRA = mastQuery.vw.HRA;
                                    his.Medical = mastQuery.vw.MedicalAmmount;
                                    his.OrderId = ordQuery.o.OrderId;
                                    his.OrderTypeId = ordQuery.o.OrderType;
                                    his.OrderType = "Maternity Leave Application";
                                    his.ProjectId = mastQuery.vw.ProjectId;
                                    his.isMedicalInclusive = mastQuery.vw.MedicalType == 2 ? true : false;
                                    his.IITMPensioner_f = mastQuery.vw.IITMPensionerorCSIRStaff == "IITM Pensioner" ? true : false;
                                    context.tblRCTOrderEffectHistory.Add(his);
                                    context.SaveChanges();
                                }
                                ordQuery.o.Status = "Open";
                                mailsent_f = RCTEmailContentService.SendMaterbityLeaveMail(OrderId, loggedInUser);
                            }
                            else if (ordQuery.o.Status == "Rejoined")
                            {
                                ordQuery.o.Status = "Completed";
                                if (!ordQuery.o.isUpdated)
                                    ordQuery.o.isUpdated = true;
                                mailsent_f = RCTEmailContentService.SendMaterbityLeaveMail(OrderId, loggedInUser, true);
                            }
                            if (!mailsent_f)
                            {
                                //  coreAccountService.RollBackLastApproved(OrderId, 202);
                                return false;
                            }
                            ordQuery.o.UpdtUser = loggedInUser;
                            ordQuery.o.UpdtTS = DateTime.Now;
                            context.SaveChanges();
                            RequirementService.PostOrderStatusLog(OrderId, prestatus, ordQuery.o.Status, loggedInUser);
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler WriteLog = new ErrorHandler();
                WriteLog.SendErrorToText(ex);
                //   coreAccountService.RollBackLastApproved(OrderId, 202);
                return false;
            }
        }

        public bool RecruitAmendmentWFInitSccess(int OrderId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = (from o in context.tblOrder
                                     from od in context.tblOrderDetail
                                     from vw in context.vw_RCTOverAllApplicationEntry
                                     where o.OrderId == od.OrderId && o.OrderId == vw.OrderId &&
                                     o.Status == "Sent for approval" && o.OrderId == OrderId
                                     select new { o, od, vw }).FirstOrDefault();
                        if (query != null)
                        {

                            query.o.Status = "Awaiting Commitment Booking";
                            query.o.UpdtUser = loggedInUser;
                            query.o.UpdtTS = DateTime.Now;
                            query.o.isExtended = true;

                            tblRCTCommitmentRequest addorwithdraw = new tblRCTCommitmentRequest();
                            addorwithdraw.ReferenceNumber = query.vw.ApplicationNo;
                            addorwithdraw.OrderNumber = query.o.OrderNo;
                            addorwithdraw.OrderId = OrderId;
                            addorwithdraw.AppointmentType = "Amendment";
                            addorwithdraw.TypeCode = query.vw.Category;
                            addorwithdraw.CandidateName = query.vw.CandidateName;
                            addorwithdraw.CandidateDesignation = query.vw.PostRecommended;
                            addorwithdraw.ProjectId = query.o.NewProjectId;
                            addorwithdraw.ProjectNumber = Common.getprojectnumber(query.o.NewProjectId ?? 0);
                            addorwithdraw.TotalSalary = query.o.Basic;
                            if (query.od.WithdrawCommitment == true)
                            {
                                addorwithdraw.RequestedCommitmentAmount = query.o.WithdrawAmmount;
                                addorwithdraw.RequestType = "Withdraw Commitment";
                            }
                            else
                            {
                                addorwithdraw.RequestedCommitmentAmount = query.o.CommitmentAmmount;
                                addorwithdraw.RequestType = "Add Commitment";
                            }
                            addorwithdraw.Status = "Awaiting Commitment Booking";
                            addorwithdraw.EmpNumber = query.vw.EmployeersID;
                            addorwithdraw.EmpId = loggedInUser;
                            addorwithdraw.Crtd_TS = DateTime.Now;
                            addorwithdraw.Crtd_UserId = loggedInUser;
                            context.tblRCTCommitmentRequest.Add(addorwithdraw);
                            context.SaveChanges();
                            RequirementService.PostOrderStatusLog(OrderId, "Sent for approval", query.o.Status, loggedInUser);
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool OSGWFInitSuccess(int OSGID, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var Query = (from c in context.tblRCTOutsourcing
                                     from d in context.tblRCTDesignation
                                     where c.DesignationId == d.DesignationId && c.OSGID == OSGID
                                     && c.Status == "Sent for approval"
                                     select new { c, d }).FirstOrDefault();
                        if (Query != null)
                        {
                            Query.c.Status = "Awaiting Commitment Booking";
                            Query.c.UptdUser = loggedInUser;
                            Query.c.UptdTs = DateTime.Now;
                            tblRCTCommitmentRequest Commitment = new tblRCTCommitmentRequest();
                            Commitment.ReferenceNumber = Query.c.ApplicationNumber;
                            Commitment.AppointmentType = "Outsourcing";
                            Commitment.TypeCode = "OSG";
                            Commitment.CandidateName = Query.c.Name;
                            Commitment.CandidateDesignation = Query.d.Designation;
                            Commitment.ProjectId = Query.c.ProjectId;
                            Commitment.ProjectNumber = Common.getprojectnumber(Query.c.ProjectId ?? 0);
                            Commitment.TotalSalary = Query.c.Salary;
                            Commitment.RequestedCommitmentAmount = Query.c.CommitmentAmount;
                            Commitment.Status = "Awaiting Commitment Booking";
                            Commitment.RequestType = "New Appointment";
                            Commitment.EmpNumber = Query.c.EmployeersID;
                            Commitment.RefId = Query.c.OSGID;
                            Commitment.Crtd_TS = DateTime.Now;
                            Commitment.Crtd_UserId = loggedInUser;
                            context.tblRCTCommitmentRequest.Add(Commitment);
                            context.SaveChanges();
                            RequirementService.PostOSGStatusLog(OSGID, "Sent for approval", Query.c.Status, loggedInUser);
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RecruitOTHPDWFInitSccess(int OthId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = (from oth in context.tblRCTOTHPaymentDeduction
                                     where oth.OTHPayDeductionId == OthId && oth.Status == "Sent for approval"
                                     select oth).FirstOrDefault();
                        if (query != null)
                        {
                            if (query.IsNoCommitment_f != true)
                            {
                                string Empno = query.EmployeeNo;
                                var verct = (from vw in context.vw_RCTOverAllApplicationEntry
                                             where vw.EmployeeNo == Empno && vw.ApplicationType == "New" && vw.IsActiveNow == true
                                             orderby vw.AppointmentEnddate descending
                                             select vw).FirstOrDefault();
                                decimal sumofpayment = (from othd in context.tblRCTOTHPaymentDeductionDetail
                                                        where othd.OTHPayDeductionId == OthId && othd.OtherType == 1 && othd.Status != "InActive"
                                                        select othd.Amount).Sum() ?? 0;
                                decimal sumofdeduction = (from othd in context.tblRCTOTHPaymentDeductionDetail
                                                          where othd.OTHPayDeductionId == OthId && othd.OtherType == 2 && othd.Status != "InActive"
                                                          select othd.Amount).Sum() ?? 0;

                                query.Status = "Awaiting Commitment Booking";
                                query.UpdtTs = DateTime.Now;
                                query.UpdtUser = loggedInUser;
                                tblRCTCommitmentRequest AddCommitment = new tblRCTCommitmentRequest();
                                AddCommitment.ReferenceNumber = verct.ApplicationNo;

                                AddCommitment.TypeCode = verct.Category;
                                AddCommitment.CandidateName = verct.CandidateName;
                                AddCommitment.CandidateDesignation = verct.PostRecommended;
                                AddCommitment.ProjectId = query.ProjectId;
                                AddCommitment.ProjectNumber = Common.getprojectnumber(query.ProjectId ?? 0);
                                AddCommitment.TotalSalary = verct.BasicPay;
                                AddCommitment.RefId = query.OTHPayDeductionId;
                                if (sumofpayment >= sumofdeduction)
                                {
                                    AddCommitment.RequestType = "Add Commitment";
                                    AddCommitment.AppointmentType = "OtherPayment";
                                    AddCommitment.RequestedCommitmentAmount = Math.Round(sumofpayment - sumofdeduction);
                                }
                                else if (sumofpayment <= sumofdeduction)
                                {
                                    AddCommitment.RequestType = "Withdraw Commitment";
                                    AddCommitment.AppointmentType = "OtherDeduction";
                                    AddCommitment.RequestedCommitmentAmount = Math.Round(sumofdeduction - sumofpayment);
                                }


                                //if (query.OtherType == 1)
                                //{

                                //    AddCommitment.RequestType = "Add Commitment";
                                //    AddCommitment.AppointmentType = "OtherPayment";
                                //}
                                //else
                                //{
                                //    AddCommitment.RequestType = "Withdraw Commitment";
                                //    AddCommitment.AppointmentType = "OtherDeduction";
                                //}
                                AddCommitment.Status = "Awaiting Commitment Booking";
                                AddCommitment.EmpNumber = query.EmployeeNo;
                                AddCommitment.EmpId = loggedInUser;
                                AddCommitment.Crtd_TS = DateTime.Now;
                                AddCommitment.Crtd_UserId = loggedInUser;
                                context.tblRCTCommitmentRequest.Add(AddCommitment);
                                context.SaveChanges();
                                #region log
                                tblRCTOTHPaymentDeductionStatuslog log = new tblRCTOTHPaymentDeductionStatuslog();
                                log.OTHPayDeductionId = query.OTHPayDeductionId;
                                log.PresentStatus = "Sent for approval";
                                log.NewStatus = "Awaiting Commitment Booking";
                                //log.preby = Common.GetUserFirstName(loggedInUser);
                                log.Crt_By = loggedInUser;
                                log.Crt_Ts = DateTime.Now;
                                log.Message = "Sent for approval";
                                context.tblRCTOTHPaymentDeductionStatuslog.Add(log);
                                context.SaveChanges();
                                #endregion
                            }
                            else
                            {
                                query.Status = "Completed";
                                query.UpdtTs = DateTime.Now;
                                query.UpdtUser = loggedInUser;
                                context.SaveChanges();
                                #region log
                                tblRCTOTHPaymentDeductionStatuslog log = new tblRCTOTHPaymentDeductionStatuslog();
                                log.OTHPayDeductionId = query.OTHPayDeductionId;
                                log.PresentStatus = "Sent for approval";
                                log.NewStatus = "Completed";
                                //log.preby = Common.GetUserFirstName(loggedInUser);
                                log.Crt_By = loggedInUser;
                                log.Crt_Ts = DateTime.Now;
                                log.Message = "Sent for approval";
                                context.tblRCTOTHPaymentDeductionStatuslog.Add(log);
                                context.SaveChanges();
                                #endregion
                            }
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

        public bool RecruitOTHPUploadInitSccess(int OthIdMastrid, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = (from oth in context.tblRCTOTHPaymentDeductionUploadMaster
                                     where oth.OTHUploadMasterId == OthIdMastrid && oth.Status == "Sent for approval"
                                     select oth).FirstOrDefault();
                        if (query != null)
                        {
                            RequirementService RQS = new RequirementService();
                            var res = RQS.ValidateAndaddOtherPayment(query.OTHPaymentDeductionUploadId ?? 0, loggedInUser);
                            if (res.Item2 != 1)
                            {
                                //    coreAccountService.RollBackLastApproved(OthIdMastrid, 216);
                                return false;
                            }
                            query.Status = "Completed";
                            query.Uptd_Ts = DateTime.Now;
                            query.Uptd_by = loggedInUser;
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


        private static readonly Object ReceiptBUlockObj = new Object();
        public bool ReceiptBUWFInitSuccess(int id, int loggedInUser)
        {
            try
            {
                lock (ReceiptBUlockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblReceiptBreakup.FirstOrDefault(m => m.ReceiptBreakupId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            query.Status = "Completed";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_Ts = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        {
                            //   coreAccountService.RollBackLastApproved(id, 217);
                            return false;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                // coreAccountService.RollBackLastApproved(id, 217);
                return false;
            }
        }
        public bool OverheadsPostingWFInitSuccess(Int32 id, int loggedInUser)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (!coreAccountService.ValidateOverheadsPostingBillApproved(id))
                        return false;
                    var billQuery = context.tblOverheadsPosting.FirstOrDefault(m => m.OverheadsPostingId == id && m.Status == "Submit for approval" && m.TransactionTypeCode == "OHP");
                    if (billQuery != null)
                    {
                        var ohdetails = (from o in context.tblOverheadsPostingDetails
                                         where o.OverheadsPostingId == id
                                         select o).ToList();

                        for (int i = 0; i < ohdetails.Count(); i++)
                        {
                            int ProjectId = ohdetails[i].ProjectId ?? 0;
                            decimal newOHamt = ohdetails[i].TotalOverheadsAmount ?? 0;
                            int ReceiptId = ohdetails[i].ReceiptId ?? 0;
                            decimal OHamt = context.tblReceipt.Where(m => m.ReceiptId == ReceiptId).Select(m => m.ReceiptOverheadValue).FirstOrDefault() ?? 0;
                            if (!Common.ValidateProjectSummary(ProjectId, 6, OHamt, true, newOHamt))
                                return false;
                        }

                        if (!coreAccountService.getOHPostingBOAmodeldetails(id))
                        {
                            return false;
                        }
                        else
                        {
                            var expensedet = (from c in context.tblOverheadsPostingCorpusDetails
                                              where c.OverheadsPostingId == id
                                              select c).ToList();
                            for (int i = 0; i < expensedet.Count(); i++)
                            {
                                var rcvid = expensedet[i].ReceiptId ?? 0;
                                var rcv = context.tblReceipt.FirstOrDefault(m => m.ReceiptId == rcvid);
                                tblCorpusLog clog = new tblCorpusLog();
                                clog.ProjectId = rcv.ProjectId;
                                clog.RefId = id;
                                clog.RefNo = billQuery.OverheadsPostingNumber;
                                clog.Amount = expensedet[i].CorpusAmount;
                                clog.TransactionDate = DateTime.Now;
                                clog.TransactionType = "Credit";
                                clog.Pmt_f = true;
                                clog.TypeCode = "OHP";
                                clog.Status = "Active";
                                context.tblCorpusLog.Add(clog);
                                context.SaveChanges();
                            }
                            if (!coreAccountService.getOtherReceiptIUdetails(id, loggedInUser))
                            {
                                return false;
                            }
                            else
                            {

                                for (int i = 0; i < ohdetails.Count(); i++)
                                {
                                    var ohreceiptid = ohdetails[i].ReceiptId;
                                    decimal? ohvalue = ohdetails[i].TotalOverheadsAmount;
                                    decimal? rcvoriginalohvalue = ohdetails[i].ReceiptOriginalOverheadsValue;
                                    var rcvohbreakquery = (from o in context.tblReceiptOverheadBreakup
                                                           where o.ReceiptId == ohreceiptid
                                                           select o).ToList();
                                    var receipt = (from o in context.tblReceipt
                                                   where o.ReceiptId == ohreceiptid
                                                   select o).FirstOrDefault();
                                    decimal? pcfamount = ohdetails[i].PCFAmount;
                                    decimal? rmfamount = ohdetails[i].RMFAmount;
                                    decimal? icsrohamount = ohdetails[i].ICSROHAmount;
                                    decimal? corpusamount = ohdetails[i].CorpousAmount;
                                    decimal? ddfamount = ohdetails[i].DDFAmount;
                                    decimal? staffwelfareamount = ohdetails[i].StaffWelfareAmount;

                                    for (int j = 0; j < rcvohbreakquery.Count(); j++)
                                    {
                                        if (rcvohbreakquery[j].ReceiptId == ohreceiptid && rcvohbreakquery[j].ReceiptOverheadTypeId == 1)
                                        {
                                            rcvohbreakquery[j].ReceiptOverheadAmount = corpusamount;
                                            rcvohbreakquery[j].IsPosted_f = true;
                                            rcvohbreakquery[j].UpdtTS = DateTime.Now;
                                            rcvohbreakquery[j].UpdtUserId = loggedInUser;
                                            context.SaveChanges();
                                        }
                                        if (rcvohbreakquery[j].ReceiptId == ohreceiptid && rcvohbreakquery[j].ReceiptOverheadTypeId == 2)
                                        {
                                            rcvohbreakquery[j].ReceiptOverheadAmount = rmfamount;
                                            rcvohbreakquery[j].IsPosted_f = true;
                                            rcvohbreakquery[j].UpdtTS = DateTime.Now;
                                            rcvohbreakquery[j].UpdtUserId = loggedInUser;
                                            context.SaveChanges();
                                        }
                                        if (rcvohbreakquery[j].ReceiptId == ohreceiptid && rcvohbreakquery[j].ReceiptOverheadTypeId == 3)
                                        {
                                            rcvohbreakquery[j].ReceiptOverheadAmount = icsrohamount;
                                            rcvohbreakquery[j].IsPosted_f = true;
                                            rcvohbreakquery[j].UpdtTS = DateTime.Now;
                                            rcvohbreakquery[j].UpdtUserId = loggedInUser;
                                            context.SaveChanges();
                                        }
                                        if (rcvohbreakquery[j].ReceiptId == ohreceiptid && rcvohbreakquery[j].ReceiptOverheadTypeId == 4)
                                        {
                                            rcvohbreakquery[j].ReceiptOverheadAmount = ddfamount;
                                            rcvohbreakquery[j].IsPosted_f = true;
                                            rcvohbreakquery[j].UpdtTS = DateTime.Now;
                                            rcvohbreakquery[j].UpdtUserId = loggedInUser;
                                            context.SaveChanges();
                                        }
                                        if (rcvohbreakquery[j].ReceiptId == ohreceiptid && rcvohbreakquery[j].ReceiptOverheadTypeId == 5)
                                        {
                                            rcvohbreakquery[j].ReceiptOverheadAmount = staffwelfareamount;
                                            rcvohbreakquery[j].IsPosted_f = true;
                                            rcvohbreakquery[j].UpdtTS = DateTime.Now;
                                            rcvohbreakquery[j].UpdtUserId = loggedInUser;
                                            context.SaveChanges();
                                        }
                                        if (rcvohbreakquery[j].ReceiptId == ohreceiptid && rcvohbreakquery[j].ReceiptOverheadTypeId == 6)
                                        {
                                            rcvohbreakquery[j].ReceiptOverheadAmount = pcfamount;
                                            rcvohbreakquery[j].IsPosted_f = true;
                                            rcvohbreakquery[j].UpdtTS = DateTime.Now;
                                            rcvohbreakquery[j].UpdtUserId = loggedInUser;
                                            context.SaveChanges();
                                        }
                                        receipt.ReceiptOverheadValue = ohvalue;
                                    }

                                }
                                billQuery.Status = "Completed";
                                billQuery.UPTD_By = loggedInUser;
                                billQuery.UPTD_TS = DateTime.Now;
                                context.SaveChanges();
                                return true;
                            }
                        }
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(this, ex);
                return false;
            }
        }
    }
}
