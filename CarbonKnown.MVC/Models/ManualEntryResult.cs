using System;
using System.Collections.Generic;

namespace CarbonKnown.MVC.Models
{
    public class ManualEntryResult
    {
        public Guid SourceId { get; set; }
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; } 
    }
}