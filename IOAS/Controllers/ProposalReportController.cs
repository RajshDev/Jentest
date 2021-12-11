using CrystalDecisions.CrystalReports.Engine;
using IOAS.GenericServices;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CrystalDecisions.Shared;
using System.Globalization;
namespace IOAS.Controllers
{
    public class ProposalReportController : Controller
    {
        // GET: ProposalReport
        [Authorize]
        [HttpGet]
        public ActionResult Newproposalfunding(string message, string Errormsg)
        {
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
        [Authorize]
        public ActionResult Proposalfundingreport(ProposalReportViewModel model)
        {
            try
            {
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "NewPropsalssentfunding.rpt"));
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                List<ProposalReportViewModel> listmodel = new List<ProposalReportViewModel>();
                listmodel = ProposalReportService.GetFundingnewproposal(model);
                if (listmodel.Count > 0)
                {
                    rd.SetDataSource(listmodel);
                    var date = model.Month + "/" + model.Year;
                    rd.SetParameterValue("Monthdate", date);
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=NewProposalFunding.pdf");
                    return File(stream, "application/pdf");
                }
                else
                {
                   
                    return RedirectToAction("Newproposalfunding", new { message = "No records found for this type of search entry" });
                }
            }
            catch(Exception ex)
            {
                return RedirectToAction("Newproposalfunding", new { Errormsg = "Something went to wrong please contact admin." });
               
            }
        }
    }
}