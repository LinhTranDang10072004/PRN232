namespace ClientMVC.Models.Shared
{
    public class CalendarFilterModel
    {
        public string? FilterKeyword { get; set; }
        public int? FilterCategoryId { get; set; }
        public string? FilterStatus { get; set; }
        public decimal? FilterMinAmount { get; set; }
    }
}
