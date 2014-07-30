using System;
using System.Collections.Generic;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.FileReaders.Readers;
using CarbonKnown.WCF.CarHire;
using CarbonKnown.WCF.Fleet;

namespace CarbonKnown.FileReaders.LibertyAvis
{
    public sealed class LibertyAvisHandler : FileHandlerBase<CarHireDataContract>
    {
        public LibertyAvisHandler(string host)
            : base(host)
        {
            MapColumns(c => c.StartDate, ConvertYearMonth, "Input Calendar Year & Month Number");
            MapColumns(c => c.CarGroupBill, "Car Group Driven Code");
            MapUnitsColumns("Driven Distance");
        }

        public override IFileReader GetReaderFromType(string fullPath, Guid sourceId)
        {
            var fileReader = base.GetReaderFromType(fullPath, sourceId);
            var xlsxFileReader = fileReader as XlsxFileReader;
            if (xlsxFileReader != null)
            {
                xlsxFileReader.RowStart = 6;
                xlsxFileReader.SheetName = "Data";
                return xlsxFileReader;
            }
            var reader = fileReader as XlsFileReader;
            if (reader == null) return fileReader;
            reader.RowStart = 6;
            reader.SheetName = "Data";
            return reader;
        }


        private static void ConvertYearMonth(CarHireDataContract contract, object value)
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

        public override void UpsertDataEntry(CarHireDataContract contract)
        {
            contract.CostCode = "lb001";
            contract.Money = 0;
            CallService<ICarHireService>(service => service.UpsertDataEntry(contract));
        }
    }
}
