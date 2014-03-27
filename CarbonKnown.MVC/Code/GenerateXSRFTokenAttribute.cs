using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using CarbonKnown.MVC.Properties;
using ActionFilterAttribute = System.Web.Mvc.ActionFilterAttribute;

namespace CarbonKnown.MVC.Code
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class GenerateXSRFTokenAttribute : ActionFilterAttribute
    {
        public const string XSRFTokenKey = "XSRF-TOKEN";

        public static string CreateToken()
        {
            string cookieToken;
            string formToken;
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            return cookieToken + ":" + formToken;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var context = filterContext.HttpContext;
            var request = context.Request;
            var cookie = request.Cookies[XSRFTokenKey];
            if (cookie != null) return;
            var response = context.Response;
            var headerToken = CreateToken();
            cookie = new HttpCookie(XSRFTokenKey, headerToken)
                {
                    Expires = DateTime.Now.Add(Settings.Default.XSRFTimeout)
                };
            response.Cookies.Add(cookie);
        }
    }
}