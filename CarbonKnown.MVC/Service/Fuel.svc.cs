using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.Fuel;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class Fuel :
        DataEntryServiceBase<CarbonKnown.DAL.Models.Fuel.FuelData, FuelDataContract>,
        IFuelService
    {
		protected static readonly Guid CalculationId = new Guid("d70ed5bc-6ad9-45e7-9498-27c0f131b449");

	    public Fuel(ISourceDataContext context, ICalculationFactory calculationFactory) : base(context, calculationFactory)
	    {
	    }

	    public override Guid GetCalculationId()
	    {
	        return CalculationId;
	    }        
		
		public override void SetEntryValues(CarbonKnown.DAL.Models.Fuel.FuelData instance, FuelDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.FuelType = (CarbonKnown.DAL.Models.FuelType?)dataEntry.FuelType;
            instance.UOM = (CarbonKnown.DAL.Models.Fuel.UnitOfMeasure?)dataEntry.UOM;
        }
    }
}
