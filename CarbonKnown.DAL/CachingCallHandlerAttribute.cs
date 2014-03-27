using System;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace CarbonKnown.DAL
{
    public class CachingCallHandlerAttribute : HandlerAttribute, ICallHandler
    {
        private readonly ICacheKeyGenerator keyGenerator = new DefaultCacheKeyGenerator();

        public CachingCallHandlerAttribute()
        {
            ExpirationTime = TimeSpan.Zero;
        }

        public CachingCallHandlerAttribute(int hours, int minutes, int seconds)
        {
            ExpirationTime = new TimeSpan(hours, minutes, seconds);
        }

        public CachingCallHandlerAttribute(int days, int hours, int minutes, int seconds, int milliseconds)
        {
            ExpirationTime = new TimeSpan(days, hours, minutes, seconds, milliseconds);
        }

        public CachingCallHandlerAttribute(int days, int hours, int minutes, int seconds)
        {
            ExpirationTime = new TimeSpan(days, hours, minutes, seconds);
        }

        public string RegionName { get; set; }
        public CacheItemPolicy Policy { get; set; }
        public TimeSpan ExpirationTime { get; set; }

        #region ICallHandler Members

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            if (TargetMethodReturnsVoid(input))
            {
                return getNext()(input, getNext);
            }

            var inputs = new object[input.Inputs.Count];
            for (var i = 0; i < inputs.Length; ++i)
            {
                inputs[i] = input.Inputs[i];
            }

            var cacheKey = keyGenerator.CreateCacheKey(input.MethodBase, inputs);

            var cachedResult = MemoryCache.Default.Get(cacheKey);

            if (cachedResult == null)
            {
                var realReturn = getNext()(input, getNext);
                if (realReturn.Exception == null)
                {
                    AddToCache(cacheKey, realReturn.ReturnValue);
                }
                return realReturn;
            }

            var cachedReturn = input.CreateMethodReturn(cachedResult, input.Arguments);
            return cachedReturn;
        }

        private static bool TargetMethodReturnsVoid(IMethodInvocation input)
        {
            var targetMethod = input.MethodBase as MethodInfo;
            return targetMethod != null && targetMethod.ReturnType == typeof (void);
        }

        #endregion

        private void AddToCache(string key, object value)
        {
            if (Policy != null)
            {
                MemoryCache.Default.Add(key, value, Policy, RegionName);
            }
            var offset = (ExpirationTime == TimeSpan.Zero)
                             ? DateTimeOffset.MaxValue
                             : new DateTimeOffset(DateTime.Now, ExpirationTime);
            MemoryCache.Default.Add(key, value, offset, RegionName);
        }

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            var handler = new CachingCallHandlerAttribute
                {
                    RegionName = RegionName,
                    ExpirationTime = ExpirationTime,
                    Policy = Policy
                };

            if ((Policy == null) && (container.IsRegistered<CacheItemPolicy>()))
            {
                handler.Policy = container.Resolve<CacheItemPolicy>();
            }

            return handler;
        }


        /// <summary>
        /// This interface describes classes that can be used to generate cache key strings
        /// for the <see cref="CachingCallHandlerAttribute"/>.
        /// </summary>
        public interface ICacheKeyGenerator
        {
            /// <summary>
            /// Creates a cache key for the given method and set of input arguments.
            /// </summary>
            /// <param name="method">Method being called.</param>
            /// <param name="inputs">Input arguments.</param>
            /// <returns>A (hopefully) unique string to be used as a cache key.</returns>
            string CreateCacheKey(MethodBase method, object[] inputs);
        }

        /// <summary>
        /// The default <see cref="ICacheKeyGenerator"/> used by the <see cref="CachingCallHandlerAttribute"/>.
        /// </summary>
        public class DefaultCacheKeyGenerator : ICacheKeyGenerator
        {
            #region ICacheKeyGenerator Members

            /// <summary>
            /// Create a cache key for the given method and set of input arguments.
            /// </summary>
            /// <param name="method">Method being called.</param>
            /// <param name="inputs">Input arguments.</param>
            /// <returns>A (hopefully) unique string to be used as a cache key.</returns>
            public string CreateCacheKey(MethodBase method, params object[] inputs)
            {
                var sb = new StringBuilder();
                if (method.DeclaringType != null)
                {
                    sb.Append(method.DeclaringType.FullName);
                }
                sb.Append(':');
                sb.Append(method.Name);

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

            #endregion
        }
    }
}
