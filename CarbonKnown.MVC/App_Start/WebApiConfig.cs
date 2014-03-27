using System.Web.Http;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.BindParameter(typeof(DashboardRequest), new DashboardRequestModelBinderAttribute());
            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();
        }
    }
}