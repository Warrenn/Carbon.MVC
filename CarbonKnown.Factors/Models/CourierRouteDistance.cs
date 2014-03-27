using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbonKnown.Factors.Models
{
    public class CourierRouteDistance
    {
        [Key]
        [Column(Order = 0)]
        public string Code1 { get; set; }

        [Key]
        [Column(Order = 1)]
        public string Code2 { get; set; }

        public DateTime? CalculationDate { get; set; }
        public decimal Distance { get; set; }
    }
}
