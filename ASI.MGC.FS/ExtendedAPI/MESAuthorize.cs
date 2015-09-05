using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;

namespace ASI.MGC.FS.ExtendedAPI
{
    public class MESAuthorize : AuthorizeAttribute
    {
        private readonly string[] allowedroles;
        public MESAuthorize(params string[] roles)
        {
            this.allowedroles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            foreach (var role in allowedroles)
            {
                //var user = context.AppUser.Where(m => m.UserID == GetUser.CurrentUser/* getting user form current context */ && m.Role == role &&  
                //m.IsActive == true); // checking active users with allowed roles.  
                //if (user.Count() > 0)  
                //{  
                //   authorize = true; /* return true if Entity has current user(active) with specific role */  
                //}  
            }
            return authorize;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}