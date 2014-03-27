namespace CarbonKnown.DAL.Models.Source
{
    public class FileDataSource : DataSource
    {
        public string FileHash { get; set; }
        public string OriginalFileName { get; set; }
        public string CurrentFileName { get; set; }
        public string HandlerName { get; set; }
        public string MediaType { get; set; }
    }
}
