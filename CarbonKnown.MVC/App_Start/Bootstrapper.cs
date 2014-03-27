using System;
using System.Configuration;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Mvc;
using CarbonKnown.Calculation;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.DAL;
using CarbonKnown.Factors.WCF;
using CarbonKnown.FileReaders;
using CarbonKnown.MVC.BLL;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Controllers;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.DataSource;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension;
using DataSourceService = CarbonKnown.MVC.Service.DataSourceService;

namespace CarbonKnown.MVC.App_Start
{
    public static partial class Bootstrapper
    {
        private static readonly Lazy<IUnityContainer> LazyContainer
            = new Lazy<IUnityContainer>(BuildUnityContainer);

        public static IUnityContainer Container
        {
            get { return LazyContainer.Value; }
        }

        public static void Initialize(HttpConfiguration config)
        {
            var container = Container;
            var mvc4Resolver = new Unity.Mvc4.UnityDependencyResolver(container);
            DependencyResolver.SetResolver(mvc4Resolver);
            var previousResolver = DependencyResolver.Current;
            var newResolver = new UnityDependencyResolver(container, previousResolver);
            DependencyResolver.SetResolver(newResolver);
            config.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var section = (UnityConfigurationSection)
                          ConfigurationManager.GetSection(UnityConfigurationSection.SectionName);
            var returnContainer = new UnityContainer();
            var container = section.Configure(returnContainer);
            container.RegisterInstance(container);
            container.AddNewExtension<Interception>();
            RegisterTypes(container);

            var config = ConfigurationSourceFactory.Create();
            container.RegisterInstance(CreateLogWriter(config));
            container.RegisterInstance(CreateExceptionManager(config));
            return container;
        }

        public static IFactorsService CreateFactorsService()
        {
            var factory = new ChannelFactory<IFactorsService>(Constants.Constants.FactorsEndpointName);
            var client = factory.CreateChannel();
            return client;
        }

        private static ExceptionManager CreateExceptionManager(IConfigurationSource config)
        {
            var factory = new ExceptionPolicyFactory(config);
            var manger = factory.CreateManager();
            ExceptionPolicy.SetExceptionManager(manger);
            return manger;
        }

        private static LogWriter CreateLogWriter(IConfigurationSource config)
        {
            var factory = new LogWriterFactory(config);
            var writer = factory.Create();
            Logger.SetLogWriter(writer);
            return writer;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            HandlerFactory.RegisterHandlers(container);
            CalculationFactory.RegisterCalculations(container);
            RegisterDataEntryServices(container);

            container.RegisterType<IFactorsService>(
                new InjectionFactory(c => CreateFactorsService()));

            container.RegisterType<IAccountService, AccountService>(
                new HierarchicalLifetimeManager(),
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());

            container.RegisterType<ISourceDataContext, SourceDataContext>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());

            container.RegisterType<ICalculationDataContext, CalculationDataContext>(
                new HierarchicalLifetimeManager(),
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());

            container.RegisterType<IDataEntriesUnitOfWork, DataEntriesUnitOfWork>(
                new HierarchicalLifetimeManager(),
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());

            container.RegisterType<IDataSourceService, DataSourceService>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());

            container.RegisterType<DataContext, DataContext>();
            container.RegisterType<ISliceService, SliceService>();
            container.RegisterType<IEmailManager, EmailManager>();
            container.RegisterType<ICalculationFactory, CalculationFactory>();
            container.RegisterType<IHandlerFactory, HandlerFactory>();
            container.RegisterType<IStreamManager, StreamManager>();
            container.RegisterType<ITreeWalkService, TreeWalkService>();
            container.RegisterType<TreeWalkController, TreeWalkController>();
            container.RegisterType<OverviewReportController, OverviewReportController>();
        }
    }
}