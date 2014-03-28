using CarbonKnown.MVC.App_Start;
using Microsoft.Practices.Unity;
using Unity.Wcf;

namespace CarbonKnown.MVC.Code
{
	public class WcfServiceFactory : UnityServiceHostFactory
    {
        protected override void ConfigureContainer(IUnityContainer container)
        {
			Bootstrapper.RegisterTypes(container);
        }
    }    
}