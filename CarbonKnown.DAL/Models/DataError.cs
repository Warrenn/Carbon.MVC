using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbonKnown.DAL.Models
{
    public class DataError
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual DataEntry DataEntry { get; set; }
        public Guid DataEntryId { get; set; }
        public string Column { get; set; }
        public virtual DataErrorType ErrorType { get; set; }
        public string Message { get; set; }
    }
}
