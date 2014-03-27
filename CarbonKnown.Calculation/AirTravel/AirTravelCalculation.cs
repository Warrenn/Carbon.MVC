using System;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.AirTravel;
using CalculationIds = CarbonKnown.DAL.Models.Constants.Calculation;
using ActivityIds = CarbonKnown.DAL.Models.Constants.AirTravelActivityId;
using FactorsIds = CarbonKnown.DAL.Models.Constants.Factors;

namespace CarbonKnown.Calculation.AirTravel
{
    [Calculation(
        "Air Travel",
        CalculationIds.AirTravel,
        ConsumptionType = ConsumptionType.AirTravel,
        Activities = typeof (ActivityIds),
        Factors = typeof (FactorsIds.AirTravel))]
    public class AirTravelCalculation : CalculationBase<AirTravelData>
    {
        public AirTravelCalculation(ICalculationDataContext context)
            : base(context)
        {
        }
        
        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData, AirTravelData entry)
        {
            var distance = (decimal)dailyData.UnitsPerDay;
            var travelClass = (TravelClass)entry.TravelClass;
            var service = new AirTravelCalculationService(Context);
            return service.CalculateEmission(effectiveDate, distance, travelClass, false);
        }
    }
}
