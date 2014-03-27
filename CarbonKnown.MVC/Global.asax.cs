using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CarbonKnown.MVC.App_Start;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Models;
using Bootstrapper = CarbonKnown.MVC.App_Start.Bootstrapper;

namespace CarbonKnown.MVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            FilterConfig.RegisterWebApiFilters(GlobalConfiguration.Configuration.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DbConfig.ConfigureDb();
            Bootstrapper.Initialize(GlobalConfiguration.Configuration);
            ModelBinders.Binders.Add(typeof(DataTableParamModel), new DataTableParamModelBinderAttribute());
        }
    }
}