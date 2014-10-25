using System;
using System.ServiceModel;
using System.ServiceModel.Web;
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
