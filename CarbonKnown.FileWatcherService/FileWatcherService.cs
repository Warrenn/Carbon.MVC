using System;
using System.Collections.Generic;
using System.ServiceProcess;
using CarbonKnown.FileReaders;
using CarbonKnown.FileReaders.Constants;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace CarbonKnown.FileWatcherService
{
    public partial class FileWatcherService : ServiceBase
    {
        private readonly IFolderMonitorFactory folderMonitorFactory;
        private readonly IHandlerFactory handlerFactory;
        private readonly Queue<FolderMonitor> services;

        public FileWatcherService(IFolderMonitorFactory folderMonitorFactory, IHandlerFactory handlerFactory)
        {
            this.folderMonitorFactory = folderMonitorFactory;
            this.handlerFactory = handlerFactory;
            services = new Queue<FolderMonitor>();
            InitializeComponent();
        }

        public void Start()
        {
            var section = FileWatcherConfigSection.Instance;
            if (section == null) return;
            foreach (var setting in section.Handlers)
            {
                var handler = handlerFactory
                    .CreateHandler(setting.Value.HandlerName, setting.Value.Host);

                var folderMonitor = folderMonitorFactory.CreateFolderMonitor(setting.Key, handler);

                folderMonitor.StartMonitoring();
                services.Enqueue(folderMonitor);
            }            
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        protected override void OnStop()
        {
            while (services.Count > 0)
            {
                var service = services.Dequeue();
                try
                {
                    service.Dispose();
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, PolicyName.Default);
                }
            }
        }
    }
}