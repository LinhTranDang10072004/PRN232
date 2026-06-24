using ExpenseManagementAPI.DTOs.Personal;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IPersonalExpenseService
    {
        IQueryable<ExpenseResponse> GetForUser(int userId);
        Task<(bool Success, ExpenseResponse? Data, string? Error)> GetByIdAsync(int userId, int id);
        Task<(bool Success, ExpenseResponse? Data, string? Error)> CreateAsync(int userId, ExpenseRequest request);
        Task<(bool Success, ExpenseResponse? Data, string? Error)> UpdateAsync(int userId, int id, ExpenseRequest request);
        Task<(bool Success, string? Error)> DeleteAsync(int userId, int id);
    }
}
