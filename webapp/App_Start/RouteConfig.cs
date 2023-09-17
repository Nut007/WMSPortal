#region Using

using System.Web.Mvc;
using System.Web.Routing;

#endregion

namespace WMSPortal
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.LowercaseUrls = true;

            routes.MapRoute("Default", "{controller}/{action}/{id}", new
            {
                controller = "Account",
                action = "Login",
                id = UrlParameter.Optional
            }).RouteHandler = new DashRouteHandler();

            //routes.MapRoute("Default", "{controller}/{action}/{id}", new
            //{
            //    controller = "Orders",
            //    action = "GmapTesting",
            //    id = UrlParameter.Optional
            //}).RouteHandler = new DashRouteHandler();
          //  routes.MapRoute(
          //    name: "Default",
          //    url: "{controller}/{action}/{id}",
          //    defaults: new { controller = "Customer", action = "CustomerList", id = UrlParameter.Optional }
          //);
        }
    }
}