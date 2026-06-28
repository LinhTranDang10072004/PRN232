using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        IQueryable<Expense> GetForPersonalUser(int userId);
        IQueryable<Expense> GetForStaff(int staffId);
        IQueryable<Expense> GetForAdminCompany(int companyId);
        Task<Expense?> GetByIdForPersonalUserAsync(int userId, int id);
        Task<Expense?> GetByIdForStaffAsync(int staffId, int id);
        Task<Expense?> GetByIdTrackedAsync(int id);
        Task AddAsync(Expense expense);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(Expense expense);
        Task<Expense?> GetByIdForAdminAsync(int companyId, int expenseId);
        Task<(bool Success, string? Error, int? StaffUserId)> ApproveForAdminAsync(int adminId, int companyId, int expenseId);
        Task<(bool Success, string? Error, int? StaffUserId)> RejectForAdminAsync(int adminId, int companyId, int expenseId, string comment);
    }
}
