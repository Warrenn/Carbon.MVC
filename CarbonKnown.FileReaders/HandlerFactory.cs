using CarbonKnown.FileReaders.AvisCourier;
using CarbonKnown.FileReaders.Constants;
using CarbonKnown.FileReaders.Courier;
using CarbonKnown.FileReaders.EzShuttle;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.FileReaders.Fleet;
using CarbonKnown.FileReaders.Generic;
using CarbonKnown.FileReaders.LibertyAirTickets;
using CarbonKnown.FileReaders.LibertyAvis;
using CarbonKnown.FileReaders.LibertyEuroCar;
using CarbonKnown.FileReaders.MondiPaper;
using CarbonKnown.FileReaders.Rennies;
using CarbonKnown.FileReaders.TWF;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace CarbonKnown.FileReaders
{
    public class HandlerFactory : IHandlerFactory
    {
        private readonly IUnityContainer container;

        public HandlerFactory(IUnityContainer container)
        {
            this.container = container;
        }

        public static void RegisterHandlers(IUnityContainer container)
        {
            container.RegisterType<IHandlerFactory, HandlerFactory>();
            RegisterHandler<AvisCourierHandler>(container, HandlerName.AvisCourierHandler);
            RegisterHandler<CourierHandler>(container, HandlerName.CourierHandler);
            RegisterHandler<FleetHandler>(container, HandlerName.FleetHandler);
            RegisterHandler<MondiPaperHandler>(container, HandlerName.MondiPaperHandler);
            RegisterHandler<RenniesHandler>(container, HandlerName.RenniesHandler);
            RegisterHandler<TWFHandler>(container, HandlerName.TWFHandler);
            RegisterHandler<GenericHandler>(container, HandlerName.GenericHandler);
            RegisterHandler<LibertyAirTicketsHandler>(container, HandlerName.LibertyAirTicketsHandler);
            RegisterHandler<LibertyAvisHandler>(container, HandlerName.LibertyAvisHandler);
            RegisterHandler<LibertyEuroCarHandler>(container, HandlerName.LibertyEuroCarHandler);
            RegisterHandler<EzShuttleHandler>(container, HandlerName.EzShuttleHandler);
        }

        public IFileHandler CreateHandler(string handlerName)
        {
            return CreateHandler(handlerName, string.Empty);
        }

        public  IFileHandler CreateHandler(string handlerName, string host)
        {
            return container
                       .IsRegistered(typeof (IFileHandler), handlerName)
                       ? container.Resolve<IFileHandler>(handlerName, new ParameterOverride("host", host))
                       : null;
        }

        private static void RegisterHandler<THandler>(IUnityContainer container, string handlerName)
            where THandler : IFileHandler
        {
            container.RegisterType<IFileHandler, THandler>(
                handlerName,
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());

        }
    }
}
