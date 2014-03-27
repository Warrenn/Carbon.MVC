using CarbonKnown.FileReaders.FileHandler;
using Microsoft.Practices.Unity;

namespace CarbonKnown.FileWatcherService
{
    public class FolderMonitorFactory : IFolderMonitorFactory
    {
        private readonly IUnityContainer container;

        public static void RegisterFolderMonitor(IUnityContainer container)
        {
            container.RegisterType<IFolderMonitorFactory, FolderMonitorFactory>();
            container.RegisterType<FolderMonitor>();
        }

        public FolderMonitorFactory(IUnityContainer container)
        {
            this.container = container;
        }

        public FolderMonitor CreateFolderMonitor(string path, IFileHandler handler)
        {
            return
                container.Resolve<FolderMonitor>(
                    new ParameterOverride("path", path),
                    new ParameterOverride("fileHandler", handler));
        }
    }
}
