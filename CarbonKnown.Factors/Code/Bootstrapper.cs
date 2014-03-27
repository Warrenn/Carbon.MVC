using System;
using System.Configuration;
using System.Web.Mvc;
using CarbonKnown.Factors.WCF;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension;
using Unity.Mvc4;

namespace CarbonKnown.Factors.Code
{
    public static class Bootstrapper
    {
        private static readonly Lazy<IUnityContainer> LazyContainer
            = new Lazy<IUnityContainer>(BuildUnityContainer);

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
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            RegisterTypes(container);
            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IFactorsService, Service.Factors>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
        }
    }
}