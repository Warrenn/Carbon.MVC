using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.Fleet;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class Fleet :
        DataEntryServiceBase<CarbonKnown.DAL.Models.Fleet.FleetData, FleetDataContract>,
        IFleetService
    {
		protected static readonly Guid CalculationId = new Guid("3c13f9c9-e7e6-4aed-8b0d-55ff484f8d8f");

	    public Fleet(ISourceDataContext context, ICalculationFactory calculationFactory) : base(context, calculationFactory)
	    {
	    }

	    public override Guid GetCalculationId()
	    {
	        return CalculationId;
	    }        
		
		public override void SetEntryValues(CarbonKnown.DAL.Models.Fleet.FleetData instance, FleetDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.Scope = (CarbonKnown.DAL.Models.Fleet.FleetScope?)dataEntry.Scope;
            instance.FuelType = (CarbonKnown.DAL.Models.FuelType?)dataEntry.FuelType;
        }
    }
}
