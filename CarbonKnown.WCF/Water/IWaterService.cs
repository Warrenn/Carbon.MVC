using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Water
{
    [ServiceContract]
    public partial interface IWaterService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(WaterDataContract dataEntry);
    }
}
