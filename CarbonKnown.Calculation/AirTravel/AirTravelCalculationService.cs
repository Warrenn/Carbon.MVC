using System;
using System.Collections.Generic;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.Calculation.Properties;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.AirTravel;
using CarbonKnown.DAL.Models.Constants;
using FactorIds = CarbonKnown.DAL.Models.Constants.Factors;

namespace CarbonKnown.Calculation.AirTravel
{
    public class AirTravelCalculationService
    {
        private readonly ICalculationDataContext context;
        public const decimal UpliftFactor = 1.09m;
        public const int DomesticDistance = 462;
        public const int LongHaulDistance = 3700;

        public static readonly IDictionary<DistanceType, IDictionary<TravelClass, Guid>>
            FactorMapping = new SortedDictionary<DistanceType, IDictionary<TravelClass, Guid>>
                {
                    {
                        DistanceType.Domestic, new SortedDictionary<TravelClass, Guid>
                            {
                                {TravelClass.Average, FactorIds.AirTravel.DomesticAverage},
                                {TravelClass.Business, FactorIds.AirTravel.DomesticAverage},
                                {TravelClass.Economy, FactorIds.AirTravel.DomesticAverage},
                                {TravelClass.FirstClass, FactorIds.AirTravel.DomesticAverage}
                            }
                    },
                    {
                        DistanceType.LongHaul, new SortedDictionary<TravelClass, Guid>
                            {
                                {TravelClass.Average, FactorIds.AirTravel.LongHaulAverage},
                                {TravelClass.Business, FactorIds.AirTravel.LongHaulBusinessClass},
                                {TravelClass.Economy, FactorIds.AirTravel.LongHaulEconomyClass},
                                {TravelClass.FirstClass, FactorIds.AirTravel.LongHaulFirstClass}
                            }
                    },
                    {
                        DistanceType.ShortHaul, new SortedDictionary<TravelClass, Guid>
                            {
                                {TravelClass.Average, FactorIds.AirTravel.ShortHaulAverage},
                                {TravelClass.Business, FactorIds.AirTravel.ShortHaulBusinessClass},
                                {TravelClass.Economy, FactorIds.AirTravel.ShortHaulEconomyClass},
                                {TravelClass.FirstClass, FactorIds.AirTravel.ShortHaulBusinessClass}
                            }
                    }
                };

        public static readonly IDictionary<DistanceType, IDictionary<TravelClass, Guid>>
            ActivityMapping = new SortedDictionary<DistanceType, IDictionary<TravelClass, Guid>>
                {
                    {
                        DistanceType.Domestic, new SortedDictionary<TravelClass, Guid>
                            {
                                {TravelClass.Average, AirTravelActivityId.DomesticAverageId},
                                {TravelClass.Business, AirTravelActivityId.DomesticBusinessClassId},
                                {TravelClass.Economy, AirTravelActivityId.DomesticEconomyClassId},
                                {TravelClass.FirstClass, AirTravelActivityId.DomesticFirstClassId}
                            }
                    },
                    {
                        DistanceType.LongHaul, new SortedDictionary<TravelClass, Guid>
                            {
                                {TravelClass.Average, AirTravelActivityId.LongHaulAverageId},
                                {TravelClass.Business, AirTravelActivityId.LongHaulBusinessClassId},
                                {TravelClass.Economy, AirTravelActivityId.LongHaulEconomyClassId},
                                {TravelClass.FirstClass, AirTravelActivityId.LongHaulFirstClassId}
                            }
                    },
                    {
                        DistanceType.ShortHaul, new SortedDictionary<TravelClass, Guid>
                            {
                                {TravelClass.Average, AirTravelActivityId.ShortHaulAverageId},
                                {TravelClass.Business, AirTravelActivityId.ShortHaulBusinessClassId},
                                {TravelClass.Economy, AirTravelActivityId.ShortHaulEconomyClassId},
                                {TravelClass.FirstClass, AirTravelActivityId.ShortHaulFirstClassId}
                            }
                    }
                };


        public static DistanceType GetDistanceType(decimal distance)
        {
            if ((distance < DomesticDistance))
            {
                return DistanceType.Domestic;
            }
            if ((distance >= DomesticDistance) && (distance < LongHaulDistance))
            {
                return DistanceType.ShortHaul;
            }
            return DistanceType.LongHaul;
        }


        public AirTravelCalculationService(ICalculationDataContext context)
        {
            this.context = context;
        }

        public CalculationResult CalculateEmission(DateTime effectiveDate, decimal distance, TravelClass travelClass,
                                                   bool reversal)
        {
            var distanceType = GetDistanceType(distance);
            var factorId = FactorMapping[distanceType][travelClass];
            var factorValue = context.FactorValue(effectiveDate, factorId);
            if (factorValue == null)
            {
                var message = string.Format(Resources.FactorValueNotFound, factorId, effectiveDate);
                throw new NullReferenceException(message);
            }
            var emissions = (factorValue*distance)*UpliftFactor;
            if (reversal)
            {
                emissions = emissions*-1;
            }
            var calculationDate = context.CalculationDateForFactorId(factorId);
            return new CalculationResult
                {
                    CalculationDate = calculationDate,
                    ActivityGroupId = ActivityMapping[distanceType][travelClass],
                    Emissions = emissions
                };
        }

    }
}
