using System.ServiceModel;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Fuel
{
    [ServiceContract]
    public partial interface IFuelService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(FuelDataContract dataEntry);
    }
}
