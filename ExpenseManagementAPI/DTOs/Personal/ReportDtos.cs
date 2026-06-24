namespace ExpenseManagementAPI.DTOs.Personal
{
    public class MonthlySummaryResponse
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalSpent { get; set; }
        public int ExpenseCount { get; set; }
    }

    public class CategoryReportItem
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public decimal TotalSpent { get; set; }
        public int ExpenseCount { get; set; }
    }

    public class WalletReportItem
    {
        public int WalletId { get; set; }
        public string WalletName { get; set; } = null!;
        public decimal TotalSpent { get; set; }
        public int ExpenseCount { get; set; }
    }

    public class BudgetStatusItem
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; } = null!;
        public string? CategoryName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal LimitAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsExceeded { get; set; }
    }

    public class YearlyReportResponse
    {
        public int Year { get; set; }
        public decimal TotalSpent { get; set; }
        public List<CategoryReportItem> ByCategory { get; set; } = new();
    }

    public class NotificationResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
