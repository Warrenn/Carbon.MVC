using System;
using System.Collections.ObjectModel;

namespace CarbonKnown.WCF.DataSource
{
    public class SourceResultDataContract
    {
        public SourceResultDataContract()
        {
            ErrorMessages = new Collection<string>();
        }

        public Guid SourceId { get; set; }
        public bool Succeeded { get; set; }
        public Collection<string> ErrorMessages { get; set; }
    }
} 
