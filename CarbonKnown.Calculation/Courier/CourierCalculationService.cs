using System;
using System.Collections.Generic;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.Calculation.Properties;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Constants;
using CarbonKnown.DAL.Models.Courier;

namespace CarbonKnown.Calculation.Courier
{
    public class CourierCalculationService
    {
        private readonly ICalculationDataContext context;
        public const int RoadDistance = 301;
        public const int DomesticDistance = 462;
        public const int LongHaulDistance = 3700;

        public static readonly IDictionary<DistanceType, Guid>
            FactorMapping = new SortedDictionary<DistanceType, Guid>
                {
                    {DistanceType.Domestic, CarbonKnown.DAL.Models.Constants.Factors.Courier.Domestic},
                    {DistanceType.LongHaul, CarbonKnown.DAL.Models.Constants.Factors.Courier.LongHaul},
                    {DistanceType.Road, CarbonKnown.DAL.Models.Constants.Factors.Courier.Road},
                    {DistanceType.ShortHaul, CarbonKnown.DAL.Models.Constants.Factors.Courier.LongHaul}
                };

        public static readonly IDictionary<DistanceType, Guid>
            ActivityMapping = new SortedDictionary<DistanceType, Guid>
                {
                    {DistanceType.Domestic, CourierActivityId.DomesticId},
                    {DistanceType.LongHaul, CourierActivityId.LongHaulId},
                    {DistanceType.Road, CourierActivityId.RoadId},
                    {DistanceType.ShortHaul, CourierActivityId.LongHaulId}
                };


        public static DistanceType GetDistanceType(decimal distance, ServiceType serviceType)
        {
            if (serviceType == ServiceType.Economy)
            {
                return DistanceType.Road;
            }
            if (distance < RoadDistance)
            {
                return DistanceType.Road;
            }
            if ((distance >= RoadDistance) &&
                (distance < DomesticDistance))
            {
                return DistanceType.Domestic;
            }
            if (distance >= DomesticDistance &&
                distance < LongHaulDistance)
            {
                return DistanceType.ShortHaul;
            }
            return DistanceType.LongHaul;
        }

        public CourierCalculationService(ICalculationDataContext context)
        {
            this.context = context;
        }

        public CalculationResult CalculateEmission(DateTime effectiveDate, decimal distance, ServiceType serviceType,
                                                   decimal chargeMass,bool reversal)
        {
            var distanceType = GetDistanceType(distance, serviceType);
            var factorId = FactorMapping[distanceType];
            var factorValue = context.FactorValue(effectiveDate, factorId);
            if (factorValue == null)
            {
                var message = string.Format(Resources.FactorValueNotFound, factorId, effectiveDate);
                throw new NullReferenceException(message);
            }
            var emissions = distance*(chargeMass/1000)*factorValue;
            if (reversal)
            {
                emissions = emissions*-1;
            }
            var calculationDate = context.CalculationDateForFactorId(factorId);

            return new CalculationResult
                {
                    CalculationDate = calculationDate,
                    ActivityGroupId = ActivityMapping[distanceType],
                    Emissions = emissions
                };
        }
    }
}
