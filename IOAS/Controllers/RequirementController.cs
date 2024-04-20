using IOAS.Filter;
using IOAS.GenericServices;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using Rotativa.MVC;
using Rotativa.Core;
using Rotativa.Core.Options;
using System.Threading.Tasks;
using IOAS.DataModel;
using ClosedXML.Excel;
using System.Data.SqlClient;

namespace IOAS.Controllers
{
    [Authorized]
    public class RequirementController : Controller
    {
        private static readonly Object lockObj = new Object();
        private static readonly Object CONSWfInitlockObj = new Object();
        private static readonly Object lockCommitCloserequestObj = new Object();
        private static readonly Object lockCommitAddrequestObj = new Object();
        private static readonly Object lockCommitbookrequestObj = new Object();
        private static readonly Object lockCommitrejectrequestObj = new Object();
        ErrorHandler WriteLog = new ErrorHandler();
        RequirementService recruitmentService = new RequirementService();
        Utility _uty = new Utility();
        CoreAccountsService coreAccountService = new CoreAccountsService();

        private string getEmployeeActionLink(string category, string retrunLink = null)
        {
            if (!string.IsNullOrEmpty(retrunLink))
            {
                if (retrunLink == "DA")
                    return category == "STE" ? "STEDAApplicationStatus" : category == "CON" ? "CONDAApplicationStatus" : category == "OSG" ? "OSGDAApplicationStatus" : "";
                else if (retrunLink == "AL")
                    return category == "STE" ? "STEList" : category == "CON" ? "ConsultantAppointmentList" : category == "OSG" ? "OutsourcingList" : "";
                else if (retrunLink == "VAL")
                    return category == "STE" ? "STEVerficationList" : category == "CON" ? "ConsultantAppointmentList" : category == "OSG" ? "OutsourcingList" : "";
                else if (retrunLink == "OVAL")
                    return category == "STE" ? "STEVerficationList" : category == "CON" ? "ConsultantAppointmentList" : category == "OSG" ? "OutsourcingList" : "";
                else if (retrunLink == "OSGVAL")
                    return category == "STE" ? "OSGVerificationList" : category == "CON" ? "ConsultantAppointmentList" : category == "OSG" ? "OutsourcingList" : "";
                else if (retrunLink == "OSGORVAL")
                    return category == "STE" ? "OSGVerificationList" : category == "CON" ? "ConsultantAppointmentList" : category == "OSG" ? "OutsourcingList" : "";
                else if (retrunLink == "TC")
                    return "TenureClosureList";
                else if (retrunLink == "HR")
                    return "ApplicationList";
                else if (retrunLink == "AC")
                    return "ApplicationCancel";
                else if (retrunLink == "HRA")
                    return "HRAList";
                else if (retrunLink == "ML")
                    return "MaternityLeaveList";
                else if (retrunLink == "SPLOP")
                    return "StoppaymentList";
                else if (retrunLink == "RL")
                    return "RelievingList";
            }
            return category == "STE" ? "STEEmployeeMaster" : category == "CON" ? "CONEmployeeMaster" : category == "OSG" ? "OSGEmployeeMaster" : "";
        }

        private string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }

        [HttpGet]
        public JsonResult DeleteDocument(int Appid, string Apptype, string Doctype, string File, int? Orderid = null)
        {
            try
            {
                lock (lockObj)
                {
                    var data = RequirementService.DeleteDocument(Appid, Apptype, Doctype, File, Orderid);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Loaded Data

        [HttpGet]
        public JsonResult LoadDesignationNameList(string term)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteDesignationNameList(term);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadAnnouncementdesList(string term)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoDesignationList(term);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadDesignationList(string term, int? TypeCode, bool? isConsolidatePay)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteDesignationList(term, TypeCode ?? 0, isConsolidatePay ?? false);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadPIList(string term, int? type = null)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompletePIList(term, type);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPIDetails(int UserID)
        {
            var projectData = RequirementService.getPIDetails(Convert.ToInt32(UserID));
            var result = new { projectData = projectData };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPIByCourseList(int DepartmentId)
        {
            var locationdata = Common.GetCourseList(DepartmentId);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadProjectDetails(int ProjectId)
        {
            var projectData = RequirementService.getProjectSummary(Convert.ToInt32(ProjectId));
            var result = new { projectData = projectData };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadDesignationDetails(int DesignationID)
        {
            var data = recruitmentService.getDesignationDetails(Convert.ToInt32(DesignationID));
            var result = new { DesignationData = data };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadBankNameList(string term)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteBankName(term);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadWokrPlaceList(string term)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteWorkPlace(term);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadVendorCodeList(string Vendor)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteSlyAgencyList(Vendor);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadVendorName(string VendorCode)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.LoadSalaryAgencyName(VendorCode);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadSalCalPercent(int VendorId, decimal Salary = 0)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetSalCalPercent(VendorId, Salary);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadEmployeeList(string term)
        {
            try
            {
                string user = User.Identity.Name;
                var data = Common.GetAutoCompleteEmployeeList(term, user);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Annoncement

        public ActionResult AnnouncementWFInitiate(int AnnouncementID)
        {
            int userId = Common.GetUserid(User.Identity.Name);
            try
            {
                var Result = recruitmentService.AnnouncementWFInit(AnnouncementID, userId);
                return Json(Result.Item1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult CreateAnnouncement(int AnnouncementID = 0, bool isRepost = false)
        {
            try
            {
                AnnouncementMasterModel model = new AnnouncementMasterModel();
                model.isRepost = isRepost;
                if (AnnouncementID > 0)
                {
                    model = recruitmentService.GetEditAnnouncement(AnnouncementID);
                    string Type = "";
                    if (model.FlowApprover == "CMAdmin" && (model.StatusID == 2 || model.StatusID < 6))
                        Type = "Announcement CMNote";
                    else if (model.FlowApprover2 == "CMAdmin" && (model.StatusID == 6 || model.StatusID < 10))
                        Type = "Announcement Shortlist CMNote";
                    else if (model.FlowApprover3 == "CMAdmin" && (model.StatusID == 10 || model.StatusID <= 13))
                        Type = "Announcement Selectlist CMNote";
                    else if (model.FlowApprover == "DeanFlow" && (model.StatusID == 2 || model.StatusID < 6))
                        Type = "Announcement DeanNote";
                    else if (model.FlowApprover2 == "DeanFlow" && (model.StatusID == 6 || model.StatusID < 10))
                        Type = "Announcement Shortlist DeanNote";
                    else if (model.FlowApprover3 == "DeanFlow" && (model.StatusID == 10 || model.StatusID <= 13))
                        Type = "Announcement Selectlist DeanNote";
                    else
                    {
                        if (model.StatusID == 2 || model.StatusID < 6)
                            Type = "Announcement Note";
                        else if (model.StatusID == 6 || model.StatusID < 10)
                            Type = "Announcement Shortlist Note";
                        else if (model.StatusID == 10 || model.StatusID <= 13)
                            Type = "Announcement Selectlist Note";
                    }
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(191, Type, 0);
                    ViewBag.Backdateannouncementenabled = System.Web.Configuration.WebConfigurationManager.AppSettings["Backdateannouncementenabled"];
                    model.isRepost = isRepost;
                }
                else
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(190, "", 0);
                ViewBag.announcementType = Common.GetCodeControlList("Announcement Status");
                ViewBag.Institution = Common.GetCodeControlList("Institution");
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return RedirectToAction("Home", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult CreateAnnouncement(AnnouncementMasterModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.announcementType = Common.GetCodeControlList("Announcement Status");
                ViewBag.Institution = Common.GetCodeControlList("Institution");
                string ErrMsg = string.Empty;
                var button = Request["button"];
                if (button.Contains("Save as drafts"))
                    button = button == null ? "" : button.Split(',')[1];
                if (ModelState.IsValid)
                {
                    using (var context = new IOASDBEntities())
                    {
                        if (model.AnnouncementID > 0 && model.StatusID == 6 && model.isShortlist)
                        {
                            if (model.RemarkDocument != null)
                            {
                                var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
                                var extension = Path.GetExtension(model.RemarkDocument.FileName);
                                if (!allowedExtensions.Contains(extension.ToLower()))
                                {
                                    ModelState.AddModelError("", "Please upload any one of these type doc [.pdf,.doc,.docx]");
                                    return View(model);
                                }
                            }
                            foreach (var item in model.DesignationDetails)
                            {
                                if (item.ShortlistedApplicantFile != null && item.TotalApplicantFile != null)
                                {
                                    string[] validFileTypes = { ".xls", ".xlsx", ".doc", ".docx", ".pdf" };
                                    var extension = Path.GetExtension(item.ShortlistedApplicantFile.FileName).ToLower();
                                    var extension1 = Path.GetExtension(item.TotalApplicantFile.FileName).ToLower();
                                    if (!validFileTypes.Contains(extension.ToLower()) || !validFileTypes.Contains(extension1.ToLower()))
                                    {
                                        ModelState.AddModelError("", "Please upload any one of these type doc ['.xls','.xlsx','.doc', '.docx', '.pdf' ]");
                                        return View(model);
                                    }
                                }
                                //else
                                //{
                                //    if (context.tblRCTAnnouncementDetails.Any(m => m.AnnouncementID == model.AnnouncementID && m.isCurrentVersion == true && (string.IsNullOrEmpty(m.ShortlistedApplicantsDoc) || string.IsNullOrEmpty(m.TotalApplicantsDoc))))
                                //    {
                                //        ErrMsg = "Please upload file any one of these type doc ['.xls','.xlsx' ]";
                                //        TempData["errMsg"] = ErrMsg;
                                //    }
                                //}
                            }
                        }

                        if (model.AnnouncementID > 0 && model.StatusID == 10 && model.isSelectionlist)
                        {
                            foreach (var item in model.DesignationDetails)
                            {
                                if (item.SelectedApplicantFile != null)
                                {
                                    string[] validFileTypes = { ".xls", ".xlsx", ".doc", ".docx", ".pdf" };
                                    var extension = Path.GetExtension(item.SelectedApplicantFile.FileName);
                                    if (!validFileTypes.Contains(extension.ToLower()))
                                    {
                                        ModelState.AddModelError("", "Please upload any one of these type doc ['.xls','.xlsx','.doc', '.docx', '.pdf' ]");
                                        return View(model);
                                    }
                                }
                                //else
                                //{
                                //    if (context.tblRCTAnnouncementDetails.Any(m => m.AnnouncementID == model.AnnouncementID && m.isCurrentVersion == true && string.IsNullOrEmpty(m.SelectedApplicantsDoc)))
                                //    {
                                //        ErrMsg = "Please upload file any one of these type doc ['.xls','.xlsx' ]";
                                //        TempData["errMsg"] = ErrMsg;
                                //    }
                                //}
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(ErrMsg))
                    {
                        var result = RequirementService.AnnouncementEntry(model, userId, button);
                        if (!string.IsNullOrEmpty(result.Item2))
                        {
                            TempData["succMsg"] = result.Item2;
                            return RedirectToAction("AnnoncementMaster", "Requirement");
                        }
                        else
                        {
                            TempData["errMsg"] = "Something went wrong please contact administrator";
                            return RedirectToAction("AnnoncementMaster", "Requirement");
                        }
                    }
                    else
                    {
                        model.strRequestReceiveDate = string.Format("{0:dd-MMMM-yyyy}", model.RequestReceiveDate);
                        model.strAnnouncementClosureDate = string.Format("{0:dd-MMMM-yyyy}", model.AnnouncementClosureDate);
                        model.strInterviewDate = string.Format("{0:dd-MMMM-yyyy}", model.InterviewDate);
                        model.strOfferletterGenerationdate = string.Format("{0:dd-MMMM-yyyy}", model.OfferletterGenerationdate);
                    }
                }
                else if (button == "Save as drafts")
                {
                    var result = RequirementService.AnnouncementEntry(model, userId, button);
                    TempData["succMsg"] = result.Item2;
                    return RedirectToAction("AnnoncementMaster", "Requirement");
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
                WriteLog.SendErrorToText(ex);
                ViewBag.announcementType = Common.GetCodeControlList("Announcement Status");
                ViewBag.Institution = Common.GetCodeControlList("Institution");
                return View(model);
            }
        }

        #region AnnoncementMaster
        public ActionResult AnnoncementMaster()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetAnnoncementList(int pageIndex, int pageSize, SearchAnnouncementModel model, DateFilterModel strRequestReceiveDate, DateFilterModel strAnnouncementClosureDate)
        {
            try
            {
                object output = recruitmentService.GetAnnouncementList(pageIndex, pageSize, model, strRequestReceiveDate, strAnnouncementClosureDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region AnnoncementCancel
        [HttpPost]
        public JsonResult AnnoncementCancel(int AnnouncementID, string CancelReason)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                var output = recruitmentService.AnnouncementCancel(AnnouncementID, CancelReason, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Annoncement Notes

        public ActionResult AnnouncementNoteView(int AnnouncementID)
        {
            try
            {
                AnnouncementMasterModel model = new AnnouncementMasterModel();
                model = recruitmentService.GetAnnouncementDetails(AnnouncementID);
                string Type = "";
                if (model.FlowApprover == "CMAdmin" && (model.StatusID == 2 || model.StatusID < 6))
                    Type = "Announcement CMNote";
                else if (model.FlowApprover2 == "CMAdmin" && (model.StatusID == 6 || model.StatusID < 10))
                    Type = "Announcement Shortlist CMNote";
                else if (model.FlowApprover3 == "CMAdmin" && (model.StatusID == 10 || model.StatusID <= 13))
                    Type = "Announcement Selectlist CMNote";
                else if (model.FlowApprover == "DeanFlow" && (model.StatusID == 2 || model.StatusID < 6))
                    Type = "Announcement DeanNote";
                else if (model.FlowApprover2 == "DeanFlow" && (model.StatusID == 6 || model.StatusID < 10))
                    Type = "Announcement Shortlist DeanNote";
                else if (model.FlowApprover3 == "DeanFlow" && (model.StatusID == 10 || model.StatusID <= 13))
                    Type = "Announcement Selectlist DeanNote";
                else
                {
                    if (model.StatusID == 2 || model.StatusID < 6)
                        Type = "Announcement Note";
                    else if (model.StatusID == 6 || model.StatusID < 9)
                        Type = "Announcement Shortlist Note";
                    else if (model.StatusID == 10 || model.StatusID <= 13)
                        Type = "Announcement Selectlist Note";
                }
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(191, Type, 0);
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "Home");
            }
        }
        #endregion

        [HttpPost]
        public async Task<JsonResult> UploadXL(int AnnouncementID = 0, string Category = "")
        {
            try
            {
                var Error = "";
                List<AnnouncementMailModel> list = new List<AnnouncementMailModel>();
                var fileContent = Request.Files[0];
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(fileContent.FileName);
                    string extension = Path.GetExtension(fileContent.FileName);
                    string guid = Guid.NewGuid().ToString();
                    string docName = guid + "_" + fileName;
                    string[] allowedExtensions = { ".xls", ".xlsx" };
                    string connString = "";
                    if (allowedExtensions.Contains(extension.ToLower()))
                    {
                        string folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/UploadXL");
                        string path = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/UploadXL/" + docName);
                        if (!Directory.Exists(folder))
                            Directory.CreateDirectory(Server.MapPath("~/Content/Requirement/UploadXL"));
                        fileContent.SaveAs(path);
                        if (extension.ToLower().Trim() == ".xls" && Environment.Is64BitOperatingSystem == false)
                        {
                            connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            DataTable dt = _uty.ConvertXSLXtoDataTable(path, connString);
                            list = IOAS.GenericServices.Converter.ConvertDataTable<AnnouncementMailModel>(dt);
                        }
                        else
                        {
                            connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            DataTable dt = _uty.ConvertXSLXtoDataTable(path, connString);
                            list = IOAS.GenericServices.Converter.ConvertDataTable<AnnouncementMailModel>(dt);
                        }
                    }
                    else
                    {
                        Error = "Please upload any one of these type doc [.xls, .xlsx]";
                    }
                }
                var result = new { Count = list.Count, Error = Error };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                var result = new { Count = 0 };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Short term engagement

        public ActionResult STEList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetSTEList(STESearchModel model, int pageIndex, int pageSize)
        {
            try
            {
                object output = RequirementService.GetSTEList(model, pageIndex, pageSize);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }

        public ActionResult STEJunior(int STEID = 0, int WFid = 0)
        {
            STEModel model = new STEModel();
            try
            {
                ViewBag.List = new List<MasterlistviewModel>();
                model.ApplicationEntryDate = DateTime.Now;
                model.Status = string.Empty;
                model.Medical = 2;
                ViewBag.Apptype = Common.GetCodeControlList("STEAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                if (STEID > 0)
                {
                    model = recruitmentService.GetEditSTE(STEID);
                    if (model.DateofBirth != null)
                        ViewBag.Years = Common.getRequirementyear(model.DateofBirth.Value.Year);
                    if (model.FlowApprover == "CMAdmin")
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STEAdminFlow", 0);
                    else if (model.FlowApprover == "NDean")
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STEFlowDean", 0);
                    else
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STE Flow", 0);
                }
                else if (WFid > 0 && STEID == 0)
                {
                    model = Common.GetWFEditSTE(WFid);
                    var listcommitte = Common.GetCommittee();
                    if (listcommitte.Item1.Count > 0)
                    {
                        for (int i = 0; i < listcommitte.Item1.Count; i++)
                        {
                            if (i == 0)
                            {
                                model.CommiteeMemberId1 = listcommitte.Item1[i].id ?? 0;
                                model.CommiteeMember1 = listcommitte.Item1[i].name;
                            }
                            if (i == 1)
                            {
                                model.CommiteeMemberId2 = listcommitte.Item1[i].id ?? 0;
                                model.CommiteeMember2 = listcommitte.Item1[i].name;
                            }
                        }
                        var datacharperson = Common.GetChairPerson();
                        model.ChairpersonNameId = datacharperson.Item1;
                        model.ChairpersonName = datacharperson.Item2;
                    }
                }
                else
                {
                    var listcommitte = Common.GetCommittee();
                    if (listcommitte.Item1.Count > 0)
                    {
                        for (int i = 0; i < listcommitte.Item1.Count; i++)
                        {
                            if (i == 0)
                            {
                                model.CommiteeMemberId1 = listcommitte.Item1[i].id ?? 0;
                                model.CommiteeMember1 = listcommitte.Item1[i].name;
                            }
                            if (i == 1)
                            {
                                model.CommiteeMemberId2 = listcommitte.Item1[i].id ?? 0;
                                model.CommiteeMember2 = listcommitte.Item1[i].name;
                            }
                        }
                        var datacharperson = Common.GetChairPerson();
                        model.ChairpersonNameId = datacharperson.Item1;
                        model.ChairpersonName = datacharperson.Item2;
                    }
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(191, "", 0);
                }
                model.isDraftbtn = false;
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Apptype = Common.GetCodeControlList("STEAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(191, "", 0);
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(model.DateofBirth.Value.Year);
                else
                    ViewBag.Years = Common.getRequirementyear();
                WriteLog.SendErrorToText(ex);
                return View(model);
            }

        }

        [HttpPost]
        public ActionResult STEJunior(STEModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Apptype = Common.GetCodeControlList("STEAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                if (model.EducationDetail.Count > 0)
                {
                    foreach (var item in model.EducationDetail)
                        item.DisiplineList = Common.GetCourseList(item.QualificationId ?? 0);
                }

                #region FileValidation
                if (model.PersonImage != null)
                {
                    var allowedExtensions = new[] { ".jpeg", ".png", ".jpg", ".gif" };
                    var extension = Path.GetExtension(model.PersonImage.FileName);
                    if (!allowedExtensions.Contains(extension.ToLower()))
                    {
                        TempData["alertMsg"] = "Please upload any one of these type candidate image [.jpeg,.png,.jpg]";
                        return View(model);
                    }
                    if (model.PersonImage.ContentLength > 1000000)
                    {
                        TempData["alertMsg"] = "You can upload candidate image up to 1MB";
                        return View(model);
                    }
                }

                if (model.Resume != null)
                {
                    var extension = Path.GetExtension(model.Resume.FileName);
                    if (extension.ToLower() != ".pdf")
                    {
                        TempData["alertMsg"] = "Please upload any one of these type Resume [.pdf]";
                        return View(model);
                    }
                    if (model.Resume.ContentLength > 5242880)
                    {
                        TempData["alertMsg"] = "You can upload Resume up to 5MB";
                        return View(model);
                    }
                }

                if (model.PIJustificationFile != null)
                {
                    var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
                    for (int i = 0; i < model.PIJustificationFile.Count(); i++)
                    {
                        if (model.PIJustificationFile[i] != null)
                        {
                            string extension = Path.GetExtension(model.PIJustificationFile[i].FileName);
                            if (!allowedExtensions.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type PI judtification document [.doc,.docx,.pdf]";
                                return View(model);
                            }
                            if (model.PIJustificationFile[i].ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload PI judtification document up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }

                if (model.EducationDetail != null)
                {
                    var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                    {
                        if (model.EducationDetail[i].Certificate != null)
                        {
                            string extension = Path.GetExtension(model.EducationDetail[i].Certificate.FileName);
                            if (!allowedExtensionsCer.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type certificate [.jpeg,.png,.jpg,.gif,.pdf]";
                                return View(model);
                            }
                            if (model.EducationDetail[i].Certificate.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload certificate up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }

                if (model.ExperienceDetail != null)
                {
                    var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                    for (int i = 0; i < model.ExperienceDetail.Count; i++)
                    {
                        if (model.ExperienceDetail[i].ExperienceFile != null)//...........Experience Certificates
                        {
                            string filename = System.IO.Path.GetFileName(model.ExperienceDetail[i].ExperienceFile.FileName);
                            string extension = Path.GetExtension(filename);
                            if (!allowedExtensionsCer.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type certificate [.jpeg,.png,.jpg,.gif,.pdf]";
                                return View(model);
                            }
                            if (model.ExperienceDetail[i].ExperienceFile.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload certificate up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }
                #endregion

                if (model.STEId > 0)
                {
                    if (model.aadharnumber != null)
                    {
                        var chkemployeeadhar = Common.CheckPreviousEmployeeAdharserver(Convert.ToString(model.aadharnumber), model.ApplicationNo, true, model.OldEmployeeNumber, "STE");
                        if (chkemployeeadhar != "")
                        {
                            TempData["alertMsg"] = chkemployeeadhar;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                    if (!string.IsNullOrEmpty(model.PAN))
                    {
                        var chkemployeepanno = Common.CheckPreviousEmployeePanserver(model.PAN, model.ApplicationNo, true, model.OldEmployeeNumber, "STE");
                        if (chkemployeepanno != "")
                        {
                            TempData["alertMsg"] = chkemployeepanno;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                }
                else
                {
                    if (model.aadharnumber != null)
                    {
                        var chkemployeeadhar = Common.CheckPreviousEmployeeAdharserver(Convert.ToString(model.aadharnumber), null, true, model.OldEmployeeNumber, "STE");
                        if (chkemployeeadhar != "")
                        {
                            TempData["errMsg"] = chkemployeeadhar;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                    if (!string.IsNullOrEmpty(model.PAN))
                    {
                        var chkemployeepanno = Common.CheckPreviousEmployeePanserver(model.PAN, null, true, model.OldEmployeeNumber, "STE");
                        if (chkemployeepanno != "")
                        {
                            TempData["errMsg"] = "This Pan Number is linked to  " + chkemployeepanno;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                }

                if (model.isDraftbtn == false && ModelState.IsValid)
                {
                    string validationMsg = ValidateSTEFormData(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        model.Status = model.Status == null ? "" : model.Status;
                        return View(model);
                    }
                    var result = recruitmentService.PostSTE(model, userId);
                    if (result.Item1 == 1)
                    {
                        TempData["succMsg"] = "Application submitted for approval";
                        return RedirectToAction("STEList", "Requirement");
                    }
                    else if (result.Item1 == 2)
                    {
                        TempData["succMsg"] = "STE application submitted for PI justification";
                        return RedirectToAction("STEList", "Requirement");
                    }
                    else if (result.Item1 == -1)
                    {
                        TempData["errMsg"] = result.Item3;
                        return RedirectToAction("STEList", "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("STEList", "Requirement");
                    }
                }
                else if (model.isDraftbtn == false && !ModelState.IsValid)
                {
                    string messages = string.Join("\n", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                    model.Status = model.Status == null ? "" : model.Status;
                    TempData["errMsg"] = messages;
                }
                else
                {
                    var result = recruitmentService.PostSTE(model, userId);//....Draft button....
                    if (result.Item1 == 1)
                    {
                        TempData["succMsg"] = "Draft Saved Successfully";
                        return RedirectToAction("STEList", "Requirement");
                    }
                    else if (result.Item1 == 2)
                    {
                        TempData["succMsg"] = "Draft updated";
                        return RedirectToAction("STEList", "Requirement");
                    }
                    else if (result.Item1 == -1)
                    {
                        TempData["errMsg"] = result.Item3;
                        return RedirectToAction("STEList", "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("STEList", "Requirement");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Apptype = Common.GetCodeControlList("STEAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                #region UpdateDisiplineList
                if (model.EducationDetail.Count > 0)
                {
                    foreach (var item in model.EducationDetail)
                        item.DisiplineList = Common.GetCourseList(item.QualificationId ?? 0);
                }
                #endregion
                model.Status = model.Status == null ? "" : model.Status;
                TempData["errMsg"] = "Something went wrong please contact administrator";
                return View(model);
            }
        }

        private string ValidateSTEFormData(STEModel model)
        {
            string msg = "Valid";
            using (var context = new IOASDBEntities())
            {

                var queryste = (from s in context.tblRCTSTE
                                where s.STEID == model.STEId && (s.Status == "Open" || s.Status == "Draft" || s.Status == "Note to PI")
                                select s).FirstOrDefault();
                if (model.EmployeeType.Contains("Old"))
                {
                    if (!string.IsNullOrEmpty(model.OldEmployeeNumber))
                    {
                        var query = (from s in context.tblRCTSTE
                                     where s.EmployeersID == model.OldEmployeeNumber && s.Status == "Relieved"
                                     orderby s.STEID descending
                                     select s).FirstOrDefault();
                        if (query != null)
                        {
                            if (query.AppointmentEnddate >= model.Appointmentstartdate)
                                msg = msg == "Valid" ? "Appointment date cannot be less than relieving date." : msg + "<br /> Appointment date cannot be less than relieving date.";
                        }
                        else
                            msg = msg == "Valid" ? "Please enter valid old employee number." : msg + "<br /> Please enter valid old employee number.";
                    }
                    else
                        msg = msg == "Valid" ? "Please enter old employee number." : msg + "<br /> Please enter old employee number.";
                }
                string aadhar = Convert.ToString(model.aadharnumber);
                var vwQuery = (from vw in context.vw_RCTOverAllApplicationEntry.AsNoTracking()
                               where (string.IsNullOrEmpty(aadhar) || vw.AadhaarNo.Contains(aadhar))
                               && (string.IsNullOrEmpty(model.PAN) || vw.PANNo.Contains(model.PAN)) && vw.ApplicationType == "New"
                               && vw.Status == "Relieved"
                               orderby vw.ApplicationEntryDate descending
                               select vw).FirstOrDefault();
                if (vwQuery != null)
                {
                    DateTime? relievedate = vwQuery.RelieveDate == null ? vwQuery.AppointmentEnddate : vwQuery.RelieveDate;
                    if (relievedate >= model.Appointmentstartdate)
                        msg = msg == "Valid" ? "Appointment date cannot be less than relieving date (" + string.Format("{0:dd-MMMM-yyyy}", relievedate) + " - " + vwQuery.EmployeersID + ")." : msg + "<br /> Appointment date cannot be less than relieving date (" + string.Format("{0:dd-MMMM-yyyy}", relievedate) + " - " + vwQuery.EmployeersID + ").";
                }

                if (model.DesignationId != null)
                {
                    var querydes = context.tblRCTDesignation.Where(m => m.DesignationId == model.DesignationId && m.TypeOfAppointment == 2 && m.RecordStatus == "Active").FirstOrDefault();
                    if (querydes != null)
                    {
                        if (querydes.Medical != true)
                        {
                            if (model.Medical != 3 || model.MedicalAmmount > 0)
                                msg = msg == "Valid" ? "Entered designation does not have medical contribution. please contact administrator." : msg + "<br /> Entered designation does not have medical contribution. please contact administrator.";
                        }
                        else
                        {
                            decimal Deduction = querydes.MedicalDeduction ?? 0;
                            if (model.MedicalAmmount > 0 && model.MedicalAmmount != Deduction)
                                msg = msg == "Valid" ? "Medical contribution data mis match. please contact administrator." : msg + "<br /> Medical contribution data mis match. please contact administrator.";
                        }

                        if (querydes.HRA != true)
                        {
                            if (model.HRA > 0)
                                msg = msg == "Valid" ? "Entered designation does not have HRA provision. Please contact administrator." : msg + "<br />Entered designation does not have HRA provision. Please contact administrator.";
                        }
                        else
                        {
                            var hraper = querydes.HRABasic / 100;
                            var hra = model.Salary * hraper;
                            if (model.HRA > 0 && model.HRA != hra)
                                msg = msg == "Valid" ? "HRA entered exceeds institution norms." : msg + "<br /> HRA entered exceeds institution norms.";
                        }
                        if (querydes.SalaryLevel > 0)
                        {
                            if (querydes.SalaryLevel != model.SalaryLevelId)
                                msg = msg == "Valid" ? "Entered designation does not have salary level. Please contact administrator." : msg + "<br /> Entered designation does not have salary level. Please contact administrator.";
                        }
                    }
                }
                else
                {
                    msg = msg == "Valid" ? "Enter valid designation code." : msg + "<br /> Enter valid designation code.";
                }

                if (model.ProjectId != null)
                {
                    var _projectDetail = RequirementService.getProjectSummary(model.ProjectId ?? 0);
                    if (_projectDetail != null)
                    {
                        if (model.Appointmentstartdate != null && model.AppointmentEndDate != null)
                        {
                            double monthdiff = model.AppointmentEndDate.Value.Subtract(model.Appointmentstartdate.Value).Days + 1;

                            if (model.Appointmentstartdate.Value.Year == model.AppointmentEndDate.Value.Year && model.Appointmentstartdate.Value.Month == model.AppointmentEndDate.Value.Month && model.Appointmentstartdate.Value.Month == 2)
                            {
                                if (monthdiff >= 28)
                                    monthdiff = 30;
                            }

                            if (monthdiff <= 29)
                                msg = msg == "Valid" ? "Appointment tenure should be minimum 1 month to 1 year." : msg + "<br /> Appointment tenure should be minimum 1 month to 1 year.";

                            var Days = model.AppointmentEndDate.Value.Subtract(model.Appointmentstartdate.Value).TotalDays + 1;

                            decimal YearDays = Common.GetAvgDaysInAYear((DateTime)model.Appointmentstartdate, (DateTime)model.AppointmentEndDate);

                            var Years = Convert.ToDecimal(Days) / YearDays;

                            if (model.DesignationId == 1)
                            {
                                decimal permonth = Convert.ToDecimal(YearDays) / Convert.ToDecimal(12);
                                var monthDiff = Convert.ToDecimal(Days) / permonth;
                                monthDiff = Decimal.Round(monthDiff);
                                if (monthDiff > 6)
                                    msg = msg == "Valid" ? "Entered designation appointment tenure should be minimum 1 month to 6 months." : msg + "<br />Entered designation appointment tenure should be minimum 1 month to 6 months.";
                            }
                            else
                            {
                                if (Years > 1)
                                    msg = msg == "Valid" ? "Appointment tenure should be minimum 1 month to 1 year." : msg + "<br />Appointment tenure should be minimum 1 month to 1 year.";
                            }

                            DateTime StartDate = DateTime.Parse(_projectDetail.ProjectStartDate);
                            DateTime CloseDate = DateTime.Parse(_projectDetail.ProjectClosureDate);

                            if (StartDate > model.Appointmentstartdate || model.AppointmentEndDate > CloseDate)
                                msg = msg == "Valid" ? "Appointment tenure must be between the project start date and project closure date." : msg + "<br /> Appointment tenure must be between the project start date and project closure date.";

                            var now = DateTime.Now.Date.AddDays(-18);
                            if (model.TypeofappointmentId == 4 || (model.TypeofappointmentId == 3 && model.MsPhdType > 0))
                                now = DateTime.Now.Date.AddYears(-2).AddDays(+1);
                            if (now >= StartDate && now <= CloseDate)
                            {
                                if (queryste != null && queryste.AppointmentStartdate < now)
                                    now = queryste.AppointmentStartdate ?? now;
                            }
                            else
                            {
                                now = StartDate;
                            }
                            if (now > model.Appointmentstartdate || model.AppointmentEndDate > CloseDate)
                                msg = msg == "Valid" ? "Appointment tenure must be between the criteria." : msg + "<br /> Appointment tenure must be between the criteria.";
                        }
                        else
                            msg = msg == "Valid" ? "Enter appointment tenure." : msg + "<br /> Enter appointment tenure.";
                    }
                }
                else
                    msg = msg == "Valid" ? "Enter valid project number." : msg + "<br /> Enter valid project number.";

                if (model.ProfessionalId != null)
                {
                    if (model.ProfessionalId == 4)
                    {
                        if (model.EducationDetail != null)
                        {
                            if (!model.EducationDetail.Select(x => x.QualificationId).ToArray().Contains(3))
                                msg = msg == "Valid" ? "Please select Doctorate degree for Salutation Dr." : msg + "<br />Please select Doctorate degree for Salutation Dr.";
                        }
                    }
                }
            }
            return msg;
        }

        [HttpPost]
        public JsonResult STEEmailProcess(CheckDevationModel model)
        {
            EmailBuilder _eb = new EmailBuilder();
            NotePIModel templatemodel = new NotePIModel();
            using (var context = new IOASDBEntities())
            {
                templatemodel.PersonName = model.PersonName;
                templatemodel.AppointmentStartDate = String.Format("{0:ddd dd-MMMM-yyyy}", model.AppointmentStartDate);
                templatemodel.AppointmentEndDate = String.Format("{0:ddd dd-MMMM-yyyy}", model.AppointmentEndDate);
                templatemodel.DesignationName = RequirementService.getDesignationName(model.DesignationId ?? 0);
                templatemodel.TypeofAppointment = model.AppointmentType;
                templatemodel.Paytype = model.Paytype;
                templatemodel.ProjectNumber = Common.getprojectnumber(model.ProjectID ?? 0);
                templatemodel.BasicPay = Convert.ToString(model.BasicPay ?? 0);
                var user = Common.getUserIdAndRole(User.Identity.Name);
                templatemodel.DAName = Common.GetUserNameBasedonId(user.Item1);
            }
            var loadEmialView = _eb.RunCompile("RCTSTEAckTemplate.cshtml", "", templatemodel, typeof(NotePIModel));
            var result = new { output = loadEmialView };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult STERequestedBy(int Requestpi)
        {
            try
            {
                STEModel model = new STEModel();
                model = recruitmentService.STERequestedByPI(Requestpi);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                STEModel model = new STEModel();
                return Json("");
            }
        }
        public ActionResult STEView(int STEID=0, string listf = null)
        {
            try
            {
                STEViewModel model = new STEViewModel();
                model = recruitmentService.GetSTEView(STEID);
                var user = Common.getUserIdAndRole(User.Identity.Name);
                model.RoleId = user.Item2;
                if (model.FlowApprover == "CMAdmin")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STEAdminFlow", 0);
                else if (model.FlowApprover == "NDean")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STEFlowDean", 0);
                else
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STE Flow", 0);
                model.List_f = getEmployeeActionLink("STE", listf);
                return View(model);
            }
            catch (Exception ex)
            {
                STEViewModel model = new STEViewModel();
                WriteLog.SendErrorToText(ex);
                return View(model);
            }
        }

        public ActionResult STEViewProfile(int STEID, string listf = null)
        {
            try
            {
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                STEViewModel viewModel = new STEViewModel();
                if (STEID > 0)
                    viewModel = recruitmentService.GetSTEView(STEID);
                viewModel.List_f = getEmployeeActionLink("STE", listf);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                STEViewModel viewModel = new STEViewModel();
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                if (viewModel.EducationDetail.Count > 0)
                {
                    for (int i = 0; i < viewModel.EducationDetail.Count; i++)
                        viewModel.EducationDetail[i].DisiplineList = Common.GetCourseList(viewModel.EducationDetail[i].QualificationId ?? 0);
                }
                return View(viewModel);
            }
        }

        public ActionResult STEModifyProfile(int STEID)
        {
            STEModel model = new STEModel();
            try
            {
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Professional = Common.getProfessionalBasedId(STEID, "STE");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                ViewBag.List = new List<MasterlistviewModel>();
                model = recruitmentService.GetEditSTE(STEID);
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(model.DateofBirth.Value.Year);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Professional = Common.getProfessionalBasedId(STEID, "STE");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(model.DateofBirth.Value.Year);
                if (model.EducationDetail.Count > 0)
                {
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                        model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                }
                ViewBag.List = new List<MasterlistviewModel>();
                return View(model);
            }

        }

        [HttpPost]
        public ActionResult STEModifyProfile(STEModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Professional = Common.getProfessionalBasedId(model.STEId, "STE");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.List = new List<MasterlistviewModel>();
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(model.DateofBirth.Value.Year);

                if (model.EducationDetail.Count > 0)
                {
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                        model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                }

                if (model.aadharnumber != null)
                {
                    var chkemployeeadhar = Common.CheckPreviousEmployeeAdharserver(Convert.ToString(model.aadharnumber), model.ApplicationNo);
                    if (chkemployeeadhar != "")
                    {
                        TempData["alertMsg"] = chkemployeeadhar;
                        return View(model);
                    }
                }
                if (!string.IsNullOrEmpty(model.PAN))
                {
                    var chkemployeepanno = Common.CheckPreviousEmployeePanserver(model.PAN, model.ApplicationNo);
                    if (chkemployeepanno != "")
                    {
                        TempData["alertMsg"] = "This Pan Number is linked to  " + chkemployeepanno;
                        return View(model);
                    }
                }

                #region fileValidation

                if (model.PersonImage != null)//...........Canditates images
                {
                    var allowedExtension = new[] { ".jpeg", ".png", ".jpg", ".gif" };
                    var extension = Path.GetExtension(model.PersonImage.FileName);
                    if (!allowedExtension.Contains(extension.ToLower()))
                    {
                        TempData["alertMsg"] = "Please upload any one of these type doc [.jpeg,.png,.jpg]";
                        return View(model);
                    }
                    if (model.PersonImage.ContentLength > 1000000)
                    {
                        TempData["alertMsg"] = "You can upload candidate image up to 1MB";
                        return View(model);
                    }
                }

                if (model.CantidateSignature != null)//...........Canditates images
                {
                    var allowedExtension = new[] { ".jpeg", ".png", ".jpg", ".gif" };
                    var extension = Path.GetExtension(model.CantidateSignature.FileName);
                    if (!allowedExtension.Contains(extension.ToLower()))
                    {
                        TempData["alertMsg"] = "Please upload any one of these type doc [.jpeg,.png,.jpg]";
                        return View(model);
                    }
                    if (model.CantidateSignature.ContentLength > 1000000)
                    {
                        TempData["alertMsg"] = "You can upload candidate image up to 1MB";
                        return View(model);
                    }
                }

                if (model.EducationDetail != null)
                {
                    var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                    {
                        if (model.EducationDetail[i].Certificate != null)//...........Education Certificates
                        {
                            string extension = Path.GetExtension(model.EducationDetail[i].Certificate.FileName);
                            if (!allowedExtensionsCer.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type doc [.jpeg,.png,.jpg,.gif,.pdf]";
                                return View(model);
                            }
                            if (model.EducationDetail[i].Certificate.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload certificate up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }

                if (model.ExperienceDetail != null)
                {
                    for (int i = 0; i < model.ExperienceDetail.Count; i++)
                    {
                        var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                        if (model.ExperienceDetail[i].ExperienceFile != null)//...........Experience Certificates
                        {
                            string filename = System.IO.Path.GetFileName(model.ExperienceDetail[i].ExperienceFile.FileName);
                            string extension = Path.GetExtension(filename);
                            if (!allowedExtensionsCer.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type doc [.jpeg,.png,.jpg,.gif,.pdf]";
                                return View(model);
                            }
                            if (model.ExperienceDetail[i].ExperienceFile.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload experience certificate up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }

                if (model.OtherDocList != null)
                {
                    var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                    for (int i = 0; i < model.OtherDocList.Count; i++)
                    {
                        if (model.OtherDocList[i].Document != null)//...........Other Documents
                        {
                            string filename = System.IO.Path.GetFileName(model.OtherDocList[i].Document.FileName);
                            string extension = Path.GetExtension(filename);
                            if (!allowedExtensionsCer.Contains(extension.ToLower()))
                            {
                                TempData["errMsg"] = "Please upload other document any one of these type doc [.jpeg,.png,.jpg,.gif,.pdf]";
                                return View(model);
                            }
                            if (model.OtherDocList[i].Document.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload experience certificate up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }

                #endregion

                var result = recruitmentService.STEModifyProfile(model, userId);
                if (result == 1)
                {
                    TempData["succMsg"] = "Modified Successfully";
                    return RedirectToAction("STEEmployeeMaster", "Requirement");
                }
                else if (result == -1)
                {
                    TempData["succMsg"] = "Record not found";
                    return RedirectToAction("STEEmployeeMaster", "Requirement");
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator";
                    return RedirectToAction("STEEmployeeMaster", "Requirement");
                }
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Professional = Common.getProfessionalBasedId(model.STEId, "STE");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.List = new List<MasterlistviewModel>();
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(model.DateofBirth.Value.Year);
                if (model.EducationDetail.Count > 0)
                {
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                        model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                }
                return View(model);
            }
        }

        #region STE Verfication

        public ActionResult STEVerficationList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetSTEVerficationList(int pageIndex, int pageSize, SearchSTEVerificationModel model, DateFilterModel OfferDate, DateFilterModel strAnnouncementClosureDate, DateFilterModel DateOfJoining)
        {
            try
            {
                object output = recruitmentService.GetSTEVerificationList(pageIndex, pageSize, model, OfferDate, strAnnouncementClosureDate, DateOfJoining);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }

        public ActionResult STEVerification(int STEID)
        {
            STEVerificationModel model = new STEVerificationModel();
            try
            {
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                if (validateSTEVerification(STEID))
                {
                    model = recruitmentService.GetVerification(STEID);
                    if (model.Status == "Awaiting Verification-Open")
                    {
                        var user = Common.getUserIdAndRole(User.Identity.Name);
                        model.RoleId = user.Item2;
                        if (model.FlowApprover == "CMAdmin")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(188, "STEVERAdminFlow", 0);
                        else if (model.FlowApprover == "NDean")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(188, "STEVERFlowDean", 0);
                        else
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(188, "STEVER Flow", 0);
                        model.List_f = getEmployeeActionLink("STE", "VAL");
                        //ViewBag.processGuideLineId = processGuideLineId;
                        ViewBag.currentRefId = model.STEId;



                    }
                }
                else
                {
                    TempData["alertMsg"] = "Date of joining should be only within the tenure.";
                    return RedirectToAction("STEVerficationList", "Requirement");
                }
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(Convert.ToInt32(model.DateofBirth.Split('-')[2]));
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult STEVerification(STEVerificationModel model)
        {
            try
            {
                if (!validateSTEVerification(model.STEId ?? 0))
                {
                    TempData["alertMsg"] = "Date of joining should be only within the tenure.";
                    return RedirectToAction("STEVerficationList", "Requirement");
                }
                var button = Request["button"];
                if (button.Contains("Save as drafts"))
                    button = button == null ? "" : button.Split(',')[1];
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(Convert.ToInt32(model.DateofBirth.Split('-')[2]));

                #region UpdateDisiplineList
                if (model.EducationDetail.Count > 0)
                {
                    foreach (var item in model.EducationDetail)
                        item.DisiplineList = Common.GetCourseList(item.QualificationId ?? 0);
                }
                #endregion


                //if (model.STEId > 0)
                //{
                //    if (model.aadharnumber != null)
                //    {
                //        var chkemployeeadhar = Common.CheckPreviousEmployeeAdharserver(Convert.ToString(model.aadharnumber), model.ApplicationNo, true, model.OldEmployeeNumber, "STE");
                //        if (chkemployeeadhar != "")
                //        {
                //            TempData["alertMsg"] = chkemployeeadhar;
                //            model.Status = model.Status == null ? "" : model.Status;
                //            return View(model);
                //        }
                //    }
                //    if (!string.IsNullOrEmpty(model.PAN))
                //    {
                //        var chkemployeepanno = Common.CheckPreviousEmployeePanserver(model.PAN, model.ApplicationNo, true, model.OldEmployeeNumber, "STE");
                //        if (chkemployeepanno != "")
                //        {
                //            TempData["alertMsg"] = chkemployeepanno;
                //            model.Status = model.Status == null ? "" : model.Status;
                //            return View(model);
                //        }
                //    }
                //}
                //else
                //{
                //    if (model.aadharnumber != null)
                //    {
                //        var chkemployeeadhar = Common.CheckPreviousEmployeeAdharserver(Convert.ToString(model.aadharnumber), null, true, model.OldEmployeeNumber, "STE");
                //        if (chkemployeeadhar != "")
                //        {
                //            TempData["errMsg"] = chkemployeeadhar;
                //            model.Status = model.Status == null ? "" : model.Status;
                //            return View(model);
                //        }
                //    }
                //    if (!string.IsNullOrEmpty(model.PAN))
                //    {
                //        var chkemployeepanno = Common.CheckPreviousEmployeePanserver(model.PAN, null, true, model.OldEmployeeNumber, "STE");
                //        if (chkemployeepanno != "")
                //        {
                //            TempData["errMsg"] = "This Pan Number is linked to  " + chkemployeepanno;
                //            model.Status = model.Status == null ? "" : model.Status;
                //            return View(model);
                //        }
                //    }
                //}

                if (button != "Save as drafts")
                {
                    bool isHaveExperience = RequirementService.checkIsHaveExperience(model.STEId ?? 0, "STE");

                    #region FileValidation
                    if (model.JoiningReport != null)//...........Canditates images
                    {
                        var allowedExtensionsDoc = new[] { ".doc", ".docx", ".pdf" };
                        var extension = Path.GetExtension(model.JoiningReport.FileName);
                        if (!allowedExtensionsDoc.Contains(extension.ToLower()))
                        {
                            TempData["alertMsg"] = "Please upload joining report any one of these type doc [.doc,.docx,.pdf]";
                            return View(model);
                        }
                    }

                    if (model.Resume != null)
                    {
                        var extension = Path.GetExtension(model.Resume.FileName);
                        if (extension.ToLower() != ".pdf")
                        {
                            TempData["alertMsg"] = "Please upload resume any one of these type doc [.pdf]";
                            return View(model);
                        }
                    }
                    if (model.PersonImage != null)//...........Canditates images
                    {
                        var allowedExtension = new[] { ".jpeg", ".png", ".jpg", ".gif" };
                        var extension = Path.GetExtension(model.PersonImage.FileName);
                        if (!allowedExtension.Contains(extension.ToLower()))
                        {
                            TempData["alertMsg"] = "Please upload any one of these type doc [.jpeg,.png,.jpg]";
                            return View(model);
                        }
                        if (model.PersonImage.ContentLength > 1000000)
                        {
                            TempData["alertMsg"] = "You can upload candidate image up to 1MB";
                            return View(model);
                        }
                    }

                    if (model.CantidateSignature != null)//...........Canditates images
                    {
                        var allowedExtension = new[] { ".jpeg", ".png", ".jpg", ".gif" };
                        var extension = Path.GetExtension(model.CantidateSignature.FileName);
                        if (!allowedExtension.Contains(extension.ToLower()))
                        {
                            TempData["alertMsg"] = "Please upload any one of these type doc [.jpeg,.png,.jpg]";
                            return View(model);
                        }
                        if (model.CantidateSignature.ContentLength > 1000000)
                        {
                            TempData["alertMsg"] = "You can upload candidate image up to 1MB";
                            return View(model);
                        }
                    }

                    using (var context = new IOASDBEntities())
                    {
                        if (model.EducationDetail != null)
                        {
                            var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                            for (int i = 0; i < model.EducationDetail.Count; i++)
                            {
                                var EduId = model.EducationDetail[i].EducationId ?? 0;
                                if (context.tblRCTSTEEducationDetail.Any(m => m.STEEducationDetailID == EduId && string.IsNullOrEmpty(m.DocumentFilePath)) && model.EducationDetail[i].Certificate == null)
                                {
                                    TempData["alertMsg"] = "Please upload education certificates any one of these type doc [.jpeg,.png,.jpg,.gif,.pdf]";
                                    return View(model);
                                }
                                else
                                {
                                    if (model.EducationDetail[i].Certificate != null)//...........Education Certificates
                                    {
                                        string extension = Path.GetExtension(model.EducationDetail[i].Certificate.FileName);
                                        if (!allowedExtensionsCer.Contains(extension.ToLower()))
                                        {
                                            TempData["alertMsg"] = "Please upload any one of these type doc [.jpeg,.png,.jpg,.gif,.pdf]";
                                            return View(model);
                                        }
                                        if (model.EducationDetail[i].Certificate.ContentLength > 5242880)
                                        {
                                            TempData["alertMsg"] = "You can upload certificate up to 5MB";
                                            return View(model);
                                        }
                                    }
                                }

                            }
                        }

                        if (model.ExperienceDetail != null)
                        {
                            for (int i = 0; i < model.ExperienceDetail.Count; i++)
                            {
                                var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                                var ExpId = model.ExperienceDetail[i].ExperienceId ?? 0;

                                if (context.tblRCTSTEExperienceDetail.Any(m => m.STEExperienceDetailID == ExpId && string.IsNullOrEmpty(m.DocumentFilePath)) && model.ExperienceDetail[i].ExperienceFile == null && isHaveExperience)
                                {
                                    TempData["alertMsg"] = "Please upload experience certificates any one of these type doc [.jpeg,.png,.jpg,.gif,.pdf]";
                                    return View(model);
                                }
                                else
                                {
                                    if (model.ExperienceDetail[i].ExperienceFile != null)//...........Experience Certificates
                                    {
                                        string filename = System.IO.Path.GetFileName(model.ExperienceDetail[i].ExperienceFile.FileName);
                                        string extension = Path.GetExtension(filename);
                                        if (!allowedExtensionsCer.Contains(extension.ToLower()))
                                        {
                                            TempData["alertMsg"] = "Please upload any one of these type doc [.jpeg,.png,.jpg,.gif,.pdf]";
                                            return View(model);
                                        }
                                        if (model.ExperienceDetail[i].ExperienceFile.ContentLength > 5242880)
                                        {
                                            TempData["alertMsg"] = "You can upload experience certificate up to 5MB";
                                            return View(model);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (model.OtherDocList != null)
                    {
                        var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                        for (int i = 0; i < model.OtherDocList.Count; i++)
                        {
                            if (model.OtherDocList[i].Document != null)//...........Other Documents
                            {
                                string filename = System.IO.Path.GetFileName(model.OtherDocList[i].Document.FileName);
                                string extension = Path.GetExtension(filename);
                                if (!allowedExtensionsCer.Contains(extension.ToLower()))
                                {
                                    TempData["errMsg"] = "Please upload other document any one of these type doc [.jpeg,.png,.jpg,.gif,.pdf]";
                                    return View(model);
                                }
                                if (model.OtherDocList[i].Document.ContentLength > 5242880)
                                {
                                    TempData["alertMsg"] = "You can upload experience certificate up to 5MB";
                                    return View(model);
                                }
                            }
                        }
                    }

                    #endregion

                    var result = recruitmentService.VerifySTE(model, userId, button);
                    if (result.Item1 == 1)
                    {
                        //TempData["succMsg"] = "Application verified / Employee number generated:" + result.Item2;
                        TempData["succMsg"] = "Application verified";
                        return RedirectToAction("STEVerficationList", "Requirement");
                    }
                    else if (result.Item1 == 2)
                    {
                        TempData["succMsg"] = "Already verified";
                        return RedirectToAction("STEVerficationList", "Requirement");
                    }
                    else if (result.Item1 == -1)
                    {
                        TempData["errMsg"] = "Record not found";
                        return RedirectToAction("STEVerficationList", "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("STEVerficationList", "Requirement");
                    }
                }
                else if (button == "Save as drafts")
                {
                    var result = recruitmentService.VerifySTE(model, userId, button);

                    if (result.Item1 == 1)
                    {
                        TempData["succMsg"] = "Draft Saved Successfully";
                        return RedirectToAction("STEVerficationList", "Requirement");
                    }
                    else if (result.Item1 == 2)
                    {
                        TempData["succMsg"] = "Draft updated";
                        return RedirectToAction("STEVerficationList", "Requirement");
                    }
                    else if (result.Item1 == -1)
                    {
                        //TempData["errMsg"] = result.Item3;
                        return RedirectToAction("STEVerficationList", "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("STEList", "Requirement");
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
                WriteLog.SendErrorToText(ex);
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(Convert.ToInt32(model.DateofBirth.Split('-')[2]));
                #region UpdateDisiplineList
                if (model.EducationDetail.Count > 0)
                {
                    foreach (var item in model.EducationDetail)
                        item.DisiplineList = Common.GetCourseList(item.QualificationId ?? 0);
                }
                #endregion
                return View(model);
            }
        }



        public ActionResult STEVerificationView(int STEID, string listf = null)
        {
            try
            {
                STEVerificationModel model = new STEVerificationModel();
                model = recruitmentService.GetVerification(STEID);
                var user = Common.getUserIdAndRole(User.Identity.Name);
                model.RoleId = user.Item2;
                if (model.FlowApprover == "CMAdmin")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(188, "STEVERAdminFlow", 0);
                else if (model.FlowApprover == "NDean")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(188, "STEVERFlowDean", 0);
                else
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(188, "STEVER Flow", 0);
                model.List_f = getEmployeeActionLink("STE", listf);
                return View(model);



            }
            catch (Exception ex)
            {
                STEVerificationModel model = new STEVerificationModel();
                WriteLog.SendErrorToText(ex);
                return View(model);
            }
        }
        public ActionResult OSGVerificationView(int OSGID, string listf = null)
        {
            try
            {
                STEVerificationModel model = new STEVerificationModel();

                model = recruitmentService.GetOSGVerification(OSGID);
                var user = Common.getUserIdAndRole(User.Identity.Name);
                model.RoleId = user.Item2;
                if (model.FlowApprover == "CMAdmin")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(206, "OSGVERAdminFlow", 0);
                else if (model.FlowApprover == "NDean")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(206, "OSGVERFlowDean", 0);
                else
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(206, "OSGVER Flow", 0);
                model.List_f = getEmployeeActionLink("STE", listf);
                return View(model);
            }
            catch (Exception ex)
            {
                STEVerificationModel model = new STEVerificationModel();
                WriteLog.SendErrorToText(ex);
                return View(model);
            }
        }

        public bool validateSTEVerification(int STEID)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var curr = DateTime.Now.Date;
                    return context.tblRCTSTE.Any(m => m.STEID == STEID && m.AppointmentStartdate <= curr && m.AppointmentEnddate >= curr);
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #endregion

        #region Mail functions

        [HttpPost]
        public JsonResult SendPIReminderEmail(int appid, string apptype, int? orderid = null, bool isgetbody_f = false)
        {
            string user_logged_in = User.Identity.Name;
            int UserId = Common.GetUserid(user_logged_in);
            object output = RCTEmailContentService.SendPIReminderMail(appid, apptype, orderid, isgetbody_f);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendVerificationReminder(int AppId, string Apptype, bool isBody = false, int? OrderId = null)
        {
            string user_logged_in = User.Identity.Name;
            int UserId = Common.GetUserid(user_logged_in);
            if (OrderId == 0)
                OrderId = null;
            object output = RCTEmailContentService.SendVerificationReminder(AppId, Apptype, isBody, OrderId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendOfferOrder(int appid, string apptype, string offercategory, bool isgetbody_f = false, int? orderid = null)
        {
            string user_logged_in = User.Identity.Name;
            int UserId = Common.GetUserid(user_logged_in);
            if (orderid == 0)
                orderid = null;
            object output = "";
            if (offercategory == "OfferLetter")
            {
                if (apptype == "STE")
                    output = RCTEmailContentService.SendMailForOfferLetter(appid, apptype, isgetbody_f, orderid);
                else if (apptype == "CON")
                    output = RCTEmailContentService.SendCONOfferLetter(appid, isgetbody_f, orderid);
                else
                    output = RCTEmailContentService.SendOSGOfferReleaseMail(appid, UserId, isgetbody_f, orderid);
            }
            else if (offercategory == "OfficeOrder")
            {
                if (apptype == "OSG")
                    output = RCTEmailContentService.SendOSGAppointmentorderMail(appid, UserId, offercategory, isgetbody_f, orderid);
                else if (apptype == "CON")
                    output = RCTEmailContentService.SendCONOfficeOrder(appid, isgetbody_f, orderid);
                else
                    output = RCTEmailContentService.SendMailForOfficeOrder(appid, apptype, isgetbody_f, orderid);
            }
            else if (offercategory == "Cancel")
                output = RCTEmailContentService.SendMailForCancelApp(appid, apptype, UserId, isgetbody_f, orderid);
            else if (offercategory == "Order")
            {
                if (apptype == "OSG")
                    output = RCTEmailContentService.SendOSGAppointmentorderMail(appid, UserId, offercategory, isgetbody_f, orderid);
                else if (apptype == "CON")
                    output = RCTEmailContentService.SendCONOrder(orderid ?? 0, UserId, isgetbody_f);
                else
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.tblOrder.Where(x => x.OrderId == orderid).Select(x => x.OrderType).FirstOrDefault();
                        if (query == 5 || query == 6)
                            output = RCTEmailContentService.SendMailForHRA(orderid ?? 0, UserId, false, true, isgetbody_f);
                        else if (query == 9)
                            output = RCTEmailContentService.SendRelievingOrder(orderid ?? 0, UserId, isgetbody_f);
                        else
                            output = RCTEmailContentService.SendOrder(orderid ?? 0, UserId, isgetbody_f);
                    }
                }
            }
            System.Reflection.PropertyInfo pi1 = output.GetType().GetProperty("Item1");
            int Item1 = (int)(pi1.GetValue(output, null));
            System.Reflection.PropertyInfo pi2 = output.GetType().GetProperty("Item2");
            string Item2 = (string)(pi2.GetValue(output, null));
            if (Item1 == 0 && string.IsNullOrEmpty(Item2))
                output = Tuple.Create(Item1, "Mail not sent. Please contact admin");
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Cancel Application

        public ActionResult CancelRequestList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetCancelRequestList(ApplicationSearchListModel model, int pageIndex, int pageSize)
        {
            try
            {
                object output = RequirementService.GetCancelRequestList(model, pageIndex, pageSize);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult CancelApplicationView(int appid, string apptype, int? orderid = null)
        {
            try
            {
                STEViewModel model = new STEViewModel();
                int UserId = Common.GetUserid(User.Identity.Name);
                model = recruitmentService.GetOverAllApplicationViewDetails(appid, apptype, orderid ?? 0);
                return View(model);
            }
            catch (Exception e)
            {
                WriteLog.SendErrorToText(e);
                STEViewModel model = new STEViewModel();
                return View(model);
            }

        }

        [HttpPost]
        public JsonResult ApproveCancelApplication(int appid, string apptype, int? orderid = null)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                int UserId = Common.GetUserid(user_logged_in);
                orderid = orderid == 0 ? null : orderid;
                object output = RequirementService.ApproveCancelRequest(appid, apptype, UserId, orderid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                WriteLog.SendErrorToText(e);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RequestCancelApplication(int appid, string apptype, string reason, int? orderid = null, HttpPostedFileBase attachement = null, bool? backdate_f = null)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                string docName = string.Empty;
                int UserId = Common.GetUserid(user_logged_in);
                if (attachement != null)
                {
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                    var extension = Path.GetExtension(attachement.FileName);
                    if (!allowedExtensions.Contains(extension.ToLower()))
                        return Json("Attachment allowed document type [pdf,doc,docx]", JsonRequestBehavior.AllowGet);
                    if (attachement.ContentLength > 5242880)
                        return Json("You can upload upto 5MB", JsonRequestBehavior.AllowGet);
                    var guid = Guid.NewGuid().ToString();
                    docName = guid + "_" + attachement.FileName;
                    attachement.UploadFile("Requirement", docName);
                }
                var Valid = "Valid";
                if (appid > 0 && !string.IsNullOrEmpty(apptype))
                {
                    Valid = ValidateCancelApplications(appid, apptype, orderid, backdate_f);
                    if (Valid != "Valid")
                        return Json(Valid, JsonRequestBehavior.AllowGet);
                }
                object output = RequirementService.RequestCancelApplications(appid, apptype, orderid, reason, docName, UserId);
                if (output.Equals(0))
                    return Json("Application already cancelled or cancelled initiated", JsonRequestBehavior.AllowGet);
                else if (output.Equals(1))
                    return Json("Application cancelled & cancellation notification sent to PI", JsonRequestBehavior.AllowGet);
                else if (output.Equals(2))
                    return Json("Cancel request placed successfully", JsonRequestBehavior.AllowGet);

                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                WriteLog.SendErrorToText(e);
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }

        public static string ValidateCancelApplications(int appid, string apptype, int? orderid = null, bool? backdate_f = null)
        {
            try
            {
                if (orderid == 5006 || orderid == 8892)
                    return "Valid";
                if (backdate_f == true)
                    return "Valid";

                using (var context = new IOASDBEntities())
                {
                    if (appid > 0 && !string.IsNullOrEmpty(apptype))
                    {
                        int apptypeId = RequirementService.getAppointmentType(apptype);
                        DateTime curr = DateTime.Now.Date;
                        if (orderid > 0)
                        {
                            string[] notexpstatus = new string[] { "Rejected", "Canceled", "Cancel" };

                            var queryorder = (from c in context.tblOrder
                                              where c.AppointmentId == appid && c.AppointmentType == apptypeId && c.OrderId > orderid
                                              && !notexpstatus.Contains(c.Status) && c.isUpdated != true
                                              orderby c.OrderId descending
                                              select c).FirstOrDefault();
                            if (queryorder != null)
                                return "Order pending against the Employee. To proceed, please cancel the pending order (" + queryorder.OrderNo + ").";
                            if (context.tblOrder.Any(m => m.OrderId == orderid && m.Status == "Sent for approval"))
                                return "Application is available with HR Admin. Do not cancel the order";
                            if (context.tblOrder.Any(m => m.OrderId == orderid && m.Status == "Awaiting Commitment Booking"))
                                return "Application is available with commitment booking. Do not cancel the order";
                            if (context.tblOrder.Any(m => m.OrderId == orderid && m.Status == "Awaiting Committee Approval"))
                                return "Application is available with committee approval. Do not cancel the order";
                            if (context.tblOrder.Any(m => m.AppointmentId == appid && m.AppointmentType == apptypeId && m.OrderId == orderid && m.Status == "Completed" && m.isUpdated == true && ((m.FromDate.Value.Year == curr.Year && m.FromDate.Value.Month == curr.Month) || m.FromDate >= curr)))
                                return "Valid";
                            if (context.tblOrder.Any(m => m.AppointmentId == appid && m.AppointmentType == apptypeId && m.OrderId == orderid && m.Status == "Completed" && m.isUpdated == true && m.FromDate < curr))
                                return "Appointment tenure started. Order cannot be cancelled";
                        }
                        else
                        {
                            string[] notexpstatus = new string[] { "Rejected", "Canceled", "Cancel" };

                            if (context.vw_RCTOverAllApplicationEntry.Any(m => m.ApplicationId == appid && m.Category == apptype && m.ApplicationType == "New" && m.Status == "Sent for approval"))
                                return "Application is available with HR Admin. Do not cancel the application.";
                            if (context.vw_RCTOverAllApplicationEntry.Any(m => m.ApplicationId == appid && m.Category == apptype && m.ApplicationType == "New" && m.Status == "Awaiting Commitment Booking"))
                                return "Application is available with Commitment booking. Do not cancel the application.";
                            if (context.vw_RCTOverAllApplicationEntry.Any(m => m.ApplicationId == appid && m.Category == apptype && m.ApplicationType == "New" && m.Status == "Awaiting Committee Approval"))
                                return "Application is available with committee approval. Do not cancel the application.";

                            if (context.tblOrder.Any(m => m.AppointmentId == appid && m.AppointmentType == apptypeId && !notexpstatus.Contains(m.Status) && m.isUpdated != true))
                                return "Order pending against the Employee. To proceed, please cancel the pending order.";
                            if (context.tblOrder.Any(m => m.AppointmentId == appid && m.AppointmentType == apptypeId && m.Status == "Completed" && m.isUpdated == true))
                                return "Offer cannot be cancelled as new order tenure is started.";
                            if (context.vw_RCTOverAllApplicationEntry.Any(m => m.ApplicationId == appid && m.Category == apptype && m.ApplicationType == "New" && m.isEmployee == true && ((m.AppointmentStartdate.Value.Year == curr.Year && m.AppointmentStartdate.Value.Month == curr.Month) || m.AppointmentStartdate > curr)))
                                return "Valid";
                            if (context.vw_RCTOverAllApplicationEntry.Any(m => m.ApplicationId == appid && m.Category == apptype && m.ApplicationType == "New" && m.isEmployee == true && m.AppointmentStartdate < curr))
                                return "Appointment tenure started.";
                        }
                    }
                }
                return "Valid";
            }
            catch (Exception ex)
            {
                return "Please contact administrator";
            }
        }

        #endregion

        #region Tenure Closure 

        public ActionResult TenureClosureList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetTenureClosureList(int pageIndex, int pageSize, SearchOrderModel model, DateFilterModel ToDate)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = RequirementService.GetTenureClosureList(model, pageIndex, pageSize, ToDate, userid, roleid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult SendTermEndMail(int Appid, string Apptype)
        {
            string user_logged_in = User.Identity.Name;
            int UserId = Common.GetUserid(user_logged_in);
            object output = RCTEmailContentService.SendTermEndMail(Appid, Apptype, UserId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region OrderVerification

        public ActionResult OrderVerificationView(int OrderId, string listf = null)
        {

            try
            {
                STEVerificationModel model = new STEVerificationModel();
                model = recruitmentService.GetOrderVerification(OrderId);
                var user = Common.getUserIdAndRole(User.Identity.Name);
                model.RoleId = user.Item2;
                if (model.ApplicationType == "STE")
                {
                    //if (model.FlowApprover == "CMAdmin")
                    //    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(188, "STEORDVERAdminFlow", 0);
                    //else if (model.FlowApprover == "NDean")
                    //    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(188, "STEORDVERFlowDean", 0);
                    //else
                    //    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(188, "STEORDVER Flow", 0);
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(188, "STEORDVER Flow", 0);
                    model.List_f = getEmployeeActionLink("STE", listf);
                    ViewBag.currentRefId = model.STEId;

                }
                else if (model.ApplicationType == "OSG")
                {
                    //if (model.FlowApprover == "CMAdmin")
                    //    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(206, "OSGORDVERAdminFlow", 0);
                    //else if (model.FlowApprover == "NDean")
                    //    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(206, "OSGORDVERFlowDean", 0);
                    //else
                    //    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(206, "OSGORDVER Flow", 0);
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(206, "OSGORDVER Flow", 0);
                    model.List_f = getEmployeeActionLink("STE", listf);
                    ViewBag.currentRefId = model.OrderId;

                }
                return View(model);

            }
            catch (Exception ex)
            {
                STEVerificationModel model = new STEVerificationModel();
                WriteLog.SendErrorToText(ex);
                return View(model);
            }
        }

        public ActionResult OrderVerification(int OrderId)
        {
            STEVerificationModel model = new STEVerificationModel();
            try
            {
                if (OrderId > 0)
                {
                    if (validateOrderVerification(OrderId))
                    {
                        model = recruitmentService.GetOrderVerification(OrderId);
                        if (model.ApplicationType == "STE")
                        {

                            if (model.Status == "Awaiting Verification-Open")
                            {
                                var user = Common.getUserIdAndRole(User.Identity.Name);
                                model.RoleId = user.Item2;
                                if (model.FlowApprover == "CMAdmin")
                                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(188, "STEVERAdminFlow", 0);
                                else if (model.FlowApprover == "NDean")
                                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(188, "STEVERFlowDean", 0);
                                else
                                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(188, "STEVER Flow", 0);
                                model.List_f = getEmployeeActionLink("STE", "OVAL");
                                //ViewBag.processGuideLineId = processGuideLineId;
                                ViewBag.currentRefId = model.STEId;

                            }
                        }
                        else if (model.ApplicationType == "OSG")
                        {
                            if (model.Status == "Awaiting Verification-Open")
                            {
                                var user = Common.getUserIdAndRole(User.Identity.Name);
                                model.RoleId = user.Item2;
                                if (model.FlowApprover == "CMAdmin")
                                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(206, "OSGORDVERAdminFlow", 0);
                                else if (model.FlowApprover == "NDean")
                                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(206, "OSGORDVERFlowDean", 0);
                                else
                                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(206, "OSGORDVER Flow", 0);
                                model.List_f = getEmployeeActionLink("STE", "OSGORVAL");
                                //ViewBag.processGuideLineId = processGuideLineId;
                                ViewBag.currentRefId = model.OrderId;

                            }
                        }


                    }
                    else
                    {
                        int Apptype = Common.GetAppointmentType(OrderId);
                        TempData["alertMsg"] = "Date of joining should be only within the tenure.";
                        string action = Apptype == 1 ? "CONVerificationList" : Apptype == 2 ? "STEVerficationList" : Apptype == 3 ? "OSGVerificationList" : "";
                        return RedirectToAction(action, "Requirement");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult OrderVerification(STEVerificationModel model)
        {
            try
            {
                var button = Request["button"];
                if (button.Contains("Save as drafts"))
                    button = button == null ? "" : button.Split(',')[1];
                int Apptype = Common.GetAppointmentType(model.OrderId ?? 0);
                string action = Apptype == 1 ? "CONVerificationList" : Apptype == 2 ? "STEVerficationList" : Apptype == 3 ? "OSGVerificationList" : "";
                int userId = Common.GetUserid(User.Identity.Name);
                //if (button == "Save as drafts")
                if (button != "Save as drafts")
                {
                    if (model.JoiningReport != null)
                    {
                        string filename = System.IO.Path.GetFileName(model.JoiningReport.FileName);
                        string extension = Path.GetExtension(filename);
                        if (extension.ToLower() != ".pdf")
                        {
                            TempData["errMsg"] = "Please upload other document any one of these type doc [.pdf]";
                            return View(model);
                        }
                    }

                    if (!validateOrderVerification(model.OrderId ?? 0))
                    {
                        TempData["alertMsg"] = "Date of joining should be only within the tenure.";
                        return RedirectToAction(action, "Requirement");
                    }
                    RequirementService requirement = new RequirementService();
                    var result = requirement.UpdateVerificationOrder(model, userId, button);
                    if (result.Item1 == 1 && model.OrderId > 0)
                    {
                        TempData["succMsg"] = "Order verified successfully";
                        return RedirectToAction(action, "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction(action, "Requirement");
                    }

                }
                else
                {
                    RequirementService requirement = new RequirementService();
                    var result = requirement.UpdateVerificationOrder(model, userId, button);
                    if (result.Item1 == 1 && model.OrderId > 0)
                    {

                        TempData["succMsg"] = "Order verified-Draft successfully";
                        return RedirectToAction(action, "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction(action, "Requirement");
                    }

                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        public bool validateOrderVerification(int OrderId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var curr = DateTime.Now.Date;
                    return context.tblOrder.Any(m => m.OrderId == OrderId && m.FromDate <= curr && m.ToDate >= curr);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region Employee Master

        [HttpPost]
        public JsonResult getPreviousOrderId(int appid, string apptype, int ordertype)
        {
            try
            {
                var res = recruitmentService.getEditOrderId(appid, apptype, ordertype);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                WriteLog.SendErrorToText(e);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        #region CheckDeviation & Acknowledgement

        [HttpPost]
        public JsonResult CheckDeviationQualfication(CheckDevationModel model)
        {
            List<CheckListModel> listmodel = new List<CheckListModel>();
            List<CheckListEmailModel> checkdetails = new List<CheckListEmailModel>();
            NotePIModel emailModel = new NotePIModel();
            EmailBuilder _eb = new EmailBuilder();
            string username = User.Identity.Name;
            var user = Common.getUserIdAndRole(username);
            var Data = Common.GetDeviationofAppointments(model);
            listmodel = Data.Item1;
            if (listmodel.Count > 0)
            {
                for (int i = 0; i < listmodel.Count; i++)
                {
                    var sno = i + 1;
                    checkdetails.Add(new CheckListEmailModel()
                    {
                        CheckList = listmodel[i].CheckList,
                        checklistId = listmodel[i].FunctionCheckListId
                    });
                }
            }
            model.devChecklist = checkdetails;
            using (var context = new IOASDBEntities())
            {
                string designation = context.tblRCTDesignation.FirstOrDefault(m => m.DesignationId == model.DesignationId).Designation;
                emailModel.AppointmentType = model.AppointmentType;
                emailModel.checkdetails = RCTEmailContentService.getDevNormsDetails(model);
                emailModel.DesignationName = designation;
                emailModel.PersonName = model.PersonName;
                emailModel.Comments = model.Comments;
                emailModel.DAName = Common.GetUserNameBasedonId(user.Item1);
                emailModel.BasicPay = Convert.ToString(model.ChekSalary);
                emailModel.IsDeviation = true;
                emailModel.SendSlryStruct = model.SendSalaryStructure;
                int ProjectID = model.ProjectID ?? 0;
                var qryProject = (from prj in context.tblProject
                                  where prj.ProjectId == ProjectID
                                  select prj).FirstOrDefault();
                if (qryProject != null)
                {
                    emailModel.ProjectNumber = qryProject.ProjectNumber;
                    emailModel.ProjectTitle = qryProject.ProjectTitle;
                }
            }
            var loadEmialView = Tuple.Create(false, "", "");
            if (model.AppType == "STE")
                loadEmialView = _eb.RunCompile("RCTSTEDevTemplate.cshtml", "", emailModel, typeof(NotePIModel));
            else if (model.AppType == "OSG")
                loadEmialView = _eb.RunCompile("RCTOSGApplicationack.cshtml", "", emailModel, typeof(NotePIModel));
            else
                loadEmialView = _eb.RunCompile("NotePIProcess.cshtml", "", emailModel, typeof(NotePIModel));

            var result = new { output = ConvertViewToString("_DeviationCheckListDetail", listmodel), isRes = Data.Item2, template = loadEmialView.Item2 };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult OrderEmailProcess(CheckDevationModel model)
        {
            var UserName = User.Identity.Name;
            var UserId = Common.GetUserid(UserName);
            EmailBuilder _eb = new EmailBuilder();
            NotePIModel npmodel = new NotePIModel();
            using (var context = new IOASDBEntities())
            {
                var query = (from vw in context.vw_RCTOverAllApplicationEntry
                             where vw.ApplicationId == model.AppId && vw.Category == model.AppType
                             && vw.ApplicationType == "New"
                             select new { vw.TypeofAppointment, vw.PostRecommended, vw.ConsolidatedPay, vw.Fellowship, vw.isMsPhd, vw.BasicPay, vw.DesignationId, vw.ProjectId, vw.AppointmentEnddate }).FirstOrDefault();
                npmodel.AppointmentType = query.TypeofAppointment;
                npmodel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", model.AppointmentStartDate);
                npmodel.AppointmentEndDate = String.Format("{0:dd-MMMM-yyyy}", model.AppointmentEndDate);
                npmodel.PersonName = model.PersonName;
                npmodel.ProjectNumber = Common.getprojectnumber(model.ProjectID ?? 0);
                if (model.AppType == "OSG")
                    npmodel.Ordertype = model.OrderType;
                else
                    npmodel.Ordertype = model.OrderType.ToLower();
                npmodel.DAName = Common.GetUserNameBasedonId(UserId);
                if (model.OrderType == "Enhancement")
                    npmodel.DesignationName = RequirementService.getDesignationName(model.DesignationId ?? 0);
                else
                    npmodel.DesignationName = query.PostRecommended;
                if (model.OrderType == "Enhancement")
                    npmodel.Paytype = model.Paytype;
                else
                    npmodel.Paytype = query.ConsolidatedPay == true ? "Consolidated pay" : query.Fellowship == true ? "Fellowship pay" : "";
                npmodel.BasicPay = Convert.ToString(model.BasicPay);
                npmodel.ApplicationReceiveDate = String.Format("{0:ddd dd-MMMM-yyyy}", model.AppointmentReciveDate);
                npmodel.isMSPhd = query.isMsPhd ?? false;
                npmodel.TypeofAppointment = query.TypeofAppointment;
                npmodel.SendSlryStruct = model.SendSalaryStructure;
                if (model.AppType == "STE")
                {
                    if (model.OrderType == "Extension" && query.BasicPay > model.BasicPay)
                        npmodel.FillFields = "revision of pay";
                    else if (model.OrderType == "Enhancement" && (model.BasicPay == query.BasicPay || model.BasicPay > query.BasicPay) && model.DesignationId == query.DesignationId && model.ProjectID == query.ProjectId)
                        npmodel.subject = "Salary enhancement status for";
                    else if (model.OrderType == "Enhancement" && model.BasicPay < query.BasicPay && model.DesignationId == query.DesignationId && model.ProjectID == query.ProjectId)
                        npmodel.FillFields = "revision of pay";
                    else if (model.OrderType == "Enhancement" && model.DesignationId != query.DesignationId)
                        npmodel.FillFields = "change of designation";
                    else if (model.OrderType == "Enhancement" && model.ProjectID != query.ProjectId)
                        npmodel.FillFields = "change of project";
                }
                if (model.AppType == "OSG")
                {

                    if (query.BasicPay != model.BasicPay)
                        npmodel.SalaryDiff_f = true;

                    if (query.DesignationId != model.DesignationId)
                        npmodel.DesignationDiff_f = true;

                    if (query.AppointmentEnddate < model.AppointmentStartDate)
                        npmodel.Extended_f = true;

                    if (model.OrderType == "Extension")
                    {
                        npmodel.FillFields = "term extension";
                        if (model.BasicPay < query.BasicPay)
                            npmodel.FillFields = "revision of pay";
                        else if (query.BasicPay < model.BasicPay)
                            npmodel.FillFields = "extension cum enhancement";
                    }
                    else if (model.OrderType == "Enhancement")
                    {

                        npmodel.FillFields = "salary enhancement";
                        if (model.BasicPay < query.BasicPay)
                            npmodel.FillFields = "appointment";
                        else if (model.ProjectID != query.ProjectId)
                            npmodel.FillFields = "extension with change of project";
                        else if (model.DesignationId != query.DesignationId)
                            npmodel.FillFields = "change of designation";
                    }
                }


                if (model.OrderType == "HRA Booking")
                {
                    npmodel.Ack_f = true;
                    npmodel.ProjectNumber = Common.getprojectnumber(query.ProjectId ?? 0);

                }
            }
            var loadEmialView = Tuple.Create(false, "", "");
            if (model.OrderType == "HRA Booking")
                loadEmialView = _eb.RunCompile("RCTHRAMailTemplate.cshtml", "", npmodel, typeof(NotePIModel));
            else if (model.AppType == "OSG")
                loadEmialView = _eb.RunCompile("OSGOrdersAcknowledgements.cshtml", "", npmodel, typeof(NotePIModel));
            else
                loadEmialView = _eb.RunCompile("RCTOrderAckTemplate.cshtml", "", npmodel, typeof(NotePIModel));
            var result = new { output = loadEmialView };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult OrderCheckDeviation(CheckDevationModel model)
        {
            var UserName = User.Identity.Name;
            var UserId = Common.GetUserid(UserName);
            List<CheckListModel> listmodel = new List<CheckListModel>();
            List<CheckListEmailModel> checkdetails = new List<CheckListEmailModel>();
            NotePIModel PIModel = new NotePIModel();
            EmailBuilder _eb = new EmailBuilder();
            var Data = Common.GetDeviationofAppointments(model);
            listmodel = Data.Item1;
            if (listmodel.Count > 0)
            {
                for (int i = 0; i < listmodel.Count; i++)
                {
                    var sno = i + 1;
                    checkdetails.Add(new CheckListEmailModel()
                    {
                        CheckList = listmodel[i].CheckList,
                        checklistId = listmodel[i].FunctionCheckListId
                    });
                }
            }
            model.devChecklist = checkdetails;
            using (var context = new IOASDBEntities())
            {
                var query = (from vw in context.vw_RCTOverAllApplicationEntry
                             where vw.ApplicationId == model.AppId && vw.Category == model.AppType && vw.ApplicationType == "New"
                             select new
                             {
                                 vw.PostRecommended,
                                 vw.TypeofAppointment,
                                 vw.BasicPay,
                                 vw.DesignationId,
                                 vw.ProjectId,
                                 vw.AppointmentEnddate,
                                 vw.EmployeersID
                             }).FirstOrDefault();
                PIModel.AppointmentType = query.TypeofAppointment;
                model.OldEmployee = query.EmployeersID;
                PIModel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", model.AppointmentStartDate);
                PIModel.AppointmentEndDate = String.Format("{0:dd-MMMM-yyyy}", model.AppointmentEndDate);
                PIModel.PersonName = model.PersonName;
                PIModel.ProjectNumber = Common.getprojectnumber(model.ProjectID ?? 0);
                if (model.AppType == "OSG")
                    PIModel.Ordertype = model.OrderType;
                else
                    PIModel.Ordertype = model.OrderType.ToLower();
                PIModel.DAName = Common.GetUserNameBasedonId(UserId);
                if (model.OrderType == "Enhancement")
                    PIModel.DesignationName = RequirementService.getDesignationName(model.DesignationId ?? 0);
                else
                    PIModel.DesignationName = query.PostRecommended;
                PIModel.checkdetails = RCTEmailContentService.getDevNormsDetails(model);
                PIModel.IsDeviation = true;
                PIModel.Salary = model.ChekSalary ?? 0;
                PIModel.SendSlryStruct = model.SendSalaryStructure;
                PIModel.AppointmentType = query.TypeofAppointment;
                PIModel.Comments = model.Comments;

                if (model.ChekSalary != query.BasicPay)
                    PIModel.SalaryDiff_f = true;
                if (model.OrderType == "Extension" && model.AppType == "OSG")
                {
                    PIModel.FillFields = "term extension";
                    if (query.BasicPay > model.ChekSalary)
                        PIModel.FillFields = "revision of pay";
                    else if (query.BasicPay < model.ChekSalary)
                        PIModel.FillFields = "extension cum enhancement";
                }
                if (model.OrderType == "Enhancement" && model.AppType == "OSG")
                {
                    PIModel.FillFields = "salary enhancement";
                    if (query.BasicPay > model.ChekSalary)
                        PIModel.FillFields = "appointment";
                    else if (query.ProjectId != model.ProjectID)
                        PIModel.FillFields = "extension with change of project";
                    else if (query.DesignationId != model.DesignationId && query.AppointmentEnddate < model.AppointmentStartDate && query.BasicPay < model.ChekSalary)
                        PIModel.FillFields = "extension cum enhancement with change of designation";
                    else if (query.DesignationId != model.DesignationId && query.AppointmentEnddate < model.AppointmentStartDate && query.BasicPay == model.ChekSalary)
                        PIModel.FillFields = "extension with change of designation";
                    else if (query.DesignationId != model.DesignationId && query.AppointmentEnddate == model.AppointmentEndDate && query.BasicPay < model.ChekSalary)
                        PIModel.FillFields = "change of designation with enhancement";
                    else if (query.DesignationId != model.DesignationId && query.AppointmentEnddate == model.AppointmentEndDate)
                        PIModel.FillFields = "change of designation";
                }
            }
            var loadEmialView = Tuple.Create(false, "", "");
            if (model.AppType == "OSG")
                loadEmialView = _eb.RunCompile("OSGOrdersDeviation.cshtml", "", PIModel, typeof(NotePIModel));
            else
                loadEmialView = _eb.RunCompile("DeviationMailForExEnh.cshtml", "", PIModel, typeof(NotePIModel));
            var result = new { output = ConvertViewToString("_DeviationCheckListDetail", listmodel), isRes = Data.Item2, template = loadEmialView.Item2 };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult HRACheckDeviation(CheckDevationModel model)
        {
            List<CheckListModel> listmodel = new List<CheckListModel>();
            List<CheckListEmailModel> checkdetails = new List<CheckListEmailModel>();
            NotePIModel PIModel = new NotePIModel();
            EmailBuilder _eb = new EmailBuilder();
            var Data = Common.GetHRADeviation(model);
            listmodel = Data.Item1;
            if (listmodel.Count > 0)
            {
                for (int i = 0; i < listmodel.Count; i++)
                {
                    var sno = i + 1;
                    checkdetails.Add(new CheckListEmailModel()
                    {
                        CheckList = listmodel[i].CheckList
                    });
                }
            }
            using (var context = new IOASDBEntities())
            {
                PIModel.checkdetails = checkdetails;
                PIModel.PersonName = model.PersonName;
                PIModel.AppointmentStartDate = String.Format("{0:ddd dd-MMMM-yyyy}", model.AppointmentStartDate);
                PIModel.AppointmentEndDate = String.Format("{0:ddd dd-MMMM-yyyy}", model.AppointmentEndDate);
                PIModel.Body = "The application received for the HRA Booking from " + PIModel.AppointmentStartDate + " to " + PIModel.AppointmentEndDate + " for the employee " + PIModel.PersonName + " is having following deviation from the norms prescribed by the institute.";
            }
            var loadEmialView = _eb.RunCompile("OrdersDeviation.cshtml", "", PIModel, typeof(NotePIModel));
            var result = new { output = ConvertViewToString("_DeviationCheckListDetail", listmodel), isRes = Data.Item2, template = loadEmialView.Item2 };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult EmployeeMaster()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetEmployeeList(SearchEmployeeModel model, int pageIndex, int pageSize, DateFilterModel strDateofBirth, DateFilterModel strDateofJoining)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = recruitmentService.GetEmployeeList(model, pageIndex, pageSize, strDateofBirth, strDateofJoining, userid, roleid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }


        #region Employee Master Options

        #region Change of project

        public ActionResult RecruitChangeOfProject(int appid, string apptype, int orderId = 0, string listf = null)
        {
            OrderModel model = new OrderModel();
            try
            {
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                int? ordId = null;
                if (orderId > 0)
                {
                    model = recruitmentService.getOrderDetails(orderId);
                    ordId = orderId;
                }
                else if (appid > 0 && !string.IsNullOrEmpty(apptype))
                {
                    model = recruitmentService.getOrderProjectDetails(appid, apptype, 1);
                }
                model.List_f = getEmployeeActionLink(apptype, listf);
                var data = RequirementService.validPreOrder(model.ApplicationID, model.TypeCode, model.OrderType, ordId);
                if (data.Item1)
                {
                    TempData["alertMsg"] = data.Item2;
                    return RedirectToAction(model.List_f);
                }

                var listcommitte = Common.GetCommittee();
                if (listcommitte.Item1.Count > 0)
                {
                    for (int i = 0; i < listcommitte.Item1.Count; i++)
                    {
                        if (i == 0)
                        {
                            model.CommiteeMemberId1 = listcommitte.Item1[i].id ?? 0;
                            model.CommiteeMember1 = listcommitte.Item1[i].name;
                        }
                        if (i == 1)
                        {
                            model.CommiteeMemberId2 = listcommitte.Item1[i].id ?? 0;
                            model.CommiteeMember2 = listcommitte.Item1[i].name;
                        }
                    }
                }
                var datacharperson = Common.GetChairPerson();
                model.ChairpersonNameId = datacharperson.Item1;
                model.ChairpersonName = datacharperson.Item2;
                model.isConsolidatePay = "ConsolidatedPay";
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult RecruitChangeOfProject(OrderModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                var allowedExtensionsDoc = new[] { ".doc", ".docx", ".pdf" };
                if (model.PIJustificationFile != null)
                {
                    foreach (var file in model.PIJustificationFile)
                    {
                        if (file != null)
                        {
                            string filename = System.IO.Path.GetFileName(file.FileName);
                            string extension = Path.GetExtension(filename);
                            if (!allowedExtensionsDoc.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type doc [.doc,.docx,.pdf]";
                                return View(model);
                            }
                            if (file.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload PI judtification document up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }

                if (model.PILetter != null)
                {
                    string filename = System.IO.Path.GetFileName(model.PILetter.FileName);
                    string extension = Path.GetExtension(filename);
                    if (!allowedExtensionsDoc.Contains(extension.ToLower()))
                    {
                        TempData["alertMsg"] = "Please upload any one of these type of pi letter document [.doc,.docx,.pdf]";
                        return View(model);
                    }
                    if (model.PILetter.ContentLength > 5242880)
                    {
                        TempData["alertMsg"] = "You can upload pi letter document up to 5MB";
                        return View(model);
                    }
                }
                else
                {
                    if (model.OrderID == 0)
                    {
                        TempData["alertMsg"] = "Please upload pi request document";
                        return View(model);
                    }
                }

                var validationMsg = ValidateCOPFormData(model);
                if (validationMsg != "Valid")
                {
                    if (model.OrderID > 0)
                        model = recruitmentService.getOrderDetails(model.OrderID);
                    else
                        model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 1);
                    TempData["alertMsg"] = validationMsg;
                    return View(model);
                }
                int? ordId = null;
                if (model.OrderID > 0)
                    ordId = model.OrderID;
                var validationPre = RequirementService.validPreOrder(model.ApplicationID, model.TypeCode, model.OrderType, ordId);
                if (validationPre.Item1)
                {
                    if (model.OrderID > 0)
                        model = recruitmentService.getOrderDetails(model.OrderID);
                    else
                        model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 1);
                    TempData["alertMsg"] = validationPre.Item2;
                    return View(model);
                }

                var result = recruitmentService.CommitCOP(model, userId);
                if (result.Item1 == 1)
                    TempData["succMsg"] = "Request submitted Successfully";
                else if (result.Item1 == 2)
                    TempData["succMsg"] = "Request re-submitted for commitment booking";
                else if (result.Item1 == -1)
                    TempData["succMsg"] = "Request submitted for PI justification";
                else if (result.Item1 == 3)
                    TempData["succMsg"] = result.Item2;
                else
                    TempData["errMsg"] = "Something went wrong please contact administrator";

                return RedirectToAction(model.List_f);

            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                if (model.OrderID > 0)
                    model = recruitmentService.getOrderDetails(model.OrderID);
                else
                    model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 1);
                TempData["errMsg"] = "Something went wrong please contact administrator";
                return View(model);
            }
        }

        private string ValidateCOPFormData(OrderModel model)
        {
            string msg = "Valid";
            int projectId = 0, olddesignationid = 0;
            DateTime orginalappointmentstartdate = DateTime.Now;
            DateTime orginalappointmentenddate = DateTime.Now;
            ProjectService _PS = new ProjectService();
            using (var context = new IOASDBEntities())
            {
                if (model.Appointmentstartdate != null && model.AppointmentEndDate != null)
                {
                    var query = context.vw_RCTOverAllApplicationEntry.Where(m => m.ApplicationId == model.ApplicationID && m.Category == model.TypeCode && m.ApplicationType == "New" && m.isEmployee == true).Select(m => new { m.ProjectId, m.DesignationId, m.AppointmentStartdate, m.AppointmentEnddate }).FirstOrDefault();
                    if (query != null)
                    {
                        projectId = query.ProjectId ?? 0;
                        olddesignationid = query.DesignationId ?? 0;
                        orginalappointmentstartdate = query.AppointmentStartdate ?? DateTime.Now;
                        orginalappointmentenddate = query.AppointmentEnddate ?? DateTime.Now;
                    }
                    else
                        msg = msg == "Valid" ? "Record not found. Something went wrong please contact administrator." : msg + "<br />Record not found. Something went wrong please contact administrator.";

                    if (olddesignationid == model.DesignationId)
                    {
                        var querydes = context.tblRCTDesignation.FirstOrDefault(M => M.DesignationId == olddesignationid && M.RecordStatus == "Active");
                        if (querydes != null)
                        {
                            if (querydes.Medical != true)
                            {
                                if (model.Medical != 3 && model.Medical != 0 || model.MedicalAmmount > 0)
                                    msg = msg == "Valid" ? "Entered designation does not have medical contribution. please contact administrator." : msg + "<br /> Entered designation does not have medical contribution. please contact administrator.";
                            }
                            else
                            {
                                var medicaldeduction = querydes.MedicalDeduction ?? 0;
                                if (model.MedicalAmmount > 0 && model.MedicalAmmount != medicaldeduction)
                                    msg = msg == "Valid" ? "Medical contribution data mis match. please contact administrator." : msg + "<br /> Medical contribution data mis match. please contact administrator.";
                            }

                            if (querydes.HRA != true)
                            {
                                if (model.HRA > 0)
                                    msg = msg == "Valid" ? "Entered designation does not have HRA provision. Please contact administrator." : msg + "<br />Entered designation does not have HRA provision. Please contact administrator.";
                            }
                            else
                            {
                                var HRAPercentage = querydes.HRABasic / 100;
                                var HRAValue = model.Salary * HRAPercentage;
                                if (model.HRA > 0 && model.HRA != HRAValue)
                                    msg = msg == "Valid" ? "HRA entered exceeds institution norms." : msg + "<br /> HRA entered exceeds institution norms.";
                            }
                        }
                    }
                    else
                        msg = msg == "Valid" ? "Change of project option only change the project.Please contact administrator" : msg + "<br /> Change of project option only change the project.Please contact administrator.";


                    if (projectId == model.ProjectId)
                        msg = msg == "Valid" ? "Same project is not allowed." : msg + "<br /> Same project is not allowed.";

                    if (model.ProjectId > 0)
                    {
                        //Check Period tenure
                        var _projectDetail = RequirementService.getProjectSummary(model.ProjectId ?? 0);
                        double DiffMonth = model.AppointmentEndDate.Value.Subtract(model.Appointmentstartdate.Value).Days + 1;
                        decimal totdays = Common.GetAvgDaysInAYear((DateTime)model.Appointmentstartdate, (DateTime)model.AppointmentEndDate);
                        var Days = model.AppointmentEndDate.Value.Subtract(model.Appointmentstartdate.Value).Days + 1;
                        var Years = Convert.ToDecimal(Days) / Convert.ToDecimal(totdays);
                        //if (Years > 1 || DiffMonth <= 29)
                        //    msg = msg == "Valid" ? "Appointment tenure should be minimum 1 month to 1 year." : msg + "<br />Appointment tenure should be minimum 1 month to 1 year.";

                        if (!(orginalappointmentstartdate <= model.Appointmentstartdate && model.AppointmentEndDate == orginalappointmentenddate))
                            msg = msg == "Valid" ? "Appointment tenure must be between the old appointment start date and old appointment closure date." : msg + "<br /> Appointment tenure must be between the old appointment start date and old appointment closure date.";

                        DateTime StartDate = DateTime.Parse(_projectDetail.ProjectStartDate);
                        DateTime CloseDate = DateTime.Parse(_projectDetail.ProjectClosureDate);

                        if (StartDate <= model.Appointmentstartdate != model.AppointmentEndDate <= CloseDate)
                            msg = msg == "Valid" ? "Appointment tenure must be between the project start date and project closure date." : msg + "<br /> Appointment tenure must be between the project start date and project closure date.";
                    }
                    else
                        msg = msg == "Valid" ? "Please enter project number." : msg + "<br />Please enter project number.";
                }
                else
                    msg = msg == "Valid" ? "Enter appointment tenure." : msg + "<br /> Enter appointment tenure.";
            }
            return msg;
        }

        public ActionResult RecruitCOPView(int OrderId, string listf = null)
        {
            OrderModel model = new OrderModel();
            try
            {
                if (OrderId > 0)
                {
                    model = recruitmentService.getOrderDetails(OrderId);
                    model.List_f = getEmployeeActionLink(model.TypeCode, listf);
                    if (OrderId <= 518)
                    {
                        if (model.FlowApprover == "CMAdmin")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RecruitCOPCMAdmin", 0);
                        else if (model.FlowApprover == "NDean")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RecruitCOPDean", 0);
                        else
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RecruitCOP Flow", 0);
                    }
                    else
                    {
                        if (model.FlowApprover == "NDean")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RCT / " + model.TypeCode + " COP Dean Flow", 0);
                        else if (model.FlowApprover == "CMAdmin")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RCT / " + model.TypeCode + " COP CM Flow", 0);
                        else
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RCT / " + model.TypeCode + " COP HR Flow", 0);
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(195, "", 0);
                return View(model);
            }
        }

        #endregion

        #region Extension

        public ActionResult RecruitExtension(int appid, string apptype, int orderId = 0, string listf = null)
        {
            OrderModel model = new OrderModel();
            try
            {
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.AppointmentType = Common.AppointmentType(apptype);
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                string actionLink = getEmployeeActionLink(apptype, listf);
                int? ordId = null;
                if (orderId > 0)
                {
                    model = recruitmentService.getOrderDetails(orderId);
                    ordId = orderId;
                }
                else if (appid > 0)
                {
                    model = recruitmentService.getOrderProjectDetails(appid, apptype, 3);
                    model.SourceReferenceNumber = 0;
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator";
                    return RedirectToAction(actionLink);
                }

                var data = RequirementService.validPreOrder(model.ApplicationID, model.TypeCode, model.OrderType, ordId);
                if (data.Item1)
                {
                    TempData["alertMsg"] = data.Item2;
                    return RedirectToAction(actionLink);
                }

                var listcommitte = Common.GetCommittee();
                if (listcommitte.Item1.Count > 0)
                {
                    for (int i = 0; i < listcommitte.Item1.Count; i++)
                    {
                        if (i == 0)
                        {
                            model.CommiteeMemberId1 = listcommitte.Item1[i].id ?? 0;
                            model.CommiteeMember1 = listcommitte.Item1[i].name;
                        }
                        if (i == 1)
                        {
                            model.CommiteeMemberId2 = listcommitte.Item1[i].id ?? 0;
                            model.CommiteeMember2 = listcommitte.Item1[i].name;
                        }
                    }
                }
                var datacharperson = Common.GetChairPerson();
                model.ChairpersonNameId = datacharperson.Item1;
                model.ChairpersonName = datacharperson.Item2;

                model.List_f = actionLink;
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.AppointmentType = Common.AppointmentType(apptype);
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult RecruitExtension(OrderModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.AppointmentType = Common.AppointmentType(model.TypeCode);
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                var allowedJustificationDoc = new[] { ".doc", ".docx", ".pdf" };
                if (model.ApplicationID > 0)
                {
                    if (model.PIJustificationFile != null)
                    {
                        foreach (var item in model.PIJustificationFile)
                        {
                            if (item != null)//...........Justification Document
                            {
                                string filename = System.IO.Path.GetFileName(item.FileName);
                                string extension = Path.GetExtension(filename);
                                if (!allowedJustificationDoc.Contains(extension.ToLower()))
                                {
                                    if (model.OrderID > 0)
                                        model = recruitmentService.getOrderDetails(model.OrderID);
                                    else
                                        model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 3);
                                    TempData["alertMsg"] = "Please upload any one of these type doc [.doc,.docx,.pdf]";
                                    return View(model);
                                }
                                if (item.ContentLength > 5242880)
                                {
                                    if (model.OrderID > 0)
                                        model = recruitmentService.getOrderDetails(model.OrderID);
                                    else
                                        model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 3);
                                    TempData["alertMsg"] = "You can upload PI judtification document up to 5MB";
                                    return View(model);
                                }
                            }
                        }
                    }
                    if (model.PILetter != null)
                    {
                        string filename = System.IO.Path.GetFileName(model.PILetter.FileName);
                        string extension = Path.GetExtension(filename);
                        if (!allowedJustificationDoc.Contains(extension.ToLower()))
                        {
                            if (model.OrderID > 0)
                                model = recruitmentService.getOrderDetails(model.OrderID);
                            else
                                model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 3);
                            TempData["alertMsg"] = "Please upload any one of these type of pi letter document  [.doc,.docx,.pdf]";
                            return View(model);
                        }
                        if (model.PILetter.ContentLength > 5242880)
                        {
                            if (model.OrderID > 0)
                                model = recruitmentService.getOrderDetails(model.OrderID);
                            else
                                model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 3);
                            TempData["alertMsg"] = "You can upload PI judtification document up to 5MB";
                            return View(model);
                        }
                    }
                    else
                    {
                        if (model.OrderID == 0)
                        {
                            model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 3);
                            TempData["alertMsg"] = "Please upload pi request document";
                            return View(model);
                        }
                    }

                    var validationMsg = ValidateExtensionFormData(model);
                    if (validationMsg != "Valid")
                    {
                        if (model.OrderID > 0)
                            model = recruitmentService.getOrderDetails(model.OrderID);
                        else
                            model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 3);
                        TempData["alertMsg"] = validationMsg;
                        return View(model);
                    }
                    int? ordId = null;
                    if (model.OrderID > 0)
                        ordId = model.OrderID;
                    var data = RequirementService.validPreOrder(model.ApplicationID, model.TypeCode, model.OrderType, ordId);
                    if (data.Item1)
                    {
                        if (model.OrderID > 0)
                            model = recruitmentService.getOrderDetails(model.OrderID);
                        else
                            model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 3);
                        TempData["alertMsg"] = data.Item2;
                        return View(model);
                    }

                    var result = recruitmentService.CommitExtension(model, userId);
                    if (result.Item1 == 1)
                        TempData["succMsg"] = "Request submitted Successfully";
                    else if (result.Item1 == -1)
                        TempData["succMsg"] = "Request submitted for PI justification";
                    else if (result.Item1 == 2)
                        TempData["succMsg"] = "Request re-submitted for commitment booking";
                    else if (result.Item1 == 3)
                        TempData["succMsg"] = result.Item2;
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                    return RedirectToAction(model.List_f);
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator";
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.AppointmentType = Common.AppointmentType("");
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                TempData["errMsg"] = "Something went wrong please contact administrator";
                if (model.OrderID > 0)
                    model = recruitmentService.getOrderDetails(model.OrderID);
                else
                    model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 3);
                return View(model);
            }
        }

        private string ValidateExtensionFormData(OrderModel model)
        {
            string msg = "Valid";
            int appid = model.ApplicationID;
            int projectid = 0, olddesid = 0;
            DateTime actstartdate = DateTime.Now;
            DateTime actenddate = DateTime.Now;
            using (var context = new IOASDBEntities())
            {
                if (model.FromDate != null && model.ToDate != null)
                {
                    var query = context.vw_RCTOverAllApplicationEntry.Where(m => m.ApplicationId == appid && m.Category == model.TypeCode && m.ApplicationType == "New").Select(m => new { m.ProjectId, m.DesignationId, m.AppointmentStartdate, m.AppointmentEnddate }).FirstOrDefault();
                    if (query != null)
                    {
                        projectid = query.ProjectId ?? 0;
                        olddesid = query.DesignationId ?? 0;
                        actstartdate = query.AppointmentStartdate ?? DateTime.Now;
                        actenddate = query.AppointmentEnddate ?? DateTime.Now;
                    }
                    else
                    {
                        msg = msg == "Valid" ? "Record not found. Something went wrong please contact administrator." : msg + "<br />Record not found. Something went wrong please contact administrator.";
                    }

                    if (actenddate.AddDays(+1) != model.FromDate)
                    {
                        msg = msg == "Valid" ? "Extension from date must be greater than appointment end date." : msg + "<br /> Extension from date must be greater than appointment end date.";
                    }

                    if (olddesid != model.DesignationId)
                    {
                        msg = msg == "Valid" ? "Extension does not have designation change provision." : msg + "<br /> Extension does not have designation change provision.";
                    }
                    else
                    {
                        var querydes = context.tblRCTDesignation.FirstOrDefault(M => M.DesignationId == olddesid && M.RecordStatus == "Active");
                        if (querydes != null)
                        {
                            if (querydes.Medical != true)
                            {
                                if (model.Medical != 3 && model.Medical != 0 || model.MedicalAmmount > 0)
                                    msg = msg == "Valid" ? "Entered designation does not have medical contribution. please contact administrator." : msg + "<br /> Entered designation does not have medical contribution. please contact administrator.";
                            }
                            else
                            {
                                if (model.MedicalAmmount != querydes.MedicalDeduction && model.MedicalAmmount > 0)
                                    msg = msg == "Valid" ? "Medical contribution data mis match. please contact administrator." : msg + "<br /> Medical contribution data mis match. please contact administrator.";
                            }

                            if (querydes.HRA != true)
                            {
                                if (model.HRA > 0)
                                    msg = msg == "Valid" ? "Entered designation does not have HRA provision. Please contact administrator." : msg + "<br />Entered designation does not have HRA provision. Please contact administrator.";
                            }
                            else
                            {
                                var HRAPercentage = querydes.HRABasic / 100;
                                var HRAValue = model.Salary * HRAPercentage;
                                if (model.HRA > 0 && model.HRA != HRAValue)
                                {
                                    msg = msg == "Valid" ? "HRA entered exceeds institution norms." : msg + "<br /> HRA entered exceeds institution norms.";
                                }
                            }
                        }
                    }

                    if (projectid > 0 && projectid == model.ProjectId)
                    {
                        //Check Period tenure
                        var Days = model.ToDate.Value.Subtract(model.FromDate.Value).Days + 1;
                        decimal totdays = Common.GetAvgDaysInAYear((DateTime)model.FromDate, (DateTime)model.ToDate);
                        var Years = Convert.ToDecimal(Days) / totdays;
                        if (Years > 1)
                            msg = msg == "Valid" ? "Appointment tenure should be minimum 1 month to 1 year." : msg + "<br />Appointment tenure should be minimum 1 month to 1 year.";

                        var projectdet = RequirementService.getProjectSummary(projectid);
                        DateTime StartDate = DateTime.Parse(projectdet.ProjectStartDate);
                        DateTime CloseDate = DateTime.Parse(projectdet.ProjectClosureDate);
                        if (StartDate <= model.FromDate != model.ToDate <= CloseDate)
                            msg = msg == "Valid" ? "Appointment tenure must be between the project start date and project closure date." : msg + "<br /> Appointment tenure must be between the project start date and project closure date.";
                    }
                    else
                    {
                        msg = msg == "Valid" ? "Extension does not have project change provision." : msg + "<br /> Extension does not have project change provision.";
                    }
                }
                else
                {
                    msg = msg == "Valid" ? "Enter appointment tenure." : msg + "<br /> Enter appointment tenure.";
                }
            }

            return msg;
        }

        public ActionResult RecruitExtensionView(int OrderId, string listf = null)
        {
            OrderModel model = new OrderModel();
            try
            {
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                if (OrderId > 0)
                {
                    model = recruitmentService.getOrderDetails(OrderId);
                    model.List_f = getEmployeeActionLink(model.TypeCode, listf);
                    ViewBag.AppointmentType = Common.AppointmentType(model.TypeCode);
                    if (OrderId <= 518)
                    {
                        if (model.FlowApprover == "CMAdmin")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(197, "RecruitExAdmin", 0);
                        else if (model.FlowApprover == "NDean")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(197, "RecruitExDean", 0);
                        else
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(197, "RecruitExtension Flow", 0);
                    }
                    else
                    {
                        string Type = "";
                        if (model.FlowApprover == "NDean")
                            Type = "RCT / " + model.TypeCode + " EX Dean Flow";
                        else if (model.FlowApprover == "CMAdmin")
                            Type = "RCT / " + model.TypeCode + " EX CM Flow";
                        else
                            Type = "RCT / " + model.TypeCode + " EX HR Flow";
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(197, Type, 0);
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "", 0);
                return View(model);
            }
        }

        #endregion

        #region Enhancement

        public ActionResult RecruitEnhancement(int appid, string apptype, int orderId = 0, string listf = null)
        {
            OrderModel model = new OrderModel();
            try
            {
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.AppointmentType = Common.AppointmentType(apptype);
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                string actionLink = getEmployeeActionLink(apptype, listf);
                int? ordId = null;
                if (orderId > 0)
                {
                    model = recruitmentService.getOrderDetails(orderId);
                    ViewBag.AppointmentType = Common.AppointmentType(model.TypeCode);
                    ordId = orderId;
                }
                else if (appid > 0)
                {
                    model = recruitmentService.getOrderProjectDetails(appid, apptype, 2);
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator";
                    return RedirectToAction(actionLink);
                }

                var Data = RequirementService.validPreOrder(model.ApplicationID, model.TypeCode, model.OrderType, ordId);
                if (Data.Item1)
                {
                    TempData["alertMsg"] = Data.Item2;
                    return RedirectToAction(actionLink);
                }
                model.List_f = actionLink;
                var listcommitte = Common.GetCommittee();
                if (listcommitte.Item1.Count > 0)
                {
                    for (int i = 0; i < listcommitte.Item1.Count; i++)
                    {
                        if (i == 0)
                        {
                            model.CommiteeMemberId1 = listcommitte.Item1[i].id ?? 0;
                            model.CommiteeMember1 = listcommitte.Item1[i].name;
                        }
                        if (i == 1)
                        {
                            model.CommiteeMemberId2 = listcommitte.Item1[i].id ?? 0;
                            model.CommiteeMember2 = listcommitte.Item1[i].name;
                        }
                    }
                }
                var datacharperson = Common.GetChairPerson();
                model.ChairpersonNameId = datacharperson.Item1;
                model.ChairpersonName = datacharperson.Item2;
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.AppointmentType = Common.AppointmentType(apptype);
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult RecruitEnhancement(OrderModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.AppointmentType = Common.AppointmentType(model.TypeCode);
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                var allowedDoc = new[] { ".doc", ".docx", ".pdf" };
                if (model.ApplicationID > 0)
                {
                    var validationMsg = ValidateEnhancementFormData(model);
                    if (validationMsg != "Valid")
                    {
                        if (model.OrderID > 0)
                            model = recruitmentService.getOrderDetails(model.OrderID);
                        else
                            model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 2);
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    int? ordId = null;
                    if (model.OrderID > 0)
                        ordId = model.OrderID;
                    var validationPre = RequirementService.validPreOrder(model.ApplicationID, model.TypeCode, model.OrderType, ordId);
                    if (validationPre.Item1)
                    {
                        if (model.OrderID > 0)
                            model = recruitmentService.getOrderDetails(model.OrderID);
                        else
                            model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 2);
                        TempData["alertMsg"] = validationPre.Item2;
                        return View(model);
                    }
                    #region FileValidation                    
                    if (model.PIJustificationFile != null)
                    {
                        foreach (var item in model.PIJustificationFile)
                        {
                            if (item != null)//...........Justification Document
                            {
                                string filename = System.IO.Path.GetFileName(item.FileName);
                                string extension = Path.GetExtension(filename);
                                if (!allowedDoc.Contains(extension.ToLower()))
                                {
                                    TempData["errMsg"] = "Please upload any one of these type doc [.doc,.docx,.pdf]";
                                    return View(model);
                                }
                            }
                        }
                    }
                    if (model.PILetter != null)
                    {
                        string filename = System.IO.Path.GetFileName(model.PILetter.FileName);
                        string extension = Path.GetExtension(filename);
                        if (!allowedDoc.Contains(extension.ToLower()))
                        {
                            TempData["errMsg"] = "Please upload any one of these type of pi letter document [.doc,.docx,.pdf]";
                            return View(model);
                        }
                    }
                    else
                    {
                        if (model.OrderID == 0)
                        {
                            TempData["errMsg"] = "Please upload pi request document";
                            return View(model);
                        }
                    }
                    #endregion
                    var result = recruitmentService.CommitEnhancement(model, userId);
                    if (result.Item1 == 1)
                        TempData["succMsg"] = "Request submitted Successfully";
                    else if (result.Item1 == 2)
                        TempData["succMsg"] = "Request re-submitted for commitment booking";
                    else if (result.Item1 == -1)
                        TempData["succMsg"] = "Request submitted for PI justification";
                    else if (result.Item1 == 3)
                        TempData["errMsg"] = result.Item2;
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                    return RedirectToAction(model.List_f);
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
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.AppointmentType = Common.AppointmentType("");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                return View(model);
            }
        }

        private string ValidateEnhancementFormData(OrderModel model)
        {
            string msg = "Valid";
            var now = DateTime.Now;
            int newdesid = model.DesignationId ?? 0;
            int projectid = model.ProjectId ?? 0;
            int oldprojectid = 0, olddesid = 0;
            int appid = model.ApplicationID;
            DateTime actstartdate = DateTime.Now;
            DateTime actenddate = DateTime.Now;
            using (var context = new IOASDBEntities())
            {
                if (model.TypeCode == "STE")
                {
                    var query = context.tblRCTSTE.FirstOrDefault(m => m.STEID == appid);
                    if (query != null)
                    {
                        oldprojectid = query.ProjectId ?? 0;
                        olddesid = query.DesignationId ?? 0;
                        actstartdate = query.AppointmentStartdate ?? DateTime.Now;
                        actenddate = query.AppointmentEnddate ?? DateTime.Now;
                    }
                    else
                    {
                        msg = msg == "Valid" ? "Something went wrong please contact administrator." : msg + "<br /> Something went wrong please contact administrator.";
                    }
                }
                else if (model.TypeCode == "CON")
                {
                    var query = context.tblRCTConsultantAppointment.FirstOrDefault(m => m.ConsultantAppointmentId == appid);
                    if (query != null)
                    {
                        oldprojectid = query.ProjectId ?? 0;
                        olddesid = query.DesignationId ?? 0;
                        actstartdate = query.AppointmentStartdate ?? DateTime.Now;
                        actenddate = query.AppointmentEnddate ?? DateTime.Now;
                    }
                    else
                    {
                        msg = msg == "Valid" ? "Something went wrong please contact administrator." : msg + "<br /> Something went wrong please contact administrator.";
                    }
                }
                else if (model.TypeCode == "OSG")
                {
                    var query = context.tblRCTOutsourcing.FirstOrDefault(m => m.OSGID == appid);
                    if (query != null)
                    {
                        oldprojectid = query.ProjectId ?? 0;
                        olddesid = query.DesignationId ?? 0;
                        actstartdate = query.AppointmentStartdate ?? DateTime.Now;
                        actenddate = query.AppointmentEnddate ?? DateTime.Now;
                    }
                    else
                    {
                        msg = msg == "Valid" ? "Something went wrong please contact administrator." : msg + "<br /> Something went wrong please contact administrator.";
                    }
                }

                if (newdesid > 0)
                {
                    var query = context.tblRCTDesignation.FirstOrDefault(M => M.DesignationId == newdesid && M.RecordStatus == "Active");
                    if (query != null)
                    {
                        if (query.Medical != true)
                        {
                            if (model.Medical != 3 && model.Medical != 0 || model.MedicalAmmount > 0)
                                msg = msg == "Valid" ? "Entered designation does not have medical contribution. please contact administrator." : msg + "<br /> Entered designation does not have medical contribution. please contact administrator.";

                        }
                        else
                        {
                            var medicaldeduction = query.MedicalDeduction ?? 0;
                            if (model.MedicalAmmount > 0 && model.MedicalAmmount != medicaldeduction)
                                msg = msg == "Valid" ? "Medical contribution data mis match. please contact administrator." : msg + "<br /> Medical contribution data mis match. please contact administrator.";
                        }

                        if (query.HRA != true)
                        {
                            if (model.HRA > 0)
                                msg = msg == "Valid" ? "Entered designation does not have HRA provision. Please contact administrator." : msg + "<br />Entered designation does not have HRA provision. Please contact administrator.";
                        }
                        else
                        {
                            var HRAPercentage = query.HRABasic / 100;
                            var HRAValue = model.Salary * HRAPercentage;
                            if (model.HRA > 0 && model.HRA != HRAValue)
                                msg = msg == "Valid" ? "HRA entered exceeds institution norms." : msg + "<br /> HRA entered exceeds institution norms.";
                        }
                    }
                }
                else
                {
                    msg = msg == "Valid" ? "Enter valid designation code." : msg + "<br /> Enter valid designation code.";
                }

                if (projectid > 0)
                {
                    var _projectDetail = Common.GetProjectsDetails(projectid);
                    if (_projectDetail != null)
                    {
                        if (model.FromDate != null && model.ToDate != null)
                        {
                            double DiffMonth = model.ToDate.Value.Subtract(model.FromDate.Value).Days + 1;
                            var Days = model.ToDate.Value.Subtract(model.FromDate.Value).Days + 1;
                            decimal totdays = Common.GetAvgDaysInAYear((DateTime)model.FromDate, (DateTime)model.ToDate);
                            var Years = Convert.ToDecimal(Days) / Convert.ToDecimal(totdays);
                            if (model.FromDate.Value.Year == model.ToDate.Value.Year && model.FromDate.Value.Month == 2 && model.FromDate.Value.Day == 1)
                            {
                                if (DiffMonth >= 28)
                                    DiffMonth = 30;
                            }
                            if ((DiffMonth <= 29 || Years > 1) && actenddate < model.FromDate)
                            {
                                msg = msg == "Valid" ? "Appointment tenure should be minimum 1 month to 1 year." : msg + "<br /> Appointment tenure should be minimum 1 month to 1 year.";
                            }
                            if (oldprojectid == projectid)
                            {
                                var currDate = new DateTime(now.Year, now.Month, now.Day);
                                var monthbegin = currDate.AddDays(-currDate.Day).AddDays(+1);
                                if (actstartdate <= monthbegin && monthbegin <= actenddate)
                                    monthbegin = actstartdate;

                                if (olddesid != newdesid && actenddate.AddDays(+1) == model.FromDate)
                                    monthbegin = model.FromDate ?? DateTime.Now;

                                if (monthbegin > model.FromDate)
                                    msg = msg == "Valid" ? "Appointment start date tenure must be month begin." : msg + "<br /> Appointment start date tenure must be month begin.";
                            }
                            DateTime projectstartdate = _projectDetail.SancationDate;
                            DateTime projectenddate = _projectDetail.CloseDate;
                            if (oldprojectid != projectid)
                            {
                                if (!(projectstartdate <= model.FromDate && model.ToDate <= projectenddate && actenddate.AddDays(+1) == model.FromDate))
                                    msg = msg == "Valid" ? "Appointment tenure must be between the project start date and project closure date." : msg + "<br /> Appointment tenure must be between the project start date and project closure date.";
                            }
                            else if (olddesid != newdesid)
                            {
                                if (!((projectstartdate <= model.FromDate && model.ToDate <= projectenddate) && ((actstartdate <= model.FromDate && model.ToDate == actenddate) || actenddate.AddDays(+1) == model.FromDate)))
                                    msg = msg == "Valid" ? "Appointment tenure must be between the project start date and project closure date." : msg + "<br /> Appointment tenure must be between the project start date and project closure date.";
                            }
                            else
                            {
                                if (!(projectstartdate <= model.FromDate && model.ToDate <= projectenddate))
                                    msg = msg == "Valid" ? "Appointment tenure must be between the project start date and project closure date." : msg + "<br /> Appointment tenure must be between the project start date and project closure date.";
                            }
                        }
                        else
                        {
                            msg = msg == "Valid" ? "Enter appointment tenure." : msg + "<br /> Enter appointment tenure.";
                        }
                    }
                }
                else
                {
                    msg = msg == "Valid" ? "Enter valid project number." : msg + "<br /> Enter valid project number.";
                }
            }
            return msg;
        }

        public ActionResult RecruitEnhancementView(int OrderId, string listf = null)
        {
            OrderModel model = new OrderModel();
            try
            {
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                if (OrderId > 0)
                {
                    model = recruitmentService.getOrderDetails(OrderId);
                    model.List_f = getEmployeeActionLink(model.TypeCode, listf);
                    ViewBag.AppointmentType = Common.AppointmentType(model.TypeCode);
                    if (OrderId <= 518)
                    {
                        if (model.FlowApprover == "CMAdmin")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(198, "RecruitEnhAdmin", 0);
                        else if (model.FlowApprover == "NDean")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(198, "RecruitEnhDean", 0);
                        else
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(198, "RecruitEnhancement Flow", 0);
                    }
                    else
                    {
                        string Type = "";
                        if (model.FlowApprover == "NDean")
                            Type = "RCT / " + model.TypeCode + " EN Dean Flow";
                        else if (model.FlowApprover == "CMAdmin")
                            Type = "RCT / " + model.TypeCode + " EN CM Flow";
                        else
                            Type = "RCT / " + model.TypeCode + " EN HR Flow";
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(198, Type, 0);
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(197, "", 0);
                return View(model);
            }
        }

        #endregion

        #region Amendment

        public ActionResult RecruitAmendment(int appid, string apptype, int orderId = 0, string listf = null)
        {
            AmendmentOrderModel model = new AmendmentOrderModel();
            try
            {
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                model = recruitmentService.GetAmendmentOrderDetails(appid, apptype, orderId);
                ViewBag.AppointmentType = Common.AppointmentType(apptype);
                string actionLink = getEmployeeActionLink(apptype, listf);
                int? ordId = null;
                if (orderId > 0)
                    ordId = orderId;
                var data = RequirementService.validPreOrder(model.ApplicationID, model.TypeCode, model.OrderType, ordId);
                if (data.Item1)
                {
                    TempData["alertMsg"] = data.Item2;
                    return RedirectToAction(actionLink);
                }
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(209, "RCTAmendmentOrder", 0);
                model.List_f = actionLink;
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult RecruitAmendment(AmendmentOrderModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.AppointmentType = Common.AppointmentType(model.TypeCode);
                var allowedDoc = new[] { ".doc", ".docx", ".pdf" };
                var validationMsg = ValidateAmendmentFormData(model);
                if (validationMsg != "Valid")
                {
                    model = recruitmentService.GetAmendmentOrderDetails(model.ApplicationID, model.TypeCode, model.OrderID);
                    TempData["errMsg"] = validationMsg;
                    return View(model);
                }
                int? ordId = null;
                if (model.OrderID > 0)
                    ordId = model.OrderID;
                var validationPre = RequirementService.validPreOrder(model.ApplicationID, model.TypeCode, model.OrderType, ordId);
                if (validationPre.Item1)
                {
                    model = recruitmentService.GetAmendmentOrderDetails(model.ApplicationID, model.TypeCode, model.OrderID);
                    TempData["alertMsg"] = validationPre.Item2;
                    return View(model);
                }

                if (model.PILetter != null)
                {
                    string filename = System.IO.Path.GetFileName(model.PILetter.FileName);
                    string extension = Path.GetExtension(filename);
                    if (!allowedDoc.Contains(extension.ToLower()))
                    {
                        model = recruitmentService.GetAmendmentOrderDetails(model.ApplicationID, model.TypeCode, model.OrderID);
                        TempData["errMsg"] = "Please upload any one of these type of pi letter document [.doc,.docx,.pdf]";
                        return View(model);
                    }
                }

                var result = recruitmentService.CommitAmendment(model, userId);
                if (result.Item1 == 1)
                    TempData["succMsg"] = "Request submitted Successfully";
                else if (result.Item1 == 2)
                    TempData["succMsg"] = "Request re-submitted for commitment booking";
                else if (result.Item1 == -1)
                    TempData["succMsg"] = "Request submitted for PI justification";
                else if (result.Item1 == 3)
                    TempData["errMsg"] = result.Item2;
                else
                    TempData["errMsg"] = "Something went wrong please contact administrator";

                return RedirectToAction(model.List_f);

            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.AppointmentType = Common.AppointmentType("");
                return View(model);
            }
        }

        public ActionResult RecruitAmendmentView(int OrderId, string listf = null)
        {
            AmendmentOrderModel model = new AmendmentOrderModel();
            try
            {
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                model = recruitmentService.GetAmendmentOrderDetails(0, "", OrderId);
                if (OrderId <= 518)
                {
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(209, "RCTAmendmentOrder", 0);
                }
                else
                {
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(209, "RCT / " + model.TypeCode + " AM Flow", 0);
                }
                model.List_f = getEmployeeActionLink(model.TypeCode, listf);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                return View(model);
            }
        }

        private string ValidateAmendmentFormData(AmendmentOrderModel model)
        {
            string msg = "Valid";
            using (var context = new IOASDBEntities())
            {
                var mastQuery = context.vw_RCTOverAllApplicationEntry.Where(m => m.ApplicationId == model.ApplicationID && m.Category == model.TypeCode && m.ApplicationType == "New").Select(m => new { m.ProjectId, m.AppointmentStartdate, m.AppointmentEnddate, m.TypeofAppointmentinInt }).FirstOrDefault();
                var queryExp = (from E in context.tblRCTEmployeeExperience
                                where E.ApplicationId == model.ApplicationID
                                orderby E.EffectiveFrom descending
                                select E).FirstOrDefault();
                if (mastQuery != null)
                {
                    var projectDetail = Common.GetProjectsDetails(mastQuery.ProjectId ?? 0);
                    if (projectDetail != null)
                    {
                        if (model.FromDate != null && model.ToDate != null)
                        {
                            int days = model.ToDate.Value.Subtract(model.FromDate.Value).Days + 1;
                            decimal totdays = Common.GetAvgDaysInAYear((DateTime)model.FromDate, (DateTime)model.ToDate);
                            decimal years = Convert.ToDecimal(days) / Convert.ToDecimal(totdays);
                            if (years > 1)
                                msg = msg == "Valid" ? "Appointment tenure should be minimum 1 month to 1 year." : msg + "<br />Appointment tenure should be minimum 1 month to 1 year.";

                            if (!(projectDetail.SancationDate <= model.FromDate && model.ToDate <= projectDetail.CloseDate))
                                msg = msg == "Valid" ? "Appointment tenure must be between the project start date and project closure date." : msg + "<br /> Appointment tenure must be between the project start date and project closure date.";
                            //if (mastQuery.AppointmentStartdate != model.FromDate)
                            //    msg = msg == "Valid" ? "Appointment from date must be same as appointment start date." : msg + "<br /> Appointment from date must be same as appointment start date.";
                            //if (mastQuery.AppointmentEnddate < model.ToDate)
                            //    msg = msg == "Valid" ? "Appointment to date must be lesser than appointment end date." : msg + "<br /> Appointment to date must be lesser than appointment end date.";
                            if (queryExp.EffectiveFrom != model.FromDate)
                                msg = msg == "Valid" ? "Appointment from date must be same as appointment start date." : msg + "<br /> Appointment from date must be same as appointment start date.";
                            if (queryExp.EffectiveTo < model.ToDate)
                                msg = msg == "Valid" ? "Appointment to date must be lesser than appointment end date." : msg + "<br /> Appointment to date must be lesser than appointment end date.";
                            if (model.isWithdrawCommitment != true)
                            {
                                if (Common.IsAvailablefundProject(mastQuery.ProjectId ?? 0, model.CommitmentAmount ?? 0, mastQuery.TypeofAppointmentinInt))
                                    msg = msg == "Valid" ? "Project fund not available." : msg + "<br /> Project fund not available.";
                            }
                        }
                        else
                            msg = msg == "Valid" ? " Enter amendment start date and end date." : msg + "<br /> Enter amendment start date and end date.";
                    }

                }
                else
                    msg = msg == "Valid" ? "Something went wrong please contact administrator." : msg + "<br /> Something went wrong please contact administrator.";
            }
            return msg;
        }

        #endregion

        #region HRA

        public ActionResult HRAList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult getHRAList(int pageIndex, int pageSize, SearchOrderModel model)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = recruitmentService.GetHRAList(pageIndex, pageSize, model, roleid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }

        public ActionResult HRABooking(int Steid, int Orderid = 0, string listf = null)
        {
            HRAOrderModel model = new HRAOrderModel();
            try
            {
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.ProofofAddress = Common.GetCodeControlList("Proof of Address");
                model = recruitmentService.getHRAOrderDetails(Steid, "STE", Orderid);
                model.List_f = getEmployeeActionLink("STE", listf);
                int? ordId = null;
                if (Orderid > 0)
                    ordId = Orderid;
                var Data = RequirementService.validPreOrder(model.ApplicationID, "STE", 5, ordId);
                if (Data.Item1)
                {
                    TempData["alertMsg"] = Data.Item2;
                    return RedirectToAction(model.List_f);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.ProofofAddress = Common.GetCodeControlList("Proof of Address");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult HRABooking(HRAOrderModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.ProofofAddress = Common.GetCodeControlList("Proof of Address");
                var allowedDoc = new[] { ".doc", ".docx", ".pdf" };
                if (model.Status != "Open")
                {
                    if (model.ProofAddress != null)
                    {
                        string filename = System.IO.Path.GetFileName(model.ProofAddress.FileName);
                        string extension = Path.GetExtension(filename);
                        if (!allowedDoc.Contains(extension.ToLower()))
                        {
                            model = recruitmentService.getHRAOrderDetails(model.ApplicationID, "STE", model.OrderID);
                            TempData["alertMsg"] = "Please upload any one of these type address proof document [.doc,.docx,.pdf]";
                            return View(model);
                        }
                        if (model.ProofAddress.ContentLength > 5242880)
                        {
                            model = recruitmentService.getHRAOrderDetails(model.ApplicationID, "STE", model.OrderID);
                            TempData["alertMsg"] = "You can upload address proof up to 5MB";
                            return View(model);
                        }
                    }
                    else
                    {
                        model = recruitmentService.getHRAOrderDetails(model.ApplicationID, "STE", model.OrderID);
                        TempData["alertMsg"] = "Please upload address proof";
                        return View(model);
                    }

                    if (model.HRAForm != null)
                    {
                        string filename = System.IO.Path.GetFileName(model.HRAForm.FileName);
                        string extension = Path.GetExtension(filename);
                        if (!allowedDoc.Contains(extension.ToLower()))
                        {
                            model = recruitmentService.getHRAOrderDetails(model.ApplicationID, "STE", model.OrderID);
                            TempData["errMsg"] = "Please upload any one of these type doc [.doc,.docx,.pdf]";
                            return View(model);
                        }
                        if (model.HRAForm.ContentLength > 5242880)
                        {
                            model = recruitmentService.getHRAOrderDetails(model.ApplicationID, "STE", model.OrderID);
                            TempData["alertMsg"] = "You can upload hra form up to 5MB";
                            return View(model);
                        }
                    }
                    else
                    {
                        model = recruitmentService.getHRAOrderDetails(model.ApplicationID, "STE", model.OrderID);
                        TempData["errMsg"] = "Please upload HRA form file any one of these type doc [.doc,.docx,.pdf]";
                        return View(model);
                    }
                }

                var alertMsg = ValidateHRABookingFormData(model);
                if (alertMsg != "Valid")
                {
                    model = recruitmentService.getHRAOrderDetails(model.ApplicationID, "STE", model.OrderID);
                    TempData["alertMsg"] = alertMsg;
                    return View(model);
                }
                int? ordId = null;
                if (model.OrderID > 0)
                    ordId = model.OrderID;
                var Data = RequirementService.validPreOrder(model.ApplicationID, "STE", 5, ordId);
                if (Data.Item1)
                {
                    model = recruitmentService.getHRAOrderDetails(model.ApplicationID, "STE", model.OrderID);
                    TempData["alertMsg"] = Data.Item2;
                    return RedirectToAction(model.List_f);
                }
                var result = recruitmentService.CommitHRA(model, userId, true);
                if (result.Item1 == 1)
                    TempData["succMsg"] = "HRA Booking Request submitted Successfully";
                else if (result.Item1 == -1)
                    TempData["succMsg"] = "HRA Booking Request submitted for PI justification";
                else if (result.Item1 == 3)
                    TempData["succMsg"] = result.Item2;
                else
                    TempData["errMsg"] = "Something went wrong please contact administrator";

                return RedirectToAction(model.List_f);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.ProofofAddress = Common.GetCodeControlList("Proof of Address");
                return View(model);
            }
        }

        private string ValidateHRABookingFormData(HRAOrderModel model)
        {
            string msg = "Valid";
            using (var context = new IOASDBEntities())
            {
                if (model.TypeCode == "STE")
                {
                    var query = context.tblRCTSTE.FirstOrDefault(m => m.STEID == model.ApplicationID);
                    if (query != null)
                    {
                        if (model.FromDate != null && model.ToDate != null)
                        {
                            if (model.DesignationId != 0 && model.DesignationId == query.DesignationId)
                            {
                                var querydes = context.tblRCTDesignation.FirstOrDefault(M => M.DesignationId == query.DesignationId && M.RecordStatus == "Active");
                                if (querydes != null)
                                {
                                    if (querydes.HRA == true)
                                    {
                                        var hrapre = querydes.HRABasic / 100;
                                        var hraval = query.Salary * hrapre;
                                        if (model.HRA > 0 && model.HRA != hraval)
                                            msg = msg == "Valid" ? "HRA entered exceeds institution norms." : msg + "<br /> HRA entered exceeds institution norms.";
                                    }
                                    else
                                    {
                                        msg = msg == "Valid" ? "Entered designation does not have HRA provision. Please contact administrator." : msg + "<br />Entered designation does not have HRA provision. Please contact administrator.";
                                    }
                                }
                            }
                            else
                                msg = msg == "Valid" ? "HRA booking does not change designation. Please contact administrator." : msg + "<br />HRA booking does not change designation. Please contact administrator.";
                        }
                        else
                            return "Enter appointment tenure.";

                        if (query.HRA > 0)
                            msg = msg == "Valid" ? "Candidate are already have hra." : msg + "<br /> Candidate are already have hra.";

                        if (model.isHRAFullTenure != true)
                        {
                            var setdate = query.AppointmentStartdate ?? DateTime.Now.Date;
                            if (query.AppointmentStartdate <= DateTime.Now.Date)
                            {
                                var beginDate = DateTime.Now.Date.AddDays(-DateTime.Now.Date.Day).AddDays(+1);
                                if (query.AppointmentStartdate <= beginDate && beginDate <= query.AppointmentEnddate)
                                    setdate = beginDate;
                            }
                            if (setdate > model.FromDate || model.ToDate != query.AppointmentEnddate)
                                msg = msg == "Valid" ? "HRA booking tenure must be between the old appointment start date and old appointment closure date." : msg + "<br /> HRA booking tenure must be between the old appointment start date and old appointment closure date.";
                        }
                        else
                        {
                            if (query.AppointmentStartdate > model.FromDate || model.ToDate != query.AppointmentEnddate)
                                msg = msg == "Valid" ? "HRA booking tenure must be between the old appointment start date and old appointment closure date." : msg + "<br /> HRA booking tenure must be between the old appointment start date and old appointment closure date.";
                        }
                    }
                    else
                    {
                        return "Record not found. Something went wrong please contact administrator.";
                    }
                }
            }
            return msg;
        }

        public ActionResult HRACancellation(int Steid = 0, int Orderid = 0, string listf = null)
        {
            HRAOrderModel model = new HRAOrderModel();
            try
            {
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.ProofofAddress = Common.GetCodeControlList("Proof of Address");
                model = recruitmentService.getHRAOrderDetails(Steid, "STE", Orderid, false);
                model.List_f = getEmployeeActionLink("STE", listf);
                int? ordId = null;
                if (model.OrderID > 0)
                    ordId = model.OrderID;
                var Data = RequirementService.validPreOrder(model.ApplicationID, "STE", 6, ordId);
                if (Data.Item1)
                {
                    TempData["alertMsg"] = Data.Item2;
                    return RedirectToAction(model.List_f);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.ProofofAddress = Common.GetCodeControlList("Proof of Address");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult HRACancellation(HRAOrderModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.ProofofAddress = Common.GetCodeControlList("Proof of Address");
                var allowedDoc = new[] { ".doc", ".docx", ".pdf" };
                #region FileValidation    
                if (model.Status != "Open")
                {
                    if (model.PILetter != null)
                    {
                        string filename = System.IO.Path.GetFileName(model.PILetter.FileName);
                        string extension = Path.GetExtension(filename);
                        if (!allowedDoc.Contains(extension.ToLower()))
                        {
                            model = recruitmentService.getHRAOrderDetails(model.ApplicationID, "STE", model.OrderID, false);
                            TempData["errMsg"] = "Please upload any one of these type of pi letter document [.doc,.docx,.pdf]";
                            return View(model);
                        }
                        if (model.PILetter.ContentLength > 5242880)
                        {
                            model = recruitmentService.getHRAOrderDetails(model.ApplicationID, "STE", model.OrderID, false);
                            TempData["alertMsg"] = "You can upload address proof up to 5MB";
                            return View(model);
                        }
                    }
                    else
                    {
                        model = recruitmentService.getHRAOrderDetails(model.ApplicationID, "STE", model.OrderID, false);
                        TempData["errMsg"] = "Please upload pi request document";
                        return View(model);
                    }
                }
                #endregion
                var alertMsg = ValidateHRACancellationFormData(model);
                if (alertMsg != "Valid")
                {
                    model = recruitmentService.getHRAOrderDetails(model.ApplicationID, "STE", model.OrderID, false);
                    TempData["alertMsg"] = alertMsg;
                    return View(model);
                }
                int? ordId = null;
                if (model.OrderID > 0)
                    ordId = model.OrderID;
                var validationPre = RequirementService.validPreOrder(model.ApplicationID, "STE", 6, ordId);
                if (validationPre.Item1)
                {
                    model = recruitmentService.getHRAOrderDetails(model.ApplicationID, "STE", model.OrderID, false);
                    TempData["alertMsg"] = validationPre.Item2;
                    return View(model);
                }
                var result = recruitmentService.CommitHRA(model, userId, false);
                if (result.Item1 == 1)
                    TempData["succMsg"] = "HRA Cancellation Request submitted Successfully";
                else if (result.Item1 == -1)
                    TempData["succMsg"] = "HRA Cancellation Request submitted for PI justification";
                else if (result.Item1 == 3)
                    TempData["succMsg"] = result.Item2;
                else
                    TempData["errMsg"] = "Something went wrong please contact administrator";

                return RedirectToAction(model.List_f);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.ProofofAddress = Common.GetCodeControlList("Proof of Address");
                return View(model);
            }
        }

        private string ValidateHRACancellationFormData(HRAOrderModel model)
        {
            string msg = "Valid";
            using (var context = new IOASDBEntities())
            {
                if (model.FromDate != null && model.ToDate != null)
                {
                    var query = context.tblRCTSTE.FirstOrDefault(m => m.STEID == model.ApplicationID);
                    if (query != null)
                    {
                        var HRAFromDate = query.AppointmentStartdate;
                        var HRAToDate = query.AppointmentEnddate;
                        var querypreodr = (from O in context.tblOrder
                                           where O.AppointmentId == model.ApplicationID && O.AppointmentType == 2
                                           && O.OrderType == 5 && O.Status.Contains("Completed") && O.isUpdated == true
                                           && O.FromDate >= query.AppointmentStartdate && O.ToDate <= query.AppointmentEnddate
                                           orderby O.OrderId descending
                                           select O).FirstOrDefault();
                        if (querypreodr != null)
                        {
                            HRAFromDate = querypreodr.FromDate ?? DateTime.Now;
                            HRAToDate = querypreodr.ToDate ?? DateTime.Now;
                        }

                        var setdate = DateTime.Now.Date;
                        if (HRAFromDate <= setdate)
                        {
                            setdate = setdate.AddDays(-setdate.Day).AddDays(+1);
                            if (HRAFromDate <= setdate && HRAToDate >= setdate)
                                HRAFromDate = setdate;
                        }

                        if (HRAFromDate > model.FromDate || HRAToDate != model.ToDate)
                            msg = msg == "Valid" ? "HRA cancel date must begion of the month start date." : msg + "<br />HRA cancel date must begion of the month start date.";
                    }
                    else
                    {
                        msg = msg == "Valid" ? "Record not found. Something went wrong please contact administrator." : msg + "<br />Record not found. Something went wrong please contact administrator.";
                    }
                }
                else
                {
                    msg = msg == "Valid" ? "Enter appointment tenure." : msg + "<br /> Enter appointment tenure.";
                }
            }
            return msg;
        }

        public ActionResult HRAView(int OrderId, string listf = null)
        {
            HRAOrderModel viewModel = new HRAOrderModel();
            try
            {
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.ProofofAddress = Common.GetCodeControlList("Proof of Address");
                if (OrderId <= 518)
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(190, "HRA Flow", 0);
                else
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(190, "RCT / STE HRA Flow", 0);
                viewModel = recruitmentService.getHRAOrderDetails(viewModel.ApplicationID, "STE", OrderId);
                viewModel.List_f = getEmployeeActionLink("STE", listf);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.ProofofAddress = Common.GetCodeControlList("Proof of Address");
                return View(viewModel);
            }
        }

        #endregion

        #region MaternityLeave

        //public ActionResult MaternityLeaveList()
        //{
        //    return View();
        //}

        [HttpPost]
        public JsonResult getMaternityLeaveList(int pageIndex, int pageSize, SearchOrderModel model, DateFilterModel FromDate, DateFilterModel ToDate)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = recruitmentService.GetMaternityList(pageIndex, pageSize, model, FromDate, ToDate, roleid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }

        public ActionResult MaternityLeave(int appid, string apptype, int orderid = 0, string listf = null)
        {
            OrderModel model = new OrderModel();
            try
            {
                ViewBag.ReferenceType = Common.ReferenceType();
                model = recruitmentService.getMaternityLeaveDetails(appid, apptype, orderid);
                model.List_f = getEmployeeActionLink(model.TypeCode, listf);
                if (model.ApplicationID > 0 && !string.IsNullOrEmpty(model.TypeCode))
                {
                    var isvalid = validatePreMatrnityLeave(model.ApplicationID, model.TypeCode);
                    if (isvalid != "Valid")
                    {
                        TempData["errMsg"] = isvalid;
                        return RedirectToAction(model.List_f);
                    }
                }
                string Type = string.Empty;
                if (orderid <= 518)
                    Type = (model.Status == "Rejoined" || model.Status == "Completed") ? "RCTMaternityLeaveRejoin" : "RCTMaternityLeave";
                else
                    Type = (model.Status == "Rejoined" || model.Status == "Completed") ? "RCT / " + model.TypeCode + " MATR Flow" : "RCT / " + model.TypeCode + " MAT Flow";
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(202, Type, 0);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.ReferenceType = Common.ReferenceType();
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult MaternityLeave(OrderModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.ReferenceType = Common.ReferenceType();
                string Valid = ValidateMaternityformdata(model);
                if (Valid != "Valid")
                {
                    TempData["alertMsg"] = Valid;
                    model = recruitmentService.getMaternityLeaveDetails(model.ApplicationID, model.TypeCode, model.OrderID);
                    return View(model);
                }
                var result = recruitmentService.CommitMaternityLeave(model, userId);
                if (result.Item1 == 1)
                    TempData["succMsg"] = "Maternity Leave Applied Successfully";
                else if (result.Item1 == 2)
                    TempData["succMsg"] = "Rejoining Successfully";
                else
                    TempData["errMsg"] = result.Item2;
                return RedirectToAction(model.List_f);

            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.ReferenceType = Common.ReferenceType();
                TempData["errMsg"] = "Something went wrong please contact administrator";
                return View(model);
            }
        }

        public ActionResult MaternityLeaveView(int OrderId, string listf = null)
        {
            OrderModel model = new OrderModel();
            try
            {
                ViewBag.ReferenceType = Common.ReferenceType();
                model = recruitmentService.getMaternityLeaveDetails(0, "", OrderId);
                if (OrderId <= 518)
                {
                    if (model.Status == "Rejoined")
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(202, "RCTMaternityLeaveRejoin", 0);
                    else
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(202, "RCTMaternityLeave", 0);
                }
                else
                {
                    string Type = "";
                    if (model.Status == "Rejoined")
                        Type = "RCT / " + model.TypeCode + " MATR Flow";
                    else
                        Type = "RCT / " + model.TypeCode + " MAT Flow";
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(202, Type, 0);
                }
                model.List_f = getEmployeeActionLink(model.TypeCode, listf);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(202, "RCTMaternityLeave", 0);
                return View(model);
            }
        }

        public string ValidateMaternityformdata(OrderModel model)
        {
            string msg = "Valid";
            try
            {
                if (model.Status != "Open" && model.Status != "Initiated")
                {
                    if (model.FromDate == null || model.ToDate == null)
                        msg = msg == "Valid" ? "Please enter from date and to date." : msg + "<br /> Please enter from date and to date.";
                    if (model.FromDate >= model.ToDate)
                        msg = msg == "Valid" ? "Please enter to date must be grater than from date." : msg + "<br /> Please enter to date must be grater than from date.";

                    if (model.ApplicationReceiveDate == null)
                        msg = msg == "Valid" ? "ApplicationReceiveDate field is required." : msg + "<br /> ApplicationReceiveDate field is required.";
                    if (model.PILetter != null)
                    {
                        string[] allowedExtensionsDoc = new string[] { ".doc", ".docx", ".pdf" };
                        string filename = System.IO.Path.GetFileName(model.PILetter.FileName);
                        string extension = Path.GetExtension(filename);
                        if (!allowedExtensionsDoc.Contains(extension.ToLower()))
                            msg = msg == "Valid" ? "Please upload any one of these type of pi letter document [.doc,.docx,.pdf]." : msg + "<br /> Please upload any one of these type of pi letter document [.doc,.docx,.pdf].";
                        if (model.PILetter.ContentLength > 5242880)
                            msg = msg == "Valid" ? "You can upload pi letter document up to 5MB." : msg + "<br /> You can upload pi letter document up to 5MB.";
                    }
                    else
                    {
                        if (model.OrderID == 0)
                            msg = msg == "Valid" ? "Please upload pi request document." : msg + "<br /> Please upload pi request document.";
                    }

                    var Days = model.ToDate.Value.Subtract(model.FromDate.Value).TotalDays + 1;

                    decimal YearDays = Common.GetAvgDaysInAYear((DateTime)model.FromDate, (DateTime)model.ToDate);

                    decimal permonth = Convert.ToDecimal(YearDays) / Convert.ToDecimal(12);
                    var monthDiff = Convert.ToDecimal(Days) / permonth;
                    monthDiff = decimal.Round(monthDiff);
                    if (monthDiff > 6)
                        msg = msg == "Valid" ? "Maternity leave tenure should be below 6 months." : msg + "<br />Maternity leave tenure should be below 6 months.";


                    using (var context = new IOASDBEntities())
                    {
                        var appointmenttype = RequirementService.getAppointmentType(model.TypeCode);
                        string[] notexpstatus = new string[] { "Rejected", "Canceled", "Cancel" };
                        if (model.Status != "PI Initiated")
                        {
                            if (context.tblOrder.Any(m => m.AppointmentId == model.ApplicationID && m.AppointmentType == appointmenttype && m.OrderType == 10))
                            {

                                var query = (from o in context.tblOrder
                                             from od in context.tblOrderDetail
                                             where o.OrderId == od.OrderId && o.AppointmentId == model.ApplicationID
                                             && o.AppointmentType == appointmenttype
                                             && !notexpstatus.Contains(o.Status) && o.OrderType == 10
                                             && ((od.RejoinDate == null || od.RejoinDate < model.FromDate) && (od.RejoinDate != null || o.ToDate < model.FromDate))
                                             orderby o.OrderId descending
                                             select o).FirstOrDefault();

                                if (query == null)
                                    msg = msg == "Valid" ? "Maternity leave already available for this tenure." : msg + "<br /> Maternity leave already available for this tenure.";

                            }
                        }
                        int?[] exceptedType = new int?[] { 1, 2, 3 };
                        if (context.tblOrder.Any(m => m.AppointmentId == model.ApplicationID && m.AppointmentType == appointmenttype && exceptedType.Contains(m.OrderType) && !notexpstatus.Contains(m.Status) && m.isUpdated != true && (m.FromDate <= model.ToDate/* || m.FromDate <= model.FromDate*/)))
                            msg = msg == "Valid" ? "Maternity leave only for the current tenure." : msg + "<br /> Maternity leave only for the current tenure.";

                    }
                }

                if (model.Status == "Open")
                {
                    if (model.RejoiningLetter != null)
                    {
                        string[] allowedExtensionsDoc = new string[] { ".doc", ".docx", ".pdf" };
                        string filename = System.IO.Path.GetFileName(model.RejoiningLetter.FileName);
                        string extension = Path.GetExtension(filename);
                        if (!allowedExtensionsDoc.Contains(extension.ToLower()))
                            msg = msg == "Valid" ? "Please upload any one of these type of rejoin letter [.doc,.docx,.pdf]." : msg + "<br /> Please upload any one of these type of rejoin letter [.doc,.docx,.pdf].";
                        if (model.RejoiningLetter.ContentLength > 5242880)
                            msg = msg == "Valid" ? "You can upload rejoin letter up to 5MB." : msg + "<br /> You can upload rejoin letter up to 5MB.";
                    }
                    else
                        msg = msg == "Valid" ? "Please upload rejoin letter." : msg + "<br /> Please upload rejoin letter.";
                }
                return msg;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string validatePreMatrnityLeave(int appid, string category)
        {
            try
            {
                string res = "Valid";
                using (var context = new IOASDBEntities())
                {
                    if (appid > 0 && !string.IsNullOrEmpty(category))
                    {
                        int appointmenttype = RequirementService.getAppointmentType(category);

                        if (!context.vw_RCTOverAllApplicationEntry.Any(m => m.ApplicationId == appid && m.Category == category && m.isEmployee == true && m.ApplicationType == "New" && m.Sex == 2))
                            return "Maternity leave not applicable for this candidate.";

                        var totalMaternity = (from O in context.tblOrder
                                              where O.Status == "Completed" & O.OrderType == 10
                                              && O.AppointmentType == appointmenttype && O.AppointmentId == appid
                                              select O).Count();
                        if (totalMaternity >= 2)
                            return "Maternity leave cannot be availed more than two times.";

                        if (context.tblOrder.Any(m => m.Status == "Open" && m.OrderType == 10 && m.AppointmentType == appointmenttype && m.AppointmentId == appid))
                            return "Valid";
                        if (context.tblOrder.Any(m => m.Status == "PI Initiated" && m.OrderType == 10 && m.AppointmentType == appointmenttype && m.AppointmentId == appid))
                            return "Valid";
                        if (context.tblOrder.Any(m => m.Status == "Initiated" && m.OrderType == 10 && m.AppointmentType == appointmenttype && m.AppointmentId == appid && m.Is_Clarify == true))
                            return "Valid";

                        if (context.tblOrder.Any(m => (m.Status != "Completed" && m.Status != "Cancel" && m.Status != "Rejected") && m.OrderType == 10 && m.AppointmentType == appointmenttype && m.AppointmentId == appid))
                            return "Previous order is pending.";
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return "Valid";
            }
        }

        #endregion

        #region Stop payment and Loss of pay

        //public ActionResult StoppaymentList()
        //{
        //    return View();
        //}

        [HttpPost]
        public JsonResult getStoppaymentList(int pageIndex, int pageSize, SearchOrderModel model)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = recruitmentService.GetLOPSPList(pageIndex, pageSize, model, roleid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }

        public ActionResult Stoppayment(int appid, string apptype, int orderid = 0, bool? islosspay = null, string listf = null)
        {
            StopaymentlosspayModel model = new StopaymentlosspayModel();
            try
            {
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.Meridiem = Common.GetCodeControlList("Meridiem");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.Signature = Common.GetCodeControlList("Signature");
                if (orderid > 0)
                    model = recruitmentService.getSPLOPDetailsbyorder(orderid);
                else if (appid > 0 && recruitmentService.IsEmployee(appid, apptype))
                    model = recruitmentService.getSPLOPDetails(appid, apptype, islosspay ?? false);
                model.List_f = getEmployeeActionLink(apptype, listf);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.Meridiem = Common.GetCodeControlList("Meridiem");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.Signature = Common.GetCodeControlList("Signature");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Stoppayment(StopaymentlosspayModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.Meridiem = Common.GetCodeControlList("Meridiem");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.Signature = Common.GetCodeControlList("Signature");
                if (ModelState.IsValid)
                {
                    if (model.OrderID == null)
                    {
                        var ValidForm = ValidatestoplosspayForm(model);
                        if (ValidForm != "Valid")
                        {
                            TempData["alertMsg"] = ValidForm;
                            return View(model);
                        }
                    }

                    if (model.PILetter != null)
                    {
                        var allowedextension = new[] { ".doc", ".docx", ".pdf" };
                        string filename = System.IO.Path.GetFileName(model.PILetter.FileName);
                        string extension = Path.GetExtension(filename);
                        if (!allowedextension.Contains(extension.ToLower()))
                        {
                            TempData["alertMsg"] = "Please upload any one of these type address proof document [.doc,.docx,.pdf]";
                            return View(model);
                        }
                        if (model.PILetter.ContentLength > 5242880)
                        {
                            TempData["alertMsg"] = "You can upload address proof up to 5MB";
                            return View(model);
                        }
                    }
                    else
                    {
                        if (model.OrderID == null)
                        {
                            TempData["alertMsg"] = "Please upload address proof";
                            return View(model);
                        }
                    }

                    if (model.Status == "Approved")
                    {
                        if (model.RejoiningLetter != null)
                        {
                            var allowedextension = new[] { ".doc", ".docx", ".pdf" };
                            string filename = System.IO.Path.GetFileName(model.RejoiningLetter.FileName);
                            string extension = Path.GetExtension(filename);
                            if (!allowedextension.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type rejoining letter document [.doc,.docx,.pdf]";
                                return View(model);
                            }
                            if (model.RejoiningLetter.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload rejoining letter up to 5MB";
                                return View(model);
                            }
                        }
                        else
                        {
                            TempData["alertMsg"] = "Please upload rejoining letter ";
                            return View(model);
                        }
                    }

                    var result = recruitmentService.CommitSPLOP(model, userId);
                    if (result == 1)
                    {
                        if (model.OrderType == 8)
                            TempData["succMsg"] = "Loss of pay request placed Successfully";
                        else
                            TempData["succMsg"] = "Stop payment request placed Successfully";

                    }
                    else if (result == 2)
                    {
                        if (model.OrderType == 8)
                            TempData["succMsg"] = "Loss of pay removed";
                        else
                            TempData["succMsg"] = "Stop payment removed";
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator";

                    return RedirectToAction(model.List_f);
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
                WriteLog.SendErrorToText(ex);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.Meridiem = Common.GetCodeControlList("Meridiem");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.Signature = Common.GetCodeControlList("Signature");
                TempData["errMsg"] = "Something went wrong please contact administrator";
                return View(model);
            }
        }

        private string ValidatestoplosspayForm(StopaymentlosspayModel model)
        {
            var appointmentstartdate = DateTime.Now;
            var appointmentenddate = DateTime.Now;
            var msg = "Valid";
            using (var context = new IOASDBEntities())
            {
                var validationPre = RequirementService.validPreOrder(model.ApplicationID, model.TypeCode, model.OrderType ?? 0, model.OrderID);
                if (validationPre.Item1)
                    return validationPre.Item2;

                if (model.FromDate != null && model.ToDate != null)
                {
                    var query = context.vw_RCTOverAllApplicationEntry.Where(m => m.ApplicationId == model.ApplicationID && m.Category == model.TypeCode && m.ApplicationType == "New" && m.Status == "Verification Completed").Select(m => new { m.AppointmentStartdate, m.AppointmentEnddate }).FirstOrDefault();
                    if (query != null)
                    {
                        appointmentstartdate = query.AppointmentStartdate ?? DateTime.Now;
                        appointmentenddate = query.AppointmentEnddate ?? DateTime.Now;
                    }
                    else
                    {
                        msg = msg == "Valid" ? "Record not found. Something went wrong please contact administrator." : msg + "<br />Record not found. Something went wrong please contact administrator.";
                    }

                    var setdate = DateTime.Now.Date;
                    setdate = setdate.AddDays(-setdate.Day).AddDays(+1);

                    if (appointmentstartdate <= setdate && appointmentenddate >= setdate)
                        appointmentstartdate = setdate;

                    if (appointmentstartdate > model.FromDate || appointmentenddate < model.ToDate)
                        msg = msg == "Valid" ? "Loss of pay / Stop payment can be enabled only from current date or current month." : msg + "<br />Loss of pay / Stop payment can be enabled only from current date or current month.";

                    var appointmenttype = RequirementService.getAppointmentType(model.TypeCode);

                    int?[] exceptedType = { 7, 8 };
                    string[] notexpstatus = new string[] { "Rejected", "Canceled", "Cancel" };
                    DateTime? FromDate = model.FromDate.Value.AddHours(+12);
                    if (model.FromMeridiem == 2)
                        FromDate = model.FromDate.Value.AddHours(+24);

                    if (context.tblOrder.Any(m => m.AppointmentId == model.ApplicationID && m.AppointmentType == appointmenttype && exceptedType.Contains(m.OrderType ?? 0) && !notexpstatus.Contains(m.Status) && m.FromDate != m.ToDate && m.ToDate >= FromDate)) //Is only check loss of pay stop payment
                        msg = msg == "Valid" ? "stop payment / loss of pay already available for this tenure." : msg + "<br />stop payment / loss of pay already available for this tenure.";

                    if (context.tblOrder.Any(m => m.AppointmentId == model.ApplicationID && m.AppointmentType == appointmenttype && m.OrderType == 10 && !notexpstatus.Contains(m.Status) && m.ToDate >= model.FromDate)) //Is only check loss of pay stop payment
                        msg = msg == "Valid" ? "Maternity leave already available for this tenure." : msg + "<br /> Maternity leave already available for this tenure.";

                    int?[] exceptedTypeids = new int?[] { 1, 2, 3, 4, 5, 6 };
                    if (context.tblOrder.Any(m => m.AppointmentId == model.ApplicationID && m.AppointmentType == appointmenttype && !notexpstatus.Contains(m.Status) && exceptedTypeids.Contains(m.OrderType ?? 0) && m.isUpdated != true && m.FromDate <= model.ToDate)) //Check SP/LO order affect pending Orders or not
                        msg = msg == "Valid" ? "stop payment / loss of pay can be initiated only for the current tenure." : msg + "<br />stop payment / loss of pay can be initiated only for the current tenure.";

                }
                else
                    msg = msg == "Valid" ? "Please enter from date and to date." : msg + "<br />Please enter from date and to date.";

                return msg;
            }
        }

        public ActionResult StoppaymentView(int OrderId)
        {
            StopaymentlosspayModel model = new StopaymentlosspayModel();
            try
            {
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.Meridiem = Common.GetCodeControlList("Meridiem");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.Signature = Common.GetCodeControlList("Signature");
                if (OrderId > 0)
                {
                    model = recruitmentService.getSPLOPDetailsbyorder(OrderId);
                    if (OrderId <= 518)
                    {
                        if (model.Status == "Initiated" || model.Status == "Approved")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(203, "Stop/Loss pay approval", 0);
                        if (model.Status == "Reversal" || model.Status == "Completed")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(203, "Stop/Loss pay reversal", 0);
                    }
                    else
                    {
                        string Type = "";
                        if ((model.Status == "Initiated" || model.Status == "Approved") && model.OrderType == 8)
                            Type = "RCT / " + model.TypeCode + " LOP Flow";
                        else if ((model.Status == "Initiated" || model.Status == "Approved") && model.OrderType == 7)
                            Type = "RCT / " + model.TypeCode + " SP Flow";
                        else if ((model.Status == "Reversal" || model.Status == "Completed") && model.OrderType == 7)
                            Type = "RCT / " + model.TypeCode + " SPR Flow";
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(203, Type, 0);
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.Meridiem = Common.GetCodeControlList("Meridiem");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.Signature = Common.GetCodeControlList("Signature");
                return View(model);
            }
        }

        #endregion

        #region Relieving

        //public ActionResult RelievingList()
        //{
        //    return View();
        //}

        [HttpPost]
        public JsonResult getRelievingList(int pageIndex, int pageSize, SearchOrderModel model, DateFilterModel EmployeeDateofBirth)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = recruitmentService.GetRelievingList(pageIndex, pageSize, model, roleid, EmployeeDateofBirth);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }

        public ActionResult RelievingView(int OrderId, string listf = null)
        {
            RelievingModel model = new RelievingModel();
            try
            {
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.RelieveMode = Common.GetCodeControlList("RelieveMode");
                ViewBag.CommitmentOptions = Common.GetCodeControlList("Commitment Option");
                if (OrderId > 0)
                    model = recruitmentService.getRelievingDetails(0, string.Empty, OrderId);
                if (OrderId <= 518)
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(201, "RCTRelieving", 0);
                else
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(201, "RCT / " + model.TypeCode + " RE Flow", 0);
                model.List_f = getEmployeeActionLink(model.TypeCode, listf);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.RelieveMode = Common.GetCodeControlList("RelieveMode");
                ViewBag.CommitmentOptions = Common.GetCodeControlList("Commitment Option");
                return View(model);
            }
        }

        public ActionResult Relieving(int appid, string apptype, int orderid = 0, string listf = null)
        {
            RelievingModel model = new RelievingModel();
            try
            {

                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.RelieveMode = Common.GetCodeControlList("RelieveMode");
                ViewBag.CommitmentOptions = Common.GetCodeControlList("Commitment Option");
                ViewBag.BackdatedEnabled = System.Web.Configuration.WebConfigurationManager.AppSettings["BackdateRelievingEnabled"];
                model = recruitmentService.getRelievingDetails(appid, apptype, orderid);
                model.List_f = getEmployeeActionLink(model.TypeCode, listf);
                var Valid = validateRelieving(model.ApplicationID, model.TypeCode);
                if (Valid != "Valid")
                {
                    TempData["alertMsg"] = Valid;
                    return RedirectToAction(model.List_f);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.RelieveMode = Common.GetCodeControlList("RelieveMode");
                ViewBag.CommitmentOptions = Common.GetCodeControlList("Commitment Option");
                ViewBag.BackdatedEnabled = System.Web.Configuration.WebConfigurationManager.AppSettings["BackdateRelievingEnabled"];
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Relieving(RelievingModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                RequirementService recruitmentService = new RequirementService();
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.RelieveMode = Common.GetCodeControlList("RelieveMode");
                ViewBag.CommitmentOptions = Common.GetCodeControlList("Commitment Option");
                ViewBag.BackdatedEnabled = System.Web.Configuration.WebConfigurationManager.AppSettings["BackdateRelievingEnabled"];
                var allowedExtensionsDoc = new[] { ".doc", ".docx", ".pdf" };
                if (ModelState.IsValid)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = context.vw_RCTOverAllApplicationEntry.Where(m => m.ApplicationId == model.ApplicationID && m.Category == model.TypeCode && m.ApplicationType == "New" && m.isEmployee == true).Select(x => new { x.AppointmentStartdate, x.AppointmentEnddate }).FirstOrDefault();
                        if (query != null && model.RelievingDate != null)
                        {
                            var begindate = DateTime.Now.Date.AddDays(-DateTime.Now.Day).AddDays(+1);
                            begindate = begindate.AddDays(-7);
                            var startdatepicker = query.AppointmentStartdate ?? DateTime.Now;
                            var enddatepicker = query.AppointmentEnddate ?? DateTime.Now;
                            if (ViewBag.BackdatedEnabled == "false")
                            {
                                if (query.AppointmentStartdate <= begindate && query.AppointmentEnddate >= begindate)
                                    startdatepicker = begindate;
                                if (enddatepicker >= DateTime.Now.Date)
                                    enddatepicker = DateTime.Now.Date;
                                if (model.Spcomerelieving_f == true)
                                {
                                    int appTypeId = RequirementService.getAppointmentType(model.TypeCode);
                                    int maxOrderId = (from m in context.tblOrder where m.AppointmentId == model.ApplicationID && m.AppointmentType == appTypeId select m.OrderId).Max();
                                    var queryordr = context.tblOrder.Where(m => m.AppointmentId == model.ApplicationID && m.AppointmentType == appTypeId && m.OrderType == 7 && m.Status == "Completed" && (m.FromDate == m.ToDate || m.FromDate > m.ToDate) && m.OrderId == maxOrderId).FirstOrDefault();
                                    if (queryordr != null)
                                    {
                                        startdatepicker = queryordr.FromDate.Value.AddDays(-1);
                                        model.Spcomerelieving_f = true;
                                    }
                                }
                            }
                            if (!(startdatepicker <= model.RelievingDate && model.RelievingDate <= enddatepicker))
                            {
                                TempData["alertMsg"] = "Please ender valid relieve date";
                                return View(model);
                            }
                        }
                    }

                    if (model.PILetter != null)
                    {
                        string filename = System.IO.Path.GetFileName(model.PILetter.FileName);
                        string extension = Path.GetExtension(filename);
                        if (!allowedExtensionsDoc.Contains(extension.ToLower()))
                        {
                            TempData["alertMsg"] = "Please upload any one of these type of pi letter document [.doc,.docx,.pdf]";
                            return View(model);
                        }
                        if (model.PILetter.ContentLength > 5242880)
                        {
                            TempData["alertMsg"] = "You can upload address proof up to 5MB";
                            return View(model);
                        }
                    }
                    else
                    {
                        if (model.OrderID == null)
                        {
                            TempData["alertMsg"] = "Upload pi request document";
                            return View(model);
                        }
                    }

                    if (model.NODuesFile != null)
                    {
                        foreach (var fileNOC in model.NODuesFile)
                        {
                            if (fileNOC != null)
                            {
                                string filename = System.IO.Path.GetFileName(fileNOC.FileName);
                                string extension = Path.GetExtension(filename);
                                if (!allowedExtensionsDoc.Contains(extension.ToLower()))
                                {
                                    TempData["alertMsg"] = "Please upload any one of these type of noc document [.doc,.docx,.pdf]";
                                    return View(model);
                                }
                                if (fileNOC.ContentLength > 5242880)
                                {
                                    TempData["alertMsg"] = "You can upload noc up to 5MB";
                                    return View(model);
                                }
                            }
                        }
                    }

                    var result = recruitmentService.CommitRelieving(model, userId);
                    if (result.Item1 == 0)
                        TempData["errMsg"] = result.Item2;
                    else
                        TempData["succMsg"] = result.Item2;
                    return RedirectToAction(model.List_f);

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
                WriteLog.SendErrorToText(ex);
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.RelieveMode = Common.GetCodeControlList("RelieveMode");
                ViewBag.CommitmentOptions = Common.GetCodeControlList("Commitment Option");
                ViewBag.BackdatedEnabled = System.Web.Configuration.WebConfigurationManager.AppSettings["BackdateRelievingEnabled"];
                return View(model);
            }
        }

        public static string validateRelieving(int appid, string category)
        {
            try
            {
                string res = "Valid";
                using (var context = new IOASDBEntities())
                {
                    if (appid > 0 && !string.IsNullOrEmpty(category))
                    {
                        int appointmenttype = RequirementService.getAppointmentType(category);
                        if (context.tblOrder.Any(m => m.AppointmentId == appid && m.AppointmentType == appointmenttype && m.OrderType == 9 && (m.Status == "Open" || m.Status == "PI Initiated")))
                            return "Valid";
                        if (context.tblOrder.Any(m => m.AppointmentId == appid && m.AppointmentType == appointmenttype && m.OrderType == 9 && m.Status == "Relieving initiated"))
                            return "Already relieving request under process.";
                        if (context.tblOrder.Any(m => m.AppointmentId == appid && m.AppointmentType == appointmenttype && m.OrderType == 9 && m.Status == "Completed"))
                            return "Already employee relieved.";
                        if (context.tblOrder.Any(m => m.AppointmentId == appid && m.AppointmentType == appointmenttype && (m.Status != "Rejected" && m.Status != "Canceled" && m.Status != "Cancel" && m.Status != "Completed")))
                            return "Order pending against the Employee. To proceed, please cancel the pending order.";
                        if (context.tblOrder.Any(m => m.AppointmentId == appid && m.AppointmentType == appointmenttype && m.Status == "Completed" && m.isUpdated != true))
                            return "Order pending against the Employee. To proceed, please cancel the pending order.";
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        #endregion

        #endregion

        #endregion

        [HttpPost]
        public JsonResult CalculateWithdrawalAmount(int appid, string apptype, DateTime From, DateTime To, bool isIncludedate = true)
        {
            try
            {
                decimal res = 0;
                if (appid > 0 && !string.IsNullOrEmpty(apptype))
                    res = Common.calculateWithdrawalAmount(appid, apptype, From, To, isIncludedate);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult calculateWithdrawalAmountlossofPay(int appid, string apptype, DateTime From, DateTime To, bool isIncludedate = true)
        {
            try
            {
                decimal res = 0;
                if (appid > 0 && !string.IsNullOrEmpty(apptype))
                    res = Common.calculateWithdrawalAmountlossofPay(appid, apptype, From, To, isIncludedate);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }


        #region CommitmentRequest

        public ActionResult RecruitmentCommitmentRequestList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetCommitReqList(CommitReqstSearchModel model, int pageIndex, int pageSize)
        {
            try
            {
                object output = recruitmentService.GetCommitRequestList(model, pageIndex, pageSize);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }

        public ActionResult RecruitmentCommitmentBooking(int Id = 0, string apptype = null)
        {
            try
            {
                STEViewModel model = new STEViewModel();
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                model = recruitmentService.GetRecruitBookCommitDetails(Id, apptype);
                if (apptype == "Change of Project")

                {
                    ViewBag.ChangeOfProjectFreeze = RequirementService.GetFreezeDataForChangeOfProject(model.ApplicationNo);
                }
                 
                //if(apptype == "Change of Project")
                //{
                //    model = 
                //}
                ProjSummaryModel psModel = new ProjSummaryModel();
                //ProjectSummaryModel psModel = new ProjectSummaryModel();
                ProjectService pro = new ProjectService();
                var ProjectId = model.ProjectId ?? 0;
                if (ProjectId > 0)
                {
                    ViewBag.AllocationHead = Common.getAllocationHeadBasedOnProject(ProjectId);
                    psModel.Detail = pro.getProjectSummaryDetails(ProjectId);
                    psModel.Summary = pro.getProjectSummary(ProjectId);
                    psModel.Common = Common.GetProjectsDetails(ProjectId);
                    psModel.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.SancationDate);
                    psModel.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.CloseDate);
                    psModel.ProjId = ProjectId;
                    psModel.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                    psModel.ExpAmt = psModel.Summary.AmountSpent; 
                    model.Projsummary = psModel;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult RecruitmentCommitmentBooking(STEViewModel model)
        {
            try
            {
                lock (lockCommitbookrequestObj)
                {
                    bool isValidRequest = Common.CheckRCTCommitmentRequest(model.CommitReqModel.CommitmentRequestId);
                    if (isValidRequest)
                    {
                        CommitmentModel commit = new CommitmentModel();
                        ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                        ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                        var username = User.Identity.Name;
                        int userid = Common.GetUserid(username);
                        //model = recruitmentService.GetRecruitBookCommitDetails(Id);
                        ProjSummaryModel psModel = new ProjSummaryModel();
                        //ProjectSummaryModel psModel = new ProjectSummaryModel();
                        ProjectService pro = new ProjectService();
                        var ProjectId = model.ProjectId ?? 0;
                        if (ProjectId > 0)
                        {
                            ViewBag.AllocationHead = Common.getAllocationHeadBasedOnProject(ProjectId);
                            psModel.Detail = pro.getProjectSummaryDetails(ProjectId);
                            psModel.Summary = pro.getProjectSummary(ProjectId);
                            psModel.Common = Common.GetProjectsDetails(ProjectId);
                            psModel.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.SancationDate);
                            psModel.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.CloseDate);
                            psModel.ProjId = ProjectId;
                            psModel.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                            psModel.ExpAmt = psModel.Summary.AmountSpent;
                            model.Projsummary = psModel;
                            commit.BankId = psModel.Common.BankId;
                        }
                        commit.commitmentValue = model.CommitReqModel.CommitmentAmount ?? 0;
                        commit.selAllocationHead = model.CommitReqModel.AllocationHeadId ?? 0;
                        commit.Remarks = model.CommitReqModel.Remarks;
                        commit.SelProjectNumber = model.ProjectId ?? 0;
                        commit.AllocationValue = model.CommitReqModel.CommitmentAmount ?? 0;
                        commit.selCommitmentType = 1;
                        var basicpayandmedical = Common.getbasicpay(model.CommitReqModel.ReferenceNumber, model.CommitReqModel.AppointmentTypeCode);
                        commit.BasicPay = basicpayandmedical.Item1;
                        commit.MedicalAllowance = basicpayandmedical.Item2;
                        commit.StartDate = basicpayandmedical.Item3;
                        commit.CloseDate = basicpayandmedical.Item4;

                        AccountService _AS = new AccountService();
                        var commitmentId = _AS.SaveCommitDetails(commit, userid, true);
                        if (commitmentId.Item1 > 1)
                        {
                            var comitreqid = 0;
                            if (model.CommitReqModel.ApplicationType == "Change of Project" || model.CommitReqModel.ApplicationType == "Enhancement" || model.CommitReqModel.ApplicationType == "Extension")
                            {
                                comitreqid = recruitmentService.UpdateNewCommitDetails(model, commitmentId.Item1, userid);
                            }
                            else
                            {
                                comitreqid = recruitmentService.UpdateCommitDetails(model, commitmentId.Item1, userid);
                            }

                            if (comitreqid > 0)
                            {
                                TempData["succMsg"] = "Commitment booked successfully.";
                                return RedirectToAction("RecruitmentCommitmentRequestList", "Requirement");
                            }
                            else
                            {
                                TempData["errMsg"] = "Commitment booked but something went wrong in data updation. Please contact administrator";
                                return RedirectToAction("RecruitmentCommitmentRequestList", "Requirement");
                            }
                        }
                        else
                        {
                            model = recruitmentService.GetRecruitBookCommitDetails(model.CommitReqModel.CommitmentRequestId, model.CommitReqModel.ApplicationType);
                            psModel.Detail = pro.getProjectSummaryDetails(ProjectId);
                            psModel.Summary = pro.getProjectSummary(ProjectId);
                            psModel.Common = Common.GetProjectsDetails(ProjectId);
                            psModel.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.SancationDate);
                            psModel.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.CloseDate);
                            psModel.ProjId = ProjectId;
                            psModel.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                            psModel.ExpAmt = psModel.Summary.AmountSpent;
                            model.Projsummary = psModel;

                            TempData["errMsg"] = commitmentId.Item2;
                            return View(model);
                        }
                    }
                    else
                    {
                        TempData["errMsg"] = "This Request already Booked are not Vaild booking Request.";
                        return RedirectToAction("RecruitmentCommitmentRequestList", "Requirement");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return View(model);
                throw ex;
            }
        }

        public ActionResult RCTCommitmentBookingView(int Id = 0, string apptype = null)
        {
            try
            {
                STEViewModel model = new STEViewModel();
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                var type = Common.GetApplTypebyReqId(Id);
                apptype = type.Item1;
                var reqtype = type.Item2;
                if (reqtype == "New Appointment" || reqtype == "New Commitment")
                {
                    model = recruitmentService.GetRecruitBookCommitDetails(Id, apptype, true);
                }
                else
                {
                    model = recruitmentService.GetAddorWithdrawCommitDetails(Id);
                }

                ProjSummaryModel psModel = new ProjSummaryModel();
                //ProjectSummaryModel psModel = new ProjectSummaryModel();
                ProjectService pro = new ProjectService();
                var ProjectId = model.ProjectId ?? 0;
                if (ProjectId > 0)
                {
                    ViewBag.AllocationHead = Common.getAllocationHeadBasedOnProject(ProjectId);
                    psModel.Detail = pro.getProjectSummaryDetails(ProjectId);
                    psModel.Summary = pro.getProjectSummary(ProjectId);
                    psModel.Common = Common.GetProjectsDetails(ProjectId);
                    psModel.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.SancationDate);
                    psModel.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.CloseDate);
                    psModel.ProjId = ProjectId;
                    psModel.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                    psModel.ExpAmt = psModel.Summary.AmountSpent;
                    model.Projsummary = psModel;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }

        public ActionResult RCTAddCommitment(int Id = 0)
        {
            try
            {
                STEViewModel model = new STEViewModel();
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.Reason = Common.GetCommitmentAction();
                var Reqsttype = Common.GetRequestType(Id);
                if (Reqsttype != "Add Commitment")
                {
                    TempData["errMsg"] = "Add Commitment request can't be done because the request type is - " + Reqsttype;
                    return RedirectToAction("RecruitmentCommitmentRequestList");
                }
                model = recruitmentService.GetAddorWithdrawCommitDetails(Id);
                if (model.STEId == -1 && model.Status != "Awaiting Commitment Booking")
                {
                    TempData["errMsg"] = "Add Commitment request already done or does not exist";
                    return RedirectToAction("RecruitmentCommitmentRequestList");
                }
                ProjSummaryModel psModel = new ProjSummaryModel();
                //ProjectSummaryModel psModel = new ProjectSummaryModel();
                ProjectService pro = new ProjectService();
                var ProjectId = model.ProjectId ?? 0;
                if (ProjectId > 0)
                {
                    ViewBag.AllocationHead = Common.getAllocationHeadBasedOnProject(ProjectId);
                    psModel.Detail = pro.getProjectSummaryDetails(ProjectId);
                    psModel.Summary = pro.getProjectSummary(ProjectId);
                    psModel.Common = Common.GetProjectsDetails(ProjectId);
                    psModel.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.SancationDate);
                    psModel.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.CloseDate);
                    psModel.ProjId = ProjectId;
                    psModel.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                    psModel.ExpAmt = psModel.Summary.AmountSpent;
                    model.Projsummary = psModel;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult RCTAddCommitment(STEViewModel model)
        {
            try
            {
                lock (lockCommitAddrequestObj)
                {
                    bool isValidRequest = Common.CheckRCTCommitmentRequest(model.CommitReqModel.CommitmentRequestId);
                    if (isValidRequest)
                    {
                        CommitmentResultModel commit = new CommitmentResultModel();
                        ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                        ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                        ViewBag.Reason = Common.GetCommitmentAction();
                        var username = User.Identity.Name;
                        int userid = Common.GetUserid(username);

                        //model = recruitmentService.GetRecruitBookCommitDetails(Id);
                        ProjSummaryModel psModel = new ProjSummaryModel();
                        //ProjectSummaryModel psModel = new ProjectSummaryModel();
                        ProjectService pro = new ProjectService();
                        var ProjectId = model.ProjectId ?? 0;
                        if (ProjectId > 0)
                        {
                            ViewBag.AllocationHead = Common.getAllocationHeadBasedOnProject(ProjectId);
                            psModel.Detail = pro.getProjectSummaryDetails(ProjectId);
                            psModel.Summary = pro.getProjectSummary(ProjectId);
                            psModel.Common = Common.GetProjectsDetails(ProjectId);
                            psModel.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.SancationDate);
                            psModel.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.CloseDate);
                            psModel.ProjId = ProjectId;
                            psModel.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                            psModel.ExpAmt = psModel.Summary.AmountSpent;
                            model.Projsummary = psModel;
                        }
                        commit.LogTypeId = 1;
                        commit.ComitmentId = model.CommitReqModel.CommitmentId ?? 0;
                        commit.strRemarks = model.CommitReqModel.Remarks;
                        commit.AddCloseAmt = model.CommitReqModel.AddCommitmentAmount ?? 0;
                        commit.ProjectId = ProjectId;
                        commit.Reason = model.CommitReqModel.Reason;
                        commit.AllHeadId = model.CommitReqModel.AllocationHeadId ?? 0;
                        //commit.selAllocationHead = model.CommitReqModel.AllocationHeadId ?? 0;
                        commit.Remarks = model.CommitReqModel.Remarks;
                        //commit.SelProjectNumber = model.ProjectId ?? 0;
                        //commit.AllocationValue = model.CommitReqModel.CommitmentAmount ?? 0;
                        //commit.selCommitmentType = 1;
                        //var basicpayandmedical = Common.getbasicpay(model.CommitReqModel.ReferenceNumber, model.CommitReqModel.AppointmentTypeCode);
                        //commit.BasicPay = basicpayandmedical.Item1;
                        //commit.MedicalAllowance = basicpayandmedical.Item2;
                        //commit.StartDate = basicpayandmedical.Item3;
                        //commit.CloseDate = basicpayandmedical.Item4;
                        int commitmentid = model.CommitReqModel.CommitmentId ?? 0;
                        if (commit.LogTypeId == 1 && Common.IsInUcCommitment(commitmentid))
                        {
                            TempData["errMsg"] = "You can't add any addition value to this commitment. Becouse this commitment treated as expenditure in UC.";
                            return RedirectToAction("RecruitmentCommitmentRequestList");
                        }
                        AccountService _AS = new AccountService();
                        var result = AccountService.CloseThisCommitment(commit, userid);
                        if (result == 1)
                        {
                            var comitreqid = recruitmentService.UpdateAddCommitDetails(model, commitmentid, userid);
                            if (comitreqid > 0)
                            {
                                TempData["succMsg"] = "Commitment amount added successfully.";
                                return RedirectToAction("RecruitmentCommitmentRequestList", "Requirement");
                            }
                            else
                            {
                                TempData["errMsg"] = "Commitment amount added successfully but something went wrong in data updation. Please contact administrator";
                                return RedirectToAction("RecruitmentCommitmentRequestList", "Requirement");
                            }
                        }
                        else
                        {
                            model = recruitmentService.GetRecruitBookCommitDetails(model.CommitReqModel.CommitmentRequestId);
                            psModel.Detail = pro.getProjectSummaryDetails(ProjectId);
                            psModel.Summary = pro.getProjectSummary(ProjectId);
                            psModel.Common = Common.GetProjectsDetails(ProjectId);
                            psModel.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.SancationDate);
                            psModel.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.CloseDate);
                            psModel.ProjId = ProjectId;
                            psModel.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                            psModel.ExpAmt = psModel.Summary.AmountSpent;
                            model.Projsummary = psModel;
                            TempData["errMsg"] = "Commitment amount not updated. Please try again or contact administrator.";
                            return View(model);
                        }
                    }
                    else
                    {
                        TempData["errMsg"] = "This Request already Booked are not Vaild booking Request";
                        return RedirectToAction("RecruitmentCommitmentRequestList", "Requirement");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return View(model);
                throw ex;
            }
        }

        public ActionResult RCTCloseCommitment(int Id = 0)
        {
            try
            {
                STEViewModel model = new STEViewModel();
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.Reason = Common.GetCommitmentAction();
                var Reqsttype = Common.GetRequestType(Id);
                if (Reqsttype != "Withdraw Commitment")
                {
                    TempData["errMsg"] = "Withdraw Commitment request can't be done because the request type is - " + Reqsttype;
                    return RedirectToAction("RecruitmentCommitmentRequestList");
                }
                model = recruitmentService.GetAddorWithdrawCommitDetails(Id);
                if (model.STEId == -1 && model.Status != "Awaiting Commitment Booking")
                {
                    TempData["errMsg"] = "Withdraw Commitment request already done";
                    return RedirectToAction("RecruitmentCommitmentRequestList");
                }
                ProjSummaryModel psModel = new ProjSummaryModel();
                //ProjectSummaryModel psModel = new ProjectSummaryModel();
                ProjectService pro = new ProjectService();
                var ProjectId = model.ProjectId ?? 0;
                if (ProjectId > 0)
                {
                    ViewBag.AllocationHead = Common.getAllocationHeadBasedOnProject(ProjectId);
                    psModel.Detail = pro.getProjectSummaryDetails(ProjectId);
                    psModel.Summary = pro.getProjectSummary(ProjectId);
                    psModel.Common = Common.GetProjectsDetails(ProjectId);
                    psModel.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.SancationDate);
                    psModel.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.CloseDate);
                    psModel.ProjId = ProjectId;
                    psModel.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                    psModel.ExpAmt = psModel.Summary.AmountSpent;
                    model.Projsummary = psModel;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult RCTCloseCommitment(STEViewModel model)
        {
            try
            {
                lock (lockCommitCloserequestObj)
                {
                    bool isValidRequest = Common.CheckRCTCommitmentRequest(model.CommitReqModel.CommitmentRequestId);
                    if (isValidRequest)
                    {
                        CommitmentResultModel commit = new CommitmentResultModel();
                        ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                        ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                        ViewBag.Reason = Common.GetCommitmentAction();
                        var username = User.Identity.Name;
                        int userid = Common.GetUserid(username);
                        //model = recruitmentService.GetRecruitBookCommitDetails(Id);
                        ProjSummaryModel psModel = new ProjSummaryModel();
                        //ProjectSummaryModel psModel = new ProjectSummaryModel();
                        ProjectService pro = new ProjectService();
                        var ProjectId = model.ProjectId ?? 0;
                        if (ProjectId > 0)
                        {
                            ViewBag.AllocationHead = Common.getAllocationHeadBasedOnProject(ProjectId);
                            psModel.Detail = pro.getProjectSummaryDetails(ProjectId);
                            psModel.Summary = pro.getProjectSummary(ProjectId);
                            psModel.Common = Common.GetProjectsDetails(ProjectId);
                            psModel.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.SancationDate);
                            psModel.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.CloseDate);
                            psModel.ProjId = ProjectId;
                            psModel.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                            psModel.ExpAmt = psModel.Summary.AmountSpent;
                            model.Projsummary = psModel;
                        }
                        using (var context = new IOASDBEntities())
                        {
                            commit.LogTypeId = 2;
                            var comitrquestid = model.CommitReqModel.CommitmentRequestId;
                            var commitrequest = (from prj in context.tblRCTCommitmentRequest
                                                 where prj.RecruitmentRequestId == comitrquestid
                                                 select prj).FirstOrDefault();
                            if (commitrequest != null)
                            {
                                if (commitrequest.AppointmentType.Contains("Change of Project"))
                                    commit.LogTypeId = 3;
                                else if (commitrequest.AppointmentType.Contains("Relieving"))
                                    commit.LogTypeId = 3;
                            }
                        }
                        commit.ComitmentId = model.CommitReqModel.CommitmentId ?? 0;
                        commit.strRemarks = model.CommitReqModel.Remarks;
                        commit.AddCloseAmt = model.CommitReqModel.AddCommitmentAmount ?? 0;
                        commit.ProjectId = ProjectId;
                        commit.Reason = model.CommitReqModel.Reason;
                        commit.AllHeadId = model.CommitReqModel.AllocationHeadId ?? 0;
                        //commit.selAllocationHead = model.CommitReqModel.AllocationHeadId ?? 0;
                        commit.Remarks = model.CommitReqModel.Remarks;
                        //commit.SelProjectNumber = model.ProjectId ?? 0;
                        //commit.AllocationValue = model.CommitReqModel.CommitmentAmount ?? 0;
                        //commit.selCommitmentType = 1;
                        //var basicpayandmedical = Common.getbasicpay(model.CommitReqModel.ReferenceNumber, model.CommitReqModel.AppointmentTypeCode);
                        //commit.BasicPay = basicpayandmedical.Item1;
                        //commit.MedicalAllowance = basicpayandmedical.Item2;
                        //commit.StartDate = basicpayandmedical.Item3;
                        //commit.CloseDate = basicpayandmedical.Item4;
                        int commitmentid = model.CommitReqModel.CommitmentId ?? 0;
                        if (model.CommitReqModel.LogTypeId == 1 && Common.IsInUcCommitment(commitmentid))
                        {
                            TempData["errMsg"] = "You can't add any addition value to this commitment. Becouse this commitment treated as expenditure in UC.";
                            return RedirectToAction("RecruitmentCommitmentRequestList");
                        }
                        AccountService _AS = new AccountService();
                        var result = AccountService.CloseThisCommitment(commit, userid);
                        if (result == 1)
                        {
                            var comitreqid = recruitmentService.UpdateCloseCommitDetails(model, commitmentid, userid);
                            if (comitreqid > 0)
                            {
                                TempData["succMsg"] = "Commitment amount withdrawn successfully.";
                                return RedirectToAction("RecruitmentCommitmentRequestList", "Requirement");
                            }
                            else
                            {
                                TempData["errMsg"] = "Commitment amount withdrawn successfully but something went wrong in data updation. Please contact administrator";
                                return RedirectToAction("RecruitmentCommitmentRequestList", "Requirement");
                            }
                        }
                        else
                        {
                            model = recruitmentService.GetRecruitBookCommitDetails(model.CommitReqModel.CommitmentRequestId);
                            psModel.Detail = pro.getProjectSummaryDetails(ProjectId);
                            psModel.Summary = pro.getProjectSummary(ProjectId);
                            psModel.Common = Common.GetProjectsDetails(ProjectId);
                            psModel.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.SancationDate);
                            psModel.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.CloseDate);
                            psModel.ProjId = ProjectId;
                            psModel.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                            psModel.ExpAmt = psModel.Summary.AmountSpent;
                            model.Projsummary = psModel;

                            TempData["errMsg"] = "Commitment amount not updated. Please try again or contact administrator.";
                            return View(model);
                        }
                    }
                    else
                    {
                        TempData["errMsg"] = "This Request already Booked are not Vaild booking Request.";
                        return RedirectToAction("RecruitmentCommitmentRequestList", "Requirement");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return View(model);
                throw ex;
            }
        }

        public ActionResult RCTRejectCommitRequest(int Id = 0)
        {
            try
            {
                STEViewModel model = new STEViewModel();
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.Reason = Common.GetCommitmentAction();

                model = recruitmentService.GetRejectCommitRequestDetails(Id);
                ProjSummaryModel psModel = new ProjSummaryModel();
                //ProjectSummaryModel psModel = new ProjectSummaryModel();
                ProjectService pro = new ProjectService();
                var ProjectId = model.ProjectId ?? 0;
                if (ProjectId > 0)
                {
                    ViewBag.AllocationHead = Common.getAllocationHeadBasedOnProject(ProjectId);
                    psModel.Detail = pro.getProjectSummaryDetails(ProjectId);
                    psModel.Summary = pro.getProjectSummary(ProjectId);
                    psModel.Common = Common.GetProjectsDetails(ProjectId);
                    psModel.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.SancationDate);
                    psModel.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.CloseDate);
                    psModel.ProjId = ProjectId;
                    psModel.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                    psModel.ExpAmt = psModel.Summary.AmountSpent;
                    model.Projsummary = psModel;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult RCTRejectCommitRequest(STEViewModel model)
        {
            try
            {
                lock (lockCommitrejectrequestObj)
                {
                    bool isValidRequest = Common.CheckRCTCommitmentRequest(model.CommitReqModel.CommitmentRequestId);
                    if (isValidRequest)
                    {
                        CommitmentResultModel commit = new CommitmentResultModel();
                        ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                        ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                        ViewBag.Reason = Common.GetCommitmentAction();
                        var username = User.Identity.Name;
                        int userid = Common.GetUserid(username);
                        //model = recruitmentService.GetRecruitBookCommitDetails(Id);
                        ProjSummaryModel psModel = new ProjSummaryModel();
                        //ProjectSummaryModel psModel = new ProjectSummaryModel();
                        ProjectService pro = new ProjectService();
                        var ProjectId = model.ProjectId ?? 0;
                        if (ProjectId > 0)
                        {
                            ViewBag.AllocationHead = Common.getAllocationHeadBasedOnProject(ProjectId);
                            psModel.Detail = pro.getProjectSummaryDetails(ProjectId);
                            psModel.Summary = pro.getProjectSummary(ProjectId);
                            psModel.Common = Common.GetProjectsDetails(ProjectId);
                            psModel.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.SancationDate);
                            psModel.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.CloseDate);
                            psModel.ProjId = ProjectId;
                            psModel.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                            psModel.ExpAmt = psModel.Summary.AmountSpent;
                            model.Projsummary = psModel;
                        }
                        commit.LogTypeId = 2;
                        commit.ComitmentId = model.CommitReqModel.CommitmentId ?? 0;
                        commit.strRemarks = model.CommitReqModel.Remarks;
                        commit.AddCloseAmt = model.CommitReqModel.AddCommitmentAmount ?? 0;
                        commit.ProjectId = ProjectId;
                        commit.Reason = model.CommitReqModel.Reason;
                        commit.AllHeadId = model.CommitReqModel.AllocationHeadId ?? 0;
                        //commit.selAllocationHead = model.CommitReqModel.AllocationHeadId ?? 0;
                        commit.Remarks = model.CommitReqModel.Remarks;
                        //commit.SelProjectNumber = model.ProjectId ?? 0;
                        //commit.AllocationValue = model.CommitReqModel.CommitmentAmount ?? 0;
                        //commit.selCommitmentType = 1;
                        //var basicpayandmedical = Common.getbasicpay(model.CommitReqModel.ReferenceNumber, model.CommitReqModel.AppointmentTypeCode);
                        //commit.BasicPay = basicpayandmedical.Item1;
                        //commit.MedicalAllowance = basicpayandmedical.Item2;
                        //commit.StartDate = basicpayandmedical.Item3;
                        //commit.CloseDate = basicpayandmedical.Item4;
                        int commitmentid = model.CommitReqModel.CommitmentId ?? 0;
                        if (model.CommitReqModel.LogTypeId == 1 && Common.IsInUcCommitment(commitmentid))
                        {
                            TempData["errMsg"] = "You can't add any addition value to this commitment. Becouse this commitment treated as expenditure in UC.";
                            return RedirectToAction("RecruitmentCommitmentRequestList");
                        }
                        AccountService _AS = new AccountService();
                        var result = recruitmentService.RejectCommitRequestDetails(model, userid);
                        if (result == 1)
                        {
                            TempData["succMsg"] = "Commitment Request is Rejected.";
                            return RedirectToAction("RecruitmentCommitmentRequestList", "Requirement");
                        }
                        else
                        {
                            model = recruitmentService.GetRecruitBookCommitDetails(model.CommitReqModel.CommitmentRequestId);
                            psModel.Detail = pro.getProjectSummaryDetails(ProjectId);
                            psModel.Summary = pro.getProjectSummary(ProjectId);
                            psModel.Common = Common.GetProjectsDetails(ProjectId);
                            psModel.ProjectStartDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.SancationDate);
                            psModel.ProjectCloseDate = string.Format("{0:dd-MMM-yyyy}", psModel.Common.CloseDate);
                            psModel.ProjId = ProjectId;
                            psModel.DistributionAmount = Common.GetDistribuAmount(ProjectId);
                            psModel.ExpAmt = psModel.Summary.AmountSpent;
                            model.Projsummary = psModel;

                            TempData["errMsg"] = "Rejection of Commitment Request has failed. Please try later or contact administrator";
                            return View(model);
                        }
                    }
                    else
                    {
                        TempData["errMsg"] = "This Request already Booked are not Vaild booking Request.";
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return View(model);
                throw ex;
            }
        }

        #endregion

        #region OverallapplicationList

        public ActionResult ApplicationList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetAllApplicationList(ApplicationSearchListModel model, int pageIndex, int pageSize)
        {
            try
            {
                object output = RequirementService.GetApplicationList(model, pageIndex, pageSize);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetAllExtensionEnhList(ProjectExtentionEnhmentSearchListModel model, int pageIndex, int pageSize, DateFilterModel StrFrmDate, DateFilterModel StrtoDate)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = RequirementService.GetExtandEnhment(model, pageIndex, pageSize, StrFrmDate, StrtoDate, roleid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetChangeOfProjectList(SearchChangeofProjectModel model, int pageIndex, int pageSize)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = RequirementService.GetChangeofProject(model, pageIndex, pageSize, roleid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetCancelledApplicationList(ApplicationSearchListModel model, int pageIndex, int pageSize)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = RequirementService.GetCancelApplicationList(model, pageIndex, pageSize, roleid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Note to Dean dashboard and Clarify committee Approvall
        public ActionResult NoteToDeanList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetNoteToDeanList(ApplicationSearchListModel model, int pageIndex, int pageSize)
        {
            try
            {
                object output = RequirementService.GetNoteToDeanList(model, pageIndex, pageSize);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Approval view for Dean's View and Committee View
        public ActionResult _STEApprovalView(int STEID, bool isCommitteeApproval = false)
        {
            try
            {
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                STEViewModel model = new STEViewModel();
                if (STEID > 0)
                    model = recruitmentService.GetSTEView(STEID);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                return RedirectToAction("Dashboard", "Home");
            }
        }

        public ActionResult RCTOverAllView(int ApplicationId = 0, string AppType = "", int orderid = 0, string listf = null)
        {
            try
            {
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                STEViewModel model = new STEViewModel();
                model = recruitmentService.GetOverAllApplicationViewDetails(ApplicationId, AppType, orderid);
                if (ApplicationId > 0)
                {
                    ViewBag.RefId = ApplicationId;
                    if (AppType == "STE")
                    {
                        if (model.FlowApprover == "CMAdmin")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STEAdminFlow", 0);
                        else if (model.FlowApprover == "NDean")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STEFlowDean", 0);
                        else
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STE Flow", 0);
                    }
                    else if (AppType == "OSG")
                    {
                        if (model.FlowApprover == "CMAdmin")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "OSGAdminFlow", 0);
                        else if (model.FlowApprover == "NDean")
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "OSGFlowDean", 0);
                        else
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "Outsourcing Flow", 0);
                    }
                }
                else
                {
                    ViewBag.RefId = orderid;
                    if (model.ordertype == 1)
                    {
                        if (orderid <= 518)
                        {
                            if (model.FlowApprover == "CMAdmin")
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RecruitCOPCMAdmin", 0);
                            else if (model.FlowApprover == "NDean")
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RecruitCOPDean", 0);
                            else
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RecruitCOP Flow", 0);
                        }
                        else
                        {
                            if (model.FlowApprover == "NDean")
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RCT / " + model.appType + " COP Dean Flow", 0);
                            else if (model.FlowApprover == "CMAdmin")
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RCT / " + model.appType + " COP CM Flow", 0);
                            else
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RCT / " + model.appType + " COP HR Flow", 0);
                        }
                    }
                    else if (model.ordertype == 3)
                    {
                        if (orderid <= 518)
                        {
                            if (model.FlowApprover == "CMAdmin")
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(197, "RecruitExAdmin", 0);
                            else if (model.FlowApprover == "NDean")
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(197, "RecruitExDean", 0);
                            else
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(197, "RecruitExtension Flow", 0);
                        }
                        else
                        {
                            string Type = "";
                            if (model.FlowApprover == "NDean")
                                Type = "RCT / " + model.appType + " EX Dean Flow";
                            else if (model.FlowApprover == "CMAdmin")
                                Type = "RCT / " + model.appType + " EX CM Flow";
                            else
                                Type = "RCT / " + model.appType + " EX HR Flow";
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(197, Type, 0);
                        }
                    }
                    else if (model.ordertype == 2)
                    {
                        if (orderid <= 518)
                        {
                            if (model.FlowApprover == "CMAdmin")
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(198, "RecruitEnhAdmin", 0);
                            else if (model.FlowApprover == "NDean")
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(198, "RecruitEnhDean", 0);
                            else
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(198, "RecruitEnhancement Flow", 0);
                        }
                        else
                        {
                            string Type = "";
                            if (model.FlowApprover == "NDean")
                                Type = "RCT / " + model.appType + " EN Dean Flow";
                            else if (model.FlowApprover == "CMAdmin")
                                Type = "RCT / " + model.appType + " EN CM Flow";
                            else
                                Type = "RCT / " + model.appType + " EN HR Flow";
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(198, Type, 0);
                        }
                    }
                    else if (orderid > 0 && model.ordertype == 5 || model.ordertype == 6)
                    {
                        if (orderid <= 518)
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(190, "HRA Flow", 0);
                        else
                            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(190, "RCT / STE HRA Flow", 0);
                    }
                }
                model.List_f = getEmployeeActionLink(listf);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                return RedirectToAction("Dashboard", "Home");
            }
        }

        [HttpPost]
        public JsonResult DeansApproval(int ApplicationID, string Category, bool isApprove)
        {
            try
            {
                bool res = false;
                int userId = Common.GetUserid(User.Identity.Name);
                if (ApplicationID > 0 && !string.IsNullOrEmpty(Category))
                    res = recruitmentService.DeanInitApproval(ApplicationID, Category, isApprove, userId);
                return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        //Committee Re-send
        [HttpPost]
        public JsonResult SendCommitteeApproval(int appid, string apptype, int? orderid = null, string Comments = "", HttpPostedFileBase attachement = null)
        {
            try
            {

                int userId = Common.GetUserid(User.Identity.Name);
                bool res = recruitmentService.SendForCommitteeApproval(appid, apptype, orderid, userId, Comments, attachement);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        //Govt Agencies
        [HttpPost]
        public JsonResult SenforCommitmentBooking(int appid, string apptype, int? orderid = null, string Comments = "")
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool res = recruitmentService.SendForCommitmentBooking(appid, apptype, orderid, userId, Comments);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region PopUp

        public ActionResult ViewDocuments(string employeeNo, int? appid = null, string apptype = null)
        {
            RCTViewDocumentsModel model = new RCTViewDocumentsModel();
            string user_logged_in = User.Identity.Name;
            int UserId = Common.GetUserid(user_logged_in);
            if (appid > 0 && !string.IsNullOrEmpty(apptype))
                model = Common.getDocuments(appid ?? 0, apptype);
            else
                model = Common.getDocuments(employeeNo);
            model.DeviationList = RCTEmailContentService.getDevNormsList(appid ?? 0, apptype);
            return View(model);
        }

        [HttpPost]
        public ActionResult _ViewStaffAllocation(int ProjectId)
        {
            try
            {
                ViewStaffAllocationModel model = new ViewStaffAllocationModel();
                if (ProjectId > 0)
                    model = Common.getViewStaffAllocation(ProjectId);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewStaffAllocationModel model = new ViewStaffAllocationModel();
                return View(model);
            }

        }

        [HttpPost]
        public ActionResult _ViewDeviationList(int AppId, string AppType)
        {
            try
            {
                List<CheckListEmailModel> list = new List<CheckListEmailModel>();
                list = RCTEmailContentService.getDevNormsList(AppId, AppType);
                return View(list);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                List<CheckListEmailModel> list = new List<CheckListEmailModel>();
                return View(list);
            }

        }

        [HttpPost]
        public ActionResult _ViewPopUpEmployeeDetails(string employeeId)
        {
            RCTPopupModel model = new RCTPopupModel();
            try
            {
                model = recruitmentService.getEmployeeWorkingDatails(employeeId);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult _ViewPopUpStaffDetails(string employeeId)
        {
            RCTPopupModel model = new RCTPopupModel();
            try
            {
                model = recruitmentService.getStaffWorkingDatails(employeeId);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return View(model);
            }
        }

        #endregion

        public ActionResult GenerateIDCard(int appid, string apptype)
        {
            try
            {
                string username = User.Identity.Name;
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                GenerateIDCardModel model = new GenerateIDCardModel();
                if (appid > 0 && recruitmentService.IsEmployee(appid, apptype))
                {
                    model = recruitmentService.getIDCardDetails(appid, apptype);
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator";
                    return RedirectToAction("EmployeeMaster", "Requirement");
                }
                ReportService RS = new ReportService();
                using (XLWorkbook wb = new XLWorkbook())
                {
                    DataTable dtResult = new DataTable();
                    var ws = wb.Worksheets.Add("ID Card");
                    ws.Cell(1, 1).Value = "Name";
                    ws.Cell(1, 2).Value = "Employee No";
                    ws.Cell(1, 3).Value = "Designation";
                    ws.Cell(1, 4).Value = "Department";
                    ws.Cell(1, 5).Value = "Project No";
                    ws.Cell(1, 6).Value = "Extension";
                    ws.Cell(1, 7).Value = "Staff Sign";
                    ws.Cell(1, 8).Value = "Staff Photo";
                    ws.Cell(1, 9).Value = "Present Address";
                    //ws.Cell(1, 10).Value = "Permanant Address";
                    //ws.Cell(1, 11).Value = "Email";
                    ws.Cell(1, 11).Value = "Emergency contact number";
                    var FromRange = ws.Range("A1:L1");
                    FromRange.Style.Font.Bold = true;
                    FromRange.Style.Font.FontSize = 11;
                    //FromRange.Style.Fill.BackgroundColor = XLColor.Yellow;
                    FromRange.SetAutoFilter();
                    int Firstrow = 2;
                    ws.Cell(Firstrow, 1).Value = model.Name;
                    ws.Cell(Firstrow, 2).Value = model.EmployeeId;
                    ws.Cell(Firstrow, 3).Value = model.Designation;
                    ws.Cell(Firstrow, 4).Value = model.DepartmentName;
                    ws.Cell(Firstrow, 5).Value = model.ProjectNumber;
                    ws.Cell(Firstrow, 6).Value = string.Format("{0:dd-MMMM-yyyy}", model.AppointmentEndDate);
                    ws.Cell(Firstrow, 7).Value = model.CantidateSignatureFilePath;
                    ws.Cell(Firstrow, 8).Value = model.PersonImagePath;
                    ws.Cell(Firstrow, 9).Value = model.PresentAddress;
                    //ws.Cell(Firstrow, 10).Value = model.PermanentAddress;
                    //ws.Cell(Firstrow, 11).Value = model.Email;
                    ws.Cell(Firstrow, 12).Value = model.ContactNumber;
                    ws.Range("F2").Style.DateFormat.Format = "dd-MMMM-yyyy";
                    ws.Range("A2:L" + Firstrow).Style.Font.FontSize = 11;
                    //ws.Range("A2:L" + Firstrow).Style.Font.Bold = true;
                    ws.Range("A2:L" + Firstrow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("A2:L" + Firstrow).Style.Border.InsideBorder = XLBorderStyleValues.Dotted;
                    ws.Range("A2:L" + Firstrow).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range("A2:L" + Firstrow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("A2:L" + Firstrow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("A2:L" + Firstrow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Columns("A", "BJ").AdjustToContents();
                    wb.SaveAs(workStream);
                    workStream.Position = 0;
                }
                return new FileStreamResult(workStream, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult EmployeeverificationList()
        {
            return View();
        }

        public ActionResult ExtensionEnhancementStatus()
        {
            return View();
        }

        public ActionResult DAApplicationStatus()
        {
            return View();
        }

        #region Outsourcing

        public ActionResult OutsourcingList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetOSGList(STESearchModel model, int pageIndex, int pageSize)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = RequirementService.GetOSGList(model, pageIndex, pageSize, userid, roleid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }

        public ActionResult Outsourcing(int OSGID = 0, int WFid = 0)
        {
            STEModel model = new STEModel();
            try
            {
                ViewBag.List = new List<MasterlistviewModel>();
                model.ApplicationEntryDate = DateTime.Now;
                model.Status = "";
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                ViewBag.VendorCode = Common.GetAgencyMasterList();
                if (OSGID > 0)
                {
                    model = recruitmentService.GetEditOSG(OSGID);
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "Outsourcing Flow", 0);
                }
                else if (WFid > 0 && OSGID == 0)
                {
                    model = Common.GetWFEditOSG(WFid);
                    var listcommitte = Common.GetCommittee();
                    if (listcommitte.Item1.Count > 0)
                    {
                        for (int i = 0; i < listcommitte.Item1.Count; i++)
                        {
                            if (i == 0)
                            {
                                model.CommiteeMemberId1 = listcommitte.Item1[i].id ?? 0;
                                model.CommiteeMember1 = listcommitte.Item1[i].name;
                            }
                            if (i == 1)
                            {
                                model.CommiteeMemberId2 = listcommitte.Item1[i].id ?? 0;
                                model.CommiteeMember2 = listcommitte.Item1[i].name;
                            }
                        }
                        var datacharperson = Common.GetChairPerson();
                        model.ChairpersonNameId = datacharperson.Item1;
                        model.ChairpersonName = datacharperson.Item2;
                    }
                }
                else
                {
                    if (OSGID == 0)
                    {
                        var listcommitte = Common.GetCommittee();
                        if (listcommitte.Item1.Count > 0)
                        {
                            for (int i = 0; i < listcommitte.Item1.Count; i++)
                            {
                                if (i == 0)
                                {
                                    model.CommiteeMemberId1 = listcommitte.Item1[i].id ?? 0;
                                    model.CommiteeMember1 = listcommitte.Item1[i].name;
                                }
                                if (i == 1)
                                {
                                    model.CommiteeMemberId2 = listcommitte.Item1[i].id ?? 0;
                                    model.CommiteeMember2 = listcommitte.Item1[i].name;
                                }
                            }
                        }
                        var datacharperson = Common.GetChairPerson();
                        model.ChairpersonNameId = datacharperson.Item1;
                        model.ChairpersonName = datacharperson.Item2;
                        model.isConsolidatePay = "ConsolidatedPay";
                    }
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "", 0);
                    model.ApplicationEntryDate = DateTime.Now;
                }
                model.isDraftbtn = false;
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "", 0);
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                WriteLog.SendErrorToText(ex);
                return View(model);
            }

        }

        [HttpPost]
        public ActionResult Outsourcing(STEModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                string folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement");
                string Errormessages = string.Empty;
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                ViewBag.VendorCode = Common.GetAgencyMasterList();
                #region UpdateDisiplineList	
                if (model.EducationDetail.Count > 0)
                {
                    foreach (var item in model.EducationDetail)
                        item.DisiplineList = Common.GetCourseList(item.QualificationId ?? 0);
                }
                #endregion
                //#region FileSave
                //if (model.PersonImage != null)//...........Canditates images
                //{
                //    string filename = System.IO.Path.GetFileName(model.PersonImage.FileName);
                //    var fileId = Guid.NewGuid().ToString();
                //    filename = fileId + "_" + filename;

                //    if (!Directory.Exists(folder))
                //        Directory.CreateDirectory(folder);
                //    model.PersonImage.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/" + filename));
                //    model.PersonImagePath = filename;
                //}
                //if (model.PIJustificationFile != null)//...........PI Justification documents
                //{
                //    string[] fileName = new string[model.PIJustificationFile.Count()];
                //    string[] ActialfileName = new string[model.PIJustificationFile.Count()];
                //    for (int i = 0; i < model.PIJustificationFile.Count(); i++)
                //    {
                //        if (model.PIJustificationFile[i] != null)
                //        {
                //            string filename = System.IO.Path.GetFileName(model.PIJustificationFile[i].FileName);
                //            string ActualFileName = System.IO.Path.GetFileName(model.PIJustificationFile[i].FileName);
                //            var fileId = Guid.NewGuid().ToString();
                //            filename = fileId + "_" + filename;
                //            if (!Directory.Exists(folder))
                //                Directory.CreateDirectory(folder);
                //            model.PIJustificationFile[i].SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/" + filename));
                //            fileName[i] = filename;
                //            ActialfileName[i] = ActualFileName;
                //        }
                //    }
                //    model.PIJustificationFilePath = fileName;
                //    model.PIJustificationFileName = ActialfileName;
                //}
                //if (model.EducationDetail != null)
                //{
                //    for (int i = 0; i < model.EducationDetail.Count; i++)
                //    {
                //        if (model.EducationDetail[i].Certificate != null)//...........Education Certificates
                //        {
                //            string filename = System.IO.Path.GetFileName(model.EducationDetail[i].Certificate.FileName);
                //            string ActualFileName = System.IO.Path.GetFileName(model.EducationDetail[i].Certificate.FileName);
                //            var fileId = Guid.NewGuid().ToString();
                //            filename = fileId + "_" + filename;
                //            if (!Directory.Exists(folder))
                //                Directory.CreateDirectory(folder);
                //            model.EducationDetail[i].Certificate.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/" + filename));
                //            model.EducationDetail[i].CertificatePath = filename;
                //            model.EducationDetail[i].CertificateName = ActualFileName;
                //        }
                //    }
                //}
                //if (model.ExperienceDetail != null)
                //{
                //    for (int i = 0; i < model.ExperienceDetail.Count; i++)
                //    {
                //        if (model.ExperienceDetail[i].ExperienceFile != null)//...........Experience Certificates
                //        {
                //            string filename = System.IO.Path.GetFileName(model.ExperienceDetail[i].ExperienceFile.FileName);
                //            string ActualFileName = System.IO.Path.GetFileName(model.ExperienceDetail[i].ExperienceFile.FileName);
                //            var fileId = Guid.NewGuid().ToString();
                //            filename = fileId + "_" + filename;
                //            if (!Directory.Exists(folder))
                //                Directory.CreateDirectory(folder);
                //            model.ExperienceDetail[i].ExperienceFile.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/" + filename));
                //            model.ExperienceDetail[i].ExperienceFilePath = filename;
                //            model.ExperienceDetail[i].ExperienceFileName = ActualFileName;
                //        }
                //    }
                //}
                //if (model.OtherDetail != null)
                //{
                //    for (int i = 0; i < model.OtherDetail.Count; i++)
                //    {
                //        if (model.OtherDetail[i].OtherDetailFile != null)//...........Experience Certificates
                //        {
                //            string filename = System.IO.Path.GetFileName(model.OtherDetail[i].OtherDetailFile.FileName);
                //            string ActualFileName = System.IO.Path.GetFileName(model.OtherDetail[i].OtherDetailFile.FileName);
                //            var fileId = Guid.NewGuid().ToString();
                //            filename = fileId + "_" + filename;
                //            if (!Directory.Exists(folder))
                //                Directory.CreateDirectory(folder);
                //            model.OtherDetail[i].OtherDetailFile.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/" + filename));
                //            model.OtherDetail[i].OtherDetailFilePath = filename;
                //            model.OtherDetail[i].OtherDetailFileName = ActualFileName;
                //        }
                //    }
                //}
                //#endregion

                #region FileValidation	
                if (model.PersonImage != null)
                {
                    var allowedExtensions = new[] { ".jpeg", ".png", ".jpg", ".gif" };
                    var extension = Path.GetExtension(model.PersonImage.FileName);
                    if (!allowedExtensions.Contains(extension.ToLower()))
                    {
                        TempData["alertMsg"] = "Please upload any one of these type candidate image [.jpeg,.png,.jpg]";
                        return View(model);
                    }
                    if (model.PersonImage.ContentLength > 1000000)
                    {
                        TempData["alertMsg"] = "You can upload candidate image up to 1MB";
                        return View(model);
                    }
                }
                if (model.PIJustificationFile != null)
                {
                    var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
                    for (int i = 0; i < model.PIJustificationFile.Count(); i++)
                    {
                        if (model.PIJustificationFile[i] != null)
                        {
                            string extension = Path.GetExtension(model.PIJustificationFile[i].FileName);
                            if (!allowedExtensions.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type PI judtification document [.doc,.docx,.pdf]";
                                return View(model);
                            }
                            if (model.PIJustificationFile[i].ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload PI judtification document up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }
                if (model.EducationDetail != null)
                {
                    var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                    {
                        if (model.EducationDetail[i].Certificate != null)
                        {
                            string extension = Path.GetExtension(model.EducationDetail[i].Certificate.FileName);
                            if (!allowedExtensionsCer.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type certificate [.jpeg,.png,.jpg,.gif,.pdf]";
                                return View(model);
                            }
                            if (model.EducationDetail[i].Certificate.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload certificate up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }
                if (model.ExperienceDetail != null)
                {
                    var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                    for (int i = 0; i < model.ExperienceDetail.Count; i++)
                    {
                        if (model.ExperienceDetail[i].ExperienceFile != null)//...........Experience Certificates	
                        {
                            string filename = System.IO.Path.GetFileName(model.ExperienceDetail[i].ExperienceFile.FileName);
                            string extension = Path.GetExtension(filename);
                            if (!allowedExtensionsCer.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type certificate [.jpeg,.png,.jpg,.gif,.pdf]";
                                return View(model);
                            }
                            if (model.ExperienceDetail[i].ExperienceFile.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload certificate up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }
                if (model.OtherDetail != null)
                {
                    var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                    for (int i = 0; i < model.OtherDetail.Count; i++)
                    {
                        if (model.OtherDetail[i].OtherDetailFile != null)//...........Experience Certificates	
                        {
                            string filename = System.IO.Path.GetFileName(model.OtherDetail[i].OtherDetailFile.FileName);
                            string extension = Path.GetExtension(filename);
                            if (!allowedExtensionsCer.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type other document [.jpeg,.png,.jpg,.gif,.pdf]";
                                return View(model);
                            }
                            if (model.OtherDetail[i].OtherDetailFile.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload other documents up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }
                #endregion
                if (model.STEId > 0)
                {
                    if (model.aadharnumber != null)
                    {
                        var chkemployeeadhar = Common.CheckPreviousEmployeeAdharserver(Convert.ToString(model.aadharnumber), model.ApplicationNo, true, model.OldEmployeeNumber, "OSG");
                        if (chkemployeeadhar != "")
                        {
                            TempData["alertMsg"] = chkemployeeadhar;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                    if (!string.IsNullOrEmpty(model.PAN))
                    {
                        var chkemployeepanno = Common.CheckPreviousEmployeePanserver(model.PAN, model.ApplicationNo, true, model.OldEmployeeNumber, "OSG");
                        if (chkemployeepanno != "")
                        {
                            TempData["alertMsg"] = chkemployeepanno;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                }
                else
                {
                    if (model.aadharnumber != null)
                    {
                        var chkemployeeadhar = Common.CheckPreviousEmployeeAdharserver(Convert.ToString(model.aadharnumber), null, true, model.OldEmployeeNumber, "OSG");
                        if (chkemployeeadhar != "")
                        {
                            TempData["errMsg"] = chkemployeeadhar;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                    if (!string.IsNullOrEmpty(model.PAN))
                    {
                        var chkemployeepanno = Common.CheckPreviousEmployeePanserver(model.PAN, null, true, model.OldEmployeeNumber, "OSG");
                        if (chkemployeepanno != "")
                        {
                            TempData["errMsg"] =chkemployeepanno;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                }
                if (!model.isDraftbtn)//..........Submit Button
                {
                    ModelState.Remove("CSIRStaff");
                    if (ModelState.IsValid)
                    {
                        if (string.IsNullOrEmpty(Errormessages))
                        {
                            string validationMsg = ValidateOSGFormData(model);
                            if (validationMsg != "Valid")
                            {
                                TempData["errMsg"] = validationMsg;
                                model.Status = model.Status == null ? "" : model.Status;
                                return View(model);
                            }
                            var result = recruitmentService.PostOSG(model, userId);
                            if (result.Item1 == 1)
                            {
                                TempData["succMsg"] = "Outsourcing Application submitted";
                                return RedirectToAction("OutsourcingList", "Requirement");
                            }
                            else if (result.Item1 == 2)
                            {
                                TempData["succMsg"] = "Outsourcing application submitted for PI justification";
                                return RedirectToAction("OutsourcingList", "Requirement");
                            }
                            else if (result.Item1 == 3)
                            {
                                TempData["errMsg"] = result.Item3;
                                return RedirectToAction("OutsourcingList", "Requirement");
                            }
                            else
                            {
                                TempData["errMsg"] = "Something went wrong please contact administrator";
                                return RedirectToAction("OutsourcingList", "Requirement");
                            }
                        }
                        else
                        {
                            string messages = string.Join("<br />", ModelState.Values
                                                .SelectMany(x => x.Errors)
                                                .Select(x => x.ErrorMessage));
                            messages += Errormessages;
                            TempData["errMsg"] = messages;
                        }
                    }
                    else
                    {
                        string messages = string.Join("<br />", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                        TempData["errMsg"] = messages;
                    }
                }
                else //..........Is-Draft button
                {
                    if (string.IsNullOrEmpty(Errormessages))
                    {
                        var result = recruitmentService.PostOSG(model, userId);
                        if (result.Item1 == 1)
                        {
                            TempData["succMsg"] = "Draft Saved Successfully";
                            return RedirectToAction("OutsourcingList", "Requirement");
                        }
                        else if (result.Item1 == 2)
                        {
                            TempData["succMsg"] = "Draft updated";
                            return RedirectToAction("OutsourcingList", "Requirement");
                        }
                        else
                        {
                            TempData["errMsg"] = "Something went wrong please contact administrator";
                            //return RedirectToAction("OutsourcingList", "Requirement");
                            return View(model);
                        }
                    }
                    else
                    {
                        string messages = string.Join("<br />", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));
                        messages += Errormessages;
                        TempData["errMsg"] = messages;
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Apptype = Common.GetCodeControlList("STEAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Years = Common.getRequirementyear();
                #region UpdateDisiplineList
                if (model.EducationDetail.Count > 0)
                {
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                    {
                        model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                    }
                }
                #endregion
                return View(model);
            }
        }

        private string ValidateOSGFormData(STEModel model)
        {
            string msg = "Valid";
            using (var context = new IOASDBEntities())
            {
                var queryosg = (from s in context.tblRCTOutsourcing
                                where s.OSGID == model.STEId && (s.Status == "Open" || s.Status == "Draft" || s.Status == "Note to PI")
                                select s).FirstOrDefault();
                if (model.EmployeeType.Contains("Old"))
                {
                    if (!string.IsNullOrEmpty(model.OldEmployeeNumber))
                    {
                        var query = (from s in context.tblRCTOutsourcing
                                     where s.EmployeersID == model.OldEmployeeNumber
                                     && s.Status == "Relieved"
                                     orderby s.OSGID descending
                                     select s).FirstOrDefault();
                        if (query != null)
                        {
                            if (query.AppointmentEnddate >= model.Appointmentstartdate)
                                msg = msg == "Valid" ? "Appointment date cannot be less than relieving date." : msg + "<br /> Appointment date cannot be less than relieving date.";
                        }
                        else
                            msg = msg == "Valid" ? "Please enter valid old employee number." : msg + "<br /> Please enter valid old employee number.";
                    }
                    else
                        msg = msg == "Valid" ? "Please enter old employee number." : msg + "<br /> Please enter old employee number.";
                }

                string aadhar = Convert.ToString(model.aadharnumber);
                var vwQuery = (from vw in context.vw_RCTOverAllApplicationEntry.AsNoTracking()
                               where ((string.IsNullOrEmpty(aadhar) || vw.AadhaarNo.Contains(aadhar))
                               && (string.IsNullOrEmpty(model.PAN) || vw.PANNo.Contains(model.PAN))) && vw.ApplicationType == "New"
                               && vw.Status == "Relieved"
                               orderby vw.ApplicationEntryDate descending
                               select vw).FirstOrDefault();
                if (vwQuery != null)
                {
                    DateTime? relievedate = vwQuery.RelieveDate == null ? vwQuery.AppointmentEnddate : vwQuery.RelieveDate;
                    if (relievedate >= model.Appointmentstartdate)
                        msg = msg == "Valid" ? "Appointment date cannot be less than relieving date (" + string.Format("{0:dd-MMMM-yyyy}", relievedate) + " - " + vwQuery.EmployeersID + ")." : msg + "<br /> Appointment date cannot be less than relieving date (" + string.Format("{0:dd-MMMM-yyyy}", relievedate) + " - " + vwQuery.EmployeersID + ").";
                }

                if (model.DesignationId != null)
                {
                    var querydes = context.tblRCTDesignation.FirstOrDefault(M => M.DesignationId == model.DesignationId && M.RecordStatus == "Active");
                    if (querydes != null)
                    {
                        if (querydes.Medical != true)
                        {
                            if (model.Medical != 3 || model.MedicalAmmount > 0)
                                msg = msg == "Valid" ? "Entered designation does not have medical contribution. please contact administrator." : msg + "<br /> Entered designation does not have medical contribution. please contact administrator.";
                        }
                        else
                        {
                            decimal Deduction = querydes.MedicalDeduction ?? 0;
                            if (model.MedicalAmmount > 0 && model.MedicalAmmount != Deduction)//Medical amount should be same as designation master medical value
                                msg = msg == "Valid" ? "Medical contribution data mis match. please contact administrator." : msg + "<br /> Medical contribution data mis match. please contact administrator.";
                        }

                        if (querydes.HRA != true)
                        {
                            if (model.HRA > 0)
                                msg = msg == "Valid" ? "Entered designation does not have HRA provision. Please contact administrator." : msg + "<br />Entered designation does not have HRA provision. Please contact administrator.";
                        }
                        else
                        {
                            var HRAPercentage = querydes.HRABasic / 100;
                            var HRAValue = model.Salary * HRAPercentage;
                            if (model.HRA > HRAValue)
                                msg = msg == "Valid" ? "HRA entered exceeds institution norms." : msg + "<br /> HRA entered exceeds institution norms.";
                        }
                        if (querydes.SalaryLevel > 0)
                        {
                            if (querydes.SalaryLevel != model.SalaryLevelId)
                                msg = msg == "Valid" ? "Entered designation does not have salary level. Please contact administrator." : msg + "<br /> Entered designation does not have salary level. Please contact administrator.";
                        }
                    }
                }
                else
                    msg = msg == "Valid" ? "Enter valid designation code." : msg + "<br /> Enter valid designation code.";

                if (model.ProjectId != null)
                {
                    ProjectService _PS = new ProjectService();
                    var _projectDetail = RequirementService.getProjectSummary(model.ProjectId ?? 0);
                    if (_projectDetail != null)
                    {
                        if (model.Appointmentstartdate != null && model.AppointmentEndDate != null)
                        {
                            double DiffMonth = model.AppointmentEndDate.Value.Subtract(model.Appointmentstartdate.Value).Days + 1;
                            if (model.Appointmentstartdate.Value.Year == model.AppointmentEndDate.Value.Year && model.Appointmentstartdate.Value.Month == model.AppointmentEndDate.Value.Month && model.Appointmentstartdate.Value.Month == 2)
                            {
                                if (DiffMonth >= 28)
                                    DiffMonth = 30;
                            }

                            if (DiffMonth <= 29)
                                msg = msg == "Valid" ? "Appointment tenure should be minimum 1 month to 1 year." : msg + "<br /> Appointment tenure should be minimum 1 month to 1 year.";

                            var Days = model.AppointmentEndDate.Value.Subtract(model.Appointmentstartdate.Value).Days + 1;
                            decimal totdays = Common.GetAvgDaysInAYear((DateTime)model.Appointmentstartdate, (DateTime)model.AppointmentEndDate);
                            var Years = Convert.ToDecimal(Days) / totdays;
                            if (model.DesignationId == 1)
                            {
                                decimal permonth = Convert.ToDecimal(365) / Convert.ToDecimal(12);
                                var monthDiff = Convert.ToDecimal(Days) / permonth;
                                if (monthDiff > 6)
                                    msg = msg == "Valid" ? "Entered designation appointment tenure should be minimum 1 month to 6 months." : msg + "<br />Entered designation appointment tenure should be minimum 1 month to 6 months.";
                            }
                            else
                            {
                                if (Years > 1)
                                    msg = msg == "Valid" ? "Appointment tenure should be minimum 1 month to 1 year." : msg + "<br />Appointment tenure should be minimum 1 month to 1 year.";
                            }

                            DateTime StartDate = DateTime.Parse(_projectDetail.ProjectStartDate);
                            DateTime CloseDate = DateTime.Parse(_projectDetail.ProjectClosureDate);


                            if (StartDate > model.Appointmentstartdate || model.AppointmentEndDate > CloseDate)
                                msg = msg == "Valid" ? "Appointment tenure must be between the project start date and project closure date." : msg + "<br /> Appointment tenure must be between the project start date and project closure date.";

                            var now = DateTime.Now.Date.AddDays(-15);
                            if (now >= StartDate && now <= CloseDate)
                            {
                                if (queryosg != null && queryosg.AppointmentStartdate < now)
                                    now = queryosg.AppointmentStartdate ?? now;
                            }
                            else
                            {
                                now = StartDate;
                            }

                            if (now > model.Appointmentstartdate || model.AppointmentEndDate > CloseDate)
                                msg = msg == "Valid" ? "Appointment tenure must be between the criteria." : msg + "<br /> Appointment tenure must be between the criteria.";

                        }
                        else
                            msg = msg == "Valid" ? "Enter appointment tenure." : msg + "<br /> Enter appointment tenure.";
                    }
                }
                else
                    msg = msg == "Valid" ? "Enter valid project number." : msg + "<br /> Enter valid project number.";


                if (model.ProfessionalId != null)
                {
                    if (model.ProfessionalId == 4)
                    {
                        if (model.EducationDetail != null)
                        {
                            if (!model.EducationDetail.Select(x => x.QualificationId).ToArray().Contains(3))
                                msg = msg == "Valid" ? "Please select Doctorate degree for Salutation Dr." : msg + "<br />Please select Doctorate degree for Salutation Dr.";
                        }
                    }
                }
            }
            return msg;
        }

        public ActionResult OSGModifyProfile(int OSGID)
        {
            STEModel model = new STEModel();
            try
            {
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                ViewBag.List = new List<MasterlistviewModel>();
                if (OSGID > 0)
                    model = recruitmentService.GetEditOSG(OSGID);
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(model.DateofBirth.Value.Year);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.List = new List<MasterlistviewModel>();
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(model.DateofBirth.Value.Year);
                if (model.EducationDetail.Count > 0)
                {
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                        model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                }
                return View(model);
            }

        }

        [HttpPost]
        public ActionResult OSGModifyProfile(STEModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                string Errormessages = "";
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                ViewBag.List = new List<MasterlistviewModel>();
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(model.DateofBirth.Value.Year);
                if (model.aadharnumber != null)
                {
                    var chkemployeeadhar = Common.CheckPreviousEmployeeAdharserver(Convert.ToString(model.aadharnumber), model.ApplicationNo);
                    if (chkemployeeadhar != "")
                    {
                        TempData["alertMsg"] = chkemployeeadhar;
                        model.Status = model.Status == null ? "" : model.Status;
                        if (model.EducationDetail.Count > 0)
                        {
                            for (int i = 0; i < model.EducationDetail.Count; i++)
                                model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                        }
                        return View(model);
                    }
                }
                if (!string.IsNullOrEmpty(model.PAN))
                {
                    var chkemployeepanno = Common.CheckPreviousEmployeePanserver(model.PAN, model.ApplicationNo);
                    if (chkemployeepanno != "")
                    {
                        TempData["alertMsg"] = "This Pan Number is linked to  " + chkemployeepanno;
                        model.Status = model.Status == null ? "" : model.Status;
                        if (model.EducationDetail.Count > 0)
                        {
                            for (int i = 0; i < model.EducationDetail.Count; i++)
                                model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                        }
                        return View(model);
                    }
                }

                if (string.IsNullOrEmpty(Errormessages))
                {
                    var result = recruitmentService.OSGModifyProfile(model, userId);
                    if (result == 1)
                    {
                        TempData["succMsg"] = "Modified Successfully";
                        return RedirectToAction("OSGEmployeeMaster", "Requirement");
                    }
                    else if (result == -1)
                    {
                        TempData["succMsg"] = "Record not found";
                        return RedirectToAction("OSGEmployeeMaster", "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("OSGEmployeeMaster", "Requirement");
                    }
                }
                else
                {
                    TempData["errMsg"] = Errormessages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(model.DateofBirth.Value.Year);
                if (model.EducationDetail.Count > 0)
                {
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                    {
                        model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                    }
                }
                return View(model);
            }
        }

        public ActionResult OSGViewProfile(int OSGID, string listf = null)
        {
            try
            {
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                STEViewModel model = new STEViewModel();
                if (OSGID > 0)
                    model = recruitmentService.GetOSGView(OSGID);
                model.List_f = getEmployeeActionLink("OSG", listf);
                if (model.FlowApprover == "CMAdmin")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "OSGAdminFlow", 0);
                else if (model.FlowApprover == "NDean")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "OSGFlowDean", 0);
                else
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "Outsourcing Flow", 0);
                return View(model);
            }
            catch (Exception ex)
            {
                STEViewModel model = new STEViewModel();
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                #region UpdateDisiplineList
                if (model.EducationDetail.Count > 0)
                {
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                    {
                        model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                    }
                }
                #endregion
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult LoadVendorList(string term)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteSlyAgencyList(term);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult OSGEmailProcess(CheckDevationModel model)
        {
            EmailBuilder _eb = new EmailBuilder();
            NotePIModel ackmodel = new NotePIModel();
            using (var context = new IOASDBEntities())
            {
                var designation = context.tblRCTDesignation.Where(m => m.DesignationId == model.DesignationId).Select(m => m.Designation).FirstOrDefault();
                ackmodel.DesignationName = designation;
                ackmodel.PersonName = model.PersonName;
                ackmodel.ApplicationReceiveDate = String.Format("{0:ddd dd-MMMM-yyyy}", model.AppointmentReciveDate);
                ackmodel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", model.AppointmentStartDate);
                ackmodel.AppointmentEndDate = String.Format("{0:dd-MMMM-yyyy}", model.AppointmentEndDate);
                ackmodel.AppointmentType = model.AppointmentType;
                ackmodel.ProjectNumber = Common.getprojectnumber(model.ProjectID ?? 0);
                ackmodel.BasicPay = Convert.ToString(model.BasicPay ?? 0);
                ackmodel.IsDeviation = false;
                ackmodel.SendSlryStruct = model.SendSalaryStructure;
                var user = Common.getUserIdAndRole(User.Identity.Name);
                ackmodel.DAName = Common.GetUserNameBasedonId(user.Item1);
            }

            var loadEmialView = _eb.RunCompile("RCTOSGApplicationack.cshtml", "", ackmodel, typeof(NotePIModel));
            var result = new { output = loadEmialView };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OutsourcingView(int OSGID, string listf = null)
        {
            try
            {
                STEViewModel model = new STEViewModel();
                if (OSGID > 0)
                    model = recruitmentService.GetOSGView(OSGID);
                if (model.FlowApprover == "CMAdmin")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "OSGAdminFlow", 0);
                else if (model.FlowApprover == "NDean")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "OSGFlowDean", 0);
                else
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "Outsourcing Flow", 0);
                model.List_f = getEmployeeActionLink("OSG", listf);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return RedirectToAction("Dashboard", "Home");
            }
        }

        #region OSG Verification
        public ActionResult OSGVerificationList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetOSGVerificationList(int pageIndex, int pageSize, SearchSTEVerificationModel model, DateFilterModel OfferDate, DateFilterModel strAnnouncementClosureDate, DateFilterModel DateOfJoining)
        {
            try
            {
                object output = recruitmentService.GetOSGVerificationList(pageIndex, pageSize, model, OfferDate, strAnnouncementClosureDate, DateOfJoining);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }

        public ActionResult OSGVerification(int OSGID)
        {
            STEVerificationModel model = new STEVerificationModel();
            try
            {
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");

                if (OSGID > 0)
                {
                    if (validateOSGVerification(OSGID))
                    {
                        model = recruitmentService.GetOSGVerification(OSGID);
                        if (model.Status == "Awaiting Verification-Open")
                        {
                            var user = Common.getUserIdAndRole(User.Identity.Name);
                            model.RoleId = user.Item2;
                            if (model.FlowApprover == "CMAdmin")
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(206, "OSGVERAdminFlow", 0);
                            else if (model.FlowApprover == "NDean")
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(206, "OSGVERFlowDean", 0);
                            else
                                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(206, "OSGVER Flow", 0);
                            model.List_f = getEmployeeActionLink("STE", "OVAL");
                            ViewBag.currentRefId = model.STEId;
                        }

                    }
                    else
                    {
                        TempData["alertMsg"] = "Date of joining should be only within the tenure.";
                        return RedirectToAction("OSGVerificationList", "Requirement");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult OSGVerification(STEVerificationModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                string Errormessages = "";
                var button = Request["button"];
                if (button.Contains("Save as drafts"))
                    button = button == null ? "" : button.Split(',')[1];
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(Convert.ToInt32(model.DateofBirth.Split('-')[2]));
                if (!validateOSGVerification(model.STEId ?? 0))
                {
                    TempData["alertMsg"] = "Date of joining should be only within the tenure.";
                    return RedirectToAction("OSGVerificationList", "Requirement");
                }

                if (model.aadharnumber != null)
                {
                    var chkemployeeadhar = Common.CheckPreviousEmployeeAdharserver(Convert.ToString(model.aadharnumber), model.ApplicationNo, true, model.OldEmployeeNumber, "OSG");
                    if (chkemployeeadhar != "")
                    {
                        TempData["alertMsg"] = chkemployeeadhar;
                        model.Status = model.Status == null ? "" : model.Status;
                        return View(model);
                    }
                }
                if (!string.IsNullOrEmpty(model.PAN))
                {
                    var chkemployeepanno = Common.CheckPreviousEmployeePanserver(model.PAN, model.ApplicationNo, true, model.OldEmployeeNumber, "OSG");
                    if (chkemployeepanno != "")
                    {
                        TempData["alertMsg"] = "This Pan Number is linked to  " + chkemployeepanno;
                        model.Status = model.Status == null ? "" : model.Status;
                        return View(model);
                    }
                }

                if (model.STEId > 0 && button == "Save as drafts")
                {
                    if (string.IsNullOrEmpty(Errormessages))
                    {

                        var result = recruitmentService.VerifyOSG(model, userId, button);
                        if (result.Item1 == 1)
                        {
                            TempData["succMsg"] = "Draft successfully";
                            return RedirectToAction("OSGVerificationList", "Requirement");
                        }
                        else
                        {
                            TempData["errMsg"] = "Something went wrong please contact administrator";
                            return RedirectToAction("OSGVerificationList", "Requirement");
                        }
                    }
                    else
                    {
                        string messages = string.Join("<br />", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));
                        #region UpdateDisiplineList
                        if (model.EducationDetail.Count > 0)
                        {
                            for (int i = 0; i < model.EducationDetail.Count; i++)
                            {
                                model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                            }
                        }
                        #endregion
                        TempData["errMsg"] = messages;
                    }
                }
                else if (model.STEId > 0)
                {
                    if (string.IsNullOrEmpty(Errormessages))
                    {

                        var result = recruitmentService.VerifyOSG(model, userId, button);
                        if (result.Item1 == 1)
                        {
                            TempData["succMsg"] = "Application verified";
                            return RedirectToAction("OSGVerificationList", "Requirement");
                        }
                        else if (result.Item1 == 2)
                        {
                            TempData["succMsg"] = "Already verified";
                            return RedirectToAction("OSGVerificationList", "Requirement");
                        }
                        else if (result.Item1 == -1)
                        {
                            TempData["errMsg"] = "Record not found";
                            return RedirectToAction("OSGVerificationList", "Requirement");
                        }
                        else
                        {
                            TempData["errMsg"] = "Something went wrong please contact administrator";
                            return RedirectToAction("OSGVerificationList", "Requirement");
                        }
                    }
                    else
                    {
                        string messages = string.Join("<br />", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));
                        #region UpdateDisiplineList
                        if (model.EducationDetail.Count > 0)
                        {
                            for (int i = 0; i < model.EducationDetail.Count; i++)
                            {
                                model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                            }
                        }
                        #endregion
                        TempData["errMsg"] = messages;
                    }
                }
                else
                {
                    if (model.DateofBirth != null)
                        ViewBag.Years = Common.getRequirementyear(Convert.ToInt32(model.DateofBirth.Split('-')[2]));
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                    messages += Errormessages;
                    TempData["errMsg"] = messages;
                    #region UpdateDisiplineList
                    if (model.EducationDetail.Count > 0)
                    {
                        for (int i = 0; i < model.EducationDetail.Count; i++)
                        {
                            model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                        }
                    }
                    #endregion
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                #region UpdateDisiplineList
                if (model.EducationDetail.Count > 0)
                {
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                    {
                        model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                    }
                }
                #endregion
                return View(model);
            }
        }

        public bool validateOSGVerification(int OSGID)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var curr = DateTime.Now.Date;
                    return context.tblRCTOutsourcing.Any(m => m.OSGID == OSGID && m.AppointmentStartdate <= curr && m.AppointmentEnddate >= curr);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ActionResult OSGOfferLetter(int OSGID, bool SaveFile = false)
        {
            try
            {
                string ViewName = "";
                STEViewModel model = new STEViewModel();
                if (OSGID > 0)
                {
                    model = recruitmentService.getOSGOfferletterDetails(OSGID, 0);
                    if (model.MsPhd)
                    {
                        ViewName = "_STEMsPhdOfferLetter";
                    }
                    else if (model.TypeofappointmentID == 1 || model.TypeofappointmentID == 2)
                    {
                        ViewName = "_STEFullOrPartTimeOfferLetter";
                    }
                    else if (model.DesignationId == 1)
                    {
                        ViewName = "_STETraineeOfferLetter";
                    }
                    else if (model.DesignationId == 20)
                    {
                        ViewName = "_STEAdvisorOfferLetter";
                    }
                    else
                    {
                        ViewName = "_STEOfferLetter";
                    }
                }
                var yourpdf = new PartialViewAsPdf(ViewName, model)
                {
                    RotativaOptions = new DriverOptions()
                    {
                        PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                        PageSize = Rotativa.Core.Options.Size.A4,
                        PageMargins = new Margins(10, 10, 10, 10)
                    }

                };
                return yourpdf;
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                STEViewModel model = new STEViewModel();
                return PartialView("_STEOfferLetter", model);
            }
        }
        #endregion

        public ActionResult OSGSendForApproval(int OSGID)
        {
            try
            {
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                STEViewModel model = new STEViewModel();
                model = recruitmentService.GetOSGView(OSGID);
                return View(model);
            }
            catch (Exception ex)
            {
                STEViewModel model = new STEViewModel();
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                #region UpdateDisiplineList
                if (model.EducationDetail.Count > 0)
                {
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                    {
                        model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                    }
                }
                #endregion
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult SendOSGForApproval(STEViewModel model)
        {
            try
            {
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                //STEViewModel model = new STEViewModel();
                int userId = Common.GetUserid(User.Identity.Name);
                int OSGID = model.STEId;
                var result = recruitmentService.OSGApprove(model, userId);
                if (result.Item1 == false && result.Item2 != null)
                {
                    return RedirectToAction("OSGSendForApproval", "Requirement", new { OSGID = OSGID });
                }
                else if (result.Item1 == true && (result.Item2 == null || result.Item2 == ""))
                {
                    TempData["succMsg"] = "Application submitted for approval";
                    return RedirectToAction("OutsourcingList", "Requirement");
                }
                else if (result.Item2 != null)
                {
                    TempData["errMsg"] = result.Item2;
                    return RedirectToAction("OutsourcingList", "Requirement");
                }
                //model.VerifyProfile = verfiy;
                //if (model.FlowApprover == "CMAdmin")
                //{
                //    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "OSGAdminFlow", 0);
                //}
                //else if (model.FlowApprover == "NDean")
                //{
                //    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "OSGFlowDean", 0);
                //}
                //else
                //{
                //    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "Outsourcing Flow", 0);
                //}
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                #region UpdateDisiplineList
                if (model.EducationDetail.Count > 0)
                {
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                    {
                        model.EducationDetail[i].DisiplineList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                    }
                }
                #endregion
                return View(model);
            }
        }

        #region OrderApproval
        public ActionResult RecruitCOPSendForApproval(int AppId, string listf = null)
        {
            OrderModel model = new OrderModel();
            try
            {
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                if (AppId > 0)
                {
                    model = recruitmentService.getOrderDetails(AppId);
                    model.listf = listf;
                    if (model.FlowApprover == "CMAdmin")
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RecruitCOPCMAdmin", 0);
                    else if (model.FlowApprover == "NDean")
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RecruitCOPDean", 0);
                    else
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "RecruitCOP Flow", 0);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(195, "", 0);
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult RecruitCOPSendForApproval(OrderModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                var allowedExtensionsDoc = new[] { ".doc", ".docx", ".pdf" };
                if (model.ApplicationID > 0)
                {
                    if (model.PIJustificationFile != null)
                    {
                        foreach (var item in model.PIJustificationFile)
                        {
                            if (item != null)//...........Justification Document
                            {
                                string filename = System.IO.Path.GetFileName(item.FileName);
                                string extension = Path.GetExtension(filename);
                                if (!allowedExtensionsDoc.Contains(extension.ToLower()))
                                {
                                    TempData["alertMsg"] = "Please upload any one of these type doc [.doc,.docx,.pdf]";
                                    return View(model);
                                }
                                if (item.ContentLength > 5242880)
                                {
                                    TempData["alertMsg"] = "You can upload PI judtification document up to 5MB";
                                    return View(model);
                                }
                            }
                        }
                    }

                    //if (model.PILetter != null)
                    //{
                    //    string filename = System.IO.Path.GetFileName(model.PILetter.FileName);
                    //    string extension = Path.GetExtension(filename);
                    //    if (!allowedExtensionsDoc.Contains(extension.ToLower()))
                    //    {
                    //        TempData["alertMsg"] = "Please upload any one of these type of pi letter document [.doc,.docx,.pdf]";
                    //        return View(model);
                    //    }
                    //    if (model.PILetter.ContentLength > 5242880)
                    //    {
                    //        TempData["alertMsg"] = "You can upload pi letter document up to 5MB";
                    //        return View(model);
                    //    }
                    //}
                    //else
                    //{
                    //    if (model.OrderID == 0)
                    //    {
                    //        TempData["alertMsg"] = "Please upload pi request document";
                    //        return View(model);
                    //    }
                    //}

                    //var validationMsg = ValidateCOPFormData(model);
                    //if (validationMsg != "Valid")
                    //{
                    //    if (model.OrderID > 0)
                    //        model = recruitmentService.getOrderDetails(model.OrderID);
                    //    else
                    //        model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 1);
                    //    TempData["alertMsg"] = validationMsg;
                    //    return View(model);
                    //}

                    var result = recruitmentService.OSGCOPApprove(model, userId);
                    if (result.Item1 == true && (result.Item2 == null || result.Item2 == ""))
                    {
                        TempData["succMsg"] = "Request sent for Approval";
                        return RedirectToAction("OSGDAApplicationStatus", "Requirement");
                    }
                    else if (result.Item1 == true && result.Item2 != null)
                    {
                        TempData["errMsg"] = result.Item2;
                        return RedirectToAction("OSGDAApplicationStatus", "Requirement");
                    }
                    else if (result.Item1 == false && result.Item2 != null)
                    {
                        TempData["errMsg"] = result.Item2;
                        return RedirectToAction("OSGDAApplicationStatus", "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("OSGDAApplicationStatus", "Requirement");
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
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                if (model.OrderID > 0)
                    model = recruitmentService.getOrderDetails(model.OrderID);
                else
                    model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 1);
                TempData["errMsg"] = "Something went wrong please contact administrator";
                return View(model);
            }
        }

        public ActionResult RecruitExtensionSendForApproval(int AppId, bool Adminf = false, bool daf = false)
        {
            OrderModel model = new OrderModel();
            try
            {
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                if (AppId > 0)
                {
                    model = recruitmentService.getOrderDetails(AppId);
                    ViewBag.AppointmentType = Common.AppointmentType(model.TypeCode);
                    if (model.FlowApprover == "CMAdmin")
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(197, "RecruitExAdmin", 0);
                    else if (model.FlowApprover == "NDean")
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(197, "RecruitExDean", 0);
                    else
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(197, "RecruitExtension Flow", 0);
                }
                model.Adminintif = Adminf;
                model.isDADashboard = daf;
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(196, "", 0);
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult RecruitExtensionSendForApproval(OrderModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.AppointmentType = Common.AppointmentType(model.TypeCode);
                var allowedJustificationDoc = new[] { ".doc", ".docx", ".pdf" };
                if (model.ApplicationID > 0)
                {
                    if (model.PIJustificationFile != null)
                    {
                        foreach (var item in model.PIJustificationFile)
                        {
                            if (item != null)//...........Justification Document
                            {
                                string filename = System.IO.Path.GetFileName(item.FileName);
                                string extension = Path.GetExtension(filename);
                                if (!allowedJustificationDoc.Contains(extension.ToLower()))
                                {
                                    if (model.OrderID > 0)
                                        model = recruitmentService.getOrderDetails(model.OrderID);
                                    else
                                        model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 3);
                                    TempData["alertMsg"] = "Please upload any one of these type doc [.doc,.docx,.pdf]";
                                    return View(model);
                                }
                                if (item.ContentLength > 5242880)
                                {
                                    if (model.OrderID > 0)
                                        model = recruitmentService.getOrderDetails(model.OrderID);
                                    else
                                        model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 3);
                                    TempData["alertMsg"] = "You can upload PI judtification document up to 5MB";
                                    return View(model);
                                }
                            }
                        }
                    }
                    //else
                    //{
                    //    if (model.OrderID == 0)
                    //    {
                    //        model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 3);
                    //        TempData["alertMsg"] = "Please upload pi request document";
                    //        return View(model);
                    //    }
                    //}

                    var result = recruitmentService.OSGExtensionApprove(model, userId);
                    if (result.Item1 == true && (result.Item2 == null || result.Item2 == ""))
                        TempData["succMsg"] = "Request sent for Approval";
                    else if (result.Item1 == true && result.Item2 != null)
                        TempData["errMsg"] = result.Item2;
                    else if (result.Item1 == false && result.Item2 != null)
                        TempData["errMsg"] = result.Item2;
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                    return RedirectToAction("OSGDAApplicationStatus", "Requirement");
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
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.AppointmentType = Common.AppointmentType("");
                TempData["errMsg"] = "Something went wrong please contact administrator";
                if (model.OrderID > 0)
                    model = recruitmentService.getOrderDetails(model.OrderID);
                else
                    model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 3);
                return View(model);
            }
        }

        public ActionResult RecruitEnhancementSendForApproval(int AppId)
        {
            OrderModel model = new OrderModel();
            try
            {
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                if (AppId > 0)
                {
                    model = recruitmentService.getOrderDetails(AppId);
                    ViewBag.AppointmentType = Common.AppointmentType(model.TypeCode);
                    if (model.FlowApprover == "CMAdmin")
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(198, "RecruitEnhAdmin", 0);
                    else if (model.FlowApprover == "NDean")
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(198, "RecruitEnhDean", 0);
                    else
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(198, "RecruitEnhancement Flow", 0);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(197, "", 0);
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult RecruitEnhancementSendForApproval(OrderModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.Apptype = Common.GetCodeControlList("OSGAppointmenttype");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.AppointmentType = Common.AppointmentType(model.TypeCode);
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");

                var allowedDoc = new[] { ".doc", ".docx", ".pdf" };
                if (model.ApplicationID > 0)
                {
                    //var validationMsg = ValidateEnhancementFormData(model);
                    //if (validationMsg != "Valid")
                    //{
                    //    if (model.OrderID > 0)
                    //        model = recruitmentService.getOrderDetails(model.OrderID);
                    //    else
                    //        model = recruitmentService.getOrderProjectDetails(model.ApplicationID, model.TypeCode, 2);
                    //    TempData["errMsg"] = validationMsg;
                    //    return View(model);
                    //}

                    #region FileValidation                    
                    if (model.PIJustificationFile != null)
                    {
                        foreach (var item in model.PIJustificationFile)
                        {
                            if (item != null)//...........Justification Document
                            {
                                string filename = System.IO.Path.GetFileName(item.FileName);
                                string extension = Path.GetExtension(filename);
                                if (!allowedDoc.Contains(extension.ToLower()))
                                {
                                    TempData["errMsg"] = "Please upload any one of these type doc [.doc,.docx,.pdf]";
                                    return View(model);
                                }
                            }
                        }
                    }

                    #endregion
                    var result = recruitmentService.OSGEnhancementApprove(model, userId);
                    if (result.Item1 == true && (result.Item2 == null || result.Item2 == ""))
                        TempData["succMsg"] = "Request sent for Approval";
                    else if (result.Item1 == true && result.Item2 != null)
                        TempData["errMsg"] = result.Item2;
                    else if (result.Item1 == false && result.Item2 != null)
                        TempData["errMsg"] = result.Item2;
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator";

                    return RedirectToAction("OSGDAApplicationStatus", "Requirement");
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
                WriteLog.SendErrorToText(ex);
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.OrderType = Common.GetCodeControlList("OrderType");
                ViewBag.ReferenceType = Common.ReferenceType();
                ViewBag.AppointmentType = Common.AppointmentType("");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                return View(model);
            }
        }
        #endregion

        #endregion

        #region Load old employee and aadhar, pan validation
        [HttpGet]
        public JsonResult LoadRCTEmployeeList(string term, string apptype = null)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteRCTEmployee(term, apptype);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadRCTOSGEmployeeList(string term, string apptype = null)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteRCTOSGEmployee(term, apptype);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadRCTSTEEmployeeList(string term, string apptype = null)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteRCTSTEEmployee(term, "STE");
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadRCTApplicationNumberList(string term, string apptype = null)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteApplicationNumber(term, apptype);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadRCTSTEApplicationNumberList(string term, string apptype = null)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteSTEApplicationNumber(term, apptype);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetOldEmployeeDetails(string EmpNo = null, string AppRefNo = null)
        {
            try
            {
                object output = Common.GetEmployeeDetails(EmpNo, AppRefNo);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult CheckPreviousEmployeeAdhar(string adharno, string oldId = null, string apptype = null, string appref = null)
        {
            object output = Common.CheckPreviousEmployeeAdharserver(adharno, appref, true, oldId, apptype);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckPreviousEmployeePan(string Panno, string oldId = null, string apptype = null, string appref = null)
        {
            object output = Common.CheckPreviousEmployeePanserver(Panno, appref, true, oldId, apptype);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckConsultantEmployeePan(string Panno, string GST, string EmpID)
        {
            object output = Common.CheckConsultantEmployeePan(Panno, GST, EmpID);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        
        #endregion

        #region VerficationUserApplicationCancel

        public ActionResult ApplicationCancel()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetCancelledApplication(SearchVerificationList model, int pageIndex, int pageSize)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = RequirementService.GetCancelList(model, pageIndex, pageSize, userid, roleid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region OtherPaymentDeduction

        public ActionResult OtherPaymentDeductionList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetOtherPaymentList(SearchOtherPaymentModel model, int pageIndex, int pageSize, DateFilterModel CreatedDate)
        {
            try
            {
                object output = RequirementService.GetOtherPaymentList(model, pageIndex, pageSize, CreatedDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }

        public ActionResult OtherPaymentDeduction(int OthId = 0)
        {
            OtherPaymentDeductionModel model = new OtherPaymentDeductionModel();
            try
            {
                ViewBag.OtherType = Common.GetCodeControlList("OtherType");
                ViewBag.MonthList = Common.GetmonthList();
                ViewBag.YearList = Common.GetYearOthList();
                ViewBag.List = new List<MasterlistviewModel>();
                if (OthId > 0)
                {
                    model = RequirementService.EditPaymentdeduction(OthId);
                    FinOp fac = new FinOp(System.DateTime.Now);
                    model.FrmDateEdit = fac.GetMonthFirstDate(model.Month);
                    model.ToDateEdit = fac.GetMonthLastDate(model.Month);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.OtherType = Common.GetCodeControlList("OtherType");
                ViewBag.PaymentType = Common.GetCodeControlList("RCTPayment");
                ViewBag.Deduction = Common.GetCodeControlList("RCTDeduction");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult OtherPaymentDeduction(OtherPaymentDeductionModel model)
        {
            ViewBag.OtherType = Common.GetCodeControlList("OtherType");
            //ViewBag.PaymentType = Common.GetCodeControlList("RCTPayment");
            //ViewBag.Deduction = Common.GetCodeControlList("RCTDeduction");
            ViewBag.MonthList = Common.GetmonthList();
            ViewBag.YearList = Common.GetYearOthList();
            ViewBag.List = new List<MasterlistviewModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.OTHDetail.Count > 0)
                    {
                        foreach (var item in model.OTHDetail)
                        {
                            item.PaydecList = Common.GetCommonHeadList(1, item.OtherType ?? 0);
                        }
                    }
                    if (model.OTHAttachement != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".PDF", ".DOCX", ".docx" };
                        string filename = Path.GetFileName(model.OTHAttachement.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["alertMsg"] = "Please upload any one of these type doc [.pdf,.docx]";
                            return View(model);
                        }
                        if (model.OTHAttachement.ContentLength > 5242880)
                        {
                            TempData["alertMsg"] = "You can upload File up to 5 MB";
                            return View(model);
                        }

                    }
                    model.UserId = Common.GetUserid(User.Identity.Name);
                    var status = recruitmentService.CreatePaymentdeduction(model);
                    if (model.OTHPayDeductionId == null && status.Item1 == 1)
                    {
                        TempData["succMsg"] = "Add Sucessfully";
                        return RedirectToAction("OtherPaymentDeductionList", "Requirement");
                    }
                    else if (model.OTHPayDeductionId > 0 && status.Item1 == 2)
                    {
                        TempData["succMsg"] = "Update Sucessfully";
                        return RedirectToAction("OtherPaymentDeductionList", "Requirement");
                    }
                    else if ((model.OTHPayDeductionId == null || model.OTHPayDeductionId != null) && status.Item1 == 3)
                    {
                        TempData["alertMsg"] = status.Item3;
                        return RedirectToAction("OtherPaymentDeductionList", "Requirement");
                    }
                    else if ((model.OTHPayDeductionId == null || model.OTHPayDeductionId != null) && status.Item1 == 4)
                    {
                        TempData["alertMsg"] = status.Item3;
                        return RedirectToAction("OtherPaymentDeductionList", "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("OtherPaymentDeductionList", "Requirement");
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                #region Updatepaydeduction
                if (model.OTHDetail.Count > 0)
                {
                    foreach (var item in model.OTHDetail)
                    {

                        item.PaydecList = Common.GetCommonHeadList(1, item.OtherType ?? 0);
                    }
                }
                #endregion
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.OtherType = Common.GetCodeControlList("OtherType");

                TempData["errMsg"] = "Something went wrong please contact administrator";
                #region Updatepaydeduction
                if (model.OTHDetail.Count > 0)
                {
                    foreach (var item in model.OTHDetail)
                    {
                        item.PaydecList = Common.GetCommonHeadList(1, item.OtherType ?? 0);
                    }
                }
                #endregion
                return View(model);
            }
        }

        public ActionResult OtherPaymentDeductionView(int othId)
        {
            OtherPaymentDeductionModel model = new OtherPaymentDeductionModel();
            try
            {
                model = RequirementService.ViewOtherPaymentDeduction(othId);
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(212, "OTHPaydeu", 0);
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadEmployeeDetails(string EMPNo)
        {
            var Employeedetails = RequirementService.GetEmployeeDetails(EMPNo);
            var result = new { EMPData = Employeedetails };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadOTHDeclist(int othtypeId)
        {
            var locationdata = RequirementService.GetOthPaydeectionList(othtypeId);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadOTHMonthDate(string MonthAndYear)
        {
            DateTime Frmdate = DateTime.Now;
            DateTime todate = DateTime.Now;
            FinOp fac = new FinOp(System.DateTime.Now);
            Frmdate = fac.GetMonthFirstDate(MonthAndYear);
            todate = fac.GetMonthLastDate(MonthAndYear);
            var result = new { FrmSalDate = Frmdate, toSalDate = todate };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Consultant Appointment

        public ActionResult ConsultantAppointment(int consultantAppId = 0)
        {
            ConsultantAppointmentModel model = new ConsultantAppointmentModel();
            try
            {
                model.ApplicatonEntryDate = DateTime.Now;
                ViewBag.Apptype = Common.GetCodeControlList("Appointmenttype");
                ViewBag.paytype = Common.GetCodeControlList("RCTPayType");
                ViewBag.professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.gender = Common.GetCodeControlList("RCTGender");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.PensionerorCSIR = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                ViewBag.GSTapplicable = Common.GetCodeControlList("GSTApplicable");
                model.TypeofappointmentId = 1;
                model.PaytypeId = 1;
                if (consultantAppId > 0)
                {
                    model = RequirementService.EditConsultantAppointment(consultantAppId);
                }
                model.isDraftbtn = false;
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }

        }

        [HttpPost]
        public ActionResult ConsultantAppointment(ConsultantAppointmentModel model)
        {

            try
            {
                string user_logged_in = User.Identity.Name;
                ViewBag.Apptype = Common.GetCodeControlList("Appointmenttype");
                ViewBag.paytype = Common.GetCodeControlList("RCTPayType");
                ViewBag.professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.gender = Common.GetCodeControlList("RCTGender");
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.PensionerorCSIR = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                ViewBag.GSTapplicable = Common.GetCodeControlList("GSTApplicable");
                #region  FileDocument validation
                if (model.PersonDocImage != null)
                {
                    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".PNG", ".JPG", ".JPEG" };
                    string extpersonImgename = Path.GetFileName(model.PersonDocImage.FileName);
                    var imageextension = Path.GetExtension(extpersonImgename);
                    if (!allowedExtensions.Contains(imageextension))
                    {
                        TempData["alertMsg"] = "Please upload any one of these type image [png, jpg, jpeg]";
                        List<EducationModel> list = new List<EducationModel>();
                        for (int i = 0; i < model.EducationDetail.Count; i++)
                        {
                            int ddid = model.EducationDetail[i].EducationId ?? 0;
                            int qulid = model.EducationDetail[i].QualificationId ?? 0;
                            List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                            datalist = Common.GetCourseList(qulid);
                            list.Add(new EducationModel()
                            {
                                QualificationId = model.EducationDetail[i].QualificationId,
                                ddlList = datalist,

                            });
                        }
                        model.EducationDetail = list;
                        return View(model);
                    }
                    if (model.PersonDocImage.ContentLength > 1000000)
                    {
                        TempData["alertMsg"] = "You can upload image up to 1 MB";
                        List<EducationModel> list = new List<EducationModel>();
                        for (int i = 0; i < model.EducationDetail.Count; i++)
                        {
                            int ddid = model.EducationDetail[i].EducationId ?? 0;
                            int qulid = model.EducationDetail[i].QualificationId ?? 0;
                            List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                            datalist = Common.GetCourseList(qulid);
                            list.Add(new EducationModel()
                            {
                                QualificationId = model.EducationDetail[i].QualificationId,
                                ddlList = datalist,

                            });
                        }
                        model.EducationDetail = list;
                        return View(model);
                    }
                }
                if (model.Resume != null)
                {
                    var extension = Path.GetExtension(model.Resume.FileName);
                    if (extension.ToLower() != ".pdf")
                    {
                        TempData["alertMsg"] = "Please upload any one of these type Resume [.pdf]";
                        List<EducationModel> list = new List<EducationModel>();
                        for (int i = 0; i < model.EducationDetail.Count; i++)
                        {
                            int ddid = model.EducationDetail[i].EducationId ?? 0;
                            int qulid = model.EducationDetail[i].QualificationId ?? 0;
                            List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                            datalist = Common.GetCourseList(qulid);
                            list.Add(new EducationModel()
                            {
                                QualificationId = model.EducationDetail[i].QualificationId,
                                ddlList = datalist,

                            });
                        }
                        model.EducationDetail = list;
                        return View(model);
                    }
                    if (model.Resume.ContentLength > 5242880)
                    {
                        TempData["alertMsg"] = "You can upload Resume up to 5MB";
                        List<EducationModel> list = new List<EducationModel>();
                        for (int i = 0; i < model.EducationDetail.Count; i++)
                        {
                            int ddid = model.EducationDetail[i].EducationId ?? 0;
                            int qulid = model.EducationDetail[i].QualificationId ?? 0;
                            List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                            datalist = Common.GetCourseList(qulid);
                            list.Add(new EducationModel()
                            {
                                QualificationId = model.EducationDetail[i].QualificationId,
                                ddlList = datalist,

                            });
                        }
                        model.EducationDetail = list;
                        return View(model);
                    }
                }
                if (model.FormDocument != null)
                {
                    var extension = Path.GetExtension(model.FormDocument.FileName);
                    if (extension.ToLower() != ".pdf")
                    {
                        TempData["alertMsg"] = "Please upload any one of these type Form [.pdf]";
                        List<EducationModel> list = new List<EducationModel>();
                        for (int i = 0; i < model.EducationDetail.Count; i++)
                        {
                            int ddid = model.EducationDetail[i].EducationId ?? 0;
                            int qulid = model.EducationDetail[i].QualificationId ?? 0;
                            List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                            datalist = Common.GetCourseList(qulid);
                            list.Add(new EducationModel()
                            {
                                QualificationId = model.EducationDetail[i].QualificationId,
                                ddlList = datalist,

                            });
                        }
                        model.EducationDetail = list;
                        return View(model);
                    }
                    if (model.FormDocument.ContentLength > 5242880)
                    {
                        TempData["alertMsg"] = "You can upload Form up to 5MB";
                        List<EducationModel> list = new List<EducationModel>();
                        for (int i = 0; i < model.EducationDetail.Count; i++)
                        {
                            int ddid = model.EducationDetail[i].EducationId ?? 0;
                            int qulid = model.EducationDetail[i].QualificationId ?? 0;
                            List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                            datalist = Common.GetCourseList(qulid);
                            list.Add(new EducationModel()
                            {
                                QualificationId = model.EducationDetail[i].QualificationId,
                                ddlList = datalist,

                            });
                        }
                        model.EducationDetail = list;
                        return View(model);
                    }
                }
                foreach (var item in model.EducationDetail)
                {
                    if (item.Certificate != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".PDF", ".gif", ".PNG", ".GIF", ".JPG", ".JPEG", ".png", ".jpg", ".jpeg" };
                        string filename = Path.GetFileName(item.Certificate.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["alertMsg"] = "Please upload any one of these type doc [.pdf,.gif,.png,.jpg,.jpeg]";
                            List<EducationModel> list = new List<EducationModel>();
                            for (int i = 0; i < model.EducationDetail.Count; i++)
                            {
                                int ddid = model.EducationDetail[i].EducationId ?? 0;
                                int qulid = model.EducationDetail[i].QualificationId ?? 0;
                                List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                                datalist = Common.GetCourseList(qulid);
                                list.Add(new EducationModel()
                                {
                                    QualificationId = model.EducationDetail[i].QualificationId,
                                    ddlList = datalist,

                                });
                            }
                            model.EducationDetail = list;
                            return View(model);
                        }
                        if (item.Certificate.ContentLength > 5242880)
                        {
                            TempData["alertMsg"] = "You can upload File up to 5 MB";
                            List<EducationModel> list = new List<EducationModel>();
                            for (int i = 0; i < model.EducationDetail.Count; i++)
                            {
                                int ddid = model.EducationDetail[i].EducationId ?? 0;
                                int qulid = model.EducationDetail[i].QualificationId ?? 0;
                                List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                                datalist = Common.GetCourseList(qulid);
                                list.Add(new EducationModel()
                                {
                                    QualificationId = model.EducationDetail[i].QualificationId,
                                    ddlList = datalist,

                                });
                            }
                            model.EducationDetail = list;
                            return View(model);
                        }

                    }
                }
                foreach (var item in model.ExperienceDetail)
                {
                    if (item.ExperienceFile != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".PDF", ".gif", ".PNG", ".GIF", ".JPG", ".JPEG", ".png", ".jpg", ".jpeg" };
                        string filename = Path.GetFileName(item.ExperienceFile.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["alertMsg"] = "Please upload any one of these type doc [.pdf,.gif,.png,.jpg,.jpeg]";
                            List<EducationModel> list = new List<EducationModel>();
                            for (int i = 0; i < model.EducationDetail.Count; i++)
                            {
                                int ddid = model.EducationDetail[i].EducationId ?? 0;
                                int qulid = model.EducationDetail[i].QualificationId ?? 0;
                                List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                                datalist = Common.GetCourseList(qulid);
                                list.Add(new EducationModel()
                                {
                                    QualificationId = model.EducationDetail[i].QualificationId,
                                    ddlList = datalist,

                                });
                            }
                            model.EducationDetail = list;
                            return View(model);
                        }
                        if (item.ExperienceFile.ContentLength > 5242880)
                        {
                            TempData["alertMsg"] = "You can upload File up to 5 MB";
                            List<EducationModel> list = new List<EducationModel>();
                            for (int i = 0; i < model.EducationDetail.Count; i++)
                            {
                                int ddid = model.EducationDetail[i].EducationId ?? 0;
                                int qulid = model.EducationDetail[i].QualificationId ?? 0;
                                List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                                datalist = Common.GetCourseList(qulid);
                                list.Add(new EducationModel()
                                {
                                    QualificationId = model.EducationDetail[i].QualificationId,
                                    ddlList = datalist,

                                });
                            }
                            model.EducationDetail = list;
                            return View(model);
                        }
                    }
                }
                if (model.PIJustificationFile[0] != null)
                {
                    for (int i = 0; i < model.PIJustificationFile.Length; i++)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(model.PIJustificationFile[i].FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["alertMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            List<EducationModel> list = new List<EducationModel>();
                            for (int j = 0; j < model.EducationDetail.Count; i++)
                            {
                                int ddid = model.EducationDetail[j].EducationId ?? 0;
                                int qulid = model.EducationDetail[j].QualificationId ?? 0;
                                List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                                datalist = Common.GetCourseList(qulid);
                                list.Add(new EducationModel()
                                {
                                    QualificationId = model.EducationDetail[j].QualificationId,
                                    ddlList = datalist,

                                });
                            }
                            model.EducationDetail = list;
                            return View(model);
                        }
                        if (model.PIJustificationFile[i].ContentLength > 5242880)
                        {
                            TempData["alertMsg"] = "You can upload File up to 5 MB";
                            List<EducationModel> list = new List<EducationModel>();
                            for (int j = 0; j < model.EducationDetail.Count; i++)
                            {
                                int ddid = model.EducationDetail[j].EducationId ?? 0;
                                int qulid = model.EducationDetail[j].QualificationId ?? 0;
                                List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                                datalist = Common.GetCourseList(qulid);
                                list.Add(new EducationModel()
                                {
                                    QualificationId = model.EducationDetail[j].QualificationId,
                                    ddlList = datalist,

                                });
                            }
                            model.EducationDetail = list;
                            return View(model);
                        }
                    }
                }
                #endregion

                if (model.isDraftbtn != true)
                {
                    if (ModelState.IsValid)
                    {

                        model.UserId = Common.GetUserid(user_logged_in);
                        var result = recruitmentService.CreateConsultantAppointment(model);
                        if (result.Item1 == 1)
                        {
                            TempData["succMsg"] = "Application submitted for approval";
                            return RedirectToAction("ConsultantAppointmentList");
                        }
                        else if (result.Item1 == 2)
                        {

                            TempData["succMsg"] = "Consultant Appointment submitted for PI justification";
                            return RedirectToAction("ConsultantAppointmentList");
                        }
                        else if (result.Item1 == 3)
                        {
                            TempData["alertMsg"] = "No Approval Flow found.";
                            return RedirectToAction("ConsultantAppointmentList");
                        }
                        else
                        {
                            TempData["errMsg"] = "Something went wrong please contact administrator";
                            return RedirectToAction("ConsultantAppointmentList");
                        }
                    }
                    else
                    {
                        string messages = string.Join("<br />", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                        TempData["errMsg"] = messages;
                        List<EducationModel> list = new List<EducationModel>();
                        for (int i = 0; i < model.EducationDetail.Count; i++)
                        {
                            int ddid = model.EducationDetail[i].EducationId ?? 0;
                            int qulid = model.EducationDetail[i].QualificationId ?? 0;
                            List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                            datalist = Common.GetCourseList(qulid);
                            list.Add(new EducationModel()
                            {
                                QualificationId = model.EducationDetail[i].QualificationId,
                                ddlList = datalist,

                            });
                        }
                        model.EducationDetail = list;
                    }
                }
                else
                {

                    model.UserId = Common.GetUserid(user_logged_in);
                    var result = recruitmentService.CreateConsultantAppointment(model);
                    if (result.Item1 == 1)
                    {

                        TempData["succMsg"] = "Consultant Appointment has been added Draft successfully.";
                        return RedirectToAction("ConsultantAppointmentList");
                    }
                    else if (result.Item1 == 2)
                    {

                        TempData["succMsg"] = "Consultant Appointment Draft Updated successfully.";
                        return RedirectToAction("ConsultantAppointmentList");
                    }
                    else if (result.Item1 == 3)
                    {
                        TempData["alertMsg"] = "No Approval Flow found.";
                        return RedirectToAction("ConsultantAppointmentList");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went to Wrong Please Contact Administrator";
                        List<EducationModel> list = new List<EducationModel>();
                        for (int i = 0; i < model.EducationDetail.Count; i++)
                        {
                            int ddid = model.EducationDetail[i].EducationId ?? 0;
                            int qulid = model.EducationDetail[i].QualificationId ?? 0;
                            List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                            datalist = Common.GetCourseList(qulid);
                            list.Add(new EducationModel()
                            {
                                QualificationId = model.EducationDetail[i].QualificationId,
                                ddlList = datalist,

                            });
                        }
                        model.EducationDetail = list;
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Apptype = Common.GetCodeControlList("Appointmenttype");
                ViewBag.paytype = Common.GetCodeControlList("RCTPayType");
                ViewBag.professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.gender = Common.GetCodeControlList("RCTGender");
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.PensionerorCSIR = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                List<EducationModel> list = new List<EducationModel>();
                for (int i = 0; i < model.EducationDetail.Count; i++)
                {
                    int ddid = model.EducationDetail[i].EducationId ?? 0;
                    int qulid = model.EducationDetail[i].QualificationId ?? 0;
                    List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                    datalist = Common.GetCourseList(qulid);
                    list.Add(new EducationModel()
                    {
                        QualificationId = model.EducationDetail[i].QualificationId,
                        ddlList = datalist,

                    });
                }
                model.EducationDetail = list;
                ViewBag.GSTapplicable = Common.GetCodeControlList("GSTApplicable");
                return View(model);
            }

        }

        public ActionResult ConsultantView(int consultantAppId, string listf = null)
        {
            ConsultantAppointmentModel model = new ConsultantAppointmentModel();
            try
            {
                model = recruitmentService.ViewConsultantAppointment(consultantAppId);
                if (model.FlowApprover == "CMAdmin")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(195, "Consultant Appointment CM Admin", 0);
                else if (model.FlowApprover == "NDean")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(195, "Consultant Appointment Dean", 0);
                else
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(195, "Consultant Appointment HR Admin", 0);
                model.List_f = getEmployeeActionLink("CON", listf);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return RedirectToAction("Dashboard", "Home");
            }
        }

        public ActionResult ConsultantAppointmentList()
        {
            return View();
        }

        #region Consultant Modify Profile 
        public ActionResult CONModifyProfile(int ConsultantAppId)
        {
            ConsultantAppointmentModel model = new ConsultantAppointmentModel();
            try
            {
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");

                if (ConsultantAppId > 0)
                    model = RequirementService.EditConsultantAppointment(ConsultantAppId);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                List<EducationModel> list = new List<EducationModel>();
                for (int i = 0; i < model.EducationDetail.Count; i++)
                {
                    int ddid = model.EducationDetail[i].EducationId ?? 0;
                    int qulid = model.EducationDetail[i].QualificationId ?? 0;
                    List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                    datalist = Common.GetCourseList(qulid);
                    list.Add(new EducationModel()
                    {
                        QualificationId = model.EducationDetail[i].QualificationId,
                        ddlList = datalist,

                    });
                }
                model.EducationDetail = list;
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult CONModifyProfile(ConsultantAppointmentModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                var folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement");
                string Errormessages = "";


                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                var allowedExtensionsPhoto = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                string[] validFileTypes = { ".pdf", ".PDF", ".jpg", ".jpeg", ".gif" };
                if (model.ConsultantAppointmentId > 0)
                {
                    #region FileSave
                    if (model.PersonDocImage != null)
                    {
                        var extensionPhoto = Path.GetExtension(model.PersonDocImage.FileName.ToLower());
                        if (!allowedExtensionsPhoto.Contains(extensionPhoto))
                        {
                            Errormessages = "Please upload any one of these type photo [.jpg,.jpeg,.png,.pdf,.PDF]";
                        }
                    }
                    if (model.JoiningReport != null)
                    {
                        var extensionPhoto = Path.GetExtension(model.JoiningReport.FileName.ToLower());
                        if (!validFileTypes.Contains(extensionPhoto))
                        {
                            Errormessages = "Please upload any one of these type photo [.jpg,.jpeg,.png]";
                        }
                    }
                    if (model.CantidateSignature != null)
                    {
                        var extensionPhoto = Path.GetExtension(model.CantidateSignature.FileName.ToLower());
                        if (!validFileTypes.Contains(extensionPhoto))
                        {
                            Errormessages = "Please upload any one of these type documents [.pdf,.jpg,.jpeg,gif]";
                        }
                    }
                    if (model.EducationDetail.Count > 0)
                    {
                        for (int i = 0; i < model.EducationDetail.Count; i++)
                        {
                            if (model.EducationDetail[i].Certificate != null)
                            {
                                var extension = Path.GetExtension(model.EducationDetail[i].Certificate.FileName.ToLower());

                                if (!validFileTypes.Contains(extension))
                                {
                                    Errormessages = "Please upload any one of these type documents [.pdf,.jpg,.jpeg,gif]";
                                }
                            }
                        }
                    }
                    if (model.ExperienceDetail.Count > 0)
                    {
                        for (int i = 0; i < model.ExperienceDetail.Count; i++)
                        {
                            if (model.ExperienceDetail[i].ExperienceFile != null)
                            {
                                var extension = Path.GetExtension(model.ExperienceDetail[i].ExperienceFile.FileName.ToLower());

                                if (!validFileTypes.Contains(extension))
                                {
                                    Errormessages = "Please upload any one of these type documents [.pdf,.jpg,.jpeg,gif]";
                                }
                            }
                        }
                    }
                    #endregion
                    if (string.IsNullOrEmpty(Errormessages))
                    {
                        var result = recruitmentService.ModifyConsultant(model);
                        if (result.Item1 == 1)
                        {
                            TempData["succMsg"] = "Modified Successfully";
                            return RedirectToAction("CONEmployeeMaster", "Requirement");
                        }
                        else
                        {
                            TempData["errMsg"] = "Something went wrong please contact administrator";
                            return RedirectToAction("CONEmployeeMaster", "Requirement");
                        }
                    }
                    else
                    {
                        string messages = string.Join("<br />", ModelState.Values
                                          .SelectMany(x => x.Errors)
                                          .Select(x => x.ErrorMessage));
                        messages += Errormessages;
                        TempData["errMsg"] = messages;
                        List<EducationModel> list = new List<EducationModel>();
                        for (int i = 0; i < model.EducationDetail.Count; i++)
                        {
                            int ddid = model.EducationDetail[i].EducationId ?? 0;
                            int qulid = model.EducationDetail[i].QualificationId ?? 0;
                            List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                            datalist = Common.GetCourseList(qulid);
                            list.Add(new EducationModel()
                            {
                                QualificationId = model.EducationDetail[i].QualificationId,
                                ddlList = datalist,

                            });
                        }
                        model.EducationDetail = list;
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.professional = Common.GetCodeControlList("RCTProfessional");
                List<EducationModel> list = new List<EducationModel>();
                for (int i = 0; i < model.EducationDetail.Count; i++)
                {
                    int ddid = model.EducationDetail[i].EducationId ?? 0;
                    int qulid = model.EducationDetail[i].QualificationId ?? 0;
                    List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                    datalist = Common.GetCourseList(qulid);
                    list.Add(new EducationModel()
                    {
                        QualificationId = model.EducationDetail[i].QualificationId,
                        ddlList = datalist,

                    });
                }
                model.EducationDetail = list;
                return View(model);
            }
        }
        public ActionResult CONViewProfile(int ConsultantAppId, string listf = null)
        {
            ConsultantAppointmentModel model = new ConsultantAppointmentModel();
            try
            {
                if (ConsultantAppId > 0)
                {
                    model = recruitmentService.ViewConsultantAppointment(ConsultantAppId);
                    model.List_f = getEmployeeActionLink("CON", listf);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }
        #endregion

        //[HttpPost]
        //public JsonResult GetConsultantList(ConsultantSearchModel model, int pageIndex, int pageSize)
        //{
        //    try
        //    {
        //        object output = RequirementService.GetConsultantList(model, pageIndex, pageSize);
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpPost]
        public JsonResult CONEmailProcess(CheckDevationModel model)
        {
            EmailBuilder _eb = new EmailBuilder();
            var context = new IOASDBEntities();
            int designationID = model.DesignationId ?? 0;
            var designation = context.tblRCTDesignation.FirstOrDefault(m => m.DesignationId == designationID).Designation;
            NotePIModel npmodel = new NotePIModel();
            npmodel.AppointmentStartDate = String.Format("{0:dd-MMMM-yyyy}", model.AppointmentStartDate);
            npmodel.AppointmentEndDate = String.Format("{0:dd-MMMM-yyyy}", model.AppointmentEndDate);
            npmodel.PersonName = model.PersonName;
            npmodel.ProjectNumber = Common.getprojectnumber(model.ProjectID ?? 0);
            var user = Common.getUserIdAndRole(User.Identity.Name);
            npmodel.DAName = Common.GetUserFirstName(user.Item1);
            npmodel.DesignationName = designation;
            npmodel.BasicPay = Convert.ToString(model.BasicPay ?? 0);
            context.Dispose();
            var loadEmialView = _eb.RunCompile("RCTCONAckTemplate.cshtml", "", npmodel, typeof(NotePIModel));
            var result = new { output = loadEmialView };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult CONHRNote(int CONAPId, string Note)
        //{
        //    string user_logged_in = User.Identity.Name;
        //    int UserId = Common.GetUserid(user_logged_in);
        //    int output = RequirementService.UpdateCONHRNote(CONAPId, UserId, Note);
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}

        #region Consultant Verification

        public bool validateCONVerification(int CONID)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var curr = DateTime.Now.Date;
                    return context.tblRCTConsultantAppointment.Any(m => m.ConsultantAppointmentId == CONID && m.AppointmentStartdate <= curr && m.AppointmentEnddate >= curr);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [HttpPost]
        public JsonResult GetCONVerficationList(int pageIndex, int pageSize, SearchCONVerificationModel model, DateFilterModel OfferDate, DateFilterModel strAnnouncementClosureDate, DateFilterModel DateOfJoining)
        {
            try
            {
                object output = recruitmentService.GetCONVerificationList(pageIndex, pageSize, model, OfferDate, strAnnouncementClosureDate, DateOfJoining);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }


        [HttpPost]
        public JsonResult SendCONVerificationRemindermail(int ConAppId)
        {
            string user_logged_in = User.Identity.Name;
            int UserId = Common.GetUserid(user_logged_in);
            object output = RequirementService.SendEmailCONRemindermailForVerification(ConAppId, UserId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CONVerificationList()
        {
            return View();
        }
        public ActionResult CONVerification(int ConAppId)
        {
            ConsultantAppointmentModel model = new ConsultantAppointmentModel();
            try
            {
                ViewBag.List = new List<MasterlistviewModel>();
                model.ApplicatonEntryDate = DateTime.Now;
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                if (ConAppId > 0 && validateCONVerification(ConAppId))
                {
                    model = recruitmentService.GetCONVerification(ConAppId);
                }
                else
                {
                    TempData["alertMsg"] = "Date of joining should be only within the tenure.";
                    return RedirectToAction("STEVerficationList", "Requirement");
                }
                if (model.DateBrith != null)
                    ViewBag.Years = Common.getRequirementyear(Convert.ToInt32(model.DateBrith.Split('-')[2]));
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.List = new List<MasterlistviewModel>();
                model.ApplicatonEntryDate = DateTime.Now;
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                return View(model);
            }
        }

        public string GenerateCONOfferLetter(int ConAppId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var folder = "";
                    folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/CONOfferLetter");
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement/CONOfferLetter"));
                    foreach (string sFile in System.IO.Directory.GetFiles(folder))
                        System.IO.File.Delete(sFile);

                    //string loginuser = User.Identity.Name;
                    string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/ReportMaster/_RCTCONOfferletter?ConAppId=" + ConAppId + "&SaveFile=" + true;
                    string pdf_page_size = "A4";
                    SelectPdf.PdfPageSize pageSize =
                        (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);
                    string pdf_orientation = "Portrait";
                    SelectPdf.PdfPageOrientation pdfOrientation =
                        (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                        pdf_orientation, true);
                    int webPageWidth = 1024;
                    int webPageHeight = 0;
                    SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                    converter.Options.PdfPageSize = pageSize;
                    converter.Options.PdfPageOrientation = pdfOrientation;
                    converter.Options.WebPageWidth = webPageWidth;
                    converter.Options.WebPageHeight = webPageHeight;
                    SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                    var docName = "OfferLetter_" + ConAppId + ".pdf";
                    string Path = folder + '\\' + docName;
                    doc.Save(@Path);
                    // close pdf document
                    doc.Close();

                    return Path;
                }
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return "";
            }
        }
        [HttpPost]
        public ActionResult CONVerification(ConsultantAppointmentModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                var folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement");
                string Errormessages = "";
                ViewBag.List = new List<MasterlistviewModel>();
                model.ApplicatonEntryDate = DateTime.Now;
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                var allowedExtensionsPhoto = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                string[] validFileTypes = { ".pdf", ".PDF", ".jpg", ".jpeg", ".gif", ".png" };
                if (model.ConsultantAppointmentId > 0)
                {
                    if (model.AadharNumber != null)
                    {
                        var chkemployeeadhar = Common.CheckPreviousEmployeeAdharserver(Convert.ToString(model.AadharNumber), model.ApplicationNumber, true, model.OldEmployeeNumber, "CON");
                        if (chkemployeeadhar != "")
                        {
                            TempData["alertMsg"] = chkemployeeadhar;
                            model.Status = model.Status == null ? "" : model.Status;
                            if (model.EducationDetail.Count > 0)
                            {
                                for (int i = 0; i < model.EducationDetail.Count; i++)
                                {
                                    model.EducationDetail[i].ddlList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                                }
                            }
                            return View(model);
                        }
                    }
                    if (!string.IsNullOrEmpty(model.PANNo))
                    {
                        var chkemployeepanno = Common.CheckPreviousEmployeePanserver(model.PANNo, model.ApplicationNumber, true, model.OldEmployeeNumber, "CON");
                        if (chkemployeepanno != "")
                        {
                            TempData["alertMsg"] = chkemployeepanno;
                            model.Status = model.Status == null ? "" : model.Status;
                            if (model.EducationDetail.Count > 0)
                            {
                                for (int i = 0; i < model.EducationDetail.Count; i++)
                                {
                                    model.EducationDetail[i].ddlList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                                }
                            }
                            return View(model);
                        }
                    }
                    #region FileSave
                    if (model.PersonDocImage != null)
                    {
                        var extensionPhoto = Path.GetExtension(model.PersonDocImage.FileName.ToLower());
                        if (!allowedExtensionsPhoto.Contains(extensionPhoto))
                        {
                            Errormessages = "Please upload any one of these type photo [.jpg,.jpeg,.png,.pdf,.PDF]";
                        }
                    }
                    if (model.JoiningReport != null)
                    {
                        var extensionPhoto = Path.GetExtension(model.JoiningReport.FileName.ToLower());
                        if (!validFileTypes.Contains(extensionPhoto))
                        {
                            Errormessages = "Please upload any one of these type photo [.jpg,.jpeg,.png]";
                        }
                    }
                    if (model.CantidateSignature != null)
                    {
                        var extensionPhoto = Path.GetExtension(model.CantidateSignature.FileName.ToLower());
                        if (!validFileTypes.Contains(extensionPhoto))
                        {
                            Errormessages = "Please upload any one of these type documents [.png,.jpg,.jpeg,gif]";
                        }
                    }
                    if (model.EducationDetail.Count > 0)
                    {
                        for (int i = 0; i < model.EducationDetail.Count; i++)
                        {
                            if (model.EducationDetail[i].Certificate != null)
                            {
                                var extension = Path.GetExtension(model.EducationDetail[i].Certificate.FileName.ToLower());

                                if (!validFileTypes.Contains(extension))
                                {
                                    Errormessages = "Please upload any one of these type documents [.pdf,.jpg,.jpeg,.gif,.png]";
                                }
                            }
                        }
                    }
                    #endregion
                    if (Errormessages == "")
                    {
                        var result = recruitmentService.VerifyCON(model, userId);
                        if (result.Item1 == 1)
                        {
                            int ConAppId = model.ConsultantAppointmentId ?? 0;
                            TempData["succMsg"] = "Application verified / Employee number generated:" + result.Item3;
                            return RedirectToAction("CONVerificationList", "Requirement");
                        }
                        else if (result.Item1 == 2)
                        {
                            TempData["succMsg"] = "Already verified";
                            return RedirectToAction("CONVerificationList", "Requirement");
                        }
                        else if (result.Item1 == -1)
                        {
                            TempData["succMsg"] = "Record not found";
                            return RedirectToAction("CONVerificationList", "Requirement");
                        }
                        else
                        {
                            TempData["errMsg"] = "Something went wrong please contact administrator";
                            return RedirectToAction("CONVerificationList", "Requirement");
                        }
                    }
                    else
                    {
                        string messages = string.Join("<br />", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));
                        if (model.EducationDetail.Count > 0)
                        {
                            for (int i = 0; i < model.EducationDetail.Count; i++)
                            {
                                model.EducationDetail[i].ddlList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                            }
                        }
                        TempData["errMsg"] = messages;
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                    messages += Errormessages;
                    TempData["errMsg"] = messages;
                    if (model.EducationDetail.Count > 0)
                    {
                        for (int i = 0; i < model.EducationDetail.Count; i++)
                        {
                            model.EducationDetail[i].ddlList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                        }
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.List = new List<MasterlistviewModel>();
                model.ApplicatonEntryDate = DateTime.Now;
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.Proof = Common.GetCodeControlList("tblRCTGovProof");
                ViewBag.EMPType = Common.GetCodeControlList("tblRCTEmployeeTypeCategory");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                if (model.EducationDetail.Count > 0)
                {
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                    {
                        model.EducationDetail[i].ddlList = Common.GetCourseList(model.EducationDetail[i].QualificationId ?? 0);
                    }
                }
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult LoadDesignationConsultantList(string term)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteDesignationConsultantList(term);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #endregion

        #region Payroll Initiation 
        public ActionResult PayrollInitiation()
        {
            try
            {
                PayrollInitiationModel model = new PayrollInitiationModel();
                List<MasterlistviewModel> monthlist = new List<MasterlistviewModel>();
                FinOp fo = new FinOp(System.DateTime.Now, true);
                var MonthFinList = fo.GetAllSalMonths();
                if (MonthFinList.Count > 0)
                {
                    for (int i = 0; i < MonthFinList.Count; i++)
                    {
                        monthlist.Add(new MasterlistviewModel()
                        {

                            id = i,
                            name = MonthFinList[i]
                        });
                    }
                }
                var user = Common.getUserIdAndRole(User.Identity.Name);
                model.RoleId = user.Item2;
                ViewBag.Finyearmonth = monthlist;
                ViewBag.SalaryType = Common.GetCodeControlList("PayOfBill");
                ViewBag.OSGVendor = Common.GetAgencyMasterList();
                ViewBag.EmployeeCategory = Common.GetCodeControlList("RCTEmployeeCategory");
                return View(model);
            }
            catch (Exception ex)
            {
                PayrollInitiationModel model = new PayrollInitiationModel();
                List<MasterlistviewModel> monthlist = new List<MasterlistviewModel>();
                FinOp fo = new FinOp(System.DateTime.Now);
                var MonthFinList = fo.GetAllSalMonths();
                if (MonthFinList.Count > 0)
                {
                    for (int i = 0; i < MonthFinList.Count; i++)
                    {
                        monthlist.Add(new MasterlistviewModel()
                        {

                            id = i,
                            name = MonthFinList[i]
                        });
                    }
                }
                var user = Common.getUserIdAndRole(User.Identity.Name);
                model.RoleId = user.Item2;
                ViewBag.Finyearmonth = monthlist;
                ViewBag.SalaryType = Common.GetCodeControlList("PayOfBill");
                ViewBag.EmployeeCategory = Common.GetCodeControlList("RCTEmployeeCategory");
                ViewBag.OSGVendor = Common.GetAgencyMasterList();
                return View(model);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPayRollInitaDate(string MonthAndYear, int SalaryType, int roleid)
        {
            string saltype = "";
            if (roleid == 102 || roleid == 103 || roleid == 104 || roleid == 104 || roleid == 105 || roleid == 106 || roleid == 93)
            { saltype = "Adhoc"; }
            else if (roleid == 107 || roleid == 108 || roleid == 109)
            { saltype = "OSG"; }

            DateTime Frmdate = DateTime.Now;
            DateTime Frmdate_osg = DateTime.Now;
            DateTime todate = DateTime.Now;

            if (saltype == "Adhoc")
            {
                FinOp fac = new FinOp(System.DateTime.Now);
                Frmdate = fac.GetMonthFirstDate(MonthAndYear);
                todate = fac.GetMonthLastDate(MonthAndYear);
            }
            else
            {
                FinOp fac = new FinOp(System.DateTime.Now);

                Frmdate = RequirementService.getOSGFirstSalaryProcessdate();
                Frmdate_osg = RequirementService.getOSGLastSalaryProcessdate();
                //  Frmdate = fac.GetMonthFirstDate(MonthAndYear);
                todate = fac.GetMonthLastDate(MonthAndYear);
            }
            //}
            var result = new { FrmSalDate = Frmdate, toSalDate = todate, FrmSalDateOSG = Frmdate_osg };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult PayrollInitiation(PayrollInitiationModel model)
        {
            FinOp fac = new FinOp(System.DateTime.Now);
            try
            {
                List<MasterlistviewModel> monthlist = new List<MasterlistviewModel>();
                FinOp fo = new FinOp(System.DateTime.Now);
                var MonthFinList = fo.GetAllSalMonths();
                if (MonthFinList.Count > 0)
                {
                    for (int i = 0; i < MonthFinList.Count; i++)
                    {
                        monthlist.Add(new MasterlistviewModel()
                        {

                            id = i,
                            name = MonthFinList[i]
                        });
                    }
                }
                ViewBag.Finyearmonth = monthlist;
                ViewBag.SalaryType = Common.GetCodeControlList("PayOfBill");
                ViewBag.EmployeeCategory = Common.GetCodeControlList("RCTEmployeeCategory");
                ViewBag.OSGVendor = Common.GetAgencyMasterList();
                if (ModelState.IsValid)
                {
                    string username = User.Identity.Name;
                    var user = Common.getUserIdAndRole(username);
                    int userid = user.Item1;
                    int roleid = user.Item2;
                    if (roleid == 102 || roleid == 103 || roleid == 104 || roleid == 104 || roleid == 105 || roleid == 106)
                        model.Appointmenttype = "Adhoc";
                    else if (roleid == 93)
                        model.Appointmenttype = "Adhoc";
                    else if (roleid == 107 || roleid == 108 || roleid == 109)
                        model.Appointmenttype = "OSG";
                    model.UserId = userid;
                    model.SalaryMonthDate = fac.GetMonthFirstDate(model.SalaryMonth);
                    if (model.Appointmenttype == "OSG")
                        model.SalaryType = 1;
                    //*check hotcode*//
                    //model.SalaryMonth = "Feb - 2021";
                    //model.SalaryMonthDate = fac.GetMonthFirstDate(model.SalaryMonth);
                    //model.FromInitDate = fac.GetMonthFirstDate(model.SalaryMonth);
                    //model.ToInitDate = fac.GetMonthLastDate(model.SalaryMonth);
                    //*End with hotcode*//
                    int payroleid = RequirementService.ExecuteSalaryProcessing(model);
                    //if(payroleid>0&&(roleid == 89|| roleid == 93))
                    //{
                    //    ReportsController RPC = new ReportsController();
                    //    RPC.DownloadExportAdhocPayrollDetails(payroleid);
                    //}
                    //else if(payroleid > 0 && roleid == 97) 
                    //{
                    //    ReportsController RPC = new ReportsController();
                    //    RPC.DownloadExportOSGPayrollDetail(payroleid);
                    //}
                    if (payroleid > 0)
                    {
                        TempData["succMsg"] = "Payroll Initiation Successfully";
                        return RedirectToAction("PayrollInitiationList");
                    }
                    //else if(payroleid==0)
                    //{
                    //    TempData["alertMsg"] = "No Records Found For this Month";
                    //    return RedirectToAction("PayrollInitiationList");
                    //}
                    else
                    {
                        TempData["errMsg"] = "Something went to wrong Please Contact Administrator";
                        return RedirectToAction("PayrollInitiationList");
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
                List<MasterlistviewModel> monthlist = new List<MasterlistviewModel>();
                FinOp fo = new FinOp(System.DateTime.Now);
                var MonthFinList = fo.GetAllSalMonths();
                if (MonthFinList.Count > 0)
                {
                    for (int i = 0; i < MonthFinList.Count; i++)
                    {
                        monthlist.Add(new MasterlistviewModel()
                        {

                            id = i,
                            name = MonthFinList[i]
                        });
                    }
                }
                ViewBag.Finyearmonth = monthlist;
                ViewBag.SalaryType = Common.GetCodeControlList("PayOfBill");
                ViewBag.EmployeeCategory = Common.GetCodeControlList("RCTEmployeeCategory");
                WriteLog.SendErrorToText(ex);
                TempData["errMsg"] = ex.Message;
                return View(model);
            }
        }
        //public ActionResult PayrollInitiationList()
        //{
        //    return View();
        //}
        [HttpPost]
        public JsonResult GetPayrollInitiation(PayrollInitiationSearchModel model, int pageIndex, int pageSize, DateFilterModel MonthStartDate, DateFilterModel MonthEndDate)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = recruitmentService.GetPayrollInitiationList(model, pageIndex, pageSize, MonthStartDate, MonthEndDate, userid, roleid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }
        public ActionResult DownloadPayrollAdhoc(int PayrollId)
        {
            if (PayrollId > 0)
            {
                RCTReportMasterController RPC = new RCTReportMasterController();
                RPC.DownloadExportAdhocPayrollDetails(PayrollId);
            }
            return RedirectToAction("PayrollInitiationList", "Requirement");
        }
        [HttpPost]
        public JsonResult SendMailPayrollAdhoc(int PayrollId)
        {
            string user_logged_in = User.Identity.Name;
            int UserId = Common.GetUserid(user_logged_in);
            object output = RCTEmailContentService.SendMailForPayrollAttachment(PayrollId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SendMailOSGPayroll(int PayrollId)
        {
            string user_logged_in = User.Identity.Name;
            int UserId = Common.GetUserid(user_logged_in);
            object output = RCTEmailContentService.SendMailForPayrollOSGAccounts(PayrollId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SendMailOSGPayrollVendor(int PayrollId)
        {
            string user_logged_in = User.Identity.Name;
            int UserId = Common.GetUserid(user_logged_in);
            object output = RCTEmailContentService.SendMailForPayrollOSGVendor(PayrollId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public static int CheckPayrollProcessing(int Payrollid, int userId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from pah in context.tblRCTPayroll
                                 where pah.RCTPayrollId == Payrollid && pah.AppointmentType == "Adhoc" && pah.Status == "Init"
                                 select pah).FirstOrDefault();
                    var salaryCompleted = context.tblSalaryPaymentHead.Any(m => m.PaymentMonthYear == query.SalaryMonth
                    && m.TypeOfPayBill == query.SalaryType && m.Status == "Approval Pending");
                    if (salaryCompleted)
                        return -2;
                    else if (query != null)
                        return 1;
                    else
                        return -1;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public JsonResult StartProcessingPayroll(int Payrollid)
        {
            try
            {
                byte[] byteInfo = null;
                string fileType = string.Empty;
                string excelname = string.Empty;
                string username = User.Identity.Name;
                int userid = Common.GetUserid(username);
                var PayrollData = CheckPayrollProcessing(Payrollid, userid);
                if (PayrollData == 1)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = (from pah in context.tblRCTPayroll where pah.RCTPayrollId == Payrollid select pah).FirstOrDefault();
                        string salarytype = string.Empty;
                        if (query.SalaryType == 1)
                            salarytype = "Main";
                        else if (query.SalaryType == 0)
                            salarytype = "Pensioner";
                        else
                            salarytype = "Supplementary";
                        DataTable dt1 = new DataTable();
                        DataTable dt2 = new DataTable();
                        excelname = "PAYROLL DATA Process " + salarytype + " Adhoc THE MONTH OF" + "-" + query.SalaryMonth;
                        DataSet dset = new DataSet();
                        //DataSet dset = RCTReportMasterController.PayrollProcessingData(Payrollid, userid);
                        using (SqlConnection conn = Common.getConnection())
                        {
                            SqlCommand sqlComm = new SqlCommand("SPRCTRequestToProcessSalary", conn);
                            sqlComm.Parameters.AddWithValue("@PayrollId", Payrollid);
                            sqlComm.Parameters.AddWithValue("@userId", userid);
                            sqlComm.CommandType = CommandType.StoredProcedure;
                            sqlComm.CommandTimeout = 1000;
                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = sqlComm;
                            da.Fill(dset);
                        }
                        dt1 = dset.Tables[0].Copy();
                        if (dset.Tables.Count > 1)
                            dt2 = dset.Tables[1].Copy();
                        MemoryStream workStream = new MemoryStream();
                        byteInfo = workStream.ToArray();
                        workStream.Write(byteInfo, 0, byteInfo.Length);
                        if (dset != null)
                        {
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                if (dt1.Rows.Count > 0)
                                    wb.Worksheets.Add(dt1, "New");
                                if (dt2.Rows.Count > 0)
                                    wb.Worksheets.Add(dt2, "Different");
                                wb.SaveAs(workStream);
                                workStream.Position = 0;
                                byteInfo = workStream.ToArray();
                                int mailstatus = 0;
                                if (byteInfo != null && byteInfo.Length > 0)
                                {
                                    mailstatus = RCTEmailContentService.SendMailStartProcessingPayrollAttachment(Payrollid, byteInfo);
                                }
                            }
                        }

                        fileType = Common.GetMimeType("xls");
                        //Response.AddHeader("Content-Disposition", "filename=" + excelname + ".xls");
                        //return File(workStream, fileType);
                    }
                }
                var result = new { msg = PayrollData, bytefile = byteInfo, fileType = fileType, fileName = excelname + ".xls" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                var result = new { msg = -1 };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }
        public JsonResult ChangProcessStatusOSG(int PayrollId)
        {
            try
            {
                byte[] byteInfo = null;
                string excelname = string.Empty;
                string fileType = string.Empty;
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                int res = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = (from pay in context.tblRCTPayroll
                                 where pay.RCTPayrollId == PayrollId && pay.AppointmentType == "OSG"
                                 select pay).FirstOrDefault();
                    if (query != null)
                    {
                        string month = query.SalaryMonth;
                        if (!context.tblAgencySalary.Any(x => x.MonthYearStr == month && x.VendorId == query.VendorId && x.Status == "Completed"))
                        {
                            excelname = "PAYROLL DATA Process Main OSG THE MONTH OF" + "-" + query.SalaryMonth;
                            DataSet dset = new DataSet();
                            using (SqlConnection conn = Common.getConnection())
                            {
                                SqlCommand sqlComm = new SqlCommand("SPRCTRequestToProcessSalary", conn);
                                sqlComm.Parameters.AddWithValue("@PayrollId", PayrollId);
                                sqlComm.Parameters.AddWithValue("@userId", userid);
                                sqlComm.CommandType = CommandType.StoredProcedure;
                                sqlComm.CommandTimeout = 3500;
                                SqlDataAdapter da = new SqlDataAdapter();
                                da.SelectCommand = sqlComm;
                                da.Fill(dset);
                            }
                            dt1 = dset.Tables[0].Copy();
                            if (dset.Tables.Count > 1)
                                dt2 = dset.Tables[1].Copy();

                            MemoryStream workStream = new MemoryStream();
                            byteInfo = workStream.ToArray();
                            workStream.Write(byteInfo, 0, byteInfo.Length);

                            if (dset != null)
                            {
                                using (XLWorkbook wb = new XLWorkbook())
                                {
                                    if (dt1.Rows.Count > 0)
                                        wb.Worksheets.Add(dt1, "New");
                                    if (dt2.Rows.Count > 0)
                                        wb.Worksheets.Add(dt2, "Different");
                                    wb.SaveAs(workStream);
                                    workStream.Position = 0;
                                    byteInfo = workStream.ToArray();
                                    if (byteInfo != null && byteInfo.Length > 0)
                                    {
                                        int mailstatus = RCTEmailContentService.SendMailForPayrollOSGAccounts(PayrollId, byteInfo);
                                    }
                                }
                            }
                            fileType = Common.GetMimeType("xls");
                            //Response.AddHeader("Content-Disposition", "filename=" + excelname + ".xls");
                            //return File(workStream, fileType);
                            res = 1;
                        }
                    }
                    else
                        res = -1;
                    //var query = (from pay in context.tblRCTPayroll
                    //             where pay.RCTPayrollId == PayrollId && pay.AppointmentType == "OSG"
                    //             select pay).FirstOrDefault();
                    //if (query != null)
                    //{
                    //    string month = query.SalaryMonth;
                    //    var chkQuery = (from c in context.tblRCTPayroll
                    //                    where c.SalaryMonth == month && c.AppointmentType == "OSG"
                    //                    && c.Status == "Requested for salary processing"
                    //                    orderby c.RCTPayrollId descending
                    //                    select c).FirstOrDefault();
                    //    if (chkQuery != null)
                    //        chkQuery.Status = "Overwritten";
                    //    var data = RCTEmailContentService.SendMailForPayrollOSGAccounts(PayrollId);
                    //    query.Status = "Requested for salary processing";
                    //    context.SaveChanges();
                    //    res = 1;
                    //}
                    //else
                    //    res = -1;
                }
                var result = new { msg = res, bytefile = byteInfo, fileType = fileType, fileName = excelname + ".xls" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                var result = new { msg = -1 };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region RCTCommitmentDetails

        public ActionResult RCTCommitmentDetails()
        {
            return View();
        }

        public ActionResult _RCTCommitmentDetails(string CommitmentNumber)
        {
            EmployeeBasicDetails model = new EmployeeBasicDetails();
            try
            {
                model = RequirementService.GetCommitmenttransactionDetails(CommitmentNumber);
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult LoadRCTCommitmentList(string term)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteRCTCommitmentNo(term);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        [HttpGet]
        public JsonResult RCTHis(int AppId, string Cat, int OrderId)
        {
            try
            {
                var data = Common.EmployeeHistoryLog(AppId, Cat, OrderId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region OTHPaymentDeductionUpload

        public ActionResult OTHPaymentDeductionUpload(int OTHUpMastrId = 0)
        {
            OtherPaymentDeductionUploadModel model = new OtherPaymentDeductionUploadModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    ViewBag.MonthList = Common.GetmonthList();
                    string username = User.Identity.Name;
                    var user = Common.getUserIdAndRole(username);
                    model.UserId = user.Item1;
                    model.RoleId = user.Item2;
                    if (OTHUpMastrId > 0)
                    {
                        var query = context.tblRCTOTHPaymentDeductionUploadMaster.Where(x => x.OTHUploadMasterId == OTHUpMastrId).FirstOrDefault();
                        if (query != null)
                        {
                            model.MonthandYear = query.PaymentMonth;
                            model.FromDate = query.FromDate;
                            model.ToDate = query.ToDate;
                            ViewBag.uploadId = query.OTHPaymentDeductionUploadId;
                            model.OTHUploadMasterId = query.OTHUploadMasterId;
                        }
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.MonthList = Common.GetmonthList();
                return View();
            }
        }

        [HttpPost]
        public ActionResult OTHPaymentDeductionUpload(OtherPaymentDeductionUploadModel model)
        {
            ViewBag.MonthList = Common.GetmonthList();
            try
            {
                if (ModelState.IsValid)
                {
                    string username = User.Identity.Name;
                    var user = Common.getUserIdAndRole(username);
                    model.UserId = user.Item1;
                    var result = recruitmentService.AddOthMasterUpload(model);
                    if (result.Item1 == 1 && result.Item2 == "")
                    {
                        TempData["succMsg"] = "Add SucessFully in Master";
                        return RedirectToAction("OTHPaymentDeductionUploadList");
                    }
                    else if (result.Item1 == 2 && result.Item2 == "")
                    {
                        TempData["succMsg"] = "Add and  iniat WorkFlow SucessFully";
                        return RedirectToAction("OTHPaymentDeductionUploadList");
                    }
                    else if (result.Item1 == 2 && result.Item2 != "")
                    {
                        TempData["errMsg"] = result.Item2;
                        return View(model);
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                           .SelectMany(x => x.Errors)
                                           .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                    return View(model);
                }
                return RedirectToAction("OTHPaymentDeductionUploadList");
            }
            catch (Exception ex)
            {
                return RedirectToAction("OTHPaymentDeductionUploadList");
            }
        }

        public ActionResult OTHPaymentDeductionUploadView(int OTHUpMastrId, bool isView = false)
        {
            OtherPaymentDeductionUploadModel model = new OtherPaymentDeductionUploadModel();
            try
            {
                using (var context = new IOASDBEntities())
                {

                    string username = User.Identity.Name;
                    var user = Common.getUserIdAndRole(username);
                    model.UserId = user.Item1;
                    model.RoleId = user.Item2;
                    if (OTHUpMastrId > 0)
                    {
                        var query = context.tblRCTOTHPaymentDeductionUploadMaster.Where(x => x.OTHUploadMasterId == OTHUpMastrId).FirstOrDefault();
                        if (query != null)
                        {
                            model.MonthandYear = query.PaymentMonth;
                            model.FromDate = query.FromDate;
                            model.ToDate = query.ToDate;
                            model.UploadId = query.OTHPaymentDeductionUploadId;
                            model.OTHUploadMasterId = query.OTHUploadMasterId;
                            model.FormStrDate = String.Format("{0:dd-MMMM-yyyy}", query.FromDate);
                            model.ToStrDate = String.Format("{0:dd-MMMM-yyyy}", query.ToDate);
                            var MasterUp = context.tblRCTOTHPaymentDeductionUpload.Where(x => x.OTHPaymentDeductionUploadId == model.UploadId).Select(x => x.Status).FirstOrDefault();
                            if (MasterUp != null)
                                model.Status = MasterUp;
                        }

                        model.IsViewMode = isView;
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(216, "OTHUpload", 0);

                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {

                return View(model);
            }
        }

        public ActionResult OTHPaymentDeductionUploadList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetOTHMasterList(SearchOTHUploadMaster model, int pageIndex, int pageSize, DateFilterModel FormStrDate, DateFilterModel ToStrDate)
        {
            try
            {
                var result = RequirementService.GetOtherPaymentUploadList(model, pageIndex, pageSize, FormStrDate, ToStrDate);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ValidateOTHPaymentDeduction(ValidatePaymentDeduction model)
        {
            try
            {
                Utility _uty = new Utility();
                List<tblRCTOTHPaymentDeductionUploadDetail> list = new List<tblRCTOTHPaymentDeductionUploadDetail>();
                string extension = Path.GetExtension(model.template.FileName).ToLower();
                string connString = "";
                string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                string actName = Path.GetFileName(model.template.FileName);
                var guid = Guid.NewGuid().ToString();
                var docName = guid + "_" + actName;
                string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/RCTOTHTemplate"), docName);
                string msg = "Valid";
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                int uploadId = 0;
                int resultId = 0;
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/RCTOTHTemplate"));
                }
                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path1))
                    { System.IO.File.Delete(path1); }
                    model.template.SaveAs(path1);
                    if (extension.ToLower().Trim() == ".csv")
                    {
                        DataTable dt = _uty.ConvertCSVtoDataTable(path1);
                        list = Converter.GetEntityList<tblRCTOTHPaymentDeductionUploadDetail>(dt);
                    }
                    else if (extension.ToLower().Trim() == ".xls" && Environment.Is64BitOperatingSystem == false)
                    {
                        connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        DataTable dt = _uty.ConvertXSLXtoDataTable(path1, connString, "OtherPaydeduction");
                        list = Converter.GetEntityList<tblRCTOTHPaymentDeductionUploadDetail>(dt);
                    }
                    else if (extension.ToLower().Trim() == ".xlsx" && Environment.Is64BitOperatingSystem == false)
                    {
                        connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        DataTable dt = _uty.ConvertXSLXtoDataTable(path1, connString, "OtherPaydeduction");
                        list = Converter.GetEntityList<tblRCTOTHPaymentDeductionUploadDetail>(dt);
                    }
                    else
                    {
                        connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        DataTable dt = _uty.ConvertXSLXtoDataTable(path1, connString, "OtherPaydeduction");
                        list = Converter.GetEntityList<tblRCTOTHPaymentDeductionUploadDetail>(dt);
                    }
                    if (list != null && list.Count > 0)
                    {
                        tblRCTOTHPaymentDeductionUpload master = new tblRCTOTHPaymentDeductionUpload();
                        master.ActualName = actName;
                        master.DocName = docName;
                        master.Guid = guid;
                        master.PaymentDeductionMonthYear = model.MonthandYear;
                        master.FromDate = model.FromDate;
                        master.ToDate = model.ToDate;
                        master.Crtd_By = userId;
                        master.Crtd_Ts = DateTime.Now;
                        var uploadresult = recruitmentService.ValidateOTHPDList(master, list);


                        uploadId = uploadresult.Item1;
                        resultId = uploadresult.Item2;
                        if (uploadId == 0)
                            msg = "Something went wrong please contact administrator.";
                    }
                    else
                        msg = "Please upload valid upload file.";
                }
                else
                {
                    msg = "Please Upload Files in .xls or .xlsx format";
                }
                return Json(new { status = msg, guid = guid, uploadId = uploadId, resultId=resultId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //IOASException.Instance.HandleMe(this, ex);
                WriteLog.SendErrorToText(ex);
                return Json(new { status = "Something went wrong please contact administrator." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetValidateOTHList(int pageIndex, int pageSize, int uploadId)
        {
            try
            {
                var model = recruitmentService.GetValidateOTHList(pageIndex, pageSize, uploadId);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult AddOtherPaymentValidatedEmployees(int uploadId)
        {
            lock (lockObj)
            {
                string msg = "Something went wrong please contact administrator.";
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                var result = recruitmentService.ValidateAndaddOtherPayment(uploadId, userId);
                if (result.Item2 > 0 && result.Item1 != "")
                    msg = result.Item1;
                else
                    msg = result.Item1;
                return Json(new { status = msg, paymentHeadId = result }, JsonRequestBehavior.AllowGet);
            }
        }

        public FileStreamResult ExportOTHUploadData(int uploadId)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = recruitmentService.GetBulkUploadEmployeeList(uploadId);
                return coreAccountService.toSpreadSheet(dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        #endregion

        #region  OSGDocumentUpload

        [HttpGet]
        public JsonResult LoadOSGEmployeeList(string term)
        {
            try
            {
                string user = User.Identity.Name;
                var data = Common.GetAutoCompleteOSGEmployeeList(term, user);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Loaddocbaseappointment(int id)
        {
            var locationdata = RequirementService.GettypeOfDocumentList(id);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult FillDocumentlist(int id, string appointmentname)
        {
            object output = RequirementService.GetAttachementList(id, appointmentname);
            return Json(new { result = output });

        }

        [HttpPost]
        public JsonResult OSGDocumentadd(OutSourcingDocumentUpload model)
        {

            int value = RequirementService.AddAttachement(model);
            return Json(value, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult OSGUploadDocument()
        {
            try
            {
                ViewBag.type = new List<MasterlistviewModel>();

                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        #endregion

        #region Employee Portal
        [HttpGet]
        public ActionResult _EmployeeResetPassword(string employeeId)
        {
            EmployeeDetailModel model = new EmployeeDetailModel();
            try
            {
                model = RequirementService.GetRestEmployeeDetails(employeeId);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return View(model);
            }
        }

        public ActionResult EmployeePasswordReset(string employeeId)
        {
            try
            {
                string username = User.Identity.Name;
                int userid = Common.GetUserid(username);
                var status = RequirementService.ResetEmployeePassword(employeeId, userid);
                if (status == 1)
                    TempData["succMsg"] = "Employee password reset successfully and sent to the employee's registered E-mail";
                else
                    TempData["errMsg"] = "Something went to Wrong contact Adminstrator";
                return RedirectToAction("EmployeeMaster");
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                TempData["errMsg"] = "Something went to Wrong contact Adminstrator";
                return RedirectToAction("EmployeeMaster");
            }
        }
        [HttpGet]
        public ActionResult _EmployeePortalAccessModifier(string employeeId)
        {
            EmployeeDetailModel model = new EmployeeDetailModel();
            ViewBag.getStatus = Common.GetCodeControlList("EmployeeStatus");
            try
            {
                model = RequirementService.GetAccessEmployeeDetails(employeeId);
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return View(model);
            }
        }
        public ActionResult EmployeeStatusChange(EmployeeDetailModel model)
        {
            try
            {
                string username = User.Identity.Name;
                int userid = Common.GetUserid(username);
                var status = RequirementService.Employeeaccessmodifier(model, userid);
                if (status == 1 && model.EmployeeStatus == "Active")
                    TempData["succMsg"] = "Employee portal access enabled.";
                else if (status == 1 && model.EmployeeStatus == "InActive")
                    TempData["succMsg"] = "Employee portal access disabled.";
                else
                    TempData["errMsg"] = "Something went to Wrong contact Adminstrator";
                return RedirectToAction("EmployeeMaster");
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                TempData["errMsg"] = "Something went to Wrong contact Adminstrator";
                return RedirectToAction("EmployeeMaster");
            }
        }
        #endregion

        public ActionResult ShowFile(string filePath, string file)
        {
            try
            {
                string fileType = Common.GetMimeType(Path.GetExtension(file));
                byte[] fileData = file.GetFileData(Server.MapPath(filePath));
                file = file.Substring(file.LastIndexOf("_") + 1);
                Response.AddHeader("Content-Disposition", "inline; filename=" + file);
                return File(fileData, fileType);
            }
            catch (FileNotFoundException)
            {
                throw new HttpException(404, "File not found.");
            }
        }

        [HttpPost]
        public ActionResult StopTermEndNotification(string employeeId, int? stopmailId = null)
        {
            try
            {
                int UserId = Common.GetUserid(User.Identity.Name);
                var res = RequirementService.StopTermEndNotification(employeeId, UserId, stopmailId);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ConsultantMaster()
        {
            ViewBag.consultantNationality = Common.GetConsultantNationality();
            ViewBag.Gender = Common.GetCodeControlList("RCTGender");
            ViewBag.state = Common.GetStatelist();
            ViewBag.country = Common.getCountryList();
            ViewBag.GstDoc = Common.GetGstSupportingDoc();
            ViewBag.Vensupdoc = Common.GetVendorSupportingDoc();
            ViewBag.ventdsdoc = Common.GetVendorTdsDoc();
            ViewBag.consultantCategory = Common.GetConsultantCategory();
            ViewBag.serviceCategory = Common.GetCategoryService();
            ViewBag.serviceType = Common.GetServiceTypeList();
            ViewBag.suppliertype = Common.GetSupplierType();
            ViewBag.tdssection = Common.GetTdsList();
            ViewBag.bankcountry = Common.getCountryList();
            ViewBag.Professional = Common.GetCodeControlList("ConsultantProfessional");
            ViewBag.ProfessionalFirm = Common.GetCodeControlList("ConsultantFirmProfessional");
            ViewBag.ConsultantMasterList = Common.GetConsultantMasterList();
            return View();
        }

        [Authorized]
        [HttpPost]
        public JsonResult GetConsultantMaster(ConsultantMasterSearchModel model, int pageIndex, int pageSize)
        {


            object output = RequirementService.GetConsultantMasterList(model, pageIndex, pageSize);

            //object output = MasterService.GetVendorList(model, pageIndex, pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ConsultantMaster(ConsultantMaster model)
        {
            ViewBag.consultantNationality = Common.GetConsultantNationality();
            ViewBag.consultantCategory = Common.GetConsultantCategory();
            ViewBag.vendorCountry = Common.GetAgencyType();
            ViewBag.Professional = Common.GetCodeControlList("ConsultantProfessional");
            ViewBag.ProfessionalFirm = Common.GetCodeControlList("ConsultantFirmProfessional");
            ViewBag.Gender = Common.getGender();
            ViewBag.state = Common.GetStatelist();
            ViewBag.country = Common.getCountryList();
            ViewBag.GstDoc = Common.GetGstSupportingDoc();
            ViewBag.Vensupdoc = Common.GetVendorSupportingDoc();
            ViewBag.ventdsdoc = Common.GetVendorTdsDoc();
            ViewBag.vendorcode = MasterService.GetVendorCode();
            ViewBag.serviceCategory = Common.GetCategoryService();
            ViewBag.serviceType = Common.GetServiceTypeList();
            ViewBag.suppliertype = Common.GetSupplierType();
            ViewBag.tdssection = Common.GetTdsList();
            var Username = User.Identity.Name;
            model.UserId = Common.GetUserid(Username);
            ViewBag.bankcountry = Common.getCountryList();
            //model.ConsultantCategory= txtNationality.



            if (ModelState.IsValid)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };

                if ((model.AttachmentName != null))
                {
                    for (int i = 0; i < model.AttachmentName.Length; i++)
                    {
                        if (model.ConsultantFile[i] != null)
                        {
                            string docname = Path.GetFileName(model.ConsultantFile[i].FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                ViewBag.filemsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }

                    }
                }


                int consultantStatus = RequirementService.ConsultantEmpMaster(model);
                if (consultantStatus == 1)
                {
                    //ViewBag.success = "Saved successfully";
                    //return RedirectToAction("ConsultantMaster", "Requirement");
                    TempData["succMsg"] = "Saved successfully";
                    return RedirectToAction("ConsultantMaster", "Requirement");
                }
                //else if (vendorStatus == 2)
                //{
                //    ViewBag.Msg = "This Vendor Account Number and GSTIN Number Already Exits";
                //    return View(model);
                //}
                else if (consultantStatus == 3)
                {
                    TempData["succMsg"] = "Consultant Master Updated successfully";
                    return RedirectToAction("ConsultantMaster", "Requirement");
                }

                else
                {
                    TempData["errMsg"] = "Somthing went to worng please contact Admin!.";
                    return RedirectToAction("ConsultantMaster", "Requirement");
                    //return View(model);
                }
                return View();
            }
            else
            {
                string messages = string.Join("<br />", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                TempData["errMsg"] = messages;


                return View();
            }
        }

        [Authorized]
        [HttpPost]
        public JsonResult EditConsultantMasterlist(int consultantMasterId)
        {
            object output = RequirementService.EditConsultantMaster(consultantMasterId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ConsultantMasterView(int MasterId = 0)
        {
            try
            {
                ConsultantMasterView model = new ConsultantMasterView();
                model = RequirementService.GetConsultantMasterView(MasterId);
                //ViewBag.processGuideLineId = 160;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ViewConsultantMaster(int ConsultantMasterId = 0)
        {
            try
            {
                ConsultantMasterView model = new ConsultantMasterView();
                model = RequirementService.GetConsultantMasterView(ConsultantMasterId);
                //ViewBag.processGuideLineId = 160;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        public JsonResult CheckPreviousGSTNumber(string GSTno)
        {
            object output = Common.CheckPreviousGSTNumber(GSTno);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckTINNumber(string TINno)
        {
            object output = Common.CheckTINNumber(TINno);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckPreviousMasterGSTNumber(string GSTno)
        {
            object output = Common.CheckPreviousMasterGSTNumber(GSTno);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        #region Release Payment
        public ActionResult RCTConsultantPaymentRelease(int ConsultantMasterId)
        {
            ConsultantMasterId = 5;
            ConsultantPaymentRelease model = new ConsultantPaymentRelease();
            ViewBag.OtherType = Common.GetCodeControlList("OtherType");
            ViewBag.List = new List<MasterlistviewModel>();
            //model.BasicAmount = 0;
            model = recruitmentService.GetConsultantPaymentRelease(ConsultantMasterId);
            return View(model);
        }


        [HttpPost]
        public ActionResult RCTConsultantPaymentRelease(ConsultantPaymentRelease model)
        {
            ViewBag.OtherType = Common.GetCodeControlList("OtherType");
            //ViewBag.PaymentType = Common.GetCodeControlList("RCTPayment");
            //ViewBag.Deduction = Common.GetCodeControlList("RCTDeduction");           
            ViewBag.List = new List<MasterlistviewModel>();
            model.TaxConversion_rate = 0;
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.CONOTHDetail.Count > 0)
                    {
                        foreach (var item in model.CONOTHDetail)
                        {
                            item.PaydecList = Common.GetCommonHeadList(1, item.OtherType ?? 0);
                        }
                    }
                    model.PaymentRelease_CrtdUser = Common.GetUserid(User.Identity.Name);
                    var result = recruitmentService.RCTConsultantPaymentRelease(model);
                    if (result.Item1 == 1)
                    {
                        TempData["succMsg"] = "Add Sucessfully";
                        return RedirectToAction("ConsultantMaster", "Requirement");
                    }
                    //else if (model.Consultant_MasterId > 0 && status.Item1 == 2)
                    //{
                    //    TempData["succMsg"] = "Update Sucessfully";
                    //    return RedirectToAction("OtherPaymentDeductionList", "Requirement");
                    //}
                    //else if ((model.OTHPayDeductionId == null || model.OTHPayDeductionId != null) && status.Item1 == 3)
                    //{
                    //    TempData["alertMsg"] = status.Item3;
                    //    return RedirectToAction("OtherPaymentDeductionList", "Requirement");
                    //}
                    //else if ((model.OTHPayDeductionId == null || model.OTHPayDeductionId != null) && status.Item1 == 4)
                    //{
                    //    TempData["alertMsg"] = status.Item3;
                    //    return RedirectToAction("OtherPaymentDeductionList", "Requirement");
                    //}
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("RCTConsultantPaymentRelease", "Requirement");
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                #region Updatepaydeduction
                if (model.CONOTHDetail.Count > 0)
                {
                    foreach (var item in model.CONOTHDetail)
                    {

                        item.PaydecList = Common.GetCommonHeadList(1, item.OtherType ?? 0);
                    }
                }
                #endregion
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult LoadConsultantServiceList(string term, int? type = null)
        {
            try
            {

                var data = new List<AutoCompleteModel>();

                data = Common.GetAutoCompleteConsultantServiceList(term, type);

                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult ConsultantServicePayment(int ConsAppID)
        {
            ConsultantPaymentRelease model = new ConsultantPaymentRelease();
            ViewBag.OtherType = Common.GetCodeControlList("OtherType");
            ViewBag.List = new List<MasterlistviewModel>();
            //var consultantData= RequirementService.ConsultantServicePayment(ConsAppID);
            model = RequirementService.ConsultantServicePayment(ConsAppID);
            //var result = new { consultantData = consultantData };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region EmployeeMaster Separate
        public ActionResult CONEmployeeMaster()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetConsultantEmployeeList(SearchEmployeeModel model, int pageIndex, int pageSize, DateFilterModel DateOfBirth, DateFilterModel DateOfJoining)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = recruitmentService.GetConsultantEmployeeList(model, pageIndex, pageSize, DateOfBirth, DateOfJoining);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }
        public ActionResult STEEmployeeMaster()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetSTEEmployeeList(SearchEmployeeModel model, int pageIndex, int pageSize, DateFilterModel strDateofBirth, DateFilterModel strDateofJoining)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = recruitmentService.GetSTEEmployeeList(model, pageIndex, pageSize, strDateofBirth, strDateofJoining);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }
        public ActionResult OSGEmployeeMaster()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetOSGEmployeeList(SearchEmployeeModel model, int pageIndex, int pageSize, DateFilterModel strDateofBirth, DateFilterModel strDateofJoining)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = recruitmentService.GetOSGEmployeeList(model, pageIndex, pageSize, strDateofBirth, strDateofJoining);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Reliving tabList

        public ActionResult RelievingList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult getEmployeeRelievingList(int pageIndex, int pageSize, SearchOrderModel model, DateFilterModel EmployeeDateofBirth, string Category)
        {
            try
            {
                object output = recruitmentService.GetEmployeeRelievingList(pageIndex, pageSize, model, EmployeeDateofBirth, Category);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Stop Payment List

        public ActionResult StoppaymentList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult getEmployeeStoppaymentList(int pageIndex, int pageSize, SearchOrderModel model, string Category)
        {
            try
            {
                object output = recruitmentService.GetEmployeeLOPSPList(pageIndex, pageSize, model, Category);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Maternity leave

        public ActionResult MaternityLeaveList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetMaternityEmployeeLeaveList(int pageIndex, int pageSize, SearchOrderModel model, DateFilterModel FromDate, DateFilterModel ToDate, string Category)
        {
            try
            {
                object output = recruitmentService.GetEmployeeMaternityList(pageIndex, pageSize, model, FromDate, ToDate, Category);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Other Paymentdeduction

        //public ActionResult OtherPaymentDeductionList()
        //{
        //    return View();
        //}

        [HttpPost]
        public JsonResult GetEmployeeOtherPaymentList(SearchOtherPaymentModel model, int pageIndex, int pageSize, DateFilterModel CreatedDate, string Category)
        {
            try
            {
                object output = RequirementService.GetEmployeeOtherPaymentList(model, pageIndex, pageSize, CreatedDate, Category);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }
        #endregion

        #region PayrollInitiation
        public ActionResult PayrollInitiationList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetEmployeePayrollInitiation(PayrollInitiationSearchModel model, int pageIndex, int pageSize, DateFilterModel MonthStartDate, DateFilterModel MonthEndDate, string Category)
        {
            try
            {
                object output = recruitmentService.GetEmployeePayrollInitiationList(model, pageIndex, pageSize, MonthStartDate, MonthEndDate, Category);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region DAApplication Separate
        [HttpPost]
        public JsonResult GetEmployeeExtensionEnhList(ProjectExtentionEnhmentSearchListModel model, int pageIndex, int pageSize, DateFilterModel StrFrmDate, DateFilterModel StrtoDate, string Category)
        {
            try
            {
                object output = RequirementService.GetEmployeeExtandEnhment(model, pageIndex, pageSize, StrFrmDate, StrtoDate, Category);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult getEmployeeHRAList(int pageIndex, int pageSize, SearchOrderModel model, string Category)
        {
            try
            {
                object output = recruitmentService.GetEmployeeHRAList(pageIndex, pageSize, model, Category);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public JsonResult GetEmployeeChangeOfProjectList(SearchChangeofProjectModel model, int pageIndex, int pageSize, string Category)
        {
            try
            {
                object output = RequirementService.GetEmployeeChangeofProject(model, pageIndex, pageSize, Category);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetEmployeeCancelledApplicationList(ApplicationSearchListModel model, int pageIndex, int pageSize, string Category)
        {
            try
            {
                object output = RequirementService.GetEmployeeCancelApplicationList(model, pageIndex, pageSize, Category);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult CONDAApplicationStatus()
        {
            return View();
        }

        public ActionResult STEDAApplicationStatus()
        {
            return View();
        }
        public ActionResult OSGDAApplicationStatus()
        {
            return View();
        }
        #endregion

        [HttpGet]
        public JsonResult GetSalaryBreakUpHead(int groupId, int categoryId = 1)
        {
            try
            {
                object output = Common.GetRecruitCommonHeadList(categoryId, groupId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public JsonResult VerifyAadharPan(int STEId, string aadharnumber, string PanNo, string ApplicationNo, string EmployeeNumber)
        {
            try
            {
                object output = Common.GetVerifyAadharPan(STEId, aadharnumber, PanNo, ApplicationNo, EmployeeNumber);
                //output.
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public JsonResult nextVerifyAadharPan(int STEId, string aadharnumber, string PanNo, string AppicationNo, string EmployeeNumber)
        {
            try
            {
                object output = Common.GetnextVerifyAadharPan(STEId, aadharnumber, PanNo, AppicationNo, EmployeeNumber);
                //output.
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public JsonResult GetEmployeeAwaitingApplicationList(ApplicationSearchListModel model, int pageIndex, int pageSize)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = RequirementService.GetAwaitApplicationList(model, pageIndex, pageSize, roleid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpGet]
        public ActionResult STENewView(int STEID = 0, int WFid = 0)

        {
            STEModel model = new STEModel();
            try
            {
                ViewBag.List = new List<MasterlistviewModel>();
                model.ApplicationEntryDate = DateTime.Now;
                model.Status = string.Empty;
                model.Medical = 2;
                ViewBag.Apptype = Common.GetCodeControlList("STEAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                if (STEID > 0)
                {
                    model = recruitmentService.GetEditSTE(STEID);
                    if (model.DateofBirth != null)
                        ViewBag.Years = Common.getRequirementyear(model.DateofBirth.Value.Year);
                    if (model.FlowApprover == "CMAdmin")
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STEAdminFlow", 0);
                    else if (model.FlowApprover == "NDean")
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STEFlowDean", 0);
                    else
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STE Flow", 0);
                }
                else if (WFid > 0 && STEID == 0)
                {
                    model = Common.GetWFEditSTE(WFid);
                    var listcommitte = Common.GetCommittee();
                    if (listcommitte.Item1.Count > 0)
                    {
                        for (int i = 0; i < listcommitte.Item1.Count; i++)
                        {
                            if (i == 0)
                            {
                                model.CommiteeMemberId1 = listcommitte.Item1[i].id ?? 0;
                                model.CommiteeMember1 = listcommitte.Item1[i].name;
                            }
                            if (i == 1)
                            {
                                model.CommiteeMemberId2 = listcommitte.Item1[i].id ?? 0;
                                model.CommiteeMember2 = listcommitte.Item1[i].name;
                            }
                        }
                        var datacharperson = Common.GetChairPerson();
                        model.ChairpersonNameId = datacharperson.Item1;
                        model.ChairpersonName = datacharperson.Item2;
                    }
                }
                else
                {
                    var listcommitte = Common.GetCommittee();
                    if (listcommitte.Item1.Count > 0)
                    {
                        for (int i = 0; i < listcommitte.Item1.Count; i++)
                        {
                            if (i == 0)
                            {
                                model.CommiteeMemberId1 = listcommitte.Item1[i].id ?? 0;
                                model.CommiteeMember1 = listcommitte.Item1[i].name;
                            }
                            if (i == 1)
                            {
                                model.CommiteeMemberId2 = listcommitte.Item1[i].id ?? 0;
                                model.CommiteeMember2 = listcommitte.Item1[i].name;
                            }
                        }
                        var datacharperson = Common.GetChairPerson();
                        model.ChairpersonNameId = datacharperson.Item1;
                        model.ChairpersonName = datacharperson.Item2;
                    }
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(191, "", 0);
                }
                model.isDraftbtn = false;
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Apptype = Common.GetCodeControlList("STEAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(191, "", 0);
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(model.DateofBirth.Value.Year);
                else
                    ViewBag.Years = Common.getRequirementyear();
                WriteLog.SendErrorToText(ex);
                return View(model);
            }

        }

        [HttpPost]
        public ActionResult STENewView(STEModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Apptype = Common.GetCodeControlList("STEAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                if (model.EducationDetail.Count > 0)
                {
                    foreach (var item in model.EducationDetail)
                        item.DisiplineList = Common.GetCourseList(item.QualificationId ?? 0);
                }

                #region FileValidation
                if (model.PersonImage != null)
                {
                    var allowedExtensions = new[] { ".jpeg", ".png", ".jpg", ".gif" };
                    var extension = Path.GetExtension(model.PersonImage.FileName);
                    if (!allowedExtensions.Contains(extension.ToLower()))
                    {
                        TempData["alertMsg"] = "Please upload any one of these type candidate image [.jpeg,.png,.jpg]";
                        return View(model);
                    }
                    if (model.PersonImage.ContentLength > 1000000)
                    {
                        TempData["alertMsg"] = "You can upload candidate image up to 1MB";
                        return View(model);
                    }
                }

                if (model.Resume != null)
                {
                    var extension = Path.GetExtension(model.Resume.FileName);
                    if (extension.ToLower() != ".pdf")
                    {
                        TempData["alertMsg"] = "Please upload any one of these type Resume [.pdf]";
                        return View(model);
                    }
                    if (model.Resume.ContentLength > 5242880)
                    {
                        TempData["alertMsg"] = "You can upload Resume up to 5MB";
                        return View(model);
                    }
                }

                if (model.PIJustificationFile != null)
                {
                    var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
                    for (int i = 0; i < model.PIJustificationFile.Count(); i++)
                    {
                        if (model.PIJustificationFile[i] != null)
                        {
                            string extension = Path.GetExtension(model.PIJustificationFile[i].FileName);
                            if (!allowedExtensions.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type PI judtification document [.doc,.docx,.pdf]";
                                return View(model);
                            }
                            if (model.PIJustificationFile[i].ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload PI judtification document up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }

                if (model.EducationDetail != null)
                {
                    var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                    {
                        if (model.EducationDetail[i].Certificate != null)
                        {
                            string extension = Path.GetExtension(model.EducationDetail[i].Certificate.FileName);
                            if (!allowedExtensionsCer.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type certificate [.jpeg,.png,.jpg,.gif,.pdf]";
                                return View(model);
                            }
                            if (model.EducationDetail[i].Certificate.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload certificate up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }

                if (model.ExperienceDetail != null)
                {
                    var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                    for (int i = 0; i < model.ExperienceDetail.Count; i++)
                    {
                        if (model.ExperienceDetail[i].ExperienceFile != null)//...........Experience Certificates
                        {
                            string filename = System.IO.Path.GetFileName(model.ExperienceDetail[i].ExperienceFile.FileName);
                            string extension = Path.GetExtension(filename);
                            if (!allowedExtensionsCer.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type certificate [.jpeg,.png,.jpg,.gif,.pdf]";
                                return View(model);
                            }
                            if (model.ExperienceDetail[i].ExperienceFile.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload certificate up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }
                #endregion

                if (model.STEId > 0)
                {
                    if (model.aadharnumber != null)
                    {
                        var chkemployeeadhar = Common.CheckPreviousEmployeeAdharserver(Convert.ToString(model.aadharnumber), model.ApplicationNo, true, model.OldEmployeeNumber, "STE");
                        if (chkemployeeadhar != "")
                        {
                            TempData["alertMsg"] = chkemployeeadhar;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                    if (!string.IsNullOrEmpty(model.PAN))
                    {
                        var chkemployeepanno = Common.CheckPreviousEmployeePanserver(model.PAN, model.ApplicationNo, true, model.OldEmployeeNumber, "STE");
                        if (chkemployeepanno != "")
                        {
                            TempData["alertMsg"] = chkemployeepanno;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                }
                else
                {
                    if (model.aadharnumber != null)
                    {
                        var chkemployeeadhar = Common.CheckPreviousEmployeeAdharserver(Convert.ToString(model.aadharnumber), null, true, model.OldEmployeeNumber, "STE");
                        if (chkemployeeadhar != "")
                        {
                            TempData["errMsg"] = chkemployeeadhar;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                    if (!string.IsNullOrEmpty(model.PAN))
                    {
                        var chkemployeepanno = Common.CheckPreviousEmployeePanserver(model.PAN, null, true, model.OldEmployeeNumber, "STE");
                        if (chkemployeepanno != "")
                        {
                            TempData["errMsg"] = "This Pan Number is linked to  " + chkemployeepanno;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                }

                if (model.isDraftbtn == false && ModelState.IsValid)
                {
                    string validationMsg = ValidateSTEFormData(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        model.Status = model.Status == null ? "" : model.Status;
                        return View(model);
                    }
                    var result = recruitmentService.PostSTE(model, userId);
                    if (result.Item1 == 1)
                    {
                        TempData["succMsg"] = "Application submitted for approval";
                        return RedirectToAction("STEList", "Requirement");
                    }
                    else if (result.Item1 == 2)
                    {
                        TempData["succMsg"] = "STE application submitted for PI justification";
                        return RedirectToAction("STEList", "Requirement");
                    }
                    else if (result.Item1 == -1)
                    {
                        TempData["errMsg"] = result.Item3;
                        return RedirectToAction("STEList", "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("STEList", "Requirement");
                    }
                }
                else if (model.isDraftbtn == false && !ModelState.IsValid)
                {
                    string messages = string.Join("\n", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                    model.Status = model.Status == null ? "" : model.Status;
                    TempData["errMsg"] = messages;
                }
                else
                {
                    var result = recruitmentService.PostSTE(model, userId);//....Draft button....
                    if (result.Item1 == 1)
                    {
                        TempData["succMsg"] = "Draft Saved Successfully";
                        return RedirectToAction("STEList", "Requirement");
                    }
                    else if (result.Item1 == 2)
                    {
                        TempData["succMsg"] = "Draft updated";
                        return RedirectToAction("STEList", "Requirement");
                    }
                    else if (result.Item1 == -1)
                    {
                        TempData["errMsg"] = result.Item3;
                        return RedirectToAction("STEList", "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("STEList", "Requirement");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Apptype = Common.GetCodeControlList("STEAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                #region UpdateDisiplineList
                if (model.EducationDetail.Count > 0)
                {
                    foreach (var item in model.EducationDetail)
                        item.DisiplineList = Common.GetCourseList(item.QualificationId ?? 0);
                }
                #endregion
                model.Status = model.Status == null ? "" : model.Status;
                TempData["errMsg"] = "Something went wrong please contact administrator";
                return View(model);
            }
        }




        public ActionResult OSGNewView(int OSGID, string listf = null)

        {
            STEModel model = new STEModel();
            try
            {
                ViewBag.List = new List<MasterlistviewModel>();
                model.ApplicationEntryDate = DateTime.Now;
                model.Status = string.Empty;
                model.Medical = 2;
                ViewBag.Apptype = Common.GetCodeControlList("STEAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                if (OSGID > 0)
                {
                    model = recruitmentService.GetEditOSG(OSGID);
                    if (model.DateofBirth != null)
                        ViewBag.Years = Common.getRequirementyear(model.DateofBirth.Value.Year);
                    if (model.FlowApprover == "CMAdmin")
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "OSGAdminFlow", 0);
                    else if (model.FlowApprover == "NDean")
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "OSGFlowDean", 0);
                    else
                        ViewBag.processGuideLineId = Common.GetProcessGuidelineId(194, "Outsourcing Flow", 0);
                }
                else if (OSGID == 0)
                {
                    // model = Common.GetWFEditSTE(listf);
                    var listcommitte = Common.GetCommittee();
                    if (listcommitte.Item1.Count > 0)
                    {
                        for (int i = 0; i < listcommitte.Item1.Count; i++)
                        {
                            if (i == 0)
                            {
                                model.CommiteeMemberId1 = listcommitte.Item1[i].id ?? 0;
                                model.CommiteeMember1 = listcommitte.Item1[i].name;
                            }
                            if (i == 1)
                            {
                                model.CommiteeMemberId2 = listcommitte.Item1[i].id ?? 0;
                                model.CommiteeMember2 = listcommitte.Item1[i].name;
                            }
                        }
                        var datacharperson = Common.GetChairPerson();
                        model.ChairpersonNameId = datacharperson.Item1;
                        model.ChairpersonName = datacharperson.Item2;
                    }
                }
                else
                {
                    var listcommitte = Common.GetCommittee();
                    if (listcommitte.Item1.Count > 0)
                    {
                        for (int i = 0; i < listcommitte.Item1.Count; i++)
                        {
                            if (i == 0)
                            {
                                model.CommiteeMemberId1 = listcommitte.Item1[i].id ?? 0;
                                model.CommiteeMember1 = listcommitte.Item1[i].name;
                            }
                            if (i == 1)
                            {
                                model.CommiteeMemberId2 = listcommitte.Item1[i].id ?? 0;
                                model.CommiteeMember2 = listcommitte.Item1[i].name;
                            }
                        }
                        var datacharperson = Common.GetChairPerson();
                        model.ChairpersonNameId = datacharperson.Item1;
                        model.ChairpersonName = datacharperson.Item2;
                    }
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(191, "", 0);
                }
                model.isDraftbtn = false;
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Apptype = Common.GetCodeControlList("STEAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.processGuideLineId = Common.GetProcessGuidelineId(191, "", 0);
                if (model.DateofBirth != null)
                    ViewBag.Years = Common.getRequirementyear(model.DateofBirth.Value.Year);
                else
                    ViewBag.Years = Common.getRequirementyear();
                WriteLog.SendErrorToText(ex);
                return View(model);
            }

        }

        [HttpPost]
        public ActionResult OSGNewView(STEModel model)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Apptype = Common.GetCodeControlList("STEAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                if (model.EducationDetail.Count > 0)
                {
                    foreach (var item in model.EducationDetail)
                        item.DisiplineList = Common.GetCourseList(item.QualificationId ?? 0);
                }

                #region FileValidation
                if (model.PersonImage != null)
                {
                    var allowedExtensions = new[] { ".jpeg", ".png", ".jpg", ".gif" };
                    var extension = Path.GetExtension(model.PersonImage.FileName);
                    if (!allowedExtensions.Contains(extension.ToLower()))
                    {
                        TempData["alertMsg"] = "Please upload any one of these type candidate image [.jpeg,.png,.jpg]";
                        return View(model);
                    }
                    if (model.PersonImage.ContentLength > 1000000)
                    {
                        TempData["alertMsg"] = "You can upload candidate image up to 1MB";
                        return View(model);
                    }
                }

                if (model.Resume != null)
                {
                    var extension = Path.GetExtension(model.Resume.FileName);
                    if (extension.ToLower() != ".pdf")
                    {
                        TempData["alertMsg"] = "Please upload any one of these type Resume [.pdf]";
                        return View(model);
                    }
                    if (model.Resume.ContentLength > 5242880)
                    {
                        TempData["alertMsg"] = "You can upload Resume up to 5MB";
                        return View(model);
                    }
                }

                if (model.PIJustificationFile != null)
                {
                    var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
                    for (int i = 0; i < model.PIJustificationFile.Count(); i++)
                    {
                        if (model.PIJustificationFile[i] != null)
                        {
                            string extension = Path.GetExtension(model.PIJustificationFile[i].FileName);
                            if (!allowedExtensions.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type PI judtification document [.doc,.docx,.pdf]";
                                return View(model);
                            }
                            if (model.PIJustificationFile[i].ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload PI judtification document up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }

                if (model.EducationDetail != null)
                {
                    var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                    for (int i = 0; i < model.EducationDetail.Count; i++)
                    {
                        if (model.EducationDetail[i].Certificate != null)
                        {
                            string extension = Path.GetExtension(model.EducationDetail[i].Certificate.FileName);
                            if (!allowedExtensionsCer.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type certificate [.jpeg,.png,.jpg,.gif,.pdf]";
                                return View(model);
                            }
                            if (model.EducationDetail[i].Certificate.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload certificate up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }

                if (model.ExperienceDetail != null)
                {
                    var allowedExtensionsCer = new[] { ".jpeg", ".png", ".jpg", ".gif", ".pdf" };
                    for (int i = 0; i < model.ExperienceDetail.Count; i++)
                    {
                        if (model.ExperienceDetail[i].ExperienceFile != null)//...........Experience Certificates
                        {
                            string filename = System.IO.Path.GetFileName(model.ExperienceDetail[i].ExperienceFile.FileName);
                            string extension = Path.GetExtension(filename);
                            if (!allowedExtensionsCer.Contains(extension.ToLower()))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type certificate [.jpeg,.png,.jpg,.gif,.pdf]";
                                return View(model);
                            }
                            if (model.ExperienceDetail[i].ExperienceFile.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload certificate up to 5MB";
                                return View(model);
                            }
                        }
                    }
                }
                #endregion

                if (model.STEId > 0)
                {
                    if (model.aadharnumber != null)
                    {
                        var chkemployeeadhar = Common.CheckPreviousEmployeeAdharserver(Convert.ToString(model.aadharnumber), model.ApplicationNo, true, model.OldEmployeeNumber, "OSG");
                        if (chkemployeeadhar != "")
                        {
                            TempData["alertMsg"] = chkemployeeadhar;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                    if (!string.IsNullOrEmpty(model.PAN))
                    {
                        var chkemployeepanno = Common.CheckPreviousEmployeePanserver(model.PAN, model.ApplicationNo, true, model.OldEmployeeNumber, "OSG");
                        if (chkemployeepanno != "")
                        {
                            TempData["alertMsg"] = chkemployeepanno;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                }
                else
                {
                    if (model.aadharnumber != null)
                    {
                        var chkemployeeadhar = Common.CheckPreviousEmployeeAdharserver(Convert.ToString(model.aadharnumber), null, true, model.OldEmployeeNumber, "OSG");
                        if (chkemployeeadhar != "")
                        {
                            TempData["errMsg"] = chkemployeeadhar;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                    if (!string.IsNullOrEmpty(model.PAN))
                    {
                        var chkemployeepanno = Common.CheckPreviousEmployeePanserver(model.PAN, null, true, model.OldEmployeeNumber, "OSG");
                        if (chkemployeepanno != "")
                        {
                            TempData["errMsg"] = "This Pan Number is linked to  " + chkemployeepanno;
                            model.Status = model.Status == null ? "" : model.Status;
                            return View(model);
                        }
                    }
                }

                if (model.isDraftbtn == false && ModelState.IsValid)
                {
                    string validationMsg = ValidateSTEFormData(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        model.Status = model.Status == null ? "" : model.Status;
                        return View(model);
                    }
                    var result = recruitmentService.PostSTE(model, userId);
                    if (result.Item1 == 1)
                    {
                        TempData["succMsg"] = "Application submitted for approval";
                        return RedirectToAction("OutsourcingList", "Requirement");
                    }
                    else if (result.Item1 == 2)
                    {
                        TempData["succMsg"] = "OSG application submitted for PI justification";
                        return RedirectToAction("OutsourcingList", "Requirement");
                    }
                    else if (result.Item1 == -1)
                    {
                        TempData["errMsg"] = result.Item3;
                        return RedirectToAction("OutsourcingList", "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("OutsourcingList", "Requirement");
                    }
                }
                else if (model.isDraftbtn == false && !ModelState.IsValid)
                {
                    string messages = string.Join("\n", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                    model.Status = model.Status == null ? "" : model.Status;
                    TempData["errMsg"] = messages;
                }
                else
                {
                    var result = recruitmentService.PostSTE(model, userId);//....Draft button....
                    if (result.Item1 == 1)
                    {
                        TempData["succMsg"] = "Draft Saved Successfully";
                        return RedirectToAction("OutsourcingList", "Requirement");
                    }
                    else if (result.Item1 == 2)
                    {
                        TempData["succMsg"] = "Draft updated";
                        return RedirectToAction("OutsourcingList", "Requirement");
                    }
                    else if (result.Item1 == -1)
                    {
                        TempData["errMsg"] = result.Item3;
                        return RedirectToAction("OutsourcingList", "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("OutsourcingList", "Requirement");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.Apptype = Common.GetCodeControlList("STEAppointmenttype");
                ViewBag.Caste = Common.GetCodeControlList("STECaste");
                ViewBag.Nationality = Common.GetCodeControlList("Nationality");
                ViewBag.IITMPensionerOrCSIRStaff = Common.GetCodeControlList("IITMPensioner/CSIRStaff");
                ViewBag.CSIRStaffPayMode = Common.GetCodeControlList("STECSIRStaffPayMode");
                ViewBag.Medical = Common.GetCodeControlList("SETMedical");
                ViewBag.BloodGroup = Common.GetCodeControlList("SETBloodGroup");
                ViewBag.BloodGroupRH = Common.GetCodeControlList("SETBloodGroupRH");
                ViewBag.Professional = Common.GetCodeControlList("RCTProfessional");
                ViewBag.formtype = Common.GetCodeControlList("RCTFormtype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Exptype = Common.GetCodeControlList("RCTExperienceType");
                ViewBag.Gender = Common.GetCodeControlList("RCTGender");
                ViewBag.MarkType = Common.GetCodeControlList("RCTMarkType");
                ViewBag.YesNo = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Years = Common.getRequirementyear();
                ViewBag.MsPhd = Common.GetCodeControlList("MsPhd");
                #region UpdateDisiplineList
                if (model.EducationDetail.Count > 0)
                {
                    foreach (var item in model.EducationDetail)
                        item.DisiplineList = Common.GetCourseList(item.QualificationId ?? 0);
                }
                #endregion
                model.Status = model.Status == null ? "" : model.Status;
                TempData["errMsg"] = "Something went wrong please contact administrator";
                return View(model);
            }
        }





        //public ActionResult STENewView(int STEID = 0, string listf = null)
        //{
        //    try
        //    {
        //        STEViewModel model = new STEViewModel();
        //        model = recruitmentService.GetSTEView(STEID);
        //        var user = Common.getUserIdAndRole(User.Identity.Name);
        //        model.RoleId = user.Item2;
        //        if (model.FlowApprover == "CMAdmin")
        //            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STEAdminFlow", 0);
        //        else if (model.FlowApprover == "NDean")
        //            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STEFlowDean", 0);
        //        else
        //            ViewBag.processGuideLineId = Common.GetProcessGuidelineId(192, "STE Flow", 0);
        //        model.List_f = getEmployeeActionLink("STE", listf);
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        STEViewModel model = new STEViewModel();
        //        WriteLog.SendErrorToText(ex);
        //        return View(model);
        //    }
        //}

        [HttpPost]
        public JsonResult CheckDeviationQualfication1(CheckDevationModel model)
        {
            List<CheckListModel> listmodel = new List<CheckListModel>();
            List<CheckListEmailModel> checkdetails = new List<CheckListEmailModel>();
            NotePIModel emailModel = new NotePIModel();
            EmailBuilder _eb = new EmailBuilder();
            string username = User.Identity.Name;
            var user = Common.getUserIdAndRole(username);
            var Data = Common.GetDeviationofAppointments(model);
            listmodel = Data.Item1;
            if (listmodel.Count > 0)
            {
                for (int i = 0; i < listmodel.Count; i++)
                {
                    var sno = i + 1;
                    checkdetails.Add(new CheckListEmailModel()
                    {
                        CheckList = listmodel[i].CheckList,
                        checklistId = listmodel[i].FunctionCheckListId
                    });
                }
            }
            model.devChecklist = checkdetails;
            using (var context = new IOASDBEntities())
            {
                string designation = context.tblRCTDesignation.FirstOrDefault(m => m.DesignationId == model.DesignationId).Designation;
                emailModel.AppointmentType = model.AppointmentType;
                emailModel.checkdetails = RCTEmailContentService.getDevNormsDetails(model);
                emailModel.DesignationName = designation;
                emailModel.PersonName = model.PersonName;
                emailModel.Comments = model.Comments;
                emailModel.DAName = Common.GetUserNameBasedonId(user.Item1);
                emailModel.BasicPay = Convert.ToString(model.ChekSalary);
                emailModel.IsDeviation = true;
                emailModel.SendSlryStruct = model.SendSalaryStructure;
                int ProjectID = model.ProjectID ?? 0;
                var qryProject = (from prj in context.tblProject
                                  where prj.ProjectId == ProjectID
                                  select prj).FirstOrDefault();
                if (qryProject != null)
                {
                    emailModel.ProjectNumber = qryProject.ProjectNumber;
                    emailModel.ProjectTitle = qryProject.ProjectTitle;
                }
            }
            var loadEmialView = Tuple.Create(false, "", "");
            if (model.AppType == "STE")
                loadEmialView = _eb.RunCompile("RCTSTEDevTemplate.cshtml", "", emailModel, typeof(NotePIModel));
            else if (model.AppType == "OSG")
                loadEmialView = _eb.RunCompile("RCTOSGApplicationack.cshtml", "", emailModel, typeof(NotePIModel));
            else
                loadEmialView = _eb.RunCompile("NotePIProcess.cshtml", "", emailModel, typeof(NotePIModel));

            var result = new { output = ConvertViewToString("_DeviationCheckListDetail", listmodel), isRes = Data.Item2, template = loadEmialView.Item2 };
            return Json(result, JsonRequestBehavior.AllowGet);
        }




        #region Consultant New Appointment
        [HttpGet]
        public ActionResult RCTConsultantNewEntry(HttpPostedFileBase[] file, int ConsultantServiceID = 0)
        {
            ConsultantEmployeeEntry ConsultantModel = new ConsultantEmployeeEntry();
            try
            {
                ViewBag.ConsultantGSTType = Common.GetCodeControlList("ConsultantGSTType");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.Currency = Common.getCurrency(true);
                ViewBag.ConsultantCommonTaxType1 = Common.GetCodeControlList("ConsultantCommonTaxType1");
                ViewBag.ConsultantCommonTaxType2 = Common.GetCodeControlList("ConsultantCommonTaxType2");
                ViewBag.ConsultantRCMTaxType = Common.GetCodeControlList("ConsultantRCMTaxType");
                ViewBag.TDSSectionList = Common.GetTdsList();
                ViewBag.ConsultantMode = Common.GetCodeControlList("ConsultantMode");
                ViewBag.ConsultantPaymentType = Common.GetCodeControlList("ConsultantPaymentType");
                ViewBag.ConsultantDocType = Common.GetCodeControlList("ConsultantDocType");
                ConsultantModel.Consultant_Status = string.Empty;
                //ConsultantModel.Consultant_Code = string.Empty;
                if (ConsultantServiceID > 0)
                {
                    ConsultantModel = recruitmentService.GetEditConsultant(ConsultantServiceID, file);
                }
                return View(ConsultantModel);
            }
            catch (Exception ex)
            {
                ViewBag.ConsultantGSTType = Common.GetCodeControlList("ConsultantGSTType");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.Currency = Common.getCurrency(true);
                ViewBag.ConsultantCommonTaxType1 = Common.GetCodeControlList("ConsultantCommonTaxType1");
                ViewBag.ConsultantCommonTaxType2 = Common.GetCodeControlList("ConsultantCommonTaxType2");
                ViewBag.ConsultantRCMTaxType = Common.GetCodeControlList("ConsultantRCMTaxType");
                ViewBag.TDSSectionList = Common.GetTdsList();
                ViewBag.ConsultantMode = Common.GetCodeControlList("ConsultantMode");
                ViewBag.ConsultantPaymentType = Common.GetCodeControlList("ConsultantPaymentType");
                ViewBag.ConsultantDocType = Common.GetCodeControlList("ConsultantDocType");
                ConsultantModel.Consultant_Status = string.Empty;
                return View(ConsultantModel);
            }
        }

        [HttpPost]
        public ActionResult RCTConsultantNewEntry(ConsultantEmployeeEntry model, HttpPostedFileBase[] file)
        {
            try
            {
                int UserId = Common.GetUserid(User.Identity.Name);
                string folder = System.Web.HttpContext.Current.Server.MapPath("~/Content/Requirement");
                string Errormessages = string.Empty;
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.ConsultantGSTType = Common.GetCodeControlList("ConsultantGSTType");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.Currency = Common.getCurrency(true);
                ViewBag.ConsultantCommonTaxType1 = Common.GetCodeControlList("ConsultantCommonTaxType1");
                ViewBag.ConsultantCommonTaxType2 = Common.GetCodeControlList("ConsultantCommonTaxType2");
                ViewBag.ConsultantRCMTaxType = Common.GetCodeControlList("ConsultantRCMTaxType");
                ViewBag.TDSSectionList = Common.GetTdsList();
                ViewBag.ConsultantMode = Common.GetCodeControlList("ConsultantMode");
                ViewBag.ConsultantPaymentType = Common.GetCodeControlList("ConsultantPaymentType");
                ViewBag.ConsultantDocType = Common.GetCodeControlList("ConsultantDocType");
                if (model.Consultant_Status == "Draft")
                {
                    var result = recruitmentService.PostConsultant(model, file, UserId);
                    if (result.Item1 == 1)
                    {
                        TempData["succMsg"] = "Appointment Entry submitted Successfully";
                        return RedirectToAction("RCTConsultantList", "Requirement");
                    }
                    else if (result.Item1 == 2)
                    {
                        TempData["succMsg"] = "Saved as draft";
                        return RedirectToAction("RCTConsultantList", "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("RCTConsultantList", "Requirement");
                    }
                }
                else if (model.Consultant_Status == "Open")
                {
                    var result = recruitmentService.PostConsultant(model, file, UserId);
                    if (result.Item1 == 1)
                    {
                        TempData["succMsg"] = "Appointment Entry submitted Successfully";
                        return RedirectToAction("RCTConsultantList", "Requirement");
                    }
                    else if (result.Item1 == 2)
                    {
                        TempData["succMsg"] = "Saved as draft";
                        return RedirectToAction("RCTConsultantList", "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("RCTConsultantList", "Requirement");
                    }
                }
                else if (model.Consultant_Status == "Awaiting Verification")
                {
                    var result = recruitmentService.PostConsultant(model, file, UserId);
                    if (result.Item1 == 3)
                    {
                        TempData["succMsg"] = "Verification submitted for commitment update";
                        return RedirectToAction("RCTConsultantList", "Requirement");
                    }
                    else if (result.Item1 == 4)
                    {
                        TempData["succMsg"] = "Verification Completed";
                        return RedirectToAction("RCTConsultantList", "Requirement");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("RCTConsultantList", "Requirement");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.ConsultantGSTType = Common.GetCodeControlList("ConsultantGSTType");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.Currency = Common.getCurrency(true);
                ViewBag.ConsultantCommonTaxType1 = Common.GetCodeControlList("ConsultantCommonTaxType1");
                ViewBag.ConsultantCommonTaxType2 = Common.GetCodeControlList("ConsultantCommonTaxType2");
                ViewBag.ConsultantRCMTaxType = Common.GetCodeControlList("ConsultantRCMTaxType");
                ViewBag.TDSSectionList = Common.GetTdsList();
                ViewBag.ConsultantMode = Common.GetCodeControlList("ConsultantMode");
                ViewBag.ConsultantPaymentType = Common.GetCodeControlList("ConsultantPaymentType");
                ViewBag.ConsultantDocType = Common.GetCodeControlList("ConsultantDocType");
                TempData["errMsg"] = "Something went wrong please contact administrator";
                return RedirectToAction("RCTConsultantList", "Requirement");
            }
            return RedirectToAction("RCTConsultantList", "Requirement");
        }
        [HttpGet]
        public JsonResult LoadRCTCONEmployeeList(string term, string apptype = null)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteRCTCONEmployee(term, "CON");
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetConsEmpDetails(string ConsEmpID = null)
        {
            try
            {
                object output = Common.GetConsEmployeeDetails(ConsEmpID);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public ActionResult RCTConsultantList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetRCTConsultantList(RCTConsultantSearchModel model, int pageIndex, int pageSize)
        {
            try
            {
                object output = RequirementService.GetRCTConsultantList(model, pageIndex, pageSize);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult RCTConsultantEMPMasterList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetRCTConsultantEMPMasterList(RCTConsultantSearchModel model, int pageIndex, int pageSize)
        {
            try
            {
                object output = RequirementService.GetRCTConsultantList(model, pageIndex, pageSize);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                throw ex;
            }
        }

        public ActionResult _ConsultantMasterView(int Vendorid = 0)
        {
            try
            {
                ConsultantMasterView model = new ConsultantMasterView();
                model = RequirementService.GetConsultantMasterView(Vendorid);
                ViewBag.processGuideLineId = 160;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult RCTConsultantNewEntryView(HttpPostedFileBase[] file, int ConsultantServiceID = 0)
        {
            ConsultantEmployeeEntry model = new ConsultantEmployeeEntry();
            try
            {
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.ConsultantGSTType = Common.GetCodeControlList("ConsultantGSTType");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.Currency = Common.getCurrency(true);
                ViewBag.ConsultantCommonTaxType1 = Common.GetCodeControlList("ConsultantCommonTaxType1");
                ViewBag.ConsultantCommonTaxType2 = Common.GetCodeControlList("ConsultantCommonTaxType2");
                ViewBag.ConsultantRCMTaxType = Common.GetCodeControlList("ConsultantRCMTaxType");
                ViewBag.TDSSectionList = Common.GetTdsList();
                ViewBag.ConsultantMode = Common.GetCodeControlList("ConsultantMode");
                ViewBag.ConsultantPaymentType = Common.GetCodeControlList("ConsultantPaymentType");
                ViewBag.ConsultantDocType = Common.GetCodeControlList("ConsultantDocType");
                model = recruitmentService.GetRCTConsultantNewEntryView(ConsultantServiceID, file);
                if (model.Consultant_FlowApprover == "CMAdmin")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(241, "ConsAdminFlow", 0);
                else if (model.Consultant_FlowApprover == "NDean")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(241, "ConsDeanFlow", 0);
                else
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(241, "ConsHRFlow", 0);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult _RCTConsultantNewEntryView(HttpPostedFileBase[] file, int ConsultantServiceID = 0)
        {
            ConsultantEmployeeEntry model = new ConsultantEmployeeEntry();
            try
            {
                ViewBag.List = new List<MasterlistviewModel>();
                ViewBag.ConsultantGSTType = Common.GetCodeControlList("ConsultantGSTType");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.Currency = Common.getCurrency(true);
                ViewBag.ConsultantCommonTaxType1 = Common.GetCodeControlList("ConsultantCommonTaxType1");
                ViewBag.ConsultantCommonTaxType2 = Common.GetCodeControlList("ConsultantCommonTaxType2");
                ViewBag.ConsultantRCMTaxType = Common.GetCodeControlList("ConsultantRCMTaxType");
                ViewBag.TDSSectionList = Common.GetTdsList();
                ViewBag.ConsultantMode = Common.GetCodeControlList("ConsultantMode");
                ViewBag.ConsultantPaymentType = Common.GetCodeControlList("ConsultantPaymentType");
                ViewBag.ConsultantDocType = Common.GetCodeControlList("ConsultantDocType");
                model = recruitmentService.GetRCTConsultantNewEntryView(ConsultantServiceID, file);
                if (model.Consultant_FlowApprover == "CMAdmin")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(241, "ConsAdminFlow", 0);
                else if (model.Consultant_FlowApprover == "NDean")
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(241, "ConsDeanFlow", 0);
                else
                    ViewBag.processGuideLineId = Common.GetProcessGuidelineId(241, "ConsHRFlow", 0);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public string RCTConsultantNewEntryInit(int ConsultantServiceID = 0, string status = "false")
        {
            ConsultantEmployeeEntry model = new ConsultantEmployeeEntry();
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                var sts = recruitmentService.CONSWFInit(ConsultantServiceID, userId);
                string retmsg = "";
                if (sts.Item1 == true && (sts.Item2 == null || sts.Item2 == ""))
                {
                    retmsg = "Application submitted for approval";
                }
                else if (sts.Item2 != null)
                {
                    retmsg = sts.Item2;
                }
                return retmsg;
            }
            catch (Exception ex)
            {
                WriteLog.SendErrorToText(ex);
                return ex.Message;
            }
        }
        public JsonResult RCTGetTDSpercentage(String type = "")
        {
            try
            {
                object output = RequirementService.getRCTConsTDSdetail(type);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        public ActionResult ShowDocument(string file, string filepath)
        {
            try
            {

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



        #region Test
        //public JsonResult GetServicebarExperirence()
        //{
        //    decimal iitmExp = RequirementService.IITExperience(34260,2,"IC34809");
        //    return Json(iitmExp, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult CheckEMPExp(int id)
        //{
        //    string exp=Common.getExperienceInWordings(id, "STE");
        //    return View(exp);
        //}
        //InCase recuirment commitmentbooked not changed status run this service
        //public JsonResult UpdateSteCommitmentRequesttable()
        //{
        //    STEViewModel model = new STEViewModel();
        //    RecruitCommitRequestModel reqmodel = new RecruitCommitRequestModel();
        //    reqmodel.CommitmentRequestId = 17854;
        //    reqmodel.AllocationHeadId = 34;
        //    reqmodel.CommitmentAmount = 240000;
        //    int logged_in_userId = 119;
        //    int commitmentId = 195640;
        //    model.CommitReqModel = reqmodel;
        //    int status = recruitmentService.UpdateCommitDetails(model, commitmentId, logged_in_userId);
        //    return Json(status, JsonRequestBehavior.AllowGet);
        //}
        //InCase Extension Process not Updated in Commitment Requesttable Run this service
        //public JsonResult UpdateExtensionCommitmentRequesttable()
        //{
        //    STEViewModel model = new STEViewModel();
        //    RecruitCommitRequestModel reqmodel = new RecruitCommitRequestModel();
        //    reqmodel.CommitmentRequestId = 18476;
        //    int commitmentid = 195532;
        //    reqmodel.CommitmentBookedId = 18172;
        //    reqmodel.AllocationHeadId = 34;
        //    reqmodel.AddCommitmentAmount = 420000;
        //    int logged_in_userId = 119;
        //    model.CommitReqModel = reqmodel;
        //    int status=recruitmentService.UpdateAddCommitDetails(model, commitmentid, logged_in_userId);
        //    return Json(status, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult UpdateCommitmentinRCTRequesttable()
        //{
        //    bool status = recruitmentService.UpdateEnhancementDetails(11646, 201);
        //    return Json(status, JsonRequestBehavior.AllowGet);
        //}

        #endregion
    }
}
  