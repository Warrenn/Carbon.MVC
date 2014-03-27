using System;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.WCF.CourierRoute;

namespace CarbonKnown.FileReaders.Courier
{
    public sealed class CourierHandler : FileHandlerBase<CourierRouteDataContract>
    {
        public CourierHandler(string host)
            : base(host)
        {
            MapCostCodeColumns();
            MapStartDateColumns("ShipmentDate", "Shipment Date", "Shipment_Date", "Shipment-Date");
            MapEndDateColumns("ShipmentDate", "Shipment Date", "Shipment_Date", "Shipment-Date");
            MapColumns(c => c.FromCode, "FromCode", "From Code", "From_Code", "From-Code");
            MapColumns(c => c.ToCode, "ToCode", "To Code", "To_Code", "To-Code");
            MapColumns(c => c.ServiceType,ConvertServiceType, "Service");
            MapColumns(c => c.ChargeMass, "ChargeMass", "Charge Mass", "Charge_Mass", "Charge-Mass");
            MapColumns(c => c.Money, ConvertMoney, "TotalCost", "Total Cost", "Total_Cost", "Total-Cost");
        }

        private static void ConvertMoney(CourierRouteDataContract contract, object value)
        {
            contract.Money = TryParser.Nullable<decimal>(value);
            contract.Reversal = contract.Money < 0;
        }

        private static void ConvertServiceType(CourierRouteDataContract contract, object value)
        {
            var serviceData = string.Format("{0}", value).Trim();
            contract.ServiceType = string.Equals("Economy", serviceData, StringComparison.InvariantCultureIgnoreCase)
                                  ? ServiceType.Economy
                                  : ServiceType.Other;
        }

        public override void UpsertDataEntry(CourierRouteDataContract contract)
        {
            CallService<ICourierRouteService>(service => service.UpsertDataEntry(contract));
        }
    }
}
