using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.Accommodation;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class Accommodation :
        DataEntryServiceBase<CarbonKnown.DAL.Models.Accommodation.AccommodationData, AccommodationDataContract>,
        IAccommodationService
    {
		protected static readonly Guid CalculationId = new Guid("263129dc-29c2-40ed-8184-e9f1083fe2a8");

	    public Accommodation(ISourceDataContext context, ICalculationFactory calculationFactory) : base(context, calculationFactory)
	    {
	    }

	    public override Guid GetCalculationId()
	    {
	        return CalculationId;
	    }        
		
		public override void SetEntryValues(CarbonKnown.DAL.Models.Accommodation.AccommodationData instance, AccommodationDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
        }
    }
}
