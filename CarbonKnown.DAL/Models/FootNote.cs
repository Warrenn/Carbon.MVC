using System.ComponentModel.DataAnnotations.Schema;

namespace CarbonKnown.DAL.Models
{
    public class FootNote
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual Census Census { get; set; }
        public int CensusId { get; set; }
        public string Comments { get; set; }
    }
}
