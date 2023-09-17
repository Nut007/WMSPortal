using Autofac;
using Autofac.Integration.Mvc;
using WMSPortal.Data;
using WMSPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using WMSPortal.Models;

namespace WMSPortal
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            // Register dependencies in controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // Register dependencies in filter attributes
            builder.RegisterFilterProvider();

            // Register dependencies in custom views
            builder.RegisterSource(new ViewRegistrationSource());

            // Register our Data dependencies
            builder.RegisterModule(new DataModule("WMSPortalSecure"));

            builder.RegisterType<OrdersViewModelListBuilder>().As<IOrdersViewModelListBuilder>();
            builder.RegisterType<LoadingViewModelListBuilder>().As<ILoadingViewModelListBuilder>();
            var container = builder.Build();
           
            // Set MVC DI resolver to use our Autofac container
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


        }
    }   
}