using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.Refrigerant;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class Refrigerant :
        DataEntryServiceBase<CarbonKnown.DAL.Models.Refrigerant.RefrigerantData, RefrigerantDataContract>,
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
		
		public override void SetEntryValues(CarbonKnown.DAL.Models.Refrigerant.RefrigerantData instance, RefrigerantDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.RefrigerantType = (CarbonKnown.DAL.Models.Refrigerant.RefrigerantType?)dataEntry.RefrigerantType;
        }
    }
}
