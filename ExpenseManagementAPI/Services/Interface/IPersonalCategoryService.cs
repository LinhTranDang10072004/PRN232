using ExpenseManagementAPI.DTOs.Personal;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IPersonalCategoryService
    {
        IQueryable<CategoryResponse> GetMyCategories(int userId);
        Task<CategoryResponse?> GetByIdAsync(int userId, int categoryId);
        Task<(CategoryResponse? Result, string? Error)> CreateAsync(int userId, CreateCategoryRequest request);
        Task<(CategoryResponse? Result, string? Error)> UpdateAsync(int userId, int categoryId, UpdateCategoryRequest request);
        Task<(bool Success, string? Error)> DeleteAsync(int userId, int categoryId);
        Task<bool> CanUserUseCategoryAsync(int userId, int categoryId);
    }
}
