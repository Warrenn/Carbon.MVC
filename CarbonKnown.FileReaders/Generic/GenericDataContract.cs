using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.FileReaders.Generic
{
    public class GenericDataContract : DataEntryDataContract
    {
        public string ConsumptionType { get; set; }
        public string Uom { get; set; }
    }
}
