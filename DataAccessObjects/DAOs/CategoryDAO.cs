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

        public async Task<List<Category>> GetByBranchAsync(ExpenseDbContext context, CategoryBranch branch) =>
            await context.Categories
                .Where(c => c.Branch == branch)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Category?> GetByIdAsync(ExpenseDbContext context, int id) =>
            await context.Categories.FindAsync(id);
    }
}
