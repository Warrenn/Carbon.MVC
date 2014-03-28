using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.DAL.Models.Refrigerant;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.Refrigerant;
using RefrigerantType = CarbonKnown.DAL.Models.Refrigerant.RefrigerantType;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class Refrigerant :
        DataEntryServiceBase<RefrigerantData, RefrigerantDataContract>,
        IRefrigerantService
    {
		protected static readonly Guid CalculationId = new Guid("dc5b2b93-9267-4119-b0ec-5c7407ebd230");

	    public Refrigerant(ISourceDataContext context, ICalculationFactory calculationFactory) : base(context, calculationFactory)
	    {
	    }

	    public override Guid GetCalculationId()
	    {
	        return CalculationId;
	    }        
		
		public override void SetEntryValues(RefrigerantData instance, RefrigerantDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.RefrigerantType = (RefrigerantType?)dataEntry.RefrigerantType;
        }
    }
}
