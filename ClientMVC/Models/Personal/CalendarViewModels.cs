namespace ClientMVC.Models.Personal
{
    public class CalendarMonthViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthLabel { get; set; } = null!;
        public List<CalendarWeekViewModel> Weeks { get; set; } = new();
        public decimal MonthTotal { get; set; }
        public int ExpenseCount { get; set; }
        public List<ExpenseDto> SelectedDayExpenses { get; set; } = new();
        public int? SelectedDay { get; set; }
    }

    public class CalendarWeekViewModel
    {
        public List<CalendarDayViewModel> Days { get; set; } = new();
    }

    public class CalendarDayViewModel
    {
        public int? Day { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public decimal DayTotal { get; set; }
        public List<ExpenseDto> Expenses { get; set; } = new();
    }

    public class DashboardViewModel
    {
        public MonthlySummaryDto? Summary { get; set; }
        public List<BudgetStatusDto> Budgets { get; set; } = new();
        public List<CategoryReportDto> TopCategories { get; set; } = new();
        public int UnreadNotifications { get; set; }
    }
}
