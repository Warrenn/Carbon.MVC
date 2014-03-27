using System;
using System.Diagnostics;
using System.ServiceProcess;
using CarbonKnown.FileReaders.Constants;
using CarbonKnown.FileWatcherService.Properties;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.Unity;

namespace CarbonKnown.FileWatcherService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            try
            {
                var container = Bootstrapper.Container;
                var mainService = container.Resolve<ServiceBase>();
                var servicesToRun = new[] {mainService};
                ServiceBase.Run(servicesToRun);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, PolicyName.Default);
            }
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex == null) return;
            Trace.TraceError(Resources.ExceptionErrorMessage, ex.Message, ex.StackTrace);
        }
    }
}
