using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.CarHire
{
    [ServiceContract]
    public partial interface ICarHireService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(CarHireDataContract dataEntry);
    }
}
