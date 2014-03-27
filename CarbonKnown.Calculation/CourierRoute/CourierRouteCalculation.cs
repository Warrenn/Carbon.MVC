using System;
using CarbonKnown.Calculation.Courier;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.Calculation.Properties;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Constants;
using CarbonKnown.DAL.Models.Courier;

namespace CarbonKnown.Calculation.CourierRoute
{
    [Calculation(
        "Courier (Route)",
        CarbonKnown.DAL.Models.Constants.Calculation.CourierRoute,
        ConsumptionType = ConsumptionType.Courier,
        Activities = typeof (CourierActivityId),
        Factors = typeof (CarbonKnown.DAL.Models.Constants.Factors.Courier))]
    public class CourierRouteCalculation : CalculationBase<CourierRouteData>
    {
        public CourierRouteCalculation(ICalculationDataContext context)
            : base(context)
        {
        }

        public override DailyData CalculateDailyData(CourierRouteData entry)
        {
            var distance = Context.CourierRouteDistance(entry.FromCode, entry.ToCode);
            if (distance == null)
            {
                var message = string.Format(Resources.RouteDistanceMissing, entry.FromCode, entry.ToCode);
                throw new NullReferenceException(message);
            }
            entry.Units = distance;
            return base.CalculateDailyData(entry);
        }

        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData, CourierRouteData entry)
        {
            var distance = (decimal)dailyData.UnitsPerDay;
            var serviceType = (ServiceType)entry.ServiceType;
            var chargeMass = (decimal) entry.ChargeMass;
            var service = new CourierCalculationService(Context);
            return service.CalculateEmission(effectiveDate, distance, serviceType, chargeMass, entry.Reversal);
        }
    }
}
