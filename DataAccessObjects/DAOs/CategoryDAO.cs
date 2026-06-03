using BusinessObjects.Enums;
using BusinessObjects.Models;
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

        /// <summary>Danh mục Personal: hệ thống + do chính user tạo.</summary>
        public IQueryable<Category> ForPersonalUser(ExpenseDbContext context, int userId) =>
            GetQueryable(context).Where(c =>
                c.Branch == CategoryBranch.Personal &&
                (c.OwnerUserId == null || c.OwnerUserId == userId));

        public async Task<List<Category>> GetByBranchAsync(ExpenseDbContext context, CategoryBranch branch) =>
            await context.Categories
                .Where(c => c.Branch == branch)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Category?> GetByIdAsync(ExpenseDbContext context, int id) =>
            await context.Categories.FindAsync(id);

        public async Task<bool> NameExistsForUserAsync(
            ExpenseDbContext context, int userId, string name, int? excludeId = null)
        {
            var normalized = name.Trim();
            return await context.Categories.AnyAsync(c =>
                c.Branch == CategoryBranch.Personal &&
                c.OwnerUserId == userId &&
                c.Name == normalized &&
                (excludeId == null || c.Id != excludeId));
        }

        public async Task<bool> IsUsedByExpensesAsync(ExpenseDbContext context, int categoryId) =>
            await context.Expenses.AnyAsync(e => e.CategoryId == categoryId);

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
