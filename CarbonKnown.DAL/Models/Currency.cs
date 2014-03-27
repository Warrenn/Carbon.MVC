using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbonKnown.DAL.Models
{
    public class Currency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(3)]
        public string Code { get; set; }

        [StringLength(1)]
        public string Symbol { get; set; }

        public string Name { get; set; }

        [StringLength(5)]
        public string Locale { get; set; }
    }
}
