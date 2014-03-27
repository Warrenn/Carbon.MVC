using System;
using System.Collections.Generic;

namespace CarbonKnown.DAL.Models
{
    public class Calculation
    {
        public Guid Id { get; set; }
        public virtual ICollection<Factor> Factors { get; set; }
        public virtual ICollection<DataEntry> DataEntries { get; set; }
        public virtual ICollection<ActivityGroup> ActivityGroups { get; set; }
        public virtual ConsumptionType ConsumptionType { get; set; }
        public string AssemblyQualifiedName { get; set; }
        public string Name { get; set; }
    }
}
