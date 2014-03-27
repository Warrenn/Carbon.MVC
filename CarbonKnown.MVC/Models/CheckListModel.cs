namespace CarbonKnown.MVC.Models
{
    public class CheckListModel
    {
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }
        public int MinYearInRange { get; set; }
        public int MaxYearInRange { get; set; }
    }
}