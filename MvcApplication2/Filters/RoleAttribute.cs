using System;

using System.Web.Mvc;
using WebMatrix.WebData;
using MvcApplication2.Models;
using System.Web;
using System.Web.Security;
using System.Linq;

namespace MvcApplication2.Filters
{
    public class RoleAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.Request.IsAuthenticated)
                return false;

            UsersContext uc=new UsersContext();            
            //UsersInRoles ur=new UsersInRoles();
            //var userId = Membership.GetUser().ProviderUserKey;
            var userId = uc.UserProfiles.Where(a => a.UserName == HttpContext.Current.User.Identity.Name).First().UserId;
            long[] roleIds = (from u in uc.UsersInRoles where u.UserId == userId select u.RoleId).ToArray();
          
            foreach (string role in this.Roles.Split(','))
            {
                foreach (int roleId in roleIds)
                {
                    String name = (from u in uc.Roles where u.RoleId == roleId select u.RoleName).First();
                    if (name.Equals(role))
                        return true;
                }
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
 	         base.HandleUnauthorizedRequest(filterContext);
        }
        
    }

    
}
