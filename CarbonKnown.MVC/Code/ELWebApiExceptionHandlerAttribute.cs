using System;
using System.Web.Http.Filters;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Newtonsoft.Json;

namespace CarbonKnown.MVC.Code
{
    public class ELWebApiExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            var exceptionData = new
                {
                    actionExecutedContext.ActionContext.ModelState, 
                    actionExecutedContext.ActionContext.ActionDescriptor.ActionName, 
                    actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName,
                    actionExecutedContext.Request.Headers,
                    actionExecutedContext.Request.Method,
                    actionExecutedContext.Request.RequestUri,
                    actionExecutedContext.ActionContext.ActionArguments
                };
            var actionData = JsonConvert.SerializeObject(exceptionData);
            exception.Data.Add("ActionData", actionData);
            Exception responseException;
            if (!ExceptionPolicy
                     .HandleException(
                         exception,
                         Constants.Policy.ELExceptionHandlerAttribute,
                         out responseException)) return;
            actionExecutedContext.Exception = responseException;
            base.OnException(actionExecutedContext);
        }
    }
}