using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IOAS.GenericServices.Process;
using IOAS.Models.Process;
using IOAS.Filter;
using IOAS.Infrastructure;

namespace IOAS.Controllers
{
    [Authorized]
    public class ProcessGuidelineController : Controller
    {
        
        [HttpGet]
        public ActionResult ProcessGuideline()
        {
            try
            {
                return View();
            }
            catch
            {
                return Json("Error:ProcessGuideline", JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpGet]
        public JsonResult LoadControls()
        {
            try
            {
                object output = ProcessGuidelineBO.LoadControls();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error:LoadControls", JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpGet]
        public JsonResult GetProcessFlowList(int processGuidelineId)
        {
            try
            {
                object output = ProcessGuidelineBO.GetProcessFlowList(processGuidelineId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json("Error:GetProcessFlowList"+ex.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpPost]
        public ActionResult AddProcessFlow(ProcessFlowModel model)
        {
            try
            {
                var Username = User.Identity.Name;
                model.UserId = Common.GetUserid(Username);
                int value = ProcessGuidelineBO.AddProcessFlow(model);
                return Json(new { result = value });
            }
            catch
            {
                return Json("Error:AddProcessFlow", JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpPost]
        public ActionResult UpdateProcessFlow(ProcessFlowModel model)
        {
            try
            {
                var Username = User.Identity.Name;
                model.UserId = Common.GetUserid(Username);
                int value = ProcessGuidelineBO.UpdateProcessFlow(model);
                return Json(new { result = value });
            }
            catch
            {
                return Json("Error:UpdateProcessFlow", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteProcessFlow(int processGuidelineDetailId)
        {
            try
            {
                object output = ProcessGuidelineBO.DeleteProcessFlow(processGuidelineDetailId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error:DeleteProcessFlow", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetProcessFlowUserDetails(int processGuidelineDetailId)
        {
            try
            {
                object output = ProcessGuidelineBO.GetProcessFlowUserDetails(processGuidelineDetailId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error:GetProcessFlowUserDetails", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetUserCounts(int pgDetailId)
        {
            try
            {
                object output = ProcessGuidelineBO.GetUserCount(pgDetailId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json("Error:GetProcessFlowUserDetails", JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpGet]
        public JsonResult GetApproverList()
        {
            try
            {
                object output = ProcessGuidelineBO.GetApproverList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error:GetApproverList", JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpGet]
        public JsonResult GetStatus()
        {
            try
            {
                object output = ProcessGuidelineBO.GetStatus();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error:GetStatus", JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpPost]
        public ActionResult AddApproverDetails(ProcessFlowApproverList model)
        {
            try
            {

                int value = ProcessGuidelineBO.AddApproverDetails(model);
                return Json(new { result = value });
            }
            catch
            {
                return Json("Error:AddApproverDetails", JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpGet]
        public ActionResult GetAllApproverList(int processheaderid, int processDetailId)
        {
            try
            {

                object output = ProcessGuidelineBO.GetAllApproverList(processheaderid, processDetailId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error:GetAllApproverList", JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public ActionResult DeletePGLWorkflow(int processguidlineworkflowId)
        {
            try
            {
                object output = ProcessGuidelineBO.DeletePGLWorkflow(processguidlineworkflowId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error:DeletePGLWorkflow", JsonRequestBehavior.AllowGet);
            }
        }


        
        [HttpPost]
        public ActionResult InsertProcessGuideline(ProcessGuideline model)
        {
            try
            {
                int value = ProcessGuidelineBO.InsertProcessGuideline(model);
                return Json(new { result = value });

            }
            catch
            {
                return Json("Error:InsertProcessGuideline", JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpGet]
        public ActionResult GetProcessGuideLineList(int functionId, string processName)
        {
            try
            {
                object output = ProcessGuidelineBO.GetProcessGuideLineList(functionId, processName);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error:GetProcessGuideLineList", JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpPost]
        public ActionResult MapProcessflowUser(List<ProcessFlowUser> selectedUser)
        {
            try
            {
                int value = ProcessGuidelineBO.MapProcessflowUser(selectedUser);
                return Json(new { result = value });
            }
            catch
            {
                return Json("Error:MapProcessflowUser", JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpPost]
        public ActionResult UnmapProcessflowUser(List<ProcessFlowUser> selectedUser)
        {
            try
            {

                int value = ProcessGuidelineBO.UnmapProcessflowUser(selectedUser);
                return Json(new { result = value });
            }
            catch
            {
                return Json("Error:UnmapProcessflowUser", JsonRequestBehavior.AllowGet);
            }
        }

    }
}