using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace CarbonKnown.Factors.WCF
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CachingAttribute : Attribute, IOperationBehavior
    {
        public string RegionName { get; set; }
        public TimeSpan ExpirationTime { get; set; }

        public CachingAttribute()
        {
            ExpirationTime = TimeSpan.Zero;
        }

        public CachingAttribute(int hours, int minutes, int seconds)
        {
            ExpirationTime = new TimeSpan(hours, minutes, seconds);
        }

        public CachingAttribute(int days, int hours, int minutes, int seconds, int milliseconds)
        {
            ExpirationTime = new TimeSpan(days, hours, minutes, seconds, milliseconds);
        }

        public CachingAttribute(int days, int hours, int minutes, int seconds)
        {
            ExpirationTime = new TimeSpan(days, hours, minutes, seconds);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            var previousInvoker = dispatchOperation.Invoker;
            var methodName = operationDescription.Name;
            var newInvoker = new CachingOperationInvoker(methodName, previousInvoker, ExpirationTime, RegionName);
            dispatchOperation.Invoker = newInvoker;
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void AddBindingParameters(OperationDescription operationDescription,
                                         BindingParameterCollection bindingParameters)
        {
        }
    }
}
