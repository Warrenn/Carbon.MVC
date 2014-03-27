using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbonKnown.Factors.Models
{
    public class FactorValue
    {
        [Key]
        [Column(Order = 0)]
        public Guid FactorId { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime EffectiveDate { get; set; }

        public DateTime CalculationDate { get; set; }
        public Factor Factor { get; set; }
        public decimal Value { get; set; }
    }
}