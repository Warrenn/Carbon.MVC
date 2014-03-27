using System;
using System.Collections.Generic;
using System.IO;

namespace CarbonKnown.FileReaders.Readers
{
    public class CsvFileReader : IFileReader
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDictionary<string, object>> ExtractData(Stream fileStream)
        {
            throw new NotImplementedException();
        }
    }
}
