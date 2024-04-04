using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using IOAS.Models;
using IOAS.GenericServices;
using System.Collections.Generic;
using System.Web.Security;
using IOAS.Infrastructure;
using System.IO;
using IOAS.Filter;

namespace IOAS.Controllers
{
    [Authorized]
    public class ProjectController : Controller
    {
        // Creation of Project (Project Opening)
        private static readonly Object lockObj = new Object();
        public ActionResult ProjectOpening(int pId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1)
                //    return RedirectToAction("Index", "Home");
                CreateProjectModel model = new CreateProjectModel();
                var country = Common.getCountryList();
                var department = Common.getDepartment();
                var gender = Common.getGender();
                var PI = Common.GetProjectPIWithDetails();
                var Doctype = Common.GetDocTypeList(9);
                var scheme = Common.getschemes();
                var agency = Common.getagency();
                var projecttype = Common.getprojecttype();
                var proposalnumber = Common.getproposalnumber();
                var ministry = Common.getMinistry();
                var projectsponsubtype = Common.getsponprojectsubtype();
                var projectconssubtype = Common.getconsprojectsubtype();
                var categoryofproject = Common.getcategoryofproject();
                var Cadre = Common.getFacultyCadre();
                var fundingcategory = Common.getconsfundingcategory();
                var allocatehead = Common.getallocationhead();
                var taxservice = Common.gettaxservice();
                var internalfundingagency = Common.getinternalfundingagency();
                var staffcategory = Common.getstaffcategory();
                var fundingtype = Common.getfundingtype();
                var fundedby = Common.getfundedby();
                var indfundinggovtbody = Common.getindfundinggovtbody();
                var indfundingnongovtbody = Common.getindfundingnongovtbody();
                var forgnfundinggovtbody = Common.getforgnfundinggovtbody();
                var forgnfundingnongovtbody = Common.getforgnfundingnongovtbody();
                var typeofproject = Common.gettypeofproject();
                var sponprojectcategory = Common.getsponprojectcategory();
                var constaxtype = Common.getconstaxtype();
                var taxregstatus = Common.gettaxregstatus();
                //var projectfundingcategory = Common.getprojectfunding();
                ViewBag.FacultyCodeList = Common.GetCodeControlList("ProjectFacultyCode", string.Empty, true);
                ViewBag.CentreList = Common.GetCommonHeadList(2, 2);
                ViewBag.LabList = Common.GetCommonHeadList(2, 4);
                ViewBag.CorList = Common.GetCommonHeadList(2, 5);
                ViewBag.Currency = Common.getCurrency(true);
                ViewBag.projectcategory = Common.getprojectcategory();
                ViewBag.schemeCodeList = Common.GetSponsoredSchemeCodeList();
                ViewBag.ProjectList = new List<MasterlistviewModel>();
                ViewBag.consScheme = AccountService.getcategory(2);
                //ViewBag.finYearList = Common.GetFinYearList();
                ViewBag.country = country;
                ViewBag.deprtmnt = department;
                ViewBag.gender = gender;
                ViewBag.Docmenttype = Doctype;
                ViewBag.PI = PI;
                ViewBag.SubheadList = new List<MasterlistviewModel>();
                ViewBag.Scheme = AccountService.getcategory(1);
                ViewBag.Agency = agency;
                ViewBag.projecttype = projecttype;
                ViewBag.proposalnumber = proposalnumber;
                ViewBag.ministry = ministry;
                ViewBag.projectsponsubtype = projectsponsubtype;
                ViewBag.projectconssubtype = projectconssubtype;
                ViewBag.categoryofproject = categoryofproject;
                ViewBag.Cadre = Cadre;
                ViewBag.fundingcategory = fundingcategory;
                ViewBag.allocatehead = allocatehead;
                ViewBag.taxservice = taxservice;
                ViewBag.internalfundingagency = internalfundingagency;
                ViewBag.staffcategory = staffcategory;
                ViewBag.fundingtype = fundingtype;
                ViewBag.fundedby = fundedby;
                ViewBag.fundingtypeWOBoth = Common.GetFundingTypeWOBoth();
                ViewBag.sponprojectcategory = sponprojectcategory;
                ViewBag.indfundinggovtbody = indfundinggovtbody;
                ViewBag.indfundingnongovtbody = indfundingnongovtbody;
                ViewBag.forgnfundinggovtbody = forgnfundinggovtbody;
                ViewBag.forgnfundingnongovtbody = forgnfundingnongovtbody;
                ViewBag.typeofproject = typeofproject;
                ViewBag.constaxtype = constaxtype;
                ViewBag.taxregstatus = taxregstatus;
                ViewBag.Projectdomain = Common.GetCodeControlList("Domainrelevantproject");
                ViewBag.SponPrjFunType = Common.GetSponsoredTypeCategory();
                ViewBag.prjClassification = Common.GetProjectClassification();
                ViewBag.rptClassification = Common.GetReportClassification();
                //model.Inputdate = DateTime.Now;
                ViewBag.ProjectFundingCategory = Common.getprojectfunding();
                ViewBag.BankID = Common.GetTSABankList();
                //ViewBag.Msg = System.Web.HttpContext.Current.Request.ApplicationPath;
                // model.ProposalID = 0;
                return View(model);

            }
            catch (Exception ex)
            {

                return View();
            }

        }

        [HttpPost]
        public ActionResult ProjectOpening(CreateProjectModel model, HttpPostedFileBase[] file, HttpPostedFileBase taxprooffile)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                var data = Common.getUserIdAndRole(user_logged_in);
                int logged_in_userid = data.Item1;
                int user_role = data.Item2;
                //if (user_role != 1)
                //    return RedirectToAction("Index", "Home");
                //var username = User.Identity.Name;
                //int userid = Common.GetUserid(username);
                var country = Common.getCountryList();
                var department = Common.getDepartment();
                var gender = Common.getGender();
                var PI = Common.GetProjectPIWithDetails();
                var Doctype = Common.GetDocTypeList(9);
                var scheme = Common.getschemes();
                var agency = Common.getagency();
                var projecttype = Common.getprojecttype();
                var proposalnumber = Common.getproposalnumber();
                var ministry = Common.getMinistry();
                var projectsponsubtype = Common.getsponprojectsubtype();
                var projectconssubtype = Common.getconsprojectsubtype();
                var categoryofproject = Common.getcategoryofproject();
                var Cadre = Common.getFacultyCadre();
                var fundingcategory = Common.getconsfundingcategory();
                var allocatehead = Common.getallocationhead();
                var taxservice = Common.gettaxservice();
                var internalfundingagency = Common.getinternalfundingagency();
                var staffcategory = Common.getstaffcategory();
                var fundingtype = Common.getfundingtype();
                var fundedby = Common.getfundedby();
                var indfundinggovtbody = Common.getindfundinggovtbody();
                var indfundingnongovtbody = Common.getindfundingnongovtbody();
                var forgnfundinggovtbody = Common.getforgnfundinggovtbody();
                var forgnfundingnongovtbody = Common.getforgnfundingnongovtbody();
                var typeofproject = Common.gettypeofproject();
                var sponprojectcategory = Common.getsponprojectcategory();
                var constaxtype = Common.getconstaxtype();
                var taxregstatus = Common.gettaxregstatus();
                //var projectfundingcategory = Common.getprojectfunding();
                ViewBag.Currency = Common.getCurrency(true);
                ViewBag.projectcategory = Common.getprojectcategory();
                ViewBag.consScheme = AccountService.getcategory(2);
                ViewBag.schemeCodeList = Common.GetSponsoredSchemeCodeList();
                //ViewBag.finYearList = Common.GetFinYearList();
                ViewBag.ProjectList = Common.GetMainProjectNumberList(model.Prjcttype ?? 0);
                ViewBag.FacultyCodeList = Common.GetCodeControlList("ProjectFacultyCode", string.Empty, true);
                ViewBag.CentreList = Common.GetCommonHeadList(2, 2);
                ViewBag.LabList = Common.GetCommonHeadList(2, 4);
                ViewBag.CorList = Common.GetCommonHeadList(2, 5);
                ViewBag.country = country;
                ViewBag.deprtmnt = department;
                ViewBag.gender = gender;
                ViewBag.Docmenttype = Doctype;
                ViewBag.PI = PI;
                ViewBag.Scheme = AccountService.getcategory(1);
                ViewBag.Agency = agency;
                ViewBag.projecttype = projecttype;
                ViewBag.SubheadList = new List<MasterlistviewModel>();
                ViewBag.proposalnumber = proposalnumber;
                ViewBag.ministry = ministry;
                ViewBag.projectsponsubtype = projectsponsubtype;
                ViewBag.projectconssubtype = projectconssubtype;
                ViewBag.categoryofproject = categoryofproject;
                ViewBag.Cadre = Cadre;
                ViewBag.fundingcategory = fundingcategory;
                ViewBag.allocatehead = allocatehead;
                ViewBag.taxservice = taxservice;
                ViewBag.internalfundingagency = internalfundingagency;
                ViewBag.staffcategory = staffcategory;
                ViewBag.fundingtype = fundingtype;
                ViewBag.fundingtypeWOBoth = Common.GetFundingTypeWOBoth();
                ViewBag.fundedby = fundedby;
                ViewBag.sponprojectcategory = sponprojectcategory;
                ViewBag.indfundinggovtbody = indfundinggovtbody;
                ViewBag.indfundingnongovtbody = indfundingnongovtbody;
                ViewBag.forgnfundinggovtbody = forgnfundinggovtbody;
                ViewBag.forgnfundingnongovtbody = forgnfundingnongovtbody;
                ViewBag.typeofproject = typeofproject;
                ViewBag.constaxtype = constaxtype;
                ViewBag.taxregstatus = taxregstatus;
                ViewBag.Projectdomain = Common.GetCodeControlList("Domainrelevantproject");
                ViewBag.SponPrjFunType = Common.GetSponsoredTypeCategory();
                ViewBag.prjClassification = Common.GetProjectClassification();
                ViewBag.rptClassification = Common.GetReportClassification();
                ViewBag.ProjectFundingCategory = Common.getprojectfunding();
                ViewBag.BankID = Common.GetTSABankList();
                // model.PIEmail = model.PIEmail;

                var ForgnProjectFundingGovtBody = model.ForgnProjectFundingGovtBody_Qust_1;


                ViewBag.SelectedIds = ForgnProjectFundingGovtBody;
                if (ModelState.IsValid)
                {
                    Nullable<decimal> ttlAllowVal = 0, ttlEMIVal = 0;
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                    if (model.IsYearWiseAllocation)
                    {
                        foreach (var item in model.YearWiseHead)
                        {
                            Nullable<decimal> ttlEMIValOfYear = item.EMIValue != null ? item.EMIValue.Sum() : 0;
                            ttlAllowVal = ttlAllowVal + item.AllocationValueYW.Sum();
                            ttlEMIVal = ttlEMIVal + ttlEMIValOfYear;
                            var groupsVal = item.AllocationHeadYW.GroupBy(v => v);
                            if (item.AllocationHeadYW.Length != groupsVal.Count())
                            {
                                ViewBag.errMsg = "Duplicate allocation head exist in year " + item.Year + ". Please select a different allocation.";
                                return View(model);
                            }
                            if (ttlEMIValOfYear > 0 && ttlEMIValOfYear != item.EMIValueForYear)
                            {
                                ViewBag.errMsg = "Sum of No of Installment values is different from total installment value.";
                                return View(model);
                            }
                        }
                    }
                    else if (model.IsSubProject)
                    {
                        var pVals = Common.GetMainAndSubProjectValues(model.MainProjectId ?? 0, model.ProjectID ?? 0);
                        if (pVals.Item1 < (pVals.Item2 + model.BaseValue))
                        {
                            ViewBag.errMsg = "The total sanctioned value of sub projects exceeds the total sanctioned value of the main project.";
                            return View(model);
                        }
                    }
                    else if (model.TaxException && taxprooffile == null && String.IsNullOrEmpty(model.Docpathfornotax))
                    {
                        ViewBag.errMsg = "Tax exception proof field is required.";
                        return View(model);
                    }
                    else
                    {
                        ttlAllowVal = model.Allocationvalue.Sum();
                        var groupsVal = model.Allocationhead.GroupBy(v => v);
                        if (model.Allocationhead.Length != groupsVal.Count())
                        {
                            ViewBag.errMsg = "Duplicate allocation head exist. Please select a different allocation.";
                            return View(model);
                        }
                        ttlEMIVal = model.ArrayEMIValue != null ? model.ArrayEMIValue.Sum() : 0;
                    }
                    if (ttlEMIVal > 0 && ttlEMIVal != model.BaseValue)
                    {
                        ViewBag.errMsg = "Overall installment values is different from project value.";
                        return View(model);
                    }
                    else if (model.IsYearWiseAllocation && ttlAllowVal != model.BaseValue)
                    {
                        ViewBag.errMsg = "Overall allocation values is different from project value.";
                        return View(model);
                    }
                    if (model.CoPIname != null && model.CoPIname[0] != 0)
                    {
                        var groupsVal = model.CoPIname.GroupBy(v => v);
                        if (model.CoPIname.Length != groupsVal.Count())
                        {
                            ViewBag.errMsg = "Some of the Co PI are duplicated. Please rectify and submit again.";
                            return View(model);
                        }
                        else if (model.CoPIname.Contains(model.PIname ?? 0))
                        {
                            ViewBag.errMsg = "Co PI should not be the PI what you have declared. Please rectify and submit again.";
                            return View(model);
                        }
                    }

                    if (taxprooffile != null)
                    {
                        string taxprooffilename = Path.GetFileName(taxprooffile.FileName);
                        var docextension = Path.GetExtension(taxprooffilename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            ViewBag.errMsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    for (int i = 0; i < model.DocType.Length; i++)
                    {
                        if (file[i] != null)
                        {
                            string docname = Path.GetFileName(file[i].FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                ViewBag.errMsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }

                    }
                    ProjectService _ps = new ProjectService();
                    model.ProjectcrtdID = logged_in_userid;

                    var projectid = _ps.ProjectOpening(model, file, taxprooffile);

                    if ((model.ProjectID == 0 || model.ProjectID == null) && projectid > 0)
                    {
                        var projectnumber = Common.getprojectnumber(projectid);
                        ViewBag.succMsg = "Project has been opened successfully with Project number - " + projectnumber + ".";
                        return View(model);
                    }
                    if (model.ProjectID > 0 && projectid > 0)
                    {
                        var projectnumber = Common.getprojectnumber(projectid);
                        ViewBag.succMsg = "Project - " + projectnumber + " updated successfully.";
                        return View(model);
                    }
                    else if (projectid == 0)
                    {
                        ViewBag.errMsg = "Project " + model.Projecttitle + "Already Exists";
                    }
                    else
                        ViewBag.errMsg = "Something went wrong please contact administrator";

                }
                else
                {
                    string messages = string.Join("", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    ViewBag.errMsg = messages;
                }
                //  return RedirectToAction("ProjectOpening", "Project");
                return View(model);
            }
            catch (Exception ex)
            {

                ViewBag.errMsg = "Something went wrong please contact administrator";
                return View(model);
            }

        }

        [Authorize]
        public ActionResult ProjectEnhancement(int pId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1)
                //    return RedirectToAction("Index", "Home");
                ProjectEnhancementModel model = new ProjectEnhancementModel();
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();
                var allocatehead = Common.getallocationhead();
                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;
                ViewBag.allocatehead = allocatehead;
                //ViewBag.deprtmnt = Common.getDepartment();
                //ViewBag.PI = Common.GetProjectPIWithDetails();
                ViewBag.Cadre = Common.getFacultyCadre();
                //ViewBag.FreezeListForAdd = ProjectService.EditProject();
                return View(model);

            }
            catch (Exception ex)
            {

                return View();
            }

        }
        public ActionResult ProjectPatternChange()
        {

            return View();
        }
        //public JsonResult MaintenanceStatusChanger(string StatusChanger, string Depmess)
        //{
        //    var empty = new ProjectStatusUpdateModel();
        //    ProjectStatusUpdateModel data = new ProjectStatusUpdateModel();
        //    if (Depmess != "")
        //    {
        //        int userId = Common.GetUserid(User.Identity.Name);
        //        data.Message = CoreAccountsService.UpdateMaintenanceStatus(StatusChanger, Depmess, userId) == true ? "Success" : "Failed";
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }

        //    else
        //    {
        //        data = empty;
        //        return Json(data, JsonRequestBehavior.AllowGet);

        //    }

        //}


        public JsonResult UpdateProjectNumber(string OldProjectNum="", string NewProjectNum="")
        {
            var empty = new ProjectPatternChange();
            ProjectPatternChange data = new ProjectPatternChange();

            if (OldProjectNum != null || OldProjectNum == "")
            {
                data.Message = CoreAccountsService.ChangeProjectNumber(OldProjectNum, NewProjectNum) == true ? "Success" : "Failed";
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
                data = empty;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetFreezeAndAllocationData(int ProjectId, int AllocationId)
        {
            try
            {
                var data = ProjectService.GetFreezeAndAllocationValues(ProjectId, AllocationId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetFreezeOverHeads(int ProjectId)
        {
            try
            {
                var data = ProjectService.GetFreezeOverHeadsValues(ProjectId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetFreezeProjectData(int ProjectId)
        {
            try
            {
                var data = ProjectService.GetFreezeprojectDataValues(ProjectId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult ProjectEnhancement(ProjectEnhancementModel model, HttpPostedFileBase file)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                var data = Common.getUserIdAndRole(user_logged_in);
                int logged_in_userid = data.Item1;
                int user_role = data.Item2;
                //if (user_role != 1)
                //    return RedirectToAction("Index", "Home");
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();
                var allocatehead = Common.getallocationhead();

                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;
                ViewBag.allocatehead = allocatehead;
                //ViewBag.deprtmnt = Common.getDepartment();
                //ViewBag.PI = Common.GetProjectPIWithDetails();
                ViewBag.Cadre = Common.getFacultyCadre();
                if (ModelState.IsValid)
                {
                    if (model.Enhancement_Qust_1 == "No" && model.Extension_Qust_1 == "No")
                    {
                        ViewBag.errMsg = "At least do anyone of these actions enhancement or extension.";
                        return View(model);
                    }
                    if (model.Enhancement_Qust_1 == "Yes" && model.EnhancedAllocationvalue.Sum() != model.EnhancedSanctionValue && model.Allochead[0] != 0)
                    {
                        ViewBag.errMsg = "The enhanced sanction value is not equal to enhanced allocation value. Please check the values.";
                        return View(model);
                    }
                    //string errMsg = Common.ValidateEnhancementAndExtension(model);
                    //if (errMsg != "Valid")
                    //{
                    //    ViewBag.errMsg = errMsg;
                    //    return View(model);
                    //}
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };

                    if (file != null)
                    {
                        string docname = Path.GetFileName(file.FileName);
                        var docextension = Path.GetExtension(docname);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            ViewBag.errMsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    if (model.Allocationhead != null && model.Allocationhead[0] != 0)
                    {
                        var groupsVal = model.Allocationhead.GroupBy(v => v);
                        if (model.Allocationhead.Length != groupsVal.Count())
                        {
                            ViewBag.errMsg = "Some of the Allocation heads are duplicated. Please rectify and submit again.";
                            return View(model);
                        }
                    }
                    if (model.Allochead != null && model.Allochead[0] != 0)
                    {
                        var groupsVal = model.Allochead.GroupBy(v => v);
                        if (model.Allochead.Length != groupsVal.Count())
                        {
                            ViewBag.errMsg = "Some of the Allocation heads are duplicated. Please rectify and submit again.";
                            return View(model);
                        }
                    }

                    ProjectService _ps = new ProjectService();
                    model.CrtdUserid = logged_in_userid;

                    var projectid = _ps.ProjectEnhancement(model, file);
                    if (projectid > 0)
                    {
                        var projectnumber = Common.getprojectnumber(projectid);
                        if (model.Enhancement_Qust_1 == "Yes" && model.Extension_Qust_1 == "No")
                        {
                            ViewBag.succMsg = "Enhancement successfully done for Project - " + projectnumber + ".";
                            return View(model);
                        }

                        if (model.Extension_Qust_1 == "Yes" && model.Enhancement_Qust_1 == "No")
                        {
                            ViewBag.succMsg = "Extension successfully done for Project - " + projectnumber + ".";
                            return View(model);
                        }
                        if (model.Extension_Qust_1 == "Yes" && model.Enhancement_Qust_1 == "Yes")
                        {
                            ViewBag.succMsg = "Enhancement and Extension successfully done for Project - " + projectnumber + ".";
                            return View(model);
                        }
                    }
                    else
                        ViewBag.errMsg = "Something went wrong please contact administrator";


                }
                else
                {
                    string messages = string.Join("", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    ViewBag.errMsg = messages;
                }
                //  return RedirectToAction("ProjectOpening", "Project");
                return View(model);
            }
            catch (Exception ex)
            {

                ViewBag.errMsg = "Something went wrong please contact administrator";
                return View(model);
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult ProjectExtension(ProjectEnhancementModel model, HttpPostedFileBase file)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                var data = Common.getUserIdAndRole(user_logged_in);
                int logged_in_userid = data.Item1;
                int user_role = data.Item2;
                //if (user_role != 1)
                //    return RedirectToAction("Index", "Home");
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();

                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;

                if (ModelState.IsValid)
                {
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                    if ((model.AttachmentName != null && model.AttachmentName != ""))
                    {

                        if (file != null)
                        {
                            string docname = Path.GetFileName(file.FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                ModelState.AddModelError("", "Please upload any one of these type doc [.pdf, .doc, .docx]");
                                View(model);
                            }
                        }
                        ProjectService _ps = new ProjectService();
                        model.CrtdUserid = logged_in_userid;

                        var projectid = _ps.ProjectExtension(model, file);
                        if (projectid > 0)
                        {
                            var projectnumber = Common.getprojectnumber(projectid);
                            ViewBag.succMsg = "Extension successfully done for Project - " + projectnumber + ".";
                            return View(model);
                        }

                        else
                            ViewBag.errMsg = "Something went wrong please contact administrator";

                    }

                    else
                    {
                        ProjectService _ps = new ProjectService();
                        model.CrtdUserid = logged_in_userid;

                        var projectid = _ps.ProjectExtension(model, file);
                        if (projectid > 0)
                        {
                            var projectnumber = Common.getprojectnumber(projectid);
                            ViewBag.succMsg = "Extension successfully done for Project - " + projectnumber + ".";
                            return View(model);
                        }

                        else
                            ViewBag.errMsg = "Something went wrong please contact administrator";
                    }
                }
                else
                {
                    string messages = string.Join("", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    ViewBag.errMsg = messages;
                }

                return View(model);
            }
            catch (Exception ex)
            {

                ViewBag.errMsg = "Something went wrong please contact administrator";
                return View(model);
            }

        }
        [Authorize]
        public ActionResult ProjectExtension(int pId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1)
                //    return RedirectToAction("Index", "Home");
                ProjectEnhancementModel model = new ProjectEnhancementModel();
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();

                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;

                return View(model);

            }
            catch (Exception ex)
            {

                return View();
            }

        }
        //[Authorize]
        //[HttpPost]
        //public ActionResult ProjectExtension(ProjectEnhancementModel model)
        //{
        //    try
        //    {
        //        string user_logged_in = User.Identity.Name;
        //        var data = Common.getUserIdAndRole(user_logged_in);
        //        int logged_in_userid = data.Item1;
        //        int user_role = data.Item2;
        //        //if (user_role != 1)
        //        //    return RedirectToAction("Index", "Home");
        //        var Projecttitle = Common.GetProjecttitledetails();
        //        var projecttype = Common.getprojecttype();

        //        ViewBag.Project = Projecttitle;
        //        ViewBag.projecttype = projecttype;

        //        if (ModelState.IsValid)
        //        {
        //            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
        //            if ((model.AttachmentName != null && model.AttachmentName != ""))
        //            {

        //                if (model.file != null)
        //                {
        //                    string docname = Path.GetFileName(model.file.FileName);
        //                    var docextension = Path.GetExtension(docname);
        //                    if (!allowedExtensions.Contains(docextension))
        //                    {
        //                        ModelState.AddModelError("", "Please upload any one of these type doc [.pdf, .doc, .docx]");
        //                        View(model);
        //                    }
        //                }
        //                ProjectService _ps = new ProjectService();
        //                model.CrtdUserid = logged_in_userid;

        //                var projectid = _ps.ProjectExtension(model);
        //                if (projectid > 0)
        //                {
        //                    var projectnumber = Common.getprojectnumber(projectid);
        //                    ViewBag.succMsg = "Extension successfully done for Project - " + projectnumber + ".";
        //                    return View(model);
        //                }

        //                else
        //                    ViewBag.errMsg = "Something went wrong please contact administrator";

        //            }

        //            else
        //            {
        //                ProjectService _ps = new ProjectService();
        //                model.CrtdUserid = logged_in_userid;

        //                var projectid = _ps.ProjectExtension(model);
        //                if (projectid > 0)
        //                {
        //                    var projectnumber = Common.getprojectnumber(projectid);
        //                    ViewBag.succMsg = "Extension successfully done for Project - " + projectnumber + ".";
        //                    return View(model);
        //                }

        //                else
        //                    ViewBag.errMsg = "Something went wrong please contact administrator";
        //            }
        //        }
        //        else
        //        {
        //            string messages = string.Join("", ModelState.Values
        //                                .SelectMany(x => x.Errors)
        //                                .Select(x => x.ErrorMessage));

        //            ViewBag.errMsg = messages;
        //        }

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {

        //        ViewBag.errMsg = "Something went wrong please contact administrator";
        //        return View(model);
        //    }

        //}
        [HttpGet]
        public JsonResult GetProjectList()
        {
            object output = ProjectService.GetProjectList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSearchProposalList(string keyword)
        {
            object output = ProposalService.GetProposalDetails();
            return Json(output, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult EditProject(int ProjectId)
        {

            
            object output = ProjectService.EditProject(ProjectId);
            
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult DeleteProject(int ProjectId)
        {
            object output = ProjectService.DeleteProject(ProjectId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ShowDocument(string file, string filepath)
        {
            try
            {
                int roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1 && roleId != 3)
                //    return new EmptyResult();
                string fileType = Common.GetMimeType(Path.GetExtension(file));
                byte[] fileData = file.DownloadFile(Common.GetDirectoryName(filepath)); //file.GetFileData(Server.MapPath(filepath));

                Response.AddHeader("Content-Disposition", "inline; filename=\"" + file + "\"");
                return File(fileData, fileType);
            }
            catch (FileNotFoundException)
            {
                throw new HttpException(404, "File not found.");
            }
        }

        public ActionResult ShowDocumentLocalPath(string file, string filepath)
        {
            try
            {
                int roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1 && roleId != 3)
                //    return new EmptyResult();
                string fileType = Common.GetMimeType(Path.GetExtension(file));
                byte[] fileData = file.GetFileData(Server.MapPath(filepath));

                Response.AddHeader("Content-Disposition", "inline; filename=\"" + file + "\"");
                return File(fileData, fileType);
            }
            catch (FileNotFoundException)
            {
                throw new HttpException(404, "File not found.");
            }
        }


        [HttpPost]
        public JsonResult Loadproposaldetailsbyid(int ProposalId)
        {

            object output = ProjectService.getproposaldetails(ProposalId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProposalList()
        {
            object output = ProjectService.GetProposalDetails();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetProposalList(ProposalSearchModel model, int pageIndex, int pageSize)
        {
            object output = ProjectService.GetProposalDetails(model, pageIndex, pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Loadprojectdetailsbytype(string projecttype)
        {
            projecttype = projecttype == "" ? "0" : projecttype;
            var locationdata = ProjectService.LoadProjecttitledetails(Convert.ToInt32(projecttype));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadProjecttitlebybankid(string projecttype,int bankid)
        {
            projecttype = projecttype == "" ? "0" : projecttype;
            TempData["BankHeadId"] = bankid;
            var locationdata = ProjectService.LoadProjecttitlebybankid(Convert.ToInt32(projecttype),bankid);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Loadbankbyproject(int ProjectId)
        {           
            var locationdata = Common.Loadbankbyproject(ProjectId);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEnhancedProjectList(ProjectEnchancementSearch model, DateFilterModel PrsntDueDate, int pageIndex, int pageSize)
        {
            object output = ProjectService.GetEnhancedProjectList(model, PrsntDueDate, pageIndex, pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public JsonResult LoadProjectdetailsforenhance(int ProjectId)
        {

            object output = ProjectService.getprojectdetailsforenhance(ProjectId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize]
        public JsonResult DeleteEnhancement(int EnhanceId)
        {
            int userId = Common.GetUserid(User.Identity.Name);
            ProjectService ps = new ProjectService();
            bool output = ps.DeleteEnhamcement(EnhanceId, userId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [HttpPost]
        public JsonResult EditProjectenhancement(int EnhanceId)
        {
            object output = ProjectService.EditEnhancement(EnhanceId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetExtendedProjectList()
        {
            object output = ProjectService.GetExtendedProjectList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult LoadProjectdetailsforextend(int ProjectId)
        {

            object output = ProjectService.getprojectdetailsforextension(ProjectId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [HttpPost]
        public JsonResult EditProjectextension(int ExtensionId)
        {
            object output = ProjectService.EditExtension(ExtensionId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult LoadAgencycodebyagency(string agencyid)
        {
            object output = AccountService.Geteditagency(Convert.ToInt32(agencyid));//AccountService.getagencycode(Convert.ToInt32(agencyid));
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult CloseProject(int pId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1)
                //    return RedirectToAction("Index", "Home");
                ProjectClosingModel model = new ProjectClosingModel();
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();

                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;

                return View(model);

            }
            catch (Exception ex)
            {

                return View();
            }

        }
        [Authorize]
        [HttpPost]
        public ActionResult CloseProject(ProjectClosingModel model)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                var data = Common.getUserIdAndRole(user_logged_in);
                int logged_in_userid = data.Item1;
                int user_role = data.Item2;
                //if (user_role != 1)
                //    return RedirectToAction("Index", "Home");
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();

                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;

                if (ModelState.IsValid)
                {

                    ProjectService _ps = new ProjectService();
                    model.UpdtUserid = logged_in_userid;
                    model.Updt_TS = DateTime.Now;

                    var projectid = _ps.CloseProject(model);
                    if (projectid > 0)
                    {
                        var projectnumber = Common.getprojectnumber(projectid);
                        ViewBag.succMsg = "Project - " + projectnumber + "has been closed successfully.";
                        return View(model);
                    }

                    else
                        ViewBag.errMsg = "Something went wrong please contact administrator";
                }

                else
                {
                    string messages = string.Join("", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    ViewBag.errMsg = messages;
                }

                return View(model);
            }
            catch (Exception ex)
            {

                ViewBag.errMsg = "Something went wrong please contact administrator";
                return View(model);
            }

        }

        [HttpGet]
        public JsonResult GetClosedProjectList()
        {
            object output = ProjectService.GetClosedProjectList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProjectStatus()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetProjectDetails()
        {
            object output = ProjectService.GetProjectDetails();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadActionDDL()
        {
            try
            {
                object output = ProjectService.LoadControls();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error:LoadControls", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UpdateStatusDetails(UpdateProjectStatusModel model)
        {
            try
            {
                ProjectStatusModel newModel = new ProjectStatusModel();
                var user = User.Identity.Name;
                var UserId = Common.GetUserid(user);
                string docname = "";
                if (ModelState.IsValid)
                {
                    if (model.file != null)
                    {
                        docname = Path.GetFileName(model.file.FileName);
                        var fileId = Guid.NewGuid().ToString();
                        docname = fileId + "_" + docname;

                        model.file.UploadFile("OtherDocuments", docname);
                    }
                    int result = ProjectService.UpdateProjectDetails(model, UserId, docname);
                    if (result == 1)
                    {
                        ViewBag.SuccMsg = "Status has been updated successfully";
                    }
                    else
                    {
                        ViewBag.ErrMsg = "Something went wrong please contact administrator";
                    }

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    ViewBag.errMsg = messages;
                }
                return View("ProjectStatus", newModel);
            }
            catch (Exception ex)
            {
                ProjectStatusModel newModel = new ProjectStatusModel();
                ViewBag.ErrMsg = "Something went wrong please contact administrator";
                return View("ProjectStatus", newModel);
            }
        }

        [HttpPost]
        public ActionResult PopupUpdateStatus(int ProjectId, string StatusId)
        {
            try
            {
                UpdateProjectStatusModel model = new UpdateProjectStatusModel();
                model.ProjectID = ProjectId;
                model.StatusID = StatusId;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }

        //[HttpPost]
        //public JsonResult SearchProjectList(/*int ProjectType, string ProposalNumber, string FromSODate, string ToSOdate, DateTime Fromdate, DateTime Todate*/)
        //{
        //    object output = ProjectService.SearchProjectList(ProjectType, ProposalNumber, FromSODate, ToSOdate);
        //    //object output = "";
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}
        

        public ActionResult _ExtensionandEnhancementHistory(int Projectid)
        {
            ProjectService _ps = new ProjectService();
            ProjectEnhanceandExtenDetailsModel model = new ProjectEnhanceandExtenDetailsModel();
            model = _ps.GetEnhancementandExtensionDetails(Projectid);
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult SearchProjectList(int pageIndex, int pageSize, ProjectSearchFieldModel model, DateFilterModel PrpsalApprovedDate)
        {

            //ProjectSearchFieldModel model = new ProjectSearchFieldModel();

            object output = ProjectService.SearchProjectList(model, pageIndex, pageSize, PrpsalApprovedDate);
            //object output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewProject(int ProjectId, bool Pfinit = false)
        {
            try
            {
                ProjectViewDetailsModel model = new ProjectViewDetailsModel();
                ProjectService _PS = new ProjectService();
                model = _PS.getProjectViewDetails(ProjectId);
                model.PFInit = Pfinit;
                
                int pgId = 0;
                int sponCate = Convert.ToInt32(model.Projectcatgry_Qust_1);
                int pType = model.ProjectType ?? 0;
                decimal sanctionVal = model.Sanctionvalue ?? 0;
                if (sanctionVal < 10000000 && pType == 2)
                    pgId = 12;
                else if (sanctionVal >= 10000000 && sanctionVal < 50000000 && pType == 2)
                    pgId = 20;
                else if (sanctionVal < 10000000 && pType == 1 && sponCate == 1)
                    pgId = 19;
                else if (sanctionVal >= 10000000 && sanctionVal < 50000000 && pType == 1 && sponCate == 1)
                    pgId = 18;
                else if (sanctionVal >= 10000000 && sanctionVal < 50000000 && pType == 1 && sponCate == 2)
                    pgId = 14;
                else if (sanctionVal >= 50000000 && pType == 1 && sponCate == 2)
                    pgId = 203;
                else if (sanctionVal >= 50000000 && pType == 1 && sponCate == 1)
                    pgId = 204;
                else if (sanctionVal >= 50000000 && pType == 2)
                    pgId = 205;
                else
                    pgId = 13;
                ViewBag.processGuideLineId = pgId;
                return View(model);

            }
            catch (Exception ex)
            {

                return View();
            }
        }

        public ActionResult ViewProjectEnhancement(int EnhanceId, bool Pfinit = false)
        {
            var roleId = Common.GetRoleId(User.Identity.Name);
            //if (roleId != 1)
            //    return RedirectToAction("Index", "Home");
            ProjectEnhancementViewModel model = new ProjectEnhancementViewModel();
            model = ProjectService.ViewExtension(EnhanceId);
            model.PFInit = Pfinit;
            int pgId = 0;
            var data = Common.GetProjectType(model.ProjectId);
            int pType = data.Item1;
            int sponCate = data.Item2;
            if (pType == 2)
                pgId = 15;
            else if (pType == 1 && sponCate == 1)
                pgId = 17;
            else
                pgId = 16;
            ViewBag.processGuideLineId = pgId;
            return View(model);
        }

        [HttpGet]
        public JsonResult LoadMainProjectList(string term, int type)
        {
            try
            {
                var data = Common.GetAutoMainProjectNumberList(term, type);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ProjectWFInit(int projectId)
        {
            try
            {
                lock (lockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    ProjectService ps = new ProjectService();
                    bool status = ps.ProjectWFInit(projectId, userId);
                        return Json(status, JsonRequestBehavior.AllowGet);
                   
                }
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ProjectEnhancementWFInit(int projectEnhancementID)
        {
            try
            {
                lock (lockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    ProjectService ps = new ProjectService();
                    bool status = ps.ProjectEnhancementWFInit(projectEnhancementID, userId);
                    return Json(status, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult LoadAgencytList(string term, int? type = null)
        {
            try
            {
                var data = Common.GetAutoCompleteAgency(term, type);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadSchemeList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteScheme(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetAutoCompleteBankdetails(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteBankDetails(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        #region ProposalStatusModify
        public ActionResult ProposalStatusChanger()
        {
            return View();
        }
        [HttpGet]
        public JsonResult LoadAllProposalNumber(string term)
        {
            try
            {
                var data = Common.GetAllProposalNumber(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetProposalStatus(int ProposalId)
        {
            try
            {
                var data = Common.GetCurrentProposalStatus(ProposalId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public JsonResult UpdateProposalStatus(int ProposalId = 0, string Status = "")
        {
            var empty = new ProposalStatusUpdateModel();
            ProposalStatusUpdateModel data = new ProposalStatusUpdateModel();
            ProjectSummaryModel psModel = new ProjectSummaryModel();
            ProjectService ps = new ProjectService();
            if (ProposalId > 0)
            {
                data.Message = ps.UpdateProposalStatus(ProposalId, Status) == true ? "Success" : "Failed";
            }
            else
                data = empty;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
       
        #region Internal Project
        public ActionResult InternalProjectList()
        {
            return View();
        }
        public ActionResult InternalProject(int ProjectId = 0)
        {
            InternalProjectModel model = new InternalProjectModel();
            try
            {
                ViewBag.finYearList = Common.GetFinYearList();
                ViewBag.Type = Common.GetSponsoredInternalType();
                ViewBag.FacultyCodeList = Common.GetInternalCodeFacaltyList("ProjectFacultyCode");
                ViewBag.CentreList = Common.GetCommonHeadList(2, 2);
                ViewBag.LabList = Common.GetCommonHeadList(2, 4);
                ViewBag.InternalClassfication = Common.GetInternalProjectClassfication();
                ViewBag.refFinYear = Common.GetCurrentFinYearId();
                ViewBag.Docmenttype = Common.GetDocTypeList(9);
                ViewBag.deprtmnt = Common.getDepartment();
                ViewBag.PI = Common.GetProjectPIWithDetails();

                if (ProjectId > 0)
                {
                    model = ProjectService.EditInternalProject(ProjectId);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult InternalProject(InternalProjectModel model, HttpPostedFileBase[] file)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                var data = Common.getUserIdAndRole(user_logged_in);
                int logged_in_userid = data.Item1;
                int user_role = data.Item2;
                ViewBag.finYearList = Common.GetFinYearList();
                ViewBag.Type = Common.GetSponsoredInternalType();
                ViewBag.FacultyCodeList = Common.GetInternalCodeFacaltyList("ProjectFacultyCode");
                ViewBag.CentreList = Common.GetCommonHeadList(2, 2);
                ViewBag.LabList = Common.GetCommonHeadList(2, 4);
                ViewBag.InternalClassfication = Common.GetInternalProjectClassfication();
                ViewBag.Docmenttype = Common.GetDocTypeList(9);
                ViewBag.deprtmnt = Common.getDepartment();
                ViewBag.PI = Common.GetProjectPIWithDetails();
                if (ModelState.IsValid)
                {
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                    for (int i = 0; i < model.DocType.Length; i++)
                    {
                        if (file[i] != null)
                        {
                            string docname = Path.GetFileName(file[i].FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                ViewBag.errMsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }

                    }
                    model.CrtdUserId = logged_in_userid;
                    int status = ProjectService.CreateInternalProject(model, file);
                    if (status == 1)
                    {
                        string ProjectNum = Common.getprojectnumber(model.Projectid ?? 0);
                        TempData["succMsg"] = "Project has been opened successfully with Project number - " + ProjectNum + ".";
                        return RedirectToAction("InternalProjectList");
                    }
                    else if (status == 2)
                    {
                        string ProjectNum = Common.getprojectnumber(model.Projectid ?? 0);
                        TempData["succMsg"] = "Project has been Updated Successfully" + ProjectNum;
                        return RedirectToAction("InternalProjectList");
                    }
                    else if (status == 3)
                    {
                        string ProjectNum = Common.getprojectnumber(model.Projectid ?? 0);
                        TempData["alertMsg"] = "This PI already Project Number Available";
                        return RedirectToAction("InternalProjectList");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                    }
                }
                else
                {
                    string messages = string.Join("", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    ViewBag.errMsg = messages;
                    return View(model);
                }
                return RedirectToAction("InternalProjectList");
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public JsonResult SearchInternalProjectList(int pageIndex, int pageSize, InternalProjectSearchModel model)
        {

            object output = ProjectService.GetInternalSearchProjectList(model, pageIndex, pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ApprovalForInternalProject(int ProjectId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                var status = Common.ApprovalForInternalProject(ProjectId, logged_in_user);
                if (!status)
                    return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                return Json(new { status = status, msg = "Submit for Approval Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult InternalProjectWFInit(int projectId)
        {
            try
            {
                lock (lockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    ProjectService ps = new ProjectService();
                    bool status = ps.InternalProjectWFInit(projectId, userId);
                    return Json(status, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult InternalProjectView(int ProjectId, bool Pfinit = false)
        {
            InternalProjectModel viewmodel = new InternalProjectModel();
            viewmodel = ProjectService.GetInternalDetailsView(ProjectId);
            viewmodel.PFInit = Pfinit;
            int pgId = 0;
            var data = Common.GetProjectType(viewmodel.Projectid??0);
            int pType = data.Item1;
            int sponCate = data.Item2;
            decimal sanctionVal = viewmodel.SanctionValue ?? 0;
            if (sanctionVal < 10000000 && pType == 2)
                pgId = 259;
            else if (sanctionVal >= 10000000 && sanctionVal < 50000000 && pType == 2)
                pgId = 264;
            else if (sanctionVal < 10000000 && pType == 1 && sponCate == 1)
                pgId = 263;
            else if (sanctionVal >= 10000000 && sanctionVal < 50000000 && pType == 1 && sponCate == 1)
                pgId = 262;
            else if (sanctionVal >= 10000000 && sanctionVal < 50000000 && pType == 1 && sponCate == 2)
                pgId = 261;
            else if (sanctionVal < 10000000 && pType == 1 && sponCate == 2)
                pgId = 260;
            else if (sanctionVal >= 50000000 && pType == 1 && sponCate == 2)
                pgId = 265;
            else if (sanctionVal >= 50000000 && pType == 1 && sponCate == 1)
                pgId = 266;
            else if (sanctionVal >= 50000000 && pType == 2)
                pgId = 267;
            else
                pgId = 13;
            ViewBag.processGuideLineId = pgId;
            return View(viewmodel);
        }
        #endregion
        #region HeadCenterLab
        public ActionResult HeadCenterLabList()
        {
            return View();
        }
        public ActionResult HeadCenterLab(int headId = 0)
        {
            try
            {
                HeadCenterLabModel model = new HeadCenterLabModel();
                ViewBag.FacultyCodeList = Common.GetCodeFacaltyList("ProjectFacultyCode");
                if (headId > 0)
                {
                    model = ProjectService.GetEditCenter(headId);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult HeadCenterLab(HeadCenterLabModel model)
        {
            try
            {
                ViewBag.FacultyCodeList = Common.GetCodeFacaltyList("ProjectFacultyCode");
                if (ModelState.IsValid)
                {
                    int status = ProjectService.CreateHeadCenterLab(model);
                    if (status == 1)
                    {
                        TempData["succMsg"] = "Name Add Scucessfully";
                        return RedirectToAction("HeadCenterLabList");
                    }
                    else if (status == 2)
                    {
                        TempData["succMsg"] = "Name Updated Scucessfully";
                        return RedirectToAction("HeadCenterLabList");
                    }
                    else if (status == 3)
                    {

                        TempData["alertMsg"] = "This Head Name already Exist";
                        return RedirectToAction("HeadCenterLabList");
                    }
                    else if (status == 4)
                    {

                        TempData["alertMsg"] = "This Head Code already Exist";
                        return RedirectToAction("HeadCenterLabList");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                    }
                }
                else
                {
                    string messages = string.Join("", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    ViewBag.errMsg = messages;
                    return View(model);
                }
                return RedirectToAction("HeadCenterLabList");


            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public JsonResult SearchHeadenterList(int pageIndex, int pageSize, HeadCodeSearchModel model)
        {

            object output = ProjectService.GetCenterHeadList(model, pageIndex, pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region ProjectNumbering Format
        [HttpPost]
        public JsonResult GetProjectNumbering(ProjectNumberingModel model)
        {

            object output = ProjectService.GetProjectNumberingFormat(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetInternalProjectNumbering(ProjectNumberingModel model)
        {

            object output = ProjectService.GetProjectNumberingFormatInternal(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}