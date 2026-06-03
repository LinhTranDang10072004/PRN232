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

        public Task<List<Category>> GetByBranchAsync(CategoryBranch branch) =>
            _dao.GetByBranchAsync(_context, branch);

        public Task<Category?> GetByIdAsync(int id) =>
            _dao.GetByIdAsync(_context, id);
    }
}
