using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace CarbonKnown.Print
{
    public sealed partial class PrintResult : ActionResult
    {
        private readonly string viewName;
        private readonly string masterName;
        private readonly object model;
        private readonly Action<string, string, Bitmap, HttpResponseBase> printCommand;

        private PrintResult(string viewName, string masterName, object model, Action<string, string, Bitmap, HttpResponseBase> printCommand)
        {
            this.viewName = viewName;
            this.masterName = masterName;
            this.model = model;
            this.printCommand = printCommand;
        }

        public object Model { get { return model; } }

        internal Func<string, string, object, ControllerContext, string> RetrieveHtml =
            (viewName, masterName, model, controllerContext) =>
                {
                    if (controllerContext == null)
                    {
                        throw new ArgumentNullException("controllerContext");
                    }
                    if (string.IsNullOrEmpty(viewName))
                    {
                        viewName = controllerContext.RouteData.GetRequiredString("action");
                    }
                    var controller = controllerContext.Controller;
                    var result = ViewEngines.Engines.FindView(controllerContext, viewName, masterName);
                    var view = result.View;
                    var viewData = controller.ViewData;
                    if ((viewData != null) && (model != null))
                    {
                        viewData.Model = model;
                    }
                    var tempData = controller.TempData;

                    using (var output = new StringWriter())
                    {
                        var viewContext = new ViewContext(controllerContext, view, viewData, tempData, output);
                        view.Render(viewContext, output);
                        result.ViewEngine.ReleaseView(controllerContext, view);
                        return output.ToString();
                    }
                };

        public override void ExecuteResult(ControllerContext context)
        {
            var htmlContent = RetrieveHtml(viewName, masterName, model, context);

            var actionName = context.RouteData.GetRequiredString("action");
            var controllerName = context.RouteData.GetRequiredString("controller");
            var resetEvent = new AutoResetEvent(false);

            Bitmap bitmap;
            using (var browser = new Browser(htmlContent, resetEvent))
            {
                WaitHandle.WaitAll(new WaitHandle[] {resetEvent});
                bitmap = browser.BitmapResult;
            }
            var httpContext = context.HttpContext;
            var response = httpContext.Response;

            printCommand(controllerName, actionName, bitmap, response);
        }
    }
}
