using System;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.WCF.CarHire;

namespace CarbonKnown.FileReaders.AvisCourier
{
    public sealed class AvisCourierHandler : FileHandlerBase<CarHireDataContract>
    {
        public AvisCourierHandler(string host)
            : base(host)
        {
            MapCostCodeColumns();
            MapUnitsColumns("TOTAL-KMS", "TOTAL KMS");
            MapMoneyColumns("TOTAL-CHARGE", "TOTAL CHARGE", "TOTAL_CHARGE");
            MapColumns(c => c.CarGroupBill, "CAR-GROUP-BILL", "CAR GROUP BILL", "CAR_GROUP_BILL");
            MapColumns(c => c.StartDate, (c, o) => c.StartDate = TryParser.DateTime(o) ?? ConvertToDateTime(o),
                       "CHECK-OUT-DATE", "CHECK OUT DATE", "CHECK_OUT_DATE");
            MapColumns(c => c.EndDate, (c, o) => c.EndDate = TryParser.DateTime(o) ?? ConvertToDateTime(o),
                       "CHECK-IN-DATE", "CHECK IN DATE", "CHECK_IN_DATE");
        }

        private static DateTime? ConvertToDateTime(object value)
        {
            var stringValue = string.Format("{0}", value).Trim();
            int year;
            int month;
            int day;
            if (!string.IsNullOrEmpty(stringValue) &&
                (stringValue.Length == 8) &&
                (int.TryParse(stringValue.Substring(0, 4), out year)) &&
                (int.TryParse(stringValue.Substring(4, 2), out month)) &&
                (int.TryParse(stringValue.Substring(6, 2), out day)))
            {
                return new DateTime(year, month, day);
            }
            return null;
        }

        public override void UpsertDataEntry(CarHireDataContract contract)
        {
            CallService<ICarHireService>(service => service.UpsertDataEntry(contract));
        }
    }
}
