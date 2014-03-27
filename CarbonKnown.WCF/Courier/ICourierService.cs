using System.ServiceModel;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Courier
{
    [ServiceContract]
    public partial interface ICourierService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(CourierDataContract dataEntry);
    }
}
