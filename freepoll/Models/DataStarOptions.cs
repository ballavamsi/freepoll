namespace freepoll.Models
{
    public partial class DataStarOptions
    {
        public int StarOptionId { get; set; }
        public string OptionText { get; set; }
        public string OptionDisplayText { get; set; }
        public int? DisplayOrder { get; set; }
        public int? IsActive { get; set; }
    }
}