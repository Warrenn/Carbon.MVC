using System;
using System.ServiceModel;
using System.ServiceModel.Web;
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
