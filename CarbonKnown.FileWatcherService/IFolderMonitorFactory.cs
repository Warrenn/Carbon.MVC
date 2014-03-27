using CarbonKnown.FileReaders.FileHandler;

namespace CarbonKnown.FileWatcherService
{
    public interface IFolderMonitorFactory
    {
        FolderMonitor CreateFolderMonitor(string path, IFileHandler handler);
    }
}