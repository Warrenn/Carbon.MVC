using Microsoft.Practices.Unity;
using Unity.Wcf;

namespace CarbonKnown.MVC.Code
{
	public class WcfServiceFactory : UnityServiceHostFactory
    {
        protected override void ConfigureContainer(IUnityContainer container)
        {
			App_Start.Bootstrapper.RegisterTypes(container);
        }
    }    
}