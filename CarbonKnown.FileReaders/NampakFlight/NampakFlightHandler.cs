using System;
using System.Text.RegularExpressions;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.FileReaders.Readers;
using CarbonKnown.FileReaders.TWF;
using CarbonKnown.WCF.AirTravel;

namespace CarbonKnown.FileReaders.NampakFlight
{
    public sealed class NampakFlightHandler : TravelHandlerBase
    {
        public NampakFlightHandler(string host)
            : base(host)
        {
            MapStartDateColumns("Doc Date");
            MapEndDateColumns("Doc Date");
            MapColumns(c => c.RouteDetails, "Route Details");
            MapColumns(c => c.ClassCategory, ClassCategoryConversion, "Cabin Class");
            MapMoneyColumns("Sum of Total Amount (Excl VAT)");
        }

        public override IFileReader GetReaderFromType(string fullPath, Guid sourceId)
        {
            var reader = base.GetReaderFromType(fullPath, sourceId);
            var xlsxReader = reader as XlsxFileReader;
            if (xlsxReader != null)
            {
                xlsxReader.RowStart = 9;
                return xlsxReader;
            }
            var xlsReader = reader as XlsFileReader;
            if (xlsReader == null) return reader;
            xlsReader.RowStart = 9;
            return reader;
        }

        private static void ClassCategoryConversion(TravelDataContract contract, object value)
        {
            contract.TravelType = TravelType.AirTravel;
            var stringValue = string.Format("{0}", value);
            if (string.Equals("Economy", stringValue, StringComparison.InvariantCultureIgnoreCase))
            {
                contract.ClassCategory = TravelClass.Economy;
                return;
            }
            if (string.Equals("Business", stringValue, StringComparison.InvariantCultureIgnoreCase))
            {
                contract.ClassCategory = TravelClass.Business;
                return;
            }
            if (string.Equals("First", stringValue, StringComparison.InvariantCultureIgnoreCase))
            {
                contract.ClassCategory = TravelClass.FirstClass;
                return;
            }
            contract.ClassCategory = TravelClass.Average;
        }
    }
}
