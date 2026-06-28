using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.Validation;
using DataAccessObjects.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public sealed class CategoryDAO
    {
        private static CategoryDAO? _instance;
        private static readonly object _lock = new();

        private CategoryDAO() { }

        public static CategoryDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new CategoryDAO();
                    }
                }
                return _instance;
            }
        }

        public IQueryable<Category> GetQueryable(ExpenseDbContext context) =>
            context.Categories.AsNoTracking();

        public IQueryable<Category> ForPersonalUser(ExpenseDbContext context, int userId) =>
            CategoryScopeRules.ForPersonalUser(GetQueryable(context), userId);

        public IQueryable<Category> ForCorporateCompany(ExpenseDbContext context, int companyId) =>
            CategoryScopeRules.ForCorporateCompany(GetQueryable(context), companyId);

        public IQueryable<Category> ForPersonalUserManage(ExpenseDbContext context, int userId) =>
            CategoryScopeRules.ForPersonalUserManage(GetQueryable(context), userId);

        public async Task<Category?> GetByIdAsync(ExpenseDbContext context, int id) =>
            await context.Categories.FindAsync(id);

        public async Task<bool> NameExistsForPersonalUserAsync(
            ExpenseDbContext context, int userId, string name, int? excludeId = null)
        {
            var normalized = name.Trim();
            return await context.Categories.AnyAsync(c =>
                c.CompanyId == null &&
                c.OwnerUserId == userId &&
                c.Name == normalized &&
                (excludeId == null || c.Id != excludeId));
        }

        public async Task<bool> IsUsedByExpensesAsync(ExpenseDbContext context, int categoryId) =>
            await context.Expenses.AnyAsync(e => e.CategoryId == categoryId);

        public async Task<bool> NameExistsForCompanyAsync(
            ExpenseDbContext context, int companyId, string name, int? excludeId = null)
        {
            var normalized = name.Trim();
            return await context.Categories.AnyAsync(c =>
                c.CompanyId == companyId &&
                c.Name == normalized &&
                (excludeId == null || c.Id != excludeId));
        }

        public IQueryable<Category> ForCorporateCompanyManage(ExpenseDbContext context, int companyId) =>
            context.Categories.AsNoTracking().Where(c => c.CompanyId == companyId);

        public async Task AddAsync(ExpenseDbContext context, Category category)
        {
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExpenseDbContext context, Category category)
        {
            context.Categories.Update(category);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ExpenseDbContext context, Category category)
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
        }
    }
}
