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

namespace IOAS.Controllers
{
    public class TapalReportController : Controller
    {
        // GET: TapalReport
        [Authorize]
        [HttpGet]
        public ActionResult Tapalreport(string message,string Errormsg)
        {
            ViewBag.dept = AccountService.Getdepartment();
            ViewBag.user = Common.GetUserList();
            if (message != null)
            {
                ViewBag.msg = message;
            }
            if(Errormsg!=null)
            {
                ViewBag.error = Errormsg;
            }
            return View();
        }
        [Authorize]
        
        public ActionResult Tapalreportdetail(TapalReportViewModel model)
        {
            try
            {
                ReportDocument rd = new ReportDocument();
                string conn = "IOASDB";
                rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "TapalTransReport.rpt"));
                for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                    rd.DataSourceConnections[i].SetConnection(".", conn, "sa", "Welc0me");
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                List<TapalReportViewModel> listmodel = new List<TapalReportViewModel>();
               
                listmodel = ReportService.Gettapaltansaction(model);
                if (listmodel.Count > 0)
                {
                   
                    rd.SetDataSource(listmodel);
                    
                    rd.SetParameterValue("FRMdate", model.fromdate);
                    rd.SetParameterValue("TOdate", model.todate);
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=Tapaltransactionreport.pdf");
                    return File(stream, "application/pdf");
                }
                else
                {
                    return RedirectToAction("Tapalreport", new { message = "No records found for this type of search entry"  });

                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Tapalreport", new { Errormsg = "Something went to wrong please contact admin." });
            }
        }
    }
    }
