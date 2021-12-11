using IOAS.Filter;
using IOAS.GenericServices;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IOAS.Controllers
{
    [Authorized]
    public class FacilityController : Controller
    {
        #region Tapal
        public ActionResult Tapal()
        {
            //int page = 1;
            CreateTapalModel model = new CreateTapalModel();
            int pagesize = 3;
            ViewBag.Department = Common.GetDepartment();
            ViewBag.Role = Common.GetRoleListByDepId(model.selRole);
            ViewBag.UserList = Common.GetUserListByRoleId(model.selDepartment);
            ViewBag.PIList = Common.GetPIWithDetails();
            ViewBag.TapalCatagory = Common.GetTapalCatagory();
            FacilityService _TS = new FacilityService();
            TapalSearchFieldModel Search = new TapalSearchFieldModel();
            DateTime? ToDate = DateTime.Now;
            DateTime? FromeDate = DateTime.Now.AddDays(-180);
            Search.FromCreatedDate = FromeDate;
            Search.ToCreatedDate = ToDate;
            var UserName = User.Identity.Name;
            var role = Common.GetRoleId(UserName);
            var UserId = Common.GetUserid(UserName);
            ViewBag.Action = Common.GetTapalAction(UserId);
            var AccessRole = Common.GetRoleAccess(UserId, 14);
            if (AccessRole.Count > 0)
            {
                if (AccessRole[0].IsRead == true)
                {
                    model.GetTapalInventryDetails = _TS.GetInventryDetailByUser(1, pagesize, Search, UserId);
                }
                //else if(AccessRole[0].IsRead == true)
            }
            //if (role == 1)
            //{
            //    model.GetTapalInventryDetails = _TS.GetInventryDetail(page, pagesize, Search);
            //}
            //else
            //{
            //    model.GetTapalInventryDetails = _TS.GetInventryDetailByUser(page, pagesize, Search, UserId);
            //}
            //model.T
            return View(model);
        }
        [HttpPost]
        public ActionResult TapalIU(CreateTapalModel model, string tapalType)
        {
            try
            {
                int page = 1;
                int pagesize = 3;
                ViewBag.Department = Common.GetDepartment();
                ViewBag.UserList = Common.GetUserListByRoleId(model.selDepartment);
                ViewBag.Role = Common.GetRoleListByDepId(model.selRole);
                FacilityService _TS = new FacilityService();
                TapalSearchFieldModel Search = new TapalSearchFieldModel();
                DateTime? ToDate = DateTime.Now;
                DateTime? FromeDate = DateTime.Now.AddDays(-180);
                Search.FromCreatedDate = FromeDate;
                Search.ToCreatedDate = ToDate;
                int logged_in_userId = Common.GetUserid(User.Identity.Name);
                int RoleId = Common.GetRoleId(User.Identity.Name);
                ViewBag.Action = Common.GetTapalAction(logged_in_userId);
                var AccessRole = Common.GetRoleAccess(logged_in_userId, 14);
                if (AccessRole.Count > 0)
                {
                    if (AccessRole[0].IsRead == true)
                    {
                        model.GetTapalInventryDetails = _TS.GetInventryDetailByUser(page, pagesize, Search, logged_in_userId);
                    }
                    //else if(AccessRole[0].IsRead == true)
                }
                // if (RoleId == 1)
                // {
                //     model.GetTapalInventryDetails = _TS.GetInventryDetail(page, pagesize, Search);
                // }
                //else
                // {
                //     model.GetTapalInventryDetails = _TS.GetInventryDetailByUser(page, pagesize, Search, logged_in_userId);
                // }
                if (tapalType == "Yes")
                {
                    model.ProjectTabal = true;
                }
                else
                {
                    model.ProjectTabal = false;
                }
                if (ModelState.IsValid)
                {
                    List<DocumentDetailModel> DocDetail = new List<DocumentDetailModel>();
                    if (model.TapalId == 0 && model.files[0] == null)
                    {
                        ViewBag.ErrMsg = "Documents filed is required.";
                        return RedirectToAction("Tapal");
                    }
                    foreach (HttpPostedFileBase file in model.files)
                    {
                        if (file != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".png", ".jpeg", ".jpg" };
                            string docname = Path.GetFileName(file.FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                               TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx, .png, .jpeg, .jpg]";
                                return RedirectToAction("Tapal");
                            }
                            if (docextension == ".jpeg" || docextension == ".png" || docextension == ".jpg")
                            {
                                if (file.ContentLength > 26214400)
                                {
                                    TempData["errMsg"] = "You can upload image up to 20 MB";
                                    return RedirectToAction("Tapal");
                                }
                                string ImgName = System.IO.Path.GetFileName(file.FileName);
                                var fileId = Guid.NewGuid().ToString();
                                ImgName = fileId + "_" + ImgName;
                                file.UploadFile("TapalDocuments", ImgName);
                                DocDetail.Add(new DocumentDetailModel()
                                {
                                    DocName = ImgName,
                                    FileName = file.FileName
                                });
                            }
                            else
                            {
                                string docpath = System.IO.Path.GetFileName(file.FileName);
                                var docfileId = Guid.NewGuid().ToString();
                                var fileName = docfileId + "_" + docpath;
                                /*Save the file in server folder*/
                                file.UploadFile("TapalDocuments", fileName);
                                DocDetail.Add(new DocumentDetailModel()
                                {
                                    DocName = fileName,
                                    FileName = file.FileName
                                });
                            }

                        }
                    }
                    model.DocDetail = DocDetail.ToList();
                    FacilityService TS = new FacilityService();
                    int AddEntry = TS.AddNewEntry(model, logged_in_userId);
                    if (AddEntry == 1 && model.TapalId == 0)
                    {
                        TempData["SuccMsg"] = "New entry has been added successfully";
                    }
                    else if (AddEntry == 1 && model.TapalId > 0)
                    {
                        TempData["SuccMsg"] = "Inward has been updated successfully";
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return RedirectToAction("Tapal");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult SaveInwardEntry(int Action,int Department,int Role, int ToUser, string Remarks, int TapalId,bool PopUpEdit)
        {
            try
            {
                FacilityService _TS = new FacilityService();
                int logged_in_userId = Common.GetUserid(User.Identity.Name);
                int logged_in_RoleId = Common.GetRoleId(User.Identity.Name);
                int Update = _TS.SaveInwardEntry(Action,Department, Role, ToUser, Remarks, TapalId, logged_in_userId, PopUpEdit);
                if (Update == 1)
                {
                    return Json(Update, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Update, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetOutwardDetails()
        {
            var Role = Common.GetRoleId(User.Identity.Name);
            var UserId = Common.GetUserid(User.Identity.Name);
            object output = FacilityService.GetOutwardDetails(Role, UserId);
            //object output = FacilityService.GetOutwardDetails(UserId,new ListTapalModel(),new DateFilterModel(),1,5);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="OutwardDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetOutwardDetails(ListTapalModel model, DateFilterModel OutwardDate,int pageIndex,int pageSize)
        {
            var Role = Common.GetRoleId(User.Identity.Name);
            var UserId = Common.GetUserid(User.Identity.Name);
            if (model == null)
                model = new ListTapalModel();
            if (OutwardDate == null)
                OutwardDate = new DateFilterModel();
            object output = FacilityService.GetOutwardDetails(UserId,model,OutwardDate,pageIndex,pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAcceptedTapalDetails()
        {
            var Role = Common.GetRoleId(User.Identity.Name);
            var UserId = Common.GetUserid(User.Identity.Name);
            object output = FacilityService.GetAcceptedTapalDetails(Role, UserId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetInwardForEdit(int TapalId)
        {
            try
            {
                ViewBag.PIList = Common.GetPIWithDetails();
                ViewBag.PIProject = new List<MasterlistviewModel>();
                ViewBag.TapalCatagory = Common.GetTapalCatagory();
                FacilityService _fs = new FacilityService();
                CreateTapalModel model = new CreateTapalModel();
                if (TapalId > 0)
                {
                    model = _fs.GetInwardForEdit(TapalId);                   
                    if (model.PIName != null)
                        ViewBag.PIProject = Common.GetPIProjects((Int32)model.PIName);
                }
                else
                {
                    model.TapalId = 0;
                    model.ReceiptDate = DateTime.Now;
                    List<DocumentDetailModel> DocDetails = new List<DocumentDetailModel>();
                    model.DocDetail = DocDetails;
                }
                return PartialView("PopupTapalInward", model);
            }
            catch (Exception ex)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }

        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetOutwardForEdit(int TapalId,int UserId)
        {
            try
            {
                var role = Common.GetRoleId(User.Identity.Name);
                ViewBag.Action = Common.GetTapalAction(UserId);
                ViewBag.Department = Common.GetDepartment();                
                FacilityService _fs = new FacilityService();
                CreateTapalModel model = new CreateTapalModel();
                model = _fs.GetOutwardForEdit(TapalId, UserId);
                ViewBag.Role = Common.GetRoleListByDepId(model.selDepartment);
                ViewBag.UserList = Common.GetUserListByRoleId(model.selRole);
                return PartialView("PopupTapalOutward", model);
            }
            catch (Exception ex)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }

        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadRoleList(int ID)
        {
            var dataRole = Common.GetRoleListByDepId(ID);
            return Json(dataRole, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadUserList(int ID)
        {
            var dataUser = Common.GetUserListByRoleId(ID);
            return Json(dataUser, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowDocument(int TapalDocId, int TapalId)
        {
            try
            {
                var fileName = Common.getUploadFileName(TapalDocId, TapalId);
                //var filepath = "~/Content/TapalDocuments/";
                byte[] contents = fileName.Item2.DownloadFile("TapalDocuments");
                Response.AddHeader("Content-Disposition", "inline; filename=\"" + fileName.Item1 + "\"");
                string ext = Common.GetMimeType(Path.GetExtension(fileName.Item1));
                return File(contents, ext);
            }
            catch (Exception ex)
            {
                int pagesize = 3;
                string Search = "";
                ViewBag.Department = Common.GetDepartment();
                ViewBag.UserList = Common.GetUserList();
                ViewBag.TapalCatagory = Common.GetTapalCatagory();
                CreateTapalModel model = new CreateTapalModel();
                FacilityService _TS = new FacilityService();
                model.GetTapalInventryDetails = _TS.GetInventryDetail(1, pagesize, Search);
                model.ReceiptDate = DateTime.Now;
                ViewBag.ErrMsg = "Document not available, Please contact administrator";
                return View("Tapal", model);
            }

        }


        [HttpPost]
        public ActionResult PopupTapalDetails(int TapalId)
        {
            try
            {
                TapalDetailsModel model = new TapalDetailsModel();
                FacilityService _fs = new FacilityService();
                int userId = Common.GetUserid(User.Identity.Name);
                model = _fs.GetTapalDetails(TapalId, userId);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }

        //[HttpPost]
        //public ActionResult PopupProjectDetails(int ProjectId)
        //{
        //    try
        //    {
        //        ProjectDetailModel model = new ProjectDetailModel();
        //        //FacilityService _fs = new FacilityService();
        //        model = Common.GetProjectsDetails(ProjectId);
        //        return PartialView("PopupProjectDetails",model);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new EmptyResult();
        //    }
        //}

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadProjectDetails(int ProjectId)
        {           
            var data = Common.GetProjectsDetails(ProjectId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult DeleteDocument(int TapalDocId, int TapalId)
        {
            try
            {
                FacilityService _FS = new FacilityService();
                var result = _FS.DeleteDocument(TapalDocId, TapalId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var result = 0;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchTapal(TapalSearchFieldModel srchModel, int page)
        {
            try
            {
                CreateTapalModel model = new CreateTapalModel();
                int pageSize = 3;
                ViewBag.Department = Common.GetDepartment();
                ViewBag.Role = Common.GetRoleListByDepId(model.selRole);
                ViewBag.UserList = Common.GetUserListByRoleId(model.selDepartment);
                FacilityService _TS = new FacilityService();
                var UserName = User.Identity.Name;
                var role = Common.GetRoleId(UserName);
                var UserId = Common.GetUserid(UserName);
                ViewBag.Action = Common.GetTapalAction(UserId);
                var data = new PagedData<ListTapalModel>();
                FacilityService _fs = new FacilityService();
                int UserID = Common.GetUserid(User.Identity.Name);
                //if (srchModel.FromCreatedDate == null)
                //{
                //    DateTime? FromeDate = DateTime.Now.AddDays(-180);
                //    srchModel.FromCreatedDate = FromeDate;
                //}
                //if (srchModel.ToCreatedDate == null)
                //{
                //    DateTime? ToDate = DateTime.Now;
                //    srchModel.ToCreatedDate = ToDate;
                //}
                data = _fs.GetInventryDetailByUser(page, pageSize, srchModel, UserID);
                model.GetTapalInventryDetails = data;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }

        [HttpPost]
        public JsonResult GetAcceptedTapalDetails(ListTapalModel model, DateFilterModel OutwardDate, int pageIndex, int pageSize)
        {
            var Role = Common.GetRoleId(User.Identity.Name);
            var UserId = Common.GetUserid(User.Identity.Name);
            object output = FacilityService.GetAcceptedTapalDetails(Role, UserId, model, OutwardDate, pageIndex, pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult _TapalDetails(int TapalId)
        {
            try
            {
                FacilityService _fs = new FacilityService();
                CreateTapalModel model = new CreateTapalModel();
                if (TapalId > 0)
                {
                    model = _fs.GetDisplayTapalDetails(TapalId);
                }
                else
                {
                    model.TapalId = 0;
                    model.ReceiptDate = DateTime.Now;
                    List<DocumentDetailModel> DocDetails = new List<DocumentDetailModel>();
                    model.DocDetail = DocDetails;
                }
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region SRB
        public ActionResult SRBList()
        {
            try
            {
                int page = 1;
                int pageSize = 5;
                ViewBag.DepartmentList = Common.GetDepartment();
                var data = new PagedData<SRBSearchResultModel>();
                SRBListModel model = new SRBListModel();
                FacilityService _fs = new FacilityService();
                SRBSearchFieldModel srchModel = new SRBSearchFieldModel();

                data = _fs.GetSRBList(srchModel, page, pageSize);

                model.SearchResult = data;
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchSRBList(SRBSearchFieldModel srchModel, int page)
        {
            try
            {
                int pageSize = 5;
                var data = new PagedData<SRBSearchResultModel>();
                SRBListModel model = new SRBListModel();
                FacilityService _fs = new FacilityService();
                if (srchModel.ToPODate != null)
                {
                    DateTime todate = (DateTime)srchModel.ToPODate;
                    srchModel.ToPODate = todate.Date.AddDays(1).AddTicks(-2);
                }
                else if (srchModel.ToSRBDate != null)
                {
                    DateTime todate = (DateTime)srchModel.ToSRBDate;
                    srchModel.ToSRBDate = todate.Date.AddDays(1).AddTicks(-2);
                }

                data = _fs.GetSRBList(srchModel, page, pageSize);

                model.SearchResult = data;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }
        [HttpPost]
        public ActionResult PopupSRBDetails(int SRBId)
        {
            try
            {
                SRBDetailsModel model = new SRBDetailsModel();
                FacilityService _fs = new FacilityService();

                model = _fs.GetSRBDetails(SRBId);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }

        public ActionResult SRB(int SRBId = 0)
        {
            try
            {
                ViewBag.CategoryList = Common.GetSRBItemCatagory();
                ViewBag.DepartmentList = Common.GetDepartment();
                ViewBag.UOMList = Common.GetCodeControlList("UOM");
                //ViewBag.PIList = Common.GetPIWithDetails();
                //ViewBag.PIProject = new List<MasterlistviewModel>();
                SRBModel model = new SRBModel();
                FacilityService _fs = new FacilityService();
                if (SRBId > 0)
                {
                    model = _fs.GetSRBForEdit(SRBId);
                    //if (model.PIName != null)
                    //    ViewBag.PIProject = Common.GetPIProjects((Int32)model.PIName);
                }
                else
                {
                    model.SRBId = 0;
                    model.InwardDate = DateTime.Now;
                    model.PurchaseDate = DateTime.Now;
                }
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }
        [HttpPost]
        public ActionResult SRB(SRBModel model)
        {
            try
            {
                ViewBag.CategoryList = Common.GetSRBItemCatagory();
                ViewBag.DepartmentList = Common.GetDepartment();
                ViewBag.UOMList = Common.GetCodeControlList("UOM");
                //ViewBag.PIList = Common.GetPIWithDetails();
                //ViewBag.PIProject = model.PIName == null ? new List<MasterlistviewModel>() : Common.GetPIProjects((Int32)model.PIName);
                if (ModelState.IsValid)
                {
                    List<MasterlistviewModel> checkdup = new List<MasterlistviewModel>();
                    for (int i = 0; i < model.selBuyBack.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(model.selBuyBack[i]))
                        {
                            var selItems = model.selBuyBack[i].Split(',');
                            for (int j = 0; j < selItems.Length; j++)
                            {
                                int item = Convert.ToInt32(selItems[j]);
                                var query = checkdup.FirstOrDefault(m => m.id == item);
                                if(query != null)
                                {
                                    TempData["PostErrMsg"] = "Some of the buyback are duplicated. Please rectify and submit again.";
                                    return RedirectToAction("SRBList");
                                }
                                checkdup.Add(new MasterlistviewModel()
                                {
                                    id = item
                                });
                            }
                        }
                    }
                    if (model.PODocument != null)
                    {
                        var allowedExtensions = new[] { ".doc", ".docx", ".pdf", ".DOC", ".DOCX", ".PDF" };
                        var ext = Path.GetExtension(model.PODocument.FileName);
                        if (!allowedExtensions.Contains(ext))
                        {
                            TempData["PostErrMsg"] = "Please upload any one of these type file [doc, docx, pdf]";
                            return RedirectToAction("SRBList");
                        }
                        else if (model.PODocument.ContentLength > 5242880)
                        {
                            TempData["PostErrMsg"] = "You can upload the file up to 5 MB";
                            return RedirectToAction("SRBList");
                        }

                        string DocName = System.IO.Path.GetFileName(model.PODocument.FileName);
                        var fileId = Guid.NewGuid().ToString();
                        DocName = fileId + "_" + DocName;
                        model.PODocument.UploadFile("SRBDocuments", DocName);
                        model.DocFullName = DocName;
                    }
                    int UserId = Common.GetUserid(User.Identity.Name);
                    FacilityService _fs = new FacilityService();
                    var status = _fs.PostSRB(model, UserId);
                    if (status == 1)
                        TempData["PostSuccMsg"] = "SRB has been created successfully.";
                    else if (status == 2)
                        TempData["PostSuccMsg"] = "SRB has been updated successfully.";
                    else if (status == -2)
                        TempData["PostErrMsg"] = "No records found for update the details.";
                    else
                        TempData["PostErrMsg"] = "Something went wrong please contact administrator";
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["PostErrMsg"] = messages;
                }
                return RedirectToAction("SRBList");
            }
            catch (Exception ex)
            {
                ViewBag.CategoryList = Common.GetSRBItemCatagory();
                ViewBag.DepartmentList = Common.GetDepartment();
                //ViewBag.PIList = Common.GetPIWithDetails();
                //ViewBag.PIProject = model.PIName == null ? new List<MasterlistviewModel>() : Common.GetPIProjects((Int32)model.PIName);
                TempData["PostErrMsg"] = "Something went wrong please contact administrator";
                return RedirectToAction("SRBList");
            }
        }

        [Authorize]
        public ActionResult ActivateSRB(int Id)
        {
            try
            {
                FacilityService _fs = new FacilityService();
                int UserId = Common.GetUserid(User.Identity.Name);
                bool status = _fs.ActivateSRB(Id, UserId);
                if (status)
                    TempData["PostSuccMsg"] = "SRB has been activated successfully.";
                else
                    TempData["PostErrMsg"] = "Something went wrong please contact administrator";
                return RedirectToAction("SRBList");
            }
            catch (Exception ex)
            {
                TempData["PostErrMsg"] = "Something went wrong please contact administrator";
                return RedirectToAction("SRBList");
            }
        }
        [Authorize]
        public ActionResult SRBDeactivation()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult SRBItemDeactivate(int SRBDetailId, string Status)
        {
            try
            {
                if (Status == "Buyback")
                {
                    FacilityService _fs = new FacilityService();
                    SRBItemDetailsModel model = new SRBItemDetailsModel();
                    model = _fs.GetSRBItemForDeactivate(SRBDetailId);
                    return PartialView("PopupDeactivation", model);
                }
                else if (Status == "Scrap")
                {
                    FacilityService _fs = new FacilityService();
                    int UserId = Common.GetUserid(User.Identity.Name);
                    bool status = _fs.ScrapSRBItem(SRBDetailId, UserId);
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult PopupDeactivation(SRBItemDetailsModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Attachment != null)
                    {
                        var allowedExtensions = new[] { ".doc", ".docx", ".pdf", ".DOC", ".DOCX", ".PDF" };
                        var ext = Path.GetExtension(model.Attachment.FileName);
                        if (!allowedExtensions.Contains(ext))
                        {
                            TempData["PostErrMsg"] = "Please upload any one of these type file [doc, docx, pdf]";
                            return RedirectToAction("SRBDeactivation");
                        }
                        else if (model.Attachment.ContentLength > 5242880)
                        {
                            TempData["PostErrMsg"] = "You can upload the file up to 5 MB";
                            return RedirectToAction("SRBDeactivation");
                        }

                        string DocName = System.IO.Path.GetFileName(model.Attachment.FileName);
                        var fileId = Guid.NewGuid().ToString();
                        DocName = fileId + "_" + DocName;
                        model.Attachment.UploadFile("SRBDocuments", DocName);
                        model.DocName = DocName;
                    }
                    int UserId = Common.GetUserid(User.Identity.Name);
                    FacilityService _fs = new FacilityService();
                    var status = _fs.Deactivation(model, UserId);
                    if (status == 1)
                        TempData["PostSuccMsg"] = "Item has been deactivated successfully.";
                    else if (status == -2)
                        TempData["PostErrMsg"] = "No records found for update the details.";
                    else
                        TempData["PostErrMsg"] = "Something went wrong please contact administrator";
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["PostErrMsg"] = messages;
                }
                return RedirectToAction("SRBDeactivation");
            }
            catch (Exception ex)
            {
                TempData["PostErrMsg"] = "Something went wrong please contact administrator";
                return RedirectToAction("SRBDeactivation");
            }
        }

        [HttpPost]
        public ActionResult CustomSelectList(string selItems)
        {
            try
            {
                FacilityService _fs = new FacilityService();                
                var model = _fs.GetSRBDeactivatedItem(selItems);
                ViewBag.selItems = selItems.Split(',');
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return PartialView();
            }
        }
        #endregion
        #region Functions
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPIByInstitute(int InstituteId)
        {
            var data = Common.GetPIByInstitute(InstituteId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPIProject(int PIId)
        {
            var data = Common.GetPIProjects(PIId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult CheckIsAsset(int CategoryId)
        {
            var data = Common.CheckIsAsset(CategoryId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetSRBItemForDeactivate()
        {
            FacilityService _fs = new FacilityService();
            var output = _fs.GetSRBItemForDeactivate();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}