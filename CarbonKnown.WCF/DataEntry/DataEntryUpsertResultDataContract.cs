using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CarbonKnown.WCF.DataEntry
{
    public class DataEntryUpsertResultDataContract
    {
        public DataEntryUpsertResultDataContract()
        {
            Errors = new Collection<DataEntryErrorDataContract>();
        }

        public Guid EntryId { get; set; }
        public bool Succeeded { get; set; }
        public ICollection<DataEntryErrorDataContract> Errors { get; set; }
    }
}
