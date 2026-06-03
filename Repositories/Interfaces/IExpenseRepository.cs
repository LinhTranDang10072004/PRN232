using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        IQueryable<Expense> GetForUser(int userId);
        IQueryable<Expense> GetForStaff(int staffId);
        IQueryable<Expense> GetForAdmin(int adminId);
        Task<Expense?> GetByIdAsync(int id);
        Task AddAsync(Expense expense);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(Expense expense);
        Task<bool> ApproveAsync(int expenseId, int adminId);
        Task<bool> RejectAsync(int expenseId, int adminId, string reason);
    }
}
