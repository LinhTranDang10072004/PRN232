using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetForPersonalUser(int userId);
        Task<List<Category>> GetByBranchAsync(CategoryBranch branch);
        Task<Category?> GetByIdAsync(int id);
        Task<bool> NameExistsForUserAsync(int userId, string name, int? excludeId = null);
        Task<bool> IsUsedByExpensesAsync(int categoryId);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}
