using System.ComponentModel.DataAnnotations;

namespace ExpenseManagementAPI.DTOs.Personal
{
    public class CloseMonthRequest
    {
        [Range(1, 12)]
        public int Month { get; set; }

        [Range(2000, 2100)]
        public int Year { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class MonthClosingBudgetItem
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

    public class MonthClosingWalletItem
    {
        public int WalletId { get; set; }
        public string WalletName { get; set; } = null!;
        public decimal Balance { get; set; }
    }

    public class MonthClosingPreviewResponse
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
        public List<MonthClosingBudgetItem> Budgets { get; set; } = new();
        public List<MonthClosingWalletItem> Wallets { get; set; } = new();
        public bool CanClose { get; set; }
        public string? CannotCloseReason { get; set; }
        public bool PreviousMonthNotClosed { get; set; }
    }

    public class MonthClosingStatusResponse
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosedAt { get; set; }
    }

    public class MonthClosingResponse
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
