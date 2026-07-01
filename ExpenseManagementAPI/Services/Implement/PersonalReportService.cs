using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class PersonalReportService : IPersonalReportService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IBudgetRepository _budgetRepository;

        public PersonalReportService(IExpenseRepository expenseRepository, IBudgetRepository budgetRepository)
        {
            _expenseRepository = expenseRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task<MonthlySummaryResponse> GetMonthlySummaryAsync(int userId, int month, int year)
        {
            var expenses = FilterByMonth(_expenseRepository.GetForPersonalUser(userId), month, year);
            var total = await expenses.SumAsync(e => e.Amount);
            var count = await expenses.CountAsync();
            return new MonthlySummaryResponse
            {
                Month = month,
                Year = year,
                TotalSpent = total,
                ExpenseCount = count
            };
        }

        public async Task<List<CategoryReportItem>> GetByCategoryAsync(int userId, int month, int year)
        {
            var expenses = FilterByMonth(_expenseRepository.GetForPersonalUser(userId), month, year);
            return await expenses
                .Where(e => e.CategoryId != null)
                .GroupBy(e => new { e.CategoryId, Name = e.Category!.Name })
                .Select(g => new CategoryReportItem
                {
                    CategoryId = g.Key.CategoryId!.Value,
                    CategoryName = g.Key.Name,
                    TotalSpent = g.Sum(e => e.Amount),
                    ExpenseCount = g.Count()
                })
                .OrderByDescending(x => x.TotalSpent)
                .ToListAsync();
        }

        public async Task<List<WalletReportItem>> GetByWalletAsync(int userId, int month, int year)
        {
            var expenses = FilterByMonth(_expenseRepository.GetForPersonalUser(userId), month, year);
            return await expenses
                .Where(e => e.WalletId != null)
                .GroupBy(e => new { e.WalletId, Name = e.Wallet!.Name })
                .Select(g => new WalletReportItem
                {
                    WalletId = g.Key.WalletId!.Value,
                    WalletName = g.Key.Name,
                    TotalSpent = g.Sum(e => e.Amount),
                    ExpenseCount = g.Count()
                })
                .OrderByDescending(x => x.TotalSpent)
                .ToListAsync();
        }

        public async Task<List<BudgetStatusItem>> GetBudgetStatusAsync(int userId, int month, int year)
        {
            var budgets = _budgetRepository.GetForPersonalUser(userId)
                .Where(b => b.Month == month && b.Year == year);

            return await budgets.Select(b => new BudgetStatusItem
            {
                BudgetId = b.Id,
                BudgetName = b.Name,
                CategoryName = b.Category != null ? b.Category.Name : null,
                Month = b.Month ?? month,
                Year = b.Year ?? year,
                LimitAmount = b.Details.Sum(d => d.LimitAmount),
                CarryOverDebt = b.Details.Sum(d => d.CarryOverDebt),
                SpentAmount = b.Details.Sum(d => d.CurrentAmount),
                RemainingAmount = b.Details.Sum(d => d.LimitAmount - d.CarryOverDebt - d.CurrentAmount),
                IsExceeded = b.Details.Sum(d => d.CurrentAmount) >
                    b.Details.Sum(d => d.LimitAmount - d.CarryOverDebt)
            }).ToListAsync();
        }

        public async Task<YearlyReportResponse> GetYearlyReportAsync(int userId, int year)
        {
            var expenses = _expenseRepository.GetForPersonalUser(userId)
                .Where(e => e.ExpenseDate != null && e.ExpenseDate.Value.Year == year);

            var byCategory = await expenses
                .Where(e => e.CategoryId != null)
                .GroupBy(e => new { e.CategoryId, Name = e.Category!.Name })
                .Select(g => new CategoryReportItem
                {
                    CategoryId = g.Key.CategoryId!.Value,
                    CategoryName = g.Key.Name,
                    TotalSpent = g.Sum(e => e.Amount),
                    ExpenseCount = g.Count()
                })
                .OrderByDescending(x => x.TotalSpent)
                .ToListAsync();

            return new YearlyReportResponse
            {
                Year = year,
                TotalSpent = byCategory.Sum(x => x.TotalSpent),
                ByCategory = byCategory
            };
        }

        private static IQueryable<BusinessObjects.Models.Expense> FilterByMonth(
            IQueryable<BusinessObjects.Models.Expense> query, int month, int year) =>
            query.Where(e =>
                e.ExpenseDate != null &&
                e.ExpenseDate.Value.Month == month &&
                e.ExpenseDate.Value.Year == year);
    }
}
