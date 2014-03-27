using System;
using CarbonKnown.FileReaders.TWF;

namespace CarbonKnown.FileReaders.Rennies
{
    public sealed class RenniesHandler : TravelHandlerBase
    {
        public RenniesHandler(string host)
            : base(host)
        {
            MapCostCodeColumns();
            MapMoneyColumns("TOver", "T-Over", "T_Over", "T Over");
            MapUnitsColumns("HotelNights", "Hotel Nights", "Hotel-Nights", "Hotel_Nights");
            MapColumns(c => c.TravelType, TravelTypeConversion, "IND", "Indicator", "I");
            MapColumns(c => c.StartDate, DateConversion,
                       "AirDepartureDate",
                       "Air Departure Date",
                       "Air_Departure_Date",
                       "Air-Departure-Date");
            MapColumns(c => c.RouteDetails,
                       "Route Details",
                       "Route-Details",
                       "Route_Details",
                       "RouteDetails",
                       "Routing",
                       "Route");
        }

        private static void TravelTypeConversion(TravelDataContract contract, object value)
        {
            var stringValue = string.Format("{0}", value).Trim();
            if (string.Equals(stringValue,"A",StringComparison.InvariantCultureIgnoreCase))
            {
                contract.TravelType = TravelType.AirTravel;
            }
            if (string.Equals(stringValue, "H", StringComparison.InvariantCultureIgnoreCase))
            {
                contract.TravelType = TravelType.Hotel;
            }
        }

        private static void DateConversion(TravelDataContract contract, object value)
        {
            var datetime = TryParser.DateTime(value);
            contract.StartDate = datetime;
            contract.EndDate = datetime;
        }
    }
}
