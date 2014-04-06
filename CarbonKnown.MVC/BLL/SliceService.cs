using System;
using System.Collections.Generic;
using System.Data.Entity.Hierarchy;
using System.Linq;
using System.Text.RegularExpressions;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Constants;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.BLL
{
    public class SliceService : ISliceService
    {
        public static IDictionary<Section, DashboardConfiguration> Configurations
            = new SortedDictionary<Section, DashboardConfiguration>
            {
                {
                    Section.Overview,
                    new DashboardConfiguration
                    {
                        DisplayName = "Overview",
                        ShowCo2 = true,
                        ActivityIds = new[]
                        {
                            Activity.Scope1Id,
                            Activity.Scope2Id,
                            Activity.Scope3Id,
                            Activity.NonKyotoId
                        }
                    }
                },
                {
                    Section.Water,
                    new DashboardConfiguration
                    {
                        DisplayName = "Water",
                        ShowCo2 = false,
                        ActivityIds = new[]
                        {
                            Activity.WaterId
                        }
                    }
                },
                {
                    Section.Electricity,
                    new DashboardConfiguration
                    {
                        DisplayName = "Electricity",
                        ShowCo2 = false,
                        ActivityIds = new[]
                        {
                            Activity.Scope2Id
                        }
                    }
                },
                {
                    Section.Paper,
                    new DashboardConfiguration
                    {
                        DisplayName = "Paper",
                        ShowCo2 = false,
                        ActivityIds = new[]
                        {
                            Activity.PaperId
                        }
                    }
                },
                {
                    Section.Waste,
                    new DashboardConfiguration
                    {
                        DisplayName = "Waste",
                        ShowCo2 = false,
                        ActivityIds = new[]
                        {
                            WasteActivityId.WasteToLandfillId,
                            WasteActivityId.RecylcedWasteId
                        }
                    }
                },
                {
                    Section.Travel,
                    new DashboardConfiguration
                    {
                        DisplayName = "Travel",
                        ShowCo2 = false,
                        ActivityIds = new[]
                        {
                            Activity.BusinessTravelId
                        }
                    }
                },
                {
                    Section.Fleet,
                    new DashboardConfiguration
                    {
                        DisplayName = "Fleet",
                        ShowCo2 = false,
                        ActivityIds = new[]
                        {
                            Activity.ThirdPartyVehicleFleetId,
                            Activity.CompanyOwnedVehicleFleetId
                        }
                    }
                },
                {
                    Section.Courier,
                    new DashboardConfiguration
                    {
                        DisplayName = "Courier",
                        ShowCo2 = false,
                        ActivityIds = new[]
                        {
                            Activity.CourierId
                        }
                    }
                }
            };

        private readonly DataContext context;

        public SliceService(DataContext context)
        {
            this.context = context;
        }

        public static string CreateSliceId(Guid? activityId, string costCode)
        {
            costCode = Regex.Replace(costCode, "[^0-9a-zA-Z]", string.Empty, RegexOptions.Compiled);
            return string.Format("{0}_{1}", activityId ?? Guid.Empty, costCode);
        }

        public virtual IEnumerable<string>
            GetCurrencies(
            DateTime startDate,
            DateTime endDate,
            HierarchyId[] nodes,
            HierarchyId centreNode)
        {
            var currencyTotals = new SortedDictionary<string, decimal>();
            foreach (var node in nodes)
            {
                var activityId = node;
                foreach (var total in 
                    from e in context.CarbonEmissionEntries
                    where
                        (e.EntryDate >= startDate) &&
                        (e.EntryDate <= endDate) &&
                        (e.ActivityGroupNode.IsDescendantOf(activityId)) &&
                        (e.CostCentreNode.IsDescendantOf(centreNode))
                    group
                        e.Money
                        by e.CostCentre.CurrencyCode
                    into g
                    select new
                    {
                        Code = g.Key,
                        TotalMoney = g.Sum()
                    })
                {
                    if (currencyTotals.ContainsKey(total.Code))
                    {
                        currencyTotals[total.Code] = currencyTotals[total.Code] + total.TotalMoney;
                    }
                    else
                    {
                        currencyTotals.Add(total.Code, total.TotalMoney);
                    }
                }
            }
            return currencyTotals
                .Select(pair => string.Format(CurrenciesContext.Cultures[pair.Key], "{0:C}", pair.Value));
        }

        public decimal TotalEmissions(
            DateTime startDate,
            DateTime endDate,
            HierarchyId groupNode,
            HierarchyId centreNode)
        {
            var query =
                from e in context.CarbonEmissionEntries
                where
                    (e.EntryDate >= startDate) &&
                    (e.EntryDate <= endDate) &&
                    (e.ActivityGroupNode.IsDescendantOf(groupNode)) &&
                    (e.CostCentreNode.IsDescendantOf(centreNode))
                select (decimal?) e.CarbonEmissions;
            return query.Sum() ?? 0M;
        }

        public decimal TotalUnits(
            DateTime startDate,
            DateTime endDate,
            HierarchyId groupNode,
            HierarchyId centreNode)
        {
            var query =
                from e in context.CarbonEmissionEntries
                where
                    (e.EntryDate >= startDate) &&
                    (e.EntryDate <= endDate) &&
                    (e.ActivityGroupNode.IsDescendantOf(groupNode)) &&
                    (e.CostCentreNode.IsDescendantOf(centreNode))
                select (decimal?) e.Units;
            return query.Sum() ?? 0M;
        }

        public static decimal GetTotal(
            Func<DateTime, DateTime, HierarchyId, HierarchyId, decimal> totalFunc,
            DateTime startDate,
            DateTime endDate,
            HierarchyId activityNode,
            HierarchyId centreNode,
            HierarchyId[] nodes)
        {
            return activityNode == null
                ? nodes.Sum(a => totalFunc(startDate, endDate, a, centreNode))
                : totalFunc(startDate, endDate, activityNode, centreNode);
        }

        public DashboardSummary CreateSummary(
            DashboardRequest request,
            SliceDataModel[] sliceData,
            ActivityGroup[] activities,
            CostCentre centre)
        {
            var configuration = Configurations[request.Section];
            var uomLong = string.Empty;
            var showCo2 = false;
            var groupNodes = activities.Select(a => a.Node).ToArray();
            var summary = new DashboardSummary
            {
                costCentre = centre.Name,
                activityGroup = (request.ActivityGroupId == null)
                    ? configuration.DisplayName
                    : activities[0].Name,
                displayTotal = true
            };
            var slices = sliceData
                .OrderBy(s => s.OrderId)
                .Select(s =>
                {
                    var slice = new SliceModel
                    {
                        activityGroupId = s.ActivityGroupId,
                        color = s.Color,
                        costCode = s.CostCode,
                        description = s.Description,
                        title = s.Title,
                        sliceId = CreateSliceId(s.ActivityGroupId, s.CostCode),
                    };
                    var noUnits =
                        (string.IsNullOrEmpty(s.UomShort)) ||
                        (string.IsNullOrEmpty(s.UomLong));

                    if (configuration.ShowCo2 || noUnits)
                    {
                        showCo2 = true;
                        slice.co2label = Constants.Constants.Co2LabelShort;
                        slice.amount = GetTotal(
                            TotalEmissions,
                            request.StartDate,
                            request.EndDate,
                            s.ActivityGroupNode,
                            s.CentreNode,
                            groupNodes)/1000;
                    }
                    if (configuration.ShowCo2 && !noUnits)
                    {
                        slice.uom = s.UomShort;
                        slice.units = GetTotal(
                            TotalUnits,
                            request.StartDate,
                            request.EndDate,
                            s.ActivityGroupNode,
                            s.CentreNode,
                            groupNodes);
                    }
                    if (!configuration.ShowCo2 && !noUnits)
                    {
                        slice.co2label = s.UomShort;
                        uomLong = s.UomLong;
                        slice.amount = GetTotal(
                            TotalUnits,
                            request.StartDate,
                            request.EndDate,
                            s.ActivityGroupNode,
                            s.CentreNode,
                            groupNodes);
                    }
                    return slice;
                }).ToArray();

            decimal lastYearTotal;
            decimal totalAmount;
            if (showCo2)
            {
                totalAmount = GetTotal(
                    TotalEmissions,
                    request.StartDate,
                    request.EndDate,
                    null,
                    centre.Node,
                    groupNodes)/1000;
                lastYearTotal = GetTotal(
                    TotalEmissions,
                    request.StartDate.AddYears(-1),
                    request.EndDate.AddYears(-1),
                    null,
                    centre.Node,
                    groupNodes)/1000;
            }
            else
            {
                totalAmount = GetTotal(
                    TotalUnits,
                    request.StartDate,
                    request.EndDate,
                    null,
                    centre.Node,
                    groupNodes);
                lastYearTotal = GetTotal(
                    TotalUnits,
                    request.StartDate.AddYears(-1),
                    request.EndDate.AddYears(-1),
                    null,
                    centre.Node,
                    groupNodes);
            }

            var yoy = (lastYearTotal == 0)
                ? 0
                : ((totalAmount - lastYearTotal)/lastYearTotal)*100;
            var currencies = GetCurrencies(
                request.StartDate,
                request.EndDate,
                groupNodes,
                centre.Node);
            summary.co2label = (showCo2)
                ? Constants.Constants.Co2LabelLong
                : uomLong;
            summary.currencies = currencies;
            summary.slices = slices;
            summary.total = totalAmount;
            summary.yoy = yoy;
            return summary;
        }

        public DashboardSummary CostCentre(DashboardRequest request)
        {
            var activity = (request.ActivityGroupId == null)
                ? null
                : context.ActivityGroups.Find(request.ActivityGroupId);

            var costCentreRoot = context.CostCentres.Find(request.CostCode);
            var slices = costCentreRoot
                .ChildrenCostCentres
                .Select(centre =>
                {
                    var returnModel = new SliceDataModel
                    {
                        CentreNode = centre.Node,
                        Color = centre.Color,
                        CostCode = centre.CostCode,
                        Description = centre.Description,
                        OrderId = centre.OrderId,
                        Title = centre.Name
                    };
                    if (activity == null) return returnModel;
                    returnModel.ActivityGroupId = activity.Id;
                    returnModel.ActivityGroupNode = activity.Node;
                    returnModel.UomLong = activity.UOMLong;
                    returnModel.UomShort = activity.UOMShort;
                    return returnModel;
                }).ToArray();
            if (!slices.Any())
            {
                slices = new[]
                {
                    new SliceDataModel
                    {
                        CentreNode = costCentreRoot.Node,
                        Color = costCentreRoot.Color,
                        CostCode = costCentreRoot.CostCode,
                        Description = costCentreRoot.Description,
                        OrderId = costCentreRoot.OrderId,
                        Title = costCentreRoot.Name
                    }
                };
                if (activity != null)
                {
                    slices[0].ActivityGroupId = activity.Id;
                    slices[0].ActivityGroupNode = activity.Node;
                    slices[0].UomLong = activity.UOMLong;
                    slices[0].UomShort = activity.UOMShort;
                }
            }
            if (activity != null) return CreateSummary(request, slices, new[] {activity}, costCentreRoot);
            var configuration = Configurations[request.Section];
            var activities = configuration.ActivityIds.Select(guid => context.ActivityGroups.Find(guid)).ToArray();
            return CreateSummary(request, slices, activities, costCentreRoot);
        }

        public DashboardSummary ActivityGroup(DashboardRequest request)
        {
            var costCentreRoot = context.CostCentres.Find(request.CostCode);
            IEnumerable<ActivityGroup> childGroups;
            ActivityGroup[] activities;

            var configuration = Configurations[request.Section];
            if (request.ActivityGroupId == null)
            {
                activities = configuration
                    .ActivityIds
                    .Select(id => context.ActivityGroups.Find(id))
                    .ToArray();
                var index = 1;
                childGroups = activities.Select(a =>
                {
                    a.OrderId = index*100;
                    index++;
                    return a;
                });
            }
            else
            {
                var actvity = context.ActivityGroups.Find(request.ActivityGroupId);
                activities = new[] {actvity};
                childGroups = actvity.ChildGroups;
            }
            var slices = childGroups
                .Select(a =>
                    new SliceDataModel
                    {
                        CentreNode = costCentreRoot.Node,
                        Color = a.Color,
                        CostCode = costCentreRoot.CostCode,
                        Description = a.Description,
                        OrderId = a.OrderId,
                        Title = a.Name,
                        ActivityGroupId = a.Id,
                        ActivityGroupNode = a.Node,
                        UomLong = a.UOMLong,
                        UomShort = a.UOMShort
                    }).ToArray();
            return CreateSummary(request, slices, activities, costCentreRoot);
        }
    }
}