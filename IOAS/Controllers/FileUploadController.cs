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
    public class FileUploadController : Controller
    {
        // GET: FileUpload
        [Authorize]
        public ActionResult Index()
        {
            return View("FileUploader");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
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
            return View("FileUploader");
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetDocumentList(int processGuideLineId)
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


        [Authorize]
        [HttpPost]
        public JsonResult upload()
        {
            string msg = "";
            int count = Request.Files.Count;
            HttpFileCollectionBase files = Request.Files;

            var uuid = System.Guid.NewGuid();
            var processId = Request.Form["processGuideLineId"];
            var fileInfo = Request.Form["fileInfo"];
            int processGuideLineId = -1;
            if (processId != null)
            {
                processGuideLineId = Convert.ToInt32(processId.ToString());
            }
            
            var userName = HttpContext.User.Identity.Name;
            var userId = AdminService.getUserByName(userName);
            List<ProcessGuidelineWorkflowDocument> postedDocuments = new List<ProcessGuidelineWorkflowDocument>();
            if (fileInfo != null)
            {
                postedDocuments = new JavaScriptSerializer().Deserialize<List<ProcessGuidelineWorkflowDocument>>(fileInfo);
            }
            List<ProcessGuidelineWorkflowDocument> documents = ProcessEngineService.GetWorkFlowDocumentList(processGuideLineId, userId);
            List<ProcessTransactionDocuments> requiredDocs = new List<ProcessTransactionDocuments>();
            for (int i = 0; i < documents.Count; i++)
            {
                var doc = postedDocuments.Where(x => x.ProcessGuidelineWorkflowDocumentId == documents[i].ProcessGuidelineWorkflowDocumentId).ToList();
                if (doc != null && doc.Count > 0)
                {
                    requiredDocs.Add(new ProcessTransactionDocuments
                    {
                        DocumentId = documents[i].DocumentId,
                        DocumentName = doc[0].DocumentName,
                        IsRequired = documents[i].IsRequired,
                        UUID = uuid.ToString(),
                    });
                }

            }

            for (int i = 0; i < files.Count; i++)
            {
                try
                {
                    var uniqueName = System.Guid.NewGuid();
                    var actualFileName = files[i].FileName;

                    var fileName = files[i].FileName.Replace(' ', '-');
                    string path = Path.Combine(Server.MapPath("~/Content/SupportDocuments"), uniqueName + "_" + fileName);
                    //Path.GetFileName(uniqueName+"_"+fileName));

                    files[i].UploadFile("SupportDocuments", uniqueName + "_" + fileName);
                    //files[i].SaveAs(path);
                    var record = requiredDocs.Where(x => x.DocumentName == actualFileName && x.UUID == uuid.ToString()).ToList();
                    record.ForEach(x => x.DocumentPath = path);
                    msg = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    msg = "ERROR:" + ex.Message.ToString();
                }
            }
            if (count == 0)
            {
                msg = "You have not specified a file.";
            }
            
            if (count != 0)
            {
                int refId = -1;
                var engine = FlowEngine.Init(processGuideLineId, userId, refId, "ProcessGuidelineWorkflowDocumentId");
                engine.SaveDocuments(requiredDocs);
                if (engine.documents != null && engine.documents.Count > 0)
                {
                    var uploadResult = new { message = msg, uuid = uuid.ToString() };
                    return Json(uploadResult, JsonRequestBehavior.AllowGet);
                }
            }

            var result = new { message = msg };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}