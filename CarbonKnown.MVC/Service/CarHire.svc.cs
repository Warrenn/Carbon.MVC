using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.CarHire;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class CarHire :
        DataEntryServiceBase<CarbonKnown.DAL.Models.CarHire.CarHireData, CarHireDataContract>,
        ICarHireService
    {
		protected static readonly Guid CalculationId = new Guid("d60848df-7852-4bff-8c80-ecdb21ae328b");

	    public CarHire(ISourceDataContext context, ICalculationFactory calculationFactory) : base(context, calculationFactory)
	    {
	    }

	    public override Guid GetCalculationId()
	    {
	        return CalculationId;
	    }        
		
		public override void SetEntryValues(CarbonKnown.DAL.Models.CarHire.CarHireData instance, CarHireDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.CarGroupBill = (CarbonKnown.DAL.Models.CarHire.CarGroupBill?)dataEntry.CarGroupBill;
        }
    }
}
