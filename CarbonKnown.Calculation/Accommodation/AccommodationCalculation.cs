using System;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Accommodation;
using CarbonKnown.DAL.Models.Constants;

namespace CarbonKnown.Calculation.Accommodation
{
    [Calculation(
        "Hotel Nights",
        CarbonKnown.DAL.Models.Constants.Calculation.HotelNights,
        ConsumptionType = ConsumptionType.HotelNights,
        Activity = Activity.HotelNights,
        Factor = CarbonKnown.DAL.Models.Constants.Factors.Accommodation)]
    public class AccommodationCalculation : CalculationBase<AccommodationData>
    {
        public AccommodationCalculation(ICalculationDataContext context)
            : base(context)
        {
        }

        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData,
                                                            AccommodationData entry)
        {
            var factorValue = GetFactorValue(CarbonKnown.DAL.Models.Constants.Factors.AccommodationId, effectiveDate);
            var emissions = dailyData.UnitsPerDay*factorValue;
            var calculationDate =
                Context.CalculationDateForFactorId(CarbonKnown.DAL.Models.Constants.Factors.AccommodationId);
            return new CalculationResult
                {
                    CalculationDate = calculationDate,
                    ActivityGroupId = Activity.HotelNightsId,
                    Emissions = emissions
                };
        }
    }
}
