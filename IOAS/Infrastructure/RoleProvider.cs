using IOAS.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Infrastructure
{
    public class RoleProvider
    {
        public static string[] Get(string controller, string action)
         {
            // get your roles based on the controller and the action name
            using (var context = new IOASDBEntities())
            {

                var query = (from RA in context.tblRoleaccess
                             from R in context.tblRole.Where(RO=>RO.RoleId==RA.RoleId)
                             from F in context.tblFunction
                             where (F.ActionName == action && F.ControllerName == controller && F.FunctionId == RA.FunctionId)
                             select R.RoleName).ToArray();
                //return new string[] { "Office Admin", "Office DA", "Facility Admin", "Facility DA" };
                return query;
            }
               
        }
    }
}