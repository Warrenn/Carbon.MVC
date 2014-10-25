using System;
using System.Runtime.Serialization;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Refrigerant
{
    public partial class RefrigerantDataContract : DataEntryDataContract
    {
        public RefrigerantType? RefrigerantType { get; set; }
    }
}
