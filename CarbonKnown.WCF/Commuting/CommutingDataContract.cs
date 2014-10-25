using System;
using System.Runtime.Serialization;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Commuting
{
    public partial class CommutingDataContract : DataEntryDataContract
    {
        public CommutingType? CommutingType { get; set; }
    }
}
