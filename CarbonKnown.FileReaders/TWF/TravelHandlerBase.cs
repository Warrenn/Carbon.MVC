using System;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.WCF.Accommodation;
using CarbonKnown.WCF.AirTravelRoute;

namespace CarbonKnown.FileReaders.TWF
{
    public class TravelHandlerBase : FileHandlerBase<TravelDataContract>
    {
        protected TravelHandlerBase(string host) : base(host)
        {
        }

        public void UpsertAirTravel(TravelDataContract contract)
        {
            var routing = contract.RouteDetails;
            foreach (var legGroup in routing.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries))
            {
                var legParts = legGroup.Split("/\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var money = contract.Money;
                var reversal = contract.Money < 1;
                for (var legIndex = 0; legIndex < (legParts.Length - 1); legIndex++)
                {
                    var fromCode = string.Format("{0}", legParts[legIndex]).ToUpper().Trim();
                    var toCode = string.Format("{0}", legParts[legIndex + 1]).ToUpper().Trim();
                    var airTravelData = new AirTravelRouteDataContract
                        {
                            CostCode = contract.CostCode,
                            EndDate = contract.EndDate,
                            FromCode = fromCode,
                            Money = money,
                            RowNo = contract.RowNo,
                            StartDate = contract.StartDate,
                            ToCode = toCode,
                            TravelClass = (TravelClass)contract.ClassCategory,
                            SourceId = contract.SourceId,
                            Reversal = reversal
                        };
                    CallService<IAirTravelRouteService>(service => service.UpsertDataEntry(airTravelData));
                    money = 0;
                }
            }
        }

        public void UpsertHotelTravel(TravelDataContract contract)
        {
            var money = contract.Money;
            var units = contract.Units;
            var accommodationData = new AccommodationDataContract
                {
                    CostCode = contract.CostCode,
                    EndDate = contract.EndDate,
                    Money = money,
                    RowNo = contract.RowNo,
                    StartDate = contract.StartDate,
                    Units = units,
                    SourceId = contract.SourceId
                };
            CallService<IAccommodationService>(service => service.UpsertDataEntry(accommodationData));
        }

        public override void UpsertDataEntry(TravelDataContract contract)
        {
            if (contract.TravelType == TravelType.AirTravel)
            {
                UpsertAirTravel(contract);
            }
            if (contract.TravelType == TravelType.Hotel)
            {
                UpsertHotelTravel(contract);
            }
        }
    }
}