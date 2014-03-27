 
 

using System;
using System.Data.Entity.Migrations;
using CarbonKnown.Factors.DAL;
using CarbonKnown.Factors.Models;
namespace CarbonKnown.Factors.Migrations
{
    internal  sealed partial class Configuration
	{
        private static void GeneratedSeed(DataContext context)
        {
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("fe6e13b0-575d-4822-ac04-f100cf01a6ee"),
					FactorName = "A_Economy_1_4"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("692ec00a-315b-49bb-8aec-4d86422c8ccb"),
					FactorName = "B_Compact_1_4_2_l_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("3afd8119-5090-4863-a474-6f4cec6a2821"),
					FactorName = "C_Intermediate_1_6_l_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("36df1d51-4b1f-4552-ac83-6c6cec4f6f71"),
					FactorName = "D_Intermediate_1_6_l_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("3195f930-cdf5-482a-b393-7589236467c1"),
					FactorName = "E_Standard_2_0_2_4_l_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("e8bf433f-214d-4d89-8c02-a2794a6f1938"),
					FactorName = "F_Full_Size_1_8_2_l_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("269f3ef3-e74d-4dcc-b5a7-3e5e3f05a189"),
					FactorName = "G_Premium_1_8_2_l_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("3eb227d9-b369-47eb-9a0a-bdde5e44a2f8"),
					FactorName = "H_Hybrid_HYB"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("2f1cfef5-4801-4afd-80bf-edc1642fb00b"),
					FactorName = "I_Compact_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("c3e1ceef-51e8-435f-8ab7-77d0b759bd30"),
					FactorName = "J_Luxury_2_3_2_5_l_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("585dce22-93b4-4da4-aa8b-43c5a26ef4b1"),
					FactorName = "K_Speciality_SUV_2_4_2_5_l_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("5f7d5b47-79a0-4838-a1f5-1b7e353eb7e0"),
					FactorName = "L_Speciality_Leisure_4X4_3_l"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("6fdfe1fc-8d82-47c7-970c-fcd7bc174e7c"),
					FactorName = "M_Economy_1_1_1_4_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("2944b143-a0a1-4755-9d62-b7e6b08fc149"),
					FactorName = "N_Speciality_People_Carrier"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("84c39f06-3c18-406a-9fdb-f143d4333e3b"),
					FactorName = "O_Full_Size_1_8_2_l_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("06909133-805e-4271-9fc2-6be805588a28"),
					FactorName = "P_Full_Size_1_4_l_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("5a15585c-f779-433b-853b-e037c1c54b80"),
					FactorName = "Average_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("4c639165-9531-4a31-b32e-fd8e2b43915e"),
					FactorName = "Greater_Than_2_Litres_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "CarGroupBill",
					FactorId = new Guid("3205ae00-929e-445f-a31c-7a48f95ccc55"),
					FactorName = "Less_Than_1_4_Litres_Petrol"
				});
            context.Factors.AddOrUpdate(
				new Factor
				{
					FactorGroup = "NonKyotoR22FreonFactors",
					FactorId = new Guid("585e0250-1396-464f-8476-12877913b6c9"),
					FactorName = "NonKyotoR22FreonId"
				});
        } 
    }
}