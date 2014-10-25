using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.AirTravel
{
    [ServiceContract]
    public partial interface IAirTravelService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(AirTravelDataContract dataEntry);
    }
}
