using System;
using System.Collections.Generic;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Commuting;
using ActivityIds = CarbonKnown.DAL.Models.Constants.ComuttingActivityId;
using FactorIds = CarbonKnown.DAL.Models.Constants.Factors;

namespace CarbonKnown.Calculation.Commuting
{
    [Calculation(
        "Commuting",
        CarbonKnown.DAL.Models.Constants.Calculation.Commuting,
        ConsumptionType = ConsumptionType.Other,
        Activities = typeof(ActivityIds),
        Factors = typeof(FactorIds.Comutting))]
    public class CommutingCalculation : CalculationBase<CommutingData>
    {
        private static readonly IDictionary<CommutingType, Guid> ComutingFactors = new SortedDictionary<CommutingType, Guid>
            {
                {CommutingType.EmployeeAverage, FactorIds.Comutting.EmployeeAverage},
                {CommutingType.Train, FactorIds.Comutting.Train},
                {CommutingType.MiniBusTaxi, FactorIds.Comutting.MiniBusTaxi},
                {CommutingType.Bus, FactorIds.Comutting.Bus}
            };

        private static readonly IDictionary<CommutingType, Guid> ComutingActivities = new SortedDictionary
            <CommutingType, Guid>
            {
                {CommutingType.EmployeeAverage, ActivityIds.EmployeeAverageId},
                {CommutingType.Train, ActivityIds.TrainId},
                {CommutingType.MiniBusTaxi, ActivityIds.MiniBusTaxiId},
                {CommutingType.Bus, ActivityIds.BusId}
            };

        public CommutingCalculation(ICalculationDataContext context)
            : base(context)
        {
        }

        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData, CommutingData entry)
        {
            var commutingType = (CommutingType) entry.CommutingType;
            var factorId = ComutingFactors[commutingType];
            var factorValue = GetFactorValue(factorId, effectiveDate);
            var emissions = dailyData.UnitsPerDay*factorValue;
            var calculationDate = Context.CalculationDateForFactorId(factorId);
            return new CalculationResult
            {
                CalculationDate = calculationDate,
                ActivityGroupId = ComutingActivities[commutingType],
                    Emissions = emissions
                };
        }
    }
}
