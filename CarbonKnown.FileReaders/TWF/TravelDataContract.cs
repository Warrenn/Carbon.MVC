using CarbonKnown.WCF.AirTravel;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.FileReaders.TWF
{
    public class TravelDataContract : DataEntryDataContract
    {
        public TravelType TravelType { get; set; }
        public string RouteDetails { get; set; }
        public TravelClass ClassCategory { get; set; }
    }
}
