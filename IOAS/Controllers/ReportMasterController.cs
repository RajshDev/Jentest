using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IOAS.DataModel;
using CrystalDecisions.CrystalReports.Engine;
using IOAS.Infrastructure;
using System.IO;
using IOAS.GenericServices;
using System.Data;
using Rotativa.MVC;
using Rotativa.Core;
using Rotativa.Core.Options;
using System.Configuration;
using DataAccessLayer;
using SelectPdf;
using IOASExternal.DataModel;

namespace IOAS.Controllers
{
    public class ReportMasterController : Controller
    {

        string strServer = ConfigurationManager.AppSettings["ServerName"].ToString();
        string strDatabase = ConfigurationManager.AppSettings["DataBaseName"].ToString();
        string strUserID = ConfigurationManager.AppSettings["UserId"].ToString();
        string strPwd = ConfigurationManager.AppSettings["Password"].ToString();
        string BillMode = ConfigurationManager.AppSettings["BillMode"].ToString();
        string BillMonth = ConfigurationManager.AppSettings["BillMonth"].ToString();
        string BillDate = ConfigurationManager.AppSettings["BillDate"].ToString();

        #region CashBook
        [HttpGet]
        public ActionResult CashBookReport()
        {
            ViewBag.Bank = Common.GetBankAccount();
            return View();
        }
        public ActionResult CashBookShow(string fdate, string tdate, int BankId)
        {
            try
            {
                //string loginuser = User.Identity.Name;
                //int role = Common.GetRoleId(loginuser);
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/CashBookDetails?fdate=" + fdate + "&&tdate=" + tdate + "&&BankId=" + BankId;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 0;

                // instantiate a html to pdf converter object
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);

                // save pdf document
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=Proposal.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {

                //writeError(ex);
                return new EmptyResult();
            }
        }
        public ActionResult CashBookDetails(string fdate, string tdate, int BankId)
        {
            var username = User.Identity.Name;
            var res = new object();
            var fromdate = Convert.ToDateTime(fdate);
            var todate = Convert.ToDateTime(tdate);
            CashBookModel model = new CashBookModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Pay = (from c in context.vw_CashBookPayment
                               where (c.ReferenceDate < fromdate) && (c.BankHeadID == BankId)
                               select new
                               { c }).ToList();
                    decimal PayAmt = Pay.Select(m => m.c.Amount).Sum() ?? 0;
                    Math.Round(PayAmt, 2);
                    var Rec = (from c in context.vw_CashBookReceipt
                               where (c.ReferenceDate < fromdate) && (c.BankHeadID == BankId)
                               select new
                               { c }).ToList();
                    decimal RecAmt = Rec.Select(m => m.c.Amount).Sum() ?? 0;
                    Math.Round(RecAmt, 2);
                    var OB = (from c in context.tblHeadOpeningBalance
                              where c.AccountHeadId == BankId
                              select new { c }).FirstOrDefault();
                    string FinalOB = "";
                    decimal COB = 0;
                    if (OB.c.TransactionType == "Credit")
                    {
                        COB = -OB.c.OpeningBalance + (RecAmt - PayAmt) ?? 0;
                        Math.Round(COB, 2);

                    }
                    else if (OB.c.TransactionType == "Debit") { COB = OB.c.OpeningBalance + (RecAmt - PayAmt) ?? 0; Math.Round(COB, 2); }
                    var PayCB = (from c in context.vw_CashBookPayment
                                 where (c.ReferenceDate >= fromdate && c.ReferenceDate <= todate) && (c.BankHeadID == BankId)
                                 select new
                                 { c }).ToList();
                    decimal PayAmtCB = PayCB.Select(m => m.c.Amount).Sum() ?? 0;
                    Math.Round(PayAmtCB, 2);
                    var RecCB = (from c in context.vw_CashBookReceipt
                                 where (c.ReferenceDate >= fromdate && c.ReferenceDate <= todate) && (c.BankHeadID == BankId)
                                 select new
                                 { c }).ToList();
                    decimal RecAmtCB = RecCB.Select(m => m.c.Amount).Sum() ?? 0;
                    Math.Round(RecAmtCB, 2);
                    decimal CB = 0;
                    if (COB < 0)
                    {
                        CB = COB + (RecAmtCB - PayAmtCB);
                        Math.Round(CB, 2);
                        FinalOB = -COB + " Cr";
                    }
                    else if (COB > 0) { CB = COB + (RecAmtCB - PayAmtCB); Math.Round(CB, 2); FinalOB = COB + " Dr"; }
                    string FinalCB = "";
                    if (CB < 0) { FinalCB = -CB + " Cr"; } else { FinalCB = CB + " Dr"; }
                    model.REC = ReportService.CashBookReceiptRep(fromdate, todate, BankId);
                    model.PAY = ReportService.CashBookPaymentRep(fromdate, todate, BankId);
                    model.fromdate = String.Format("{0:dd-MMMM-yyyy}", fromdate);
                    model.todate = String.Format("{0:dd-MMMM-yyyy}", todate);
                    model.Bank = Common.GetBankName(BankId);
                    model.CashBookDate = String.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
                    model.Id = 1;
                    model.FinalOB = FinalOB;
                    model.FinalCB = FinalCB;
                    model.username = username;
                    model.PaymentTotalAmount = model.PAY.Select(m => m.Amount).Sum();
                    model.ReceiptTotalAmount = model.REC.Select(m => m.Amount).Sum();


                }

                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView("ErrOutput");
            }
        }
        public ActionResult CashBook(DateTime fromdate, DateTime todate, int BankId, int format)
        {
            var username = User.Identity.Name;
            var em = 0;
            var res = new object();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Pay = (from c in context.vw_CashBookPayment
                               where (c.ReferenceDate < fromdate) && (c.BankHeadID == BankId)
                               select new
                               { c }).ToList();
                    decimal PayAmt = Pay.Select(m => m.c.Amount).Sum() ?? 0;
                    Math.Round(PayAmt, 2);
                    var Rec = (from c in context.vw_CashBookReceipt
                               where (c.ReferenceDate < fromdate) && (c.BankHeadID == BankId)
                               select new
                               { c }).ToList();
                    decimal RecAmt = Rec.Select(m => m.c.Amount).Sum() ?? 0;
                    Math.Round(RecAmt, 2);
                    var OB = (from c in context.tblHeadOpeningBalance
                              where c.AccountHeadId == BankId
                              select new { c }).FirstOrDefault();
                    string FinalOB = "";
                    decimal COB = 0;
                    if (OB.c.TransactionType == "Credit")
                    {
                        COB = -OB.c.OpeningBalance + (RecAmt - PayAmt) ?? 0;
                        Math.Round(COB, 2);

                    }
                    else if (OB.c.TransactionType == "Debit") { COB = OB.c.OpeningBalance + (RecAmt - PayAmt) ?? 0; Math.Round(COB, 2); }
                    var PayCB = (from c in context.vw_CashBookPayment
                                 where (c.ReferenceDate >= fromdate && c.ReferenceDate <= todate) && (c.BankHeadID == BankId)
                                 select new
                                 { c }).ToList();
                    decimal PayAmtCB = PayCB.Select(m => m.c.Amount).Sum() ?? 0;
                    Math.Round(PayAmtCB, 2);
                    var RecCB = (from c in context.vw_CashBookReceipt
                                 where (c.ReferenceDate >= fromdate && c.ReferenceDate <= todate) && (c.BankHeadID == BankId)
                                 select new
                                 { c }).ToList();
                    decimal RecAmtCB = RecCB.Select(m => m.c.Amount).Sum() ?? 0;
                    Math.Round(RecAmtCB, 2);
                    decimal CB = 0;
                    if (COB < 0)
                    {
                        CB = COB + (RecAmtCB - PayAmtCB);
                        Math.Round(CB, 2);
                        FinalOB = -COB + " Cr";
                    }
                    else if (COB > 0) { CB = COB + (RecAmtCB - PayAmtCB); Math.Round(CB, 2); FinalOB = COB + " Dr"; }
                    string FinalCB = "";
                    if (CB < 0) { FinalCB = -CB + " Cr"; } else { FinalCB = CB + " Dr"; }
                    ReportDocument rd = new ReportDocument();
                    string conn = "IOASDB";
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "CashBook.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection("10.18.0.11,1433", conn, "sa", "IcsR@123#");
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    var Receipt = ReportService.CashBookReceiptRep(fromdate, todate, BankId);
                    var Payment = ReportService.CashBookPaymentRep(fromdate, todate, BankId);
                    rd.Subreports[0].SetDataSource(Payment);
                    rd.Subreports[1].SetDataSource(Receipt);
                    rd.SetParameterValue("fromdate", fromdate);
                    rd.SetParameterValue("todate", todate);
                    rd.SetParameterValue("BankId", Common.GetBankName(BankId));
                    rd.SetParameterValue("FinalOB", FinalOB);
                    rd.SetParameterValue("FinalCB", FinalCB);
                    if (username != null)
                    {
                        rd.SetParameterValue("username", username);
                    }
                    Stream stream;
                    if (format == 1)
                    {
                        stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        Response.AddHeader("Content-Disposition", "inline; filename=CashBook.pdf");
                        return File(stream, "application/pdf");
                    }
                    else
                    {
                        stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                        stream.Seek(0, SeekOrigin.Begin);
                        Response.AddHeader("Content-Disposition", "inline; filename=CashBook.xls");
                        return File(stream, "application/vnd.ms-excel");
                    }
                }
            }
            catch (Exception ex)
            {
                em = 1;
                res = new { em = em };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetDataForCashbookReport(DateTime fromdate, DateTime todate, int BankId)
        {
            var pay = 0;
            var rec = 0;
            var data = new object();
            var Receipt = ReportService.CashBookReceiptRep(fromdate, todate, BankId);
            var Payment = ReportService.CashBookPaymentRep(fromdate, todate, BankId);

            if (Payment.Count > 0)
            {
                pay = 1;
            }
            else { pay = 2; }

            if (Receipt.Count > 0)
            {
                rec = 1;
            }
            else { rec = 2; }
            data = new { rec = rec, pay = pay };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Posting
        public ActionResult PostingReport()
        {
            ViewBag.TransactionType = GetTransactionType();
            return View();
        }
        public ActionResult Posting(DateTime fromdate, DateTime todate, string transactiontype, int format)
        {
            ReportDocument rd = new ReportDocument();
            try
            {
                string conn = "IOASDB";
                rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "Postings.rpt"));
                for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                    rd.DataSourceConnections[i].SetConnection(".", conn, "sa", "Welc0me");
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                var data = ReportService.PostingsRep(fromdate, todate, transactiontype);
                rd.SetDataSource(data);
                rd.SetParameterValue("fromdate", fromdate);
                rd.SetParameterValue("todate", todate);
                if (transactiontype != null)
                {
                    rd.SetParameterValue("transactiontype", transactiontype);
                }
                Stream stream;
                if (format == 1)
                {
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=PostingReport.pdf");
                    return File(stream, "application/pdf");
                }
                else
                {
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=PostingReport.xls");
                    return File(stream, "application/vnd.ms-excel");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult GetDataForPostingReport(DateTime fromdate, DateTime todate, string transactiontype)
        {
            var em = 0;
            var data = new object();
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
                               b.TransType

                           }).ToList();

                if (Qry.Count > 0)
                {
                    em = 1;
                }
                else { em = 2; }

            }
            data = new { em = em };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Commitment
        public ActionResult CommitmentReport()
        {
            ViewBag.ProjectNumber = GetProjectNumber();
            return View();
        }
        public ActionResult Commitment(DateTime fromdate, DateTime todate, int projecttype, int format, int projectnumber = 0)
        {
            ReportDocument rd = new ReportDocument();
            try
            {
                bool appendPIName = true;
                string conn = "IOASDB";
                rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "CommitmentReport.rpt"));
                for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                    rd.DataSourceConnections[i].SetConnection("10.18.0.11,1433", conn, "sa", "IcsR@123#");
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                var data = ReportService.CommitmentRep(fromdate, todate, projecttype, projectnumber);
                rd.SetDataSource(data);
                rd.SetParameterValue("fromdate", fromdate);
                rd.SetParameterValue("todate", todate);
                rd.SetParameterValue("projecttype", Common.getprojectTypeName(projecttype));
                if (projectnumber != 0)
                {
                    rd.SetParameterValue("projectnumber", Common.GetProjectNumber(projectnumber, appendPIName));
                }
                else
                {
                    rd.SetParameterValue("projectnumber", "");

                }
                Stream stream;
                if (format == 1)
                {
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=CommitmentReport.pdf");
                    return File(stream, "application/pdf");
                }
                else
                {
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=CommitmentReport.xls");
                    return File(stream, "application/vnd.ms-excel");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult GetDataForCommitmentReport(DateTime fromdate, DateTime todate, int projecttype, int projectnumber = 0)
        {
            var em = 0;
            var data = new object();
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
                    em = 1;
                }
                else { em = 2; }

            }
            data = new { em = em };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadProjectNo(string ProjType)
        {
            ProjType = ProjType == "" ? "0" : ProjType;
            var locationdata = ProjectService.LoadProjecttitledetails(Convert.ToInt32(ProjType));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        #endregion
        //public ActionResult CashBookReport()
        //{
        //    return View();
        //}
        //public ActionResult CashBook(DateTime fromdate,DateTime todate)
        //{
        //    ReportDocument rd = new ReportDocument();
        //    try
        //    {
        //        string conn = "IOASDBTH";
        //        rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), ".rpt"));
        //        for (int i = 0; i < rd.DataSourceConnections.Count; i++)
        //            rd.DataSourceConnections[i].SetConnection(".", conn, "sa", "Welc0me");
        //        Response.Buffer = false;
        //        Response.ClearContent();
        //        Response.ClearHeaders();
        //        DataSet ds = new DataSet();
        //        var data1 = ReportService.CashBookRep(fromdate, todate);
        //        ds.Tables.Add(data1);
        //        rd.SetDataSource(ds);
        //        rd.SetParameterValue("fromdate", fromdate);
        //        rd.SetParameterValue("todate", todate);                                                        
        //        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //        stream.Seek(0, SeekOrigin.Begin);
        //        Response.AddHeader("Content-Disposition", "inline; filename=NEW.pdf");
        //        return File(stream, "application/pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public static List<ProjectNumModel> GetProjectNumber()
        {
            List<ProjectNumModel> projnum = new List<ProjectNumModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from C in context.tblProject
                             orderby C.ProjectNumber
                             select new { C.ProjectNumber }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        projnum.Add(new ProjectNumModel()
                        {
                            ProjectNumber = query[i].ProjectNumber
                        });
                    }
                }
            }
            return projnum;
        }
        public static List<TransactionTypeModel> GetTransactionType()
        {
            List<TransactionTypeModel> transtype = new List<TransactionTypeModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from C in context.tblTransactionTypeCode
                             orderby C.TransactionType
                             select new { C.TransactionType }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        transtype.Add(new TransactionTypeModel()
                        {
                            TransactionType = query[i].TransactionType
                        });
                    }
                }
            }
            return transtype;
        }
        public static List<AccountTypeModel> GetFinancialYear()
        {
            List<AccountTypeModel> acctype = new List<AccountTypeModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from C in context.tblFinYear
                             orderby C.FinYearId
                             select new { C }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        acctype.Add(new AccountTypeModel()
                        {
                            FinancialYear = query[i].C.Year,
                            FinancialId = query[i].C.FinYearId
                        });
                    }
                }
            }
            return acctype;
        }
        public ActionResult DailyBalanceVerification()
        {
            ViewBag.ProjectType = Common.getprojecttype();
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetDailyBalSummary(int projectid, DateTime SerDate)
        {
            var locationdata = ReportService.GetDailyBalanceVerfication(projectid, SerDate);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult BOATransactionDetailsReport()
        {
            ViewBag.ProjectNumber = GetProjectNumber();
            ViewBag.TransactionType = GetTransactionType();
            return View();
        }
        public ActionResult BOATransactionDetails(DateTime fromdate, DateTime todate, string projectnumber, string transactiontype)
        {
            ReportDocument rd = new ReportDocument();
            try
            {
                string conn = "IOASDB";
                rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "BOATransactionDetails.rpt"));
                for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                    rd.DataSourceConnections[i].SetConnection(".", conn, "sa", "Welc0me");
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                var data = ReportService.BOATransactionDetailsRep(fromdate, todate,
                    projectnumber, transactiontype);
                rd.SetDataSource(data);
                rd.SetParameterValue("fromdate", fromdate);
                rd.SetParameterValue("todate", todate);
                if (transactiontype != null)
                {
                    rd.SetParameterValue("transactiontype", transactiontype);
                }
                if (transactiontype != null)
                {

                    rd.SetParameterValue("projectnumber", projectnumber);
                }
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                Response.AddHeader("Content-Disposition", "inline; filename=NEW.pdf");
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region ProjectTransaction
        public ActionResult ProjectTransactionReport()
        {
            return View();
        }
        
        #endregion
        #region Process GuideLine Report
        [HttpPost]
        public ActionResult ApprovalStatusReport(ProcessGuideLineReportModel model)
        {
            //ProcessGuideLineReportModel model = new ProcessGuideLineReportModel();
            model.Flow = ReportService.GetApprovalDetails(model.UserId);
            model.Heading = ReportService.GetHeading(model.UserId);
            model.Id = 1;
            return View(model);

        }
        [HttpGet]
        public ActionResult ApprovalStatusReport()
        {
            ProcessGuideLineReportModel model = new ProcessGuideLineReportModel();
            int id = Common.GetUserid(User.Identity.Name);
            //bool value = Common.GetUserIdBasedOnRole(id);
            var name = User.Identity.Name;
            var RoleId = Common.GetRoleId(name);
            var FunctionId = 92;
            var value = Common.GetApproverBasedonRole(RoleId, FunctionId);
            if (value)
                model.User = 1;
            else
                model.User = 2;
            if (!value)
            {
                model.Flow = ReportService.GetApprovalDetails(id);
            }
            model.Id = 1;
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadUserName(string term)
        {
            try
            {
                var data = Common.GetUserName(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Approverwise Pending Report
        [HttpGet]
        public ActionResult ApproverwisePendingReport()
        {
            ProcessGuideLineReportModel model = new ProcessGuideLineReportModel();
            var name = User.Identity.Name;
            int id = Common.GetUserid(User.Identity.Name);
            var RoleId = Common.GetRoleId(name);
            var FunctionId = 93;
            var value = Common.GetApproverBasedonRole(RoleId, FunctionId);
            if (value)
                model.User = 1;
            else
                model.User = 2;
            if (!value)
            {
                model.Flow = ReportService.GetApproverwisePendingDetails(id);
            }
            model.Id = 1;
            return View(model);
        }
        [HttpPost]
        public ActionResult ApproverwisePendingReport(ProcessGuideLineReportModel model)
        {
            model.Flow = ReportService.GetApproverwisePendingDetails(model.UserId);
            model.Heading = ReportService.GetHeading(model.UserId);
            model.Id = 1;
            return View(model);
        }
        #endregion
        
        #region FinalOutstandigReport
        public ActionResult InvoiceReport()
        {
            return View();
        }
        #endregion

        #region Agency Report
        public ActionResult AgencyReport()
        {
            return View();
        }
        #endregion
        #region Foreign Remittance
        public ActionResult FRMCBDirectImportLetterdownload(int foreignRemitId)
        {
            try
            {
                //string loginuser = User.Identity.Name;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblForeignRemittance.FirstOrDefault(m => m.ForeignRemitId == foreignRemitId);
                    string url = "";
                    if (query.PaymentBank == 20 && query.TypeofPayment == 1 && query.PurposeofRemittance == 1)
                    {
                        url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/FRMCBAdvanceImportOutput?id=" + foreignRemitId;
                    }
                    if (query.PaymentBank == 20 && query.TypeofPayment == 2 && query.PurposeofRemittance == 1)
                    {
                        url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/FRMCBDirectImportOutput?id=" + foreignRemitId;
                    }
                    if (query.PaymentBank == 21 && query.TypeofPayment == 1 && query.PurposeofRemittance == 1)
                    {
                        url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/FRMHDFCAdvanceImportOutput?id=" + foreignRemitId;
                    }
                    if (query.PaymentBank == 21 && query.TypeofPayment == 2 && query.PurposeofRemittance == 1)
                    {
                        url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/FRMHDFCDirectImportOutput?id=" + foreignRemitId;
                    }
                    if (query.PurposeofRemittance == 2 || query.PurposeofRemittance == 3 || query.PurposeofRemittance == 4 || query.PurposeofRemittance == 5)
                    {
                        url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/FRMOtherServicesOutput?id=" + foreignRemitId;
                    }

                    string pdf_page_size = "A4";
                    SelectPdf.PdfPageSize pageSize =
                        (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                    string pdf_orientation = "Portrait";
                    SelectPdf.PdfPageOrientation pdfOrientation =
                        (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                        pdf_orientation, true);

                    int webPageWidth = 924;

                    int webPageHeight = 0;

                    // instantiate a html to pdf converter object
                    SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

                    // set converter options
                    converter.Options.PdfPageSize = pageSize;
                    converter.Options.MaxPageLoadTime = 240;
                    converter.Options.PdfPageOrientation = pdfOrientation;
                    converter.Options.WebPageWidth = webPageWidth;
                    converter.Options.WebPageHeight = webPageHeight;

                    // create a new pdf document converting an url
                    SelectPdf.PdfDocument doc = converter.ConvertUrl(url);

                    // save pdf document
                    byte[] pdf = doc.Save();

                    // close pdf document
                    doc.Close();
                    Response.AddHeader("Content-Disposition", "inline; filename=ForeignRemitanceReport.pdf");
                    return File(pdf, "application/pdf");
                    // return resulted pdf document
                    //FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                    //if (query.PaymentBank == 20 && query.TypeofPayment == 1 && query.PurposeofRemittance == 1)
                    //{
                    //    fileResult.FileDownloadName = "Advance Import Request Letter_CB " + foreignRemitId + ".pdf";
                    //}
                    //if (query.PaymentBank == 20 && query.TypeofPayment == 2 && query.PurposeofRemittance == 1)
                    //{
                    //    fileResult.FileDownloadName = "Direct Import Request Letter_CB " + foreignRemitId + ".pdf";
                    //}
                    //if (query.PaymentBank == 21 && query.TypeofPayment == 1 && query.PurposeofRemittance == 1)
                    //{
                    //    fileResult.FileDownloadName = "Advance Import Request Letter_HDFC " + foreignRemitId + ".pdf";
                    //}
                    //if (query.PaymentBank == 21 && query.TypeofPayment == 2 && query.PurposeofRemittance == 1)
                    //{
                    //    fileResult.FileDownloadName = "Direct Import Request Letter_HDFC " + foreignRemitId + ".pdf";
                    //}
                    //if (query.PurposeofRemittance == 2 || query.PurposeofRemittance == 3 || query.PurposeofRemittance == 4 || query.PurposeofRemittance == 5)
                    //{
                    //    fileResult.FileDownloadName = "Other Services Request Letter_HDFC " + foreignRemitId + ".pdf";
                    //}
                    //return fileResult;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(this, ex);
                return new EmptyResult();
            }
        }
        public ActionResult FRMCBDirectImportOutput(int id)
        {
            try
            {
                ForeignRemittanceModel model = new ForeignRemittanceModel();
                CoreAccountsService coreAccountService = new CoreAccountsService();
                model = coreAccountService.GetForeignRemitPrintDetails(id);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView("ErrOutput");
            }
        }
        public ActionResult FRMCBAdvanceImportOutput(int id)
        {
            try
            {
                ForeignRemittanceModel model = new ForeignRemittanceModel();
                CoreAccountsService coreAccountService = new CoreAccountsService();
                model = coreAccountService.GetForeignRemitPrintDetails(id);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView("ErrOutput");
            }
        }
        public ActionResult FRMHDFCAdvanceImportOutput(int id)
        {
            try
            {
                ForeignRemittanceModel model = new ForeignRemittanceModel();
                CoreAccountsService coreAccountService = new CoreAccountsService();
                model = coreAccountService.GetForeignRemitPrintDetails(id);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView("ErrOutput");
            }
        }
        public ActionResult FRMHDFCDirectImportOutput(int id)
        {
            try
            {
                ForeignRemittanceModel model = new ForeignRemittanceModel();
                CoreAccountsService coreAccountService = new CoreAccountsService();
                model = coreAccountService.GetForeignRemitPrintDetails(id);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView("ErrOutput");
            }
        }
        public ActionResult FRMOtherServicesOutput(int id)
        {
            try
            {
                ForeignRemittanceModel model = new ForeignRemittanceModel();
                CoreAccountsService coreAccountService = new CoreAccountsService();
                model = coreAccountService.GetForeignRemitPrintDetails(id);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView("ErrOutput");
            }
        }
        #endregion
        #region LC Opening 
        public ActionResult LCDraftdownload(int LCDraftId = 0)        {
            try            {                LCOpeningModel model = new LCOpeningModel();                CoreAccountsService coreAccountService = new CoreAccountsService();                model = coreAccountService.GetLCPrintDetails(LCDraftId);                var yourpdf = new PartialViewAsPdf("LCDraftOutput", model)                {                    RotativaOptions = new DriverOptions()                    {                        PageOrientation = Rotativa.Core.Options.Orientation.Portrait,                        PageSize = Rotativa.Core.Options.Size.A4,                        IsLowQuality = true,                        PageMargins = new Margins(5, 0, 5, 0)                    }                };                return yourpdf;            }            catch (Exception ex)            {                return new EmptyResult();            }        }
        public ActionResult LCDraftOutput(int id)
        {
            try
            {
                LCOpeningModel model = new LCOpeningModel();
                CoreAccountsService coreAccountService = new CoreAccountsService();
                model = coreAccountService.GetLCPrintDetails(id);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView("ErrOutput");
            }
        }
        public ActionResult LCEstablishWithoutChangedownload(int LCDraftId)
        {
            try
            {
                //string loginuser = User.Identity.Name;

                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/LCEstablishWithoutChange?id=" + LCDraftId;

                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;

                int webPageHeight = 0;

                // instantiate a html to pdf converter object
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);

                // save pdf document
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=LCEstablishmentWithoutChange.pdf");
                return File(pdf, "application/pdf");
                // return resulted pdf document
                //FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                //fileResult.FileDownloadName = "LC Establishment Without Changes " + LCDraftId + ".pdf";
                //return fileResult;
            }
            catch (Exception ex)
            {

                return new EmptyResult();
            }
        }
        public ActionResult LCEstablishWithoutChange(int id)
        {
            try
            {
                LCOpeningModel model = new LCOpeningModel();
                CoreAccountsService coreAccountService = new CoreAccountsService();
                model = coreAccountService.GetLCEstablishPrintDetails(id);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView("ErrOutput");
            }
        }
        public ActionResult LCEstablishWithChangesdownload(int LCDraftId)
        {
            try
            {
                //string loginuser = User.Identity.Name;

                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/LCEstablishWithChanges?id=" + LCDraftId;

                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;

                int webPageHeight = 0;

                // instantiate a html to pdf converter object
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);

                // save pdf document
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=LCEstablishmentWithChange.pdf");
                return File(pdf, "application/pdf");
                // return resulted pdf document
                //FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                //fileResult.FileDownloadName = "LC Establishment With Changes " + LCDraftId + ".pdf";
                //return fileResult;
            }
            catch (Exception ex)
            {

                return new EmptyResult();
            }
        }
        public ActionResult LCEstablishWithChanges(int id)
        {
            try
            {
                LCOpeningModel model = new LCOpeningModel();
                CoreAccountsService coreAccountService = new CoreAccountsService();
                model = coreAccountService.GetLCEstablishPrintDetails(id);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView("ErrOutput");
            }
        }
        public ActionResult LCAmendmentdownload(int LCAmendId)
        {
            try
            {
                //string loginuser = User.Identity.Name;

                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/LCAmendmentOutput?id=" + LCAmendId;

                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;

                int webPageHeight = 0;

                // instantiate a html to pdf converter object
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                converter.Options.MarginTop = 15;
                converter.Options.MarginBottom = 15;
                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);

                // save pdf document
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=LCAmendment.pdf");
                return File(pdf, "application/pdf");
                // return resulted pdf document
                //FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                //fileResult.FileDownloadName = "LC Ammendment " + LCAmendId + ".pdf";
                //return fileResult;
            }
            catch (Exception ex)
            {

                return new EmptyResult();
            }
        }
        public ActionResult LCAmendmentOutput(int id)
        {
            try
            {
                LCOpeningModel model = new LCOpeningModel();
                CoreAccountsService coreAccountService = new CoreAccountsService();
                model = coreAccountService.GetLCAmendPrintDetails(id);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView("ErrOutput");
            }
        }
        #endregion
        #region LC Retirement 
        public ActionResult LCRetiredownload(int LCRetirementId)
        {
            try
            {
                //string loginuser = User.Identity.Name;

                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/LCRetireOutput?id=" + LCRetirementId;

                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;

                int webPageHeight = 0;

                // instantiate a html to pdf converter object
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);

                // save pdf document
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=LCRetirement.pdf");
                return File(pdf, "application/pdf");
                // return resulted pdf document
                //FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                //fileResult.FileDownloadName = "Retirment of Documents " + LCRetirementId + ".pdf";
                //return fileResult;
            }
            catch (Exception ex)
            {

                return new EmptyResult();
            }
        }
        public ActionResult LCRetireOutput(int id)
        {
            try
            {
                LCOpeningModel model = new LCOpeningModel();
                CoreAccountsService coreAccountService = new CoreAccountsService();
                model = coreAccountService.GetLCRetirementDetailsbyId(id);
                if (BillMode == "Old")
                    model.LCRetireDate = BillDate;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView("ErrOutput");
            }
        }
        #endregion

        #region TemporaryAdvance Bill Report
        public ActionResult TemporaryAdvanceBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/TemporaryAdvanceBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=TravelBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult TemporaryAdvanceBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetTemporaryAdvanceBillReportDetails(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);

            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion

        #region SummerIntenship Bill Report
        public ActionResult SummerIntenshipBill(int Id = 0)
        {

            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/SummerIntenshipBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=TravelBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult SummerIntenshipBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetSummerIntenshipBillReportDetails(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);

            }
            catch (Exception ex)
            {
                return PartialView(model);
            }

        }
        #endregion

        #region PartTimeStudent Bill Report
        public ActionResult PartTimeStudentBill(int Id = 0)
        {

            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/PartTimeStudentBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=TravelBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult PartTimeStudentBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetPartTimeStudentBillReportDetails(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);

            }
            catch (Exception ex)
            {
                return PartialView(model);
            }

        }
        #endregion

        #region TempAdvanceSettlement Bill Report
        public ActionResult TemporaryAdvanceSettlementBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/TemporaryAdvanceSettlementDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=TravelBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult TemporaryAdvanceSettlementDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetTemporaryAdvanceSettlementDetails(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);

            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion

        #region ClearancePayment Bill Report
        public ActionResult ClearancePaymentBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/ClearancePaymentBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=TravelBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult ClearancePaymentBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetClearancePaymentDetails(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);

            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion

        #region ImprestBillBoking Bill Report
        public ActionResult ImprestBillBokingBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/ImprestBillBokingDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=TravelBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult ImprestBillBokingDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetImprestBillBokingDetails(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion

        #region ImprestBillsRecoupment Bill Report
        public ActionResult ImprestBillsRecoupmentBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/ImprestBillsRecoupmentDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=TravelBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult ImprestBillsRecoupmentDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetImprestBillsRecoupmentDetails(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion

        #region Contra Bill Report
        public ActionResult ContraBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/ContraBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=TravelBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult ContraBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetContraDetails(Id);
                model.PrintedBy = LoginUser;
                return PartialView("ContraDetails",model);
            }
            catch (Exception ex)
            {
                return PartialView("ContraDetails", model);
            }
        }
        #endregion

        #region Journal Bill Report
        public ActionResult JournalBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/JournalBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=TravelBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult JournalBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetJournalBillDetails(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion

        #region Reimbursement Bill Report
        public ActionResult ReimbursementBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/ReimbursementBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=TravelBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult ReimbursementBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetReimbursementBillDetails(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion

        #region Direct Fund Transfer Bill Report
        public ActionResult DirectFundTransferBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/DirectFundTransferBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=TravelBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult DirectFundTransferBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetDirectFundTransferBillDetails(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion

        #region Headwise Fund Transfer Bill Reprot
        public ActionResult HeadwiseFundTransferBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/HeadwiseFundTransferBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=TravelBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult HeadwiseFundTransferBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetHeadwiseFundTransferBillDetails(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region Honorarium Bill Report



        public ActionResult HonorariumBill(int Id = 0)        {            TravelBillReportModel model = new TravelBillReportModel();            try            {                string loginuser = User.Identity.Name;                ReportService reportservice = new ReportService();                model = reportservice.GetHonorariumBillReport(Id);                model.PrintedBy = loginuser;                var yourpdf = new PartialViewAsPdf("HonorariumBillDetails", model)                {                    RotativaOptions = new DriverOptions()                    {                        PageOrientation = Rotativa.Core.Options.Orientation.Portrait,                        PageSize = Rotativa.Core.Options.Size.A4,                        IsLowQuality = true,                        PageMargins = new Margins(5, 0, 5, 0)                    }                };                return yourpdf;            }            catch (Exception ex)            {                return new EmptyResult();            }        }

        //public ActionResult HonorariumBill(int Id = 0)
        //{
        //    try
        //    {
        //        string loginuser = User.Identity.Name;
        //        string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/HonorariumBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
        //        string pdf_page_size = "A4";
        //        SelectPdf.PdfPageSize pageSize =
        //            (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
        //        string pdf_orientation = "Portrait";
        //        SelectPdf.PdfPageOrientation pdfOrientation =
        //            (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
        //            pdf_orientation, true);
        //        int webPageWidth = 1024;
        //        int webPageHeight = 0;
        //        SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
        //        converter.Options.PdfPageSize = pageSize;
        //        converter.Options.PdfPageOrientation = pdfOrientation;
        //        converter.Options.MaxPageLoadTime = 240;
        //        converter.Options.WebPageWidth = webPageWidth;
        //        converter.Options.WebPageHeight = webPageHeight;
        //        SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
        //        byte[] pdf = doc.Save();
        //        doc.Close();
        //        Response.AddHeader("Content-Disposition", "inline; filename=HonorariumBill.pdf");
        //        return File(pdf, "application/pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        return new EmptyResult();
        //    }
        //}
        public ActionResult HonorariumBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetHonorariumBillReport(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region Travel Bill Report

        public ActionResult TravelBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                //int role = Common.GetRoleId(loginuser);
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/TravelBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                //  html = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/Policy/FullPoliticalViolenceMRCSlipDraft?QuoteID=" + QuoteId + "&&QuoteType=" + Quotetype;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 0;

                // instantiate a html to pdf converter object
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);

                // save pdf document
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=TravelBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {

                //writeError(ex);
                return new EmptyResult();
            }
        }
        public ActionResult TravelBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetTravelBillReportDetails(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region General Voucher Bill Report
        public ActionResult GeneralVoucherBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/GeneralVoucherBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=GeneralVoucherBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult GeneralVoucherBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetGeneralVoucherBillReport(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region Bill Report
        public ActionResult BillReport(int Id = 0)
        {
            BillReportModel model = new BillReportModel();
            return View(model);
        }
        public ActionResult BillReportPdf(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                //int role = Common.GetRoleId(loginuser);
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/BillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                //  html = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/Policy/FullPoliticalViolenceMRCSlipDraft?QuoteID=" + QuoteId + "&&QuoteType=" + Quotetype;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 0;

                // instantiate a html to pdf converter object
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                converter.Options.MarginTop = 15;
                converter.Options.MarginBottom = 15;
                // page numbers can be added using a PdfTextSection object
                PdfTextSection text = new PdfTextSection(0, 2, "Page: {page_number} of {total_pages}  ", new System.Drawing.Font("Arial", 8));
                text.HorizontalAlign = PdfTextHorizontalAlign.Right;
                converter.Footer.Add(text);
                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);

                // save pdf document
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=Proposal.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {

                //writeError(ex);
                return new EmptyResult();
            }
        }
        public ActionResult BillDetails(int Id, string LoginUser = "")
        {
            BillReportModel model = new BillReportModel();
            try
            {
                CoreAccountsService coreAccountService = new CoreAccountsService();

                model = coreAccountService.BillDetail(Id);
                var username = User.Identity.Name;
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
        #region Commitment Bill Report
        public ActionResult CommitmentBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/CommitmentBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=GeneralVoucherBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult CommitmentBillDetails(int Id = 0, string LoginUser = "")
        {
            ProjSummaryModel model = new ProjSummaryModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetCommitmentBillReport(Id);
                string logged_in_user = User.Identity.Name;
                int logged_in_user_id = Common.GetUserid(logged_in_user);
                model.LoginTS = String.Format("{0:ddd dd-MM-yy h:mm tt}", DateTime.Now);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region Invoice Bill Report
        public ActionResult InvoiceBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/InvoicetBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=InvoiceBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult InvoicetBillDetails(int Id = 0, string LoginUser = "")
        {
            InvoiceReportModel model = new InvoiceReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = ReportService.GetInvoiceReport(Id);
                //model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region AnnualAccounts Report
        public ActionResult AnnualAccounts()
        {
            AnnualAccounts model = new AnnualAccounts();
            ViewBag.ProjectType = Common.GetCodeControlList("AnnualAccounts");
            return View(model);
        }
        #endregion

        #region Provisional Statement
        public ActionResult ProvisionalStatement()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult ProvisionalStatementReport(string FromDate, string ToDate, int ProjectId)
        {
            try
            {

                string loginuser = User.Identity.Name;
                //int role = Common.GetRoleId(loginuser);
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/ProvisionalStatementReportDetails?FromDate=" + FromDate + "&ToDate=" + ToDate + "&ProjectId=" + ProjectId;
                //  html = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/Policy/FullPoliticalViolenceMRCSlipDraft?QuoteID=" + QuoteId + "&&QuoteType=" + Quotetype;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                string pdf_orientation = "Landscape";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 0;

                // instantiate a html to pdf converter object
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);

                // save pdf document
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename= ProvisionalStatement.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {

                //writeError(ex);
                return new EmptyResult();
            }
        }
        public ActionResult ProvisionalStatementReportDetails(string FromDate, string ToDate, int ProjectId)
        {
            ProvisionalStatementReportModel model = new ProvisionalStatementReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = ReportService.GetProvisionalStatement(FromDate, ToDate, ProjectId);
                string logged_in_user = User.Identity.Name;
                int logged_in_user_id = Common.GetUserid(logged_in_user);
                model.LoginTS = String.Format("{0:ddd dd-MM-yy h:mm tt}", DateTime.Now);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        
        #region Receipt Voucher
        public ActionResult ReceiptVoucher(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/ReceiptVoucherDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=ReceiptVoucher.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult ReceiptVoucherDetails(int Id = 0, string LoginUser = "")
        {
            ReceiptVoucherModel model = new ReceiptVoucherModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = ReportService.GetReceiptVoucher(Id);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region ReceiptOverHead
        public ActionResult ReceiptOverHead()
        {
            return View();
        }
        #endregion
        
        #region DistributionReport
        public ActionResult DistributionReport()
        {
            ReceiptReportModel model = new ReceiptReportModel();
            FinOp fo = new FinOp(System.DateTime.Now);
            DateTime Today = System.DateTime.Now;
            ViewBag.Month = fo.GetAllMonths();
            return View(model);
        }
        public ActionResult DistributionSummary(string Month = "")
        {
            TrailBalanceModel model = new TrailBalanceModel();
            ReportDocument rd = new ReportDocument();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var UserName = User.Identity.Name;
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "DistribytionSummary.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    rd.SetParameterValue("Month", Month);
                    Stream stream;
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelWorkbook);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=DistributionSummary.xls");
                    return File(stream, "application/vnd.ms-excel");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Report", new { Errormsg = "Something went to wrong please contact admin." });
            }
        }
        public ActionResult DistributionBreakUp(string Month = "")
        {
            TrailBalanceModel model = new TrailBalanceModel();
            ReportDocument rd = new ReportDocument();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var UserName = User.Identity.Name;
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "DistributionBreakUp.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    rd.SetParameterValue("Month", Month);
                    Stream stream;
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelWorkbook);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=DistributionBreakUp.xls");
                    return File(stream, "application/vnd.ms-excel");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Report", new { Errormsg = "Something went to wrong please contact admin." });

            }
        }
        #endregion
        #region SalaryReport
      
        public ActionResult CanaraBankSalary(int SalaryId = 0)
        {
            TrailBalanceModel model = new TrailBalanceModel();
            ReportDocument rd = new ReportDocument();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var UserName = User.Identity.Name;
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "CanaraBankSalary.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    rd.SetParameterValue("SalaryId", SalaryId);
                    Stream stream;
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=CanaraBankSalary.pdf");
                    return File(stream, "application/pdf");
                   
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Report", new { Errormsg = "Something went to wrong please contact admin." });
            }
        }
        public ActionResult NonCanaraBankSalary(int SalaryId = 0)
        {
            TrailBalanceModel model = new TrailBalanceModel();
            ReportDocument rd = new ReportDocument();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var UserName = User.Identity.Name;
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "NonCanaraBankSalary.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    rd.SetParameterValue("SalaryId", SalaryId);
                    Stream stream;
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=NonCanaraBankSalary.pdf");
                    return File(stream, "application/pdf");
                 
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Report", new { Errormsg = "Something went to wrong please contact admin." });

            }
        }
        #endregion
        #region Long Bill
        public ActionResult LongBill(int Id)
        {
            LongBill model = new LongBill();
            try
            {
                model = ReportService.GetLongBill(Id);
                var yourpdf = new PartialViewAsPdf(model)
                {
                    RotativaOptions = new DriverOptions()
                    {
                        PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                        PageSize = Rotativa.Core.Options.Size.A4,
                        IsLowQuality = true,
                        PageMargins = new Margins(0, 0, 0, 0)
                    }
                };
                return yourpdf;
            }
            catch (Exception ex)
            {
                return new Rotativa.MVC.ViewAsPdf("LongBill", model);
            }
        }
        #endregion
        #region Receipt Report
        public ActionResult ReceiptReport()
        {
            ReceiptReportModel model = new ReceiptReportModel();
            FinOp fo = new FinOp(System.DateTime.Now);
            DateTime Today = System.DateTime.Now;
            ViewBag.Month = fo.GetAllMonths();
            return View(model);

        }
        #endregion

        #region Office Monthly Report
        public ActionResult OfficeMonthlyReport()
        {
            OfficeMonthlyReportModel model = new OfficeMonthlyReportModel();
            FinOp fo = new FinOp(System.DateTime.Now);
            DateTime Today = System.DateTime.Now;
            ViewBag.months = fo.GetAllMonths();
            return View(model);
        }
        public ActionResult OfficeMonthlyBill(string Date = "")
        {
            OfficeMonthlyReportModel model = new OfficeMonthlyReportModel();
            try
            {
                FinOp fac = new FinOp(System.DateTime.Now);
                var fromdate = fac.GetMonthFirstDate(Date);
                var todate = fac.GetMonthLastDate(Date);
                DateTime startDate = new DateTime(fromdate.Year, 4, 1); // 1st Feb this year
                DateTime endDate = fromdate.AddDays(-1);
                ReportService reportservice = new ReportService();
                model = ReportService.GetOfficeMonthlyReport(Date);
                model.CurrMonth = Date;
                //model.FromDate = String.Format("{0:MMM yyyy}", startDate);
                //model.ToDate = String.Format("{0:MMM yyyy}", Date);
                return new Rotativa.MVC.ViewAsPdf("OfficeMonthlyBillDetails", model)
                {


                };
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult OfficeMonthlyBillDetails(string Date = "")
        {
            OfficeMonthlyReportModel model = new OfficeMonthlyReportModel();
            try
            {
                FinOp fac = new FinOp(System.DateTime.Now);
                var fromdate = fac.GetMonthFirstDate(Date);
                var todate = fac.GetMonthLastDate(Date);
                DateTime startDate = new DateTime(fromdate.Year, 4, 1); // 1st Feb this year
                DateTime endDate = fromdate.AddDays(-1);
                ReportService reportservice = new ReportService();
                model = ReportService.GetOfficeMonthlyReport(Date);
                model.CurrMonth = Date;
                //model.FromDate = String.Format("{0:MMM yyyy}", startDate);
                var yourpdf = new PartialViewAsPdf(model)
                {
                    RotativaOptions = new DriverOptions()
                    {
                        PageOrientation = Rotativa.Core.Options.Orientation.Landscape,
                        PageSize = Rotativa.Core.Options.Size.A4,
                        IsLowQuality = false,
                        PageMargins = new Margins(0, 0, 0, 0)
                    }
                };
                return yourpdf;
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region Sponser Office Monthly Report
        public ActionResult SponserOfficeReport()
        {
            OfficeMonthlyReportModel model = new OfficeMonthlyReportModel();
            FinOp fo = new FinOp(System.DateTime.Now);
            DateTime Today = System.DateTime.Now;
            ViewBag.months = fo.GetAllMonths();
            return View(model);
        }
        public ActionResult SponserOfficeMonthlyReport(string Date = "")
        {
            OfficeMonthlyReportModel model = new OfficeMonthlyReportModel();
            try
            {
                FinOp fac = new FinOp(System.DateTime.Now);
                var fromdate = fac.GetMonthFirstDate(Date);
                var todate = fac.GetMonthLastDate(Date);
                DateTime startDate = new DateTime(fromdate.Year, 4, 1); // 1st Feb this year
                DateTime endDate = fromdate.AddDays(-1);
                ReportService reportservice = new ReportService();
                model = ReportService.GetSponserMonthlyReport(Date);
                model.CurrMonth = Date;
                //model.FromDate = String.Format("{0:MMM yyyy}", startDate);
                var yourpdf = new PartialViewAsPdf(model)
                {
                    RotativaOptions = new DriverOptions()
                    {
                        PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                        PageSize = Rotativa.Core.Options.Size.A4,
                        IsLowQuality = false,
                        PageMargins = new Margins(0, 0, 0, 0)
                    }
                };
                return yourpdf;
                //return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }

        #endregion
        #region Project Summary
        public ActionResult ProjectSummary(int ProjectId)
        {
            ProjSummaryModel model = new ProjSummaryModel();
            ProjectSummaryModel psModel = new ProjectSummaryModel();
            ProjectService pro = new ProjectService();
            try
            {
                if (ProjectId > 0)
                {
                    model.Detail = pro.getProjectSummaryDetails(ProjectId);
                    model.Summary = pro.getProjectSummary(ProjectId);
                    model.Common = Common.GetProjectsDetails(ProjectId);
                    model.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", model.Common.SancationDate);
                    model.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", model.Common.CloseDate);
                    model.ProjId = ProjectId;
                    model.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                    model.ExpAmt = model.Summary.AmountSpent;
                }
                else
                    model.Summary = psModel;
                string logged_in_user = User.Identity.Name;
                int logged_in_user_id = Common.GetUserid(logged_in_user);
                model.LoginTS = String.Format("{0:ddd dd-MM-yy h:mm tt}", DateTime.Now);
                var yourpdf = new PartialViewAsPdf(model)
                {
                    RotativaOptions = new DriverOptions()
                    {
                        PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                        PageSize = Rotativa.Core.Options.Size.A4,
                        IsLowQuality = false,
                        PageMargins = new Margins(0, 0, 0, 0)
                    }
                };
                return yourpdf;
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region TravelReport
        [HttpGet]
        public ActionResult TravelReport(string Errormsg)
        {
            ViewBag.travel = Common.GetTravelDetails();
            if (Errormsg != null)
            {
                ViewBag.error = Errormsg;
            }
            return View();
        }
        public ActionResult TravelReportDetails(TravelReportModel model)
        {
            try
            {
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "TravelReport.rpt"));
                for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                    rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                model.ToDate = model.ToDate.Date.AddDays(1).AddTicks(-2);
                rd.SetParameterValue("FromDate", model.FromDate);
                rd.SetParameterValue("ToDate", model.ToDate);
                if (model.TravelType != null)
                {
                    string tav = string.Empty;
                    if (model.TravelType == 1)
                    {
                        tav = "TAD";
                        rd.SetParameterValue("Transactype", tav);
                    }
                    else if (model.TravelType == 2)
                    {
                        tav = "TST";
                        rd.SetParameterValue("Transactype", tav);
                    }
                    else if (model.TravelType == 3)
                    {
                        tav = "DTV";
                        rd.SetParameterValue("Transactype", tav);
                    }
                }
                if (model.BillNumber != null)
                    rd.SetParameterValue("BillNo", model.BillNumber);
                if (model.PayeeName != null)
                    rd.SetParameterValue("Payname", model.PayeeName);
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                Response.AddHeader("Content-Disposition", "inline; filename=TravelReport.pdf");
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                return RedirectToAction("TravelReport", new { Errormsg = "Something went to wrong please contact admin." });
            }
        }
        [HttpGet]
        public JsonResult LoadBillNo(string term)
        {
            try
            {
                var data = Common.GettravelBillNo(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Vendor Report
        public ActionResult VendorReport()
        {
            VendorReportModel model = new VendorReportModel();
            ViewBag.dropdownbilltype = Common.GetCodeControlList("VendorReport");
            return View(model);
        }
        public ActionResult vendorSummary(string Payeeid, DateTime date1, DateTime date2)
        {
            VendorReportModel model = new VendorReportModel();
            ReportDocument rd = new ReportDocument();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    string name = Payeeid;
                    var UserName = User.Identity.Name;
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "Vendor.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    rd.SetParameterValue("date1", date1);
                    rd.SetParameterValue("date2", date2);
                    rd.SetParameterValue("name", name);
                    Stream stream;
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelWorkbook);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=Vendor.xls");
                    return File(stream, "application/vnd.ms-excel");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Report", new { Errormsg = "Something went to wrong please contact admin." });
            }
        }
        #endregion
        #region Claim Bill Print
        public ActionResult ClaimBill(int Id)
        {
            InvoiceReportModel model = new InvoiceReportModel();
            try
            {
                model = ReportService.GetClaimBillReport(Id);
                var yourpdf = new PartialViewAsPdf(model)
                {
                    RotativaOptions = new DriverOptions()
                    {
                        PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                        PageSize = Rotativa.Core.Options.Size.A4,
                        IsLowQuality = true,
                        PageMargins = new Margins(0, 0, 0, 0)
                    }
                };
                return yourpdf;
            }
            catch (Exception ex)
            {
                return new Rotativa.MVC.ViewAsPdf("ClaimBill", model);
            }
        }
        #endregion
        #region InterestRefund
        public ActionResult InterestRefundReport()
        {
            try
            {

                ViewBag.finyr = new List<MasterlistviewModel>();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.finyr = new List<MasterlistviewModel>();
                return View();
            }
        }
        [HttpGet]
        public JsonResult GetFinYear(int pId)
        {
            try
            {
                object output = Common.GetFinYearList(pId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult InterestRefundReportDetails(int finYearId, int projectId = 0)
        {
            try
            {
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/InterestRefundReportView?finYearId=" + finYearId + "&projectId=" + projectId;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                string pdf_orientation = "Landscape";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 200;
                // add a new page to the document

                // instantiate a html to pdf converter object
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.MaxPageLoadTime = 600;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                converter.Options.MarginLeft = 30;
                //converter.Options.MarginRight = 20;

                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);

                // save pdf document
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename= ProjectInterestReport.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult InterestRefundReportView(int finYearId, int projectId = 0)
        {
            InterestRefundReportModel model = new InterestRefundReportModel();
            List<InterestRefundMonthReport> listmodel = new List<InterestRefundMonthReport>();
            try
            {
                if (projectId > 0)
                {
                    model = ReportService.GetInterestRefundMonth(finYearId, projectId);
                }
                else
                    model.Monthlist = listmodel;
                //var yourpdf = new PartialViewAsPdf(model)
                //{
                //    RotativaOptions = new DriverOptions()
                //    {
                //        PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                //        PageSize = Rotativa.Core.Options.Size.A4,
                //        IsLowQuality = false,
                //        PageMargins = new Margins(0, 0, 0, 0)
                //    }
                //};
                //return yourpdf;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(this, ex);
                return PartialView(model);
            }
        }

        #endregion
        #region Contraprint
        public ActionResult ContractorBillViewFile(int ContractID)
        {
            ContractorBillModel model = new ContractorBillModel();
            List<ContractorBillDetailsModel> list = new List<ContractorBillDetailsModel>();
            try
            {

                model = ReportService.GetContractorBill(ContractID);
                var yourpdf = new PartialViewAsPdf(model)
                {
                    RotativaOptions = new DriverOptions()
                    {
                        PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                        PageSize = Rotativa.Core.Options.Size.A4,
                        IsLowQuality = true,
                        PageMargins = new Margins(0, 0, 0, 0)
                    }
                };

                return yourpdf;


            }
            catch (Exception ex)
            {
                return new Rotativa.MVC.ViewAsPdf("ContractorBillViewFile", model);
            }

        }
        #endregion

        #region ContingentBillPrint
        public ActionResult ContingentBillPrintFile(int ContingentId = 0)
        {
            ContingentBillModel model = new ContingentBillModel();
            model = ReportService.GetContingentBill(ContingentId);

            try
            {
                model = ReportService.GetContingentBill(ContingentId);
                var yourpdf = new PartialViewAsPdf(model)
                {
                    RotativaOptions = new DriverOptions()
                    {
                        PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                        PageSize = Rotativa.Core.Options.Size.A4,
                        IsLowQuality = true,
                        PageMargins = new Margins(0, 0, 0, 0)
                    }
                };
                return yourpdf;
            }
            catch (Exception ex)
            {
                return new Rotativa.MVC.ViewAsPdf("ContingentBillPrintFile", model);
            }
        }
        #endregion
        #region HeadCredit  Print
        public ActionResult HeadCredit(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                //int role = Common.GetRoleId(loginuser);
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/HeadCreditPrint?Id=" + Id + "&&LoginUser=" + loginuser;
                //  html = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/Policy/FullPoliticalViolenceMRCSlipDraft?QuoteID=" + QuoteId + "&&QuoteType=" + Quotetype;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 0;

                // instantiate a html to pdf converter object
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);

                // save pdf document
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=TravelBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {

                //writeError(ex);
                return new EmptyResult();
            }
        }
        public ActionResult HeadCreditPrint(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetHeadCreditPrintDetails(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        #region GSTR 
        public ActionResult GSTR()
        {
            return View();
        }
        #endregion
        #region Project Schemes 
        public ActionResult SchemeCodeWiseBalance()
        {
            return View();
        }
        public ActionResult ProjectSchemes()
        {
            return View();
        }
        #endregion
        #region Financial Report
        public ActionResult FinancialReport()
        {
            var emptyList = new List<ProjectExpDateModel>();
            ViewBag.YearList = emptyList;
            return View();
        }
        public ActionResult ProjectFinancialReport()
        {

            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetProjectExpyear(int projid)
        {
            var locationdata = Common.GetProjectExpYear(projid);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Credit Note Bill Print
        public ActionResult CreditNote(int Id)
        {
            InvoiceReportModel model = new InvoiceReportModel();
            try
            {
                model = ReportService.GetCreditnoteBillReport(Id);
                var yourpdf = new PartialViewAsPdf(model)
                {
                    RotativaOptions = new DriverOptions()
                    {
                        PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                        PageSize = Rotativa.Core.Options.Size.A4,
                        IsLowQuality = true,
                        PageMargins = new Margins(0, 0, 0, 0)
                    }
                };
                return yourpdf;
            }
            catch (Exception ex)
            {
                return new Rotativa.MVC.ViewAsPdf("CreditNote", model);
            }
        }
        #endregion
        #region TrailBalance
        public ActionResult TrailBalanceReport()
        {
            ViewBag.FinYr = GetFinancialYear();
            return View();
        }
        public ActionResult TrailBalanceRep(int Finyear, int format)
        {
            TrailBalanceModel model = new TrailBalanceModel();
            ReportDocument rd = new ReportDocument();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var UserName = User.Identity.Name;
                    //                var Qry = (from b in context.vw_DemoLedgers
                    //                           where String.IsNullOrEmpty(accounts) || b.Accounts.Contains(accounts)
                    //                           select new
                    //                           {
                    //                               b.AccountGroupId,
                    //                               b.AccountHead,
                    //                               b.AccountHeadId,
                    //                               b.Accounts,
                    //                               b.Amount,
                    //                               b.Creditor_f,
                    //                               b.Debtor_f,
                    //                               b.Groups,
                    //                               b.TransactionType
                    //                           }).ToList();
                    //                var AssetCr = Qry.Where(m => m.TransactionType == "Credit" && m.Accounts == "Asset")
                    //    .GroupBy(a => a.AccountHeadId)
                    //    .Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    //    .OrderByDescending(a => a.Amount)
                    //    .ToList();

                    //                var AssetDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Asset")
                    // .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    // .OrderByDescending(a => a.Amount)
                    // .ToList();
                    //                decimal? Ass = 0, ttCr = 0, ttDr = 0;
                    //                var Qray=(from Dr in AssetDr
                    //                          join Cr in AssetCr on Dr.Name equals Cr.Name into temp
                    //                         from Cr in temp.DefaultIfEmpty()
                    //                         select new TrailBalanceModel()
                    //                         {   HeadId=Convert.ToInt32(Dr.Name),
                    //                             Debit = Convert.ToDecimal(Dr.Amount),
                    //                             Credit = Convert.ToDecimal(Cr?.Amount),
                    //                         }).ToList();
                    //                for (int i = 0; i < Qray.Count; i++)
                    //                {
                    //                    Ass = Qray[i].Debit - Qray[i].Credit;
                    //                    if (Ass < 0)
                    //                        ttCr += (-Ass);
                    //                    else
                    //                        ttDr += (Ass);
                    //                }

                    //                string TotalAssetDr = Convert.ToString(ttDr ?? 0);                                       
                    //                string TotalAssetCr = Convert.ToString(ttCr ?? 0);

                    //                var LiabilityCr = Qry.Where(m => m.TransactionType == "Credit" && m.Accounts == "Liability")
                    // .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    // .OrderByDescending(a => a.Amount)
                    // .ToList();

                    //                var LiabilityDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Liability")
                    //.GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    //.OrderByDescending(a => a.Amount)
                    //.ToList();
                    //                string TotalLiabilityDr = Convert.ToString(LiabilityDr.Sum(m => m.Amount) ?? 0);
                    //                string TotalLiabilityCr = Convert.ToString(LiabilityCr.Sum(m => m.Amount) ?? 0);
                    //                var IncomeCr = Qry.Where(m => m.TransactionType == "Credit" && m.Accounts == "Income")
                    //             .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    //             .OrderByDescending(a => a.Amount)
                    //             .ToList();
                    //                var IncomeDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Income")
                    //.GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    //.OrderByDescending(a => a.Amount)
                    //.ToList();
                    //                string TotalIncomeDr = Convert.ToString(IncomeDr.Sum(m => m.Amount) ?? 0);
                    //                string TotalIncomeCr = Convert.ToString(IncomeCr.Sum(m => m.Amount) ?? 0);
                    //                var ExpenseCr = Qry.Where(m => m.TransactionType == "Credit" && m.Accounts == "Expense")
                    //           .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    //           .OrderByDescending(a => a.Amount)
                    //           .ToList();

                    //                var ExpenseDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Expense")
                    //.GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    //.OrderByDescending(a => a.Amount)
                    //.ToList();
                    //                string TotaExpenseDr = Convert.ToString(ExpenseDr.Sum(m => m.Amount) ?? 0);
                    //                string TotalExpenseCr = Convert.ToString(ExpenseCr.Sum(m => m.Amount) ?? 0);
                    string conn = "IOASDB";
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "TrailBalance2.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    var data = ReportService.TrailBalanceRep2(Finyear);
                    rd.SetDataSource(data);
                    rd.SetParameterValue("Finyear", Common.GetFinancialYear(Finyear));
                    rd.SetParameterValue("UserName", UserName);
                    Stream stream;
                    if (format == 1)
                    {
                        stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        Response.AddHeader("Content-Disposition", "inline; filename=TrailBalance(" + Common.GetFinancialYear(Finyear) + ").pdf");
                        return File(stream, "application/pdf");
                    }
                    else
                    {
                        stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                        stream.Seek(0, SeekOrigin.Begin);
                        Response.AddHeader("Content-Disposition", "inline; filename=TrailBalance(" + Common.GetFinancialYear(Finyear) + ").xls");
                        return File(stream, "application/vnd.ms-excel");
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Report", new { Errormsg = "Something went to wrong please contact admin." });

            }
        }
        public ActionResult GetDataForTrailBalance2(int Finyear)
        {
            var em = 0;
            var data = new object();
            var Qry = ReportService.TrailBalanceRep2(Finyear);
            if (Qry.Count > 0)
            {
                em = 1;
            }
            else { em = 2; }
            data = new { em = em };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult IsrohIncomeAndExp()
        {
            ViewBag.Finyr = Common.GetAllFinancial();
            return View();
        }
        public ActionResult ResearchFund()
        {
            ViewBag.Finyr = Common.GetAllFinancial();
            return View();
        }
        public ActionResult NIRF()
        {
            return View();
        }
        public ActionResult ExpenditureReportwithTax()
        {
            return View();
        }
        public ActionResult ReceiptTransfer()
        {
            ReceiptReportModel model = new ReceiptReportModel();
            FinOp fo = new FinOp(System.DateTime.Now);
            DateTime Today = System.DateTime.Now;
            ViewBag.Month = fo.GetAllMonths();
            return View(model);
        }
        public ActionResult Annexuresalary()
        {
            ViewBag.Finyr = Common.GetAllFinancial();
            return View();
        }
        public ActionResult LedgerBalance()
        {
            ViewBag.Accountlist = Common.GetAccountHeadList();
            return View();
        }
        public ActionResult BalanceSheet()
        {
            ViewBag.Finyr = Common.GetAllFinancial();
            return View();
        }
        public ActionResult SOE()
        {
            return View();
        }
        #region Admin Voucher Bill Report
        public ActionResult AdminVoucherBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/AdminVoucherBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=GeneralVoucherBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult AdminVoucherBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetAdminVoucherBillReport(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region Mandays Bill Report
        public ActionResult MandaysBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/MandaysBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=HonorariumBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult MandaysBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetMandaysBillReport(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region AdminSalary Bill Report
        public ActionResult AdminSalary(int ID = 0)
        {
            TrailBalanceModel model = new TrailBalanceModel();
            ReportDocument rd = new ReportDocument();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var UserName = User.Identity.Name;
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "AdminSalary.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    rd.SetParameterValue("ID", ID);
                    Stream stream;
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=CanaraBankSalary.pdf");
                    return File(stream, "application/pdf");

                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Report", new { Errormsg = "Something went to wrong please contact admin." });
            }
        }
        public ActionResult AdminSalaryBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/AdminSalaryBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=HonorariumBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult AdminSalaryBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetAdminSalaryBillReport(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region Fellowship Bill Report
        public ActionResult FellowshipBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/FellowshipBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=HonorariumBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult FellowshipBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetFellowshipBillReport(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region Distribution Bill Report
        public ActionResult DistributionBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/DistributionBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=HonorariumBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult DistributionBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetDistributionBillReport(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region Old Receipt and Exp
        public ActionResult OldRecandExp()
        {
            return View();
        }
        #endregion
        #region TDSPayment Bill Report
        public ActionResult TDSPaymentBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/TDSPaymentBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=HonorariumBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult TDSPaymentBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetTdsPaymentBillReport(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region InterestRefund SP
        public ActionResult InterestRefund()
        {
            ViewBag.FinYear = Common.GetFinYearList();
            return View();
        }
        [HttpPost]
        public ActionResult InterestRefund(InterestRefundMonthReport model)
        {
            ListDatabaseObjects listdata = new ListDatabaseObjects();
            CoreAccountsService coreaccountService = new CoreAccountsService();
            using (var context = new IOASDBEntities())
            {
                var FromDate = context.tblFinYear.Where(m => m.FinYearId == model.FinYear).Select(m => m.StartDate).FirstOrDefault() ?? DateTime.Now;

                ViewBag.FinYear = Common.GetFinYearList();
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                listdata.ExceuteInterestRefund(model.FinYear, logged_in_user);
                TempData["succMsg"] = "Interest is calculating in Back end .It will take more than a hour";
                return View(model);
            }
        }
        [HttpPost]
        public JsonResult GetInterestRefund(int FinId)
        {
            try
            {
                object output = ReportService.CheckInterestRefundRunning(FinId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        public ActionResult BRSReconciled()
        {
            ViewBag.Bank = Common.GetAccountHeadList(38);
            return View();
        }
        #region TDSPayment Bill Report
        public ActionResult SRBBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/SRBDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=HonorariumBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult SRBDetails(int Id = 0, string LoginUser = "")
        {
            SRBDetailsModel model = new SRBDetailsModel();
            try
            {               
                FacilityService _fs = new FacilityService();

                model = _fs.GetSRBDetails(Id);
              
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        public ActionResult ProjectExpReport()
        {
            try
            {
                var data = Common.GetProjectExpSPStatus();
                return View(data);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetProjectExpSPStatus()
        {
            try
            {
                var data = Common.GetProjectExpSPStatus();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ExecuteProjectExpSP()
        {
            try
            {
                int id = Common.GetUserid(User.Identity.Name);
                //System.Threading.Tasks.Task.Run(() => Common.ExecuteProjectExpSP(id));
                var data = Common.ExecuteProjectExpSP(id);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult ProfitandLoss()
        {
            
            return View();
        }

        public ActionResult ReceiptsandPayments()
        {

            return View();
        }
        public ActionResult InvoicePrint(int Id)
        {
            InvoiceReportPrintModel model = new InvoiceReportPrintModel();
            try
            {
                model = ReportService.GetInvoicePrintReport(Id);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        public ActionResult DaywiseTapal()
        {
            return View();
        }


        #region GSTOffset Bill Report
        public ActionResult GSTOffsetBill(int Id = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/GSTOffsetBillDetails?Id=" + Id + "&&LoginUser=" + loginuser;
                string pdf_page_size = "A4";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);
                int webPageWidth = 1024;
                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=HonorariumBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        public ActionResult GSTOffsetBillDetails(int Id = 0, string LoginUser = "")
        {
            TravelBillReportModel model = new TravelBillReportModel();
            try
            {
                ReportService reportservice = new ReportService();
                model = reportservice.GetGstOffsetBillReport(Id);
                model.PrintedBy = LoginUser;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        #endregion
        #region PayinSlip
        public ActionResult Payinslipdownload(int PayinslipId)
        {
            try
            {
                //string loginuser = User.Identity.Name;
                using (var context = new IOASExternalEntities())
                {
                    var query = context.tblPayinSlip.FirstOrDefault(m => m.PayinslipId == PayinslipId);
                    string url = "";

                    url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/PayinslipOutput?id=" + PayinslipId;

                    string pdf_page_size = "A4";
                    SelectPdf.PdfPageSize pageSize =
                        (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                    string pdf_orientation = "Portrait";
                    SelectPdf.PdfPageOrientation pdfOrientation =
                        (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                        pdf_orientation, true);

                    int webPageWidth = 924;

                    int webPageHeight = 0;

                    // instantiate a html to pdf converter object
                    SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

                    // set converter options
                    converter.Options.PdfPageSize = pageSize;
                    converter.Options.PdfPageOrientation = pdfOrientation;
                    converter.Options.WebPageWidth = webPageWidth;
                    converter.Options.WebPageHeight = webPageHeight;

                    // create a new pdf document converting an url
                    SelectPdf.PdfDocument doc = converter.ConvertUrl(url);

                    // save pdf document
                    byte[] pdf = doc.Save();

                    // close pdf document
                    doc.Close();
                    Response.AddHeader("Content-Disposition", "inline; filename=Payinslip.pdf");
                    return File(pdf, "application/pdf");
                    // return resulted pdf document
                    //FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                    //if (query.PaymentBank == 20 && query.TypeofPayment == 1 && query.PurposeofRemittance == 1)
                    //{
                    //    fileResult.FileDownloadName = "Advance Import Request Letter_CB " + foreignRemitId + ".pdf";
                    //}
                    //if (query.PaymentBank == 20 && query.TypeofPayment == 2 && query.PurposeofRemittance == 1)
                    //{
                    //    fileResult.FileDownloadName = "Direct Import Request Letter_CB " + foreignRemitId + ".pdf";
                    //}
                    //if (query.PaymentBank == 21 && query.TypeofPayment == 1 && query.PurposeofRemittance == 1)
                    //{
                    //    fileResult.FileDownloadName = "Advance Import Request Letter_HDFC " + foreignRemitId + ".pdf";
                    //}
                    //if (query.PaymentBank == 21 && query.TypeofPayment == 2 && query.PurposeofRemittance == 1)
                    //{
                    //    fileResult.FileDownloadName = "Direct Import Request Letter_HDFC " + foreignRemitId + ".pdf";
                    //}
                    //if (query.PurposeofRemittance == 2 || query.PurposeofRemittance == 3 || query.PurposeofRemittance == 4 || query.PurposeofRemittance == 5)
                    //{
                    //    fileResult.FileDownloadName = "Other Services Request Letter_HDFC " + foreignRemitId + ".pdf";
                    //}
                    //return fileResult;
                }
            }
            catch (Exception ex)
            {

                return new EmptyResult();
            }
        }
        public ActionResult PayinslipOutput(int id)
        {
            try
            {
                ReportService rs = new ReportService();
                CreateInvoiceModel model = new CreateInvoiceModel();
                ProjectService projectService = new ProjectService();
                model = rs.GetPayinslip(id);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView("ErrOutput");
            }
        }
        #endregion
    }
}