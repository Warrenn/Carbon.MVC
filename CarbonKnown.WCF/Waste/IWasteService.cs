using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Waste
{
    [ServiceContract]
    public partial interface IWasteService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(WasteDataContract dataEntry);
    }
}
