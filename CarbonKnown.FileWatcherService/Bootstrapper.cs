using System;
using System.Configuration;
using System.ServiceProcess;
using CarbonKnown.FileReaders;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace CarbonKnown.FileWatcherService
{
    public static class Bootstrapper
    {
        private static readonly Lazy<IUnityContainer> LazyContainer =
            new Lazy<IUnityContainer>(BuildUnityContainer);

        public static IUnityContainer Container
        {
            get { return LazyContainer.Value; }
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var section = (UnityConfigurationSection)
                          ConfigurationManager.GetSection(UnityConfigurationSection.SectionName);
            var returnContainer = new UnityContainer();
            var container = section.Configure(returnContainer);
            var config = ConfigurationSourceFactory.Create();
            container.RegisterInstance(CreateLogWriter(config));
            container.RegisterInstance(CreateExceptionManager(config));
            container.RegisterInstance(container);
            container.AddNewExtension<Interception>();
            RegisterTypes(container);
            return container;
        }

        private static LogWriter CreateLogWriter(IConfigurationSource config)
        {
            var factory = new LogWriterFactory(config);
            var writer = factory.Create();
            Logger.SetLogWriter(writer);
            return writer;
        }

        private static ExceptionManager CreateExceptionManager(IConfigurationSource config)
        {
            var factory = new ExceptionPolicyFactory(config);
            var manger = factory.CreateManager();
            ExceptionPolicy.SetExceptionManager(manger);
            return manger;
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            HandlerFactory.RegisterHandlers(container);
            FolderMonitorFactory.RegisterFolderMonitor(container);
            container.RegisterType<ServiceBase, FileWatcherService>(new HierarchicalLifetimeManager());
        }
    }
}
