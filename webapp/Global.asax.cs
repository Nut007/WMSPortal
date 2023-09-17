#region Using

using FluentValidation.Mvc;
using FluentValidationApplication.Validation;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

#endregion

namespace WMSPortal
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            AutofacConfig.ConfigureContainer();
            AutoMapperConfig.SetupMappings();
            AreaRegistration.RegisterAllAreas();
            IdentityConfig.RegisterIdentities();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            ValidationConfiguration();  
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);           
        }
        private void ValidationConfiguration()
        {
            FluentValidationModelValidatorProvider.Configure(provider =>
            {
                provider.ValidatorFactory = new ValidatorFactory();
            });
        }  
    }
}