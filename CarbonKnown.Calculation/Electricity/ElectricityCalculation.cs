using System;
using System.Collections.Generic;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Constants;
using CarbonKnown.DAL.Models.Electricity;
using FactorIds = CarbonKnown.DAL.Models.Constants.Factors;

namespace CarbonKnown.Calculation.Electricity
{
    [Calculation(
        "Electricity",
        CarbonKnown.DAL.Models.Constants.Calculation.Electricity,
        ConsumptionType = ConsumptionType.Electricity,
        Activities = typeof(ElectricityActivityId),
        Factors = typeof(FactorIds.Electricity))]
    public class ElectricityCalculation : CalculationBase<ElectricityData>
    {
        public ElectricityCalculation(ICalculationDataContext context)
            : base(context)
        {
        }

        public static IDictionary<ElectricityType, Guid> ActivityMapping = new SortedDictionary<ElectricityType, Guid>
            {
                {ElectricityType.SouthAfricanNationalGrid, ElectricityActivityId.SouthAfricanNationalGridId},
                {ElectricityType.AngolaNationalGrid, ElectricityActivityId.AngolaNationalGridId},
                {ElectricityType.BotswanaNationalGrid, ElectricityActivityId.BotswanaNationalGridId},
                {ElectricityType.ZambiaNationalGrid, ElectricityActivityId.ZambiaNationalGridId},
                {ElectricityType.TanzaniaNationalGrid, ElectricityActivityId.TanzaniaNationalGridId},
                {ElectricityType.KenyaNationalGrid, ElectricityActivityId.KenyaNationalGridId},
                {ElectricityType.NigeriaNationalGrid, ElectricityActivityId.NigeriaNationalGridId},
                {ElectricityType.ZimbabweNationalGrid, ElectricityActivityId.ZimbabweNationalGridId},
                {ElectricityType.IsleOfManNationalGrid, ElectricityActivityId.IsleofManNationalGridId},
                {ElectricityType.UKNationalGrid, ElectricityActivityId.UKNationalGridId},
                {ElectricityType.MalawiNationalGrid, ElectricityActivityId.MalawiNationalGridId},
                {ElectricityType.SwazilandNationalGrid, ElectricityActivityId.SwazilandNationalGridId},
                {ElectricityType.PurchasedSteam, ElectricityActivityId.PurchasedSteamId}
            };

        public static IDictionary<ElectricityType, Guid> FactorMapping = new SortedDictionary<ElectricityType, Guid>
            {
                {ElectricityType.SouthAfricanNationalGrid, FactorIds.Electricity.SouthAfricanNationalGrid},
                {ElectricityType.AngolaNationalGrid, FactorIds.Electricity.AngolaNationalGrid},
                {ElectricityType.BotswanaNationalGrid, FactorIds.Electricity.BotswanaNationalGrid},
                {ElectricityType.ZambiaNationalGrid, FactorIds.Electricity.ZambiaNationalGrid},
                {ElectricityType.NamibiaNationalGrid, FactorIds.Electricity.NamibiaNationalGrid},
                {ElectricityType.TanzaniaNationalGrid, FactorIds.Electricity.TanzaniaNationalGrid},
                {ElectricityType.KenyaNationalGrid, FactorIds.Electricity.KenyaNationalGrid},
                {ElectricityType.NigeriaNationalGrid, FactorIds.Electricity.NigeriaNationalGrid},
                {ElectricityType.ZimbabweNationalGrid, FactorIds.Electricity.ZimbabweNationalGrid},
                {ElectricityType.IsleOfManNationalGrid, FactorIds.Electricity.IsleofManNationalGrid},
                {ElectricityType.UKNationalGrid, FactorIds.Electricity.UKNationalGrid},
                {ElectricityType.MalawiNationalGrid, FactorIds.Electricity.MalawiNationalGrid},
                {ElectricityType.SwazilandNationalGrid, FactorIds.Electricity.SwazilandNationalGrid}

            };

        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData, ElectricityData entry)
        {
            var electricityType = (ElectricityType) entry.ElectricityType;
            var factorId = FactorMapping[electricityType];
            var factorValue = GetFactorValue(factorId, effectiveDate);
            var emissions = dailyData.UnitsPerDay*factorValue;
            var calculationDate = Context.CalculationDateForFactorId(factorId);
            return new CalculationResult
            {
                CalculationDate = calculationDate,
                ActivityGroupId = ActivityMapping[electricityType],
                Emissions = emissions
            };
        }
    }
}
