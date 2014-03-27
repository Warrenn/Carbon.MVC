using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.Factors.WCF;

namespace CarbonKnown.MVC.DAL
{
    public class CalculationDataContext : ICalculationDataContext, IDisposable
    {
        private readonly IFactorsService factorsService;
        private readonly DataContext context;

        private static readonly ConcurrentDictionary<Guid, Lazy<IEnumerable<FactorValues>>> FactorValuesById =
            new ConcurrentDictionary<Guid, Lazy<IEnumerable<FactorValues>>>();

        private static readonly ConcurrentDictionary<string, Lazy<IEnumerable<FactorValues>>> FactorValuesByName =
            new ConcurrentDictionary<string, Lazy<IEnumerable<FactorValues>>>();

        private readonly Lazy<IEnumerable<RouteDistance>> airRouteDistances =
            new Lazy<IEnumerable<RouteDistance>>();

        private readonly Lazy<IEnumerable<RouteDistance>> courierRouteDistances =
            new Lazy<IEnumerable<RouteDistance>>();

        private  IEnumerable<FactorValues> GetValuesById(Guid factorId)
        {
            var lazy = FactorValuesById
                .GetOrAdd(
                    factorId,
                    id => new Lazy<IEnumerable<FactorValues>>(
                              () => factorsService.FactorValuesById(factorId)));
            return lazy.Value;
        }

        private  IEnumerable<FactorValues> GetValuesByName(string factorName)
        {
            var lazy = FactorValuesByName
                .GetOrAdd(
                    factorName,
                    id => new Lazy<IEnumerable<FactorValues>>(
                              () => factorsService.FactorValuesByName(factorName)));
            return lazy.Value;
        }

        public CalculationDataContext(DataContext context, IFactorsService factorsService)
        {
            this.factorsService = factorsService;
            this.context = context;
            courierRouteDistances = new Lazy<IEnumerable<RouteDistance>>(factorsService.CourierRouteDistances);
            airRouteDistances = new Lazy<IEnumerable<RouteDistance>>(factorsService.AirRouteDistances);
        }

        public decimal? CourierRouteDistance(string code1, string code2)
        {
            return
                (from distance in courierRouteDistances.Value
                 where (((distance.Code1 == code1) && (distance.Code2 == code2)) ||
                        ((distance.Code1 == code2) && (distance.Code2 == code1)))
                 select distance.Distance).FirstOrDefault();
        }

        public decimal? AirRouteDistance(string code1, string code2)
        {
            return
                (from distance in airRouteDistances.Value
                 where (((distance.Code1 == code1) && (distance.Code2 == code2)) ||
                        ((distance.Code1 == code2) && (distance.Code2 == code1)))
                 select distance.Distance).FirstOrDefault();
        }

        public decimal? FactorValue(DateTime effectiveDate, Guid factorId)
        {
            var factorValue = GetValuesById(factorId).FirstOrDefault(values => values.EffectiveDate <= effectiveDate);
            return (factorValue == null) ? (decimal?) null : factorValue.FactorValue;
        }

        public decimal? FactorValue(DateTime effectiveDate, string factorName)
        {
            var factorValue = GetValuesByName(factorName).FirstOrDefault(values => values.EffectiveDate <= effectiveDate);
            return (factorValue == null) ? (decimal?)null : factorValue.FactorValue;
        }

        public IEnumerable<string> AirRouteCodes(string code)
        {
            return
                airRouteDistances
                    .Value
                    .Where(distance => (string.IsNullOrEmpty(code)) || (distance.Code1 == code))
                    .Select(distance => distance.Code2)
                    .Union(
                        airRouteDistances
                            .Value
                            .Where(distance => (string.IsNullOrEmpty(code)) || (distance.Code2 == code))
                            .Select(distance => distance.Code1))
                    .Distinct();
        }

        public IEnumerable<string> CourierRouteCodes(string code)
        {
            return
                courierRouteDistances
                    .Value
                    .Where(distance => (string.IsNullOrEmpty(code)) || (distance.Code1 == code))
                    .Select(distance => distance.Code2)
                    .Union(
                        courierRouteDistances
                            .Value
                            .Where(distance => (string.IsNullOrEmpty(code)) || (distance.Code2 == code))
                            .Select(distance => distance.Code1))
                    .Distinct();
        }

        public DateTime? CalculationDateForFactorName(string factorName)
        {
            return GetValuesByName(factorName).Max(values => values.CalculationDate);
        }

        public DateTime? CalculationDateForFactorId(Guid factorId)
        {
            return GetValuesById(factorId).Max(values => values.CalculationDate);
        }

        public bool CostCodeValid(string costCode)
        {
            return context
                .CostCentres
                .Any(centre => centre.CostCode == costCode);
        }

        public Variance Variance(Guid calculationId, string column)
        {
            return context
                .Variances
                .FirstOrDefault(variance =>
                                (variance.CalculationId == calculationId) &&
                                (variance.ColumnName == column));
        }

        public bool EntryIsDuplicate(Guid entryId, int hash)
        {
            return context
                .DataEntries
                .Any(e => (e.Hash == hash) && (e.Id != entryId));
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
