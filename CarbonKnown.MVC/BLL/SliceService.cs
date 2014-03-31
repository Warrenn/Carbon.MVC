using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Constants;
using CarbonKnown.MVC.DAL;
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

        private readonly ISummaryDataContext summaryContext;
        private readonly DataContext context;

        public SliceService(ISummaryDataContext summaryContext,DataContext context)
        {
            this.summaryContext = summaryContext;
            this.context = context;
        }

        public static string CreateSliceId(Guid? activityId, string costCode)
        {
            costCode = Regex.Replace(costCode, "[^0-9a-zA-Z]", string.Empty, RegexOptions.Compiled);
            return string.Format("{0}_{1}", activityId ?? Guid.Empty, costCode);
        }

        public static string CreateCurrency(CurrencySummary summary)
        {
            return string.Format(new CultureInfo(summary.Locale), "{0:C}", summary.TotalMoney);
        }

        public IEnumerable<SliceTotal> TotalsByActivityGroup(DashboardRequest request)
        {
            var configuration = Configurations[request.Section];
            if (request.ActivityGroupId == null)
            {
                return configuration
                    .ActivityIds
                    .Select(activityId => new SliceTotal
                    {
                        ActivityGroupId = activityId,
                        CostCode = request.CostCode,
                        TotalUnits = summaryContext.TotalUnits(
                            request.StartDate,
                            request.EndDate,
                            activityId,
                            request.CostCode),
                        TotalCarbonEmissions = summaryContext.TotalEmissions(
                            request.StartDate,
                            request.EndDate,
                            activityId,
                            request.CostCode)
                    });
            }
            return summaryContext.TotalsByActivityGroup(
                request.StartDate,
                request.EndDate,
                request.ActivityGroupId,
                request.CostCode).ToArray();
        }


        public DashboardSummary CostCentre(DashboardRequest request)
        {
            var totalAmount = 0M;
            var configuration = Configurations[request.Section];
            var activity = context.ActivityGroups.Find(request.ActivityGroupId);
            Func<SliceTotal, decimal> aggregateFunction;
            string shortLabel;
            string longLabel;
            //when co2 is specifically asked for or when we can get get a total for units then use co2 by default
            var showCo2 = (configuration.ShowCo2) ||
                          (activity == null) ||
                          (string.IsNullOrEmpty(activity.UOMShort)) ||
                          (string.IsNullOrEmpty(activity.UOMLong));

            if(showCo2)
            {
                aggregateFunction = total => (total.TotalCarbonEmissions)/1000;
                shortLabel = Constants.Constants.Co2LabelShort;
                longLabel = Constants.Constants.Co2LabelLong;
                activity = activity ?? new ActivityGroup
                    {
                        UOMShort = shortLabel
                    };
            }
            else
            {
                aggregateFunction = total => total.TotalUnits;
                shortLabel = activity.UOMShort;
                longLabel = activity.UOMLong;
            }

            var slices = summaryContext
                .TotalsByCostCentre(
                    request.StartDate,
                    request.EndDate,
                    request.ActivityGroupId,
                    request.CostCode)
                .ToArray()
                .Select(total =>
                {
                    var costCentre = context.CostCentres.Find(total.CostCode) ?? new CostCentre();
                    var amount = aggregateFunction(total);
                    totalAmount = totalAmount + amount;
                    var sliceId = CreateSliceId(total.ActivityGroupId, total.CostCode);
                    return new
                    {
                        costCentre.OrderId,
                        slice = new SliceModel
                        {
                            activityGroupId = total.ActivityGroupId,
                            costCode = total.CostCode,
                            amount = amount,
                            co2label = shortLabel,
                            color = costCentre.Color,
                            description = costCentre.Description,
                            sliceId = sliceId,
                            title = costCentre.Name,
                            units = total.TotalUnits,
                            uom = activity.UOMShort
                        }
                    };
                })
                .OrderBy(arg => arg.OrderId)
                .Select(arg => arg.slice);
            var lastYearStart = request.StartDate.AddYears(-1);
            var lastYearEnd = request.EndDate.AddYears(-1);
            var lastYearTotal = (showCo2)
                ? summaryContext.TotalEmissions(lastYearStart, lastYearEnd, request.ActivityGroupId, request.CostCode)
                : summaryContext.TotalUnits(lastYearStart, lastYearEnd, request.ActivityGroupId, request.CostCode);
            var yoy = (lastYearTotal == 0)
                ? 0
                : ((totalAmount - lastYearTotal)/lastYearTotal)*100;
            var selectedCostCentre = context.CostCentres.Find(request.CostCode);
            var currencies = summaryContext
                .CurrenciesSummary(
                    request.StartDate,
                    request.EndDate,
                    request.ActivityGroupId,
                    request.CostCode)
                .Select(CreateCurrency)
                .ToArray();
            var summary = new DashboardSummary
            {
                activityGroup = activity.Name,
                co2label = longLabel,
                costCentre = selectedCostCentre.Name,
                currencies = currencies,
                displayTotal = true,
                slices = slices,
                total = totalAmount,
                yoy = yoy
            };
            return summary;
        }

        public DashboardSummary ActivityGroup(DashboardRequest request)
        {
            var totalAmount = 0M;
            var displayTotal = true;
            var co2Label = string.Empty;
            var configuration = Configurations[request.Section];
            var activityId = request.ActivityGroupId ?? Guid.Empty;
            var slices = TotalsByActivityGroup(request)
                .ToArray()
                .Select(total =>
                {
                    var activity = context.ActivityGroups.Find(total.ActivityGroupId);
                    decimal amount;
                    string shortLabel;
                    string longLabel;
                    var showCo2 = (configuration.ShowCo2) ||
                                  (activity == null) ||
                                  (string.IsNullOrEmpty(activity.UOMShort)) ||
                                  (string.IsNullOrEmpty(activity.UOMLong));
                    if (showCo2)
                    {
                        amount = (total.TotalCarbonEmissions)/1000;
                        shortLabel = Constants.Constants.Co2LabelShort;
                        longLabel = Constants.Constants.Co2LabelLong;
                        activity = activity ?? new ActivityGroup
                        {
                            OrderId = 0
                        };
                    }
                    else
                    {
                        amount = total.TotalUnits;
                        shortLabel = activity.UOMShort;
                        longLabel = activity.UOMLong;
                    }
                    if (!string.IsNullOrEmpty(co2Label) && (!string.Equals(co2Label, longLabel)))
                    {
                        displayTotal = false;
                    }
                    co2Label = longLabel;
                    totalAmount = totalAmount + amount;
                    var sliceId = CreateSliceId(total.ActivityGroupId, total.CostCode);
                    return new
                    {
                        activity.OrderId,
                        slice = new SliceModel
                        {
                            activityGroupId = total.ActivityGroupId,
                            costCode = total.CostCode,
                            amount = amount,
                            co2label = shortLabel,
                            color = activity.Color,
                            description = activity.Description,
                            sliceId = sliceId,
                            title = activity.Name,
                            units = total.TotalUnits,
                            uom = activity.UOMShort
                        }
                    };
                })
                .OrderBy(arg => arg.OrderId)
                .Select(arg => arg.slice);
            if (string.IsNullOrEmpty(co2Label) && displayTotal)
            {
                co2Label = Constants.Constants.Co2LabelLong;
            }
            var activityIds = (request.ActivityGroupId == null)
                                  ? configuration.ActivityIds
                                  : new[] {activityId};
            var yoy = 0M;
            if (displayTotal)
            {
                var lastYearStart = request.StartDate.AddYears(-1);
                var lastYearEnd = request.EndDate.AddYears(-1);
                var lastYearTotal =configuration.ShowCo2
                        ? activityIds.Sum(id => summaryContext.TotalEmissions(lastYearStart, lastYearEnd, id, request.CostCode))
                        : activityIds.Sum(id => summaryContext.TotalUnits(lastYearStart, lastYearEnd, id, request.CostCode));
                yoy = (lastYearTotal == 0)
                    ? 0
                    : ((totalAmount - lastYearTotal)/lastYearTotal)*100;
            }
            var activityName = (request.ActivityGroupId == null)
                               ? configuration.DisplayName
                               : context.ActivityGroups.Find(activityId).Name;
            var costCentre = context.CostCentres.Find(request.CostCode) ?? new CostCentre();
            var currencySummaries =
                (from id in activityIds
                 from currency in summaryContext
                     .CurrenciesSummary(
                         request.StartDate,
                         request.EndDate,
                         id,
                         request.CostCode)
                     .ToArray()
                 group currency by currency.Locale
                 into g
                 select new CurrencySummary
                     {
                         Locale = g.Key,
                         TotalMoney = g.Sum(s => s.TotalMoney)
                     })
                    .ToArray();
            var currencies = currencySummaries.Select(CreateCurrency);
            var summary = new DashboardSummary
            {
                activityGroup = activityName,
                co2label = co2Label,
                costCentre = costCentre.Name,
                currencies = currencies,
                displayTotal = displayTotal && !string.IsNullOrEmpty(co2Label),
                slices = slices,
                total = totalAmount,
                yoy = yoy
            };
            return summary;
        }
    }
}