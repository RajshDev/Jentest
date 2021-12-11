using CrystalDecisions.CrystalReports.Engine;
using IOAS.GenericServices;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CrystalDecisions.Shared;
using System.Configuration;
using IOAS.Filter;

namespace IOAS.Controllers
{
    [Authorized]
    public class ProjectReportController : Controller
    {
        string strServer = ConfigurationManager.AppSettings["ServerName"].ToString();
        string strDatabase = ConfigurationManager.AppSettings["DataBaseName"].ToString();
        string strUserID = ConfigurationManager.AppSettings["UserId"].ToString();
        string strPwd = ConfigurationManager.AppSettings["Password"].ToString();
        // GET: ProjectReport
        [Authorized]
        public ActionResult Projectreports(ProjectReportViewModel model)
        {
            try
            {
                ViewBag.projtype = Common.getprojecttype();
                ViewBag.month = Common.Getmonth();
                ViewBag.year = Common.Getyear();
                ViewBag.report = Common.Getreport();
                if (model.Reportname == "Department")
                {
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "SanctionProjectReport.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID,strPwd);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    List<ProjectReportViewModel> listmodel = new List<ProjectReportViewModel>();
                    listmodel = ProjectReportService.Getdeptwiseproject(model);
                    if (listmodel.Count > 0)
                    {

                        rd.SetDataSource(listmodel);
                        var date = model.Month + "/" + model.year;
                        rd.SetParameterValue("monthdate", date);

                        if (model.Projecttype == 1)
                        {
                            rd.SetParameterValue("Heading", "DEPARTMENT WISE SPONSORED PROJECT SANCTIONED DURING");
                        }
                        else
                        {
                            rd.SetParameterValue("Heading", "DEPARTMENT WISE CONSULTANCY PROJECT SANCTIONED DURING");
                        }

                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        Response.AddHeader("Content-Disposition", "inline; filename=Sanctionreport.pdf");
                        return File(stream, "application/pdf");
                    }
                    else
                    {
                        return RedirectToAction("Sanctionreport", new { message = "No records found for this type of search entry" });

                    }
                }
                else if (model.Reportname == "Faculty")
                {
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "Facultywisesanction.rpt"));
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    List<ProjectReportViewModel> listmodel = new List<ProjectReportViewModel>();
                    listmodel = ProjectReportService.Getfacultywiseproject(model);
                    if (listmodel.Count > 0)
                    {
                        rd.SetDataSource(listmodel);
                        var date = model.Month + "/" + model.year;
                        rd.SetParameterValue("month", date);

                        if (model.Projecttype == 1)
                        {
                            rd.SetParameterValue("Heading", "FACULTY WISE SPONSORED PROJECT SANCTIONED DURING");
                            rd.SetParameterValue("protype", "Sponsored");

                        }
                        else
                        {
                            rd.SetParameterValue("Heading", "FACULTY WISE CONSULTANCY PROJECT SANCTIONED DURING");
                            rd.SetParameterValue("protype", "consultancy");
                        }

                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        Response.AddHeader("Content-Disposition", "inline; filename=Sanctionreport.pdf");
                        return File(stream, "application/pdf");
                    }
                    else
                    {
                        return RedirectToAction("Sanctionreport", new { message = "No records found for this type of search entry" });
                    }
                }
                else if (model.Reportname == "Agency")
                {
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "Agencywisesanction.rpt"));
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    List<ProjectReportViewModel> listmodel = new List<ProjectReportViewModel>();
                    listmodel = ProjectReportService.Getagencywiseproject(model);
                    if (listmodel.Count > 0)
                    {
                        rd.SetDataSource(listmodel);
                        var date = model.Month + "/" + model.year;
                        rd.SetParameterValue("monthdate", date);

                        if (model.Projecttype == 1)
                        {
                            rd.SetParameterValue("Heading", "AGENCY WISE SPONSORED PROJECT SANCTIONED DURING");
                            rd.SetParameterValue("protype", "Sponsored");
                        }
                        else
                        {
                            rd.SetParameterValue("Heading", "AGENCY WISE CONSULTANCY PROJECT SANCTIONED DURING");
                            rd.SetParameterValue("protype", "Consultancy");
                        }

                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        Response.AddHeader("Content-Disposition", "inline; filename=Sanctionreport.pdf");
                        return File(stream, "application/pdf");
                    }
                    else
                    {
                        return RedirectToAction("Sanctionreport", new { message = "No records found for this type of search entry" });
                    }
                }
                return RedirectToAction("Sanctionreport", new { message = "No records found for this type of search entry" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Sanctionreport", new { Errormsg = "Something went to wrong please contact admin." });
            }
        }
        [Authorized]
        [HttpGet]
        public ActionResult Sanctionreport(string message, string Errormsg)
        {
            ViewBag.projtype = Common.getprojecttype();
            ViewBag.month = Common.Getmonth();
            ViewBag.year = Common.Getyear();
            ViewBag.report = Common.Getreport();
            if (message != null)
            {
                ViewBag.msg = message;
            }
            if (Errormsg != null)
            {
                ViewBag.error = Errormsg;
            }
            return View(); 
        }
        [Authorized]
        [HttpGet]
        public ActionResult ProposalApprovedReport(string message, string Errormsg)
        {
            ViewBag.projtype = Common.getprojecttype();
            if (message != null)
            {
                ViewBag.msg = message;
            }
            if (Errormsg != null)
            {
                ViewBag.error = Errormsg;
            }
            return View();
        }
        public ActionResult ProjectProposalApproved(ProjectProposalReport model, int reporttype)
        {
            try
            {
                if (reporttype == 1)
                {
                    ViewBag.projtype = Common.getprojecttype();
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "ProposalApprovedReport.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    //List<ProjectProposalReport> listmodel = new List<ProjectProposalReport>();
                    //var query = "select * from vw_ProjectProposal where Crtd_TS>=" + model.FromDate + " and Crtd_TS<=" + model.ToDate + "";
                    if (model.ToDate != null)
                    {
                        model.ToDate = model.ToDate.AddDays(1).AddTicks(-2);
                    }
                    rd.SetParameterValue("FromDate", model.FromDate);
                    rd.SetParameterValue("ToDate", model.ToDate);
                    rd.SetParameterValue("Displaytodate", model.ToDate.AddDays(-1));
                    rd.SetParameterValue("ProposalType", model.ProposalType);
                    if (model.ProposalType == 1)
                    {
                        rd.SetParameterValue("Heading", "SPONSERED PROJECT PROPOSALS");
                    }
                    else
                    {
                        rd.SetParameterValue("Heading", "CONSULTANCY PROJECT PROPOSALS");
                    }


                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=ProposalApprovedReport.pdf");
                    return File(stream, "application/pdf");

                }
                else
                {
                    ViewBag.projtype = Common.getprojecttype();
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "ProposalApprovedReportExcel.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    //List<ProjectProposalReport> listmodel = new List<ProjectProposalReport>();
                    //var query = "select * from vw_ProjectProposal where Crtd_TS>=" + model.FromDate + " and Crtd_TS<=" + model.ToDate + "";
                    if (model.ToDate != null)
                    {
                        model.ToDate = model.ToDate.AddDays(1).AddTicks(-2);
                    }
                    rd.SetParameterValue("FromDate", model.FromDate);
                    rd.SetParameterValue("ToDate", model.ToDate);
                    rd.SetParameterValue("Displaytodate", model.ToDate.AddDays(-1));
                    rd.SetParameterValue("ProposalType", model.ProposalType);
                    if (model.ProposalType == 1)
                    {
                        rd.SetParameterValue("Heading", "SPONSERED PROJECT PROPOSALS");
                    }
                    else
                    {
                        rd.SetParameterValue("Heading", "CONSULTANCY PROJECT PROPOSALS");
                    }


                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelWorkbook);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=ProposalApprovedReportExcel.xls");
                    return File(stream, "application/vnd.ms-excel");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ProposalApprovedReport", new { Errormsg = "Something went to wrong please contact admin." });
            }
        }
        public ActionResult ProposalReport(string message, string Errormsg)
        {
            try
            {
                ViewBag.projtype = Common.getprojecttype();
                ViewBag.month = Common.Getmonth();
                ViewBag.year = Common.Getyear();
                if (message != null)
                {
                    ViewBag.msg = message;
                }
                if (Errormsg != null)
                {
                    ViewBag.error = Errormsg;
                }
                return View();
            }
            catch(Exception ex)
            {
                return View();
            }
            
        }
        public ActionResult ProposalReportWise(ProjectReportViewModel model)
        {
            try
            {
                ViewBag.projtype = Common.getprojecttype();
                ViewBag.month = Common.Getmonth();
                ViewBag.year = Common.Getyear();
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "AgencyWiseProposal.rpt"));
                for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                    rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                //List<ProjectProposalReport> listmodel = new List<ProjectProposalReport>();
                //var query = "select * from vw_ProjectProposal where Crtd_TS>=" + model.FromDate + " and Crtd_TS<=" + model.ToDate + "";

                var date = model.Month+"/"+ model.year;
                rd.SetParameterValue("Monthyear",date);
                rd.SetParameterValue("ProjectType", model.Projecttype);
                if (model.Projecttype == 1)
                {
                    rd.SetParameterValue("Heading", "New Proposals sent for funding during Sponsored");
                }
                else
                {
                    rd.SetParameterValue("Heading", "New Proposals sent for funding during Consultancy");
                }
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                Response.AddHeader("Content-Disposition", "inline; filename=ProposalApprovedReport.pdf");
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ProposalReport", new { Errormsg = "Something went to wrong please contact admin." });
            }
        }
        public ActionResult ProjectEnchancementReport(string message, string Errormsg)
        {
            try
            {
                ViewBag.projtype = Common.getprojecttype();
                ViewBag.month = Common.Getmonth();
                ViewBag.year = Common.Getyear();
                if (message != null)
                {
                    ViewBag.msg = message;
                }
                if (Errormsg != null)
                {
                    ViewBag.error = Errormsg;
                }
                return View();
            }
            catch(Exception ex)
            {
                return View();
            }
        }
        public ActionResult AdditionalAmount(ProjectReportViewModel model)
        {
            try
            {
                ViewBag.projtype = Common.getprojecttype();
                ViewBag.month = Common.Getmonth();
                ViewBag.year = Common.Getyear();
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "AddtionalAmount.rpt"));
                for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                    rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                //List<ProjectProposalReport> listmodel = new List<ProjectProposalReport>();
                //var query = "select * from vw_ProjectProposal where Crtd_TS>=" + model.FromDate + " and Crtd_TS<=" + model.ToDate + "";

                var date = model.Month + "/" + model.year;
                rd.SetParameterValue("Monthyear", date);
                rd.SetParameterValue("Projecttype", model.Projecttype);
                if (model.Projecttype == 1)
                {
                    rd.SetParameterValue("Heading", "ADDITIONAL AMOUNT DURING SPONSORED PROJECT");
                }
                else
                {
                    rd.SetParameterValue("Heading", "ADDTIONAL AMOUNT DURING CONSULTANCY PROJECT");
                }
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                Response.AddHeader("Content-Disposition", "inline; filename=ProposalApprovedReport.pdf");
                return File(stream, "application/pdf");
            }
            catch(Exception ex)
            {
                return RedirectToAction("ProjectEnchancementReport", new { Errormsg = "Something went to wrong please contact admin." });
            }
        }
        public ActionResult SponsoredProject(string message, string Errormsg)
        {
            try
            {
                ViewBag.projtype = Common.getprojecttype();
                ViewBag.month = Common.Getmonth();
                ViewBag.year = Common.Getyear();
                if (message != null)
                {
                    ViewBag.msg = message;
                }
                if (Errormsg != null)
                {
                    ViewBag.error = Errormsg;
                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult SponsoredProjectExportExcel(ProjectReportViewModel model)
        {
            try
            {

                ViewBag.month = Common.Getmonth();
                ViewBag.year = Common.Getyear();
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "SponsoredProjects.rpt"));
                for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                    rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                //List<ProjectProposalReport> listmodel = new List<ProjectProposalReport>();
                //var query = "select * from vw_ProjectProposal where Crtd_TS>=" + model.FromDate + " and Crtd_TS<=" + model.ToDate + "";

                var date = model.Month + "/" + model.year;
                rd.SetParameterValue("Monthyear", date);
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelWorkbook);
                stream.Seek(0, SeekOrigin.Begin);
                Response.AddHeader("Content-Disposition", "inline; filename=SponsoredProjects.xls");
                return File(stream, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                return RedirectToAction("SponsoredProject", new { Errormsg = "Something went to wrong please contact admin." });
            }
        }
        public ActionResult SourceOfFunding(string message, string Errormsg)
        {
            try
            {
                ViewBag.projtype = Common.getprojecttype();
                ViewBag.month = Common.Getmonth();
                ViewBag.year = Common.Getyear();
                if (message != null)
                {
                    ViewBag.msg = message;
                }
                if (Errormsg != null)
                {
                    ViewBag.error = Errormsg;
                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public ActionResult SourceofFundingReport(ProjectReportViewModel model)
        {
            try
            {
                ViewBag.projtype = Common.getprojecttype();
                ViewBag.month = Common.Getmonth();
                ViewBag.year = Common.Getyear();
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "SourceFunding.rpt"));
                for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                    rd.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                var date = model.Month + "/" + model.year;
                rd.SetParameterValue("Monthyear", date);
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                Response.AddHeader("Content-Disposition", "inline; filename=SourceFunding.pdf");
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                return RedirectToAction("SourceOfFunding", new { Errormsg = "Something went to wrong please contact admin." });
            }
        }
    }
}