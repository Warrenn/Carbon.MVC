namespace CarbonKnown.MVC.Models
{
    public class UrlSelectionModel
    {
        public int StepNumber { get; set; }
        public string Label { get; set; }
        public bool CanEdit { get; set; }
        public string Name { get; set; }
        public string IdKey { get; set; }
        public string NameKey { get; set; }
        public string SearchFunction { get; set; }
        public string Url { get; set; }
        public string InitialValue { get; set; }
        public int? MinInputLength { get; set; }
    }
}