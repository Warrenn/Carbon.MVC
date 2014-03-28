using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.WCF.Accommodation;
using CarbonKnown.WCF.AirTravel;
using CarbonKnown.WCF.CarHire;
using CarbonKnown.WCF.Commuting;
using CarbonKnown.WCF.Electricity;
using CarbonKnown.WCF.Fleet;
using CarbonKnown.WCF.Fuel;
using CarbonKnown.WCF.Paper;
using CarbonKnown.WCF.Refrigerant;
using CarbonKnown.WCF.Waste;
using CarbonKnown.WCF.Water;
using FuelType = CarbonKnown.WCF.Fleet.FuelType;

namespace CarbonKnown.FileReaders.Generic
{
    public sealed class GenericHandler : FileHandlerBase<GenericDataContract>
    {

        public readonly static IDictionary<string, Action<FileHandlerBase<GenericDataContract>, GenericDataContract>> Mappings
            = new SortedDictionary
                <string, Action<FileHandlerBase<GenericDataContract>, GenericDataContract>>
                {
                    {
                        "(Scope 3) Business Travel > Commercial Airlines > Long-haul International First Class (> 3700 km) - kilometres"
                        , AirTravelFirstClass
                    },
                    {"(Scope 3) Business Travel > Hotel Accommodation > Hotel Accomodation - Bed Nights", Accommodation},
                    {"(Scope 3) Third-party vehicle fleet > Diesel Vehicles - litres", FleetDieselThirdParty},
                    {"(Scope 1) Equipment owned or controlled > Aviation Fuel - litres", FuelAviationLitres},
                    {"(Non-Kyoto) Non-Kyoto > R22 (Freon) - kg", RefrigerantR22Freon},
                    {
                        "(Scope 3) Office Paper > Mondi > Mondi Rotatrim - Indirect Emissions  - tonnes",
                        PaperMondiA4Tonnes
                    },
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Diesel Cars > L - Speciality Leisure 4X4  3 l  - kilometres"
                        , CarHireL
                    },
                    {"(Scope 1) Air conditioning and refrigeration gas refills > 143a - kg", Refrigerant143A},
                    {
                        "(Scope 3) Business Travel > Commercial Airlines > Short-haul International  Economy (463 - 3700 km) - kilometres"
                        , AirTravelEconomy
                    },
                    {
                        "(Scope 3) Business Travel > Commercial Airlines > Long-haul International Average (> 3700 km)  - kilometres"
                        , AirTravelAverage
                    },
                    {"(Scope 1) Equipment owned or controlled > Aviation Fuel - tonnes", FuelAviationTonnes},
                    {"(Scope 3) Water > Water - litres", Water},
                    {"(Scope 3) Employee Commuting > Employee Commuting (Average) - kilometres", CommutingAverage},
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Petrol Cars > G - Premium  1.8 - 2 l (Petrol) - kilometres"
                        , CarHireG
                    },
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Petrol Cars > K - Speciality SUV 2.4 - 2.5 l (Petrol) - kilometres"
                        , CarHireK
                    },
                    {"(Scope 1) Equipment owned or controlled > Coal - industrial  - tonnes", FuelCoalIndustrialTonnes},
                    {"(Scope 3) Office Paper > Sappi > Typek Bond - To air at production  - tonnes", PaperSappiA4Tonnes},
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Petrol Cars > B - Compact 1.4 - 2 l (Petrol) - kilometres"
                        , CarHireB
                    },
                    {"(Scope 1) Equipment owned or controlled > Coal - domestic - tonnes", FuelCoalDomesticTonnes},
                    {"(Scope 3) Employee Commuting > Train  - kilometres", CommutingTrain},
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Petrol Cars > P - Full Size 1.4 l (Petrol) - kilometres"
                        , CarHireP
                    },
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Petrol Cars > D - Intermediate 1.6 l (Petrol) - kilometres"
                        , CarHireD
                    },
                    {"(Scope 1) Air conditioning and refrigeration gas refills > R410A - kg", Refrigerant410A},
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Petrol Cars > C - Intermediate 1.6 l (Petrol) - kilometres"
                        , CarHireC
                    },
                    {"(Scope 3) Employee Commuting > Bus - kilometres", CommutingBus},
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Petrol Cars > E - Standard 2.0 - 2.4 l (Petrol) - kilometres"
                        , CarHireE
                    },
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Petrol Cars > J - Luxury 2.3 - 2.5 l (Petrol) - kilometres"
                        , CarHireJ
                    },
                    {
                        "(Scope 3) Employee Commuting > Road Transport Conversion Factors > Employee Commuting > Petrol Cars > Less than 1.4 l petrol  - kilometres"
                        , CarHireLessThan14LPetrol
                    },
                    {"(Scope 2) Electricity > South African National Grid  - kWh", Electricity},
                    {"(Non-Kyoto) Non-Kyoto > R22 Refrigerant - kilograms", RefrigerantR22},
                    {"(Scope 1) Equipment owned or controlled > Petrol  - litres", FuelPetrolLitres},
                    {"(Scope 3) Waste > Waste to landfill - tonnes", WasteLandfill},
                    {
                        "(Scope 3) Business Travel > Commercial Airlines > Long-haul International Business (> 3700 km) - kilometres"
                        , AirTravelBusiness
                    },
                    {"(Scope 1) Company-owned Vehicle Fleet > Petrol Vehicles - litres", FleetPetrolCompanyOwned},
                    {"(Scope 3) Office Paper > Mondi > Mondi Rotatrim A3 - To air at production - tonnes", PaperMondiA3Tonnes},
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Petrol Cars > F - Full Size 1.8 - 2 l (Petrol) - kilometres"
                        , CarHireF
                    },
                    {"(Scope 3) Employee Commuting > Mini-bus taxi  - kilometres", CommuttingMiniBusTaxi},
                    {"(Scope 1) Equipment owned or controlled > LPG  - litres", FuelLPGLitres},
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Diesel Cars > N - Speciality People Carrier  - kilometres"
                        , CarHireN
                    },
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Petrol Cars > H - Hybrid HYB - kilometres", CarHireH
                    },
                    {"(Scope 1) Company-owned Vehicle Fleet > Diesel Vehicles - litres", FleetDieselCompanyOwned},
                    {
                        "(Scope 1) Air conditioning and refrigeration gas refills > R404A Refrigerant - kilograms",
                        RefrigerantR404A
                    },
                    {"(Scope 3) Third-party vehicle fleet > Petrol Vehicles - litres", FleetPetrolThirdParty},
                    {
                        "(Scope 1) Air conditioning and refrigeration gas refills > HcFC 134a Refrigerant  - kilograms",
                        RefrigerantHcFC134A
                    },
                    {"(Scope 1) Air conditioning and refrigeration gas refills > R410 - kilograms", RefrigerantR410},
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Petrol Cars > I - Compact  (Petrol) - kilometres"
                        ,
                        CarHireI
                    },
                    {"(Scope 1) Air conditioning and refrigeration gas refills > 134 - kg", Refrigerant134},
                    {"(Scope 3) Waste > Recycled Waste - tonnes", WasteRecycled},
                    {"(Scope 1) Equipment owned or controlled > Diesel  - litres", FuelDieselLitres},
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Petrol Cars > A - Economy  < 1.4 l  - kilometres"
                        ,
                        CarHireA
                    },
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Petrol Cars > O - Full Size 1.8 - 2 l (Petrol) - kilometres"
                        , CarHireO
                    },
                    {
                        "(Scope 3) Business Travel > Rental Vehicles > Petrol Cars > M - Economy 1.1 - 1.4 (Petrol) - kilometres"
                        , CarHireM
                    },
                    {
                        "(Scope 3) Employee Commuting > Road Transport Conversion Factors > Employee Commuting > Petrol Cars > Greater than 2.0 l petrol  - kilometres"
                        , CarHireGreaterThan2LPetrol
                    }
                };


        public GenericHandler(string host)
            : base(host)
        {
            MapColumns(c => c.CostCode, ConvertCostCode, "Carbon Transactions");
            MapColumns(c => c.ConsumptionType, "Column2");
            MapColumns(c => c.StartDate, "Column3");
            MapColumns(c => c.EndDate, "Column4");
            MapColumns(c => c.Units, "Column5");
            MapColumns(c => c.Money, "Column8");
        }

        private static void AirTravelFirstClass(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new AirTravelDataContract
                {
                    CostCode = obj.CostCode,
                    EndDate = obj.EndDate,
                    Money = obj.Money,
                    RowNo = obj.RowNo,
                    SourceId = obj.SourceId,
                    StartDate = obj.StartDate,
                    TravelClass = TravelClass.FirstClass,
                    Units = obj.Units
                };
            handler.CallService<IAirTravelService>(service => service.UpsertDataEntry(data));
        }

        private static void Accommodation(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new AccommodationDataContract
                {
                    CostCode = obj.CostCode,
                    EndDate = obj.EndDate,
                    Money = obj.Money,
                    RowNo = obj.RowNo,
                    SourceId = obj.SourceId,
                    StartDate = obj.StartDate,
                    Units = obj.Units
                };
            handler.CallService<IAccommodationService>(service => service.UpsertDataEntry(data));
        }

        private static void FleetDieselThirdParty(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new FleetDataContract
                {
                    CostCode = obj.CostCode,
                    EndDate = obj.EndDate,
                    Money = obj.Money,
                    RowNo = obj.RowNo,
                    SourceId = obj.SourceId,
                    StartDate = obj.StartDate,
                    Units = obj.Units,
                    FuelType = FuelType.Diesel,
                    Scope = FleetScope.ThirdParty
                };
            handler.CallService<IFleetService>(service => service.UpsertDataEntry(data));
        }

        private static void FuelAviationLitres(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new FuelDataContract
                {
                    CostCode = obj.CostCode,
                    EndDate = obj.EndDate,
                    Money = obj.Money,
                    RowNo = obj.RowNo,
                    SourceId = obj.SourceId,
                    StartDate = obj.StartDate,
                    Units = obj.Units,
                    FuelType = WCF.Fuel.FuelType.AviationFuel,
                    UOM = UnitOfMeasure.Litres
                };
            handler.CallService<IFuelService>(service => service.UpsertDataEntry(data));
        }

        private static void RefrigerantR22Freon(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new RefrigerantDataContract
                {
                    CostCode = obj.CostCode,
                    EndDate = obj.EndDate,
                    Money = obj.Money,
                    RowNo = obj.RowNo,
                    SourceId = obj.SourceId,
                    StartDate = obj.StartDate,
                    Units = obj.Units,
                    RefrigerantType = RefrigerantType.R22Freon
                };
            handler.CallService<IRefrigerantService>(service => service.UpsertDataEntry(data));
        }

        private static void PaperMondiA4Tonnes(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new PaperDataContract
                {
                    CostCode = obj.CostCode,
                    EndDate = obj.EndDate,
                    Money = obj.Money,
                    RowNo = obj.RowNo,
                    SourceId = obj.SourceId,
                    StartDate = obj.StartDate,
                    Units = obj.Units,
                    PaperType = PaperType.MondiA4,
                    PaperUom = PaperUom.Tonnes
                };
            handler.CallService<IPaperService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireL(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
                {
                    CostCode = obj.CostCode,
                    EndDate = obj.EndDate,
                    Money = obj.Money,
                    RowNo = obj.RowNo,
                    SourceId = obj.SourceId,
                    StartDate = obj.StartDate,
                    Units = obj.Units,
                    CarGroupBill = CarGroupBill.L
                };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void Refrigerant143A(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new RefrigerantDataContract
                {
                    CostCode = obj.CostCode,
                    EndDate = obj.EndDate,
                    Money = obj.Money,
                    RowNo = obj.RowNo,
                    SourceId = obj.SourceId,
                    StartDate = obj.StartDate,
                    Units = obj.Units,
                    RefrigerantType = RefrigerantType.Refrigerant143A
                };
            handler.CallService<IRefrigerantService>(service => service.UpsertDataEntry(data));
        }

        private static void AirTravelEconomy(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new AirTravelDataContract
                {
                    CostCode = obj.CostCode,
                    EndDate = obj.EndDate,
                    Money = obj.Money,
                    RowNo = obj.RowNo,
                    SourceId = obj.SourceId,
                    StartDate = obj.StartDate,
                    Units = obj.Units,
                    TravelClass = TravelClass.Economy
                };
            handler.CallService<IAirTravelService>(service => service.UpsertDataEntry(data));
        }

        private static void AirTravelAverage(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new AirTravelDataContract
                {
                    CostCode = obj.CostCode,
                    EndDate = obj.EndDate,
                    Money = obj.Money,
                    RowNo = obj.RowNo,
                    SourceId = obj.SourceId,
                    StartDate = obj.StartDate,
                    Units = obj.Units,
                    TravelClass = TravelClass.Average
                };
            handler.CallService<IAirTravelService>(service => service.UpsertDataEntry(data));
        }

        private static void FuelAviationTonnes(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new FuelDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                FuelType = WCF.Fuel.FuelType.AviationFuel,
                UOM = UnitOfMeasure.Tonnes
            };
            handler.CallService<IFuelService>(service => service.UpsertDataEntry(data));
        }

        private static void Water(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new WaterDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units
            };
            handler.CallService<IWaterService>(service => service.UpsertDataEntry(data));
        }

        private static void CommutingAverage(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CommutingDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CommutingType = CommutingType.EmployeeAverage
            };
            handler.CallService<ICommutingService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireG(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.G
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireGreaterThan2LPetrol(FileHandlerBase<GenericDataContract> handler,
                                                       GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.GreaterThan2LPetrol
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireM(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.M
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireO(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.O
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireA(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.A
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void FuelDieselLitres(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new FuelDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                FuelType = WCF.Fuel.FuelType.Diesel,
                UOM = UnitOfMeasure.Litres
            };
            handler.CallService<IFuelService>(service => service.UpsertDataEntry(data));
        }

        private static void WasteRecycled(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new WasteDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                WasteType = WasteType.RecycledWaste
            };
            handler.CallService<IWasteService>(service => service.UpsertDataEntry(data));
        }

        private static void Refrigerant134(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new RefrigerantDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                RefrigerantType = RefrigerantType.Refrigerant134
            };
            handler.CallService<IRefrigerantService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireI(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.I
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void RefrigerantR410(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new RefrigerantDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                RefrigerantType = RefrigerantType.R410
            };
            handler.CallService<IRefrigerantService>(service => service.UpsertDataEntry(data));
        }

        private static void RefrigerantHcFC134A(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new RefrigerantDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                RefrigerantType = RefrigerantType.HcFC134A
            };
            handler.CallService<IRefrigerantService>(service => service.UpsertDataEntry(data));
        }

        private static void FleetPetrolThirdParty(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new FleetDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                FuelType = FuelType.Petrol,
                Scope = FleetScope.ThirdParty
            };
            handler.CallService<IFleetService>(service => service.UpsertDataEntry(data));
        }

        private static void RefrigerantR404A(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new RefrigerantDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                RefrigerantType = RefrigerantType.R404A
            };
            handler.CallService<IRefrigerantService>(service => service.UpsertDataEntry(data));
        }

        private static void FleetDieselCompanyOwned(FileHandlerBase<GenericDataContract> handler,
                                                    GenericDataContract obj)
        {
            var data = new FleetDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                FuelType = FuelType.Diesel,
                Scope = FleetScope.CompanyOwned
            };
            handler.CallService<IFleetService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireH(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.H
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireN(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.N
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void FuelLPGLitres(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new FuelDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                FuelType = WCF.Fuel.FuelType.LPG,
                UOM = UnitOfMeasure.Litres
            };
            handler.CallService<IFuelService>(service => service.UpsertDataEntry(data));
        }

        private static void CommuttingMiniBusTaxi(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CommutingDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CommutingType = CommutingType.MiniBusTaxi
            };
            handler.CallService<ICommutingService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireF(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.F
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void PaperMondiA3Tonnes(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new PaperDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                PaperType = PaperType.MondiA3,
                PaperUom = PaperUom.Tonnes
            };
            handler.CallService<IPaperService>(service => service.UpsertDataEntry(data));
        }

        private static void FleetPetrolCompanyOwned(FileHandlerBase<GenericDataContract> handler,
                                                    GenericDataContract obj)
        {
            var data = new FleetDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                FuelType = FuelType.Petrol,
                Scope = FleetScope.CompanyOwned
            };
            handler.CallService<IFleetService>(service => service.UpsertDataEntry(data));
        }

        private static void AirTravelBusiness(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new AirTravelDataContract
                {
                    CostCode = obj.CostCode,
                    EndDate = obj.EndDate,
                    Money = obj.Money,
                    RowNo = obj.RowNo,
                    SourceId = obj.SourceId,
                    StartDate = obj.StartDate,
                    Units = obj.Units,
                    TravelClass = TravelClass.Business
                };
            handler.CallService<IAirTravelService>(service => service.UpsertDataEntry(data));
        }

        private static void WasteLandfill(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new WasteDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                WasteType = WasteType.WasteToLandFill
            };
            handler.CallService<IWasteService>(service => service.UpsertDataEntry(data));
        }

        private static void FuelPetrolLitres(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new FuelDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                FuelType = WCF.Fuel.FuelType.Petrol,
                UOM = UnitOfMeasure.Litres
            };
            handler.CallService<IFuelService>(service => service.UpsertDataEntry(data));
        }

        private static void RefrigerantR22(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new RefrigerantDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                RefrigerantType = RefrigerantType.R22Refrigerant
            };
            handler.CallService<IRefrigerantService>(service => service.UpsertDataEntry(data));
        }

        private static void Electricity(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new ElectricityDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                ElectricityType = ElectricityType.SouthAfricanNationalGrid
            };
            handler.CallService<IElectricityService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireLessThan14LPetrol(FileHandlerBase<GenericDataContract> handler,
                                                     GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.LessThan14LPetrol
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireJ(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.J
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireE(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.E
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void CommutingBus(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CommutingDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CommutingType = CommutingType.Bus
            };
            handler.CallService<ICommutingService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireC(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.C
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void Refrigerant410A(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new RefrigerantDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                RefrigerantType = RefrigerantType.R410A
            };
            handler.CallService<IRefrigerantService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireD(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.D
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireP(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.P
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void CommutingTrain(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CommutingDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CommutingType = CommutingType.Train
            };
            handler.CallService<ICommutingService>(service => service.UpsertDataEntry(data));
        }

        private static void FuelCoalDomesticTonnes(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new FuelDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                FuelType = WCF.Fuel.FuelType.CoalDomestic,
                UOM = UnitOfMeasure.Tonnes
            };
            handler.CallService<IFuelService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireB(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.B
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void PaperSappiA4Tonnes(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new PaperDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                PaperType = PaperType.SappiA4,
                PaperUom = PaperUom.Tonnes
            };
            handler.CallService<IPaperService>(service => service.UpsertDataEntry(data));
        }

        private static void FuelCoalIndustrialTonnes(FileHandlerBase<GenericDataContract> handler,
                                                     GenericDataContract obj)
        {
            var data = new FuelDataContract
                {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                FuelType = WCF.Fuel.FuelType.CoalIndustrial,
                UOM = UnitOfMeasure.Tonnes
            };
            handler.CallService<IFuelService>(service => service.UpsertDataEntry(data));
        }

        private static void CarHireK(FileHandlerBase<GenericDataContract> handler, GenericDataContract obj)
        {
            var data = new CarHireDataContract
            {
                CostCode = obj.CostCode,
                EndDate = obj.EndDate,
                Money = obj.Money,
                RowNo = obj.RowNo,
                SourceId = obj.SourceId,
                StartDate = obj.StartDate,
                Units = obj.Units,
                CarGroupBill = CarGroupBill.K
            };
            handler.CallService<ICarHireService>(service => service.UpsertDataEntry(data));
        }

        private static void ConvertCostCode(GenericDataContract contract, object value)
        {
            var stringValue = string.Format("{0}", value).Trim();
            var costCodeParts = stringValue.Split(new[] {':'});
            var costCode = costCodeParts[0].Trim();
            contract.CostCode = costCode;
        }

        public override IDictionary<string, IEnumerable<string>> MissingColumns(string fullPath,
                                                                                Stream fileStream)
        {
            var sourceId = GetSourceId(fullPath);
            FileReader = GetReader(fullPath, sourceId);
            if (FileReader == null) return new Dictionary<string, IEnumerable<string>>();
            using (FileReader)
            {
                var firstRow = FileReader.ExtractData(fileStream).FirstOrDefault();
                if (firstRow == null) return new Dictionary<string, IEnumerable<string>>();
                var column1 = string.Format("{0}", firstRow.First().Value).Trim();
                var column2 = string.Format("{0}", firstRow["Column2"]).Trim();
                var errors = new Dictionary<string, IEnumerable<string>>
                    {
                        {"Worksheet Version", new[] {"3", "4"}}
                    };
                if (!string.Equals(column1, "Worksheet Version:", StringComparison.InvariantCultureIgnoreCase))
                {
                    return errors;
                }
                int version;
                if ((!int.TryParse(column2, out version)) ||
                    ((version != 3) &&
                     (version != 4)))
                {
                    return errors;
                }
                return new Dictionary<string, IEnumerable<string>>();
            }
        }

        public override void UpsertDataEntry(GenericDataContract contract)
        {
            var key = contract.ConsumptionType;
            if (!string.IsNullOrEmpty(key) && Mappings.ContainsKey(key))
            {
                contract.RowNo = contract.RowNo - 2;
                Mappings[key](this, contract);
            }
        }
    }
}
