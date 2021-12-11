using CrystalDecisions.CrystalReports.Engine;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IOAS.GenericServices;
namespace IOAS.Controllers
{
    
    public class CrystalReportController : Controller
    {
        // GET: CrystalReport
        [Authorize]
        [HttpGet]
        public ActionResult ProjectProposal()
        {
            ViewBag.projecttype = Common.getprojecttype();
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult ProjectProposal(ProposalRepotViewModels model)
        {
            ViewBag.projecttype = Common.getprojecttype();
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "Proposalsponsored.rpt"));
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            ViewBag.report = CrystalReportService.Getproposal(model);
            rd.SetDataSource(ViewBag.report);
            rd.SetParameterValue("Fromdate", model.FromDate);
            rd.SetParameterValue("Todate", model.ToDate);
            if (model.ProjecttypeId == 1)
            {
                rd.SetParameterValue("heading", "SPONSORED PROJECT PROPOSALS");
            }
            else
            {
                rd.SetParameterValue("heading", "CONSULTANCY PROJECT PROPOSALS");
            }
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            Response.AddHeader("Content-Disposition", "inline; filename=Proposal_Report.pdf");
            return File(stream, "application/pdf");
           
            
        }

        [Authorize]
        public ActionResult ProposalReport(ProposalRepotViewModels model)
        {
            ViewBag.projecttype = Common.getprojecttype();
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "Proposalsponsored.rpt"));
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            ViewBag.report = CrystalReportService.Getproposal(model);
            rd.SetDataSource(ViewBag.report);
            rd.SetParameterValue("Fromdate", model.FromDate);
            rd.SetParameterValue("Todate", model.ToDate);
            if (model.ProjecttypeId == 1)
            {
                rd.SetParameterValue("heading", "SPONSORED PROJECT PROPOSALS");
            }
            else
            {
                rd.SetParameterValue("heading", "CONSULTANCY PROJECT PROPOSALS");
            }
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            Response.AddHeader("Content-Disposition", "inline; filename=Proposal_Report.pdf");
            return File(stream, "application/pdf");
        }
    }
}