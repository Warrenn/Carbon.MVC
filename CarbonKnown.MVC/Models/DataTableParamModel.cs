using System.Collections.Generic;

namespace CarbonKnown.MVC.Models
{
    // ReSharper disable InconsistentNaming
    public class DataTableParamModel
    {
        public string sEcho { get; set; }
        public string sSearch { get; set; }
        public int iDisplayLength { get; set; }
        public int iDisplayStart { get; set; }
        public int iColumns { get; set; }
        public int iSortingCols { get; set; }
        public string sColumns { get; set; }
        public IEnumerable<DataTableSortModel> SortCol { get; set; }
    }

    // ReSharper restore InconsistentNaming
}