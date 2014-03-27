using System.Collections.Generic;

namespace CarbonKnown.MVC.Models
{
    // ReSharper disable InconsistentNaming
    public class DataTableResultModel
    {
        public string sEcho { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public IEnumerable<object[]> aaData { get; set; }
    }

    // ReSharper restore InconsistentNaming
}