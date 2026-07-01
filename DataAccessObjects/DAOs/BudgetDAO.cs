using BusinessObjects.Models;
using DataAccessObjects.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public sealed class BudgetDAO
    {
        private static BudgetDAO? _instance;
        private static readonly object _lock = new();

        private BudgetDAO() { }

        public static BudgetDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                        _instance ??= new BudgetDAO();
                }
                return _instance;
            }
        }

        public IQueryable<Budget> ForPersonalUser(ExpenseDbContext context, int userId) =>
            context.Budgets
                .Include(b => b.Category)
                .Include(b => b.Details)
                .Where(b => b.UserId == userId)
                .AsNoTracking();

        public async Task<Budget?> GetByIdWithDetailsAsync(ExpenseDbContext context, int id) =>
            await context.Budgets
                .Include(b => b.Category)
                .Include(b => b.Details)
                .FirstOrDefaultAsync(b => b.Id == id);

        public async Task<Budget?> FindForMonthAsync(
            ExpenseDbContext context, int userId, int categoryId, int month, int year) =>
            await context.Budgets
                .Include(b => b.Details)
                .FirstOrDefaultAsync(b =>
                    b.UserId == userId &&
                    b.CategoryId == categoryId &&
                    b.Month == month &&
                    b.Year == year);

        public async Task<Budget?> FindForMonthAndWalletAsync(
            ExpenseDbContext context, int userId, int categoryId, int walletId, int month, int year) =>
            await context.Budgets
                .Include(b => b.Details)
                .FirstOrDefaultAsync(b =>
                    b.UserId == userId &&
                    b.CategoryId == categoryId &&
                    b.WalletId == walletId &&
                    b.Month == month &&
                    b.Year == year);

        public async Task<bool> ExistsForMonthAsync(
            ExpenseDbContext context, int userId, int categoryId, int month, int year, int? excludeId = null) =>
            await context.Budgets.AnyAsync(b =>
                b.UserId == userId &&
                b.CategoryId == categoryId &&
                b.Month == month &&
                b.Year == year &&
                (excludeId == null || b.Id != excludeId));

        public async Task AddAsync(ExpenseDbContext context, Budget budget)
        {
            await context.Budgets.AddAsync(budget);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExpenseDbContext context, Budget budget)
        {
            context.Budgets.Update(budget);
            await context.SaveChangesAsync();
        }

        public async Task<BudgetDetail?> GetDetailByIdAsync(ExpenseDbContext context, int detailId) =>
            await context.BudgetDetails
                .Include(d => d.Budget)
                .FirstOrDefaultAsync(d => d.Id == detailId);

        public async Task UpdateDetailAsync(ExpenseDbContext context, BudgetDetail detail)
        {
            context.BudgetDetails.Update(detail);
            await context.SaveChangesAsync();
        }
    }
}
