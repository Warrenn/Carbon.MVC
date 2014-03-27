using System;
using System.Collections.Generic;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Constants;
using CarbonKnown.DAL.Models.Electricity;
using FactorIds = CarbonKnown.DAL.Models.Constants.Factors;

namespace CarbonKnown.Calculation.Electricity
{
    [Calculation(
        "Electricity",
        CarbonKnown.DAL.Models.Constants.Calculation.Electricity,
        ConsumptionType = ConsumptionType.Electricity,
        Activities = typeof(ElectricityActivityId),
        Factors = typeof(FactorIds.Electricity))]
    public class ElectricityCalculation : CalculationBase<ElectricityData>
    {
        public ElectricityCalculation(ICalculationDataContext context)
            : base(context)
        {
        }

        public static IDictionary<ElectricityType, Guid> ActivityMapping = new SortedDictionary<ElectricityType, Guid>
            {
                {ElectricityType.SouthAfricanNationalGrid, ElectricityActivityId.SouthAfricanNationalGridId}
            };

        public static IDictionary<ElectricityType, Guid> FactorMapping = new SortedDictionary<ElectricityType, Guid>
            {
                {ElectricityType.SouthAfricanNationalGrid, FactorIds.Electricity.SouthAfricanNationalGrid}
            };

        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData, ElectricityData entry)
        {
            var electricityType = (ElectricityType) entry.ElectricityType;
            var factorId = FactorMapping[electricityType];
            var factorValue = GetFactorValue(factorId, effectiveDate);
            var emissions = dailyData.UnitsPerDay*factorValue;
            var calculationDate = Context.CalculationDateForFactorId(factorId);
            return new CalculationResult
            {
                CalculationDate = calculationDate,
                ActivityGroupId = ActivityMapping[electricityType],
                Emissions = emissions
            };
        }
    }
}
