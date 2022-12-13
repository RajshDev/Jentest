using IOAS.DataModel;
using IOAS.Infrastructure;
using IOAS.Models;
using IOASExternal.DataModel;
using System;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Web;
using DataAccessLayer;
using System.Reflection;
using System.Configuration;

namespace IOAS.GenericServices
{
    public class ReportService
    {
        ProjectService ProjSer = new ProjectService();
        CoreAccountsService coreAccountService = new CoreAccountsService();
        string BillMode = ConfigurationManager.AppSettings["BillMode"].ToString();
        string BillMonth = ConfigurationManager.AppSettings["BillMonth"].ToString();
        string BillDate = ConfigurationManager.AppSettings["BillDate"].ToString();
        #region General Voucher Report
        public TravelBillReportModel GetGeneralVoucherBillReport(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblGeneralVoucher.Where(m => m.GeneralVoucherId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    var query = (from e in context.tblGeneralVoucherCommitmentDetail
                                 join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                 join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                 join h in context.tblProject on g.ProjectId equals h.ProjectId
                                 join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                 where e.GeneralVoucherId == Id && e.PaymentAmount > 0
                                 select new
                                 {
                                     h.ProjectType,
                                     h.ProjectId,
                                     h.SponProjectCategory
                                 }).ToList();
                    for (int i = 0; i < query.Count; i++)
                    {
                        if (query[i].ProjectType == 2)
                        {
                            model.BillHeading = "Consultancy";
                        }
                        if (query[i].ProjectType == 1)
                        {
                            var proid = query[i].ProjectId;
                            var head = context.tblProject.Where(m => m.ProjectId == proid).Select(m => m.SponProjectCategory).FirstOrDefault();
                            if (head == "1")
                            {
                                model.BillHeading = "Sponsored-PFMS";
                                break;
                            }
                            if (head == "2")
                            {
                                model.BillHeading = "Sponsored-NON-PFMS";

                            }
                        }
                        if (query[i].ProjectId == 2067)
                        {
                            model.BillHeading = "ICSR Over Head";
                        }
                    }
                    model.BankAccount = Common.GetBankName(Convert.ToInt32(Qry.BankHeadId));
                    model.BillId = Id;
                    model.TotalBillValue = Convert.ToDecimal(Qry.TotalAmount);
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.VoucherNumber;
                    model.BillType = "General Voucher";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    List<PayableListModel> PayList = new List<PayableListModel>();
                    var Pay = (from e in context.tblGeneralVoucher
                               join f in context.tblGeneralVoucherPaymentBreakUpDetail on e.GeneralVoucherId equals f.GeneralVoucherId
                               where e.GeneralVoucherId == Id
                               select new
                               {
                                   f.PaymentAmount,
                                   f.Name,
                                   f.CategoryId,
                                   f.UserId
                               }).ToList();
                    for (int i = 0; i < Pay.Count; i++)
                    {
                        string typecode = "GVR"; int refid = Id; string type = ""; string name = "";
                        var nam = "";
                        var Bank = "";
                        var Ifsc = "";
                        var Branch = ""; var Accno = "";
                        int catid = Convert.ToInt32(Pay[i].UserId);
                        name = Pay[i].Name;
                        if (Pay[i].CategoryId == 1)
                        {
                            nam = Common.GetVWCombineStaffName(catid, "Professor") + "-Professor";
                            Bank = Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").Bank == null ? (Common.GetBankDetailsFromPayment(typecode, refid, catid, "Professor", name).Bank) : Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").Bank;
                            Ifsc = Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").IFSC == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Professor", name).IFSC : Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").IFSC;
                            Branch = Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").Branch == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Professor", name).Branch : Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").Branch;
                            Accno = Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").AccNo == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Professor", name).AccNo : Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").AccNo;
                        }
                        else if (Pay[i].CategoryId == 2)
                        {
                            string[] StudentRoll = Pay[i].Name.Split(new char[] { '-' });
                            var Roll = StudentRoll[0];
                            nam = Pay[i].Name + "-Student";
                            Bank = Common.GetBankDetailsForStudent(Roll, typecode, refid).Bank;
                            Ifsc = Common.GetBankDetailsForStudent(Roll, typecode, refid).IFSC;
                            Branch = Common.GetBankDetailsForStudent(Roll, typecode, refid).Branch;
                            Accno = Common.GetBankDetailsForStudent(Roll, typecode, refid).AccNo;
                        }
                        else if (Pay[i].CategoryId == 4)
                        {
                            nam = Pay[i].Name + "-Travel Agency";
                            Bank = Common.GetBankDetailsForTarvelAgency(catid).Bank;
                            Ifsc = Common.GetBankDetailsForTarvelAgency(catid).IFSC;
                            Branch = Common.GetBankDetailsForTarvelAgency(catid).Branch;
                            Accno = Common.GetBankDetailsForTarvelAgency(catid).AccNo;
                        }
                        else if (Pay[i].CategoryId == 9)
                        {
                            nam = Pay[i].Name + "-Vendor";
                            Bank = Common.GetBankDetailsForVendor(catid).Bank;
                            Ifsc = Common.GetBankDetailsForVendor(catid).IFSC;
                            Branch = Common.GetBankDetailsForVendor(catid).Branch;
                            Accno = Common.GetBankDetailsForVendor(catid).AccNo;
                        }
                        else if (Pay[i].CategoryId == 8)
                        {
                            nam = Pay[i].Name + "-Visitor";
                            Bank = Common.GetBankDetailsFromPayment(typecode, refid, catid, "Others", name).Bank;
                            Ifsc = Common.GetBankDetailsFromPayment(typecode, refid, catid, "Others", name).IFSC;
                            Branch = Common.GetBankDetailsFromPayment(typecode, refid, catid, "Others", name).Branch;
                            Accno = Common.GetBankDetailsFromPayment(typecode, refid, catid, "Others", name).AccNo;
                        }
                        else if (Pay[i].CategoryId == 5)
                        {
                            nam = Common.GetVWCombineStaffName(catid, "Project Staff") + "-T&M Staff";
                            Bank = Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").Bank == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "T&M Staff", name).Bank : Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").Bank;
                            Ifsc = Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").IFSC == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "T&M Staff", name).IFSC : Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").IFSC;
                            Branch = Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").Branch == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "T&M Staff", name).Branch : Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").Branch;
                            Accno = Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").AccNo == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "T&M Staff", name).AccNo : Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").AccNo;
                        }
                        else if (Pay[i].CategoryId == 6)
                        {
                            nam = Common.GetVWCombineStaffName(catid, "Staff") + "-Institute Staff";
                            Bank = Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").Bank == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Institute Staff", name).Bank : Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").Bank;
                            Ifsc = Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").IFSC == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Institute Staff", name).IFSC : Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").IFSC;
                            Branch = Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").Branch == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Institute Staff", name).Branch : Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").Branch;
                            Accno = Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").AccNo == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Institute Staff", name).AccNo : Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").AccNo;

                        }
                        else if (Pay[i].CategoryId == 7)
                        {
                            nam = Common.GetVWCombineStaffName(catid, "AdhocStaff") + "-AdhocStaff";
                            Bank = Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").Bank == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Adhoc Staff", name).Bank : Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").Bank;
                            Ifsc = Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").IFSC == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Adhoc Staff", name).IFSC : Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").IFSC;
                            Branch = Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").Branch == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Adhoc Staff", name).Branch : Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").Branch;
                            Accno = Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").AccNo == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Adhoc Staff", name).AccNo : Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").AccNo;
                        }

                        PayList.Add(new PayableListModel()
                        {
                            Name = nam,
                            Bank = Bank,
                            Amount = Convert.ToDecimal(Pay[i].PaymentAmount),
                            AccNo = Accno,
                            IFSC = Ifsc
                        });
                    }
                    model.Payable = PayList;
                    decimal? PayableAmt = 0;
                    if (model.Payable != null)
                    {
                        PayableAmt = model.Payable.Select(m => m.Amount).Sum();
                    }
                    model.Comm = (from e in context.tblGeneralVoucherCommitmentDetail
                                  join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.GeneralVoucherId == Id && e.PaymentAmount > 0
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.PaymentAmount,
                                      h.ProjectId
                                  }).AsEnumerable()
                             .Select((x) => new CommitListModel()
                             {
                                 Number = x.CommitmentNumber,
                                 ProjNo = x.ProjectNumber,
                                 ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                                 NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                                 Date = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                                 StartDate = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectStartDate(x.ProjectId)),
                                 SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                                 Head = x.HeadName,
                                 Value = Convert.ToDecimal(x.PaymentAmount)
                             }).ToList();
                    int[] id = { 44, 45, 46 };

                    model.TDSGST = (from e in context.tblGeneralVoucherExpenseDetail
                                    join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                    where e.GeneralVoucherId == Id && (id.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                    select new
                                    {
                                        e.Amount,
                                        f.AccountHead
                                    }).AsEnumerable()
                                          .Select((x) => new TDSGSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();

                    int[] IT = context.tblAccountHead.Where(m => m.AccountGroupId == 15 && m.AccountHeadId != 135).Select(m => m.AccountHeadId).ToArray();

                    model.TDSIT = (from e in context.tblGeneralVoucherExpenseDetail
                                   join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                   where e.GeneralVoucherId == Id
                                   //&& (IT.Contains(e.AccountHeadId ?? 0)) 
                                   && e.Amount > 0
                                   && e.AccountHeadId != 135 && e.AccountGroupId == 15
                                   select new
                                   {
                                       e.Amount,
                                       f.AccountHead
                                   }).AsEnumerable()
                                          .Select((x) => new TDSITListForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();
                    int[] GST = { 32, 33, 34, 36, 37, 38 };

                    model.GST = (from e in context.tblGeneralVoucherDeductionDetail
                                 join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                 where e.GeneralVoucherId == Id && (GST.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                 select new
                                 {
                                     e.Amount,
                                     f.AccountHead
                                 }).AsEnumerable()
                    .Select((x) => new GSTForTravelModel()
                    {
                        Head = x.AccountHead,
                        Value = Convert.ToDecimal(x.Amount)
                    }).ToList();
                    model.PayableAmount = Qry.BankTxAmount ?? 0;
                    model.TotalAmount = model.PayableAmount;
                    model.Rupees = CoreAccountsService.words(model.PayableAmount);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.Remarks = Qry.Remarks;
                    model.StudentName = Common.GetCodeControlName(Convert.ToInt32(Qry.CategoryId), "PaymentCategory");
                    model.TravelerName = Common.GetUserNameBasedonId(Convert.ToInt32(Qry.CRTD_By));
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion
        #region Travel Bill Report
        public TravelBillReportModel GetTravelBillReportDetails(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {

                using (var context = new IOASDBEntities())
                {
                    var qry = context.tblTravelBill.Where(m => m.TravelBillId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", qry.CRTD_TS);
                    var query = (from e in context.tblTravelBillCommitmentDetail
                                 join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                 join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                 join h in context.tblProject on g.ProjectId equals h.ProjectId
                                 join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                 where e.TravelBillId == Id
                                 select new
                                 {
                                     h.ProjectType,
                                     h.ProjectId,
                                     h.SponProjectCategory
                                 }).ToList();
                    if (query.Count > 0)
                    {


                        for (int i = 0; i < query.Count; i++)
                        {
                            if (query[i].ProjectType == 2)
                            {
                                model.BillHeading = "Consultancy";
                            }
                            if (query[i].ProjectType == 1)
                            {
                                var proid = query[i].ProjectId;
                                var head = context.tblProject.Where(m => m.ProjectId == proid).Select(m => m.SponProjectCategory).FirstOrDefault();
                                if (head == "1")
                                {
                                    model.BillHeading = "Sponsored-PFMS";
                                    break;
                                }
                                if (head == "2")
                                {
                                    model.BillHeading = "Sponsored-NON-PFMS";

                                }
                            }
                            if (query[i].ProjectId == 2067)
                            {
                                model.BillHeading = "ICSR Over Head";
                            }
                        }
                    }
                    int[] headid = { 38, 61 };
                    var BankHeadId = (from e in
                       context.tblTravelBillExpenseDetail
                                      where (headid.Contains(e.AccountGroupId ?? 0) && e.TravelBillId == Id
                                      && (e.TransactionType == "Credit" || e.TransactionType == "Debit"))
                                      select new { e.AccountHeadId }).FirstOrDefault();
                    if (BankHeadId != null)
                        model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == (BankHeadId.AccountHeadId ?? 0)).Select(m => m.AccountHead).FirstOrDefault();

                    model.BillId = Id;
                    // model.BankAccount = Common.GetBankName(Convert.ToInt32(qry.BankHead));
                    if (qry.TransactionTypeCode == "TAD")
                    {
                        model.TotalBillValue = Convert.ToDecimal(qry.AdvanceValue);
                    }
                    else if (qry.TransactionTypeCode == "DTV")
                    {
                        model.TotalBillValue = Convert.ToDecimal(qry.OverallExpense);
                    }
                    else if (qry.TransactionTypeCode == "TST")
                    {
                        if (qry.PaymentValue > 0)
                            model.TotalBillValue = Convert.ToDecimal(qry.PaymentValue);
                        else if (qry.OverallExpense > 0)
                            model.TotalBillValue = Convert.ToDecimal(qry.OverallExpense);
                    }
                    model.BillMonth = String.Format("{0:MMM yyyy}", qry.CRTD_TS);
                    //  model.InvoiceDate = String.Format("{0:dd-MMMM-yyyy}", qry.InvoiceDate);
                    model.BillNumber = qry.BillNumber;
                    model.Remarks = qry.Remarks;
                    model.BillType = Common.gettransactioncode(qry.TransactionTypeCode);
                    // model.PODate = String.Format("{0:dd-MMMM-yyyy}", qry.PODate);
                    // model.PONumber = qry.PONumber;
                    // model.ModeOfSupply = Common.GetCodeControlName(Convert.ToInt32(qry.PaymentType), "SettlementType");
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", qry.CRTD_TS);
                    var InvoiceQry = (from a in context.tblTravelInvoiceDetail
                                      where a.TravelBillId == Id
                                      select new { a.InvoiceDate, a.InvoiceNumber }).ToList();
                    List<InvoiceListModel> InvoiceList = new List<InvoiceListModel>();
                    if (InvoiceQry.Count > 0)
                    {
                        for (int i = 0; i < InvoiceQry.Count; i++)
                        {
                            InvoiceList.Add(new InvoiceListModel
                            {
                                InvoiceNo = InvoiceQry[i].InvoiceNumber,
                                InvoiceDate = String.Format("{0:dd-MMM-yyyy}", InvoiceQry[i].InvoiceDate)
                            });
                        }
                    }
                    model.InvoiceList = InvoiceList;
                    List<PayableListModel> PayList = new List<PayableListModel>();
                    var Pay = (from e in context.tblTravelBill
                               join f in context.tblTravelPaymentBreakUpDetail on e.TravelBillId equals f.TravelBillId
                               // from g in context.tblPayment.Where(m => m.TransactionTypeCode == e.TransactionTypeCode && m.ReferenceId == e.TravelBillId)
                               //join h in context.tblPayment on e.TravelBillId equals h.ReferenceId
                               //  where e.TransactionTypeCode == h.TransactionTypeCode
                               // join i in context.tblPaymentPayee on h.PaymentId equals i.PaymentId
                               //join j in context.tblClearanceAgentMaster on f.TravelBillId equals j.ClearanceAgentId where e.TransactionTypeCode == h.TransactionTypeCode && f.CategoryId==3
                               where e.TravelBillId == Id
                               select new
                               {
                                   f.PaymentAmount,
                                   f.Name,
                                   f.CategoryId,
                                   f.UserId
                                   // i.AccountNumber,
                                   // i.IFSC
                               }).ToList();
                    if (Pay.Count > 0)
                    {
                        for (int i = 0; i < Pay.Count; i++)
                        {
                            string typecode = qry.TransactionTypeCode; int refid = Id; string type = ""; string name = "";
                            string GSTIN = ""; string PAN = "";
                            var nam = "";
                            var Bank = "";
                            var Ifsc = "";
                            var Branch = ""; var Accno = ""; name = Pay[i].Name;
                            int catid = Convert.ToInt32(Pay[i].UserId);
                            if (Pay[i].CategoryId == 1)
                            {
                                nam = Common.GetVWCombineStaffName(catid, "Professor") + "-Professor";
                                Bank = Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").Bank == null ? (Common.GetBankDetailsFromPayment(typecode, refid, catid, "PI / CoPI", name).Bank) : Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").Bank;
                                Ifsc = Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").IFSC == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "PI / CoPI", name).IFSC : Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").IFSC;
                                Branch = Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").Branch == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "PI / CoPI", name).Branch : Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").Branch;
                                Accno = Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").AccNo == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "PI / CoPI", name).AccNo : Common.GetBankDetailsFromStaffBankAccount(catid, "Professor").AccNo;
                            }
                            if (Pay[i].CategoryId == 2)
                            {
                                string[] StudentRoll = Pay[i].Name.Split(new char[] { '-' });
                                var Roll = StudentRoll[0];
                                nam = Pay[i].Name + "-Student";
                                Bank = Common.GetBankDetailsForStudent(Roll, typecode, refid).Bank;
                                Ifsc = Common.GetBankDetailsForStudent(Roll, typecode, refid).IFSC;
                                Branch = Common.GetBankDetailsForStudent(Roll, typecode, refid).Branch;
                                Accno = Common.GetBankDetailsForStudent(Roll, typecode, refid).AccNo;
                            }
                            if (Pay[i].CategoryId == 4)
                            {
                                nam = Pay[i].Name + "-Travel Agency";
                                Bank = Common.GetBankDetailsForTarvelAgency(catid).Bank;
                                Ifsc = Common.GetBankDetailsForTarvelAgency(catid).IFSC;
                                Branch = Common.GetBankDetailsForTarvelAgency(catid).Branch;
                                Accno = Common.GetBankDetailsForTarvelAgency(catid).AccNo;
                                PAN = Common.GetBankDetailsForTarvelAgency(catid).PAN;
                                GSTIN = Common.GetBankDetailsForTarvelAgency(catid).GSTIN;
                            }
                            if (Pay[i].CategoryId == 8)
                            {
                                nam = Pay[i].Name + "-Visitor";
                                Bank = Common.GetBankDetailsFromPayment(typecode, refid, catid, "Others", name).Bank;
                                Ifsc = Common.GetBankDetailsFromPayment(typecode, refid, catid, "Others", name).IFSC;
                                Branch = Common.GetBankDetailsFromPayment(typecode, refid, catid, "Others", name).Branch;
                                Accno = Common.GetBankDetailsFromPayment(typecode, refid, catid, "Others", name).AccNo;
                            }
                            if (Pay[i].CategoryId == 5)
                            {

                                nam = Common.GetVWCombineStaffName(catid, "Project Staff") + "-T&M Staff";
                                Bank = Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").Bank == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "T&M Staff", name).Bank : Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").Bank;
                                Ifsc = Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").IFSC == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "T&M Staff", name).IFSC : Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").IFSC;
                                Branch = Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").Branch == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "T&M Staff", name).Branch : Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").Branch;
                                Accno = Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").AccNo == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "T&M Staff", name).AccNo : Common.GetBankDetailsFromStaffBankAccount(catid, "ProjectStaff").AccNo;

                            }
                            if (Pay[i].CategoryId == 6)
                            {
                                nam = Common.GetVWCombineStaffName(catid, "Staff") + "-Institute Staff";
                                Bank = Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").Bank == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Institute Staff", name).Bank : Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").Bank;
                                Ifsc = Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").IFSC == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Institute Staff", name).IFSC : Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").IFSC;
                                Branch = Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").Branch == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Institute Staff", name).Branch : Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").Branch;
                                Accno = Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").AccNo == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Institute Staff", name).AccNo : Common.GetBankDetailsFromStaffBankAccount(catid, "Staff").AccNo;
                            }
                            if (Pay[i].CategoryId == 7)
                            {
                                nam = Common.GetVWCombineStaffName(catid, "AdhocStaff") + "-AdhocStaff";
                                Bank = Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").Bank == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Adhoc Staff", name).Bank : Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").Bank;
                                Ifsc = Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").IFSC == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Adhoc Staff", name).IFSC : Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").IFSC;
                                Branch = Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").Branch == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Adhoc Staff", name).Branch : Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").Branch;
                                Accno = Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").AccNo == null ? Common.GetBankDetailsFromPayment(typecode, refid, catid, "Adhoc Staff", name).AccNo : Common.GetBankDetailsFromStaffBankAccount(catid, "AdhocStaff").AccNo;
                            }
                            PayList.Add(new PayableListModel()
                            {
                                Name = nam,
                                GSTIN = GSTIN,
                                PAN = PAN,
                                Bank = Bank,
                                Amount = Convert.ToDecimal(Pay[i].PaymentAmount),
                                AccNo = Accno,
                                IFSC = Ifsc
                            });
                        }
                    }
                    model.Payable = PayList;
                    decimal? PayableAmt = 0;
                    if (model.Payable != null)
                    {
                        PayableAmt = model.Payable.Select(m => m.Amount).Sum();
                    }
                    model.Comm = (from e in context.tblTravelBillCommitmentDetail
                                  join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.TravelBillId == Id && e.PaymentAmount > 0
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.PaymentAmount,
                                      h.ProjectId
                                  }).AsEnumerable()
                                 .Select((x) => new CommitListModel()
                                 {
                                     Number = x.CommitmentNumber,
                                     ProjNo = x.ProjectNumber,
                                     ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                                     NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                                     Date = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                                     StartDate = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectStartDate(x.ProjectId)),
                                     SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                                     Head = x.HeadName,
                                     Value = Convert.ToDecimal(x.PaymentAmount)
                                 }).ToList();
                    int[] GST = { 32, 33, 34, 36, 37, 38 };

                    model.GST = (from e in context.tblTravelBillDeductionDetail
                                 join d in context.tblDeductionHead on e.DeductionHeadId equals d.DeductionHeadId
                                 join f in context.tblAccountHead on d.AccountHeadId equals f.AccountHeadId
                                 where e.TravelBillId == Id && (GST.Contains(d.AccountHeadId ?? 0)) && e.Amount > 0
                                 select new
                                 {
                                     e.Amount,
                                     f.AccountHead
                                 }).AsEnumerable()
                                          .Select((x) => new GSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();
                    List<TravelerVistListModel> TravelList = new List<TravelerVistListModel>();

                    var SetQry = (from a in context.tblTravelBill
                                  join b in context.tblTravelBillDetail on a.TravelBillId equals b.TravelBillId
                                  join c in context.tblTravelBillTravellerDetail on a.TravelBillId equals c.TravelBillDetailId
                                  where a.TravelBillId == Id && c.TransactionTypeCode == "TAD"
                                  select new
                                  {
                                      c.TravellerId,
                                      b.CountryId,
                                      b.Place,
                                      b.TravelFromDate,
                                      b.TravelToDate,
                                      c.CategoryId,
                                      c.TravellerName,
                                  }).ToArray();
                    var AdvQry = (from a in context.tblTravelBill
                                  join b in context.tblTravelBillDetail on a.TravelBillId equals b.TravelBillId
                                  join c in context.tblTravelBillTravellerDetail on b.TravelBillDetailId equals c.TravelBillDetailId
                                  where a.TravelBillId == Id && c.TransactionTypeCode != "TAD"
                                  select new
                                  {
                                      c.TravellerId,
                                      b.CountryId,
                                      b.Place,
                                      b.TravelFromDate,
                                      b.TravelToDate,
                                      c.CategoryId,
                                      c.TravellerName,
                                  }).ToArray();
                    var TravelQry = SetQry.Union(AdvQry).ToList();
                    if (TravelQry.Count > 0)
                    {

                        for (int i = 0; i < TravelQry.Count(); i++)
                        {
                            var nam = "";
                            int catid = 0;
                            catid = TravelQry[i].TravellerId ?? 0;
                            if (TravelQry[i].CategoryId == 1)
                            {

                                nam = Common.GetVWCombineStaffName(catid, "Professor");
                            }
                            if (TravelQry[i].CategoryId == 2 || TravelQry[i].CategoryId == 3)
                            {

                                nam = TravelQry[i].TravellerName;
                            }
                            if (TravelQry[i].CategoryId == 4)
                            {

                                nam = Common.GetVWCombineStaffName(catid, "Project Staff");
                            }
                            if (TravelQry[i].CategoryId == 5)
                            {

                                nam = Common.GetVWCombineStaffName(catid, "Staff");
                            }
                            if (TravelQry[i].CategoryId == 6)
                            {

                                nam = Common.GetVWCombineStaffName(catid, "AdhocStaff");
                            }
                            TravelList.Add(new TravelerVistListModel()
                            {
                                TravlerName = nam,
                                Duration = String.Format("{0:dd-MMMM-yyyy}", TravelQry[i].TravelFromDate) + " to " + String.Format("{0:dd-MMMM-yyyy}", TravelQry[i].TravelToDate),
                                PlaceOfvisit = TravelQry[i].Place.ToUpper() + Common.GetCountryName(Convert.ToInt32(TravelQry[i].CountryId))

                            });
                        }
                    }
                    model.TravlerVisit = TravelList;
                    int[] id = { 44, 45, 46 };

                    model.TDSGST = (from e in context.tblTravelBillExpenseDetail
                                    join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                    where e.TravelBillId == Id && (id.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                    select new
                                    {
                                        e.Amount,
                                        f.AccountHead
                                    }).AsEnumerable()
                                          .Select((x) => new TDSGSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();


                    int[] IT = context.tblAccountHead.Where(m => m.AccountGroupId == 15 && m.AccountHeadId != 135).Select(m => m.AccountHeadId).ToArray();

                    model.TDSIT = (from e in context.tblTravelBillExpenseDetail
                                   join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                   where e.TravelBillId == Id && (IT.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                   select new
                                   {
                                       e.Amount,
                                       f.AccountHead
                                   }).AsEnumerable()
                                          .Select((x) => new TDSITListForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();
                    model.PayableAmount = PayableAmt ?? 0;
                    model.TotalAmount = model.PayableAmount;
                    model.Rupees = CoreAccountsService.words(model.PayableAmount);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.TravelerName = Common.GetVWCombineStaffName(Convert.ToInt32(qry.PI), "Professor");
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;

                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region Honorarium Bill Report
        public TravelBillReportModel GetHonorariumBillReport(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblHonororium.Where(m => m.HonororiumId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    var query = (from e in context.tblHonororiumCommitmentDetail
                                 join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                 join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                 join h in context.tblProject on g.ProjectId equals h.ProjectId
                                 join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                 where e.HonororiumId == Id && e.Amount > 0
                                 select new
                                 {
                                     h.ProjectType,
                                     h.ProjectId,
                                     h.SponProjectCategory
                                 }).ToList();
                    if (query != null)
                    {


                        for (int i = 0; i < query.Count; i++)
                        {
                            if (query[i].ProjectType == 2)
                            {
                                model.BillHeading = "Consultancy";
                            }
                            if (query[i].ProjectType == 1)
                            {
                                var proid = query[i].ProjectId;
                                var head = context.tblProject.Where(m => m.ProjectId == proid).Select(m => m.SponProjectCategory).FirstOrDefault();
                                if (head == "1")
                                {
                                    model.BillHeading = "Sponsored-PFMS";
                                    break;
                                }
                                if (head == "2")
                                {
                                    model.BillHeading = "Sponsored-NON-PFMS";

                                }
                            }
                            if (query[i].ProjectId == 2067)
                            {
                                model.BillHeading = "ICSR Over Head";
                            }
                        }
                    }
                    int[] headid = { 38, 61 };
                    var BankHeadId = (from e in
                       context.tblHonororiumExpenseDetail
                                      where (headid.Contains(e.AccountGroupId ?? 0) && e.HonororiumId == Id && e.TransactionType == "Credit")
                                      select new { e.AccountHeadId }).FirstOrDefault();
                    if (BankHeadId != null)
                        model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == BankHeadId.AccountHeadId).Select(m => m.AccountHead).FirstOrDefault();
                    model.BillId = Id;
                    model.TotalBillValue = Convert.ToDecimal(Qry.NetTotal);
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.HonororiumNo;
                    model.BillType = "Honorarium";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.Reason = Qry.Remarks;
                    List<PayableListModel> PayList = new List<PayableListModel>();
                    List<HonororiumPCFModel> PCFList = new List<HonororiumPCFModel>();
                    List<TDSPerModel> TDSList = new List<TDSPerModel>();
                    var Pay = (from e in context.tblHonororium
                               join f in context.tblHonororiumDetails on e.HonororiumId equals f.HonororiumId
                               where e.HonororiumId == Id
                               select new
                               {
                                   f.NetAmount,
                                   f.PayeeName,
                                   f.BankName,
                                   f.Branch,
                                   f.IFSC,
                                   f.AccountNo,
                                   f.PayeeType,
                                   f.Amount,
                                   f.TDS,
                                   f.TDSPercentage,
                                   f.PAN
                               }).ToList();
                    var PCF = (from e in context.tblHonororium
                               join f in context.tblHonororiumPCFDetail on e.HonororiumId equals f.HonororiumId
                               where e.HonororiumId == Id
                               select new
                               {
                                   f
                               }).ToList();
                    if (PCF != null)
                    {
                        for (int i = 0; i < PCF.Count; i++)
                        {

                            PCFList.Add(new HonororiumPCFModel()
                            {
                                PCFName = PCF[i].f.PayeeName,
                                OHDropdown = PCF[i].f.OHPercentage,
                                PCFAmount = PCF[i].f.Amount,
                                PayableToPCF = PCF[i].f.PayableToPCF,
                                PayableToOH = PCF[i].f.PayableToOH
                            });
                        }
                    }

                    model.PCF = PCFList;
                    if (Pay != null)
                    {

                        for (int i = 0; i < Pay.Count; i++)
                        {
                            int catid = Convert.ToInt32(Pay[i].PayeeType);
                            int TdsSec = Convert.ToInt32(Pay[i].TDSPercentage);
                            TDSList.Add(new TDSPerModel()
                            {
                                PayeeName = Pay[i].PayeeName + "-" + Common.GetCodeControlName(catid, "HonorCategory"),
                                BasicAmt = Convert.ToDecimal(Pay[i].Amount),
                                NetAmt = Convert.ToDecimal(Pay[i].NetAmount),
                                TDSAmt = Convert.ToDecimal(Pay[i].TDS),
                                TDSPer = Common.GetCodeControlName(TdsSec, "TDS")
                            });
                        }
                        for (int i = 0; i < Pay.Count; i++)
                        {
                            int catid = Convert.ToInt32(Pay[i].PayeeType);
                            PayList.Add(new PayableListModel()
                            {
                                Name = Pay[i].PayeeName + "-" + Common.GetCodeControlName(catid, "HonorCategory"),
                                Bank = Pay[i].BankName,
                                Amount = Convert.ToDecimal(Pay[i].NetAmount),
                                AccNo = Pay[i].AccountNo,
                                IFSC = Pay[i].IFSC,
                                Branch = Pay[i].Branch,
                                PAN = Pay[i].PAN
                            });
                        }
                    }
                    model.TDSPerList = TDSList;

                    model.Payable = PayList;
                    decimal? PayableAmt = 0;

                    PayableAmt = Qry.TotalAmount;

                    model.Comm = (from e in context.tblHonororiumCommitmentDetail
                                  join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.HonororiumId == Id && e.Amount > 0
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.Amount,
                                      h.ProjectId
                                  }).AsEnumerable()
                               .Select((x) => new CommitListModel()
                               {
                                   Number = x.CommitmentNumber,
                                   ProjNo = x.ProjectNumber,
                                   ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                                   NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                                   Date = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                                   StartDate = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectStartDate(x.ProjectId)),
                                   SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                                   Head = x.HeadName,
                                   Value = Convert.ToDecimal(x.Amount)
                               }).ToList();
                    int[] id = { 44, 45, 46 };

                    model.TDSGST = (from e in context.tblHonororiumExpenseDetail
                                    join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                    where e.HonororiumId == Id && (id.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                    select new
                                    {
                                        e.Amount,
                                        f.AccountHead
                                    }).AsEnumerable()
                                          .Select((x) => new TDSGSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();


                    int[] IT = context.tblAccountHead.Where(m => m.AccountGroupId == 15 && m.AccountHeadId != 135).Select(m => m.AccountHeadId).ToArray();

                    model.TDSIT = (from e in context.tblHonororiumExpenseDetail
                                   join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                   where e.HonororiumId == Id && (IT.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                   select new
                                   {
                                       e.Amount,
                                       f.AccountHead
                                   }).AsEnumerable()
                                          .Select((x) => new TDSITListForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();
                    model.PayableAmount = PayableAmt ?? 0;
                    model.TotalAmount = model.PayableAmount;
                    model.Rupees = CoreAccountsService.words(model.PayableAmount);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.Remarks = Qry.RequestReceivedFrom;
                    model.TravelerName = Common.GetUserNameBasedonId(Convert.ToInt32(Qry.CRTD_By));
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region ClearancePayment Bill Report
        public TravelBillReportModel GetClearancePaymentDetails(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblClearancePaymentEntry.Where(m => m.ClearancePaymentId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    int[] headid = { 38, 61 };
                    var BankHeadId = (from e in
                       context.tblClearancePaymentExpenseDetail
                                      where (headid.Contains(e.AccountGroupId ?? 0) && e.ClearancePaymentId == Id && e.TransactionType == "Credit")
                                      select new { e.AccountHeadId }).FirstOrDefault();
                    model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == Qry.BankHeadId).Select(m => m.AccountHead).FirstOrDefault();
                    var query = (from e in context.tblClearancePaymentCommitmentDetail
                                 join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                 join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                 join h in context.tblProject on g.ProjectId equals h.ProjectId
                                 join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                 where e.ClearancePaymentId == Id && e.PaymentAmount > 0
                                 select new
                                 {
                                     h.ProjectType,
                                     h.ProjectId
                                 }).ToList();
                    for (int i = 0; i < query.Count; i++)
                    {
                        if (query[i].ProjectType == 2)
                        {
                            model.BillHeading = "Consultancy";
                        }
                        if (query[i].ProjectType == 1)
                        {
                            var proid = query[i].ProjectId;
                            var head = context.tblProject.Where(m => m.ProjectId == proid).Select(m => m.SponProjectCategory).FirstOrDefault();
                            if (head == "1")
                            {
                                model.BillHeading = "Sponsored-PFMS";
                                break;
                            }
                            if (head == "2")
                            {
                                model.BillHeading = "Sponsored-NON-PFMS";

                            }
                        }
                        if (query[i].ProjectId == 2067)
                        {
                            model.BillHeading = "ICSR Over Head";
                        }
                    }
                    model.BillId = Id;
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.BillNumber;
                    model.BillType = Common.gettransactioncode(Qry.TransactionTypeCode);
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    //  var billval = (Qry.ExpenseAmount ?? 0) + (Qry.DeductionAmount ?? 0);
                    model.TotalBillValue = Convert.ToDecimal(Qry.CommitmentAmount);
                    model.TaxAmount = Convert.ToDecimal(Qry.BillTaxAmount);
                    model.PONumber = Qry.ReferencePONumber;
                    model.PODate = String.Format("{0:dd-MMMM-yyyy}", Qry.ReferencePODate);
                    model.RefNumber = Qry.InvoiceNumber;
                    model.InvoiceDate = String.Format("{0:dd-MMM-yyyy}", Qry.InvoiceDate);
                    var InvoiceQry = (from a in context.tblClearancePaymentInvoiceDetail
                                      where a.ClearancePaymentId == Id
                                      select new { a.InvoiceDate, a.InvoiceNumber, a.Amount, a.TaxValue }).ToList();
                    List<InvoiceListModel> InvoiceList = new List<InvoiceListModel>();
                    decimal TotalBilltaxValue = context.tblClearancePaymentPODetail.Where(m => m.ClearancePaymentId == Id).Sum(m => m.Amount) ?? 0;
                    TotalBilltaxValue += context.tblClearancePaymentPODetail.Where(m => m.ClearancePaymentId == Id).Sum(m => m.TaxAmount) ?? 0;

                    InvoiceList.Add(new InvoiceListModel
                    {
                        InvoiceNo = Qry.InvoiceNumber,
                        InvoiceDate = String.Format("{0:dd-MMM-yyyy}", Qry.InvoiceDate),
                        Amount = TotalBilltaxValue
                    });


                    //if (InvoiceQry.Count > 0)
                    //{
                    //    for (int i = 0; i < InvoiceQry.Count; i++)
                    //    {
                    //        InvoiceList.Add(new InvoiceListModel
                    //        {
                    //            InvoiceNo = InvoiceQry[i].InvoiceNumber,
                    //            InvoiceDate = String.Format("{0:dd-MMM-yyyy}", InvoiceQry[i].InvoiceDate),
                    //            Amount = ((InvoiceQry[i].Amount ?? 0)+(InvoiceQry[i].TaxValue??0))
                    //        });
                    //    }
                    //}
                    model.InvoiceList = InvoiceList;
                    decimal ttlInvVal = context.tblClearancePaymentExpenseDetail.Where(m => m.ClearancePaymentId == Id && m.IsJV_f != true && m.TransactionType == "Credit").Sum(m => m.Amount) ?? 0; //invVal + invTaxVal;
                    decimal TDSPayIT = context.tblClearancePaymentExpenseDetail.Where(m => m.ClearancePaymentId == Id && m.AccountHeadId == 135 && m.AccountGroupId == 15).Select(m => m.Amount).FirstOrDefault() ?? 0;
                    decimal TDSPayGST = context.tblClearancePaymentExpenseDetail.Where(m => m.ClearancePaymentId == Id && m.AccountHeadId == 137 && m.AccountGroupId == 16).Select(m => m.Amount).FirstOrDefault() ?? 0;
                    decimal ttlTDSPayable = TDSPayIT + TDSPayGST;
                    bool interState = Common.CheckClearanceAgentIsInterState(Qry.ClearancePaymentAgentId ?? 0);
                    decimal advAmt = 0;
                    decimal projCreditor = ttlInvVal - ttlTDSPayable;
                    decimal bankAC = ttlInvVal - advAmt - ttlTDSPayable;
                    model.Payable = (from f in context.tblClearancePaymentEntry
                                     join e in context.tblClearancePaymentBreakUpDetail on f.ClearancePaymentId equals e.ClearancePaymentId
                                     join d in context.tblClearanceAgentMaster on e.UserId equals d.ClearanceAgentId
                                     where f.ClearancePaymentId == Id
                                     select new
                                     {
                                         e.PaymentAmount,
                                         e.Name,
                                         d.BankName,
                                         d.AccountNumber,
                                         d.IFSC,
                                         d.GSTIN,
                                         d.PAN,
                                         e.ModeOfPayment
                                     }).AsEnumerable()
                                          .Select((x) => new PayableListModel()
                                          {
                                              Name = x.Name,
                                              Bank = x.BankName,
                                              PAN = x.PAN,
                                              Particulars = x.ModeOfPayment == 1 ? "Cheque" : "Bank Transfer",
                                              GSTIN = x.GSTIN,
                                              Amount = Convert.ToDecimal(x.PaymentAmount),
                                              AccNo = x.AccountNumber,
                                              IFSC = x.IFSC
                                          }).ToList();
                    decimal? PayableAmt = 0;
                    if (model.Payable != null)
                    {
                        PayableAmt = model.Payable.Select(m => m.Amount).Sum();
                    }
                    model.Comm = (from e in context.tblClearancePaymentCommitmentDetail
                                  join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.ClearancePaymentId == Id && e.PaymentAmount > 0
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.PaymentAmount,
                                      h.ProjectId
                                  }).AsEnumerable()
                               .Select((x) => new CommitListModel()
                               {
                                   Number = x.CommitmentNumber,
                                   ProjNo = x.ProjectNumber,
                                   ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                                   NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                                   Date = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                                   StartDate = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectStartDate(x.ProjectId)),
                                   SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                                   Head = x.HeadName,
                                   Value = Convert.ToDecimal(x.PaymentAmount)
                               }).ToList();
                    int[] GST = { 32, 33, 34, 36, 37, 38 };

                    model.GST = (from e in context.tblClearancePaymentDeductionDetail
                                 join d in context.tblDeductionHead on e.DeductionHeadId equals d.DeductionHeadId
                                 join f in context.tblAccountHead on d.AccountHeadId equals f.AccountHeadId
                                 where e.ClearancePaymentId == Id && (GST.Contains(d.AccountHeadId ?? 0)) && e.Amount > 0
                                 select new
                                 {
                                     e.Amount,
                                     f.AccountHead
                                 }).AsEnumerable()
                                          .Select((x) => new GSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();

                    //int[] id = { 44, 45, 46 };

                    //model.TDSGST = (from e in context.tblClearancePaymentExpenseDetail
                    //                join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                    //                where e.ClearancePaymentId == Id && (id.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                    //                select new
                    //                {
                    //                    e.Amount,
                    //                    f.AccountHead
                    //                }).AsEnumerable()
                    //                      .Select((x) => new TDSGSTForTravelModel()
                    //                      {
                    //                          Head = x.AccountHead,
                    //                          Value = Convert.ToDecimal(x.Amount)
                    //                      }).ToList();

                    //int[] IT = { 39, 40, 41, 42, 43 };

                    //model.TDSIT = (from e in context.tblClearancePaymentExpenseDetail
                    //               join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                    //               where e.ClearancePaymentId == Id && (IT.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                    //               select new
                    //               {
                    //                   e.Amount,
                    //                   f.AccountHead
                    //               }).AsEnumerable()
                    //                      .Select((x) => new TDSITListForTravelModel()
                    //                      {
                    //                          Head = x.AccountHead,
                    //                          Value = Convert.ToDecimal(x.Amount)
                    //                      }).ToList();
                    var TDSHead = (from a in context.tblClearancePaymentBreakUpDetail
                                   join b in context.tblTDSMaster on a.TDSSection equals b.TdsMasterId
                                   join c in context.tblAccountHead on b.AccountHeadId equals c.AccountHeadId
                                   where a.ClearancePaymentId == Id
                                   select c.AccountHead).FirstOrDefault();
                    List<TDSITListForTravelModel> TDSModel = new List<TDSITListForTravelModel>();
                    if (TDSPayIT > 0)
                    {
                        TDSModel.Add(new TDSITListForTravelModel()
                        {
                            Head = TDSHead,
                            Value = Convert.ToDecimal(TDSPayIT)

                        });
                    }
                    model.TDSIT = TDSModel;
                    var billQuery = context.tblBillEntry.SingleOrDefault(m => m.BillId == Id);
                    List<TDSGSTForTravelModel> GSTModel = new List<TDSGSTForTravelModel>();
                    if (TDSPayGST > 0)
                    {
                        if (interState)
                        {

                            GSTModel.Add(new TDSGSTForTravelModel()
                            {
                                Head = "IGST - TDS",
                                Value = Convert.ToDecimal(TDSPayGST)
                            });
                        }
                        else
                        {
                            GSTModel.Add(new TDSGSTForTravelModel()
                            {
                                Head = "CGST - TDS",
                                Value = Convert.ToDecimal(TDSPayGST / 2)
                            });
                            GSTModel.Add(new TDSGSTForTravelModel()
                            {
                                Head = "SGST - TDS",
                                Value = Convert.ToDecimal(TDSPayGST / 2)
                            });
                        }
                    }
                    model.TDSGST = GSTModel;
                    decimal? Gst = 0;
                    if (model.TDSGST != null)
                    {
                        Gst = model.TDSGST.Select(m => m.Value).Sum();
                    }
                    decimal? TDS = 0;
                    if (model.TDSIT != null)
                    {
                        TDS = model.TDSIT.Select(m => m.Value).Sum();
                    }
                    model.PayableAmount = PayableAmt ?? 0;
                    model.TotalBillValue = (TDS + Gst + PayableAmt) ?? 0;
                    model.TotalAmount = model.PayableAmount;
                    model.Rupees = CoreAccountsService.words(model.PayableAmount);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                }
                if (BillMode == "Old")
                {
                    model.BillMonth = BillMonth;
                    model.BillDate = BillDate;
                    model.PrintedDate = BillDate;
                }
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion
        #region Reimbursement Bill Report
        public TravelBillReportModel GetReimbursementBillDetails(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblAdhocPayment.Where(m => m.AdhocPaymentId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    var query = (from e in context.tblAdhocPayCommitmentDetail
                                 join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                 join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                 join h in context.tblProject on g.ProjectId equals h.ProjectId
                                 join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                 where e.AdhocPaymentId == Id && e.PaymentAmount > 0
                                 select new
                                 {
                                     h.ProjectType,
                                     h.ProjectId
                                 }).ToList();
                    int[] headid = { 38, 61 };
                    var BankHeadId = (from e in
                        context.tblAdhocPayExpenseDetail
                                      where (headid.Contains(e.AccountGroupId ?? 0) && e.AdhocPaymentId == Id && e.TransactionType == "Credit")
                                      select new { e.AccountHeadId }).FirstOrDefault();
                    model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == BankHeadId.AccountHeadId).Select(m => m.AccountHead).FirstOrDefault();

                    for (int i = 0; i < query.Count; i++)
                    {
                        if (query[i].ProjectType == 2)
                        {
                            model.BillHeading = "Consultancy";
                        }
                        if (query[i].ProjectType == 1)
                        {
                            var proid = query[i].ProjectId;
                            var head = context.tblProject.Where(m => m.ProjectId == proid).Select(m => m.SponProjectCategory).FirstOrDefault();
                            if (head == "1")
                            {
                                model.BillHeading = "Sponsored-PFMS";
                                break;
                            }
                            if (head == "2")
                            {
                                model.BillHeading = "Sponsored-NON-PFMS";

                            }
                        }
                        if (query[i].ProjectId == 2067)
                        {
                            model.BillHeading = "ICSR Over Head";
                        }
                    }
                    model.BillId = Id;
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.AdhocPaymentNumber;
                    model.BillType = "Reimbursement";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.PayableBankName = Qry.BankName;
                    model.Branch = Qry.BankBranch;
                    model.AccNo = Qry.AccountNumber;
                    model.Remarks = Qry.Remarks;
                    model.IFSC = Qry.IFSC;
                    if (Qry.PayeeType == 1)
                    {
                        int PayId = Convert.ToInt32(Qry.PayeeID);
                        model.PI = context.vwFacultyStaffDetails.Where(m => m.UserId == PayId).Select(m => m.FirstName).FirstOrDefault() + "-PI";
                    }
                    else if (Qry.PayeeType == 2)
                    {
                        model.PI = Qry.PayeeName + "-Student";
                    }
                    else if (Qry.PayeeType == 3)
                    {
                        model.PI = Qry.PayeeName + "-Others";
                    }
                    var InvoiceQry = (from a in context.tblAdhocPayInvoiceDetails
                                      where a.AdhocPaymentId == Id
                                      select new { a.InvoiceDate, a.InvoiceNumber, a.Amount }).ToList();
                    List<InvoiceListModel> InvoiceList = new List<InvoiceListModel>();
                    if (InvoiceQry.Count > 0)
                    {
                        for (int i = 0; i < InvoiceQry.Count; i++)
                        {
                            InvoiceList.Add(new InvoiceListModel
                            {
                                InvoiceNo = InvoiceQry[i].InvoiceNumber,
                                InvoiceDate = String.Format("{0:dd-MMM-yyyy}", InvoiceQry[i].InvoiceDate),
                                Amount = InvoiceQry[i].Amount ?? 0
                            });
                        }
                    }
                    model.InvoiceList = InvoiceList;
                    model.Comm = (from e in context.tblAdhocPayCommitmentDetail
                                  join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.AdhocPaymentId == Id && e.PaymentAmount > 0
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.PaymentAmount,
                                      h.ProjectId
                                  }).AsEnumerable()
                                .Select((x) => new CommitListModel()
                                {
                                    Number = x.CommitmentNumber,
                                    ProjNo = x.ProjectNumber,
                                    ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                                    NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                                    Date = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                                    StartDate = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectStartDate(x.ProjectId)),
                                    SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                                    Head = x.HeadName,
                                    Value = Convert.ToDecimal(x.PaymentAmount)
                                }).ToList();
                    int[] GST = { 32, 33, 34, 36, 37, 38 };

                    model.GST = (from e in context.tblAdhocPayDeductionDetail
                                 join d in context.tblDeductionHead on e.DeductionHeadId equals d.DeductionHeadId
                                 join f in context.tblAccountHead on d.AccountHeadId equals f.AccountHeadId
                                 where e.AdhocPaymentId == Id && (GST.Contains(d.AccountHeadId ?? 0)) && e.Amount > 0
                                 select new
                                 {
                                     e.Amount,
                                     f.AccountHead
                                 }).AsEnumerable()
                                          .Select((x) => new GSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();
                    int[] id = { 44, 45, 46 };

                    model.TDSGST = (from e in context.tblAdhocPayExpenseDetail
                                    join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                    where e.AdhocPaymentId == Id && (id.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                    select new
                                    {
                                        e.Amount,
                                        f.AccountHead
                                    }).AsEnumerable()
                                          .Select((x) => new TDSGSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();


                    int[] IT = context.tblAccountHead.Where(m => m.AccountGroupId == 15 && m.AccountHeadId != 135).Select(m => m.AccountHeadId).ToArray();

                    model.TDSIT = (from e in context.tblAdhocPayExpenseDetail
                                   join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                   where e.AdhocPaymentId == Id && (IT.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                   select new
                                   {
                                       e.Amount,
                                       f.AccountHead
                                   }).AsEnumerable()
                                          .Select((x) => new TDSITListForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();
                    model.TotalBillValue = Convert.ToDecimal(Qry.NetPayableAmount);
                    model.PayableAmount = model.TotalBillValue;
                    model.TotalAmount = model.TotalBillValue;
                    model.Rupees = CoreAccountsService.words(model.TotalBillValue);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                }
                if (BillMode == "Old")
                {
                    model.BillMonth = BillMonth;
                    model.BillDate = BillDate;
                    model.PrintedDate = BillDate;
                }
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion
        #region TemporaryAdvance Bill Report
        public TravelBillReportModel GetTemporaryAdvanceBillReportDetails(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var qry = context.tblTemporaryAdvance.Where(m => m.TemporaryAdvanceId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", qry.CRTD_TS);
                    model.PI = context.vwFacultyStaffDetails.Where(m => m.UserId == qry.PIId).Select(m => m.FirstName).FirstOrDefault();
                    var ProjectQry = context.tblProject.Where(m => m.ProjectId == qry.ProjectId).FirstOrDefault();
                    model.PayableBankName = Common.GetBankDetailsFromStaffBankAccount(qry.PIId ?? 0, "Professor").Bank;
                    model.IFSC = Common.GetBankDetailsFromStaffBankAccount(qry.PIId ?? 0, "Professor").IFSC;
                    model.Branch = Common.GetBankDetailsFromStaffBankAccount(qry.PIId ?? 0, "Professor").Branch;
                    model.AccNo = Common.GetBankDetailsFromStaffBankAccount(qry.PIId ?? 0, "Professor").AccNo;
                    model.ProjectNumber = ProjectQry.ProjectNumber;
                    model.ProjectType = context.tblCodeControl.Where(m => m.CodeName == "Projecttype" && m.CodeValAbbr == ProjectQry.ProjectType).Select(m => m.CodeValDetail).FirstOrDefault();
                    model.NameOfReceiver = qry.NameofReceiver;
                    model.Department = qry.DepartmentorSection;
                    int[] headid = { 38, 61 };
                    var BankHeadId = (from e in
                       context.tblTempAdvExpenseDetail
                                      where (headid.Contains(e.AccountGroupId ?? 0) && e.TempAdvId == Id && e.TransactionType == "Credit")
                                      select new { e.AccountHeadId }).FirstOrDefault();
                    //  var BankHeadId = context.tblTempAdvExpenseDetail.Where(m => m.AccountGroupId == 38 && m.TempAdvId == Id && m.TransactionType == "Credit").Select(m => m.AccountHeadId).FirstOrDefault();
                    model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == BankHeadId.AccountHeadId).Select(m => m.AccountHead).FirstOrDefault();
                    if (ProjectQry.ProjectType == 2)
                    {
                        model.BillHeading = "Consultancy";
                    }
                    if (ProjectQry.ProjectType == 1)
                    {
                        var head = ProjectQry.SponProjectCategory;
                        if (head == "1")
                            model.BillHeading = "Sponsored-PFMS";
                        if (head == "2")
                            model.BillHeading = "Sponsored-NON-PFMS";
                    }
                    if (ProjectQry.ProjectId == 2067)
                    {
                        model.BillHeading = "ICSR Over Head";
                    }
                    model.BillId = Id;
                    model.BillMonth = String.Format("{0:MMM yyyy}", qry.CRTD_TS);
                    model.BillNumber = qry.TemporaryAdvanceNumber;
                    model.BillType = Common.gettransactioncode(qry.TransactionTypeCode);
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", qry.CRTD_TS);
                    model.Payable = (from e in context.tblTemporaryAdvance
                                     join h in context.tblTempAdvanceBreakUpDetail on e.TemporaryAdvanceId equals h.TemporaryAdvanceId
                                     where e.TemporaryAdvanceId == Id
                                     select new
                                     {
                                         h.Amount,
                                         h.Particulars,
                                         e.PIId
                                     }).AsEnumerable()
                                         .Select((x) => new PayableListModel()
                                         {
                                             Name = x.Particulars,
                                             Amount = Convert.ToDecimal(x.Amount),

                                         }).ToList();
                    model.Comm = (from e in context.tblTempAdvCommitmentDetail
                                  join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.TempAdvId == Id && e.PaymentAmount > 0
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.PaymentAmount,
                                      h.ProjectId
                                  }).AsEnumerable()
                                 .Select((x) => new CommitListModel()
                                 {
                                     Number = x.CommitmentNumber,
                                     ProjNo = x.ProjectNumber,
                                     ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                                     NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                                     Date = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                                     StartDate = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectStartDate(x.ProjectId)),
                                     SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                                     Head = x.HeadName,
                                     Value = Convert.ToDecimal(x.PaymentAmount)
                                 }).ToList();
                    int[] GST = { 32, 33, 34, 36, 37, 38 };

                    model.GST = (from e in context.tblTempAdvDeductionDetail
                                 join d in context.tblDeductionHead on e.DeductionHeadId equals d.DeductionHeadId
                                 join f in context.tblAccountHead on d.AccountHeadId equals f.AccountHeadId
                                 where e.TempAdvId == Id && (GST.Contains(d.AccountHeadId ?? 0)) && e.Amount > 0
                                 select new
                                 {
                                     e.Amount,
                                     f.AccountHead
                                 }).AsEnumerable()
                                          .Select((x) => new GSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();

                    int[] id = { 44, 45, 46 };

                    model.TDSGST = (from e in context.tblTempAdvExpenseDetail
                                    join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                    where e.TempAdvId == Id && (id.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                    select new
                                    {
                                        e.Amount,
                                        f.AccountHead
                                    }).AsEnumerable()
                                          .Select((x) => new TDSGSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();


                    int[] IT = context.tblAccountHead.Where(m => m.AccountGroupId == 15 && m.AccountHeadId != 135).Select(m => m.AccountHeadId).ToArray();

                    model.TDSIT = (from e in context.tblTempAdvExpenseDetail
                                   join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                   where e.TempAdvId == Id && (IT.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                   select new
                                   {
                                       e.Amount,
                                       f.AccountHead
                                   }).AsEnumerable()
                                          .Select((x) => new TDSITListForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();
                    decimal? PayableAmt = 0;
                    if (model.Payable != null)
                    {
                        PayableAmt = model.Payable.Select(m => m.Amount).Sum();
                    }
                    model.PayableAmount = PayableAmt ?? 0;
                    model.TotalAmount = model.PayableAmount;
                    model.Rupees = CoreAccountsService.words(model.PayableAmount);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.TotalBillValue = Convert.ToDecimal(qry.TemporaryAdvanceAmountReceived);
                    model.Remarks = qry.Remarks;
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region SummerIntenship Bill Report
        public TravelBillReportModel GetSummerIntenshipBillReportDetails(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblSummerInternshipStudentDetails.Where(m => m.SummerInternshipStudentId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.StudentName = Qry.StudentName;
                    model.InstName = Qry.CollegeName;
                    model.RollNo = Qry.RollNo;
                    model.CourseOfStudy = Qry.Course;
                    model.YearOfStudy = Convert.ToInt32(Qry.YearofStudy);
                    model.Email = Qry.Email;
                    model.IntenshipDuration = Qry.Duration;
                    model.IntenshipStartDate = String.Format("{0:dd-MMMM-yyyy}", Qry.InternStartDate);
                    model.IntenshipEndDate = String.Format("{0:dd-MMMM-yyyy}", Qry.InternCloseDate);
                    model.TotalBillValue = Convert.ToDecimal(Qry.TotalStipendAmount);
                    model.AccNo = Qry.AccountNumber;
                    model.PayableBankName = Qry.BankName;
                    model.Branch = Qry.BranchName;
                    model.IFSC = Qry.IFSCCode;
                    var ProjectQry = context.tblProject.Where(m => m.ProjectId == Qry.ProjectId).FirstOrDefault();
                    // var BankHeadId = context.tblSummerInternshipExpenseDetail.Where(m => m.AccountGroupId == 38 && m.SummerInternStudentId == Id && m.TransactionType == "Credit").Select(m => m.AccountHeadId).FirstOrDefault();
                    // model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == BankHeadId).Select(m => m.AccountHead).FirstOrDefault();
                    int[] headid = { 38, 61 };
                    var BankHeadId = (from e in
                       context.tblSummerInternshipExpenseDetail
                                      where (headid.Contains(e.AccountGroupId ?? 0) && e.SummerInternStudentId == Id && e.TransactionType == "Credit")
                                      select new { e.AccountHeadId }).FirstOrDefault();
                    //  var BankHeadId = context.tblTempAdvExpenseDetail.Where(m => m.AccountGroupId == 38 && m.TempAdvId == Id && m.TransactionType == "Credit").Select(m => m.AccountHeadId).FirstOrDefault();
                    model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == BankHeadId.AccountHeadId).Select(m => m.AccountHead).FirstOrDefault();

                    if (ProjectQry.ProjectType == 2)
                    {
                        model.BillHeading = "Consultancy";
                    }
                    if (ProjectQry.ProjectType == 1)
                    {
                        var head = ProjectQry.SponProjectCategory;
                        if (head == "1")
                            model.BillHeading = "Sponsored-PFMS";
                        if (head == "2")
                            model.BillHeading = "Sponsored-NON-PFMS";
                    }
                    if (ProjectQry.ProjectId == 2067)
                    {
                        model.BillHeading = "ICSR Over Head";
                    }
                    model.BillId = Id;
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.SummerInternshipNumber;
                    model.BillType = "SummerIntenship";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.Comm = (from e in context.tblSummrInternCommitmentDetail
                                  join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.SummerInternId == Id && e.PaymentAmount > 0
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.PaymentAmount,
                                      h.ProjectId
                                  }).AsEnumerable()
                               .Select((x) => new CommitListModel()
                               {
                                   Number = x.CommitmentNumber,
                                   ProjNo = x.ProjectNumber,
                                   ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                                   NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                                   Date = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                                   StartDate = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectStartDate(x.ProjectId)),
                                   SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                                   Head = x.HeadName,
                                   Value = Convert.ToDecimal(x.PaymentAmount)
                               }).ToList();
                    int[] GST = { 32, 33, 34, 36, 37, 38 };

                    model.GST = (from e in context.tblSummrInternDeductionDetail
                                 join d in context.tblDeductionHead on e.DeductionHeadId equals d.DeductionHeadId
                                 join f in context.tblAccountHead on d.AccountHeadId equals f.AccountHeadId
                                 where e.SummrInternId == Id && (GST.Contains(d.AccountHeadId ?? 0)) && e.Amount > 0
                                 select new
                                 {
                                     e.Amount,
                                     f.AccountHead
                                 }).AsEnumerable()
                                          .Select((x) => new GSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();

                    int[] id = { 44, 45, 46 };

                    model.TDSGST = (from e in context.tblSummerInternshipExpenseDetail
                                    join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                    where e.SummerInternStudentId == Id && (id.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                    select new
                                    {
                                        e.Amount,
                                        f.AccountHead
                                    }).AsEnumerable()
                                          .Select((x) => new TDSGSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();


                    int[] IT = context.tblAccountHead.Where(m => m.AccountGroupId == 15 && m.AccountHeadId != 135).Select(m => m.AccountHeadId).ToArray();

                    model.TDSIT = (from e in context.tblSummerInternshipExpenseDetail
                                   join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                   where e.SummerInternStudentId == Id && (IT.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                   select new
                                   {
                                       e.Amount,
                                       f.AccountHead
                                   }).AsEnumerable()
                                          .Select((x) => new TDSITListForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();
                    model.TotalAmount = model.TotalBillValue;
                    model.Rupees = CoreAccountsService.words(model.TotalBillValue);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.Remarks = Qry.Remarks;
                }
                if (BillMode == "Old")
                {
                    model.BillMonth = BillMonth;
                    model.BillDate = BillDate;
                    model.PrintedDate = BillDate;
                }
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region PartTimeStudent Bill Report
        public TravelBillReportModel GetPartTimeStudentBillReportDetails(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblPartTimePayment.Where(m => m.PartTimePaymentId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    var ProjectQry = context.tblProject.Where(m => m.ProjectId == Qry.ProjectId).FirstOrDefault();
                    model.PI = context.vwFacultyStaffDetails.Where(m => m.UserId == ProjectQry.PIName).Select(m => m.FirstName).FirstOrDefault();
                    model.ProjectNumber = ProjectQry.ProjectNumber;
                    //  var BankHeadId = context.tblPartTimePaymentExpenseDetail.Where(m => m.AccountGroupId == 38 && m.PartTimePaymentId == Id && m.TransactionType == "Credit").Select(m => m.AccountHeadId).FirstOrDefault();
                    // model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == BankHeadId).Select(m => m.AccountHead).FirstOrDefault();
                    int[] headid = { 38, 61 };
                    var BankHeadId = (from e in
                       context.tblPartTimePaymentExpenseDetail
                                      where (headid.Contains(e.AccountGroupId ?? 0) && e.PartTimePaymentId == Id && e.TransactionType == "Credit")
                                      select new { e.AccountHeadId }).FirstOrDefault();
                    //  var BankHeadId = context.tblTempAdvExpenseDetail.Where(m => m.AccountGroupId == 38 && m.TempAdvId == Id && m.TransactionType == "Credit").Select(m => m.AccountHeadId).FirstOrDefault();
                    model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == BankHeadId.AccountHeadId).Select(m => m.AccountHead).FirstOrDefault();

                    if (ProjectQry.ProjectType == 2)
                    {
                        model.BillHeading = "Consultancy";
                    }
                    if (ProjectQry.ProjectType == 1)
                    {
                        var head = ProjectQry.SponProjectCategory;
                        if (head == "1")
                            model.BillHeading = "Sponsored-PFMS";
                        if (head == "2")
                            model.BillHeading = "Sponsored-NON-PFMS";
                    }
                    if (ProjectQry.ProjectId == 2067)
                    {
                        model.BillHeading = "ICSR Over Head";
                    }
                    model.BillId = Id;
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.PartTimePaymentNumber;
                    model.BillType = "Part Time Payment";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.TotalBillValue = Convert.ToDecimal(Qry.TotalStipendAmount);
                    model.Payable = (from e in context.tblPartTimeStudentDetails
                                     where e.PaymentId == Id
                                     select new
                                     {
                                         e.BankName,
                                         e.AccountNumber,
                                         e.IFSCCode,
                                         e.BranchName,
                                         e.RollNo
                                     }).AsEnumerable()
                                          .Select((x) => new PayableListModel()
                                          {
                                              RollNo = x.RollNo,
                                              Bank = x.BankName,
                                              AccNo = x.AccountNumber,
                                              IFSC = x.IFSCCode,
                                              Branch = x.BranchName
                                          }).ToList();
                    model.StudentList = (from e in context.tblPartTimeStudentDetails
                                         join d in context.tblStudentDetail on e.RollNo equals d.RollNumber
                                         where e.PaymentId == Id
                                         select new
                                         {
                                             e.StudentName,
                                             e.InternStartDate,
                                             e.InternCloseDate,
                                             e.StipendAmountperHour,
                                             e.TotalStipend,
                                             e.RollNo
                                         }).AsEnumerable()
                                          .Select((x) => new StudentListReportModel()
                                          {
                                              RollNo = x.RollNo,
                                              StudentName = x.StudentName,
                                              IntenStartDate = String.Format("{0:dd-MMMM-yyyy}", x.InternStartDate),
                                              IntenEndDate = String.Format("{0:dd-MMMM-yyyy}", x.InternCloseDate),
                                              AmountPerDay = Convert.ToDecimal(x.StipendAmountperHour),
                                              TotalAmount = Convert.ToDecimal(x.TotalStipend)
                                          }).ToList();
                    model.Comm = (from e in context.tblPartTimePaymentCommitmentDetail
                                  join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.PartTimePaymentId == Id && e.PaymentAmount > 0
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.PaymentAmount,
                                      h.ProjectId
                                  }).AsEnumerable()
                              .Select((x) => new CommitListModel()
                              {
                                  Number = x.CommitmentNumber,
                                  ProjNo = x.ProjectNumber,
                                  ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                                  NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                                  Date = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                                  StartDate = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectStartDate(x.ProjectId)),
                                  SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                                  Head = x.HeadName,
                                  Value = Convert.ToDecimal(x.PaymentAmount)
                              }).ToList();
                    int[] GST = { 32, 33, 34, 36, 37, 38 };

                    model.GST = (from e in context.tblPartTimePaymentDeductionDetail
                                 join d in context.tblDeductionHead on e.DeductionHeadId equals d.DeductionHeadId
                                 join f in context.tblAccountHead on d.AccountHeadId equals f.AccountHeadId
                                 where e.PartTimePaymentId == Id && (GST.Contains(d.AccountHeadId ?? 0)) && e.Amount > 0
                                 select new
                                 {
                                     e.Amount,
                                     f.AccountHead
                                 }).AsEnumerable()
                                          .Select((x) => new GSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();

                    int[] id = { 44, 45, 46 };

                    model.TDSGST = (from e in context.tblPartTimePaymentExpenseDetail
                                    join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                    where e.PartTimePaymentId == Id && (id.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                    select new
                                    {
                                        e.Amount,
                                        f.AccountHead
                                    }).AsEnumerable()
                                          .Select((x) => new TDSGSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();


                    int[] IT = context.tblAccountHead.Where(m => m.AccountGroupId == 15 && m.AccountHeadId != 135).Select(m => m.AccountHeadId).ToArray();

                    model.TDSIT = (from e in context.tblPartTimePaymentExpenseDetail
                                   join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                   where e.PartTimePaymentId == Id && (IT.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                   select new
                                   {
                                       e.Amount,
                                       f.AccountHead
                                   }).AsEnumerable()
                                          .Select((x) => new TDSITListForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();
                    model.TotalAmount = model.TotalBillValue;
                    model.Rupees = CoreAccountsService.words(model.TotalBillValue);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.Remarks = Qry.Remarks;
                }
                if (BillMode == "Old")
                {
                    model.BillMonth = BillMonth;
                    model.BillDate = BillDate;
                    model.PrintedDate = BillDate;
                }
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region TempAdvanceSettlement Bill Report
        public TravelBillReportModel GetTemporaryAdvanceSettlementDetails(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var qry = context.tblTempAdvanceSettlement.Where(m => m.TempAdvSettlementId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", qry.CRTD_TS);
                    var AdvanceQry = context.tblTemporaryAdvance.Where(m => m.TemporaryAdvanceId == qry.TemporaryAdvanceId).FirstOrDefault();
                    model.PI = context.vwFacultyStaffDetails.Where(m => m.UserId == AdvanceQry.PIId).Select(m => m.FirstName).FirstOrDefault();
                    var ProjectQry = context.tblProject.Where(m => m.ProjectId == qry.ProjectId).FirstOrDefault();
                    model.ProjectNumber = ProjectQry.ProjectNumber;
                    model.ProjectType = context.tblCodeControl.Where(m => m.CodeName == "Projecttype" && m.CodeValAbbr == ProjectQry.ProjectType).Select(m => m.CodeValDetail).FirstOrDefault();
                    model.NameOfReceiver = AdvanceQry.NameofReceiver;
                    model.Department = AdvanceQry.DepartmentorSection;
                    model.PayableBankName = Common.GetBankDetailsFromStaffBankAccount(AdvanceQry.PIId ?? 0, "Professor").Bank;
                    model.IFSC = Common.GetBankDetailsFromStaffBankAccount(AdvanceQry.PIId ?? 0, "Professor").IFSC;
                    model.Branch = Common.GetBankDetailsFromStaffBankAccount(AdvanceQry.PIId ?? 0, "Professor").Branch;
                    model.AccNo = Common.GetBankDetailsFromStaffBankAccount(AdvanceQry.PIId ?? 0, "Professor").AccNo;


                    int[] headid = { 38, 61 };
                    var BankHeadId = (from e in
                       context.tblTempAdvSettlExpenseDetail
                                      where (headid.Contains(e.AccountGroupId ?? 0) && e.TempAdvSettlId == Id && e.TransactionType == "Credit")
                                      select new { e.AccountHeadId }).FirstOrDefault();
                    //  var BankHeadId = context.tblTempAdvExpenseDetail.Where(m => m.AccountGroupId == 38 && m.TempAdvId == Id && m.TransactionType == "Credit").Select(m => m.AccountHeadId).FirstOrDefault();
                    int Bank = 0;
                    if (BankHeadId != null)
                    {
                        Bank = BankHeadId.AccountHeadId ?? 0;
                    }

                    model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == Bank).Select(m => m.AccountHead).FirstOrDefault();

                    if (ProjectQry.ProjectType == 2)
                    {
                        model.BillHeading = "Consultancy";
                    }
                    if (ProjectQry.ProjectType == 1)
                    {
                        var head = ProjectQry.SponProjectCategory;
                        if (head == "1")
                            model.BillHeading = "Sponsored-PFMS";
                        if (head == "2")
                            model.BillHeading = "Sponsored-NON-PFMS";
                    }
                    if (ProjectQry.ProjectId == 2067)
                    {
                        model.BillHeading = "ICSR Over Head";
                    }
                    model.BillId = Id;
                    model.BillMonth = String.Format("{0:MMM yyyy}", qry.CRTD_TS);
                    model.BillNumber = qry.TempSettlNumber;
                    model.BillType = Common.gettransactioncode(qry.TransactionTypeCode);
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", qry.CRTD_TS);
                    model.TotalBillValue = Convert.ToDecimal(AdvanceQry.TemporaryAdvanceAmountReceived);

                    decimal? tempAdvAmt = 0;
                    tempAdvAmt = AdvanceQry.TemporaryAdvanceAmountReceived;
                    decimal? billAmt = AdvanceQry.ExpenseAmount;

                    if (billAmt > tempAdvAmt)
                    {
                        model.PaymentValue = Convert.ToDecimal(billAmt - tempAdvAmt);
                        model.BalanceinAdvance = 0;
                    }
                    else if (billAmt < tempAdvAmt)
                    {
                        model.PaymentValue = 0;
                        model.BalanceinAdvance = Convert.ToDecimal(tempAdvAmt - billAmt);
                    }
                    else
                    {
                        model.PaymentValue = 0;
                        model.BalanceinAdvance = 0;
                    }
                    model.TempAdvanceValue = Convert.ToDecimal(tempAdvAmt);
                    model.TempSettleValue = Convert.ToDecimal(billAmt);
                    model.Remarks = qry.Remarks;
                    model.Payable = (from e in context.tblTempAdvanceSettlement
                                     join h in context.tblTempAdvSettlementBillBreakup on e.TempAdvSettlementId equals h.TempAdvSettlementId
                                     where e.TempAdvSettlementId == Id
                                     select new
                                     {
                                         h.Amount,
                                         h.Particulars,
                                         h.VendorBillNumber,
                                         h.VendorName
                                     }).AsEnumerable()
                                         .Select((x) => new PayableListModel()
                                         {
                                             Name = x.VendorName,
                                             Amount = Convert.ToDecimal(x.Amount),
                                             Particulars = x.Particulars,
                                             BillNo = x.VendorBillNumber

                                         }).ToList();
                    model.Comm = (from e in context.tblTempAdvSettlCommitmentDetail
                                  join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.TempAdvSettlId == Id && e.PaymentAmount > 0
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.PaymentAmount,
                                      h.ProjectId
                                  }).AsEnumerable()
                                 .Select((x) => new CommitListModel()
                                 {
                                     Number = x.CommitmentNumber,
                                     ProjNo = x.ProjectNumber,
                                     ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                                     NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                                     Date = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                                     StartDate = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectStartDate(x.ProjectId)),
                                     SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                                     Head = x.HeadName,
                                     Value = Convert.ToDecimal(x.PaymentAmount)
                                 }).ToList();
                    int[] GST = { 32, 33, 34, 36, 37, 38 };

                    model.GST = (from e in context.tblTempAdvSettlDeductionDetail
                                 join d in context.tblDeductionHead on e.DeductionHeadId equals d.DeductionHeadId
                                 join f in context.tblAccountHead on d.AccountHeadId equals f.AccountHeadId
                                 where e.TempSettlAdvId == Id && (GST.Contains(d.AccountHeadId ?? 0)) && e.Amount > 0
                                 select new
                                 {
                                     e.Amount,
                                     f.AccountHead
                                 }).AsEnumerable()
                                          .Select((x) => new GSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();

                    int[] id = { 44, 45, 46 };

                    model.TDSGST = (from e in context.tblTempAdvSettlExpenseDetail
                                    join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                    where e.TempAdvSettlId == Id && (id.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                    select new
                                    {
                                        e.Amount,
                                        f.AccountHead
                                    }).AsEnumerable()
                                          .Select((x) => new TDSGSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();


                    int[] IT = context.tblAccountHead.Where(m => m.AccountGroupId == 15 && m.AccountHeadId != 135).Select(m => m.AccountHeadId).ToArray();

                    model.TDSIT = (from e in context.tblTempAdvSettlExpenseDetail
                                   join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                   where e.TempAdvSettlId == Id && (IT.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                   select new
                                   {
                                       e.Amount,
                                       f.AccountHead
                                   }).AsEnumerable()
                                          .Select((x) => new TDSITListForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();
                    decimal? PayableAmt = 0;
                    if (model.Payable != null)
                    {
                        PayableAmt = model.Payable.Select(m => m.Amount).Sum();
                    }
                    model.PayableAmount = context.tblTempAdvSettlExpenseDetail.Where(m => m.AccountGroupId == 38 && m.TempAdvSettlId == Id).Select(m => m.Amount).FirstOrDefault() ?? 0;
                    model.TotalAmount = model.PayableAmount;
                    decimal CommTotalAmt = model.Comm.Sum(m => m.Value);
                    model.Rupees = CoreAccountsService.words(CommTotalAmt);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    // model.TotalBillValue = Convert.ToDecimal(qry.TemporaryAdvanceAmountReceived);
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region ImprestBillBoking Bill Report
        public TravelBillReportModel GetImprestBillBokingDetails(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblImprestRecoupment.Where(m => m.RecoupmentId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    var query = (from e in context.tblImpRecoupCommitmentDetail
                                 join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                 join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                 join h in context.tblProject on g.ProjectId equals h.ProjectId
                                 join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                 where e.RecoupmentId == Id && e.PaymentAmount > 0
                                 select new
                                 {
                                     h.ProjectType,
                                     h.ProjectId
                                 }).FirstOrDefault();
                    int[] headid = { 38, 61 };
                    var ProjectQry = context.tblProject.Where(m => m.ProjectId == query.ProjectId).FirstOrDefault();
                    model.PI = context.vwFacultyStaffDetails.Where(m => m.UserId == Qry.PIId).Select(m => m.FirstName).FirstOrDefault();
                    model.ProjectNumber = ProjectQry.ProjectNumber;
                    model.Reason = coreAccountService.GetImprestCardDetails(Id).ImprestBankACNumber;
                    var BankHeadId = (from e in
                    context.tblImprestRecoupExpenseDetail
                                      where (headid.Contains(e.AccountGroupId ?? 0) && e.ImprestRecoupId == Id && e.TransactionType == "Credit")
                                      select new { e.AccountHeadId }).FirstOrDefault();
                    model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == BankHeadId.AccountHeadId).Select(m => m.AccountHead).FirstOrDefault();

                    if (query.ProjectType == 2)
                    {
                        model.BillHeading = "Consultancy";
                    }
                    if (query.ProjectType == 1)
                    {
                        var proid = query.ProjectId;
                        var head = context.tblProject.Where(m => m.ProjectId == proid).Select(m => m.SponProjectCategory).FirstOrDefault();
                        if (head == "1")
                        {
                            model.BillHeading = "Sponsored-PFMS";

                        }
                        if (head == "2")
                        {
                            model.BillHeading = "Sponsored-NON-PFMS";

                        }
                    }
                    if (query.ProjectId == 2067)
                    {
                        model.BillHeading = "ICSR Over Head";
                    }

                    model.BillId = Id;
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.RecoupmentNumber;
                    model.BillType = "Imprest Bill Boking";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.TotalBillValue = Convert.ToDecimal(Qry.RecoupmentValue);
                    model.AllocatedValue = Convert.ToDecimal(Qry.AllocatedValue);
                    model.BalanceinAdvance = Convert.ToDecimal(Qry.BalanceinAdvance);
                    model.Payable = (from e in context.tblImprestRecoupment
                                     join h in context.tblImprestRecoupBillBreakup on e.RecoupmentId equals h.RecoupmentId
                                     where e.RecoupmentId == Id
                                     select new
                                     {
                                         h.Amount,
                                         h.Particulars,
                                         h.ImprestRecoupBillNumber,
                                         h.VendorName
                                     }).AsEnumerable()
                    .Select((x) => new PayableListModel()
                    {
                        Name = x.VendorName,
                        Amount = Convert.ToDecimal(x.Amount),
                        BillNo = x.ImprestRecoupBillNumber,
                        Particulars = x.Particulars
                    }).ToList();
                    model.Comm = (from e in context.tblImpRecoupCommitmentDetail
                                  join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.RecoupmentId == Id && e.PaymentAmount > 0
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.PaymentAmount,
                                      h.ProjectId
                                  }).AsEnumerable()
                    .Select((x) => new CommitListModel()
                    {
                        Number = x.CommitmentNumber,
                        ProjNo = x.ProjectNumber,
                        ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                        NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                        Date = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                        StartDate = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectStartDate(x.ProjectId)),
                        SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                        Head = x.HeadName,
                        Value = Convert.ToDecimal(x.PaymentAmount)
                    }).ToList();
                    int[] GST = { 32, 33, 34, 36, 37, 38 };

                    model.GST = (from e in context.tblImprestRecoupDeductionDetail
                                 join d in context.tblDeductionHead on e.DeductionHeadId equals d.DeductionHeadId
                                 join f in context.tblAccountHead on d.AccountHeadId equals f.AccountHeadId
                                 where e.RecoupmentId == Id && (GST.Contains(d.AccountHeadId ?? 0)) && e.Amount > 0
                                 select new
                                 {
                                     e.Amount,
                                     f.AccountHead
                                 }).AsEnumerable()
                    .Select((x) => new GSTForTravelModel()
                    {
                        Head = x.AccountHead,
                        Value = Convert.ToDecimal(x.Amount)
                    }).ToList();

                    int[] id = { 44, 45, 46 };

                    model.TDSGST = (from e in context.tblImprestRecoupExpenseDetail
                                    join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                    where e.ImprestRecoupId == Id && (id.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                    select new
                                    {
                                        e.Amount,
                                        f.AccountHead
                                    }).AsEnumerable()
                    .Select((x) => new TDSGSTForTravelModel()
                    {
                        Head = x.AccountHead,
                        Value = Convert.ToDecimal(x.Amount)
                    }).ToList();


                    int[] IT = context.tblAccountHead.Where(m => m.AccountGroupId == 15 && m.AccountHeadId != 135).Select(m => m.AccountHeadId).ToArray();

                    model.TDSIT = (from e in context.tblImprestRecoupExpenseDetail
                                   join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                   where e.ImprestRecoupId == Id && (IT.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                   select new
                                   {
                                       e.Amount,
                                       f.AccountHead
                                   }).AsEnumerable()
                    .Select((x) => new TDSITListForTravelModel()
                    {
                        Head = x.AccountHead,
                        Value = Convert.ToDecimal(x.Amount)
                    }).ToList();
                    model.TotalAmount = model.TotalBillValue;
                    model.Rupees = CoreAccountsService.words(model.TotalBillValue);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);

                }
                if (BillMode == "Old")
                {
                    model.BillMonth = BillMonth;
                    model.BillDate = BillDate;
                    model.PrintedDate = BillDate;
                }
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region ImprestBillsRecoupment Bill Report
        public TravelBillReportModel GetImprestBillsRecoupmentDetails(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblImprestBillRecoupment.Where(m => m.ImprestBillRecoupId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.BillId = Id;
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.ImprestBillRecoupNumber;
                    model.BillType = "Imprest Bills Recoupment";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.TotalBillValue = Convert.ToDecimal(Qry.Amount);
                    List<TDSGSTForTravelModel> list = new List<TDSGSTForTravelModel>();
                    var qry = (from e in context.tblImprestBillRecoupDetail
                               join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                               join g in context.tblAccountGroup on e.AccountGroupId equals g.AccountGroupId
                               where e.ImprestBillRecoupId == Id
                               select new
                               {
                                   e.Amount,
                                   f.AccountHead,
                                   g.AccountGroup
                               }).ToList();
                    for (int i = 0; i < qry.Count; i++)
                    {
                        list.Add(new TDSGSTForTravelModel()
                        {
                            Head = qry[i].AccountGroup + "-" + qry[i].AccountHead,
                            Value = Convert.ToDecimal(qry[i].Amount)
                        });
                    }
                    model.TDSGST = list;
                    decimal PayableAmt = 0;
                    if (model.TDSGST != null)
                    {
                        PayableAmt = model.TDSGST.Select(m => m.Value).Sum();
                    }
                    model.PayableAmount = PayableAmt;
                    model.TotalAmount = model.TotalBillValue;
                    model.Rupees = CoreAccountsService.words(model.TotalBillValue);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.TotalBillValue = Convert.ToDecimal(Qry.Amount);
                    model.Remarks = Qry.Narration;
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion
        #region Direct Fund Transfer Bill Report
        public TravelBillReportModel GetDirectFundTransferBillDetails(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblProjectDirectTransfer.Where(m => m.ProjectTransferId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.BillId = Id;
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.TransferNumber;
                    model.BillType = "Direct Fund Transfer";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.PayableBankName = context.tblAccountHead.Where(m => m.AccountHeadId == Qry.CreditBank).Select(m => m.AccountHead).FirstOrDefault();
                    model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == Qry.DebitBank).Select(m => m.AccountHead).FirstOrDefault();
                    model.TotalBillValue = Convert.ToDecimal(Qry.Amount);
                    model.Remarks = Qry.Remarks;
                    var CreditProject = context.tblProject.Where(m => m.ProjectId == Qry.CreditProjectId).FirstOrDefault();
                    var DebitProject = context.tblProject.Where(m => m.ProjectId == Qry.DebitProjectId).FirstOrDefault();
                    model.ProjectNumber = CreditProject.ProjectNumber;
                    model.ProjectType = DebitProject.ProjectNumber;
                    model.BillHeading = Common.GetProjectTypeForBill(Qry.CreditProjectId ?? 0);

                    if (CreditProject.ProjectId == 2067)
                    {
                        model.BillHeading = "ICSR Over Head";
                    }
                    model.Comm = (from e in context.tblProjectTransCommitmentDetail
                                  join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.ProjectTransferId == Id && e.PaymentAmount > 0
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.PaymentAmount,
                                      h.ProjectId
                                  }).AsEnumerable()
                              .Select((x) => new CommitListModel()
                              {
                                  Number = x.CommitmentNumber,
                                  ProjNo = x.ProjectNumber,
                                  ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                                  NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                                  Date = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                                  StartDate = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectStartDate(x.ProjectId)),
                                  SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                                  Head = x.HeadName,
                                  Value = Convert.ToDecimal(x.PaymentAmount)
                              }).ToList();
                    model.Rupees = CoreAccountsService.words(Qry.Amount);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion
        #region Headwise Fund Transfer Bill Reprot
        public TravelBillReportModel GetHeadwiseFundTransferBillDetails(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblProjectTransfer.Where(m => m.ProjectTransferId == Id).FirstOrDefault();
                    model.BillId = Id;
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.TransferNumber;
                    model.BillType = " Headwise Fund Transfer";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    // model.TotalBillValue = Convert.ToDecimal(Qry.Amount);
                    model.Reason = Qry.Narration;
                    model.NameOfReceiver = Qry.CreditReason;
                    model.Remarks = Qry.DebitReason;
                    var CreditProject = context.tblProject.Where(m => m.ProjectId == Qry.CreditProjectId).FirstOrDefault();
                    var DebitProject = context.tblProject.Where(m => m.ProjectId == Qry.DebitProjectId).FirstOrDefault();
                    model.ProjectNumber = CreditProject.ProjectNumber + "(" + Common.GetProjectTypeForBill(Qry.CreditProjectId ?? 0) + ") /" + string.Format("{0:dd-MMM-yyyy}", Common.GetProjectsDetails(Qry.CreditProjectId ?? 0).CloseDate);
                    model.ProjectType = DebitProject.ProjectNumber + "(" + Common.GetProjectTypeForBill(Qry.DebitProjectId ?? 0) + ") /" + string.Format("{0:dd-MMM-yyyy}", Common.GetProjectsDetails(Qry.DebitProjectId ?? 0).CloseDate);
                    model.BillHeading = Common.GetProjectTypeForBill(Qry.CreditProjectId ?? 0);

                    if (CreditProject.ProjectId == 2067)
                    {
                        model.BillHeading = "ICSR Over Head";
                    }
                    List<TDSGSTForTravelModel> list = new List<TDSGSTForTravelModel>();
                    var qry = (from a in context.tblProjectTransfer
                               join e in context.tblProjectTransferDetails on a.ProjectTransferId equals e.ProjectTransferId
                               join f in context.tblBudgetHead on e.BudgetHeadId equals f.BudgetHeadId
                               where e.TransactionType == "Credit" && e.ProjectTransferId == Id
                               select new
                               {
                                   e.Amount,
                                   f.HeadName,
                               }).ToList();
                    if (qry != null)
                    {
                        for (int i = 0; i < qry.Count; i++)
                        {
                            list.Add(new TDSGSTForTravelModel()
                            {
                                Head = qry[i].HeadName,
                                Value = Convert.ToDecimal(qry[i].Amount)
                            });
                        }
                    }
                    model.TDSGST = list;
                    List<TDSITListForTravelModel> newlist = new List<TDSITListForTravelModel>();
                    var tdslist = (from a in context.tblProjectTransfer
                                   join e in context.tblProjectTransferDetails on a.ProjectTransferId equals e.ProjectTransferId
                                   join f in context.tblBudgetHead on e.BudgetHeadId equals f.BudgetHeadId
                                   where e.TransactionType == "Debit" && e.ProjectTransferId == Id
                                   select new
                                   {
                                       e.Amount,
                                       f.HeadName
                                   }).ToList();
                    if (tdslist != null)
                    {
                        for (int i = 0; i < tdslist.Count; i++)
                        {
                            newlist.Add(new TDSITListForTravelModel()
                            {
                                Head = tdslist[i].HeadName,
                                Value = Convert.ToDecimal(tdslist[i].Amount)
                            });
                        }
                    }
                    model.TDSIT = newlist;
                    // model.TotalBillValue = Convert.ToDecimal(Qry.NetPayableAmount);
                    // model.PayableAmount = model.TotalBillValue;
                    // model.TotalAmount = model.TotalBillValue;
                    model.Rupees = CoreAccountsService.words(Qry.Amount);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                }
                if (BillMode == "Old")
                {
                    model.BillMonth = BillMonth;
                    model.BillDate = BillDate;
                    model.PrintedDate = BillDate;
                }
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region Contra Bill Report
        public TravelBillReportModel GetContraDetails(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblContra.Where(m => m.ContraId == Id).FirstOrDefault();
                    model.BillId = Id;
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.ContraNumber;
                    model.BillType = "Contra";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.TotalBillValue = Convert.ToDecimal(Qry.Amount);
                    List<TDSGSTForTravelModel> list = new List<TDSGSTForTravelModel>();
                    var qry = (from e in context.tblContraDetail
                               join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                               join g in context.tblAccountGroup on e.AccountGroupId equals g.AccountGroupId
                               where e.ContraId == Id
                               select new
                               {
                                   e.Amount,
                                   f.AccountHead,
                                   g.AccountGroup,
                                   e.TransactionType
                               }).ToList();
                    for (int i = 0; i < qry.Count; i++)
                    {
                        list.Add(new TDSGSTForTravelModel()
                        {
                            Head = qry[i].AccountGroup + "-" + qry[i].AccountHead,
                            Value = Convert.ToDecimal(qry[i].Amount),
                            Type = qry[i].TransactionType
                        });
                    }
                    model.TDSGST = list;
                    decimal PayableAmt = 0;
                    if (model.TDSGST != null)
                    {
                        PayableAmt = model.TDSGST.Select(m => m.Value).Sum();
                    }
                    model.PayableAmount = PayableAmt;
                    model.TotalAmount = model.TotalBillValue;
                    model.Rupees = CoreAccountsService.words(model.TotalBillValue);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.TotalBillValue = Convert.ToDecimal(Qry.Amount);
                    model.Remarks = Qry.Narration;
                    model.RefNumber = Qry.BillRefNumber;
                }
                if (BillMode == "Old")
                {
                    model.BillMonth = BillMonth;
                    model.BillDate = BillDate;
                    model.PrintedDate = BillDate;
                }
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region Journal Bill Report
        public TravelBillReportModel GetJournalBillDetails(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblJournal.Where(m => m.JournalId == Id).FirstOrDefault();
                    if (Qry != null)
                    {
                        model.BillId = Id;
                        model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                        model.BillNumber = Qry.JournalNumber;
                        model.BillType = "Journal";
                        model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                        model.TotalBillValue = Convert.ToDecimal(Qry.Amount);
                        model.Reason = Common.GetCodeControlName(Convert.ToInt32(Qry.Reason), "Journal Reason");
                        List<TDSGSTForTravelModel> list = new List<TDSGSTForTravelModel>();
                        var qry = (from e in context.tblJournalTransationDetail
                                   join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                   join g in context.tblAccountGroup on e.AccountGroupId equals g.AccountGroupId
                                   where e.TransactionType == "Credit" && e.JournalId == Id
                                   select new
                                   {
                                       e.Amount,
                                       f.AccountHead,
                                       g.AccountGroup
                                   }).ToList();
                        if (qry != null)
                        {
                            for (int i = 0; i < qry.Count; i++)
                            {
                                list.Add(new TDSGSTForTravelModel()
                                {
                                    Head = qry[i].AccountGroup + "-" + qry[i].AccountHead,
                                    Value = Convert.ToDecimal(qry[i].Amount)
                                });
                            }
                        }
                        model.TDSGST = list;
                        List<TDSITListForTravelModel> newlist = new List<TDSITListForTravelModel>();
                        var tdslist = (from e in context.tblJournalTransationDetail
                                       join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                       join g in context.tblAccountGroup on e.AccountGroupId equals g.AccountGroupId
                                       where e.TransactionType == "Debit" && e.JournalId == Id
                                       select new
                                       {
                                           e.Amount,
                                           f.AccountHead,
                                           g.AccountGroup
                                       }).ToList();
                        if (tdslist != null)
                        {
                            for (int i = 0; i < tdslist.Count; i++)
                            {
                                newlist.Add(new TDSITListForTravelModel()
                                {
                                    Head = tdslist[i].AccountGroup + "-" + tdslist[i].AccountHead,
                                    Value = Convert.ToDecimal(tdslist[i].Amount)
                                });
                            }
                        }
                        model.TDSIT = newlist;
                        decimal PayableAmt = 0;
                        if (model.TDSGST != null)
                        {
                            PayableAmt = model.TDSGST.Select(m => m.Value).Sum();
                        }
                        model.PayableAmount = PayableAmt;
                        model.TotalAmount = model.TotalBillValue;
                        model.Rupees = CoreAccountsService.words(model.TotalBillValue);
                        model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                        model.TotalBillValue = Convert.ToDecimal(Qry.Amount);
                        model.Remarks = Qry.Narration;
                    }
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        public static List<TrailBalanceModel> TrailBalanceRep2(int Finyear = 0)
        {
            List<TrailBalanceModel> boa = new List<TrailBalanceModel>();
            using (var context = new IOASDBEntities())
            {
                var Qry = (from b in context.vw_DemoLedgers
                           where Finyear == 0 || b.FinancialYear == Finyear
                           select new
                           {
                               b.AccountGroupId,
                               b.AccountHead,
                               b.AccountHeadId,
                               b.Accounts,
                               b.Amount,
                               b.Creditor_f,
                               b.Debtor_f,
                               b.Groups,
                               b.TransactionType
                           }).ToList();
                //                var AssetCr = Qry.Where(m=>m.TransactionType=="Credit"&&m.Accounts== "Asset")
                //    .GroupBy(a => a.AccountHeadId)
                //    .Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                //    .OrderByDescending(a => a.Amount)
                //    .ToList();

                //                var AssetDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Asset")
                // .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                // .OrderByDescending(a => a.Amount)
                // .ToList();
                //                decimal ttlAssetDr = AssetDr.Sum(m => m.Amount) ?? 0;
                //                decimal ttlAssetCr = AssetCr.Sum(m => m.Amount) ?? 0;
                //                var LiabilityCr = Qry.Where(m => m.TransactionType == "Credit" && m.Accounts == "Liability")
                // .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                // .OrderByDescending(a => a.Amount)
                // .ToList();

                //                var LiabilityDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Liability")
                //.GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                //.OrderByDescending(a => a.Amount)
                //.ToList();
                //                decimal ttlLiabilityDr = LiabilityDr.Sum(m => m.Amount) ?? 0 ;
                //                decimal ttlLiabilityCr = LiabilityCr.Sum(m => m.Amount) ?? 0;
                //                var IncomeCr = Qry.Where(m => m.TransactionType == "Credit" && m.Accounts == "Income")
                //             .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                //             .OrderByDescending(a => a.Amount)
                //             .ToList();
                //                var IncomeDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Income")
                //.GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                //.OrderByDescending(a => a.Amount)
                //.ToList();
                //                decimal ttlIncomeDr = IncomeDr.Sum(m => m.Amount) ?? 0 ;
                //                decimal ttlIncomeCr = IncomeCr.Sum(m => m.Amount) ?? 0;
                //                var ExpenseCr = Qry.Where(m => m.TransactionType == "Credit" && m.Accounts == "Expense")
                //           .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                //           .OrderByDescending(a => a.Amount)
                //           .ToList();

                //                var ExpenseDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Expense")
                //.GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                //.OrderByDescending(a => a.Amount)
                //.ToList();
                //                decimal ttlExpenseDr = ExpenseDr.Sum(m => m.Amount) ?? 0 ;
                //                decimal ttlExpenseCr = ExpenseCr.Sum(m => m.Amount) ?? 0;
                if (Qry.Count > 0)
                {
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        boa.Add(new TrailBalanceModel()
                        {
                            Accounts = Qry[i].Accounts,
                            AccountGroupId = Convert.ToInt32(Qry[i].AccountGroupId),
                            AccountHeadId = Convert.ToInt32(Qry[i].AccountHeadId),
                            AccountHead = Qry[i].AccountHead,
                            TransactionType = Qry[i].TransactionType,
                            Amount = Convert.ToDecimal(Qry[i].Amount),
                            Creditor_f = Convert.ToBoolean(Qry[i].Creditor_f),
                            Debtor_f = Convert.ToBoolean(Qry[i].Debtor_f),
                            Groups = Qry[i].Groups
                        });
                    }
                }

            }
            return boa;
        }
        public static List<PostingsModel> PostingsRep(DateTime fromdate, DateTime todate, string transactiontype)
        {

            List<PostingsModel> boa = new List<PostingsModel>();
            todate = todate.AddDays(1).AddTicks(-2);
            using (var context = new IOASDBEntities())
            {
                var Qry = (from b in context.vw_Postings
                           where ((b.PostedDate >= fromdate && b.PostedDate <= todate) && (String.IsNullOrEmpty(transactiontype) ||
                           b.TransType.Contains(transactiontype)) && !String.IsNullOrEmpty(b.TransactionType))
                           orderby b.PostedDate descending
                           select new
                           {
                               b.PostedDate,
                               b.AccountHead,
                               b.Accounts,
                               b.Creditor_f,
                               b.Debtor_f,
                               b.Groups,
                               b.TransactionType,
                               b.Amount,
                               b.TransType,
                               b.TempVoucherNumber

                           }).ToList();

                if (Qry.Count > 0)
                {
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        boa.Add(new PostingsModel()
                        {
                            Amount = Convert.ToDecimal(Qry[i].Amount),
                            PostedDate = Convert.ToDateTime(Qry[i].PostedDate),
                            Groups = Qry[i].Groups,
                            AccountHead = Qry[i].AccountHead,
                            TransactionType = Qry[i].TransactionType,
                            TransType = Qry[i].TransType,
                            Accounts = Qry[i].Accounts,
                            Creditor_f = Convert.ToBoolean(Qry[i].Creditor_f),
                            Debtor_f = Convert.ToBoolean(Qry[i].Debtor_f),
                            TempVoucherNumber = Qry[i].TempVoucherNumber
                        });
                    }
                }

            }
            return boa;
        }
        public static List<CommitmentReportModel> CommitmentRep(DateTime fromdate, DateTime todate, int projecttype, int projectnumber)
        {
            List<CommitmentReportModel> com = new List<CommitmentReportModel>();
            todate = todate.AddDays(1).AddTicks(-2);
            using (var context = new IOASDBEntities())
            {
                var Qry = (from b in context.vw_CommitmentReport
                           where (((b.CommitmentDate >= fromdate && b.CommitmentDate <= todate)) &&
                           (b.ProjectType == projecttype) && (projectnumber == 0 || b.ProjectId == projectnumber))
                           orderby b.CommitmentDate descending
                           select new
                           {
                               b.ProjectNumber,
                               b.CommitmentNumber,
                               b.ProjectType,
                               b.ProjectTypeName,
                               b.CommitmentType,
                               b.CommitmentDate,
                               b.CommitmentAmount,
                               b.BookedValue,
                           }).ToList();

                if (Qry.Count > 0)
                {
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        com.Add(new CommitmentReportModel()
                        {
                            CommitmentAmount = Convert.ToDecimal(Qry[i].CommitmentAmount),
                            CommitmentDate = Convert.ToDateTime(Qry[i].CommitmentDate),
                            ProjectNumber = Qry[i].ProjectNumber,
                            CommitmentNumber = Qry[i].CommitmentNumber,
                            ProjectTypeName = Qry[i].ProjectTypeName,
                            CommitmentType = Qry[i].CommitmentType,
                            BookedValue = Convert.ToDecimal(Qry[i].BookedValue)
                        });
                    }
                }

            }
            return com;
        }
        public static DailyBalanceVerificationModel GetDailyBalanceVerfication(int projectid, DateTime Date)
        {
            try
            {

                DailyBalanceVerificationModel Dailmodel = new DailyBalanceVerificationModel();
                using (var context = new IOASDBEntities())
                {
                    ProjectService proser = new ProjectService();
                    var Service = proser.getProjectSummary(projectid);
                    var Ser = proser.getProjectSummaryForDailyBalance(projectid, Date);
                    var qryProject = (from prj in context.tblProject
                                      where (prj.ProjectId == projectid)
                                      select prj).FirstOrDefault();
                    var qryPreviousCommit = (from C in context.tblCommitment
                                             join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                             where (C.ProjectId == projectid && C.Status == "Active") && ((C.CRTD_TS == Date))
                                             select new { D.BalanceAmount, D.ReversedAmount }).FirstOrDefault();
                    decimal? BalanceAmt;
                    decimal? ReversedAmount;
                    if (qryPreviousCommit == null)
                    {
                        BalanceAmt = 0;
                        ReversedAmount = 0;
                    }
                    else
                    {
                        BalanceAmt = qryPreviousCommit.BalanceAmount;
                        ReversedAmount = qryPreviousCommit.ReversedAmount;
                    }
                    decimal? Debit = 0, Credit = 0, spentAmt = 0;
                    var qrySpenAmt = (from C in context.vwCommitmentSpentBalance where (C.ProjectId == projectid) && ((C.CRTD_TS == Date)) select C.AmountSpent).Sum();
                    if (qrySpenAmt == null)
                        qrySpenAmt = 0;
                    spentAmt = qrySpenAmt;
                    var FundTransferDebit = (from C in context.tblProjectTransfer
                                             from D in context.tblProjectTransferDetails
                                             where C.ProjectTransferId == D.ProjectTransferId
                                             where C.DebitProjectId == projectid && C.CRTD_TS == Date
                                             select D).ToList();
                    if (FundTransferDebit.Count > 0)
                    {
                        Debit = FundTransferDebit.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum();
                        if (Debit != 0)
                            spentAmt = spentAmt + Debit;
                    }
                    var FundTransferCredit = (from C in context.tblProjectTransfer
                                              from D in context.tblProjectTransferDetails
                                              where C.ProjectTransferId == D.ProjectTransferId
                                              where C.CreditProjectId == projectid && C.CRTD_TS == Date
                                              select D).ToList();
                    if (FundTransferCredit.Count > 0)
                    {
                        Credit = FundTransferCredit.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum();
                        if (Credit != 0)
                            spentAmt = spentAmt - Credit;
                    }

                    var AvailableCommitment = BalanceAmt + ReversedAmount;
                    var QryReceipt = (from C in context.tblReceipt where ((C.ProjectId == projectid) && ((C.CrtdTS == Date))) select C).FirstOrDefault();
                    decimal receiptAmt = 0;
                    decimal OverHead = 0;
                    decimal CGST = 0;
                    decimal SGST = 0;
                    decimal IGST = 0;
                    decimal GST = 0;
                    if (QryReceipt != null)
                    {
                        receiptAmt = QryReceipt.ReceiptAmount ?? 0;
                        OverHead = QryReceipt.ReceiptOverheadValue ?? 0;
                        CGST = QryReceipt.CGST ?? 0;
                        SGST = QryReceipt.SGST ?? 0;
                        IGST = QryReceipt.IGST ?? 0;
                        GST = CGST + SGST + IGST;
                    }

                    /* Negative balance taking query*/
                    var qryNegativeBal = (from Neg in context.tblNegativeBalance
                                          where (Neg.ProjectId == projectid && Neg.Status == "Approved") && ((Neg.CRTD_TS == Date))
                                          select Neg.NegativeBalanceAmount).Sum();
                    /* Opening balance taking query*/
                    decimal qryOpeningBal = (from OB in context.tblProjectOB
                                             where OB.ProjectId == projectid && ((OB.Crt_TS == Date))
                                             select OB.OpeningBalance).Sum() ?? 0;
                    /* Opening balance taking query end*/
                    Dailmodel.ProjectNo = qryProject.ProjectNumber;
                    Dailmodel.PI = Common.GetPIName(qryProject.PIName ?? 0);
                    Dailmodel.OB = qryOpeningBal;
                    //sum(ReciptAmt-(GST+OverHeads ))                 
                    Dailmodel.Receipt = Convert.ToDecimal(receiptAmt);
                    Dailmodel.PreviousCommitment = AvailableCommitment ?? 0;
                    Dailmodel.AvailBalance = ((qryOpeningBal + Dailmodel.TotalReceipt) - (Dailmodel.AmountSpent + Dailmodel.PreviousCommitment));
                    Dailmodel.ApprovedNegativeBalance = qryNegativeBal ?? 0;
                    Dailmodel.NetBalance = Service.NetBalance;
                    Dailmodel.OpeningBalance = ((Ser.TotalReceipt + Ser.ApprovedNegativeBalance) - (Ser.PreviousCommitment + Ser.AmountSpent));
                    Dailmodel.TotalReceipt = Service.TotalReceipt;
                    Dailmodel.TotalSanction = Service.SanctionedValue;
                    Dailmodel.TotalNegativeBalance = Service.ApprovedNegativeBalance;
                    Dailmodel.TotalDistribution = Common.GetDistribuAmount(projectid);
                    Dailmodel.TotalExpent = Service.AmountSpent - Dailmodel.TotalDistribution;
                    Dailmodel.TotalCommitment = Service.PreviousCommitment;
                    Dailmodel.TotalAvailBalance = Service.AvailableBalance;
                    Dailmodel.TlOB = Service.OpeningBalance;
                    Dailmodel.ClosingBalance = ((Dailmodel.OpeningBalance - (Dailmodel.PreviousCommitment + Dailmodel.AmountSpent) + (Dailmodel.Receipt + Dailmodel.ApprovedNegativeBalance)));
                    var disQry = context.tblDistribution.Where(m => m.ProjectId == projectid && m.CRTD_TS == Date).Select(m => m.DistributionAmount).ToList();
                    decimal Dis = 0;
                    if (disQry != null)
                    {
                        Dis = disQry.Sum() ?? 0;
                    }
                    Dailmodel.Distribution = Dis;
                    Dailmodel.AmountSpent = (spentAmt ?? 0) - (Dis);
                    if (qryProject.ProjectType == 2)
                    {
                        Dailmodel.TypeName = "Consultancy";
                    }
                    if (qryProject.ProjectType == 1)
                    {
                        var head = context.tblProject.Where(m => m.ProjectId == qryProject.ProjectId).Select(m => m.AcctType).FirstOrDefault();
                        if (head == "PFMS")
                            Dailmodel.TypeName = "Sponsored-PFMS";
                        if (head == "NON-PFMS")
                            Dailmodel.TypeName = "Sponsored-NON-PFMS";
                    }
                    if (qryProject.ProjectId == 2067)
                    {
                        Dailmodel.TypeName = "ICSR Over Head";
                    }
                    Dailmodel.ProjClassification = Common.GetCodeControlName(Convert.ToInt32(qryProject.ProjectClassification), "ProjectClassification");
                    Dailmodel.ReportDate = String.Format("{0:dd-MMMM-yyyy}", Date);
                }
                
                return Dailmodel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<TapalReportViewModel> Gettapaltansaction(TapalReportViewModel model)
        {
            try
            {
                List<TapalReportViewModel> tpltranslist = new List<TapalReportViewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from TW in context.tblTapalWorkflow
                                 from T in context.tblTapal
                                 from U in context.tblUser
                                 from C in context.tblCodeControl
                                 join DP in context.tblDepartment on TW.MarkTo equals DP.DepartmentId into D
                                 from c in D.DefaultIfEmpty()
                                 join R in context.tblRole on TW.Role equals R.RoleId into RR
                                 from S in RR.DefaultIfEmpty()
                                 where TW.InwardDateTime >= model.fromdate && TW.InwardDateTime <= model.todate
                                       && (TW.MarkTo == model.departmentid || model.departmentid == 0)
                                       && (TW.Role == model.roleid || model.roleid == 0)
                                       && (TW.UserId == model.id || model.id == 0)
                                       && TW.UserId == U.UserId
                                       && TW.TapalId == T.TapalId && C.CodeName == "TapalAction" && C.CodeValAbbr == TW.TapalAction
                                 select new { TW.InwardDateTime, TW.OutwardDateTime, c.DepartmentName, S.RoleName, U.FirstName, U.LastName, C.CodeValDetail, T.TapalId }).Distinct().ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            var deptname = "";
                            var role = "";
                            var outwarddate = "";
                            if (query[i].OutwardDateTime == null)
                            {
                                outwarddate = "-";
                            }
                            else
                            {
                                outwarddate = String.Format("{0: dd/MM/yyyy   h:mm:ss tt}", query[i].OutwardDateTime);
                            }
                            if (query[i].DepartmentName == null)
                            {
                                deptname = "NA";
                            }
                            else
                            {
                                deptname = query[i].DepartmentName;
                            }
                            if (query[i].RoleName == null)
                            {
                                role = "NA";
                            }
                            else
                            {
                                role = query[i].RoleName;
                            }
                            tpltranslist.Add(new TapalReportViewModel()
                            {
                                InwardDateTime = (DateTime)query[i].InwardDateTime,
                                TapalAction = query[i].CodeValDetail,
                                OutwardDateTime = outwarddate,
                                MarkTo = deptname,
                                Role = role,
                                UserId = query[i].FirstName + ' ' + query[i].LastName,
                                TapalId = query[i].TapalId
                            });
                        }
                    }
                }
                return tpltranslist;
            }
            catch (Exception ex)
            {
                List<TapalReportViewModel> tpltranslist = new List<TapalReportViewModel>();
                return tpltranslist;
            }
        }
        public static List<BOATransactionDetailsModels> BOATransactionDetailsRep(DateTime fromdate, DateTime todate, string projectnumber, string transactiontype)
        {

            List<BOATransactionDetailsModels> boa = new List<BOATransactionDetailsModels>();
            todate = todate.AddDays(1).AddTicks(-2);
            using (var context = new IOASDBEntities())
            {
                var Qry = (from b in context.vw_BOATransactionDetails
                           where ((b.PostedDate >= fromdate && b.PostedDate <= todate) && (String.IsNullOrEmpty(transactiontype) || b.TransactionType.Contains(transactiontype)) && (String.IsNullOrEmpty(b.ProjectNumber) || b.ProjectNumber.Contains(projectnumber)))
                           select new
                           {
                               b.PostedDate,
                               b.CommitmentNumber,
                               b.ProjectNumber,
                               b.HeadName,
                               b.TransactionType,
                               b.Amount
                           }).ToList();

                if (Qry.Count > 0)
                {
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        boa.Add(new BOATransactionDetailsModels()
                        {
                            Amount = Convert.ToDecimal(Qry[i].Amount),
                            PostedDate = Convert.ToDateTime(Qry[i].PostedDate),
                            CommitmentNumber = Qry[i].CommitmentNumber,
                            ProjectNumber = Qry[i].ProjectNumber,
                            HeadName = Qry[i].HeadName,
                            TransactionType = Qry[i].TransactionType
                        });
                    }
                }

            }
            return boa;
        }
        //public static List<CashBookModel> CashBookRep(DateTime fromdate ,DateTime todate)
        //{
        //    List <CashBookModel> cash= new List<CashBookModel>();

        //    return cash;
        //}
        public static List<TrailBalanceModel> TrailBalanceRep(string accounts)
        {
            List<TrailBalanceModel> boa = new List<TrailBalanceModel>();
            using (var context = new IOASDBEntities())
            {
                var Qry = (from b in context.vw_DemoLedgers
                           where b.Accounts == accounts
                           select new
                           {
                               b.AccountGroupId,
                               b.AccountHead,
                               b.AccountHeadId,
                               b.Accounts,
                               b.Amount,
                               b.Creditor_f,
                               b.Debtor_f,
                               b.Groups
                           }).ToList();

                if (Qry.Count > 0)
                {
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        boa.Add(new TrailBalanceModel()
                        {
                            Accounts = Qry[i].Accounts,
                            AccountGroupId = Convert.ToInt32(Qry[i].AccountGroupId),
                            AccountHeadId = Convert.ToInt32(Qry[i].AccountHeadId),
                            AccountHead = Qry[i].AccountHead,
                            Amount = Convert.ToDecimal(Qry[i].Amount),
                            Creditor_f = Convert.ToBoolean(Qry[i].Creditor_f),
                            Debtor_f = Convert.ToBoolean(Qry[i].Debtor_f),
                            Groups = Qry[i].Groups
                        });
                    }
                }

            }
            return boa;
        }
        public static List<CommitmentReportModel> CommitmentRep(DateTime fromdate, DateTime todate, string projecttype, string projectnumber)
        {
            List<CommitmentReportModel> com = new List<CommitmentReportModel>();
            todate = todate.AddDays(1).AddTicks(-2);
            using (var context = new IOASDBEntities())
            {
                var Qry = (from b in context.vw_CommitmentReport
                           where (((b.CommitmentDate >= fromdate && b.CommitmentDate <= todate)) &&
                           (b.ProjectTypeName == projecttype) && (String.IsNullOrEmpty(projectnumber) || b.ProjectNumber.Contains(projectnumber)))
                           orderby b.CommitmentDate descending
                           select new
                           {
                               b.ProjectNumber,
                               b.CommitmentNumber,
                               b.ProjectType,
                               b.ProjectTypeName,
                               b.CommitmentType,
                               b.CommitmentDate,
                               b.CommitmentAmount,
                               b.BookedValue,
                           }).ToList();

                if (Qry.Count > 0)
                {
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        com.Add(new CommitmentReportModel()
                        {
                            CommitmentAmount = Convert.ToDecimal(Qry[i].CommitmentAmount),
                            CommitmentDate = Convert.ToDateTime(Qry[i].CommitmentDate),
                            ProjectNumber = Qry[i].ProjectNumber,
                            CommitmentNumber = Qry[i].CommitmentNumber,
                            ProjectTypeName = Qry[i].ProjectTypeName,
                            CommitmentType = Qry[i].CommitmentType,
                            BookedValue = Convert.ToDecimal(Qry[i].BookedValue)
                        });
                    }
                }

            }
            return com;
        }
        public static InvoiceReportModel GetClaimBillReport(int Id)
        {
            InvoiceReportModel model = new InvoiceReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblProjectInvoice.Where(m => m.InvoiceId == Id).FirstOrDefault();

                    //var QryAgency = context.tblAgencyMaster.Where(m => m.AgencyId == AgencyId).FirstOrDefault();

                    var QryInvoiceTax = context.tblInvoiceTaxDetails.Where(m => m.InvoiceId == Id).FirstOrDefault();
                    if (Qry != null)
                    {
                        int AgencyId = Qry.AgencyId ?? 0;
                        int AgencyRegState = Qry.AgencyRegState ?? 0;
                        var QryState = context.tblStateMaster.Where(m => m.StateId == AgencyRegState).FirstOrDefault();
                        int InvoiceId = Qry.InvoiceId;
                        CultureInfo Indian = new CultureInfo("hi-IN");
                        model.InvoiceNo = Qry.InvoiceNumber;
                        model.InvoiceDate = string.Format("{0:dd-MMM-yyyy}", Qry.InvoiceDate);
                        int ProjId = Qry.ProjectId ?? 0;
                        model.ProjectNumber = context.tblProject.Where(m => m.ProjectId == ProjId).Select(m => m.ProjectNumber).FirstOrDefault();
                        int PIId = Qry.PIId ?? 0;
                        model.DepartmentName = context.vwFacultyStaffDetails.Where(m => m.UserId == PIId).Select(m => m.DepartmentName).FirstOrDefault();
                        model.PIName = context.vwFacultyStaffDetails.Where(m => m.UserId == PIId).Select(m => m.FirstName).FirstOrDefault();
                        model.SACNumber = Convert.ToString(Qry.TaxCode);
                        model.DescriptionofServices = Qry.DescriptionofServices;

                        model.IITMGSTIN = "33AAAAI3615G1Z6";
                        model.Name = Qry.AgencyRegName;
                        model.Address = Qry.CommunicationAddress;
                        model.GSTIN = Qry.AgencyRegGSTIN;
                        model.PANNo = Qry.AgencyRegPAN;
                        model.District = Qry.AgencyDistrict;
                        model.PinCode = Qry.AgencyPincode.ToString();
                        model.TANNo = Qry.AgencyRegTAN;
                        model.Email = Qry.AgencyContactPersonEmail;
                        model.ContactPerson = Qry.AgencyContactPersonName;
                        model.ContactNo = Qry.AgencyContactPersonNumber;
                        //if (QryAgency != null)
                        //{
                        //    model.District = QryAgency.District;
                        //    model.PinCode = Convert.ToString(QryAgency.PinCode);
                        //    model.TANNo = QryAgency.TAN;
                        //    model.Email = QryAgency.ContactEmail;
                        //    model.ContactPerson = QryAgency.ContactPerson;
                        //    model.ContactNo = QryAgency.ContactNumber;
                        //}
                        if (QryState != null)
                        {
                            model.State = QryState.StateName ?? "";
                            model.StateCode = QryState.StateCode ?? "";
                        }

                        decimal ttlVal = Qry.TotalInvoiceValue ?? 0;
                        model.TaxableValue1 = String.Format(Indian, "{0:N}", Qry.TaxableValue);
                        model.Amount = String.Format(Indian, "{0:N}", ttlVal);
                        model.TotalInvoiceValueInWords = CoreAccountsService.words(ttlVal);

                        model.ACName = "The Registrar, IIT Madras";
                        model.ACNo = "2722101016162";
                        model.BankName = "Canara Bank";
                        model.BranchName = "IIT-Madras Branch";
                        model.IFSC = "CNRB0002722";
                        model.MICRCode = "600015085";
                        model.SWIFTCode = "CNRBINBBIIT";
                    }
                    
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #region  ProcessWorkFlowReport
        public static List<ProcessGuideLineListReportModel> GetApprovalDetails(int id)
        {
            List<ProcessGuideLineListReportModel> model = new List<ProcessGuideLineListReportModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {

                    var qry = (from b in context.vw_ApprovalProcessFlowDetails
                               where b.ApproverId == id && b.ActionStatus == "Initiated"
                               group b by b.ProcessTransactionId into grp
                               select new
                               {
                                   //ApproverId = grp.Key,
                                   ProcessTransactionId = grp.Select(m => m.ProcessTransactionId).FirstOrDefault()
                               }).ToList();

                    for (int i = 0; i < qry.Count; i++)
                    {
                        int ProcId = qry[i].ProcessTransactionId ?? 0;
                        var Pending = context.vw_ApprovalProcessFlowDetails.Where(m => m.ProcessTransactionId == ProcId && m.closed_f == false && m.ActionStatus == "Pending").Select(m => m.ProcessTransactionId ?? 0).FirstOrDefault();
                        // for (int k = 0; k < Pending.Count; k++)
                        // {
                        // int PendingId = Pending.FirstOrDefault();
                        var Query = (from b in context.vw_ApprovalProcessFlowDetails
                                     where b.ProcessTransactionId == Pending
                                     select new
                                     {
                                         b.Name,
                                         b.ActionStatus,
                                         b.FunctionName,
                                         b.RefNumber,
                                         b.TransactionTS,
                                         b.ApproverLevel,
                                         b.InitiatedTS
                                     }).OrderBy(m => m.TransactionTS ?? DateTime.MaxValue).ThenBy(x => x.ApproverLevel).ToList();

                        // Query = Query.Union(Query).ToList();
                        for (int j = 0; j < Query.Count; j++)
                        {
                            model.Add(new ProcessGuideLineListReportModel()
                            {
                                Action = Query[j].ActionStatus,
                                Name = Query[j].Name,
                                FunctionName = Query[j].FunctionName,
                                RefNo = Query[j].RefNumber,
                                Date = String.Format("{0:dd-MMMM-yyyy}", Query[j].TransactionTS),
                                InitatedDate = String.Format("{0:dd-MMMM-yyyy}", Query[j].InitiatedTS)
                            });
                        }
                        // }
                    }
                    return model;
                }

            }
            catch (Exception ex)
            {
                return model;
            }
        }
        public static List<ProcessGuideLineHeadingModel> GetHeading(int id)
        {
            List<ProcessGuideLineHeadingModel> model = new List<ProcessGuideLineHeadingModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var qry = (from b in context.vw_ApprovalProcessFlowDetails
                               where b.ApproverId == id
                               select new
                               {
                                   b.RefNumber,
                                   b.FunctionName,
                                   b.ProcessTransactionId,
                                   b.InitiatedTS

                               }).ToList();
                    for (int i = 0; i < qry.Count; i++)
                    {
                        model.Add(new ProcessGuideLineHeadingModel()
                        {
                            Head = qry[i].FunctionName,
                            RefNumber = qry[i].RefNumber,
                            //  Date= qry[i].InitiatedTS,
                        });
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region Approverwise Pending Report
        public static List<ProcessGuideLineListReportModel> GetApproverwisePendingDetails(int id)
        {
            List<ProcessGuideLineListReportModel> model = new List<ProcessGuideLineListReportModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {

                    var qry = (from b in context.vw_ApprovalProcessFlowDetails
                               where b.ApproverId == id && b.ActionStatus == "Pending" && b.closed_f == false
                               group b by b.ProcessTransactionId into grp
                               select new
                               {
                                   //ApproverId = grp.Key,
                                   ProcessTransactionId = grp.Select(m => m.ProcessTransactionId).FirstOrDefault()
                               }).ToList();

                    for (int i = 0; i < qry.Count; i++)
                    {
                        int ProcId = qry[i].ProcessTransactionId ?? 0;
                        var Pending = context.vw_ApprovalProcessFlowDetails.Where(m => m.ProcessTransactionId == ProcId).Select(m => m.ProcessTransactionId ?? 0).FirstOrDefault();

                        var Query = (from b in context.vw_ApprovalProcessFlowDetails
                                     where b.ProcessTransactionId == Pending
                                     //where b.ActionStatus == "Pending" && b.ApproverId != id
                                     select new
                                     {
                                         b.Name,
                                         b.ActionStatus,
                                         b.FunctionName,
                                         b.RefNumber,
                                         b.TransactionTS,
                                         b.ApproverLevel,
                                         b.ApproverId,
                                         b.ActionLink,
                                         b.InitiatedTS
                                     }).OrderBy(m => m.TransactionTS ?? DateTime.MaxValue).ThenBy(x => x.ApproverLevel).ToList();
                        var CurrentAprLevel = Query.Where(m => m.ApproverId == id).Select(m => m.ApproverLevel).FirstOrDefault();
                        var FurtherPending = Query.Where(m => m.ActionStatus == "Pending" && m.ApproverId != id).ToList();
                        bool Condition = false;
                        if (FurtherPending != null || FurtherPending.Count > 0)
                        {
                            for (int k = 0; k < FurtherPending.Count; k++)
                            {
                                if (CurrentAprLevel > FurtherPending[k].ApproverLevel)
                                {
                                    Condition = true;
                                }
                            }
                        }
                        if (Condition == false)
                        {
                            for (int j = 0; j < Query.Count; j++)
                            {



                                model.Add(new ProcessGuideLineListReportModel()
                                {
                                    Action = Query[j].ActionStatus,
                                    Name = Query[j].Name,
                                    FunctionName = Query[j].FunctionName,
                                    RefNo = Query[j].RefNumber,
                                    Date = String.Format("{0:dd-MMMM-yyyy}", Query[j].TransactionTS),
                                    ActionLink = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + Query[j].ActionLink,

                                    InitatedDate = String.Format("{0:dd-MMMM-yyyy}", Query[j].InitiatedTS)
                                });

                            }
                        }
                    }
                    return model;
                }

            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region Provisional Statement
        public static ProvisionalStatementReportModel GetProvisionalStatement(string FromDate, string ToDate, int ProjectId)
        {
            ProvisionalStatementReportModel model = new ProvisionalStatementReportModel();
            try
            {
                DateTime From = Convert.ToDateTime(FromDate);
                DateTime To = Convert.ToDateTime(ToDate);
                To = To.AddDays(1).AddTicks(-2);
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblProject.Where(m => m.ProjectId == ProjectId).FirstOrDefault();
                    decimal qryOpeningBal = context.tblProjectOB.Where(m => m.ProjectId == ProjectId).Sum(m => m.OpeningBalance) ?? 0;
                    var QryReceipt = (from C in context.tblReceipt where C.ProjectId == ProjectId && C.Posted_f == true && C.CategoryId != 16 && C.CrtdTS >= From && C.CrtdTS <= To && C.Status == "Completed" select C).ToList();
                    decimal receiptAmt = QryReceipt.Sum(m => m.ReceiptAmount ?? 0);
                    decimal CGST = QryReceipt.Sum(m => m.CGST ?? 0);
                    decimal SGST = QryReceipt.Sum(m => m.SGST ?? 0);
                    decimal IGST = QryReceipt.Sum(m => m.IGST ?? 0);
                    decimal GST = CGST + SGST + IGST;
                    receiptAmt = (receiptAmt) - (GST);
                    decimal? spentAmt = 0;
                    var qrySpenAmt = (from C in context.vw_ProjectExpenditureReport.AsNoTracking() where C.ProjectId == ProjectId && C.Posted_f == true && C.CommitmentDate >= From && C.CommitmentDate <= To select C.AmountSpent).Sum() ?? 0;
                    spentAmt = qrySpenAmt;
                    var OBQryReceipt = (from C in context.tblReceipt where C.ProjectId == ProjectId && C.Posted_f == true && C.CrtdTS < From && C.Status == "Completed" select C).ToList();
                    decimal OBreceiptAmt = OBQryReceipt.Sum(m => m.ReceiptAmount ?? 0);
                    decimal OBCGST = OBQryReceipt.Sum(m => m.CGST ?? 0);
                    decimal OBSGST = OBQryReceipt.Sum(m => m.SGST ?? 0);
                    decimal OBIGST = OBQryReceipt.Sum(m => m.IGST ?? 0);
                    decimal OBGST = OBCGST + OBSGST + OBIGST;
                    OBreceiptAmt = (OBreceiptAmt) - (OBGST);
                    decimal? OBspentAmt = 0;
                    var OBqrySpenAmt = (from C in context.vw_ProjectExpenditureReport.AsNoTracking() where C.ProjectId == ProjectId && C.Posted_f == true && C.CommitmentDate < From select C.AmountSpent).Sum() ?? 0;
                    OBspentAmt = OBqrySpenAmt;
                    decimal OpeningBal = (qryOpeningBal + (OBreceiptAmt - OBspentAmt)) ?? 0;
                    int[] headid = { 1, 2, 3, 4, 5, 6, 7, 8 };
                    decimal Staff = (from C in context.vw_ProjectExpenditureReport.AsNoTracking() where C.ProjectId == ProjectId && C.Posted_f == true && C.AllocationHeadId == 1 && C.CommitmentDate >= From && C.CommitmentDate <= To select C.AmountSpent).Sum() ?? 0;
                    decimal Consumables = (from C in context.vw_ProjectExpenditureReport.AsNoTracking() where C.ProjectId == ProjectId && C.Posted_f == true && C.AllocationHeadId == 2 && C.CommitmentDate >= From && C.CommitmentDate <= To select C.AmountSpent).Sum() ?? 0;
                    decimal Equipment = (from C in context.vw_ProjectExpenditureReport.AsNoTracking() where C.ProjectId == ProjectId && C.Posted_f == true && C.AllocationHeadId == 7 && C.CommitmentDate >= From && C.CommitmentDate <= To select C.AmountSpent).Sum() ?? 0;
                    decimal Contingencies = (from C in context.vw_ProjectExpenditureReport.AsNoTracking() where C.ProjectId == ProjectId && C.Posted_f == true && C.AllocationHeadId == 3 && C.CommitmentDate >= From && C.CommitmentDate <= To select C.AmountSpent).Sum() ?? 0;
                    decimal Travel = (from C in context.vw_ProjectExpenditureReport.AsNoTracking() where C.ProjectId == ProjectId && C.Posted_f == true && C.AllocationHeadId == 4 && C.CommitmentDate >= From && C.CommitmentDate <= To select C.AmountSpent).Sum() ?? 0;
                    decimal Components = (from C in context.vw_ProjectExpenditureReport.AsNoTracking() where C.ProjectId == ProjectId && C.Posted_f == true && C.AllocationHeadId == 5 && C.CommitmentDate >= From && C.CommitmentDate <= To select C.AmountSpent).Sum() ?? 0;
                    decimal InsOverhead = (from C in context.vw_ProjectExpenditureReport.AsNoTracking() where C.ProjectId == ProjectId && C.Posted_f == true && C.AllocationHeadId == 6 && C.CommitmentDate >= From && C.CommitmentDate <= To select C.AmountSpent).Sum() ?? 0;
                    decimal Others = (from C in context.vw_ProjectExpenditureReport.AsNoTracking() where C.ProjectId == ProjectId && C.Posted_f == true && C.AllocationHeadId == 8 && C.CommitmentDate >= From && C.CommitmentDate <= To select C.AmountSpent).Sum() ?? 0;
                    decimal Miscellaneous = (from C in context.vw_ProjectExpenditureReport.AsNoTracking() where C.ProjectId == ProjectId && C.Posted_f == true && !headid.Contains(C.AllocationHeadId ?? 0) && C.CommitmentDate >= From && C.CommitmentDate <= To select C.AmountSpent).Sum() ?? 0;
                    var qryPreviousCommit = (from C in context.tblCommitment
                                             join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                             where C.ProjectId == ProjectId && C.Status == "Active"
                                             select new { D.BalanceAmount, D.ReversedAmount }).ToList();
                    var BalanceAmt = (qryPreviousCommit.Select(m => m.BalanceAmount).Sum() ?? 0) +
                        ((from C in context.vwCommitmentSpentBalance where C.ProjectId == ProjectId && (C.Posted_f == false || C.Posted_f == null) select C.AmountSpent).Sum() ?? 0);

                    decimal? Interest = 0;
                    Interest = (from C in context.tblReceipt where C.ProjectId == ProjectId && C.Posted_f == true && C.CategoryId == 16 && C.CrtdTS >= From && C.CrtdTS <= To && C.Status == "Completed" select C.ReceiptAmount).Sum();

                    if (Qry.SponsoringAgency > 0)
                        model.SponseringAgency = context.tblAgencyMaster.Where(m => m.AgencyId == Qry.SponsoringAgency).Select(m => m.AgencyName).FirstOrDefault();
                    model.ProjectNo = Qry.ProjectNumber;
                    model.ProjectTitle = Qry.ProjectTitle;
                    model.SanctionNo = Qry.SanctionOrderNumber;
                    model.FromDate = String.Format("{0:dd-MMMM-yyyy}", FromDate);
                    model.ToDate = String.Format("{0:dd-MMMM-yyyy}", ToDate);
                    model.ProjCor = context.vwFacultyStaffDetails.Where(m => m.UserId == Qry.PIName).Select(m => m.FirstName).FirstOrDefault();
                    model.OB = OpeningBal;
                    model.Receipt = receiptAmt;
                    model.Interest = Interest ?? 0;
                    model.ReceiptTotal = model.OB + model.Receipt + model.Interest;
                    model.Staff = Staff;
                    model.Consumables = Consumables;
                    model.Equipment = Equipment;
                    model.Contingency = Contingencies;
                    model.Travel = Travel;
                    model.Components = Components;
                    model.Overheads = InsOverhead;
                    model.Others = Others;
                    model.Miscellaneous = Miscellaneous;
                    model.TotalExpenditure = context.vw_ProjectExpenditureReport.Where(m => m.ProjectId == ProjectId && m.Posted_f == true && m.CommitmentDate >= From && m.CommitmentDate <= To).Sum(m => m.AmountSpent) ?? 0;
                    model.Balance = model.ReceiptTotal - model.TotalExpenditure;
                    model.BalanceCommitments = BalanceAmt;
                    model.NetBalance = model.Balance - model.BalanceCommitments;
                }
                
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region CashBook
        public static List<CashBookPayModel> CashBookPaymentRep(DateTime fromdate, DateTime todate, int BankId)
        {
            List<CashBookPayModel> boa = new List<CashBookPayModel>();
            //  todate = todate.AddDays(1).AddTicks(-2);
            using (var context = new IOASDBEntities())
            {
                boa = (from c in context.vw_CashBookPayment
                       where c.ReferenceDate >= fromdate && c.ReferenceDate <= todate && c.BankHeadID == BankId
                       orderby c.ReferenceDate
                       select c).AsEnumerable()
                            .Select((x, index) => new CashBookPayModel()
                            {
                                Amount = Convert.ToDecimal(x.Amount),
                                BankHeadID = Convert.ToInt32(x.BankHeadID),
                                BOAId = Convert.ToInt32(x.BOAId),
                                BOAPaymentDetailId = Convert.ToInt32(x.BOAPaymentDetailId),
                                TransactionType = x.TransactionType,
                                PayeeBank = x.PayeeBank,
                                PayeeName = x.PayeeName,
                                ReferenceDate = String.Format("{0:dd-MMMM-yyyy}", x.ReferenceDate),
                                VoucherNumber = x.VoucherNumber,
                                VoucherPayee = x.VoucherPayee,
                                TempVoucherNo = x.TempVoucherNumber
                            }).ToList();
                //if (Qry.Count > 0)
                //{
                //    for (int i = 0; i < Qry.Count; i++)
                //    {
                //        boa.Add(new CashBookPayModel()
                //        {
                //            Amount = Convert.ToDecimal(Qry[i].Amount),
                //            BankHeadID = Convert.ToInt32(Qry[i].BankHeadID),
                //            BOAId = Convert.ToInt32(Qry[i].BOAId),
                //            BOAPaymentDetailId = Convert.ToInt32(Qry[i].BOAPaymentDetailId),
                //            TransactionType = Qry[i].TransactionType,
                //            PayeeBank = Qry[i].PayeeBank,
                //            PayeeName = Qry[i].PayeeName,
                //            ReferenceDate = String.Format("{0:dd-MMMM-yyyy}", Qry[i].ReferenceDate),
                //            VoucherNumber = Qry[i].VoucherNumber,
                //            VoucherPayee = Qry[i].VoucherPayee,
                //            TempVoucherNo = Qry[i].TempVoucherNumber
                //        });
                //    }
                //}

            }
            return boa;
        }
        public static List<CashBookRecModel> CashBookReceiptRep(DateTime fromdate, DateTime todate, int BankId)
        {
            List<CashBookRecModel> boa = new List<CashBookRecModel>();
            //   todate = todate.AddDays(1).AddTicks(-2);
            using (var context = new IOASDBEntities())
            {
                boa = (from c in context.vw_CashBookReceipt
                       where c.ReferenceDate >= fromdate && c.ReferenceDate <= todate && c.BankHeadID == BankId
                       orderby c.ReferenceDate
                       select c)
                           .AsEnumerable()
                            .Select((x, index) => new CashBookRecModel()
                            {
                                Amount = Convert.ToDecimal(x.Amount),
                                BankHeadID = Convert.ToInt32(x.BankHeadID),
                                BOAId = Convert.ToInt32(x.BOAId),
                                BOAPaymentDetailId = Convert.ToInt32(x.BOAPaymentDetailId),
                                TransactionType = x.TransactionType,
                                PayeeBank = x.PayeeBank,
                                PayeeName = x.PayeeName,
                                ReferenceDate = String.Format("{0:dd-MMMM-yyyy}", x.ReferenceDate),
                                VoucherNumber = x.VoucherNumber,
                                VoucherPayee = x.VoucherPayee,
                                TempVoucherNo = x.TempVoucherNumber,
                                ChequeNo = x.ChequeNumber
                            }).ToList();

                ////if (Qry.Count > 0)
                ////{
                ////    for (int i = 0; i < Qry.Count; i++)
                ////    {
                ////        boa.Add(new CashBookRecModel()
                ////        {
                ////            Amount = Convert.ToDecimal(Qry[i].Amount),
                ////            BankHeadID = Convert.ToInt32(Qry[i].BankHeadID),
                ////            BOAId = Convert.ToInt32(Qry[i].BOAId),
                ////            BOAPaymentDetailId = Convert.ToInt32(Qry[i].BOAPaymentDetailId),
                ////            TransactionType = Qry[i].TransactionType,
                ////            PayeeBank = Qry[i].PayeeBank,
                ////            PayeeName = Qry[i].PayeeName,
                ////            ReferenceDate = String.Format("{0:dd-MMMM-yyyy}", Qry[i].ReferenceDate),
                ////            VoucherNumber = Qry[i].VoucherNumber,
                ////            VoucherPayee = Qry[i].VoucherPayee,
                ////            TempVoucherNo = Qry[i].TempVoucherNumber,
                ////            ChequeNo = Qry[i].ChequeNumber
                ////        });
                ////    }
                ////}

            }
            return boa;
        }
        #endregion

        #region Invoice Report
        public static InvoiceReportModel GetInvoiceReport(int Id)
        {
            InvoiceReportModel model = new InvoiceReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblProjectInvoice.Where(m => m.InvoiceId == Id).FirstOrDefault();
                    var QryAgency = context.tblAgencyMaster.Where(m => m.AgencyId == Qry.AgencyId).FirstOrDefault();
                    var QryState = context.tblStateMaster.Where(m => m.StateId == Qry.AgencyRegState).FirstOrDefault();
                    var QryInvoiceTax = context.tblInvoiceTaxDetails.Where(m => m.InvoiceId == Qry.InvoiceId).FirstOrDefault();
                    if (Qry != null)
                    {
                        model.InvoiceNo = Qry.InvoiceNumber;
                        int ProjId = Qry.ProjectId ?? 0;
                        int PIId = Qry.PIId ?? 0;
                        model.InvoiceDate = string.Format("{0:dd-MMM-yyyy}", Qry.InvoiceDate);
                        model.ProjectNumber = context.tblProject.Where(m => m.ProjectId == Qry.ProjectId).Select(m => m.ProjectNumber).FirstOrDefault();
                        model.SignNR_f = Common.CheckIsSAIFProject(ProjId);
                        if (model.SignNR_f)
                            model.DepartmentName = "Sophisticated Analytical Instrumentation Facility";
                        else
                            model.DepartmentName = context.vwFacultyStaffDetails.Where(m => m.UserId == PIId).Select(m => m.DepartmentName).FirstOrDefault();
                        model.PIName = context.vwFacultyStaffDetails.Where(m => m.UserId == Qry.PIId).Select(m => m.FirstName).FirstOrDefault();
                        model.SACNumber = Convert.ToString(Qry.TaxCode);
                        model.DescriptionofServices = Qry.DescriptionofServices;
                        model.TaxableValue = Qry.TaxableValue ?? 0;
                        model.IITMGSTIN = "33AAAAI3615G1Z6";
                        model.Name = Qry.AgencyRegName;
                        model.Address = Qry.CommunicationAddress;
                        model.GSTIN = Qry.AgencyRegGSTIN;
                        model.PANNo = Qry.AgencyRegPAN;
                        if (QryAgency != null)
                        {
                            model.District = QryAgency.District;
                            model.PinCode = Convert.ToString(QryAgency.PinCode);
                            model.TANNo = QryAgency.TAN;
                            model.Email = QryAgency.ContactEmail;
                            model.ContactPerson = QryAgency.ContactPerson;
                            model.ContactNo = QryAgency.ContactNumber;
                        }
                        if (QryState != null)
                        {
                            model.State = QryState.StateName ?? "";
                            model.StateCode = QryState.StateCode ?? "";
                        }
                        if (QryInvoiceTax != null)
                        {
                            model.SGST = QryInvoiceTax.SGSTAmount ?? 0;
                            model.CGST = QryInvoiceTax.CGSTAmount ?? 0;
                            model.IGST = QryInvoiceTax.IGSTAmount ?? 0;
                        }
                        model.TotalInvoiceValue = model.TaxableValue + model.SGST + model.CGST + model.IGST;
                        if (Qry.InvoiceType == 1)
                            model.Amount = Convert.ToString(Qry.TotalInvoiceValue + " " + Qry.CurrencyCode);
                        else
                            model.Amount = Convert.ToString(model.TotalInvoiceValue);
                        model.TotalInvoiceValueInWords = CoreAccountsService.words(model.TotalInvoiceValue);
                        model.ACName = "The Registrar, IIT Madras";
                        model.ACNo = "2722101016162";
                        model.BankName = "Canara Bank";
                        model.BranchName = "IIT-Madras Branch";
                        model.IFSC = "CNRB0002722";
                        model.MICRCode = "600015085";
                        model.SWIFTCode = "CNRBINBBIIT";
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region Receipt Voucher
        public static ReceiptVoucherModel GetReceiptVoucher(int Id)
        {
            ReceiptVoucherModel model = new ReceiptVoucherModel();
            ProjectSummaryModel model1 = new ProjectSummaryModel();
            ProjectService projser = new ProjectService();
            List<ReceiptJournal> Journal = new List<ReceiptJournal>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblReceipt.Where(m => m.ReceiptId == Id).FirstOrDefault();
                    model.ReceiptNo = Qry.ReceiptNumber;
                    model.ReceiptId = Id;
                    int invoiceid = Qry.InvoiceId ?? 0;


                    model.ReceiptDate = string.Format("{0:dd-MMMM-yyyy}", Qry.CrtdTS);
                    int ProjId = Qry.ProjectId ?? 0;
                    var ProDet = context.tblProject.Where(m => m.ProjectId == ProjId).FirstOrDefault();
                    model1 = projser.getProjectSummary(ProjId);
                    model.projsumm = model1;
                    int CreditjouId = Qry.ReceivedFrom ?? 0;
                    var InvDet = context.tblProjectInvoice.Where(m => m.InvoiceId == invoiceid).FirstOrDefault();
                    if (InvDet != null)
                    {
                        model.InvoiceNo = InvDet.InvoiceNumber;
                        model.InvoiceAmt = InvDet.TotalInvoiceValue ?? 0;
                        model.projsumm.AgencyName = InvDet.AgencyRegName;
                        model.projsumm.AgencyAddress = InvDet.CommunicationAddress;
                        model.InvoiceValueInFRNCurrency = InvDet.InvoiceValueinForeignCurrency ?? 0;
                        model.InvoiceCurrencyCode = InvDet.CurrencyCode;
                    }

                    int PIid = 0;
                    if (ProDet != null)
                    {
                        model.ProjectNo = ProDet.ProjectNumber;
                        PIid = ProDet.PIName ?? 0;
                    }
                    var PIDet = context.vwFacultyStaffDetails.Where(m => m.UserId == PIid).FirstOrDefault();
                    if (PIDet != null)
                    {
                        model.PIName = PIDet.FirstName;
                        model.Dept = PIDet.DepartmentName;
                    }
                    model.Description = Qry.Description;
                    model.BillRefNumber = Qry.ReferenceNumber;
                    model.ReceiptAmt = Qry.ReceiptAmount ?? 0;
                    List<Receivables> RecList = new List<Receivables>();
                    var List = (from e in context.tblReceiptRecivables
                                join a in context.tblAccountHead on e.ReceivablesHeadId equals a.AccountHeadId
                                where e.ReceiptId == Id && e.ReceivabesAmount > 0
                                select new
                                {
                                    e.ReceivabesAmount,
                                    a.AccountHead
                                }).AsEnumerable()
                                          .Select((x) => new Receivables()
                                          {
                                              Head = x.AccountHead,
                                              Amount = Convert.ToDecimal(x.ReceivabesAmount)
                                          }).ToList();
                    var journallist = (from e in context.tblReceiptRecivables
                                       join a in context.tblAccountHead on e.ReceivablesHeadId equals a.AccountHeadId

                                       where e.ReceiptId == Id && e.ReceivabesAmount > 0
                                       select new
                                       {
                                           e.ReceivabesAmount,
                                           a.AccountHead,
                                           e.TransactionType
                                       }).ToList();
                    if (journallist.Count > 0)
                    {
                        for (int i = 0; i < journallist.Count; i++)
                        {
                            Journal.Add(new ReceiptJournal
                            {
                                Head = journallist[i].AccountHead,
                                Amount = Convert.ToDecimal(journallist[i].ReceivabesAmount),
                                TransactionType = journallist[i].TransactionType ?? "Debit"
                            });
                        }
                    }
                    model.BankAmt = Qry.BankAmountDr ?? 0;
                    int BankId = Qry.BankAccountHeadDr ?? 0;

                    model.Bank = context.tblAccountHead.Where(m => m.AccountHeadId == BankId).Select(m => m.AccountHead).FirstOrDefault();

                    Journal.Add(new ReceiptJournal
                    {
                        Head = model.Bank,
                        Amount = model.BankAmt,
                        TransactionType = Qry.ReceiptAmount > 0 ? "Debit" : "Credit"
                    });

                    string Creditjou = context.tblAccountHead.Where(m => m.AccountHeadId == 10).Select(m => m.AccountHead).FirstOrDefault();
                    if (CreditjouId > 0)
                    {
                        Journal.Add(new ReceiptJournal
                        {
                            Head = Creditjou,
                            Amount = Qry.ReceivedAmount ?? 0,
                            TransactionType = "Credit"
                        });
                    }

                    model.Journal = Journal;
                    // model.Description=
                    model.Category = Common.GetCodeControlName(Qry.CategoryId ?? 0, "ReceiptCategory");
                    model.RecList = List;
                    var TransDet = context.tblReceiptTransactionDetails.Where(m => m.ReceiptId == Id).FirstOrDefault();
                    if (TransDet != null)
                    {
                        model.TransNo = TransDet.TransactionNumber;
                        model.TransDate = string.Format("{0:dd-MMMM-yyyy}", TransDet.TransactionDate);

                    }

                   
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion
        #region ProjectTransaction
        public static ProjectTransactionModel ProjectTransaction(int Id)
        {
            ProjectTransactionModel model = new ProjectTransactionModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    decimal TotalAmt = context.vw_OverallProjectTransaction.Where(m => m.ProjectId == Id).Sum(m => m.RunningTotal) ?? 0;
                    model.TotalAmt = TotalAmt;
                    decimal NegBal = context.tblNegativeBalance.Where(m => m.ProjectId == Id && m.Status == "Approved").Sum(m => m.NegativeBalanceAmount) ?? 0;
                    model.NegBal = NegBal;
                    model.NetBalance = (model.TotalAmt + model.NegBal);
                    var ProjDet = context.tblProject.Where(m => m.ProjectId == Id).FirstOrDefault();
                    model.Title = ProjDet.ProjectTitle;
                    model.SantionedValue = ProjDet.SanctionValue ?? 0;
                    int PiId = ProjDet.PIName ?? 0;
                    model.PI = context.vwFacultyStaffDetails.Where(m => m.UserId == PiId).Select(m => m.FirstName).FirstOrDefault();
                    var copi = new List<string>();
                    var query = (from a in context.tblProject
                                 join b in context.tblProjectCoPI on a.ProjectId equals b.ProjectId
                                 join c in context.vwFacultyStaffDetails on b.Name equals c.UserId
                                 where b.ProjectId == Id
                                 select new { b.ProjectId, a.ProjectNumber, c.FirstName }).ToList();
                    if (query != null)
                    {
                        for (int i = 0; i < query.Count(); i++)
                        {
                            copi.Add(query[i].FirstName);
                        }
                    }
                    model.Copi = copi;
                    model.ProjectNo = ProjDet.ProjectNumber;
                    
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }

        }
        #endregion

        #region Long Bill
        public static LongBill GetLongBill(int Id = 0)
        {
            LongBill model = new LongBill();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblSalaryPayment.Where(m => m.PaymentHeadId == Id).ToList();
                    int[] PaymentId = Qry.Select(m => m.PaymentId).ToArray();
                    model.BASICPAY = Qry.Sum(m => m.Basic) ?? 0;
                    model.MEDINS = Qry.Sum(m => m.MA) ?? 0;
                    model.MEDCHARG = Qry.Sum(m => m.MedicalRecovery) ?? 0;
                    model.MISCPAYNT = Qry.Sum(m => m.DirectAllowance) ?? 0;
                    model.CFACC = Qry.Sum(m => m.HRA) ?? 0;
                    model.CCALL = 0;
                    model.OTHERPAY = Qry.Sum(m => m.OtherAllowance) ?? 0;
                    model.OTALL = 0;
                    model.TRANSALL = 0;

                    model.LIC = 0;
                    model.INCOMETAX = Qry.Sum(m => m.MonthlyTax) ?? 0;
                    model.TEMPLE = 0;
                    model.AYYAPPA = 0;
                    model.MISCRECVY = context.tblAdhocSalaryBreakUpDetail.Where(m => m.HeadId == 4 && PaymentId.Contains(m.PaymentId ?? 0)).Sum(m => m.Amount) ?? 0;
                    model.PEELI = 0;
                    model.CANBANK = 0;
                    model.PAYRECVRY = 0;
                    model.PROFLTAX = Qry.Sum(m => m.ProfTax) ?? 0;
                    model.COURT = 0;
                    model.PMRELFND = 0;
                    model.ITOTHPAY = 0;
                    model.PAYMENTS = (model.BASICPAY + model.MISCPAYNT + model.CFACC + model.CCALL +
                        model.OTHERPAY + model.OTALL + model.TRANSALL);
                    model.RECOVERIES = (model.LIC + model.INCOMETAX + model.TEMPLE + model.AYYAPPA + model.MISCRECVY + model.PEELI + model.CANBANK +
                        model.MEDCHARG + model.PAYRECVRY + model.PROFLTAX + model.COURT + model.PMRELFND + model.ITOTHPAY);
                    model.NETPAYABLE = model.PAYMENTS - model.RECOVERIES;
                    model.CANARABANK = context.vw_CanaraBankForsalary.Where(m => m.SalaryId == Id).Sum(m => m.Amount) ?? 0;
                    model.OTHERBANK = context.vw_NonCanaraBankForsalary.Where(m => m.SalaryId == Id).Sum(m => m.Amount) ?? 0;
                    model.CHEQUE = 0;
                    model.IITMPENSIONER = 0;
                    model.AmtWords = CoreAccountsService.words(model.NETPAYABLE);
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion
        #region Form 24Q
        public static Form24QModel GetForm26Q(DateTime Date)
        {

            Form24QModel model = new Form24QModel();
            List<TDS94CModel> TDS94C = new List<TDS94CModel>();
            List<TDS94JModel> TDS94J = new List<TDS94JModel>();
            List<TDS94HModel> TDS94H = new List<TDS94HModel>();
            List<TDS94IModel> TDS94I = new List<TDS94IModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    int[] _94C = { 39, 330, 350, 373 };
                    string[] status = { "Approved", "Completed" };
                    model.TDS94C = (from a in context.tblTDSPayment
                                    join b in context.tblTDSIncomeTaxDetails on a.tblTDSPaymentId equals b.tblTDSPaymentId
                                    join c in context.tblTDSDetails on new { DetId = b.DetailId, Refno = b.ReferenceNo, AccId = a.Section } equals new { DetId = c.Detailid, Refno = c.ReferenceNumber, AccId = c.AccountHeadId } into g
                                    from c in g.DefaultIfEmpty()
                                    where status.Contains(a.Status) && _94C.Contains(a.Section ?? 0) && a.ToDate == Date
                                    select new
                                    {
                                        a.BankReferenceNo,
                                        a.Section,
                                        c.Name,
                                        c.PAN,
                                        b.Amount,
                                        c.Basic,
                                        c.PostedDate,
                                        a.DateOfPayment,
                                        a.ChellanNo,
                                        a.FromDate
                                    }).AsEnumerable()
                                 .Select((x, index) => new TDS94CModel()
                                 {
                                     SlNo = index + 1,
                                     Name = x.Name,
                                     PAN = x.PAN,
                                     AmountPaid = x.Basic ?? 0,
                                     TDS = x.Amount ?? 0,
                                     DateofPayment = Convert.ToDateTime(x.PostedDate),
                                     Rate = Common.GetTDSPercentage(x.Section ?? 0),
                                     BSRCode = x.BankReferenceNo,
                                     ChallenNo = x.ChellanNo
                                 }).ToList();
                    int[] _94I = { 42, 331, 375, 376 };
                    model.TDS94I = (from a in context.tblTDSPayment
                                    join b in context.tblTDSIncomeTaxDetails on a.tblTDSPaymentId equals b.tblTDSPaymentId
                                    join c in context.tblTDSDetails on new { DetId = b.DetailId, Refno = b.ReferenceNo, AccId = a.Section } equals new { DetId = c.Detailid, Refno = c.ReferenceNumber, AccId = c.AccountHeadId } into g
                                    from c in g.DefaultIfEmpty()
                                    where status.Contains(a.Status) && _94I.Contains(a.Section ?? 0) && a.ToDate == Date
                                    select new
                                    {
                                        a.BankReferenceNo,

                                        a.Section,
                                        b.Party,
                                        b.PAN,
                                        b.Amount,
                                        c.Basic,
                                        c.PostedDate,
                                        a.DateOfPayment,
                                        a.ChellanNo,
                                        a.FromDate
                                    }).AsEnumerable()
                                 .Select((x, index) => new TDS94IModel()
                                 {
                                     SlNo = index + 1,
                                     Name = x.Party,
                                     PAN = x.PAN,
                                     AmountPaid = x.Basic ?? 0,
                                     TDS = x.Amount ?? 0,
                                     DateofPayment = Convert.ToDateTime(x.PostedDate),
                                     Rate = Common.GetTDSPercentage(x.Section ?? 0),
                                     BSRCode = x.BankReferenceNo,
                                     ChallenNo = x.ChellanNo
                                 }).ToList();
                    int[] _94J = { 41, 377 };
                    model.TDS94J = (from a in context.tblTDSPayment
                                    join b in context.tblTDSIncomeTaxDetails on a.tblTDSPaymentId equals b.tblTDSPaymentId
                                    join c in context.tblTDSDetails on new { DetId = b.DetailId, Refno = b.ReferenceNo, AccId = a.Section } equals new { DetId = c.Detailid, Refno = c.ReferenceNumber, AccId = c.AccountHeadId } into g
                                    from c in g.DefaultIfEmpty()
                                    where status.Contains(a.Status) && _94J.Contains(a.Section ?? 0) && a.ToDate == Date
                                    select new
                                    {
                                        a.BankReferenceNo,

                                        a.Section,
                                        b.Party,
                                        b.PAN,
                                        b.Amount,
                                        c.Basic,
                                        c.PostedDate,
                                        a.DateOfPayment,
                                        a.ChellanNo,
                                        a.FromDate
                                    }).AsEnumerable()
                                 .Select((x, index) => new TDS94JModel()
                                 {
                                     SlNo = index + 1,
                                     Name = x.Party,
                                     PAN = x.PAN,
                                     AmountPaid = x.Basic ?? 0,
                                     TDS = x.Amount ?? 0,
                                     DateofPayment = Convert.ToDateTime(x.PostedDate),
                                     Rate = Common.GetTDSPercentage(x.Section ?? 0),
                                     BSRCode = x.BankReferenceNo,
                                     ChallenNo = x.ChellanNo
                                 }).ToList();
                    int[] _94H = { 43, 374 };
                    model.TDS94H = (from a in context.tblTDSPayment
                                    join b in context.tblTDSIncomeTaxDetails on a.tblTDSPaymentId equals b.tblTDSPaymentId
                                    join c in context.tblTDSDetails on new { DetId = b.DetailId, Refno = b.ReferenceNo, AccId = a.Section } equals new { DetId = c.Detailid, Refno = c.ReferenceNumber, AccId = c.AccountHeadId } into g
                                    from c in g.DefaultIfEmpty()
                                    where status.Contains(a.Status) && _94H.Contains(a.Section ?? 0) && a.ToDate == Date
                                    select new
                                    {
                                        a.BankReferenceNo,
                                        a.Section,
                                        b.Party,
                                        b.PAN,
                                        b.Amount,
                                        c.Basic,
                                        c.PostedDate,
                                        a.DateOfPayment,
                                        a.ChellanNo,
                                        a.FromDate
                                    }).AsEnumerable()
                                .Select((x, index) => new TDS94HModel()
                                {
                                    Name = x.Party,
                                    PAN = x.PAN,
                                    AmountPaid = x.Basic ?? 0,
                                    TDS = x.Amount ?? 0,
                                    DateofPayment = Convert.ToDateTime(x.PostedDate),
                                    Rate = Common.GetTDSPercentage(x.Section ?? 0),
                                    BSRCode = x.BankReferenceNo,
                                    ChallenNo = x.ChellanNo
                                }).ToList();

                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        public static Form24QModel GetForm27Q(DateTime Date)
        {

            Form24QModel model = new Form24QModel();
            List<TDS94CModel> TDS94C = new List<TDS94CModel>();
            List<TDS94JModel> TDS94J = new List<TDS94JModel>();
            List<TDS94HModel> TDS94H = new List<TDS94HModel>();
            List<TDS94IModel> TDS94I = new List<TDS94IModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    string[] status = { "Approved", "Completed" };
                    int[] _94C = { 332, 333, 334, 335, 336 };
                    var Qry = (from a in context.tblTDSPayment
                               join b in context.tblTDSIncomeTaxDetails on a.tblTDSPaymentId equals b.tblTDSPaymentId
                               join c in context.tblTDSDetails on new { DetId = b.DetailId, Refno = b.ReferenceNo, AccId = a.Section } equals new { DetId = c.Detailid, Refno = c.ReferenceNumber, AccId = c.AccountHeadId } into g
                               from c in g.DefaultIfEmpty()
                               where status.Contains(a.Status) && _94C.Contains(a.Section ?? 0) && a.ToDate == Date
                               select new
                               {
                                   a.Section,
                                   b.Party,
                                   b.PAN,
                                   b.Amount,
                                   c.Basic,
                                   c.PostedDate,
                                   a.DateOfPayment,
                                   a.ChellanNo,
                                   a.FromDate,
                                   a.BankReferenceNo
                               }).ToList();
                    if (Qry != null)
                    {
                        if (Qry.Count > 0)
                        {
                            for (int i = 0; i < Qry.Count; i++)
                            {
                                int rate = 0;
                                if (Qry[i].Section == 332)
                                    rate = 10;
                                if (Qry[i].Section == 333)
                                    rate = 15;
                                if (Qry[i].Section == 334)
                                    rate = 20;
                                if (Qry[i].Section == 335)
                                    rate = 25;
                                if (Qry[i].Section == 336)
                                    rate = 30;
                                TDS94C.Add(new TDS94CModel()
                                {
                                    SlNo = i + 1,
                                    Name = Qry[i].Party,
                                    PAN = Qry[i].PAN,
                                    AmountPaid = Qry[i].Basic ?? 0,
                                    TDS = Qry[i].Amount ?? 0,
                                    DateofPayment = Convert.ToDateTime(Qry[i].PostedDate),
                                    Rate = Common.GetTDSPercentage(Qry[i].Section ?? 0),
                                    BSRCode = Qry[i].BankReferenceNo,
                                    ChallenNo = Qry[i].ChellanNo
                                });
                            }
                        }
                    }
                    model.TDS94C = TDS94C;
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        public static Form24QModel GetForm24Q(DateTime Date, DateTime Date1)
        {

            Form24QModel model = new Form24QModel();
            List<TDS94CModel> TDS94C = new List<TDS94CModel>();
            List<TDS94JModel> TDS94J = new List<TDS94JModel>();
            List<TDS94HModel> TDS94H = new List<TDS94HModel>();
            List<TDS94IModel> TDS94I = new List<TDS94IModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    string[] status = { "Approved", "Completed" };
                    var Qry = (
                               from a in context.tblTDSPayment
                               join b in context.tblTDSIncomeTaxDetails on a.tblTDSPaymentId equals b.tblTDSPaymentId
                               join c in context.tblSalaryPaymentHead on b.ReferenceNo equals c.PaymentNo
                               join d in context.tblSalaryPayment on c.PaymentHeadId equals d.PaymentHeadId
                               join e in context.vwCombineStaffDetails on d.PayBill equals e.EmployeeId
                               join f in context.tblStaffBankAccount on d.PayBill equals f.EmployeeId
                               where status.Contains(a.Status) && a.Section == 40 && a.FromDate == Date && d.Deduction > 0
                               && c.PaidDate >= Date && c.PaidDate <= Date1
                               //from a in context.tblSalaryPaymentHead 
                               //join d in context.tblSalaryPayment on a.PaymentHeadId equals d.PaymentHeadId
                               //join e in context.vwCombineStaffDetails on d.PayBill equals e.ID
                               //join f in context.tblStaffBankAccount on d.PayBill equals f.UserId
                               //where  a.PaymentHeadId==23 && d.Deduction > 0 &&a.PaidDate
                               select new
                               {
                                   e.Name,
                                   f.PAN,
                                   d.NetSalary,
                                   d.Deduction,
                                   a.DateOfPayment,
                                   a.ChellanNo,
                                   a.FromDate,
                                   e.ID,
                                   a.BankReferenceNo,
                                   c.PaidDate
                               }).ToList();
                    if (Qry != null)
                    {
                        if (Qry.Count > 0)
                        {
                            for (int i = 0; i < Qry.Count; i++)
                            {
                                decimal edufee = ((Qry[i].Deduction ?? 0) / 100) * 4;
                                decimal TDS = ((Qry[i].Deduction ?? 0) - edufee);
                                TDS94C.Add(new TDS94CModel()
                                {
                                    PaymentMonth = String.Format("{0:MMMM}", Qry[i].PaidDate),
                                    Name = Qry[i].Name,
                                    PAN = Qry[i].PAN,
                                    AmountPaid = Qry[i].NetSalary ?? 0,
                                    TDS = TDS,
                                    EmployeeId = Convert.ToString(Qry[i].ID),
                                    EducationFees = edufee,
                                    Deduction = Qry[i].Deduction ?? 0,
                                    DateofPayment = Convert.ToDateTime(Qry[i].DateOfPayment),
                                    BSRCode = Qry[i].BankReferenceNo,
                                    ChallenNo = Qry[i].ChellanNo
                                });
                            }
                        }
                    }
                    model.TDS94C = TDS94C;
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion
        #region Sponser Office Monthly Report
        public static OfficeMonthlyReportModel GetSponserMonthlyReport(string Month)
        {
            OfficeMonthlyReportModel model = new OfficeMonthlyReportModel();
            List<OfficeMonthlyListReportModel> List = new List<OfficeMonthlyListReportModel>();
            string Type = "";
            try
            {
                using (var context = new IOASDBEntities())
                {
                    FinOp fac = new FinOp(System.DateTime.Now);
                    var fromdate = fac.GetMonthFirstDate(Month);
                    var todate = fac.GetMonthLastDate(Month);
                    todate = todate.AddDays(1).AddTicks(-2);
                    DateTime startDate; // 1st Feb this year
                    DateTime endDate;
                    if (fromdate.Month > 3)
                    {
                        if (fromdate.Month == 4)
                        {
                            endDate = fromdate.AddMonths(-1).AddTicks(-2);
                            startDate = new DateTime(fromdate.Year, 4, 1);
                        }
                        else
                        {
                            startDate = new DateTime(fromdate.Year, 4, 1);
                            endDate = fromdate.AddTicks(-2);
                        }
                    }
                    else
                    {
                        startDate = new DateTime((fromdate.Year) - 1, 4, 1);
                        endDate = fromdate.AddTicks(-2);
                    }
                    int CurrentYear = fromdate.Year;
                    int PreviousYear = fromdate.Year - 1;
                    int NextYear = fromdate.Year + 1;
                    string PreYear = PreviousYear.ToString();
                    string NexYear = NextYear.ToString();
                    string CurYear = CurrentYear.ToString();
                    string FinYear = null;
                    if (fromdate.Month > 3)
                        FinYear = CurYear.Substring(2) + NexYear.Substring(2);
                    else
                        FinYear = PreYear.Substring(2) + CurYear.Substring(2);
                    int FinId = context.tblFinYear.Where(m => m.Year == FinYear).Select(m => m.FinYearId).FirstOrDefault();
                    var QryDept = context.tblProject.Select(m => m.PIDepartment).Distinct().ToList();
                    var Qry = (from c in context.tblProject
                               join d in context.tblFacultyDetail on c.PIDepartment equals d.DepartmentCode
                               where !String.IsNullOrEmpty(d.DepartmentCode) && !String.IsNullOrEmpty(d.DepartmentName) && !String.IsNullOrEmpty(c.PIDepartment)
                               select new { c.PIDepartment, d.DepartmentName }).Distinct().ToList();
                    int? TtlPrevProj_A = 0; int? TtlCurrProj_A = 0; int? TtlProj_A = 0;
                    int? TtlPrevProj_B = 0; int? TtlCurrProj_B = 0; int? TtlProj_B = 0;
                    int? TtlPrevProj = 0; int? TtlCurrProj = 0; int? TtlProj = 0;
                    decimal? TtlPrevAmt_A = 0; decimal? TtlCurrAmt_A = 0; decimal? TtlAmt_A = 0;
                    decimal? TtlPrevAmt_B = 0; decimal? TtlCurrAmt_B = 0; decimal? TtlAmt_B = 0;
                    decimal? TtlPrevAmt = 0; decimal? TtlCurrAmt = 0; decimal? TtlAmt = 0;
                    decimal? TtlPrevRecAmt = 0; decimal? TtlCurrRecAmt = 0; decimal? TtlRecAmt = 0;
                    int[] Categ = { 1, 14, 13, 16 };
                    int[] OfficeCateg = { 2, 3 };
                    int[] ReceiptCateg = { 1, 3 };
                    for (int i = 0; i < Qry.Count(); i++)
                    {
                        var DeptName = Qry[i].DepartmentName;
                        var DeptCode = Qry[i].PIDepartment;
                        DeptCode = DeptCode.Trim();
                        ////// A //////
                        decimal TotalAmt_A = 0; decimal AmtforPrevMonth_A = 0; decimal AmtforCurMonth_A = 0; decimal receiptAmt = 0; decimal QryRecAMt = 0;
                        decimal TotalAmt_B = 0; decimal AmtforPrevMonth_B = 0; decimal AmtforCurMonth_B = 0;
                        var ProjDet_A = context.tblProject.Where(m => m.CrtdTS >= fromdate && m.CrtdTS <= todate && m.FacultyDetailId == null
                        && m.FinancialYear == FinId && m.PIDepartment == DeptCode && OfficeCateg.Contains(m.ReportClassification ?? 0)
                        && m.ProjectType == 1 && m.SchemeName != 33 && !m.ProjectNumber.Contains("ISRO") && m.ProjectClassification != 16).ToList();
                        var PrevProjDet_A = context.tblProject.Where(m => m.CrtdTS >= startDate && m.CrtdTS <= endDate
                        && m.FacultyDetailId == null && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.FinancialYear == FinId
                        && m.PIDepartment == DeptCode && m.ProjectType == 1 && m.SchemeName != 33 && !m.ProjectNumber.Contains("ISRO") && m.ProjectClassification != 16).ToList();
                        int[] EnhanceArr_A = ProjDet_A.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_A = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && b.FacultyDetailId == null && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                             //&& !EnhanceArr_A.Contains(aa.ProjectId ?? 0)
                                             && b.ProjectClassification != 16 && !b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true
                                          && c.DepartmentCode == DeptCode && b.ProjectType == 1 && b.SchemeName != 33
                                             select aa).ToList();
                        decimal Enhance_A = ListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        var PrevListEnhance_A = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && b.FacultyDetailId == null && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                                 && b.ProjectClassification != 16 && !b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true
                                                 && c.DepartmentCode == DeptCode && b.ProjectType == 1 && b.SchemeName != 33
                                                 select aa).ToList();
                        decimal PrevEnhance_A = PrevListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        PrevEnhance_A = (PrevEnhance_A / 100000);
                        Enhance_A = (Enhance_A / 100000);
                        AmtforCurMonth_A = (ProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_A = AmtforCurMonth_A + Enhance_A;
                        AmtforPrevMonth_A = (PrevProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_A = AmtforPrevMonth_A + PrevEnhance_A;
                        TotalAmt_A = AmtforCurMonth_A + AmtforPrevMonth_A;
                        int? PrevProj_A = PrevProjDet_A.Count();
                        int? CurrProj_A = ProjDet_A.Count();
                        int? TotalProj_A = PrevProj_A + CurrProj_A;
                        ////// B //////
                        var ProjDet_B = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= todate
                        && m.FacultyDetailId == null && m.FinancialYear == FinId && m.PIDepartment == DeptCode && m.ProjectType == 1 && m.SchemeName == 33 && !m.ProjectNumber.Contains("ISRO") && m.ProjectClassification != 16).ToList();
                        var PrevProjDet_B = context.tblProject.Where(m => m.CrtdTS >= startDate
                        && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= endDate
                        && m.FacultyDetailId == null && m.PIDepartment == DeptCode && m.FinancialYear == FinId && m.ProjectType == 1 && m.SchemeName == 33 && !m.ProjectNumber.Contains("ISRO") && m.ProjectClassification != 16).ToList();
                        int[] EnhanceArr_B = ProjDet_B.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_B = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && b.FacultyDetailId == null && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                             //&& !EnhanceArr_B.Contains(aa.ProjectId ?? 0)
                                              && b.ProjectClassification != 16 && !b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true
                                               && c.DepartmentCode == DeptCode && b.ProjectType == 1 && b.SchemeName == 33
                                             select aa).ToList();
                        decimal Enhance_B = ListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        var PrevListEnhance_B = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && b.FacultyDetailId == null && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate && !EnhanceArr_B.Contains(aa.ProjectId ?? 0)
                                                && b.ProjectClassification != 16 && !b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true
                                                 && c.DepartmentCode == DeptCode && b.ProjectType == 1 && b.SchemeName == 33
                                                 select aa).ToList();
                        decimal PrevEnhance_B = PrevListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        PrevEnhance_B = (PrevEnhance_B / 100000);
                        Enhance_B = (Enhance_B / 100000);
                        AmtforCurMonth_B = (ProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_B = AmtforCurMonth_B + Enhance_B;
                        AmtforPrevMonth_B = (PrevProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_B = AmtforPrevMonth_B + PrevEnhance_B;
                        TotalAmt_B = AmtforCurMonth_B + AmtforPrevMonth_B;
                        int? PrevProj_B = PrevProjDet_B.Count();
                        int? CurrProj_B = ProjDet_B.Count();
                        int? TotalProj_B = PrevProj_B + CurrProj_B;
                        // int[] ProjDet = context.tblProject.Where(m => m.CrtdTS >= fromdate && m.CrtdTS <= todate && m.ProjectSubType != 1 && m.FinancialYear == FinId && m.PIDepartment == DeptCode && m.Status == "Active" && m.ProjectType == 1 && m.ProjectClassification != 16).Select(m => m.ProjectId).ToArray();
                        //int[] PrevProjDet = context.tblProject.Where(m => m.CrtdTS >= startDate && m.CrtdTS <= endDate && m.ProjectSubType != 1 && m.FinancialYear == FinId && m.PIDepartment == DeptCode && m.Status == "Active" && m.ProjectType == 1 && m.ProjectClassification != 16).Select(m => m.ProjectId).ToArray();
                        int[] ProjClass = { 4, 5, 6, 7 };
                        var QryRect = (from a in context.tblReceipt
                                       join b in context.tblProject on a.ProjectId equals b.ProjectId
                                       join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                       where b.FacultyDetailId == null && a.Posted_f == true
                                       && c.DepartmentCode == DeptCode && a.CrtdTS >= startDate && a.CrtdTS <= endDate
                                       && !Categ.Contains(a.CategoryId ?? 0) && b.ProjectClassification != 16 && b.ProjectType == 1 && a.Status == "Completed"
                                       && !a.ReceiptNumber.Contains("RBU") && !b.ProjectNumber.StartsWith("DIA")
                                      && !b.ProjectNumber.Contains("ISRO") && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                       select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                        QryRecAMt = QryRect.Sum(m => m.ReceiptAmount ?? 0);

                        QryRecAMt = (QryRecAMt) / 100000;
                        var QryReceipt = (from a in context.tblReceipt
                                          join b in context.tblProject on a.ProjectId equals b.ProjectId
                                          join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where b.FacultyDetailId == null && !b.ProjectNumber.StartsWith("DIA") && a.Posted_f == true
                                          && c.DepartmentCode == DeptCode && a.CrtdTS >= fromdate && a.CrtdTS <= todate
                                         && !Categ.Contains(a.CategoryId ?? 0) && b.ProjectClassification != 16 && b.ProjectType == 1 && a.Status == "Completed"
                                         && !a.ReceiptNumber.Contains("RBU") && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                          && !ProjClass.Contains(b.ProjectClassification ?? 0) && !b.ProjectNumber.Contains("ISRO")
                                          select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();

                        receiptAmt = QryReceipt.Sum(m => m.ReceiptAmount ?? 0);
                        receiptAmt = receiptAmt / 100000;
                        TtlPrevRecAmt += QryRecAMt;
                        TtlCurrRecAmt += receiptAmt;
                        decimal TotalRecAmt = receiptAmt + QryRecAMt;
                        TtlPrevProj_A += PrevProj_A;
                        TtlCurrProj_A += CurrProj_A;
                        TtlPrevProj_B += PrevProj_B;
                        TtlCurrProj_B += CurrProj_B;
                        TtlPrevAmt_A += AmtforPrevMonth_A;
                        TtlCurrAmt_A += AmtforCurMonth_A;
                        TtlPrevAmt_B += AmtforPrevMonth_B;
                        TtlCurrAmt_B += AmtforCurMonth_B;
                        var LbAmtforCurMonth_A = ""; var LbAmtforCurMonth_B = "";
                        if (Enhance_A > 0)
                            LbAmtforCurMonth_A = "*" + String.Format("{0:0.##}", +AmtforCurMonth_A);
                        else
                            LbAmtforCurMonth_A = String.Format("{0:0.##}", AmtforCurMonth_A);
                        if (Enhance_B > 0)
                            LbAmtforCurMonth_B = "*" + String.Format("{0:0.##}", +AmtforCurMonth_B);
                        else
                            LbAmtforCurMonth_B = String.Format("{0:0.##}", AmtforCurMonth_B);
                        var LbAmtforPrevMonth_A = ""; var LbAmtforPrevMonth_B = "";
                        if (PrevEnhance_A > 0)
                            LbAmtforPrevMonth_A = "*" + String.Format("{0:0.00}", AmtforPrevMonth_A);
                        else
                            LbAmtforPrevMonth_A = String.Format("{0:0.00}", AmtforPrevMonth_A);
                        if (PrevEnhance_B > 0)
                            LbAmtforPrevMonth_B = "*" + String.Format("{0:0.00}", AmtforPrevMonth_B);
                        else
                            LbAmtforPrevMonth_B = String.Format("{0:0.00}", AmtforPrevMonth_B);

                        List.Add(new OfficeMonthlyListReportModel()
                        {
                            DepartmentName = DeptName,
                            Type = Type,
                            NoOfProjCurMonth_A = CurrProj_A ?? 0,
                            NoOfProjPrevMonth_A = PrevProj_A ?? 0,
                            TotalProj_A = TotalProj_A ?? 0,
                            AmtforCurMonth_A = LbAmtforCurMonth_A,
                            AmtforPrevMonth_A = LbAmtforPrevMonth_A,
                            TotalAmt_A = TotalAmt_A,
                            NoOfProjCurMonth_B = CurrProj_B ?? 0,
                            NoOfProjPrevMonth_B = PrevProj_B ?? 0,
                            TotalProj_B = TotalProj_B ?? 0,
                            AmtforCurMonth_B = LbAmtforCurMonth_B,
                            AmtforPrevMonth_B = LbAmtforPrevMonth_B,
                            TotalAmt_B = TotalAmt_B,
                            AmtforRecCurMonth = receiptAmt,
                            AmtforRecPrevMonth = QryRecAMt,
                            TotalRecAmt = TotalRecAmt
                        });
                    }
                    ////////Center ////////
                    List<OfficeMonthlyCenterListReportModel> CenterList = new List<OfficeMonthlyCenterListReportModel>();
                    int[] Center = { 59, 62, 65, 72, 73, 79, 83, 85, 86, 87, 94, 109, 132, 135, 138, 145, 146, 152, 156, 158, 159, 160, 167, 180, 216 };
                    if (Center.Length > 0)
                    {


                        // var DeptName = Center[a].Head;
                        //int DeptCode = Center[a].HeadId;
                        decimal TotalAmt_A = 0; decimal AmtforPrevMonth_A = 0; decimal AmtforCurMonth_A = 0; decimal receiptAmt = 0; decimal QryRecAMt = 0;
                        decimal TotalAmt_B = 0; decimal AmtforPrevMonth_B = 0; decimal AmtforCurMonth_B = 0;
                        //////A///////
                        var ProjDet_A = context.tblProject.Where(m => m.CrtdTS >= fromdate && m.CrtdTS <= todate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.FinancialYear == FinId && Center.Contains(m.FacultyDetailId ?? 0) && m.ProjectType == 1 && m.SchemeName != 33 && !m.ProjectNumber.Contains("ISRO") && m.ProjectClassification != 16).ToList();
                        var PrevProjDet_A = context.tblProject.Where(m => m.CrtdTS >= startDate && m.CrtdTS <= endDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.FinancialYear == FinId && Center.Contains(m.FacultyDetailId ?? 0) && m.ProjectType == 1 && m.SchemeName != 33 && !m.ProjectNumber.Contains("ISRO") && m.ProjectClassification != 16).ToList();
                        int[] EnhanceArr_A = ProjDet_A.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_A = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate

                                            && b.ProjectClassification != 16 && !b.ProjectNumber.Contains("ISRO")
                                            && aa.IsEnhancementonly == true && OfficeCateg.Contains(b.ReportClassification ?? 0) && Center.Contains(b.FacultyDetailId ?? 0) && b.ProjectType == 1 && b.SchemeName != 33
                                             select aa).ToList();
                        decimal Enhance_A = ListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        var PrevListEnhance_A = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                               //&& !EnhanceArr_A.Contains(aa.ProjectId ?? 0)
                                               && b.ProjectClassification != 16 && !b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true
                                             && Center.Contains(b.FacultyDetailId ?? 0) && b.ProjectType == 1 && b.SchemeName != 33
                                                 select aa).ToList();
                        decimal PrevEnhance_A = PrevListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        PrevEnhance_A = (PrevEnhance_A / 100000);
                        Enhance_A = (Enhance_A / 100000);
                        AmtforCurMonth_A = (ProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_A = AmtforCurMonth_A + Enhance_A;
                        AmtforPrevMonth_A = (PrevProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_A = AmtforPrevMonth_A + PrevEnhance_A;
                        TotalAmt_A = AmtforCurMonth_A + AmtforPrevMonth_A;
                        int? PrevProj_A = PrevProjDet_A.Count();
                        int? CurrProj_A = ProjDet_A.Count();
                        int? TotalProj_A = PrevProj_A + CurrProj_A;
                        ////// B //////
                        var ProjDet_B = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= todate && m.FinancialYear == FinId && Center.Contains(m.FacultyDetailId ?? 0) && m.ProjectType == 1 && m.SchemeName == 33 && !m.ProjectNumber.Contains("ISRO") && m.ProjectClassification != 16).ToList();
                        var PrevProjDet_B = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= endDate && m.FinancialYear == FinId && Center.Contains(m.FacultyDetailId ?? 0) && m.ProjectType == 1 && m.SchemeName == 33 && !m.ProjectNumber.Contains("ISRO") && m.ProjectClassification != 16).ToList();
                        int[] EnhanceArr_B = ProjDet_B.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_B = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                            //&& !EnhanceArr_B.Contains(aa.ProjectId ?? 0)
                                            && b.ProjectClassification != 16 && !b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true
                                         && Center.Contains(b.FacultyDetailId ?? 0) && b.ProjectType == 1 && b.SchemeName == 33
                                             select aa).ToList();
                        decimal Enhance_B = ListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        var PrevListEnhance_B = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate && !EnhanceArr_B.Contains(aa.ProjectId ?? 0)
                                               && b.ProjectClassification != 16 && !b.ProjectNumber.Contains("ISRO")
                                               && aa.IsEnhancementonly == true && Center.Contains(b.FacultyDetailId ?? 0) && b.ProjectType == 1 && b.SchemeName == 33
                                                 select aa).ToList();
                        decimal PrevEnhance_B = PrevListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        PrevEnhance_B = (PrevEnhance_B / 100000);
                        Enhance_B = (Enhance_B / 100000);
                        AmtforCurMonth_B = (ProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_B = AmtforCurMonth_B + Enhance_B;
                        AmtforPrevMonth_B = (PrevProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_B = AmtforPrevMonth_B + PrevEnhance_B;
                        TotalAmt_B = AmtforCurMonth_B + AmtforPrevMonth_B;
                        int? PrevProj_B = PrevProjDet_B.Count();
                        int? CurrProj_B = ProjDet_B.Count();
                        int? TotalProj_B = PrevProj_B + CurrProj_B;
                        //int[] ProjDet = context.tblProject.Where(m => m.CrtdTS >= fromdate && m.FinancialYear == FinId && m.ProjectSubType != 1 && m.CrtdTS <= todate && Center.Contains(m.FacultyDetailId ?? 0) && m.Status == "Active" && m.ProjectType == 1 && m.ProjectClassification != 16).Select(m => m.ProjectId).ToArray();
                        // int[] PrevProjDet = context.tblProject.Where(m => m.CrtdTS >= startDate && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.ProjectSubType != 1 && Center.Contains(m.FacultyDetailId ?? 0) && m.Status == "Active" && m.ProjectType == 1 && m.ProjectClassification != 16).Select(m => m.ProjectId).ToArray();
                        int[] ProjClass = { 4, 5, 6, 7 };
                        var QryRect = (from aa in context.tblReceipt
                                       join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                       join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                       where Center.Contains(b.FacultyDetailId ?? 0) && aa.Posted_f == true && aa.CrtdTS >= startDate && aa.CrtdTS <= endDate
                                     && !Categ.Contains(aa.CategoryId ?? 0) && b.ProjectClassification != 16 && b.ProjectType == 1 && aa.Status == "Completed" && !aa.ReceiptNumber.Contains("RBU")
                                     && !b.ProjectNumber.Contains("ISRO") && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                       select new { aa.ReceiptAmount, aa.ReceiptOverheadValue, aa.IGST, aa.SGST, aa.CGST, b.ProjectNumber }).ToList();
                        QryRecAMt = QryRect.Sum(m => m.ReceiptAmount ?? 0);

                        QryRecAMt = (QryRecAMt) / 100000;
                        var QryReceipt = (from aa in context.tblReceipt
                                          join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                          join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where Center.Contains(b.FacultyDetailId ?? 0) && aa.CrtdTS >= fromdate && aa.CrtdTS <= todate
                                         && !Categ.Contains(aa.CategoryId ?? 0) && aa.Posted_f == true && b.ProjectClassification != 16 && b.ProjectType == 1 && aa.Status == "Completed" && !aa.ReceiptNumber.Contains("RBU")
                                        && !b.ProjectNumber.Contains("ISRO") && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                          select new { aa.ReceiptAmount, aa.ReceiptOverheadValue, aa.IGST, aa.SGST, aa.CGST, b.ProjectNumber }).ToList();
                        receiptAmt = QryReceipt.Sum(m => m.ReceiptAmount ?? 0);
                        receiptAmt = receiptAmt / 100000;
                        TtlPrevRecAmt += QryRecAMt;
                        TtlCurrRecAmt += receiptAmt;
                        decimal TotalRecAmt = receiptAmt + QryRecAMt;
                        TtlPrevProj_A += PrevProj_A;
                        TtlCurrProj_A += CurrProj_A;
                        TtlPrevProj_B += PrevProj_B;
                        TtlCurrProj_B += CurrProj_B;
                        TtlPrevAmt_A += AmtforPrevMonth_A;
                        TtlCurrAmt_A += AmtforCurMonth_A;
                        TtlPrevAmt_B += AmtforPrevMonth_B;
                        TtlCurrAmt_B += AmtforCurMonth_B;
                        var LbAmtforCurMonth_A = ""; var LbAmtforCurMonth_B = "";
                        if (Enhance_A > 0)
                            LbAmtforCurMonth_A = "*" + String.Format("{0:0.##}", +AmtforCurMonth_A);
                        else
                            LbAmtforCurMonth_A = String.Format("{0:0.##}", AmtforCurMonth_A);
                        if (Enhance_B > 0)
                            LbAmtforCurMonth_B = "*" + String.Format("{0:0.##}", +AmtforCurMonth_B);
                        else
                            LbAmtforCurMonth_B = String.Format("{0:0.##}", AmtforCurMonth_B);
                        var LbAmtforPrevMonth_A = ""; var LbAmtforPrevMonth_B = "";
                        if (PrevEnhance_A > 0)
                            LbAmtforPrevMonth_A = "*" + String.Format("{0:0.00}", AmtforPrevMonth_A);
                        else
                            LbAmtforPrevMonth_A = String.Format("{0:0.00}", AmtforPrevMonth_A);
                        if (PrevEnhance_B > 0)
                            LbAmtforPrevMonth_B = "*" + String.Format("{0:0.00}", AmtforPrevMonth_B);
                        else
                            LbAmtforPrevMonth_B = String.Format("{0:0.00}", AmtforPrevMonth_B);
                        List.Add(new OfficeMonthlyListReportModel()
                        {
                            DepartmentName = "AAR,ADM,NPT,TAP,CAT,CSH,ICW,IGC,SOL,STL,SHS,RBC,GWC",
                            Type = Type,
                            NoOfProjCurMonth_A = CurrProj_A ?? 0,
                            NoOfProjPrevMonth_A = PrevProj_A ?? 0,
                            TotalProj_A = TotalProj_A ?? 0,
                            AmtforCurMonth_A = LbAmtforCurMonth_A,
                            AmtforPrevMonth_A = LbAmtforPrevMonth_A,
                            TotalAmt_A = TotalAmt_A,
                            NoOfProjCurMonth_B = CurrProj_B ?? 0,
                            NoOfProjPrevMonth_B = PrevProj_B ?? 0,
                            TotalProj_B = TotalProj_B ?? 0,
                            AmtforCurMonth_B = LbAmtforCurMonth_B,
                            AmtforPrevMonth_B = LbAmtforPrevMonth_B,
                            TotalAmt_B = TotalAmt_B,
                            AmtforRecCurMonth = receiptAmt,
                            AmtforRecPrevMonth = QryRecAMt,
                            TotalRecAmt = TotalRecAmt
                        });

                    }
                    ////////ICSR ////////
                    int[] ICSR = { 16 };
                    if (Center.Length > 0)
                    {
                        // var DeptName = Center[a].Head;
                        //int DeptCode = Center[a].HeadId;
                        decimal TotalAmt_A = 0; decimal AmtforPrevMonth_A = 0; decimal AmtforCurMonth_A = 0; decimal receiptAmt = 0; decimal QryRecAMt = 0;
                        decimal TotalAmt_B = 0; decimal AmtforPrevMonth_B = 0; decimal AmtforCurMonth_B = 0;
                        //////A///////
                        var ProjDet_A = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= todate && m.FinancialYear == FinId && m.ProjectType == 1 && m.SchemeName != 33 && !m.ProjectNumber.Contains("ISRO") && m.ProjectClassification == 16).ToList();
                        var PrevProjDet_A = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.ProjectType == 1 && m.SchemeName != 33 && !m.ProjectNumber.Contains("ISRO") && m.ProjectClassification == 16).ToList();
                        int[] EnhanceArr_A = ProjDet_A.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_A = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                            //&& !EnhanceArr_A.Contains(aa.ProjectId ?? 0)
                                            && b.ProjectClassification == 16 && !b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true
                                              && b.ProjectType == 1 && b.SchemeName != 33
                                             select aa).ToList();
                        decimal Enhance_A = ListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        var PrevListEnhance_A = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate && !EnhanceArr_A.Contains(aa.ProjectId ?? 0)
                                               && b.ProjectClassification == 16 && !b.ProjectNumber.Contains("ISRO")
                                               && aa.IsEnhancementonly == true && b.ProjectType == 1 && b.SchemeName != 33
                                                 select aa).ToList();
                        decimal PrevEnhance_A = PrevListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        PrevEnhance_A = (PrevEnhance_A / 100000);
                        Enhance_A = (Enhance_A / 100000);
                        AmtforCurMonth_A = (ProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_A = AmtforCurMonth_A + Enhance_A;
                        AmtforPrevMonth_A = (PrevProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_A = AmtforPrevMonth_A + PrevEnhance_A;
                        TotalAmt_A = AmtforCurMonth_A + AmtforPrevMonth_A;
                        int? PrevProj_A = PrevProjDet_A.Count();
                        int? CurrProj_A = ProjDet_A.Count();
                        int? TotalProj_A = PrevProj_A + CurrProj_A;
                        ////// B //////
                        var ProjDet_B = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= todate && m.FinancialYear == FinId && m.ProjectType == 1 && m.SchemeName == 33 && !m.ProjectNumber.Contains("ISRO") && m.ProjectClassification == 16).ToList();
                        var PrevProjDet_B = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.ProjectType == 1 && m.SchemeName == 33 && !m.ProjectNumber.Contains("ISRO") && m.ProjectClassification == 16).ToList();
                        int[] EnhanceArr_B = ProjDet_B.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_B = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                            //&& !EnhanceArr_B.Contains(aa.ProjectId ?? 0)
                                            && b.ProjectClassification == 16 && !b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true
                                            && Center.Contains(b.FacultyDetailId ?? 0) && b.ProjectType == 1 && b.SchemeName == 33
                                             select aa).ToList();
                        decimal Enhance_B = ListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        var PrevListEnhance_B = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate && !EnhanceArr_B.Contains(aa.ProjectId ?? 0)
                                               && b.ProjectClassification == 16 && !b.ProjectNumber.Contains("ISRO")
                                               && aa.IsEnhancementonly == true && Center.Contains(b.FacultyDetailId ?? 0) && b.ProjectType == 1 && b.SchemeName == 33
                                                 select aa).ToList();
                        decimal PrevEnhance_B = PrevListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        PrevEnhance_B = (PrevEnhance_B / 100000);
                        Enhance_B = (Enhance_B / 100000);
                        AmtforCurMonth_B = (ProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_B = AmtforCurMonth_B + Enhance_B;
                        AmtforPrevMonth_B = (PrevProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_B = AmtforPrevMonth_B + PrevEnhance_B;
                        TotalAmt_B = AmtforCurMonth_B + AmtforPrevMonth_B;
                        int? PrevProj_B = PrevProjDet_B.Count();
                        int? CurrProj_B = ProjDet_B.Count();
                        int? TotalProj_B = PrevProj_B + CurrProj_B;
                        //   int[] ProjDet = context.tblProject.Where(m => m.CrtdTS >= fromdate && m.FinancialYear == FinId && m.ProjectSubType != 1 && m.CrtdTS <= todate && m.Status == "Active" && m.ProjectType == 1 && m.ProjectClassification == 16).Select(m => m.ProjectId).ToArray();
                        //     int[] PrevProjDet = context.tblProject.Where(m => m.CrtdTS >= startDate && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.ProjectSubType != 1 && m.Status == "Active" && m.ProjectType == 1 && m.ProjectClassification == 16).Select(m => m.ProjectId).ToArray();
                        int[] ProjClass = { 4, 5, 6, 7 };
                        var QryRect = (from aa in context.tblReceipt
                                       join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                       join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                       where b.ProjectClassification == 16 && aa.CrtdTS >= startDate && aa.CrtdTS <= endDate
                                        && !Categ.Contains(aa.CategoryId ?? 0) && aa.Posted_f == true && b.ProjectType == 1 && aa.Status == "Completed"
                                        && !aa.ReceiptNumber.Contains("RBU") && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                      && !b.ProjectNumber.Contains("ISRO")
                                       select new { aa.ReceiptAmount, aa.ReceiptOverheadValue, aa.IGST, aa.SGST, aa.CGST, b.ProjectNumber }).ToList();
                        QryRecAMt = QryRect.Sum(m => m.ReceiptAmount ?? 0);

                        QryRecAMt = (QryRecAMt) / 100000;
                        var QryReceipt = (from aa in context.tblReceipt
                                          join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                          join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where b.ProjectClassification == 16 && aa.CrtdTS >= fromdate && aa.CrtdTS <= todate
                                          && b.ProjectType == 1 && aa.Status == "Completed" && !aa.ReceiptNumber.Contains("RBU")
                                         && !Categ.Contains(aa.CategoryId ?? 0) && aa.Posted_f == true && !b.ProjectNumber.Contains("ISRO")
                                         && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                          select new { aa.ReceiptAmount, aa.ReceiptOverheadValue, aa.IGST, aa.SGST, aa.CGST, b.ProjectNumber }).ToList();
                        receiptAmt = QryReceipt.Sum(m => m.ReceiptAmount ?? 0);
                        receiptAmt = receiptAmt / 100000;
                        TtlPrevRecAmt += QryRecAMt;
                        TtlCurrRecAmt += receiptAmt;
                        decimal TotalRecAmt = receiptAmt + QryRecAMt;
                        TtlPrevProj_A += PrevProj_A;
                        TtlCurrProj_A += CurrProj_A;
                        TtlPrevProj_B += PrevProj_B;
                        TtlCurrProj_B += CurrProj_B;
                        TtlPrevAmt_A += AmtforPrevMonth_A;
                        TtlCurrAmt_A += AmtforCurMonth_A;
                        TtlPrevAmt_B += AmtforPrevMonth_B;
                        TtlCurrAmt_B += AmtforCurMonth_B;
                        var LbAmtforCurMonth_A = ""; var LbAmtforCurMonth_B = "";
                        if (Enhance_A > 0)
                            LbAmtforCurMonth_A = "*" + String.Format("{0:0.##}", +AmtforCurMonth_A);
                        else
                            LbAmtforCurMonth_A = String.Format("{0:0.##}", AmtforCurMonth_A);
                        if (Enhance_B > 0)
                            LbAmtforCurMonth_B = "*" + String.Format("{0:0.##}", +AmtforCurMonth_B);
                        else
                            LbAmtforCurMonth_B = String.Format("{0:0.##}", AmtforCurMonth_B);
                        var LbAmtforPrevMonth_A = ""; var LbAmtforPrevMonth_B = "";
                        if (PrevEnhance_A > 0)
                            LbAmtforPrevMonth_A = "*" + String.Format("{0:0.00}", AmtforPrevMonth_A);
                        else
                            LbAmtforPrevMonth_A = String.Format("{0:0.00}", AmtforPrevMonth_A);
                        if (PrevEnhance_B > 0)
                            LbAmtforPrevMonth_B = "*" + String.Format("{0:0.00}", AmtforPrevMonth_B);
                        else
                            LbAmtforPrevMonth_B = String.Format("{0:0.00}", AmtforPrevMonth_B);
                        List.Add(new OfficeMonthlyListReportModel()
                        {
                            DepartmentName = "ICSR",
                            Type = Type,
                            NoOfProjCurMonth_A = CurrProj_A ?? 0,
                            NoOfProjPrevMonth_A = PrevProj_A ?? 0,
                            TotalProj_A = TotalProj_A ?? 0,
                            AmtforCurMonth_A = LbAmtforCurMonth_A,
                            AmtforPrevMonth_A = LbAmtforPrevMonth_A,
                            TotalAmt_A = TotalAmt_A,
                            NoOfProjCurMonth_B = CurrProj_B ?? 0,
                            NoOfProjPrevMonth_B = PrevProj_B ?? 0,
                            TotalProj_B = TotalProj_B ?? 0,
                            AmtforCurMonth_B = LbAmtforCurMonth_B,
                            AmtforPrevMonth_B = LbAmtforPrevMonth_B,
                            TotalAmt_B = TotalAmt_B,
                            AmtforRecCurMonth = receiptAmt,
                            AmtforRecPrevMonth = QryRecAMt,
                            TotalRecAmt = TotalRecAmt
                        });

                    }
                    ////////Non Center ////////
                    var NonCenter = context.tblCommonHeads.Where(m => m.CategoryId == 2 && !Center.Contains(m.HeadId) && (m.GroupId == 2 || m.GroupId == 5)).ToList();
                    for (int a = 0; a < NonCenter.Count(); a++)
                    {
                        var DeptName = NonCenter[a].Head;
                        int DeptCode = NonCenter[a].HeadId;
                       
                        decimal TotalAmt_A = 0; decimal AmtforPrevMonth_A = 0; decimal AmtforCurMonth_A = 0; decimal receiptAmt = 0; decimal QryRecAMt = 0;
                        decimal TotalAmt_B = 0; decimal AmtforPrevMonth_B = 0; decimal AmtforCurMonth_B = 0;
                        //////A///////
                        var ProjDet_A = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= todate && m.FinancialYear == FinId && m.FacultyDetailId == DeptCode && m.Status == "Active" && m.ProjectType == 1 && m.SchemeName != 33 && !m.ProjectNumber.Contains("ISRO")).ToList();
                        var PrevProjDet_A = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.FacultyDetailId == DeptCode && m.Status == "Active" && m.ProjectType == 1 && m.SchemeName != 33 && !m.ProjectNumber.Contains("ISRO")).ToList();
                        int[] EnhanceArr_A = ProjDet_A.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_A = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                             //&& !EnhanceArr_A.Contains(aa.ProjectId ?? 0)
                                             && !b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true && b.FacultyDetailId == DeptCode && b.ProjectType == 1 && b.SchemeName != 33
                                             select aa).ToList();
                        decimal Enhance_A = ListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        var PrevListEnhance_A = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate && !EnhanceArr_A.Contains(aa.ProjectId ?? 0)
                                                 && !b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true && b.FacultyDetailId == DeptCode && b.ProjectType == 1 && b.SchemeName != 33
                                                 select aa).ToList();
                        decimal PrevEnhance_A = PrevListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        PrevEnhance_A = (PrevEnhance_A / 100000);
                        Enhance_A = (Enhance_A / 100000);
                        AmtforCurMonth_A = (ProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_A = AmtforCurMonth_A + Enhance_A;
                        AmtforPrevMonth_A = (PrevProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_A = AmtforPrevMonth_A + PrevEnhance_A;
                        TotalAmt_A = AmtforCurMonth_A + AmtforPrevMonth_A;
                        int? PrevProj_A = PrevProjDet_A.Count();
                        int? CurrProj_A = ProjDet_A.Count();
                        int? TotalProj_A = PrevProj_A + CurrProj_A;
                        ////// B //////
                        var ProjDet_B = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= todate && m.FinancialYear == FinId && m.FacultyDetailId == DeptCode && m.ProjectType == 1 && m.SchemeName == 33 && !m.ProjectNumber.Contains("ISRO")).ToList();
                        var PrevProjDet_B = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.FacultyDetailId == DeptCode && m.ProjectType == 1 && m.SchemeName == 33 && !m.ProjectNumber.Contains("ISRO")).ToList();
                        int[] EnhanceArr_B = ProjDet_B.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_B = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                             //&& !EnhanceArr_B.Contains(aa.ProjectId ?? 0)
                                             && !b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true && b.FacultyDetailId == DeptCode && b.ProjectType == 1 && b.SchemeName == 33
                                             select aa).ToList();
                        decimal Enhance_B = ListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        var PrevListEnhance_B = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate && !EnhanceArr_B.Contains(aa.ProjectId ?? 0)
                                               && !b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true && b.FacultyDetailId == DeptCode && b.ProjectType == 1 && b.SchemeName == 33
                                                 select aa).ToList();
                        decimal PrevEnhance_B = PrevListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        PrevEnhance_B = (PrevEnhance_B / 100000);
                        Enhance_B = (Enhance_B / 100000);
                        AmtforCurMonth_B = (ProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_B = AmtforCurMonth_B + Enhance_B;
                        AmtforPrevMonth_B = (PrevProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_B = AmtforPrevMonth_B + PrevEnhance_B;
                        TotalAmt_B = AmtforCurMonth_B + AmtforPrevMonth_B;
                        int? PrevProj_B = PrevProjDet_B.Count();
                        int? CurrProj_B = ProjDet_B.Count();
                        int? TotalProj_B = PrevProj_B + CurrProj_B;
                        //int[] ProjDet = context.tblProject.Where(m => m.CrtdTS >= fromdate && m.FinancialYear == FinId && m.ProjectSubType != 1 && m.CrtdTS <= todate && m.FacultyDetailId == DeptCode && m.Status == "Active" && m.ProjectType == 1).Select(m => m.ProjectId).ToArray();
                        // int[] PrevProjDet = context.tblProject.Where(m => m.CrtdTS >= startDate && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.ProjectSubType != 1 && m.FacultyDetailId == DeptCode && m.Status == "Active" && m.ProjectType == 1).Select(m => m.ProjectId).ToArray();
                        int[] ProjClass = { 4, 5, 6, 7 };
                        var QryRect = (from aa in context.tblReceipt
                                       join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                       join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                       where b.FacultyDetailId == DeptCode && aa.CrtdTS >= startDate && aa.CrtdTS <= endDate
                                       && b.ProjectType == 1 && aa.Status == "Completed" && !aa.ReceiptNumber.Contains("RBU")
                                          && ReceiptCateg.Contains(b.ReportClassification ?? 0) && aa.Posted_f == true
                                       && !Categ.Contains(aa.CategoryId ?? 0) && aa.Posted_f == true && !b.ProjectNumber.Contains("ISRO")
                                       && ((b.ProjectNumber.StartsWith("DIA") || b.FacultyDetailId == 67) || (
                                        b.FacultyDetailId == DeptCode && aa.CrtdTS >= startDate && aa.CrtdTS <= endDate
                                       && b.ProjectType == 1 && aa.Status == "Completed" && !aa.ReceiptNumber.Contains("RBU")
                                       && !Categ.Contains(aa.CategoryId ?? 0) && !b.ProjectNumber.Contains("ISRO")
                                          && ReceiptCateg.Contains(b.ReportClassification ?? 0) && aa.Posted_f == true
                                  ))
                                       select new { aa.ReceiptAmount, aa.ReceiptOverheadValue, aa.IGST, aa.SGST, aa.CGST, b.ProjectNumber }).ToList();
                        QryRecAMt = QryRect.Sum(m => m.ReceiptAmount ?? 0);

                        QryRecAMt = (QryRecAMt) / 100000;
                        var QryReceipt = (from aa in context.tblReceipt
                                          join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                          join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where b.FacultyDetailId == DeptCode && aa.CrtdTS >= fromdate && aa.CrtdTS <= todate
                                          && b.ProjectType == 1 && aa.Status == "Completed" && !aa.ReceiptNumber.Contains("RBU")
                                          && ReceiptCateg.Contains(b.ReportClassification ?? 0) && aa.Posted_f == true
                                          && !Categ.Contains(aa.CategoryId ?? 0) && !b.ProjectNumber.Contains("ISRO")
                                          && ((b.ProjectNumber.StartsWith("DIA") || b.FacultyDetailId == 67) ||  (
                                          b.FacultyDetailId == DeptCode && aa.CrtdTS >= fromdate && aa.CrtdTS <= todate
                                       && b.ProjectType == 1 && aa.Status == "Completed" && !aa.ReceiptNumber.Contains("RBU")
                                       && !Categ.Contains(aa.CategoryId ?? 0) && !b.ProjectNumber.Contains("ISRO")
                                          && ReceiptCateg.Contains(b.ReportClassification ?? 0) && aa.Posted_f == true
                                      ))
                                          select new { aa.ReceiptAmount, aa.ReceiptOverheadValue, aa.IGST, aa.SGST, aa.CGST, b.ProjectNumber }).ToList();
                        receiptAmt = QryReceipt.Sum(m => m.ReceiptAmount ?? 0);
                        receiptAmt = receiptAmt / 100000;
                        TtlPrevRecAmt += QryRecAMt;
                        TtlCurrRecAmt += receiptAmt;
                        decimal TotalRecAmt = receiptAmt + QryRecAMt;
                        TtlPrevProj_A += PrevProj_A;
                        TtlCurrProj_A += CurrProj_A;
                        TtlPrevProj_B += PrevProj_B;
                        TtlCurrProj_B += CurrProj_B;
                        TtlPrevAmt_A += AmtforPrevMonth_A;
                        TtlCurrAmt_A += AmtforCurMonth_A;
                        TtlPrevAmt_B += AmtforPrevMonth_B;
                        TtlCurrAmt_B += AmtforCurMonth_B;
                        var LbAmtforCurMonth_A = ""; var LbAmtforCurMonth_B = "";
                        if (Enhance_A > 0)
                            LbAmtforCurMonth_A = "*" + String.Format("{0:0.##}", +AmtforCurMonth_A);
                        else
                            LbAmtforCurMonth_A = String.Format("{0:0.##}", AmtforCurMonth_A);
                        if (Enhance_B > 0)
                            LbAmtforCurMonth_B = "*" + String.Format("{0:0.##}", +AmtforCurMonth_B);
                        else
                            LbAmtforCurMonth_B = String.Format("{0:0.##}", AmtforCurMonth_B);
                        var LbAmtforPrevMonth_A = ""; var LbAmtforPrevMonth_B = "";
                        if (PrevEnhance_A > 0)
                            LbAmtforPrevMonth_A = "*" + String.Format("{0:0.00}", AmtforPrevMonth_A);
                        else
                            LbAmtforPrevMonth_A = String.Format("{0:0.00}", AmtforPrevMonth_A);
                        if (PrevEnhance_B > 0)
                            LbAmtforPrevMonth_B = "*" + String.Format("{0:0.00}", AmtforPrevMonth_B);
                        else
                            LbAmtforPrevMonth_B = String.Format("{0:0.00}", AmtforPrevMonth_B);
                        if (AmtforCurMonth_A > 0 || AmtforCurMonth_B > 0 || TotalAmt_A > 0 || TotalAmt_B > 0|| receiptAmt!=0 || TotalRecAmt != 0)
                        {
                            List.Add(new OfficeMonthlyListReportModel()
                            {
                                DepartmentName = DeptName,
                                Type = Type,
                                NoOfProjCurMonth_A = CurrProj_A ?? 0,
                                NoOfProjPrevMonth_A = PrevProj_A ?? 0,
                                TotalProj_A = TotalProj_A ?? 0,
                                AmtforCurMonth_A = LbAmtforCurMonth_A,
                                AmtforPrevMonth_A = LbAmtforPrevMonth_A,
                                TotalAmt_A = TotalAmt_A,
                                NoOfProjCurMonth_B = CurrProj_B ?? 0,
                                NoOfProjPrevMonth_B = PrevProj_B ?? 0,
                                TotalProj_B = TotalProj_B ?? 0,
                                AmtforCurMonth_B = LbAmtforCurMonth_B,
                                AmtforPrevMonth_B = LbAmtforPrevMonth_B,
                                TotalAmt_B = TotalAmt_B,
                                AmtforRecCurMonth = receiptAmt,
                                AmtforRecPrevMonth = QryRecAMt,
                                TotalRecAmt = TotalRecAmt
                            });
                        }

                    }
                    /////// ISRO ////////

                    //var DeptName = Center[a].Head;
                    //int DeptCode = Center[a].HeadId;
                    decimal ISROTotalAmt_A = 0; decimal ISROAmtforPrevMonth_A = 0; decimal ISROAmtforCurMonth_A = 0; decimal ISROreceiptAmt = 0; decimal ISROQryRecAMt = 0;
                    decimal ISROTotalAmt_B = 0; decimal ISROAmtforPrevMonth_B = 0; decimal ISROAmtforCurMonth_B = 0;
                    //////A///////
                    var ISROProjDet_A = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= todate && m.FinancialYear == FinId && m.ProjectType == 1 && m.SchemeName != 33 && m.ProjectNumber.Contains("ISRO")).ToList();
                    var ISROPrevProjDet_A = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.ProjectType == 1 && m.SchemeName != 33 && m.ProjectNumber.Contains("ISRO")).ToList();
                    int[] ISROEnhanceArr_A = ISROProjDet_A.Select(m => m.ProjectId).ToArray();
                    var ISROListEnhance_A = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                             //&& !ISROEnhanceArr_A.Contains(aa.ProjectId ?? 0)
                                             && b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true && b.ProjectType == 1 && b.SchemeName != 33
                                             select aa).ToList();
                    decimal ISROEnhance_A = ISROListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                    var ISROPrevListEnhance_A = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate && !ISROEnhanceArr_A.Contains(aa.ProjectId ?? 0)
                                                 && b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true && b.ProjectType == 1 && b.SchemeName != 33
                                                 select aa).ToList();
                    decimal ISROPrevEnhance_A = ISROPrevListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                    ISROPrevEnhance_A = (ISROPrevEnhance_A / 100000);
                    ISROEnhance_A = (ISROEnhance_A / 100000);
                    ISROAmtforCurMonth_A = (ISROProjDet_A.Sum(m =>
                    {
                        return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                    }) / 100000) ?? 0;
                    ISROAmtforCurMonth_A = ISROAmtforCurMonth_A + ISROEnhance_A;
                    ISROAmtforPrevMonth_A = (ISROPrevProjDet_A.Sum(m =>
                    {
                        return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                    }) / 100000) ?? 0;
                    ISROAmtforPrevMonth_A = ISROAmtforPrevMonth_A + ISROPrevEnhance_A;
                    ISROTotalAmt_A = ISROAmtforCurMonth_A + ISROAmtforPrevMonth_A;
                    int? ISROPrevProj_A = ISROPrevProjDet_A.Count();
                    int? ISROCurrProj_A = ISROProjDet_A.Count();
                    int? ISROTotalProj_A = ISROPrevProj_A + ISROCurrProj_A;
                    ////// B //////
                    var ISROProjDet_B = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= todate && m.FinancialYear == FinId && m.ProjectType == 1 && m.SchemeName == 33 && m.ProjectNumber.Contains("ISRO")).ToList();
                    var ISROPrevProjDet_B = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.ProjectType == 1 && m.SchemeName == 33 && m.ProjectNumber.Contains("ISRO")).ToList();
                    int[] ISROEnhanceArr_B = ISROProjDet_B.Select(m => m.ProjectId).ToArray();
                    var ISROListEnhance_B = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                             //&& !ISROEnhanceArr_B.Contains(aa.ProjectId ?? 0)
                                             && b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true && b.ProjectType == 1 && b.SchemeName == 33
                                             select aa).ToList();
                    decimal ISROEnhance_B = ISROListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                    var ISROPrevListEnhance_B = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate && !ISROEnhanceArr_B.Contains(aa.ProjectId ?? 0)
                                               && b.ProjectNumber.Contains("ISRO") && aa.IsEnhancementonly == true && b.ProjectType == 1 && b.SchemeName == 33
                                                 select aa).ToList();
                    decimal ISROPrevEnhance_B = ISROPrevListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                    ISROPrevEnhance_B = (ISROPrevEnhance_B / 100000);
                    ISROEnhance_B = (ISROEnhance_B / 100000);
                    ISROAmtforCurMonth_B = (ISROProjDet_B.Sum(m =>
                    {
                        return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                    }) / 100000) ?? 0;
                    ISROAmtforCurMonth_B = ISROAmtforCurMonth_B + ISROEnhance_B;
                    ISROAmtforPrevMonth_B = (ISROPrevProjDet_B.Sum(m =>
                    {
                        return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                    }) / 100000) ?? 0;
                    ISROAmtforPrevMonth_B = ISROAmtforPrevMonth_B + ISROPrevEnhance_B;
                    ISROTotalAmt_B = ISROAmtforCurMonth_B + ISROAmtforPrevMonth_B;
                    int? ISROPrevProj_B = ISROPrevProjDet_B.Count();
                    int? ISROCurrProj_B = ISROProjDet_B.Count();
                    int? ISROTotalProj_B = ISROPrevProj_B + ISROCurrProj_B;
                    // int[] ISROProjDet = context.tblProject.Where(m => m.CrtdTS >= fromdate && m.FinancialYear == FinId && m.ProjectSubType != 1 && m.CrtdTS <= todate && m.Status == "Active" && m.ProjectType == 1).Select(m => m.ProjectId).ToArray();
                    //   int[] ISROPrevProjDet = context.tblProject.Where(m => m.CrtdTS >= startDate && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.ProjectSubType != 1 && m.Status == "Active" && m.ProjectType == 1).Select(m => m.ProjectId).ToArray();
                    int[] ISROProjClass = { 4, 5, 6, 7 };
                    var ISROQryRect = (from aa in context.tblReceipt
                                       join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                       join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                       where aa.CrtdTS >= startDate && aa.CrtdTS <= endDate && aa.Posted_f == true
                                     && !Categ.Contains(aa.CategoryId ?? 0) && b.ProjectType == 1
                                     && aa.Status == "Completed" && !aa.ReceiptNumber.Contains("RBU")
                                        && b.ProjectNumber.Contains("ISRO") && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                       select new { aa.ReceiptAmount, aa.ReceiptOverheadValue, aa.IGST, aa.SGST, aa.CGST, b.ProjectNumber }).ToList();
                    ISROQryRecAMt = ISROQryRect.Sum(m => m.ReceiptAmount ?? 0);

                    ISROQryRecAMt = (ISROQryRecAMt) / 100000;
                    var ISROQryReceipt = (from aa in context.tblReceipt
                                          join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                          join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where aa.CrtdTS >= fromdate && aa.CrtdTS <= todate && aa.Posted_f == true
                                        && !Categ.Contains(aa.CategoryId ?? 0) && b.ProjectType == 1
                                        && aa.Status == "Completed" && !aa.ReceiptNumber.Contains("RBU")
                                         && b.ProjectNumber.Contains("ISRO") && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                          select new { aa.ReceiptAmount, aa.ReceiptOverheadValue, aa.IGST, aa.SGST, aa.CGST, b.ProjectNumber }).ToList();
                    ISROreceiptAmt = ISROQryReceipt.Sum(m => m.ReceiptAmount ?? 0);
                    ISROreceiptAmt = ISROreceiptAmt / 100000;
                    TtlPrevRecAmt += ISROQryRecAMt;
                    TtlCurrRecAmt += ISROreceiptAmt;
                    decimal ISROTotalRecAmt = ISROreceiptAmt + ISROQryRecAMt;
                    TtlPrevProj_A += ISROPrevProj_A;
                    TtlCurrProj_A += ISROCurrProj_A;
                    TtlPrevProj_B += ISROPrevProj_B;
                    TtlCurrProj_B += ISROCurrProj_B;
                    TtlPrevAmt_A += ISROAmtforPrevMonth_A;
                    TtlCurrAmt_A += ISROAmtforCurMonth_A;
                    TtlPrevAmt_B += ISROAmtforPrevMonth_B;
                    TtlCurrAmt_B += ISROAmtforCurMonth_B;
                    var ISROLbAmtforCurMonth_A = ""; var ISROLbAmtforCurMonth_B = "";
                    if (ISROEnhance_A > 0)
                        ISROLbAmtforCurMonth_A = "*" + String.Format("{0:0.##}", +ISROAmtforCurMonth_A);
                    else
                        ISROLbAmtforCurMonth_A = String.Format("{0:0.##}", ISROAmtforCurMonth_A);
                    if (ISROEnhance_B > 0)
                        ISROLbAmtforCurMonth_B = "*" + String.Format("{0:0.##}", +ISROAmtforCurMonth_B);
                    else
                        ISROLbAmtforCurMonth_B = String.Format("{0:0.##}", ISROAmtforCurMonth_B);
                    var ISROLbAmtforPrevMonth_A = ""; var ISROLbAmtforPrevMonth_B = "";
                    if (ISROPrevEnhance_A > 0)
                        ISROLbAmtforPrevMonth_A = "*" + String.Format("{0:0.00}", ISROAmtforPrevMonth_A);
                    else
                        ISROLbAmtforPrevMonth_A = String.Format("{0:0.00}", ISROAmtforPrevMonth_A);
                    if (ISROPrevEnhance_B > 0)
                        ISROLbAmtforPrevMonth_B = "*" + String.Format("{0:0.00}", ISROAmtforPrevMonth_B);
                    else
                        ISROLbAmtforPrevMonth_B = String.Format("{0:0.00}", ISROAmtforPrevMonth_B);

                    //Changes done #7448,#7476  12/09/2022 to fix the total issue 
                    Decimal DecTotalAmt_A = Convert.ToDecimal(String.Join(string.Empty, ISROLbAmtforCurMonth_A.Where(x => x != '*')))
                    + Convert.ToDecimal(String.Join(string.Empty, ISROLbAmtforPrevMonth_A.Where(x => x != '*')));

                    List.Add(new OfficeMonthlyListReportModel()
                    {
                        DepartmentName = "ISRO -IITM CELL",
                        Type = Type,
                        NoOfProjCurMonth_A = ISROCurrProj_A ?? 0,
                        NoOfProjPrevMonth_A = ISROPrevProj_A ?? 0,
                        TotalProj_A = ISROTotalProj_A ?? 0,
                        AmtforCurMonth_A = ISROLbAmtforCurMonth_A,
                        AmtforPrevMonth_A = ISROLbAmtforPrevMonth_A,
                        NoOfProjCurMonth_B = ISROCurrProj_B ?? 0,
                        NoOfProjPrevMonth_B = ISROPrevProj_B ?? 0,
                        //Changes done #7448,#7476 12/09/2022 to fix the total issue
                        TotalAmt_A = DecTotalAmt_A,
                        TotalProj_B = ISROTotalProj_B ?? 0,
                        AmtforCurMonth_B = ISROLbAmtforCurMonth_B,
                        AmtforPrevMonth_B = ISROLbAmtforPrevMonth_B,
                        TotalAmt_B = ISROTotalAmt_B,
                        AmtforRecCurMonth = ISROreceiptAmt,
                        AmtforRecPrevMonth = ISROQryRecAMt,
                        TotalRecAmt = ISROTotalRecAmt
                    });

                    //////////////
                    TtlProj_A = TtlPrevProj_A + TtlCurrProj_A;
                    TtlProj_B = TtlPrevProj_B + TtlCurrProj_B;
                    TtlPrevProj = TtlPrevProj_A + TtlPrevProj_B;
                    TtlCurrProj = TtlPrevProj_A + TtlPrevProj_B;
                    TtlProj = TtlProj_A + TtlProj_B;
                    TtlAmt_A = TtlPrevAmt_A + TtlCurrAmt_A;
                    TtlAmt_B = TtlPrevAmt_B + TtlCurrAmt_B;
                    TtlPrevAmt = TtlPrevAmt_A + TtlPrevAmt_B;
                    TtlCurrAmt = TtlCurrAmt_A + TtlCurrAmt_B;
                    TtlAmt = TtlAmt_A + TtlAmt_B;
                    TtlRecAmt = TtlCurrRecAmt + TtlPrevRecAmt;
                    List.Add(new OfficeMonthlyListReportModel()
                    {
                        DepartmentName = "Total",
                        Type = Type,
                        NoOfProjCurMonth_A = TtlCurrProj_A ?? 0,
                        NoOfProjPrevMonth_A = TtlPrevProj_A ?? 0,
                        TotalProj_A = TtlProj_A ?? 0,
                        AmtforCurMonth_A = String.Format("{0:0.##}", TtlCurrAmt_A),
                        AmtforPrevMonth_A = String.Format("{0:0.##}", TtlPrevAmt_A),
                        TotalAmt_A = TtlAmt_A ?? 0,
                        NoOfProjCurMonth_B = TtlCurrProj_B ?? 0,
                        NoOfProjPrevMonth_B = TtlPrevProj_B ?? 0,
                        TotalProj_B = TtlProj_B ?? 0,
                        AmtforCurMonth_B = String.Format("{0:0.##}", TtlCurrAmt_B),
                        AmtforPrevMonth_B = String.Format("{0:0.##}", TtlPrevAmt_B),
                        TotalAmt_B = TtlAmt_B ?? 0,
                        AmtforRecCurMonth = TtlCurrRecAmt ?? 0,
                        AmtforRecPrevMonth = TtlPrevRecAmt ?? 0,
                        TotalRecAmt = TtlRecAmt ?? 0
                    });
                    model.TotalAmount = TtlCurrAmt ?? 0;
                    model.TotalPrevAmt = TtlPrevAmt ?? 0;
                    if (fromdate.Month != 4)
                    {
                        model.FromDate = String.Format("{0:MMM yyyy}", startDate);
                        model.ToDate = String.Format("{0:MMM yyyy}", endDate);
                    }

                    model.List = List;

                    decimal checktoa = Math.Round((model.List.Sum(m => m.AmtforRecCurMonth) / 2), 2);

                }
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion
        #region Office Monthly Report
        public static OfficeMonthlyReportModel GetOfficeMonthlyReport(string Month)
        {
            OfficeMonthlyReportModel model = new OfficeMonthlyReportModel();
            List<OfficeMonthlyListReportModel> List = new List<OfficeMonthlyListReportModel>();
            string Type = "";
            try
            {
                using (var context = new IOASDBEntities())
                {
                    FinOp fac = new FinOp(System.DateTime.Now);
                    var fromdate = fac.GetMonthFirstDate(Month);
                    var todate = fac.GetMonthLastDate(Month);
                    todate = todate.AddDays(1).AddTicks(-2);
                    DateTime startDate; // 1st Feb this year
                    DateTime endDate;
                    if (fromdate.Month > 3)
                    {
                        if (fromdate.Month == 4)
                        {
                            endDate = fromdate.AddMonths(-1).AddTicks(-2);
                            startDate = new DateTime(fromdate.Year, 4, 1);
                        }
                        else
                        {
                            startDate = new DateTime(fromdate.Year, 4, 1);
                            endDate = fromdate.AddTicks(-2);
                        }
                    }
                    else
                    {
                        startDate = new DateTime((fromdate.Year) - 1, 4, 1);
                        endDate = fromdate.AddTicks(-2);
                    }
                    int CurrentYear = fromdate.Year;
                    int PreviousYear = fromdate.Year - 1;
                    int NextYear = fromdate.Year + 1;
                    string PreYear = PreviousYear.ToString();
                    string NexYear = NextYear.ToString();
                    string CurYear = CurrentYear.ToString();
                    string FinYear = null;
                    if (fromdate.Month > 3)
                        FinYear = CurYear.Substring(2) + NexYear.Substring(2);
                    else
                        FinYear = PreYear.Substring(2) + CurYear.Substring(2);
                    int FinId = context.tblFinYear.Where(m => m.Year == FinYear).Select(m => m.FinYearId).FirstOrDefault();
                    var QryDept = context.tblProject.Select(m => m.PIDepartment).Distinct().ToList();
                    var Qry = (from c in context.tblProject
                               join d in context.tblFacultyDetail on c.PIDepartment equals d.DepartmentCode
                               where !String.IsNullOrEmpty(d.DepartmentCode) && !String.IsNullOrEmpty(d.DepartmentName) && !String.IsNullOrEmpty(c.PIDepartment)
                               select new { c.PIDepartment, d.DepartmentName }).Distinct().ToList();
                    int[] A1 = { 5, 8, 10, 11 };
                    int[] B1 = { 7 };
                    int[] C1 = { 12, 14, 24, 30 };
                    int[] D1 = { 9 };
                    int[] Categ = { 1, 14, 13, 16 };
                    int[] OfficeCateg = { 2, 3 };
                    int[] ReceiptCateg = { 1, 3 };
                    int? TtlPrevProj_A = 0; int? TtlCurrProj_A = 0; int? TtlProj_A = 0;
                    int? TtlPrevProj_B = 0; int? TtlCurrProj_B = 0; int? TtlProj_B = 0;
                    int? TtlPrevProj_C = 0; int? TtlCurrProj_C = 0; int? TtlProj_C = 0;
                    int? TtlPrevProj_D = 0; int? TtlCurrProj_D = 0; int? TtlProj_D = 0;
                    int? TtlPrevProj = 0; int? TtlCurrProj = 0; int? TtlProj = 0;


                    decimal? TtlPrevAmt_A = 0; decimal? TtlCurrAmt_A = 0; decimal? TtlAmt_A = 0;
                    decimal? TtlPrevAmt_B = 0; decimal? TtlCurrAmt_B = 0; decimal? TtlAmt_B = 0;
                    decimal? TtlPrevAmt_C = 0; decimal? TtlCurrAmt_C = 0; decimal? TtlAmt_C = 0;
                    decimal? TtlPrevAmt_D = 0; decimal? TtlCurrAmt_D = 0; decimal? TtlAmt_D = 0;
                    decimal? TtlPrevAmt = 0; decimal? TtlCurrAmt = 0; decimal? TtlAmt = 0;
                    decimal? TtlPrevRecAmt = 0; decimal? TtlCurrRecAmt = 0; decimal? TtlRecAmt = 0;

                    //Ex
                    decimal? ExTtlPrevAmt_A = 0; decimal? ExTtlCurrAmt_A = 0; decimal? ExTtlAmt_A = 0;
                    decimal? ExTtlPrevAmt_B = 0; decimal? ExTtlCurrAmt_B = 0; decimal? ExTtlAmt_B = 0;
                    decimal? ExTtlPrevAmt_C = 0; decimal? ExTtlCurrAmt_C = 0; decimal? ExTtlAmt_C = 0;
                    decimal? ExTtlPrevAmt_D = 0; decimal? ExTtlCurrAmt_D = 0; decimal? ExTtlAmt_D = 0;
                    decimal? ExTtlPrevAmt = 0; decimal? ExTtlCurrAmt = 0; decimal? ExTtlAmt = 0;
                    decimal? ExTtlPrevRecAmt = 0; decimal? ExTtlCurrRecAmt = 0; decimal? ExTtlRecAmt = 0;

                    for (int i = 0; i < Qry.Count(); i++)
                    {
                        var DeptName = Qry[i].DepartmentName;

                        var DeptCode = Qry[i].PIDepartment;
                        string ss;
                        if (DeptCode.Contains("OE"))
                            ss = "";

                        ////// A //////
                        decimal TotalAmt_A = 0; decimal AmtforPrevMonth_A = 0; decimal AmtforCurMonth_A = 0; decimal receiptAmt = 0; decimal QryRecAMt = 0;
                        decimal ExTotalAmt_A = 0; decimal ExAmtforPrevMonth_A = 0; decimal ExAmtforCurMonth_A = 0; decimal ExreceiptAmt = 0; decimal ExQryRecAMt = 0;
                        decimal TotalAmt_B = 0; decimal AmtforPrevMonth_B = 0; decimal AmtforCurMonth_B = 0;
                        decimal ExTotalAmt_B = 0; decimal ExAmtforPrevMonth_B = 0; decimal ExAmtforCurMonth_B = 0;
                        decimal TotalAmt_C = 0; decimal AmtforPrevMonth_C = 0; decimal AmtforCurMonth_C = 0;
                        decimal ExTotalAmt_C = 0; decimal ExAmtforPrevMonth_C = 0; decimal ExAmtforCurMonth_C = 0;
                        decimal TotalAmt_D = 0; decimal AmtforPrevMonth_D = 0; decimal AmtforCurMonth_D = 0;
                        decimal ExTotalAmt_D = 0; decimal ExAmtforPrevMonth_D = 0; decimal ExAmtforCurMonth_D = 0;

                        var ProjDet_A = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= todate
                        && m.FacultyDetailId == null && m.FinancialYear == FinId && m.PIDepartment == DeptCode
                        && m.ProjectClassification != 16 && m.ProjectType == 2
                        && A1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        var PrevProjDet_A = context.tblProject.Where(m => m.CrtdTS >= startDate
                         && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= endDate
                        && m.FacultyDetailId == null && m.FinancialYear == FinId && m.PIDepartment == DeptCode
                         && m.ProjectClassification != 16 && m.ProjectType == 2
                        && A1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_A = ProjDet_A.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_A = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && b.FacultyDetailId == null && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                             //&& !EnhanceArr_A.Contains(aa.ProjectId ?? 0)
                                         && b.ProjectClassification != 16 && aa.IsEnhancementonly == true && c.DepartmentCode == DeptCode
                                         && b.ProjectType == 2 && A1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_A = ListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_A = ListEnhance_A.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_A = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && b.FacultyDetailId == null && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                             && b.ProjectClassification != 16 && aa.IsEnhancementonly == true && c.DepartmentCode == DeptCode && b.ProjectType == 2 && A1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_A = prevListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_A = prevListEnhance_A.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        prevEnhance_A = (prevEnhance_A / 100000);
                        Enhance_A = (Enhance_A / 100000);
                        AmtforCurMonth_A = (ProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_A = AmtforCurMonth_A + Enhance_A;
                        AmtforPrevMonth_A = (PrevProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_A = AmtforPrevMonth_A + prevEnhance_A;
                        TotalAmt_A = AmtforCurMonth_A + AmtforPrevMonth_A;

                        //Ex

                        ExprevEnhance_A = (ExprevEnhance_A / 100000);
                        ExEnhance_A = (ExEnhance_A / 100000);
                        ExAmtforCurMonth_A = (ProjDet_A.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_A = ExAmtforCurMonth_A + ExEnhance_A;
                        ExAmtforPrevMonth_A = (PrevProjDet_A.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_A = ExAmtforPrevMonth_A + ExprevEnhance_A;
                        ExTotalAmt_A = ExAmtforCurMonth_A + ExAmtforPrevMonth_A;

                        int? PrevProj_A = PrevProjDet_A.Count();
                        int? CurrProj_A = ProjDet_A.Count();
                        int? TotalProj_A = PrevProj_A + CurrProj_A;
                        ////// B //////
                        var ProjDet_B = context.tblProject.Where(m => m.CrtdTS >= fromdate
                         && OfficeCateg.Contains(m.ReportClassification ?? 0)
                        && m.ProjectClassification != 16 && m.CrtdTS <= todate
                        && m.FacultyDetailId == null && m.FinancialYear == FinId && m.PIDepartment == DeptCode
                         && m.ProjectType == 2 && B1.Contains(m.ConsultancyFundingCategory ?? 0)
                 ).ToList();
                        var PrevProjDet_B = context.tblProject.Where(m => m.CrtdTS >= startDate
                         && OfficeCateg.Contains(m.ReportClassification ?? 0)
                        && m.ProjectClassification != 16 && m.CrtdTS <= endDate
                        && m.FacultyDetailId == null && m.PIDepartment == DeptCode && m.FinancialYear == FinId
                         && m.ProjectType == 2 && B1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_B = ProjDet_B.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_B = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && b.FacultyDetailId == null && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                              //&& !EnhanceArr_B.Contains(aa.ProjectId ?? 0)
                                              && b.ProjectClassification != 16 && aa.IsEnhancementonly == true && c.DepartmentCode == DeptCode && b.ProjectType == 2 && B1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_B = ListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_B = ListEnhance_B.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_B = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && b.FacultyDetailId == null && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                                 && b.ProjectClassification != 16 && aa.IsEnhancementonly == true && c.DepartmentCode == DeptCode && b.ProjectType == 2 && B1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_B = prevListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_B = prevListEnhance_B.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        prevEnhance_B = (prevEnhance_B / 100000);
                        Enhance_B = (Enhance_B / 100000);
                        AmtforCurMonth_B = (ProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_B = AmtforCurMonth_B + Enhance_B;
                        AmtforPrevMonth_B = (PrevProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_B = AmtforPrevMonth_B + prevEnhance_B;
                        TotalAmt_B = AmtforCurMonth_B + AmtforPrevMonth_B;

                        //Ex
                        ExprevEnhance_B = (ExprevEnhance_B / 100000);
                        ExEnhance_B = (ExEnhance_B / 100000);
                        ExAmtforCurMonth_B = (ProjDet_B.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_B = ExAmtforCurMonth_B + ExEnhance_B;
                        ExAmtforPrevMonth_B = (PrevProjDet_B.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_B = ExAmtforPrevMonth_B + ExprevEnhance_B;
                        ExTotalAmt_B = ExAmtforCurMonth_B + ExAmtforPrevMonth_B;


                        int? PrevProj_B = PrevProjDet_B.Count();
                        int? CurrProj_B = ProjDet_B.Count();
                        int? TotalProj_B = PrevProj_B + CurrProj_B;



                        ////// C //////
                        var ProjDet_C = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0)
                        && m.ProjectClassification != 16
                        && m.CrtdTS <= todate && m.FacultyDetailId == null && m.FinancialYear == FinId && m.PIDepartment == DeptCode
                        && m.ProjectType == 2 && C1.Contains(m.ConsultancyFundingCategory ?? 0)
                   ).ToList();
                        var PrevProjDet_C = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.ProjectClassification != 16
                        && m.CrtdTS <= endDate && m.FacultyDetailId == null && m.PIDepartment == DeptCode && m.FinancialYear == FinId
                         && m.ProjectType == 2 && C1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_C = ProjDet_C.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_C = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && b.FacultyDetailId == null && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                            //&& !EnhanceArr_C.Contains(aa.ProjectId ?? 0)
                                            && b.ProjectClassification != 16 && aa.IsEnhancementonly == true && c.DepartmentCode == DeptCode && b.ProjectType == 2 && C1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_C = ListEnhance_C.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_C = ListEnhance_C.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_C = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && b.FacultyDetailId == null && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                             && b.ProjectClassification != 16 && aa.IsEnhancementonly == true && c.DepartmentCode == DeptCode && b.ProjectType == 2 && C1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_C = prevListEnhance_C.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_C = prevListEnhance_C.Sum(m => m.EnhancedSanctionValue) ?? 0;

                        prevEnhance_C = (prevEnhance_C / 100000);
                        Enhance_C = (Enhance_C / 100000);
                        AmtforCurMonth_C = (ProjDet_C.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_C = AmtforCurMonth_C + Enhance_C;
                        AmtforPrevMonth_C = (PrevProjDet_C.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_C = AmtforPrevMonth_C + prevEnhance_C;
                        TotalAmt_C = AmtforCurMonth_C + AmtforPrevMonth_C;

                        //Ex
                        ExprevEnhance_C = (ExprevEnhance_C / 100000);
                        ExEnhance_C = (ExEnhance_C / 100000);
                        ExAmtforCurMonth_C = (ProjDet_C.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_C = ExAmtforCurMonth_C + ExEnhance_C;
                        ExAmtforPrevMonth_C = (PrevProjDet_C.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_C = ExAmtforPrevMonth_C + ExprevEnhance_C;
                        ExTotalAmt_C = ExAmtforCurMonth_C + ExAmtforPrevMonth_C;

                        int? PrevProj_C = PrevProjDet_C.Count();
                        int? CurrProj_C = ProjDet_C.Count();
                        int? TotalProj_C = PrevProj_C + CurrProj_C;
                        ////// D //////
                        var ProjDet_D = context.tblProject.Where(m => m.CrtdTS >= fromdate
                        && OfficeCateg.Contains(m.ReportClassification ?? 0)
                        && m.ProjectClassification != 16
                        && m.CrtdTS <= todate
                        && m.FacultyDetailId == null && m.FinancialYear == FinId &&
                        m.PIDepartment == DeptCode
                        && m.ProjectType == 2 && D1.Contains(m.ConsultancyFundingCategory ?? 0)
                        ).ToList();
                        var PrevProjDet_D = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0)
                        && m.ProjectClassification != 16 && m.CrtdTS <= endDate
                        && m.FacultyDetailId == null && m.PIDepartment == DeptCode
                        && m.FinancialYear == FinId &&
                        m.ProjectType == 2 && D1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_D = ProjDet_D.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_D = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && b.FacultyDetailId == null && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                             //&& !EnhanceArr_D.Contains(aa.ProjectId ?? 0)
                                             && b.ProjectClassification != 16 && aa.IsEnhancementonly == true && c.DepartmentCode == DeptCode && b.ProjectType == 2 && D1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_D = ListEnhance_D.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_D = ListEnhance_D.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_D = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && b.FacultyDetailId == null && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                               && b.ProjectClassification != 16 && aa.IsEnhancementonly == true
                                               && c.DepartmentCode == DeptCode && b.ProjectType == 2 && D1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_D = prevListEnhance_D.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_D = prevListEnhance_D.Sum(m => m.EnhancedSanctionValue) ?? 0;

                        prevEnhance_D = (prevEnhance_D / 100000);
                        Enhance_D = (Enhance_D / 100000);
                        AmtforCurMonth_D = (ProjDet_D.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_D = AmtforCurMonth_D + Enhance_D;
                        AmtforCurMonth_D = 0;
                        AmtforPrevMonth_D = (PrevProjDet_D.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_D = AmtforPrevMonth_D + prevEnhance_D;
                        AmtforPrevMonth_D = 0;
                        TotalAmt_D = AmtforCurMonth_D + AmtforPrevMonth_D;

                        //Ex
                        ExprevEnhance_D = (ExprevEnhance_D / 100000);
                        ExEnhance_D = (ExEnhance_D / 100000);
                        ExAmtforCurMonth_D = (ProjDet_D.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_D = ExAmtforCurMonth_D + ExEnhance_D;
                        ExAmtforCurMonth_D = 0;
                        ExAmtforPrevMonth_D = (PrevProjDet_D.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_D = ExAmtforPrevMonth_D + ExprevEnhance_D;
                        ExAmtforPrevMonth_D = 0;
                        ExTotalAmt_D = ExAmtforCurMonth_D + ExAmtforPrevMonth_D;

                        int? PrevProj_D = PrevProjDet_D.Count();
                        PrevProj_D = 0;
                        int? CurrProj_D = ProjDet_D.Count();
                        CurrProj_D = 0;
                        int? TotalProj_D = PrevProj_D + CurrProj_D;
                        int[] ProjClass = { 2, 3, 14 };
                        int[] ProjDet = context.tblProject.Where(m => m.CrtdTS >= fromdate && m.FacultyDetailId == null
                        && m.CrtdTS <= todate && m.FinancialYear == FinId && m.PIDepartment == DeptCode
                         && m.ProjectType == 2).Select(m => m.ProjectId).ToArray();
                        int[] PrevProjDet = context.tblProject.Where(m => m.CrtdTS >= startDate && m.FacultyDetailId == null && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.PIDepartment == DeptCode && m.ProjectType == 2).Select(m => m.ProjectId).ToArray();
                        var QryRect = (from a in context.tblReceipt
                                       join b in context.tblProject on a.ProjectId equals b.ProjectId
                                       join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                       where b.FacultyDetailId == null && a.Posted_f == true
                                       //&& !(b.ConsultancyFundingCategory == 9) && !(b.ConsultancyFundingCategory == 12)
                                       && !Categ.Contains(a.CategoryId ?? 0) && b.ProjectClassification != 16 && c.DepartmentCode == DeptCode && a.CrtdTS >= startDate && a.CrtdTS <= endDate
                                       && b.ProjectType == 2 && a.Status == "Completed" && !a.ReceiptNumber.Contains("RBU")
                                       && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                       select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();

                        QryRecAMt = QryRect.Sum(m => m.ReceiptAmount ?? 0);
                        QryRecAMt = (QryRecAMt) / 100000;


                        ExQryRecAMt = (QryRect.Sum(m =>
                        {
                            return m.ReceiptAmount - ((m.CGST ?? 0) + (m.SGST ?? 0) + (m.IGST ?? 0));
                        }) / 100000) ?? 0;


                        var QryReceipt = (from a in context.tblReceipt
                                          join b in context.tblProject on a.ProjectId equals b.ProjectId
                                          join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where b.FacultyDetailId == null && a.Posted_f == true
                                          //&& !(b.ConsultancyFundingCategory == 9) && !(b.ConsultancyFundingCategory == 12)
                                         && !Categ.Contains(a.CategoryId ?? 0) && c.DepartmentCode == DeptCode && a.CrtdTS >= fromdate && a.CrtdTS <= todate
                                         && b.ProjectClassification != 16 && b.ProjectType == 2 && a.Status == "Completed" && !a.ReceiptNumber.Contains("RBU")
                                         && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                          select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                        receiptAmt = QryReceipt.Sum(m => m.ReceiptAmount ?? 0);
                        receiptAmt = (receiptAmt) / 100000;

                        ExreceiptAmt = (QryReceipt.Sum(m =>
                        {
                            return m.ReceiptAmount - ((m.CGST ?? 0) + (m.SGST ?? 0) + (m.IGST ?? 0));
                        }) / 100000) ?? 0;


                        TtlPrevRecAmt += QryRecAMt;
                        TtlCurrRecAmt += receiptAmt;
                        decimal TotalRecAmt = receiptAmt + QryRecAMt;
                        TtlPrevProj_A += PrevProj_A;
                        TtlCurrProj_A += CurrProj_A;
                        TtlPrevProj_B += PrevProj_B;
                        TtlCurrProj_B += CurrProj_B;
                        TtlPrevProj_C += PrevProj_C;
                        TtlCurrProj_C += CurrProj_C;
                        TtlPrevProj_D += PrevProj_D;
                        TtlCurrProj_D += CurrProj_D;
                        TtlPrevAmt_A += AmtforPrevMonth_A;
                        TtlCurrAmt_A += AmtforCurMonth_A;
                        TtlPrevAmt_B += AmtforPrevMonth_B;
                        TtlCurrAmt_B += AmtforCurMonth_B;
                        TtlPrevAmt_C += AmtforPrevMonth_C;
                        TtlCurrAmt_C += AmtforCurMonth_C;
                        TtlPrevAmt_D += AmtforPrevMonth_D;
                        TtlCurrAmt_D += AmtforCurMonth_D;


                        //Ex


                        ExTtlPrevRecAmt += ExQryRecAMt;
                        ExTtlCurrRecAmt += ExreceiptAmt;
                        decimal ExTotalRecAmt = ExreceiptAmt + ExQryRecAMt;

                        ExTtlPrevAmt_A += ExAmtforPrevMonth_A;
                        ExTtlCurrAmt_A += ExAmtforCurMonth_A;
                        ExTtlPrevAmt_B += ExAmtforPrevMonth_B;
                        ExTtlCurrAmt_B += ExAmtforCurMonth_B;
                        ExTtlPrevAmt_C += ExAmtforPrevMonth_C;
                        ExTtlCurrAmt_C += ExAmtforCurMonth_C;
                        ExTtlPrevAmt_D += ExAmtforPrevMonth_D;
                        ExTtlCurrAmt_D += ExAmtforCurMonth_D;


                        var LbAmtforCurMonth_A = ""; var LbAmtforCurMonth_B = ""; var LbAmtforCurMonth_C = ""; var LbAmtforCurMonth_D = "";

                        var ExLbAmtforCurMonth_A = ""; var ExLbAmtforCurMonth_B = ""; var ExLbAmtforCurMonth_C = ""; var ExLbAmtforCurMonth_D = "";


                        if (Enhance_A > 0)
                            LbAmtforCurMonth_A = "*" + String.Format("{0:0.00}", AmtforCurMonth_A);
                        else
                            LbAmtforCurMonth_A = String.Format("{0:0.00}", AmtforCurMonth_A);
                        if (Enhance_B > 0)
                            LbAmtforCurMonth_B = "*" + String.Format("{0:0.00}", AmtforCurMonth_B);
                        else
                            LbAmtforCurMonth_B = String.Format("{0:0.00}", AmtforCurMonth_B);
                        if (Enhance_C > 0)
                            LbAmtforCurMonth_C = "*" + String.Format("{0:0.00}", AmtforCurMonth_C);
                        else
                            LbAmtforCurMonth_C = String.Format("{0:0.00}", AmtforCurMonth_C);
                        if (Enhance_D > 0)
                            LbAmtforCurMonth_D = "*" + String.Format("{0:0.00}", AmtforCurMonth_D);
                        else
                            LbAmtforCurMonth_D = String.Format("{0:0.00}", AmtforCurMonth_D);
                        var LbAmtforPrevMonth_A = ""; var LbAmtforPrevMonth_B = ""; var LbAmtforPrevMonth_C = ""; var LbAmtforPrevMonth_D = "";
                        if (prevEnhance_A > 0)
                            LbAmtforPrevMonth_A = "*" + String.Format("{0:0.00}", AmtforPrevMonth_A);
                        else
                            LbAmtforPrevMonth_A = String.Format("{0:0.00}", AmtforPrevMonth_A);
                        if (prevEnhance_B > 0)
                            LbAmtforPrevMonth_B = "*" + String.Format("{0:0.00}", AmtforPrevMonth_B);
                        else
                            LbAmtforPrevMonth_B = String.Format("{0:0.00}", AmtforPrevMonth_B);
                        if (prevEnhance_C > 0)
                            LbAmtforPrevMonth_C = "*" + String.Format("{0:0.00}", AmtforPrevMonth_C);
                        else
                            LbAmtforPrevMonth_C = String.Format("{0:0.00}", AmtforPrevMonth_C);
                        if (prevEnhance_D > 0)
                            LbAmtforPrevMonth_D = "*" + String.Format("{0:0.00}", AmtforPrevMonth_D);
                        else
                            LbAmtforPrevMonth_D = String.Format("{0:0.00}", AmtforPrevMonth_D);

                        //Ex
                        if (ExEnhance_A > 0)
                            ExLbAmtforCurMonth_A = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_A);
                        else
                            ExLbAmtforCurMonth_A = String.Format("{0:0.00}", ExAmtforCurMonth_A);
                        if (ExEnhance_B > 0)
                            ExLbAmtforCurMonth_B = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_B);
                        else
                            ExLbAmtforCurMonth_B = String.Format("{0:0.00}", ExAmtforCurMonth_B);
                        if (ExEnhance_C > 0)
                            ExLbAmtforCurMonth_C = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_C);
                        else
                            ExLbAmtforCurMonth_C = String.Format("{0:0.00}", ExAmtforCurMonth_C);
                        if (ExEnhance_D > 0)
                            ExLbAmtforCurMonth_D = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_D);
                        else
                            ExLbAmtforCurMonth_D = String.Format("{0:0.00}", ExAmtforCurMonth_D);
                        var ExLbAmtforPrevMonth_A = ""; var ExLbAmtforPrevMonth_B = ""; var ExLbAmtforPrevMonth_C = ""; var ExLbAmtforPrevMonth_D = "";
                        if (ExprevEnhance_A > 0)
                            ExLbAmtforPrevMonth_A = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_A);
                        else
                            ExLbAmtforPrevMonth_A = String.Format("{0:0.00}", ExAmtforPrevMonth_A);
                        if (ExprevEnhance_B > 0)
                            ExLbAmtforPrevMonth_B = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_B);
                        else
                            ExLbAmtforPrevMonth_B = String.Format("{0:0.00}", ExAmtforPrevMonth_B);
                        if (ExprevEnhance_C > 0)
                            ExLbAmtforPrevMonth_C = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_C);
                        else
                            ExLbAmtforPrevMonth_C = String.Format("{0:0.00}", ExAmtforPrevMonth_C);
                        if (ExprevEnhance_D > 0)
                            ExLbAmtforPrevMonth_D = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_D);
                        else
                            ExLbAmtforPrevMonth_D = String.Format("{0:0.00}", ExAmtforPrevMonth_D);

                        List.Add(new OfficeMonthlyListReportModel()
                        {
                            DepartmentName = DeptName,
                            Type = Type,
                            NoOfProjCurMonth_A = CurrProj_A ?? 0,
                            NoOfProjPrevMonth_A = PrevProj_A ?? 0,
                            TotalProj_A = TotalProj_A ?? 0,
                            AmtforCurMonth_A = LbAmtforCurMonth_A,
                            AmtforPrevMonth_A = LbAmtforPrevMonth_A,
                            TotalAmt_A = TotalAmt_A,
                            ExAmtforCurMonth_A = ExLbAmtforCurMonth_A,
                            ExAmtforPrevMonth_A = ExLbAmtforPrevMonth_A,
                            ExTotalAmt_A = ExTotalAmt_A,
                            NoOfProjCurMonth_B = CurrProj_B ?? 0,
                            NoOfProjPrevMonth_B = PrevProj_B ?? 0,
                            TotalProj_B = TotalProj_B ?? 0,
                            AmtforCurMonth_B = LbAmtforCurMonth_B,
                            AmtforPrevMonth_B = LbAmtforPrevMonth_B,
                            TotalAmt_B = TotalAmt_B,
                            ExAmtforCurMonth_B = ExLbAmtforCurMonth_B,
                            ExAmtforPrevMonth_B = ExLbAmtforPrevMonth_B,
                            ExTotalAmt_B = ExTotalAmt_B,
                            NoOfProjCurMonth_C = CurrProj_C ?? 0,
                            NoOfProjPrevMonth_C = PrevProj_C ?? 0,
                            TotalProj_C = TotalProj_C ?? 0,
                            AmtforCurMonth_C = LbAmtforCurMonth_C,
                            AmtforPrevMonth_C = LbAmtforPrevMonth_C,
                            TotalAmt_C = TotalAmt_C,
                            ExAmtforCurMonth_C = ExLbAmtforCurMonth_C,
                            ExAmtforPrevMonth_C = ExLbAmtforPrevMonth_C,
                            ExTotalAmt_C = ExTotalAmt_C,
                            NoOfProjCurMonth_D = CurrProj_D ?? 0,
                            NoOfProjPrevMonth_D = PrevProj_D ?? 0,
                            TotalProj_D = TotalProj_D ?? 0,
                            AmtforCurMonth_D = LbAmtforCurMonth_D,
                            AmtforPrevMonth_D = LbAmtforPrevMonth_D,
                            TotalAmt_D = TotalAmt_D,
                            ExAmtforCurMonth_D = ExLbAmtforCurMonth_D,
                            ExAmtforPrevMonth_D = ExLbAmtforPrevMonth_D,
                            ExTotalAmt_D = ExTotalAmt_D,
                            AmtforRecCurMonth = receiptAmt,
                            AmtforRecPrevMonth = QryRecAMt,
                            TotalRecAmt = TotalRecAmt,
                            ExAmtforRecCurMonth = ExreceiptAmt,
                            ExAmtforRecPrevMonth = ExQryRecAMt,
                            ExTotalRecAmt = ExTotalRecAmt
                        });
                    }
                    ////////Center ////////
                    List<OfficeMonthlyCenterListReportModel> CenterList = new List<OfficeMonthlyCenterListReportModel>();
                    int[] Center = { 59, 62, 65, 72, 73, 79, 83, 85, 86, 87, 94, 109, 132, 135, 138, 145, 146, 152, 156, 158, 159, 160, 167, 180 };

                    if (Center.Length > 0)
                    {


                        decimal TotalAmt_A = 0; decimal AmtforPrevMonth_A = 0; decimal AmtforCurMonth_A = 0; decimal receiptAmt = 0; decimal QryRecAMt = 0;
                        decimal ExTotalAmt_A = 0; decimal ExAmtforPrevMonth_A = 0; decimal ExAmtforCurMonth_A = 0; decimal ExreceiptAmt = 0; decimal ExQryRecAMt = 0;
                        decimal TotalAmt_B = 0; decimal AmtforPrevMonth_B = 0; decimal AmtforCurMonth_B = 0;
                        decimal ExTotalAmt_B = 0; decimal ExAmtforPrevMonth_B = 0; decimal ExAmtforCurMonth_B = 0;
                        decimal TotalAmt_C = 0; decimal AmtforPrevMonth_C = 0; decimal AmtforCurMonth_C = 0;
                        decimal ExTotalAmt_C = 0; decimal ExAmtforPrevMonth_C = 0; decimal ExAmtforCurMonth_C = 0;
                        decimal TotalAmt_D = 0; decimal AmtforPrevMonth_D = 0; decimal AmtforCurMonth_D = 0;
                        decimal ExTotalAmt_D = 0; decimal ExAmtforPrevMonth_D = 0; decimal ExAmtforCurMonth_D = 0;

                        var ProjDet_A = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.ProjectClassification != 16 && m.CrtdTS <= todate && m.FinancialYear == FinId && Center.Contains(m.FacultyDetailId ?? 0) && m.ProjectType == 2 && A1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        var PrevProjDet_A = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.ProjectClassification != 16 && m.CrtdTS <= endDate && m.FinancialYear == FinId && Center.Contains(m.FacultyDetailId ?? 0) && m.ProjectType == 2 && A1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_A = ProjDet_A.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_A = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                               //&& !EnhanceArr_A.Contains(aa.ProjectId ?? 0)
                                               && b.ProjectClassification != 16
                                               && aa.IsEnhancementonly == true && Center.Contains(b.FacultyDetailId ?? 0) && b.ProjectType == 2 && A1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_A = ListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_A = ListEnhance_A.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_A = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                                && b.ProjectClassification != 16 && aa.IsEnhancementonly == true && Center.Contains(b.FacultyDetailId ?? 0) && b.ProjectType == 2 && A1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_A = prevListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_A = prevListEnhance_A.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        prevEnhance_A = (prevEnhance_A / 100000);
                        Enhance_A = (Enhance_A / 100000);
                        AmtforCurMonth_A = (ProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_A = AmtforCurMonth_A + Enhance_A;
                        AmtforPrevMonth_A = (PrevProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_A = AmtforPrevMonth_A + prevEnhance_A;
                        TotalAmt_A = AmtforCurMonth_A + AmtforPrevMonth_A;

                        //Ex

                        ExprevEnhance_A = (ExprevEnhance_A / 100000);
                        ExEnhance_A = (ExEnhance_A / 100000);
                        ExAmtforCurMonth_A = (ProjDet_A.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_A = ExAmtforCurMonth_A + ExEnhance_A;
                        ExAmtforPrevMonth_A = (PrevProjDet_A.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_A = ExAmtforPrevMonth_A + ExprevEnhance_A;
                        ExTotalAmt_A = ExAmtforCurMonth_A + ExAmtforPrevMonth_A;

                        int? PrevProj_A = PrevProjDet_A.Count();
                        int? CurrProj_A = ProjDet_A.Count();
                        int? TotalProj_A = PrevProj_A + CurrProj_A;
                        ////// B //////
                        var ProjDet_B = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.ProjectClassification != 16 && m.CrtdTS <= todate && m.FinancialYear == FinId && Center.Contains(m.FacultyDetailId ?? 0) && m.ProjectType == 2 && B1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        var PrevProjDet_B = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.ProjectClassification != 16 && m.CrtdTS <= endDate && m.FinancialYear == FinId && Center.Contains(m.FacultyDetailId ?? 0) && m.ProjectType == 2 && B1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_B = ProjDet_B.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_B = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                               //&& !EnhanceArr_B.Contains(aa.ProjectId ?? 0)
                                               && b.ProjectClassification != 16 && aa.IsEnhancementonly == true
                                                && Center.Contains(b.FacultyDetailId ?? 0) && b.ProjectType == 2 && B1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_B = ListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_B = ListEnhance_B.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_B = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                                && b.ProjectClassification != 16 && aa.IsEnhancementonly == true
                                             && Center.Contains(b.FacultyDetailId ?? 0) && b.ProjectType == 2 && B1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_B = prevListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_B = prevListEnhance_B.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        prevEnhance_B = (prevEnhance_B / 100000);
                        Enhance_B = (Enhance_B / 100000);
                        AmtforCurMonth_B = (ProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_B = AmtforCurMonth_B + Enhance_B;
                        AmtforPrevMonth_B = (PrevProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_B = AmtforPrevMonth_B + prevEnhance_B;
                        TotalAmt_B = AmtforCurMonth_B + AmtforPrevMonth_B;

                        //Ex
                        ExprevEnhance_B = (ExprevEnhance_B / 100000);
                        ExEnhance_B = (ExEnhance_B / 100000);
                        ExAmtforCurMonth_B = (ProjDet_B.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_B = ExAmtforCurMonth_B + ExEnhance_B;
                        ExAmtforPrevMonth_B = (PrevProjDet_B.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_B = ExAmtforPrevMonth_B + ExprevEnhance_B;
                        ExTotalAmt_B = ExAmtforCurMonth_B + ExAmtforPrevMonth_B;


                        int? PrevProj_B = PrevProjDet_B.Count();
                        int? CurrProj_B = ProjDet_B.Count();
                        int? TotalProj_B = PrevProj_B + CurrProj_B;
                        ////// C //////
                        var ProjDet_C = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.ProjectClassification != 16 && m.FinancialYear == FinId && m.CrtdTS <= todate && Center.Contains(m.FacultyDetailId ?? 0) && m.ProjectType == 2 && C1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        var PrevProjDet_C = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.ProjectClassification != 16 && m.CrtdTS <= endDate && m.FinancialYear == FinId && Center.Contains(m.FacultyDetailId ?? 0) && m.ProjectType == 2 && C1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_C = ProjDet_C.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_C = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                             //&& !EnhanceArr_C.Contains(aa.ProjectId ?? 0)
                                             && b.ProjectClassification != 16 && aa.IsEnhancementonly == true
                                             && Center.Contains(b.FacultyDetailId ?? 0) && b.ProjectType == 2 && C1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_C = ListEnhance_C.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_C = ListEnhance_C.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_C = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                                && b.ProjectClassification != 16 && aa.IsEnhancementonly == true
                                               && Center.Contains(b.FacultyDetailId ?? 0) && b.ProjectType == 2 && C1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_C = prevListEnhance_C.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_C = prevListEnhance_C.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        prevEnhance_C = (prevEnhance_C / 100000);
                        Enhance_C = (Enhance_C / 100000);
                        AmtforCurMonth_C = (ProjDet_C.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_C = AmtforCurMonth_C + Enhance_C;
                        AmtforPrevMonth_C = (PrevProjDet_C.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_C = AmtforPrevMonth_C + prevEnhance_C;
                        TotalAmt_C = AmtforCurMonth_C + AmtforPrevMonth_C;

                        //Ex
                        ExprevEnhance_C = (ExprevEnhance_C / 100000);
                        ExEnhance_C = (ExEnhance_C / 100000);
                        ExAmtforCurMonth_C = (ProjDet_C.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_C = ExAmtforCurMonth_C + ExEnhance_C;
                        ExAmtforPrevMonth_C = (PrevProjDet_C.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_C = ExAmtforPrevMonth_C + ExprevEnhance_C;
                        ExTotalAmt_C = ExAmtforCurMonth_C + ExAmtforPrevMonth_C;

                        int? PrevProj_C = PrevProjDet_C.Count();
                        int? CurrProj_C = ProjDet_C.Count();
                        int? TotalProj_C = PrevProj_C + CurrProj_C;
                        ////// D //////
                        var ProjDet_D = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.ProjectClassification != 16 && m.CrtdTS <= todate && m.FinancialYear == FinId && Center.Contains(m.FacultyDetailId ?? 0) && m.ProjectType == 2 && D1.Contains(m.ConsultancyFundingCategory ?? 0)
                        ).ToList();
                        var PrevProjDet_D = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= endDate && Center.Contains(m.FacultyDetailId ?? 0)
                        && m.FinancialYear == FinId && m.ProjectType == 2 && D1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_D = ProjDet_D.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_D = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                               //&& !EnhanceArr_D.Contains(aa.ProjectId ?? 0)
                                               && b.ProjectClassification != 16 && aa.IsEnhancementonly == true
                                                 && Center.Contains(b.FacultyDetailId ?? 0)
                                               && b.ProjectType == 2 && D1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_D = ListEnhance_D.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_D = ListEnhance_D.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_D = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                                && b.ProjectClassification != 16 && aa.IsEnhancementonly == true
                                                && Center.Contains(b.FacultyDetailId ?? 0) && b.ProjectType == 2 && D1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_D = prevListEnhance_D.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_D = prevListEnhance_D.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        prevEnhance_D = (prevEnhance_D / 100000);
                        Enhance_D = (Enhance_D / 100000);
                        AmtforCurMonth_D = (ProjDet_D.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_D = AmtforCurMonth_D + Enhance_D;
                        AmtforCurMonth_D = 0;
                        AmtforPrevMonth_D = (PrevProjDet_D.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_D = AmtforPrevMonth_D + prevEnhance_D;
                        AmtforPrevMonth_D = 0;
                        TotalAmt_D = AmtforCurMonth_D + AmtforPrevMonth_D;


                        //Ex
                        ExprevEnhance_D = (ExprevEnhance_D / 100000);
                        ExEnhance_D = (ExEnhance_D / 100000);
                        ExAmtforCurMonth_D = (ProjDet_D.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_D = ExAmtforCurMonth_D + ExEnhance_D;
                        ExAmtforCurMonth_D = 0;
                        ExAmtforPrevMonth_D = (PrevProjDet_D.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_D = ExAmtforPrevMonth_D + ExprevEnhance_D;
                        ExAmtforPrevMonth_D = 0;
                        ExTotalAmt_D = ExAmtforCurMonth_D + ExAmtforPrevMonth_D;

                        int? PrevProj_D = PrevProjDet_D.Count();
                        PrevProj_D = 0;
                        int? CurrProj_D = ProjDet_D.Count();
                        CurrProj_D = 0;
                        int? TotalProj_D = PrevProj_D + CurrProj_D;
                        //int[] ProjDet = context.tblProject.Where(m => m.CrtdTS >= fromdate && m.FinancialYear == FinId && m.CrtdTS <= todate
                        //&& m.ProjectClassification != 16 && Center.Contains(m.FacultyDetailId ?? 0) && m.Status == "Active" && m.ProjectType == 2).Select(m => m.ProjectId).ToArray();
                        //int[] PrevProjDet = context.tblProject.Where(m => m.CrtdTS >= startDate && m.CrtdTS <= endDate && m.FinancialYear == FinId
                        // && m.ProjectClassification != 16 && Center.Contains(m.FacultyDetailId ?? 0) && m.Status == "Active" && m.ProjectType == 2).Select(m => m.ProjectId).ToArray();
                        int[] ProjClass = { 2, 3, 14 };
                        var QryRect = (from aa in context.tblReceipt
                                       join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                       join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                       where
                                         Center.Contains(b.FacultyDetailId ?? 0) && aa.CrtdTS >= startDate && aa.CrtdTS <= endDate && aa.Posted_f == true
                                      && !Categ.Contains(aa.CategoryId ?? 0) && b.ProjectType == 2 && aa.Status == "Completed" && !aa.ReceiptNumber.Contains("RBU")
                                      && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                      && b.ProjectClassification != 16
                                       select new { aa.ReceiptAmount, aa.ReceiptOverheadValue, aa.IGST, aa.SGST, aa.CGST, b.ProjectNumber }).ToList();
                        QryRecAMt = QryRect.Sum(m => m.ReceiptAmount ?? 0);

                        QryRecAMt = (QryRecAMt) / 100000;
                        ExQryRecAMt = (QryRect.Sum(m =>
                        {
                            return m.ReceiptAmount - ((m.CGST ?? 0) + (m.SGST ?? 0) + (m.IGST ?? 0));
                        }) / 100000) ?? 0;
                        var QryReceipt = (from aa in context.tblReceipt
                                          join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                          join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where
                                           Center.Contains(b.FacultyDetailId ?? 0) && aa.Posted_f == true && aa.CrtdTS >= fromdate && aa.CrtdTS <= todate
                                        && !Categ.Contains(aa.CategoryId ?? 0) && b.ProjectType == 2 && aa.Status == "Completed" && !aa.ReceiptNumber.Contains("RBU")
                                         && !b.ProjectNumber.Contains("NFSC") && b.ProjectClassification != 16
                                            && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                          select new { aa.ReceiptAmount, aa.ReceiptOverheadValue, aa.IGST, aa.SGST, aa.CGST, b.ProjectNumber }).ToList();
                        receiptAmt = QryReceipt.Sum(m => m.ReceiptAmount ?? 0);

                        receiptAmt = (receiptAmt) / 100000;
                        ExreceiptAmt = (QryReceipt.Sum(m =>
                        {
                            return m.ReceiptAmount - ((m.CGST ?? 0) + (m.SGST ?? 0) + (m.IGST ?? 0));
                        }) / 100000) ?? 0;


                        TtlPrevRecAmt += QryRecAMt;
                        TtlCurrRecAmt += receiptAmt;
                        decimal TotalRecAmt = receiptAmt + QryRecAMt;
                        TtlPrevProj_A += PrevProj_A;
                        TtlCurrProj_A += CurrProj_A;
                        TtlPrevProj_B += PrevProj_B;
                        TtlCurrProj_B += CurrProj_B;
                        TtlPrevProj_C += PrevProj_C;
                        TtlCurrProj_C += CurrProj_C;
                        TtlPrevProj_D += PrevProj_D;
                        TtlCurrProj_D += CurrProj_D;
                        TtlPrevAmt_A += AmtforPrevMonth_A;
                        TtlCurrAmt_A += AmtforCurMonth_A;
                        TtlPrevAmt_B += AmtforPrevMonth_B;
                        TtlCurrAmt_B += AmtforCurMonth_B;
                        TtlPrevAmt_C += AmtforPrevMonth_C;
                        TtlCurrAmt_C += AmtforCurMonth_C;
                        TtlCurrAmt_D += AmtforCurMonth_D;
                        TtlCurrAmt_D += AmtforCurMonth_D;

                        //Ex


                        ExTtlPrevRecAmt += ExQryRecAMt;
                        ExTtlCurrRecAmt += ExreceiptAmt;
                        decimal ExTotalRecAmt = ExreceiptAmt + ExQryRecAMt;

                        ExTtlPrevAmt_A += ExAmtforPrevMonth_A;
                        ExTtlCurrAmt_A += ExAmtforCurMonth_A;
                        ExTtlPrevAmt_B += ExAmtforPrevMonth_B;
                        ExTtlCurrAmt_B += ExAmtforCurMonth_B;
                        ExTtlPrevAmt_C += ExAmtforPrevMonth_C;
                        ExTtlCurrAmt_C += ExAmtforCurMonth_C;
                        ExTtlPrevAmt_D += ExAmtforPrevMonth_D;
                        ExTtlCurrAmt_D += ExAmtforCurMonth_D;


                        var LbAmtforCurMonth_A = ""; var LbAmtforCurMonth_B = ""; var LbAmtforCurMonth_C = ""; var LbAmtforCurMonth_D = "";
                        var ExLbAmtforCurMonth_A = ""; var ExLbAmtforCurMonth_B = ""; var ExLbAmtforCurMonth_C = ""; var ExLbAmtforCurMonth_D = "";

                        if (Enhance_A > 0)
                            LbAmtforCurMonth_A = "*" + String.Format("{0:0.00}", AmtforCurMonth_A);
                        else
                            LbAmtforCurMonth_A = String.Format("{0:0.00}", AmtforCurMonth_A);
                        if (Enhance_B > 0)
                            LbAmtforCurMonth_B = "*" + String.Format("{0:0.00}", AmtforCurMonth_B);
                        else
                            LbAmtforCurMonth_B = String.Format("{0:0.00}", AmtforCurMonth_B);
                        if (Enhance_C > 0)
                            LbAmtforCurMonth_C = "*" + String.Format("{0:0.00}", AmtforCurMonth_C);
                        else
                            LbAmtforCurMonth_C = String.Format("{0:0.00}", AmtforCurMonth_C);
                        if (Enhance_D > 0)
                            LbAmtforCurMonth_D = "*" + String.Format("{0:0.00}", AmtforCurMonth_D);
                        else
                            LbAmtforCurMonth_D = String.Format("{0:0.00}", AmtforCurMonth_D);
                        var LbAmtforPrevMonth_A = ""; var LbAmtforPrevMonth_B = ""; var LbAmtforPrevMonth_C = ""; var LbAmtforPrevMonth_D = "";
                        if (prevEnhance_A > 0)
                            LbAmtforPrevMonth_A = "*" + String.Format("{0:0.00}", AmtforPrevMonth_A);
                        else
                            LbAmtforPrevMonth_A = String.Format("{0:0.00}", AmtforPrevMonth_A);
                        if (prevEnhance_B > 0)
                            LbAmtforPrevMonth_B = "*" + String.Format("{0:0.00}", AmtforPrevMonth_B);
                        else
                            LbAmtforPrevMonth_B = String.Format("{0:0.00}", AmtforPrevMonth_B);
                        if (prevEnhance_C > 0)
                            LbAmtforPrevMonth_C = "*" + String.Format("{0:0.00}", AmtforPrevMonth_C);
                        else
                            LbAmtforPrevMonth_C = String.Format("{0:0.00}", AmtforPrevMonth_C);
                        if (prevEnhance_D > 0)
                            LbAmtforPrevMonth_D = "*" + String.Format("{0:0.00}", AmtforPrevMonth_D);
                        else
                            LbAmtforPrevMonth_D = String.Format("{0:0.00}", AmtforPrevMonth_D);


                        //Ex
                        if (ExEnhance_A > 0)
                            ExLbAmtforCurMonth_A = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_A);
                        else
                            ExLbAmtforCurMonth_A = String.Format("{0:0.00}", ExAmtforCurMonth_A);
                        if (ExEnhance_B > 0)
                            ExLbAmtforCurMonth_B = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_B);
                        else
                            ExLbAmtforCurMonth_B = String.Format("{0:0.00}", ExAmtforCurMonth_B);
                        if (ExEnhance_C > 0)
                            ExLbAmtforCurMonth_C = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_C);
                        else
                            ExLbAmtforCurMonth_C = String.Format("{0:0.00}", ExAmtforCurMonth_C);
                        if (ExEnhance_D > 0)
                            ExLbAmtforCurMonth_D = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_D);
                        else
                            ExLbAmtforCurMonth_D = String.Format("{0:0.00}", ExAmtforCurMonth_D);
                        var ExLbAmtforPrevMonth_A = ""; var ExLbAmtforPrevMonth_B = ""; var ExLbAmtforPrevMonth_C = ""; var ExLbAmtforPrevMonth_D = "";
                        if (ExprevEnhance_A > 0)
                            ExLbAmtforPrevMonth_A = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_A);
                        else
                            ExLbAmtforPrevMonth_A = String.Format("{0:0.00}", ExAmtforPrevMonth_A);
                        if (ExprevEnhance_B > 0)
                            ExLbAmtforPrevMonth_B = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_B);
                        else
                            ExLbAmtforPrevMonth_B = String.Format("{0:0.00}", ExAmtforPrevMonth_B);
                        if (ExprevEnhance_C > 0)
                            ExLbAmtforPrevMonth_C = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_C);
                        else
                            ExLbAmtforPrevMonth_C = String.Format("{0:0.00}", ExAmtforPrevMonth_C);
                        if (ExprevEnhance_D > 0)
                            ExLbAmtforPrevMonth_D = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_D);
                        else
                            ExLbAmtforPrevMonth_D = String.Format("{0:0.00}", ExAmtforPrevMonth_D);


                        List.Add(new OfficeMonthlyListReportModel()
                        {
                            DepartmentName = "AAR,ADM,NPT,TAP,CAT,CSH,ICW,IGC,SOL,STL,SHS,RBC",
                            Type = Type,
                            NoOfProjCurMonth_A = CurrProj_A ?? 0,
                            NoOfProjPrevMonth_A = PrevProj_A ?? 0,
                            TotalProj_A = TotalProj_A ?? 0,
                            AmtforCurMonth_A = LbAmtforCurMonth_A,
                            AmtforPrevMonth_A = LbAmtforPrevMonth_A,
                            TotalAmt_A = TotalAmt_A,
                            ExAmtforCurMonth_A = ExLbAmtforCurMonth_A,
                            ExAmtforPrevMonth_A = ExLbAmtforPrevMonth_A,
                            ExTotalAmt_A = ExTotalAmt_A,
                            NoOfProjCurMonth_B = CurrProj_B ?? 0,
                            NoOfProjPrevMonth_B = PrevProj_B ?? 0,
                            TotalProj_B = TotalProj_B ?? 0,
                            AmtforCurMonth_B = LbAmtforCurMonth_B,
                            AmtforPrevMonth_B = LbAmtforPrevMonth_B,
                            TotalAmt_B = TotalAmt_B,
                            ExAmtforCurMonth_B = ExLbAmtforCurMonth_B,
                            ExAmtforPrevMonth_B = ExLbAmtforPrevMonth_B,
                            ExTotalAmt_B = ExTotalAmt_B,
                            NoOfProjCurMonth_C = CurrProj_C ?? 0,
                            NoOfProjPrevMonth_C = PrevProj_C ?? 0,
                            TotalProj_C = TotalProj_C ?? 0,
                            AmtforCurMonth_C = LbAmtforCurMonth_C,
                            AmtforPrevMonth_C = LbAmtforPrevMonth_C,
                            TotalAmt_C = TotalAmt_C,
                            ExAmtforCurMonth_C = ExLbAmtforCurMonth_C,
                            ExAmtforPrevMonth_C = ExLbAmtforPrevMonth_C,
                            ExTotalAmt_C = ExTotalAmt_C,
                            NoOfProjCurMonth_D = CurrProj_D ?? 0,
                            NoOfProjPrevMonth_D = PrevProj_D ?? 0,
                            TotalProj_D = TotalProj_D ?? 0,
                            AmtforCurMonth_D = LbAmtforCurMonth_D,
                            AmtforPrevMonth_D = LbAmtforPrevMonth_D,
                            TotalAmt_D = TotalAmt_D,
                            ExAmtforCurMonth_D = ExLbAmtforCurMonth_D,
                            ExAmtforPrevMonth_D = ExLbAmtforPrevMonth_D,
                            ExTotalAmt_D = ExTotalAmt_D,
                            AmtforRecCurMonth = receiptAmt,
                            AmtforRecPrevMonth = QryRecAMt,
                            TotalRecAmt = TotalRecAmt,
                            ExAmtforRecCurMonth = ExreceiptAmt,
                            ExAmtforRecPrevMonth = ExQryRecAMt,
                            ExTotalRecAmt = ExTotalRecAmt
                        });
                    }

                    ////////Non Center ////////
                    var NonCenter = context.tblCommonHeads.Where(m => m.CategoryId == 2 && m.HeadId != 71 && !Center.Contains(m.HeadId) && (m.GroupId == 2 || m.GroupId == 5)).ToList();
                    for (int a = 0; a < NonCenter.Count(); a++)
                    {

                        var DeptName = NonCenter[a].Head;
                        int DeptCode = NonCenter[a].HeadId;
                        decimal TotalAmt_A = 0; decimal AmtforPrevMonth_A = 0; decimal AmtforCurMonth_A = 0; decimal receiptAmt = 0; decimal QryRecAMt = 0;
                        decimal ExTotalAmt_A = 0; decimal ExAmtforPrevMonth_A = 0; decimal ExAmtforCurMonth_A = 0; decimal ExreceiptAmt = 0; decimal ExQryRecAMt = 0;
                        decimal TotalAmt_B = 0; decimal AmtforPrevMonth_B = 0; decimal AmtforCurMonth_B = 0;
                        decimal ExTotalAmt_B = 0; decimal ExAmtforPrevMonth_B = 0; decimal ExAmtforCurMonth_B = 0;
                        decimal TotalAmt_C = 0; decimal AmtforPrevMonth_C = 0; decimal AmtforCurMonth_C = 0;
                        decimal ExTotalAmt_C = 0; decimal ExAmtforPrevMonth_C = 0; decimal ExAmtforCurMonth_C = 0;
                        decimal TotalAmt_D = 0; decimal AmtforPrevMonth_D = 0; decimal AmtforCurMonth_D = 0;
                        decimal ExTotalAmt_D = 0; decimal ExAmtforPrevMonth_D = 0; decimal ExAmtforCurMonth_D = 0;
                        var ProjDet_A = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= todate && m.FinancialYear == FinId && m.FacultyDetailId == DeptCode && m.Status == "Active" && m.ProjectType == 2 && A1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        var PrevProjDet_A = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.FacultyDetailId == DeptCode && m.Status == "Active" && m.ProjectType == 2 && A1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_A = ProjDet_A.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_A = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                             && aa.IsEnhancementonly == true && b.FacultyDetailId == DeptCode && b.ProjectType == 2 && A1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_A = ListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_A = ListEnhance_A.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_A = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                                && aa.IsEnhancementonly == true && b.FacultyDetailId == DeptCode && b.ProjectType == 2 && A1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_A = prevListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_A = prevListEnhance_A.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        prevEnhance_A = (prevEnhance_A / 100000);
                        Enhance_A = (Enhance_A / 100000);
                        AmtforCurMonth_A = (ProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_A = AmtforCurMonth_A + Enhance_A;
                        AmtforPrevMonth_A = (PrevProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_A = AmtforPrevMonth_A + prevEnhance_A;
                        TotalAmt_A = AmtforCurMonth_A + AmtforPrevMonth_A;

                        //Ex

                        ExprevEnhance_A = (ExprevEnhance_A / 100000);
                        ExEnhance_A = (ExEnhance_A / 100000);
                        ExAmtforCurMonth_A = (ProjDet_A.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_A = ExAmtforCurMonth_A + ExEnhance_A;
                        ExAmtforPrevMonth_A = (PrevProjDet_A.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_A = ExAmtforPrevMonth_A + ExprevEnhance_A;
                        ExTotalAmt_A = ExAmtforCurMonth_A + ExAmtforPrevMonth_A;

                        int? PrevProj_A = PrevProjDet_A.Count();
                        int? CurrProj_A = ProjDet_A.Count();
                        int? TotalProj_A = PrevProj_A + CurrProj_A;
                        ////// B //////
                        var ProjDet_B = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= todate && m.FinancialYear == FinId && m.FacultyDetailId == DeptCode && m.ProjectType == 2 && B1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        var PrevProjDet_B = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.FacultyDetailId == DeptCode && m.ProjectType == 2 && B1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_B = ProjDet_B.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_B = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                             && aa.IsEnhancementonly == true && b.FacultyDetailId == DeptCode && b.ProjectType == 2 && B1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_B = ListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_B = ListEnhance_B.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_B = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                              && aa.IsEnhancementonly == true && b.FacultyDetailId == DeptCode && b.ProjectType == 2 && B1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_B = prevListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_B = prevListEnhance_B.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        prevEnhance_B = (prevEnhance_B / 100000);
                        Enhance_B = (Enhance_B / 100000);
                        AmtforCurMonth_B = (ProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_B = AmtforCurMonth_B + Enhance_B;
                        AmtforPrevMonth_B = (PrevProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_B = AmtforPrevMonth_B + prevEnhance_B;
                        TotalAmt_B = AmtforCurMonth_B + AmtforPrevMonth_B;

                        //Ex
                        ExprevEnhance_B = (ExprevEnhance_B / 100000);
                        ExEnhance_B = (ExEnhance_B / 100000);
                        ExAmtforCurMonth_B = (ProjDet_B.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_B = ExAmtforCurMonth_B + ExEnhance_B;
                        ExAmtforPrevMonth_B = (PrevProjDet_B.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_B = ExAmtforPrevMonth_B + ExprevEnhance_B;
                        ExTotalAmt_B = ExAmtforCurMonth_B + ExAmtforPrevMonth_B;


                        int? PrevProj_B = PrevProjDet_B.Count();
                        int? CurrProj_B = ProjDet_B.Count();
                        int? TotalProj_B = PrevProj_B + CurrProj_B;
                        ////// C //////
                        var ProjDet_C = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.FinancialYear == FinId && m.CrtdTS <= todate && m.FacultyDetailId == DeptCode && m.ProjectType == 2 && C1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        var PrevProjDet_C = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.FacultyDetailId == DeptCode && m.ProjectType == 2 && C1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_C = ProjDet_C.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_C = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                         && aa.IsEnhancementonly == true && b.FacultyDetailId == DeptCode && b.ProjectType == 2 && C1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_C = ListEnhance_C.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_C = ListEnhance_C.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_C = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                           && aa.IsEnhancementonly == true && b.FacultyDetailId == DeptCode && b.ProjectType == 2 && C1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_C = prevListEnhance_C.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_C = prevListEnhance_C.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        prevEnhance_C = (prevEnhance_C / 100000);
                        Enhance_C = (Enhance_C / 100000);
                        AmtforCurMonth_C = (ProjDet_C.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_C = AmtforCurMonth_C + Enhance_C;
                        AmtforPrevMonth_C = (PrevProjDet_C.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_C = AmtforPrevMonth_C + prevEnhance_C;
                        TotalAmt_C = AmtforCurMonth_C + AmtforPrevMonth_C;

                        //Ex
                        ExprevEnhance_C = (ExprevEnhance_C / 100000);
                        ExEnhance_C = (ExEnhance_C / 100000);
                        ExAmtforCurMonth_C = (ProjDet_C.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_C = ExAmtforCurMonth_C + ExEnhance_C;
                        ExAmtforPrevMonth_C = (PrevProjDet_C.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_C = ExAmtforPrevMonth_C + ExprevEnhance_C;
                        ExTotalAmt_C = ExAmtforCurMonth_C + ExAmtforPrevMonth_C;

                        int? PrevProj_C = PrevProjDet_C.Count();
                        int? CurrProj_C = ProjDet_C.Count();
                        int? TotalProj_C = PrevProj_C + CurrProj_C;
                        ////// D //////
                        var ProjDet_D = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= todate && m.FinancialYear == FinId && m.FacultyDetailId == DeptCode && m.ProjectType == 2 && D1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        var PrevProjDet_D = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= endDate && m.FacultyDetailId == DeptCode && m.FinancialYear == FinId && m.ProjectType == 2 && D1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_D = ProjDet_D.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_D = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                               && aa.IsEnhancementonly == true && b.FacultyDetailId == DeptCode && b.ProjectType == 2 && D1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_D = ListEnhance_D.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_D = ListEnhance_D.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_D = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                                && aa.IsEnhancementonly == true && b.FacultyDetailId == DeptCode && b.ProjectType == 2 && D1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_D = prevListEnhance_D.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_D = prevListEnhance_D.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        prevEnhance_D = (prevEnhance_D / 100000);
                        Enhance_D = (Enhance_D / 100000);
                        AmtforCurMonth_D = (ProjDet_D.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_D = AmtforCurMonth_D + Enhance_D;
                        AmtforCurMonth_D = 0;
                        AmtforPrevMonth_D = (PrevProjDet_D.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_D = AmtforPrevMonth_D + prevEnhance_D;
                        AmtforPrevMonth_D = 0;
                        TotalAmt_D = AmtforCurMonth_D + AmtforPrevMonth_D;

                        //Ex
                        ExprevEnhance_D = (ExprevEnhance_D / 100000);
                        ExEnhance_D = (ExEnhance_D / 100000);
                        ExAmtforCurMonth_D = (ProjDet_D.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_D = ExAmtforCurMonth_D + ExEnhance_D;
                        ExAmtforCurMonth_D = 0;
                        ExAmtforPrevMonth_D = (PrevProjDet_D.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_D = ExAmtforPrevMonth_D + ExprevEnhance_D;
                        ExAmtforPrevMonth_D = 0;
                        ExTotalAmt_D = ExAmtforCurMonth_D + ExAmtforPrevMonth_D;

                        int? PrevProj_D = PrevProjDet_D.Count();
                        PrevProj_D = 0;
                        int? CurrProj_D = ProjDet_D.Count();
                        CurrProj_D = 0;
                        int? TotalProj_D = PrevProj_D + CurrProj_D;
                        //   int[] ProjDet = context.tblProject.Where(m => m.CrtdTS >= fromdate && m.FinancialYear == FinId && m.CrtdTS <= todate && m.FacultyDetailId == DeptCode && m.Status == "Active" && m.ProjectType == 2).Select(m => m.ProjectId).ToArray();
                        // int[] PrevProjDet = context.tblProject.Where(m => m.CrtdTS >= startDate && m.CrtdTS <= endDate && m.FinancialYear == FinId && m.FacultyDetailId == DeptCode && m.Status == "Active" && m.ProjectType == 2).Select(m => m.ProjectId).ToArray();
                        int[] ProjClass = { 2, 3, 14 };
                        var QryRect = (from aa in context.tblReceipt
                                       join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                       join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                       where
                                        //!(b.ConsultancyFundingCategory == 9) && !(b.ConsultancyFundingCategory == 12)&&
                                        b.FacultyDetailId == DeptCode && aa.CrtdTS >= startDate && aa.CrtdTS <= endDate && aa.Posted_f == true &&
                                       b.ProjectClassification != 16 && !Categ.Contains(aa.CategoryId ?? 0) && b.ProjectType == 2
                                       && aa.Status == "Completed" && !aa.ReceiptNumber.Contains("RBU")
                                       && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                       select new { aa.ReceiptAmount, aa.ReceiptOverheadValue, aa.IGST, aa.SGST, aa.CGST, b.ProjectNumber }).ToList();
                        QryRecAMt = QryRect.Sum(m => m.ReceiptAmount ?? 0);

                        QryRecAMt = (QryRecAMt) / 100000;
                        ExQryRecAMt = (QryRect.Sum(m =>
                        {
                            return m.ReceiptAmount - ((m.CGST ?? 0) + (m.SGST ?? 0) + (m.IGST ?? 0));
                        }) / 100000) ?? 0;
                        var QryReceipt = (from aa in context.tblReceipt
                                          join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                          join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where b.FacultyDetailId == DeptCode && aa.CrtdTS >= fromdate && aa.Posted_f == true && aa.CrtdTS <= todate &&
                                          b.ProjectClassification != 16 && !Categ.Contains(aa.CategoryId ?? 0) && b.ProjectType == 2
                                          && aa.Status == "Completed" && !aa.ReceiptNumber.Contains("RBU")
                                          && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                          select new { aa.ReceiptAmount, aa.ReceiptOverheadValue, aa.IGST, aa.SGST, aa.CGST, b.ProjectNumber }).ToList();
                        receiptAmt = QryReceipt.Sum(m => m.ReceiptAmount ?? 0);

                        receiptAmt = (receiptAmt) / 100000;
                        ExreceiptAmt = (QryReceipt.Sum(m =>
                        {
                            return m.ReceiptAmount - ((m.CGST ?? 0) + (m.SGST ?? 0) + (m.IGST ?? 0));
                        }) / 100000) ?? 0;

                        TtlPrevRecAmt += QryRecAMt;
                        TtlCurrRecAmt += receiptAmt;
                        decimal TotalRecAmt = receiptAmt + QryRecAMt;
                        TtlPrevProj_A += PrevProj_A;
                        TtlCurrProj_A += CurrProj_A;
                        TtlPrevProj_B += PrevProj_B;
                        TtlCurrProj_B += CurrProj_B;
                        TtlPrevProj_C += PrevProj_C;
                        TtlCurrProj_C += CurrProj_C;
                        TtlPrevProj_D += PrevProj_D;
                        TtlCurrProj_D += CurrProj_D;
                        TtlPrevAmt_A += AmtforPrevMonth_A;
                        TtlCurrAmt_A += AmtforCurMonth_A;
                        TtlPrevAmt_B += AmtforPrevMonth_B;
                        TtlCurrAmt_B += AmtforCurMonth_B;
                        TtlPrevAmt_C += AmtforPrevMonth_C;
                        TtlCurrAmt_C += AmtforCurMonth_C;
                        TtlCurrAmt_D += AmtforCurMonth_D;
                        TtlCurrAmt_D += AmtforCurMonth_D;

                        //Ex


                        ExTtlPrevRecAmt += ExQryRecAMt;
                        ExTtlCurrRecAmt += ExreceiptAmt;
                        decimal ExTotalRecAmt = ExreceiptAmt + ExQryRecAMt;

                        ExTtlPrevAmt_A += ExAmtforPrevMonth_A;
                        ExTtlCurrAmt_A += ExAmtforCurMonth_A;
                        ExTtlPrevAmt_B += ExAmtforPrevMonth_B;
                        ExTtlCurrAmt_B += ExAmtforCurMonth_B;
                        ExTtlPrevAmt_C += ExAmtforPrevMonth_C;
                        ExTtlCurrAmt_C += ExAmtforCurMonth_C;
                        ExTtlPrevAmt_D += ExAmtforPrevMonth_D;
                        ExTtlCurrAmt_D += ExAmtforCurMonth_D;

                        var LbAmtforCurMonth_A = ""; var LbAmtforCurMonth_B = ""; var LbAmtforCurMonth_C = ""; var LbAmtforCurMonth_D = "";
                        var ExLbAmtforCurMonth_A = ""; var ExLbAmtforCurMonth_B = ""; var ExLbAmtforCurMonth_C = ""; var ExLbAmtforCurMonth_D = "";

                        if (Enhance_A > 0)
                            LbAmtforCurMonth_A = "*" + String.Format("{0:0.00}", AmtforCurMonth_A);
                        else
                            LbAmtforCurMonth_A = String.Format("{0:0.00}", AmtforCurMonth_A);
                        if (Enhance_B > 0)
                            LbAmtforCurMonth_B = "*" + String.Format("{0:0.00}", AmtforCurMonth_B);
                        else
                            LbAmtforCurMonth_B = String.Format("{0:0.00}", AmtforCurMonth_B);
                        if (Enhance_C > 0)
                            LbAmtforCurMonth_C = "*" + String.Format("{0:0.00}", AmtforCurMonth_C);
                        else
                            LbAmtforCurMonth_C = String.Format("{0:0.00}", AmtforCurMonth_C);
                        if (Enhance_D > 0)
                            LbAmtforCurMonth_D = "*" + String.Format("{0:0.00}", AmtforCurMonth_D);
                        else
                            LbAmtforCurMonth_D = String.Format("{0:0.00}", AmtforCurMonth_D);
                        var LbAmtforPrevMonth_A = ""; var LbAmtforPrevMonth_B = ""; var LbAmtforPrevMonth_C = ""; var LbAmtforPrevMonth_D = "";
                        if (prevEnhance_A > 0)
                            LbAmtforPrevMonth_A = "*" + String.Format("{0:0.00}", AmtforPrevMonth_A);
                        else
                            LbAmtforPrevMonth_A = String.Format("{0:0.00}", AmtforPrevMonth_A);
                        if (prevEnhance_B > 0)
                            LbAmtforPrevMonth_B = "*" + String.Format("{0:0.00}", AmtforPrevMonth_B);
                        else
                            LbAmtforPrevMonth_B = String.Format("{0:0.00}", AmtforPrevMonth_B);
                        if (prevEnhance_C > 0)
                            LbAmtforPrevMonth_C = "*" + String.Format("{0:0.00}", AmtforPrevMonth_C);
                        else
                            LbAmtforPrevMonth_C = String.Format("{0:0.00}", AmtforPrevMonth_C);
                        if (prevEnhance_D > 0)
                            LbAmtforPrevMonth_D = "*" + String.Format("{0:0.00}", AmtforPrevMonth_D);
                        else
                            LbAmtforPrevMonth_D = String.Format("{0:0.00}", AmtforPrevMonth_D);

                        //Ex
                        if (ExEnhance_A > 0)
                            ExLbAmtforCurMonth_A = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_A);
                        else
                            ExLbAmtforCurMonth_A = String.Format("{0:0.00}", ExAmtforCurMonth_A);
                        if (ExEnhance_B > 0)
                            ExLbAmtforCurMonth_B = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_B);
                        else
                            ExLbAmtforCurMonth_B = String.Format("{0:0.00}", ExAmtforCurMonth_B);
                        if (ExEnhance_C > 0)
                            ExLbAmtforCurMonth_C = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_C);
                        else
                            ExLbAmtforCurMonth_C = String.Format("{0:0.00}", ExAmtforCurMonth_C);
                        if (ExEnhance_D > 0)
                            ExLbAmtforCurMonth_D = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_D);
                        else
                            ExLbAmtforCurMonth_D = String.Format("{0:0.00}", ExAmtforCurMonth_D);
                        var ExLbAmtforPrevMonth_A = ""; var ExLbAmtforPrevMonth_B = ""; var ExLbAmtforPrevMonth_C = ""; var ExLbAmtforPrevMonth_D = "";
                        if (ExprevEnhance_A > 0)
                            ExLbAmtforPrevMonth_A = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_A);
                        else
                            ExLbAmtforPrevMonth_A = String.Format("{0:0.00}", ExAmtforPrevMonth_A);
                        if (ExprevEnhance_B > 0)
                            ExLbAmtforPrevMonth_B = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_B);
                        else
                            ExLbAmtforPrevMonth_B = String.Format("{0:0.00}", ExAmtforPrevMonth_B);
                        if (ExprevEnhance_C > 0)
                            ExLbAmtforPrevMonth_C = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_C);
                        else
                            ExLbAmtforPrevMonth_C = String.Format("{0:0.00}", ExAmtforPrevMonth_C);
                        if (ExprevEnhance_D > 0)
                            ExLbAmtforPrevMonth_D = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_D);
                        else
                            ExLbAmtforPrevMonth_D = String.Format("{0:0.00}", ExAmtforPrevMonth_D);


                        if (AmtforCurMonth_A > 0 || AmtforCurMonth_B > 0 || AmtforCurMonth_C > 0 || AmtforPrevMonth_A > 0 || AmtforPrevMonth_B > 0 || AmtforPrevMonth_C > 0 || TotalAmt_B > 0 || TotalAmt_C > 0 || TotalAmt_D > 0 || TotalRecAmt > 0)
                        {
                            List.Add(new OfficeMonthlyListReportModel()
                            {
                                DepartmentName = DeptName,
                                Type = Type,
                                NoOfProjCurMonth_A = CurrProj_A ?? 0,
                                NoOfProjPrevMonth_A = PrevProj_A ?? 0,
                                TotalProj_A = TotalProj_A ?? 0,
                                AmtforCurMonth_A = LbAmtforCurMonth_A,
                                AmtforPrevMonth_A = LbAmtforPrevMonth_A,
                                TotalAmt_A = TotalAmt_A,
                                ExAmtforCurMonth_A = ExLbAmtforCurMonth_A,
                                ExAmtforPrevMonth_A = ExLbAmtforPrevMonth_A,
                                ExTotalAmt_A = ExTotalAmt_A,
                                NoOfProjCurMonth_B = CurrProj_B ?? 0,
                                NoOfProjPrevMonth_B = PrevProj_B ?? 0,
                                TotalProj_B = TotalProj_B ?? 0,
                                AmtforCurMonth_B = LbAmtforCurMonth_B,
                                AmtforPrevMonth_B = LbAmtforPrevMonth_B,
                                TotalAmt_B = TotalAmt_B,
                                ExAmtforCurMonth_B = ExLbAmtforCurMonth_B,
                                ExAmtforPrevMonth_B = ExLbAmtforPrevMonth_B,
                                ExTotalAmt_B = ExTotalAmt_B,
                                NoOfProjCurMonth_C = CurrProj_C ?? 0,
                                NoOfProjPrevMonth_C = PrevProj_C ?? 0,
                                TotalProj_C = TotalProj_C ?? 0,
                                AmtforCurMonth_C = LbAmtforCurMonth_C,
                                AmtforPrevMonth_C = LbAmtforPrevMonth_C,
                                TotalAmt_C = TotalAmt_C,
                                ExAmtforCurMonth_C = ExLbAmtforCurMonth_C,
                                ExAmtforPrevMonth_C = ExLbAmtforPrevMonth_C,
                                ExTotalAmt_C = ExTotalAmt_C,
                                NoOfProjCurMonth_D = CurrProj_D ?? 0,
                                NoOfProjPrevMonth_D = PrevProj_D ?? 0,
                                TotalProj_D = TotalProj_D ?? 0,
                                AmtforCurMonth_D = LbAmtforCurMonth_D,
                                AmtforPrevMonth_D = LbAmtforPrevMonth_D,
                                TotalAmt_D = TotalAmt_D,
                                ExAmtforCurMonth_D = ExLbAmtforCurMonth_D,
                                ExAmtforPrevMonth_D = ExLbAmtforPrevMonth_D,
                                ExTotalAmt_D = ExTotalAmt_D,
                                AmtforRecCurMonth = receiptAmt,
                                AmtforRecPrevMonth = QryRecAMt,
                                TotalRecAmt = TotalRecAmt,
                                ExAmtforRecCurMonth = ExreceiptAmt,
                                ExAmtforRecPrevMonth = ExQryRecAMt,
                                ExTotalRecAmt = ExTotalRecAmt
                            });
                        }
                    }
                    ////////ICSR ////////
                    int[] ICSR = { 16 };
                    if (ICSR.Length > 0)
                    {


                        decimal TotalAmt_A = 0; decimal AmtforPrevMonth_A = 0; decimal AmtforCurMonth_A = 0; decimal receiptAmt = 0; decimal QryRecAMt = 0;
                        decimal ExTotalAmt_A = 0; decimal ExAmtforPrevMonth_A = 0; decimal ExAmtforCurMonth_A = 0; decimal ExreceiptAmt = 0; decimal ExQryRecAMt = 0;
                        decimal TotalAmt_B = 0; decimal AmtforPrevMonth_B = 0; decimal AmtforCurMonth_B = 0;
                        decimal ExTotalAmt_B = 0; decimal ExAmtforPrevMonth_B = 0; decimal ExAmtforCurMonth_B = 0;
                        decimal TotalAmt_C = 0; decimal AmtforPrevMonth_C = 0; decimal AmtforCurMonth_C = 0;
                        decimal ExTotalAmt_C = 0; decimal ExAmtforPrevMonth_C = 0; decimal ExAmtforCurMonth_C = 0;
                        decimal TotalAmt_D = 0; decimal AmtforPrevMonth_D = 0; decimal AmtforCurMonth_D = 0;
                        decimal ExTotalAmt_D = 0; decimal ExAmtforPrevMonth_D = 0; decimal ExAmtforCurMonth_D = 0;
                        var ProjDet_A = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && m.CrtdTS <= todate && m.FinancialYear == FinId
                         && m.ProjectType == 2 && A1.Contains(m.ConsultancyFundingCategory ?? 0)
                       && (m.FacultyDetailId == 71 || m.ProjectClassification == 16)).ToList();
                        var PrevProjDet_A = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0)
                        && (m.FacultyDetailId == 71 || m.ProjectClassification == 16) && m.CrtdTS <= endDate && m.FinancialYear == FinId
                       && m.ProjectType == 2 && A1.Contains(m.ConsultancyFundingCategory ?? 0)
                 ).ToList();
                        int[] EnhanceArr_A = ProjDet_A.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_A = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                             //&& !EnhanceArr_A.Contains(aa.ProjectId ?? 0)
                                             && (b.FacultyDetailId == 71 || b.ProjectClassification == 16) && aa.IsEnhancementonly == true
                                           && b.ProjectType == 2 && A1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_A = ListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_A = ListEnhance_A.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_A = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                                && (b.FacultyDetailId == 71 || b.ProjectClassification == 16)
                                                && aa.IsEnhancementonly == true
                                              && b.ProjectType == 2 && A1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_A = prevListEnhance_A.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_A = prevListEnhance_A.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        prevEnhance_A = (prevEnhance_A / 100000);
                        Enhance_A = (Enhance_A / 100000);
                        AmtforCurMonth_A = (ProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_A = AmtforCurMonth_A + Enhance_A;
                        AmtforPrevMonth_A = (PrevProjDet_A.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_A = AmtforPrevMonth_A + prevEnhance_A;
                        TotalAmt_A = AmtforCurMonth_A + AmtforPrevMonth_A;

                        //Ex

                        ExprevEnhance_A = (ExprevEnhance_A / 100000);
                        ExEnhance_A = (ExEnhance_A / 100000);
                        ExAmtforCurMonth_A = (ProjDet_A.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_A = ExAmtforCurMonth_A + ExEnhance_A;
                        ExAmtforPrevMonth_A = (PrevProjDet_A.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_A = ExAmtforPrevMonth_A + ExprevEnhance_A;
                        ExTotalAmt_A = ExAmtforCurMonth_A + ExAmtforPrevMonth_A;

                        int? PrevProj_A = PrevProjDet_A.Count();
                        int? CurrProj_A = ProjDet_A.Count();
                        int? TotalProj_A = PrevProj_A + CurrProj_A;
                        ////// B //////
                        var ProjDet_B = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && (m.FacultyDetailId == 71 || m.ProjectClassification == 16) && m.CrtdTS <= todate && m.FinancialYear == FinId && m.ProjectType == 2 && B1.Contains(m.ConsultancyFundingCategory ?? 0)
                      ).ToList();
                        var PrevProjDet_B = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && (m.FacultyDetailId == 71 || m.ProjectClassification == 16) && m.CrtdTS <= endDate && m.FinancialYear == FinId
                        && m.ProjectType == 2 && B1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_B = ProjDet_B.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_B = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                              //&& !EnhanceArr_B.Contains(aa.ProjectId ?? 0)
                                              && (b.FacultyDetailId == 71 || b.ProjectClassification == 16) && aa.IsEnhancementonly == true
                                              && b.ProjectType == 2 && B1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_B = ListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_B = ListEnhance_B.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_B = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                              && (b.FacultyDetailId == 71 || b.ProjectClassification == 16) && aa.IsEnhancementonly == true
                                             && b.ProjectType == 2 && B1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_B = prevListEnhance_B.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_B = prevListEnhance_B.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        prevEnhance_B = (prevEnhance_B / 100000);
                        Enhance_B = (Enhance_B / 100000);
                        AmtforCurMonth_B = (ProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_B = AmtforCurMonth_B + Enhance_B;
                        AmtforPrevMonth_B = (PrevProjDet_B.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_B = AmtforPrevMonth_B + prevEnhance_B;
                        TotalAmt_B = AmtforCurMonth_B + AmtforPrevMonth_B;

                        //Ex
                        ExprevEnhance_B = (ExprevEnhance_B / 100000);
                        ExEnhance_B = (ExEnhance_B / 100000);
                        ExAmtforCurMonth_B = (ProjDet_B.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_B = ExAmtforCurMonth_B + ExEnhance_B;
                        ExAmtforPrevMonth_B = (PrevProjDet_B.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_B = ExAmtforPrevMonth_B + ExprevEnhance_B;
                        ExTotalAmt_B = ExAmtforCurMonth_B + ExAmtforPrevMonth_B;


                        int? PrevProj_B = PrevProjDet_B.Count();
                        int? CurrProj_B = ProjDet_B.Count();
                        int? TotalProj_B = PrevProj_B + CurrProj_B;
                        ////// C //////
                        var ProjDet_C = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && (m.FacultyDetailId == 71 || m.ProjectClassification == 16) && m.FinancialYear == FinId && m.CrtdTS <= todate
                       && m.ProjectType == 2 && C1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        var PrevProjDet_C = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && (m.FacultyDetailId == 71 || m.ProjectClassification == 16) && m.CrtdTS <= endDate && m.FinancialYear == FinId
                         && m.ProjectType == 2 && C1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_C = ProjDet_C.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_C = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                             //&& !EnhanceArr_C.Contains(aa.ProjectId ?? 0)
                                             && (b.FacultyDetailId == 71 || b.ProjectClassification == 16) && aa.IsEnhancementonly == true
                                             && b.ProjectType == 2 && C1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_C = ListEnhance_C.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_C = ListEnhance_C.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_C = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                               && (b.FacultyDetailId == 71 || b.ProjectClassification == 16) && aa.IsEnhancementonly == true
                                              && b.ProjectType == 2 && C1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_C = prevListEnhance_C.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_C = prevListEnhance_C.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        prevEnhance_C = (prevEnhance_C / 100000);
                        Enhance_C = (Enhance_C / 100000);
                        AmtforCurMonth_C = (ProjDet_C.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_C = AmtforCurMonth_C + Enhance_C;
                        AmtforPrevMonth_C = (PrevProjDet_C.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_C = AmtforPrevMonth_C + prevEnhance_C;
                        TotalAmt_C = AmtforCurMonth_C + AmtforPrevMonth_C;

                        //Ex
                        ExprevEnhance_C = (ExprevEnhance_C / 100000);
                        ExEnhance_C = (ExEnhance_C / 100000);
                        ExAmtforCurMonth_C = (ProjDet_C.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_C = ExAmtforCurMonth_C + ExEnhance_C;
                        ExAmtforPrevMonth_C = (PrevProjDet_C.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_C = ExAmtforPrevMonth_C + ExprevEnhance_C;
                        ExTotalAmt_C = ExAmtforCurMonth_C + ExAmtforPrevMonth_C;


                        int? PrevProj_C = PrevProjDet_C.Count();
                        int? CurrProj_C = ProjDet_C.Count();
                        int? TotalProj_C = PrevProj_C + CurrProj_C;
                        ////// D //////
                        var ProjDet_D = context.tblProject.Where(m => m.CrtdTS >= fromdate && OfficeCateg.Contains(m.ReportClassification ?? 0) && (m.FacultyDetailId == 71 || m.ProjectClassification == 16) && m.CrtdTS <= todate && m.FinancialYear == FinId && m.ProjectType == 2 && D1.Contains(m.ConsultancyFundingCategory ?? 0)
                    ).ToList();
                        var PrevProjDet_D = context.tblProject.Where(m => m.CrtdTS >= startDate && OfficeCateg.Contains(m.ReportClassification ?? 0) && (m.FacultyDetailId == 71 || m.ProjectClassification == 16) && m.CrtdTS <= endDate
                        && m.FinancialYear == FinId && m.ProjectType == 2 && D1.Contains(m.ConsultancyFundingCategory ?? 0)).ToList();
                        int[] EnhanceArr_D = ProjDet_D.Select(m => m.ProjectId).ToArray();
                        var ListEnhance_D = (from aa in context.tblProjectEnhancement
                                             join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                             where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= fromdate && aa.CrtsTS <= todate
                                                && aa.IsEnhancementonly == true
                                               && (b.FacultyDetailId == 71 || b.ProjectClassification == 16)
                                                && b.ProjectType == 2 && D1.Contains(b.ConsultancyFundingCategory ?? 0)
                                             select aa).ToList();
                        decimal Enhance_D = ListEnhance_D.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExEnhance_D = ListEnhance_D.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        var prevListEnhance_D = (from aa in context.tblProjectEnhancement
                                                 join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                                 where aa.Status == "Active" && OfficeCateg.Contains(b.ReportClassification ?? 0) && aa.CrtsTS >= startDate && aa.CrtsTS <= endDate
                                                && aa.IsEnhancementonly == true
                                                && (b.FacultyDetailId == 71 || b.ProjectClassification == 16) && b.ProjectType == 2 && D1.Contains(b.ConsultancyFundingCategory ?? 0)
                                                 select aa).ToList();
                        decimal prevEnhance_D = prevListEnhance_D.Sum(m => m.EnhancedSanctionValue + (m.EnhancedTaxValue ?? 0)) ?? 0;
                        decimal ExprevEnhance_D = prevListEnhance_D.Sum(m => m.EnhancedSanctionValue) ?? 0;
                        prevEnhance_D = (prevEnhance_D / 100000);
                        Enhance_D = (Enhance_D / 100000);
                        AmtforCurMonth_D = (ProjDet_D.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforCurMonth_D = AmtforCurMonth_D + Enhance_D;
                        AmtforCurMonth_D = 0;
                        AmtforPrevMonth_D = (PrevProjDet_D.Sum(m =>
                        {
                            return ((m.BaseValue * (m.ApplicableTax) ?? 0) / 100) + m.BaseValue;
                        }) / 100000) ?? 0;
                        AmtforPrevMonth_D = AmtforPrevMonth_D + prevEnhance_D;
                        AmtforPrevMonth_D = 0;
                        TotalAmt_D = AmtforCurMonth_D + AmtforPrevMonth_D;

                        //Ex
                        ExprevEnhance_D = (ExprevEnhance_D / 100000);
                        ExEnhance_D = (ExEnhance_D / 100000);
                        ExAmtforCurMonth_D = (ProjDet_D.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforCurMonth_D = ExAmtforCurMonth_D + ExEnhance_D;
                        ExAmtforCurMonth_D = 0;
                        ExAmtforPrevMonth_D = (PrevProjDet_D.Sum(m =>
                        {
                            return m.BaseValue;
                        }) / 100000) ?? 0;
                        ExAmtforPrevMonth_D = ExAmtforPrevMonth_D + ExprevEnhance_D;
                        ExAmtforPrevMonth_D = 0;
                        ExTotalAmt_D = ExAmtforCurMonth_D + ExAmtforPrevMonth_D;

                        int? PrevProj_D = PrevProjDet_D.Count();
                        PrevProj_D = 0;
                        int? CurrProj_D = ProjDet_D.Count();
                        CurrProj_D = 0;
                        int? TotalProj_D = PrevProj_D + CurrProj_D;
                        //  int[] ProjDet = context.tblProject.Where(m => m.CrtdTS >= fromdate && m.FinancialYear == FinId && m.CrtdTS <= todate
                        //    && m.Status == "Active" && m.ProjectType == 2).Select(m => m.ProjectId).ToArray();
                        //  int[] PrevProjDet = context.tblProject.Where(m => m.CrtdTS >= startDate && m.CrtdTS <= endDate && m.FinancialYear == FinId
                        // && m.Status == "Active" && m.ProjectType == 2).Select(m => m.ProjectId).ToArray();
                        int[] ProjClass = { 2, 3, 14 };
                        var QryRect = (from aa in context.tblReceipt
                                       join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                       join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                       where
                                        (b.FacultyDetailId == 71 || b.ProjectClassification == 16) && aa.Posted_f == true && aa.CrtdTS >= startDate && aa.CrtdTS <= endDate
                                     && !Categ.Contains(aa.CategoryId ?? 0) && b.ProjectType == 2 && aa.Status == "Completed"
                                     && !aa.ReceiptNumber.Contains("RBU") && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                       select new { aa.ReceiptAmount, aa.ReceiptOverheadValue, aa.IGST, aa.SGST, aa.CGST, b.ProjectNumber }).ToList();
                        QryRecAMt = QryRect.Sum(m => m.ReceiptAmount ?? 0);

                        QryRecAMt = (QryRecAMt) / 100000;
                        ExQryRecAMt = (QryRect.Sum(m =>
                        {
                            return m.ReceiptAmount - ((m.CGST ?? 0) + (m.SGST ?? 0) + (m.IGST ?? 0));
                        }) / 100000) ?? 0;
                        var QryReceipt = (from aa in context.tblReceipt
                                          join b in context.tblProject on aa.ProjectId equals b.ProjectId
                                          join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where
                                           (b.FacultyDetailId == 71 || b.ProjectClassification == 16) && aa.Posted_f == true && aa.CrtdTS >= fromdate && aa.CrtdTS <= todate
                                      && !Categ.Contains(aa.CategoryId ?? 0) && b.ProjectType == 2 && aa.Status == "Completed"
                                      && !aa.ReceiptNumber.Contains("RBU") && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                          select new { aa.ReceiptAmount, aa.ReceiptOverheadValue, aa.IGST, aa.SGST, aa.CGST, b.ProjectNumber }).ToList();
                        receiptAmt = QryReceipt.Sum(m => m.ReceiptAmount ?? 0);

                        receiptAmt = (receiptAmt) / 100000;
                        ExreceiptAmt = (QryReceipt.Sum(m =>
                        {
                            return m.ReceiptAmount - ((m.CGST ?? 0) + (m.SGST ?? 0) + (m.IGST ?? 0));
                        }) / 100000) ?? 0;

                        TtlPrevRecAmt += QryRecAMt;
                        TtlCurrRecAmt += receiptAmt;
                        decimal TotalRecAmt = receiptAmt + QryRecAMt;
                        TtlPrevProj_A += PrevProj_A;
                        TtlCurrProj_A += CurrProj_A;
                        TtlPrevProj_B += PrevProj_B;
                        TtlCurrProj_B += CurrProj_B;
                        TtlPrevProj_C += PrevProj_C;
                        TtlCurrProj_C += CurrProj_C;
                        TtlPrevProj_D += PrevProj_D;
                        TtlCurrProj_D += CurrProj_D;
                        TtlPrevAmt_A += AmtforPrevMonth_A;
                        TtlCurrAmt_A += AmtforCurMonth_A;
                        TtlPrevAmt_B += AmtforPrevMonth_B;
                        TtlCurrAmt_B += AmtforCurMonth_B;
                        TtlPrevAmt_C += AmtforPrevMonth_C;
                        TtlCurrAmt_C += AmtforCurMonth_C;
                        TtlCurrAmt_D += AmtforCurMonth_D;
                        TtlCurrAmt_D += AmtforCurMonth_D;

                        //Ex


                        ExTtlPrevRecAmt += ExQryRecAMt;
                        ExTtlCurrRecAmt += ExreceiptAmt;
                        decimal ExTotalRecAmt = ExreceiptAmt + ExQryRecAMt;

                        ExTtlPrevAmt_A += ExAmtforPrevMonth_A;
                        ExTtlCurrAmt_A += ExAmtforCurMonth_A;
                        ExTtlPrevAmt_B += ExAmtforPrevMonth_B;
                        ExTtlCurrAmt_B += ExAmtforCurMonth_B;
                        ExTtlPrevAmt_C += ExAmtforPrevMonth_C;
                        ExTtlCurrAmt_C += ExAmtforCurMonth_C;
                        ExTtlPrevAmt_D += ExAmtforPrevMonth_D;
                        ExTtlCurrAmt_D += ExAmtforCurMonth_D;

                        var LbAmtforCurMonth_A = ""; var LbAmtforCurMonth_B = ""; var LbAmtforCurMonth_C = ""; var LbAmtforCurMonth_D = "";
                        var ExLbAmtforCurMonth_A = ""; var ExLbAmtforCurMonth_B = ""; var ExLbAmtforCurMonth_C = ""; var ExLbAmtforCurMonth_D = "";

                        if (Enhance_A > 0)
                            LbAmtforCurMonth_A = "*" + String.Format("{0:0.00}", AmtforCurMonth_A);
                        else
                            LbAmtforCurMonth_A = String.Format("{0:0.00}", AmtforCurMonth_A);
                        if (Enhance_B > 0)
                            LbAmtforCurMonth_B = "*" + String.Format("{0:0.00}", AmtforCurMonth_B);
                        else
                            LbAmtforCurMonth_B = String.Format("{0:0.00}", AmtforCurMonth_B);
                        if (Enhance_C > 0)
                            LbAmtforCurMonth_C = "*" + String.Format("{0:0.00}", AmtforCurMonth_C);
                        else
                            LbAmtforCurMonth_C = String.Format("{0:0.00}", AmtforCurMonth_C);
                        if (Enhance_D > 0)
                            LbAmtforCurMonth_D = "*" + String.Format("{0:0.00}", AmtforCurMonth_D);
                        else
                            LbAmtforCurMonth_D = String.Format("{0:0.00}", AmtforCurMonth_D);
                        var LbAmtforPrevMonth_A = ""; var LbAmtforPrevMonth_B = ""; var LbAmtforPrevMonth_C = ""; var LbAmtforPrevMonth_D = "";
                        if (prevEnhance_A > 0)
                            LbAmtforPrevMonth_A = "*" + String.Format("{0:0.00}", AmtforPrevMonth_A);
                        else
                            LbAmtforPrevMonth_A = String.Format("{0:0.00}", AmtforPrevMonth_A);
                        if (prevEnhance_B > 0)
                            LbAmtforPrevMonth_B = "*" + String.Format("{0:0.00}", AmtforPrevMonth_B);
                        else
                            LbAmtforPrevMonth_B = String.Format("{0:0.00}", AmtforPrevMonth_B);
                        if (prevEnhance_C > 0)
                            LbAmtforPrevMonth_C = "*" + String.Format("{0:0.00}", AmtforPrevMonth_C);
                        else
                            LbAmtforPrevMonth_C = String.Format("{0:0.00}", AmtforPrevMonth_C);
                        if (prevEnhance_D > 0)
                            LbAmtforPrevMonth_D = "*" + String.Format("{0:0.00}", AmtforPrevMonth_D);
                        else
                            LbAmtforPrevMonth_D = String.Format("{0:0.00}", AmtforPrevMonth_D);

                        //Ex
                        if (ExEnhance_A > 0)
                            ExLbAmtforCurMonth_A = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_A);
                        else
                            ExLbAmtforCurMonth_A = String.Format("{0:0.00}", ExAmtforCurMonth_A);
                        if (ExEnhance_B > 0)
                            ExLbAmtforCurMonth_B = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_B);
                        else
                            ExLbAmtforCurMonth_B = String.Format("{0:0.00}", ExAmtforCurMonth_B);
                        if (ExEnhance_C > 0)
                            ExLbAmtforCurMonth_C = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_C);
                        else
                            ExLbAmtforCurMonth_C = String.Format("{0:0.00}", ExAmtforCurMonth_C);
                        if (ExEnhance_D > 0)
                            ExLbAmtforCurMonth_D = "*" + String.Format("{0:0.00}", ExAmtforCurMonth_D);
                        else
                            ExLbAmtforCurMonth_D = String.Format("{0:0.00}", ExAmtforCurMonth_D);
                        var ExLbAmtforPrevMonth_A = ""; var ExLbAmtforPrevMonth_B = ""; var ExLbAmtforPrevMonth_C = ""; var ExLbAmtforPrevMonth_D = "";
                        if (ExprevEnhance_A > 0)
                            ExLbAmtforPrevMonth_A = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_A);
                        else
                            ExLbAmtforPrevMonth_A = String.Format("{0:0.00}", ExAmtforPrevMonth_A);
                        if (ExprevEnhance_B > 0)
                            ExLbAmtforPrevMonth_B = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_B);
                        else
                            ExLbAmtforPrevMonth_B = String.Format("{0:0.00}", ExAmtforPrevMonth_B);
                        if (ExprevEnhance_C > 0)
                            ExLbAmtforPrevMonth_C = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_C);
                        else
                            ExLbAmtforPrevMonth_C = String.Format("{0:0.00}", ExAmtforPrevMonth_C);
                        if (ExprevEnhance_D > 0)
                            ExLbAmtforPrevMonth_D = "*" + String.Format("{0:0.00}", ExAmtforPrevMonth_D);
                        else
                            ExLbAmtforPrevMonth_D = String.Format("{0:0.00}", ExAmtforPrevMonth_D);



                        List.Add(new OfficeMonthlyListReportModel()
                        {
                            DepartmentName = "ICSR",
                            Type = Type,
                            NoOfProjCurMonth_A = CurrProj_A ?? 0,
                            NoOfProjPrevMonth_A = PrevProj_A ?? 0,
                            TotalProj_A = TotalProj_A ?? 0,
                            AmtforCurMonth_A = LbAmtforCurMonth_A,
                            AmtforPrevMonth_A = LbAmtforPrevMonth_A,
                            TotalAmt_A = TotalAmt_A,
                            ExAmtforCurMonth_A = ExLbAmtforCurMonth_A,
                            ExAmtforPrevMonth_A = ExLbAmtforPrevMonth_A,
                            ExTotalAmt_A = ExTotalAmt_A,
                            NoOfProjCurMonth_B = CurrProj_B ?? 0,
                            NoOfProjPrevMonth_B = PrevProj_B ?? 0,
                            TotalProj_B = TotalProj_B ?? 0,
                            AmtforCurMonth_B = LbAmtforCurMonth_B,
                            AmtforPrevMonth_B = LbAmtforPrevMonth_B,
                            TotalAmt_B = TotalAmt_B,
                            ExAmtforCurMonth_B = ExLbAmtforCurMonth_B,
                            ExAmtforPrevMonth_B = ExLbAmtforPrevMonth_B,
                            ExTotalAmt_B = ExTotalAmt_B,
                            NoOfProjCurMonth_C = CurrProj_C ?? 0,
                            NoOfProjPrevMonth_C = PrevProj_C ?? 0,
                            TotalProj_C = TotalProj_C ?? 0,
                            AmtforCurMonth_C = LbAmtforCurMonth_C,
                            AmtforPrevMonth_C = LbAmtforPrevMonth_C,
                            TotalAmt_C = TotalAmt_C,
                            ExAmtforCurMonth_C = ExLbAmtforCurMonth_C,
                            ExAmtforPrevMonth_C = ExLbAmtforPrevMonth_C,
                            ExTotalAmt_C = ExTotalAmt_C,
                            NoOfProjCurMonth_D = CurrProj_D ?? 0,
                            NoOfProjPrevMonth_D = PrevProj_D ?? 0,
                            TotalProj_D = TotalProj_D ?? 0,
                            AmtforCurMonth_D = LbAmtforCurMonth_D,
                            AmtforPrevMonth_D = LbAmtforPrevMonth_D,
                            TotalAmt_D = TotalAmt_D,
                            ExAmtforCurMonth_D = ExLbAmtforCurMonth_D,
                            ExAmtforPrevMonth_D = ExLbAmtforPrevMonth_D,
                            ExTotalAmt_D = ExTotalAmt_D,
                            AmtforRecCurMonth = receiptAmt,
                            AmtforRecPrevMonth = QryRecAMt,
                            TotalRecAmt = TotalRecAmt,
                            ExAmtforRecCurMonth = ExreceiptAmt,
                            ExAmtforRecPrevMonth = ExQryRecAMt,
                            ExTotalRecAmt = ExTotalRecAmt
                        });
                    }
                    TtlProj_A = TtlPrevProj_A + TtlCurrProj_A;
                    TtlProj_B = TtlPrevProj_B + TtlCurrProj_B;
                    TtlProj_C = TtlPrevProj_C + TtlCurrProj_C;
                    TtlProj_D = TtlPrevProj_D + TtlCurrProj_D;
                    TtlPrevProj = TtlPrevProj_A + TtlPrevProj_B + TtlPrevProj_C + TtlPrevProj_D;
                    TtlCurrProj = TtlCurrProj_A + TtlCurrProj_B + TtlCurrProj_C + TtlCurrProj_D;
                    TtlProj = TtlProj_A + TtlProj_B + TtlProj_C + TtlProj_D;
                    TtlAmt_A = TtlPrevAmt_A + TtlCurrAmt_A;
                    TtlAmt_B = TtlPrevAmt_B + TtlCurrAmt_B;
                    TtlAmt_C = TtlPrevAmt_C + TtlCurrAmt_C;
                    TtlAmt_D = TtlPrevAmt_D + TtlCurrAmt_D;
                    TtlPrevAmt = TtlPrevAmt_A + TtlPrevAmt_B + TtlPrevAmt_C + TtlPrevAmt_D;
                    TtlCurrAmt = TtlCurrAmt_A + TtlCurrAmt_B + TtlCurrAmt_C + TtlCurrAmt_D;
                    TtlAmt = TtlAmt_A + TtlAmt_B + TtlAmt_C + TtlAmt_D;
                    TtlRecAmt = TtlCurrRecAmt + TtlPrevRecAmt;

                    //Ex
                    ExTtlAmt_A = ExTtlPrevAmt_A + ExTtlCurrAmt_A;
                    ExTtlAmt_B = ExTtlPrevAmt_B + ExTtlCurrAmt_B;
                    ExTtlAmt_C = ExTtlPrevAmt_C + ExTtlCurrAmt_C;
                    ExTtlAmt_D = ExTtlPrevAmt_D + ExTtlCurrAmt_D;
                    ExTtlPrevAmt = ExTtlPrevAmt_A + ExTtlPrevAmt_B + ExTtlPrevAmt_C + ExTtlPrevAmt_D;
                    ExTtlCurrAmt = ExTtlCurrAmt_A + ExTtlCurrAmt_B + ExTtlCurrAmt_C + ExTtlCurrAmt_D;
                    ExTtlAmt = ExTtlAmt_A + ExTtlAmt_B + ExTtlAmt_C + ExTtlAmt_D;
                    ExTtlRecAmt = ExTtlCurrRecAmt + ExTtlPrevRecAmt;
                    List.Add(new OfficeMonthlyListReportModel()
                    {
                        DepartmentName = "Total",
                        Type = Type,
                        NoOfProjCurMonth_A = TtlCurrProj_A ?? 0,
                        NoOfProjPrevMonth_A = TtlPrevProj_A ?? 0,
                        TotalProj_A = TtlProj_A ?? 0,
                        AmtforCurMonth_A = String.Format("{0:0.00}", TtlCurrAmt_A),
                        AmtforPrevMonth_A = String.Format("{0:0.00}", TtlPrevAmt_A),
                        TotalAmt_A = TtlAmt_A ?? 0,
                        ExAmtforCurMonth_A = String.Format("{0:0.00}", ExTtlCurrAmt_A),
                        ExAmtforPrevMonth_A = String.Format("{0:0.00}", ExTtlPrevAmt_A),
                        ExTotalAmt_A = ExTtlAmt_A ?? 0,
                        NoOfProjCurMonth_B = TtlCurrProj_B ?? 0,
                        NoOfProjPrevMonth_B = TtlPrevProj_B ?? 0,
                        TotalProj_B = TtlProj_B ?? 0,
                        AmtforCurMonth_B = String.Format("{0:0.00}", TtlCurrAmt_B),
                        AmtforPrevMonth_B = String.Format("{0:0.00}", TtlPrevAmt_B),
                        TotalAmt_B = TtlAmt_B ?? 0,
                        ExAmtforCurMonth_B = String.Format("{0:0.00}", ExTtlCurrAmt_B),
                        ExAmtforPrevMonth_B = String.Format("{0:0.00}", ExTtlPrevAmt_B),
                        ExTotalAmt_B = ExTtlAmt_B ?? 0,
                        NoOfProjCurMonth_C = TtlCurrProj_C ?? 0,
                        NoOfProjPrevMonth_C = TtlPrevProj_C ?? 0,
                        TotalProj_C = TtlProj_C ?? 0,
                        AmtforCurMonth_C = String.Format("{0:0.00}", TtlCurrAmt_C),
                        AmtforPrevMonth_C = String.Format("{0:0.00}", TtlPrevAmt_C),
                        TotalAmt_C = TtlAmt_C ?? 0,
                        ExAmtforCurMonth_C = String.Format("{0:0.00}", ExTtlCurrAmt_C),
                        ExAmtforPrevMonth_C = String.Format("{0:0.00}", ExTtlPrevAmt_C),
                        ExTotalAmt_C = ExTtlAmt_C ?? 0,
                        NoOfProjCurMonth_D = TtlCurrProj_D ?? 0,
                        NoOfProjPrevMonth_D = TtlPrevProj_D ?? 0,
                        TotalProj_D = TtlProj_D ?? 0,
                        AmtforCurMonth_D = String.Format("{0:0.00}", TtlCurrAmt_D),
                        AmtforPrevMonth_D = String.Format("{0:0.00}", TtlPrevAmt_D),
                        TotalAmt_D = TtlAmt_D ?? 0,
                        ExAmtforCurMonth_D = String.Format("{0:0.00}", ExTtlCurrAmt_D),
                        ExAmtforPrevMonth_D = String.Format("{0:0.00}", ExTtlPrevAmt_D),
                        ExTotalAmt_D = ExTtlAmt_D ?? 0,
                        AmtforRecCurMonth = TtlCurrRecAmt ?? 0,
                        AmtforRecPrevMonth = TtlPrevRecAmt ?? 0,
                        TotalRecAmt = TtlRecAmt ?? 0,
                        ExAmtforRecCurMonth = ExTtlCurrRecAmt ?? 0,
                        ExAmtforRecPrevMonth = ExTtlPrevRecAmt ?? 0,
                        ExTotalRecAmt = ExTtlRecAmt ?? 0
                    });
                    model.TotalAmount = TtlCurrAmt ?? 0;
                    model.TotalPrevAmt = TtlPrevAmt ?? 0;
                    model.ExTotalAmount = ExTtlCurrAmt ?? 0;
                    model.ExTotalPrevAmt = ExTtlPrevAmt ?? 0;
                    model.List = List;
                    if (fromdate.Month != 4)
                    {
                        model.FromDate = String.Format("{0:MMM yyyy}", startDate);
                        model.ToDate = String.Format("{0:MMM yyyy}", endDate);
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion
        #region Receipt Report
        public static ReceiptReportModel GetSponserReceiptReportDetails(string Month)
        {
            ReceiptReportModel model = new ReceiptReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    decimal? Amt;
                    FinOp fac = new FinOp(System.DateTime.Now);
                    var fromdate = fac.GetMonthFirstDate(Month);
                    var todate = fac.GetMonthLastDate(Month);
                    todate = todate.AddDays(1).AddTicks(-2);
                    DateTime startDate; DateTime endDate;
                    if (fromdate.Month > 3)
                    {
                        startDate = new DateTime(fromdate.Year, 4, 1);
                        endDate = fromdate.AddTicks(-2);
                    }
                    else
                    {
                        startDate = new DateTime((fromdate.Year) - 1, 4, 1);
                        endDate = fromdate.AddTicks(-2);
                    }
                    DateTime SecondStartDate = endDate;
                    DateTime SecondEndDate = fromdate.AddYears(1).AddTicks(-2);
                    int NoOfMonth = ((endDate.Month + endDate.Year * 12) - (startDate.Month + startDate.Year * 12)) + 1;
                    int SecondNoOfMonth = ((SecondEndDate.Month + SecondEndDate.Year * 12) - (SecondStartDate.Month + SecondStartDate.Year * 12)) + 1;
                    model.NoOfClm = NoOfMonth + 3;
                    model.Month = Month;
                    DateTime start;
                    DateTime end;
                    decimal? CurrRecAmt = 0; decimal? PrevRecAmt = 0;
                    int CurrProjCount = 0; int PrevProjCount = 0;
                    int CurrentYear = fromdate.Year;
                    int PreviousYear = fromdate.Year - 1;
                    int NextYear = fromdate.Year + 1;
                    string PreYear = PreviousYear.ToString();
                    string NexYear = NextYear.ToString();
                    string CurYear = CurrentYear.ToString();
                    string FinYear = null;
                    if (fromdate.Month > 3)
                        FinYear = CurYear.Substring(2) + NexYear.Substring(2);
                    else
                        FinYear = PreYear.Substring(2) + CurYear.Substring(2);
                    int FinId = context.tblFinYear.Where(m => m.Year == FinYear).Select(m => m.FinYearId).FirstOrDefault();
                    List<ReceiptReportListModel> ValuesList = new List<ReceiptReportListModel>();
                    List<MonthListReportModel> Monlist = new List<MonthListReportModel>();
                    List<SponSchemeList> SponScheme = new List<SponSchemeList>();
                    var QryDept = context.tblProject.Select(m => m.PIDepartment).Distinct().ToList();
                    var QryHead = (from c in context.tblProject
                                   join d in context.tblFacultyDetail on c.PIDepartment equals d.DepartmentCode
                                   where !String.IsNullOrEmpty(d.DepartmentCode) && !String.IsNullOrEmpty(d.DepartmentName) && !String.IsNullOrEmpty(c.PIDepartment)
                                   select new { c.PIDepartment, d.DepartmentName }).Distinct().ToList();
                    var center = (from a in context.tblCommonHeads
                                  where a.CategoryId == 2
                                  select new { DepartmentName = a.Head, PIDepartment = a.Code }).ToList();
                    string[] Qry = QryHead.Select(m => m.PIDepartment).Distinct().ToArray();
                    var SchemeList = context.tblSchemes.Where(m => m.ProjectType == 2).ToList();
                    int[] ProjClass = { 4, 5, 6, 7 };
                    int[] Categ = { 1, 14, 13, 16 };

                    int[] ReceiptCateg = { 1, 3 };
                    var TTlRecQry = (from a in context.tblReceipt
                                     join b in context.tblProject on a.ProjectId equals b.ProjectId
                                     where a.CrtdTS >= startDate && a.CrtdTS <= endDate
                                     && !Categ.Contains(a.CategoryId ?? 0) && b.ProjectType == 1 && a.Status == "Completed" && !a.ReceiptNumber.Contains("RBU")
                                      && ReceiptCateg.Contains(b.ReportClassification ?? 0) && a.Posted_f == true
                                     select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                    int PrevNoOfProject = TTlRecQry.Select(m => m.ProjectNumber).ToList().Distinct().Count();
                    decimal PrevRec = TTlRecQry.Select(mn => mn.ReceiptAmount).Sum() ?? 0;
                    decimal PrevValue = (PrevRec) / 100000;
                    var TTlCurrRecQry = (from a in context.tblReceipt
                                         join b in context.tblProject on a.ProjectId equals b.ProjectId
                                         where a.CrtdTS >= fromdate && a.CrtdTS <= todate
                                         && !Categ.Contains(a.CategoryId ?? 0) && b.ProjectType == 1 && a.Status == "Completed" && !a.ReceiptNumber.Contains("RBU")
                                      && ReceiptCateg.Contains(b.ReportClassification ?? 0) && a.Posted_f == true
                                         select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                    int CurrNoOfProject = TTlCurrRecQry.Select(m => m.ProjectNumber).ToList().Distinct().Count();
                    decimal CurrRec = TTlCurrRecQry.Select(mn => mn.ReceiptAmount).Sum() ?? 0;
                    decimal CurrValue = (CurrRec) / 100000;
                    string[] ProjectNumber = new string[] { };
                    for (int j = 0; j < Qry.Length; j++)
                    {
                        decimal? TtlAmt = 0;
                        ValuesList.Add(new ReceiptReportListModel()
                        {
                            values = Qry[j]
                        });
                        for (int i = 0; i <= NoOfMonth; i++)
                        {
                            start = startDate.AddMonths(i);
                            end = start.AddMonths(1);
                            end = end.AddTicks(-2);
                            var DeptCode = Qry[j].Trim();
                            var RecQry = (from a in context.tblReceipt
                                          join b in context.tblProject on a.ProjectId equals b.ProjectId
                                          join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where !Categ.Contains(a.CategoryId ?? 0) && c.DepartmentCode == DeptCode && a.CrtdTS >= start && a.CrtdTS <= end
                                          && !b.ProjectNumber.Contains("ISRO") && a.Posted_f == true
                                          && !b.ProjectNumber.StartsWith("DIA")
                                          && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                           && b.ProjectType == 1 && a.Status == "Completed" && !a.ReceiptNumber.Contains("RBU")
                                        && b.FacultyDetailId == null

                                          select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                            ProjectNumber = ProjectNumber.Concat(RecQry.Select(mn => mn.ProjectNumber).ToArray()).ToArray();
                            decimal Rec = RecQry.Select(mn => mn.ReceiptAmount).Sum() ?? 0;
                            Amt = (Rec) / 100000;
                            TtlAmt += Amt;
                            if (i != NoOfMonth)
                            {
                                PrevProjCount += RecQry.Select(m => m.ProjectNumber).ToList().Distinct().Count();
                                PrevRecAmt += Amt;
                            }
                            else
                            {
                                CurrProjCount += RecQry.Select(m => m.ProjectNumber).ToList().Distinct().Count();
                                CurrRecAmt += Amt;
                            }

                            ValuesList.Add(new ReceiptReportListModel()
                            {
                                values = String.Format("{0:0.00}", Amt)
                            });
                        }
                        ValuesList.Add(new ReceiptReportListModel()
                        {
                            values = String.Format("{0:0.00}", TtlAmt)
                        });
                    }
                    var Center = context.tblCommonHeads.Where(m => m.CategoryId == 2 && (m.GroupId == 2 || m.GroupId == 5) && m.HeadId != 67).ToArray();
                    for (int j = 0; j < Center.Length; j++)
                    {
                        var DeptName = Center[j].Code;
                        int DeptCode = Center[j].HeadId;
                        decimal? TtlAmt = 0;
                        ValuesList.Add(new ReceiptReportListModel()
                        {
                            values = DeptName
                        });
                        for (int i = 0; i <= NoOfMonth; i++)
                        {
                            start = startDate.AddMonths(i);
                            end = start.AddMonths(1);
                            end = end.AddTicks(-2);
                            var RecQry = (from a in context.tblReceipt
                                          join b in context.tblProject on a.ProjectId equals b.ProjectId
                                          //  join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where
                                         //!b.ProjectNumber.Contains("IITM")
                                         // && 
                                         !Categ.Contains(a.CategoryId ?? 0) && a.CrtdTS >= start && a.CrtdTS <= end
                                && b.ProjectType == 1 && a.Posted_f == true
                                  && a.Status == "Completed" && !a.ReceiptNumber.Contains("RBU")
                                  && !b.ProjectNumber.Contains("ISRO")
                                  && !b.ProjectNumber.StartsWith("DIA") && b.FacultyDetailId == DeptCode
                                      && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                          select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                            decimal Rec = RecQry.Select(mn => mn.ReceiptAmount).Sum() ?? 0;
                            ProjectNumber = ProjectNumber.Concat(RecQry.Select(mn => mn.ProjectNumber).ToArray()).ToArray();
                            Amt = (Rec) / 100000;
                            TtlAmt += Amt;
                            if (i != NoOfMonth)
                            {
                                PrevProjCount += RecQry.Select(m => m.ProjectNumber).ToList().Distinct().Count();
                                PrevRecAmt += Amt;
                            }
                            else
                            {
                                CurrProjCount += RecQry.Select(m => m.ProjectNumber).ToList().Distinct().Count();
                                CurrRecAmt += Amt;
                            }
                            ValuesList.Add(new ReceiptReportListModel()
                            {
                                values = String.Format("{0:0.00}", Amt)
                            });
                        }
                        ValuesList.Add(new ReceiptReportListModel()
                        {
                            values = String.Format("{0:0.00}", TtlAmt)
                        });
                    }
                    //ISRO


                    if (Month != null)
                    {
                        decimal? TtlAmt = 0;
                        ValuesList.Add(new ReceiptReportListModel()
                        {
                            values = "ISRO-IITM Cell"
                        });
                        for (int i = 0; i <= NoOfMonth; i++)
                        {
                            start = startDate.AddMonths(i);
                            end = start.AddMonths(1);
                            end = end.AddTicks(-2);
                            var RecQry = (from a in context.tblReceipt
                                          join b in context.tblProject on a.ProjectId equals b.ProjectId
                                          //  join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where
                                        !Categ.Contains(a.CategoryId ?? 0) && a.CrtdTS >= start && a.CrtdTS <= end
                                      && b.ProjectType == 1 && a.Status == "Completed" && !a.ReceiptNumber.Contains("RBU")
                                      && a.Posted_f == true
                                      && b.ProjectNumber.Contains("ISRO") && !b.ProjectNumber.StartsWith("DIA")
                                       && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                          select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                            decimal Rec = RecQry.Select(mn => mn.ReceiptAmount).Sum() ?? 0;
                            ProjectNumber = ProjectNumber.Concat(RecQry.Select(mn => mn.ProjectNumber).ToArray()).ToArray();
                            Amt = (Rec) / 100000;
                            TtlAmt += Amt;
                            if (i != NoOfMonth)
                            {
                                PrevProjCount += RecQry.Select(m => m.ProjectNumber).ToList().Distinct().Count();
                                PrevRecAmt += Amt;
                            }
                            else
                            {
                                CurrProjCount += RecQry.Select(m => m.ProjectNumber).ToList().Distinct().Count();
                                CurrRecAmt += Amt;
                            }
                            ValuesList.Add(new ReceiptReportListModel()
                            {
                                values = String.Format("{0:0.00}", Amt)
                            });
                        }
                        ValuesList.Add(new ReceiptReportListModel()
                        {
                            values = String.Format("{0:0.00}", TtlAmt)
                        });
                    }
                    //DIA

                    string DIA = "DIA";
                    if (DIA != null)
                    {
                        decimal? TtlAmt = 0;
                        ValuesList.Add(new ReceiptReportListModel()
                        {
                            values = "DIA"
                        });
                        for (int i = 0; i <= NoOfMonth; i++)
                        {
                            start = startDate.AddMonths(i);
                            end = start.AddMonths(1);
                            end = end.AddTicks(-2);
                            var RecQry = (from a in context.tblReceipt
                                          join b in context.tblProject on a.ProjectId equals b.ProjectId
                                          //    join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where
                                          a.CrtdTS >= start && a.CrtdTS <= end && !Categ.Contains(a.CategoryId ?? 0)
                                        && b.ProjectType == 1 && a.Status == "Completed" && !a.ReceiptNumber.Contains("RBU")
                                        && !b.ProjectNumber.Contains("ISRO") && (b.ProjectNumber.StartsWith("DIA") || b.FacultyDetailId == 67)
                                          && ReceiptCateg.Contains(b.ReportClassification ?? 0) && a.Posted_f == true
                                          select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                            decimal Rec = RecQry.Select(mn => mn.ReceiptAmount).Sum() ?? 0;
                            ProjectNumber = ProjectNumber.Concat(RecQry.Select(mn => mn.ProjectNumber).ToArray()).ToArray();
                            Amt = (Rec) / 100000;
                            TtlAmt += Amt;
                            if (i != NoOfMonth)
                            {
                                PrevProjCount += RecQry.Select(m => m.ProjectNumber).ToList().Distinct().Count();
                                PrevRecAmt += Amt;
                            }
                            else
                            {
                                CurrProjCount += RecQry.Select(m => m.ProjectNumber).ToList().Distinct().Count();
                                CurrRecAmt += Amt;
                            }
                            ValuesList.Add(new ReceiptReportListModel()
                            {
                                values = String.Format("{0:0.00}", Amt)
                            });
                        }
                        ValuesList.Add(new ReceiptReportListModel()
                        {
                            values = String.Format("{0:0.00}", TtlAmt)
                        });
                    }
                    for (int l = 0; l <= NoOfMonth; l++)
                    {
                        start = startDate.AddMonths(l);
                        end = start.AddMonths(1);
                        end = end.AddDays(-1);
                        Monlist.Add(new MonthListReportModel()
                        {
                            Month = String.Format("{0:MMM yyyy}", start)
                        });
                    }
                    Monlist.Add(new MonthListReportModel()
                    {
                        Month = "Total Grant Received"
                    });
                    model.List = ValuesList;
                    model.MonthList = Monlist;
                    SponScheme.Add(new SponSchemeList()
                    {
                        Name = "Sponsored Research Projects",
                        PrevNoOfProject = PrevProjCount,
                        PrevValue = String.Format("{0:0.00}", PrevRecAmt),
                        CurrNoOfProject = CurrProjCount,
                        CurrValue = String.Format("{0:0.00}", CurrRecAmt),
                        TotalValue = String.Format("{0:0.00}", CurrRecAmt + PrevRecAmt),
                        TotalNoOfProject = ProjectNumber.Distinct().Count()
                    });
                    model.SponScheme = SponScheme;
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        public static ConsReceiptReportModel GetConsultancyReceiptReportDetails(string Month)
        {
            ConsReceiptReportModel model = new ConsReceiptReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    decimal? Amt;
                    decimal? ExAmt;
                    FinOp fac = new FinOp(System.DateTime.Now);
                    var fromdate = fac.GetMonthFirstDate(Month);
                    var todate = fac.GetMonthLastDate(Month);
                    todate = todate.AddDays(1).AddTicks(-2);
                    DateTime startDate; DateTime endDate;
                    if (fromdate.Month > 3)
                    {
                        startDate = new DateTime(fromdate.Year, 4, 1);
                        endDate = fromdate.AddTicks(-2);
                    }
                    else
                    {
                        startDate = new DateTime((fromdate.Year) - 1, 4, 1);
                        endDate = fromdate.AddTicks(-2);
                    }
                    int NoOfMonth = ((endDate.Month + endDate.Year * 12) - (startDate.Month + startDate.Year * 12)) + 1;
                    model.NoOfClm = NoOfMonth + 3;
                    model.NoOfClm = model.NoOfClm * 2;
                    model.Month = Month;
                    DateTime start;
                    DateTime end;
                    List<ConsReceiptReportListModel> ValuesList = new List<ConsReceiptReportListModel>();
                    List<ConsMonthListReportModel> Monlist = new List<ConsMonthListReportModel>();
                    List<ConsSchemeList> ConsScheme = new List<ConsSchemeList>();

                    var QryDept = context.tblProject.Select(m => m.PIDepartment).Distinct().ToList();
                    var QryHead = (from c in context.tblProject
                                   join d in context.tblFacultyDetail on c.PIDepartment equals d.DepartmentCode
                                   where !String.IsNullOrEmpty(d.DepartmentCode) && !String.IsNullOrEmpty(d.DepartmentName) && !String.IsNullOrEmpty(c.PIDepartment)
                                   select new { c.PIDepartment, d.DepartmentName }).Distinct().ToList();
                    var center = (from a in context.tblCommonHeads
                                  where a.CategoryId == 2
                                  select new { DepartmentName = a.Head, PIDepartment = a.Code }).ToList();
                    string[] Qry = QryHead.Select(m => m.PIDepartment).Distinct().ToArray();
                    var SchemeList = context.tblSchemes.Where(m => m.ProjectType == 2).ToList();
                    int[] ProjClass = { 2, 3, 14 };
                    int[] Categ = { 1, 14, 13, 16 };
                    int[] ReceiptCateg = { 1, 3 };
                    string[] Projno = { "DEAN", "IT", "BMF" };
                    for (int m = 0; m < SchemeList.Count; m++)
                    {
                        var Scheme = SchemeList[m].SchemeName;
                        int SchemeId = SchemeList[m].SchemeId;
                        string[] ProjectNumber = new string[] { };
                        var RecQry = (from a in context.tblReceipt
                                      join b in context.tblProject on a.ProjectId equals b.ProjectId
                                      where b.ConsultancyFundingCategory == SchemeId
                                     && !Categ.Contains(a.CategoryId ?? 0) && b.ProjectType == 2 && a.Status == "Completed" && !a.ReceiptNumber.Contains("RBU")
                                     && a.CrtdTS >= startDate && a.CrtdTS <= endDate
                                       && ReceiptCateg.Contains(b.ReportClassification ?? 0) && a.Posted_f == true

                                      select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                        int PrevNoOfProject = RecQry.Select(mn => mn.ProjectNumber).Distinct().Count();
                        decimal PrevRec = RecQry.Select(mn => mn.ReceiptAmount).Sum() ?? 0;
                        decimal PrevValue = (PrevRec) / 100000;
                        decimal ExPrevValue = (RecQry.Sum(mn =>
                        {
                            return mn.ReceiptAmount - ((mn.CGST ?? 0) + (mn.SGST ?? 0) + (mn.IGST ?? 0));
                        }) / 100000) ?? 0;
                        ProjectNumber = ProjectNumber.Concat(RecQry.Select(mn => mn.ProjectNumber).ToArray()).ToArray();
                        var CurrRecQry = (from a in context.tblReceipt
                                          join b in context.tblProject on a.ProjectId equals b.ProjectId
                                          where
                                          b.ConsultancyFundingCategory == SchemeId
                                        && !Categ.Contains(a.CategoryId ?? 0) && a.CrtdTS >= fromdate && a.CrtdTS <= todate
                                     && b.ProjectType == 2 && a.Status == "Completed" && !a.ReceiptNumber.Contains("RBU")
                                          && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                          && a.Posted_f == true
                                          select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                        int CurrNoOfProject = CurrRecQry.Select(mn => mn.ProjectNumber).Distinct().Count();
                        decimal CurrRec = CurrRecQry.Select(mn => mn.ReceiptAmount).Sum() ?? 0;
                        decimal CurrValue = (CurrRec) / 100000;
                        ProjectNumber = ProjectNumber.Concat(CurrRecQry.Select(mn => mn.ProjectNumber).ToArray()).ToArray();
                        decimal ExCurrValue = (CurrRecQry.Sum(mn =>
                        {
                            return mn.ReceiptAmount - ((mn.CGST ?? 0) + (mn.SGST ?? 0) + (mn.IGST ?? 0));
                        }) / 100000) ?? 0;
                        int totProject = ProjectNumber.Distinct().Count();
                        ConsScheme.Add(new ConsSchemeList()
                        {
                            Name = Scheme,
                            PrevNoOfProject = PrevNoOfProject,
                            PrevValue = String.Format("{0:0.00}", PrevValue),
                            ExPrevValue = String.Format("{0:0.00}", ExPrevValue),
                            CurrNoOfProject = CurrNoOfProject,
                            CurrValue = String.Format("{0:0.00}", CurrValue),
                            ExCurrValue = String.Format("{0:0.00}", ExCurrValue),
                            TotalIncValue = String.Format("{0:0.00}", PrevValue + CurrValue),
                            TotalExValue = String.Format("{0:0.00}", ExPrevValue + ExCurrValue),
                            TotalNoOfProject = totProject
                        });
                    }
                    model.ConsScheme = ConsScheme;

                    for (int j = 0; j < Qry.Length; j++)
                    {
                        decimal? TtlAmt = 0;
                        decimal? ExTtlAmt = 0;
                        ValuesList.Add(new ConsReceiptReportListModel()
                        {
                            values = Qry[j]
                        });
                        for (int i = 0; i <= NoOfMonth; i++)
                        {
                            start = startDate.AddMonths(i);
                            end = start.AddMonths(1);
                            end = end.AddTicks(-2);
                            var DeptCode = Qry[j].Trim();
                            var RecQry = (from a in context.tblReceipt
                                          join b in context.tblProject on a.ProjectId equals b.ProjectId
                                          join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where !(b.ConsultancyFundingCategory == 9) && !(b.ConsultancyFundingCategory == 12)
                                         && !Categ.Contains(a.CategoryId ?? 0) && c.DepartmentCode == DeptCode && a.CrtdTS >= start && a.CrtdTS <= end
                                          && b.ProjectType == 2 && a.Status == "Completed" && !a.ReceiptNumber.Contains("RBU")
                                          && b.FacultyDetailId == null && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                          && a.Posted_f == true
                                          select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                            decimal Rec = RecQry.Select(mn => mn.ReceiptAmount).Sum() ?? 0;
                            Amt = (Rec) / 100000;
                            TtlAmt += Amt;

                            decimal ExRec = (RecQry.Sum(mn =>
                            {
                                return mn.ReceiptAmount - ((mn.CGST ?? 0) + (mn.SGST ?? 0) + (mn.IGST ?? 0));
                            }) / 100000) ?? 0;
                            ExAmt = ExRec;
                            ExTtlAmt += ExAmt;
                            ValuesList.Add(new ConsReceiptReportListModel()
                            {
                                values = String.Format("{0:0.00}", Amt)
                            });
                            ValuesList.Add(new ConsReceiptReportListModel()
                            {
                                values = String.Format("{0:0.00}", ExAmt)
                            });
                        }
                        ValuesList.Add(new ConsReceiptReportListModel()
                        {
                            values = String.Format("{0:0.00}", TtlAmt)
                        });
                        ValuesList.Add(new ConsReceiptReportListModel()
                        {
                            values = String.Format("{0:0.00}", ExTtlAmt)
                        });
                    }
                    int[] ConsFun = { 9, 12 };
                    for (int j = 0; j < ConsFun.Length; j++)
                    {
                        decimal? TtlAmt = 0;
                        decimal? ExTtlAmt = 0;
                        ValuesList.Add(new ConsReceiptReportListModel()
                        {
                            values = ConsFun[j] == 9 ? "TT" : "CR"
                        });
                        for (int i = 0; i <= NoOfMonth; i++)
                        {
                            start = startDate.AddMonths(i);
                            end = start.AddMonths(1);
                            end = end.AddTicks(-2);
                            var Cons = ConsFun[j];
                            var RecQry = (from a in context.tblReceipt
                                          join b in context.tblProject on a.ProjectId equals b.ProjectId
                                          join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where b.ConsultancyFundingCategory == Cons && a.CrtdTS >= start && a.CrtdTS <= end
                                         && !Categ.Contains(a.CategoryId ?? 0) && b.ProjectType == 2 && a.Status == "Completed" && a.Posted_f == true
                                          && !a.ReceiptNumber.Contains("RBU") && ReceiptCateg.Contains(b.ReportClassification ?? 0)
                                          select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                            decimal Rec = RecQry.Select(mn => mn.ReceiptAmount).Sum() ?? 0;
                            Amt = (Rec) / 100000;
                            TtlAmt += Amt;

                            decimal ExRec = (RecQry.Sum(mn =>
                            {
                                return mn.ReceiptAmount - ((mn.CGST ?? 0) + (mn.SGST ?? 0) + (mn.IGST ?? 0));
                            }) / 100000) ?? 0;
                            ExAmt = ExRec;
                            ExTtlAmt += ExAmt;
                            ValuesList.Add(new ConsReceiptReportListModel()
                            {
                                values = String.Format("{0:0.00}", Amt)
                            });
                            ValuesList.Add(new ConsReceiptReportListModel()
                            {
                                values = String.Format("{0:0.00}", ExAmt)
                            });
                        }
                        ValuesList.Add(new ConsReceiptReportListModel()
                        {
                            values = String.Format("{0:0.00}", TtlAmt)
                        });
                        ValuesList.Add(new ConsReceiptReportListModel()
                        {
                            values = String.Format("{0:0.00}", ExTtlAmt)
                        });
                    }
                    var Center = context.tblCommonHeads.Where(m => m.CategoryId == 2 && (m.GroupId == 2 || m.GroupId == 5)).ToArray();
                    for (int j = 0; j < Center.Length; j++)
                    {
                        decimal? TtlAmt = 0; decimal? ExTtlAmt = 0;
                        ValuesList.Add(new ConsReceiptReportListModel()
                        {
                            values = Center[j].Code
                        });
                        for (int i = 0; i <= NoOfMonth; i++)
                        {
                            start = startDate.AddMonths(i);
                            end = start.AddMonths(1);
                            end = end.AddTicks(-2);
                            var DeptCode = Center[j].HeadId;
                            var RecQry = (from a in context.tblReceipt
                                          join b in context.tblProject on a.ProjectId equals b.ProjectId
                                          join c in context.vwFacultyStaffDetails on b.PIName equals c.UserId
                                          where !(b.ConsultancyFundingCategory == 9) && !(b.ConsultancyFundingCategory == 12) && a.Posted_f == true
                                        && !Categ.Contains(a.CategoryId ?? 0) && a.CrtdTS >= start && a.CrtdTS <= end
                                          && b.ProjectType == 2 && a.Status == "Completed" && !a.ReceiptNumber.Contains("RBU")
                                       && b.FacultyDetailId == DeptCode
                                       && ReceiptCateg.Contains(b.ReportClassification ?? 0)

                                          select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                            decimal Rec = RecQry.Select(mn => mn.ReceiptAmount).Sum() ?? 0;
                            Amt = (Rec) / 100000;
                            TtlAmt += Amt;

                            decimal ExRec = (RecQry.Sum(mn =>
                            {
                                return mn.ReceiptAmount - ((mn.CGST ?? 0) + (mn.SGST ?? 0) + (mn.IGST ?? 0));
                            }) / 100000) ?? 0;
                            ExAmt = ExRec;
                            ExTtlAmt += ExAmt;
                            ValuesList.Add(new ConsReceiptReportListModel()
                            {
                                values = String.Format("{0:0.00}", Amt)
                            });
                            ValuesList.Add(new ConsReceiptReportListModel()
                            {
                                values = String.Format("{0:0.00}", ExAmt)
                            });
                        }
                        ValuesList.Add(new ConsReceiptReportListModel()
                        {
                            values = String.Format("{0:0.00}", TtlAmt)
                        });
                        ValuesList.Add(new ConsReceiptReportListModel()
                        {
                            values = String.Format("{0:0.00}", ExTtlAmt)
                        });
                    }

                    for (int l = 0; l <= NoOfMonth; l++)
                    {
                        start = startDate.AddMonths(l);
                        end = start.AddMonths(1);
                        end = end.AddTicks(-2);
                        Monlist.Add(new ConsMonthListReportModel()
                        {
                            Month = String.Format("{0:MMM yyyy}", start)
                        });
                    }
                    Monlist.Add(new ConsMonthListReportModel()
                    {
                        Month = "Total Grant Received"
                    });
                    model.List = ValuesList;
                    model.MonthList = Monlist;

                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }

        #endregion     
        public static List<ReceiptReportBasedonScheme> GetSchemeReportDetails(string Month)
        {
            List<ReceiptReportBasedonScheme> model = new List<ReceiptReportBasedonScheme>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    FinOp fac = new FinOp(System.DateTime.Now);
                    var fromdate = fac.GetMonthFirstDate(Month);
                    var todate = fac.GetMonthLastDate(Month);
                    todate = todate.AddDays(1).AddTicks(-2);
                    DateTime startDate; DateTime endDate;
                    if (fromdate.Month > 3)
                    {
                        startDate = new DateTime(fromdate.Year, 4, 1);
                        endDate = fromdate.AddTicks(-2);
                    }
                    else
                    {
                        startDate = new DateTime((fromdate.Year) - 1, 4, 1);
                        endDate = fromdate.AddTicks(-2);
                    }
                    var Qry = context.tblSchemes.Where(m => m.ProjectType == 2).ToList();
                    int[] Categ = { 1, 14, 13 };
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        var Scheme = Qry[i].SchemeName;
                        int SchemeId = Qry[i].SchemeId;
                        var RecQry = (from a in context.tblReceipt
                                      join b in context.tblProject on a.ProjectId equals b.ProjectId
                                      where b.ConsultancyFundingCategory == SchemeId && !b.ProjectNumber.Contains("NFSC")
                                      && !Categ.Contains(a.CategoryId ?? 0) && a.CrtdTS >= startDate && a.CrtdTS <= endDate && a.Status == "Completed" && a.Posted_f == true
                                      select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                        int PrevNoOfProject = RecQry.Select(m => m.ProjectNumber).Distinct().Count();
                        decimal PrevRec = RecQry.Select(mn => mn.ReceiptAmount).Sum() ?? 0;

                        decimal PrevValue = (PrevRec) / 100000;
                        var CurrRecQry = (from a in context.tblReceipt
                                          join b in context.tblProject on a.ProjectId equals b.ProjectId
                                          where b.ConsultancyFundingCategory == SchemeId && !b.ProjectNumber.Contains("NFSC")
                                          && !Categ.Contains(a.CategoryId ?? 0) && a.CrtdTS >= fromdate && a.CrtdTS <= todate && a.Status == "Completed" && a.Posted_f == true
                                          select new { a.ReceiptAmount, a.ReceiptOverheadValue, a.IGST, a.SGST, a.CGST, b.ProjectNumber }).ToList();
                        int CurrNoOfProject = CurrRecQry.Select(m => m.ProjectNumber).Distinct().Count();
                        decimal CurrRec = CurrRecQry.Select(mn => mn.ReceiptAmount).Sum() ?? 0;

                        decimal CurrValue = (CurrRec) / 100000;
                        model.Add(new ReceiptReportBasedonScheme()
                        {
                            Name = Scheme,
                            PrevNoOfProject = PrevNoOfProject,
                            PrevValue = String.Format("{0:0.00}", PrevValue),
                            CurrNoOfProject = CurrNoOfProject,
                            CurrValue = String.Format("{0:0.00}", CurrValue)
                        });
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }

        #region InterestRefund
        public static InterestRefundReportModel GetInterestRefundMonth(int finYearId, int projectId)
        {
            InterestRefundReportModel interst = new InterestRefundReportModel();
            List<InterestRefundMonthReport> month = new List<InterestRefundMonthReport>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    //context.Database.CommandTimeout = 1800;//Increase timeout
                    var finQuery = context.tblFinYear.Where(m => m.FinYearId == finYearId).FirstOrDefault();
                    if (finQuery == null) return interst;
                    interst.InterestPct = finQuery.InterestPct.GetValueOrDefault(0);
                    interst.CommitmentIncl_f = finQuery.CommitmentInclForInterest_f.GetValueOrDefault(false);
                    bool considerCommit = finQuery.CommitmentInclForInterest_f ?? false;
                    DateTime fromdate = finQuery.StartDate ?? DateTime.Now;
                    DateTime EndDate; DateTime StartDate; DateTime CurMonth;
                    DateTime CurrntDate = DateTime.Now;
                    decimal TotalInterest = 0;
                    CurMonth = new DateTime(CurrntDate.Year, CurrntDate.Month, 1);
                    var Project = context.tblProject.Where(p => p.ProjectId == projectId).Select(p => p.ProjectNumber).FirstOrDefault();
                    var finyear = finQuery.Year;
                    if (!considerCommit && interst.InterestPct > 0)
                    {
                        decimal Ob = 0, monthOpeningBal = 0;
                        Ob = context.tblProjectOB.Where(m => m.ProjectId == projectId).Sum(m => m.OpeningBalance) ?? 0;
                        for (int i = 0; i < 12; i++)
                        {
                            StartDate = fromdate.AddMonths(i);
                            EndDate = StartDate.AddMonths(1).AddTicks(-2);


                            if (StartDate < CurMonth)
                            {

                                var PreRecp = (from C in context.tblReceipt where C.ProjectId == projectId && C.Posted_f == true && C.ReceiptDate < StartDate && C.Status == "Completed" select C).ToList();
                                decimal mPreRecp = PreRecp.Sum(m => m.ReceiptAmount ?? 0);
                                decimal mpreCGST = PreRecp.Sum(m => m.CGST ?? 0);
                                decimal mpreSGST = PreRecp.Sum(m => m.SGST ?? 0);
                                decimal mpreIGST = PreRecp.Sum(m => m.IGST ?? 0);
                                decimal mpreGST = mpreCGST + mpreSGST + mpreIGST;
                                mPreRecp = (mPreRecp) - (mpreGST);
                                decimal mPreExp = context.vw_ProjectExpenditureReport.AsNoTracking().Where(PE => PE.ProjectId == projectId && PE.CommitmentDate < StartDate).Sum(PE => PE.AmountSpent) ?? 0;

                                monthOpeningBal = Ob + mPreRecp - mPreExp;
                                decimal mComAvail = context.tblCommitment.Where(C => C.ProjectId == projectId && C.CRTD_TS >= StartDate && C.CRTD_TS <= EndDate && C.Status == "Active").Sum(C => C.CommitmentBalance) ?? 0;
                                decimal mExp = (from exp in context.vw_ProjectExpenditureReport.AsNoTracking()
                                                where exp.ProjectId == projectId && exp.CommitmentDate >= StartDate && exp.CommitmentDate <= EndDate
                                                select exp.AmountSpent).Sum() ?? 0;

                                var QryReceipt = (from C in context.tblReceipt where C.ProjectId == projectId && C.Posted_f == true && C.ReceiptDate >= StartDate && C.ReceiptDate <= EndDate && C.Status == "Completed" select C).ToList();
                                decimal receiptAmt = QryReceipt.Sum(m => m.ReceiptAmount ?? 0);
                                decimal CGST = QryReceipt.Sum(m => m.CGST ?? 0);
                                decimal SGST = QryReceipt.Sum(m => m.SGST ?? 0);
                                decimal IGST = QryReceipt.Sum(m => m.IGST ?? 0);
                                decimal GST = CGST + SGST + IGST;
                                receiptAmt = (receiptAmt) - (GST);

                                decimal Avlbal = monthOpeningBal + receiptAmt - mExp;
                                decimal AMI = monthOpeningBal - mExp;
                                decimal Intest = interst.InterestPct / 100;
                                decimal IRPA = (AMI * Intest) / 12;
                                //if (IRPA>0)
                                //{
                                //    TotalInterest = TotalInterest + IRPA;
                                //}
                                if (IRPA < 0)
                                {
                                    var mcnRecp = (from C in context.tblReceipt where C.ProjectId == projectId && C.Posted_f == true && C.ReceiptDate <= EndDate && C.Status == "Completed" select C).ToList();
                                    decimal mCRecp = mcnRecp.Sum(m => m.ReceiptAmount ?? 0);
                                    decimal mcnCGST = mcnRecp.Sum(m => m.CGST ?? 0);
                                    decimal mcnSGST = mcnRecp.Sum(m => m.SGST ?? 0);
                                    decimal mcnIGST = mcnRecp.Sum(m => m.IGST ?? 0);
                                    decimal mcnGST = mcnCGST + mcnSGST + mcnIGST;
                                    mCRecp = (mCRecp) - (mcnGST);
                                    decimal mCExp = context.vw_ProjectExpenditureReport.AsNoTracking().Where(PE => PE.ProjectId == projectId && PE.CommitmentDate <= EndDate).Sum(PE => PE.AmountSpent) ?? 0;

                                    decimal mCb = Ob + mCRecp - mCExp;
                                    if (mCb < 0)
                                        TotalInterest = TotalInterest + IRPA;
                                    else
                                    {
                                        IRPA = 0;
                                        AMI = 0;
                                    }
                                }
                                else
                                {
                                    TotalInterest = TotalInterest + IRPA;
                                }
                                month.Add(new InterestRefundMonthReport()
                                {
                                    SNo = i + 1,
                                    Month = string.Format("{0:MMM-yyyy}", StartDate),
                                    OpeningBalance = monthOpeningBal,
                                    ReceiptReceived = receiptAmt,
                                    ExpenditureTotal = mExp,
                                    CommitmentTotal = mComAvail,
                                    ActualBalance = Avlbal,
                                    ReceiptNotConsideredforinterest = receiptAmt,
                                    AmountEligibleforInterest = AMI,
                                    InterestAmount = Convert.ToDecimal(IRPA.ToString("0.00"))

                                });
                            }
                        }
                        interst.TotalIntrestAMT = Convert.ToDecimal(TotalInterest.ToString("0.00"));
                        interst.ProjectNumber = Project;
                        interst.FinancialYear = finyear;
                        interst.Monthlist = month;
                    }
                    else if (interst.InterestPct > 0)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            decimal qryOpeningBal = context.tblProjectOB.Where(m => m.ProjectId == projectId).Sum(m => m.OpeningBalance) ?? 0;
                            StartDate = fromdate.AddMonths(i);
                            EndDate = StartDate.AddMonths(1).AddTicks(-2);


                            if (StartDate < CurMonth)
                            {

                                var PreRecp = (from C in context.tblReceipt where C.ProjectId == projectId && C.Posted_f == true && C.CrtdTS < StartDate && C.Status == "Completed" select C).ToList();
                                decimal mPreRecp = PreRecp.Sum(m => m.ReceiptAmount ?? 0);
                                decimal mpreCGST = PreRecp.Sum(m => m.CGST ?? 0);
                                decimal mpreSGST = PreRecp.Sum(m => m.SGST ?? 0);
                                decimal mpreIGST = PreRecp.Sum(m => m.IGST ?? 0);
                                decimal mpreGST = mpreCGST + mpreSGST + mpreIGST;
                                mPreRecp = (mPreRecp) - (mpreGST);



                                decimal mPreComAvail = context.tblCommitment.Where(C => C.ProjectId == projectId && C.CRTD_TS < StartDate && C.Status == "Active").Sum(C => C.CommitmentBalance) ?? 0;

                                decimal mPreExp = context.vw_ProjectExpenditureReport.AsNoTracking().Where(PE => PE.ProjectId == projectId && PE.CommitmentDate < StartDate).Sum(PE => PE.AmountSpent) ?? 0;
                                decimal mPreComAvailExp = (from c in context.tblCommitment
                                                           join exp in context.vw_ProjectExpenditureReport.AsNoTracking() on c.CommitmentId equals exp.CommitmentId
                                                           where c.ProjectId == projectId && c.CRTD_TS < StartDate && exp.CommitmentDate >= StartDate && c.Status != "InActive"
                                                           select exp.AmountSpent).Sum() ?? 0;
                                decimal preoldAmount = (from c in context.tblCommitment
                                                        join cd in context.tblCommitmentDetails on c.CommitmentId equals cd.CommitmentId
                                                        join cc in context.tblCommitmentLog on cd.ComitmentDetailId equals cc.CommitmentDetailID
                                                        where c.ProjectId == projectId && c.CRTD_TS < StartDate && cc.CRTD_TS >= StartDate && cc.IsClosed == true && c.Status != "InActive"
                                                        select cc.OldAmount).Sum() ?? 0;
                                decimal preNewAmount = (from c in context.tblCommitment
                                                        join cd in context.tblCommitmentDetails on c.CommitmentId equals cd.CommitmentId
                                                        join cc in context.tblCommitmentLog on cd.ComitmentDetailId equals cc.CommitmentDetailID
                                                        where c.ProjectId == projectId && c.CRTD_TS < StartDate && cc.CRTD_TS >= StartDate && cc.IsClosed == true && c.Status != "InActive"
                                                        select cc.NewAmount).Sum() ?? 0;
                                decimal mPreComClosed = preoldAmount - preNewAmount;
                                mPreComAvail = mPreComAvail + mPreComAvailExp + mPreComClosed;
                                qryOpeningBal = qryOpeningBal + mPreRecp - mPreExp - mPreComAvail;
                                //qryOpeningBal = (qryOpeningBal + (mPreRecp - (mPreExp + mPreComAvail)));
                                decimal OpeningBal = (qryOpeningBal + (mPreRecp - (mPreExp + mPreComAvail)));
                                decimal mComAvail = context.tblCommitment.Where(C => C.ProjectId == projectId && C.CRTD_TS >= StartDate && C.CRTD_TS <= EndDate && C.Status == "Active").Sum(C => C.CommitmentBalance) ?? 0;
                                decimal mExp = (from exp in context.vw_ProjectExpenditureReport.AsNoTracking()
                                                join c in context.tblCommitment on exp.CommitmentId equals c.CommitmentId
                                                where exp.ProjectId == projectId && exp.CommitmentDate >= StartDate && exp.CommitmentDate <= EndDate
                                                && c.CRTD_TS >= StartDate
                                                select exp.AmountSpent).Sum() ?? 0;
                                decimal mComAvailExp = (from c in context.tblCommitment
                                                        join exp in context.vw_ProjectExpenditureReport.AsNoTracking() on c.CommitmentId equals exp.CommitmentId
                                                        where c.ProjectId == projectId && c.CRTD_TS >= StartDate && c.CRTD_TS <= EndDate && exp.CommitmentDate > EndDate && c.Status != "InActive"
                                                        select exp.AmountSpent).Sum() ?? 0;
                                decimal oldAmount = (from c in context.tblCommitment
                                                     join cd in context.tblCommitmentDetails on c.CommitmentId equals cd.CommitmentId
                                                     join cc in context.tblCommitmentLog on cd.ComitmentDetailId equals cc.CommitmentDetailID
                                                     where c.ProjectId == projectId && c.CRTD_TS >= StartDate && c.CRTD_TS <= EndDate && cc.CRTD_TS > EndDate && cc.IsClosed == true && c.Status != "InActive"
                                                     select cc.OldAmount).Sum() ?? 0;
                                decimal NewAmount = (from c in context.tblCommitment
                                                     join cd in context.tblCommitmentDetails on c.CommitmentId equals cd.CommitmentId
                                                     join cc in context.tblCommitmentLog on cd.ComitmentDetailId equals cc.CommitmentDetailID
                                                     where c.ProjectId == projectId && c.CRTD_TS >= StartDate && c.CRTD_TS <= EndDate && cc.CRTD_TS > EndDate && cc.IsClosed == true && c.Status != "InActive"
                                                     select cc.NewAmount).Sum() ?? 0;
                                decimal mComClosed = oldAmount - NewAmount;
                                mComAvail = mComAvail + mComAvailExp + mComClosed;
                                //qryOpeningBal = qryOpeningBal + mPreRecp - mPreExp - mPreComAvail;

                                var QryReceipt = (from C in context.tblReceipt where C.ProjectId == projectId && C.Posted_f == true && C.CrtdTS >= StartDate && C.CrtdTS <= EndDate && C.Status == "Completed" select C).ToList();
                                decimal receiptAmt = QryReceipt.Sum(m => m.ReceiptAmount ?? 0);
                                decimal CGST = QryReceipt.Sum(m => m.CGST ?? 0);
                                decimal SGST = QryReceipt.Sum(m => m.SGST ?? 0);
                                decimal IGST = QryReceipt.Sum(m => m.IGST ?? 0);
                                decimal GST = CGST + SGST + IGST;
                                receiptAmt = (receiptAmt) - (GST);


                                decimal Avlbal = (qryOpeningBal + receiptAmt) - (mExp + mComAvail);
                                //decimal Avlbal = qryOpeningBal - mExp - mComAvail;
                                decimal AMI = (Avlbal - receiptAmt);
                                decimal Intest = interst.InterestPct / 100;
                                //decimal IRPA = (Avlbal * Intest) / 12;
                                decimal IRPA = (AMI * Intest) / 12;
                                if (IRPA > 0)
                                {
                                    TotalInterest = TotalInterest + IRPA;
                                }
                                else
                                {
                                    var mcnRecp = (from C in context.tblReceipt where C.ProjectId == projectId && C.Posted_f == true && C.CrtdTS <= EndDate && C.Status == "Completed" select C).ToList();
                                    decimal mCRecp = mcnRecp.Sum(m => m.ReceiptAmount ?? 0);
                                    decimal mcnCGST = mcnRecp.Sum(m => m.CGST ?? 0);
                                    decimal mcnSGST = mcnRecp.Sum(m => m.SGST ?? 0);
                                    decimal mcnIGST = mcnRecp.Sum(m => m.IGST ?? 0);
                                    decimal mcnGST = mcnCGST + mcnSGST + mcnIGST;
                                    mCRecp = (mCRecp) - (mcnGST);
                                    decimal mCComAvail = context.tblCommitment.Where(C => C.ProjectId == projectId && C.CRTD_TS <= EndDate && C.Status == "Active").Sum(C => C.CommitmentBalance) ?? 0;
                                    decimal mCExp = context.vw_ProjectExpenditureReport.AsNoTracking().Where(PE => PE.ProjectId == projectId && PE.CommitmentDate <= EndDate).Sum(PE => PE.AmountSpent) ?? 0;
                                    decimal mCComAvailExp = (from c in context.tblCommitment
                                                             join exp in context.vw_ProjectExpenditureReport.AsNoTracking() on c.CommitmentId equals exp.CommitmentId
                                                             where c.ProjectId == projectId && c.CRTD_TS <= EndDate && exp.CommitmentDate > EndDate && c.Status != "InActive"
                                                             select exp.AmountSpent).Sum() ?? 0;
                                    decimal mcoldAmount = (from c in context.tblCommitment
                                                           join cd in context.tblCommitmentDetails on c.CommitmentId equals cd.CommitmentId
                                                           join cc in context.tblCommitmentLog on cd.ComitmentDetailId equals cc.CommitmentDetailID
                                                           where c.ProjectId == projectId && c.CRTD_TS <= EndDate && cc.CRTD_TS > EndDate && cc.IsClosed == true && c.Status != "InActive"
                                                           select cc.OldAmount).Sum() ?? 0;
                                    decimal mcNewAmount = (from c in context.tblCommitment
                                                           join cd in context.tblCommitmentDetails on c.CommitmentId equals cd.CommitmentId
                                                           join cc in context.tblCommitmentLog on cd.ComitmentDetailId equals cc.CommitmentDetailID
                                                           where c.ProjectId == projectId && c.CRTD_TS <= EndDate && cc.CRTD_TS > EndDate && cc.IsClosed == true && c.Status != "InActive"
                                                           select cc.NewAmount).Sum() ?? 0;
                                    decimal mCComClosed = mcoldAmount - mcNewAmount;
                                    mCComAvail = mCComAvail + mCComAvailExp + mCComClosed;
                                    decimal Ob = context.tblProjectOB.Where(m => m.ProjectId == projectId).Sum(m => m.OpeningBalance) ?? 0;
                                    decimal mCb = Ob + mCRecp - mCExp - mCComAvail;
                                    if (mCb < 0)
                                        TotalInterest = TotalInterest + IRPA;
                                }

                                //decimal expbal = mExp;
                                //decimal combal = mComAvail;
                                month.Add(new InterestRefundMonthReport()
                                {
                                    SNo = i + 1,
                                    Month = string.Format("{0:MMM-yyyy}", StartDate),
                                    OpeningBalance = qryOpeningBal,
                                    ReceiptReceived = receiptAmt,
                                    ExpenditureTotal = mExp,
                                    CommitmentTotal = mComAvail,
                                    ActualBalance = Avlbal,
                                    ReceiptNotConsideredforinterest = receiptAmt,
                                    AmountEligibleforInterest = AMI,
                                    InterestAmount = Convert.ToDecimal(IRPA.ToString("0.00"))

                                });
                            }
                        }
                        interst.TotalIntrestAMT = Convert.ToDecimal(TotalInterest.ToString("0.00"));
                        interst.ProjectNumber = Project;
                        interst.FinancialYear = finyear;
                        interst.Monthlist = month;
                    }
                }
                return interst;
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return interst;
            }
        }
        public static DataTable GetInterestRefundYear(int finYearId)
        {
            DataTable dt = new DataTable("Interest");
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var finQuery = context.tblFinYear.Where(m => m.FinYearId == finYearId).FirstOrDefault();
                    if (finQuery != null)
                    {
                        DateTime EndDate = Convert.ToDateTime(finQuery.EndDate);
                        DateTime StartDate = Convert.ToDateTime(finQuery.StartDate);
                        string hd = "Interest earned as on " + String.Format("{0:MM-dd-yyyy}", EndDate);
                        dt.Columns.Add("Project No.", typeof(String));
                        dt.Columns.Add("Project Type", typeof(String));
                        dt.Columns.Add("Agency Name", typeof(String));
                        dt.Columns.Add("Agency Code", typeof(String));
                        dt.Columns.Add("Opening Balance", typeof(Decimal));
                        dt.Columns.Add("Project Close Date", typeof(DateTime));
                        dt.Columns.Add("Receipt", typeof(Decimal));
                        dt.Columns.Add("Expenditure", typeof(Decimal));
                        dt.Columns.Add("Commitment", typeof(Decimal));
                        dt.Columns.Add("Closing Balance", typeof(Decimal));
                        dt.Columns.Add(hd, typeof(Decimal));
                        dt.Columns.Add("Interest earned as on closing date of the Project", typeof(Decimal));
                        dt.Columns.Add("Interest amount to be refunded", typeof(Decimal));

                        var query = (from p in context.tblProject
                                     join a in context.tblAgencyMaster on p.SponsoringAgency equals a.AgencyId
                                     where p.TentativeCloseDate > StartDate && p.ProjectClassification != 14 && p.Status != "InActive"
                                     select new { p, a.AgencyName, a.AgencyCode }).ToList();
                        if (query.Count > 0)
                        {
                            foreach (var item in query)
                            {
                                int projectId = item.p.ProjectId;
                                decimal qryOpeningBal = context.tblProjectOB.Where(m => m.ProjectId == projectId).Sum(m => m.OpeningBalance) ?? 0;
                                var QryReceipt = (from C in context.tblReceipt where C.ProjectId == projectId && C.Posted_f == true && C.CrtdTS >= StartDate && C.CrtdTS <= EndDate && C.Status == "Completed" select C).ToList();
                                decimal receiptAmt = QryReceipt.Sum(m => m.ReceiptAmount ?? 0);
                                decimal CGST = QryReceipt.Sum(m => m.CGST ?? 0);
                                decimal SGST = QryReceipt.Sum(m => m.SGST ?? 0);
                                decimal IGST = QryReceipt.Sum(m => m.IGST ?? 0);
                                decimal GST = CGST + SGST + IGST;
                                receiptAmt = (receiptAmt) - (GST);
                                decimal spentAmt = (from C in context.vw_ProjectExpenditureReport.AsNoTracking() where C.ProjectId == projectId && C.CommitmentDate >= StartDate && C.CommitmentDate <= EndDate select C.AmountSpent).Sum() ?? 0;
                                var OBQryReceipt = (from C in context.tblReceipt where C.ProjectId == projectId && C.Posted_f == true && C.CrtdTS < StartDate && C.Status == "Completed" select C).ToList();
                                decimal OBreceiptAmt = OBQryReceipt.Sum(m => m.ReceiptAmount ?? 0);
                                decimal OBCGST = OBQryReceipt.Sum(m => m.CGST ?? 0);
                                decimal OBSGST = OBQryReceipt.Sum(m => m.SGST ?? 0);
                                decimal OBIGST = OBQryReceipt.Sum(m => m.IGST ?? 0);
                                decimal OBGST = OBCGST + OBSGST + OBIGST;
                                OBreceiptAmt = (OBreceiptAmt) - (OBGST);
                                decimal OBspentAmt = (from C in context.vw_ProjectExpenditureReport.AsNoTracking() where C.ProjectId == projectId && C.CommitmentDate < StartDate select C.AmountSpent).Sum() ?? 0;
                                var OBcombalspent = (from c in context.tblCommitment
                                                     join cc in context.tblCommitmentDetails on c.CommitmentId equals cc.CommitmentId
                                                     where (c.CRTD_TS < StartDate && c.ProjectId == projectId)
                                                     select cc.BalanceAmount).Sum() ?? 0;
                                decimal OpeningBal = qryOpeningBal + (OBreceiptAmt - (OBspentAmt + OBcombalspent));
                                decimal commitmentbal = (from c in context.tblCommitment
                                                         join cc in context.tblCommitmentDetails on c.CommitmentId equals cc.CommitmentId
                                                         where (c.CRTD_TS >= StartDate && c.CRTD_TS <= EndDate && c.ProjectId == projectId)
                                                         select cc.BalanceAmount).Sum() ?? 0;
                                decimal Avlbal = (OpeningBal + receiptAmt) - (spentAmt + commitmentbal);
                                decimal AMI = (Avlbal - receiptAmt);
                                decimal Intest = Convert.ToDecimal(0.035);
                                decimal IRPA = AMI * Intest;
                                decimal IRCD = IRPA;
                                DateTime pCloseDate = Common.GetProjectCloseDate(projectId);
                                if (pCloseDate < EndDate)
                                {
                                    int noOfDays = (pCloseDate - StartDate).Days;
                                    IRCD = IRPA / 365 * noOfDays;
                                }
                                string pType = Common.getprojectTypeName(item.p.ProjectType ?? 0);
                                if (item.p.ProjectType == 1 && item.p.ProjectSubType != 1)
                                    pType += item.p.SponProjectCategory == "1" ? "-PFMS" : item.p.SponProjectCategory == "2" ? "-NON-PFMS" : "";
                                else if (item.p.ProjectType == 1 && item.p.ProjectSubType == 1)
                                    pType += " - Internal";
                                var row = dt.NewRow();
                                row["Project No."] = item.p.ProjectNumber;
                                row["Project Type"] = pType;
                                row["Agency Name"] = item.AgencyName;
                                row["Agency Code"] = item.AgencyCode;
                                row["Opening Balance"] = OpeningBal;
                                row["Project Close Date"] = pCloseDate;
                                row["Receipt"] = receiptAmt;
                                row["Expenditure"] = spentAmt;
                                row["Commitment"] = commitmentbal;
                                row["Closing Balance"] = OpeningBal + receiptAmt - spentAmt - commitmentbal;
                                row[hd] = IRPA;
                                row["Interest earned as on closing date of the Project"] = IRCD;
                                row["Interest amount to be refunded"] = IRCD < IRPA ? IRCD : IRPA;
                                dt.Rows.Add(row);
                            }
                        }
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                return dt;
            }
        }
        #endregion

        #region HeadCredit Print
        public TravelBillReportModel GetHeadCreditPrintDetails(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblHeadCredit.Where(m => m.HeadCreditId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CrtdTS);
                    var ProjectQry = context.tblProject.Where(m => m.ProjectId == Qry.ProjectId).FirstOrDefault();
                    model.RefNumber = Qry.ReferenceNumber;
                    model.PONumber = Qry.BillReferenceNumber;
                    model.ModeOfSupply = Common.GetCodeControlName(Qry.CreditMode ?? 0, "ModeofCredit");
                    model.CheqDate = String.Format("{0:dd-MMMM-yyyy}", Qry.ReferenceDate);
                    model.Remarks = Qry.Description;
                    model.ProjectNumber = ProjectQry.ProjectNumber;
                    model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == Qry.BankAccountHeadDr).Select(m => m.AccountHead).FirstOrDefault();
                    model.TotalAmount = Qry.BankAmountDr ?? 0;
                    model.BillId = Id;
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CrtdTS);
                    model.BillNumber = Qry.HeadCreditNumber;
                    model.BillType = "Head Credit";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CrtdTS);
                    model.PayeeLeadger = (from f in context.tblHeadCreditExpenseDetails
                                          join e in context.tblAccountHead on f.AccountHeadId equals e.AccountHeadId
                                          where f.HeadCreditId == Id
                                          select new
                                          {
                                              f.Amount,
                                              f.TransactionType,
                                              f.Name,
                                              e.AccountHead,

                                          }).AsEnumerable()
                                          .Select((x) => new HeadCreditPayeeLedgerModel()
                                          {
                                              PayeeName = x.Name,
                                              Amount = x.Amount ?? 0,
                                              TransactionType = x.TransactionType,
                                              AccountHead = x.AccountHead
                                          }).ToList();

                    model.Comm = (from e in context.tblHeadCreditCommitmentDetails
                                  join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.HeadCreditId == Id && e.PaymentAmount > 0
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.PaymentAmount,
                                      h.ProjectId
                                  }).AsEnumerable()
                              .Select((x) => new CommitListModel()
                              {
                                  Number = x.CommitmentNumber,
                                  ProjNo = x.ProjectNumber,
                                  ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                                  NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                                  Date = String.Format("{0:dd-MMMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                                  SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                                  Head = x.HeadName,
                                  Value = Convert.ToDecimal(x.PaymentAmount)
                              }).ToList();
                    var InvoiceQry = (from a in context.tblHeadCreditInvoiceDetails
                                      where a.HeadCreditId == Id
                                      select a).ToList();
                    List<InvoiceListModel> InvoiceList = new List<InvoiceListModel>();
                    if (InvoiceQry.Count > 0)
                    {
                        for (int i = 0; i < InvoiceQry.Count; i++)
                        {
                            InvoiceList.Add(new InvoiceListModel
                            {
                                InvoiceNo = InvoiceQry[i].ReversedInvoiceNumber,
                                GSTAmt = InvoiceQry[i].ReversedInvoiceGSTAmount ?? 0,
                                Amount = InvoiceQry[i].ReversedInvoiceAmount ?? 0
                            });
                        }
                    }
                    model.TDSGST = (from e in context.tblHeadCreditDetails
                                    join f in context.tblBudgetHead on e.BudgetHeadId equals f.BudgetHeadId
                                    where e.HeadCreditId == Id
                                    select new
                                    {
                                        e.Amount,
                                        f.HeadName
                                    }).AsEnumerable()
                                         .Select((x) => new TDSGSTForTravelModel()
                                         {
                                             Head = x.HeadName,
                                             Value = Convert.ToDecimal(x.Amount)
                                         }).ToList();
                    model.InvoiceList = InvoiceList;
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region Contraprint
        public static ContractorBillModel GetContractorBill(int ContractBillId)
        {
            ContractorBillModel dtls = new ContractorBillModel();
            List<ContractorBillDetailsModel> list = new List<ContractorBillDetailsModel>();
            try
            {

                using (var context = new IOASDBEntities())
                {
                    var QryCon = (from D in context.tblContractorBill
                                  where (D.ContractorBillId == ContractBillId)
                                  select new { D }).FirstOrDefault();
                    var QryConDetail = (from CD in context.tblContractorBillDetails
                                        where (CD.ContractorBillId == ContractBillId)
                                        select new { CD }).ToList();
                    decimal TatCost = context.tblContractorBillDetails.Where(x => x.ContractorBillId == ContractBillId).Sum(x => x.TotalCost ?? 0);
                    if (QryCon != null)
                    {
                        CultureInfo Indian = new CultureInfo("hi-IN");
                        dtls.ProjectNumber = Common.getprojectnumber(QryCon.D.ProjectId ?? 0);
                        dtls.HeadOfAccounts = QryCon.D.HeadOfAccounts;
                        dtls.AgreementNo = QryCon.D.AgreementNo;
                        if (QryCon.D.FromDate != null)
                            dtls.FDate = String.Format("{0:dd/MM/yyyy}", QryCon.D.FromDate);
                        if (QryCon.D.ToDate != null)
                            dtls.TDate = String.Format("{0:dd/MM/yyyy}", QryCon.D.ToDate);
                        dtls.Address = QryCon.D.Address;
                        if (QryCon.D.FirmDate != null)
                            dtls.FirDate = String.Format("{0:dd/MM/yyyy}", QryCon.D.FirmDate);
                        if (QryCon.D.IssuedDate != null)
                            dtls.IssuDate = String.Format("{0:dd/MMM/yyyy}", QryCon.D.IssuedDate);
                        dtls.RefNo = QryCon.D.RefNo;
                        dtls.DocDetail = QryCon.D.FinalDocuments;
                        dtls.ContractorBillId = QryCon.D.ContractorBillId;
                        dtls.Totalcostwords = CoreAccountsService.words(TatCost);
                        dtls.CurrentDate = String.Format("{0:dd/MM/yyyy}", QryCon.D.CRTD_TS ?? DateTime.Now);
                        if (QryConDetail.Count > 0)
                        {
                            for (int i = 0; i < QryConDetail.Count; i++)
                            {
                                list.Add(new ContractorBillDetailsModel
                                {

                                    ContractorBillDetailsId = QryConDetail[i].CD.ContractorBillDetailsId,
                                    ContractorBillId = QryConDetail[i].CD.ContractorBillId,
                                    SupportingVocherNo = QryConDetail[i].CD.SupportingVocherNo,
                                    VoucherDate = QryConDetail[i].CD.VoucherDate,
                                    vocDate = String.Format("{0:dd/MM/yyyy}", QryConDetail[i].CD.VoucherDate ?? DateTime.Now),
                                    Unit = QryConDetail[i].CD.Unit,
                                    Description = QryConDetail[i].CD.Description,
                                    Quantity = QryConDetail[i].CD.Quantity,
                                    Rate = QryConDetail[i].CD.Rate,
                                    TotalCost = QryConDetail[i].CD.TotalCost,
                                    DetailRemarks = QryConDetail[i].CD.Remarks,
                                    Amount = String.Format(Indian, "{0:N}", QryConDetail[i].CD.TotalCost)
                                });
                            }

                        }
                        dtls.BillDetails = list;
                    }
                }
                
                return dtls;
            }
            catch (Exception ex)
            {

                return dtls;
            }
        }
        #endregion

        #region ContigentBill
        public static ContingentBillModel GetContingentBill(int ContingentId)
        {
            ContingentBillModel dtls = new ContingentBillModel();
            try
            {

                using (var context = new IOASDBEntities())
                {
                    var QryCon = (from D in context.tblContingentBill
                                  where (D.ConingentBillId == ContingentId)
                                  select D).FirstOrDefault();

                    if (QryCon != null)
                    {
                        CultureInfo Indian = new CultureInfo("hi-IN");
                        dtls.ProjectNumber = Common.getprojectnumber(QryCon.ProjectId ?? 0);
                        dtls.AllotmentAmount = QryCon.AllotmentAmount;
                        dtls.ExpendedAmount = QryCon.ExpendedAmount;
                        dtls.VoucherNo = QryCon.VoucherNo;
                        dtls.BalanceAllotmentAmount = QryCon.BalanceAllotmentAmount;
                        dtls.ExpenditureAccount = QryCon.ExpenditureAccount;
                        dtls.AuthorityNo = QryCon.AuthorityNo;
                        dtls.AuthrityDate = String.Format("{0:dd.MM.yyyy}", QryCon.AuthorityDate ?? DateTime.Now);
                        dtls.PayDate = String.Format("{0:dd.MM.yyyy}", QryCon.PaymentDate ?? DateTime.Now);
                        dtls.CurrentDate = String.Format("{0:dd.MM.yyyy}", QryCon.CRTD_TS ?? DateTime.Now);
                        dtls.ProjectTittle = QryCon.ProjectTittle;
                        dtls.ProjectCordinator = QryCon.ProjectCordinator;
                        dtls.DeptofEngg = QryCon.DeptOfEngg;
                        dtls.RefNo = QryCon.RefNo;
                        dtls.DocDetail = QryCon.FinalAttachedDoc;
                        dtls.ConingentBillId = QryCon.ConingentBillId;
                        dtls.NoOfQuantity = QryCon.NoofQuantity ?? 0;
                        dtls.Rate = QryCon.Rate ?? 0;
                        dtls.per = Convert.ToDecimal(QryCon.Per);
                        dtls.PrjPayment = QryCon.ProjectPayment ?? 0;
                        dtls.Rate = QryCon.Rate ?? 0;
                        if (QryCon.AdvRecievedDate != null)
                            dtls.AdvRecDate = String.Format("{0:dd.MM.yyyy}", QryCon.AdvRecievedDate);
                        dtls.AdvReceivedFrom = QryCon.AdvReceviedFrom;
                        dtls.ReceivedPayment = QryCon.ReceivedFrom ?? 0;
                        decimal tatamt = QryCon.NetAmount ?? 0;
                        dtls.AmountinWords = CoreAccountsService.words(tatamt);
                        dtls.Amounts = String.Format(Indian, "{0:N}", tatamt);
                        dtls.NetAmount = QryCon.NetAmount ?? 0;
                        dtls.Milestone = QryCon.Milestone;
                        dtls.Duringyear = QryCon.Duringyear;
                    }
                }
                return dtls;
            }
            catch (Exception ex)
            {
                return dtls;
            }
        }
        #endregion

        #region Financial report
        public ProjectSummaryModel getProjectSummary(int ProjectId, DateTime Date, IOASDBEntities cntx = null, DbTransaction tx = null)
        {
            try
            {
                ProjectSummaryModel prjModel = new ProjectSummaryModel();
                using (var context = cntx == null ? new IOASDBEntities() : cntx)
                {
                    if (tx != null)
                        context.Database.UseTransaction(tx);
                    var qryProject = (from prj in context.tblProject
                                      where prj.ProjectId == ProjectId
                                      select prj).FirstOrDefault();
                    var qryPreviousCommit = (from C in context.tblCommitment
                                             join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                             where C.ProjectId == ProjectId && C.Status == "Active" && C.CRTD_TS <= Date
                                             select new { D.BalanceAmount, D.ReversedAmount }).ToList();
                    var BalanceAmt = qryPreviousCommit.Select(m => m.BalanceAmount).Sum();
                    var ReversedAmount = 0;// qryPreviousCommit.Select(m => m.ReversedAmount).Sum();

                    /*Spent amount calculation Start*/
                    decimal ohAddition = 0, ohRever = 0;
                    var ohAR = context.tblOHReversal.Where(m => m.ProjectId == ProjectId && m.Status == "Approved" && m.CRTD_TS <= Date).ToList();
                    //ohAddition = ohAR.Where(m => m.Type == 1).Sum(m => m.TotalAmount) ?? 0;
                    ohRever = ohAR.Where(m => m.Type == 2).Sum(m => m.TotalAmount) ?? 0;
                    decimal? Debit = 0, Credit = 0, spentAmt = 0;
                    var qrySpenAmt = (from C in context.vwCommitmentSpentBalance where C.ProjectId == ProjectId && C.CRTD_TS <= Date select C.AmountSpent).Sum();
                    var queryOB = (from C in context.tblProjectOBDetail
                                   where C.ProjectId == ProjectId
                                   select C).ToList();
                    decimal OpExp = queryOB.Select(m => m.OpeningExp).Sum() ?? 0;
                    var queryRecOB = (from C in context.tblReceiptOB
                                      where C.ProjectId == ProjectId
                                      select C).ToList();
                    prjModel.PrevRec = queryRecOB.Select(m => m.ReceiptOpeningBalExclInterest).Sum() ?? 0;
                    prjModel.PrevInterest = queryRecOB.Select(m => m.InterestReceiptOpeningBal).Sum() ?? 0;
                    prjModel.PrevExp = OpExp;
                    //OpExp = 0;//OB Reset to Zero .To be Disscused with Praveen if the PrjOpeningBalance includes the Opening Expen
                    if (qrySpenAmt == null)
                        qrySpenAmt = 0;
                    spentAmt = qrySpenAmt;
                    var FundTransferDebit = (from C in context.tblProjectTransfer
                                             from D in context.tblProjectTransferDetails
                                             where C.ProjectTransferId == D.ProjectTransferId
                                             where C.DebitProjectId == ProjectId && C.Status == "Completed" && C.CRTD_TS <= Date
                                             select D).ToList();
                    if (FundTransferDebit.Count > 0)
                    {
                        Debit = FundTransferDebit.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum();
                        if (Debit != 0)
                            spentAmt = spentAmt + Debit;
                    }
                    var FundTransferCredit = (from C in context.tblProjectTransfer
                                              from D in context.tblProjectTransferDetails
                                              where C.ProjectTransferId == D.ProjectTransferId
                                              where C.CreditProjectId == ProjectId && C.Status == "Completed" && C.CRTD_TS <= Date
                                              select D).ToList();
                    if (FundTransferCredit.Count > 0)
                    {
                        Credit = FundTransferCredit.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum();
                        if (Credit != 0)
                            spentAmt = spentAmt - Credit;
                    }
                    /*claim amount institute start*/
                    var qryInstituteClaim = (from Neg in context.tblInstituteClaims
                                             where Neg.ProjectId == ProjectId && Neg.Status == "Completed"
                                             select Neg).ToList();
                    var claimAmt = qryInstituteClaim.Select(m => m.ClaimAmount).Sum();
                    spentAmt = spentAmt - claimAmt;
                    /*claim amount institute end*/
                    /*Spent amount calculation End*/
                    var AvailableCommitment = BalanceAmt + ReversedAmount;
                    var QryReceipt = (from C in context.tblReceipt where C.ProjectId == ProjectId && C.Posted_f == true && C.CrtdTS <= Date && C.Status == "Completed" select C).ToList();
                    decimal interestRec = QryReceipt.Where(m => m.CategoryId == 16).Sum(m => m.ReceiptAmount) ?? 0;
                    decimal receiptAmt = QryReceipt.Sum(m => m.ReceiptAmount) ?? 0;
                    decimal OverHead = QryReceipt.Sum(m => m.ReceiptOverheadValue) ?? 0;
                    decimal CGST = QryReceipt.Sum(m => m.CGST) ?? 0;
                    decimal SGST = QryReceipt.Sum(m => m.SGST) ?? 0;
                    decimal IGST = QryReceipt.Sum(m => m.IGST) ?? 0;
                    decimal GST = CGST + SGST + IGST;
                    receiptAmt = receiptAmt - GST;
                    decimal OverheadsOBExp = (from C in context.tblProjectOBDetail
                                              where C.ProjectId == ProjectId && C.HeadId == 6
                                              select C).Sum(m => m.OpeningExp) ?? 0;
                    decimal ExpoverheadAmt = context.vw_ProjectExpenditureReport.AsNoTracking().Where(m => m.ProjectId == ProjectId && m.CommitmentDate <= Date && m.AllocationHeadId == 6).Sum(m => m.AmountSpent) ?? 0;
                    decimal ttlOverHead = OverheadsOBExp + ExpoverheadAmt;
                    /* Negative balance taking query*/
                    var qryNegativeBal = (from Neg in context.tblNegativeBalance
                                          where Neg.ProjectId == ProjectId && Neg.Status == "Approved" && Neg.CRTD_TS <= Date
                                          select Neg.NegativeBalanceAmount).Sum();
                    /* Negative balance taking query end*/
                    /* Opening balance taking query*/
                    decimal qryOpeningBal = (from OB in context.tblProjectOB
                                             where OB.ProjectId == ProjectId
                                             select OB).Sum(m => m.OpeningBalance) ?? 0;
                    /* Opening balance taking query end*/
                    if (qryProject != null)
                    {
                        string pType = Common.getprojectTypeName(qryProject.ProjectType ?? 0);
                        if (qryProject.ProjectType == 1 && qryProject.ProjectSubType != 1)
                            pType += qryProject.SponProjectCategory == "1" ? "-PFMS" : qryProject.SponProjectCategory == "2" ? "-NON-PFMS" : "";
                        else if (qryProject.ProjectType == 1 && qryProject.ProjectSubType == 1)
                            pType += " - Internal";
                        prjModel.ProjectTittle = qryProject.ProjectTitle;
                        prjModel.PIname = Common.GetPIName(qryProject.PIName ?? 0);
                        prjModel.SanctionedValue = qryProject.SanctionValue ?? 0;
                        prjModel.OpeningBalance = qryOpeningBal;
                        //sum(ReciptAmt-(GST+OverHeads ))
                        prjModel.TotalReceipt = receiptAmt - interestRec;// + qryOpeningBal;
                        prjModel.AfterInterest = interestRec;
                        prjModel.Tax = GST;
                        prjModel.OH = ttlOverHead + ohAddition - ohRever;
                        prjModel.AmountSpent = (spentAmt ?? 0) + OverHead + ohAddition - ohRever;
                        prjModel.PreviousCommitment = AvailableCommitment ?? 0;
                        //TotalReceipt - AmountSpent + PreviousCommitment
                        prjModel.AvailableBalance = ((qryOpeningBal + receiptAmt) - (prjModel.AmountSpent + prjModel.PreviousCommitment));
                        prjModel.FinancialYear = Common.GetFinYear(qryProject.FinancialYear ?? 0);
                        prjModel.SanctionOrderNo = qryProject.SanctionOrderNumber;
                        prjModel.SanctionOrderDate = String.Format("{0:ddd dd-MMM-yyyy}", qryProject.SanctionOrderDate);
                        prjModel.ProjectApprovalDate = string.Format("{0:ddd dd-MMM-yyyy}", qryProject.ProposalApprovedDate);
                        prjModel.StartDate = String.Format("{0:ddd dd-MMM-yyyy}", qryProject.TentativeStartDate);
                        prjModel.CloseDate = string.Format("{0:ddd dd-MMM-yyyy}", Common.GetProjectDueDate(ProjectId) ?? qryProject.TentativeCloseDate);
                        prjModel.ProjectDuration = prjModel.ProjectDuration;
                        prjModel.ProposalNo = qryProject.ProposalNumber;
                        prjModel.ProjectNo = qryProject.ProjectNumber;
                        prjModel.BaseValue = qryProject.BaseValue ?? 0;
                        prjModel.ProjectType = pType;
                        prjModel.ApplicableTax = qryProject.ApplicableTax ?? 0;
                        //var Data= Common.getProjectNo(qryProject.ProjectType ?? 0);
                        //prjModel.CommitNo = Data.Item2;
                        prjModel.ApprovedNegativeBalance = qryNegativeBal ?? 0;
                        prjModel.OverHeads = OverHead + ohAddition - ohRever;
                        prjModel.AllocationNR_f = qryProject.ProjectClassification != 1 ? true : qryProject.AllocationNR_f ?? false;
                        //sum(CGST+SGST+IGST) 
                        prjModel.ExpAmt = prjModel.AmountSpent;
                        prjModel.TotalGrantReceived = prjModel.PrevRec + prjModel.TotalReceipt;
                        prjModel.TotalExpenditure = prjModel.PrevExp + prjModel.ExpAmt;
                        prjModel.TotalInterest = prjModel.AfterInterest + prjModel.PrevInterest;
                        prjModel.AvailableBalanceinProject = (prjModel.TotalGrantReceived + prjModel.TotalInterest) - (prjModel.TotalExpenditure + prjModel.PreviousCommitment);
                        prjModel.NetBalance = (prjModel.AvailableBalanceinProject + prjModel.ApprovedNegativeBalance);
                        prjModel.GST = GST;
                    }

                    //taking total commitment amount headwise
                    var qryHeadCommit = (from C in context.tblCommitment
                                         join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                         where C.ProjectId == ProjectId && C.Status == "Active"
                                         select new { D.AllocationHeadId, D.Amount }).ToList();
                    List<HeadWiseDetailModel> List = new List<HeadWiseDetailModel>();
                    List<HeadWiseDetailModel> ListSpent = new List<HeadWiseDetailModel>();
                    List<HeadWiseDetailModel> Balance = new List<HeadWiseDetailModel>();
                    if (qryHeadCommit.Count > 0)
                    {
                        var distCount = qryHeadCommit.Select(m => m.AllocationHeadId).Distinct().ToArray();
                        for (int i = 0; i < distCount.Length; i++)
                        {
                            int headId = distCount[i] ?? 0;
                            var HeadName = Common.getAllocationHeadName(headId);
                            decimal amt = qryHeadCommit.Where(m => m.AllocationHeadId == headId).Sum(m => m.Amount) ?? 0;
                            List.Add(new HeadWiseDetailModel()
                            {
                                AllocationId = headId,
                                AllocationHeadName = HeadName,
                                Amount = amt
                            });
                        }

                    }

                    //taking total spent amount headwise
                    var qrySpent = (from C in context.vwCommitmentSpentBalance
                                    where C.ProjectId == ProjectId
                                    select new { C.AllocationHeadId, C.AmountSpent }).ToList();

                    if (qrySpent.Count > 0)
                    {
                        var distCount = qrySpent.Select(m => m.AllocationHeadId).Distinct().ToArray();
                        for (int i = 0; i < distCount.Length; i++)
                        {
                            int headId = distCount[i] ?? 0;
                            var HeadName = Common.getAllocationHeadName(headId);
                            decimal amt = qrySpent.Where(m => m.AllocationHeadId == headId).Sum(m => m.AmountSpent) ?? 0;
                            ListSpent.Add(new HeadWiseDetailModel()
                            {
                                AllocationId = headId,
                                AllocationHeadName = HeadName,
                                Amount = amt
                            });
                        }

                    }
                    //taking balance for future commitments
                    var qryAllocation = (from C in context.tblProjectAllocation
                                         where C.ProjectId == ProjectId
                                         select new { C.AllocationHead, C.AllocationValue }
                    ).ToList();

                    //var qryAllocation

                    if (qryAllocation.Count > 0)
                    {
                        decimal balance = 0;
                        for (int j = List.Count() - 1; j < qryAllocation.Count(); j++)
                        {
                            if (j + 1 == List.Count())
                            {
                                for (int k = 0; k < List.Count(); k++)
                                {
                                    decimal amt = qryAllocation[k].AllocationValue ?? 0;
                                    balance = amt - List[k].Amount;
                                    //if (qryInstituteClaim[k].ClaimAmount != null)
                                    //{
                                    //    balance = balance - qryInstituteClaim[k].ClaimAmount ?? 0;
                                    //}
                                    Balance.Add(new HeadWiseDetailModel()
                                    {
                                        AllocationId = qryAllocation[k].AllocationHead ?? 0,
                                        AllocationHeadName = List[k].AllocationHeadName,
                                        Amount = balance
                                    });
                                }
                            }
                            else
                            {
                                var HeadId = qryAllocation[j].AllocationHead ?? 0;
                                var HeadName = Common.getAllocationHeadName(HeadId);
                                Balance.Add(new HeadWiseDetailModel()
                                {
                                    AllocationId = HeadId,
                                    AllocationHeadName = HeadName,
                                    Amount = qryAllocation[j].AllocationValue ?? 0
                                });
                            }
                        }
                    }
                    prjModel.HeadWiseCommitment = List;
                    prjModel.HeadWiseSpent = ListSpent;
                    prjModel.HeadWiseAllocation = Balance;
                }
                return prjModel;
            }
            catch (Exception ex)
            {
                ProjectSummaryModel prjModel = new ProjectSummaryModel();
                return prjModel;
            }
        }
        public ProjectSummaryDetailModel getProjectSummaryDetails(int ProjectId, DateTime Date)
        {
            try
            {
                ProjectSummaryDetailModel model = new ProjectSummaryDetailModel();
                List<HeadWiseDetailModel> List = new List<HeadWiseDetailModel>();
                using (var context = new IOASDBEntities())
                {

                    var qryHead = (from C in context.tblCommitment
                                   join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                   where C.ProjectId == ProjectId && C.Status != "InActive" && C.CRTD_TS <= Date
                                   select new { AllocationHead = D.AllocationHeadId }).ToList();
                    var queryAlloc = (from C in context.tblProjectAllocation
                                      where C.ProjectId == ProjectId
                                      select C).OrderBy(c => c.AllocationId).ToList();
                    var queryPFT = (from C in context.tblProjectTransfer
                                    join D in context.tblProjectTransferDetails on C.ProjectTransferId equals D.ProjectTransferId
                                    where (C.DebitProjectId == ProjectId || C.CreditProjectId == ProjectId) && C.CRTD_TS <= Date && C.Status == "Completed"
                                    select new { AllocationHead = D.BudgetHeadId }).ToList();
                    var queryOpenExp = (from C in context.tblProjectOBDetail
                                        where C.ProjectId == ProjectId
                                        select new { AllocationHead = C.HeadId }).ToList();
                    var queryEnhAlloc = (from C in context.tblProjectEnhancementAllocation
                                         where C.ProjectId == ProjectId && C.Status == "Active" && C.CrtdTS <= Date
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
                    //TotalHead = TotalHead.Union(headname).ToArray();
                    //TotalHead = TotalHead.Union()
                    //decimal OverHead = (from C in context.tblReceipt
                    //                    where C.ProjectId == ProjectId
                    //                    && C.Status == "Completed"
                    //                    select C).Sum(m => m.ReceiptOverheadValue) ?? 0;
                    if (TotalHead.Length > 0)
                    {
                        for (int i = 0; i < TotalHead.Length; i++)
                        {
                            decimal TotBudgetAmt = 0;
                            decimal OB = 0;
                            int headId = TotalHead[i] ?? 0;
                            // var HeadName = Common.getAllocationHeadName(headId);
                            var HeadName = context.tblBudgetHead.Where(m => m.BudgetHeadId == headId).Select(m => m.HeadName).FirstOrDefault();
                            decimal AllocatValue = queryAlloc.Where(m => m.AllocationHead == headId).Sum(m => m.AllocationValue) ?? 0;
                            decimal EnhanveValue = queryEnhAlloc.Where(m => m.AllocationHead == headId).Sum(m => m.EnhancedValue) ?? 0;
                            if (AllocatValue != 0)
                            {
                                TotBudgetAmt += AllocatValue;
                            }
                            if (EnhanveValue != 0)
                            {
                                TotBudgetAmt += EnhanveValue;
                            }


                            var qryHeadCommit = (from C in context.tblCommitment
                                                 join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                                 where C.ProjectId == ProjectId && C.CRTD_TS <= Date && D.AllocationHeadId == headId && C.Status == "Active"
                                                 select new { D.AllocationHeadId, D.Amount, C.CommitmentBalance, D.BalanceAmount }).ToList();
                            decimal BalComm = qryHeadCommit.Select(m => m.BalanceAmount).Sum() ?? 0;
                            var qryBalance = (from exp in context.vw_ProjectExpenditureReport.AsNoTracking()
                                              where exp.ProjectId == ProjectId && exp.CommitmentDate <= Date && exp.AllocationHeadId == headId
                                              select exp).ToList();
                            decimal NewAmount = qryBalance.Select(m => m.NewAmount).Sum() ?? 0;
                            decimal SpentAmount = qryBalance.Select(m => m.AmountSpent).Sum() ?? 0;
                            //var FundTransferDebit = (from C in context.tblProjectTransfer
                            //                         from D in context.tblProjectTransferDetails
                            //                         where C.ProjectTransferId == D.ProjectTransferId
                            //                         where C.DebitProjectId == ProjectId && C.Status == "Completed" && D.BudgetHeadId == headId
                            //                         && D.TransactionType == "Debit"
                            //                         select D).Sum(m => m.Amount) ?? 0;
                            //SpentAmount = SpentAmount + FundTransferDebit;
                            //var FundTransferCredit = (from C in context.tblProjectTransfer
                            //                          from D in context.tblProjectTransferDetails
                            //                          where C.ProjectTransferId == D.ProjectTransferId
                            //                          where C.CreditProjectId == ProjectId && C.Status == "Completed" && D.BudgetHeadId == headId
                            //                         && D.TransactionType == "Credit"
                            //                          select D).Sum(m => m.Amount) ?? 0;
                            //SpentAmount = SpentAmount - FundTransferCredit;
                            var queryOB = (from C in context.tblProjectOBDetail
                                           where C.ProjectId == ProjectId && C.HeadId == headId
                                           select C).FirstOrDefault();
                            if (queryOB != null)

                                OB = queryOB.OpeningExp ?? 0;
                            //  OB = 0;//OB Reset to Zero .To be Disscused with Praveen if the PrjOpeningBalance includes the Opening Expen
                            var OpExp = TotBudgetAmt - OB;
                            //if (headId == 6)
                            //{
                            //    //var ohAR = context.tblOHReversal.Where(m => m.ProjectId == ProjectId && m.Status == "Approved").ToList();
                            //    //decimal ohAddition = ohAR.Where(m => m.Type == 1).Sum(m => m.TotalAmount) ?? 0;
                            //    //decimal ohRever = ohAR.Where(m => m.Type == 2).Sum(m => m.TotalAmount) ?? 0;
                            //    SpentAmount = SpentAmount + OverHead;//  - ohRever;
                            //}

                            List.Add(new HeadWiseDetailModel()
                            {
                                AllocationId = headId,
                                AllocationHeadName = HeadName,
                                Amount = TotBudgetAmt,
                                BalanceAmount = BalComm,
                                Expenditure = SpentAmount + OB,
                                Total = BalComm + SpentAmount + OB,
                                Available = TotBudgetAmt - (BalComm + SpentAmount + OB)
                            });
                        }
                    }
                }
                model.HeadWise = List;
                model.PrjSummary = getProjectSummary(ProjectId, Date);
                return model;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #region  Year Financial report
        public ProjectSummaryDetailModel getProjectSummaryDetails(int ProjectId, DateTime FromDate, DateTime ToDate, int year)
        {
            try
            {
                ProjectSummaryDetailModel model = new ProjectSummaryDetailModel();
                List<HeadWiseDetailModel> List = new List<HeadWiseDetailModel>();
                using (var context = new IOASDBEntities())
                {
                    year = year + 1;
                    var qryHead = (from C in context.tblCommitment
                                   join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                   where C.ProjectId == ProjectId && C.Status != "InActive" && C.CRTD_TS >= FromDate && C.CRTD_TS <= ToDate
                                   select new { AllocationHead = D.AllocationHeadId }).ToList();
                    var queryAlloc = (from C in context.tblProjectAllocation
                                      where C.ProjectId == ProjectId
                                      select C).OrderBy(c => c.AllocationId).ToList();
                    var queryPFT = (from C in context.tblProjectTransfer
                                    join D in context.tblProjectTransferDetails on C.ProjectTransferId equals D.ProjectTransferId
                                    where (C.DebitProjectId == ProjectId || C.CreditProjectId == ProjectId) && C.CRTD_TS >= FromDate && C.CRTD_TS <= ToDate && C.Status == "Completed"
                                    select new { AllocationHead = D.BudgetHeadId }).ToList();
                    var queryOpenExp = (from C in context.tblProjectOBDetail
                                        where C.ProjectId == ProjectId
                                        select new { AllocationHead = C.HeadId }).ToList();
                    var queryEnhAlloc = (from C in context.tblProjectEnhancementAllocation
                                         where C.ProjectId == ProjectId && C.Status == "Active"
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
                        for (int i = 0; i < TotalHead.Length; i++)
                        {
                            decimal TotBudgetAmt = 0;
                            decimal OB = 0;
                            int headId = TotalHead[i] ?? 0;
                            decimal AllocatValue = 0; decimal EnhanveValue = 0;
                            var YearAlloc = (from C in context.tblProject
                                             where C.ProjectId == ProjectId && C.IsYearWiseAllocation == true
                                             select C).FirstOrDefault();

                            var HeadName = context.tblBudgetHead.Where(m => m.BudgetHeadId == headId).Select(m => m.HeadName).FirstOrDefault();
                            if (YearAlloc == null)
                            {
                                AllocatValue = queryAlloc.Where(m => m.AllocationHead == headId).Sum(m => m.AllocationValue) ?? 0;
                                EnhanveValue = queryEnhAlloc.Where(m => m.AllocationHead == headId).Sum(m => m.EnhancedValue) ?? 0;
                                if (AllocatValue != 0)
                                    TotBudgetAmt += AllocatValue;
                                if (EnhanveValue != 0)
                                    TotBudgetAmt += EnhanveValue;
                            }
                            else
                            {
                                var Datelist = Common.GetProjectExpYear(ProjectId);
                                int id = Datelist.OrderByDescending(m => m.Id).Select(m => m.Id).FirstOrDefault();
                                if (year == (id + 1))
                                    EnhanveValue = queryEnhAlloc.Where(m => m.AllocationHead == headId).Sum(m => m.EnhancedValue) ?? 0;
                                AllocatValue = queryAlloc.Where(m => m.AllocationHead == headId && m.Year == year).Sum(m => m.AllocationValue) ?? 0;
                                if (AllocatValue != 0)
                                    TotBudgetAmt += AllocatValue;
                                if (EnhanveValue != 0)
                                    TotBudgetAmt += EnhanveValue;
                            }


                            var qryHeadCommit = (from C in context.tblCommitment
                                                 join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                                 where C.ProjectId == ProjectId && C.CRTD_TS >= FromDate && C.CRTD_TS <= ToDate && D.AllocationHeadId == headId && C.Status == "Active"
                                                 select new { D.AllocationHeadId, D.Amount, C.CommitmentBalance, D.BalanceAmount }).ToList();
                            decimal BalComm = qryHeadCommit.Select(m => m.BalanceAmount).Sum() ?? 0;
                            var qryBalance = (from exp in context.vw_ProjectExpenditureReport.AsNoTracking()
                                              where exp.ProjectId == ProjectId && exp.CommitmentDate >= FromDate && exp.CommitmentDate <= ToDate && exp.AllocationHeadId == headId
                                              select exp).ToList();
                            decimal NewAmount = qryBalance.Select(m => m.NewAmount).Sum() ?? 0;
                            decimal SpentAmount = qryBalance.Select(m => m.AmountSpent).Sum() ?? 0;

                            var queryOB = (from C in context.tblProjectOBDetail
                                           where C.ProjectId == ProjectId && C.HeadId == headId
                                           select C).FirstOrDefault();
                            if (queryOB != null)

                                OB = queryOB.OpeningExp ?? 0;
                            var OpExp = TotBudgetAmt - OB;

                            List.Add(new HeadWiseDetailModel()
                            {
                                AllocationId = headId,
                                AllocationHeadName = HeadName,
                                Amount = TotBudgetAmt,
                                BalanceAmount = BalComm,
                                Expenditure = SpentAmount + OB,
                                Total = BalComm + SpentAmount + OB,
                                Available = TotBudgetAmt - (BalComm + SpentAmount + OB)
                            });
                        }
                    }
                }
                model.HeadWise = List;

                return model;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
        #endregion
        public static InvoiceReportModel GetCreditnoteBillReport(int Id)
        {
            InvoiceReportModel model = new InvoiceReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var CreditnoteQry = context.tblCreditNote.Where(m => m.CreditNoteId == Id).FirstOrDefault();
                    var Qry = context.tblProjectInvoice.Where(m => m.InvoiceId == CreditnoteQry.InvoiceId).FirstOrDefault();

                    var QryAgency = context.tblAgencyMaster.Where(m => m.AgencyId == Qry.AgencyId).FirstOrDefault();

                    var QryInvoiceTax = context.tblInvoiceTaxDetails.Where(m => m.InvoiceId == Id).FirstOrDefault();
                    if (Qry != null)
                    {
                        model.CreditNoteNo = CreditnoteQry.CreditNoteNumber;
                        model.CreditNoteDate = string.Format("{0:dd-MMM-yyyy}", CreditnoteQry.CRTD_TS);
                        int AgencyId = Qry.AgencyId ?? 0;
                        int AgencyRegState = Qry.AgencyRegState ?? 0;
                        var QryState = context.tblStateMaster.Where(m => m.StateId == AgencyRegState).FirstOrDefault();
                        int InvoiceId = Qry.InvoiceId;
                        CultureInfo Indian = new CultureInfo("hi-IN");
                        model.InvoiceNo = Qry.InvoiceNumber;
                        model.InvoiceDate = string.Format("{0:dd-MMM-yyyy}", Qry.InvoiceDate);
                        int ProjId = Qry.ProjectId ?? 0;
                        model.ProjectNumber = context.tblProject.Where(m => m.ProjectId == ProjId).Select(m => m.ProjectNumber).FirstOrDefault();
                        int PIId = Qry.PIId ?? 0;
                        model.DepartmentName = context.vwFacultyStaffDetails.Where(m => m.UserId == PIId).Select(m => m.DepartmentName).FirstOrDefault();
                        model.PIName = context.vwFacultyStaffDetails.Where(m => m.UserId == PIId).Select(m => m.FirstName).FirstOrDefault();
                        model.SACNumber = Convert.ToString(Qry.TaxCode);
                        model.DescriptionofServices = Qry.DescriptionofServices;

                        model.IITMGSTIN = "33AAAAI3615G1Z6";
                        model.Name = Qry.AgencyRegName;
                        model.Address = Qry.CommunicationAddress;
                        model.GSTIN = Qry.AgencyRegGSTIN;
                        model.PANNo = Qry.AgencyRegPAN;
                        model.District = Qry.AgencyDistrict;
                        model.PinCode = Qry.AgencyPincode.ToString();
                        model.TANNo = Qry.AgencyRegTAN;
                        model.Email = Qry.AgencyContactPersonEmail;
                        model.ContactPerson = Qry.AgencyContactPersonName;
                        model.ContactNo = Qry.AgencyContactPersonNumber;
                        if (QryAgency != null)
                        {
                            model.District = Qry.AgencyDistrict;
                            model.PinCode = Convert.ToString(QryAgency.PinCode);
                            model.TANNo = QryAgency.TAN;
                            model.Email = QryAgency.ContactEmail;
                            model.ContactPerson = QryAgency.ContactPerson;
                            model.ContactNo = QryAgency.ContactNumber;
                        }
                        if (QryState != null)
                        {
                            model.State = QryState.StateName ?? "";
                            model.StateCode = QryState.StateCode ?? "";
                        }

                        decimal ttlVal = CreditnoteQry.TotalCreditAmount ?? 0;
                        model.TaxableValue1 = String.Format(Indian, "{0:N}", CreditnoteQry.CreditAmount);
                        model.Amount = String.Format(Indian, "{0:N}", ttlVal);
                        model.TotalInvoiceValueInWords = CoreAccountsService.words(ttlVal);
                        if (QryInvoiceTax != null)
                        {
                            model.SGSTstr = String.Format(Indian, "{0:N}", CreditnoteQry.SGST);
                            model.CGSTstr = String.Format(Indian, "{0:N}", CreditnoteQry.CGST);
                            model.IGSTstr = String.Format(Indian, "{0:N}", CreditnoteQry.IGST);
                            if (CreditnoteQry.IGSTPercentage > 0)
                            {
                                decimal? gstSplit = CreditnoteQry.IGST / 2;
                                model.IGSTPct = String.Format("{0:0.##}", CreditnoteQry.IGSTPercentage);
                                model.SGSTPct = String.Format("{0:0.##}", gstSplit);
                                model.CGSTPct = String.Format("{0:0.##}", gstSplit);
                            }
                            else if (CreditnoteQry.CGSTPercentage > 0)
                            {
                                decimal? ttlGST = CreditnoteQry.CGSTPercentage + CreditnoteQry.SGSTPercentage;
                                model.IGSTPct = String.Format("{0:0.##}", ttlGST);
                                model.SGSTPct = String.Format("{0:0.##}", QryInvoiceTax.SGSTRate);
                                model.CGSTPct = String.Format("{0:0.##}", QryInvoiceTax.CGSTRate);
                            }
                        }
                        model.ACName = "The Registrar, IIT Madras";
                        model.ACNo = "2722101016162";
                        model.BankName = "Canara Bank";
                        model.BranchName = "IIT-Madras Branch";
                        model.IFSC = "CNRB0002722";
                        model.MICRCode = "600015085";
                        model.SWIFTCode = "CNRBINBBIIT";
                    }
                    
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #region Commitment Bill Report
        public ProjSummaryModel GetCommitmentBillReport(int Id = 0)
        {
            ProjSummaryModel model = new ProjSummaryModel();
            ProjectSummaryModel Summary = new ProjectSummaryModel();
            try
            {
                using (var context = new IOASDBEntities())
                {

                    var Commit = context.tblCommitment.Where(m => m.CommitmentId == Id).FirstOrDefault();
                    var ProjDet = context.tblProject.Where(m => m.ProjectId == Commit.ProjectId).FirstOrDefault();
                    var CommDet = context.tblCommitmentDetails.Where(m => m.CommitmentId == Id).FirstOrDefault();
                    ProjectService pro = new ProjectService();
                    var SummServ = pro.getProjectSummary(Commit.ProjectId ?? 0);
                    var Provisional = GetProvisionalStatement(Convert.ToString(Commit.CRTD_TS), Convert.ToString(Commit.CRTD_TS), Commit.ProjectId ?? 0);

                    if (Commit.ProjectId > 0)
                    {
                        model.MaxAllowed = Common.getAllocationValue(Commit.ProjectId ?? 0, CommDet.AllocationHeadId ?? 0, Commit.CRTD_TS).TotalCommitForCurrentYear;
                        model.BudgAlloc = Common.getAllocationValue(Commit.ProjectId ?? 0, CommDet.AllocationHeadId ?? 0, Commit.CRTD_TS).TotalAllocation;

                        model.vendorName = Commit.VendorNameStr;

                        if (Commit.Currency > 0)
                            model.Currency = context.tblCurrency.Where(m => m.CurrencyID == Commit.Currency).Select(m => m.ISOCode).FirstOrDefault() ?? "INR";
                        else
                            model.Currency = "INR";
                        if (CommDet.ForignCurrencyValue > 0)
                            model.ForeCurr = CommDet.ForignCurrencyValue ?? 0;
                        if (Commit.CurrencyRate > 0)
                            model.ExcRate = Commit.CurrencyRate ?? 0;
                        if (Commit.AdditionalCharge > 0)
                            model.Addcharge = (Commit.AdditionalCharge ?? 0) + " %";
                        if (Commit.CastEmployeeId > 0)
                            model.StaffName = context.vwCombineStaffDetails.Where(m => m.ID == Commit.CastEmployeeId).Select(m => m.Name).FirstOrDefault();
                        if (Commit.StartDate != null)
                            model.StartDate = string.Format("{0:dd-MMM-yyyy}", Commit.StartDate);
                        if (Commit.CloseDate != null)
                            model.EndDate = string.Format("{0:dd-MMM-yyyy}", Commit.CloseDate);
                        model.PoNo = Commit.PurchaseOrder;
                        model.ItemDesc = Commit.ItemDescription;
                        Summary.OpeningBalance = Provisional.OB;
                        Summary.NetBalance = SummServ.NetBalance;
                        Summary.SanctionedValue = SummServ.SanctionedValue;
                        Summary.ProjectNo = ProjDet.ProjectNumber;
                        Summary.ProjectTittle = ProjDet.ProjectTitle;
                        Summary.PIname = SummServ.PIname;
                        Summary.ProjectType = SummServ.ProjectType;
                        var common = Common.GetProjectsDetails(Commit.ProjectId ?? 0);
                        Summary.StartDate = string.Format("{0:dd-MMM-yyyy}", common.SancationDate);
                        Summary.CloseDate = string.Format("{0:dd-MMM-yyyy}", common.CloseDate);
                        model.Summary = Summary;

                        if (model.Summary.StartDate != null)
                            model.ProjectStartDate = String.Format("{0:dd-MMMM-yyyy}", model.Summary.StartDate);
                        if (model.Summary.CloseDate != null)
                            model.ProjectCloseDate = String.Format("{0:dd-MMMM-yyyy}", model.Summary.CloseDate);
                        model.ProjId = Commit.ProjectId ?? 0;
                        model.ProjectNo = ProjDet.ProjectNumber;
                        model.CommitNo = Commit.CommitmentNumber;
                        model.CommitAmt = CommDet.Amount ?? 0;
                        model.CommitBalanceAmt = CommDet.BalanceAmount ?? 0;
                        model.BalCommitAmt = CommDet.ClosedAmount ?? 0;
                        model.ExpAmt = Common.getAllocationValue(Commit.ProjectId ?? 0, CommDet.AllocationHeadId ?? 0, Commit.CRTD_TS).TotalCommitmentTilDate;
                        model.Date = String.Format("{0:dd-MMMM-yyyy}", Commit.CRTD_TS);
                        model.PrintDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                        model.loginUser = Common.GetUserNameBasedonId(Commit.CRTD_UserID ?? 0);
                        model.HeadAmount = CommDet.Amount ?? 0;
                        model.HeadName = Common.GetAllocationHead(CommDet.AllocationHeadId ?? 0);
                        model.Remarks = (Commit.Description ?? "") + "-" + (Commit.ItemDescription ?? "") + "-" + (Commit.AttachmentName ?? "");
                        model.Status = Commit.Status;
                        if (Commit.ReferenceNo != null)
                            model.SourceReference = Common.GetTapalNo(Convert.ToInt32(Commit.ReferenceNo));
                        if (Commit.EmailDate != null)
                            model.SourceEmail = String.Format("{0:dd-MMMM-yyyy}", Commit.EmailDate);

                        //IC36775 13/12/2022 for the ticket #8167
                        model.CommitType = Commit.CommitmentType ?? 0;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region RF Report
        public static RFreportModel GetAngencywiseRF(int FinYear)
        {
            RFreportModel model = new RFreportModel();

            List<RFAgencyModel> Agencylist = new List<RFAgencyModel>();
            List<RFAgencyValueModel> AgencyValuelist = new List<RFAgencyValueModel>();

            try
            {
                using (var context = new IOASDBEntities())
                {
                    int?[] Finyear = null;
                    int?[] AgencyId = null;
                    DateTime StartDate = new DateTime(2012, 4, 1);
                    int[] Financialyear = context.tblFinYear.Where(m => m.StartDate >= StartDate).Select(m => m.FinYearId).ToArray();
                    Finyear = context.tblProject.Where(m => m.PIDepartment != null && Financialyear.Contains(m.FinancialYear ?? 0) && m.FinancialYear <= FinYear && m.SponsoringAgency > 0 && m.Status == "Active" && m.ProjectClassification == 12).Select(m => m.FinancialYear).OrderBy(m => m.Value).Distinct().ToArray();
                    // int?[] AgencyId = { 2241, 2243, 2244 };
                    AgencyId = context.tblProject.Where(m => m.PIDepartment != null && Financialyear.Contains(m.FinancialYear ?? 0) && m.FinancialYear <= FinYear && m.SponsoringAgency > 0 && m.Status == "Active" && m.ProjectClassification == 12).Select(m => m.SponsoringAgency).OrderBy(m => m.Value).Distinct().ToArray();


                    // object[] arr = new object[Finyear.Count()];
                    // object[] arr2 = new object[3];

                    decimal[,] a = new decimal[Finyear.Length, 3];
                    decimal TotalCount = 0; decimal TotalSanction = 0; decimal TotalExp = 0;
                    for (int j = 0; j < AgencyId.Length; j++)
                    {
                        int? Agency = AgencyId[j];
                        string AgencyName = context.tblAgencyMaster.Where(m => m.AgencyId == Agency).Select(m => m.AgencyCode).FirstOrDefault();
                        Agencylist.Add(new RFAgencyModel()
                        { Agency = AgencyName });
                        decimal Count = 0; decimal Sanction = 0; decimal Exp = 0;
                        List<decimal> Values = new List<decimal>();
                        for (int i = 0; i < Finyear.Length; i++)
                        {

                            int? Finyearid = Finyear[i];
                            var FinQry = context.tblFinYear.Where(m => m.FinYearId == Finyearid).FirstOrDefault();
                            DateTime FromDate = Convert.ToDateTime(FinQry.StartDate);
                            DateTime ToDate = Convert.ToDateTime(FinQry.EndDate);
                            ToDate = ToDate.AddDays(1).AddTicks(-1);

                            int[] Projectidlist = null;
                            Projectidlist = context.tblProject.Where(m => m.SponsoringAgency == Agency && Financialyear.Contains(m.FinancialYear ?? 0) && m.PIDepartment != null && m.Status == "Active" && m.ProjectClassification == 12).Select(m => m.ProjectId).Distinct().ToArray();
                            decimal projectcount = context.tblProject.Where(m => m.SponsoringAgency == Agency && Financialyear.Contains(m.FinancialYear ?? 0) && m.PIDepartment != null && m.FinancialYear == FinQry.FinYearId && m.Status == "Active" && m.ProjectClassification == 12).Count();
                            Values.Add(projectcount);

                            // arr2[0] = Convert.ToDecimal(arr2[0]) + projectcount;
                            a[i, 0] = Convert.ToDecimal(a[i, 0]) + projectcount;
                            Count += projectcount;

                            decimal projectcost = context.tblProject.Where(m => m.SponsoringAgency == Agency && Financialyear.Contains(m.FinancialYear ?? 0) && m.PIDepartment != null && m.FinancialYear == FinQry.FinYearId && m.Status == "Active" && m.ProjectClassification == 12).Sum(m => m.SanctionValue) ?? 0;
                            projectcost = projectcost / 100000;
                            Values.Add(projectcost);
                            // arr2[1] = Convert.ToDecimal(arr2[1]) + projectcost;
                            a[i, 1] = Convert.ToDecimal(a[i, 1]) + projectcost;
                            Sanction += projectcost;

                            decimal projectexp = context.vw_ProjectExpenditureReport.AsNoTracking().Where(m => m.CommitmentDate >= FromDate && m.Posted_f == true && m.CommitmentDate <= ToDate && Projectidlist.Contains(m.ProjectId ?? 0)).Sum(m => m.AmountSpent) ?? 0;
                            projectexp = projectexp / 100000;
                            Values.Add(projectexp);
                            //  arr2[2] = Convert.ToDecimal(arr2[2]) + projectexp;
                            a[i, 2] = Convert.ToDecimal(a[i, 2]) + projectexp;
                            Exp += projectexp;

                            //  arr[i] = arr2;

                        }
                        Values.Add(Count);
                        TotalCount = TotalCount + Count;
                        Values.Add(Sanction);
                        TotalSanction = TotalSanction + Sanction;
                        Values.Add(Exp);
                        TotalExp = TotalExp + Exp;

                        AgencyValuelist.Add(new RFAgencyValueModel()
                        { values = Values });
                    }
                    List<decimal> TotalValues = new List<decimal>();
                    Agencylist.Add(new RFAgencyModel()
                    { Agency = "Grand Total" });
                    for (int k = 0; k < Finyear.Length; k++)
                    {

                        TotalValues.Add(a[k, 0]);
                        TotalValues.Add(a[k, 1]);
                        TotalValues.Add(a[k, 2]);
                    }
                    TotalValues.Add(TotalCount);
                    TotalValues.Add(TotalSanction);
                    TotalValues.Add(TotalExp);
                    AgencyValuelist.Add(new RFAgencyValueModel()
                    { values = TotalValues });

                    model.Agencylist = Agencylist;
                    model.AgencyValuelist = AgencyValuelist;
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        public static RFreportModel GetDepartwiseRF(int FinYear)
        {
            RFreportModel model = new RFreportModel();
            List<RFDepartModel> Departlist = new List<RFDepartModel>();
            List<RFDepartValueModel> DepartValuelist = new List<RFDepartValueModel>();
            try
            {

                using (var context = new IOASDBEntities())
                {

                    string[] Department = null;
                    int?[] AgencyId = null;
                    DateTime StartDate = new DateTime(2012, 4, 1);
                    int[] Financialyear = context.tblFinYear.Where(m => m.StartDate >= StartDate).Select(m => m.FinYearId).ToArray();
                    AgencyId = context.tblProject.Where(m => m.PIDepartment != null && Financialyear.Contains(m.FinancialYear ?? 0) && m.FinancialYear <= FinYear && m.SponsoringAgency > 0 && m.Status == "Active" && m.ProjectClassification == 12).Select(m => m.SponsoringAgency).OrderBy(m => m.Value).Distinct().ToArray();
                    //  int?[] AgencyId = { 2241, 2243, 2244 };
                    Department = context.tblProject.Where(m => m.PIDepartment != null && Financialyear.Contains(m.FinancialYear ?? 0) && m.FinancialYear <= FinYear && AgencyId.Contains(m.SponsoringAgency ?? 0) && m.Status == "Active" && m.ProjectClassification == 12).Select(m => m.PIDepartment).Distinct().ToArray();

                    decimal[,] a = new decimal[AgencyId.Length, 3];
                    decimal TotalCount = 0; decimal TotalSanction = 0; decimal TotalExp = 0;
                    for (int i = 0; i < Department.Length; i++)
                    {
                        int[] Projectidlist = null;
                        string Departments = Department[i];
                        Departlist.Add(new RFDepartModel()
                        { Departments = Departments });
                        decimal Count = 0; decimal Sanction = 0; decimal Exp = 0;
                        List<decimal> Values = new List<decimal>();
                        for (int j = 0; j < AgencyId.Length; j++)
                        {
                            int? Agency = AgencyId[j];

                            Projectidlist = context.tblProject.Where(m => m.SponsoringAgency == Agency && Financialyear.Contains(m.FinancialYear ?? 0) && m.FinancialYear <= FinYear && m.PIDepartment == Departments && m.Status == "Active" && m.ProjectClassification == 12).Select(m => m.ProjectId).Distinct().ToArray();
                            decimal projectcount = context.tblProject.Where(m => m.SponsoringAgency == Agency && Financialyear.Contains(m.FinancialYear ?? 0) && m.FinancialYear <= FinYear && m.PIDepartment == Departments && m.Status == "Active" && m.ProjectClassification == 12).Count();
                            Values.Add(projectcount);
                            a[j, 0] = Convert.ToDecimal(a[j, 0]) + projectcount;
                            Count += projectcount;

                            decimal projectcost = context.tblProject.Where(m => m.SponsoringAgency == Agency && Financialyear.Contains(m.FinancialYear ?? 0) && m.FinancialYear <= FinYear && m.PIDepartment == Departments && m.Status == "Active" && m.ProjectClassification == 12).Sum(m => m.SanctionValue) ?? 0;
                            projectcost = projectcost / 100000;
                            Values.Add(projectcost);
                            a[j, 1] = Convert.ToDecimal(a[j, 1]) + projectcost;
                            Sanction += projectcost;

                            decimal projectexp = context.vw_ProjectExpenditureReport.AsNoTracking().Where(m => Projectidlist.Contains(m.ProjectId ?? 0) && m.Posted_f == true).Sum(m => m.AmountSpent) ?? 0;
                            projectexp = projectexp / 100000;
                            Values.Add(projectexp);
                            a[j, 2] = Convert.ToDecimal(a[j, 2]) + projectexp;
                            Exp += projectexp;
                        }
                        Values.Add(Count);
                        TotalCount = TotalCount + Count;
                        Values.Add(Sanction);
                        TotalSanction = TotalSanction + Sanction;
                        Values.Add(Exp);
                        TotalExp = TotalExp + Exp;

                        DepartValuelist.Add(new RFDepartValueModel()
                        { values = Values });
                    }

                    List<decimal> TotalValues = new List<decimal>();
                    Departlist.Add(new RFDepartModel()
                    { Departments = "Grand Total" });
                    for (int k = 0; k < AgencyId.Length; k++)
                    {

                        TotalValues.Add(a[k, 0]);
                        TotalValues.Add(a[k, 1]);
                        TotalValues.Add(a[k, 2]);
                    }
                    TotalValues.Add(TotalCount);
                    TotalValues.Add(TotalSanction);
                    TotalValues.Add(TotalExp);
                    DepartValuelist.Add(new RFDepartValueModel()
                    { values = TotalValues });

                    model.Departlist = Departlist;
                    model.DepartValuelist = DepartValuelist;
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        public static RFreportModel GetRFsummary(int FinYear)
        {
            RFreportModel model = new RFreportModel();
            List<RFYearModel> Yearlist = new List<RFYearModel>();
            List<RFYearValueModel> YearValuelist = new List<RFYearValueModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    int[] Finyear;
                    int?[] AgencyId = context.tblProject.Where(m => m.PIDepartment != null && m.FinancialYear <= FinYear && m.SponsoringAgency > 0 && m.ProjectClassification == 12).Select(m => m.SponsoringAgency).Distinct().ToArray();
                    //  int?[] AgencyId = { 2241, 2243, 2244 };
                    DateTime StartDate = new DateTime(2012, 4, 1);
                    int[] Financialyear = context.tblFinYear.Where(m => m.StartDate >= StartDate).Select(m => m.FinYearId).ToArray();

                    var Finyearlist = context.tblProject.Where(m => m.PIDepartment != null && Financialyear.Contains(m.FinancialYear ?? 0) && m.FinancialYear <= FinYear && AgencyId.Contains(m.SponsoringAgency ?? 0) && m.ProjectClassification == 12).Select(m => m.FinancialYear).Distinct().ToArray();
                    var Qry = context.tblFinYear.Where(m => Finyearlist.Contains(m.FinYearId)).OrderBy(m => m.Year).ToArray();
                    Finyear = Qry.Select(m => m.FinYearId).Distinct().ToArray();
                    decimal[] a = new decimal[Finyear.Length];

                    decimal TotalExp = 0;
                    for (int i = 0; i < Finyear.Count(); i++)
                    {
                        int? Finyearid = Finyear[i];
                        int[] Projectidlist = null;
                        Projectidlist = context.tblProject.Where(m => AgencyId.Contains(m.SponsoringAgency ?? 0) && m.FinancialYear == Finyearid && m.PIDepartment != null && m.Status == "Active" && m.ProjectClassification == 12).Select(m => m.ProjectId).Distinct().ToArray();
                        var Fin = context.tblFinYear.Where(m => m.FinYearId == Finyearid).FirstOrDefault();

                        Yearlist.Add(new RFYearModel()
                        { Year = Convert.ToString(Fin.Year) });
                        decimal Exp = 0;
                        List<decimal> Values = new List<decimal>();
                        for (int j = 0; j < Finyear.Count(); j++)
                        {
                            int? Finid = Finyear[j];
                            var FinQry = context.tblFinYear.Where(m => m.FinYearId == Finid).FirstOrDefault();
                            DateTime FromDate = Convert.ToDateTime(FinQry.StartDate);
                            DateTime ToDate = Convert.ToDateTime(FinQry.EndDate);
                            decimal projectexp = context.vw_ProjectExpenditureReport.AsNoTracking().Where(m => m.CommitmentDate >= FromDate && m.Posted_f == true && m.CommitmentDate <= ToDate && Projectidlist.Contains(m.ProjectId ?? 0)).Sum(m => m.AmountSpent) ?? 0;
                            projectexp = projectexp / 100000;
                            Values.Add(projectexp);
                            a[j] = Convert.ToDecimal(a[j]) + projectexp;
                            Exp += projectexp;
                        }
                        Values.Add(Exp);
                        TotalExp = TotalExp + Exp;
                        YearValuelist.Add(new RFYearValueModel()
                        { values = Values });
                    }

                    List<decimal> TotalValues = new List<decimal>();
                    Yearlist.Add(new RFYearModel()
                    { Year = "Grand Total" });
                    for (int k = 0; k < Finyear.Count(); k++)
                    {
                        TotalValues.Add(a[k]);
                    }
                    TotalValues.Add(TotalExp);
                    YearValuelist.Add(new RFYearValueModel()
                    { values = TotalValues });
                    model.Yearlist = Yearlist;
                    model.YearValuelist = YearValuelist;
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region SOE
        public static MonthandYearExp GetMonthandYearExp(DateTime Fromdate, DateTime ToDate, int Projectid)
        {
            MonthandYearExp model = new MonthandYearExp();
            try
            {
                decimal PrevAmount = 0;
                using (var context = new IOASDBEntities())
                {
                    decimal Amount = (from C in context.vw_ProjectExpenditureReport.AsNoTracking() where C.ProjectId == Projectid && C.Posted_f == true && C.CommitmentDate >= Fromdate && C.CommitmentDate <= ToDate select C.AmountSpent).Sum() ?? 0;
                    using (var contextEx = new IOASExternalEntities())
                    {
                        PrevAmount = (from C in contextEx.tblOldExpenditure where C.ProjectId == Projectid && C.ExpenditureDate >= Fromdate && C.ExpenditureDate <= ToDate select C.TotalAmount).Sum() ?? 0;
                    }
                    model.Amount = Amount + PrevAmount;
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        public static MonthandYearExp GetMonthandYearRCV(DateTime Fromdate, DateTime ToDate, int Projectid)
        {
            MonthandYearExp model = new MonthandYearExp();
            try
            {

                using (var context = new IOASDBEntities())
                {
                    decimal PrevAmount = 0;
                    var recqry = (from C in context.tblReceipt where C.ProjectId == Projectid && C.Posted_f == true && C.Status == "Completed" && C.CrtdTS >= Fromdate && C.CrtdTS <= ToDate select C).ToList();
                    using (var contextEx = new IOASExternalEntities())
                    {
                        PrevAmount = (from C in contextEx.tblOldreceipts where C.ProjectId == Projectid && C.Date >= Fromdate && C.Date <= ToDate select C.Amountreceived).Sum() ?? 0;
                    }
                    model.Amount = (recqry.Where(m => m.CategoryId != 16).Select(m => m.ReceiptAmount).Sum() ?? 0) - ((recqry.Where(m => m.CategoryId != 16).Select(m => m.IGST).Sum() ?? 0) + (recqry.Where(m => m.CategoryId != 16).Select(m => m.SGST).Sum() ?? 0) + (recqry.Where(m => m.CategoryId != 16).Select(m => m.CGST).Sum() ?? 0));
                    model.Amount = model.Amount + PrevAmount;
                    model.IntersetAmount = (recqry.Where(m => m.CategoryId == 16).Select(m => m.ReceiptAmount).Sum() ?? 0) - ((recqry.Where(m => m.CategoryId == 16).Select(m => m.IGST).Sum() ?? 0) + (recqry.Where(m => m.CategoryId == 16).Select(m => m.SGST).Sum() ?? 0) + (recqry.Where(m => m.CategoryId == 16).Select(m => m.CGST).Sum() ?? 0));
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        public ProjectSummaryDetailModel getProjectSummaryDetailsSOE(int ProjectId, DateTime FromDate, DateTime ToDate, int year)
        {
            try
            {
                ProjectSummaryDetailModel model = new ProjectSummaryDetailModel();
                List<HeadWiseDetailModel> List = new List<HeadWiseDetailModel>();
                using (var context = new IOASDBEntities())
                {
                    using (var contextEx = new IOASExternalEntities())
                    {
                        // ToDate = ToDate.AddDays(1).AddTicks(-2);
                        year = year + 1;
                        var qryHead = (from C in context.tblCommitment
                                       join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                       where C.ProjectId == ProjectId && C.Status != "InActive"
                                       select new { AllocationHead = D.AllocationHeadId }).ToList();
                        var queryAlloc = (from C in context.tblProjectAllocation
                                          where C.ProjectId == ProjectId && C.AllocationValue != 0
                                          select C).OrderBy(c => c.AllocationId).ToList();
                        var queryPFT = (from C in context.tblProjectTransfer
                                        join D in context.tblProjectTransferDetails on C.ProjectTransferId equals D.ProjectTransferId
                                        where (C.DebitProjectId == ProjectId || C.CreditProjectId == ProjectId) && C.Status == "Completed"
                                        select new { AllocationHead = D.BudgetHeadId }).ToList();
                        var queryOpenExp = (from C in context.tblProjectOBDetail
                                            where C.ProjectId == ProjectId && C.OpeningExp != 0
                                            select new { AllocationHead = C.HeadId }).ToList();
                        var queryEnhAlloc = (from C in context.tblProjectEnhancementAllocation
                                             where C.ProjectId == ProjectId && C.Status == "Active" && C.EnhancedValue != 0
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
                        TotalHead = TotalHead.OrderBy(m => m.Value).ToArray();
                        if (TotalHead.Length > 0)
                        {
                            for (int i = 0; i < TotalHead.Length; i++)
                            {
                                decimal TotBudgetAmt = 0;
                                decimal OB = 0;
                                int headId = TotalHead[i] ?? 0;
                                decimal AllocatValue = 0; decimal EnhanveValue = 0;
                                var YearAlloc = (from C in context.tblProject
                                                 where C.ProjectId == ProjectId && C.IsYearWiseAllocation == true
                                                 select C).FirstOrDefault();

                                var HeadName = context.tblBudgetHead.Where(m => m.BudgetHeadId == headId).Select(m => m.HeadName).FirstOrDefault();
                                if (YearAlloc == null)
                                {
                                    AllocatValue = queryAlloc.Where(m => m.AllocationHead == headId).Sum(m => m.AllocationValue) ?? 0;
                                    EnhanveValue = queryEnhAlloc.Where(m => m.AllocationHead == headId).Sum(m => m.EnhancedValue) ?? 0;
                                    if (AllocatValue != 0)
                                        TotBudgetAmt += AllocatValue;
                                    if (EnhanveValue != 0)
                                        TotBudgetAmt += EnhanveValue;
                                }
                                else
                                {
                                    var Datelist = Common.GetProjectExpYear(ProjectId);
                                    int id = Datelist.OrderByDescending(m => m.Id).Select(m => m.Id).FirstOrDefault();
                                    if (year == (id + 1))
                                        EnhanveValue = queryEnhAlloc.Where(m => m.AllocationHead == headId).Sum(m => m.EnhancedValue) ?? 0;
                                    AllocatValue = queryAlloc.Where(m => m.AllocationHead == headId && m.Year == year).Sum(m => m.AllocationValue) ?? 0;
                                    if (AllocatValue != 0)
                                        TotBudgetAmt += AllocatValue;
                                    if (EnhanveValue != 0)
                                        TotBudgetAmt += EnhanveValue;
                                }


                                var qryHeadCommit = (from C in context.tblCommitment
                                                     join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                                     where C.ProjectId == ProjectId && D.AllocationHeadId == headId && C.Status == "Active"
                                                     select new { D.AllocationHeadId, D.Amount, C.CommitmentBalance, D.BalanceAmount }).ToList();
                                decimal BalComm = (qryHeadCommit.Select(m => m.BalanceAmount).Sum() ?? 0) +
                        ((from C in context.vwCommitmentSpentBalance where C.ProjectId == ProjectId && (C.Posted_f == false || C.Posted_f == null) select C.AmountSpent).Sum() ?? 0);

                                var qryBalance = (from exp in context.vw_ProjectExpenditureReport.AsNoTracking()
                                                  where exp.ProjectId == ProjectId && exp.Posted_f == true && exp.CommitmentDate >= FromDate && exp.CommitmentDate <= ToDate && exp.AllocationHeadId == headId
                                                  select exp).ToList();
                                decimal NewAmount = qryBalance.Select(m => m.NewAmount).Sum() ?? 0;
                                decimal SpentAmount = qryBalance.Select(m => m.AmountSpent).Sum() ?? 0;
                                var queryOB = (from C in contextEx.tblOldExpenditure
                                               where C.ProjectId == ProjectId && C.Headid == headId
                                               && C.ExpenditureDate >= FromDate && C.ExpenditureDate <= ToDate
                                               select C).ToList();

                                if (queryOB != null)

                                    OB = queryOB.Sum(m => m.TotalAmount) ?? 0;
                                var OpExp = TotBudgetAmt - OB;

                                List.Add(new HeadWiseDetailModel()
                                {
                                    AllocationId = headId,
                                    AllocationHeadName = HeadName,
                                    Amount = TotBudgetAmt,
                                    BalanceAmount = BalComm,
                                    Expenditure = SpentAmount + OB,
                                    Total = BalComm + SpentAmount + OB,
                                    Available = TotBudgetAmt - (BalComm + SpentAmount + OB)
                                });
                            }
                        }
                    }
                }
                model.HeadWise = List;

                return model;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #region ICSROH Report
        public static ICSROHReportModel GetICSROH(DateTime FromDate, DateTime ToDate)
        {
            ICSROHReportModel model = new ICSROHReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    // int[] InterId = { 72, 73, 74, 75 };
                    int[] InterId = context.tblAccountHead.Where(m => m.AccountGroupId == 27).Select(m => m.AccountHeadId).ToArray();
                    int[] ProjectIdArr = context.tblProject.Where(m => m.ProjectClassification == 4 || m.ProjectClassification == 6).Select(m => m.ProjectId).ToArray();

                    //var FinQry = context.tblFinYear.Where(m => m.FinYearId == Year).FirstOrDefault();
                    //DateTime FromDate = Convert.ToDateTime(FinQry.StartDate);
                    //DateTime ToDate = Convert.ToDateTime(FinQry.EndDate);
                    ToDate = ToDate.AddDays(1).AddTicks(-1);

                    //var InterlistCr = (from a in context.tblBOA
                    //                   join b in context.tblBOATransaction on a.BOAId equals b.BOAId
                    //                   where InterId.Contains(b.AccountHeadId ?? 0) && a.PostedDate >= FromDate && a.PostedDate <= ToDate && a.Status == "Posted"
                    //                   && b.TransactionType == "Credit"
                    //                   select b).ToList();
                    //var InterlistDr = (from a in context.tblBOA
                    //                   join b in context.tblBOATransaction on a.BOAId equals b.BOAId
                    //                   where InterId.Contains(b.AccountHeadId ?? 0) && a.PostedDate >= FromDate && a.PostedDate <= ToDate && a.Status == "Posted"
                    //                   && b.TransactionType == "Debit"
                    //                   select b).ToList();
                    //string[] InterNo = (from a in context.tblBOA
                    //                    join b in context.tblBOATransaction on a.BOAId equals b.BOAId
                    //                    where InterId.Contains(b.AccountHeadId ?? 0) && a.PostedDate >= FromDate && a.PostedDate <= ToDate && a.Status == "Posted"
                    //                    // && b.TransactionType == "Credit"
                    //                    select a.RefNumber).ToArray();


                    //  decimal Inter = (InterlistCr.Sum(m => m.Amount) ?? 0) - ((InterlistDr.Sum(m => m.Amount) ?? 0));
                    // Inter = (Math.Round(Inter) / 100000);
                    decimal Inter = 0;
                    var SponReclist = context.tblReceipt.Where(m => m.CrtdTS >= FromDate && m.CrtdTS <= ToDate && m.Posted_f == true && ProjectIdArr.Contains(m.ProjectId ?? 0) && m.Status == "Completed" && m.Description.StartsWith("RCV/")).ToList();
                    var SponOHlist = (from a in context.tblReceipt
                                      join b in context.tblOHReversal on a.Description equals b.OHReversalNumber
                                      where ProjectIdArr.Contains(a.ProjectId ?? 0) && b.ProjectType == 1 && a.CrtdTS >= FromDate && a.Posted_f == true && a.CrtdTS <= ToDate && a.Status == "Completed"
                                      select a).ToList();
                    decimal SponRec = (SponReclist.Sum(m => m.ReceiptAmount)) - (SponReclist.Sum(m => m.CGST) + SponReclist.Sum(m => m.IGST) + SponReclist.Sum(m => m.SGST)) ?? 0;
                    SponRec = (SponRec + SponOHlist.Sum(m => m.ReceiptAmount)) ?? 0;
                    SponRec = (Math.Round(SponRec) / 100000);
                    var ConsReclist = context.tblReceipt.Where(m => m.CrtdTS >= FromDate && m.CrtdTS <= ToDate && m.Posted_f == true && ProjectIdArr.Contains(m.ProjectId ?? 0) && m.Status == "Completed" && (m.Description.StartsWith("DOP/"))).ToList();
                    var ConsOHlist = (from a in context.tblReceipt
                                      join b in context.tblOHReversal on a.Description equals b.OHReversalNumber
                                      where ProjectIdArr.Contains(a.ProjectId ?? 0) && b.ProjectType == 2 && a.CrtdTS >= FromDate && a.Posted_f == true && a.CrtdTS <= ToDate && a.Status == "Completed"
                                      select a).ToList();
                    var ConsDislist = context.tblReceipt.Where(m => m.CrtdTS >= FromDate && m.CrtdTS <= ToDate && m.Posted_f == true && ProjectIdArr.Contains(m.ProjectId ?? 0) && m.Status == "Completed" && (m.Description.StartsWith("HON/") || m.Description.StartsWith("DIS/"))).ToList();
                    decimal ConsDis = (ConsDislist.Sum(m => m.ReceiptAmount)) ?? 0;
                    ConsDis = Math.Round(ConsDis) / 100000;
                    decimal ConsRec = ((ConsReclist.Sum(m => m.ReceiptAmount)) + ConsOHlist.Sum(m => m.ReceiptAmount)) ?? 0;
                    ConsRec = Math.Round(ConsRec) / 100000;

                    string[] SponOHArray = SponOHlist.Select(m => m.ReceiptNumber).ToArray();
                    string[] ConsOHArray = ConsOHlist.Select(m => m.ReceiptNumber).ToArray();
                    var OtherReclist = context.tblReceipt.Where(m => m.CrtdTS >= FromDate && m.CrtdTS <= ToDate && m.Posted_f == true && ProjectIdArr.Contains(m.ProjectId ?? 0) && m.Status == "Completed"
                   && !m.Description.StartsWith("RCV/") && !m.Description.StartsWith("DOP/") && !m.Description.StartsWith("HON/") && !m.Description.StartsWith("DIS/")
                   && !SponOHArray.Contains(m.ReceiptNumber) && !SponOHArray.Contains(m.ReceiptNumber)).ToList();
                    decimal OtherRec = OtherReclist.Sum(m => m.ReceivedAmount) ?? 0;
                    OtherRec = Math.Round(OtherRec) / 100000;
                    decimal? TotalIncome = (Inter + SponRec + ConsDis + ConsRec + OtherRec);
                    model.TotalIncome = TotalIncome ?? 0;

                    decimal InterPct = ((Inter / model.TotalIncome) * 100);
                    InterPct = Math.Round(InterPct);
                    model.Interest = Inter;
                    model.InterestPct = InterPct + " %";


                    decimal SponRecPct = ((SponRec / model.TotalIncome) * 100);
                    SponRecPct = Math.Round(SponRecPct);
                    model.SPONCorpus = SponRec;
                    model.SPONCorpusPct = SponRecPct + " %";

                    decimal ConsDisPct = ((ConsDis / model.TotalIncome) * 100);
                    ConsDisPct = Math.Round(ConsDisPct);
                    model.CONSDist = ConsDis;
                    model.CONSDistPct = ConsDisPct + " %";

                    decimal ConsRecPct = ((ConsRec / model.TotalIncome) * 100);
                    ConsRecPct = Math.Round(ConsRecPct);
                    model.CONSCorpus = ConsRec;
                    model.CONSCorpusPct = ConsRecPct + " %";

                    decimal OtherRecPct = ((OtherRec / model.TotalIncome) * 100);
                    OtherRecPct = Math.Round(OtherRecPct);
                    model.OtherIncome = OtherRec;
                    model.OtherIncomePct = OtherRecPct + " %";

                    int[] interAlloId = { 19, 29 };
                    int[] interNotAlloId = { 1, 2, 7, 19, 29 };
                    decimal Salary = context.vw_ProjectExpenditureReport.AsNoTracking().Where(m => m.CommitmentDate >= FromDate && m.Posted_f == true && m.CommitmentDate <= ToDate && ProjectIdArr.Contains(m.ProjectId ?? 0) && m.AllocationHeadId == 1).Sum(m => m.AmountSpent) ?? 0;
                    model.Salary = Math.Round(Salary) / 100000;
                    decimal InterestExp = context.vw_ProjectExpenditureReport.AsNoTracking().Where(m => m.CommitmentDate >= FromDate && m.Posted_f == true && m.CommitmentDate <= ToDate && ProjectIdArr.Contains(m.ProjectId ?? 0) && interAlloId.Contains(m.AllocationHeadId ?? 0)).Sum(m => m.AmountSpent) ?? 0;
                    model.InterestExp = Math.Round(InterestExp) / 100000;
                    decimal RepairsMaint = context.vw_ProjectExpenditureReport.AsNoTracking().Where(m => m.CommitmentDate >= FromDate && m.Posted_f == true && m.CommitmentDate <= ToDate && ProjectIdArr.Contains(m.ProjectId ?? 0) && m.AllocationHeadId == 2).Sum(m => m.AmountSpent) ?? 0;
                    model.RepairsMaint = Math.Round(RepairsMaint) / 100000;
                    decimal ITEquipment = context.vw_ProjectExpenditureReport.AsNoTracking().Where(m => m.CommitmentDate >= FromDate && m.Posted_f == true && m.CommitmentDate <= ToDate && ProjectIdArr.Contains(m.ProjectId ?? 0) && m.AllocationHeadId == 7).Sum(m => m.AmountSpent) ?? 0;
                    model.ITEquipment = Math.Round(ITEquipment) / 100000;
                    decimal OtherExpenses = context.vw_ProjectExpenditureReport.AsNoTracking().Where(m => m.CommitmentDate >= FromDate && m.Posted_f == true && m.CommitmentDate <= ToDate && ProjectIdArr.Contains(m.ProjectId ?? 0) && !interNotAlloId.Contains(m.AllocationHeadId ?? 0)).Sum(m => m.AmountSpent) ?? 0;
                    model.OtherExpenses = Math.Round(OtherExpenses) / 100000;
                    decimal TotalExpenses = (model.Salary + model.InterestExp + model.RepairsMaint + model.ITEquipment + model.OtherExpenses);
                    model.TotalExpenses = (TotalExpenses);

                    decimal TotalExpensesPct = ((model.TotalExpenses / model.TotalIncome) * 100);
                    TotalExpensesPct = Math.Round(TotalExpensesPct);
                    model.TotalExpensesPct = TotalExpensesPct + " %";

                    decimal SalaryPct = ((model.Salary / TotalExpenses) * TotalExpensesPct);
                    SalaryPct = Math.Round(SalaryPct);
                    model.SalaryPct = SalaryPct + " %";
                    decimal InterestExpPct = ((model.InterestExp / TotalExpenses) * TotalExpensesPct);
                    InterestExpPct = Math.Round(InterestExpPct);
                    model.InterestExpPct = InterestExpPct + " %";
                    decimal RepairsMaintPct = ((model.RepairsMaint / TotalExpenses) * TotalExpensesPct);
                    RepairsMaintPct = Math.Round(RepairsMaintPct);
                    model.RepairsMaintPct = RepairsMaintPct + " %";
                    decimal ITEquipmentPct = ((model.ITEquipment / TotalExpenses) * TotalExpensesPct);
                    ITEquipmentPct = Math.Round(ITEquipmentPct);
                    model.ITEquipmentPct = ITEquipmentPct + " %";
                    decimal OtherExpensesPct = ((model.OtherExpenses / TotalExpenses) * TotalExpensesPct);
                    OtherExpensesPct = Math.Round(OtherExpensesPct);
                    model.OtherExpensesPct = OtherExpensesPct + " %";

                    model.NetIncome = model.TotalIncome - model.TotalExpenses;
                    decimal NetIncomePct = (100 - TotalExpensesPct);
                    model.NetIncomePct = NetIncomePct + " %";
                    return model;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
                  (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return model;
            }
        }
        public static ICSROHReportModel GetICSROHList(DateTime start, DateTime end)
        {
            ICSROHReportModel model = new ICSROHReportModel();
            ICSROHReportListModel model1 = new ICSROHReportListModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    //  int[] InterId = { 72, 73, 74, 75 };
                    int[] InterId = context.tblAccountHead.Where(m => m.AccountGroupId == 27).Select(m => m.AccountHeadId).ToArray();
                    int[] ProjectIdArr = context.tblProject.Where(m => m.ProjectClassification == 4 || m.ProjectClassification == 6).Select(m => m.ProjectId).ToArray();
                    //var FinQry = context.tblFinYear.Where(m => m.FinYearId == Year).FirstOrDefault();
                    //DateTime start = Convert.ToDateTime(FinQry.StartDate);
                    //DateTime end = Convert.ToDateTime(FinQry.EndDate);
                    end = end.AddDays(1).AddTicks(-1);
                    DateTime FromDate;
                    DateTime ToDate;
                    List<decimal> SponReclt = new List<decimal>();
                    List<decimal> ConsDislt = new List<decimal>();
                    List<decimal> ConsReclt = new List<decimal>();
                    List<decimal> Intereslt = new List<decimal>();
                    List<decimal> OtherReclt = new List<decimal>();
                    List<decimal> Salarylt = new List<decimal>();
                    List<decimal> InterestExplt = new List<decimal>();
                    List<decimal> RepairsMaintlt = new List<decimal>();
                    List<decimal> ITEquipmentlt = new List<decimal>();
                    List<decimal> OtherExpenseslt = new List<decimal>();
                    List<decimal> TotalExpenseslt = new List<decimal>();
                    List<decimal> TotalIncomelt = new List<decimal>();
                    List<decimal> NetIncomelt = new List<decimal>();
                    // for (int i = 0; i < 12; i++)
                    //{
                    //FromDate = start.AddMonths(i);
                    //ToDate = FromDate.AddMonths(1);
                    //ToDate = ToDate.AddTicks(-1);
                    FromDate = start;
                    ToDate = end;
                    //var InterlistCr = (from a in context.tblBOA
                    //                       join b in context.tblBOATransaction on a.BOAId equals b.BOAId
                    //                       where InterId.Contains(b.AccountHeadId ?? 0) && a.PostedDate >= FromDate && a.PostedDate <= ToDate && a.Status == "Posted"
                    //                       && b.TransactionType == "Credit"
                    //                       select b).ToList();
                    //    var InterlistDr = (from a in context.tblBOA
                    //                       join b in context.tblBOATransaction on a.BOAId equals b.BOAId
                    //                       where InterId.Contains(b.AccountHeadId ?? 0) && a.PostedDate >= FromDate && a.PostedDate <= ToDate && a.Status == "Posted"
                    //                       && b.TransactionType == "Debit"
                    //                       select b).ToList();
                    //    string[] InterNo = (from a in context.tblBOA
                    //                        join b in context.tblBOATransaction on a.BOAId equals b.BOAId
                    //                        where InterId.Contains(b.AccountHeadId ?? 0) && a.PostedDate >= FromDate && a.PostedDate <= ToDate && a.Status == "Posted"
                    //                        // && b.TransactionType == "Credit"
                    //                        select a.RefNumber).ToArray();

                    //    decimal Inter = (InterlistCr.Sum(m => m.Amount) ?? 0) - (InterlistDr.Sum(m => m.Amount) ?? 0);
                    decimal Inter = 0;
                    Intereslt.Add(Inter);
                    var SponReclist = context.tblReceipt.Where(m => m.CrtdTS >= FromDate && m.CrtdTS <= ToDate && m.Posted_f == true && ProjectIdArr.Contains(m.ProjectId ?? 0) && m.Status == "Completed" && m.Description.StartsWith("RCV/")).ToList();
                    var SponOHlist = (from a in context.tblReceipt
                                      join b in context.tblOHReversal on a.Description equals b.OHReversalNumber
                                      where ProjectIdArr.Contains(a.ProjectId ?? 0) && b.ProjectType == 1 && a.CrtdTS >= FromDate && a.Posted_f == true && a.CrtdTS <= ToDate && a.Status == "Completed"
                                      select a).ToList();
                    decimal SponRec = (SponReclist.Sum(m => m.ReceiptAmount)) - (SponReclist.Sum(m => m.CGST) + SponReclist.Sum(m => m.IGST) + SponReclist.Sum(m => m.SGST)) ?? 0;
                    SponRec = (SponRec + SponOHlist.Sum(m => m.ReceiptAmount)) ?? 0;

                    SponReclt.Add(SponRec);

                    var ConsReclist = context.tblReceipt.Where(m => m.CrtdTS >= FromDate && m.Posted_f == true && m.CrtdTS <= ToDate && ProjectIdArr.Contains(m.ProjectId ?? 0) && m.Status == "Completed" && (m.Description.StartsWith("DOP/"))).ToList();
                    var ConsOHlist = (from a in context.tblReceipt
                                      join b in context.tblOHReversal on a.Description equals b.OHReversalNumber
                                      where ProjectIdArr.Contains(a.ProjectId ?? 0) && b.ProjectType == 2 && a.Posted_f == true && a.CrtdTS >= FromDate && a.CrtdTS <= ToDate && a.Status == "Completed"
                                      select a).ToList();
                    var ConsDislist = context.tblReceipt.Where(m => m.CrtdTS >= FromDate && m.Posted_f == true && m.CrtdTS <= ToDate && ProjectIdArr.Contains(m.ProjectId ?? 0) && m.Status == "Completed" && (m.Description.StartsWith("HON/") || m.Description.StartsWith("DIS/"))).ToList();
                    decimal ConsDis = (ConsDislist.Sum(m => m.ReceiptAmount)) ?? 0;
                    ConsDislt.Add(ConsDis);
                    decimal ConsRec = ((ConsReclist.Sum(m => m.ReceiptAmount)) + ConsOHlist.Sum(m => m.ReceiptAmount)) ?? 0;
                    ConsReclt.Add(ConsRec);
                    string[] SponOHArray = SponOHlist.Select(m => m.ReceiptNumber).ToArray();
                    string[] ConsOHArray = ConsOHlist.Select(m => m.ReceiptNumber).ToArray();
                    var OtherReclist = context.tblReceipt.Where(m => m.CrtdTS >= FromDate && m.Posted_f == true && m.CrtdTS <= ToDate && ProjectIdArr.Contains(m.ProjectId ?? 0)
                     && m.Status == "Completed" && !m.Description.StartsWith("RCV/")
                    && !m.Description.StartsWith("DOP/") && !m.Description.StartsWith("HON/") && !m.Description.StartsWith("DIS/")
                     && !SponOHArray.Contains(m.ReceiptNumber) && !ConsOHArray.Contains(m.ReceiptNumber)).ToList();
                    decimal OtherRec = OtherReclist.Sum(m => m.ReceivedAmount) ?? 0;
                    OtherReclt.Add(OtherRec);
                    decimal TotalIncome = (Inter + SponRec + ConsDis + ConsRec + OtherRec);
                    TotalIncomelt.Add(TotalIncome);

                    int[] interAlloId = { 19, 29 };
                    int[] interNotAlloId = { 1, 2, 7, 19, 29 };
                    decimal Salary = context.vw_ProjectExpenditureReport.AsNoTracking().Where(m => m.CommitmentDate >= FromDate && m.CommitmentDate <= ToDate && m.Posted_f == true && ProjectIdArr.Contains(m.ProjectId ?? 0) && m.AllocationHeadId == 1).Sum(m => m.AmountSpent) ?? 0;
                    Salarylt.Add(Salary);
                    decimal InterestExp = context.vw_ProjectExpenditureReport.AsNoTracking().Where(m => m.CommitmentDate >= FromDate && m.CommitmentDate <= ToDate && m.Posted_f == true && ProjectIdArr.Contains(m.ProjectId ?? 0) && interAlloId.Contains(m.AllocationHeadId ?? 0)).Sum(m => m.AmountSpent) ?? 0;
                    InterestExplt.Add(InterestExp);
                    decimal RepairsMaint = context.vw_ProjectExpenditureReport.AsNoTracking().Where(m => m.CommitmentDate >= FromDate && m.CommitmentDate <= ToDate && m.Posted_f == true && ProjectIdArr.Contains(m.ProjectId ?? 0) && m.AllocationHeadId == 2).Sum(m => m.AmountSpent) ?? 0;
                    RepairsMaintlt.Add(RepairsMaint);
                    decimal ITEquipment = context.vw_ProjectExpenditureReport.AsNoTracking().Where(m => m.CommitmentDate >= FromDate && m.CommitmentDate <= ToDate && m.Posted_f == true && ProjectIdArr.Contains(m.ProjectId ?? 0) && m.AllocationHeadId == 7).Sum(m => m.AmountSpent) ?? 0;
                    ITEquipmentlt.Add(ITEquipment);
                    decimal OtherExpenses = context.vw_ProjectExpenditureReport.AsNoTracking().Where(m => m.CommitmentDate >= FromDate && m.CommitmentDate <= ToDate && m.Posted_f == true && ProjectIdArr.Contains(m.ProjectId ?? 0) && !interNotAlloId.Contains(m.AllocationHeadId ?? 0)).Sum(m => m.AmountSpent) ?? 0;
                    OtherExpenseslt.Add(OtherExpenses);
                    decimal TotalExpenses = (Salary + InterestExp + RepairsMaint + ITEquipment + OtherExpenses);
                    TotalExpenseslt.Add(TotalExpenses);

                    decimal NetIncome = (TotalIncome - TotalExpenses);
                    NetIncomelt.Add(NetIncome);
                    //}

                    model1.Interest = Intereslt;
                    model.Interest = Intereslt.Sum();

                    model1.SPONCorpus = SponReclt;
                    model.SPONCorpus = SponReclt.Sum();

                    model1.CONSDist = ConsDislt;
                    model.CONSDist = ConsDislt.Sum();

                    model1.CONSCorpus = ConsReclt;
                    model.CONSCorpus = ConsReclt.Sum();

                    model1.OtherIncome = OtherReclt;
                    model.OtherIncome = OtherReclt.Sum();

                    model1.Salary = Salarylt;
                    model.Salary = Salarylt.Sum();

                    model1.InterestExp = InterestExplt;
                    model.InterestExp = InterestExplt.Sum();

                    model1.RepairsMaint = RepairsMaintlt;
                    model.RepairsMaint = RepairsMaintlt.Sum();

                    model1.ITEquipment = ITEquipmentlt;
                    model.ITEquipment = ITEquipmentlt.Sum();

                    model1.OtherExpenses = OtherExpenseslt;
                    model.OtherExpenses = OtherExpenseslt.Sum();

                    model1.TotalExpenses = TotalExpenseslt;
                    model.TotalExpenses = TotalExpenseslt.Sum();

                    model1.TotalIncome = TotalIncomelt;
                    model.TotalIncome = TotalIncomelt.Sum();

                    model1.NetIncome = NetIncomelt;
                    model.NetIncome = NetIncomelt.Sum();

                    model.ICSROHlist = model1;
                    return model;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
                  (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return model;
            }
        }
        #endregion

        #region Admin Voucher Report
        public TravelBillReportModel GetAdminVoucherBillReport(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblAdminVoucher.Where(m => m.AdminVoucherId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.BankAccount = Common.GetBankName(Convert.ToInt32(Qry.BankHeadId));
                    model.BillId = Id;
                    model.TotalBillValue = Convert.ToDecimal(Qry.Amount);
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.AdminVoucherNumber;
                    model.BillType = "Admin Voucher";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    List<PayableListModel> PayList = new List<PayableListModel>();
                    var Pay = (from e in context.tblAdminVoucher
                               join f in context.tblAdminVoucherPaymentBreakUpDetail on e.AdminVoucherId equals f.AdminVoucherId
                               where e.AdminVoucherId == Id
                               select new
                               {
                                   f.PaymentAmount,
                                   f.Name
                               }).ToList();
                    for (int i = 0; i < Pay.Count; i++)
                    {
                        PayList.Add(new PayableListModel()
                        {
                            Name = Pay[i].Name,
                            Amount = Pay[i].PaymentAmount ?? 0
                        });
                    }
                    model.Payable = PayList;


                    int[] id = { 44, 45, 46 };

                    model.TDSGST = (from e in context.tblAdminVoucherExpenseDetail
                                    join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                    where e.AdminVoucherId == Id && (id.Contains(e.AccountHeadId ?? 0)) && e.Amount > 0
                                    select new
                                    {
                                        e.Amount,
                                        f.AccountHead
                                    }).AsEnumerable()
                                          .Select((x) => new TDSGSTForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();

                    int[] IT = { 39, 40, 41, 42, 43 };

                    model.TDSIT = (from e in context.tblAdminVoucherExpenseDetail
                                   join f in context.tblAccountHead on e.AccountHeadId equals f.AccountHeadId
                                   where e.AdminVoucherId == Id
                                   //&& (IT.Contains(e.AccountHeadId ?? 0)) 
                                   && e.Amount > 0
                                   && e.AccountHeadId != 135 && e.AccountGroupId == 15
                                   select new
                                   {
                                       e.Amount,
                                       f.AccountHead
                                   }).AsEnumerable()
                                          .Select((x) => new TDSITListForTravelModel()
                                          {
                                              Head = x.AccountHead,
                                              Value = Convert.ToDecimal(x.Amount)
                                          }).ToList();
                    model.PayableAmount = Qry.Amount ?? 0;
                    model.TotalAmount = Qry.Amount ?? 0;
                    model.Rupees = CoreAccountsService.words(Qry.Amount);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.Remarks = Qry.Remarks;
                    model.TravelerName = Common.GetUserNameBasedonId(Convert.ToInt32(Qry.CRTD_By));
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region Mandays Bill Report
        public TravelBillReportModel GetMandaysBillReport(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblManDay.Where(m => m.ManDayId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    var query = (from e in context.tblManDayCommitmentDetail
                                 join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                 join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                 join h in context.tblProject on g.ProjectId equals h.ProjectId
                                 join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                 where e.ManDayId == Id && e.Amount > 0
                                 select new
                                 {
                                     h.ProjectType,
                                     h.ProjectId,
                                     h.SponProjectCategory
                                 }).ToList();
                    if (query != null)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            if (query[i].ProjectType == 2)
                            {
                                model.BillHeading = "Consultancy";
                            }
                            if (query[i].ProjectType == 1)
                            {
                                var proid = query[i].ProjectId;
                                var head = context.tblProject.Where(m => m.ProjectId == proid).Select(m => m.SponProjectCategory).FirstOrDefault();
                                if (head == "1")
                                {
                                    model.BillHeading = "Sponsored-PFMS";
                                    break;
                                }
                                if (head == "2")
                                {
                                    model.BillHeading = "Sponsored-NON-PFMS";

                                }
                            }
                            if (query[i].ProjectId == 2067)
                            {
                                model.BillHeading = "ICSR Over Head";
                            }
                        }
                    }
                    int[] headid = { 38, 61 };
                    var BankHeadId = (from e in
                       context.tblManDayExpenseDetail
                                      where (headid.Contains(e.AccountGroupId ?? 0) && e.ManDayId == Id && e.TransactionType == "Credit")
                                      select new { e.AccountHeadId }).FirstOrDefault();
                    if (BankHeadId != null)
                        model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == BankHeadId.AccountHeadId).Select(m => m.AccountHead).FirstOrDefault();
                    model.BillId = Id;
                    model.TotalBillValue = Convert.ToDecimal(Qry.Amount);
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.ManDayNumber;
                    model.BillType = "Mandays";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.Reason = Qry.Remark;
                    List<PayableListModel> PayList = new List<PayableListModel>();

                    var Pay = (from e in context.tblManDay
                               join f in context.tblManDayDetails on e.ManDayId equals f.ManDayId
                               where e.ManDayId == Id
                               select new
                               {
                                   f.StaffName,
                                   f.NoofDays,
                                   f.AmountPerDay,
                                   f.TotalAmount,
                                   f.ModeOfPayment
                               }).ToList();

                    if (Pay != null)
                    {

                        for (int i = 0; i < Pay.Count; i++)
                        {
                            int PaymentMode = Convert.ToInt32(Pay[i].ModeOfPayment);
                            PayList.Add(new PayableListModel()
                            {
                                Name = Pay[i].StaffName,
                                AmountPerdays = Pay[i].AmountPerDay ?? 0,
                                Amount = Convert.ToDecimal(Pay[i].TotalAmount),
                                NoOfDays = Pay[i].NoofDays ?? 0,
                                PaymentMode = Common.GetCodeControlnameCommon(PaymentMode, "PaymentModeManDay"),
                            });
                        }
                    }

                    model.Payable = PayList;
                    decimal? PayableAmt = 0;

                    PayableAmt = Qry.Amount;

                    model.Comm = (from e in context.tblManDayCommitmentDetail
                                  join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.ManDayId == Id && e.Amount > 0
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.Amount,
                                      h.ProjectId
                                  }).AsEnumerable()
                               .Select((x) => new CommitListModel()
                               {
                                   Number = x.CommitmentNumber,
                                   ProjNo = x.ProjectNumber,
                                   ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                                   NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                                   Date = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                                   StartDate = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectStartDate(x.ProjectId)),
                                   SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                                   Head = x.HeadName,
                                   Value = Convert.ToDecimal(x.Amount)
                               }).ToList();

                    model.PayableAmount = PayableAmt ?? 0;
                    model.TotalAmount = model.PayableAmount;
                    model.Rupees = CoreAccountsService.words(model.PayableAmount);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.Remarks = Qry.Remark;
                    model.TravelerName = Common.GetUserNameBasedonId(Convert.ToInt32(Qry.CRTD_By));
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region AdminSalary Bill Report
        public TravelBillReportModel GetAdminSalaryBillReport(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblInstituteSalaryPayment.Where(m => m.InstituteSalaryPaymentId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);

                    int[] headid = { 38, 61 };
                    var BankHeadId = (from e in
                       context.tblInstituteSalaryPaymentExpenseDetail
                                      where (headid.Contains(e.AccountGroupId ?? 0) && e.InstituteSalaryPaymentId == Id && e.TransactionType == "Credit")
                                      select new { e.AccountHeadId }).FirstOrDefault();
                    if (BankHeadId != null)
                        model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == BankHeadId.AccountHeadId).Select(m => m.AccountHead).FirstOrDefault();
                    model.BillId = Id;
                    model.TotalBillValue = Convert.ToDecimal(Qry.Amount);
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.InstituteSalaryPaymentNumber;
                    model.BillType = "Admin Salary";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.Reason = Qry.Remarks;
                    List<PayableListModel> PayList = new List<PayableListModel>();

                    var Pay = (from e in context.tblInstituteSalaryPayment
                               join f in context.tblInstituteSalaryPaymentDetails on e.InstituteSalaryPaymentId equals f.InstituteSalaryPaymentId
                               where e.InstituteSalaryPaymentId == Id
                               select new
                               {
                                   f.Category,
                                   f.Name,
                                   f.Amount,
                                   f.Type,
                                   f.UserId

                               }).ToList();

                    if (Pay != null)
                    {

                        for (int i = 0; i < Pay.Count; i++)
                        {
                            int userid = Pay[i].UserId ?? 0;
                            PayList.Add(new PayableListModel()
                            {
                                Category = Pay[i].Category,
                                Name = Pay[i].Name,
                                Amount = Convert.ToDecimal(Pay[i].Amount),
                                PaybillNo = Common.GetPayBillNo(userid),
                                Type = Pay[i].Type,
                            });
                        }
                    }

                    model.Payable = PayList;
                    decimal? PayableAmt = 0;

                    PayableAmt = Qry.Amount;

                    var ExpQry = context.tblInstituteSalaryPaymentExpenseDetail.Where(m => m.InstituteSalaryPaymentId == Id).ToList();
                    model.HonSalary = ExpQry.Where(m => m.AccountHeadId == 316).Select(m => m.Amount).FirstOrDefault() ?? 0;
                    model.MdySalary = ExpQry.Where(m => m.AccountHeadId == 318).Select(m => m.Amount).FirstOrDefault() ?? 0;
                    model.FssSalary = ExpQry.Where(m => m.AccountHeadId == 321).Select(m => m.Amount).FirstOrDefault() ?? 0;
                    model.DisSalary = ExpQry.Where(m => m.AccountHeadId == 317).Select(m => m.Amount).FirstOrDefault() ?? 0;
                    model.PayableAmount = PayableAmt ?? 0;
                    model.TotalAmount = model.PayableAmount;
                    model.Rupees = CoreAccountsService.words(model.PayableAmount);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.Remarks = Qry.Remarks;
                    model.PrintedBy = Common.GetUserNameBasedonId(Convert.ToInt32(Qry.CRTD_By));
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region Fellowship Bill Report
        public TravelBillReportModel GetFellowshipBillReport(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblFellowshipSalary.Where(m => m.FellowshipSalaryId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);

                    int[] headid = { 38, 61 };
                    var BankHeadId = (from e in
                       context.tblFellowshipSalaryExpenseDetail
                                      where (headid.Contains(e.AccountGroupId ?? 0) && e.FellowshipSalaryId == Id && e.TransactionType == "Credit")
                                      select new { e.AccountHeadId }).FirstOrDefault();
                    if (BankHeadId != null)
                        model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == BankHeadId.AccountHeadId).Select(m => m.AccountHead).FirstOrDefault();
                    model.BillId = Id;
                    model.TotalBillValue = Convert.ToDecimal(Qry.Amount);
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.FellowshipSalaryNumber;
                    model.BillType = "Fellowship";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.Reason = Qry.Remark;
                    List<PayableListModel> PayList = new List<PayableListModel>();

                    var Pay = (from e in context.tblFellowshipSalary
                               join f in context.tblFellowshipSalaryDetails on e.FellowshipSalaryId equals f.FellowshipSalaryId
                               where e.FellowshipSalaryId == Id && f.Amount > 0
                               select new
                               {
                                   f.UserId,
                                   f.Amount,


                               }).ToList();

                    if (Pay != null)
                    {
                        for (int i = 0; i < Pay.Count; i++)
                        {
                            int Userid = Pay[i].UserId ?? 0;
                            PayList.Add(new PayableListModel()
                            {
                                Name = Common.GetPIName(Userid),

                                Amount = Convert.ToDecimal(Pay[i].Amount),

                            });
                        }
                    }

                    model.Payable = PayList;
                    decimal? PayableAmt = 0;

                    PayableAmt = Qry.Amount;

                    model.Comm = (from e in context.tblFellowshipSalaryDetails
                                  join f in context.tblCommitmentDetails on e.CommitmentId equals f.CommitmentId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.FellowshipSalaryId == Id && e.Amount > 0
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.Amount,
                                      h.ProjectId
                                  }).AsEnumerable()
                               .Select((x) => new CommitListModel()
                               {
                                   Number = x.CommitmentNumber,
                                   ProjNo = x.ProjectNumber,
                                   ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                                   NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                                   Date = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                                   StartDate = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectStartDate(x.ProjectId)),
                                   SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                                   Head = x.HeadName,
                                   Value = Convert.ToDecimal(x.Amount)
                               }).ToList();

                    model.PayableAmount = PayableAmt ?? 0;
                    model.TotalAmount = model.PayableAmount;
                    model.Rupees = CoreAccountsService.words(model.PayableAmount);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.Remarks = Qry.Remark;
                    model.PrintedBy = Common.GetUserNameBasedonId(Convert.ToInt32(Qry.CRTD_By));
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region Distribution Bill Report
        public TravelBillReportModel GetDistributionBillReport(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblDistribution.Where(m => m.DistributionId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    var query = (from e in context.tblDistributionCommitmentDetail
                                 join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                 join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                 join h in context.tblProject on g.ProjectId equals h.ProjectId
                                 join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                 where e.DistributionId == Id
                                 select new
                                 {
                                     h.ProjectType,
                                     h.ProjectId,
                                     h.SponProjectCategory
                                 }).ToList();
                    if (query != null)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            if (query[i].ProjectType == 2)
                            {
                                model.BillHeading = "Consultancy";
                            }
                            if (query[i].ProjectType == 1)
                            {
                                var proid = query[i].ProjectId;
                                var head = context.tblProject.Where(m => m.ProjectId == proid).Select(m => m.SponProjectCategory).FirstOrDefault();
                                if (head == "1")
                                {
                                    model.BillHeading = "Sponsored-PFMS";
                                    break;
                                }
                                if (head == "2")
                                {
                                    model.BillHeading = "Sponsored-NON-PFMS";

                                }
                            }
                            if (query[i].ProjectId == 2067)
                            {
                                model.BillHeading = "ICSR Over Head";
                            }
                        }
                    }
                    int[] headid = { 38, 61 };
                    var BankHeadId = (from e in
                       context.tblDistributionExpenseDetail
                                      where (headid.Contains(e.AccountGroupId ?? 0) && e.DistributionId == Id && e.TransactionType == "Credit")
                                      select new { e.AccountHeadId }).FirstOrDefault();
                    if (BankHeadId != null)
                        model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == BankHeadId.AccountHeadId).Select(m => m.AccountHead).FirstOrDefault();
                    model.BillId = Id;
                    model.TotalBillValue = Convert.ToDecimal(Qry.DistributionAmount);
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.DistributionNumber;
                    model.BillType = "Distribution";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.Reason = Qry.Remarks;
                    model.EucoCode = Qry.LabCode;
                    model.EucoValue = Qry.EUCOAmount ?? 0;
                    model.Others = Qry.OthersAmount ?? 0;
                    model.ProjectNumber = context.tblProject.Where(m => m.ProjectId == Qry.ProjectId).Select(m => m.ProjectNumber).FirstOrDefault();
                    model.ProjCloseDate = String.Format("{0:dd-MMMM-yyyy}", Common.GetProjectCloseDate(Qry.ProjectId ?? 0));
                    List<PayableListModel> PayList = new List<PayableListModel>();
                    List<HonororiumPCFModel> PCFList = new List<HonororiumPCFModel>();
                    List<TDSPerModel> TDSList = new List<TDSPerModel>();
                    var Pay = (from e in context.tblDistribution
                               join f in context.tblDistributionDetails on e.DistributionId equals f.DistributionId
                               where e.DistributionId == Id
                               select new
                               {
                                   f.Name,
                                   f.Amount,
                                   f.UserID,
                                   f.PaymentMode,
                                   f.Category

                               }).ToList();
                    var PCF = (from e in context.tblDistribution
                               join f in context.tblDistributionPCFDetails on e.DistributionId equals f.DistributionId
                               where e.DistributionId == Id
                               select new
                               {
                                   f
                               }).ToList();
                    var OhQry = context.tblDistributeInstituteOHBreakup.Where(m => m.DistributionId == Id).ToList();
                    if (OhQry != null)
                    {
                        for (int i = 0; i < OhQry.Count; i++)
                        {

                            TDSList.Add(new TDSPerModel()
                            {
                                PayeeName = OhQry[i].InstituteOverheadType,
                                TDSAmt = OhQry[i].InstituteOverheadAmount ?? 0,
                                TDSPer = Convert.ToString(OhQry[i].InstituteOverheadPercentage),
                            });
                        }
                    }
                    model.TDSPerList = TDSList;

                    if (PCF != null)
                    {
                        for (int i = 0; i < PCF.Count; i++)
                        {

                            PCFList.Add(new HonororiumPCFModel()
                            {
                                PCFName = PCF[i].f.PCFPIName,
                                OHDropdown = PCF[i].f.PIPCFId,
                                PCFAmount = PCF[i].f.PCFAmount,

                            });
                        }
                    }

                    model.PCF = PCFList;
                    if (Pay != null)
                    {

                        for (int i = 0; i < Pay.Count; i++)
                        {
                            int catid = Convert.ToInt32(Pay[i].Category);
                            int userid = Convert.ToInt32(Pay[i].UserID);
                            int PaymentMode = Convert.ToInt32(Pay[i].UserID);
                            PayList.Add(new PayableListModel()
                            {
                                Name = Pay[i].Name + "-" + Common.GetCodeControlnameCommon(catid, "FacultyType"),
                                PaymentMode = Common.GetCodeControlnameCommon(PaymentMode, "DistributionPaymentMode"),
                                Amount = Convert.ToDecimal(Pay[i].Amount),
                                PaybillNo = Common.GetPayBillNo(userid),
                            });
                        }
                    }

                    model.Payable = PayList;
                    decimal? PayableAmt = 0;

                    PayableAmt = Qry.DistributionAmount;

                    model.Comm = (from e in context.tblDistributionCommitmentDetail
                                  join f in context.tblCommitmentDetails on e.CommitmentDetailId equals f.ComitmentDetailId
                                  join g in context.tblCommitment on f.CommitmentId equals g.CommitmentId
                                  join h in context.tblProject on g.ProjectId equals h.ProjectId
                                  join i in context.tblBudgetHead on f.AllocationHeadId equals i.BudgetHeadId
                                  where e.DistributionId == Id
                                  select new
                                  {
                                      g.CommitmentNumber,
                                      h.ProjectNumber,
                                      i.HeadName,
                                      e.PaymentAmount,
                                      h.ProjectId
                                  }).AsEnumerable()
                               .Select((x) => new CommitListModel()
                               {
                                   Number = x.CommitmentNumber,
                                   ProjNo = x.ProjectNumber,
                                   ProjectType = Common.GetProjectTypeForBill(x.ProjectId),
                                   NetBalance = GetProvisionalStatement(FromDate, ToDate, x.ProjectId).NetBalance,
                                   Date = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectCloseDate(x.ProjectId)),
                                   StartDate = string.Format("{0:dd-MMM-yyyy}", Common.GetProjectStartDate(x.ProjectId)),
                                   SchemeCode = Common.GetSchemeCodeForBill(x.ProjectId),
                                   Head = x.HeadName,
                                   Value = Convert.ToDecimal(x.PaymentAmount)
                               }).ToList();

                    model.PayableAmount = PayableAmt ?? 0;
                    model.TotalAmount = model.PayableAmount;
                    model.Rupees = CoreAccountsService.words(model.PayableAmount);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.Remarks = Qry.Remarks;
                    model.TravelerName = Common.GetUserNameBasedonId(Convert.ToInt32(Qry.CRTD_By));
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        #region Old Receipt and Exp
        public static DataTable GetOldreceipts(int ProjectId)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = Common.getExternalConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    if (ProjectId > 0)
                        command.CommandText = "select * from tblOldreceipts where ProjectId =" + ProjectId;
                    else
                        command.CommandText = "select * from tblOldreceipts ";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    adapter.SelectCommand.CommandTimeout = 1800;
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];

                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
    (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        public static DataTable GetOldExpenditure(int ProjectId)
        {
            DataTable dtColumns = new DataTable();
            try
            {

                using (var connection = Common.getExternalConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    if (ProjectId > 0)
                        command.CommandText = "select * from tblOldExpenditure where ProjectId =" + ProjectId;
                    else
                        command.CommandText = "select * from tblOldExpenditure ";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    adapter.SelectCommand.CommandTimeout = 1800;
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];

                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
    (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw ex;
            }
        }
        #endregion

        #region TDSPayment Bill Report
        public TravelBillReportModel GetTdsPaymentBillReport(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblTDSPayment.Where(m => m.tblTDSPaymentId == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);

                    int[] headid = { 38, 61 };
                    var BankHeadId = Qry.PaymentBank ?? Qry.BankId;
                    if (BankHeadId != null)
                        model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == BankHeadId).Select(m => m.AccountHead).FirstOrDefault();
                    model.BillId = Id;
                    decimal Amt = 0;
                    if (Qry.Category == 1)
                        Amt = Qry.BankTransForIncometax ?? 0;
                    else
                        Amt = Qry.BankTransForGST ?? 0;
                    model.TotalBillValue = Convert.ToDecimal(Amt);
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.TDSPaymentNumber;
                    model.BillType = "Tax Voucher";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.Reason = Qry.Remark;
                    model.BillHeading = Qry.Category == 1 ? "TDS-IT" : "TDS-GST";
                    model.BSRCode = Qry.BankReferenceNo;
                    model.CheqNo = Qry.ChellanNo;
                    model.CheqDate = String.Format("{0:dd-MMMM-yyyy}", Qry.DateOfPayment);
                    if (Qry.Category == 1)
                        model.TDSSection = context.tblAccountHead.Where(m => m.AccountHeadId == Qry.Section).Select(m => m.AccountHead).FirstOrDefault();
                    else
                        model.TDSSection = "TDS-GST";
                    decimal? PayableAmt = 0;
                    PayableAmt = Amt;
                    model.PayableAmount = PayableAmt ?? 0;
                    model.TotalAmount = model.PayableAmount;
                    model.Rupees = CoreAccountsService.words(model.PayableAmount);
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.Remarks = Qry.Remark;
                    model.TravelerName = Common.GetUserNameBasedonId(Convert.ToInt32(Qry.CRTD_By));
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion
        public static Tuple<string, int, int, int> CheckInterestRefundRunning(int FinId)
        {
            ListDatabaseObjects listdata = new ListDatabaseObjects();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var FromDate = context.tblFinYear.Where(m => m.FinYearId == FinId).Select(m => m.StartDate).FirstOrDefault() ?? DateTime.Now;

                    var Qry = context.tblInterestrefundSPprocess.Where(m => m.FinYear == FinId).FirstOrDefault();
                    if (Qry != null)
                    {
                        int balance = (Qry.ProjectCount - Qry.ExecutedCount) ?? 0;
                        return Tuple.Create(Qry.Status, Qry.ProjectCount ?? 0, Qry.ExecutedCount ?? 0, balance);
                    }
                    else
                    {
                        int count = listdata.CheckInterestRefundCount(FinId, FromDate);
                        return Tuple.Create("Not Running", count, 0, 0);
                    }

                }
            }
            catch (Exception ex)
            {
                return Tuple.Create((string)"NA", 0, 0, 0);
            }
        }

        #region Balance Sheet
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public class m1
        {
            public string AccountHead { get; set; }
            public decimal Credit { get; set; }
            public decimal Debit { get; set; }
        }
        //public static BalanceSheetModel GetBalanceSheetNote(int[] AccGrp, int[] AccHed, bool Asset_f, DateTime Date)
        //{
        //    BalanceSheetModel model = new BalanceSheetModel();
        //    List<BalanceSheetNoteModel> Note = new List<BalanceSheetNoteModel>();
        //    decimal CurrGrossAmt = 0;
        //    string CurrstrGrossAmt = "";
        //    try
        //    {
        //        using (var context = new IOASDBEntities())
        //        {

        //            Date = Date.AddDays(1).AddTicks(-10001);

        //            //int[] AccHeadId = (from a in context.tblAccountHead
        //            //                   where (AccGrp.Contains(a.AccountGroupId ?? 0) || AccHed.Contains(a.AccountHeadId))
        //            //                   select a.AccountHeadId).ToArray();

        //            //  DateTime StartDate = new DateTime(2020, 4, 1);

        //            if (Asset_f == true)
        //            {
        //                Note = (from a in context.tblAccountHead
        //                        join b in context.vw_BalanceSheet on a.AccountHeadId equals b.AccountHeadId
        //                        where (AccGrp.Contains(a.AccountGroupId ?? 0) || AccHed.Contains(a.AccountHeadId)) && b.PostedDate <= Date
        //                        //   && b.PostedDate >= StartDate
        //                        group b by b.AccountHead into g
        //                        select new { Debit = g.Sum(m => m.Debit), Credit = g.Sum(m => m.Credit), AccountHead = g.Key })
        //                                  .AsEnumerable()
        //                                      .Select((x) => new BalanceSheetNoteModel()
        //                                      {
        //                                          Head = x.AccountHead,
        //                                          CurrAmount = (x.Debit - x.Credit) < 0 ? "(" + Convert.ToString((-(x.Debit - x.Credit))) + ")" : Convert.ToString((x.Debit - x.Credit)),
        //                                          DecimalCurrAmount = (x.Debit - x.Credit) ?? 0
        //                                      }).ToList();
        //            }

        //            else
        //            {
        //                Note = (from a in context.tblAccountHead
        //                        join b in context.vw_BalanceSheet on a.AccountHeadId equals b.AccountHeadId
        //                        where (AccGrp.Contains(a.AccountGroupId ?? 0) || AccHed.Contains(a.AccountHeadId))
        //                        && b.PostedDate <= Date
        //                        //  && b.PostedDate >= StartDate
        //                        group b by b.AccountHead into g
        //                        select new { Debit = g.Sum(m => m.Debit), Credit = g.Sum(m => m.Credit), AccountHead = g.Key })
        //              .AsEnumerable()
        //                  .Select((x) => new BalanceSheetNoteModel()
        //                  {
        //                      Head = x.AccountHead,
        //                      CurrAmount = (x.Credit - x.Debit) < 0 ? "(" + Convert.ToString((-(x.Credit - x.Debit))) + ")" : Convert.ToString((x.Credit - x.Debit)),
        //                      DecimalCurrAmount = (x.Credit - x.Debit) ?? 0
        //                  }).ToList();

        //            }


        //            //for (int j = 0; j < AccHeadId.Length; j++)
        //            //{
        //            //    int id = AccHeadId[j];
        //            //    string HeadName = "";
        //            //    HeadName = context.tblAccountHead.Where(m => m.AccountHeadId == id).Select(m => m.AccountHead).FirstOrDefault();
        //            //   decimal CurrAmt = 0;

        //            //    if (Asset_f == true)
        //            //        CurrAmt = context.vw_BalanceSheet.Where(m => m.AccountHeadId == id
        //            //      && m.PostedDate <= Date).ToList().Sum(m => { return (m.Debit - m.Credit); })??0;
        //            //    else
        //            //        CurrAmt = context.vw_BalanceSheet.Where(m => m.AccountHeadId == id
        //            //       && m.PostedDate <= Date).ToList().Sum(m => { return (m.Credit - m.Debit); }) ?? 0;


        //            //    CurrGrossAmt += CurrAmt;
        //            //    string CurrstrAmt = "";
        //            //    if (CurrAmt > 0)
        //            //        CurrstrAmt = Convert.ToString(CurrAmt);
        //            //    else if (CurrAmt == 0)
        //            //        CurrstrAmt = "-";
        //            //    else
        //            //    {
        //            //        CurrAmt = -CurrAmt;
        //            //        CurrstrAmt = "(" + CurrAmt + ")";
        //            //    }



        //            //    Note.Add(new BalanceSheetNoteModel()
        //            //    {
        //            //        Head = HeadName,

        //            //        CurrAmount = CurrstrAmt,
        //            //    });
        //            //}
        //            CurrGrossAmt = Note.Sum(m => m.DecimalCurrAmount);
        //            model.CurrDecGrossAmt = CurrGrossAmt;


        //            if (CurrGrossAmt > 0)
        //                CurrstrGrossAmt = Convert.ToString(CurrGrossAmt);
        //            else if (CurrGrossAmt == 0)
        //                CurrstrGrossAmt = "-";
        //            else
        //            {
        //                CurrGrossAmt = -CurrGrossAmt;
        //                CurrstrGrossAmt = "(" + CurrGrossAmt + ")";
        //            }

        //            model.CurrGrossAmt = CurrstrGrossAmt;
        //            model.Note = Note;
        //            return model;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Infrastructure.IOASException.Instance.HandleMe(
        //          (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
        //        return model;
        //    }
        //}
        public static BalanceSheetModel GetRetainedEarnings(DateTime Date)
        {
            BalanceSheetModel model = new BalanceSheetModel();
            List<BalanceSheetNoteModel> Note = new List<BalanceSheetNoteModel>();
            decimal GrossAmt = 0;
            string strGrossAmt = "";
            try
            {
                using (var context = new IOASDBEntities())
                {
                    Date = Date.AddDays(1).AddTicks(-10001);
                    DateTime StartDate = new DateTime(2020, 4, 1);
                    decimal Income = context.vw_ProfitandLoss.Where(m => m.Accounts == "Income" && m.PostedDate >= StartDate && m.PostedDate <= Date).ToList().Sum(m => { return (m.Credit - m.Debit); }) ?? 0;
                    decimal Expense = context.vw_ProfitandLoss.Where(m => m.Accounts == "Expense" && m.PostedDate >= StartDate && m.PostedDate <= Date).ToList().Sum(m => { return (m.Debit - m.Credit); }) ?? 0;
                    GrossAmt = Income - Expense;
                    model.DecGrossAmt = GrossAmt;
                    if (GrossAmt > 0)
                        strGrossAmt = Convert.ToString(GrossAmt);
                    else if (GrossAmt == 0)
                        strGrossAmt = "-";
                    else
                    {
                        GrossAmt = -GrossAmt;
                        strGrossAmt = "(" + GrossAmt + ")";
                    }

                }
                model.GrossAmt = strGrossAmt;
                return model;
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
                  (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return model;
            }
        }

        //public static BalanceSheetModel GetProjectFund(DateTime Date)
        //{
        //    BalanceSheetModel model = new BalanceSheetModel();
        //    List<BalanceSheetNoteModel> Note = new List<BalanceSheetNoteModel>();
        //    decimal GrossAmt = 0;
        //    string strGrossAmt = "";
        //    try
        //    {
        //        using (var context = new IOASDBEntities())
        //        {
        //            Date = Date.AddDays(1).AddTicks(-10001);
        //            decimal Receipt = 0;
        //            var RecQry = context.vw_ReceiptsandPayments.Where(m => m.PostedDate <= Date).ToList();
        //            Receipt = RecQry.Sum(m => { return (m.Credit - m.Debit); }) ?? 0;
        //            decimal Payment = 0;

        //            DateTime StartDate = new DateTime(2020, 4, 1);
        //            //  var BOANumber = context.tblBOA.Where(m => m.PostedDate >= StartDate && m.Status=="Posted"&& m.PostedDate <= Date).Select(m=> new { m.RefNumber }).Distinct().ToList();
        //            string[] BOANumber = context.tblBOA.Where(m => m.PostedDate >= StartDate && m.Status == "Posted" && m.PostedDate <= Date).Select(m => m.RefNumber).Distinct().ToArray();


        //            var PayQry = (from a in context.vw_ProjectExpenditureReport
        //                              // join b in BOANumber on a.BillNumber equals b.RefNumber
        //                          where a.ProjectClassification != 4 && a.ProjectClassification != 6
        //                          && a.Posted_f == true && BOANumber.Contains(a.BillNumber)
        //                          select a).ToList();

        //            Payment = PayQry.Sum(m => m.AmountSpent) ?? 0;
        //            GrossAmt = Receipt - Payment;
        //            model.DecGrossAmt = GrossAmt;
        //            if (GrossAmt > 0)
        //                strGrossAmt = Convert.ToString(GrossAmt);
        //            else if (GrossAmt == 0)
        //                strGrossAmt = "-";
        //            else
        //            {
        //                GrossAmt = -GrossAmt;
        //                strGrossAmt = "(" + GrossAmt + ")";
        //            }
        //        }
        //        model.GrossAmt = strGrossAmt;
        //        return model;
        //    }
        //    catch (Exception ex)
        //    {
        //        Infrastructure.IOASException.Instance.HandleMe(
        //          (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
        //        return model;
        //    }
        //}

        public static BalanceSheetModel GetBalanceSheetNote(int[] AccGrp, int[] AccHed, bool Asset_f, DateTime FromDate, DateTime ToDate)
        {
            BalanceSheetModel model = new BalanceSheetModel();
            List<BalanceSheetNoteModel> Note = new List<BalanceSheetNoteModel>();
            decimal CurrGrossAmt = 0;
            string CurrstrGrossAmt = "";
            try
            {
                using (var context = new IOASDBEntities())
                {

                    ToDate = ToDate.AddDays(1).AddTicks(-10001);


                    if (Asset_f == true)
                    {
                        Note = (from a in context.tblAccountHead
                                join b in context.vw_BalanceSheet on a.AccountHeadId equals b.AccountHeadId
                                where (AccGrp.Contains(a.AccountGroupId ?? 0) || AccHed.Contains(a.AccountHeadId)) && b.PostedDate <= ToDate
                              && b.PostedDate >= FromDate
                                group b by b.AccountHead into g
                                select new { Debit = g.Sum(m => m.Debit), Credit = g.Sum(m => m.Credit), AccountHead = g.Key })
                                          .AsEnumerable()
                                              .Select((x) => new BalanceSheetNoteModel()
                                              {
                                                  Head = x.AccountHead,
                                                  CurrAmount = (x.Debit - x.Credit) < 0 ? "(" + Convert.ToString((-(x.Debit - x.Credit))) + ")" : Convert.ToString((x.Debit - x.Credit)),
                                                  DecimalCurrAmount = (x.Debit - x.Credit) ?? 0
                                              }).ToList();
                    }

                    else
                    {
                        Note = (from a in context.tblAccountHead
                                join b in context.vw_BalanceSheet on a.AccountHeadId equals b.AccountHeadId
                                where (AccGrp.Contains(a.AccountGroupId ?? 0) || AccHed.Contains(a.AccountHeadId))
                                && b.PostedDate <= ToDate
                                && b.PostedDate >= FromDate
                                group b by b.AccountHead into g
                                select new { Debit = g.Sum(m => m.Debit), Credit = g.Sum(m => m.Credit), AccountHead = g.Key })
                      .AsEnumerable()
                          .Select((x) => new BalanceSheetNoteModel()
                          {
                              Head = x.AccountHead,
                              CurrAmount = (x.Credit - x.Debit) < 0 ? "(" + Convert.ToString((-(x.Credit - x.Debit))) + ")" : Convert.ToString((x.Credit - x.Debit)),
                              DecimalCurrAmount = (x.Credit - x.Debit) ?? 0
                          }).ToList();

                    }


                    CurrGrossAmt = Note.Sum(m => m.DecimalCurrAmount);
                    model.CurrDecGrossAmt = CurrGrossAmt;


                    if (CurrGrossAmt > 0)
                        CurrstrGrossAmt = Convert.ToString(CurrGrossAmt);
                    else if (CurrGrossAmt == 0)
                        CurrstrGrossAmt = "-";
                    else
                    {
                        CurrGrossAmt = -CurrGrossAmt;
                        CurrstrGrossAmt = "(" + CurrGrossAmt + ")";
                    }

                    model.CurrGrossAmt = CurrstrGrossAmt;
                    model.Note = Note;
                    return model;
                }

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
                  (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return model;
            }
        }
        #endregion

        public static InvoiceReportPrintModel GetInvoicePrintReport(int Id)
        {
            InvoiceReportPrintModel model = new InvoiceReportPrintModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblProjectInvoice.Where(m => m.InvoiceId == Id).FirstOrDefault();
                    int AgencyId = Qry.AgencyId ?? 0;
                    //var QryAgency = context.tblAgencyMaster.Where(m => m.AgencyId == AgencyId).FirstOrDefault();
                    int AgencyRegState = Qry.AgencyRegState ?? 0;
                    var QryState = context.tblStateMaster.Where(m => m.StateId == AgencyRegState).FirstOrDefault();
                    int InvoiceId = Qry.InvoiceId;
                    var QryInvoiceTax = context.tblInvoiceTaxDetails.Where(m => m.InvoiceId == InvoiceId).FirstOrDefault();
                    if (Qry != null)
                    {
                        CultureInfo Indian = new CultureInfo("hi-IN");
                        model.InvoiceNo = Qry.InvoiceNumber;
                        model.InvoiceDate = string.Format("{0:dd-MMM-yyyy}", Qry.InvoiceDate);
                        int ProjId = Qry.ProjectId ?? 0;
                        model.ProjectNumber = context.tblProject.Where(m => m.ProjectId == ProjId).Select(m => m.ProjectNumber).FirstOrDefault();
                        int PIId = Qry.PIId ?? 0;
                        model.SignNR_f = Common.CheckIsSAIFProject(ProjId);
                        if (model.SignNR_f)
                            model.DepartmentName = "Sophisticated Analytical Instrumentation Facility";
                        else
                            model.DepartmentName = context.vwFacultyStaffDetails.Where(m => m.UserId == PIId).Select(m => m.DepartmentName).FirstOrDefault();
                        model.PIName = context.vwFacultyStaffDetails.Where(m => m.UserId == PIId).Select(m => m.FirstName).FirstOrDefault();
                        model.SACNumber = Convert.ToString(Qry.TaxCode);
                        model.DescriptionofServices = Qry.DescriptionofServices;

                        model.IITMGSTIN = "33AAAAI3615G1Z6";
                        model.Name = Qry.AgencyRegName;
                        model.Address = Qry.CommunicationAddress;
                        model.GSTIN = Qry.AgencyRegGSTIN;
                        model.PANNo = Qry.AgencyRegPAN;
                        model.District = Qry.AgencyDistrict;
                        model.PinCode = Qry.AgencyPincode.ToString();
                        model.TANNo = Qry.AgencyRegTAN;
                        model.Email = Qry.AgencyContactPersonEmail;
                        model.ContactPerson = Qry.AgencyContactPersonName;
                        model.ContactNo = Qry.AgencyContactPersonNumber;
                        model.PONumber = Qry.PONumber;
                        model.InvoiceType = Qry.InvoiceType;
                        if (Qry.InvoiceType == 1)
                            model.Country = Common.GetCountryName(Qry.AgencyCountry ?? 0);
                        if ((model.InvoiceType == 2 || model.InvoiceType == 3 || model.InvoiceType == 5) && String.IsNullOrEmpty(Qry.IRNDocumentName) && (Qry.Status == "Open" || Qry.Status == "Approval Pending"))
                            model.Watermark_f = true;
                        //if (QryAgency != null)
                        //{
                        // model.District = QryAgency.District;
                        // model.PinCode = Convert.ToString(QryAgency.PinCode);
                        // model.TANNo = QryAgency.TAN;
                        // model.Email = QryAgency.ContactEmail;
                        // model.ContactPerson = QryAgency.ContactPerson;
                        // model.ContactNo = QryAgency.ContactNumber;
                        //}
                        if (QryState != null)
                        {
                            model.State = QryState.StateName ?? "";
                            model.StateCode = QryState.StateCode ?? "";
                        }
                        if (QryInvoiceTax != null)
                        {
                            model.SGST = String.Format(Indian, "{0:N}", QryInvoiceTax.SGSTAmount);
                            model.CGST = String.Format(Indian, "{0:N}", QryInvoiceTax.CGSTAmount);
                            model.IGST = String.Format(Indian, "{0:N}", QryInvoiceTax.IGSTAmount);
                            if (QryInvoiceTax.IGSTRate > 0)
                            {
                                decimal? gstSplit = QryInvoiceTax.IGSTRate / 2;
                                model.IGSTPct = String.Format("{0:0.##}", QryInvoiceTax.IGSTRate);
                                model.SGSTPct = String.Format("{0:0.##}", gstSplit);
                                model.CGSTPct = String.Format("{0:0.##}", gstSplit);
                            }
                            else if (QryInvoiceTax.CGSTRate > 0)
                            {
                                decimal? ttlGST = QryInvoiceTax.CGSTRate + QryInvoiceTax.SGSTRate;
                                model.IGSTPct = String.Format("{0:0.##}", ttlGST);
                                model.SGSTPct = String.Format("{0:0.##}", QryInvoiceTax.SGSTRate);
                                model.CGSTPct = String.Format("{0:0.##}", QryInvoiceTax.CGSTRate);
                            }
                        }

                        if (Qry.InvoiceType == 1)
                        {
                            var Currency = context.tblCurrency.Where(m => m.CurrencyID == Qry.CurrencyId).Select(m => m.ISOCode).FirstOrDefault() ?? "";
                            model.Amount = Convert.ToString(Currency + " " + Qry.InvoiceValueinForeignCurrency);
                            model.TotalInvoiceValueInWords = CoreAccountsService.Foreignwords(Qry.InvoiceValueinForeignCurrency, false, Currency);
                            model.TaxableValue = String.Format(Indian, "{0:N}", Qry.InvoiceValueinForeignCurrency);
                        }
                        else
                        {
                            decimal ttlVal = Qry.TotalInvoiceValue ?? 0;
                            model.TaxableValue = String.Format(Indian, "{0:N}", Qry.TaxableValue);
                            model.Amount = String.Format(Indian, "{0:N}", ttlVal);
                            model.TotalInvoiceValueInWords = CoreAccountsService.words(ttlVal);

                        }

                        model.ACName = "The Registrar, IIT Madras";
                        model.ACNo = "2722101016162";
                        model.BankName = "Canara Bank";
                        model.BranchName = "IIT-Madras Branch";
                        model.IFSC = "CNRB0002722";
                        model.MICRCode = "600015085";
                        model.SWIFTCode = "CNRBINBBIIT";
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
  (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return model;
            }
        }

        #region PayinSlip
        public CreateInvoiceModel GetPayinslip(int Payinslipid)
        {
            try
            {
                CreateInvoiceModel model = new CreateInvoiceModel();

                using (var context = new IOASExternalEntities())
                {
                    var query = context.tblPayinSlip.SingleOrDefault(m => m.PayinslipId == Payinslipid);
                    var invid = query.InvoiceId;
                    var prjctid = query.ProjectId;
                    var userid = query.PIId;
                    var invquery = context.tblProjectInvoice.SingleOrDefault(m => m.InvoiceId == invid);
                    var invtaxquery = context.tblInvoiceTaxDetails.SingleOrDefault(m => m.InvoiceId == invid);
                    CreateInvoiceModel summary = coreAccountService.GetProjectDetailsForPS(prjctid ?? 0);

                    if (summary != null)
                    {
                        var piquery = context.VwUserAD.SingleOrDefault(m => m.UserId == userid);

                        if (summary.PIId != userid)
                        {
                            return null;
                        }
                        var stateid = invquery.AgencyRegState ?? 0;

                        var state = (from st in context.tblStateMaster
                                     where st.StateId == stateid
                                     select st).FirstOrDefault();
                        var cc = context.tblCodeControl.SingleOrDefault(m => m.CodeName == "InvoiceType" && m.CodeValAbbr == summary.InvoiceType);


                        var currency = context.tblCurrency.SingleOrDefault(m => m.CurrencyID == summary.SelCurr);
                        model.InvoiceDate = DateTime.Now;
                        model.Invoicedatestrng = String.Format("{0:dd-MMM-yyyy}", invquery.InvoiceDate);
                        model.ProjectNumber = invquery.ProjectNumber;
                        model.InvoiceNumber = invquery.InvoiceNumber;
                        model.Projecttitle = summary.Projecttitle;
                        model.ProjectID = prjctid;
                        model.ProjectType = summary.ProjectType;
                        model.PIDepartmentName = summary.PIDepartmentName;
                        model.PIId = userid;
                        model.NameofPI = piquery.Name;
                        model.SanctionOrderNumber = summary.SanctionOrderNumber;
                        model.Sanctionvalue = summary.Sanctionvalue;
                        model.SponsoringAgency = invquery.AgencyId;
                        model.SponsoringAgencyName = invquery.AgencyRegName;
                        // model.Agencyregaddress = query.CommunicationAddress;
                        model.Agencydistrict = invquery.AgencyDistrict;
                        model.AgencyPincode = invquery.AgencyPincode;
                        if (state != null)
                        {
                            model.Agncystatecode = Convert.ToInt32(state.StateCode);
                            model.Agencystate = state.StateName;
                        }
                        else
                        {
                            model.Agncystatecode = Convert.ToInt32(summary.Agencystatecode);
                        }
                        model.Agencystatecode = Convert.ToInt32(invquery.AgencyRegStateCode);
                        model.AgencystateId = invquery.AgencyRegState;
                        model.GSTNumber = invquery.AgencyRegGSTIN;
                        model.PAN = invquery.AgencyRegPAN;
                        model.TAN = invquery.AgencyRegTAN;
                        model.Agencycontactperson = invquery.AgencyContactPersonName;
                        model.AgencycontactpersonEmail = invquery.AgencyContactPersonEmail;
                        model.Agencycontactpersonmobile = invquery.AgencyContactPersonNumber;
                        model.CommunicationAddress = invquery.CommunicationAddress;
                        model.TaxStatus = summary.TaxStatus;
                        model.TaxableValue = invquery.TaxableValue;
                        model.TotalInvoiceValue = invquery.TotalInvoiceValue;
                        model.BankName = summary.BankName;
                        model.BankAccountNumber = summary.BankAccountNumber;
                        model.InvoiceType = invquery.InvoiceType;
                        if (cc != null)
                        {
                            model.TypeofInvoice = cc.CodeValDetail;
                        }

                        model.CurrentFinancialYear = summary.CurrentFinancialYear;
                        model.CurrentFinyearId = summary.CurrentFinyearId;
                        model.AvailableBalance = summary.AvailableBalance;
                        model.SelCurr = summary.SelCurr;
                        if (currency != null)
                        {
                            model.CurrencyCode = currency.ISOCode;
                        }

                        model.AllocatedForeignCurrencyValue = summary.ForgnCurrenyRate;

                        model.AmountReceived = query.AmountReceived;
                        model.OutstandingAmount = query.OutstandingAmount;
                        model.PaySlipReferenceNumber = query.PayinslipReferenceNumber;
                        model.PayinslipDate = String.Format("{0:dd-MMM-yyyy}", query.CrtdTS);
                        model.ReceiptReference = query.ReferenceNumber;
                        model.InstrumentsRemarks = query.Remarks;
                        model.ModeofPayment = query.ModeofPayment;
                        model.InstrumentsDate = String.Format("{0:dd-MMMM-yyyy}", query.InstrumentsDate);
                        // model.ReceiptReference = query.Remarks;
                        model.TDSReceivableGST = query.GSTTdsReceived;
                        model.TDSReceived = query.ITTdsReceived;
                        model.CGST = invtaxquery.CGSTAmount;
                        model.CGSTRate = invtaxquery.CGSTRate;
                        model.SGST = invtaxquery.SGSTAmount;
                        model.SGSTRate = invtaxquery.SGSTRate;
                        model.IGST = invtaxquery.IGSTAmount;
                        model.IGSTRate = invtaxquery.IGSTRate;
                    }

                }

                return model;
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(this, ex);
                return new CreateInvoiceModel();
            }
        }


        #endregion
        #region GstOffset Bill Report
        public TravelBillReportModel GetGstOffsetBillReport(int Id = 0)
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Qry = context.tblGSTOffset.Where(m => m.GSTOffsetid == Id).FirstOrDefault();
                    string FromDate = "01-April-2019";
                    string ToDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    var ExpQry = context.tblGSTOffsetExpenseDetail.Where(m => m.GSTOffsetId == Id).ToList();
                    int[] headid = { 38, 61 };
                    var BankHeadId = ExpQry.Where(m => m.AccountGroupId == 38).Select(m => m.AccountHeadId).FirstOrDefault();
                    if (BankHeadId != null)
                        model.BankAccount = context.tblAccountHead.Where(m => m.AccountHeadId == BankHeadId).Select(m => m.AccountHead).FirstOrDefault();
                    model.BillId = Id;
                    model.BillMonth = String.Format("{0:MMM yyyy}", Qry.CRTD_TS);
                    model.BillNumber = Qry.GSTOffsetNumber;
                    model.BillType = "Tax Voucher";
                    model.BillDate = String.Format("{0:dd-MMMM-yyyy}", Qry.CRTD_TS);
                    model.Reason = Qry.Remarks;
                    model.GSTInput = ExpQry.Where(m => m.AccountGroupId == 13 && m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
                    model.GSTOutput = ExpQry.Where(m => m.AccountGroupId == 14).Select(m => m.Amount).Sum() ?? 0;
                    int[] TDSReceivable = { 27, 296, 297 };
                    model.TDSReceivable = ExpQry.Where(m => TDSReceivable.Contains(m.AccountHeadId ?? 0)).Select(m => m.Amount).Sum() ?? 0;
                    model.RoundOffCredit = ExpQry.Where(m => m.AccountHeadId == 319).Select(m => m.Amount).Sum() ?? 0;
                    model.RoundOffDebit = ExpQry.Where(m => m.AccountHeadId == 320).Select(m => m.Amount).Sum() ?? 0;
                    //
                    //decimal Amt = ExpQry.Where(m => m.AccountGroupId == 38).Select(m => m.Amount).Sum() ?? 0;
                    decimal Amt = (model.GSTOutput + model.RoundOffDebit) - (model.GSTInput + model.TDSReceivable + model.RoundOffCredit);
                    model.PayableAmount = Amt;
                    model.TotalBillValue = Amt;
                    model.TotalAmount = Amt;
                    model.Rupees = CoreAccountsService.words(Amt);
                    //
                    model.PrintedDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.Remarks = Qry.Remarks;
                    model.TravelerName = Common.GetUserNameBasedonId(Convert.ToInt32(Qry.CRTD_By));
                    if (BillMode == "Old")
                    {
                        model.BillMonth = BillMonth;
                        model.BillDate = BillDate;
                        model.PrintedDate = BillDate;
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

    }
}