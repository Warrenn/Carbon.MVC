using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarbonKnown.DAL.Models.Source
{
    public class FeedDataSource : DataSource
    {
        public string SourceUrl { get; set; }
        public string ScriptPath { get; set; }
        public string HandlerName { get; set; }
    }
}
