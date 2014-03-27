using System;
using CarbonKnown.Calculation.AirTravel;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.Calculation.Properties;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.AirTravel;
using CarbonKnown.DAL.Models.Constants;

namespace CarbonKnown.Calculation.AirTravelRoute
{
    [Calculation(
        "Air Travel (Route)",
        CarbonKnown.DAL.Models.Constants.Calculation.AirTravelRoute,
        ConsumptionType = ConsumptionType.AirTravel,
        Activities = typeof (AirTravelActivityId),
        Factors = typeof (CarbonKnown.DAL.Models.Constants.Factors.AirTravel))]
    public class AirTravelRouteCalculation : CalculationBase<AirTravelRouteData>
    {
        public AirTravelRouteCalculation(ICalculationDataContext context)
            : base(context)
        {
        }

        public override DailyData CalculateDailyData(AirTravelRouteData entry)
        {
            var distance = Context.AirRouteDistance(entry.FromCode, entry.ToCode);
            if (distance == null)
            {
                var message = string.Format(Resources.RouteDistanceMissing, entry.FromCode, entry.ToCode);
                throw new NullReferenceException(message);
            }
            entry.Units = distance;
            return base.CalculateDailyData(entry);
        }

        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData,
                                                            AirTravelRouteData entry)
        {
            var distance = (decimal) dailyData.UnitsPerDay;
            var travelClass = (TravelClass) entry.TravelClass;
            var service = new AirTravelCalculationService(Context);
            return service.CalculateEmission(effectiveDate, distance, travelClass, entry.Reversal);
        }
    }
}
