using System;
using System.ServiceModel;
using System.ServiceModel.Web;
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
