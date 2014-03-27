using System;
using System.Collections.Generic;
using System.IO;

namespace CarbonKnown.FileReaders.Readers
{
    public interface IFileReader : IDisposable
    {
        IEnumerable<IDictionary<string, object>> ExtractData(Stream fileStream);
    }
}