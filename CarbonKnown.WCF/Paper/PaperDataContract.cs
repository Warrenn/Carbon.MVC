using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Paper
{
    public partial class PaperDataContract : DataEntryDataContract
    {
        public PaperType? PaperType { get; set; }
        public PaperUom? PaperUom { get; set; }
    }
}
