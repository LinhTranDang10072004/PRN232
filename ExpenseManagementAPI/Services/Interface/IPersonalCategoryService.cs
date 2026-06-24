using ExpenseManagementAPI.DTOs.Personal;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IPersonalCategoryService
    {
        IQueryable<CategoryResponse> GetForUser(int userId);
        Task<(bool Success, CategoryResponse? Data, string? Error)> GetByIdAsync(int userId, int id);
        Task<(bool Success, CategoryResponse? Data, string? Error)> CreateAsync(int userId, CategoryRequest request);
        Task<(bool Success, CategoryResponse? Data, string? Error)> UpdateAsync(int userId, int id, CategoryRequest request);
        Task<(bool Success, string? Error)> DeactivateAsync(int userId, int id);
    }
}
