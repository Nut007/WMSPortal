using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace WMSPortal.Controllers
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            HttpContext ctx = HttpContext.Current;
            //if (HttpContext.Current.Session["userRoles"] == null)
            //{
            //    filterContext.Result = new RedirectResult("~/Account/Login");
            //    return;
            //}
            //base.OnActionExecuting(filterContext);
            // check  sessions here
            if (HttpContext.Current.Session["userRoles"] == null)
            {
                if (new HttpRequestWrapper(ctx.Request).IsAjaxRequest())
                {
                    ctx.Items["SessionExpired"] = true;
                    FormsAuthentication.SignOut();
                }
                else
                {
                    //string redirectUrl = string.Format("/Account/Login?returnUrl={0}", ctx.Request.Url.AbsolutePath);
                    filterContext.Result = new RedirectResult(string.Format("/Account/Login?returnUrl={0}", ctx.Request.Url.AbsolutePath));
                    FormsAuthentication.SignOut();
                    return;
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}