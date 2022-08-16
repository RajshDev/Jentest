using IOAS.Filter;
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
    public class HomeController : Controller
    {

        [Authorized]
        public ActionResult Index()
        {
            return View();
        }

        

        [Authorized]
        public ActionResult Dashboard()
        {
            try
            {
                string logged_in_user = User.Identity.Name;
                int logged_in_user_id = Common.GetUserid(logged_in_user);
                ViewBag.FirstName = Common.GetUserFirstName(logged_in_user);
                ViewBag.LoginTS = Common.GetLoginTS(logged_in_user_id);
                DashboardModel model = new DashboardModel();
                model.nofity = Common.GetNotification(logged_in_user_id);
                model.approveList = ProcessEngineService.GetPendingTransactionByUser(-1,logged_in_user_id);
                model.wfproposallist = Common.GetWFProposal(logged_in_user_id);
                return View(model);
            }
            catch (FileNotFoundException ex)
            {
                return View();
            }
        }
       
       
        [Authorized]
        public ActionResult ShowDocument(string file, string filepath)
        {
            try
            {
                var doc = file.Split(new char[] { '_' }, 2);
                string actName = string.Empty;
                actName = doc.Length == 2 ? doc[1] : file;
                string fileType = Common.GetMimeType(Path.GetExtension(file));
                byte[] fileData = file.DownloadFile(Common.GetDirectoryName(filepath));
                Response.AddHeader("Content-Disposition", "inline; filename=\"" + actName + "\"");
                return File(fileData, fileType);
            }
            catch (FileNotFoundException ex)
            {
                throw new HttpException(404, "File not found.");
            }
        }
    }
}