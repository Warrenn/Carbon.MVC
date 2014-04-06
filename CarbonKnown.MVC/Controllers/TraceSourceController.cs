using System;
using System.Data.Entity.Hierarchy;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Source;
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

            var activityNode =
                (activityGroupId == null)
                    ? new HierarchyId("/")
                    : context.ActivityGroups.Find(activityGroupId).Node;
            var costCentreNode =
                context.CostCentres.Find(costCode).Node;
            var query =
                from e in context.CarbonEmissionEntries
                where
                    (e.EntryDate >= startDate) &&
                    (e.EntryDate <= endDate) &&
                    (e.ActivityGroupNode.IsDescendantOf(activityNode)) &&
                    (e.CostCentreNode.IsDescendantOf(costCentreNode))
                group new
                {
                    e.Units,
                    e.Money,
                    e.CarbonEmissions
                } by e.SourceEntry.SourceId
                into g
                from source in context.DataSources
                join fileDataSource in context.Set<FileDataSource>() on
                    source.Id equals fileDataSource.Id into filejoin
                from subFileSource in filejoin.DefaultIfEmpty()
                join manualDataSource in context.Set<ManualDataSource>() on
                    source.Id equals manualDataSource.Id into manualjoin
                from subManualSource in manualjoin.DefaultIfEmpty()
                where source.Id == g.Key
                select new AuditHistory
                {
                    CurrentFileName = (subFileSource == null) ? null : subFileSource.CurrentFileName,
                    Name = (subFileSource == null) ? "Manual Entry" : subFileSource.OriginalFileName,
                    DateEdit = source.DateEdit,
                    UserName = source.UserName,
                    HandlerName = (subFileSource == null) ? subManualSource.DisplayType : subFileSource.HandlerName,
                    Emissions = g.Sum(arg => arg.CarbonEmissions)/1000,
                    Cost = g.Sum(arg => arg.Money),
                    Units = g.Sum(arg => arg.Units),
                    SourceId = g.Key
                };
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