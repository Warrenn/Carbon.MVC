using System;
using System.Runtime.Caching;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace CarbonKnown.Factors.WCF
{
    public class CachingOperationInvoker : IOperationInvoker
    {
        private readonly IOperationInvoker previousInvoker;
        private readonly string operationName;
        private readonly TimeSpan expirationTime;
        private readonly string regionName;

        public CachingOperationInvoker(
            string operationName,
            IOperationInvoker previousInvoker,
            TimeSpan expirationTime,
            string regionName)
        {
            this.previousInvoker = previousInvoker;
            this.operationName = operationName;
            this.expirationTime = expirationTime;
            this.regionName = regionName;
        }

        public object[] AllocateInputs()
        {
            return previousInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            var cacheKey = CreateCacheKey(operationName, inputs);

            if (MemoryCache.Default.Contains(cacheKey, regionName))
            {
                var cachedValue = (CacheObject) MemoryCache.Default.Get(cacheKey);
                outputs = cachedValue.Outputs;
                return cachedValue.ReturnValue;
            }

            var returnValue = previousInvoker.Invoke(instance, inputs, out outputs);
            var cacheValue = new CacheObject
                {
                    Outputs = outputs,
                    ReturnValue = returnValue
                };
            AddToCache(cacheKey, cacheValue);
            return returnValue;
        }

        private void AddToCache(string key, object value)
        {
            var offset = (expirationTime == TimeSpan.Zero)
                             ? DateTimeOffset.MaxValue
                             : new DateTimeOffset(DateTime.Now, expirationTime);
            MemoryCache.Default.Add(key, value, offset, regionName);
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return previousInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return previousInvoker.InvokeEnd(instance, out outputs, result);
        }

        public bool IsSynchronous
        {
            get { return previousInvoker.IsSynchronous; }
        }

        public string CreateCacheKey(string method, params object[] inputs)
        {
            var sb = new StringBuilder(method);

            if (inputs != null)
            {
                foreach (var input in inputs)
                {
                    sb.Append(':');
                    if (input != null)
                    {
                        sb.Append(input.GetHashCode().ToString());
                    }
                }
            }

            return sb.ToString();
        }

        public class CacheObject
        {
            public object ReturnValue { get; set; }
            public object[] Outputs { get; set; }
        }
    }
}
