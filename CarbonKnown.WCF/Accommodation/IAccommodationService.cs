using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Accommodation
{
    [ServiceContract]
    public partial interface IAccommodationService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(AccommodationDataContract dataEntry);
    }
}
