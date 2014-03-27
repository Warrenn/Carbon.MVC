using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace CarbonKnown.Calculation
{
    public class CalculationFactory : ICalculationFactory
    {
        private readonly IUnityContainer container;

        public CalculationFactory(IUnityContainer container)
        {
            this.container = container;
        }

        public static void RegisterCalculations(IUnityContainer container)
        {
            var interfaceType = typeof (ICalculation);
            foreach (var calculation in CalculationModelFactory.Calculations)
            {
                container.RegisterType(
                    interfaceType,
                    calculation.Key,
                    calculation.Value.Id.ToString(),
                    new InterceptionBehavior<PolicyInjectionBehavior>(),
                    new Interceptor<InterfaceInterceptor>());
            }
            container.RegisterType<ICalculationFactory, CalculationFactory>();
        }

        public virtual ICalculation ResolveCalculation(Guid calculationId)
        {
            return container.Resolve<ICalculation>(calculationId.ToString());
        }
    }
}
