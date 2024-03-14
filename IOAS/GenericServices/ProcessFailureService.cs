using IOAS.DataModel;
using IOAS.Infrastructure;
using System;
using System.Linq;

namespace IOAS.GenericServices
{
    public class ProcessFailureService
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
        public bool TADWFInitFailure(int travelBillId, int loggedInUser)
        {
            try
            {
              //  lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == travelBillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "TAD");
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool BillWFInitFailure(int billId, int loggedInUser)
        {
            try
            {
              //  lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblBillEntry.FirstOrDefault(m => m.BillId == billId && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.BillCommitmentBalanceUpdate(billId, true, false, loggedInUser, query.TransactionTypeCode);
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool TSTWFInitFailure(int travelBillId, int loggedInUser)
        {
            try
            {
                //lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == travelBillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "TST");
                        if (query != null)
                        {
                            var status = coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, true, false, loggedInUser, "TST");
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DTVWFInitFailure(int travelBillId, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == travelBillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "DTV");
                        if (query != null)
                        {
                            if (!coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, true, false, loggedInUser, "DTV"))
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
              //  }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SalaryInitFailure(int PaymentHeadId, int userId)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblSalaryPaymentHead.FirstOrDefault(m => m.PaymentHeadId == PaymentHeadId && m.Status == "Approval Pending");
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UpdatedBy = userId;
                            query.UpdatedAt = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ProjectWFInitFailure(int projectId, int loggedInUser)
        {
            try
            {
                //lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblProject.FirstOrDefault(m => m.ProjectId == projectId && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            tblProjectStatusLog status = new tblProjectStatusLog();
                            status.FromStatus = "Submit for approval";
                            status.ToStatus = "Rejected";
                            status.ProjectId = projectId;
                            status.UpdtdUserId = loggedInUser;
                            status.UpdtdTS = DateTime.Now;
                            context.tblProjectStatusLog.Add(status);
                            context.SaveChanges();

                            query.Status = "Rejected";
                            query.UpdatedUserId = loggedInUser;
                            query.UpdatedTS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                //}
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool InternalProjectWFInitFailure(int projectId, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblProject.FirstOrDefault(m => m.ProjectId == projectId && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            tblProjectStatusLog status = new tblProjectStatusLog();
                            status.FromStatus = "Submit for approval";
                            status.ToStatus = "Rejected";
                            status.ProjectId = projectId;
                            status.UpdtdUserId = loggedInUser;
                            status.UpdtdTS = DateTime.Now;
                            context.tblProjectStatusLog.Add(status);
                            context.SaveChanges();

                            query.Status = "Rejected";
                            query.UpdatedUserId = loggedInUser;
                            query.UpdatedTS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ProjectEnhancementWFInitFailure(int projectEnhancementId, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               //{
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblProjectEnhancement.FirstOrDefault(m => m.ProjectEnhancementId == projectEnhancementId && m.Status == "Submit for approval");
                        if (query != null)
                        {

                            query.Status = "Rejected";
                            query.LastUpdtUserId = loggedInUser;
                            query.LastUpdtTS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ProposalWFInitFailure(int proposalId, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
              //  {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblProposal.FirstOrDefault(m => m.ProposalId == proposalId && m.Status == "Submit for approval");
                        if (query != null)
                        {

                            query.Status = "Rejected";
                            query.Updt_Userid = loggedInUser;
                            query.Updt_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool GeneralVoucherWFInitFailure(int id, int loggedInUser)
        {
            try
            {
             //   lock (lockObj)
              //  {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblGeneralVoucher.FirstOrDefault(m => m.GeneralVoucherId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.GVRCommitmentBalanceUpdate(id, true, false, loggedInUser, query.TransactionTypeCode);
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
              //  }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool PFTWFInitFailure(int id, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblProjectTransfer.FirstOrDefault(m => m.ProjectTransferId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ProjectTransferWFInitFailure(int id, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
              //  {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblProjectDirectTransfer.FirstOrDefault(m => m.ProjectTransferId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.ProjectTransferCommitmentBalanceUpdate(id, true, false, loggedInUser, query.TransactionTypeCode);
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool HonororiumWFInitFailure(int id, int loggedInUser)
        {
            try
            {
                //lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblHonororium.FirstOrDefault(m => m.HonororiumId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.HonororiumCommitmentBalanceUpdate(id, true, false, loggedInUser, "HON");
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
             //   }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ClearancePaymentWFInitFailure(int id, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblClearancePaymentEntry.FirstOrDefault(m => m.ClearancePaymentId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.CLPCommitmentBalanceUpdate(id, true, false, loggedInUser, query.TransactionTypeCode);
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool OtherReceiptInitFailure(int id, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblReceipt.FirstOrDefault(m => m.ReceiptId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UpdtUserId = loggedInUser;
                            query.UpdtTS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
              //  }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool AdhocPaymentWFInitFailure(int id, int loggedInUser)
        {
            try
            {
              //  lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblAdhocPayment.FirstOrDefault(m => m.AdhocPaymentId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.AdhocPayCommitmentBalanceUpdate(id, true, false, loggedInUser, query.TransactionTypeCode);
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool VendorWFInitFailure(int id, int loggedInUser)
        {
            try
            {
             //   lock (lockObj)
              //  {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblVendorMaster.FirstOrDefault(m => m.VendorId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UPDT_UserID = loggedInUser;
                            query.UPDT_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
             //   }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool NegativeBalanceWFInitFailure(int id, int loggedInUser)
        {
            try
            {
              //  lock (lockObj)
              //  {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblNegativeBalance.FirstOrDefault(m => m.NegativeBalanceId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ContraWFInitFailure(int id, int loggedInUser)
        {
            try
            {
              //  lock (lockObj)
              //  {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblContra.SingleOrDefault(m => m.ContraId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool FRMWFInitFailure(int id, int loggedInUser)
        {
            try
            {
              //  lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblForeignRemittance.FirstOrDefault(m => m.ForeignRemitId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.ForeignRemittanceCommitmentBalanceUpdate(id, true, false, loggedInUser, query.TransactionTypeCode);
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                //}
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DistributionWFInitFailure(int id, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblDistribution.FirstOrDefault(m => m.DistributionId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.DistributionCommitmentBalanceUpdate(id, true, false, loggedInUser, "DIS");
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
              //  }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
     
        public bool ECDWFInitFailure(int SBICardProjectDetailsId, int loggedInUser)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblSBIPrepaidCardProjectDetails.FirstOrDefault(m => m.SBIECardProjectDetailsId == SBICardProjectDetailsId && m.Status == "Submit for approval" && m.TransactionTypeCode == "ECD");
                    if (query != null)
                    {
                        query.Status = "Rejected";
                        query.UpdtUserId = loggedInUser;
                        query.UpdtTS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ECRWFInitFailure(int SBICardBillBookingId, int loggedInUser)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblSBICardRecoupment.FirstOrDefault(m => m.RecoupmentId == SBICardBillBookingId && m.Status == "Submit for approval" && m.TransactionTypeCode == "ECR");
                    if (query != null)
                    {
                        var status = coreAccountService.SBICardBillCommitmentBalanceUpdate(SBICardBillBookingId, true, false, loggedInUser, query.TransactionTypeCode);
                        if (!status)
                            return false;
                        query.Status = "Rejected";
                        query.UPTD_By = loggedInUser;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ECBRWFInitFailure(int SBICardBillRecoupId, int loggedInUser)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblSBICardBillRecoupment.FirstOrDefault(m => m.SBICardBillRecoupId == SBICardBillRecoupId && m.Status == "Submit for approval" && m.TransactionTypeCode == "ECBR");
                    if (query != null)
                    {
                        query.Status = "Rejected";
                        query.UPTD_By = loggedInUser;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SMIWFInitFailure(int id, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblSummerInternshipStudentDetails.FirstOrDefault(m => m.SummerInternshipStudentId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.SummerInternshipBalanceUpdate(id, true, false, loggedInUser, query.TransactionTypeCode);
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
              //  }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IMPWFInitFailure(int id, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
              //  {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblIMPUserDetails.FirstOrDefault(m => m.IMPUserDetailsId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UpdtUserId = loggedInUser;
                            query.UpdtTS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
              //  }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IMPEWFInitFailure(int id, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblImprestPaymentDetails.FirstOrDefault(m => m.ImprestPaymentDetailsId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UpdtUserId = loggedInUser;
                            query.UpdtTS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
              //  }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IMRWFInitFailure(int ImprestBillBookingId, int loggedInUser)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblImprestRecoupment.FirstOrDefault(m => m.RecoupmentId == ImprestBillBookingId && m.Status == "Submit for approval" && m.TransactionTypeCode == "IMR");
                    if (query != null)
                    {
                        var status = coreAccountService.ImprestRecoupmentBalanceUpdate(ImprestBillBookingId, true, false, loggedInUser, query.TransactionTypeCode);
                        if (!status)
                            return false;
                        query.Status = "Rejected";
                        query.UPTD_By = loggedInUser;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IBRWFInitFailure(int ImprestBillRecoupId, int loggedInUser)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblImprestBillRecoupment.FirstOrDefault(m => m.ImprestBillRecoupId == ImprestBillRecoupId && m.Status == "Submit for approval" && m.TransactionTypeCode == "IBR");
                    if (query != null)
                    {
                        query.Status = "Rejected";
                        query.UPTD_By = loggedInUser;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool PTPWFInitFailure(int Parttimestudentid, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblPartTimePayment.FirstOrDefault(m => m.PartTimePaymentId == Parttimestudentid && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.PartPaymentBalanceUpdate(Parttimestudentid, true, false, loggedInUser, query.TransactionTypeCode);
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool TMPWFInitFailure(int TempAdvanceid, int loggedInUser)
        {
            try
            {
              //  lock (lockObj)
              //  {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblTemporaryAdvance.FirstOrDefault(m => m.TemporaryAdvanceId == TempAdvanceid && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.TempAdvCommitmentBalanceUpdate(TempAdvanceid, true, false, loggedInUser, query.TransactionTypeCode);
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        else
                            return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool LCOWFInitFailure(int LCDraftid, int loggedInUser)
        {
            try
            {
                //lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblLCDraftDetails.FirstOrDefault(m => m.Id == LCDraftid && m.Status == "Establish LC Approval Pending");
                        if (query != null)
                        {
                            var status = coreAccountService.LCOpeningCommitmentBalanceUpdate(LCDraftid, true, false, loggedInUser, query.TransactionTypeCode);
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool LCAWFInitFailure(int LCAmmendmentid, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblLCAmmendment.FirstOrDefault(m => m.Id == LCAmmendmentid && m.Status == "Amendment Approval Pending");
                        if (query != null)
                        {
                            var status = coreAccountService.LCAmmendCommitmentBalanceUpdate(LCAmmendmentid, true, false, loggedInUser, query.TransactionTypeCode);
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
              //  }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool LCRWFInitFailure(int LCRetirmentid, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblLCRetirement.FirstOrDefault(m => m.Id == LCRetirmentid && m.Status == "Retirement Approval Pending");
                        if (query != null)
                        {
                            var status = coreAccountService.LCRetireCommitmentBalanceUpdate(LCRetirmentid, true, false, loggedInUser, query.TransactionTypeCode);
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool HCRWFInitFailure(int id, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblHeadCredit.FirstOrDefault(m => m.HeadCreditId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.HeadCreditCommitmentBalanceUpdate(id, true, false, loggedInUser, query.TransactionTypeCode);
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UpdtUserId = loggedInUser;
                            query.UpdtTS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool PaymentProcessWFInitFailure(int id, int loggedInUser)
        {
            try
            {
              //  lock (lockObj)
              //  {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblBOADraft.FirstOrDefault(m => m.BOADraftId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool FSSWFInitFailure(int id, int loggedInUser)
        {
            try
            {
              //  lock (lockObj)
              //  {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblFellowshipSalary.FirstOrDefault(m => m.FellowshipSalaryId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.FellowShipSalaryCommitmentBalanceUpdate(id, true, false, loggedInUser, "FSS");
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPDT_By = loggedInUser;
                            query.UPDT_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool MDYWFInitFailure(int id, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblManDay.FirstOrDefault(m => m.ManDayId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.ManDayCommitmentBalanceUpdate(id, true, false, loggedInUser, "MDY");
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPDT_By = loggedInUser;
                            query.UPDT_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
              //  }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool AVOWFInitFailure(int id, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblAdminVoucher.FirstOrDefault(m => m.AdminVoucherId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            //var status = coreAccountService.ManDayCommitmentBalanceUpdate(id, true, false, loggedInUser, "MDY");
                            //if (!status)
                            //    return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
              //  }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool OHARWFInitFailure(int id, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblOHReversal.FirstOrDefault(m => m.OHReversalId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.AdditionandReversalOHCommitmentBalanceUpdate(id, true, false, loggedInUser, "OHAR");
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_BY = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
             //   }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool TMSWFInitFailure(int id, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
              //  {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblTempAdvanceSettlement.FirstOrDefault(m => m.TempAdvSettlementId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            var status = coreAccountService.TempAdvSettlementCommitmentBalanceUpdate(id, true, loggedInUser, "TMS");
                            if (!status)
                                return false;
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool JVWFInitFailure(int id, int loggedInUser)
        {
            try
            {
              //  lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblJournal.FirstOrDefault(m => m.JournalId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool TXPWFInitFailure(int id, int loggedInUser)
        {
            try
            {
              //  lock (lockObj)
              //  {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblTDSPayment.FirstOrDefault(m => m.tblTDSPaymentId == id && m.Status == "Submit for approval");
                        if (query != null)
                        {
                          
                            query.Status = "Rejected";
                            query.UPDT_By = loggedInUser;
                            query.UPDT_TS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
               // }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool OverheadsWFInitFailure(int id, int loggedInUser)
        {
            try
            {
               using (var context = new IOASDBEntities())
                {
                    var query = context.tblOverheadsPosting.FirstOrDefault(m => m.OverheadsPostingId == id && m.Status == "Submit for approval");
                    if (query != null)
                    {

                        query.Status = "Rejected";
                        query.UPTD_By = loggedInUser;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IMEWFInitFailure(int id, int loggedInUser)
        {
            try
            {
               // lock (lockObj)
               // {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblImprestPaymentDetails.FirstOrDefault(m => m.ImprestPaymentDetailsId == id && m.Status == "Open");
                        if (query != null)
                        {

                            query.Status = "Rejected";
                            query.UpdtUserId = loggedInUser;
                            query.UpdtTS = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
              //  }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AnnouncementWFInitFailure(int AnnouncementID, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {

                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblRCTAnnouncementMaster.FirstOrDefault(m => m.AnnouncementID == AnnouncementID);
                        if (query != null)
                        {
                            query.Status = 14;
                            query.Upt_User = loggedInUser;
                            query.Upt_TS = DateTime.Now;
                            context.SaveChanges();
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

        public bool CONAPWFInitFailure(int CONAPId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblRCTConsultantAppointment.FirstOrDefault(m => m.ConsultantAppointmentId == CONAPId && m.Status == "Sent for approval");
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UptdUser = loggedInUser;
                            query.UptdTs = DateTime.Now;
                            context.SaveChanges();
                            RequirementService.PostCONStatusLog(CONAPId, "Sent for approval", query.Status, loggedInUser);

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
        public bool STEWFInitFailure(int STEID, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblRCTSTE.FirstOrDefault(m => m.STEID == STEID && m.Status == "Sent for approval");
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UptdUser = loggedInUser;
                            query.UptdTs = DateTime.Now;
                            context.SaveChanges();
                            RequirementService.PostSTEStatusLog(STEID, "Sent for approval", query.Status, loggedInUser);
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

        public bool RecruitCOPWFInitFailure(int OrderId, int loggedInUser)
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
                            query.Status = "Rejected";
                            query.UpdtUser = loggedInUser;
                            query.UpdtTS = DateTime.Now;
                            context.SaveChanges();
                            #region log
                            tblRCTOrderLog log = new tblRCTOrderLog();
                            log.OrderID = OrderId;
                            log.PresentStatus = "Sent for approval";
                            log.NewStatus = "Rejected";
                            log.Crt_By = loggedInUser;
                            log.Crt_TS = DateTime.Now;
                            log.Message = "Rejected by " + Common.GetUserFirstName(loggedInUser);
                            context.tblRCTOrderLog.Add(log);
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

        public bool RecruitExtensionWFInitFailure(int OrderId, int loggedInUser)
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
                            query.Status = "Rejected";
                            query.UpdtUser = loggedInUser;
                            query.UpdtTS = DateTime.Now;
                            context.SaveChanges();
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

        public bool RecruitEnhancementWFInitFailure(int OrderId, int loggedInUser)
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
                            query.Status = "Rejected";
                            query.UpdtUser = loggedInUser;
                            query.UpdtTS = DateTime.Now;
                            context.SaveChanges();
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

        public bool RecruitHRAWFInitFailure(int OrderId, int loggedInUser)
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
                            query.Status = "Rejected";
                            query.UpdtUser = loggedInUser;
                            query.UpdtTS = DateTime.Now;
                            context.SaveChanges();
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

        public bool RecruitSLPWFInitFailure(int OrderId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblOrder.FirstOrDefault(m => m.OrderId == OrderId && (m.Status == "Sent for approval" || m.Status == "Sent for reversal approval"));
                        if (query != null)
                        {
                            var PreStatus = query.Status;
                            query.Status = "Rejected";
                            query.UpdtUser = loggedInUser;
                            query.UpdtTS = DateTime.Now;
                            context.SaveChanges();
                            RequirementService.PostOrderStatusLog(OrderId, PreStatus, query.Status, loggedInUser);
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

        public bool RecruitRelieveWFInitFailure(int OrderId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblOrder.FirstOrDefault(m => m.OrderId == OrderId);
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UpdtUser = loggedInUser;
                            query.UpdtTS = DateTime.Now;
                            context.SaveChanges();
                            RequirementService.PostOrderStatusLog(OrderId, "Relieving initiated", query.Status, loggedInUser);
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

        public bool RecruitMLWFInitFailure(int OrderId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblOrder.FirstOrDefault(m => m.OrderId == OrderId);
                        if (query != null)
                        {
                            var Prestatus = query.Status;
                            query.Status = "Rejected";
                            query.UpdtUser = loggedInUser;
                            query.UpdtTS = DateTime.Now;
                            context.SaveChanges();
                            RequirementService.PostOrderStatusLog(OrderId, Prestatus, query.Status, loggedInUser);
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
        public bool RecruitAmendmentWFInitFailure(int OrderId, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblOrder.FirstOrDefault(m => m.OrderId == OrderId);
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UpdtUser = loggedInUser;
                            query.UpdtTS = DateTime.Now;
                            context.SaveChanges();
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

        public bool OSGWFInitFailure(int OSGID, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblRCTOutsourcing.FirstOrDefault(m => m.OSGID == OSGID && m.Status == "Sent for approval");
                        if (query != null)
                        {
                            query.Status = "Rejected";
                            query.UptdUser = loggedInUser;
                            query.UptdTs = DateTime.Now;
                            context.SaveChanges();
                            RequirementService.PostOSGStatusLog(OSGID, "Sent for approval", query.Status, loggedInUser);
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
        public bool RecruitOTHPDWFInitFailure(int ID, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var otherpayded = context.tblRCTOTHPaymentDeduction.FirstOrDefault(m => m.OTHPayDeductionId == ID && m.Status == "Sent for approval");
                        if (otherpayded != null)
                        {
                            otherpayded.Status = "Rejected";
                            otherpayded.UpdtUser = loggedInUser;
                            otherpayded.UpdtTs = DateTime.Now;
                            context.SaveChanges();
                            #region log
                            tblRCTOTHPaymentDeductionStatuslog log = new tblRCTOTHPaymentDeductionStatuslog();
                            log.OTHPayDeductionId = otherpayded.OTHPayDeductionId;
                            log.PresentStatus = "Sent for approval";
                            log.NewStatus = "Rejected";
                            log.Crt_By = loggedInUser;
                            log.Crt_Ts = DateTime.Now;
                            log.Message = "Sent for Rejected";
                            context.tblRCTOTHPaymentDeductionStatuslog.Add(log);
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

        public bool RecruitOTHPUploadInitFailure(int OthIdMastrid, int loggedInUser)
        {
            try
            {
                lock (lockObj)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var otherpayded = context.tblRCTOTHPaymentDeductionUploadMaster.FirstOrDefault(m => m.OTHUploadMasterId == OthIdMastrid && m.Status == "Sent for approval");
                        if (otherpayded != null)
                        {
                            otherpayded.Status = "Rejected";
                            otherpayded.Uptd_by = loggedInUser;
                            otherpayded.Uptd_Ts = DateTime.Now;
                            context.SaveChanges();

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


        private static readonly Object ReceiptBUlockObj = new Object();

        public bool ReceiptBUInitFailure(int id, int loggedInUser)
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
                            query.Status = "Rejected";
                            query.UPTD_By = loggedInUser;
                            query.UPTD_Ts = DateTime.Now;
                            context.SaveChanges();
                            return true;
                        }
                     //   coreAccountService.RollBackLastApproved(id, 217);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
             //   coreAccountService.RollBackLastApproved(id, 217);
                return false;
            }
        }

    }
}