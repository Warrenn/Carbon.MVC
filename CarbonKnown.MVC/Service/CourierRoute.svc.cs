using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.CourierRoute;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class CourierRoute :
        DataEntryServiceBase<CarbonKnown.DAL.Models.Courier.CourierRouteData, CourierRouteDataContract>,
        ICourierRouteService
    {
		protected static readonly Guid CalculationId = new Guid("183d2234-8a1d-4599-8ac7-dd5fcc5af819");

	    public CourierRoute(ISourceDataContext context, ICalculationFactory calculationFactory) : base(context, calculationFactory)
	    {
	    }

	    public override Guid GetCalculationId()
	    {
	        return CalculationId;
	    }        
		
		public override void SetEntryValues(CarbonKnown.DAL.Models.Courier.CourierRouteData instance, CourierRouteDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.ServiceType = (CarbonKnown.DAL.Models.Courier.ServiceType?)dataEntry.ServiceType;
            instance.ChargeMass = dataEntry.ChargeMass;
            instance.FromCode = dataEntry.FromCode;
            instance.ToCode = dataEntry.ToCode;
            instance.Reversal = dataEntry.Reversal;
        }
    }
}
