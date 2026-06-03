using ExpenseManagementAPI.DTOs.Personal;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IPersonalExpenseService
    {
        IQueryable<ExpenseResponse> GetMyExpenses(int userId);
        Task<ExpenseResponse?> GetByIdAsync(int userId, int expenseId);
        Task<(ExpenseResponse? Result, string? Error)> CreateAsync(int userId, CreateExpenseRequest request);
        Task<(ExpenseResponse? Result, string? Error)> UpdateAsync(int userId, int expenseId, UpdateExpenseRequest request);
        Task<(bool Success, string? Error)> DeleteAsync(int userId, int expenseId);
    }
}
