using System.Collections.Generic;
using System.IO;
using CarbonKnown.GenericFile.Properties;
using OfficeOpenXml;

namespace CarbonKnown.GenericFile
{
    public class GenericExcelFile
    {
        private readonly Stream fileStream;

        public GenericExcelFile(Stream excelStream)
        {
            fileStream = excelStream;
        }

        public ExcelPackage CreatePackage(IEnumerable<string> costCodes,IEnumerable<string>  consumptionTypes)
        {
            var package = new ExcelPackage(fileStream);

            var workbook = package.Workbook;
            var sheet = workbook.Worksheets[Settings.Default.SourceSheet];
            if (sheet == null) return null;

            sheet.Cells[ExcelCellBase.GetAddress(4,1,sheet.Dimension.End.Row,sheet.Dimension.End.Column)].Clear();
            var factorRowNo = 3;
            foreach (var consumptionType in consumptionTypes)
            {
                factorRowNo++;
                var dashIndex = consumptionType.LastIndexOf('-');
                var uom = consumptionType.Substring(dashIndex).Trim(new[] {' ', '-'});
                sheet.Cells[factorRowNo, 3].Value = consumptionType;
                sheet.Cells[factorRowNo, 4].Value = uom;
            }
            var entityRowNo = 3;
            foreach (var costCode in costCodes)
            {
                entityRowNo++;
                sheet.Cells[entityRowNo, 1].Value = costCode;
            }
            workbook.Names["CarbonKnownEmissionFactors"].Address =
                ExcelCellBase.GetFullAddress(Settings.Default.SourceSheet,
                                             ExcelCellBase.GetAddress(4, 3, factorRowNo, 3));

            workbook.Names["CarbonKnownEmissionFactorsAll"].Address =
                ExcelCellBase.GetFullAddress(Settings.Default.SourceSheet,
                                             ExcelCellBase.GetAddress(4, 3, factorRowNo, 4));

            workbook.Names["CarbonKnownEntityCodes"].Address =
                ExcelCellBase.GetFullAddress(Settings.Default.SourceSheet,
                                             ExcelCellBase.GetAddress(4, 1, entityRowNo, 1));
            workbook.Names["CarbonKnownEntityVLookup"].Address =
                ExcelCellBase.GetFullAddress(Settings.Default.SourceSheet,
                                             ExcelCellBase.GetAddress(4, 1, entityRowNo, 2));

            return package;
        }
    }
}
