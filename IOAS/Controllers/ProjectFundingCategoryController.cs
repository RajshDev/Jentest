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
        // GET: ProjectFunding
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateRO(int ProjectId = 0, int ROId = 0)
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
            var pfcs = new ProjectFundingCategoryService();
            roModel.TempRODetails = Common.getTempRODetails(ProjectId, ROId);
            roModel.RODetails = Common.getRoDetails(ProjectId,ROId);
            
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
                if (model.TotalNewValue >= model.SanctionValue)
                {
                    return msg = "TotalRo value not exceed Sanctioned value";
                }
                if (model.TotalEditedValue >= model.SanctionValue)
                {
                    return msg = "Ro TotalEdited value not exceed Sanctioned value";
                }
                var ROValue = model.RODetails.Any(x => x.EditedValue >= model.SanctionValue);
                if (ROValue)
                {
                    return msg = "Ro Edited Value should not exceed Sanctioned value";
                }
                var RoValue1 = model.RODetails.Any(x => x.NewValue >= model.SanctionValue);
                if (RoValue1)
                {
                    return msg = " RO new Value should not exceed Sanctioned value";
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
        public JsonResult GetROList(RODetailSearch model, DateFilterModel PrsntDueDate, int pageIndex, int pageSize)
        {
            object output = ProjectFundingCategoryService.GetROList(model, PrsntDueDate, pageIndex, pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ROList(CreateROModel model)
        {
            return View(model);

        }

        public ActionResult ViewRODetails(int ProjectId,int ROId)
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
            
            roModel.RODetails = Common.getRoDetails(ProjectId, ROId);
            return View(roModel);

        }
    }
}

          