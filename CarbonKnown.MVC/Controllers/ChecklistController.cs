using System;
using System.Linq;
using System.Web.Mvc;
using CarbonKnown.DAL;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize(Roles = "Admin,Capturer")]
    public class ChecklistController : Controller
    {
        private readonly DataContext context;

        public ChecklistController(DataContext context)
        {
            this.context = context;
        }

        [ChildActionOnly]
        public ActionResult SingleCostCodeCheckListTable(ChecklistTableSettings settings)
        {
            var startDate = new DateTime(settings.Year, settings.Month + 1, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var model = new ChecklistTableViewModel
                {
                    EndDate = endDate,
                    StartDate = startDate,
                    Heading = string.Empty,
                    Rows = new[]
                        {
                            new ChecklistTableRow
                                {
                                    CostCode = settings.CostCode,
                                    Heading = settings.Heading,
                                    Columns = settings
                                        .ActivityColumns
                                        .Select(pair => new ChecklistTableColumn
                                            {
                                                Heading = pair.Value,
                                                ActivitiyId = pair.Key,
                                                ContainsValues =
                                                    (context.TotalUnits(startDate, endDate, pair.Key,
                                                                        settings.CostCode)) > 0
                                            })
                                }
                        }
                };
            return PartialView("_ChecklistTable", model);
        }

        [ChildActionOnly]
        public ActionResult MultipleCostCodesCheckListTable(ChecklistTableSettings settings)
        {
            var startDate = new DateTime(settings.Year, settings.Month + 1, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var costCentre = context.CostCentres.Find(settings.CostCode);
            var model = new ChecklistTableViewModel
                {
                    EndDate = endDate,
                    StartDate = startDate,
                    Heading = settings.Heading,
                    Rows = costCentre
                        .ChildrenCostCentres
                        .Select(centre => new ChecklistTableRow
                            {
                                CostCode = centre.CostCode,
                                Heading = centre.Name,
                                Columns = settings
                                              .ActivityColumns
                                              .Select(pair => new ChecklistTableColumn
                                                  {
                                                      Heading = pair.Value,
                                                      ActivitiyId = pair.Key,
                                                      ContainsValues =
                                                          (context.TotalUnits(startDate, endDate, pair.Key,
                                                                              centre.CostCode) ) > 0
                                                  })
                            })
                };
            return PartialView("_ChecklistTable", model);
        }

        [HttpGet]
        public ActionResult Index(int? month, int? year)
        {
            var today = DateTime.Today;
            var dates =
                from e in context.CarbonEmissionEntries
                select e.EntryDate;
            if (!dates.Any())
            {
                dates = (new[] {today}).AsQueryable();
            }
            var minDate = dates.Min();
            var maxDate = dates.Max();
            month = (month ?? (today.Month - 1))%12;
            year = year ?? today.Year;
            year = (year.Value > maxDate.Year) ? maxDate.Year : year;
            year = (year.Value < minDate.Year) ? minDate.Year : year;
            var model = new CheckListModel
                {
                    MaxYearInRange = maxDate.Year,
                    MinYearInRange = minDate.Year,
                    SelectedMonth = month.Value,
                    SelectedYear = year.Value
                };
            return View(model);
        }
    }
}