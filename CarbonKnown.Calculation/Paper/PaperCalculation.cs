using System;
using System.IO;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Constants;
using CarbonKnown.DAL.Models.Paper;
using PaperFactors = CarbonKnown.DAL.Models.Constants.Factors.Paper;

namespace CarbonKnown.Calculation.Paper
{
    [Calculation(
        "Paper",
        CarbonKnown.DAL.Models.Constants.Calculation.Paper,
        ConsumptionType = ConsumptionType.Paper,
        Activities = typeof (PaperActivityId),
        Factors = typeof (PaperFactors))]
    public class PaperCalculation : CalculationBase<PaperData>
    {
        public const int ReamsPerTonne = 400;
        public const decimal PolicyPaperMondiToSappiRatio = 0.4M;

        public PaperCalculation(ICalculationDataContext context)
            : base(context)
        {
        }

        private decimal MondiEmissions(decimal units, DateTime calculationDate)
        {
            var directFactorValue = GetFactorValue(PaperFactors.MondiA4Direct, calculationDate);
            var indirectFactorValue = GetFactorValue(PaperFactors.MondiA4Indirect, calculationDate);
            var emissions = (units*directFactorValue) + (units*indirectFactorValue);
            return emissions;
        }

        private decimal SappiEmissions(decimal units, DateTime calculationDate)
        {
            var directFactorValue = GetFactorValue(PaperFactors.SappiA4Direct, calculationDate);
            var indirectFactorValue = GetFactorValue(PaperFactors.SappiA4Indirect, calculationDate);
            var emissions = (units * directFactorValue) + (units * indirectFactorValue);
            return emissions;
        }

        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData,
                                                            PaperData entry)
        {
            var units =  (decimal)dailyData.UnitsPerDay;
            var paperType = (PaperType) entry.PaperType;
            var paperUom = (PaperUom) entry.PaperUom;

            if (((paperType == PaperType.MondiA3) ||
                (paperType == PaperType.SappiA3))&&
                (paperUom == PaperUom.Reams))
            {
                units = units*2;
            }

            if (paperUom == PaperUom.Reams)
            {
                units = units/ReamsPerTonne;
            }

            if ((paperType == PaperType.MondiA3) ||
                (paperType == PaperType.MondiA4))
            {
                var emissions = MondiEmissions(units, effectiveDate);
                var calculationDate = Context.CalculationDateForFactorId(PaperFactors.MondiA4Direct);
                return new CalculationResult
                    {
                        CalculationDate = calculationDate,
                        ActivityGroupId = PaperActivityId.MondiId,
                        Emissions = emissions
                    };
            }
            if ((paperType == PaperType.SappiA3) ||
                (paperType == PaperType.SappiA4))
            {
                var emissions = SappiEmissions(units, effectiveDate);
                var calculationDate = Context.CalculationDateForFactorId(PaperFactors.SappiA4Direct);
                return new CalculationResult
                    {
                        CalculationDate = calculationDate,
                        ActivityGroupId = PaperActivityId.SappiId,
                        Emissions = emissions
                    };
            }
            if (entry.PaperType == PaperType.PolicyPaper)
            {
                var mondiQuantity = units*PolicyPaperMondiToSappiRatio;
                var mondiEmissions = MondiEmissions(mondiQuantity, effectiveDate);
                var sappiQuantity = units*(1 - PolicyPaperMondiToSappiRatio);
                var sappiEmissions = SappiEmissions(sappiQuantity, effectiveDate);
                var emissions = sappiEmissions + mondiEmissions;
                var calculationDate = Context.CalculationDateForFactorId(PaperFactors.MondiA4Direct);
                return new CalculationResult
                    {
                        CalculationDate = calculationDate,
                        ActivityGroupId = PaperActivityId.PolicyId,
                        Emissions = emissions
                    };
            }
            throw new InvalidDataException("entry.PaperType");
        }
    }
}
