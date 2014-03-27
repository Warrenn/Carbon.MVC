using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Models;
using CarbonKnown.MVC.Properties;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CostCentreController : Controller
    {
        private readonly DataContext context;

        public CostCentreController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        private static IEnumerable<string> GetConsumptionTypes(ConsumptionType? type)
        {
            if (type == null) return Enumerable.Empty<string>();
            return (Enum.GetValues(typeof (ConsumptionType))
                        .Cast<ConsumptionType>()
                        .Where(value => type.Value.HasFlag(value))
                        .Select(value => Enum.GetName(typeof (ConsumptionType), value)));
        }

        private static ConsumptionType? GetConsumptionType(IEnumerable<string> types)
        {
            ConsumptionType? returnType = null;
            if ((types == null) || (!types.Any())) return null;
            foreach (var type in types)
            {
                ConsumptionType currentType;
                if (Enum.TryParse(type, true, out currentType))
                {
                    returnType = (returnType ?? currentType) | currentType;
                }
            }
            return returnType;
        }

        private Select2Model GetCurrency(string currencyCode)
        {
            var currency = context.Currencies.Find(currencyCode);
            if (currency == null) return new Select2Model();
            return new Select2Model
                {
                    id = currencyCode,
                    text = currency.Name
                };
        }

        [HttpGet]
        public ActionResult ChildCostCentres(string costCode)
        {
            var children = context
                .CostCentres
                .Where(centre => centre.ParentCostCentreCostCode == costCode)
                .OrderBy(centre => centre.OrderId)
                .ToArray()
                .Select(centre => new CostCentreModel
                    {
                        name = centre.Name,
                        costCode = centre.CostCode,
                        color = centre.Color,
                        currencyCode = GetCurrency(centre.CurrencyCode),
                        consumptionTypes = GetConsumptionTypes(centre.ConsumptionType),
                        description = centre.Description,
                        orderId = centre.OrderId,
                        parentCostCode = centre.ParentCostCentreCostCode
                    });

            return Json(children, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Currency()
        {
            var currencies = context
                .Currencies
                .Select(currency => new Select2Model{id = currency.Code, text = currency.Name});

            return Json(currencies, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CanDelete(string costCode)
        {
            var dependencies =
                context
                    .CostCentres
                    .Any(centre => centre.ParentCostCentreCostCode == costCode) ||
                context
                    .DataEntries
                    .Any(entry => entry.CostCode == costCode) ||
                context
                    .EmissionTargets
                    .Any(entry => entry.CostCentreCostCode == costCode);
            

            return Json(new {canDelete = !dependencies}, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        [XSRFTokenValidation]
        public ActionResult DeleteCostCentre(string costCode)
        {
            var costCentre = context.CostCentres.Find(costCode);
            if (costCentre != null)
            {
                context.CostCentres.Remove(costCentre);
                context.SaveChanges();
            }

            return Json(new { costCode, success = (costCentre != null) });
        }

        [HttpPost]
        [XSRFTokenValidation]
        public ActionResult UpsertCostCentre(CostCentreModel costCentre)
        {
            var costCode = costCentre.costCode;
            if ((string.IsNullOrEmpty(costCentre.name)) ||
                (string.IsNullOrEmpty(costCentre.color)))
            {
                return Json(new {costCode, success = false});
            }

            var parentCostCode = string.IsNullOrEmpty(costCentre.parentCostCode)
                                            ? Settings.Default.RootCostCentre
                                            : costCentre.parentCostCode;

            if (string.Equals(parentCostCode, costCode, StringComparison.InvariantCultureIgnoreCase))
            {
                return Json(new {costCode, success = false});
            }

            var update = true;
            var centre = context.CostCentres.Find(costCode);
            if (centre == null)
            {
                update = false;
                centre = context.CostCentres.Create();
                costCentre.orderId =
                    context
                        .CostCentres
                        .Count(centre1 => centre1.ParentCostCentreCostCode == parentCostCode);
                centre.ParentCostCentreCostCode = parentCostCode;
                centre.CostCode = costCode;
            }

            centre.Color = costCentre.color;
            centre.ConsumptionType = GetConsumptionType(costCentre.consumptionTypes);
            centre.CurrencyCode = costCentre.currencyCode.id;
            centre.Description = costCentre.description;
            centre.Name = costCentre.name;
            centre.OrderId = costCentre.orderId;

            if (!update)
            {
                context.CostCentres.Add(centre);
            }

            context.SaveChanges();

            return Json(new {costCode, success = true});
        }

        [HttpPut]
        [XSRFTokenValidation]
        public ActionResult ReParent(string costCode, string newParent)
        {
            if (string.Equals(costCode, newParent, StringComparison.InvariantCultureIgnoreCase))
            {
                return Json(new {costCode, success = false});
            }
            var costCentre = context.CostCentres.Find(costCode);
            if (costCentre != null)
            {
                costCentre.ParentCostCentreCostCode = newParent;
                context.SaveChanges();
            }

            return Json(new {costCode, success = (costCentre != null)});
        }

        [HttpPut]
        [XSRFTokenValidation]
        public ActionResult ReOrder(string costCode, int index)
        {
            var costCentre = context.CostCentres.Find(costCode);
            if (costCentre != null)
            {
                costCentre.OrderId = index;
                context.SaveChanges();
            }

            return Json(new {costCode, success = (costCentre != null)});
        }
    }
}
