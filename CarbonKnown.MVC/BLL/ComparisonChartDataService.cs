using System;
using System.Collections.Generic;
using System.Linq;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.MVC.DAL;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.BLL
{
    public class ComparisonChartDataService
    {
        private readonly DataContext context;
        private readonly ISummaryDataContext summaryContext;
        public static Random Rnd = new Random();

        public ComparisonChartDataService(DataContext context, ISummaryDataContext summaryContext)
        {
            this.context = context;
            this.summaryContext = summaryContext;
        }

        public static IEnumerable<string> Categories(DateTime startDate, DateTime endDate)
        {
            var indexDate = new DateTime(startDate.Year, startDate.Month, 1);
            var endRange = new DateTime(endDate.Year, endDate.Month, 1);
            while (indexDate <= endRange)
            {
                yield return indexDate.ToString("MMM yyyy");
                indexDate = indexDate.AddMonths(1);
            }
        }

        public static string GetUom(TargetType targetType, string currencyCode, string uom)
        {
            switch (targetType)
            {
                case TargetType.CarbonEmissions:
                    return Constants.Constants.Co2LabelShort;
                case TargetType.Money:
                    return currencyCode;
                case TargetType.Units:
                    return uom;
                default:
                    throw new ArgumentOutOfRangeException("targetType");
            }
        }

        public IEnumerable<ComparisonSeriesViewModel> ChartSeries(TargetType targetType)
        {
            foreach (var series in context.GraphSeries
                                          .Where(series =>
                                                 (series.TargetType == targetType)).Select(series => new
                                                     {
                                                         activityId = series.ActivityGroupId,
                                                         costCode = series.CostCentreCostCode,
                                                         id = series.Id,
                                                         name = series.Name,
                                                         target = series.IncludeTarget,
                                                         currencyCode = series.CostCentre.CurrencyCode,
                                                         uom = series.ActivityGroup.UOMShort
                                                     }).ToArray())
            {
                yield return new ComparisonSeriesViewModel
                    {
                        activityId = series.activityId,
                        costCode = series.costCode,
                        id = series.id,
                        name = series.name,
                        target = false,
                        uom = GetUom(targetType, series.currencyCode, series.uom)
                    };
                if (series.target)
                {
                    yield return new ComparisonSeriesViewModel
                        {
                            activityId = series.activityId,
                            costCode = series.costCode,
                            id = series.id,
                            name = series.name,
                            target = true,
                            uom = GetUom(targetType, series.currencyCode, series.uom)
                        };
                }
            }
        }

        public IEnumerable<decimal> Target(ComparisonSeriesRequestModel request)
        {
            var indexDate = new DateTime(request.startDate.Year, request.startDate.Month, 1);
            var target = context
                .EmissionTargets
                .FirstOrDefault(emissionTarget =>
                                (emissionTarget.CostCentreCostCode == request.costCode) &&
                                (emissionTarget.ActivityGroupId == request.activityId) &&
                                (emissionTarget.TargetType == request.targetType));
            if (target == null) yield break;
            var initialAmount = target.InitialAmount;
            var initialDate = new DateTime(target.InitialDate.Year, target.InitialDate.Month, 1);
            var targetDate = new DateTime(target.TargetDate.Year, target.TargetDate.Month, 1);
            var totalDays = (targetDate - initialDate).TotalDays;
            var factor = (target.TargetAmount - initialAmount) / (decimal)totalDays;
            while (indexDate <= request.endDate)
            {
                var nextDate = indexDate.AddMonths(1);
                var daysDifference = (decimal)(nextDate - initialDate).TotalDays;
                var nextAmount = initialAmount + (daysDifference * factor);
                yield return ((nextDate < initialDate) || (indexDate > targetDate)) ? 0M : nextAmount;
                indexDate = nextDate;
            }
        }

        public IEnumerable<decimal> ComparisonData(ComparisonSeriesRequestModel request)
        {
            if (request.target) return Target(request);
            IEnumerable<AverageData> averages;
            switch (request.targetType)
            {
                case TargetType.CarbonEmissions:
                    averages = summaryContext.AverageCo2(
                        request.startDate,
                        request.endDate,
                        request.activityId,
                        request.costCode);
                    break;
                case TargetType.Money:
                    averages = summaryContext.AverageMoney(
                        request.startDate,
                        request.endDate,
                        request.activityId,
                        request.costCode);
                    break;
                case TargetType.Units:
                    averages = summaryContext.AverageUnits(
                        request.startDate,
                        request.endDate,
                        request.activityId,
                        request.costCode);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("request");
            }
            var averageDictionary = averages
                .ToArray()
                .ToDictionary(data => data.YearMonth, data => data.Average);

            var returnValue = new List<decimal>();
            var indexDate = new DateTime(request.startDate.Year, request.startDate.Month, 1);
            var endRange = new DateTime(request.endDate.Year, request.endDate.Month, 1);
            
            while (indexDate <= endRange)
            {
                var lookup = (indexDate.Year*100) + indexDate.Month;
                var value = averageDictionary.ContainsKey(lookup) ? averageDictionary[lookup] : 0M;
                returnValue.Add(value);
                indexDate = indexDate.AddMonths(1);
            }
            
            return returnValue;
        }

        public ComparisonChartViewModel CreateModel(ComparisonChartRequestModel request)
        {
            var model = new ComparisonChartViewModel
                {
                    categories = Categories(request.startDate, request.endDate),
                    series = ChartSeries(request.targetType).ToArray()
                };
            return model;
        }
    }
}