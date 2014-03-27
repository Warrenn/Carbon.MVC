using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CarbonKnown.MVC.Tests
{
    public static class MvcMockHelpers
    {
        public static GenericPrincipal TestUser()
        {
            return new GenericPrincipal(new GenericIdentity("testuser"), new[] {"Administrator"});
        }

        public static ControllerContext SetFakeControllerContext(
            this Controller controller,
            HttpContextBase httpContext = null,
            RouteData routeData = null,
            Dictionary<string, string> routeValues = null)
        {
            httpContext = httpContext ?? (new FakeHttpContext()).Object;
            routeData = routeData ?? new RouteData();
            routeValues = routeValues ?? new Dictionary<string, string>();
            foreach (var routeValue in routeValues)
            {
                routeData.Values.Add(routeValue.Key, routeValue.Value);
            }
            var context = new ControllerContext(new RequestContext(httpContext, routeData), controller);
            controller.ControllerContext = context;
            return context;
        }

        public static void SetFakeControllerContextWithLogin(
            this Controller controller, string userName,
            string password,
            string returnUrl)
        {

            var httpContext = (new FakeHttpContext()).Object;


            httpContext.Request.Form.Add("username", userName);
            httpContext.Request.Form.Add("password", password);
            httpContext.Request.QueryString.Add("ReturnUrl", returnUrl);

            var context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
            controller.ControllerContext = context;
        }
    }
}
