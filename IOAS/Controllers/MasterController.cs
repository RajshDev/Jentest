using IOAS.Filter;
using IOAS.GenericServices;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Drawing;
using System.IO;

namespace IOAS.Controllers
{
    public class MasterController : Controller
    {
        [Authorized]
        [HttpGet]
        public ActionResult Vendor()
        {
            ViewBag.vendorCountry = Common.GetAgencyType();
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
            ViewBag.bankcountry = Common.getCountryList();
            return View();
        }
        [Authorized]
        [HttpPost]
        public ActionResult Vendor(VendorMasterViewModel model)
        {
            try
            {
                ViewBag.vendorCountry = Common.GetAgencyType();
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
                ViewBag.bankcountry = Common.getCountryList();
                var Username = User.Identity.Name;
                model.UserId = Common.GetUserid(Username);
                if (ModelState.IsValid)
                {
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                    if ((model.GSTAttachName[0] != null && model.GSTAttachName[0] != ""))
                    {
                        for (int i = 0; i < model.GSTDocumentType.Length; i++)
                        {
                            if (model.GSTFile[i] != null)
                            {
                                string docname = Path.GetFileName(model.GSTFile[i].FileName);
                                var docextension = Path.GetExtension(docname);
                                if (!allowedExtensions.Contains(docextension))
                                {
                                    ViewBag.filemsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                    return View(model);
                                }
                            }

                        }
                    }
                    if ((model.VendorAttachName[0] != null && model.VendorAttachName[0] != ""))
                    {
                        for (int i = 0; i < model.VendorDocumentType.Length; i++)
                        {
                            if (model.VendorFile[i] != null)
                            {
                                string docname = Path.GetFileName(model.VendorFile[i].FileName);
                                var docextension = Path.GetExtension(docname);
                                if (!allowedExtensions.Contains(docextension))
                                {
                                    ViewBag.filemsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                    return View(model);
                                }
                            }

                        }
                    }
                    if ((model.TDSAttachName[0] != null && model.TDSAttachName[0] != ""))
                    {
                        for (int i = 0; i < model.TDSDocumentType.Length; i++)
                        {
                            if (model.TDSFile[i] != null)
                            {
                                string docname = Path.GetFileName(model.TDSFile[i].FileName);
                                var docextension = Path.GetExtension(docname);
                                if (!allowedExtensions.Contains(docextension))
                                {
                                    ViewBag.filemsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                    return View(model);
                                }
                            }

                        }
                    }

                    int vendorStatus = MasterService.VendorMaster(model);
                    if (vendorStatus == 1)
                    {
                        ViewBag.success = "Saved successfully";
                    }
                    else if (vendorStatus == 2)
                    {
                        ViewBag.Msg = "This Vendor Account Number and GSTIN Number Already Exits";
                        return View(model);
                    }
                    else if (vendorStatus == 3)
                    {
                        ViewBag.update = "Vendor updated successfully";
                    }
                    else if (vendorStatus == 4)
                    {
                        ViewBag.Msgs = "This PFMS Number Alredy Exits";
                        return View(model);
                    }
                    else
                    {
                        ViewBag.error = "Somthing went to worng please contact Admin!.";
                        return View(model);
                    }
                    return View();
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    ViewBag.error = messages;

                    return View();
                }
            }catch(Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
       (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return View();
            }
            

        }
        // GET: Master
        [Authorized]
        [HttpGet]
        public ActionResult InternalAgency()
        {
            ViewBag.project = Common.getprojecttype();
            ViewBag.agencydoc = Common.GetDocTypeList(19);
            return View();
        }
        [Authorized]
        [HttpPost]
        public ActionResult InternalAgency(InternalAgencyViewModel model, HttpPostedFileBase File)
        {
            ViewBag.project = Common.getprojecttype();
            ViewBag.agencydoc = Common.GetDocTypeList(19);
            var Username = User.Identity.Name;
            model.InternalAgencyUserId = Common.GetUserid(Username);
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
            if (ModelState.IsValid)
            {
                if ((model.AttachName[0] != null && model.AttachName[0] != ""))
                {
                    for (int i = 0; i < model.DocumentType.Length; i++)
                    {
                        if (model.File[i] != null)
                        {
                            string docname = Path.GetFileName(model.File[i].FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                ViewBag.filemsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View();
                            }
                        }

                    }
                }
                int internalstatus = MasterService.InternalAgency(model);
                if (internalstatus == 1)
                {
                    ViewBag.success = "Saved successfully";
                }
                else if (internalstatus == 2)
                    ViewBag.Msg = "This agency name already exits";
                else if (internalstatus == 3)
                    ViewBag.update = "Agency updated successfully";
                else if (internalstatus == 4)
                    ViewBag.intalcode = "Internal agency Code already exits";
                else
                    ViewBag.error = "Somthing went to worng please contact Admin!.";
                return View();
            }
            else
            {
                string messages = string.Join("<br />", ModelState.Values
                                   .SelectMany(x => x.Errors)
                                   .Select(x => x.ErrorMessage));

                ViewBag.error = messages;

                return View();
            }
        }
        [Authorized]
        [HttpGet]
        public JsonResult GetInternalAgency(InternalAgencyViewModel model)
        {
            object output = MasterService.GetInternalAgency(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpGet]
        public JsonResult GetInternalAgencycode()
        {
            object output = MasterService.InternalAgencyCode();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult EditInternalAgency(int agencyId)
        {
            object output = MasterService.EditInternalAgency(agencyId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public ActionResult DeleteInternalAgency(int agencyId)
        {
            string Username = User.Identity.Name;
            object output = MasterService.DeleteInternalAgency(agencyId, Username);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
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
        
        [Authorized]
        [HttpPost]
        public JsonResult GetVendorMaster(VendorSearchModel model, int pageIndex, int pageSize)
        {
            object output = MasterService.GetVendorList(model, pageIndex, pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult VendorWFInit(int vendorId)
        {
            try
            {
                if (Common.ValidateVendorOnEdit(vendorId, "Open"))
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    bool status = MasterService.VendorWFInit(vendorId, userId);
                    return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = false, msg = "This vendor already approved" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorized]
        [HttpPost]
        public JsonResult EditVendorlist(int vendorId)
        {
            object output = MasterService.EditVendor(vendorId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public ActionResult VendorManagementView(int Vendorid = 0)
        {
            try
            {
                VendorManagementModel model = new VendorManagementModel();
                model = MasterService.GetVendorManagementDetails(Vendorid);
                ViewBag.processGuideLineId = 160;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Authorized]
        [HttpPost]
        public JsonResult Gettdspercetage(int sectionId)
        {
            object output = MasterService.GetSectiontds(sectionId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorized]
        public ActionResult LedgerOBBalance()
        {
            ViewBag.AcctCty = Common.GetAccounttype();
            ViewBag.FinYear = Common.GetFinYearList();
            return View();
        }
        [Authorized]
        [HttpGet]
        public JsonResult LoadListWiseHead(int accounttypid)
        {
            object output = MasterService.GetAccountWiseHead(accounttypid);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpGet]
        public JsonResult GetOpeningBal(int accheadid)
        {
            object output = MasterService.GetOpeningBalance(accheadid);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult AddOpeningBalanceLedger(LedgerOBBalanceModel model)
        {
            var Username = User.Identity.Name;
            model.Userid = Common.GetUserid(Username);
            object output = MasterService.AddOpeningBalanceLedger(model, Username);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorized]
        [HttpGet]
        public ActionResult BankMaster(int StaffBankId = 0)
        {
            ViewBag.bankcategory = Common.GetBankCategory();
            ViewBag.bankname = Common.GetBankName();
            BankAccountMaster model = new BankAccountMaster();
            if (StaffBankId > 0)
            {
                model = MasterService.EditBankMaster(StaffBankId);
            }
            return View(model);
        }
        [Authorized]
        [HttpPost]
        public ActionResult BankMaster(BankAccountMaster model)
        {
            try
            {
                var Username = User.Identity.Name;
                model.CreateUser = Common.GetUserid(Username);
                int status = MasterService.CreatBankAccount(model);
                if (model.StaffBankId == null && status == 1)
                {
                    TempData["succMsg"] = "Account has been added successfully.";
                    return RedirectToAction("BankMasterList");
                }
                else if (model.StaffBankId > 0 && status > 0)
                {
                    TempData["succMsg"] = "Account has been updated successfully.";
                    return RedirectToAction("BankMasterList");
                }
                else if (model.StaffBankId == null && status == 3)
                {
                    TempData["alertMsg"] = "No Records Found";
                    return RedirectToAction("BankMasterList");
                }
                else if (model.StaffBankId == null && status == 4)
                {
                    TempData["alertMsg"] = "This AccountNumber already Exits";
                    return RedirectToAction("BankMasterList");
                }
                else if (model.StaffBankId == null && status == 5)
                {
                    TempData["alertMsg"] = "This User already Exits";
                    return RedirectToAction("BankMasterList");
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator.";
                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [Authorized]
        [HttpGet]
        public ActionResult BankMasterList()
        {
            return View();
        }

        [Authorized]
        [HttpPost]
        public JsonResult LoadBankList(SearchBankMaster model, int pageIndex, int pageSize)
        {
            object output = MasterService.GetBankList(model, pageIndex, pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorized]
        [HttpGet]
        public ActionResult RevisedVendor()
        {
            ViewBag.vendorCountry = Common.GetAgencyType();
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
            ViewBag.bankcountry = Common.getCountryList();
            return View();
        }
        [Authorized]
        [HttpPost]
        public ActionResult RevisedVendor(VendorMasterViewModel model)
        {
            ViewBag.vendorCountry = Common.GetAgencyType();
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
            if (ModelState.IsValid)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                if ((model.GSTAttachName[0] != null && model.GSTAttachName[0] != ""))
                {
                    for (int i = 0; i < model.GSTDocumentType.Length; i++)
                    {
                        if (model.GSTFile[i] != null)
                        {
                            string docname = Path.GetFileName(model.GSTFile[i].FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                ViewBag.filemsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }

                    }
                }
                if ((model.VendorAttachName[0] != null && model.VendorAttachName[0] != ""))
                {
                    for (int i = 0; i < model.VendorDocumentType.Length; i++)
                    {
                        if (model.VendorFile[i] != null)
                        {
                            string docname = Path.GetFileName(model.VendorFile[i].FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                ViewBag.filemsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }

                    }
                }
                if ((model.TDSAttachName[0] != null && model.TDSAttachName[0] != ""))
                {
                    for (int i = 0; i < model.TDSDocumentType.Length; i++)
                    {
                        if (model.TDSFile[i] != null)
                        {
                            string docname = Path.GetFileName(model.TDSFile[i].FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                ViewBag.filemsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }

                    }
                }

                int vendorStatus = MasterService.VendorMaster(model);
                if (vendorStatus == 1)
                {
                    ViewBag.success = "Saved successfully";
                }
                else if (vendorStatus == 2)
                {
                    ViewBag.Msg = "This Vendor Account Number and GSTIN Number Already Exits";
                    return View(model);
                }
                else if (vendorStatus == 3)
                {
                    ViewBag.update = "Vendor updated successfully";
                }
                else if (vendorStatus == 4)
                {
                    ViewBag.Msgs = "This PFMS Number Alredy Exits";
                    return View(model);
                }
                else
                {
                    ViewBag.error = "Somthing went to worng please contact Admin!.";
                    return View(model);
                }
                return View();
            }
            else
            {
                string messages = string.Join("<br />", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                ViewBag.error = messages;


                return View();
            }
        }
        [Authorized]
        [HttpPost]
        public JsonResult GetRevisedVendorMaster(VendorSearchModel model, int pageIndex, int pageSize)
        {
            object output = MasterService.GetRevisedVendorList(model, pageIndex, pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult RevisedEditVendorlist(int vendorId)
        {
            object output = MasterService.RevisedEditvendor(vendorId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #region AdhocPICreation
        public ActionResult AdhocPICreationList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SearchAdhocPIList(int pageIndex, int pageSize, AdhocPISearchModel model)
        {

            object output = MasterService.GetAdhocPIList(model, pageIndex, pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AdhocPICreation(int adhocId = 0)
        {
            AdhocPICreationModel model = new AdhocPICreationModel();
            try
            {
                if (adhocId > 0)
                {
                    model = MasterService.EditPICreation(adhocId);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult AdhocPICreation(AdhocPICreationModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var Username = User.Identity.Name;
                    int UserId = Common.GetUserid(Username);
                    int Status = MasterService.CreateAdhocPI(model, UserId);
                    if (Status == 1)
                    {
                        TempData["succMsg"] = "PI Add Sucesssfully";
                        return RedirectToAction("AdhocPICreationList");
                    }
                    else if (Status == 2)
                    {
                        TempData["succMsg"] = "PI Updated Sucesssfully";
                        return RedirectToAction("AdhocPICreationList");
                    }
                    else if (Status == 3)
                    {
                        ViewBag.alerMsg = "PI Code already Exist";
                        //return RedirectToAction("AdhocPICreationList");
                    }
                    else
                    {
                        ViewBag.errMsg = "Something went wrong please contact administrator";
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
                //return RedirectToAction("AdhocPICreationList");
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpGet]
        public JsonResult GetDeparmentName(string term)
        {

            object output = Common.GetAutoCompleteDepartment(term);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}