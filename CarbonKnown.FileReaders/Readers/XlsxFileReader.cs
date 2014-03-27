using System.Collections.Generic;
using System.IO;
using System.Linq;
using CarbonKnown.FileReaders.Constants;
using OfficeOpenXml;

namespace CarbonKnown.FileReaders.Readers
{
    public class XlsxFileReader : IFileReader
    {
        protected ExcelPackage Package;
        protected string[] FieldNames;
        protected ExcelWorkbook Workbook;
        protected ExcelWorksheet Worksheet;
        protected ExcelAddressBase Dimension;
        protected ExcelCellAddress EndAddress;

        public XlsxFileReader()
        {
            SheetName = string.Empty;
            RowStart = 2;
        }

        public string SheetName { get; set; }
        public int RowStart { get; set; }

        protected virtual IEnumerable<IDictionary<string, object>> ReadData()
        {
            for (var rowIndex = RowStart; rowIndex <= EndAddress.Row; rowIndex++)
            {
                var rowData = new Dictionary<string, object>();

                for (var colIndex = 1; colIndex <= EndAddress.Column; colIndex++)
                {
                    var currentCell = Worksheet.Cells[rowIndex, colIndex];
                    var currentValue = currentCell.Value;
                    var currentKey = FieldNames[(colIndex - 1)];
                    rowData[currentKey] = currentValue;
                }
                if (rowData.All(pair => string.IsNullOrEmpty(string.Format("{0}", pair.Value)))) continue;
                yield return rowData;
            }
        }

        protected virtual void ReadHeaders()
        {
            FieldNames = new string[EndAddress.Column];
            for (var colIndex = 1; colIndex <= EndAddress.Column ; colIndex++)
            {
                var headerIndex = RowStart - 1;
                var currentCell = Worksheet.Cells[headerIndex, colIndex];
                var currentValue = string.Format("{0}", currentCell.Value).ToUpper().Trim();
                if (string.IsNullOrEmpty(currentValue)) currentValue = string.Format("Column{0}", colIndex);
                FieldNames[colIndex - 1] = currentValue;
            }
        }

        public virtual void Dispose()
        {
            Package.Dispose(PolicyName.Disposable);
        }

        public IEnumerable<IDictionary<string, object>> ExtractData(Stream fileStream)
        {
            Package = new ExcelPackage(fileStream);
            Workbook = Package.Workbook;
            Worksheet = (string.IsNullOrEmpty(SheetName)) ? Workbook.Worksheets[1] : Workbook.Worksheets[SheetName];
            Dimension = Worksheet.Dimension;
            if (Dimension == null) return Enumerable.Empty<IDictionary<string, object>>();
            EndAddress = Dimension.End;

            ReadHeaders();
            return ReadData();
        }
    }
}
