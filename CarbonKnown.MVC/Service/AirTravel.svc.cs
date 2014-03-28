using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.DAL.Models.AirTravel;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.AirTravel;
using TravelClass = CarbonKnown.DAL.Models.AirTravel.TravelClass;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class AirTravel :
        DataEntryServiceBase<AirTravelData, AirTravelDataContract>,
        IAirTravelService
    {
		protected static readonly Guid CalculationId = new Guid("89a1c672-f4fb-4753-974d-26dd86deb0db");

	    public AirTravel(ISourceDataContext context, ICalculationFactory calculationFactory) : base(context, calculationFactory)
	    {
	    }

	    public override Guid GetCalculationId()
	    {
	        return CalculationId;
	    }        
		
		public override void SetEntryValues(AirTravelData instance, AirTravelDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.TravelClass = (TravelClass)dataEntry.TravelClass;
        }
    }
}
