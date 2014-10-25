using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Refrigerant
{
    [ServiceContract]
    public partial interface IRefrigerantService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(RefrigerantDataContract dataEntry);
    }
}
