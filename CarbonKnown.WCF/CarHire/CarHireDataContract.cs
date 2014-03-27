using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.CarHire
{
    public partial class CarHireDataContract : DataEntryDataContract
    {
        public CarGroupBill? CarGroupBill { get; set; }
    }
}
