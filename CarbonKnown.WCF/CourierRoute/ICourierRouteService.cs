using System.ServiceModel;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.CourierRoute
{
    [ServiceContract]
    public partial interface ICourierRouteService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(CourierRouteDataContract dataEntry);
    }
}
