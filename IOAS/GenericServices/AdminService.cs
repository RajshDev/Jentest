using System;
using System.Collections.Generic;
using System.Linq;
using IOAS.Models;
using IOAS.DataModel;

namespace IOAS.GenericServices
{
    public class AdminService
    {

        public static List<RolesModel> GetRoles()
        {
            try
            {
                List<RolesModel> roles = new List<RolesModel>();
                using (var context = new IOASDBEntities())
                {
                    roles = (from R in context.tblRole
                               orderby R.RoleName
                               select new RolesModel { RoleID = R.RoleId, RoleName = R.RoleName }).ToList();

                }
                return roles;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<ReportModulesModel> GetModules()
        {
            try
            {
                List<ReportModulesModel> modules = new List<ReportModulesModel>();
                using (var context = new IOASDBEntities())
                {
                    modules = (from M in context.tblModules
                               orderby M.ModuleName
                               select new ReportModulesModel { ModuleID = M.ModuleID, ModuleName = M.ModuleName }).ToList();

                }
                return modules;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int getUserByName(string UserName)
        {
            using (var context = new IOASDBEntities())
            {
                var userId = 0;
                var userquery = context.tblUser.SingleOrDefault(dup => dup.UserName == UserName && dup.Status == "Active");

                if (userquery != null)
                {
                    userId = userquery.UserId;
                }
                return userId;
            }
        }
    }


}