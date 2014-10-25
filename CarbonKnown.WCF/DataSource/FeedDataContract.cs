using System;

namespace CarbonKnown.WCF.DataSource
{
    public class FeedDataContract
    {
        public Guid? SourceId { get; set; }
        public string ReferenceNotes { get; set; }
        public string OriginalFileName { get; set; }
        public string HandlerName { get; set; }
        public string MediaType { get; set; }
        public string UserName { get; set; }
        public string SourceUrl { get; set; }
        public string ScriptPath { get; set; }
    }
}