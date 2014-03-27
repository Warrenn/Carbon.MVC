using System;
using System.Collections.Generic;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.MVC.Models
{
    public class EditSourceModel
    {
        public Guid SourceId { get; set; }
        public string Name { get; set; }
        public string CurrentFileName { get; set; }
        public DateTime EditDate { get; set; }
        public string Type { get; set; }
        public string UserName { get; set; }
        public SourceStatus SourceStatus { get; set; }
        public string Comment { get; set; }
        public int DataEntries { get; set; }
        public int DataEntriesInError { get; set; }
        public IDictionary<DataErrorType, int> ErrorTypeCount { get; set; }
        public IDictionary<CarbonKnown.DAL.Models.Calculation, int> Calculations { get; set; }
        public bool SourceContainsErrors { get; set; }
        public IEnumerable<SourceError> SourceErrors { get; set; } 
    }
}