using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using IOAS.Infrastructure;
using IOAS.Models;
using IOAS.GenericServices;
using System.Web.Script.Serialization;
using IOAS.Filter;
using System.Data;
using IOAS.DataModel;

namespace IOAS.Controllers
{
    [Authorized]
    public class StaffPaymentController : Controller
    {
        CoreAccountsService coreAccountService = new CoreAccountsService();
        StaffPaymentService payment = new StaffPaymentService();
        AdhocSalaryProcess adhoc = new AdhocSalaryProcess();
        FinOp fo = new FinOp(System.DateTime.Now);
        DateTime Today = System.DateTime.Now;
        private static readonly Object lockObj = new Object();


        [Authorize]
        public ActionResult Salary(int PaymentHeadId = 0)
        {
            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
            int processGuideLineId = Process.ProcessGuidelineId;

            ViewBag.processGuideLineId = processGuideLineId;
            ViewBag.currentRefId = -1;
            ViewBag.months = fo.GetAllMonths();
            ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
            ViewBag.ModeOfPayment = Common.GetCodeControlList("PaymentMode");
            ViewBag.Department = adhoc.GetDepartments();
            ViewBag.EmployeeCategoryList = adhoc.GetEmployeeCategory("Direct Salary");
            ViewBag.TypeList = Common.GetCodeControlList("Salary Breakup Type");
            ViewBag.HeadList = new List<MasterlistviewModel>();

            PagedList<AdhocEmployeeModel> model = new PagedList<AdhocEmployeeModel>();
            var peymentHead = adhoc.GetSalayPaymentHead(PaymentHeadId);

            string Status = "open";


            if (peymentHead != null)
            {
                ViewBag.SelectedPaytype = peymentHead.TypeOfPayBill;
                ViewBag.SelectedPaymonth = peymentHead.PaymentMonthYear;
            }

            if (Status == null || Status.ToLower() == "open")
            {
                ViewBag.AllowSave = "";
                ViewBag.AllowEdit = "";
                ViewBag.Mode = "Edit";
            }
            else
            {
                ViewBag.AllowSave = "disabled";
                ViewBag.AllowEdit = "disabled";
                ViewBag.Mode = "View";
            }

            var OADetail = adhoc.GetAdhocOtherExpBreakUp(PaymentHeadId);
            ViewBag.TotalDistributionAmount = OADetail.TotalDistributionAmount;
            ViewBag.TotalHonororiumAmount = OADetail.TotalHonororiumAmount;
            ViewBag.TotalMandaysAmount = OADetail.TotalMandaysAmount;
            ViewBag.TotalFellowshipAmount = OADetail.TotalFellowshipAmount;
            return View("SalaryInitGrid", model);
        }

        [HttpPost]
        public ActionResult Salary(ValidateSalary model, FormCollection formCollection)
        {
            string msg = "";
            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);

            var btnSave = Request["btnSave"];
            var btnProceed = Request["btnProceed"];
            var btnBack = Request["btnBack"];
            var btnValidate = Request["btnValidate"];
            var Department = Request["departmentcode"];
            var PaymentMonthYear = Request["PaymentMonthYear"];
            var paymentHeadId = Request["PaymentHeadId"];
            var Status = Request["Status"];
            var typeOfPayBill = Request["TypeOfPayBill"];
            //var EmployeeId = Request["EmployeeId"];
            //var EmployeeName = Request["EmployeeName"];
            // var DepartmentCode = Request["DepartmentCode"];
            string empCat = String.IsNullOrEmpty(Request["EmployeeCategory"]) ? "0" : Request["EmployeeCategory"];
            int EmployeeCategory = Convert.ToInt32(empCat);

            int PaymentHeadId = (paymentHeadId != null && paymentHeadId != "") ? Convert.ToInt32(paymentHeadId) : 0;
            int TypeOfPayBill = (typeOfPayBill != null && typeOfPayBill != "") ? Convert.ToInt32(typeOfPayBill) : -1;

            ViewBag.PaymentHeadId = PaymentHeadId;
            ViewBag.months = fo.GetAllMonths();
            ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
            ViewBag.ModeOfPayment = Common.GetCodeControlList("PaymentMode");
            ViewBag.Department = adhoc.GetDepartments();
            ViewBag.EmployeeCategoryList = adhoc.GetEmployeeCategory("Direct Salary");
            ViewBag.TypeList = Common.GetCodeControlList("Salary Breakup Type");
            ViewBag.HeadList = new List<MasterlistviewModel>();
            ViewBag.SelectedPaytype = TypeOfPayBill;
            ViewBag.SelectedPaymonth = PaymentMonthYear;
            ViewBag.SelectedDepartment = Department;
            ViewBag.SelectedEmpCategory = EmployeeCategory;


            if (TypeOfPayBill < 0)
            {
                msg = "Please select Type of PayBill";
                ModelState.AddModelError("", msg);
                ViewBag.Errors = msg;
                return View("SalaryInitGrid");
            }
            else if (PaymentMonthYear == "" || PaymentMonthYear == null)
            {
                //msg = "Please select Payment Month Year";
                //ModelState.AddModelError("", msg);
                //ViewBag.Errors = msg;
                return View("SalaryInitGrid");
            }
            else if (fo.IsFutureMonthYear(PaymentMonthYear) == false)
            {
                msg = "Selected salary month should not be future month";
                ModelState.AddModelError("", msg);
                ViewBag.Errors = msg;
                return View("SalaryInitGrid");
            }
            if (btnValidate != null)
            {
                //var data = ValidateSalTemplate(model.template, TypeOfPayBill, PaymentMonthYear);
                //if (data.Item1 == "Valid")
                //{
                //    ViewBag.Message = "Validation has been done successfully.";
                //    ViewBag.uploadId = data.Item2;
                //}
                //else
                //    ViewBag.Errors = data.Item1;
                return View("SalaryInitGrid");
            }
            var paybill = adhoc.GetStatusFieldById("PayOfBill", TypeOfPayBill);

            var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
            int processGuideLineId = Process.ProcessGuidelineId;
            int refId = (PaymentHeadId > 0) ? PaymentHeadId : -1;

            ViewBag.processGuideLineId = processGuideLineId;
            ViewBag.currentRefId = refId;

            TempData["PaymentHeadId"] = PaymentHeadId;
            TempData["PaymentMonthYear"] = PaymentMonthYear;
            TempData["TypeOfPayBill"] = TypeOfPayBill;
            if (PaymentHeadId <= 0)
            {
                ViewBag.AllowProceed = "disabled";
            }
            else
            {
                ViewBag.AllowProceed = "";
            }
            if (Status == null || Status == "open")
            {
                ViewBag.AllowSave = "";
                ViewBag.AllowEdit = "";
            }
            else
            {
                ViewBag.AllowSave = "disabled";
                ViewBag.AllowEdit = "disabled";
            }

            if (btnBack != null)
            {
                return RedirectToAction("List");
            }
            else if (btnProceed != null)
            {
                return RedirectToAction("Commitment");
            }
            else
            {
                return View("SalaryInitGrid");
            }
        }

        public FileStreamResult ExportAdhocSalary(int PaymentHeadId, int TypeOfPayBill, string PaymentMonthYear)
        {
            try
            {
                DataSet ds = new DataSet();
                //var paybill = adhoc.GetStatusFieldById("PayOfBill", TypeOfPayBill);
                //if (paybill.ToLower() == "main")
                //{

                ds = adhoc.GetEmployeesSalaryList(PaymentMonthYear, PaymentHeadId, TypeOfPayBill);
                //}
                //else if (paybill.ToLower() == "supplementary")
                //{
                //    ds = adhoc.GetEmployeesSalaryList(PaymentMonthYear, PaymentHeadId);
                //}
                //else
                //{
                //    throw new Exception();
                //}
                return coreAccountService.toSpreadSheet(ds);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(this, ex);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Department"></param>
        /// <param name="EmployeeId"></param>
        /// <param name="EmployeeName"></param>
        /// <param name="EmployeeCategory"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SearchEmployeeSalary(SalaryPaymentHead model, string Department, string EmployeeId, string EmployeeName, int EmployeeCategory, int pageIndex, int pageSize)
        {
            var pagedList = new PagedList<AdhocEmployeeModel>();
            try
            {
                string msg = "";

                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                List<SalaryModel> SalaryList = new List<SalaryModel>();

                if (model.TypeOfPayBill < 0 || model.PaymentMonthYear == "" || model.PaymentMonthYear == null || fo.IsFutureMonthYear(model.PaymentMonthYear) == false)
                {
                    pagedList.Data = new List<AdhocEmployeeModel>();
                    return Json(pagedList, JsonRequestBehavior.AllowGet);
                }
                adhoc.setFilter(EmployeeId, EmployeeName, Department, EmployeeCategory);
                pagedList = adhoc.GetSalaryEmployees(model.PaymentMonthYear, model.PaymentHeadId, model.TypeOfPayBill, pageIndex, pageSize);


                pagedList.CurrentPage = pageIndex;
                pagedList.pageSize = pageSize;
                pagedList.visiblePages = 5;

                return Json(pagedList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pagedList.Data = new List<AdhocEmployeeModel>();
                return Json(pagedList, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult SearchSelectedEmployee(SalaryPaymentHead model, string Department, string EmployeeId, string EmployeeName, int EmployeeCategory, int page)
        //{
        //    string msg = "";
        //    int pageSize = 5;

        //    var user = User.Identity.Name;
        //    var userId = AdminService.getUserByName(user);
        //    List<AdhocEmployeeModel> employee = new List<AdhocEmployeeModel>();
        //    List<SalaryModel> SalaryList = new List<SalaryModel>();
        //    ViewBag.ModeOfPayment = payment.GetStatusFields("SalaryPaymentType");
        //    ViewBag.SelectedPaytype = model.TypeOfPayBill;
        //    ViewBag.SelectedPaymonth = model.PaymentMonthYear;
        //    ViewBag.SelectedDepartment = Department;
        //    ViewBag.SelectedEmpCategory = EmployeeCategory;
        //    var pagedList = new PagedList<AdhocEmployeeModel>();

        //    if (model.TypeOfPayBill <= 0)
        //    {
        //        msg = "Please select Type of PayBill";
        //        ModelState.AddModelError("", msg);
        //        ViewBag.Errors = msg;
        //        return View("StaffPartialList", pagedList);
        //    }
        //    else if (model.PaymentMonthYear == "" || model.PaymentMonthYear == null)
        //    {
        //        msg = "Please select Payment Month Year";
        //        ModelState.AddModelError("", msg);
        //        ViewBag.Errors = msg;
        //        return View("StaffPartialList", pagedList);
        //    }
        //    else if (fo.IsFutureMonthYear(model.PaymentMonthYear) == false)
        //    {
        //        msg = "Please selected Month Year can not be future month";
        //        ModelState.AddModelError("", msg);
        //        ViewBag.Errors = msg;
        //        return View("StaffPartialList", pagedList);
        //    }
        //    if (model.PaymentHeadId == 0)
        //    {
        //        model.PaymentHeadId = -1;
        //    }
        //    var paybill = adhoc.GetStatusFieldById("PayOfBill", model.TypeOfPayBill);
        //    if (model.PaymentMonthYear != null && model.PaymentMonthYear != "" && paybill.ToLower() == "main")
        //    {
        //        adhoc.setFilter(EmployeeId, EmployeeName, Department, EmployeeCategory);
        //        pagedList = adhoc.GetEmployeesByPaymentHead(model.PaymentHeadId, page, pageSize);
        //    }
        //    else if (model.PaymentMonthYear != null && paybill.ToLower() == "supplementary")
        //    {
        //        pagedList = adhoc.GetEmployeesByPaymentHead(model.PaymentHeadId, page, pageSize);
        //    }

        //    var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
        //    int processGuideLineId = Process.ProcessGuidelineId;
        //    int refId = (model.PaymentHeadId > 0) ? model.PaymentHeadId : -1;
        //    pagedList.CurrentPage = page;
        //    pagedList.pageSize = pageSize;
        //    pagedList.visiblePages = 5;

        //    return PartialView("StaffSelectedPartialList", pagedList);
        //}
        //public ActionResult AdhocSelectedEmployee(SalaryPaymentHead model, string Department, string EmployeeId, string EmployeeName, string ModeOfPaymentName, int EmployeeCategory, int pageIndex, int pageSize)
        //{
        //    var pagedList = new PagedList<AdhocEmployeeModel>();
        //    try
        //    {
        //        string msg = "";
        //        var user = User.Identity.Name;
        //        var userId = AdminService.getUserByName(user);
        //        List<AdhocEmployeeModel> employee = new List<AdhocEmployeeModel>();
        //        List<SalaryModel> SalaryList = new List<SalaryModel>();


        //        if (model.TypeOfPayBill <= 0 || model.PaymentMonthYear == "" || model.PaymentMonthYear == null || fo.IsFutureMonthYear(model.PaymentMonthYear) == false)
        //        {
        //            pagedList.Data = new List<AdhocEmployeeModel>();
        //            return Json(pagedList, JsonRequestBehavior.AllowGet);
        //        }

        //        if (model.PaymentHeadId == 0)
        //        {
        //            model.PaymentHeadId = -1;
        //        }
        //        // var paybill = adhoc.GetStatusFieldById("PayOfBill", model.TypeOfPayBill);
        //        adhoc.setFilter(EmployeeId, EmployeeName, Department, EmployeeCategory);
        //        //if (model.PaymentMonthYear != null && model.PaymentMonthYear != "" && paybill.ToLower() == "main")
        //        //{
        //        //    adhoc.setFilter(EmployeeId, EmployeeName, Department, EmployeeCategory);
        //        //    pagedList = adhoc.GetEmployeesByPaymentHead(model.PaymentHeadId, pageIndex, pageSize);
        //        //}
        //        //else if (model.PaymentMonthYear != null && paybill.ToLower() == "supplementary")
        //        //{
        //        pagedList = adhoc.GetEmployeesByPaymentHead(model.PaymentHeadId, ModeOfPaymentName, pageIndex, pageSize);
        //        //}

        //        var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
        //        int processGuideLineId = Process.ProcessGuidelineId;
        //        int refId = (model.PaymentHeadId > 0) ? model.PaymentHeadId : -1;
        //        pagedList.CurrentPage = pageIndex;
        //        pagedList.pageSize = pageSize;
        //        pagedList.visiblePages = 5;

        //        return Json(pagedList, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        pagedList.Data = new List<AdhocEmployeeModel>();
        //        return Json(pagedList, JsonRequestBehavior.AllowGet);
        //    }
        //}
        [Authorize]
        public ActionResult SalaryPayment()
        {
            int page = 1;
            int pageSize = 10;
            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            var PaymentHeadId = TempData["PaymentHeadId"];
            int paymentId = 0;
            if (PaymentHeadId != null)
            {
                paymentId = Convert.ToInt32(PaymentHeadId);
            }

            //var monthYear = fo.GetCurrentMonthYear();
            if (TempData["PaymentMonthYear"] == null)
            {
                return RedirectToAction("Salary");
            }

            var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
            var model = payment.GetEmployeeSalary(page, pageSize, paymentId);
            ViewBag.SelectedMonth = paymentMonthYear;
            ViewBag.months = fo.GetAllMonths();
            ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
            var SalaryPaymentHead = adhoc.GetSalayPaymentHead(paymentId);
            if (SalaryPaymentHead == null || SalaryPaymentHead.Status == null || SalaryPaymentHead.Status.ToLower() == "open")
            {
                ViewBag.AllowSubmit = "";
                ViewBag.AllowSave = "";
                ViewBag.AllowEdit = "";
            }
            else
            {
                ViewBag.AllowSubmit = "disabled";
                ViewBag.AllowSave = "disabled";
                ViewBag.AllowEdit = "disabled";
            }
            TempData["PaymentMonthYear"] = paymentMonthYear;
            TempData["PaymentHeadId"] = paymentId;
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SalaryPayment(int page, int pageSize, FormCollection formCollection)
        {
            var btnSave = Request["btnSave"];
            var btnSubmit = Request["btnSubmit"];
            var btnBack = Request["btnBack"];

            var PaymentHeadId = TempData["PaymentHeadId"];
            int paymentId = 0;
            if (PaymentHeadId != null)
            {
                paymentId = Convert.ToInt32(PaymentHeadId);
            }

            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
            var model = payment.GetEmployeeSalary(page, pageSize, paymentId);
            ViewBag.months = fo.GetAllMonths();
            ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
            ViewBag.SelectedMonth = paymentMonthYear;

            var SalaryPaymentHead = adhoc.GetSalayPaymentHead(paymentId);
            if (SalaryPaymentHead == null || SalaryPaymentHead.Status == null || SalaryPaymentHead.Status.ToLower() == "open")
            {
                ViewBag.AllowSubmit = "";
                ViewBag.AllowSave = "";
                ViewBag.AllowEdit = "";
            }
            else
            {
                ViewBag.AllowSubmit = "disabled";
                ViewBag.AllowSave = "disabled";
                ViewBag.AllowEdit = "disabled";
            }

            TempData["PaymentMonthYear"] = paymentMonthYear;
            TempData["PaymentHeadId"] = paymentId;

            if (btnBack != null)
            {
                return RedirectToAction("Transaction");
            }
            else if (btnSubmit != null)
            {

                SalaryPaymentHead payHead = new SalaryPaymentHead();

                string currentStatus = "open";
                string newStatus = "Approval Pending";
                var msg = adhoc.UpdateSalaryPayment(paymentId, currentStatus, newStatus, userId);
                if (msg == "")
                {
                    return View(model);
                }
                TempData["Message"] = msg;
                return RedirectToAction("List");
            }
            return View(model);
        }

        [Authorize]
        public ActionResult Commitment()
        {
            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            if (TempData["PaymentMonthYear"] == null)
            {
                return RedirectToAction("Salary");
            }
            var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
            var PaymentHeadId = (TempData["PaymentHeadId"] != null) ? Convert.ToInt32(TempData["PaymentHeadId"].ToString()) : -1;
            //var model = payment.GetProjectCommitment(PaymentHeadId,1,10,"","");
            ViewBag.months = fo.GetAllMonths();
            ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
            ProjectCommitmentModel model = new ProjectCommitmentModel();
            model.TotalAmount = payment.GetCommitmentTotalAmount(PaymentHeadId);

            model.PaymentHeadId = PaymentHeadId;
            model.PaymentMonthYear = paymentMonthYear;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SearchAdhocSalaryCommitmentList(int PaymentHeadId, int pageIndex, int pageSize, string CommitmentNo, string ProjectNumber)
        {
            object output = payment.GetProjectCommitment(PaymentHeadId, pageIndex, pageSize, CommitmentNo, ProjectNumber);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Commitment(int page, int pageSize, FormCollection formCollection)
        {

            var btnSave = Request["btnSave"];
            var btnProceed = Request["btnProceed"];
            var btnBack = Request["btnBack"];

            string message = "";
            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);

            var paymentMonthYear = (Request["PaymentMonthYear"] != null) ? Request["PaymentMonthYear"].ToString() : "";
            var PaymentHeadId = Convert.ToInt32((Request["PaymentHeadId"] != null) ? Convert.ToInt32(Request["PaymentHeadId"].ToString()) : -1);
            if (String.IsNullOrEmpty(paymentMonthYear))
            {
                return RedirectToAction("Salary");
            }
            //var model = payment.GetProjectCommitment(paymentMonthYear, PaymentHeadId);
            ProjectCommitmentModel model = new ProjectCommitmentModel();
            model.TotalAmount = payment.GetCommitmentTotalAmount(PaymentHeadId);
            model.PaymentHeadId = PaymentHeadId;
            model.PaymentMonthYear = paymentMonthYear;

            ViewBag.months = fo.GetAllMonths();
            ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");

            TempData["PaymentHeadId"] = PaymentHeadId;
            TempData["PaymentMonthYear"] = paymentMonthYear;
            //if (btnProceed != null)
            //{
            //    if (model != null)
            //    {
            //        for (int i = 0; i < model.Count; i++)
            //        {
            //            if (model[i].IsBalanceAavailable == false)
            //            {
            //                message = "Commitment balance is too low for CommitmentNo : " + model[i].CommitmentNo;
            //                break;
            //            }
            //        }

            //    }
            //    else
            //    {
            //        message = "Invalid model data.";
            //    }
            //}


            if (btnBack != null)
            {
                return RedirectToAction("Salary", new { PaymentHeadId = PaymentHeadId });
            }
            else if (btnProceed != null)
            {
                //if (message != "")
                //{
                //    ViewBag.Errors = message;
                //    return View(model);
                //}
                return RedirectToAction("Transaction");
            }

            return View(model);

        }

        [Authorize]
        public ActionResult Transaction(int billId = 0)
        {

            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            if (TempData["PaymentMonthYear"] == null)
            {
                return RedirectToAction("Salary");
            }

            if (TempData["PaymentHeadId"] == null || TempData["PaymentHeadId"].ToString() == "")
            {
                return RedirectToAction("Salary");
            }
            var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
            var PaymentHeadId = Convert.ToInt32((TempData["PaymentHeadId"] != null) ? TempData["PaymentHeadId"].ToString() : "");

            ViewBag.AccountGroups = adhoc.GetAccountGroup("SAL-01");
            ViewBag.AccountHead = adhoc.GetAccountHead("SAL-01");


            var model = adhoc.GetSalaryTransaction(PaymentHeadId, paymentMonthYear);
            //var tx = coreAccountService.GetTransactionDetails(0, false, "SAL", "1", false, null);
            //model.ExpenseDetail = tx.ExpenseDetail;
            //model.DeductionDetail = tx.DeductionDetail;

            ViewBag.CreditAmount = model.TotalCredit;
            ViewBag.DebitAmount = model.TotalDebit;
            if (model == null || model.TransactionId <= 0)
            {
                ViewBag.AllowProceed = "disabled";
            }
            else
            {
                ViewBag.AllowProceed = "";
            }

            if (model == null || model.Status == null || model.Status.ToLower() == "open")
            {
                ViewBag.AllowSave = "";
                ViewBag.AllowEdit = "";
            }
            else
            {
                ViewBag.AllowSave = "disabled";
                ViewBag.AllowEdit = "disabled";
            }

            TempData["PaymentMonthYear"] = paymentMonthYear;
            TempData["PaymentHeadId"] = PaymentHeadId;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Transaction(SalaryTransaction model, FormCollection formCollection)
        {
            var btnSave = Request["btnSave"];
            var btnProceed = Request["btnProceed"];
            var btnBack = Request["btnBack"];
            var btnAddRow = Request["btnAddRow"];
            var CreditAmount = Request["CreditAmount"];
            var DebitAmount = Request["DebitAmount"];

            string msg = "";
            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            if (TempData["PaymentMonthYear"] == null)
            {
                return RedirectToAction("Salary");
            }
            var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
            var PaymentHeadId = (TempData["PaymentHeadId"] != null) ? Convert.ToInt32(TempData["PaymentHeadId"].ToString()) : 0;
            ViewBag.months = fo.GetAllMonths();
            ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
            TempData["PaymentMonthYear"] = paymentMonthYear;

            ViewBag.CreditAmount = CreditAmount;
            ViewBag.DebitAmount = DebitAmount;

            ViewBag.AccountGroups = adhoc.GetAccountGroup("SAL-01");
            ViewBag.AccountHead = adhoc.GetAccountHead("SAL-01");

            if (model.ExpenseDetail == null)
            {
                var tx = coreAccountService.GetTransactionDetails(0, false, "SAL", "1", false, null);
                model.ExpenseDetail = tx.ExpenseDetail;
                model.DeductionDetail = tx.DeductionDetail;
            }


            if (btnAddRow != null)
            {
                if (model.detail == null)
                {
                    model.detail = new List<SalaryTransactionDetail>();
                }
                model.detail.Add(new SalaryTransactionDetail
                {
                    AccountGroupId = 61,
                    AccountHeadId = 138,
                    Amount = 0
                });
                return View(model);
            }

            if (ModelState.IsValid && CreditAmount == DebitAmount)
            {
                msg = adhoc.SalaryTransactionIU(model, userId);
                ViewBag.Message = msg;
                model = adhoc.GetSalaryTransaction(PaymentHeadId, paymentMonthYear);
            }
            else
            {
                var tx = coreAccountService.GetTransactionDetails(0, false, "SAL", "1", false, null);
                model.ExpenseDetail = tx.ExpenseDetail;
                model.DeductionDetail = tx.DeductionDetail;
                string messages = string.Join("<br />", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));
                if (messages == "" && CreditAmount != DebitAmount)
                {
                    messages = "Credit is not matching with debit.";
                }
                TempData["errMsg"] = messages;
                ViewBag.Message = messages;

            }

            TempData["PaymentHeadId"] = PaymentHeadId;
            if (model == null || model.TransactionId <= 0)
            {
                ViewBag.AllowProceed = "disabled";
            }
            else
            {
                ViewBag.AllowProceed = "";
            }
            if (model == null || model.Status == null || model.Status.ToLower() == "open")
            {
                ViewBag.AllowSave = "";
                ViewBag.AllowEdit = "";
            }
            else
            {
                ViewBag.AllowSave = "disabled";
                ViewBag.AllowEdit = "disabled";
            }
            if (btnBack != null)
            {
                return RedirectToAction("Commitment");
            }
            else if (btnProceed != null)
            {
                return RedirectToAction("SalaryPayment");
            }

            return View(model);
        }

        // GET: List
        [Authorize]
        public ActionResult List()
        {
            try
            {
                ViewBag.Errors = TempData["Message"];
                return View();

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: PaymentEntry
        [HttpPost]
        [Authorize]
        public ActionResult List(int pageIndex, int pageSize, SalaryPaymentHead model)
        {
            try
            {

                var data = adhoc.ListSalayPayment(pageIndex, pageSize, model);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // GET: List
        [Authorize]
        public JsonResult EmplyeeSalaryDetail(string EmpNo, string PaymentMonthYear, int TypeOfPayBill, bool IsNotInMain, int PayrollProDetId)
        {
            try
            {
                DateTime Today = System.DateTime.Now;
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                AdhocEmployeeModel model = new AdhocEmployeeModel();

                //var paybill = adhoc.GetStatusFieldById("PayOfBill", TypeOfPayBill);
                //if (PaymentMonthYear != "" && paybill.ToLower() == "main")
                //{
                model = adhoc.GetMainSalaryEmployeeDetails(EmpNo, PayrollProDetId, PaymentMonthYear, TypeOfPayBill, IsNotInMain);
                //}
                //else if (PaymentMonthYear != "" && paybill.ToLower() == "supplementary")
                //{
                //    model = adhoc.GetSubSalaryEmployee(EmpNo, PaymentMonthYear);
                //}

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult VerifyEmplyeeSalary(AdhocEmployeeModel employee, int PaymentHeadId, bool verify, string stringify, string remarks, string commitmentNo, decimal tax = 0, decimal PT = 0, int PaymentId = 0, int payrollProDetId = 0, decimal MA = 0, decimal IH = 0, decimal annualTax = 0, bool isNotInMain = false, decimal basic = 0, decimal hra = 0)
        {
            //string EmpNo, string PaymentMonthYear, int TypeOfPayBill, int PaymentHeadId, int modeOfPay
            try
            {
                DateTime Today = System.DateTime.Now;
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                AdhocEmployeeModel model = new AdhocEmployeeModel();
                SalaryPaymentHead headerModel = new SalaryPaymentHead();
                string EmpNo = employee.EmployeeID;
                string PaymentMonthYear = employee.PaymentMonthYear;
                int TypeOfPayBill = employee.TypeOfPayBill;
                int modeOfPay = employee.ModeOfPayment;
                var paybill = adhoc.GetStatusFieldById("PayOfBill", TypeOfPayBill);
                //if (PaymentMonthYear != "" && paybill.ToLower() == "main")
                //{
                //    model = adhoc.GetAnEmployeeDetails(EmpNo, PaymentMonthYear);
                //}
                //else if (PaymentMonthYear != "" && paybill.ToLower() == "supplementary")
                //{
                //    model = adhoc.GetSubSalaryEmployee(EmpNo, PaymentMonthYear);
                //}
                headerModel.PaymentHeadId = PaymentHeadId;
                headerModel.PaymentMonthYear = PaymentMonthYear;
                headerModel.TypeOfPayBill = TypeOfPayBill;
                //headerModel.AdhocEmployees = new List<AdhocEmployeeModel>();
                //headerModel.AdhocEmployees.Add(model);
                if (verify == false)
                {
                    int ret = adhoc.RemoveVerifiedEmployee(PaymentHeadId, EmpNo, userId, verify, PaymentId);
                    headerModel.OADetail = adhoc.GetAdhocOtherExpBreakUp(PaymentHeadId);
                }
                else if (headerModel != null)
                {
                    var modelBU = new JavaScriptSerializer().Deserialize<AgencyVerifyEmployeeModel>(stringify);
                    var result = adhoc.EmployeeSalaryIU(headerModel, EmpNo, payrollProDetId, employee.ModeOfPayment, userId, basic, hra, tax, PT, MA, IH, annualTax, modelBU, commitmentNo, remarks, isNotInMain);
                    if (result > 0)
                    {
                        PaymentHeadId = result;
                        headerModel.SaveStatus = "Success";
                        headerModel.PaymentHeadId = PaymentHeadId;
                        headerModel.OADetail = adhoc.GetAdhocOtherExpBreakUp(PaymentHeadId);
                    }
                    else if (result == -2)
                    {
                        //headerModel.PaymentHeadId = PaymentHeadId;
                        headerModel.SaveStatus = "Commitment not exists for this employee. Please update the commitment and try again.";
                    }
                    else if (result == -3)
                    {
                        //headerModel.PaymentHeadId = PaymentHeadId;
                        headerModel.SaveStatus = "Employee net salary is greater than commitment available balance. Please update the commitment and try again.";
                    }
                    else if (result == -4)
                        headerModel.SaveStatus = "Can't able to create contingencies commitment. Please check the project details.";
                    else if (result == -5)
                        headerModel.SaveStatus = "Project number mismatch between commitment and employee working";
                    else if (result == -1)
                        headerModel.SaveStatus = "Salary already processed for this Month & Year.";
                }
                return Json(headerModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: List
        [Authorize]
        public JsonResult GetSalaryPayment(int page, int pageSize)
        {
            try
            {
                DateTime Today = System.DateTime.Now;
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                var PaymentHeadId = 0;
                var model = payment.GetEmployeeSalary(page, pageSize, PaymentHeadId);

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult LoadAdhocEmpList(string term, string typeOfPayBill, string PaymentMonthYear)
        {
            try
            {
                var data = adhoc.GetACAdhocEmpList(term, typeOfPayBill, PaymentMonthYear);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadAdhocOtherExpBreakUp(int PaymentHeadId)
        {
            try
            {
                var data = adhoc.GetAdhocOtherExpBreakUp(PaymentHeadId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult AdhocSelectedEmployee(SalaryPaymentHead model, string Department, string EmployeeId, string EmployeeName, string ModeOfPaymentName, int EmployeeCategory, int pageIndex, int pageSize)
        {
            var pagedList = new PagedList<AdhocEmployeeModel>();
            try
            {
                string msg = "";
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                List<AdhocEmployeeModel> employee = new List<AdhocEmployeeModel>();
                List<SalaryModel> SalaryList = new List<SalaryModel>();


                if (model.TypeOfPayBill < 0 || model.PaymentMonthYear == "" || model.PaymentMonthYear == null || fo.IsFutureMonthYear(model.PaymentMonthYear) == false)
                {
                    pagedList.Data = new List<AdhocEmployeeModel>();
                    return Json(pagedList, JsonRequestBehavior.AllowGet);
                }

                if (model.PaymentHeadId == 0)
                {
                    model.PaymentHeadId = -1;
                }
                // var paybill = adhoc.GetStatusFieldById("PayOfBill", model.TypeOfPayBill);
                adhoc.setFilter(EmployeeId, EmployeeName, Department, EmployeeCategory);
                //if (model.PaymentMonthYear != null && model.PaymentMonthYear != "" && paybill.ToLower() == "main")
                //{
                //    adhoc.setFilter(EmployeeId, EmployeeName, Department, EmployeeCategory);
                //    pagedList = adhoc.GetEmployeesByPaymentHead(model.PaymentHeadId, pageIndex, pageSize);
                //}
                //else if (model.PaymentMonthYear != null && paybill.ToLower() == "supplementary")
                //{
                pagedList = adhoc.GetEmployeesByPaymentHead(model.PaymentHeadId, ModeOfPaymentName, pageIndex, pageSize);
                //}

                //  var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
                // int processGuideLineId = Process.ProcessGuidelineId;
                int refId = (model.PaymentHeadId > 0) ? model.PaymentHeadId : -1;
                pagedList.CurrentPage = pageIndex;
                pagedList.pageSize = pageSize;
                pagedList.visiblePages = 5;

                return Json(pagedList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pagedList.Data = new List<AdhocEmployeeModel>();
                return Json(pagedList, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult _Projection(string empNo, string paymentMonthYear)
        {
            ProjectionModel model = new ProjectionModel();
            try
            {
                FinOp fo = new FinOp(DateTime.Now);
                string finPeriod = fo.GetFinPeriod(paymentMonthYear);
                int finyear = Common.GetFinancialYearId(finPeriod);
                model.SalaryDet = payment.GetEmployeesSalaryDetails(empNo, finyear);
                model.SupplemSalaryDet = payment.GetEmployeesSupplymentarySalaryDetails(empNo, finyear);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        [HttpPost]
        public ActionResult _OfficeOrder(string empNo)
        {
            RCTPopupModel model = new RCTPopupModel();
            try
            {
                RequirementService RQS = new RequirementService();
                model = RQS.getEmployeeWorkingDatails(empNo);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
        //#region Pensioner Salary
        //[Authorize]
        //public ActionResult PensionerSalary(int PaymentHeadId = 0)
        //{
        //    var user = User.Identity.Name;
        //    var userId = AdminService.getUserByName(user);
        //    var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
        //    int processGuideLineId = Process.ProcessGuidelineId;

        //    ViewBag.processGuideLineId = processGuideLineId;
        //    ViewBag.currentRefId = -1;
        //    ViewBag.months = fo.GetAllMonths();
        //    //ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
        //    ViewBag.ModeOfPayment = Common.GetCodeControlList("PaymentMode");
        //    //ViewBag.Department = adhoc.GetDepartments();
        //    ViewBag.EmployeeCategoryList = adhoc.GetEmployeeCategory("Main  account");
        //    ViewBag.TypeList = Common.GetCodeControlList("Salary Breakup Type");
        //    ViewBag.HeadList = new List<MasterlistviewModel>();

        //    PagedList<AdhocEmployeeModel> model = new PagedList<AdhocEmployeeModel>();
        //    var peymentHead = adhoc.GetSalayPaymentHead(PaymentHeadId);

        //    string Status = "open";


        //    if (peymentHead != null)
        //    {
        //        ViewBag.SelectedPaytype = peymentHead.TypeOfPayBill;
        //        ViewBag.SelectedPaymonth = peymentHead.PaymentMonthYear;
        //    }

        //    if (Status == null || Status.ToLower() == "open")
        //    {
        //        ViewBag.AllowSave = "";
        //        ViewBag.AllowEdit = "";
        //        ViewBag.Mode = "Edit";
        //    }
        //    else
        //    {
        //        ViewBag.AllowSave = "disabled";
        //        ViewBag.AllowEdit = "disabled";
        //        ViewBag.Mode = "View";
        //    }

        //    var OADetail = adhoc.GetAdhocOtherExpBreakUp(PaymentHeadId);
        //    ViewBag.TotalDistributionAmount = OADetail.TotalDistributionAmount;
        //    ViewBag.TotalHonororiumAmount = OADetail.TotalHonororiumAmount;
        //    ViewBag.TotalMandaysAmount = OADetail.TotalMandaysAmount;
        //    ViewBag.TotalFellowshipAmount = OADetail.TotalFellowshipAmount;
        //    return View("PensionerSalaryInitGrid", model);
        //}

        //[HttpPost]
        //public ActionResult PensionerSalary(ValidateSalary model, FormCollection formCollection)
        //{
        //    string msg = "";
        //    var user = User.Identity.Name;
        //    var userId = AdminService.getUserByName(user);

        //    var btnSave = Request["btnSave"];
        //    var btnProceed = Request["btnProceed"];
        //    var btnBack = Request["btnBack"];
        //    var btnValidate = Request["btnValidate"];
        //    //var Department = Request["departmentcode"];
        //    var PaymentMonthYear = Request["PaymentMonthYear"];
        //    var paymentHeadId = Request["PaymentHeadId"];
        //    var Status = Request["Status"];
        //    // var typeOfPayBill = Request["TypeOfPayBill"];
        //    //var EmployeeId = Request["EmployeeId"];
        //    //var EmployeeName = Request["EmployeeName"];
        //    // var DepartmentCode = Request["DepartmentCode"];
        //    string empCat = String.IsNullOrEmpty(Request["EmployeeCategory"]) ? "0" : Request["EmployeeCategory"];
        //    int EmployeeCategory = Convert.ToInt32(empCat);

        //    int PaymentHeadId = (paymentHeadId != null && paymentHeadId != "") ? Convert.ToInt32(paymentHeadId) : 0;
        //    // int TypeOfPayBill = (typeOfPayBill != null && typeOfPayBill != "") ? Convert.ToInt32(typeOfPayBill) : 0;

        //    ViewBag.PaymentHeadId = PaymentHeadId;
        //    ViewBag.months = fo.GetAllMonths();
        //    // ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
        //    ViewBag.ModeOfPayment = Common.GetCodeControlList("PaymentMode");
        //    //ViewBag.Department = adhoc.GetDepartments();
        //    ViewBag.EmployeeCategoryList = adhoc.GetEmployeeCategory("Main  account");
        //    ViewBag.TypeList = Common.GetCodeControlList("Salary Breakup Type");
        //    ViewBag.HeadList = new List<MasterlistviewModel>();
        //    //   ViewBag.SelectedPaytype = TypeOfPayBill;
        //    ViewBag.SelectedPaymonth = PaymentMonthYear;
        //    //ViewBag.SelectedDepartment = Department;
        //    ViewBag.SelectedEmpCategory = EmployeeCategory;


        //    //if (TypeOfPayBill <= 0)
        //    //{
        //    //    msg = "Please select Type of PayBill";
        //    //    ModelState.AddModelError("", msg);
        //    //    ViewBag.Errors = msg;
        //    //    return View("SalaryInitGrid");
        //    //}
        //    if (PaymentMonthYear == "" || PaymentMonthYear == null)
        //    {
        //        //msg = "Please select Payment Month Year";
        //        //ModelState.AddModelError("", msg);
        //        //ViewBag.Errors = msg;
        //        return View("PensionerSalaryInitGrid");
        //    }
        //    else if (fo.IsFutureMonthYear(PaymentMonthYear) == false)
        //    {
        //        msg = "Selected salary month should not be future month";
        //        ModelState.AddModelError("", msg);
        //        ViewBag.Errors = msg;
        //        return View("PensionerSalaryInitGrid");
        //    }
        //    if (btnValidate != null)
        //    {
        //        var data = ValidateSalTemplate(model.template, 0, PaymentMonthYear);
        //        if (data.Item1 == "Valid")
        //        {
        //            ViewBag.Message = "Validation has been done successfully.";
        //            ViewBag.uploadId = data.Item2;
        //        }
        //        else
        //            ViewBag.Errors = data.Item1;
        //        return View("PensionerSalaryInitGrid");
        //    }
        //    //var paybill = adhoc.GetStatusFieldById("PayOfBill", TypeOfPayBill);

        //    var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
        //    int processGuideLineId = Process.ProcessGuidelineId;
        //    int refId = (PaymentHeadId > 0) ? PaymentHeadId : -1;

        //    ViewBag.processGuideLineId = processGuideLineId;
        //    ViewBag.currentRefId = refId;

        //    TempData["PaymentHeadId"] = PaymentHeadId;
        //    TempData["PaymentMonthYear"] = PaymentMonthYear;
        //    //  TempData["TypeOfPayBill"] = TypeOfPayBill;
        //    if (PaymentHeadId <= 0)
        //    {
        //        ViewBag.AllowProceed = "disabled";
        //    }
        //    else
        //    {
        //        ViewBag.AllowProceed = "";
        //    }
        //    if (Status == null || Status == "open")
        //    {
        //        ViewBag.AllowSave = "";
        //        ViewBag.AllowEdit = "";
        //    }
        //    else
        //    {
        //        ViewBag.AllowSave = "disabled";
        //        ViewBag.AllowEdit = "disabled";
        //    }

        //    if (btnBack != null)
        //    {
        //        return RedirectToAction("PensionerSalaryList");
        //    }
        //    else if (btnProceed != null)
        //    {
        //        return RedirectToAction("PensionerCommitment");
        //    }
        //    else
        //    {
        //        return View("PensionerSalaryInitGrid");
        //    }
        //}

        //public FileStreamResult ExportPensionerSalary(int PaymentHeadId, string PaymentMonthYear)
        //{
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        ds = adhoc.GetPensionSalaryEmployeesList(PaymentMonthYear, PaymentHeadId);

        //        return coreAccountService.toSpreadSheet(ds);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        //public ActionResult SearchPensionerSalary(SalaryPaymentHead model, string Department, string EmployeeId, string EmployeeName, int EmployeeCategory, int pageIndex, int pageSize)
        //{
        //    var pagedList = new PagedList<AdhocEmployeeModel>();
        //    try
        //    {
        //        string msg = "";

        //        var user = User.Identity.Name;
        //        var userId = AdminService.getUserByName(user);
        //        List<SalaryModel> SalaryList = new List<SalaryModel>();

        //        if (model.PaymentMonthYear == "" || model.PaymentMonthYear == null || fo.IsFutureMonthYear(model.PaymentMonthYear) == false)
        //        {
        //            pagedList.Data = new List<AdhocEmployeeModel>();
        //            return Json(pagedList, JsonRequestBehavior.AllowGet);
        //        }
        //        //var paybill = adhoc.GetStatusFieldById("PayOfBill", model.TypeOfPayBill);
        //        adhoc.setFilter(EmployeeId, EmployeeName, Department, EmployeeCategory);
        //        if (model.PaymentMonthYear != null && model.PaymentMonthYear != "")
        //        {
        //            pagedList = adhoc.GetPensionSalaryEmployees(model.PaymentMonthYear, model.PaymentHeadId, pageIndex, pageSize);
        //        }

        //        pagedList.CurrentPage = pageIndex;
        //        pagedList.pageSize = pageSize;
        //        pagedList.visiblePages = 5;

        //        return Json(pagedList, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        pagedList.Data = new List<AdhocEmployeeModel>();
        //        return Json(pagedList, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[Authorize]
        //public ActionResult PensionerCommitment()
        //{
        //    var user = User.Identity.Name;
        //    var userId = AdminService.getUserByName(user);
        //    if (TempData["PaymentMonthYear"] == null)
        //    {
        //        return RedirectToAction("PensionerSalary");
        //    }
        //    var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
        //    var PaymentHeadId = (TempData["PaymentHeadId"] != null) ? Convert.ToInt32(TempData["PaymentHeadId"].ToString()) : -1;
        //    //var model = payment.GetProjectCommitment(PaymentHeadId,1,10,"","");
        //    ViewBag.months = fo.GetAllMonths();
        //    ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
        //    ProjectCommitmentModel model = new ProjectCommitmentModel();
        //    model.TotalAmount = payment.GetCommitmentTotalAmount(PaymentHeadId);

        //    model.PaymentHeadId = PaymentHeadId;
        //    model.PaymentMonthYear = paymentMonthYear;

        //    return View(model);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult SearchPensionerSalaryCommitmentList(int PaymentHeadId, int pageIndex, int pageSize, string CommitmentNo, string ProjectNumber)
        //{
        //    object output = payment.GetProjectCommitment(PaymentHeadId, pageIndex, pageSize, CommitmentNo, ProjectNumber);
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}
        //[Authorize]
        //[HttpPost]
        //public ActionResult PensionerCommitment(int page, int pageSize, FormCollection formCollection)
        //{

        //    var btnSave = Request["btnSave"];
        //    var btnProceed = Request["btnProceed"];
        //    var btnBack = Request["btnBack"];

        //    string message = "";
        //    var user = User.Identity.Name;
        //    var userId = AdminService.getUserByName(user);

        //    var paymentMonthYear = (Request["PaymentMonthYear"] != null) ? Request["PaymentMonthYear"].ToString() : "";
        //    var PaymentHeadId = Convert.ToInt32((Request["PaymentHeadId"] != null) ? Convert.ToInt32(Request["PaymentHeadId"].ToString()) : -1);
        //    if (String.IsNullOrEmpty(paymentMonthYear))
        //    {
        //        return RedirectToAction("PensionerSalary");
        //    }
        //    //var model = payment.GetProjectCommitment(paymentMonthYear, PaymentHeadId);
        //    ProjectCommitmentModel model = new ProjectCommitmentModel();
        //    model.TotalAmount = payment.GetCommitmentTotalAmount(PaymentHeadId);
        //    model.PaymentHeadId = PaymentHeadId;
        //    model.PaymentMonthYear = paymentMonthYear;

        //    ViewBag.months = fo.GetAllMonths();
        //    ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");

        //    TempData["PaymentHeadId"] = PaymentHeadId;
        //    TempData["PaymentMonthYear"] = paymentMonthYear;
        //    //if (btnProceed != null)
        //    //{
        //    //    if (model != null)
        //    //    {
        //    //        for (int i = 0; i < model.Count; i++)
        //    //        {
        //    //            if (model[i].IsBalanceAavailable == false)
        //    //            {
        //    //                message = "Commitment balance is too low for CommitmentNo : " + model[i].CommitmentNo;
        //    //                break;
        //    //            }
        //    //        }

        //    //    }
        //    //    else
        //    //    {
        //    //        message = "Invalid model data.";
        //    //    }
        //    //}


        //    if (btnBack != null)
        //    {
        //        return RedirectToAction("PensionerSalary", new { PaymentHeadId = PaymentHeadId });
        //    }
        //    else if (btnProceed != null)
        //    {
        //        //if (message != "")
        //        //{
        //        //    ViewBag.Errors = message;
        //        //    return View(model);
        //        //}
        //        return RedirectToAction("PensionerTransaction");
        //    }

        //    return View(model);

        //}

        //[Authorize]
        //public ActionResult PensionerTransaction(int billId = 0)
        //{

        //    var user = User.Identity.Name;
        //    var userId = AdminService.getUserByName(user);
        //    if (TempData["PaymentMonthYear"] == null)
        //    {
        //        return RedirectToAction("PensionerSalary");
        //    }

        //    if (TempData["PaymentHeadId"] == null || TempData["PaymentHeadId"].ToString() == "")
        //    {
        //        return RedirectToAction("PensionerSalary");
        //    }
        //    var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
        //    var PaymentHeadId = Convert.ToInt32((TempData["PaymentHeadId"] != null) ? TempData["PaymentHeadId"].ToString() : "");

        //    ViewBag.AccountGroups = adhoc.GetAccountGroup("SAL-01");
        //    ViewBag.AccountHead = adhoc.GetAccountHead("SAL-01");


        //    var model = adhoc.GetPensionerSalaryTransaction(PaymentHeadId, paymentMonthYear);
        //    //var tx = coreAccountService.GetTransactionDetails(0, false, "SAL", "1", false, null);
        //    //model.ExpenseDetail = tx.ExpenseDetail;
        //    //model.DeductionDetail = tx.DeductionDetail;

        //    ViewBag.CreditAmount = model.TotalCredit;
        //    ViewBag.DebitAmount = model.TotalDebit;
        //    if (model == null || model.TransactionId <= 0)
        //    {
        //        ViewBag.AllowProceed = "disabled";
        //    }
        //    else
        //    {
        //        ViewBag.AllowProceed = "";
        //    }

        //    if (model == null || model.Status == null || model.Status.ToLower() == "open")
        //    {
        //        ViewBag.AllowSave = "";
        //        ViewBag.AllowEdit = "";
        //    }
        //    else
        //    {
        //        ViewBag.AllowSave = "disabled";
        //        ViewBag.AllowEdit = "disabled";
        //    }

        //    TempData["PaymentMonthYear"] = paymentMonthYear;
        //    TempData["PaymentHeadId"] = PaymentHeadId;

        //    return View(model);
        //}

        //[Authorize]
        //[HttpPost]
        //public ActionResult PensionerTransaction(SalaryTransaction model, FormCollection formCollection)
        //{
        //    var btnSave = Request["btnSave"];
        //    var btnProceed = Request["btnProceed"];
        //    var btnBack = Request["btnBack"];
        //    var btnAddRow = Request["btnAddRow"];
        //    var CreditAmount = Request["CreditAmount"];
        //    var DebitAmount = Request["DebitAmount"];

        //    string msg = "";
        //    var user = User.Identity.Name;
        //    var userId = AdminService.getUserByName(user);
        //    if (TempData["PaymentMonthYear"] == null)
        //    {
        //        return RedirectToAction("PensionerSalary");
        //    }
        //    var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
        //    var PaymentHeadId = (TempData["PaymentHeadId"] != null) ? Convert.ToInt32(TempData["PaymentHeadId"].ToString()) : 0;
        //    ViewBag.months = fo.GetAllMonths();
        //    ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
        //    TempData["PaymentMonthYear"] = paymentMonthYear;

        //    ViewBag.CreditAmount = CreditAmount;
        //    ViewBag.DebitAmount = DebitAmount;

        //    ViewBag.AccountGroups = adhoc.GetAccountGroup("SAL-01");
        //    ViewBag.AccountHead = adhoc.GetAccountHead("SAL-01");

        //    if (model.ExpenseDetail == null)
        //    {
        //        var tx = coreAccountService.GetTransactionDetails(0, false, "PSAL", "1", false, null);
        //        model.ExpenseDetail = tx.ExpenseDetail;
        //        model.DeductionDetail = tx.DeductionDetail;
        //    }


        //    if (btnAddRow != null)
        //    {
        //        if (model.detail == null)
        //        {
        //            model.detail = new List<SalaryTransactionDetail>();
        //        }
        //        model.detail.Add(new SalaryTransactionDetail
        //        {
        //            AccountGroupId = 61,
        //            AccountHeadId = 138,
        //            Amount = 0
        //        });
        //        return View(model);
        //    }

        //    if (ModelState.IsValid && CreditAmount == DebitAmount)
        //    {
        //        msg = adhoc.SalaryTransactionIU(model, userId);
        //        ViewBag.Message = msg;
        //        model = adhoc.GetPensionerSalaryTransaction(PaymentHeadId, paymentMonthYear);
        //    }
        //    else
        //    {
        //        var tx = coreAccountService.GetTransactionDetails(0, false, "PSAL", "1", false, null);
        //        model.ExpenseDetail = tx.ExpenseDetail;
        //        model.DeductionDetail = tx.DeductionDetail;
        //        string messages = string.Join("<br />", ModelState.Values
        //                            .SelectMany(x => x.Errors)
        //                            .Select(x => x.ErrorMessage));
        //        if (messages == "" && CreditAmount != DebitAmount)
        //        {
        //            messages = "Credit is not matching with debit.";
        //        }
        //        TempData["errMsg"] = messages;
        //        ViewBag.Message = messages;

        //    }

        //    TempData["PaymentHeadId"] = PaymentHeadId;
        //    if (model == null || model.TransactionId <= 0)
        //    {
        //        ViewBag.AllowProceed = "disabled";
        //    }
        //    else
        //    {
        //        ViewBag.AllowProceed = "";
        //    }
        //    if (model == null || model.Status == null || model.Status.ToLower() == "open")
        //    {
        //        ViewBag.AllowSave = "";
        //        ViewBag.AllowEdit = "";
        //    }
        //    else
        //    {
        //        ViewBag.AllowSave = "disabled";
        //        ViewBag.AllowEdit = "disabled";
        //    }
        //    if (btnBack != null)
        //    {
        //        return RedirectToAction("PensionerCommitment");
        //    }
        //    else if (btnProceed != null)
        //    {
        //        return RedirectToAction("PensionerSalaryPayment");
        //    }

        //    return View(model);
        //}

        //// GET: List
        //[Authorize]
        //public ActionResult PensionerSalaryList()
        //{
        //    try
        //    {
        //        return View();

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //// POST: PaymentEntry
        //[HttpPost]
        //[Authorize]
        //public ActionResult PensionerSalaryList(int pageIndex, int pageSize, SalaryPaymentHead model)
        //{
        //    try
        //    {

        //        var data = adhoc.ListPensionerSalaryPayment(pageIndex, pageSize, model);
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[Authorize]
        //public ActionResult PensionerSalaryPayment()
        //{
        //    int page = 1;
        //    int pageSize = 10;
        //    var user = User.Identity.Name;
        //    var userId = AdminService.getUserByName(user);
        //    var PaymentHeadId = TempData["PaymentHeadId"];
        //    int paymentId = 0;
        //    if (PaymentHeadId != null)
        //    {
        //        paymentId = Convert.ToInt32(PaymentHeadId);
        //    }

        //    //var monthYear = fo.GetCurrentMonthYear();
        //    if (TempData["PaymentMonthYear"] == null)
        //    {
        //        return RedirectToAction("PensionerSalary");
        //    }

        //    var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
        //    var model = payment.GetEmployeeSalary(page, pageSize, paymentId);
        //    ViewBag.SelectedMonth = paymentMonthYear;
        //    ViewBag.months = fo.GetAllMonths();
        //    ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
        //    var SalaryPaymentHead = adhoc.GetSalayPaymentHead(paymentId);
        //    if (SalaryPaymentHead == null || SalaryPaymentHead.Status == null || SalaryPaymentHead.Status.ToLower() == "open")
        //    {
        //        ViewBag.AllowSubmit = "";
        //        ViewBag.AllowSave = "";
        //        ViewBag.AllowEdit = "";
        //    }
        //    else
        //    {
        //        ViewBag.AllowSubmit = "disabled";
        //        ViewBag.AllowSave = "disabled";
        //        ViewBag.AllowEdit = "disabled";
        //    }
        //    TempData["PaymentMonthYear"] = paymentMonthYear;
        //    TempData["PaymentHeadId"] = paymentId;
        //    return View(model);
        //}

        //[Authorize]
        //[HttpPost]
        //public ActionResult PensionerSalaryPayment(int page, int pageSize, FormCollection formCollection)
        //{
        //    var btnSave = Request["btnSave"];
        //    var btnSubmit = Request["btnSubmit"];
        //    var btnBack = Request["btnBack"];

        //    var PaymentHeadId = TempData["PaymentHeadId"];
        //    int paymentId = 0;
        //    if (PaymentHeadId != null)
        //    {
        //        paymentId = Convert.ToInt32(PaymentHeadId);
        //    }

        //    var user = User.Identity.Name;
        //    var userId = AdminService.getUserByName(user);
        //    var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
        //    var model = payment.GetEmployeeSalary(page, pageSize, paymentId);
        //    ViewBag.months = fo.GetAllMonths();
        //    ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
        //    ViewBag.SelectedMonth = paymentMonthYear;

        //    var SalaryPaymentHead = adhoc.GetSalayPaymentHead(paymentId);
        //    if (SalaryPaymentHead == null || SalaryPaymentHead.Status == null || SalaryPaymentHead.Status.ToLower() == "open")
        //    {
        //        ViewBag.AllowSubmit = "";
        //        ViewBag.AllowSave = "";
        //        ViewBag.AllowEdit = "";
        //    }
        //    else
        //    {
        //        ViewBag.AllowSubmit = "disabled";
        //        ViewBag.AllowSave = "disabled";
        //        ViewBag.AllowEdit = "disabled";
        //    }

        //    TempData["PaymentMonthYear"] = paymentMonthYear;
        //    TempData["PaymentHeadId"] = paymentId;

        //    if (btnBack != null)
        //    {
        //        return RedirectToAction("PensionerTransaction");
        //    }
        //    else if (btnSubmit != null)
        //    {

        //        SalaryPaymentHead payHead = new SalaryPaymentHead();

        //        string currentStatus = "open";
        //        string newStatus = "Approval Pending";
        //        var msg = adhoc.UpdateSalaryPayment(paymentId, currentStatus, newStatus, userId);
        //        if (msg == "")
        //        {
        //            return View(model);
        //        }
        //        TempData["Message"] = msg;
        //        return RedirectToAction("PensionerSalaryList");
        //    }
        //    return View(model);
        //}

        //#endregion

        #region Agency Salary
        public ActionResult AgencySalaryList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AgencySalary(int agencySalaryId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.months = fo.GetAllMonths();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                AgencySalaryModel model = new AgencySalaryModel();
                var data = new PagedData<AgencyStaffDetailsModel>();
                string currMY = fo.GetCurrentMonthYear();
                model.MonthYear = currMY;
                string[] status = { "Open", "Init" };
                if (agencySalaryId > 0 && payment.ValidateAgencySalaryStatus(agencySalaryId, status))
                {
                    model = payment.GetAgencySalaryDetails(agencySalaryId);
                    if (model.CheckListDetail.Count == 0)
                        model.CheckListDetail = Common.GetCheckedList(63);
                }
                else if (agencySalaryId > 0)
                    return RedirectToAction("AgencySalaryList");
                //int Page = 1, Pagesize = 5;
                //data = payment.GetAgencyEmployeeSalary(Page, Pagesize, agencySalaryId, currMY);
                //data.pageSize = 5;
                //data.visiblePages = 5;
                //data.CurrentPage = Page;
                //model.EmployeeDetails = data;
                model.CreditorType = "Agency";
                model.PaymentNo = Common.getPaymentNo(model.AgencySalaryID ?? 0);
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "Home");
            }

        }
        [HttpPost]
        public ActionResult AgencySalary(AgencySalaryModel model)
        {
            try
            {
                model.CreditorType = "Agency";
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.months = fo.GetAllMonths();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (ModelState.IsValid)
                {

                    string validationMsg = ValidateAgencySalary(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = payment.CreateSalaryAgency(model, logged_in_user);
                    if (result > 0)
                    {
                        TempData["succMsg"] = "Agency salary payment has been updated successfully.";
                        return RedirectToAction("AgencySalaryList");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator.";
                    }
                }

                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {

                var emptyList = new List<MasterlistviewModel>();
                ViewBag.months = fo.GetAllMonths();
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }

        private string ValidateAgencySalary(AgencySalaryModel model)
        {
            decimal netSalAmt = model.NetAmount ?? 0;
            decimal payableAmt = model.NetPayable ?? 0;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.NetCommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal netDrAmt = ttldrAmt + ttldeductAmt;

            if (netSalAmt > commitmentAmt || payableAmt < commitmentAmt)
                msg = "There is a mismatch between the requested advance value and allocated commitment value. Please update the value to continue.";
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }
            if (netCrAmt != ttlExpAmt || netCrAmt != netDrAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";

            if (payableAmt != netCrAmt)
                msg = msg == "Valid" ? "TNot a valid entry. Net payable and transaction value are not equal." : msg + "<br /> Not a valid entry. Net payable and transaction value are not equal.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";

            return msg;
        }
        [HttpGet]
        public ActionResult AgencySalaryView(int agencySalaryId)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.months = fo.GetAllMonths();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                AgencySalaryModel model = new AgencySalaryModel();
                var data = new PagedData<AgencyStaffDetailsModel>();
                string currMY = fo.GetCurrentMonthYear();
                model.MonthYear = currMY;
                model = payment.GetAgencySalaryDetails(agencySalaryId);
                if (model.CheckListDetail.Count == 0)
                    model.CheckListDetail = Common.GetCheckedList(63);
                //int Page = 1, Pagesize = 5;
                //data = payment.GetAgencyEmployeeSalary(Page, Pagesize, agencySalaryId, currMY);
                //data.pageSize = 5;
                //data.visiblePages = 5;
                //data.CurrentPage = Page;
                //model.EmployeeDetails = data;
                model.CreditorType = "Agency";
                model.PaymentNo = Common.getPaymentNo(model.AgencySalaryID ?? 0);
                ViewBag.disabled = "readonly";
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "Home");
            }

        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult _AgencySalaryStaffDetailsPartial(int Page, int AgencySalaryID, string MonthYear)
        //{
        //    AgencySalaryModel model = new AgencySalaryModel();
        //    model.EmployeeDetails = new PagedData<AgencyStaffDetailsModel>();
        //    StaffPaymentService _SPS = new StaffPaymentService();
        //    int Pagesize = 5;
        //    model.EmployeeDetails = _SPS.GetAgencyEmployeeSalary(Page, Pagesize, AgencySalaryID, MonthYear);
        //    model.EmployeeDetails.pageSize = 5;
        //    model.EmployeeDetails.visiblePages = 5;
        //    model.EmployeeDetails.CurrentPage = Page;
        //    model.EmployeeDetails = model.EmployeeDetails;
        //    return PartialView(model);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SearchAgencySalaryList(int vendorId, int pageIndex, int pageSize, int AgencySalaryId, string MonthYear, string EmployeeId, string Name)
        {
            object output = payment.GetAgencyEmployeeSalary(vendorId,pageIndex, pageSize, AgencySalaryId, MonthYear, EmployeeId, Name);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAgencySalaryList()
        {
            try
            {
                var model = payment.getAgencySalaryList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public JsonResult SearchAgencySalaryList(AgencySearchFieldModel model)
        {
            object output = StaffPaymentService.SearchAgencySalaryList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public FileStreamResult ExportAgencySalary(int AgencySalaryID, string MonthYear)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = payment.GetAgencyEmployeeSalary(AgencySalaryID, MonthYear);
                return coreAccountService.toSpreadSheet(ds);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SearchAgencySalaryVerifiedList(int pageIndex, int pageSize, int AgencySalaryId, string EmployeeId, string Name)
        {
            object output = payment.GetVerifiedEmployeeSalary(pageIndex, pageSize, AgencySalaryId, EmployeeId, Name);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SearchAgencySalaryCommitmentList(int pageIndex, int pageSize, int AgencySalaryId, string CommitmentNumber, string ProjectNumber, string HeadName)
        {
            object output = payment.GetAgencySalaryCommitmentDetail(pageIndex, pageSize, AgencySalaryId, CommitmentNumber, ProjectNumber, HeadName);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult PopupEmployeeDetails(string EmployeeID, int AgencySalaryID, string MonthYear, int payrollDetId)
        {
            try
            {
                AgencyStaffDetailsModel model = new AgencyStaffDetailsModel();
                ViewBag.TypeList = Common.GetCodeControlList("Salary Breakup Type");
                ViewBag.HeadList = new List<MasterlistviewModel>();
                if (EmployeeID != "0")
                {
                    string validationMsg = payment.ValidateAgencyVerify(EmployeeID, payrollDetId, AgencySalaryID, MonthYear);
                    if (validationMsg != "Valid")
                    {
                        Response.StatusCode = 400;
                        return Json(validationMsg, JsonRequestBehavior.AllowGet);
                        //return new HttpStatusCodeResult(400, validationMsg);
                    }
                    model = payment.getEmployeeSalaryDetails(AgencySalaryID, payrollDetId);
                }
                else
                {
                    model.AgencySalaryID = AgencySalaryID;
                    model.AddNew_f = true;
                }
                return PartialView(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteVerifiedEmployee(int VerifiedSalaryId)
        {
            int userId = Common.GetUserid(User.Identity.Name);
            object output = payment.DeleteVerifiedEmployee(VerifiedSalaryId, userId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AgencySalaryApproved(int agencySalaryId)
        {
            try
            {
                lock (lockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    string[] chckStatus = { "Open" };
                    if (payment.ValidateAgencySalaryStatus(agencySalaryId, chckStatus))
                    {
                        bool cStatus = payment.SLACommitmentBalanceUpdate(agencySalaryId, false, false, userId, "SLA");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = payment.ApproveSLA(agencySalaryId, userId);
                        if (!status)
                            payment.SLACommitmentBalanceUpdate(agencySalaryId, true, false, userId, "SLA");
                        return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult VerifyEmployeeDetails(string stringify)
        {
            try
            {
                var model = new JavaScriptSerializer().Deserialize<AgencyVerifyEmployeeModel>(stringify);
                string validationMsg = payment.ValidateAgencyVerify(model.EmployeeId, model.PayrollDetailId, model.AgencySalaryID, model.MonthYear, model, true);
                if (validationMsg != "Valid")
                    return Json(new { id = -1, msg = validationMsg });
                int userId = Common.GetUserid(User.Identity.Name);
                lock (lockObj)
                {
                    var data = payment.VerifyEmployeeDetails(model, userId);

                    if (data.Status == "success")
                    {
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { Status = "error", msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetEmployeeDetails(int payrollDetId, int agencySalaryID)
        {
            try
            {
                var data = payment.getEmployeeSalaryDetails(agencySalaryID, payrollDetId);
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyAllEmployeeDetails(int AgencySalaryID, string MonthYear)
        {
            try
            {
                lock (lockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    var data = payment.VerifyAllEmployeeDetails(AgencySalaryID, MonthYear, userId);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public ActionResult getNetTotalNetSalary(int AgencySalaryID)
        {
            try
            {
                var Result = Common.getSumNetSalary(AgencySalaryID);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                var Result = Common.getSumNetSalary(AgencySalaryID);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetSalaryBreakUpHead(Int32 categoryId, int groupId)
        {
            try
            {
                object output = Common.GetCommonHeadList(categoryId, groupId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetCommonHeadFlag(Int32 headId)
        {
            try
            {
                object output = Common.GetCommonHeadFlag(headId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadTandMEmpList(string term)
        {
            try
            {
                var data = Common.GetACTandMEmpList(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        //[HttpGet]
        //public JsonResult TestUpdateSalaryPayment()
        //{
        //    try
        //    {
        //        AdhocSalaryProcess SP = new AdhocSalaryProcess();
        //        var data = SP.TestUpdateSalaryPayment();
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

    }
}