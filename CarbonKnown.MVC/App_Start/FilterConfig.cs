using System.Web.Http.Filters;
using System.Web.Mvc;
using CarbonKnown.MVC.Code;

namespace CarbonKnown.MVC.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ELMvcExceptionHandlerAttribute());
            filters.Add(new GenerateXSRFTokenAttribute());
        }

        public static void RegisterWebApiFilters(HttpFilterCollection filters)
        {
            filters.Add(new ELWebApiExceptionHandlerAttribute());
        }
    }
}