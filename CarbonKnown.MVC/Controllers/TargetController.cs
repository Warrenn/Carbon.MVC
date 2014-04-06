using System;
using System.Linq;
using System.Web.Mvc;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TargetController : Controller
    {
        private readonly DataContext context;

        public TargetController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Targets()
        {
            var model = context
                .EmissionTargets
                .ToArray()
                .Select(target => new TargetModel
                    {
                        id = target.Id,
                        activityGroupId = target.ActivityGroupId,
                        activityGroupName = target.ActivityGroup.Name,
                        costCentreName = target.CostCentre.Name,
                        costCode = target.CostCentreCostCode,
                        initialAmount = target.InitialAmount,
                        initialDate = target.InitialDate,
                        targetAmount = target.TargetAmount,
                        targetDate = target.TargetDate,
                        targetType = Enum.GetName(typeof (TargetType), target.TargetType)
                    });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpsertTarget(TargetModel model)
        {
            if (model.activityGroupId == Guid.Empty)
            {
                return Json(new {model.id, sucess = false }, JsonRequestBehavior.DenyGet);
            }

            var id = model.id;
            var update = true;
            var target = context.EmissionTargets.Find(id) ??
                         context
                             .EmissionTargets
                             .FirstOrDefault(t =>
                                             (t.ActivityGroupId == model.activityGroupId) &&
                                             (t.CostCentreCostCode == model.costCode));
            if (target == null)
            {
                update = false;
                target = context.EmissionTargets.Create();
            }
            TargetType targetType;
            Enum.TryParse(model.targetType, true, out targetType);
            target.ActivityGroupId = model.activityGroupId;
            target.CostCentreCostCode = model.costCode;
            target.InitialAmount = model.initialAmount;
            target.InitialDate = model.initialDate;
            target.TargetAmount = model.targetAmount;
            target.TargetDate = model.targetDate;
            target.TargetType = targetType;

            if (!update)
            {
                target = context.EmissionTargets.Add(target);
            }

            context.SaveChanges();

            return Json(new { id = target.Id, sucess = true }, JsonRequestBehavior.DenyGet);
        }


        [HttpDelete]
        public ActionResult DeleteTarget(int id)
        {
            var target = context.EmissionTargets.Find(id);
            if (target == null) return Json(new { id, success = false }, JsonRequestBehavior.DenyGet);
            context.EmissionTargets.Remove(target);

            context.SaveChanges();

            return Json(new { id, sucess = true }, JsonRequestBehavior.DenyGet);
        }
    }
}
