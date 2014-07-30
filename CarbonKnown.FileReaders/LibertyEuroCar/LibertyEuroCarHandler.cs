using System;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.FileReaders.Readers;
using CarbonKnown.WCF.CarHire;

namespace CarbonKnown.FileReaders.LibertyEuroCar
{
    public sealed class LibertyEuroCarHandler : FileHandlerBase<CarHireDataContract>
    {
        public LibertyEuroCarHandler(string host)
            : base(host)
        {
            MapColumns(c => c.StartDate, (c, o) => c.StartDate = ConvertYearMonthDay(o), "START DATE");
            MapColumns(c => c.EndDate, (c, o) => c.EndDate = ConvertYearMonthDay(o), "END DATE");
            MapColumns(c => c.CarGroupBill, "ACTUAL VGRP");
            MapUnitsColumns("KMS");
            MapMoneyColumns("TOTAL COST(AS PER CURRENCY)");
        }

        public override IFileReader GetReaderFromType(string fullPath, Guid sourceId)
        {
            var fileReader = base.GetReaderFromType(fullPath, sourceId);
            var xlsxFileReader = fileReader as XlsxFileReader;
            if (xlsxFileReader != null)
            {
                xlsxFileReader.RowStart = 3;
                return xlsxFileReader;
            }
            var reader = fileReader as XlsFileReader;
            if (reader == null) return fileReader;
            reader.RowStart = 3;
            return reader;
        }


        private static DateTime? ConvertYearMonthDay(object value)
        {
            var stringValue = string.Format("{0}", value).Trim();
            int year;
            int month;
            int day;
            if (string.IsNullOrEmpty(stringValue) ||
                (stringValue.Length != 8) ||
                (!int.TryParse(stringValue.Substring(0, 4), out year)) ||
                (!int.TryParse(stringValue.Substring(4, 2), out month)) ||
                (!int.TryParse(stringValue.Substring(6, 2), out day))) return null;
            return new DateTime(year, month, day);
        }

        public override void UpsertDataEntry(CarHireDataContract contract)
        {
            contract.CostCode = "lb001";
            CallService<ICarHireService>(service => service.UpsertDataEntry(contract));
        }
    }
}
