using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CarbonKnown.Calculation;
using CarbonKnown.DAL;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VarianceController : Controller
    {
        private readonly DataContext context;

        private readonly IEnumerable<Select2Model> defaultColumnNames = new[]
            {
                new Select2Model {id = "Money", text = "Money"},
                new Select2Model {id = "Units", text = "Units"}
            };

        public VarianceController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Variances()
        {
            var model = context
                .Variances
                .Include("Calculation")
                .OrderByDescending(variance => variance.CalculationId)
                .ToArray()
                .Select(variance => new VarianceModel
                    {
                        id = variance.Id,
                        calculation = new Select2Model
                            {
                                id = variance.CalculationId.ToString(),
                                text = variance.Calculation.Name
                            },
                        columnName = variance.ColumnName,
                        maxValue = variance.MaxValue,
                        minValue = variance.MinValue
                    });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Calculations()
        {
            var model = context
                .Calculations
                .ToArray()
                .Select(calc => new Select2Model
                    {
                        id = calc.Id.ToString(),
                        text = calc.Name
                    });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private static bool IsNumericType(Type type)
        {
            if (type.IsEnum) return false;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        private static Type GetUnderlyingType(Type source)
        {
            if (source.IsGenericType
                && (source.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                return Nullable.GetUnderlyingType(source);
            }
            return source;
        }


        [HttpGet]
        public ActionResult ColumnNames(Guid calculationId)
        {
            if (calculationId == Guid.Empty)
            {
                return Json(new object[] {}, JsonRequestBehavior.AllowGet);
            }
            var model = defaultColumnNames
                .Union(CalculationModelFactory
                           .GetCustomProperties(calculationId)
                           .Where(descriptor => IsNumericType(GetUnderlyingType(descriptor.PropertyType)))
                           .Select(descriptor =>
                                   new Select2Model {id = descriptor.Name, text = descriptor.Name}));
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpsertVariance(VarianceModel model)
        {
            Guid calculationId;
            if (!Guid.TryParse(model.calculation.id, out calculationId))
            {
                return Json(new {model.id, sucess = false}, JsonRequestBehavior.DenyGet);
            }
            var id = model.id;
            var update = true;
            var variance =
                context.Variances.Find(id) ??
                context.Variances
                       .FirstOrDefault(v =>
                                       (v.CalculationId == calculationId) &&
                                       (v.ColumnName == model.columnName));

            if (variance == null)
            {
                update = false;
                variance = context.Variances.Create();
            }
            variance.CalculationId = calculationId;
            variance.ColumnName = model.columnName;
            variance.MaxValue = (model.minValue > model.maxValue) ? model.minValue : model.maxValue;
            variance.MinValue = (model.maxValue < model.minValue) ? model.maxValue : model.minValue;
            
            if (!update)
            {
                variance = context.Variances.Add(variance);
            }

            context.SaveChanges();

            return Json(new { id = variance.Id, sucess = true }, JsonRequestBehavior.DenyGet);
        }


        [HttpDelete]
        public ActionResult DeleteVariance(int id)
        {
            var variance = context.Variances.Find(id);
            if (variance == null) return Json(new { id, success = false }, JsonRequestBehavior.DenyGet);
            context.Variances.Remove(variance);

            context.SaveChanges();

            return Json(new { id, sucess = true }, JsonRequestBehavior.DenyGet);
        }

    }
}
