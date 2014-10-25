using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CarbonKnown.Calculation;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.AirTravel;
using CarbonKnown.DAL.Models.CarHire;
using CarbonKnown.DAL.Models.Commuting;
using CarbonKnown.DAL.Models.Courier;
using CarbonKnown.DAL.Models.Electricity;
using CarbonKnown.DAL.Models.Fleet;
using CarbonKnown.DAL.Models.Fuel;
using CarbonKnown.DAL.Models.Paper;
using CarbonKnown.DAL.Models.Refrigerant;
using CarbonKnown.DAL.Models.Source;
using CarbonKnown.DAL.Models.Waste;
using CarbonKnown.MVC.Models;
using CarbonKnown.MVC.Properties;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize(Roles = "Admin,Capturer")]
    public class InputController : Controller
    {
        private readonly DataContext context;
        private readonly ICalculationDataContext calculation;

        public static readonly IDictionary<CarGroupBill, string> GroupBills =
            new SortedDictionary<CarGroupBill, string>
                {
                    {CarGroupBill.A, "A - Economy  < 1.4 l"},
                    {CarGroupBill.B, "B - Compact 1.4 - 2 l (Petrol)"},
                    {CarGroupBill.C, "C - Intermediate 1.6 l (Petrol) "},
                    {CarGroupBill.D, "D - Intermediate 1.6 l (Petrol)"},
                    {CarGroupBill.E, "E - Standard 2.0 - 2.4 l (Petrol)"},
                    {CarGroupBill.F, "F - Full Size 1.8 - 2 l (Petrol)"},
                    {CarGroupBill.G, "G - Premium  1.8 - 2 l (Petrol)"},
                    {CarGroupBill.H, "H - Hybrid HYB"},
                    {CarGroupBill.I, "I - Compact  (Petrol)"},
                    {CarGroupBill.J, "J - Luxury 2.3 - 2.5 l (Petrol)"},
                    {CarGroupBill.K, "K - Speciality SUV 2.4 - 2.5 l (Petrol)"},
                    {CarGroupBill.L, "L - Speciality Leisure 4X4  3 l "},
                    {CarGroupBill.M, "M - Economy 1.1 - 1.4 (Petrol)"},
                    {CarGroupBill.N, "N - Speciality People Carrier "},
                    {CarGroupBill.O, "O - Full Size 1.8 - 2 l (Petrol)"},
                    {CarGroupBill.P, "P - Full Size 1.4 l (Petrol)"},
                    {CarGroupBill.AveragePetrol, "Average Petrol (If not known)"},
                    {CarGroupBill.GreaterThan2LPetrol, "Greater Than 2.0 l petrol"},
                    {CarGroupBill.LessThan14LPetrol, "Less than 1.4 l petrol"},
                    {CarGroupBill.Diesel17To2L, " 1.7  - 2.0 l diesel"},
                    {CarGroupBill.LessThan17Diesel, "Less than 1.7 l diesel "},
                    {CarGroupBill.GreaterThan2LDiesel, "Greater than 2.0 l diesel "},
                    {CarGroupBill.GreaterThan500Cc, "Greater than 500cc"},
                    {CarGroupBill.AverageDiesel, "Average Diesel (capacity not known)"}
                };

        public static readonly IDictionary<ElectricityType, string> ElectricityTypes =
            new SortedDictionary<ElectricityType, string>
                {
                    {ElectricityType.SouthAfricanNationalGrid, "South African National Grid"},
                    {ElectricityType.AngolaNationalGrid, "Angola National Grid"},
                    {ElectricityType.BotswanaNationalGrid, "Botswana National Grid"},
                    {ElectricityType.ZambiaNationalGrid, "Zambia National Grid"},
                    {ElectricityType.NamibiaNationalGrid, "Namibia National Grid"},
                    {ElectricityType.TanzaniaNationalGrid, "Tanzania National Grid"},
                    {ElectricityType.KenyaNationalGrid, "Kenya National Grid"},
                    {ElectricityType.NigeriaNationalGrid, "Nigeria National Grid"},
                    {ElectricityType.ZimbabweNationalGrid, "Zimbabwe National Grid"},
                    {ElectricityType.IsleOfManNationalGrid, "Isle of Man National Grid"},
                    {ElectricityType.UKNationalGrid, "UK National Grid"},
                    {ElectricityType.MalawiNationalGrid, "Malawi National Grid"},
                    {ElectricityType.SwazilandNationalGrid, "Swaziland National Grid"},
                    {ElectricityType.PurchasedSteam, "Purchased Steam"}


                };

        public static readonly IDictionary<Guid, string> SelectableCalculations =
            new SortedDictionary<Guid, string>
                {
                    {CarbonKnown.DAL.Models.Constants.Calculation.FuelId, "Fuel"},
                    {CarbonKnown.DAL.Models.Constants.Calculation.FleetId, "Fleet"},
                    {CarbonKnown.DAL.Models.Constants.Calculation.RefrigerantId, "Refrigerants"},
                    {CarbonKnown.DAL.Models.Constants.Calculation.CommutingId, "Commuting"}
                };

        public static readonly IDictionary<TravelClass, string> TravelClasses =
            new SortedDictionary<TravelClass, string>
                {
                    {TravelClass.Average, "Average"},
                    {TravelClass.Business, "Business"},
                    {TravelClass.Economy, "Economy"},
                    {TravelClass.FirstClass, "FirstClass"},
                };

        public static readonly IDictionary<ServiceType, string> CourierServiceTypes =
            new SortedDictionary<ServiceType, string>
                {
                    {ServiceType.Economy, "Economy"},
                    {ServiceType.Other, "Other"}
                };

        public static readonly IDictionary<FuelType, string> FuelTypes =
            new SortedDictionary<FuelType, string>
                {
                    {FuelType.AviationFuel, "Aviation Fuel"},
                    {FuelType.CoalDomestic, "Coal Domestic"},
                    {FuelType.CoalIndustrial, "Coal Industrial"},
                    {FuelType.Diesel, "Diesel"},
                    {FuelType.LPG, "Liquified Petroleum Gas (LPG)"},
                    {FuelType.Petrol, "Petrol"},
                    {FuelType.LPGGigajoule, "LPG(Gigajoule)"},
                    {FuelType.MarineFuelOil, "Marine Fuel Oil"},
                    {FuelType.LNGlitres, "Liquid Natural Gas (litres)"},
                    {FuelType.LNGkWH, "Liquid Natural Gas (kWH)"},
                    {FuelType.LNGTonnes, "Liquid Natural Gas (Tonnes)"},
                    {FuelType.Paraffin, "Paraffin"},
                    {FuelType.HeavyFuelOil, "Heavy Fuel Oil"}
                };

        public static readonly IDictionary<RefrigerantType, string> RefrigerantTypes =
            new SortedDictionary<RefrigerantType, string>
                {
                    {RefrigerantType.HcFC134A, "HcFC 134a"},
                    {RefrigerantType.R22Freon, "R22 Freon (non Kyoto)"},
                    {RefrigerantType.R404A, "R404a"},
                    {RefrigerantType.R410, "R410"},
                    {RefrigerantType.R410A, "R410a"},
                    {RefrigerantType.Refrigerant134, "134"},
                    {RefrigerantType.Refrigerant143A, "143 a"},
                    {RefrigerantType.R22Refrigerant, "R22 Refrigerant"},
                    {RefrigerantType.R407c, "R407c"},
                    {RefrigerantType.HP80, "HP80"},
                    {RefrigerantType.R408, "R408"},
                    {RefrigerantType.R417a, "R417a"},
                    {RefrigerantType.R507, "R507"}

                };

        public static readonly IDictionary<FuelType, string> FleetFuelTypes =
            new SortedDictionary<FuelType, string>
                {
                    {FuelType.Diesel, "Diesel"},
                    {FuelType.Petrol, "Petrol"}
                };

        public static readonly IDictionary<PaperType, string> PaperTypes =
            new SortedDictionary<PaperType, string>
                {
                    {PaperType.MondiA3, "MONDI ROTATRIM Copy Paper A3 297mmx420mm 80gsm Bond White"},
                    {PaperType.MondiA4, "MONDI ROTATRIM Copy Paper A4 80Gsm White"},
                    {PaperType.SappiA3, "Sappi Typek Copy Paper A3 297mmx420mm 80gsm Bond White"},
                    {PaperType.SappiA4, "Sappi Typek Copy Paper A4 80Gsm White"},
                    {PaperType.PolicyPaper, "Policy Paper"}
                };

        public static readonly IDictionary<WasteType, string> WasteTypes =
            new SortedDictionary<WasteType, string>
                {
                    {WasteType.RecycledWaste, "Recyled Waste"},
                    {WasteType.WasteToLandFill, "Waste To Landfill"}
                };

        public static readonly IDictionary<WasteType, object> WasteTypesUom =
            new SortedDictionary<WasteType, object>
                {
                    {WasteType.RecycledWaste, "tonnes"},
                    {WasteType.WasteToLandFill, "tonnes"}
                };

        public static readonly IDictionary<FleetScope, string> FleetScopes =
            new SortedDictionary<FleetScope, string>
                {
                    {FleetScope.CompanyOwned, "Company owned vehicle fleet"},
                    {FleetScope.ThirdParty, "Third party vehicle fleet"}
                };

        public static readonly IDictionary<CommutingType, string> CommutingTypes =
            new SortedDictionary<CommutingType, string>
                {
                    {CommutingType.Bus, "Bus"},
                    {CommutingType.EmployeeAverage, "Employee Commuting (Average)"},
                    {CommutingType.Train, "Train"},
                    {CommutingType.MiniBusTaxi, "Mini-bus taxi"},
                };

        private static string GetName<T>(T value) where T : struct
        {
            return Enum.GetName(typeof (T), value);
        }

        public static readonly IDictionary<FuelType, object> FuelTypesUOM =
            new SortedDictionary<FuelType, object>
                {
                    {
                        FuelType.AviationFuel, new[]
                            {
                                new {id = GetName(UnitOfMeasure.Litres), text = "litres"},
                                new {id = GetName(UnitOfMeasure.Tonnes), text = "tonnes"}
                            }
                    },
                    {
                        FuelType.CoalDomestic, new[]
                            {
                                new {id = GetName(UnitOfMeasure.Tonnes), text = "tonnes"}
                            }
                    },
                    {
                        FuelType.CoalIndustrial, new[]
                            {
                                new {id = GetName(UnitOfMeasure.Tonnes), text = "tonnes"}
                            }
                    },
                    {
                        FuelType.Diesel, new[]
                            {
                                new {id = GetName(UnitOfMeasure.Litres), text = "litres"}
                            }
                    },
                    {
                        FuelType.LPG, new[]
                            {
                                new {id = GetName(UnitOfMeasure.Litres), text = "litres"}
                            }
                    },
                    {
                        FuelType.Petrol, new[]
                            {
                                new {id = GetName(UnitOfMeasure.Litres), text = "litres"}
                            }
                    },
                    {
                        FuelType.HeavyFuelOil, new[]
                            {
                                new {id = GetName(UnitOfMeasure.Kilograms), text = "Kilograms"}
                            }
                    },
                    {
                        FuelType.MarineFuelOil, new[]
                            {
                                new {id = GetName(UnitOfMeasure.Tonnes), text = "Tonnes"}
                            }
                    },
                    {
                        FuelType.Paraffin, new[]
                            {
                                new {id = GetName(UnitOfMeasure.Litres), text = "Litres"}
                            }
                    },
                    {
                        FuelType.LNGkWH, new[]
                            {
                                new {id = GetName(UnitOfMeasure.KiloWattHours), text = "KWH"}
                            }
                    },
                     {
                        FuelType.LNGlitres, new[]
                            {
                                new {id = GetName(UnitOfMeasure.Litres), text = "Litres"}
                            }
                    },
                     {
                        FuelType.LNGTonnes, new[]
                            {
                                new {id = GetName(UnitOfMeasure.Tonnes), text = "Tonnes"}
                            }
                    },
                     {
                        FuelType.LPGGigajoule, new[]
                            {
                                new {id = GetName(UnitOfMeasure.Gigajoules), text = "Gigajoules"}
                            }
                    }
                };

        public InputController(DataContext context, ICalculationDataContext calculation)
        {
            this.context = context;
            this.calculation = calculation;
        }

        [HttpGet]
        public ActionResult AirRouteCodes(string searchTerm,string otherCode)
        {
            var codes = calculation.AirRouteCodes(otherCode);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToUpper();
                codes = codes.Where(s => s.Contains(searchTerm));
            }
            return Json(codes.OrderBy(s => s), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CourierRouteCodes(string searchTerm, string otherCode)
        {
            var codes = calculation.CourierRouteCodes(otherCode);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToUpper();
                codes = codes.Where(s => s.Contains(searchTerm));
            }
            return Json(codes.OrderBy(s => s), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CostCentres(ConsumptionType consumptionType, string term, string id)
        {
            var centres = context.CostCentres.Where(centre => ((centre.ConsumptionType & consumptionType) > 0));
            if (!string.IsNullOrEmpty(term))
            {
                centres = centres.Where(centre => centre.CostCode.Contains(term) || centre.Name.Contains(term));
            }
            if (!string.IsNullOrEmpty(id))
            {
                centres = centres.Where(centre => centre.CostCode == id);
            }
            var model = centres.Select(centre => new {centre.CostCode, centre.Name, centre.CurrencyCode});
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EnterData(Guid? calculationId)
        {
            var initialId = calculationId ?? Settings.Default.InitialCalculationId;
            if (initialId == CarbonKnown.DAL.Models.Constants.Calculation.MigrationId)
            {
                return RedirectToActionPermanent("Index", "InputHistory");
            }
            var viewName = CalculationModelFactory.EntryTypes[initialId].Name;
            var today = DateTime.Today;
            var startDate = new DateTime(today.Year, today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var model = new EnterDataModel
                {
                    EntryData = new DataEntry
                        {
                            StartDate = startDate,
                            EndDate = endDate
                        },
                    EntryErrors = new string[] {},
                    Variance = GetVariances(initialId),
                    CanRevert = false,
                    CanEdit = true,
                    ManualEntry = true,
                    ViewName = viewName
                };
            return ViewInputType(viewName, model);
        }

        [HttpGet]
        public ActionResult EditData(Guid? entryId)
        {
            var entry = context.DataEntries.Find(entryId);
            if (entry == null)
            {
                return RedirectToAction("EnterData");
            }
            var calculationId = entry.CalculationId;
            if (calculationId == CarbonKnown.DAL.Models.Constants.Calculation.MigrationId)
            {
                return RedirectToActionPermanent("Index", "InputHistory");
            }
            var type = CalculationModelFactory.EntryTypes[calculationId];
            var viewName = type.Name;
            var model = new EnterDataModel
                {
                    EntryData = (context.Set(type).Find(entryId) as DataEntry) ?? entry,
                    ManualEntry = (string.Equals(entry.Source.SourceType, typeof (ManualDataSource).Name)),
                    Variance = GetVariances(calculationId),
                    EntryErrors = entry
                        .Errors
                        .Where(error => error.Column == string.Empty)
                        .Select(error => error.Message),
                    CanRevert = (entry.Source != null) && (entry.Source.InputStatus == SourceStatus.Calculated),
                    CanEdit = (entry.Source != null) && (entry.Source.InputStatus == SourceStatus.PendingCalculation),
                    ViewName = viewName,
                    ReferenceNotes = (entry.Source == null) ? string.Empty : entry.Source.ReferenceNotes
                };

            return ViewInputType(viewName, model);
        }

        private IEnumerable<Variance> GetVariances(Guid? calculationId)
        {
            if (calculationId == null) return Enumerable.Empty<Variance>();
            return context
               .Variances
               .Where(variance => variance.CalculationId == calculationId)
               .ToArray();
        }

         [NonAction]
        private ActionResult ViewInputType(string viewName,EnterDataModel model)
        {
            return View("~/Views/Input/Calculations/" + viewName + ".cshtml", model);
        }

        [HttpGet]
        public ActionResult UploadFile()
        {
            var username = User.Identity.Name;
            var model = context.UserProfiles.FirstOrDefault(profile => profile.UserName == username);
            return View(model);
        }
    }
}
