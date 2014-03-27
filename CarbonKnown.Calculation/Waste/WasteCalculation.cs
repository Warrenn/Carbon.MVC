using System;
using System.Collections.Generic;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Constants;
using CarbonKnown.DAL.Models.Waste;

namespace CarbonKnown.Calculation.Waste
{
    [Calculation(
        "Waste",
        CarbonKnown.DAL.Models.Constants.Calculation.Waste,
        ConsumptionType = ConsumptionType.Waste,
        Activities = typeof (WasteActivityId),
        Factors = typeof (CarbonKnown.DAL.Models.Constants.Factors.Waste))]
    public class WasteCalculation : CalculationBase<WasteData>
    {
        public WasteCalculation(ICalculationDataContext context)
            : base(context)
        {
        }

        public static IDictionary<WasteType, Guid> FactorMapping =
            new SortedDictionary<WasteType, Guid>
                {
                    {WasteType.RecycledWaste, CarbonKnown.DAL.Models.Constants.Factors.Waste.RecylcedWaste},
                    {WasteType.WasteToLandFill, CarbonKnown.DAL.Models.Constants.Factors.Waste.WasteToLandfill}
                };

        public static IDictionary<WasteType, Guid> ActivityMapping =
            new SortedDictionary<WasteType, Guid>
                {
                    {WasteType.RecycledWaste, WasteActivityId.RecylcedWasteId},
                    {WasteType.WasteToLandFill, WasteActivityId.WasteToLandfillId}
                };

        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData,
                                                            WasteData entry)
        {
            var units = (decimal) dailyData.UnitsPerDay;
            var wasteType = (WasteType) entry.WasteType;
            var factorId = FactorMapping[wasteType];
            var factorValue = GetFactorValue(factorId, effectiveDate);
            var emissions = units*factorValue;
            var calculationDate = Context.CalculationDateForFactorId(factorId);
            return new CalculationResult
                {
                    CalculationDate = calculationDate,
                    ActivityGroupId = ActivityMapping[wasteType],
                    Emissions = emissions
                };
        }
    }
}
