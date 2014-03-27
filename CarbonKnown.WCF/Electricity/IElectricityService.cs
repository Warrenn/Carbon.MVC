using System.ServiceModel;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Electricity
{
    [ServiceContract]
    public partial interface IElectricityService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(ElectricityDataContract dataEntry);
    }
}
