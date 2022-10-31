using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IOAS.Models;
using IOAS.Infrastructure;
using IOAS.GenericServices;

namespace IOAS.Controllers
{
    public class ProjectFundingCategoryController : Controller
    {
        private static readonly Object lockObj = new Object();
        // GET: ProjectFunding
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateRO(int ProjectId = 0, int aprvdId = 0)
        {
            var roModel = new CreateROModel();
            var projModel = Common.GetProjectsDetails(ProjectId);
            roModel.NameofPI = Common.GetPIdetailsbyProject(ProjectId);//projModel.PIName;
            roModel.ProjTitle = projModel.ProjectTittle;
            roModel.SanctionValue = projModel.SancationValue;
            roModel.AgencyName = Common.GetAgencyByProjectId(ProjectId);
            roModel.ActualStartDate = String.Format("{0:dd}", (DateTime)projModel.SancationDate) + "-" + String.Format("{0:MMMM}", (DateTime)projModel.SancationDate) + "-" + String.Format("{0:yyyy}", (DateTime)projModel.SancationDate);
            roModel.ActualCloseDate = String.Format("{0:dd}", (DateTime)projModel.CloseDate) + "-" + String.Format("{0:MMMM}", (DateTime)projModel.CloseDate) + "-" + String.Format("{0:yyyy}", (DateTime)projModel.CloseDate);
            roModel.ProjId = ProjectId;
            roModel.TotalEditedValue = Common.GetTotEditedValue(ProjectId);
            roModel.TotalNewValue = Common.GetTotNewValue(ProjectId);
            roModel.ProjectNumber = Common.getprojectnumber(ProjectId);
            roModel.TempRODetails = Common.getTempRODetails(ProjectId, aprvdId);
            roModel.RODetails = Common.getRoDetails(ProjectId,aprvdId);
            
            return View(roModel);
        }


        [HttpPost]
        public ActionResult CreateRO(CreateROModel rOModel)
        {
            int userId = Common.GetUserid(User.Identity.Name);
            ProjectFundingCategoryService pfcs = new ProjectFundingCategoryService();

            if (ModelState.IsValid)
            {
                var RODetailValidation = validateRODetails(rOModel);
                if (RODetailValidation != "valid")
                {
                    TempData["errMsg"] = RODetailValidation;
                    return View(rOModel);
                }
            }
            var RoCreation = pfcs.CreateRO(rOModel, userId);

            return RedirectToAction("ROList", "ProjectFundingCategory");
        }

        public string validateRODetails(CreateROModel model)
        {
            string msg = "valid";

            if (model.RODetails != null)
            {
                var emptyRONumber = model.RODetails.All(x => string.IsNullOrWhiteSpace(x.RONumber));
                if (emptyRONumber)
                    return msg = "RO Number should not be empty!";

                var emptyEditedVal = model.RODetails.Any(x => x.EditedValue.HasValue);
                if (!emptyEditedVal)
                    return msg = "Edited Value should not be empty!";

                /*To validate Duplicate RO Number */
                    
                var query = model.RODetails.GroupBy(x => x.RONumber).SelectMany(a => a.Skip(1)).Distinct().ToList();
                if (query.Count > 0)
                {
                    return msg = "RO Number already exist!";
                }

                var ROValue = model.RODetails.Any(x => x.EditedValue >= model.SanctionValue);
                if (ROValue)
                {
                    return msg = "RO Value should not exceed Sanctioned value";
                }
            }
            //var NewROValue = model.RODetails.Select(b => b.RO_Id,)
            return msg;
        }

       [HttpGet]
        public ActionResult ROList()
        {
            return View();

        }
        [HttpPost]
        public JsonResult GetROList(RODetailSearch model, int pageIndex, int pageSize)
        {
            object output = ProjectFundingCategoryService.GetROList(model, pageIndex, pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ROList(CreateROModel model)
        {
            return View(model);

        }

        public ActionResult ViewRODetails(int ProjectId=0,int aprvdId = 0,bool Pfinit = false)
        {
            CreateROModel roModel = new CreateROModel();
            var projModel = Common.GetProjectsDetails(ProjectId);
            roModel.NameofPI = Common.GetPIdetailsbyProject(ProjectId);//projModel.PIName;
            roModel.ProjTitle = projModel.ProjectTittle;
            roModel.SanctionValue = projModel.SancationValue;
            roModel.AgencyName = Common.GetAgencyByProjectId(ProjectId);
            roModel.ActualStartDate = String.Format("{0:dd}", (DateTime)projModel.SancationDate) + "-" + String.Format("{0:MMMM}", (DateTime)projModel.SancationDate) + "-" + String.Format("{0:yyyy}", (DateTime)projModel.SancationDate);
            roModel.ActualCloseDate = String.Format("{0:dd}", (DateTime)projModel.CloseDate) + "-" + String.Format("{0:MMMM}", (DateTime)projModel.CloseDate) + "-" + String.Format("{0:yyyy}", (DateTime)projModel.CloseDate);
            roModel.ProjId = ProjectId;
            //roModel.TotalEditedValue = Common.GetTotEditedValue(ProjectId);
            roModel.TotalNewValue = Common.GetTotNewValue(ProjectId);
            roModel.ProjectNumber = Common.getprojectnumber(ProjectId);
            roModel.RODetails = Common.getRoDetails(ProjectId, aprvdId);

            /*Process flow*/
            ViewBag.processGuideLineId = 349;
            roModel.PFInit = Pfinit;
            roModel.ROAprvId = aprvdId;
            return View(roModel);

        }
        /*From respective js*/
        [HttpPost]
        public ActionResult ProjectROWFInit(int AprvdId, int projId)
        {
            try
            {
                lock (lockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    ProjectFundingCategoryService pfcs = new ProjectFundingCategoryService();
                    bool status = pfcs.ProjectROWFInit(AprvdId, projId,userId);
                    return Json(status, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}

          