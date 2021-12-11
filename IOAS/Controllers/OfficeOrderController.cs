using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IOAS.Models;
using IOAS.GenericServices;

namespace IOAS.Controllers
{
    public class OfficeOrderController : Controller
    {
        StaffPaymentService payment = new StaffPaymentService();
        OfficeOrderService order = new OfficeOrderService();
        FinOp fo = new FinOp(System.DateTime.Now);

        // GET: OfficeOrder
        public ActionResult Index()
        {
            return View();
        }


        // GET: List
        [Authorize]
        public ActionResult List()
        {
            try
            {
                DateTime Today = System.DateTime.Now;
                int pageSize = 10;
                int page = 1;
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);

                var model = order.GetOfficeOrders(userId, page, pageSize);
                model.CurrentPage = page;
                model.pageSize = pageSize;
                model.visiblePages = 5;
                ViewBag.months = fo.GetAllMonths(Today.Year);
                ViewBag.PaymentType = payment.GetPaymentType();
                return View(model);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult AddEntry()
        {
            try
            {
                DateTime Today = System.DateTime.Now;
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                var model = new OfficeOrderModel();
                model.OrderDate = DateTime.Now;
                model.SalaryHead = order.GetEmployeeSalaryHead();
                model.EmpInfo = payment.GetEmpInfo(model.EmployeeId);
                ViewBag.EmpList = order.GetEmployeeList();
                ViewBag.OrderType = order.GetStatusType("OfficeOrderType");
                return View("Add",model);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddEntry(OfficeOrderModel model, FormCollection formCollection)
        {
            try
            {
                var button = Request["button"];

                DateTime Today = System.DateTime.Now;
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                string result = "";
                if (button == "Submit")
                {
                    result = order.OfficeOrderIU(model);
                }
                
                model.EmpInfo = payment.GetEmpInfo(model.EmployeeId);
                model.SalaryHead = order.GetEmployeeSalaryHead();
                ViewBag.EmpList = order.GetEmployeeList();
                ViewBag.EmpInfo = payment.GetEmpInfo(model.EmpInfo.EmployeeID);
                ViewBag.OrderType = order.GetStatusType("OfficeOrderType");

                return View("Add", model);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditEntry(OfficeOrderModel model, FormCollection formCollection)
        {
            try
            {
                DateTime Today = System.DateTime.Now;
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                var result = order.OfficeOrderIU(model);
                ViewBag.months = fo.GetAllMonths(Today.Year);
                ViewBag.PaymentType = payment.GetPaymentType();
                return View(model);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}