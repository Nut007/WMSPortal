using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMSPortal.Core.Model;

namespace WMSPortal.Extensions
{
    public static class MenuSecurity
    {
        public static User currentUser;
        
        public static bool IsUserInRole(int applicationId)
        {
            HttpContext context = HttpContext.Current;
            currentUser = (User)context.Session["userRoles"];
            if (currentUser.UserName == "admin")
            {
                if (applicationId == 4 || applicationId == 5 || applicationId == 6) return true;
            }
            IEnumerable<ApplicationRole> roles = currentUser.ApplicationRoles.Where(x => x.ApplicationId == applicationId && x.RoleId == currentUser.RoleId && x.IsAllowAccess==true);
            if (roles.Count() == 0)
                return false;
            else
                return true;
        }
    }
}