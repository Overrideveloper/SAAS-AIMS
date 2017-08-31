using RollbarDotNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SAAS_AIMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Rollbar.Init(new RollbarConfig
            {
                AccessToken = ConfigurationManager.AppSettings["Rollbar.AccessToken"],
                Environment = ConfigurationManager.AppSettings["Rollbar.Environment"],
                EndPoint = "https://api.rollbar.com/api/1/",
                Enabled = true
            });
        }
    }
}
