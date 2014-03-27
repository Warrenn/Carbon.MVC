using Microsoft.Practices.Unity;
using Unity.Wcf;

namespace CarbonKnown.Factors.Code
{
    public class WcfServiceFactory : UnityServiceHostFactory
    {
        protected override void ConfigureContainer(IUnityContainer container)
        {
            Bootstrapper.RegisterTypes(container);
        }
    }
}