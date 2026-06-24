using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IBudgetRepository
    {
        IQueryable<Budget> GetForPersonalUser(int userId);
        Task<Budget?> GetByIdForUserAsync(int userId, int id);
        Task<Budget?> FindForMonthAsync(int userId, int categoryId, int month, int year);
        Task<bool> ExistsForMonthAsync(int userId, int categoryId, int month, int year, int? excludeId = null);
        Task AddAsync(Budget budget);
        Task UpdateAsync(Budget budget);
        Task<BudgetDetail?> GetDetailByIdAsync(int detailId);
        Task UpdateDetailAsync(BudgetDetail detail);
    }
}
