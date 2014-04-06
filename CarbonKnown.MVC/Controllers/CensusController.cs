using System.Linq;
using System.Web.Mvc;
using CarbonKnown.DAL;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Models;
using CarbonKnown.MVC.Properties;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CensusController : Controller
    {
        private readonly DataContext context;

        public CensusController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult Census()
        {
            var model = context
                .Census
                .OrderByDescending(census => census.StartDate)
                .Select(census => new CensusModel
                    {
                        id = census.Id,
                        displayName = census.DisplayName,
                        startDate = census.StartDate,
                        endDate = census.EndDate,
                        employeesCovered = census.EmployeesCovered,
                        scopeBoundries = census.ScopeBoundries,
                        squareMeters = census.SquareMeters,
                        totalEmployees = census.TotalEmployees
                    });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpsertCensus(CensusModel model)
        {
            var id = model.id;
            var update = true;
            var census = context.Census.Find(id);
            var costCentre = context.CostCentres.Find(Settings.Default.RootCostCentre);
            if (census == null)
            {
                update = false;
                census = context.Census.Create();
                census.CompanyName = costCentre.Name;
                census.CostCentres = new[] {costCentre};
            }
            census.DisplayName = model.displayName;
            census.StartDate = model.startDate;
            census.EndDate = model.endDate;
            census.EmployeesCovered = model.employeesCovered;
            census.TotalEmployees = model.totalEmployees;
            census.SquareMeters = model.squareMeters;
            census.ScopeBoundries = model.scopeBoundries;

            if (!update)
            {
                census = context.Census.Add(census);
            }

            context.SaveChanges();

            return Json(new {id = census.Id, sucess = true}, JsonRequestBehavior.DenyGet);
        }


        [HttpDelete]
        public ActionResult DeleteCensus(int id)
        {
            var census = context.Census.Find(id);
            if (census == null) return Json(new {id, success = false}, JsonRequestBehavior.DenyGet);
            context.Census.Remove(census);

            context.SaveChanges();

            return Json(new {id, sucess = true}, JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}
