using CarbonKnown.FileWatcherService.AvisCourier;
using CarbonKnown.FileWatcherService.Constants;
using CarbonKnown.FileWatcherService.FileHandler;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace CarbonKnown.FileReaders
{
    public static class Bootstrapper
    {
        private static  IUnityContainer internalContainer;

        private static void RegisterHandlers(IUnityContainer container)
        {
            RegisterHandler<AvisCourierHandler>(container, HandlerName.AvisCourierHandler);
            internalContainer = container;
        }

        public static IFileHandler ResolveHandler(string handlerName)
        {
            return internalContainer.Resolve<IFileHandler>(handlerName);
        }

        public static IFileHandler ResolveHandler(string handlerName, string host)
        {
            return internalContainer.Resolve<IFileHandler>(
                handlerName,
                new ParameterOverride("host", host));
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
