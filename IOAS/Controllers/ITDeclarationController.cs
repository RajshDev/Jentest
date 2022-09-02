using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IOAS.Models;
using IOAS.GenericServices;
using IOAS.Infrastructure;

namespace IOAS.Controllers
{
    public class ITDeclarationController : Controller
    {

        StaffPaymentService payment = new StaffPaymentService();
        AdhocSalaryProcess adhoc = new AdhocSalaryProcess();
        OfficeOrderService order = new OfficeOrderService();
        FinOp fp = new FinOp(System.DateTime.Now);
        // GET: ITDeclaration
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult GetList(string EmpId = "", int FinYear = 0)
        {
            try
            {
                EmpITDeductionModel model = new EmpITDeductionModel();
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);

                //int PayBillNo = EmpId;
                // string employeeId = Common.GetAdhocStaffEmployeeId(EmpId);
                string employeeId = EmpId;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                ViewBag.Finyr = Common.GetAllFinancial();
                model.FinancialYear = FinYear;
                model.ItList = payment.GetITEmpDeclarations(employeeId, FinYear);
                model.ItSOP = payment.GetITEmpSOP();
                model.ItOtherIncome = payment.GetITEmpOtherIncome(employeeId, FinYear);
                model.EmpInfo = StaffPaymentService.GetEmployeeDetails(employeeId);
                model.SalaryDet = payment.GetEmployeesSalaryDetails(employeeId, FinYear);
                model.SupplemSalaryDet = payment.GetEmployeesSupplymentarySalaryDetails(employeeId, FinYear);

                return View("List", model);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize]
        public ActionResult List(string EmpId = "", int FinYear = 0)
        {
            try
            {
                EmpITDeductionModel model = new EmpITDeductionModel();
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);

                //int PayBillNo = EmpId;
                //  string employeeId = Common.GetAdhocStaffEmployeeId(EmpId);
                string employeeId = EmpId;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                ViewBag.Finyr = Common.GetAllFinancial();
                model.FinancialYear = FinYear;
                model.ItList = payment.GetITEmpDeclarations(employeeId, FinYear);
                model.ItSOP = payment.GetITEmpSOP();
                model.ItOtherIncome = payment.GetITEmpOtherIncome(employeeId, FinYear);
                model.EmpInfo = StaffPaymentService.GetEmployeeDetails(employeeId);
                model.SalaryDet = payment.GetEmployeesSalaryDetails(employeeId, FinYear);
                model.SupplemSalaryDet = payment.GetEmployeesSupplymentarySalaryDetails(employeeId, FinYear);

                return View(model);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: List
        [Authorize]
        [HttpPost]
        public ActionResult List(EmpITDeductionModel model, FormCollection formCollection)
        {
            try
            {
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                string msg = validate(model);
                //int PayBillNo = model.EmpInfo.EmployeeID;
                int Finyear = Convert.ToInt32(model.FinancialYear);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                ViewBag.Finyr = Common.GetAllFinancial();
                if (msg != "")
                {

                    model.errMsg = msg;
                    ModelState.Remove("errMsg");
                    ModelState.AddModelError("", msg);
                    ViewBag.Errors = msg;
                    return View(model);
                }
                var btnSave = Request["btnSave"];
                // string employeeId = Common.GetAdhocStaffEmployeeId(model.EmpInfo.ID);
                string employeeId = model.EmpInfo.EmployeeID;
                model.EmpInfo.EmployeeID = employeeId;
                msg = payment.ITEmpDeclarationIU(model, Finyear);

                model.ItList = payment.GetITEmpDeclarations(employeeId, Finyear);
                model.ItSOP = payment.GetITEmpSOP();
                model.ItOtherIncome = payment.GetITEmpOtherIncome(employeeId, Finyear);
                model.EmpInfo = StaffPaymentService.GetEmployeeDetails(employeeId);
                model.SalaryDet = payment.GetEmployeesSalaryDetails(employeeId, Finyear);
                model.SupplemSalaryDet = payment.GetEmployeesSupplymentarySalaryDetails(employeeId, Finyear);
                ModelState.Clear();
                return View(model);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize]
        public JsonResult GetSOPDeduction()
        {
            try
            {
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                EmpITDeductionModel model = new EmpITDeductionModel();
                model.ItList = payment.GetITEmpDeclarations("",0);
                model.ItSOP = payment.GetITEmpSOP();
                model.ItOtherIncome = payment.GetITEmpOtherIncome("",0);
                model.EmpInfo = adhoc.GetEmployeeByEmpId(0);
                //model.CurrentPage = page;
                //model.pageSize = pageSize;
                //model.visiblePages = 5;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult GetOtherIncomeDeduction()
        {
            try
            {
                int pageSize = 10;
                int page = 1;
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);

                var model = payment.GetITEmpDeclarations("",0);
                //model.CurrentPage = page;
                //model.pageSize = pageSize;
                //model.visiblePages = 5;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        private string validate(EmpITDeductionModel model)
        {
            try
            {
                string msg = "";

                foreach (var item in model.ItList)
                {
                    if (item.MaxLimit != 0 && item.MaxLimit < item.Amount)
                    {
                        msg = item.SectionName + " exceeds max limit " + item.Amount;
                        return msg;
                    }
                }

                return msg;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        [HttpGet]
        public JsonResult LoadAdhocStaffList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteAdhocStaffWithDetails(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetAutoCompleteAdhocStaffWitheEmpDetails(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteAdhocStaffWitheEmpDetails(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult _Declaration(string empNo)
        {
            ITDeclarationViewModel model = new ITDeclarationViewModel();
            try
            {
                model = payment.GetDeclarationDetails(empNo);
                ViewBag.Remarks = payment.GetDeclarationRemarks(empNo);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView(model);
            }
        }
    }
}