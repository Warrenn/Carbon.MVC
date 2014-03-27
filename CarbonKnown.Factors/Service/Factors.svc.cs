using System;
using System.Collections.Generic;
using System.Linq;
using CarbonKnown.Factors.DAL;
using CarbonKnown.Factors.WCF;

namespace CarbonKnown.Factors.Service
{
    public class Factors : IFactorsService
    {
        private readonly DataContext context;

        public Factors()
            : this(new DataContext())
        {
        }

        public Factors(DataContext context)
        {
            this.context = context;
        }

        public IEnumerable<FactorValues> FactorValuesById(Guid factorId)
        {
            return
                (from factorValue in context.FactorValues
                 where
                     (factorValue.FactorId == factorId)
                 orderby factorValue.EffectiveDate descending
                 select new FactorValues
                     {
                         CalculationDate = factorValue.CalculationDate,
                         EffectiveDate = factorValue.EffectiveDate,
                         FactorValue = factorValue.Value
                     }).ToArray();
        }

        public IEnumerable<FactorValues> FactorValuesByName(string factorName)
        {
            return
                (from factorValue in context.FactorValues.Include("Factor")
                 where
                     (factorValue.Factor.FactorName == factorName)
                 orderby factorValue.EffectiveDate descending
                 select new FactorValues
                     {
                         CalculationDate = factorValue.CalculationDate,
                         EffectiveDate = factorValue.EffectiveDate,
                         FactorValue = factorValue.Value
                     }).ToArray();
        }

        public IEnumerable<RouteDistance> AirRouteDistances()
        {
            return context
                .AirRouteDistances
                .Select(distance => new RouteDistance
                    {
                        CalculationDate = distance.CalculationDate,
                        Code1 = distance.Code1,
                        Code2 = distance.Code2,
                        Distance = distance.Distance
                    });
        }

        public IEnumerable<RouteDistance> CourierRouteDistances()
        {
            return context
                .CourierRouteDistances
                .Select(distance => new RouteDistance
                {
                    CalculationDate = distance.CalculationDate,
                    Code1 = distance.Code1,
                    Code2 = distance.Code2,
                    Distance = distance.Distance
                });
        }
    }
}
