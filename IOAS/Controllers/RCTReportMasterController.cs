using IOAS.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using IOAS.DataModel;
using CrystalDecisions.CrystalReports.Engine;
using IOAS.Infrastructure;
using System.IO;
using IOAS.GenericServices;
using System.Data;
using System.Configuration;
using ClosedXML.Excel;
using System.Data.SqlClient;
namespace IOAS.Controllers
{
    public class RCTReportMasterController : Controller
    {

        RequirementService RQS = new RequirementService();
        ErrorHandler WriteLog = new ErrorHandler();

        string strServer = ConfigurationManager.AppSettings["ServerName"].ToString();
        string strDatabase = ConfigurationManager.AppSettings["DataBaseName"].ToString();
        string strUserID = ConfigurationManager.AppSettings["UserId"].ToString();
        string strPwd = ConfigurationManager.AppSettings["Password"].ToString();

        public ActionResult CommitteeConsolidateReport()
        {
            CashBookModel model = new CashBookModel();
            return View(model);
        }

        public ActionResult GetApprovedApplicationsReport(DateTime fromdate, DateTime todate)
        {
            using (var context = new IOASDBEntities())
            {
                var userID = Common.GetUserid(User.Identity.Name);
                //using (var transaction = context.Database.BeginTransaction())
                //{
                try
                {
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "ApprovedApplicationsReport.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    var datas = (from m in context.vw_RCTApprovedApplicationsReport.AsNoTracking()
                                 where m.CRTD_TS >= fromdate && m.CRTD_TS <= todate
                                 select new
                                 {
                                     m.ApplicationId,
                                     m.Category,
                                     m.CandidateName,
                                     m.PostRecommended,
                                     m.ProjectNumber,
                                     m.AppointmentStartdate,
                                     m.AppointmentEnddate,
                                     m.CRTD_TS,
                                     m.BasicPay,
                                     m.TypeofAppointment,
                                     m.PIName,
                                     m.CheckList,
                                     m.CommiteeApproveName,
                                     m.ApplicationType,
                                     m.SalaryLevel,
                                     m.EmployeeNo,
                                     m.ApplicationReceivedDate,
                                     m.ApprovalInitiatedDate,
                                     m.ApprovedDate,
                                     m.CommitmentBookedDate,
                                     m.CommiteeApprovedDate,
                                     m.Offerreleaseddate
                                 }).AsEnumerable()
                    .Select((x) => new ComitteeApprovalDetailReportModel()
                    {
                        EmployeeID = x.EmployeeNo,
                        ApplicationReceivedDate = x.ApplicationReceivedDate ?? DateTime.Now,
                        SalaryLevel = x.SalaryLevel,
                        ApprovalInitiatedDate = x.ApprovalInitiatedDate,
                        ApprovedDate = x.ApprovedDate,
                        CommitmentBookedDate = x.CommitmentBookedDate,
                        CommitteeApprovedDate = x.CommiteeApprovedDate ?? DateTime.Now,
                        OfferReleasedDate = x.Offerreleaseddate,
                        CandidateName = x.CandidateName,
                        Category = x.Category,
                        ApplicationId = x.ApplicationId ?? 0,
                        PostRecommended = x.PostRecommended,
                        ProjectNumber = x.ProjectNumber,
                        AppointmentStartdate = x.AppointmentStartdate ?? DateTime.Now,
                        AppointmentEnddate = x.AppointmentEnddate ?? DateTime.Now,
                        CRTD_TS = x.CRTD_TS ?? DateTime.Now,
                        BasicPay = x.BasicPay ?? 0,
                        TypeofAppointment = x.TypeofAppointment,
                        ApplicationType = x.ApplicationType,
                        PIName = x.PIName,
                        CheckList = x.CheckList,
                        CommiteeApproveName = x.CommiteeApproveName
                    }).ToList();
                    rd.SetDataSource(datas);
                    var listcommitte = Common.GetCommittee();
                    if (listcommitte.Item1.Count > 0)
                    {
                        for (int i = 0; i < listcommitte.Item1.Count; i++)
                        {
                            if (i == 0)
                            {
                                rd.SetParameterValue("FirstMember", listcommitte.Item1[i].name);
                            }
                            if (i == 1)
                            {
                                rd.SetParameterValue("SecondMember", listcommitte.Item1[i].name);
                            }
                        }
                    }
                    var datacharperson = Common.GetChairPerson();
                    if (datacharperson != null)
                        rd.SetParameterValue("ChairPerson", datacharperson.Item2);
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=ApprovedReport.pdf");

                    //foreach (var item in datas)
                    //{
                    //    tblRCTApprovedApplicationReport rpt = new tblRCTApprovedApplicationReport();
                    //    rpt.ApplicationCategory = item.Category;
                    //    rpt.ApplicationId = item.ApplicationId;
                    //    rpt.CRTD_By = userID;
                    //    rpt.CRTD_TS = DateTime.Now;
                    //    context.tblRCTApprovedApplicationReport.Add(rpt);
                    //    context.SaveChanges();
                    //}
                    //transaction.Commit();
                    return File(stream, "application/pdf");
                }
                catch (Exception ex)
                {
                    TempData["errMsg"] = "Something went to wrong please contact admin.";
                    return RedirectToAction("CommitteeConsolidateReport", "RCTReportMaster");
                }
                //}
            }
        }
        public ActionResult GetApprovedApplicationsReportExcel(DateTime fromdate, DateTime todate)
        {
            using (var context = new IOASDBEntities())
            {
                var userID = Common.GetUserid(User.Identity.Name);
                try
                {
                    var datas = (from m in context.vw_RCTApprovedApplicationsReport.AsNoTracking()
                                 where m.CRTD_TS >= fromdate && m.CRTD_TS <= todate
                                 select new
                                 {
                                     m.ApplicationId,
                                     m.Category,
                                     m.CandidateName,
                                     m.PostRecommended,
                                     m.ProjectNumber,
                                     m.AppointmentStartdate,
                                     m.AppointmentEnddate,
                                     m.CRTD_TS,
                                     m.BasicPay,
                                     m.TypeofAppointment,
                                     m.PIName,
                                     m.CheckList,
                                     m.CommiteeApproveName,
                                     m.ApplicationType,
                                     m.SalaryLevel,
                                     m.EmployeeNo,
                                     m.ApplicationReceivedDate,
                                     m.ApprovalInitiatedDate,
                                     m.ApprovedDate,
                                     m.CommitmentBookedDate,
                                     m.CommiteeApprovedDate,
                                     m.Offerreleaseddate
                                 }).AsEnumerable()
                  .Select((x) => new ComitteeApprovalDetailReportModel()
                  {
                      EmployeeID = x.EmployeeNo,
                      ApplicationReceivedDate = x.ApplicationReceivedDate ?? DateTime.Now,
                      SalaryLevel = x.SalaryLevel,
                      ApprovalInitiatedDate = x.ApprovalInitiatedDate,
                      ApprovedDate = x.ApprovedDate,
                      CommitmentBookedDate = x.CommitmentBookedDate,
                      CommitteeApprovedDate = x.CommiteeApprovedDate ?? DateTime.Now,
                      OfferReleasedDate = x.Offerreleaseddate,
                      CandidateName = x.CandidateName,
                      Category = x.Category,
                      ApplicationId = x.ApplicationId ?? 0,
                      PostRecommended = x.PostRecommended,
                      ProjectNumber = x.ProjectNumber,
                      AppointmentStartdate = x.AppointmentStartdate ?? DateTime.Now,
                      AppointmentEnddate = x.AppointmentEnddate ?? DateTime.Now,
                      CRTD_TS = x.CRTD_TS ?? DateTime.Now,
                      BasicPay = x.BasicPay ?? 0,
                      TypeofAppointment = x.TypeofAppointment,
                      ApplicationType = x.ApplicationType,
                      PIName = x.PIName,
                      CheckList = x.CheckList,
                      CommiteeApproveName = x.CommiteeApproveName
                  }).ToList();
                    MemoryStream workStream = new MemoryStream();
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        DataTable dtResult = new DataTable();
                        var ws = wb.Worksheets.Add("CommitteeReport");
                        int Firstrow = 1;
                        if (datas.Count > 0)
                        {
                            ws.Cell(Firstrow, 1).InsertTable(datas);
                        }
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                    }


                    string fileType = Common.GetMimeType("xls");
                    Response.AddHeader("Content-Disposition", "filename=CommitteeConsolidateReport.xls");
                    return File(workStream, fileType);
                }
                catch (Exception ex)
                {
                    TempData["errMsg"] = "Something went to wrong please contact admin.";
                    return RedirectToAction("CommitteeConsolidateReport", "RCTReportMaster");
                }
            }
        }

        #region Requirement

        //NotNeeded For me
        public string GenerateSTEOfferLetter(int STEID)
        {
            try
            {
                var folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter"));
                string loginuser = User.Identity.Name;
                string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/RCTReportMaster/STEOfferLetter?STEID=" + STEID;
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
                var docName = "OfferLetter_" + STEID + ".pdf";
                string Path = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter/" + docName);
                doc.Save(@Path);
                // close pdf document
                doc.Close();
                return docName;
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return "";
            }
        }

        public ActionResult STEOfferLetter(int STEID, int? orderId)
        {
            try
            {
                STEViewModel model = new STEViewModel();
                model = RQS.getSTEOfferletterDetails(STEID, orderId);
                if (model.DesignationId == 1)
                    return PartialView("_STETraineeOfferLetter", model);
                else
                    return View(model);

            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }

        public ActionResult CONOfferLetter(int CONID)
        {
            try
            {
                ConsultantAppointmentModel model = new ConsultantAppointmentModel();
                model = RQS.getCONOfferletterDetails(CONID);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }
        //OfferLetter
        public static ByteEmailAttachmentModel GenerateOfferLetter(int appid, int orderid, bool show_f = false)
        {
            try
            {
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.HttpContext.Current.Request.ApplicationPath + "/RCTReportMaster/STEOfferLetter?STEID=" + appid + "&orderId=" + orderid;
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
                converter.Options.MaxPageLoadTime = 120;
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                converter.Options.MarginTop = 15;
                converter.Options.MarginBottom = 15;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                var docName = "OfferLetter_" + appid + ".pdf";
                var fileName = Guid.NewGuid().ToString() + "_OfferLetter_" + appid + ".pdf";
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                model.displayName = "OfferLetter_" + appid + ".pdf";
                model.actualName = Guid.NewGuid().ToString() + "_OfferLetter_" + appid + ".pdf";
                model.dataByte = doc.Save();
                if (!show_f)
                    model.dataByte.UploadByteFile("RCTOfferLetter", model.actualName);
                doc.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ByteEmailAttachmentModel GenerateConsultantOfferLetter(int appid, int orderid, bool show_f = false)
        {
            try
            {
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.HttpContext.Current.Request.ApplicationPath + "/RCTReportMaster/CONOfferLetter?CONID=" + appid;
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
                converter.Options.MaxPageLoadTime = 120;
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                var docName = "OfferLetter_" + appid + ".pdf";
                var fileName = Guid.NewGuid().ToString() + "_OfferLetter_" + appid + ".pdf";
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                model.displayName = "OfferLetter_" + appid + ".pdf";
                model.actualName = Guid.NewGuid().ToString() + "_OfferLetter_" + appid + ".pdf";
                model.dataByte = doc.Save();
                if (!show_f)
                    model.dataByte.UploadByteFile("RCTOfferLetter", model.actualName);
                doc.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //

        public ActionResult ShowOrderDocument(int appid, string apptype, string offercategory, int? orderid = null)
        {
            try
            {
                string loginuser = User.Identity.Name;
                byte[] pdf = new byte[] { };
                if (orderid == 0)
                    orderid = null;
                if (offercategory == "OfferLetter")
                {
                    if (apptype == "STE")
                        pdf = GenerateOfferLetter(appid, orderid ?? 0, true).dataByte;
                    else
                        pdf = GenerateConsultantOfferLetter(appid, orderid ?? 0, true).dataByte;
                }
                else if (offercategory == "OfficeOrder")
                {
                    pdf = RCTOfficeOrderPrint(appid, apptype, orderid, true).dataByte;
                }
                else if (offercategory == "Order")
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblOrder.Where(x => x.OrderId == orderid).FirstOrDefault();
                        if (query.OrderType == 2)
                            pdf = GenerateEnhancementOrder(orderid ?? 0, true).dataByte;
                        else if (query.OrderType == 3)
                            pdf = GenerateExtensionOrder(orderid ?? 0, true).dataByte;
                        else if (query.OrderType == 4)
                            pdf = GenerateAmendmentOrder(orderid ?? 0, true).dataByte;
                        else if (query.OrderType == 5 || query.OrderType == 6)
                            pdf = HRAOrderPrint(orderid ?? 0, true).dataByte;
                        else if (query.OrderType == 4)
                            pdf = GenerateAmendmentOrder(orderid ?? 0, true).dataByte;
                        else if (query.OrderType == 9)
                        {
                            //var queryodr = context.tblOrderDetail.Where(x => x.OrderId == orderid).FirstOrDefault();

                            byte[] pdf1 = PrintRelievingOrder(orderid ?? 0, true).dataByte;
                            //byte[] pdf2 = new byte[] { };
                            //if (query.Status == "Completed" && queryodr.RelievingMode != 3)
                            //    pdf2 = PrintServiceCertificate(orderid ?? 0, true).dataByte;
                            //byte[] combinedBuff = new byte[] { };
                            //if (query.Status == "Open")
                            //    combinedBuff = pdf1;
                            //else
                            //{
                            //    if(pdf2.Length > 0)
                            //        combinedBuff = pdf1.Concat(pdf2).ToArray();
                            //    else
                            //        combinedBuff = pdf1;
                            //}
                            pdf = pdf1;
                        }
                        else
                            pdf = GenerateEnhancementOrder(orderid ?? 0, true).dataByte;
                    }
                }

                Response.AddHeader("Content-Disposition", "inline; filename=" + offercategory + ".pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return new EmptyResult();
            }
        }

        public ActionResult EnhancementOrder(int OrderID)
        {
            try
            {
                GenerateOrdersModel model = new GenerateOrdersModel();
                if (OrderID > 0)
                {
                    model = RQS.GenearateOrders(OrderID);
                }
                return PartialView("_EnhancementOrder", model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                GenerateOrdersModel model = new GenerateOrdersModel();
                return PartialView("_EnhancementOrder", model);
            }
        }

        public ByteEmailAttachmentModel GenerateEnhancementOrder(int OrderID, bool show_f = false)
        {
            try
            {
                var folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter"));
                //string loginuser = User.Identity.Name;
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.HttpContext.Current.Request.ApplicationPath + "/RCTReportMaster/EnhancementOrder?OrderID=" + OrderID;
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
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.MarginTop = 10;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                model.displayName = "EnhancementOrder.pdf";
                model.actualName = Guid.NewGuid().ToString() + "_EnhancementOrder.pdf";
                model.dataByte = doc.Save();
                if (!show_f)
                    model.dataByte.UploadByteFile("RCTOfferLetter", model.actualName);

                doc.Close();
                return model;
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                return model;
            }
        }

        public ActionResult AmendmentOrder(int OrderID)
        {
            try
            {
                GenerateOrdersModel model = new GenerateOrdersModel();
                if (OrderID > 0)
                {
                    model = RQS.GenearateOrders(OrderID);
                }
                return PartialView("_AmendmentOrder", model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                GenerateOrdersModel model = new GenerateOrdersModel();
                return PartialView("_AmendmentOrder", model);
            }
        }

        public ByteEmailAttachmentModel GenerateAmendmentOrder(int OrderID, bool show_f = false)
        {
            try
            {
                var folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter"));
                //string loginuser = User.Identity.Name;
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.HttpContext.Current.Request.ApplicationPath + "/RCTReportMaster/AmendmentOrder?OrderID=" + OrderID;
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
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.MarginTop = 10;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                model.displayName = "AmendmentOrder.pdf";
                model.actualName = Guid.NewGuid().ToString() + "_AmendmentOrder.pdf";
                model.dataByte = doc.Save();
                if (!show_f)
                    model.dataByte.UploadByteFile("RCTOfferLetter", model.actualName);
                doc.Close();
                return model;
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                return model;
            }
        }

        public ActionResult ExtensionOrder(int OrderID)
        {
            try
            {
                GenerateOrdersModel model = new GenerateOrdersModel();
                if (OrderID > 0)
                {
                    model = RQS.GenearateOrders(OrderID);
                }
                return PartialView("_ExtensionOrder", model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                GenerateOrdersModel model = new GenerateOrdersModel();
                return PartialView("_ExtensionOrder", model);
            }
        }

        public ByteEmailAttachmentModel GenerateExtensionOrder(int OrderID, bool show_f = false)
        {
            try
            {
                var folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter"));
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.HttpContext.Current.Request.ApplicationPath + "/RCTReportMaster/ExtensionOrder?OrderID=" + OrderID;
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
                converter.Options.MaxPageLoadTime = 240;
                converter.Options.MarginTop = 10;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                var guid = Guid.NewGuid().ToString();
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                model.displayName = "ExtensionOrder.pdf";
                model.actualName = guid + "_ExtensionOrder.pdf";
                model.dataByte = doc.Save();
                if (!show_f)
                    model.dataByte.UploadByteFile("RCTOfferLetter", model.actualName);
                doc.Close();
                return model;
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                return model;
            }
        }

        public ActionResult ServiceCertificate(int orderid)
        {
            try
            {
                RelieveCertificateModel model = new RelieveCertificateModel();
                if (orderid > 0)
                {
                    model = RQS.getServiceCertificateDatails(orderid);
                    var res = RQS.UpdateRelifOrderGeneration(orderid, "Service Certificate");
                }
                return PartialView("ServiceCertificate", model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                RelieveCertificateModel model = new RelieveCertificateModel();
                return PartialView("ServiceCertificate", model);
            }
        }

        public ByteEmailAttachmentModel PrintServiceCertificate(int orderid, bool show_f = false)
        {
            try
            {
                string loginuser = System.Web.HttpContext.Current.User.Identity.Name;
                var folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter");
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.HttpContext.Current.Request.ApplicationPath + "/RCTReportMaster/ServiceCertificate?orderid=" + orderid;
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
                converter.Options.MaxPageLoadTime = 500;
                converter.Options.MarginTop = 10;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                //Response.AddHeader("Content-Disposition", "inline; filename=" + docName);
                //return File(pdf, "application/pdf");
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                model.displayName = "ServiceCertificate.pdf";
                model.actualName = Guid.NewGuid().ToString() + "_ServiceCertificate.pdf";
                model.dataByte = doc.Save();
                if (!show_f)
                    model.dataByte.UploadByteFile("RCTOfferLetter", model.actualName);
                doc.Close();
                return model;
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                return model;
            }
        }

        public ActionResult ServiceCertificatePrint(int orderid)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/RCTReportMaster/ServiceCertificate?orderid=" + orderid;
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
                converter.Options.MaxPageLoadTime = 500;
                converter.Options.MarginTop = 10;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=ServiceCertificate.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return new EmptyResult();
            }
        }

        public ActionResult FinalSettlement(int orderid)
        {
            try
            {
                RelieveCertificateModel model = new RelieveCertificateModel();
                if (orderid > 0)
                {
                    model = RQS.getFinalSettelment(orderid);
                    var res = RQS.UpdateRelifOrderGeneration(orderid, "Final Settlement");
                }
                return PartialView(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                RelieveCertificateModel model = new RelieveCertificateModel();
                return PartialView(model);
            }
        }

        public ActionResult FinalSettlementPrint(int orderid)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/RCTReportMaster/FinalSettlement?orderid=" + orderid;
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
                converter.Options.MaxPageLoadTime = 500;
                converter.Options.MarginTop = 10;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=FinalSettlement.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return new EmptyResult();
            }
        }

        public ActionResult RelievingOrder(int orderid)
        {
            try
            {
                GenerateOrdersModel model = new GenerateOrdersModel();
                if (orderid > 0)
                {
                    model = RQS.GenearateOrders(orderid);
                    var res = RQS.UpdateRelifOrderGeneration(orderid, "Relieve Order");
                }
                return PartialView(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                GenerateOrdersModel model = new GenerateOrdersModel();
                return PartialView(model);
            }
        }

        public ByteEmailAttachmentModel PrintRelievingOrder(int orderid, bool show_f = false)
        {
            try
            {
                string loginuser = System.Web.HttpContext.Current.User.Identity.Name;
                var folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter");
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.HttpContext.Current.Request.ApplicationPath + "/RCTReportMaster/RelievingOrder?orderid=" + orderid;
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
                converter.Options.MaxPageLoadTime = 500;
                converter.Options.MarginTop = 10;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                model.displayName = "RelieveOrder.pdf";
                model.actualName = Guid.NewGuid().ToString() + "_Relieveorder.pdf";
                model.dataByte = doc.Save();
                if (!show_f)
                    model.dataByte.UploadByteFile("RCTOfferLetter", model.actualName);
                doc.Close();
                return model;
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                return model;
            }
        }

        public ActionResult RelievingOrderPrint(int orderid, bool isSave = false)
        {
            try
            {
                string loginuser = User.Identity.Name;
                var folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter");
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/RCTReportMaster/RelievingOrder?orderid=" + orderid;
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
                converter.Options.MaxPageLoadTime = 500;
                converter.Options.MarginTop = 10;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=RelieveOrder.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return new EmptyResult();
            }
        }

        public ActionResult RCTOfficeOrder(int appid, string apptype, int? orderid)
        {
            try
            {
                RCTOfficeOrderModel model = new RCTOfficeOrderModel();
                model = RQS.getOfficeOrderDetails(appid, apptype, orderid);
                if (apptype == "CON")
                    return PartialView("RCTCONOfficeOrder", model);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                RCTOfficeOrderModel model = new RCTOfficeOrderModel();
                return View(model);
            }
        }

        public ByteEmailAttachmentModel RCTOfficeOrderPrint(int appid = 0, string apptype = "", int? orderid = null, bool show_f = false)
        {
            try
            {
                var folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter"));
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.HttpContext.Current.Request.ApplicationPath + "/RCTReportMaster/RCTOfficeOrder?appid=" + appid + "&apptype=" + apptype + "&orderid=" + orderid;
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
                converter.Options.MaxPageLoadTime = 500;
                converter.Options.MarginTop = 10;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                model.displayName = "Officeorder.pdf";
                model.actualName = Guid.NewGuid().ToString() + "_officeorder.pdf"; ;
                model.dataByte = doc.Save();
                if (!show_f)
                    model.dataByte.UploadByteFile("RCTOfferLetter", model.actualName);
                doc.Close();
                return model;
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                return model;
            }
        }

        public ActionResult HRAOrder(int orderid)
        {
            try
            {
                GenerateOrdersModel model = new GenerateOrdersModel();
                model = RQS.GenearateOrders(orderid);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                RCTOfficeOrderModel model = new RCTOfficeOrderModel();
                return View(model);
            }
        }

        public ByteEmailAttachmentModel HRAOrderPrint(int orderid, bool show_f = false)
        {
            try
            {
                var folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/OfferLetter"));
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.HttpContext.Current.Request.ApplicationPath + "/RCTReportMaster/HRAOrder?orderid=" + orderid;
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
                converter.Options.MaxPageLoadTime = 500;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                model.displayName = "HRAorder.pdf";
                converter.Options.MarginTop = 10;
                model.actualName = Guid.NewGuid().ToString() + "_hraorder.pdf";
                model.dataByte = doc.Save();
                if (!show_f)
                    model.dataByte.UploadByteFile("RCTOfferLetter", model.actualName);
                doc.Close();
                return model;
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ByteEmailAttachmentModel model = new ByteEmailAttachmentModel();
                return model;
            }
        }

        public string GenerateSalaryStructure(int OSGID, int OdrId = 0)
        {
            try
            {
                var folder = "";
                folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/SalaryStructure");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/SalaryStructure"));
                //foreach (string sFile in System.IO.Directory.GetFiles(folder))
                //    System.IO.File.Delete(sFile);

                //string loginuser = User.Identity.Name;
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.HttpContext.Current.Request.ApplicationPath + "/RCTReportMaster/SalaryStructure?OSGID=" + OSGID + "&OdrId=" + OdrId;
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
                converter.Options.MaxPageLoadTime = 200;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                var docName = "SalaryStructure_" + OSGID + "_" + OdrId + ".pdf";
                string Path = folder + '\\' + docName;
                doc.Save(@Path);
                // close pdf document
                doc.Close();
                return Path;
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return "";
            }
        }

        public ActionResult SalaryStructure(int OSGID, int OdrId = 0)
        {
            try
            {
                STEViewModel model = new STEViewModel();
                if (OSGID > 0)
                    model = RQS.getOSGSalaryStructureDetails(OSGID, OdrId);
                return PartialView("_RCTSalaryStructure", model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                STEViewModel model = new STEViewModel();
                return PartialView("_RCTSalaryStructure", model);
            }
        }

        public ActionResult PrintStaffDetails(string EmployeeNo)
        {
            try
            {
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.HttpContext.Current.Request.ApplicationPath + "/RCTReportMaster/StaffDetails?EmployeeNo=" + EmployeeNo;
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
                converter.Options.MaxPageLoadTime = 500;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=Staffdetail_" + EmployeeNo);
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return new EmptyResult();
            }
        }

        public ActionResult StaffDetails(string EmployeeNo)
        {
            try
            {
                RCTPopupModel model = new RCTPopupModel();
                model = RQS.getStaffWorkingDatails(EmployeeNo);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                RCTPopupModel model = new RCTPopupModel();
                return View(model);
            }
        }

        public ActionResult SalaryStructureBill(int OSGID, int OrderId = 0)
        {
            try
            {
                string loginuser = User.Identity.Name;
                string url = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.HttpContext.Current.Request.ApplicationPath + "/RCTReportMaster/SalaryStructure?OSGID=" + OSGID + "&OdrId=" + OrderId;
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
                converter.Options.MaxPageLoadTime = 300;
                SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                byte[] pdf = doc.Save();
                doc.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=SalaryStructureBill.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }

        public ActionResult ChangeofProjectOrder(int OrderID)
        {
            try
            {
                GenerateOrdersModel model = new GenerateOrdersModel();
                if (OrderID > 0)
                {
                    model = RQS.GenearateOrders(OrderID);
                }
                return PartialView("_ChangeofProjectOrder", model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                GenerateOrdersModel model = new GenerateOrdersModel();
                return PartialView("_ChangeofProjectOrder", model);
            }
        }


        #endregion

        #region payroll
        public DataTable ExportAdhocPayrollDetail(int payrollId)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = Common.getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    //command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EmployeeId]) AS [S. No.],[EmployeeId], [CandidateName],[DesignationCode],[ProjectId],[ProjectNumber], [DepartmentCode], [DepartmentName],[CommitmentNumber],[AppointmentStartDate], [AppointmentEndDate], [RelieveDate], [DOB], [Gender], [Basic],[CurrentBasic],[HRA],[CurrentHRA],[Medical],[CurrentMedical],[TotalDaysInMonth] as [Month Total Days],[TotalDays],[MedicalInclusive_f],[Arrears], [OthersPay],[Spl_Allowance],[Transport_Allowance],[Loss_Of_Pay],[OthersDeduction],[Recovery],[Medical_Recovery],[Contribution_to_PF],[PF_Revision],[ESIC_Revision],[TaxExempted], [PTExempted], [Round_off], [Professional_tax], [BankName], [AccountNo], [IFSC],[HRA_Arrears],[HRA_Recovery],(Spl_Allowance + Transport_Allowance + PF_Revision + ESIC_Revision + Round_off + Arrears + OthersPay + HRA_Arrears) - (Contribution_to_PF + Recovery + OthersDeduction + Professional_tax + Loss_Of_Pay + Medical_Recovery + HRA_Recovery)+(CurrentBasic + CurrentHRA)as NetSalary from tblRCTPayrollDetail where RCTPayrollId = " + payrollId;
                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY p.EmployeeId) AS [S. No.],p.EmployeeId, p.CandidateName,d.Designation,p.ProjectNumber, p.DepartmentName,p.AppointmentStartDate, p.AppointmentEndDate, p.RelieveDate,p.CommitmentNumber,p.TotalDaysInMonth as [Month Total Days],p.TotalDays as [Employee Working Days], p.DOB, p.Gender, p.Basic,p.CurrentBasic,p.HRA,p.CurrentHRA,p.Medical,p.CurrentMedical,p.MedicalInclusive_f,p.Arrears, p.OthersPay,p.Spl_Allowance,p.Transport_Allowance,ISNULL(p.LOPDays,0) as [Loss_Of_Pay days],p.Loss_Of_Pay,p.OthersDeduction,p.Recovery,p.Medical_Recovery,p.TaxExempted, p.PTExempted, p.Round_off, p.Professional_tax, p.BankName, p.AccountNo, p.IFSC,p.HRA_Arrears,p.HRA_Recovery,(p.Spl_Allowance + p.Transport_Allowance + p.PF_Revision + p.ESIC_Revision + p.Round_off + p.Arrears + p.OthersPay + p.HRA_Arrears) - (p.Contribution_to_PF + p.Recovery + p.OthersDeduction + p.Professional_tax + p.Loss_Of_Pay +  p.Medical_Recovery +  p.HRA_Recovery) + ( p.CurrentBasic +  p.CurrentHRA) as NetSalary from tblRCTPayrollDetail as p join tblRCTDesignation as d on p.DesignationId = d.DesignationId where p.RCTPayrollId = " + payrollId;
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet ExportOSGPayrollDetail(int payrollId)
        {
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            try
            {

                using (var connection = Common.getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [Employee ID]) AS [S. No.], [Employee ID], [Salutation], [Employee Name], [Designation], [Employment Mode], [Deapartment], [Project Number], [Commitment Number], [Process Type], [DOJ],[ToDate] as DOE, [Recommended Salary],[FromDate],[ToDate],[RelieveDate], [Gross Salary], [PF-BASIC],[DaysInMonth], [Total Working Days], [Employee Working Days], [Gross Salary For the Month],[PF - BASIC For the Month],[Additional Pay],[LOP],[Recovery], [Insurance], [PF Eligibility], [ESIC Eligiblity], [Calculated Pay], [BankName], [AccountNo], [Branch], [IFSC] from vw_RCTOSGPayroll  where  RelieveDate is null and RCTPayrollId =" + payrollId;
                    //command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [Employee ID]) AS [S.No.],* from [vw_RCTOSGPayroll] where RelieveDate is null and RCTPayrollId =" + payrollId;
                    var adapter = new SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dt1 = dataset.Tables[0].Copy();
                    dt1.TableName = "Active_Employee";

                    var command1 = new SqlCommand();
                    command1.CommandText = "select ROW_NUMBER() OVER(ORDER BY [Employee ID]) AS [S. No.], [Employee ID], [Salutation], [Employee Name], [Designation], [Employment Mode], [Deapartment], [Project Number], [Commitment Number], [Process Type], [DOJ],[ToDate] as DOE, [Recommended Salary],[FromDate],[ToDate],[RelieveDate], [Gross Salary], [PF-BASIC],[DaysInMonth], [Total Working Days], [LOP], [Employee Working Days], [Gross Salary For the Month],[PF - BASIC For the Month], [Additional Pay],[LOP],[Recovery], [Insurance], [PF Eligibility], [ESIC Eligiblity], [Calculated Pay], [BankName], [AccountNo], [Branch], [IFSC] from vw_RCTOSGPayroll  where  RelieveDate is not null and RCTPayrollId =" + payrollId;
                    //command1.CommandText = "select ROW_NUMBER() OVER(ORDER BY [Employee ID]) AS [S.No.],* from [vw_RCTOSGPayroll] where RelieveDate is not null and RCTPayrollId =" + payrollId;
                    var adapter1 = new SqlDataAdapter(command1);
                    var dataset1 = new DataSet();
                    if (dataset1.Tables.Count > 0)
                    {
                        adapter1.Fill(dataset1);
                        dt2 = dataset1.Tables[0].Copy();
                    }
                    dt2.TableName = "Relieved_Employee";
                    ds.Tables.Add(dt1);
                    ds.Tables.Add(dt2);
                }
                return ds;
            }
            catch (Exception ex)
            {
                return ds;
            }
        }

        public FileStreamResult DownloadExportAdhocPayrollDetails(int PayrollId)
        {
            try
            {
                string excelname = string.Empty;
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var datadt = ExportAdhocPayrollDetail(PayrollId);
                        var query = (from pah in context.tblRCTPayroll
                                     where pah.RCTPayrollId == PayrollId
                                     select pah).FirstOrDefault();
                        excelname = "PAYROLL DATA FOR THE MONTH OF" + "-" + query.SalaryMonth;
                        var ws = wb.Worksheets.Add(query.SalaryMonth);
                        ws.Cell(1, 8).Value = "PAYROLL DATA FOR THE MONTH OF" + " " + query.SalaryMonth;
                        ws.Range("H1:L1").Row(1).Merge();
                        //ws.Cell(2, 1).Value = "S. No.";
                        //ws.Cell(2, 2).Value = "Employee ID";
                        //ws.Cell(2, 3).Value = "ProjectId";
                        //ws.Cell(2, 4).Value = "CurrentBasic";
                        //ws.Cell(2, 5).Value = "Current HRA";
                        //ws.Cell(2, 6).Value = "Total Days";
                        //ws.Cell(2, 7).Value = "Current Medical";
                        //ws.Cell(2, 8).Value = "Medical Inclusive_f";
                        //ws.Cell(2, 9).Value = "Appointment End Date";
                        //ws.Cell(2, 10).Value = "Relieve Date";
                        //ws.Cell(2, 11).Value = "Appointment Start Date";
                        //ws.Cell(2, 12).Value = "Candidate Name";
                        //ws.Cell(2, 13).Value = "DOB";
                        //ws.Cell(2, 14).Value = "Gender";
                        //ws.Cell(2, 15).Value = "Basic";
                        //ws.Cell(2, 16).Value = "HRA";
                        //ws.Cell(2, 17).Value = "Medical";
                        //ws.Cell(2, 18).Value = "Commitment Number";
                        //ws.Cell(2, 19).Value = "Project Number";
                        //ws.Cell(2, 20).Value = "Department Code";
                        //ws.Cell(2, 21).Value = "Department Name";
                        //ws.Cell(2, 22).Value = "Designation Code";
                        //ws.Cell(2, 23).Value = "Tax Exempted";
                        //ws.Cell(2, 24).Value = "PT Exempted";
                        //ws.Cell(2, 25).Value = "Spl_Allowance";
                        //ws.Cell(2, 26).Value = "Transport_Allowance";
                        //ws.Cell(2, 27).Value = "PF_Revision";
                        //ws.Cell(2, 28).Value = "ESIC_Revision";
                        //ws.Cell(2, 29).Value = "Round_off";
                        //ws.Cell(2, 30).Value = "OthersPay";
                        //ws.Cell(2, 31).Value = "Contribution_to_PF";
                        //ws.Cell(2, 32).Value = "Recovery";
                        //ws.Cell(2, 33).Value = "Others Deduction";
                        //ws.Cell(2, 34).Value = "Professional_tax";
                        //ws.Cell(2, 35).Value = "Loss Of Pay";
                        //ws.Cell(2, 36).Value = "Medical Recovery";
                        //ws.Cell(2, 37).Value = "Bank Name";
                        //ws.Cell(2, 38).Value = "Account No";
                        //ws.Cell(2, 39).Value = "Branch";
                        //ws.Cell(2, 40).Value = "IFSC";
                        //ws.Cell(2, 41).Value = "Month Total Days";
                        var FromtitleRange = ws.Range("H1:L1");
                        FromtitleRange.Style.Font.Bold = true;
                        FromtitleRange.Style.Font.FontSize = 14;
                        FromtitleRange.Style.Font.FontName = "Arial Unicode MS";
                        var FromRange = ws.Range("A2:AO2");
                        FromRange.Style.Font.Bold = true;
                        FromRange.Style.Font.FontSize = 11;
                        FromRange.SetAutoFilter();
                        int Firstrow = 2;
                        //int sno = 1;
                        if (datadt.Rows.Count > 0)
                        {
                            ws.Cell(Firstrow, 1).InsertTable(datadt);
                        }
                        //foreach (DataRow row in datadt.Rows)
                        //{

                        //    ws.Cell(Firstrow, 1).Value = sno;
                        //    ws.Cell(Firstrow, 2).Value = row["EmployeeId"].ToString();
                        //    ws.Cell(Firstrow, 3).Value = row["ProjectId"].ToString();
                        //    ws.Cell(Firstrow, 4).Value = row["CurrentBasic"].ToString();
                        //    ws.Cell(Firstrow, 5).Value = row["CurrentHRA"].ToString();
                        //    ws.Cell(Firstrow, 6).Value = row["TotalDays"].ToString();
                        //    ws.Cell(Firstrow, 7).Value = row["CurrentMedical"].ToString();
                        //    ws.Cell(Firstrow, 8).Value = row["MedicalInclusive_f"].ToString();
                        //    ws.Cell(Firstrow, 9).Value = row["AppointmentEndDate"].ToString();
                        //    ws.Cell(Firstrow, 10).Value = row["RelieveDate"].ToString();
                        //    ws.Cell(Firstrow, 11).Value = row["AppointmentStartDate"].ToString();
                        //    ws.Cell(Firstrow, 12).Value = row["CandidateName"].ToString();
                        //    ws.Cell(Firstrow, 13).Value = row["DOB"].ToString();
                        //    ws.Cell(Firstrow, 14).Value = row["Gender"].ToString();
                        //    ws.Cell(Firstrow, 15).Value = row["Basic"].ToString();
                        //    ws.Cell(Firstrow, 16).Value = row["HRA"].ToString();
                        //    ws.Cell(Firstrow, 17).Value = row["Medical"].ToString();
                        //    ws.Cell(Firstrow, 18).Value = row["CommitmentNumber"].ToString();
                        //    ws.Cell(Firstrow, 19).Value = row["ProjectNumber"].ToString();
                        //    ws.Cell(Firstrow, 20).Value = row["DepartmentCode"].ToString();
                        //    ws.Cell(Firstrow, 21).Value = row["DepartmentName"].ToString();
                        //    ws.Cell(Firstrow, 22).Value = row["DesignationCode"].ToString();
                        //    ws.Cell(Firstrow, 23).Value = row["TaxExempted"].ToString();
                        //    ws.Cell(Firstrow, 24).Value = row["PTExempted"].ToString();
                        //    ws.Cell(Firstrow, 25).Value = row["Spl_Allowance"].ToString();
                        //    ws.Cell(Firstrow, 26).Value = row["Transport_Allowance"].ToString();
                        //    ws.Cell(Firstrow, 27).Value = row["PF_Revision"].ToString();
                        //    ws.Cell(Firstrow, 28).Value = row["ESIC_Revision"].ToString();
                        //    ws.Cell(Firstrow, 29).Value = row["Round_off"].ToString();
                        //    ws.Cell(Firstrow, 30).Value = row["OthersPay"].ToString();
                        //    ws.Cell(Firstrow, 31).Value = row["Contribution_to_PF"].ToString();
                        //    ws.Cell(Firstrow, 32).Value = row["Recovery"].ToString();
                        //    ws.Cell(Firstrow, 33).Value = row["OthersDeduction"].ToString();
                        //    ws.Cell(Firstrow, 34).Value = row["Professional_tax"].ToString();
                        //    ws.Cell(Firstrow, 35).Value = row["Loss_Of_Pay"].ToString();
                        //    ws.Cell(Firstrow, 36).Value = row["Medical_Recovery"].ToString();
                        //    ws.Cell(Firstrow, 37).Value = row["BankName"].ToString();
                        //    ws.Cell(Firstrow, 38).Value = row["AccountNo"].ToString();
                        //    ws.Cell(Firstrow, 39).Value = row["Branch"].ToString();
                        //    ws.Cell(Firstrow, 40).Value = row["IFSC"].ToString();
                        //    ws.Cell(Firstrow, 41).Value = row["Month Total Days"].ToString();


                        //    ws.Range("K3" + Firstrow).Style.DateFormat.Format = "dd-MMMM-yyyy";
                        //    ws.Range("A" + Firstrow + ":AO" + Firstrow).Style.Font.FontSize = 11;
                        //    ws.Range("A" + Firstrow + ":AO" + Firstrow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        //    ws.Range("A" + Firstrow + ":AO" + Firstrow).Style.Border.InsideBorder = XLBorderStyleValues.Dotted;
                        //    ws.Range("A" + Firstrow + ":AO" + Firstrow).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        //    ws.Range("A" + Firstrow + ":AO" + Firstrow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //    ws.Range("A" + Firstrow + ":AO" + Firstrow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        //    ws.Range("A" + Firstrow + ":AO" + Firstrow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        //    //ws.Columns("A", "BJ").AdjustToContents();
                        //    Firstrow++;
                        //    sno++;
                        //}
                        //ws.Range("K3" + Firstrow).Style.DateFormat.Format = "dd-MMMM-yyyy";

                        wb.SaveAs(workStream);
                        workStream.Position = 0;

                    }
                }
                string fileType = Common.GetMimeType("xls");
                Response.AddHeader("Content-Disposition", "filename=" + excelname + ".xls");
                return File(workStream, fileType);
                //return new FileStreamResult(workStream, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ExportAdhocPayrollDetailMail(int PayrollId)
        {
            try
            {
                string excelname = string.Empty;
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (var context = new IOASDBEntities())
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var datadt = ExportAdhocPayrollDetail(PayrollId);
                        var query = (from pah in context.tblRCTPayroll
                                     where pah.RCTPayrollId == PayrollId
                                     select pah).FirstOrDefault();
                        excelname = "PAYROLL DATA FOR THE MONTH OF" + query.SalaryMonth;
                        var ws = wb.Worksheets.Add(query.SalaryMonth);
                        ws.Cell(1, 8).Value = "PAYROLL DATA FOR THE MONTH OF" + " " + query.SalaryMonth;
                        ws.Range("H1:L1").Row(1).Merge();
                        var FromtitleRange = ws.Range("H1:L1");
                        FromtitleRange.Style.Font.Bold = true;
                        FromtitleRange.Style.Font.FontSize = 14;
                        FromtitleRange.Style.Font.FontName = "Arial Unicode MS";
                        var FromRange = ws.Range("A2:AO2");
                        FromRange.Style.Font.Bold = true;
                        FromRange.Style.Font.FontSize = 11;
                        FromRange.SetAutoFilter();
                        int Firstrow = 2;
                        if (datadt.Rows.Count > 0)
                            ws.Cell(Firstrow, 1).InsertTable(datadt);
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        byte[] bytes = workStream.ToArray();

                        return bytes;
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FileStreamResult DownloadExportOSGPayrollDetail(int PayrollId)
        {
            string excelname = string.Empty;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from pah in context.tblRCTPayroll
                                 where pah.RCTPayrollId == PayrollId
                                 select pah).FirstOrDefault();
                    excelname = "PAYROLL DATA OSG FOR THE MONTH OF" + "-" + query.SalaryMonth;
                    DataSet dsTrasaction = ExportOSGPayrollDetail(PayrollId);
                    //return toSpreadSheets(dsTrasaction);
                    MemoryStream workStream = new MemoryStream();

                    byte[] byteInfo = workStream.ToArray();
                    workStream.Write(byteInfo, 0, byteInfo.Length);


                    if (dsTrasaction != null)
                    {
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            foreach (DataTable dt in dsTrasaction.Tables)
                            {
                                //Add DataTable as Worksheet.
                                wb.Worksheets.Add(dt);
                            }
                            //wb.Worksheets.Add(dtReport, "Customers");
                            wb.SaveAs(workStream);
                            workStream.Position = 0;

                        }
                    }
                    string fileType = Common.GetMimeType("xls");
                    Response.AddHeader("Content-Disposition", "filename=" + excelname + ".xls");
                    return File(workStream, fileType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] DownloadExportOSGPayrollDetailMail(int PayrollId)
        {
            try
            {
                DataSet dsTrasaction = ExportOSGPayrollDetail(PayrollId);
                MemoryStream workStream = new MemoryStream();

                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                if (dsTrasaction != null)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        foreach (DataTable dt in dsTrasaction.Tables)
                        {
                            //Add DataTable as Worksheet.
                            wb.Worksheets.Add(dt);
                        }
                        //wb.Worksheets.Add(dtReport, "Customers");
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                        byteInfo = workStream.ToArray();
                    }
                }
                return byteInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet PayrollProcessingData(int Payrollid, int userId)
        {
            DataSet ds = new DataSet();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from pah in context.tblRCTPayroll
                                 where pah.RCTPayrollId == Payrollid && pah.AppointmentType == "Adhoc" && pah.Status == "Init"
                                 select pah).FirstOrDefault();
                    var salaryCompleted = context.tblSalaryPaymentHead.Any(m => m.PaymentMonthYear == query.SalaryMonth && m.TypeOfPayBill == query.SalaryType && m.Status == "Approval Pending");
                    if (salaryCompleted)
                        return ds;
                    if (query != null)
                    {
                        using (SqlConnection conn = Common.getConnection())
                        {
                            SqlCommand sqlComm = new SqlCommand("SPRCTRequestToProcessSalary", conn);
                            sqlComm.Parameters.AddWithValue("@PayrollId", Payrollid);
                            sqlComm.Parameters.AddWithValue("@userId", userId);
                            sqlComm.CommandType = CommandType.StoredProcedure;
                            sqlComm.CommandTimeout = 1000;
                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = sqlComm;
                            da.Fill(ds);
                            return ds;
                        }
                    }
                    else
                        return ds;
                }
            }
            catch (Exception ex)
            {
                return ds;
            }
        }

        public FileStreamResult DownloadStartProcessingPayroll(int Payrollid)//No need
        {
            string excelname = string.Empty;
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                //var filedata=ExportAdhocStartProcessPayrollDetail(PayrollData.Item2, Payrollid);
                // return RedirectToAction("ExportAdhocStartProcessPayrollDetail", "RCTReportMaster", new { @dset = PayrollData.Item2, @payrollid= Payrollid });
                using (var context = new IOASDBEntities())
                {
                    var query = (from pah in context.tblRCTPayroll
                                 where pah.RCTPayrollId == Payrollid
                                 select pah).FirstOrDefault();
                    string salarytype = string.Empty;
                    if (query.SalaryType == 1)
                        salarytype = "Main";
                    else if (query.SalaryType == 0)
                        salarytype = "Pensioner";
                    else
                        salarytype = "Supplementary";
                    excelname = "PAYROLL DATA Process " + salarytype + " Adhoc THE MONTH OF" + "-" + query.SalaryMonth;
                    DataSet dset = PayrollProcessingData(Payrollid, userid);
                    dt1 = dset.Tables[0].Copy();
                    if (dset.Tables.Count > 1)
                    {
                        dt2 = dset.Tables[1].Copy();
                    }
                    MemoryStream workStream = new MemoryStream();
                    byte[] byteInfo = workStream.ToArray();
                    workStream.Write(byteInfo, 0, byteInfo.Length);
                    if (dset != null)
                    {
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            if (dt1.Rows.Count > 0)
                            {
                                wb.Worksheets.Add(dt1, "New");
                            }
                            if (dt2.Rows.Count > 0)
                            {
                                wb.Worksheets.Add(dt2, "Different");
                            }
                            //wb.Worksheets.Add(dtReport, "Customers");
                            wb.SaveAs(workStream);
                            workStream.Position = 0;
                            byteInfo = workStream.ToArray();
                            int mailstatus = RCTEmailContentService.SendMailStartProcessingPayrollAttachment(Payrollid, byteInfo);
                        }
                    }
                    string fileType = Common.GetMimeType("xls");
                    Response.AddHeader("Content-Disposition", "filename=" + excelname + ".xls");
                    return File(workStream, fileType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet ExportOSGPayrollCalculationDetail(int payrollId)
        {
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            //DataTable dt2 = new DataTable();
            try
            {

                using (var connection = Common.getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    //command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [Employee ID]) AS [S. No.], [Employee ID],[Employee Name], [Designation], [Employment Mode],[DOJ],[DOE],[FromDate],[ToDate],[RelieveDate],[PF Eligibility],[ESIC Eligiblity],[Recommended Salary],[Gross Salary For the Month],[Gross Salary For the Month]as Gross,[PF-BASIC],[DaysInMonth],[Employee Working Days],[Gross Salary],[EmployeePF_CM] as [PF- BASIC For the Month],[Recovery],[Additional Pay] as [Other Payments],[EmployerPF] as [PF (13% of Basic)],[EmployeeESIC_CM] as [ESIC  (3.25% of Gross)],[LWF],[InsuranceAmount_CM],[Total],[CTC_CM],[ServiceCharges],[InvoiceAmt],[GST],[Amount]from vw_RCTOSGPayroll  where  RCTPayrollId =" + payrollId;
                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [Employee ID]) AS [S. No.], [Employee ID],[Employee Name], [Designation], [Employment Mode],[FromDate],[ToDate],[RelieveDate],[PF Eligibility],[ESIC Eligiblity],[Recommended Salary],[PF-BASIC],[DaysInMonth],[Employee Working Days],[Gross Salary For the Month] as GrossSalary,[PF - BASIC For the Month],[Recovery],[Additional Pay] as [Other Payments],[EmployerPF_CM] as [PF (13% of Basic)],[EmployerESIC_CM] as [ESIC  (3.25% of Gross)],[LWF],[InsuranceAmount_CM],[Total],[CTC_CM],[ServiceCharges],[InvoiceAmount],[GST],[Amount] from vw_RCTOSGPayroll where  RCTPayrollId =" + payrollId;
                    var adapter = new SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dt1 = dataset.Tables[0].Copy();
                    dt1.TableName = "OS Calculation Report";
                    ds.Tables.Add(dt1);
                }
                return ds;
            }
            catch (Exception ex)
            {
                return ds;
            }
        }

        public FileStreamResult DownloadExportOSGPayrollCalculationDetail(int PayrollId)
        {
            string excelname = string.Empty;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from pah in context.tblRCTPayroll
                                 where pah.RCTPayrollId == PayrollId
                                 select pah).FirstOrDefault();
                    excelname = "OS Calculation Report";
                    DataSet dsTrasaction = ExportOSGPayrollCalculationDetail(PayrollId);
                    //return toSpreadSheets(dsTrasaction);
                    MemoryStream workStream = new MemoryStream();

                    byte[] byteInfo = workStream.ToArray();
                    workStream.Write(byteInfo, 0, byteInfo.Length);


                    if (dsTrasaction != null)
                    {
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            foreach (DataTable dt in dsTrasaction.Tables)
                            {
                                //Add DataTable as Worksheet.
                                wb.Worksheets.Add(dt);
                            }
                            //wb.Worksheets.Add(dtReport, "Customers");
                            wb.SaveAs(workStream);
                            workStream.Position = 0;

                        }
                    }
                    string fileType = Common.GetMimeType("xls");
                    Response.AddHeader("Content-Disposition", "filename=" + excelname + ".xls");
                    return File(workStream, fileType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FileStreamResult DownloadOSGStartProcessingPayroll(int Payrollid)//No need
        {
            string excelname = string.Empty;
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                using (var context = new IOASDBEntities())
                {
                    var query = (from pah in context.tblRCTPayroll
                                 where pah.RCTPayrollId == Payrollid
                                 select pah).FirstOrDefault();
                    excelname = "PAYROLL DATA Process Main OSG THE MONTH OF" + "-" + query.SalaryMonth;
                    DataSet dset = new DataSet();
                    using (SqlConnection conn = Common.getConnection())
                    {
                        SqlCommand sqlComm = new SqlCommand("SPRCTRequestToProcessSalary", conn);
                        sqlComm.Parameters.AddWithValue("@PayrollId", Payrollid);
                        sqlComm.Parameters.AddWithValue("@userId", userid);
                        sqlComm.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = sqlComm;
                        da.Fill(dset);
                    }
                    dt1 = dset.Tables[0].Copy();
                    if (dset.Tables.Count > 1)
                        dt2 = dset.Tables[1].Copy();

                    MemoryStream workStream = new MemoryStream();

                    byte[] byteInfo = workStream.ToArray();
                    workStream.Write(byteInfo, 0, byteInfo.Length);

                    if (dset != null)
                    {
                        using (XLWorkbook wb = new XLWorkbook())
                        {

                            if (dt1.Rows.Count > 0)
                                wb.Worksheets.Add(dt1, "New");
                            if (dt2.Rows.Count > 0)
                                wb.Worksheets.Add(dt2, "Different");
                            //wb.Worksheets.Add(dtReport, "Customers");
                            wb.SaveAs(workStream);
                            workStream.Position = 0;
                            byteInfo = workStream.ToArray();
                            int mailstatus = RCTEmailContentService.SendMailForPayrollOSGAccounts(Payrollid, byteInfo);
                        }
                    }
                    string fileType = Common.GetMimeType("xls");
                    Response.AddHeader("Content-Disposition", "filename=" + excelname + ".xls");
                    return File(workStream, fileType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ProjectTypeCount

        public DataTable ExportRCTAppointmentTypeDetail()
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = Common.getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from tblRCTProjectFinType";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileStreamResult RCTgetAppointmenttypeProject(RCTReportProjectModel model)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Recruitment");
                    bool result = RequirementService.ExecuteSPApplicationTypeProjectCount(model);
                    if (result == true)
                    {
                        var dataset = ExportRCTAppointmentTypeDetail();
                        if (dataset.Rows.Count > 0)
                            ws.Cell(1, 1).InsertTable(dataset);
                    }
                    wb.SaveAs(workStream);
                    workStream.Position = 0;
                    string fileType = Common.GetMimeType("xls");
                    Response.AddHeader("Content-Disposition", "filename=RecruitmentAppointmentTypeProject.xls");
                    return File(workStream, fileType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult AppointmentBaseProjectType()
        {
            return View();
        }
        #endregion

        #region Daily Report

        public ActionResult DailyReport()
        {
            CashBookModel model = new CashBookModel();
            return View(model);
        }

        public DataSet ExportSTEDailyReport(DateTime From, DateTime To)
        {
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            DataTable dt4 = new DataTable();
            DataTable dt5 = new DataTable();
            DataTable dt6 = new DataTable();
            DataTable dt7 = new DataTable();
            DataTable dt8 = new DataTable();
            DataTable dt9 = new DataTable();
            DataTable dt10 = new DataTable();
            DataTable dt11 = new DataTable();
            try
            {
                using (var connection = Common.getConnection())
                {
                    var dataset1 = new DataSet();
                    var dataset2 = new DataSet();
                    var dataset3 = new DataSet();
                    var dataset4 = new DataSet();
                    var dataset5 = new DataSet();
                    var dataset6 = new DataSet();
                    var dataset7 = new DataSet();
                    var dataset8 = new DataSet();
                    var dataset9 = new DataSet();
                    var dataset10 = new DataSet();
                    var dataset11 = new DataSet();
                    To = To.AddDays(+1);
                    string strFrom = string.Format("{0:yyyy-MM-dd}", From);
                    string strTo = string.Format("{0:yyyy-MM-dd}", To);

                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.], * from [dbo].[vw_RCTSTENewJoineeReport] where VERIFICATION_APPROVED_DATE between '" + strFrom + "' and '" + strTo + "'";
                    var adapter1 = new SqlDataAdapter(command);
                    adapter1.Fill(dataset1);
                    dt1 = dataset1.Tables[0].Copy();
                    dt1.TableName = "NEW JOINEE";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],[EMPLOYEE ID],[NAME],[DESIGNATION],[APPOINTMENT START DATE],[END DATE],[SALARY],[HRA],[PROJECT NO.],[DEPARTMENT],[APPOINTMENT TYPE],[ARREARS TO BE PROCESSED W.E.F],[Requested by],[Request Reference],[Request Reference Number],[REQUEST RECEIVED DATE],[INITIATED_BY],[INITIATED_DATE],[APPROVED_BY],[APPROVED_DATE],[COMMITMENT BOOKED BY],[COMMITMENT BOOKED DATE],[COMMITEE APPROVED DATE],[VERFIFICATION_INITIATED_BY],[VERFIFICATION_INITIATED_DATE],[VERFIFICATION_APPROVED_BY],[VERFIFICATION_APPROVED_DATE],[MS/PHD COURSE] from [dbo].[vw_RCTSTEExtensionEnhancementReport] where OrderType = 3 and APPROVED_DATE between '" + strFrom + "' and '" + strTo + "'";
                    var adapter2 = new SqlDataAdapter(command);
                    adapter2 = new SqlDataAdapter(command);
                    adapter2.Fill(dataset2);
                    dt2 = dataset2.Tables[0].Copy();
                    dt2.TableName = "EXTENSION";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],[EMPLOYEE ID],[NAME],[DESIGNATION],[APPOINTMENT START DATE],[END DATE],[SALARY],[HRA],[PROJECT NO.],[DEPARTMENT],[APPOINTMENT TYPE],[ARREARS TO BE PROCESSED W.E.F],[ENHANCEMENT TYPE],[Requested by],[Request Reference],[Request Reference Number],[REQUEST RECEIVED DATE],[INITIATED_BY],[INITIATED_DATE],[APPROVED_BY],[APPROVED_DATE],[COMMITMENT BOOKED BY],[COMMITMENT BOOKED DATE],[COMMITEE APPROVED DATE],[VERFIFICATION_INITIATED_BY],[VERFIFICATION_INITIATED_DATE],[VERFIFICATION_APPROVED_BY],[VERFIFICATION_APPROVED_DATE],[MS/PHD COURSE]from [dbo].[vw_RCTSTEExtensionEnhancementReport] where OrderType = 2 and VERFIFICATION_APPROVED_DATE between '" + strFrom + "' and '" + strTo + "'";
                    var adapter3 = new SqlDataAdapter(command);
                    adapter3 = new SqlDataAdapter(command);
                    adapter3.Fill(dataset3);
                    dt3 = dataset3.Tables[0].Copy();
                    dt3.TableName = "ENHANCEMENT";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],* from [dbo].[vw_RCTSTELOPReport] where APPROVED_DATE between '" + strFrom + "' and '" + strTo + "'";
                    var adapter4 = new SqlDataAdapter(command);
                    adapter4 = new SqlDataAdapter(command);
                    adapter4.Fill(dataset4);
                    dt4 = dataset4.Tables[0].Copy();
                    dt4.TableName = "LOSS OF PAY";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEENO]) AS [S. No.],EMPLOYEENO,EmployeeName [EMPLOYEE NAME],ProjectNumber [PROJECT NO],DESIGNATION ,OtherType [CATEGORY],Head [CATEGORY TYPE],Amount [AMOUNT (Rs.)],[INITIATED_BY],[INITIATED_DATE],[APPROVED_BY],[APPROVED_DATE],[COMMITMENT_BOOKED/WITHDRAWAL_BY],[COMMITMENT_BOOKED/WITHDRAWAL_DATE],[Remarks] from [dbo].[vw_RCTOTHPayDeductionReport] where EmployeeNo LIKE '%IC%' and Status = 'Completed' and INITIATED_DATE between '" + strFrom + "' and '" + strTo + "'";
                    var adapter5 = new SqlDataAdapter(command);
                    adapter5 = new SqlDataAdapter(command);
                    adapter5.Fill(dataset5);
                    dt5 = dataset5.Tables[0].Copy();
                    dt5.TableName = "OTHER PAYMENT & DEDUCTIONS";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],* from [dbo].[vw_RCTSTERelievingReport] where APPROVED_DATE between '" + strFrom + "' and '" + strTo + "'";
                    var adapter6 = new SqlDataAdapter(command);
                    adapter6 = new SqlDataAdapter(command);
                    adapter6.Fill(dataset6);
                    dt6 = dataset6.Tables[0].Copy();
                    dt6.TableName = "RELIEVING";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],[EMPLOYEE ID],[NAME],[DESIGNATION],[APPOINTMENT START DATE],[END DATE],[SALARY],[HRA],[PROJECT NO.],[DEPARTMENT],[APPOINTMENT TYPE],[ARREARS TO BE PROCESSED W.E.F],[CHANGE OF PROJECT TYPE],[Requested by],[Request Reference],[Request Reference Number],[REQUEST RECEIVED DATE],[INITIATED BY],[INITIATED DATE],[APPROVED BY],[APPROVED DATE],[COMMITMENT BOOKED BY],[COMMITMENT BOOKING DATE],[VERIFICATION INITIATED BY],[VERIFICATION INITIATED DATE],[VERIFICATION APPROVED BY],[VERIFICATION APPROVED DATE],[MS/PHD COURSE] from [dbo].[vw_RCTSTEChagofprojectReport] where  [VERIFICATION APPROVED DATE] between '" + strFrom + "' and '" + strTo + "'";
                    var adapter7 = new SqlDataAdapter(command);
                    adapter7 = new SqlDataAdapter(command);
                    adapter7.Fill(dataset7);
                    dt7 = dataset7.Tables[0].Copy();
                    dt7.TableName = "CHANGE OF PROJECT";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],* from [dbo].[vw_RCTSTEHRA] where [COMMITMENT BOOKING DATE] between '" + strFrom + "' and '" + strTo + "'";
                    var adapter8 = new SqlDataAdapter(command);
                    adapter8 = new SqlDataAdapter(command);
                    adapter8.Fill(dataset8);
                    dt8 = dataset8.Tables[0].Copy();
                    dt8.TableName = "HRA";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],[EMPLOYEE ID],[NAME],[DESIGNATION],[APPOINTMENT START DATE],[END DATE],[SALARY],[HRA],[PROJECT NO.],[DEPARTMENT],[APPOINTMENT TYPE],[ARREARS TO BE PROCESSED W.E.F],[Requested by],[Request Reference],[Request Reference Number],[REQUEST RECEIVED DATE], [INITIATED BY],[INITIATED DATE],[APPROVED BY],[APPROVED DATE],[COMMITMENT WITHDRAWAL BY],[COMMITMENT WITHDRAWAL DATE],[MS/PHD COURSE] from [dbo].[vw_RCTSTEAmendmentReport] where  [APPROVED DATE] between '" + strFrom + "' and '" + strTo + "'";
                    var adapter9 = new SqlDataAdapter(command);
                    adapter9 = new SqlDataAdapter(command);
                    adapter9.Fill(dataset9);
                    dt9 = dataset9.Tables[0].Copy();
                    dt9.TableName = "AMENDMENT";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],* from [dbo].[vw_RCTSTEMaternity] where [REJOIN APPROVED DATE] between '" + strFrom + "' and '" + strTo + "'";
                    var adapter10 = new SqlDataAdapter(command);
                    adapter10 = new SqlDataAdapter(command);
                    adapter10.Fill(dataset10);
                    dt10 = dataset10.Tables[0].Copy();
                    dt10.TableName = "Maternity";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],* from [dbo].[vw_RCTSTEStoppayment] where [VERIFICATION APPROVED DATE] between '" + strFrom + "' and '" + strTo + "'";
                    var adapter11 = new SqlDataAdapter(command);
                    adapter11 = new SqlDataAdapter(command);
                    adapter11.Fill(dataset11);
                    dt11 = dataset11.Tables[0].Copy();
                    dt11.TableName = "Stop Payment";

                    ds.Tables.Add(dt1);
                    ds.Tables.Add(dt2);
                    ds.Tables.Add(dt3);
                    ds.Tables.Add(dt4);
                    ds.Tables.Add(dt5);
                    ds.Tables.Add(dt6);
                    ds.Tables.Add(dt7);
                    ds.Tables.Add(dt8);
                    ds.Tables.Add(dt9);
                    ds.Tables.Add(dt10);
                    ds.Tables.Add(dt11);
                }
                return ds;
            }
            catch (Exception ex)
            {
                return ds;
            }
        }

        public DataSet ExportOSGDailyReport(DateTime From, DateTime To)
        {
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            DataTable dt4 = new DataTable();
            DataTable dt5 = new DataTable();
            DataTable dt6 = new DataTable();
            DataTable dt7 = new DataTable();
            DataTable dt8 = new DataTable();
            DataTable dt9 = new DataTable();
            DataTable dt10 = new DataTable();
            DataTable dt11 = new DataTable();
            try
            {
                using (var connection = Common.getConnection())
                {
                    var dataset1 = new DataSet();
                    var dataset2 = new DataSet();
                    var dataset3 = new DataSet();
                    var dataset4 = new DataSet();
                    var dataset5 = new DataSet();
                    var dataset6 = new DataSet();
                    var dataset7 = new DataSet();
                    var dataset8 = new DataSet();
                    var dataset9 = new DataSet();
                    var dataset10 = new DataSet();
                    var dataset11 = new DataSet();
                    To = To.AddDays(+1);
                    string strFrom = string.Format("{0:yyyy-MM-dd}", From);
                    string strTo = string.Format("{0:yyyy-MM-dd}", To);

                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 1000;
                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.], * from [dbo].[vw_RCTOSGNewJoineeReport] where VERIFICATION_APPROVED_DATE between '" + strFrom + "' and '" + strTo + "'";
                    var adapter1 = new SqlDataAdapter(command);
                    adapter1.Fill(dataset1);
                    dt1 = dataset1.Tables[0].Copy();
                    dt1.TableName = "NEW JOINEE";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],[EMPLOYEE ID],[NAME],[EMPL TYPE],[DESIGNATION],[EXISTING FROM],[EXISTING TO],[PREVIOUS BASIC PAY],[PREVIOUS GROSS PAY],[PROJECT NO.],[PROJECT DETAILS],[COMMITMENT NO.],[FROM],[TO],[DATE OF INCREMENT],[YEARLY INCREMENT],[CURRENT BASIC SALARY],[CURRENT GROSS PAY],[PF Eligiblity],[ESIC Eligiblity],[Requested by],[Request Reference],[Request Reference Number],[REQUEST RECEIVED DATE],[INITIATED_BY],[INITIATED_DATE],[APPROVED_BY],[APPROVED_DATE],[COMMITMENT BOOKED BY],[COMMITMENT BOOKED DATE],[COMMITEE APPROVED DATE],[VERFIFICATION_INITIATED_BY],[VERFIFICATION_INITIATED_DATE],[VERFIFICATION_APPROVED_BY],[VERFIFICATION_APPROVED_DATE],[MS/PHD COURSE] FROM [dbo].[vw_RCTOSGExtensionEnhancementReport] where OrderType = 3 and APPROVED_DATE between '" + strFrom + "' and '" + strTo + "'";
                    var adapter2 = new SqlDataAdapter(command);
                    adapter2 = new SqlDataAdapter(command);
                    adapter2.Fill(dataset2);
                    dt2 = dataset2.Tables[0].Copy();
                    dt2.TableName = "EXTENSION";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],[EMPLOYEE ID],[NAME],[EMPL TYPE],[DESIGNATION],[EXISTING FROM],[EXISTING TO],[PREVIOUS BASIC PAY],[PREVIOUS GROSS PAY],[PROJECT NO.],[PROJECT DETAILS],[COMMITMENT NO.],[FROM],[TO],[DATE OF INCREMENT],[YEARLY INCREMENT],[CURRENT BASIC SALARY],[CURRENT GROSS PAY],[PF Eligiblity],[ESIC Eligiblity],[Requested by],[Request Reference],[Request Reference Number],[REQUEST RECEIVED DATE],[INITIATED_BY],[INITIATED_DATE],[APPROVED_BY],[APPROVED_DATE],[COMMITMENT BOOKED BY],[COMMITMENT BOOKED DATE],[COMMITEE APPROVED DATE],[VERFIFICATION_INITIATED_BY],[VERFIFICATION_INITIATED_DATE],[VERFIFICATION_APPROVED_BY],[VERFIFICATION_APPROVED_DATE],[MS/PHD COURSE] FROM [dbo].[vw_RCTOSGExtensionEnhancementReport] where OrderType = 2 and VERFIFICATION_APPROVED_DATE between '" + strFrom + "' and '" + strTo + "'";
                    var adapter3 = new SqlDataAdapter(command);
                    adapter3 = new SqlDataAdapter(command);
                    adapter3.Fill(dataset3);
                    dt3 = dataset3.Tables[0].Copy();
                    dt3.TableName = "ENHANCEMENT";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],* from [dbo].[vw_RCTOSGAttendanceReport] where LOG_TIME between '" + strFrom + "' and '" + strTo + "'";
                    var adapter4 = new SqlDataAdapter(command);
                    adapter4 = new SqlDataAdapter(command);
                    adapter4.Fill(dataset4);
                    dt4 = dataset4.Tables[0].Copy();
                    dt4.TableName = "ATTENDANCE";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEENO]) AS [S. No.],EMPLOYEENO,EmployeeName [EMPLOYEE NAME],ProjectNumber [PROJECT NO],DESIGNATION ,OtherType [CATEGORY],Head [CATEGORY TYPE],Amount [AMOUNT (Rs.)],[INITIATED_BY],[INITIATED_DATE],[APPROVED_BY],[APPROVED_DATE],[COMMITMENT_BOOKED/WITHDRAWAL_BY],[COMMITMENT_BOOKED/WITHDRAWAL_DATE],[Remarks] from [dbo].[vw_RCTOTHPayDeductionReport] where Status = 'Completed' and EMPLOYEENO LIKE '%VS%' and INITIATED_DATE between '" + strFrom + "' and '" + strTo + "'";
                    var adapter5 = new SqlDataAdapter(command);
                    adapter5 = new SqlDataAdapter(command);
                    adapter5.Fill(dataset5);
                    dt5 = dataset5.Tables[0].Copy();
                    dt5.TableName = "OTHER PAYMENT & DEDUCTIONS";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],* from [dbo].[vw_RCTOSGRelievingReport] where APPROVED_DATE between '" + strFrom + "' and '" + strTo + "'";
                    var adapter6 = new SqlDataAdapter(command);
                    adapter6 = new SqlDataAdapter(command);
                    adapter6.Fill(dataset6);
                    dt6 = dataset6.Tables[0].Copy();
                    dt6.TableName = "RELIEVING";


                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],[EMPLOYEE ID],[NAME],[EMPL TYPE],[DESIGNATION],[EXISTING FROM],[EXISTING TO],[PREVIOUS BASIC PAY],[PREVIOUS GROSS PAY],[PROJECT NO.],[PROJECT DETAILS],[COMMITMENT NO.],[FROM],[TO],[DATE OF INCREMENT],[YEARLY INCREMENT],[CURRENT BASIC SALARY],[CURRENT GROSS PAY],[PF Eligiblity],[ESIC Eligiblity],[Requested by],[Request Reference],[Request Reference Number],[REQUEST RECEIVED DATE],[INITIATED BY],[INITIATED DATE],[APPROVED_BY],[APPROVED_DATE],[COMMITMENT BOOKED BY],[COMMITMENT BOOKING DATE],[VERIFICATION INITIATED BY],[VERIFICATION INITIATED DATE],[VERIFICATION APPROVED BY],[VERIFICATION APPROVED DATE],[MS/PHD COURSE] FROM [dbo].[vw_RCTOSGChagofprojectReport] where [VERIFICATION APPROVED DATE] between '" + strFrom + "' and '" + strTo + "'";
                    var adapter7 = new SqlDataAdapter(command);
                    adapter7 = new SqlDataAdapter(command);
                    adapter7.Fill(dataset7);
                    dt7 = dataset7.Tables[0].Copy();
                    dt7.TableName = "CHANGE OF PROJECT";



                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],[EMPLOYEE ID],[NAME],[EMPL TYPE],[DESIGNATION],[EXISTING FROM],[EXISTING TO],[PREVIOUS BASIC PAY],[PREVIOUS GROSS PAY],[PROJECT NO.],[PROJECT DETAILS],[COMMITMENT NO.],[FROM],[TO],[DATE OF INCREMENT],[YEARLY INCREMENT],[CURRENT BASIC SALARY],[CURRENT GROSS PAY],[PF Eligiblity],[ESIC Eligiblity],[REQUEST RECEIVED DATE],[INITIATED BY],[INITIATED DATE],[APPROVED_BY],[APPROVED_DATE], [COMMITMENT WITHDRAWAL BY],[COMMITMENT WITHDRAWAL DATE] FROM [dbo].[vw_RCTOSGAmendmentReport] where [APPROVED_DATE] between '" + strFrom + "' and '" + strTo + "'";
                    var adapter8 = new SqlDataAdapter(command);
                    adapter8 = new SqlDataAdapter(command);
                    adapter8.Fill(dataset8);
                    dt8 = dataset8.Tables[0].Copy();
                    dt8.TableName = "AMENDMENT";


                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],* from [dbo].[vw_RCTOSGLOPReport] where [APPROVED_DATE] between '" + strFrom + "' and '" + strTo + "'";
                    var adapter9 = new SqlDataAdapter(command);
                    adapter9 = new SqlDataAdapter(command);
                    adapter9.Fill(dataset9);
                    dt9 = dataset9.Tables[0].Copy();
                    dt9.TableName = "LOSS OF PAY";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],* from [dbo].[vw_RCTOSGMaternity] where [REJOIN APPROVED DATE] between '" + strFrom + "' and '" + strTo + "'";
                    var adapter10 = new SqlDataAdapter(command);
                    adapter10 = new SqlDataAdapter(command);
                    adapter10.Fill(dataset10);
                    dt10 = dataset10.Tables[0].Copy();
                    dt10.TableName = "Maternity";

                    command.CommandText = "select ROW_NUMBER() OVER(ORDER BY [EMPLOYEE ID]) AS [S. No.],* from [dbo].[vw_RCTOSGStoppayment] where [VERIFICATION APPROVED DATE] between '" + strFrom + "' and '" + strTo + "'";
                    var adapter11 = new SqlDataAdapter(command);
                    adapter11 = new SqlDataAdapter(command);
                    adapter11.Fill(dataset11);
                    dt11 = dataset11.Tables[0].Copy();
                    dt11.TableName = "Stop Payment";


                    ds.Tables.Add(dt1);
                    ds.Tables.Add(dt2);
                    ds.Tables.Add(dt3);
                    ds.Tables.Add(dt4);
                    ds.Tables.Add(dt5);
                    ds.Tables.Add(dt6);
                    ds.Tables.Add(dt7);
                    ds.Tables.Add(dt8);
                    ds.Tables.Add(dt9);
                    ds.Tables.Add(dt10);
                    ds.Tables.Add(dt11);

                }
                return ds;
            }
            catch (Exception ex)
            {
                return ds;
            }
        }

        public FileStreamResult DownloadExportSTEDailyReport(DateTime From, DateTime To)
        {
            string excelname = string.Empty;
            try
            {
                using (var context = new IOASDBEntities())
                {

                    excelname = "STE DAILY REPORT " + string.Format("{0:dd-MM-yyyy}", From) + " TO " + string.Format("{0:dd-MM-yyyy}", To);
                    DataSet dsTrasaction = ExportSTEDailyReport(From, To);
                    //return toSpreadSheets(dsTrasaction);
                    MemoryStream workStream = new MemoryStream();

                    byte[] byteInfo = workStream.ToArray();
                    workStream.Write(byteInfo, 0, byteInfo.Length);


                    if (dsTrasaction != null)
                    {
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            foreach (DataTable dt in dsTrasaction.Tables)
                            {
                                wb.Worksheets.Add(dt);
                            }
                            wb.SaveAs(workStream);
                            workStream.Position = 0;
                        }
                    }
                    string fileType = Common.GetMimeType("xlsx");
                    Response.AddHeader("Content-Disposition", "filename=" + excelname + ".xlsx");
                    return File(workStream, fileType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FileStreamResult DownloadExportOSGDailyReport(DateTime From, DateTime To)
        {
            string excelname = string.Empty;
            try
            {
                using (var context = new IOASDBEntities())
                {

                    excelname = "OSG DAILY REPORT " + string.Format("{0:dd-MM-yyyy}", From) + " TO " + string.Format("{0:dd-MM-yyyy}", To);
                    DataSet dsTrasaction = ExportOSGDailyReport(From, To);
                    //return toSpreadSheets(dsTrasaction);
                    MemoryStream workStream = new MemoryStream();

                    byte[] byteInfo = workStream.ToArray();
                    workStream.Write(byteInfo, 0, byteInfo.Length);


                    if (dsTrasaction != null)
                    {
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            foreach (DataTable dt in dsTrasaction.Tables)
                            {
                                wb.Worksheets.Add(dt);
                            }
                            wb.SaveAs(workStream);
                            workStream.Position = 0;
                        }
                    }
                    string fileType = Common.GetMimeType("xlsx");
                    Response.AddHeader("Content-Disposition", "filename=" + excelname + ".xlsx");
                    return File(workStream, fileType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}