using System;
using System.Runtime.Serialization;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Courier
{
    public partial class CourierDataContract : DataEntryDataContract
    {
        public ServiceType? ServiceType { get; set; }
        public Decimal? ChargeMass { get; set; }
    }
}
