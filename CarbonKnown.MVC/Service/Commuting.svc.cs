using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.Commuting;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class Commuting :
        DataEntryServiceBase<CarbonKnown.DAL.Models.Commuting.CommutingData, CommutingDataContract>,
        ICommutingService
    {
		protected static readonly Guid CalculationId = new Guid("fb1c2b6d-39b9-4153-995b-1cdd6823bd71");

	    public Commuting(ISourceDataContext context, ICalculationFactory calculationFactory) : base(context, calculationFactory)
	    {
	    }

	    public override Guid GetCalculationId()
	    {
	        return CalculationId;
	    }        
		
		public override void SetEntryValues(CarbonKnown.DAL.Models.Commuting.CommutingData instance, CommutingDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.CommutingType = (CarbonKnown.DAL.Models.Commuting.CommutingType?)dataEntry.CommutingType;
        }
    }
}
