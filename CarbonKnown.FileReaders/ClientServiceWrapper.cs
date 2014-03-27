using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using CarbonKnown.FileReaders.Constants;
using CarbonKnown.FileReaders.Properties;

namespace CarbonKnown.FileReaders
{
    public class ClientServiceWrapper<TService> : IDisposable
        where TService : class
    {
        protected readonly string BaseUrl;
        protected readonly string ConfigurationName;
        protected ChannelFactory<TService> Factory;
        protected ChannelEndpointElement EndpointElement; 
        private TService service;

        public ClientServiceWrapper()
            : this(string.Empty, string.Empty)
        {
        }

        public ClientServiceWrapper(string baseUrl)
            : this(baseUrl, string.Empty)
        {
        }

        public ClientServiceWrapper(string baseUrl, string configurationName)
        {
            BaseUrl = baseUrl;
            ConfigurationName = configurationName;
            EndpointElement = GetEndPointElement(configurationName);
            Factory = new ChannelFactory<TService>(EndpointElement.Name);
            Service = CreateService(baseUrl, Factory, EndpointElement);
        }

        private static TService CreateService(string baseUrl, IChannelFactory<TService> factory, ChannelEndpointElement endPoint)
        {
            var address = endPoint.Address.AbsoluteUri;
            var serviceUri = address.Replace("domain", baseUrl);
            return factory.CreateChannel(new EndpointAddress(serviceUri));
        }

        private static ChannelEndpointElement GetEndPointElement(string configurationName)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var sectionGroup = ServiceModelSectionGroup.GetSectionGroup(config);
            if (sectionGroup == null)
                throw new ConfigurationErrorsException(Resources.GroupExpectedErrorMessage);

            var client = sectionGroup.Client;
            var contractType = typeof (TService);
            var endPoint = client.Endpoints.OfType<ChannelEndpointElement>()
                                 .FirstOrDefault(ep =>
                                                 (ep.Name == configurationName) ||
                                                 (ep.Contract == contractType.Name) ||
                                                 (ep.Contract == contractType.AssemblyQualifiedName) ||
                                                 (ep.Contract == contractType.FullName));
            if (endPoint != null) return endPoint;

            var errorMessage = string.Format(Resources.EndPointMissingErrorMessage,
                                             contractType.AssemblyQualifiedName);
            throw new ConfigurationErrorsException(errorMessage);
        }

        public TService Service
        {
            get
            {
                var communicationObject = service as ICommunicationObject;
                if ((communicationObject != null) && (communicationObject.State == CommunicationState.Opened))
                    return service;
                service = CreateService(BaseUrl, Factory, EndpointElement);
                return service;
            }
            protected set { service = value; }
        }

        public virtual void Dispose()
        {
            ((IDisposable) service).Dispose(PolicyName.Disposable);
            Factory.Dispose(PolicyName.Disposable);
            Service = null;
            Factory = null;
        }
    }
}
