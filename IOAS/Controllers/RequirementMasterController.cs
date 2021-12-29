using IOAS.Filter;
using IOAS.GenericServices;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.IO;
using System.Data;
using ClosedXML.Excel;

namespace IOAS.Controllers
{
    [Authorized]
    public class RequirementMasterController : Controller
    {
        // GET: RequirementMaster
        #region Designation
        public ActionResult DesignationList()
        {

            return View();
        }
        [HttpGet]
        public ActionResult DesignationMaster(int designationId = 0)
        {
            DesignationModel model = new DesignationModel();
            try
            {
                ViewBag.Apptype = Common.GetCodeControlList("Appointmenttype");
                ViewBag.Dept = Common.GetDepartment();
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Relevant = Common.GetCodeControlList("RelevantExperienceType");
                ViewBag.Status = Common.GetCodeControlList("DesignationStatus");
                ViewBag.cgpatype = Common.GetCodeControlList("CGPAType");
                ViewBag.course = new List<MasterlistviewModel>();
                ViewBag.SalaryLevel = Common.GetSalaryLevelList(model.TypeOfAppointment);
                if (designationId > 0)
                {
                    model = RequirementMasterService.EditDesignation(designationId);
                }
                //model.MedicalDeduction= Convert.ToDecimal(WebConfigurationManager.AppSettings["MedicalAmt"]);
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult DesignationMaster(DesignationModel model)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                ViewBag.Apptype = Common.GetCodeControlList("Appointmenttype");
                ViewBag.Dept = Common.GetDepartment();
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Relevant = Common.GetCodeControlList("RelevantExperienceType");
                ViewBag.Status = Common.GetCodeControlList("DesignationStatus");
                ViewBag.cgpatype = Common.GetCodeControlList("CGPAType");
                ViewBag.course = new List<MasterlistviewModel>();
                ViewBag.SalaryLevel = Common.GetSalaryLevelList(model.TypeOfAppointment);
                if (ModelState.IsValid)
                {
                    model.UserId = Common.GetUserid(user_logged_in);
                    int Status = RequirementMasterService.CreateDesignation(model);
                    if (model.DesignationId == null && Status == 1)
                    {
                        TempData["succMsg"] = "Designation has been added successfully.";
                        return RedirectToAction("DesignationList");
                    }
                    else if (model.DesignationId > 0 && Status == 2)
                    {
                        TempData["succMsg"] = "Designation has been Updated successfully.";
                        return RedirectToAction("DesignationList");
                    }
                    else if (model.DesignationId == null && Status == 3)
                    {
                        ViewBag.alertMsg = "Designation Code already exists.";
                        //return View(model);
                    }
                    else if (model.DesignationId == null && Status == 4)
                    {
                        ViewBag.alertMsg = "Designation name already exists.";
                        //return View(model);
                    }
                    else
                    {
                        ViewBag.errMsg = "Something went wrong please contact administrator." + "<br/>" + model.ErrorMsg;
                        //return RedirectToAction("DesignationList");
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                if (model.Detail.Count > 0)
                {
                    foreach (var item in model.Detail)
                    {
                        item.ddlList = Common.GetCourseList(item.Qualification ?? 0);
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Apptype = Common.GetCodeControlList("Appointmenttype");
                ViewBag.Dept = Common.GetDepartment();
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.marks = Common.GetCodeControlList("Markstype");
                ViewBag.Relevant = Common.GetCodeControlList("RelevantExperienceType");
                ViewBag.Status = Common.GetCodeControlList("DesignationStatus");
                ViewBag.cgpatype = Common.GetCodeControlList("CGPAType");
                ViewBag.SalaryLevel = Common.GetSalaryLevelList(model.TypeOfAppointment);
                TempData["errMsg"] = "Something went wrong please contact administrator." + "<br/>" + ex.Message;
                if (model.Detail.Count > 0)
                {
                    List<DesignationDetailModel> list = new List<DesignationDetailModel>();
                    for (int i = 0; i < model.Detail.Count; i++)
                    {
                        int ddid = model.Detail[i].DesignationDetailId ?? 0;
                        int qulid = model.Detail[i].Qualification ?? 0;
                        List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                        datalist = Common.GetCourseList(qulid);
                        list.Add(new DesignationDetailModel()
                        {
                            QualificationCourse = model.Detail[i].QualificationCourse,
                            ddlList = datalist,

                        });
                    }
                    model.Detail = list;
                }
                return View(model);
            }
        }
        [HttpPost]
        public JsonResult GetDesignationList(SearchdesignationModel model, int pageIndex, int pageSize)
        {
            try
            {
                object output = RequirementMasterService.GetDesignationList(model, pageIndex, pageSize);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPIByCourseList(int DepartmentId)
        {
            var locationdata = Common.GetCourseList(DepartmentId);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult LoadDesignationList(string term)
        {
            try
            {
                var data = Common.GetDesigantionList(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Statutory
        public ActionResult StatutoryMasterList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetStatutoryList(SearchStatutoryModel model, int pageIndex, int pageSize, DateFilterModel ValueDateType, DateFilterModel EndDateType)
        {
            try
            {
                object output = RequirementMasterService.GetStatutoryList(model, pageIndex, pageSize, ValueDateType, EndDateType);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult StatutoryMaster()
        {
            return View();
        }
        public ActionResult StatutoryMasterView(int statutoryId)
        {
            StatutoryModel model = new StatutoryModel();
            model = RequirementMasterService.ViewStatutory(statutoryId);
            return View(model);
        }
        [HttpPost]
        public ActionResult StatutoryMaster(StatutoryModel model)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                if (ModelState.IsValid)
                {
                    model.UserId = Common.GetUserid(user_logged_in);
                    int Status = RequirementMasterService.CreateStatutory(model);
                    if (model.StatutoryId == null && Status == 1)
                    {
                        TempData["succMsg"] = "Statutory has been added successfully.";
                        return RedirectToAction("StatutoryMasterList");
                    }
                    else if (model.StatutoryId == null && Status == 3)
                    {
                        TempData["alertMsg"] = "Record already exist for the current period.";
                        return RedirectToAction("StatutoryMasterList");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator.";
                        return RedirectToAction("StatutoryMasterList");
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
                return View(model);
            }
        }



        public ActionResult UpdateOSGSalaryComp(int statutoryId)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                if (ModelState.IsValid)
                {
                    int UserId = Common.GetUserid(user_logged_in);
                    int Status = RequirementMasterService.UpdateOSGSalaryComp(statutoryId, UserId);
                    if (statutoryId > 0 && Status == 1)
                    {
                        TempData["succMsg"] = "Outsourcing Employee Salary details has been updated.";
                        return RedirectToAction("StatutoryMasterList");
                    }
                    else if (statutoryId <= 0 && Status == 2)
                    {
                        TempData["alertMsg"] = "StatutoryId is not valid.";
                        return RedirectToAction("StatutoryMasterList");
                    }
                    else if (statutoryId > 0 && Status == 2)
                    {
                        TempData["alertMsg"] = "StatutoryId is not valid.";
                        return RedirectToAction("StatutoryMasterList");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator.";
                        return RedirectToAction("StatutoryMasterList");
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return RedirectToAction("StatutoryMasterList");
            }
            catch (Exception ex)
            {
                return RedirectToAction("StatutoryMasterList");
            }
        }
        #endregion

        #region ProfessionalTax
        public ActionResult ProfessionalTaxList()
        {

            return View();
        }
        public ActionResult ProfessionalTaxView(int professionalTaxId)
        {
            ProfessionalTaxModel model = new ProfessionalTaxModel();
            model = RequirementMasterService.ViewProftax(professionalTaxId);
            return View(model);
        }
        [HttpPost]
        public JsonResult GetProfessionaltaxList(SearchProfessionalTaxModel model, int pageIndex, int pageSize, DateFilterModel ValueDateType, DateFilterModel EndDateType)
        {
            try
            {
                object output = RequirementMasterService.GetProfessiontaxList(model, pageIndex, pageSize, ValueDateType, EndDateType);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult ProfessionalTaxEntry()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ProfessionalTaxEntry(ProfessionalTaxModel model)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                if (ModelState.IsValid)
                {
                    model.UserId = Common.GetUserid(user_logged_in);
                    int Status = RequirementMasterService.CreateProfessionalTax(model);
                    if (model.ProfessionalTaxId == null && Status == 1)
                    {
                        TempData["succMsg"] = "Professional Tax has been added successfully.";
                        return RedirectToAction("ProfessionalTaxList");
                    }
                    else if (model.ProfessionalTaxId > 0 && Status == 2)
                    {
                        TempData["succMsg"] = "Professional Tax has been Updated successfully.";
                        return RedirectToAction("ProfessionalTaxList");
                    }
                    else if (model.ProfessionalTaxId == null && Status == 3)
                    {
                        TempData["alertMsg"] = "Record already exist for the current period.";
                        return RedirectToAction("ProfessionalTaxList");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator.";
                        return RedirectToAction("ProfessionalTaxList");
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
                return View(model);
            }
        }
        #endregion

        #region MemberCreation
        public ActionResult MemberMaster(int MemberId=0)
        {
            MemberModel model = new MemberModel();
            try
            {
                ViewBag.member = Common.GetCodeControlList("MemberType");
                ViewBag.memberstatus = Common.GetCodeControlList("MemberStatus");
                if(MemberId>0)
                {
                    model = RequirementMasterService.EditMemberModel(MemberId);
                }
                return View(model);
            }
            catch(Exception ex)
            {
                return View(model);
            }
        }
        //[HttpPost]
        //public ActionResult MemberMaster(MemberModel model)
        //{
        //    ViewBag.member = Common.GetCodeControlList("MemberType");
        //    ViewBag.memberstatus = Common.GetCodeControlList("MemberStatus");
        //    string user_logged_in = User.Identity.Name;
        //    if (ModelState.IsValid)
        //    {
        //        model.UserId = Common.GetUserid(user_logged_in);
        //        int result = RequirementMasterService.CreateMember(model);
        //        if(model.MemberId==null&& result==1&&model.MemberType==1)
        //        {
        //            TempData["succMsg"] = "Member created successfully.";
        //            return RedirectToAction("MemberMasterList");
        //        }
        //        else if(model.MemberId == null && result == 1 && model.MemberType == 2)
        //        {
        //            TempData["succMsg"] = "Chairperson Created successfully.";
        //            return RedirectToAction("MemberMasterList");
        //        }
        //        else if(model.MemberId == null && result == 2)
        //        {
        //            TempData["alertMsg"] = "Cannot create a new member for the same period. Kindly deactivate a member before creating a new member";
        //            return RedirectToAction("MemberMasterList");
        //        }
        //        else if (model.MemberId == null && result == 3)
        //        {
        //            TempData["alertMsg"] = "Cannot create a new Chair Person for the same period. Kindly deactivate a Chair Person before creating a new Chair Person";
        //            return RedirectToAction("MemberMasterList");
        //        }
        //        else if (model.MemberId == null && result == 4)
        //        {
        //            TempData["alertMsg"] = "Record already exist";
        //            return RedirectToAction("MemberMasterList");
        //        }
        //        else if (model.MemberId!=null && result == 5&&model.MemberType==1)
        //        {
        //            TempData["succMsg"] = "Member removed";
        //            return RedirectToAction("MemberMasterList");
        //        }
        //        else if (model.MemberId != null && result == 5 && model.MemberType == 2)
        //        {
        //            TempData["succMsg"] = "chairperson removed";
        //            return RedirectToAction("MemberMasterList");
        //        }
        //        else if (model.MemberId != null && result == 6 && model.MemberType == 1)
        //        {
        //            TempData["succMsg"] = "Member Active";
        //            return RedirectToAction("MemberMasterList");
        //        }
        //        else if (model.MemberId != null && result == 6 && model.MemberType == 2)
        //        {
        //            TempData["succMsg"] = "chairperson Active";
        //            return RedirectToAction("MemberMasterList");
        //        }
        //        else
        //        {
        //            TempData["errMsg"] = "Something went wrong please contact administrator.";
        //            return RedirectToAction("MemberMasterList");
        //        }
        //    }
        //    else
        //    {
        //        string messages = string.Join("<br />", ModelState.Values
        //                            .SelectMany(x => x.Errors)
        //                            .Select(x => x.ErrorMessage));

        //        TempData["errMsg"] = messages;
        //    }
        //    return View(model);
        //}

        [HttpPost]
        public ActionResult MemberMaster(MemberModel model)
        {
            ViewBag.member = Common.GetCodeControlList("MemberType");
            ViewBag.memberstatus = Common.GetCodeControlList("MemberStatus");
            string user_logged_in = User.Identity.Name;
            if (ModelState.IsValid)
            {
                model.UserId = Common.GetUserid(user_logged_in);
                int result = RequirementMasterService.CreateMember(model);
                if (model.MemberId == null && result == 1 && model.MemberType == 1)
                {
                    TempData["succMsg"] = "Member created successfully.";
                    return RedirectToAction("MemberMasterList");
                }
                else if (model.MemberId == null && result == 1 && model.MemberType == 2)
                {
                    TempData["succMsg"] = "Chairperson Created successfully.";
                    return RedirectToAction("MemberMasterList");
                }
                else if (model.MemberId == null && result == 2)
                {
                    TempData["alertMsg"] = "Cannot create a new member for the same period. Kindly deactivate a member before creating a new member";
                    return RedirectToAction("MemberMasterList");
                }
                else if (model.MemberId == null && result == 3)
                {
                    TempData["alertMsg"] = "Cannot create a new Chair Person for the same period. Kindly deactivate a Chair Person before creating a new Chair Person";
                    return RedirectToAction("MemberMasterList");
                }
                else if (model.MemberId == null && result == 4)
                {
                    TempData["alertMsg"] = "Record already exist";
                    return RedirectToAction("MemberMasterList");
                }
                else if (model.MemberId != null && result == 5 && model.MemberType == 1)
                {
                    TempData["succMsg"] = "Member removed";
                    return RedirectToAction("MemberMasterList");
                }
                else if (model.MemberId != null && result == 5 && model.MemberType == 2)
                {
                    TempData["succMsg"] = "chairperson removed";
                    return RedirectToAction("MemberMasterList");
                }
                else if (model.MemberId != null && result == 6 && model.MemberType == 1)
                {
                    TempData["succMsg"] = "Member Active";
                    return RedirectToAction("MemberMasterList");
                }
                else if (model.MemberId != null && result == 6 && model.MemberType == 2)
                {
                    TempData["succMsg"] = "chairperson Active";
                    return RedirectToAction("MemberMasterList");
                }
                else if (model.MemberId != null && result == 6 && model.MemberType == 2)
                {
                    TempData["alertMsg"] = "Please inactive chairperson then try the same";
                    return RedirectToAction("MemberMasterList");
                }
                else if (model.MemberId != null && result == 6 && model.MemberType == 1)
                {
                    TempData["alertMsg"] = "Please inactive one member then try the same";
                    return RedirectToAction("MemberMasterList");
                }
                else if (result == 8)
                {
                    TempData["succMsg"] = "Update successfully";
                    return RedirectToAction("MemberMasterList");
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator.";
                    return RedirectToAction("MemberMasterList");
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

        public ActionResult MemberMasterList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetMemberList(SearchMemberModel model, int pageIndex, int pageSize, DateFilterModel FrmDate, DateFilterModel TostrDate)
        {
            try
            {
                object output = RequirementMasterService.GetMemberList(model, pageIndex, pageSize, FrmDate, TostrDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult MemberInActive(int MemberId)
        {
            string user_logged_in = User.Identity.Name;
            int UserId = Common.GetUserid(user_logged_in);
            object output = RequirementMasterService.InActiveMember(MemberId, UserId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MemberMasterView(int MemberId)
        {
            MemberViewModel model = new MemberViewModel();
            try
            {
                model = RequirementMasterService.GetMemberView(MemberId);
                return View(model);
            }
            catch(Exception ex)
            {
                return View(model);
            }
           
        }
        #endregion

        #region OutSourceing AgencyMaster
        public ActionResult Outsourceagencymaster(int SalaryId = 0)
        {
            AgencySalaryMasterModel model = new AgencySalaryMasterModel();
            try
            {
                ViewBag.Country = Common.getCountryList();
                ViewBag.State = Common.GetStatelist();
                if (SalaryId > 0)
                    model = RequirementMasterService.EditOutsourceagency(SalaryId);
                var frmfin = Common.GetRCTFinPeriod();
                model.FinFormDate = frmfin.Item1;
                model.FinToDate = frmfin.Item2;
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Country = Common.getCountryList();
                ViewBag.State = Common.GetStatelist();
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult Outsourceagencymaster(AgencySalaryMasterModel model)
        {
            try
            {
                ViewBag.Country = Common.getCountryList();
                ViewBag.State = Common.GetStatelist();
                if (ModelState.IsValid)
                {
                    string gstvalide = model.GSTIN.Substring(0, 2);
                    string statecode = Common.GetStateCode(model.StateId ?? 0);
                    if (gstvalide != statecode)
                    {
                        TempData["alertMsg"] = "The GST number and State not match";
                        return View(model);
                    }
                    foreach (var item in model.AgencyDocList)
                    {
                        if (item.Document != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".PDF", ".DOCX", ".docx" };
                            string filename = Path.GetFileName(item.Document.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["alertMsg"] = "Please upload any one of these type doc [.pdf,.docx]";
                                return View(model);
                            }
                            if (item.Document.ContentLength > 5242880)
                            {
                                TempData["alertMsg"] = "You can upload File up to 5 MB";
                                return View(model);
                            }
                        }
                    }
                    model.UserId = Common.GetUserid(User.Identity.Name);
                    var status = RequirementMasterService.CreateOutsourceagency(model);
                    if (model.SalaryAgencyId == null && status == 1)
                    {
                        TempData["succMsg"] = "Add Sucessfully";
                        return RedirectToAction("OutsourceAgencyMasterList", "RequirementMaster");
                    }
                    else if (model.SalaryAgencyId == null && status == 3)
                    {
                        TempData["alertMsg"] = "This GST Number Already Exists";
                        return View(model);
                    }
                    else if (model.SalaryAgencyId == null && status == 4)
                    {
                        TempData["alertMsg"] = "This PAN Number Already Exists";
                        return View(model);
                    }
                    else if (model.SalaryAgencyId == null && status == 5)
                    {
                        TempData["alertMsg"] = "Same Agency name and GST Number";
                        return View(model);
                    }
                    else if (model.SalaryAgencyId == null && status == 6)
                    {
                        TempData["alertMsg"] = "Same Agency name and PAN Number";
                        return View(model);
                    }
                    else if (model.SalaryAgencyId != null && status == 2)
                    {
                        TempData["succMsg"] = "Update Sucessfully";
                        return RedirectToAction("OutsourceAgencyMasterList", "RequirementMaster");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                        return RedirectToAction("OutsourceAgencyMasterList", "RequirementMaster");
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
                ViewBag.Country = Common.getCountryList();
                ViewBag.State = Common.GetStatelist();
                ErrorHandler WriteLog = new ErrorHandler();
                WriteLog.SendErrorToText(ex);
                return View(model);
            }
        }
        public ActionResult OutsourceAgencyMasterList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetOutsourceAgencyList(AgencySalaryMasterSearchModel model, int pageIndex, int pageSize, DateFilterModel ModifyDate)
        {
            try
            {
                string username = User.Identity.Name;
                var user = Common.getUserIdAndRole(username);
                int userid = user.Item1;
                int roleid = user.Item2;
                object output = RequirementMasterService.GetAgencySalarySearch(model, pageIndex, pageSize, ModifyDate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorHandler WriteLog = new ErrorHandler();
                WriteLog.SendErrorToText(ex);
                throw new Exception(ex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadSatecode(int StateId)
        {
            var statecode = Common.GetStateCode(StateId);
            var result = new { Statecode = statecode };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Amendment law

        public ActionResult Outsourceamendmentmaster()
        {
            try
            {
                ViewBag.AmendmentType = Common.getAmendmentType();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.AmendmentType = Common.getAmendmentType();
                return View();
            }
        }

        [HttpPost]
        public JsonResult AmendmentOSGSalary(int componentId)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                int UserId = Common.GetUserid(user_logged_in);
                int Status = RequirementMasterService.UpdateOSGSalaryComp(componentId, UserId);
                if (componentId > 0 && Status == 1)
                {
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Status, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(0, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult getOSGSalaryComparison(int componentId)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                CoreAccountsService coreAccountService = new CoreAccountsService();
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                var Data = RequirementMasterService.UpdateOSGSalaryList(componentId);
                dt = RequirementMasterService.ExportSalaryList(Data.Item1);
                dt1 = RequirementMasterService.ExportSalaryList(Data.Item2);
                //coreAccountService.toSpreadSheet(dt, "New Component");
                //coreAccountService.toSpreadSheet(dt1, "Old Component");
                if (dt != null && dt1 != null)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "inline;  filename=Data.xlsx");
                        wb.Worksheets.Add(dt, "New Component");
                        wb.Worksheets.Add(dt1, "Old Component");
                        wb.SaveAs(workStream);
                        workStream.Position = 0;
                    }
                }
                return new FileStreamResult(workStream, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                return RedirectToAction("StatutoryMasterList");
            }
        }

        #endregion

        #region SalaryLevel
        public ActionResult SalaryLevelList()
        {

            return View();
        }
        [HttpGet]
        public ActionResult SalaryLevelMaster(int SalaryLevelId = 0)
        {
            SalaryLevelModel model = new SalaryLevelModel();
            try
            {
                ViewBag.Apptype = Common.GetCodeControlList("Appointmenttype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Marks = Common.GetCodeControlList("Markstype");
                ViewBag.Experience = Common.GetCodeControlList("RelevantExperienceType");
                ViewBag.Status = Common.GetCodeControlList("DesignationStatus");
                ViewBag.CGPAType = Common.GetCodeControlList("CGPAType");
                ViewBag.Course = new List<MasterlistviewModel>();
                if (SalaryLevelId > 0)
                    model = RequirementMasterService.EditSalaryLevel(SalaryLevelId);
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult SalaryLevelMaster(SalaryLevelModel model)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                ViewBag.Apptype = Common.GetCodeControlList("Appointmenttype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Marks = Common.GetCodeControlList("Markstype");
                ViewBag.Experience = Common.GetCodeControlList("RelevantExperienceType");
                ViewBag.Status = Common.GetCodeControlList("DesignationStatus");
                ViewBag.CGPAType = Common.GetCodeControlList("CGPAType");
                ViewBag.Course = new List<MasterlistviewModel>();
                if (ModelState.IsValid)
                {
                    model.UserId = Common.GetUserid(user_logged_in);
                    int Status = RequirementMasterService.CreateSalaryLevel(model);
                    if (model.SalaryLevelId == null && Status == 1)
                    {
                        TempData["succMsg"] = "Salary level has been added successfully.";
                        return RedirectToAction("SalaryLevelList");
                    }
                    else if (model.SalaryLevelId > 0 && Status == 2)
                    {
                        TempData["succMsg"] = "Salary level has been Updated successfully.";
                        return RedirectToAction("SalaryLevelList");
                    }
                    else if (model.SalaryLevelId == null && Status == 3)
                        ViewBag.alertMsg = "Salary level Code already exists.";
                    else
                        ViewBag.errMsg = "Something went wrong please contact administrator.";
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                    TempData["errMsg"] = messages;
                }
                if (model.SalaryLevelDetail.Count > 0)
                {
                    foreach (var item in model.SalaryLevelDetail)
                        item.ddlList = Common.GetCourseList(item.Qualification ?? 0);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Apptype = Common.GetCodeControlList("Appointmenttype");
                ViewBag.Qualification = Common.GetQualificationList();
                ViewBag.Marks = Common.GetCodeControlList("Markstype");
                ViewBag.Experience = Common.GetCodeControlList("RelevantExperienceType");
                ViewBag.Status = Common.GetCodeControlList("DesignationStatus");
                ViewBag.CGPAType = Common.GetCodeControlList("CGPAType");
                ViewBag.Course = new List<MasterlistviewModel>();
                TempData["errMsg"] = "Something went wrong please contact administrator." + "<br/>" + ex.Message;
                //if (model.SalaryLevelDetail.Count > 0)
                //{
                //    List<DesignationDetailModel> list = new List<DesignationDetailModel>();
                //    for (int i = 0; i < model.SalaryLevelDetail.Count; i++)
                //    {
                //        int ddid = model.SalaryLevelDetail[i].DesignationDetailId ?? 0;
                //        int qulid = model.SalaryLevelDetail[i].Qualification ?? 0;
                //        List<MasterlistviewModel> datalist = new List<MasterlistviewModel>();
                //        datalist = Common.GetCourseList(qulid);
                //        list.Add(new DesignationDetailModel()
                //        {
                //            QualificationCourse = model.SalaryLevelDetail[i].QualificationCourse,
                //            ddlList = datalist,

                //        });
                //    }
                //    model.SalaryLevelDetail = list;
                //}
                return View(model);
            }
        }
        [HttpPost]
        public JsonResult GetSalaryLevelMasterList(SearchdesignationModel model, int pageIndex, int pageSize)
        {
            try
            {
                object output = RequirementMasterService.GetSalaryLevelList(model, pageIndex, pageSize);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetSalaryLevelDetail(int SalaryLevelId)
        {
            try
            {
                var result = RequirementMasterService.GetSalaryLevelDetail(SalaryLevelId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetSalaryRange(int Apptype)
        {
            try
            {
                var result = Common.GetSalaryLevelList(Apptype);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}