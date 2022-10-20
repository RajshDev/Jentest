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

        public ActionResult CreateRO(int ProjectId = 0)
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
            roModel.ProjectNumber = Common.getprojectnumber(ProjectId);
            var pfcs = new ProjectFundingCategoryService();
            roModel.RODetails = pfcs.GetRoDetails(ProjectId);
            return View(roModel);
        }

        [HttpPost]
        public ActionResult CreateRO(CreateROModel rOModel)
        {
            int userId = Common.GetUserid(User.Identity.Name);
            ProjectFundingCategoryService pfcs = new ProjectFundingCategoryService();
            var RoCreation = pfcs.CreateRO(rOModel, userId);

            return View();
        }
    }
}

          