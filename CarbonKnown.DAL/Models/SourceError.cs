using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbonKnown.DAL.Models
{
    public class SourceError
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid DataSourceId { get; set; }
        public virtual SourceErrorType ErrorType { get; set; }
        public string ErrorMessage { get; set; }
    }
}
