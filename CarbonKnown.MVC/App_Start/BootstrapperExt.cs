using Microsoft.Practices.Unity;
using CarbonKnown.MVC.Service;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace CarbonKnown.MVC.App_Start
{
    public static partial class Bootstrapper
    {
		public static void RegisterDataEntryServices(IUnityContainer container)
		{
            container.RegisterType<CarbonKnown.WCF.Accommodation.IAccommodationService, Accommodation>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<CarbonKnown.WCF.AirTravelRoute.IAirTravelRouteService, AirTravelRoute>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<CarbonKnown.WCF.AirTravel.IAirTravelService, AirTravel>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<CarbonKnown.WCF.CarHire.ICarHireService, CarHire>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<CarbonKnown.WCF.Commuting.ICommutingService, Commuting>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<CarbonKnown.WCF.CourierRoute.ICourierRouteService, CourierRoute>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<CarbonKnown.WCF.Courier.ICourierService, Courier>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<CarbonKnown.WCF.Electricity.IElectricityService, Electricity>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<CarbonKnown.WCF.Fuel.IFuelService, Fuel>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<CarbonKnown.WCF.Paper.IPaperService, Paper>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<CarbonKnown.WCF.Refrigerant.IRefrigerantService, Refrigerant>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<CarbonKnown.WCF.Fleet.IFleetService, Fleet>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<CarbonKnown.WCF.Waste.IWasteService, Waste>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
            container.RegisterType<CarbonKnown.WCF.Water.IWaterService, Water>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());
		}
    }
}
