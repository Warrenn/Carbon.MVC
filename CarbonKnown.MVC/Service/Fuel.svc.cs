using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.DAL.Models.Fuel;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.Fuel;
using FuelType = CarbonKnown.DAL.Models.FuelType;
using UnitOfMeasure = CarbonKnown.DAL.Models.Fuel.UnitOfMeasure;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class Fuel :
        DataEntryServiceBase<FuelData, FuelDataContract>,
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
		
		public override void SetEntryValues(FuelData instance, FuelDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.FuelType = (FuelType?)dataEntry.FuelType;
            instance.UOM = (UnitOfMeasure?)dataEntry.UOM;
        }
    }
}
