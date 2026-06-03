using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects.Context;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ExpenseDbContext _context;
        private readonly CategoryDAO _dao = CategoryDAO.Instance;

        public CategoryRepository(ExpenseDbContext context)
        {
            _context = context;
        }

        public IQueryable<Category> GetForPersonalUser(int userId) =>
            _dao.ForPersonalUser(_context, userId);

        public Task<List<Category>> GetByBranchAsync(CategoryBranch branch) =>
            _dao.GetByBranchAsync(_context, branch);

        public Task<Category?> GetByIdAsync(int id) =>
            _dao.GetByIdAsync(_context, id);

        public Task<bool> NameExistsForUserAsync(int userId, string name, int? excludeId = null) =>
            _dao.NameExistsForUserAsync(_context, userId, name, excludeId);

        public Task<bool> IsUsedByExpensesAsync(int categoryId) =>
            _dao.IsUsedByExpensesAsync(_context, categoryId);

        public Task AddAsync(Category category) =>
            _dao.AddAsync(_context, category);

        public Task UpdateAsync(Category category) =>
            _dao.UpdateAsync(_context, category);

        public Task DeleteAsync(Category category) =>
            _dao.DeleteAsync(_context, category);
    }
}
