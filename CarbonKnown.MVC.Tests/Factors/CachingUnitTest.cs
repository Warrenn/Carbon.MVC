using System;
using System.ServiceModel;
using CarbonKnown.Factors.WCF;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarbonKnown.MVC.Tests.Factors
{
    [TestClass]
    public class CachingUnitTest
    {
        [TestMethod]
        public void CachingMethodsMustBeCalledOnlyOnceForTheSameParameters()
        {
            //Arrange
            var url = Guid.NewGuid().ToString();
            int firstResult;
            int secondResult;

            var localPipe = new Uri("net.pipe://localhost/" + url);
            using (var serviceHost = new ServiceHost(typeof (TestObject), localPipe))
            {
                serviceHost.Open();
                var factory = new ChannelFactory<ITestObject>(new NetNamedPipeBinding(), new EndpointAddress(localPipe));
                var client = factory.CreateChannel();
                
                //Act
                firstResult = client.GetCacheValue(1);
                secondResult = client.GetCacheValue(1);

                serviceHost.Close();
            }
            //Assert
            Assert.AreEqual(firstResult, secondResult);
        }

        [TestMethod]
        public void CachingMethodsMustNotCacheForDifferentParameters()
        {
            //Arrange
            var url = Guid.NewGuid().ToString();
            int firstResult;
            int secondResult;

            var localPipe = new Uri("net.pipe://localhost/" + url);
            using (var serviceHost = new ServiceHost(typeof(TestObject), localPipe))
            {
                serviceHost.Open();
                var factory = new ChannelFactory<ITestObject>(new NetNamedPipeBinding(), new EndpointAddress(localPipe));
                var client = factory.CreateChannel();

                //Act
                firstResult = client.GetCacheValue(1);
                secondResult = client.GetCacheValue(2);

                serviceHost.Close();
            }
            //Assert
            Assert.AreNotEqual(firstResult, secondResult);
        }

        [ServiceContract]
        public interface ITestObject
        {
            [Caching]
            [OperationContract]
            int GetCacheValue(int parameter);
        }

        public class TestObject : ITestObject
        {
            private static int cacheValue;
            public int GetCacheValue(int parameter)
            {
                cacheValue++;
                return cacheValue;
            }
        }
    }
}
