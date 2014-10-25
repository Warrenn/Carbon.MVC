using System;
using System.ServiceModel;
using System.ServiceModel.Web;
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
