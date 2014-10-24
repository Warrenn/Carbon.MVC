using System;
using System.Runtime.Serialization;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.CourierRoute
{
    public partial class CourierRouteDataContract : DataEntryDataContract
    {
        public ServiceType? ServiceType { get; set; }
        public Decimal? ChargeMass { get; set; }
        public String FromCode { get; set; }
        public String ToCode { get; set; }
        public Boolean Reversal { get; set; }
    }
}
