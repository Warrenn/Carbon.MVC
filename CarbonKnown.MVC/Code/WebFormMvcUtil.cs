using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CarbonKnown.MVC.Code
{
    public static class WebFormMvcUtil
    {
        public static void RenderPartial(string partialName)
        {
            RenderPartial(partialName, null, null);
        }

        public static void RenderPartial(string partialName, object model)
        {
            RenderPartial(partialName, model, null);
        }

        public static void RenderPartial(string partialName, ViewDataDictionary viewData)
        {
            RenderPartial(partialName, null, viewData);
        }

        public static void RenderPartial(string partialName, object model, ViewDataDictionary viewData)
        {
            RenderPartial(partialName, model, viewData, HttpContext.Current.Response.Output);
        }

        public static string RenderHtml(string partialName)
        {
            return RenderHtml(partialName, null, null);
        }

        public static string RenderHtml(string partialName, object model)
        {
            return RenderHtml(partialName, model, null);
        }

        public static string RenderHtml(string partialName, ViewDataDictionary viewData)
        {
            return RenderHtml(partialName, null, viewData);
        }

        public static string RenderHtml(string partialName, object model, ViewDataDictionary viewData)
        {
            using (var output = new StringWriter())
            {
                RenderPartial(partialName, model, viewData, output);
                return output.ToString();
            }
        }

        private static void RenderPartial(string partialName, object model, ViewDataDictionary viewData,
                                          TextWriter writer)
        {
            //get a wrapper for the legacy WebForm context
            var httpContext = new HttpContextWrapper(HttpContext.Current);

            //create a mock route that points to the empty controller
            var routeData = new RouteData();
            routeData.Values.Add("controller", "WebFormController");

            //create a controller context for the route and http context
            var controllerContext = new ControllerContext(
                new RequestContext(httpContext, routeData), new WebFormController());

            //find the partial view using the viewengine
            var view = ViewEngines.Engines.FindPartialView(controllerContext, partialName).View;

            var dictionary = new ViewDataDictionary(viewData ?? model);
            if ((viewData != null) && (model != null))
            {
                dictionary.Model = model;
            }

            //create a view context and assign the model
            var viewContext = new ViewContext(controllerContext, view,
                                              dictionary,
                                              new TempDataDictionary(),
                                              writer);

            //render the partial view
            view.Render(viewContext, writer);
        }

        public class WebFormController : Controller
        {
        }
    }
}