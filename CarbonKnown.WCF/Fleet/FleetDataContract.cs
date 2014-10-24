using System;
using System.Runtime.Serialization;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Fleet
{
    public partial class FleetDataContract : DataEntryDataContract
    {
        public FleetScope? Scope { get; set; }
        public FuelType? FuelType { get; set; }
    }
}
