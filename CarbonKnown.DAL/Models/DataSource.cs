using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbonKnown.DAL.Models
{
    public class DataSource
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string UserName { get; set; }
        public DateTime DateEdit { get; set; }
        public virtual SourceStatus InputStatus { get; set; }
        public virtual ICollection<SourceError> SourceErrors { get; set; }
        public virtual ICollection<DataEntry> DataEntries { get; set; }
        public string ReferenceNotes { get; set; }

        public Type NonProxyType()
        {
            var entityType = GetType();
            if ((entityType.BaseType != null) && (entityType.Namespace == "System.Data.Entity.DynamicProxies"))
            {
                return entityType.BaseType;
            }
            return entityType;
        }

        public string SourceType
        {
            get { return NonProxyType().Name; }
            set { }
        }
    }
}