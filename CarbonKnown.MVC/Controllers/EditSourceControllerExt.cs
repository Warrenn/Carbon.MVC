using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Accommodation;
using CarbonKnown.DAL.Models.AirTravel;
using CarbonKnown.DAL.Models.CarHire;
using CarbonKnown.DAL.Models.Commuting;
using CarbonKnown.DAL.Models.Courier;
using CarbonKnown.DAL.Models.Electricity;
using CarbonKnown.DAL.Models.Fleet;
using CarbonKnown.DAL.Models.Fuel;
using CarbonKnown.DAL.Models.Paper;
using CarbonKnown.DAL.Models.Refrigerant;
using CarbonKnown.DAL.Models.Waste;
using CarbonKnown.DAL.Models.Water;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Models;
using CarbonKnown.FileReaders;

namespace CarbonKnown.MVC.Controllers
{
    public partial class EditSourceController
    {
        [HttpGet]
        public ActionResult AccommodationData(DataTableParamModel request, Guid sourceId)
        {
            var builder = new DataTableResultModelBuilder<AccommodationData>();
            builder.AddQueryable(context.Set<AccommodationData>()
			.Where(data => data.SourceId == sourceId));
            var columnIndex = new List<string>
                {
                    "",
                    "",
                    "",
                    "StartDate",
                    "EndDate",
                    "CostCode",
                    "Money",
                    "Units",
                };
            builder.AddDataExpression(data => new object[]
                {
                    data.Id.ToString(),
                    data.Errors.Select(error => new
                        {
                            error.Column, 
                            error.Message, 
                            error.ErrorType,
                            index = columnIndex.IndexOf(error.Column)
                        }).ToArray(),
                    ConvertToString(data.RowNo),
                    ConvertToString(data.StartDate),
                    ConvertToString(data.EndDate),
                    ConvertToString(data.CostCode),
                    ConvertToString(data.Money),
                    ConvertToString(data.Units),
                });
            var searchSet = false;
            int numeric;
            DataErrorType errorType;
            if (!(int.TryParse(request.sSearch, out numeric)) && Enum.TryParse(request.sSearch, true, out errorType))
            {
                searchSet = true;
                builder.AddSearchFilter(data => (data.Errors.Any(error => error.ErrorType == errorType)));
            }
			if(string.Equals("AllErrors",request.sSearch,StringComparison.InvariantCultureIgnoreCase)){
                searchSet = true;
                builder.AddSearchFilter(data => data.Errors.Any());
			}
            DateTime searchDate;
            if ((!searchSet) && (DateTime.TryParse(request.sSearch,CultureInfo.CurrentCulture,DateTimeStyles.None,  out searchDate)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.StartDate == searchDate) ||
                                                                  (data.EndDate == searchDate));
            }
            int searchRow;
            if ((!searchSet) && (int.TryParse(request.sSearch,  out searchRow)))
            {
                builder.AddSearchFilter(data =>(data.RowNo == searchRow));
            }
            decimal amount;
            if ((!searchSet) && (decimal.TryParse(request.sSearch,  out amount)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.Money == amount) ||
                                                                  (data.Units == amount));
            }
            if (!searchSet)
            {
                builder.AddSearchFilter(data => data.CostCode.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.RowNo);
            builder.AddSortExpression(data => data.StartDate);
            builder.AddSortExpression(data => data.EndDate);
            builder.AddSortExpression(data => data.CostCode);
            builder.AddSortExpression(data => data.Money);
            builder.AddSortExpression(data => data.Units);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AirTravelRouteData(DataTableParamModel request, Guid sourceId)
        {
            var builder = new DataTableResultModelBuilder<AirTravelRouteData>();
            builder.AddQueryable(context.Set<AirTravelRouteData>()
			.Where(data => data.SourceId == sourceId));
            var columnIndex = new List<string>
                {
                    "",
                    "",
                    "",
                    "StartDate",
                    "EndDate",
                    "CostCode",
                    "Money",
                    "Units",
                    "TravelClass",
                    "Reversal",
                    "FromCode",
                    "ToCode",
                };
            builder.AddDataExpression(data => new object[]
                {
                    data.Id.ToString(),
                    data.Errors.Select(error => new
                        {
                            error.Column, 
                            error.Message, 
                            error.ErrorType,
                            index = columnIndex.IndexOf(error.Column)
                        }).ToArray(),
                    ConvertToString(data.RowNo),
                    ConvertToString(data.StartDate),
                    ConvertToString(data.EndDate),
                    ConvertToString(data.CostCode),
                    ConvertToString(data.Money),
                    ConvertToString(data.Units),
                    ConvertToString(data.TravelClass),
                    ConvertToString(data.Reversal),
                    ConvertToString(data.FromCode),
                    ConvertToString(data.ToCode),
                });
            var searchSet = false;
            int numeric;
            DataErrorType errorType;
            if (!(int.TryParse(request.sSearch, out numeric)) && Enum.TryParse(request.sSearch, true, out errorType))
            {
                searchSet = true;
                builder.AddSearchFilter(data => (data.Errors.Any(error => error.ErrorType == errorType)));
            }
			if(string.Equals("AllErrors",request.sSearch,StringComparison.InvariantCultureIgnoreCase)){
                searchSet = true;
                builder.AddSearchFilter(data => data.Errors.Any());
			}
			var travelclass = TryParser.Nullable<TravelClass>(request.sSearch);
            if((!searchSet) && (travelclass !=null))
			{
                builder.AddSearchFilter(data => data.TravelClass == travelclass);
			}
			var reversal = TryParser.Nullable<Boolean>(request.sSearch);
            if((!searchSet) && (reversal !=null))
			{
                builder.AddSearchFilter(data => data.Reversal == reversal);
			}
            if (!searchSet)
            {
                builder.AddSearchFilter(data =>data.FromCode.Contains(request.sSearch));
			}
            if (!searchSet)
            {
                builder.AddSearchFilter(data =>data.ToCode.Contains(request.sSearch));
			}
            DateTime searchDate;
            if ((!searchSet) && (DateTime.TryParse(request.sSearch,CultureInfo.CurrentCulture,DateTimeStyles.None,  out searchDate)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.StartDate == searchDate) ||
                                                                  (data.EndDate == searchDate));
            }
            int searchRow;
            if ((!searchSet) && (int.TryParse(request.sSearch,  out searchRow)))
            {
                builder.AddSearchFilter(data =>(data.RowNo == searchRow));
            }
            decimal amount;
            if ((!searchSet) && (decimal.TryParse(request.sSearch,  out amount)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.Money == amount) ||
                                                                  (data.Units == amount));
            }
            if (!searchSet)
            {
                builder.AddSearchFilter(data => data.CostCode.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.RowNo);
            builder.AddSortExpression(data => data.StartDate);
            builder.AddSortExpression(data => data.EndDate);
            builder.AddSortExpression(data => data.CostCode);
            builder.AddSortExpression(data => data.Money);
            builder.AddSortExpression(data => data.Units);
            builder.AddSortExpression(data => data.TravelClass);
            builder.AddSortExpression(data => data.Reversal);
            builder.AddSortExpression(data => data.FromCode);
            builder.AddSortExpression(data => data.ToCode);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AirTravelData(DataTableParamModel request, Guid sourceId)
        {
            var builder = new DataTableResultModelBuilder<AirTravelData>();
            builder.AddQueryable(context.Set<AirTravelData>()
			.Where(data => data.SourceId == sourceId));
            var columnIndex = new List<string>
                {
                    "",
                    "",
                    "",
                    "StartDate",
                    "EndDate",
                    "CostCode",
                    "Money",
                    "Units",
                    "TravelClass",
                };
            builder.AddDataExpression(data => new object[]
                {
                    data.Id.ToString(),
                    data.Errors.Select(error => new
                        {
                            error.Column, 
                            error.Message, 
                            error.ErrorType,
                            index = columnIndex.IndexOf(error.Column)
                        }).ToArray(),
                    ConvertToString(data.RowNo),
                    ConvertToString(data.StartDate),
                    ConvertToString(data.EndDate),
                    ConvertToString(data.CostCode),
                    ConvertToString(data.Money),
                    ConvertToString(data.Units),
                    ConvertToString(data.TravelClass),
                });
            var searchSet = false;
            int numeric;
            DataErrorType errorType;
            if (!(int.TryParse(request.sSearch, out numeric)) && Enum.TryParse(request.sSearch, true, out errorType))
            {
                searchSet = true;
                builder.AddSearchFilter(data => (data.Errors.Any(error => error.ErrorType == errorType)));
            }
			if(string.Equals("AllErrors",request.sSearch,StringComparison.InvariantCultureIgnoreCase)){
                searchSet = true;
                builder.AddSearchFilter(data => data.Errors.Any());
			}
			var travelclass = TryParser.Nullable<TravelClass>(request.sSearch);
            if((!searchSet) && (travelclass !=null))
			{
                builder.AddSearchFilter(data => data.TravelClass == travelclass);
			}
            DateTime searchDate;
            if ((!searchSet) && (DateTime.TryParse(request.sSearch,CultureInfo.CurrentCulture,DateTimeStyles.None,  out searchDate)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.StartDate == searchDate) ||
                                                                  (data.EndDate == searchDate));
            }
            int searchRow;
            if ((!searchSet) && (int.TryParse(request.sSearch,  out searchRow)))
            {
                builder.AddSearchFilter(data =>(data.RowNo == searchRow));
            }
            decimal amount;
            if ((!searchSet) && (decimal.TryParse(request.sSearch,  out amount)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.Money == amount) ||
                                                                  (data.Units == amount));
            }
            if (!searchSet)
            {
                builder.AddSearchFilter(data => data.CostCode.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.RowNo);
            builder.AddSortExpression(data => data.StartDate);
            builder.AddSortExpression(data => data.EndDate);
            builder.AddSortExpression(data => data.CostCode);
            builder.AddSortExpression(data => data.Money);
            builder.AddSortExpression(data => data.Units);
            builder.AddSortExpression(data => data.TravelClass);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CarHireData(DataTableParamModel request, Guid sourceId)
        {
            var builder = new DataTableResultModelBuilder<CarHireData>();
            builder.AddQueryable(context.Set<CarHireData>()
			.Where(data => data.SourceId == sourceId));
            var columnIndex = new List<string>
                {
                    "",
                    "",
                    "",
                    "StartDate",
                    "EndDate",
                    "CostCode",
                    "Money",
                    "Units",
                    "CarGroupBill",
                };
            builder.AddDataExpression(data => new object[]
                {
                    data.Id.ToString(),
                    data.Errors.Select(error => new
                        {
                            error.Column, 
                            error.Message, 
                            error.ErrorType,
                            index = columnIndex.IndexOf(error.Column)
                        }).ToArray(),
                    ConvertToString(data.RowNo),
                    ConvertToString(data.StartDate),
                    ConvertToString(data.EndDate),
                    ConvertToString(data.CostCode),
                    ConvertToString(data.Money),
                    ConvertToString(data.Units),
                    ConvertToString(data.CarGroupBill),
                });
            var searchSet = false;
            int numeric;
            DataErrorType errorType;
            if (!(int.TryParse(request.sSearch, out numeric)) && Enum.TryParse(request.sSearch, true, out errorType))
            {
                searchSet = true;
                builder.AddSearchFilter(data => (data.Errors.Any(error => error.ErrorType == errorType)));
            }
			if(string.Equals("AllErrors",request.sSearch,StringComparison.InvariantCultureIgnoreCase)){
                searchSet = true;
                builder.AddSearchFilter(data => data.Errors.Any());
			}
			var cargroupbill = TryParser.Nullable<CarGroupBill>(request.sSearch);
            if((!searchSet) && (cargroupbill !=null))
			{
                builder.AddSearchFilter(data => data.CarGroupBill == cargroupbill);
			}
            DateTime searchDate;
            if ((!searchSet) && (DateTime.TryParse(request.sSearch,CultureInfo.CurrentCulture,DateTimeStyles.None,  out searchDate)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.StartDate == searchDate) ||
                                                                  (data.EndDate == searchDate));
            }
            int searchRow;
            if ((!searchSet) && (int.TryParse(request.sSearch,  out searchRow)))
            {
                builder.AddSearchFilter(data =>(data.RowNo == searchRow));
            }
            decimal amount;
            if ((!searchSet) && (decimal.TryParse(request.sSearch,  out amount)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.Money == amount) ||
                                                                  (data.Units == amount));
            }
            if (!searchSet)
            {
                builder.AddSearchFilter(data => data.CostCode.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.RowNo);
            builder.AddSortExpression(data => data.StartDate);
            builder.AddSortExpression(data => data.EndDate);
            builder.AddSortExpression(data => data.CostCode);
            builder.AddSortExpression(data => data.Money);
            builder.AddSortExpression(data => data.Units);
            builder.AddSortExpression(data => data.CarGroupBill);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CommutingData(DataTableParamModel request, Guid sourceId)
        {
            var builder = new DataTableResultModelBuilder<CommutingData>();
            builder.AddQueryable(context.Set<CommutingData>()
			.Where(data => data.SourceId == sourceId));
            var columnIndex = new List<string>
                {
                    "",
                    "",
                    "",
                    "StartDate",
                    "EndDate",
                    "CostCode",
                    "Money",
                    "Units",
                    "CommutingType",
                };
            builder.AddDataExpression(data => new object[]
                {
                    data.Id.ToString(),
                    data.Errors.Select(error => new
                        {
                            error.Column, 
                            error.Message, 
                            error.ErrorType,
                            index = columnIndex.IndexOf(error.Column)
                        }).ToArray(),
                    ConvertToString(data.RowNo),
                    ConvertToString(data.StartDate),
                    ConvertToString(data.EndDate),
                    ConvertToString(data.CostCode),
                    ConvertToString(data.Money),
                    ConvertToString(data.Units),
                    ConvertToString(data.CommutingType),
                });
            var searchSet = false;
            int numeric;
            DataErrorType errorType;
            if (!(int.TryParse(request.sSearch, out numeric)) && Enum.TryParse(request.sSearch, true, out errorType))
            {
                searchSet = true;
                builder.AddSearchFilter(data => (data.Errors.Any(error => error.ErrorType == errorType)));
            }
			if(string.Equals("AllErrors",request.sSearch,StringComparison.InvariantCultureIgnoreCase)){
                searchSet = true;
                builder.AddSearchFilter(data => data.Errors.Any());
			}
			var commutingtype = TryParser.Nullable<CommutingType>(request.sSearch);
            if((!searchSet) && (commutingtype !=null))
			{
                builder.AddSearchFilter(data => data.CommutingType == commutingtype);
			}
            DateTime searchDate;
            if ((!searchSet) && (DateTime.TryParse(request.sSearch,CultureInfo.CurrentCulture,DateTimeStyles.None,  out searchDate)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.StartDate == searchDate) ||
                                                                  (data.EndDate == searchDate));
            }
            int searchRow;
            if ((!searchSet) && (int.TryParse(request.sSearch,  out searchRow)))
            {
                builder.AddSearchFilter(data =>(data.RowNo == searchRow));
            }
            decimal amount;
            if ((!searchSet) && (decimal.TryParse(request.sSearch,  out amount)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.Money == amount) ||
                                                                  (data.Units == amount));
            }
            if (!searchSet)
            {
                builder.AddSearchFilter(data => data.CostCode.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.RowNo);
            builder.AddSortExpression(data => data.StartDate);
            builder.AddSortExpression(data => data.EndDate);
            builder.AddSortExpression(data => data.CostCode);
            builder.AddSortExpression(data => data.Money);
            builder.AddSortExpression(data => data.Units);
            builder.AddSortExpression(data => data.CommutingType);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CourierRouteData(DataTableParamModel request, Guid sourceId)
        {
            var builder = new DataTableResultModelBuilder<CourierRouteData>();
            builder.AddQueryable(context.Set<CourierRouteData>()
			.Where(data => data.SourceId == sourceId));
            var columnIndex = new List<string>
                {
                    "",
                    "",
                    "",
                    "StartDate",
                    "EndDate",
                    "CostCode",
                    "Money",
                    "Units",
                    "ServiceType",
                    "ChargeMass",
                    "FromCode",
                    "ToCode",
                    "Reversal",
                };
            builder.AddDataExpression(data => new object[]
                {
                    data.Id.ToString(),
                    data.Errors.Select(error => new
                        {
                            error.Column, 
                            error.Message, 
                            error.ErrorType,
                            index = columnIndex.IndexOf(error.Column)
                        }).ToArray(),
                    ConvertToString(data.RowNo),
                    ConvertToString(data.StartDate),
                    ConvertToString(data.EndDate),
                    ConvertToString(data.CostCode),
                    ConvertToString(data.Money),
                    ConvertToString(data.Units),
                    ConvertToString(data.ServiceType),
                    ConvertToString(data.ChargeMass),
                    ConvertToString(data.FromCode),
                    ConvertToString(data.ToCode),
                    ConvertToString(data.Reversal),
                });
            var searchSet = false;
            int numeric;
            DataErrorType errorType;
            if (!(int.TryParse(request.sSearch, out numeric)) && Enum.TryParse(request.sSearch, true, out errorType))
            {
                searchSet = true;
                builder.AddSearchFilter(data => (data.Errors.Any(error => error.ErrorType == errorType)));
            }
			if(string.Equals("AllErrors",request.sSearch,StringComparison.InvariantCultureIgnoreCase)){
                searchSet = true;
                builder.AddSearchFilter(data => data.Errors.Any());
			}
			var servicetype = TryParser.Nullable<ServiceType>(request.sSearch);
            if((!searchSet) && (servicetype !=null))
			{
                builder.AddSearchFilter(data => data.ServiceType == servicetype);
			}
			var chargemass = TryParser.Nullable<Decimal>(request.sSearch);
            if((!searchSet) && (chargemass !=null))
			{
                builder.AddSearchFilter(data => data.ChargeMass == chargemass);
			}
            if (!searchSet)
            {
                builder.AddSearchFilter(data =>data.FromCode.Contains(request.sSearch));
			}
            if (!searchSet)
            {
                builder.AddSearchFilter(data =>data.ToCode.Contains(request.sSearch));
			}
			var reversal = TryParser.Nullable<Boolean>(request.sSearch);
            if((!searchSet) && (reversal !=null))
			{
                builder.AddSearchFilter(data => data.Reversal == reversal);
			}
            DateTime searchDate;
            if ((!searchSet) && (DateTime.TryParse(request.sSearch,CultureInfo.CurrentCulture,DateTimeStyles.None,  out searchDate)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.StartDate == searchDate) ||
                                                                  (data.EndDate == searchDate));
            }
            int searchRow;
            if ((!searchSet) && (int.TryParse(request.sSearch,  out searchRow)))
            {
                builder.AddSearchFilter(data =>(data.RowNo == searchRow));
            }
            decimal amount;
            if ((!searchSet) && (decimal.TryParse(request.sSearch,  out amount)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.Money == amount) ||
                                                                  (data.Units == amount));
            }
            if (!searchSet)
            {
                builder.AddSearchFilter(data => data.CostCode.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.RowNo);
            builder.AddSortExpression(data => data.StartDate);
            builder.AddSortExpression(data => data.EndDate);
            builder.AddSortExpression(data => data.CostCode);
            builder.AddSortExpression(data => data.Money);
            builder.AddSortExpression(data => data.Units);
            builder.AddSortExpression(data => data.ServiceType);
            builder.AddSortExpression(data => data.ChargeMass);
            builder.AddSortExpression(data => data.FromCode);
            builder.AddSortExpression(data => data.ToCode);
            builder.AddSortExpression(data => data.Reversal);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CourierData(DataTableParamModel request, Guid sourceId)
        {
            var builder = new DataTableResultModelBuilder<CourierData>();
            builder.AddQueryable(context.Set<CourierData>()
			.Where(data => data.SourceId == sourceId));
            var columnIndex = new List<string>
                {
                    "",
                    "",
                    "",
                    "StartDate",
                    "EndDate",
                    "CostCode",
                    "Money",
                    "Units",
                    "ServiceType",
                    "ChargeMass",
                };
            builder.AddDataExpression(data => new object[]
                {
                    data.Id.ToString(),
                    data.Errors.Select(error => new
                        {
                            error.Column, 
                            error.Message, 
                            error.ErrorType,
                            index = columnIndex.IndexOf(error.Column)
                        }).ToArray(),
                    ConvertToString(data.RowNo),
                    ConvertToString(data.StartDate),
                    ConvertToString(data.EndDate),
                    ConvertToString(data.CostCode),
                    ConvertToString(data.Money),
                    ConvertToString(data.Units),
                    ConvertToString(data.ServiceType),
                    ConvertToString(data.ChargeMass),
                });
            var searchSet = false;
            int numeric;
            DataErrorType errorType;
            if (!(int.TryParse(request.sSearch, out numeric)) && Enum.TryParse(request.sSearch, true, out errorType))
            {
                searchSet = true;
                builder.AddSearchFilter(data => (data.Errors.Any(error => error.ErrorType == errorType)));
            }
			if(string.Equals("AllErrors",request.sSearch,StringComparison.InvariantCultureIgnoreCase)){
                searchSet = true;
                builder.AddSearchFilter(data => data.Errors.Any());
			}
			var servicetype = TryParser.Nullable<ServiceType>(request.sSearch);
            if((!searchSet) && (servicetype !=null))
			{
                builder.AddSearchFilter(data => data.ServiceType == servicetype);
			}
			var chargemass = TryParser.Nullable<Decimal>(request.sSearch);
            if((!searchSet) && (chargemass !=null))
			{
                builder.AddSearchFilter(data => data.ChargeMass == chargemass);
			}
            DateTime searchDate;
            if ((!searchSet) && (DateTime.TryParse(request.sSearch,CultureInfo.CurrentCulture,DateTimeStyles.None,  out searchDate)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.StartDate == searchDate) ||
                                                                  (data.EndDate == searchDate));
            }
            int searchRow;
            if ((!searchSet) && (int.TryParse(request.sSearch,  out searchRow)))
            {
                builder.AddSearchFilter(data =>(data.RowNo == searchRow));
            }
            decimal amount;
            if ((!searchSet) && (decimal.TryParse(request.sSearch,  out amount)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.Money == amount) ||
                                                                  (data.Units == amount));
            }
            if (!searchSet)
            {
                builder.AddSearchFilter(data => data.CostCode.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.RowNo);
            builder.AddSortExpression(data => data.StartDate);
            builder.AddSortExpression(data => data.EndDate);
            builder.AddSortExpression(data => data.CostCode);
            builder.AddSortExpression(data => data.Money);
            builder.AddSortExpression(data => data.Units);
            builder.AddSortExpression(data => data.ServiceType);
            builder.AddSortExpression(data => data.ChargeMass);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ElectricityData(DataTableParamModel request, Guid sourceId)
        {
            var builder = new DataTableResultModelBuilder<ElectricityData>();
            builder.AddQueryable(context.Set<ElectricityData>()
			.Where(data => data.SourceId == sourceId));
            var columnIndex = new List<string>
                {
                    "",
                    "",
                    "",
                    "StartDate",
                    "EndDate",
                    "CostCode",
                    "Money",
                    "Units",
                    "ElectricityType",
                };
            builder.AddDataExpression(data => new object[]
                {
                    data.Id.ToString(),
                    data.Errors.Select(error => new
                        {
                            error.Column, 
                            error.Message, 
                            error.ErrorType,
                            index = columnIndex.IndexOf(error.Column)
                        }).ToArray(),
                    ConvertToString(data.RowNo),
                    ConvertToString(data.StartDate),
                    ConvertToString(data.EndDate),
                    ConvertToString(data.CostCode),
                    ConvertToString(data.Money),
                    ConvertToString(data.Units),
                    ConvertToString(data.ElectricityType),
                });
            var searchSet = false;
            int numeric;
            DataErrorType errorType;
            if (!(int.TryParse(request.sSearch, out numeric)) && Enum.TryParse(request.sSearch, true, out errorType))
            {
                searchSet = true;
                builder.AddSearchFilter(data => (data.Errors.Any(error => error.ErrorType == errorType)));
            }
			if(string.Equals("AllErrors",request.sSearch,StringComparison.InvariantCultureIgnoreCase)){
                searchSet = true;
                builder.AddSearchFilter(data => data.Errors.Any());
			}
			var electricitytype = TryParser.Nullable<ElectricityType>(request.sSearch);
            if((!searchSet) && (electricitytype !=null))
			{
                builder.AddSearchFilter(data => data.ElectricityType == electricitytype);
			}
            DateTime searchDate;
            if ((!searchSet) && (DateTime.TryParse(request.sSearch,CultureInfo.CurrentCulture,DateTimeStyles.None,  out searchDate)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.StartDate == searchDate) ||
                                                                  (data.EndDate == searchDate));
            }
            int searchRow;
            if ((!searchSet) && (int.TryParse(request.sSearch,  out searchRow)))
            {
                builder.AddSearchFilter(data =>(data.RowNo == searchRow));
            }
            decimal amount;
            if ((!searchSet) && (decimal.TryParse(request.sSearch,  out amount)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.Money == amount) ||
                                                                  (data.Units == amount));
            }
            if (!searchSet)
            {
                builder.AddSearchFilter(data => data.CostCode.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.RowNo);
            builder.AddSortExpression(data => data.StartDate);
            builder.AddSortExpression(data => data.EndDate);
            builder.AddSortExpression(data => data.CostCode);
            builder.AddSortExpression(data => data.Money);
            builder.AddSortExpression(data => data.Units);
            builder.AddSortExpression(data => data.ElectricityType);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FuelData(DataTableParamModel request, Guid sourceId)
        {
            var builder = new DataTableResultModelBuilder<FuelData>();
            builder.AddQueryable(context.Set<FuelData>()
			.Where(data => data.SourceId == sourceId));
            var columnIndex = new List<string>
                {
                    "",
                    "",
                    "",
                    "StartDate",
                    "EndDate",
                    "CostCode",
                    "Money",
                    "Units",
                    "FuelType",
                    "UOM",
                };
            builder.AddDataExpression(data => new object[]
                {
                    data.Id.ToString(),
                    data.Errors.Select(error => new
                        {
                            error.Column, 
                            error.Message, 
                            error.ErrorType,
                            index = columnIndex.IndexOf(error.Column)
                        }).ToArray(),
                    ConvertToString(data.RowNo),
                    ConvertToString(data.StartDate),
                    ConvertToString(data.EndDate),
                    ConvertToString(data.CostCode),
                    ConvertToString(data.Money),
                    ConvertToString(data.Units),
                    ConvertToString(data.FuelType),
                    ConvertToString(data.UOM),
                });
            var searchSet = false;
            int numeric;
            DataErrorType errorType;
            if (!(int.TryParse(request.sSearch, out numeric)) && Enum.TryParse(request.sSearch, true, out errorType))
            {
                searchSet = true;
                builder.AddSearchFilter(data => (data.Errors.Any(error => error.ErrorType == errorType)));
            }
			if(string.Equals("AllErrors",request.sSearch,StringComparison.InvariantCultureIgnoreCase)){
                searchSet = true;
                builder.AddSearchFilter(data => data.Errors.Any());
			}
			var fueltype = TryParser.Nullable<FuelType>(request.sSearch);
            if((!searchSet) && (fueltype !=null))
			{
                builder.AddSearchFilter(data => data.FuelType == fueltype);
			}
			var uom = TryParser.Nullable<UnitOfMeasure>(request.sSearch);
            if((!searchSet) && (uom !=null))
			{
                builder.AddSearchFilter(data => data.UOM == uom);
			}
            DateTime searchDate;
            if ((!searchSet) && (DateTime.TryParse(request.sSearch,CultureInfo.CurrentCulture,DateTimeStyles.None,  out searchDate)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.StartDate == searchDate) ||
                                                                  (data.EndDate == searchDate));
            }
            int searchRow;
            if ((!searchSet) && (int.TryParse(request.sSearch,  out searchRow)))
            {
                builder.AddSearchFilter(data =>(data.RowNo == searchRow));
            }
            decimal amount;
            if ((!searchSet) && (decimal.TryParse(request.sSearch,  out amount)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.Money == amount) ||
                                                                  (data.Units == amount));
            }
            if (!searchSet)
            {
                builder.AddSearchFilter(data => data.CostCode.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.RowNo);
            builder.AddSortExpression(data => data.StartDate);
            builder.AddSortExpression(data => data.EndDate);
            builder.AddSortExpression(data => data.CostCode);
            builder.AddSortExpression(data => data.Money);
            builder.AddSortExpression(data => data.Units);
            builder.AddSortExpression(data => data.FuelType);
            builder.AddSortExpression(data => data.UOM);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PaperData(DataTableParamModel request, Guid sourceId)
        {
            var builder = new DataTableResultModelBuilder<PaperData>();
            builder.AddQueryable(context.Set<PaperData>()
			.Where(data => data.SourceId == sourceId));
            var columnIndex = new List<string>
                {
                    "",
                    "",
                    "",
                    "StartDate",
                    "EndDate",
                    "CostCode",
                    "Money",
                    "Units",
                    "PaperType",
                    "PaperUom",
                };
            builder.AddDataExpression(data => new object[]
                {
                    data.Id.ToString(),
                    data.Errors.Select(error => new
                        {
                            error.Column, 
                            error.Message, 
                            error.ErrorType,
                            index = columnIndex.IndexOf(error.Column)
                        }).ToArray(),
                    ConvertToString(data.RowNo),
                    ConvertToString(data.StartDate),
                    ConvertToString(data.EndDate),
                    ConvertToString(data.CostCode),
                    ConvertToString(data.Money),
                    ConvertToString(data.Units),
                    ConvertToString(data.PaperType),
                    ConvertToString(data.PaperUom),
                });
            var searchSet = false;
            int numeric;
            DataErrorType errorType;
            if (!(int.TryParse(request.sSearch, out numeric)) && Enum.TryParse(request.sSearch, true, out errorType))
            {
                searchSet = true;
                builder.AddSearchFilter(data => (data.Errors.Any(error => error.ErrorType == errorType)));
            }
			if(string.Equals("AllErrors",request.sSearch,StringComparison.InvariantCultureIgnoreCase)){
                searchSet = true;
                builder.AddSearchFilter(data => data.Errors.Any());
			}
			var papertype = TryParser.Nullable<PaperType>(request.sSearch);
            if((!searchSet) && (papertype !=null))
			{
                builder.AddSearchFilter(data => data.PaperType == papertype);
			}
			var paperuom = TryParser.Nullable<PaperUom>(request.sSearch);
            if((!searchSet) && (paperuom !=null))
			{
                builder.AddSearchFilter(data => data.PaperUom == paperuom);
			}
            DateTime searchDate;
            if ((!searchSet) && (DateTime.TryParse(request.sSearch,CultureInfo.CurrentCulture,DateTimeStyles.None,  out searchDate)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.StartDate == searchDate) ||
                                                                  (data.EndDate == searchDate));
            }
            int searchRow;
            if ((!searchSet) && (int.TryParse(request.sSearch,  out searchRow)))
            {
                builder.AddSearchFilter(data =>(data.RowNo == searchRow));
            }
            decimal amount;
            if ((!searchSet) && (decimal.TryParse(request.sSearch,  out amount)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.Money == amount) ||
                                                                  (data.Units == amount));
            }
            if (!searchSet)
            {
                builder.AddSearchFilter(data => data.CostCode.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.RowNo);
            builder.AddSortExpression(data => data.StartDate);
            builder.AddSortExpression(data => data.EndDate);
            builder.AddSortExpression(data => data.CostCode);
            builder.AddSortExpression(data => data.Money);
            builder.AddSortExpression(data => data.Units);
            builder.AddSortExpression(data => data.PaperType);
            builder.AddSortExpression(data => data.PaperUom);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult RefrigerantData(DataTableParamModel request, Guid sourceId)
        {
            var builder = new DataTableResultModelBuilder<RefrigerantData>();
            builder.AddQueryable(context.Set<RefrigerantData>()
			.Where(data => data.SourceId == sourceId));
            var columnIndex = new List<string>
                {
                    "",
                    "",
                    "",
                    "StartDate",
                    "EndDate",
                    "CostCode",
                    "Money",
                    "Units",
                    "RefrigerantType",
                };
            builder.AddDataExpression(data => new object[]
                {
                    data.Id.ToString(),
                    data.Errors.Select(error => new
                        {
                            error.Column, 
                            error.Message, 
                            error.ErrorType,
                            index = columnIndex.IndexOf(error.Column)
                        }).ToArray(),
                    ConvertToString(data.RowNo),
                    ConvertToString(data.StartDate),
                    ConvertToString(data.EndDate),
                    ConvertToString(data.CostCode),
                    ConvertToString(data.Money),
                    ConvertToString(data.Units),
                    ConvertToString(data.RefrigerantType),
                });
            var searchSet = false;
            int numeric;
            DataErrorType errorType;
            if (!(int.TryParse(request.sSearch, out numeric)) && Enum.TryParse(request.sSearch, true, out errorType))
            {
                searchSet = true;
                builder.AddSearchFilter(data => (data.Errors.Any(error => error.ErrorType == errorType)));
            }
			if(string.Equals("AllErrors",request.sSearch,StringComparison.InvariantCultureIgnoreCase)){
                searchSet = true;
                builder.AddSearchFilter(data => data.Errors.Any());
			}
			var refrigeranttype = TryParser.Nullable<RefrigerantType>(request.sSearch);
            if((!searchSet) && (refrigeranttype !=null))
			{
                builder.AddSearchFilter(data => data.RefrigerantType == refrigeranttype);
			}
            DateTime searchDate;
            if ((!searchSet) && (DateTime.TryParse(request.sSearch,CultureInfo.CurrentCulture,DateTimeStyles.None,  out searchDate)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.StartDate == searchDate) ||
                                                                  (data.EndDate == searchDate));
            }
            int searchRow;
            if ((!searchSet) && (int.TryParse(request.sSearch,  out searchRow)))
            {
                builder.AddSearchFilter(data =>(data.RowNo == searchRow));
            }
            decimal amount;
            if ((!searchSet) && (decimal.TryParse(request.sSearch,  out amount)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.Money == amount) ||
                                                                  (data.Units == amount));
            }
            if (!searchSet)
            {
                builder.AddSearchFilter(data => data.CostCode.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.RowNo);
            builder.AddSortExpression(data => data.StartDate);
            builder.AddSortExpression(data => data.EndDate);
            builder.AddSortExpression(data => data.CostCode);
            builder.AddSortExpression(data => data.Money);
            builder.AddSortExpression(data => data.Units);
            builder.AddSortExpression(data => data.RefrigerantType);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FleetData(DataTableParamModel request, Guid sourceId)
        {
            var builder = new DataTableResultModelBuilder<FleetData>();
            builder.AddQueryable(context.Set<FleetData>()
			.Where(data => data.SourceId == sourceId));
            var columnIndex = new List<string>
                {
                    "",
                    "",
                    "",
                    "StartDate",
                    "EndDate",
                    "CostCode",
                    "Money",
                    "Units",
                    "Scope",
                    "FuelType",
                };
            builder.AddDataExpression(data => new object[]
                {
                    data.Id.ToString(),
                    data.Errors.Select(error => new
                        {
                            error.Column, 
                            error.Message, 
                            error.ErrorType,
                            index = columnIndex.IndexOf(error.Column)
                        }).ToArray(),
                    ConvertToString(data.RowNo),
                    ConvertToString(data.StartDate),
                    ConvertToString(data.EndDate),
                    ConvertToString(data.CostCode),
                    ConvertToString(data.Money),
                    ConvertToString(data.Units),
                    ConvertToString(data.Scope),
                    ConvertToString(data.FuelType),
                });
            var searchSet = false;
            int numeric;
            DataErrorType errorType;
            if (!(int.TryParse(request.sSearch, out numeric)) && Enum.TryParse(request.sSearch, true, out errorType))
            {
                searchSet = true;
                builder.AddSearchFilter(data => (data.Errors.Any(error => error.ErrorType == errorType)));
            }
			if(string.Equals("AllErrors",request.sSearch,StringComparison.InvariantCultureIgnoreCase)){
                searchSet = true;
                builder.AddSearchFilter(data => data.Errors.Any());
			}
			var scope = TryParser.Nullable<FleetScope>(request.sSearch);
            if((!searchSet) && (scope !=null))
			{
                builder.AddSearchFilter(data => data.Scope == scope);
			}
			var fueltype = TryParser.Nullable<FuelType>(request.sSearch);
            if((!searchSet) && (fueltype !=null))
			{
                builder.AddSearchFilter(data => data.FuelType == fueltype);
			}
            DateTime searchDate;
            if ((!searchSet) && (DateTime.TryParse(request.sSearch,CultureInfo.CurrentCulture,DateTimeStyles.None,  out searchDate)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.StartDate == searchDate) ||
                                                                  (data.EndDate == searchDate));
            }
            int searchRow;
            if ((!searchSet) && (int.TryParse(request.sSearch,  out searchRow)))
            {
                builder.AddSearchFilter(data =>(data.RowNo == searchRow));
            }
            decimal amount;
            if ((!searchSet) && (decimal.TryParse(request.sSearch,  out amount)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.Money == amount) ||
                                                                  (data.Units == amount));
            }
            if (!searchSet)
            {
                builder.AddSearchFilter(data => data.CostCode.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.RowNo);
            builder.AddSortExpression(data => data.StartDate);
            builder.AddSortExpression(data => data.EndDate);
            builder.AddSortExpression(data => data.CostCode);
            builder.AddSortExpression(data => data.Money);
            builder.AddSortExpression(data => data.Units);
            builder.AddSortExpression(data => data.Scope);
            builder.AddSortExpression(data => data.FuelType);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult WasteData(DataTableParamModel request, Guid sourceId)
        {
            var builder = new DataTableResultModelBuilder<WasteData>();
            builder.AddQueryable(context.Set<WasteData>()
			.Where(data => data.SourceId == sourceId));
            var columnIndex = new List<string>
                {
                    "",
                    "",
                    "",
                    "StartDate",
                    "EndDate",
                    "CostCode",
                    "Money",
                    "Units",
                    "WasteType",
                };
            builder.AddDataExpression(data => new object[]
                {
                    data.Id.ToString(),
                    data.Errors.Select(error => new
                        {
                            error.Column, 
                            error.Message, 
                            error.ErrorType,
                            index = columnIndex.IndexOf(error.Column)
                        }).ToArray(),
                    ConvertToString(data.RowNo),
                    ConvertToString(data.StartDate),
                    ConvertToString(data.EndDate),
                    ConvertToString(data.CostCode),
                    ConvertToString(data.Money),
                    ConvertToString(data.Units),
                    ConvertToString(data.WasteType),
                });
            var searchSet = false;
            int numeric;
            DataErrorType errorType;
            if (!(int.TryParse(request.sSearch, out numeric)) && Enum.TryParse(request.sSearch, true, out errorType))
            {
                searchSet = true;
                builder.AddSearchFilter(data => (data.Errors.Any(error => error.ErrorType == errorType)));
            }
			if(string.Equals("AllErrors",request.sSearch,StringComparison.InvariantCultureIgnoreCase)){
                searchSet = true;
                builder.AddSearchFilter(data => data.Errors.Any());
			}
			var wastetype = TryParser.Nullable<WasteType>(request.sSearch);
            if((!searchSet) && (wastetype !=null))
			{
                builder.AddSearchFilter(data => data.WasteType == wastetype);
			}
            DateTime searchDate;
            if ((!searchSet) && (DateTime.TryParse(request.sSearch,CultureInfo.CurrentCulture,DateTimeStyles.None,  out searchDate)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.StartDate == searchDate) ||
                                                                  (data.EndDate == searchDate));
            }
            int searchRow;
            if ((!searchSet) && (int.TryParse(request.sSearch,  out searchRow)))
            {
                builder.AddSearchFilter(data =>(data.RowNo == searchRow));
            }
            decimal amount;
            if ((!searchSet) && (decimal.TryParse(request.sSearch,  out amount)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.Money == amount) ||
                                                                  (data.Units == amount));
            }
            if (!searchSet)
            {
                builder.AddSearchFilter(data => data.CostCode.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.RowNo);
            builder.AddSortExpression(data => data.StartDate);
            builder.AddSortExpression(data => data.EndDate);
            builder.AddSortExpression(data => data.CostCode);
            builder.AddSortExpression(data => data.Money);
            builder.AddSortExpression(data => data.Units);
            builder.AddSortExpression(data => data.WasteType);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult WaterData(DataTableParamModel request, Guid sourceId)
        {
            var builder = new DataTableResultModelBuilder<WaterData>();
            builder.AddQueryable(context.Set<WaterData>()
			.Where(data => data.SourceId == sourceId));
            var columnIndex = new List<string>
                {
                    "",
                    "",
                    "",
                    "StartDate",
                    "EndDate",
                    "CostCode",
                    "Money",
                    "Units",
                };
            builder.AddDataExpression(data => new object[]
                {
                    data.Id.ToString(),
                    data.Errors.Select(error => new
                        {
                            error.Column, 
                            error.Message, 
                            error.ErrorType,
                            index = columnIndex.IndexOf(error.Column)
                        }).ToArray(),
                    ConvertToString(data.RowNo),
                    ConvertToString(data.StartDate),
                    ConvertToString(data.EndDate),
                    ConvertToString(data.CostCode),
                    ConvertToString(data.Money),
                    ConvertToString(data.Units),
                });
            var searchSet = false;
            int numeric;
            DataErrorType errorType;
            if (!(int.TryParse(request.sSearch, out numeric)) && Enum.TryParse(request.sSearch, true, out errorType))
            {
                searchSet = true;
                builder.AddSearchFilter(data => (data.Errors.Any(error => error.ErrorType == errorType)));
            }
			if(string.Equals("AllErrors",request.sSearch,StringComparison.InvariantCultureIgnoreCase)){
                searchSet = true;
                builder.AddSearchFilter(data => data.Errors.Any());
			}
            DateTime searchDate;
            if ((!searchSet) && (DateTime.TryParse(request.sSearch,CultureInfo.CurrentCulture,DateTimeStyles.None,  out searchDate)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.StartDate == searchDate) ||
                                                                  (data.EndDate == searchDate));
            }
            int searchRow;
            if ((!searchSet) && (int.TryParse(request.sSearch,  out searchRow)))
            {
                builder.AddSearchFilter(data =>(data.RowNo == searchRow));
            }
            decimal amount;
            if ((!searchSet) && (decimal.TryParse(request.sSearch,  out amount)))
            {
                builder.AddSearchFilter(data =>
                                                                  (data.Money == amount) ||
                                                                  (data.Units == amount));
            }
            if (!searchSet)
            {
                builder.AddSearchFilter(data => data.CostCode.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.Id);
            builder.AddSortExpression(data => data.RowNo);
            builder.AddSortExpression(data => data.StartDate);
            builder.AddSortExpression(data => data.EndDate);
            builder.AddSortExpression(data => data.CostCode);
            builder.AddSortExpression(data => data.Money);
            builder.AddSortExpression(data => data.Units);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
