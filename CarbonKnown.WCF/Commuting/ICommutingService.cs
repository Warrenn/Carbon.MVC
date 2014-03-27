using System.ServiceModel;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Commuting
{
    [ServiceContract]
    public partial interface ICommutingService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(CommutingDataContract dataEntry);
    }
}
