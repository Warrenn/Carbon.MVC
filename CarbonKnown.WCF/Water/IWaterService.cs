using System.ServiceModel;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Water
{
    [ServiceContract]
    public partial interface IWaterService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(WaterDataContract dataEntry);
    }
}
