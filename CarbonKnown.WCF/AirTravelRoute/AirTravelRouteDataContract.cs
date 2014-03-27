using System;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.AirTravelRoute
{
    public partial class AirTravelRouteDataContract : DataEntryDataContract
    {
        public TravelClass TravelClass { get; set; }
        public Boolean Reversal { get; set; }
        public String FromCode { get; set; }
        public String ToCode { get; set; }
    }
}
