using System;
using System.Globalization;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.FileReaders.Readers;
using CarbonKnown.WCF.CarHire;

namespace CarbonKnown.FileReaders.EzShuttle
{
    public sealed class EzShuttleHandler : FileHandlerBase<CarHireDataContract>
    {
        public EzShuttleHandler(string host)
            : base(host)
        {
            MapColumns(c => c.StartDate, (c, o) => c.StartDate = Convert(o), "PickupDateTime");
            MapColumns(c => c.EndDate, (c, o) => c.EndDate = Convert(o), "PickupDateTime");
            MapCostCodeColumns("DriverName");
            MapUnitsColumns("Avg Km per transfer");
            MapMoneyColumns("Price");
        }

        public static DateTime? Convert(object o)
        {
            var stringValue = string.Format("{0}", o);
            DateTime value;
            if (DateTime.TryParse(stringValue, new CultureInfo("en-US"), DateTimeStyles.None, out value))
            {
                return value;
            }
            return null;
        }

        public override void UpsertDataEntry(CarHireDataContract contract)
        {
            if (string.Equals(contract.CostCode, "CANCELLATIONS *", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
            contract.CostCode = "lb001";
            contract.CarGroupBill = CarGroupBill.AveragePetrol;
            CallService<ICarHireService>(service => service.UpsertDataEntry(contract));
        }
    }
}
