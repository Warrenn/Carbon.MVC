using System.Collections.Generic;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.MVC.Models
{
    public class EnterDataModel
    {
        public bool ManualEntry { get; set; }
        public DataEntry EntryData { get; set; }
        public string ViewName { get; set; }
        public string ReferenceNotes { get; set; }
        public IEnumerable<Variance> Variance { get; set; }
        public IEnumerable<string> EntryErrors { get; set; }
        public bool CanRevert { get; set; }
        public bool CanEdit { get; set; }
    }
}