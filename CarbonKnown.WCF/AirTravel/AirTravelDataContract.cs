using System;
using System.Runtime.Serialization;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.AirTravel
{
    public partial class AirTravelDataContract : DataEntryDataContract
    {
        public TravelClass TravelClass { get; set; }
    }
}
