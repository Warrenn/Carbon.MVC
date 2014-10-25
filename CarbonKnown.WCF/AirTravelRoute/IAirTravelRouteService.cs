using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.AirTravelRoute
{
    [ServiceContract]
    public partial interface IAirTravelRouteService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(AirTravelRouteDataContract dataEntry);
    }
}
