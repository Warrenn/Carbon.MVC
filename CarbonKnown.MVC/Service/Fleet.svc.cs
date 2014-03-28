using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.DAL.Models.Fleet;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.Fleet;
using FleetScope = CarbonKnown.DAL.Models.Fleet.FleetScope;
using FuelType = CarbonKnown.DAL.Models.FuelType;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class Fleet :
        DataEntryServiceBase<FleetData, FleetDataContract>,
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
		
		public override void SetEntryValues(FleetData instance, FleetDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.Scope = (FleetScope?)dataEntry.Scope;
            instance.FuelType = (FuelType?)dataEntry.FuelType;
        }
    }
}
