using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarbonKnown.WCF.DataEntry;

namespace CarbonKnown.FileReaders.LibertyAirTickets
{
    public class LibertyAirTicketsDataContract : DataEntryDataContract
    {
        public string TicketType { get; set; }
    }
}
