using System;
using System.Collections.Generic;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Constants;
using CarbonKnown.DAL.Models.Fuel;
using FactorIds = CarbonKnown.DAL.Models.Constants.Factors;

namespace CarbonKnown.Calculation.Fuel
{
    [Calculation(
        "Fuel",
        CarbonKnown.DAL.Models.Constants.Calculation.Fuel,
        ConsumptionType = ConsumptionType.Other,
        Activities = typeof (FuelActivityId),
        Factors = typeof (FactorIds.Fuel))]
    public class FuelCalculation : CalculationBase<FuelData>
    {
        public FuelCalculation(ICalculationDataContext context)
            : base(context)
        {
        }

        public static IDictionary<FuelType, IDictionary<UnitOfMeasure, Guid>> FactorMapping =
            new SortedDictionary<FuelType, IDictionary<UnitOfMeasure, Guid>>
                {
                    {
                        FuelType.AviationFuel, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FactorIds.Fuel.AviationFuelTonnes},
                                {UnitOfMeasure.Litres, FactorIds.Fuel.AviationFuelLitres},
                                {UnitOfMeasure.Tonnes, FactorIds.Fuel.AviationFuelTonnes},
                            }
                    },
                    {
                        FuelType.CoalIndustrial, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FactorIds.Fuel.CoalIndustrial},
                                {UnitOfMeasure.Litres, FactorIds.Fuel.CoalIndustrial},
                                {UnitOfMeasure.Tonnes, FactorIds.Fuel.CoalIndustrial},
                            }
                    },
                    {
                        FuelType.CoalDomestic, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FactorIds.Fuel.CoalDomestic},
                                {UnitOfMeasure.Litres, FactorIds.Fuel.CoalDomestic},
                                {UnitOfMeasure.Tonnes, FactorIds.Fuel.CoalDomestic},
                            }
                    },
                    {
                        FuelType.LPG, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FactorIds.Fuel.LPG},
                                {UnitOfMeasure.Litres, FactorIds.Fuel.LPG},
                                {UnitOfMeasure.Tonnes, FactorIds.Fuel.LPG},
                            }
                    },
                    {
                        FuelType.Diesel, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FactorIds.Fuel.Diesel},
                                {UnitOfMeasure.Litres, FactorIds.Fuel.Diesel},
                                {UnitOfMeasure.Tonnes, FactorIds.Fuel.Diesel},
                            }
                    },
                    {
                        FuelType.Petrol, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FactorIds.Fuel.Petrol},
                                {UnitOfMeasure.Litres, FactorIds.Fuel.Petrol},
                                {UnitOfMeasure.Tonnes, FactorIds.Fuel.Petrol},
                            }
                    },
                    {
                        FuelType.MarineFuelOil, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.MarineFuelOilTonnesId},
                                {UnitOfMeasure.Litres, FuelActivityId.MarineFuelOilTonnesId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.MarineFuelOilTonnesId}
                            }
                    },
                    {
                        FuelType.HeavyFuelOil, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.HeavyFuelOilId},
                                {UnitOfMeasure.Litres, FuelActivityId.HeavyFuelOilId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.HeavyFuelOilId}
                            }
                    },
                     {
                        FuelType.Paraffin, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.ParaffinId},
                                {UnitOfMeasure.Litres, FuelActivityId.ParaffinId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.ParaffinId}
                            }
                    },
                    {
                        FuelType.LNGkWH, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.LNGKWHId},
                                {UnitOfMeasure.Litres, FuelActivityId.LNGKWHId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.LNGKWHId}
                            }
                    },
                    {
                        FuelType.LNGlitres, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.LNGLitresId},
                                {UnitOfMeasure.Litres, FuelActivityId.LNGLitresId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.LNGLitresId}
                            }
                    },
                    {
                        FuelType.LPGGigajoule, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.LPGGigajoulesId},
                                {UnitOfMeasure.Litres, FuelActivityId.LPGGigajoulesId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.LPGGigajoulesId},
                                {UnitOfMeasure.Gigajoules, FuelActivityId.LPGGigajoulesId}
                            }
                    },
                };

        public static IDictionary<FuelType, IDictionary<UnitOfMeasure, Guid>> ActivityMapping =
            new SortedDictionary<FuelType, IDictionary<UnitOfMeasure, Guid>>
                {
                    {
                        FuelType.AviationFuel, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.AviationFuelTonnesId},
                                {UnitOfMeasure.Litres, FuelActivityId.AviationFuelLitresId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.AviationFuelTonnesId},
                            }
                    },
                    {
                        FuelType.CoalIndustrial, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.CoalIndustrialId},
                                {UnitOfMeasure.Litres, FuelActivityId.CoalIndustrialId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.CoalIndustrialId}
                            }
                    },
                    {
                        FuelType.CoalDomestic, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.CoalDomesticId},
                                {UnitOfMeasure.Litres, FuelActivityId.CoalDomesticId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.CoalDomesticId}
                            }
                    },
                    {
                        FuelType.LPG, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.LPGId},
                                {UnitOfMeasure.Litres, FuelActivityId.LPGId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.LPGId},
                            }
                    },
                    {
                        FuelType.Diesel, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.DieselId},
                                {UnitOfMeasure.Litres, FuelActivityId.DieselId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.DieselId}
                            }
                    },
                    {
                        FuelType.Petrol, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.PetrolId},
                                {UnitOfMeasure.Litres, FuelActivityId.PetrolId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.PetrolId}
                            }
                    },
                      {
                        FuelType.MarineFuelOil, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.MarineFuelOilTonnesId},
                                {UnitOfMeasure.Litres, FuelActivityId.MarineFuelOilTonnesId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.MarineFuelOilTonnesId}
                            }
                    },
                    {
                        FuelType.HeavyFuelOil, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.HeavyFuelOilId},
                                {UnitOfMeasure.Litres, FuelActivityId.HeavyFuelOilId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.HeavyFuelOilId}
                            }
                    },
                     {
                        FuelType.Paraffin, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.ParaffinId},
                                {UnitOfMeasure.Litres, FuelActivityId.ParaffinId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.ParaffinId}
                            }
                    },
                    {
                        FuelType.LNGkWH, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.LNGKWHId},
                                {UnitOfMeasure.Litres, FuelActivityId.LNGKWHId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.LNGKWHId}
                            }
                    },
                    {
                        FuelType.LNGlitres, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.LNGLitresId},
                                {UnitOfMeasure.Litres, FuelActivityId.LNGLitresId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.LNGLitresId}
                            }
                    },
                    {
                        FuelType.LPGGigajoule, new SortedDictionary<UnitOfMeasure, Guid>
                            {
                                {UnitOfMeasure.KiloWattHours, FuelActivityId.LPGGigajoulesId},
                                {UnitOfMeasure.Litres, FuelActivityId.LPGGigajoulesId},
                                {UnitOfMeasure.Tonnes, FuelActivityId.LPGGigajoulesId},
                                {UnitOfMeasure.Gigajoules, FuelActivityId.LPGGigajoulesId}
                            }
                    },
                };

        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData, FuelData entry)
        {
            var units = (decimal) dailyData.UnitsPerDay;
            var fuelType = (FuelType) entry.FuelType;
            var unitOfMeasure = (UnitOfMeasure) entry.UOM;

            var factorId = FactorMapping[fuelType][unitOfMeasure];
            var factorValue = GetFactorValue(factorId, effectiveDate);
            var emissions = units*factorValue;
            var calculationDate = Context.CalculationDateForFactorId(factorId);
            return new CalculationResult
                {
                    CalculationDate = calculationDate,
                    ActivityGroupId = ActivityMapping[fuelType][unitOfMeasure],
                    Emissions = emissions
                };
        }
    }
}
