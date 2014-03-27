using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace CarbonKnown.MVC.Code
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer container;
        private readonly IDependencyResolver previous;

        public UnityDependencyResolver(IUnityContainer container, IDependencyResolver previous)
        {
            this.container = container;
            this.previous = previous;
        }

        public object GetService(Type serviceType)
        {
            return container.IsRegistered(serviceType)
                       ? container.Resolve(serviceType, string.Empty)
                       : previous.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return container.IsRegistered(serviceType)
                       ? container.ResolveAll(serviceType)
                       : previous.GetServices(serviceType);
        }
    }
}