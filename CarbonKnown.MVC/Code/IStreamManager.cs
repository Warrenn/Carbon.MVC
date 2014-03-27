using System;
using System.IO;
using System.Threading.Tasks;

namespace CarbonKnown.MVC.Code
{
    public interface IStreamManager
    {
        string StageStream(Guid sourceId, string originalFileName, Stream stream);
        string PrepareForExtraction(Guid sourceId, string handlerName, string currentFileName);
        void RemoveStream(Guid sourceId, string currentFileName);
        Task<MemoryStream> RetrieveData(Guid sourceId, string currentFileName);
    }
}
