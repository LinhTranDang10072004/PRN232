using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public sealed class ExpenseDAO
    {
        private static ExpenseDAO? _instance;
        private static readonly object _lock = new();

        private ExpenseDAO() { }

        public static ExpenseDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new ExpenseDAO();
                    }
                }
                return _instance;
            }
        }

        public IQueryable<Expense> GetQueryable(ExpenseDbContext context) =>
            context.Expenses
                .Include(e => e.Category)
                .Include(e => e.User)
                .Include(e => e.Wallet)
                .Include(e => e.Account)
                .AsNoTracking();

        public async Task<Expense?> GetByIdForStaffAsync(ExpenseDbContext context, int staffId, int id) =>
            await context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Account)
                .Include(e => e.ApprovalHistories)
                    .ThenInclude(h => h.Admin)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == staffId);

        public async Task<Expense?> GetByIdAsync(ExpenseDbContext context, int id) =>
            await context.Expenses
                .Include(e => e.Category)
                .Include(e => e.User)
                .Include(e => e.Wallet)
                .FirstOrDefaultAsync(e => e.Id == id);

        public async Task<Expense?> GetByIdForAdminAsync(ExpenseDbContext context, int companyId, int expenseId) =>
            await context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Account)
                .Include(e => e.User)
                .Include(e => e.ApprovalHistories)
                    .ThenInclude(h => h.Admin)
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.Id == expenseId &&
                    e.User != null &&
                    e.User.Role == UserRole.CompanyStaff &&
                    e.User.CompanyId == companyId);

        public async Task AddAsync(ExpenseDbContext context, Expense expense)
        {
            await context.Expenses.AddAsync(expense);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExpenseDbContext context, Expense expense)
        {
            context.Expenses.Update(expense);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ExpenseDbContext context, Expense expense)
        {
            context.Expenses.Remove(expense);
            await context.SaveChangesAsync();
        }

        /// <summary>Nhánh 1: chỉ chi tiêu của chính User.</summary>
        public IQueryable<Expense> ForPersonalUser(ExpenseDbContext context, int userId) =>
            GetQueryable(context).Where(e => e.UserId == userId);

        /// <summary>Nhánh 2 Staff: chi tiêu của chính Staff.</summary>
        public IQueryable<Expense> ForStaff(ExpenseDbContext context, int staffId) =>
            GetQueryable(context).Where(e => e.UserId == staffId);

        /// <summary>Nhánh 2 Admin: chi tiêu của mọi Staff cùng CompanyId.</summary>
        public IQueryable<Expense> ForAdminCompany(ExpenseDbContext context, int companyId) =>
            GetQueryable(context).Where(e =>
                e.User != null &&
                e.User.Role == UserRole.CompanyStaff &&
                e.User.CompanyId == companyId);
    }
}
