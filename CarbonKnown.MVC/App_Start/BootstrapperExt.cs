using CarbonKnown.WCF.Accommodation;
using CarbonKnown.WCF.AirTravel;
using CarbonKnown.WCF.AirTravelRoute;
using CarbonKnown.WCF.CarHire;
using CarbonKnown.WCF.Commuting;
using CarbonKnown.WCF.Courier;
using CarbonKnown.WCF.CourierRoute;
using CarbonKnown.WCF.Electricity;
using CarbonKnown.WCF.Fleet;
using CarbonKnown.WCF.Fuel;
using CarbonKnown.WCF.Paper;
using CarbonKnown.WCF.Refrigerant;
using CarbonKnown.WCF.Waste;
using CarbonKnown.WCF.Water;
using Microsoft.Practices.Unity;
using CarbonKnown.MVC.Service;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace CarbonKnown.MVC.App_Start
{
    public static partial class Bootstrapper
    {
		public static void RegisterDataEntryServices(IUnityContainer container)
		{
            container.RegisterType<IAccommodationService, Accommodation>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IAirTravelRouteService, AirTravelRoute>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IAirTravelService, AirTravel>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<ICarHireService, CarHire>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<ICommutingService, Commuting>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<ICourierRouteService, CourierRoute>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<ICourierService, Courier>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IElectricityService, Electricity>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IFuelService, Fuel>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IPaperService, Paper>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IRefrigerantService, Refrigerant>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IFleetService, Fleet>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IWasteService, Waste>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IWaterService, Water>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
		}
    }
}
