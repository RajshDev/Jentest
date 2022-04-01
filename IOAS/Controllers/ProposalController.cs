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
    public class ProposalController : Controller
    {
        // Creation of Proposal (Proposal Opening)
        private static readonly Object lockObj = new Object();

        [HttpGet]
        public JsonResult GetAutoCompleteSponsoredSchemes(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteSponsoredSchemes(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        public ActionResult CreateProposal(int proposalId = 0, int WFproposalid = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1)
                //    return RedirectToAction("Index", "Home");
                CreateProposalModel model = new CreateProposalModel();
                var country = Common.getCountryList();
                var department = Common.getDepartment();
                var gender = Common.getGender();
                var PI = Common.GetProjectPIWithDetails();
                var Doctype = Common.GetDocTypeList(9);
                var scheme = Common.getschemes();
                var agency = Common.getagency();
                var projecttype = Common.getprojecttype();
                var projectsubtype = Common.getprojectsubtype();
                var projectcategory = Common.getprojectcategory();
                var institute = Common.GetInstitute();
                var projectsource = Common.GetSourceList();
                ViewBag.finYearList = Common.GetFinYearList();
                ViewBag.refFinYear = Common.GetCurrentFinYearId();

                ViewBag.schemeCodeList = Common.GetSponsoredSchemeCodeList();
                ViewBag.country = country;
                ViewBag.deprtmnt = department;
                ViewBag.gender = gender;
                ViewBag.Docmenttype = Doctype;
                ViewBag.PI = PI;
                ViewBag.Scheme = scheme;
                ViewBag.Agency = agency;
                ViewBag.projType = projecttype;
                ViewBag.projectsubtype = projectsubtype;
                ViewBag.projectcategory = projectcategory;
                ViewBag.institute = institute;
                ViewBag.projectsource = projectsource;
                ViewBag.SponproposalType = Common.GetSponsoredTypeCategory();
                ViewBag.functionstatus = Common.GetFunctionStatus(9);
                ViewBag.Proposaldomain = Common.GetCodeControlList("Domainrelevantproject");
                //model.Inputdate = DateTime.Now;
                //model.Inptdate = String.Format("{0:dd}", (DateTime)model.Inputdate) + "-" + String.Format("{0:MMMM}", (DateTime)model.Inputdate) + "-" + String.Format("{0:yyyy}", (DateTime)model.Inputdate);
                //var finyear = Common.GetCurrentFinYearId();
                //var Sequencenumber = Common.getseqncenumber(finyear);
                //int year = (DateTime.Now.Year) % 100;
                //var institutecode = "IITM";
                //if (Sequencenumber > 0)
                //{
                //    model.ProposalNumber = year + "_" + institutecode + "_" + Sequencenumber;
                //}
                //else
                //{
                //    model.ProposalNumber = year + "_" + institutecode + "_" + "1";
                //}
                if (proposalId > 0)
                {
                    model = ProposalService.EditProposal(proposalId);
                }
                if (WFproposalid > 0 && proposalId == 0)
                {
                    model = ProposalService.EditWFProposal(WFproposalid);
                    List<MasterlistviewModel> refNumberList = new List<MasterlistviewModel>();

                    refNumberList.Add(new MasterlistviewModel()
                    {
                        id = WFproposalid,
                        name = Common.GetWFProposalNo(WFproposalid).name
                    });
                    ViewBag.refNumberList = refNumberList;
                }
                else
                    ViewBag.refNumberList = new List<MasterlistviewModel>();
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
        public ActionResult CreateProposal(CreateProposalModel model, HttpPostedFileBase[] file)
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
                var projectsubtype = Common.getprojectsubtype();
                var projectcategory = Common.getprojectcategory();
                var institute = Common.GetInstitute();
                var projectsource = Common.GetSourceList();
                ViewBag.finYearList = Common.GetFinYearList();
                ViewBag.refNumberList = new List<MasterlistviewModel>();
                ViewBag.schemeCodeList = Common.GetSponsoredSchemeCodeList();
                ViewBag.country = country;
                ViewBag.deprtmnt = department;
                ViewBag.gender = gender;
                ViewBag.Docmenttype = Doctype;
                ViewBag.PI = PI;
                ViewBag.Scheme = scheme;
                ViewBag.Agency = agency;
                ViewBag.projType = projecttype;
                ViewBag.projectsubtype = projectsubtype;
                ViewBag.projectcategory = projectcategory;
                ViewBag.institute = institute;
                ViewBag.projectsource = projectsource;
                ViewBag.SponproposalType = Common.GetSponsoredTypeCategory();
                ViewBag.functionstatus = Common.GetFunctionStatus(9);
                ViewBag.Proposaldomain = Common.GetCodeControlList("Domainrelevantproject");
                // model.PIEmail = model.PIEmail;
                if (ModelState.IsValid)
                {
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
                    if (model.OtherInstituteCoPIName != null && model.OtherInstituteCoPIName[0] != "")
                    {
                        var otherinstgroupsVal = model.OtherInstituteCoPIName.GroupBy(v => v);
                        if (model.OtherInstituteCoPIName.Length != otherinstgroupsVal.Count())
                        {
                            ViewBag.errMsg = "Some of the Co PI from other institute are duplicated. Please rectify and submit again.";
                            return View(model);
                        }
                        //else if (model.OtherInstituteCoPIName.Contains(model.PIname))
                        //{
                        //    ViewBag.errMsg = "Other Institute Co PI should not be the PI what you have declared. Please rectify and submit again.";
                        //    return View(model);
                        //}
                    }

                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                    //if ((model.AttachName[0] != null && model.AttachName[0] != ""))
                    //{
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
                    ProposalService _ps = new ProposalService();
                    model.ProposalcrtdID = logged_in_userid;

                    var proposalid = _ps.CreateProposal(model, file);
                    if (proposalid > 0)
                    {
                        var proposalnumber = Common.getproposalnumber(proposalid);
                        if (model.ProposalID > 0)
                        {
                            TempData["succMsg"] = "Proposal has been updated successfully with Proposal Number - " + proposalnumber + ".";
                            return RedirectToAction("ProposalList");
                        }
                        else
                        {
                            TempData["succMsg"] = "Proposal has been created successfully with Proposal Number - " + proposalnumber + ".";
                            return RedirectToAction("ProposalList");
                        }

                    }
                    else if (proposalid == 0)
                    {
                        ViewBag.errMsg = "Proposal " + model.Projecttitle + "Already Exists";
                    }
                    else
                        ViewBag.errMsg = "Something went wrong please contact administrator";
                    //}
                    //else
                    //{
                    //    ProposalService _ps = new ProposalService();
                    //    model.ProposalcrtdID = logged_in_userid;

                    //    var proposalid = _ps.CreateProposal(model, file);
                    //    if (proposalid > 0)
                    //    {
                    //        var proposalnumber = Common.getproposalnumber(proposalid);
                    //        if (model.ProposalID > 0)
                    //            ViewBag.succMsg = "Proposal has been updated successfully with Proposal Number - " + proposalnumber + ".";
                    //        else
                    //            ViewBag.succMsg = "Proposal has been created successfully with Proposal Number - " + proposalnumber + ".";
                    //        return View(model);
                    //    }
                    //    else if (proposalid == 0)
                    //    {
                    //        ViewBag.errMsg = "Proposal " + model.Projecttitle + "Already Exists";
                    //    }
                    //    else
                    //        ViewBag.errMsg = "Something went wrong please contact administrator";
                    //}
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    ViewBag.errMsg = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {

                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                ViewBag.errMsg = "Something went wrong please contact administrator";
                return View(model);
            }

        }


        [HttpGet]
        public JsonResult GetProposalList()
        {
            object output = ProposalService.GetProposalDetails();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSearchProposalList(string keyword)
        {
            object output = ProposalService.GetProposalDetails();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetTapalRefNumber()
        {
            int depId = Common.GetDepartmentId(User.Identity.Name);
            object output = Common.GetTapalRefNumberList(depId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWorkflowRefNumber()
        {
            object output = Common.GetWorkflowRefNumberList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetACTapalRefNumber(string term)
        {
            int depId = Common.GetDepartmentId(User.Identity.Name);
            object output = Common.GetAutoCompleteTapalRefNumberList(term,depId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetACWorkflowRefNumber(string term)
        {
            object output = Common.GetAutoCompleteWorkflowRefNumberList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult ProposalList(int page = 1)
        //{
        //    try
        //    {

        //        string user_logged_in = User.Identity.Name;
        //        var data = Common.getUserIdAndRole(user_logged_in);
        //        int logged_in_userid = data.Item1;
        //        int user_role = data.Item2;
        //        //if (user_role == 1 || user_role == 2)
        //        return View();

        //        //else
        //        //    return RedirectToAction("Department", "Account");


        //    }
        //    catch (Exception ex)
        //    {

        //        ProposalModel model = new ProposalModel();
        //        var proposals = new PagedData<ProposalResultModels>();
        //        model.resultField = proposals;
        //        return View(model);
        //    }
        //}


        [HttpPost]
        public ActionResult SearchProposalList(string proposalstatus, string Institute, string Industry, string selMinistry, string srchKeyword, int page = 1)
        {
            try
            {
                int pageSize = 10;
                ProposalModel model = new ProposalModel();
                var srchFields = new ProposalSrchFieldsModel();
                var proposals = new PagedData<ProposalResultModels>();
                srchFields.srchKeyword = srchKeyword;
                if (!string.IsNullOrWhiteSpace(proposalstatus))
                {
                    srchFields.Proposalstatus = Convert.ToInt32(proposalstatus);
                }
                if (!string.IsNullOrWhiteSpace(Institute))
                {
                    srchFields.Institute = Convert.ToInt32(Institute);
                }
                if (!string.IsNullOrWhiteSpace(Industry))
                {
                    srchFields.Industry = Convert.ToInt32(Industry);
                }
                if (!string.IsNullOrWhiteSpace(selMinistry))
                {
                    srchFields.selMinistry = Convert.ToInt32(selMinistry);
                }
                ProposalService _ps = new ProposalService();
                string user_logged_in = User.Identity.Name;
                var data = Common.getUserIdAndRole(user_logged_in);
                int logged_in_userid = data.Item1;
                int user_role = data.Item2;
                //if (user_role == 1 || user_role == 2)
                proposals = _ps.GetProposal(srchFields, page, pageSize);

                //else
                //    return RedirectToAction("Department", "Account");
                model.resultField = proposals;
                return PartialView(model);
            }
            catch (Exception ex)
            {

                var proposals = new PagedData<ProposalResultModels>();
                return PartialView(proposals);
            }
        }

        [HttpPost]
        public JsonResult EditProposal(int ProposalId)
        {
            object output = ProposalService.EditProposal(ProposalId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult DeleteProposal(int ProposalId)
        {
            int logged_in_userId = Common.GetUserid(User.Identity.Name);
            object output = ProposalService.DeleteProposal(ProposalId, logged_in_userId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ShowDocument(string file, string filepath)
        {
            try
            {
                //int roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1 && roleId != 3)
                //    return new EmptyResult();
                string fileType = Common.GetMimeType(Path.GetExtension(file));
                byte[] fileData = file.DownloadFile(Common.GetDirectoryName(filepath));
                Response.AddHeader("Content-Disposition", "inline; filename=\"" + file + "\"");
                return File(fileData, fileType);
            }
            catch (FileNotFoundException)
            {
                throw new HttpException(404, "File not found.");
            }
        }
        public ActionResult ShowWFProposalDocument(string file, string filepath)
        {
            try
            {
                //int roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1 && roleId != 3)
                //    return new EmptyResult();
                string fileType = Common.GetMimeType(Path.GetExtension(file));
                byte[] fileData = file.WFProposalDownloadFile(Common.GetDirectoryName(filepath));
                Response.AddHeader("Content-Disposition", "inline; filename=\"" + file + "\"");
                return File(fileData, fileType);
            }
            catch (FileNotFoundException)
            {
                throw new HttpException(404, "File not found.");
            }
        }

        public ActionResult ProjectOpening()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetProposalList(string FunctionStatus, string Projecttitle, string NameofPI,string EmpCode, DateFilterModel Prpsalinwrddate, string ProposalNumber,string PslNumber, int pageIndex, int pageSize, int? ProjectType = null,  DateTime? Fromdate = null, DateTime? Todate = null, int? PIName = null)
        {
            object output = ProposalService.SearchProposalList(ProjectType, ProposalNumber, PslNumber, PIName, Fromdate, Todate, FunctionStatus,Projecttitle,NameofPI,EmpCode, Prpsalinwrddate, pageIndex,pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ViewProposal(int ProposalId, bool Pfinit = false)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1)
                //    return RedirectToAction("Index", "Home");
                ViewBag.disabled = "Disabled";
                TempData["viewMode"] = "ViewOnly";
                ViewProposalModel model = new ViewProposalModel();
                
                model = ProposalService.ViewProposal(ProposalId);
                model.PFInit = Pfinit;
                ViewBag.processGuideLineId = 6;
                return View(model);

            }
            catch (Exception ex)
            {

                return View();
            }

        }

        [HttpPost]
        public ActionResult ProposalWFInit(int proposalID)
        {
            try
            {
                lock (lockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    ProposalService ps = new ProposalService();
                    bool status = ps.ProposalWFInit(proposalID, userId);
                    return Json(status, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ProposalList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DeleteWFProposal(int WFid, string Type)
        {
            int logged_in_userId = Common.GetUserid(User.Identity.Name);
            object output = Common.IsDeleteWFProposal(WFid, logged_in_userId, Type);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
      
    }
}