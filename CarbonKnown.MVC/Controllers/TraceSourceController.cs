using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Models;
using CarbonKnown.MVC.Properties;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize(Roles = "Admin,Capturer")]
    public class TraceSourceController : Controller
    {
        private readonly DataContext context;

        public TraceSourceController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult AuditHistory(
            DateTime startDate,
            DateTime endDate,
            string costCode,
            Guid? activityGroupId,
            DataTableParamModel request)
        {
            var builder = new DataTableResultModelBuilder<AuditHistory>();
            var query = context.AuditHistory(startDate, endDate, activityGroupId, costCode);
            builder.AddQueryable(query);
            builder.AddDataExpression(arg => new object[]
            {
                HttpUtility.HtmlEncode(arg.Name),
                arg.DateEdit.ToString(Constants.Constants.DateFormat),
                HttpUtility.HtmlEncode(arg.UserName),
                arg.Units.ToString(Constants.Constants.NumberFormat, CultureInfo.CurrentCulture),
                arg.Emissions.ToString(Constants.Constants.NumberFormat, CultureInfo.CurrentCulture),
                arg.Cost.ToString(Constants.Constants.NumberFormat, CultureInfo.CurrentCulture),
                arg.HandlerName,
                Url.RouteUrl("editsource", new {arg.SourceId}),
                Url.RouteUrl("selectsource", new {arg.SourceId})
            });

            builder.AddSearchFilter(arg =>
                arg.Name.Contains(request.sSearch) ||
                arg.UserName.Contains(request.sSearch) ||
                arg.HandlerName.Contains(request.sSearch));
            builder.AddSortExpression(data => data.Name);
            builder.AddSortExpression(data => data.DateEdit);
            builder.AddSortExpression(data => data.UserName);
            builder.AddSortExpression(data => data.Units);
            builder.AddSortExpression(data => data.Emissions);
            builder.AddSortExpression(data => data.Cost);
            builder.AddSortExpression(data => data.HandlerName);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Index(
            DateTime? startDate,
            DateTime? endDate,
            string costCode,
            Guid? activityGroupId,
            DataTableParamModel request)
        {
            var today = DateTime.Today;
            startDate = startDate ?? new DateTime(today.Year, today.Month, 1);
            endDate = endDate ?? startDate.Value.AddMonths(1).AddDays(-1);
            costCode = costCode ?? Settings.Default.RootCostCentre;

            var model = new AuditHistoryModel
            {
                ActivityGroupId = activityGroupId,
                CostCode = costCode,
                StartDate = startDate.Value,
                EndDate = endDate.Value
            };
            return View(model);
        }
    }
}