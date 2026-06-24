using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        IQueryable<Expense> GetForPersonalUser(int userId);
        IQueryable<Expense> GetForStaff(int staffId);
        Task<Expense?> GetByIdForPersonalUserAsync(int userId, int id);
        Task<Expense?> GetByIdForStaffAsync(int staffId, int id);
        Task<Expense?> GetByIdTrackedAsync(int id);
        Task AddAsync(Expense expense);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(Expense expense);
    }
}
