using BusinessObjects.Enums;

namespace ClientMVC.Models.Personal
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string Role { get; set; } = null!;
    }

    public class WalletDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Type { get; set; }
        public decimal Balance { get; set; }
        public string? Status { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public CategoryStatus Status { get; set; }
        public bool IsSystem { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }

    public class BudgetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public decimal LimitAmount { get; set; }
        public decimal CarryOverDebt { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsExceeded { get; set; }
    }

    public class ExpenseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public ExpenseStatus Status { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? WalletId { get; set; }
        public string? WalletName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool BudgetExceeded { get; set; }
        public decimal BudgetOverflowAmount { get; set; }
    }

    public class MonthlySummaryDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalSpent { get; set; }
        public int ExpenseCount { get; set; }
    }

    public class CategoryReportDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public decimal TotalSpent { get; set; }
        public int ExpenseCount { get; set; }
    }

    public class BudgetStatusDto
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; } = null!;
        public string? CategoryName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal LimitAmount { get; set; }
        public decimal CarryOverDebt { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsExceeded { get; set; }
    }

    public class YearlyReportDto
    {
        public int Year { get; set; }
        public decimal TotalSpent { get; set; }
        public List<CategoryReportDto> ByCategory { get; set; } = new();
    }

    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public bool IsRead { get; set; }
        public string Severity { get; set; } = "info";
        public DateTime CreatedAt { get; set; }
    }

    public class MonthClosingBudgetItemDto
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; } = null!;
        public string? CategoryName { get; set; }
        public decimal LimitAmount { get; set; }
        public decimal CarryOverDebt { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal SurplusAmount { get; set; }
        public decimal OverflowAmount { get; set; }
        public bool IsExceeded { get; set; }
        public string Outcome { get; set; } = null!;
        public string? Status { get; set; }
    }

    public class MonthClosingWalletItemDto
    {
        public int WalletId { get; set; }
        public string WalletName { get; set; } = null!;
        public decimal Balance { get; set; }
    }

    public class MonthClosingPreviewDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int ExpenseCount { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal TotalBudgetLimit { get; set; }
        public decimal TotalSurplus { get; set; }
        public decimal TotalOverflow { get; set; }
        public int BudgetCount { get; set; }
        public List<MonthClosingBudgetItemDto> Budgets { get; set; } = new();
        public List<MonthClosingWalletItemDto> Wallets { get; set; } = new();
        public bool CanClose { get; set; }
        public string? CannotCloseReason { get; set; }
        public bool PreviousMonthNotClosed { get; set; }
    }

    public class MonthClosingResultDto
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime ClosedAt { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal TotalBudgetLimit { get; set; }
        public decimal TotalSurplus { get; set; }
        public int BudgetCount { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = null!;
    }
}
