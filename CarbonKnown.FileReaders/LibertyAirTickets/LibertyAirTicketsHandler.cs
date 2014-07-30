using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.FileReaders.TWF;
using CarbonKnown.WCF.AirTravelRoute;

namespace CarbonKnown.FileReaders.LibertyAirTickets
{
    public sealed class LibertyAirTicketsHandler : FileHandlerBase<LibertyAirTicketsDataContract>
    {
        public LibertyAirTicketsHandler(string host) : base(host)
        {
            MapStartDateColumns("Issue_Date");
            MapEndDateColumns("Issue_Date");
            MapCostCodeColumns("CostCentre");
            MapMoneyColumns("Spend");
            MapColumns(c => c.TicketType, "Ticket Type");
        }

        public override void UpsertDataEntry(LibertyAirTicketsDataContract contract)
        {
            var parts = contract.TicketType.Split(' ');
            var routeCodes = new string[0];
            foreach (var part in parts)
            {
                if ((part.IndexOf('-') <= 0) || (routeCodes = part.Split('-')).Length != 2) continue;
                break;
            }
            if (routeCodes.Length != 2) return;
            var travelData = new AirTravelRouteDataContract
            {
                CostCode = "lb001",
                EndDate = contract.EndDate,
                FromCode = routeCodes[0],
                Money = contract.Money,
                RowNo = contract.RowNo,
                StartDate = contract.StartDate,
                ToCode = routeCodes[1],
                Reversal = false,
                TravelClass = TravelClass.Economy,
                SourceId = contract.SourceId
            };
            CallService<IAirTravelRouteService>(s => s.UpsertDataEntry(travelData));
        }
    }
}
