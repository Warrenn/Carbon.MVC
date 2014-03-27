using System;
using System.Linq;
using System.Web.Mvc;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.MVC.BLL;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Models;
using CarbonKnown.Print;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize]
    public class ComparisonController : Controller
    {
        private readonly DataContext context;
        private readonly ComparisonChartDataService service;

        public ComparisonController(DataContext context,ComparisonChartDataService service)
        {
            this.context = context;
            this.service = service;
        }

        private ComparisonChartViewModel CreatePrintModel(ComparisonChartRequestModel request)
        {
            var model = service.CreateModel(request);
            model.request = request;
            foreach (var series in model.series.ToArray())
            {
                var seriesRequest = new ComparisonSeriesRequestModel
                    {
                        activityId = series.activityId,
                        costCode = series.costCode,
                        endDate = request.endDate,
                        startDate = request.startDate,
                        target = series.target,
                        targetType = request.targetType
                    };
                series.data = service.ComparisonData(seriesRequest).ToArray();
            }
            return model;
        }

        [HttpGet]
        public ActionResult PrintJpg(ComparisonChartRequestModel request)
        {
            var model = CreatePrintModel(request);
            return PrintResult.PrintToJpeg("Print", model);
        }

        [HttpGet]
        public ActionResult PrintPdf(ComparisonChartRequestModel request)
        {
            var model = CreatePrintModel(request);
            return PrintResult.PrintToPdf("Print", model);
        }

        [HttpGet]
        public ActionResult Index()
        {
            var today = DateTime.Today.AddYears(-1);
            var startDate = new DateTime(today.Year, today.Month, 1);
            var endDate = startDate.AddYears(1).AddDays(-1);
            var model = new ComparisonChartRequestModel
                {
                    targetType = TargetType.CarbonEmissions,
                    startDate = startDate,
                    endDate = endDate
                };
            return View(model);
        }

        [HttpGet]
        public ActionResult ComparisonChart(ComparisonChartRequestModel request)
        {
            var model = service.CreateModel(request);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ComparisonData(ComparisonSeriesRequestModel request)
        {
            var model = service.ComparisonData(request).Select(value => new { value });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult HasTarget(
            Guid? activityId,
            string costCode,
            TargetType targetType)
        {
            var hasTarget = context
                .EmissionTargets
                .Any(target =>
                     (target.ActivityGroupId == activityId) &&
                     (target.CostCentreCostCode == costCode) &&
                     (target.TargetType == targetType));
            return Json(new {hasTarget}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [XSRFTokenValidation]
        public ActionResult AddSeries(ComparisonAddSeriesModel model)
        {
            if (string.IsNullOrEmpty(model.name) ||
                string.IsNullOrEmpty(model.costCode) ||
                (model.activityId == Guid.Empty))
            {
                return Json(new
                    {
                        id = -1,
                        success = false
                    }, JsonRequestBehavior.DenyGet);
            }

            var newSeries = context
                .GraphSeries
                .FirstOrDefault(series =>
                                (series.ActivityGroupId == model.activityId) &&
                                (series.CostCentreCostCode == model.costCode) &&
                                (series.TargetType == model.targetType));

            var addnew = (newSeries == null);
            if (addnew)
            {
                newSeries = context.GraphSeries.Create();
            }

            newSeries.ActivityGroupId = model.activityId;
            newSeries.CostCentreCostCode = model.costCode;
            newSeries.IncludeTarget = model.target;
            newSeries.TargetType = model.targetType;
            newSeries.Name = model.name;

            if (addnew)
            {
                newSeries = context.GraphSeries.Add(newSeries);
            }

            context.SaveChanges();

            return Json(new
                {
                    id = newSeries.Id,
                    success = true
                }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [XSRFTokenValidation]
        public ActionResult RemoveSeries(int id)
        {
            var graphSeries = context.GraphSeries.FirstOrDefault(series => series.Id == id);
            if (graphSeries == null) return Json(new {succeeded = false}, JsonRequestBehavior.DenyGet);
            context.GraphSeries.Remove(graphSeries);
            context.SaveChanges();
            return Json(new {succeeded = true}, JsonRequestBehavior.DenyGet);
        }
    }
}