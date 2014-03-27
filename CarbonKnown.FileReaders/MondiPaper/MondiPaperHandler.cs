using System;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.WCF.Paper;

namespace CarbonKnown.FileReaders.MondiPaper
{
    public sealed class MondiPaperHandler : FileHandlerBase<PaperDataContract>
    {
        public MondiPaperHandler(string host)
            : base(host)
        {
            MapCostCodeColumns();
            MapMoneyColumns(
                "Total-Cost",
                "Total Cost",
                "Total_Cost",
                "TotalCost");
            MapUnitsColumns(
                "Order Qty",
                "Order_Qty",
                "OrderQty",
                "Order-Qty");
            MapColumns(c => c.PaperUom, ConvertUOM, "UOM");
            MapColumns(c => c.PaperType, ConvertPaperType,
                       "transactiontype",
                       "transaction-type",
                       "transaction_type",
                       "transaction type");
            MapStartDateColumns(
                "datestart",
                "date_start",
                "date-start",
                "date start",
                "startdate",
                "start-date",
                "start_date",
                "start date");
            MapEndDateColumns(
                "dateend",
                "date-end",
                "date_end",
                "date end",
                "enddate",
                "end-date",
                "end_date",
                "end date");
        }

        public static void ConvertPaperType(PaperDataContract contract, object value)
        {
            var stringValue = string.Format("{0}", value).Trim();
            if (string.Equals(stringValue, "A3", StringComparison.InvariantCultureIgnoreCase))
            {
                contract.PaperType = PaperType.MondiA3;
            }
            if (string.Equals(stringValue, "A4", StringComparison.InvariantCultureIgnoreCase))
            {
                contract.PaperType = PaperType.MondiA4;
            }
        }

        private static void ConvertUOM(PaperDataContract contract, object value)
        {
            var stringValue = string.Format("{0}", value).Trim();
            contract.PaperUom = (string.Equals(stringValue, "RM", StringComparison.InvariantCultureIgnoreCase))
                                    ? PaperUom.Reams
                                    : PaperUom.Tonnes;
        }

        public override void UpsertDataEntry(PaperDataContract contract)
        {
            CallService<IPaperService>(service => service.UpsertDataEntry(contract));
        }
    }
}
