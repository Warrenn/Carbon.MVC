using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.DAL.Models.Water;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.Water;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class Water :
        DataEntryServiceBase<WaterData, WaterDataContract>,
        IWaterService
    {
		protected static readonly Guid CalculationId = new Guid("2b8cf591-19ab-4251-b083-2db440972f23");

	    public Water(ISourceDataContext context, ICalculationFactory calculationFactory) : base(context, calculationFactory)
	    {
	    }

	    public override Guid GetCalculationId()
	    {
	        return CalculationId;
	    }        
		
		public override void SetEntryValues(WaterData instance, WaterDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
        }
    }
}
