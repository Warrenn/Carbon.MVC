using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.Waste;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class Waste :
        DataEntryServiceBase<CarbonKnown.DAL.Models.Waste.WasteData, WasteDataContract>,
        IWasteService
    {
		protected static readonly Guid CalculationId = new Guid("fb995404-7f1c-4041-97d7-f5bb8ec5b7bd");

	    public Waste(ISourceDataContext context, ICalculationFactory calculationFactory) : base(context, calculationFactory)
	    {
	    }

	    public override Guid GetCalculationId()
	    {
	        return CalculationId;
	    }        
		
		public override void SetEntryValues(CarbonKnown.DAL.Models.Waste.WasteData instance, WasteDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.WasteType = (CarbonKnown.DAL.Models.Waste.WasteType?)dataEntry.WasteType;
        }
    }
}
