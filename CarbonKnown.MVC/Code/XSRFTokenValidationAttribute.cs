using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using FilterAttribute = System.Web.Http.Filters.FilterAttribute;
using IAuthorizationFilter = System.Web.Http.Filters.IAuthorizationFilter;

namespace CarbonKnown.MVC.Code
{
    public class XSRFTokenValidationAttribute : FilterAttribute, IAuthorizationFilter
    {
        public const string XSRFTokenKey = "X-XSRF-TOKEN";

        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(
            HttpActionContext actionContext,
            CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            try
            {
                var cookieToken = string.Empty;
                var formToken = string.Empty;
                IEnumerable<string> tokenHeaders;
                if (actionContext.Request.Headers.TryGetValues(XSRFTokenKey, out tokenHeaders))
                {
                    var tokens = tokenHeaders.First().Split(':');
                    if (tokens.Length == 2)
                    {
                        cookieToken = tokens[0].Trim();
                        formToken = tokens[1].Trim();
                    }
                }
                AntiForgery.Validate(cookieToken, formToken);
            }
            catch (HttpAntiForgeryException ex)
            {
                ExceptionPolicy.HandleException(ex, Constants.Policy.XSRFTokenValidation);
                actionContext.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    RequestMessage = actionContext.ControllerContext.Request
                };
                var source = new TaskCompletionSource<HttpResponseMessage>();
                source.SetResult(actionContext.Response);
                return source.Task;
            }
            return continuation();
        }
    }
}