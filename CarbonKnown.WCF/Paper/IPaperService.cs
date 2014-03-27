using System.ServiceModel;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.WCF.Paper
{
    [ServiceContract]
    public partial interface IPaperService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(PaperDataContract dataEntry);
    }
}
