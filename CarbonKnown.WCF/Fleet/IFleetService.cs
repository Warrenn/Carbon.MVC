using System.ServiceModel;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Fleet
{
    [ServiceContract]
    public partial interface IFleetService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(FleetDataContract dataEntry);
    }
}
