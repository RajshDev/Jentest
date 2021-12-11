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
               string Password = Cryptography.Decrypt("gpH1sEtKvgPWgzeQjKAhSuhLbvIRePs/dJWoAEI8o9Q=", "LFPassW0rd");

                //AdhocSalaryProcess _as = new AdhocSalaryProcess();
                //_as.UpdateSalaryPayment(logged_in_user_id);
                //CoreAccountsService _cs = new CoreAccountsService();
                //_cs.getCLPBOAmodeldetails(313);
                //_cs.PDTCorrection();
                //_cs.backendDOPreceipt();
                //_cs.EditBillofEntry();
                //_cs.AdhocPayCommitmentBalanceUpdate(4360, true, false, 1, "REM");
                //_cs.disCommitmentRevoke();
                //_cs.BillBackEndEntry(8730,1);
                //_cs.BOABackendExpenditure();
                //_cs.ApproveProjectTransfer();
                //_cs.BackendReceipt();
                //ProcessClarifyService _ps = new ProcessClarifyService();
                //_ps.BillWFInitClarify(15646, 24);
                //AccountService.CloseCommitment(logged_in_user_id);
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
        //public ActionResult CLPCorrection()
        //{
        //    CoreAccountsService _cs = new CoreAccountsService();
        //    _cs.getCLPBOAmodeldetails(313);
        //    return new EmptyResult();
        //}
        //public ActionResult SalRevert()
        //{
        //    using (var context = new DataModel.IOASDBEntities())
        //    {
        //        CoreAccountsService _cs = new CoreAccountsService();
        //        var txList = (from c in context.tblAdhocSalaryCommitmentDetail
        //                      join det in context.tblCommitmentDetails on c.CommitmentDetailId equals det.ComitmentDetailId
        //                      join com in context.tblCommitment on det.CommitmentId equals com.CommitmentId
        //                      where c.PaymentHeadId == 74 && c.Status == "Active"
        //                      select new BillCommitmentDetailModel()
        //                      {
        //                          CommitmentDetailId = c.CommitmentDetailId,
        //                          PaymentAmount = c.Amount,
        //                          CommitmentId = com.CommitmentId,
        //                          ReversedAmount = c.Amount
        //                      }).ToList();
        //        var rsl = _cs.UpdateCommitmentBalance(txList, false, false, 1, 74, "SAL");

        //        return Json(rsl, JsonRequestBehavior.AllowGet);
        //    }
        //}
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

        //public ActionResult Example()
        //{
        //    PaymentProcessHtmlEmail e = new PaymentProcessHtmlEmail();
        //    e.From = "iruthaya89@gmail.com";
        //    e.To = "iruthaya1989@gmail.com";
        //    e.Subject = "test";
        //    e.Send();
        //    return View();
        //    //return new Postal.EmailViewResult(email);
        //}
        //public ActionResult SalContcommit()
        //{
        //    CoreAccountsService _cs = new CoreAccountsService();
        //    _cs.BackendContCommitment();
        //    return new EmptyResult();
        //}
        //public ActionResult CrtSalContcommit()
        //{
        //    CoreAccountsService _cs = new CoreAccountsService();
        //    _cs.CrtBackendContCommitment();
        //    return new EmptyResult();
        //}
        //public ActionResult PostSalContcommit()
        //{
        //    CoreAccountsService _cs = new CoreAccountsService();
        //    _cs.PostBackendContCommitment();
        //    return new EmptyResult();
        //}
        [HttpGet]
        public JsonResult Commitmenttest(int ProjectId,int HeadId,decimal Amt)
        {
            try
            {
               
                var data = Common.ValidateCommitment(ProjectId, HeadId, Amt);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
    }
}