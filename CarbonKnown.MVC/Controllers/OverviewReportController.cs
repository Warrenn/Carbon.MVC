using System;
using System.Linq;
using System.Web.Mvc;
using CarbonKnown.DAL;
using CarbonKnown.MVC.DAL;
using CarbonKnown.MVC.Models;
using CarbonKnown.Print;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize]
    public class OverviewReportController : Controller
    {
        private readonly DataContext context;
        private readonly ISummaryDataContext summaryContext;

        public OverviewReportController(DataContext context, ISummaryDataContext summaryContext)
        {
            this.context = context;
            this.summaryContext = summaryContext;
        }

        [HttpGet]
        public ActionResult Index(int? id = null)
        {
            var model = CreateOverViewReportModel(id);
            return View(model);
        }

        private OverviewReportModel CreateOverViewReportModel(int? id = null)
        {
            var censusList =
                (from census in context.Census
                 orderby census.EndDate descending
                 select census).ToArray();
            id = id ?? censusList.First().Id;
            var selectedCensus = context.Census.Find(id);
            var costCodes = selectedCensus.CostCentres.Select(centre => centre.CostCode).ToArray();
            if (selectedCensus == null) throw new ArgumentOutOfRangeException("id");
            var percentage = selectedCensus.TotalEmployees == 0
                                 ? 1M
                                 : selectedCensus.EmployeesCovered/(decimal) selectedCensus.TotalEmployees;
            var selectedId = selectedCensus.Id;
            var model =
                new OverviewReportModel
                    {
                        CensusItems = censusList,
                        CensusId = selectedId,
                        StartDate = selectedCensus.StartDate,
                        EndDate = selectedCensus.EndDate,
                        CompanyName = selectedCensus.CompanyName,
                        EmployeesCovered = selectedCensus.EmployeesCovered,
                        TotalEmployees = selectedCensus.TotalEmployees,
                        EmployeePerc = percentage,
                        SquareMeters = selectedCensus.SquareMeters,
                        ScopeBoundries = selectedCensus.ScopeBoundries,
                        CostCodes = costCodes
                    };
            return model;
        }

        [NonAction]
        public decimal CalculateTotal(DateTime startDate,DateTime endDate, Guid? activityId, string costCode)
        {
            return summaryContext.TotalEmissions(startDate, endDate, activityId, costCode);
        }

        [HttpGet]
        public ActionResult PrintPdf(int? id = null)
        {
            var model = CreateOverViewReportModel(id);
            return PrintResult.PrintToPdf("Print", model);
        }

        [HttpGet]
        public ActionResult PrintJpg(int? id = null)
        {
            var model = CreateOverViewReportModel(id);
            return PrintResult.PrintToJpeg("Print", model);
        }
    }
}