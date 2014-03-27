using CarbonKnown.FileReaders.FileHandler;

namespace CarbonKnown.FileReaders
{
    public interface IHandlerFactory
    {
        IFileHandler CreateHandler(string handlerName);
        IFileHandler CreateHandler(string handlerName, string host);
    }
}