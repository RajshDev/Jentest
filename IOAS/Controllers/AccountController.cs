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
using System.Drawing;
using IOAS.Infrastructure;
using System.IO;
using IOAS.Filter;
using reCAPTCHA.MVC;

namespace IOAS.Controllers
{

    public class AccountController : Controller
    {
        [Authorized]
        [HttpGet]
        public ActionResult Role()
        {
            ViewBag.dept = AccountService.Getdepartment();
            return View();
        }

        [Authorized]
        [HttpPost]
        public ActionResult Role(RoleModel model)
        {
            try
            {
                ViewBag.dept = AccountService.Getdepartment();
                model.Createduser = User.Identity.Name;
                int Rolestatus = AccountService.Addrole(model);
                if (Rolestatus == 1)
                {
                    ViewBag.message = "Role name created successfully.";
                }
                else if (Rolestatus == 2)
                    ViewBag.Msg = "Role already exists.";
                else if (Rolestatus == 3)
                    ViewBag.update = "Role name Updated successfully.";
                else
                    ViewBag.error = "This smothing went to Error Please Contact your Admin.";

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.error = "This smothing went to Error Please Contact your Admin.";
                return View();
            }
        }
        [Authorized]
        [HttpGet]
        public ActionResult Department()
        {
            ViewBag.message = null;
            return View();
        }

        [Authorized]
        [HttpPost]
        public ActionResult Department(DepartmentModel model)
        {
            try
            {
                model.Createduser = User.Identity.Name;
                int status = AccountService.AddDepartment(model);
                if (status == 1)
                {
                    ViewBag.add = "Department name created successfully.";
                }
                else if (status == 2)
                    ViewBag.update = "Department name Updated successfully.";
                else if (status == 3)
                    ViewBag.message = "Department name already exists.";
                else
                    ViewBag.error = "This smothing went to Error Please Contact your Admin.";

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.error = "This smothing went to Error Please Contact your Admin.";
                return View();
            }
        }
        [Authorized]
        [HttpGet]
        public ActionResult AccessRights()
        {
            ViewBag.Function = AccountService.GetFunction();
            ViewBag.dept = AccountService.Getdepartment();
            return View();
        }

        [Authorized]
        [HttpPost]
        public ActionResult AccessRights(int Depertmentid)
        {

            ViewBag.Function = AccountService.GetFunction();
            ViewBag.dept = AccountService.Getdepartment();
            object output = AccountService.GetFunctionlist(Depertmentid);
            return Json(new { result = output });

        }

        [Authorized]
        [HttpPost]
        public ActionResult Rolelist(int Depertmentid)
        {
            object result = AccountService.GetFunctionlist(Depertmentid);
            return Json(result, JsonRequestBehavior.AllowGet);

        }


        [Authorized]
        [HttpPost]
        public ActionResult AccessRightsadd(List<Functionviewmodel> model)

        {
            List<Functionviewmodel> value = new List<Functionviewmodel>();
            value = AccountService.AddDepartmentrole(model);
            return Json(value);

        }

        [HttpGet]
        public ActionResult Login()
        {
            string test = User.Identity.Name;
            if (!string.IsNullOrWhiteSpace(test))
                return RedirectToAction("Dashboard", "Home");
            return View();
        }
        [HttpPost]
        //[CaptchaValidator]
        public ActionResult Login(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int UserId = AccountService.Logon(model);
                    if (UserId > 0)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, false);
                        string roles = Common.GetRoles(UserId);
                        var authTicket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.AddHours(4), false, roles);
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        HttpContext.Response.Cookies.Add(authCookie);
                        if (!String.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);
                        return RedirectToAction("Dashboard", "Home");
                    }
                    else
                    {
                        if (UserId == 0)
                        {
                            ViewBag.Msg = string.Format("The username or password provided is incorrect.");
                        }
                        if (UserId == -2)
                        {
                            ViewBag.Msg = string.Format("This Username expiry please contact Admin");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Msg = "Something went to wrong please contact administrator"; ;
                }
            }
            return View();
        }

        public JsonResult Verification(LogOnModel model)
        {
            try
            {
                model.UserName = User.Identity.Name;
                int UserId = AccountService.Logon(model);
                if (UserId > 0)
                {
                    string name = Common.GetUserFirstName(UserId);
                    return Json(new { firstName = name, userId = UserId }, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LogOff()
        {
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetExpires(DateTime.Now.AddDays(-1d));
            //Response.Cache.SetNoStore();
            //Response.Cookies.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
        [Authorized]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorized]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    String currentusername = User.Identity.Name;
                    changePasswordSucceeded = AccountService.ChangePasswordforuser(model, currentusername);
                }
                catch (Exception ex)
                {
                    //Infrastructure.UAYException.Instance.HandleMe(this, ex);
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {

            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                bool sendresetpassword;
                sendresetpassword = AccountService.UserForgotPassword(model);
                if (sendresetpassword)
                {
                    ViewBag.Msg = "We have emailed a new password to your email address. Please check and use it in your next login";
                    return View();
                }
                {
                    ViewBag.Msg = "The username and email address combination does not exist in our database";
                    return View();

                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //
        // GET: /Account/ChangePasswordSuccess
        [Authorized]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }


        [Authorized]
        [HttpGet]
        public ActionResult Createuser()
        {
            RegisterModel model = new RegisterModel();
            ViewBag.gender = AccountService.GetGender();
            ViewBag.dept = AccountService.Getdepartment();
            ViewBag.role = Common.GetRoleList();
            return View(model);
        }

        [Authorized]
        [HttpPost]
        public ActionResult Createuser(RegisterModel model, HttpPostedFileBase UserImage)
        {
            try
            {

                if (UserImage != null)
                {
                    Bitmap img = new Bitmap(UserImage.InputStream, false);
                    int height = img.Height;
                    int width = img.Width;
                    string ratio = Common.GetRatio(width, height);
                    var imageextensions = new[] { ".png", ".jpg", ".jpeg", ".PNG", ".JPG", ".JPEG" };
                    var ext = Path.GetExtension(UserImage.FileName);
                    if (!imageextensions.Contains(ext))
                    {
                        ModelState.AddModelError("", "Please upload any one of these type image [png, jpg, jpeg]");
                        return View();
                    }
                    else if (UserImage.ContentLength > 10485760)
                    {
                        ModelState.AddModelError("", "You can upload image up to 10 MB");
                        return View();
                    }
                    string logoName = System.IO.Path.GetFileName(UserImage.FileName);
                    var fileId = Guid.NewGuid().ToString();
                    logoName = fileId + "_" + logoName;
                    /*Saving the file in server folder*/
                    UserImage.SaveAs(Server.MapPath("~/Content/UserImage/" + logoName));
                    model.Image = logoName;
                }
                ViewBag.role = Common.GetRoleList();
                ViewBag.dept = AccountService.Getdepartment();
                ViewBag.gender = AccountService.GetGender();
                model.Createuser = User.Identity.Name;
                //if (ModelState.IsValid)
                //{
                var status = AccountService.UserRegistration(model);
                if (status == 1)
                {
                    ViewBag.message = "You have registered Successfully";
                }
                else if (status == 2)
                    ViewBag.Msg = "Username " + model.Username + " Already Exists";
                else if (status == 3)
                    ViewBag.update = "User updated successfully.";

                else
                    ViewBag.error = "Something went wrong please contact administrator";

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.message = "Something went wrong please contact administrator";
                return View();
            }
        }
        [Authorized]
        [HttpGet]
        public JsonResult GetCreateUserlist(RegisterModel model)
        {
            object output = AccountService.GetUserList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpGet]
        public JsonResult GetCheckUserList(string userName)
        {
            object output = AccountService.GetVerifyUsername(userName);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult EditUserlist(int UserId)
        {
            object output = AccountService.EditUserList(UserId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorized]
        [HttpPost]
        public JsonResult DeletUserlist(int UserId)
        {
            string Username = User.Identity.Name;
            object output = AccountService.Deleteuserlist(UserId, Username);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorized]
        [HttpGet]
        public JsonResult GetDepartmentlist()
        {
            object output = AccountService.GetDepartmentlist();
            return Json(output, JsonRequestBehavior.AllowGet);

        }

        [Authorized]
        [HttpPost]
        public ActionResult GetEditDepartmentlist(int DepartmentId)
        {

            object output = AccountService.GetEditDepartmentlist(DepartmentId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorized]
        [HttpPost]
        public ActionResult DeleteDepartment(int DepartmentId)
        {
            string Username = User.Identity.Name;
            int deletestatus = AccountService.Deletedepartment(DepartmentId, Username);
            if (deletestatus == 1)
            {
                object output = 1;
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            else if (deletestatus == 2)
            {
                object output = 2;
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            else
            {
                object output = -1;
                return Json(output, JsonRequestBehavior.AllowGet);
            }

        }

        [Authorized]
        [HttpGet]
        public ActionResult GetDepartmentrole(RoleModel model)
        {

            object output = AccountService.GetDepartmentRolelist(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorized]
        [HttpPost]
        public ActionResult GetEditRolelist(int RoleId)
        {

            object output = AccountService.GetEditRolelist(RoleId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorized]
        [HttpPost]
        public ActionResult Deleterolelist(int RoleId)
        {
            string Username = User.Identity.Name;
            int deleterolestatus = AccountService.Deleterole(RoleId, Username);
            if (deleterolestatus == 1)
            {
                object output = 1;
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            else if (deleterolestatus == 2)
            {
                object output = 2;
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            else
            {
                object output = -1;
                return Json(output, JsonRequestBehavior.AllowGet);
            }

        }

        [Authorized]
        [HttpPost]
        public ActionResult GetaddtionalRolelist(int Roleid)
        {

            object result = AccountService.GetaddtionalRole(Roleid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPIByDepartment(string Departmentid, string Instituteid)
        {
            var locationdata = AccountService.getPIList(Departmentid);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        [Authorized]
        [HttpPost]
        public ActionResult AccessRightsFill(int functionid, int Departmentid)
        {
            object output = AccountService.AccessRightsfunction(functionid, Departmentid);
            return Json(new { list = output });

        }
        public ActionResult AllocationHead()
        {
            ViewBag.acctgroup = Common.GetAccountGroup();
            return View();
        }
        [HttpPost]
        [Authorized]
        public ActionResult AllocationHead(AccountHeadViewModel model)
        {
            try
            {

                ViewBag.acctgroup = Common.GetAccountGroup();
                var Username = User.Identity.Name;
                model.userid = Common.GetUserid(Username);
                //if (model.userid == 1)
               // {
                    int status = AccountService.CreateAccontHead(model);

                    if (status == 4)
                     ViewBag.warrning = "please update tblAccountHeadandgroup";                    
                    else if (status == 1)
                    ViewBag.success = "Saved successfully";                    
                    else if (status == 2)
                        ViewBag.warrning = "The Account head name already exists";
                    else if (status == 3)
                        ViewBag.update = "updated successfully";
                    else
                        ViewBag.error = "Somthing went to worng please contact Admin!.";
               // }
               // else
              //  {
                   // ViewBag.UserMsg = "Your Not Authorised Add Account Head";
                //}
                return View();
            }
            catch (Exception ex)
            {
                var msg = ex;
                ViewBag.error = ex;
                return View();
            }
        }
        [Authorized]
        [HttpPost]
        public JsonResult GetAccoutHeadCode(int accountgroupcode)
        {
            object output = AccountService.GetAllocationHeadCode(accountgroupcode);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpGet]
        public JsonResult GetAccountHeadList(AccountHeadViewModel model)
        {
            object output = AccountService.AccountHeadList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult EditAccountHead(int headid)
        {
            try
            {
                object output = AccountService.AccountHeadEdit(headid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorized]
        [HttpPost]
        public JsonResult DeleteAccountHead(int headid)
        {
            try
            {
                var Username = User.Identity.Name;
                int userid = Common.GetUserid(Username);
                object output = AccountService.DeleteAccountHead(headid, userid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorized]
        [HttpGet]
        public ActionResult Projectstaff()
        {
            return View();
        }
        [Authorized]
        [HttpPost]
        public ActionResult Projectstaff(Projectstaffcategorymodel model)
        {
            try
            {
                var Username = User.Identity.Name;
                model.userid = Common.GetUserid(Username);
                int prostatus = AccountService.Projectstaff(model);
                if (prostatus == 1)
                {
                    ViewBag.success = "Saved successfully";
                }
                else if (prostatus == 2)
                    ViewBag.warrning = "The project staff name already exists";
                else if (prostatus == 3)
                    ViewBag.update = "updated successfully";
                else
                    ViewBag.error = "Somthing went to worng please contact Admin!.";
                return View();
            }
            catch (Exception ex)
            {
                var msg = ex;
                ViewBag.error = msg;
                return View();
            }
        }
        [Authorized]
        [HttpGet]
        public JsonResult Getprojectstafflist()
        {
            object output = AccountService.Getprojectlist();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult EditProjectstaffcate(int Projid)
        {
            try
            {

                object output = AccountService.Editprojectstaff(Projid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorized]
        [HttpPost]
        public JsonResult Deleteprojectstaff(int Projid)
        {
            try
            {
                var Username = User.Identity.Name;
                int userid = Common.GetUserid(Username);
                object output = AccountService.Deleteprojectstaff(Projid, userid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        [Authorized]
        public ActionResult Consultancyfundingcategory()
        {
            return View();
        }

        [Authorized]
        [HttpPost]
        public ActionResult Consultancyfundingcategory(ConsultancyFundingcategorymodel model)
        {
            try
            {
                var Username = User.Identity.Name;
                model.userid = Common.GetUserid(Username);
                int confunding = AccountService.consultancyfundingadd(model);
                if (confunding == 1)
                {
                    ViewBag.success = "Saved successfully";
                }
                else if (confunding == 2)
                    ViewBag.warrning = " Consultancyfunding already exists";
                else if (confunding == 3)
                    ViewBag.update = "updated successfully";
                else
                    ViewBag.error = "Somthing went to worng please contact Admin!.";
                return View();
            }
            catch (Exception ex)
            {
                var msg = ex;
                ViewBag.error = msg;
                return View();
            }
        }
        [Authorized]
        [HttpPost]
        public JsonResult Editconscategoryfunding(int fundingId)
        {
            try
            {

                object output = AccountService.EditConsfundingcategory(fundingId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorized]
        [HttpGet]
        public JsonResult GetConsfundinglist()
        {
            object output = AccountService.Getconsfundinglist();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult Deleteconsfundingcategory(int fundingId)
        {
            try
            {
                var Username = User.Identity.Name;
                int userid = Common.GetUserid(Username);
                object output = AccountService.Deleteconsfundingcategory(fundingId, userid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Authorized]
        public ActionResult ResetPassword()
        {
            try
            {
                ViewBag.Role = AccountService.GetRole();
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        [Authorized]
        public ActionResult ResetPassword(ResetPassword model)
        {
            try
            {
                ViewBag.Role = AccountService.GetRole();
                model.Username = User.Identity.Name;
                int resetstatus = AccountService.Resetpassword(model);
                if (resetstatus == 1)
                {
                    ViewBag.success = "Your password reset successfully";
                }
                else if (resetstatus == 2)
                {
                    ViewBag.failuer = "This user not in list";
                }
                else
                {
                    ViewBag.error = "Something went to wrong please contact Admin !.";
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.error = "Something went to wrong please contact Admin !.";
                return View();
            }
        }
        [Authorized]
        [HttpPost]
        public ActionResult Userlist(int Roleid)
        {
            object result = Common.GetUserlist(Roleid);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [Authorized]
        [HttpGet]
        public ActionResult Institute()
        {
            ViewBag.country = Common.getCountryList();
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="logoURL"></param>
        /// <returns></returns>
        [Authorized]
        [HttpPost]
        public ActionResult Institute(CreateInstituteModel model, HttpPostedFileBase logoURL)
        {
            try
            {
                var Username = User.Identity.Name;
                int userid = Common.GetUserid(Username);
                ViewBag.country = Common.getCountryList();
                //if (ModelState.IsValid)
                //{
                if (logoURL != null)
                {
                    Bitmap img = new Bitmap(logoURL.InputStream, false);
                    int height = img.Height;
                    int width = img.Width;
                    string ratio = Common.GetRatio(width, height);
                    var imageextensions = new[] { ".png", ".jpg", "jpeg", ".PNG", ".JPG", ".JPEG" };
                    var ext = Path.GetExtension(logoURL.FileName);
                    if (!imageextensions.Contains(ext))
                    {
                        ModelState.AddModelError("", "Please upload any one of these type image [png, jpg, jpeg]");
                        return View(model);
                    }
                    else if (logoURL.ContentLength > 10485760)
                    {
                        ModelState.AddModelError("", "You can upload image up to 10 MB");
                        return View(model);
                    }
                    string logoName = System.IO.Path.GetFileName(logoURL.FileName);
                    var fileId = Guid.NewGuid().ToString();
                    logoName = fileId + "_" + logoName;
                    /*Saving the file in server folder*/
                    logoURL.SaveAs(Server.MapPath("~/Content/InstituteLogo/" + logoName));
                    model.logo = logoName;
                }
                var status = AccountService.Institute(model, userid);
                if (status == 1)
                {
                    ViewBag.success = "Saved successfully";
                }
                else if (status == 2)
                    ViewBag.warrning = "The instituite name already exists";
                else if (status == 3)
                    ViewBag.update = "Institute updated successfully";
                else
                    ViewBag.error = "Somthing went to worng please contact Admin!.";
                // }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [Authorized]
        [HttpGet]
        public JsonResult GetInstuitelist()
        {
            object output = AccountService.Getinstitutelist();
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        [Authorized]
        [HttpPost]
        public ActionResult GetEditInstuitelist(int instituteid)
        {

            object output = AccountService.GetEditinstuite(instituteid);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult Deleteinstitute(int Instituteid)
        {
            string Username = User.Identity.Name;
            object output = AccountService.Deleteinstitute(Instituteid, Username);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public ActionResult Getdepartmentlisttype(int UsertypeId)
        {
            if (UsertypeId == 1)
            {
                object result = AccountService.Getdepartment();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                object result = null;//Common.getPIdepartment();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorized]
        [HttpGet]
        public ActionResult Createagency()
        {

            ViewBag.country = Common.getCountryList();
            ViewBag.schemes = Common.getschemes();
            ViewBag.projtype = Common.getsponprojectsubtype();
            ViewBag.state = Common.GetStatelist();
            ViewBag.agycounty = Common.GetAgencyType();
            ViewBag.indcategory = Common.GetIndianAgencyCategory();
            ViewBag.NONSEZ = Common.GetNonSEZCategory();
            ViewBag.agencydoc = Common.GetAgencyDocument();
            ViewBag.projecttype = Common.getprojecttype();
            ViewBag.Company = Common.GetCompanyType();
            ViewBag.gov = Common.GetGovermentAgy();
            return View();

        }
        [Authorized]
        [HttpPost]
        public ActionResult Createagency(AgencyModel model, HttpPostedFileBase File)
        {
            try
            {
                ViewBag.country = Common.getCountryList();
                ViewBag.schemes = Common.getschemes();
                ViewBag.projtype = Common.getsponprojectsubtype();
                ViewBag.state = Common.GetStatelist();
                ViewBag.agycounty = Common.GetAgencyType();
                ViewBag.indcategory = Common.GetIndianAgencyCategory();
                ViewBag.NONSEZ = Common.GetNonSEZCategory();
                ViewBag.agencydoc = Common.GetDocTypeList(19);
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.Company = Common.GetCompanyType();
                ViewBag.gov = Common.GetGovermentAgy();
                var Username = User.Identity.Name;
                model.UserId = Common.GetUserid(Username);
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
                    var status = AccountService.Agencyregistration(model);
                    if (status == 1)
                    {
                        ViewBag.success = "Saved successfully";
                        ViewBag.agencycode = model.AgencyCode;
                    }
                    else if (status == 2)
                        ViewBag.Msg = "This agency name already exists";
                    else if (status == 3)
                        ViewBag.update = "Agency updated successfully";
                    //else if (status == 4)
                    //    ViewBag.agyCode = "Agency code already exists";
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
            catch (Exception ex)
            {
                var Msg = ex;
                ViewBag.error = Msg;
                return View();
            }
        }
        [Authorized]
        [HttpGet]
        public JsonResult Getagencylist(AgencyModel model)
        {
            object output = AccountService.Getagenctlist(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult GetCheckAgencycode(string agyCode)
        {
            object output = AccountService.CheckAgencyCode(agyCode);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpGet]
        public JsonResult GetAgencyCode()
        {
            object output = AccountService.GetAgencyCode();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public ActionResult GetEditagency(int Agencyid)
        {

            object output = AccountService.Geteditagency(Agencyid);
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
        public ActionResult deleteagency(int Agencyid)
        {
            string Username = User.Identity.Name;
            object output = AccountService.Deleteagency(Agencyid, Username);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult GetStateCode(int StateId)
        {
            object output = AccountService.Getstatecode(StateId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpGet]
        public ActionResult Schemes()
        {
            ViewBag.projtype = Common.getprojecttype();
            return View();
        }
        [Authorized]
        [HttpPost]
        public ActionResult Schemes(Schemeviewmodel model)
        {
            try
            {
                ViewBag.projtype = Common.getprojecttype();
                var Username = User.Identity.Name;
                model.userId = Common.GetUserid(Username);
                int schemestatus = AccountService.createscheme(model);
                if (schemestatus == 1)
                {
                    ViewBag.success = "Saved successfully";
                }
                else if (schemestatus == 2)
                    ViewBag.Msg = "This Scheme name already exists";
                else if (schemestatus == 3)
                    ViewBag.update = "Scheme updated successfully";
                else
                    ViewBag.error = "Somthing went to worng please contact Admin!.";
                return View();

            }
            catch (Exception ex)
            {
                ViewBag.error = ex;
                return View();
            }
        }
        [Authorized]
        [HttpGet]
        public JsonResult Getschemelist()
        {
            object output = AccountService.Getschemelist();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public ActionResult Editscheme(int schemeid)
        {

            object output = AccountService.Editscheme(schemeid);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public ActionResult deletescheme(int schemeid)
        {
            string Username = User.Identity.Name;
            object output = AccountService.Deletescheme(schemeid, Username);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadSponPrjctsubtypebytype(string typeid)
        {
            typeid = typeid == "" ? "0" : typeid;
            var locationdata = AccountService.getsponprojectsubtype(Convert.ToInt32(typeid));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadConsPrjctsubtypebytype(string typeid)
        {
            typeid = typeid == "" ? "0" : typeid;
            var locationdata = AccountService.getconsprojectsubtype(Convert.ToInt32(typeid));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadAgencycodebyagency(string agencyid)
        {
            agencyid = agencyid == "" ? "0" : agencyid;
            var locationdata = AccountService.getagencycode(Convert.ToInt32(agencyid));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [HttpGet]
        public ActionResult PrentGroupList()
        {
            object result = Common.Parentaccountgroup();
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        [Authorize]
        [HttpGet]
        public ActionResult AccountGroup()
        {
            ViewBag.account = Common.GetAccounttype();
            // ViewBag.paraccount = Common.Parentaccountgroup();
            return View();
        }
        [HttpPost]
        [Authorized]
        public ActionResult AccountGroup(Accountgroupmodel model)
        {
            try
            {
                ViewBag.account = Common.GetAccounttype();
                //ViewBag.paraccount = Common.Parentaccountgroup();
                var Username = User.Identity.Name;
                model.userid = Common.GetUserid(Username);
                var status = AccountService.Accountgroup(model);
                if (status == 1)
                {
                    ViewBag.success = "Saved successfully";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else if (status == 2)
                {
                    ViewBag.Msg = "This Account group already exists";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else if (status == 3)
                {
                    ViewBag.update = "Account group updated successfully";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ViewBag.error = "Somthing went to worng please contact Admin!.";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var Msg = ex;
                return Json(Msg, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorized]
        [HttpGet]
        public JsonResult GetAccountgrouplist(Accountgroupmodel model)
        {
            object output = AccountService.Getaccountgrouplist(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        //[Authorized]
        //[HttpGet]
        //public ActionResult Editaccontgroup(int acccountgrpId)
        //{

        //    object output = AccountService.Editaccountgroup(acccountgrpId);
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}
        [Authorized]
        [HttpPost]
        public ActionResult DeleteAccountGroup(int acccountgrpId)
        {
            string Username = User.Identity.Name;
            object output = AccountService.DeleteAccountgroup(acccountgrpId, Username);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult Getaccounttypecode(int accttypeid)
        {
            object output = AccountService.Accountgroupcode(accttypeid);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult Getparentgroupcode(int parentgrpId)
        {
            object output = AccountService.Parentgroupcode(parentgrpId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult AddSubGroupCode(int accountgrpId)
        {
            object output = AccountService.AccountSubGroupAdd(accountgrpId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpGet]
        public ActionResult SRBItemcategory()
        {

            return View();
        }
        [Authorized]
        [HttpPost]
        public ActionResult SRBItemcategory(SRBItemcategory model)
        {
            try
            {

                var Username = User.Identity.Name;
                model.userid = Common.GetUserid(Username);
                int srbitemstatus = AccountService.AddSRBitemcategory(model);
                if (srbitemstatus == 1)
                {
                    ViewBag.success = "Saved successfully";
                }
                else if (srbitemstatus == 2)
                    ViewBag.Msg = "This SRB Item name already exists";
                else if (srbitemstatus == 3)
                    ViewBag.update = "SRB item name updated successfully";
                else
                    ViewBag.error = "Somthing went to worng please contact Admin!.";
                return View();

            }
            catch (Exception ex)
            {
                ViewBag.error = ex;
                return View();
            }
        }
        [Authorized]
        [HttpGet]
        public JsonResult SRBItemcategorylist()
        {
            object output = AccountService.SRBcategorylist();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult editsrbitemcategory(int srbitmcateid)
        {
            object output = AccountService.Editsrbitemcategory(srbitmcateid);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public ActionResult deletesrbcategory(int srbitmcateid)
        {
            string Username = User.Identity.Name;
            object output = AccountService.deletesrbitemcategory(srbitmcateid, Username);
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadAgencybysubtype(string subtypeid)
        {
            subtypeid = subtypeid == "" ? "0" : subtypeid;
            var locationdata = AccountService.getagency(Convert.ToInt32(subtypeid));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPrjctcategorybytype(string typeid)
        {
            typeid = typeid == "" ? "0" : typeid;
            var locationdata = new List<MasterlistviewModel>();
            if (typeid == "1")
            {
                locationdata = AccountService.getcategory(Convert.ToInt32(typeid));
            }
            else if (typeid == "2")
            {
                locationdata = AccountService.getConsultancycategory(Convert.ToInt32(typeid));
            }
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPIEmailById(string Profid)
        {
            Profid = Profid == "" ? "0" : Profid;
            var locationdata = AccountService.getpiemail(Convert.ToInt32(Profid));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadSchemeCodeById(string SchemeId)
        {
            SchemeId = SchemeId == "" ? "0" : SchemeId;
            var locationdata = AccountService.getschemecode(Convert.ToInt32(SchemeId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Loadcategorybytype(string typeid)
        {
            typeid = typeid == "" ? "0" : typeid;
            var locationdata = AccountService.getcategorybyprojecttype(Convert.ToInt32(typeid));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult CommitmentList()
        {
            return View();
        }

        public ActionResult Commitment(int CommitmentId = 0)
        {
            CommitmentModel model = new CommitmentModel();
            ViewBag.CommitmentType = Common.getCommitmentType();
            ViewBag.Purpose = Common.getPurpose();
            ViewBag.Currency = Common.getFRMcurrency();
            ViewBag.BudgetHead = Common.getBudgetHead();
            ViewBag.Employee = Common.GetEmployeeName();
            ViewBag.AccountHead = Common.getBudgetHead();
            ViewBag.ProjectNo = Common.getProjectNumber();
            ViewBag.Vendor = Common.getVendor();
            ViewBag.RequestRef = Common.getprojectsource();
            ViewBag.RefNo = new List<MasterlistviewModel>();
            ViewBag.SubheadList =
            ViewBag.FundingBody = Common.GetFundingBody(0);
            model.CommitmentNo = "0";
            model.commitmentValue = 0;
            model.currencyRate = 0;
           
            
            if (CommitmentId > 0)
            {
                model = AccountService.getEditCommitDetails(CommitmentId);
                model.doc = Common.GetProjectDocument(model.SelProjectNumber);
                if (model.selRequestRefrence == 1)
                {
                    ViewBag.RefNo = Common.GetWorkflowRefNumberList();
                }
                else
                {
                    int depId = Common.GetDepartmentId(User.Identity.Name);
                    ViewBag.RefNo = Common.GetTapalRefNumberList(depId);
                }
                var subHead = Common.getSubheadList(model.SelProjectNumber, model.selAllocationHead);
                if (!String.IsNullOrEmpty(model.selSubhead) && !subHead.Any(m => m.name.Contains(model.selSubhead)))
                    subHead.Add(new MasterlistviewModel()
                    {
                        name = model.selSubhead,
                        code = model.selSubhead
                    });
                ViewBag.SubheadList = subHead;
            }
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public ActionResult Commitment(CommitmentModel model)
        {
            try
            {
                ViewBag.CommitmentType = Common.getCommitmentType();
                ViewBag.Purpose = Common.getPurpose();
                ViewBag.Currency = Common.getFRMcurrency();
                ViewBag.BudgetHead = Common.getBudgetHead();
                ViewBag.ProjectType = Common.getprojecttype();
                ViewBag.AccountHead = Common.getBudgetHead();
                ViewBag.Employee = Common.GetEmployeeName();
                var Data = Common.getProjectNo(model.selProjectType ?? 0);
                ViewBag.ProjectNo = Data.Item1;
                ViewBag.Vendor = Common.getVendor();
                ViewBag.SubheadList =
                ViewBag.FundingBody = Common.GetFundingBody(model.SelProjectNumber);
                ViewBag.RequestRef = Common.getprojectsource();
                ViewBag.RefNo = new List<MasterlistviewModel>();
              
                var subHead = Common.getSubheadList(model.SelProjectNumber, model.selAllocationHead);
                if (!String.IsNullOrEmpty(model.selSubhead) && !subHead.Any(m => m.name.Contains(model.selSubhead)))
                    subHead.Add(new MasterlistviewModel()
                    {
                        name = model.selSubhead,
                        code = model.selSubhead
                    });
                ViewBag.SubheadList = subHead;
                var UserId = Common.GetUserid(User.Identity.Name);
                AccountService _AS = new AccountService();
                int result = 0;
                if (ModelState.IsValid)
                {
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                    if (model.file != null)
                    {
                        string docname = Path.GetFileName(model.file.FileName);
                        var docextension = Path.GetExtension(docname);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            ViewBag.errMsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    var data = _AS.SaveCommitDetails(model, UserId);
                    result = data.Item1;
                    if (result > 0 && model.commitmentId == null)
                    {
                        TempData["succMsg"] = "Saved successfully";
                        return RedirectToAction("CommitmentList", "Account");
                    }
                    else if (result > 0 && model.commitmentId > 0)
                    {
                        TempData["succMsg"] = "Updated successfully";
                        return RedirectToAction("CommitmentList", "Account");
                    }
                    //else if (result == -1)
                    //{
                    //    ViewBag.ValidationMsg = "Commitment value cannot allow above allocation value";
                    //}
                    //else if (result == -3)
                    //{
                    //    ViewBag.ValidationMsg = "Allocation value cannot allow above total allocation value";
                    //}
                    //else if (result == -5)
                    //{
                    //    ViewBag.ValidationMsg = "Allocation value cannot allow above sanctioned value";
                    //}
                    //else if (result == -6 || result == -2 || result == -4)
                    //{
                    //    ViewBag.ValidationMsg = "Balance Commitment allowed for this year";
                    //}
                    else
                    {
                        TempData["errMsg"] = data.Item2;
                        return RedirectToAction("CommitmentList", "Account");
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));

                    ViewBag.ValidationMsg = messages;
                }
                model.CommitmentNo = model.CommitmentNo;
                ProjectService _PS = new ProjectService();
                model.prjDetails = _PS.getProjectSummary(model.SelProjectNumber);
                model.AllocationDtls = Common.getAllocationValue(model.SelProjectNumber, model.selAllocationHead);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["errMsg"] = "Something went wrong please contact administrator";
                return RedirectToAction("CommitmentList", "Account");
            }
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadProjectNumber(int projecttype)
        {
            var projectData = Common.getProjectNo(Convert.ToInt32(projecttype));
            return Json(projectData, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadCurrencyRate(string Currency)
        {
            var projectData = Common.getExchangeRate(Currency);
            return Json(projectData, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetFundingBody(int ProjectID)
        {
            var result = Common.GetFundingBody(ProjectID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllocationBasedProject(int ProjectId)
        {
            var Data = Common.getAllocationHeadBasedOnProject(ProjectId);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadProjectDetails(int ProjectId)
        {
            ProjectService _PS = new ProjectService();
            var projectData = _PS.getProjectSummary(Convert.ToInt32(ProjectId));
            var Data = Common.getAllocationHeadBasedOnProject(ProjectId);
            var PrjTypeId = Common.getProjecTypeId(projectData.ProjectType);
            var Data1 = Common.getProjectNo(PrjTypeId);
            var Copi = Common.GetProjectsDetails(ProjectId);
            var CommitNo = Data1.Item2;
            var doc = Common.GetProjectDocument(ProjectId);
            var result = new { projectData = projectData, Data = Data, CommitNo = CommitNo, Copi = Copi, doc = doc };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCommitmentDetails()
        {
            object output = AccountService.GetCommitmentDetails();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetCommitmentDetails(CommitmentSearchModel model, int pageIndex, int pageSize, DateFilterModel CreatedDate)
        {
            object output = AccountService.GetCommitmentDetails(model, pageIndex, pageSize, CreatedDate);
            return Json(output, JsonRequestBehavior.AllowGet);
        }




        [Authorized]
        [HttpPost]
        public ActionResult getEditCommitmentDetails(int CommitmentId)
        {
            object output = AccountService.getEditCommitDetails(CommitmentId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        public ActionResult SubmitCommitment(int CommitmentId, decimal commitVal)
        {
            var UserId = Common.GetUserid(User.Identity.Name);
            var output = AccountService.ActiveCommitment(CommitmentId, UserId, commitVal);
            if (output == 1)
            {
                TempData["succMsg"] = "Successfully activated";
                return RedirectToAction("CommitmentList", "Account");
            }
            else
            {
                TempData["errMsg"] = "Something went wrong please contact administrator";
                return RedirectToAction("CommitmentList", "Account");
            }
            //return Json(output, JsonRequestBehavior.DenyGet);
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

        [Authorize]
        public ActionResult ActiveCommitments()
        {
            CommitSearchFieldModel model = new CommitSearchFieldModel();
            ViewBag.ProjectType = Common.getprojecttype();
            var Data = Common.getProjectNo(model.ProjectType ?? 0);
            ViewBag.ProjectNo = Data.Item1;
            return View();
        }
        [HttpGet]
        public JsonResult GetCommitDetails()
        {
            object output = AccountService.GetActiveCommitmentDetails();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public ActionResult ActiveCommitments(string Keyword,int ProjectType,int ProjectNo,DateTime FromDate,DateTime ToDate)
        //{
        //    CommitmentStatusModel model = new CommitmentStatusModel();
        //    List<CommitmentResultModel> List = new List<CommitmentResultModel>();
        //    CommitSearchFieldModel Search = new CommitSearchFieldModel();
        //        Search.Keyword = Keyword;
        //        Search.ProjectType = ProjectType;
        //        Search.ProjectNumber = ProjectNo;
        //        Search.FromCreatedDate = FromDate;
        //    Search.ToCreatedDate = ToDate;
        //    ViewBag.ProjectType = Common.getprojecttype();
        //    var Data = Common.getProjectNo(Search.ProjectType ?? 0);
        //    ViewBag.ProjectNo = Data.Item1;
        //    List = AccountService.SearchActiveCommitmentDetails(Search);
        //    model.searchField = Search;
        //    model.getDetails = List;
        //    return PartialView(model);
        //}

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
        public ActionResult PopupCommitment(int ComitmentId, string Status, int Action)
        {
            try
            {
                CommitmentResultModel model = new CommitmentResultModel();
                AccountService _AS = new AccountService();
                ViewBag.Reason = Common.GetCommitmentAction();
                ViewBag.LogType = Common.GetCodeControlList("AddandRevCommit");
                model = _AS.getPopViewCommitDetails(ComitmentId, Status);
                model.Action = Action;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }
        //[HttpPost]
        //public ActionResult PopupUpdateCommitStatus(int CommitmentID, int Action)
        //{
        //    try
        //    {
        //        CommitmentUpdateStatusModel model = new CommitmentUpdateStatusModel();
        //        model.CommitmentId = CommitmentID;
        //        model.Action = Action;
        //        return PartialView(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new EmptyResult();
        //    }
        //}

        [Authorized]
        [HttpPost]
        public ActionResult CloseCommitment(CommitmentResultModel model)
        {
            var UserId = Common.GetUserid(User.Identity.Name);
            if (model.LogTypeId == 1)
            {
                var validation = Common.ValidateCommitment(model.ProjectId, model.AllHeadId, model.AddCloseAmt);
                if (validation != "Valid")
                {
                    TempData["errMsg"] = validation;
                    return RedirectToAction("ActiveCommitments");
                }
                else if (Common.IsInUcCommitment(model.ComitmentId))
                {
                    TempData["errMsg"] = "You can't add any addition value to this commitment. Becouse this commitment treated as expenditure in UC.";
                    return RedirectToAction("ActiveCommitments");
                }
            }
            int output = AccountService.CloseThisCommitment(model, UserId);
            if (output > 0)
            {
                TempData["succMsg"] = "Commitment has been upadated successfully.";
                return RedirectToAction("ActiveCommitments");

            }
            else if (output == -1)
            {
                TempData["errMsg"] = "Reversal Amount should not be greater than Balance Amount";
                return RedirectToAction("ActiveCommitments");
            }
            else if (output == -2)
            {
                TempData["errMsg"] = "This Commitment has used for some other bills.";
                return RedirectToAction("ActiveCommitments");
            }
            else
            {
                TempData["errMsg"] = "something went wrong";
                return RedirectToAction("ActiveCommitments");
            }

        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllocationValue(int ProjectID, int AllocationID)
        {
            var allocData = Common.getAllocationValue(ProjectID, AllocationID);
            allocData.subheadList = Common.getSubheadList(ProjectID, AllocationID);
            return Json(allocData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SearchActiveCommitments(CommitSearchFieldModel model, int pageIndex, int pageSize)
        {
            CommitSearchFieldModel Search = new CommitSearchFieldModel();
            //Search.Keyword = Keyword;
            //Search.ProjectType = ProjectType;
            //Search.ProjectNumber = ProjectNo;
            //Search.FromCreatedDate = FromDate;
            //Search.ToCreatedDate = ToDate;
            object output = AccountService.SearchActiveCommitmentDetails(model, pageIndex, pageSize);
            //object output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Getagencylist(AgencySearchModel model, int pageIndex, int pageSize)
        {
            object output = AccountService.Getagenctlist(model, pageIndex, pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        
    }
}