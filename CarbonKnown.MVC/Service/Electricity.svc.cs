using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.Electricity;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class Electricity :
        DataEntryServiceBase<CarbonKnown.DAL.Models.Electricity.ElectricityData, ElectricityDataContract>,
        IElectricityService
    {
		protected static readonly Guid CalculationId = new Guid("c1809f62-369d-413f-a643-2489fdb8d3a3");

	    public Electricity(ISourceDataContext context, ICalculationFactory calculationFactory) : base(context, calculationFactory)
	    {
	    }

	    public override Guid GetCalculationId()
	    {
	        return CalculationId;
	    }        
		
		public override void SetEntryValues(CarbonKnown.DAL.Models.Electricity.ElectricityData instance, ElectricityDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.ElectricityType = (CarbonKnown.DAL.Models.Electricity.ElectricityType?)dataEntry.ElectricityType;
        }
    }
}
