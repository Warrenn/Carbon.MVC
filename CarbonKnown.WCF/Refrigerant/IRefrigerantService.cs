using System.ServiceModel;
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
