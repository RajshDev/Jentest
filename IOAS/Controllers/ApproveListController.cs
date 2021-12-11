using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using IOAS.GenericServices;
using IOAS.Models;
using System.Web.Script.Serialization;
using IOAS.Infrastructure;

namespace IOAS.Controllers
{
    public class ApproveListController : Controller
    {
        // GET: FileUpload
        [Authorize]
        public ActionResult Load()
        {
            return View("load");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Load(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Content/SupportDocuments"),
                                               Path.GetFileName(file.FileName));
                    file.UploadFile("SupportDocuments", Path.GetFileName(file.FileName));
                    //file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View("load");
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetApproveList(int processGuideLineId)
        {
            try
            {
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);

                var resultJson = ProcessEngineService.GetWorkFlowDocumentList(processGuideLineId, userId);
                return Json(resultJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var error = new { reason = ex.ToString() };
                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }


    }
}