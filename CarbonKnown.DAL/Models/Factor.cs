using System;
using System.Collections.Generic;

namespace CarbonKnown.DAL.Models
{
    public class Factor
    {
        public Guid Id { get; set; }

        public virtual ICollection<Calculation> Collection { get; set; }
    }
}
