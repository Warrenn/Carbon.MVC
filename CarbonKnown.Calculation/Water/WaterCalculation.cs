using System;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Constants;
using CarbonKnown.DAL.Models.Water;

namespace CarbonKnown.Calculation.Water
{
    [Calculation(
        "Water",
        CarbonKnown.DAL.Models.Constants.Calculation.Water,
        ConsumptionType = ConsumptionType.Water,
        Activity = Activity.Water,
        Factor = CarbonKnown.DAL.Models.Constants.Factors.Water)]
    public class WaterCalculation : CalculationBase<WaterData>
    {
        public WaterCalculation(ICalculationDataContext context)
            : base(context)
        {
        }

        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData,
                                                            WaterData entry)
        {
            var calculationDate = DateTime.Today;
            return new CalculationResult
                {
                    CalculationDate = calculationDate,
                    ActivityGroupId = Activity.WaterId,
                    Emissions = 0
                };
        }
    }
}
