using System;
using System.Collections.Generic;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.CarHire;
using CarbonKnown.DAL.Models.Constants;

namespace CarbonKnown.Calculation.CarHire
{
    [Calculation(
        "Car Hire",
        CarbonKnown.DAL.Models.Constants.Calculation.CarHire,
        ConsumptionType = ConsumptionType.CarHire,
        Activities = typeof (CarHireActivityId),
        Factors = typeof (CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill))]
    public class CarHireCalculation : CalculationBase<CarHireData>
    {
        public static readonly IDictionary<CarGroupBill, Guid> FactorMapping =
            new SortedDictionary<CarGroupBill, Guid>
                {
                    {CarGroupBill.A, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.A_Economy_1_4},
                    {CarGroupBill.B, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.B_Compact_1_4_2_l_Petrol},
                    {CarGroupBill.C, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.C_Intermediate_1_6_l_Petrol},
                    {CarGroupBill.D, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.D_Intermediate_1_6_l_Petrol},
                    {CarGroupBill.E, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.E_Standard_2_0_2_4_l_Petrol},
                    {CarGroupBill.F, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.F_Full_Size_1_8_2_l_Petrol},
                    {CarGroupBill.G, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.G_Premium_1_8_2_l_Petrol},
                    {CarGroupBill.H, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.H_Hybrid_HYB},
                    {CarGroupBill.I, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.I_Compact_Petrol},
                    {CarGroupBill.J, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.J_Luxury_2_3_2_5_l_Petrol},
                    {
                        CarGroupBill.K,
                        CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.K_Speciality_SUV_2_4_2_5_l_Petrol
                    },
                    {CarGroupBill.L, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.L_Speciality_Leisure_4X4_3_l},
                    {CarGroupBill.M, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.M_Economy_1_1_1_4_Petrol},
                    {CarGroupBill.N, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.N_Speciality_People_Carrier},
                    {CarGroupBill.O, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.O_Full_Size_1_8_2_l_Petrol},
                    {CarGroupBill.P, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.P_Full_Size_1_4_l_Petrol},
                    {CarGroupBill.AveragePetrol, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.Average_Petrol},
                    {
                        CarGroupBill.GreaterThan2LPetrol,
                        CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.Greater_Than_2_Litres_Petrol
                    },
                    {
                        CarGroupBill.LessThan14LPetrol,
                        CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.Less_Than_1_4_Litres_Petrol
                    },
                    {
                        CarGroupBill.Diesel17To2L,
                        CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.Diesel17To2L
                    },
                    {
                        CarGroupBill.LessThan17Diesel,
                        CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.LessThan17Diesel
                    },
                    {
                        CarGroupBill.GreaterThan2LDiesel,
                        CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.GreaterThan2LDiesel
                    },
                    {
                        CarGroupBill.GreaterThan500Cc,
                        CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.GreaterThan500Cc
                    },
                    {
                        CarGroupBill.AverageDiesel,
                        CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.AverageDiesel
                    },
                };

        public static readonly IDictionary<CarGroupBill, Guid> ActivityMapping =
            new SortedDictionary<CarGroupBill, Guid>
                {
                    {CarGroupBill.A, CarHireActivityId.A_Economy_1_4Id},
                    {CarGroupBill.B, CarHireActivityId.B_Compact_1_4_2_l_PetrolId},
                    {CarGroupBill.C, CarHireActivityId.C_Intermediate_1_6_l_PetrolId},
                    {CarGroupBill.D, CarHireActivityId.D_Intermediate_1_6_l_PetrolId},
                    {CarGroupBill.E, CarHireActivityId.E_Standard_2_0_2_4_l_PetrolId},
                    {CarGroupBill.F, CarHireActivityId.F_Full_Size_1_8_2_l_PetrolId},
                    {CarGroupBill.G, CarHireActivityId.G_Premium_1_8_2_l_PetrolId},
                    {CarGroupBill.H, CarHireActivityId.H_Hybrid_HYBId},
                    {CarGroupBill.I, CarHireActivityId.I_Compact_PetrolId},
                    {CarGroupBill.J, CarHireActivityId.J_Luxury_2_3_2_5_l_PetrolId},
                    {CarGroupBill.K, CarHireActivityId.K_Speciality_SUV_2_4_2_5_l_PetrolId},
                    {CarGroupBill.L, CarHireActivityId.L_Speciality_Leisure_4X4_3_lId},
                    {CarGroupBill.M, CarHireActivityId.M_Economy_1_1_1_4_PetrolId},
                    {CarGroupBill.N, CarHireActivityId.N_Speciality_People_CarrierId},
                    {CarGroupBill.O, CarHireActivityId.O_Full_Size_1_8_2_l_PetrolId},
                    {CarGroupBill.P, CarHireActivityId.P_Full_Size_1_4_l_PetrolId},
                    {CarGroupBill.AveragePetrol, CarHireActivityId.Average_PetrolId},
                    {CarGroupBill.GreaterThan2LPetrol, CarHireActivityId.Greater_Than_2_Litres_PetrolId},
                    {CarGroupBill.LessThan14LPetrol, CarHireActivityId.Less_Than_1_4_Litres_PetrolId},
                    {CarGroupBill.Diesel17To2L, CarHireActivityId.Diesel17To2LId},
                    {CarGroupBill.LessThan17Diesel, CarHireActivityId.LessThan17DieselId},
                    {CarGroupBill.GreaterThan2LDiesel, CarHireActivityId.GreaterThan2LDieselId},
                    {CarGroupBill.GreaterThan500Cc, CarHireActivityId.GreaterThan500CcId},
                    {CarGroupBill.AverageDiesel, CarHireActivityId.AverageDieselId}
                };

        public CarHireCalculation(ICalculationDataContext context)
            : base(context)
        {
        }

        public override CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData,
                                                            CarHireData entry)
        {
            var factorId = FactorMapping[(CarGroupBill)entry.CarGroupBill];
            var factor = GetFactorValue(factorId, effectiveDate);
            var emissions = factor*dailyData.UnitsPerDay;
            var calculationDate = Context.CalculationDateForFactorId(factorId);
            return new CalculationResult
                {
                    CalculationDate = calculationDate,
                    Emissions = emissions,
                    ActivityGroupId = ActivityMapping[entry.CarGroupBill.Value]
                };
        }
    }
}
