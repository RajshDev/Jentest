using IOAS.Filter;
using IOAS.GenericServices;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IOAS.Controllers
{
    [Authorized]
    public class UCController : Controller
    {
        UCService _ucService = new UCService();
        // GET: UC
        public ActionResult Create(int UCId = 0)
        {
            try
            {
                UCModel model = new UCModel();
                ViewBag.UCTypeList = Common.GetCodeControlList("TypeofUC");
                ViewBag.FinYearList = new List<MasterlistviewModel>();
                ViewBag.TemplateList = Common.GetUCTemplateList();
                if (UCId > 0)
                {
                    model = _ucService.GetUCDetail(UCId);
                    ViewBag.FinYearList = Common.GetFinYearList(model.ProjectId ?? 0);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                UCModel model = new UCModel();
                ViewBag.UCTypeList = Common.GetCodeControlList("TypeofUC");
                ViewBag.FinYearList = new List<MasterlistviewModel>();
                ViewBag.TemplateList = Common.GetUCTemplateList();
                return View(model);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(UCModel model)
        {
            try
            {
                ViewBag.UCTypeList = Common.GetCodeControlList("TypeofUC");
                ViewBag.FinYearList = Common.GetFinYearList(model.ProjectId ?? 0);
                ViewBag.TemplateList = Common.GetUCTemplateList();
                if (ModelState.IsValid)
                {

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = _ucService.CreateUC(model, logged_in_user);
                    if (model.UCId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "UC has been added successfully.";
                        return RedirectToAction("List");
                    }
                    else if (model.UCId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "UC has been updated successfully.";
                        return RedirectToAction("List");
                    }
                    else if (result == -2)
                        TempData["errMsg"] = "Audited UC already created for this financial year.";
                    else if (result == -3)
                        TempData["errMsg"] = "Final UC already created for this project.";
                    else if (result == -4)
                        TempData["errMsg"] = "Provisional can't create for this project. Audited / Final UC Created for this financial year.";
                    else if (result == -5)
                        TempData["errMsg"] = "You can't update an outdated provisional UC.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

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
                ViewBag.UCTypeList = Common.GetCodeControlList("TypeofUC");
                ViewBag.FinYearList = Common.GetFinYearList(model.ProjectId ?? 0);
                ViewBag.TemplateList = Common.GetUCTemplateList();
                return View(model);
            }
        }

        public ActionResult View(int UCId)
        {
            try
            {
                UCModel model = new UCModel();
                ViewBag.UCTypeList = Common.GetCodeControlList("TypeofUC");
                ViewBag.FinYearList = Common.GetFinYearList();
                ViewBag.TemplateList = Common.GetUCTemplateList();
                model = _ucService.GetUCDetail(UCId);
                return View(model);
            }
            catch (Exception ex)
            {
                UCModel model = new UCModel();
                ViewBag.UCTypeList = Common.GetCodeControlList("TypeofUC");
                ViewBag.FinYearList = Common.GetFinYearList();
                ViewBag.TemplateList = Common.GetUCTemplateList();
                return View(model);
            }
        }
        public ActionResult List()
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
        [HttpGet]
        public JsonResult SearchUCCommitments(int projectId, int UCId)
        {
            try
            {
                object output = _ucService.SearchUCCommitments(projectId, UCId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetTemplateDetail(int tempId)
        {
            try
            {
                object output = _ucService.GetTemplateDetail(tempId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetFinYearList(int pId)
        {
            try
            {
                object output = Common.GetFinYearList(pId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetExpenditure(int projectId, int UCId, int UCType, int finYear)
        {
            try
            {
                object output = _ucService.GetExpenditure(projectId, UCId, UCType, finYear);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult GetUCList(SearchUCModel model)
        {

            PagedData<UCListModel> data = new PagedData<UCListModel>();
            data = _ucService.GetUCList(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateTemplate(string tempName, string tempData)
        {
            try
            {
                var userId = Common.GetUserid(User.Identity.Name);
                string result = _ucService.CreateTemplate(tempName, tempData, userId);
                if (result == "Success")
                {
                    var data = Common.GetUCTemplateList();
                    return Json(new { status = result, data = data }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "Error" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult DownloadUC(int UCId)
        {
           try
            {
                string loginuser = User.Identity.Name;
                string pdf_page_size = "A4";                SelectPdf.PdfPageSize pageSize =                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);                string pdf_orientation = "Portrait";                SelectPdf.PdfPageOrientation pdfOrientation =                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),                    pdf_orientation, true);                int webPageWidth = 1024;                int webPageHeight = 0;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = pageSize;                converter.Options.PdfPageOrientation = pdfOrientation;                converter.Options.WebPageWidth = webPageWidth;                converter.Options.WebPageHeight = webPageHeight;
                converter.Options.MarginLeft = 10;
                converter.Options.MarginRight = 10;
                converter.Options.MarginTop = 20;
                converter.Options.MarginBottom = 20;
                string rawHtml = _ucService.GetCreatedUC(UCId);
                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(rawHtml);
                byte[] pdf = doc.Save();
                doc.Close();                Response.AddHeader("Content-Disposition", "inline; filename=UC.pdf");                return File(pdf, "application/pdf");
            }
            catch(Exception ex)
            {
                return new EmptyResult();
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetUCComponent(int ProjectId, int finYear)
        {
            var locationdata = _ucService.GetUCComponent(ProjectId, finYear);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateTemplate(int tempId, string tempData)
        {
            try
            {
                var userId = Common.GetUserid(User.Identity.Name);
                string result = _ucService.UpdateTemplate(tempId, tempData, userId);
                if (result == "Success")
                {
                    var data = Common.GetUCTemplateList();
                    return Json(new { status = result, data = data }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "Error" }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult _TemplateBuilderPrint(int UCId)
        {
            UCModel model = new UCModel();

            try
            {

                model = _ucService.GetUCDetail(UCId);
                return View(model);
            }
            catch (Exception ex)
            {

                return View(model);
            }

        }
    }
}