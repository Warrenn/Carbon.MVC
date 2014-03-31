using CarbonKnown.DAL.Models;

namespace CarbonKnown.MVC.Models
{
    public class EmissionSummaryModel
    {
        public ActivityGroup ActivityGroup { get; set; }
        public CostCentre CostCentre { get; set; }
        public CarbonEmissionEntry Entry { get; set; }
    }
}
