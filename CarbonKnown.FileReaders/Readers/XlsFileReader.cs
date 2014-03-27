using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CarbonKnown.FileReaders.Constants;
using Excel;

namespace CarbonKnown.FileReaders.Readers
{
    public class XlsFileReader : IFileReader
    {
        protected string TempFileName = string.Empty;
        protected IExcelDataReader Reader;
        protected Stream TempFileStream;
        protected string[] FieldNames;

        public XlsFileReader()
        {
            SheetName = string.Empty;
            RowStart = 2;
        }

        public string SheetName { get; set; }
        public int RowStart { get; set; }

        public IEnumerable<IDictionary<string, object>> ExtractData(Stream fileStream)
        {
            try
            {
                Reader = ExcelReaderFactory.CreateBinaryReader(fileStream);
            }
            catch (ArgumentOutOfRangeException)
            {
                //Known bug with excel reader sometimes will throw argument out of range exception http://exceldatareader.codeplex.com/discussions/431882
                var contents = ((MemoryStream) (fileStream)).ToArray();
                TempFileName = Path.GetTempFileName();
                File.WriteAllBytes(TempFileName, contents);
                TempFileStream = new FileStream(TempFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                Reader = ExcelReaderFactory.CreateBinaryReader(TempFileStream);
            }
            ReadHeader();
            return ReadFileContents();
        }

        protected virtual IEnumerable<IDictionary<string, object>> ReadFileContents()
        {
            while (Reader.Read())
            {
                var rowData = new Dictionary<string, object>();
                for (var fieldIndex = 0; fieldIndex < Reader.FieldCount; fieldIndex++)
                {
                    var name = FieldNames[fieldIndex];
                    var value = Reader.GetValue(fieldIndex);
                    rowData[name] = value;
                }
                if (rowData.All(pair => string.IsNullOrEmpty(string.Format("{0}", pair.Value)))) continue;
                yield return rowData;
            }
        }

        protected virtual void ReadHeader()
        {
            for (var rowNo = 1; rowNo < RowStart; rowNo++)
            {
                if (!Reader.Read()) return;
            }
            FieldNames = new string[Reader.FieldCount];
            for (var fieldIndex = 0; fieldIndex < Reader.FieldCount; fieldIndex++)
            {
                var fieldName = string.Format("{0}", Reader.GetValue(fieldIndex)).ToUpper().Trim(); ;
                if (string.IsNullOrEmpty(fieldName)) fieldName = string.Format("Column{0}", fieldIndex + 1);
                FieldNames[fieldIndex] = fieldName;
            }
        }

        public void Dispose()
        {
            Reader.Dispose(PolicyName.Disposable);
            TempFileStream.Dispose(PolicyName.Disposable);
            if ((!string.IsNullOrEmpty(TempFileName)) && (File.Exists(TempFileName)))
            {
                File.Delete(TempFileName);
            }
        }
    }
}
