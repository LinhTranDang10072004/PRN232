using BusinessObjects.Models;
using DataAccessObjects.Context;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ExpenseDbContext _context;

        public CategoryRepository(ExpenseDbContext context) => _context = context;

        public IQueryable<Category> GetForPersonalUser(int userId) =>
            CategoryDAO.Instance.ForPersonalUser(_context, userId);

        public IQueryable<Category> GetForPersonalUserManage(int userId) =>
            CategoryDAO.Instance.ForPersonalUserManage(_context, userId);

        public IQueryable<Category> GetForCorporateCompany(int companyId) =>
            CategoryDAO.Instance.ForCorporateCompany(_context, companyId);

        public Task<Category?> GetByIdAsync(int id) =>
            CategoryDAO.Instance.GetByIdAsync(_context, id);

        public Task<bool> NameExistsForPersonalUserAsync(int userId, string name, int? excludeId = null) =>
            CategoryDAO.Instance.NameExistsForPersonalUserAsync(_context, userId, name, excludeId);

        public Task<bool> IsUsedByExpensesAsync(int categoryId) =>
            CategoryDAO.Instance.IsUsedByExpensesAsync(_context, categoryId);

        public Task AddAsync(Category category) =>
            CategoryDAO.Instance.AddAsync(_context, category);

        public Task UpdateAsync(Category category) =>
            CategoryDAO.Instance.UpdateAsync(_context, category);

        public Task DeleteAsync(Category category) =>
            CategoryDAO.Instance.DeleteAsync(_context, category);
    }
}
