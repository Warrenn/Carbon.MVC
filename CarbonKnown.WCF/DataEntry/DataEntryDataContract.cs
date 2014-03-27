using System;

namespace CarbonKnown.WCF.DataEntry
{
    public class DataEntryDataContract
    {
        public Guid SourceId { get; set; }
        public Guid? EntryId { get; set; }
        public int RowNo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CostCode { get; set; }
        public string UserName { get; set; }
        public decimal? Money { get; set; }
        public decimal? Units { get; set; }
    }
}
