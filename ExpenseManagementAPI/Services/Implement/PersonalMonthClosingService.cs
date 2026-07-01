using BusinessObjects.Models;
using BusinessObjects.Validation;
using DataAccessObjects.Context;
using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class PersonalMonthClosingService : IPersonalMonthClosingService
    {
        private readonly ExpenseDbContext _context;
        private readonly IMonthClosingRepository _monthClosingRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IExpenseRepository _expenseRepository;

        public PersonalMonthClosingService(
            ExpenseDbContext context,
            IMonthClosingRepository monthClosingRepository,
            IBudgetRepository budgetRepository,
            IWalletRepository walletRepository,
            IExpenseRepository expenseRepository)
        {
            _context = context;
            _monthClosingRepository = monthClosingRepository;
            _budgetRepository = budgetRepository;
            _walletRepository = walletRepository;
            _expenseRepository = expenseRepository;
        }

        public async Task<MonthClosingPreviewResponse> GetClosingPreviewAsync(int userId, int month, int year)
        {
            var existing = await _monthClosingRepository.GetForMonthAsync(userId, month, year);
            var budgets = await BuildBudgetItemsAsync(userId, month, year);
            var wallets = await _walletRepository.GetForPersonalUser(userId)
                .Select(w => new MonthClosingWalletItem
                {
                    WalletId = w.Id,
                    WalletName = w.Name,
                    Balance = w.Balance
                })
                .ToListAsync();

            var expenses = FilterExpensesByMonth(_expenseRepository.GetForPersonalUser(userId), month, year);
            var expenseCount = await expenses.CountAsync();
            var totalSpent = await expenses.SumAsync(e => e.Amount);

            var totalLimit = budgets.Sum(b => b.LimitAmount);
            var totalSurplus = budgets.Sum(b => b.SurplusAmount);
            var totalOverflow = budgets.Sum(b => b.OverflowAmount);

            var (canClose, reason) = await ValidateCanCloseAsync(userId, month, year, existing != null);

            return new MonthClosingPreviewResponse
            {
                Month = month,
                Year = year,
                IsClosed = existing != null,
                ClosedAt = existing?.ClosedAt,
                ExpenseCount = expenseCount,
                TotalSpent = totalSpent,
                TotalBudgetLimit = totalLimit,
                TotalSurplus = totalSurplus,
                TotalOverflow = totalOverflow,
                BudgetCount = budgets.Count,
                Budgets = budgets,
                Wallets = wallets,
                CanClose = canClose,
                CannotCloseReason = reason,
                PreviousMonthNotClosed = await ShouldWarnPreviousMonthNotClosedAsync(userId, month, year)
            };
        }

        public async Task<(bool Success, MonthClosingResponse? Data, string? Error)> CloseMonthAsync(
            int userId, int month, int year, string? notes = null)
        {
            if (await _monthClosingRepository.ExistsForMonthAsync(userId, month, year))
                return (false, null, "Tháng này đã được chốt sổ.");

            var (canClose, reason) = await ValidateCanCloseAsync(userId, month, year, isAlreadyClosed: false);
            if (!canClose)
                return (false, null, reason);

            var budgets = await _budgetRepository.GetForMonthTrackedAsync(userId, month, year);
            if (budgets.Count == 0)
                return (false, null, "Không có budget nào trong tháng này để chốt sổ.");

            var budgetItems = MapBudgetItems(budgets);
            var expenses = FilterExpensesByMonth(_expenseRepository.GetForPersonalUser(userId), month, year);
            var totalSpent = await expenses.SumAsync(e => e.Amount);
            var totalLimit = budgetItems.Sum(b => b.LimitAmount);
            var totalSurplus = budgetItems.Sum(b => b.SurplusAmount);

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var closing = new MonthClosing
                {
                    UserId = userId,
                    Month = month,
                    Year = year,
                    ClosedAt = DateTime.UtcNow,
                    TotalSpent = totalSpent,
                    TotalBudgetLimit = totalLimit,
                    TotalSurplus = totalSurplus,
                    BudgetCount = budgets.Count,
                    Notes = notes?.Trim(),
                    Status = "Closed"
                };

                await _monthClosingRepository.AddAsync(closing);

                foreach (var budget in budgets)
                {
                    budget.Status = "Closed";
                    budget.UpdatedAt = DateTime.UtcNow;
                    await _budgetRepository.UpdateAsync(budget);
                }

                await tx.CommitAsync();

                return (true, new MonthClosingResponse
                {
                    Id = closing.Id,
                    Month = closing.Month,
                    Year = closing.Year,
                    ClosedAt = closing.ClosedAt,
                    TotalSpent = closing.TotalSpent,
                    TotalBudgetLimit = closing.TotalBudgetLimit,
                    TotalSurplus = closing.TotalSurplus,
                    BudgetCount = closing.BudgetCount,
                    Notes = closing.Notes,
                    Status = closing.Status
                }, null);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<MonthClosingStatusResponse> GetClosingStatusAsync(int userId, int month, int year)
        {
            var existing = await _monthClosingRepository.GetForMonthAsync(userId, month, year);
            return new MonthClosingStatusResponse
            {
                Month = month,
                Year = year,
                IsClosed = existing != null,
                ClosedAt = existing?.ClosedAt
            };
        }

        public async Task<bool> ShouldWarnPreviousMonthNotClosedAsync(int userId, int month, int year)
        {
            var (prevMonth, prevYear) = PersonalBudgetRules.PreviousMonth(month, year);
            var hasPrevBudgets = await _budgetRepository.GetForPersonalUser(userId)
                .AnyAsync(b => b.Month == prevMonth && b.Year == prevYear);
            if (!hasPrevBudgets)
                return false;

            return !await _monthClosingRepository.ExistsForMonthAsync(userId, prevMonth, prevYear);
        }

        private async Task<List<MonthClosingBudgetItem>> BuildBudgetItemsAsync(int userId, int month, int year)
        {
            var budgets = await _budgetRepository.GetForPersonalUser(userId)
                .Where(b => b.Month == month && b.Year == year)
                .ToListAsync();
            return MapBudgetItems(budgets);
        }

        private static List<MonthClosingBudgetItem> MapBudgetItems(IEnumerable<Budget> budgets) =>
            budgets.Select(b =>
            {
                var limit = b.Details.Sum(d => d.LimitAmount);
                var carryOver = b.Details.Sum(d => d.CarryOverDebt);
                var spent = b.Details.Sum(d => d.CurrentAmount);
                var remaining = PersonalBudgetRules.Remaining(limit, carryOver, spent);
                var surplus = PersonalBudgetRules.SurplusAmount(limit, carryOver, spent);
                var overflow = b.Details.Sum(d =>
                    PersonalBudgetRules.OverflowAmount(d.LimitAmount, d.CurrentAmount));
                var isExceeded = PersonalBudgetRules.IsExceeded(limit, carryOver, spent);

                return new MonthClosingBudgetItem
                {
                    BudgetId = b.Id,
                    BudgetName = b.Name,
                    CategoryName = b.Category?.Name,
                    LimitAmount = limit,
                    CarryOverDebt = carryOver,
                    SpentAmount = spent,
                    RemainingAmount = remaining,
                    SurplusAmount = surplus,
                    OverflowAmount = overflow,
                    IsExceeded = isExceeded,
                    Outcome = PersonalBudgetRules.CloseOutcome(limit, carryOver, spent),
                    Status = b.Status
                };
            }).ToList();

        private async Task<(bool CanClose, string? Reason)> ValidateCanCloseAsync(
            int userId, int month, int year, bool isAlreadyClosed)
        {
            if (isAlreadyClosed)
                return (false, "Tháng này đã được chốt sổ.");

            var today = DateTime.Today;
            if (year > today.Year || (year == today.Year && month > today.Month))
                return (false, "Không thể chốt sổ cho tháng trong tương lai.");

            return (true, null);
        }

        private static IQueryable<Expense> FilterExpensesByMonth(
            IQueryable<Expense> query, int month, int year) =>
            query.Where(e =>
                e.ExpenseDate != null &&
                e.ExpenseDate.Value.Month == month &&
                e.ExpenseDate.Value.Year == year);
    }
}
