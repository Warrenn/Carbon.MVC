using System;
using System.Runtime.Serialization;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Electricity
{
    public partial class ElectricityDataContract : DataEntryDataContract
    {
        public ElectricityType? ElectricityType { get; set; }
    }
}
