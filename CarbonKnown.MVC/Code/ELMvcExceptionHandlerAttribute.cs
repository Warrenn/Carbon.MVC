using System;
using System.Web.Mvc;
using CarbonKnown.MVC.Constants;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Newtonsoft.Json;

namespace CarbonKnown.MVC.Code
{
    public class ELMvcExceptionHandlerAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception;
            var actionData = JsonConvert.SerializeObject(filterContext.RouteData.Values);
            exception.Data.Add("RouteData", actionData);
            Exception responseException;
            if (ExceptionPolicy
                .HandleException(
                    exception,
                    Policy.ELExceptionHandlerAttribute,
                    out responseException))
            {
                filterContext.Exception = responseException;
            }
            base.OnException(filterContext);
        }
    }
}