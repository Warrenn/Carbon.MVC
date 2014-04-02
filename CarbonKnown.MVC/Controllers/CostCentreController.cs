using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Hierarchy;
using System.Linq;
using System.Runtime.InteropServices;
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
                .Select(currency => new Select2Model {id = currency.Code, text = currency.Name});

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

            return Json(new {costCode, success = (costCentre != null)});
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
            var parentCentre = context.CostCentres.Find(parentCostCode);
            if (parentCentre == null)
            {
                return Json(new {costCode, success = false});
            }
            var parentNode = parentCentre.Node;
            if (centre == null)
            {
                update = false;
                centre = context.CostCentres.Create();
                costCentre.orderId =
                    context
                        .CostCentres
                        .Count(centre1 => centre1.ParentCostCentreCostCode == parentCostCode) + 1;
                costCentre.node = parentNode.ToString() + costCentre.orderId + "/";
                centre.ParentCostCentreCostCode = parentCostCode;
                centre.CostCode = costCode;
            }

            centre.Node = new HierarchyId(costCentre.node);
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
            var parentCentre = context.CostCentres.Find(newParent);
            if ((costCentre == null) || (parentCentre == null))
            {
                return Json(new {costCode, success = false});
            }
            var newParentNode = parentCentre.Node;
            var oldNode = costCentre.Node;

            var orderId = context
                .CostCentres
                .Count(c => c.ParentCostCentreCostCode == newParent) + 1;
            var newNodeString = newParentNode.ToString() + orderId + "/";
            var newNode = new HierarchyId(newNodeString);

            ReParentNodes(newNode, oldNode);

            costCentre.ParentCostCentreCostCode = newParent;
            context.SaveChanges();

            return Json(new {costCode, success = true});
        }

        private void ReParentNodes(HierarchyId newNode, HierarchyId oldNode)
        {
            foreach (var centre in context.CostCentres.Where(centre => centre.Node.IsDescendantOf(oldNode)))
            {
                var reparentedNode = centre.Node.GetReparentedValue(oldNode, newNode);
                centre.Node = reparentedNode;
                var code = centre.CostCode;
                foreach (var entry in context.CarbonEmissionEntries.Where(e => e.SourceEntry.CostCode == code))
                {
                    entry.CostCentreNode = centre.Node;
                }
            }
        }

        [HttpPut]
        [XSRFTokenValidation]
        public ActionResult ReOrder(string costCode, int index)
        {
            var costCentre = context.CostCentres.Find(costCode);
            if (costCentre == null)
            {
                return Json(new {costCode, success = false});
            }
            var parentCostCode = costCentre.ParentCostCentreCostCode;
            var parent = context.CostCentres.Find(parentCostCode);
            var currentOrderId = costCentre.OrderId;
            var indexOrderId = index*100;
            var destinationOrderId = indexOrderId + 1;
            var nodeId = index;
            var shift = -1;
            var centres = context
                .CostCentres
                .Where(c => (c.ParentCostCentreCostCode == parentCostCode));

            if (currentOrderId > indexOrderId)
            {
                shift = 1;
                destinationOrderId = indexOrderId - 1;
                nodeId = (index - 1);
                centres = centres
                    .Where(c => (c.OrderId > destinationOrderId) && (c.OrderId <= currentOrderId))
                    .OrderByDescending(c => c.OrderId);
            }
            else
            {
                centres = centres
                    .Where(c => (c.OrderId >= currentOrderId) && (c.OrderId < destinationOrderId))
                    .OrderBy(c => c.OrderId);
            }

            var parentNode = parent.Node;
            var currentNode = costCentre.Node;
            var tempNode = new HierarchyId(parentNode.ToString() + nodeId + ".1/");
            ReParentNodes(tempNode, currentNode);
            costCentre.OrderId = destinationOrderId;
            context.SaveChanges();

            foreach (var centre in centres)
            {
                var orderId = centre.OrderId/100;
                orderId = orderId + shift;
                var node = centre.Node;
                var newNode = new HierarchyId(parentNode.ToString() + orderId + "/");
                ReParentNodes(newNode, node);
                centre.OrderId = orderId*100;
            }
            context.SaveChanges();

            var updatedNode = new HierarchyId(parentNode.ToString() + index + "/");
            ReParentNodes(updatedNode, tempNode);
            costCentre.OrderId = index*100;
            context.SaveChanges();
            
            return Json(new {costCode, success = true});
        }
    }
}
