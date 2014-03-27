using System;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.WCF.Fleet;

namespace CarbonKnown.FileReaders.Fleet
{
    public sealed class FleetHandler : FileHandlerBase<FleetDataContract>
    {
        public const string IndicatorFlag = "I";

        public FleetHandler(string host)
            :base(host)
        {
            MapCostCodeColumns();
            MapMoneyColumns(
                "Total-Fuel-Amount", 
                "Total_Fuel_Amount", 
                "Total Fuel Amount");
            MapUnitsColumns(
                "Total litres",
                "Total-litres",
                "Total_litres");
            MapColumns(c => c.Scope, ConvertIndicator, "I", "Ind");
            MapColumns(c => c.FuelType,ConvertFuelType,
                "Fuel Type",
                "Fuel-Type",
                "Fuel_Type");
            MapColumns(c => c.StartDate, ConvertPeriod, "Period");
        }

        private static void ConvertFuelType(FleetDataContract contract, object value)
        {
            var stringValue = string.Format("{0}", value).Trim();
            if (string.Equals(stringValue, "Petrol", StringComparison.InvariantCultureIgnoreCase))
            {
                contract.FuelType = FuelType.Petrol;
            }
            if (string.Equals(stringValue, "Diesel", StringComparison.InvariantCultureIgnoreCase))
            {
                contract.FuelType = FuelType.Diesel;
            }
        }

        private static void ConvertIndicator(FleetDataContract contract, object value)
        {
            var stringValue = string.Format("{0}", value).Trim();
            contract.Scope = string.Equals(stringValue, "I", StringComparison.InvariantCultureIgnoreCase) ? 
                FleetScope.CompanyOwned : 
                FleetScope.ThirdParty;
        }

        private static void ConvertPeriod(FleetDataContract contract, object value)
        {
            var stringValue = string.Format("{0}", value).Trim();
            int year;
            int month;
            if (string.IsNullOrEmpty(stringValue) ||
                (stringValue.Length != 6) ||
                (!int.TryParse(stringValue.Substring(0, 4), out year)) ||
                (!int.TryParse(stringValue.Substring(4, 2), out month))) return;
            contract.StartDate = new DateTime(year, month, 1);
            contract.EndDate = contract.StartDate.Value.AddMonths(1).AddDays(-1);
        }
        
        public override void UpsertDataEntry(FleetDataContract contract)
        {
            CallService<IFleetService>(service => service.UpsertDataEntry(contract));
        }
    }
}
