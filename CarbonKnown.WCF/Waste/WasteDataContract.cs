﻿using System;
using System.Runtime.Serialization;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Waste
{
    public partial class WasteDataContract : DataEntryDataContract
    {
        public WasteType? WasteType { get; set; }
    }
}
