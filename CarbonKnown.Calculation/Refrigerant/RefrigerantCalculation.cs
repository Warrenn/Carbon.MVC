using System;
using System.Collections.Generic;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Constants;
using CarbonKnown.DAL.Models.Refrigerant;
using FactorIds = CarbonKnown.DAL.Models.Constants.Factors;

namespace CarbonKnown.Calculation.Refrigerant
{
    [Calculation(
        "Refrigerant",
        CarbonKnown.DAL.Models.Constants.Calculation.Refrigerant,
        ConsumptionType = ConsumptionType.Other,
        Activities = typeof (RefrigerantActivityId),
        Factors = typeof (FactorIds.Refrigerant))]
    public class RefrigerantCalculation : CalculationBase<RefrigerantData>
    {
        public RefrigerantCalculation(ICalculationDataContext context)
            : base(context)
        {
        }

        public static IDictionary<RefrigerantType, Guid> FactorMapping =
            new SortedDictionary<RefrigerantType, Guid>
                {
                    {RefrigerantType.R22Freon, FactorIds.Refrigerant.R22Freon},
                    {RefrigerantType.R22Refrigerant, FactorIds.Refrigerant.R22Refrigerant},
                    {RefrigerantType.Refrigerant134, FactorIds.Refrigerant.Refrigerant134},
                    {RefrigerantType.Refrigerant143A, FactorIds.Refrigerant.Refrigerant143A},
                    {RefrigerantType.HcFC134A, FactorIds.Refrigerant.HcFC134A},
                    {RefrigerantType.R404A, FactorIds.Refrigerant.R404A},
                    {RefrigerantType.R410, FactorIds.Refrigerant.R410},
                    {RefrigerantType.R410A, FactorIds.Refrigerant.R410A}
                };

        public static IDictionary<RefrigerantType, Guid> ActivityMapping =
            new SortedDictionary<RefrigerantType, Guid>
                {
                    {RefrigerantType.R22Freon, RefrigerantActivityId.R22FreonId},
                    {RefrigerantType.R22Refrigerant, RefrigerantActivityId.R22RefrigerantId},
                    {RefrigerantType.Refrigerant134, RefrigerantActivityId.Refrigerant134Id},
                    {RefrigerantType.Refrigerant143A, RefrigerantActivityId.Refrigerant143AId},
                    {RefrigerantType.HcFC134A, RefrigerantActivityId.HcFC134AId},
                    {RefrigerantType.R404A, RefrigerantActivityId.R404AId},
                    {RefrigerantType.R410, RefrigerantActivityId.R410Id},
                    {RefrigerantType.R410A, RefrigerantActivityId.R410AId}
                };

        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData, RefrigerantData entry)
        {
            var units = (decimal)dailyData.UnitsPerDay;
            var refrigerantType = (RefrigerantType) entry.RefrigerantType;

            var factorId = FactorMapping[refrigerantType];
            var factorValue = GetFactorValue(factorId, effectiveDate);
            var emissions = units * factorValue;
            var calculationDate = Context.CalculationDateForFactorId(factorId);
            return new CalculationResult
            {
                CalculationDate = calculationDate,
                ActivityGroupId = ActivityMapping[refrigerantType],
                Emissions = emissions
            };
        }
    }
}
