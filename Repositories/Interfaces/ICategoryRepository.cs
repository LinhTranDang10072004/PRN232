using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetForPersonalUser(int userId);
        IQueryable<Category> GetForPersonalUserManage(int userId);
        IQueryable<Category> GetForCorporateCompany(int companyId);
        Task<Category?> GetByIdAsync(int id);
        Task<bool> NameExistsForPersonalUserAsync(int userId, string name, int? excludeId = null);
        Task<bool> IsUsedByExpensesAsync(int categoryId);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}
