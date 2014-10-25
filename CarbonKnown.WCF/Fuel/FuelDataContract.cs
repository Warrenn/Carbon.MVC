using System;
using System.Runtime.Serialization;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Fuel
{
    public partial class FuelDataContract : DataEntryDataContract
    {
        public FuelType? FuelType { get; set; }
        public UnitOfMeasure? UOM { get; set; }
    }
}
