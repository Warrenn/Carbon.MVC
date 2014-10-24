using System;
using System.ServiceModel;
using System.ServiceModel.Web;
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
