using System;
using System.ServiceModel;
using System.ServiceModel.Web;
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
