using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        IQueryable<Expense> GetForPersonalUser(int userId);
        Task<Expense?> GetByIdForPersonalUserAsync(int userId, int id);
        Task<Expense?> GetByIdTrackedAsync(int id);
        Task AddAsync(Expense expense);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(Expense expense);
    }
}
