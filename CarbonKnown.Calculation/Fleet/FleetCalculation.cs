using System;
using System.Collections.Generic;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Constants;
using CarbonKnown.DAL.Models.Fleet;

namespace CarbonKnown.Calculation.Fleet
{
    [Calculation(
        "Vehicle",
        CarbonKnown.DAL.Models.Constants.Calculation.Fleet,
        ConsumptionType = ConsumptionType.Other,
        Activities = typeof (FleetActivityId),
        Factors = typeof (CarbonKnown.DAL.Models.Constants.Factors.Fleet))]
    public class FleetCalculation : CalculationBase<FleetData>
    {
        public FleetCalculation(ICalculationDataContext context)
            : base(context)
        {
        }

        public static IDictionary<FleetScope, IDictionary<FuelType, Guid>> FactorMapping =
            new SortedDictionary<FleetScope, IDictionary<FuelType, Guid>>
                {
                    {
                        FleetScope.CompanyOwned, new SortedDictionary<FuelType, Guid>
                            {
                                {FuelType.Diesel, CarbonKnown.DAL.Models.Constants.Factors.Fleet.Scope1Diesel},
                                {FuelType.Petrol, CarbonKnown.DAL.Models.Constants.Factors.Fleet.Scope1Petrol}
                            }
                    },
                    {
                        FleetScope.ThirdParty, new SortedDictionary<FuelType, Guid>
                            {
                                {FuelType.Diesel, CarbonKnown.DAL.Models.Constants.Factors.Fleet.Scope3Diesel},
                                {FuelType.Petrol, CarbonKnown.DAL.Models.Constants.Factors.Fleet.Scope3Petrol}
                            }
                    },
                };

        public static IDictionary<FleetScope, IDictionary<FuelType, Guid>> ActivityMapping =
            new SortedDictionary<FleetScope, IDictionary<FuelType, Guid>>
                {
                    {
                        FleetScope.CompanyOwned, new SortedDictionary<FuelType, Guid>
                            {
                                {FuelType.Diesel, FleetActivityId.Scope1DieselId},
                                {FuelType.Petrol, FleetActivityId.Scope1PetrolId}
                            }
                    },
                    {
                        FleetScope.ThirdParty, new SortedDictionary<FuelType, Guid>
                            {
                                {FuelType.Diesel, FleetActivityId.Scope3DieselId},
                                {FuelType.Petrol, FleetActivityId.Scope3PetrolId}
                            }
                    },
                };

        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData,
                                                            FleetData entry)
        {
            var units = (decimal) dailyData.UnitsPerDay;
            var fuelType = (FuelType) entry.FuelType;
            var scope = (FleetScope) entry.Scope;

            var factorId = FactorMapping[scope][fuelType];
            var factorValue = GetFactorValue(factorId, effectiveDate);
            var emissions = units*factorValue;
            var calculationDate = Context.CalculationDateForFactorId(factorId);
            return new CalculationResult
                {
                    CalculationDate = calculationDate,
                    ActivityGroupId = ActivityMapping[scope][fuelType],
                    Emissions = emissions
                };
        }
    }
}
