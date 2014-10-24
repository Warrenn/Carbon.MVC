using System;

namespace CarbonKnown.DAL.Models.Constants
{
    public static class Factors
    {
        public const string Accommodation = "70569D47-A4FA-4B8E-93BA-0543418CDCE8";
        public static readonly Guid AccommodationId = new Guid(Accommodation);
        public const string Water = "9B06348F-2E2E-4B91-9A2B-3A21D852D55B";
        public static readonly Guid WaterId = new Guid(Water);

        public static class CarGroupBill
        {
            public static readonly Guid A_Economy_1_4 = new Guid("FE6E13B0-575D-4822-AC04-F100CF01A6EE");
            public static readonly Guid B_Compact_1_4_2_l_Petrol = new Guid("692EC00A-315B-49BB-8AEC-4D86422C8CCB");
            public static readonly Guid C_Intermediate_1_6_l_Petrol = new Guid("3AFD8119-5090-4863-A474-6F4CEC6A2821");
            public static readonly Guid D_Intermediate_1_6_l_Petrol = new Guid("36DF1D51-4B1F-4552-AC83-6C6CEC4F6F71");
            public static readonly Guid E_Standard_2_0_2_4_l_Petrol = new Guid("3195F930-CDF5-482A-B393-7589236467C1");
            public static readonly Guid F_Full_Size_1_8_2_l_Petrol = new Guid("E8BF433F-214D-4D89-8C02-A2794A6F1938");
            public static readonly Guid G_Premium_1_8_2_l_Petrol = new Guid("269F3EF3-E74D-4DCC-B5A7-3E5E3F05A189");
            public static readonly Guid H_Hybrid_HYB = new Guid("3EB227D9-B369-47EB-9A0A-BDDE5E44A2F8");
            public static readonly Guid I_Compact_Petrol = new Guid("2F1CFEF5-4801-4AFD-80BF-EDC1642FB00B");
            public static readonly Guid J_Luxury_2_3_2_5_l_Petrol = new Guid("C3E1CEEF-51E8-435F-8AB7-77D0B759BD30");

            public static readonly Guid K_Speciality_SUV_2_4_2_5_l_Petrol =
                new Guid("585DCE22-93B4-4DA4-AA8B-43C5A26EF4B1");

            public static readonly Guid L_Speciality_Leisure_4X4_3_l = new Guid("5F7D5B47-79A0-4838-A1F5-1B7E353EB7E0");
            public static readonly Guid M_Economy_1_1_1_4_Petrol = new Guid("6FDFE1FC-8D82-47C7-970C-FCD7BC174E7C");
            public static readonly Guid N_Speciality_People_Carrier = new Guid("2944B143-A0A1-4755-9D62-B7E6B08FC149");
            public static readonly Guid O_Full_Size_1_8_2_l_Petrol = new Guid("84C39F06-3C18-406A-9FDB-F143D4333E3B");
            public static readonly Guid P_Full_Size_1_4_l_Petrol = new Guid("06909133-805E-4271-9FC2-6BE805588A28");
            public static readonly Guid Average_Petrol = new Guid("5A15585C-F779-433B-853B-E037C1C54B80");
            public static readonly Guid Greater_Than_2_Litres_Petrol = new Guid("4C639165-9531-4A31-B32E-FD8E2B43915E");
            public static readonly Guid Less_Than_1_4_Litres_Petrol = new Guid("3205AE00-929E-445F-A31C-7A48F95CCC55");
            public static readonly Guid Diesel17To2L = new Guid("7625c31c-b6e9-46d6-9fd8-fc2a39d14533");
            public static readonly Guid LessThan17Diesel = new Guid("99bbef2a-f249-4f9a-8f1a-729bc403a0af");
            public static readonly Guid GreaterThan2LDiesel = new Guid("f899d184-db66-403d-b2d1-2164c23caf0c");
            public static readonly Guid GreaterThan500Cc = new Guid("72e5b386-3778-4200-b36a-d3d08c70a365");
            public static readonly Guid AverageDiesel = new Guid("41380c49-96cc-4a8f-9d57-ec983f67d77e");
        }

        public static class Paper
        {
            public static readonly Guid MondiA4Direct = new Guid("A7A80D93-DA3B-48A5-868C-996846496161");
            public static readonly Guid SappiA4Direct = new Guid("072A30F6-B9F8-4A20-BC73-48B0FA0590D9");
            public static readonly Guid MondiA4Indirect = new Guid("FEA803DC-0058-4F03-91DF-1749112745D2");
            public static readonly Guid SappiA4Indirect = new Guid("E9C65569-13F1-49D5-9A69-8599C69CB14B");
        }

        public static class Fuel
        {
            public static readonly Guid Petrol = new Guid("92AB41D3-A874-415A-8B15-9367431735FF");
            public static readonly Guid Diesel = new Guid("F55DE783-DE44-499A-AC9E-F083EC70E1A4");
            public static readonly Guid LPG = new Guid("C0715292-0F24-41D1-9511-B2BF050AD605");
            public static readonly Guid CoalDomestic = new Guid("E516798F-1009-443B-B747-50EF93DCA163");
            public static readonly Guid CoalIndustrial = new Guid("E30BDFE4-00E5-4432-BA3C-47AF6555697A");
            public static readonly Guid AviationFuelTonnes = new Guid("60CA828A-CCBA-461B-8C3F-39780533B3EB");
            public static readonly Guid AviationFuelLitres = new Guid("DFF978EA-ED38-47FD-AB45-06DE179840E3");
            public static readonly Guid MarineFuelOilTonnes = new Guid("DBE2F19E-806F-41FA-9DCD-77440B968D36");
            public static readonly Guid LNGLitres = new Guid("CA62AD6A-3C76-4D06-A013-6E5BDC4A221E");
            public static readonly Guid LNGKWH = new Guid("6A8371FE-B281-4BA9-8D2F-20C6ADA3C05F");
            public static readonly Guid LNGTonnes = new Guid("1A426811-4A05-4180-82E9-CC5745B3AFE8");
            public static readonly Guid Paraffin = new Guid("CC795C4B-6F8B-4F7C-892E-C0CEBDD505C1");
            public static readonly Guid HeavyFuelOil = new Guid("900D43BE-F8ED-465B-B34A-3818AE8841DA");
            public static readonly Guid LPGGigajoules = new Guid("21A2F68A-1730-420D-BACD-B204D5489260");
        }

        public static class Fleet
        {
            public static readonly Guid Scope1Petrol = new Guid("203DE165-1678-491C-8C32-9CB19781FF42");
            public static readonly Guid Scope1Diesel = new Guid("48D0167D-53FA-4B44-83D5-C5A571409EA5");
            public static readonly Guid Scope3Petrol = new Guid("76BE78A6-55B6-461B-A712-D8DECCC0BEFC");
            public static readonly Guid Scope3Diesel = new Guid("281BD4DA-C051-4DCE-BC30-062D36E8A989");
        }

        public static class Refrigerant
        {
            public static readonly Guid R22Freon = new Guid("585E0250-1396-464F-8476-12877913B6C9");
            public static readonly Guid Refrigerant134 = new Guid("46EE37FB-EEA1-4A33-8C45-EF7CA8FD526F");
            public static readonly Guid Refrigerant143A = new Guid("0204F144-B401-4983-BEC8-2B71B77D39FD");
            public static readonly Guid HcFC134A = new Guid("3D323103-F97F-4B8A-BE3A-E174D59E4F01");
            public static readonly Guid R404A = new Guid("66160F37-AE8F-4DFB-9717-CF8640BE53C1");
            public static readonly Guid R410 = new Guid("3E5D69BE-182C-41A0-811B-E5C1464CB903");
            public static readonly Guid R410A = new Guid("054AF8E5-130F-47E1-BE8B-6D9DFC0FEDD3");
            public static readonly Guid R22Refrigerant = new Guid("83030C6B-36F4-4FA5-AA89-7CD551D07DFA");
            public static readonly Guid R407c = new Guid("BF4E689C-B0FB-47DA-AF00-E2E4DF88DBBD");
            public static readonly Guid HP80 = new Guid("B7973F38-779C-41AF-BA73-934C990D1DAA");
            public static readonly Guid R408 = new Guid("1EDA3C5F-05BC-42A9-A74A-04506EC7123D");
            public static readonly Guid R417a = new Guid("8824AE0A-249E-4A2C-B9C3-941EC1E1BDD5");
            public static readonly Guid R507 = new Guid("E4134B15-297B-451F-A8EC-58FA8DFDBA40");
        }

        public static class Comutting
        {
            public static readonly Guid EmployeeAverage = new Guid("41D7E99A-4E44-4D5A-9A21-3CD3BC56BC78");
            public static readonly Guid Train = new Guid("A6E0745A-B821-48C8-AB4F-649748BF33AC");
            public static readonly Guid Bus = new Guid("06B322C3-71C5-4BFD-A08C-6FCF5B6ADB0F");
            public static readonly Guid MiniBusTaxi = new Guid("E8A0B935-64A3-4FCB-9A7E-A9246D2BE7F0");
        }

        public static class Electricity
        {
            public static readonly Guid SouthAfricanNationalGrid = new Guid("C4D57FDE-2538-41E3-BAA1-7C3A479AB81E");
            public static readonly Guid BotswanaNationalGrid = new Guid("CB4EB33B-C476-4EB4-9BEA-86F6FD047F6C");
            public static readonly Guid AngolaNationalGrid = new Guid("D1F4EA23-D78B-4F20-BF45-847ABF5B858A");
            public static readonly Guid ZambiaNationalGrid = new Guid("D9E8D689-345E-4524-B943-025DCE04752F");
            public static readonly Guid NamibiaNationalGrid = new Guid("CDB4E208-DFE3-4F42-8ABF-931C043EA1A7");
            public static readonly Guid TanzaniaNationalGrid = new Guid("C906D820-75ED-4B85-9DA9-1B09EBC1F08E");
            public static readonly Guid KenyaNationalGrid = new Guid("80AC3EE4-5BD4-4C6D-84CD-89F79E988251");
            public static readonly Guid NigeriaNationalGrid = new Guid("AA93F4A5-5D7D-4709-BCA9-BA1C58652ACD");
            public static readonly Guid ZimbabweNationalGrid = new Guid("12F2AF21-3D73-468E-9D97-82672E0F324A");
            public static readonly Guid IsleofManNationalGrid = new Guid("61E6BD2D-A006-49F3-87D8-AF1F1F84364A");
            public static readonly Guid UKNationalGrid = new Guid("9D2319C9-CFA6-4C33-A339-0A8E10BAEAAE");
            public static readonly Guid MalawiNationalGrid = new Guid("6685CCFB-2FF1-4699-89BB-9F6B12F276E7");
            public static readonly Guid SwazilandNationalGrid = new Guid("27286FA1-0CEB-4505-8649-A476955A1E53");
            public static readonly Guid PurchasedSteam = new Guid("CB4012CB-9DBB-4E6F-AA2F-EBA987A9EE5D");
        }

        public static class Waste
        {
            public static readonly Guid WasteToLandfill = new Guid("AA354F3D-A91F-46F4-84FC-991048D19D23");
            public static readonly Guid RecylcedWaste = new Guid("3A2ACA96-9085-4099-9130-F027530387E4");
        }
        
        public static class AirTravel
        {
            public static readonly Guid LongHaulFirstClass = new Guid("0109781A-989B-4C99-B9D6-03B3A4259485");
            public static readonly Guid LongHaulAverage = new Guid("9262C558-1D4D-4F65-9874-367D1B4C3085");
            public static readonly Guid ShortHaulAverage = new Guid("6D3269D5-6339-4697-9F59-DCEDCD8B3655");
            public static readonly Guid DomesticAverage = new Guid("61730C1C-A6DF-49DA-837F-66C687D0669B");
            public static readonly Guid LongHaulEconomyClass = new Guid("90309BB6-72F3-4967-8FA9-412569C5A78D");
            public static readonly Guid ShortHaulEconomyClass = new Guid("825E3EDC-C89F-425C-B4F9-2CD109F6A8A0");
            public static readonly Guid LongHaulBusinessClass = new Guid("1AB050B7-26FA-420C-9F69-9B0FFE419ED5");
            public static readonly Guid ShortHaulBusinessClass = new Guid("0D7D7DE7-DB31-489B-B40B-B6C282B56436");
        }

        public static class Courier
        {
            public static readonly Guid Road = new Guid("18C261B5-54D0-4F8B-BBF5-65A2395A7B46");
            public static readonly Guid Domestic = new Guid("411F45FC-3CBC-4126-AD50-679FE05686FB");
            public static readonly Guid LongHaul = new Guid("60C6495C-34A1-480F-AE43-D30CF454DBBD");
            public static readonly Guid ShortHaul = new Guid("166B54EA-627E-41B2-9C95-E0B9F9C85AE2");
        }

    }
}
