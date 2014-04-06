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
        private readonly DataContext dataContext;

        public ChecklistController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public bool ContainsValues(
            DateTime startDate,
            DateTime endDate,
            Guid groupId,
            string costCode)
        {
            var activity = dataContext.ActivityGroups.Find(groupId);
            var costCentre = dataContext.CostCentres.Find(costCode);

            var query =
                from e in dataContext.CarbonEmissionEntries
                where
                    (e.EntryDate >= startDate) &&
                    (e.EntryDate <= endDate) &&
                    (e.ActivityGroupNode.IsDescendantOf(activity.Node)) &&
                    (e.CostCentreNode.IsDescendantOf(costCentre.Node))
                select e.CarbonEmissions;
            return query.Any();
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
                                    ContainsValues(startDate, endDate, pair.Key, settings.CostCode)
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
            var costCentre = dataContext.CostCentres.Find(settings.CostCode);
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
                                    (ContainsValues(startDate, endDate, pair.Key,
                                        centre.CostCode))
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
                from e in dataContext.CarbonEmissionEntries
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