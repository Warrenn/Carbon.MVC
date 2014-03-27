using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbonKnown.Factors.Models
{
    public class Factor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FactorId { get; set; }

        public string FactorGroup { get; set; }
        public string FactorName { get; set; }
        public ICollection<FactorValue> FactorValues { get; set; } 
    }
}
