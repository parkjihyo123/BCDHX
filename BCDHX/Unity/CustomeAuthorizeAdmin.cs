using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BCDHX.Moduns.Unity
{
    public class CustomeAuthorizeAdmin : AuthorizeAttribute
    {
       
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
           
            var authorized = base.AuthorizeCore(httpContext);
            
            if (!authorized)
            {
                // The user is not authenticated
                return false;
            }
            else
            {
                var user = httpContext.User;
                if (user.Identity.IsAuthenticated)
                {
                    // Administrator => let him in
                    
                    if (user.IsInRole("Staff") || user.IsInRole("Admin")||user.IsInRole("Manager"))
                    {
                        return true;

                    }               
                    else
                    {
                        return false;
                    }
                    
                }
                else
                {
                    return false;
                }
            }
           
        }
       
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var context = System.Web.HttpContext.Current;
            var PreLink = context.Request.UrlReferrer;
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                         new RouteValueDictionary(new { controller = "Account", action = "Login" })
                            );
            }else
            {
                var urlHelper = new UrlHelper(context.Request.RequestContext);
                var destinationUrl = urlHelper.Action("NotAuthorized", "NotAuthorize", new { notAuthorized= "Không có quyền!" });
                context.Server.TransferRequest(destinationUrl,true,"POST",null);
              
            }
        }

    }
}
