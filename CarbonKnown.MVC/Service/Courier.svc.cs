using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.DAL.Models.Courier;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.Courier;
using ServiceType = CarbonKnown.DAL.Models.Courier.ServiceType;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class Courier :
        DataEntryServiceBase<CourierData, CourierDataContract>,
        ICourierService
    {
		protected static readonly Guid CalculationId = new Guid("44a2cc3f-d365-4e78-a3bc-eea835f59afb");

	    public Courier(ISourceDataContext context, ICalculationFactory calculationFactory) : base(context, calculationFactory)
	    {
	    }

	    public override Guid GetCalculationId()
	    {
	        return CalculationId;
	    }        
		
		public override void SetEntryValues(CourierData instance, CourierDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.ServiceType = (ServiceType?)dataEntry.ServiceType;
            instance.ChargeMass = dataEntry.ChargeMass;
        }
    }
}
