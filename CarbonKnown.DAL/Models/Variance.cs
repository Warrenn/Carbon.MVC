using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbonKnown.DAL.Models
{
    public class Variance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual Calculation Calculation { get; set; }
        public Guid CalculationId { get; set; }
        public string ColumnName { get; set; }
        public decimal MaxValue { get; set; }
        public decimal MinValue { get; set; }
    }
}
