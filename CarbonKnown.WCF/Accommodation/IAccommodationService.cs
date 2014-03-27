using System.ServiceModel;
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
