using ExpenseManagementAPI.DTOs.Personal;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IPersonalBudgetService
    {
        IQueryable<BudgetResponse> GetForUser(int userId, int? month = null, int? year = null);
        Task<(bool Success, BudgetResponse? Data, string? Error)> GetByIdAsync(int userId, int id);
        Task<(bool Success, BudgetResponse? Data, string? Error)> CreateAsync(int userId, CreateBudgetRequest request);
        Task<(bool Success, BudgetResponse? Data, string? Error)> UpdateLimitAsync(int userId, int id, UpdateBudgetLimitRequest request);
    }
}
