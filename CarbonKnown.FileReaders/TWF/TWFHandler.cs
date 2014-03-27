using CarbonKnown.WCF.AirTravel;
using System;
using System.Text.RegularExpressions;

namespace CarbonKnown.FileReaders.TWF
{
    public sealed class TWFHandler : TravelHandlerBase
    {
        public TWFHandler(string host)
            : base(host)
        {
            MapColumns(c => c.StartDate, DateConversion,
                       "Tvl Date",
                       "Tvl_Date",
                       "Tvl-Date",
                       "TvlDate",
                       "Travel Date",
                       "Travel-Date",
                       "Travel_Date",
                       "TravelDate");
            MapColumns(c => c.CostCode, CostCodeConversion,
                       "Client 2nd Ref",
                       "Client_2nd_Ref",
                       "Client-2nd-Ref",
                       "Client2ndRef");
            MapMoneyColumns("Net Fare", "Net-Fare", "Net_Fare", "NetFare");
            MapUnitsColumns("Units");
            MapColumns(c => c.TravelType, TravelTypeConversion, "Type2");
            MapColumns(c => c.RouteDetails,
                       "Route Details",
                       "Route-Details",
                       "Route_Details",
                       "RouteDetails",
                       "Routing",
                       "Route");
            MapColumns(c => c.ClassCategory, ClassCategoryConversion,
                       "Class Category",
                       "Class-Category",
                       "Class_Category",
                       "ClassCategory");
        }

        private static readonly Regex CostCodeRegEx = new Regex(@"\d{8,9}", RegexOptions.Compiled);

        private static void TravelTypeConversion(TravelDataContract contract, object value)
        {
            var stringValue = string.Format("{0}", value).Trim().ToUpper();
            if (stringValue.Contains("AIR"))
            {
                contract.TravelType = TravelType.AirTravel;
            }
            if (stringValue.Contains("HOTEL"))
            {
                contract.TravelType = TravelType.Hotel;
            }
        }

        private static void ClassCategoryConversion(TravelDataContract contract, object value)
        {
            var stringValue = string.Format("{0}", value).Trim();
            if (string.Equals(stringValue, "Eco", StringComparison.InvariantCultureIgnoreCase))
            {
                contract.ClassCategory = TravelClass.Economy;
                return;
            }
            if (string.Equals(stringValue, "Bus", StringComparison.InvariantCultureIgnoreCase))
            {
                contract.ClassCategory = TravelClass.Business;
                return;
            }
            contract.ClassCategory = TravelClass.Average;
        }

        private static void DateConversion(TravelDataContract contract, object value)
        {
            var datetime = TryParser.DateTime(value);
            contract.StartDate = datetime;
            contract.EndDate = datetime;
        }

        private static void CostCodeConversion(TravelDataContract contract, object value)
        {
            var stringValue = string.Format("{0}", value);
            if (CostCodeRegEx.IsMatch(stringValue))
            {
                contract.CostCode = CostCodeRegEx.Match(stringValue).Value;
            }
        }
    }
}
