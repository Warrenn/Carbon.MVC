using System;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Constants;
using CarbonKnown.DAL.Models.Courier;

namespace CarbonKnown.Calculation.Courier
{
    [Calculation(
        "Courier",
        CarbonKnown.DAL.Models.Constants.Calculation.Courier,
        ConsumptionType = ConsumptionType.Courier,
        Activities = typeof (CourierActivityId),
        Factors = typeof (CarbonKnown.DAL.Models.Constants.Factors.Courier))]
    public class CourierCalculation : CalculationBase<CourierData>
    {
        public CourierCalculation(ICalculationDataContext context)
            : base(context)
        {
        }

        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData,
                                                            CourierData entry)
        {
            var distance = (decimal) dailyData.UnitsPerDay;
            var travelClass = (ServiceType) entry.ServiceType;
            var chargeMass = (decimal) entry.ChargeMass;
            var service = new CourierCalculationService(Context);
            return service.CalculateEmission(effectiveDate, distance, travelClass, chargeMass,false);
        }
    }
}
