using IOAS.DataModel;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.IO;
using System.Net.Http;
using System.Globalization;
using System.Web.Configuration;
using System.Data;

namespace IOAS.GenericServices
{
    public class AccountService
    {
        private static readonly Object ClsCommitlockObj = new Object();
        /// <summary>
        /// This method when user login check user name and password 
        /// </summary>
        /// <param name="logon"></param>
        /// <returns></returns>
        public static int Logon(LogOnModel logon)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    String Encpassword = Cryptography.Encrypt(logon.Password, "LFPassW0rd");
                    var userquery = context.tblUser.SingleOrDefault(dup => dup.UserName == logon.UserName && dup.Password == Encpassword && dup.Status == "Active");
                    var userexpiry = context.tblUser.SingleOrDefault(exp => exp.UserName == logon.UserName && exp.Password == Encpassword && exp.Status == "Active" && exp.ExpiryDate < DateTime.Now);

                    if (userquery != null)
                    {
                        var Loggedin = context.tblLoginDetails.OrderByDescending(l => l.LoginTime).FirstOrDefault(l => l.UserId == userquery.UserId);
                        if (Loggedin != null)
                            if (Loggedin.isLoggedIn == true)
                                return -3;
                        if (userexpiry != null)
                            return -2;
                        tblLoginDetails log = new tblLoginDetails();
                        log.UserId = userquery.UserId;
                        log.LoginTime = DateTime.Now;
                        log.isLoggedIn = true;
                        context.tblLoginDetails.Add(log);
                        context.SaveChanges();
                        return userquery.UserId;

                    }

                    else
                    {
                        return 0;
                    }

                }
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
 (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);

                return -1;
            }
        }
        public static bool setLogin(string userName)
        {
            using (var context = new IOASDBEntities())
            {
                var user = context.tblUser.SingleOrDefault(dup => dup.UserName.ToLower() == userName.ToLower() && dup.Status == "Active");

                if (user != null)
                {
                    var userloggedIn = context.tblLoginDetails.OrderByDescending(log => log.LoginTime).FirstOrDefault(log => log.UserId == user.UserId);
                    if (userloggedIn != null)
                    {
                        if (userloggedIn.isLoggedIn == true)
                        {
                            userloggedIn.isLoggedIn = false;
                            context.SaveChanges();
                        }
                    }
                }
            }
            return true;
        }
        public static bool ChangePasswordforuser(ChangePasswordModel model, String username)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var oldpassword = Cryptography.Encrypt(model.OldPassword, "LFPassW0rd");
                    var userquery = context.tblUser.SingleOrDefault(dup => dup.UserName == username && dup.Password == oldpassword);

                    if (userquery != null)
                    {
                        userquery.Password = Cryptography.Encrypt(model.NewPassword, "LFPassW0rd"); ;
                        context.SaveChanges();
                        context.Dispose();
                        return true;

                    }

                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        /// <summary>
        /// This method used for user forgetpassword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UserForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {

                    var userquery = context.tblUser.SingleOrDefault(dup => dup.Email == model.Email);

                    //if (userquery != null)
                    //{
                    //    string temppass = userquery.Password;
                    //    userquery.Password = Cryptography.Decrypt(temppass, "LFPassW0rd");


                    //    if (userquery.Password != null)
                    //    {

                    //        model.password = userquery.Password;

                    //    }


                    //    //context.SaveChanges();
                    //    //context.Dispose();


                    //}
                    if (userquery != null)
                    {
                        string temppass = Guid.NewGuid().ToString().Substring(0, 8);
                        userquery.Password = Cryptography.Encrypt(temppass, "LFPassW0rd");


                        var Disclaimer = EmailTemplate.disclaimer;
                        using (MailMessage mm = new MailMessage(EmailTemplate.mailid, model.Email))
                        {
                            mm.Subject = "IOAS Website Account Password";
                            string body = "Hello " + userquery.UserName + ",";
                            body += "<br /><br />Your account password has been reset successfully. Please use the below password to log into the system";
                            //body += "<br /><a href = '" + Request.Url.AbsoluteUri.Replace("Jobseekers.aspx", "CS_Activation.aspx?ActivationCode=" + activationCode) + "'>Click here to activate your account.</a>";
                            body += "<br />Your new password is " + temppass;
                            body += "<br /><br />Thanks";
                            body += "<br /><br />________________________________________________________________________________________________________________";
                            body += "<br /><br />*** This is an automatically generated email, please do not reply ***";
                            body += "<br /><br />" + Disclaimer;
                            mm.Body = body;
                            mm.IsBodyHtml = true;

                            using (SmtpClient smtp = new SmtpClient(EmailTemplate.smtpAddress, EmailTemplate.portNumber))
                            {
                                smtp.Credentials = new NetworkCredential(EmailTemplate.mailid, EmailTemplate.password);
                                //smtp.Credentials = new NetworkCredential("info@crescentglobal.com", "ofni963");
                                //smtp.EnableSsl = EmailTemplate.enableSSL;
                                smtp.Send(mm);
                            }
                        }




                        context.SaveChanges();
                        context.Dispose();
                        return true;

                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {

                return true;
            }

        }
        /// <summary>
        /// This method to load in user list grid
        /// </summary>
        /// <returns>user details in list</returns>
        public static List<UserResultModels> GetUserList(RegisterModel model)
        {
            try
            {
                List<UserResultModels> Userlist = new List<UserResultModels>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from U in context.tblUser
                                 from D in context.tblDepartment
                                 from R in context.tblRole
                                 where D.DepartmentId == U.DepartmentId && U.Status == "Active" && U.RoleId == R.RoleId
                                  && (U.DepartmentId == model.SearchDeptId || model.SearchDeptId == null)
                                 && (U.RoleId == model.SearchRoleId || model.SearchRoleId == null)
                                 && (U.FirstName.Contains(model.SearchName) || model.SearchName == null)
                                 select new { U.UserId, U.FirstName, U.LastName, U.RoleId, R.RoleName, U.DepartmentId, U.Email, D.DepartmentName, U.UserName, U.UserImage }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {

                            Userlist.Add(new UserResultModels
                            {
                                Sno = i + 1,
                                Userid = query[i].UserId,
                                Username = query[i].UserName,
                                Firstname = query[i].FirstName + query[i].LastName,
                                RoleId = query[i].RoleId,
                                RoleName = query[i].RoleName,
                                DepartmentId = Convert.ToInt32(query[i].DepartmentId),
                                DepartmentName = query[i].DepartmentName,
                                Image = query[i].UserImage
                            });
                        }
                    }
                    return Userlist;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static int GetVerifyUsername(string userName)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {

                    var query = (from U in context.tblUser
                                 where U.UserName == userName && U.Status == "Active" && U.RoleId != 7
                                 select U).FirstOrDefault();
                    if (query != null)
                    {
                        return 1;
                    }

                }
                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }


        /// <summary>
        /// edit option click in user details to get user details
        /// </summary>
        /// <param name="UserId">This parameter used for single user user Id</param>
        /// <returns>To get Single user details</returns>
        public static RegisterModel EditUserList(int UserId)
        {
            try
            {
                RegisterModel editUserlist = new RegisterModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from U in context.tblUser
                                 from D in context.tblDepartment
                                 from G in context.tblCodeControl
                                 from R in context.tblRole
                                 where (D.DepartmentId == U.DepartmentId && U.UserId == UserId && G.CodeValAbbr == U.Gender && G.CodeName == "Gender" && U.RoleId == R.RoleId)
                                 select new { U.UserId, U.FirstName, U.LastName, U.RoleId, R.RoleName, U.DepartmentId, U.Email, D.DepartmentName, U.UserName, G.CodeValDetail, G.CodeValAbbr, U.ExpiryDate }).FirstOrDefault();
                    var userrolequery = (from Ur in context.tblUserRole
                                         from R in context.tblRole
                                         where (Ur.UserId == UserId && R.RoleId == Ur.RoleId)
                                         select new { Ur.RoleId, R.RoleName }).ToList();
                    if (query != null)
                    {
                        editUserlist.Firstname = query.FirstName;
                        editUserlist.Lastname = query.LastName;
                        editUserlist.Gender = query.CodeValAbbr;
                        editUserlist.ExpiryDateof = String.Format("{0:dd}", (DateTime)query.ExpiryDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.ExpiryDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.ExpiryDate);
                        //editUserlist.Dateofbirth = query.Dateofbirth;
                        editUserlist.Department = query.DepartmentId;
                        editUserlist.RoleId = query.RoleId;
                        editUserlist.Username = query.UserName;
                        editUserlist.UserId = query.UserId;
                        editUserlist.Email = query.Email;

                    }
                    if (userrolequery.Count > 0)
                    {
                        int[] _roleid = new int[userrolequery.Count];
                        string[] _rolename = new string[userrolequery.Count];
                        for (int i = 0; i < userrolequery.Count; i++)
                        {
                            _roleid[i] = (Int32)userrolequery[i].RoleId;
                            _rolename[i] = userrolequery[i].RoleName;
                        }
                        editUserlist.SelectedRoles = _roleid;
                        editUserlist.SelectedRolesName = _rolename;
                    }
                    return editUserlist;
                }
            }
            catch (Exception ex)
            {
                RegisterModel editUserlist = new RegisterModel();
                return editUserlist;
            }
        }
        /// <summary>
        /// Add new Department in table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddDepartment(DepartmentModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    tblDepartment objdept = new tblDepartment();
                    if (model.Departmentid == 0)
                    {
                        var objdepts = context.tblDepartment.Where(m => m.DepartmentName == model.Departmentname && m.Status == "Active").FirstOrDefault();
                        if (objdepts != null)
                        {
                            model.Departmentname = null;
                            return 3;
                        }
                        else
                        {
                            string Username = model.Createduser;
                            objdept.DepartmentName = model.Departmentname;
                            objdept.HOD = model.HOD;
                            objdept.Status = "Active";
                            objdept.CreatedUserId = Common.GetUserid(Username);
                            objdept.CreatedTS = DateTime.Now;
                            context.tblDepartment.Add(objdept);
                            context.SaveChanges();
                            return 1;
                        }
                    }
                    else
                    {
                        objdept = context.tblDepartment.Where(m => m.DepartmentId == model.Departmentid && m.Status == "Active").FirstOrDefault();
                        if (objdept != null)
                        {
                            string Username = model.Createduser;
                            objdept.DepartmentName = model.Departmentname;
                            objdept.HOD = model.HOD;
                            objdept.Status = "Active";
                            objdept.UpdatedTS = DateTime.Now;
                            objdept.LastUpdateUserId = Common.GetUserid(Username);
                            context.SaveChanges();
                        }
                        context.Dispose();
                        return 2;
                    }

                }

            }
            catch (Exception ex)
            {
                var Msg = ex;
                return -1;
            }
        }
        /// <summary>
        /// To load department in dropdownlist
        /// </summary>
        /// <returns></returns>
        public static List<RoleModel> Getdepartment()
        {
            try
            {
                List<RoleModel> dept = new List<RoleModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from d in context.tblDepartment
                                 orderby d.DepartmentName
                                 where (d.Status == "Active")
                                 select new { d.DepartmentId, d.DepartmentName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            dept.Add(new RoleModel
                            {

                                Departmentid = query[i].DepartmentId,
                                Departmentname = query[i].DepartmentName
                            });
                        }
                    }
                }
                return dept;
            }
            catch (Exception ex)
            {
                List<RoleModel> dept = new List<RoleModel>();
                return dept;
            }
        }
        /// <summary>
        /// To load and get role details using dropdown
        /// </summary>
        /// <returns></returns>
        public static List<RoleModel> GetRole()
        {
            try
            {
                List<RoleModel> role = new List<RoleModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from R in context.tblRole
                                 where (R.Status == "Active")
                                 orderby R.RoleName
                                 select new { R.RoleId, R.RoleName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            role.Add(new RoleModel
                            {

                                Roleid = query[i].RoleId,
                                Rolename = query[i].RoleName
                            });
                        }
                    }
                }
                return role;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// To create new Role in table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Addrole(RoleModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    tblRole objrole = new tblRole();
                    if (model.Roleid == 0)
                    {
                        var objroles = context.tblRole.Where(m => m.RoleName == model.Rolename && m.Status == "Active").FirstOrDefault();
                        if (objroles != null)
                        {
                            model.Rolename = null;
                            return 2;
                        }
                        else
                        {
                            string Username = model.Createduser;
                            objrole.RoleName = model.Rolename;
                            objrole.DepartmentId = model.Departmentid;
                            objrole.CreatedUserId = Common.GetUserid(Username);
                            objrole.CreatedTS = DateTime.Now;
                            objrole.UpdatedTS = DateTime.Now;
                            objrole.Status = "Active";
                            context.tblRole.Add(objrole);
                            context.SaveChanges();
                            return 1;
                        }
                    }
                    else
                    {
                        objrole = context.tblRole.Where(m => m.RoleId == model.Roleid).FirstOrDefault();
                        if (objrole != null)
                        {
                            string Username = model.Createduser;
                            objrole.RoleName = model.Rolename;
                            objrole.DepartmentId = model.Departmentid;
                            objrole.LastUpdateUserId = Common.GetUserid(Username);
                            objrole.UpdatedTS = DateTime.Now;
                            objrole.Status = "Active";
                            context.SaveChanges();
                        }
                        context.Dispose();
                        return 3;
                    }

                }

            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        /// <summary>
        /// Get additional roles to load in create user form
        /// </summary>
        /// <param name="Roleid">To pass RoleId</param>
        /// <returns></returns>
        public static List<RoleModel> GetaddtionalRole(int Roleid)
        {
            try
            {
                List<RoleModel> role = new List<RoleModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from R in context.tblRole
                                 orderby R.RoleName
                                 where (R.RoleId != Roleid && R.Status == "Active")
                                 select new { R.RoleId, R.RoleName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            role.Add(new RoleModel
                            {

                                Roleid = query[i].RoleId,
                                Rolename = query[i].RoleName
                            });
                        }
                    }
                }
                return role;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Get function list in table
        /// </summary>
        /// <returns>To Fill in dropdown list</returns>
        public static List<Functionlistmodel> GetFunction()
        {
            try
            {
                List<Functionlistmodel> Function = new List<Functionlistmodel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from F in context.tblFunction
                                 orderby F.FunctionName
                                 select new { F.FunctionId, F.FunctionName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Function.Add(new Functionlistmodel
                            {
                                Functionid = query[i].FunctionId,
                                Functionname = query[i].FunctionName
                            });
                        }
                    }
                }
                return Function;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// Get Department list 
        /// </summary>
        /// <param name="Departmentid">To pass DepartmentId in this parameter</param>
        /// <returns></returns>
        public static List<Functionviewmodel> GetFunctionlist(int Departmentid)
        {
            try
            {
                List<Functionviewmodel> Funmodel = new List<Functionviewmodel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from D in context.tblRole
                                 where (D.DepartmentId == Departmentid && D.Status == "Active")
                                 select new { D.RoleId, D.RoleName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Funmodel.Add(new Functionviewmodel()
                            {
                                sno = i + 1,
                                Roleid = query[i].RoleId,
                                Rolename = query[i].RoleName
                            });
                        }
                    }
                }
                return Funmodel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Add department wise role authorization   
        /// </summary>
        /// <param name="model">Role access form data</param>
        /// <returns></returns>
        public static List<Functionviewmodel> AddDepartmentrole(List<Functionviewmodel> model)
        {
            try
            {

                List<Functionviewmodel> listfunction = new List<Functionviewmodel>();
                tblRoleaccess objrole = new tblRoleaccess();
                using (var dbctx = new IOASDBEntities())
                {

                    var funid = model[0].Functionid;
                    var deptid = model[0].Departmentid;
                    var query = (from f in dbctx.tblRoleaccess
                                 where (f.FunctionId == funid && f.DepartmentId == deptid)
                                 select f).ToList();
                    if (query.Count > 0)
                    {
                        dbctx.tblRoleaccess.RemoveRange(query);
                        dbctx.SaveChanges();

                    }

                }
                using (var context = new IOASDBEntities())
                {

                    if (model.Count > 0)
                    {

                        for (int i = 0; i < model.Count; i++)
                        {

                            if (model[i].Read == true)
                            {
                                objrole.RoleId = model[i].Roleid;
                                objrole.FunctionId = model[i].Functionid;
                                objrole.Read_f = model[i].Read;
                                objrole.Add_f = model[i].Add;
                                objrole.Delete_f = model[i].Delete;
                                objrole.Approve_f = model[i].Approve;
                                objrole.Update_f = model[i].Update;
                                objrole.DepartmentId = model[i].Departmentid;
                                objrole.Status = "Active";
                                context.tblRoleaccess.Add(objrole);
                                context.SaveChanges();
                            }
                        }
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                var msg = ex;
                return model;
            }
        }
        /// <summary>
        /// Get Department list to load in table 
        /// </summary>
        /// <param name="page">To pass in page number</param>
        /// <param name="pageSize">Number of page size</param>
        /// <returns></returns>
        public static List<DepartmentModel> GetDepartmentlist(DepartmentModel model)
        {
            try
            {
                List<DepartmentModel> Deptlist = new List<DepartmentModel>();

                using (var context = new IOASDBEntities())
                {
                    //decimal totalpage = (from D in context.tblDepartment
                    //             orderby D.DepartmentName
                    //             select new { D.DepartmentId, D.DepartmentName, D.HOD }).Count();

                    var query = (from D in context.tblDepartment
                                 orderby D.DepartmentName
                                 where D.Status == "Active"
                                 && (D.DepartmentName.Contains(model.SearchDepartment) || model.SearchDepartment == null)
                                 && (D.HOD.Contains(model.SearchHead) || model.SearchHead == null)
                                 select new { D.DepartmentId, D.DepartmentName, D.HOD }).ToList();
                    //decimal totalpagcount = (totalpage / pageSize);
                    //var tatcountpage = Math.Ceiling(totalpagcount);
                    if (query.Count > 0)
                    {

                        for (int i = 0; i < query.Count; i++)
                        {
                            var sno = i + 1;
                            Deptlist.Add(new DepartmentModel()
                            {
                                Sno = sno,
                                Departmentid = query[i].DepartmentId,
                                Departmentname = query[i].DepartmentName,
                                HOD = query[i].HOD,
                                //totalPages= (Int32)tatcountpage
                            });
                        }
                    }
                }
                return Deptlist;
            }
            catch (Exception ex)
            {
                List<DepartmentModel> Deptlist = new List<DepartmentModel>();
                return Deptlist;
            }
        }
        /// <summary>
        /// Edit to list table to department list
        /// </summary>
        /// <param name="DepartmentId">To pass single Department Id </param>
        /// <returns>Get Single Department list in modified</returns>
        public static DepartmentModel GetEditDepartmentlist(int DepartmentId)
        {
            try
            {
                DepartmentModel Deptlist = new DepartmentModel();

                using (var context = new IOASDBEntities())
                {

                    var query = (from D in context.tblDepartment
                                 orderby D.DepartmentName
                                 where (D.DepartmentId == DepartmentId)
                                 select new { D.DepartmentId, D.DepartmentName, D.HOD }).FirstOrDefault();

                    if (query != null)
                    {
                        Deptlist.Departmentid = query.DepartmentId;
                        Deptlist.Departmentname = query.DepartmentName;
                        Deptlist.HOD = query.HOD;
                    }
                    return Deptlist;
                }

            }
            catch (Exception ex)
            {
                DepartmentModel Deptlist = new DepartmentModel();
                return Deptlist;
            }
        }
        /// <summary>
        /// Remove Department List in table
        /// </summary>
        /// <param name="DepartmentId">To pass single Department Id</param>
        /// <returns></returns>
        public static int Deletedepartment(int DepartmentId, string Username)
        {
            try
            {
                tblDepartment inv;
                using (var context = new IOASDBEntities())
                {
                    var roleid = (from R in context.tblRole
                                  where (R.DepartmentId == DepartmentId && R.Status == "Active")
                                  select R.RoleId).ToList();
                    if (roleid.Count > 0)
                    {
                        return 2;
                    }
                    else
                    {
                        inv = context.tblDepartment.Where(s => s.DepartmentId == DepartmentId).FirstOrDefault();
                        if (inv != null)
                        {
                            inv.Status = "InActive";
                            inv.LastUpdated_TS = DateTime.Now;
                            inv.LastUpdateUserId = Common.GetUserid(Username);
                            //context.Entry(inv).State = System.Data.Entity.EntityState.Deleted;
                            context.SaveChanges();
                        }
                        return 1;
                    }
                }

            }
            catch (Exception ex)
            {
                var Msg = ex;
                return -1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<RoleModel> GetDepartmentRolelist(RoleModel model)
        {
            try
            {
                List<RoleModel> Roledept = new List<RoleModel>();
                //int skipprec = 0;
                //if (page == 1)
                //{
                //    skipprec = 0;
                //}
                //else
                //{
                //    skipprec = (page - 1) * pageSize;
                //}
                using (var context = new IOASDBEntities())
                {
                    //decimal totalpage = (from DR in context.tblRole
                    //                     orderby DR.RoleName
                    //                     select new { DR.RoleId, DR.RoleName, DR.DepartmentId }).Count();
                    var query = (from DR in context.tblRole
                                 from D in context.tblDepartment
                                 where DR.DepartmentId == D.DepartmentId && DR.Status == "Active"
                                 && (DR.RoleName.Contains(model.SearchRole) || model.SearchRole == null)
                                 && (DR.DepartmentId == model.SearchDepartment || model.SearchDepartment == null)
                                 orderby DR.RoleName
                                 select new { DR.DepartmentId, D.DepartmentName, DR.RoleId, DR.RoleName }).ToList();
                    //decimal totalpagcount = (totalpage / pageSize);
                    //var tatcountpage = Math.Ceiling(totalpagcount);
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            var sno = i + 1;
                            Roledept.Add(new RoleModel
                            {
                                sno = sno,
                                Departmentid = (Int32)query[i].DepartmentId,
                                Departmentname = query[i].DepartmentName,
                                Roleid = (Int32)query[i].RoleId,
                                Rolename = query[i].RoleName
                                //totalPages = (Int32)tatcountpage
                            });
                        }
                    }
                }
                return Roledept;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static RoleModel GetEditRolelist(int RoleId)
        {
            try
            {
                RoleModel Rolelist = new RoleModel();

                using (var context = new IOASDBEntities())
                {

                    var query = (from DR in context.tblRole
                                 orderby DR.RoleName
                                 where (DR.RoleId == RoleId)
                                 select new { DR.RoleId, DR.RoleName, DR.DepartmentId }).FirstOrDefault();

                    if (query != null)
                    {
                        Rolelist.Departmentid = (Int32)query.DepartmentId;
                        Rolelist.Roleid = query.RoleId;
                        Rolelist.Rolename = query.RoleName;
                    }

                }
                return Rolelist;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static int Deleterole(int RoleId, string Username)
        {
            try
            {
                tblRole role;

                using (var context = new IOASDBEntities())
                {
                    var userinrole = context.tblUser.Where(M => M.RoleId == RoleId && M.Status == "Active").FirstOrDefault();
                    var roleuser = context.tblUserRole.Where(M => M.RoleId == RoleId).FirstOrDefault();
                    if (userinrole != null || roleuser != null)
                    {
                        return 2;
                    }
                    else
                    {
                        role = context.tblRole.Where(s => s.RoleId == RoleId).FirstOrDefault();
                        //context.Entry(role).State = System.Data.Entity.EntityState.Deleted;
                        if (role != null)
                        {
                            role.Status = "InActive";
                            role.LastUpdateUserId = Common.GetUserid(Username);
                            role.UpdatedTS = DateTime.Now;
                            context.SaveChanges();
                        }
                        //userrole = context.tblUserRole.Where(R => R.RoleId == RoleId).FirstOrDefault();
                        //context.Entry(userrole).State = System.Data.Entity.EntityState.Deleted;
                        //context.SaveChanges();
                        return 1;
                    }

                }



            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public static List<MasterlistviewModel> GetGender()
        {
            try
            {
                List<MasterlistviewModel> gender = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from G in context.tblCodeControl
                                 where (G.CodeName == "Gender")
                                 orderby G.CodeValDetail
                                 select new { G.CodeValAbbr, G.CodeValDetail }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            gender.Add(new MasterlistviewModel()
                            {
                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail
                            });
                        }
                    }
                }
                return gender;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> gender = new List<MasterlistviewModel>();
                return gender;
            }
        }

        public static int UserRegistration(RegisterModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())

                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        tblUserRole objuserrole = new tblUserRole();

                        if (model.UserId == 0)
                        {
                            try
                            {
                                tblUser reg = new tblUser();
                                var chkuser = context.tblUser.FirstOrDefault(dup => dup.UserName == model.Username && dup.Status == "Active");
                                if (chkuser != null)
                                    return 2;
                                reg.FirstName = model.Firstname;
                                reg.LastName = model.Lastname;
                                reg.RoleId = model.RoleId;
                                reg.UserName = model.Username;
                                reg.Password = Cryptography.Encrypt(model.Password, "LFPassW0rd");
                                reg.ExpiryDate = model.ExpiryDate;
                                reg.DepartmentId = model.Department;
                                reg.Gender = model.Gender;
                                reg.CRTDDateTS = DateTime.Now;
                                reg.UPDTDateTS = DateTime.Now;
                                string Username = model.Createuser;
                                reg.CreatedUserId = Common.GetUserid(Username);
                                reg.Email = model.Email;
                                reg.Status = "Active";
                                reg.UserImage = model.Image;
                                context.tblUser.Add(reg);
                                context.SaveChanges();
                                if (model.SelectedRoles != null)
                                {
                                    var userid = (from U in context.tblUser
                                                  where (U.UserName == model.Username)
                                                  select U.UserId).FirstOrDefault();
                                    model.UserId = userid;
                                    for (int i = 0; i < model.SelectedRoles.Length; i++)
                                    {
                                        objuserrole.UserId = model.UserId;
                                        objuserrole.RoleId = model.SelectedRoles[i];
                                        objuserrole.Delegated_f = false;
                                        context.tblUserRole.Add(objuserrole);
                                        context.SaveChanges();
                                    }
                                }
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return -1;
                            }
                            return 1;
                        }
                        else
                        {
                            var objupdateuser = context.tblUser.Where(U => U.UserId == model.UserId).FirstOrDefault();
                            if (objupdateuser != null)
                            {
                                try
                                {
                                    objupdateuser.UserId = model.UserId;
                                    objupdateuser.FirstName = model.Firstname;
                                    objupdateuser.LastName = model.Lastname;
                                    objupdateuser.RoleId = model.RoleId;
                                    //bjupdateuser.UserName = model.Username;
                                    //reg.Password = Cryptography.Encrypt(model.Password, "LFPassW0rd");
                                    objupdateuser.ExpiryDate = model.ExpiryDate;
                                    objupdateuser.DepartmentId = model.Department;
                                    objupdateuser.Gender = model.Gender;
                                    objupdateuser.UPDTDateTS = DateTime.Now;
                                    string Username = model.Createuser;
                                    objupdateuser.Email = model.Email;
                                    objupdateuser.LastUpdateUserId = Common.GetUserid(Username);
                                    //objupdateuser.Email = model.Username;
                                    if (model.Image != null)
                                    {
                                        objupdateuser.UserImage = model.Image;
                                    }

                                    context.SaveChanges();

                                    var username = (from U in context.tblUser
                                                    where (U.UserId == model.UserId)
                                                    select U.UserName).FirstOrDefault();
                                    model.Username = username;
                                    var query = (from R in context.tblUserRole
                                                 where (R.UserId == model.UserId)
                                                 select R).ToList();
                                    if (query.Count > 0)
                                    {
                                        context.tblUserRole.RemoveRange(query);
                                        context.SaveChanges();

                                    }
                                    if (model.SelectedRoles != null)
                                    {
                                        for (int i = 0; i < model.SelectedRoles.Length; i++)
                                        {
                                            objuserrole.UserId = model.UserId;
                                            objuserrole.RoleId = model.SelectedRoles[i];
                                            objuserrole.Delegated_f = false;
                                            context.tblUserRole.Add(objuserrole);
                                            context.SaveChanges();
                                        }
                                    }
                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return -1;
                                }
                            }
                            return 3;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public static int Deleteuserlist(int UserId, string Username)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from D in context.tblUser
                                 where (D.UserId == UserId)
                                 select D.UserId).FirstOrDefault();

                    var user = context.tblUser.Where(U => U.UserId == UserId).FirstOrDefault();
                    if (user != null)
                    {
                        user.LastUpdateUserId = Common.GetUserid(Username);
                        user.UPDTDateTS = DateTime.Now;
                        user.Status = "InActive";
                        context.SaveChanges();
                    }
                    var userrole = (from R in context.tblUserRole
                                    where (R.UserId == UserId)
                                    select R).ToList();
                    if (userrole.Count > 0)
                    {
                        context.tblUserRole.RemoveRange(userrole);
                        context.SaveChanges();

                    }
                }
                return 4;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public static int PIRegistration(RegisterModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (model.UserId == 0)
                    {
                        tblUser PIreg = new tblUser();
                        var chkPI = context.tblUser.FirstOrDefault(dup => dup.Email == model.Email && dup.Status == "Active");
                        if (chkPI != null)
                            return 2;
                        string Username = model.Createuser;
                        PIreg.FirstName = model.Firstname;
                        PIreg.LastName = model.Lastname;
                        PIreg.RoleId = 7;
                        PIreg.ExpiryDate = model.ExpiryDate;
                        PIreg.DepartmentId = model.Department;
                        PIreg.Gender = model.Gender;
                        PIreg.CRTDDateTS = DateTime.Now;
                        PIreg.UPDTDateTS = DateTime.Now;
                        PIreg.CreatedUserId = Common.GetUserid(Username);
                        PIreg.Email = model.Email;
                        PIreg.Status = "Active";
                        PIreg.InstituteId = model.InstituteId;
                        PIreg.UserImage = model.Image;
                        PIreg.EMPCode = model.EMPCode;
                        PIreg.Designation = model.Designation;
                        context.tblUser.Add(PIreg);
                        context.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        var objupdatePI = context.tblUser.Where(M => M.UserId == model.UserId).FirstOrDefault();
                        if (objupdatePI != null)
                        {
                            string Username = model.Createuser;
                            objupdatePI.FirstName = model.Firstname;
                            objupdatePI.LastName = model.Lastname;
                            objupdatePI.RoleId = 7;
                            objupdatePI.ExpiryDate = model.ExpiryDate;
                            objupdatePI.DepartmentId = model.Department;
                            objupdatePI.Gender = model.Gender;
                            objupdatePI.UPDTDateTS = DateTime.Now;
                            objupdatePI.LastUpdateUserId = Common.GetUserid(Username);
                            objupdatePI.Email = model.Email;
                            objupdatePI.Status = "Active";
                            objupdatePI.InstituteId = model.InstituteId;
                            if (model.Image != null)
                            {
                                objupdatePI.UserImage = model.Image;
                            }
                            objupdatePI.EMPCode = model.EMPCode;
                            objupdatePI.Designation = model.Designation;
                            context.SaveChanges();

                        }
                        return 3;
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        //delete PI List in grid
        public static int DeletePIlist(int UserId, string Username)
        {
            try
            {


                using (var context = new IOASDBEntities())
                {
                    var query = (from D in context.tblUser
                                 where (D.UserId == UserId)
                                 select D.UserId).FirstOrDefault();

                    var user = context.tblUser.Where(U => U.UserId == UserId).FirstOrDefault();

                    if (user != null)
                    {
                        user.Status = "InActive";
                        user.UPDTDateTS = DateTime.Now;
                        user.LastUpdateUserId = Common.GetUserid(Username);
                        context.SaveChanges();
                    }
                }
                return 4;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public static List<MasterlistviewModel> getPIList(string Departmentid)
        {
            try
            {

                List<MasterlistviewModel> PIList = new List<MasterlistviewModel>();
                PIList.Add(new MasterlistviewModel()
                {
                    id = 0,
                    name = "Select any"

                });
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.vwFacultyStaffDetails
                                     //join ins in context.tblInstituteMaster on C.InstituteId equals ins.InstituteId
                                 where C.DepartmentCode == Departmentid
                                 orderby C.FirstName
                                 select new { C.UserId, C.FirstName, C.EmployeeId }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            PIList.Add(new MasterlistviewModel()
                            {
                                id = query[i].UserId,
                                name = query[i].EmployeeId + "-" + query[i].FirstName
                            });
                        }
                    }

                }

                return PIList;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static int Agencyregistration(AgencyModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            int agencyid = 0;
                            if (model.AgencyId == null)
                            {
                                try
                                {
                                    tblAgencyMaster regagency = new tblAgencyMaster();
                                    //var chkagency = context.tblAgencyMaster.FirstOrDefault(M => M.AgencyName == model.AgencyName && M.Status == "Active" && M.AgencyType == 2);
                                    //if (chkagency != null)
                                    //    return 2;
                                    //var chkagycode = context.tblAgencyMaster.FirstOrDefault(C => C.AgencyCode == model.AgencyCode && C.Status == "Active" && C.AgencyType == 2);
                                    //if(chkagycode!=null)
                                    //    return 4;
                                    var Sqnbr = (from A in context.tblAgencyMaster
                                                 where (A.AgencyType == 2)
                                                 select A.SeqNbr).Max();
                                    regagency.AgencyName = model.AgencyName;
                                    regagency.AgencyCode = model.AgencyCode;
                                    regagency.ContactPerson = model.ContactPerson;
                                    regagency.ContactNumber = model.ContactNumber;
                                    regagency.ContactEmail = model.ContactEmail;
                                    regagency.Address = model.Address;
                                    regagency.State = model.State;
                                    regagency.ProjectTypeId = model.ProjectTypeId;
                                    regagency.CompanyType = model.CompanyId;
                                    regagency.GovermentAgencyType = model.Ministry;
                                    if (model.Country > 0)
                                    {
                                        regagency.Country = model.Country;
                                    }
                                    else
                                    {
                                        regagency.Country = 128;
                                    }
                                    regagency.AgencyType = 2;
                                    regagency.Crtd_TS = DateTime.Now;
                                    regagency.Crtd_UserId = model.UserId;
                                    regagency.Status = "Active";
                                    regagency.GSTIN = model.GSTIN;
                                    regagency.TAN = model.TAN;
                                    regagency.PAN = model.PAN;
                                    regagency.StateId = model.StateId;
                                    regagency.StateCode = model.StateCode;
                                    regagency.BankName = model.BankName;
                                    regagency.AccountNumber = model.AccountNumber;
                                    regagency.BranchName = model.BranchName;
                                    regagency.SwiftCode = model.SWIFTCode;
                                    regagency.MICRCode = model.MICRCode;
                                    regagency.IFSCCode = model.IFSCCode;
                                    regagency.BankAddress = model.BankAddress;
                                    regagency.District = model.District;
                                    regagency.PinCode = model.PinCode;
                                    regagency.AgencyCountryCategoryId = model.AgencycountryCategoryId;
                                    regagency.IndianAgencyCategoryId = model.IndianagencyCategoryId;
                                    regagency.NonSezCategoryId = model.NonSezCategoryId;
                                    regagency.AgencyRegisterName = model.AgencyRegisterName;
                                    regagency.AgencyRegisterAddress = model.AgencyRegisterAddress;
                                    regagency.SeqNbr = (Convert.ToInt32(Sqnbr) + 1);
                                    context.tblAgencyMaster.Add(regagency);
                                    context.SaveChanges();
                                    agencyid = regagency.AgencyId;
                                    if (model.AttachName[0] != null && model.AttachName[0] != "")
                                    {
                                        for (int i = 0; i < model.DocumentType.Length; i++)
                                        {
                                            string docpath = "";
                                            docpath = System.IO.Path.GetFileName(model.File[i].FileName);
                                            var docfileId = Guid.NewGuid().ToString();
                                            var docname = docfileId + "_" + docpath;

                                            /*Saving the file in server folder*/
                                            model.File[i].UploadFile("AgencyDocument", docname);
                                            tblAgencyDocument document = new tblAgencyDocument();
                                            document.AgencyId = agencyid;
                                            if (model.File[i] != null)
                                            {
                                                document.AgencyDocument = model.File[i].FileName;

                                            }
                                            document.AttachmentPath = docname;
                                            document.AttachmentName = model.AttachName[i];
                                            document.DocumentType = model.DocumentType[i];
                                            document.IsCurrentVersion = true;
                                            document.DocumentUploadUserId = model.UserId;
                                            document.DocumentUpload_Ts = DateTime.Now;
                                            context.tblAgencyDocument.Add(document);
                                            context.SaveChanges();
                                        }

                                    }
                                    transaction.Commit();
                                    return 1;
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return -1;
                                }
                            }
                            else
                            {
                                var regagencyupdate = context.tblAgencyMaster.FirstOrDefault(M => M.AgencyId == model.AgencyId);
                                if (regagencyupdate != null)
                                {
                                    regagencyupdate.AgencyName = model.AgencyName;
                                    regagencyupdate.AgencyCode = model.AgencyCode;
                                    regagencyupdate.ContactPerson = model.ContactPerson;
                                    regagencyupdate.ContactNumber = model.ContactNumber;
                                    regagencyupdate.ContactEmail = model.ContactEmail;
                                    regagencyupdate.Address = model.Address;
                                    regagencyupdate.State = model.State;
                                    regagencyupdate.ProjectTypeId = model.ProjectTypeId;
                                    regagencyupdate.CompanyType = model.CompanyId;
                                    regagencyupdate.GovermentAgencyType = model.Ministry;
                                    if (model.Country > 0)
                                    {
                                        regagencyupdate.Country = model.Country;
                                    }
                                    else
                                    {
                                        regagencyupdate.Country = 128;
                                    }
                                    regagencyupdate.AgencyType = 2;
                                    regagencyupdate.LastupdatedUserid = model.UserId;
                                    regagencyupdate.Lastupdate_TS = DateTime.Now;
                                    regagencyupdate.Status = "Active";
                                    regagencyupdate.GSTIN = model.GSTIN;
                                    regagencyupdate.TAN = model.TAN;
                                    regagencyupdate.PAN = model.PAN;
                                    regagencyupdate.StateId = model.StateId;
                                    regagencyupdate.StateCode = model.StateCode;
                                    regagencyupdate.BankName = model.BankName;
                                    regagencyupdate.AccountNumber = model.AccountNumber;
                                    regagencyupdate.BranchName = model.BranchName;
                                    regagencyupdate.SwiftCode = model.SWIFTCode;
                                    regagencyupdate.MICRCode = model.MICRCode;
                                    regagencyupdate.IFSCCode = model.IFSCCode;
                                    regagencyupdate.BankAddress = model.BankAddress;
                                    regagencyupdate.District = model.District;
                                    regagencyupdate.PinCode = model.PinCode;
                                    regagencyupdate.AgencyCountryCategoryId = model.AgencycountryCategoryId;
                                    regagencyupdate.IndianAgencyCategoryId = model.IndianagencyCategoryId;
                                    regagencyupdate.NonSezCategoryId = model.NonSezCategoryId;
                                    regagencyupdate.AgencyRegisterName = model.AgencyRegisterName;
                                    regagencyupdate.AgencyRegisterAddress = model.AgencyRegisterAddress;
                                    context.SaveChanges();
                                    int agencyId = regagencyupdate.AgencyId;
                                    if (model.AttachName[0] != null && model.AttachName[0] != "")
                                    {
                                        for (int i = 0; i < model.DocumentType.Length; i++)
                                        {
                                            if (model.DocumentType[i] != 0)
                                            {
                                                var docid = model.DocumentId[i];
                                                var query = (from D in context.tblAgencyDocument
                                                             where (D.AgencyDocumentId == docid && D.AgencyId == model.AgencyId && D.IsCurrentVersion == true)
                                                             select D).ToList();
                                                if (query.Count == 0)
                                                {
                                                    string docpath = "";
                                                    docpath = System.IO.Path.GetFileName(model.File[i].FileName);
                                                    var docfileId = Guid.NewGuid().ToString();
                                                    var docname = docfileId + "_" + docpath;

                                                    /*Saving the file in server folder*/
                                                    model.File[i].UploadFile("AgencyDocument", docname);
                                                    tblAgencyDocument document = new tblAgencyDocument();
                                                    document.AgencyId = model.AgencyId;
                                                    if (model.File[i] != null)
                                                    {
                                                        document.AgencyDocument = model.File[i].FileName;

                                                    }
                                                    document.AttachmentPath = docname;
                                                    document.AttachmentName = model.AttachName[i];
                                                    document.DocumentType = model.DocumentType[i];
                                                    document.IsCurrentVersion = true;
                                                    document.DocumentUploadUserId = model.UserId;
                                                    document.DocumentUpload_Ts = DateTime.Now;
                                                    context.tblAgencyDocument.Add(document);
                                                    context.SaveChanges();
                                                }
                                                else
                                                {
                                                    query[0].DocumentType = model.DocumentType[i];
                                                    query[0].AttachmentName = model.AttachName[i];
                                                    query[0].DocumentUploadUserId = model.UserId;
                                                    query[0].DocumentUpload_Ts = DateTime.Now;
                                                    query[0].IsCurrentVersion = true;
                                                    context.SaveChanges();

                                                }

                                            }
                                        }


                                        var deldocument = (from RD in context.tblAgencyDocument
                                                           where RD.AgencyId == agencyId &&
                                                           !model.DocumentId.Contains(RD.AgencyDocumentId) && RD.IsCurrentVersion != false
                                                           select RD).ToList();
                                        int delCount = deldocument.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                deldocument[i].IsCurrentVersion = false;
                                                context.SaveChanges();
                                            }
                                        }

                                    }
                                    transaction.Commit();
                                }
                                return 3;

                            }

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var MSg = ex;
                return -1;
            }
        }

        public static string CheckAgencyCode(string agyCode)
        {
            try
            {
                string code = "";
                using (var context = new IOASDBEntities())
                {
                    var checkcode = (from AC in context.tblAgencyMaster
                                     where (AC.AgencyCode == agyCode && AC.Status == "Active" && AC.AgencyType == 2)
                                     select AC.AgencyCode).FirstOrDefault();
                    if (checkcode != null)
                    {
                        code = checkcode;
                    }

                }
                return code;
            }
            catch (Exception ex)
            {
                string code = "";
                return code;
            }
        }
        public static AgencySearchModel Getagenctlist(AgencySearchModel model, int page, int pageSize)
        {
            try
            {
                AgencySearchModel agysearch = new AgencySearchModel();
                List<AgencyModel> agency = new List<AgencyModel>();
                using (var context = new IOASDBEntities())
                {
                    int skiprec = 0;
                    if (page == 1)
                    {
                        skiprec = 0;
                    }
                    else
                    {
                        skiprec = (page - 1) * pageSize;
                    }
                    var query = (from A in context.tblAgencyMaster
                                 join C in context.tblCountries on A.Country equals C.countryID into CO
                                 from cl in CO.DefaultIfEmpty()
                                 orderby A.AgencyId descending
                                 where A.Status == "Active" && A.AgencyType == 2
                                 && (A.AgencyName.Contains(model.InName) || A.GSTIN.Contains(model.InName) || model.InName == null)
                                 && (A.AgencyCode.Contains(model.InCode) || model.InCode == null)
                                 && (A.AgencyName.Contains(model.ExName) || A.GSTIN.Contains(model.ExName) || model.ExName == null)
                                 && (A.AgencyCode.Contains(model.Excode) || model.Excode == null)
                                 && (A.Country == model.ExCountry || model.ExCountry == null)
                                 select new { A.GSTIN, A.AgencyId, A.AgencyName, A.AgencyCode, A.ContactPerson, A.ContactEmail, A.Country, cl.countryName }).Skip(skiprec).Take(pageSize).ToList();
                    agysearch.TotalRecords = (from A in context.tblAgencyMaster
                                              join C in context.tblCountries on A.Country equals C.countryID into CO
                                              from cl in CO.DefaultIfEmpty()
                                              orderby A.AgencyId descending
                                              where A.Status == "Active" && A.AgencyType == 2
                                              && (A.AgencyName.Contains(model.InName) || A.GSTIN.Contains(model.InName) || model.InName == null)
                                              && (A.AgencyCode.Contains(model.InCode) || model.InCode == null)
                                                && (A.AgencyName.Contains(model.ExName) || A.GSTIN.Contains(model.ExName) || model.ExName == null)
                                              && (A.AgencyCode.Contains(model.Excode) || model.Excode == null)
                                              && (A.Country == model.ExCountry || model.ExCountry == null)
                                              select new { A.GSTIN, A.AgencyId, A.AgencyName, A.AgencyCode, A.ContactPerson, A.ContactEmail, A.Country, cl.countryName }).Count();
                    if (query.Count > 0)
                    {
                        var country = "";
                        for (int i = 0; i < query.Count; i++)
                        {
                            if (query[i].Country == 128)
                            {
                                country = "INDIA";
                            }
                            else
                            {
                                country = query[i].countryName;
                            }
                            var sno = i + 1;
                            agency.Add(new AgencyModel()
                            {
                                sno = sno,
                                AgencyId = query[i].AgencyId,
                                AgencyName = query[i].AgencyName + " - " + query[i].GSTIN,
                                AgencyCode = query[i].AgencyCode,
                                CountryName = country,
                            });
                        }

                    }
                    agysearch.agencylist = agency;
                    return agysearch;
                }
            }
            catch (Exception ex)
            {
                return new AgencySearchModel();

            }
        }
        public static List<AgencyModel> Getagenctlist(AgencyModel model)
        {
            try
            {
                List<AgencyModel> agency = new List<AgencyModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from A in context.tblAgencyMaster
                                 join C in context.tblCountries on A.Country equals C.countryID into CO
                                 from cl in CO.DefaultIfEmpty()
                                 where A.Status == "Active" && A.AgencyType == 2
                                 && (A.AgencyName.Contains(model.SearchAgencyName) || model.SearchAgencyName == null)
                                 && (A.AgencyCode.Contains(model.SearchAgencyCode) || model.SearchAgencyCode == null)
                                 && (A.Country == model.SearchAgencyCountry || model.SearchAgencyCountry == null)
                                 select new { A.AgencyId, A.AgencyName, A.AgencyCode, A.ContactPerson, A.ContactEmail, A.Country, cl.countryName }).ToList();
                    if (query.Count > 0)
                    {
                        var country = "";
                        for (int i = 0; i < query.Count; i++)
                        {
                            if (query[i].Country == 128)
                            {
                                country = "INDIA";
                            }
                            else
                            {
                                country = query[i].countryName;
                            }
                            var sno = i + 1;
                            agency.Add(new AgencyModel()
                            {
                                sno = sno,
                                AgencyId = query[i].AgencyId,
                                AgencyName = query[i].AgencyName,
                                AgencyCode = query[i].AgencyCode,
                                CountryName = country,
                            });
                        }
                    }
                    return agency;
                }
            }
            catch (Exception ex)
            {
                List<AgencyModel> agency = new List<AgencyModel>();
                return agency;
            }
        }
        public static AgencyModel Geteditagency(int Agencyid)
        {
            try
            {
                AgencyModel editagency = new AgencyModel();
                using (var context = new IOASDBEntities())
                {
                    var filenamelist = (from F in context.tblAgencyDocument
                                        where F.AgencyId == Agencyid && F.IsCurrentVersion == true
                                        select new { F.AgencyDocumentId, F.AttachmentPath, F.AgencyDocument, F.AttachmentName, F.DocumentType }).ToList();
                    var query = (from E in context.tblAgencyMaster
                                 where (E.AgencyId == Agencyid)
                                 select new
                                 {
                                     E.AgencyId,
                                     E.AgencyName,
                                     E.AgencyCode,
                                     E.ContactPerson,
                                     E.ContactNumber,
                                     E.ContactEmail,
                                     E.Address,
                                     E.State,
                                     E.Country,
                                     E.AgencyType,
                                     E.Scheme,
                                     E.GSTIN,
                                     E.TAN,
                                     E.PAN,
                                     E.StateId,
                                     E.StateCode,
                                     E.BankName,
                                     E.AccountNumber,
                                     E.BranchName,
                                     E.SwiftCode,
                                     E.MICRCode,
                                     E.IFSCCode,
                                     E.BankAddress,
                                     E.PinCode,
                                     E.District,
                                     E.AgencyCountryCategoryId,
                                     E.IndianAgencyCategoryId,
                                     E.NonSezCategoryId,
                                     E.AgencyRegisterName,
                                     E.AgencyRegisterAddress,
                                     E.ProjectTypeId,
                                     E.CompanyType,
                                     E.GovermentAgencyType
                                 }).FirstOrDefault();

                    if (query != null)
                    {
                        editagency.AgencyId = query.AgencyId;
                        editagency.AgencyName = query.AgencyName;
                        editagency.AgencyCode = query.AgencyCode;
                        editagency.ContactPerson = query.ContactPerson;
                        editagency.ContactNumber = query.ContactNumber;
                        editagency.ContactEmail = query.ContactEmail;
                        editagency.Address = query.Address;
                        editagency.State = query.State;
                        editagency.Country = Convert.ToInt32(query.Country);
                        editagency.AgencyType = Convert.ToInt32(query.AgencyType);
                        //.Scheme = (Int32)query.Scheme;
                        editagency.GSTIN = query.GSTIN;
                        editagency.TAN = query.TAN;
                        editagency.PAN = query.PAN;
                        editagency.ProjectTypeId = Convert.ToInt32(query.ProjectTypeId);
                        editagency.CompanyId = Convert.ToInt32(query.CompanyType);
                        editagency.Ministry = Convert.ToInt32(query.GovermentAgencyType);
                        if (query.StateId != null)
                        {
                            editagency.StateId = Convert.ToInt32(query.StateId);
                        }
                        else
                        {
                            editagency.StateId = 0;
                        }
                        editagency.StateCode = query.StateCode;
                        editagency.BankName = query.BankName;
                        if (query.AccountNumber != null)
                        {
                            editagency.AccountNumber = query.AccountNumber;
                        }
                        else
                        {
                            editagency.AccountNumber = null;
                        }
                        editagency.BranchName = query.BranchName;
                        editagency.SWIFTCode = query.SwiftCode;
                        editagency.MICRCode = query.MICRCode;
                        editagency.IFSCCode = query.IFSCCode;
                        editagency.BankAddress = query.BankAddress;
                        if (query.PinCode != null)
                        {
                            editagency.PinCode = Convert.ToInt32(query.PinCode);
                        }
                        else
                        {
                            editagency.PinCode = null;
                        }
                        editagency.District = query.District;
                        editagency.AgencycountryCategoryId = Convert.ToInt32(query.AgencyCountryCategoryId);
                        if (query.IndianAgencyCategoryId != null)
                        {
                            editagency.IndianagencyCategoryId = Convert.ToInt32(query.IndianAgencyCategoryId);
                        }
                        else
                        {
                            editagency.IndianagencyCategoryId = 0;
                        }
                        if (query.NonSezCategoryId != null)
                        {
                            editagency.NonSezCategoryId = Convert.ToInt32(query.NonSezCategoryId);
                        }
                        else
                        {
                            editagency.NonSezCategoryId = 0;
                        }
                        editagency.AgencyRegisterName = query.AgencyRegisterName;
                        editagency.AgencyRegisterAddress = query.AgencyRegisterAddress;
                        if (filenamelist.Count > 0)
                        {
                            int[] _docid = new int[filenamelist.Count];
                            int[] _doctype = new int[filenamelist.Count];
                            string[] _docname = new string[filenamelist.Count];
                            string[] _attchname = new string[filenamelist.Count];
                            string[] _docpath = new string[filenamelist.Count];
                            for (int i = 0; i < filenamelist.Count; i++)
                            {
                                _docid[i] = Convert.ToInt32(filenamelist[i].AgencyDocumentId);
                                _doctype[i] = Convert.ToInt32(filenamelist[i].DocumentType);
                                _docname[i] = filenamelist[i].AgencyDocument;
                                _attchname[i] = filenamelist[i].AttachmentName;
                                _docpath[i] = filenamelist[i].AttachmentPath;
                            }
                            editagency.DocumentId = _docid;
                            editagency.DocumentType = _doctype;
                            editagency.DocumentName = _docname;
                            editagency.AttachName = _attchname;
                            editagency.DocPath = _docpath;
                        }
                    }
                }
                return editagency;
            }
            catch (Exception ex)
            {
                var Msg = ex;
                AgencyModel editagency = new AgencyModel();
                return editagency;
            }
        }
        public static int Deleteagency(int Agencyid, string Username)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {

                    var agency = context.tblAgencyMaster.Where(D => D.AgencyId == Agencyid).FirstOrDefault();

                    if (agency != null)
                    {
                        agency.Status = "InActive";
                        agency.Lastupdate_TS = DateTime.Now;
                        agency.LastupdatedUserid = Common.GetUserid(Username);
                        context.SaveChanges();
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                var msg = ex;
                return -1;
            }
        }
        public static string Getstatecode(int StateId)
        {
            try
            {
                var statecode = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from S in context.tblStateMaster
                                 where (S.StateId == StateId)
                                 select S.StateCode).FirstOrDefault();
                    if (query != null)
                    {
                        statecode = query;
                    }
                }
                return statecode;
            }
            catch (Exception ex)
            {
                var statecode = "";
                return statecode;
            }
        }
        public static int CreateAccontHead(AccountHeadViewModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (model.AccountHeadId == 0)
                    {
                        var chkhead = context.tblAccountHead.FirstOrDefault(aloc => aloc.AccountHead == model.AccountHead && aloc.Status == "Active");
                        if (chkhead != null)
                            return 2;
                        var Sqnbr = (from AH in context.tblAccountHead
                                     select AH.SeqNbr).Max();
                        tblAccountHead acctheadreg = new tblAccountHead();
                        acctheadreg.AccountGroupId = model.AccountGroupId;
                        acctheadreg.AccountHead = model.AccountHead;
                        acctheadreg.AccountHeadCode = model.AccountHeadCode;
                        acctheadreg.CrtdUserId = model.userid;
                        acctheadreg.CrtdTS = DateTime.Now;
                        acctheadreg.Status = "Active";
                        if (model.AccountGroupId == 61 || model.AccountGroupId == 38)
                            acctheadreg.Bank_f = true;
                        else
                            acctheadreg.Bank_f = false;
                        acctheadreg.SeqNbr = Convert.ToInt32((Sqnbr + 1).ToString().PadLeft(4, '1'));
                        context.tblAccountHead.Add(acctheadreg);
                        context.SaveChanges();
                        int Hedid = (from a in context.tblAccountGroup
                                     join b in context.tblAccountHead on a.AccountGroupId equals b.AccountGroupId
                                     where b.AccountHeadId != acctheadreg.AccountHeadId && b.AccountGroupId
                                     == model.AccountGroupId
                                     select b.AccountHeadId).FirstOrDefault();

                        var Acchedgrp = context.tblAccountHeadandgroup.Where(m => m.AccountHeadId == Hedid).FirstOrDefault();
                        if (Acchedgrp != null)
                        {
                            tblAccountHeadandgroup hedgrp = new tblAccountHeadandgroup();
                            hedgrp.Accounts = Acchedgrp.Accounts;
                            hedgrp.Groups = Acchedgrp.Groups;
                            hedgrp.AccountHead = model.AccountHead;
                            hedgrp.AccountHeadId = acctheadreg.AccountHeadId;
                            context.tblAccountHeadandgroup.Add(hedgrp);
                            context.SaveChanges();
                        }
                        else
                            return 4;

                        return 1;
                    }
                    else
                    {
                        var acctheadupdate = context.tblAccountHead.Where(AU => AU.AccountHeadId == model.AccountHeadId).FirstOrDefault();
                        if (acctheadupdate != null)
                        {
                            acctheadupdate.AccountGroupId = model.AccountGroupId;
                            acctheadupdate.AccountHead = model.AccountHead;
                            acctheadupdate.AccountHeadCode = model.AccountHeadCode;
                            acctheadupdate.Status = "Active";
                            acctheadupdate.LastUpdatedUserId = model.userid;
                            acctheadupdate.Lastupdated_TS = DateTime.Now;
                            context.SaveChanges();
                        }
                        return 3;
                    }
                }
            }
            catch (Exception ex)
            {
                var Msg = ex;
                return -1;
            }
        }
        public static AccountHeadViewModel AccountHeadEdit(int headid)
        {
            try
            {
                AccountHeadViewModel account = new AccountHeadViewModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from H in context.tblAccountHead
                                 where (H.AccountHeadId == headid)
                                 select new { H.AccountHeadId, H.AccountGroupId, H.AccountHeadCode, H.AccountHead }).FirstOrDefault();
                    if (query != null)
                    {
                        account.AccountHeadId = query.AccountHeadId;
                        account.AccountGroupId = Convert.ToInt32(query.AccountGroupId);
                        account.AccountHeadCode = query.AccountHeadCode;
                        account.AccountHead = query.AccountHead;
                    }

                }
                return account;
            }
            catch (Exception ex)
            {
                var msg = ex;
                AccountHeadViewModel account = new AccountHeadViewModel();
                return account;
            }
        }
        public static List<AccountHeadViewModel> AccountHeadList(AccountHeadViewModel model)
        {
            try
            {
                List<AccountHeadViewModel> accounthead = new List<AccountHeadViewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from H in context.tblAccountHead
                                 where H.Status == "Active"
                                 && (H.AccountHead.Contains(model.AccountHeadSearch) || model.AccountHeadSearch == null)
                                 && (H.AccountHeadCode.Contains(model.AccountHeadCodeSearch) || model.AccountHeadCodeSearch == null)
                                 && (H.AccountGroupId == model.AccountGroupIdSearch || model.AccountGroupIdSearch == null)
                                 orderby H.AccountHead
                                 select new { H.AccountHeadId, H.AccountHead, H.AccountHeadCode }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            var s = i + 1;
                            accounthead.Add(new AccountHeadViewModel()
                            {
                                sno = s,
                                AccountHeadId = query[i].AccountHeadId,
                                AccountHead = query[i].AccountHead,
                                AccountHeadCode = query[i].AccountHeadCode
                            });
                        }
                    }
                    return accounthead;
                }
            }
            catch (Exception ex)
            {
                List<AccountHeadViewModel> accounthead = new List<AccountHeadViewModel>();
                return accounthead;
            }
        }
        public static int DeleteAccountHead(int headid, int userid)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (userid == 1)
                    {
                        var query = context.tblAccountHead.FirstOrDefault(H => H.AccountHeadId == headid);
                        if (query != null)
                        {
                            query.Status = "InActive";
                            query.LastUpdatedUserId = userid;
                            query.Lastupdated_TS = DateTime.Now;
                            context.SaveChanges();
                        }
                        var finId = context.tblFinYear.Where(F => F.CurrentYearFlag == true).Select(F => F.FinYearId).FirstOrDefault();
                        var headquery = context.tblHeadOpeningBalance.Where(h => h.AccountHeadId == headid && h.FinYearId == finId).FirstOrDefault();
                        if (headquery != null)
                        {
                            tblHeadOBDeletedLog deletelog = new tblHeadOBDeletedLog();
                            deletelog.AccountHeadId = headquery.AccountHeadId;
                            deletelog.CurrentOBBalance = headquery.OpeningBalance;
                            deletelog.CurrentFinYearId = finId;
                            deletelog.DeleteUserId = userid;
                            deletelog.Delete_Ts = DateTime.Now;
                            context.tblHeadOBDeletedLog.Add(deletelog);
                            context.SaveChanges();
                            var removequery = context.tblHeadOpeningBalance.Where(d => d.AccountHeadId == headid).ToList();
                            if (removequery.Count > 0)
                            {
                                context.tblHeadOpeningBalance.RemoveRange(removequery);
                                context.SaveChanges();
                            }
                        }
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex;
                return -1;
            }
        }
        public static string GetAllocationHeadCode(int accountgroupcode)
        {
            try
            {
                string accounthead = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from AG in context.tblAccountGroup
                                 where (AG.AccountGroupId == accountgroupcode)
                                 select AG.AccountGroupCode).FirstOrDefault();
                    var SqNbr = (from AH in context.tblAccountHead
                                 select AH.SeqNbr).Max();

                    var acctcode = query;

                    accounthead = acctcode + '-' + (Convert.ToInt32(SqNbr) + 1).ToString().PadLeft(4, '1');
                }
                return accounthead;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static int Projectstaff(Projectstaffcategorymodel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (model.ProjectstaffcategoryId == 0)
                    {
                        var chkproj = context.tblProjectStaffCategoryMaster.FirstOrDefault(P => P.ProjectStaffCategory == model.ProjectstaffCategory && P.Status == "Active");
                        if (chkproj != null)
                            return 2;
                        tblProjectStaffCategoryMaster addstproj = new tblProjectStaffCategoryMaster();
                        addstproj.ProjectStaffCategory = model.ProjectstaffCategory;
                        addstproj.CrtdTS = DateTime.Now;
                        addstproj.CrtdUserId = model.userid;
                        addstproj.Status = "Active";
                        context.tblProjectStaffCategoryMaster.Add(addstproj);
                        context.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        var projstaff = context.tblProjectStaffCategoryMaster.FirstOrDefault(P => P.ProjectStaffCategoryId == model.ProjectstaffcategoryId);
                        if (projstaff != null)
                        {
                            projstaff.ProjectStaffCategory = model.ProjectstaffCategory;
                            projstaff.Status = "Active";
                            projstaff.LastUpdatedUserid = model.userid;
                            projstaff.LastUpdated_TS = DateTime.Now;
                            context.SaveChanges();
                        }
                        return 3;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex;
                return -1;
            }
        }
        public static List<Projectstaffcategorymodel> Getprojectlist()
        {
            try
            {
                List<Projectstaffcategorymodel> proj = new List<Projectstaffcategorymodel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProjectStaffCategoryMaster
                                 where (P.Status == "Active")
                                 orderby P.ProjectStaffCategory
                                 select new { P.ProjectStaffCategoryId, P.ProjectStaffCategory }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            var s = i + 1;
                            proj.Add(new Projectstaffcategorymodel()
                            {
                                sno = s,
                                ProjectstaffcategoryId = query[i].ProjectStaffCategoryId,
                                ProjectstaffCategory = query[i].ProjectStaffCategory

                            });
                        }
                    }
                }
                return proj;
            }
            catch (Exception ex)
            {
                List<Projectstaffcategorymodel> proj = new List<Projectstaffcategorymodel>();
                return proj;
            }
        }
        public static Projectstaffcategorymodel Editprojectstaff(int Projid)
        {
            try
            {
                Projectstaffcategorymodel proj = new Projectstaffcategorymodel();
                using (var context = new IOASDBEntities())
                {

                    var query = (from P in context.tblProjectStaffCategoryMaster
                                 where (P.ProjectStaffCategoryId == Projid)
                                 select new { P.ProjectStaffCategoryId, P.ProjectStaffCategory }).FirstOrDefault();
                    if (query != null)
                    {
                        proj.ProjectstaffcategoryId = query.ProjectStaffCategoryId;
                        proj.ProjectstaffCategory = query.ProjectStaffCategory;
                    }
                }

                return proj;
            }
            catch (Exception ex)
            {
                var msg = ex;
                Projectstaffcategorymodel proj = new Projectstaffcategorymodel();
                return proj;
            }
        }
        public static int Deleteprojectstaff(int Projid, int userid)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblProjectStaffCategoryMaster.Where(P => P.ProjectStaffCategoryId == Projid).FirstOrDefault();
                    if (query != null)
                    {
                        query.Status = "InActive";
                        query.LastUpdatedUserid = userid;
                        query.LastUpdated_TS = DateTime.Now;
                        context.SaveChanges();
                    }

                }
                return 1;
            }
            catch (Exception ex)
            {
                var msg = ex;
                return -1;
            }
        }
        public static int createscheme(Schemeviewmodel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (model.SchemeId == 0)
                    {
                        var chksheme = context.tblSchemes.FirstOrDefault(S => S.SchemeName == model.SchemeName && S.Status == "Active");
                        if (chksheme != null)
                            return 2;
                        tblSchemes addscheme = new tblSchemes();
                        addscheme.SchemeName = model.SchemeName;
                        addscheme.SchemeCode = model.Schemecode;
                        addscheme.ProjectType = model.ProjectType;
                        addscheme.CreatedUserId = model.userId;
                        addscheme.Created_TS = DateTime.Now;
                        addscheme.Status = "Active";
                        context.tblSchemes.Add(addscheme);
                        context.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        var updatescheme = context.tblSchemes.Where(S => S.SchemeId == model.SchemeId).FirstOrDefault();
                        if (updatescheme != null)
                        {
                            updatescheme.SchemeName = model.SchemeName;
                            updatescheme.SchemeCode = model.Schemecode;
                            updatescheme.ProjectType = model.ProjectType;
                            updatescheme.Status = "Active";
                            updatescheme.LastUpdatedUsedId = model.userId;
                            updatescheme.LastUpdated_TS = DateTime.Now;
                            context.SaveChanges();
                        }
                        return 3;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex;
                return -1;
            }
        }
        public static List<Schemeviewmodel> Getschemelist()
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    List<Schemeviewmodel> schemelist = new List<Schemeviewmodel>();
                    var query = (from S in context.tblSchemes
                                 where (S.Status == "Active")
                                 select new { S.SchemeId, S.SchemeName, S.SchemeCode }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            var s = i + 1;
                            schemelist.Add(new Schemeviewmodel()
                            {
                                sno = s,
                                SchemeId = query[i].SchemeId,
                                SchemeName = query[i].SchemeName,
                                Schemecode = query[i].SchemeCode,

                            });
                        }
                    }
                    return schemelist;
                }
            }
            catch (Exception ex)
            {
                var msg = ex;
                List<Schemeviewmodel> schemelist = new List<Schemeviewmodel>();
                return schemelist;
            }
        }
        public static Schemeviewmodel Editscheme(int schemeid)
        {
            try
            {
                Schemeviewmodel editmodel = new Schemeviewmodel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from S in context.tblSchemes
                                 where (S.SchemeId == schemeid)
                                 select new { S.SchemeId, S.SchemeName, S.SchemeCode, S.ProjectType }).FirstOrDefault();
                    if (query != null)
                    {
                        editmodel.SchemeId = query.SchemeId;
                        editmodel.SchemeName = query.SchemeName;
                        editmodel.Schemecode = query.SchemeCode;
                        editmodel.ProjectType = (Int32)query.ProjectType;
                    }
                }
                return editmodel;
            }
            catch (Exception ex)
            {
                var msg = ex;
                Schemeviewmodel editmodel = new Schemeviewmodel();
                return editmodel;
            }
        }
        public static int Deletescheme(int schemeid, string Username)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblSchemes.Where(S => S.SchemeId == schemeid).FirstOrDefault();
                    if (query != null)
                    {
                        query.Status = "InActive";
                        query.LastUpdatedUsedId = Common.GetUserid(Username);
                        query.LastUpdated_TS = DateTime.Now;
                        context.SaveChanges();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                var msg = ex;
                return -1;
            }
        }
        public static int consultancyfundingadd(ConsultancyFundingcategorymodel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (model.ConsultancyFundingcategoryid == 0)
                    {
                        var chkcons = context.tblConsultancyFundingCategory.FirstOrDefault(C => C.ConsultancyFundingCategory == model.ConsultancyFundingcategory && C.Status == "Active");
                        if (chkcons != null)
                            return 2;
                        tblConsultancyFundingCategory consfunding = new tblConsultancyFundingCategory();

                        consfunding.ConsultancyFundingCategory = model.ConsultancyFundingcategory;
                        consfunding.Status = "Active";
                        consfunding.CrtdTS = DateTime.Now;
                        consfunding.CrtdUserId = model.userid;
                        context.tblConsultancyFundingCategory.Add(consfunding);
                        context.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        var consfundingupdate = context.tblConsultancyFundingCategory.Where(C => C.ConsultancyFundingCategoryId == model.ConsultancyFundingcategoryid).FirstOrDefault();
                        if (consfundingupdate != null)
                        {
                            consfundingupdate.ConsultancyFundingCategory = model.ConsultancyFundingcategory;
                            consfundingupdate.Status = "Active";
                            consfundingupdate.LastUpdatedUserId = model.userid;
                            consfundingupdate.Lastupdated_TS = DateTime.Now;
                            context.SaveChanges();
                        }
                        return 3;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex;
                return -1;
            }
        }
        public static List<ConsultancyFundingcategorymodel> Getconsfundinglist()
        {
            try
            {
                List<ConsultancyFundingcategorymodel> fundinglist = new List<ConsultancyFundingcategorymodel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblConsultancyFundingCategory
                                 where (C.Status == "Active")
                                 orderby C.ConsultancyFundingCategory
                                 select new { C.ConsultancyFundingCategoryId, C.ConsultancyFundingCategory }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            var s = i + 1;
                            fundinglist.Add(new ConsultancyFundingcategorymodel()
                            {
                                sno = s,
                                ConsultancyFundingcategoryid = query[i].ConsultancyFundingCategoryId,
                                ConsultancyFundingcategory = query[i].ConsultancyFundingCategory,
                            });
                        }
                    }
                }
                return fundinglist;
            }
            catch (Exception ex)
            {
                List<ConsultancyFundingcategorymodel> fundinglist = new List<ConsultancyFundingcategorymodel>();
                return fundinglist;
            }
        }
        public static ConsultancyFundingcategorymodel EditConsfundingcategory(int fundingId)
        {
            try
            {
                ConsultancyFundingcategorymodel editcons = new ConsultancyFundingcategorymodel();
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblConsultancyFundingCategory.Where(C => C.ConsultancyFundingCategoryId == fundingId).FirstOrDefault();
                    if (query != null)
                    {
                        editcons.ConsultancyFundingcategoryid = query.ConsultancyFundingCategoryId;
                        editcons.ConsultancyFundingcategory = query.ConsultancyFundingCategory;
                    }
                }
                return editcons;
            }
            catch (Exception ex)
            {
                var msg = ex;
                ConsultancyFundingcategorymodel editcons = new ConsultancyFundingcategorymodel();
                return editcons;
            }
        }
        public static int Deleteconsfundingcategory(int fundingId, int userid)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblConsultancyFundingCategory.Where(C => C.ConsultancyFundingCategoryId == fundingId).FirstOrDefault();
                    if (query != null)
                    {
                        query.Status = "InActive";
                        query.LastUpdatedUserId = userid;
                        query.Lastupdated_TS = DateTime.Now;
                        context.SaveChanges();
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                var msg = ex;
                return -1;
            }
        }
        public static List<Functionviewmodel> AccessRightsfunction(int functionid, int Departmentid)
        {
            try
            {
                List<Functionviewmodel> funaccess = new List<Functionviewmodel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from R in context.tblRoleaccess
                                 where (R.FunctionId == functionid && R.DepartmentId == Departmentid)
                                 select new { R.FunctionId, R.DepartmentId, R.RoleId, R.Read_f, R.Add_f, R.Approve_f, R.Delete_f, R.Update_f }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            funaccess.Add(new Functionviewmodel()
                            {
                                Roleid = (Int32)query[i].RoleId,
                                Functionid = (Int32)query[i].FunctionId,
                                Departmentid = (Int32)query[i].DepartmentId,
                                Read = (bool)query[i].Read_f,
                                Add = (bool)query[i].Add_f,
                                Approve = (bool)query[i].Approve_f,
                                Delete = (bool)query[i].Delete_f,
                                Update = (bool)query[i].Update_f
                            });
                        }
                    }
                    return funaccess;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int Institute(CreateInstituteModel model, int userid)
        {
            try
            {
                var activationcode = Guid.NewGuid();
                tblInstituteMaster Institute = new tblInstituteMaster();
                using (var context = new IOASDBEntities())
                {
                    if (model.InstituteId == 0)
                    {
                        var chkinstitute = context.tblInstituteMaster.FirstOrDefault(dup => dup.Institutename == model.Institutename && dup.Status == "Active");
                        if (chkinstitute != null)
                            return 2;
                        Institute.Firstname = model.FirstName;
                        Institute.Lastname = model.lastName;
                        Institute.State = model.State;
                        Institute.Country = model.selCountry;
                        Institute.Address1 = model.Address1;
                        Institute.Zipcode = model.zipCode;
                        Institute.ContactMobile = model.ContactMobile;
                        Institute.Email = model.Email;
                        Institute.Address2 = model.Address2;
                        Institute.Location = model.City;
                        Institute.Institutename = model.Institutename;
                        Institute.CRTE_UserID = userid;
                        Institute.logoURL = model.logo;
                        Institute.Designation = model.contactDES;
                        Institute.Institutecode = model.InstituteCode;
                        Institute.CRTE_TS = DateTime.Now;
                        Institute.Status = "Active";
                        context.tblInstituteMaster.Add(Institute);
                        context.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        var updateinstitute = context.tblInstituteMaster.Where(M => M.InstituteId == model.InstituteId).FirstOrDefault();
                        if (updateinstitute != null)
                        {
                            updateinstitute.Firstname = model.FirstName;
                            updateinstitute.Lastname = model.lastName;
                            updateinstitute.State = model.State;
                            updateinstitute.Country = model.selCountry;
                            updateinstitute.Address1 = model.Address1;
                            updateinstitute.Address2 = model.Address2;
                            updateinstitute.Location = model.City;
                            updateinstitute.Zipcode = model.zipCode;
                            updateinstitute.ContactMobile = model.ContactMobile;
                            updateinstitute.Email = model.Email;
                            updateinstitute.Institutename = model.Institutename;
                            updateinstitute.LastUpdatedUserId = userid;
                            updateinstitute.Status = "Active";
                            updateinstitute.UPDT_TS = DateTime.Now;
                            updateinstitute.Designation = model.contactDES;
                            updateinstitute.Institutecode = model.InstituteCode;
                            updateinstitute.Designation = model.contactDES;
                            if (model.logo != null)
                            {
                                updateinstitute.logoURL = model.logo;
                            }
                            context.SaveChanges();

                        }
                        return 3;
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public static List<CreateInstituteModel> Getinstitutelist()
        {
            try
            {
                List<CreateInstituteModel> listmodel = new List<CreateInstituteModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from I in context.tblInstituteMaster
                                 join country in context.tblCountries on I.Country equals country.countryID
                                 where (I.Status == "Active")
                                 orderby I.Institutename
                                 select new { I.InstituteId, I.Institutename, I.logoURL, country.countryName, I.Location, I.State }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {

                            listmodel.Add(new CreateInstituteModel()
                            {
                                Sno = i + 1,
                                InstituteId = query[i].InstituteId,
                                Institutename = query[i].Institutename,
                                Countryname = query[i].countryName,
                                Location = query[i].Location,
                                State = query[i].State,
                                logo = query[i].logoURL
                            });
                        }
                    }
                    return listmodel;
                }
            }
            catch (Exception ex)
            {
                List<CreateInstituteModel> listmodel = new List<CreateInstituteModel>();
                return listmodel;
            }
        }
        public static int Deleteinstitute(int Instituteid, string Username)
        {
            try
            {


                using (var context = new IOASDBEntities())
                {


                    var user = context.tblInstituteMaster.Where(I => I.InstituteId == Instituteid).FirstOrDefault();
                    if (user != null)
                    {
                        user.Status = "InActive";
                        user.UPDT_TS = DateTime.Now;
                        user.LastUpdatedUserId = Common.GetUserid(Username);
                        context.SaveChanges();
                    }

                }
                return 4;
            }
            catch (Exception ex)
            {
                return 4;
            }
        }

        public static CreateInstituteModel GetEditinstuite(int instituteid)
        {
            try
            {
                CreateInstituteModel editmodel = new CreateInstituteModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from I in context.tblInstituteMaster
                                 from C in context.tblCountries
                                 where (I.InstituteId == instituteid && I.Country == C.countryID)
                                 select new
                                 {
                                     I.InstituteId,
                                     I.Institutename,
                                     I.Address1,
                                     I.Address2,
                                     I.State,
                                     I.Zipcode,
                                     C.countryID,
                                     C.countryName,
                                     I.Firstname,
                                     I.Lastname,
                                     I.Designation,
                                     I.ContactMobile,
                                     I.Institutecode,
                                     I.logoURL,
                                     I.Email,
                                     I.Location
                                 }).FirstOrDefault();

                    if (query != null)
                    {

                        editmodel.InstituteId = query.InstituteId;
                        editmodel.Institutename = query.Institutename;
                        editmodel.Address1 = query.Address1;
                        editmodel.Address2 = query.Address2;
                        editmodel.State = query.State;
                        editmodel.zipCode = query.Zipcode;
                        editmodel.selCountry = query.countryID;
                        editmodel.FirstName = query.Firstname;
                        editmodel.lastName = query.Lastname;
                        editmodel.InstituteCode = query.Institutecode;
                        editmodel.contactDES = query.Designation;
                        editmodel.ContactMobile = query.ContactMobile;
                        editmodel.Email = query.Email;
                        editmodel.logo = query.logoURL;
                        editmodel.City = query.Location;
                    }
                    return editmodel;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static int Resetpassword(ResetPassword model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var user = context.tblUser.Where(U => U.UserId == model.Userid).FirstOrDefault();
                    if (user != null)
                    {
                        string Username = model.Username;
                        user.Password = Cryptography.Encrypt(model.NewPassword, "LFPassW0rd");
                        user.UPDTDateTS = DateTime.Now;
                        user.LastUpdateUserId = Common.GetUserid(Username);
                        context.SaveChanges();
                        context.Dispose();
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static int deletesrbitemcategory(int srbitmcateid, string Username)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from D in context.tblSRBItemCategory
                                 where (D.SRBItemCategotyId == srbitmcateid)
                                 select D).FirstOrDefault();
                    if (query != null)
                    {
                        query.Status = "InActive";
                        query.UPDT_TS = DateTime.Now;
                        query.UPDT_UserID = Common.GetUserid(Username);
                        context.SaveChanges();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                var msg = ex;
                return -1;
            }
        }
        public static List<SRBItemcategory> SRBcategorylist()
        {
            try
            {
                List<SRBItemcategory> srbcatelist = new List<SRBItemcategory>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from S in context.tblSRBItemCategory
                                 where (S.Status == "Active")
                                 orderby S.Category
                                 select new { S.SRBItemCategotyId, S.Category, S.Asset_f }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            var s = i + 1;
                            srbcatelist.Add(new SRBItemcategory()
                            {
                                sno = s,
                                SRBItemCategotyId = query[i].SRBItemCategotyId,
                                Category = query[i].Category,
                                Asset_f = (bool)query[i].Asset_f
                            });
                        }
                    }
                    return srbcatelist;
                }
            }
            catch (Exception ex)
            {
                List<SRBItemcategory> srbcatelist = new List<SRBItemcategory>();
                return srbcatelist;
            }
        }
        public static List<MasterlistviewModel> getagency(int subtypeid)
        {
            try
            {
                List<MasterlistviewModel> Agency = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {

                    Agency.Add(new MasterlistviewModel()
                    {
                        id = null,
                        name = "Select any"

                    });

                    if (subtypeid > 0)
                    {
                        var query = (from C in context.tblAgencyMaster
                                     where (C.AgencyType == subtypeid)
                                     select C).ToList();
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                Agency.Add(new MasterlistviewModel()
                                {
                                    id = query[i].AgencyId,
                                    name = query[i].AgencyCode + " - " + query[i].AgencyName,

                                });
                            }
                        }
                    }


                    return Agency;

                }


            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<MasterlistviewModel> getcategory(int typeid)
        {
            try
            {
                List<MasterlistviewModel> category = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {


                    if (typeid > 0)
                    {
                        var query = (from C in context.tblSchemes
                                     where (C.ProjectType == typeid)
                                     select C).ToList();
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                category.Add(new MasterlistviewModel()
                                {
                                    id = query[i].SchemeId,
                                    name = query[i].SchemeName,

                                });
                            }
                        }
                    }


                    return category;

                }


            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static MasterlistviewModel getpiemail(int Profid)
        {
            try
            {

                MasterlistviewModel Email = new MasterlistviewModel();

                using (var context = new IOASDBEntities())
                {
                    Email.id = null;
                    Email.name = "";
                    if (Profid > 0)
                    {
                        var query = (from C in context.vwFacultyStaffDetails
                                     where C.UserId == Profid
                                     select C).FirstOrDefault();
                        if (query != null)
                        {

                            Email.id = query.UserId;
                            Email.name = query.Email;

                        }
                    }


                }

                return Email;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static MasterlistviewModel getschemecode(int SchemeId)
        {
            try
            {

                MasterlistviewModel Schemecode = new MasterlistviewModel();

                using (var context = new IOASDBEntities())
                {
                    Schemecode.id = null;
                    Schemecode.name = "";
                    if (SchemeId > 0)
                    {
                        var query = (from C in context.tblSchemes
                                     where C.SchemeId == SchemeId
                                     orderby C.SchemeId
                                     select C).FirstOrDefault();
                        if (query != null)
                        {

                            Schemecode.id = query.SchemeId;
                            Schemecode.name = query.SchemeCode;

                        }
                    }


                }

                return Schemecode;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CodeControllistviewModel> getcategorybyprojecttype(int typeid)
        {
            try
            {

                List<CodeControllistviewModel> category = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    if (typeid == 1)
                    {
                        var query = (from C in context.tblCodeControl
                                     where C.CodeName == "SponProjectCategory"
                                     orderby C.CodeValAbbr
                                     select C).ToList();


                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                category.Add(new CodeControllistviewModel()
                                {
                                    CodeName = query[i].CodeName,
                                    codevalAbbr = query[i].CodeValAbbr,
                                    CodeValDetail = query[i].CodeValDetail
                                });
                            }
                        }
                    }

                    else if (typeid == 2)
                    {
                        var query = (from C in context.tblCodeControl
                                     where C.CodeName == "ConsultancyProjectSubtype"
                                     orderby C.CodeValDetail
                                     select C).ToList();


                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                category.Add(new CodeControllistviewModel()
                                {
                                    CodeName = query[i].CodeName,
                                    codevalAbbr = query[i].CodeValAbbr,
                                    CodeValDetail = query[i].CodeValDetail
                                });
                            }
                        }
                    }

                    else
                    {

                        category.Add(new CodeControllistviewModel()
                        {
                            CodeName = "",
                            codevalAbbr = 0,
                            CodeValDetail = ""
                        });

                    }

                }

                return category;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> getpibydepartment(int Departmentid, int Instituteid)
        {
            try
            {

                List<MasterlistviewModel> PIList = new List<MasterlistviewModel>();
                PIList.Add(new MasterlistviewModel()
                {
                    id = 0,
                    name = "Select Any",
                });
                using (var context = new IOASDBEntities())
                {
                    if (Departmentid > 0)
                    {
                        var query = (from C in context.tblUser
                                     join ins in context.tblInstituteMaster on C.InstituteId equals ins.InstituteId
                                     where (C.RoleId == 7 && C.DepartmentId == Departmentid && C.InstituteId == Instituteid)
                                     orderby C.UserId
                                     select new { C.UserId, C.FirstName, C.LastName, C.EMPCode, ins.Institutecode }).ToList();
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                PIList.Add(new MasterlistviewModel()
                                {
                                    id = query[i].UserId,
                                    name = query[i].EMPCode + "-" + query[i].FirstName + " " + query[i].LastName + "-" + query[i].Institutecode,

                                });
                            }
                        }
                    }

                    else if (Departmentid == 0)
                    {

                        var query = (from C in context.tblUser
                                     join ins in context.tblInstituteMaster on C.InstituteId equals ins.InstituteId
                                     where (C.RoleId == 7 && C.InstituteId == Instituteid)
                                     orderby C.UserId
                                     select new { C.UserId, C.FirstName, C.LastName, C.EMPCode, ins.Institutecode }).ToList();
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                PIList.Add(new MasterlistviewModel()
                                {
                                    id = query[i].UserId,
                                    name = query[i].EMPCode + "-" + query[i].FirstName + " " + query[i].LastName + "-" + query[i].Institutecode,

                                });
                            }
                        }

                    }

                }

                return PIList;
            }
            catch (Exception ex)
            {
                return null;
            }

        }




        public static List<CodeControllistviewModel> getsponprojectsubtype(int typeid)
        {
            try
            {

                List<CodeControllistviewModel> Projectsubtype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    if (typeid == 1)
                    {
                        var query = (from C in context.tblCodeControl
                                     where C.CodeName == "SponsoredProjectSubtype"
                                     orderby C.CodeValAbbr
                                     select C).ToList();


                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                Projectsubtype.Add(new CodeControllistviewModel()
                                {
                                    CodeName = query[i].CodeName,
                                    codevalAbbr = query[i].CodeValAbbr,
                                    CodeValDetail = query[i].CodeValDetail
                                });
                            }
                        }
                    }


                    else
                    {

                        Projectsubtype.Add(new CodeControllistviewModel()
                        {
                            CodeName = "",
                            codevalAbbr = 0,
                            CodeValDetail = ""
                        });

                    }

                }

                return Projectsubtype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<CodeControllistviewModel> getconsprojectsubtype(int typeid)
        {
            try
            {

                List<CodeControllistviewModel> Projectsubtype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    if (typeid == 2)
                    {
                        var query = (from C in context.tblCodeControl
                                     where C.CodeName == "ConsultancyProjectSubtype"
                                     orderby C.CodeValAbbr
                                     select C).ToList();


                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                Projectsubtype.Add(new CodeControllistviewModel()
                                {
                                    CodeName = query[i].CodeName,
                                    codevalAbbr = query[i].CodeValAbbr,
                                    CodeValDetail = query[i].CodeValDetail
                                });
                            }
                        }
                    }
                    else
                    {

                        Projectsubtype.Add(new CodeControllistviewModel()
                        {
                            CodeName = "",
                            codevalAbbr = 0,
                            CodeValDetail = ""
                        });

                    }

                }

                return Projectsubtype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static MasterlistviewModel getagencycode(int agencyid)
        {
            try
            {

                MasterlistviewModel Agencycode = new MasterlistviewModel();

                using (var context = new IOASDBEntities())
                {
                    Agencycode.id = null;
                    Agencycode.name = "";
                    if (agencyid > 0)
                    {
                        var query = (from C in context.tblAgencyMaster
                                     where (C.AgencyId == agencyid)
                                     select C).FirstOrDefault();
                        if (query != null)
                        {

                            Agencycode.id = query.AgencyId;
                            Agencycode.name = query.AgencyCode;

                        }
                    }


                }

                return Agencycode;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<CodeControllistviewModel> getprojectcategory(int subtypeid)
        {
            try
            {

                List<CodeControllistviewModel> Projectsubtype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    if (subtypeid == 1)
                    {
                        var query = (from C in context.tblCodeControl
                                     where C.CodeName == "SponsoredProjectSubtype"
                                     orderby C.CodeValAbbr
                                     select C).ToList();


                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                Projectsubtype.Add(new CodeControllistviewModel()
                                {
                                    CodeName = query[i].CodeName,
                                    codevalAbbr = query[i].CodeValAbbr,
                                    CodeValDetail = query[i].CodeValDetail
                                });
                            }
                        }
                    }


                    else
                    {

                        Projectsubtype.Add(new CodeControllistviewModel()
                        {
                            CodeName = "",
                            codevalAbbr = 0,
                            CodeValDetail = ""
                        });

                    }

                }

                return Projectsubtype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<MasterlistviewModel> getschemebyagency(int agency)
        {
            try
            {

                List<MasterlistviewModel> Scheme = new List<MasterlistviewModel>();
                Scheme.Add(new MasterlistviewModel()
                {
                    id = 0,
                    name = "Select any"

                });
                using (var context = new IOASDBEntities())
                {

                    var query = (from C in context.tblAgencyMaster
                                 where C.AgencyId == agency
                                 select C).FirstOrDefault();
                    var Schemeid = query.Scheme;
                    var scheme = (from s in context.tblSchemes
                                  where s.SchemeId == Schemeid
                                  select s).FirstOrDefault();
                    Scheme.Add(new MasterlistviewModel()
                    {
                        id = query.AgencyId,
                        name = scheme.SchemeName,
                    });

                }

                return Scheme;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> getsponprojectcategory(int typeid)
        {
            try
            {

                List<MasterlistviewModel> Scheme = new List<MasterlistviewModel>();
                Scheme.Add(new MasterlistviewModel()
                {
                    id = 0,
                    name = "Select any"

                });
                using (var context = new IOASDBEntities())
                {
                    if (typeid == 1 || typeid == 2)
                    {
                        var query = (from C in context.tblSchemes
                                     where C.ProjectType == typeid
                                     orderby C.SchemeId
                                     select C).ToList();


                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                Scheme.Add(new MasterlistviewModel()
                                {
                                    id = query[i].SchemeId,
                                    name = query[i].SchemeName
                                });
                            }
                        }

                    }

                }

                return Scheme;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<MasterlistviewModel> getAgencybysubtype(int subtypeid)
        {
            try
            {

                List<MasterlistviewModel> Agency = new List<MasterlistviewModel>();
                Agency.Add(new MasterlistviewModel()
                {
                    id = 0,
                    name = "Select any"

                });
                using (var context = new IOASDBEntities())
                {

                    var query = (from C in context.tblAgencyMaster
                                 where C.AgencyType == subtypeid
                                 select C).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Agency.Add(new MasterlistviewModel()
                            {
                                id = query[i].AgencyId,
                                name = query[i].AgencyName,
                            });
                        }
                    }

                }


                return Agency;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<MasterlistviewModel> getOtherInstCoPIList(int Departmentid, int Instituteid)
        {
            try
            {

                List<MasterlistviewModel> PIList = new List<MasterlistviewModel>();
                PIList.Add(new MasterlistviewModel()
                {
                    id = null,
                    name = "Select any"

                });
                using (var context = new IOASDBEntities())
                {
                    if (Departmentid > 0)
                    {
                        var query = (from C in context.tblUser
                                     join ins in context.tblInstituteMaster on C.InstituteId equals ins.InstituteId
                                     where (C.RoleId == 7 && C.DepartmentId == Departmentid && C.InstituteId == Instituteid)
                                     orderby C.UserId
                                     select new { C.UserId, C.FirstName, C.LastName, C.EMPCode, ins.Institutecode }).ToList();
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                PIList.Add(new MasterlistviewModel()
                                {
                                    id = query[i].UserId,
                                    name = query[i].EMPCode + "-" + query[i].FirstName + " " + query[i].LastName + "-" + query[i].Institutecode,

                                });
                            }
                        }
                    }


                }

                return PIList;
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        public static List<MasterlistviewModel> getPIDesignation(int PIid)
        {
            try
            {

                List<MasterlistviewModel> PIList = new List<MasterlistviewModel>();
                PIList.Add(new MasterlistviewModel()
                {
                    id = null,
                    name = "Select any"

                });
                using (var context = new IOASDBEntities())
                {
                    if (PIid > 0)
                    {
                        var query = (from C in context.tblUser
                                     join cc in context.tblCodeControl on
                                     new { desig = Convert.ToInt32(C.Designation), codeName = "FacultyCadre" } equals
                                     new { desig = cc.CodeValAbbr, codeName = cc.CodeName }
                                     where (C.RoleId == 7 && C.UserId == PIid)
                                     orderby C.UserId
                                     select new { C, cc }).ToList();
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                PIList.Add(new MasterlistviewModel()
                                {
                                    id = query[i].C.Designation,
                                    name = query[i].cc.CodeValDetail,

                                });
                            }
                        }
                    }


                }

                return PIList;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static MasterlistviewModel getSchemecode(int SchemeId)
        {
            try
            {

                MasterlistviewModel Schemecode = new MasterlistviewModel();

                using (var context = new IOASDBEntities())
                {
                    Schemecode.id = null;
                    Schemecode.name = "";
                    if (SchemeId > 0)
                    {
                        var query = (from C in context.tblSchemes
                                     where (C.SchemeId == SchemeId)
                                     select C).SingleOrDefault();
                        if (query != null)
                        {
                            Schemecode.id = query.SchemeId;
                            Schemecode.name = query.SchemeCode;
                        }
                    }


                }

                return Schemecode;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static int Accountgroup(Accountgroupmodel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (model.AccountGroupId == 0)
                    {

                        var chkacctgroup = context.tblAccountGroup.FirstOrDefault(M => (M.AccountGroup == model.AccountGroup || M.AccountGroupCode == model.AccountGroupCode) && M.Status == "Active");
                        if (chkacctgroup != null)
                            return 2;
                        tblAccountGroup addaccount = new tblAccountGroup();
                        if (model.parentgroupId == 0)
                        {
                            var SeqNbr = (from M in context.tblAccountGroup
                                          where (M.Is_Subgroup == false && M.AccountType == model.AccountType)
                                          select M.SeqNbr).Max();
                            addaccount.AccountGroup = model.AccountGroup;
                            addaccount.AccountType = model.AccountType;
                            addaccount.AccountGroupCode = model.AccountGroupCode;
                            addaccount.SeqNbr = Convert.ToInt32(SeqNbr) + 1;
                            addaccount.Is_Subgroup = false;
                            addaccount.Status = "Active";
                            addaccount.CreatedTS = DateTime.Now;
                            addaccount.CreatedUserId = model.userid;
                            context.tblAccountGroup.Add(addaccount);
                            context.SaveChanges();
                            // return 1;
                        }
                        else
                        {
                            var SeqNbr = (from M in context.tblAccountGroup
                                          where (M.Is_Subgroup == true && M.ParentgroupId == model.parentgroupId)
                                          select M.SeqNbr).Max();
                            addaccount.AccountGroup = model.AccountGroup;
                            addaccount.AccountType = model.AccountType;
                            addaccount.AccountGroupCode = model.AccountGroupCode;
                            addaccount.Is_Subgroup = true;
                            addaccount.ParentgroupId = model.parentgroupId;
                            addaccount.SeqNbr = Convert.ToInt32(SeqNbr) + 1;
                            addaccount.CreatedTS = DateTime.Now;
                            addaccount.CreatedUserId = model.userid;
                            addaccount.Status = "Active";
                            context.tblAccountGroup.Add(addaccount);
                            context.SaveChanges();
                            //return 1;
                        }
                    }
                    //else
                    //{
                    //    var acctupdate = context.tblAccountGroup.Where(M => M.AccountGroupId == model.AccountGroupId).FirstOrDefault();
                    //    if(acctupdate!=null)
                    //    {
                    //        if (acctupdate.AccountGroup != model.AccountGroup)
                    //        {
                    //            var chkacctgroup = context.tblAccountGroup.FirstOrDefault(M => M.AccountGroup == model.AccountGroup);
                    //            if (chkacctgroup != null)
                    //                return 2;
                    //            if (acctupdate.AccountGroupCode != model.AccountGroupCode)
                    //            {
                    //                var chkacctgroupcode = context.tblAccountGroup.FirstOrDefault(M => M.AccountGroupCode == model.AccountGroupCode);
                    //                if (chkacctgroupcode != null)
                    //                    return 2;
                    //            }
                    //            acctupdate.AccountGroup = model.AccountGroup;
                    //            acctupdate.AccountType = model.AccountType;
                    //            acctupdate.AccountGroupCode = model.AccountGroupCode;
                    //            acctupdate.LastUpdatedTS = DateTime.Now;
                    //            acctupdate.LastUpdatedUserId = model.userid;
                    //            context.SaveChanges();
                    //        }
                    //        else
                    //        {
                    //            if (acctupdate.AccountGroupCode != model.AccountGroupCode)
                    //            {
                    //                var chkacctgroupcode = context.tblAccountGroup.FirstOrDefault(M => M.AccountGroupCode == model.AccountGroupCode);
                    //                if (chkacctgroupcode != null)
                    //                    return 2;
                    //                acctupdate.AccountGroup = model.AccountGroup;
                    //                acctupdate.AccountType = model.AccountType;
                    //                acctupdate.AccountGroupCode = model.AccountGroupCode;
                    //                acctupdate.LastUpdatedTS = DateTime.Now;
                    //                acctupdate.LastUpdatedUserId = model.userid;
                    //                context.SaveChanges();
                    //            }
                    //            else
                    //            {
                    //                acctupdate.AccountGroup = model.AccountGroup;
                    //                acctupdate.AccountType = model.AccountType;
                    //                acctupdate.AccountGroupCode = model.AccountGroupCode;
                    //                acctupdate.LastUpdatedTS = DateTime.Now;
                    //                acctupdate.LastUpdatedUserId = model.userid;
                    //                context.SaveChanges();
                    //            }

                    //        }
                    //    }
                    //    return 3;
                    //}
                    return 1;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }

        }
        public static string Accountgroupcode(int accttypeid)
        {
            try
            {
                string accountcode = "";
                using (var context = new IOASDBEntities())
                {
                    var accttypename = (from AT in context.tblAccountType
                                        where (AT.AccountTypeId == accttypeid)
                                        select AT.AccountType).FirstOrDefault();
                    var query = (from M in context.tblAccountGroup
                                 where (M.Is_Subgroup == false && M.AccountType == accttypeid)
                                 select M.SeqNbr).Max();
                    //if(query!=null)
                    //{
                    var maxnum = (Convert.ToInt32(query) + 1).ToString("00");
                    string acttype = accttypename.Substring(0, 1);
                    accountcode = acttype + maxnum;
                    //}
                }
                return accountcode;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string Parentgroupcode(int parentgrpId)
        {
            try
            {
                string Parentcode = "";
                using (var context = new IOASDBEntities())
                {
                    var acctcode = (from AG in context.tblAccountGroup
                                    where (AG.AccountGroupId == parentgrpId)
                                    select AG.AccountGroupCode).FirstOrDefault();
                    var query = (from PG in context.tblAccountGroup
                                 where (PG.ParentgroupId == parentgrpId && PG.Is_Subgroup == true)
                                 select PG.SeqNbr).Max();
                    var maxmum = (Convert.ToInt32(query) + 1).ToString("00");
                    Parentcode = acctcode + '-' + maxmum;
                }
                return Parentcode;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static Accountgroupmodel AccountSubGroupAdd(int accountgrpId)
        {
            try
            {
                Accountgroupmodel Subgroupadd = new Accountgroupmodel();
                using (var context = new IOASDBEntities())
                {
                    var accountcode = (from AG in context.tblAccountGroup
                                       where (AG.AccountGroupId == accountgrpId)
                                       select AG.AccountGroupCode).FirstOrDefault();
                    var query = (from PG in context.tblAccountGroup
                                 where (PG.ParentgroupId == accountgrpId && PG.Is_Subgroup == true)
                                 select PG.SeqNbr).Max();
                    var maxmum = (Convert.ToInt32(query) + 1).ToString("00");
                    Subgroupadd.parentgroupId = accountgrpId;
                    Subgroupadd.AccountGroupCode = accountcode + '-' + maxmum;
                }
                return Subgroupadd;
            }
            catch (Exception ex)
            {
                Accountgroupmodel Subgroupadd = new Accountgroupmodel();
                return Subgroupadd;
            }
        }
        public static List<Accountgroupmodel> Getaccountgrouplist(Accountgroupmodel model)
        {
            try
            {
                List<Accountgroupmodel> acctgroup = new List<Accountgroupmodel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from A in context.tblAccountGroup
                                 join AT in context.tblAccountType on A.AccountType equals AT.AccountTypeId into AG
                                 from c in AG.DefaultIfEmpty()
                                 where A.Status == "Active"
                                 && (A.AccountGroup.Contains(model.AccountGroupSearch) || model.AccountGroupSearch == null)
                                 && (A.AccountGroupCode.Contains(model.AccountGroupCodeSearch) || model.AccountGroupCodeSearch == null)
                                 && (A.AccountType == model.AccountTypeSearch || model.AccountTypeSearch == null)
                                 orderby A.AccountGroup
                                 select new { A.AccountGroup, A.AccountGroupCode, A.AccountGroupId, c.AccountType }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            var accttype = "";
                            var s = i + 1;
                            if (query[i].AccountType == null)
                            {
                                accttype = "Sub Group";
                            }
                            else
                            {
                                accttype = query[i].AccountType;
                            }
                            acctgroup.Add(new Accountgroupmodel()
                            {
                                sno = s,
                                AccountGroupId = query[i].AccountGroupId,
                                AccountGroup = query[i].AccountGroup,
                                AccountGroupCode = query[i].AccountGroupCode,
                                //AccountType=query[i].AccountTypeId,
                                Accounttypename = accttype

                            });
                        }
                    }
                    return acctgroup;
                }
            }
            catch (Exception ex)
            {
                List<Accountgroupmodel> acctgroup = new List<Accountgroupmodel>();
                return acctgroup;
            }
        }
        public static Accountgroupmodel Editaccountgroup(int acccountgrpId)
        {
            try
            {
                Accountgroupmodel editacct = new Accountgroupmodel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from AG in context.tblAccountGroup
                                 where (AG.AccountGroupId == acccountgrpId)
                                 select AG).FirstOrDefault();
                    if (query != null)
                    {
                        editacct.AccountGroupId = query.AccountGroupId;
                        editacct.AccountGroup = query.AccountGroup;
                        editacct.AccountType = (Int32)query.AccountType;
                        editacct.AccountGroupCode = query.AccountGroupCode;
                    }
                    return editacct;
                }
            }
            catch (Exception ex)
            {
                Accountgroupmodel editacct = new Accountgroupmodel();
                return editacct;
            }
        }
        public static int DeleteAccountgroup(int acccountgrpId, string Username)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var chkparentgroup = context.tblAccountGroup.Where(A => A.ParentgroupId == acccountgrpId).FirstOrDefault();
                    if (chkparentgroup != null)
                    {
                        return 1;
                    }
                    else
                    {
                        var acctgrp = context.tblAccountGroup.Where(AG => AG.AccountGroupId == acccountgrpId).FirstOrDefault();
                        if (acctgrp != null)
                        {
                            acctgrp.Status = "InActive";
                            acctgrp.LastUpdatedTS = DateTime.Now;
                            acctgrp.LastUpdatedUserId = Common.GetUserid(Username);
                            context.SaveChanges();
                        }
                        return 2;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex;
                return -1;
            }
        }
        public static int AddSRBitemcategory(SRBItemcategory model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (model.SRBItemCategotyId == 0)
                    {
                        var chksrbitem = context.tblSRBItemCategory.Where(M => M.Category == model.Category && M.Status == "Active").FirstOrDefault();
                        if (chksrbitem != null)
                            return 2;
                        tblSRBItemCategory addsrbitm = new tblSRBItemCategory();
                        addsrbitm.Category = model.Category;
                        addsrbitm.Asset_f = model.Asset_f;
                        addsrbitm.CRTD_TS = DateTime.Now;
                        addsrbitm.CRTD_UserID = model.userid;
                        addsrbitm.Status = "Active";
                        context.tblSRBItemCategory.Add(addsrbitm);
                        context.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        var uptsrbitem = context.tblSRBItemCategory.Where(M => M.SRBItemCategotyId == model.SRBItemCategotyId).FirstOrDefault();
                        if (uptsrbitem.Category != model.Category)
                        {
                            var chksrbitem = context.tblSRBItemCategory.Where(M => M.Category == model.Category && M.Status == "Active").FirstOrDefault();
                            if (chksrbitem != null)
                                return 2;
                            uptsrbitem.Category = model.Category;
                            uptsrbitem.Asset_f = model.Asset_f;
                            uptsrbitem.UPDT_TS = DateTime.Now;
                            uptsrbitem.UPDT_UserID = model.userid;
                            uptsrbitem.Status = "Active";
                            context.SaveChanges();
                            return 3;
                        }
                        if (uptsrbitem != null)
                        {
                            uptsrbitem.Category = model.Category;
                            uptsrbitem.Asset_f = model.Asset_f;
                            uptsrbitem.UPDT_TS = DateTime.Now;
                            uptsrbitem.UPDT_UserID = model.userid;
                            uptsrbitem.Status = "Active";
                            context.SaveChanges();
                        }
                        return 3;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex;
                return -1;
            }
        }
        public static SRBItemcategory Editsrbitemcategory(int srbitmcateid)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    SRBItemcategory editsrb = new SRBItemcategory();

                    var query = (from E in context.tblSRBItemCategory
                                 where (E.SRBItemCategotyId == srbitmcateid)
                                 select new { E.SRBItemCategotyId, E.Category, E.Asset_f }).FirstOrDefault();
                    if (query != null)
                    {
                        editsrb.SRBItemCategotyId = query.SRBItemCategotyId;
                        editsrb.Category = query.Category;
                        editsrb.Asset_f = (bool)query.Asset_f;

                    }
                    return editsrb;
                }
            }
            catch (Exception ex)
            {
                var msg = ex;
                SRBItemcategory editsrb = new SRBItemcategory();
                return editsrb;
            }
        }
        public static string GetAgencyCode()
        {
            try
            {
                string agycode = "";
                using (var context = new IOASDBEntities())
                {

                    var agencymax = (from A in context.tblAgencyMaster
                                     where (A.AgencyType == 2)
                                     select A.SeqNbr).Max();
                    agycode = "A" + (Convert.ToInt32(agencymax) + 1).ToString("00");
                }
                return agycode;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        //public static List<MasterlistviewModel> getPIList(int Departmentid, int Instituteid)
        //{
        //    try
        //    {

        //        List<MasterlistviewModel> PIList = new List<MasterlistviewModel>();
        //        PIList.Add(new MasterlistviewModel()
        //        {
        //            id = null,
        //            name = "Select any"

        //        });
        //        using (var context = new IOASDBEntities())
        //        {
        //            if (Departmentid > 0)
        //            {
        //                var query = (from C in context.tblUser
        //                             join ins in context.tblInstituteMaster on C.InstituteId equals ins.InstituteId
        //                             where (C.RoleId == 7 && C.DepartmentId == Departmentid && C.InstituteId == Instituteid)
        //                             orderby C.UserId
        //                             select new { C.UserId, C.FirstName, C.LastName, C.EMPCode, ins.Institutecode }).ToList();
        //                if (query.Count > 0)
        //                {
        //                    for (int i = 0; i < query.Count; i++)
        //                    {
        //                        PIList.Add(new MasterlistviewModel()
        //                        {
        //                            id = query[i].UserId,
        //                            name = query[i].EMPCode + "-" + query[i].FirstName + " " + query[i].LastName + "-" + query[i].Institutecode,

        //                        });
        //                    }
        //                }
        //            }


        //        }

        //        return PIList;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}


        public static List<CommitmentResultModel> GetCommitmentDetails()
        {
            try
            {
                List<CommitmentResultModel> GetDetail = new List<CommitmentResultModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCommitment
                                     //where C.Status == "Active"
                                 orderby C.CommitmentId descending
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {

                            GetDetail.Add(new CommitmentResultModel()
                            {
                                SlNo = i + 1,
                                ComitmentId = query[i].CommitmentId,
                                CommitmentType = Common.getCommitmentName(query[i].CommitmentType ?? 0),
                                CommitmentNo = query[i].CommitmentNumber,
                                projectNumber = Common.GetProjectNumber(query[i].ProjectId ?? 0),
                                VendorName = "NA",
                                CommitmentAmount = query[i].CommitmentAmount ?? 0,
                                AmountSpent = query[i].AmountSpent ?? 0,
                                CreatedDate = String.Format("{0:s}", query[i].CRTD_TS),
                                Status = query[i].Status,
                            });

                        }
                    }
                }
                return GetDetail;
            }
            catch (Exception ex)
            {
                List<CommitmentResultModel> GetDetail = new List<CommitmentResultModel>();
                return GetDetail;
            }

        }
        public static CommitmentSearchModel GetCommitmentDetails(CommitmentSearchModel model, int page, int pageSize, DateFilterModel CreatedDate)
        {
            try
            {
                CommitmentSearchModel list = new CommitmentSearchModel();
                List<CommitmentResultModel> GetDetail = new List<CommitmentResultModel>();
                using (var context = new IOASDBEntities())
                {
                    int skiprec = 0;
                    if (page == 1)
                    {
                        skiprec = 0;
                    }
                    else
                    {
                        skiprec = (page - 1) * pageSize;
                    }
                    if (CreatedDate.to != null)
                        CreatedDate.to = CreatedDate.to.Value.Date.AddDays(1).AddTicks(-2);
                    string absoluteLink = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + "/CoreAccounts/ShowUploadedDocument";
                    var Path = "~/Content/CommitmentDocument/";
                    GetDetail = (from C in context.tblCommitment
                                     // from D  in context.tblCommitmentDetails
                                 from P in context.tblProject
                                     //where C.Status == "Active"
                                 where (C.ProjectId == P.ProjectId)
                                 && (P.ProjectNumber.Contains(model.SearchProjectNumber) || model.SearchProjectNumber == null)
                                 && (C.CommitmentNumber.Contains(model.SearchCommitmentNumber) || model.SearchCommitmentNumber == null)
                                 && ((C.CRTD_TS > CreatedDate.@from && C.CRTD_TS < CreatedDate.to) || (CreatedDate.@from == null && CreatedDate.to == null))
                                 && (C.CommitmentAmount == model.CommitmentAmount || model.CommitmentAmount == null)
                                 && (C.CommitmentBalance == model.AvilableAmount || model.AvilableAmount == null)
                                 && (C.Status.Contains(model.Status) || model.Status == null)
                                 orderby C.CommitmentId descending
                                 select new { C }).AsEnumerable()
                                 .Select((x, index) => new CommitmentResultModel()
                                 {
                                     SlNo = index + 1,
                                     ComitmentId = x.C.CommitmentId,
                                     CommitmentType = Common.getCommitmentName(x.C.CommitmentType ?? 0),
                                     CommitmentNo = x.C.CommitmentNumber,
                                     projectNumber = Common.GetProjectNumber(x.C.ProjectId ?? 0),
                                     VendorName = "NA",
                                     CommitmentAmount = x.C.CommitmentAmount ?? 0,
                                     AmountSpent = x.C.CommitmentBalance ?? 0,
                                     CreatedDate = String.Format("{0:s}", x.C.CRTD_TS),
                                     Status = x.C.Status,
                                     NewGenerateDoc = x.C.DocName,
                                     Newhref = absoluteLink + "?file=" + x.C.AttachmentPath + "&filepath=" + Path
                                 }).Skip(skiprec).Take(pageSize).ToList();
                    list.TotalRecords = (from C in context.tblCommitment

                                         from P in context.tblProject
                                             //where C.Status == "Active"
                                         where (C.ProjectId == P.ProjectId)
                                         && (P.ProjectNumber.Contains(model.SearchProjectNumber) || model.SearchProjectNumber == null)
                                         && (C.CommitmentNumber.Contains(model.SearchCommitmentNumber) || model.SearchCommitmentNumber == null)
                                         && ((C.CRTD_TS > CreatedDate.@from && C.CRTD_TS < CreatedDate.to) || (CreatedDate.@from == null && CreatedDate.to == null))
                                         && (C.CommitmentAmount == model.CommitmentAmount || model.CommitmentAmount == null)
                                         && (C.CommitmentBalance == model.AvilableAmount || model.AvilableAmount == null)
                                         && (C.Status.Contains(model.Status) || model.Status == null)
                                         orderby C.CommitmentId descending
                                         select new { C }).Count();

                    //if (query.Count > 0)
                    //{
                    //    for (int i = 0; i < query.Count; i++)
                    //    {

                    //        GetDetail.Add(new CommitmentResultModel()
                    //        {
                    //            SlNo = i + 1,
                    //            ComitmentId = query[i].C.CommitmentId,
                    //            CommitmentType = Common.getCommitmentName(query[i].C.CommitmentType ?? 0),
                    //            CommitmentNo = query[i].C.CommitmentNumber,
                    //            projectNumber = Common.GetProjectNumber(query[i].C.ProjectId ?? 0),
                    //            VendorName = "NA",
                    //            CommitmentAmount = query[i].C.CommitmentAmount ?? 0,
                    //            AmountSpent = query[i].C.CommitmentBalance ?? 0,
                    //            CreatedDate = String.Format("{0:s}", query[i].C.CRTD_TS),
                    //            Status = query[i].C.Status,
                    //            NewGenerateDoc = query[i].C.DocName,
                    //            Newhref = absoluteLink + "?file=" + query[i].C.AttachmentPath + "&filepath=" + Path
                    //        });

                    //    }
                    //}
                }
                list.CommitmentList = GetDetail;
                return list;
            }
            catch (Exception ex)
            {
                CommitmentSearchModel list = new CommitmentSearchModel();
                return list;
            }

        }
        public Tuple<int, string> SaveCommitDetails(CommitmentModel model, int UserId, bool isActive = false)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    using (var transaction = context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        var validateCommit = Common.ValidateCommitment(model.SelProjectNumber, model.selAllocationHead, model.AllocationValue);
                        if (validateCommit != "Valid")
                        {
                            transaction.Rollback();
                            return Tuple.Create(-1, validateCommit);
                        }
                        if(model.commitmentValue!=model.AllocationValue)
                        {
                            transaction.Rollback();
                            return Tuple.Create(-1, "Commitment amount and allocated Commitment amount difference");
                        }
                        if (model.commitmentId > 0)
                        {
                            int cmtId = model.commitmentId ?? 0;
                            var Query = (from C in context.tblCommitment where C.CommitmentId == model.commitmentId select C).FirstOrDefault();
                            if (Query != null)
                            {
                                string docpath = "";
                                string docname = "";
                                if (model.file != null)
                                {
                                    docname = System.IO.Path.GetFileName(model.file.FileName);
                                    var docfileId = Guid.NewGuid().ToString();
                                    docpath = docfileId + "_" + docname;
                                    /*Saving the file in server folder*/
                                    model.file.UploadFile("CommitmentDocument", docpath);
                                    Query.AttachmentPath = docpath;
                                    Query.DocName = docname;
                                }
                                Query.AttachmentName = model.AttachName;
                                Query.CommitmentType = model.selCommitmentType;
                                Query.ProjectId = model.SelProjectNumber;
                                Query.CommitmentAmount = model.commitmentValue;
                                Query.FundingBody = model.selFundingBody;
                                Query.CommitmentBalance = model.commitmentValue;
                                Query.PurchaseOrder = model.PONumber;
                                Query.VendorNameStr = model.VendorName;
                                Query.ItemDescription = model.ItemDescription;
                                Query.Description = model.Remarks;
                                Query.Purpose = model.selPurpose;
                                Query.Currency = model.selCurrency;
                                Query.CurrencyRate = model.currencyRate;
                                Query.AdditionalCharge = model.AdditionalCharge;
                                Query.StartDate = model.StartDate;
                                Query.CloseDate = model.CloseDate;
                                Query.BasicPay = model.BasicPay;
                                Query.MedicalAllowance = model.MedicalAllowance;
                                Query.EmployeeId = model.EmployeeId;
                                Query.CastEmployeeId = model.CastEmployeeId;
                                Query.Total = model.Total;
                                Query.IsDeansApproval = model.IsDeansApproval;
                                Query.AttachmentName = model.AttachName;
                                Query.UPDT_UserID = UserId;
                                Query.UPDT_TS = DateTime.Now;
                                Query.BankID = model.BankId;
                                context.SaveChanges();
                                var detailQry = (from D in context.tblCommitmentDetails where D.CommitmentId == model.commitmentId select D).ToList();
                                if (detailQry != null)
                                {
                                    // remove old Commit details
                                    context.tblCommitmentDetails.RemoveRange(context.tblCommitmentDetails.Where(m => m.CommitmentId == model.commitmentId));
                                    context.SaveChanges();
                                    // insert new Commit details
                                    tblCommitmentDetails comitDtls = new tblCommitmentDetails();
                                    comitDtls.ForignCurrencyValue = model.ForeignCurrencyValue;
                                    comitDtls.CommitmentId = model.commitmentId;
                                    comitDtls.AllocationHeadId = model.selAllocationHead;
                                    comitDtls.Amount = model.AllocationValue;
                                    comitDtls.BalanceAmount = model.AllocationValue;
                                    comitDtls.Subhead = model.selSubhead;
                                    context.tblCommitmentDetails.Add(comitDtls);
                                    context.SaveChanges();

                                }
                                transaction.Commit();
                                return Tuple.Create(cmtId, "Success");
                            }
                            else
                            {
                                transaction.Rollback();
                                return Tuple.Create(-1, "Something went wrong please contact administrator");
                            }

                        }
                        else
                        {
                            //var allocData = Common.getAllocationValue(model.SelProjectNumber, model.selAllocationHead);
                            //if (model.AllocationValue > allocData.TotalAllocation)
                            //{
                            //    return -1;
                            //}
                            tblCommitment Comit = new tblCommitment();
                            string docpath = "";
                            string docname = "";
                            if (model.file != null)
                            {
                                docname = System.IO.Path.GetFileName(model.file.FileName);
                                var docfileId = Guid.NewGuid().ToString();
                                docpath = docfileId + "_" + docname;
                                /*Saving the file in server folder*/
                                model.file.UploadFile("CommitmentDocument", docpath);
                            }
                            Comit.AttachmentPath = docpath;
                            Comit.AttachmentName = model.AttachName;
                            Comit.DocName = docname;
                            Comit.CommitmentType = model.selCommitmentType;
                            Comit.ProjectId = model.SelProjectNumber;

                            Comit.SequenceNo = Common.GetCommitmentSequenceno();
                            Comit.PurchaseOrder = model.PONumber;
                            Comit.VendorNameStr = model.VendorName;
                            Comit.ItemDescription = model.ItemDescription;
                            Comit.Description = model.Remarks;
                            Comit.CommitmentAmount = model.commitmentValue;
                            Comit.CommitmentBalance = model.commitmentValue;
                            Comit.FundingBody = model.selFundingBody;
                            Comit.ProjectType = model.selProjectType;
                            Comit.Purpose = model.selPurpose;
                            Comit.Currency = model.selCurrency;
                            Comit.CurrencyRate = model.currencyRate;
                            Comit.AdditionalCharge = model.AdditionalCharge;
                            Comit.Reference = model.selRequestRefrence;
                            Comit.ReferenceNo = model.selRefNo;
                            Comit.CRTD_UserID = UserId;
                            Comit.CRTD_TS = DateTime.Now;
                            if (!isActive)
                                Comit.Status = "Open";
                            else
                                Comit.Status = "Active";
                            Comit.EmailDate = model.EmailDate;
                            Comit.StartDate = model.StartDate;
                            Comit.CloseDate = model.CloseDate;
                            Comit.BasicPay = model.BasicPay;
                            Comit.MedicalAllowance = model.MedicalAllowance;
                            Comit.EmployeeId = model.EmployeeId;
                            Comit.CastEmployeeId = model.CastEmployeeId;
                            Comit.Total = model.Total;
                            Comit.IsDeansApproval = model.IsDeansApproval;
                            Comit.AttachmentName = model.AttachName;
                            Comit.CommitmentNumber = Common.GetNewCommitmentNo();
                            Comit.FinYearId = model.FinYearId;
                            Comit.BankID = model.BankId;
                            context.tblCommitment.Add(Comit);
                            context.SaveChanges();
                            int CommitId = Comit.CommitmentId;

                            model.CommitmentNo = Comit.CommitmentNumber;


                            tblCommitmentDetails comitDtls = new tblCommitmentDetails();
                            comitDtls.ForignCurrencyValue = model.ForeignCurrencyValue;
                            comitDtls.CommitmentId = CommitId;
                            comitDtls.AllocationHeadId = model.selAllocationHead;
                            comitDtls.Amount = model.AllocationValue;
                            comitDtls.BalanceAmount = model.AllocationValue;
                            comitDtls.Subhead = model.selSubhead;
                            context.tblCommitmentDetails.Add(comitDtls);
                            context.SaveChanges();

                            if (Common.IsUAYProject(model.SelProjectNumber))
                                UAYEmailSend(UserId, model);
                            transaction.Commit();
                            return Tuple.Create(CommitId, "Success");
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                return Tuple.Create(-1, "Something went wrong please contact administrator");
            }
        }
        //public int SaveCommitDetails(CommitmentModel model, int UserId, bool isActive = false)
        //{
        //    try
        //    {
        //        using (var context = new IOASDBEntities())
        //        {
        //            int CommitId = 0;
        //            if (model.commitmentId > 0)
        //            {
        //                CommitId = model.commitmentId ?? 0;
        //                var Query = (from C in context.tblCommitment where C.CommitmentId == model.commitmentId select C).FirstOrDefault();
        //                if (Query != null)
        //                {
        //                    Query.CommitmentType = model.selCommitmentType;
        //                    Query.ProjectId = model.SelProjectNumber;
        //                    Query.PurchaseOrder = model.PONumber;
        //                    Query.CommitmentAmount = model.commitmentValue;
        //                    Query.CommitmentBalance = model.commitmentValue;
        //                    Query.VendorName = model.selVendor;
        //                    Query.Description = model.Remarks;
        //                    Query.Purpose = model.selPurpose;
        //                    Query.Currency = model.selCurrency;
        //                    Query.CurrencyRate = model.currencyRate;
        //                    Query.UPDT_UserID = UserId;
        //                    Query.UPDT_TS = DateTime.Now;
        //                    context.SaveChanges();
        //                    var detailQry = (from D in context.tblCommitmentDetails where D.CommitmentId == model.commitmentId select D).ToList();
        //                    if (detailQry.Count > 0)
        //                    {
        //                        // remove old Commit details
        //                        context.tblCommitmentDetails.RemoveRange(context.tblCommitmentDetails.Where(m => m.CommitmentId == model.commitmentId));
        //                        context.SaveChanges();
        //                        // insert new Commit details
        //                        for (int i = 0; i < model.selAllocationHead.Length; i++)
        //                        {
        //                            tblCommitmentDetails comitDtls = new tblCommitmentDetails();
        //                            comitDtls.CommitmentId = model.commitmentId;
        //                            comitDtls.AllocationHeadId = model.selAllocationHead[i];
        //                            comitDtls.Amount = model.AllocationValue[i];
        //                            comitDtls.BalanceAmount = model.AllocationValue[i];
        //                            context.tblCommitmentDetails.Add(comitDtls);
        //                            context.SaveChanges();
        //                        }
        //                    }

        //                    return CommitId;
        //                }
        //                else
        //                    return -1;
        //            }
        //            else
        //            {
        //                tblCommitment Comit = new tblCommitment();
        //                Comit.CommitmentType = model.selCommitmentType;
        //                Comit.ProjectId = model.SelProjectNumber;
        //                Comit.CommitmentNumber = Common.GetNewCommitmentNo(model.selProjectType);
        //                Comit.PurchaseOrder = model.PONumber;
        //                Comit.VendorName = model.selVendor;
        //                Comit.Description = model.Remarks;
        //                Comit.CommitmentAmount = model.commitmentValue;
        //                Comit.CommitmentBalance = model.commitmentValue;
        //                Comit.ProjectType = model.selProjectType;
        //                Comit.Purpose = model.selPurpose;
        //                Comit.Currency = model.selCurrency;
        //                Comit.CurrencyRate = model.currencyRate;
        //                Comit.Reference = model.selRequestRefrence;
        //                Comit.ReferenceNo = model.selRefNo;
        //                Comit.CRTD_UserID = UserId;
        //                Comit.CRTD_TS = DateTime.Now;
        //                if (!isActive)
        //                    Comit.Status = "Open";
        //                else
        //                    Comit.Status = "Active";
        //                Comit.EmailDate = model.EmailDate;
        //                context.tblCommitment.Add(Comit);
        //                context.SaveChanges();
        //                CommitId = Comit.CommitmentId;
        //                for (int i = 0; i < model.selAllocationHead.Length; i++)
        //                {
        //                    tblCommitmentDetails comitDtls = new tblCommitmentDetails();
        //                    comitDtls.CommitmentId = CommitId;
        //                    comitDtls.AllocationHeadId = model.selAllocationHead[i];
        //                    comitDtls.Amount = model.AllocationValue[i];
        //                    comitDtls.BalanceAmount = model.AllocationValue[i];
        //                    context.tblCommitmentDetails.Add(comitDtls);
        //                    context.SaveChanges();
        //                }
        //                return CommitId;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }
        //}
        public void UAYEmailSend(int uId, CommitmentModel cModel)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    CultureInfo Indian = new CultureInfo("hi-IN");
                    UAYEmailModel model = new UAYEmailModel();
                    model.crt_Name = Common.GetUserFirstName(uId);
                    model.projectNo = Common.getprojectnumber(cModel.SelProjectNumber);
                    model.commitmentAmount = String.Format(Indian, "{0:N}", cModel.commitmentValue);
                    model.commitmentNo = cModel.CommitmentNo;
                    model.date = String.Format("{0:s}", DateTime.Now);

                    EmailBuilder _eb = new EmailBuilder();
                    EmailModel emodel = model;
                    var bodyResp = _eb.RunCompile("UAYProjectCommitmentBooking.cshtml", "", model, typeof(UAYEmailModel));
                    model.subject = "Notification of commitment booked under UAY projects";
                    model.toMail = WebConfigurationManager.AppSettings["MUAYToMail"];
                    List<string> cc = new List<string>();
                    string ccMail = WebConfigurationManager.AppSettings["defaultCCMail"];
                    cc.Add(ccMail);
                    model.cc = cc;
                    if (bodyResp.Item1)
                        _eb.SendEmail(emodel, bodyResp.Item2);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static CommitmentModel getEditCommitDetails(int CommitmentId)
        {
            try
            {
                CommitmentModel Commit = new CommitmentModel();
                List<AllocationDetailsModel> list = new List<AllocationDetailsModel>();
                using (var context = new IOASDBEntities())
                {

                    var query = (from D in context.tblCommitment
                                 join p in context.tblProject on D.ProjectId equals p.ProjectId
                                 where (D.CommitmentId == CommitmentId)
                                 select new { D, p.ProjectNumber, p.ProjectClassification, p.AllocationNR_f }).FirstOrDefault();
                    var qryCommitDetails = (from C in context.tblCommitmentDetails where C.CommitmentId == CommitmentId select C).FirstOrDefault();

                    if (query != null)
                    {
                        Commit.selCommitmentType = query.D.CommitmentType ?? 0;
                        Commit.selPurpose = query.D.Purpose ?? 0;
                        Commit.Remarks = query.D.Description;
                        Commit.ItemDescription = query.D.ItemDescription;
                        Commit.PONumber = query.D.PurchaseOrder;
                        Commit.VendorName = query.D.VendorNameStr;
                        Commit.selCurrency = query.D.Currency ?? 0;
                        Commit.currencyRate = query.D.CurrencyRate ?? 0;
                        Commit.selProjectType = query.D.ProjectType ?? 0;
                        Commit.SelProjectNumber = query.D.ProjectId ?? 0;
                        Commit.projectNumber = query.ProjectNumber;
                        Commit.AllocationNR_f = query.ProjectClassification != 1 ? true : query.AllocationNR_f ?? false;
                        Commit.selRequestRefrence = query.D.Reference ?? 0;
                        Commit.selRefNo = query.D.ReferenceNo;
                        Commit.RefNo = query.D.ReferenceNo;
                        Commit.commitmentValue = query.D.CommitmentAmount ?? 0;
                        Commit.commitmentId = query.D.CommitmentId;
                        Commit.CommitmentNo = query.D.CommitmentNumber;
                        Commit.EmployeeId = query.D.EmployeeId;
                        Commit.CastEmployeeId = query.D.CastEmployeeId;
                        Commit.StartDate = query.D.StartDate;
                        Commit.CloseDate = query.D.CloseDate;
                        Commit.BasicPay = query.D.BasicPay ?? 0;
                        Commit.MedicalAllowance = query.D.MedicalAllowance ?? 0;
                        Commit.Total = query.D.Total ?? 0;
                        Commit.IsDeansApproval = query.D.IsDeansApproval ?? false;
                        Commit.AdditionalCharge = query.D.AdditionalCharge ?? 0;
                        Commit.AttachName = query.D.AttachmentName;
                        Commit.AttachPath = query.D.AttachmentPath;
                        Commit.DocName = query.D.DocName;
                        Commit.selFundingBody = query.D.FundingBody ?? 0;
                        Commit.BankId = query.D.BankID;
                        if (qryCommitDetails != null)
                        {
                            Commit.selAllocationHead = qryCommitDetails.AllocationHeadId ?? 0;
                            Commit.AllocationValue = qryCommitDetails.Amount ?? 0;
                            Commit.ForeignCurrencyValue = qryCommitDetails.ForignCurrencyValue;
                            Commit.selSubhead = qryCommitDetails.Subhead;
                        }
                        if (Commit.selRequestRefrence == 2)
                        {
                            Commit.EmailDate = query.D.EmailDate ?? DateTime.Now;
                        }


                    }
                    ProjectService _PS = new ProjectService();
                    Commit.prjDetails = _PS.getProjectSummary(Commit.SelProjectNumber);
                    Commit.AllocationDtls = Common.getAllocationValue(Commit.SelProjectNumber, Commit.selAllocationHead);
                    Commit.ProjectDetail = Common.GetProjectsDetails(query.D.ProjectId ?? 0);
                }
                return Commit;
            }
            catch (Exception ex)
            {
                CommitmentModel commit = new CommitmentModel();
                return commit;
            }
        }
        public static int ActiveCommitment(int CommitmentId, int UserId, decimal CommitVal)
        {
            try
            {
                int result = 0;

                using (var context = new IOASDBEntities())
                {
                    var Check = (from D in context.tblCommitment
                                 join p in context.tblProject on D.ProjectId equals p.ProjectId
                                 from C in context.tblCommitmentDetails
                                 where D.CommitmentId == C.CommitmentId
                                 where (D.CommitmentId == CommitmentId)
                                 select new { D.ProjectId, C.AllocationHeadId, p.ProjectClassification, p.AllocationNR_f }).FirstOrDefault();
                    ProjectService _ps = new ProjectService();
                    var pData = _ps.getProjectSummary(Check.ProjectId ?? 0);
                    if (!pData.AllocationNR_f)
                    {
                        var AllocData = Common.getAllocationValue(Check.ProjectId ?? 0, Check.AllocationHeadId ?? 0);
                        if (AllocData.IsAllocation == true)
                        {
                            if (AllocData.IsYearWise == true)
                            {
                                if (AllocData.AllocationForCurrentYear != 0)
                                {
                                    if (CommitVal > AllocData.AllocationForCurrentYear)
                                    {
                                        return -1;
                                    }
                                    else if (CommitVal > AllocData.TotalCommitForCurrentYear)
                                    {
                                        return -2;
                                    }
                                }
                            }
                            else
                            {
                                if (AllocData.TotalAllocation != 0)
                                {
                                    if (CommitVal > AllocData.TotalAllocation)
                                    {
                                        return -3;
                                    }
                                    else if (CommitVal > AllocData.TotalCommitForCurrentYear)
                                    {
                                        return -4;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (CommitVal > AllocData.SanctionedValue)
                            {
                                return -5;
                            }
                            else if (CommitVal > AllocData.TotalCommitForCurrentYear)
                            {
                                return -6;
                            }
                        }
                    }
                    else if (CommitVal > pData.NetBalance)
                    {
                        return -7;
                    }
                    var query = (from D in context.tblCommitment
                                 where (D.CommitmentId == CommitmentId)
                                 select D).FirstOrDefault();

                    if (query != null)
                    {
                        query.Status = "Active";
                        //query.AmountSpent = AmountSpent;
                        query.UPDT_UserID = UserId;
                        query.UPDT_TS = DateTime.Now;
                        context.SaveChanges();
                        result = 1;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static List<CommitmentResultModel> GetActiveCommitmentDetails()
        {
            try
            {
                List<CommitmentResultModel> GetDetail = new List<CommitmentResultModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCommitment
                                 where C.Status == "Active"
                                 orderby C.CommitmentId descending
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {

                            GetDetail.Add(new CommitmentResultModel()
                            {
                                SlNo = i + 1,
                                ComitmentId = query[i].CommitmentId,
                                CommitmentType = Common.getCommitmentName(query[i].CommitmentType ?? 0),
                                CommitmentNo = query[i].CommitmentNumber,
                                projectNumber = Common.GetProjectNumber(query[i].ProjectId ?? 0),
                                VendorName = "NA",
                                CommitmentAmount = query[i].CommitmentAmount ?? 0,
                                AmountSpent = query[i].AmountSpent ?? 0,
                                CreatedDate = String.Format("{0:ddd dd-MMM-yyyy}", query[i].CRTD_TS),
                                Status = query[i].Status,
                                ProjectId = query[i].ProjectId ?? 0
                            });

                        }
                    }
                }
                return GetDetail;
            }
            catch (Exception ex)
            {
                List<CommitmentResultModel> GetDetail = new List<CommitmentResultModel>();
                return GetDetail;
            }

        }

        public static PagedData<CommitmentResultModel> SearchActiveCommitmentDetails(CommitSearchFieldModel model, int page, int pageSize)
        {
            try
            {
                var commitment = new PagedData<CommitmentResultModel>();
                List<CommitmentResultModel> GetDetail = new List<CommitmentResultModel>();
                using (var context = new IOASDBEntities())
                {
                    int skiprec = 0;
                    if (page == 1)
                    {
                        skiprec = 0;
                    }
                    else
                    {
                        skiprec = (page - 1) * pageSize;
                    }
                    var prequery = from C in context.tblCommitment
                                   join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                   join P in context.tblProject on C.ProjectId equals P.ProjectId
                                   select new CommitmentPredicate
                                   {
                                       com = C,
                                       prj = P,
                                       ComDet = D
                                   };
                    var predicate = PredicateBuilder.BaseAnd<CommitmentPredicate>();
                    predicate = predicate.And(d => d.com.Status == "Active");
                    if (!string.IsNullOrEmpty(model.Keyword))
                        predicate = predicate.And(d => d.com.CommitmentNumber.Contains(model.Keyword) || d.com.Status.Contains(model.Keyword));
                    if (model.ProjectType != null)
                        predicate = predicate.And(d => d.prj.ProjectType == model.ProjectType);
                    if (model.ProjectNumber != null)
                        predicate = predicate.And(d => d.com.ProjectId == model.ProjectNumber);
                    if (model.SearchINCommitmentNumber != null)
                        predicate = predicate.And(d => d.com.CommitmentNumber.Contains(model.SearchINCommitmentNumber));
                    if (model.SearchINProjectNumber != null)
                        predicate = predicate.And(d => d.prj.ProjectNumber.Contains(model.SearchINProjectNumber));
                    if (model.FromCreatedDate != null && model.ToCreatedDate != null)
                        predicate = predicate.And(d => d.com.CRTD_TS >= model.FromCreatedDate && d.com.CRTD_TS <= model.ToCreatedDate);
                    //predicate = predicate.And(m => m.com.CommitmentBalance != 0);
                    var query = prequery.Where(predicate).OrderByDescending(m => m.com.CommitmentId).Skip(skiprec).Take(pageSize).ToList();
                    commitment.TotalRecords = prequery.Where(predicate).Count();
                    //var query = (from C in context.tblCommitment
                    // where C.Status == "Active" && C.ProjectType == model.ProjectType && C.ProjectId == model.ProjectNumber
                    // && (String.IsNullOrEmpty(model.Keyword) || C.CommitmentNumber.Contains(model.Keyword) || C.Status.Contains(model.Keyword))
                    // &&((C.CRTD_TS>model.FromCreatedDate)&&(C.CRTD_TS<model.ToCreatedDate))
                    // orderby C.CommitmentId descending
                    // select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {

                            GetDetail.Add(new CommitmentResultModel()
                            {
                                SlNo = i + 1,
                                ComitmentId = query[i].com.CommitmentId,
                                CommitmentType = Common.getCommitmentName(query[i].com.CommitmentType ?? 0),
                                CommitmentNo = query[i].com.CommitmentNumber,
                                projectNumber = Common.GetProjectNumber(query[i].com.ProjectId ?? 0),
                                VendorName = "NA",
                                CommitmentAmount = query[i].ComDet.Amount ?? 0,
                                AmountSpent = query[i].com.AmountSpent ?? 0,
                                CreatedDate = String.Format("{0:ddd dd-MMM-yyyy}", query[i].com.CRTD_TS),
                                Status = query[i].com.Status,
                                ProjectId = query[i].com.ProjectId ?? 0,
                            });

                        }
                    }
                }
                commitment.Data = GetDetail;
                return commitment;
            }
            catch (Exception ex)
            {
                var commitment = new PagedData<CommitmentResultModel>();
                return commitment;
            }

        }
        public CommitmentResultModel getPopViewCommitDetails(int CommitmentId, string Status)
        {
            try
            {
                CommitmentResultModel Commit = new CommitmentResultModel();
                using (var context = new IOASDBEntities())
                {

                    var query = (from D in context.tblCommitment
                                 where (D.CommitmentId == CommitmentId && D.Status == "Active")
                                 select D).FirstOrDefault();
                    var qryCommitDetails = (from C in context.tblCommitmentDetails where C.CommitmentId == CommitmentId select C).ToList();
                    var bankheadname = (from D in context.tblAccountHead join P in context.tblProject on D.AccountHeadId equals P.BankID
                                        where (P.ProjectId == query.ProjectId && P.Status == "Active")
                                        select D).FirstOrDefault();
                    var pre_bank = context.tblAccountHead.Where(m => m.AccountHeadId == query.BankID).Select(m => m.AccountHeadId).FirstOrDefault();

                    if (query != null)
                    {
                        var CommitDetailId = qryCommitDetails[0].ComitmentDetailId;
                        var SpentAmt = context.vwCommitmentSpentBalance.Where(m => m.CommitmentDetailID == CommitDetailId).OrderByDescending(m => m.CommitmentLogId).Select(m => m.AmountSpent).FirstOrDefault();
                        Commit.AvlBalance = qryCommitDetails[0].BalanceAmount ?? 0;
                        Commit.AmountSpent = SpentAmt ?? 0;
                        Commit.ComitmentId = query.CommitmentId;
                        Commit.CommitmentType = Common.getCommitmentName(query.CommitmentType ?? 0);
                        Commit.Purpose = Common.getPurpose(query.Purpose ?? 0);
                        Commit.Remarks = query.Description;
                        Commit.PONumber = query.PurchaseOrder;
                        Commit.VendorName = Common.getVendorName(query.VendorName ?? 0);
                        Commit.Currency = Common.getCurrency(query.Currency ?? 0);
                        Commit.CurrencyRate = query.CurrencyRate ?? 0;
                        Commit.ProjectType = Common.getProjecTypeName(query.ProjectType ?? 0);
                        Commit.projectNumber = Common.GetProjectNumber(query.ProjectId ?? 0);
                        Commit.ProjectId = query.ProjectId ?? 0;
                        Commit.RequestRef = Common.getRefrenceName(query.Reference ?? 0);
                        var TapalNo = "";
                        int TapalId = 0;
                        if (query.ReferenceNo != "")
                            TapalId = Convert.ToInt32(query.ReferenceNo);
                        TapalNo = context.tblTapal.Where(m => m.TapalId == TapalId).Select(m => m.TapalNo).FirstOrDefault();
                        Commit.RefNo = TapalNo;
                        Commit.CommitmentAmount = query.CommitmentAmount ?? 0;
                        Commit.CommitmentNo = query.CommitmentNumber;
                        int? AllocHeadId = qryCommitDetails[0].AllocationHeadId;
                        int PojectId = query.ProjectId ?? 0;
                        Commit.AllHeadId = AllocHeadId ?? 0;
                        CommitAllocationHeadDetails Allomodel = new CommitAllocationHeadDetails();
                        var Allocationmodel = Common.getAllocationValue(PojectId, AllocHeadId ?? 0);
                        if (Allocationmodel != null)
                        {
                            Allomodel.IsAllocation = Allocationmodel.IsAllocation;
                            Allomodel.IsYearWise = Allocationmodel.IsYearWise;
                            Allomodel.AllocationForCurrentYear = Allocationmodel.AllocationForCurrentYear;
                            Allomodel.TotalCommitForCurrentYear = Allocationmodel.TotalCommitForCurrentYear;
                            Allomodel.TotalAllocation = Allocationmodel.TotalAllocation;
                        }
                        Commit.AllocModel = Allomodel;
                        Commit.AllocationHead = context.tblBudgetHead.Where(m => m.BudgetHeadId == AllocHeadId).Select(m => m.HeadName).FirstOrDefault();
                        //Commit.AvlBalance = qryCommitDetails[0].BalanceAmount ?? 0;
                        Commit.Status = query.Status;
                        Commit.EmailDate = String.Format("{0:s}", query.EmailDate);
                        Commit.CreatedDate = String.Format("{0:s}", query.CRTD_TS);
                        Commit.CloseDate = String.Format("{0:s}", query.CloseDate);
                        Commit.StartDate = String.Format("{0:s}", query.StartDate);
                        Commit.BasicPay = query.BasicPay ?? 0;
                        Commit.MedicalAllowance = query.MedicalAllowance ?? 0;
                        Commit.AdditionalCharge = query.AdditionalCharge ?? 0;
                        Commit.Pre_BankID = pre_bank;
                        if (bankheadname != null)
                        {
                            Commit.BankHeadName = bankheadname.AccountHead.ToString();
                            Commit.BankHeadId = bankheadname.AccountHeadId;
                        }
                        else
                        {
                            Commit.BankHeadName = "";
                            Commit.BankHeadId = 0;
                        }
                    }
                    ProjectService _PS = new ProjectService();
                    Commit.prjDetails = _PS.getProjectSummary(query.ProjectId ?? 0);
                }
                return Commit;
            }
            catch (Exception ex)
            {
                CommitmentResultModel commit = new CommitmentResultModel();
                return commit;
            }
        }
        public static int CloseThisCommitment(CommitmentResultModel model, int UserId)        {            try            {               lock (ClsCommitlockObj)
               {                 bool isClosed = true;                int result = 0;                List<BillCommitmentDetailModel> commitbalance = new List<BillCommitmentDetailModel>();                using (var context = new IOASDBEntities())                {                    var CommQry = (from C in context.tblCommitment                                   join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId                                   where C.Status == "Active" && C.CommitmentId == model.ComitmentId                                   select new                                   { C, D }).FirstOrDefault();                        var prveBankID = CommQry.C.BankID;
                        var bankheadname = (from D in context.tblAccountHead
                                            join P in context.tblProject on D.AccountHeadId equals P.BankID
                                            where (P.ProjectId == model.ProjectId && P.Status == "Active")
                                            select D).FirstOrDefault();

                        if (commitbalance != null)
                        {
                            decimal? AvailBal = CommQry.D.BalanceAmount;
                            if (model.LogTypeId == 1)
                            {
                                var validation = Common.ValidateCommitment(model.ProjectId, model.AllHeadId, model.AddCloseAmt);
                                if (validation != "Valid")
                                    return -2;
                                else if (Common.IsInUcCommitment(model.ComitmentId))
                                    return -2;
                            }

                            if (model.LogTypeId == 3)
                            {
                                bool valres = Common.ValidateCloseCommitment(model.ComitmentId, model.AddCloseAmt);
                                if (!valres)
                                    return -2;
                                if (model.AddCloseAmt > AvailBal || model.AddCloseAmt < AvailBal)
                                    return -1;
                                commitbalance = (from C in context.tblCommitment
                                                 join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                                 where C.Status == "Active" && C.CommitmentId == model.ComitmentId
                                                 select new BillCommitmentDetailModel()
                                                 {
                                                     CommitmentDetailId = D.ComitmentDetailId,
                                                     AvailableAmount = D.BalanceAmount,
                                                     CommitmentId = C.CommitmentId,
                                                 }).ToList();
                                CoreAccountsService _CAS = new CoreAccountsService();
                                bool commitdetls = _CAS.UpdateCommitmentBalance(commitbalance, false, false, UserId, 0, "", isClosed);

                                if (commitdetls == true)
                                {
                                    var query = (from D in context.tblCommitment
                                                 where (D.CommitmentId == model.ComitmentId)
                                                 select D).FirstOrDefault();

                                    if (query != null)                                    {
                                        query.Status = "Closed";
                                        query.UPDT_UserID = UserId;
                                        query.UPDT_TS = DateTime.Now;
                                        if (bankheadname != null)
                                            query.BankID = bankheadname.AccountHeadId; //model.BankHeadId;
                                        else
                                            query.BankID = 0;
                                        context.SaveChanges();
                                        tblCommitmentClosedLog clLog = new tblCommitmentClosedLog();
                                        clLog.CommitmentID = model.ComitmentId;
                                        clLog.Reason = model.Reason;
                                        clLog.Remarks = model.Remarks;
                                        clLog.CRTD_By = UserId;
                                        clLog.CRTD_TS = DateTime.Now;
                                        context.tblCommitmentClosedLog.Add(clLog);
                                        context.SaveChanges();

                                        tblCommitmentAddandCloseLog clAddClose = new tblCommitmentAddandCloseLog();
                                        clAddClose.LogType = model.LogTypeId;
                                        clAddClose.CommitmentId = CommQry.C.CommitmentId;
                                        clAddClose.CommitmentDetailId = CommQry.D.ComitmentDetailId;
                                        clAddClose.PrevCommitmentAmount = CommQry.C.CommitmentAmount;
                                        clAddClose.PrevCommtBalanceAmount = CommQry.C.CommitmentBalance;
                                        clAddClose.PrevCommtDetBalanceAmount = CommQry.D.BalanceAmount;
                                        clAddClose.Remarks = model.strRemarks;
                                        clAddClose.Amount = model.AddCloseAmt;
                                        clAddClose.ProjectId = model.ProjectId;
                                        clAddClose.Reason = model.Reason;
                                        clAddClose.HeadId = model.AllHeadId;
                                        clAddClose.CRTD_BY = UserId;
                                        clAddClose.CRTD_TS = DateTime.Now;

                                        if (bankheadname != null)
                                        {
                                            clAddClose.BankID = bankheadname.AccountHeadId;
                                            clAddClose.Pre_BankID = prveBankID;//model.BankHeadId;
                                        }
                                        else
                                        {
                                            clAddClose.Pre_BankID = 0;
                                            clAddClose.BankID = 0;
                                        }

                                        context.tblCommitmentAddandCloseLog.Add(clAddClose);
                                        context.SaveChanges();
                                        result = 1;

                                    }
                                }                            }
                            else
                            {
                                //var query = (from D in context.tblCommitment
                                //             where (D.CommitmentId == model.ComitmentId)
                                //             select D).FirstOrDefault();

                                //if(query !=null)
                                //{
                                //    tblCommitment updatecommit = new tblCommitment();
                                //    updatecommit.BankID = model.BankHeadId;
                                //    context.tblCommitment.Add(updatecommit);
                                //    context.SaveChanges();
                                //}
                                tblCommitmentAddandCloseLog clAddClose = new tblCommitmentAddandCloseLog();
                                clAddClose.LogType = model.LogTypeId;
                                clAddClose.CommitmentId = CommQry.C.CommitmentId;
                                clAddClose.CommitmentDetailId = CommQry.D.ComitmentDetailId;
                                clAddClose.PrevCommitmentAmount = CommQry.C.CommitmentAmount;
                                clAddClose.PrevCommtBalanceAmount = CommQry.C.CommitmentBalance;
                                clAddClose.PrevCommtDetBalanceAmount = CommQry.D.BalanceAmount;
                                clAddClose.Remarks = model.strRemarks;
                                clAddClose.Amount = model.AddCloseAmt;
                                clAddClose.ProjectId = model.ProjectId;
                                clAddClose.Reason = model.Reason;
                                clAddClose.HeadId = model.AllHeadId;
                                clAddClose.CRTD_BY = UserId;
                                clAddClose.CRTD_TS = DateTime.Now;
                                //clAddClose.BankID = model.BankHeadId;
                                //clAddClose.Pre_BankID = model.Pre_BankID;
                                if (bankheadname != null)
                                {
                                    clAddClose.BankID = bankheadname.AccountHeadId;
                                    clAddClose.Pre_BankID = prveBankID;
                                }
                                else
                                {
                                    clAddClose.Pre_BankID = 0;
                                    clAddClose.BankID = 0;
                                }
                                context.tblCommitmentAddandCloseLog.Add(clAddClose);
                                context.SaveChanges();
                                result = 1;
                            }
                            var Comqry = context.tblCommitment.Where(m => m.CommitmentId == model.ComitmentId).FirstOrDefault();
                            var ComDetqry = context.tblCommitmentDetails.Where(m => m.CommitmentId == model.ComitmentId).FirstOrDefault();
                            if (Comqry != null && ComDetqry != null)
                            {
                                if (model.LogTypeId == 2)
                                {
                                    if (model.AddCloseAmt > AvailBal)
                                        return -1;
                                    //Comqry.CommitmentAmount = (Comqry.CommitmentAmount - model.AddCloseAmt);
                                    Comqry.CommitmentBalance = (Comqry.CommitmentBalance - model.AddCloseAmt);
                                    context.SaveChanges();
                                    //ComDetqry.Amount = (ComDetqry.Amount - model.AddCloseAmt);
                                    ComDetqry.BalanceAmount = (ComDetqry.BalanceAmount - model.AddCloseAmt);
                                    context.SaveChanges();
                                }
                                else if (model.LogTypeId == 1)
                                {
                                    Comqry.CommitmentAmount = (Comqry.CommitmentAmount + model.AddCloseAmt);
                                    Comqry.CommitmentBalance = (Comqry.CommitmentBalance + model.AddCloseAmt);
                                    context.SaveChanges();
                                    ComDetqry.Amount = (ComDetqry.Amount + model.AddCloseAmt);
                                    ComDetqry.BalanceAmount = (ComDetqry.BalanceAmount + model.AddCloseAmt);
                                    context.SaveChanges();
                                }
                            }
                        }

                    //return 

                }                return result;
            }            }            catch (Exception ex)            {                return 0;            }        }

      
        public static List<DepartmentModel> GetDepartmentlist()
        {
            try
            {
                List<DepartmentModel> Deptlist = new List<DepartmentModel>();

                using (var context = new IOASDBEntities())
                {
                    //decimal totalpage = (from D in context.tblDepartment
                    //             orderby D.DepartmentName
                    //             select new { D.DepartmentId, D.DepartmentName, D.HOD }).Count();

                    var query = (from D in context.tblDepartment
                                 orderby D.DepartmentName
                                 where (D.Status == "Active")
                                 select new { D.DepartmentId, D.DepartmentName, D.HOD }).ToList();
                    //decimal totalpagcount = (totalpage / pageSize);
                    //var tatcountpage = Math.Ceiling(totalpagcount);
                    if (query.Count > 0)
                    {

                        for (int i = 0; i < query.Count; i++)
                        {
                            var sno = i + 1;
                            Deptlist.Add(new DepartmentModel()
                            {
                                Sno = sno,
                                Departmentid = query[i].DepartmentId,
                                Departmentname = query[i].DepartmentName,
                                HOD = query[i].HOD,
                                //totalPages= (Int32)tatcountpage
                            });
                        }
                    }
                }
                return Deptlist;
            }
            catch (Exception ex)
            {
                List<DepartmentModel> Deptlist = new List<DepartmentModel>();
                return Deptlist;
            }
        }
        public static List<MasterlistviewModel> getConsultancycategory(int typeid)
        {
            try
            {
                List<MasterlistviewModel> category = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {


                    if (typeid > 0)
                    {

                        var query = (from C in context.tblSchemes
                                     join cc in context.tblCodeControl on C.ConsultancyType equals cc.CodeValAbbr
                                     where (C.ProjectType == typeid && cc.CodeName == "ConsultancyType")
                                     select new { C.SchemeId, C.SchemeName, cc.CodeValDetail }).ToList();
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                category.Add(new MasterlistviewModel()
                                {
                                    id = query[i].SchemeId,
                                    name = query[i].CodeValDetail + "-" + query[i].SchemeName,

                                });
                            }
                        }
                    }


                    return category;

                }


            }
            catch (Exception ex)
            {
                return null;
            }

        }

        #region UserManual
        public static List<MasterlistviewModel> getModulesList()
        {
            try
            {
                List<MasterlistviewModel> category = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {

                    var query = (from C in context.tblModules
                                 from D in context.tblFunction
                                 where C.ModuleID == D.ModuleID
                                 select C).Distinct().ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            category.Add(new MasterlistviewModel()
                            {
                                id = query[i].ModuleID,
                                name = query[i].ModuleName,

                            });
                        }
                    }

                    return category;

                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<MasterlistviewModel> GetMenuGroupBasedonModule(int? ModuleId = 0)
        {
            try
            {
                List<MasterlistviewModel> category = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {



                    var query = (from C in context.tblMenuGroup
                                 from D in context.tblFunction
                                 where C.ModuleID == ModuleId && D.MenuGroupID == C.MenuGroupID
                                 select C).Distinct().ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            category.Add(new MasterlistviewModel()
                            {
                                id = query[i].MenuGroupID,
                                name = query[i].MenuGroup,

                            });
                        }
                    }

                    return category;

                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<MasterlistviewModel> GetFunctionBasedonMenugroup(int? MenugrpId = 0)
        {
            try
            {
                List<MasterlistviewModel> category = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {

                    var query = (from D in context.tblFunction
                                 where D.MenuGroupID == MenugrpId
                                 select D).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            category.Add(new MasterlistviewModel()
                            {
                                id = query[i].FunctionId,
                                name = query[i].FunctionName,

                            });
                        }
                    }

                    return category;

                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static int SaveUserManual(UserManualModel model)
        {
            try
            {
                int result = 0;
                using (var context = new IOASDBEntities())
                {
                    var Rec = new tblFunction();
                    var check = (from m in context.tblFunction
                                 where m.FunctionId == model.FunctionId
                                 select m).FirstOrDefault();
                    if (check != null)
                    {

                        string _FileName = Path.GetFileName(model.Attachment.FileName);
                        check.FileName = _FileName;
                        _FileName = Guid.NewGuid() + "_" + _FileName;
                        model.Attachment.UploadFile("UserManual", _FileName);
                        check.ActualFileName = _FileName;
                        check.Status = "Open";
                        check.Updt_TS = DateTime.Now;
                        check.Updt_UserId = Common.GetUserid(model.UserName);
                        // context.tblFunction.Add(Rec);
                        context.SaveChanges();
                        result = 1;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static PagedData<UserManualModel> getUserManualList(int page, int pageSize, UserManualModel model)
        {
            List<UserManualModel> list = new List<UserManualModel>();
            PagedData<UserManualModel> SearchData = new PagedData<UserManualModel>();
            int skiprec = 0;
            if (page == 1)
            {
                skiprec = 0;
            }
            else
            {
                skiprec = (page - 1) * pageSize;
            }
            try
            {
                using (var Context = new IOASDBEntities())
                {
                    var getdata = (from r in Context.tblModules
                                   join m in Context.tblMenuGroup on r.ModuleID equals m.ModuleID
                                   join n in Context.tblFunction on m.MenuGroupID equals n.MenuGroupID
                                   where
                                    ((n.FunctionName.Contains(model.Function) || model.Function == null)
                                        && (r.ModuleName.Contains(model.Module) || model.Module == null)
                                         && (m.MenuGroup.Contains(model.Menugroup) || model.Menugroup == null)
                                          && (n.Status == (model.Status) || model.Status == "Select" || model.Status == null)
                                          && ((n.Status == "Active") || (n.Status == "Open")))
                                   orderby n.FunctionName
                                   select new
                                   {
                                       n.FileName,
                                       n.FunctionName,
                                       n.ActualFileName,
                                       n.Status,
                                       n.FunctionId,
                                       r.ModuleName,
                                       m.MenuGroup
                                   }).Skip(skiprec).Take(pageSize).ToList();

                    if (getdata.Count > 0)
                    {
                        for (var i = 0; i < getdata.Count; i++)
                        {
                            list.Add(new UserManualModel()
                            {
                                SINo = i + 1 + skiprec,
                                Module = getdata[i].ModuleName,
                                Menugroup = getdata[i].MenuGroup,
                                Function = getdata[i].FunctionName,
                                FileName = getdata[i].FileName,
                                ActualFileName = getdata[i].ActualFileName,
                                Status = getdata[i].Status,
                                FunctionId = getdata[i].FunctionId,
                            });
                        }
                    }
                    var totcount = (from r in Context.tblModules
                                    join m in Context.tblMenuGroup on r.ModuleID equals m.ModuleID
                                    join n in Context.tblFunction on m.MenuGroupID equals n.MenuGroupID
                                    where
                                     //(r.FileName.Contains(model.FileName) || model.FileName == null) &&
                                     ((n.FunctionName.Contains(model.Function) || model.Function == null)
                                         && (r.ModuleName.Contains(model.Module) || model.Module == null)
                                          && (m.MenuGroup.Contains(model.Menugroup) || model.Menugroup == null)
                                           && (n.Status == (model.Status) || model.Status == "Select" || model.Status == null)
                                           && ((n.Status == "Active") || (n.Status == "Open")))
                                    orderby n.FunctionName
                                    select new
                                    {
                                        n.FunctionName,
                                        r.ModuleName,
                                        m.MenuGroup
                                    }).Count();
                    SearchData.TotalPages = totcount;
                    SearchData.Data = list;
                    return SearchData;
                }
            }
            catch (Exception ex)
            {
                SearchData.Data = list;
                return SearchData;
            }
        }
        public static int SaveState(int FunctionId, int Status)
        {
            int result = 0;
            try
            {

                using (var context = new IOASDBEntities())
                {
                    var query = (from t in context.tblFunction
                                 where t.FunctionId == FunctionId
                                 select t).FirstOrDefault();
                    if (query != null)
                    {
                        if (Status == 1)
                            query.Status = "Active";
                        else if (Status == 2)
                            query.Status = "InActive";
                        context.SaveChanges();
                        result = 1;
                    }
                }
            }
            catch (Exception er)
            {

                throw;
            }
            return result;
        }
        public static UserManualModel EditUserManual(int FunctionId)
        {
            var model = new UserManualModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from c in context.tblFunction
                                 join d in context.tblModules on c.ModuleID equals d.ModuleID
                                 join e in context.tblMenuGroup on c.MenuGroupID equals e.MenuGroupID
                                 where c.FunctionId == FunctionId
                                 select new { c.FunctionId, d.ModuleID, e.MenuGroupID, c.FileName, c.ActualFileName }).FirstOrDefault();
                    if (query != null)
                    {
                        model.FunctionId = query.FunctionId;
                        model.MenugroupId = query.MenuGroupID;
                        model.ModuleId = query.ModuleID;
                        model.FileName = query.FileName;
                        model.ActualFileName = query.ActualFileName;

                    }
                }
                return model;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static List<MenuListViewModel> GetUserMenu()
        {
            List<MenuListViewModel> addmenu = new List<MenuListViewModel>();

            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from F in context.tblFunction
                                 from M in context.tblModules
                                 from MG in context.tblMenuGroup
                                 where (F.ModuleID == M.ModuleID && F.MenuGroupID == MG.MenuGroupID && F.ActualFileName != null)
                                 select new { F.FunctionId, F.FunctionName, F.ActualFileName, M.ModuleID, M.ModuleIcon, M.ModuleName, MG.MenuGroup, MG.MenuGroupID, F.MenuSeq }).Distinct().ToList();

                    if (query.Count > 0)
                    {
                        //This query using get modules in query count using distinct
                        var module = query.Select(m => m.ModuleName).Distinct().ToArray();
                        var modulId = query.Select(m => m.ModuleID).Distinct().ToArray();
                        var menugroup = query.Select(m => m.MenuGroup).Distinct().ToArray();
                        var menugroupId = query.Select(m => m.MenuGroupID).Distinct().ToArray();

                        for (int m = 0; m < module.Length; m++)
                        {
                            string moduleName = module[m];
                            int moduleId = (from C in query where C.ModuleName == moduleName select C.ModuleID).FirstOrDefault();
                            var moduleIconName = (from C in query where C.ModuleName == moduleName select C.ModuleIcon).FirstOrDefault(); //query.Select(S => S.ModuleName == moduleName).FirstOrDefault();
                            List<submodulemenuviewmodel> submodules = new List<submodulemenuviewmodel>();
                            for (int i = 0; i < menugroup.Length; i++)
                            {
                                string submodule = menugroup[i];
                                // int submoduleId=men
                                var menu = query.Where(S => S.ModuleName == moduleName && S.MenuGroup == submodule).FirstOrDefault();
                                var submoduleId = (from S in query where S.ModuleName == moduleName && S.MenuGroup == submodule select S.MenuGroupID).FirstOrDefault();
                                if (menu != null)
                                {
                                    submodules.Add(new submodulemenuviewmodel()
                                    {
                                        Menugroupname = submodule,
                                        Menugroupid = submoduleId,
                                        Submenu = (from sm in query
                                                   where sm.ModuleName == moduleName && sm.MenuGroup == submodule
                                                   orderby sm.MenuSeq
                                                   select new SubmenuViewModel()
                                                   {
                                                       FunctionId = sm.FunctionId,
                                                       Functioname = sm.FunctionName,
                                                       FileName = sm.ActualFileName,
                                                   }).Distinct().ToList()
                                    });
                                }
                            }
                            addmenu.Add(new MenuListViewModel()
                            {
                                Modulename = moduleName,
                                Moduleid = moduleId,
                                submodule = submodules,
                                ModuleIconName = moduleIconName,
                            });

                        }
                    }

                    return addmenu;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
    }
}